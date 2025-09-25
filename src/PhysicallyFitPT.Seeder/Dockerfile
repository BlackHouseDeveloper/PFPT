# Multi-stage Dockerfile for PhysicallyFitPT.Seeder
# Supports both migration and seeding operations in containerized environments

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project files
COPY ["src/PhysicallyFitPT.Core/PhysicallyFitPT.Core.csproj", "src/PhysicallyFitPT.Core/"]
COPY ["src/PhysicallyFitPT.Infrastructure/PhysicallyFitPT.Infrastructure.csproj", "src/PhysicallyFitPT.Infrastructure/"]
COPY ["src/PhysicallyFitPT.Seeder/PhysicallyFitPT.Seeder.csproj", "src/PhysicallyFitPT.Seeder/"]
COPY ["Directory.Packages.props", "./"]
COPY ["Directory.Build.props", "./"]

# Restore dependencies
RUN dotnet restore "src/PhysicallyFitPT.Seeder/PhysicallyFitPT.Seeder.csproj"

# Copy all source code
COPY src/ src/

# Build and publish the seeder application
RUN dotnet publish "src/PhysicallyFitPT.Seeder/PhysicallyFitPT.Seeder.csproj" \
    -c Release \
    -o /app/publish \
    --no-restore

# Runtime stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS runtime
WORKDIR /app

# Install dotnet-ef tool for migrations
RUN dotnet tool install --global dotnet-ef --version 8.0.8
ENV PATH="${PATH}:/root/.dotnet/tools"

# Copy published application
COPY --from=build /app/publish .

# Copy data files (if they exist)
COPY --from=build /src/src/PhysicallyFitPT.Seeder/Data/ ./Data/

# Set environment variables
ENV ASPNETCORE_ENVIRONMENT=Production
ENV PFP_LOGLEVEL=Information

# Create volume for SQLite database
VOLUME ["/data"]

# Default entry point - can be overridden
ENTRYPOINT ["dotnet", "PhysicallyFitPT.Seeder.dll"]

# Default command - run seeding for Production environment
CMD ["seed", "--env", "Production"]

# Example usage:
# docker build -t pfpt-seeder .
# docker run -v ./data:/data -e PFP_DB_PATH=/data/pfpt.db pfpt-seeder
# docker run -v ./data:/data -e PFP_DB_PATH=/data/pfpt.db pfpt-seeder migrate
# docker run -v ./data:/data -e PFP_DB_PATH=/data/pfpt.db pfpt-seeder seed --env Development
# docker run -v ./data:/data -e PFP_DB_PATH=/data/pfpt.db pfpt-seeder verify
# docker run -v ./data:/data -e PFP_DB_PATH=/data/pfpt.db pfpt-seeder dump --out /data/export