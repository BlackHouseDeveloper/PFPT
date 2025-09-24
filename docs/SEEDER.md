# PFPT Database Seeder

The PFPT Database Seeder is a production-ready versioned seeding framework that provides reliable, repeatable initialization of reference data and sample datasets for the PhysicallyFitPT application.

## Features

### Core Capabilities
- **Versioned Seed Tasks**: Ordered execution through numeric task IDs (001.cpt.codes, 002.icd10.codes, etc.)
- **Hash-Based Idempotence**: SHA256 content descriptors prevent unnecessary re-runs
- **Environment Filtering**: Tasks can restrict to specific environments (Development/Staging/Production)
- **Transaction Management**: Each task executes in isolation with rollback on failure
- **Concurrency Control**: Database locking prevents multiple seeder instances
- **Change Detection**: Replay tasks whose content has changed with `--replay-changed`

### Command Line Interface
Built with System.CommandLine providing comprehensive CLI functionality:

```bash
# List all tasks with status
dotnet run --project PhysicallyFitPT.Seeder -- seed --list

# Run seeding for specific environment
dotnet run --project PhysicallyFitPT.Seeder -- seed --env Production

# Verify required baseline tasks are applied
dotnet run --project PhysicallyFitPT.Seeder -- verify

# Export reference data as JSON
dotnet run --project PhysicallyFitPT.Seeder -- dump --out ./exported-data

# Run only EF migrations
dotnet run --project PhysicallyFitPT.Seeder -- migrate

# Show help
dotnet run --project PhysicallyFitPT.Seeder -- --help
```

## Seed Tasks

The seeder includes four built-in seed tasks:

### 001.cpt.codes - CPT Codes
- **Source**: `Data/cpt.json` with fallback to inline constants
- **Purpose**: Seeds Current Procedural Terminology codes for physical therapy
- **Environment**: All environments
- **Data**: Therapeutic exercise (97110), Manual therapy (97140), Therapeutic activities (97530)

### 002.icd10.codes - ICD-10 Codes  
- **Source**: `Data/icd10.json` with fallback to inline constants
- **Purpose**: Seeds ICD-10 diagnosis codes for common conditions
- **Environment**: All environments
- **Data**: Knee pain codes (M25.561, M25.562), Low back pain (M54.50)

### 003.patients.sample - Sample Patients
- **Source**: `Data/patients.dev.json` with fallback to inline constants
- **Purpose**: Seeds sample patient records for development/testing
- **Environment**: Development only (unless `--force` is used)
- **Data**: Jane Doe (A1001), John Smith (A1002), Sam Sample (A1003)

### 004.composite.reference - Reference Summary
- **Source**: Logic-based (no external file)
- **Purpose**: Maintains summary counts of CPT and ICD-10 codes in ReferenceSummary table
- **Environment**: All environments
- **Data**: Composite record with current counts and timestamp

## Usage Examples

### Basic Usage
```bash
# Standard seeding (Development environment, includes sample patients)
dotnet run --project PhysicallyFitPT.Seeder -- seed

# Production seeding (excludes sample patients)
dotnet run --project PhysicallyFitPT.Seeder -- seed --env Production

# Force seeding with all tasks regardless of environment
dotnet run --project PhysicallyFitPT.Seeder -- seed --env Production --force
```

### Advanced Usage
```bash
# Dry run (preview what would happen)
dotnet run --project PhysicallyFitPT.Seeder -- seed --dry-run

# Re-run tasks whose content has changed
dotnet run --project PhysicallyFitPT.Seeder -- seed --replay-changed

# Run specific task by ID
dotnet run --project PhysicallyFitPT.Seeder -- seed --task 001.cpt.codes

# Run specific task by name pattern
dotnet run --project PhysicallyFitPT.Seeder -- seed --task cpt
```

## Configuration

### Environment Variables
- `PFP_DB_PATH`: Override database file path
- `PFP_ENV`: Set default environment (Development/Staging/Production)
- `PFP_LOGLEVEL`: Set logging level (Debug/Information/Warning/Error)

### Configuration Files
- `appsettings.json`: Base configuration
- `appsettings.Seeder.json`: Seeder-specific settings
- `appsettings.{Environment}.json`: Environment-specific overrides

### Example Configuration
```json
{
  "Seeder": {
    "Environment": "Development",
    "LogLevel": "Information",
    "UseMigrations": true
  }
}
```

## Database Schema

### New Tables Added
The seeder adds three new tables to track its operations:

**SeedHistory**
- `TaskId` (PK): Unique task identifier
- `Name`: Human-readable task name
- `Hash`: SHA256 content descriptor
- `AppliedAtUtc`: Timestamp when task was applied

**SeederLocks**
- `Id` (PK): Lock identifier (always 1)
- `AcquiredAtUtc`: When lock was acquired
- `ProcessInfo`: Information about the process holding the lock

