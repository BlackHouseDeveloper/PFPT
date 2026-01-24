# Accessibility Utilities Usage Guide

## Overview

The `AccessibilityUtilities` service provides WCAG 2.1 AA compliant accessibility helpers for Blazor components, including screen reader announcements, focus management, keyboard navigation, and ARIA label generation.

## Setup

The accessibility utilities are automatically registered in the DI container and the JavaScript interop module is loaded in `index.html`.

## Injecting the Service

Add the `@inject` directive at the top of your `.razor` component:

```razor
@inject AccessibilityUtilities Accessibility
```

## Core Features

### 1. Screen Reader Announcements

Announce messages to screen reader users using ARIA live regions:

```csharp
// Polite announcement (waits for screen reader to pause)
await Accessibility.AnnounceToScreenReaderAsync(
    "Appointment saved successfully", 
    ScreenReaderPriority.Polite
);

// Assertive announcement (interrupts immediately)
await Accessibility.AnnounceToScreenReaderAsync(
    "Error: Invalid date selected", 
    ScreenReaderPriority.Assertive
);
```

**When to use:**
- Form submission confirmations
- Data save/update notifications
- Error messages
- Dynamic content updates

### 2. Focus Management

#### Trap Focus in Modals/Dialogs

Prevent keyboard users from tabbing outside modal dialogs:

```csharp
private string? _focusTrapId;

private async Task OpenModal()
{
    // Trap focus when modal opens
    _focusTrapId = await Accessibility.TrapFocusAsync("modal-dialog-id");
}

private async Task CloseModal()
{
    // Restore focus when modal closes
    if (_focusTrapId != null)
    {
        await Accessibility.RestoreFocusAsync(_focusTrapId);
        _focusTrapId = null;
    }
}
```

**HTML structure:**
```html
<div id="modal-dialog-id" role="dialog" aria-modal="true">
    <!-- Modal content with focusable elements -->
</div>
```

#### Restore Focus After Actions

Return focus to the triggering element after closing overlays:

```csharp
await Accessibility.RestoreFocusAsync("triggering-button-id");
```

### 3. ARIA Label Generation

Use the static `AriaLabels` class to generate semantic, consistent ARIA labels:

```razor
<!-- Buttons -->
<button aria-label="@AriaLabels.Button.Save()">Save</button>
<button aria-label="@AriaLabels.Button.Edit("Visit Type")">Edit</button>
<button aria-label="@AriaLabels.Button.Delete("Appointment")">Delete</button>
<button aria-label="@AriaLabels.Button.Cancel()">Cancel</button>
<button aria-label="@AriaLabels.Button.Close()">×</button>

<!-- Forms -->
<input type="text" aria-label="@AriaLabels.Form.Required("Patient Name")" />
<select aria-label="@AriaLabels.Form.Optional("Visit Type")">...</select>
<input type="email" aria-label="@AriaLabels.Form.Email()" />
<input type="tel" aria-label="@AriaLabels.Form.Phone()" />
<input type="date" aria-label="@AriaLabels.Form.Date("Appointment Date")" />

<!-- Status indicators -->
<span aria-label="@AriaLabels.Status.Active()">●</span>
<span aria-label="@AriaLabels.Status.Inactive()">○</span>
<span aria-label="@AriaLabels.Status.Loading()">⏳</span>

<!-- Progress -->
<div aria-label="@AriaLabels.Progress(3, 5)">Step 3 of 5</div>

<!-- Clinical entities -->
<h2 aria-label="@AriaLabels.Note("SOAP", patient.Name)">Note</h2>
<div aria-label="@AriaLabels.Appointment(appt.DateTime, patient.Name)">...</div>
```

### 4. Keyboard Navigation

Add keyboard handlers for common interactions:

```csharp
protected override async Task OnAfterRenderAsync(bool firstRender)
{
    if (firstRender)
    {
        await Accessibility.AddKeyboardHandlersAsync("custom-component-id", new KeyboardHandlerConfig
        {
            OnEnter = "handleEnterKey",      // JS function name
            OnEscape = "handleEscapeKey",
            OnSpace = "handleSpaceKey",
            OnArrowUp = "handleArrowUp",
            OnArrowDown = "handleArrowDown",
            OnArrowLeft = "handleArrowLeft",
            OnArrowRight = "handleArrowRight"
        });
    }
}
```

**JavaScript handlers in your component:**
```javascript
window.handleEnterKey = (event) => {
    event.preventDefault();
    // Handle Enter key press
};
```

### 5. Accessibility Validation

