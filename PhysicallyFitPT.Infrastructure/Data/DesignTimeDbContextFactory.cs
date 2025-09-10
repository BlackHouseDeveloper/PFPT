// <copyright file="DesignTimeDbContextFactory.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace PhysicallyFitPT.Infrastructure.Data;

using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

/// <summary>
/// Factory for creating ApplicationDbContext instances at design time for Entity Framework tooling.
/// </summary>
public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
  /// <inheritdoc/>
  public ApplicationDbContext CreateDbContext(string[] args)
  {
    var options = new DbContextOptionsBuilder<ApplicationDbContext>()
        .UseSqlite($"Data Source={ResolveDbPath()}")
        .Options;
    return new ApplicationDbContext(options);
  }

  private static string ResolveDbPath()
  {
    var root = FindRepoRoot() ?? Directory.GetCurrentDirectory();
    return Path.Combine(root, "pfpt.design.sqlite");
  }

  private static string? FindRepoRoot()
  {
    var d = new DirectoryInfo(Directory.GetCurrentDirectory());
    while (d != null &&
           !File.Exists(Path.Combine(d.FullName, ".gitignore")) &&
           !File.Exists(Path.Combine(d.FullName, ".editorconfig")))
    {
      d = d.Parent;
    }

    return d?.FullName;
  }
}
