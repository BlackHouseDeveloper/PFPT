# PFPT Script Usage Reference

Use this quick reference to recall the PFPT helper scripts, what they do, and how to run them. Paths are relative to the repository root unless noted otherwise.

| Script | Location | Primary purpose | Typical scenarios |
| --- | --- | --- | --- |
| `PFPT-Foundry.sh` | `./PFPT-Foundry.sh` | Bootstrap or refresh the full PFPT development environment | First-time setup, regenerating migrations, seeding demo data |
| `run-pfpt.sh` | `./run-pfpt.sh` | Launch the Blazor Web or MAUI clients from one prompt | Quickly switching between Web, Android, and iOS builds |
| `cleanbuild-pfpt.sh` | `./cleanbuild-pfpt.sh` | Perform a clean build/test cycle with detailed logging | Validating a branch before commits or releases |
| `ingest.sh` | `./ingest.sh` | Capture repository metadata/assets into `reference/` for indexing | Producing snapshots for MCP/AI memory or documentation |
| `run-workflows.sh` | `.github/scripts/run-workflows.sh` | Drive GitHub Actions locally via `act` with PFPT-specific helpers | Debugging CI, exploring workflow dispatch inputs |

---

## PFPT-Foundry.sh — environment bootstrapper

**What it does:** Sets up or updates the full PFPT solution with Clean Architecture conventions, installs dependencies, and optionally handles database migration/seed operations. The script is idempotent and safe to rerun.

**Prerequisites:** .NET 8 SDK, Git, macOS/Linux/WSL Bash environment. Do **not** run with `sudo`.

### Flags
- `--create-migration` — generate the initial EF Core migration and update the local database.
- `--seed` — populate the development database with sample data.
- `--verbose` / `-v` — turn on verbose output for troubleshooting.
- `--help` / `-h` — print the built-in usage guide.

### Helpful environment variables
- `PFP_DB_PATH` — override the SQLite database location (default `./dev.physicallyfitpt.db`).
- `DOTNET_CLI_HOME`, `NUGET_PACKAGES` — customize cache locations (useful for Homebrew installs on macOS).

### Example runs
```bash
./PFPT-Foundry.sh                        # Standard setup
./PFPT-Foundry.sh --create-migration     # Setup + initial migration
./PFPT-Foundry.sh --create-migration --seed
```

---

## run-pfpt.sh — project launcher

**What it does:** Presents an interactive menu to run either the Blazor WebAssembly site or the MAUI targets (Android, iOS). Handles DOTNET_ROOT detection from `~/.dotnet-sdk` and validates tooling before launch.

**Prerequisites:** .NET 8 SDK; for mobile targets you also need `adb` + Android SDK tools (Android) or Xcode CLI tools (`xcrun`) and configured simulators (iOS).

### Workflow
1. Run the script: `./run-pfpt.sh`.
2. Choose `1` for Blazor WASM, `2` for Android, or `3` for iOS when prompted.
3. The script runs `dotnet run` for Web, or `dotnet build -t:Run` for the MAUI mobile targets.

### Notes
- Android option auto-starts the first available emulator if no device is connected.
- iOS option boots the first available simulator when none are already running.
- Script exits early if the expected project files under `src/` are missing.

### Example run
```bash
./run-pfpt.sh
# select option 1, 2, or 3 when prompted
```

---

## cleanbuild-pfpt.sh — clean build & test pipeline

**What it does:** Performs an aggressive clean (bin/obj removal, `dotnet build-server shutdown`), runs `dotnet clean`, `dotnet restore`, `dotnet build -warnaserror`, and `dotnet test`, while logging all output to a timestamped file. On macOS it opens the log in TextEdit automatically.

**Prerequisites:** .NET 8 SDK, Git (optional but suggested for context in logs), Bash 4+.

### Key behaviors
- Generates `build-output-<timestamp>.txt` in the repo root with full logs.
- Tracks pass/fail counts for each step and prints a summary with ✅/❌ status.
- Skips testing automatically if the build fails.
- Removes Android build artifacts under `PhysicallyFitPT/obj/Debug/net8.0-android/lp` to prevent stale resource errors.

### Example run
```bash
chmod +x cleanbuild-pfpt.sh   # one-time setup if needed
./cleanbuild-pfpt.sh
```

---

## ingest.sh — repository snapshot generator

