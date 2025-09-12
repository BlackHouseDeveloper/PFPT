#!/bin/bash
# PFPT Clean Build Script (color + summary + opens log)
# Usage: chmod +x pfpt-cleanbuild-colored.sh && ./pfpt-cleanbuild-colored.sh

set -u

# ---------- Colors ----------
if command -v tput >/dev/null 2>&1 && [ "$(tput colors 2>/dev/null || echo 0)" -ge 8 ]; then
  BOLD="$(tput bold)"; RESET="$(tput sgr0)"
  RED="$(tput setaf 1)"; GREEN="$(tput setaf 2)"; YELLOW="$(tput setaf 3)"; BLUE="$(tput setaf 4)"
else
  BOLD=""; RESET=""
  RED=""; GREEN=""; YELLOW=""; BLUE=""
fi

# ---------- Prep ----------
timestamp="$(date +"%Y%m%d-%H%M%S")"
logfile="build-output-${timestamp}.txt"

echo -e "${BLUE}${BOLD}PFPT Clean Build started at ${timestamp}${RESET}"
echo "Log file: ${logfile}"
echo "PFPT Clean Build - ${timestamp}" >"$logfile"
echo "---------------------------------------" >>"$logfile"

# Ensure dotnet exists
if ! command -v dotnet >/dev/null 2>&1; then
  echo -e "${RED}✖ dotnet SDK not found on PATH.${RESET}"
  echo "Ensure .NET SDK 8+ is installed." | tee -a "$logfile"
  exit 127
fi

# ---------- Helpers ----------
STEP_OK=0
STEP_FAIL=0

run_step () {
  local title="$1"
  shift
  local cmd=( "$@" )

  echo -e "\n${YELLOW}${BOLD}▶ ${title}${RESET}" | tee -a "$logfile"
  {
    echo "----- ${title} -----"
    echo "\$ ${cmd[*]}"
  } >>"$logfile"

  # Run and append output
  "${cmd[@]}" >>"$logfile" 2>&1
  local ec=$?

  if [ $ec -eq 0 ]; then
    echo -e "${GREEN}✔ ${title}${RESET}"
    STEP_OK=$((STEP_OK+1))
  else
    echo -e "${RED}✖ ${title} (exit $ec)${RESET}"
    STEP_FAIL=$((STEP_FAIL+1))
  fi
  return $ec
}

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

# ---------- Clean bin/obj ----------
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
