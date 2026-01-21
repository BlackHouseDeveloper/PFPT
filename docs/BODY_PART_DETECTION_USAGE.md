# Body Part Detection Service Usage Guide

## Overview

The `BodyPartDetectionService` provides intelligent detection of body parts from clinical text and returns region-specific clinical data including pain locations, functional limitations, ROM measurements, MMT muscles, and special orthopedic tests.

## Setup

The service is automatically registered as a singleton in the DI container.

## Injecting the Service

Add the `@inject` directive at the top of your `.razor` component:

```razor
@inject BodyPartDetectionService BodyPartService
```

## Core Features

### 1. Automatic Body Part Detection

Detect the primary body part from a chief complaint or clinical text:

```csharp
string chiefComplaint = "Right shoulder pain with overhead reaching";
BodyPartRegion? detected = BodyPartService.DetectBodyPart(chiefComplaint);

if (detected.HasValue)
{
    // Body part detected: BodyPartRegion.Shoulder
    var clinicalData = BodyPartService.GetBodyPartData(detected.Value);
}
```

**Supported keywords:**
- **Shoulder**: "shoulder"
- **Knee**: "knee"
- **Hip**: "hip"
- **Ankle**: "ankle", "foot"
- **Elbow**: "elbow"
- **Wrist**: "wrist", "hand"
- **Neck**: "neck", "cervical"
- **Back**: "back", "lumbar", "spine"

**Detection priority:** Keywords are checked in order of specificity (e.g., "shoulder" before "back").

### 2. Get Clinical Data for Body Part

Retrieve comprehensive clinical data once a body part is identified:

```csharp
BodyPartRegion region = BodyPartRegion.Shoulder;
BodyPartData? data = BodyPartService.GetBodyPartData(region);

if (data != null)
{
    // data.Name: "Shoulder"
    // data.PainLocations: List of 8 anatomical locations
    // data.FunctionalLimitations: List of 10 common limitations
    // data.RomMeasurements: List of 7 ROM measurements
    // data.MmtMuscles: List of 12 muscles to test
    // data.SpecialTests: List of 10 orthopedic tests
}
```

### 3. Get All Body Part Regions

List all supported body parts for UI dropdowns or selection:

```csharp
var allRegions = BodyPartService.GetAllBodyPartRegions();
// Returns: Shoulder, Knee, Hip, Back, Neck, Ankle, Elbow, Wrist
```

### 4. Get Previous Note Data (Mock)

Retrieve previous note data for carry-forward functionality:

```csharp
string patientId = "12345";
string noteType = "evaluation";
PreviousNoteData? previousNote = BodyPartService.GetPreviousNoteData(patientId, noteType);

// Access previous data:
// previousNote.ChiefComplaint
// previousNote.PainLevel (List<int>)
// previousNote.PainLocation
// previousNote.Diagnosis
// previousNote.Goals (List<string>)
```

**Note:** This currently returns mock data. In production, this would query the database for the patient's most recent note of the specified type.

## Complete SOAP Note Builder Example

