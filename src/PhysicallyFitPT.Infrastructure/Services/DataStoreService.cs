// <copyright file="DataStoreService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System.Diagnostics;
using Microsoft.JSInterop;

namespace PhysicallyFitPT.Infrastructure.Services;

/// <summary>
/// Service for persisting clinical data to browser localStorage to prevent data loss.
/// Handles SOAP note drafts, intake forms, goals, and app state with automatic cleanup.
/// </summary>
public class DataStoreService
{
    private readonly IJSRuntime _jsRuntime;

    /// <summary>
    /// Initializes a new instance of the <see cref="DataStoreService"/> class.
    /// </summary>
    /// <param name="jsRuntime">JavaScript runtime for localStorage interop.</param>
    public DataStoreService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    // ==================
    // SOAP Note Functions
    // ==================

    /// <summary>
    /// Saves SOAP note draft to localStorage.
    /// </summary>
    /// <param name="data">The SOAP note data to save.</param>
    /// <param name="noteType">The type of note (e.g., "evaluation", "daily").</param>
    /// <param name="patientId">Optional patient ID.</param>
    /// <returns>True if save was successful.</returns>
    public async Task<bool> SaveSoapDraftAsync(object data, string noteType, string? patientId = null)
    {
        try
        {
            return await _jsRuntime.InvokeAsync<bool>("dataStore.saveSOAPDraft", data, noteType, patientId);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Failed to save SOAP draft: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Loads SOAP note draft from localStorage.
    /// </summary>
    /// <returns>The stored SOAP data, or null if none exists.</returns>
    public async Task<StoredSoapData?> LoadSoapDraftAsync()
    {
        try
        {
            return await _jsRuntime.InvokeAsync<StoredSoapData?>("dataStore.loadSOAPDraft");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Failed to load SOAP draft: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// Clears SOAP note draft from localStorage.
    /// </summary>
    public async Task ClearSoapDraftAsync()
    {
        try
        {
            await _jsRuntime.InvokeVoidAsync("dataStore.clearSOAPDraft");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Failed to clear SOAP draft: {ex.Message}");
        }
    }

    /// <summary>
    /// Checks if a SOAP note draft exists.
    /// </summary>
    /// <returns>True if draft exists.</returns>
    public async Task<bool> HasSoapDraftAsync()
    {
        try
        {
            return await _jsRuntime.InvokeAsync<bool>("dataStore.hasSOAPDraft");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Failed to check SOAP draft: {ex.Message}");
            return false;
        }
    }

    // ==================
    // Intake Form Functions
    // ==================

    /// <summary>
    /// Saves intake form draft to localStorage.
    /// </summary>
    /// <param name="data">The intake form data to save.</param>
    /// <param name="currentStep">The current step in the intake wizard.</param>
    /// <returns>True if save was successful.</returns>
    public async Task<bool> SaveIntakeDraftAsync(object data, int currentStep)
    {
        try
        {
            return await _jsRuntime.InvokeAsync<bool>("dataStore.saveIntakeDraft", data, currentStep);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Failed to save intake draft: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Loads intake form draft from localStorage.
    /// </summary>
    /// <returns>The stored intake data, or null if none exists.</returns>
    public async Task<StoredIntakeData?> LoadIntakeDraftAsync()
    {
        try
        {
            return await _jsRuntime.InvokeAsync<StoredIntakeData?>("dataStore.loadIntakeDraft");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Failed to load intake draft: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// Clears intake form draft from localStorage.
    /// </summary>
    public async Task ClearIntakeDraftAsync()
    {
        try
        {
            await _jsRuntime.InvokeVoidAsync("dataStore.clearIntakeDraft");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Failed to clear intake draft: {ex.Message}");
        }
    }

    /// <summary>
    /// Checks if an intake form draft exists.
    /// </summary>
    /// <returns>True if draft exists.</returns>
    public async Task<bool> HasIntakeDraftAsync()
    {
        try
        {
            return await _jsRuntime.InvokeAsync<bool>("dataStore.hasIntakeDraft");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Failed to check intake draft: {ex.Message}");
            return false;
        }
    }

    // ==================
    // Goals & Interventions Functions
    // ==================

    /// <summary>
    /// Saves goals and interventions draft to localStorage.
    /// </summary>
    /// <param name="data">The goals data to save.</param>
    /// <param name="patientId">Optional patient ID.</param>
    /// <returns>True if save was successful.</returns>
    public async Task<bool> SaveGoalsDraftAsync(object data, string? patientId = null)
    {
        try
        {
            return await _jsRuntime.InvokeAsync<bool>("dataStore.saveGoalsDraft", data, patientId);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Failed to save goals draft: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Loads goals and interventions draft from localStorage.
    /// </summary>
    /// <returns>The stored goals data, or null if none exists.</returns>
    public async Task<StoredGoalsData?> LoadGoalsDraftAsync()
    {
        try
        {
            return await _jsRuntime.InvokeAsync<StoredGoalsData?>("dataStore.loadGoalsDraft");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Failed to load goals draft: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// Clears goals and interventions draft from localStorage.
    /// </summary>
    public async Task ClearGoalsDraftAsync()
    {
        try
        {
            await _jsRuntime.InvokeVoidAsync("dataStore.clearGoalsDraft");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Failed to clear goals draft: {ex.Message}");
        }
    }

    /// <summary>
    /// Checks if a goals draft exists.
    /// </summary>
    /// <returns>True if draft exists.</returns>
    public async Task<bool> HasGoalsDraftAsync()
    {
        try
        {
            return await _jsRuntime.InvokeAsync<bool>("dataStore.hasGoalsDraft");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Failed to check goals draft: {ex.Message}");
            return false;
        }
    }

    // ==================
    // App State Functions
    // ==================

    /// <summary>
    /// Saves application state to localStorage.
    /// </summary>
    /// <param name="state">The app state to save.</param>
    /// <returns>True if save was successful.</returns>
    public async Task<bool> SaveAppStateAsync(object state)
    {
        try
        {
            return await _jsRuntime.InvokeAsync<bool>("dataStore.saveAppState", state);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Failed to save app state: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Loads application state from localStorage.
    /// </summary>
    /// <returns>The stored app state, or null if none exists.</returns>
    public async Task<object?> LoadAppStateAsync()
    {
        try
        {
            return await _jsRuntime.InvokeAsync<object?>("dataStore.loadAppState");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Failed to load app state: {ex.Message}");
            return null;
        }
    }

    // ==================
    // Auto-save Timestamp
    // ==================

    /// <summary>
    /// Updates the auto-save timestamp to current time.
    /// </summary>
    public async Task UpdateAutoSaveTimestampAsync()
    {
        try
        {
            await _jsRuntime.InvokeVoidAsync("dataStore.updateAutoSaveTimestamp");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Failed to update auto-save timestamp: {ex.Message}");
        }
    }

    /// <summary>
    /// Gets the last auto-save timestamp.
    /// </summary>
    /// <returns>Timestamp in milliseconds, or null if never saved.</returns>
    public async Task<long?> GetAutoSaveTimestampAsync()
    {
        try
        {
            return await _jsRuntime.InvokeAsync<long?>("dataStore.getAutoSaveTimestamp");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Failed to get auto-save timestamp: {ex.Message}");
            return null;
        }
    }

    // ==================
    // Utility Functions
    // ==================

    /// <summary>
    /// Checks if localStorage is available in the browser.
    /// </summary>
    /// <returns>True if localStorage is available and working.</returns>
    public async Task<bool> IsLocalStorageAvailableAsync()
    {
        try
        {
            return await _jsRuntime.InvokeAsync<bool>("dataStore.isLocalStorageAvailable");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Failed to check localStorage availability: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Gets storage usage information.
    /// </summary>
    /// <returns>Storage info with used bytes and availability status.</returns>
    public async Task<StorageInfo?> GetStorageInfoAsync()
    {
        try
        {
            return await _jsRuntime.InvokeAsync<StorageInfo?>("dataStore.getStorageInfo");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Failed to get storage info: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// Clears all PFPT data from localStorage.
    /// </summary>
    public async Task ClearAllDataAsync()
    {
        try
        {
            await _jsRuntime.InvokeVoidAsync("dataStore.clearAllData");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Failed to clear all data: {ex.Message}");
        }
    }

    /// <summary>
    /// Manually triggers cleanup of old drafts (older than 7 days).
    /// </summary>
    public async Task ClearOldDraftsAsync()
    {
        try
        {
            await _jsRuntime.InvokeVoidAsync("dataStore.clearOldDrafts");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Failed to clear old drafts: {ex.Message}");
        }
    }
}

/// <summary>
/// Represents stored SOAP note data.
/// </summary>
public class StoredSoapData
{
    /// <summary>
    /// Gets or sets the timestamp when data was saved (milliseconds since epoch).
    /// </summary>
    public long Timestamp { get; set; }

    /// <summary>
    /// Gets or sets the SOAP note data (deserialized as dynamic object).
    /// </summary>
    public object? Data { get; set; }

    /// <summary>
    /// Gets or sets the note type (e.g., "evaluation", "daily").
    /// </summary>
    public string? NoteType { get; set; }

    /// <summary>
    /// Gets or sets the patient ID associated with this note.
    /// </summary>
    public string? PatientId { get; set; }
}

/// <summary>
/// Represents stored intake form data.
/// </summary>
public class StoredIntakeData
{
    /// <summary>
    /// Gets or sets the timestamp when data was saved (milliseconds since epoch).
    /// </summary>
    public long Timestamp { get; set; }

    /// <summary>
    /// Gets or sets the intake form data (deserialized as dynamic object).
    /// </summary>
    public object? Data { get; set; }

    /// <summary>
    /// Gets or sets the current step in the intake wizard.
    /// </summary>
    public int CurrentStep { get; set; }
}

/// <summary>
/// Represents stored goals and interventions data.
/// </summary>
public class StoredGoalsData
{
    /// <summary>
    /// Gets or sets the timestamp when data was saved (milliseconds since epoch).
    /// </summary>
    public long Timestamp { get; set; }

    /// <summary>
    /// Gets or sets the goals data (deserialized as dynamic object).
    /// </summary>
    public object? Data { get; set; }

    /// <summary>
    /// Gets or sets the patient ID associated with these goals.
    /// </summary>
    public string? PatientId { get; set; }
}

/// <summary>
/// Represents localStorage usage information.
/// </summary>
public class StorageInfo
{
    /// <summary>
    /// Gets or sets the number of bytes used in localStorage.
    /// </summary>
    public int Used { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether localStorage is available.
    /// </summary>
    public bool Available { get; set; }
}
