using Domain.Registrations.Enums;

namespace Domain.Registrations.ValueObjects;

public sealed record RegistrationPhase
{
    public DateOnly StartDate { get; init; }
    public DateOnly? EndDate { get; init; }
    public RegistrationPhaseType PhaseType { get; init; }
    public AllowedApplicantType AllowedApplicantType { get; init; }

    private RegistrationPhase() { }

    internal RegistrationPhase(
        DateOnly startDate,
        DateOnly? endDate,
        RegistrationPhaseType phaseType,
        AllowedApplicantType allowedApplicantType)
    {
        StartDate = startDate;
        EndDate = endDate;
        PhaseType = phaseType;
        AllowedApplicantType = allowedApplicantType;
    }

    public static ErrorOr<RegistrationPhase> Create(
        DateOnly startDate,
        DateOnly? endDate,
        RegistrationPhaseType phaseType,
        AllowedApplicantType allowedApplicantType)
    {
        if (startDate >= endDate)
        {
            return Error.Validation(
                "RegistrationPhase.InvalidRange",
                "La date de début doit être antérieure à la date de fin.");
        }

        return new RegistrationPhase(startDate, endDate, phaseType, allowedApplicantType);
    }


    public bool IsActive(DateTime utcNow) =>
        DateOnly.FromDateTime(utcNow) >= StartDate && DateOnly.FromDateTime(utcNow) <= EndDate;

    public bool IsForApplicantType(ApplicantType applicantType) =>
        AllowedApplicantType == AllowedApplicantType.All
        || (AllowedApplicantType == AllowedApplicantType.ReturningOnly
            && applicantType == ApplicantType.Returning);
}
