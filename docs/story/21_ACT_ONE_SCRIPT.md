# 🎬 Act I — Full Script: *"The Gilded Gate"*

> The complete scene-by-scene script for the Prologue + Act I — the vertical-slice content. Each scene:
> **location · purpose · beats · branches · exit/flags.** Key new dialogue is written inline; the
> marquee conversations reference `10_DIALOGUE_SCRIPTS.md`. This is the spine; side quests (`11`) bolt
> on between scenes. Designed to be ~the first 6–8 hours of play (the Prologue ≈ the first hour / demo).

---

# PROLOGUE — *What the Dead Owe*

### P.1 — Character Creation · *the workshop of an unknown life*
**Purpose:** build the Returned; establish *your face* (which the Mirror will wear). **Beats:** the
creation frame is diegetic — a half-remembered self assembling in grey, the Wall faintly visible
behind the options. Last prompt: a name (which the game will quietly "forget" by P.4). **Exit:** SET
`pc.background`, `pc.created`.

### P.2 — The Harvest · *a wet cellar beneath Baldur's Gate*
**Purpose:** tutorial-by-trauma; the inciting death. **Beats:** an ordinary errand puts you in the
wrong cellar; the Choir's herald and cultists are bleeding soul-light from the dying; you're seen; the
chant turns toward you. *You do not see the blade.* **No combat yet** — this is a cutscene of helpless
witness. **Exit:** **death.** Cut to vacuum-silence (see `18` audio staging). SET `pc.died`.

### P.3 — The Wall · *the Fugue Plane* (the demo's emotional core)
**Purpose:** establish the central mystery & atrocity in ten silent minutes. **Beats:** a playable,
combat-free walk in the grey river of the dead → the City of Judgment → the Wall, made of faces → the
**niche with your name forming.** A voice speaks a word you can't hold. A cold hand reaches in and
**pulls.** **Exit:** SET `wall.seen`, `niche.name_glimpsed`, `unmade.word_spoken` (the withheld true
name).

### P.4 — The Return · *back in the cellar*
**Purpose:** establish "Returned"; first companion; first combat. **Beats:** your corpse breathes; the
herald recoils — *"You came back. They never come back."* — and flees. The harvest tore souls loose:
the **Unbound** rise. **First combat (tutorial):** Husks + a Wailer; teaches movement, attack, a
condition (**Frightened**), and the **first Lay vs. Shatter** choice on a downed Unbound. A trapped
ally fights beside you — **Garrow** (if Doomguide's-Ward/default) or a quick-met **Roen** (if
Flaming-Fist/Candlekeep), depending on background — your first companion. **Exit:** SET
`pc.returned`, `companion.<first>.met`.

### P.5 — The Cinderhaunt · *the harvest-site* (the demo's combat showcase)
**Purpose:** prove tactical depth; the first boss. **Beats:** fight up through the Cinderhaunt (already
built in code); loot; a locked way; the **Unbound Maw** mini-boss (`19` §V) — two phases, the inhale
mechanic, and at 0 HP it *speaks in the chorus of the harvested*, the first hint the monsters are
people. **Exit:** SET `prologue.cleared`; emerge into the city, marked and hunted.

> **END OF VERTICAL-SLICE PART 1.** A complete small horror story with a gut-punch. Everything after is
> Act I proper.

---

# ACT I — *The Gilded Gate*

## Chapter 1 — *Three Doors* (establishing the powers)

### 1.1 — Out of the Dark · *Baldur's Gate, Outer City*
**Purpose:** transition to the hub; establish tone of the living world (warm grime). **Beats:** you
surface into the slums; the recently dead linger at the edges of your vision (the Returned aura); a
gutter-kid (**Nib**) appraises you — *"You walk like a dead man who forgot to lie down."* He becomes
your first "information network." **Branch:** tip him / threaten him / ignore — sets early Outer City
rapport. **Exit:** the hub opens; the journal frames as the Ledger. SET `hub.unlocked`.

