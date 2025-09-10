// <copyright file="IPdfRenderer.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace PhysicallyFitPT.Infrastructure.Services.Interfaces;

using PhysicallyFitPT.Shared;

public interface IPdfRenderer
{
  byte[] RenderSimple(string title, string body);

  byte[] RenderSoapNote(NoteDtoDetail noteDto, PatientDto patientDto);
}
