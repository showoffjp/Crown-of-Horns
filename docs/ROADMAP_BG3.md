<div align="center">

# 🎯 ROADMAP — "Baldur's Gate 3, but superior"

*Where Crown of Horns chooses to fight a 9-figure AAA game — and win.*

</div>

---

## The honest framing

We will **not** beat BG3 on 3D fidelity, photoreal cinematics, mocap, or full voice. That's
budget, not design. The browser slice is Infinity-Engine-era 2D and proud of it.

We compete — and aim to be **superior** — on the four axes where *design and writing beat
budget*: **Quality of Life · Plot · Characters · Gameplay mechanics.** Every milestone below is
built as a real, **headless-test-verified** system and shown live in the isometric combat demo
where it applies. ✅ = shipped · 🔜 = next · ⏳ = needs Unity/art/budget you provide.

---

## ⚔️ Pillar 1 — Gameplay mechanics (out-tactic BG3)

BG3's combat is its crown jewel; this is our biggest open frontier and the most demonstrable.

- ✅ **5e core** — d20 vs AC, crits, saves, conditions, spell slots, save-for-half AoE
- ✅ **Positioning** — flanking, half-cover, opportunity attacks, reach
- ✅ **Environmental surfaces** *(v3.86.0)* — lay **oil**, **ignite** it, fire **spreads** through
  grease and burns anyone standing in it, **water douses** it. Real combos, verified.
- ✅ **Shove** *(v3.87.0)* — a Strength contest that pushes a foe a tile: **into a hazard surface**
  (the BG3 environmental kill) or **into a wall** for impact damage. Verified in all 8 directions.
- ✅ **Electrified water** *(v3.88.0)* — conjure **water**, then a **Storm Bolt** arcs through it and
  zaps everyone standing in it (lightning **chains through water**). Verified.
- ✅ **Surface breadth** *(v3.94.0)* — all five elemental surfaces are now player-creatable: oil/grease,
  fire, water, **poison cloud** (Roen's Venom Vial) and **ice** (Garrow's Frost, which slips). Verified.
- ✅ **Steam** *(v3.96.0)* — fire + water makes a **steam cloud** that grants whoever stands in it
  **cover (+2 AC)**. The last surface combo. Verified.
- ✅ **Jump** *(v4.1.0)* — leap to a tile in range (3), vaulting walls/units, provoking no opportunity
  attack. Verified.
- ✅ **Arcanist spells** *(v4.2.0, BG2 wizardry)* — a playable mage (Naeve) brings three new ability kinds:
  **Magic Missile** (`autoHit`, never misses), **Fireball** (`enemyburst`, blasts every foe in radius and
  seeds a fire surface), and **Mirror Image** (`selfbuff`, +4 AC). Burst targeting + never-miss math verified.
- 🔜 **More verticality** — multi-level maps, teleport; caustic brine, blood→slip
- ✅ **Height advantage** *(v3.89.0)* — striking from higher ground grants **advantage**; from below,
  **disadvantage**. Folded into the hit math, the forecast, and the threat readout; verified.
- ✅ **Shove off ledges** *(v4.4.0)* — winning a shove contest while a foe stands on the high-ground
  mound pushes them **off** it: they **fall** (2d6 per level dropped) and land **Prone** (attackers gain
  advantage). Braids Height + Shove into BG3's signature environmental kill. Verified.
- 🔜 **Verticality (more)** — jump/teleport, multi-level maps
- ✅ **Throw** *(v4.3.0)* — lob **Alchemist's Fire** onto **any tile in range** (empty ground included,
  vaulting walls/units): it bursts on every foe in the blast (save for half) **and seeds a fire surface**.
  The first ability that can **pre-place a hazard** where no foe yet stands. Range + wiring verified.
- ✅ **Opportunity attacks + Disengage** *(v3.90.0)* — leaving a foe's reach without Disengaging
  provokes a free strike (once per round, both sides). Verified.
- ✅ **Shield reaction** *(v4.5.0)* — Naeve spends her reaction to gain **+5 AC** against an attack that
  would *just* land, turning it aside; once per round, shared with opportunity attacks, toggle-gated. The
  first defensive reaction. Verified.
