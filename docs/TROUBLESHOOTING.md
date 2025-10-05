# PFPT Troubleshooting Guide

Comprehensive troubleshooting guide for common development, build, and deployment issues.

## Quick Diagnostic Commands

```bash
# Check environment status
dotnet --version
dotnet workload list
./PFPT-Foundry.sh --help

# Verify project structure
ls -la *.sln
dotnet sln list

# Clean build test
./pfpt-cleanbuild.sh
```

## Development Environment Issues

### .NET SDK Problems

**Issue**: `.NET SDK not found` or `dotnet command not found`
```bash
# Verify installation
dotnet --version

# Install .NET 8.0 SDK
# macOS: Download from https://dotnet.microsoft.com/download/dotnet/8.0
# Linux: sudo apt install dotnet-sdk-8.0
# Windows: Download installer from Microsoft

# Verify PATH configuration
echo $PATH | grep dotnet
```

**Issue**: `Wrong .NET version detected`
```bash
# Check global.json requirements
cat global.json

# List installed SDKs
dotnet --list-sdks

# Install required version
dotnet workload install maui
```

### MAUI Workload Issues

**Issue**: `MAUI workload not installed` or `NETSDK1147 error`
```bash
# Install MAUI workloads
dotnet workload install maui

# Update existing workloads
dotnet workload update

# Repair corrupted workloads
dotnet workload repair

# Clean workload cache (if corrupted)
dotnet workload clean
```

**Issue**: `Platform-specific builds fail`
```bash
# Verify platform requirements
# Android: Ensure ANDROID_HOME is set
echo $ANDROID_HOME

# iOS: Verify Xcode installation
xcode-select -p

# macOS: Check command line tools
xcode-select --install
```

## Database and Entity Framework Issues

### Database Connection Problems

**Issue**: `Database not found` or `Cannot open database file`
```bash
# Check database file exists
ls -la *.db

# Verify environment variable
echo $PFP_DB_PATH

# Create database if missing
./PFPT-Foundry.sh --create-migration

# Set database path
export PFP_DB_PATH="$(pwd)/dev.physicallyfitpt.db"
```

**Issue**: `EF Core design-time failures`
```bash
# Set EF provider environment variable
export EF_PROVIDER=sqlite

# Verify DbContext configuration
dotnet ef dbcontext info -p PhysicallyFitPT.Infrastructure -s PhysicallyFitPT

# Check EF tools installation
dotnet tool list --global | grep dotnet-ef

# Install/update EF tools
dotnet tool install --global dotnet-ef
dotnet tool update --global dotnet-ef
```

### Migration Issues

**Issue**: `Migration fails to apply`
```bash
# Check migration status
dotnet ef migrations list -p PhysicallyFitPT.Infrastructure -s PhysicallyFitPT

# Reset database (development only)
rm dev.physicallyfitpt.db
./PFPT-Foundry.sh --create-migration

# Manual migration application
dotnet ef database update -p PhysicallyFitPT.Infrastructure -s PhysicallyFitPT
```

**Issue**: `Design-time factory not found`
```bash
# Verify DesignTimeDbContextFactory exists
ls PhysicallyFitPT.Infrastructure/Data/DesignTimeDbContextFactory.cs

# Check factory implementation
grep -n "IDesignTimeDbContextFactory" PhysicallyFitPT.Infrastructure/Data/*.cs
```

## Build and Compilation Issues

### Architecture-Specific Build Problems

**Issue**: `Web project has EF/SQLite dependencies`
- **CI Error**: "EF/SQLite leaked into Web"
- **Solution**: 
  ```bash
  # Remove EF-related PackageReference from PhysicallyFitPT.Web.csproj
  # Ensure no ProjectReference to Infrastructure
  dotnet remove PhysicallyFitPT.Web package Microsoft.EntityFrameworkCore.Sqlite
  dotnet remove PhysicallyFitPT.Web reference PhysicallyFitPT.Infrastructure
  ```

**Issue**: `Web project contains DbContext usage`
- **CI Error**: "DbContext usage found in Web"
- **Solution**:
  ```bash
  # Remove DbContext registrations from Program.cs
  # Remove: AddDbContext, UseSqlite(), UseInMemoryDatabase()
  # Use HTTP clients for data access instead
  ```

### Platform Build Failures

**Issue**: `Android build fails`
```bash
# Verify Android SDK
echo $ANDROID_HOME
ls $ANDROID_HOME/platforms

# Install required Android components
# Open Android Studio > SDK Manager
# Install: Android SDK Platform-Tools, Build-Tools

# Check Java version (requires Java 17)
java -version

# Clear Android build cache
rm -rf PhysicallyFitPT/obj/Debug/net8.0-android
```

**Issue**: `iOS build fails`
```bash
# Verify Xcode installation
xcode-select -p

# Accept Xcode license
sudo xcodebuild -license accept

# Check iOS simulators
xcrun simctl list devices

# Open project in Xcode for code signing
open PhysicallyFitPT.sln
```

**Issue**: `Mac Catalyst build fails`
```bash
# Verify macOS version compatibility
sw_vers

# Check Xcode version
xcodebuild -version

# Clean Mac-specific build artifacts
rm -rf PhysicallyFitPT/obj/Debug/net8.0-maccatalyst
```

