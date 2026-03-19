namespace SharedKernel.ValueObjects;

public record AcademicYear
{
    public string Value { get; init; }
    public int StartYear { get; init; }
    public int EndYear { get; init; }

    private AcademicYear(string value, int startYear, int endYear)
    {
        Value = value;
        StartYear = startYear;
        EndYear = endYear;
    }

    public static ErrorOr<AcademicYear> Create(string value)
    {
        if (!string.IsNullOrWhiteSpace(value))
        {
            return DomainErrors.Required(nameof(value));
        }

        string[] parts = value.Split('-');
        if (parts.Length != 2
            || !int.TryParse(parts[0], out int startYear)
            || !int.TryParse(parts[1], out int endYear))
        {
            return Error.Validation("AcademicYear.Create",
                "Academic value must be this format AAAA-AAAA (ex: 2025-2026)");
        }

        if (endYear != startYear + 1)
        {
            return Error.Validation("AcademicYear.Range", "Starting year must be less then end date");
        }

        return new AcademicYear(value, startYear, endYear);
    }
}
