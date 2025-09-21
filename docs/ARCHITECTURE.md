# PFPT Architecture Documentation

## Overview

**Physically Fit PT (PFPT)** is a cross-platform clinician documentation application designed for physical therapy practice. It follows Clean Architecture principles and is built with .NET MAUI Blazor for mobile/desktop and Blazor WebAssembly for web deployment.

## Solution Structure

```
PFPT/
├── PhysicallyFitPT/                    # .NET MAUI Blazor App (Multi-platform)
├── PhysicallyFitPT.Domain/             # Domain Entities (Clean Architecture Core)
├── PhysicallyFitPT.Infrastructure/     # Data Access, EF Core, Services
├── PhysicallyFitPT.Shared/             # Shared DTOs and Clinical Libraries
├── PhysicallyFitPT.Tests/              # Unit and Integration Tests (xUnit)
├── PhysicallyFitPT.Seeder/             # Database Seeding Console App
├── PhysicallyFitPT.Web/                # Blazor WebAssembly Client
├── PFPT-Foundry.sh                     # Development Environment Setup Script
└── PhysicallyFitPT.sln                 # Solution File
```

## Projects Detail

### 1. PhysicallyFitPT (MAUI Blazor App)

**Target Frameworks:** `net8.0-android;net8.0-ios;net8.0-maccatalyst`

**Purpose:** Multi-platform application providing native mobile and desktop experiences.

**Key Components:**
- `App.xaml` - Application entry point
- `MainPage.xaml` - Main page hosting Blazor WebView
- `MauiProgram.cs` - Dependency injection and service configuration
- `Components/` - Blazor components for UI
  - `Layout/` - Application layout components
  - `Pages/` - Application pages and views
  - `Routes.razor` - Client-side routing configuration

**Platform Support:**
- **Android** - Native Android application
- **iOS** - Native iOS application  
- **macOS (Mac Catalyst)** - Native macOS desktop application

### 2. PhysicallyFitPT.Domain (Domain Layer)

**Target Framework:** `net8.0`

**Purpose:** Contains pure domain entities following Domain-Driven Design principles. No external dependencies or EF Core attributes.

**Key Entities:**
- `Patient.cs` - Core patient entity with demographics and audit fields
- `Appointment.cs` - Patient appointment scheduling and tracking
- `NoteAndSections.cs` - Clinical documentation and progress notes
- `Goal.cs` - Patient treatment goals and outcomes
- `Codes.cs` - Medical billing codes (CPT, ICD-10)
- `Questionnaires.cs` - Patient assessment forms and surveys
- `Messaging.cs` - Communication and notification entities
- `Common.cs` - Shared value objects and enumerations

**Design Principles:**
- Pure POCOs (Plain Old CLR Objects)
- No infrastructure dependencies
- Rich domain behavior
- Immutable where appropriate
- Comprehensive audit trail support

### 3. PhysicallyFitPT.Infrastructure (Infrastructure Layer)

**Target Framework:** `net8.0`

**Purpose:** Implements data access patterns, external service integrations, and cross-cutting concerns.

**Key Components:**

#### Data Access (`Data/`)
- `ApplicationDbContext.cs` - EF Core database context with entity configurations
- `DesignTimeDbContextFactory.cs` - Design-time factory for EF Core tools
- `Migrations/` - Entity Framework database migrations

#### Services (`Services/`)
- `PatientService.cs` - Patient management operations
- `AppointmentService.cs` - Appointment scheduling and management
- `NoteBuilderService.cs` - Clinical note creation and formatting
- `QuestionnaireService.cs` - Assessment form management
- `AutoMessagingService.cs` - Automated communication workflows
- `PdfRenderer.cs` - PDF report generation using QuestPDF
- `BaseService.cs` - Common service functionality

#### Interfaces (`Services/Interfaces/`)
- Service contracts defining application capabilities
- Enables dependency injection and testability

**Dependencies:**
- **Entity Framework Core** - Data access and ORM
- **SQLite** - Embedded database engine
- **QuestPDF** - PDF document generation
- **SkiaSharp** - Cross-platform 2D graphics

### 4. PhysicallyFitPT.Shared (Shared Libraries)

**Target Framework:** `net8.0`

**Purpose:** Contains DTOs, shared utilities, and clinical domain knowledge that can be used across projects.

**Typical Contents:**
- Data Transfer Objects (DTOs)
- Validation logic
- Clinical calculation utilities
- Shared constants and enumerations
- Cross-cutting domain services

### 5. PhysicallyFitPT.Tests (Test Suite)

**Target Framework:** `net8.0`

**Purpose:** Comprehensive test coverage using xUnit testing framework.

**Test Categories:**
- `SmokeTests.cs` - Basic application startup and configuration tests
- `PatientServiceTests.cs` - Patient management functionality tests
- `AppointmentServiceTests.cs` - Appointment scheduling tests
- `NoteBuilderServiceTests.cs` - Clinical documentation tests
- `AutoMessagingServiceTests.cs` - Communication workflow tests
- `RuntimeErrorTests.cs` - Error handling and resilience tests

