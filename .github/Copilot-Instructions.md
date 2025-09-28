# PFPT Copilot Instructions

Applies to this repository. Do not request reviews or merge until all required checks pass.

## Workflow Awareness & Review Requests

- Wait for CI:
  - All required checks must pass:
    - StyleCop formatting (`dotnet format --verify-no-changes`)
    - Roslynator static analysis (`roslynator analyze`)
    - Web build (auto-detected Web project)
    - Mobile builds: Android and iOS (.NET MAUI, unsigned)
    - Unit tests

- Review readiness (gate):
  - Request review only when all checks are green.
  - CI will auto-request reviewers on success.

- Failure notifications:
  - If any job fails, CI posts a comment tagging you with:
    - Failed jobs/steps summary
    - Last 1000 chars of each failed step's log (inline)
    - Links to full logs (artifacts)

## Code Quality & Formatting

- StyleCop:
  - All code must pass `dotnet format --verify-no-changes`.

- Roslynator:
  - All code must pass `roslynator analyze`.
  - Resolve warnings/suggestions/errors before review or merge.

- Enforcement:
  - PRs are ineligible for review/merge until StyleCop and Roslynator checks pass.

## Build & Test Discipline

- Mobile builds:
  - Android and iOS build must succeed (.NET MAUI).
  - iOS builds run on macOS; signing disabled (`Codesign=false`).

- Web build:
  - CI auto-detects the Web project (SDK `Microsoft.NET.Sdk.Web`) and builds it.

- Unit tests:
  - Run all tests; add tests for new/changed code.
  - All tests must pass.

## Efficient Collaboration

- Notifications:
  - GitHub Actions notifies you on failures and includes log tails.

- Artifacts:
  - Build/test logs are uploaded. Link to them only on failure.

- Documentation:
  - Explain changes in PR descriptions, especially impacting build/test/formatting.

- Changelog:
  - Reference `changelog.md` entries relevant to the PR.

## Example CI Flow

- On PR open/update:
  - Trigger CI (restore, format, analyze, build Web, build Android, build iOS, test)
  - On any failure → comment with failed logs and tag reviewer
  - On success → auto-request reviewer

---

## Summary for Copilot

- Don't request reviews or merges until all formatting, analysis, builds (Android, iOS, Web), and tests pass.
- On CI failure, automatically comment with inline log tails (1000 chars) and artifact links.
- Enforce StyleCop and Roslynator.
- Reference `changelog.md` in PRs for context.
- **NEW**: Use comprehensive MCP workflows for advanced diagnostics:
  - `mcp-database-diagnostics.yml` for database operations
  - `mcp-pdf-diagnostics.yml` for PDF export testing
  - `mcp-accessibility-compliance.yml` for accessibility audits
  - `mcp-localization-workflow.yml` for localization management
  - `mcp-error-reproduction.yml` for debugging assistance
  - `mcp-documentation-automation.yml` for documentation updates
- **NEW**: Follow comprehensive guidelines in `.github/copilot-agent-instructions.md`

Notes:
- No teams are used. The CI defaults to requesting/mentioning: @BlackHouseDeveloper
- Override via repository variables (Settings → Variables → Repository):
  - REVIEWERS: comma/space/newline-separated GitHub usernames
  - MENTIONS: mention string to prefix failure comments (e.g., "@BlackHouseDeveloper and @copilot")