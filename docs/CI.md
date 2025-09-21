# CI Guardrails and Changelog

## Guardrails

- Web has no EF/SQLite packages (including transitive) and no reference to Infrastructure.
- Web source contains no `AddDbContext`, `.UseSqlite()`, or `.UseInMemoryDatabase()`.
- Device TFMs include SQLite and publish successfully.
- EF design-time gate passes with `EF_PROVIDER=sqlite`.

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
- The workflow generates the changelog first on PRs, emphasizing successes (✅) for quick triage.
- Premium API usage is minimized:
  - Fetch workflow runs once, map by `head_sha`.
  - Paginate and cap runs/commits/jobs.
  - PR-scoped commit ranges when available to reduce API calls.
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