using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Infrastructure.Database.Migrations;

/// <inheritdoc />
public partial class addRegistrationMigration : Migration
{
    private static readonly string[] columns = new[] { "school_id", "status" };
    private static readonly string[] columnsArray = new[] { "session_id", "status" };
    private static readonly string[] columnsArray0 = new[] { "student_id", "session_id" };

    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<int>(
            name: "capacity_max_slots",
            schema: "public",
            table: "registration_sessions",
            type: "integer",
            nullable: false,
            defaultValue: 0,
            oldClrType: typeof(int),
            oldType: "integer",
            oldNullable: true);

        migrationBuilder.AddColumn<string>(
            name: "assignment_strategy",
            schema: "public",
            table: "registration_sessions",
            type: "character varying(50)",
            maxLength: 50,
            nullable: false,
            defaultValue: "");

        migrationBuilder.AddColumn<TimeOnly>(
            name: "daily_cutoff_time",
            schema: "public",
            table: "registration_sessions",
            type: "time without time zone",
            nullable: false,
            defaultValue: new TimeOnly(0, 0, 0));

        migrationBuilder.AddColumn<int>(
            name: "daily_processing_quota",
            schema: "public",
            table: "registration_sessions",
            type: "integer",
            nullable: true);

        migrationBuilder.AddColumn<int>(
            name: "enrolled_count",
            schema: "public",
            table: "registration_sessions",
            type: "integer",
            nullable: false,
            defaultValue: 0);

        migrationBuilder.AddColumn<int>(
            name: "reserved_count",
            schema: "public",
            table: "registration_sessions",
            type: "integer",
            nullable: false,
            defaultValue: 0);

        migrationBuilder.CreateTable(
            name: "registration_session_phases",
            schema: "public",
            columns: table => new
            {
                id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                start_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                end_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                phase_type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                allowed_applicant_type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                session_id = table.Column<Guid>(type: "uuid", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_registration_session_phases", x => x.id);
                table.ForeignKey(
                    name: "FK_registration_session_phases_registration_sessions_session_id",
                    column: x => x.session_id,
                    principalSchema: "public",
                    principalTable: "registration_sessions",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "student_applications",
            schema: "public",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                session_id = table.Column<Guid>(type: "uuid", nullable: false),
                school_id = table.Column<Guid>(type: "uuid", nullable: false),
                target_grade_id = table.Column<Guid>(type: "uuid", nullable: false),
                academic_year_value = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                academic_year_start_year = table.Column<int>(type: "integer", nullable: false),
                academic_year_end_year = table.Column<int>(type: "integer", nullable: false),
                application_type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                submitted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                queue_position = table.Column<int>(type: "integer", nullable: true),
                queue_processing_date = table.Column<DateOnly>(type: "date", nullable: true),
                expiry_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                reschedule_used = table.Column<bool>(type: "boolean", nullable: false),
                assigned_class_id = table.Column<Guid>(type: "uuid", nullable: true),
                payment_status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                student_id = table.Column<Guid>(type: "uuid", nullable: false),
                parent_id = table.Column<Guid>(type: "uuid", nullable: false),
                created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                created_by = table.Column<Guid>(type: "uuid", nullable: false),
                last_modified_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                last_modified_by = table.Column<Guid>(type: "uuid", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_student_applications", x => x.id);
            });

        migrationBuilder.CreateIndex(
            name: "IX_registration_session_phases_session_id",
            schema: "public",
            table: "registration_session_phases",
            column: "session_id");

        migrationBuilder.CreateIndex(
            name: "ix_student_applications_parent_id",
            schema: "public",
            table: "student_applications",
            column: "parent_id");

        migrationBuilder.CreateIndex(
            name: "ix_student_applications_school_status",
            schema: "public",
            table: "student_applications",
            columns: columns);

        migrationBuilder.CreateIndex(
            name: "ix_student_applications_session_status",
            schema: "public",
            table: "student_applications",
            columns: columnsArray);

        migrationBuilder.CreateIndex(
            name: "ix_student_applications_student_session",
            schema: "public",
            table: "student_applications",
            columns: columnsArray0,
            unique: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "registration_session_phases",
            schema: "public");

        migrationBuilder.DropTable(
            name: "student_applications",
            schema: "public");

        migrationBuilder.DropColumn(
            name: "assignment_strategy",
            schema: "public",
            table: "registration_sessions");

        migrationBuilder.DropColumn(
            name: "daily_cutoff_time",
            schema: "public",
            table: "registration_sessions");

        migrationBuilder.DropColumn(
            name: "daily_processing_quota",
            schema: "public",
            table: "registration_sessions");

        migrationBuilder.DropColumn(
            name: "enrolled_count",
            schema: "public",
            table: "registration_sessions");

        migrationBuilder.DropColumn(
            name: "reserved_count",
            schema: "public",
            table: "registration_sessions");

        migrationBuilder.AlterColumn<int>(
            name: "capacity_max_slots",
            schema: "public",
            table: "registration_sessions",
            type: "integer",
            nullable: true,
            oldClrType: typeof(int),
            oldType: "integer");
    }
}
