namespace Domain.Students.Enum;

/// <summary>
/// Lien de parenté du tuteur avec l'élève.
/// </summary>
public enum ParentalRelation
{
    /// <summary>Père</summary>
    Father = 1,

    /// <summary>Mère</summary>
    Mother = 2,

    /// <summary>Tuteur légal</summary>
    LegalGuardian = 3,

    /// <summary>Autre</summary>
    Other = 4
}
