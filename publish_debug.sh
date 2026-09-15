#!/usr/bin/env bash

set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
VALHEIM_ROOT="${VALHEIM_INSTALL:-$HOME/Library/Application Support/Steam/steamapps/common/Valheim}"
DLL="$SCRIPT_DIR/More Ore Deposits/bin/Debug/More Ore Deposits.dll"
PLUGINS="${MOD_DEPLOYPATH:-$VALHEIM_ROOT/BepInEx/plugins}/MoreOreDeposits"

if [[ ! -f "$DLL" ]]; then
    echo "Error: Build the Debug configuration before deployment: $DLL" >&2
    exit 1
fi

mkdir -p "$PLUGINS"
cp -f "$DLL" "$PLUGINS/"

PDB="${DLL%.dll}.pdb"
if [[ -f "$PDB" ]]; then
    cp -f "$PDB" "$PLUGINS/"
fi

echo "Deployed More Ore Deposits to $PLUGINS"
