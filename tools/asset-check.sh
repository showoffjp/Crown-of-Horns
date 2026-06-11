#!/usr/bin/env bash
# ============================================================================
# asset-check.sh — "what art/audio does the game still want?"
# Scans Assets/Resources against the exact names the engine loads by name
# (from FxSystem / AudioSystem / WorldArt). The game RUNS at 0% (everything
# no-ops gracefully); this just tells you what to drop in to make it look and
# sound like a finished game. No Unity required.
# ============================================================================
set -u
cd "$(dirname "$0")/.." || exit 1
RES="Assets/Resources"
g="\033[32m"; r="\033[31m"; y="\033[33m"; d="\033[2m"; n="\033[0m"

have_file() { ls "$RES/$1/$2".* >/dev/null 2>&1; }                 # Resources/<dir>/<name>.<anyext>
have_dirframes() { [ -n "$(ls "$RES/FX/$1/" 2>/dev/null)" ]; }      # Resources/FX/<effect>/ has frames

check_group() {
  local dir="$1" label="$2" kind="$3"; shift 3
  local items=("$@") have=0 total=${#}; total=${#items[@]}
  local miss=()
  for it in "${items[@]}"; do
    if [ "$kind" = fx ]; then have_dirframes "$it" && have=$((have+1)) || miss+=("$it")
    else have_file "$dir" "$it" && have=$((have+1)) || miss+=("$it"); fi
  done
  local col=$g; [ "$have" -eq 0 ] && col=$r; [ "$have" -lt "$total" ] && [ "$have" -gt 0 ] && col=$y
  printf "${col}%-26s %2d/%-2d${n}" "$label" "$have" "$total"
  if [ "${#miss[@]}" -gt 0 ]; then printf "  ${d}missing:${n} %s" "${miss[*]}"; fi
  printf "\n"
  echo "$have $total" >> /tmp/_assetsum
}

: > /tmp/_assetsum
echo "════════════════════════════════════════════════════════════════════"
echo " Crown of Horns — asset completeness  (scanning $RES)"
echo "════════════════════════════════════════════════════════════════════"

# FX frame folders (Resources/FX/<effect>/*.png) — FxSystem.PlayImpact + heals
check_group FX "Combat VFX (FX/)" fx \
  impact impact_fire impact_ice impact_lightning impact_holy impact_dark impact_acid impact_poison heal

# SFX one-shots (Resources/SFX/<name>) — AudioSystem.PlaySfx / PlayHit
check_group SFX "Sound FX (SFX/)" file \
  hit hit_fire hit_ice hit_lightning hit_holy hit_dark hit_acid hit_poison hit_physical \
  crit miss heal deed levelup

# Music loops (Resources/Music/<track>) — AudioSystem.PlayMusic
check_group Music "Music (Music/)" file \
  Hub Combat Camp Explore Court Vault Fugue

# Unit sprites (Resources/Sprites/<unit>) — UnitSpriteSkinner billboards
check_group Sprites "Unit sprites (Sprites/)" file \
  "Sister Garrow" "Roen Alleywind" Varra Naeve Ilfaeril Maerin \
  "The Returned" "The Last Returned" Enemy Player

# Dialogue portraits (Resources/Portraits/<speaker>)
check_group Portraits "Portraits (Portraits/)" file \
  "Sister Garrow" "Roen Alleywind" Varra Naeve Ilfaeril Maerin \
  "Aldric Morn" Vayle "The Returned"

# overall
have=$(awk '{h+=$1} END{print h}' /tmp/_assetsum); total=$(awk '{t+=$2} END{print t}' /tmp/_assetsum)
pct=$(( total>0 ? 100*have/total : 0 )); rm -f /tmp/_assetsum
echo "────────────────────────────────────────────────────────────────────"
printf " OVERALL: %d/%d  (%d%%)\n" "$have" "$total" "$pct"
echo
echo " ⓘ  The game is fully playable at 0% — every missing asset no-ops."
echo "    Fastest facelift (5 files): Sprites/Enemy, Sprites/Player, SFX/hit,"
echo "    Music/Combat, FX/impact/  → instantly re-skins all combat at once."
echo "    Full spec + naming + import settings: docs/ASSET_INTEGRATION.md"
echo "════════════════════════════════════════════════════════════════════"
