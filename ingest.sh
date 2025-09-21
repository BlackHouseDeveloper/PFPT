#!/usr/bin/env bash
set -euo pipefail

# =========================
# Config (override via env)
# =========================
OUT_DIR="${OUT_DIR:-reference}"
MANIFEST="$OUT_DIR/manifest.json"
TEMP_DIR="$(mktemp -d)"
MAX_FILE_MB="${MAX_FILE_MB:-5}"         # skip files larger than this (MB)
INDEX_CMD="${INDEX_CMD:-}"              # e.g., 'dotnet run --project src/RepoMemory.Indexer -- reference reference/memory.db'
SKIP_NPM="${SKIP_NPM:-0}"               # set 1 to skip npm parse

# ---------- utils ----------
cleanup() { rm -rf "$TEMP_DIR"; }
trap cleanup EXIT

log() { printf "✅ %s\n" "$*"; }
warn() { printf "⚠️  %s\n" "$*" >&2; }
die() { printf "❌ %s\n" "$*" >&2; exit 1; }

have() { command -v "$1" >/dev/null 2>&1; }

# portable realpath
abspath() {
  if have realpath; then realpath "$1"; else
    # fallback: use python or readlink -f where available
    python3 - <<EOF 2>/dev/null || readlink -f "$1"
import os,sys
print(os.path.abspath(sys.argv[1]))
EOF
  fi
}

need_tools=(git find rsync awk sed grep)
for t in "${need_tools[@]}"; do have "$t" || die "Missing required tool: $t"
done

if ! have jq; then
  warn "jq not found—install jq for robust JSON. Attempting minimal JSON via printf."
  JQ_PRESENT=0
else
  JQ_PRESENT=1
fi

log "Starting ingest…"

# 1) Prepare output
rm -rf "$OUT_DIR"
mkdir -p "$OUT_DIR"

# 2) Repo provenance
repo_url="$(git config --get remote.origin.url || true)"
commit_sha="$(git rev-parse --verify HEAD 2>/dev/null || echo unknown)"
branch_name="$(git rev-parse --abbrev-ref HEAD 2>/dev/null || echo unknown)"
ingest_date="$(date -u +%Y-%m-%dT%H:%M:%SZ)"

# 3) Discover solution + projects
solution_file="$(find . -maxdepth 2 -type f -name '*.sln' | head -n1 || true)"
if [[ -n "$solution_file" ]]; then
  log "Solution: $solution_file"
  proj_lines="$(dotnet sln "$solution_file" list 2>/dev/null | grep -E '\.csproj$' || true)"
else
  warn "No solution found—scanning for .csproj"
  proj_lines="$(find . -type f -name '*.csproj' | sed 's/^/Project "/;s/$/" (Unknown)/')"
fi

