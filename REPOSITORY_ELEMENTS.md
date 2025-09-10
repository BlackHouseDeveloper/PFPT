# PFPT Repository Elements Documentation

## Repository Structure Overview

This document provides a comprehensive listing of all elements in the PFPT (Physically Fit PT) repository, organized by category and purpose.

## Root Level Files

### Configuration Files
- **`PhysicallyFitPT.sln`** - Visual Studio solution file containing all projects
- **`global.json`** - .NET SDK version pinning (targets .NET 8.0.x)
- **`Directory.Build.props`** - MSBuild properties shared across all projects
- **`Directory.Packages.props`** - Centralized NuGet package version management
- **`.editorconfig`** - Code style and formatting rules
- **`.gitignore`** - Git exclusion patterns for build artifacts and temporary files

### Development Tools
- **`PFPT-Foundry.sh`** - Main development environment setup script
  - Automates project scaffolding and dependency installation
  - Handles EF Core migrations and database seeding
  - Enforces .NET 8 target framework consistency
  - Includes safety checks and environment validation
- **`.config/dotnet-tools.json`** - Local .NET CLI tools manifest

### Documentation
- **`README.md`** - Primary project documentation and getting started guide
- **`ARCHITECTURE.md`** - Detailed system architecture and design patterns
- **`DEVELOPMENT.md`** - Comprehensive development workflow guide
- **`REPOSITORY_ELEMENTS.md`** - This file, documenting all repository contents
- **`RUNTIME_ERRORS_ANALYSIS.md`** - Analysis of runtime issues and fixes
- **`RUNTIME_ERROR_FIXES_SUMMARY.md`** - Summary of runtime error resolutions

### Build Artifacts & Logs
- **`build-output.txt`** - Build process output log
- **`PFPT-Foundry-log.txt`** - Setup script execution log
- **`pfpt.design.sqlite`** - Design-time SQLite database for EF Core tooling

### Shared Code
- **`GlobalUsings.cs`** - Global using statements for C#

## Project Structure

### 1. PhysicallyFitPT/ (MAUI Blazor Application)
**Purpose:** Multi-platform application (Android, iOS, macOS)

#### Configuration
- **`PhysicallyFitPT.csproj`** - Project file with multi-target frameworks
- **`MauiProgram.cs`** - Application startup and dependency injection
- **`App.xaml`** + **`App.xaml.cs`** - Application lifecycle management
- **`MainPage.xaml`** + **`MainPage.xaml.cs`** - Main page hosting Blazor WebView

#### Platform-Specific Code
- **`Platforms/`** - Platform-specific implementations
  - **`Android/`** - Android-specific code and assets
  - **`iOS/`** - iOS-specific code and assets  
  - **`MacCatalyst/`** - macOS-specific code and assets
  - **`Windows/`** - Windows-specific code (if applicable)

#### UI Components
- **`Components/`** - Blazor components and pages
  - **`Layout/`** - Application layout components
    - Layout templates and navigation
  - **`Pages/`** - Application pages and views
    - Patient management screens
    - Appointment scheduling
    - Clinical documentation forms
  - **`Routes.razor`** - Client-side routing configuration
  - **`_Imports.razor`** - Global Razor imports

#### Resources
- **`Resources/`** - Application resources
  - **`Fonts/`** - Typography assets
  - **`Images/`** - Application imagery and icons
  - **`AppIcon/`** - Application icon variants
  - **`Splash/`** - Splash screen assets

#### Web Assets  
- **`wwwroot/`** - Static web assets served by Blazor
  - CSS stylesheets
  - JavaScript files
  - Static images and resources

#### Shared Components
- **`Shared/`** - Reusable Blazor components
- **`docs/`** - Component-specific documentation

### 2. PhysicallyFitPT.Domain/ (Domain Layer)
**Purpose:** Core business entities and domain logic

