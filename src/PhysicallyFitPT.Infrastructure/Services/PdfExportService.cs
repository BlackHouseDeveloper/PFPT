// <copyright file="PdfExportService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using Microsoft.JSInterop;

namespace PhysicallyFitPT.Infrastructure.Services;

/// <summary>
/// Service for exporting clinical documents to PDF with HIPAA audit logging.
/// </summary>
public class PdfExportService
{
  private readonly IJSRuntime _jsRuntime;

  /// <summary>
  /// Initializes a new instance of the <see cref="PdfExportService"/> class.
  /// </summary>
  /// <param name="jsRuntime">JavaScript runtime for browser download functionality.</param>
  public PdfExportService(IJSRuntime jsRuntime)
  {
    _jsRuntime = jsRuntime;
  }

  /// <summary>
  /// Triggers a file download in the browser.
  /// </summary>
  /// <param name="pdfBytes">The PDF file bytes.</param>
  /// <param name="filename">The filename for download.</param>
  public async Task DownloadPdfAsync(byte[] pdfBytes, string filename)
  {
    try
    {
      await _jsRuntime.InvokeVoidAsync("pdfExport.downloadBlob", pdfBytes, filename);

      Debug.WriteLine($"[PDF Export] File Downloaded: {filename}, Size: {pdfBytes.Length} bytes");
    }
    catch (Exception ex)
    {
      Debug.WriteLine($"Failed to trigger PDF download: {ex.Message}");
      throw;
    }
  }

  /// <summary>
  /// Creates an audit log entry for HIPAA compliance.
  /// </summary>
  /// <param name="documentType">Type of document exported.</param>
  /// <param name="patientName">Patient name for audit trail.</param>
  /// <param name="documentId">Document identifier.</param>
  /// <param name="options">Export options used.</param>
  public async Task LogExportActivityAsync(
      string documentType,
      string patientName,
      string? documentId,
      ExportOptions? options)
  {
    await Task.CompletedTask;
    try
    {
      var auditEntry = new
      {
        action = "PDF_EXPORT",
        documentType,
        timestamp = DateTime.UtcNow.ToString("O"),
        options = new
        {
          includeSignature = options?.IncludeSignature ?? false,
          includeWatermark = options?.IncludeWatermark ?? false,
          includeTimestamp = options?.IncludeTimestamp ?? false
        },
        metadata = new
        {
          patientName,
          documentId = documentId ?? "N/A"
        }
      };

      // Log to console for development
      Debug.WriteLine($"[HIPAA Audit] {System.Text.Json.JsonSerializer.Serialize(auditEntry)}");

      // TODO: Production - send to backend audit service
      // await _httpClient.PostAsJsonAsync("/api/audit/export", auditEntry);
    }
    catch (Exception ex)
    {
      Debug.WriteLine($"Failed to log export activity: {ex.Message}");
    }
  }

  /// <summary>
  /// Generates a standardized filename with timestamp.
  /// </summary>
  /// <param name="baseType">Base document type (e.g., "SOAP_Note").</param>
  /// <param name="patientName">Patient name to include in filename.</param>
  /// <returns>Sanitized filename with timestamp.</returns>
  public string GenerateFilename(string baseType, string patientName)
  {
    var sanitizedName = new string(patientName
        .Where(c => char.IsLetterOrDigit(c) || c == ' ')
        .ToArray())
        .Replace(" ", "_");

    var timestamp = DateTime.Now.ToString("yyyy-MM-dd");
    return $"PFPT_{baseType}_{sanitizedName}_{timestamp}.pdf";
  }

  /// <summary>
  /// Opens a PDF in a new window for printing.
  /// </summary>
  /// <param name="pdfBytes">The PDF file bytes.</param>
  public async Task PrintPdfAsync(byte[] pdfBytes)
  {
    try
    {
      await _jsRuntime.InvokeVoidAsync("pdfExport.openPrintDialog", pdfBytes);
    }
    catch (Exception ex)
    {
      Debug.WriteLine($"Failed to open print dialog: {ex.Message}");
      throw;
    }
  }

