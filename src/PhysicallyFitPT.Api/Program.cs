#pragma warning disable SA1600 // Elements should be documented
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable SA1633 // The file name must match the first type name
#pragma warning disable SA1009 // Closing parenthesis should be spaced correctly
#pragma warning disable SA1025 // Code should not contain multiple whitespace in a row
#pragma warning disable SA1507 // Code should not contain multiple blank lines in a row
#pragma warning disable SA1028 // Code should not contain trailing whitespace
#pragma warning disable SA1649 // File name should match first type name
#pragma warning disable SA1413 // Use trailing comma in multi-line initializers
#pragma warning disable SA1101 // Prefix local calls with 'this.'
#pragma warning disable SA1110 // Opening parenthesis should be on the line of the previous token
#pragma warning disable SA1111 // Closing parenthesis should be on its own line
#pragma warning disable SA1400 // Access modifier should be declared

using System.IO;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.FeatureManagement;
using Microsoft.Extensions.Options;
using PhysicallyFitPT.Core;
using PhysicallyFitPT.Infrastructure.Data;
using PhysicallyFitPT.Infrastructure.Services;
using PhysicallyFitPT.Infrastructure.Services.Configuration;
using PhysicallyFitPT.Infrastructure.Services.Interfaces;
using PhysicallyFitPT.Shared;
using PhysicallyFitPT.Shared.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMemoryCache();
builder.Services.Configure<AppStatsOptions>(builder.Configuration.GetSection("AppStats"));

// Add Health Checks
builder.Services.AddHealthChecks();

// Add Feature Management
builder.Services.AddFeatureManagement();

// Configure Entity Framework
var configuredConnection = builder.Configuration.GetConnectionString("DefaultConnection");
var environmentDbPath = Environment.GetEnvironmentVariable("PFP_DB_PATH");
var connectionString = !string.IsNullOrWhiteSpace(environmentDbPath)
    ? $"Data Source={environmentDbPath}"
    : configuredConnection ?? "Data Source=pfpt.db";

// Normalize relative SQLite paths to the application content root so the API
// can locate the database regardless of the current working directory.
if (!string.IsNullOrWhiteSpace(connectionString) && connectionString.Contains("Data Source=", StringComparison.OrdinalIgnoreCase))
{
    var sqliteBuilder = new SqliteConnectionStringBuilder(connectionString);
    sqliteBuilder.DataSource = ResolveSqlitePath(sqliteBuilder.DataSource, builder.Environment.ContentRootPath);
    connectionString = sqliteBuilder.ToString();
}

builder.Services.AddDbContextFactory<ApplicationDbContext>(options =>
{
    options.UseSqlite(connectionString);
});

Console.WriteLine($"[PFPT.Api] Using SQLite data source: {new SqliteConnectionStringBuilder(connectionString).DataSource}");

// DI Setup for services
builder.Services.AddScoped<IPatientService, PatientService>();
builder.Services.AddScoped<IAppointmentService, AppointmentService>();
builder.Services.AddScoped<INoteBuilderService, NoteBuilderService>();
builder.Services.AddScoped<IQuestionnaireService, QuestionnaireService>();
builder.Services.AddScoped<IAutoMessagingService, AutoMessagingService>();
builder.Services.AddScoped<IDashboardMetricsService, DashboardMetricsService>();
builder.Services.AddScoped<IAppStatsService, AppStatsService>();
builder.Services.AddScoped<IAppStatsInvalidator>(sp => (IAppStatsInvalidator)sp.GetRequiredService<IAppStatsService>());
builder.Services.AddScoped<PhysicallyFitPT.Infrastructure.Pdf.IPdfRenderer, PhysicallyFitPT.Infrastructure.Pdf.QuestPdfRenderer>();

// Add CORS for development
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors();

// Map Health Check endpoint
app.MapHealthChecks("/health");

// API v1 endpoints with versioning
var v1 = app.MapGroup("/api/v1");

// Application statistics endpoint
v1.MapGet("/stats", async (IAppStatsService appStatsService, CancellationToken cancellationToken) =>
{
    try
    {
        var stats = await appStatsService.GetAppStatsAsync(cancellationToken);
        return Results.Ok(stats);
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message);
    }
})
.WithName("GetAppStats")
.WithOpenApi();

v1.MapGet("/sync/snapshot", async (IAppStatsService appStatsService, IDashboardMetricsService dashboardMetricsService, CancellationToken cancellationToken) =>
{
    try
    {
        var appStats = await appStatsService.GetAppStatsAsync(cancellationToken);
        var dashboardStats = await dashboardMetricsService.GetDashboardStatsAsync(cancellationToken);

        var snapshot = new SyncSnapshotDto
        {
            AppStats = appStats,
            DashboardStats = dashboardStats,
            GeneratedAt = DateTimeOffset.UtcNow,
        };

        return Results.Ok(snapshot);
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message);
    }
})
.WithName("GetSyncSnapshot")
.WithOpenApi();