**ReferenceSummaries**
- `Id` (PK): Summary identifier (always 1)
- `CptCount`: Current count of CPT codes
- `Icd10Count`: Current count of ICD-10 codes
- `UpdatedAtUtc`: When summary was last updated

## Adding New Seed Tasks

To add a new seed task:

1. **Create Task Class**: Implement `ISeedTask` interface
   ```csharp
   public class MyCustomSeedTask : BaseSeedTask
   {
       public override string Id => "005.my.custom.task";
       public override string Name => "My Custom Task";
       
       public override async Task ExecuteAsync(CancellationToken cancellationToken = default)
       {
           // Implementation here
       }
       
       public override Task<string> ComputeContentDescriptorAsync()
       {
           // Hash calculation here
       }
   }
   ```

2. **Register Task**: Add to `SeedTaskRegistry.AddSeedingServices()`
   ```csharp
   services.AddScoped<ISeedTask, MyCustomSeedTask>();
   ```

3. **Create Data File** (optional): Add `Data/my-data.json`

4. **Set Environment Restrictions** (optional): Override `AllowedEnvironments` property

5. **Test**: Run seeder with `--list` to verify task appears, then apply

## Docker Support

### Dockerfile
A multi-stage Dockerfile is provided for containerized deployment:

```bash
# Build seeder image
docker build -f src/PhysicallyFitPT.Seeder/Dockerfile -t pfpt-seeder .

# Run migration
docker run -v ./data:/data -e PFP_DB_PATH=/data/pfpt.db pfpt-seeder migrate

# Run seeding
docker run -v ./data:/data -e PFP_DB_PATH=/data/pfpt.db pfpt-seeder seed --env Production

# Verify seeding
docker run -v ./data:/data -e PFP_DB_PATH=/data/pfpt.db pfpt-seeder verify
```

### Docker Compose
Use the provided `docker-compose.seeder.yml` for easier container management:

```bash
# Run migrations
docker-compose -f docker-compose.seeder.yml --profile migrate up

# Run production seeding
docker-compose -f docker-compose.seeder.yml --profile seed up

# Run development seeding (with sample patients)
docker-compose -f docker-compose.seeder.yml --profile seed-dev up
```

## Testing

### Integration Tests
Comprehensive integration tests verify:
- First run seeds all applicable tasks
- Second run is idempotent (no changes)
- Environment filtering works correctly
- Force flag overrides restrictions
- Hash change detection and replay
- Task listing and status reporting

### Running Tests
```bash
# Run all seeder tests
dotnet test tests/PhysicallyFitPT.Seeder.Tests/

# Run with coverage
dotnet test tests/PhysicallyFitPT.Seeder.Tests/ --collect:"XPlat Code Coverage"
```

## CI/CD Integration

### GitHub Actions
The `.github/workflows/seeder.yml` workflow provides automated verification:
- Runs integration tests
- Verifies CLI functionality
- Tests Docker container builds
- Validates idempotency and environment filtering

### Pipeline Integration
For CI/CD pipelines:

```yaml
# Example pipeline step
- name: Run Database Seeding
  run: |
    dotnet run --project PhysicallyFitPT.Seeder -- migrate
    dotnet run --project PhysicallyFitPT.Seeder -- seed --env ${{ env.ENVIRONMENT }}
    dotnet run --project PhysicallyFitPT.Seeder -- verify
```

## Exit Codes

The seeder uses standard exit codes:
- `0`: Success
- `1`: General failure (task execution, validation error)
- `11`: Lock acquisition failure (another instance running)

## Troubleshooting

### Common Issues

**Lock Acquisition Failure**
```bash
# Check for orphaned locks
dotnet run --project PhysicallyFitPT.Seeder -- seed --list
# If needed, manually clear locks in database
```

**Migration Failures**
```bash
# Check migration status
dotnet ef migrations list --project PhysicallyFitPT.Infrastructure

# Force database creation (development only)
dotnet run --project PhysicallyFitPT.Seeder -- migrate --ensure-created
```

**Hash Mismatches**
```bash
# Force replay of all tasks
dotnet run --project PhysicallyFitPT.Seeder -- seed --replay-changed --force

# Check specific task status
dotnet run --project PhysicallyFitPT.Seeder -- seed --list --task cpt
```

### Logging
Enable debug logging for detailed information:
```bash
export PFP_LOGLEVEL=Debug
dotnet run --project PhysicallyFitPT.Seeder -- seed --list
```

## Architecture Notes

### Design Principles
- **Idempotent**: Safe to run multiple times
- **Deterministic**: Same inputs produce same outputs
- **Transactional**: Each task executes atomically
- **Extensible**: Easy to add new seed tasks
- **Environment-Aware**: Respects deployment contexts
- **Observable**: Rich logging and status reporting

### Dependencies
- .NET 8.0
- Entity Framework Core 8.0
- System.CommandLine 2.0
- Microsoft.Extensions.Hosting 8.0

The seeder is designed to be a critical component of the PFPT deployment pipeline, ensuring reliable and consistent database initialization across all environments.