// <copyright file="SystemPreferencesSettings.razor.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

#pragma warning disable SA1600 // Private members don't require public documentation.

namespace PhysicallyFitPT.Shared.Components.Settings
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Components;
    using Microsoft.AspNetCore.Components.Forms;

    /// <summary>
    /// System preferences UI for toggling global UX options.
    /// </summary>
    public partial class SystemPreferencesSettings
    {
        private SystemPreferences settings = new();
        private string? logoPreview;

        /// <summary>
        /// Gets or sets the callback invoked when the preferences are saved.
        /// </summary>
        [Parameter]
        public EventCallback OnSave { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether unsaved changes are present.
        /// </summary>
        [Parameter]
        public bool HasUnsavedChanges { get; set; }

        /// <summary>
        /// Gets or sets the callback invoked when the unsaved change state updates.
        /// </summary>
        [Parameter]
        public EventCallback<bool> HasUnsavedChangesChanged { get; set; }

        /// <summary>
        /// Initializes the component with default system preferences.
        /// </summary>
        protected override void OnInitialized()
        {
            // Keep initial defaults in sync with the Razor UI expectations.
            this.settings = new SystemPreferences();
        }

        private void UpdateSetting(string key, string value)
        {
            switch (key)
            {
                case "SectionDensity":
                    this.settings.SectionDensity = value;
                    break;
                case "DateFormat":
                    this.settings.DateFormat = value;
                    break;
                case "TimeFormat":
                    this.settings.TimeFormat = value;
                    break;
            }

            this.NotifyUnsavedChanges();
        }

        private void ToggleOption(string key, bool value)
        {
            switch (key)
            {
                case "dark-mode":
                    this.settings.EnableDarkMode = value;
                    break;
                case "require-mfa":
                    this.settings.RequireMfa = value;
                    break;
                case "audit-logs":
                    this.settings.EnableAuditLogs = value;
                    break;
                case "debug-stats":
                    this.settings.ShowDebugStats = value;
                    break;
                case "notifications":
                    this.settings.AllowNotifications = value;
                    break;
                case "notification-sound":
                    this.settings.PlaySound = value;
                    break;
                case "notification-vibration":
                    this.settings.Vibration = value;
                    break;
                case "daily-summary":
                    this.settings.DailySummary = value;
                    break;
                case "clinician-mode":
                    this.settings.ClinicianMode = value;
                    break;
                case "admin-mode":
                    this.settings.AdminMode = value;
                    break;
                case "super-admin-mode":
                    this.settings.SuperAdminMode = value;
                    break;
                case "high-contrast":
                    this.settings.HighContrast = value;
                    break;
                case "auto-save":
                    this.settings.AutoSave = value;
                    break;
                case "smart-suggestions":
                    this.settings.SmartSuggestions = value;
                    break;
                case "bottom-nav-buttons":
                    this.settings.EnableBottomNavButtons = value;
                    break;
                case "progress-indicator":
                    this.settings.ShowProgressIndicator = value;
                    break;
            }

            this.NotifyUnsavedChanges();
        }

        private void OnColorChanged(string color, string type)
        {
            switch (type)
            {
                case "primary":
                    this.settings.PrimaryColor = color;
                    break;
                case "accent":
                    this.settings.AccentColor = color;
                    break;
                case "login-background":
                    this.settings.LoginBackground = color;
                    break;
                case "secondary":
                    this.settings.SecondaryColor = color;
                    break;
            }

            this.NotifyUnsavedChanges();
        }

        private void OnFontScaleChanged(ChangeEventArgs e)
        {
            if (double.TryParse(e.Value?.ToString(), out var value))
            {
                this.settings.FontScale = value;
                this.NotifyUnsavedChanges();
            }
        }

        private void OnAutoSaveIntervalChanged(ChangeEventArgs e)
        {
            if (int.TryParse(e.Value?.ToString(), out var value))
            {
                this.settings.AutoSaveInterval = value;
                this.NotifyUnsavedChanges();
            }
        }

        private Task OnLogoSelected(InputFileChangeEventArgs e)
        {
            if (e.FileCount == 0)
            {
                return Task.CompletedTask;
            }

            var file = e.File;
            using var stream = file.OpenReadStream(maxAllowedSize: 5 * 1024 * 1024);
            using var memoryStream = new MemoryStream();
            stream.CopyTo(memoryStream);
            var bytes = memoryStream.ToArray();
            this.settings.Logo = Convert.ToBase64String(bytes);
            this.logoPreview = $"data:{file.ContentType};base64,{this.settings.Logo}";

            this.NotifyUnsavedChanges();

            return Task.CompletedTask;
        }

        private Task HandleLogoUpload(InputFileChangeEventArgs e) => this.OnLogoSelected(e);

        private void ResetBrandColors()
        {
            this.settings.PrimaryColor = "#2563eb";
            this.settings.SecondaryColor = "#22c55e";
            this.settings.AccentColor = "#10b981";
            this.NotifyUnsavedChanges();
        }

        private void NotifyUnsavedChanges()
        {
            this.HasUnsavedChanges = true;
            if (this.HasUnsavedChangesChanged.HasDelegate)
            {
                this.HasUnsavedChangesChanged.InvokeAsync(true);
            }
        }

        private Task SavePreferences()
        {
            return this.OnSave.InvokeAsync(null);
        }

#pragma warning disable SA1516 // Allow compact property grouping inside helper model.
        /// <summary>
        /// Represents system-wide preferences for UI/UX and functionality configuration.
        /// </summary>
        public sealed record SystemPreferences
        {
            /// <summary>Gets or sets the section density preference (standard/compact/relaxed).</summary>
            public string SectionDensity { get; set; } = "standard";

            /// <summary>Gets or sets a value indicating whether bottom navigation buttons are enabled.</summary>
            public bool EnableBottomNavButtons { get; set; } = true;

            /// <summary>Gets or sets a value indicating whether the progress indicator is shown.</summary>
            public bool ShowProgressIndicator { get; set; } = true;

            /// <summary>Gets or sets a value indicating whether dark mode is enabled.</summary>
            public bool EnableDarkMode { get; set; }

            /// <summary>Gets or sets a value indicating whether multi-factor authentication is required.</summary>
            public bool RequireMfa { get; set; }

            /// <summary>Gets or sets a value indicating whether audit logging is enabled.</summary>
            public bool EnableAuditLogs { get; set; } = true;

            /// <summary>Gets or sets a value indicating whether debug statistics are shown.</summary>
            public bool ShowDebugStats { get; set; }

            /// <summary>Gets or sets the primary brand color code.</summary>
            public string PrimaryColor { get; set; } = "#2563eb";

            /// <summary>Gets or sets the secondary brand color code.</summary>
            public string SecondaryColor { get; set; } = "#22c55e";

            /// <summary>Gets or sets the accent color code.</summary>
            public string AccentColor { get; set; } = "#10b981";

            /// <summary>Gets or sets the logo image data or URL.</summary>
            public string Logo { get; set; } = string.Empty;

            /// <summary>Gets or sets the login page background image or color.</summary>
            public string LoginBackground { get; set; } = "#0f172a";

            /// <summary>Gets or sets a value indicating whether notifications are enabled.</summary>
            public bool AllowNotifications { get; set; } = true;

            /// <summary>Gets or sets a value indicating whether sound notifications are played.</summary>
            public bool PlaySound { get; set; }

            /// <summary>Gets or sets a value indicating whether vibration feedback is enabled.</summary>
            public bool Vibration { get; set; } = true;

            /// <summary>Gets or sets a value indicating whether daily summaries are sent.</summary>
            public bool DailySummary { get; set; }

            /// <summary>Gets or sets the time for daily notifications (HH:mm format).</summary>
            public string NotificationTime { get; set; } = "18:00";

            /// <summary>Gets or sets a value indicating whether clinician mode is active.</summary>
            public bool ClinicianMode { get; set; }

            /// <summary>Gets or sets a value indicating whether admin mode is active.</summary>
            public bool AdminMode { get; set; }

            /// <summary>Gets or sets a value indicating whether super-admin mode is active.</summary>
            public bool SuperAdminMode { get; set; }

            /// <summary>Gets or sets a value indicating whether high-contrast mode is enabled.</summary>
            public bool HighContrast { get; set; }

            /// <summary>Gets or sets the font scaling factor (1.0 = 100%).</summary>
            public double FontScale { get; set; } = 1.0;

            /// <summary>Gets or sets the date display format.</summary>
            public string DateFormat { get; set; } = "MM/dd/yyyy";

            /// <summary>Gets or sets the timezone identifier.</summary>
            public string Timezone { get; set; } = "America/New_York";

            /// <summary>Gets or sets a value indicating whether smart suggestions are enabled.</summary>
            public bool SmartSuggestions { get; set; } = true;

            /// <summary>Gets or sets a value indicating whether auto-save is enabled.</summary>
            public bool AutoSave { get; set; } = true;

            /// <summary>Gets or sets the auto-save interval in seconds.</summary>
            public int AutoSaveInterval { get; set; } = 5;

            /// <summary>Gets or sets a value indicating whether the font family is locked.</summary>
            public bool FontLocked { get; set; }

            /// <summary>Gets or sets the font family name.</summary>
            public string FontFamily { get; set; } = "Inter";

            /// <summary>Gets or sets the time format preference (12 or 24 hour).</summary>
            public string TimeFormat { get; set; } = "12";
        }
#pragma warning restore SA1516
    }
}

#pragma warning restore SA1600
