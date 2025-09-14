# Troubleshooting

## Web has EF/SQLite packages

- CI error: "EF/Sqlite leaked into Web"
- Action: remove EF-related `PackageReference` items in `PhysicallyFitPT.Web.csproj` and ensure no ProjectReference to Infrastructure.

## Web contains DbContext usage

- CI error: "DbContext usage found in Web"
- Action: remove `AddDbContext`, `.UseSqlite()`, `.UseInMemoryDatabase()`. Route data access through typed HTTP clients.

## EF design-time fails

- Ensure `EF_PROVIDER=sqlite` is set and tooling is available.
- Command:
  ```bash
  EF_PROVIDER=sqlite dotnet ef dbcontext info -p ./PhysicallyFitPT.Infrastructure -s ./PhysicallyFitPT/PhysicallyFitPT.csproj
  ```

## Device publish fails

- Verify MAUI workload installed and platform prerequisites (Java 17, Android/iOS tooling).