### 1.2 — The Harper's Offer · *a Lower City tavern*
**Purpose:** introduce the Harpers / recruit Roen (if not first companion); plant his secret. **Beats:**
**Roen Alleywind** "happens" to save you from a Doomguide patrol, then recruits *you* — charming, all
exits. **Subtext for the player to catch later:** he's handling you. **Branch:** see `BaldursGateHub`
recruit dialogue + `16` Spark. **Exit:** SET `companion.roen.met`; optional recruit.

### 1.3 — The Iron Summons · *a Flaming Fist checkpoint*
**Purpose:** introduce Vayle's hunt (the institutional threat) without meeting her yet. **Beats:** the
Fist has a writ for "a soul that walked back" — you. Captain **Doryn** (`11`) can be bribed, bluffed
(PERSUADE/DECEIVE), or fought. First taste of reputation systems. **Exit:** SET `fist.aware`,
`rep.fist` shift.

### 1.4 — A Devil's Arithmetic · *a Lower City counting-house*
**Purpose:** recruit Varra; plant her leash. **Beats:** a pact-collector is dragging **Varra** to
collection; intervene (combat or a CHA standoff). She joins, acid-tongued; the collector's parting line
plants the leash: *"The Wall reads receipts, witch. We'll be in touch."* **Exit:** SET
`companion.varra.met`; the leash flag that pays off in the Breach (`15`).

> **Companion gate:** by end of Ch.1 the player has met/recruited up to three of Garrow, Roen, Varra.
> Garrow appears in Ch.2 if not the Prologue companion.

## Chapter 2 — *The Heretic's Trail* (toward Aldric)

### 2.1 — The Doomguide Who Stayed · *the potter's field*
**Purpose:** recruit/deepen Garrow; the game's moral spine, stated. **Beats:** Garrow lays an Unbound
child; she asks you the question that defines her — *"When you came back out of the Wall, did it feel
like mercy, or like theft?"* Your answer seeds her arc (`02`/`06`). She knows the harvest-trail leads
to Aldric. **Exit:** SET `companion.garrow.recruited`, `garrow.question_answered`.

### 2.2 — Ashes & Rumor · *the Outer City; a Choir safehouse*
**Purpose:** show the Choir from the inside (they're sincere). **Beats:** **Wick** the chandler — a
*former* Choir member — explains why people join: every one of them lost someone to the Wall. A Choir
cultist here *surrenders* rather than fight, begging you to understand. The easy-villain frame begins
to crack. **Branch:** spare/condemn the cultist (Garrow vs. pragmatism approval). **Exit:** SET
`choir.humanized`.

### 2.3 — Tea with a Heretic · *a safehouse above a chandler's* ⭐
**Purpose:** THE Act I set-piece; meet Aldric. **Beats:** the full scene from
`10_DIALOGUE_SCRIPTS.md` §1 — Aldric pours tea, tells the truth, and is *right about the Wall.* The
player leaves unsettled, not triumphant. **Branches:** provisional ally / neutral / named-enemy; the
[INSIGHT] grief read; the [INSIGHT] "something is using you" Myrkul seed. **Exit:** SET `aldric.met` +
the stance flags; the Crown's location is the next goal — and it's locked in **Candlekeep.**

### 2.4 — The Crown's Whisper · *(if you took anything of Aldric's / touched a Lantern-shard)*
**Purpose:** plant the Crown-Voice (Myrkul). **Beats:** the first faint, reasonable whisper — counsel
that's *technically* true but bends toward the throne. **Exit:** SET `crownvoice.first` (Myrkul seed #1,
per `13` §I).

## Chapter 3 — *The Smaller Evil* (Vayle's mirror)

### 3.1 — The Trail North · *the Fields of the Dead road*
**Purpose:** travel beat; companion banter; first big Unbound battle. **Beats:** a harvest-site on the
old battlefield — the **first large Unbound encounter** (showcase combat: Husks, Wailers, a Lucid
Wraith that can be *talked down*). Banter triggers (`06` samples). **Exit:** SET `fields.reached`.