#### Check Element Accessibility

Verify an element is visible and accessible to assistive technology:

```csharp
bool isAccessible = await Accessibility.IsElementAccessibleAsync("patient-form");
if (!isAccessible)
{
    await Accessibility.AnnounceToScreenReaderAsync(
        "Form not ready. Please wait.", 
        ScreenReaderPriority.Assertive
    );
}
```

#### Validate Touch Target Sizes

Ensure interactive elements meet the 44x44px minimum size requirement:

```csharp
bool validSize = await Accessibility.ValidateTouchTargetSizeAsync("submit-button");
if (!validSize)
{
    // Log warning or adjust button size
    System.Diagnostics.Debug.WriteLine("Button does not meet touch target size requirements");
}
```

### 6. Live Regions

#### Update Existing Live Region

Update the content of a persistent live region:

```csharp
await Accessibility.UpdateLiveRegionAsync("status-region", "3 appointments today");
```

**HTML:**
```html
<div id="status-region" role="status" aria-live="polite" aria-atomic="true">
    <!-- Content updated dynamically -->
</div>
```

### 7. Skip Links

Create skip navigation links for keyboard users:

```csharp
protected override async Task OnAfterRenderAsync(bool firstRender)
{
    if (firstRender)
    {
        await Accessibility.CreateSkipLinkAsync("main-content", "Skip to main content");
    }
}
```

**HTML:**
```html
<main id="main-content">
    <!-- Main content -->
</main>
```

### 8. Unique ID Generation

Generate unique IDs for ARIA attributes:

```csharp
private string _labelId = string.Empty;
private string _descriptionId = string.Empty;

protected override void OnInitialized()
{
    _labelId = Accessibility.GenerateAriaId("label");
    _descriptionId = Accessibility.GenerateAriaId("description");
}
```

**HTML:**
```html
<label id="@_labelId">Patient Name</label>
<span id="@_descriptionId">Enter the patient's full legal name</span>
<input type="text" 
       aria-labelledby="@_labelId" 
       aria-describedby="@_descriptionId" />
```

## Common Usage Patterns

### Modal Dialog (Complete Example)

```razor
@inject AccessibilityUtilities Accessibility

<div class="modal-overlay" @onclick="CloseModal" style="display: @(_isOpen ? "flex" : "none")">
    <div id="visit-type-modal" 
         class="modal-content" 
         role="dialog" 
         aria-modal="true"
         aria-labelledby="@_titleId"
         @onclick:stopPropagation>
        
        <h2 id="@_titleId">Edit Visit Type</h2>
        
        <button @onclick="CloseModal" 
                aria-label="@AriaLabels.Button.Close()">
            ×
        </button>
        
        <!-- Modal content -->
    </div>
</div>

@code {
    private bool _isOpen = false;
    private string? _focusTrapId;
    private string _titleId = string.Empty;

    protected override void OnInitialized()
    {
        _titleId = Accessibility.GenerateAriaId("modal-title");
    }

    private async Task OpenModal()
    {
        _isOpen = true;
        StateHasChanged();
        
        await Task.Delay(100); // Wait for render
        _focusTrapId = await Accessibility.TrapFocusAsync("visit-type-modal");
        
        await Accessibility.AnnounceToScreenReaderAsync(
            "Visit type editor opened", 
            ScreenReaderPriority.Polite
        );
    }

    private async Task CloseModal()
    {
        if (_focusTrapId != null)
        {
            await Accessibility.RestoreFocusAsync(_focusTrapId);
            _focusTrapId = null;
        }
        
        _isOpen = false;
        
        await Accessibility.AnnounceToScreenReaderAsync(
            "Visit type editor closed", 
            ScreenReaderPriority.Polite
        );
    }
}
```

### Form Submission with Feedback

```razor
@inject AccessibilityUtilities Accessibility

<form @onsubmit="HandleSubmit">
    <label for="patient-name" id="@_labelId">
        Patient Name @AriaLabels.Form.Required("")
    </label>
    <input id="patient-name" 
           type="text" 
           aria-labelledby="@_labelId"
           @bind="PatientName" />
    
    <button type="submit" aria-label="@AriaLabels.Button.Save()">
        Save
    </button>
</form>

<div id="save-status" role="status" aria-live="polite" class="sr-only"></div>

@code {
    private string _labelId = string.Empty;
    private string PatientName { get; set; } = string.Empty;

    protected override void OnInitialized()
    {
        _labelId = Accessibility.GenerateAriaId("label");
    }

    private async Task HandleSubmit()
    {
        // Validate
        bool isValid = !string.IsNullOrWhiteSpace(PatientName);
        
        if (!isValid)
        {
            await Accessibility.AnnounceToScreenReaderAsync(
                "Error: Patient name is required", 
                ScreenReaderPriority.Assertive
            );
            return;
        }
        
        // Save logic...
        await Task.Delay(500);
        
        await Accessibility.UpdateLiveRegionAsync(
            "save-status", 
            "Patient saved successfully"
        );
        
        await Accessibility.AnnounceToScreenReaderAsync(
            $"Patient {PatientName} saved successfully", 
            ScreenReaderPriority.Polite
        );
    }
}
```

