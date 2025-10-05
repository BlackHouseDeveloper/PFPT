// <copyright file="DiagnosticsInfoDto.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace PhysicallyFitPT.Shared.Diagnostics;

/// <summary>
/// Describes the current diagnostics posture for operational monitoring.
/// </summary>
public readonly record struct DiagnosticsInfoDto(bool DeveloperDiagnosticsEnabled, int CacheTtlSeconds);
