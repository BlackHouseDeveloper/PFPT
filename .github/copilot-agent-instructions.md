# GitHub Copilot Agent Development Environment Instructions for PFPT

This document provides context and guidance for GitHub Copilot and Copilot Agents (MCPs) to maximize productivity, reliability, and troubleshooting for the PFPT repository.

## Repository Overview

Physically Fit PT (PFPT) is a cross-platform clinician documentation app for physical therapy practices, built with .NET MAUI Blazor and Blazor WebAssembly. It supports mobile and desktop devices, uses SQLite for data, includes PDF export, and features modular architecture and automation scripts.

## Key Technologies

- .NET MAUI Blazor (cross-platform UI/logic)
- Blazor WebAssembly (web client)
- SQLite (local storage)
- QuestPDF (PDF export)
- MSBuild, Cake (build automation)
- xUnit, NUnit (testing)
- Shell, Docker, Makefile (scripts, containerization)

## Development Environment Setup

- Latest stable .NET SDK (see repo README)
- VS Code with .NET MAUI and C# extensions
- Android SDK & OpenJDK 17 for Android builds
- Windows SDK for Windows builds
- Xcode (macOS only) for iOS/Mac Catalyst
- Docker (optional) for containerized builds

### Setup Steps

```sh
git clone https://github.com/BlackHouseDeveloper/PFPT.git
cd PFPT
dotnet tool restore
dotnet build
```

## Project Structure

- `src/` – Application, modules, platform code
- `docs/` – Documentation
- `scripts/` – Automation scripts (setup, export, DB, etc.)
- `tests/` – Unit and UI tests
- `.github/` – Workflows, Copilot/MCP instructions

Platform-specific code and extensions: `.windows.cs`, `.android.cs`, `.ios.cs`, etc.

### Core Projects

- **PhysicallyFitPT.Core** – Domain entities (business models) with no EF Core or UI dependencies
- **PhysicallyFitPT.Infrastructure** – Persistence (EF Core ApplicationDbContext), domain services, PDF generation
- **PhysicallyFitPT.Shared** – Shared libraries, predefined lists (goal templates, interventions, outcome measures)
- **PhysicallyFitPT.Maui** – .NET MAUI Blazor app (multi-targeted for Android, iOS, MacCatalyst)
- **PhysicallyFitPT.Web** – Blazor WebAssembly app for web browsers
- **PhysicallyFitPT.Seeder** – Console application to seed SQLite database with initial data
- **PhysicallyFitPT.Core.Tests** – XUnit test project containing unit tests

---

## Comprehensive Copilot Agent Improvements & Troubleshooting Enhancements

### 1. Contextual Instructions and Agent Awareness

- This file provides Copilot/MCPs with context on PFPT's architecture, technologies, directory layout, naming conventions, and supported platforms.
- Always reference `src/`, `docs/`, and `scripts/` for all code suggestions.
- Follow Clean Architecture principles with clear separation of concerns.

### 2. Platform-Specific Build and Test Guidance

- **Android/iOS/MacCatalyst**: Validate SDK installation, conditional compilation, and platform-specific code.
- **Windows**: Test native integration and PDF generation.
- **Web (Blazor WASM)**: Validate browser-based functionality and ensure no EF/SQLite dependencies.
- **Build matrix testing**: CI tests all supported platforms using strategy matrix.

### 3. Code Quality & Formatting Guidelines

Before PR submission, always run:

```sh
dotnet format PFPT.sln --verify-no-changes --verbosity diagnostic
```