```razor
@page "/soap-builder"
@inject BodyPartDetectionService BodyPartService
@inject AccessibilityUtilities Accessibility

<div class="soap-builder">
    <h1>SOAP Note Builder</h1>
    
    <!-- Subjective Section -->
    <section aria-labelledby="subjective-heading">
        <h2 id="subjective-heading">Subjective</h2>
        
        <label for="chief-complaint">Chief Complaint</label>
        <textarea id="chief-complaint" 
                  @bind="ChiefComplaint"
                  @bind:event="oninput"
                  @onblur="DetectBodyPartFromComplaint"
                  rows="3"></textarea>
        
        @if (DetectedBodyPart.HasValue)
        {
            <div class="detection-notice" role="status">
                ✓ Detected body part: <strong>@GetBodyPartName(DetectedBodyPart.Value)</strong>
            </div>
        }
    </section>
    
    <!-- Objective Section -->
    @if (DetectedBodyPart.HasValue && ClinicalData != null)
    {
        <section aria-labelledby="objective-heading">
            <h2 id="objective-heading">Objective</h2>
            
            <!-- Pain Location -->
            <div class="form-group">
                <label for="pain-location">Pain Location</label>
                <select id="pain-location" @bind="SelectedPainLocation">
                    <option value="">Select location...</option>
                    @foreach (var location in ClinicalData.PainLocations)
                    {
                        <option value="@location">@location</option>
                    }
                </select>
            </div>
            
            <!-- Functional Limitations -->
            <div class="form-group">
                <label>Functional Limitations (select multiple)</label>
                <div class="checkbox-group">
                    @foreach (var limitation in ClinicalData.FunctionalLimitations)
                    {
                        <label class="checkbox-label">
                            <input type="checkbox" 
                                   value="@limitation"
                                   @onchange="e => ToggleLimitation(limitation, e)" />
                            @limitation
                        </label>
                    }
                </div>
            </div>
            
            <!-- Range of Motion -->
            <div class="form-group">
                <label>Range of Motion</label>
                <div class="rom-measurements">
                    @foreach (var rom in ClinicalData.RomMeasurements)
                    {
                        <div class="rom-item">
                            <span class="rom-label">@rom</span>
                            <input type="text" 
                                   placeholder="Measured value"
                                   aria-label="@rom measurement" />
                        </div>
                    }
                </div>
            </div>
            
            <!-- Manual Muscle Testing -->
            <div class="form-group">
                <label>Manual Muscle Testing</label>
                <div class="mmt-grid">
                    @foreach (var muscle in ClinicalData.MmtMuscles)
                    {
                        <div class="mmt-item">
                            <label>@muscle</label>
                            <select aria-label="@muscle strength">
                                <option value="">Grade</option>
                                <option value="0/5">0/5 - No contraction</option>
                                <option value="1/5">1/5 - Trace</option>
                                <option value="2/5">2/5 - Poor</option>
                                <option value="3/5">3/5 - Fair</option>
                                <option value="4/5">4/5 - Good</option>
                                <option value="5/5">5/5 - Normal</option>
                            </select>
                        </div>
                    }
                </div>
            </div>
            
            <!-- Special Tests -->
            <div class="form-group">
                <label>Special Tests</label>
                <div class="special-tests">
                    @foreach (var test in ClinicalData.SpecialTests)
                    {
                        <div class="test-item">
                            <label class="test-name">@test</label>
                            <div class="test-result">
                                <label>
                                    <input type="radio" 
                                           name="test-@test" 
                                           value="positive" />
                                    Positive
                                </label>
                                <label>
                                    <input type="radio" 
                                           name="test-@test" 
                                           value="negative" />
                                    Negative
                                </label>
                                <label>
                                    <input type="radio" 
                                           name="test-@test" 
                                           value="not-performed" 
                                           checked />
                                    Not performed
                                </label>
                            </div>
                        </div>
                    }
                </div>
            </div>
        </section>
    }
    
    <button @onclick="SaveNote" class="save-button">
        Save SOAP Note
    </button>
</div>

@code {
    private string ChiefComplaint { get; set; } = string.Empty;
    private BodyPartRegion? DetectedBodyPart { get; set; }
    private BodyPartData? ClinicalData { get; set; }
    private string? SelectedPainLocation { get; set; }
    private HashSet<string> SelectedLimitations { get; set; } = new();

    private async Task DetectBodyPartFromComplaint()
    {
        var detected = BodyPartService.DetectBodyPart(ChiefComplaint);
        
        if (detected.HasValue && detected != DetectedBodyPart)
        {
            DetectedBodyPart = detected;
            ClinicalData = BodyPartService.GetBodyPartData(detected.Value);
            
            await Accessibility.AnnounceToScreenReaderAsync(
                $"Body part detected: {GetBodyPartName(detected.Value)}. Clinical data loaded.",
                ScreenReaderPriority.Polite
            );
        }
    }

    private string GetBodyPartName(BodyPartRegion region)
    {
        var data = BodyPartService.GetBodyPartData(region);
        return data?.Name ?? region.ToString();
    }

    private void ToggleLimitation(string limitation, ChangeEventArgs e)
    {
        bool isChecked = (bool)(e.Value ?? false);
        
        if (isChecked)
        {
            SelectedLimitations.Add(limitation);
        }
        else
        {
            SelectedLimitations.Remove(limitation);
        }
    }

    private async Task SaveNote()
    {
        // Save logic...
        await Accessibility.AnnounceToScreenReaderAsync(
            "SOAP note saved successfully",
            ScreenReaderPriority.Polite
        );
    }
}
```

## Smart Dropdown Population Example

Dynamically populate dropdowns based on detected body part:

```razor
@inject BodyPartDetectionService BodyPartService

<div class="smart-form">
    <label for="chief-complaint">Chief Complaint</label>
    <input id="chief-complaint" 
           type="text" 
           @bind="ChiefComplaint"
           @bind:event="oninput"
           @onblur="OnChiefComplaintChanged" />
    
    @if (BodyPartData != null)
    {
        <label for="pain-location">Pain Location (specific to @BodyPartData.Name)</label>
        <select id="pain-location" @bind="PainLocation">
            <option value="">Select location...</option>
            @foreach (var location in BodyPartData.PainLocations)
            {
                <option value="@location">@location</option>
            }
        </select>
    }
</div>

@code {
    private string ChiefComplaint { get; set; } = string.Empty;
    private string PainLocation { get; set; } = string.Empty;
    private BodyPartData? BodyPartData { get; set; }

    private void OnChiefComplaintChanged()
    {
        var detected = BodyPartService.DetectBodyPart(ChiefComplaint);
        if (detected.HasValue)
        {
            BodyPartData = BodyPartService.GetBodyPartData(detected.Value);
        }
    }
}
```

