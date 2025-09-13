#!/bin/bash
set -e

echo "Restoring packages for core libraries..."
dotnet restore PhysicallyFitPT.Domain/PhysicallyFitPT.Domain.csproj --no-cache
dotnet restore PhysicallyFitPT.Infrastructure/PhysicallyFitPT.Infrastructure.csproj --no-cache
dotnet restore PhysicallyFitPT.Shared/PhysicallyFitPT.Shared.csproj --no-cache
dotnet restore PhysicallyFitPT.Web/PhysicallyFitPT.Web.csproj --no-cache
dotnet restore PhysicallyFitPT.Tests/PhysicallyFitPT.Tests.csproj --no-cache

echo "Checking code formatting..."
dotnet format --verify-no-changes --no-restore

echo "Building core libraries with warnings as errors..."
dotnet build PhysicallyFitPT.Domain/PhysicallyFitPT.Domain.csproj --no-restore --configuration Release -warnaserror
dotnet build PhysicallyFitPT.Infrastructure/PhysicallyFitPT.Infrastructure.csproj --no-restore --configuration Release -warnaserror
dotnet build PhysicallyFitPT.Shared/PhysicallyFitPT.Shared.csproj --no-restore --configuration Release -warnaserror
dotnet build PhysicallyFitPT.Web/PhysicallyFitPT.Web.csproj --no-restore --configuration Release -warnaserror
dotnet build PhysicallyFitPT.Tests/PhysicallyFitPT.Tests.csproj --no-restore --configuration Release -warnaserror

echo "Running tests..."
dotnet test PhysicallyFitPT.Tests/PhysicallyFitPT.Tests.csproj --no-build --configuration Release

echo "✅ Code formatting, build, and tests successful!"