# Crown of Horns — Headless Playable Slice

Proof that the combat engine works, runnable **without Unity**.

- **`crown_combat.html`** — open in any browser. A tactical skirmish (Sister Garrow,
  Roen Alleywind, Varra vs the Returned) driven by the game's *real* rules: d20 + ability
  mod + proficiency vs Armor Class, nat-20 crits (double dice), nat-1 auto-miss, initiative
  by d20 + Dex. Click a glowing tile to move, click an enemy in reach to strike.

- **`engine.js`** — a 1:1 JS port of `Dice.cs`, `Abilities.cs`, `CharacterSheet.cs`,
  `AttackResolver.cs`, `CombatBalance.cs`. RNG is .NET `System.Random` reimplemented
  bit-for-bit, so seeded results match the Unity build seed-for-seed.

- **`verify.js`** — reproduces `CombatBalance.Report()` over seeds 0..399 (the engine's
  own balance oracle). `node verify.js`
- **`diagnose.js`** — confirms per-swing hit-rates match hand-computed 5e math. `node diagnose.js`
- **`autobattle.js`** — auto-resolves the demo encounter 2000× to prove it runs clean. `node autobattle.js`

## Finding
`CombatBalance`'s reference duel (Hero vs Brute) wins **~95%**, flagging **HIGH** — outside
its own 55–85% design band. The port's hit-rates are dead-on the 5e math, so this is a real
observation about the reference sheets (the "Hero" out-classes the "Brute" on AC, HP, Str,
*and* weapon die at once), not a port bug.
