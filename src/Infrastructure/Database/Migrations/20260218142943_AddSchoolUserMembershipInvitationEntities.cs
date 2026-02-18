using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable
#pragma warning disable CA1861

namespace Infrastructure.Database.Migrations;

/// <inheritdoc />
public partial class AddSchoolUserMembershipInvitationEntities : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "todo_items",
            schema: "public");

        migrationBuilder.DropColumn(
            name: "first_name",
            schema: "public",
            table: "users");

        migrationBuilder.DropColumn(
            name: "last_name",
            schema: "public",
            table: "users");

        migrationBuilder.DropColumn(
            name: "password_hash",
            schema: "public",
            table: "users");

        migrationBuilder.AlterColumn<string>(
            name: "email",
            schema: "public",
            table: "users",
            type: "character varying(320)",
            maxLength: 320,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "text");

        migrationBuilder.AddColumn<DateTimeOffset>(
            name: "created_at",
            schema: "public",
            table: "users",
            type: "timestamp with time zone",
            nullable: false,
            defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

        migrationBuilder.AddColumn<Guid>(
            name: "created_by",
            schema: "public",
            table: "users",
            type: "uuid",
            nullable: false,
            defaultValue: Guid.Empty);

        migrationBuilder.AddColumn<string>(
            name: "full_name",
            schema: "public",
            table: "users",
            type: "character varying(200)",
            maxLength: 200,
            nullable: false,
            defaultValue: "");

        migrationBuilder.AddColumn<bool>(
            name: "is_active",
            schema: "public",
            table: "users",
            type: "boolean",
            nullable: false,
            defaultValue: false);

        migrationBuilder.AddColumn<string>(
            name: "keycloak_user_id",
            schema: "public",
            table: "users",
            type: "character varying(128)",
            maxLength: 128,
            nullable: false,
            defaultValue: "");

        migrationBuilder.AddColumn<DateTimeOffset>(
            name: "last_modified_at",
            schema: "public",
            table: "users",
            type: "timestamp with time zone",
            nullable: false,
            defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

        migrationBuilder.AddColumn<Guid>(
            name: "last_modified_by",
            schema: "public",
            table: "users",
            type: "uuid",
            nullable: false,
            defaultValue: Guid.Empty);

        migrationBuilder.AddColumn<string>(
            name: "phone_number",
            schema: "public",
            table: "users",
            type: "character varying(32)",
            maxLength: 32,
            nullable: true);

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
                last_modified_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                last_modified_by = table.Column<Guid>(type: "uuid", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_invitations", x => x.id);
            });

        migrationBuilder.CreateTable(
            name: "schools",
            schema: "public",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                owner_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                created_by = table.Column<Guid>(type: "uuid", nullable: false),
                last_modified_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                last_modified_by = table.Column<Guid>(type: "uuid", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_schools", x => x.id);
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

        migrationBuilder.CreateIndex(
            name: "ix_users_phone_number",
            schema: "public",
            table: "users",
            column: "phone_number",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "ix_invitations_school_id_target_type_target_value_role_status",
            schema: "public",
            table: "invitations",
            columns: new[] { "school_id", "target_type", "target_value", "role", "status" });

        migrationBuilder.CreateIndex(
            name: "ix_invitations_token_hash",
            schema: "public",
            table: "invitations",
            column: "token_hash",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "ix_user_school_memberships_user_id_school_id_role",
            schema: "public",
            table: "user_school_memberships",
            columns: new[] { "user_id", "school_id", "role" },
            unique: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "invitations",
            schema: "public");

        migrationBuilder.DropTable(
            name: "schools",
            schema: "public");

        migrationBuilder.DropTable(
            name: "user_school_memberships",
            schema: "public");

        migrationBuilder.DropIndex(
            name: "ix_users_phone_number",
            schema: "public",
            table: "users");

        migrationBuilder.DropColumn(
            name: "created_at",
            schema: "public",
            table: "users");

        migrationBuilder.DropColumn(
            name: "created_by",
            schema: "public",
            table: "users");

        migrationBuilder.DropColumn(
            name: "full_name",
            schema: "public",
            table: "users");

        migrationBuilder.DropColumn(
            name: "is_active",
            schema: "public",
            table: "users");

        migrationBuilder.DropColumn(
            name: "keycloak_user_id",
            schema: "public",
            table: "users");

        migrationBuilder.DropColumn(
            name: "last_modified_at",
            schema: "public",
            table: "users");

        migrationBuilder.DropColumn(
            name: "last_modified_by",
            schema: "public",
            table: "users");

        migrationBuilder.DropColumn(
            name: "phone_number",
            schema: "public",
            table: "users");

        migrationBuilder.AlterColumn<string>(
            name: "email",
            schema: "public",
            table: "users",
            type: "text",
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "character varying(320)",
            oldMaxLength: 320,
            oldNullable: true);

        migrationBuilder.AddColumn<string>(
            name: "first_name",
            schema: "public",
            table: "users",
            type: "text",
            nullable: false,
            defaultValue: "");

        migrationBuilder.AddColumn<string>(
            name: "last_name",
            schema: "public",
            table: "users",
            type: "text",
            nullable: false,
            defaultValue: "");

        migrationBuilder.AddColumn<string>(
            name: "password_hash",
            schema: "public",
            table: "users",
            type: "text",
            nullable: false,
            defaultValue: "");

        migrationBuilder.CreateTable(
            name: "todo_items",
            schema: "public",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                completed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                description = table.Column<string>(type: "text", nullable: false),
                due_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                is_completed = table.Column<bool>(type: "boolean", nullable: false),
                labels = table.Column<List<string>>(type: "text[]", nullable: false),
                priority = table.Column<int>(type: "integer", nullable: false),
                user_id = table.Column<Guid>(type: "uuid", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_todo_items", x => x.id);
                table.ForeignKey(
                    name: "fk_todo_items_users_user_id",
                    column: x => x.user_id,
                    principalSchema: "public",
                    principalTable: "users",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "ix_todo_items_user_id",
            schema: "public",
            table: "todo_items",
            column: "user_id");
    }
}

#pragma warning restore CA1861
