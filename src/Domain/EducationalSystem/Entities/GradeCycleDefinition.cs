using SharedKernel;
using SharedKernel.Enums;
using SharedKernel.ValueObjects.StronglyTypedIds;

namespace Domain.EducationalSystem.Entities;
/// <summary>
/// A named cycle within an education system.
/// e.g. "Cycle primaire" (maps to PrimarySchool), "Cycle collégial" (MiddleSchool).
/// The <see cref="BroadLevel"/> bridges back to the <see cref="GradeLevel"/> flags enum
/// for discovery filtering without duplicating it on every GradeDefinition.
/// </summary>
public sealed class GradeCycleDefinition : Entity<GradeCycleDefinition,GradeCycleDefinitionId>
{
    public EducationalSystemId EducationalSystemId { get; private set; }

    /// <summary>machine-readable code e.g. "CYCLE_PRIM" . "CYCLE_COLL" </summary>
    public string Code { get; private set; } = string.Empty;

    public string Name_Fr { get; private set; } = string.Empty;
    public string Name_Ar { get; private set; } = string.Empty;

    /// <summary>
    /// Maps this cycle to the broad GradeLevel flag used for
    /// school discovery and gradeLevelOffering projection
    /// </summary>
    public GradeLevel BroadLevel { get; private set; }
    public int SortOrder { get; private set; }

    private readonly List<GradeDefinition> _grades = [];
    public IReadOnlyList<GradeDefinition> Grades => _grades.AsReadOnly();

    private GradeCycleDefinition(){}

    private GradeCycleDefinition( GradeCycleDefinitionId id,
        EducationalSystemId educationSystemId,
        string code,
        string nameFr,
        string nameAr,
        GradeLevel broadLevel,
        int sortOrder) : base(id)
    {
        EducationalSystemId = educationSystemId;
        Code = code;
        Name_Fr = nameFr;
        Name_Ar = nameAr;
        BroadLevel = broadLevel;
        SortOrder = sortOrder;
    }
    public static GradeCycleDefinition Create(
        GradeCycleDefinitionId id,
        EducationalSystemId educationSystemId,
        string code,
        string nameFr,
        string nameAr,
        GradeLevel broadLevel,
        int sortOrder)
    {
        return new GradeCycleDefinition(id, educationSystemId, code, nameFr, nameAr, broadLevel, sortOrder);
    }
}
