using SharedKernel.Enums;

namespace SharedKernel.ValueObjects.Schools;

public record GradeLevelOffering
{
    public GradeLevel Levels { get; init; }

    private GradeLevelOffering(
        GradeLevel levels) => Levels = levels;
    private GradeLevelOffering(){}

    public static GradeLevelOffering Create(
        bool hasPreSchool,
        bool hasPrimarySchool,
        bool hasMiddleSchool,
        bool hasHighSchool
    )
    {
        GradeLevel levels = GradeLevel.None;


        if (hasPreSchool)    { levels |= GradeLevel.PreSchool;}
        if (hasPrimarySchool) {levels |= GradeLevel.PrimarySchool;}
        if (hasMiddleSchool) { levels |= GradeLevel.MiddleSchool;}
        if (hasHighSchool)  {  levels |= GradeLevel.HighSchool;}

        return new GradeLevelOffering(levels);
    }
    public bool HasPreSchool     => Levels.HasFlag(GradeLevel.PreSchool);
    public bool HasPrimarySchool => Levels.HasFlag(GradeLevel.PrimarySchool);
    public bool HasMiddleSchool  => Levels.HasFlag(GradeLevel.MiddleSchool);
    public bool HasHighSchool    => Levels.HasFlag(GradeLevel.HighSchool);
    private string GetSummary()
    {
        var map = new Dictionary<GradeLevel, string>
        {
            { GradeLevel.PreSchool,     "Préscolaire" },
            { GradeLevel.PrimarySchool, "Primaire" },
            { GradeLevel.MiddleSchool,  "Collège" },
            { GradeLevel.HighSchool,    "Lycée" }
        };

        IEnumerable<string> levels = map
            .Where(kv => Levels.HasFlag(kv.Key))
            .Select(kv => kv.Value);

        return string.Join(", ", levels);
    }
    public override string ToString() =>  GetSummary();
}
