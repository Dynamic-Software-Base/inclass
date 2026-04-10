using Application.Abstractions.Interfaces.Repositories;
using Application.Abstractions.Interfaces.Storage;
using Application.Common.Utilities;
using Application.Schools.Queries.GetSchools;
using Contract.InClass.Common;
using Contract.InClass.Pagination;
using Contract.InClass.Response;
using Contract.InClass.Response.School;
using Domain.Registrations.Enums;
using Domain.Schools;
using Domain.Users;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Enums;
using SharedKernel.ValueObjects.Schools;
using SharedKernel.ValueObjects.StronglyTypedIds;

namespace Infrastructure.Repositories;

public class SchoolRepository : ISchoolRepository
{
    private readonly ApplicationDbContext _dbContext;

    public SchoolRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async  Task<ErrorOr<Success>> AddAsync(School school, CancellationToken cancellationToken = default)
    {
        await _dbContext.AddAsync(school, cancellationToken);
        return Result.Success;
    }

    public async Task<ErrorOr<bool>> ExistAsync(string name, CancellationToken cancellationToken = default)
    {
       return await _dbContext.Schools.AnyAsync(s => s.Name.Trim() == name.Trim(),cancellationToken);
    }

    public async  Task<List<NearestSchoolDto>> GetNearestSchoolsAsync(double latitude, double longitude, int count, IFileUrlResolver fileUrlResolver,
        CancellationToken cancellationToken = default)
    {
        List<School> schools = await _dbContext.Schools.AsNoTracking()
            .Where(s => s.Address.Coordinates != null)
            .Include(s => s.Pictures)
            .ToListAsync(cancellationToken);

        return schools
            .Select(s => new NearestSchoolDto(
                s.Id.Value,
                s.Name,
                s.Description,
                s.Ar_Name,
                s.Address.City,
                ToDto(s.GradeLevels),
                s.Address.Coordinates!.Latitude,
                s.Address.Coordinates!.Longitude,
                GeoDistanceCalculator.CalculateKm(latitude, longitude, s.Address.Coordinates!.Latitude,
                    s.Address.Coordinates!.Longitude),
                s.Pictures.Select(p => new SchoolPictureResponse(
                    p.StoredFileId,
                    fileUrlResolver.GetAccessUrl(p.StoredFileId),
                    p.IsMain
                )).ToList()
            ))
            .OrderBy(s => s.DistanceKm)
            .Take(count)
            .ToList();
    }

    public async Task<ErrorOr<PagedResult<SchoolSummaryDto>>> GetPagedAsync(GetSchoolsQuery query
        ,IFileUrlResolver fileResolver,
        CancellationToken cancellationToken = default)
    {
        IQueryable<School> q = _dbContext.Schools.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(query.NameSearch))
        {
            q = q.Where(s => s.Name.Contains(query.NameSearch) ||
                             (s.Ar_Name != null && s.Ar_Name.Contains(query.NameSearch)));
        }

        if (!string.IsNullOrWhiteSpace(query.City))
        {
            q = q.Where(s => s.Address.City == query.City);
        }

        if (query.GradeLevel != null && (query.GradeLevel.HasPreSchool || query.GradeLevel.HasPrimarySchool ||
            query.GradeLevel.HasHighSchool || query.GradeLevel.HasMiddleSchool))
        {
            GradeLevel filter = GradeLevel.None;
            if (query.GradeLevel.HasPreSchool)
            {
                filter |= GradeLevel.Prescolaire;
            }
            if (query.GradeLevel.HasPrimarySchool)
            {
                filter |= GradeLevel.Primaire;
            }
            if (query.GradeLevel.HasHighSchool)
            {
                filter |= GradeLevel.Lyceen;
            }
            if (query.GradeLevel.HasMiddleSchool)
            {
                filter |= GradeLevel.Collegial;
            }
            //The `(Levels & filter) == filter` pattern means "school must offer ALL the requested levels" — change to `!= GradeLevel.None` if you want "school offers ANY of the requested levels".
            q = q.Where(s => (s.GradeLevels.Levels & filter) == filter);

        }

        int totalCount = await q.CountAsync(cancellationToken);


        // -- sort

        q = query.SortBy?.ToUpperInvariant() switch
        {
            "NAME"    => query.Descending ? q.OrderByDescending(s => s.Name)    : q.OrderBy(s => s.Name),
            "CITY"    => query.Descending ? q.OrderByDescending(s => s.Address.City) : q.OrderBy(s => s.Address.City),
            _         => q.OrderBy(s => s.Name)
        };