declare -a projects
while IFS= read -r line; do
  # dotnet sln list prints: Project "path/to.csproj" ...
  p="$(sed -E 's/Project "([^"]+)".*/\1/' <<<"$line")"
  [[ -n "$p" ]] && projects+=("$(abspath "$p")")
done <<< "$proj_lines"

((${#projects[@]})) || die "No .NET projects found."

log "Found ${#projects[@]} project(s)."

# 4) Build include list (files-of-interest)
#    We collect a file list, then rsync -aR (relative) to preserve structure.
files_txt="$TEMP_DIR/files.txt"

# folders
include_dirs=(
  ".github/workflows"
  ".configurations"
  ".devcontainer"
  ".vscode"
  "eng"
  "scripts"
)

# file patterns (use find -regex blocks for portability)
# NOTE: we still filter out heavyweight files and junk below
include_globs=(
  "*.sln" "*.slnf" "*.csproj" "*.props" "*.targets" "Directory.Packages.props" "nuget.config"
  "*.cs" "*.razor" "*.cshtml" "*.xaml" "*.xml"
  "*.json" "*.yaml" "*.yml" "*.http" "*.rest"
  "Dockerfile*" "docker-compose*.yml"
  "*.ps1" "*.sh" "*.sql"
  "*.md" "LICENSE" ".editorconfig" ".gitattributes" ".gitignore" "global.json"
  "package.json" "package-lock.json" "pnpm-lock.yaml" "yarn.lock" "playwright.config.*" "tsconfig*.json"
)

# exclude dirs / files
exclude_dirs_regex='/(bin|obj|node_modules|.git)(/|$)'

# Collect directories first
for d in "${include_dirs[@]}"; do
  if [[ -d "$d" ]]; then
    find "$d" -type f | grep -Ev "$exclude_dirs_regex" >> "$files_txt" || true
  fi
done

# Collect patterns from repo root
for pat in "${include_globs[@]}"; do
  # macOS find: use -name for single glob
  find . -type f -name "$pat" | grep -Ev "$exclude_dirs_regex" >> "$files_txt" || true
done

# Filter size (skip > MAX_FILE_MB)
awk -v max="$MAX_FILE_MB" '
  function sizeMB(p){ cmd="wc -c < \"" p "\""; cmd | getline n; close(cmd); return (n/1024/1024); }
  { if ($0 ~ /./) { if (sizeMB($0) <= max) print $0; } }
' "$files_txt" | sort -u > "$TEMP_DIR/files.filtered"

# If nothing selected, bail politely
if [[ ! -s "$TEMP_DIR/files.filtered" ]]; then
  warn "No files matched include set under size limit (${MAX_FILE_MB}MB)."
fi

# Rsync using relative paths (-R) to OUT_DIR
# Also copy global.json if present (helps SDK detection)
rsync -aR --files-from="$TEMP_DIR/files.filtered" / "$OUT_DIR"/ 2>/dev/null || true

log "Files collected into $OUT_DIR/"

# 5) SDK detection
sdk_version="unknown"
if [[ -f "global.json" ]]; then
  if (( JQ_PRESENT )); then
    sdk_version="$(jq -r '.sdk.version // empty' global.json || true)"
    sdk_version="${sdk_version:-unknown}"
  else
    sdk_version="$(grep -Eo '"version"\s*:\s*"[^"]+"' global.json | sed -E 's/.*"([^"]+)".*/\1/' || true)"
    sdk_version="${sdk_version:-unknown}"
  fi
else
  sdk_version="$(dotnet --version 2>/dev/null || echo unknown)"
fi
log "SDK: $sdk_version"

# 6) Project classification + graph
project_json_array="[]"
proj_refs_json="[]"

project_type_of() {
  local csproj="$1"
  local sdk_line; sdk_line="$(grep -m1 '<Project Sdk' "$csproj" || true)"
  local tfm; tfm="$(grep -m1 '<TargetFramework' "$csproj" | sed -E 's/.*<TargetFramework>([^<]+)<.*/\1/' || true)"

  local is_maui=0; local is_web=0; local is_test=0; local is_aspire=0; local is_wasm=0; local is_server=0
  [[ "$sdk_line" =~ Microsoft\.NET\.Sdk\.Maui ]] && is_maui=1
  grep -q '<UseMaui>true</UseMaui>' "$csproj" && is_maui=1
  [[ "$tfm" =~ net[0-9\.]+-android|net[0-9\.]+-ios|net[0-9\.]+-maccatalyst ]] && is_maui=1

  [[ "$sdk_line" =~ Microsoft\.NET\.Sdk\.Web ]] && is_web=1
  grep -q '<IsTestProject>true</IsTestProject>' "$csproj" && is_test=1
  grep -q -E '<PackageReference Include="(xunit|nunit|MSTest\.TestAdapter)"' "$csproj" && is_test=1

  [[ "$sdk_line" =~ Aspire\.AppHost\.Sdk ]] && is_aspire=1

  # Blazor WASM hints
  grep -q -E '<(WasmBuildNative|RunAOTCompilation)>' "$csproj" && is_wasm=1
  grep -q -E '<UseWebAssembly>true</UseWebAssembly>' "$csproj" && is_wasm=1

  # Blazor Server hint
  [[ $is_web -eq 1 && $is_wasm -eq 0 ]] && is_server=1

  local kind="Library"
  (( is_test ))   && kind="Test"
  (( is_maui ))   && kind="MAUI (UI)"
  (( is_web ))    && kind="Web/API"
  (( is_server )) && kind="Blazor Server"
  (( is_wasm ))   && kind="Blazor WASM"
  (( is_aspire )) && kind="Aspire AppHost"

  printf "%s|%s\n" "$kind" "${tfm:-unknown}"
}

# Build project metadata + refs
for p in "${projects[@]}"; do
  kind_tfm="$(project_type_of "$p")"
  kind="${kind_tfm%%|*}"
  tfm="${kind_tfm#*|}"

  rel="$(python3 - "$p" <<'EOF'
import os,sys
print(os.path.relpath(sys.argv[1], "."))
EOF
)"
  # project refs (ProjectReference)
  refs=$(grep -E '<ProjectReference Include=' "$p" | sed -E 's/.*Include="([^"]+)".*/\1/' || true)
  if (( JQ_PRESENT )); then
    pj="$(jq -n --arg path "$rel" --arg type "$kind" --arg framework "$tfm" '{path:$path,type:$type,framework:$framework}')"
    project_json_array="$(jq --argjson item "$pj" '. + [ $item ]' <<<"$project_json_array")"
  else
    project_json_array="$(printf '%s\n{"path":"%s","type":"%s","framework":"%s"}' "$project_json_array" "$rel" "$kind" "$tfm")"
  fi

  while IFS= read -r r; do
    [[ -z "$r" ]] && continue
    # normalize reference path relative to repo root
    rrel="$(python3 - <<EOF
import os,sys
base = sys.argv[1]
ref  = sys.argv[2]
import os.path as p
joined = p.normpath(p.join(p.dirname(base), ref))
print(p.relpath(joined, "."))
EOF
)"
    if (( JQ_PRESENT )); then
      edge="$(jq -n --arg from "$rel" --arg to "$rrel" '{from:$from,to:$to}')"
      proj_refs_json="$(jq --argjson e "$edge" '. + [ $e ]' <<<"$proj_refs_json")"
    else
      proj_refs_json="$(printf '%s\n{"from":"%s","to":"%s"}' "$proj_refs_json" "$rel" "$rrel")"
    fi
  done <<<"$refs"
done

# 7) Repo-level classification
repo_type="Monolith"
((${#projects[@]} > 1)) && repo_type="Multiple projects"
orchestration="None"
if grep -qrE 'DistributedApplication\.CreateBuilder|builder\.AddServiceDefaults' . || grep -qr 'Aspire.AppHost.Sdk' .; then
  orchestration="Aspire/AppHost"
elif [[ -f "docker-compose.yml" ]]; then
  orchestration="Docker Compose"
elif find . -maxdepth 2 -type f -name 'Dockerfile*' | grep -q .; then
  orchestration="Containerized (Docker)"
fi

# 8) Dependencies
log "Gathering dependencies…"
nuget_json="[]"
for p in "${projects[@]}"; do
  out="$(dotnet list "$p" package --include-transitive 2>/dev/null || true)"
  # Parse simple " > Package Version" lines
  while read -r pkg ver; do
    [[ -z "$pkg" ]] && continue
    if (( JQ_PRESENT )); then
      nuget_json="$(jq --arg pkg "$pkg" --arg ver "$ver" '. + [{"package":$pkg,"version":$ver}]' <<<"$nuget_json")"
    else
      nuget_json="$(printf '%s\n{"package":"%s","version":"%s"}' "$nuget_json" "$pkg" "$ver")"
    fi
  done < <(printf "%s\n" "$out" | grep -E '^\s*>' | sed -E 's/^\s*>\s*([^\s]+)\s+([^\s]+).*/\1 \2/')
done

npm_json="[]"
if (( SKIP_NPM == 0 )); then
  if [[ -f "package-lock.json" && $JQ_PRESENT -eq 1 ]]; then
    npm_json="$(jq -r '
      .packages
      | to_entries
      | map(select(.key != ""))        # skip root
      | map({ package: (.key|split("node_modules/")[-1]), version: .value.version })
      | unique_by(.package)
    ' package-lock.json 2>/dev/null || echo '[]')"
  elif [[ -f "package.json" ]]; then
    # fallback: best-effort using npm list
    have npm && npm_json="$(
      npm list --depth=0 2>/dev/null | awk -F@ '/├──|└──/ {print $2" "$3}' \
      | awk '{print "{ \"package\": \""$1"\", \"version\": \""$2"\" }"}' \
      | jq -s '.'
    )"
  fi
fi

# 9) File‑type counts
file_counts_json="[]"
while IFS= read -r f; do
  ext="${f##*.}"; [[ "$f" == *"/."* ]] && continue
  echo "$ext"
done < <(find "$OUT_DIR" -type f | sed "s|$OUT_DIR/||") \
| awk 'NF' | sort | uniq -c | awk '{printf("{\"ext\":\"%s\",\"count\":%d}\n",$2,$1)}' \
| { if (( JQ_PRESENT )); then jq -s '.'; else cat; fi; } \
| { read -r file_counts_json || true; echo "$file_counts_json"; } >/dev/null

# Actually capture into a variable
file_counts_json="$(
  find "$OUT_DIR" -type f | sed "s|$OUT_DIR/||" \
  | awk -F. 'NF>1 {print $NF}' \
  | sort | uniq -c \
  | awk '{printf("{\"ext\":\"%s\",\"count\":%d}\n",$2,$1)}' \
  | { if (( JQ_PRESENT )); then jq -s '.'; else cat; fi; }
)"

# 10) Write manifest
log "Writing manifest → $MANIFEST"
if (( JQ_PRESENT )); then
  jq -n \
    --arg repo_url "$repo_url" \
    --arg commit "$commit_sha" \
    --arg branch "$branch_name" \
    --arg sdk "$sdk_version" \
    --arg solution "${solution_file:-}" \
    --arg repo_type "$repo_type" \
    --arg orchestration "$orchestration" \
    --arg ingest_date "$ingest_date" \
    --argjson projects "$project_json_array" \
    --argjson proj_refs "$proj_refs_json" \
    --argjson nuget "$nuget_json" \
    --argjson npm "$npm_json" \
    --argjson file_counts "$file_counts_json" \
    --arg include_globs "$(printf '%s\n' "${include_globs[@]}" | jq -R . | jq -s .)" \
    --arg include_dirs "$(printf '%s\n' "${include_dirs[@]}" | jq -R . | jq -s .)" \
    --arg excludes "$exclude_dirs_regex" '
    {
      provenance: { repo_url: $repo_url, commit: $commit, branch: $branch, ingest_date: $ingest_date },
      sdk_version: $sdk,
      solution: $solution,
      repo_type: $repo_type,
      orchestration: $orchestration,
      projects: $projects,
      project_references: $proj_refs,
      nuget_dependencies: $nuget,
      npm_dependencies: $npm,
      filetype_counts: $file_counts,
      policy: { include_dirs: ($include_dirs|fromjson), include_globs: ($include_globs|fromjson), exclude_dirs_regex: $excludes, max_file_mb: '"$MAX_FILE_MB"' }
    }' > "$MANIFEST"
else
  # Minimal manifest fallback
  cat > "$MANIFEST" <<EOF
{
  "provenance": { "repo_url": "$repo_url", "commit": "$commit_sha", "branch": "$branch_name", "ingest_date": "$ingest_date" },
  "sdk_version": "$sdk_version",
  "solution": "${solution_file:-}",
  "repo_type": "$repo_type",
  "orchestration": "$orchestration",
  "projects": $project_json_array,
  "project_references": $proj_refs_json,
  "nuget_dependencies": $nuget_json,
  "npm_dependencies": $npm_json,
  "filetype_counts": $file_counts_json,
  "policy": { "include_dirs": [], "include_globs": [], "exclude_dirs_regex": "$exclude_dirs_regex", "max_file_mb": "$MAX_FILE_MB" }
}
EOF
fi

log "Manifest complete."

# 11) Optional auto-index
if [[ -n "$INDEX_CMD" ]]; then
  log "Indexing with: $INDEX_CMD"
  # Example expected: INDEX_CMD='dotnet run --project src/RepoMemory.Indexer -- reference reference/memory.db'
  # or: INDEX_CMD='python3 tools/index.py --input reference --out reference/memory.db'
  eval "$INDEX_CMD" || warn "Indexing failed."
else
  warn "INDEX_CMD not set—skipping auto-index."
fi

log "Ingest finished successfully."
