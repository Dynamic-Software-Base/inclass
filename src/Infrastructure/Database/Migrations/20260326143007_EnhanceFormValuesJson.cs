using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Database.Migrations;

/// <inheritdoc />
public partial class EnhanceFormValuesJson : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "application_field_values",
            schema: "public");

        migrationBuilder.AddColumn<string>(
            name: "form_values_json",
            schema: "public",
            table: "student_applications",
            type: "jsonb",
            nullable: false,
            defaultValue: "");
    }

    private static readonly string[] columns = new[] { "application_id", "field_key" };

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "form_values_json",
            schema: "public",
            table: "student_applications");

        migrationBuilder.CreateTable(
            name: "application_field_values",
            schema: "public",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                application_id = table.Column<Guid>(type: "uuid", nullable: false),
                field_key = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                field_type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                value = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                application_id1 = table.Column<Guid>(type: "uuid", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_application_field_values", x => x.id);
                table.ForeignKey(
                    name: "FK_application_field_values_student_applications_application_i~",
                    column: x => x.application_id1,
                    principalSchema: "public",
                    principalTable: "student_applications",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "ix_application_field_values_application_field",
            schema: "public",
            table: "application_field_values",
            columns: columns,
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_application_field_values_application_id1",
            schema: "public",
            table: "application_field_values",
            column: "application_id1");
    }
}
