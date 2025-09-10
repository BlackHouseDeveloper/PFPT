// <copyright file="PdfRenderer.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace PhysicallyFitPT.Infrastructure.Services;

using System;
using System.Linq;
using PhysicallyFitPT.Infrastructure.Services.Interfaces;
using PhysicallyFitPT.Shared;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

public class PdfRenderer : IPdfRenderer
{
  /// <inheritdoc/>
  public byte[] RenderSimple(string title, string body)
  {
    return Document.Create(container =>
    {
      container.Page(page =>
      {
        page.Size(PageSizes.A4);
        page.Margin(36);
        page.DefaultTextStyle(t => t.FontSize(11));
        page.Content().Column(col =>
        {
          col.Item().Text(title).FontSize(18).SemiBold();
          col.Item().Text(body);
        });
      });
    }).GeneratePdf();
  }

  /// <inheritdoc/>
  public byte[] RenderSoapNote(NoteDtoDetail noteDto, PatientDto patientDto)
  {
    return Document.Create(container =>
    {
      container.Page(page =>
      {
        page.Size(PageSizes.A4);
        page.Margin(36);
        page.DefaultTextStyle(t => t.FontSize(10).FontFamily("Arial"));
        
        page.Header().Column(header =>
        {
          header.Item().AlignCenter().Text("Physical Therapy Evaluation/Treatment Note")
            .FontSize(16).SemiBold();
          header.Item().PaddingVertical(10);
        });

        page.Content().Column(content =>
        {
          // Patient Information Section
          content.Item().Element(container => RenderPatientInfo(container, patientDto, noteDto));
          
          content.Item().PaddingVertical(10);

          // SOAP Note Sections
          content.Item().Element(container => RenderSubjectiveSection(container, noteDto.Subjective));
          content.Item().Element(container => RenderObjectiveSection(container, noteDto.Objective));
          content.Item().Element(container => RenderAssessmentSection(container, noteDto.Assessment));
          content.Item().Element(container => RenderPlanSection(container, noteDto.Plan));

          // Signature Section
          content.Item().PaddingTop(20).Element(container => RenderSignatureSection(container, noteDto));
        });

        page.Footer().Column(footer =>
        {
          footer.Item().AlignCenter().Text(text =>
          {
            text.Span("Generated on ");
            text.Span(DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm UTC")).SemiBold();
          });
        });
      });
    }).GeneratePdf();
  }

  private static void RenderPatientInfo(IContainer container, PatientDto patient, NoteDtoDetail note)
  {
    container.Border(1).Padding(10).Column(col =>
    {
      col.Item().Text("PATIENT INFORMATION").FontSize(12).SemiBold();
      col.Item().PaddingVertical(5);
      
      col.Item().Row(row =>
      {
        row.RelativeItem().Text($"Name: {patient.FirstName} {patient.LastName}").FontSize(10);
        row.RelativeItem().Text($"DOB: {patient.DateOfBirth?.ToString("MM/dd/yyyy") ?? "N/A"}").FontSize(10);
        row.RelativeItem().Text($"Sex: {patient.Sex ?? "N/A"}").FontSize(10);
      });
      
      col.Item().Row(row =>
      {
        row.RelativeItem().Text($"MRN: {patient.MRN ?? "N/A"}").FontSize(10);
        row.RelativeItem().Text($"Visit Type: {note.VisitType}").FontSize(10);
        row.RelativeItem().Text($"Date: {DateTime.UtcNow:MM/dd/yyyy}").FontSize(10);
      });
    });
  }

  private static void RenderSubjectiveSection(IContainer container, SubjectiveDto subjective)
  {
    container.PaddingVertical(10).Column(col =>
    {
      col.Item().Text("SUBJECTIVE").FontSize(12).SemiBold();
      col.Item().PaddingVertical(5);
      
      if (!string.IsNullOrEmpty(subjective.ChiefComplaint))
      {
        col.Item().Text($"Chief Complaint: {subjective.ChiefComplaint}").FontSize(10);
        col.Item().PaddingVertical(2);
      }
      
      if (!string.IsNullOrEmpty(subjective.HistoryOfPresentIllness))
      {
        col.Item().Text($"History: {subjective.HistoryOfPresentIllness}").FontSize(10);
        col.Item().PaddingVertical(2);
      }
      
      if (!string.IsNullOrEmpty(subjective.PainLocationsCsv))
      {
        col.Item().Text($"Pain Locations: {subjective.PainLocationsCsv}").FontSize(10);
        col.Item().PaddingVertical(2);
      }
      
      if (!string.IsNullOrEmpty(subjective.PainSeverity0to10))
      {
        col.Item().Text($"Pain Level: {subjective.PainSeverity0to10}/10").FontSize(10);
        col.Item().PaddingVertical(2);
      }
      
      if (!string.IsNullOrEmpty(subjective.FunctionalLimitations))
      {
        col.Item().Text($"Functional Limitations: {subjective.FunctionalLimitations}").FontSize(10);
        col.Item().PaddingVertical(2);
      }
      
      if (!string.IsNullOrEmpty(subjective.PatientGoalsNarrative))
      {
        col.Item().Text($"Patient Goals: {subjective.PatientGoalsNarrative}").FontSize(10);
      }
    });
  }

  private static void RenderObjectiveSection(IContainer container, ObjectiveDto objective)
  {
    container.PaddingVertical(10).Column(col =>
    {
      col.Item().Text("OBJECTIVE").FontSize(12).SemiBold();
      col.Item().PaddingVertical(5);
      
      // ROM Measurements
      if (objective.Rom.Any())
      {
        col.Item().Text("Range of Motion:").FontSize(11).SemiBold();
        foreach (var rom in objective.Rom)
        {
          col.Item().Text($"  • {rom.Joint} {rom.Movement} ({rom.Side}): {rom.MeasuredDegrees}° (Normal: {rom.NormalDegrees}°){(rom.WithPain ? " - with pain" : "")}").FontSize(10);
        }
        col.Item().PaddingVertical(3);
      }
      
      // MMT Measurements
      if (objective.Mmt.Any())
      {
        col.Item().Text("Manual Muscle Testing:").FontSize(11).SemiBold();
        foreach (var mmt in objective.Mmt)
        {
          col.Item().Text($"  • {mmt.MuscleGroup} ({mmt.Side}): {mmt.Grade}{(mmt.WithPain ? " - with pain" : "")}").FontSize(10);
        }
        col.Item().PaddingVertical(3);
      }
      
      // Special Tests
      if (objective.SpecialTests.Any())
      {
        col.Item().Text("Special Tests:").FontSize(11).SemiBold();
        foreach (var test in objective.SpecialTests)
        {
          col.Item().Text($"  • {test.Name} ({test.Side}): {test.Result}").FontSize(10);
        }
        col.Item().PaddingVertical(3);
      }
      
      // Outcome Measures
      if (objective.OutcomeMeasures.Any())
      {
        col.Item().Text("Outcome Measures:").FontSize(11).SemiBold();
        foreach (var om in objective.OutcomeMeasures)
        {
          col.Item().Text($"  • {om.Instrument}: {om.RawScore} ({om.Percent:F1}%)").FontSize(10);
        }
        col.Item().PaddingVertical(3);
      }
      
      // Interventions
      if (objective.ProvidedInterventions.Any())
      {
        col.Item().Text("Interventions Provided:").FontSize(11).SemiBold();
        foreach (var intervention in objective.ProvidedInterventions)
        {
          col.Item().Text($"  • {intervention.CptCode}: {intervention.Description} - {intervention.Units} units, {intervention.Minutes} minutes").FontSize(10);
        }
      }
    });
  }

  private static void RenderAssessmentSection(IContainer container, AssessmentDto assessment)
  {
    container.PaddingVertical(10).Column(col =>
    {
      col.Item().Text("ASSESSMENT").FontSize(12).SemiBold();
      col.Item().PaddingVertical(5);
      
      if (!string.IsNullOrEmpty(assessment.ClinicalImpression))
      {
        col.Item().Text($"Clinical Impression: {assessment.ClinicalImpression}").FontSize(10);
        col.Item().PaddingVertical(2);
      }
      
      if (!string.IsNullOrEmpty(assessment.RehabPotential))
      {
        col.Item().Text($"Rehab Potential: {assessment.RehabPotential}").FontSize(10);
        col.Item().PaddingVertical(2);
      }
      
      if (assessment.Icd10Codes.Any())
      {
        col.Item().Text("Diagnoses:").FontSize(11).SemiBold();
        foreach (var icd in assessment.Icd10Codes)
        {
          col.Item().Text($"  • {icd.Code}: {icd.Description}").FontSize(10);
        }
        col.Item().PaddingVertical(2);
      }
      
      if (assessment.Goals.Any())
      {
        col.Item().Text("Goals:").FontSize(11).SemiBold();
        foreach (var goal in assessment.Goals)
        {
          col.Item().Text($"  • {goal.Description} (Target: {goal.TargetDate?.ToString("MM/dd/yyyy")})").FontSize(10);
        }
      }
    });
  }

  private static void RenderPlanSection(IContainer container, PlanDto plan)
  {
    container.PaddingVertical(10).Column(col =>
    {
      col.Item().Text("PLAN").FontSize(12).SemiBold();
      col.Item().PaddingVertical(5);
      
      if (!string.IsNullOrEmpty(plan.Frequency))
      {
        col.Item().Text($"Frequency: {plan.Frequency}").FontSize(10);
        col.Item().PaddingVertical(2);
      }
      
      if (!string.IsNullOrEmpty(plan.Duration))
      {
        col.Item().Text($"Duration: {plan.Duration}").FontSize(10);
        col.Item().PaddingVertical(2);
      }
      
      if (!string.IsNullOrEmpty(plan.PlannedInterventionsCsv))
      {
        col.Item().Text($"Planned Interventions: {plan.PlannedInterventionsCsv}").FontSize(10);
        col.Item().PaddingVertical(2);
      }
      
      if (!string.IsNullOrEmpty(plan.NextVisitFocus))
      {
        col.Item().Text($"Next Visit Focus: {plan.NextVisitFocus}").FontSize(10);
        col.Item().PaddingVertical(2);
      }
      
      if (plan.Hep.Any())
      {
        col.Item().Text("Home Exercise Program:").FontSize(11).SemiBold();
        foreach (var exercise in plan.Hep)
        {
          col.Item().Text($"  • {exercise.Name}: {exercise.Dosage}").FontSize(10);
          if (!string.IsNullOrEmpty(exercise.Notes))
          {
            col.Item().Text($"    Notes: {exercise.Notes}").FontSize(9).Italic();
          }
        }
      }
    });
  }

  private static void RenderSignatureSection(IContainer container, NoteDtoDetail note)
  {
    container.Border(1).Padding(10).Column(col =>
    {
      col.Item().Text("SIGNATURE").FontSize(12).SemiBold();
      col.Item().PaddingVertical(5);
      
      if (note.IsSigned)
      {
        col.Item().Row(row =>
        {
          row.RelativeItem().Text($"Signed by: {note.SignedBy}").FontSize(10);
          row.RelativeItem().Text($"Date: {note.SignedAt?.ToString("MM/dd/yyyy HH:mm")}").FontSize(10);
        });
      }
      else
      {
        col.Item().Text("*** DRAFT - NOT SIGNED ***").FontSize(12).SemiBold().FontColor(Colors.Red.Medium);
      }
    });
  }
}




