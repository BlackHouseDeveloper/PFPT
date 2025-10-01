# Physically Fit PT (PFPT) – Enterprise Healthcare Documentation Platform

**Physically Fit PT (PFPT)** is a modern, enterprise-grade clinician documentation platform designed specifically for physical therapy practices. Built with .NET MAUI Blazor for native mobile and desktop experiences, Blazor WebAssembly for web deployment, and a comprehensive automation framework, PFPT follows Clean Architecture principles to ensure maintainability, scalability, and healthcare compliance.

## 🎯 Key Features

- **🏥 Clinical Documentation**: Comprehensive patient notes, appointment tracking, and treatment planning
- **📱 Multi-Platform Support**: Native iOS, Android, macOS desktop, and web applications
- **📄 PDF Export**: Professional patient reports and documentation export using QuestPDF
- **🗄️ SQLite Database**: Local data storage with Entity Framework Core for offline capabilities
- **🔧 Automation Tools**: Automated messaging workflows and assessment management
- **🎯 Modular Architecture**: Clean separation of concerns with domain-driven design
- **🔒 Data Security**: Local SQLite encryption support and audit trail capabilities

## Prerequisites

To set up and run PFPT on a development machine, ensure you have the following:

- **macOS** (Apple Silicon or Intel) – *PFPT is primarily developed and tested on macOS.*
- **.NET 8.0 SDK** – Download from Microsoft’s [.NET downloads](https://dotnet.microsoft.com/en-us/download/dotnet/8.0). Make sure `dotnet --version` shows a 8.x SDK.
- **Xcode** (latest) – Required for iOS and Mac Catalyst projects (install from the App Store). After installing, run `sudo xcode-select --switch /Applications/Xcode.app`.
- **.NET MAUI Workloads** – After installing the .NET SDK, install MAUI workloads by running:  
  ```bash
  dotnet workload install maui
  ```

This will set up Android, iOS, and MacCatalyst targets for .NET MAUI.

**IDE (optional)** – You can use Visual Studio 2022 (Windows or Mac) or Visual Studio Code. VS Code works well for editing, but launching the MAUI app might require CLI commands or VS for Mac.

> **Note**: Do not run any setup scripts with sudo. All development tasks should be run with normal user permissions.

## Getting Started

### 1. Clone the Repository

Start by cloning the PFPT repository from GitHub:

```bash
git clone https://github.com/BlackHouseDeveloper/PFPT.git
cd PFPT
```

### 2. Initial Setup and Database Configuration
PFPT includes a setup script PFPT-Foundry.sh to scaffold the solution and prepare the local database:
To ensure the project is fully set up (restore NuGet packages, ensure correct .NET 8 targets, etc.), you can run the setup script:
bash
Copy code
./PFPT-Foundry.sh
This will add any missing projects, enforce .NET 8.0 as the target framework for all projects, and set up boilerplate code. It’s safe to run multiple times (the script checks for existing files and won’t overwrite your code or data).
Creating the initial database migration: The first time you set up PFPT, you’ll need to create the initial EF Core migration (which sets up the SQLite schema). Run:
bash
Copy code
./PFPT-Foundry.sh --create-migration
This will install the EF Core tools (if not already installed), add an Initial migration in the PhysicallyFitPT.Infrastructure/Data/Migrations folder (if one doesn’t exist), and apply it to create a local SQLite database.
Seeding the development database: To insert sample data (e.g. a couple of patients and reference codes), run:
bash
Copy code
./PFPT-Foundry.sh --seed
This uses the PhysicallyFitPT.Seeder console project to populate the SQLite database with initial test data. By default, the data file is created at dev.physicallyfitpt.db in the project root. If the file already exists, the seeder will add any missing data without duplicating existing entries.
#### Verify the Database Path

The seeder creates `dev.physicallyfitpt.db` in the repo root. The API resolves its SQLite path in this order:

1. `PFP_DB_PATH` environment variable (CI/CD or container override)
2. `ConnectionStrings:DefaultConnection` in `appsettings.{Environment}.json`
3. Fallback `Data Source=pfpt.db`

For development, `appsettings.Development.json` already points at `dev.physicallyfitpt.db`, so the API immediately serves the seeded data. Use the helper to confirm what each context is using:

```bash
scripts/check_db_status.py --context api --require-data --require-tables Patients --require-nonzero
scripts/check_db_status.py --context seeder --require-data --require-tables Patients --require-nonzero
```

The script prints the resolved path and row counts, making it easy to spot mismatches.

### 3. Running the Application

PFPT supports multiple deployment targets for different use cases:

#### Desktop Application (Mac Catalyst)

Run as a native macOS desktop application:

```bash
dotnet build -t:Run -f net8.0-maccatalyst PhysicallyFitPT/PhysicallyFitPT.csproj
```

This compiles and launches the app as a Mac Catalyst desktop application. The UI displays remote statistics from the API; the local SQLite cache remains in the app sandbox unless you explicitly override `PFP_DB_PATH`.

**Alternative**: Open `PhysicallyFitPT.sln` in Visual Studio 2022 (Mac) and run the PhysicallyFitPT project targeting Mac Catalyst.

#### Mobile Applications

Deploy to iOS or Android once the API is running and reachable:

```bash
# Terminal 1 – start the API (Development config uses dev.physicallyfitpt.db)
dotnet run --project src/PhysicallyFitPT.Api --urls http://localhost:5114

# Terminal 2 – launch a platform target
dotnet build -t:Run -f net8.0-ios PhysicallyFitPT/PhysicallyFitPT.csproj
dotnet build -t:Run -f net8.0-android PhysicallyFitPT/PhysicallyFitPT.csproj
```

The MAUI bootstraper chooses the correct base URL:

- iOS simulator → http://localhost:5114
- Android emulator → http://10.0.2.2:5114

Override with `PFPT_API_BASE_URL` if you need to target a different host.

#### Web Application (Browser)

Run the Blazor WebAssembly client for browser-based access:

```bash
dotnet run --project PhysicallyFitPT.Web/PhysicallyFitPT.Web.csproj
```

This starts a local development server. Open the displayed URL (typically `http://localhost:5000`) in your browser.

**Note**: The web version talks to the same API instance. Configure the base URL in `wwwroot/appsettings.json` (or at deployment time) to point at the correct environment.

#### Development Tips

- **Hot Reload**: Supported in Visual Studio and VS Code for MAUI projects
- **Web Hot Reload**: Automatic when using `dotnet run` with the web project
- **Code Changes**: Re-run `dotnet build` or restart the development server after changes
### 4. Enterprise Architecture & Project Structure

PFPT follows Clean Architecture principles with clear separation of concerns:
PhysicallyFitPT – The .NET MAUI Blazor app (multi-targeted for Android, iOS, MacCatalyst, etc.). This is the primary app project.
PhysicallyFitPT.Web – The Blazor WebAssembly app for running PFPT in a web browser.
PhysicallyFitPT.Core – The domain entities (business models) with no EF Core or UI dependencies.
PhysicallyFitPT.Infrastructure – Implements the persistence (EF Core ApplicationDbContext), domain services, and PDF generation. It references the Domain project.
PhysicallyFitPT.Shared – Shared libraries, such as predefined lists (e.g., goal templates, interventions, outcome measures) that can be used by both the app and other projects.
PhysicallyFitPT.Tests – XUnit test project containing unit tests (runs on .NET 8.0).
PhysicallyFitPT.Seeder – A console application to seed the SQLite database with initial data for development/testing.
5. Using the PFPT-Foundry.sh Script
As mentioned, the repository includes a script (PFPT-Foundry.sh) to automate environment setup tasks. Here’s a quick reference on using it:
Running the script with no arguments will scaffold or update the solution structure (adding missing files, normalizing project settings). It’s idempotent – it won’t overwrite existing code, and you can run it after pulling updates to ensure your local projects match the expected configuration.
--create-migration: Generates the initial EF Core migration (if not already present) and updates the database. This is typically done once at project setup. If an initial migration already exists, the script will skip creating a duplicate.
--seed: Runs the seeder to populate sample data. Safe to run multiple times; it only inserts data if not already present (e.g., it won’t add duplicate patients if you run it again).
--help: Shows usage info. You can also open the script in a text editor – it’s heavily commented to explain each step it performs.
6. PDF Export and Branding
PFPT includes a basic PDF generation feature for patient notes or reports. The PDF rendering is handled by the PdfRenderer service (using QuestPDF). Currently, the PDF output is a simple template (A4 page with a title and body text).
Branding in the application is still in progress:
We have defined design tokens in CSS (see wwwroot/css/design-tokens.css) for colors and styles that match the intended brand palette (for example, a lime green accent color, certain font choices, etc.).
The current UI and PDF are using placeholder styling. Expectations: As the project evolves, logos and polished styles will be incorporated. For now, the focus is on functionality – the UI is minimalist (“Pre-Figma shell”) and the PDF export is for demonstration. In future updates, we plan to include clinic branding (e.g., logo, header) in PDF outputs and apply a consistent design system across the app.
7. Troubleshooting & FAQ
Database not found / issues: Ensure you have run the migration (--create-migration) and seeding steps. The SQLite database file dev.physicallyfitpt.db should be present in the project root and referenced by `appsettings.Development.json`. Use `scripts/check_db_status.py` to confirm the resolved path, or override it via PFP_DB_PATH if you need a different location.
iOS build issues: If building for iOS, make sure you’ve opened the project in Xcode at least once to accept any license agreements, and that you have an iOS simulator selected. You might also need to adjust code signing settings in Xcode for the iOS target if you deploy to a physical device.
Hot Reload/Live Reload: When running the MAUI app, .NET Hot Reload should work if you launch from Visual Studio. For the Blazor Web app, code changes generally require rebuilding (dotnet run will pick up changes on restart). Develop with the approach that suits you (for quick UI iteration, the Web app is convenient; for testing native features, use the MAUI app).
Developer diagnostics bar: See `docs/developer-mode.md` for enabling the debug stats bar and understanding override precedence.
App stats cache: Override `AppStats:CacheTtlSeconds` (default 15) to tune dashboard refresh cadence; write operations automatically invalidate the cache.
Diagnostics health endpoint: `/api/v1/diagnostics/info` reports whether developer diagnostics are active (adds `PFPT-Diagnostics: true` when enabled) along with the current cache TTL; optionally gate it with `App:DiagnosticsRequiredRole` for operator-only access.
Reverse-proxy hosting: Set `Api:BasePath` (e.g. `/pfpt`) to ensure generated client routes align with your deployment prefix.
For any other issues, please check the repository’s issue tracker or contact the maintainers. Happy documenting!

---

## Contributing

We welcome contributions to PFPT! Our enterprise-grade development environment includes automated workflows to ensure healthcare compliance and code quality.

### Quick Start for Contributors

1. **Fork and Clone**: Fork the repository and clone your fork
2. **Environment Setup**: Run `./PFPT-Foundry.sh` to set up development environment
3. **Validate Setup**: Run `gh workflow run mcp-copilot-setup-validation.yml -f validation_scope=full`
4. **Create Branch**: Use conventional naming (`feat/`, `fix/`, `docs/`, `clinical/`)
5. **Make Changes**: Follow coding standards and write tests
6. **Auto-formatting**: Our MCP workflows will auto-format code on PR submission
7. **Test Thoroughly**: Run `dotnet test` and verify all platforms build
8. **Submit PR**: Include clear description and reference any related issues

### Enterprise Development Standards

- **Code Style**: Auto-enforced via MCP workflows with StyleCop and Roslynator
- **Healthcare Compliance**: HIPAA considerations validated in all changes
- **Testing**: Comprehensive test coverage with platform-specific validation
- **Architecture**: Clean Architecture with healthcare-focused domain modeling
- **Accessibility**: WCAG 2.1 AA/AAA compliance for inclusive healthcare technology
- **Documentation**: Clinical documentation standards and API documentation

### Automated Quality Assurance

Our MCP framework automatically:
- **Validates environment setup** for all contributors
- **Formats code** according to healthcare software standards
- **Tests accessibility compliance** for clinical users
- **Validates PDF reports** for clinical documentation standards
- **Checks database changes** for HIPAA compliance
- **Generates comprehensive release notes** with clinical impact assessment

### Healthcare-Specific Contribution Guidelines

- **Clinical Workflow Impact**: Assess how changes affect clinical workflows
- **Patient Data Privacy**: Ensure all changes maintain HIPAA compliance
- **Accessibility**: Test with screen readers and keyboard navigation
- **Multi-platform Testing**: Validate changes across iOS, Android, macOS, and web
- **Clinical Terminology**: Use standardized medical terminology and coding systems

For detailed contribution guidelines and healthcare-specific development practices, see:
- `docs/DEVELOPMENT.md` - Comprehensive development guide
- `docs/ARCHITECTURE.md` - Technical architecture and patterns
- `.github/copilot-agent-instructions.md` - Copilot and MCP usage guide
- `docs/pfpt-script-usage-reference.md` - Quick reference for PFPT helper scripts and options

### Community & Support

- **Discussions**: Join community discussions for questions and ideas
- **Healthcare Provider Support**: Specialized support for clinical workflow questions
- **Security Issues**: Report security vulnerabilities via GitHub Security Advisories
- **Documentation**: Comprehensive guides in the `docs/` directory

---

*Happy documenting! 🏥📱*
