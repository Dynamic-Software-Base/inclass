using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Schools.Contracts;
using Domain.Schools;
using MediatR;
using SharedKernel;
using SharedKernel.Enums;
using SharedKernel.ValueObjects.StronglyTypedIds;

namespace Application.Schools.Commands.CreateSchool;

public sealed class CreateSchoolCommandHandler(
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUserService)
    : IRequestHandler<CreateSchoolCommand, ErrorOr<CreateSchoolResponse>>
{
    private const int MinSchoolNameLength = 2;
    private const int MaxSchoolNameLength = 200;

    public async Task<ErrorOr<CreateSchoolResponse>> Handle(
        CreateSchoolCommand request,
        CancellationToken cancellationToken)
    {
        ICurrentUser currentUser = currentUserService.GetCurrentUser();
        if (!currentUser.IsAuthenticated)
        {
            return Error.Unauthorized("School.Create.Unauthorized", "Authentication is required to create schools.");
        }

        string name = request.Name?.Trim() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(name))
        {
            return Error.Validation("School.Create.Name.Required", "School name is required.");
        }

        if (name.Length < MinSchoolNameLength)
        {
            return Error.Validation(
                "School.Create.Name.MinLength",
                $"School name must be at least {MinSchoolNameLength} characters.");
        }

        if (name.Length > MaxSchoolNameLength)
        {
            return Error.Validation(
                "School.Create.Name.MaxLength",
                $"School name must be {MaxSchoolNameLength} characters or fewer.");
        }

        var school = School.Create(
            SchoolId.New(),
            currentUser.Id,
            name,
            currentUser.Id);

        var membership = UserSchoolMembership.Create(
            UserSchoolMembershipId.New(),
            currentUser.Id,
            school.Id,
            UserRole.SchoolOwner,
            isActive: true);

        await unitOfWork
            .Set<School>()
            .AddAsync(school, cancellationToken);

        await unitOfWork
            .Set<UserSchoolMembership>()
            .AddAsync(membership, cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new CreateSchoolResponse(school.Id, school.Name);
    }
}