        // pagination

        var pagedSchools = await q
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(s => new
            {
                s.Id,
                s.Name,
                s.Ar_Name,
                s.Address,
                s.Description,
                s.GradeLevels,
                Pictures = s.Pictures.Select(p => new { p.StoredFileId, p.IsMain }).ToList()
            })
            .ToListAsync(cancellationToken);


        var schoolIds = pagedSchools.Select(s => s.Id).ToList();

        var openSessions = await _dbContext.RegistrationSessions
            .AsNoTracking()
            .Where(rs => schoolIds.Contains(rs.SchoolId) && rs.Status == RegistrationSessionStatus.Open)
            .Join(
                _dbContext.GradeDefinitions.Include(g => g.Cycle),
                rs => rs.GradeDefinitionId,
                g  => g.Id,
                (rs,g) => new
                {
                    rs.Id,
                    rs.SchoolId,
                    rs.GradeDefinitionId,
                    GradeName_Fr  = g.Name_Fr,
                    GradeName_Ar  = g.Name_Ar,
                    CycleName_Fr  = g.Cycle.Name_Fr,
                    CycleName_Ar  = g.Cycle.Name_Ar,
                    OpenDate      = rs.Period.OpenDate,
                    CloseDate     = rs.Period.CloseDate,
                    MaxSlots      = rs.Capacity.MaxSlots,
                    rs.ReservedCount,
                    rs.EnrolledCount
                })
            .ToListAsync(cancellationToken);

        var sessionsBySchool = openSessions
            .GroupBy(rs => rs.SchoolId)
            .ToDictionary(
                g => g.Key,
                g => g.Select(rs => new OpenSessionSummaryDto(
                    rs.Id.Value,
                    rs.GradeDefinitionId.Value,
                    rs.GradeName_Fr,
                    rs.GradeName_Ar,
                    rs.CycleName_Fr,
                    rs.CycleName_Ar,
                    rs.OpenDate.ToDateTime(new TimeOnly(8, 0, 0)),
                    rs.CloseDate.HasValue ? rs.CloseDate.Value.ToDateTime(new TimeOnly(8, 0, 0)) : null,
                    rs.MaxSlots,
                    rs.ReservedCount,
                    rs.EnrolledCount
                )).ToList());
        var items = pagedSchools.Select(s => new SchoolSummaryDto(
            s.Id.Value,
            s.Name,
            s.Ar_Name,
            s.Address.City,
            s.Description,
            s.Address.Coordinates?.Latitude,
            s.Address.Coordinates?.Longitude,
            ToDto(s.GradeLevels),
            s.Pictures.Select(p => new SchoolPictureResponse(
                p.StoredFileId,
                fileResolver.GetAccessUrl(p.StoredFileId),
                p.IsMain)).ToList(),
            sessionsBySchool.TryGetValue(s.Id, out List<OpenSessionSummaryDto>? sessions) ? sessions : []
        )).ToList();
         return new PagedResult<SchoolSummaryDto>
        {
            Items    = items,
            TotalCount = totalCount,
            Page     = query.Page,
            PageSize = query.PageSize
        };
    }

    public async Task<ErrorOr<List<SchoolSummaryDto>>> GetAllOwnerAsync(UserId id, IFileUrlResolver fileResolver, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Schools.AsNoTracking()
            .Where(s => s.OwnerUserId == id.Value)
            .Select(s => new SchoolSummaryDto(
                s.Id.Value,
                s.Name,
                s.Ar_Name,
                s.Address.City,
                s.Description,
                s.Address.Coordinates!.Latitude,
                s.Address.Coordinates.Longitude,
                ToDto(s.GradeLevels),
                s.Pictures.Select(p => new SchoolPictureResponse(
                    StoredFileId: p.StoredFileId,
                    Url: fileResolver.GetAccessUrl(p.StoredFileId),
                    IsMain: p.IsMain)).ToList(),
                new List<OpenSessionSummaryDto>()))
            .ToListAsync(cancellationToken);
    }

    private static GradeLevelOfferingDto ToDto(GradeLevelOffering gradeLevelOffering) => new GradeLevelOfferingDto(
        gradeLevelOffering.HasPreSchool,
        gradeLevelOffering.HasPrimarySchool,
        gradeLevelOffering.HasMiddleSchool,
        gradeLevelOffering.HasHighSchool);
}
