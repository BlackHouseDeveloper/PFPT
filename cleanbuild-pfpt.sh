#!/usr/bin/env bash

# ============================================================================
# PFPT Clean Build Script - Enhanced Cross-Platform Build Tool
# ============================================================================
#
# This script provides a comprehensive build and test workflow for the 
# Physically Fit PT project with colored output, detailed logging, and
# cross-platform compatibility.
#
# Features:
# - Cross-platform compatibility (macOS, Linux, Windows with WSL)
# - Colored terminal output with fallback for unsupported terminals
# - Comprehensive build cleanup and validation
# - Detailed logging with timestamped output
# - Build step tracking and failure reporting
# - Automatic log file opening (macOS only)
#
# Usage:
#   chmod +x pfpt-cleanbuild.sh && ./pfpt-cleanbuild.sh
#
# Requirements:
#   - .NET 8.0 SDK or later
#   - Git (for version control operations)
#   - Bash 4.0+ (default on modern systems)
#
# ============================================================================

set -u  # Exit on undefined variables

# ---- Platform Detection & Color Configuration -----------------------------

# Detect operating system
OS_TYPE=""
case "$(uname -s)" in
    Darwin*)    OS_TYPE="macOS" ;;
    Linux*)     OS_TYPE="Linux" ;;
    CYGWIN*|MINGW*|MSYS*) OS_TYPE="Windows" ;;
    *)          OS_TYPE="Unknown" ;;
esac

# Configure colors based on terminal capabilities
# Check for color support and tput availability
if command -v tput >/dev/null 2>&1 && [ "$(tput colors 2>/dev/null || echo 0)" -ge 8 ]; then
  # Terminal supports colors
  BOLD="$(tput bold)"; RESET="$(tput sgr0)"
  RED="$(tput setaf 1)"; GREEN="$(tput setaf 2)"; 
  YELLOW="$(tput setaf 3)"; BLUE="$(tput setaf 4)"
  CYAN="$(tput setaf 6)"; MAGENTA="$(tput setaf 5)"
else
  # Fallback for terminals without color support
  BOLD=""; RESET=""
  RED=""; GREEN=""; YELLOW=""; BLUE=""; CYAN=""; MAGENTA=""
fi

# ---- Initialization & Logging Setup ---------------------------------------
# Generate timestamp for this build session
timestamp="$(date +"%Y%m%d-%H%M%S")"
logfile="build-output-${timestamp}.txt"

# Display build header with system information
echo -e "${BLUE}${BOLD}============================================================================${RESET}"
echo -e "${BLUE}${BOLD}PFPT Clean Build - Enhanced Cross-Platform Build Tool${RESET}"
echo -e "${BLUE}${BOLD}============================================================================${RESET}"
echo -e "${CYAN}Started:${RESET}     ${timestamp}"
echo -e "${CYAN}Platform:${RESET}    ${OS_TYPE}"
echo -e "${CYAN}Log file:${RESET}    ${logfile}"
echo -e "${CYAN}.NET SDK:${RESET}    $(dotnet --version 2>/dev/null || echo 'Not found')"
echo

# Initialize log file with header
{
  echo "PFPT Clean Build Log - ${timestamp}"
  echo "Platform: ${OS_TYPE}"
  echo ".NET SDK: $(dotnet --version 2>/dev/null || echo 'Not found')"
  echo "Working Directory: $(pwd)"
  echo "User: $(whoami)"
  echo "============================================================================"
  echo
} > "$logfile"

# ---- Environment Validation -----------------------------------------------

# Verify .NET SDK is available
if ! command -v dotnet >/dev/null 2>&1; then
  echo -e "${RED}${BOLD}✖ ERROR: .NET SDK not found on PATH${RESET}"
  {
    echo "ERROR: .NET SDK not found on PATH"
    echo "Please install .NET SDK 8.0 or later from:"
    echo "https://dotnet.microsoft.com/download/dotnet/8.0"
  } | tee -a "$logfile"
  exit 127
fi

# Check if we're in a .NET solution directory
if [ ! -f "PFPT.sln" ]; then
  echo -e "${YELLOW}${BOLD}⚠️  WARNING: PFPT.sln not found in current directory${RESET}"
  echo -e "${YELLOW}   Continuing anyway, but some operations may fail${RESET}"
  echo "WARNING: PFPT.sln not found in current directory" >> "$logfile"
