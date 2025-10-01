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

using Microsoft.EntityFrameworkCore;
using Microsoft.FeatureManagement;
using PhysicallyFitPT.Core;
using PhysicallyFitPT.Infrastructure.Data;
using PhysicallyFitPT.Infrastructure.Services;
using PhysicallyFitPT.Infrastructure.Services.Interfaces;
using PhysicallyFitPT.Shared;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

builder.Services.AddDbContextFactory<ApplicationDbContext>(options =>
{
    options.UseSqlite(connectionString);
});

// DI Setup for services
builder.Services.AddScoped<IPatientService, PatientService>();
builder.Services.AddScoped<IAppointmentService, AppointmentService>();
builder.Services.AddScoped<INoteBuilderService, NoteBuilderService>();
builder.Services.AddScoped<IQuestionnaireService, QuestionnaireService>();
builder.Services.AddScoped<IAutoMessagingService, AutoMessagingService>();
builder.Services.AddScoped<IDashboardMetricsService, DashboardMetricsService>();
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
v1.MapGet("/stats", async (IDbContextFactory<ApplicationDbContext> dbContextFactory, CancellationToken cancellationToken) =>
{
    try
    {
        await using var db = await dbContextFactory.CreateDbContextAsync(cancellationToken);
        
        var patientCount = await db.Patients.CountAsync(cancellationToken);
        var appointmentCount = await db.Appointments.CountAsync(cancellationToken);
        
        // Get the most recent patient update using ToListAsync to work around SQLite DateTimeOffset limitation
        var patients = await db.Patients
            .Select(p => new { p.UpdatedAt, p.CreatedAt })
            .ToListAsync(cancellationToken);
        
        var lastPatientUpdated = patients
            .Select(p => p.UpdatedAt ?? p.CreatedAt)
            .OrderByDescending(d => d)
            .FirstOrDefault();

        var stats = new
        {
            Patients = patientCount,
            Appointments = appointmentCount,
            LastPatientUpdated = lastPatientUpdated,
            ApiHealthy = true,
        };

        return Results.Ok(stats);
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message);
    }
})
.WithName("GetAppStats")
.WithOpenApi();

// Legacy endpoint for backwards compatibility
app.MapGet("/api/stats", async (IDbContextFactory<ApplicationDbContext> dbContextFactory, CancellationToken cancellationToken) =>
{
    try
    {
        await using var db = await dbContextFactory.CreateDbContextAsync(cancellationToken);
        
        var patientCount = await db.Patients.CountAsync(cancellationToken);
        var appointmentCount = await db.Appointments.CountAsync(cancellationToken);
        
        // Get the most recent patient update using ToListAsync to work around SQLite DateTimeOffset limitation
        var patients = await db.Patients
            .Select(p => new { p.UpdatedAt, p.CreatedAt })
            .ToListAsync(cancellationToken);
        
        var lastPatientUpdated = patients
            .Select(p => p.UpdatedAt ?? p.CreatedAt)
            .OrderByDescending(d => d)
            .FirstOrDefault();

        var stats = new
        {
            Patients = patientCount,
            Appointments = appointmentCount,
            LastPatientUpdated = lastPatientUpdated,
            ApiHealthy = true,
        };

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
public partial class Program
{
}
