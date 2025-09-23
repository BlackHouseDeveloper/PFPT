# PFPT Development Guide

## Quick Start

### Prerequisites Checklist
- [ ] **macOS** (Apple Silicon or Intel) - Primary development platform
- [ ] **.NET 8.0 SDK** - [Download here](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- [ ] **Xcode** (latest) - Required for iOS/Mac targets
- [ ] **Git** - Version control

### Initial Setup

1. **Clone the repository:**
   ```bash
   git clone https://github.com/BlackHouseDeveloper/PFPT.git
   cd PFPT
   ```

2. **Run the setup script:**
   ```bash
   ./PFPT-Foundry.sh
   ```

3. **Create database and seed data:**
   ```bash
   ./PFPT-Foundry.sh --create-migration --seed
   ```

4. **Set environment variable (optional):**
   ```bash
   export PFP_DB_PATH="$(pwd)/dev.physicallyfitpt.db"
   ```

## PFPT-Foundry.sh Script

The `PFPT-Foundry.sh` script is the central development tool for PFPT. It automates project setup, dependency management, and development environment configuration.

### Usage

```bash
./PFPT-Foundry.sh [OPTIONS]

Options:
  --create-migration   Create initial EF Core migration and update database
  --seed              Seed the dev SQLite database with sample data  
  -h, --help          Show help information
```

### What the Script Does

#### Safety & Environment Checks
- **Root User Prevention**: Refuses to run as root to prevent permission issues
- **.NET SDK Validation**: Ensures .NET 8.0 SDK is installed and available
- **Homebrew Detection**: Automatically configures user-local caches for Homebrew-managed .NET

#### Project Scaffolding
- Creates solution file if it doesn't exist
- Generates all project structures with correct templates:
  - MAUI Blazor app for multi-platform support
  - Class libraries for clean architecture layers
  - xUnit test project for comprehensive testing
  - Console seeder app for database population
  - Blazor WebAssembly app for web deployment

#### Target Framework Normalization
- Enforces .NET 8.0 across all projects
- Configures multi-target frameworks for MAUI (Android, iOS, Mac Catalyst)
- Validates and reports any inconsistent framework versions

#### Package Management
- Installs required NuGet packages automatically
- Sets up EF Core with SQLite provider
- Configures PDF generation libraries (QuestPDF, SkiaSharp)
- Adds testing frameworks and assertion libraries

#### Development Tooling
- Installs EF Core CLI tools locally
- Creates design-time DbContext factory for migrations
- Sets up EditorConfig for consistent formatting
- Configures code analyzers (StyleCop, Roslynator)

#### CI/CD Integration
- Generates GitHub Actions workflow for automated builds
- Configures multi-platform build targets
- Sets up automated testing and artifact collection

## Development Workflow

### Daily Development

1. **Start development session:**
   ```bash
   # Ensure environment is up to date
   ./PFPT-Foundry.sh
   
   # Pull latest changes
   git pull origin main
   ```

2. **Run the application:**
   ```bash
   # MAUI app (Mac Catalyst)
   dotnet build -t:Run -f net8.0-maccatalyst PhysicallyFitPT/PhysicallyFitPT.csproj
   
   # Web app (browser)
   dotnet run --project PhysicallyFitPT.Web/PhysicallyFitPT.Web.csproj
   ```

3. **Run tests continuously:**
   ```bash
   dotnet test --logger "console;verbosity=detailed" --watch
   ```

### Database Development

#### Creating Migrations
```bash
# Automatic via script
./PFPT-Foundry.sh --create-migration

# Manual approach
dotnet ef migrations add MigrationName --project PhysicallyFitPT.Infrastructure --startup-project PhysicallyFitPT
dotnet ef database update --project PhysicallyFitPT.Infrastructure --startup-project PhysicallyFitPT
```

#### Seeding Data
```bash
# Via script (recommended)
./PFPT-Foundry.sh --seed

# Direct seeder execution
export PFP_DB_PATH="$(pwd)/dev.physicallyfitpt.db"
dotnet run --project PhysicallyFitPT.Seeder
```

#### Database Management
```bash
# View current migration status
dotnet ef migrations list --project PhysicallyFitPT.Infrastructure

# Reset database (development only)
rm dev.physicallyfitpt.db
./PFPT-Foundry.sh --create-migration --seed

# Generate SQL scripts
dotnet ef migrations script --project PhysicallyFitPT.Infrastructure --output migration.sql
```

### Code Quality

#### Running Tests
```bash
# All tests
dotnet test

# Specific project
dotnet test PhysicallyFitPT.Tests/PhysicallyFitPT.Tests.csproj

# With coverage
dotnet test --collect:"XPlat Code Coverage"

# Filter by category
dotnet test --filter "Category=Unit"
```

#### Code Formatting
```bash
# Install dotnet-format tool (if not available)
dotnet tool install -g dotnet-format

# Format code
dotnet format

# Check formatting without changes
dotnet format --verify-no-changes
```

#### Static Analysis
The project includes automated analyzers:
- **StyleCop.Analyzers** - Code style consistency
- **Roslynator.Analyzers** - Code quality improvements
- **TreatWarningsAsErrors** - Ensures clean builds

## Automation Features

PFPT includes several automation capabilities to streamline clinical workflows and reduce manual tasks.

### Automated Messaging

The `AutoMessagingService` provides patient communication automation:

```csharp
// Example: Sending automated appointment reminders
public interface IAutoMessagingService
{
    Task SendAppointmentReminderAsync(Guid appointmentId);
    Task SendFollowUpMessageAsync(Guid patientId, string templateType);
    Task ScheduleAutomatedMessagesAsync(Guid patientId, MessageScheduleConfig config);
}
```

#### Key Features
- **Appointment Reminders**: Automatic notifications before scheduled visits
- **Follow-up Messages**: Post-treatment check-ins and care instructions
- **Customizable Templates**: Configurable message content for different scenarios
- **Scheduling System**: Time-based message delivery with patient preferences

#### Configuration
Message templates and schedules are defined in the `PhysicallyFitPT.Shared` project:

```csharp
public class MessageTemplate
{
    public string Type { get; set; } // "reminder", "followup", "assessment"
    public string Subject { get; set; }
    public string Content { get; set; }
    public TimeSpan SendBefore { get; set; } // For appointment reminders
}
```

### PDF Export Automation

The `PdfRenderer` service enables automated report generation:

```csharp
public interface IPdfRenderer
{
    byte[] RenderSimple(string title, string body);
    byte[] RenderPatientSummary(Patient patient, List<Appointment> appointments);
    byte[] RenderTreatmentPlan(Guid patientId, List<Goal> goals);
}
```

#### Automation Capabilities
- **Batch Report Generation**: Export multiple patient reports for billing
- **Scheduled Reports**: Automatic generation of progress summaries
- **Template Customization**: Modify layouts and branding through QuestPDF
- **Data Integration**: Pulls from multiple services for comprehensive reports

#### Usage Examples
```csharp
// Generate patient summary for billing
var patientService = serviceProvider.GetService<IPatientService>();
var pdfRenderer = serviceProvider.GetService<IPdfRenderer>();

var patient = await patientService.GetByIdAsync(patientId);
var appointments = await patientService.GetAppointmentsAsync(patientId);
var summaryPdf = pdfRenderer.RenderPatientSummary(patient, appointments);

// Save or email the PDF
await File.WriteAllBytesAsync($"patient-summary-{patient.MRN}.pdf", summaryPdf);
```

### Assessment Management Automation

The `QuestionnaireService` automates patient assessment workflows:

```csharp
public interface IQuestionnaireService
{
    Task<QuestionnaireResponse> CreateResponseAsync(Guid questionnaireId, Guid patientId);
    Task<AssessmentScore> CalculateScoreAsync(QuestionnaireResponse response);
    Task<List<OutcomeMeasure>> GetOutcomeTrendsAsync(Guid patientId);
}
```

#### Automated Features
- **Smart Scoring**: Automatic calculation of standardized assessment scores (TUG, BBS, NPRS)
- **Outcome Tracking**: Trend analysis and progress monitoring
- **Reminder Systems**: Scheduled follow-up assessments
- **Data Validation**: Real-time validation of assessment responses

#### Supported Assessments
- **Timed Up and Go (TUG)**: Fall risk and mobility assessment
- **Berg Balance Scale (BBS)**: Balance confidence evaluation
- **Numeric Pain Rating Scale (NPRS)**: Pain level tracking
- **Functional assessments**: Custom clinic-specific evaluations

### Workflow Integration

Automation features integrate seamlessly with the clinical workflow:

1. **Patient Check-in**: Automatic assessment scheduling based on treatment protocols
2. **Documentation**: Auto-population of common fields and templates
3. **Billing**: Automated report generation with CPT code integration
4. **Follow-up**: Scheduled messaging based on treatment plans and outcomes

### Configuration and Customization

Automation settings are configurable through:
- **appsettings.json**: Global automation preferences
- **Database Settings**: Per-clinic customization
- **User Preferences**: Individual clinician settings
- **Template Management**: Custom message and report templates

## Project Structure Deep Dive

### Adding New Features

1. **Domain Entity** (if needed):
   ```csharp
   // PhysicallyFitPT.Core/NewEntity.cs
   public class NewEntity
   {
       public Guid Id { get; set; }
       // Domain properties...
       
       // Audit properties
       public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
       public string? CreatedBy { get; set; }
       public DateTimeOffset? UpdatedAt { get; set; }
       public string? UpdatedBy { get; set; }
       public bool IsDeleted { get; set; }
   }
   ```

2. **Service Interface**:
   ```csharp
   // PhysicallyFitPT.Infrastructure/Services/Interfaces/INewService.cs
   public interface INewService
   {
       Task<NewEntity> GetByIdAsync(Guid id);
       Task<NewEntity> CreateAsync(NewEntity entity);
       // Other operations...
   }
   ```

3. **Service Implementation**:
   ```csharp
   // PhysicallyFitPT.Infrastructure/Services/NewService.cs
   public class NewService : BaseService, INewService
   {
       public NewService(ApplicationDbContext context) : base(context) { }
       
       // Implementation...
   }
   ```

4. **UI Components**:
   ```razor
   @* PhysicallyFitPT/Components/Pages/NewFeature.razor *@
   @page "/new-feature"
   @inject INewService NewService
   
   <h1>New Feature</h1>
   <!-- Component implementation -->
   ```

5. **Tests**:
   ```csharp
   // PhysicallyFitPT.Tests/NewServiceTests.cs
   public class NewServiceTests
   {
       [Fact]
       public async Task GetByIdAsync_ReturnsEntity_WhenExists()
       {
           // Arrange, Act, Assert...
       }
   }
   ```

### Database Schema Changes

1. **Update Domain Entity**:
   - Add new properties to core entities
   - Ensure audit trail properties are included

2. **Update DbContext**:
   ```csharp
   // PhysicallyFitPT.Infrastructure/Data/ApplicationDbContext.cs
   public DbSet<NewEntity> NewEntities { get; set; }
   
   protected override void OnModelCreating(ModelBuilder modelBuilder)
   {
       base.OnModelCreating(modelBuilder);
       
       // Configure new entity
       modelBuilder.Entity<NewEntity>(entity =>
       {
           entity.HasKey(e => e.Id);
           entity.Property(e => e.Name).HasMaxLength(200);
           // Additional configuration...
       });
   }
   ```

3. **Create Migration**:
   ```bash
   dotnet ef migrations add AddNewEntity --project PhysicallyFitPT.Infrastructure --startup-project PhysicallyFitPT
   ```

4. **Update Seeder** (if needed):
   ```csharp
   // PhysicallyFitPT.Seeder/Program.cs
   if (!await db.NewEntities.AnyAsync())
   {
       db.NewEntities.AddRange(
           new NewEntity { Name = "Sample 1" },
           new NewEntity { Name = "Sample 2" }
       );
       await db.SaveChangesAsync();
       Console.WriteLine("✓ Seeded NewEntity data");
   }
   ```

## Build & Deployment

### Local Development Builds

```bash
# Full solution build
dotnet build

# Platform-specific MAUI builds
dotnet build PhysicallyFitPT/PhysicallyFitPT.csproj -f net8.0-maccatalyst
dotnet build PhysicallyFitPT/PhysicallyFitPT.csproj -f net8.0-android
dotnet build PhysicallyFitPT/PhysicallyFitPT.csproj -f net8.0-ios

# Web application build
dotnet build PhysicallyFitPT.Web/PhysicallyFitPT.Web.csproj

# Release builds
dotnet build -c Release
```

### Running Applications

#### MAUI Application
```bash
# Mac Catalyst (macOS desktop)
dotnet build -t:Run -f net8.0-maccatalyst PhysicallyFitPT/PhysicallyFitPT.csproj

# Android (requires Android emulator or device)
dotnet build -t:Run -f net8.0-android PhysicallyFitPT/PhysicallyFitPT.csproj

# iOS (requires iOS simulator or device)
dotnet build -t:Run -f net8.0-ios PhysicallyFitPT/PhysicallyFitPT.csproj
```

#### Web Application
```bash
# Development server
dotnet run --project PhysicallyFitPT.Web/PhysicallyFitPT.Web.csproj

# Production build and serve
dotnet publish PhysicallyFitPT.Web -c Release -o ./publish-web
# Serve ./publish-web/wwwroot with any static web server
```

### Continuous Integration

The project includes a GitHub Actions workflow (`.github/workflows/build.yml`) that:

1. **Environment Setup**:
   - Uses macOS runners for MAUI compatibility
   - Installs .NET 8.0 SDK with NuGet caching
   - Installs required MAUI workloads

2. **Build Validation**:
   - Restores NuGet packages
   - Enforces code formatting standards
   - Builds Android and Web targets

3. **Testing**:
   - Runs unit tests with result collection
   - Generates test reports

4. **Artifact Collection**:
   - Uploads build logs and test results
   - Preserves build artifacts for debugging

## Troubleshooting

### Common Issues

#### .NET Workload Problems
```bash
# Reinstall MAUI workloads
dotnet workload update
dotnet workload install maui

# Clean workload cache
dotnet workload clean
```

#### Database Issues
```bash
# Reset development database
rm dev.physicallyfitpt.db pfpt.design.sqlite
./PFPT-Foundry.sh --create-migration --seed

# Check EF Core tools
dotnet tool update dotnet-ef
```

#### Build Errors
```bash
# Clean solution
dotnet clean
rm -rf */bin */obj

# Restore packages
dotnet restore

# Full rebuild
dotnet build
```

#### Permission Issues
```bash
# Fix file permissions (never use sudo with the script)
chmod +x PFPT-Foundry.sh
sudo chown -R $USER:$USER .
```

### Development Environment Validation

Run this checklist to verify your development setup:

```bash
# Check .NET version
dotnet --version  # Should be 8.0.x

# Check MAUI workloads
dotnet workload list  # Should include 'maui'

# Verify project builds
dotnet build PhysicallyFitPT.Core
dotnet build PhysicallyFitPT.Infrastructure  
dotnet build PhysicallyFitPT.Tests

# Run tests
dotnet test PhysicallyFitPT.Tests

# Check database tools
dotnet ef --version  # Should be available

# Verify script permissions
ls -la PFPT-Foundry.sh  # Should show execute permissions
```

## IDE Configuration

### Visual Studio Code
Recommended extensions:
- C# for Visual Studio Code
- .NET MAUI extension
- SQLite Viewer
- GitLens

### Visual Studio (Mac/Windows)
- Ensure .NET MAUI workload is installed
- Configure code style settings to match EditorConfig
- Enable NuGet package restore on build

## Performance Tips

### Development Performance
- Use `dotnet watch` for hot reload during development
- Configure NuGet package caching
- Use local NuGet feeds for frequently used packages
- Enable parallel builds: `dotnet build -m`

### Application Performance
- Profile with dotnet-trace for performance analysis
- Use async/await patterns consistently
- Implement proper EF Core query patterns
- Monitor memory usage during development

## Security Guidelines

### Development Security
- Never commit connection strings or secrets
- Use development certificates for HTTPS
- Validate all user inputs in services
- Follow OWASP guidelines for web security

### Data Protection
- Encrypt sensitive data in local databases
- Implement proper audit logging
- Use parameterized queries (EF Core provides this)
- Follow medical data handling best practices