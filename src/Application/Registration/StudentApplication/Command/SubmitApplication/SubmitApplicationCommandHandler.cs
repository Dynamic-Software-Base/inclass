using System.Text.Json;
using Application.Abstractions.Authentication;
using Application.Abstractions.Interfaces;
using Application.Abstractions.Interfaces.Repositories;
using Domain.Registrations;
using Domain.Registrations.Enums;
using Domain.Registrations.ValueObjects;
using Domain.Students;
using MediatR;
using SharedKernel.ValueObjects;
using SharedKernel.ValueObjects.Registration;
using SharedKernel.ValueObjects.StronglyTypedIds.Registration;

namespace Application.Registration.StudentApplication.Command.SubmitApplication;


public sealed class SubmitApplicationCommandHandler
    : IRequestHandler<SubmitApplicationCommand, ErrorOr<SubmitApplicationResult>>
{
    private readonly IStudentApplicationRepository _applicationRepository;
    private readonly IRegistrationSessionRepository _sessionRepository;
    private readonly IRegistrationFormSchemaRepository _schemaRepository;
    private readonly IStudentRepository _studentRepository;
    private readonly ICurrentUserService _currentUser;

    public SubmitApplicationCommandHandler(
        IStudentApplicationRepository applicationRepository,
        IRegistrationSessionRepository sessionRepository,
        IRegistrationFormSchemaRepository schemaRepository,
        IStudentRepository studentRepository,
        ICurrentUserService currentUser)
    {
        _applicationRepository = applicationRepository;
        _sessionRepository = sessionRepository;
        _schemaRepository = schemaRepository;
        _studentRepository = studentRepository;
        _currentUser = currentUser;
    }

    public async Task<ErrorOr<SubmitApplicationResult>> Handle(
        SubmitApplicationCommand request,
        CancellationToken cancellationToken)
    {
        var sessionId = RegistrationSessionId.From(request.SessionId);

        RegistrationSession? session = await _sessionRepository
            .GetByIdAsync(sessionId, cancellationToken);

        if (session is null)
        {
            return Error.NotFound("Application.SessionNotFound",
                "The specified registration session was not found.");
        }

        // 2. Validate session is accepting registrations
        if (!session.IsAcceptingRegistrations(DateTime.UtcNow))
        {
            return Error.Conflict("Application.SessionNotOpen",
                "This registration session is not currently accepting applications.");
        }

        StudentId? studentId = null;
        ApplicantType applicantType = ApplicantType.New;

        if (request.IsReturning)
        {
            if (string.IsNullOrWhiteSpace(request.IdentityKey))
            {
                return Error.Validation("Application.IdentityKeyRequired",
                    "IdentityKey is required for returning students.");
            }
            Student? existingStudent = await _studentRepository.GetByIdentityKeyAsync(request.IdentityKey,cancellationToken);
            if (existingStudent is null)
            {
                return Error.NotFound("Application.StudentNotFound",
                    "No student found with the provided identity key.");
            }
            studentId = existingStudent.Id;
            applicantType = ApplicantType.Returning;
        }
        RegistrationPhase? activePhase = session.GetActivePhase(DateTime.UtcNow);

        if (activePhase is null)
        {
            return Error.Conflict("Application.NoActivePhase",
                "There is no active registration phase at this time.");
        }
        if (!activePhase.IsForApplicantType(applicantType))
        {
            return Error.Conflict("Application.PhaseNotAllowed",
                "The current registration phase does not allow this applicant type.");
        }
        bool alreadyApplied = await _applicationRepository.ExistsAsync(
            sessionId,
            request.StudentFirstName,
            request.StudentLastName,
            request.ContactPhone,
            cancellationToken);
        if (alreadyApplied)
        {
            return Error.Conflict("Application.AlreadyExists",
                "An application with the same name and phone number already exists for this session.");
        }
        RegistrationFormSchema? schema =
            await _schemaRepository.GetBySchoolAndGradeAsync(
                session.SchoolId, session.GradeDefinitionId, cancellationToken)
            ?? await _schemaRepository.GetDefaultByGradeAsync(
                session.GradeDefinitionId, cancellationToken);
        if (schema is null)
        {
            return Error.NotFound("Application.SchemaNotFound",
                "No form schema found for this grade level.");
        }
        ErrorOr<Success> schemaValidation = ValidateFormAgainstSchema(
            request.FormValuesJson, schema.SchemaJson);
        if (schemaValidation.IsError)
        {
            return schemaValidation.Errors;
        }
            // 7. Build contact value object
        Email? email = string.IsNullOrWhiteSpace(request.ContactEmail)
            ? null
            : Email.Create(request.ContactEmail).Value;

        PhoneNumber? phone = PhoneNumber.Create(request.ContactPhone).Value;

        ErrorOr<ApplicantContact> contactResult = ApplicantContact.Create(email, phone);
        if (contactResult.IsError)
        {
            return contactResult.Errors;
        }

        // 8. Create application
        ErrorOr<Domain.Registrations.StudentApplication> applicationResult = Domain.Registrations.StudentApplication.Create(
            StudentApplicationId.New(),
            sessionId,
            session.SchoolId,
            session.GradeDefinitionId,
            session.AcademicYear,
            applicantType,
            studentId,
            new ParentTuteurId(Guid.Empty), // parent resolved later from form JSON at enrollment
            contactResult.Value,
            request.FormValuesJson,
            request.StudentFirstName,
            request.StudentLastName,
            _currentUser.GetCurrentUser().Id);

        if (applicationResult.IsError)
        {
            return applicationResult.Errors;
        }

        Domain.Registrations.StudentApplication application = applicationResult.Value;

        // 9. Reserve or waitlist
        if (session.GetAvailableSlots() > 0)
        {
            int queuePosition = session.ReservedCount + 1;

            ErrorOr<DateOnly> processingDateResult = session.CalculateProcessingDate(
                queuePosition, DateTime.UtcNow);

            if (processingDateResult.IsError)
            {
                return processingDateResult.Errors;
            }

            ErrorOr<QueueInfo> queueInfoResult = QueueInfo.Create(
                queuePosition,
                processingDateResult.Value);

            if (queueInfoResult.IsError)
            {
                return queueInfoResult.Errors;
            }

            ErrorOr<Success> reserveSpotResult = session.ReserveSpot();
            if (reserveSpotResult.IsError)
            {
                return reserveSpotResult.Errors;
            }

            ErrorOr<Success> reserveResult = application.Reserve(
                queueInfoResult.Value,
                DateTime.SpecifyKind(
                    processingDateResult.Value.ToDateTime(TimeOnly.MinValue),
                    DateTimeKind.Utc));

            if (reserveResult.IsError)
            {
                return reserveResult.Errors;
            }
        }
        else
        {
            session.AddToWaitlist();

            ErrorOr<Success> waitlistResult = application.AddToWaitlist(session.WaitlistCount);
            if (waitlistResult.IsError)
            {
                return waitlistResult.Errors;
            }
        }

        // 10. Persist
        await _applicationRepository.AddAsync(application, cancellationToken);

        return new SubmitApplicationResult(application.Id.Value, application.IdentityKey);
    }

    private static ErrorOr<Success> ValidateFormAgainstSchema(
        string formValuesJson,
        string schemaJson)
    {
        try
        {
            using var formDoc = System.Text.Json.JsonDocument.Parse(formValuesJson);
            using var schemaDoc = System.Text.Json.JsonDocument.Parse(schemaJson);

            var requiredFields = schemaDoc.RootElement
                .GetProperty("sections")
                .EnumerateArray()
                .SelectMany(section => section.GetProperty("fields").EnumerateArray())
                .Where(field =>
                    field.TryGetProperty("required", out JsonElement req) &&
                    req.GetBoolean())
                .Select(field => field.GetProperty("key").GetString())
                .Where(key => key is not null)
                .ToList();

            JsonElement formRoot = formDoc.RootElement;

            foreach (string? key in requiredFields)
            {
                if (!formRoot.TryGetProperty(key!, out JsonElement value) ||
                    value.ValueKind == System.Text.Json.JsonValueKind.Null ||
                    (value.ValueKind == System.Text.Json.JsonValueKind.String &&
                     string.IsNullOrWhiteSpace(value.GetString())))
                {
                    return Error.Validation(
                        "Application.FormValidation",
                        $"Required field '{key}' is missing or empty.");
                }
            }

            return Result.Success;
        }
        catch (System.Text.Json.JsonException)
        {
            return Error.Validation(
                "Application.FormValidation.ParseError",
                "Form values JSON is malformed.");
        }
    }
}
