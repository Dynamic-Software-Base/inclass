namespace Domain.Registrations.Enums;

/// <summary>
/// Statut d'une session d'inscription.
/// </summary>
public enum RegistrationSessionStatus
{
    /// <summary>Créée mais pas encore ouverte — OpenDate dans le futur</summary>
    Scheduled = 1,

    /// <summary>Ouverte — accepte des dossiers</summary>
    Open = 2,

    /// <summary>Fermée — plus d'inscriptions acceptées</summary>
    Closed = 3,

    /// <summary>Annulée par l'école avant ouverture</summary>
    Cancelled = 4
}
