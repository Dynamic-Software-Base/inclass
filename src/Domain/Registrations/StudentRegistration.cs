using Domain.Registrations.Enums;
using Domain.Registrations.Events;
using Domain.Schools;
using ErrorOr;
using SharedKernel;
using SharedKernel.ValueObjects;
using SharedKernel.ValueObjects.Registration;
using SharedKernel.ValueObjects.StronglyTypedIds;
using SharedKernel.ValueObjects.StronglyTypedIds.Registration;

namespace Domain.Registrations;

/// <summary>
/// Dossier d'inscription d'un élève soumis via l'endpoint public.
///
/// FormValuesJson est l'archive immuable — le snapshot exact de ce que le candidat a soumis.
/// Il n'est jamais modifié après soumission.
///
/// Lors de l'approbation, le handler lit FormValuesJson et le schéma associé pour :
///   - Créer le record Student (champs personnels)
///   - Créer le record ParentTuteur (champs parent)
///   - Créer les StudentExtendedData (champs dynamiques additionnels)
/// </summary>
public sealed class StudentRegistration
    : AggregateRoot<StudentRegistration, StudentRegistrationId>
{
    /// <summary>École cible</summary>
    public SchoolId SchoolId { get; private set; }

    /// <summary>Session dans laquelle ce dossier a été soumis</summary>
    public RegistrationSessionId SessionId { get; private set; }

    /// <summary>
    /// Snapshot du schéma utilisé au moment de la soumission.
    /// Préservé même si l'école modifie son schéma ultérieurement.
    /// </summary>
    public RegistrationFormSchemaId FormSchemaId { get; private set; }

    /// <summary>Niveau scolaire ciblé</summary>
    public GradeDefinitionId GradeDefinitionId { get; private set; }

    /// <summary>
    /// Archive immuable des réponses du candidat au format JSON.
    /// Structure : { "field_key": "value", ... }
    /// Jamais modifié après soumission.
    /// </summary>
    public string FormValuesJson { get; private set; } = "{}";

    /// <summary>
    /// Coordonnées de contact du candidat.
    /// Utilisées pour notifier l'approbation ou le rejet.
    /// </summary>
    public ApplicantContact ApplicantContact { get; private set; } = null!;

    /// <summary>Statut courant du dossier</summary>
    public RegistrationStatus Status { get; private set; } = RegistrationStatus.Draft;

    /// <summary>Date de soumission officielle</summary>
    public DateTime? SubmittedAt { get; private set; }

    /// <summary>
    /// Note de révision — créée lors de l'approbation ou du rejet.
    /// Null tant que le dossier n'a pas été traité.
    /// </summary>
    public ReviewNote? ReviewNote { get; private set; }

    private StudentRegistration() { }

    private StudentRegistration(
        StudentRegistrationId id,
        SchoolId schoolId,
        RegistrationSessionId sessionId,
        RegistrationFormSchemaId formSchemaId,
        GradeDefinitionId gradeDefinitionId,
        ApplicantContact applicantContact)
        : base(id,UserId.Empty)
    {
        SchoolId = schoolId;
        SessionId = sessionId;
        FormSchemaId = formSchemaId;
        GradeDefinitionId = gradeDefinitionId;
        ApplicantContact = applicantContact;
        Status = RegistrationStatus.Draft;
    }

    /// <summary>
    /// Démarre un brouillon — le candidat commence à remplir le formulaire.
    /// </summary>
    public static ErrorOr<StudentRegistration> StartDraft(
        StudentRegistrationId id,
        SchoolId schoolId,
        RegistrationSessionId sessionId,
        RegistrationFormSchemaId formSchemaId,
        GradeDefinitionId gradeDefinitionId,
        ApplicantContact applicantContact)
    {
        return new StudentRegistration(
            id, schoolId, sessionId, formSchemaId, gradeDefinitionId, applicantContact);
    }

    // --- Lifecycle ---

    /// <summary>
    /// Sauvegarde les réponses en cours (brouillon).
    /// Peut être appelé plusieurs fois avant soumission.
    /// </summary>
    public ErrorOr<Success> SaveDraft(string formValuesJson)
    {
        if (Status != RegistrationStatus.Draft)
        {
            return Error.Conflict("Registration.NotDraft",
                "Le dossier ne peut être modifié qu'en mode brouillon.");
        }


        if (string.IsNullOrWhiteSpace(formValuesJson))
        {
            return Error.Validation("Registration.FormValues.Empty",
                "Les données du formulaire sont obligatoires.");
        }


        FormValuesJson = formValuesJson;
        return Result.Success;
    }

    /// <summary>
    /// Soumet officiellement le dossier.
    /// FormValuesJson devient immuable après cet appel.
    /// La validation des champs obligatoires est faite dans le handler (nécessite le schéma).
    /// </summary>
    public ErrorOr<Success> Submit(string formValuesJson, DateTime submittedAt)
    {
        if (Status != RegistrationStatus.Draft)
        {
            return Error.Conflict("Registration.AlreadySubmitted",
                "Le dossier a déjà été soumis.");
        }


        if (string.IsNullOrWhiteSpace(formValuesJson))
        {
            return Error.Validation("Registration.FormValues.Empty",
                "Les données du formulaire sont obligatoires.");
        }


        FormValuesJson = formValuesJson;
        Status = RegistrationStatus.Submitted;
        SubmittedAt = submittedAt;

        return Result.Success;
    }

    public ErrorOr<Success> MarkUnderReview(UserId reviewerId)
    {
        if (Status != RegistrationStatus.Submitted)
        {
            return Error.Conflict("Registration.NotSubmitted",
                "Seuls les dossiers soumis peuvent être mis en révision.");
        }


        Status = RegistrationStatus.UnderReview;
        SetUpdated(DateTimeOffset.UtcNow, reviewerId);

        return Result.Success;
    }

    public ErrorOr<Success> Approve(UserId reviewerId, DateTime reviewedAt)
    {
        if (Status is not (RegistrationStatus.Submitted or RegistrationStatus.UnderReview))
        {
            return Error.Conflict("Registration.CannotApprove",
                "Le dossier doit être soumis ou en révision pour être approuvé.");
        }


        ErrorOr<ReviewNote> noteResult = ReviewNote.CreateApproval(reviewerId, reviewedAt);
        if (noteResult.IsError)
        {
            return noteResult.Errors;
        }

        Status = RegistrationStatus.Approved;
        ReviewNote = noteResult.Value;
        SetUpdated(DateTimeOffset.UtcNow, reviewerId);

        RaiseDomainEvent(new RegistrationApprovedEvent(
            Guid.NewGuid(), DateTime.UtcNow,
            Id, SchoolId, GradeDefinitionId, FormSchemaId,
            // AcademicYear resolved by handler via session
            null!));

        return Result.Success;
    }

    public ErrorOr<Success> Reject(UserId reviewerId, DateTime reviewedAt, string reason)
    {
        if (Status is not (RegistrationStatus.Submitted or RegistrationStatus.UnderReview))
        {
            return Error.Conflict("Registration.CannotReject",
                "Le dossier doit être soumis ou en révision pour être rejeté.");
        }


        ErrorOr<ReviewNote> noteResult = ReviewNote.CreateRejection(reviewerId, reviewedAt, reason);
        if (noteResult.IsError) {return noteResult.Errors;}

        Status = RegistrationStatus.Rejected;
        ReviewNote = noteResult.Value;
        SetUpdated(DateTimeOffset.UtcNow, reviewerId);

        RaiseDomainEvent(new RegistrationRejectedEvent(
            Guid.NewGuid(), DateTime.UtcNow,
            Id, SchoolId, reason));

        return Result.Success;
    }
}
