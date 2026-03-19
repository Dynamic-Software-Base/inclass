namespace Domain.Registrations.Enums;

/// <summary>
/// Statut d'un dossier d'inscription élève.
/// </summary>
public enum RegistrationStatus
{
    /// <summary>Brouillon — non encore soumis par le candidat</summary>
    Draft = 1,

    /// <summary>Soumis — en attente de traitement par l'école</summary>
    Submitted = 2,

    /// <summary>En cours de révision par le personnel de l'école</summary>
    UnderReview = 3,

    /// <summary>Approuvé — élève et tuteur créés</summary>
    Approved = 4,

    /// <summary>Rejeté — motif fourni dans ReviewNote</summary>
    Rejected = 5
}
