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
using PhysicallyFitPT.Core;
using PhysicallyFitPT.Infrastructure.Data;
using PhysicallyFitPT.Infrastructure.Services;
using PhysicallyFitPT.Infrastructure.Services.Interfaces;
using PhysicallyFitPT.Shared;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure Entity Framework
builder.Services.AddDbContextFactory<ApplicationDbContext>(options =>
{
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=pfpt.db");
});

// DI Setup for services
builder.Services.AddScoped<IPatientService, PatientService>();
builder.Services.AddScoped<IAppointmentService, AppointmentService>();
builder.Services.AddScoped<INoteBuilderService, NoteBuilderService>();
builder.Services.AddScoped<IQuestionnaireService, QuestionnaireService>();
builder.Services.AddScoped<IAutoMessagingService, AutoMessagingService>();

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
app.MapGet("/api/notes/appointment/{appointmentId}", async (Guid appointmentId, INoteBuilderService noteService) =>
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

// Minimal APIs for questionnaires
app.MapGet("/api/questionnaires", async (IQuestionnaireService questionnaireService) =>
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
app.MapGet("/api/dashboard/stats", async (IPatientService patientService, IAppointmentService appointmentService) =>
{
    try
    {
        // For now, we'll return mock statistics since we need more infrastructure to get real stats
        // TODO: Implement real dashboard statistics once aggregation services are available
        var stats = new DashboardStatsDto
        {
            TodaysAppointments = 8,  // Mock data
            ActivePatients = 127,    // Mock data
            PendingNotes = 3,        // Mock data
            OverdueOutcomeMeasures = 5 // Mock data
        };
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
