using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PhysicallyFitPT.Infrastructure.Migrations
{
  /// <inheritdoc />
  public partial class AddSeedingInfrastructure : Migration
  {
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.CreateTable(
          name: "ReferenceSummaries",
          columns: table => new
          {
            Id = table.Column<int>(type: "INTEGER", nullable: false)
                  .Annotation("Sqlite:Autoincrement", true),
            CptCount = table.Column<int>(type: "INTEGER", nullable: false),
            Icd10Count = table.Column<int>(type: "INTEGER", nullable: false),
            UpdatedAtUtc = table.Column<DateTimeOffset>(type: "TEXT", nullable: false)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_ReferenceSummaries", x => x.Id);
          });

      migrationBuilder.CreateTable(
          name: "SeederLocks",
          columns: table => new
          {
            Id = table.Column<int>(type: "INTEGER", nullable: false)
                  .Annotation("Sqlite:Autoincrement", true),
            AcquiredAtUtc = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
            ProcessInfo = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_SeederLocks", x => x.Id);
          });

      migrationBuilder.CreateTable(
          name: "SeedHistory",
          columns: table => new
          {
            TaskId = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
            Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
            Hash = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false),
            AppliedAtUtc = table.Column<DateTimeOffset>(type: "TEXT", nullable: false)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_SeedHistory", x => x.TaskId);
          });

      migrationBuilder.CreateIndex(
          name: "IX_SeedHistory_AppliedAtUtc",
          table: "SeedHistory",
          column: "AppliedAtUtc");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.DropTable(
          name: "ReferenceSummaries");

      migrationBuilder.DropTable(
          name: "SeederLocks");

      migrationBuilder.DropTable(
          name: "SeedHistory");
    }
  }
}
