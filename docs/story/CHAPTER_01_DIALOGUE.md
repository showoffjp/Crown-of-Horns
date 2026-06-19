<div align="center">

# 📖 Chapter 1 — *"The Gilded Gate"*
## Full Dialogue Synopsis & Reactivity Guide

*Prologue cold-open · Act I main spine · Act I reactive aftermath*

</div>

---

> **What this is.** A complete map of every authored conversation in Chapter 1 — speakers, branches,
> skill checks (ability + DC), the flags each path writes, and the consequences that thread forward.
> All of it is authored in the `.dlg` DSL (`play/dialogue/{prologue,act_one,act_one_deep}.dlg`),
> compiled to C# `Content` builders by `tools/dlg-compile.py`, and live in the **Dialogue Viewer**
> (`play/dialogue_viewer.html`, Map + Walk modes).

**Register.** A *Baldur's Gate* political thriller with a death-cult underneath. Three powers — the
Heretic **Aldric**, the church (**Doomguide Vayle**), and the **Harpers** — all want the walking-dead
miracle that just climbed out of the Cinderhaunt. The chapter assembles your party, then **collapses the
easy moral frame** so completely that by the last scene you no longer know who the villain is.

**Chapter 1 at a glance**

| | Count |
|---|---|
| Conversations | **13** (3 Prologue · 5 Act I spine · 5 Act I aftermath) |
| Nodes | ~99 |
| Skill checks | Persuade, Intimidate, Insight, Arcana, History (DC 12–17) |
| Companions gained | **Roen** (rogue), **Varra** (tiefling warlock) — beside **Sister Garrow** |
| Set-piece | **The Burning of Hollowmere** |
| Chapter twist | the world **skips** — three heartbeats in a sky-city ten thousand years gone |

**Reading the flag notation.** `SET x` writes a bool · `appr.<id> ±N` shifts a companion's approval ·
`rep.<faction> ±N` shifts reputation · `[SKILL DC]` is a d20 + ability check, with a **fail branch**.

---

## 🕯️ Cold open — the Prologue (*"What the Dead Owe"*)

Tutorial by trauma: you die, read your own half-formed name in the **Wall of the Faithless**, are pulled
back, and cut your way out.

