# Roadmap — from starter to shippable

A realistic milestone plan. The golden rule: **build the vertical slice before the
content.** Prove the whole machine on a tiny scope, then scale.

> Scope reality check: BG3 was ~400 people over ~6 years. Solo/tiny-team CRPGs that
> ship (e.g. *Knights of the Chalice*, *Vagrus*) succeed by *narrowing* — smaller party,
> tighter map count, focused systems. Plan to cut. The Story Bible is intentionally
> bigger than v1; ship the Prologue + Act I first.

## Milestone 0 — You are here ✅
- Engine starter: grid + iso projection, A* pathing, turn-based combat, 5e stats,
  dialogue/quest/flag/save systems, one-click playable demo, full story bible.

## Milestone 1 — Vertical slice (the proof) — *~1–3 months solo*
Goal: one polished hour that contains every system the final game uses.
- [ ] Replace demo cubes with **sprite-based GridUnits** + idle/walk/attack anims.
- [ ] Build **one hub map** (a slice of Saltmarch) on an isometric Tilemap.
- [ ] Build **one dungeon** (the Tidewrack) with a scripted encounter.
- [ ] Real **uGUI HUD**: portraits, action bar, health, turn order, combat log.
- [ ] **Character creation** screen (race/class/abilities/background) writing a sheet.
- [ ] **3–4 dialogue graphs** incl. Corin's intro; one **companion recruit** (Sable).
- [ ] **2 quests** (1 main, 1 side) end-to-end with journal UI.
- [ ] Save/load from a menu (slots), main menu, pause menu.
- [ ] **Ship this to ~5 testers.** This milestone decides if the game is fun.

## Milestone 2 — Act I content — *~3–6 months*
- [ ] 1 reactive hub + 3 dungeons; ~8 enemy archetypes; basic loot tables.
- [ ] Spell slots/resources, status effects, opportunity attacks, equipment→AC/damage.
- [ ] 3–4 companions with recruit convos + banter + approval reactivity.
- [ ] Utility-AI for enemies (targeting, ability choice, positioning, fleeing).
- [ ] ~10 quests; faction reputation surfaced in UI; first real ending branch flagged.
- [ ] Audio pass: music states (explore/combat/dialogue), SFX, ambience.

## Milestone 3 — Systems depth
- [ ] Full class/spell roster; multiclass if desired; level-up UI.
- [ ] Inventory/equipment UI with drag-drop; crafting/merchants/economy.
- [ ] Stealth, traps, locks, skill checks in the world (not just dialogue).
- [ ] Cinematics/camera scripting for set-pieces; cutscene system.
- [ ] Companion personal-quest framework (the Act II detonations).

## Milestone 4 — Full narrative (Acts II–III)
- [ ] Remaining hubs/dungeons; the Tenth Seat finale gauntlet.
- [ ] All 7 companions complete; the secret "Ninth" companion.
- [ ] The five endings + per-companion/faction epilogue slides keyed to flags.
- [ ] The flag/reputation matrix fully wired so endings truly diverge.

## Milestone 5 — Production / ship
- [ ] Balance pass (seed `Dice` for repeatable combat tests; encounter budgets).
- [ ] Localization pass (externalize all strings early — do this from M1!).
- [ ] Accessibility: text size, colorblind-safe states, remappable input, combat speed.
- [ ] Performance: object pooling for VFX/projectiles; A* heap; chunked maps.
- [ ] QA, save-migration safety, Steam/itch build pipeline, marketing page.

## Team & tools (when you outgrow solo)
- **Roles you'll want:** a writer/narrative designer (huge for a CRPG), a 2D artist,
  a systems programmer, a composer (contract). Companions are the #1 content cost.
- **Asset Store / middleware** worth evaluating: A* Pathfinding Project (Aron Granberg)
  if you need heavier pathing; Ink/Yarn Spinner for dialogue; FMOD/Wwise for audio;
  Odin Inspector for nicer data-asset editing.

## Anti-scope-creep rules
1. No new system without a cut elsewhere until M1 ships.
2. Every feature must serve the Prologue+Act I slice or it waits.
3. Reactivity > breadth: a small world that *remembers* beats a big one that doesn't.
4. Externalize strings and keep the flag spreadsheet from day one — retrofitting hurts.
```
