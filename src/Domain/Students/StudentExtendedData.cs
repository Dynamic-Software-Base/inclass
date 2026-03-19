using Domain.Schools;
using SharedKernel;
using SharedKernel.ValueObjects.StronglyTypedIds;
using SharedKernel.ValueObjects.StronglyTypedIds.Registration;

namespace Domain.Students;

/// <summary>
/// Données dynamiques d'un élève issues des champs personnalisés du formulaire d'inscription.
///
/// Créées lors de l'approbation d'un dossier — le handler promeut les champs
/// non-core du FormValuesJson vers cette table EAV.
///
/// L'index composite (SchoolId, GradeDefinitionId, FieldKey, FieldValue) rend
/// les filtres dynamiques performants sans scan complet de la table.
///
/// Ce n'est PAS un aggregate root — pas de lifecycle, pas d'événements domaine.
/// Écrit par le handler d'approbation, lu par les queries de filtrage.
/// </summary>
public sealed class StudentExtendedData : Entity<StudentExtendedData, StudentExtendedDataId>
{
    /// <summary>Élève auquel appartient cette donnée</summary>
    public StudentId StudentId { get; private set; }

    /// <summary>
    /// École — dupliqué ici pour l'index composite.
    /// Évite une jointure avec Student sur chaque requête de filtrage.
    /// </summary>
    public SchoolId SchoolId { get; private set; }

    /// <summary>
    /// Niveau scolaire — dupliqué pour l'index composite.
    /// Permet de filtrer "tous les élèves de 7ème avec blood_type = O+".
    /// </summary>
    public GradeDefinitionId GradeDefinitionId { get; private set; }

    /// <summary>
    /// Clé du champ — correspond à FormField.Key dans le schéma JSON.
    /// Ex: "blood_type", "previous_school_name", "has_learning_disabilities"
    /// </summary>
    public string FieldKey { get; private set; } = string.Empty;

    /// <summary>
    /// Valeur stockée comme string — tous les types sont sérialisés.
    /// Bool → "true"/"false", Date → "2008-03-15", Select → "O+"
    /// </summary>
    public string? FieldValue { get; private set; }

    /// <summary>
    /// Type du champ — copié depuis le schéma au moment de la promotion.
    /// Permet à l'UI de rendre le bon input de filtre (dropdown, date picker, etc.)
    /// Ex: "select", "boolean", "date", "text"
    /// </summary>
    public string FieldType { get; private set; } = string.Empty;

    private StudentExtendedData() { }

    private StudentExtendedData(
        StudentExtendedDataId id,
        StudentId studentId,
        SchoolId schoolId,
        GradeDefinitionId gradeDefinitionId,
        string fieldKey,
        string? fieldValue,
        string fieldType)
        : base(id)
    {
        StudentId = studentId;
        SchoolId = schoolId;
        GradeDefinitionId = gradeDefinitionId;
        FieldKey = fieldKey;
        FieldValue = fieldValue;
        FieldType = fieldType;
    }

    public static StudentExtendedData Create(
        StudentExtendedDataId id,
        StudentId studentId,
        SchoolId schoolId,
        GradeDefinitionId gradeDefinitionId,
        string fieldKey,
        string? fieldValue,
        string fieldType)
    {
        return new StudentExtendedData(
            id, studentId, schoolId, gradeDefinitionId,
            fieldKey, fieldValue, fieldType);
    }
}
