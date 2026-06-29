# 🗺️ Side Quests & The Living City

> The texture that makes a CRPG feel like a *place*. Side quests (each a small, deliberate echo of
> the main themes — death, the forgotten, belief, grief, the price of mercy) and the memorable minor
> NPCs who make Baldur's Gate and the eras feel inhabited. Every side quest is optional; many feed
> flags that ripple into the finale and the companion arcs.

> **Theming rule:** every side quest is the central question — *what is a soul worth that no god
> claimed?* — asked in a smaller key. The city *rhymes* with the cosmos.

> **🗂️ Now a LIVE tracked catalog.** This doc is the design bible; the playable build now has a real,
> structured **Side-Quest Catalog** — [`../../play/sidequests.json`](../../play/sidequests.json) — that
> runs on the game's `QuestManager` engine: **20 tracked, multi-objective, branching quests** (incl. the murder-mystery *Second Death*) with
> givers, premises, themes, rewards, and optional/hidden objectives. **Every objective/completion/failure
> flag is set by real walkable content** (cross-checked in [`../../play/sidequests.test.js`](../../play/sidequests.test.js)
> — no dead quests), so the rich zones already built (the Wake, the Last Lantern-Feast, the Last Word,
> the After, the One Left, the Lidless Eye, the Honest Devil + the Knife You Carry, the Intake Queue, the
> Sigil doors — Reunion / Last Job / Long Truce / the Song / the Unseen Hands / the Forbidden Loves — the
> Custody, the Siege, the Wayward Mile's caprices, and the meta **Ledger of Small Mercies**) are now
> first-class, named, completable side quests in a quest log, not just walkable encounters. The catalog
> grows as content does — each new quest is one entry wired to flags the content sets.

---

## I. Baldur's Gate — Side Quests

### 🕯️ *The Last Name* — the Niche-Keeper
A half-mad clerk named **Old Pell** keeps an illegal ledger of the city's *unclaimed* dead — paupers,
the godless, the nameless — because, he says, "someone has to write them down before the Wall does."
He asks you to recover the name of a specific corpse in the potter's field before it's buried
unnamed. The investigation is small and human (a barmaid, a debt, a love affair); the payoff is that
Pell carves the recovered name into a quiet wall of the godless dead he tends in secret.
- **Theme:** to *name* the discarded is the whole game in miniature. **Reward:** Pell becomes a lore
  source; in the Break the Loop ending, Pell is the one who carves *your* name. **Flag:** `pell.wall`.

### ⚖️ *The Smaller Number* — a Flaming Fist captain
Captain **Doryn** must decide whether to quarantine (and effectively doom) a plague-struck tenement to
save the ward, or risk the ward to save the tenement. She asks your counsel — and whatever you advise,
you watch it cost. A deliberate, low-stakes rehearsal of Aldric's and Vayle's arithmetic, *before* you
meet either of them, so the main-plot dilemmas land on prepared ground.
- **Theme:** utilitarian atrocity, dry run. **Reward:** Fist reputation; Doryn appears at the
  Convergence on the side your advice implied. **Flag:** `doryn.choice`.

### 🎭 *The Understudy* — the Guild's face-thief
A Guild grifter, **Sabs**, has been *impersonating a dead noblewoman* so well that the woman's grieving
mother now believes her daughter returned. Expose the con (truth, cruelty), maintain it (mercy, lies),
or convince Sabs to *become* the kindness she's faking. A small, sharp study in whether a comforting
lie about the returned dead is a sin or a grace — which is, of course, *your* whole situation.
- **Theme:** the Returned, in a minor key. **Reward:** Guild access; ties to Roen's arc. **Flag:**
  `sabs.fate`.

### 📖 *The Book It Doesn't Have* — Candlekeep entry (mini-epic)
The Act II gate: Candlekeep admits only those bearing a volume it lacks. Multiple solutions — recover a
lost work from a flooded crypt (adventure), forge one convincingly (skill + risk), transcribe a dying
hermit's unwritten memoir (poignant), or steal one from a rival collector (Guild). Each route colors how
Candlekeep's monks treat you inside.
- **Theme:** knowledge as a toll; what's a truth worth. **Reward:** Act II access + a monk ally. **Flag:**
  `candlekeep.entry_method`.

### 🐦 *A Cage of Their Own* — the talking starling
A street-conjurer's familiar, a starling that has learned *too many* words, begs you to free it — but
freedom means it forgets the words and becomes a bird again. A tiny, perfect koan about whether a longer
life worth less is better than a short life that *meant.* (Varra has the best banter here.)
- **Theme:** the Deathless Garden, bird-sized. **Reward:** a charm; a Varra approval beat. **Flag:**
  `starling.freed`.

### ⛓️ *The Toll-Priest* — a corrupt cleric
A priest of a wealthy temple charges the poor for *guaranteed* passage past the Wall — a fraud, since no
mortal can promise that. Expose him (justice), extort him (Guild), or — the dark option — realize his
fraud is the only *comfort* the dying poor can afford, and decide whether to take it from them.
- **Theme:** mercy that charges a toll (the Apocrypha's exact heresy). **Reward:** gold/rep; a Garrow
  approval crucible. **Flag:** `tollpriest.fate`.

### 🌹 *The Widower's Garden* — a quiet one
An old man tends a garden for a wife five years dead, talking to her as he works. No combat, no twist —
just the choice of whether to gently tell him she's gone, sit and listen, or help him keep the garden.
A deliberate breather that foreshadows the Deathless Garden's horror by showing its *humane* version:
grief that *moves*, that changes, that is allowed to end.
- **Theme:** healthy grief vs. the frozen kind. **Reward:** a seed that blooms in your epilogue.
  **Flag:** `widower.visited`.

### 🗝️ *The Returned Before You* — a haunting
Rumors of another Returned, years ago, who came back and then *vanished.* Tracking the cold trail leads
to a sealed room, a journal in a familiar hand, and the dawning realization (for attentive players) that
the "other Returned" was **you, a cycle ago** — your first real clue to the loop, hidden in optional
content. The journal's last line is the *The Niche* text from the Codex.
- **Theme:** the loop, foreshadowed. **Reward:** a major lore flag toward the Nameless route. **Flag:**
  `prior_returned.found` → feeds `pc.true_name`.

---

## II. Per-Era Side Quests (samplers)

- **Netheril — *The Last Apprentice*:** a child arcanist in the falling enclave doesn't understand the
  city is doomed and just wants to finish her first spell before "the grown-ups stop arguing." You can't
  save the city. You can decide how she spends her last minutes. (Naeve approval crucible.)
- **Crown Wars — *The Vote Not Yet Cast*:** you arrive *just before* a lesser court votes its own
  damnation-decree. For the only time in the game, you can argue a Faithless-verdict *down* — proof that
  the law was always a choice. It changes nothing cosmic (history reasserts), but a single people's souls
  are spared in one valley, and the act unlocks dialogue with the Unmade about whether *small* mercies
  matter. (Ilfaeril's heart.)
- **Time of Troubles — *A God's Errand*:** a deity-made-mortal, amnesiac and terrified in a Waterdeep
  alley, asks for help they don't realize is beneath them. Kindness to a fallen god; cruelty to one; or
  the unsettling option of *robbing* one. (Reputation across pantheons.)
- **Spellplague — *The Two-Hearted Man*:** a victim of the merging worlds exists *twice*, two lives
  bleeding into one mind. Help him choose which self to keep — or refuse to, arguing he's allowed to be
  both. A direct rehearsal of the protagonist's own identity-across-time crisis.

---

## III. The Living City — Memorable NPCs

> The faces that make the Gate breathe. Most are non-combat; all are reactive to your reputation,
> companions, and how far the main plot has progressed (the city visibly *frays* as the harvest spreads).

- **Maelle, Harper handler** — the woman who recruited Roen; warm, lethal, heartbroken to be hunting her
  own. Central to Roen's quest. Can become an ally, a casualty, or your enemy.
- **The Marshal of the Fist** — pragmatic, tired, running a city the dukes have stopped governing. Sells
  you order; charges in favors.
- **The Guildmistress ("Auntie")** — runs the thieves like a family business; treats murder as bad for
  trade. Dryly funny; the city's best information for the worst prices.
- **Brother Cassom, doubting Doomguide** — a young priest of Kelemvor quietly losing his faith over the
  Wall; a recurring confessor figure who tracks *your* moral drift back to you in conversation. A mirror
  for the player's choices.
- **Nib & Tully, gutter-kids** — two Outer City orphans who appraise you with merciless honesty and
  appoint themselves your "information network" (mostly wrong, occasionally gold). Comic relief that turns
  to stakes when the harvest reaches the slums.
- **Vesriel, the patriar widow** — old money, old grief, hosts the salon where Upper City secrets spill;
  knows everyone's price. Gateway to the dukes.
- **The Chandler ("Wick")** — keeps the safehouse below Aldric's; a former Choir member who *left*, and
  can tell you what the cult looks like from the inside, and why people stay.
- **Hester the Verse-Monger** — a street-corner prophet selling apocalypse pamphlets who is, alarmingly,
  *correct* about more and more of them as the acts progress. The city's barometer of dread.
- **The Ferryman of the Potter's Field** — barely speaks; carries the unclaimed dead to the pauper graves;
  the only NPC who treats *you* as already-dead, with a quiet courtesy that's either kindness or warning.
- **Spite, Varra's quasit** — not a quest-giver but a constant gremlin presence; comments on everything,
  betrays Varra's feelings she won't admit, occasionally steals from shops while you talk.

---

## IV. Reactive World State (how the city changes)

The Gate is not static set-dressing — it *responds*, so returning between dungeons feels alive:
- **Harvest spread:** as the main plot advances, wards go quiet, shutters close, the Unbound appear at the
  edges; vendors raise prices or flee; Hester's pamphlets get truer.
- **Reputation:** Doomguide patrols hassle or hunt you; Choir sympathizers slip you aid or warnings; the
  Fist opens or closes gates; the Guild fences for you or marks you.
- **Companion presence:** recruited companions appear in the hub between quests, react to city events, and
  spark ambient banter at specific locations (Garrow at the potter's field; Roen at the Harper safehouse;
  Maerin, if saved, learning to be alive in a market).
- **The Returned aura:** the recently dead linger near you everywhere; some minor NPCs (the Ferryman,
  Old Pell, Brother Cassom) can *see* this and treat you accordingly — a constant, quiet reminder of what
  you are, woven into the ambient life of the city.

> **Design note:** side content should never feel like filler — each quest is a *thesis statement* in
> miniature, so that by the time the player reaches Aldric's tea, the Unmade's offer, and the Mirror, the
> game has already taught them, in a dozen small human stories, exactly how much a discarded soul is worth.
