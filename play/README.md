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
| `verify.js` | Reproduces `CombatBalance.Report()` over seeds 0..399 — the engine's own balance oracle. |
| `diagnose.js` | Per-swing hit-rates match hand-computed 5e math (65% / 45%). |
| `autobattle.js` | Auto-resolves the base encounter 2000× — clean, terminating. |
| `smoke.js` | Boots `crown_combat.html` under a stubbed DOM and auto-plays 400 full games through the UI. **0 errors.** |
| `analyze.js` | Monte-Carlos the engine and writes **`balance_report.html`** — per-hero hit% / DPR vs armor class, time-to-kill, and the balance oracle. |
| `run-all.js` | One command — runs all gates + reports; exits non-zero on any failure (used by CI). |

## Finding
`CombatBalance`'s reference duel (Hero vs Brute) wins **~95%**, flagging **HIGH** — outside its own
55–85% design band. Port hit-rates are exact vs 5e math, so it's a real observation about the
reference sheets (the "Hero" out-classes the "Brute" on AC, HP, Str *and* weapon die at once),
not a port bug.
