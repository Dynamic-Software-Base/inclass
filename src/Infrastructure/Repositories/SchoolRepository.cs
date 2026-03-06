using Application.Abstractions.Interfaces.Repositories;
using Application.Abstractions.Interfaces.Storage;
using Application.Schools.Queries.GetSchools;
using Contract.InClass.Common;
using Contract.InClass.Pagination;
using Contract.InClass.Response;
using Domain.Schools;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Enums;

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
                filter |= GradeLevel.PreSchool;
            }
            if (query.GradeLevel.HasPrimarySchool)
            {
                filter |= GradeLevel.PrimarySchool;
            }
            if (query.GradeLevel.HasHighSchool)
            {
                filter |= GradeLevel.HighSchool;
            }
            if (query.GradeLevel.HasMiddleSchool)
            {
                filter |= GradeLevel.MiddleSchool;
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

        List<SchoolSummaryDto> items = await q.Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(s => new SchoolSummaryDto(
                s.Id.Value,
                s.Name,
                s.Ar_Name,
                s.Address.City,
                s.Description,
                new GradeLevelOfferingDto(
                    s.GradeLevels.HasPreSchool,
                    s.GradeLevels.HasPrimarySchool,
                    s.GradeLevels.HasMiddleSchool,
                    s.GradeLevels.HasHighSchool),
                s.Pictures.Select(p => new SchoolPictureResponse(
                    StoredFileId:p.StoredFileId ,
                    Url: fileResolver.GetAccessUrl(p.StoredFileId),
                    IsMain: p.IsMain)).ToList()))
            .ToListAsync<SchoolSummaryDto>(cancellationToken);

        return new PagedResult<SchoolSummaryDto>()
        {
            Items = items,
            TotalCount = totalCount,
            Page = query.Page,
            PageSize = query.PageSize
        };
    }

}
