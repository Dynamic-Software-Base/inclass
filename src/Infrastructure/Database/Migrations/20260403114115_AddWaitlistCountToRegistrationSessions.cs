using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Database.Migrations;

/// <inheritdoc />
public partial class AddWaitlistCountToRegistrationSessions : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.RenameColumn(
            name: "WaitlistCount",
            schema: "public",
            table: "registration_sessions",
            newName: "waitlist_count");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.RenameColumn(
            name: "waitlist_count",
            schema: "public",
            table: "registration_sessions",
            newName: "WaitlistCount");
    }
}
