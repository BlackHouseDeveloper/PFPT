/**
 * Local Storage Data Persistence Utility
 * Handles saving and loading SOAP note data, intake forms, and app state
 * Prevents data loss during navigation and crashes
 */

window.dataStore = (function() {
    'use strict';

    const STORAGE_KEYS = {
        SOAP_DRAFT: 'pfpt_soap_draft',
        INTAKE_DRAFT: 'pfpt_intake_draft',
        GOALS_DRAFT: 'pfpt_goals_draft',
        APP_STATE: 'pfpt_app_state',
        AUTO_SAVE_TIMESTAMP: 'pfpt_autosave_timestamp'
    };

    /**
     * Safely save data to localStorage with error handling
     */
    function safeSetItem(key, value) {
        try {
            const serialized = JSON.stringify({
                ...value,
                timestamp: Date.now()
            });
            localStorage.setItem(key, serialized);
            return true;
        } catch (error) {
            console.error(`Failed to save to localStorage (${key}):`, error);
            
            // Handle quota exceeded error
            if (error.name === 'QuotaExceededError') {
                console.warn('LocalStorage quota exceeded. Clearing old data...');
                clearOldDrafts();
                
                // Retry save after clearing
                try {
                    const serialized = JSON.stringify({
                        ...value,
                        timestamp: Date.now()
                    });
                    localStorage.setItem(key, serialized);
                    return true;
                } catch (retryError) {
                    console.error('Failed to save even after clearing:', retryError);
                    return false;
                }
            }
            return false;
        }
    }

    /**
     * Safely retrieve data from localStorage with error handling
     */
    function safeGetItem(key) {
        try {
            const item = localStorage.getItem(key);
            if (!item) return null;
            return JSON.parse(item);
        } catch (error) {
            console.error(`Failed to retrieve from localStorage (${key}):`, error);
            return null;
        }
    }

    /**
     * Clear old drafts that are older than 7 days
     */
    function clearOldDrafts() {
        const sevenDaysAgo = Date.now() - (7 * 24 * 60 * 60 * 1000);
        
        Object.values(STORAGE_KEYS).forEach(key => {
            try {
                const data = safeGetItem(key);
                if (data && data.timestamp && data.timestamp < sevenDaysAgo) {
                    localStorage.removeItem(key);
                    console.log(`Cleared old draft: ${key}`);
                }
            } catch (error) {
                console.error(`Error clearing old draft (${key}):`, error);
            }
        });
    }

    // ===================
    // SOAP Note Functions
    // ===================

    function saveSOAPDraft(data, noteType, patientId) {
        return safeSetItem(STORAGE_KEYS.SOAP_DRAFT, {
            data: data,
            noteType: noteType,
            patientId: patientId || null
        });
    }

    function loadSOAPDraft() {
        return safeGetItem(STORAGE_KEYS.SOAP_DRAFT);
    }

    function clearSOAPDraft() {
        try {
            localStorage.removeItem(STORAGE_KEYS.SOAP_DRAFT);
        } catch (error) {
            console.error('Failed to clear SOAP draft:', error);
        }
    }

    function hasSOAPDraft() {
        const draft = loadSOAPDraft();
        return draft !== null && draft.data !== null;
    }

    // ===================
    // Intake Form Functions
    // ===================

    function saveIntakeDraft(data, currentStep) {
        return safeSetItem(STORAGE_KEYS.INTAKE_DRAFT, {
            data: data,
            currentStep: currentStep
        });
    }

    function loadIntakeDraft() {
        return safeGetItem(STORAGE_KEYS.INTAKE_DRAFT);
    }

    function clearIntakeDraft() {
        try {
            localStorage.removeItem(STORAGE_KEYS.INTAKE_DRAFT);
        } catch (error) {
            console.error('Failed to clear intake draft:', error);
        }
    }

    function hasIntakeDraft() {
        const draft = loadIntakeDraft();
        return draft !== null && draft.data !== null;
    }

    // ===================
    // Goals & Interventions Functions
    // ===================

    function saveGoalsDraft(data, patientId) {
        return safeSetItem(STORAGE_KEYS.GOALS_DRAFT, {
            data: data,
            patientId: patientId || null
        });
    }

    function loadGoalsDraft() {
        return safeGetItem(STORAGE_KEYS.GOALS_DRAFT);
    }

    function clearGoalsDraft() {
        try {
            localStorage.removeItem(STORAGE_KEYS.GOALS_DRAFT);
        } catch (error) {
            console.error('Failed to clear goals draft:', error);
        }
    }

    function hasGoalsDraft() {
        const draft = loadGoalsDraft();
        return draft !== null && draft.data !== null;
    }

    // ===================
    // App State Functions
    // ===================

    function saveAppState(state) {
        return safeSetItem(STORAGE_KEYS.APP_STATE, state);
    }

    function loadAppState() {
        return safeGetItem(STORAGE_KEYS.APP_STATE);
    }

    // ===================
    // Auto-save Timestamp
    // ===================

    function updateAutoSaveTimestamp() {
        try {
            localStorage.setItem(STORAGE_KEYS.AUTO_SAVE_TIMESTAMP, Date.now().toString());
        } catch (error) {
            console.error('Failed to update auto-save timestamp:', error);
        }
    }

    function getAutoSaveTimestamp() {
        try {
            const timestamp = localStorage.getItem(STORAGE_KEYS.AUTO_SAVE_TIMESTAMP);
            return timestamp ? parseInt(timestamp, 10) : null;
        } catch (error) {
            console.error('Failed to get auto-save timestamp:', error);
            return null;
        }
    }

    // ===================
    // Utility Functions
    // ===================

    /**
     * Check if localStorage is available and working
     */
    function isLocalStorageAvailable() {
        try {
            const testKey = '__pfpt_test__';
            localStorage.setItem(testKey, 'test');
            localStorage.removeItem(testKey);
            return true;
        } catch (error) {
            return false;
        }
    }

    /**
     * Get storage usage information
     */
    function getStorageInfo() {
        let used = 0;
        const available = isLocalStorageAvailable();
        
        if (available) {
            try {
                for (const key in localStorage) {
                    if (localStorage.hasOwnProperty(key)) {
                        used += localStorage[key].length + key.length;
                    }
                }
            } catch (error) {
                console.error('Failed to calculate storage usage:', error);
            }
        }
        
        return { used: used, available: available };
    }

    /**
     * Clear all PFPT data from localStorage
     */
    function clearAllData() {
        Object.values(STORAGE_KEYS).forEach(key => {
            try {
                localStorage.removeItem(key);
            } catch (error) {
                console.error(`Failed to remove ${key}:`, error);
            }
        });
    }

    // Run cleanup on module load
    clearOldDrafts();

    // Public API
    return {
        // SOAP Note functions
        saveSOAPDraft: saveSOAPDraft,
        loadSOAPDraft: loadSOAPDraft,
        clearSOAPDraft: clearSOAPDraft,
        hasSOAPDraft: hasSOAPDraft,
        
        // Intake Form functions
        saveIntakeDraft: saveIntakeDraft,
        loadIntakeDraft: loadIntakeDraft,
        clearIntakeDraft: clearIntakeDraft,
        hasIntakeDraft: hasIntakeDraft,
        
        // Goals functions
        saveGoalsDraft: saveGoalsDraft,
        loadGoalsDraft: loadGoalsDraft,
        clearGoalsDraft: clearGoalsDraft,
        hasGoalsDraft: hasGoalsDraft,
        
        // App State functions
        saveAppState: saveAppState,
        loadAppState: loadAppState,
        
        // Auto-save functions
        updateAutoSaveTimestamp: updateAutoSaveTimestamp,
        getAutoSaveTimestamp: getAutoSaveTimestamp,
        
        // Utility functions
        isLocalStorageAvailable: isLocalStorageAvailable,
        getStorageInfo: getStorageInfo,
        clearAllData: clearAllData,
        clearOldDrafts: clearOldDrafts
    };
})();

// CommonJS export for compatibility
if (typeof module !== 'undefined' && module.exports) {
    module.exports = window.dataStore;
}
