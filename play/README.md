# Crown of Horns — Headless Playable Slice

Proof that the combat engine works, runnable **without Unity**.

## Play it
- **`crown_combat.html`** — open in any browser. A tactical skirmish: Sister Garrow,
  Roen Alleywind, and Varra vs the Returned and their Last Returned boss. Each hero has an
  **ability bar** (attacks, a heal, a save-for-half AoE, Cleave) and the cast trades real
  conditions — **Slowed, Stunned, Frightened, Burning**. Click a glowing tile to move, pick an
  ability, click a target. Initiative, HP bars, condition pips, a combat chronicle, win/lose.

## The engine (faithful ports of the real C#)
- **`engine.js`** — 1:1 port of `Dice.cs`, `Abilities.cs`, `CharacterSheet.cs`, `StatusEffect.cs`,
  `AttackResolver.cs`, `CombatBalance.cs`. RNG is .NET `System.Random` reimplemented bit-for-bit,
  so seeded results match the Unity build seed-for-seed.

## Verification (run with `node <file>`)
| Script | Proves |
|---|---|
| `tests.js` | Ports your EditMode suite (DiceTests, AbilityScoresTests, AttackResolverTests, StatusEffectTests). **39/39 green.** |
| `abilities.js` | The demo's ability kit (heal, save-for-half, Slowed, Stun, Wail, Burning DoT) behaves per the engine. **6/6.** |
| `forecast.test.js` | Closed-form hit/crit/damage/kill preview + a Monte-Carlo cross-check that the forecast equals the real resolver. **8/8.** |
| `threat.test.js` | Incoming-damage + down-chance forecast (convolved distributions) vs a Monte-Carlo of the real resolver. **5/5.** |
| `verify.js` | Reproduces `CombatBalance.Report()` over seeds 0..399 — the engine's own balance oracle. |
| `diagnose.js` | Per-swing hit-rates match hand-computed 5e math (65% / 45%). |
| `autobattle.js` | Auto-resolves the base encounter 2000× — clean, terminating. |
| `smoke.js` | Boots `crown_combat.html` under a stubbed DOM and auto-plays 400 full games through the UI. **0 errors.** |
| `analyze.js` | Monte-Carlos the engine and writes **`balance_report.html`** — per-hero hit% / DPR vs armor class, time-to-kill, and the balance oracle. |
| `endings.test.js` | Ports `EndingResolverTests` — proves choices gate the endings (incl. the two golden roads) per your Unity tests. **7/7.** |
| `epilogue.test.js` | Mirrors the new `EpilogueTests` (gating, anchor, verdict), plus a **prose-parity gate** (every C# prose literal byte-identical in the port) and a 500-playthrough fuzz. **21/21.** |
| `items.test.js` | Ports `InventoryTests` + `ItemDatabaseTests` — stacking, removal, gold floor, change events, id registry. **14/14.** |
| `save.test.js` | Pins the SaveSystem serialization contract — GameFlags ⇄ flattened DTO ⇄ JSON round-trip, parallel-list lockstep, edge ints, re-save. **7/7.** |
| `quests.test.js` | Models the QuestManager state machine — explicit start, live flag-reactive objective/completion/failure advancement (PlayMode-only on the Unity side), and export/import. **15/15.** |
| `pathfind.test.js` | Ports the A* grid pathfinding + a brute-force Dijkstra oracle; a 3000-map fuzz proves every A* path is least-cost. **8/8.** |
| `progression.test.js` | Ports `ProgressionTests` — 5e XP table, single/multi level-ups, level-20 cap, level-up event. **9/9.** |
| `run-all.js` | One command — runs all gates + reports; exits non-zero on any failure (used by CI). |
| `retune.js` | Design-space search that derived the Brute retune (Str16/AC13/HP34) — reruns the seeded duels over candidate stat-lines and picks the one centred in both bands. |

## Finding — found *and fixed*
`CombatBalance`'s reference duel originally won **~95%**, flagging **HIGH** — outside its own
55–85% design band (the "Hero" out-classed the "Brute" on AC, HP, Str *and* weapon die at once).
Because the port is seed-faithful, we searched the Brute's stat-space headlessly and retuned it
(Str 14→16, HP 22→34): the canary now prints **Hero 70% [OK] · Duelist 53% [OK]** — and the Unity
build will print exactly those numbers.
