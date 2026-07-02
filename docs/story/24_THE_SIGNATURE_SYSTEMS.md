# 👁️ The Three Signature Systems — *the eye, the cost, and the book*

> The mechanics that make *Crown of Horns* **a game, not a novel you click through** — three systems
> that turn the saga's themes into *play.* Each is the same idea seen from a different side: **the way
> you see the dead is the enemy's; the price of seeing is becoming the thing you fight; and the record
> you keep of the seen is the only weapon that wins.** All three converge, playable, in the capstone
> zone **[The Lidless Eye](../../play/lidlesseye.json)** (off the Counting-House) — *"a door with no
> other side — the place behind your own eyes."*

> **Status.** The capstone (the reveal, the haunt-gating, the book-fullness branches, the read-aloud
> with crit/fumble) is **built and test-verified** in the walkable build, using the engine's existing
> `when:{int:{…}}` gating on both variants and choices. The *full-game rollouts* noted below (narration
> slip across all souls; live Niche-Book HUD; NG+ recording) are **specced here for the Unity build.**

---

## System 1 — *The eye you were given was the enemy's*
**The dead-sense narrator is the Unmade.**

Every read in the game is written in the same second-person voice: *"Your sense finds a wound at the
root of the work…"* The player assumes that voice is **their own perception.** It is not. It is **the
Unmade** — the accreted grief of every discarded soul — riding the seam it tore in you when it pulled
you back, showing you the suffering of the discarded in lavish, unbearable detail, because **a soul
made to *see* that much injustice will come to want what the Unmade wants: the Wall torn down,
whatever the cost.** Your compassion was never just the loop's engine. It was **cultivated.** The
reads you fell in love with are **radicalization** — grooming you toward the Abolition-atrocity, one
beautiful, *true* act of empathy at a time. (The Unmade never lies. That's the horror. It only chooses
*which truths* to show — see the Eye's "blind spot": it kept very quiet about why the Wall might *need*
to stand.)

- **Why you can see the dead when no living soul can:** because *something is letting you.*
- **The reveal (built):** in *The Lidless Eye*, the familiar narrating voice drops the pretense and
  addresses you as its author. *"'Your sense finds.' You thought that voice was yours. It was never
  yours. It was mine."*
- **The rollout (now built, 15 reads — expanding):** after the reveal sets `le.narrator_revealed`, the
  dead-sense reads begin to **slip.** Fifteen of the most-witnessed reads (across *the Wake*, *the Last
  Lantern-Feast*, *the Last Word*, *the After*, and *the One Left*) now carry a **second variant**, gated
  on `le.narrator_revealed`, in which the Unmade's mask bleeds through — the same read the player loved,
  *verbatim*, now bracketed by the narrator's undisguised voice. The **consistent tell** (so the player
  learns to recognize it): the voice slips from *"your sense"* into *"my sense / I / we"* and sometimes
  catches itself; it claims **kinship** (*"this one is already, in part, in me"*); it names the **Wall**
  as the author of the suffering and lingers, aggrieved; and it ends on the **recruitment whisper** —
  *"a soul who has truly seen this cannot, in conscience, leave the thing that does it standing… you
  know now whose eyes you borrowed, and what I would have you do."* The **original read is preserved
  verbatim inside the slip** (the player must recognize it), and the pre-reveal default is the original,
  untouched. Implemented purely as node `variants` with a required no-`when` default, so it renders in
  **both** the playable build and the dialogue simulator. *Rollout expands toward all ~241 reads.*
