# PFPT Enterprise Architecture Documentation

## Overview

**Physically Fit PT (PFPT)** is an enterprise-grade, cross-platform healthcare documentation platform designed specifically for physical therapy practices. PFPT is built with .NET MAUI Blazor for mobile and desktop, and Blazor WebAssembly for web deployment. The platform follows Clean Architecture principles, with healthcare compliance, enterprise automation, and clinical workflow optimization at its core.

## Enterprise Solution Structure

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

## Enterprise Architecture Layers

### 1. Presentation Layer

#### PhysicallyFitPT.Maui (Native Applications)
**Target Frameworks:** `net8.0-android;net8.0-ios;net8.0-maccatalyst`

**Purpose:** Multi-platform native applications providing optimal clinical user experience.

**Key Components:**
- `App.xaml` - Application lifecycle and dependency injection
- `MainPage.xaml` - Main page hosting Blazor WebView
- `MauiProgram.cs` - Platform-specific service configuration
- `Components/` - Healthcare-focused Blazor components
  - `Layout/` - Clinical application layouts
  - `Pages/` - Patient management and clinical workflow pages
  - `Clinical/` - Assessment tools and outcome measures
  - `Reports/` - PDF generation and report viewing

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

**Purpose:** Implements healthcare-compliant data access, clinical service integrations, and enterprise infrastructure concerns.

**Key Components:**

#### Data Access (`Data/`)
- `ApplicationDbContext.cs` - EF Core context with healthcare entity configurations
- `DesignTimeDbContextFactory.cs` - Design-time factory for clinical database tools
- `Migrations/` - Healthcare-compliant database schema migrations
- `Configurations/` - Entity type configurations with HIPAA considerations
- `Interceptors/` - Audit trail and data privacy interceptors

#### Clinical Services (`Services/`)
- `PatientService.cs` - HIPAA-compliant patient management operations
- `ClinicalAssessmentService.cs` - Standardized assessment administration and scoring
- `AppointmentService.cs` - Clinical appointment scheduling with outcome tracking
- `NoteBuilderService.cs` - Structured clinical note creation with templates
- `QuestionnaireService.cs` - Patient-reported outcome measure management
- `ClinicalDecisionService.cs` - Evidence-based clinical decision support
- `AutoMessagingService.cs` - HIPAA-compliant automated communication workflows
- `PdfRenderer.cs` - Clinical report generation with accessibility compliance
- `AuditService.cs` - Comprehensive healthcare audit trail management

#### Enterprise Infrastructure (`Infrastructure/`)
- `HealthcareComplianceService.cs` - HIPAA validation and compliance checking
- `ClinicalTerminologyService.cs` - Medical terminology validation and standardization
- `AccessibilityService.cs` - WCAG 2.1 compliance validation for clinical interfaces
- `DataEncryptionService.cs` - Healthcare data encryption and key management
- `IntegrationService.cs` - External healthcare system integration (HL7, FHIR)

### 4. Application Services Layer (PhysicallyFitPT.Api)

**Target Framework:** `net8.0`

**Purpose:** Web API for cloud services, external integrations, and administrative functions.

**Key Features:**
- RESTful API for clinical data synchronization
- HL7 FHIR integration for healthcare interoperability
- Administrative dashboard for clinical practice management
- External EHR system integration endpoints
- Healthcare analytics and reporting services

### 5. Shared Libraries (PhysicallyFitPT.Shared)

**Target Framework:** `net8.0`

**Purpose:** Cross-platform clinical libraries and standardized healthcare components.

**Clinical Libraries:**
- `ClinicalAssessments/` - Standardized assessment tools and scoring algorithms
- `HealthcareStandards/` - Medical coding systems (ICD-10, CPT, SNOMED)
- `ClinicalTemplates/` - Evidence-based documentation templates
- `OutcomeMeasures/` - Patient-reported and performance-based outcome measures
- `ComplianceValidators/` - HIPAA and accessibility compliance validation
- `ClinicalDecisionSupport/` - Evidence-based care pathway algorithms

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

**Purpose:** Contains DTOs, shared utilities, and clinical core knowledge that can be used across projects.

**Typical Contents:**
- Data Transfer Objects (DTOs)
- Validation logic
- Clinical calculation utilities
- Shared constants and enumerations
- Cross-cutting core services

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

1. **Domain Layer** (PhysicallyFitPT.Core)
   - Business entities and rules
   - No external dependencies

2. **Application Layer** (Interfaces in Infrastructure)
   - Use cases and application services
   - Orchestrates core objects

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
- Audit trails built into core entities
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
1. Define core entities in Domain project
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
### 6. Enterprise AI/ML Layer (PhysicallyFitPT.AI)

**Target Framework:** `net8.0`

**Purpose:** AI-powered clinical intelligence and decision support systems.

**Key Features:**
- Natural language processing for clinical documentation
- Predictive analytics for patient outcomes
- Clinical decision support algorithms
- Automated assessment scoring and interpretation
- Evidence-based treatment recommendation engines

## Enterprise MCP Framework Architecture

PFPT includes a comprehensive Model Context Protocol (MCP) framework with 14 production-ready workflows designed specifically for healthcare software development.

### MCP Framework Structure