  /// <summary>
  /// Generates a secure random password for PDF protection.
  /// </summary>
  /// <returns>12-character secure password.</returns>
  public string GenerateSecurePassword()
  {
    const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnpqrstuvwxyz23456789!@#$%";
    var password = new StringBuilder(12);
    var randomBytes = new byte[12];

    using (var rng = RandomNumberGenerator.Create())
    {
      rng.GetBytes(randomBytes);
    }

    for (int i = 0; i < 12; i++)
    {
      password.Append(chars[randomBytes[i] % chars.Length]);
    }

    return password.ToString();
  }

  /// <summary>
  /// Checks if the browser supports PDF downloads.
  /// </summary>
  /// <returns>True if PDF downloads are supported.</returns>
  public async Task<bool> IsPdfDownloadSupportedAsync()
  {
    try
    {
      return await _jsRuntime.InvokeAsync<bool>("pdfExport.isDownloadSupported");
    }
    catch
    {
      return false;
    }
  }
}

/// <summary>
/// Options for PDF export configuration.
/// </summary>
public class ExportOptions
{
  /// <summary>
  /// Gets or sets the export format.
  /// </summary>
  public string Format { get; set; } = "pdf";

  /// <summary>
  /// Gets or sets a value indicating whether to include signature line.
  /// </summary>
  public bool IncludeSignature { get; set; } = true;

  /// <summary>
  /// Gets or sets a value indicating whether to include timestamp.
  /// </summary>
  public bool IncludeTimestamp { get; set; } = true;

  /// <summary>
  /// Gets or sets a value indicating whether to include watermark.
  /// </summary>
  public bool IncludeWatermark { get; set; } = true;

  /// <summary>
  /// Gets or sets the PDF password (if encryption is desired).
  /// </summary>
  public string? Password { get; set; }
}

/// <summary>
/// Base data structure for SOAP note exports.
/// </summary>
public class SoapNoteExportData
{
  /// <summary>
  /// Gets or sets the note ID.
  /// </summary>
  public string? Id { get; set; }

  /// <summary>
  /// Gets or sets the patient name.
  /// </summary>
  public required string PatientName { get; set; }

  /// <summary>
  /// Gets or sets the patient date of birth.
  /// </summary>
  public required string PatientDob { get; set; }

  /// <summary>
  /// Gets or sets the patient medical record number.
  /// </summary>
  public string? PatientMrn { get; set; }

  /// <summary>
  /// Gets or sets the visit date.
  /// </summary>
  public required string VisitDate { get; set; }

  /// <summary>
  /// Gets or sets the clinician name.
  /// </summary>
  public required string ClinicianName { get; set; }

  /// <summary>
  /// Gets or sets the clinician credentials.
  /// </summary>
  public string? ClinicianCredentials { get; set; }

  /// <summary>
  /// Gets or sets the subjective section data.
  /// </summary>
  public required SubjectiveSection Subjective { get; set; }

  /// <summary>
  /// Gets or sets the objective section data.
  /// </summary>
  public required ObjectiveSection Objective { get; set; }

  /// <summary>
  /// Gets or sets the assessment section data.
  /// </summary>
  public required AssessmentSection Assessment { get; set; }

  /// <summary>
  /// Gets or sets the plan section data.
  /// </summary>
  public required PlanSection Plan { get; set; }
}

/// <summary>
/// Subjective section of SOAP note.
/// </summary>
public class SubjectiveSection
{
  /// <summary>
  /// Gets or sets the chief complaint.
  /// </summary>
  public required string ChiefComplaint { get; set; }

  /// <summary>
  /// Gets or sets the pain level(s).
  /// </summary>
  public required List<int> PainLevel { get; set; }

  /// <summary>
  /// Gets or sets the pain location.
  /// </summary>
  public required string PainLocation { get; set; }

  /// <summary>
  /// Gets or sets the pain description.
  /// </summary>
  public required string PainDescription { get; set; }

  /// <summary>
  /// Gets or sets the functional limitations.
  /// </summary>
  public required string Limitations { get; set; }

  /// <summary>
  /// Gets or sets information about previous treatment.
  /// </summary>
  public required string PreviousTreatment { get; set; }

  /// <summary>
  /// Gets or sets the list of associated symptoms.
  /// </summary>
  public required List<string> Symptoms { get; set; }
}

