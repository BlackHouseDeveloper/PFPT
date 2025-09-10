// <copyright file="PatientMapperExtensions.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace PhysicallyFitPT.Infrastructure.Mappers
{
  using PhysicallyFitPT.Domain;
  using PhysicallyFitPT.Shared;

  public static class PatientMapperExtensions
    {
        public static PatientDto ToDto(this Patient patient)
        {
            return new PatientDto
            {
                Id = patient.Id,
                MRN = patient.MRN,
                FirstName = patient.FirstName,
                LastName = patient.LastName,
                DateOfBirth = patient.DateOfBirth,
                Sex = patient.Sex,
                Email = patient.Email,
                MobilePhone = patient.MobilePhone,
                MedicationsCsv = patient.MedicationsCsv,
                ComorbiditiesCsv = patient.ComorbiditiesCsv,
                AssistiveDevicesCsv = patient.AssistiveDevicesCsv,
                LivingSituation = patient.LivingSituation,
            };
        }
    }
}