**Testing Dependencies:**
- **xUnit** - Testing framework
- **FluentAssertions** - Assertion library for readable tests
- **Entity Framework Core** - In-memory database for testing

### 6. PhysicallyFitPT.Seeder (Database Seeder)

**Target Framework:** `net8.0`

**Purpose:** Console application for populating development databases with sample data.

**Features:**
- Creates sample patients and appointments
- Populates reference data (CPT codes, questionnaires)
- Idempotent operations (safe to run multiple times)
- Respects existing data (no duplicates)

**Usage:**
```bash
./PFPT-Foundry.sh --seed
# OR
export PFP_DB_PATH="$(pwd)/dev.physicallyfitpt.db"
dotnet run --project PhysicallyFitPT.Seeder
```

### 7. PhysicallyFitPT.Web (Blazor WebAssembly)

**Target Framework:** `net8.0`

**Purpose:** Browser-based client application using Blazor WebAssembly.

**Key Features:**
- Client-side C# execution in the browser
- Progressive Web App (PWA) capabilities
- Offline-first design with local storage
- Shared component library with MAUI app

**Architecture:**
- `Program.cs` - Application startup and DI configuration
- `App.razor` - Root application component
- `Layout/` - Web-specific layout components
- `Pages/` - Web application pages
- `wwwroot/` - Static web assets

**Database Strategy:**
- Uses Entity Framework Core InMemory provider for client-side data
- HTTP client integration for server communication
- Local browser storage for offline capabilities

## Technology Stack

### Core Technologies
- **.NET 8.0** - Application framework
- **C# 12** - Programming language
- **Blazor** - UI framework (Hybrid and WebAssembly)
- **.NET MAUI** - Multi-platform application framework

### Data & Storage
- **Entity Framework Core** - Object-relational mapping
- **SQLite** - Embedded database (development and mobile)
- **InMemory Provider** - Browser-based data storage

### Document Generation
- **QuestPDF** - Modern PDF document generation
- **SkiaSharp** - Cross-platform graphics and imaging

### Testing & Quality
- **xUnit** - Unit testing framework
- **FluentAssertions** - Fluent testing assertions
- **StyleCop** - Code style analysis
- **Roslynator** - Code quality analysis

### Development Tools
- **EF Core Tools** - Database migration management
- **MAUI Workloads** - Platform-specific tooling
- **Source Link** - Source code debugging support

## Architecture Patterns

### Clean Architecture
The solution follows Clean Architecture principles:

1. **Domain Layer** (PhysicallyFitPT.Domain)
   - Business entities and rules
   - No external dependencies

2. **Application Layer** (Interfaces in Infrastructure)
   - Use cases and application services
   - Orchestrates domain objects

3. **Infrastructure Layer** (PhysicallyFitPT.Infrastructure)
   - External concerns (database, file system, web services)
   - Implements application layer interfaces

4. **Presentation Layer** (PhysicallyFitPT, PhysicallyFitPT.Web)
   - User interfaces and user interaction logic

### Dependency Injection
- Constructor-based dependency injection throughout
- Service interfaces defined in Infrastructure
- Registered in platform-specific startup code

### Cross-Cutting Concerns
- Audit trails built into domain entities
- Soft delete patterns for data retention
- Comprehensive error handling and logging
- Consistent validation patterns

## Database Design

### Entity Relationships
```
Patient (1) ←→ (N) Appointment
Appointment (1) ←→ (N) Note
Patient (1) ←→ (N) Goal
Patient (1) ←→ (N) QuestionnaireResponse
```

### Key Features
- **Audit Trail**: All entities include Created/Updated timestamps and user tracking
- **Soft Delete**: Logical deletion with IsDeleted flag
- **Flexible Schema**: CSV fields for extensible data (medications, comorbidities)
- **Reference Data**: CPT codes, ICD-10 codes, questionnaire templates

## Development Environment

### Prerequisites
- **macOS** (primary development platform)
- **.NET 8.0 SDK**
- **Xcode** (for iOS/macOS targets)
- **.NET MAUI Workloads**

### Setup Script
The `PFPT-Foundry.sh` script provides automated environment setup:

```bash
# Basic setup
./PFPT-Foundry.sh

# With database migration
./PFPT-Foundry.sh --create-migration

# With sample data
./PFPT-Foundry.sh --seed

# Help information
./PFPT-Foundry.sh --help
```

### Build Targets
- **MAUI App**: `dotnet build -f net8.0-maccatalyst`
- **Web App**: `dotnet build PhysicallyFitPT.Web`
- **All Projects**: `dotnet build`
- **Tests**: `dotnet test`

## Deployment Strategies

### MAUI Application
- **Development**: Mac Catalyst for desktop testing
- **Production**: 
  - iOS App Store distribution
  - Android Play Store distribution
  - macOS App Store or direct distribution

