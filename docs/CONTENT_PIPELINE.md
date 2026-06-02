# Content Pipeline — authoring the game without code

Everything narrative- or rules-shaped is a **ScriptableObject asset**. This doc shows
how to turn Story-Bible material into assets the engine runs.

## Conventions

- **Flag keys** are dotted and lowercase: `act1.lantern.recovered`,
  `companion.sable.approval`, `faction.wardens.reputation`, `bg.wardensward`.
  Keep a master list in a spreadsheet — it's the contract between writers and systems.
- **Asset folders:** `Assets/Data/Classes`, `/Races`, `/Abilities`, `/Items`,
  `/Dialogue`, `/Quests`, `/Characters`, `/Encounters`.
- **Ids** on assets (`questId`, `itemId`, `conversationId`) must be unique and stable;
  saves reference them.

## 1. A class / race / ability

`Create → Sundered Crown → Class Definition` (Race / Ability similarly). Fill the
inspector fields. A `CharacterSheet` points at one Class + one Race + a list of
Abilities. Call `RecalculateMaxHitPoints()` after assembling a sheet.

## 2. A dialogue graph (the core writing tool)

`Create → Sundered Crown → Dialogue Graph`. Set `startNodeId`. Add `nodes`. Each node:

- `id` — unique within the graph (`corin.intro.1`).
- `speaker`, `text`.
- `onEnter` — `FlagClause[]` applied when shown (e.g. `SetTrue corin.met`).
- `choices` — each choice has:
  - `text`, `nextNodeId`
  - `conditions` — `FlagClause[]`; ALL must pass or the choice is hidden
    (`RequireBoolTrue`, `RequireBoolFalse`, `RequireIntAtLeast`).
  - `effects` — `FlagClause[]` applied on pick (`SetTrue`, `SetFalse`, `AddInt`).
  - `checkAbility` + `checkDC` — optional d20 skill check; on failure jump to
    `failNodeId`. (`AddInt` on a `companion.*.approval` key = approval shift.)
- `autoNextNodeId` — if a node has no choices, auto-advance here (blank = end).

**Worked example — Corin's intro from the Story Bible:**

| id | speaker | text (abbrev) | choices → next (conditions / effects) |
|---|---|---|---|
| `corin.intro.0` | The Mender | "Sit. The tea's real..." | *(auto → `corin.intro.1`; onEnter: SetTrue `corin.met`)* |
| `corin.intro.1` | The Mender | "I mean to give it back. Help me?" | 1 "Give it back how?" → `corin.intro.truth` · 2 "[INSIGHT 15] You're grieving" → `corin.intro.grief` (Wisdom DC15, fail→`corin.intro.deflect`) · 3 "Not buying it" → `corin.intro.refuse` (effect: AddInt `companion.sable.approval` +5) · 4 "The order says heretic" → `corin.intro.warden` (cond: RequireBoolTrue `bg.wardensward`) |
| `corin.intro.truth` | The Mender | "...it needs souls. Mine. Yours, if you choose." | onEnter: SetTrue `corin.knows_cost_revealed` |

Wire an NPC's interact trigger to `DialogueRunner.Instance.Begin(thatGraph)`. Set
`DialogueRunner.ResolvePlayerSpeaker` to a func returning the speaking PC's sheet so
skill checks use real modifiers. Bind UI to `OnNodeShown` / `OnSkillCheck` /
`OnConversationEnded`.

## 3. A quest

`Create → Sundered Crown → Quest`. Give it `questId`, `title`, `summary`. Add
`objectives`, each pointing `completionFlag` at the flag that finishes it. Set the
quest's `completionFlag` / `failureFlag`. Register the asset in `QuestManager.allQuests`.
Call `QuestManager.Instance.StartQuest("q.sable.mercy")` from dialogue/triggers; the
manager then completes objectives automatically as flags flip.

> Because quests just watch flags, dialogue and quests stay decoupled: a conversation
> sets `act1.lantern.recovered = true`, and *any* quest objective watching that key
> ticks — no direct references between them.

## 4. An item / inventory

`Create → Sundered Crown → Item`. Weapons set `weaponDamage`/`weaponDamageType`;
armor sets `armorClassBonus`; consumables reference a `useEffect` ability.
`Party.Instance.inventory.Add(item)` / `.AddGold(n)`.

## 5. An encounter / map

For the demo we spawn cubes from code. For real maps:
1. Build the floor with a **2D Isometric Tilemap** (Unity package), cell size matched to
   `GridSystem.tileWidth/tileHeight`.
2. Mark non-walkable tiles by setting `GridCell.walkable=false` (drive this from a
   collision tilemap or a hand-authored bitmask at scene load).
3. Place character **prefabs** (a sprite + `GridUnit`) at `startCoord`s, assign each a
   `CharacterSheet` (built from your Data assets) in a spawner.
4. Add a `TurnManager` + `EncounterController` to start combat when the party enters.

## Authoring at scale — a tip

Hand-authoring hundreds of dialogue nodes in the inspector gets painful. Two good
escape hatches once you've outgrown the built-in graph:
- **Ink** (inkle) or **Yarn Spinner** — write branching dialogue in a text DSL, import
  at build time, and feed it into the same `GameFlags` brain. Keep the flag convention.
- A small CSV/JSON importer that builds `DialogueGraph` assets from a spreadsheet.
Both let writers work in text, not the inspector, while the *engine* stays identical.
```
