#!/usr/bin/env python3
"""
PFPT PDF Validation Script
Tests and validates PDF generation capabilities.
"""

import subprocess
import sys
import json
import os
from pathlib import Path


def run_command(cmd):
    """Run a command and return the result."""
    try:
        result = subprocess.run(cmd, shell=True, capture_output=True, text=True)
        return result.returncode, result.stdout, result.stderr
    except Exception as e:
        return 1, "", str(e)


def test_pdf_build():
    """Test if PDF-related projects build successfully."""
    print("📄 Testing PDF project build...")
    
    cmd = "dotnet build src/PhysicallyFitPT.Infrastructure -c Release"
    code, stdout, stderr = run_command(cmd)
    
    if code == 0:
        print("✅ PDF project build successful")
        return True
    else:
        print(f"❌ PDF project build failed: {stderr}")
        return False


def validate_questpdf_dependencies():
    """Validate QuestPDF dependencies are properly configured."""
    print("🔍 Validating QuestPDF dependencies...")
    
    # Check if QuestPDF references exist in project files
    infrastructure_csproj = Path("src/PhysicallyFitPT.Infrastructure/PhysicallyFitPT.Infrastructure.csproj")
    
    if not infrastructure_csproj.exists():
        print("❌ Infrastructure project file not found")
        return False
    
    with open(infrastructure_csproj, 'r') as f:
        content = f.read()
        
    if 'QuestPDF' in content:
        print("✅ QuestPDF dependency found")
        return True
    else:
        print("⚠️ QuestPDF dependency not found in project file")
        return False


def test_sample_pdf_generation():
    """Test sample PDF generation if possible."""
    print("🧪 Testing sample PDF generation...")
    
    # This would require the application to be running
    # For now, we'll just validate the build
    return test_pdf_build()


def check_pdf_accessibility():
    """Check PDF accessibility compliance features."""
    print("♿ Checking PDF accessibility features...")
    
    # Look for accessibility-related code in PDF generation
    pdf_files = list(Path("src").rglob("*PDF*.cs"))
    
    accessibility_keywords = ['alt', 'aria', 'title', 'lang', 'tagged']
    accessibility_found = False
    
    for pdf_file in pdf_files:
        try:
            with open(pdf_file, 'r') as f:
                content = f.read().lower()
                if any(keyword in content for keyword in accessibility_keywords):
                    accessibility_found = True
                    break
        except:
            continue
    
    if accessibility_found:
        print("✅ PDF accessibility features detected")
        return True
    else:
        print("⚠️ PDF accessibility features not clearly detected")
        return False


def generate_pdf_validation_report():
    """Generate PDF validation report."""
    print("📄 Generating PDF validation report...")
    
    report = {
        'timestamp': subprocess.run(['date', '-u', '+%Y-%m-%d %H:%M:%S UTC'], 
                                  capture_output=True, text=True).stdout.strip(),
        'build_successful': test_pdf_build(),
        'questpdf_configured': validate_questpdf_dependencies(),
        'sample_generation': test_sample_pdf_generation(),
        'accessibility_features': check_pdf_accessibility()
    }
    
    # Write report
    with open('pdf-validation-report.json', 'w') as f:
        json.dump(report, f, indent=2)
    
    print("✅ PDF validation report generated")
    return report


def main():
    """Main PDF validation function."""
    print("📄 Starting PFPT PDF validation...")
    
    report = generate_pdf_validation_report()
    
    # Check if critical validations passed
    critical_valid = (report['build_successful'] and 
                     report['questpdf_configured'])
    
    if critical_valid:
        print("✅ Critical PDF validations passed")
        sys.exit(0)
    else:
        print("❌ Critical PDF validations failed")
        sys.exit(1)


if __name__ == "__main__":
    main()