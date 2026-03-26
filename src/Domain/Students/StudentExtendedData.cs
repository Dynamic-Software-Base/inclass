using Domain.Schools;
using SharedKernel;
using SharedKernel.ValueObjects.StronglyTypedIds;
using SharedKernel.ValueObjects.StronglyTypedIds.Registration;

namespace Domain.Students;
public sealed class StudentExtendedData : Entity<StudentExtendedData, StudentExtendedDataId>
{
    public StudentId StudentId { get; private set; }
    public string FieldKey { get; private set; } = string.Empty;
    public string? FieldValue { get; private set; }
    public string FieldType { get; private set; } = string.Empty;

    private StudentExtendedData() { }

    private StudentExtendedData(
        StudentExtendedDataId id,
        StudentId studentId,
        string fieldKey,
        string? fieldValue,
        string fieldType)
        : base(id)
    {
        StudentId = studentId;
        FieldKey = fieldKey;
        FieldValue = fieldValue;
        FieldType = fieldType;
    }

    public static StudentExtendedData Create(
        StudentExtendedDataId id,
        StudentId studentId,
        string fieldKey,
        string? fieldValue,
        string fieldType)
    {
        return new StudentExtendedData(
            id, studentId, fieldKey, fieldValue, fieldType);
    }

    public void UpdateValue(string? newValue)
    {
        FieldValue = newValue;
    }
}
