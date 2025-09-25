# Physically Fit PT (PFPT) – Enterprise Healthcare Documentation Platform

**Physically Fit PT (PFPT)** is a modern, enterprise-grade clinician documentation platform designed specifically for physical therapy practices. Built with .NET MAUI Blazor for native mobile and desktop experiences, Blazor WebAssembly for web deployment, and a comprehensive automation framework, PFPT follows Clean Architecture principles to ensure maintainability, scalability, and healthcare compliance.

## 🎯 Key Features

### 🏥 Clinical Documentation & Workflows
- **Comprehensive patient management** with HIPAA-compliant documentation
- **Assessment and outcome tracking** with standardized clinical tools  
- **Treatment planning and progress monitoring** with automated reporting
- **Clinical decision support** with evidence-based assessment integration

### 📱 Multi-Platform Support
- **Native iOS and Android** applications for mobile clinical use
- **macOS desktop** application for comprehensive clinic management
- **Web application** for browser-based access and administration
- **Cross-platform synchronization** with offline capabilities

### 📄 Professional Reporting & Documentation
- **PDF report generation** using QuestPDF with clinical templates
- **Accessibility-compliant documents** meeting PDF/UA standards
- **Customizable clinical templates** for different therapy specialties
- **Automated report generation** with patient progress summaries

### 🤖 Enterprise Automation & MCP Framework
- **11 production-ready MCP workflows** for development automation
- **Intelligent issue triage** with healthcare-specific categorization
- **Automated code formatting** and quality assurance
- **Release notes generation** with clinical impact assessment
- **Comprehensive testing automation** across all platforms

### 🔒 Healthcare Compliance & Security
- **HIPAA-compliant architecture** with patient data protection
- **Local SQLite encryption** with audit trail capabilities
- **Accessibility compliance** (WCAG 2.1 AA/AAA) for inclusive healthcare
- **Clinical data validation** with medical terminology support

### 🛠️ Developer Experience & Architecture
- **Clean Architecture** with domain-driven design principles
- **Modular component system** with shared clinical libraries
- **Comprehensive testing framework** with xUnit and automated validation
- **Enterprise-level CI/CD** with automated quality gates

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
#### Configure Database Path

After setup, you'll have a SQLite database (`dev.physicallyfitpt.db`) with the latest schema and seed data. To have the app use this database, set the environment variable:

```bash
export PFP_DB_PATH="$(pwd)/dev.physicallyfitpt.db"
```

If `PFP_DB_PATH` is not set:
- **MAUI app**: Uses `FileSystem.AppDataDirectory` for local storage
- **Web app**: Uses in-memory database (no persistence across sessions)

### 3. Running the Application

PFPT supports multiple deployment targets for different use cases:

#### Desktop Application (Mac Catalyst)

Run as a native macOS desktop application:

```bash
dotnet build -t:Run -f net8.0-maccatalyst PhysicallyFitPT/PhysicallyFitPT.csproj
```

This compiles and launches the app as a Mac Catalyst desktop application. The app will use the SQLite database if `PFP_DB_PATH` is set, otherwise it uses local app storage.

**Alternative**: Open `PhysicallyFitPT.sln` in Visual Studio 2022 (Mac) and run the PhysicallyFitPT project targeting Mac Catalyst.

#### Mobile Applications

Deploy to iOS or Android devices/simulators via Visual Studio:

```bash
# iOS Simulator
dotnet build -t:Run -f net8.0-ios PhysicallyFitPT/PhysicallyFitPT.csproj

# Android Emulator/Device  
dotnet build -t:Run -f net8.0-android PhysicallyFitPT/PhysicallyFitPT.csproj
```

*Ensure you have the respective SDKs and devices/simulators configured.*

#### Web Application (Browser)

Run the Blazor WebAssembly client for browser-based access:

```bash
dotnet run --project PhysicallyFitPT.Web/PhysicallyFitPT.Web.csproj
```

This starts a local development server. Open the displayed URL (typically `http://localhost:5000`) in your browser.

**Note**: The web version uses an in-memory database with no persistence across sessions. This is designed for demo and development convenience.

#### Development Tips

- **Hot Reload**: Supported in Visual Studio and VS Code for MAUI projects
- **Web Hot Reload**: Automatic when using `dotnet run` with the web project
- **Code Changes**: Re-run `dotnet build` or restart the development server after changes
### 4. Enterprise Architecture & Project Structure

PFPT follows Clean Architecture principles with a modular, healthcare-focused design:

```
PFPT/
├── src/
│   ├── PhysicallyFitPT.Maui/           # .NET MAUI Blazor (iOS, Android, macOS)
│   ├── PhysicallyFitPT.Web/            # Blazor WebAssembly (Browser)
│   ├── PhysicallyFitPT.Api/            # Web API (Future: Cloud Services)
│   ├── PhysicallyFitPT.Core/           # Domain Entities & Business Logic
│   ├── PhysicallyFitPT.Infrastructure/ # Data Access, EF Core, PDF Services
│   ├── PhysicallyFitPT.Shared/         # Clinical Libraries & DTOs
│   ├── PhysicallyFitPT.AI/             # AI/ML Services (Clinical Intelligence)
│   └── PhysicallyFitPT.Seeder/         # Database Seeding Console App
├── tests/
│   ├── PhysicallyFitPT.Core.Tests/     # Unit Tests (xUnit)
│   ├── PhysicallyFitPT.Api.Tests/      # API Integration Tests
│   └── PhysicallyFitPT.Maui.Tests/     # UI and Platform Tests
├── docs/                               # Comprehensive Documentation
├── .github/workflows/                  # 14 Enterprise MCP Workflows
└── PFPT-Foundry.sh                    # Development Environment Setup
```

