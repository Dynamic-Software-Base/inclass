using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Database.Migrations;

/// <inheritdoc />
public partial class fixSnakeCaseForUser : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
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

        migrationBuilder.RenameIndex(
            name: "IX_users_Email",
            schema: "public",
            table: "users",
            newName: "IX_users_email");

        migrationBuilder.RenameIndex(
            name: "IX_users_PhoneNumber",
            schema: "public",
            table: "users",
            newName: "IX_users_phone_number");

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
            newName: "IX_user_school_memberships_user_id_school_id_role");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
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

        migrationBuilder.RenameIndex(
            name: "IX_users_email",
            schema: "public",
            table: "users",
            newName: "IX_users_Email");

        migrationBuilder.RenameIndex(
            name: "IX_users_phone_number",
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
            name: "IX_user_school_memberships_user_id_school_id_role",
            schema: "public",
            table: "user_school_memberships",
            newName: "IX_user_school_memberships_UserId_SchoolId_Role");
    }
}
