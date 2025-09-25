#!/usr/bin/env bash
# scripts/run-workflows.sh
# Run GitHub Actions workflows locally with act, with PFPT-specific helpers.

set -euo pipefail

WORKFLOWS_DIR="${WORKFLOWS_DIR:-.github/workflows}"
ACT_CMD="${ACT_CMD:-act}"
YQ_CMD="${YQ_CMD:-yq}"
CONTAINER_RUNTIME="${CONTAINER_RUNTIME:-docker}"
DEFAULT_EVENT="${DEFAULT_EVENT:-push}"
ARCH_FLAG="${ARCH_FLAG:-}"
ACT_IMAGE_MAP="${ACT_IMAGE_MAP:-}"
DRY_RUN=0
CONTINUE_ON_ERROR=0
MODE="help"

EVENT_OVERRIDE=""
REF_OVERRIDE="${REF_OVERRIDE:-refs/heads/main}"
ACT_EVENT_ACTOR="${ACT_EVENT_ACTOR:-pfpt-local}"
INPUT_OVERRIDES=()
JOB_FILTERS=()
MATRIX_FILTERS=()
EXTRA_ACT_ARGS=()
TEMP_EVENT_FILES=()
DISPATCH_INPUTS=()

REPO_ROOT="$(git rev-parse --show-toplevel 2>/dev/null || pwd)"
REMOTE_URL="$(git -C "$REPO_ROOT" config --get remote.origin.url 2>/dev/null || true)"
REPO_SLUG="${REPO_SLUG:-}"
if [[ -z "$REPO_SLUG" && -n "$REMOTE_URL" ]]; then
  REPO_SLUG="$(printf '%s\n' "$REMOTE_URL" | sed -E 's#(git@|ssh://git@|https://|http://)([^/:]+)[:/]([^/]+/[^/]+)(\.git)?#\3#' | head -n1)"
fi
if [[ -z "$REPO_SLUG" ]]; then
  REPO_SLUG="$(basename "$REPO_ROOT")"
