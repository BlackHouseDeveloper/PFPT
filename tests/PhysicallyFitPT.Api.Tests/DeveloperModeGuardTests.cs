using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PhysicallyFitPT.Shared.Diagnostics;
using Xunit;

namespace PhysicallyFitPT.Api.Tests;

public class DeveloperModeGuardTests
{
  private const string EnvKey = DeveloperModeGuard.EnvironmentVariableName;

  public DeveloperModeGuardTests() => DeveloperModeGuard.ResetStateForTests();

  [Fact]
  public void AppSettingOverridesEnvironment()
  {
    var config = new ConfigurationBuilder()
      .AddInMemoryCollection(new Dictionary<string, string?>
      {
        ["App:DeveloperMode"] = "true",
      })
      .Build();

    var logger = new TestLogger();

    var result = DeveloperModeGuard.Resolve(config, _ => "false", logger, isDebugBuild: false, environmentOverridesSupported: true, EnvKey, "Test");

    Assert.True(result);
    Assert.Empty(logger.Entries);
  }

  [Fact]
  public void EnvironmentOverrideEnablesDiagnosticsInRelease()
  {
    var logger = new TestLogger();

    var result = DeveloperModeGuard.Resolve(null, _ => "true", logger, isDebugBuild: false, environmentOverridesSupported: true, EnvKey, "Test");

    Assert.True(result);
    Assert.Contains(logger.Entries, entry => entry.Level == LogLevel.Warning && entry.Message.Contains("Developer diagnostics enabled"));
  }

  [Fact]
  public void EnvironmentOverrideLogsOnlyOnce()
  {
    var logger = new TestLogger();

    _ = DeveloperModeGuard.Resolve(null, _ => "true", logger, isDebugBuild: false, environmentOverridesSupported: true, EnvKey, "Test");
    _ = DeveloperModeGuard.Resolve(null, _ => "true", logger, isDebugBuild: false, environmentOverridesSupported: true, EnvKey, "Test");

    var warnings = logger.Entries.FindAll(entry => entry.Level == LogLevel.Warning);
    Assert.Single(warnings);
  }

  [Fact]
  public void ReleaseDefaultsToFalseWhenUnset()
  {
    var logger = new TestLogger();

    var result = DeveloperModeGuard.Resolve(null, _ => null, logger, isDebugBuild: false, environmentOverridesSupported: true, EnvKey, "Test");

    Assert.False(result);
    Assert.Empty(logger.Entries);
  }

  [Fact]
  public void DebugDefaultsToTrueWhenUnset()
  {
    var logger = new TestLogger(LogLevel.Debug);

    var result = DeveloperModeGuard.Resolve(null, _ => null, logger, isDebugBuild: true, environmentOverridesSupported: true, EnvKey, "Test");

    Assert.True(result);
    Assert.Empty(logger.Entries);
  }

  [Fact]
  public void EnvironmentOverrideDisabledLogsDebugOnce()
  {
    var logger = new TestLogger(LogLevel.Debug);

    _ = DeveloperModeGuard.Resolve(null, _ => "false", logger, isDebugBuild: false, environmentOverridesSupported: true, EnvKey, "Test");
    _ = DeveloperModeGuard.Resolve(null, _ => "false", logger, isDebugBuild: false, environmentOverridesSupported: true, EnvKey, "Test");

    var debugLogs = logger.Entries.FindAll(entry => entry.Level == LogLevel.Debug && entry.Message.Contains("disabled"));
    Assert.Single(debugLogs);
  }

  [Fact]
  public void EnvironmentOverrideIgnoredWhenOverridesUnsupported()
  {
    var logger = new TestLogger();

    var result = DeveloperModeGuard.Resolve(null, _ => "true", logger, isDebugBuild: false, environmentOverridesSupported: false, EnvKey, "BrowserTest");

    Assert.False(result);
    Assert.Empty(logger.Entries);
  }

  private sealed class TestLogger : ILogger
  {
    private readonly LogLevel minimumLevel;

    public TestLogger(LogLevel minimumLevel = LogLevel.Information)
    {
      this.minimumLevel = minimumLevel;
    }

    public List<(LogLevel Level, string Message)> Entries { get; } = new();

    public IDisposable BeginScope<TState>(TState state) where TState : notnull => NullScope.Instance;

    public bool IsEnabled(LogLevel logLevel) => logLevel >= this.minimumLevel;

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
      if (!this.IsEnabled(logLevel))
      {
        return;
      }

      var message = formatter(state, exception);
      this.Entries.Add((logLevel, message));
    }

    private sealed class NullScope : IDisposable
    {
      public static readonly NullScope Instance = new();

      public void Dispose()
      {
      }
    }
  }
}