- **The escalation — Systems 1 + 2 fused (built):** the slip **worsens with your cold.** Each of the
  15 reads now carries a **third, top-priority variant** gated on `le.narrator_revealed` **and**
  `disp.haunted ≥ 18`, in which the mask drops *all the way.* Where the plain slip catches itself
  (*"your sense — my sense"*), the escalated tier **never corrects** — it is pure first person, names
  *your* cold directly (*"you feel how thin the warm world has gone in you. Good."*), addresses you as
  **kin who has nearly crossed over** (*"you are very nearly me"*), shares the read rather than showing
  it (*"look with me, kin to kin"*), and turns the recruitment whisper into **settled fact** (*"there is
  no whisper left between us… you already know what we must do. Finish the walk."*). Because
  `pickVariantText` returns the **first** matching variant, the tiers are ordered most-gated first:
  `[escalated (revealed + cold), slip (revealed), verbatim default]`. **This is the two systems made
  one:** the narrator you can't trust (1) becomes *more* untrustworthy exactly as the cost of trusting
  it (2) turns you into it.

## System 2 — *The cost of seeing is becoming the monster*
**Witnessing accrues `disp.haunted`; maxed, it makes you the Last Returned.**

Already **canon in embryo** in [`assize.json`](../../play/assize.json), which puts the over-haunted
player on trial *not for cruelty* but for becoming the Last Returned **"by attrition — freezing one
cold-but-kindly choice at a time,"** with a witness *"made of every soul you eased"* testifying that
*"you saved us with a hand that stopped being able to feel us."* The Lidless Eye completes the loop:
the Unmade greets the deep-cold player as **nearly itself** — *"I did not make you cruel. I made you
kind, until the kindness froze. We are nearly the same thing now."*

- **The mechanic:** every dead-sense read takes a measure of your warm world and spends it to see by
  the grey one. `disp.haunted` is a real counter (**431 increments** seeded across the game). Past a
  threshold, cold/certain options open and warm ones thin — *the empathy that makes you good at saving
  souls is the same thing turning you into the thing you fight.*
- **The agonizing choice it creates:** *this soul is suffering, and I could witness them — but every
  witnessing brings me a step closer to the Last Returned.* **You have to ration your compassion.** The
  players who try to save *everyone* are the ones who burn out cold — the truest statement the game can
  make about grief.
- **The way back (built):** the Unmade names it — *"a soul that only witnesses freezes; a soul that
  witnesses **and records**, that says the true thing aloud and lets it land in a book, can carry an
  ocean and stay warm."* The cold comes from carrying the seen **alone, un-set-down.** The Niche-Book
  is where you set it down. Which is System 3.

## System 3 — *The book you kept was the cure*
**The Niche-Book: the witness-ledger you forged without knowing, and the only weapon that wins.**

Every soul you truly witnessed wrote a line in a book you never saw: *a name, and the one true thing.*
**Tatters fed the gutter-cats. Brynalla Coorne kept the lock at Hambry. The knife wept.** It kept no
score of your power, cunning, or kills — only the **witnessing**, the souls you pulled out of the
nothing into a name. And it is the **answer neither the Unmade nor the Wall's defenders wanted you to
find:** not *burn it* (the grief's answer — fire is just more unmaking) and not *defend it* (the
church's answer — silence), but a **third thing.**

- **Why it's the weapon:** the Wall runs on **one** trick — making the discarded into *nothing*, so
  forgotten that erasing them costs the world no grief. Every name in the book **breaks that** — a soul
  that's been seen, named, written down *cannot be quietly dissolved, because someone would notice.*
  **You don't have to unmake the law to defeat it. You only have to out-write it.** A grief can argue
  with fire (fire is what it's made of); a grief **cannot argue with a name.**
- **Book-fullness is real and reactive (built):** branched on `disp.merciful` (**426 increments**
  seeded). The book greets you **full** (a chorus, almost singing), **thin** (real names with blank
  stretches where you walked on), or **near-blank** (a few names lonely in the white). The blank pages
  speak too — *"here is a soul you passed; here is a person the Wall kept because you walked on."*
- **The endgame, made playable (built):** you **read the book aloud** to the grief. A mercy-gated
  *"read the full book"* Performance (**crit:** the refutation completes, the grief comes apart gently
  back into the people it forgot it was; **fumble:** the names crash over you at once and your voice
  closes — *carry them as a procession, one at a time, not an ocean*) sits beside an always-available
  *"read what you kept"* that speaks the names **and** the silences honestly.
- **The blank-page reproach (now built):** the book's power has an inverse, and the capstone now closes
  all three systems on the player at once. In *The Lidless Eye* you can **turn to the blank pages** —
  and the Unmade rises through the empty paper to read your **specific un-witnessed souls back at you,
  by name.** Nine marquee souls (Tatters, Corliss, Brenn, Dot, Marisa, Onora, Sift, Brynalla, Tace) each
  get a reproach beat **gated on the *absence* of their witness flag** — so you hear, individually,
  exactly the people you walked past, *what you'll never know about them* (the one true thing you didn't
  learn), and the cut: *"the Wall kept them, because you walked on… a blank page is a name **I** keep."*
  Hearing each reproach **deepens your cold** (`disp.haunted +1` — System 2 fusion: your omissions make
  you colder) and marks it heard so it won't repeat. A player who witnessed **all nine** instead gets
  the Unmade *almost grateful* — *"there are no blanks… you wrote down every single one. Close the book.
  It is full."* And the close is the game's quiet thesis: *"you can go back… the next soul you meet, you
  could stop, you could learn the name, you could make the book one page less blank than the Wall would
  like."*
- **Engine note:** this required a small, faithful extension — `when.flagsNot` (every listed flag must
  be **unset**) added to the JS `matchesWhen` in both generators, **mirroring C#'s existing
  `RequireBoolFalse`** (the negation capability already lived in `DialogueRunner.cs`; the JS `when`
  object simply hadn't exposed it). Covered by new `dialogue_sim` engine unit tests.
- **The rollout (specced):** a live **Niche-Book HUD** — a diegetic book the player can open any time,
  filling line by line as they witness, becoming their most precious possession; at the climax it is
  *literally* read aloud against the Unmade. *(The blank-page reproach above is the first built piece of
  this; expanding the named-soul set beyond the marquee nine is the rollout.)*

---

## How the three rhyme
> **The Unmade gave you an eye to make you a weapon (1). Using it cost you your warmth and nearly made
> you the monster (2). And the only thing that redeemed both — the eye *and* the cold — was the book of
> names you kept while you looked (3).** The sight was the enemy's; the seeing was always yours; and
> what you chose to *do* with the seeing — witness, name, record — is the entire game, and the entire
> answer to *what is a soul worth that no god ever claimed?* **Exactly the number of times someone
> bothered to learn its name.**

## Tuning notes
- Capstone thresholds (first pass, tunable): **haunt-cold** at `disp.haunted ≥ 14`; **book-full** at
  `disp.merciful ≥ 24`, **book-thin** at `≥ 8`, **near-blank** below. Calibrate against real
  playthrough telemetry once the full act is assembled.
- All gating uses the shipped engine (`when:{int:{key:N}}`, `>=`, on variants **and** choices), so the
  systems are **live in the browser build today**, not just on paper.

> See also: [`13_TWIST_ARCHITECTURE.md`](13_TWIST_ARCHITECTURE.md) (the foreshadowing ledger, the
> weaponized-kindness twist), [`14_SCENE_WEAPONIZED_KINDNESS.md`](14_SCENE_WEAPONIZED_KINDNESS.md), and
> the cast's [`../characters/the-unmade.md`](../characters/the-unmade.md).
