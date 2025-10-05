# CI Guardrails and Changelog

## Guardrails

- Web has no EF/SQLite packages (including transitive) and no reference to Infrastructure.
- Web source contains no `AddDbContext`, `.UseSqlite()`, or `.UseInMemoryDatabase()`.
- Device TFMs include SQLite and publish successfully.
- EF design-time gate passes with `EF_PROVIDER=sqlite`.
- CI fails fast if `PFPT_DEVELOPER_MODE` is enabled in the build environment.
- CI validates that both the API and seeder resolve seeded SQLite databases via `scripts/check_db_status.py`.
- Allowlist for diagnostics overrides: set `DEV_MODE_OVERRIDE_ALLOWLIST` (comma-delimited refs) to permit non-production branches to run with developer diagnostics enabled.

## Performance Optimizations

- **Strategic runner targeting**: 
  - Domain project (no dependencies) builds on Ubuntu for cost/speed efficiency
  - All other projects build on macOS due to mobile framework dependencies
  - Tests consolidated with iOS build to avoid duplicate macOS runners
- **MAUI workloads**: Installed once during Android build, verified on other mobile builds.
- **Parallel execution**: Format checks and analysis run in parallel where possible.
- **Reduced API usage**: Changelog generation limited to 500 commits/runs vs 1500.

## Changelog Policy

- `/docs/CHANGELOG.md` is a primary context document for agents and reviewers.
- **Post-merge strategy**: The workflow generates the changelog automatically after merges to main branch, preventing merge conflicts and ensuring accuracy.
- Premium API usage is minimized:
  - Fetch workflow runs once, map by `head_sha`.
  - Paginate and cap runs/commits/jobs.
  - Only runs on merge to main, not on every PR.
  - Write back once with `[skip ci]`.

## Failure Notifications

- Enhanced log access via artifact links and collapsible log sections.
- Improved reliability with better error handling for log extraction.
- Detects `timed_out` jobs in addition to `failure` and `cancelled`.
- Fallback mechanisms for undefined MENTIONS/REVIEWERS variables.

## Build Strategy

Due to the multi-targeted nature of shared components, the build strategy is:

- **Ubuntu Runner**: Domain project only (fastest, cheapest for dependency-free work)
- **macOS Runner**: All other projects due to transitive mobile dependencies:
  - Web, Infrastructure, Shared, Tests, Seeder (net8.0 variants)
  - PhysicallyFitPT MAUI app (mobile TFMs)
  
This provides cost optimization while maintaining compatibility.

## Known Follow-ups

- `pfpt-ci-roslynator-razor` (2025-09-27): Roslynator CLI does not yet load Razor-generated components from `PhysicallyFitPT.Shared` when analyzing `PhysicallyFitPT.Web`. The net8.0-ios guardrail temporarily skips that project to keep the workflow green. Re-enable the Web analysis once an upstream Roslynator release confirms Razor RCL support, then run **Build & Guardrails (net8.0-ios)** to verify.