- ✅ **Counterspell** *(v4.6.0)* — Naeve spends a **once-per-battle** reaction to **unravel an enemy spell**
  (the boss's Wail) before it resolves — it fizzles entirely. The offensive half of the reaction system,
  sharing one economy with Shield + opportunity attacks. Verified.
- 🔜 **More reactions** — a reaction **prompt UI** (choose per-trigger, BG3's full reaction system)
- ✅ **Superiority twist — deeds-in-combat** *(v4.7.0, Pillar 1 × 4)* — *how* you win a fight is appraised
  (flawless/hard-won, merciful/ruthless, a ledge kill, the boss's spell countered) and written back as
  **story flags** (`coh.combat.deeds`), the reverse of the flag bridge combat already reads. Verified.

## 🧭 Pillar 2 — Quality of life (out-comfort BG3)

The layer players *feel* every minute. Mostly unbuilt — high-leverage.

- 🔜 **Highlight-all interactables** (hold Alt), **loot-all**, search
- 🔜 **Group select & move**, formation, follow
- ✅ **Undo move** *(v3.91.0)* — take back a move (with refunded movement) as long as it triggered
  nothing irreversible (no surface, no opportunity attack, you survived). Verified.
- ✅ **Hotkeys + Reaction toggle** *(v3.92.0)* — keyboard shortcuts for every action (1-9 / V / X / U /
  Enter), and a toggle for whether your heroes take opportunity attacks. Verified.
- ✅ **Non-lethal toggle** *(v3.97.0)* — knock foes out & **spare** them instead of slaying; the run
  tallies how many you spared (a narrative axis the story can read). Verified.
- 🔜 group select & move, loot-all, highlight interactables
- ✅ **In-combat Tactics help** *(v4.0.0)* — a ❔ panel (and <kbd>?</kbd> hotkey) documenting every
  system, so the depth is discoverable. Verified.
- 🔜 **Tactical forecast everywhere** *(already in combat — extend to the whole UI)*
- 🔜 **Respec** at camp, **save-anywhere** *(browser save-inspector already round-trips saves ✅)*
- ⏳ Tactical camera / split-screen co-op — Unity-side

## 🧑‍🤝‍🧑 Pillar 3 — Characters & reactivity (out-react BG3)

Already competitive in depth; "superior" means **reactivity density**.

- ✅ **6 companions** — approval → night-talks → personal quests → **4 romances** → epilogues
- ✅ **34 fireside banters** reactive to choices, losses, bosses; rivalry/rupture arcs
- ✅ **Combat barks** *(v3.93.0)* — companions react in-voice to crits, kills, an ally going down,
  a foe igniting, a wall-slam, and victory (present-aware, rate-limited). Verified.
- ✅ **Story-flag-reactive barks** *(v3.98.0, Pillar 3 × 4)* — the line *changes with your run's flags*:
  a romance-aware crit, a quest-resolved kill, an NG+ wink. Fed from the run's `GameFlags`. Verified.
- ✅ **Save → Combat handoff** *(v3.99.0)* — the Save Inspector stages a run's flags straight into the
  Combat tab, so loading a save *visibly* changes what the companions say mid-fight. Verified.
- ✅ **Combat → Save return leg** *(v4.9.0)* — the Save Inspector's new **Combat Chronicle** panel reads back
  the deeds combat writes (`coh.combat.deeds`), so the Pillar 1 ↔ 4 loop is whole and visible end-to-end.
  A cross-page test pins the deed labels to combat's deed set so they can't drift. Verified.
- ✅ **Inter-companion banter** *(v3.95.0)* — the party bickers with *each other* mid-fight (two-line
  exchanges, present-aware, rate-limited). Verified.
- ✅ **Deed-reactive victory barks** *(v4.8.0, Pillar 1 × 3)* — when the fight is won, the present companion
  reacts in-voice to *how* you won (a Counterspell, a ledge kill, a merciful or ruthless finish), most-
  distinctive deed first, presence-gated. Pairs with v4.7.0's deed ledger. Verified.
- 🔜 **In-the-moment interjections** — companions speak up *inside* other scenes & combats
- 🔜 **Finer approval** — silent disapproval, thresholds, "they remember that"
- 🔜 **More romances/rivalries** + the BG3-style *"approval is not consent"* nuance

## 📜 Pillar 4 — Plot & consequence (already our moat)

A genuinely novel premise (the **Wall of the Faithless**), 4 masks, 6 endings, time-travel eras,
NG+ memory — arguably **already superior in concept**. "Superior" = wider consequence webs.

- ✅ **167-flag reactivity graph**, six branching endings, personalized epilogues, NG+ memory
- ✅ Browsable: Dialogue-tree viewer, Endings explorer, Flag-dependency graph, Saga map
- 🔜 **Faction-outcome webs** — choices in one act visibly reshape later hubs
- 🔜 **Deeper NG+** — the Lady *remembers specific* prior-run choices
- 🔜 **Consequence telegraphing** — the QoL forecast, applied to *narrative* stakes

---

## How we ship it

Same cadence as everything else: one pillar increment per release → headless tests gate it →
release notes → squash-merge → the browser demo shows it. The
[`play/`](../play) suite stays the living proof; `node play/run-all.js` stays green.

> The dream isn't "clone BG3." It's: *the tactics are deeper, the comforts are kinder, the
> companions argue back, and the story is about something.* That's a game worth finishing.
