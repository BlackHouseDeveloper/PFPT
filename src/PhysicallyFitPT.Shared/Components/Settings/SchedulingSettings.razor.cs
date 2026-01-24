// <copyright file="SchedulingSettings.razor.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

#pragma warning disable SA1600 // Private members don't require public documentation.

namespace PhysicallyFitPT.Shared.Components.Settings;

using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Components;

/// <summary>
/// Scheduling settings UI for managing visit types and schedule blocks.
/// </summary>
public partial class SchedulingSettings : ComponentBase
{
  private List<VisitType> visitTypes = new();
  private List<ScheduleBlock> scheduleBlocks = new();
  private Settings settings = new();

  /// <summary>
  /// Gets or sets the callback invoked when the settings are saved.
  /// </summary>
  [Parameter]
  public EventCallback OnSave { get; set; }

  /// <summary>
  /// Gets or sets a value indicating whether unsaved changes exist.
  /// </summary>
  [Parameter]
  public bool HasUnsavedChanges { get; set; }

  /// <summary>
  /// Gets or sets the callback invoked when unsaved change state updates.
  /// </summary>
  [Parameter]
  public EventCallback HasUnsavedChangesChanged { get; set; }

  /// <summary>
  /// Initializes the component's default visit types and schedule blocks.
  /// </summary>
  protected override void OnInitialized()
  {
    this.InitializeVisitTypes();
    this.InitializeScheduleBlocks();
  }

  private void InitializeVisitTypes()
  {
    this.visitTypes = new()
        {
            new() { Id = "1", Name = "Initial Evaluation", Duration = 60, Color = "#3b82f6", Billable = true, DefaultNoteType = "eval", RequiresIntake = true, AllowsPTA = false },
            new() { Id = "2", Name = "Re-Evaluation", Duration = 60, Color = "#8b5cf6", Billable = true, DefaultNoteType = "re-eval", RequiresIntake = false, AllowsPTA = false },
            new() { Id = "3", Name = "Daily Treatment", Duration = 45, Color = "#10b981", Billable = true, DefaultNoteType = "daily", RequiresIntake = false, AllowsPTA = true },
            new() { Id = "4", Name = "Progress Note", Duration = 45, Color = "#f97316", Billable = true, DefaultNoteType = "progress", RequiresIntake = false, AllowsPTA = true },
            new() { Id = "5", Name = "Discharge", Duration = 30, Color = "#ef4444", Billable = true, DefaultNoteType = "discharge", RequiresIntake = false, AllowsPTA = false },
            new() { Id = "6", Name = "Follow-Up", Duration = 30, Color = "#06b6d4", Billable = true, DefaultNoteType = "daily", RequiresIntake = false, AllowsPTA = true },
            new() { Id = "7", Name = "Group Therapy", Duration = 60, Color = "#ec4899", Billable = true, DefaultNoteType = "daily", RequiresIntake = false, AllowsPTA = true },
            new() { Id = "8", Name = "Dry Needling", Duration = 30, Color = "#14b8a6", Billable = true, DefaultNoteType = "dry-needling", RequiresIntake = false, AllowsPTA = false },
            new() { Id = "9", Name = "Telehealth Visit", Duration = 30, Color = "#6366f1", Billable = true, DefaultNoteType = "daily", RequiresIntake = false, AllowsPTA = true },
            new() { Id = "10", Name = "Home Health Visit", Duration = 60, Color = "#f59e0b", Billable = true, DefaultNoteType = "daily", RequiresIntake = false, AllowsPTA = true },
            new() { Id = "11", Name = "Consultation (Non-Billable)", Duration = 15, Color = "#6b7280", Billable = false, DefaultNoteType = "note", RequiresIntake = false, AllowsPTA = false },
            new() { Id = "12", Name = "No Show", Duration = 0, Color = "#9ca3af", Billable = false, DefaultNoteType = "none", RequiresIntake = false, AllowsPTA = false },
        };
  }

  private void InitializeScheduleBlocks()
  {
    this.scheduleBlocks = new()
        {
            new() { Id = "1", Name = "Lunch Break", Type = "lunch", Color = "#fbbf24", Duration = 60, Recurring = true, RecurrencePattern = "Daily 12:00-13:00" },
            new() { Id = "2", Name = "Admin Time", Type = "admin", Color = "#a78bfa", Duration = 30, Recurring = true, RecurrencePattern = "Daily 8:00-8:30" },
            new() { Id = "3", Name = "Team Meeting", Type = "meeting", Color = "#60a5fa", Duration = 30, Recurring = true, RecurrencePattern = "Weekly Monday 9:00-9:30" },
            new() { Id = "4", Name = "Break", Type = "break", Color = "#34d399", Duration = 15, Recurring = false },
        };
  }

  private void AddVisitType() => this.NotifyUnsavedChanges();

  private void EditVisitType(VisitType visitType) => this.NotifyUnsavedChanges();

  private void DeleteVisitType(string id)
  {
    this.visitTypes = this.visitTypes.Where(vt => vt.Id != id).ToList();
    this.NotifyUnsavedChanges();
  }

  private void AddScheduleBlock() => this.NotifyUnsavedChanges();

