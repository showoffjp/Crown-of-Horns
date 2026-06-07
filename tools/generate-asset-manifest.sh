#!/usr/bin/env bash
#
# generate-asset-manifest.sh
# Walks Assets/ and writes a CSV catalog of every binary asset so the asset
# library can be reviewed/diffed without needing the binaries themselves.
#
# Usage:  bash tools/generate-asset-manifest.sh
# Output: Assets/asset-manifest.csv
#
set -euo pipefail

ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
ASSETS_DIR="$ROOT/Assets"
OUT="$ASSETS_DIR/asset-manifest.csv"

# Binary asset extensions we care about (sync with .gitattributes LFS rules).
EXTS="png jpg jpeg psd fbx blend wav mp3 ogg anim mp4 ase aseprite ttf otf mat"

# Map a path to a human category based on folder + filename prefix.
categorize() {
  case "$1" in
    */Sprites/Characters/*) echo "character" ;;
    */Sprites/Tiles/*)      echo "tile" ;;
    */Sprites/Props/*)      echo "prop" ;;
    */Sprites/Items/*)      echo "item" ;;
    */Sprites/UI/*)         echo "ui" ;;
    */Sprites/FX/*)         echo "fx" ;;
    */Tilesets/*)           echo "tileset" ;;
    */Materials/*)          echo "material" ;;
    */Textures/*)           echo "texture" ;;
    */Fonts/*)              echo "font" ;;
    */Audio/Music/*)        echo "music" ;;
    */Audio/SFX/*)          echo "sfx" ;;
    */Audio/Ambience/*)     echo "ambience" ;;
    */Audio/VO/*)           echo "vo" ;;
    */Models/*)             echo "model" ;;
    */Animations/*)         echo "animation" ;;
    */_Source/*)            echo "source" ;;
    *)                      echo "uncategorized" ;;
  esac
}

# Human-readable size.
hsize() { numfmt --to=iec --suffix=B "$1" 2>/dev/null || echo "${1}B"; }

# Is this path tracked by Git LFS?
lfs_files="$(git -C "$ROOT" lfs ls-files -n 2>/dev/null || true)"
is_lfs() { grep -qxF "$1" <<<"$lfs_files" && echo "yes" || echo "no"; }

echo "path,category,type,size,size_bytes,lfs" > "$OUT"

count=0
total=0
# Build a find expression for all extensions.
find_args=()
for e in $EXTS; do find_args+=( -iname "*.$e" -o ); done
unset 'find_args[${#find_args[@]}-1]'   # drop trailing -o

while IFS= read -r -d '' f; do
  rel="${f#"$ROOT"/}"
  bytes=$(stat -c%s "$f" 2>/dev/null || echo 0)
  ext="${f##*.}"
  cat="$(categorize "$f")"
  lfs="$(is_lfs "$rel")"
  echo "\"$rel\",$cat,${ext,,},$(hsize "$bytes"),$bytes,$lfs" >> "$OUT"
  count=$((count + 1))
  total=$((total + bytes))
done < <(find "$ASSETS_DIR" -type f \( "${find_args[@]}" \) -print0 2>/dev/null | sort -z)

echo "Catalogued $count asset(s), $(hsize "$total") total."
echo "Wrote $OUT"
