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
  /// <summary>
  /// Initializes a new instance of the <see cref="BaseService"/> class.
  /// </summary>
  /// <param name="logger">Logger instance for logging operations.</param>
  protected BaseService(ILogger logger)
  {
    this.Logger = logger;
  }

  /// <summary>
  /// Gets the logger instance for logging operations.
  /// </summary>
  protected ILogger Logger { get; }

  /// <summary>
  /// Executes an operation with error handling and optional default value on failure.
  /// </summary>
  /// <typeparam name="T">The return type of the operation.</typeparam>
  /// <param name="operation">The operation to execute.</param>
  /// <param name="operationName">Name of the operation for logging purposes.</param>
  /// <param name="defaultValue">Default value to return on exception, if any.</param>
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
  /// Executes an operation with error handling.
  /// </summary>
  /// <param name="operation">The operation to execute.</param>
  /// <param name="operationName">Name of the operation for logging purposes.</param>
  /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
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

  /// <summary>
  /// Executes a synchronous operation with error handling and optional default value on failure.
  /// </summary>
  /// <typeparam name="T">The return type of the operation.</typeparam>
  /// <param name="operation">The operation to execute.</param>
  /// <param name="operationName">Name of the operation for logging purposes.</param>
  /// <param name="defaultValue">Default value to return on exception, if any.</param>
  /// <returns>The result of the operation or the default value on error.</returns>
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
