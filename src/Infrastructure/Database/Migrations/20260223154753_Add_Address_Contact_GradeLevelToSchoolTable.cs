using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Database.Migrations;

/// <inheritdoc />
public partial class Add_Address_Contact_GradeLevelToSchoolTable : Migration
{
    private static readonly string[] columns = new[] { "owner_user_id", "name" };

    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<DateTimeOffset>(
            name: "last_modified_at",
            schema: "public",
            table: "users",
            type: "timestamp with time zone",
            nullable: true,
            oldClrType: typeof(DateTimeOffset),
            oldType: "timestamp with time zone");

        migrationBuilder.AlterColumn<DateTimeOffset>(
            name: "last_modified_at",
            schema: "public",
            table: "schools",
            type: "timestamp with time zone",
            nullable: true,
            oldClrType: typeof(DateTimeOffset),
            oldType: "timestamp with time zone");

        migrationBuilder.AddColumn<string>(
            name: "ar_name",
            schema: "public",
            table: "schools",
            type: "character varying(200)",
            maxLength: 200,
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "description",
            schema: "public",
            table: "schools",
            type: "character varying(1000)",
            maxLength: 1000,
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "email",
            schema: "public",
            table: "schools",
            type: "character varying(200)",
            maxLength: 200,
            nullable: false,
            defaultValue: "");

        migrationBuilder.AddColumn<int>(
            name: "grade_levels",
            schema: "public",
            table: "schools",
            type: "integer",
            nullable: false,
            defaultValue: 0);

        migrationBuilder.AddColumn<string>(
            name: "primary_phone",
            schema: "public",
            table: "schools",
            type: "character varying(20)",
            maxLength: 20,
            nullable: false,
            defaultValue: "");

        migrationBuilder.AddColumn<string>(
            name: "secondary_phone",
            schema: "public",
            table: "schools",
            type: "character varying(20)",
            maxLength: 20,
            nullable: true);

        migrationBuilder.AlterColumn<DateTimeOffset>(
            name: "last_modified_at",
            schema: "public",
            table: "invitations",
            type: "timestamp with time zone",
            nullable: true,
            oldClrType: typeof(DateTimeOffset),
            oldType: "timestamp with time zone");

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

        migrationBuilder.CreateIndex(
            name: "ix_schools_name",
            schema: "public",
            table: "schools",
            column: "name");

        migrationBuilder.CreateIndex(
            name: "ix_schools_owner_name_unique",
            schema: "public",
            table: "schools",
            columns: columns,
            unique: true);

        migrationBuilder.CreateIndex(
            name: "ix_schools_owner_user_id",
            schema: "public",
            table: "schools",
            column: "owner_user_id");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "school_addresses",
            schema: "public");

        migrationBuilder.DropIndex(
            name: "ix_schools_name",
            schema: "public",
            table: "schools");

        migrationBuilder.DropIndex(
            name: "ix_schools_owner_name_unique",
            schema: "public",
            table: "schools");

        migrationBuilder.DropIndex(
            name: "ix_schools_owner_user_id",
            schema: "public",
            table: "schools");

        migrationBuilder.DropColumn(
            name: "ar_name",
            schema: "public",
            table: "schools");

        migrationBuilder.DropColumn(
            name: "description",
            schema: "public",
            table: "schools");

        migrationBuilder.DropColumn(
            name: "email",
            schema: "public",
            table: "schools");

        migrationBuilder.DropColumn(
            name: "grade_levels",
            schema: "public",
            table: "schools");

        migrationBuilder.DropColumn(
            name: "primary_phone",
            schema: "public",
            table: "schools");

        migrationBuilder.DropColumn(
            name: "secondary_phone",
            schema: "public",
            table: "schools");

        migrationBuilder.AlterColumn<DateTimeOffset>(
            name: "last_modified_at",
            schema: "public",
            table: "users",
            type: "timestamp with time zone",
            nullable: false,
            defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)),
            oldClrType: typeof(DateTimeOffset),
            oldType: "timestamp with time zone",
            oldNullable: true);

        migrationBuilder.AlterColumn<DateTimeOffset>(
            name: "last_modified_at",
            schema: "public",
            table: "schools",
            type: "timestamp with time zone",
            nullable: false,
            defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)),
            oldClrType: typeof(DateTimeOffset),
            oldType: "timestamp with time zone",
            oldNullable: true);

        migrationBuilder.AlterColumn<DateTimeOffset>(
            name: "last_modified_at",
            schema: "public",
            table: "invitations",
            type: "timestamp with time zone",
            nullable: false,
            defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)),
            oldClrType: typeof(DateTimeOffset),
            oldType: "timestamp with time zone",
            oldNullable: true);
    }
}
