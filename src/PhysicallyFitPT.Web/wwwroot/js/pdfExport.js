/**
 * PDF Export JavaScript Interop
 * Handles browser-side PDF download and print functionality
 */

window.pdfExport = (function() {
    'use strict';

    /**
     * Triggers file download in the browser
     * @param {Uint8Array} pdfBytes - The PDF file bytes
     * @param {string} filename - The filename for download
     */
    function downloadBlob(pdfBytes, filename) {
        try {
            // Convert byte array to blob
            const blob = new Blob([pdfBytes], { type: 'application/pdf' });
            const url = URL.createObjectURL(blob);
            
            // Create temporary link and trigger download
            const link = document.createElement('a');
            link.href = url;
            link.download = filename;
            link.style.display = 'none';
            document.body.appendChild(link);
            link.click();
            document.body.removeChild(link);
            
            // Clean up the URL object after a short delay
            setTimeout(() => URL.revokeObjectURL(url), 100);

            console.log('[PDF Export] File Downloaded:', {
                filename: filename,
                size: blob.size,
                timestamp: new Date().toISOString(),
                type: 'application/pdf'
            });
            
            return true;
        } catch (error) {
            console.error('[PDF Export Error]', error);
            return false;
        }
    }

    /**
     * Opens PDF in new window for printing
     * @param {Uint8Array} pdfBytes - The PDF file bytes
     */
    function openPrintDialog(pdfBytes) {
        try {
            // Convert byte array to blob
            const blob = new Blob([pdfBytes], { type: 'application/pdf' });
            const url = URL.createObjectURL(blob);
            
            // Open in new window
            const printWindow = window.open(url, '_blank');
            
            if (printWindow) {
                printWindow.onload = function() {
                    // Wait for PDF to load, then trigger print dialog
                    setTimeout(() => {
                        printWindow.print();
                    }, 500);
                };
                
                console.log('[PDF Print] Print dialog opened');
            } else {
                console.error('[PDF Print] Failed to open new window');
                return false;
            }
            
            // Clean up URL after delay
            setTimeout(() => URL.revokeObjectURL(url), 10000);
            
            return true;
        } catch (error) {
            console.error('[PDF Print Error]', error);
            return false;
        }
    }

    /**
     * Checks if download functionality is supported
     * @returns {boolean}
     */
    function isDownloadSupported() {
        try {
            return typeof Blob !== 'undefined' && 
                   typeof URL !== 'undefined' && 
                   typeof URL.createObjectURL === 'function';
        } catch (error) {
            return false;
        }
    }

    /**
     * Creates a data URI from PDF bytes
     * @param {Uint8Array} pdfBytes - The PDF file bytes
     * @returns {string} Data URI
     */
    function createDataUri(pdfBytes) {
        try {
            const blob = new Blob([pdfBytes], { type: 'application/pdf' });
            return URL.createObjectURL(blob);
        } catch (error) {
            console.error('[PDF Data URI Error]', error);
            return null;
        }
    }

    /**
     * Opens PDF in iframe for preview
     * @param {Uint8Array} pdfBytes - The PDF file bytes
     * @param {string} containerId - ID of container element
     */
    function previewPdf(pdfBytes, containerId) {
        try {
            const container = document.getElementById(containerId);
            if (!container) {
                console.error('[PDF Preview] Container not found:', containerId);
                return false;
            }

            const blob = new Blob([pdfBytes], { type: 'application/pdf' });
            const url = URL.createObjectURL(blob);
            
            // Create iframe for preview
            const iframe = document.createElement('iframe');
            iframe.src = url;
            iframe.style.width = '100%';
            iframe.style.height = '100%';
            iframe.style.border = 'none';
            
            // Clear container and add iframe
            container.innerHTML = '';
            container.appendChild(iframe);
            
            console.log('[PDF Preview] PDF loaded in container:', containerId);
            
            return true;
        } catch (error) {
            console.error('[PDF Preview Error]', error);
            return false;
        }
    }

    /**
     * Gets estimated PDF size without downloading
     * @param {Uint8Array} pdfBytes - The PDF file bytes
     * @returns {object} Size information
     */
    function getPdfInfo(pdfBytes) {
        try {
            const sizeBytes = pdfBytes.length;
            const sizeKB = (sizeBytes / 1024).toFixed(2);
            const sizeMB = (sizeBytes / (1024 * 1024)).toFixed(2);
            
            return {
                bytes: sizeBytes,
                kilobytes: parseFloat(sizeKB),
                megabytes: parseFloat(sizeMB),
                formatted: sizeBytes < 1024 * 1024 ? `${sizeKB} KB` : `${sizeMB} MB`
            };
        } catch (error) {
            console.error('[PDF Info Error]', error);
            return null;
        }
    }

    // Public API
    return {
        downloadBlob: downloadBlob,
        openPrintDialog: openPrintDialog,
        isDownloadSupported: isDownloadSupported,
        createDataUri: createDataUri,
        previewPdf: previewPdf,
        getPdfInfo: getPdfInfo
    };
})();

// CommonJS export for compatibility
if (typeof module !== 'undefined' && module.exports) {
    module.exports = window.pdfExport;
}
