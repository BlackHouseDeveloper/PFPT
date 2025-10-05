#!/usr/bin/env python3
"""Check the active SQLite database path and report row counts."""

from __future__ import annotations

import argparse
import json
import os
import sqlite3
import sys
from pathlib import Path
from typing import Dict, Iterable, Optional, Tuple

REPO_ROOT = Path(__file__).resolve().parents[1]
DEFAULTS = {
    "api": {
        "config_dir": REPO_ROOT / "src/PhysicallyFitPT.Api",
        "fallback": REPO_ROOT / "pfpt.db",
    },
    "seeder": {
        "config_dir": REPO_ROOT / "src/PhysicallyFitPT.Seeder",
        "fallback": REPO_ROOT / "dev.physicallyfitpt.db",
    },
}


def load_connection_string(config_files: Iterable[Path]) -> Optional[str]:
    connection: Optional[str] = None
    for config_path in config_files:
        if not config_path.exists():
            continue
        try:
            data = json.loads(config_path.read_text(encoding="utf-8"))
        except json.JSONDecodeError:
            continue
        connection_strings = data.get("ConnectionStrings")
        if isinstance(connection_strings, dict):
            candidate = connection_strings.get("DefaultConnection")
            if candidate:
                connection = candidate
    return connection


def parse_data_source(connection_string: Optional[str]) -> Optional[str]:
    if not connection_string:
        return None
    parts = [segment.strip() for segment in connection_string.split(";") if segment.strip()]
    for part in parts:
        if "=" not in part:
            continue
        key, value = part.split("=", 1)
        if key.strip().lower() in {"data source", "datasource", "filename"}:
            return value.strip()
    return None


def resolve_database_path(context: str, environment: str) -> Tuple[Path, str]:
    env_override = os.getenv("PFP_DB_PATH")
    if env_override:
        return Path(env_override).expanduser(), "PFP_DB_PATH environment variable"

    defaults = DEFAULTS[context]
    config_dir = defaults["config_dir"].resolve()
    config_files = [config_dir / "appsettings.json"]
    env_config = config_dir / f"appsettings.{environment}.json"
    if env_config != config_files[0]:
        config_files.append(env_config)

    connection_string = load_connection_string(config_files)
    data_source = parse_data_source(connection_string)
    if data_source:
        candidate = Path(data_source).expanduser()
        if not candidate.is_absolute():
            candidate = (config_dir / candidate)
            candidate = candidate.resolve()
            if not candidate.exists():
                current = config_dir.parent
                while current is not None:
                    alternative = (current / Path(data_source)).resolve()
                    if alternative.exists():
                        candidate = alternative
                        break
                    current = current.parent
        return candidate, "ConnectionStrings:DefaultConnection"

    return defaults["fallback"].resolve(), "context fallback"


def collect_counts(db_path: Path) -> Dict[str, int]:
    counts: Dict[str, int] = {}
    if not db_path.exists():
        return counts
    try:
        with sqlite3.connect(f"file:{db_path}?mode=ro", uri=True) as conn:
            cursor = conn.cursor()
            cursor.execute(
                "SELECT name FROM sqlite_master WHERE type='table' AND name NOT LIKE 'sqlite_%' ORDER BY name"
            )
            tables = [row[0] for row in cursor.fetchall()]
            for table in tables:
                safe_table = table.replace('"', '""')
                cursor.execute(f'SELECT COUNT(*) FROM "{safe_table}"')
                count = cursor.fetchone()[0]
                counts[table] = count
    except sqlite3.Error as exc:
        print(f"Error opening database: {exc}", file=sys.stderr)
    return counts


def main() -> int:
    parser = argparse.ArgumentParser(description="Inspect the PFPT SQLite database")
    parser.add_argument(
        "--context",
        choices=sorted(DEFAULTS.keys()),
        default="api",
        help="Which application context to emulate when resolving the database path",
    )
    parser.add_argument(
        "--environment",
        default=os.getenv("ASPNETCORE_ENVIRONMENT", "Development"),
        help="Environment name used when loading appsettings.{Environment}.json",
    )
    parser.add_argument(
        "--require-data",
        action="store_true",
        help="Exit with a non-zero status if the database is missing or contains no tables.",
    )
    parser.add_argument(
        "--require-tables",
        nargs="*",
        default=[],
        metavar="TABLE",
        help="Table names that must exist in the database.",
    )
    parser.add_argument(
        "--require-nonzero",
        action="store_true",
        help="When used with --require-tables, also require each table to contain at least one row. If no tables are specified, all tables must be non-empty.",
    )

    args = parser.parse_args()

    db_path, source = resolve_database_path(args.context, args.environment)
    db_path = db_path if db_path.is_absolute() else (REPO_ROOT / db_path)

    print(f"Context: {args.context}")
    print(f"Environment: {args.environment}")
    print(f"Resolved from: {source}")
    print(f"Database path: {db_path}")
    exists = db_path.exists()
    print(f"Exists: {'yes' if exists else 'no'}")

    if not exists:
        print("Database file not found")
        return 1 if args.require_data else 0

    counts = collect_counts(db_path)
    if not counts:
        print("No table data available (database missing or empty)")
        return 1 if args.require_data else 0

    required_tables = set(args.require_tables)
    if required_tables:
        missing = sorted(required_tables.difference(counts.keys()))
        if missing:
            print(f"Missing required tables: {', '.join(missing)}", file=sys.stderr)
            return 1

    if args.require_nonzero:
        targets = required_tables or set(counts.keys())
        zero_tables = sorted(table for table in targets if counts.get(table, 0) == 0)
        if zero_tables:
            print(f"Tables with zero rows: {', '.join(zero_tables)}", file=sys.stderr)
            return 1

    total_rows = sum(counts.values())
    non_empty = {name: count for name, count in counts.items() if count}

    print(f"Tables found: {len(counts)} | Total rows: {total_rows}")
    if non_empty:
        print("Sample populated tables:")
        for name, count in sorted(non_empty.items(), key=lambda item: item[1], reverse=True)[:10]:
            print(f"  - {name}: {count}")
    else:
        print("All tables are currently empty")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
