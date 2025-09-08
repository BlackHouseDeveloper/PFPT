using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using PhysicallyFitPT.Infrastructure.Data;
using PhysicallyFitPT.Infrastructure.Services;
using PhysicallyFitPT.Infrastructure.Services.Interfaces;
using PhysicallyFitPT.Web.Components;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Add HTTP context accessor
builder.Services.AddHttpContextAccessor();

// Add authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/login";
        options.LogoutPath = "/logout";
        options.AccessDeniedPath = "/login";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.SlidingExpiration = true;
    });
builder.Services.AddAuthorization();

// Add database context
var connectionString = builder.Configuration.GetConnectionString("Default") 
    ?? "Data Source=pfpt.web.sqlite";
builder.Services.AddDbContextFactory<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));

// Add application services
builder.Services.AddScoped<IPatientService, PatientService>();
builder.Services.AddScoped<IAppointmentService, AppointmentService>();
builder.Services.AddScoped<IAutoMessagingService, AutoMessagingService>();
builder.Services.AddScoped<INoteBuilderService, NoteBuilderService>();
builder.Services.AddSingleton<IPdfRenderer, PdfRenderer>();

// Add HTTP client
builder.Services.AddHttpClient();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.UseAuthentication();
app.UseAuthorization();

// PDF Export API endpoint
app.MapPost("/api/notes/{noteId}/pdf", async (Guid noteId, IPdfRenderer pdfRenderer, INoteBuilderService noteService) =>
{
    var note = await noteService.GetAsync(noteId);
    if (note == null)
        return Results.NotFound();
    
    // Create a simple PDF representation (HIPAA-safe - only note ID logged)
    var title = $"SOAP Note - {note.CreatedAt:yyyy-MM-dd}";
    var body = $"Note ID: {note.Id}\nVisit Type: {note.VisitType}\n";
    
    var pdfBytes = pdfRenderer.RenderSimple(title, body);
    return Results.File(pdfBytes, "application/pdf", $"note-{noteId}.pdf");
});

// Health check endpoint
app.MapGet("/health", () => Results.Ok(new { status = "healthy", timestamp = DateTime.UtcNow }));

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
