// <copyright file="IPatientService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace PhysicallyFitPT.Infrastructure.Services.Interfaces;

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using PhysicallyFitPT.Core;
using PhysicallyFitPT.Shared;

/// <summary>
/// Interface for patient management services including search functionality.
/// </summary>
public interface IPatientService
{
  /// <summary>
  /// Searches for patients based on a query string.
  /// </summary>
  /// <param name="query">The search query to match against patient names.</param>
  /// <param name="take">The maximum number of patients to return.</param>
  /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
  /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
  Task<IEnumerable<PatientDto>> SearchAsync(string query, int take = 50, CancellationToken cancellationToken = default);
}
