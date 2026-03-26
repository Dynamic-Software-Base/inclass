using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Database.Migrations;

/// <inheritdoc />
public partial class AddStudentFieldValue : Migration
{
    private static readonly string[] columns = new[] { "field_key", "field_value" };
    private static readonly string[] columnsArray = new[] { "application_id", "field_key" };
    private static readonly string[] columnsArray0 = new[] { "school_id", "grade_definition_id", "field_key" };
    private static readonly string[] columnsArray1 = new[] { "school_id", "status" };

    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "student_registrations",
            schema: "public");

        migrationBuilder.DropIndex(
            name: "ix_student_extended_data_school_grade_field_key",
            schema: "public",
            table: "student_extended_data");

        migrationBuilder.DropIndex(
            name: "ix_student_extended_data_student_id",
            schema: "public",
            table: "student_extended_data");

        migrationBuilder.DropColumn(
            name: "grade_definition_id",
            schema: "public",
            table: "student_extended_data");

        migrationBuilder.DropColumn(
            name: "school_id",
            schema: "public",
            table: "student_extended_data");

        migrationBuilder.RenameIndex(
            name: "ix_student_extended_data_student_field_key_unique",
            schema: "public",
            table: "student_extended_data",
            newName: "ix_student_extended_data_student_field");

        migrationBuilder.AddColumn<string>(
            name: "contact_email",
            schema: "public",
            table: "student_applications",
            type: "character varying(255)",
            maxLength: 255,
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "contact_phone",
            schema: "public",
            table: "student_applications",
            type: "character varying(20)",
            maxLength: 20,
            nullable: true);

        migrationBuilder.CreateTable(
            name: "application_field_values",
            schema: "public",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                application_id = table.Column<Guid>(type: "uuid", nullable: false),
                field_key = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                value = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                field_type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
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
            name: "ix_student_extended_data_field_value",
            schema: "public",
            table: "student_extended_data",
            columns: columns);

        migrationBuilder.CreateIndex(
            name: "ix_application_field_values_application_field",
            schema: "public",
            table: "application_field_values",
            columns: columnsArray,
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_application_field_values_application_id1",
            schema: "public",
            table: "application_field_values",
            column: "application_id1");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "application_field_values",
            schema: "public");

        migrationBuilder.DropIndex(
            name: "ix_student_extended_data_field_value",
            schema: "public",
            table: "student_extended_data");

        migrationBuilder.DropColumn(
            name: "contact_email",
            schema: "public",
            table: "student_applications");

        migrationBuilder.DropColumn(
            name: "contact_phone",
            schema: "public",
            table: "student_applications");

        migrationBuilder.RenameIndex(
            name: "ix_student_extended_data_student_field",
            schema: "public",
            table: "student_extended_data",
            newName: "ix_student_extended_data_student_field_key_unique");

        migrationBuilder.AddColumn<Guid>(
            name: "grade_definition_id",
            schema: "public",
            table: "student_extended_data",
            type: "uuid",
            nullable: false,
            defaultValue: Guid.Empty);

        migrationBuilder.AddColumn<Guid>(
            name: "school_id",
            schema: "public",
            table: "student_extended_data",
            type: "uuid",
            nullable: false,
            defaultValue: Guid.Empty);

        migrationBuilder.CreateTable(
            name: "student_registrations",
            schema: "public",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                created_by = table.Column<Guid>(type: "uuid", nullable: false),
                form_schema_id = table.Column<Guid>(type: "uuid", nullable: false),
                form_values_json = table.Column<string>(type: "jsonb", nullable: false, defaultValue: "{}"),
                grade_definition_id = table.Column<Guid>(type: "uuid", nullable: false),
                last_modified_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                last_modified_by = table.Column<Guid>(type: "uuid", nullable: false),
                school_id = table.Column<Guid>(type: "uuid", nullable: false),
                session_id = table.Column<Guid>(type: "uuid", nullable: false),
                status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false, defaultValue: "Draft"),
                submitted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                contact_email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                contact_phone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                review_comment = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                review_reviewed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                review_reviewed_by = table.Column<Guid>(type: "uuid", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_student_registrations", x => x.id);
            });

        migrationBuilder.CreateIndex(
            name: "ix_student_extended_data_school_grade_field_key",
            schema: "public",
            table: "student_extended_data",
            columns: columnsArray0);

        migrationBuilder.CreateIndex(
            name: "ix_student_extended_data_student_id",
            schema: "public",
            table: "student_extended_data",
            column: "student_id");

        migrationBuilder.CreateIndex(
            name: "ix_student_registrations_form_schema_id",
            schema: "public",
            table: "student_registrations",
            column: "form_schema_id");

        migrationBuilder.CreateIndex(
            name: "ix_student_registrations_school_status",
            schema: "public",
            table: "student_registrations",
            columns: columnsArray1);

        migrationBuilder.CreateIndex(
            name: "ix_student_registrations_session_id",
            schema: "public",
            table: "student_registrations",
            column: "session_id");
    }
}
