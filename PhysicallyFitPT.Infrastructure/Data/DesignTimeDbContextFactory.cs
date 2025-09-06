using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace PhysicallyFitPT.Infrastructure.Data;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite($"Data Source={ResolveDbPath()}")
            .Options;
        return new ApplicationDbContext(options);
    }

    static string ResolveDbPath()
    {
        var root = FindRepoRoot() ?? Directory.GetCurrentDirectory();
        return Path.Combine(root, "pfpt.design.sqlite");
    }

    static string? FindRepoRoot()
    {
        var d = new DirectoryInfo(Directory.GetCurrentDirectory());
        while (d != null &&
               !File.Exists(Path.Combine(d.FullName, ".gitignore")) &&
               !File.Exists(Path.Combine(d.FullName, ".editorconfig")))
            d = d.Parent;
        return d?.FullName;
    }
}
