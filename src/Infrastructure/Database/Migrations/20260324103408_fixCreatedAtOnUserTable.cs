using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Database.Migrations;

/// <inheritdoc />
public partial class fixCreatedAtOnUserTable : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.RenameColumn(
            name: "CreatedAt",
            schema: "public",
            table: "users",
            newName: "created_at");

        migrationBuilder.AlterColumn<DateTimeOffset>(
            name: "created_at",
            schema: "public",
            table: "users",
            type: "timestamp with time zone",
            nullable: false,
            defaultValueSql: "now()",
            oldClrType: typeof(DateTimeOffset),
            oldType: "timestamp with time zone");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.RenameColumn(
            name: "created_at",
            schema: "public",
            table: "users",
            newName: "CreatedAt");

        migrationBuilder.AlterColumn<DateTimeOffset>(
            name: "CreatedAt",
            schema: "public",
            table: "users",
            type: "timestamp with time zone",
            nullable: false,
            oldClrType: typeof(DateTimeOffset),
            oldType: "timestamp with time zone",
            oldDefaultValueSql: "now()");
    }
}
