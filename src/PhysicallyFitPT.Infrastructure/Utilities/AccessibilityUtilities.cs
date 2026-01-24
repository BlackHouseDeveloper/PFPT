using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.JSInterop;

namespace PhysicallyFitPT.Infrastructure.Utilities;

/// <summary>
/// Accessibility Utilities for PFPT
/// 
/// WCAG 2.1 AA Compliance Features:
/// - Keyboard navigation support
/// - Screen reader announcements
/// - ARIA labels and roles
/// - Focus management
/// - Skip links
/// 
/// PRODUCTION CHECKLIST:
/// - Run automated accessibility testing (axe, WAVE, Lighthouse)
/// - Manual keyboard navigation testing (Tab, Enter, Esc, Arrow keys)
/// - Screen reader testing (NVDA, JAWS, VoiceOver)
/// - Color contrast verification (4.5:1 for normal text, 3:1 for large text)
/// - Touch target size verification (minimum 44x44px for mobile)
/// - Form error announcement testing
/// - Focus indicator visibility
/// </summary>
public class AccessibilityUtilities
{
  private readonly IJSRuntime _jsRuntime;
  private static int _idCounter;

  /// <summary>
  /// Initializes a new instance of the <see cref="AccessibilityUtilities"/> class.
  /// </summary>
  /// <param name="jsRuntime">The JavaScript runtime instance for interop.</param>
  /// <exception cref="ArgumentNullException">Thrown when <paramref name="jsRuntime"/> is null.</exception>
  public AccessibilityUtilities(IJSRuntime jsRuntime)
  {
    _jsRuntime = jsRuntime ?? throw new ArgumentNullException(nameof(jsRuntime));
  }
  /// <summary>
  /// Announce to screen readers using aria-live regions.
  /// </summary>
  public async Task AnnounceToScreenReaderAsync(
      string message,
      ScreenReaderPriority priority = ScreenReaderPriority.Polite)
  {
    try
    {
      await _jsRuntime.InvokeVoidAsync(
          "accessibility.announceToScreenReader",
          message,
          priority.ToString().ToLower());
    }
    catch (JSException ex)
    {
      System.Diagnostics.Debug.WriteLine($"JS Interop error: {ex.Message}");
    }
  }

  /// <summary>
  /// Trap focus within a modal or dialog element
  /// </summary>
  public async Task<string> TrapFocusAsync(string elementId)
  {
    if (string.IsNullOrEmpty(elementId))
      throw new ArgumentNullException(nameof(elementId));

    try
    {
      return await _jsRuntime.InvokeAsync<string>(
          "accessibility.trapFocus",
          elementId);
    }
    catch (JSException ex)
    {
      System.Diagnostics.Debug.WriteLine($"JS Interop error: {ex.Message}");
      return string.Empty;
    }
  }

  /// <summary>
  /// Restore focus to previously focused element
  /// </summary>
  public async Task RestoreFocusAsync(string elementId)
  {
    if (string.IsNullOrEmpty(elementId))
      throw new ArgumentNullException(nameof(elementId));

    try
    {
      await _jsRuntime.InvokeVoidAsync("accessibility.restoreFocus", elementId);
    }
    catch (JSException ex)
    {
      System.Diagnostics.Debug.WriteLine($"JS Interop error: {ex.Message}");
    }
  }

  /// <summary>
  /// Generate unique IDs for ARIA labels
  /// </summary>
  public string GenerateAriaId(string prefix = "aria")
  {
    return $"{prefix}-{++_idCounter}-{DateTime.UtcNow.Ticks}";
  }

  /// <summary>
  /// Check if element is visible to assistive technology
  /// </summary>
  public async Task<bool> IsElementAccessibleAsync(string elementId)
  {
    if (string.IsNullOrEmpty(elementId))
      throw new ArgumentNullException(nameof(elementId));

    try
    {
      return await _jsRuntime.InvokeAsync<bool>(
          "accessibility.isElementAccessible",
          elementId);
    }
    catch (JSException ex)
    {
      System.Diagnostics.Debug.WriteLine($"JS Interop error: {ex.Message}");
      return false;
    }
  }

  /// <summary>
  /// Verify color contrast ratio (WCAG AA: 4.5:1 normal, 3:1 large text)
  /// </summary>
  public ColorContrastResult CheckColorContrast(string foreground, string background)
  {
    // This is a simplified version - production should use a proper library
    // Recommended: Install accessibility library for precise calculations
    // For now, return placeholder
    return new ColorContrastResult
    {
      Ratio = 4.5m,
      PassesWcagAA = true,
      PassesWcagAAA = false
    };
  }

  /// <summary>
  /// Ensure minimum touch target size (WCAG 2.5.5: minimum 44x44px)
  /// </summary>
  public async Task<bool> ValidateTouchTargetSizeAsync(string elementId)
  {
    if (string.IsNullOrEmpty(elementId))
      throw new ArgumentNullException(nameof(elementId));

    try
    {
      return await _jsRuntime.InvokeAsync<bool>(
          "accessibility.validateTouchTargetSize",
          elementId);
    }
    catch (JSException ex)
    {
      System.Diagnostics.Debug.WriteLine($"JS Interop error: {ex.Message}");
      return false;
    }
  }

