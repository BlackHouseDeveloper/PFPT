// <copyright file="SmokeTests.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace PhysicallyFitPT.Tests;

using FluentAssertions;
using Xunit;

public class SmokeTests
{
  [Fact]
  public void Sanity() => true.Should().BeTrue();
}
