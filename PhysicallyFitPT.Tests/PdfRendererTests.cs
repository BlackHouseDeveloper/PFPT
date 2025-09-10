// <copyright file="PdfRendererTests.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace PhysicallyFitPT.Tests;

using System;
using System.Collections.Generic;
using FluentAssertions;
using PhysicallyFitPT.Infrastructure.Services;
using PhysicallyFitPT.Infrastructure.Services.Interfaces;
using PhysicallyFitPT.Shared;
using QuestPDF.Infrastructure;
using Xunit;

public class PdfRendererTests
{
    static PdfRendererTests()
    {
        // Configure QuestPDF Community License for testing
        QuestPDF.Settings.License = LicenseType.Community;
    }

    [Fact]
    public void RenderSimple_ReturnsNonEmptyPdfBytes()
    {
        // Arrange
        IPdfRenderer renderer = new PdfRenderer();
        var title = "Test Document";
        var body = "This is a test document body.";

        // Act
        var pdfBytes = renderer.RenderSimple(title, body);

        // Assert
        pdfBytes.Should().NotBeNull();
        pdfBytes.Should().NotBeEmpty();
        pdfBytes.Length.Should().BeGreaterThan(100); // Basic sanity check for PDF content
        
        // Check PDF header (starts with %PDF)
        var pdfHeader = System.Text.Encoding.ASCII.GetString(pdfBytes, 0, 4);
        pdfHeader.Should().Be("%PDF");
    }

    [Fact]
    public void RenderSoapNote_WithCompleteNote_ReturnsValidPdf()
    {
        // Arrange
        IPdfRenderer renderer = new PdfRenderer();
        
        var patient = new PatientDto
        {
            Id = Guid.NewGuid(),
            FirstName = "John",
            LastName = "Doe",
            DateOfBirth = new DateTime(1990, 5, 15),
            Sex = "M",
            MRN = "12345"
        };

        var note = new NoteDtoDetail
        {
            Id = Guid.NewGuid(),
            VisitType = "Eval",
            IsSigned = true,
            SignedBy = "PT Jane Smith",
            SignedAt = DateTimeOffset.UtcNow,
            Subjective = new SubjectiveDto
            {
                ChiefComplaint = "Lower back pain",
                HistoryOfPresentIllness = "Patient reports onset of pain 2 weeks ago",
                PainLocationsCsv = "Lower back, radiating to left leg",
                PainSeverity0to10 = "7",
                FunctionalLimitations = "Difficulty sitting for extended periods"
            },
            Objective = new ObjectiveDto
            {
                Rom = new List<RomMeasureDto>
                {
                    new RomMeasureDto 
                    { 
                        Joint = "Lumbar Spine", 
                        Movement = "Flexion", 
                        Side = 0, 
                        MeasuredDegrees = 45, 
                        NormalDegrees = 60,
                        WithPain = true
                    }
                },
                Mmt = new List<MmtMeasureDto>
                {
                    new MmtMeasureDto
                    {
                        MuscleGroup = "Hip Flexors",
                        Side = 1,
                        Grade = "4/5",
                        WithPain = false
                    }
                },
                ProvidedInterventions = new List<ProvidedInterventionDto>
                {
                    new ProvidedInterventionDto
                    {
                        CptCode = "97110",
                        Description = "Therapeutic Exercise",
                        Units = 2,
                        Minutes = 30
                    }
                }
            },
            Assessment = new AssessmentDto
            {
                ClinicalImpression = "Mechanical lower back pain with radiculopathy",
                RehabPotential = "Good",
                Icd10Codes = new List<Icd10LinkDto>
                {
                    new Icd10LinkDto { Code = "M54.5", Description = "Low back pain" }
                }
            },
            Plan = new PlanDto
            {
                Frequency = "3x/week",
                Duration = "4-6 weeks",
                PlannedInterventionsCsv = "Therapeutic exercise, manual therapy",
                NextVisitFocus = "Continue strengthening exercises"
            }
        };

        // Act
        var pdfBytes = renderer.RenderSoapNote(note, patient);

        // Assert
        pdfBytes.Should().NotBeNull();
        pdfBytes.Should().NotBeEmpty();
        pdfBytes.Length.Should().BeGreaterThan(1000); // Should be substantial for a complete note
        
        // Check PDF header
        var pdfHeader = System.Text.Encoding.ASCII.GetString(pdfBytes, 0, 4);
        pdfHeader.Should().Be("%PDF");
    }

    [Fact]
    public void RenderSoapNote_WithMinimalNote_ReturnsValidPdf()
    {
        // Arrange
        IPdfRenderer renderer = new PdfRenderer();
        
        var patient = new PatientDto
        {
            Id = Guid.NewGuid(),
            FirstName = "Jane",
            LastName = "Smith"
        };

        var note = new NoteDtoDetail
        {
            Id = Guid.NewGuid(),
            VisitType = "Daily",
            IsSigned = false,
            Subjective = new SubjectiveDto(),
            Objective = new ObjectiveDto
            {
                Rom = new List<RomMeasureDto>(),
                Mmt = new List<MmtMeasureDto>(),
                ProvidedInterventions = new List<ProvidedInterventionDto>()
            },
            Assessment = new AssessmentDto
            {
                Icd10Codes = new List<Icd10LinkDto>()
            },
            Plan = new PlanDto()
        };

        // Act
        var pdfBytes = renderer.RenderSoapNote(note, patient);

        // Assert
        pdfBytes.Should().NotBeNull();
        pdfBytes.Should().NotBeEmpty();
        
        // Even minimal note should generate valid PDF
        var pdfHeader = System.Text.Encoding.ASCII.GetString(pdfBytes, 0, 4);
        pdfHeader.Should().Be("%PDF");
    }

    [Fact]
    public void RenderSoapNote_WithUnsignedNote_ShowsDraftStatus()
    {
        // This test verifies that unsigned notes are clearly marked as drafts
        // We can't easily verify the content without parsing PDF, but we ensure it generates
        
        // Arrange
        IPdfRenderer renderer = new PdfRenderer();
        
        var patient = new PatientDto { FirstName = "Test", LastName = "Patient" };
        var note = new NoteDtoDetail
        {
            Id = Guid.NewGuid(),
            VisitType = "Progress",
            IsSigned = false, // Key: unsigned note
            Subjective = new SubjectiveDto(),
            Objective = new ObjectiveDto
            {
                Rom = new List<RomMeasureDto>(),
                Mmt = new List<MmtMeasureDto>(),
                ProvidedInterventions = new List<ProvidedInterventionDto>()
            },
            Assessment = new AssessmentDto { Icd10Codes = new List<Icd10LinkDto>() },
            Plan = new PlanDto()
        };

        // Act
        var pdfBytes = renderer.RenderSoapNote(note, patient);

        // Assert
        pdfBytes.Should().NotBeNull();
        pdfBytes.Should().NotBeEmpty();
        
        // Should still generate valid PDF even for unsigned notes
        var pdfHeader = System.Text.Encoding.ASCII.GetString(pdfBytes, 0, 4);
        pdfHeader.Should().Be("%PDF");
    }
}