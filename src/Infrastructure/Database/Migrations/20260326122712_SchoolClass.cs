using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Database.Migrations;

/// <inheritdoc />
public partial class SchoolClass : Migration
{
    private static readonly string[] columns = new[] { "school_id", "grade_definition_id" };

    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "school_classes",
            schema: "public",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                school_id = table.Column<Guid>(type: "uuid", nullable: false),
                grade_definition_id = table.Column<Guid>(type: "uuid", nullable: false),
                academic_year_value = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                academic_year_start_year = table.Column<int>(type: "integer", nullable: false),
                academic_year_end_year = table.Column<int>(type: "integer", nullable: false),
                name = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                capacity_max_students = table.Column<int>(type: "integer", nullable: false),
                gender = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                current_enrollment_count = table.Column<int>(type: "integer", nullable: false),
                created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                created_by = table.Column<Guid>(type: "uuid", nullable: false),
                last_modified_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                last_modified_by = table.Column<Guid>(type: "uuid", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_school_classes", x => x.id);
            });

        migrationBuilder.CreateIndex(
            name: "ix_school_classes_school_grade",
            schema: "public",
            table: "school_classes",
            columns: columns);

        migrationBuilder.CreateIndex(
            name: "ix_school_classes_school_id",
            schema: "public",
            table: "school_classes",
            column: "school_id");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "school_classes",
            schema: "public");
    }
}