## Previous Note Carry-Forward Example

Pre-fill form fields with data from previous note:

```razor
@inject BodyPartDetectionService BodyPartService

<div class="carry-forward-section">
    <button @onclick="LoadPreviousNote" class="btn-secondary">
        Carry Forward From Previous Note
    </button>
    
    @if (PreviousNote != null)
    {
        <div class="previous-data" role="region" aria-label="Previous note data">
            <h3>Previous Evaluation (@PreviousNote.ChiefComplaint)</h3>
            
            <div class="data-item">
                <strong>Pain Level:</strong> @string.Join(", ", PreviousNote.PainLevel)/10
            </div>
            
            <div class="data-item">
                <strong>Pain Location:</strong> @PreviousNote.PainLocation
            </div>
            
            <div class="data-item">
                <strong>Diagnosis:</strong> @PreviousNote.Diagnosis
            </div>
            
            <div class="data-item">
                <strong>Goals:</strong>
                <ul>
                    @foreach (var goal in PreviousNote.Goals)
                    {
                        <li>@goal</li>
                    }
                </ul>
            </div>
            
            <button @onclick="ApplyPreviousData" class="btn-primary">
                Apply to Current Note
            </button>
        </div>
    }
</div>

@code {
    private PreviousNoteData? PreviousNote { get; set; }
    private string PatientId { get; set; } = "12345"; // From route or state

    private void LoadPreviousNote()
    {
        PreviousNote = BodyPartService.GetPreviousNoteData(PatientId, "evaluation");
    }

    private void ApplyPreviousData()
    {
        if (PreviousNote != null)
        {
            // Apply previous note data to current form fields
            // ChiefComplaint = PreviousNote.ChiefComplaint;
            // PainLocation = PreviousNote.PainLocation;
            // etc.
        }
    }
}
```

## Body Part Selector Component

Create a reusable body part selector:

```razor
<div class="body-part-selector">
    <label for="body-part">Body Part</label>
    <select id="body-part" @bind="SelectedRegion" @bind:after="OnBodyPartChanged">
        <option value="">Detect automatically or select...</option>
        @foreach (var region in AllRegions)
        {
            <option value="@region">@GetBodyPartName(region)</option>
        }
    </select>
    
    @if (SelectedRegion.HasValue)
    {
        <div class="body-part-info">
            <strong>Available tests:</strong> @TestCount special tests
        </div>
    }
</div>

@code {
    [Parameter] public EventCallback<BodyPartRegion?> OnRegionChanged { get; set; }
    
    private BodyPartRegion? SelectedRegion { get; set; }
    private List<BodyPartRegion> AllRegions { get; set; } = new();
    private int TestCount { get; set; }

    protected override void OnInitialized()
    {
        AllRegions = BodyPartService.GetAllBodyPartRegions().ToList();
    }

    private async Task OnBodyPartChanged()
    {
        if (SelectedRegion.HasValue)
        {
            var data = BodyPartService.GetBodyPartData(SelectedRegion.Value);
            TestCount = data?.SpecialTests.Count ?? 0;
        }
        
        await OnRegionChanged.InvokeAsync(SelectedRegion);
    }

    private string GetBodyPartName(BodyPartRegion region)
    {
        var data = BodyPartService.GetBodyPartData(region);
        return data?.Name ?? region.ToString();
    }
}
```

## Clinical Data Reference

### Shoulder
- **Pain Locations**: 8 options (anterior, posterior, lateral, AC joint, etc.)
- **Functional Limitations**: 10 activities (overhead reaching, dressing, sleeping, etc.)
- **ROM Measurements**: 7 movements (flexion 0-180°, abduction 0-180°, etc.)
- **MMT Muscles**: 12 muscles (deltoid, rotator cuff, biceps, trapezius, etc.)
- **Special Tests**: 10 tests (empty can, Hawkins-Kennedy, Neer, Speeds, etc.)

### Knee
- **Pain Locations**: 9 options (anterior, medial/lateral joint line, patella, etc.)
- **Functional Limitations**: 11 activities (stairs, squatting, prolonged sitting, etc.)
- **ROM Measurements**: 5 movements (flexion 0-135°, extension 0-5°, etc.)
- **MMT Muscles**: 8 muscle groups (quadriceps, hamstrings, hip muscles, etc.)
- **Special Tests**: 10 tests (Lachman, McMurray, valgus/varus stress, etc.)

