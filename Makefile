# ---------- Core config ----------
SHELL := /bin/bash
.SHELLFLAGS := -eu -o pipefail -c
MAKEFLAGS += --warn-undefined-variables
.ONESHELL:
.DEFAULT_GOAL := help


# Load .env if present (doesn't error if missing)
ifneq (,$(wildcard .env))
include .env
export
endif

# Defaults (override via .env or CLI)
OUT_DIR       ?= reference
MAX_FILE_MB   ?= 5
INDEX_CMD     ?=
INGEST_SCRIPT ?= ./ingest.sh

CHECK_TOOLS   := git find rsync awk sed grep
OPTIONAL_TOOLS:= jq dotnet npm curl

# ---------- Safety ----------
# Prevent catastrophic rm if OUT_DIR is empty or "/"
ifeq ($(strip $(OUT_DIR)),)
$(error OUT_DIR is empty; set OUT_DIR or keep the default)
endif
ifeq ($(strip $(OUT_DIR)),/)
$(error OUT_DIR cannot be root "/")
endif

# ---------- Phony targets ----------
.PHONY: help check bootstrap clean ingest index all print-env verify manifest ingest-url ci

# ---------- Targets ----------
help: ## Show available targets
	@echo ""
	@echo "RepoMemory Facade — operational shortcuts"
	@echo ""
	@awk 'BEGIN {FS=":.*##"; printf "\033[1m%-20s\033[0m %s\n","Target","Description"} \
		/^[a-zA-Z0-9_.-]+:.*?##/ {printf "%-20s %s\n", $$1, $$2}' $(MAKEFILE_LIST) | sort
	@echo ""
	@echo "Examples:"
	@echo "  make bootstrap             # install missing tools (macOS Homebrew)"
	@echo "  make ingest                # run ingest.sh against current repo"
	@echo "  make index                 # run INDEX_CMD against $(OUT_DIR)"
	@echo "  make ingest-url REPO=https://github.com/org/repo"
	@echo "  make all                   # ingest + index"
	@echo "  make verify                # validate manifest JSON"
	@echo "  make manifest              # pretty-print manifest.json"
	@echo ""
	@echo "Config via .env or CLI:"
	@echo "  OUT_DIR=$(OUT_DIR)  MAX_FILE_MB=$(MAX_FILE_MB)"
	@echo "  INDEX_CMD='$(INDEX_CMD)'"
	@echo ""

check: ## Verify required tools are available
	@fail=0; \
	for t in $(CHECK_TOOLS); do \
		if ! command -v $$t >/dev/null 2>&1; then echo "❌ Missing: $$t"; fail=1; fi; \
	done; \
	for t in $(OPTIONAL_TOOLS); do \
		if ! command -v $$t >/dev/null 2>&1; then echo "⚠️  Optional tool not found: $$t"; fi; \
	done; \
	[ $$fail -eq 0 ] || (echo "Install missing tools or run 'make bootstrap' (macOS)"; exit 1)
	@echo "✅ All required tools are available."

DOTNET_LOCAL := $(HOME)/.dotnet/dotnet
DOTNET := dotnet
ifeq ("$(wildcard $(DOTNET_LOCAL))","")
	DOTNET := dotnet
else
	DOTNET := $(DOTNET_LOCAL)
endif

bootstrap:
	@echo "🔍 Checking .NET SDK version compatibility…"
	@if [ ! -f global.json ]; then \
		echo "❌ global.json not found. Aborting."; exit 1; \
	fi
	@REQUIRED_VERSION=$$(jq -r .sdk.version global.json); \
	INSTALLED=$$($(DOTNET) --list-sdks | grep $$REQUIRED_VERSION || true); \
	if [ -z "$$INSTALLED" ]; then \
		echo "📦 Installing required .NET SDK $$REQUIRED_VERSION"; \
		curl -sSL https://dot.net/v1/dotnet-install.sh | bash -s -- --version $$REQUIRED_VERSION; \
	else \
		echo "✅ Required SDK $$REQUIRED_VERSION already installed"; \
	fi
	@export PATH="$(HOME)/.dotnet:$$PATH"; \
	echo "🔧 Using dotnet from: $$(which $(DOTNET))"; \
	$(DOTNET) --info
	@echo "📦 Installing .NET workloads (MAUI, etc)…"
	@$(DOTNET) workload update
	@$(DOTNET) workload install maui


guard-%:
	@if [ -z "${${*}}" ]; then \
		echo "❌ Missing required var: $*"; exit 1; \
	fi

clean: ## Remove the reference output directory
	@rm -rf "$(OUT_DIR)"
	@echo "🧹 Cleaned $(OUT_DIR)"

