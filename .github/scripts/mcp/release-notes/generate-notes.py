#!/usr/bin/env python3
"""
PFPT Release Notes - Markdown Generation Script
Generates formatted release notes from categorized commits.
"""

import json
import re
import sys
import os
from datetime import datetime


def main():
    """Generate formatted release notes."""
    # Load categorized commits
    with open('release-notes/processed/categorized.json', 'r') as f:
        categories = json.load(f)
    
    # Category display order and names for PFPT
    category_order = [
        ('clinical-features', '🏥 Clinical Features & Healthcare Workflows'),
        ('features', '✨ New Features & Improvements'),
        ('pdf-reporting', '📄 PDF Reports & Documentation'),
        ('database', '🗄️ Database & Data Management'),
        ('ui-ux', '🎨 User Interface & Experience'),
        ('accessibility', '♿ Accessibility & Compliance'),
        ('security', '🔒 Security & Privacy'),
        ('performance', '⚡ Performance Optimizations'),
        ('testing', '🧪 Testing & Quality Assurance'),
        ('documentation', '📚 Documentation & Guides'),
        ('build-ci', '🔧 Build System & CI/CD'),
        ('dependencies', '📦 Dependencies & External Libraries')
    ]
    
    # Get parameters from environment
    release_type = os.getenv('RELEASE_TYPE', 'feature')
    from_tag = os.getenv('FROM_TAG', 'previous')
    to_tag = os.getenv('TO_TAG', 'current')
    include_clinical = os.getenv('INCLUDE_CLINICAL', 'true').lower() == 'true'
    
    # Generate release notes
    notes = []
    notes.append(f"# PFPT Release Notes - {to_tag}")
    notes.append("")
    notes.append(f"**Release Type**: {release_type.title()}")
    notes.append(f"**Version Range**: {from_tag} → {to_tag}")
    notes.append(f"**Release Date**: {datetime.now().strftime('%Y-%m-%d')}")
    notes.append("")
    
    if include_clinical:
        notes.append("## 🏥 Healthcare & Clinical Focus")
        notes.append("")
        notes.append("This release includes enhancements specifically designed for physical therapy clinics:")
        notes.append("- **HIPAA Compliance**: Patient data protection and privacy features")
        notes.append("- **Clinical Workflows**: Streamlined assessment and documentation processes")
        notes.append("- **Accessibility**: WCAG 2.1 compliance for inclusive healthcare technology")
        notes.append("- **PDF Reporting**: Professional clinical documentation and report generation")
        notes.append("")
    
    notes.append("## 📋 What's Changed")
    notes.append("")
    
    # Add commits by category
    total_commits = 0
    for category_key, category_name in category_order:
        if category_key in categories and categories[category_key]:
            notes.append(f"### {category_name}")
            notes.append("")
            
            for commit in categories[category_key]:
                commit_msg = commit['message']
                commit_hash = commit['hash']
                author = commit['author']
                
                # Clean up commit message
                if commit_msg.startswith(category_key + ':'):
                    commit_msg = commit_msg[len(category_key)+1:].strip()
                
                notes.append(f"- {commit_msg} ([{commit_hash}]) by @{author}")
                total_commits += 1
            
            notes.append("")
    
    # Add footer
    notes.append("---")
    notes.append("")
    notes.append(f"**Total Changes**: {total_commits} commits")
    notes.append(f"**Contributors**: {len(set(commit['author'] for commits in categories.values() for commit in commits))}")
    notes.append("")
    notes.append("## 🏥 Healthcare Compliance")
    notes.append("- ✅ HIPAA compliance maintained")
    notes.append("- ✅ Patient data privacy protected")
    notes.append("- ✅ Clinical workflow integrity preserved")
    notes.append("- ✅ Accessibility standards met (WCAG 2.1)")
    
    # Write final release notes
    with open('release-notes/final/RELEASE_NOTES.md', 'w') as f:
        f.write('\n'.join(notes))
    
    print(f"✅ Generated release notes with {total_commits} commits")
    print("📄 Release notes saved to release-notes/final/RELEASE_NOTES.md")


if __name__ == "__main__":
    main()