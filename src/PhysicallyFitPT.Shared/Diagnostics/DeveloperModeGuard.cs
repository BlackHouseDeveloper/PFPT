// <copyright file="DeveloperModeGuard.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace PhysicallyFitPT.Shared.Diagnostics;

using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

/// <summary>
/// Resolves whether developer diagnostics should be enabled, respecting configuration, environment overrides, and build configuration.
/// </summary>
public static class DeveloperModeGuard
{
  /// <summary>
  /// Well-known environment variable key that can toggle developer diagnostics at runtime.
  /// </summary>
  /// <summary>
  /// Well-known environment variable key that toggles developer diagnostics at runtime.
  /// </summary>
  public const string EnvironmentVariableName = "PFPT_DEVELOPER_MODE";
  private static int releaseEnableWarningLogged;
  private static int releaseDisableNoticeLogged;
  private static int debugOverrideNoticeLogged;

  /// <summary>
  /// Evaluates the developer-mode flag using configuration, environment overrides, and build configuration defaults.
  /// </summary>
  /// <param name="configuration">The active configuration root.</param>
  /// <param name="environmentVariableProvider">Function used to look up environment variables by name.</param>
  /// <param name="logger">Logger used for diagnostic output.</param>
  /// <param name="isDebugBuild">Indicates whether the assembly was compiled in debug mode.</param>
  /// <param name="environmentOverridesSupported">Indicates whether environment variables can be resolved in the current host (false for WASM).</param>
  /// <param name="environmentKey">The environment variable key to check for overrides.</param>
  /// <param name="hostIdentifier">Identifier describing the component invoking the guard, used for logging scope context.</param>
  /// <returns><c>true</c> if developer diagnostics should be enabled; otherwise <c>false</c>.</returns>
  public static bool Resolve(
    IConfiguration? configuration,
    Func<string, string?> environmentVariableProvider,
    ILogger? logger,
    bool isDebugBuild,
    bool environmentOverridesSupported,
    string environmentKey,
    string hostIdentifier)
  {
    using var scope = logger?.BeginScope(new Dictionary<string, object>
    {
      ["DiagnosticsHost"] = hostIdentifier,
      ["BuildConfiguration"] = isDebugBuild ? "Debug" : "Release",
    });

    if (configuration is not null)
    {
      var configuredValue = configuration.GetValue<bool?>("App:DeveloperMode");
      if (configuredValue.HasValue)
      {
        return configuredValue.Value;
      }
    }

    var envValue = environmentOverridesSupported ? environmentVariableProvider?.Invoke(environmentKey) : null;
    if (bool.TryParse(envValue, out var envFlag))
    {
      EmitEnvironmentLogs(logger, envFlag, isDebugBuild);
      return envFlag;
    }

    return isDebugBuild;
  }

  /// <summary>
  /// Resets static logging guards. Intended for use in unit tests to ensure predictable assertions.
  /// </summary>
  internal static void ResetStateForTests()
  {
    Interlocked.Exchange(ref releaseEnableWarningLogged, 0);
    Interlocked.Exchange(ref releaseDisableNoticeLogged, 0);
    Interlocked.Exchange(ref debugOverrideNoticeLogged, 0);
  }

  private static void EmitEnvironmentLogs(ILogger? logger, bool envFlag, bool isDebugBuild)
  {
    if (logger is null)
    {
      return;
    }

    if (isDebugBuild)
    {
      if (!logger.IsEnabled(LogLevel.Debug))
      {
        return;
      }

      if (Interlocked.Exchange(ref debugOverrideNoticeLogged, 1) == 0)
      {
        logger.LogDebug(DiagnosticsEventIds.DeveloperDiagnosticsOverrideNotice, "PFPT_DEVELOPER_MODE override applied in debug build.");
      }

      return;
    }

    if (envFlag)
    {
      if (Interlocked.Exchange(ref releaseEnableWarningLogged, 1) == 0)
      {
        logger.LogWarning(DiagnosticsEventIds.DeveloperDiagnosticsEnabled, "Developer diagnostics enabled via PFPT_DEVELOPER_MODE override in production mode.");
      }
    }
    else if (logger.IsEnabled(LogLevel.Debug) && Interlocked.Exchange(ref releaseDisableNoticeLogged, 1) == 0)
    {
      logger.LogDebug(DiagnosticsEventIds.DeveloperDiagnosticsDisabled, "PFPT_DEVELOPER_MODE override disabled developer diagnostics in production mode.");
    }
  }
}
