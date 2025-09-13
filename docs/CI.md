# CI Guardrails and Changelog

## Guardrails

- Web has no EF/SQLite packages (including transitive) and no reference to Infrastructure.
- Web source contains no `AddDbContext`, `.UseSqlite()`, or `.UseInMemoryDatabase()`.
- Device TFMs include SQLite and publish successfully.
- EF design-time gate passes with `EF_PROVIDER=sqlite`.

## Changelog Policy

- `/docs/CHANGELOG.md` is a primary context document for agents and reviewers.
- The workflow generates the changelog first on PRs, emphasizing successes (✅) for quick triage.
- Premium API usage is minimized:
  - Fetch workflow runs once, map by `head_sha`.
  - Paginate and cap runs/commits/jobs.
  - Write back once with `[skip ci]`.