| Conversation | Beat | Key branches / flags |
|---|---|---|
| **The Reservation** `prologue.reservation` | The Wall *itself* speaks as you read your niche. | `[ARCANA 12]` "It's a filing system" → `prologue.wall_insight`; "I don't belong here" → `prologue.crack_seeded` (the Wall notices yours is *the one that didn't take*). |
| **Returned** `prologue.waking` | **Sister Garrow** — forty years a grave-priest — watches a corpse sit up, Kelemvor's seal torn *from the inside*. | `[INSIGHT 13]` "You're afraid you've been lied to for forty years" → `garrow_doubt`, `appr.garrow +8`. Your first companion, won by honesty or defiance. |
| **They Never Come Back** `prologue.choir_herald` | A panicking Choir herald names the man who's prayed twenty years to find you. | All roads → `aldric.name_known`; `[PERSUADE 14]` to be escorted (`appr.garrow −2` — she distrusts the Choir). |

> **Highlight — the Wall, on what you are**
> *"…A cold way to put it. A true one. We are the souls no ledger wanted. **Filed under nothing.** You
> read quickly, for the newly dead. You will need that."*

> **Highlight — Garrow, refusing comfort over truth**
> *"If you are real — and you are **insultingly warm for a corpse** — then someone has been wrong for a
> very long time, and I have helped them be wrong. I intend to make that right. Starting with getting
> you out of here. Move."*

---

## ⚔️ Act I — the main spine

### 1 · *"The Gilded Gate"* `act1.gate.arrival`
A Flaming Fist sergeant stops a thing he "hasn't got a form for." **How do you exist in a city that files everyone?**

| Your line | Check | → | Consequence |
|---|---|---|---|
| "I'm a witness to a cult harvesting souls under your city." | **Persuade 15** | uneasy ally | `act1.fist_ally`, `rep.fist +5` |
| "The form would frighten your Marshal. Walk away." | **Intimidate 14** | he backs off you | `act1.fist_wary`, `rep.fist −3` |
| Coin | — | buys an afternoon | `rep.fist −2` |
| "Try the cell. I'll wait." | — | the Guild has ears in every cell | `act1.owes_guild` |

### 2 · *"The Fist and the Knife"* — recruiting **Roen** `act1.roen.recruit`
The thief the Harpers hired to confirm you're real… who'd rather not file the report. **Can you trust the
man sent to find you?** The `[INSIGHT 14]` line cracks him open early (→ the Wrenna reveal, `roen.sister_hinted`,
seeding his personal quest). Routes to `companion.roen.recruited` or a token at the Low Lantern.

### 3 · *"A Devil's Arithmetic"* — recruiting **Varra** `act1.varra.recruit`
A warlock with a soul-debt signed at age six. `[ARCANA 15]` exposes her real reason for wanting you near
(`varra.crack_drawn`). Sets `varra.quill_known` (her collector → her quest) or `companion.varra.recruited`.

### 4 · *"The Burning of Hollowmere"* — **the set-piece** `act1.hollowmere`
**The chapter's spine snaps here.** Doomguide Vayle burns a village "to stop the spread," arithmetic already done.

| Your line | Check | → | Consequence |
|---|---|---|---|
| "There's a third path — end the harvest at the source." | **Persuade 17** | she dares to hope | `act1.vayle_third_path`, `rep.kelemvor +3` |
| "I'll save the ones you won't." | — | she calls off the east lane | `act1.hollowmere_saved_some`, `appr.garrow +6` |
| "You're not certain. You're just too far in to stop." | **Insight 16** | she admits it | `act1.vayle_doubt_seen` |
| "This is monstrous." | — | she agrees — *"It gets easier."* | `act1.named_vayle_monster`, `rep.choir −10` |

Failed checks → `zealot` (she armors over). Every branch lands on Garrow's gut-punch (below).

### 5 · *"Three Heartbeats"* — the world-skip `act1.skip`
You touch the Crown's psychic wake and the world **skips** — a sky-city, ten thousand years gone, a voice
you don't yet know (Naeve). `[ARCANA 16]` reframes it: *this is a memory — yours — and it hasn't happened
yet* (`crown.memory_runs_backward`). Points the party at **Candlekeep**; closes the chapter.

---

## 🏙️ Act I — reactive aftermath (*"The City Remembers"*)

These five conversations **read the flags above** and answer them. The Lower City becomes a place that
knows what you did.

| Conversation | Reacts to | What changes |
|---|---|---|
| **The Count** `act1.aldric.aftermath` | `vayle_third_path` · `hollowmere_saved_some` · `named_vayle_monster` | Return to Aldric; "Stop the harvest" / "Why keep a count?" can **crack his self-justification** (`aldric.crown_doubt_planted`) — the first fracture in the man who'll wear the Crown. |
| **The Doomguide's Question** `act1.vayle.return` | `vayle_third_path` · `hollowmere_saved_some` · `named_vayle_monster` | Vayle seeks you out, out of armor. She **turns** (`vayle_turns`, `rep.kelemvor +5`), doubts, or asks to learn one survivor's *name* — the mercy-as-discipline lesson she once taught Garrow, returning. |
| **A Name From Hollowmere** `act1.hub.widow` | `hollowmere_saved_some` | **Sable & her son Wick**, alive only if you saved them. Give her your attention, not a coin → `act1.mercy_remembered`. Names, not weight. |
| **The Guild's Interest** `act1.hub.fence` | `companion.roen.recruited` · `act1.owes_guild` | A fence prices you by your credit rating — Roen is literally your *credit rating* (`act1.guild_ally`); the cell-debt becomes a shield (`act1.guild_debt_is_shield`). |
| **The Choir on the Corner** `act1.hub.preacher` | `faction.choir` · `aldric.named_monster` | A street-preacher recruiting grief. **Nuance**, refuse to be a symbol, **expose** the harvest's hand in their wells (`Persuade 16` → `crowd_disillusioned`), or **endorse** the Choir (`rep.choir +8`). |

---

## 🎭 The best interactions — in full

### ⚖️ Hollowmere — *the third path* (the moral collapse)
> **Vayle:** *"I can burn ten acres tonight or bury ten thousand by spring."* *(A child screams somewhere
> in the smoke.)* *"You think I don't hear that. I hear all of them. I have simply learned to finish the
> sentence anyway."*
>
> **▶ [PERSUADE 17]** *"There's a third path. Help me end Aldric's harvest at the source, and there's no
> spread to burn."*
>
> **Vayle:** *(the calm cracks — not into doubt, but into a terrible hope she won't let herself hold)*
> *"…At the source. You think it can be stopped at the source. If you are wrong, this village dies for
> nothing and the next one burns anyway. If you are **right** — then I have burned a great many sentences
> I could have left unfinished. Save who you can tonight. Bring me the source. And pray you're not another
> comfortable lie."*

### 🗡️ Roen — *the Wrenna read*
> **▶ [INSIGHT 14]** *"You're not tired of the Harpers. You're running from one of them."*
>
> **Roen:** *(the smile doesn't move, but everything behind it does)* *"…That's a nasty trick, reading a
> man mid-pitch. Her name's Wrenna. She's a Harper, and she's the reason I can't go home, and that's all
> you get for one cup of someone else's wine. But yes. I'd rather be three planes away from the people who
> own her leash. Take me with you and I'll owe you a true thing later. I'm stingy with those."*

### 🔥 Varra — *the crack read*
> **▶ [ARCANA 15]** *"You don't want my trick. You want to be near the crack when it widens."*
>
> **Varra:** *(for half a heartbeat the grin is gone, and underneath it is something very tired and very
> sharp)* *"…Clever. Yes. Fine — both things are true. I want the trick **and** I want to stand close to
> the one place in creation where a signed debt didn't hold. You'd want the same, in my seat. Don't
> pretend you wouldn't."*

### 🕯️ Aldric — *the count, cracked* (reactive aftermath)
> **▶** *"Stop the harvest, Aldric. Today. Or the count is just a way of forgiving yourself in advance."*
>
> **Aldric:** *(the cup stops halfway to the table, and tea slops over the rim — the first ungraceful
> thing you have seen him do)* *"That is the cruelest true thing anyone has said to me in twenty years…
> I cannot stop the harvest today. The Wall does not pause for my conscience. But you have put a crack in
> the count — the count was the last wall I had left between me and what I am doing — and I find I cannot
> quite seal it back. Stay near me. I think I am going to need a witness who isn't afraid to say that."*

### 🌫️ Sable — *a name from Hollowmere* (reactive aftermath)
> **▶** *"Just your name. And the boy's. So that two more of you are remembered as people, not weight."*
>
> **Sable:** *(her breath catches)* *"…Sable. And this is Wick — he's four, he likes frogs and hates
> turnips and he is the entire reason I let you pull me through that lane. No one has asked our names
> since the fire. They ask how many died. They never ask who lived, or what we were called. You walked
> out of the place where they turn names into nothing. I think that's why you're the only one who thought
> to turn it back the other way."*

### 🕯️ Garrow — *the line the whole chapter is built to earn*
> **Garrow:** *"I trained under her, you know. Vayle. She taught me that mercy is a discipline. I do not
> think she remembers the lesson she taught. Or perhaps she remembers it too well. That is what frightens
> me about all of this, Returned. **None of them started as monsters. Every one of them finished a
> sentence."***

---

## 🕸️ Reactivity threads (what Chapter 1 sets up for later)

| Flag set in Ch.1 | Read by |
|---|---|
| `aldric.crown_doubt_planted` / `aldric.count_cracked` | Act V — whether Aldric can be talked down at the Convergence |
| `act1.vayle_turns` / `vayle_will_learn_a_name` | Vayle's reappearances; `rep.kelemvor` ending gates |
| `roen.sister_hinted` · `varra.quill_known` | the companions' Act II personal quests (Wrenna; the broker Quill) |
| `companion.roen.recruited` · `companion.varra.recruited` | party-composition banter; who stands with you in Act V |
| `act1.mercy_remembered` · `named_vayle_monster` | the *deeds* axis the endings read (merciful vs. ruthless) |
| `crown.memory_runs_backward` | Act II's Sealed Stacks (*you are the crack*) and the Last Returned reveal |

> **The thesis of Chapter 1:** you arrive certain there's a villain, and you leave unable to find one —
> only people finishing sentences. Everything after is the cost of that lost certainty.
