// <copyright file="IPatientService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace PhysicallyFitPT.Infrastructure.Services.Interfaces;

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using PhysicallyFitPT.Domain;
using PhysicallyFitPT.Shared;

public interface IPatientService
{
  /// <summary>
  ///
  /// </summary>
  /// <param name="query"></param>
  /// <param name="take"></param>
  /// <param name="cancellationToken"></param>
  /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
  Task<IEnumerable<PatientDto>> SearchAsync(string query, int take = 50, CancellationToken cancellationToken = default);
}
