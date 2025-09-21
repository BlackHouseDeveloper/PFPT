# PFPT Build Guide

Comprehensive build instructions for all platforms and deployment scenarios.

## Prerequisites

### Development Environment
- **.NET 8.0 SDK** or later
- **Platform-specific tools** (see Platform Requirements below)
- **Git** for source control
- **PFPT-Foundry.sh** setup script (run once)

### Platform Requirements

#### Windows
- **Windows 10 version 1809+** or **Windows 11**
- **Visual Studio 2022** with .NET MAUI workload
- **Windows SDK** (latest version)

#### macOS  
- **macOS 10.15+** (Catalina or later)
- **Xcode 14+** (from App Store)
- **.NET MAUI workloads**: `dotnet workload install maui`
- **Command Line Tools**: `xcode-select --install`

#### Linux
- **Ubuntu 20.04+** or equivalent distribution
- **.NET 8.0 SDK** installed via package manager
- **Development packages**: `build-essential`, `libgtk-3-dev`

## Quick Build Commands

### Development Builds

```bash
# Clean and restore (recommended first step)
./pfpt-cleanbuild.sh

# Build all projects (fastest)
dotnet build

# Build specific project
dotnet build PhysicallyFitPT/PhysicallyFitPT.csproj
dotnet build PhysicallyFitPT.Web/PhysicallyFitPT.Web.csproj
```

### Platform-Specific Builds

#### Web Application (Browser)
```bash
# Development build
dotnet build PhysicallyFitPT.Web/PhysicallyFitPT.Web.csproj -c Debug -f net8.0

# Production build
dotnet build PhysicallyFitPT.Web/PhysicallyFitPT.Web.csproj -c Release -f net8.0

# Publish for deployment
dotnet publish PhysicallyFitPT.Web -c Release -o ./publish-web
```

#### MAUI Applications

**macOS (Mac Catalyst)**
```bash
# Development build
dotnet build PhysicallyFitPT/PhysicallyFitPT.csproj -f net8.0-maccatalyst -c Debug

# Release build  
dotnet publish PhysicallyFitPT/PhysicallyFitPT.csproj -f net8.0-maccatalyst -c Release
```

**Android**
```bash
# Debug build (for emulator/testing)
dotnet build PhysicallyFitPT/PhysicallyFitPT.csproj -f net8.0-android -c Debug

# Release build (for distribution)
dotnet publish PhysicallyFitPT/PhysicallyFitPT.csproj -f net8.0-android -c Release
```

**iOS**
```bash
# Debug build (simulator)
dotnet build PhysicallyFitPT/PhysicallyFitPT.csproj -f net8.0-ios -c Debug

# Release build (device/App Store)
dotnet publish PhysicallyFitPT/PhysicallyFitPT.csproj -f net8.0-ios -c Release
```

## Advanced Build Scenarios

### Multi-Target Builds
```bash
# Build all platforms (where supported)
dotnet build PhysicallyFitPT/PhysicallyFitPT.csproj

# Build multiple specific targets
dotnet build PhysicallyFitPT/PhysicallyFitPT.csproj -f net8.0-maccatalyst
dotnet build PhysicallyFitPT/PhysicallyFitPT.csproj -f net8.0-android
```

### CI/CD Builds
```bash
# Restore with locked package versions
dotnet restore --locked-mode

# Build with warnings as errors
dotnet build --configuration Release --verbosity normal -warnaserror

# Run tests with coverage
dotnet test --collect:"XPlat Code Coverage" --logger trx
```

### Performance Builds
```bash
# Parallel builds (faster on multi-core systems)
dotnet build -m

# Optimized release builds
dotnet build -c Release -p:DebugType=None -p:DebugSymbols=false
```

## Database Operations

### Entity Framework Commands

**Design-Time Context**
```bash
# Set provider for design-time operations
export EF_PROVIDER=sqlite

# Verify DbContext configuration
dotnet ef dbcontext info -p PhysicallyFitPT.Infrastructure -s PhysicallyFitPT

# List migrations
dotnet ef migrations list -p PhysicallyFitPT.Infrastructure -s PhysicallyFitPT
```

**Migration Management**
```bash
# Create new migration
dotnet ef migrations add MigrationName -p PhysicallyFitPT.Infrastructure -s PhysicallyFitPT

# Update database to latest migration
dotnet ef database update -p PhysicallyFitPT.Infrastructure -s PhysicallyFitPT

# Generate SQL script
dotnet ef migrations script -p PhysicallyFitPT.Infrastructure -s PhysicallyFitPT -o migration.sql
```

## Build Configuration

### Architecture-Specific Dependencies

#### Web Application (net8.0)
- **Stateless design**: No EF Core or SQLite dependencies
- **In-memory data**: Uses browser storage and HTTP APIs
- **No Infrastructure reference**: Maintains clean separation

#### Mobile/Desktop Applications (net8.0-android/ios/maccatalyst)
- **EF Core + SQLite**: For local data storage and offline capabilities
- **Infrastructure integration**: Full access to services and database
- **Platform-specific APIs**: Native functionality per platform

### Build Properties

Key MSBuild properties for customization:

```xml
<!-- In project files or Directory.Build.props -->
<PropertyGroup>
  <TargetFrameworks>net8.0-android;net8.0-ios;net8.0-maccatalyst</TargetFrameworks>
  <RootNamespace>PhysicallyFitPT</RootNamespace>
  <UseMaui>true</UseMaui>
  <MauiVersion>8.0.0</MauiVersion>
  <TreatWarningsAsErrors>true</TreatWarningsAsErrors>
</PropertyGroup>
```

## Troubleshooting Builds

### Common Issues

**MAUI Workload Missing**
```bash
# Install required workloads
dotnet workload install maui

# Update existing workloads
dotnet workload update
```

**Android SDK Issues**
```bash
# Verify Android SDK path
echo $ANDROID_HOME

# Install missing components via Android Studio SDK Manager
```

**iOS Code Signing**
```bash
# Open project in Xcode to configure signing
open PhysicallyFitPT/Platforms/iOS/PhysicallyFitPT.iOS.csproj
```

**Permission Errors**
```bash
# Fix file permissions (never use sudo with dotnet)
sudo chown -R $USER:$USER ~/.dotnet ~/.nuget

# Clear NuGet cache if needed
dotnet nuget locals all --clear
```

### Build Performance

**Optimize Build Times**
```bash
# Use local NuGet cache
export NUGET_PACKAGES=~/.nuget/packages

# Enable parallel builds
export DOTNET_CLI_TELEMETRY_OPTOUT=1
dotnet build -m

# Skip unnecessary restores
dotnet build --no-restore
```

**Clean Builds**
```bash
# Full clean (removes all build artifacts)
./pfpt-cleanbuild.sh

# Manual clean
dotnet clean
rm -rf */bin */obj
```

## Deployment Preparation

### Web Deployment
```bash
# Publish for static hosting
dotnet publish PhysicallyFitPT.Web -c Release -o ./dist

# Optimize for CDN deployment
dotnet publish PhysicallyFitPT.Web -c Release \
  -p:PublishProfile=DefaultContainer \
  -p:BlazorWebAssemblyLoadAllGlobalizationData=false
```

### Mobile App Distribution
```bash
# Android APK for testing
dotnet publish PhysicallyFitPT -f net8.0-android -c Release

# iOS for TestFlight/App Store (requires macOS + Xcode)
dotnet publish PhysicallyFitPT -f net8.0-ios -c Release \
  -p:ArchiveOnBuild=true -p:RuntimeIdentifier=ios-arm64
```

For detailed deployment instructions, see the main `README.md` and `DEVELOPMENT.md` files.