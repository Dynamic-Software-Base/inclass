using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Database.Migrations;

/// <inheritdoc />
public partial class registrationSessionModifcation : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "gender",
            schema: "public",
            table: "school_classes");

        migrationBuilder.AddColumn<string>(
            name: "IdentityKey",
            schema: "public",
            table: "students",
            type: "text",
            nullable: false,
            defaultValue: "");

        migrationBuilder.AlterColumn<Guid>(
            name: "student_id",
            schema: "public",
            table: "student_applications",
            type: "uuid",
            nullable: true,
            oldClrType: typeof(Guid),
            oldType: "uuid");

        migrationBuilder.AddColumn<string>(
            name: "IdentityKey",
            schema: "public",
            table: "student_applications",
            type: "text",
            nullable: false,
            defaultValue: "");

        migrationBuilder.AddColumn<string>(
            name: "StudentFirstName",
            schema: "public",
            table: "student_applications",
            type: "text",
            nullable: false,
            defaultValue: "");

        migrationBuilder.AddColumn<string>(
            name: "StudentLastName",
            schema: "public",
            table: "student_applications",
            type: "text",
            nullable: false,
            defaultValue: "");

        migrationBuilder.AddColumn<int>(
            name: "WaitlistCount",
            schema: "public",
            table: "registration_sessions",
            type: "integer",
            nullable: false,
            defaultValue: 0);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "IdentityKey",
            schema: "public",
            table: "students");

        migrationBuilder.DropColumn(
            name: "IdentityKey",
            schema: "public",
            table: "student_applications");

        migrationBuilder.DropColumn(
            name: "StudentFirstName",
            schema: "public",
            table: "student_applications");

        migrationBuilder.DropColumn(
            name: "StudentLastName",
            schema: "public",
            table: "student_applications");

        migrationBuilder.DropColumn(
            name: "WaitlistCount",
            schema: "public",
            table: "registration_sessions");

        migrationBuilder.AlterColumn<Guid>(
            name: "student_id",
            schema: "public",
            table: "student_applications",
            type: "uuid",
            nullable: false,
            defaultValue: Guid.Empty,
            oldClrType: typeof(Guid),
            oldType: "uuid",
            oldNullable: true);

        migrationBuilder.AddColumn<string>(
            name: "gender",
            schema: "public",
            table: "school_classes",
            type: "character varying(20)",
            maxLength: 20,
            nullable: false,
            defaultValue: "");
    }
}
