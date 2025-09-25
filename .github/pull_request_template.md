# PR Checklist

## Core Requirements
- [ ] Web is DB-stateless: no DbContext/EF packages and no Infrastructure reference
- [ ] Devices include EF Core + SQLite and publish successfully
- [ ] EF design-time check passes with `EF_PROVIDER=sqlite`
- [ ] CI guardrails added/maintained
- [ ] CHANGELOG updated by CI on this PR with job outcomes (✅ emphasized on success)

## Code Quality & Testing
- [ ] All StyleCop formatting checks pass (`dotnet format --verify-no-changes`)
- [ ] All Roslynator analysis passes (severity-level: info)
- [ ] Unit tests pass for affected components
- [ ] Cross-platform compatibility verified

## Artifact Testing & Validation
- [ ] **Build Artifacts**: All target frameworks build successfully
  - [ ] net8.0 (Ubuntu-compatible projects)
  - [ ] net8.0-android (Android MAUI)
  - [ ] net8.0-ios (iOS MAUI) 
  - [ ] net8.0-maccatalyst (macOS MAUI)
  - [ ] Blazor WebAssembly (web deployment)

- [ ] **Functional Testing**: Core features work as expected
  - [ ] Patient management operations
  - [ ] Assessment creation and editing
  - [ ] PDF report generation (if applicable)
  - [ ] Database operations (if applicable)
  - [ ] Cross-platform UI consistency

- [ ] **Performance Impact**: No significant performance degradation
  - [ ] Application startup time acceptable
  - [ ] UI responsiveness maintained
  - [ ] Memory usage within expected bounds

## Platform-Specific Validation
- [ ] **Android**: APK builds and installs successfully
- [ ] **iOS**: IPA builds (unsigned) successfully  
- [ ] **Web**: Blazor WASM loads and functions correctly
- [ ] **Database**: Migration scripts work across platforms

## Documentation & Communication
- [ ] Changes documented in appropriate files
- [ ] Breaking changes clearly identified
- [ ] User-facing changes explained
- [ ] Developer impact documented

## Summary

Explain what changed and why, linking to docs where helpful. The Web must remain API-only with no database dependencies.

### Type of Change
- [ ] Bug fix (non-breaking change which fixes an issue)
- [ ] New feature (non-breaking change which adds functionality)
- [ ] Breaking change (fix or feature that would cause existing functionality to not work as expected)
- [ ] Documentation update
- [ ] Performance improvement
- [ ] Code refactoring

### Impact Areas
- [ ] Core business logic
- [ ] Database schema/migrations
- [ ] PDF generation
- [ ] User interface
- [ ] Build system
- [ ] CI/CD workflows

## Testing Instructions

Please test the following scenarios:

1. **Basic Functionality**:
   ```bash
   # Clone and setup
   git checkout <this-branch>
   ./PFPT-Foundry.sh
   
   # Test builds
   dotnet build src/PhysicallyFitPT.Web/PhysicallyFitPT.Web.csproj -c Release
   dotnet build src/PhysicallyFitPT.Maui/PhysicallyFitPT.Maui.csproj -c Release -f net8.0-android
   ```

2. **Database Operations** (if applicable):
   ```bash
   # Test migrations
   dotnet ef database update -p src/PhysicallyFitPT.Infrastructure -s src/PhysicallyFitPT.Maui
   
   # Verify design-time factory
   EF_PROVIDER=sqlite dotnet ef dbcontext info -p src/PhysicallyFitPT.Infrastructure
   ```

3. **PDF Generation** (if applicable):
   ```bash
   # Run PDF diagnostics
   gh workflow run mcp-pdf-diagnostics.yml -f test_type=validate
   ```

## Verification Commands

Run these commands locally to verify the changes:

```bash
# Code quality
dotnet format PFPT.sln --verify-no-changes --verbosity diagnostic
roslynator analyze PFPT.sln --severity-level info

# Build verification
dotnet build src/PhysicallyFitPT.Web/PhysicallyFitPT.Web.csproj -c Release -f net8.0
dotnet publish src/PhysicallyFitPT.Maui/PhysicallyFitPT.Maui.csproj -c Release -f net8.0-android
dotnet publish src/PhysicallyFitPT.Maui/PhysicallyFitPT.Maui.csproj -c Release -f net8.0-ios
dotnet publish src/PhysicallyFitPT.Maui/PhysicallyFitPT.Maui.csproj -c Release -f net8.0-maccatalyst

# Database validation (if applicable)
EF_PROVIDER=sqlite dotnet ef dbcontext info -p src/PhysicallyFitPT.Infrastructure

# Test execution
dotnet test tests/PhysicallyFitPT.Core.Tests/PhysicallyFitPT.Core.Tests.csproj -c Release
```

## Review Feedback

**For Reviewers**: Please comment on:
- [ ] Artifact functionality after downloading CI builds
- [ ] Cross-platform behavior consistency
- [ ] Performance impact (if observable)
- [ ] Documentation clarity and completeness
- [ ] Test coverage adequacy

**Post-Review**: Once approved, verify CI artifacts one final time before merging.

## Changelog Context

Reviewers and agents: treat [`/docs/CHANGELOG.md`](../docs/CHANGELOG.md) as a primary context document for this repository. The workflow generates or updates it first to guide subsequent tasks with minimal premium API calls.

## MCP Automation

This PR can leverage the following MCP workflows for enhanced validation:
- `mcp-database-diagnostics.yml` - Database migration and schema validation
- `mcp-pdf-diagnostics.yml` - PDF generation and template testing
- Copilot auto-fix workflows for CI failures