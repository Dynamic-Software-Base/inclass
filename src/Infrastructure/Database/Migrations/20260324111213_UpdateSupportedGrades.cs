using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Database.Migrations;

/// <inheritdoc />
public partial class UpdateSupportedGrades : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.RenameColumn(
            name: "Capacity",
            schema: "public",
            table: "school_supported_grades",
            newName: "capacity");

        migrationBuilder.RenameColumn(
            name: "IsOffered",
            schema: "public",
            table: "school_supported_grades",
            newName: "is_offered");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.RenameColumn(
            name: "capacity",
            schema: "public",
            table: "school_supported_grades",
            newName: "Capacity");

        migrationBuilder.RenameColumn(
            name: "is_offered",
            schema: "public",
            table: "school_supported_grades",
            newName: "IsOffered");
    }
}
