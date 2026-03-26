using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Database.Migrations;

/// <inheritdoc />
public partial class FixNamingSnakeCase : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_grade_definitions_grade_cycle_definitions_GradeCycleDefinit~",
            schema: "public",
            table: "grade_definitions");

        migrationBuilder.RenameColumn(
            name: "Id",
            schema: "public",
            table: "grade_definitions",
            newName: "id");

        migrationBuilder.RenameColumn(
            name: "GradeCycleDefinitionId",
            schema: "public",
            table: "grade_definitions",
            newName: "grade_cycle_definition_id");

        migrationBuilder.RenameIndex(
            name: "IX_grade_definitions_GradeCycleDefinitionId",
            schema: "public",
            table: "grade_definitions",
            newName: "IX_grade_definitions_grade_cycle_definition_id");

        migrationBuilder.RenameColumn(
            name: "Id",
            schema: "public",
            table: "grade_cycle_definitions",
            newName: "id");

        migrationBuilder.AddForeignKey(
            name: "FK_grade_definitions_grade_cycle_definitions_grade_cycle_defin~",
            schema: "public",
            table: "grade_definitions",
            column: "grade_cycle_definition_id",
            principalSchema: "public",
            principalTable: "grade_cycle_definitions",
            principalColumn: "id",
            onDelete: ReferentialAction.Restrict);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_grade_definitions_grade_cycle_definitions_grade_cycle_defin~",
            schema: "public",
            table: "grade_definitions");

        migrationBuilder.RenameColumn(
            name: "id",
            schema: "public",
            table: "grade_definitions",
            newName: "Id");

        migrationBuilder.RenameColumn(
            name: "grade_cycle_definition_id",
            schema: "public",
            table: "grade_definitions",
            newName: "GradeCycleDefinitionId");

        migrationBuilder.RenameIndex(
            name: "IX_grade_definitions_grade_cycle_definition_id",
            schema: "public",
            table: "grade_definitions",
            newName: "IX_grade_definitions_GradeCycleDefinitionId");

        migrationBuilder.RenameColumn(
            name: "id",
            schema: "public",
            table: "grade_cycle_definitions",
            newName: "Id");

        migrationBuilder.AddForeignKey(
            name: "FK_grade_definitions_grade_cycle_definitions_GradeCycleDefinit~",
            schema: "public",
            table: "grade_definitions",
            column: "GradeCycleDefinitionId",
            principalSchema: "public",
            principalTable: "grade_cycle_definitions",
            principalColumn: "Id",
            onDelete: ReferentialAction.Restrict);
    }
}
