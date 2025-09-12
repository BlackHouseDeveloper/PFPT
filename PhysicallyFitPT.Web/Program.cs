// <copyright file="Program.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.EntityFrameworkCore;
using PhysicallyFitPT.Infrastructure.Data;
using PhysicallyFitPT.Infrastructure.Services;
using PhysicallyFitPT.Infrastructure.Services.Interfaces;
using PhysicallyFitPT.Web;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Register DbContextFactory with in-memory database for web
builder.Services.AddDbContextFactory<ApplicationDbContext>(opt =>
    opt.UseInMemoryDatabase("PhysicallyFitPT_Web"));

// Register browser-compatible data store
builder.Services.AddScoped<IDataStore, BrowserDataStore>();

// Register all services - mirror MAUI app setup
builder.Services.AddScoped<IPatientService, PatientService>();
builder.Services.AddScoped<IAppointmentService, AppointmentService>();
builder.Services.AddScoped<IAutoMessagingService, AutoMessagingService>();
builder.Services.AddScoped<INoteBuilderService, NoteBuilderService>();
builder.Services.AddScoped<IQuestionnaireService, QuestionnaireService>();
builder.Services.AddSingleton<IPdfRenderer, PdfRenderer>();

// HTTP client services
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddHttpClient("integrations");

var app = builder.Build();

// Initialize data store
try
{
  using var scope = app.Services.CreateScope();
  var dataStore = scope.ServiceProvider.GetRequiredService<IDataStore>();
  await dataStore.InitializeAsync();
}
catch (Exception ex)
{
  Console.WriteLine($"Failed to initialize data store: {ex.Message}");
  // Continue anyway - the app should still load but may have limited functionality
}

await app.RunAsync();
