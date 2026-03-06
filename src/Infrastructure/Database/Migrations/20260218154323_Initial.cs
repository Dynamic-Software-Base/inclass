using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Database.Migrations;

/// <inheritdoc />
public partial class Initial : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.EnsureSchema(
            name: "public");

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

        migrationBuilder.CreateTable(
            name: "users",
            schema: "public",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                keycloak_user_id = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                email = table.Column<string>(type: "character varying(320)", maxLength: 320, nullable: true),
                phone_number = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                full_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                is_active = table.Column<bool>(type: "boolean", nullable: false),
                created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                created_by = table.Column<Guid>(type: "uuid", nullable: false),
                last_modified_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                last_modified_by = table.Column<Guid>(type: "uuid", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_users", x => x.id);
            });

        migrationBuilder.CreateIndex(
            name: "ix_invitations_school_id_target_type_target_value_role_status",
            schema: "public",
            table: "invitations",
#pragma warning disable CA1861
            columns: new[] { "school_id", "target_type", "target_value", "role", "status" });
#pragma warning restore CA1861

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
#pragma warning disable CA1861
            columns: new[] { "user_id", "school_id", "role" },
#pragma warning restore CA1861
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
            name: "invitations",
            schema: "public");

        migrationBuilder.DropTable(
            name: "schools",
            schema: "public");

        migrationBuilder.DropTable(
            name: "user_school_memberships",
            schema: "public");

        migrationBuilder.DropTable(
            name: "users",
            schema: "public");
    }
}
