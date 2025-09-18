// <copyright file="Program.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<PhysicallyFitPT.Shared.Components.App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// HTTP client services for API communication
builder.Services.AddScoped(_ => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddHttpClient("integrations");

// TODO: Add typed HTTP client services for API endpoints here
// Example: builder.Services.AddHttpClient<IPatientApiClient, PatientApiClient>();
await builder.Build().RunAsync();
