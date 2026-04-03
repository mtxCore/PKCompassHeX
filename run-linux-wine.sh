#!/usr/bin/env bash
# run-linux-wine.sh - Build PKCompassHeX on Linux and launch with Wine
# Usage:
#   ./run-linux-wine.sh [clean | --skip-build]
#   ./run-linux-wine.sh --help

set -euo pipefail

ARG1="${1-}"
DOTNET_INSTALL_DIR="$HOME/.dotnet"
PUBLISH_LOCK_LOG=""

if [[ "$ARG1" == "--help" || "$ARG1" == "-h" ]]; then
  cat <<EOF
Usage: $0 [clean | --skip-build]

This script:
  - Detects or installs .NET SDK and Wine
  - Builds PKHeX.WinForms for Windows x64 (unless skipped)
  - Launches PKCompassHeX.exe via Wine

Arguments:
  clean         - Performs a 'dotnet clean' before building
  --skip-build  - Skips the build process and attempts to launch the existing EXE
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


publish_pkhex() {
  dotnet publish PKHeX.WinForms -c Release -r win-x64 --self-contained false "$@"
}

if [[ "$ARG1" == "--skip-build" ]]; then
  echo "Skipping build as requested..."
else
  if [[ "$ARG1" == "clean" ]]; then
    echo "Cleaning previous builds..."
    dotnet clean PKHeX.sln
  fi
  dotnet build-server shutdown >/dev/null 2>&1 || true
  wineserver -k >/dev/null 2>&1 || true

  echo "Building PKHeX for Windows..."
  if ! publish_pkhex; then
    PUBLISH_LOCK_LOG="$(mktemp)"
    if publish_pkhex 2>&1 | tee "$PUBLISH_LOCK_LOG"; then
      :
    else
      if grep -qE "NETSDK1096|CrossGen|being used by another process" "$PUBLISH_LOCK_LOG"; then
        echo "Detected publish lock during ReadyToRun optimization; retrying with PublishReadyToRun=false..."
        dotnet build-server shutdown >/dev/null 2>&1 || true
        wineserver -k >/dev/null 2>&1 || true
        publish_pkhex -p:PublishReadyToRun=false
      else
        cat "$PUBLISH_LOCK_LOG"
        exit 1
      fi
    fi
  fi
fi

EXE_PATH=$(find PKHeX.WinForms/bin -path "*/win-x64/publish/*" -iname "PKCompassHeX.exe" | head -n 1)

if [[ -n "$EXE_PATH" && -f "$EXE_PATH" ]]; then
  echo "Found binary: $EXE_PATH"
  echo "Launching via Wine..."
  wine "$EXE_PATH"
else
  echo "ERROR: PKCompassHeX.exe not found."
  if [[ "$ARG1" == "--skip-build" ]]; then
    echo "You used --skip-build, but no binary exists. Run without the flag first."
  else
    echo "Check if the build failed or the publish directory path is correct."
  fi
  exit 1
fi

if [[ -n "$PUBLISH_LOCK_LOG" && -f "$PUBLISH_LOCK_LOG" ]]; then
  rm -f "$PUBLISH_LOCK_LOG"
fi