using Application.Abstractions.Authentication;
using Application.Abstractions.Interfaces.Repositories;
using Application.Abstractions.Interfaces.Services;
using Application.Schools.Contracts;
using Domain.EducationalSystem.Entities;
using Domain.Schools;
using Domain.Schools.Entities;
using MediatR;
using SharedKernel;
using SharedKernel.Enums;
using SharedKernel.ValueObjects;
using SharedKernel.ValueObjects.Schools;
using SharedKernel.ValueObjects.StronglyTypedIds;

namespace Application.Schools.Commands.CreateSchool;

public sealed class CreateSchoolCommandHandler
    : IRequestHandler<CreateSchoolCommand, ErrorOr<CreateSchoolResponse>>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly ISchoolRepository _schoolRepo;
    private readonly IMemberShipReposiory _membershipRepo;
    private readonly IGeoCodingService _geocodingService;
    private readonly IEducationalSystemRepository _educationalSystemRepo;

    public CreateSchoolCommandHandler(
        ICurrentUserService currentUserService,
        ISchoolRepository schoolRepo,
        IMemberShipReposiory membershipRepo,
        IGeoCodingService geocodingService,
        IEducationalSystemRepository educationalSystemRepo)
    {
        _currentUserService = currentUserService;
        _schoolRepo = schoolRepo;
        _membershipRepo = membershipRepo;
        _geocodingService = geocodingService;
        _educationalSystemRepo = educationalSystemRepo;
    }

    public async Task<ErrorOr<CreateSchoolResponse>> Handle(
        CreateSchoolCommand request,
        CancellationToken cancellationToken)
    {
        ICurrentUser currentUser = _currentUserService.GetCurrentUser();
        UserId currentUserId = currentUser.Id;

        // ── 1. Validate EducationalSystem exists ─────────────────────────────
        var educationalSystemId = EducationalSystemId.From(request.EducationalSystemId);

        bool systemExists = await _educationalSystemRepo.ExistsAsync(
            educationalSystemId,
            cancellationToken);

        if (!systemExists)
        {
            return DomainErrors.EducationalSystemErrors.NotFound;
        }


        // ── 2. Build Address ─────────────────────────────────────────────────
        ErrorOr<Address> addressResult = Address.Create(
            request.Address.StreetAddress,
            request.Address.BuildingNumber,
            request.Address.ApartmentNumber,
            request.Address.City,
            request.Address.Province,
            request.Address.Region,
            request.Address.PostalCode,
            null);

        if (addressResult.IsError)
        {
            return addressResult.Errors;
        }


        // Geocoding is non-blocking — failure falls back to no coordinates
        ErrorOr<Coordinates> geocodeResult = await _geocodingService
            .GetCoordinatesAsync(addressResult.Value.ToString(), cancellationToken);

        Address address = geocodeResult.IsError
            ? addressResult.Value
            : addressResult.Value.WithCoordinates(geocodeResult.Value);

        // ── 3. Build ContactInfo ─────────────────────────────────────────────
        ErrorOr<SchoolContactInfo> contactInfoResult = SchoolContactInfo.Create(
            request.ContactInfo.PrimaryPhoneNumber,
            request.ContactInfo.SecondaryPhoneNumber,
            request.ContactInfo.Email);

        if (contactInfoResult.IsError)
        {
            return contactInfoResult.Errors;
        }


        // ── 4. Create School ─────────────────────────────────────────────────
        // GradeLevelOffering starts as Empty — derived automatically as
        // supported grades are added below. Never passed in manually.
        ErrorOr<School> schoolResult = School.Create(
            SchoolId.New(),
            currentUserId,
            request.Name,
            request.Ar_Name,
            currentUserId,
            address,
            educationalSystemId,
            contactInfoResult.Value,
            request.Description);

        if (schoolResult.IsError)
        {
            return schoolResult.Errors;

        }

        School school = schoolResult.Value;

        // ── 5. Add supported grades ──────────────────────────────────────────
        if (request.SupportedGradeIds.Count > 0)
        {
            var gradeDefinitionIds = request.SupportedGradeIds
                .Select(GradeDefinitionId.From)
                .ToList();

            // Single query — loads GradeDefinition + parent GradeCycleDefinition
            List<(GradeDefinition Grade, GradeCycleDefinition Cycle)> gradePairs =
                await _educationalSystemRepo.GetGradeDefinitionsWithCyclesAsync(
                    gradeDefinitionIds,
                    cancellationToken);

            // If count differs, one or more IDs were not found or inactive
            if (gradePairs.Count != request.SupportedGradeIds.Count)
            {
                return DomainErrors.SchoolErrors.OneOrMoreGradeDefinitionsNotFound;
            }


            foreach ((GradeDefinition grade, GradeCycleDefinition cycle) in gradePairs)
            {
                ErrorOr<SchoolSupportedGrade> addResult =
                    school.AddSupportedGrade(grade, cycle);

                if (addResult.IsError)
                {
                    return addResult.Errors;
                }

            }
        }

        // ── 6. Add pictures ──────────────────────────────────────────────────
        foreach (CreateSchoolPicturesDto pictureDto in request.Pictures)
        {
            var picture = SchoolPicture.Create(
                StoredFileId.From(pictureDto.PictureId),
                null,
                pictureDto.IsMain);

            ErrorOr<Updated> addPictureResult = school.AddPicture(picture);
            if (addPictureResult.IsError)
            {
                return addPictureResult.Errors;
            }

        }

        // ── 7. Create membership ─────────────────────────────────────────────
        var membership = UserSchoolMembership.Create(
            UserSchoolMembershipId.New(),
            currentUserId,
            school.Id,
            UserRole.SchoolOwner,
            isActive: true);

        // ── 8. Persist (UoW commits in middleware) ───────────────────────────
        await _schoolRepo.AddAsync(school, cancellationToken);
        await _membershipRepo.AddAsync(membership, cancellationToken);

        return new CreateSchoolResponse(school.Id, school.Name);
    }
}
