#!/usr/bin/env bash
set -euo pipefail

# PFPT launch helper that targets the new multi-project solution layout.
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
ROOT_DIR="$SCRIPT_DIR"
WEB_CSPROJ="$ROOT_DIR/src/PhysicallyFitPT.Web/PhysicallyFitPT.Web.csproj"
MAUI_CSPROJ="$ROOT_DIR/src/PhysicallyFitPT.Maui/PhysicallyFitPT.Maui.csproj"
API_CSPROJ="$ROOT_DIR/src/PhysicallyFitPT.Api/PhysicallyFitPT.Api.csproj"
API_URL="${API_URL:-http://localhost:7001}"
API_PORT="${API_URL##*:}"
API_PID=""

cleanup() {
  if [[ -n "$API_PID" ]]; then
    if kill -0 "$API_PID" 2>/dev/null; then
      echo "Stopping API (PID $API_PID)..."
      kill "$API_PID" 2>/dev/null || true
      wait "$API_PID" 2>/dev/null || true
    fi
  fi
}

trap cleanup EXIT INT

if [[ -d "$HOME/.dotnet-sdk" && -x "$HOME/.dotnet-sdk/dotnet" ]]; then
  export DOTNET_ROOT="${DOTNET_ROOT:-$HOME/.dotnet-sdk}"
  case ":$PATH:" in
    *":$DOTNET_ROOT:"*) ;;
    *) export PATH="$DOTNET_ROOT:$PATH" ;;
  esac
fi

if ! command -v dotnet >/dev/null 2>&1; then
  echo "❌ .NET SDK not found. Install .NET 8.0+ and try again." >&2
  exit 1
fi

if [[ ! -f "$WEB_CSPROJ" ]]; then
  echo "❌ Web project not found at $WEB_CSPROJ" >&2
  exit 1
fi

if [[ ! -f "$MAUI_CSPROJ" ]]; then
  echo "❌ MAUI project not found at $MAUI_CSPROJ" >&2
  exit 1
fi

if [[ ! -f "$API_CSPROJ" ]]; then
  echo "❌ API project not found at $API_CSPROJ" >&2
  exit 1
fi

start_api() {
  if [[ -n "${SKIP_API:-}" ]]; then
    return
  fi

  if lsof -Pi :"$API_PORT" -sTCP:LISTEN -t >/dev/null 2>&1; then
    echo "ℹ️  API already listening on port $API_PORT; skipping auto-start."
    return
  fi

  echo "Starting PhysicallyFitPT.Api on $API_URL..."
  dotnet run --project "$API_CSPROJ" --no-build --urls "$API_URL" >/tmp/pfpt-api.log 2>&1 &
  API_PID=$!
  sleep 5

  if ! kill -0 "$API_PID" 2>/dev/null; then
    echo "❌ Failed to start PhysicallyFitPT.Api. Check /tmp/pfpt-api.log for details." >&2
    exit 1
  fi

  echo "✅ API started (PID $API_PID)."
}

echo "Select project type to run:"
echo "1) Blazor WebAssembly (Web)"
echo "2) Android"
echo "3) iOS"
read -rp "Enter choice [1-3]: " choice

case "$choice" in
  1)
    echo "Launching Blazor WebAssembly (PhysicallyFitPT.Web)..."
    start_api
    dotnet run --project "$WEB_CSPROJ"
    ;;
  2)
    if ! command -v adb >/dev/null 2>&1; then
      echo "❌ Android SDK (adb) is required but was not found on PATH." >&2
      exit 1
    fi

    echo "Launching Android project..."
    start_api
    device_id=$(adb devices | tail -n +2 | awk '/device$/{print $1; exit}')

    if [[ -z "$device_id" ]]; then
      echo "No Android device detected. Attempting to start the first available emulator..."
      if ! command -v emulator >/dev/null 2>&1; then
        echo "❌ Android emulator binary not found. Ensure the Android SDK tools are installed." >&2
        exit 1
      fi

      avd_name=$(emulator -list-avds | head -n1)
      if [[ -z "$avd_name" ]]; then
        echo "❌ No Android Virtual Devices found. Create one with Android Studio or avdmanager first." >&2
        exit 1
      fi

      echo "Starting Android emulator: $avd_name"
      nohup emulator -avd "$avd_name" >/dev/null 2>&1 &
      echo "Waiting for emulator to connect..."
      adb wait-for-device

      echo "Waiting for Android to finish booting..."
      until [[ "$(adb shell getprop sys.boot_completed 2>/dev/null | tr -d '\r')" == "1" ]]; do
        sleep 2
      done
      echo "✅ Emulator booted."
      device_id=$(adb devices | tail -n +2 | awk '/device$/{print $1; exit}')
    fi

    if [[ -z "$device_id" ]]; then
      echo "❌ Unable to find an active Android device or emulator." >&2
      exit 1
    fi

    echo "Running MAUI Android app on device $device_id..."
    dotnet build -t:Run -f net8.0-android "$MAUI_CSPROJ"
    ;;
  3)
    if ! command -v xcrun >/dev/null 2>&1; then
      echo "❌ Xcode command-line tools (xcrun) are required for iOS development." >&2
      exit 1
    fi

    echo "Launching iOS project..."
    start_api
    sim_info=$(xcrun simctl list devices)
    booted_line=$(echo "$sim_info" | grep "(Booted)" | head -n1 || true)

    if [[ -n "$booted_line" ]]; then
      device_udid=$(echo "$booted_line" | sed -E 's/.*\(([A-F0-9-]+)\).*/\1/')
      device_name=$(echo "$booted_line" | sed -E 's/^\s*([^()]+)\s*\(.*$/\1/')
    else
      avail_line=$(echo "$sim_info" | grep -E "(iPhone|iPad).*\(Shutdown\)" | head -n1 || true)
      if [[ -z "$avail_line" ]]; then
        echo "❌ No available iOS simulators found. Create one via Xcode." >&2
        exit 1
      fi
      device_udid=$(echo "$avail_line" | sed -E 's/.*\(([A-F0-9-]+)\).*/\1/')
      device_name=$(echo "$avail_line" | sed -E 's/^\s*([^()]+)\s*\(.*$/\1/')
      echo "Booting iOS simulator: $device_name ($device_udid)"
      xcrun simctl boot "$device_udid" >/dev/null
      echo "Waiting for simulator to boot..."
      sleep 5
    fi

    echo "Running MAUI iOS app on simulator..."
    dotnet build -t:Run -f net8.0-ios -p:_DeviceName=:v2:udid=$device_udid "$MAUI_CSPROJ"
    ;;
  *)
    echo "Invalid choice. Exiting." >&2
    exit 1
    ;;
esac
