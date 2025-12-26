#!/bin/bash

# PSLastOutput Linux Installer
# Installs the PSLastOutput module for PowerShell Core (pwsh) on Linux.

echo "Installing PSLastOutput for Linux..."

# 1. Determine Module Path
# User scope for PowerShell Core is usually ~/.local/share/powershell/Modules
MODULE_DIR="$HOME/.local/share/powershell/Modules"
TARGET_DIR="$MODULE_DIR/PSLastOutput"

echo "Target Directory: $TARGET_DIR"

if [ ! -d "$TARGET_DIR" ]; then
    mkdir -p "$TARGET_DIR"
fi

# 2. Copy Files
# We assume this script is running from the 'Linux' folder, and sources are in '../Src/PSLastOutput'
SCRIPT_DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" &> /dev/null && pwd )"
SOURCE_DIR="$SCRIPT_DIR/../Src/PSLastOutput"

if [ ! -f "$SOURCE_DIR/PSLastOutput.psm1" ]; then
    echo "Error: Source files not found at $SOURCE_DIR"
    echo "Please ensure you run this script from the Linux folder inside the project structure."
    exit 1
fi

cp "$SOURCE_DIR/PSLastOutput.psm1" "$TARGET_DIR/"
cp "$SOURCE_DIR/PSLastOutput.psd1" "$TARGET_DIR/"

echo "Module files copied."

# 3. Update Profile
# Profile for pwsh is usually ~/.config/powershell/Microsoft.PowerShell_profile.ps1

PROFILE_DIR="$HOME/.config/powershell"
PROFILE_PATH="$PROFILE_DIR/Microsoft.PowerShell_profile.ps1"

if [ ! -d "$PROFILE_DIR" ]; then
    mkdir -p "$PROFILE_DIR"
fi

if [ ! -f "$PROFILE_PATH" ]; then
    echo "# PowerShell Profile" > "$PROFILE_PATH"
fi

# Check if already imported
if grep -q "Import-Module PSLastOutput" "$PROFILE_PATH"; then
    echo "Profile already includes PSLastOutput."
else
    echo "" >> "$PROFILE_PATH"
    echo "Import-Module PSLastOutput -ErrorAction SilentlyContinue" >> "$PROFILE_PATH"
    echo "Profile updated at $PROFILE_PATH"
fi

echo ""
echo "Installation Complete!"
echo "Please restart PowerShell (pwsh) to start using PSLastOutput."
echo "Shortcut: Ctrl+Shift+C"
