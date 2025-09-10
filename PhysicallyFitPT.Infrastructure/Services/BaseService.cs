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
  private readonly ILogger logger;

  /// <summary>
  /// Initializes a new instance of the <see cref="BaseService"/> class.
  /// </summary>
  /// <param name="logger"></param>
  protected BaseService(ILogger logger)
  {
    this.logger = logger;
  }

  /// <summary>
  ///
  /// </summary>
  /// <typeparam name="T"></typeparam>
  /// <param name="operation"></param>
  /// <param name="operationName"></param>
  /// <param name="defaultValue"></param>
  /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
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

  /// <summary>
  ///
  /// </summary>
  /// <param name="operation"></param>
  /// <param name="operationName"></param>
  /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
  protected async Task ExecuteWithErrorHandlingAsync(Func<Task> operation, string operationName)
  {
    try
    {
      await operation();
    }
    catch (Exception ex)
    {
      this.logger.LogError(ex, "Error executing {OperationName}: {ErrorMessage}", operationName, ex.Message);
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
      this.logger.LogError(ex, "Error executing {OperationName}: {ErrorMessage}", operationName, ex.Message);

      if (defaultValue is not null)
      {
        return defaultValue;
      }

      throw; // Re-throw if no default value provided
    }
  }
}
