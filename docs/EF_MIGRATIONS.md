# EF Migrations

- EF tooling should use a relational provider at design time. InMemory is not supported for migrations.
- Use `EF_PROVIDER=sqlite` to ensure a relational provider is selected.

## Commands

```bash
EF_PROVIDER=sqlite dotnet ef dbcontext info \
  -p ./PhysicallyFitPT.Infrastructure \
  -s ./PhysicallyFitPT/PhysicallyFitPT.csproj
```

## Web Runtime

- The web runtime must not reference EF or carry any database provider. All data access is via HTTP API.