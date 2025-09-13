# PR Checklist

- [ ] Web is DB-stateless: no DbContext/EF packages and no Infrastructure reference.
- [ ] Devices include EF Core + SQLite and publish successfully.
- [ ] EF design-time check passes with `EF_PROVIDER=sqlite`.
- [ ] CI guardrails added/maintained.
- [ ] CHANGELOG updated by CI on this PR with job outcomes (✅ emphasized on success).

## Summary

Explain what changed and why, linking to docs where helpful. The Web must remain API-only with no database dependencies.

## Verification

- Commands run locally (build/publish, EF design-time):
  - `dotnet build ./PhysicallyFitPT.Web/PhysicallyFitPT.Web.csproj -c Release -f net8.0`
  - `dotnet publish ./PhysicallyFitPT/PhysicallyFitPT.csproj -c Release -f net8.0-android`
  - `dotnet publish ./PhysicallyFitPT/PhysicallyFitPT.csproj -c Release -f net8.0-ios`
  - `dotnet publish ./PhysicallyFitPT/PhysicallyFitPT.csproj -c Release -f net8.0-maccatalyst`
  - `EF_PROVIDER=sqlite dotnet ef dbcontext info -p ./PhysicallyFitPT.Infrastructure -s ./PhysicallyFitPT/PhysicallyFitPT.csproj`

## Changelog as Primary Context

Reviewers and agents: treat [`/docs/CHANGELOG.md`](../docs/CHANGELOG.md) as a primary context document for this repository. The workflow generates or updates it first to guide subsequent tasks with minimal premium API calls.