### Dynamic List Updates

```razor
@inject AccessibilityUtilities Accessibility

<div role="region" aria-labelledby="appointments-heading">
    <h2 id="appointments-heading">Today's Appointments</h2>
    
    <div id="appointment-count" role="status" aria-live="polite" class="sr-only">
        @_appointments.Count appointments
    </div>
    
    <ul>
        @foreach (var appt in _appointments)
        {
            <li aria-label="@AriaLabels.Appointment(appt.DateTime, appt.PatientName)">
                @appt.Time - @appt.PatientName
            </li>
        }
    </ul>
</div>

@code {
    private List<Appointment> _appointments = new();

    private async Task RefreshAppointments()
    {
        // Fetch appointments...
        _appointments = await DataService.GetAppointmentsAsync();
        
        StateHasChanged();
        
        await Accessibility.UpdateLiveRegionAsync(
            "appointment-count", 
            $"{_appointments.Count} appointments"
        );
    }
}
```

## CSS for Screen Reader Only Content

Add this to your component CSS for content that should only be announced to screen readers:

```css
.sr-only {
    position: absolute;
    width: 1px;
    height: 1px;
    padding: 0;
    margin: -1px;
    overflow: hidden;
    clip: rect(0, 0, 0, 0);
    white-space: nowrap;
    border: 0;
}
```

## Best Practices

1. **Always provide ARIA labels** for icon-only buttons and interactive elements
2. **Use semantic HTML** (`<button>`, `<nav>`, `<main>`, `<article>`) when possible
3. **Trap focus in modal dialogs** to prevent keyboard users from escaping
4. **Announce dynamic content changes** to screen reader users
5. **Validate touch target sizes** (minimum 44x44px for mobile)
6. **Use polite announcements** for non-critical updates
7. **Use assertive announcements** only for errors or urgent information
8. **Test with keyboard navigation** (Tab, Enter, Escape, Arrow keys)
9. **Test with screen readers** (NVDA, JAWS, VoiceOver)
10. **Provide skip links** for keyboard users to bypass repetitive navigation

## Testing Checklist

- [ ] All interactive elements have descriptive ARIA labels
- [ ] Modal dialogs trap focus and restore focus on close
- [ ] Form submissions announce success/error messages
- [ ] Dynamic content changes are announced to screen readers
- [ ] All buttons meet 44x44px minimum touch target size
- [ ] Keyboard navigation works (Tab, Shift+Tab, Enter, Escape)
- [ ] Screen reader announces all important state changes
- [ ] Skip links are present and functional
- [ ] Color contrast meets WCAG AA standards (4.5:1 for text)
- [ ] No keyboard traps (user can always escape with keyboard)

## Troubleshooting

### Screen Reader Not Announcing

1. Check that `accessibility.js` is loaded in `index.html`
2. Verify the element ID matches exactly (case-sensitive)
3. Use browser DevTools to confirm ARIA attributes are rendered
4. Try both `Polite` and `Assertive` priorities
5. Check browser console for JavaScript errors

### Focus Not Trapping

1. Ensure the modal element has `role="dialog"` and `aria-modal="true"`
2. Verify there are focusable elements inside the modal
3. Check that `TrapFocusAsync()` is called after the element is rendered
4. Use `await Task.Delay(100)` before calling if needed

### Touch Target Validation Failing

1. Check CSS for `width` and `height` properties (must be >= 44px)
2. Verify `padding` is not causing visual size to differ from computed size
3. Use browser DevTools to inspect actual element dimensions
4. Consider adding `min-width` and `min-height` in CSS

## Related Documentation

- [WCAG 2.1 Guidelines](https://www.w3.org/WAI/WCAG21/quickref/)
- [ARIA Authoring Practices](https://www.w3.org/WAI/ARIA/apg/)
- [MDN Accessibility](https://developer.mozilla.org/en-US/docs/Web/Accessibility)
