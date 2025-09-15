#!/usr/bin/env bash
set -euo pipefail

# PhysicallyFitPT (PFPT) Launch Script
# Prompts user to run PFPT in Web (Blazor), Android, or iOS mode.
# Uses .NET CLI for web and MAUI, and native tools for device management.

# Ensure required tools are available
command -v dotnet >/dev/null 2>&1 || { echo "❌ .NET CLI not found. Install .NET 8.0 SDK (see PFPT prerequisites:contentReference[oaicite:0]{index=0})." >&2; exit 1; }
command -v xcrun >/dev/null 2>&1 || { echo "❌ Xcode CLI tools not found. Install Xcode (required for iOS):contentReference[oaicite:1]{index=1})." >&2; exit 1; }
# Check for Android debug tool (optional warning)
command -v adb >/dev/null 2>&1 || echo "⚠️ 'adb' not found. Android SDK may not be installed."

# Prompt user to select project type
echo "Select project type to run:"
echo "1) Blazor WebAssembly (Web)"
echo "2) Android"
echo "3) iOS"
read -rp "Enter choice [1-3]: " choice

case "$choice" in
  1)
    # Blazor WebAssembly
    echo "Launching Blazor WebAssembly (PhysicallyFitPT.Web)..."
    # Run the Blazor WASM project using .NET CLI (PFPT README:contentReference[oaicite:2]{index=2})
    dotnet run --project PhysicallyFitPT.Web/PhysicallyFitPT.Web.csproj
    ;;
  2)
    # Android
    echo "Launching Android project..."
    if ! command -v adb >/dev/null 2>&1; then
      echo "❌ Android SDK (adb) is required but not found." >&2
      exit 1
    fi
    # Check for connected devices
    device_id=$(adb devices | tail -n +2 | awk '/device$/{print $1; exit}')
    if [ -z "$device_id" ]; then
      # No device: try emulator
      echo "No Android device detected. Listing available emulators..."
      if ! command -v emulator >/dev/null 2>&1; then
        echo "❌ Android emulator not found. Ensure Android SDK is installed." >&2
        exit 1
      fi
      avd_name=$(emulator -list-avds | head -n1)
      if [ -z "$avd_name" ]; then
        echo "❌ No Android Virtual Device (AVD) found. Create one first." >&2
        exit 1
      fi
      echo "Starting Android emulator: $avd_name"
      emulator -avd "$avd_name" >/dev/null 2>&1 &
      echo "Waiting for emulator to start..."
      adb wait-for-device
      sleep 5
      device_id=$(adb devices | tail -n +2 | awk '/device$/{print $1; exit}')
      if [ -z "$device_id" ]; then
        echo "❌ Emulator failed to start or be recognized." >&2
        exit 1
      fi
      echo "Emulator started (Device ID: $device_id)."
    else
      echo "Using connected Android device: $device_id"
    fi
    # Build and run the Android app on the device
    echo "Running MAUI Android app on device $device_id..."
    # Launch .NET MAUI Android app via CLI (per example:contentReference[oaicite:3]{index=3})
    dotnet build -t:Run -f net8.0-android PhysicallyFitPT/PhysicallyFitPT.csproj
    ;;
  3)
    # iOS
    echo "Launching iOS project..."
    sim_info=$(xcrun simctl list devices)
    # Use booted simulator if available
    booted_line=$(echo "$sim_info" | grep "Booted" | head -n1 || true)
    if [ -n "$booted_line" ]; then
      device_udid=$(echo "$booted_line" | sed -E 's/.*\(([A-F0-9-]+)\).*/\1/')
      device_name=$(echo "$booted_line" | awk -F ' \\(' '{print $1}')
      echo "Using booted simulator: $device_name (UDID: $device_udid)"
    else
      # Pick first available (Shutdown) iOS device
      avail_line=$(echo "$sim_info" | grep -E "(iPhone|iPad).*\\(Shutdown\\)" | head -n1 || true)
      if [ -z "$avail_line" ]; then
        echo "❌ No available iOS simulator found. Please create one in Xcode." >&2
        exit 1
      fi
      device_udid=$(echo "$avail_line" | sed -E 's/.*\(([A-F0-9-]+)\).*/\1/')
      device_name=$(echo "$avail_line" | awk -F ' \\(' '{print $1}')
      echo "Booting iOS simulator: $device_name (UDID: $device_udid)"
      xcrun simctl boot "$device_udid"
      echo "Waiting for simulator to boot..."
      sleep 5
    fi
    # Build and run the iOS app on the chosen simulator
    echo "Running MAUI iOS app on simulator $device_name..."
    # Launch iOS app using simulator UDID (MS docs:contentReference[oaicite:4]{index=4})
    dotnet build -t:Run -f net8.0-ios -p:_DeviceName=:v2:udid=$device_udid PhysicallyFitPT/PhysicallyFitPT.csproj
    ;;
  *)
    echo "Invalid choice. Exiting." >&2
    exit 1
    ;;
esac
