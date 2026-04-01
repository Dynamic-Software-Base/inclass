using Application.Abstractions.Authentication;
using Application.Abstractions.Interfaces;
using Application.Abstractions.Interfaces.Repositories;
using Contract.InClass.Request.Registration.Session;
using Domain.Registrations;
using Domain.Registrations.Enums;
using Domain.Registrations.ValueObjects;
using Domain.Schools;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SharedKernel.ValueObjects;
using SharedKernel.ValueObjects.Registration;
using SharedKernel.ValueObjects.StronglyTypedIds;
using SharedKernel.ValueObjects.StronglyTypedIds.Registration;

namespace Application.Registration.Sessions.Command.CreateRegistratinSession;

public sealed class CreateBatchRegistrationSessionsCommandHandler
    : IRequestHandler<CreateBatchRegistrationSessionsCommand, ErrorOr<CreateBatchSessionsResult>>
{
    private readonly IRegistrationSessionRepository _sessionRepository;
    private readonly IRegistrationFormSchemaRepository _schemaRepository;
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public CreateBatchRegistrationSessionsCommandHandler(
        IRegistrationSessionRepository sessionRepository,
        IRegistrationFormSchemaRepository schemaRepository,
        IApplicationDbContext context,
        ICurrentUserService currentUser)
    {
        _sessionRepository = sessionRepository;
        _schemaRepository = schemaRepository;
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<ErrorOr<CreateBatchSessionsResult>> Handle(
        CreateBatchRegistrationSessionsCommand request,
        CancellationToken cancellationToken)
    {
        var schoolId = SchoolId.From(request.SchoolId);
        UserId userId = _currentUser.GetCurrentUser().Id;

        // 1. Validate school exists — hard fail
        bool schoolExists = await _context.Schools
            .AnyAsync(s => s.Id == schoolId, cancellationToken);

        if (!schoolExists)
        {
            return Error.NotFound("RegistrationSession.SchoolNotFound",
                "The specified school was not found.");
        }

        // 2. Parse academic year — hard fail
        ErrorOr<AcademicYear> academicYearResult = AcademicYear.Create(request.AcademicYear);
        if (academicYearResult.IsError)
        {
            return academicYearResult.Errors;
        }

        AcademicYear academicYear = academicYearResult.Value;

        // 3. Build shared value objects — hard fail
        ErrorOr<List<RegistrationPhase>> phasesResult =
            BuildPhases(request.Phases);
        if (phasesResult.IsError)
        {
            return phasesResult.Errors;
        }

        ErrorOr<RegistrationPeriod> periodResult =
            RegistrationPeriod.Create(request.OpenDate, request.CloseDate);
        if (periodResult.IsError)
        {
            return periodResult.Errors;
        }

        DailyProcessingQuota? quota = null;
        if (request.DailyQuota.HasValue)
        {
            ErrorOr<DailyProcessingQuota> quotaResult =
                DailyProcessingQuota.Create(request.DailyQuota.Value);
            if (quotaResult.IsError)
            {
                return quotaResult.Errors;
            }

            quota = quotaResult.Value;
        }

        // 4. Process each grade
        var createdSessions = new List<RegistrationSession>();
        var skipped = new List<SkippedGradeResult>();

        foreach (Guid gradeId in request.GradeDefinitionIds)
        {
            var gradeDefinitionId = GradeDefinitionId.From(gradeId);

            // 4a. Check duplicate session — skip
            bool sessionExists = await _sessionRepository.ExistsAsync(
                schoolId, gradeDefinitionId, academicYear, cancellationToken);

            if (sessionExists)
            {
                skipped.Add(new SkippedGradeResult(
                    gradeId,
                    "A registration session already exists for this grade and academic year."));
                continue;
            }

            // 4b. Resolve capacity from existing classes — skip if none
            List<int> classCapacities = await _context.SchoolClasses
                .Where(c =>
                    c.SchoolId == schoolId &&
                    c.GradeDefinitionId == gradeDefinitionId &&
                    c.AcademicYear == academicYear)
                .Select(c => c.Capacity.MaxStudents)
                .ToListAsync(cancellationToken);

            if (classCapacities.Count == 0)
            {
                skipped.Add(new SkippedGradeResult(
                    gradeId,
                    "No classes found for this grade. Create classes before opening a session."));
                continue;
            }

            int totalCapacity = classCapacities.Sum();
            ErrorOr<RegistrationCapacity> capacityResult =
                RegistrationCapacity.Create(totalCapacity);
            if (capacityResult.IsError)
            {
                skipped.Add(new SkippedGradeResult(gradeId, capacityResult.Errors[0].Description));
                continue;
            }

            // 4c. Resolve form schema — skip if not found
            RegistrationFormSchema? schema =
                await _schemaRepository.GetBySchoolAndGradeAsync(schoolId, gradeDefinitionId, cancellationToken)
                ?? await _schemaRepository.GetDefaultByGradeAsync(gradeDefinitionId, cancellationToken);

            if (schema is null)
            {
                skipped.Add(new SkippedGradeResult(
                    gradeId,
                    "No form schema found for this grade level."));
                continue;
            }

            // 4d. Create session
            ErrorOr<RegistrationSession> sessionResult = RegistrationSession.Create(
                RegistrationSessionId.New(),
                schoolId,
                gradeDefinitionId,
                schema.Id,
                academicYear,
                periodResult.Value,
                phasesResult.Value,
                (AssignmentStrategy)request.AssignmentStrategy,
                userId,
                capacityResult.Value,
                quota,
                request.DailyCutoffTime);

            if (sessionResult.IsError)
            {
                skipped.Add(new SkippedGradeResult(
                    gradeId,
                    sessionResult.Errors[0].Description));
                continue;
            }

            createdSessions.Add(sessionResult.Value);
        }

        // 5. Persist all successful sessions atomically
        if (createdSessions.Count > 0)
        {
            await _sessionRepository.AddRangeAsync(createdSessions, cancellationToken);
        }

        return new CreateBatchSessionsResult(
            CreatedSessionIds: createdSessions.Select(s => s.Id.Value).ToList(),
            Skipped: skipped);
    }

    private static ErrorOr<List<RegistrationPhase>> BuildPhases(
        List<RegistrationPhaseRequest> phaseRequests)
    {
        var phases = new List<RegistrationPhase>();

        foreach (RegistrationPhaseRequest phaseRequest in phaseRequests)
        {
            ErrorOr<RegistrationPhase> result = RegistrationPhase.Create(
                phaseRequest.StartDate,
                phaseRequest.EndDate,
                (RegistrationPhaseType)phaseRequest.PhaseType,
                (AllowedApplicantType)phaseRequest.AllowedApplicantType);

            if (result.IsError)
            {
                return result.Errors;
            }

            phases.Add(result.Value);
        }

        return phases;
    }
}
