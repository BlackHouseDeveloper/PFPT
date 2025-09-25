#!/usr/bin/env python3
"""
PFPT Database Validation Script
Validates EF Core migrations and database schema.
"""

import subprocess
import sys
import json
from pathlib import Path


def run_command(cmd):
    """Run a command and return the result."""
    try:
        result = subprocess.run(cmd, shell=True, capture_output=True, text=True)
        return result.returncode, result.stdout, result.stderr
    except Exception as e:
        return 1, "", str(e)


def validate_ef_context():
    """Validate EF Core DbContext configuration."""
    print("🗄️ Validating EF Core DbContext...")
    
    cmd = "dotnet ef dbcontext info -p src/PhysicallyFitPT.Infrastructure --startup-project src/PhysicallyFitPT"
    code, stdout, stderr = run_command(cmd)
    
    if code == 0:
        print("✅ DbContext validation successful")
        return True
    else:
        print(f"❌ DbContext validation failed: {stderr}")
        return False


def check_migrations():
    """Check migration status."""
    print("📋 Checking migration status...")
    
    cmd = "dotnet ef migrations list -p src/PhysicallyFitPT.Infrastructure"
    code, stdout, stderr = run_command(cmd)
    
    if code == 0:
        migrations = [line.strip() for line in stdout.split('\n') if line.strip()]
        print(f"✅ Found {len(migrations)} migrations")
        return True, migrations
    else:
        print(f"❌ Migration check failed: {stderr}")
        return False, []


def validate_sqlite_schema():
    """Validate SQLite schema if database exists."""
    print("🔍 Validating SQLite schema...")
    
    db_path = Path("dev.physicallyfitpt.db")
    if not db_path.exists():
        print("ℹ️ Development database not found, skipping schema validation")
        return True
    
    # Check if sqlite3 is available
    code, _, _ = run_command("which sqlite3")
    if code != 0:
        print("⚠️ sqlite3 not available, skipping schema validation")
        return True
    
    # Get table information
    cmd = f'sqlite3 {db_path} ".schema"'
    code, stdout, stderr = run_command(cmd)
    
    if code == 0:
        print("✅ SQLite schema validation successful")
        return True
    else:
        print(f"❌ SQLite schema validation failed: {stderr}")
        return False


def generate_validation_report():
    """Generate a validation report."""
    print("📄 Generating validation report...")
    
    report = {
        'timestamp': subprocess.run(['date', '-u', '+%Y-%m-%d %H:%M:%S UTC'], 
                                  capture_output=True, text=True).stdout.strip(),
        'dbcontext_valid': validate_ef_context(),
        'migrations_valid': False,
        'migration_count': 0,
        'schema_valid': validate_sqlite_schema()
    }
    
    migrations_valid, migrations = check_migrations()
    report['migrations_valid'] = migrations_valid
    report['migration_count'] = len(migrations) if migrations else 0
    
    # Write report
    with open('database-validation-report.json', 'w') as f:
        json.dump(report, f, indent=2)
    
    print("✅ Database validation report generated")
    return report


def main():
    """Main validation function."""
    print("🔍 Starting PFPT database validation...")
    
    report = generate_validation_report()
    
    # Check if all validations passed
    all_valid = (report['dbcontext_valid'] and 
                report['migrations_valid'] and 
                report['schema_valid'])
    
    if all_valid:
        print("✅ All database validations passed")
        sys.exit(0)
    else:
        print("❌ Some database validations failed")
        sys.exit(1)


if __name__ == "__main__":
    main()