### Web Application  
- **Development**: Local Kestrel server
- **Production**: 
  - Static hosting (GitHub Pages, Netlify, etc.)
  - Azure Static Web Apps
  - Any web server supporting SPAs

## Security Considerations

### Data Protection
- Local SQLite encryption for sensitive data
- HTTPS enforcement for web deployments
- Input validation and sanitization
- SQL injection prevention through EF Core

### Authentication & Authorization
- Framework prepared for identity provider integration
- Role-based access control structure
- Audit logging for compliance

## Performance Optimizations

### Database
- Efficient EF Core queries with proper indexing
- Lazy loading disabled for predictable performance
- Connection pooling in multi-user scenarios

### UI Responsiveness  
- Async/await patterns throughout
- Progressive loading for large datasets
- Efficient Blazor component rendering

### Mobile Performance
- Native UI performance through .NET MAUI
- Efficient memory management
- Platform-specific optimizations

## Extensibility Points

### Adding New Features
1. Define domain entities in Domain project
2. Add service interfaces in Infrastructure/Services/Interfaces
3. Implement services in Infrastructure/Services
4. Create UI components in Components folder
5. Add comprehensive tests

### Integration Points
- Database provider swappable via EF Core
- PDF rendering customizable via QuestPDF
- Authentication pluggable via .NET Identity
- External APIs integrable via HTTP clients

## Automation Architecture

PFPT includes a comprehensive automation framework designed to streamline clinical workflows and reduce manual tasks.

### Automation Services Layer

#### AutoMessagingService
```
┌─────────────────────────────────────┐
│ AutoMessagingService                │
├─────────────────────────────────────┤
│ + SendAppointmentReminderAsync()    │
│ + SendFollowUpMessageAsync()        │
│ + ScheduleAutomatedMessagesAsync()  │
│ + ProcessMessageQueueAsync()        │
└─────────────────────────────────────┘
          │
          ▼
┌─────────────────────────────────────┐
│ Message Templates & Scheduling      │
├─────────────────────────────────────┤
│ • Appointment reminders             │
│ • Post-treatment follow-ups         │
│ • Assessment notifications          │
│ • Customizable content templates    │
└─────────────────────────────────────┘
```

#### PDF Export Automation
```
┌─────────────────────────────────────┐
│ PdfRenderer (QuestPDF)              │
├─────────────────────────────────────┤
│ + RenderPatientSummary()            │
│ + RenderTreatmentPlan()             │
│ + RenderProgressReport()            │
│ + BatchGenerateReports()            │
└─────────────────────────────────────┘
          │
          ▼
┌─────────────────────────────────────┐
│ Report Generation Pipeline          │
├─────────────────────────────────────┤
│ • Data aggregation from services    │
│ • Template-based formatting         │
│ • Batch processing capabilities     │
│ • Integration with billing systems  │
└─────────────────────────────────────┘
```

#### Assessment Management
```
┌─────────────────────────────────────┐
│ QuestionnaireService                │
├─────────────────────────────────────┤
│ + CreateResponseAsync()             │
│ + CalculateScoreAsync()             │
│ + GetOutcomeTrendsAsync()           │
│ + ScheduleFollowUpAsync()           │
└─────────────────────────────────────┘
          │
          ▼
┌─────────────────────────────────────┐
│ Standardized Assessments            │
├─────────────────────────────────────┤
│ • TUG (Timed Up and Go)             │
│ • BBS (Berg Balance Scale)          │
│ • NPRS (Numeric Pain Rating)        │
│ • Custom clinic assessments         │
└─────────────────────────────────────┘
```

### Automation Data Flow

```
Patient Check-in
      │
      ▼
┌─────────────────┐    ┌──────────────────┐    ┌─────────────────┐
│ Assessment      │    │ Documentation    │    │ Communication   │
│ Scheduling      │───▶│ Auto-Population  │───▶│ Automated       │
│                 │    │                  │    │ Messaging       │
└─────────────────┘    └──────────────────┘    └─────────────────┘
      │                         │                       │
      ▼                         ▼                       ▼
┌─────────────────┐    ┌──────────────────┐    ┌─────────────────┐
│ Outcome         │    │ Report           │    │ Follow-up       │
│ Tracking        │    │ Generation       │    │ Coordination    │
│                 │    │                  │    │                 │
└─────────────────┘    └──────────────────┘    └─────────────────┘
```

### Integration Points

- **Clinical Workflow**: Seamless integration with patient management
- **Billing Systems**: Automated CPT code integration and report generation
- **External APIs**: HTTP client support for third-party integrations
- **Notification Systems**: Configurable messaging templates and schedules

## Compliance & Standards

### Medical Standards
- Designed for HIPAA compliance considerations
- Audit trail capabilities for clinical documentation
- Data retention and deletion policies support

### Code Quality
- Nullable reference types enabled
- Treat warnings as errors
- Consistent code style via EditorConfig
- Automated static analysis via StyleCop and Roslynator