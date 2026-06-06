#!/usr/bin/env bash
#
# Optimize a raw ShareX capture of the demo gallery tour into the README GIF.
#
# Capture the tour with:  dotnet run --project src/CCSWE.Avalonia.Material.Demo -- --record
# then run:               eng/optimize-demo-gif.sh <capture.gif> [output.gif]
#
# Output defaults to docs/images/demo.gif (the path the root README links). Tune size/quality with
# the SCALE / COLORS / LOSSY environment variables.
#
set -euo pipefail

REPO_ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"

IN="${1:?usage: optimize-demo-gif.sh <capture.gif> [output.gif]}"
OUT="${2:-$REPO_ROOT/docs/images/demo.gif}"

SCALE="${SCALE:-0.58}"    # 0.58 of the 1377px capture -> ~800px wide
COLORS="${COLORS:-80}"    # palette size
LOSSY="${LOSSY:-120}"     # higher = smaller + lossier

if ! command -v gifsicle >/dev/null 2>&1; then
    echo "error: gifsicle not found. Install it with: sudo apt-get install -y gifsicle" >&2
    exit 1
fi

mkdir -p "$(dirname "$OUT")"
gifsicle -O3 --lossy="$LOSSY" --colors "$COLORS" --scale "$SCALE" "$IN" -o "$OUT"

echo "in:  $(du -h "$IN" | cut -f1)	$IN"
echo "out: $(du -h "$OUT" | cut -f1)	$OUT"
