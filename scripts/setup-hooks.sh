#!/usr/bin/env bash
set -euo pipefail

ROOT="$(git rev-parse --show-toplevel 2>/dev/null || true)"
if [[ -z "${ROOT}" ]]; then
  echo "Not inside a Git repository."
  exit 1
fi

cd "${ROOT}"

chmod +x .githooks/pre-commit .githooks/pre-push
git config core.hooksPath .githooks

echo "Git hooks enabled (core.hooksPath=.githooks)."
echo "Pre-commit will format code; pre-push will verify format and build."