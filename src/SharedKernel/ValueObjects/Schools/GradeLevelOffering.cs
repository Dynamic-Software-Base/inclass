using SharedKernel.Enums;

namespace SharedKernel.ValueObjects.Schools;

/// <summary>
/// Immutable projection of which broad grade levels a school covers.
/// Never constructed directly by the caller — always derived from
/// the school's SchoolSupportedGrade collection via <see cref="FromGrades"/>.
/// Stored as a single int column for fast discovery queries.
/// </summary>
public sealed record GradeLevelOffering
{
    public GradeLevel Levels { get; init; }

    private GradeLevelOffering(GradeLevel levels) => Levels = levels;

    // Required by EF Core
    private GradeLevelOffering() { }

    public static readonly GradeLevelOffering Empty = new(GradeLevel.None);

    /// <summary>
    /// Derives the offering from a set of (broadLevel) values sourced from
    /// the school's supported GradeDefinitions. Call this inside
    /// School.AddSupportedGrade / School.RemoveSupportedGrade.
    /// </summary>
    public static GradeLevelOffering FromLevels(IEnumerable<GradeLevel> broadLevels)
    {
        GradeLevel combined = broadLevels.Aggregate(GradeLevel.None, (acc, l) => acc | l);
        return new GradeLevelOffering(combined);
    }

    public bool HasPreSchool     => Levels.HasFlag(GradeLevel.Prescolaire);
    public bool HasPrimarySchool => Levels.HasFlag(GradeLevel.Primaire);
    public bool HasMiddleSchool  => Levels.HasFlag(GradeLevel.Collegial);
    public bool HasHighSchool    => Levels.HasFlag(GradeLevel.Lyceen);

    public override string ToString()
    {
        var labels = new Dictionary<GradeLevel, string>
        {
            { GradeLevel.Prescolaire,     "Préscolaire"  },
            { GradeLevel.Primaire, "Primaire"     },
            { GradeLevel.Collegial,  "Collège"      },
            { GradeLevel.Lyceen,    "Lycée"        }
        };

        IEnumerable<string> active = labels
            .Where(kv => Levels.HasFlag(kv.Key))
            .Select(kv => kv.Value);

        return string.Join(", ", active);
    }
}