/// <summary>
/// Objective section of SOAP note.
/// </summary>
public class ObjectiveSection
{
  /// <summary>
  /// Gets or sets the range of motion.
  /// </summary>
  public required string Rom { get; set; }

  /// <summary>
  /// Gets or sets the manual muscle testing results.
  /// </summary>
  public required string Mmt { get; set; }

  /// <summary>
  /// Gets or sets the special tests performed.
  /// </summary>
  public required string SpecialTests { get; set; }

  /// <summary>
  /// Gets or sets clinical observations.
  /// </summary>
  public required string Observations { get; set; }

  /// <summary>
  /// Gets or sets objective measurements.
  /// </summary>
  public required string Measurements { get; set; }

  /// <summary>
  /// Gets or sets posture findings.
  /// </summary>
  public required string Posture { get; set; }

  /// <summary>
  /// Gets or sets gait analysis notes.
  /// </summary>
  public required string Gait { get; set; }

  /// <summary>
  /// Gets or sets prescribed exercises.
  /// </summary>
  public List<ExerciseItem> Exercises { get; set; } = new();

  /// <summary>
  /// Gets or sets manual therapy techniques.
  /// </summary>
  public List<ManualTechniqueItem> ManualTechniques { get; set; } = new();
}

/// <summary>
/// Assessment section of SOAP note.
/// </summary>
public class AssessmentSection
{
  /// <summary>
  /// Gets or sets the diagnosis.
  /// </summary>
  public required string Diagnosis { get; set; }

  /// <summary>
  /// Gets or sets assessment goals.
  /// </summary>
  public required List<string> Goals { get; set; }

  /// <summary>
  /// Gets or sets the prognosis.
  /// </summary>
  public required string Prognosis { get; set; }

  /// <summary>
  /// Gets or sets the assessment summary.
  /// </summary>
  public required string Summary { get; set; }
}

/// <summary>
/// Plan section of SOAP note.
/// </summary>
public class PlanSection
{
  /// <summary>
  /// Gets or sets the CPT codes.
  /// </summary>
  public required List<string> CptCodes { get; set; }

  /// <summary>
  /// Gets or sets treatment frequency.
  /// </summary>
  public required string Frequency { get; set; }

  /// <summary>
  /// Gets or sets treatment duration.
  /// </summary>
  public required string Duration { get; set; }

  /// <summary>
  /// Gets or sets the home exercise program (HEP).
  /// </summary>
  public required string Hep { get; set; }

  /// <summary>
  /// Gets or sets the discharge plan.
  /// </summary>
  public required string DischargePlan { get; set; }

  /// <summary>
  /// Gets or sets the follow-up plan.
  /// </summary>
  public required string FollowUp { get; set; }
}

/// <summary>
/// Exercise item for objective section.
/// </summary>
public class ExerciseItem
{
  /// <summary>
  /// Gets or sets the exercise name.
  /// </summary>
  public required string Name { get; set; }

  /// <summary>
  /// Gets or sets the number of sets.
  /// </summary>
  public int? Sets { get; set; }

  /// <summary>
  /// Gets or sets the number of repetitions.
  /// </summary>
  public int? Reps { get; set; }

  /// <summary>
  /// Gets or sets the exercise frequency.
  /// </summary>
  public string? Frequency { get; set; }

  /// <summary>
  /// Gets or sets additional notes.
  /// </summary>
  public string? Notes { get; set; }
}

/// <summary>
/// Manual technique item for objective section.
/// </summary>
public class ManualTechniqueItem
{
  /// <summary>
  /// Gets or sets the technique name.
  /// </summary>
  public required string Name { get; set; }

  /// <summary>
  /// Gets or sets technique duration.
  /// </summary>
  public string? Duration { get; set; }

  /// <summary>
  /// Gets or sets additional notes.
  /// </summary>
  public string? Notes { get; set; }
}

/// <summary>
/// Data structure for goals and interventions export.
/// </summary>
public class GoalsExportData
{
  /// <summary>
  /// Gets or sets the patient name.
  /// </summary>
  public required string PatientName { get; set; }

  /// <summary>
  /// Gets or sets the patient's date of birth.
  /// </summary>
  public required string PatientDob { get; set; }

