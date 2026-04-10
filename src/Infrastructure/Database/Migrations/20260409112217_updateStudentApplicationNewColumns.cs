using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Database.Migrations;

/// <inheritdoc />
public partial class updateStudentApplicationNewColumns : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.RenameColumn(
            name: "StudentLastName",
            schema: "public",
            table: "student_applications",
            newName: "student_last_name");

        migrationBuilder.RenameColumn(
            name: "StudentFirstName",
            schema: "public",
            table: "student_applications",
            newName: "student_first_name");

        migrationBuilder.RenameColumn(
            name: "IdentityKey",
            schema: "public",
            table: "student_applications",
            newName: "identity_key");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.RenameColumn(
            name: "student_last_name",
            schema: "public",
            table: "student_applications",
            newName: "StudentLastName");

        migrationBuilder.RenameColumn(
            name: "student_first_name",
            schema: "public",
            table: "student_applications",
            newName: "StudentFirstName");

        migrationBuilder.RenameColumn(
            name: "identity_key",
            schema: "public",
            table: "student_applications",
            newName: "IdentityKey");
    }
}
