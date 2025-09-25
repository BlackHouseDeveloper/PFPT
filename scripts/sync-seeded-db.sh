#!/usr/bin/env bash
set -euo pipefail

# Synchronise the seeded SQLite database (pfpt.db) into a running MAUI target.
#
# Usage:
#   scripts/sync-seeded-db.sh android [path/to/pfpt.db]
#   scripts/sync-seeded-db.sh ios [path/to/pfpt.db]
#
# The database path defaults to src/PhysicallyFitPT.Api/pfpt.db.

TARGET="${1:-}"
DB_PATH="${2:-src/PhysicallyFitPT.Api/pfpt.db}"

if [[ -z "$TARGET" ]]; then
  echo "Usage: $0 <android|ios> [path/to/pfpt.db]" >&2
  exit 1
fi

if [[ ! -f "$DB_PATH" ]]; then
  echo "❌ Database file not found at $DB_PATH" >&2
  exit 1
fi

case "$TARGET" in
  android)
    if ! command -v adb >/dev/null 2>&1; then
      echo "❌ adb not found on PATH. Install/enable the Android SDK platform-tools." >&2
      exit 1
    fi

    PACKAGE_ID="com.companyname.physicallyfitpt"
    TMP_REMOTE="/data/local/tmp/pfpt.db"

    echo "Pushing $DB_PATH to Android device as $TMP_REMOTE ..."
    adb push "$DB_PATH" "$TMP_REMOTE" >/dev/null

    echo "Copying database into app sandbox (run-as $PACKAGE_ID) ..."
    adb shell run-as "$PACKAGE_ID" mkdir -p files >/dev/null 2>&1 || true
    adb shell run-as "$PACKAGE_ID" cp "$TMP_REMOTE" files/physicallyfitpt.db
    adb shell rm "$TMP_REMOTE"

    echo "✅ Android app database updated. Restart the app to pick up changes."
    ;;

  ios)
    if ! command -v xcrun >/dev/null 2>&1; then
      echo "❌ xcrun not found. Install Xcode command-line tools." >&2
      exit 1
    fi

    APP_ID="com.companyname.physicallyfitpt"
    CONTAINER_PATH=$(xcrun simctl get_app_container booted "$APP_ID" data 2>/dev/null || true)

    if [[ -z "$CONTAINER_PATH" ]]; then
      echo "❌ Could not resolve data container for $APP_ID. Is the simulator booted and the app installed?" >&2
      exit 1
    fi

    DEST="$CONTAINER_PATH/Documents/physicallyfitpt.db"
    mkdir -p "$(dirname "$DEST")"

    echo "Copying $DB_PATH to iOS simulator container: $DEST"
    cp "$DB_PATH" "$DEST"

    echo "✅ iOS simulator database updated. Relaunch the app to load the new data."
    ;;

  *)
    echo "Unsupported target '$TARGET'. Use 'android' or 'ios'." >&2
    exit 1
    ;;
esac
