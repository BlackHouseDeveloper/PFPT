// <copyright file="DataStoreTests.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace PhysicallyFitPT.Tests;

using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using PhysicallyFitPT.Domain;
using PhysicallyFitPT.Infrastructure.Data;
using PhysicallyFitPT.Infrastructure.Services;

/// <summary>
/// Tests for data store implementations to verify browser vs mobile compatibility.
/// </summary>
public class DataStoreTests
{
  /// <summary>
  /// Tests that BrowserDataStore can be initialized and perform basic operations.
  /// </summary>
  /// <returns><placeholder>A <see cref="Task"/> representing the asynchronous unit test.</placeholder></returns>
  [Fact]
  public async Task BrowserDataStore_InitializeAndCreatePatient_ShouldWork()
  {
    // Arrange
    var options = new DbContextOptionsBuilder<ApplicationDbContext>()
      .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
      .Options;

    var factory = new TestDbContextFactory(options);
    var dataStore = new BrowserDataStore(factory);

    // Act
    await dataStore.InitializeAsync();

    var patient = new Patient
    {
      FirstName = "John",
      LastName = "Doe",
      Email = "john.doe@example.com",
      DateOfBirth = DateTime.Parse("1990-01-01"),
    };

    var createdPatient = await dataStore.CreatePatientAsync(patient);
    var retrievedPatient = await dataStore.GetPatientByIdAsync(createdPatient.Id);

    // Assert
    dataStore.StorageBackend.Should().Be("In-Memory (Browser)");
    createdPatient.Should().NotBeNull();
    createdPatient.Id.Should().NotBe(Guid.Empty);
    retrievedPatient.Should().NotBeNull();
    retrievedPatient!.FirstName.Should().Be("John");
    retrievedPatient.LastName.Should().Be("Doe");
  }

  /// <summary>
  /// Tests that SqliteDataStore can be initialized and perform basic operations.
  /// </summary>
  /// <returns><placeholder>A <see cref="Task"/> representing the asynchronous unit test.</placeholder></returns>
  [Fact]
  public async Task SqliteDataStore_InitializeAndCreatePatient_ShouldWork()
  {
    // Arrange
    var options = new DbContextOptionsBuilder<ApplicationDbContext>()
      .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
      .Options;

    var factory = new TestDbContextFactory(options);
    var dataStore = new SqliteDataStore(factory);

    // Act
    await dataStore.InitializeAsync();

    var patient = new Patient
    {
      FirstName = "Jane",
      LastName = "Smith",
      Email = "jane.smith@example.com",
      DateOfBirth = DateTime.Parse("1985-05-15"),
    };

    var createdPatient = await dataStore.CreatePatientAsync(patient);
    var retrievedPatient = await dataStore.GetPatientByIdAsync(createdPatient.Id);

    // Assert
    dataStore.StorageBackend.Should().Be("SQLite");
    createdPatient.Should().NotBeNull();
    createdPatient.Id.Should().NotBe(Guid.Empty);
    retrievedPatient.Should().NotBeNull();
    retrievedPatient!.FirstName.Should().Be("Jane");
    retrievedPatient.LastName.Should().Be("Smith");
  }

  /// <summary>
  /// Tests that BrowserDataStore seeds reference data during initialization.
  /// </summary>
  /// <returns><placeholder>A <see cref="Task"/> representing the asynchronous unit test.</placeholder></returns>
  [Fact]
  public async Task BrowserDataStore_Initialize_ShouldSeedReferenceData()
  {
    // Arrange
    var options = new DbContextOptionsBuilder<ApplicationDbContext>()
      .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
      .Options;

    var factory = new TestDbContextFactory(options);
    var dataStore = new BrowserDataStore(factory);

    // Act
    await dataStore.InitializeAsync();
    var cptCodes = await dataStore.GetCptCodesAsync();
    var icd10Codes = await dataStore.GetIcd10CodesAsync();

    // Assert
    cptCodes.Should().NotBeEmpty();
    cptCodes.Should().Contain(c => c.Code == "97110");
    icd10Codes.Should().NotBeEmpty();
    icd10Codes.Should().Contain(c => c.Code == "M25.50");
  }
}
