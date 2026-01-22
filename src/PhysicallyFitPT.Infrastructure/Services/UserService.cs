// <copyright file="UserService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace PhysicallyFitPT.Infrastructure.Services
{
  using System.Security.Claims;
  using System.Threading;
  using System.Threading.Tasks;
  using Microsoft.Extensions.Logging;
  using PhysicallyFitPT.Shared;

  /// <summary>
  /// Basic user service with hard-coded credentials for Week 2 authentication stub.
  /// </summary>
  public class UserService : BaseService, IUserService
  {
    // PIN-based demo auth for parity with Login UI
    private const string DemoPin = "1234";
    private const string DemoDisplayName = "Dr. Demo Clinician";
    private ClaimsPrincipal? currentUser;

    /// <summary>
    /// Initializes a new instance of the <see cref="UserService"/> class.
    /// </summary>
    /// <param name="logger">Logger instance for logging operations.</param>
    public UserService(ILogger<UserService> logger)
        : base(logger)
    {
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
      // Use the same 4-digit PIN for both username and password inputs to align with the login screen
      if (username == DemoPin && password == DemoPin)
      {
        var claims = new[]
        {
          new Claim(ClaimTypes.Email, $"{DemoPin}@demo.pin"),
          new Claim(ClaimTypes.Name, DemoDisplayName),
          new Claim(ClaimTypes.Role, "Clinician"),
        };

        var identity = new ClaimsIdentity(claims, "DemoAuth");
        this.currentUser = new ClaimsPrincipal(identity);

        this.Logger.LogInformation("User {Username} logged in successfully", username);
        return Task.FromResult(true);
      }

      this.Logger.LogWarning("Failed login attempt for username: {Username}", username);
      return Task.FromResult(false);
    }

    /// <inheritdoc/>
    public Task LogoutAsync(CancellationToken cancellationToken = default)
    {
      this.currentUser = null;
      this.Logger.LogInformation("User logged out");
      return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task<string?> GetAccessTokenAsync(CancellationToken cancellationToken = default)
    {
      // For now, return a dummy token when authenticated
      return Task.FromResult(this.IsAuthenticated ? "demo-token-placeholder" : null);
    }

    /// <inheritdoc/>
    public Task<bool> RefreshTokenAsync(CancellationToken cancellationToken = default)
    {
      // Not implemented for Week 2 stub
      return Task.FromResult(false);
    }
  }
}
