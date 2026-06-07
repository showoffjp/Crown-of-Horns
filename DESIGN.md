# THE CROWN OF HORNS — Design Vision & Pillars

> *Planescape: Torment asked, "What can change the nature of a man?"*
> ***The Crown of Horns** asks: "What can change the nature of a god's justice?" — and makes you pay for the answer.*

This is the north star. Every system, scene, line, and asset should be able to point
back to something on this page. If it can't, question whether it belongs.

---

## 1. The core question

In the Forgotten Realms, souls who held to no god — or were **false** to the one they
swore — are cemented into the **Wall of the Faithless** in the Fugue Plane, and slowly
dissolve into nothing. It is canon. It is one of fantasy's most quietly monstrous ideas:
an eternity of erasure, administered as *bureaucracy*.

The whole game is one question turned in the light:

> **Is damnation ever just — and what would you give to unmake an injustice woven into
> the fabric of reality itself?**

You don't read about the Wall. You are walked to the scene of every crime that built it
— Netheril's hubris, the Crown Wars' first damnation, the forging of the Crown from
Myrkul's skull, the Spellplague's unmaking — and then, at the Court of the Dead, you
decide. The game has been collecting evidence the whole time. So have you.

---

## 2. Design pillars

These five are the load-bearing walls. Trade-offs get resolved in their favor.

### I. The Wall is the whole game
Every era, every fight, every companion is a different **angle on one question**: who
gets damned, who decides, and what it costs to refuse. A feature that doesn't illuminate
the Wall is decoration. (✅ Already true: the eras, the Verdict, the Breach, the endings.)

### II. Consequence is permanent and *authored* — never random
**The Breach** takes a companion *forever*, and **who** is chosen by an authored-fate
system that reads how you cared earlier — not a dice roll. Endings are **earned** from
the whole playthrough (`EndingResolver`). The game *remembers*, and it never lets you
save-scum your morality. Permanence is the point: the Wall is forever, so your choices
rhyme with it. (✅ `Party.Remove`, authored fate, six earned endings.)

### III. One brain, total reactivity
**`GameFlags` is the nervous system.** Dialogue, reputation tiers, companion approval,
keepsakes, deeds, endings — all flow through one flag store. The world feels alive
because it *is* one organism reacting, not a hundred disconnected scripts. New reactivity
should plug into the brain, not grow a second one. (✅ Already the architecture.)

### IV. 5e you can trust
The tactical layer is **honest, legible D&D 5e** — flanking, opportunity attacks, action
economy, save-for-half, advantage that cancels per the rules. Combat earns the narrative
weight because the player can *trust the math* and play to it. No fudged dice, no hidden
rubber-banding (difficulty scales transparently at one chokepoint). (✅ `AbilityRunner`.)

### V. Witness, then decide
Time-travel is **not** a gimmick or a power fantasy. It is a subpoena. You are brought to
each era so that your final verdict is the verdict of someone who *was there*. Every era
module must end by making the central question heavier, not lighter. (✅ Era-module
architecture via `SimpleEra`.)

---

## 3. The structural spine

```
                        ┌─────────────────────────┐
   Baldur's Gate Hub ──▶│  flag-gated era rifts   │──▶ Court of the Dead (finale)
   (Lower City slice)   │  Netheril · Crown Wars  │     EndingResolver reads the
        ▲   │           │  Time of Troubles ·     │     whole playthrough →
        │   ▼           │  Spellplague            │     the endings you EARNED
   explore ⇄ combat     └─────────────────────────┘
   (one director swaps modes)        each: explore → moral weight → optional boss → Codex
```

Everything is built on **reusable modules** (`EncounterBuilder`, `SimpleEra`,
`DialogueGraph`, `QuestManager`) so new content is *authored*, not *engineered*. That's
the multiplier: the engine is done enough that we now pour in **world**.

---

## 4. The bid for "greatest of all time"

Great CRPGs are remembered for **one feeling they alone gave you.** Ours is the weight of
**informed complicity** — the game makes you a witness so that you cannot pretend the
final choice was made in ignorance. To hit that, three things must stay true:

1. **Theme before content before systems.** Systems are done. Now every hour of content
   must earn its place against the core question. Quantity that dilutes the throughline
   makes the game *worse*.
2. **The player must feel the Wall before they judge it.** Art, audio, and writing exist
   to make erasure *felt*, not described. (See §5.)
3. **No choice the game forgets.** If a decision doesn't ripple into a flag, a keepsake,
   a reactive line, or an ending slide — it's not a choice yet.

---

## 5. Art & audio direction (so the asset library serves the vision)

This is what the `Assets/` pipeline (see `Assets/ASSETS.md`) should aim *at*:

- **Mood: sacred dread.** Beauty that is also a little wrong. Gold leaf over rot.
  Cathedral light in a place that damns people. Every era is gorgeous *and* indicts itself.
- **Palette per era** (a tool for legibility and theme):
  - **Netheril** — gold & sky-blue, gilded hubris, light *falling*.
  - **Crown Wars** — silver & elven green going cold; the first frost of judgment.
  - **Time of Troubles** — bone-white & blood, gods bleeding into the mortal world.
  - **Spellplague** — that signature **blue fire**, reality skipping, wrong-colored shadow.
  - **The Fugue / the Wall** — grey desaturation, faces in the stone, the absence of color *as* the horror.
- **Companions read silhouette-first.** Each must be recognizable as a black shape — they
  carry the emotional load of the permanent-loss system.
- **Audio sells permanence.** The Breach, a Verdict, an ending — these need a *sound* the
  player will flinch at later. Sparse, not bombastic.

When you push art, tag it to an era/mood and run `tools/generate-asset-manifest.sh` so we
can see at a glance whether the library is serving all five eras or starving some.

---

## 6. What "keep going" means from here

The engine is a vertical slice that *works*. The frontier is now:

1. **World density** — more authored content per era that turns the screw on the question.
2. **The art & audio that make the Wall *felt*** — via the asset pipeline we just built.
3. **Tighten the throughline** — audit every existing scene against §1 and cut what dilutes.

> *The Wall is forever. So is a great game's hold on the people who played it.*
> *Let's make this one of those.*