**What it does:** Collects solution metadata, project graphs, dependencies, and selected source files into the `reference/` directory, emitting a JSON manifest and optionally running a follow-up indexing command.

**Prerequisites:** `git`, `find`, `rsync`, `awk`, `sed`, `grep`, `python3`, `.NET` CLI, and optionally `jq` for richer JSON output. Requires a POSIX shell environment.

### Core workflow
1. Removes and recreates the target output directory (`reference/` by default).
2. Captures repository provenance (remote URL, commit, branch, timestamp).
3. Enumerates solution/projects, classifies project types, and maps project references.
4. Harvests NuGet and (optionally) NPM dependencies.
5. Copies relevant files under size limits and writes `reference/manifest.json`.
6. Runs an optional indexing command when `INDEX_CMD` is supplied.

### Environment switches
- `OUT_DIR` — change the destination directory (default `reference`).
- `MAX_FILE_MB` — skip files larger than this size (default `5`).
- `INDEX_CMD` — command to run after ingest (e.g. `dotnet run --project src/RepoMemory.Indexer -- reference reference/memory.db`).
- `SKIP_NPM` — set to `1` to skip NPM dependency extraction.

### Example runs
```bash
./ingest.sh                                      # default snapshot into reference/
OUT_DIR=tmp/reference-snapshot ./ingest.sh       # write to a custom directory
INDEX_CMD='dotnet run --project tools/Indexer -- reference reference/memory.db' ./ingest.sh
```

---

## run-workflows.sh — local GitHub Actions runner

**What it does:** Wraps [`act`](https://github.com/nektos/act) with PFPT-friendly helpers: interactive workflow selection, automatic trigger detection, job/matrix filtering, and optional chaining for `workflow_run` dependencies. Uses `.github/workflows/.act.secrets` for local secrets.

**Prerequisites:** `git`, [`act`](https://github.com/nektos/act) (v0.2.60+ recommended), [`yq` v4](https://github.com/mikefarah/yq), Docker or Podman, and `python3`. For Apple Silicon, Docker must support amd64 emulation or use the `--amd64` switch.

### Common modes
- `--menu` — interactive workflow picker and runner (default recommendation).
- `--all` — run every workflow sequentially.
- `--chain` — resolve `workflow_run` triggers and run the dependency chain.
- `--list` — print workflows with resolved events.
- `--suggest-events` — inspect triggers detected for a selected workflow.

### Useful flags
- `--event <name>` — force a specific GitHub event (e.g. `workflow_dispatch`).
- `--job <name>` — run only a specific job (repeatable).
- `--matrix key=value` — constrain a matrix build (repeatable).
- `--input key=value` — supply `workflow_dispatch` inputs (repeatable).
- `--act-arg <arg>` — pass extras straight through to `act` (repeatable).
- `--image-map foo=bar,...` — override the default image mapping.
- `--amd64` — force `linux/amd64` containers (helpful on Apple Silicon).
- `--dry-run` — print the resolved `act` command without running.
- `--continue-on-error` — keep going when a workflow fails.
- `--help` — show the full built-in usage text.

### Environment overrides
`WORKFLOWS_DIR`, `ACT_CMD`, `YQ_CMD`, `CONTAINER_RUNTIME`, `DEFAULT_EVENT`, `ARCH_FLAG`, `ACT_IMAGE_MAP`, `ACT_DEFAULT_IMAGE_MAP`, `ACT_LOCAL_DIR`, `ACT_ACTION_CACHE_PATH`, `ACT_ARTIFACT_PATH`, `ACT_CACHE_SERVER_PATH`, `REF_OVERRIDE`, `ACT_EVENT_ACTOR`, `ACT_REPO_OWNER`, `ACT_REPO_NAME`.

### Example runs
```bash
.github/scripts/run-workflows.sh --menu
.github/scripts/run-workflows.sh --event workflow_dispatch --job build --input environment=staging
ACT_CMD=./bin/act .github/scripts/run-workflows.sh --chain --continue-on-error
```

---

**Tips**
- Make scripts executable once (`chmod +x <script>`). Git tracks executable bit, so you usually only need to do this after cloning.
- Pair `run-workflows.sh` with a local `.act.secrets` file (see `.github/workflows/.act.secrets` template) to reproduce CI secrets.
- When experimenting with `ingest.sh`, consider running in a scratch directory by overriding `OUT_DIR` to avoid clobbering previous snapshots.
