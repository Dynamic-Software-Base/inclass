using SharedKernel.Enums;
using SharedKernel.ValueObjects.StronglyTypedIds;

namespace Domain.Schools.Entities;

/// <summary>
/// Declares that a school offers a specific grade.
/// This answers: "does this school teach this grade at all?" —
/// independently of whether enrollment is currently open.
///
/// <see cref="IsOffered"/> can be toggled to temporarily suspend a grade
/// without removing it from the school's curriculum definition.
///
/// <see cref="CachedBroadLevel"/> is denormalized from the GradeCycleDefinition
/// so that <see cref="School.GradeLevels"/> can be recomputed without an extra join.
/// </summary>
public sealed class SchoolSupportedGrade
{
    public SchoolId SchoolId { get; private set; }
    public GradeDefinitionId GradeDefinitionId { get; private set; }

    /// <summary>
    /// Denormalized broad level from GradeCycleDefinition.BroadLevel.
    /// Used to recompute GradeLevelOffering without loading the full config tree.
    /// </summary>
    public GradeLevel CachedBroadLevel { get; private set; }

    /// <summary>
    /// Whether the school actively offers this grade to prospective students.
    /// False = grade exists in curriculum but is not currently advertised/available.
    /// This is NOT about enrollment being open — that is OpenRegistration's concern.
    /// </summary>
    public bool IsOffered { get; private set; } = true;

    /// <summary>Standing total capacity for this grade across all classes.</summary>
    public int? Capacity { get; private set; }

    private SchoolSupportedGrade(){}

    private SchoolSupportedGrade(
        SchoolId schoolId,
        GradeDefinitionId gradeDefinitionId,
        GradeLevel cachedBroadLevel,
        int? capacity = null)
    {
        SchoolId = schoolId;
        GradeDefinitionId = gradeDefinitionId;
        CachedBroadLevel = cachedBroadLevel;
        Capacity = capacity;

    }

    internal static SchoolSupportedGrade Create(
        SchoolId schoolId,
        GradeDefinitionId gradeDefinitionId,
        GradeLevel cachedBroadLevel,
        int? capacity = null)
    {
     return new SchoolSupportedGrade(schoolId, gradeDefinitionId, cachedBroadLevel, capacity);
    }

    public void Suspend() => IsOffered = false;
    public void Restore() => IsOffered = true;
    public void UpdateCapacity(int? capacity) => Capacity = capacity;
}