  /// <summary>
  /// Gets or sets the clinician name.
  /// </summary>
  public required string ClinicianName { get; set; }

  /// <summary>
  /// Gets or sets the creation date.
  /// </summary>
  public required string CreatedDate { get; set; }

  /// <summary>
  /// Gets or sets the collection of goals.
  /// </summary>
  public required List<GoalItem> Goals { get; set; }
}

/// <summary>
/// Individual goal item.
/// </summary>
public class GoalItem
{
  /// <summary>
  /// Gets or sets the goal identifier.
  /// </summary>
  public required string Id { get; set; }

  /// <summary>
  /// Gets or sets the goal description.
  /// </summary>
  public required string Description { get; set; }

  /// <summary>
  /// Gets or sets the goal type.
  /// </summary>
  public required string Type { get; set; }

  /// <summary>
  /// Gets or sets the target date.
  /// </summary>
  public required string TargetDate { get; set; }

  /// <summary>
  /// Gets or sets the goal status.
  /// </summary>
  public required string Status { get; set; }

  /// <summary>
  /// Gets or sets progress percentage.
  /// </summary>
  public int? Progress { get; set; }

  /// <summary>
  /// Gets or sets associated interventions.
  /// </summary>
  public List<string>? Interventions { get; set; }
}

/// <summary>
/// Data structure for intake form export.
/// </summary>
public class IntakeFormExportData
{
  /// <summary>
  /// Gets or sets the patient's full name.
  /// </summary>
  public required string FullName { get; set; }

  /// <summary>
  /// Gets or sets the patient's date of birth.
  /// </summary>
  public required string Birthday { get; set; }

  /// <summary>
  /// Gets or sets the patient's email address.
  /// </summary>
  public required string Email { get; set; }

  /// <summary>
  /// Gets or sets the patient's phone number.
  /// </summary>
  public required string PhoneNumber { get; set; }

  /// <summary>
  /// Gets or sets the emergency contact.
  /// </summary>
  public required string EmergencyContact { get; set; }

  /// <summary>
  /// Gets or sets the emergency contact phone.
  /// </summary>
  public required string EmergencyPhone { get; set; }

  /// <summary>
  /// Gets or sets the primary body part of concern.
  /// </summary>
  public required string PrimaryBodyPart { get; set; }

  /// <summary>
  /// Gets or sets elaboration of the chief complaint.
  /// </summary>
  public required string ChiefComplaintElaboration { get; set; }

  /// <summary>
  /// Gets or sets the list of associated symptoms.
  /// </summary>
  public required List<string> AssociatedSymptoms { get; set; }

  /// <summary>
  /// Gets or sets the list of functional limitations.
  /// </summary>
  public required List<string> FunctionalLimitations { get; set; }

  /// <summary>
  /// Gets or sets the medical history section.
  /// </summary>
  public required MedicalHistorySection MedicalHistory { get; set; }
}

/// <summary>
/// Medical history section of intake form.
/// </summary>
public class MedicalHistorySection
{
  /// <summary>
  /// Gets or sets a value indicating whether the patient takes medications.
  /// </summary>
  public bool Medications { get; set; }

  /// <summary>
  /// Gets or sets details about medications.
  /// </summary>
  public string? MedicationsDetails { get; set; }

  /// <summary>
  /// Gets or sets a value indicating whether comorbidities are present.
  /// </summary>
  public bool Comorbidities { get; set; }

  /// <summary>
  /// Gets or sets details about comorbidities.
  /// </summary>
  public string? ComorbiditiesDetails { get; set; }

  /// <summary>
  /// Gets or sets a value indicating whether assistive devices are used.
  /// </summary>
  public bool AssistiveDevices { get; set; }

  /// <summary>
  /// Gets or sets details about assistive devices.
  /// </summary>
  public string? AssistiveDevicesDetails { get; set; }

  /// <summary>
  /// Gets or sets a value indicating whether previous surgeries occurred.
  /// </summary>
  public bool PreviousSurgeries { get; set; }

  /// <summary>
  /// Gets or sets details about previous surgeries.
  /// </summary>
  public string? PreviousSurgeriesDetails { get; set; }
}
