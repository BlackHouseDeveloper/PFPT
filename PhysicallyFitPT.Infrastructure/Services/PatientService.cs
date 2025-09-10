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

public class PatientService : BaseService, IPatientService
{
  private readonly IDbContextFactory<ApplicationDbContext> dbFactory;

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

  /// <inheritdoc/>
  public async Task<PatientDto> CreateAsync(PatientDto patientDto, CancellationToken cancellationToken = default)
  {
    try
    {
      // Validate required fields
      if (string.IsNullOrWhiteSpace(patientDto.FirstName))
      {
        throw new ArgumentException("FirstName is required", nameof(patientDto));
      }

      if (string.IsNullOrWhiteSpace(patientDto.LastName))
      {
        throw new ArgumentException("LastName is required", nameof(patientDto));
      }

      using var db = await this.dbFactory.CreateDbContextAsync();
      var patient = patientDto.FromDto();
      patient.Id = Guid.NewGuid(); // Ensure new ID
      
      db.Patients.Add(patient);
      await db.SaveChangesAsync(cancellationToken);

      return patient.ToDto();
    }
    catch (Exception ex)
    {
      this.Logger.LogError(ex, "Error executing CreateAsync: {ErrorMessage}", ex.Message);
      throw;
    }
  }

  /// <inheritdoc/>
  public async Task<PatientDto?> GetByIdAsync(Guid patientId, CancellationToken cancellationToken = default)
  {
    try
    {
      if (patientId == Guid.Empty)
      {
        throw new ArgumentException("Patient ID cannot be empty", nameof(patientId));
      }

      using var db = await this.dbFactory.CreateDbContextAsync();
      var patient = await db.Patients.AsNoTracking()
          .FirstOrDefaultAsync(p => p.Id == patientId, cancellationToken);

      return patient?.ToDto();
    }
    catch (Exception ex)
    {
      this.Logger.LogError(ex, "Error executing GetByIdAsync: {ErrorMessage}", ex.Message);
      throw;
    }
  }

  /// <inheritdoc/>
  public async Task<PatientDto?> UpdateAsync(Guid patientId, PatientDto patientDto, CancellationToken cancellationToken = default)
  {
    try
    {
      if (patientId == Guid.Empty)
      {
        throw new ArgumentException("Patient ID cannot be empty", nameof(patientId));
      }

      // Validate required fields
      if (string.IsNullOrWhiteSpace(patientDto.FirstName))
      {
        throw new ArgumentException("FirstName is required", nameof(patientDto));
      }

      if (string.IsNullOrWhiteSpace(patientDto.LastName))
      {
        throw new ArgumentException("LastName is required", nameof(patientDto));
      }

      using var db = await this.dbFactory.CreateDbContextAsync();
      var patient = await db.Patients.FirstOrDefaultAsync(p => p.Id == patientId, cancellationToken);
      
      if (patient == null)
      {
        return null;
      }

      // Update properties
      patient.MRN = patientDto.MRN;
      patient.FirstName = patientDto.FirstName;
      patient.LastName = patientDto.LastName;
      patient.DateOfBirth = patientDto.DateOfBirth;
      patient.Sex = patientDto.Sex;
      patient.Email = patientDto.Email;
      patient.MobilePhone = patientDto.MobilePhone;
      patient.MedicationsCsv = patientDto.MedicationsCsv;
      patient.ComorbiditiesCsv = patientDto.ComorbiditiesCsv;
      patient.AssistiveDevicesCsv = patientDto.AssistiveDevicesCsv;
      patient.LivingSituation = patientDto.LivingSituation;

      await db.SaveChangesAsync(cancellationToken);
      return patient.ToDto();
    }
    catch (Exception ex)
    {
      this.Logger.LogError(ex, "Error executing UpdateAsync: {ErrorMessage}", ex.Message);
      throw;
    }
  }

  /// <inheritdoc/>
  public async Task<bool> SoftDeleteAsync(Guid patientId, CancellationToken cancellationToken = default)
  {
    try
    {
      if (patientId == Guid.Empty)
      {
        throw new ArgumentException("Patient ID cannot be empty", nameof(patientId));
      }

      using var db = await this.dbFactory.CreateDbContextAsync();
      var patient = await db.Patients.FirstOrDefaultAsync(p => p.Id == patientId, cancellationToken);
      
      if (patient == null)
      {
        return false;
      }

      patient.IsDeleted = true;
      await db.SaveChangesAsync(cancellationToken);
      return true;
    }
    catch (Exception ex)
    {
      this.Logger.LogError(ex, "Error executing SoftDeleteAsync: {ErrorMessage}", ex.Message);
      throw;
    }
  }
}