fi

# ---- Build Step Tracking --------------------------------------------------

# Counters for build step results
STEP_OK=0
STEP_FAIL=0
STEP_SKIP=0

# Function to execute and track build steps
run_step() {
  local title="$1"
  shift
  local cmd=("$@")

  echo -e "\n${YELLOW}${BOLD}▶ ${title}${RESET}" | tee -a "$logfile"
  
  # Log the command being executed
  {
    echo "----------------------------------------"
    echo "STEP: ${title}"
    echo "COMMAND: ${cmd[*]}"
    echo "TIMESTAMP: $(date)"
    echo "----------------------------------------"
  } >> "$logfile"

  # Execute the command and capture exit code
  if "${cmd[@]}" >> "$logfile" 2>&1; then
    echo -e "${GREEN}✔ ${title}${RESET}"
    STEP_OK=$((STEP_OK + 1))
    return 0
  else
    local exit_code=$?
    echo -e "${RED}✖ ${title} (exit code: ${exit_code})${RESET}"
    STEP_FAIL=$((STEP_FAIL + 1))
    return $exit_code
  fi
}

# Function to skip a build step with logging
skip_step() {
  local title="$1"
  local reason="$2"
  
  echo -e "${MAGENTA}${BOLD}⏭️  Skipping: ${title}${RESET}"
  echo -e "${MAGENTA}   Reason: ${reason}${RESET}"
  echo "SKIPPED: ${title} - ${reason}" >> "$logfile"
  STEP_SKIP=$((STEP_SKIP + 1))
}

# ---- Pre-Build Cleanup Operations -----------------------------------------

extract_test_summary () {
  # Try to pull a friendly test summary from the log
  # Covers xUnit/VSTest formats commonly seen
  local summary=""
  summary="$(grep -E 'Passed!.*Failed:|Total tests:|Summary:' "$logfile" | tail -n 1 || true)"
  if [ -n "$summary" ]; then
    echo "$summary"
  else
    echo "See full test details in $logfile"
  fi
}


# ---------- Hardened pre-clean ----------
echo -e "${YELLOW}${BOLD}▶ Shutting down build servers...${RESET}" | tee -a "$logfile"
dotnet build-server shutdown >>"$logfile" 2>&1

echo -e "${YELLOW}${BOLD}▶ Removing Android lp/ intermediates (if present)...${RESET}" | tee -a "$logfile"
rm -rf "PhysicallyFitPT/obj/Debug/net8.0-android/lp" 2>>"$logfile"

echo -e "${YELLOW}${BOLD}▶ Removing bin/ and obj/ folders...${RESET}" | tee -a "$logfile"
find . -type d \( -name bin -o -name obj \) -exec rm -rf {} + 2>>"$logfile"
echo -e "${GREEN}✔ bin/obj removed${RESET}"

# ---------- Steps ----------
run_step "dotnet clean"   dotnet clean
run_step "dotnet restore" dotnet restore
run_step "dotnet build (warnaserror)" dotnet build -warnaserror
BUILD_EC=$?

TEST_EC=0
if [ $BUILD_EC -eq 0 ]; then
  run_step "dotnet test" dotnet test
  TEST_EC=$?
else
  echo -e "${RED}${BOLD}Skipping tests because build failed.${RESET}"
  STEP_FAIL=$((STEP_FAIL+1))
fi

# ---------- Summary ----------
echo -e "\n${BOLD}=========== SUMMARY ==========${RESET}"
echo -e "${GREEN}Successful steps: ${STEP_OK}${RESET}"
echo -e "${RED}Failed steps:     ${STEP_FAIL}${RESET}"

if [ $TEST_EC -eq 0 ] && [ $BUILD_EC -eq 0 ]; then
  echo -e "${GREEN}${BOLD}OVERALL: PASS ✅${RESET}"
else
  echo -e "${RED}${BOLD}OVERALL: FAIL ❌${RESET}"
fi

echo -e "\n${BLUE}${BOLD}Test Summary:${RESET} $(extract_test_summary)"

# ---------- Open log in TextEdit ----------
# -e flag opens directly in TextEdit on macOS
if command -v open >/dev/null 2>&1; then
  open -e "$logfile" >/dev/null 2>&1 &
fi

# Exit with non-zero if any step failed
exit $(( BUILD_EC != 0 || TEST_EC != 0 ))
