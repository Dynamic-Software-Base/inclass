using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Database.Migrations;

/// <inheritdoc />
public partial class remove_school_indexers : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "ix_schools_owner_name_unique",
            schema: "public",
            table: "schools");
    }

    private static readonly string[] columns = new[] { "owner_user_id", "name" };

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateIndex(
            name: "ix_schools_owner_name_unique",
            schema: "public",
            table: "schools",
            columns: columns,
            unique: true);
    }
}
