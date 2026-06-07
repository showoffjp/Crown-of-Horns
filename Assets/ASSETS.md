# Asset Pipeline & Organization Guide

This document is the single source of truth for **where assets go**, **how they're
named**, and **how to get them into the repo** without hitting upload limits.

---

## TL;DR — getting big assets in

GitHub's 25 MB and the chat's 30 MB limits are *weak side-channels*. Don't use them.
This repo is already configured for **Git LFS** (see `../.gitattributes`), which handles
files up to **2 GB each**. Push from the command line:

```bash
git lfs install                          # one-time, per machine
git add Assets/Art/Sprites/Characters/   # binaries auto-routed to LFS by .gitattributes
git commit -m "Add hero sprite set"
git push -u origin <your-branch>
```

> **Storage ceiling to watch:** GitHub's free tier is **1 GB LFS storage + 1 GB/month
> bandwidth**. Beyond that, $5 "data packs" add 50 GB each. Keep heavy raw masters
> (`.psd`, `.blend`) in `_Source/` and consider whether they belong in the repo at all
> (see below).

---

## Folder structure

```
Assets/
├── Art/
│   ├── Sprites/
│   │   ├── Characters/   # player, NPCs, enemies (8-dir iso sprite sets)
│   │   ├── Tiles/        # isometric terrain/floor/wall tiles
│   │   ├── Props/        # decorations, furniture, world objects
│   │   ├── Items/        # inventory icons + world pickups
│   │   ├── UI/           # panels, buttons, frames, cursors
│   │   └── FX/           # spell/particle sprite sheets
│   ├── Tilesets/         # Unity Tilemap palettes / TileBase assets
│   ├── Materials/        # .mat
│   ├── Textures/         # non-sprite textures, normal maps
│   └── Fonts/            # .ttf / .otf / SDF font assets
├── Audio/
│   ├── Music/            # looping tracks (mus_*)
│   ├── SFX/              # one-shots (sfx_*)
│   ├── Ambience/         # environmental beds (amb_*)
│   └── VO/               # voice-over lines (vo_*)
├── Models/              # 3D, if any (.fbx)
├── Animations/         # .anim / .controller
└── _Source/            # raw editable masters (.psd, .blend, .ase) — heavy, see note
```

**`_Source/` note:** these are the *editable originals*, not the game-ready exports.
They're large and the engine never loads them. Options, in order of preference:
1. Keep them out of the repo entirely (a Drive/Dropbox folder), OR
2. Commit them via LFS but be aware they eat your 1 GB quota fastest.

---

## Naming convention

**`lowercase_snake_case`**, with a **category prefix**. No spaces, no caps, no version
suffixes like `_final_v2_USE_THIS`.

| Category   | Prefix  | Example |
|------------|---------|---------|
| Character  | `char_` | `char_hero_idle_se.png` |
| Tile       | `tile_` | `tile_grass_01.png` |
| Prop       | `prop_` | `prop_barrel_wood.png` |
| Item icon  | `item_` | `item_potion_health.png` |
| UI         | `ui_`   | `ui_panel_inventory.png` |
| FX         | `fx_`   | `fx_fireball_sheet.png` |
| Music      | `mus_`  | `mus_combat_01.ogg` |
| SFX        | `sfx_`  | `sfx_sword_hit_01.wav` |
| Ambience   | `amb_`  | `amb_forest_loop.ogg` |
| Voice-over | `vo_`   | `vo_npc_elder_greeting.wav` |

### Isometric direction suffixes
For directional sprites use compass suffixes (matches the iso camera). Use 8-dir where
art exists, 4-dir otherwise:

`_n  _ne  _e  _se  _s  _sw  _w  _nw`

Example animation set:
```
char_hero_walk_se_00.png
char_hero_walk_se_01.png
char_hero_attack_n_00.png
```

---

## Cataloging

Run the manifest generator any time you add assets — it produces a CSV I can read
without needing the binaries themselves:

```bash
bash tools/generate-asset-manifest.sh
```

This writes `Assets/asset-manifest.csv` (path, category, type, size, LFS-tracked?).
Commit it alongside the assets so the catalog stays in sync.

> **Planning a big import?** Use `ASSET_IMPORT_MANIFEST.md` — it triages your library into
> *re-import* (Asset Store packages you should **not** upload), *commit-via-LFS* (your custom
> assets), and *keep-out* (raw masters), with a no-terminal GitHub Desktop workflow and an LFS
> quota tracker. Doing that triage first is what keeps the repo small and the push under quota.

---

## What I (Claude) actually need

- **To write game code:** just the **manifest + `.meta` files** (filenames, paths, sprite
  dimensions, import settings). These are text and commit with no size issues.
- **To review art visually:** the actual image for the *specific* files in question —
  push them to the repo, or drop individual files (≤30 MB each) into chat.
