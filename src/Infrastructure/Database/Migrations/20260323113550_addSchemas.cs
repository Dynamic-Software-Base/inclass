using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Database.Migrations;

/// <inheritdoc />
public partial class addSchemas : Migration
{
    private static readonly string[] columns = new[] { "SchoolId", "GradeDefinitionId" };
    private static readonly string[] columnsArray = new[] { "school_id", "grade_definition_id" };
    private static readonly string[] columnsArray0 = new[] { "school_id", "grade_definition_id", "status" };
    private static readonly string[] columnsArray1 = new[] { "school_id", "grade_definition_id", "field_key" };
    private static readonly string[] columnsArray2 = new[] { "school_id", "status" };
    private static readonly string[] columnsArray3 = new[] { "school_id", "grade_definition_id", "is_active" };
    private static readonly string[] columnsArray4 = new[] { "student_id", "field_key" };
    private static readonly string[] columnsArray5 = new[] { "school_id", "grade_definition_id" };

    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "fk_grade_cycle_definitions_educational_systems_educational_sys",
            schema: "public",
            table: "grade_cycle_definitions");

        migrationBuilder.DropForeignKey(
            name: "fk_grade_definitions_grade_cycle_definitions_grade_cycle_defin",
            schema: "public",
            table: "grade_definitions");

        migrationBuilder.DropForeignKey(
            name: "fk_school_addresses_schools_school_id",
            schema: "public",
            table: "school_addresses");

        migrationBuilder.DropForeignKey(
            name: "fk_school_pictures_schools_school_id",
            schema: "public",
            table: "school_pictures");

        migrationBuilder.DropForeignKey(
            name: "fk_school_supported_grades_schools_school_id",
            schema: "public",
            table: "school_supported_grades");

        migrationBuilder.DropForeignKey(
            name: "fk_schools_educational_systems_educational_system_id",
            schema: "public",
            table: "schools");

        migrationBuilder.DropPrimaryKey(
            name: "pk_users",
            schema: "public",
            table: "users");

        migrationBuilder.DropPrimaryKey(
            name: "pk_user_school_memberships",
            schema: "public",
            table: "user_school_memberships");

        migrationBuilder.DropPrimaryKey(
            name: "pk_stored_files",
            schema: "public",
            table: "stored_files");

        migrationBuilder.DropPrimaryKey(
            name: "pk_schools",
            schema: "public",
            table: "schools");

        migrationBuilder.DropPrimaryKey(
            name: "pk_school_supported_grades",
            schema: "public",
            table: "school_supported_grades");

        migrationBuilder.DropPrimaryKey(
            name: "pk_school_pictures",
            schema: "public",
            table: "school_pictures");

        migrationBuilder.DropPrimaryKey(
            name: "pk_school_addresses",
            schema: "public",
            table: "school_addresses");

        migrationBuilder.DropPrimaryKey(
            name: "pk_invitations",
            schema: "public",
            table: "invitations");

        migrationBuilder.DropPrimaryKey(
            name: "pk_grade_definitions",
            schema: "public",
            table: "grade_definitions");

        migrationBuilder.DropPrimaryKey(
            name: "pk_grade_cycle_definitions",
            schema: "public",
            table: "grade_cycle_definitions");

        migrationBuilder.DropPrimaryKey(
            name: "pk_educational_systems",
            schema: "public",
            table: "educational_systems");

        migrationBuilder.RenameColumn(
            name: "email",
            schema: "public",
            table: "users",
            newName: "Email");

        migrationBuilder.RenameColumn(
            name: "id",
            schema: "public",
            table: "users",
            newName: "Id");

        migrationBuilder.RenameColumn(
            name: "phone_number",
            schema: "public",
            table: "users",
            newName: "PhoneNumber");

        migrationBuilder.RenameColumn(
            name: "last_modified_by",
            schema: "public",
            table: "users",
            newName: "LastModifiedBy");

        migrationBuilder.RenameColumn(
            name: "last_modified_at",
            schema: "public",
            table: "users",
            newName: "LastModifiedAt");

        migrationBuilder.RenameColumn(
            name: "is_active",
            schema: "public",
            table: "users",
            newName: "IsActive");

        migrationBuilder.RenameColumn(
            name: "full_name",
            schema: "public",
            table: "users",
            newName: "FullName");

        migrationBuilder.RenameColumn(
            name: "created_by",
            schema: "public",
            table: "users",
            newName: "CreatedBy");

        migrationBuilder.RenameColumn(
            name: "created_at",
            schema: "public",
            table: "users",
            newName: "CreatedAt");

        migrationBuilder.RenameIndex(
            name: "ix_users_email",
            schema: "public",
            table: "users",
            newName: "IX_users_Email");

        migrationBuilder.RenameIndex(
            name: "ix_users_phone_number",
            schema: "public",
            table: "users",
            newName: "IX_users_PhoneNumber");

        migrationBuilder.RenameColumn(
            name: "role",
            schema: "public",
            table: "user_school_memberships",
            newName: "Role");

        migrationBuilder.RenameColumn(
            name: "id",
            schema: "public",
            table: "user_school_memberships",
            newName: "Id");

        migrationBuilder.RenameColumn(
            name: "user_id",
            schema: "public",
            table: "user_school_memberships",
            newName: "UserId");

        migrationBuilder.RenameColumn(
            name: "school_id",
            schema: "public",
            table: "user_school_memberships",
            newName: "SchoolId");

        migrationBuilder.RenameColumn(
            name: "is_active",
            schema: "public",
            table: "user_school_memberships",
            newName: "IsActive");

        migrationBuilder.RenameIndex(
            name: "ix_user_school_memberships_user_id_school_id_role",
            schema: "public",
            table: "user_school_memberships",
            newName: "IX_user_school_memberships_UserId_SchoolId_Role");

        migrationBuilder.RenameIndex(
            name: "ix_stored_files_stored_file_name",
            schema: "public",
            table: "stored_files",
            newName: "IX_stored_files_stored_file_name");

        migrationBuilder.RenameIndex(
            name: "ix_stored_files_owner_id",
            schema: "public",
            table: "stored_files",
            newName: "IX_stored_files_owner_id");

        migrationBuilder.RenameIndex(
            name: "ix_schools_educational_system_id",
            schema: "public",
            table: "schools",
            newName: "IX_schools_educational_system_id");

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

        migrationBuilder.RenameIndex(
            name: "ix_school_pictures_school_id_stored_file_id",
            schema: "public",
            table: "school_pictures",
            newName: "IX_school_pictures_school_id_stored_file_id");

        migrationBuilder.RenameColumn(
            name: "status",
            schema: "public",
            table: "invitations",
            newName: "Status");

        migrationBuilder.RenameColumn(
            name: "role",
            schema: "public",
            table: "invitations",
            newName: "Role");

        migrationBuilder.RenameColumn(
            name: "id",
            schema: "public",
            table: "invitations",
            newName: "Id");

        migrationBuilder.RenameColumn(
            name: "token_hash",
            schema: "public",
            table: "invitations",
            newName: "TokenHash");

        migrationBuilder.RenameColumn(
            name: "target_value",
            schema: "public",
            table: "invitations",
            newName: "TargetValue");

        migrationBuilder.RenameColumn(
            name: "target_type",
            schema: "public",
            table: "invitations",
            newName: "TargetType");

        migrationBuilder.RenameColumn(
            name: "school_id",
            schema: "public",
            table: "invitations",
            newName: "SchoolId");

        migrationBuilder.RenameColumn(
            name: "last_modified_by",
            schema: "public",
            table: "invitations",
            newName: "LastModifiedBy");

        migrationBuilder.RenameColumn(
            name: "last_modified_at",
            schema: "public",
            table: "invitations",
            newName: "LastModifiedAt");

        migrationBuilder.RenameColumn(
            name: "expires_at",
            schema: "public",
            table: "invitations",
            newName: "ExpiresAt");

        migrationBuilder.RenameColumn(
            name: "created_by",
            schema: "public",
            table: "invitations",
            newName: "CreatedBy");

        migrationBuilder.RenameColumn(
            name: "created_at",
            schema: "public",
            table: "invitations",
            newName: "CreatedAt");

        migrationBuilder.RenameColumn(
            name: "accepted_by_user_id",
            schema: "public",
            table: "invitations",
            newName: "AcceptedByUserId");

        migrationBuilder.RenameColumn(
            name: "accepted_at",
            schema: "public",
            table: "invitations",
            newName: "AcceptedAt");

        migrationBuilder.RenameIndex(
            name: "ix_invitations_token_hash",
            schema: "public",
            table: "invitations",
            newName: "IX_invitations_TokenHash");

        migrationBuilder.RenameIndex(
            name: "ix_invitations_school_id_target_type_target_value_role_status",
            schema: "public",
            table: "invitations",
            newName: "IX_invitations_SchoolId_TargetType_TargetValue_Role_Status");

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
            name: "ix_grade_definitions_grade_cycle_definition_id",
            schema: "public",
            table: "grade_definitions",
            newName: "IX_grade_definitions_GradeCycleDefinitionId");

        migrationBuilder.RenameColumn(
            name: "id",
            schema: "public",
            table: "grade_cycle_definitions",
            newName: "Id");

        migrationBuilder.RenameIndex(
            name: "ix_grade_cycle_definitions_educational_system_id",
            schema: "public",
            table: "grade_cycle_definitions",
            newName: "IX_grade_cycle_definitions_educational_system_id");

        migrationBuilder.RenameColumn(
            name: "id",
            schema: "public",
            table: "educational_systems",
            newName: "Id");

        migrationBuilder.RenameIndex(
            name: "ix_educational_systems_code",
            schema: "public",
            table: "educational_systems",
            newName: "IX_educational_systems_code");

        migrationBuilder.AddPrimaryKey(
            name: "PK_users",
            schema: "public",
            table: "users",
            column: "Id");

        migrationBuilder.AddPrimaryKey(
            name: "PK_user_school_memberships",
            schema: "public",
            table: "user_school_memberships",
            column: "Id");

        migrationBuilder.AddPrimaryKey(
            name: "PK_stored_files",
            schema: "public",
            table: "stored_files",
            column: "id");

        migrationBuilder.AddPrimaryKey(
            name: "PK_schools",
            schema: "public",
            table: "schools",
            column: "id");

        migrationBuilder.AddPrimaryKey(
            name: "PK_school_supported_grades",
            schema: "public",
            table: "school_supported_grades",
            columns: columns);

        migrationBuilder.AddPrimaryKey(
            name: "PK_school_pictures",
            schema: "public",
            table: "school_pictures",
            column: "id");

        migrationBuilder.AddPrimaryKey(
            name: "PK_school_addresses",
            schema: "public",
            table: "school_addresses",
            column: "school_id");

        migrationBuilder.AddPrimaryKey(
            name: "PK_invitations",
            schema: "public",
            table: "invitations",
            column: "Id");

        migrationBuilder.AddPrimaryKey(
            name: "PK_grade_definitions",
            schema: "public",
            table: "grade_definitions",
            column: "Id");

        migrationBuilder.AddPrimaryKey(
            name: "PK_grade_cycle_definitions",
            schema: "public",
            table: "grade_cycle_definitions",
            column: "Id");

        migrationBuilder.AddPrimaryKey(
            name: "PK_educational_systems",
            schema: "public",
            table: "educational_systems",
            column: "Id");

        migrationBuilder.CreateTable(
            name: "parent_tuteurs",
            schema: "public",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                school_id = table.Column<Guid>(type: "uuid", nullable: false),
                registration_id = table.Column<Guid>(type: "uuid", nullable: false),
                first_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                last_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                cin = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                phone_number = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                secondary_phone_number = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                address_street = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                address_building_number = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                address_apartment = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                address_city = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                address_province = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                address_region = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                address_postal_code = table.Column<string>(type: "character varying(5)", maxLength: 5, nullable: true),
                address_latitude = table.Column<double>(type: "double precision", precision: 10, scale: 7, nullable: true),
                address_longitude = table.Column<double>(type: "double precision", precision: 10, scale: 7, nullable: true),
                relation = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                is_legal_guardian = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                created_by = table.Column<Guid>(type: "uuid", nullable: false),
                last_modified_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                last_modified_by = table.Column<Guid>(type: "uuid", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_parent_tuteurs", x => x.id);
            });

        migrationBuilder.CreateTable(
            name: "registration_form_schemas",
            schema: "public",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                school_id = table.Column<Guid>(type: "uuid", nullable: true),
                grade_definition_id = table.Column<Guid>(type: "uuid", nullable: false),
                schema_json = table.Column<string>(type: "jsonb", nullable: false, defaultValue: "{}"),
                version = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                is_customized = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                base_version = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false, defaultValue: ""),
                created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                created_by = table.Column<Guid>(type: "uuid", nullable: false),
                last_modified_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                last_modified_by = table.Column<Guid>(type: "uuid", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_registration_form_schemas", x => x.id);
            });

        migrationBuilder.CreateTable(
            name: "registration_sessions",
            schema: "public",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                school_id = table.Column<Guid>(type: "uuid", nullable: false),
                grade_definition_id = table.Column<Guid>(type: "uuid", nullable: false),
                form_schema_id = table.Column<Guid>(type: "uuid", nullable: false),
                academic_year_value = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                academic_year_start_year = table.Column<int>(type: "integer", nullable: false),
                academic_year_end_year = table.Column<int>(type: "integer", nullable: false),
                period_open_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                period_close_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                capacity_max_slots = table.Column<int>(type: "integer", nullable: true),
                status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                batch_id = table.Column<Guid>(type: "uuid", nullable: true),
                created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                created_by = table.Column<Guid>(type: "uuid", nullable: false),
                last_modified_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                last_modified_by = table.Column<Guid>(type: "uuid", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_registration_sessions", x => x.id);
            });

        migrationBuilder.CreateTable(
            name: "student_extended_data",
            schema: "public",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                student_id = table.Column<Guid>(type: "uuid", nullable: false),
                school_id = table.Column<Guid>(type: "uuid", nullable: false),
                grade_definition_id = table.Column<Guid>(type: "uuid", nullable: false),
                field_key = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                field_value = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                field_type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_student_extended_data", x => x.id);
            });

        migrationBuilder.CreateTable(
            name: "student_registrations",
            schema: "public",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                school_id = table.Column<Guid>(type: "uuid", nullable: false),
                session_id = table.Column<Guid>(type: "uuid", nullable: false),
                form_schema_id = table.Column<Guid>(type: "uuid", nullable: false),
                grade_definition_id = table.Column<Guid>(type: "uuid", nullable: false),
                form_values_json = table.Column<string>(type: "jsonb", nullable: false, defaultValue: "{}"),
                contact_email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                contact_phone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false, defaultValue: "Draft"),
                submitted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                review_reviewed_by = table.Column<Guid>(type: "uuid", nullable: true),
                review_reviewed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                review_comment = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                created_by = table.Column<Guid>(type: "uuid", nullable: false),
                last_modified_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                last_modified_by = table.Column<Guid>(type: "uuid", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_student_registrations", x => x.id);
            });

        migrationBuilder.CreateTable(
            name: "students",
            schema: "public",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                school_id = table.Column<Guid>(type: "uuid", nullable: false),
                registration_id = table.Column<Guid>(type: "uuid", nullable: false),
                grade_definition_id = table.Column<Guid>(type: "uuid", nullable: false),
                first_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                last_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                date_of_birth = table.Column<DateOnly>(type: "date", nullable: false),
                gender = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                national_id_value = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                national_id_type = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                academic_year_value = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                academic_year_start_year = table.Column<int>(type: "integer", nullable: false),
                academic_year_end_year = table.Column<int>(type: "integer", nullable: false),
                is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                created_by = table.Column<Guid>(type: "uuid", nullable: false),
                last_modified_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                last_modified_by = table.Column<Guid>(type: "uuid", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_students", x => x.id);
            });

        migrationBuilder.CreateTable(
            name: "parent_tuteur_students",
            schema: "public",
            columns: table => new
            {
                parent_tuteur_id = table.Column<Guid>(type: "uuid", nullable: false),
                student_id = table.Column<Guid>(type: "uuid", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_parent_tuteur_students", x => new { x.parent_tuteur_id, x.student_id });
                table.ForeignKey(
                    name: "fk_parent_tuteur_students_parent_tuteur_id",
                    column: x => x.parent_tuteur_id,
                    principalSchema: "public",
                    principalTable: "parent_tuteurs",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "fk_parent_tuteur_students_student_id",
                    column: x => x.student_id,
                    principalSchema: "public",
                    principalTable: "students",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "ix_parent_tuteur_students_student_id",
            schema: "public",
            table: "parent_tuteur_students",
            column: "student_id");

        migrationBuilder.CreateIndex(
            name: "ix_parent_tuteurs_registration_id",
            schema: "public",
            table: "parent_tuteurs",
            column: "registration_id");

        migrationBuilder.CreateIndex(
            name: "ix_parent_tuteurs_school_cin",
            schema: "public",
            table: "parent_tuteurs",
            column: "school_id");

        migrationBuilder.CreateIndex(
            name: "ix_registration_form_schemas_grade_definition_id",
            schema: "public",
            table: "registration_form_schemas",
            column: "grade_definition_id");

        migrationBuilder.CreateIndex(
            name: "ix_registration_form_schemas_school_grade",
            schema: "public",
            table: "registration_form_schemas",
            columns: columnsArray,
            unique: true);

        migrationBuilder.CreateIndex(
            name: "ix_registration_sessions_batch_id",
            schema: "public",
            table: "registration_sessions",
            column: "batch_id");

        migrationBuilder.CreateIndex(
            name: "ix_registration_sessions_form_schema_id",
            schema: "public",
            table: "registration_sessions",
            column: "form_schema_id");

        migrationBuilder.CreateIndex(
            name: "ix_registration_sessions_school_grade_status",
            schema: "public",
            table: "registration_sessions",
            columns: columnsArray0);

        migrationBuilder.CreateIndex(
            name: "ix_student_extended_data_school_grade_field_key",
            schema: "public",
            table: "student_extended_data",
            columns: columnsArray1);

        migrationBuilder.CreateIndex(
            name: "ix_student_extended_data_student_field_key_unique",
            schema: "public",
            table: "student_extended_data",
            columns: columnsArray4,
            unique: true);

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
            columns: columnsArray2);

        migrationBuilder.CreateIndex(
            name: "ix_student_registrations_session_id",
            schema: "public",
            table: "student_registrations",
            column: "session_id");

        migrationBuilder.CreateIndex(
            name: "ix_students_registration_id",
            schema: "public",
            table: "students",
            column: "registration_id");

        migrationBuilder.CreateIndex(
            name: "ix_students_school_grade_active",
            schema: "public",
            table: "students",
            columns: columnsArray3);

        migrationBuilder.AddForeignKey(
            name: "FK_grade_cycle_definitions_educational_systems_educational_sys~",
            schema: "public",
            table: "grade_cycle_definitions",
            column: "educational_system_id",
            principalSchema: "public",
            principalTable: "educational_systems",
            principalColumn: "Id",
            onDelete: ReferentialAction.Restrict);

        migrationBuilder.AddForeignKey(
            name: "FK_grade_definitions_grade_cycle_definitions_GradeCycleDefinit~",
            schema: "public",
            table: "grade_definitions",
            column: "GradeCycleDefinitionId",
            principalSchema: "public",
            principalTable: "grade_cycle_definitions",
            principalColumn: "Id",
            onDelete: ReferentialAction.Restrict);

        migrationBuilder.AddForeignKey(
            name: "FK_school_addresses_schools_school_id",
            schema: "public",
            table: "school_addresses",
            column: "school_id",
            principalSchema: "public",
            principalTable: "schools",
            principalColumn: "id",
            onDelete: ReferentialAction.Cascade);

        migrationBuilder.AddForeignKey(
            name: "FK_school_pictures_schools_school_id",
            schema: "public",
            table: "school_pictures",
            column: "school_id",
            principalSchema: "public",
            principalTable: "schools",
            principalColumn: "id",
            onDelete: ReferentialAction.Cascade);

        migrationBuilder.AddForeignKey(
            name: "FK_school_supported_grades_schools_SchoolId",
            schema: "public",
            table: "school_supported_grades",
            column: "SchoolId",
            principalSchema: "public",
            principalTable: "schools",
            principalColumn: "id",
            onDelete: ReferentialAction.Cascade);

        migrationBuilder.AddForeignKey(
            name: "FK_schools_educational_systems_educational_system_id",
            schema: "public",
            table: "schools",
            column: "educational_system_id",
            principalSchema: "public",
            principalTable: "educational_systems",
            principalColumn: "Id",
            onDelete: ReferentialAction.Restrict);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_grade_cycle_definitions_educational_systems_educational_sys~",
            schema: "public",
            table: "grade_cycle_definitions");

        migrationBuilder.DropForeignKey(
            name: "FK_grade_definitions_grade_cycle_definitions_GradeCycleDefinit~",
            schema: "public",
            table: "grade_definitions");

        migrationBuilder.DropForeignKey(
            name: "FK_school_addresses_schools_school_id",
            schema: "public",
            table: "school_addresses");

        migrationBuilder.DropForeignKey(
            name: "FK_school_pictures_schools_school_id",
            schema: "public",
            table: "school_pictures");

        migrationBuilder.DropForeignKey(
            name: "FK_school_supported_grades_schools_SchoolId",
            schema: "public",
            table: "school_supported_grades");

        migrationBuilder.DropForeignKey(
            name: "FK_schools_educational_systems_educational_system_id",
            schema: "public",
            table: "schools");

        migrationBuilder.DropTable(
            name: "parent_tuteur_students",
            schema: "public");

        migrationBuilder.DropTable(
            name: "registration_form_schemas",
            schema: "public");

        migrationBuilder.DropTable(
            name: "registration_sessions",
            schema: "public");

        migrationBuilder.DropTable(
            name: "student_extended_data",
            schema: "public");

        migrationBuilder.DropTable(
            name: "student_registrations",
            schema: "public");

        migrationBuilder.DropTable(
            name: "parent_tuteurs",
            schema: "public");

        migrationBuilder.DropTable(
            name: "students",
            schema: "public");

        migrationBuilder.DropPrimaryKey(
            name: "PK_users",
            schema: "public",
            table: "users");

        migrationBuilder.DropPrimaryKey(
            name: "PK_user_school_memberships",
            schema: "public",
            table: "user_school_memberships");

        migrationBuilder.DropPrimaryKey(
            name: "PK_stored_files",
            schema: "public",
            table: "stored_files");

        migrationBuilder.DropPrimaryKey(
            name: "PK_schools",
            schema: "public",
            table: "schools");

        migrationBuilder.DropPrimaryKey(
            name: "PK_school_supported_grades",
            schema: "public",
            table: "school_supported_grades");

        migrationBuilder.DropPrimaryKey(
            name: "PK_school_pictures",
            schema: "public",
            table: "school_pictures");

        migrationBuilder.DropPrimaryKey(
            name: "PK_school_addresses",
            schema: "public",
            table: "school_addresses");

        migrationBuilder.DropPrimaryKey(
            name: "PK_invitations",
            schema: "public",
            table: "invitations");

        migrationBuilder.DropPrimaryKey(
            name: "PK_grade_definitions",
            schema: "public",
            table: "grade_definitions");

        migrationBuilder.DropPrimaryKey(
            name: "PK_grade_cycle_definitions",
            schema: "public",
            table: "grade_cycle_definitions");

        migrationBuilder.DropPrimaryKey(
            name: "PK_educational_systems",
            schema: "public",
            table: "educational_systems");

        migrationBuilder.RenameColumn(
            name: "Email",
            schema: "public",
            table: "users",
            newName: "email");

        migrationBuilder.RenameColumn(
            name: "Id",
            schema: "public",
            table: "users",
            newName: "id");

        migrationBuilder.RenameColumn(
            name: "PhoneNumber",
            schema: "public",
            table: "users",
            newName: "phone_number");

        migrationBuilder.RenameColumn(
            name: "LastModifiedBy",
            schema: "public",
            table: "users",
            newName: "last_modified_by");

        migrationBuilder.RenameColumn(
            name: "LastModifiedAt",
            schema: "public",
            table: "users",
            newName: "last_modified_at");

        migrationBuilder.RenameColumn(
            name: "IsActive",
            schema: "public",
            table: "users",
            newName: "is_active");

        migrationBuilder.RenameColumn(
            name: "FullName",
            schema: "public",
            table: "users",
            newName: "full_name");

        migrationBuilder.RenameColumn(
            name: "CreatedBy",
            schema: "public",
            table: "users",
            newName: "created_by");

        migrationBuilder.RenameColumn(
            name: "CreatedAt",
            schema: "public",
            table: "users",
            newName: "created_at");

        migrationBuilder.RenameIndex(
            name: "IX_users_Email",
            schema: "public",
            table: "users",
            newName: "ix_users_email");

        migrationBuilder.RenameIndex(
            name: "IX_users_PhoneNumber",
            schema: "public",
            table: "users",
            newName: "ix_users_phone_number");

        migrationBuilder.RenameColumn(
            name: "Role",
            schema: "public",
            table: "user_school_memberships",
            newName: "role");

        migrationBuilder.RenameColumn(
            name: "Id",
            schema: "public",
            table: "user_school_memberships",
            newName: "id");

        migrationBuilder.RenameColumn(
            name: "UserId",
            schema: "public",
            table: "user_school_memberships",
            newName: "user_id");

        migrationBuilder.RenameColumn(
            name: "SchoolId",
            schema: "public",
            table: "user_school_memberships",
            newName: "school_id");

        migrationBuilder.RenameColumn(
            name: "IsActive",
            schema: "public",
            table: "user_school_memberships",
            newName: "is_active");

        migrationBuilder.RenameIndex(
            name: "IX_user_school_memberships_UserId_SchoolId_Role",
            schema: "public",
            table: "user_school_memberships",
            newName: "ix_user_school_memberships_user_id_school_id_role");

        migrationBuilder.RenameIndex(
            name: "IX_stored_files_stored_file_name",
            schema: "public",
            table: "stored_files",
            newName: "ix_stored_files_stored_file_name");

        migrationBuilder.RenameIndex(
            name: "IX_stored_files_owner_id",
            schema: "public",
            table: "stored_files",
            newName: "ix_stored_files_owner_id");

        migrationBuilder.RenameIndex(
            name: "IX_schools_educational_system_id",
            schema: "public",
            table: "schools",
            newName: "ix_schools_educational_system_id");

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

        migrationBuilder.RenameIndex(
            name: "IX_school_pictures_school_id_stored_file_id",
            schema: "public",
            table: "school_pictures",
            newName: "ix_school_pictures_school_id_stored_file_id");

        migrationBuilder.RenameColumn(
            name: "Status",
            schema: "public",
            table: "invitations",
            newName: "status");

        migrationBuilder.RenameColumn(
            name: "Role",
            schema: "public",
            table: "invitations",
            newName: "role");

        migrationBuilder.RenameColumn(
            name: "Id",
            schema: "public",
            table: "invitations",
            newName: "id");

        migrationBuilder.RenameColumn(
            name: "TokenHash",
            schema: "public",
            table: "invitations",
            newName: "token_hash");

        migrationBuilder.RenameColumn(
            name: "TargetValue",
            schema: "public",
            table: "invitations",
            newName: "target_value");

        migrationBuilder.RenameColumn(
            name: "TargetType",
            schema: "public",
            table: "invitations",
            newName: "target_type");

        migrationBuilder.RenameColumn(
            name: "SchoolId",
            schema: "public",
            table: "invitations",
            newName: "school_id");

        migrationBuilder.RenameColumn(
            name: "LastModifiedBy",
            schema: "public",
            table: "invitations",
            newName: "last_modified_by");

        migrationBuilder.RenameColumn(
            name: "LastModifiedAt",
            schema: "public",
            table: "invitations",
            newName: "last_modified_at");

        migrationBuilder.RenameColumn(
            name: "ExpiresAt",
            schema: "public",
            table: "invitations",
            newName: "expires_at");

        migrationBuilder.RenameColumn(
            name: "CreatedBy",
            schema: "public",
            table: "invitations",
            newName: "created_by");

        migrationBuilder.RenameColumn(
            name: "CreatedAt",
            schema: "public",
            table: "invitations",
            newName: "created_at");

        migrationBuilder.RenameColumn(
            name: "AcceptedByUserId",
            schema: "public",
            table: "invitations",
            newName: "accepted_by_user_id");

        migrationBuilder.RenameColumn(
            name: "AcceptedAt",
            schema: "public",
            table: "invitations",
            newName: "accepted_at");

        migrationBuilder.RenameIndex(
            name: "IX_invitations_TokenHash",
            schema: "public",
            table: "invitations",
            newName: "ix_invitations_token_hash");

        migrationBuilder.RenameIndex(
            name: "IX_invitations_SchoolId_TargetType_TargetValue_Role_Status",
            schema: "public",
            table: "invitations",
            newName: "ix_invitations_school_id_target_type_target_value_role_status");

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
            newName: "ix_grade_definitions_grade_cycle_definition_id");

        migrationBuilder.RenameColumn(
            name: "Id",
            schema: "public",
            table: "grade_cycle_definitions",
            newName: "id");

        migrationBuilder.RenameIndex(
            name: "IX_grade_cycle_definitions_educational_system_id",
            schema: "public",
            table: "grade_cycle_definitions",
            newName: "ix_grade_cycle_definitions_educational_system_id");

        migrationBuilder.RenameColumn(
            name: "Id",
            schema: "public",
            table: "educational_systems",
            newName: "id");

        migrationBuilder.RenameIndex(
            name: "IX_educational_systems_code",
            schema: "public",
            table: "educational_systems",
            newName: "ix_educational_systems_code");

        migrationBuilder.AddPrimaryKey(
            name: "pk_users",
            schema: "public",
            table: "users",
            column: "id");

        migrationBuilder.AddPrimaryKey(
            name: "pk_user_school_memberships",
            schema: "public",
            table: "user_school_memberships",
            column: "id");

        migrationBuilder.AddPrimaryKey(
            name: "pk_stored_files",
            schema: "public",
            table: "stored_files",
            column: "id");

        migrationBuilder.AddPrimaryKey(
            name: "pk_schools",
            schema: "public",
            table: "schools",
            column: "id");

        migrationBuilder.AddPrimaryKey(
            name: "pk_school_supported_grades",
            schema: "public",
            table: "school_supported_grades",
            columns: columnsArray5);

        migrationBuilder.AddPrimaryKey(
            name: "pk_school_pictures",
            schema: "public",
            table: "school_pictures",
            column: "id");

        migrationBuilder.AddPrimaryKey(
            name: "pk_school_addresses",
            schema: "public",
            table: "school_addresses",
            column: "school_id");

        migrationBuilder.AddPrimaryKey(
            name: "pk_invitations",
            schema: "public",
            table: "invitations",
            column: "id");

        migrationBuilder.AddPrimaryKey(
            name: "pk_grade_definitions",
            schema: "public",
            table: "grade_definitions",
            column: "id");

        migrationBuilder.AddPrimaryKey(
            name: "pk_grade_cycle_definitions",
            schema: "public",
            table: "grade_cycle_definitions",
            column: "id");

        migrationBuilder.AddPrimaryKey(
            name: "pk_educational_systems",
            schema: "public",
            table: "educational_systems",
            column: "id");

        migrationBuilder.AddForeignKey(
            name: "fk_grade_cycle_definitions_educational_systems_educational_sys",
            schema: "public",
            table: "grade_cycle_definitions",
            column: "educational_system_id",
            principalSchema: "public",
            principalTable: "educational_systems",
            principalColumn: "id",
            onDelete: ReferentialAction.Restrict);

        migrationBuilder.AddForeignKey(
            name: "fk_grade_definitions_grade_cycle_definitions_grade_cycle_defin",
            schema: "public",
            table: "grade_definitions",
            column: "grade_cycle_definition_id",
            principalSchema: "public",
            principalTable: "grade_cycle_definitions",
            principalColumn: "id",
            onDelete: ReferentialAction.Restrict);

        migrationBuilder.AddForeignKey(
            name: "fk_school_addresses_schools_school_id",
            schema: "public",
            table: "school_addresses",
            column: "school_id",
            principalSchema: "public",
            principalTable: "schools",
            principalColumn: "id",
            onDelete: ReferentialAction.Cascade);

        migrationBuilder.AddForeignKey(
            name: "fk_school_pictures_schools_school_id",
            schema: "public",
            table: "school_pictures",
            column: "school_id",
            principalSchema: "public",
            principalTable: "schools",
            principalColumn: "id",
            onDelete: ReferentialAction.Cascade);

        migrationBuilder.AddForeignKey(
            name: "fk_school_supported_grades_schools_school_id",
            schema: "public",
            table: "school_supported_grades",
            column: "school_id",
            principalSchema: "public",
            principalTable: "schools",
            principalColumn: "id",
            onDelete: ReferentialAction.Cascade);

        migrationBuilder.AddForeignKey(
            name: "fk_schools_educational_systems_educational_system_id",
            schema: "public",
            table: "schools",
            column: "educational_system_id",
            principalSchema: "public",
            principalTable: "educational_systems",
            principalColumn: "id",
            onDelete: ReferentialAction.Restrict);
    }
}
