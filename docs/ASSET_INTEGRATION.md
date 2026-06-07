<div align="center">

# 🎨 Asset Integration Guide

### *How your art plugs into The Crown of Horns*

![VFX](https://img.shields.io/badge/VFX-auto--loaded-brightgreen)
![Pipeline](https://img.shields.io/badge/Pipeline-Resources_+_data--driven-blue)

</div>

The engine is **asset-agnostic**: drop sprites into the right folders and the systems find
them — no inspector wiring. This guide covers the **pixel FX pack** (which you've added)
and the conventions for **character sprites** and **tilemaps** you add next.

---

## ✨ 1. Combat VFX (the pixel FX pack) — WIRED ✅

The `FxSystem` plays sprite-frame animations on hits and spells. It loads frames from
**`Assets/Resources/FX/<effect>/`** at runtime (`Resources.LoadAll<Sprite>`), sorts them
naturally (so `2` before `10`), and plays them once at the target/caster.

### 📁 Where to put the frames
```
Assets/
└── Resources/
    └── FX/
        ├── impact/          ← generic hit (fallback)
        │   ├── 0.png 1.png 2.png ...   (any names; sorted by number)
        ├── impact_fire/     ← Fireball / Fire Bolt hits
        ├── impact_ice/  impact_lightning/  impact_holy/
        ├── impact_dark/ impact_acid/ impact_poison/
        └── heal/            ← Cure Wounds / Healing Word
```

> The pack ships as `Free/Part 1..15/<numbered>.png`. Each **Part = one effect** (12
> frames). To use one, copy a Part's frames into a folder above. Suggested first picks:
> - a fiery burst → `impact_fire/`  · a slash/impact → `impact/`  · a sparkle → `heal/`
> - an icy/holy/lightning Part → the matching `impact_*` folder
>
> You don't have to fill them all — anything missing simply no-ops (the game still runs).

### ⚙️ Import settings (crisp pixel art)
Select the frames in Unity → Inspector:
- **Texture Type:** `Sprite (2D and UI)`
- **Filter Mode:** `Point (no filter)`  ·  **Compression:** `None`
- **Pixels Per Unit:** match your tiles (e.g. `32` or `64`)
- (If a Part is a single sheet image, set **Sprite Mode: Multiple** and slice it in the
  Sprite Editor; each sub-sprite becomes one frame.)

### 🔌 How abilities use VFX
On any `AbilityDefinition` (or the code-built ones in `SwordCoastContent`):
- `hitVfx` — effect folder played on each target hit. **Blank → auto-picked by damage type**
  (`impact_fire` for Fire, `impact_holy` for Radiant, … else `impact`).
- `castVfx` — effect played at the caster when the ability is used (optional).
- Heals automatically play the `heal/` effect.

Tune timing in code: `FxSystem.PlayAt("impact_fire", pos, fps: 20, scale: 1.5f)`.

---

## 🧍 2. Character & Unit Sprites — HOOK READY ✅

Units render as colored cubes by default, but the **art seam is live**: `UnitSpriteSkinner`
(on the campaign director) scans `GridUnit`s every half-second and, the moment a matching
sprite exists, **hides the cube and parents a camera-facing sprite billboard** in its place —
keeping the cube's collider so clicks still land. No per-unit wiring; no art = no change.

**Just drop a PNG (imported as a Sprite) into `Assets/Resources/Sprites/`**, named after the unit.
Lookup order (first hit wins), via `WorldArt.Sprite`:
1. the full display name — `Sprites/Sister Garrow.png`
2. the first word — `Sprites/Doomguide.png` (covers "Doomguide Knight", "Doomguide Enforcer", …)
3. the faction — `Sprites/Enemy.png`, `Sprites/Player.png` (a catch-all silhouette)

So one `Sprites/Enemy.png` re-skins every foe at once; add `Sprites/Garrow.png` and she alone
upgrades. (`IsoDepthSorter` now sets `sortingOrder = -(x+y)*100` for true iso depth as units move; next: idle/walk/attack clips.)

> **Dialogue portraits — LIVE ✅:** drop `Assets/Resources/Portraits/<speaker>.png` (or reuse a
> `Sprites/<name>.png`) and it appears beside the conversation box automatically, via
> `WorldArt.Portrait` in `DialogueScreen`. Lookup: full speaker name → first word → map sprite, so
> `Portraits/Doomguide.png` covers every "Doomguide …" line. No portrait present → the box stands
> alone (graceful). Atlased or standalone sprites both work.

---

## 🗺️ 3. Tilemaps & Environments — PLANNED 📋

For hand-built isometric levels (vs. the current code-built grids):
1. Install Unity's **2D Tilemap** packages (Package Manager → *2D Tilemap Editor*).
2. Create an **Isometric Tilemap**; set the **Grid → Cell Size** to match
   `GridSystem.tileWidth / tileHeight` (default `1 × 0.5`).
3. Paint floors/walls; drive `GridCell.walkable` from a collision tilemap layer at load.
4. Put tile sprites under `Assets/Art/Tiles/` (any structure — they're referenced by the
   Tile assets, not by code).

---

## ✅ Quick checklist for new art

| Art type | Put it in | Consumed by |
|---|---|---|
| 💥 Combat effect | `Resources/FX/<effect>/` | `FxSystem` (auto) |
| 🧍 Unit sprite | `Resources/Sprites/<unit>.png` | **LIVE ✅** `UnitSpriteSkinner` (billboard + `IsoDepthSorter`) |
| 🖼️ Portrait | `Resources/Portraits/<speaker>.png` | Dialogue portraits — **LIVE ✅** (`WorldArt.Portrait`) |
| 🗺️ Tiles | `Assets/Art/Tiles/` + Tilemap | Isometric Tilemap |
| 🔊 SFX | `Resources/SFX/<name>.wav` | **LIVE ✅** (`AudioSystem.PlaySfx`) — `heal`, `miss`, `hit`, `hit_fire`/`hit_ice`/… |
| 🎵 Music | `Resources/Music/<track>.ogg` | **LIVE ✅** (`AudioSystem.PlayMusic`) — `Hub`, `Combat`, `Camp`, `Fugue`, `Court`, `Vault`, `Explore` |

> **Rule of thumb:** anything the code loads by name lives under a `Resources/` folder;
> anything wired by reference (Tilemaps, prefabs) can live anywhere under `Assets/`.

*Keep large binary art out of frequent commits where possible; Git LFS is already
configured in `.gitattributes` for `*.png`, `*.fbx`, `*.wav`, etc.*
