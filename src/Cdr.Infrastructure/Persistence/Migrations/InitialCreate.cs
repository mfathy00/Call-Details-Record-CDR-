using Microsoft.EntityFrameworkCore.Migrations;

namespace Cdr.Infrastructure.Persistence.Migrations;

public partial class InitialCreate : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        // Create the __EFMigrationsHistory table first
        migrationBuilder.CreateTable(
            name: "__EFMigrationsHistory",
            columns: table => new
            {
                MigrationId = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                ProductVersion = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK___EFMigrationsHistory", x => x.MigrationId);
            });

        // Create the cdr_records table
        migrationBuilder.CreateTable(
            name: "cdr_records",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                call_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                caller_id = table.Column<string>(type: "text", nullable: false),
                recipient = table.Column<string>(type: "text", nullable: false),
                duration_seconds = table.Column<int>(type: "integer", nullable: false),
                cost = table.Column<decimal>(type: "numeric", nullable: false),
                currency = table.Column<string>(type: "text", nullable: false),
                reference = table.Column<string>(type: "text", nullable: false),
                created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_cdr_records", x => x.id);
            });

        // Create indexes
        migrationBuilder.CreateIndex(
            name: "ix_cdr_records_call_date_caller_id",
            table: "cdr_records",
            columns: new[] { "call_date", "caller_id" });

        migrationBuilder.CreateIndex(
            name: "ix_cdr_records_recipient",
            table: "cdr_records",
            column: "recipient");

        migrationBuilder.CreateIndex(
            name: "ix_cdr_records_reference",
            table: "cdr_records",
            column: "reference");

        // Insert the migration record
        migrationBuilder.InsertData(
            table: "__EFMigrationsHistory",
            columns: new[] { "MigrationId", "ProductVersion" },
            values: new object[] { "InitialCreate", "8.0.4" });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "cdr_records");

        migrationBuilder.DropTable(
            name: "__EFMigrationsHistory");
    }
}
