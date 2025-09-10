// <copyright file="PatientService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace PhysicallyFitPT.Infrastructure.Services;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PhysicallyFitPT.Domain;
using PhysicallyFitPT.Infrastructure.Data;
using PhysicallyFitPT.Infrastructure.Mappers;
using PhysicallyFitPT.Infrastructure.Services.Interfaces;
using PhysicallyFitPT.Shared;

/// <summary>
/// Service for managing patient operations including search, creation, and retrieval.
/// </summary>
public class PatientService : BaseService, IPatientService
{
  private readonly IDbContextFactory<ApplicationDbContext> dbFactory;

  /// <summary>
  /// Initializes a new instance of the <see cref="PatientService"/> class.
  /// </summary>
  /// <param name="dbFactory">Database context factory for data access.</param>
  /// <param name="logger">Logger instance for logging operations.</param>
  public PatientService(IDbContextFactory<ApplicationDbContext> dbFactory, ILogger<PatientService> logger)
      : base(logger)
  {
    this.dbFactory = dbFactory;
  }

  /// <inheritdoc/>
  public async Task<IEnumerable<PatientDto>> SearchAsync(string query, int take = 50, CancellationToken cancellationToken = default)
  {
    try
    {
      // Validate inputs
      if (take <= 0 || take > 1000)
      {
        throw new ArgumentException("Take parameter must be between 1 and 1000", nameof(take));
      }

      using var db = await this.dbFactory.CreateDbContextAsync();
      string q = (query ?? string.Empty).Trim().ToLower();

      // Prevent SQL injection by validating query length
      if (q.Length > 100)
      {
        throw new ArgumentException("Search query too long", nameof(query));
      }

      string like = $"%{q}%";
      var patients = await db.Patients.AsNoTracking()
          .Where(p => EF.Functions.Like((p.FirstName + " " + p.LastName).ToLower(), like))
          .OrderBy(p => p.LastName).ThenBy(p => p.FirstName)
          .Take(take).
          ToListAsync(cancellationToken);

      return patients.Select(p => p.ToDto()).ToList();
    }
    catch (Exception ex)
    {
      this.Logger.LogError(ex, "Error executing SearchAsync: {ErrorMessage}", ex.Message);
      return Enumerable.Empty<PatientDto>();
    }
  }
}