#### Domain Entities
- **`Patient.cs`** - Core patient entity with demographics
- **`Appointment.cs`** - Appointment scheduling and tracking
- **`NoteAndSections.cs`** - Clinical documentation structures
- **`Goal.cs`** - Treatment goals and patient outcomes
- **`Codes.cs`** - Medical coding entities (CPT, ICD-10)
- **`Questionnaires.cs`** - Patient assessment and survey forms
- **`Messaging.cs`** - Communication and notification entities
- **`Common.cs`** - Shared value objects and enumerations

#### Configuration
- **`PhysicallyFitPT.Domain.csproj`** - Class library project file
- **`packages.lock.json`** - NuGet package dependency lock file

### 3. PhysicallyFitPT.Infrastructure/ (Infrastructure Layer)
**Purpose:** Data access, external services, and cross-cutting concerns

#### Data Access
- **`Data/`** - Entity Framework Core implementation
  - **`ApplicationDbContext.cs`** - Main EF Core database context
  - **`DesignTimeDbContextFactory.cs`** - Design-time factory for EF tools
  - **`Migrations/`** - EF Core database migration files

#### Services
- **`Services/`** - Application service implementations
  - **`PatientService.cs`** - Patient management operations
  - **`AppointmentService.cs`** - Appointment scheduling logic
  - **`NoteBuilderService.cs`** - Clinical note creation and formatting
  - **`QuestionnaireService.cs`** - Assessment form management
  - **`AutoMessagingService.cs`** - Automated communication workflows
  - **`PdfRenderer.cs`** - PDF document generation using QuestPDF
  - **`BaseService.cs`** - Common service functionality and patterns

#### Service Contracts
- **`Services/Interfaces/`** - Service interface definitions
  - **`IPatientService.cs`** - Patient service contract
  - **`IAppointmentService.cs`** - Appointment service contract
  - **`INoteBuilderService.cs`** - Note building service contract
  - **`IQuestionnaireService.cs`** - Questionnaire service contract
  - **`IAutoMessagingService.cs`** - Messaging service contract
  - **`IPdfRenderer.cs`** - PDF generation service contract

#### Utilities
- **`SafeJsonHelper.cs`** - JSON serialization utilities
- **`Mappers/`** - Object mapping utilities

#### Configuration
- **`PhysicallyFitPT.Infrastructure.csproj`** - Infrastructure project file
- **`packages.lock.json`** - Dependency lock file

### 4. PhysicallyFitPT.Shared/ (Shared Libraries)
**Purpose:** DTOs, utilities, and cross-cutting domain services

#### Configuration
- **`PhysicallyFitPT.Shared.csproj`** - Shared library project file

#### Typical Contents
- Data Transfer Objects (DTOs)
- Validation logic and rules
- Clinical calculation utilities
- Shared constants and enumerations
- Cross-cutting domain services

### 5. PhysicallyFitPT.Tests/ (Test Suite)
**Purpose:** Comprehensive testing coverage using xUnit

#### Test Categories
- **`SmokeTests.cs`** - Basic application startup validation
- **`PatientServiceTests.cs`** - Patient management functionality tests
- **`AppointmentServiceTests.cs`** - Appointment scheduling tests  
- **`NoteBuilderServiceTests.cs`** - Clinical documentation tests
- **`AutoMessagingServiceTests.cs`** - Communication workflow tests
- **`RuntimeErrorTests.cs`** - Error handling and resilience validation

#### Configuration
- **`PhysicallyFitPT.Tests.csproj`** - Test project configuration
- **`bin/`** + **`obj/`** - Build output directories

### 6. PhysicallyFitPT.Seeder/ (Database Seeder)
**Purpose:** Development database population and sample data creation

#### Core Files
- **`Program.cs`** - Console application entry point
- **`PhysicallyFitPT.Seeder.csproj`** - Console project configuration

#### Functionality
- Creates sample patients and appointments
- Populates medical reference data (CPT codes, questionnaires)
- Idempotent operations (safe to run multiple times)
- Respects existing data (prevents duplicates)

