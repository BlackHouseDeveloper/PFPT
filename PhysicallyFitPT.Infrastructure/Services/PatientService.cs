using Microsoft.EntityFrameworkCore;
using PhysicallyFitPT.Domain;
using PhysicallyFitPT.Infrastructure.Data;
using PhysicallyFitPT.Infrastructure.Services.Interfaces;

namespace PhysicallyFitPT.Infrastructure.Services;
public class PatientService : IPatientService
{
    private readonly IDbContextFactory<ApplicationDbContext> _dbFactory;
    public PatientService(IDbContextFactory<ApplicationDbContext> dbFactory) => _dbFactory = dbFactory;

    public async Task<IEnumerable<Patient>> SearchAsync(string query, int take = 50)
    {
        using var db = await _dbFactory.CreateDbContextAsync();
        var q = (query ?? string.Empty).Trim().ToLower();
        var like = $"%{q}%";
        return await db.Patients.AsNoTracking()
            .Where(p => EF.Functions.Like((p.FirstName + " " + p.LastName).ToLower(), like))
            .OrderBy(p => p.LastName).ThenBy(p => p.FirstName)
            .Take(take).ToListAsync();
    }
}