// Legacy endpoint for backwards compatibility
app.MapGet("/api/stats", async (IAppStatsService appStatsService, CancellationToken cancellationToken) =>
{
    try
    {
        var stats = await appStatsService.GetAppStatsAsync(cancellationToken);
        return Results.Ok(stats);
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message);
    }
})
.WithName("GetAppStatsLegacy")
.WithOpenApi();

// Minimal APIs for patients
app.MapGet("/api/patients/search", async (string? query, int take, IPatientService patientService) =>
{
    try
    {
        var patients = await patientService.SearchAsync(query ?? string.Empty, take > 0 ? take : 50);
        return Results.Ok(patients);
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message);
    }
})
.WithName("SearchPatients")
.WithOpenApi();

// Minimal APIs for appointments
app.MapPost("/api/appointments", async (AppointmentCreateRequest request, IAppointmentService appointmentService) =>
{
    try
    {
        var appointment = await appointmentService.ScheduleAsync(
            request.PatientId,
            request.Start,
            request.End,
            request.VisitType,
            request.Location,
            request.ClinicianName,
            request.ClinicianNpi);
        return Results.Created($"/api/appointments/{appointment.Id}", appointment);
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message);
    }
})
.WithName("CreateAppointment")
.WithOpenApi();

app.MapGet("/api/patients/{patientId}/appointments/upcoming", async (Guid patientId, DateTimeOffset? from, int take, IAppointmentService appointmentService) =>
{
    try
    {
        var appointments = await appointmentService.GetUpcomingByPatientAsync(
            patientId,
            from ?? DateTimeOffset.UtcNow,
            take > 0 ? take : 50);
        return Results.Ok(appointments);
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message);
    }
})
.WithName("GetPatientAppointments")
.WithOpenApi();

app.MapDelete("/api/appointments/{id}", async (Guid id, IAppointmentService appointmentService) =>
{
    try
    {
        var success = await appointmentService.CancelAsync(id);
        return success ? Results.NoContent() : Results.NotFound();
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message);
    }
})
.WithName("CancelAppointment")
.WithOpenApi();

// Minimal APIs for notes
app.MapGet("/api/notes/appointment/{appointmentId}", (Guid appointmentId, INoteBuilderService noteService) =>
{
    try
    {
        // Implementation would depend on note service methods
        return Results.Ok(new { message = "Note endpoint placeholder" });
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message);
    }
})
.WithName("GetNoteByAppointment")
.WithOpenApi();

// PDF export endpoint (behind feature flag)
app.MapGet("/api/notes/export/{id}", async (Guid id, PhysicallyFitPT.Infrastructure.Pdf.IPdfRenderer pdfRenderer, IFeatureManager featureManager, CancellationToken cancellationToken) =>
{
    if (!await featureManager.IsEnabledAsync("PDFExport"))
    {
        return Results.NotFound(new { error = "PDF export feature is not enabled" });
    }

    try
    {
        var pdfBytes = await pdfRenderer.RenderNoteAsync(id, cancellationToken);
        return Results.File(pdfBytes, "application/pdf", $"note-{id}.pdf");
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message);
    }
})
.WithName("ExportNotePdf")
.WithOpenApi();

// Demo PDF endpoint (behind feature flag)
app.MapGet("/api/pdf/demo", async (PhysicallyFitPT.Infrastructure.Pdf.IPdfRenderer pdfRenderer, IFeatureManager featureManager, CancellationToken cancellationToken) =>
{
    if (!await featureManager.IsEnabledAsync("PDFExport"))
    {
        return Results.NotFound(new { error = "PDF export feature is not enabled" });
    }

    try
    {
        var pdfBytes = await pdfRenderer.RenderDemoAsync(cancellationToken);
        return Results.File(pdfBytes, "application/pdf", "pfpt-demo.pdf");
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message);
    }
})
.WithName("DemoPdf")
.WithOpenApi();

// Minimal APIs for questionnaires
app.MapGet("/api/questionnaires",  (IQuestionnaireService questionnaireService) =>
{
    try
    {
        // Implementation would depend on questionnaire service methods
        return Results.Ok(new { message = "Questionnaire endpoint placeholder" });
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message);
    }
})
.WithName("GetQuestionnaires")
.WithOpenApi();

