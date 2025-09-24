// <copyright file="SeedTaskRegistry.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using Microsoft.Extensions.DependencyInjection;
using PhysicallyFitPT.Seeder.Seeding.Hashing;
using PhysicallyFitPT.Seeder.Seeding.Tasks;
using PhysicallyFitPT.Seeder.Utils;

namespace PhysicallyFitPT.Seeder.Seeding;

/// <summary>
/// Extension methods for registering seeding services.
/// </summary>
public static class SeedTaskRegistry
{
  /// <summary>
  /// Adds seeding services to the dependency injection container.
  /// </summary>
  /// <param name="services">Service collection.</param>
  /// <returns>Service collection for chaining.</returns>
  public static IServiceCollection AddSeedingServices(this IServiceCollection services)
  {
    // Core seeding infrastructure
    services.AddSingleton<SeedHashCalculator>();
    services.AddSingleton<JsonDataLoader>();
    services.AddScoped<SeedRunner>();

    // Register all seed tasks
    services.AddScoped<ISeedTask, CptCodeSeedTask>();
    services.AddScoped<ISeedTask, Icd10CodeSeedTask>();
    services.AddScoped<ISeedTask, PatientSeedTask>();
    services.AddScoped<ISeedTask, CompositeClinicalReferenceSeedTask>();

    return services;
  }
}