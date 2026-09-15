#!/usr/bin/env bash

set -euo pipefail

if [[ $# -ne 1 ]]; then
    echo "Usage: $0 <version>" >&2
    exit 1
fi

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
VERSION="$1"
PROJECT_DIR="$SCRIPT_DIR/More Ore Deposits"
DLL="$PROJECT_DIR/bin/Release/More Ore Deposits.dll"
PACKAGE_DIR="$PROJECT_DIR/Package"
PLUGINS="$PACKAGE_DIR/plugins"
ZIP_DESTINATION="$PROJECT_DIR/bin/Release/MoreOreDeposits.$VERSION.zip"

if [[ ! -f "$DLL" ]]; then
    echo "Error: Build the Release configuration before packaging: $DLL" >&2
    exit 1
fi

mkdir -p "$PLUGINS"
cp -f "$DLL" "$PLUGINS/"

rm -f "$ZIP_DESTINATION"
(
    cd "$PACKAGE_DIR"
    zip -r "$ZIP_DESTINATION" .
)

echo "Created $ZIP_DESTINATION"
