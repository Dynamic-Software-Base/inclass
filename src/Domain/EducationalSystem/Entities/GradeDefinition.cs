using SharedKernel;
using SharedKernel.ValueObjects.StronglyTypedIds;

namespace Domain.EducationalSystem.Entities;
/// <summary>
/// A single academic year/grade within a cycle.
/// e.g. "7ème année (1ère collège)", "Tronc commun", "2ème Bac Sciences Maths"
/// Seeded — schools reference these by ID, never create their own.
/// </summary>
public sealed class GradeDefinition : Entity<GradeDefinition,GradeDefinitionId>
{
    public GradeCycleDefinitionId GradeCycleDefinitionId { get; private set; }

    /// <summary>
    /// Navigation property to the parent cycle.
    /// Used only by the repository when loading grade+cycle pairs
    /// for school.AddSupportedGrade — never traversed from the domain.
    /// EF Core populates this via .Include(g => g.Cycle).
    /// </summary>
    public GradeCycleDefinition Cycle { get; private set; } = null!;
    /// <summary>Machine-readable code. e.g. "MS_Y1", "HS_TC", "HS_2BAC_SM"</summary>
    public string Code { get; private set; } = string.Empty;

    public string Name_Fr { get; private set; } = string.Empty;
    public string Name_Ar { get; private set; } = string.Empty;

    /// <summary>Display order within the cycle.</summary>
    public int SortOrder { get; private set; }

    public bool IsActive { get; private set; } = true;
    private GradeDefinition(){}

    private GradeDefinition(GradeDefinitionId id, GradeCycleDefinitionId cycleDefinitionId,string code, string name_Fr, string name_Ar, int sortOrder)
    :base(id)
    {
        GradeCycleDefinitionId = cycleDefinitionId;
        Code = code;
        Name_Fr = name_Fr;
        Name_Ar = name_Ar;
        SortOrder = sortOrder;

    }

    public static GradeDefinition Create(GradeDefinitionId id, GradeCycleDefinitionId cycleDefinitionId, string code,
        string name_Fr, string name_Ar, int sortOrder)
    {
        return new GradeDefinition(id, cycleDefinitionId, code, name_Fr, name_Ar, sortOrder);
    }

    public void Deactivate() => IsActive = false;
    public void Activate() => IsActive = true;
}