ingest: check ## Run ingest.sh with current config (.env respected)
	@if [ ! -f .env ]; then \
		echo "⚠️  .env not found. Using defaults. Consider copying from .env.example."; \
	fi
	@if [ -d .git ] && [ -n "$$(git status --porcelain)" ]; then \
		echo "⚠️  Git working directory is not clean. Consider committing changes before ingesting."; \
	fi
	@chmod +x "$(INGEST_SCRIPT)"
	@OUT_DIR="$(OUT_DIR)" MAX_FILE_MB="$(MAX_FILE_MB)" INDEX_CMD="" "$(INGEST_SCRIPT)"
	@echo "📂 Artifacts at: $(OUT_DIR)"

index: check guard-INDEX_CMD ## Run the configured index command over OUT_DIR
	@echo "🔎 Indexing with: $(INDEX_CMD)"
	@$(INDEX_CMD)

all: ingest index ## Ingest then index (end-to-end)
	@echo "✅ Ingest and indexing complete."

print-env: ## Print effective configuration
	@echo "OUT_DIR=$(OUT_DIR)"
	@echo "MAX_FILE_MB=$(MAX_FILE_MB)"
	@echo "INDEX_CMD=$(INDEX_CMD)"
	@echo "INGEST_SCRIPT=$(INGEST_SCRIPT)"

verify: ## Validate manifest JSON (jq)
	@if [ ! -f "$(OUT_DIR)/manifest.json" ]; then echo "❌ $(OUT_DIR)/manifest.json not found"; exit 1; fi
	@if ! command -v jq >/dev/null 2>&1; then echo "❌ jq not installed"; exit 1; fi
	@jq -e . "$(OUT_DIR)/manifest.json" >/dev/null && echo "✅ manifest.json is valid"

manifest: ## Pretty-print manifest.json
	@if [ ! -f "$(OUT_DIR)/manifest.json" ]; then echo "❌ $(OUT_DIR)/manifest.json not found"; exit 1; fi
	@if ! command -v jq >/dev/null 2>&1; then echo "❌ jq not installed"; exit 1; fi
	@jq . "$(OUT_DIR)/manifest.json"

# --- Quick external snapshot without changing your ingest.sh ---
# Requires: git + rsync. Creates OUT_DIR/mirror/<name>-<sha>/ with README/docs/src slices.
ingest-url: check guard-REPO ## Sparse, pinned mirror of a public repo URL (REPO=...)
	@tmp_dir="$$(mktemp -d)"; \
	name="$$(basename -s .git "$(REPO)")"; \
	echo "🌐 Ingesting $$name from $(REPO) …"; \
	git clone --depth 1 --filter=blob:none --sparse "$(REPO)" "$$tmp_dir/$$name" >/dev/null 2>&1 || { echo "❌ clone failed"; exit 1; }; \
	( cd "$$tmp_dir/$$name" && git sparse-checkout set README.md README.* docs/** samples/** src/** && sha="$$(git rev-parse HEAD)"; \
	  mkdir -p "$(OUT_DIR)/mirror/$$name-$$sha"; \
	  rsync -a --prune-empty-dirs --include="README.md" --include="README.*" --include="docs/***" --include="samples/***" --include="src/***" --exclude="*" ./ "$(OUT_DIR)/mirror/$$name-$$sha/"; \
	  printf '%s\n' "$$sha" > "$(OUT_DIR)/mirror/$$name-$$sha/.commit"; \
	  cat >"$(OUT_DIR)/mirror/$$name-$$sha/MANIFEST.yaml" <<EOF\nsource: $(REPO)\ncommit: $$sha\ningest_date: $$(date -u +%F)\ninclude: [README.*, docs/**, samples/**, src/**]\nEOF \
	); \
	rm -rf "$$tmp_dir"; \
	echo "📦 Mirror ready at $(OUT_DIR)/mirror/$$name-$$sha"
	@echo "👉 Next: set INDEX_CMD and run 'make index' if you want embeddings."

# --- CI-friendly pipeline (non-interactive, verbose off) ---
ci: ## Run in CI (ingest + optional index). Honors INDEX_CMD if set.
	@$(MAKE) --no-print-directory check
	@OUT_DIR="$(OUT_DIR)" MAX_FILE_MB="$(MAX_FILE_MB)" INDEX_CMD="" "$(INGEST_SCRIPT)"
	@if [ -n "$(INDEX_CMD)" ]; then \
		echo "🔎 CI indexing with: $(INDEX_CMD)"; \
		$(INDEX_CMD); \
	else \
		echo "ℹ️  INDEX_CMD not set; skipping index in CI"; \
	fi
