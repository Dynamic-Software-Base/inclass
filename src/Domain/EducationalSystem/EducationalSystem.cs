using Domain.EducationalSystem.Entities;
using SharedKernel;
using SharedKernel.ValueObjects.StronglyTypedIds;

namespace Domain.EducationalSystem;

/// <summary>
/// Root of the curriculum configuration tree.
/// e.g. "System marocain MEN" , "Mission francaise OSUI", "System internaional IB"
/// seeded a startup , not created by users.
/// </summary>
public sealed class EducationalSystem : Entity<EducationalSystem,EducationalSystemId>
{
    /// <summary> Short machine-readable code e.g. "MEN_MA" , "FR_MISSION" , "IB" </summary>
    public string Code { get; private set; } = string.Empty;

    public string Name_Fr { get; private set; } = string.Empty;
    public string Name_Ar { get; private set; } = string.Empty;
    private readonly List<GradeCycleDefinition> _cycles = [];
    public IReadOnlyList<GradeCycleDefinition> Cycles => _cycles.AsReadOnly();
    private EducationalSystem (){}

    private EducationalSystem(EducationalSystemId id, string code, string name_Fr, string name_Ar) : base(id)
    {
        Code = code;
        Name_Fr = name_Fr;
        Name_Ar = name_Ar;
    }
    public static EducationalSystem Create(EducationalSystemId id,string code, string name_Fr, string name_Ar)
    {
        return new EducationalSystem(id, code, name_Fr, name_Ar);
    }
}
