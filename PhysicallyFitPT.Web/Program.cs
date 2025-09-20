// <copyright file="Program.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Http;
using PhysicallyFitPT.Shared;
using PhysicallyFitPT.Web.Services;
using Polly;
using Polly.Extensions.Http;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add(typeof(PhysicallyFitPT.Web.App), "#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Configuration
builder.Services.Configure<ApiConfiguration>(
    builder.Configuration.GetSection("Api"));

// Platform services
builder.Services.AddSingleton<IPlatformInfo, WebPlatformInfo>();

// HTTP client with Polly resilience patterns
builder.Services.AddHttpClient<IDataService, WebApiDataService>(client =>
{
  var apiConfig = builder.Configuration.GetSection("Api").Get<ApiConfiguration>() ?? new ApiConfiguration();
  client.BaseAddress = new Uri(apiConfig.BaseUrl);
  client.Timeout = TimeSpan.FromSeconds(apiConfig.TimeoutSeconds);
})
.AddPolicyHandler(GetRetryPolicy())
.AddPolicyHandler(GetCircuitBreakerPolicy());

// Default HTTP client for other services
builder.Services.AddScoped(_ => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

await builder.Build().RunAsync();

static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
{
  return HttpPolicyExtensions
      .HandleTransientHttpError()
      .WaitAndRetryAsync(
          retryCount: 3,
          sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
          onRetry: (_, timespan, retryCount, _) =>
            Console.WriteLine($"Retry {retryCount} after {timespan} seconds"));
}

static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
{
  return HttpPolicyExtensions
      .HandleTransientHttpError()
      .CircuitBreakerAsync(
          handledEventsAllowedBeforeBreaking: 5,
          durationOfBreak: TimeSpan.FromSeconds(30),
          onBreak: (_, duration) =>
            Console.WriteLine($"Circuit breaker opened for {duration}"),
          onReset: () =>
            Console.WriteLine("Circuit breaker reset"));
}
