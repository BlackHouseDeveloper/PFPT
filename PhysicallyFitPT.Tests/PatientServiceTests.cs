// <copyright file="PatientServiceTests.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace PhysicallyFitPT.Tests;

using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using PhysicallyFitPT.Domain;
using PhysicallyFitPT.Infrastructure.Data;
using PhysicallyFitPT.Infrastructure.Services;
using PhysicallyFitPT.Infrastructure.Services.Interfaces;
using PhysicallyFitPT.Shared;
using Xunit;

public class PatientServiceTests
{
  [Fact]
  public async Task SearchAsync_Returns_Empty_On_Empty_DB()
  {
    var options = new DbContextOptionsBuilder<ApplicationDbContext>()
        .UseInMemoryDatabase(databaseName: System.Guid.NewGuid().ToString())
        .Options;

    var factory = new TestDbContextFactory(options);
    var mockLogger = new Microsoft.Extensions.Logging.Abstractions.NullLogger<PatientService>();
    IPatientService svc = new PatientService(factory, mockLogger);

    var results = await svc.SearchAsync("john", 10);
    results.Should().BeEmpty();
  }

  [Fact]
  public async Task CreateAsync_ValidPatient_ReturnsPatientDto()
  {
      var options = new DbContextOptionsBuilder<ApplicationDbContext>()
          .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
          .Options;

      var factory = new TestDbContextFactory(options);
      var mockLogger = new NullLogger<PatientService>();
      IPatientService svc = new PatientService(factory, mockLogger);

      var patientDto = new PatientDto
      {
          FirstName = "John",
          LastName = "Doe",
          Email = "john.doe@example.com",
          MobilePhone = "555-1234"
      };

      var result = await svc.CreateAsync(patientDto);

      result.Should().NotBeNull();
      result.Id.Should().NotBeEmpty();
      result.FirstName.Should().Be("John");
      result.LastName.Should().Be("Doe");
      result.Email.Should().Be("john.doe@example.com");
  }

  [Fact]
  public async Task CreateAsync_MissingFirstName_ThrowsArgumentException()
  {
      var options = new DbContextOptionsBuilder<ApplicationDbContext>()
          .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
          .Options;

      var factory = new TestDbContextFactory(options);
      var mockLogger = new NullLogger<PatientService>();
      IPatientService svc = new PatientService(factory, mockLogger);

      var patientDto = new PatientDto
      {
          LastName = "Doe",
          Email = "john.doe@example.com"
      };

      await svc.Invoking(s => s.CreateAsync(patientDto))
          .Should().ThrowAsync<ArgumentException>()
          .WithMessage("*FirstName is required*");
  }

  [Fact]
  public async Task GetByIdAsync_ExistingPatient_ReturnsPatient()
  {
      var options = new DbContextOptionsBuilder<ApplicationDbContext>()
          .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
          .Options;

      var factory = new TestDbContextFactory(options);
      var mockLogger = new NullLogger<PatientService>();
      IPatientService svc = new PatientService(factory, mockLogger);

      // Create a patient first
      var patientDto = new PatientDto
      {
          FirstName = "Jane",
          LastName = "Smith",
          Email = "jane.smith@example.com"
      };

      var created = await svc.CreateAsync(patientDto);
      
      // Get by ID
      var result = await svc.GetByIdAsync(created.Id);

      result.Should().NotBeNull();
      result!.FirstName.Should().Be("Jane");
      result.LastName.Should().Be("Smith");
  }

  [Fact]
  public async Task SearchAsync_ValidQuery_ReturnsMatchingPatients()
  {
      var options = new DbContextOptionsBuilder<ApplicationDbContext>()
          .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
          .Options;

      var factory = new TestDbContextFactory(options);
      var mockLogger = new NullLogger<PatientService>();
      IPatientService svc = new PatientService(factory, mockLogger);

      // Create test patients
      await svc.CreateAsync(new PatientDto { FirstName = "John", LastName = "Doe" });
      await svc.CreateAsync(new PatientDto { FirstName = "Jane", LastName = "Doe" });
      await svc.CreateAsync(new PatientDto { FirstName = "Bob", LastName = "Smith" });

      var results = await svc.SearchAsync("Doe");

      results.Should().HaveCount(2);
      results.Should().OnlyContain(p => p.LastName == "Doe");
  }

  [Fact]
  public async Task SoftDeleteAsync_ExistingPatient_ReturnsTrue()
  {
      var options = new DbContextOptionsBuilder<ApplicationDbContext>()
          .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
          .Options;

      var factory = new TestDbContextFactory(options);
      var mockLogger = new NullLogger<PatientService>();
      IPatientService svc = new PatientService(factory, mockLogger);

      // Create a patient first
      var created = await svc.CreateAsync(new PatientDto { FirstName = "Test", LastName = "Patient" });
      
      // Soft delete
      var result = await svc.SoftDeleteAsync(created.Id);

      result.Should().BeTrue();
      
      // Verify patient is no longer returned by GetById (due to soft-delete filter)
      var deletedPatient = await svc.GetByIdAsync(created.Id);
      deletedPatient.Should().BeNull();
  }

  [Fact]
  public async Task UpdateAsync_ExistingPatient_ReturnsUpdatedPatient()
  {
      var options = new DbContextOptionsBuilder<ApplicationDbContext>()
          .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
          .Options;

      var factory = new TestDbContextFactory(options);
      var mockLogger = new NullLogger<PatientService>();
      IPatientService svc = new PatientService(factory, mockLogger);

      // Create a patient first
      var created = await svc.CreateAsync(new PatientDto { FirstName = "Original", LastName = "Name" });
      
      // Update the patient
      var updateDto = new PatientDto
      {
          FirstName = "Updated",
          LastName = "Name",
          Email = "updated@example.com"
      };

      var result = await svc.UpdateAsync(created.Id, updateDto);

      result.Should().NotBeNull();
      result!.FirstName.Should().Be("Updated");
      result.Email.Should().Be("updated@example.com");
  }
}