**StyleCop Requirements:**
- All code must pass `dotnet format --verify-no-changes`
- Consistent indentation (2 spaces for C#/Razor)
- UTF-8 BOM encoding for source files

**Roslynator Analysis:**
- All code must pass `roslynator analyze --severity-level info`
- Resolve warnings/suggestions/errors before review or merge
- Focus on code quality and maintainability

### 4. Database and EF Core Guidelines

**Critical Guardrails:**
- Web project must remain database-stateless
- Web has NO EF/SQLite packages (including transitive)
- Web source contains NO `AddDbContext`, `.UseSqlite()`, or `.UseInMemoryDatabase()`
- Device TFMs include SQLite and publish successfully
- EF design-time gate passes with `EF_PROVIDER=sqlite`

**Database Workflow:**
```sh
# Create migration
dotnet ef migrations add <MigrationName> -p src/PhysicallyFitPT.Infrastructure -s src/PhysicallyFitPT.Maui

# Update database
dotnet ef database update -p src/PhysicallyFitPT.Infrastructure -s src/PhysicallyFitPT.Maui

# Verify design-time factory
EF_PROVIDER=sqlite dotnet ef dbcontext info -p src/PhysicallyFitPT.Infrastructure
```

### 5. Testing Strategy

**Unit Tests:**
- Located in `tests/PhysicallyFitPT.Core.Tests/`
- Run with: `dotnet test tests/PhysicallyFitPT.Core.Tests/PhysicallyFitPT.Core.Tests.csproj`
- All tests must pass before PR approval

**Integration Tests:**
- Test PDF generation with sample documents
- Validate database operations and migrations
- Test cross-platform compatibility

### 6. Build Optimization Strategies

**Runner Targeting:**
- Domain project (no dependencies) builds on Ubuntu for cost/speed efficiency
- All other projects build on macOS due to mobile framework dependencies
- Tests consolidated with iOS build to avoid duplicate macOS runners

**MAUI Workloads:**
- Installed once during Android build
- Verified on other mobile builds
- Use `dotnet workload restore` for consistency

**Performance Optimizations:**
- Strategic runner selection based on dependencies
- Parallel execution where possible
- Cached NuGet packages and workloads

---

## MCP Automation & Workflow Enhancements

### 1. CI/CD Automation MCPs

**Build Matrix MCP:**
```yaml
strategy:
  matrix:
    tfm: [net8.0, net8.0-android, net8.0-ios, net8.0-maccatalyst]
```

**Continuous Integration Flow:**
1. Format Check (StyleCop) → 2. Analysis (Roslynator) → 3. Build (Matrix) → 4. Test → 5. Deploy

**Auto-Review Request:**
- CI automatically requests reviewers on success
- Posts failure notifications with log tails on failure
- Integrates with Copilot auto-fix workflows

### 2. Documentation Automation MCPs

**API Documentation MCP:**
- Auto-generate XML docs for public APIs
- Update README.md with new features
- Maintain CHANGELOG.md with CI outcomes

**Architecture Documentation MCP:**
- Generate diagrams from code structure
- Update ARCHITECTURE.md with new modules
- Create onboarding documentation

### 3. Testing Automation MCPs

**Sample Generation MCP:**
- Auto-generate Blazor/MAUI sample pages for new components
- Create corresponding xUnit/NUnit tests for new modules
- Generate integration test scenarios

**Test Data MCP:**
- Create realistic test data for clinical scenarios
- Generate sample patient records
- Provide assessment templates and outcomes

### 4. Troubleshooting & Diagnostic MCPs

**Error Reproduction MCP:**
```sh
# Create minimal reproduction project
dotnet new console -n ReproCase
# Add necessary packages and reproduce issue
```

**Database Troubleshooting MCP:**
- Validate migration scripts
- Check database schema consistency
- Generate migration rollback scripts
- Verify data integrity

**PDF Export Diagnostic MCP:**
- Validate PDF layouts and templates
- Check for export errors and formatting issues
- Generate sample PDFs for testing
- Optimize PDF generation performance

**Accessibility Compliance MCP:**
- Scan UI components for accessibility compliance
- Generate accessibility audit reports
- Recommend WCAG 2.1 fixes
- Test with screen readers and assistive technologies

**Localization Workflow MCP:**
- Detect new UI strings requiring translation
- Auto-generate localization resource files
- Validate translation completeness
- Generate locale-specific test cases

### 5. Advanced Diagnostic MCPs

**Error Reproduction MCP** (`mcp-error-reproduction.yml`):
- Reproduces build, runtime, database, PDF, UI, and platform-specific errors
- Generates comprehensive error analysis reports
- Provides debugging information and system diagnostics
- Creates reproduction instructions for development team

**Database Diagnostic MCP** (`mcp-database-diagnostics.yml`):
- Validates EF Core design-time factory and migrations
- Creates, applies, and rolls back database migrations
- Seeds database with test data
- Performs database performance analysis
- Generates schema analysis and backup operations

**PDF Export Diagnostic MCP** (`mcp-pdf-diagnostics.yml`):
- Validates PDF generation and template rendering
- Tests PDF accessibility compliance (PDF/UA)
- Performs PDF generation performance benchmarking
- Generates sample PDFs for different scenarios
- Analyzes PDF structure and content extraction

**Accessibility Compliance MCP** (`mcp-accessibility-compliance.yml`):
- Performs automated accessibility audits using axe-core
- Tests WCAG 2.1 compliance at A, AA, or AAA levels
- Validates keyboard navigation and screen reader compatibility
- Generates accessibility implementation guides
- Tests form accessibility and UI component compliance

**Localization Workflow MCP** (`mcp-localization-workflow.yml`):
- Extracts localizable strings from source code
- Validates existing localization resources
- Generates localization templates and resource files
- Audits localization coverage across the application
- Updates resource files with new translations

**Documentation Automation MCP** (`mcp-documentation-automation.yml`):
- Generates API documentation from XML comments
- Creates architecture diagrams and project structure docs
- Generates developer onboarding guides
- Updates troubleshooting documentation
- Creates comprehensive documentation indexes

### 6. Security & Privacy MCPs

**Security Audit MCP:**
- Scan for sensitive data exposure
- Validate encryption implementations
- Check for SQL injection vulnerabilities
- Review authentication/authorization logic

**Privacy Compliance MCP:**
- Verify HIPAA compliance for patient data
- Check data retention policies
- Validate data anonymization
- Generate privacy impact assessments

---

## Advanced MCP Patterns & Collaboration

### 1. Cross-Platform Validation MCP

**Platform Compatibility Check:**
```sh
# Validate conditional compilation
#if ANDROID
// Android-specific code
#elif IOS
// iOS-specific code
#elif MACCATALYST
// Mac Catalyst-specific code
#endif
```

### 2. Dependency Management MCP

**Package Audit:**
- Validate NuGet package versions
- Check for security vulnerabilities
- Ensure license compatibility
- Monitor package update availability

### 3. Deployment Automation MCP

**Release Preparation:**
- Generate release notes from commit history
- Create deployment packages
- Validate configuration settings
- Prepare app store submissions

### 4. Developer Onboarding MCP

**Environment Setup:**
- Validate development environment
- Check required tools installation
- Configure IDE settings
- Run initial project setup

**Knowledge Transfer:**
- Generate architecture overview
- Create code walkthrough guides
- Provide best practices documentation
- Set up development workflows

### 5. Issue Triage & Resolution MCP

**Automated Triage:**
- Categorize issues by component
- Assign priority based on impact
- Suggest potential fixes
- Link to relevant documentation

**Bug Reproduction:**
- Create minimal reproduction cases
- Generate debug information
- Suggest troubleshooting steps
- Track resolution progress

---

## Contribution Guidelines for Copilot Agents

### Development Workflow

1. **Independent Solution Development:**
   - Develop solutions independently before referencing existing PRs
   - Compare all solution approaches
   - Document decision rationale
   - Explain improvements and trade-offs

2. **Code Quality Standards:**
   - Never commit generated files (`cgmanifest.json`, `templatestrings.json`)
   - Handle `PublicAPI.Unshipped.txt` changes with analyzer tools
   - Use revert/apply strategy for API changes
   - Always update XML docs for public APIs

3. **Testing Requirements:**
   - Add tests for new/changed code
   - Ensure all tests pass before review
   - Test cross-platform compatibility
   - Validate PDF export functionality

### PR and Issue Guidelines

**PR Templates:**
- Prompt users to test build artifacts
- Require changelog updates
- Comment on issue resolution
- Validate guardrails compliance

**Issue Templates:**
- Bug reports with reproduction steps
- Feature requests with use cases
- Documentation improvements
- Performance optimizations

---

## Monitoring & Continuous Improvement

### 1. MCP Effectiveness Tracking

**Metrics Collection:**
- Track automation success rates
- Monitor time savings achieved
- Measure code quality improvements
- Document developer satisfaction

### 2. Regular Updates & Refinements

**MCP Evolution:**
- Update instructions as PFPT evolves
- Incorporate new patterns and practices
- Refine automation based on feedback
- Expand diagnostic capabilities

**Knowledge Base Maintenance:**
- Keep documentation current
- Update examples and templates
- Refresh troubleshooting guides
- Maintain best practices

---

## Additional Resources

- [PFPT README](../README.md)
- [Architecture Documentation](../docs/ARCHITECTURE.md)
- [Development Guide](../docs/DEVELOPMENT.md)
- [Troubleshooting Guide](../docs/TROUBLESHOOTING.md)
- [.NET MAUI Documentation](https://learn.microsoft.com/en-us/dotnet/maui/)
- [Blazor WebAssembly Documentation](https://learn.microsoft.com/en-us/aspnet/core/blazor/webassembly)
- [QuestPDF Documentation](https://www.questpdf.com/)
- [Entity Framework Core Documentation](https://learn.microsoft.com/en-us/ef/core/)
- [GitHub Actions Documentation](https://docs.github.com/en/actions)

---

## Quick Reference Commands

### Environment Setup
```sh
# Environment setup
./PFPT-Foundry.sh

# Install required workloads
dotnet workload install maui
```

### Build Commands
```sh
# Build specific platforms
dotnet build src/PhysicallyFitPT.Maui/PhysicallyFitPT.Maui.csproj -c Release -f net8.0-android
dotnet build src/PhysicallyFitPT.Maui/PhysicallyFitPT.Maui.csproj -c Release -f net8.0-ios
dotnet build src/PhysicallyFitPT.Web/PhysicallyFitPT.Web.csproj -c Release

# Clean build
./pfpt-cleanbuild.sh
```

### Code Quality
```sh
# Code quality checks
dotnet format PFPT.sln --verify-no-changes
roslynator analyze PFPT.sln --severity-level info
```

### Database Operations
```sh
# Database operations
dotnet ef migrations add <Name> -p src/PhysicallyFitPT.Infrastructure -s src/PhysicallyFitPT.Maui
dotnet ef database update -p src/PhysicallyFitPT.Infrastructure -s src/PhysicallyFitPT.Maui

# Run database diagnostics
gh workflow run mcp-database-diagnostics.yml -f operation=validate
```

### Testing
```sh
# Unit testing
dotnet test tests/PhysicallyFitPT.Core.Tests/PhysicallyFitPT.Core.Tests.csproj

# PDF testing
gh workflow run mcp-pdf-diagnostics.yml -f test_type=validate

# Accessibility testing
gh workflow run mcp-accessibility-compliance.yml -f test_scope=ui-components
```

### MCP Workflow Commands
```sh
# Database diagnostics
gh workflow run mcp-database-diagnostics.yml -f operation=validate
gh workflow run mcp-database-diagnostics.yml -f operation=migrate
gh workflow run mcp-database-diagnostics.yml -f operation=seed

# PDF export testing
gh workflow run mcp-pdf-diagnostics.yml -f test_type=validate
gh workflow run mcp-pdf-diagnostics.yml -f test_type=performance
gh workflow run mcp-pdf-diagnostics.yml -f test_type=accessibility

# Documentation generation
gh workflow run mcp-documentation-automation.yml -f doc_type=api
gh workflow run mcp-documentation-automation.yml -f doc_type=all

# Accessibility compliance
gh workflow run mcp-accessibility-compliance.yml -f test_scope=full-audit -f accessibility_level=AA

# Localization workflow
gh workflow run mcp-localization-workflow.yml -f operation=extract
gh workflow run mcp-localization-workflow.yml -f operation=audit-coverage

# Error reproduction
gh workflow run mcp-error-reproduction.yml -f error_type=build -f platform=multi
gh workflow run mcp-error-reproduction.yml -f error_type=runtime -f platform=android
```

---

_Note: This document evolves with PFPT development. Expand sections as new workflows, troubleshooting techniques, and Copilot/MCP patterns are discovered or adopted._