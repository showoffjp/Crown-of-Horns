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
- 🔜 **Surface breadth** — poison clouds, ice (prone), caustic brine, blood→slip; steam clouds
  (fire+water) that block sight
- ✅ **Height advantage** *(v3.89.0)* — striking from higher ground grants **advantage**; from below,
  **disadvantage**. Folded into the hit math, the forecast, and the threat readout; verified.
- 🔜 **Verticality (more)** — **shove off ledges**, jump/teleport, multi-level maps
- 🔜 **Throw** — improvised weapons, throw an enemy, throw a *bottle of oil* to seed a surface
- ✅ **Opportunity attacks + Disengage** *(v3.90.0)* — leaving a foe's reach without Disengaging
  provokes a free strike (once per round, both sides). Verified.
- 🔜 **More reactions** — shield/counterspell **prompts** (BG3's full reaction UI)
- 🔜 **Superiority twist** — *deeds-in-combat*: how you win a fight feeds the story flags

## 🧭 Pillar 2 — Quality of life (out-comfort BG3)

The layer players *feel* every minute. Mostly unbuilt — high-leverage.

- 🔜 **Highlight-all interactables** (hold Alt), **loot-all**, search
- 🔜 **Group select & move**, formation, follow
- ✅ **Undo move** *(v3.91.0)* — take back a move (with refunded movement) as long as it triggered
  nothing irreversible (no surface, no opportunity attack, you survived). Verified.
- 🔜 **Reaction & non-lethal toggles**, more **"undo"** before committing an action
- 🔜 **Tactical forecast everywhere** *(already in combat — extend to the whole UI)*
- 🔜 **Respec** at camp, **save-anywhere** *(browser save-inspector already round-trips saves ✅)*
- ⏳ Tactical camera / split-screen co-op — Unity-side

## 🧑‍🤝‍🧑 Pillar 3 — Characters & reactivity (out-react BG3)

Already competitive in depth; "superior" means **reactivity density**.

- ✅ **6 companions** — approval → night-talks → personal quests → **4 romances** → epilogues
- ✅ **34 fireside banters** reactive to choices, losses, bosses; rivalry/rupture arcs
- 🔜 **In-the-moment interjections** — companions speak up *inside* other scenes & combats
- 🔜 **Inter-companion conflict** — they argue with *each other*, not just you
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