// Dashboard statistics endpoint
app.MapGet("/api/dashboard/stats", async (IDashboardMetricsService dashboardMetricsService, CancellationToken cancellationToken) =>
{
    try
    {
        var stats = await dashboardMetricsService.GetDashboardStatsAsync(cancellationToken);
        return Results.Ok(stats);
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message);
    }
})
.WithName("GetDashboardStats")
.WithOpenApi();

v1.MapGet("/diagnostics/info", (HttpContext httpContext, IOptionsMonitor<AppStatsOptions> appStatsOptions, IConfiguration configuration, ILogger<Program> logger) =>
{
    if (!app.Environment.IsDevelopment())
    {
        if (!(httpContext.User?.Identity?.IsAuthenticated ?? false))
        {
            return Results.Unauthorized();
        }

        var requiredRole = configuration.GetValue<string?>("App:DiagnosticsRequiredRole");
        if (!string.IsNullOrWhiteSpace(requiredRole) && !httpContext.User.IsInRole(requiredRole))
        {
            return Results.Forbid();
        }
    }

    var diagnosticsEnabled = DeveloperModeGuard.Resolve(
        configuration,
        Environment.GetEnvironmentVariable,
        logger,
        app.Environment.IsDevelopment(),
        environmentOverridesSupported: true,
        DeveloperModeGuard.EnvironmentVariableName,
        "DiagnosticsInfoEndpoint");

    if (!diagnosticsEnabled)
    {
        return Results.NotFound();
    }

    httpContext.Response.Headers["PFPT-Diagnostics"] = "true";

    var payload = new DiagnosticsInfoDto(true, appStatsOptions.CurrentValue.CacheTtlSeconds);
    return Results.Ok(payload);
})
.WithName("GetDiagnosticsInfo")
.WithOpenApi();

app.MapGet("/health/info", (IOptionsMonitor<AppStatsOptions> appStatsOptions, IConfiguration configuration, ILogger<Program> logger) =>
{
    var diagnosticsEnabled = DeveloperModeGuard.Resolve(
        configuration,
        Environment.GetEnvironmentVariable,
        logger,
        app.Environment.IsDevelopment(),
        environmentOverridesSupported: true,
        DeveloperModeGuard.EnvironmentVariableName,
        "HealthInfoEndpoint");

var payload = new DiagnosticsInfoDto(diagnosticsEnabled, appStatsOptions.CurrentValue.CacheTtlSeconds);
    return Results.Ok(payload);
})
.WithName("GetHealthInfo")
.WithOpenApi();

static string ResolveSqlitePath(string dataSource, string contentRootPath)
{
    if (string.IsNullOrWhiteSpace(dataSource))
    {
        return dataSource;
    }

    if (Path.IsPathRooted(dataSource))
    {
        return dataSource;
    }

    // First try relative to the content root (e.g. src/PhysicallyFitPT.Api)
    var candidate = Path.GetFullPath(Path.Combine(contentRootPath, dataSource));
    if (File.Exists(candidate))
    {
        return candidate;
    }

    // Walk up the directory tree looking for a matching file (covers solution-root configuration files)
    var current = Directory.GetParent(contentRootPath);
    while (current is not null)
    {
        var alternative = Path.GetFullPath(Path.Combine(current.FullName, dataSource));
        if (File.Exists(alternative))
        {
            return alternative;
        }

        current = current.Parent;
    }

    // If the file does not exist yet, EF will create it at the candidate path under the content root.
    return candidate;
}

app.Run();

// Request DTOs for API endpoints
public record AppointmentCreateRequest(
    Guid PatientId,
    DateTimeOffset Start,
    DateTimeOffset? End,
    VisitType VisitType,
    string? Location = null,
    string? ClinicianName = null,
    string? ClinicianNpi = null);

#pragma warning restore SA1400 // Access modifier should be declared
#pragma warning restore SA1111 // Closing parenthesis should be on its own line
#pragma warning restore SA1110 // Opening parenthesis should be on the line of the previous token
#pragma warning restore SA1101 // Prefix local calls with 'this.'
#pragma warning restore SA1413 // Use trailing comma in multi-line initializers
#pragma warning restore SA1649 // File name should match first type name    
#pragma warning restore SA1028 // Code should not contain trailing whitespace
#pragma warning restore SA1507 // Code should not contain multiple blank lines in a row
#pragma warning restore SA1025 // Code should not contain multiple whitespace in a row
#pragma warning restore SA1009 // Closing parenthesis should be spaced correctly
#pragma warning restore SA1633 // The file name must match the first type name
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
#pragma warning restore SA1600 // Elements should be documented

// Make Program accessible for integration tests
/// <summary>
/// Allows test assemblies to reference the application's entry point.
/// </summary>
public partial class Program
{
}
