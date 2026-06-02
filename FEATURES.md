<div align="center">

# ⚔️ THE CROWN OF HORNS — Feature Codex 📜

### *A classic isometric, party-based CRPG for the Forgotten Realms · built on D&D 5e*

![Status](https://img.shields.io/badge/Build-Vertical_Slice-brightgreen)
![Engine](https://img.shields.io/badge/Engine-Unity_6_LTS-black?logo=unity)
![Lang](https://img.shields.io/badge/C%23-10-239120?logo=csharp)
![Ruleset](https://img.shields.io/badge/Rules-D%26D_5e_(SRD_5.1)-b51e2e?logo=dungeonsanddragons)
![Combat](https://img.shields.io/badge/Combat-Turn--Based-blue)
![Scripts](https://img.shields.io/badge/Scripts-38_files_·_3.7k_LOC-informational)

</div>

---

> **Legend:** ✅ Implemented & playable · 🚧 In progress · 📋 Planned next · 💡 Stretch / future

---

## 🟢 Implemented — Playable Today

### 🗺️ World & Movement
| | Feature | Notes |
|---|---|---|
| ✅ | **Isometric grid** | Classic 2:1 diamond projection, world↔grid conversion, editor gizmos |
| ✅ | **A\* pathfinding** | Respects walkability, difficult-terrain cost, occupied tiles |
| ✅ | **Click-to-move** | Tile pathing that spends a turn's movement budget |
| ✅ | **Isometric camera** | Pan (WASD/edge-scroll), zoom, bounds clamping |
| ✅ | **Smooth unit movement** | Tile-by-tile traversal with depth sorting |

### ⚔️ Combat (D&D 5e core)
| | Feature | Notes |
|---|---|---|
| ✅ | **Turn-based initiative** | d20 + DEX, cached roll, clean ordering |
| ✅ | **Action economy** | Move · Action · Bonus Action per turn |
| ✅ | **Attack rolls vs AC** | d20 + ability mod + proficiency, nat-1 miss / nat-20 crit |
| ✅ | **Saving throws** | Spell save DC = 8 + prof + casting mod |
| ✅ | **Save-for-half** | Fireball-style partial damage |
| ✅ | **Advantage / disadvantage** | Derived from conditions, cancel per 5e |
| ✅ | **Critical hits** | Double the damage dice |
| ✅ | **Spell slots** | Per-level pools, spent by leveled spells, refreshed on rest |
| ✅ | **Healing abilities** | Cure Wounds & friends |
| ✅ | **Area-of-effect** | Burst targeting around a tile (Fireball) |
| ✅ | **Status effects / conditions** | Poisoned, Prone, Frightened, Burning (DoT), Blessed… |
| ✅ | **Damage-over-time** | Ticked at start of turn |
| ✅ | **Incapacitation** | Stun/incapacitate skips the turn |
| ✅ | **`AbilityRunner`** | One validated path: range, slots, economy, AoE, logging |
| ✅ | **Enemy AI** | Target selection, close-to-range, attack (melee & ranged) |
| ✅ | **13 damage types** | Slashing → Psychic → Radiant → Force… |

### 🧙 Characters & Progression
| | Feature | Notes |
|---|---|---|
| ✅ | **Six ability scores** | STR/DEX/CON/INT/WIS/CHA with 5e modifiers |
| ✅ | **Classes (data-driven)** | Hit die, primary ability, spellcasting, granted abilities |
| ✅ | **Ancestries / races** | Speed + ability bonuses (Human, Elf, Dwarf, Tiefling…) |
| ✅ | **Backgrounds** | Skills, feature, **flag that unlocks reactive dialogue** |
| ✅ | **Character creation** | Point-buy (27) **or** standard array, full race/class/bg flow |
| ✅ | **XP & leveling** | Full 5e XP table, HP/proficiency growth, level-up events |
| ✅ | **Party & shared inventory** | Roster, active party (4–6), gold |
| ✅ | **Equipment hooks** | Equipped weapon ability + armor AC |

### 💬 Narrative Systems
| | Feature | Notes |
|---|---|---|
| ✅ | **Branching dialogue** | Graph of nodes/choices authored as assets |
| ✅ | **Dialogue skill checks** | d20 + ability vs DC, success/fail branches |
| ✅ | **Conditions & effects on choices** | Gate/hide choices; set flags; shift approval |
| ✅ | **GameFlags story brain** | One flag store driving *everything* reactive |
| ✅ | **Companion approval** | Per-companion axis, clamped, event-driven |
| ✅ | **Faction reputation** | Numeric rep gating content & endings |
| ✅ | **Quests** | Objectives auto-complete by watching flags; XP/gold rewards |
| ✅ | **Quest journal events** | Started / objective / completed hooks |

### 🖥️ UI & Game Flow
| | Feature | Notes |
|---|---|---|
| ✅ | **Real uGUI combat HUD** | Portraits + live HP bars, initiative, action economy, **clickable ability bar**, log, End Turn — built at runtime, zero setup |
| ✅ | **Dialogue screen** | Speaker, body, choices, live skill-check results |
| ✅ | **Character-creation screen** | Point-buy stepper, race/class/bg pickers |
| ✅ | **JSON save/load** | Flags + quests + gold, with quick-save (F5/F9) |
| ✅ | **One-click demos** | `DemoBootstrap` (skirmish) & `PrologueBootstrap` (create → talk → fight → XP) |

---

## 🚧 In Progress / Next Up

| | Feature | Why it matters |
|---|---|---|
| 🚧 | **Full Act I content** | Baldur's Gate hub + Tidewrack/Cinderhaunt dungeon as real scenes |
| 📋 | **Tilemap-based maps** | Author iso levels with Unity's Isometric Tilemap + sprite art |
| 📋 | **Sprite characters & animation** | Replace demo cubes with animated portraits/units |
| 📋 | **Inventory & equipment UI** | Drag-drop, slots, tooltips, item comparison |
| 📋 | **Reactions & opportunity attacks** | Leaving reach provokes; readied actions |
| 📋 | **Concentration & buffs** | Concentration checks, dispel, duration UI |
| 📋 | **Short/long rest** | Slot/HP recovery, hit dice, camp scenes |
| 📋 | **Merchants & economy** | Buy/sell, barter, the Guild & Zhentarim fences |

---

## 📋 Planned — The Road to a Baldur's-Gate-Class RPG

<details open>
<summary><b>⚔️ Combat depth</b></summary>

- 📋 Conditions: Grappled, Restrained, Charmed, Paralyzed, Petrified, Stunned (full set)
- 📋 Cover & line-of-sight, flanking, height advantage
- 📋 Multi-target spells, lines/cones, persistent hazards (grease, web, wall of fire)
- 📋 Resistances / immunities / vulnerabilities per damage type
- 📋 Death saves, downed-but-stabilized, revivify
- 💡 Reaction-based utility AI (utility scoring, retreat, focus-fire, kiting)
</details>

<details>
<summary><b>🧙 Character systems</b></summary>

- 📋 Subclasses / archetypes, feats, ability-score improvements
- 📋 Multiclassing
- 📋 Skill list & proficiency (Athletics → Arcana → Persuasion)
- 📋 Spell preparation & spellbook (Wizard) vs known (Sorcerer/Bard)
- 📋 Inspiration, exhaustion, carrying capacity
- 💡 Origin/destiny system tied to the *Returned* trait
</details>

<details>
<summary><b>💬 Narrative & reactivity</b></summary>

- 📋 7 companions with recruit convos, banter, **personal quests**, romances, betrayals
- 📋 Companion approval gates (lock/unlock dialogue, leave the party)
- 📋 Reputation-gated faction questlines (Harpers, Flaming Fist, Order of the Gauntlet, Zhentarim)
- 📋 The **five branching endings** + per-companion/faction epilogue slides
- 📋 Ink / Yarn Spinner import pipeline for writing at scale
- 💡 Reactive world state: villages remember, the dead linger near you
</details>

<details>
<summary><b>🗺️ World & content</b></summary>

- 📋 6 hubs/regions: Baldur's Gate districts, Candlekeep, Elturel, Fields of the Dead, the Fugue Plane, the City of Judgment
- 📋 14 dungeons/encounter maps; the Court of the Dead finale gauntlet
- 📋 Bestiary of ~22 archetypes (the Unbound, Doomguides, devils, Fugue shades)
- 📋 Traps, locks, secret doors, environmental interactables
- 📋 Day/night, fog of war, dynamic 2D lighting (torchlit dungeons)
</details>

<details>
<summary><b>🖥️ UX, audio & production</b></summary>

- 📋 Main menu, settings, save-slot manager, codex/journal/bestiary
- 📋 Tooltips, damage numbers, hit/miss VFX, ability targeting previews
- 📋 Music states (explore/combat/dialogue), SFX, ambience, VO hooks
- 📋 Localization (string tables), accessibility (text size, colorblind, remap, combat speed)
- 📋 Controller support
- 💡 Steam/itch build pipeline, achievements, cloud saves
</details>

---

## 🧱 Architecture at a Glance

```
GameManager ── Party ── QuestManager ── DialogueRunner ── SaveSystem
     │            │           │              │              │
     └──────── GameFlags (the story brain: flags · approval · reputation) ───────┘
                        │
   GridSystem ── TurnManager ── CharacterSheet ── AbilityRunner ── AttackResolver
   (iso + A*)   (initiative)    (5e stats, POCO)   (validate+AoE)   (pure d20 math)
        │                                                  │
     GridUnit ◄────── EncounterController (AI + XP) ──────►  Progression (XP/level)
        │
   PlayerCombatInput · CombatHUD · DialogueScreen · CharacterCreationScreen
```

> 🧩 **Design pillar:** *Code is the engine, ScriptableObject assets are the game.* A
> writer/designer builds classes, spells, items, dialogue, and quests **without touching
> C#** — which is exactly how a small team ships a big CRPG.

---

<div align="center">

### 🎲 *"You came back. Few do."* 🎲

**Next milestone →** the Baldur's Gate hub & first real dungeon. See `docs/ROADMAP.md`.

</div>
