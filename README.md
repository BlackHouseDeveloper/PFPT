# Physically Fit PT (PFPT) – Clinician Documentation App

**Physically Fit PT (PFPT)** is a cross-platform clinician documentation application for physical therapy practice. It is built with .NET MAUI Blazor (for mobile and desktop) and Blazor WebAssembly (for web), following a Clean Architecture pattern. This app enables physical therapists to record patient notes, appointments, and more, with support for generating PDF summaries.

## Prerequisites

To set up and run PFPT on a development machine, ensure you have the following:

- **macOS** (Apple Silicon or Intel) – *PFPT is primarily developed and tested on macOS.*
- **.NET 8.0 SDK** – Download from Microsoft’s [.NET downloads](https://dotnet.microsoft.com/en-us/download/dotnet/8.0). Make sure `dotnet --version` shows a 8.x SDK.
- **Xcode** (latest) – Required for iOS and Mac Catalyst projects (install from the App Store). After installing, run `sudo xcode-select --switch /Applications/Xcode.app`.
- **.NET MAUI Workloads** – After installing the .NET SDK, install MAUI workloads by running:  
  ```bash
  dotnet workload install maui
This will set up Android, iOS, and MacCatalyst targets for .NET MAUI.
IDE (optional) – You can use Visual Studio 2022 (Windows or Mac) or Visual Studio Code. VS Code works well for editing, but launching the MAUI app might require CLI commands or VS for Mac.
Note: Do not run any setup scripts with sudo. All development tasks should be run with normal user permissions.
Getting Started
1. Clone the Repository
Start by cloning the PFPT repository from GitHub:
bash
Copy code
git clone https://github.com/BlackHouseDeveloper/PFPT.git
cd PFPT
2. Initial Setup and Database Configuration
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
After running the above, you should have a SQLite database (dev.physicallyfitpt.db) with the latest schema and seed data. To have the app use this database, set an environment variable pointing to the file before running the app:
bash
Copy code
export PFP_DB_PATH="$(pwd)/dev.physicallyfitpt.db"
If PFP_DB_PATH is not set, the MAUI app will default to an in-memory database (useful for running the Web version without persistent storage).
3. Running the Application
PFPT can be run as a MAUI app or as a Blazor WebAssembly app:
Run as MAUI App (Mac Catalyst/Desktop):
The MAUI Blazor app can run as a macOS desktop application. Use the dotnet CLI:
bash
Copy code
dotnet build -t:Run -f net8.0-maccatalyst PhysicallyFitPT/PhysicallyFitPT.csproj
This will compile and launch the app as a Mac Catalyst app. You should see a desktop window with the PFPT interface. By default, it will use the SQLite database file if PFP_DB_PATH is set (as above); otherwise it runs with an in-memory database. Alternatively: You can open PhysicallyFitPT.sln in Visual Studio 2022 (Mac) and run the PhysicallyFitPT project targeting Mac Catalyst. For iOS or Android, you may deploy via Visual Studio to a simulator or device (ensure you have the respective SDKs and devices available).
Run as Blazor Web App (WebAssembly):
The solution includes PhysicallyFitPT.Web, a Blazor WebAssembly client that runs in the browser. To start it, run:
bash
Copy code
dotnet run --project PhysicallyFitPT.Web/PhysicallyFitPT.Web.csproj
This will build the WebAssembly project and start a local development web server (using the Blazor Dev Server). The console output will indicate the local URL (typically http://localhost:5000 or http://localhost:5001). Open that URL in your browser to use the app. The Web version uses an in-memory EF Core database (no persistence) – it registers an in-memory database context so data will reset on page refresh. This is intended mainly for demo and development convenience.
Tip: If you make changes to the code, you can re-run dotnet build or dotnet run as above. For the web project, the development server supports Hot Reload in supported editors, or you can manually refresh the browser after rebuilding.
4. Project Structure
PFPT is organized into several .NET projects for a clear separation of concerns (see Solution Architecture for more details):
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
perl
Copy code

### CONTRIBUTING.md

```markdown
# Contributing to PhysicallyFitPT

Thank you for contributing to the Physically Fit PT project! This guide outlines the conventions and best practices for development to ensure a smooth collaboration.

## Branch Naming Convention

We use a branch naming strategy to categorize the work being done:
- **Feature branches** – use the prefix `feat/` followed by a short description.  
  *Example:* `feat/patient-intake-form` for a new intake form feature.
- **Bug fix branches** – use the prefix `fix/` followed by the issue or bug description.  
  *Example:* `fix/appointment-timezone-bug`.
- **Refactor branches** – use the prefix `refactor/` for code refactoring or cleanup that doesn’t add new features.  
  *Example:* `refactor/ui-components-structure`.

Use hyphens `-` to separate words, and keep branch names concise yet descriptive. Include the issue number if applicable (e.g., `fix/42-null-pointer-check` for issue #42).

## Pre-Push Checklist

Before pushing your changes or opening a Pull Request, please make sure you have completed the following:

1. **Build the Solution:** Run `dotnet build` (or use your IDE’s build command) to ensure all projects compile without errors or warnings.
2. **Run Tests:** Execute `dotnet test` to run all unit tests. All tests should pass. If you added new functionality, consider adding corresponding tests.
3. **Code Format & Style:** Ensure your code follows the project’s style guidelines. We use .editorconfig and analyzers (StyleCop, Roslynator) to enforce style. You can automatically format your code by running:  
   ```bash
   dotnet format
(This will catch things like spacing, naming, etc.) The CI pipeline will reject code with formatting issues or compiler warnings, so it’s best to fix them before pushing.
4. Static Analysis: Pay attention to any warnings or suggestions from analyzers (e.g., IDE suggestions, Roslynator). Treat warnings as errors – our build treats warnings as errors to maintain code quality.
5. Commit Message: Use clear and descriptive commit messages. Start with a short summary in imperative mood (e.g., “Add X”, “Fix Y”, “Refactor Z”). If the commit addresses a GitHub issue, include a reference (e.g., “Fix #42 - Correct null check in AppointmentService”).
Writing and Running Tests
We love contributions that include unit tests. Our test project uses XUnit and an in-memory SQLite database for any tests involving EF Core. Here are some guidelines for writing tests:
Use In-Memory SQLite: For consistency, use SQLite’s in-memory mode for tests that require a database. This ensures tests are fast and isolated. You can set this up using UseSqlite("DataSource=:memory:") in the DbContextOptions. An example pattern:
csharp
Copy code
var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlite("DataSource=:memory:")
                .Options;
using var db = new ApplicationDbContext(options);
await db.Database.OpenConnectionAsync();
await db.Database.EnsureCreatedAsync();
// ... perform test actions, possibly using db or a context factory ...
In our tests, we often use PooledDbContextFactory<ApplicationDbContext> provided by EF Core to create contexts on the fly
GitHub
GitHub
. The key is opening the connection and calling EnsureCreatedAsync() to materialize the schema in memory.
Arrange-Act-Assert: Follow the AAA pattern to keep tests readable. Set up your data and context (Arrange), perform the operation (Act), and then assert the expected outcome (Assert).
Avoid External Dependencies: Tests should not depend on external services or files. The in-memory database and in-memory data structures should be sufficient for most scenarios. If you need to test file generation (e.g., PDF output), consider capturing the output to a byte array or stream (as done in PdfRendererTests) rather than writing to disk.
Test Naming: Use descriptive test method names that convey intent. For example, SearchAsync_ReturnsEmpty_WhenDatabaseIsEmpty() clearly states what the test is verifying
GitHub
GitHub
.
Run dotnet test to execute all tests. Make sure new tests pass on the CI as well.
Making Changes in Infrastructure/Services
Please do not modify code under PhysicallyFitPT.Infrastructure/Services for feature changes unless it’s related to a bug fix or documentation. That folder contains core service logic (PatientService, AppointmentService, etc.), and active feature development might be in progress. If you need to extend a service, consider creating a new one or discuss with the team via an issue or PR comment. When updating documentation or scripts, ensure that all explanations are clear to other developers. We aim for a self-explanatory codebase.
Pull Request Guidelines
Reference any relevant issue numbers in your PR description.
Provide a summary of the changes you made and why. If your PR is a work-in-progress or exploratory, mark it as a draft.
If adding or changing UI, include screenshots of the before/after when possible.
Smaller PRs are easier to review. If your contribution is large, try to break it into smaller logical commits or multiple PRs.
Be receptive to feedback. We may ask for modifications or clarifications. This is part of the review process to maintain quality and consistency.
By following these guidelines, you help us maintain a high-quality project and make the review process smoother. We appreciate your effort in contributing to PFPT!
