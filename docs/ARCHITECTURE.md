# Architecture

The starter is organized so that **code is the engine** and **ScriptableObject assets
are the game**. A designer/writer can build most of the content without opening C#.

## Layer map

```
                       ┌─────────────────────────────┐
                       │        GameManager          │  persistent, quick save/load
                       └──────────────┬──────────────┘
        ┌──────────────┬──────────────┼──────────────┬──────────────┐
        ▼              ▼              ▼              ▼              ▼
   GameFlags        Party        QuestManager   DialogueRunner   SaveSystem
 (story brain)   (roster+inv)  (reacts to flags)(walks graphs)  (JSON DTO)
        ▲              ▲              ▲              ▲              ▲
        └──────────────┴──────── read/write ────────┴──────────────┘
                                   │
          ┌────────────────────────┼────────────────────────┐
          ▼                        ▼                         ▼
     GridSystem               TurnManager              CharacterSheet
  (iso projection +       (initiative, action      (5e stats, HP, AC,
   A* pathfinding)          economy, win/loss)        modifiers — POCO)
          ▲                        ▲                         ▲
          │                        │                         │
       GridUnit ◄──────── EncounterController ──────► AttackResolver
   (on-map MonoBehaviour)   (drives AI + player turns)  (pure d20 math)
          ▲
   PlayerCombatInput (mouse) / SimpleEnemyAI (in EncounterController)
```

## Key design decisions

1. **CharacterSheet is a plain C# object, not a MonoBehaviour.** Stats, HP, and the
   5e math live in a POCO so they serialize to save files and unit-test without a
   scene. `GridUnit` is the thin MonoBehaviour that gives a sheet a body on the map.

2. **GameFlags is the single source of truth for story state.** Dialogue conditions,
   quest objectives, companion approval, faction rep, and ending selection all read and
   write the same flag store. This is what makes a BG-scale reactive narrative tractable
   — and it serializes to one small JSON blob.

3. **Everything content-shaped is a ScriptableObject.** Classes, races, abilities/spells,
   items, dialogue graphs, and quests are *data assets*, referenced by id. Saves store
   ids + runtime state, so they stay tiny and survive content edits.

4. **Pure resolution functions.** `AttackResolver` and `Dice` have no scene dependency,
   so combat math is deterministic-testable (seed `Dice`, assert outcomes).

5. **Events over polling.** `TurnManager`, `DialogueRunner`, `QuestManager`, and
   `Inventory` raise C# events; UI binds to them. The provided `DebugCombatHUD` is an
   IMGUI consumer you'll replace with a real uGUI / UI Toolkit HUD.

## Isometric projection (the CRPG look)

`GridSystem.GridToWorld` uses a 2:1 diamond projection: grid `(x,y)` →
`world( (x-y)*tileWidth/2 , (x+y)*tileHeight/2 )`. `WorldToGrid` inverts it for mouse
picking. Draw order is depth: lower-on-screen tiles render in front. For sprites, set
`SpriteRenderer.sortingOrder = -(x + y)`; the starter bakes a small Z-offset instead so
it works with the demo cubes. For hand-built maps, use Unity's **2D Tilemap (Isometric)**
and align its cell size to `tileWidth/tileHeight`.

## Combat flow (one turn)

1. `TurnManager.StartCombat(units)` rolls initiative, sorts, fires `OnCombatStarted`.
2. `NextTurn()` advances to the next living unit, resets its budget
   (`SpeedTiles` move, 1 action, 1 bonus action), fires `OnTurnStarted`.
3. **Player turn:** `PlayerCombatInput` reads clicks → `Pathfinding.FindPath` → spend
   movement → `GridUnit.FollowPath`; or attack → `AttackResolver.Resolve/Apply`.
   Player ends turn (`Space`/button) → `NextTurn()`.
4. **Enemy turn:** `EncounterController` runs a coroutine: path adjacent to nearest
   player, attack, then `NextTurn()`. (Replace with a behavior tree / utility AI later.)
5. After each turn, win/loss is checked; `OnCombatEnded` fires.

## Where to extend (in rough priority)

- **Spell slots & resources:** add a `ResourcePool` to `CharacterSheet`; have
  `AbilityDefinition.spellSlotLevel` consume it in the resolvers.
- **Status effects/conditions:** an `EffectInstance` list on the sheet, ticked at
  turn start/end via `TurnManager.OnTurnStarted/Ended`.
- **Reactions/opportunity attacks:** hook `GridUnit.FollowPath` to test adjacency
  transitions against enemy `ActionCost.Reaction` abilities.
- **Equipment → AC/damage:** an `Equipment` component that overrides `baseArmorClass`
  and supplies the active weapon ability.
- **Real AI:** swap the `EncounterController` coroutine for a utility scorer (target
  selection, ability choice, positioning) — keep the same `TurnManager` API.
- **Fog of war / line of sight:** add a visibility pass over `GridSystem` cells.
- **Real UI:** replace `DebugCombatHUD` with a uGUI action bar, portraits, tooltips.

## Suggested assembly definitions (optional, speeds compile)

Split `Assets/Scripts` into asmdefs: `SunderedCrown.Core`, `.Combat`, `.Dialogue`, etc.
Add a `Tests` asmdef referencing them to unit-test `Dice`/`AttackResolver` headlessly.
```
