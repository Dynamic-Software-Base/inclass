using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Infrastructure.Database.Migrations;

/// <inheritdoc />
public partial class AddPicturesSupport : Migration
{
    private static readonly string[] columns = new[] { "school_id", "stored_file_id" };

    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<DateTimeOffset>(
            name: "created_at",
            schema: "public",
            table: "schools",
            type: "timestamp with time zone",
            nullable: false,
            defaultValueSql: "now()",
            oldClrType: typeof(DateTimeOffset),
            oldType: "timestamp with time zone");

        migrationBuilder.CreateTable(
            name: "school_pictures",
            schema: "public",
            columns: table => new
            {
                id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                stored_file_id = table.Column<Guid>(type: "uuid", nullable: false),
                alt_text = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                is_main = table.Column<bool>(type: "boolean", nullable: false),
                school_id = table.Column<Guid>(type: "uuid", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_school_pictures", x => x.id);
                table.ForeignKey(
                    name: "fk_school_pictures_schools_school_id",
                    column: x => x.school_id,
                    principalSchema: "public",
                    principalTable: "schools",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "stored_files",
            schema: "public",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                owner_id = table.Column<Guid>(type: "uuid", nullable: false),
                original_file_name = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                stored_file_name = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                content_type = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                size_in_bytes = table.Column<long>(type: "bigint", nullable: false),
                url = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                storage_provider = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                blob_path = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                created_by = table.Column<Guid>(type: "uuid", nullable: false),
                last_modified_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                last_modified_by = table.Column<Guid>(type: "uuid", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_stored_files", x => x.id);
            });

        migrationBuilder.CreateIndex(
            name: "ix_school_pictures_school_id_stored_file_id",
            schema: "public",
            table: "school_pictures",
            columns: columns,
            unique: true);

        migrationBuilder.CreateIndex(
            name: "ix_stored_files_owner_id",
            schema: "public",
            table: "stored_files",
            column: "owner_id");

        migrationBuilder.CreateIndex(
            name: "ix_stored_files_stored_file_name",
            schema: "public",
            table: "stored_files",
            column: "stored_file_name",
            unique: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "school_pictures",
            schema: "public");

        migrationBuilder.DropTable(
            name: "stored_files",
            schema: "public");

        migrationBuilder.AlterColumn<DateTimeOffset>(
            name: "created_at",
            schema: "public",
            table: "schools",
            type: "timestamp with time zone",
            nullable: false,
            oldClrType: typeof(DateTimeOffset),
            oldType: "timestamp with time zone",
            oldDefaultValueSql: "now()");
    }
}
