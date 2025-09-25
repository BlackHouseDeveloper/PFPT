/**
 * PFPT Keyboard Navigation Test Script
 * Tests keyboard accessibility compliance for healthcare applications
 */

const { chromium } = require('playwright');

(async () => {
  const browser = await chromium.launch();
  const page = await browser.newPage();
  
  console.log('🧭 Starting keyboard navigation accessibility test...');
  
  try {
    await page.goto('http://localhost:8080');
    
    // Test tab navigation
    console.log('Testing tab navigation...');
    await page.keyboard.press('Tab');
    const focusedElement = await page.evaluate(() => document.activeElement.tagName);
    console.log('First focusable element:', focusedElement);
    
    // Test ARIA landmarks
    console.log('Checking ARIA landmarks...');
    const landmarks = await page.evaluate(() => {
      return Array.from(document.querySelectorAll('[role="main"], [role="navigation"], [role="banner"], [role="contentinfo"]')).length;
    });
    console.log('ARIA landmarks found:', landmarks);
    
    // Test skip links
    console.log('Testing skip links...');
    const skipLinks = await page.evaluate(() => {
      return Array.from(document.querySelectorAll('a[href^="#"]')).filter(link => 
        link.textContent.toLowerCase().includes('skip')
      ).length;
    });
    console.log('Skip links found:', skipLinks);
    
    // Test focus indicators
    console.log('Testing focus indicators...');
    await page.keyboard.press('Tab');
    const hasFocusOutline = await page.evaluate(() => {
      const focused = document.activeElement;
      const computed = window.getComputedStyle(focused);
      return computed.outline !== 'none' || computed.boxShadow !== 'none';
    });
    console.log('Focus indicators visible:', hasFocusOutline);
    
    // Healthcare-specific tests
    console.log('Testing healthcare form accessibility...');
    const formLabels = await page.evaluate(() => {
      const inputs = Array.from(document.querySelectorAll('input, select, textarea'));
      return inputs.filter(input => {
        const label = document.querySelector(`label[for="${input.id}"]`);
        const ariaLabel = input.getAttribute('aria-label');
        const ariaLabelledBy = input.getAttribute('aria-labelledby');
        return label || ariaLabel || ariaLabelledBy;
      }).length;
    });
    console.log('Properly labeled form controls:', formLabels);
    
    console.log('✅ Keyboard navigation test completed successfully');
    
  } catch (error) {
    console.error('❌ Keyboard navigation test failed:', error);
    process.exit(1);
  } finally {
    await browser.close();
  }
})();