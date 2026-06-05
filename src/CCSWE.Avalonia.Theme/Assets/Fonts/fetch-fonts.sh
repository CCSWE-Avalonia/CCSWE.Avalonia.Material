#!/usr/bin/env bash
# =============================================================================
# Font acquisition for CCSWE.Avalonia.Theme  (run from this Assets/Fonts/ folder)
# =============================================================================
# The emitted bundle references DM Sans + Plus Jakarta Sans but does not vendor
# the bytes. This pulls the upstream OFL variable TTFs (confirmed working in the
# consumer: they weight-match by internal family name) plus each OFL.txt, so the
# library becomes self-contained. Re-run to refresh.
#
# Both families resolve to the family names Fonts.axaml references:
#   "Plus Jakarta Sans"  and  "DM Sans".
set -euo pipefail

BASE="https://github.com/google/fonts/raw/main/ofl"

echo "Fetching Plus Jakarta Sans..."
curl -fL "$BASE/plusjakartasans/PlusJakartaSans%5Bwght%5D.ttf" -o "PlusJakartaSans[wght].ttf"
curl -fL "$BASE/plusjakartasans/OFL.txt"                       -o "PlusJakartaSans-OFL.txt"

echo "Fetching DM Sans..."
curl -fL "$BASE/dmsans/DMSans%5Bopsz,wght%5D.ttf" -o "DMSans[opsz,wght].ttf"
curl -fL "$BASE/dmsans/OFL.txt"                   -o "DMSans-OFL.txt"

echo "Done. Ensure the csproj includes: <AvaloniaResource Include=\"Assets/Fonts/*.ttf\" />"
echo "Keep the *-OFL.txt files in the repo to satisfy the OFL redistribution clause."
