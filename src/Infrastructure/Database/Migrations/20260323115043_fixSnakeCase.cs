using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Database.Migrations;

/// <inheritdoc />
public partial class fixSnakeCase : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_school_supported_grades_schools_SchoolId",
            schema: "public",
            table: "school_supported_grades");

        migrationBuilder.RenameColumn(
            name: "GradeDefinitionId",
            schema: "public",
            table: "school_supported_grades",
            newName: "grade_definition_id");

        migrationBuilder.RenameColumn(
            name: "SchoolId",
            schema: "public",
            table: "school_supported_grades",
            newName: "school_id");

        migrationBuilder.RenameColumn(
            name: "Id",
            schema: "public",
            table: "educational_systems",
            newName: "id");

        migrationBuilder.AddForeignKey(
            name: "FK_school_supported_grades_schools_school_id",
            schema: "public",
            table: "school_supported_grades",
            column: "school_id",
            principalSchema: "public",
            principalTable: "schools",
            principalColumn: "id",
            onDelete: ReferentialAction.Cascade);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_school_supported_grades_schools_school_id",
            schema: "public",
            table: "school_supported_grades");

        migrationBuilder.RenameColumn(
            name: "grade_definition_id",
            schema: "public",
            table: "school_supported_grades",
            newName: "GradeDefinitionId");

        migrationBuilder.RenameColumn(
            name: "school_id",
            schema: "public",
            table: "school_supported_grades",
            newName: "SchoolId");

        migrationBuilder.RenameColumn(
            name: "id",
            schema: "public",
            table: "educational_systems",
            newName: "Id");

        migrationBuilder.AddForeignKey(
            name: "FK_school_supported_grades_schools_SchoolId",
            schema: "public",
            table: "school_supported_grades",
            column: "SchoolId",
            principalSchema: "public",
            principalTable: "schools",
            principalColumn: "id",
            onDelete: ReferentialAction.Cascade);
    }
}
