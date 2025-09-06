using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PhysicallyFitPT.Domain;
using PhysicallyFitPT.Infrastructure.Data;
using PhysicallyFitPT.Infrastructure.Services.Interfaces;

namespace PhysicallyFitPT.Infrastructure.Services;

public class PatientService : BaseService, IPatientService
{
    private readonly IDbContextFactory<ApplicationDbContext> _dbFactory;
    
    public PatientService(IDbContextFactory<ApplicationDbContext> dbFactory, ILogger<PatientService> logger) 
        : base(logger)
    {
        _dbFactory = dbFactory;
    }

    public async Task<IEnumerable<Patient>> SearchAsync(string query, int take = 50)
    {
        try
        {
            // Validate inputs
            if (take <= 0 || take > 1000)
                throw new ArgumentException("Take parameter must be between 1 and 1000", nameof(take));
                
            using var db = await _dbFactory.CreateDbContextAsync();
            var q = (query ?? string.Empty).Trim().ToLower();
            
            // Prevent SQL injection by validating query length
            if (q.Length > 100)
                throw new ArgumentException("Search query too long", nameof(query));
                
            var like = $"%{q}%";
            return await db.Patients.AsNoTracking()
                .Where(p => EF.Functions.Like((p.FirstName + " " + p.LastName).ToLower(), like))
                .OrderBy(p => p.LastName).ThenBy(p => p.FirstName)
                .Take(take).ToListAsync();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error executing SearchAsync: {ErrorMessage}", ex.Message);
            return Enumerable.Empty<Patient>();
        }
    }
}
