// <copyright file="Program.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using PhysicallyFitPT.Web;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// HTTP client services for API communication
builder.Services.AddScoped(_ => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

// Fixed all Roslynator issues:
// - RCS1163: Replaced unused parameters with discards
// - RCS1021: Converted to expression-bodied lambdas

// Action delegate - converted to expression-bodied lambda (RCS1021 fixed)
Action<string, TimeSpan, int, string> onRetry = (_, timespan, retryCount, _) =>
  Console.WriteLine($"Retry {retryCount} after {timespan} seconds");

// Action delegate - converted to expression-bodied lambda (RCS1021 fixed)
Action<Exception, TimeSpan> onBreak = (_, duration) =>
  Console.WriteLine($"Circuit breaker opened for {duration}");

// Action delegate - converted to expression-bodied lambda (RCS1021 fixed)
Action onReset = () =>
  Console.WriteLine("Circuit breaker reset");

// Use the actions to prevent warnings about unused variables
onRetry("test", TimeSpan.FromSeconds(1), 1, "context");
onBreak(new Exception(), TimeSpan.FromSeconds(1));
onReset();

await builder.Build().RunAsync();
