#!/usr/bin/env bash
# run-linux-wine.sh - Build PKHeX on Linux and launch with Wine
# Usage:
#   ./run-linux-wine.sh [clean]
#   ./run-linux-wine.sh --help

set -euo pipefail

ARG1="${1-}"
DOTNET_INSTALL_DIR="$HOME/.dotnet"

# --- Help Menu ---
if [[ "$ARG1" == "--help" || "$ARG1" == "-h" ]]; then
  cat <<EOF
Usage: $0 [clean]

This script:
  - Detects/Installs .NET SDK and Wine via your system package manager
  - Builds PKHeX.WinForms for Windows x64
  - Launches the resulting EXE via Wine

Arguments:
  clean  - Performs a 'dotnet clean' before building
EOF
  exit 0
fi


install_dotnet_fallback() {
  echo "Attempting dotnet-install script fallback..."
  mkdir -p "$DOTNET_INSTALL_DIR"
  curl -sSL https://dot.net/v1/dotnet-install.sh | bash -s -- --install-dir "$DOTNET_INSTALL_DIR" --channel 10.0
  export DOTNET_ROOT="$DOTNET_INSTALL_DIR"
  export PATH="$DOTNET_INSTALL_DIR:$PATH"
}


if ! command -v dotnet >/dev/null 2>&1; then
  echo ".NET SDK not found. Attempting install..."
  if command -v apt-get >/dev/null 2>&1; then
    sudo apt-get update && sudo apt-get install -y dotnet-sdk-10.0 || sudo apt-get install -y dotnet-sdk-8.0
  elif command -v pacman >/dev/null 2>&1; then
    sudo pacman -S --noconfirm --needed dotnet-sdk
  elif command -v dnf >/dev/null 2>&1; then
    sudo dnf install -y dotnet-sdk-10.0
  else
    install_dotnet_fallback
  fi
fi


if ! command -v wine >/dev/null 2>&1; then
  echo "Wine not found. Attempting install..."
  if command -v apt-get >/dev/null 2>&1; then
    sudo dpkg --add-architecture i386
    sudo apt-get update
    sudo apt-get install -y wine64 wine32 wine-mono
  elif command -v pacman >/dev/null 2>&1; then
    sudo pacman -S --noconfirm --needed wine wine-mono winetricks
  elif command -v dnf >/dev/null 2>&1; then
    sudo dnf install -y wine wine-mono
  fi
fi


if [[ "$ARG1" == "clean" ]]; then
  echo "Cleaning previous builds..."
  dotnet clean PKHeX.sln
fi

echo "Building PKHeX for Windows..."
# Using 'publish' ensures all dependencies are in one folder
dotnet publish PKHeX.WinForms -c Release -r win-x64 --self-contained false

EXE_PATH=$(find PKHeX.WinForms/bin -name "PKHeX.exe" | grep "win-x64" | head -n 1)

if [[ -f "$EXE_PATH" ]]; then
  echo "Found binary: $EXE_PATH"
  echo "Launching via Wine..."
  wine "$EXE_PATH"
else
  echo "ERROR: PKHeX.exe not found. Build may have failed."
  exit 1
fi