#### Core Projects

**PhysicallyFitPT.Core** (Domain Layer)
- Clean domain entities with no external dependencies
- Healthcare-specific business logic and clinical workflows  
- HIPAA-compliant data models and patient privacy rules

**PhysicallyFitPT.Infrastructure** (Data Layer)
- Entity Framework Core with SQLite provider
- QuestPDF integration for clinical report generation
- Clinical data repositories and healthcare services

**PhysicallyFitPT.Maui** (Presentation Layer - Native)
- Cross-platform mobile and desktop applications
- Native platform integrations for iOS, Android, macOS
- Offline-first architecture with local data synchronization

**PhysicallyFitPT.Web** (Presentation Layer - Browser)
- Blazor WebAssembly for web-based clinical access
- Progressive Web App (PWA) capabilities
- Real-time collaboration features for clinical teams

**PhysicallyFitPT.AI** (Intelligence Layer)
- Clinical decision support and assessment automation
- Natural language processing for clinical documentation
- Predictive analytics for patient outcomes

#### Enterprise MCP Automation Framework

PFPT includes a comprehensive automation framework with 14 production-ready workflows:

**Development Automation:**
- `mcp-auto-format-pr.yml` - Automatic code formatting on PRs
- `mcp-daily-formatting.yml` - Daily maintenance formatting
- `mcp-copilot-setup-validation.yml` - Development environment validation

**Healthcare-Specific Automation:**
- `mcp-accessibility-compliance.yml` - WCAG 2.1 compliance testing  
- `mcp-pdf-diagnostics.yml` - Clinical report validation
- `mcp-database-diagnostics.yml` - HIPAA-compliant data management

**Enterprise Operations:**
- `mcp-release-notes-generation.yml` - Clinical impact release notes
- `mcp-triage.yml` - Intelligent issue categorization
- `mcp-documentation-automation.yml` - Clinical documentation generation

**Quality Assurance:**
- `mcp-error-reproduction.yml` - Comprehensive debugging automation
- `mcp-localization-workflow.yml` - Multi-language clinical terminology
- `ci.yml` - Multi-platform CI/CD with healthcare compliance

## 🤖 Enterprise MCP Workflows & Automation

PFPT includes a comprehensive Model Context Protocol (MCP) framework designed for healthcare software development:

### Development Automation
```bash
# Validate development environment
gh workflow run mcp-copilot-setup-validation.yml -f validation_scope=full

# Auto-format code on PR submission  
gh workflow run mcp-auto-format-pr.yml

# Daily code maintenance
gh workflow run mcp-daily-formatting.yml
```

### Healthcare-Specific Automation
```bash
# WCAG 2.1 accessibility compliance testing
gh workflow run mcp-accessibility-compliance.yml -f test_scope=full-audit

# Clinical PDF report validation
gh workflow run mcp-pdf-diagnostics.yml -f test_type=accessibility

# HIPAA-compliant database diagnostics
gh workflow run mcp-database-diagnostics.yml -f operation=validate
```

### Enterprise Operations
```bash
# Generate clinical impact release notes
gh workflow run mcp-release-notes-generation.yml -f release_type=feature -f from_tag=v1.0.0 -f to_tag=v1.1.0

# Clinical documentation automation
gh workflow run mcp-documentation-automation.yml -f doc_type=all

# Multi-language clinical terminology management
gh workflow run mcp-localization-workflow.yml -f operation=audit-coverage
```

### Quality Assurance & Debugging
```bash
# Comprehensive error reproduction and analysis
gh workflow run mcp-error-reproduction.yml -f error_type=platform-specific -f platform=android

# Automated issue triage with healthcare categorization
# (Runs automatically on issue creation)
```

All workflows include:
- **Healthcare compliance** considerations (HIPAA, accessibility)
- **Clinical workflow** impact assessment
- **Multi-platform** testing and validation
- **Comprehensive reporting** with actionable insights

### 5. Development Environment Setup

#### Automated Setup (Recommended)
```bash
git clone https://github.com/BlackHouseDeveloper/PFPT.git
cd PFPT
./PFPT-Foundry.sh
```

#### Manual Setup
```bash
# Install .NET MAUI workloads
dotnet workload install maui

# Create database migration
./PFPT-Foundry.sh --create-migration

# Seed with clinical test data  
./PFPT-Foundry.sh --seed

# Set database path (optional)
export PFP_DB_PATH="$(pwd)/dev.physicallyfitpt.db"
```

#### Environment Validation
```bash
# Comprehensive environment check
gh workflow run mcp-copilot-setup-validation.yml -f validation_scope=full
```

This validates:
- .NET SDK and MAUI workloads
- PFPT dependencies (QuestPDF, EF Core, SQLite)
- Project structure and build capabilities
- Development tool accessibility

### 6. Using the PFPT-Foundry.sh Script
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
Database not found / issues: Ensure you have run the migration (--create-migration) at least once. The SQLite database file dev.physicallyfitpt.db should be present in the project root after running the script. Also set the PFP_DB_PATH environment variable so the app knows where to find the database file.
iOS build issues: If building for iOS, make sure you’ve opened the project in Xcode at least once to accept any license agreements, and that you have an iOS simulator selected. You might also need to adjust code signing settings in Xcode for the iOS target if you deploy to a physical device.
Hot Reload/Live Reload: When running the MAUI app, .NET Hot Reload should work if you launch from Visual Studio. For the Blazor Web app, code changes generally require rebuilding (dotnet run will pick up changes on restart). Develop with the approach that suits you (for quick UI iteration, the Web app is convenient; for testing native features, use the MAUI app).
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
