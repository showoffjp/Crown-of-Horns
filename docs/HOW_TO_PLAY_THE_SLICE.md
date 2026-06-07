# 🎮 How to Play the Vertical Slice

*A 20-minute guided run through **The Crown of Horns**, from the title screen to the Court of the Dead.*

This is the player-facing companion to [`GETTING_STARTED.md`](../GETTING_STARTED.md) (which is about
*installing* the project). Here we assume the project is open in Unity and you just want to **play the loop end to end** and see everything the slice has to offer.

---

## ▶️ Boot it

1. New empty scene → empty GameObject → **Add Component → `MainMenu`**.
2. Press **Play**. You'll get a title card with **New Game / Continue / Quit**.
3. Hit **New Game** to roll a hero (point-buy or standard array), pick class / ancestry / background, and drop into Baldur's Gate.

> 💾 **Continue** rebuilds your hero *and* your roster from the autosave — including any
> **permanent losses**. The Breach is forever, even across a reload.

---

## 🗺️ The loop, step by step

| # | Beat | What to do |
|---|------|-----------|
| 1 | **Baldur's Gate (hub)** | Click tiles to **move**. Walk up to the glowing markers and press **`E`** to interact. |
| 2 | **Meet the Herald** | He points you at the sealed stair and at Aldric. |
| 3 | **Recruit** | Talk to **Roen Alleywind** (Harper rogue) and **Varra** (tiefling warlock). Both are optional but make the fights easier. |
| 4 | **Loot the Strongbox** | Grab the healing potion and the **Cinderhaunt key**. |
| 5 | **Cinderhaunt Stairs** | The dungeon. Win the boss fight to set `prologue.cleared`. |
| 6 | **Tea with a Heretic** | Back in the hub, **Aldric Morn** now waits — the first mask. |
| 7 | **The Skip** | A shimmering rift to **Netheril**. Survive the **falling-city battle** (the floor collapses each turn). This sets `netheril.cleared` — and **opens the Court of the Dead**. |
| 8 | **Walk the eras** | Each cleared era opens the next rift: **Crown Wars → Time of Troubles → Spellplague.** |
| 9 | **The Vault of Tens** | Talk to **Tally**, the one-eyed storyteller at the fire. Solve her riddles — **wit beats correctness.** |
| 10 | **Make camp** | Take the **Campfire** marker to **long rest** (full HP + spell slots) and sit for an **approval-gated night-talk** with whoever trusts you most. |
| 11 | **The Court of the Dead** | When you're ready, take the finale marker and choose your ending. |

> 🔥 **Camp tip:** night-talks unlock per companion once their **approval** clears a threshold *and*
> they're **fielded** (`P`). Field the people you've been kind to, then rest — the fire loosens tongues.

---

## ⌨️ Controls

| Key | Action |
|-----|--------|
| **Click tile** | Move (hub) / move-or-target (combat) |
| **`1`–`9`** | Arm an ability / spell |
| **`Space`** | End turn |
| **`E`** | Interact with the marker you're standing by |
| **`I`** | Inventory |
| **`J`** | 📜 **Journey** — the quest tracker (acts, eras, endings unlocked) |
| **`P`** | 🛡️ **Party** — bench/field your companions (the leader always walks) |
| **`F5` / `F9`** | Quick save / load |
| **Mouse wheel** | Zoom · **WASD / screen edges** pan |

---

## 🧭 Reading the Journey screen (`J`)

The Journey tracker turns the whole saga into a legible checklist:

- **✔ / ○** for each beat — the Cinderhaunt, the four eras, the Breach, the Vault.
- **Endings unlocked: N/6** — how many of the six endings you've earned the right to choose.
- **★ a golden road is open** — appears when you've unlocked one of the two *golden* endings
  (**Jergal's Keyhole** needs the Reader's Boon *and* a spared Crown Wars verdict; **Break the Loop**
  needs the Reader's Boon *or* your true name).

If the Court of the Dead "isn't there yet," the Journey screen tells you why: **walk at least one era.**

---

## 🛡️ Managing the party (`P`)

You can recruit more companions than you can field. Press **`P`** to bench or field anyone in your
roster up to **maxActive** (default 4). The **protagonist is locked into the field** — you can't leave
yourself behind. Benched companions keep their levels and gear; field them before a fight.

---

## ⚰️ The permanent cost

Once you've cleared the Cinderhaunt, the **Descend into the Fugue** marker opens the **Breach**: pull
**Maerin** from the Wall of the Faithless, and **lose a companion forever** — chosen by who you've
grown closest to, so the loss is authored by your own earlier care. It is not arbitrary, and it does
not come back on reload. The Journey screen will mark it done.

---

## 🌅 The six endings

The **Court of the Dead** reads your whole playthrough out of `GameFlags` and offers whichever endings
you've unlocked. To preview *all six* in prose (without playing to them), drop **`EndingDemo`** on an
object and press Play. The two golden roads — **Jergal's Keyhole** and **Break the Loop** — are the
deepest, and the Journey screen flags when one is open.

---

*Roll for initiative.* 🎲
