using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Database.Migrations;

/// <inheritdoc />
public partial class lastModifiesAt : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.RenameColumn(
            name: "LastModifiedAt",
            schema: "public",
            table: "users",
            newName: "last_modified_at");

        migrationBuilder.AlterColumn<DateTimeOffset>(
            name: "last_modified_at",
            schema: "public",
            table: "users",
            type: "timestamp with time zone",
            nullable: true,
            defaultValueSql: "now()",
            oldClrType: typeof(DateTimeOffset),
            oldType: "timestamp with time zone",
            oldNullable: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.RenameColumn(
            name: "last_modified_at",
            schema: "public",
            table: "users",
            newName: "LastModifiedAt");

        migrationBuilder.AlterColumn<DateTimeOffset>(
            name: "LastModifiedAt",
            schema: "public",
            table: "users",
            type: "timestamp with time zone",
            nullable: true,
            oldClrType: typeof(DateTimeOffset),
            oldType: "timestamp with time zone",
            oldNullable: true,
            oldDefaultValueSql: "now()");
    }
}
