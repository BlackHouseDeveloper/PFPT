#!/usr/bin/env python3
"""
PFPT Release Notes - Commit Categorization Script
Analyzes git commits and categorizes them based on PFPT-specific patterns.
"""

import re
import sys
import json
from collections import defaultdict


def categorize_commit(commit_msg, commit_body=""):
    """Categorize a commit based on message and body content."""
    commit_lower = commit_msg.lower()
    body_lower = commit_body.lower()
    full_text = (commit_lower + " " + body_lower).strip()
    
    # Clinical/Healthcare specific categories (priority)
    if any(term in full_text for term in ['hipaa', 'phi', 'patient data', 'clinical', 'assessment', 'therapy']):
        return 'clinical-features'
    
    # Dependency updates (high priority)
    if any(term in commit_lower for term in ['update dependencies', 'bump', 'upgrade', 'dependency']):
        return 'dependencies'
    
    # PDF and reporting (PFPT-specific)
    if any(term in full_text for term in ['pdf', 'report', 'questpdf', 'export']):
        return 'pdf-reporting'
    
    # Accessibility (healthcare compliance)
    if any(term in full_text for term in ['accessibility', 'a11y', 'wcag', 'screen reader']):
        return 'accessibility'
    
    # Database and EF Core
    if any(term in full_text for term in ['database', 'migration', 'ef core', 'sqlite', 'entity framework']):
        return 'database'
    
    # Testing
    if any(term in full_text for term in ['test', 'testing', 'xunit', 'nunit', 'unit test']):
        return 'testing'
    
    # Documentation
    if any(term in full_text for term in ['doc', 'documentation', 'readme', 'guide']):
        return 'documentation'
    
    # Build and CI
    if any(term in full_text for term in ['ci', 'build', 'workflow', 'github actions', 'pipeline']):
        return 'build-ci'
    
    # UI/UX improvements
    if any(term in full_text for term in ['ui', 'ux', 'interface', 'blazor', 'maui', 'styling']):
        return 'ui-ux'
    
    # Security
    if any(term in full_text for term in ['security', 'auth', 'authentication', 'authorization']):
        return 'security'
    
    # Performance
    if any(term in full_text for term in ['performance', 'optimization', 'speed', 'memory']):
        return 'performance'
    
    # Default to features for unmatched commits
    return 'features'


def main():
    """Process commits and categorize them."""
    # Process commits
    categories = defaultdict(list)
    
    with open('release-notes/raw/commits.txt', 'r') as f:
        for line in f:
            if '|' in line:
                parts = line.strip().split('|')
                if len(parts) >= 6:
                    hash_short = parts[0]
                    message = parts[1]
                    author = parts[2]
                    email = parts[3] 
                    date = parts[4]
                    body = parts[5] if len(parts) > 5 else ""
                    
                    category = categorize_commit(message, body)
                    categories[category].append({
                        'hash': hash_short,
                        'message': message,
                        'author': author,
                        'email': email,
                        'date': date,
                        'body': body
                    })
    
    # Write categorized results
    with open('release-notes/processed/categorized.json', 'w') as f:
        json.dump(dict(categories), f, indent=2)
    
    # Generate category summary
    with open('release-notes/processed/summary.txt', 'w') as f:
        f.write("PFPT Release Notes Categories:\n")
        f.write("="*40 + "\n\n")
        for category, commits in categories.items():
            f.write(f"{category}: {len(commits)} commits\n")
    
    print(f"✅ Categorized {sum(len(commits) for commits in categories.values())} commits into {len(categories)} categories")


if __name__ == "__main__":
    main()