## Runtime and Application Issues

### Application Startup Problems

**Issue**: `Application fails to start`
```bash
# Check for startup exceptions in logs
# Enable detailed logging in appsettings.json:

# Verify database seeding
./PFPT-Foundry.sh --seed

# Check service registrations in MauiProgram.cs
```

**Issue**: `Services not found or injection fails`
```bash
# Verify service registration in MauiProgram.cs
grep -n "AddScoped\|AddTransient\|AddSingleton" PhysicallyFitPT/MauiProgram.cs

# Check interface implementations exist
ls PhysicallyFitPT.Infrastructure/Services/Interfaces/
ls PhysicallyFitPT.Infrastructure/Services/
```

### PDF Export Issues

**Issue**: `PDF generation fails`
```bash
# Verify QuestPDF package reference
dotnet list PhysicallyFitPT.Infrastructure package | grep QuestPDF

# Check PdfRenderer service registration
grep -n "IPdfRenderer" PhysicallyFitPT/MauiProgram.cs

# Test PDF generation
# Create simple test in unit tests
```

**Issue**: `PDF rendering errors`
```bash
# Check for missing fonts or resources
# Verify SkiaSharp dependencies
dotnet list PhysicallyFitPT.Infrastructure package | grep SkiaSharp

# Enable QuestPDF debugging
# Add to service configuration:
QuestPDF.Settings.License = LicenseType.Community;
```

### Automation Service Issues

**Issue**: `AutoMessagingService not working`
```bash
# Verify service registration
grep -n "IAutoMessagingService" PhysicallyFitPT/MauiProgram.cs

# Check message templates in database
# Run seeder to populate templates
./PFPT-Foundry.sh --seed

# Verify HTTP client configuration for external messaging
```

**Issue**: `Assessment automation fails`
```bash
# Check QuestionnaireService registration
grep -n "IQuestionnaireService" PhysicallyFitPT/MauiProgram.cs

# Verify assessment templates seeded
# Check database for questionnaire data
sqlite3 dev.physicallyfitpt.db ".tables"
```

## Script and Automation Issues

### PFPT-Foundry.sh Problems

**Issue**: `Script permission denied`
```bash
# Fix script permissions
chmod +x PFPT-Foundry.sh
chmod +x pfpt-cleanbuild.sh

# Never use sudo with scripts
# Fix ownership if needed:
sudo chown -R $USER:$USER .
```

**Issue**: `Script fails with Homebrew .NET`
```bash
# Script auto-detects Homebrew and configures paths
# Manual configuration if needed:
export DOTNET_CLI_HOME="$HOME/.dotnet"
export NUGET_PACKAGES="$HOME/.nuget/packages"
```

**Issue**: `Cross-platform script compatibility`
```bash
# Verify bash version (requires 4.0+)
bash --version

# For Windows, use WSL or Git Bash
# For older macOS, install bash via Homebrew:
brew install bash
```

## Performance and Resource Issues

### Build Performance

**Issue**: `Slow build times`
```bash
# Enable parallel builds
dotnet build -m

# Use local NuGet cache
export NUGET_PACKAGES=~/.nuget/packages

# Clear package cache if corrupted
dotnet nuget locals all --clear
```

**Issue**: `High memory usage during build`
```bash
# Limit parallel build processes
dotnet build -m:2

# Build projects individually
dotnet build PhysicallyFitPT.Core
dotnet build PhysicallyFitPT.Infrastructure
dotnet build PhysicallyFitPT
```

### Application Performance

**Issue**: `Slow database operations`
```bash
# Check database file size and location
ls -lh *.db

# Verify efficient queries in services
# Enable EF Core logging to identify slow queries

# Consider database cleanup/optimization
sqlite3 dev.physicallyfitpt.db "VACUUM;"
```

## Getting Additional Help

### Diagnostic Information Collection

When reporting issues, include:

```bash
# System information
uname -a
dotnet --info
./PFPT-Foundry.sh --help 2>&1 | head -20

# Project status
dotnet sln list
git status
ls -la *.db

# Build logs
./pfpt-cleanbuild.sh > build-diagnostic.log 2>&1
```

### Resources

- **GitHub Issues**: [PFPT Repository Issues](https://github.com/BlackHouseDeveloper/PFPT/issues)
- **Documentation**: See `DEVELOPMENT.md` for detailed setup instructions
- **Build Guide**: See `docs/BUILD.md` for comprehensive build information
- **.NET MAUI Docs**: [Microsoft .NET MAUI Documentation](https://docs.microsoft.com/dotnet/maui/)
- **Entity Framework**: [EF Core Documentation](https://docs.microsoft.com/ef/core/)

### Community Support

- **Stack Overflow**: Tag questions with `dotnet-maui`, `entity-framework-core`, `blazor`
- **.NET Community**: [.NET Discord](https://discord.gg/dotnet) and [Reddit r/dotnet](https://reddit.com/r/dotnet)
- **MAUI Community**: [.NET MAUI GitHub Discussions](https://github.com/dotnet/maui/discussions)
- When calling `/api/v1/diagnostics/info`, a `PFPT-Diagnostics: true` response header indicates diagnostics are intentionally enabled; absence of the header (or a 404/401/403) means diagnostics remain secured.
