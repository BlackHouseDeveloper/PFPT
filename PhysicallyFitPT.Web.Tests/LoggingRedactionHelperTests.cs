using PhysicallyFitPT.Web.Services;
using Xunit;
using FluentAssertions;

namespace PhysicallyFitPT.Web.Tests;

public class LoggingRedactionHelperTests
{
    [Fact]
    public void RedactSensitiveInfo_ShouldRedactPatientFirstName()
    {
        // Arrange
        var logMessage = "Processing patient firstName: John with id 123";

        // Act
        var result = LoggingRedactionHelper.RedactSensitiveInfo(logMessage);

        // Assert
        result.Should().Contain("firstName: [REDACTED]");
        result.Should().NotContain("John");
    }

    [Fact]
    public void RedactSensitiveInfo_ShouldRedactNoteContent()
    {
        // Arrange
        var logMessage = "Saving note=Patient has severe pain in lower back";

        // Act
        var result = LoggingRedactionHelper.RedactSensitiveInfo(logMessage);

        // Assert
        result.Should().Contain("note: [REDACTED]");
        result.Should().NotContain("severe pain in lower back");
    }

    [Fact]
    public void RedactSensitiveInfo_ShouldHandleMultipleSensitiveFields()
    {
        // Arrange
        var logMessage = "Patient firstName: John, lastName: Doe, diagnosis: Back pain";

        // Act
        var result = LoggingRedactionHelper.RedactSensitiveInfo(logMessage);

        // Assert
        result.Should().Contain("firstName: [REDACTED]");
        result.Should().Contain("lastName: [REDACTED]");  
        result.Should().Contain("diagnosis: [REDACTED]");
        result.Should().NotContain("John");
        result.Should().NotContain("Doe");
        result.Should().NotContain("Back pain");
    }

    [Fact]
    public void RedactSensitiveInfo_ShouldPreserveNonSensitiveInformation()
    {
        // Arrange
        var logMessage = "Processing patientId: 12345 at timestamp: 2024-01-01";

        // Act
        var result = LoggingRedactionHelper.RedactSensitiveInfo(logMessage);

        // Assert
        result.Should().Be(logMessage); // Should remain unchanged
    }

    [Fact]
    public void CreateSafeLogEntry_ShouldCreateHIPAASafeLog()
    {
        // Arrange
        var action = "CreateNote";
        var entityId = Guid.NewGuid();
        var userId = "admin@example.com";

        // Act
        var result = LoggingRedactionHelper.CreateSafeLogEntry(action, entityId, userId);

        // Assert
        result.Should().Contain($"Action: {action}");
        result.Should().Contain($"EntityId: {entityId}");
        result.Should().Contain($"UserId: {userId}");
        result.Should().Contain("Timestamp:");
    }
}