### 3.2 — The Burning of Hollowmere · *a harvest-village* ⭐
**Purpose:** Vayle's "lesser evil," whole; the Act's moral climax. **Beats:** you arrive as **Sister
Vayle's** Doomguides put the village to the torch *"to stop the spread."* You can save a *handful*, not
all — a timed, choice-dense sequence (who do you pull from the fire?). Vayle meets your eyes across the
flames and **does not look away.** First meeting with the antagonist-who-is-a-true-believer.
> **Vayle (across the fire):** "Look at me. Don't look at the flames — look at *me*, and understand: I
> have read what comes when someone tries to be kind to this disease. I am the smaller number. I am
> *always* the smaller number. Hate me with a clear conscience. I'd trade my conscience for yours in a
> heartbeat — but one of us has to keep the count."
**Branches:** who you save (sets minor NPC fates + Garrow/companion approval); confront or withdraw.
**Exit:** SET `hollowmere.burned`, `vayle.met`, save-tally flags.

### 3.3 — The First Skip · *the edge of the burning village* ⭐ (the scope reveal)
**Purpose:** the Act I cliffhanger; the demo's closer. **Beats:** in the ash, you touch the Crown's
psychic wake (a relic-trace Vayle was hunting) and **the world skips** — three heartbeats standing in a
golden city *in the sky*, sunlight on glass, a sense of imminent catastrophe — then *back*, nosebleeding,
alone in having felt it. No one else reacts. The crack in your soul has begun to open onto *somewhere
else.* **Exit:** SET `act1.first_skip`, `time.crack_opening`.
> **Companion (Naeve's absence felt — foreshadow):** "...You went somewhere. For a moment, you weren't
> *here.* Your shadow fell the wrong way." / **You:** "I saw a city. In the sky. It was beautiful. It
> was *falling.*"

### 3.4 — The Road to Candlekeep · *Act I → Act II hand-off*
**Purpose:** set the next goal; raise the stakes. **Beats:** the only place that knows the Crown's
location — and what the skips *mean* — is **Candlekeep**, the great library, which admits only those
bearing a book it lacks (seeds the Act II entry quest, `11` *The Book It Doesn't Have*). The skips are
getting longer. Aldric and Vayle are both racing you there. **Exit:** SET `act1.complete`,
`act2.unlocked`.

> **END OF ACT I.** The player has: a party, a stance on Aldric, a wound from Hollowmere, the first
> crack of the cosmic scope, and a clear next goal — with the dawning unease that their "second life"
> has a door in it that opens onto somewhere that shouldn't exist.

---

## Act I — Reactivity Ledger (what Act I tracks for later payoff)

| Flag | Set by | Pays off in |
|---|---|---|
| `pc.background` | creation | unique dialogue all game; the Nameless route |
| `niche.name_glimpsed` / `unmade.word_spoken` | Prologue | the Act V name reveal (Foreshadowing Ledger) |
| `companion.varra.met` + leash | 1.4 | **the Breach** (`15`) — who the Wall takes |
| `aldric.<stance>` | 2.3 | Aldric's Act V fate; Choir reputation |
| `crownvoice.first` | 2.4 | the Myrkul reveal (Act II/IV) |
| `hollowmere` save-tally | 3.2 | minor-NPC epilogues; Vayle-alliance availability |
| `vayle.met` | 3.2 | the Doomguide's Peace ending path |
| `act1.first_skip` | 3.3 | Act II's escalating skips → Act III time travel |

> **Writing note:** Act I's job is to make the player *love the warm, grimy, doomed living world* (so
> they'll care what it costs to save or lose it), *trust no one's certainty* (Aldric is right, Vayle is
> right, both are monstrous), and *feel the floor get thin* (the skip). Every later horror is a
> withdrawal against the emotional capital banked here. Bank generously.
