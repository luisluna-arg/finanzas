#!/bin/bash
#
# Updates existing shadcn/ui components in FinanceApp
#
# Usage:
#   ./.bin/bash/update-shadcn-components.sh

set -e

# Get the directory of this script and navigate to FinanceApp
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
FINANCE_APP_DIR="$SCRIPT_DIR/../../FinanceFrontEnd/FinanceApp"
SHADCN_DIR="$FINANCE_APP_DIR/app/components/ui/shadcn"

echo "Scanning existing shadcn/ui components..."

cd "$FINANCE_APP_DIR"

# Get all .tsx files (excluding index.tsx)
components=()
for file in "$SHADCN_DIR"/*.tsx; do
    filename=$(basename "$file" .tsx)
    if [ "$filename" != "index" ]; then
        components+=("$filename")
    fi
done

component_count=${#components[@]}
echo "Found $component_count components to update"
echo ""

updated=0
failed=0

for component in "${components[@]}"; do
    echo "Updating $component..."
    if npx shadcn@latest add "$component" -y -o > /dev/null 2>&1; then
        ((updated++))
    else
        ((failed++))
        echo "  Failed to update $component"
    fi
done

echo ""
echo "Update complete:"
echo "  Updated: $updated"
if [ $failed -gt 0 ]; then
    echo "  Failed: $failed"
fi
