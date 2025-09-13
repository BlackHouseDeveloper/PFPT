// <copyright file="Program.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using PhysicallyFitPT.Domain;
using PhysicallyFitPT.Infrastructure.Data;

var envPath = Environment.GetEnvironmentVariable("PFP_DB_PATH");
var dbPath = string.IsNullOrWhiteSpace(envPath)
    ? Path.Combine(Directory.GetCurrentDirectory(), "dev.physicallyfitpt.db")
    : envPath!;
Console.WriteLine($"[Seeder] Using DB: {dbPath}");

var options = new DbContextOptionsBuilder<ApplicationDbContext>()
    .UseSqlite($"Data Source={dbPath}")
    .Options;

await using var db = new ApplicationDbContext(options);
await db.Database.EnsureCreatedAsync();

if (!await db.CptCodes.AnyAsync())
{
    db.CptCodes.AddRange(
        new CptCode { Code = "97110", Description = "Therapeutic exercise" },
        new CptCode { Code = "97140", Description = "Manual therapy" },
        new CptCode { Code = "97530", Description = "Therapeutic activities" });
}

if (!await db.Icd10Codes.AnyAsync())
{
    db.Icd10Codes.AddRange(
        new Icd10Code { Code = "M25.561", Description = "Pain in right knee" },
        new Icd10Code { Code = "M25.562", Description = "Pain in left knee" });
}

if (!await db.Patients.AnyAsync())
{
    db.Patients.AddRange(
        new Patient { MRN = "A1001", FirstName = "Jane", LastName = "Doe", Email = "jane@example.com" },
        new Patient { MRN = "A1002", FirstName = "John", LastName = "Smith", Email = "john@example.com" });
}

await db.SaveChangesAsync();
Console.WriteLine("[Seeder] Seed complete.");
