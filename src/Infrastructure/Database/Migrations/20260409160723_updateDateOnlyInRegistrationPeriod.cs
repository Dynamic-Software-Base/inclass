using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Database.Migrations;

/// <inheritdoc />
public partial class updateDateOnlyInRegistrationPeriod : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<DateOnly>(
            name: "period_open_date",
            schema: "public",
            table: "registration_sessions",
            type: "date",
            nullable: false,
            oldClrType: typeof(DateTime),
            oldType: "timestamp with time zone");

        migrationBuilder.AlterColumn<DateOnly>(
            name: "period_close_date",
            schema: "public",
            table: "registration_sessions",
            type: "date",
            nullable: true,
            oldClrType: typeof(DateTime),
            oldType: "timestamp with time zone",
            oldNullable: true);

        migrationBuilder.AlterColumn<DateOnly>(
            name: "start_date",
            schema: "public",
            table: "registration_session_phases",
            type: "date",
            nullable: false,
            oldClrType: typeof(DateTime),
            oldType: "timestamp with time zone");

        migrationBuilder.AlterColumn<DateOnly>(
            name: "end_date",
            schema: "public",
            table: "registration_session_phases",
            type: "date",
            nullable: false,
            oldClrType: typeof(DateTime),
            oldType: "timestamp with time zone");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<DateTime>(
            name: "period_open_date",
            schema: "public",
            table: "registration_sessions",
            type: "timestamp with time zone",
            nullable: false,
            oldClrType: typeof(DateOnly),
            oldType: "date");

        migrationBuilder.AlterColumn<DateTime>(
            name: "period_close_date",
            schema: "public",
            table: "registration_sessions",
            type: "timestamp with time zone",
            nullable: true,
            oldClrType: typeof(DateOnly),
            oldType: "date",
            oldNullable: true);

        migrationBuilder.AlterColumn<DateTime>(
            name: "start_date",
            schema: "public",
            table: "registration_session_phases",
            type: "timestamp with time zone",
            nullable: false,
            oldClrType: typeof(DateOnly),
            oldType: "date");

        migrationBuilder.AlterColumn<DateTime>(
            name: "end_date",
            schema: "public",
            table: "registration_session_phases",
            type: "timestamp with time zone",
            nullable: false,
            oldClrType: typeof(DateOnly),
            oldType: "date");
    }
}