  private void EditScheduleBlock(ScheduleBlock scheduleBlock) => this.NotifyUnsavedChanges();

  private void DeleteScheduleBlock(string id)
  {
    this.scheduleBlocks = this.scheduleBlocks.Where(sb => sb.Id != id).ToList();
    this.NotifyUnsavedChanges();
  }

  private void NotifyUnsavedChanges()
  {
    this.HasUnsavedChanges = true;
    this.HasUnsavedChangesChanged.InvokeAsync(true);
  }

#pragma warning disable SA1516 // Allow compact property grouping inside helper models.
  /// <summary>
  /// Represents a visit type configuration for scheduling.
  /// </summary>
  public sealed record VisitType
  {
    /// <summary>Gets or sets the unique identifier.</summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>Gets or sets the display name.</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Gets or sets the duration in minutes.</summary>
    public int Duration { get; set; }

    /// <summary>Gets or sets the color code.</summary>
    public string Color { get; set; } = string.Empty;

    /// <summary>Gets or sets a value indicating whether this visit type is billable.</summary>
    public bool Billable { get; set; }

    /// <summary>Gets or sets the default note type.</summary>
    public string DefaultNoteType { get; set; } = string.Empty;

    /// <summary>Gets or sets a value indicating whether PTAs can perform this visit type.</summary>
    public bool AllowsPTA { get; set; }

    /// <summary>Gets or sets a value indicating whether intake is required.</summary>
    public bool RequiresIntake { get; set; }
  }

  /// <summary>
  /// Represents a schedule block configuration (breaks, meetings, admin time).
  /// </summary>
  public sealed record ScheduleBlock
  {
    /// <summary>Gets or sets the unique identifier.</summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>Gets or sets the display name.</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Gets or sets the block type.</summary>
    public string Type { get; set; } = string.Empty;

    /// <summary>Gets or sets the color code.</summary>
    public string Color { get; set; } = string.Empty;

    /// <summary>Gets or sets the duration in minutes.</summary>
    public int Duration { get; set; }

    /// <summary>Gets or sets a value indicating whether this block recurs.</summary>
    public bool Recurring { get; set; }

    /// <summary>Gets or sets the recurrence pattern description.</summary>
    public string? RecurrencePattern { get; set; }
  }

  /// <summary>
  /// Scheduling and clinic settings configuration.
  /// </summary>
  public sealed record Settings
  {
    /// <summary>Gets or sets the default appointment duration in minutes.</summary>
    public int DefaultDuration { get; set; } = 45;

    /// <summary>Gets or sets the buffer time between appointments in minutes.</summary>
    public int BufferTimeBetween { get; set; } = 15;

    /// <summary>Gets or sets a value indicating whether double booking is allowed.</summary>
    public bool AllowDoubleBooking { get; set; }

    /// <summary>Gets or sets a value indicating whether appointments are auto-confirmed.</summary>
    public bool AutoConfirmAppointments { get; set; } = true;

    /// <summary>Gets or sets a value indicating whether reminders are sent.</summary>
    public bool SendReminders { get; set; } = true;

    /// <summary>Gets or sets the reminder hours before appointment.</summary>
    public int ReminderHoursBefore { get; set; } = 24;

    /// <summary>Gets or sets the clinic start time (HH:mm format).</summary>
    public string ClinicStartTime { get; set; } = "08:00";

    /// <summary>Gets or sets the clinic end time (HH:mm format).</summary>
    public string ClinicEndTime { get; set; } = "18:00";

    /// <summary>Gets or sets the lunch break start time (HH:mm format).</summary>
    public string LunchBreakStart { get; set; } = "12:00";

    /// <summary>Gets or sets the lunch break end time (HH:mm format).</summary>
    public string LunchBreakEnd { get; set; } = "13:00";

    /// <summary>Gets or sets a value indicating whether click-to-create is enabled.</summary>
    public bool EnableClickToCreate { get; set; } = true;

    /// <summary>Gets or sets a value indicating whether intake status is shown.</summary>
    public bool ShowIntakeStatus { get; set; } = true;

    /// <summary>Gets or sets a value indicating whether cancel from week view is allowed.</summary>
    public bool AllowCancelFromWeekView { get; set; } = true;

    /// <summary>Gets or sets a value indicating whether reschedule from week view is allowed.</summary>
    public bool AllowRescheduleFromWeekView { get; set; } = true;

    /// <summary>Gets or sets the default clinician view type.</summary>
    public string DefaultClinicianView { get; set; } = "week";

    /// <summary>Gets or sets the default admin view type.</summary>
    public string DefaultAdminView { get; set; } = "all-day";

    /// <summary>Gets or sets the intake sent color code.</summary>
    public string IntakeSentColor { get; set; } = "#fbbf24";

    /// <summary>Gets or sets the intake incomplete color code.</summary>
    public string IntakeIncompleteColor { get; set; } = "#f97316";

    /// <summary>Gets or sets the intake complete color code.</summary>
    public string IntakeCompleteColor { get; set; } = "#10b981";
  }
#pragma warning restore SA1516
}

#pragma warning restore SA1600