### Hip
- **Pain Locations**: 7 options (anterior/groin, lateral, greater trochanter, etc.)
- **Functional Limitations**: 11 activities (walking, stairs, putting on shoes, etc.)
- **ROM Measurements**: 6 movements (flexion 0-120°, abduction 0-45°, etc.)
- **MMT Muscles**: 9 muscle groups (hip flexors, glutes, adductors, etc.)
- **Special Tests**: 9 tests (FABER, FADIR, Thomas, Trendelenburg, etc.)

### Lower Back
- **Pain Locations**: 8 options (central lumbar, paraspinals, SI joint, radicular, etc.)
- **Functional Limitations**: 11 activities (bending, lifting, prolonged sitting, etc.)
- **ROM Measurements**: 7 movements (flexion, extension, lateral flexion, rotation)
- **MMT Muscles**: 8 muscle groups (erector spinae, core, glutes, etc.)
- **Special Tests**: 9 tests (straight leg raise, slump, SI joint tests, etc.)

### Neck
- **Pain Locations**: 9 options (cervical spine, upper trap, radicular, occipital, etc.)
- **Functional Limitations**: 10 activities (looking over shoulder, desk work, sleeping, etc.)
- **ROM Measurements**: 6 movements (flexion 0-50°, rotation 0-80°, etc.)
- **MMT Muscles**: 9 muscles (paraspinals, trapezius, SCM, deep neck flexors, etc.)
- **Special Tests**: 8 tests (Spurling, distraction, vertebral artery, etc.)

### Ankle/Foot
- **Pain Locations**: 9 options (anterior/posterior ankle, Achilles, plantar foot, etc.)
- **Functional Limitations**: 11 activities (walking, stairs, balance, standing, etc.)
- **ROM Measurements**: 7 movements (dorsiflexion 0-20°, plantarflexion 0-50°, etc.)
- **MMT Muscles**: 8 muscles (tibialis anterior, gastro/soleus, peroneals, etc.)
- **Special Tests**: 9 tests (anterior drawer, Thompson, single leg heel raise, etc.)

### Elbow
- **Pain Locations**: 7 options (medial/lateral, anterior/posterior, olecranon, etc.)
- **Functional Limitations**: 10 activities (gripping, lifting, computer use, eating, etc.)
- **ROM Measurements**: 4 movements (flexion 0-150°, supination/pronation 0-80°)
- **MMT Muscles**: 8 muscles (biceps, triceps, wrist flexors/extensors, etc.)
- **Special Tests**: 6 tests (Cozen, Mill, golfer's elbow, valgus/varus stress, etc.)

### Wrist/Hand
- **Pain Locations**: 8 options (dorsal/volar wrist, carpal tunnel, thumb, fingers, etc.)
- **Functional Limitations**: 11 activities (gripping, writing, typing, fine motor, etc.)
- **ROM Measurements**: 7 movements (flexion 0-80°, extension 0-70°, grip strength, etc.)
- **MMT Muscles**: 8 muscle groups (wrist/finger flexors/extensors, intrinsics, etc.)
- **Special Tests**: 8 tests (Phalen, Tinel, Finkelstein, grip/pinch strength, etc.)

## Best Practices

1. **Auto-detection first**: Use `DetectBodyPart()` on chief complaint changes
2. **Allow manual override**: Provide a dropdown to manually select body part if auto-detection is incorrect
3. **Dynamic forms**: Show/hide sections based on detected body part
4. **Validation**: Ensure critical fields are completed before saving
5. **Screen reader announcements**: Announce when body part is detected and data is loaded
6. **Previous note integration**: Offer carry-forward for follow-up notes
7. **Database integration**: Replace `GetPreviousNoteData()` with actual database queries in production
8. **Customization**: Allow clinics to add custom options to dropdowns

## Future Enhancements

- **Multi-region support**: Detect and support multiple body parts in one note
- **ICD-10 mapping**: Auto-suggest diagnosis codes based on body part
- **CPT code integration**: Suggest billing codes based on tests performed
- **AI-powered detection**: Use NLP for more sophisticated body part and condition detection
- **Template generation**: Generate complete SOAP note templates based on body part
- **Evidence-based protocols**: Link to treatment protocols based on detected region

## Related Documentation

- [ACCESSIBILITY_USAGE.md](ACCESSIBILITY_USAGE.md) - Accessibility utilities for WCAG compliance
- [SOAP Note Builder Components](../src/PhysicallyFitPT.Shared/Components/Pages/) - UI components for clinical documentation
