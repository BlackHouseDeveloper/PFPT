// <copyright file="BaseService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace PhysicallyFitPT.Infrastructure.Services;

using Microsoft.Extensions.Logging;

/// <summary>
/// Base service class with error handling capabilities.
/// </summary>
public abstract class BaseService
{
  protected readonly ILogger Logger;

  protected BaseService(ILogger logger)
  {
    this.Logger = logger;
  }

  protected async Task<T> ExecuteWithErrorHandlingAsync<T>(Func<Task<T>> operation, string operationName, T? defaultValue = default)
  {
    try
    {
      return await operation();
    }
    catch (Exception ex)
    {
      this.Logger.LogError(ex, "Error executing {OperationName}: {ErrorMessage}", operationName, ex.Message);

      if (defaultValue is not null)
      {
        return defaultValue;
      }

      throw; // Re-throw if no default value provided
    }
  }

  protected async Task ExecuteWithErrorHandlingAsync(Func<Task> operation, string operationName)
  {
    try
    {
      await operation();
    }
    catch (Exception ex)
    {
      this.Logger.LogError(ex, "Error executing {OperationName}: {ErrorMessage}", operationName, ex.Message);
      throw; // Re-throw to let caller handle
    }
  }

  protected T ExecuteWithErrorHandling<T>(Func<T> operation, string operationName, T? defaultValue = default)
  {
    try
    {
      return operation();
    }
    catch (Exception ex)
    {
      this.Logger.LogError(ex, "Error executing {OperationName}: {ErrorMessage}", operationName, ex.Message);

      if (defaultValue is not null)
      {
        return defaultValue;
      }

      throw; // Re-throw if no default value provided
    }
  }
}
