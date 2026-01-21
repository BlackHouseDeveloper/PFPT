// Mobile detection service for Blazor
// Provides reactive viewport size detection with media queries

const MOBILE_BREAKPOINT = 768;
let mediaQueryList = null;
let dotnetHelper = null;

/**
 * Initializes the mobile listener with a media query.
 * @param {number} breakpoint - The breakpoint pixel value (default 768)
 * @param {object} helper - DotNet object reference for callbacks
 */
export function setupMobileListener(breakpoint, helper) {
    if (mediaQueryList !== null) {
        // Already initialized
        return;
    }

    dotnetHelper = helper;
    const query = `(max-width: ${breakpoint - 1}px)`;
    mediaQueryList = window.matchMedia(query);

    // Handle media query changes
    const handleChange = (e) => {
        if (dotnetHelper) {
            dotnetHelper.invokeMethodAsync('OnMediaQueryChange', e.matches);
        }
    };

    // Use addEventListener for better cross-browser support
    if (mediaQueryList.addEventListener) {
        mediaQueryList.addEventListener('change', handleChange);
    } else {
        // Fallback for older browsers
        mediaQueryList.addListener(handleChange);
    }
}

/**
 * Gets the current mobile state based on viewport width.
 * @param {number} breakpoint - The breakpoint pixel value (default 768)
 * @returns {boolean} - True if viewport width is less than breakpoint
 */
export function getIsMobile(breakpoint) {
    return window.innerWidth < breakpoint;
}

/**
 * Cleans up the media query listener.
 */
export function cleanupMobileListener() {
    if (mediaQueryList !== null) {
        if (mediaQueryList.removeEventListener) {
            mediaQueryList.removeEventListener('change', null);
        } else {
            // Fallback for older browsers
            mediaQueryList.removeListener(null);
        }
        mediaQueryList = null;
    }
    dotnetHelper = null;
}

/**
 * Gets the current breakpoint value.
 * @returns {number} - The mobile breakpoint in pixels
 */
export function getBreakpoint() {
    return MOBILE_BREAKPOINT;
}

/**
 * Gets the current window width.
 * @returns {number} - Current viewport width in pixels
 */
export function getWindowWidth() {
    return window.innerWidth;
}

/**
 * Gets the current window height.
 * @returns {number} - Current viewport height in pixels
 */
export function getWindowHeight() {
    return window.innerHeight;
}

/**
 * Gets the current viewport dimensions as an object.
 * @returns {object} - Object with width and height properties
 */
export function getViewportSize() {
    return {
        width: window.innerWidth,
        height: window.innerHeight
    };
}
