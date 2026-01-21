#!/usr/bin/env bash
# setup-figma-integration.sh
# Sets up Figma API integration for PTDoc Prototype v5

set -e

REPO_ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
FIGMA_FILE_KEY="1Fd3pzaGzvHboxFKuCz4dY"
ENV_FILE="${REPO_ROOT}/.env"
DESIGN_AI_DIR="${REPO_ROOT}/tools/design-ai-upstream"

echo "🎨 PFPT Figma Integration Setup"
echo "================================"
echo ""

# Check if .env exists
if [ ! -f "${ENV_FILE}" ]; then
  echo "❌ .env file not found. Creating from .env.example..."
  cp "${REPO_ROOT}/.env.example" "${ENV_FILE}"
fi

# Check for FIGMA_TOKEN
if ! grep -q "FIGMA_TOKEN=" "${ENV_FILE}"; then
  echo ""
  echo "📝 Step 1: Get your Figma Personal Access Token"
  echo "   1. Go to https://www.figma.com/settings"
  echo "   2. Scroll to 'Personal access tokens'"
  echo "   3. Click 'Create new token'"
  echo "   4. Copy the token (starts with 'figd_')"
  echo ""
  read -p "   Paste your Figma token: " FIGMA_TOKEN
  
  if [ -z "$FIGMA_TOKEN" ]; then
    echo "❌ No token provided. Exiting."
    exit 1
  fi
  
  echo "" >> "${ENV_FILE}"
  echo "# Figma API Configuration" >> "${ENV_FILE}"
  echo "FIGMA_TOKEN=${FIGMA_TOKEN}" >> "${ENV_FILE}"
  echo "✅ FIGMA_TOKEN added to .env"
else
  echo "✅ FIGMA_TOKEN already configured"
  # Extract token for use
  FIGMA_TOKEN=$(grep "^FIGMA_TOKEN=" "${ENV_FILE}" | cut -d '=' -f2)
fi

# Add FIGMA_FILE_KEY
if ! grep -q "FIGMA_FILE_KEY=" "${ENV_FILE}"; then
  echo "FIGMA_FILE_KEY=${FIGMA_FILE_KEY}" >> "${ENV_FILE}"
  echo "✅ FIGMA_FILE_KEY added to .env (PTDoc Prototype v5)"
else
  echo "✅ FIGMA_FILE_KEY already configured"
fi

echo ""
echo "🔧 Step 2: Installing dependencies..."
cd "${DESIGN_AI_DIR}"

# Check which package manager is available
if command -v pnpm &> /dev/null; then
  echo "Using pnpm..."
  pnpm install --no-frozen-lockfile
elif command -v npm &> /dev/null; then
  echo "Using npm..."
  npm install
else
  echo "❌ Neither pnpm nor npm found. Please install Node.js."
  exit 1
fi

echo "✅ Dependencies installed"

echo ""
echo "🎨 Step 3: Testing Figma connection..."

# Export environment variables for the Node script
export FIGMA_TOKEN
export FIGMA_FILE_KEY

# Run the figma pull script
if command -v pnpm &> /dev/null; then
  if pnpm run figma:pull; then
    echo ""
    echo "✅ Successfully connected to Figma!"
    echo "📦 Design tokens saved to: ${DESIGN_AI_DIR}/tokens/"
  else
    echo ""
    echo "❌ Failed to connect to Figma. Please check:"
    echo "   - Your FIGMA_TOKEN is valid (${FIGMA_TOKEN:0:10}...)"
    echo "   - You have access to file: ${FIGMA_FILE_KEY}"
    echo "   - The Figma file exists and is accessible"
    exit 1
  fi
else
  if npm run figma:pull; then
    echo ""
    echo "✅ Successfully connected to Figma!"
    echo "📦 Design tokens saved to: ${DESIGN_AI_DIR}/tokens/"
  else
    echo ""
    echo "❌ Failed to connect to Figma. Please check:"
    echo "   - Your FIGMA_TOKEN is valid (${FIGMA_TOKEN:0:10}...)"
    echo "   - You have access to file: ${FIGMA_FILE_KEY}"
    echo "   - The Figma file exists and is accessible"
    exit 1
  fi
fi

echo ""
echo "✨ Setup Complete!"
echo ""
echo "Your Figma credentials:"
echo "  File: PTDoc-Prototype-v5"
echo "  Key:  ${FIGMA_FILE_KEY}"
echo ""
echo "Next steps:"
echo "  1. cd tools/design-ai-upstream"
echo "  2. pnpm run tokens:build   # Generate CSS/XAML tokens"
echo "  3. pnpm run a11y:check     # Validate accessibility"
echo "  4. pnpm run sync           # Full sync pipeline"
echo ""
echo "Or use make commands:"
echo "  cd tools/design-ai-upstream && make tokens.pull"
echo ""
