# Physically Fit PT (PFPT) – Clinician Documentation App

**Physically Fit PT (PFPT)** is a modern, cross-platform clinician documentation application designed specifically for physical therapy practice. Built with .NET MAUI Blazor for native mobile and desktop experiences, and Blazor WebAssembly for web deployment, PFPT follows Clean Architecture principles to ensure maintainability and scalability.

## Key Features

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
### 4. Project Structure

PFPT follows Clean Architecture principles with clear separation of concerns:
PhysicallyFitPT – The .NET MAUI Blazor app (multi-targeted for Android, iOS, MacCatalyst, etc.). This is the primary app project.
PhysicallyFitPT.Web – The Blazor WebAssembly app for running PFPT in a web browser.
PhysicallyFitPT.Domain – The domain entities (business models) with no EF Core or UI dependencies.
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
Database not found / issues: Ensure you have run the migration (--create-migration) at least once. The SQLite database file dev.physicallyfitpt.db should be present in the project root after running the script. Also set the PFP_DB_PATH environment variable so the app knows where to find the database file.
iOS build issues: If building for iOS, make sure you’ve opened the project in Xcode at least once to accept any license agreements, and that you have an iOS simulator selected. You might also need to adjust code signing settings in Xcode for the iOS target if you deploy to a physical device.
Hot Reload/Live Reload: When running the MAUI app, .NET Hot Reload should work if you launch from Visual Studio. For the Blazor Web app, code changes generally require rebuilding (dotnet run will pick up changes on restart). Develop with the approach that suits you (for quick UI iteration, the Web app is convenient; for testing native features, use the MAUI app).
For any other issues, please check the repository’s issue tracker or contact the maintainers. Happy documenting!

---

## Contributing

We welcome contributions! Please see our contributing guidelines for development standards, branch naming conventions, and pull request process.

### Quick Steps

1. **Fork and Clone**: Fork the repository and clone your fork
2. **Environment Setup**: Run `./PFPT-Foundry.sh` to set up development environment
3. **Create Branch**: Use conventional naming (`feat/`, `fix/`, `docs/`)
4. **Make Changes**: Follow coding standards and write tests
5. **Test Thoroughly**: Run `dotnet test` and verify all platforms build
6. **Submit PR**: Include clear description and reference any related issues

### Standards

- **Code Style**: Enforced via StyleCop and Roslynator analyzers
- **Testing**: Maintain test coverage for new features using xUnit
- **Documentation**: Update relevant docs for any changes
- **Architecture**: Follow Clean Architecture and DDD principles

For detailed contribution guidelines, see `DEVELOPMENT.md`.

---

*Happy documenting! 🏥📱*
