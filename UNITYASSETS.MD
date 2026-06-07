# UNITYASSETS — asset license ledger & buy-later shopping list

**Purpose:** track every asset the game wants — what's **already in the repo** (free, license-clean)
and what to **license before launch** (paid / per-account) — so nothing ships unpaid.

> ## ⚠️ Read this first — how "test now, license later" actually works
> Paid Unity Asset Store packs are **not** committed to this repo, and won't be. Two reasons:
> 1. **It'd be infringement.** Committing a pack you haven't licensed is unauthorized copying — and
>    because this repo is (or is about to be) **public**, it's unauthorized *distribution* to the
>    world. Intent to pay later is not a license.
> 2. **It's also just impossible from here** — this environment's network is GitHub-only; the Asset
>    Store is unreachable.
>
> **The correct way to test them now:** import each pack in **your** Unity editor under **your**
> account. Most have **free versions or trials**; paid ones you can buy when ready (the table below
> is your checklist). Assets imported this way live in *your* local project for testing and are
> covered by *your* account license — they never need to enter the repo. When you buy, tick the box.
>
> Only **CC0 / CC-BY** assets are committed here (see `ATTRIBUTIONS.md` + `Assets/FREE_ASSET_SOURCES.md`).
> *Prices below are approximate — verify on the store before buying.*

---

## ✅ Already in the repo — FREE, license-clean ($0)

| Asset | License | Where | Status |
|---|---|---|---|
| Game-Icons.net — 57 curated icons (items/FX/status) | CC BY 3.0 | `Assets/Art/Sprites/{Items,FX,Status}/game-icons/` | ✅ In repo + attributed |
| Kenney Fantasy UI Borders (your upload) | CC0 | `Assets/Art/Sprites/UI/` (extract zip) | ⬜ Extract & place |

---

## 🛒 To license before launch — PAID (import in-editor for testing now)

> Genre-fit picks for an isometric, party-based fantasy CRPG. Tick **Bought** when licensed.

### Environments & characters
| Pack | Publisher | For | ~Price | Bought |
|---|---|---|---|---|
| POLYGON Dungeons | Synty Studios | Modular dungeon kit (Cinderhaunt, crypts) | ~$30 | ⬜ |
| POLYGON Fantasy Kingdom | Synty Studios | Baldur's Gate hub / town | ~$30 | ⬜ |
| POLYGON Fantasy Characters | Synty Studios | Party, NPCs, companions | ~$30 | ⬜ |
| POLYGON Dark Fantasy | Synty Studios | Villains, undead, the Wall | ~$30 | ⬜ |
| POLYGON Nature | Synty Studios | Wilds / era exteriors | ~$30 | ⬜ |
| Synty Animation Base Locomotion | Synty Studios | Character anims for the above | ~$30 | ⬜ |
| **Subtotal** | | | **~$180** | |

### VFX (spells / eras)
| Pack | Publisher | For | ~Price | Bought |
|---|---|---|---|---|
| Hovl Studio — Magic Effects Pack | Hovl Studio | Fireball/ice/lightning, Spellplague blue fire | ~$35 | ⬜ |
| All In 1 Sprite Shader | SeaSunGames | Tint icons to era palettes, status flashes | ~$25 | ⬜ |
| **Subtotal** | | | **~$60** | |

### UI
| Pack | Publisher | For | ~Price | Bought |
|---|---|---|---|---|
| GUI PRO Kit — Fantasy RPG | Layer Lab | Full HUD/menus skin | ~$40 | ⬜ |
| **Subtotal** | | | **~$40** | |

### Audio
| Pack | Publisher | For | ~Price | Bought |
|---|---|---|---|---|
| Fantasy/RPG Music Collection | (various) | Era themes, hub, combat | ~$30 | ⬜ |
| RPG Essentials / Universal SFX | (various) | Combat & UI SFX | ~$30 | ⬜ |
| **Subtotal** | | | **~$60** | |

### Optional dev tools (not shipped art)
| Tool | Publisher | For | ~Price | Bought |
|---|---|---|---|---|
| DOTween Pro | Demigiant | UI/combat tweening | ~$15 | ⬜ |
| Odin Inspector | Sirenix | Editor tooling for ScriptableObjects | ~$55 | ⬜ |

### 💰 Estimated total to license: **~$340** (art/audio) + ~$70 (optional tools)

---

## 🆓 Free CC0 to grab in-editor before buying paid (try these first — may suffice)

Pull these in your editor/browser (CC0 = commit-safe too, but big — keep via Git-LFS per `ASSETS.md`):

- **KayKit Dungeon / Adventurers / Skeletons** (CC0) — a near drop-in CRPG starter; may replace several Synty packs.
- **Quaternius** modular dungeons, creatures, weapons (CC0).
- **Kenney** tiles, UI, particle FX, audio (CC0).

See `Assets/FREE_ASSET_SOURCES.md` for the full CC0-first plan and the license rule-of-thumb.

---

## ➕ How to add a row
When you license or import something, add it here: `Pack | Publisher | For | Price | Bought ✅`.
If it's CC0/CC-BY and you commit it, also record it in `ATTRIBUTIONS.md` (CC-BY needs credit).
