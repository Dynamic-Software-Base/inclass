using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Infrastructure.Database.Migrations;

/// <inheritdoc />
public partial class Initial : Migration
{
    private static readonly string[] columns = new[] { "school_id", "target_type", "target_value", "role", "status" };
    private static readonly string[] columnsArray = new[] { "school_id", "stored_file_id" };
    private static readonly string[] columnsArray0 = new[] { "owner_user_id", "name" };
    private static readonly string[] columnsArray1 = new[] { "user_id", "school_id", "role" };

    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.EnsureSchema(
            name: "public");

        migrationBuilder.CreateTable(
            name: "educational_systems",
            schema: "public",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                code = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                name_fr = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                name_ar = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_educational_systems", x => x.id);
            });

        migrationBuilder.CreateTable(
            name: "invitations",
            schema: "public",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                school_id = table.Column<Guid>(type: "uuid", nullable: false),
                role = table.Column<int>(type: "integer", nullable: false),
                target_type = table.Column<int>(type: "integer", nullable: false),
                target_value = table.Column<string>(type: "character varying(320)", maxLength: 320, nullable: false),
                token_hash = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                expires_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                status = table.Column<int>(type: "integer", nullable: false),
                accepted_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                accepted_by_user_id = table.Column<Guid>(type: "uuid", nullable: true),
                created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                created_by = table.Column<Guid>(type: "uuid", nullable: false),
                last_modified_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                last_modified_by = table.Column<Guid>(type: "uuid", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_invitations", x => x.id);
            });

        migrationBuilder.CreateTable(
            name: "stored_files",
            schema: "public",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                owner_id = table.Column<Guid>(type: "uuid", nullable: false),
                original_file_name = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                stored_file_name = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                content_type = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                size_in_bytes = table.Column<long>(type: "bigint", nullable: false),
                url = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                storage_provider = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                blob_path = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                created_by = table.Column<Guid>(type: "uuid", nullable: false),
                last_modified_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                last_modified_by = table.Column<Guid>(type: "uuid", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_stored_files", x => x.id);
            });

        migrationBuilder.CreateTable(
            name: "user_school_memberships",
            schema: "public",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                user_id = table.Column<Guid>(type: "uuid", nullable: false),
                school_id = table.Column<Guid>(type: "uuid", nullable: false),
                role = table.Column<int>(type: "integer", nullable: false),
                is_active = table.Column<bool>(type: "boolean", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_user_school_memberships", x => x.id);
            });

        migrationBuilder.CreateTable(
            name: "users",
            schema: "public",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                email = table.Column<string>(type: "character varying(320)", maxLength: 320, nullable: true),
                phone_number = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                full_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                is_active = table.Column<bool>(type: "boolean", nullable: false),
                created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                created_by = table.Column<Guid>(type: "uuid", nullable: false),
                last_modified_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                last_modified_by = table.Column<Guid>(type: "uuid", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_users", x => x.id);
            });

        migrationBuilder.CreateTable(
            name: "grade_cycle_definitions",
            schema: "public",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                educational_system_id = table.Column<Guid>(type: "uuid", nullable: false),
                code = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                name_fr = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                name_ar = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                broad_level = table.Column<int>(type: "integer", nullable: false),
                sort_order = table.Column<int>(type: "integer", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_grade_cycle_definitions", x => x.id);
                table.ForeignKey(
                    name: "fk_grade_cycle_definitions_educational_systems_educational_sys",
                    column: x => x.educational_system_id,
                    principalSchema: "public",
                    principalTable: "educational_systems",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "schools",
            schema: "public",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                owner_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                educational_system_id = table.Column<Guid>(type: "uuid", nullable: false),
                name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                ar_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                primary_phone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                secondary_phone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                email = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                grade_levels = table.Column<int>(type: "integer", nullable: false),
                created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                created_by = table.Column<Guid>(type: "uuid", nullable: false),
                last_modified_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                last_modified_by = table.Column<Guid>(type: "uuid", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_schools", x => x.id);
                table.ForeignKey(
                    name: "fk_schools_educational_systems_educational_system_id",
                    column: x => x.educational_system_id,
                    principalSchema: "public",
                    principalTable: "educational_systems",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "grade_definitions",
            schema: "public",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                grade_cycle_definition_id = table.Column<Guid>(type: "uuid", nullable: false),
                code = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                name_fr = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                name_ar = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                sort_order = table.Column<int>(type: "integer", nullable: false),
                is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_grade_definitions", x => x.id);
                table.ForeignKey(
                    name: "fk_grade_definitions_grade_cycle_definitions_grade_cycle_defin",
                    column: x => x.grade_cycle_definition_id,
                    principalSchema: "public",
                    principalTable: "grade_cycle_definitions",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "school_addresses",
            schema: "public",
            columns: table => new
            {
                school_id = table.Column<Guid>(type: "uuid", nullable: false),
                street_address = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                building_number = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                apartment = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                city = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                province = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                region = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                postal_code = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                latitude = table.Column<double>(type: "double precision", nullable: true),
                longitude = table.Column<double>(type: "double precision", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_school_addresses", x => x.school_id);
                table.ForeignKey(
                    name: "fk_school_addresses_schools_school_id",
                    column: x => x.school_id,
                    principalSchema: "public",
                    principalTable: "schools",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "school_pictures",
            schema: "public",
            columns: table => new
            {
                id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                stored_file_id = table.Column<Guid>(type: "uuid", nullable: false),
                alt_text = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                is_main = table.Column<bool>(type: "boolean", nullable: false),
                school_id = table.Column<Guid>(type: "uuid", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_school_pictures", x => x.id);
                table.ForeignKey(
                    name: "fk_school_pictures_schools_school_id",
                    column: x => x.school_id,
                    principalSchema: "public",
                    principalTable: "schools",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "school_supported_grades",
            schema: "public",
            columns: table => new
            {
                school_id = table.Column<Guid>(type: "uuid", nullable: false),
                grade_definition_id = table.Column<Guid>(type: "uuid", nullable: false),
                cached_broad_level = table.Column<int>(type: "integer", nullable: false),
                is_offered = table.Column<bool>(type: "boolean", nullable: false),
                capacity = table.Column<int>(type: "integer", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_school_supported_grades", x => new { x.school_id, x.grade_definition_id });
                table.ForeignKey(
                    name: "fk_school_supported_grades_schools_school_id",
                    column: x => x.school_id,
                    principalSchema: "public",
                    principalTable: "schools",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "ix_educational_systems_code",
            schema: "public",
            table: "educational_systems",
            column: "code",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "ix_grade_cycle_definitions_educational_system_id",
            schema: "public",
            table: "grade_cycle_definitions",
            column: "educational_system_id");

        migrationBuilder.CreateIndex(
            name: "ix_grade_definitions_grade_cycle_definition_id",
            schema: "public",
            table: "grade_definitions",
            column: "grade_cycle_definition_id");

        migrationBuilder.CreateIndex(
            name: "ix_invitations_school_id_target_type_target_value_role_status",
            schema: "public",
            table: "invitations",
            columns: columns);

        migrationBuilder.CreateIndex(
            name: "ix_invitations_token_hash",
            schema: "public",
            table: "invitations",
            column: "token_hash",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "ix_school_pictures_school_id_stored_file_id",
            schema: "public",
            table: "school_pictures",
            columns: columnsArray,
            unique: true);

        migrationBuilder.CreateIndex(
            name: "ix_schools_educational_system_id",
            schema: "public",
            table: "schools",
            column: "educational_system_id");

        migrationBuilder.CreateIndex(
            name: "ix_schools_name",
            schema: "public",
            table: "schools",
            column: "name");

        migrationBuilder.CreateIndex(
            name: "ix_schools_owner_name_unique",
            schema: "public",
            table: "schools",
            columns: columnsArray0,
            unique: true);

        migrationBuilder.CreateIndex(
            name: "ix_schools_owner_user_id",
            schema: "public",
            table: "schools",
            column: "owner_user_id");

        migrationBuilder.CreateIndex(
            name: "ix_stored_files_owner_id",
            schema: "public",
            table: "stored_files",
            column: "owner_id");

        migrationBuilder.CreateIndex(
            name: "ix_stored_files_stored_file_name",
            schema: "public",
            table: "stored_files",
            column: "stored_file_name",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "ix_user_school_memberships_user_id_school_id_role",
            schema: "public",
            table: "user_school_memberships",
            columns: columnsArray1,
            unique: true);

        migrationBuilder.CreateIndex(
            name: "ix_users_email",
            schema: "public",
            table: "users",
            column: "email",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "ix_users_phone_number",
            schema: "public",
            table: "users",
            column: "phone_number",
            unique: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "grade_definitions",
            schema: "public");

        migrationBuilder.DropTable(
            name: "invitations",
            schema: "public");

        migrationBuilder.DropTable(
            name: "school_addresses",
            schema: "public");

        migrationBuilder.DropTable(
            name: "school_pictures",
            schema: "public");

        migrationBuilder.DropTable(
            name: "school_supported_grades",
            schema: "public");

        migrationBuilder.DropTable(
            name: "stored_files",
            schema: "public");

        migrationBuilder.DropTable(
            name: "user_school_memberships",
            schema: "public");

        migrationBuilder.DropTable(
            name: "users",
            schema: "public");

        migrationBuilder.DropTable(
            name: "grade_cycle_definitions",
            schema: "public");

        migrationBuilder.DropTable(
            name: "schools",
            schema: "public");

        migrationBuilder.DropTable(
            name: "educational_systems",
            schema: "public");
    }
}
