/**
 * Accessibility Utilities - JavaScript Interop
 * Companion file to AccessibilityUtilities.cs
 * 
 * Place in: wwwroot/js/accessibility.js
 * Reference in: App.razor or main layout
 */

window.accessibility = {
  /**
   * Announce to screen readers using aria-live regions
   */
  announceToScreenReader: function(message, priority = 'polite') {
    const announcement = document.createElement('div');
    announcement.setAttribute('role', 'status');
    announcement.setAttribute('aria-live', priority);
    announcement.setAttribute('aria-atomic', 'true');
    announcement.className = 'sr-only';
    announcement.textContent = message;
    
    document.body.appendChild(announcement);
    
    // Remove after announcement
    setTimeout(() => {
      if (document.body.contains(announcement)) {
        document.body.removeChild(announcement);
      }
    }, 1000);
  },

  /**
   * Trap focus within a modal or dialog
   */
  trapFocus: function(elementId) {
    const element = document.getElementById(elementId);
    if (!element) {
      console.warn(`Element with id "${elementId}" not found for focus trap`);
      return null;
    }

    const focusableElements = element.querySelectorAll(
      'button, [href], input, select, textarea, [tabindex]:not([tabindex="-1"])'
    );
    
    if (focusableElements.length === 0) {
      console.warn(`No focusable elements found in element "${elementId}"`);
      return null;
    }

    const firstFocusable = focusableElements[0];
    const lastFocusable = focusableElements[focusableElements.length - 1];
    
    const handleTabKey = (e) => {
      if (e.key !== 'Tab') return;
      
      if (e.shiftKey) {
        // Shift + Tab
        if (document.activeElement === firstFocusable) {
          lastFocusable?.focus();
          e.preventDefault();
        }
      } else {
        // Tab
        if (document.activeElement === lastFocusable) {
          firstFocusable?.focus();
          e.preventDefault();
        }
      }
    };
    
    element.addEventListener('keydown', handleTabKey);
    
    // Focus first element
    firstFocusable?.focus();
    
    // Return cleanup function identifier
    return {
      cleanup: () => {
        element.removeEventListener('keydown', handleTabKey);
      }
    };
  },

  /**
   * Restore focus to previously focused element
   */
  restoreFocus: function(elementId) {
    const element = document.getElementById(elementId);
    if (element && typeof element.focus === 'function') {
      element.focus();
    } else {
      console.warn(`Could not restore focus to element "${elementId}"`);
    }
  },

  /**
   * Check if element is visible to assistive technology
   */
  isElementAccessible: function(elementId) {
    const element = document.getElementById(elementId);
    if (!element) {
      console.warn(`Element with id "${elementId}" not found`);
      return false;
    }

    const rect = element.getBoundingClientRect();
    const style = getComputedStyle(element);

    return (
      rect.width > 0 &&
      rect.height > 0 &&
      style.visibility !== 'hidden' &&
      style.display !== 'none' &&
      style.opacity !== '0'
    );
  },

  /**
   * Add keyboard event handlers for common patterns
   */
  addKeyboardHandlers: function(elementId, handlers = {}) {
    const element = document.getElementById(elementId);
    if (!element) {
      console.warn(`Element with id "${elementId}" not found`);
      return null;
    }

    const handleKeyDown = (e) => {
      switch (e.key) {
        case 'Enter':
          handlers.onEnter?.();
          break;
        case 'Escape':
          handlers.onEscape?.();
          break;
        case ' ':
          handlers.onSpace?.();
          e.preventDefault(); // Prevent page scroll
          break;
        case 'ArrowUp':
          handlers.onArrowUp?.();
          e.preventDefault();
          break;
        case 'ArrowDown':
          handlers.onArrowDown?.();
          e.preventDefault();
          break;
        case 'ArrowLeft':
          handlers.onArrowLeft?.();
          break;
        case 'ArrowRight':
          handlers.onArrowRight?.();
          break;
      }
    };
    
    element.addEventListener('keydown', handleKeyDown);
    
    return {
      cleanup: () => {
        element.removeEventListener('keydown', handleKeyDown);
      }
    };
  },

  /**
   * Ensure minimum touch target size (44x44px)
   */
  validateTouchTargetSize: function(elementId) {
    const element = document.getElementById(elementId);
    if (!element) {
      console.warn(`Element with id "${elementId}" not found`);
      return false;
    }

    const rect = element.getBoundingClientRect();
    const minSize = 44; // pixels
    
    const isValid = rect.width >= minSize && rect.height >= minSize;
    
    if (!isValid) {
      console.warn('Touch target too small:', {
        elementId,
        width: rect.width,
        height: rect.height,
        required: `${minSize}x${minSize}px`
      });
    }
    
    return isValid;
  },

  /**
   * Create skip to main content link
   */
  createSkipLink: function(targetId, label = 'Skip to main content') {
    // Check if skip link already exists
    if (document.getElementById('skip-link')) {
      return;
    }

    const skipLink = document.createElement('a');
    skipLink.id = 'skip-link';
    skipLink.href = `#${targetId}`;
    skipLink.textContent = label;
    skipLink.className = 'sr-only focus:not-sr-only focus:absolute focus:top-4 focus:left-4 focus:z-50 focus:bg-blue-600 focus:text-white focus:px-4 focus:py-2 focus:rounded';
    
    skipLink.addEventListener('click', (e) => {
      e.preventDefault();
      const target = document.getElementById(targetId);
      if (target) {
        target.focus();
        target.scrollIntoView({ behavior: 'smooth' });
      }
    });
    
    // Insert as first child of body
    document.body.insertBefore(skipLink, document.body.firstChild);
  },

  /**
   * Update live region with new content
   */
  updateLiveRegion: function(regionId, message) {
    const region = document.getElementById(regionId);
    if (!region) {
      console.warn(`Live region with id "${regionId}" not found`);
      return;
    }

    region.textContent = message;
  },

  /**
   * Create a live region for dynamic announcements
   */
  createLiveRegion: function(regionId, priority = 'polite') {
    // Check if region already exists
    if (document.getElementById(regionId)) {
      return regionId;
    }

    const region = document.createElement('div');
    region.id = regionId;
    region.setAttribute('role', 'status');
    region.setAttribute('aria-live', priority);
    region.setAttribute('aria-atomic', 'true');
    region.className = 'sr-only';
    document.body.appendChild(region);
    
    return regionId;
  },

  /**
   * Remove a live region
   */
  removeLiveRegion: function(regionId) {
    const region = document.getElementById(regionId);
    if (region && document.body.contains(region)) {
      document.body.removeChild(region);
    }
  }
};

// Export for TypeScript/ES modules if needed
if (typeof module !== 'undefined' && module.exports) {
  module.exports = window.accessibility;
}