fi
if [[ "$REPO_SLUG" == */* ]]; then
  REPO_OWNER="${REPO_SLUG%%/*}"
  REPO_NAME="${REPO_SLUG##*/}"
else
  REPO_OWNER="local"
  REPO_NAME="$REPO_SLUG"
fi
REPO_OWNER="${ACT_REPO_OWNER:-$REPO_OWNER}"
REPO_NAME="${ACT_REPO_NAME:-$REPO_NAME}"

ACT_LOCAL_DIR="${ACT_LOCAL_DIR:-$REPO_ROOT/.act}"
ACT_ACTION_CACHE_PATH="${ACT_ACTION_CACHE_PATH:-$ACT_LOCAL_DIR/action-cache}"
ACT_ARTIFACT_PATH="${ACT_ARTIFACT_PATH:-$ACT_LOCAL_DIR/artifacts}"
ACT_CACHE_SERVER_PATH="${ACT_CACHE_SERVER_PATH:-$ACT_LOCAL_DIR/cache-server}"
ACT_DEFAULT_IMAGE_MAP="${ACT_DEFAULT_IMAGE_MAP:-ubuntu-latest=ghcr.io/catthehacker/ubuntu:act-latest}"

ensure_dir() {
  local dir="$1"
  [[ -z "$dir" ]] && return 0
  [[ -d "$dir" ]] || mkdir -p "$dir"
}

cleanup() {
  local f
  for f in "${TEMP_EVENT_FILES[@]-}"; do
    [[ -f "$f" ]] && rm -f "$f"
  done
}
trap cleanup EXIT

msg() { printf "%b\n" "$*"; }
die() { msg "❌ $*"; exit 1; }
have() { command -v "$1" >/dev/null 2>&1; }

check_dependencies() {
  have git || die "'git' is required."
  have "$ACT_CMD" || die "'$ACT_CMD' is required. Install: https://github.com/nektos/act"
  have "$YQ_CMD"  || die "'$YQ_CMD' is required. Install: https://github.com/mikefarah/yq"
  have "$CONTAINER_RUNTIME" || die "'$CONTAINER_RUNTIME' not found. Install Docker or Podman."
  $YQ_CMD --version 2>/dev/null | grep -Eq "version v?4" || die "'$YQ_CMD' v4 required."
  have python3 || die "'python3' is required for workflow_dispatch payload generation."
}

if [[ -z "$ARCH_FLAG" ]] && [[ "$(uname -s)" == "Darwin" ]] && [[ "$(uname -m)" == "arm64" ]]; then
  ARCH_FLAG="--container-architecture linux/amd64"
fi

list_workflows() {
  find "$WORKFLOWS_DIR" -type f \( -name "*.yml" -o -name "*.yaml" \) | LC_ALL=C sort
}

wf_name() {
  local file="$1"
  local name
  name="$($YQ_CMD -r '.name // ""' "$file" 2>/dev/null | xargs || true)"
  if [[ -z "$name" || "$name" == "null" ]]; then
    basename "$file"
  else
    printf "%s" "$name"
  fi
}

workflow_uses_macos() {
  local file="$1"
  local runs
  runs="$($YQ_CMD -r '
    .jobs[]? | .["runs-on"]? // empty
    | ( (.|tag) as $t | if $t == "!!seq" then .[] else . end )
  ' "$file" 2>/dev/null || true)"
  if printf "%s\n" "$runs" | grep -qi 'macos'; then
    return 0
  fi
  runs="$($YQ_CMD -r '
    .jobs[]?.strategy.matrix? | .["runs-on"]? // empty
    | ( (.|tag) as $t | if $t == "!!seq" then .[] else . end )
  ' "$file" 2>/dev/null || true)"
  printf "%s\n" "$runs" | grep -qi 'macos'
}

print_menu() {
  local i=1
  WF_FILES=()
  while IFS= read -r wf; do
    [[ -z "$wf" ]] && continue
    WF_FILES+=("$wf")
  done < <(list_workflows)
  (( ${#WF_FILES[@]-} > 0 )) || die "No workflows found in $WORKFLOWS_DIR."
  msg "--- Available Workflows ---"
  WF_NAMES=()
  for f in "${WF_FILES[@]-}"; do
    local n; n="$(wf_name "$f")"
    WF_NAMES+=("$n")
    local event; event="$(resolve_event "$f")"
    local indicator=""
    if workflow_uses_macos "$f"; then
      indicator=" ⚠️macOS"
    fi
    printf "%2d. %s  [%s] {%s%s}
" "$i" "$n" "$(basename "$f")" "$event" "$indicator"
    ((i++))
  done
  msg "---------------------------"
}


select_index() {
  local prompt="$1" choice
  read -r -p "$prompt" choice
  [[ "$choice" =~ ^[0-9]+$ ]] || die "Invalid selection. Enter a number."
  local index=$((choice-1))
  (( index >= 0 && index < ${#WF_FILES[@]} )) || die "Selection out of range."
  echo "$index"
}

contains_line() {
  local needle="$1"; shift
  printf "%s\n" "$@" | grep -Fx -- "$needle" >/dev/null 2>&1
}

suggest_events() {
  local file="$1"
  $YQ_CMD -r '
    .on
    | ( (.|tag) as $t
        | if $t == "!!map" then keys[]
          elif $t == "!!seq" then .[]
          elif $t == "!!str" then .
          else empty end )
  ' "$file" 2>/dev/null | sort -u
}

resolve_event() {
  local file="$1"
  local events=()
  while IFS= read -r evt; do
    [[ -z "$evt" ]] && continue
    events+=("$evt")
  done < <(suggest_events "$file" 2>/dev/null || true)
  if [[ -n "$EVENT_OVERRIDE" ]]; then
    if contains_line "$EVENT_OVERRIDE" "${events[@]-}"; then
      printf "%s" "$EVENT_OVERRIDE"
    elif (( ${#events[@]-} > 0 )); then
      printf "%s" "${events[0]}"
    else
      printf "%s" "$DEFAULT_EVENT"
    fi
    return 0
  fi
  if contains_line "workflow_dispatch" "${events[@]-}"; then
    printf "workflow_dispatch"
    return 0
  fi
  if contains_line "pull_request" "${events[@]-}"; then
    printf "pull_request"
    return 0
  fi
  if contains_line "$DEFAULT_EVENT" "${events[@]-}"; then
    printf "%s" "$DEFAULT_EVENT"
    return 0
  fi
  if (( ${#events[@]-} > 0 )); then
    printf "%s" "${events[0]}"
    return 0
  fi
  printf "%s" "$DEFAULT_EVENT"
}

prepare_dispatch_inputs() {
  local file="$1"
  DISPATCH_INPUTS=()
  local line
  while IFS='|' read -r key required default _; do
    [[ -z "$key" ]] && continue
    local value=""
    for pair in "${INPUT_OVERRIDES[@]-}"; do
      if [[ "$pair" == "$key="* ]]; then
        value="${pair#*=}"
        break
      fi
    done
    if [[ -z "$value" && -n "$default" && "$default" != "null" ]]; then
      value="$default"
    fi
    if [[ -z "$value" && "$required" == "true" ]]; then
      if [[ -t 0 ]]; then
        read -r -p "Input '$key' is required. Enter value: " value
      else
        die "Required input '$key' missing. Supply via --input $key=value."
      fi
      [[ -z "$value" ]] && die "Required input '$key' missing."
    fi
    if [[ -n "$value" ]]; then
      DISPATCH_INPUTS+=("$key=$value")
    fi
  done < <(
    $YQ_CMD -r '
      (.on.workflow_dispatch.inputs // {})
      | to_entries[]
      | "\(.key)|\(.value.required // false)|\(.value.default // "")|"
    ' "$file" 2>/dev/null || true
  )

  local pair
  for pair in "${INPUT_OVERRIDES[@]-}"; do
    local key="${pair%%=*}"
    local found=0
    for existing in "${DISPATCH_INPUTS[@]-}"; do
      if [[ "${existing%%=*}" == "$key" ]]; then
        found=1
        break
      fi
    done
    if (( ! found )); then
      DISPATCH_INPUTS+=("$pair")
    fi
  done
}

make_event_file() {
  local file="$1"
  local event="$2"
  local tmp
  tmp="$(mktemp)"
  TEMP_EVENT_FILES+=("$tmp")

  local inputs_payload
  if (( ${#DISPATCH_INPUTS[@]} )); then
    inputs_payload="$(printf '%s\n' "${DISPATCH_INPUTS[@]-}")"
  else
    inputs_payload=""
  fi

  REF_TO_WRITE="$REF_OVERRIDE" \
  ACTOR_TO_WRITE="$ACT_EVENT_ACTOR" \
  REPO_OWNER_TO_WRITE="$REPO_OWNER" \
  REPO_NAME_TO_WRITE="$REPO_NAME" \
  DISPATCH_INPUT_PAIRS="$inputs_payload" \
  python3 - "$tmp" <<'PY'
import json
import os
import sys

def build_inputs(raw: str):
    data = {}
    for line in raw.splitlines():
        if not line.strip():
            continue
        if '=' in line:
            key, value = line.split('=', 1)
            data[key] = value
    return data

def main(out_path: str):
    payload = {
        "ref": os.environ.get("REF_TO_WRITE", "refs/heads/main"),
        "repository": {
            "owner": {"login": os.environ.get("REPO_OWNER_TO_WRITE", "local")},
            "name": os.environ.get("REPO_NAME_TO_WRITE", "local"),
            "full_name": f"{os.environ.get('REPO_OWNER_TO_WRITE', 'local')}/" + os.environ.get("REPO_NAME_TO_WRITE", "local"),
        },
        "sender": {"login": os.environ.get("ACTOR_TO_WRITE", "local")},
    }
    inputs = build_inputs(os.environ.get("DISPATCH_INPUT_PAIRS", ""))
    if inputs:
        payload["inputs"] = inputs
    with open(out_path, "w", encoding="utf-8") as fout:
        json.dump(payload, fout)

if __name__ == "__main__":
    main(sys.argv[1])
PY

  echo "$tmp"
}

build_chain() {
  local start_file="$1"
  ALL_FILES=()
  while IFS= read -r wf; do
    [[ -z "$wf" ]] && continue
    ALL_FILES+=("$wf")
  done < <(list_workflows)

  local visited="" chain=""

  _dfs() {
    local file="$1"
    if contains_line "$file" $visited; then return; fi
    visited="$visited"$'\n'"$file"
    chain="$chain"$'\n'"$file"
    local file_name; file_name="$(wf_name "$file")"

    local dep_file
    for dep_file in "${ALL_FILES[@]-}"; do
      local triggers
      triggers="$($YQ_CMD -r'
        .on.workflow_run.workflows
        | ( (.|tag) as $t
            | if $t == "!!seq" then .[]
              elif $t == "!!str" then .
              else empty end )
      ' "$dep_file" 2>/dev/null || true)"
      [[ -z "$triggers" ]] && continue
      if printf "%s\n" "$triggers" | grep -Fx -- "$file_name" >/dev/null 2>&1; then
        _dfs "$dep_file"
      fi
    done
  }
  _dfs "$start_file"
  printf "%s\n" "$chain" | sed '/^$/d'
}

run_workflow() {
  local wf_file="$1"
  local wf_name_disp; wf_name_disp="$(wf_name "$wf_file")"
  local event; event="$(resolve_event "$wf_file")"
  msg "🚀 Running: $wf_name_disp ($(basename "$wf_file"))"
  msg "   • Event: $event"
  if [[ -n "$EVENT_OVERRIDE" && "$event" != "$EVENT_OVERRIDE" ]]; then
    msg "   • Note: event '$EVENT_OVERRIDE' not declared in this workflow; using '$event' instead"
  fi

  local extra_args=()
  if [[ -n "$ARCH_FLAG" ]]; then
    # shellcheck disable=SC2206
    extra_args+=( $ARCH_FLAG )
  fi

  if [[ -n "$ACT_IMAGE_MAP" ]]; then
    local IFS=','
    read -r -a map_entries <<< "$ACT_IMAGE_MAP"
    local m
    for m in "${map_entries[@]-}"; do
      [[ -n "$m" ]] && extra_args+=(-P "$m")
    done
  else
    [[ -n "$ACT_DEFAULT_IMAGE_MAP" ]] && extra_args+=(-P "$ACT_DEFAULT_IMAGE_MAP")
  fi

  ensure_dir "$ACT_ACTION_CACHE_PATH"
  ensure_dir "$ACT_ARTIFACT_PATH"
  ensure_dir "$ACT_CACHE_SERVER_PATH"

  extra_args+=(
    --action-cache-path "$ACT_ACTION_CACHE_PATH"
    --artifact-server-path "$ACT_ARTIFACT_PATH"
    --cache-server-path "$ACT_CACHE_SERVER_PATH"
  )

  local event_file=""
  if [[ "$event" == "workflow_dispatch" ]]; then
    prepare_dispatch_inputs "$wf_file"
    if (( ${#DISPATCH_INPUTS[@]} )); then
      msg "   • Inputs:"
      local pair
      for pair in "${DISPATCH_INPUTS[@]-}"; do
        msg "     - $pair"
      done
    fi
    msg "   • Ref: $REF_OVERRIDE"
    msg "   • Actor: $ACT_EVENT_ACTOR"
    event_file="$(make_event_file "$wf_file" "$event")"
  fi

  local cmd=("$ACT_CMD" "$event" -W "$wf_file" "${extra_args[@]}")

  if (( ${#JOB_FILTERS[@]-} )); then
    local job
    for job in "${JOB_FILTERS[@]-}"; do
      cmd+=(--job "$job")
    done
  fi

  if (( ${#MATRIX_FILTERS[@]-} )); then
    local matrix
    for matrix in "${MATRIX_FILTERS[@]-}"; do
      cmd+=(--matrix "$matrix")
    done
  fi

  if [[ -n "$event_file" ]]; then
    cmd+=(-e "$event_file")
  fi

  if (( ${#EXTRA_ACT_ARGS[@]} )); then
    cmd+=("${EXTRA_ACT_ARGS[@]-}")
  fi

  msg "   • Command: ${cmd[*]}"

  if (( DRY_RUN )); then
    msg "🧪 DRY-RUN (command not executed)"
    return 0
  fi

  if ! "${cmd[@]}"; then
    msg "❌ Failed: $wf_name_disp"
    if (( CONTINUE_ON_ERROR )); then
      return 0
    else
      exit 1
    fi
  fi

  msg "✅ Completed: $wf_name_disp"
  echo
}

run_all() {
  local files=()
  while IFS= read -r wf; do
    [[ -z "$wf" ]] && continue
    files+=("$wf")
  done < <(list_workflows)
  (( ${#files[@]-} > 0 )) || die "No workflows found."
  local f
  for f in "${files[@]-}"; do run_workflow "$f"; done
}


run_menu_single() {
  print_menu
  local idx; idx="$(select_index '➡️ Select a workflow number to run: ')"
  run_workflow "${WF_FILES[$idx]}"
}

run_chain_menu() {
  print_menu
  local idx; idx="$(select_index '➡️ Select the STARTING workflow for the chain: ')"
  local start="${WF_FILES[$idx]}"
  local chain; chain="$(build_chain "$start")"

  msg "--- Execution Plan ---"
  local first=1
  while IFS= read -r w; do
    [[ -z "$w" ]] && continue
    if (( first )); then
      printf "%s" "$(wf_name "$w")"
      first=0
    else
      printf " -> %s" "$(wf_name "$w")"
    fi
  done <<< "$chain"
  printf "\n\n"

  local w
  while IFS= read -r w; do
    [[ -n "$w" ]] && run_workflow "$w"
  done <<< "$chain"
}

usage() {
  cat <<EOF
Usage: $(basename "$0") [options]
Options:
  --menu                 Select and run a workflow interactively
  --all                  Run all workflows sequentially
  --chain                Resolve & run workflow_run dependency chain
  --list                 List workflows with names/events
  --suggest-events       Show detected trigger events for a workflow
  --event <name>         Force event (overrides autodetect)
  --amd64                Force linux/amd64 containers (Apple Silicon fix)
  --dry-run              Show command without running
  --continue-on-error    Continue when a workflow fails
  --image-map <maps>     Comma-separated act image map (foo=bar,baz=qux)
  --input key=value      workflow_dispatch input (repeatable)
  --job <name>           Limit to a specific job (repeatable)
  --matrix key=value     Matrix selector for act (repeatable)
  --ref <ref>            Override ref for workflow_dispatch payload
  --actor <login>        Override actor for workflow_dispatch payload
  --act-arg <arg>        Extra argument passed to act (repeatable)
  --help                 Show this help

Env overrides:
  WORKFLOWS_DIR, ACT_CMD, YQ_CMD, CONTAINER_RUNTIME, DEFAULT_EVENT,
  ARCH_FLAG, ACT_IMAGE_MAP, ACT_DEFAULT_IMAGE_MAP,
  ACT_LOCAL_DIR, ACT_ACTION_CACHE_PATH, ACT_ARTIFACT_PATH,
  ACT_CACHE_SERVER_PATH, REF_OVERRIDE, ACT_EVENT_ACTOR,
  ACT_REPO_OWNER, ACT_REPO_NAME
EOF
}

check_dependencies

while (( "$#" )); do
  case "$1" in
    --menu) MODE="menu"; shift ;;
    --all) MODE="all"; shift ;;
    --chain) MODE="chain"; shift ;;
    --list) MODE="list"; shift ;;
    --suggest-events) MODE="suggest-events"; shift ;;
    --dry-run) DRY_RUN=1; shift ;;
    --continue-on-error) CONTINUE_ON_ERROR=1; shift ;;
    --amd64) ARCH_FLAG="--container-architecture linux/amd64"; shift ;;
    --event)
      EVENT_OVERRIDE="${2:-}"
      [[ -n "$EVENT_OVERRIDE" ]] || die "--event requires a value"
      shift 2 ;;
    --image-map)
      ACT_IMAGE_MAP="${2:-}"
      [[ -n "$ACT_IMAGE_MAP" ]] || die "--image-map requires a value"
      shift 2 ;;
    --input)
      local_pair="${2:-}"
      [[ "$local_pair" == *=* ]] || die "--input expects key=value"
      INPUT_OVERRIDES+=("$local_pair")
      shift 2 ;;
    --job)
      local_job="${2:-}"
      [[ -n "$local_job" ]] || die "--job requires a value"
      JOB_FILTERS+=("$local_job")
      shift 2 ;;
    --matrix)
      local_matrix="${2:-}"
      [[ -n "$local_matrix" ]] || die "--matrix requires a value"
      MATRIX_FILTERS+=("$local_matrix")
      shift 2 ;;
    --ref)
      REF_OVERRIDE="${2:-}"
      [[ -n "$REF_OVERRIDE" ]] || die "--ref requires a value"
      shift 2 ;;
    --actor)
      ACT_EVENT_ACTOR="${2:-}"
      [[ -n "$ACT_EVENT_ACTOR" ]] || die "--actor requires a value"
      shift 2 ;;
    --act-arg)
      local_act_arg="${2:-}"
      [[ -n "$local_act_arg" ]] || die "--act-arg requires a value"
      EXTRA_ACT_ARGS+=("$local_act_arg")
      shift 2 ;;
    --help|-h)
      usage
      exit 0 ;;
    *)
      usage
      die "Unknown option: $1" ;;
  esac
done

case "$MODE" in
  menu) run_menu_single ;;
  all) run_all ;;
  chain) run_chain_menu ;;
  list) print_menu ;;
  suggest-events)
    print_menu
    idx="$(select_index '➡️ Select a workflow to inspect events: ')"
    file="${WF_FILES[$idx]}"
    msg "Events in $(wf_name "$file") [$(basename "$file")]:"
    suggest_events "$file" || true
    ;;
  help|*)
    usage
    exit $([[ "$MODE" == help ]] && echo 0 || echo 1) ;;
esac
