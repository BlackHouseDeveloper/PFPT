// <copyright file="IUserService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace PhysicallyFitPT.Shared
{
  using System.Security.Claims;
  using System.Threading;
  using System.Threading.Tasks;

  /// <summary>
  /// Service interface for user authentication and management.
  /// </summary>
  public interface IUserService
  {
    /// <summary>
    /// Gets a value indicating whether the user is currently authenticated.
    /// </summary>
    bool IsAuthenticated { get; }

    /// <summary>
    /// Gets the current user's claims principal.
    /// </summary>
    ClaimsPrincipal? CurrentUser { get; }

    /// <summary>
    /// Gets the current user's email if available.
    /// </summary>
    string? UserEmail { get; }

    /// <summary>
    /// Gets the current user's display name if available.
    /// </summary>
    string? UserDisplayName { get; }

    /// <summary>
    /// Authenticates a user with username and password.
    /// </summary>
    /// <param name="username">The username to authenticate.</param>
    /// <param name="password">The password to authenticate.</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    /// <returns>True if login was successful, false otherwise.</returns>
    Task<bool> LoginAsync(string username, string password, CancellationToken cancellationToken = default);

    /// <summary>
    /// Logs out the current user.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    /// <returns>A task representing the logout operation.</returns>
    Task LogoutAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the current access token for API calls.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    /// <returns>The access token if available, null otherwise.</returns>
    Task<string?> GetAccessTokenAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Refreshes the current authentication token if supported.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    /// <returns>True if refresh was successful, false otherwise.</returns>
    Task<bool> RefreshTokenAsync(CancellationToken cancellationToken = default);
  }
}