### 7. PhysicallyFitPT.Web/ (Blazor WebAssembly)
**Purpose:** Browser-based client application

#### Configuration
- **`PhysicallyFitPT.Web.csproj`** - WebAssembly project file with AOT settings
- **`Program.cs`** - WebAssembly application startup
- **`packages.lock.json`** - Dependency lock file

#### UI Components
- **`App.razor`** - Root application component
- **`Layout/`** - Web-specific layout components
- **`Pages/`** - Web application pages and routes
- **`_Imports.razor`** - Razor imports for WebAssembly

#### Web Assets
- **`wwwroot/`** - Static web assets and PWA configuration
- **`Properties/`** - Assembly and build properties

## CI/CD Infrastructure

### GitHub Actions
- **`.github/workflows/`** - Automated build and test workflows
  - **`build.yml`** - Main CI pipeline
    - Multi-platform builds (Android, Web)
    - Automated testing with result collection
    - Code formatting enforcement
    - Artifact collection and storage

## Database Elements

### Development Databases
- **`dev.physicallyfitpt.db`** - SQLite development database (created by seeder)
- **`pfpt.design.sqlite`** - EF Core design-time database

### Migration System
- **EF Core Migrations** - Located in `PhysicallyFitPT.Infrastructure/Data/Migrations/`
- **Design-Time Factory** - Enables EF tooling without running full MAUI app

## Development Environment

### Package Management
- **Central Package Management** - Versions controlled in `Directory.Packages.props`
- **Lock Files** - `packages.lock.json` in each project for reproducible builds

### Code Quality Tools
- **StyleCop.Analyzers** - Enforces code style consistency
- **Roslynator.Analyzers** - Provides code quality improvements
- **EditorConfig** - Maintains consistent formatting across IDEs
- **Nullable Reference Types** - Enabled project-wide for better null safety

### Local Development Tools
- **EF Core CLI** - Database migration and management
- **dotnet-format** - Code formatting tool
- **MAUI Workloads** - Platform-specific development tools

## Architecture Patterns

### Clean Architecture Layers
1. **Domain** (PhysicallyFitPT.Domain) - Pure business logic
2. **Application** (Interfaces in Infrastructure) - Use cases and workflows  
3. **Infrastructure** (PhysicallyFitPT.Infrastructure) - External concerns
4. **Presentation** (PhysicallyFitPT, PhysicallyFitPT.Web) - User interfaces

### Cross-Cutting Concerns
- **Audit Trail** - Built into all domain entities
- **Soft Delete** - Logical deletion with IsDeleted flags
- **Dependency Injection** - Constructor-based throughout
- **Error Handling** - Comprehensive logging and resilience patterns

## Target Frameworks & Technologies

### .NET Targets
- **MAUI App:** `net8.0-android;net8.0-ios;net8.0-maccatalyst`
- **All Libraries:** `net8.0`
- **Web App:** `net8.0` with WebAssembly AOT compilation

### Key Technologies
- **.NET 8.0** - Application framework
- **Blazor Hybrid & WebAssembly** - UI technology
- **Entity Framework Core** - Data access layer
- **SQLite** - Embedded database engine
- **QuestPDF** - PDF document generation
- **SkiaSharp** - Cross-platform graphics
- **xUnit** - Testing framework
- **FluentAssertions** - Test assertion library

## Security & Compliance Considerations

### Code Security
- **Input Validation** - Implemented throughout service layers
- **Parameterized Queries** - EF Core provides SQL injection protection
- **HTTPS Enforcement** - Configured for web deployments
- **Audit Logging** - Comprehensive tracking for compliance

### Data Protection
- **Local Encryption** - Prepared for SQLite database encryption
- **HIPAA Considerations** - Architecture supports medical data requirements
- **Data Retention** - Soft delete patterns maintain audit trails

This comprehensive documentation covers all major elements within the PFPT repository, providing developers with a complete understanding of the system architecture, file organization, and development workflows.