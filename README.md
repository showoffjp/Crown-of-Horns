# The Sundered Crown — Unity CRPG Starter Kit

A clean, runnable foundation for a **classic isometric, party-based, turn-based CRPG**
in the lineage of Baldur's Gate, Icewind Dale, Planescape: Torment, and Pathfinder:
Kingmaker — plus a full **story bible** to build content against.

This is a **starting point**, not a finished game. It gives you the engine and the
creative blueprint; you supply art, content assets, and time. See `docs/ROADMAP.md`
for an honest scope plan.

## What's in the box

**Engine (C#, `Assets/Scripts/`)** — compiles clean, no external packages:
- **Isometric grid** with 2:1 diamond projection + mouse picking (`Grid/`)
- **A\* pathfinding** over walkable/cost/occupied tiles (`Grid/Pathfinding.cs`)
- **Turn-based combat**: initiative, the 5e action economy (move/action/bonus), win/loss
  (`Combat/TurnManager.cs`)
- **5e/OGL-style rules**: ability scores, modifiers, proficiency, AC, HP, d20 attacks
  vs AC and saving throws (with save-for-half), crits (`Stats/`, `Characters/`,
  `Combat/AttackResolver.cs`)
- **Spells & resources**: spell slots, **healing**, **area-burst** spells, and a single
  validated `AbilityRunner` for using any ability (range/slot/action checks + AoE)
- **Status effects / conditions**: data-driven (Poisoned, Prone, Burning DoT, Blessed…)
  with duration, damage-over-time, and advantage/disadvantage (`Combat/StatusEffect.cs`)
- **Data-driven content** via ScriptableObjects: classes, races, abilities/spells,
  items, **branching dialogue graphs**, and **quests**
- **Story-state brain** (`Core/GameFlags.cs`): one flag store driving dialogue
  conditions, quest objectives, companion approval, faction reputation, and endings
- **Dialogue runner** with conditions, effects, and d20 skill checks (`Dialogue/`)
- **Quest manager** that completes objectives by watching flags (`Quests/`)
- **Party + shared inventory** (`Characters/Party.cs`, `Items/`)
- **JSON save/load** of the entire story state (`Save/SaveSystem.cs`)
- **Isometric camera** (pan/zoom/edge-scroll) (`Camera/`)
- **One-click playable demo** + on-screen HUD — press Play, fight a skirmish, zero setup
  (`Core/DemoBootstrap.cs`, `UI/DebugCombatHUD.cs`)

**Docs (`docs/`)**:
- `STORY_BIBLE.md` — the world (Aevyn), the central villain (the Mender), a second
  antagonist, **7 companions** with secrets and personal quests, factions, a 3-act +
  prologue structure with dungeons, and **five branching endings**, plus sample dialogue
  that maps 1:1 to the engine's format.
- `GETTING_STARTED.md` — install Unity → run the demo → author your first content.
- `ARCHITECTURE.md` — how the systems fit and where to extend.
- `CONTENT_PIPELINE.md` — build dialogue/quests/items as assets, no code.
- `ROADMAP.md` — milestones from vertical slice to ship, with anti-scope-creep rules.

## Quick start

1. New Unity project (**2D (URP)** template, Unity 6 LTS or 2022.3 LTS).
2. Copy this `Assets/` into it; copy `docs/` + `.gitignore` to the project root.
3. New 2D scene → empty GameObject → add the **`DemoBootstrap`** component → **Play**.
4. Click tiles to move, click adjacent enemies to attack, **Space** to end turn.

Full walkthrough: **`GETTING_STARTED.md`**.

## License / usage

Original code and the original setting/characters here are yours to build on. The
*rules chassis* is intentionally 5e/OGL-style; if you publish, review the Open Game
License / ORC terms and keep your own names and lore (the Story Bible is fully original).
```
