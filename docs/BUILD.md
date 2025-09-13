# BUILD

## Quick Commands

- Web (net8.0)
  - `dotnet build ./PhysicallyFitPT.Web/PhysicallyFitPT.Web.csproj -c Release -f net8.0`

- Android / iOS / MacCatalyst
  - `dotnet publish ./PhysicallyFitPT/PhysicallyFitPT.csproj -c Release -f net8.0-android`
  - `dotnet publish ./PhysicallyFitPT/PhysicallyFitPT.csproj -c Release -f net8.0-ios`
  - `dotnet publish ./PhysicallyFitPT/PhysicallyFitPT.csproj -c Release -f net8.0-maccatalyst`

- EF Design-time (relational provider):
  - `EF_PROVIDER=sqlite dotnet ef dbcontext info -p ./PhysicallyFitPT.Infrastructure -s ./PhysicallyFitPT/PhysicallyFitPT.csproj`

## Dependency Policy

- Web (net8.0) is DB-stateless and API-only. No EF Core, no SQLite, and no ProjectReference to Infrastructure.
- Device TFMs (net8.0-android/ios/maccatalyst) include EF Core + SQLite for on-device storage and offline sync.