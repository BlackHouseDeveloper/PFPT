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
  Task<IEnumerable<PatientDto>> SearchAsync(string query, int take = 50, CancellationToken cancellationToken = default);
}
