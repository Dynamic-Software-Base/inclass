using Application.Abstractions.Messaging;
using Contract.InClass.Request.Registration.Session;
using SharedKernel.ValueObjects;

namespace Application.Registration.Sessions.Command.CreateRegistratinSession;

public record CreateBatchRegistrationSessionsCommand(
    Guid SchoolId,
    string AcademicYear,
    List<Guid> GradeDefinitionIds,
    DateTime OpenDate,
    DateTime CloseDate,
    int AssignmentStrategy,
    List<RegistrationPhaseRequest> Phases,
    int? DailyQuota = null,
    TimeOnly? DailyCutoffTime = null) : ICommand<ErrorOr<CreateBatchSessionsResult>>;
