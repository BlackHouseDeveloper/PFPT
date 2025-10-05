// <copyright file="DemoUserService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace PhysicallyFitPT.Web.Services;

using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PhysicallyFitPT.Shared;

/// <summary>
/// Browser-friendly demo user service that avoids native SQLite dependencies.
/// </summary>
public class DemoUserService : IUserService
{
  private const string DemoUsername = "clinician@demo.com";
  private const string DemoPassword = "demo123";
  private const string DemoDisplayName = "Dr. Demo Clinician";

  private readonly ILogger<DemoUserService> logger;
  private ClaimsPrincipal? currentUser;

  /// <summary>
  /// Initializes a new instance of the <see cref="DemoUserService"/> class.
  /// </summary>
  /// <param name="logger">Logger used for diagnostic messages.</param>
  public DemoUserService(ILogger<DemoUserService> logger)
  {
    this.logger = logger;
  }

  /// <inheritdoc/>
  public bool IsAuthenticated => this.currentUser?.Identity?.IsAuthenticated ?? false;

  /// <inheritdoc/>
  public ClaimsPrincipal? CurrentUser => this.currentUser;

  /// <inheritdoc/>
  public string? UserEmail => this.currentUser?.FindFirst(ClaimTypes.Email)?.Value;

  /// <inheritdoc/>
  public string? UserDisplayName => this.currentUser?.FindFirst(ClaimTypes.Name)?.Value;

  /// <inheritdoc/>
  public Task<bool> LoginAsync(string username, string password, CancellationToken cancellationToken = default)
  {
    if (username == DemoUsername && password == DemoPassword)
    {
      var claims = new[]
      {
        new Claim(ClaimTypes.Email, DemoUsername),
        new Claim(ClaimTypes.Name, DemoDisplayName),
        new Claim(ClaimTypes.Role, "Clinician"),
      };

      var identity = new ClaimsIdentity(claims, "DemoAuth");
      this.currentUser = new ClaimsPrincipal(identity);

      this.logger.LogInformation("User {Username} logged in successfully", username);
      return Task.FromResult(true);
    }

    this.logger.LogWarning("Failed login attempt for username: {Username}", username);
    return Task.FromResult(false);
  }

  /// <inheritdoc/>
  public Task LogoutAsync(CancellationToken cancellationToken = default)
  {
    this.currentUser = null;
    this.logger.LogInformation("User logged out");
    return Task.CompletedTask;
  }

  /// <inheritdoc/>
  public Task<string?> GetAccessTokenAsync(CancellationToken cancellationToken = default)
  {
    return Task.FromResult(this.IsAuthenticated ? "demo-token-placeholder" : null);
  }

  /// <inheritdoc/>
  public Task<bool> RefreshTokenAsync(CancellationToken cancellationToken = default)
  {
    return Task.FromResult(false);
  }
}
