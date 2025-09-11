// <copyright file="SmokeTests.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace PhysicallyFitPT.Tests;

using FluentAssertions;
using Xunit;

/// <summary>
/// Basic smoke tests to verify test infrastructure is working correctly.
/// </summary>
public class SmokeTests
{
  /// <summary>
  /// Basic sanity test to verify that test infrastructure is functional.
  /// </summary>
  [Fact]
  public void Sanity() => true.Should().BeTrue();
}
