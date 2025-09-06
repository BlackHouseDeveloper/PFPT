using FluentAssertions;
using Xunit;

namespace PhysicallyFitPT.Tests;

public class SmokeTests
{
  [Fact]
  public void Sanity() => true.Should().BeTrue();
}