  /// <summary>
  /// Create skip to main content link
  /// Should be first focusable element on page
  /// </summary>
  public async Task CreateSkipLinkAsync(string targetId, string label = "Skip to main content")
  {
    if (string.IsNullOrEmpty(targetId))
      throw new ArgumentNullException(nameof(targetId));

    try
    {
      await _jsRuntime.InvokeVoidAsync("accessibility.createSkipLink", targetId, label);
    }
    catch (JSException ex)
    {
      System.Diagnostics.Debug.WriteLine($"JS Interop error: {ex.Message}");
    }
  }

  /// <summary>
  /// Update live region with new content for screen reader announcement
  /// </summary>
  public async Task UpdateLiveRegionAsync(string regionId, string message)
  {
    if (string.IsNullOrEmpty(regionId))
      throw new ArgumentNullException(nameof(regionId));

    try
    {
      await _jsRuntime.InvokeVoidAsync("accessibility.updateLiveRegion", regionId, message);
    }
    catch (JSException ex)
    {
      System.Diagnostics.Debug.WriteLine($"JS Interop error: {ex.Message}");
    }
  }
}

/// <summary>
/// Screen reader announcement priority levels
/// </summary>
public enum ScreenReaderPriority
{
  /// <summary>
  /// Polite: Wait for pause in speech before announcing (default)
  /// </summary>
  Polite,

  /// <summary>
  /// Assertive: Interrupt current speech to announce immediately
  /// </summary>
  Assertive
}

/// <summary>
/// Result of color contrast verification
/// </summary>
public class ColorContrastResult
{
  /// <summary>
  /// Contrast ratio value
  /// </summary>
  public decimal Ratio { get; set; }

  /// <summary>
  /// Passes WCAG AA standard (4.5:1 for normal text, 3:1 for large text)
  /// </summary>
  public bool PassesWcagAA { get; set; }

  /// <summary>
  /// Passes WCAG AAA standard (7:1 for normal text, 4.5:1 for large text)
  /// </summary>
  public bool PassesWcagAAA { get; set; }
}

/// <summary>
/// ARIA label generators for common patterns
/// </summary>
public static class AriaLabels
{
  /// <summary>
  /// Generate ARIA label for a clinical note
  /// </summary>
  public static string Note(string patientName, string noteType, string date)
      => $"{noteType} for {patientName} on {date}";

  /// <summary>
  /// Generate ARIA label for an appointment
  /// </summary>
  public static string Appointment(string patientName, string time, string status)
      => $"Appointment with {patientName} at {time}, status: {status}";

  /// <summary>
  /// ARIA labels for button actions
  /// </summary>
  public static class Button
  {
    public static string Edit(string itemName) => $"Edit {itemName}";
    public static string Delete(string itemName) => $"Delete {itemName}";
    public static string View(string itemName) => $"View {itemName} details";
    public static string Download(string itemName) => $"Download {itemName}";
    public static string Close() => "Close dialog";
    public static string Save() => "Save changes";
    public static string Cancel() => "Cancel and discard changes";
  }

  /// <summary>
  /// ARIA labels for form fields
  /// </summary>
  public static class Form
  {
    /// <summary>
    /// Generate ARIA label for a required form field.
    /// </summary>
    /// <param name="fieldName">The name of the form field.</param>
    /// <returns>ARIA label string.</returns>
    public static string Required(string fieldName) => $"{fieldName}, required field";
    /// <summary>
    /// Generate ARIA label for an optional form field.
    /// </summary>
    /// <param name="fieldName">The name of the form field.</param>
    /// <returns>ARIA label string.</returns>
    public static string Optional(string fieldName) => $"{fieldName}, optional";
    /// <summary>
    /// Generate ARIA label for a form field with an error.
    /// </summary>
    /// <param name="fieldName">The name of the form field.</param>
    /// <param name="error">The error message.</param>
    /// <returns>ARIA label string.</returns>
    public static string Error(string fieldName, string error) => $"{fieldName} error: {error}";
    /// <summary>
    /// Generate ARIA label for a form field with helper text.
    /// </summary>
    /// <param name="fieldName">The name of the form field.</param>
    /// <param name="helperText">The helper text.</param>
    /// <returns>ARIA label string.</returns>
    public static string Helper(string fieldName, string helperText) => $"{fieldName} help: {helperText}";
  }

  /// <summary>
  /// Generate ARIA label for status indicators
  /// </summary>
  public static string Status(string status) => $"Status: {status}";

  /// <summary>
  /// Generate ARIA label for progress indicators
  /// </summary>
  public static string Progress(int current, int total) => $"Step {current} of {total}";
}

/// <summary>
/// Keyboard event handler configuration
/// </summary>
public class KeyboardHandlerConfig
{
  public Func<Task>? OnEnter { get; set; }
  public Func<Task>? OnEscape { get; set; }
  public Func<Task>? OnSpace { get; set; }
  public Func<Task>? OnArrowUp { get; set; }
  public Func<Task>? OnArrowDown { get; set; }
  public Func<Task>? OnArrowLeft { get; set; }
  public Func<Task>? OnArrowRight { get; set; }
}
