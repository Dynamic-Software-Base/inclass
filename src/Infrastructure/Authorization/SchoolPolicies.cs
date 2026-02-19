namespace Infrastructure.Authorization;

public static class SchoolPolicies
{
    public const string OwnerOnly = "School.OwnerOnly";
    public const string OwnerOrAdmin = "School.OwnerOrAdmin";
    public const string TeacherOnly = "School.TeacherOnly";
}
