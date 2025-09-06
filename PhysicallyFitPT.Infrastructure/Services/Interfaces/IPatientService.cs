using PhysicallyFitPT.Domain;
namespace PhysicallyFitPT.Infrastructure.Services.Interfaces;
public interface IPatientService { Task<IEnumerable<Patient>> SearchAsync(string query, int take = 50); }