```
.github/workflows/
├── Development Automation
│   ├── mcp-auto-format-pr.yml           # Automatic code formatting on PRs
│   ├── mcp-daily-formatting.yml         # Daily maintenance formatting
│   └── mcp-copilot-setup-validation.yml # Development environment validation
├── Healthcare-Specific Automation
│   ├── mcp-accessibility-compliance.yml # WCAG 2.1 compliance testing
│   ├── mcp-pdf-diagnostics.yml         # Clinical report validation
│   └── mcp-database-diagnostics.yml    # HIPAA-compliant data management
├── Enterprise Operations
│   ├── mcp-release-notes-generation.yml # Clinical impact release notes
│   ├── mcp-triage.yml                  # Intelligent issue categorization
│   └── mcp-documentation-automation.yml # Clinical documentation generation
├── Quality Assurance
│   ├── mcp-error-reproduction.yml       # Comprehensive debugging automation
│   ├── mcp-localization-workflow.yml   # Multi-language clinical terminology
│   └── mcp-ci.yml                      # Multi-platform CI/CD with compliance
└── Copilot Integration
    ├── copilot-auto-fix.yml            # Automated code issue resolution
    └── copilot-mention.yml             # Intelligent development assistance
```

### Healthcare Compliance Integration

Each MCP workflow includes:
- **HIPAA Compliance Validation** - Ensures patient data protection standards
- **Clinical Workflow Assessment** - Evaluates impact on healthcare processes
- **Accessibility Testing** - WCAG 2.1 AA/AAA compliance validation
- **Medical Terminology Validation** - Standardized healthcare coding verification
- **Audit Trail Generation** - Comprehensive change tracking for healthcare compliance

### Enterprise Automation Capabilities

#### 1. Development Environment Automation
- **Automated Setup Validation** - Comprehensive development environment verification
- **Dependency Management** - .NET MAUI workloads, healthcare libraries, PDF tools
- **Code Quality Enforcement** - StyleCop, Roslynator, accessibility analyzers
- **Multi-platform Build Validation** - iOS, Android, macOS, Web compatibility

#### 2. Healthcare-Specific Quality Assurance
- **Clinical Report Validation** - PDF accessibility, clinical template compliance
- **Database Schema Compliance** - HIPAA audit trail, healthcare data standards
- **Accessibility Testing** - Screen reader compatibility, keyboard navigation
- **Clinical Terminology Validation** - ICD-10, CPT, SNOMED code verification

#### 3. Enterprise Release Management
- **Clinical Impact Assessment** - Automated evaluation of healthcare workflow changes
- **Release Notes Generation** - Healthcare-focused change categorization
- **Regulatory Compliance Tracking** - FDA, HIPAA, accessibility compliance monitoring
- **Clinical Documentation Updates** - Automated clinical guide generation

### MCP Workflow Categories

#### Development Automation (4 workflows)
Automate code quality, formatting, and development environment management with healthcare-specific standards.

#### Healthcare Compliance (3 workflows)  
Ensure HIPAA compliance, accessibility standards, and clinical documentation requirements are met.

#### Enterprise Operations (3 workflows)
Manage releases, documentation, and issue triage with clinical workflow considerations.

#### Quality Assurance (4 workflows)
Comprehensive testing, debugging, and validation with healthcare-specific requirements.

## Technology Stack & Dependencies

### Core Technologies
- **.NET 8.0** - Primary development platform
- **.NET MAUI** - Cross-platform native application framework
- **Blazor WebAssembly** - Web application framework
- **Entity Framework Core** - Object-relational mapping
- **SQLite** - Embedded database with healthcare data encryption
- **QuestPDF** - Healthcare-compliant PDF generation

### Healthcare-Specific Libraries
- **SNOMED CT Integration** - Standardized clinical terminology
- **ICD-10 & CPT Coding** - Medical billing and diagnosis codes
- **HL7 FHIR** - Healthcare interoperability standards
- **Clinical Assessment Libraries** - Standardized outcome measures

### Enterprise Development Tools
- **xUnit** - Comprehensive test framework
- **StyleCop & Roslynator** - Code quality analyzers
- **GitHub Actions** - Enterprise CI/CD pipeline
- **Docker** - Containerization for deployment
- **Accessibility Testing Tools** - WCAG 2.1 compliance validation

## Security & Compliance Architecture

### Healthcare Data Protection
- **HIPAA Compliance** - Comprehensive patient data protection
- **Data Encryption** - At-rest and in-transit encryption for all patient data
- **Audit Trails** - Complete healthcare compliance audit logging
- **Access Controls** - Role-based access with clinical privilege management

### Accessibility Compliance
- **WCAG 2.1 AA/AAA** - Full accessibility compliance for healthcare users
- **Screen Reader Compatibility** - Comprehensive assistive technology support
- **Keyboard Navigation** - Complete keyboard accessibility for clinical workflows
- **High Contrast Support** - Visual accessibility for clinical environments

### Quality Assurance
- **Multi-platform Testing** - Comprehensive testing across all supported platforms
- **Healthcare Workflow Validation** - Clinical process testing and validation
- **Performance Monitoring** - Clinical application performance optimization
- **Regulatory Compliance Testing** - FDA, HIPAA, accessibility compliance validation

This enterprise architecture ensures PFPT maintains the highest standards of healthcare compliance, clinical usability, and technical excellence while providing comprehensive automation and quality assurance through the integrated MCP framework.

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