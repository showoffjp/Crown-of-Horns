<div align="center">

# 📝 CHANGELOG — The Crown of Horns

![Keep a Changelog](https://img.shields.io/badge/format-Keep_a_Changelog-orange)
![SemVer](https://img.shields.io/badge/versioning-SemVer-blue)

*All notable changes to the project. Newest first.*

</div>

---

## 👑 v6.66.0 — *"The Road Behind You"* — the world remembers (content)

> *"The world remembers,"* concentrated into a camp beat that makes the player feel **seen.** At the
> **Hearth**, a new *"reckon up the road behind you"* choice opens a reflection where **the threads that
> appear are the ones *you* walked** — and only those. A soul looking back at what the journey made of it.
- 🪡 **The threads** (each gated on what you did): **the names you've kept** (`disp.merciful` — Tatters,
  Brynalla Coorne, the knife that wept, *the list you keep without deciding to*); **the cold in you**
  (`disp.haunted` — *there's a version of you at the end of this road who can't feel anything, and you're
  fairly sure you've already met him, and he wears your face*); **the dog that chose you**; **the
  gambler**; **Dot** (*the lantern that gutters is the only one that gives light*); **the man you couldn't
  save** (*still learning that "couldn't" and "failed" aren't the same word*); **the verdict** you handed
  down in the Second Death; **the caprices**; and **the voice behind your eyes** (*whose eye is open
  behind your own*).
- 🔀 **The reflections are themselves reactive:** Calloway reads differently if you **ruined** him vs.
  became the **friend who holds his throat and chose not to squeeze**; the verdict reads differently for
  **overturned-the-law / accused / honest-lie** — each its own weight.
- 🛟 Built as gated reflection choices off `hearth.fire` (every thread `when`-gated on a flag/disposition,
  with an ungated exit); **autoPlay-safe** (the reflection is never the validator's first pick) and
  gate-tested for both the thread-gating and the reactive variants. **76 zones, 248 souls, 24 banters.**
  Gate **801** ✓ · full suite **1548** ✓ · all-in-one **15** ✓ · bundle rebuilt ✓.

## 👑 v6.65.0 — *"The Fire Talks"* — party banter, now surfaced in-game (systems)

> The banter system (v6.63.0) was tested but not yet *playable*. Now it is. At the **Hearth** (camp), a
> new *"sit back and let the party talk"* choice plays a **live banter** — the engine picks one that fits
> your **current party + deeds + dispositions**, voices the companions line by line, marks it heard so it
> won't repeat, and returns you to the fire to hear another.
- 🔥 **The renderer hook:** a node with `banter: true` calls `pickBanterNow` over the **embedded catalog**
  (the same `banter.js` logic the headless gate proves now runs in the playable browser build — engine +
  data injected via a `__BANTER__` token). Sit at the fire after a hard zone and you'll hear Dot and
  Garrow argue what the dead are owed, or **Ilfaeril quietly clock how cold you've gotten and beg you not
  to carry it alone.**
- 🛟 **`autoPlay`-safe:** the listen choice is inserted so it's never the completeness-simulator's *first*
  pick, so the banter node (which loops back to the camp menu for "hear another") is never walked by the
  validator — no false "dead end."
- ✅ Both the **engine embed** (`pickBanterNow` / `banterEligible` / `const BANTER` present in the build)
  and the **camp surfacing** (`hearth.fire` → `hf_banter`) are gate-tested. This **completes the party-
  banter delighter** — the connective tissue that turns 76 brilliant rooms into a party you travel with.
- 📄 **76 zones, 248 souls, 24 banters (now audible).** Gate **799** ✓ · full suite **1546** ✓ ·
  all-in-one **15** ✓ · bundle rebuilt ✓.

## 👑 v6.64.0 — *"The Same Dog"* — the warm emotional through-line (content)

> The highest emotional ROI per line in the game. The **Same Dog** — the brown ordinary dog that hears
> the Wall's song — already turned up in six zones; now it **recurs deliberately**, finding you wherever
> the road bends, because it decided (the way dogs decide) to walk you home.
- 🐕 **Name it** — or refuse to, in the truest way: *"Names are for things that leave. You never leave.
  So I won't name you — I'll just call you **mine.** That's not a name. It's a promise."*
- 🐾 **Ask why it keeps finding you** — your dead-sense reads the shape of it: it hears the Wall's song,
  goes to the souls the song is pulling at, and *stays* — for nothing, forever, in a country of gods
  who won't.
- 🛋️ **Just sit** — climb-half-into-your-lap, set every grief down for one stolen minute, *let the
  simplest comfort there is — a dog, leaning — be enough.*
- 🩶 **The beat that lands hardest** (fusing the Dog with the cold-cost system): when **you** go cold —
  `disp.haunted` high, the Wall beginning to sing *your* name — the dog comes straight to you, presses
  its whole warm weight against your leg, and **will not leave**, because the one thing a dog knows how
  to do about a person slipping into the dark is *stay, lean, be warm and here.* *"Dogs don't save you.
  They just make sure you're not alone in the cold long enough to save yourself."*
- 🔧 Built as a **`need`-gated recurrence** (need: `cap.seen_dog`) with variants reactive to your state
  (cold / named / first-return); tracked in the side-quest catalog (`cap.named_the_dog`). **76 zones,
  248 souls, 13 road-caprices, 24 banters.** Gate **797** ✓ · full suite **1544** ✓ · all-in-one **15**
  ✓ · bundle rebuilt ✓.

## 👑 v6.63.0 — *"The Party Is Alive"* — reactive party banter (systems + content)

> The biggest *"this world breathes"* multiplier — the BG3/Disco-Elysium secret sauce the game was
> missing. Companions now react to **each other**, to **what you've done**, and to **how cold or kind
> you've become.** A real, tested system (`play/banter.js` + `play/banter.json`), mirroring the C#
> `CampfireBanter` / `PartyBanter` content classes — turning a set of great scenes into a *party you
> travel with.*
- 💬 **24 cross-companion barters** in the first batch. Character friction & warmth: **Dot & Garrow** on
  what the dead are owed (a party vs. solemnity); **Varra** needling **Ilfaeril**'s ten-thousand-year
  guilt, then admitting she lies awake too; **Naeve** trying to *measure* **Maerin**'s dissolution and
  getting told *"I'm dying, not a theorem — ask me about the cats";* **Roen & Maerin** (the orphan who
  chose his name and the girl who refuses her symbol); **Dot** force-feeding **Varra** because *"I made
  too much, like always, and the people I made it for are mostly dead."*
- 🪡 **Reactions to what you did:** the party on Calloway (Varra admires, Garrow can't hate him either),
  on the Wake, on **Sevrin's death you couldn't prevent**, on the **Second Death** verdict (Garrow's law
  vs. Varra's mercy), on learning the **dead-sense was the Unmade all along.**
- 🩶 **Reactions to how cold/kind you've grown** (gated on `disp.*`): **Ilfaeril** feeling you go cold and
  begging you not to carry the grief alone — *"I am ten thousand years old and still warm; the secret is
  do not carry it alone";* **Dot** taking your freezing hands and teaching you to *stay warm on purpose,
  out of spite;* **Garrow**'s quiet pride as your mercy mounts; **Roen**'s unease when **Varra** starts
  admiring how ruthless you've gotten.
- ✅ **The engine** (`banter.js`, mirrors C#): a banter fires only when its companions are present and its
  `flags` / `flagsNot` / `int` gates pass; `once` (default) means no repeats. A new gate
  (`play/banter.test.js`, wired into `run-all`) tests presence, eligibility, once-gating, and selection —
  **and a live cross-check proves no dead banter** (every flag and recruitable companion a banter needs
  is actually set by real content). *(Next: surface it in-game via a camp/road hook; then the Same Dog
  through-line.)*
- 📄 **76 zones, 248 souls, 20 quests, 24 banters.** Gate **796** ✓ · banter gate **13** ✓ · full suite
  **1543** ✓ · all green.

## 👑 v6.62.0 — *"New Side Quests"* — 4 new caprices + a murder-mystery zone, all tracked (content)

> Fresh content on top of the new quest log: **four new Wayward Mile caprices** (the random side-quest
> pool is now a **dozen** events) and a **brand-new standalone zone in a shape the game didn't have — a
> murder mystery.** All wired into the live catalog (now **20 tracked quests**).
- 🧾 **The Last Customer Complaint** — a dead shopkeeper mans a *Returns & Grievances* desk; **file a
  formal complaint against the Wall of the Faithless itself.** It goes nowhere and means everything,
  stamped **RECEIVED** — *the filing is the fix; not of the grievance, of the griever.*
- ⚔️ **The Two-Minute War** — two dead armies re-fighting forever a battle that lasted *two minutes* in
  life. Broker a **simultaneous** laying-down so no one is first (a Persuasion — crit: three centuries
  end on one soft sound; fumble: never say *surrender* to men who never ran). *The only thing they can't
  lose is the not-having-run.*
- 🤲 **The Orchard of Hands** — a soul grows, from grey trees, **the hands of everyone who ever had to
  let go of a dying one.** Walk the rows; hold a letting-go you know. *The love is the opening, not the
  clutching.*
- 🪟 **The Memory That Isn't Yours** — the Wall mis-filed a stranger's whole life into you (a kitchen, a
  blue door, sixty years). **Return it** — or discover a memory *shared* isn't halved but **doubled**, a
  love two souls remember that the Wall can't dissolve.
- 🔎 **The Second Death** *(new standalone zone — a murder mystery)* — a soul has been **un-made** (truly
  ended), which should be impossible. Investigate **Inquisitor Sahl** (her doctrine cracking), **Corwin**
  (the friend who carries it alone), and **Liss** (who knows the Wall's worst-minute torment-loop from the
  inside) — uncover that **Edda** spent *eleven years* looped in her worst minute and begged her oldest
  friend to open the door — then **deliver the verdict yourself**, each gated on what you investigated:
  **accuse** (the law is the law), **protect** (an honest lie — she chose her own end), or argue the whole
  truth and **overturn the law** (a Persuasion — *crit:* an Inquisitor of Kelemvor renounces her sigil and
  carries the *right to a true end* to the grey Concord). Corwin's grief is reactive to your verdict.
- 🔧 Catalog → **20 quests** (the new mystery + the 4 caprices tracked under *What the Road Throws*), still
  **all live** (every flag set by content). **76 walkable zones, 248 souls.** Gate **796** ✓ · side-quest
  gate **15** ✓ · full suite **1530** ✓ · all-in-one **15** ✓ · bundle rebuilt ✓.

## 👑 v6.61.0 — *"The Quest Log"* — the side quests become a live, tracked, tested catalog (systems)

> The game shipped a full **QuestManager** engine (explicit start → objectives advance on flags →
> completion/failure precedence → save round-trip, mirroring C#) — but the playable build **never
> populated a quest catalog.** Every rich zone *was* a side quest; **none were tracked.** Now they are.
- 🗂️ **[`play/sidequests.json`](play/sidequests.json) — 19 tracked, multi-objective, branching side
  quests** with givers, premises, themes, rewards, and optional/hidden objectives, spanning the whole
  game: *The Wake the World Withheld · The Last Lantern-Feast · The Last Word · The After · The One Left ·
  The Lidless Eye · The Honest Devil · The Knife You Carry* (a branching **ruin-or-protect** arc) *· The
  Intake Queue · A Custody of the Small ·* the Sigil omniverse doors (*A Reunion Across an Age · The Last
  Job · The Long Truce · The Song That Remembers · The Unseen Hands · The Forbidden Loves*) *· A Refugee
  Named · What the Road Throws* (the random caprices) *·* and the meta *Ledger of Small Mercies.*
- ✅ **No dead quests.** A new gate, [`play/sidequests.test.js`](play/sidequests.test.js), **scans every
  walkable zone** and proves that **every objective / completion / failure flag the catalog references is
  actually set by real content** — then **runs each quest through the live engine** (StartQuest →
  activate → objectives report as flags land → completes on its completion flag; and the C# contract that
  *failure beats a simultaneous completion*), and round-trips the whole catalog through
  ExportState/ImportState. Wired into `run-all.js`.
- 📄 The design bible [`docs/story/11_SIDEQUESTS_AND_NPCS.md`](docs/story/11_SIDEQUESTS_AND_NPCS.md) now
  points at the live catalog. The catalog **grows as content does** — each new quest is one entry wired
  to flags the content already sets. **75 zones, 245 souls, 19 tracked side quests.** Gate **788** ✓ ·
  side-quest gate **15** ✓ · full suite **1522** ✓ · all green.

## 👑 v6.60.0 — *"The Knife You Carry"* — Calloway's secret becomes a loaded gun (content)

> The payoff of the whole Calloway arc: the secret you may be carrying — *that the Honest Devil secretly
> forgives debts for nothing* — is **the one thing that can destroy him**, and the game puts the choice
> entirely in your hands.
- 🔪 **The lever (at the Long Odds).** Carry `cap.calloways_secret` and a new option opens with **the
  Auditor**: his power is *credibility* — that a marker is iron — so the truth that he's a soft touch
  **turns every marker he owns to paper.** Hand her that, and you can **ruin him**
  (`cal.calloway_ruined`): the house folds, his **hundred indentured souls go free overnight**, Em walks
  whole — *a genuinely just act* that also ends the one soul in the dead lands who was never afraid of
  anything. Both truths, carried at once.
- 🤝 **Or protect it** (`cal.protected_secret`) — and the Auditor, moved that you'd spare a man his single
  shameful decency, keeps her oath and walks clean too: *"closing him with a secret he was ashamed of
  isn't an audit, it's a mugging with letterhead — I did not stay honest ninety years to become that."*
  Two souls leaving his table with clean hands. *He'd hate it. Don't tell him.*
- ♠️ **The response (recurrence #2, `need: cap.calloways_secret`):** Calloway finds you again, reactive
  to what you did. **Ruined**, he is *magnificently gracious* — no hate, a connoisseur's appreciation,
  then *delight:* *"four hundred years of winning is boring, and you've just dealt me the one hand I
  don't know how to play"* — he'll claim himself from nothing again, the way he did the first time.
  **Spared**, he is **undone** — the gambler learns he read one soul right, made *safe with someone* for
  the first time since the cold, and owes you the one thing he's never given anyone in four centuries:
  **a friend who knows the worst of him and stayed** (`cap.calloway_truefriend`).
- 📄 Dossier updated with the full **three-encounter arc.** **75 zones, 245 souls.** Gate **788** ✓ ·
  engine **105** ✓ · full suite **1507** ✓ · all-in-one **15** ✓ · bundle rebuilt ✓.

## 👑 v6.59.0 — *"The House Always Finds You"* — Calloway recurs (systems + content)

> The magnificent bastard **follows up.** The marker mechanic from *The Honest Devil* now pays off: once
> you've met Calloway, he can **materialize at random on the Wayward Mile** to settle the debt — reactive
> to which marker you hold. A one-off becomes a **recurring character.**
- ♠️ **He pays** (if `cal.calloway_owes_you`) — appears *at the knife-edge*, crushes his own marker, and
  the locked way simply opens: *"Calloway honors every debt — the only law I never bend; a gambler who
  welches is just a thief in a nicer coat."* The villain as your most *dependable* ally, because of his
  code.
- ♠️ **He collects** (if `cal.owes_calloway`) — and reveals **his one secret.** He calls in your favor to
  make *you* the instrument of a kindness he'd never admit to: deliver a sealed token to **Em the
  Mortgaged Man**, his debt *forgiven, in full* — and tell him it came from *anyone but Calloway*,
  *"because if one soul learns Calloway forgave a debt for nothing, I'm ruined."* The Honest Devil's
  single crack: he's softer than he sells, and spends your marker to *hide* it (`cap.calloways_secret`).
- ♠️ **He visits** (if `cal.walked_clean`) — no angle, an empty hand: the freest soul in the afterlife
  owes everyone nothing and therefore has *no equals*, and you — who walked his floor and folded the
  winning hand — are the closest thing he's had to one since death. He gives you a real warning, free.
- 🔧 **Engine: the `need` extension.** Draw entries now accept an optional **`need`** (a prerequisite flag
  that must be *set*); `pickDraw` filters on `once` (unset) **and** `need` (set), so an event stays out of
  the random pool until the story unlocks it — *the* mechanism that turns a rare one-off into a recurring
  character. New `dialogue_sim` unit tests (excluded until met; included once met). **75 zones, 245
  souls.** Gate **786** ✓ · engine **105** ✓ · full suite **1505** ✓ · all-in-one **15** ✓ · bundle
  rebuilt ✓.

## 👑 v6.58.0 — *"The Honest Devil"* — the magnificent-bastard villain (content)

> The cast's **first villain who isn't sad.** Every other antagonist is grief-curdled — Aldric, Myrkul,
> the Unmade, the Last Returned. **Calloway** looked at the same grey cosmos that discards souls and
> *grinned, and got rich off it:* a dead con-man who beat the Wall by being **too entertaining to
> discard** — 20 CHA, an 11/10 aura, and almost impossible to hate even as he fleeces you and tells you,
> truthfully, that he's doing it. Found at **the Long Odds** (off the Night Market). Full dossier:
> [`docs/characters/calloway.md`](docs/characters/calloway.md).
- ♠️ **Calloway, the Honest Devil** — his gimmick is that he **never lies** (*"the truth, fanned out just
  so, is the prettiest hand at the table"*); his laws are *the first drink's free* and *he honors every
  wager.* The dead-sense finds **two truths at once that won't resolve**: the **predator** (monetizes
  the discarding, owns pieces of a hundred souls, would broker the world for the right number) and the
  **magnificent refusal** (the one soul in all the grey who looked at a cosmos that called him worthless
  and *claimed himself*, set his own price, threw a party at the Wall) — *both entire, which is the exact
  reason he's impossible to hate.*
- 🎴 **You can't beat him with force — only:** **out-deal** him (a haggle Insight check — **crit:** he
  tears up your debt and *owes you one*; **fumble:** you out-smart yourself into a worse deal and he's
  delighted); **out-wager** him (cut the cards — **crit:** the only soul in 400 years to beat the house
  straight, and he hands you a marker with his own name on it; **fumble:** you try to cheat *Calloway, at
  cards, in his own house*); **name** him ([RETURNED] — and he *loves* being seen, the magnificent-bastard
  hallmark: *"I'm having that engraved, in gold"*); or **walk clean** — the one move he genuinely
  respects, which flips his greeting and makes him almost an ally.
- ◐ **the Mortgaged Man (Em)** — the victim who'd do it again: sold his face, years, and name to escape
  the Wall, and *defends* Calloway, because the Wall would have dissolved him for free and Calloway
  *came.* A [RETURNED] names the **false choice** and wakes him to the con under the con.
- § **the Auditor** — the nemesis who's losing: three months and eleven framed injunctions in, she's
  being *seen* for the first time in ninety grey years and starting to like it. A [RETURNED] hands her
  the only weapon that works — **walk out owing nothing: the one audit he can't frame.**
- 🔧 New cast section ("the wildcard — the magnificent bastard") in the character index. **75 zones, 245
  souls.** Gate **785** ✓ · full suite **1502** ✓ · all-in-one **15** ✓ · bundle rebuilt ✓.

## 👑 v6.57.0 — *"The Wayward Mile"* — a random-event system (systems + content)

> Bolt-from-the-blue one-off events, each **very** unique and idiosyncratic, surfaced **rarely and at
> random**, so **no two playthroughs see the same handful.** Reached from the causeway via *"a path that
> wasn't there a moment ago…"* — the **Wayward Mile** (`play/caprice.json`). Full design:
> [`docs/story/25_THE_WAYWARD_MILE.md`](docs/story/25_THE_WAYWARD_MILE.md).
- 🎲 **New engine primitive `draw`** — a node routes at **random** to one of a pool of events, **skips
  any already seen** (a per-event `once` flag, so a one-off never repeats), **honors a per-run cap**
  (`drawMax` on `drawCount`, currently **4** — a single passage only ever spends a handful), and falls
  through to `drawElse` (*"the road has gone still… it will be different, next life"*). `pickDraw` is a
  **pure function, unit-tested deterministically** (rnd→0 picks first unseen; seen skipped; cap + drained
  pool both fall to `drawElse`); runtime RNG is `Math.random` in the **browser build only**, never in a
  tested path. The draw node mirrors `drawElse` into `auto`, so the structural `autoPlay` validator still
  completes gracefully. Added to **both** JS generators + the engine export.
- 🪙 **The Coin That Calls Heads** — a dead gambler whose coin *is his own worn-thin soul*; a Perception
  gamble (**crit:** the coin lands on its **edge**, the one outcome the loop has no rule for, and he
  gives it to you; **fumble:** he drops himself down a crack in the world).
- 🐕 **The Same Dog** *(canon)* — wordless; drops a bone scratched with a name (*Elsa*); follow it and
  learn it carries names to the dissolving, every day, for nothing.
- ⚖️ **The Last Word** *(canon: Corval)* — auctions a stranger's **final word**, paid in *carrying their
  name*: *"…oh, how silly. I've left the kettle on."*
- 🫂 **The Recursive Beggar** — a soul made of giving who **splits in two** every time you give him
  something; the cure is the one alms that doesn't divide him: **refusal.**
- 🌑 **The Apocalypse Salesman** *(Myrkul tie)* — *"one apocalypse, gently used, one careful owner (a
  god)"*; even the end of the world is just one more discarded soul holding a thing it hates.
- 🃏 **The Mid-Joke Comedian** — frozen three centuries one beat from the punchline; a Performance to
  **land it** (crit/fumble) or the mercy of **leaving him the *almost*.**
- 🔧 The Wayward Mile is a **device, not a soul**, so it's exempted from the soul-only global checks
  (Returned-sense / [RETURNED] choice) via an explicit `SYSTEM_CONVS` set. **74 zones, 242 souls.** Gate
  **776** ✓ · engine **103** ✓ · full suite **1493** ✓ · all-in-one **15** ✓ · bundle rebuilt ✓.

## 👑 v6.56.0 — *"The Blank Pages"* — System 3's reproach; all three systems close at once (systems + content)

> The Niche-Book's power has an **inverse**, and the capstone now lands **all three signature systems on
> the player at the same moment.** In *The Lidless Eye* you can **turn to the blank pages**, and the
> Unmade rises through the empty paper to read your **specific un-witnessed souls back at you, by name.**
- 🕳️ **The reproach** — nine marquee souls (**Tatters, Corliss, Brenn, Dot, Marisa, Onora, Sift,
  Brynalla Coorne, Tace**) each get a beat **gated on the *absence* of their witness flag**, so you hear,
  individually, exactly who you walked past — *what you'll never know about them* (the one true thing you
  didn't learn) — and the cut: *"the Wall kept them, because you walked on… a blank page is a name **I**
  keep."* Each reproach already heard is hidden (a per-soul `le.blank_heard_*` flag), so it never repeats.
- 🩶 **Fusion with System 2** — hearing each reproach **deepens your cold** (`disp.haunted +1`): your
  *omissions* make you colder, exactly as your witnessing does. The book's blank pages and the
  cold-road are the same ledger read from opposite ends.
- ✅ **The full-book inverse** — witness all nine and the Unmade is, instead, *almost grateful:* *"there
  are no blanks… i showed them to you to make you burn the world, and you wrote down every single one
  instead. Close the book. It is full."* And the close states the whole thesis: *"you can go back… the
  next soul you meet, you could stop, you could learn the name, you could make the book one page less
  blank than the Wall would like."*
- 🔧 **Engine extension (faithful):** `when.flagsNot` (every listed flag must be **unset**) added to the
  JS `matchesWhen` in **both** generators — **mirroring C#'s existing `RequireBoolFalse`** in
  `DialogueRunner.cs` (the negation already existed engine-side; the JS `when` object just hadn't exposed
  it). New `dialogue_sim` engine unit tests cover it (absent → pass, set → fail, all-unset required,
  composes with positive flags).
- 📄 [`docs/story/24_THE_SIGNATURE_SYSTEMS.md`](docs/story/24_THE_SIGNATURE_SYSTEMS.md) updated. **73
  zones, 241 souls.** Gate **768** ✓ · engine **98** ✓ · full suite **1480** ✓ · all-in-one bundle
  rebuilt ✓.

## 👑 v6.55.0 — *"The Cold Reading"* — Systems 1 & 2 fuse: the slip worsens with your cold (systems)

> The narration slip is no longer one-size — it **escalates with `disp.haunted`.** Each of the 15
> slipped reads gains a **third, top-priority variant** gated on `le.narrator_revealed` **and**
> `disp.haunted ≥ 18`, in which the Unmade's mask drops **all the way.** This is the two signature
> systems made one machine: **the narrator you can't trust (System 1) becomes *more* untrustworthy
> exactly as the cost of trusting it (System 2) turns you into it.**
- 🩶 **The escalated tier** — where the plain slip *catches itself* (*"your sense — my sense"*), the
  escalated tier **never corrects.** It is pure first person; it **names your cold directly** (*"you feel
  how thin the warm world has gone in you. Good."*); it addresses you as **kin who has nearly crossed
  over** (*"you are very nearly me"*); it **shares** the read instead of showing it (*"look with me, kin
  to kin"*); and it turns the recruitment whisper into **settled fact** (*"there is no whisper left
  between us… you already know what we must do. Finish the walk."*).
- 🔧 **Ordering matters:** `pickVariantText` returns the **first** matching variant, so the tiers are
  most-gated-first — `[escalated (revealed + cold), slip (revealed), verbatim default]`. A deep-cold
  revealed player gets the escalated read; a warm revealed player gets the plain slip; a pre-reveal
  player gets the untouched original. The **verbatim original read is preserved inside every tier**, and
  the nodes keep their `effects` + `auto:"END"`.
- 📄 [`docs/story/24_THE_SIGNATURE_SYSTEMS.md`](docs/story/24_THE_SIGNATURE_SYSTEMS.md) updated with the
  escalation (Systems 1+2 fusion). **73 zones, 241 souls.** Gate **765** ✓ · full suite **1473** ✓ ·
  all-in-one bundle rebuilt ✓.

## 👑 v6.54.0 — *"The Slip"* — System 1 reaches backward through the game (systems + content)

> The narration-slip rollout from the capstone's design spec, **built and test-verified.** Once *The
> Lidless Eye* sets `le.narrator_revealed`, **fifteen of the most-witnessed dead-sense reads** gain a
> **second variant** in which the Unmade's mask bleeds through — so System 1 (*the narrator was the
> Unmade all along*) is no longer confined to the capstone; it **retro-actively poisons the reads the
> player already loved.**
- 👁️ **The slip, across 15 reads** — in *the Wake* (Corliss, Tatters, Brenn), *the Last Lantern-Feast*
  (Dot, Marisa, Onora), *the Last Word* (Sevrin, Sela, Coll), *the After* (Sela, Coll, Tamsin), and
  *the One Left* (Sift, Bryn, Tace). After the reveal, each read shows the **same beloved text,
  verbatim**, now bracketed by the narrator's undisguised voice.
- 🩸 **The consistent tell** (so the player learns to recognize the one voice): the narration slips from
  *"your sense"* into *"my sense / I / we"* and sometimes catches itself; claims **kinship** (*"this one
  is already, in part, in me"*); names the **Wall** as the author of the suffering and lingers,
  aggrieved; and closes on the **recruitment whisper** — *"a soul who has truly seen this cannot, in
  conscience, leave the thing that does it standing… you know now whose eyes you borrowed, and what I
  would have you do."* *On a replay: "oh god, it was always talking like this and I never noticed."*
- 🔧 **Implementation:** pure node `variants` gated on `le.narrator_revealed`, with the **verbatim
  original as the required no-`when` default** — verified to render in **both** the playable build
  (renderer calls `pickVariantText` on every node) and the dialogue simulator. The reads keep their
  `effects` + `auto:"END"`, so all mechanics stay intact. Rollout **expands toward all 241 reads.**
- 📄 [`docs/story/24_THE_SIGNATURE_SYSTEMS.md`](docs/story/24_THE_SIGNATURE_SYSTEMS.md) updated (System 1
  rollout now "built, 15 reads — expanding"). **73 zones, 241 souls.** Gate **765** ✓ · full suite
  **1473** ✓ · all-in-one bundle rebuilt ✓.

## 👑 v6.53.0 — *"The Lidless Eye"* — the capstone: three signature systems, playable (content + systems)

> Not just a zone — the **capstone where the game's three signature mechanics converge, built and
> test-verified** in the walkable build using the engine's existing `when:{int:{…}}` gating on variants
> **and** choices. *A door with no other side*, off the Counting-House: the place behind your own eyes.
> Full design spec in [`docs/story/24_THE_SIGNATURE_SYSTEMS.md`](docs/story/24_THE_SIGNATURE_SYSTEMS.md).
- 👁️ **System 1 — the dead-sense narrator is the Unmade.** Every read in the game (*"your sense finds a
  wound at the root of the work…"*) was the grief of every discarded soul looking **through** you,
  showing you suffering to **radicalize** you toward tearing the Wall down. *You thought the voice was
  yours. It was never yours.* A **[RETURNED]** names how the weapon misfired: being shown the discarded
  didn't make you want to *unmake* — it made you want to **witness**; the eye meant to make a weapon
  made a *book of names* instead. (Entering sets `le.narrator_revealed`, the hook for the specced
  late-game narration-slip rollout.)
- 🜏 **System 2 — compassion has a cost (haunt → the Last Returned).** Already canon in `assize.json`
  (the over-haunted are tried for becoming the Last Returned *"by attrition"*); here the Unmade greets
  the deep-cold player as **nearly itself** — *"I did not make you cruel — I made you kind, until the
  kindness froze."* A haunt-gated **choice** (`when:{int:{disp.haunted:14}}`) confronts the cold, and
  the Unmade names the **way back**: *a soul that only witnesses freezes; a soul that witnesses **and
  records** can carry an ocean and stay warm.* The cost is real — `disp.haunted` carries **431
  increments** across the game.
- ❡ **System 3 — the Niche-Book is the weapon.** The witness-ledger you forged without knowing — a
  name and the one true thing per soul. Node-0 **branches on `disp.merciful`** (**426 increments**):
  **full** (a chorus, almost singing), **thin** (real names, blank stretches where you walked on), or
  **near-blank** — and the blank pages speak for the souls you passed. You **read it aloud** to the
  grief: a mercy-gated *"read the full book"* **Performance** — **crit:** the refutation completes and
  the grief comes apart *gently* back into the people it forgot it was; **fumble:** the names crash over
  you at once and your voice closes (*carry them as a procession, not an ocean*) — beside an
  always-available *"read what you kept"* that speaks the names **and** the silences. *You never had to
  tear the Wall down. You only had to out-write it.*
- ◉ **The Eye can be reclaimed** — a faculty given for a dark purpose, redeemed by the hand that used it
  for mercy: *"a weapon that chooses, every time, to heal instead of harm, stops being a weapon."*
- 📄 New canon: [`docs/story/24_THE_SIGNATURE_SYSTEMS.md`](docs/story/24_THE_SIGNATURE_SYSTEMS.md) (built
  vs. specced clearly marked). **73 walkable zones, 241 souls.** Gate **762** ✓ · full suite **1470** ✓
  · all-in-one bundle rebuilt ✓.

## 👑 v6.52.0 — *"The One Left"* — a seventy-second zone: the overlooked, and the weapon that is a victim (content)

> Off the Last Word's lot — the **western corner**, where the overlooked wash up: the souls the bright
> ones step over. Coll named it twice in *The Last Word* — *"there is always one left, by the western
> cairn."* **Here is that one.** The zone's thesis is the inverse of Sevrin's flaw: where a legend's
> greatness is too large to *look* at the small things, a soul who **looks** finds people where the
> magnificent found noise — *the very faculty that lets you save souls is the one Sevrin lacked, and
> it's why he dies and you might not.* Three fates of the overlooked: **made a weapon, made scenery,
> made nothing.**
- ◌ **Sift, the one left** — a soul the *same harvest that made you* tore loose, dissolving (awake, in
  agony) into the weapon that kills legends: its **name already taken by the Wall**, its unmaking routed
  through it and aimed at the bright by a hand it cannot stay. **The murder weapon as the most innocent
  soul on the field.** A **Persuasion** eases it — **crit:** gentle, witnessed, *sat-close*, the
  dissolving made bearable; **fail:** you reach for the soft lie and it rings false; **fumble:** you
  *flinch up close* and teach it the cruelest lesson — that even the kind can only bear it from a
  distance. A **[RETURNED]** refuses to let the Wall's truest victim believe it's a monster: *you're not
  the killer, you're the knife it's making out of a person — the Wall is the murderer; a monster doesn't
  mourn what it's becoming, only a person does.* (Easing Sift **counts toward your mercy** — but the loop
  is inexhaustible; *there is always another one left.* You can't save Sevrin. You can refuse to let his
  killer die un-witnessed.)
- ❒ **Bryn** *(Brynalla Coorne)* — saved by a bright legend, one-handed, barely-looked-at, and **never
  once seen** after; worn down across two lifetimes from a whole soul to *scenery*, a grateful face that
  made the winning feel better. A **[RETURNED]** reclaims the rescued life as **the soul's own, not the
  rescuer's** — *he gave you the years; he did not thereby own them* — and the player can ask the
  saved-and-forgotten its **whole name** and say it back, the first voice in two lifetimes that *wanted*
  to know it.
- · **Tace, who chose to be nothing** — watched a dozen legends die, concluded (correctly) that the loop
  **hunts the bright**, and so made itself *un-bait*: sanded off every spark, every catch, dimmed down to
  a grey nothing — and **survived** where eleven-lifetime legends are dust. The anti-Sevrin. A
  **[RETURNED]** grants that the hunt is real, then names the lie under the strategy: *self-erasure is the
  Wall winning without a fight; there's a third way between blaze-and-be-hunted and dim-and-be-nothing —
  a whole soul, unperformed, with its private inner fire (the part no hunter could ever see) still lit.*
  Tace dimmed that too — *did the Wall's work for it* — and the **banked coal under ten years of ash**
  begins, terrified, to stir.
- 📄 **72 walkable zones, 238 souls.** Gate **751** ✓ · full suite **1459** ✓ · all-in-one bundle rebuilt ✓.

## 👑 v6.51.0 — *"The After"* — a seventy-first zone: the scar (consequence content)

> **Realm-side** (off the Night Market) — the **aftermath** of *The Last Word*, and the proof that the
> saga's reactivity has teeth: **you cannot save Sevrin** (that's the lesson — no soul is good enough to
> outrun its own ungrieved grief), but **what you did in his zone decides who is left standing in the
> wreckage.** You return to the same lot, later — the whistling stopped, the cairns gone cold, his thin
> blade driven point-down in the earth as a grave-marker — to the **shattered court.** Every soul's
> opening reacts to the flags you set before the fall.
- ❉ **Sela, who got her fear back all at once** — she handed the unbeatable man her *whole* fear and was
  free; when he fell, it all crashed back into arms that forgot how to lift it. Her node-0 **reacts to
  the prior zone**: if you **made him promise** (`sv.watches_the_small`), he spent the half-second your
  warning bought him **shoving her clear** — dying to protect the small one exactly as his first death
  couldn't — and she must learn to carry *being saved like that;* if you **warned her** to take her fear
  back (`sv.sela_takes_it_back`), she's **standing, changed,** instead of destroyed. A **[RETURNED]**
  reframes the returning fear not as punishment but as her own **waking-up** — the one thing that was
  always hers; *the Wall waits for the souls who handed their fear away and stopped watching.*
- ❀ **Coll, the after** — who has been the one-left-standing a dozen times. **Reactive** to whether you
  believed him before the fall (`sv.two_clear_eyed` → *two* clear-eyed souls to be the after, vs. him
  alone again). A **[RETURNED]** names him not the *cloud* the courts hate but the **keeper of the dead's
  names** — the Wall erases the legends too, by scattering the grieving witnesses, and Coll is the one
  mind that refuses to let the fallen go un-marked. *The keeper, finally, kept.*
- ✦ **Tamsin, who waits at the door** — the **denial** soul, who will not believe a Returned can truly
  die (*"nine times — he always comes back"*). The hardest mercy in the lot: a **Persuasion**-gated
  **[RETURNED]** takes her down off the ceiling — the difference between a **death** and an **unmaking**
  (*the one death a Returned doesn't walk back from*). **Crit:** she grieves honest at last and goes to
  sit with Sela; **fail:** she clings to the thread; **fumble:** you break her *wrong* — too blunt, before
  she's braced — and she falls with no one holding her, *exactly as Coll warned.* Denial is a slow
  dissolving; waiting forever at a door no one is coming through.
- 📄 **Docs:** the consequence-engine in action — three souls whose grief is shaped entirely by your
  earlier choices. **71 walkable zones, 235 souls.** Gate **743** ✓ · full suite **1451** ✓ · all-in-one
  bundle rebuilt ✓.

## 👑 v6.50.0 — *"The Last Word"* — a seventieth zone: the false deus ex machina (content)

> **Realm-side** (off the Night Market) — a deliberate piece of **narrative architecture**, not just a zone:
> the saga's **false deus ex machina**, built to teach the player to **distrust relief** before Act V's
> Deathless Garden offers the ultimate relief. It's the *"Mentor Occupational Hazard"* inverted — the savior
> doesn't die *overwhelmed* and noble; he dies **mid-victory, mid-wink, uncomprehending**, the confidence
> still on his face as the light goes out. *Competence is the rope the scene hangs the audience with:* the
> more invincible he seems, the more the player hands him their dread — so the fall returns it with interest.
> Also fires three braided devices at once: **false deus ex machina**, **Chekhov's gun as red herring** (his
> reputation *"I have never lost"* is the loud gun that fires into his own chest), and the **"the loud answer
> is a lie"** thesis (the quiet gun — your accumulated mercy — was the real one).
- ⚔️ **Sevrin Vael, the Last Word** *(guest-star, not a companion — met here, killed in a later set-piece)* —
  a **Returned of nine lifetimes** who has *never once lost.* His confidence is **momentum**, not arrogance:
  in his first life he couldn't save someone small he loved, armored over the grief instead of healing it,
  and made himself **exactly the soul the loop knows how to kill.** A **[RETURNED]** warns that the loop
  sends not a worthy foe but *the small thing he stopped watching* (the warning lands; he deflects — the
  rare [RETURNED] that *doesn't* fully take, by design). A **Persuasion** asks the unbeatable man to
  **promise** to watch the small ones — **crit:** he sets the blade down and truly hears you (`watches_the_small`);
  **fail:** he charms past it; **fumble:** you wound his pride and he doubles down recklessly (`pride_wounded`).
  Every choice **reactively rewrites his death**: warn him → a half-second of grief-recognition that **feeds
  the Mournlight**; make him promise → he dies *shoving Sela clear*, redeeming his first death.
- ❉ **Sela, the Unafraid** — a soul he walked out of a swarm with a *wink*, who handed him not just her safety
  but her **capacity to be afraid**, and stands contented on ice she believes is stone. A **[RETURNED]**: a
  borrowed courage is a debt that comes due — *take some of the fear back while there's still someone to teach
  you*, so you survive the unafraid man's falling.
- ❀ **Coll, who has buried the unbeatable before** — the **fair-foreshadowing** the twist needs: an ordinary
  old soul who has watched legend after legend die *the same way* — not overwhelmed, **slipped**, through the
  one gap their greatness left open. His prior legend, **Haldane the Unbroken Blade** (eleven lifetimes),
  cleared a field, sheathed his sword, turned to speak — and died to the **one half-dead Unbound he stepped
  over**, the Wall's unmaking on its claw. The planted gun is in plain sight: *there is always one left, still
  twitching, by the western cairn.* A **[RETURNED]** names the loop a hunter of **greatness specifically** —
  the brighter the soul, the more the dark wants it.
- 📄 **Docs:** new deep dossier [`docs/characters/sevrin-vael.md`](docs/characters/sevrin-vael.md) with the
  full **"The Wink"** death set-piece (incl. the UI-betrayal staging) and the reactive death table; cast index
  gains a **guest-star** section. **70 walkable zones, 232 souls.** Gate **735** ✓ · full suite **1443** ✓ ·
  all-in-one bundle rebuilt ✓.

## 👑 v6.49.0 — *"The Last Lantern-Feast"* — a sixty-ninth zone: the road's one un-morose quest, and a hidden 8th companion (content)

> **Realm-side** (off the Lamplit Quarter) — the saga's **one un-morose side-quest**, and the cast's late-found
> **answer to the Deathless Garden.** A dead grey square lit up like a name-day, where a Baldur's Gate baker throws a
> **forbidden party for the doomed.** The thesis is the whole game's hinge: the Unmade's cruelest temptation is a
> *real* heaven with the dying quietly edited out — and the party needed one soul who has buried enough loved ones to
> feel, in her gut, **why a feast that never ends is a tomb.** *Take away the ending and you don't get eternal joy —
> you get a candle that won't burn down, which gives no light, because the light **is** the burning.*
- ✺ **Dot Marigold, the hearth-keeper** *(hidden 8th companion)* — three generations of the Marigold ovens; she buried
  everyone she loved on a rolling schedule and came out throwing **feasts** — *not despite the dying, because of it:
  the loaf matters because it goes stale.* Recruited by **defending her feast** from the church (a **Persuasion** check
  — **crit:** *feed the snuffer*, invite the grey sisters in, you can't put out a fire you've been asked to sit beside;
  **fumble:** *mock* the grey sister and prove joy cruel to her face). A **[RETURNED]** names her the **living
  refutation of the Deathless Garden** — *a feast that can't end isn't a feast, it's a room you can't leave* — and she
  joins, astonished anyone ever came for **her** party. Her answer to the saga's question: **joy.**
- ❀ **Marisa, who dances anyway** — a soul half-dissolved, days from nothing, who spent her whole careful life *saving
  up* for a right time that never came and means **not** to waste her death the way she wasted her life. A
  **Performance** dances with her — **crit:** the dance holds the dark at bay, a dying girl the *brightest light in the
  square*; **fail:** you flinch at her fading hands; **fumble:** you grieve her too early and she ends up comforting
  *you.* A **[RETURNED]**: *a joy that's running out is the truest joy there is — don't go out small; the burning IS
  the light* — the one thing the Wall can't compute, a soul that got brighter as it ended.
- ✝ **Sister Onora, who came to snuff the light** — not a brute; a true believer with a small brass snuffer and
  bottomless grief, who buried a **bright laughing sister who threw parties** and froze her own grief into doctrine
  (*the proper posture of the dead is patient sorrow*). The deepest read finds she is the feast's **most starved,
  forbidden guest** — every lit lantern a thing she never let herself have. A **[RETURNED]** names **grief-held-still-
  forever as the Wall's own logic** — a soul stuck in one shape, calling the freezing faithfulness — and she **thaws**,
  setting down the snuffer to *let her grief move, just a little,* and learn to dance again.
- 📄 **Docs:** new deep dossier [`docs/characters/dot-marigold.md`](docs/characters/dot-marigold.md) and the cast index
  updated to **eight** companions (the hidden one). **69 walkable zones, 229 souls.** Gate **726** ✓ · full suite
  **1434** ✓ · all-in-one bundle rebuilt ✓.

## 👑 v6.48.0 — *"The Wake the World Withheld"* — a sixty-eighth zone: the unmourned mark each other (content)

> **Realm-side** (off the Night Market) — and the most *on-theme* zone of the run: it's **Act V's Ledger at human
> scale.** The endgame's golden set-piece is judging the Wall's ten-thousand-year roll of the discarded *one soul at a
> time, by hand — each name a story.* This zone is that grief made intimate: a quiet candlelit lane where the
> **unmourned give each other the funerals the world denied them.** The thesis: **a wake is an act of war against the
> Wall — the Wall feeds on the *un-marked* (a soul no one grieved slides into nothing unnoticed); a soul someone marks,
> names, says *was here and is gone and worth pausing for,* becomes un-discardable. The grief is the guard.**
- ✦ **Corliss, who marks the unmarked** — a sexton who stood alone at a pauper's pit and vowed no soul she could reach
  would go into the dark un-named. A **[RETURNED]** names her candles a war on a machine that erases people — *one
  forgotten name at a time.*
- ❖ **Tatters, the unmourned pauper** — a gutter-sparrow who fed the cats and hummed to frightened children and died in
  a doorway with *no one came.* A **Performance** delivers his eulogy — **crit:** the *true small specifics* (the cats
  by name, *a kind one*) land, the room passes his name round, and the sixty-year hollow *fills*; **fail:** grand-but-
  generic (*honors the type, misses the man*); **fumble:** you make it about *your own* grief and the room comforts
  *you* while he sits forgotten at his own funeral. A **[RETURNED]**: *he wasn't un-mournable, only un-mourned — the
  song was always there; someone just finally sang it.*
- ✶ **Brenn, who refuses to be mourned** — did real wrong, knows it, has never lied about it, and calls the silence
  *justice.* The hardest case: you can't absolve them (they'd reject it, rightly). A **[RETURNED]** instead **pries
  apart 'deserving forgiveness' from 'deserving to be marked'**: existing was never earned by being good; *the worst
  soul was still **here**, and a soul that was here gets the candle — not because it was good, but because it was real.*
  Brenn takes the candle — not as forgiveness, *as true.*
- 👁️ **Three new Returned-senses.** 🧪 **+9 gates** (**gate suite 716 / 1,424 total**, all green). **68 zones, 226
  souls.** *You cannot opt out of having existed.*

---

## 👑 v6.47.0 — *"The Intake Queue"* — a sixty-seventh zone: dark-comic bureaucracy with a knife in it (content)

> A hard **tonal swing** after a long run of earnest grief — **curse words for spice, crit/fumble comedy**, a
> Pratchett-by-way-of-Kafka farce — that turns, as the best comedy does, into a gut-punch. **Realm-side** (off the
> Count-House, *not* a Sigil door, so the omniverse capstone stands untouched): the afterlife's **intake queue**, where
> the dead wait an eternity to be *processed* — and the joke becomes the horror: the three souls at the front are each,
> in their own way, **holding the door shut.** The thesis: **the Wall is the biggest clerk there is — it turns souls
> into cases, numbers, the saved and the discarded, *processed*, so that no one (not even the Wall) ever has to look at
> a person and decide, with their own heart, that they're worth something. The form is the cruelty. You cannot process
> a soul. You can only see one.**
- ✦ **Mabblethwaite, the clerk** — who has *never once exceeded his authority* and is proud of it, and has called
  *eleven* numbers in an age, because eleven is how many he could be *certain* of. A **Persuasion** can get him to act
  with no rule — **crit:** he puts the pen down, *sees a person* for the first time, and judges (*God, that felt strange
  to say*); **fumble:** you try to *deceive* a clerk (the one thing his whole life is a fortress against) and he files
  you a fresh form, delighted. A **[RETURNED]** names the Wall the biggest clerk — *cruelty with no author, so no one is
  ever to blame.*
- ❖ **Grindle, the furious** — foul-mouthed, raging that the line never moves, *desperate* to reach the front and face
  his reckoning like a man… who flinches and waves the next soul ahead *every single time* his number is called. A
  **[RETURNED]** names the rage a *guard he posted on the door*: he's not kept from the front, he's *hiding at it*,
  terrified of a verdict on a life he knows won't weigh well — *the Wall runs on souls so sure they'll be damned they
  damn themselves and never come to be weighed.*
- ✶ **Posset, who will not advance** — a nan knitting an endless scarf, the most content soul in the room, who declines
  the front *seventeen times.* Not fear — **she made a family of the waiting-room** after losing everyone, and can't
  bear to leave her second household for the loneliness on the far side. A **[RETURNED]** names the limbo-made-home the
  *gentlest trap there is* (a waiting-room a soul loves is one it never leaves) — and she finishes her scarf and lets
  her number be called, toward a house full of people who've waited even longer than she has.
- 👁️ **Three new Returned-senses.** 🧪 **+9 gates** (**gate suite 707 / 1,415 total**, all green). **67 zones, 223
  souls.** First **Realm-side** zone of this run — the omniverse's 23 Sigil doors are complete and the capstone stands;
  the town's own lanes still have room. *You cannot process a soul. You can only see one.*

---

## 👑 v6.46.0 — *"The Far Doors Echo, Whole"* — the full omniverse lands in the ending (reactivity)

> A cohesion pass so the *entire* 23-door journey pays off in the finale, not just the early doors. The Chronicler's
> `epi_sq_beyond` branch — *"the far doors… what did I carry back from them?"* — now reacts to **all** of them, and leads
> with the **capstone**: if you walked the last threshold and *became the witness*, the epilogue names that the true
> ending — *not the Wall broken, but the mercy you were given flowing through you to the next frightened soul through
> the door; the chain does not end with you, it begins again.*
- 🪶 **+8 reactive Chronicler variants** (first-match-wins, capstone highest): **the Newly Dead** (you became the witness;
  mercy outlives everything), **the Unseen** (you credited the uncredited; nothing shines by itself), **the Forbidden**
  (the Wall a false judge of love forfeits the bench), **No Man's Land** (a Wall is only ever people kept apart), **the
  Sounding Court** (worth was never permanence), **the Last Job** (case the Wall like the rigged ledger it is; burn the
  book), **the Little Ones** (worth was never doing, but being, and being loved), and **the Reunion** (the deepest
  cruelty was separation; its undoing is a witness who walks the distance). `epi_sq_beyond` now spans **17 variants**
  covering all twenty-three doors.
- 🧪 All gates green (**1,406 checks**, full ladder passing). No new zones — pure reactivity; the journey across the
  planes is now, end to end, part of the saga's spine.

---

## 👑 v6.45.0 — *"The Newly Dead"* — a sixty-sixth zone: the CAPSTONE, where the loop closes (content)

> The **capstone** of the whole omniverse arc. After twenty-two doors in which soul after soul witnessed, steadied, and
> changed the Returned, the *last* door the player finds turns out to be the *first* door every soul passes through —
> the threshold of arrival — and here the structure **inverts**: the player, witnessed across a hundred griefs, *becomes
> the witness.* Three souls have just crossed (terrified, raging, worried for the living), and the player gives each the
> exact thing they were given. The thesis that completes the saga: **you were witnessed so that you could become a
> witness; mercy received becomes mercy given; and that passing-on — not any single victory — is how worth outlives the
> Wall. We pass it on.** Each soul's opening beat **reacts to the player's journey** (a Returned who has gathered mercy
> across the doors radiates a calm the newly-dead reach for).
- ✦ **Nim, just crossed and terrified** — huddled in the dark, certain it's hell-forever-alone. The player gives the
  three things they were given (*you're not alone; it passes; here's my hand*) and Nim, minutes dead, kindles with the
  whole secret: *one day I'll be the calm one, and pass it on.* A **[RETURNED]** names the chain.
- ❖ **Dorn, just crossed and refusing it** — clawing at a one-way door, raging to go back and unsay a cruelty to a
  daughter. The player uses everything the doors taught about meeting fury (Nettle, the soldiers, Rook): a **Persuasion**
  **crit:** *met, not managed* — the love isn't trapped behind the door, it crossed over with him; **fumble:** the
  *"accept it"* peace-voice (the one thing he warned against) walls him up twice as hard.
- ✶ **Elga, just crossed and only afraid for the granddaughter she raised** — at peace with her own death, undone by
  Senna's. A **[RETURNED]** (drawing on Pae's lullaby) names the love that doesn't die with us: *she put herself inside
  the girl across fifteen years, where no door can take her* — and names the player **the grandmother of the newly dead.**
- 👁️ **Three new Returned-senses** (each a mirror in which the player recognizes their own first moment). 🧪 **+9 gates**
  (**gate suite 698 / 1,406 total**, all green). **66 zones, 220 souls. Twenty-three doors — Sigil's lanes are full.**
  *The mercy was never meant to stop with you. It was always meant to flow through you, to the next frightened soul through the door, forever.*

---

## 👑 v6.44.0 — *"The Unseen"* — a sixty-fifth zone: the invisible labor that held the world up (content)

> A dignifying door on **the unseen work of care** — the souls whose invisible, unthanked labor actually held everything
> up while the world looked only at the thrones. The thesis: **the Wall (like the world) credits the *visible* — the
> grand, the named, the shining — and is blind to the foundation beneath; nothing shines by itself, and the world was
> never held up by the ones on top. It was held up by the ones no one looked at.**
- ✦ **Tilda, the housekeeper** — who ran a great house (a staff of forty, the whole hidden machine) for fifty years so
  well that the grand lives upstairs believed bread simply *appeared.* The art of her trade was to make the labor
  *vanish* — so the better she did it, the *less she existed.* A **[RETURNED]** names her not the absence of a great
  life but the *foundation* a dozen great lives stood on: *the foundation is worth more than the whole house it carries.*
- ❖ **Garrick, the gong-farmer** — who carried away a city's nightsoil so it wouldn't drown in its own filth, and was
  taught, cruelly, that *he* was the filth. A **Persuasion** lifts the world's contempt — **crit:** the filth was never
  *his* (he scrubbed at the wrong thing all along; he was *clean*), and he sits up straight at last; **fumble:** an
  involuntary *flinch* he reads instantly (*you see a trade, not a man*). A **[RETURNED]** names the Wall as despising
  *what it cannot bear to need.*
- ✶ **Wennick, who tended the dying** — who walked *into* the rooms everyone fled (the plague-struck, the foul, the
  abandoned) so no one would die alone, and learned every soul at the end wants the same three things: *not to be alone,
  to be forgiven, to know they mattered.* Caught the fever at a bedside and never regretted it. A **[RETURNED]** names
  the Wall a **machine for abandonment** — undone not by force but by *walking in and staying.*
- 👁️ **Three new Returned-senses.** 🧪 **+8 gates** (**gate suite 689 / 1,397 total**, all green). **65 zones, 217
  souls. Twenty-two doors.** *Look down — at the ones no one looks at — and you'll find, every time, they were holding the whole thing up.*

---

## 👑 v6.43.0 — *"The Forbidden"* — a sixty-fourth zone: the loves the world condemned (content)

> A bittersweet door on **love, and the world's right to judge it** — three souls who are three relationships to a
> thwarted love, and a clean indictment of the Wall as the world's false verdict grown cosmic. The thesis: **the world
> appoints itself the judge of which loves are *allowed* and which *forbidden* — and it was wrong every time it said no;
> a love's worth was never whether the world sanctioned it, whether it lasted, or whether it was returned, only whether
> it was *real* — and the Wall is that same false judge, condemning souls by lines it had no right to draw.**
- ✦ **Rell, who chose duty** — gave up a love to keep a great heavy correct office, was *praised* for it, and has
  grieved it in *secret* ever since, ashamed, because the world that honored the choice gave him no leave to mourn its
  cost. A **[RETURNED]** gives him the permission no one would: *an honorable choice is allowed to break your heart —
  the breaking is the proof the love was real* — and names the Wall the same machine that shames the cost of correctness.
- ❖ **Mirae, who chose love** — loved across a line the world held sacred, was offered *the love or everything*, chose
  the person, and lost all the world could take — and would do *every bit of it again.* A **[RETURNED]** names the world
  a **false judge of love** (everything it took was rent on a life that was never hers; the love was the only thing that
  was), and the Wall the judge that *forfeits the bench* by calling the truest love a sin.
- ✶ **Wynn, who never said it** — loved someone for years, quietly, completely, and never dared to speak, saving the
  words for a perfect moment that never came. A **Persuasion** eases the unsaid — **crit:** the love was *real, only
  unwitnessed* (a love unheard is still a love), freeing it; **fumble:** *"you should have said it"* hands the grief
  back heavier. A **[RETURNED]** carries their silence to the living *as a warning — say it, today, whatever it costs* —
  and names the unspoken love the only kind you lose *completely.*
- 👁️ **Three new Returned-senses.** 🧪 **+8 gates** (**gate suite 681 / 1,389 total**, all green). **64 zones, 214
  souls. Twenty-one doors.** *Don't let the perfect moment save your words all the way to never.*

---

## 👑 v6.42.0 — *"No Man's Land"* — a sixty-third zone: two soldiers who killed each other, and the war the Wall is made of (content)

> A morally-rich door on **war, sides, and the lie of the enemy** — and the clearest statement yet of *what the Wall
> structurally is.* Two soldiers who killed each other in the mud, an age ago, still keep their line in death — each
> frozen by a soldier's pride, neither able to cross first. The player walks between them, carries each one's *home*
> across to the other, and discovers they are the same boy in different paint. The thesis: **the Wall is the last
> battlefield — it divides souls into sides (worthy/unworthy, owed/owing) and survives only as long as the divided fight
> each other across the line instead of turning, together, to face the power that drew it. A Wall is only ever made of
> people kept apart.**
- ✦ **Ander** (blue, a barley-farmer) and **Marek** (red, a fisherman) — mirror souls, same fear, same waiting girl
  (Sera / Lize), same stolen autumn, same lie poured in the ear: *they don't send men to kill men — they paint the
  monster onto the boy.* Each carries an always-available **[RETURNED]** witness.
- ✶ **Wenna, the field-surgeon of the middle** — who had her hands in *both* their hearts and learned they bleed the
  same: every dying boy, blue or red, calls for his mother, says a girl's name, and asks *am I going to die.* A
  **[RETURNED]** names the Wall the last battlefield and its cure the same as a war's: *not victory (that just moves the
  line) — the soldiers crossing it together.*
- 🔗 **Cross-soul reconciliation (new):** once you've opened *both* boys (carried each one's home), a **Persuasion** at
  Wenna's no-man's-land brings them across — **crit:** you speak each boy's home to the other's face (never *"forgive"*,
  which makes one the criminal) and the line dissolves into brotherhood; **fumble:** you say *"forgive him"* and split
  them onto sides again (peace, not a truce that breaks). Reconciling rewrites all three souls' opening beats.
- 👁️ **Three new Returned-senses.** 🧪 **+8 gates** (**gate suite 673 / 1,381 total**, all green). **63 zones, 211
  souls. Twenty doors.** *There are no devils. There was only ever us, the paint, and the hand that held the brush.*

---

## 👑 v6.41.0 — *"The Sounding Court"* — a sixty-second zone: music, and what doesn't have to last to matter (content)

> A new register — **art and beauty** — through three musicians who frame the same question three ways: *does a
> beautiful thing have to endure to have been worth the making?* (The answer is no.) The thesis: **the Wall weighs what
> can be held and kept — and the most precious things a soul ever makes can't be held at all; worth was never about
> permanence, but about being, fully, while you are.**
- ✦ **Pae**, a poor mother whose made-up lullaby outlived her by ten generations and is *still being sung* tonight by
  people who never knew her name. An **Insight** read finds a soul *distributed* — still doing her love, in the living
  world, by the thousand. A **[RETURNED]** names a soul that survives in a tune the Wall *can't weigh*: the name was the
  label, the love was the cargo, and the song carried the cargo and dropped the name as unnecessary weight.
- ❖ **Cale**, a true composer who made one perfect piece — *the sound of a beautiful thing ending and being glad it
  existed* — and never wrote it down, losing it to death. A **Performance** can *learn it and carry it back to the
  living* — **crit:** preserved exactly (*it's called The Glad of It*), and he's freed; **fumble:** *"near enough"*
  flattens it — worse than lost (a counterfeit crowds out the true one forever). A **[RETURNED]** names a beauty
  witnessed even once — written not on a page but into every soul in the room — the realest thing the ledger can't see.
- ✶ **Wisp**, a street musician who played a thousand songs to passing strangers, kept nothing, and *knew* it was all
  smoke — and played anyway, with the whole heart, *because* it was passing. An **Insight** read finds a bedrock peace
  with impermanence chosen young. A **[RETURNED]** names the un-lasting things the *most* alive — the sunset, the wave,
  the single afternoon — and the Wall the greatest *saver* of all, blind to the whole kingdom of worth that can only be
  spent, never kept.
- 👁️ **Three new Returned-senses.** 🧪 **+8 gates** (**gate suite 665 / 1,373 total**, all green). **62 zones, 208
  souls. Nineteen doors.** *The playing was the having. There was never going to be more than that — for any of us.*

---

## 👑 v6.40.0 — *"The Last Job"* — a sixty-first zone: a heist crew, and the biggest rigged ledger there ever was (content)

> A deliberate **tonal lift** after a run of tender doors — wit, banter, and caper energy on top of a buried grief. A
> thieves' bolthole where a crew that ran *one good con* against a slaver-magistrate lies low forever. The job is
> remembered from three angles, and the player assembles what they *really* stole (not gold) and what it cost. The
> thesis recasts the whole endgame as a heist: **the Wall is the biggest rigged ledger there ever was — a vault of
> souls, taken and called *justice*, guarded by a reputation that says it can't be cracked; and a rogue who robbed the
> cruel to free what they hoarded always knew the law itself was the crime, dressed up safe.**
- ✦ **Vesper, the Face** — the charmer who chose her marks (never the poor, only the cruel), whose patter is load-bearing
  armor over a code as strict as any knight's. A **[RETURNED]** names the Wall the finest mark ever described — *every
  vault that says it can't be cracked is lying; the "can't" is the lock.*
- ✶ **Greaves, the planner who stayed** — who held a one-way gate so the crew could get out, did the math cleanly, and
  has been at peace ever since. What they stole: a **slaver's ledger** — the book that made owning people *lawful* —
  which they *burned*, and hundreds woke owned and slept free. A **[RETURNED]** names the Wall the same vault, bigger:
  *leave the gold; find the ledger that makes its cruelty legal, and burn it.*
- ❖ **Rook, the Hands** — the genius cracksman who couldn't pick the one lock that mattered (the past), and chose *guilt*
  over the unbearable truth of *helplessness.* A **Persuasion** can lift it directly (**crit:** naming the mechanism
  frees him; **fumble:** *"noble sacrifice"* turns it into a debt). And a **cross-soul** path: carry **Greaves' own
  plain word** to him — held *steady* however he argues — and the one lock in the whole bolthole finally turns.
- 👁️ **Three new Returned-senses.** 🧪 **+8 gates** (**gate suite 657 / 1,365 total**, all green). **61 zones, 205
  souls. Eighteen doors.** *Some doors aren't locks. Some losses aren't debts. Some things just cost, and no one owes.*

---

## 👑 v6.39.0 — *"The Little Ones"* — a sixtieth zone: the children who came too soon, and the ledger's nakedest fraud (content)

> The most direct answer the game has yet made to *"a soul's worth is not its deeds."* A lamplit green where the
> children who came too soon are kept warm — handled **bright and fierce, not maudlin**: these are vivid, furious,
> tender, alive-in-death small people, and they are the souls against whom the Wall's whole ledger is most nakedly a
> fraud, **because they never got to *do* a single thing — and every heart in all the worlds already knows they were
> worth the entire sky.** The thesis: **if the most "worthless" soul on the ledger is the one every heart knows is most
> precious, the ledger was never measuring worth — only deeds; and worth was always about *being*, and being *loved.***
- ✦ **Brigid**, a midwife who caught the young for forty years and now keeps the green — who does not lie to the little
  ones (*don't do the gentle-voice; believe them; take their feelings seriously*). A **[RETURNED]** names the children
  not the Wall's failures but its *exposure.*
- ❖ **Nettle**, nine, *furious* — and **right** to be. The fresh beat: you do **not** soothe her. A **Persuasion** meets
  her fury *with agreement* — **crit:** for the first time she is not the only one angry, and it stops being lonely;
  **fumble:** *"a better place / it happened for a reason"* shuts the door cold. A **[RETURNED]** names her anger her
  *integrity* — the last part of her that still knows she was worth a whole life.
- ✶ **Lyssa**, who did her own grieving and now frets only for her mother, still crying, *not knowing her girl is all
  right.* A **[RETURNED]** takes her message to carry to the living — and names a child who *gives* (comfort, across
  death, asking nothing) the Wall's whole ledger inverted. (*Give me back my mother's singing.*)
- ✧ **Pim**, the littlest, here longest, *fading* — because no one in the living world remembers a Pim anymore. A
  **[RETURNED]** promises to carry her name back and *say* it (remembering reverses fading); she learns *counting was
  never about doing — it was about being held in someone's remembering.*
- 👁️ **Four new Returned-senses** (a four-soul zone). 🧪 **+8 gates** (**gate suite 649 / 1,357 total**, all green).
  **60 zones, 202 souls. Seventeen doors.** *They don't fail the test. They expose it.*

---

## 👑 v6.38.0 — *"The Reunion"* — a fifty-ninth zone: two souls who lost each other, and the Wall's true opposite (content)

> A **structurally new** door — the first built on **cross-soul reactivity**, and a deliberate answer to what the Wall
> actually *is.* A wide plaza of the *lost-and-searching*: souls who came through death hunting for someone and never
> stopped. Two of them — a wife and her husband — have searched *this same plaza* for an age, passing within arm's reach
> a hundred times, and never once known it, because **grief makes you search for the face you buried while the real
> face, changed by an age of looking, moves through the same crowd searching back.** You can see across that gap. You
> can carry the proof between them. You can walk them home. The thesis: **the Wall's deepest cruelty was never judgment —
> it was *separation*; and its undoing is not a kinder judge but a witness who walks the distance the parting made.**
- ✦ **Mirren**, who has searched for her husband for an age — and **Edran**, who swore on her deathbed he would *find*
  her, and has kept that one promise every hour since. Each gives you a **private proof-token** — a song hummed wrong,
  a secret word no other soul in all the worlds could know — the lock love builds around itself.
- 🔗 **Cross-soul mechanic (new):** carrying *Edran's* proof unlocks the choice to bring *Mirren* to him; carrying
  *Mirren's* unlocks the reverse. Speak the proof, and an age of wariness falls in an instant — because the token is the
  one thing the false hopes never had. The reunion fires from whichever you reach *second*, and both searchers' opening
  beats change forever after. Each also carries an always-available **[RETURNED]** witness (a love that searches an age
  unrewarded is the realest thing the ledger can't weigh).
- ✶ **Halia, the Usher of the Lost** — who *sees* every match as a thread of light and is bound by her office never to
  *point*, and has waited an age for a soul outside her law. A **[RETURNED]** names the Wall the *engine of separation*,
  and its fall **not broken but un-parted — one reunion at a time.**
- 👁️ **Three new Returned-senses.** 🧪 **+8 gates** (now **gate suite 641 / 1,349 total**, all green). **59 zones,
  198 souls. Sixteen doors.** *No one searches this hard, this long, for someone who isn't searching just as hard, back.*

---

## 👑 v6.37.0 — *"The Far Doors Echo"* — the omniverse lands in the ending (reactivity)

> Not a new zone — a **payoff.** After fifteen Sigil doors onto worlds beyond the Wheel, the side-roads now *echo in the
> finale.* The Chronicler's epilogue gains a new branch — *"The far doors… what did I carry back from them?"* — that
> reacts to the omniverse doors you actually walked and ties each lesson directly to the Wall's fall, so the journey
> across the planes is no longer parallel optional content but part of the saga's spine.
- 🪶 New reactive Chronicler node **`epi_sq_beyond`** (9 variants, first-match-wins by what you accomplished): the
  **Faithful** (a love with no deeds the ledger was too grand to see), the **Sunblasted** world (you planted a seed in
  the Wall's own ground), the **First Delve** (you understood what *you* were — the first to cross back), the
  **Lanceward** hall (a love that kept no score the ledger couldn't price), the **Long Midnight** (a soul is its
  cultivation, not its curse), the **Far Roads** (the Wall was grading the wrong exam), the **Menagerie** (*"are you all
  right in there"* — the question that un-makes a Wall), and the **Re-Written** (canon and true are not the same word) —
  each in the Chronicler's voice, each tying the door's thesis to what you carried home. Plus an honest default for souls
  who kept to their own Realm.
- 🧪 All gates green (**gate suite 633**, full ladder passing). **58 zones, 195 souls, fifteen doors** — unchanged; this
  is reactivity, not content. *The doors were never a side-show. They were the long way of learning the one thing.*

---

## 👑 v6.36.0 — *"The Faithful"* — a fifty-eighth zone: the beasts who loved us, and the love the Wall can't weigh (content)

> The most tender door in the game, and a near-capstone that strips the whole thesis to its core. A door of the
> *animals who loved us* — read through the player's dead-touched sense, which at last lets them understand what the
> beasts always felt and never could say. These are the beings the Wall **cannot even conceive of pricing** — no deeds,
> no words, no debts — and their wordless, unconditional love is the purest refutation of its entire ledger. The thesis:
> **worth was never what you did; it was how you loved — and nothing in all the worlds ever loved more truly than a
> faithful animal.**
- ✦ **Hob, the dog who waited** — who went to the gate every gold evening for a master who died far away, and waited,
  past the end of his legs, never understanding death, only absence. The player can give him the truth — a **Persuasion**
  wrapped in love: **crit:** he understands it was *not his fault*, and sets the waiting down in peace; **fumble:** the
  truth lands bare and he believes he was *unloved*, and breaks (he can only carry the heavy shape if it comes warm). A
  **[RETURNED]** names the faithful love at an empty gate the purest there is — and a Wall that finds no *deeds* in it
  is measuring *size*, not worth.
- ❖ **Brand, the warhorse who did not run** — a *prey animal*, built across ten thousand generations to flee fire, who
  stood in the screaming line of battle again and again, *afraid every time* (the brave feel all of it; they simply love
  something more). An **Insight** read finds only love can override an instinct that deep. A **[RETURNED]** names it the
  truest oath there is — *kept wordlessly, in the body, in the fire* — which no sworn knight's vow can match.
- ✶ **Mote, the familiar who chose the unchosen** — a raven clever enough to bind anywhere, who *chose* the feared,
  cast-out, lonely soul the whole world had barred its doors against — and was *right*, and proved it (the winter she
  healed the child of the village that feared her, asking nothing). A **[RETURNED]** names the world's verdict on who's
  worth loving **a fear dressed as a judgment**, and the Wall its lawful form: *choose the unchosen — it is the one
  thing that has ever proven the Wall a liar.*
- 👁️ **Three new Returned-senses.** 🧪 **+8 gates** (now **1,341 checks**, all green). **58 zones, 195 souls.** Sigil
  now opens **fifteen doors.** *There was never any "just" a dog, "just" a horse, "just" a bird. There was only the love.*

---

## 👑 v6.35.0 — *"The Sunblasted Door"* — a fifty-seventh zone: a world drunk dry, the dark mirror of the Wall (content)

> The most thematically central door yet — a **Dark Sun**-flavored homage to a world *already* dead, the wizards having
> drunk it dry for power, leaving a grey waste where survival itself excuses every cruelty. It is the **dark mirror of
> the player's own dying Realm**: a planet-scale proof of where the Wall's arithmetic leads. The thesis: **the Wall's
> "hoard, take, spend the future, owe nothing" is the *defiler's* logic written across a whole world — and the soul that
> still gives, still refuses to burn what can't regrow, still remembers it was green, is the only kind worth saving.**
- ✦ **Tamar, who gave her water** — on a world where water *is* life, she gave hers to a dying stranger and died of it,
  carrying one worry: that the stranger wasn't *worth* it. A **Persuasion** frees her — **crit:** giving is conditional
  on *nothing* (the thirst, not the merit), and she sets the worry down; **fumble:** you make it about *her own
  nobility* and she recoils (the stranger was never a prop for her virtue). A **[RETURNED]** names the Wall the burning
  world's iron rule — *hoard, give nothing back* — and the hoarders who balanced their books and died on a dead world.
- ❖ **Kessa, who would not burn the world** — a sorcerer who refused *defiling* magic (which rips the life from the land
  for power, leaving ash that never grows) for fifty years, weak and poor while lesser souls became kings. An **Arcana**
  read finds the cost was the *invisibility of restraint* — a world not killed leaves no monument. A **[RETURNED]** names
  the Wall **the greatest defiler of all: burning the worth of the unclaimed dead to fuel the order of the powerful, and
  calling the ash justice.** (The defilers won every fight and inherited a corpse.)
- ✶ **Oru, who remembers the green** — the last of a chain of rememberers who held, across the grey ages, the heretical
  memory that the world was once *green* — *because a world that forgets it was ever green can never want to be green
  again.* A **[RETURNED]** names a memory of green a **seed** — the one thing a ruin that calls itself *eternal* cannot
  abide — and the Wall's deepest wish (to seem natural, fixed, *how things are*) the very thing the rememberer defeats.
- 👁️ **Three new Returned-senses.** 🧪 **+8 gates** (now **1,333 checks**, all green). **57 zones, 192 souls.** Sigil
  now opens **fourteen doors.** *Give when the world says keep; spare when it says burn; remember when it says forget.*

---

## 👑 v6.34.0 — *"The First Delve"* — a fifty-sixth zone: the origin of adventuring, and the loneliest courage (content)

> A love letter to where it all began — a **Greyhawk**-flavored door onto the *origin*, the first souls who went into
> the unknown when there was no map, no precedent, no proof it could be survived. And the door that ties most directly
> to the player themselves: **you are the Returned — the first soul to cross *back* through death's one-way door** — and
> so the one soul in all the planes who truly understands these three. The thesis: **someone has to be first — to step
> into the dark with no proof it can be done; being first is the loneliest courage, because it alone has no
> reassurance, and everything that comes after stands on it.**
- ✦ **Vaeris, the first between worlds** — one of the first to cross to another plane and *return*, turning a wall into
  a door for every mage who would ever follow. An **Arcana** read finds her terror was not of dying but of *dying
  unwitnessed* — that a failed first crossing becomes the *proof it can't be done.* A **[RETURNED]** names her the
  *wall becoming a door*, and the player's crossing-back the very same act: *go back through your Wall, prove by
  surviving that what it calls final is only a door no one had dared — then write it down, hold it open.*
- ✶ **Tobb, the first down** — not a hero: a *well-digger* who took the first lamp into the dark under the world, after a
  lost child, because he *couldn't not.* An **Insight** read finds that the root of all delving *had* to be ordinary —
  proof that the unprecedented courage lives in the most ordinary heart and wakes when love leaves no choice. A
  **[RETURNED]** names it: *heroism was never about being special; the Wall throws away exactly the kind of soul that
  took the first step down.*
- ❖ **Hadda, the first to fall** — the first adventurer ever to go down and *not come up*, who has carried for an age
  the terror that her death only ever taught others *fear.* A **Persuasion** can lift that weight — **crit:** you refuse
  the word *worth it* and name what her death actually *did* (made the courage of everyone after *real*), and she can
  finally set it down; **fumble:** you call it a *sacrifice* (a gift she never chose to give) and reopen the wound. A
  **[RETURNED]**, spoken by a soul who *also* crossed death's dark, names her **the foundation, not the failure — the
  price that made every crossing after her courage instead of luck.**
- 👁️ **Three new Returned-senses.** 🧪 **+8 gates** (now **1,325 checks**, all green). **56 zones, 189 souls.** Sigil
  now opens **thirteen doors** (two now on the side lanes).

---

## 👑 v6.33.0 — *"The Far Roads"* — a fifty-fifth zone: the wanderers' rest, where the Wall asks the wrong question (content)

> A deliberate turn back into warmth and light after the grief doors — a filed-off love letter to **Pathfinder** and its
> wide, pulpy, open-road heroism: crusaders and goblins and wanderers, a campfire at the resting-place of the
> far-travelled. And the door that most directly indicts the Wall's whole arithmetic. The thesis: **the Wall asks how
> you ended and what you're owed — the wrong question entirely; a soul is the road it walked and the ones who walked it
> beside.** Three souls at peace, each carrying not a grief but a single question the player can finally answer.
- ✦ **Iona, the hundred-year wall** — a crusader who held a wound in the world (a demon-tide at the edge of reality) for
  *forty years*, knowing it would never close: *they fought not to win but to hold.* An **Insight** read finds she
  escaped futility's rot by refusing the long view and measuring the work by the *day* — fourteen thousand small
  complete victories. A **[RETURNED]** names the Wall's *"did you win?"* the wrong question: **the holding was the
  victory, complete every single day, never owed to a triumph that never came.** (And the company, not the cause, was
  the reason she stood up to do it again.)
- ✶ **Squik, the happy goblin** — knee-high, fire-loving, dog-fearing, gleefully and *unrepentantly* a goblin, with no
  tragic backstory and no secret wound — just a *great* little life and one good friend. The levity beat with a heart: a
  **Performance** check sings its song *with* it — **crit:** glorious, full-throated, it writes you into the song
  forever; **fumble:** you do *"the voice"* and make it the punchline (the thing it's spent forever being). A
  **[RETURNED]** names **joy its own worth — you don't need a sad story to matter.** (Squik thinks it's here as
  *evidence*: that a happy little nothing-soul *counts too.*)
- ❖ **Dalka, who went and saw** — a wanderer who walked the whole wide world for sixty years with no destination, dogged
  by the frown-question *"where are you going?"* An **Insight** read finds the next-ridge hunger was never restlessness
  but a *form of love* for the variousness of creation — the seeing was the prayer. A **[RETURNED]** names the Wall's
  *"where did you arrive?"* the wrong question: **there is no there; the journey was the worth, and you were arriving
  constantly, every step.** (She sends you off with a wonder — the northern sky dancing over a black sea, for no one —
  and the counsel that death's grey wall is just another ridge, with a horizon past it.)
- 👁️ **Three new Returned-senses.** 🧪 **+8 gates** (now **1,317 checks**, all green). **55 zones, 186 souls.** Sigil
  now opens **twelve doors** (the first onto the left lane, the bottom row being full).

---

## 👑 v6.32.0 — *"The Long Midnight"* — a fifty-fourth zone: the night-cursed, and the wolf you feed (content)

> A tonal turn into modern gothic — a door of guttering lamplight and old blood, a filed-off love letter to the
> **World of Darkness** and its central question: *you are a monster by no choice of your own — so what, then, do you
> choose to be?* The thesis: **you are not the hunger you were given; you are the thing you choose to feed instead —
> and what you feed grows, while what you starve goes quiet.** Three souls who are the *same creature* at three points
> of one long night: the start of the curse, the end of it, and the middle road that proves the end is a *choice.*
- ✦ **Mireille, the newly cursed** — three weeks turned, starving rather than feed on a person, certain she's *losing*
  because it hurts. An **Insight** read finds the opposite: the Hunger is *thinner* than it was, and the hurt is the
  *sensation of it starving.* A **Persuasion** can *anchor* her through a Hunger-spike — **crit:** you stay until the
  wave *breaks* and she learns to believe in the *after*; **fumble:** you close the distance to comfort her and the
  Hunger *lunges* (the distance was the kindness). A **[RETURNED]** reframes the war: *not a fight you're losing to a
  stronger thing — a garden, where what you water grows. You're feeding the girl. You're winning.*
- ❖ **The Forgotten, the worn-smooth elder** — so old the Hunger has eaten its loves, its languages, its *name*, leaving
  appetite wearing a person's shape; it walked the doors on a longing it can no longer name. An **Insight** read finds,
  at the bottom of the worn-out well, a *seed* — the irreducible self, starved past speech but *alive*, the one thing
  the Hunger could never reach. A **[RETURNED]** names the starved self *not gone, only waiting to be fed* — and *it is
  never too late to start watering the other thing.* (It is the warning, not the fate; the road is ten thousand separate
  nights, and the young one can refuse it.)
- ✶ **Mathias, who stayed human** — as old as the elder, the *same* curse, the same Hunger that never quiets — and still
  a man. Not by strength (everyone's fails), but by *out-growing* it: a single rule planted early as a *root* that holds
  when willpower (the *trunk*) can't, and *one warm thing he'd never let the curse make cold.* A **[RETURNED]** names the
  whole truth the door is built on: **a soul is not its curse; it is its cultivation** — and the far end of the long
  night can look like a *person.*
- 👁️ **Three new Returned-senses.** 🧪 **+8 gates** (now **1,309 checks**, all green). **54 zones, 183 souls.** Sigil's
  bottom row is now **full — eleven doors.**

---

## 👑 v6.31.0 — *"The Lanceward Hall"* — a fifty-third zone: Dragonlance, where heaven went silent (content)

> A door under a faded blue banner, smelling of cold iron and woodsmoke — a love letter to **Dragonlance**, and the
> cleanest fit yet for this game's own thesis. Dragonlance's founding grief is *the gods withdrew and went silent, and
> mortals had to decide whether to keep being good with no heaven watching* — which is precisely the Wall's question
> about the godless and the unclaimed. The thesis: **goodness done when no god is watching — when no one is keeping
> score — is the only goodness that was ever truly yours.** Three souls sit at the points of one wound.
- ✦ **Sturm Brightblade, the faithful knight** — who kept a Solamnic oath (*Est Sularus oth Mithas — My Honor Is My
  Life*) through an age when his world's gods had gone silent for lifetimes and the knighthood became a thing children
  mocked in the street. An **Insight** read finds that honor is not the *absence* of the temptation to quit but its
  *presence*, fully felt and freely answered *no*. He died holding a tower-wall in the gap, alone, under an empty sky —
  and found the watching was never the point. A **[RETURNED]** names the empty ledger not the *tragedy* of his virtue
  but the *proof* of it: *a good deed no heaven banked could only have been minted by the man himself.*
- ❖ **Raistlin Majere, the mage who chose the Art** — gold-skinned, hourglass-eyed, brilliant and ruined, who traded
  health, warmth, and every human tie for the one power a cruel world could not take from a frail boy. A gentle
  **Persuasion** can reach the *boy* under the gold — not by flattering the archmage (he'll see it) nor pitying the
  cripple (he'll despise it), but speaking past both — **crit:** the wall cracks and he is, for one moment, *seen*;
  **fumble:** you narrate his pain back at him and he re-armors *thicker than ever.* A **[RETURNED]** names ambition as
  *the scar of being unloved*, and the brother he never quite cut loose as the proof he never stopped wanting love.
- ✶ **Caramon Majere, the brother who stayed** — the giant twin who loved Raistlin with his whole self, was pushed away
  a thousand times, and *never got the message they were through.* An **Insight** read finds him *still keeping the seat
  warm* at the far end of the hall. A **[RETURNED]** names a love that keeps no score as the one kind the Wall *can't
  price* — *not a failed transaction, because it was never a transaction at all.* The twins are written as two halves of
  one wound, unable to cross the hall: the mage whose door *never actually closed*, and the brother who *waited anyway.*
- 👁️ **Three new Returned-senses.** 🧪 **+8 gates** (now **1,301 checks**, all green). **53 zones, 180 souls.** Sigil
  now has **ten doors.**

---

## 👑 v6.30.0 — *"The Menagerie of the Made"* — a fifty-second zone: D&D's monsters, who turn out to be people (content)

> A deliberate change of key after a run of heavy doors — warm, funny, and quietly devastating. The thesis: *a soul is
> not what it was made to be, nor what the tale only ever cast it as.* Three of D&D's most iconic "monsters" — the ones
> the stories put in the first room so the heroes know there's danger — turn out, when you finally stop and *stay*, to
> be the most achingly ordinary people in the City.
- ✦ **Hooting, the owlbear who was made** — *feathered death*, stitched from owl and bear by a bored cruel wizard as a
  *joke*, built to frighten and exactly as terrible as intended. An **Insight** read finds the thing no one ever stayed
  to see: a creature that, alone and unwitnessed and for no reward, *chose gentleness* against every line of its making.
  The question it is starving to be asked is the smallest one — *are you all right in there* — the thing you ask a
  *who*, never a *what*. A **[RETURNED]** names it *the part of the joke that woke up and turned out to be a person*, and
  turns that one question on the Wall.
- ❖ **Coffer, the mimic** — a thing that takes the shape of *treasure* because treasure is the one thing every soul that
  ever found it wanted, so the only company it has ever kept is the *greedy*, reaching, every time, at their very worst.
  It half-decided it must therefore *be* a trap — until it saw the flaw: it was a net that only ever caught the
  grasping, and the kind walked right past the chest. A gentle **Persuasion** can coax it to *drop the chest-shape* and
  show the soft, worthless-to-you, *real* thing underneath — **crit:** it unmasks fully (you wanted the part with no
  gold in it); **fumble:** *"show me the real you"* lands like every clever looter who ever talked the lid open, and it
  *snaps shut.* A **[RETURNED]** names it *a person cast as bait by an audience that only wanted to be bitten.*
- ✶ **Skritch, the kobold** — the *trash mob*, the little one in the doorway, the *tutorial*, the grass the heroes wade
  through to get to the dragon. It had *twelve* clutch-mates with names: Vekk who whistled birds from the sky, Ssira who
  was scared of the dark and slept in the middle so she'd have one of them on every side. An **Insight** read finds its
  last thought, dying in the doorway of its *home* — not for itself, but *counting*, making sure the littlest one got
  away. A **[RETURNED]** *flips the size* — a swarm in a doorway is twelve people defending the only home they've got;
  switch the scale and *the heroes are the monsters* — and stands the whole Wall up next to its smallest soul to ask
  who's really the monster in *that* story.
- 👁️ **Three new Returned-senses.** 🧪 **+7 gates** (now **1,293 checks**, all green). **52 zones, 177 souls.** Sigil
  now has **nine doors.**

---

## 👑 v6.29.0 — *"The Re-Written"* — a fifty-first zone: a love letter to D&D's half-century of upheaval (content)

> The deepest swing at the nostalgia: a quiet, reverent door onto D&D's *own* history of being rewritten — the
> editions, the retcons, the shelved settings — rendered mythically as cosmological upheaval, so it lands both as
> in-world grief and as a love letter to a fifty-year-old game that kept changing the rules and never stopped being
> loved. The thesis: *the structure changes; the love inside it does not.*
- ✦ **Cosmas, the cartographer of a changing heaven** — who has mapped the planes through the **Re-Writings** (the great
  Wheel un-spun and re-strung as a Tree, the Tree felled and raised as an Axis, the Axis dismantled and the old Wheel
  turned again — the cosmology revised from the foundation by powers above the gods), every map made false overnight.
  An **Arcana** read finds the one feature that survived every revision: not the planes, but *the warm roads souls wore
  between each other.* A **[RETURNED]** names the Wall as not eternal law but *a feature of the current draft* — and the
  souls it mortars as the permanent thing that will outlast it, the way they outlasted every heaven before.
- ✶ **Aelith, the un-written goddess** — a small deity of *welcomes* (the lamp left lit, the bread set out) struck from
  canon by a Re-Writing with no place for her: not killed but *un-written*, her temples reattributed, her name sanded
  from every history, the divine exact mirror of every small soul the Wall erases for being un-grand. A **[RETURNED]**
  re-canonizes her the only way a thing like her can be — *by remembering her true*, because **canon and true are not
  the same word**, and a thing that happened cannot be un-happened by any power's correction.
- ❖ **Maru, of the shelved world** — a soul from a beloved prime that was not destroyed but *set aside*, its door sealed
  gently while the dreaming moved to newer worlds. The gentler, stickier grief: *you cannot rage against fondness* —
  fond memory is a coffin with a comfortable lining. A **[RETURNED]** reframes it (*shelved isn't erased; it's a box
  with the lamp still lit, and the dreaming wanders and comes back*), and a **Persuasion** check carries the world's
  name back out — **crit:** you offer the *right-sized* promise, to *mention* it where a dreamer might hear, not to save
  it; **fumble:** you over-promise to reopen the whole world, the exact vow a hundred visitors made, and *seal the door
  tighter.*
- 👁️ **Three new Returned-senses.** 🧪 **+7 gates** (now **1,286 checks**, all green). **51 zones, 174 souls.** Sigil
  now has **eight doors.**

---

## 👑 v6.28.0 — *"The Seven Sisters"* — a fiftieth zone: the legendary women of the Realms (content)

> A deliberate rebalancing of a cast that had leaned male, and a payoff long owed: the legendary **women** of the
> Realms, three facets of one truth — *a soul is not the power, the role, or the lifespan it was handed.* The saga's
> **fiftieth zone.**
- ☽ **Storm Silverhand** — one of the Seven Sisters, the near-deathless Harper who has buried everyone she loves for
  seven centuries and stayed *warm.* A **[RETURNED]** asks the question under the whole saga — what is love worth when
  the beloved always dies and leaves you behind? — and she answers it from ten thousand gravesides: *grief is just love
  continuing, with nowhere to land but you*, and a love is worth what it *was*, not a copper less for ending. The Wall
  weighs the balance on the ledger and never the *spending* — and the godless who poured their lives into loves they
  knew they'd lose are the richest souls it ever held.
- ϟ **the Simbul** — the Witch-Queen of Aglarond, a woman *buried under her own power*, who across centuries poured
  every unbearable feeling into the Art to avoid feeling it, until the storm nearly drowned Alassra entirely. An
  **Arcana** read finds she is not possessed but *hiding* — and a **[RETURNED]** speaks the truth a being of vast power
  could blast you to ash for (*you mistook the cup for the wine*), and something deep under the storm catches the bolt,
  because *a true thing spoken to Alassra reaches Alassra, even seven centuries down.* The way back is the one she
  dreads: to feel one unbearable grief directly — shaped exactly like an old man with a pipe.
- ✺ **Shandril** — the kitchen-girl the world only ever saw as a *weapon.* She wanted a hearth and a quiet year and the
  cosmos dropped the most coveted power in the Realms into her unwilling hands, and from that hour she was never again
  allowed to be a person. A **Persuasion** check to be believed she's more than the fire — **crit:** instead of arguing
  you ask what her favorite thing to bake was (*honey-cakes*), and the test dies because there's no want in it;
  **fumble:** you call her *"more than the spellfire"* and name the fire in the very breath meant to look past it. A
  **[RETURNED]** is the first soul in all the worlds to ask about the girl and not the fire.
- 👁️ **Three new Returned-senses.** 🧪 **+8 gates** (now **1,279 checks**, all green). **50 zones, 171 souls.** Sigil
  now has **seven doors** — half the City of Doors is wired, and the cast is no longer all kings and giants.

---

## 👑 v6.27.0 — *"The Tomb"* — a forty-ninth zone: the saga's thesis put to its hardest test, against true evil (content)

> After all the warmth, the saga turns hard and tests its own thesis against a soul that *is* the Wall. A door that
> should never have opened onto a black tomb where the soul-hoarding archlich builds a god out of the devoured — and
> the question the whole game circles gets its darkest trial: *is even his soul worth something? Does the thesis have
> limits?*
- ☠ **Acererak** — the Wall *made a person.* Everything the Wall does coldly, institutionally, at a remove, he does
  personally, by hand, and calls it ambition: he gathers and devours souls as fuel for his slow ascension toward
  godhood. An **Arcana** read shows the truth his thousand years of cleverness hid — *the throne is empty*; a soul
  cannot be added to another, only destroyed by it, so every self he ate left a god-sized hole, and he is not
  ascending but vanishing, the slowest suicide in the multiverse mistaken for a coronation. A **[RETURNED]** names him
  the Wall's *disciple* (and the first thing he ever sold) — *and he is unmoved.* The saga is honest enough to let one
  soul, offered the truth, spit on it and choose the dark with its eyes open: *"some souls will take the dark, and
  neither of us is going to talk the other out of it."*
- 🗡️ **The win is the victims, not the villain.** An **Intimidation** check defies him (crit: a soul that does not fear
  death breaks his only lever and he yields the whole hoard; fumble: you pour your *horror* at him and he *eats it* —
  he is built of exactly that). The deepest lesson he gives, his only gift: *I am not the thing you must defeat. I am
  the thing you must outlast. The Wall, the same. Do not tire — it is the only defeat we fear.*
- · **Nessa**, a Wayhouse cook spent drop by drop to fuel his godhood and taught the oldest lie in every world — that
  being spent for a great purpose is the most a nobody can amount to. A **[RETURNED]** witnesses her back: *a soul is
  worth the warmth it is*, and a cook who fed cold strangers in the dark for thirty years gave more good than the
  hollow thing ever ate — and her kindness-for-its-own-sake is the **one fuel he could never quite swallow.**
- ✝ **Kael**, a bright hero the Tomb killed in the first hour, to a trap, *for nothing* — who learns from an **Insight**
  read and a **[RETURNED]** that he didn't step on the wrong flagstone: *he stepped first, on purpose, for his friends*,
  and two of them walked out on the warning his death bought. A story isn't measured by its ending but by whether it
  was real while it ran — *it didn't fail; it stopped; and a story that stopped is still a story that was told.* (He
  asks you to tell the Keeper of the Long Table to light a candle for the thousand heroes who died on the wrong
  flagstone.)
- 👁️ **Three new Returned-senses.** 🧪 **+7 gates** (now **1,271 checks**, all green). **49 zones, 168 souls.** Sigil
  now has **six doors.**

---

## 👑 v6.26.0 — *"The Companions' Fire"* — a forty-eighth zone: Drizzt's reborn family, the truest kin the Returned has (content)

> Sigil's warmest door. The Companions of the Hall — Drizzt's family — *all died and were reborn together*, which makes
> them the truest kin the Returned has in any world: a whole circle that crossed back. Four beloved souls round a fire,
> each answering *what is a soul worth that no power claimed?* from a different scar.
- ▲ **Bruenor Battlehammer** — the dwarf-king and foundling who threw out the oldest law his people hold sacred to call
  a drow, a human orphan, a halfling thief and a barbarian boy his *clan*, and weathered his own kind's scorn for it.
  A **[RETURNED]** lands the thesis: *a soul is the family it shows up for, not the blood it was born to* — and the Wall,
  which weighs blood and god, *does not know what a family is.*
- ❋ **Catti-brie** — the truest kin of all: she **died, was unmade, and was born again** — new body, new childhood, new
  age — and is *still herself.* She names the answer to the question under the whole saga: what carries the threshold is
  not the memories, the body, or the skills, but *the way a soul loves* — and the Wall mortars the *cup* (blood, body,
  one life, the god stamped on the side) and never once looks at the *wine.*
- ⟁ **Wulfgar** — dragged living into the Abyss for what his soul felt as decades, who crawled out broken and chose,
  daily and unwitnessed, to be **gentle** anyway. A **Persuasion** check reaches him (crit: honor the *choosing*, not the
  survival, and the gentle giant is seen true for the first time; fumble: ask him *what hell was like* and you walk him
  back into the cell). A **[RETURNED]** lands it: *a soul is not the worst thing done to it; the breaking is only the
  loudest thing that happened.*
- ◆ **Regis** — the small, soft, self-doubting con-man who was *certain* he'd run when it mattered, looked at the open
  door, and stayed. A **[RETURNED]** gives him the measure no one ever gave him: *not how much a soul could lift, but
  how much it gave against how much it had* — and the least who stayed outweighs the greatest who strolled. (He keeps a
  charm-gem that fakes affection, and never once uses it on the ones whose love he actually wants.)
- 👁️ **Four new Returned-senses.** 🧪 **+7 gates** (now **1,264 checks**, all green). **48 zones, 165 souls.** Sigil now
  has **five doors.**

---

## 👑 v6.25.0 — *"The Far Doors"* — a forty-seventh zone: Sigil opens onto D&D's wilder corners (Eberron / Spelljammer / Ravenloft) (content)

> After the Long Table's gut-punch, a turn of the kaleidoscope: a junction in Sigil where three *far* doors open onto
> the strangest, most beloved corners of the multiverse — a world of made souls, the star-dark between the spheres, and
> a domain of guilt-mist — each a love letter to a setting, each answering *what is a soul worth that no power claimed?*
- ⊞ **Cipher, a warforged** (Eberron) — built on a forge-table to be a soldier, who didn't power down when the war
  ended but kept walking, reading, *caring* about the breakable people it was made to kill, and asks the one question
  its kind was never built to survive: *am I a soul, or a very good imitation — and is there any way, from in here, to
  tell?* An **Investigation** read finds the soul behind the blue light; a **[RETURNED]** lands it: *a soul is grown,
  not installed* — and the very doubt is the soul examining itself, the one act no tool ever performed. (It indicts the
  Wall from a new angle: a made thing isn't even *rated* the stone — discarded *beneath* damnation.)
- ✹ **Sergeant Bombast, a giff** (Spelljammer) — gloriously martial, tusked, gunpowder-proud, who sailed wildspace
  forty years sphere to sphere and *belongs to no world.* A tonal palette-cleanser of pure gonzo wonder — and then the
  grief under it (a shroud for every shipmate the dark took). A **[RETURNED]** frees him: *belonging is who you threw
  in with, not the dirt you were born on* — and a soul that threw in its lot with the whole Great Dark belongs more
  than any wall could mortar.
- ◌ **Voss** (Ravenloft) — a lord who coldly betrayed the one soul who trusted him, and was *kept* by the Dark Powers
  in a domain of mist shaped from his own remorse: *a prison unbreakable from without, opened only from within* — the
  Wall's exact sister. The lock isn't that he *can't* forgive himself but that he believes he *must not.* A **Persuasion**
  check reaches him (crit: he walks out, for the kindness of the man he wronged; fumble: cheap comfort, and he ices
  over — *you cannot forgive me for him*); and the deepest **[RETURNED]** turns him on his jailers: *a power that feeds
  on remorse never wanted the debt paid — your fog honours nothing; it feeds the enemy he died fighting.*
- 👁️ **Three new Returned-senses.** 🧪 **+7 gates** (now **1,257 checks**, all green). **47 zones, 161 souls.** Sigil
  now has **four doors** — the cross-world cameos, the Realms' legends, the room behind the rooms, and the far doors —
  and keeps growing.

---

## 👑 v6.24.0 — *"The Long Table"* — a forty-sixth zone: the room behind all the doors — the saga's beating heart (content)

> The most personal thing in the whole project, and the moment the entire saga was secretly building toward. A door in
> Sigil that almost no one finds leads not to another world but *behind* them all — to a long table scarred with the
> rings of a thousand cups, the small burns of a thousand guttered candles, and everywhere the bitten edges where small
> carved things were rolled and rolled again. This is where the worlds come from. It is a love letter not to D&D the
> rules, but to **the table, the circle, the friends, and the decades.**
- 🕯️ **A forty-sixth zone — the Long Table.** The **Keeper of the Table** — *the one who has been telling* — reveals
  the truth under the whole saga: every world's Wall is a parish translation of one wall behind them all, and it is
  **not where the unloved go. It is where the *unfinished* go** — the hero whose story stopped mid-quest, the companion
  left forever at a cold campfire, the whole worlds dreamed with passion at this table and abandoned when the circle
  broke, when the tellers grew up and grew apart and grew old and the candles burned down and *no one came back to
  light them.* The loved, and the left.
- ✨ **The cure is the thing this entire game has been doing without its true name.** A **[RETURNED]** lands it: witnessing
  *is* the only resurrection. Someone, years on, in the middle of an ordinary life, stops and says *do you remember?* —
  and the remembering reaches down into the stone and pulls the soul home. The Returned didn't cheat death; *it was
  remembered hard enough to come back.* The choice triggers **the Naming** — a litany that speaks the forgotten of
  every world (the heroes set down three steps from triumph, the campaigns that ended on an ordinary Tuesday, the
  friends who held the chairs and are gone now) — and every Wall in every world, for the first time since the first
  story stopped, begins to let go.
- 🪑 **Two souls, two halves of the same grief.** The **Unfinished One** — a half-drawn hero frozen with its hand on the
  last door, *abandoned not in anger but by love that ran out of evenings* — freed by a [RETURNED] to understand it is
  not abandoned but *saved for later*, a bookmark set down with love at the exact page the evening ended. And the **One
  Who Left** — a soul that sat at the table a long golden while and rose one ordinary evening certain it would be back
  *next time*, and never was — who cannot make itself sit in its own chair because it half-believes it broke something
  sacred by growing up, and is freed to learn that *it did not fail the table by leaving; it kept it* — the circle isn't
  gone, it's *carried*, by everyone who ever sat in it and remembers.
- 👁️ **Three new Returned-senses.** 🧪 **+6 gates** (now **1,249 checks**, all green). **46 zones, 158 souls.** *Nothing
  imagined with love is ever truly gone. It only waits, in the warm room, for someone to come back and say: now, where
  were we?*

---

## 👑 v6.23.1 — *"The Doubter Who Ascended"* — Cadderly Bonaduce joins the Hearth of Faerûn (content)

> A sixth legend takes a table at the Hearth — and arguably the saga's most *useful* witness against the Wall, because
> he walked the whole road the Wall pretends to measure, and can testify it measures backward.
- ✶ **Cadderly Bonaduce** — the young skeptic of Deneir's library who served the god for years *without believing*,
  out of plain decency, and discovered that *the doing of good IS the prayer*; who raised the Spirit Soaring cathedral
  stone by stone out of his own life-force, aging decades in months, and **ascended toward godhood** at the last. He is
  the perfect counter to the Wall: a [RETURNED] lands the thesis through him — *a god's claim does not make a soul
  worthy; a worthy soul earns the claim, and the claim comes last, always.* The Wall inverts the most important truth
  in any cosmology, mortaring the godless for lacking a receipt that was never what mattered. (A **Religion** read maps
  his ascension; he names the Wall not unjust but *incoherent* — a committee's cowardice dressed as a commandment, a
  lie told in Heaven's name — and points you, priest to priest, at the cold threshold: *a soul cannot be taken, only
  abandoned; Irenicus has his still.*) **Faith is a practice, not a possession — and the godless faithful were passing
  the test without being told they were graded.**
- 🧪 **+1 gate** (now **1,243 checks**, all green). The Hearth of Faerûn now seats **six** Realms legends; **45 zones,
  155 souls.**

---

## 👑 v6.23.0 — *"The Hearth of Faerûn"* — a forty-fifth zone: Sigil's first door — the Realms' own legends, as cameo souls (content)

> Sigil grows. Off the City of Doors opens a homesick **Faerûnian taphouse** — because even the greatest heroes of one
> prime are just expats in the multiverse, and wherever souls wash up far from home, they build a taproom that smells
> of it. Here the Realms' own legends gather, and the saga's question lands on the icons who defined it. A reverent fan
> tribute, each one woven into the thesis, not a name-drop. *(On the IP: D&D's setting lore is named throughout this
> project already; these specific authored characters are written as loving homage — for a commercial build they'd
> need filing-off or licensing, but as the love letter, they're the heart.)*
- 🍺 **A forty-fifth zone — the Hearth of Faerûn** (through a door in Sigil). Five Realms legends, five answers to *what
  is a soul worth that no power claimed?*:
  - 🎶 **Volothamp "Volo" Geddarm** — the chaotic-neutral loremaster, raconteur, and tease who's been to every realm and
    era and survives on pure dumb luck (which a [RETURNED] reveals isn't luck — *a cosmos that runs on witnessing won't
    let its great witness die*). The comic mirror of Sigil's fading wayfarer. **Recruitable as your guide across the
    doors** — a chaotic loremaster to gossip and tease his way through the realms at your side.
  - ✶ **Elminster** — the thousand-year Sage of Shadowdale, who admits he does *not* know what a soul is worth, and is
    shown by a [RETURNED] that *understanding was his own coward's flinch* — knowing a cruelty is not the same as ending
    it, and a Chosen with power to unmake mountains never touched the Wall.
  - ❂ **Gorion** — the Harper who died a *father, not a hero*, protecting a god-marked foster-child, and needs to know
    the love he poured in *held.* A [RETURNED] tells him it always holds — and a father who died too soon can finally
    rest. (You can carry his words to his child.)
  - ❈ **Drizzt Do'Urden** — the drow who refused his birth and, with the Companions of the Hall, *died and came back* —
    kin to the Returned. He affirms the thesis with his whole life: *a soul is the sum of its choices against the
    current it was born into, and the stronger the current, the more it's worth for swimming it* — so the Wall has it
    exactly backward.
  - ⊗ **Jon Irenicus** — the **Shattered One** at the cold threshold, the saga's hardest case: an elf severed from his
    gods who *chose* the monster (Drizzt's exact mirror — same wound, opposite answer). A **Persuasion** check reaches
    the *Joneleth* under the Irenicus (crit surfaces the buried elf; fumble — a pity or a redeemer's hook — makes him
    recoil); a [RETURNED] lands the unbearable truth (*they never took your soul; you abandoned it*), and the door back
    opens on the one thing he refused to feel — guilt. Or you can **judge him and turn away** (a real fork) — and he
    names what you've done: *the exact thing the Wall does to the Faithless.*
- 🗺️ **Sigil is now a hub with two doors and counting** (the cross-world cameos of v6.22, and now the Realms' own
  legends), built to keep growing.
- 👁️ **Five new Returned-senses.** 🧪 **+7 gates** (now **1,242 checks**, all green). **45 zones, 154 souls.**

---

## 👑 v6.22.0 — *"The City of Doors"* — a forty-fourth zone: SIGIL, the omniversal crossroads — a new axis for the whole saga (content)

> The biggest swing in the project's history, and a new *spine.* The crack in the Returned's soul was always a crack in
> **time** — Netheril, the Crown Wars, the Spellplague. This reveals it was always also a crack in the **Wheel.** The
> same wound that let you fall through your own world's history lets you fall *sideways*, out of your Realm entirely,
> into **Sigil, the City of Doors** — the crossroads at the hub of the Great Wheel where every plane and every prime
> world has a portal — and the saga's local question becomes an *omniversal* one. A love letter to D&D as the thing
> that connects every world it ever dreamed.
- 🚪 **A forty-fourth zone — Sigil, the City of Doors** (through a door that shouldn't exist, at the Threshold). Five
  cameo souls converge from across the multiverse, each facing the saga's own question — *what is a soul worth that no
  power claimed?* — in their own cosmology's clothes:
  - 💀 **Grist, a Dustman of the Mortuary** (Planescape) — the framing soul, in full cant (berk, cutter, the dead-book,
    the Cage, the Lady unspoken). His faction holds all are already dead; *you* are the disproof he's waited forty
    years to meet, a soul that went into the True Death and came back. He frames the convergence: **the Wall of the
    Faithless is one prime's parish law, not the order of the cosmos** — and the catalyst that connects all these
    worlds isn't the Spire but the **shared dreaming**: they were all told round the same fire, by the same hands. *The
    room where the house finds out it's a house.* (The meta-thesis, made diegetic — D&D itself is the catalyst.)
  - 🌹 **Sir Garon of Solamnia** (Dragonlance / Krynn) — kept the Oath and the Measure through the gods' three-hundred-
    year silence after the Cataclysm. *Est Sularus oth Mithas.* By the Wall's arithmetic he's Faithless; a **[RETURNED]**
    shows him faith was never whether the sky answers — it's whether *you* do — and a Knight of Solamnia pledges to your
    cause.
  - ☼ **Kesh of Athas** (Dark Sun) — from a world with **no gods at all, ever**, so by the Wall's logic every Athasian
    soul is mortar. The killer thematic bridge: the Wall damns for a bargain whole worlds were never offered. A
    **[RETURNED]** tells the raging soul its fury is *correct*, and aims it — and a soul forged on a dying godless world
    pledges to help tear the Wall down.
  - ☾ **Lucan, a soul that would not die** (a World-of-Darkness homage, filed off — no trademarked names) — refused
    death, holds a Beast behind a private code, the mirror of the Returned. A **Persuasion** check reaches the man under
    the hunger (crit and fumble: a kindness aimed *wrong* feeds the Beast faster than malice); a **[RETURNED]** frees
    him — *a monster doesn't hold the door; you held it.*
  - ✒ **Ferrin, a fading wayfarer-chronicler** (a Pathfinder homage, filed off) — sworn to *go everywhere, record
    everything, send it home*, dying with the last chronicle undelivered, terrified of going unread. Ties straight to
    the saga's witnessing thesis: a **[RETURNED]** shows that it was never the paper — being *known* by one true soul,
    now, is the immortality, and *the loving already happened, a thousand awestruck pages over.*
- 🗺️ **A note on the crossover.** D&D's own multiverse is named outright (Sigil/Planescape, Krynn/Dragonlance,
  Athas/Dark Sun — same lineage as the Realms lore already throughout the saga). Non-D&D worlds (the night-creature, the
  wayfarer-Society) are written as **loving homages without trademarked proper nouns** — *primes from beyond the Wheel*,
  in the truest Planescape spirit.
- 👁️ **Five new Returned-senses.** 🧪 **+7 gates** (now **1,235 checks**, all green). **44 zones, 149 souls** — and
  Sigil is built to *grow*: a recurring hub for as many doors and cameo souls as the omniverse can hold.

---

## 👑 v6.21.0 — *"The Last Watch"* — a forty-third zone: a vigil (a watch kept four hundred years past all need) (content)

> A twenty-sixth structure, and a player-role no zone has held: a **vigil** — a watch that cannot end, and the question
> of whether to keep it, release it, or pass it on. Three souls frozen round one gate, each holding for the others,
> none willing to be the first to go.
- 🛡️ **A forty-third zone — the Last Watch** (up to a gate that never fell, off the almshouse line). **Bregan** was set
  to hold the inner gate of a keep in a forgotten siege and given the oldest order a soldier knows — *hold until
  relieved.* He held it the whole long day, died at the post with the order still binding, and no relief ever came —
  and four hundred years on he holds it yet, because a soldier doesn't abandon a post, and because if the war is over
  and the gate guards nothing, then his death was for nothing, and *that* is the one assault he cannot survive.
- ⚔️ **The thesis as duty.** A **[RETURNED]** names what stands a soldier down at last: the watch was never pointless.
  For four hundred years one soul sheltered behind that gate and was *not alone, because he stood faithful* — the
  realest duty there is, that *someone was kept.* The watch isn't being abandoned; it's **complete.** He didn't fail to
  be relieved — he outlasted the need for it. A **Persuasion** check can relieve him properly, by the soldier's form
  (Charisma, full **crit and fumble**): **crit**, he snaps a salute four centuries overdue and steps off the wall;
  **fumble**, you order him down *because it was all for nothing* — and you have just named his deepest terror as fact,
  and a man does not abandon a post to the soul that tells him it was waste.
- 🐝 **The cage of two keepers.** **Cael**, a beekeeper who sheltered behind the gate and is the last of the sheltering
  souls left, stays only so Bregan won't keep his lonely watch with no one to keep it for — while Bregan holds only so
  Cael stays guarded. Each other's cage, each calling it love and duty (it is both) and each frightened, too, of
  walking out of the dark alone. A **[RETURNED]** frees them to leave **together**, at the same breath, so neither feels
  they broke the watch.
- 🎖️ **The order he can't bear to give.** **Captain Wystan** gave Bregan the order, meant to send relief within the
  hour, and was swept off in the rout and killed with it un-sent — and has stood apart from the gate the whole of his
  death, unable to cross the hundred feet, because giving the stand-down means the faithful man will do the arithmetic
  and *know no one forgot him by accident.* A **[RETURNED]** frees the captain to cross (the order frees the giver too),
  and — gated back from the gate — carries Bregan's forgiveness: the sentinel feared his *captain fell on the road*,
  and is glad he didn't, and waits to see him before standing down.
- 👁️ **Three new Returned-senses.** 🧪 **+9 gates** (now **1,228 checks**, all green). **43 zones, 144 souls** —
  twenty-six structures, and the player has now been advocate, accused, thief, broker, mediator, judge, builder,
  house-comforter, stage-finisher, soul-physician, soul-stiller, blind guide's eyes, wordless mouth, and the relief
  that finally came for a soldier four hundred years late.

---

## 👑 v6.20.0 — *"The Untranslated"* — a forty-second zone: a relay (carry meaning across a gulf of language) (content)

> A twenty-fifth structure, and a *mechanic* no zone has had: a **relay.** Not bridging two lonely souls into the same
> conversation — *carrying specific meaning back and forth* across a gulf neither can cross, in a true two-directional
> shuttle where what you learn from one soul unlocks what you can deliver to the other.
- 🗣️ **A forty-second zone — the Untranslated** (where the thin place gathers the souls divided by tongue and time).
  **Baba Yelena** died in the old country with the old tongue in her mouth; her grandchild **Lio** grew up in the new
  land, never learned a word of it, was ashamed of it as a child, and pulled away before the grandmother died. They
  have found each other here — the love instant, total, obvious — and cannot exchange a single syllable.
- 🔁 **A true two-way relay.** A **[RETURNED]** reads Yelena's untranslatable meaning straight from the cold (not the
  words — the certainty beneath them: *she knew the child was ashamed, forgave it before it happened, and grieves only
  that the child carried the shame as a wound alone*), and you carry it across. **Delivering it is gated** on holding
  it, and is a **Persuasion** check — *say it the way she would, slow, with the whole of the forgiving in it, so a
  soul that's waited years will believe it*: **crit** lands it whole (the old endearment, *my small heart*, finally
  understood) and the child breaks free; **fumble** leads with the tender word before the forgiveness and the
  braced-for-blows child flinches from the kindness. And the relay runs **both ways** — the child's sorrow can be
  carried *back* to the grandmother (also gated), who absolves it utterly.
- 📜 **The thesis, in the wordless dragoman.** **Odo** carried meaning across twelve tongues for sultans and merchant-
  princes — and died mid-sentence delivering a dying man's last blessing, and woke with every word drained out, even
  his mother's first. He believes he is a ruin, the exact tool these two need, broken. A **[RETURNED]** names what an
  **Insight** read confirms: *a soul is not the thing it could do.* The vocabularies were the brush; the **seeing** —
  the bone-deep attention to what a soul truly means under its words — is the painter, and it is intact. He mistook the
  loss of his brushes for the loss of his eyes. He becomes a dragoman of the *wordless* dead, bridging the gulfs the
  Returned can't always tend — *the one tongue under all the others, that carries across every gulf there is.*
- 👁️ **Three new Returned-senses.** 🧪 **+8 gates** (now **1,219 checks**, all green). **42 zones, 141 souls** —
  twenty-five structures, and the player has now been advocate, accused, thief, broker, mediator, judge, builder,
  house-comforter, stage-finisher, soul-physician, soul-stiller, blind guide's eyes, and the mouth between two souls
  who share blood and no words.

---

## 👑 v6.19.0 — *"The Wayfinder Who Lost the Way"* — a forty-first zone: an escort (lead a soul that can't see the road) (content)

> A twenty-fourth structure, and a *player-role* no zone has held: an **escort.** Not walking *with* souls toward rest
> (that was the Long Walk) — *leading* one that physically cannot perceive the road, by transferring your sight to her
> trust, step by blind step, past a thing in the dark that wants her to stop walking forever.
- 🧭 **A forty-first zone — the Wayfinder Who Lost the Way** (off the marked pilgrim road, into the trackless dark).
  **Yorrel** guided caravans across the Anauroch and the high passes forty years and never lost a soul — until pride
  and a winter shortcut walked her whole train of twelve off a snow-cornice into a ravine, herself last on the rope.
  Now she is **blind to the road** — safe ground and the Hunger's edge all one featureless black — and she will not
  take a single step, because the last time she trusted her own bones, twelve people died trusting them with her.
- 🤝 **The thesis as guiding.** A **[RETURNED]** names what frees a guide who's lost the way: *you are not your one
  wrong turn* (forty years of safe lines against one cornice), and *a guide can be guided* the one time the dark takes
  her eyes — there's no shame in being the frightened soul roped behind, trusting eyes not her own. You become her
  eyes. A **Persuasion** check earns her first blind step on your word alone (Charisma, full **crit and fumble**):
  **crit**, she recognizes the steady tone she used on her own charges for forty years and *walks*; **fumble**, your
  read is half a turn off and you aim her blind foot toward the downhill — and every instinct she has left screams,
  and she recoils into worse terror.
- 🌸 **The child is the rope, not the trap.** **Talla**, drowned at seven, holds the wayfinder's hand with a faith so
  total it is the very knife Yorrel can't bear — *they trusted me too.* A **[RETURNED]** reframes it: trust freely
  given, said aloud, is not a trap that proves a guide will fail; it is the rope that pulls a frozen guide forward when
  nothing else will. (The guide's child-line is **knowledge-gated** — you must first learn from Talla that her trust
  is the key.)
- 🕳️ **The Kindly Dark.** A fingerling of the **Hunger**, wearing kindness like a coat, murmuring from the low ground
  the one thing a lost guide most wants — *it's easy this way, you can rest, just follow.* Its "rest" is dissolving;
  its gentle downhill is the one road that doesn't lead home. A **[RETURNED]** names it — and reads the single thing
  that defeats it: **it can only lure, never drag.** Every soul it ever took walked in on their own tired feet; it does
  not need to beat you, only to outlast you. The counter is to get the guide and child home before their strength runs
  out, and make the true road louder than the kind voice.
- 👁️ **Three new Returned-senses.** 🧪 **+9 gates** (now **1,211 checks**, all green). **41 zones, 138 souls** —
  twenty-four structures, and the player has now been advocate, accused, thief, broker, mediator, judge, builder,
  house-comforter, stage-finisher, soul-physician, soul-stiller, and the eyes for a guide who lost her own.

---

## 👑 v6.18.0 — *"The Witnessed"* — a payoff pass: the epilogue chronicler now reads the five newest structures (content)

> A cohesion pass. The chronicler's "side-roads" payoff (v6.3, completed v6.9) read every structure up through the
> Moot — but five have shipped since: the Cairn, the House, the Empty House, the Lazaret, and the One Who Runs. They
> were tested and reactive *in* their zones, but the years-after didn't yet know them. Now it does.
- 📜 **A fifth side-roads cluster — "the witnessed."** Ask the epilogue Chronicler about *the souls you witnessed free*
  — the ones that mistook themselves for the wrong thing (their walls, their applause, their death, their glory, their
  worst minute) and waited in the dark for someone with the cold to read them true — and he reads the years-after of
  each, **reactively, off your real outcomes:**
  - **The house** (`ho.house_understands`): it stopped fighting the new family and let them *deepen* it; little Posy
    grew up on its doorframe; an inherited house never dies.
  - **The lazaret** (`lz.physician_freed`): Sister Maud stopped nursing the incurable and started *witnessing* them out
    — the pallets emptied one by one, and the last to rest was herself.
  - **The Empty House** (`pf.tragedian_freed`): Orvel Vane finally looked down, finished his eighty-year play to the one
    true witness who'd never left, and the understudy went on at last.
  - **The cairn** (`bc.eddin_keystone`): it stands yet, on a forgotten fisher's true name — *a fisher named Eddin is
    holding them up.*
  - **The one who ran** (`fl.runner_stilled`): Symon stilled, turned, took the six words; he, his brother, and the hound
    who remembered he was a man went on together, and the dark is quieter where a man used to run forever.
- 🪢 **Closing the loop.** Every cluster keeps its **default** (a soul who walked other roads still gets an honest
  line: *I'll not invent endings for souls you never met — but the dark is patient, and so are the souls who wait in it
  to be seen*). The chronicler now reads **all twenty-three** structures across five clusters.
- 🧪 **+1 gate** (now **1,202 checks**, all green), proving the new cluster fires on all five outcomes and keeps a
  default. No new zones — **40 zones, 135 souls** — but the saga's ending now remembers its newest souls.

---

## 👑 v6.17.0 — *"The Thaw"* — the cold runs two ways: a deeply-haunted Returned can come back to the warm (mechanic)

> The Haunted system (v6.14–6.15) made leaning on the cold *cost* you — warned, weighed, and worried over. But a cost
> with no way back is just a doom, and the saga's whole thesis is that a soul is never only its worst tendency. This
> closes the loop: the cold is now a *two-way* street. You can drift in; you can also choose, by the fire, to come
> back.
- 🔥 **A thaw at the Hearth fire.** When you've drifted deep into the cold (Haunted ≥ 6), the fire offers a new choice
  the warm never needed: *no reckoning tonight, no reading the dead, no reaching — just sit in the warmth and remember
  I was alive once.* It is the deliberate inverse of "sit with what you are" (which lets the warmth stop at your
  wrists and *adds* haunt): here you make the warmth get *past* the wrists for the first time in weeks.
- ⬇️ **It actually decrements the disposition.** On the engine's real `AddInt`, the thaw applies **Haunted −3** and
  **Mercy +1** — pulling you back *below* the `HAUNT_WARN = 6` band, turning off the reckoning-panel warning, easing
  the living and the dead. The remembering names the people who warned you — *Dace was right by this same fire, Mab
  felt it across her bar, the dead at the assize were afraid for you* — so the thaw pays off the whole consequence arc.
- ♻️ **Self-regulating, not a free button.** The choice is gated to appear *only* at Haunted ≥ 6, and taking it drops
  you under that line — so it removes itself once you're warm again, and a very deeply frozen Returned (8, 10+) has to
  come back to the fire more than once. You stay the Returned; you'll go back into the cold tomorrow, because that is
  the work — but the cold is no longer a sentence, only a tide.
- 🧪 **+1 gate** (now **1,201 checks**, all green), proving the thaw is gated at deep haunt, decrements past the
  warning band, and returns warmth. No new zones — **40 zones, 135 souls** — the haunt arc is now complete: drift in,
  be warned, be worried over, and find your own way back to the fire.

---

## 👑 v6.16.0 — *"The One Who Runs"* — a fortieth zone: a chase (a soul you win by stilling, not catching) (content)

> A twenty-third structure, and a *shape* no zone has had: a **chase.** But the genius of the afterlife is that you
> cannot catch a soul that runs from itself — so this is a chase you win by *stilling.* **Symon** has fled across the
> whole dark country of the dead for years, and the faster he runs, the more exactly his grief keeps his pace.
- 🏃 **A fortieth zone — the One Who Runs** (out past the Last Torch, where the causeway's light gives way to the long
  dark). Three souls locked in one flight: **Symon**, who froze in a burning yard at nineteen while his twelve-year-old
  brother called his name, and has run from that minute ever since; **Edrid**, the brother who burned, chasing not to
  punish but to *forgive*; and **Gethin**, a thief-taker worn down past his own name into pure pursuit.
- 🤚 **The thesis as flight.** Symon doesn't flee the hound at his heels — he flees a *name* he pinned on himself for
  the worst minute of his life. A **[RETURNED]** names what stops a running soul: *a soul is not its worst minute; the
  thing you flee fits in a hand and can be set down.* You don't catch him — he's faster than any soul — you **still**
  him, with a voice that wants nothing from his stopping. A **Persuasion** check to root him (Charisma, full **crit
  and fumble**): **crit**, he asks himself for the first time whether the running ever once got him *away* from it,
  and *sits*; **fumble**, you reach — an instinctive helping hand — and a reaching hand is the one thing in any world
  Symon cannot bear, because *a hand into the fire is the last thing he didn't do*, and he's gone.
- 💛 **The brother chases to forgive — and that's why Symon runs hardest.** Edrid forgave him *while it was happening*,
  with his last breath, understanding the fire was between his brother and the stair. A **[RETURNED]** carries the six
  words — *it wasn't your fault, I forgive you* — into the cold the runner can't outrun, to wait until he stills, so
  he feels he was forgiven the whole time he ran. (Symon flees mercy harder than vengeance: being hunted *confirms*
  his verdict; being forgiven *unmakes* it — so the runner's forgiveness-line is **knowledge-gated**, earned only
  after you learn what Edrid carries.)
- 🐕 **The hound is a soul frozen into its function** — the same tragedy the Wall is made of, smaller: forty years a
  thief-taker, worn away to pure pursuit, hunting a stranger because the hunt is the last word of himself he can read.
  A **[RETURNED]** names the man under the verb (a name, a wife, a small house, a dog that was only a dog), and he
  opens his hand. **Freeing the hunter frees the quarry both** — the end-the-hunt line is gated on having stilled the
  runner first.
- 👁️ **Three new Returned-senses.** 🧪 **+9 gates** (now **1,200 checks**, all green). **40 zones, 135 souls** —
  twenty-three structures, and the player has now been advocate, accused, thief, broker, mediator, judge, builder,
  house-comforter, stage-finisher, soul-physician, and the pursuer who catches a soul by giving it a reason to stop.

---

## 👑 v6.15.0 — *"The Cold You Carry, part 2"* — a companion worries: the haunt reaches the people closest to you (content)

> The consequence system (v6.14.0) made the world and the dead notice you freezing. This brings it home — to the
> one person at your own fire. The warning lands hardest not from a stranger or a tribunal, but from a friend who
> threw in with you and is watching you slip away one cold mile at a time.
- 🗡️ **Dace Iron worries.** The Faithless blade-for-hire — your first companion, who let her professional guard down
  off-watch — gains a deep-Haunted (≥6) campfire greeting where she sets the whetstone *down*: she's soldiered beside
  cold ones, *been* the cold one, and she knows the look. When you came warm to the Quarter you stopped to help her
  marked girl at the river for nothing; now the fire *gutters* when you sit, the dead lean *toward* you and the living
  lean *back*, and you talk to ghosts easier than you talk to her.
- 🔀 **A worry exchange with real agency** (gated at Haunted ≥ 6, on the engine's `when.int`): she names the *pull* of
  the grave plainly — *I didn't sign on to follow you into the Wall one polite, merciful, frozen choice at a time* —
  and asks you straight. Two answers, two characters:
  - **Let her anchor you** (+merciful): she takes it as a job on top of the sword — *first night the fire guts too low
    when you sit, first time you talk to a ghost like it's easier than talking to me, I'll say so, loud* — and vows to
    read you for the cold drift the way she reads a room for a knife.
  - **Double down on the cold as 'the job'** (+haunted): she keeps her word and watches your back *into* the dark —
    but tells you about the comrade who let *her* freeze and never said stop, whom she's hated quietly for ten years,
    and refuses to be that for you: she'll say it every cold mile, that there's a warm seat by the fire with your name.
- 🧪 **+1 gate** (now **1,191 checks**, all green), proving the greeting fires at deep haunt and the worry fork is a
  real merciful-vs-haunted choice. No new zones — **39 zones, 132 souls** — the haunt now costs you the warm world,
  the dead's verdict, *and* the worry of the people who love you.

---

## 👑 v6.14.0 — *"The Cold You Carry"* — the Haunted disposition gets teeth: leaning on the [RETURNED] voice now has a warned-about cost (mechanic)

> A systems pass, not a zone. The dead-touched **[RETURNED]** dialogue choices are some of the best writing in the
> game — and until now there was nothing to stop a player picking them every time. They were always available and the
> `disp.haunted` they accrue was tracked but *never read*: no warning, no consequence. This release closes that gap.
> The cold you reach for to read a soul's secret is the same cold that is slowly turning you into the thing on the
> throne — and now the game **says so, to your face,** before it's too late to choose otherwise.
- ⚠️ **The reckoning panel warns you.** `disp.haunted` now has a threshold (`HAUNT_WARN = 6`). Cross it and the
  Haunted row in the live "who you're becoming" reckoning turns ominous (red, ⚠-marked) and a banner appears:
  *"The cold is winning. You have leaned on the Returned's voice so often that the living flinch from you now and the
  dead crowd close. Reach past the warm world much further and you risk freezing into the very thing that waits on the
  throne."* The player can **see** the cost accruing, in real time, and course-correct.
- 🍺 **The living flinch.** **Mab** the tavern-keep — who normally greets you with six bits of warm gossip — gains a
  deep-haunt (≥6) greeting where the warmth fails her: the room goes cold when you enter, the regulars find somewhere
  else to look, and she asks, kindly and frightened, whether there's *still a bit of warm left in you.* The first
  living person to feel you leaving before you've gone.
- ⚖️ **The self-trial weighs it.** The **Assize** (where the dead put your existence on the scale) already read
  `disp.merciful` and `disp.ruthless`; now it reads **Haunted** too, on the engine's real `when.int` gating:
  - At **haunted ≥ 6**, the **Arbiter** brings a charge no other path does — *you were not cruel; you spent your
    causeway in mercy. But you did all of it with the grave's voice* — naming the **second road to the Crown**, the
    one the Last Returned walked not through cruelty but through **attrition**, freezing one cold-but-kindly choice at
    a time. (Ruthlessness still outranks it: a cruel-and-haunted soul is charged as cruel.)
  - At **haunted ≥ 8**, the **Witness** — the warmth you left behind — steps up *grateful but frightened*, and begs
    the scale to weigh the thing no one else will say: not whether your mercy was real, but **what it cost the soul
    who carried it.** This testimony **overrides even the bright merciful witness**, so a player who saved everyone
    *with a hand that stopped being able to feel them* finally hears it.
- 🧪 **+5 gates** (now **1,190 checks**, all green), proving the warning machinery ships, the three new readings fire
  on the real engine, and the **precedence** is correct (ruthless > haunt charge; deep-haunt witness > merciful
  witness). No new zones — **39 zones, 132 souls** — but the most-written disposition in the game finally *matters.*

---

## 👑 v6.13.0 — *"The Lazaret of the Last Breath"* — a thirty-ninth zone: a healing-triage (souls death-locked in their final moment) (content)

> A twenty-second structure, and a new player-role: a **consulting physician** in a house of healing where no one can
> be healed. The **hard-dead** — souls who died too sudden, too violent, too frightened to understand it happened —
> are each locked reliving the instant of their death, certain the dying is still happening, *now*, forever. **Sister
> Maud**, an infirmarian who tended the plague-sick thirty years, runs the lazaret and has not cured one soul in eighty
> years, because she is treating the wrong ailment entirely.
- 🩺 **A thirty-ninth zone — the Lazaret of the Last Breath** (past the herb-garden gate off the Reed-Walk). Three
  souls, three deaths, three different relationships to the loop: **Sister Maud**, who relives her own *failing*
  (a dying orphan she couldn't save) by compulsively nursing the incurable; **Old Tam**, a ferryman who *thrashes to
  escape* his drowning; and **Wren**, a mother who *holds her death close* and will not be coaxed out of the fire.
- ⚕️ **The thesis as medicine.** A **[RETURNED]** — the one soul who crossed the threshold and came *back* — names what
  the infirmarian cannot: there is no wound to cure, because the death already happened and is *over*; what locks these
  souls is a moment they mistake for *now*. You don't nurse them out. You **witness** them out — stand in the looping
  instant with the authority of the truth and say *it ended.* And the cure for the patients is the cure for Maud: the
  orphan she "failed" died a century ago, and sitting three nights with a child who had no one was never a failure but
  the last kindness of his life.
- 🌊 **The drowning man is locked by a word, not the water.** Reading Tam's loop (an **Insight** passive) reveals he
  made his peace with dying — what snagged his soul is *alone*, going under with no hand to hold. A **Persuasion** check
  (Charisma, full **crit and fumble**) lets you breathe him up one slow surfacing breath: **crit**, he broaches into
  air in six words and calls you a born ferryman; **fumble**, you chirp *"up you pop!"* at a drowning man and he'd have
  put you overboard for it. The **[RETURNED]** goes *down* to where he is and tells him from the bottom that no one —
  not the dog he died fetching, not the ferryman — ever goes into the cold water alone.
- 🔥 **The mother is trapped in a victory, not a grief.** Reading Wren's loop (an **Insight** passive) finds she is not
  reliving loss — she is reliving the four seconds she got her daughter out the high window, the one place left where
  she still gets to save her child. The **[RETURNED]** shows her the looped moment is a **rescue, not a goodbye**: Bell
  *lived* — grew old, died warm with grandchildren in the room, sixty years bought by *"one, two, out you go."* She
  doesn't need the fire to keep the child; she can carry the saving instead of the burning.
- 👁️ **Three new Returned-senses.** 🧪 **+8 gates** (now **1,185 checks**, all green). **39 zones, 132 souls** —
  twenty-two structures, and the player has now been advocate, accused, thief, broker, mediator, judge, builder,
  house-comforter, stage-finisher, and the physician who heals by witnessing rather than curing.

---

## 👑 v6.12.0 — *"The Empty House"* — a thirty-eighth zone: a performance (a tragedian playing to no one) (content)

> A twenty-first structure, and a player-role no zone has held: you don't free a soul, judge a dispute, build a thing,
> or comfort a house — you **finish a play.** **Master Orvel Vane**, the finest tragedian on the Sword Coast, dropped
> dead of a stopped heart three lines from the end of his greatest role, on opening night, mid-soliloquy — and could
> not bear to leave his life's work unfinished, so he has performed *The Fall of Lord Greve* every night for eighty
> years to a house he believes is empty, certain that **a play no one sees never happened.**
- 🎭 **A thirty-eighth zone — the Empty House** (a dark playhouse off the Night Market). Three lonelinesses braided so
  that each is the cure for the other two: **Orvel** the tragedian, performing to the dark; **Linnet** the understudy,
  who learned Lady Mourn to the bone and never once went on, eighty years word-perfect in the wings; and **Old Dorra**,
  the laundress who spent a month's coin for a front-row seat, died in it rapt on opening night, and has watched every
  performance since — the one true audience the player on stage has never looked down to see.
- 🎬 **The thesis as theatre.** Orvel has *become* Lord Greve — the play is about a man destroyed by needing to be
  remembered, and he is trapped by the very terror it warns against. A **[RETURNED]** names what a soul who died and
  came back would know: a performance no one sees doesn't *un-happen* — you played it, gorgeously, eighty years; **you
  are the work, not the applause.** And his house was never empty: *look down — front row — she has watched every
  night.* One true witness who stayed eighty years is the fullest house a player ever had.
- 🎟️ **A Performance check finishes the eighty-year play.** A non-passive social skill (Charisma) with full **crit and
  fumble** — step into the footlights and speak Lord Greve's missing final line yourself. **Crit:** you play his death
  truer than he ever did and he weeps as the play, suspended eighty years three lines from its end, is finished at
  last. **Fumble:** you call him *Lord GROVE* and ask the empty house to "please applaud" — and the horror cracks into
  the first laugh Orvel has laughed since he died.
- 🪞 **Two more freed by the same light.** Linnet is the *part she never played* — the **[RETURNED]** frees her to take
  the three paces onto the lit boards at last (the play needs a Lady Mourn; she's been ready eighty years). Dorra is
  the *witness who was never seen* — the **[RETURNED]** makes Orvel finally look down, so the laundress who chose
  beauty over bearing-up rises to her feet, applauding, the first thing he sees after eighty years of the dark.
- 👁️ **Three new Returned-senses.** 🧪 **+8 gates** (now **1,177 checks**, all green). **38 zones, 129 souls** —
  twenty-one structures, and the player has now been advocate, accused, thief, broker, mediator, judge, builder,
  comforter of a frightened house, and the soul who steps onto a dead stage to finish an eighty-year tragedy.

---

## 👑 v6.11.0 — *"The House That Remembers"* — a thirty-seventh zone: a sentient building (a soul that became its house) (content)

> A twentieth structure, and a player-role no zone has held: you don't free a soul, judge a dispute, or build a
> thing — you **un-frighten a house.** A carpenter named **Coll** loved his home so fiercely that when he died he did
> not move on but *poured himself into the timber and became the walls* — and now the new family is renovating him
> room by room, and because he is certain he *is* the timber, every changed board feels like a death.
- 🏚️ **A thirty-seventh zone — the House That Remembers** (down the lane off the Weeping House). Three loves tangled
  in one set of walls: **the House** itself (Coll, who became what he built); **Old Bessa**, the housekeeper who kept
  it forty living years and forty dead ones more, and who alone can hear what the walls are trying to say; and
  **little Wick**, a living five-year-old who half-hears the house's grief and has decided, with a child's whole
  heart, to be its friend.
- 🔨 **The thesis as carpentry.** The house grieves the *wrong thing* — it thinks it's the boards, so the sanding of
  the children's height-marks off the doorframe feels like being un-made. A **[RETURNED]** names the secret a soul who
  died and came back would know: *you were never the walls.* A house is the **love lived in it**, not the timber — and
  a life that *happened* cannot be sanded off. New paint, new family, new years only **add** to the vessel; a vessel
  doesn't die when new wine is poured in, *it fills.* The house isn't being un-made. It's being **deepened.**
- 💔 **Bessa's mercy hides a forty-year love.** She means to help the house "let go and move on" — until she's asked
  whether that's what *it* wants or what *she* needs, and admits she'd help a friend die to free herself from a love
  she was too afraid to live. The **[RETURNED]** offers a third way: the house doesn't have to *leave* — it can
  **stay**, stop being afraid, and let her finally say her forty-year part.
- 🕯️ **The child is the keystone.** Wick loves the house the way Coll did — not as good bones but as a *friend* — and
  the frightened house has been banging her shutters to drive her off. But she means to grow up on that doorframe, new
  height-marks every year: the house isn't being un-made, it's being **inherited**, and an inherited house never dies.
  The **[RETURNED]** carries her love into the cold so the house can *feel* it.
- 👁️ **Three new Returned-senses.** 🧪 **+7 gates** (now **1,169 checks**, all green). **37 zones, 126 souls** —
  twenty structures, and the player has now been advocate, accused, thief, broker, mediator, judge, builder, and the
  one soul who can tell a terrified house it was never the walls.

---

## 👑 v6.10.0 — *"The Cairn That Won't Stand"* — a thirty-sixth zone: a construction (build a beacon of true names) (content)

> A nineteenth structure, and a fresh *activity* — you don't free a soul or judge a dispute, you help **build**
> something. **Master Cobb** raised a lighthouse in life that brought ten thousand ships home, and now cannot make a
> single stone of a beacon for the lost dead stand — and the reason is the saga's whole thesis rendered as masonry.
- 🪨 **A thirty-sixth zone — the Cairn That Won't Stand** (off the last torch). Three souls: **Cobb**, the
  master-builder who has the vision and the craft and keeps failing; **Goodwife Pell**, his neighbor who hauls stones
  she's stopped believing will hold, just so he isn't building his grief alone; and **a Newly-Dead**, turning lost
  circles with no marker to find — the living proof of why the beacon matters.
- 🧱 **The thesis as masonry.** Cobb quarries his stones from the *honored* dead — GLORY, REMEMBRANCE, the names of
  kings — and they slide, because a king is held by ten thousand living mouths and needs no holding, *and a stone
  that needs no holding cannot bind.* **Investigation** finds it's the *material*, not the craft; a **[RETURNED]**
  names the secret: a marker for the forgotten holds only on *true names* — the small real ones, the forgotten,
  because *a name that needs holding is the only stone that binds.* (Pell, who's hauled every stone, has noticed it
  herself: the grand ones slide fastest, the small ones almost hold.)
- 🕯️ **The keystone is the lost soul itself.** The Newly-Dead — *Eddin, a fisher, drowned yesterday in a fouled net*,
  certain it's nothing — gives you its true name before the Hunger takes it, and *that* becomes the keystone: the
  foundation the whole beacon stands on, so no soul after it ever turns in the dark alone. *"Tell the next lost soul
  that finds its way by us — a fisher named Eddin is holding it up."*
- 👁️ **Three new Returned-senses.** 🧪 **+7 gates** (now **1,162 checks**, all green). **36 zones, 123 souls** —
  nineteen structures, and the player has now been advocate, accused, thief, broker, mediator, judge, and *builder.*

---

## 👑 v6.9.0 — *"The Harder Roads"* — payoff pass completed: the chronicler now reads all 18 structures (content)

> A cohesion pass. The side-roads payoff (v6.3.0) covered the first batch of structures; four more shipped since
> (the Connoisseur, the Custody, the Long Walk, the Moot). Now the epilogue Chronicler reads *all eighteen* — a
> fourth **harder roads** cluster pays off the newest, so nothing built is left out of the years-after.
- 📖 **A fourth Chronicler cluster — *the harder roads*** — reactive to the four newest structures:
  - **The Connoisseur** (`pd.the_fire_plan`): the soul-eater *cooked in the suffering he spent three centuries
    making, by the souls he thought too scattered to count* — not redeemed, *stopped*; the Half-Taken walks free,
    too loved to swallow. Filed under *justice — the cold kind.*
  - **The Custody** (`cu.child_freed`): Wren freed — neither tethered nor stored — having said a real goodbye and
    passed her promise to the Returned; Sefa grieves *clean* now; and the Factor *filed it under the column it had
    never once been able to use: released. It wept.*
  - **The Moot** (`mt.carry_is_key`): the century deadlock dissolved — the low fields drained and farmed, the deep
    village left flooded, a new cairn at the waterline (*farm above, not below*); the living's children fed, Wend's
    babies undisturbed, and young Sprat freed at last to walk the good door.
  - **The Long Walk** (`pl.arrived_goes_in`): Old Ferd took the last step; the whole creed re-aimed to *arrive useful,
    not proud*, and Tamsin keeps the halfway for every frozen pilgrim after.
- 🕯️ A graceful **default** as ever, for a run that walked other ways.
- 🧪 **+1 gate** (now **1,155 checks**, all green). **The years-after now pays off every one of the 18 side-structures
  built this run** — the side-content lands in the ending, complete.

---

## 👑 v6.8.0 — *"The Moot of the Drowned"* — a thirty-fifth zone: a collective-decision structure (content)

> An eighteenth structure — a **moot**, a crowd to sway rather than a single soul, and a genuine living-vs-dead
> conflict where *both sides are right.* A flooded village's dead have been deadlocked for a century: the living
> above the water want to drain the flood and reclaim the rich bottomland their children go thin without; draining it
> exposes the drowned village — the resting dead — to the sun and the plough.
- ⚖️ **A thirty-fifth zone — the Moot of the Drowned** (down from the Underbridge). Three voices: **the Tide-Reader**
  (the neutral keeper who calls the question and counts the even vote, year after year), **Old Mother Wend** (who
  votes to keep it drowned — the dead's rest), and **Sprat** (who votes to drain it — the living's bread).
- 🎭 **A real deadlock, honestly built.** The keeper lays out that it's *not good against evil* — the dead's rest
  against the living's bread, neither worth less, the tide bringing them up *even* every moot. Both rulings are
  defensible. But the keeper, sworn to neutrality, names what it has never dared say to the circle: *a moot held even
  for a century is not one that can't decide — it's one that doesn't want to,* because both sides are arguing the
  water and meaning something else.
- 🌊 **The thing under the water.** Wend isn't guarding the dead's rest — she's guarding *three small graves* (Bett,
  Corin, little Max who was two) from the sun, and can't bear the light to find her children. Sprat doesn't care about
  the turnips — he drowned at fourteen, cheated of a life, and wants *permission to move on*; the flood is the chain
  binding the whole circle. Each votes its secret heart and calls it a vote about a field — and neither, in a century,
  has once asked the other what they're holding.
- 🌉 **The [RETURNED] dissolves the tie instead of breaking it.** You can cross — carry the living's *true* need (not
  all the land, not the graves, perhaps just the upper fields) down, and the dead's *true* fear (not "no change ever"
  but "don't let the sun touch the children's graves") up — replacing a century of guesses with what the other side
  actually says. (The elder's and the young one's true-fear beats are gated on the keeper first revealing what lies
  under the water — real moot logic.)
- 👁️ **Three new Returned-senses.** 🧪 **+8 gates** (now **1,154 checks**, all green). **35 zones, 120 souls** —
  eighteen structures, and the player has now mediated not just a soul but *a hundred-year political deadlock.*

---

## 👑 v6.7.0 — *"The Long Walk"* — a thirty-fourth zone: a pilgrimage (a road of stations) (content)

> A seventeenth structure, and a *moving* one — a road, not a room. The souls who refuse to be ferried to the Court
> and **walk** it on foot, every grey mile a devotion. You meet three pilgrims at three stations of the journey —
> and because the Returned has *been* to the destination the whole creed walks toward, you can tell each of them the
> truth the road hides.
- 🥾 **A thirty-fourth zone — the Long Walk** (off the Grey Wayshrine). Three stations: **Brother Calwin**, three
  days set out, radiant and certain; **Goodwife Tamsin**, frozen at the doubting halfway, facing neither up nor
  down; and **Old Ferd**, who walked the whole three hundred miles and *will not take the last step.*
- 🧭 **Each station interrogates faith differently.** The **starter** believes the walking *earns* the Judge — and a
  **[RETURNED]** who's stood before Kelemvor can re-aim the creed (the god is drowning, not weighing; arrive *useful*,
  not proud) or disillusion it (the walking changes only the walker, never the Judge — a vanity with blisters). The
  **doubter** is stuck because she thinks the road has only two directions, both defeats — until you give her the
  *third axis*: turn sideways, *keep the halfway* for the next frozen pilgrim. The **arrived** soul is trapped by its
  own success — it became the walking so completely that finishing un-makes the only self it has — and only a soul
  that's been *through* the door can free it: *the door isn't the end of the walking; it's where the walking finally
  arrives somewhere it can do good. The work of the dead never finishes.*
- 👁️ **Three new Returned-senses.** 🧪 **+8 gates** (now **1,146 checks**, all green). **34 zones, 117 souls** —
  seventeen structures, and the saga has now examined faith from the inside (the disputation argued whether the Wall
  is just; the pilgrimage asks whether the *destination* is worth the walking).

---

## 👑 v6.6.0 — *"The Custody of the Small"* — a thirty-third zone: an arbitration (a trilemma over a dead child) (content)

> A sixteenth structure, and a player-role the saga hadn't asked of you: **arbiter.** Not a stuck soul to free, but a
> dispute to *judge* — a dead godless child, **Wren the Younger**, claimed by two parties with mutually exclusive,
> partly-legitimate needs, and a ruling you must deliver knowing every option has a cost.
- ⚖️ **A thirty-third zone — the Custody of the Small** (off the Lamplit Quarter). Three parties: **Goodwife Sefa**,
  the grieving mother who *will not let go* (tethering her daughter to the living world, fading her by the day — and
  she knows it, and can't stop); **the Factor**, a Choir clerk who'd take the child into *loveless grey storage* —
  eternal, safe, a six-year-old kept at six forever (the least-cruel column the Concord permits); and **the child.**
- 🎭 **A real trilemma.** You can **rule for the tether** (a child belongs with the soul that loves it, whatever it
  costs) or **rule for the storage** (safe beats dissolving) — both real, both costly, both gated on hearing each
  claim. But the twist is the indictment the Factor itself names: *a fortnight of fighting over the child's head, and
  neither party once asked the child.* Rule without asking and it lands as `cu.child_unasked` — a brave, breaking
  six-year-old smile that you came all the way over to it and decided *for* it anyway.
- 🌟 **The best path is the one nobody took: ask the child.** Wren chewed on it for a fortnight and has the answer
  ready — it isn't stuck between hurting its mother and breaking its promise (*"I'll mind you, Mama"*) if a grown-up
  will help it **mind her first**: a real goodbye (the thing the fading would steal), the promise *passed* to the
  Returned who comes back to check on her, and then the child goes **free** — neither tethered nor stored. The third
  door was always the child's to name; the grown-ups just never put the smallest voice *last, where the small ones
  count the most.*
- 💛 Even the cold Factor secretly hopes you find it — *"I would rather you found it than win... I'll file it under a
  column I've never once been able to use: released."*
- 👁️ **Three new Returned-senses.** 🧪 **+8 gates** (now **1,138 checks**, all green). **33 zones, 114 souls** —
  sixteen structures, and the player has now been advocate, accused, thief, broker, mediator, and *judge.*

---

## 👑 v6.5.0 — *"The Connoisseur's Parlor"* — a thirty-second zone: the saga's thesis, tested (content)

> A fifteenth structure — and the first that deliberately *interrogates* the saga's thesis instead of affirming it.
> Every monster on the causeway so far has been a wound you could witness back into a person — the forgotten Gnaw,
> the desperate warden. **Goodman Vell** is not. He is a soul-eater with *no wound under it*: an ordinary wine-taster
> who brushed another soul's memory, found it exquisite, and chose to keep eating, refining it into an art across
> three centuries — because he *likes* it. He cannot be witnessed home, because he does not wish to leave.
- 🍷 **A thirty-second zone — the Connoisseur's Parlor** (off the Night Market). Three souls: **Vell** the eater;
  **the Half-Taken**, a soul being savoured slowly and aware (the awareness is the seasoning); and **the Emptied**,
  a husk eaten past its name but not past one stubborn ember.
- 🪞 **The thesis, tested honestly.** An **Insight** read confirms what the saga wasn't built for — *there is no part
  of him to save; he is what any ordinary soul becomes when you remove every consequence and leave only the
  appetite.* A **[RETURNED]** names it: you carry the dead so they're not borne alone; he eats them so they're not
  borne at all — the same reach, pointed opposite — and this is *the first thing you can only stop.*
- 💛 **Witness still saves the innocent — just not the guilty.** The Half-Taken can't be fought free (struggle
  aerates the meal; the eater *wants* you to fight). But you can **witness the prey too whole to swallow** — hold the
  one thing he couldn't eat, an ordinary contented Tuesday, until the soul stops suffering, because despair is his
  whole palate and *a soul that knows it's held has no despair to season it.*
- 🔥 **The villain you *stop*, not save.** The husks he scattered (the same trick the Wall plays — scatter the
  forgotten so they never add up) each kept one bitter ember of grudge, the one flavour he can't swallow. The
  Returned, who *gathers* the dead, can carry them together into one room and one fire he must stand in — *cooked at
  last in the suffering he spent three centuries making, by the ones he thought too scattered to count.* Not
  redeemed. Not witnessed home. Just stopped.
- 👁️ **Three new Returned-senses.** 🧪 **+8 gates** (now **1,130 checks**, all green). **32 zones, 111 souls** —
  fifteen structures, and the saga's thesis is now *load-bearing under stress*, not just decorative.

---

## 👑 v6.4.0 — *"Dace and Orin, Years On"* — companion parity: the older companions get their epilogue codas (content)

> A parity pass. Sennet and the Sparrow each got the full four-beat arc (recruit → fire → threshold → epilogue);
> **Dace** and **Orin** had everything but the last beat. Now they have it — and their years-after lands the same
> weight as the newer companions'.
- 🗡️ **Dace Iron's coda** (gated on `party.dace_recruited`): the Faithless sellsword who hoarded one unspent *yes*
  for ten grey years and spent it on you. Years on, she turned a blade-for-hire into a **shield-for-free** — guarding
  godless paupers, almshouse-keepers, ferry-folk for no coin — and waits for the Wall *unafraid*, because she watched
  the one soul it lost walk free. A [RETURNED] "soldier to soldier" beat: the ten years weren't warm, they were
  *unspent*; you made her death worth anything at all; she'd throw in again every day there is, and that loyalty is
  the one thing she never sold.
- ⚖️ **Orin's coda** (gated on `party.orin_may_join`): the justiciar who traded a lifetime of certainties for one
  honest doubt at the threshold. Years on, she's a **counter-justiciar** — turning every skill the Choir gave her to
  *guard* the Wall against the Wall itself, finding the unlawful reckonings, naming the forged writs, arguing the
  Faithless free in the order's own language. A [RETURNED] exchange: she'd choose you again *precisely because* you
  might still go wrong and *know* it — certainty is the fall, doubt is the safeguard — and she watches still, ready
  to choose *for* you one last time if the day ever comes, praying it never will.
- 👁️ **Two new Returned-senses** for the codas (the Faithless chill melted by a cause spent freely; the soul that
  paid the highest price in the tally and counts it the best trade of its death).
- 🧪 **+2 gates** (now **1,123 checks**, all green). **All four recruitable companions now span the full saga** —
  recruit to years-after. **108 souls.**

---

## 👑 v6.3.0 — *"The Side-Roads"* — the payoff pass: the epilogue chronicler reads the new structures (content)

> Not a new zone — a **cohesion pass.** The fourteen self-contained side-structures built across v4.94–v6.2.0 all
> resolved beautifully in the moment, but they didn't *land* in the ending. Now they do: the epilogue's **Chronicler**
> ("Some Years After") gained a **side-roads** topic that reads, reactively, how each of them turned out — so the
> quiet deeds you did where no saga was watching get their years-after, the same as Wren and Pip and the rest.
- 📖 **A new Chronicler topic** — *the side-roads* — branching into three reactive clusters:
  - **The cellar courts** — reads `tr.annet_spared` and `as.verdict_trustworthy`: the midwife walked free *and* the
    Choir's summary reckonings were abolished within years; your own Assize became scripture among the dead, *the
    Returned who refused to be acquitted.*
  - **The bargains in the dark** — reads the Scrivener, the Advocate, the Heist, the Siege: Jergal *still auditing
    the Wall empty one true line at a time*; the Nine's precedent never set (the unclaimed protected forever); the
    Great Tally burned (the frail scattered, the many spared) or carried to Jergal (all freed whole); the almshouse
    that became the first of a whole network of open doors.
  - **The songs and the rooms** — reads the Deathsong, the Locked Room, the Thin Place, the Boasting-Ring, the Wake,
    the Disputation: the art that didn't die in a reed-bed; Coll who turned around and went to the supper kept warm;
    the reunion that un-did the Wall's deepest cruelty for three souls at once; the contest taught to stop being one.
- 🕯️ Every cluster has a **default** line too — a soul that walked other roads still gets an honest years-after, and
  the page is kept *whether or not you turned it.* (The richest single outcome on each axis narrates; the threads
  the player skipped quietly don't.)
- 🧪 **+5 gates** (now **1,121 checks**, all green): the topic exists, each cluster reads its outcomes reactively,
  and each has a graceful default. The side-content now *pays off in the ending* instead of sitting beside it.

---

## 👑 v6.2.0 — *"The Locked Room"* — a thirty-first zone: a memory-dive (free a soul by showing it the truth) (content)

> A fourteenth new structure — the **memory-dive** — and a new *mechanic*: you step *inside* a soul's worst night,
> a single minute worn into a groove ten thousand repetitions deep, and you free it not by argument but by showing
> it the memory **true** — because it remembers wrong. Guilt repainted the scene.
- 🌀 **A thirty-first zone — the Locked Room** (entered off the Reed-Walk, with your dead-touched senses). Three
  pieces of one frozen night: **Coll**, the dreamer looping on the cup, the cruel word, the flinch; **the Frozen
  Woman**, his wife Edda, held mid-gesture; and **the Way Out**, a window he will not turn to face.
- 🔍 **Real dive logic.** Coll relives the night as a *murder* — he said something cruel, she stormed out in fury,
  took a chill and died hating him. But reach into the frozen echo and the truth is the opposite: Edda knew him
  thirty years, forgave the frightened word *between the door and the step*, was planning the supper to make it
  right — and the fever that took her *didn't care what he said.* You can only **free** the dreamer once you've
  learned the true version from her first (the beats are gated on it) — and freeing him **breaks the loop** (the cup
  hangs unfallen; he turns, for the first time, to the window).
- 🪟 **The dive names the saga's deepest pattern.** The Way Out reads what every self-built cage on the causeway is
  made of — the Reed-Wife, the philosopher, the confessor, all of them holding their own cell doors shut from the
  inside — *not "I cannot leave" but "I do not deserve to."* The window was never locked. The key is never absolution
  from outside; it's the soul finally believing it's allowed to set the weight down and come to the supper kept warm
  past all reason, by a love waiting just past the glass.
- 👁️ **Three new Returned-senses.** 🧪 **+8 gates** (now **1,116 checks**, all green). **31 zones, 106 souls** —
  fourteen self-contained structures, every one resolved on *witness, not force.*

---

## 👑 v6.1.0 — *"The Last Deathsong"* — a thirtieth zone: a teaching structure (learn the craft) (content)

> A thirteenth new structure — the **teaching**, where you don't free a soul, you *learn a craft* — and it's the
> capstone of the saga's whole "witness through voice" thread (the wake's songs, the thin-place river-tune, the
> boast). **Old Mireth** is the last soul alive who knows the **deathsong**: the dying art of singing a soul out
> through the good door by singing it so *true* it believes, at last, that it was real.
- 🎼 **A thirtieth zone — the Last Deathsong** (a quieter song upriver of the wake). Three souls: **Mireth** the last
  cantor; **the Unsung**, a soul that won't be sung because it's certain it deserves to fade; and **Pen**, the
  doubting apprentice who learned the notes and not the heart.
- 📖 **You learn the craft.** The deathsong's secret is the saga's thesis made into a teachable art: *no magic notes
  — witness made melodic.* You learn a soul true first (its name, its life, its wound), then sing *that* back until
  it believes it existed, and a soul that believes it was real can open the door no god will force. Learning it sets
  a flag the player carries — and a **[RETURNED]** can promise to carry the song *through* the door, to the walled
  and the far-side fading, so the ten-thousand-year art doesn't die with its last master.
- 🎵 **The hardest first song.** The Unsung — a good physician undone by one tired fatal error — can only be sung
  once you've *learned it true* (the song beats are gated on hearing its confession). And you don't sing it clean,
  you sing it *finished*: the good and the killing both, until it hears its worst thing witnessed whole and learns a
  soul that's punished itself this faithfully has already served the sentence. The **[RETURNED]** can instead turn
  its penance inside-out — fading robs its victim of its last true mourner.
- 👨‍🏫 **The apprentice's correction** keeps the art alive past the last cantor: Pen sings flawless notes *at*
  strangers and wonders why nothing happens; teach it (or talk it off quitting) that the sitting-and-learning *is*
  the art, and the deathsong has two keepers instead of none.
- 👁️ **Three new Returned-senses.** 🧪 **+8 gates** (now **1,108 checks**, all green). **30 zones, 103 souls** —
  thirteen self-contained structures, every one resolved on *witness, not force.*

---

## 👑 v6.0.0 — *"The Boasting-Ring"* — a twenty-ninth zone, and the hundredth soul 🎉 (content)

> A milestone release: a twelfth new structure — the **contest** — that crosses the walkable saga past **100 souls**
> across **29 zones** and **1,100 green checks.** And the structure is the saga's whole thesis turned into a party:
> the dead stave off the Wall by telling the grandest *true* story of their lives, because to be witnessed, loudly,
> by many, is to be too vivid for the Hunger to un-make.
- 🔥 **A twenty-ninth zone — the Boasting-Ring** (off the Night Market). Three souls: **Big Sorrel**, the ringmaster
  who disguised the cleverest mercy on the causeway as a drinking game (a machine that holds the forgotten in the
  world, forty witnesses at once); **the Reigning Tongue**, the undefeated champion *starving behind his crown*
  because he's spent his one true story and started to lie; and **the Smallest Voice**, a temple-sweeper fading
  fastest because a gentle ordinary life doesn't boast easy.
- 🏆 **The structure pays off the whole saga.** Take the log and tell your own grand true story (you walked *out* of
  the Wall — the boast that breaks the ring). Read the champion (Insight) as a soul hollowed by false boasts, and
  free him by letting him *lose* — the one move the game can't counter. And reframe the sweeper's "nothing" — forty
  years keeping clean stone for the grieving to fall on — as the grand thing it always was; with the champion's
  yielded crown, the **humblest soul in the ring is crowned and saved**, fading stopped, proof a small life
  well-lived outboasts any dragon-slaying.
- 👁️ **Three new Returned-senses.** 🧪 **+10 gates** — the suite crosses **1,100 checks**, all green.
- 🌟 **The milestone:** the walkable causeway is now **29 connected zones, 100 souls, 973 dialogue beats** — a
  complete saga (Prologue → the Court of the Dead → the years after) threaded with **two full four-beat companion
  arcs** (Sennet, the Sparrow) and **twelve self-contained side-structures**: investigation, monster-redemption,
  riddle-vault, barter, companion-recruit, courtroom, god's-bargain, heist, self-trial, siege, wake, whodunit,
  reunion, disputation, confession, and contest — every one resolved by the saga's single thesis: *witness, not
  force; the third door, not the necessary cruelty.*

---

## 👑 v5.11.0 — *"The Unburdening"* — a twenty-eighth zone: a confession structure (witness, don't absolve) (content)

> An eleventh new structure — the **confession** — where *you* are the one who hears, and the mechanic is your
> *response.* Three penitents, three guilts, and a single hard-won lesson: absolution was never the confessor's to
> give; cheap grace traps, and witnessing-without-judgment is what frees.
- ⛪ **A twenty-eighth zone — the Unburdening** (off the Hearth). Three souls: **the Penitent Baker** (a small wrong
  that grew — a shut door, a child dead three streets over, carried as murder); **the Silent Watchman** (a *failure
  to act* — one unsaid word that would have saved a man); and **Goodman Vance** (the hard case — a real cruelty, and
  a soul that wants easy absolution).
- 👂 **Hearing comes before answering** — the response beats are gated on having heard the whole confession first.
  And the right-sizing is the cure: the baker isn't a murderer (carry the grief, set down the guilt); the watchman
  carries forty murderers' crime as his one silence (name it real, but one soul's worth, not forty's). A **[RETURNED]**
  can *carry* the weight into the cold, not to erase it but so it isn't borne alone.
- 🚪 **The unrepentant soul tests the whole thesis.** Vance has said *sorry* a thousand times and rages that the door
  stays shut — because he's sorry it's *kept* him here, not sorry he *did* it. Grant him the cheap grace he begs for
  and the door *stays dark* (cheap forgiveness is the chain — it lets him not feel it, and the not-feeling is what
  seals him). An **Insight** read names the performance; the **[RETURNED]** *refuses* the cheap grace — the harder,
  kinder thing — and that refusal is the first true mercy anyone's done him, because it makes him finally carry the
  weight until it lands.
- 👁️ **Three new Returned-senses.** 🧪 **+8 gates** (now **1,091 checks**, all green). **28 zones, 97 souls** — eleven
  fresh structures this stretch; three souls short of a hundred.

---

## 👑 v5.10.0 — *"The Unfinished Argument"* — a twenty-seventh zone: a disputation (free a soul by finishing its argument) (content)

> A tenth new structure — the **disputation.** A dead philosopher, Makeb the Elder, has argued the justice of the
> Wall with an empty chair for three hundred years, anchored not by guilt or grief but by his own *undefeated record* —
> a mind that won its great argument so thoroughly it forgot how to think. You free him not by losing to him, but by
> letting *him* lose, gracefully, for the first time in three centuries.
- 🗣️ **A twenty-seventh zone — the Unfinished Argument** (off the Market). Three souls, three anchors: **Makeb** the
  philosopher; **Sennia** the rival who walked out mid-argument and has refused the empty chair ever since; and
  **Doby** the student who keeps the minutes of a meeting that never adjourns.
- ⚔️ **A real debate.** **Insight** diagnoses him (you don't *want* to win — winning is the chain); **Persuasion**
  argues the negative (the nat-20 turns his own affirmative inside out: *necessity is the confession of a failed
  cosmos*, a defense that was always an indictment); and the argument escalates to the saga's whole thesis — **the
  third door**: *necessity is a failure of imagination wearing a fact's robe*, and he spent three centuries answering
  the wrong question flawlessly. The **[RETURNED]** finishes it with testimony — a witness who's *been* inside death
  beats a philosopher who argued it from a chair.
- 💔 **Every anchor is the same chain, worn differently:** Sennia won't sit not from pride but because finishing might
  prove her wrong — and beneath even that, the longest unsaid love in the after (she stayed *near him* three
  centuries rather than move on without the teacher who wounded her). Doby files forever because the quarrel is its
  excuse to stay near the two it loved. The keeper-student knows the real answer: *they're both right*, which is
  exactly why it never ends — and the way out is to aim the two finest minds on the causeway at the Wall *together*
  instead of at each other.
- 👁️ **Three new Returned-senses.** 🧪 **+8 gates** (now **1,083 checks**, all green). **27 zones, 94 souls** — ten
  fresh structures this stretch (bargain, trial, heist, devil-pact, self-trial, siege, wake, whodunit, reunion,
  disputation).

---

## 👑 v5.9.0 — *"The Thin Place"* — a twenty-sixth zone: a reunion across the Wall (the logistics of love) (content)

> A ninth new structure — the **reunion** — and the most intimate yet. At the Wall's foot, where the stone runs
> thin, a widow kneels nightly to hear her walled husband *breathe.* She can only listen. You are the one soul who
> can *cross.*
- 💔 **A twenty-sixth zone — the Thin Place** (off the last torch). Three souls: **Goodwife Pell**, who has knelt at
  the Wall for a year with a last word she never got to say; **a voice in the stone** — Aldous, her husband, walled
  and fading, near enough the thin place to be almost heard; and **the Reed-Knower**, who maps where the Wall goes
  thinnest.
- 📨 **A courier structure with real logistics.** Learn who Aldous was and the message Pell owes him (*you're not
  alone, I came back, I'm sorry I was late*), then — as the **[RETURNED]** who can walk back to the Wall's edge —
  carry it *through.* The beats at Aldous are gated on what you learned from Pell first: give him back his **name**
  (the Wall takes the *who* first, and a name handed back catches the scattering soul like a hand around a coal),
  deliver Pell's **message**, and bring his answer back — *she was never late; the last thing he felt wasn't fear of
  the dark, it was wishing he could tell her not to blame herself.*
- 🎶 **The reunion lands** in the saga's quietest triumph: you ask Aldous to sing the bad old river-song into the
  dark, and through the thin place Pell's voice — weeping — joins the filthy third verse, forty years and a Wall
  between them, *singing together.* "This is better than getting out. Let the dark come now. I'm singing with my
  wife."
- 🗺️ **The Reed-Knower** turned its worst failure into a map of mercy: it knelt at the wrong seam while its own
  daughter faded un-heard, and has spent forty years learning every thin place so no other love lands on deaf
  granite — and it will *witness* your crossing, the thing it could never do itself.
- 👁️ **Three new Returned-senses.** 🧪 **+9 gates** (now **1,074 checks**, all green). **26 zones, 91 souls** — nine
  fresh structures this stretch, the saga's thesis proven one more way: *the Wall's deepest cruelty is making the
  dead believe they are unheard, and every voice that crosses, even once, un-does it.*

---

## 👑 v5.8.0 — *"The Inquest of the Un-Made"* — a twenty-fifth zone: a whodunit with clue-gated accusation (content)

> An eighth new structure, and a genuinely new *mechanic* — **deduction.** A soul named Sella has been *un-made*:
> not walled, not moved on, but *erased*, root and memory, which the dead are not supposed to be able to do to each
> other. You gather clues from the souls around the absence and make a **clue-gated accusation** — and the solution
> is a Crown-of-Horns trolley problem.
- 🔍 **A twenty-fifth zone — the Inquest** (off the Underbridge). Three roles: **Old Gantry**, who keeps vigil over
  the soul-shaped hole; **the Bereaved**, who loved Sella and is losing even the memory of her to the erasure; and
  **the Quiet Neighbor**, the one soul in a terrified place who isn't afraid.
- 🧩 **Real deduction.** You can't accuse until you've gathered evidence from *both* the bereaved (who saw the
  Neighbor weeping with Sella the night before) and the absence itself (a **[RETURNED]** can read the clean,
  *channeled* erasure — a soul drained, not destroyed, by a hand that walked away fuller). Only with both clues does
  the accusation option unlock at Gantry.
- 🚃 **The twist is the saga's whole question, catchable.** The Neighbor isn't a predator — it's a **warden.** It
  found a Hunger-fragment in the old foundations and feeds it one carefully-chosen soul to keep it from eating
  *everyone*: the exact arithmetic of the Concord, run small and honest and alone, *right every time*, which is the
  horror. **Insight** reveals it's emptier, not fuller, each time. And a Returned who learned the bargain can hand
  it the **third door** — the cistern's lesson, that the Hunger feeds on being *forgotten*, not on death, so it's had
  the cure backwards, feeding the fragment the very forgotten souls it grows on.
- 🕯️ **Witness beats erasure:** the dead-touched player can carry the erased Sella *into the cold* where the un-making
  can't reach, so the gentlest soul on the Underbridge isn't gone — she lives, witnessed, in you.
- 👁️ **Three new Returned-senses.** 🧪 **+9 gates** (now **1,065 checks**, all green). **25 zones, 88 souls** — eight
  fresh structures this stretch (bargain, trial, heist, devil-pact, self-trial, siege, wake, whodunit).

---

## 👑 v5.7.0 — *"The Wake of Goodman Doget"* — a twenty-fourth zone: tonal relief, a celebration among the grief (content)

> After six grand structures of cosmic weight, a deliberate change of key: the one gathering on the whole grey
> causeway where the dead *celebrate.* A wake — a funeral-feast for a soul who died *fair* — warm and funny and full
> of songs, with the saga's ache threaded quietly underneath it, so the warmth and the cruelty throw each other into
> sharper relief.
- 🍺 **A twenty-fourth zone — the Wake** (upriver off the Reed-Walk). Three souls: **Tib Garron**, a publican who
  died pouring and throws wakes for the fairly-dead *in protest* against a cosmos that murders fairness; **Goodman
  Doget**, the guest of honor, an old farmer who lived sixty square years and died easy — the rarest thing on the
  causeway; and **the Uninvited**, a Faithless soul at the window, grieving the send-off it will never get.
- 🎶 **Warmth with the ache under it.** Doget can tell you, from the far side, exactly what a *fair death* feels like
  (the gold evening, the good ache, the earned sitting-down) — and then charge you not to *envy* it but to make it
  the *rule* instead of a one-in-ten-thousand miracle. Tib's wakes turn out to be a *drunk recurring protest*; the
  nat-warm toast radicalizes him. And the saga's softest wisdom: a turnip-farmer's secret that you save the cosmos
  the same way you farm an acre — one small right thing in front of you, repeated, until it adds up to something you
  couldn't see coming.
- 🕯️ **The small mercy that fixes the one crack:** you can **invite the Faithless soul in.** The window was only ever
  cracked open; throw it wide, and the wake stops being for the *deserving* dead and becomes one for *whoever the
  stone forgot to let be loved.* Or carry its song (a filthy dockhand's work-tune) so it gets the one wake the Wall
  can't reach — the kind that lives in someone else's mouth.
- 👁️ **Three new Returned-senses.** 🧪 **+7 gates** (now **1,056 checks**, all green). **24 zones, 85 souls** — and the
  causeway, for one firelit hour, remembers what it was all supposed to feel like.

---

## 👑 v5.6.0 — *"The Siege of the Almshouse"* — a twenty-third zone: a siege held by witness, not the sword (content)

> A sixth new structure — the **siege** — and, true to the saga, one whose win condition is *moral, not martial.*
> The Choir's enforcement arm, forty strong under **Justiciar Vane**, lays siege to the last almshouse in the
> Lamplit Quarter that will shelter the Faithless. You hold it — but never with a blade.
- 🛡️ **A twenty-third zone — the Siege** (off the Lamplit Quarter). Three roles: **Goodwife Orla** (a Wall-warden of
  nineteen years who *walked* and now runs the refuge), the **sheltered** (a Faithless mother and three children),
  and **Justiciar Vane** (the career man at the door, who took Orla's old post).
- 🗝️ **Hold it four ways, none of them a fight:** **rally the room** (Persuasion — nat-20 *un-names the slur*, turning
  cowering prey into a proud congregation forty enforcers can't bear to drag off with their own men watching);
  **evacuate** down a warden's old river-drain (Investigation — then stall Vane long enough to empty the room behind
  his own voice); expose the **writ as unlawful** (the forged Concord again — a career Justiciar can't risk serving
  on a fraud); or **stand in the breach** as the [RETURNED] (the enforcers serve the Wall, and cannot drag a child
  past living proof that the Wall can be escaped — their nerve breaks).
- 💔 **Vane is winnable, not beatable** — the nat-20 persuasion makes him realize he replaced the warden who walked,
  and walks himself, six years late; lesser successes get a deferral or a legal withdrawal. The sheltered mother
  reads how you fought, names her baby for the cured bridge-boy, and gives the siege its stakes in four names.
- 👁️ **Three new Returned-senses.** 🧪 **+10 gates** (now **1,049 checks**, all green). **23 zones, 82 souls** — six
  fresh structures this stretch (bargain, trial, heist, devil-pact, self-trial, siege), every one resolved by the
  saga's thesis: *witness, not force.*

---

## 👑 v5.5.0 — *"The Assize of the Returned"* — a twenty-second zone: the trial flipped (you are the accused) (content)

> A fifth new structure, and an inversion of the Reckoning Court: there you *defended* a soul from the Wall; here
> **you stand in the dock yourself.** The dead you touched convene an assize to judge not a *crime* but your very
> *being* — the charge that a soul which can walk back out of death is a hole in the one law that makes all the dead
> equal, and one unwatched century from becoming the tyrant. The whole trial reads your run.
- ⚖️ **A twenty-second zone — the Assize** (off the Grey Wayshrine). Three roles: the **Arbiter** (presides over a
  scale), the **Accuser** (speaks for the fear of you), and the **Witness** (the sum of every soul you touched).
- 🗣️ **You defend your existence**, and the defenses are gated on *how you actually played*: a **merciful** plea
  (your record of saved souls), an **honest** plea (you never lied about being a danger), calling the **Witness**,
  **bowing to the scale** (humility itself acquits — the Crown never kneels), or — the deepest beat — **refusing the
  court's authority entirely**: a soul that insists on judging *itself*, every day, forever, can't be acquitted,
  and that permanent open question is the one lock the Crown has never picked. The merciful/honest pleas escalate to
  the forward question — *what stops you becoming the Crown?* — answerable three ways (never be alone with the
  choice; distrust your own certainty; or the unbeatable *"I don't know if I can be trusted"*).
- 🪞 **The Accuser is revealed (Insight) as your own fear** wearing a hood — your inner prosecutor, given a stone to
  stand on. The lesson: don't *win* against it, *keep* it; the Last Returned fell not by losing this argument but by
  *evicting the arguer*. And it reads your party — a soul that built a crew of no-sayers is the *variable its
  forecast can't price.*
- 👤 **The Witness** is the gathered dead, its testimony reactive to a merciful vs ruthless run — and the truest
  acquittal is letting your dead-touched sense *meet your own ledger without flinching* (the one test the Crown
  cannot pass).
- 👁️ **Three new Returned-senses.** 🧪 **+9 gates** (now **1,039 checks**, all green). **22 zones, 79 souls** — five
  fresh structures this stretch (bargain, trial, heist, devil-pact, self-trial), all circling the Crown.

---

## 👑 v5.4.0 — *"The Advocate's Booth"* — a twenty-first zone: a negotiation with a devil (read the fine print) (content)

> A fourth new structure — the **devil's contract** — and one that turns the Jergal bargain inside out. Where the
> dust at the crossroads wanted nothing and dealt fair, **Maître Crede, Advocate of the Nine Hells**, wants
> *everything* and hides it in the places he doesn't point. He's come because the Wall is a *loophole*: the Faithless
> go to neither heaven nor hell, so Hell is being cheated of its due — and a contract-devil means to probate the
> largest unclaimed estate in the planes. The whole game is *reading.*
- ⚖️ **A twenty-first zone — the Advocate's Booth** (off the Night Market; built from the existing prop vocabulary).
  Three souls: the **devil-advocate**, a **bonded soul** mid-collection, and the advocate's **bound clerk.**
- 📜 **A contract you read, not sign.** **Investigation** finds the trap (Clause the Ninth: the advocate-of-record
  becomes the *final entry* on his own manifest); **Insight** finds the real motive (the Faithless aren't the estate
  — they're the *test case*: establish that *unowned means open* and every unclaimed soul in creation falls to Hell,
  with *you*, the un-fileable Returned, as the keystone precedent). The **[RETURNED]** reading *voids his whole
  estate* with the loophole he came to close — *unowned means unownable, by Hell too; you can't sign over what was
  never owned.* And refusing is its own beat: he's not the villain, he's *the reasonable offer you were too proud
  to take*, and time is on the side of the Nine.
- 🪤 **The bonded soul** — a father who bought his daughter sixty hale years with himself as collateral, re-sealing
  the debt every dawn out of love — can be **witnessed to peace** (a debt agreed-to can't be picked, only re-framed)
  and gives the booth's true lesson: *a fair deal with Hell is still a deal with Hell; the trap isn't his paper, it's
  your own wanting.*
- 😈 **Sevenpence, the bound clerk** — an imp four thousand years into a ninety-year sentence — can be **freed**: his
  padded extension-clause never defined *whose* judgment rules a task complete, and a Returned has standing among the
  dead to declare it done. He also leaks the devil's one fear: being made a *signer* himself.
- 👁️ **Three new Returned-senses.** 🧪 **+7 gates** (now **1,030 checks**, all green). **21 zones, 76 souls** — four
  fresh structures this stretch (bargain, trial, heist, devil-pact), all orbiting the one question: *what is a soul
  worth that no god ever claimed?*

---

## 👑 v5.3.0 — *"The Sparrow's Wing"* — the fourth companion's arc completes (Threshold + epilogue) (content)

> The Sparrow gets the full companion treatment Sennet got — but with a twist that's hers alone. She is *dead*,
> and so, unlike the living companions checked at the Threshold, **she is the one soul in your party who can follow
> you through the death-door.**

- 🚪 **The Sparrow at the Threshold** (gated on `party.sparrow_recruited`). While Dace, Orin and Sennet are held at
  the edge by the breath still in their bodies, the dead thief simply *examines* the death-door like any other lock
  — and she's the *exception* the door makes (a soul it already let out once is a Returned-shaped crack she can slip
  through in your shadow). A real fork: **take her through** (she comes not as a bodyguard but a *burglar* — fades
  into the throne room and scouts your way *back out before you need it*, the first rule of the trade), or **have her
  hold the door** (you won't march a just-mended, unbought soul back into the one house with a cage for her name).
- 📜 **Her years-after coda** (joining the epilogue's conditional souls). The dead don't age — they *deepen* — and
  the Sparrow turned the line you tossed her in the archive into a *life*: she **steals people back.** A whole crew,
  the **Sparrow's Wing**, picking the locks on the after's remaining cages — no fee, no buyer, the freed soul its own
  reward. Four threads: the trade she built; why she stays instead of taking her earned rest (purpose over freedom);
  a **[RETURNED]** reckoning between two trespassers of death (she'll walk the good door at the last, *empty-handed,
  every cage picked clean behind her*); and the ones she was too late for (carried not as guilt but as instruction).
  A reactive opening reads whether she scouted the throne-room exit.
- 👁️ **Two new Returned-senses** — the one party-member for whom the Threshold is a *workplace*, and the soul that
  changed its whole nature without losing its edge.
- 🧪 **+4 gates** (now **1,023 checks**, all green). The Sparrow now spans heist → fire → door → years-after, exactly
  like Sennet — two complete four-beat companion arcs in the walkable saga. **73 souls.**

---

## 👑 v5.2.0 — *"The Sparrow"* — a fourth recruitable companion, turned mid-heist (content)

> The heist's inside thief becomes the saga's fourth companion. The Sparrow took the Tally job for the coin and
> didn't look at the cargo — but she has *one* rule, *no stealing people*, built around the one early crime she
> couldn't live having done twice. Show her the Great Tally *is* that crime ten thousand times over, and she turns
> herself off the job — and you can take her on.

- 🃏 **The Sparrow is now recruitable (`party.sparrow_recruited`).** Two routes: the **[RETURNED]** path (name what
  she's really stealing and she turns and joins in one stroke), or — once she *doubts* the job (ask about her buyer,
  or read her with Insight) — a plain recruit offer: *steal people back instead of fencing them.* She joins on her
  own terms (picks the locks, keeps her cut of anything that isn't a person, and you never ask her to leave a soul
  behind a door she could've opened).
- 🔥 **She takes a seat at the Hearth fire** (joining Dace, Wren, Pip and Sennet among the conditional camp souls).
  A full fireside conversation with four threads: how a soul ends up a *thief of the dead*; the story behind her
  **one rule** (an early job, a child-shade she lifted for a buyer before she learned to ask *why* — the wall she
  built around the one crime she couldn't repeat); a **[RETURNED]** "we both go where we're not allowed" exchange
  (you introduced the proud hands to the ashamed jobs, and she's a *whole person* for the first time in forty years);
  and whether she regrets walking away from the richest score of her life (she doesn't — *you can't fence a seat at
  a fire; it's only worth anything given*). A reactive opening reads whether you delivered the Tally to Jergal.
- 👁️ **A new Returned-sense** for her at the fire: a soul caught in the act of *integration*, the two halves it kept
  in separate pockets for forty years finally settling into one.
- 🧪 **+3 gates** (now **1,019 checks**, all green). **Four companions** now — Dace the sellsword, Sennet the
  cartographer, Orin the justiciar, and the Sparrow the reformed thief. **71 souls.**

---

## 👑 v5.1.0 — *"The Deep Archive"* — a twentieth zone: a heist (steal the master ledger of the Faithless) (content)

> Another structure the saga hadn't done: a **heist** — crew, obstacle, score. Down a darker stair from the
> Counting-House sleeps the **Great Tally**, the Choir's master ledger of *every* Faithless soul promised to the
> Wall — ten thousand names in one black book. It's in an *unlocked* room, because the Choir never dreamed a soul
> would be fool enough to *want* it. You are exactly that fool.

- 🦅 **A twentieth zone — the Deep Archive** (off the Counting-House; built from the existing prop vocabulary). The
  three pieces of a heist: a **crew** (the Sparrow, a professional thief already inside), an **obstacle** (Brother
  Codex, the soft night-watch), and the **score** (the Great Tally itself).
- 🃏 **The Sparrow — a partner-or-rival fork.** A second-storey soul who took the job for the coin and didn't think
  about the cargo. Partner her (she fully intends to cross you — and knows you know — so it's an *honest*
  partnership), read her with **Insight** and drop the dance, or — the best beat — let the Returned tell her *what*
  she's actually stealing (ten thousand people, a slaver's manifest), and watch a thief with one unbreakable rule
  *turn herself* off the job.
- 🚪 **The night-watch — a four-approach obstacle**, each with its own skill and fork: **Deception** (pose as the
  Canon's auditor — lands *harder* if you cracked Mereth upstairs, since the gossip of her crisis is real; nat-20
  has Codex *helping you carry the book out with airtight paperwork*; nat-1 has you call the Canon the wrong name),
  **Stealth** (slip past; nat-20 plants a draught to walk him the wrong way), a **[RETURNED]** turn (he's another
  unbought clerk like Pell — make him the man who *guarded the door by failing to*), or **Intimidation** (cow him,
  or panic him into ringing the alarm).
- 📕 **The score is a genuine choice with genuine trade-offs:** **burn** the book (frees all at once — but a list is
  also a *tether*, and the frailest, most un-witnessed souls *scatter* with no anchor); carry it to **Jergal** to be
  audited empty name by name (saves them *whole* — gated on having met the dust at the crossroads); **cut out Venn's
  forged founding page** so the whole ledger rests on no lawful authority (gated on confirming the fraud — the
  Sparrow's favourite, a one-page score that makes ten thousand worthless); a **[RETURNED]** option to *consult the
  ten thousand souls themselves* (they choose the patient mercy, the strong electing it for the frail); or grab-and-run
  if the alarm's already up. The Sparrow reacts to each.
- 👁️ **Three new Returned-senses**, including drowning in the book's *ten thousand held breaths.*
- 🧪 **+12 gates** (now **1,016 checks**, all green). **20 zones, 70 souls** — and the saga's three new structures
  (god's bargain, courtroom, heist) all interlock through the one black book.

---

## 👑 v5.0.0 — *"The Bone Scrivener"* — a nineteenth zone: a god's bargain (and the 1,000-check milestone) (content)

> A whole new *structure* — the **god's bargain** — and a reach deep into Realms lore for the broker. Off a dusty
> side-path from the Grey Wayshrine sits **Jergal**, the *First Lord of the Dead*: the being who held Kelemvor's
> office aeons before him, invented the very idea of a ledger of souls, and then grew so *bored* across eternity
> that he handed the whole grim portfolio to three hungry mortals and took a clerk's desk. He wants nothing, fears
> nothing, lacks nothing — which makes him the one honest broker on the causeway, and his bargains the most
> dangerous kind: *fair.*

- ✶ **A nineteenth zone — the Bone Scrivener** (off the Wayshrine; built from the existing prop vocabulary). Three
  souls: the **broker** (Jergal), the **one deciding** (Goodman Harl, a father wavering over the price to save his
  godless daughter), and the **one who already paid** (a nameless wisp who gave up its own name to strike a child
  off the Wall, drifting un-grieving into the quiet).
- ⚖️ **The bargain itself — a structure, not a switch.** Jergal can *un-write* a soul from the Wall's rolls
  entirely — not crack it, *erase* it — but the book must sum to zero, so the credit is a piece of yourself, paid
  in a currency the ledger accepts: a **year** of your life (from the grey far end you'd never notice), a **memory**
  (the warmest you own — heavier, so it balances a whole soul), or the heaviest coin, a **name** (a [RETURNED]
  beat: your *own*, the one the harvest already half-took — finishing the Wall's theft yourself to rob it of the
  finishing). Each strikes a Faithless soul truly free.
- 🧮 **The clever path: out-account the inventor of accounting.** Learn that the ledger sums to zero and that *you*
  are a credit spent and returned — money the universe owes itself — and a **Persuasion** check lets you *assign*
  that standing debt instead of paying a new coin, striking the soul free for nothing but wit (gated on having
  learned the ledger-law; nat-20/nat-1). The **nat-20** escalates to the structure's biggest swing: the Wall is
  revealed as a *vault of uncollected debts*, and a delighted Jergal sets out to spend his eternity of tedium
  gleefully **auditing the whole thing empty.**
- 🚪 **Refusing is its own honest path** — name the bargain as the Wall's arithmetic in a politer robe, and Jergal
  (who'd *like* to be put out of work) tells you the real answer: don't buy souls off one year at a time, **empty
  the book of the lie at the throne.**
- 👁️ **Three new Returned-senses**, including reading a *god who set itself down* — tedium worn into bedrock, the
  one being with no angle at all.
- 🧪 **+11 gates** — and the headless suite crosses **1,004 checks**, all green. **19 zones, 67 souls.**

---

## 👑 v4.99.0 — *"Annet, Years On"* — the Reckoning Court pays off in the epilogue (content)

> A side quest deserves a coda. Spare Goodwife Annet at the trial and, in **"Some Years After,"** you find her old
> and stubbornly above-ground — a borrowed newborn asleep on her shoulder, a midwife's bag still slung across her,
> caught breech two dawns ago. She was *meant to be mortar.* Instead she got more years of the exact work they tried
> to damn her for.

- 👶 **Annet's epilogue coda** (gated on `tr.annet_spared`, joining Wren, Pip and Sennet among the conditional
  years-after souls). **Three reactive openings keyed to *how* you won the trial:** the reeve's public recantation
  (`tr.reeve_recants` — a thousand names nailed to the Counting-House door broke the quiet reckonings for good), the
  voided Concord (`tr.concord_void` — the thorough prosecutor died at his desk un-writing the Wall's forged
  paperwork), or a default.
- 🌱 **The payoff is the saga's thesis, made concrete.** She spent her reprieve *seeding the causeway*: hundreds more
  children caught, four godless girls trained to carry the work, and a whole generation walking around wearing names
  she chose — each one a thank-you she couldn't say aloud to the stranger who vanished. A [RETURNED] exchange lets
  two godless souls settle the question the Wall was built to punish: *no god ever caught her — a person did, and
  that's not the consolation prize, that's the whole sermon.*
- 👁️ **A new Returned-sense for her coda:** the dead-touched player reads not one saved midwife but a *lineage of
  catching* — a single un-walled soul whose warmth propagates, the loudest answer in the tally to *what is a soul
  worth that no god ever claimed.*
- 🧪 **+2 gates** (now **995 checks**, all green): the coda is gated to sparing her and reads the specific way the
  reckoning broke. **64 souls** now walk the causeway.

---

## 👑 v4.98.0 — *"The Reckoning Court"* — an eighteenth zone: a courtroom side quest (defend a soul from the Wall) (content)

> A structure the saga hadn't done yet: a **trial.** The Choir holds summary *reckonings* in a cellar under the
> Counting-House — quiet, gallery-less, a form completed in the dark — to condemn the Faithless to the Wall. Tonight
> the accused is **Goodwife Annet**, a godless midwife who caught half the Quarter's children across forty years,
> charged with the one-line crime of having *served no god.* You are the only gallery she gets. Be the trouble.

- ⚖️ **An eighteenth zone — the Reckoning Court** (down a new stair from the Counting-House; built from the existing
  prop vocabulary — the big scale of judgement, a dock, the prosecutor's desk, banners and braziers). Three roles:
  the **accused**, the **prosecutor**, and the **reeve who pronounces.**
- 🗣️ **A real trial you argue, many ways.** Against **Brother Sevard** (a *thorough* man, not a cruel one — he reads
  the deeds before the indictment and has filed eleven years of midwives behind a locked door): **Persuasion**
  (a tax on kindness — nat-20 turns the Wall's own theology against it: walling the *witnessed* to dam the
  *unwitnessed* feeds the sea it's meant to hold back), **Insight** (the charge names a *state of being*, not an
  act), **Intimidation** (a mob of grateful mothers — nat-20 threatens the one thing a clerk can't survive: a
  *public record*), and two **evidence-gated** decisive arguments that pay off the Counting-House climax — the
  forged founding Concord (`ch.venn_confirmed`) and the High Measurer's cracked faith (`ch.mereth_cracked`).
- 👁️ **A [RETURNED] argument unique to you:** the only soul who ever walked back out of the Wall can give **sworn
  testimony** to what "forfeiture" actually does — opening the closed door in the middle of the courtroom.
- 🔨 **A two-step verdict.** Sparing Annet collapses the prosecution first (`tr.annet_spared`); only *then* does the
  **Reeve Oddleif** acquittal unlock — and he's a magistrate who sold his conscience for a warm room and a pension,
  whom you can move (Persuasion nat-20 makes him *count* how many of his thousand sentences were Annets) to recant
  every pronouncement and vacate the bench. Each role has nat-20/nat-1 forks, a [RETURNED] line, and a Returned-sense.
- 🧪 **+10 gates** (now **993 checks**, all green): the zone is keyed/reachable, the argument paths and crit/fumble
  forks are present, the evidence gates open only with the upstairs flags, the testimony path is Returned-only, and
  the acquittal is gated on the prosecution collapsing first. **18 zones, 63 souls.**

---

## 👑 v4.97.0 — *"Sennet, Years On"* — the cartographer's arc completes in the epilogue (content)

> The third companion's thread now runs the full length of the saga: recruited at the **wagon**, warmed at the
> **fire**, held back from death's door at the **Threshold** — and now paid off in **"Some Years After."** Save
> Sennet and, years on, you find an *old* soul at a proper table under the trees, grey-haired and impossibly alive,
> the first true atlas of the dead finished and clasped before them. It is the single clearest proof in the whole
> tally that the Wall's slow harvest can be *reversed*.

- 📖 **Sennet's epilogue coda** (gated on `party.sennet_recruited`, joining Wren and Pip among the conditional
  years-after codas). Three reactive openings keyed to the arc: the **eyes-past-the-door** Threshold choice (you
  went *in* so they could stay on the warm shore and draw), the **devoted** nat-20 recruit (your name printed in
  the title cartouche, where it outlasts you both), and default.
- 🗺️ **Four threads pay off:** the **atlas** itself (banned by the Choir, but a banned book in a *cracked* church is
  one everyone reads — now chained in every temple of Kelemvor for the bereaved, so no soul who loses theirs has to
  read *"here be terrors"* on a blank page); **Tibb** (a master cartographer now, with his *own* frightened
  apprentice, who keeps the one-line letter pressed in the back of the atlas); the **cold** (a [RETURNED] beat — the
  surveyor's frost *receded* where yours never can, so they drew you a true **map home** for the day your own road
  runs back to the grey country, pressed into the last page); and whether they were **glad** of the years (they were
  a soul racing its own death so it never had to sit still long enough to feel how alone it was — you gave them
  *time enough to be happy, and the company to be happy with*).
- 👁️ **A new Returned-sense for the epilogue:** the dead-touched player reads a soul that was *measured for an edge
  and got a hearth instead* — the Wall's harvest, run all the way to its end and *lost*.
- 🧪 **+2 gates** (now **983 checks**, all green): the coda is gated to recruitment, reads the Threshold choice, and
  pays off the atlas and the map-home. **60 souls** now walk the causeway — and one of them grew old.

---

## 👑 v4.96.0 — *"Sennet at the Edge"* — the cartographer follows you to the door the living can't cross (content)

> The saga's most pointed companion beat. Recruit Sennet and they follow you everywhere — including to the
> **Threshold of the Crown's Road**, the lip of the deep country they've spent their whole life charting from a
> half-mile off through a shaking spyglass. And here is the cruelty the surveyor never budgeted for: the map runs
> *all the way to this line, and not one inch further*, because past it is the country of the dead, and they are
> still — for the first time they have ever resented it — *alive.* The one survey that matters is the one step
> their living feet cannot take.

- 🚪 **Sennet appears at the Threshold** (gated on `party.sennet_recruited`, the conditional-NPC system already
  used for Dace and Orin at the edge). They stand an arm's length from the death-door, pen forgotten, white-knuckled
  — because to a cartographer of the dead the open door is not a horror but the *last unmapped mile*, and every
  honed instinct says *step through.*
- 🧭 **A conversation about being held back from the thing you most want.** Reactive openings keyed to how you bonded
  (the fireside *baseline* talk, the *devoted* nat-20 recruit, or default). Four resolutions: **order them to stay**
  (they needed it to be a command, not a request — so it isn't theirs to lose); **ask for the bearings** (the craft
  is the one floor that never gives way — they steady instantly and chart your crossing, *count your steps, same
  number coming back*); a **[RETURNED] "be your eyes"** beat that turns *staying* into the job (a one-station
  baseline is just a man crying at a door); or **invoke Tibb** (*I'll mind the wagon till you're home* — the one
  word that moves their feet off the edge).
- 👁️ **A new Returned-sense for the Threshold:** the player feels the deep country *pulling* at the living surveyor —
  the least-defended soul imaginable against that drag — and reads that they're in danger not of dying here but of
  *walking in on purpose, gladly, clear-headed.*
- 🧪 **+2 gates** (now **981 checks**, all green): Sennet is gated to the edge only when recruited, and the scene
  carries the hold-back path, the [RETURNED] eyes-beat, and the new sense. **59 souls** now walk the causeway.

---

## 👑 v4.95.0 — *"Sennet at the Fire"* — the recruited cartographer joins the Hearth denouement (content)

> A companion you can recruit should *show up.* The Hearth already gathers exactly the souls you saved — Dace
> off-watch, Wren with her apple, Pip sleeping with both eyes — so the new third companion now takes the third
> chair at the fire, with a full reactive fireside conversation of their own.

- 🔥 **Sennet now appears at the Hearth** — gated on `party.sennet_recruited`, using the existing conditional-NPC
  engine (`npcVisible`/`activeNpcs`/`blockedNow`). Recruit them at the wagon and they're at your fire that night;
  don't, and the chair stays empty.
- 🗺️ **A fireside conversation that reads how you took them on.** Four reactive openings: a **devoted** variant if
  you recruited them with the nat-20 persuasion (they put your name on the title cartouche; now they're drawing
  *you*), a **merciful**-disposition variant, a wary **ruthless** one (the surveyor takes a bearing on *your* cold
  and tells you, plainly, what country it leads to), and a default. Talk-threads: what they're really drawing
  (the *camp*, each soul labelled — the first map they've ever wanted to be *in*); the state of the frostbitten
  hand (the creeping cold has **stopped advancing** now they're off the edges — Tibb's count of "two, maybe three"
  isn't two anymore); a Returned-tagged "two surveyors, one baseline" exchange; and whether they wrote to Tibb
  (they did — and the lad wrote back one line).
- 👁️ **A new Returned-sense for Sennet at the fire** — the dead-touched player *watches the Wall's cold recede
  from a soul for the first time*: you didn't just recruit a surveyor, you pulled one more off the long grey road.
- 🧪 **+2 gates** (now **979 checks**, all green): Sennet is correctly gated to appear only when recruited, and the
  fireside talk carries the devoted variant, a [RETURNED] line, and the new sense. **58 souls** at the causeway.

---

## 👑 v4.94.0 — *"The Mapmaker's Wagon"* — a seventeenth zone: a recruitable third companion + a grief-redemption fork (content)

> Back to what matters most — story. A new zone off the Hearth road that does two things at once: it gives the
> party a **third companion** with a premise no other CRPG has (a cartographer who *surveys the afterlife*), and
> it hangs a complete moral fork off a single quiet antagonist — a patient buyer who turns out to be a grieving
> widower. The recruit only works *because of who you are*: a Returned is the one soul who can walk the blank
> middle of a map of the dead and come back to report the bearings.

- 🗺️ **A seventeenth zone — the Mapmaker's Wagon.** A lantern off the Hearth road; built from the existing prop
  vocabulary plus one new hand-drawn **covered wagon** (canvas sewn from map-margins, a chart spilling off the
  tailboard). Three souls: **Sennet**, **Tibb**, **Goodman Vael**.
- 🧭 **Sennet, Cartographer of the Dead — a third recruitable companion.** They are drawing the first *true* atlas
  of the afterlife — survey-grade, taken by standing too close to death and *measuring* — and it is killing them
  a lost year of warmth at a time. Because the **Returned** goes *in* and comes *out*, you can offer to be the
  surveyor where no living foot can go: recruit them (sets `party.sennet_recruited` + `cg.sennet_lives` + the
  `cg.atlas_quest`), and finish their life's work without it finishing them. A second **Persuasion** hire-path
  carries nat-20/nat-1 forks (the crit puts your *name on the title cartouche*; the fumble calls it "death-doodles"
  and wounds them).
- 🕯️ **Tibb, the apprentice, worried sick** — surfaces the cost: an **Insight** read (or the Returned's own
  kinship) reveals Sennet is *dying* (`cg.sennet_dying`), which unlocks the cutting clue-gated Insight at Sennet.
- 🤝 **Goodman Vael — a grief-redemption fork.** A patient buyer who wants the *working* door-map, not a
  comfort-copy. Only the Returned (or an Insight DC 15) sees the truth: a **widower** chasing a door to his dead
  wife. **Intimidate** him and a **nat-20 routs him** by naming the one thing his patience can't outlast — *she'd
  have hated it* (`cg.vael_redeemed`, a mercy remembered); a nat-1 or a flat threat only digs him in. The Returned
  can also warn him, soul-to-soul, that the doors lead only to the cold.
- 🎭 Full deep stack as ever: reactive openings (on `court.met_kelemvor` / `lt.wall_stirs` / godless / Choir-friend),
  the **Returned-sense** on all three souls, a [RETURNED] line apiece, dispositions, and approval/regard axes.
- 🧪 **+11 gates** (now **977 checks**, all green): the zone is keyed/reachable/collision-clean, both recruit paths
  set the party flag, the dying-clue gates the Insight, Vael's redeem/dig-in fork carries nat-20/nat-1, and his
  secret is reachable. **17 zones, 57 souls** — and the party has a third chair at the fire.

---

## 👑 v4.93.0 — *"The Old Cistern"* — a sixteenth zone: a monster-with-a-reason side quest (redeem or destroy) (content)

> Another self-contained side quest, another fresh structure: the Planescape move — the *monster you can redeem
> instead of kill.* A thing is eating a neighbourhood's *memories* from an old cistern, and it turns out to be a
> fragment of the saga's own **Hunger**: a soul forgotten so completely she curdled into a monster. The way to
> "defeat" her is the saga's whole thesis — *witness her back into a person.*

- 🕳️ **A sixteenth zone — the Old Cistern (side content).** Reached from the Underbridge; the map now branches in
  two directions (the Vault and the Cistern) off one hub. Built from the existing prop vocabulary (no new art).
- 👁️ **A monster-with-a-reason, with a redeem/destroy fork.** The **Gnaw** rises at dusk and *thins* you — you
  literally lose half-known names mid-conversation. But it is not malice; it is a starvation so total it has eaten
  even the memory of having been full. Resolutions: **witness it back** (gated on learning its name — the soul
  surfaces, becomes a person, and the eaten names return to the row); **intimidate it** (a force fork with
  nat-20/nat-1 — you save the row, but only frighten the thing back into the dark, hoard returned, soul *not*
  saved); or, if it's too far gone and you've no name, **end it** with regret (the true cost of looking away:
  by the time someone finally looks, it can be too late to do anything but close the eyes you were too slow to
  meet).
- 🔎 **The name is the weapon, and you have to earn it.** **Mother Sedge**, a washerwoman, is losing the people
  she loves and can't even grieve them (a little dress she folds and can't place). **Old Berin** holds the
  forgotten one's name — *Nettie*, a pauper-sweep who fed the strays and froze unmourned by the cistern — not out
  of love but out of *shame*, the one kind of remembering that won't let go. Learn it from him to call her home.
- 🎭 Full deep stack: reactive openings, the **Returned-sense** on each soul, a Returned line apiece, dispositions
  — and the Gnaw's whole conversation is written to *thin* you (names dissolving mid-scene) until you arrive
  carrying one it can't take.
- 🧪 **+8 gates** (now **966 checks**, all green): the side zone is keyed/reachable/collision-clean, the monster is
  revealed as a fragment of the Hunger, witnessing it back is gated on learning Nettie's name, the rememberer
  surfaces it, both redemption and sorrowful-end paths resolve, and the force fork carries nat-20/nat-1. **16
  zones, 54 souls** — the world keeps growing.

---

## 👑 v4.92.0 — *"The Weeping House"* — a fifteenth zone: a self-contained investigation/mystery side quest (content)

> The main saga is narrative-complete, so this one grows the *world* — the bible's "100+ NPCs, many rewarding
> side quests." A classic CRPG **haunting**, with a structure the walkable demo hadn't shown yet: an
> **investigation.** A townhouse in the Lamplit Quarter where a family of the dead can't move on, and only you —
> who sees the dead — can solve *why.*

- 🏚️ **A fifteenth zone — the Weeping House (side content).** Reached by a side door off the Lamplit Quarter; the
  map is no longer a single line but a hub with optional branches. Built from the existing prop vocabulary (no
  new art).
- 🔎 **A real investigation/mystery loop.** The resolution is *gated on detective work*: you can't free the family
  until you've gathered the clues. **Tam**, the bored little ghost, saw the truth on the first night — *Papa went
  up the bright door ages ago; Mama was too busy counting the silver to follow* — but no grown-up will hear a
  child. **Goodwife Harl**, the housekeeper, has guarded the freeing truth for forty years out of pure loyalty
  (*the hearth-guard is latched; the Lady is blameless; it was the master's lamp*). And an **Investigation check**
  reads the house itself — the latched guard, the cold hearth, the table laid for a dinner nobody started.
- ⚰️ **The Lady of the House** — frozen at the instant of her worst choice, keeping a burned household running by
  sheer will because *counting* is the wall she's built against the one night she chose the silver over her
  husband's hand. Confront her with the gathered truth (a gated Persuasion, with a nat-20 breakthrough and a
  nat-1 where the truth lands as a *stone* and reseals her) and the whole house finally goes up the warm stairs
  together — she had the *direction* of her waiting wrong all along.
- 🎭 Full deep stack: reactive openings, the **Returned-sense** on each soul, a Returned line apiece (Harl's is
  the keystone — only a thing unbound by service can say the unkind kind thing she couldn't), dispositions.
- 🧪 **+8 gates** (now **958 checks**, all green): the side zone is keyed/reachable/collision-clean, the
  confrontation is **gated on having investigated for the clues**, the child and housekeeper each surface a clue,
  the Investigation check reads the physical evidence, and solving the haunting frees the family (with a crit-tier
  breakthrough). **15 zones, 51 souls** — a complete saga *and* its first proper side quest.

---

## 👑 v4.91.0 — *"Some Years After"* — a fourteenth zone: the epilogue that pays off the whole saga (content)

> Every great CRPG saves one thing for last: the *after.* Through a door out of the Court lies **"Some Years
> After"** — where a chronicler tells how the world turned out (varying *completely* by which of the four endings
> you chose) and the souls who survived appear to tell you, themselves, what your small mercies *became.*

- ✒ **A fourteenth zone — the epilogue.** Reached from the Court of the Dead, completing a **fourteen-zone
  connected map**. Built from the existing prop vocabulary (no new art).
- 📖 **the Chronicler — the "where are they now," framed by your ending.** Their opening tells how the *whole
  world* turned out: the **re-made Wall** of willing keepers, the **generational appeal** still clearing its
  docket, the **kind god** you relieved walking through his own door smiling — or, if you took the Crown, the
  **sorrowful tally** of the tyrant you became. Then they recount the fates of every thread — the river, the
  bridge, the Choir, your company, and Aldric and Maerin — each coda reading the specific flags you set across
  all thirteen prior zones.
- 🍎 **The survivors appear to speak for themselves** (via the conditional-NPC system): **Wren**, old now and
  warm and *living*, who became the one who *bothers* for the next marked soul and whose mark is **thawing** —
  un-made, slowly, by the soul who never stopped; and **Pip**, grown, who took the one shared watch you stood for
  her and built a **roof over eleven lost children**, teaching them that safety is a thing you build between
  people, in turns. (If you cured Wick, he weaves true-named burial banners now, *allowed to cry.*)
- 🎭 Full deep stack: reactive greetings, the **Returned-sense** on each, a Returned line apiece, dispositions —
  turned, at the last, to the warmest uses they've had.
- 🧪 **+6 gates** (now **950 checks**, all green): the epilogue is keyed/reachable/collision-clean, the Chronicler
  frames the after by your ending (four distinct openings), the survivor-codas appear only for souls you actually
  saved, and Wren's thread (the thawing mark) pays off. **The walkable saga is complete: 14 zones, 48 souls, from
  a cold market stall to the throne of the dead to the quiet years after.**

---

## 👑 v4.90.0 — *"The Court of the Dead"* — a thirteenth zone: the finale, and the choice that ends the saga (content)

> Through the death-door at the Threshold lies the place the whole road was always walking toward — the **Court
> of the Dead**, where the Crown of Horns turns at the foot of a throne held by **Kelemvor himself**, the Judge,
> bound to a Wall he hates and waiting an age for the one soul who could end it without becoming it. The capstone
> of the entire walkable saga.

- ⚖️ **A thirteenth zone — the Court of the Dead.** Reached *through* the death-door at the Threshold,
  completing a **thirteen-zone connected map** (test-verified as fully reachable). New code-drawn prop: the
  antlered **throne.**
- 👑 **Kelemvor, the Judge of the Dead — the finale.** Not the corpse-king of the songs: a tired man grown vast,
  who judges fairly but is *bound* by the Concord to wall the Faithless *unjudged*, and *forbidden to witness*
  the souls who most need it. His opening reads which road you walked here on (loop-broken / appeal-bringer /
  Crown-seeker / stranger), and the crux offers **four divergent endings to the saga**: **rebuild the Wall out
  of the witnessed** (the seawall stays, but of volunteers, named and proud); **force the souls' lawful appeal**
  (empty it by precedent, one just verdict at a time, forever); **relieve the tired god** (take the throne the
  honest way, handed and checked, and let Kelemvor rest); or **take the Crown** and become the next tyrant — the
  one tragic ending, where the grey cold starts at the boots.
- ♛ **The Crown of Horns itself — a temptation that speaks in your own voice.** A "the One Ring"-style object
  that argues, reasonably, warmly, for being worn — and can be beaten not by humility (which it loves and turns)
  but by **naming your own weakness and putting a guard on it.** Refuse it, and even the Crown is, strangely,
  glad to have met the soul who could.
- 👧 **Esuele, the first name on the appeal** — present (via the conditional-NPC system) if you pledged the appeal
  at Aldric's table, brought up out of the Wall to stand whole before the Judge's own seat as living proof that
  *memory beats the Wall.*
- 🎭 Full deep stack: reactive greetings, the **Returned-sense** on each (Kelemvor at DC 18, the Crown at DC 20 —
  it looks *back*), a Returned line apiece, dispositions.
- 🧪 **+10 gates** (now **944 checks**, all green): the finale is keyed/reachable/collision-clean, the
  thirteen-zone map is fully connected, the Judge's four road-openings are distinct, the Crown's self-knowledge
  refusal is wired, the appeal-ending gates on having pledged it, and the four endings resolve (three resolve the
  saga; taking the Crown makes you the tyrant). **13 zones, 45 souls** — a whole saga, walkable, from a cold
  market to the throne of the dead.

---

## 👑 v4.89.0 — *"The Vault of Tens"* — a twelfth zone: a riddle-puzzle where wit beats correctness (content)

> The last core interaction the game wants is a **puzzle** — and the bible's "Vault of Tens" gets built: a
> riddle-chamber guarded by a bored bronze head, where the trap is *being right*, and the real answer is *seeing
> the person in the cage.*

- 🔟 **A twelfth zone — the Vault of Tens.** Reached behind the Underbridge's foundations, completing a
  **twelve-zone connected map** (test-proven, and the test now verifies the whole graph is reachable). Built
  entirely from the existing prop vocabulary — no new art.
- 🧩 **A puzzle interaction where wit beats correctness.** The **Warden of Tens** poses the oldest riddle in the
  world — and the *correct* answer ("Time") earns a flat *"correct"* and the **stone creeping up your boots**,
  because the Warden has been bored to bronze by ten thousand reciters. A **clever-wrong** answer (answer the
  *room*, not the book), a **foolish-true** one (insult it to its face), or **refusing the game** to talk to the
  lonely soul *inside* the lock — each makes the bronze head *laugh* and wins. Knowledge is the trap; *surprise*
  is the key.
- 🪶 **Two souls who are the puzzle's real meaning:** the **Petrified Pedant** (turned to rock for being
  *flawlessly, tediously right*, now begging you to be the *delightful fool* he couldn't), and the **Tenth** —
  *Sela*, a lamp-lighter taken as the living stake at the centre of the binding, slowly un-personed by centuries
  of clever strangers who came to win the *door* and walked past her *cage.* Learn her name, and you can answer
  the Warden's riddle with the only answer that ends the game: *the thing with a hundred names that the rich and
  poor both spend and can't keep is a* person, *treated as a prize.* Free her — and, if you ask, every "wrong
  answer" mortared in the walls.
- 🎭 Full deep stack: reactive greetings, the **Returned-sense** on each soul, a Returned line apiece,
  dispositions; and an accumulating **riddles-bested** count surfaced in the ledger.
- 🧪 **+5 gates** (now **934 checks**, all green): the twelfth zone is keyed/reachable/collision-clean, the
  twelve-zone map is *fully* connected (every zone reachable from the market), wit-beats-correctness is verified
  (clever-wrong wins, correct petrifies), naming Sela gates on learning her name, and freeing the prisoner — not
  the hoard — is the real solve. The walkable Act now spans **12 zones and 42 souls.**

---

## 👑 v4.88.0 — *"The Night Market"* — an eleventh zone: breadth, weirdness, and a barter/shop interaction (content)

> The narrative spine is complete, so this one adds *breadth* — a surreal palette-cleanser off the hub, and the
> last interaction type the game wants: a **shop.** A bazaar only the dead-touched can find, where nobody deals
> in coin, because the dead have none — they trade in **years, memories, and names.**

- 🌙 **An eleventh zone — the Night Market.** Reached by a stair from the market "that wasn't there a moment
  ago," completing an **eleven-zone connected map** (test-proven). Built entirely from the existing prop
  vocabulary (stalls, braziers, hanging cloth) — no new art needed.
- 🛒 **A real barter/shop loop, on the existing engine.** No new UI: the trade rides the `when.int` / `AddInt`
  system already proven by dispositions. The **Pawn of Hours** buys *years off the grey end of your life* — each
  trade accumulates `nm.years_given` (surfaced in the ledger), and the **back-of-stall merchandise unlocks only
  once you've spent enough** (a genuine shop gate). Spend a year, get a borrowed hour; spend three, get a
  measure of un-time the Wall's cold can't eat.
- ❀ **Three vendors who are all cautionary tales that kept their stalls:** the **Pawn of Hours** (an appetite
  building itself a life a stolen decade at a time — and the Wall's quiet second supplier); the
  **Lost-and-Found** (a keeper of dropped memories who traded away her own life piece by piece and built a shop
  around the last warm jar); and the **Regular** (a soul that sold its grief to stop hurting, learned too late
  the ache *was* the love, and now drifts the aisles having sold even the memory of the door).
- 🕯️ **The saga's heart, refracted:** the Night Market is the **Hunger** in microcosm — souls spent to nothing a
  willing trade at a time — and a **witness** can still carry the Regular *out* by remembering it, exactly the
  cure the Wayshrine named.
- 🎭 Full deep stack: reactive greetings, the **Returned-sense** on each vendor, a Returned line apiece,
  dispositions.
- 🧪 **+12 gates** (now **929 checks**, all green): the eleventh zone is keyed/reachable/collision-clean, the
  eleven-zone map is one connected graph, the trade resource accumulates and gates the back-stall goods, the
  resource is surfaced in the ledger, and the witness-carries-a-soul-out beat is wired. The whole walkable Act now
  spans **11 zones and 39 souls.**

---

## 👑 v4.87.0 — *"The Threshold of the Crown's Road"* — a tenth zone: you meet yourself, and the saga's loop is revealed (content)

> Where the road ends, the living cannot follow — and waiting at the dark door is the **Last Returned**: *you*,
> from a future that already took the Crown and spent two hundred years freezing into the very thing it broke the
> world to destroy, crawled back through the crack in time to make you answer the one question it didn't. The
> capstone of the whole Act, and the saga's deepest twist.

- ∞ **A tenth zone — the Threshold of the Crown's Road.** Reached past the wayshrine, completing a **ten-zone
  connected map** (test-proven). New code-drawn prop: the **death-door** — a dark archway with cold light rising
  through it, the threshold only a Returned can walk.
- ∞ **the Last Returned — the mirror, your worst ending given a voice.** Its opening reads the *exact* road you're
  on (witness / reckless / Crown-taker / accompanied / alone) and answers it as the soul who walked it. It reveals
  the saga's central twist — **the masks are the same soul, looping**: a braid of Returned, each one *certain* it
  learned enough to break the cycle, each one becoming the Wall by a slightly kinder road. And it names the *only*
  exit: a soul that walks the door **accompanied**, **names who may uncrown it beforehand**, and **hands the Crown
  away immediately** rather than keeping it for one more *reasonable* mercy. Break the loop and you lay your future
  self, at last, to rest — witnessed, unfrozen, free.
- 🛡️ **The companions who followed appear at the edge** — Dace (if recruited) and Orin (if she joined), via the
  conditional-NPC system, now doing party duty: they are *the variable your future self never had*, and they'll
  amend a campfire promise into a vow to both *save* you and *stop* you.
- 🎭 Full deep stack throughout: reactive greetings (five distinct mirror-openings), the **Returned-sense** on each
  soul (the Last Returned at sense-DC 2 — it is *literally you*), a Returned line apiece, dispositions.
- 🧪 **+12 gates** (now **917 checks**, all green): the tenth zone is keyed/reachable/collision-clean, the ten-zone
  map is one connected graph, the mirror's five road-openings are distinct, the loop is revealed, the
  name-an-uncrowner and hand-the-Crown-away answers gate on your anchor and on knowing the loop, and breaking it
  lays your future self to rest.

---

## 👑 v4.86.0 — *"The Grey Wayshrine"* — a ninth zone: the Judge's own voice, and the secret under the whole saga (content)

> You've heard the Choir (condemn the Faithless) and Aldric (tear it all down). On the road past camp stands a
> grey wayshrine — and the **Judge of the Dead himself** sends an aspect to weigh the one soul his ledger can't
> hold. Here the saga finally says its deepest, quietest truth out loud: *why the Wall exists at all.*

- ◈ **A ninth zone — the Grey Wayshrine.** Reached up the road from the Hearth, completing a **nine-zone
  connected map** (test-proven). New code-drawn props: the **grey balance-shrine** (its pans forever seeking
  level), road **milestones**, grave **cairns**.
- ◈ **the Grey Harbinger — an aspect of Kelemvor, the theological keystone.** Not a condemner like the Choir,
  not a destroyer like Aldric — a *weigher*, sent because you are too unusual for the ordinary judging. Its
  opening reads which campaign-road you're on (take the Crown / file the appeal / godless / stranger), and it
  reveals the **deepest secret of the setting**: the Wall is not a prison but a **seawall**; the Faithless are
  its **mortar**; behind it presses the **Hunger** — the curdled nothing of every unwitnessed soul — penned by a
  Dawn-age **Concord** that the Judge *inherited and hates but is bound to keep,* because the day it fails, the
  nothing pours in for *everyone.* The crux poses the question under the entire saga — *what will you do about the
  Hunger when you open the Wall?* — with four real answers (build a **gate**, become a **witness**, stay
  **honest** about not knowing, or recklessly let it **flood**) that get you blessed, allied, or warned that the
  aspect will *unmake* you at the Wall if you arrive a flood.
- ⚔️ **Justiciar Orin — the order's own sword, questioning.** A true believer who has read her scripture too
  closely and suspects the order is rotten at the root; she can be talked from *arresting* you to *joining* you
  to find out if the order deserves guarding (with nat-20/nat-1 on the standoff).
- 🌾 **Goodman Doget — a fair death.** An old farmer who died gentle at eighty-one with a square ledger and a
  wife waiting; the system working *as intended* for the lucky — and, once you show him his godless neighbour got
  no shrine at all, a soul who chooses to carry an unfair death's name across with him.
- 🎭 Full deep stack throughout: reactive greetings, the **Returned-sense** on each soul, a Returned line apiece,
  dispositions, crit/fumble.
- 🧪 **+12 gates** (now **905 checks**, all green): the ninth zone is keyed/reachable/collision-clean, the
  nine-zone map is one connected graph, the Harbinger's four road-openings are distinct, the Concord and the
  Unmade are revealed, the witness-answer gates on the Judge's question, a wise answer allies the aspect and a
  reckless one earns the warning, and the Justiciar can be turned.

---

## 👑 v4.85.0 — *"The Doomguide's Table"* — an eighth zone: tea with the man at the heart of the saga (content)

> The Act-1 hub now has a *door out* — and it leads to the keystone of the whole campaign. Crack the local
> Choir and a herald brings a summons: **tea with Aldric Morn**, the broken Doomguide of Kelemvor who watched the
> Wall take his godless daughter and now means to depose a god and tear it down, *no matter who he must burn.*
> This is where the cold little Act-1 world hands off to the main story — and it lands its hardest gut-punch.

- 🫖 **An eighth zone — the Doomguide's Table.** Reached by a summons-exit from the Lamplit Quarter, completing an
  **eight-zone connected map** (test-proven). New code-drawn props: the steaming **tea table** (a cup poured for
  an empty chair), **chairs**, a **hearthfire**, **bookshelves**, a cold **window**.
- 🕯️ **Aldric Morn — the keystone confrontation.** Not a villain: a good man ruined into monstrous arithmetic,
  morally the equal of the Canon and Doomguide Vayle. His opening and every argument read your **entire Act** —
  ally of the cracked Canon, friend of the Choir, servant of the Judge, or stranger. Name his daughter instead of
  his plan; out-theologize his apotheosis until *he* names the god-shaped trap he's walking into; or **show him
  the gentle road** you cracked open at the counting-house and watch twenty years of certainty break. He reveals
  the **Crown of Horns** and asks the one thing only a Returned can give: to walk into death, and come back.
- 👧 **Maerin — the daughter in the empty chair, seen by you alone.** The new conditional-NPC + Returned-sense
  systems pay off here in the saga's cruelest, tenderest beat: Aldric's mortared daughter *followed him home from
  the Wall* on a thread of his grief, and has sat beside him for twenty years, unseen, while he pours her tea.
  Only your dead-touched eyes can find her. Her one wish is that her father *stop* — and if you come to suspect
  the thread, you can carry her voice to him, and give a grieving man the reunion (and the release) he never
  knew was an arm's length away.
- 🛡️ **Wessa, the herald** — the young soldier who half-worships and half-fears the great man she serves, and is
  terrified of the day he asks her to do the unforgivable and she says yes.
- 🎭 The crux offers real, divergent bridges into the campaign: ally him for the lawful **appeal**, take the
  **Crown** yourself (with his warning about who must be willing to *uncrown* you), reunite him with **Maerin**,
  or **refuse** him outright — and even refused, he leaves the door open for the gentle road.
- 🧪 **+14 gates** (now **893 checks**, all green): the eighth zone is keyed/reachable/collision-clean, the
  eight-zone map is one connected graph, Aldric's four cross-zone openings are distinct, the gentle-road and
  daughter-reunion options gate on the deeds that earn them, a resolution puts down the matches, the unseen
  daughter has the lowest sense-DC of any soul, and the Crown of Horns is revealed as the bridge to the main quest.

---

## 👑 v4.84.0 — *"The Hearth"* — a seventh zone, a denouement, and souls that appear on what you did (content + an engine feature)

> Every Act needs its quiet after. **Make camp at the Hearth** — a fire in the dark past the city — and take
> stock. And here the demo gains the engine feature the whole game is built on: **conditionally-present NPCs.**
> The souls who gather at your fire are *exactly the ones you saved.* Save no one, and the fire burns alone.

- 🔥 **A seventh zone — the Hearth.** A campfire denouement reached by a new "make camp" exit from the market,
  completing a **seven-zone connected map** (test-proven). New code-drawn props: a living **campfire**, seating
  **logs**, **bedrolls**, **trees**.
- 🌟 **New engine feature: NPCs that appear on world-state.** A scene NPC may carry a `when` (the same matcher the
  dialogue uses), and only spawns — rendered, blockable, talkable — when the world matches. `npcVisible` /
  `activeNpcs` / `blockedNow` drive rendering, collision, and pathing off the *present* souls, recomputed when a
  conversation changes state. This is how the game will populate every reactive scene.
- 🫂 **The souls at the fire are the ones you saved:**
  - **the fire itself** (always present) — a reflective *take-stock*, its denouement reading your dominant
    disposition (haunted / merciful / ruthless / weary), with threads to reckon up the river, the Wall, the Choir,
    and yourself.
  - **Dace Iron** (if recruited) — your companion off-watch: an **approval-gated night-talk** (the camp/bonding
    interaction) where she finally puts the sword down and tells you about the sister the Wall took.
  - **Wren** (if she lives) — alive at your fire, eating a stolen apple, working out what to *do* with an "after"
    she never expected — and deciding to become the one who *bothers*, for the next Wren.
  - **Pip** (if you helped her) — warm for once, and if you cured **Wick**, sleeping with both eyes for the first
    time in years because someone else is keeping the watch.
- 🎭 Full deep stack throughout: reactive greetings, the **Returned-sense** on each soul (now turned *inward* at
  the fire), a Returned line apiece, dispositions.
- 🧪 **+13 gates** (now **879 checks**, all green): the seventh zone is keyed/reachable/collision-clean, the
  seven-zone map is one connected graph, the conditional-NPC feature is wired and verified through the real
  matcher (Dace hidden until recruited, then present), the fire's denouement reads your disposition, and the
  companion night-talk gates on approval.

---

## 👑 v4.83.0 — *"The Counting-House"* — a sixth zone, and the Act-1 climax: a confrontation that reads the whole web (content + a new interaction type)

> Deeper into the city than the Lamplit Quarter sits the **Counting-House** — the Choir's local seat, where
> Tallow's watching and Pell's lists and Vharn's measures all flow *up*, to one desk and one great brass scale.
> This is the **Act-1 climax**, and it introduces the last interaction type the hub needed: the *boss
> confrontation* — a high-stakes encounter whose every option is gated on what you did across all six zones.

- 🏛️ **A sixth zone — the Counting-House.** Reached deeper in from the Lamplit Quarter, completing a six-zone
  connected map (test-proven). New code-drawn props: the **great brass scale** of the Choir (its pans tilting),
  clerks' **desks**, towering **ledger-shelves**, a barred **holding-cell**, candlelight.
- ⚖️ **Canon Mereth Vael, High Measurer of the Causeway — the boss.** Not a cartoon: a genuine believer who
  thinks the Wall is a *necessary* evil holding back a flood of unclaimed dead. Her opening and her every argument
  read the whole saga — **expose Chancellor Venn** (gated on learning his name under the bridge), **prove the Wall
  flinched** (gated on stirring it at the torch), **name her line-items back to her** (Wren, Crake, Wick,
  Esuele), **bring a sworn sword** (gated on recruiting Dace), or **out-audit her own ledger** until its
  arithmetic betrays it (a nat-20 that turns her). The crux then offers **real, divergent resolutions** — force
  the souls' lawful **appeal**, cut **Venn's signature** with the Knotwife's pattern (gated on carrying it), walk
  it **unfinished together**, or take the cold **chaos** path — most of which ally the Canon and crack the Wall.
- 🙂 **Goodman Tallow, at his true desk.** The smiling market-watcher in his real setting. His welcome remembers
  whether you sold Wren to him or saved her; unmask the hollow man, break him (crit: you threaten to make him
  *forgotten* again and he shatters), or — if you're his kind — take his offer of a place in the work.
- ⚰️ **Old Crake, marked, awaiting the causeway.** A gravedigger of sixty years, utterly at peace, darkly funny,
  the one soul here unafraid to die — only afraid he buried *coats*, not people. Free him from his cell (gated
  Athletics, crit/fumble), or, as a Returned, give him the one truth his spade never could.
- 🎭 Full deep stack throughout: reactive identity/flag/disposition greetings, **nat-20/nat-1 crit & fumble
  comedy**, the **Returned-sense** on each soul, a Returned line apiece.
- 🧪 **+13 gates** (now **866 checks**, all green): the sixth zone is keyed/reachable/collision-clean, the six-zone
  map is one connected graph, the boss's four cross-zone openings are distinct, the Venn/Wall/sword arguments and
  the unpick resolution each gate on the deed that earns them, and a resolution provably allies the Canon.

---

## 👑 v4.82.0 — *"The Lamplit Quarter"* — a fifth zone, your first companion, and a world that gossips (content + two new interaction types)

> Climb *up* out of the market, away from the causeway, into the living city — the **Lamplit Quarter**, where
> the Sword Coast still drinks and schemes and the Choir keeps its quiet books. This zone introduces **two whole
> interaction types the full game needs:** *recruiting a companion* (with an approval score), and *a world that
> remembers everything you've done.*

- 🏙️ **A fifth zone — the Lamplit Quarter.** The market now opens *up* into the Lower City as well as down to
  the causeway, so it's a true five-zone hub (and the test proves all five form one connected graph). New
  code-drawn props: glowing **lampposts**, the **Sotted Saint** tavern with lit windows and a sign, tables, a
  notice-board.
- 🗡️ **Dace Iron — recruit your first companion.** A Faithless blade-for-hire who's turned down a month of Choir
  coin and is waiting for the one fight worth her steel. Pitch her by Insight, Persuasion, or kinship — and on
  success she **joins** (`party.dace_recruited`), with her own **approval score** (a brand-new reactive axis) and
  nat-20/nat-1 outcomes (crit: she slams her sword into the table as a vow; fumble: finger-guns, and she throws
  you out). Recruit her and she shows up as backup in a *later* scene.
- 🍺 **Mab, keep of the Sotted Saint — a world that remembers.** Her welcome reads your deeds across *all five
  zones*: shook the Wall and she greets you as the legend the Quarter's whispering about; saved Wren and you're
  the apple-girl's saviour; befriended the Choir and her grin goes cold and she warns you to keep that quiet.
  Trade her the truth of the Wall and choose whether to spread *hope* through the Quarter or sit on it.
- 📒 **Pell, the Choir's ledger-clerk — the quiet middle of the machine.** The forgettable man who *writes the
  marked down.* Unmask him (Insight), break him (Intimidation), con him colleague-to-colleague (Deception), or
  bring **Dace** as muscle — and strike names off the list. His greeting flips if you're already a Choir-friend.
  A Returned breaks him simply by existing: there is *no column* in his book for a soul that cancelled its own
  death.
- 🎭 Full deep stack throughout: reactive identity/flag/disposition greetings, **nat-20/nat-1 crit & fumble
  comedy**, the **Returned-sense** on each soul, a Returned line apiece.
- 🧪 **+13 gates** (now **853 checks**, all green): the fifth zone is keyed/reachable/collision-clean, the five-zone
  map is one connected graph, companion recruitment sets a party flag + tracks approval, the recruited companion
  becomes a usable option later, and Mab's & Pell's greetings provably read the cross-zone flags.

---

## 👑 v4.81.0 — *"Past the Last Torch"* — the fourth zone, the Wall of the Faithless, and three threads paid off at once (content)

> The causeway has an *end.* Climb the Reed-Walk past Sister Vharn, up through the mist, past the **last
> burning torch on the whole crossing** — and you're at the foot of the **Wall of the Faithless** itself,
> ten thousand faces pressed half-out of the grey stone. This is the zone the other three were pointing at,
> and it cashes in their seeded threads.

- 🧱 **A fourth zone — Past the Last Torch.** Reached by a new causeway exit at the *top* of the Reed-Walk
  (slip past the Measurer). The map is now a connected four-zone hub: market ⇄ reed-walk ⇄ **the Wall**, with
  the underbridge stair off the market. New code-drawn props: the **Wall** (with faces in the stone), a lone
  guttering **torch**, glowing **greywort**, a dead marsh-tree.
- 🕯️ **Brother Hale, Warden of the Last Torch** — a Doomguide freezing into his post a degree at a time (the
  market's Returned-sense warned of exactly this in Brother Calix). He guards the way, hates it, and is the
  guide to the cure: **fetch the greywort** from the Wall's foot (Survival check, with bespoke nat-20/nat-1
  outcomes — and a Returned can pick it cold-handed while the Wall *gets out of the way*).
- 🧩 **PAYOFF — Wick's cure.** The greywort errand is gated on having learned of the root under the bridge, and
  fetching it writes `under.wick_cure` back into the shared state. The dying child three zones away can live
  because of what you do at the Wall.
- 🪦 **Esuele, a voice in the Wall** — **the Knotwife's lost daughter**, walked out past this torch a lifetime
  ago, now a face in the stone worn down to a thimbleful of self. If you heard the blind weaver's grief under
  the bridge, you can tell Esuele her mother **never stopped weaving her name** — and watch four centuries lift.
  She also lays out the heresy from the inside: the Wall holds no sinners, only the poor and the unclaimed.
- 🚢 **Goodwin, waiting for the boat** — a cheerful dead cloth-merchant who doesn't know he's died, sat on a rock
  waiting for a ferry the Choir promised and never sent. Comic-morose relief: tell him the truth and set him
  walking toward the light — or send him to gently un-wait the whole marsh of souls on their rocks.
- 🎭 Full deep stack throughout: reactive identity/flag/disposition greetings, **nat-20/nat-1 crit & fumble
  comedy**, the **Returned-sense** on each soul, a Returned line apiece — and the marquee moment of the whole
  preview: **the Wall itself stirs** when a Returned addresses it.
- 🧪 **+15 gates** (now **840 checks**, all green): the fourth zone is keyed, the four-zone map is provably one
  connected graph, the Wall zone is collision-clean, and the three cross-zone payoffs are verified — the greywort
  gate, the cure write-back, and Esuele's recognition of her mother.

---

## 👑 v4.80.0 — *"The Underbridge"* — a third walkable zone, and a thread that ties the saga together (content)

> The triptych is complete. From the market you can now take the **stair down** into the dark under the
> great causeway-span — the **Underbridge**, where the souls the bright places can't use shelter in the cold.
> It pays off the *other* flame your Returned-sense felt in the market, and it knots the river's deepest
> secret back into the world.

- 🌉 **A third zone — the Underbridge.** New code-drawn props (bridge-pillars, a guttering brazier, a loom, a
  pallet, hanging banners), reached by a second market exit. Three zones now share one visit-state; the market
  is the hub, with the river one way and the stair the other.
- 🕯️ **Wick** — Pip's dying little brother, the second guttering flame you sense from the market. A nine-year-old
  who has already, quietly, made his peace — and is afraid of exactly one thing: what happens to **Pip** after.
  You can read him with a healer's eye (Medicine), tell him the Judge's gentle truth (Religion/Kelemvor), give
  him a fight to be *in* rather than be grieved at (Persuasion crit: he blazes up *"Wren'd be proud — it's the
  family trade now"*; fumble: you accidentally declare war on his cough), or, as a Returned, tell him true what's
  across — he's the one soul who'd rather *know.*
- 🧶 **the Knotwife** — a blind weaver of bridge-banners who shelters the orphans and weaves burial-banners for the
  souls the Choir won't bury. **She's the Reed-Wife's old guild-sister.** If you learned at the river that *the
  knot has a name*, she startles — and gives you the four-centuries-buried truth: the crossing was built not by a
  god but by a **living clerk, Chancellor Aldous Venn**, who signed his name into the third bridge-pier to make
  grief *efficient* — and she's kept the unpicking-pattern ready her whole blind life for the one soul who could
  cut it.
- 🧒 **Pip, again** — keeping a six-day watch over her brother, and she greets you on what you did upstairs: warm
  if you helped her in the market, feral if you scared her with the Watch.
- 🔗 **The cross-zone web tightens:** `market.helped_pip` / `market.scared_pip` gate Pip's welcome; the Knotwife's
  Chancellor-Venn revelation is gated on `reed.knot_has_a_name` learned from the drowned weaver downriver — the
  river and the bridge are now one story.
- 🎭 Full deep stack carries over: reactive identity/flag/disposition greetings, **nat-20/nat-1 crit & fumble
  comedy**, the **Returned-sense** on each soul, a Returned dialogue line apiece, and a greywort quest-thread to
  fetch the cure from past the last torch.
- 🧪 **+12 gates** (now **825 checks**, all green): the third zone is keyed, reachable, collision-clean; the market
  has its two exits; and the marquee cross-zone payoffs are proven — Pip's welcome flips on the market flags, and
  the Knotwife's buried history unlocks only once you've heard the phrase at the river.

---

## 👑 v4.79.0 — *"The Reed-Walk"* — a second walkable zone, and a river that remembers (content + system)

> The market was an island. Now it has a **shore.** Walk to the bottom of the square, step onto a glowing
> **causeway tile**, and the world dissolves and reforms around you on the **Reed-Walk** — the cold riverside
> below the market, where the road to the Wall of the Faithless begins. The two zones share one visit-state, so
> **what you did in the market follows you down to the water.**

- 🗺️ **A second zone — the Reed-Walk.** A whole new isometric scene in the same file: river shallows, a moored
  ferry, a wayshrine, reed-beds. New props are drawn in code (boat, reeds, pilings, shrine), so it stays a single
  self-contained HTML with no external art.
- 🚪 **Zone travel.** Glowing causeway tiles you can walk onto (or click) to cross between scenes, with a fade
  transition and a "now in" zone banner. `loadScene` swaps the board and rebuilds collision; the shared `st`
  carries every flag, regard, and disposition across the crossing.
- 🌊 **Four new souls, each deep and reactive:**
  - **Old Cassian**, the boatman of the last mile — ferries the bought-and-blessed and makes the godless *walk*;
    offer to *come back and tell him what's across*, and forty years of stoicism break (nat-20: he weeps and
    offers you the river; nat-1: you fall in the shallows with *flair*).
  - **Sister Vharn**, a Measurer of the Choir — the soul-weigher who marks the Faithless. **Her welcome flips on
    the market:** betray Wren to Tallow and Vharn greets you as a friend by name; protect Wren and she's hunting
    you. Lie to cover Wren (Deception, gated on having saved her), out-argue her doctrine (Religion), or read the
    crack in her (Insight → 311 faces she won't count).
  - **the Reed-Wife** — a weaver drowned four centuries ago who refused the Wall and let the river keep her;
    comedic-creepy lore-giver who'll trade the crossing's deepest secret (Arcana crit: *the knot has a name, and
    it's a bureaucrat's*) for someone who won't walk *faster.*
  - **Wren, again**, at the water's edge — the emotional centre. She opens with **relief if you saved her**,
    **heartbreak if you sold her** (*"you Judas with a kind face"*), wariness if you only met her. Talk her back
    from the causeway (Persuasion / Insight / Religion), hand her **Sable's holding-stone** if you carried it out
    of the market, or honor her choice and stand so she doesn't walk it unseen.
- 🎭 The full deep stack travels with the new souls: reactive identity/flag/disposition greetings, **nat-20/nat-1
  crit comedy**, passive lore, the **Returned-sense** on each, and a Returned dialogue line apiece.
- 🧪 **+15 new gates** (now **813 checks**, all green): both zones keyed & reachable, exits walkable and landing
  in-bounds, the Reed-Walk's NPCs collision-clean, and the **cross-zone payoff** proven — Vharn's and river-Wren's
  greetings provably differ on `protected_wren` vs `betrayed_wren`, and the Deception/holding-stone options gate
  on the market flags.

---

## 👑 v4.78.0 — *"Mind Where You Walk"* — real collision & A* pathfinding in the market (fix + system)

> Fair feedback, fixed: the market's movement was a naive straight-line slide with **no collision** — you could
> glide right through people, stalls, and the fountain. Now it's a proper grid mover with obstacles and
> path-finding, the way an isometric CRPG should be.

- 🚧 **Collision** — every prop *and* every NPC now blocks its tile. You can't stand inside a person or a stall
  anymore; blocked tiles read darker on the floor, and the cursor shows *not-allowed* over them.
- 🧭 **A\* pathfinding** — click anywhere and the player routes a real 8-directional path *around* obstacles (no
  corner-cutting through diagonal blockers) instead of sliding through them. Click a blocked tile and it walks
  to the nearest free tile beside it; click an NPC and it walks to a free tile *beside* them, then talks.
- ✨ **Path preview** — faint gold waypoint dots trace the route, brighter at the destination, BG3-style.
- 🧪 New gates prove it: every prop/NPC blocks its tile, **you can no longer path onto an NPC's tile** (the exact
  bug reported), no path ever crosses a blocked tile, and all seven souls remain reachable via a free adjacent
  tile. Suite now **798 checks**, all green.
- 📝 *Honest note:* the dialogue dice are a stylized **web stand-in** (procedural SVG) — the real 3D die, and a
  lot more polish, belong in the Unity build. These browser previews exist to prove the *logic and content*,
  byte-for-byte; they're the blueprint, not the final paint.

---

## 👑 v4.77.0 — *"The Die is Cast"* — an actual rolling d20 animation (system)

> The flicker-die is gone. In its place: a **real, faceted d20** that tumbles in 3D, bounces when it lands,
> flares **gold** on a natural 20, and shudders **red** on a natural 1 — and it's drawn entirely in code, so it
> stays a single self-contained file with zero external art.

- 🎲 **A procedural d20** (a five-facet SVG icosahedron, violet-and-silver) that:
  - **tumbles** — a looping 3D `rotateX/Y/Z` spin while the number flickers;
  - **lands** with a satisfying spring **bounce** (overshoot → settle), the number locking on the final face;
  - **flares gold** with a radial burst-ring on a `✦ NAT 20!`, and **dims red with a shudder** on a `💀 NAT 1!`;
  - keeps the **fanfare / sad-trombone** SFX from the crit system.
- 🧩 Wired into **both** the market and the campaign dialogue sim, downstream of the pre-roll anticipation panel
  — so the full flow is now: *see the breakdown → press ROLL → watch the d20 tumble & bounce → it flares on a
  crit → the bespoke comedy fires.* Pure CC0 (procedural SVG + CSS keyframes), no external art.
- 🧪 New gate confirms the d20 geometry and the tumble/land/crit animations are wired. Suite now **793 checks**,
  all green.

---

## 👑 v4.76.0 — *"See the Roll Before You Make It"* — the BG3 pre-roll anticipation panel (system)

> Like Baldur's Gate 3, an active check no longer just *happens.* Pick it, and a panel lays the whole thing
> bare — your modifier broken down, the exact number you need on the die, your odds, the crit/fumble stakes —
> and then *you* press **ROLL THE DICE.** The anticipation is the best part.

- 🎲 **Pre-roll preview panel**, in both the market and the campaign dialogue sim. When you choose an active
  check it shows:
  - the **breakdown**: `d20  +3 CHA  +3 proficient  = +6` (ability modifier and proficiency as separate,
    legible chips — or "+0 not proficient");
  - **the number you need**: *"You need an 8 or higher on the die,"* automatically collapsing to *"only a
    natural 20"* on an impossible check or *"anything but a natural 1"* on a trivial one;
  - a **% chance bar**, green over 50, red under;
  - the **crit stakes**: ✦ nat 20 → critical success · 💀 nat 1 → critical failure, flagged when there's a
    *special* (comedic) outcome waiting;
  - and a big gold **🎲 ROLL THE DICE** button — the roll only fires when *you* commit.
- 🔢 The math is a pure, tested helper (`rollBreakdown`) so the panel can never lie: need = DC − bonus, odds =
  `chanceToPass`, with correct edge-cases at the extremes.
- 🧪 New gates pin the breakdown (mod + proficiency sum, the need-on-die, the chance, and the only-nat-20 /
  only-nat-1 flags) and the wiring (no instant auto-roll — the preview comes first). Suite now **792 checks**,
  all green.

---

## 👑 v4.75.0 — *"Gutter-WHOOP"* — natural 20s & natural 1s, BG3-style, with the comedy turned up (system + content)

> Roll a **natural 20** and something gloriously absurd happens; roll a **natural 1** and you eat shit in the
> funniest way the writer could imagine. The single best place in an RPG for gags, gaping amazement, and
> catastrophe — now wired into the dice everywhere, with bespoke comedy on the juiciest checks.

- 🎲 **The BG3 crit rule, faithfully** — a natural 20 **auto-succeeds against any DC**, a natural 1
  **auto-fails against any bonus.** The d20 lands gold (`✦ NAT 20!`) with a little triumphant fanfare, or
  blood-red (`💀 NAT 1!`) with a sad trombone. The "decide check results" preview now offers all four branches
  (crit / success / failure / fumble) so you can read every outcome.
- 🤣 **Bespoke crit & fumble scenes** on eight of the market's checks — and they are, I hope, *quite* fucking
  funny:
  - **Sable, nat 20:** you haggle so perfectly she has an existential crisis, shoves you the whole stall, and
    tries to *adopt you as her grandchild.* **Nat 1:** your charming wink reads as a *seizure* and she flings
    (possibly cursed) chicken at you.
  - **Bram, nat 20:** you're so terrifying a twenty-year soldier *salutes, wets himself slightly,* and tries to
    enlist under *you.* **Nat 1:** your big threat cracks into a *squeak* and he pats your head and gives you a
    biscuit.
  - **Pip, nat 1:** you slip on a cobble mid-loom and 'gutter-rat' comes out as **"gutter-WHOOP"**; the child
    laughs so hard she lets you keep your purse. **Nat 20:** she's so awed she demands to be your *apprentice.*
  - Plus **Tallow** (a lie so good he writes it down as *exemplary* / so over-egged he meets *"Kelemvor's
    cousin who sells winkles"*), **Joss** (a conversion experience / he decides *you're* a god's agent),
    **Calix** (he tears off his vestments, then gets cold / you accidentally *re-convert* him), **Wren** (she
    trusts you with four secrets too many / your sincerity is so intense she flees the *teeth*), and the
    dialogue sim's **Justiciar Vane** (gives you his own holy symbol / your plea curdles into a threat).
- 🧩 The crit engine lives in **both** the market and the campaign dialogue sim; every other check across all
  209 conversations now gets the nat-20/nat-1 rule and flair even without a custom node.
- 🧪 New gates pin the crit rule (auto-success/fail vs extreme DCs), that the bespoke nodes exist and differ
  from the normal branches, and that the comedy is actually in the text. Suite now **785 checks**, all green.

---

## 👑 v4.74.0 — *"Your Reputation Precedes You"* — dispositions the NPCs react to (system + content)

> The deepest Pillars-of-Eternity trick, now live: because your **reckoning persists across the whole visit**,
> the souls you meet *later* read who you've *become* and greet you accordingly. Reputation that arrives before
> you do — earned with one soul, spent on the next.

- 🪞 **Disposition-reactive greetings** across the four newest souls. Spend the morning being **merciful** and:
  - **Wren** meets you as *"the gentle one they've been whispering about… then maybe you're safe,"* the held
    breath of her whole morning finally let go;
  - **Brother Calix** steadies just to watch you cross the square — *"watching you is the first thing in a year
    that's made me think mercy might not be a heresy."*
  - Spend it being **ruthless**, and **Goodman Tallow** welcomes you as kindred — *"a soul with a practical
    turn… we should be friends"* — while Wren goes small and grey and prays you'll find someone else to be
    whatever you are at.
  - Carry the **haunted** cold and **Mad Joss** knows you on sight — *"you've got a whole winter in you… sit
    with the only other soul who's been listening as long as you have."* Be a **heretic** and he hails a fellow
    god-burner.
- 🥇 **Identity still outranks reckoning where it's iconic** — a Faithless or Myrkul-marked soul gets their own
  greeting regardless of disposition; the reckoning catches everyone else. (Variant priority: identity →
  disposition → default.)
- 🧪 New gates pin the reactive greetings (merciful → Wren's gratitude, ruthless → Tallow's welcome, haunted ≠
  heretical for Joss) and prove identity precedence holds. All seven trees still complete for all five builds.
  Suite now **776 checks**, all green.

---

## 👑 v4.73.0 — *"Fifty Ways to Answer a Madman"* — the complexity ceiling, demonstrated (content + system)

> The thesis you described — *the player sees four or five options, but there could be 30+ behind them, gated by
> story beats, prior choices, standing, charisma, race, religion, gender, class… ad infinitum* — built, proved,
> and made **visible.** Plus **gender** as a new reactive axis, and a little well-placed profanity for spice.

- 🗣️ **Mad Joss, the Fountain-Prophet** — a cracked, grief-mad oracle who rants that the Wall isn't justice but
  a *larder*: the gods are frightened, and every Faithless soul is one more *no* they'll never have to hear. His
  single central beat authors **50 distinct responses**, gated across *every* axis at once:
  - **race** (all nine), **gender** (Female/Male), **deity** (all seven), **class** (all five), **background**
    (all seven), **alignment** (lawful/chaotic, good/evil), **ability thresholds** (STR/INT/WIS/CHA ≥ 15),
    **passive lore** (Religion/Arcana), **active checks** (Persuasion/Intimidation/Deception), **dispositions**
    (Heretical/Haunted/Merciful), and **prior choices** (met Wren, hold the charm-seller's stone, Calix's
    doubt), plus the **[RETURNED]** path where he kneels because *you are the proof his prophecy was true.*
  - Each of the five shipped builds sees a *different* **12–15 of the 50**; the rest are invisible. A live
    **"✦ N of 50 authored responses fit who you are and what you've done"** readout makes the iceberg visible.
- ⚧ **Gender** is now a full reactive axis (Female / Male) across **both** the market and the dialogue sim —
  selectable in character creation, gate-able via `when.gender`, tagged in the choice UI.
- 🤬 **Spice, judiciously applied** — the souls who'd actually swear now do (Joss, a sacked soldier, a chaotic
  tongue, a tiefling preaching to the choir). Used where it lands, not as wallpaper.
- 🧪 New gates pin the 30+-option node, that gender genuinely changes the visible set, that no build is ever
  left with a dead node, and that prior choices widen the pool. Suite now **771 checks**, all green.

---

## 👑 v4.72.0 — *"Six Souls on the Causeway"* — the market doubles, and their fates interlock (content)

> You asked to flesh it all out with as much quality dialogue as we can — so the walkable Market grows from
> three souls to **six**, **35 → 71 dialogue beats**, and for the first time the NPCs' fates **interlock** the
> way they do in BG3 and Pillars: what you do to one changes how another meets you.

- ⛪ **Brother Calix, a doubting Doomguide** — a young priest of Kelemvor at a shrine-cart, breaking under the
  doctrine he's sworn to. Reactive to faith (kin / heretic / the Faithless he can't bear to condemn), with
  Religion & Insight passive checks, a Persuasion to talk him off the causeway, a **[HERETICAL]** line gated
  on your own disposition, and a **[RETURNED]** path where you show him he's slowly *freezing into the Wall he
  guards.* The Returned-sense reads the cold already creeping up his knees.
- 🍎 **Wren, the harvest-marked** — a godless woman buying winter apples with three days to live and no one to
  tell. The emotional core: the Returned-sense perceives the death-mark on her like *cold light* the instant
  you approach. Help her flee to Hessa's smugglers, tell her the gentle truth, or lean on your uncanny nature
  — Medicine / Insight / Perception checks, real stakes.
- 🎩 **Goodman Tallow, the Choir's watcher** — a smiling, forgettable man who hunts the godless for coin and
  feeds their names to the harvest. Insight to see what he is, Religion to indict him, Intimidation to drive
  him off, **Deception** to throw him off Wren's scent — or sell her out for a disposition you'll have to live
  with. A **[RETURNED]** path unnerves even him: *the Wall is reaching for you, too, Goodman.*
- 🔗 **Interlocking fates (the headline).** Tallow hunts Wren; the church Calix serves does the marking. Drive
  Tallow off (deceive / cow / out-doctrine him) and Wren greets you next as the saviour who *bought her a day*;
  hand him her name and she flees you in terror, knowing it was you. Cross-NPC flag-gated greetings, exactly
  the reactivity that makes a town feel alive.
- 🧪 New gates pin the three souls, the cross-NPC interplay (protect/betray written at Tallow → read in Wren's
  greeting), and the meet-Wren-first gate on the Deception cover; all six trees complete for all five builds.
  Suite now **764 checks**, all green.

---

## 👑 v4.71.0 — *"The Whole Causeway Knows"* — the deep system reaches all 209 conversations (tooling)

> Last release built the lore/sense/reckoning depth into the Market. This release **lifts it to the campaign
> dialogue sim** — so the hover-glossary and passive 5e lore now light up across the *entire* corpus — and
> adds a new top tier of knowledge while it's at it.

- 🔗 **Shared glossary** — the 22-term cyclopedia now lives in one file (`play/lore-glossary.json`), read by
  both the Market and the dialogue sim, so they can never drift.
- 📖 **Hover-glossary + passive lore, campaign-wide** — every keyed term is dotted-underlined and hover-able
  across all **209** conversations; **144 of them** already surface lore your character can passively recall.
  (725 glossary term occurrences across the corpus.)
- 🕳️ **Tiered lore (a new top tier)** — seven of the heaviest concepts (Kelemvor, the Wall, the Faithless,
  Myrkul, the harvest, the Court of the Dead, the causeway) now carry a **secret** beyond the common lore —
  a deeper, weirder, more dangerous truth (the Wall is made of the dissolving souls themselves; the Court can
  be *petitioned*; Myrkul built the first Wall and Kelemvor merely inherited it) that only a true expert
  (`10 + mod + proficiency ≥ a high DC`) recalls. Shown as a "…and, deeper:" continuation of the lore inset.
- ✦ **The Returned-sense on the campaign demo** — Justiciar Vane now gets his own dead-touched perception
  (you feel the Wall has begun to *look at him*), and **[RETURNED]**-tagged paths and a **disposition
  reckoning** (Merciful / Devout / Ruthless / Haunted) now appear in "The Threshold" too.
- 🧪 New gates pin the campaign-wide glossary reach, tiered-secret gating (only a high-INT proficient priest
  reaches a DC-17 Religion secret — not even the Confessor), the demo's Returned-sense, and its disposition
  accrual. Suite now **759 checks**, all green.

---

## 👑 v4.70.0 — *"What the Dead Know"* — lore-checks, the Returned-sense, and a reckoning of dispositions (tooling + content)

> A deep, idiosyncratic pass on the walkable Market, reaching for BG3 *and* Pillars-of-Eternity-level texture.
> Three new layers, all on-theme to a soul pulled back from the Wall:

- 📖 **Hover-glossary (a cyclopedia, PoE-style)** — 22 keyed terms (Kelemvor, the Wall of the Faithless, the
  Faithless, Doomguide, Myrkul, the Dead Three, Jergal, the Ashen Watch, the causeway, the harvest, the Choir,
  Kelemvor's-penny…) are dotted-underlined inline in every line of dialogue. Hover for the common-knowledge
  blurb of who/what is being talked about — no more nodding along to names you don't know.
- 👁️ **Passive LORE checks (5e knowledge skills)** — when a topic appears in a line, your *passive* knowledge
  (`10 + ability mod + proficiency ≥ DC` in Religion / History / Arcana / Nature / Medicine / Investigation)
  auto-recalls the deeper lore your character would actually know, surfaced as a "👁 Religion (passive) — you
  recall: …" inset. A cleric recalls the rites; a sage recalls the history; a soldier just hears a name.
- ✦ **The Returned-sense** — the signature, idiosyncratic mechanic: you are a soul pulled back from the Wall,
  and on first sight of each NPC — if your Wisdom-clarity (`10 + WIS mod`) is keen enough — you perceive what
  the living can't. Sable's relics are hollow but her grief is real; three squads of the violently dead stand
  at Bram's back, waiting; a second, guttering little flame is tethered to Pip. Plus a **[RETURNED]** dialogue
  path with each soul, where you lean into your uncanny nature — and they break, and tell you everything.
- ⚖️ **A reckoning of dispositions (Pillars-of-Eternity-style)** — your choices accrue dispositions
  (**Merciful, Ruthless, Honest, Cunning, Devout, Heretical, Stoic, Haunted**), shown as a ranked, colour-coded
  "Your Reckoning" panel that **persists across the whole visit** — and *gates lines*: be merciful to Sable,
  and a **[Merciful]** option opens with Pip you'd never otherwise see.
- 🐛 **Engine fix:** `goNode` only applied a node's `onEnter` effects, silently dropping outcome-node `effects`
  — so regard, flags, and dispositions set on outcome nodes never registered at runtime. Now both `onEnter`
  and `effects` apply, in the market *and* the dialogue sim (which had the same latent bug).
- 🧪 New gates pin all of it (glossary hits, passive lore recall by class, the Returned-sense DC, disposition
  accrual, and the cross-conversation disposition gate); both reactive trees still complete for all five
  builds. Suite now **753 checks**, all green. *(This pass enriches the Market; the dialogue sim keeps the
  passive/active fix and is next in line for the glossary + sense + reckoning layers.)*

---

## 👑 v4.69.0 — *"Passive & Active"* — proper BG3/5e check resolution in the reactive dialogue (tooling)

> A correctness pass on how skill checks present, to match Baldur's Gate 3 / 5e exactly. Checks come in two
> kinds, and they should behave very differently — which they now do across both reactive pages (the dialogue
> demo *and* the walkable market).

- 👁️ **Passive checks** — the *knowledge & awareness* skills (Perception, Insight, Investigation, Religion,
  Arcana, History, Nature, Medicine, Survival): these are **not rolled.** The option only appears **if you'd
  already pass** — `passive score = 10 + ability modifier (+ proficiency) ≥ DC` — and selecting it is an
  automatic success. If your passive score is below the DC, you never see the option at all (you simply don't
  notice / don't know). Shown with a `👁 … · passive` tag and a `16 vs DC 13 ✓ auto` chip instead of a roll.
- 🎲 **Active checks** — the *social-attempt* skills (Persuasion, Deception, Intimidation): always offered (you
  may always *try*), and resolved by the d20 roll with the % chance, exactly as before.
- 🚫 **Identity & stat gates ruled out entirely** — race / class / background / deity / alignment and passive
  ability thresholds (`[STRENGTH 15]`) already hide the option completely when unmet; that behavior is
  reaffirmed and now sits cleanly alongside the passive-check hiding. ("Show choices I don't qualify for"
  reveals them greyed, with the reason — e.g. *passive Insight 11 < DC 13 — you don't catch it*.)
- 🧩 Applied to **both** `dialogue_sim.html` (the "The Threshold" demo) and `town_market.html` (Sable/Bram/Pip).
  The compiled 208-conversation corpus carries only an ability (no skill name) on its checks, so those remain
  active rolls — unchanged.
- 🧪 New tests in both gates pin the rules: passive vs active classification, `passiveScore = 10 + mod (+prof)`,
  a passive option hiding below its DC and auto-succeeding above it, and an active option always being offered
  regardless of the relevant ability. Both reactive trees still complete for all five builds. Suite now
  **742 checks**, all green.

---

## 👑 v4.68.0 — *"The Market of the Causeway"* — a walkable scene with three reactive NPCs, in one file (tooling + content)

> A single self-contained HTML you can just *open* — like the combat preview, but for talking. You **walk** a
> character around an isometric market square (click-to-move, exactly like the combat sim) and approach **three
> named souls**, each with a deep, branching, character-reactive conversation. The same engine as the dialogue
> demo: the NPC reads your race, faith, alignment, background, and stats and answers differently.

- 🏪 **New page: `play/town_market.html`** — fully self-contained (67 KB, no server, no other files):
  - **Walk the square** — a canvas isometric market (fountain, stalls, crates, barrels, banners) with
    click-to-move, depth-sorted rendering, a player token, and proximity "▶ talk (E)" prompts.
  - **Three named NPCs**, each a complete reactive tree (`play/town-market.json`, 32 beats total):
    **Mother Sable** the bone-charm seller (reacts to faith/race; haggle, Insight & Religion checks; a quiet
    truth about what she really sells), **Sergeant Bram Holloway** the recruiter (reacts to class/background;
    a STR arm-wrestle, an Insight that cracks his pitch open, an Intimidation that makes him stand down), and
    **Pip** the street-child (reacts to alignment/race; a Perception to catch the lift, an Insight into *why*
    she steals, and a kindness that earns a child's whole loyalty).
  - **The same BG3-style reactivity** as the dialogue demo: reactive opening lines, identity-tagged choices,
    passive stat gates, and skill checks rolling `d20 + ability modifier + proficiency` vs the DC with an
    animated die. A live ledger shows each NPC's regard and every story flag your visit writes — and flags
    persist across all three conversations (be kind to Pip *and* see Sable, and both are on your record).
  - **Build your character** right there (race/class/background/alignment/deity + ability steppers, or the
    five presets) and the market answers the change. Preview toggles for revealing effects and forcing checks.
- 🧩 **Added as the 11th bundle tab** (`🏪 Market`) and a card on the index.
- 🧪 **New gate (`town_market.test.js`, 36 checks)**: validates the scene, lifts the page's `/*<MKT>*/` engine
  and proves it matches the C# rules, confirms the three NPCs greet/branch differently by character, and
  **auto-plays every NPC tree to an end for all five shipped builds** (15 combos, no dead character). Suite
  now **732 checks**, all green.
- 🛠️ **CI fix:** the recurring red "Unity EditMode tests" job checked out with `lfs: true`, which fails on the
  repo's exhausted Git LFS budget — yet the logic tests never load the LFS art. Set `lfs: false` so the job
  checks out and runs (it still self-skips cleanly until a `UNITY_LICENSE` secret is added).

---

## 👑 v4.67.0 — *"The Threshold"* — BG3-style character-reactive dialogue, with a playable demo (tooling + content)

> The dialogue sim now answers *who you are.* A full **character model** — race, class, background,
> alignment, deity, and the six abilities — feeds a reactive engine where the NPC's lines and the choices on
> offer change with your identity, and skill checks add **proficiency** on top of the ability modifier. A new
> hand-authored demo, **"The Threshold,"** shows it off: a gatekeeper of the dead reads your blood, your god,
> and your bearing — and decides whether you pass.

- 🎭 **Character model** (`play/dialogue-demo.json`) — 9 races, 5 classes, 7 backgrounds, the 9 alignments,
  7 deities (incl. **"None" — the Faithless**, the saga's thematic core), a full skill→ability table, and
  class/background **skill proficiencies** (+3). The five shipped builds are now complete BG3-style identities
  (e.g. *The Confessor*, a Kelemvorite cleric; *The Warden*, a Faithless elf; *The Silver Tongue*, a tiefling).
- ✦ **Reactive demo: "The Threshold"** — Justiciar Aldous Vane, 29 nodes, deeply branching:
  - **Reactive narration** — the opening line differs for a Kelemvorite (greeted as *kin*), a tiefling
    (eyed for *fiend-blood*), a drow, an elf (*"you watched it born"*), a Myrkul-worshipper (*hostility*),
    and one of the **Faithless** (*"the air doesn't bend around the godless… you know what's waiting"*).
  - **Identity-gated choices**, tagged BG3-style and colour-coded: **[ELF] [CLERIC] [ACOLYTE] [KELEMVOR]
    [FAITHLESS] [LAWFUL]**, plus **passive stat gates [STRENGTH] / [INTELLIGENCE]** that appear only at the
    score, and active **skill checks** (Persuasion, Insight, Intimidation, Religion, Deception) that roll
    `d20 + ability modifier + proficiency` vs the DC. Different characters literally see different options.
  - **Consequences** — the Justiciar's **regard** moves with every choice (shown as a live bar), and a late
    node even reads that regard back (`int ≥ 3`) to deepen what he confides; six distinct outcomes from
    *passed-as-kin* to *forced the gate (and fed the waking Wall)* to *it comes to blades*.
- 🧩 **Engine upgrades** in the sim: a character-creation panel (race/class/background/alignment/deity +
  ability steppers), colour-coded tag chips, per-choice **% chance to pass with your build** including
  proficiency, a "show choices I don't qualify for" toggle, and the demo surfaced with a one-click CTA.
- 🧪 **Gate expanded to 82 checks** — lifts the new pure functions (`matchesWhen`, `pickVariantText`,
  `isProficient`, `checkBonus`, `choiceAvailable`) and proves them; verifies the demo greets Kelemvor vs
  Faithless differently and offers different choices per character; and **auto-plays the demo to an end for
  all five shipped builds** (no dead character). Suite now **696 checks**, all green.

---

## 👑 v4.66.0 — *"Dialogue, Played"* — a new playable preview: step INTO any conversation (tooling)

> The combat sim lets you play a fight; now there's a sibling for the writing. **`dialogue_sim.html`** lets you
> step into any of the **208 conversations** as your own Returned — browse the cast, make the choices, and roll
> real skill checks — so the saga's dialogue is as previewable as its combat. Built on the same
> `dialogue-data.json` the viewer and saga map read, and on the real resolution rules.

- 🎬 **New page: "Dialogue, Played"** — an NPC-first, visual-novel-style player:
  - **Cast browser** of **144 speakers** (companions & quest-givers featured first, each with a deterministic
    sigil), → their conversations → step in.
  - **Faithful skill checks** — a choice's check rolls `d20 + your ability modifier` vs the DC, exactly as
    `DialogueRunner.cs` does (`Modifier = floor((score-10)/2)`), with an **animated d20**, the modifier
    breakdown, and success/fail branching to the real fail node.
  - **Pick your build** — five authentic class arrays (Fighter/Wizard/Cleric/Rogue/Ranger) plus per-ability
    steppers; INT/WIS/CHA are highlighted because they're the only abilities dialogue checks use, so the
    page even shows each choice's **% chance to pass with your build**.
  - **Live state** — companion approval bars, faction reputation, and every story flag your choices write,
    grouped by domain — exactly what the real save would record.
  - **Preview tools** — *reveal what each choice does* before you click, *decide check results* to explore
    both branches of a check without save-scumming, and *assume earlier story flags* to unlock gated choices.
  - Conditional choices (`RequireBoolTrue`) lock with a "needs earlier: …" note; light synthesized SFX and
    number-key selection round it out.
- 🧩 **Added as the 10th tab** in the all-in-one bundle (`🎬 Play Dialogue`) and a card on the index.
- 🧪 **New gate (`dialogue_sim.test.js`, 49 checks)** lifts the page's pure `/*<DLGSIM>*/` block and proves it
  matches the C# engine (modifier, check resolution, pass-chance, effects, conditions), validates the cast
  index covers every conversation, and **auto-plays all 208 conversations to an end** (no broken mid-path ref,
  bounded steps). Suite now **663 checks**, all green.

---

## 👑 v4.65.0 — *"The Wiring, Revealed"* — the browsable flag graph catches up to the whole saga (tooling)

> The Flag Dependency Graph, Saga Map, and Save Inspector all read `play/flags-data.json` — which hadn't been
> regenerated since it was introduced in v3.78. It was a **stale 167-flag snapshot** while the game had grown to
> **1,240 flags**: every side quest, convergence finale, NPC, and banter added since was invisible in the tools.
> Re-extracting from the current content surfaces all of it — the reactive wiring is finally browsable.

- 🕸️ **Flag graph rebuilt: 167 → 1,240 flags**, 515 → **2,324 read/write edges**, 27 → **39 domains** — the
  `Sq` (side-quest) domain alone now carries **232** flags, including the Lantern-Feast and every convergence
  cluster. Pick any `sq.*` flag and see exactly which dialogue choice writes it and which epilogue slide reads
  it. (`tools/extract-flags.py`, unchanged — only the data was stale.)
- 🗺️ **Saga Map deepened** — the deciding-flag set it renders grew from 108 to **184**, so the campaign-to-
  endings wiring on one page now reflects the real saga, not a years-old slice.
- 💾 **Save Inspector refreshed** with the full flag set for autocomplete and domain hints — and its hand-built
  **Combat Chronicle** panel (`DEEDCHRON`) surgically preserved through the regeneration (the generator doesn't
  emit it), so the deed read-back loop stays intact.
- 📦 **All-in-one bundle regenerated** with the refreshed flags, saga, and inspector tabs.
- 🧪 No logic changed; the **613-check** suite stays green. The flag/saga/inspector gates compute their counts
  from the data, so they validated the rebuilt graph automatically (1,240 flags / 2,324 edges / 39 domains).

---

## 👑 v4.64.0 — *"Movements Coalesced"* — the convergence layer reaches the spoiler-free Chronicle (reactivity)

> The four convergence finales (v4.62) lived only in the personalized epilogue. This completes the feature
> across **both** narrative surfaces — the project's standing rule — by teaching the spoiler-free **Chronicle**
> tally to recognize the same emergent movements, named, without spoiling their prose.

- 🎖️ **New Chronicle line: "Movements coalesced: N/3"** — when a player has brought home ≥3 quests of a
  thematic cluster, the Chronicle now names the movement they built — **⚙️ The Counter-Machine**,
  **📚 The Case Was Complete**, **🕯️ Every Name Kept** — counting how many of the three coalesced, in the
  same order the epilogue uses. A breadth readout that complements the per-quest `🪙 N/21` tally.
- 🔁 **Byte-identical across all three engine copies** — `endings.js`, `EndingResolver.cs`, and the inlined
  engine in `endings_explorer.html` (DEEDEPI/import chrome preserved), the all-in-one bundle regenerated.
- 🧪 **New Chronicle gate** pins the threshold (no line below 3 in any cluster), the naming, the `1/3`→`3/3`
  count, and the exact ordering of the joined movement names — confirming JS and C# emit the identical string.
  Suite now **613 checks**, all green.

---

## 👑 v4.63.0 — *"The Last Lantern-Feast"* — the twenty-first side quest; joy as the weapon no machine planned for (Pillar 2/4 + reactivity)

> Twenty quests of the long road were about the **dead** — grief, testimony, rescue, remembrance. This one is
> about the **living**, and it brings a register the whole saga has never sounded: *joy.* A complete branching
> side quest in a deliberately un-morose key, fully wired into the epilogue and Chronicle the same release,
> rounding the reactive suite to **21**.

- 🏮 **Side Quest: "The Last Lantern-Feast"** (`lantern_feast_sidequest.dlg`, gated on `sq.lantern_feast_threatened`)
  — Dot Marigold, seventy and undefeated, has run the Steelshanks ward's Lantern-Feast for forty years: one
  night a year when the poor quarter — harvest-marked and faithful, side by side — dances, courts, eats too
  much, and at midnight floats paper lanterns for the year's losses. The Choir has cancelled it as "frivolous
  in a time of holy sorrow." The truth is worse: a quarter of the doomed *dancing, beloved, seen* is a quarter
  that's forgotten it's supposed to be cargo, and that is the one thing the harvest can't allow.
- 🎭 **Five ways in, seven resolutions** — hold it **open in defiance** (the Choir's own notice burning atop
  the first pole), keep it **safe in the cellars** (a dancer who lives to dance again has beaten them twice),
  weave joy and grief into **one whole human night**, **teach it to a hundred small hands** so it outlives her
  (you can hang a baker, not a recipe), **give it away to the whole ward** so no single arrest can end it,
  draw out the husband **Tomas** who started it all by refusing to let a man be taken in silence, or **carry a
  lantern to the Court** to blind the powers with a quarter's overflowing love.
- 🪙 **The thesis** — they built the machine against grief and against courage and had answers for both. They
  never armored it against a *party.* You can call a sorrow deserved; no doctrine in any heaven can look at a
  marked girl courted under the lights and call *that* nothing.
- 🔁 **Fully reactive, byte-identical across all three engine copies** — six priority-ordered epilogue slides
  (`endings.js`, `EndingResolver.cs`, inlined `endings_explorer.html`), the **21st** entry in the spoiler-free
  Chronicle tally (`🪙 Side quests of the long road: N/21 brought home`), the all-in-one bundle and dialogue
  viewer regenerated (now **208 conversations / 1,474 nodes**), DEEDEPI/import chrome preserved.
- 🧪 **New epilogue gate** pins the feast's primary-resolution priority and its tally membership; the
  prose-parity gate confirms the new slide text is identical JS↔C#. Suite now **612 checks**, all green.

---

## 👑 v4.62.0 — *"The Long Road"* — convergence finales: the movement your small mercies built (reactivity)

> Not a twenty-first overlapping quest, but a new reactive *layer* on top of the twenty: the epilogue now
> recognizes the **emergent movement** a player assembled across clusters of related quests. Breadth is
> rewarded, not just depth. Four new convergence slides fire when you bring home a *cluster* of kindred
> side quests — revealing that the separate small mercies were, all along, one larger work.

- ⚙️ **Convergence: "The Counter-Machine"** — fires when ≥3 of the *rescue/infrastructure* quests resolve
  (the harvest exposed, Hessa's routes, Brindle's silent forks, the Ferryman's free crossings, de Lancie's
  purse). The underground that empties the Wall's intake faster than the harvest fills it — mercy given a
  logistics to answer cruelty's Choir and clerks.
- 📚 **Convergence: "The Case Was Complete"** — fires when ≥3 of the *record/testimony* quests resolve
  (Naeve's grief, the Complaint's indictment, the Unabridged census, the Catalogue of the Unwritten,
  Threnn's true count). Five separate cries stacked into a single, undismissable brief.
- 🕯️ **Convergence: "Every Name Kept"** — fires when ≥3 of the *remembrance* quests resolve (the graves,
  the dirge, the wickless name, the reaper's forty-one, the mournlight, the Reading of the Wall). The Realm,
  proven name by name, never to have contained any such thing as "no one."
- 👑 **Capstone: "The Long Road"** — fires only when all three movements are present at once. Rescue,
  prosecution, and remembrance revealed as one work wearing three coats — the breadth reward for a player
  who did all of it, quest after small quest.
- 🔁 **Byte-identical across all three engine copies** — `play/endings.js`, `Assets/Scripts/Core/EndingResolver.cs`,
  and the inlined engine in `endings_explorer.html`, with the `make-all-in-one` bundle regenerated and the
  DEEDEPI/import chrome preserved.
- 🧪 **Two new epilogue gates** pin the cluster thresholds (no slide at 1 or 2; slide at 3; resolutions of an
  already-counted quest never double-count) and the capstone (only with all three movements). Suite now
  **611 checks**, all green; prose-parity gate confirms JS↔C# slide text is identical.

---

## 👑 v4.61.0 — *"The Two Ledgers"* — the twentieth side quest; cross-quest economics (Pillar 2/4 + reactivity)

> An economic/moral-arithmetic dilemma — the dead\'s honest count versus the living\'s roof — and the first
> quest whose cleanest answer lives *in another quest\'s books*. A twentieth full side quest, rounding the
> reactive suite to **20**, written, verified, and wired into the epilogue and Chronicle the same release.

- 📒 **Side Quest: "The Two Ledgers"** (`threnn_sidequest.dlg`, gated on `sq.threnn_true_ledger`) — Archivist
  Threnn keeps two books: the public lie (a round 412 harvest-dead, holding up every property value in the
  Upper City) and the true count, every name written correctly — the only honest grave the harvested will get.
  Publishing the truth craters rents and makes the living homeless; hiding it enrolls them in the forgetting.
  She agonized a year and never found the column that saves both. Four ways in, and resolutions: the **third
  column** — the de Lancie Fund catches the falling so the truth can be published without a widow on the street
  (`sq.threnn_third_column`, `sq.delancie_endowment` — a cross-quest payoff with *"A Point"*); **publish** and
  let the fortunes crater (`sq.threnn_truth_published`); route it to a **memorial** not an assessment
  (`sq.threnn_true_to_memorial`); carry it to the **Court** as the one honest number between the god\'s lie and
  the city\'s (`sq.threnn_to_the_court`, `act5.the_great_objection`); draw out **Corrin**, the clerk whose
  rounding-away started the secret ledger (`sq.threnn_corrin`); or **share** it so the count outlives her
  (`sq.threnn_shared`, `act5.unbroken_line`).
- 🔗 **Cross-quest mechanic**: the third-column resolution sets `sq.delancie_endowment` — the wealth quest\'s
  Fund becomes the literal answer to the economics quest\'s dilemma, so a player who ran both gets a solution
  neither could reach alone. The suite is starting to interlock.
- 🎬 **Wired into epilogue + Chronicle same release**: a slide per resolution, tally grows to **N/20 brought
  home** — byte-identical across `endings.js` and `EndingResolver.cs`, injected into the Endings Explorer
  (chrome preserved), flowed into the all-in-one. Test tally updated to `3/20`. Prose-parity green, fuzz
  crash-free.
- 🧵 **Theme**: the moral arithmetic that traps a whole city in one grief traded for another is almost never
  solved inside its own closed books, but by a column borrowed from a larger, kinder ledger than the one you
  started with — and even in the coldest counting-house, someone could not stop herself from writing the names
  of the forgotten down correctly.
- 📈 **Content**: dialogue corpus now **207 conversations · 1,462 nodes · 176 skill checks** (834 choices);
  **twenty** full side quests, all reactive. Headless suite holds at **609 checks**, all green; the 55-file
  `.dlg` corpus compiles clean.

---

## 👑 v4.60.0 — *"A Point"* — the nineteenth side quest; the bored heiress who collected a conscience (Pillar 2/4 + reactivity)

> The suite\'s only wealth/satire texture: an aristocrat\'s class-power — money, status, access, taste — pointed,
> for the first time in its history, at something other than itself. A nineteenth full side quest — written,
> verified, and wired into the epilogue and Chronicle the same release; the corpus crosses 1,450 nodes.

- 💎 **Side Quest: "A Point"** (`delancie_sidequest.dlg`, gated on `sq.delancie_patron`) — Ysolde de Lancie went
  shopping for immortality, discovered immortality is just the Wall by another name (nothing, forever), and
  found instead the one thing money can\'t buy: a use. Now she has assets the resistance never had — obscene
  wealth, a name that opens every Upper City door, and the power to *set fashion* among the class that funds the
  harvest. Four ways in (her smallest asset is money — turn her *class against itself*; be the key to authorized
  rooms; institutionalize mercy into a permanent fund; or lean into the fear that the thing that can break was
  always the thing worth being), and resolutions: make complicity unfashionable across the Upper City
  (`sq.delancie_shames_the_peers`, `act5.refusing_consent`), open the doors the resistance can\'t
  (`sq.delancie_opens_doors`, `act5.the_great_objection`), build the unbreakable de Lancie Fund
  (`sq.delancie_endowment`, `act5.unbroken_line`), or wager the inheritance itself with skin in the game
  (`sq.delancie_commits`).
- 🎬 **Wired into epilogue + Chronicle same release**: a "where they are now" slide per resolution, tally grows
  to **N/19 brought home** — byte-identical across `endings.js` and `EndingResolver.cs`, injected into the
  Endings Explorer (chrome preserved), flowed into the all-in-one. Test tally updated to `3/19`. Prose-parity
  green, fuzz crash-free.
- 🧵 **Theme**: the harvest armored itself against the grief of the poor and the swords of the brave and never
  once imagined the comfortable getting bored of pretending not to see — and the most exotic, vulgar,
  transformative thing a patrician could ever collect is a conscience.
- 📈 **Content**: dialogue corpus now **206 conversations · 1,450 nodes · 175 skill checks** (827 choices);
  **nineteen** full side quests, all reactive. Headless suite holds at **609 checks**, all green; the 54-file
  `.dlg` corpus compiles clean.

---

## 👑 v4.59.0 — *"The Coinless Crossing"* — the eighteenth side quest; the god vs the snare (Pillar 2/4 + reactivity)

> A ferrying/labor texture with the sharpest faction-revelation on the road: the cruelty was never the god\'s
> rule, but a snare the church bolted to his holy coin. An eighteenth full side quest — written, verified, and
> wired into the epilogue and Chronicle the same release.

- ⛴️ **Side Quest: "The Coinless Crossing"** (`ferryman_sidequest.dlg`, gated on `sq.ferryman_free`) — the old
  Ferryman of Chionthar runs his black skiff free at dusk now, crossing the breathless dead the harvest left
  coinless (died mid-sentence, no time to fetch a purse) before the near bank\'s harvesters can sort them to
  the Wall. But one old man and one skiff is a thin mercy against a river of the unpaid — and the toll-as-snare
  is the *church\'s*, not the god\'s. Four ways in (hold the bank so the waiting\'s safe; light a fleet so it\'s
  short; say out loud that the waiting-rule is the church\'s snare; or reforge the toll from the single coin),
  and resolutions: guard the near bank (`sq.crossing_near_bank_held`), a fleet of free crossings
  (`sq.crossing_fleet`), expose the snare — Kelemvor\'s rule was only \'a coin for the crossing,\' the waiting was
  the church\'s forgery (`sq.crossing_snare_exposed`, `act1.choir_doubt_seeded`, reactive
  `faction.choir`/`faction.kelemvor`), reforge the toll into \'the ones who can pay for the ones who can\'t\'
  (`sq.crossing_mutual_toll`), or make the crossing outlive the old man (`sq.crossing_outlives_him`,
  `act5.unbroken_line`).
- 🎬 **Wired into epilogue + Chronicle same release**: a "where they are now" slide per resolution, tally grows
  to **N/18 brought home** — byte-identical across `endings.js` and `EndingResolver.cs`, injected into the
  Endings Explorer (chrome preserved), flowed into the all-in-one. Test tally updated to `3/18`. Prose-parity
  green, fuzz crash-free.
- 🧵 **Theme**: the cruelty was never the god\'s at all, but a snare the church bolted to his holy coin and
  called his will — and the kindest thing you can do for a people taught to fear the wrong master is to teach
  them to tell the god\'s old mercy from the trap that wears its face. (A Doomguide who learns the waiting-rule
  was never Kelemvor\'s might lay the whole Wall down.)
- 📈 **Content**: dialogue corpus now **205 conversations · 1,438 nodes · 174 skill checks** (820 choices);
  **eighteen** full side quests, all reactive. Headless suite holds at **609 checks**, all green; the 53-file
  `.dlg` corpus compiles clean.

---

## 👑 v4.58.0 — *"The Catalogue of the Unwritten"* — the seventeenth side quest; grief for the uncreated (Pillar 2 + reactivity)

> Grief not for the dead but for everything they\'d have *made* — the saddest object in the Realms, and one
> of the most hopeful turns on it. A seventeenth full side quest — fresh, melancholy, ultimately a goad to the
> living — written, verified, and wired into the epilogue and Chronicle the same release.

- 📚 **Side Quest: "The Catalogue of the Unwritten"** (`faolan_sidequest.dlg`, gated on `sq.faolan_catalogue`)
  — you bring Brother Faolan the Catalogue of the Unwritten: a complete index of every work meant to exist and
  never made — the masterworks the war interrupted, the genius the plague took at nineteen, the Letters to a
  Son a soldier was walled before he could write. Faolan is in it himself, a cycle of poems he shelved at his
  vows. Four ways in (it\'s a *summons*, not a graveyard — half the makers still live; read it aloud as a
  funeral; Faolan writes his own page into a lie; or it proves the Wall\'s deepest theft), and resolutions: the
  funeral for the dead makers (`sq.unwritten_read_aloud`), a goad to the living to write before time makes
  their entries true (`sq.unwritten_goad_the_living`, `sq.faolan_writes_his_poems`), an indictment of the Wall
  for filing eleven thousand *libraries* of the unmade to nothing (`sq.unwritten_the_walls_theft`,
  `act5.the_great_objection`), a second book — the Catalogue of the *Written-After-All*, a register of
  deadlines beaten (`sq.unwritten_written_after_all`, `act5.unbroken_line`), or healing the cataloguers
  themselves, un-shelving every scholar who quit being a maker to become a filer (`sq.unwritten_heal_the_filers`).
- 🎬 **Wired into epilogue + Chronicle same release**: a "where they are now" slide per resolution, tally grows
  to **N/17 brought home** — byte-identical across `endings.js` and `EndingResolver.cs`, injected into the
  Endings Explorer (chrome preserved), flowed into the all-in-one. Test tally updated to `3/17`. Prose-parity
  green, fuzz crash-free.
- 🧵 **Theme**: the Wall\'s cruelty was never only the souls it took, but everything those souls were still
  going to make — and the saddest library in the Realms is the one each of us keeps locked inside our own
  abandoned youth, where the books we meant to write wait, unwritten but not yet impossible, for the day we
  remember we are only late.
- 📈 **Content**: dialogue corpus now **204 conversations · 1,426 nodes · 173 skill checks** (813 choices);
  **seventeen** full side quests, all reactive. Headless suite holds at **609 checks**, all green; the 52-file
  `.dlg` corpus compiles clean.

---

## 👑 v4.57.0 — *"The Reading of the Wall"* — the sixteenth side quest; the forgetting was illiteracy (Pillar 1/2 + reactivity)

> The sharpest of all the road\'s truths about the Wall, made a quest: the Faithless were never nameless. A
> sixteenth full side quest — plot-resonant (it pays off your true name) and a literacy/decipherment texture —
> written, verified, and wired into the epilogue and Chronicle the same release.

- 📜 **Side Quest: "The Reading of the Wall"** (`ekko_sidequest.dlg`, gated on `sq.read_the_wall`) — Mistress
  Ekko taught you the dead tongue the Wall was carved in, and comes with you to read it. The revelation: the
  Wall never made the Faithless *nameless* — it named them with terrible care, in the oldest tongue, and then
  carved the names in a script it made certain no living mourner could read, so grief could stand before a
  wall of the named and believe it nameless. *The forgetting was enforced by illiteracy.* Four ways in (read
  your own unfinished name, "the one who returns"; read the dead their names so they learn they were known;
  read it aloud, the first reader in ten thousand years; or end the illiteracy by teaching the tongue widely),
  and resolutions: claim your true name (`sq.true_name_claimed`, `act2.true_name_meaning`,
  `act3.break_the_crown_known`), read the walled their own names (`sq.wall_read_to_the_dead`,
  `act5.witness_then_release`), the great reading aloud (`sq.wall_read_aloud`), teach the tongue to the Realms
  (`sq.deadtongue_taught`, `act5.wall_breaking`), write the primer that outlives every reader
  (`sq.deadtongue_primer`, `act5.unbroken_line`), or carry the literacy to the Court (`sq.deadtongue_to_the_court`,
  `act5.the_great_objection`).
- 🎬 **Wired into epilogue + Chronicle same release**: a "where they are now" slide per resolution, tally grows
  to **N/16 brought home** — byte-identical across `endings.js` and `EndingResolver.cs`, injected into the
  Endings Explorer (chrome preserved), flowed into the all-in-one. Test tally updated to `3/16`. Prose-parity
  green, fuzz crash-free.
- 🧵 **Theme**: the forgetting was never erasure — it was illiteracy, engineered and maintained for ten
  thousand years, a wall of the named dressed as a wall of the nameless. And the unmaking of it is not a sword
  or a crown but the most patient weapon there is: teaching the world to read its own dead.
- 📈 **Content**: dialogue corpus now **203 conversations · 1,414 nodes · 172 skill checks** (806 choices);
  **sixteen** full side quests, all reactive. Headless suite holds at **609 checks**, all green; the 51-file
  `.dlg` corpus compiles clean.

---

## 👑 v4.56.0 — *"The Map of Silences"* — the fifteenth side quest; the harvest-compass (Pillar 2 + reactivity)

> Gnomish comic warmth over real dread, and a device/investigation texture unlike anything else in the suite.
> A fifteenth full side quest — written, verified, and wired into the epilogue and Chronicle the same release.

- 📻 **Side Quest: "The Map of Silences"** (`brindle_sidequest.dlg`, gated on `sq.brindle_resonator`) — Brindle
  Quillfeather, the gnome who spent thirty years building beautiful useless things (a summer-chord, a
  Melancholy Engine), has a Sympathetic Resonator — a frivolous empathy-detector — that goes *dead silent*
  where the harvest has eaten not just the dead but the *memory* of them: a map of eleven growing holes where
  grief can\'t reach, the unmade, the places that forgot how to ache. A harvest-compass, built by accident.
  Four ways in (it predicts where the harvest strikes *next*; it\'s a witness no one can call mad; it points to
  the deepest silence at the heart of the unmade; or it\'s a to-do list for pouring the remembering back), and
  resolutions: an early-warning to outrun the harvest and feed Hessa\'s hulls (`sq.silences_early_warning`), a
  documented witness (`sq.silences_mapped_pattern`, `sq.harvest_map_obtained`), a path to the unmade\'s heart
  (`sq.silences_deepest`), filling the holes by teaching the silent places to grieve (`sq.silences_filled`), a
  whole *watch* of forks built by the hundred (`sq.silences_resonator_guild`, `act5.unbroken_line`), or a
  measurement to damn the Court in its own chamber (`sq.silences_to_the_court`, `act5.the_great_objection`).
- 🎬 **Wired into epilogue + Chronicle same release**: a "where they are now" slide per resolution, tally grows
  to **N/15 brought home** — byte-identical across `endings.js` and `EndingResolver.cs`, injected into the
  Endings Explorer (chrome preserved), flowed into the all-in-one. Test tally updated to `3/15`. Prose-parity
  green, fuzz crash-free.
- 🧵 **Theme**: a machine built to celebrate how much a city loved became, in a time of harvest, the one tool
  that could measure how much it had been made to *forget* — and the repair of a silence is never the machine,
  but the people who go to the holes and teach the world to ache again. (Grief can be forbidden; a measurement
  cannot.)
- 📈 **Content**: dialogue corpus now **202 conversations · 1,402 nodes · 171 skill checks** (799 choices);
  **fifteen** full side quests, all reactive. Headless suite holds at **609 checks**, all green; the 50-file
  `.dlg` corpus compiles clean.

---

## 👑 v4.55.0 — *"Hessa\'s Mercy"* — the fourteenth side quest; the one that saves the living (Pillar 2 + reactivity)

> The first quest on the long road that rescues the living instead of mourning the dead. A fourteenth full
> side quest — saltier, more operational, a smuggler\'s logistics-and-conscience thriller — written, verified,
> and wired into the epilogue and Chronicle the same release.

- ⛵ **Side Quest: "Hessa\'s Mercy"** (`hessa_sidequest.dlg`, gated on `sq.hessa_underground`) — Hessa Dock-Eye
  runs the underground: free passage out of the Gate for the harvest-marked, on boats the Fist is paid not to
  search, before the Choir can wall them. Not a memorial — a *rescue*, with all the bribes, triage, and bad
  arithmetic that running people out from under a death-machine actually costs. Four ways in (take the
  god-of-the-docks triage off her gut; scale the hulls into a network; harden the route past its bought
  sergeant Brakker; or learn why *her* — a godless deckhand named Toller she waited one tenday too long to
  save), and resolutions: a council to share the gangplank\'s terrible math (`sq.mercy_route_council`), a web
  the Fist can\'t hang in a night (`sq.mercy_route_network`), a route hardened past its weakest link
  (`sq.mercy_route_hardened`), one built to outlive her noose (`sq.mercy_route_outlives_her`,
  `act5.unbroken_line`), or her manifest carried to the Court as proof the marked were never cargo
  (`sq.mercy_route_to_the_court`, `act5.the_great_objection`).
- 🎬 **Wired into epilogue + Chronicle same release**: a "where they are now" slide per resolution, tally grows
  to **N/14 brought home** — byte-identical across `endings.js` and `EndingResolver.cs`, injected into the
  Endings Explorer (chrome preserved), flowed into the all-in-one. Test tally updated to `3/14`. Prose-parity
  green, fuzz crash-free.
- 🧵 **Theme**: against a machine of cruelty — the Choir that marks, the Fist that enforces, the clerks that
  file — mercy needs a machine too, built of bribes and boards and back channels and captains shamed into
  good; and the truest monument a smuggler can leave is a route that doesn\'t notice when she\'s gone.
- 📈 **Content**: dialogue corpus now **201 conversations · 1,390 nodes · 170 skill checks** (792 choices);
  **fourteen** full side quests, all reactive. Headless suite holds at **609 checks**, all green; the 49-file
  `.dlg` corpus compiles clean.

---

## 👑 v4.54.0 — *"The Old Map"* — the thirteenth side quest; the wall that keeps the living (Pillar 2 + reactivity)

> A different wall, and a different kind of erasure: not the dead, but the living, severed by a curtain of
> blue fire. A thirteenth full side quest — written, verified, and wired into the epilogue and Chronicle the
> same release — and the dialogue corpus crosses **200 conversations.**

- 🗺️ **Side Quest: "The Old Map"** (`cartographer_sidequest.dlg`, gated on `sq.the_old_map`) — you carry the
  Plaguechanged Cartographer's master map of the world-as-it-was to the boundary where the Spellplague's blue
  fire walled Faerûn off from the Abeir-lands that fell out of the sky — including **Sevenfields**, where her
  estranged daughter lives, intact, three days' walk and one impassable curtain away. A wall that keeps the
  *living* the way the Wall of the Faithless keeps the dead: by calling the separation permanent. Four ways in
  (a map is carried where feet can't go; the quarrel was the real wall, not the fire; she's not the only one
  severed; or copy the world's memory beyond any flame), and resolutions: send the rehearsed apology onward on
  the one road that outlives the fire (`sq.map_the_reconciliation`), turn the map into a registry that
  reconnects everyone the fire severed (`sq.map_reunites_the_lost`), carry it to the Wall of the Faithless as
  proof that fire and stone tell the same lie (`sq.map_to_the_wall`, `act5.the_great_objection`), or copy the
  lost geography beyond burning (`sq.map_carried_across`, `sq.map_copied_wide`).
- 🎬 **Wired into epilogue + Chronicle same release**: a "where they are now" slide per resolution, tally grows
  to **N/13 brought home** — byte-identical across `endings.js` and `EndingResolver.cs`, injected into the
  Endings Explorer (chrome preserved), flowed into the all-in-one. Test tally updated to `3/13`. Prose-parity
  green, fuzz crash-free.
- 🧵 **Theme**: the Wall that keeps the dead and the fire that keeps the living are the *same* wall — both
  insisting the ones we love are gone when they are only on the other side — and the way across either is never
  the feet, which cannot cross, but the refusal to let the severed be forgotten, carried in maps and names and
  the unsaid words we send down roads that outlive the walls.
- 📈 **Content**: dialogue corpus now **200 conversations · 1,378 nodes · 169 skill checks** (785 choices);
  **thirteen** full side quests, all reactive. Headless suite holds at **609 checks**, all green; the 48-file
  `.dlg` corpus compiles clean.

---

## 👑 v4.53.0 — *"The Great Objection"* — the twelfth side quest (Pillar 2 + reactivity)

> The civic heart of the long road, paid off. A twelfth full side quest — the Complaint's age of grievances,
> and the daughter behind them — written, verified, and wired into the epilogue and Chronicle the same release.

- 📋 **Side Quest: "The Great Objection"** (`complaint_sidequest.dlg`, gated on `sq.the_complaints`) — the
  Complaint, the fussy man who has lodged grievances at the afterlife's never-staffed desk through every era,
  follows you to the Court, where for the first time the desk is *staffed.* His thousand petty complaints were
  always one indictment; the one he can't write is **Wrenna**, his eleven-year-old daughter, walled for the
  crime of thinking. Four ways in (file them as the indictment they always were, set Wrenna *first*, demand
  the cosmos *stamp it received*, or ask them *why* for Wrenna so he can grieve instead of file), and
  resolutions that file the great indictment (`sq.objection_indictment`, `sq.objection_wrenna_first`,
  `sq.objection_formally_received`), let you ask the powers why his daughter was walled while he finally weeps
  (`sq.objection_asked_for_him`, `act5.ask_them_why`), join his age of records to the whole road's case as its
  civic backbone (`sq.objection_joins_the_case`), or read it aloud where no drawer can swallow it
  (`sq.objection_read_aloud`).
- 🎬 **Wired into epilogue + Chronicle same release**: a "where they are now" slide per resolution, tally grows
  to **N/12 brought home** — byte-identical across `endings.js` and `EndingResolver.cs`, injected into the
  Endings Explorer (chrome preserved), flowed into the all-in-one. Test tally updated to `3/12`. Prose-parity
  green, fuzz crash-free.
- 🧵 **Theme**: a cruelty no one objects to becomes *fate* — clean, absolving, inevitable — and the simplest,
  most radical act in a cosmos arranged to answer to no one is to make it receive, in writing, in its own
  record, forever, the fact that someone stood up and said *no.*
- 📈 **Content**: dialogue corpus now **199 conversations · 1,366 nodes · 168 skill checks** (778 choices);
  **twelve** full side quests, all reactive. Headless suite holds at **609 checks**, all green; the 47-file
  `.dlg` corpus compiles clean.

---

## 👑 v4.52.0 — *"The Mournlight"* — the eleventh side quest; the golden-path key (Pillar 2 + reactivity)

> Grief as the truest weapon the road ever gave you. An eleventh full side quest — the golden-path key
> itself — written, verified, and wired into the epilogue and Chronicle the same release.

- 🕯️ **Side Quest: "The Mournlight"** (`mournlight_sidequest.dlg`, gated on `sq.carry_the_mournlight`) — you
  carry to the threshold of the Court the Mournlight: ten thousand forbidden mournings made into a radiance
  that weeps, the grief of every soul ever told it had no right to mourn a Faithless dead, borne on the one
  grief *you* were never allowed to feel. It asks not to be set down but what it is *for.* Four ways in (an
  exhibit that wins by being seen, a lantern for the dim golden road, a blaze to add your own forbidden grief
  to, or one mother's thirty unshed years for *Aline* and *Joss the fisher* as the face of the vast), and
  resolutions that shine it at the Court (`sq.mournlight_shone_at_court`, `act5.the_great_objection`), light
  the golden road with it (`sq.mournlight_lights_golden_road`, `act5.golden_road_known`), add your own grief
  (`sq.mournlight_grief_added`), or — most tender — turn it inward on the dead themselves, so every freed
  Faithless soul feels, in the moment of release, that they *were* mourned (`sq.mournlight_shone_on_the_dead`,
  `act5.witness_then_release`).
- 🎬 **Wired into epilogue + Chronicle same release**: a "where they are now" slide per resolution (priority-
  ordered), tally grows to **N/11 brought home** — byte-identical across `endings.js` and `EndingResolver.cs`,
  injected into the Endings Explorer (chrome preserved), flowed into the all-in-one. Test tally updated to
  `3/11`. Prose-parity green, fuzz crash-free.
- 🧵 **Theme**: the truest weapon the long road gives is not a sword or a crown but *grief* — the grief the
  powers forbade and that fell anyway — made visible, undeniable, and shining, in the one room that decreed it
  had no right to exist; and the kindest use of all that sorrow is not to indict the guilty but to let the dead
  feel, at the last, that they were grieved.
- 📈 **Content**: dialogue corpus now **198 conversations · 1,354 nodes · 167 skill checks** (771 choices);
  **eleven** full side quests, all reactive. Headless suite holds at **609 checks**, all green; the 46-file
  `.dlg` corpus compiles clean.

---

## 👑 v4.51.0 — *"The Dangerous Book"* — the tenth side quest (Pillar 2 + reactivity)

> Knowledge as burden, not another ledger to carry to court. A tenth full side quest — written, verified, and
> wired into the epilogue and Chronicle the same release — bringing the reactive side-quest count into double
> digits.

- 📕 **Side Quest: "The Dangerous Book"** (`candlekeep_sidequest.dlg`, gated on `sq.dangerous_book_access`) —
  Candlekeep seals one volume that is not a spell but a *truth*: the Unabridged, the full named census of ten
  thousand years of the discarded, the weight that stopped the last warm reader's heart — not by magic, but by
  the sheer size of feeling every walled soul at once. The grey cat Margin guards it, and stepped off it only
  for you, who are already half-cold. Four ways in:
  - read it **shared** — the truth that stops a solitary heart can't stop three that hold it together
    (`sq.book_read_shared`);
  - read it **whole** — the weight pours into the one heart the size can't stop, the crack in a Returned soul
    (`sq.book_read_whole`, `act5.witness_then_release`);
  - **copy** it beyond any seal — a truth too widely spread to ever concentrate back into one deadly book
    (`sq.book_copied_wide`);
  - or carry it to the **Court** — not an argument but a census, a list of the powers' own victims they cannot
    answer (`sq.book_to_the_court`, `act5.the_great_objection`); plus an *ask-Margin* resolution where the cat
    who read every reader chooses (`sq.book_margin_decides`).
- 🎬 **Wired into epilogue + Chronicle same release**: a "where they are now" slide per resolution (priority-
  ordered, with a combined whole+copied→Court slide), and the tally grows to **N/10 brought home** — mirrored
  byte-identically across `endings.js` and `EndingResolver.cs`, injected into the Endings Explorer (chrome
  preserved), flowed into the all-in-one. Test tally updated to `3/10`. Prose-parity green, fuzz crash-free.
- 🧵 **Theme**: the most dangerous thing in the greatest library is not a spell but a *list* — the true names
  of everyone the Wall threw away — and a truth too heavy for one heart is defeated not by being made lighter
  (impossible; the dead are the dead) but by being *shared*, *copied*, and *carried* by enough hands that no
  power left alive can ever seal it again.
- 📈 **Content**: dialogue corpus now **197 conversations · 1,342 nodes · 166 skill checks** (764 choices);
  **ten** full side quests, all reactive. Headless suite holds at **609 checks**, all green; the 45-file `.dlg`
  corpus compiles clean.

---

## 👑 v4.50.0 — *"The Glassblower's Window"* — the ninth side quest (Pillar 2 + reactivity)

> Grief made into light, not another ledger. A ninth full side quest — written, verified, and wired into the
> epilogue and Chronicle in the same release — and the fiftieth minor version of the saga's content work.

- 🪟 **Side Quest: "The Glassblower's Window"** (`glasswidow_sidequest.dlg`, gated on `sq.glasswidow_window`) —
  Senna the Glassblower lost her husband Corm to the Wall (Faithless, walled for a doubt muttered over ale),
  and set a stained-glass window of him — a man caught in the furnace-light — into the chapel wall. The church
  shatters it as heresy; she rebuilds it; seven times now, a war of attrition with cloth-wrapped hammers she's
  losing by aging. Four ways in (make him too *specific* to smash, *multiply* him into a hundred homes, carry
  his image to the *Court*, or just see the *picture* of the man), and resolutions that armor the memorial with
  the quarter's love (`sq.window_made_specific`, `sq.window_multiplied`), carry it to indict the Court
  (`sq.window_to_the_court`, `act5.the_great_objection`), or build one window slow enough to unveil where no
  Justiciar dares lift a hammer (`sq.window_one_that_lasts`).
- 🎬 **Wired into epilogue + Chronicle same release**: a "where they are now" slide per resolution (with a
  combined slide when she both makes him specific *and* multiplies him), and the Chronicle tally grows to
  **N/9 brought home** — mirrored byte-identically across `endings.js` and `EndingResolver.cs`, injected into the
  Endings Explorer (chrome preserved), flowed into the all-in-one. Test tally updated to `3/9`. Prose-parity
  green, fuzz crash-free.
- 🧵 **Theme**: a person the powers call *nothing* — fire and breath and the cheapest sand — can be made to
  *hold a shape* in the light that outlasts every hammer; and the memorial of the forgotten is not one window
  to be guarded, but a refusal scattered wide enough that erasing it would mean smashing the whole quarter's
  love.
- 📈 **Content**: dialogue corpus now **196 conversations · 1,330 nodes · 165 skill checks** (757 choices);
  **nine** full side quests, all reactive. Headless suite holds at **609 checks**, all green; the 44-file `.dlg`
  corpus compiles clean.

---

## 👑 v4.49.0 — *"The First Dirge"* — the eighth side quest, end-to-end (Pillar 2 + reactivity)

> A demonstration of the full content→reactivity pipeline in a single release: an eighth side quest, written,
> verified, *and* wired into the epilogue and Chronicle the same day.

- 🎶 **Side Quest: "The First Dirge"** (`lhoris_sidequest.dlg`, gated on `sq.the_first_dirge`) — Singer Lhoris,
  commanded ten thousand years ago to score the Faithless's last walk into the Wall, wove their names into the
  melody and smuggled the truth into its bones; the song became every funeral dirge since. You carry the
  original now, and at the threshold of the Court you hear its *second song* straining to be heard. Four ways
  in (it's a sealed letter in transit, an indictment for the Court, an open song to be re-sung kinder forever,
  or a vessel whose names you carry home), and resolutions that make it an anthem the whole world sings
  (`sq.dirge_becomes_anthem`), an indictment at the Court (`sq.dirge_sung_at_court`, `act5.the_great_objection`),
  a song freed to change forever (`sq.dirge_freed_to_change`), or names restored to the graves
  (`sq.dirge_names_restored`).
- 🎬 **Wired into the epilogue + Chronicle the same release**: a "where they are now" slide per resolution
  (priority-ordered), and the Chronicle tally grows to **N/8 brought home** — both mirrored byte-identically in
  `endings.js` and `EndingResolver.cs`, injected into the Endings Explorer's inlined engine (chrome preserved),
  and flowed into the all-in-one bundle. Tests updated (the tally case now reads `3/8`; a new assertion pins the
  Court-dirge slide). Prose-parity green, fuzz crash-free.
- 🧵 **Theme**: beauty in the service of power can be made to *smuggle* the very truth that power means to bury;
  a song is the one keeping the Wall can never file, because it is never finished; and the oldest dirge in the
  world was, all along, on the side of the dead it was written to mourn.
- 📈 **Content**: dialogue corpus now **195 conversations · 1,318 nodes · 164 skill checks** (750 choices);
  **eight** full side quests, all reactive. Headless suite holds at **609 checks**, all green; the 43-file
  `.dlg` corpus compiles clean.

---

## 👑 v4.48.0 — *"Brought Home"* — the side quests reach the Chronicle (Pillar 1 · reactivity)

> The companion to v4.47: the seven side quests now also register in the spoiler-free **Chronicle** — the
> running tally of your playthrough — mirroring the existing "Companion quests resolved: N/5" line.

- 🪙 **Side-quest tally in the Chronicle** (`endings.js` + `Assets/Scripts/Core/EndingResolver.cs`,
  byte-identical) — a new line, `🪙 Side quests of the long road: N/7 brought home`, counts how many of the
  seven built quests you resolved (any resolution counts; one per quest, no double-counting of alternate
  endings within a quest). Shown only when at least one is resolved, so it never disturbs the baseline
  Chronicle.
- 🧪 **Test**: a new `epilogue.test.js` case pins it — no tally when none resolved, and a 3-quest run (with a
  duplicate alternate flag set) reads exactly `3/7 brought home`, proving the no-double-count logic. The
  prose-parity gate stays green (the tally string is interpolated, outside the literal gate, and the C# and JS
  branch logic match).
- 🖥️ **Artifacts**: the tally flows into the all-in-one bundle (regenerated from `endings.js`) and is injected
  into the Endings Explorer's inlined engine, preserving its Save-Inspector and deed-folding chrome.
- 📈 **CI**: headless suite now **609 checks** (up from 608 — the new Chronicle test); all green.

---

## 👑 v4.47.0 — *"Where They Are Now"* — the side quests reach the epilogue (Pillar 1 · reactivity)

> The seven built side quests stop being self-contained and start *changing the run.* Their resolution flags
> now drive **epilogue slides** in the endings engine — so a playthrough's "where they are now" reflects the
> graves filled, the loop answered, the secrets named, the harvest exposed, the souls freed.

- 🎬 **Side quests wired into the Epilogue** (`endings.js` + `Assets/Scripts/Core/EndingResolver.cs`, byte-identical) —
  a new "side quests of the long road" block in `EndingResolver.Epilogue` emits a "where they are now" slide
  for each of the seven built quests, keyed to its resolution flags (priority-ordered, one slide per quest):
  - **The Graves That Waited** (`sq.field_of_the_rested` / `sq.graves_to_the_tenders` / `sq.every_soul_expected`);
  - **The Hand in the Margins** (`sq.margins_for_the_warden` / `sq.wrote_back_to_the_loop` / `sq.margins_warning_heeded`);
  - **The Thing in the Dark** (`sq.roen_and_sabira_reconcile` / `sq.roen_tells_the_fire` / `sq.roen_forgives_sabira`);
  - **The Indictment** (`sq.naeve_grieves_at_last` / `sq.naeve_keeps_one_page` / `sq.naeve_indictment_at_court`);
  - **The Hands That Refused** (`sq.harvest_exposed_public` / `sq.harvest_to_the_hands` / `sq.harvest_to_the_court`);
  - **The Forty-One** (`sq.fortyone_reaper_rests` / `sq.fortyone_reaper_joins_line` / the freeing flags);
  - **The Forbidden Name** (`sq.forbidden_name_spoken` / `sq.wickless_speaks_alone`).
- 🧪 **Test coverage**: two new `epilogue.test.js` cases pin the block — every quest contributes its slide when
  resolved, none leak in unset, the primary resolution outranks its alternates, and the seeded fuzz confirms
  no empty slides across the flag space. The **prose-parity gate** stays green: the new slides appear
  byte-for-byte in both the C# source and the JS port.
- 🖥️ **Artifacts refreshed**: the new slides flow into the **Endings & Epilogue Explorer** (`endings_explorer.html`,
  injected into its inlined engine without disturbing its Save-Inspector handoff or deed-folding chrome) and
  the **all-in-one bundle** (`crown_of_horns.html`, regenerated).
- 📈 **CI**: headless suite now **608 checks** (up from 606 — the two new epilogue tests); all green. The
  seeded side-quest content is now mechanically *reactive*: it changes what the game says about your run.

---

## 👑 v4.46.0 — *"The Forbidden Name"* — the candle's quest (Pillar 2 · side quests)

> The seventh `sq.*` payoff: Wickless, the candle-spirit a frightened child lit "for the person we're not
> allowed to talk about," follows its pull to the very soul it was lit for — and must find the courage to
> speak a forbidden name it has carried ten thousand years.

- 🕯️ **Side Quest: "The Forbidden Name"** (`wickless_sidequest.dlg`, gated on `sq.wickless_name`) — the candle
  leads you to a niche in the Wall where a woman has faded almost to nothing, erased by a whole family's quiet
  agreement not to speak of her. The name is *right there*, sealed by years of fear. Four ways through:
  - **reclaim** it — the name was never a shameful secret, only a love forced into hiding; say it like the love
    it always was, not a risked crime (`sq.wickless_reclaims`);
  - say it **to her** — let the first time her name is spoken in years be spoken to her face
    (`sq.wickless_to_her`, `act5.witness_then_release`);
  - **refuse the erasure** — someone once said "we don't talk about her"; be the one voice that says "we do
    now," and the family's silence breaks (`sq.wickless_refuses_erasure`, `act5.refusing_consent`);
  - or be **sure** — weigh fading-safe-but-unnamed against being-named-even-at-the-end, and choose the naming
    (`sq.wickless_sure`, `act4.deathless_is_the_wall`).
  - Climax: speak the name *with* the candle, two voices loud enough to break a forgetting
    (`sq.forbidden_name_spoken`), or let Wickless say it *alone* — the candle's to give, you only made it brave
    (`sq.wickless_speaks_alone`). The faded woman's light *catches*; a soul the Wall had almost finished erasing
    hears her own name said in love and remembers she had one.
- 🧵 **Theme**: a name carried in love — even by a thing as small as a candle, even around a child's frightened
  whisper — is a soul the Wall can never quite finish erasing; and the erasure of a person, however old and
  agreed-upon, breaks the moment one small voice refuses it and says the forbidden name out loud.
- 📈 **Content**: dialogue corpus now **194 conversations · 1,306 nodes · 163 skill checks** (743 choices).
  Seven full side quests now built. All 606 headless CI checks green; the 42-file `.dlg` corpus compiles clean.

---

## 👑 v4.45.0 — *"The Forty-One"* — the reaper's hidden souls (Pillar 2 · side quests)

> The sixth `sq.*` payoff: the Last Honest Psychopomp's forty-one fading Faithless, hidden in his cloak for
> ten thousand years against the Wall's pull — and now nearly gone, needing an ending he's too afraid to give.

- ⚱️ **Side Quest: "The Forty-One"** (`fugue_sidequest.dlg`, gated on `sq.reaper_hidden_souls`) — the reaper
  who refused the doctrine, who stole forty-one souls he was meant to deliver and slowed their fading by
  standing between them and the Wall with his own body, can hide them no longer. Four ways in:
  - the **gentle Wall** — his loving hiding became its own keeping; the forty-one need the *ending* he was too
    afraid to give (`sq.fortyone_gentle_wall`, `act4.deathless_is_the_wall`);
  - the **graves** — carry them to the coffin-man's waiting field, the *somewhere* he never had to take them
    (`sq.fortyone_to_the_graves`, links to "The Graves That Waited");
  - the **reading** — the one thing he could never do (speak their names, lest the Wall hear) becomes the thing
    that frees them (`sq.fortyone_to_the_reading`, `act5.wall_breaking`);
  - or **ask them** — open the cloak and let the forty-one choose their own ending, instead of one reaper's
    fear deciding for them (`sq.fortyone_ask_them`, `act5.witness_then_release`).
  - Epilogues for the reaper himself: join the unbroken line and go hide the *next* forty-one
    (`sq.fortyone_reaper_joins_line`), rest at last in a grave beside the souls he saved
    (`sq.fortyone_reaper_rests`), or carry the empty cloak as a victory — the shape of forty-one endings finally
    allowed (`sq.fortyone_victory`).
- 🧵 **Theme**: even *mercy* curdles into keeping; the souls we love are never ours to hold, only ours to
  *carry until they can choose*; and an empty cloak ten thousand years in the making is not the shape of a
  loss but of endings, bravely, allowed. (The ferryman, too, is owed a shore.)
- 📈 **Content**: dialogue corpus now **193 conversations · 1,295 nodes · 162 skill checks** (737 choices).
  Six full side quests now built atop the seeded hooks. All 606 headless CI checks green; the 41-file `.dlg`
  corpus compiles clean.

---

## 👑 v4.44.0 — *"The Hands That Refused"* — the harvest-exposure quest (Pillar 2/4 · side quests)

> The fifth `sq.*` payoff, and the first *convergence* quest — two NPCs from opposite ends of the same
> atrocity brought into one room. A faction-active piece (reactive `faction.choir` / `faction.fist`) with a
> more civic, plot-driving texture than the introspective companion arcs.

- ⚔️ **Side Quest: "The Hands That Refused"** (`harvest_sidequest.dlg`, gated on `sq.deserter_testimony` /
  `sq.choir_hands_refuse`) — Corwin the Fist deserter (the *fire*) and Sister Wren the Choir convert (the
  *chalk*), two halves of the harvest, brought together. Apart they're a deniable coward and a deniable
  doubter; together they're the undeniable proof the harvest runs on *ordinary hands that could always have
  opened.* Four approaches:
  - the **refutation** — stand public, living disproof of "we had no choice" (`sq.harvest_refutation`);
  - the **map** — combine fire and chalk into a flowchart of the machine (order→mark→enforce→burn) and hand
    every gear a mirror (`sq.harvest_map_obtained`, `act2.cataloguing_defeats_the_wall`);
  - the **prize** — name the real reward: not the villages they burned but the ones the *next* hand won't
    (`sq.harvest_prize`);
  - the **reconcile** — soldier and singer each say "my hand did this, and it could have opened" before they
    can testify together (`sq.harvest_reconciled`, `act5.shame_feeds_the_wall`).
  - Resolutions: nail it public where the city reads (`sq.harvest_exposed_public`), carry it to the Court as
    part of the great objection (`sq.harvest_to_the_court`, `act5.the_great_objection`), or pass it quietly
    hand-to-cracking-hand through the garrisons and Choir-houses (`sq.harvest_to_the_hands`).
- 🧵 **Theme (civic)**: the cruelest process in the world is built box by box of ordinary hands, its whole
  defense is "we had no choice," and it dies not to a rebellion it can crush but to the spreading, unkillable
  proof — a soldier and a singer in one room — that *every hand in it could always have opened.*
- 📈 **Content**: dialogue corpus now **192 conversations · 1,283 nodes · 161 skill checks** (730 choices).
  The side-quest suite now spans a payoff (Graves), the loop spine (Margins), two companion arcs (Roen,
  Naeve), and a faction convergence (Hands). All 606 headless CI checks green; the 40-file `.dlg` corpus
  compiles clean.

---

## 👑 v4.43.0 — *"The Indictment"* — Naeve's companion quest (Pillar 2 · side quests)

> The fourth `sq.*` payoff and second companion quest: Naeve's case against Netheril — the proof that the
> apocalypse was a murder with her father's signature on it, built across the whole road as a wall against a
> grief too big to file.

- ✨ **Side Quest: "The Indictment"** (`naeve_sidequest.dlg`, gated on `sq.naeve_indictment`) — the case is
  *complete*, and with nothing left to build, Naeve must face what the wall of paper was hiding: that she
  loved her father and he killed them all, and both are true. Four ways in:
  - the **wall** finished, with nowhere left to hide from the grief (`sq.naeve_wall_comes_down`);
  - the **archive**, not the case — keep all of Aubrey, un-reduced, refusing to flatten her father into his
    worst vote the way the Wall flattens the Faithless (`sq.naeve_archive_not_case`);
  - the **beautiful** — the documents hold the guilt, *she* holds the stars and the boat; the daughter is the
    column no record has (`sq.naeve_holds_the_beautiful`);
  - or the **Court** — read the indictment where it lands, making Aubrey's signature damn every power that
    ever chose prestige over a child (`sq.naeve_indictment_at_court`, `act5.the_great_objection`).
  - Resolutions: set the case down to finally *miss* him (`sq.naeve_grieves_at_last`), read the whole man aloud
    once (`sq.naeve_read_together`), keep one page of stars and boats while the rest goes to do justice
    (`sq.naeve_keeps_one_page`), or seal back behind the prosecution (`deflect`).
- 🧵 **Theme**: the opposite of being flattened into your worst day is not *acquittal* but *being remembered
  whole* — and a daughter who can hold the signature and the stars in one unbroken grief is a truer archive
  than any case the record could build.
- 📈 **Content**: dialogue corpus now **191 conversations · 1,272 nodes · 160 skill checks** (723 choices).
  Side-quest set now spans a payoff quest (the Graves), the loop-mystery spine (the Margins), and two
  companion arcs (Roen, Naeve). All 606 headless CI checks green; the 39-file `.dlg` corpus compiles clean.

---

## 👑 v4.42.0 — *"The Hand in the Margins"* — the loop-mystery quest (Pillar 1 · the spine)

> The plot-central `sq.*` payoff: the warning in Karsus's margins, followed to its end. The one side quest
> that advances the saga's *central* mystery — who the Returned is, what the loop is, and why the antidote to
> the crown is written in your own ten-thousand-year-old hand.

- 📜 **Side Quest: "The Hand in the Margins"** (`karsus_sidequest.dlg`, gated on `sq.karsus_margins` /
  `act3.returned_annotator`) — reading Proctor Ynn's folios in sequence reveals the second hand is *yours*,
  older than you, writing "stop" into catastrophes you never witnessed. The revelation: it's not a prophecy
  but a **letter chain** — each loop's Returned writing forward to the next, the warnings copied by scribes
  for ten thousand years as an unkillable correspondence addressed to the only guaranteed reader: *you.*
  - Read it as a letter chain — you're not the annotator condemned to write, but the *recipient* at last
    (`sq.margins_letter_chain`, `act5.loop_can_break`);
  - read the antidote — "you do not need to be a god to be enough" was always aimed past Karsus at *your* moment
    before the crown (`sq.margins_the_antidote`, `act3.you_do_not_need_to_be_a_god`, `act5.crown_is_fear`);
  - read the tiredness — the loop is a *sentence of witnessing*, but the weariness *refined* the warning rather
    than wasting it (`sq.margins_the_sentence`);
  - or **write back** — answer your own ancient hand, and the correspondence stops being one-way
    (`sq.wrote_back_to_the_loop`, `act5.last_returned_freed`).
  - Resolutions carry it forward as a *warning heeded* (`sq.margins_warning_heeded`), back to vindicate Ynn
    (`npc.ynn_vindicated`), or to the **warden** at the Court — the loop's tiredest hand, who thinks the letters
    abandoned him and needs to learn he was *the draft that made the final warning work*
    (`sq.margins_for_the_warden`, `act5.last_returned_freed`).
- 🧵 **Theme (the spine)**: the loop breaks not with a stronger Returned but a *better-warned* one — the whole
  chain of tired warnings existed to produce one reader who'd arrive at the Court already carrying the antidote
  in their own hand, and simply *not reach back* when the crown whispers.
- 📈 **Content**: dialogue corpus now **190 conversations · 1,260 nodes · 159 skill checks** (716 choices).
  All 606 headless CI checks green; the 38-file `.dlg` corpus compiles clean and the viewer regenerates
  byte-for-byte.

---

## 👑 v4.41.0 — *"The Thing in the Dark"* — Roen's companion quest (Pillar 2 · side quests)

> The second `sq.*` payoff: the companion Roen's buried secret, set up by Old Sabira at the Low Lantern. A
> single terrible night, wrapped so long in protective silence it curdled into a whole identity — and the cure
> is the saga's own.

- 🗡️ **Side Quest: "The Thing in the Dark"** (`roen_sidequest.dlg`, gated on `sq.roen_secret`) — Roen confronts
  the killing at fourteen: a job the fence Sabira sent a child on, a grown man who caught and wouldn't let go,
  a death in the dark. Roen has carried it for half a life not as *"a terrible night"* but as *"the real name
  under all the others."* Four ways in:
  - the **silence** that fed it (`sq.roen_named_in_daylight`) — a thing you can't say grows in the dark;
  - **Sabira's** buried fault (`sq.roen_knows_sabira`) — she sent the child and let him carry her sin;
  - the **whole** truth Roen edited out to make the guilt cleaner (`sq.roen_whole_truth`) — a cornered child
    *surviving*, not a killer;
  - and the **Wall** parallel (`act5.shame_feeds_the_wall`, `act5.wall_breaking`) — Roen has been their own
    Wall, filing themself under one act.
  - Resolutions: forgive Sabira as a weight *set down* not earned (`sq.roen_forgives_sabira`,
    `act3.forgiveness_is_not_earned`); name it at the fire as a fact refused the shame (`sq.roen_tells_the_fire`);
    go back to unlock both silences with Sabira (`sq.roen_and_sabira_reconcile`); or simply set it down for the
    night, refused the demand to recover on schedule (`sq.roen_darkthing_rests`).
- 🧵 **Theme**: the saga's whole argument proven on a companion's heart — *the Wall's cruelty and a person's
  shame run on the same engine* (one act, wrapped in silence, made a whole identity), and both break the same
  way: said out loud, in daylight, refused the power of being unspeakable.
- 📈 **Content**: dialogue corpus now **189 conversations · 1,249 nodes · 158 skill checks** (709 choices).
  All 606 headless CI checks green; the 37-file `.dlg` corpus compiles clean and the viewer regenerates
  byte-for-byte.

---

## 👑 v4.40.0 — *"The Graves That Waited"* — the payoff side quest (Pillar 2 · side quests begin)

> With the cast complete, the seeded `sq.*` hooks begin paying off as full quests. The first is the one that
> braids every carried name together — and gives the whole saga its true, unglamorous ending.

- ⚰️ **Side Quest: "The Graves That Waited"** (`court_sidequests.dlg`) — the payoff of the coffin-salesman's
  ledger of empty graves (gated on `sq.coffin_ledger` / `act5.graves_waiting`). Beyond the Court lies a field
  of *open graves* — every hole the salesman dug across ten thousand years for a soul the harvest stole before
  he could bury them, each kept open, each with a name on the stone. As the Wall reads its names free, the
  graves can finally be *filled* — one at a time, a true word over each:
  - **Halen** the potter, laid to rest with the green-glazed cup that outlived the city that filed him
    (`sq.halen_at_rest`);
  - **Maerys**, the first Faithless, buried at last — *overturning the precedent* that began the Wall, breaking
    it with a funeral instead of a crown (`sq.maerys_at_rest`, `act5.precedent_overturned`);
  - **Sariel** the weaver, whose burial lets the drowned choir *stop singing* after nine years and rest too
    (`sq.sariel_at_rest`, `act5.choir_can_rest`);
  - and **the salesman himself**, who discovers some earlier coffin-man dug *his* grave too — even he is in the
    unbroken line (`sq.salesman_has_a_grave`).
  - Resolutions: stay and fill the whole field by hand (`sq.graves_that_waited_complete`), summon the eternal
    line of grave-tenders to fill it together (`sq.graves_to_the_tenders`), or carry the ledger back through the
    Wall so *no freed soul is released into nowhere* — each one expected, with a grave already dug
    (`act5.every_soul_expected`).
- 🧵 **Theme (the ending under the ending)**: the Wall is not unmade by a crown or a breaking or a speech, but
  by the slow, holy, unglamorous work of giving the discarded what they were always owed — a grave that fits,
  a name spoken plain, and a place that was waiting all along. *Cruelty needs a machine; mercy just needs
  enough hands that decided to care.*
- 📈 **Content**: dialogue corpus now **188 conversations · 1,236 nodes · 157 skill checks** (701 choices).
  All 606 headless CI checks green; the 36-file `.dlg` corpus compiles clean and the viewer regenerates
  byte-for-byte.

---

## 👑 v4.39.0 — *"The Masks Ate the Face"* — the Guild's shadows; the roster complete (Pillar 2 · ✅ 104/104)

> The final cluster closes the entire designed roster. Four underworld figures — each a moral mirror the
> saga has been polishing all along — and with them, **every one of the 104 designed NPCs (#1–104) is fully
> voiced**, across six acts and ten thousand years.

- 🗝️ **The Guild's Shadows** (`guild_denizens.dlg`):
  - **Quietknife Senna** — a poisoner with a strict code built atop one grave: the innocent merchant she
    killed believing a liar's story. Her obsessive verification is the Wall's sharpest mirror — *the
    difference between justice and murder is whether someone checked the story*, and a register of the
    condemned trusted without verification is a liar's blade scaled to a cosmos (`act2.cataloguing_defeats_the_wall`).
  - **Gentle Hands Bevin** — forges only *kind* documents (manumissions, pardons, the dead's last loving
    words), routing around a cruel law's fences; can't forge the one pardon he needs for himself. The turn: a
    forged manumission is *more honest* than the law that enslaved — he's not faking truth, he's *recording the
    justice the world refused to sign* — and the Wall, having no author to fool, needs not his craft but a
    voice to say the gate was never guarded (`act2.wall_has_no_author`).
  - **The Memory-Thief** — lifts painful memories as a mercy and must carry every one, a museum of borrowed
    agonies that can never be set down. The Wall's exact inverse: a keeping that *remembers* rather than
    erases — and the gentler mercy is to make a memory *bearable*, witnessed and shared, not amputated
    (`act5.witness_then_release`).
  - **The Spymaster** — wore so many loyalties so fully she wore away the one beneath; the loop's warning in
    another key. The thread back is never a faction but a *person* (her sister Lira, the reason the masquerade
    began), and survival means walling off one true room and refusing, whatever it costs, to play what's
    inside (`act4.meaning_lives_in_memory`).
- 🏆 **✅ ROSTER COMPLETE — 104/104.** Every NPC the project designed — the Gilded Gate, Candlekeep, the Fugue
  Plane, Netheril, the Crown Wars, the Time of Troubles, the Spellplague, the Court of the Dead, the
  connective-tissue threads, and the Lower City's taverns, harvest quarters, and Guild shadows — is now fully
  voiced. 104 unique souls, each a name and one true thing, comedy over grief, **not a single generic.**
- 📈 **Content**: dialogue corpus now **187 conversations · 1,220 nodes · 157 skill checks** (up from
  183/1,188/153). All 606 headless CI checks green; the 35-file `.dlg` corpus compiles clean (zero broken
  refs, dup ids, or orphans) and the viewer regenerates byte-for-byte.

---

## 👑 v4.38.0 — *"The Hand That Could Have Opened"* — 100 named NPCs (Pillar 2 · the milestone)

> **The north star is reached: one hundred named, fully-voiced, individually interactable NPCs**, each a name
> and one true thing, comedy over grief — the BG2-scale cast the project set out to write. This release
> finishes the harvest quarter with its two hardest voices: the ones who *did* the harvest, and turned.

- 🕯️ **Harvest-Quarter Survivors, Wave II** (`harvest_denizens_2.dlg`) — the perpetrators who broke:
  - **Sister Wren** — a gutter-child who joined the Choir at sixteen for a warm room and a family, and sang
    the song that marked the door of old Mira, who once gave her a candle when she was cold. The turn: she
    never had a *faith*, only a *need* — the doctrine was the toll she paid for belonging, written in other
    people's doors — and the harvest dies when its *hands* refuse to mark (`sq.choir_hands_refuse`,
    `act5.refusing_consent`, reactive `faction.choir`).
  - **Corwin, the Deserter** — fifteen years a Flaming Fist soldier, four villages burned under orders, who
    put down the torch at the fifth when a girl looked at him with disappointment instead of fear. "I only
    followed orders" is the wall every hand builds to sleep; the order was always nothing without a hand to
    carry it — and the hand could always have opened. He can turn desertion into *testimony*
    (`sq.deserter_testimony`, `act5.shame_feeds_the_wall`, reactive `faction.fist`).
- 🏆 **Milestone — 100 named NPCs.** With the harvest quarter complete (and Quill-and-Ledger Mas from the Act I
  deepening), the roster reaches **100 unique, fully-voiced denizens** spanning all six acts and ten thousand
  years: the Gilded Gate, Candlekeep, the Fugue Plane, Netheril, the Crown Wars, the Time of Troubles, the
  Spellplague, the Court of the Dead, the connective-tissue threads that recur across every era, and the
  Lower City's taverns and harvest quarters. Every one is a name and one true thing; not a single generic.
- 🧵 **Theme**: the harvest's hardest truth — its cruelest work is done by the lonely (who swallow a doctrine
  as the price of family) and the dutiful (who hide behind "orders") — and both can be turned by the same
  recognition: *the hand always had a choice.*
- 📈 **Content**: dialogue corpus now **183 conversations · 1,188 nodes · 153 skill checks** (up from
  181/1,172/151). All 606 headless CI checks green; the 34-file `.dlg` corpus compiles clean (zero broken
  refs, dup ids, or orphans) and the viewer regenerates byte-for-byte.

---

## 👑 v4.37.0 — *"What the Harvest Left"* — the harvest-quarter survivors, Wave I (Pillar 2 · the wider city)

> The quarter Act I's harvest emptied — the grief the atrocity left behind, in three voices. A mother, a
> silent child, and a healer undone by the first wound her hands can't reach.

- 🕯️ **Harvest-Quarter Survivors, Wave I** (`harvest_denizens.dlg`):
  - **Goodwife Orla** — selling her carver son Davin's tools, not for coin but so his craft lives on in
    working hands rather than rusting in a shrine; the harvest burned his name, but it can't stop his chisels
    carving cradles in strangers' hands for fifty years. She keeps back one knife, worn to the shape of his
    grip — the one part the harvest couldn't take (`sq.davins_tools`).
  - **The Silent Vane Boy** — ten years old, mute since the harvest took his mother in front of him,
    communicating only in charcoal drawings of the hooded harvesters. The interaction refuses to demand his
    voice back: you read his pictures, honor the speech that *failed* him when he screamed a warning that
    changed nothing, or carry his drawings out as *evidence* — a child's testimony louder than words
    (`sq.boys_drawings`).
  - **Sister Maud** — the Lower City's finest healer, stopped cold by the harvest's survivors: wounds that
    leave no mark, grief no poultice reaches. The turn: she confused *healing* with *curing* — the man staring
    at his wife's cloak doesn't want the grief cured (that would be a second bereavement); he needs someone to
    *sit in the room and not flee the unfixable.* Presence is the medicine that never runs out
    (`act4.faith_through_doubt`).
- 🧵 **Theme**: the harvest's true cost isn't the taken but the *left* — and the saga's answer at survivor
  scale: craft passed on outlives the burning of a name; the grieving are *heard* in whatever language loss
  left them; and the deepest care is the staying, not the fixing. Roster now **97 named denizens.**
- 📈 **Content**: dialogue corpus now **181 conversations · 1,172 nodes · 151 skill checks** (up from
  178/1,149/147). All 606 headless CI checks green; the 33-file `.dlg` corpus compiles clean (zero broken
  refs, dup ids, or orphans) and the viewer regenerates byte-for-byte.

---

## 👑 v4.36.0 — *"The Keeper of Debts"* — the Low Lantern, complete (Pillar 2 · the wider city)

> Three more regulars finish the Low Lantern's roster — the woman who holds the whole quarter's accounts, the
> drunk who reasoned too far, and the fence who raised a companion.

- 🍺 **The Low Lantern, Wave II** (`lowlantern_denizens_2.dlg`):
  - **Tess** — the barmaid who carries the quarter's debts of coin *and* conscience in her head, judging
    nothing, forgetting nothing: the informal court of the poor, where the wronged don't disappear. The turn,
    keeper to keeper: a debt remembered *so it can be settled* is mercy; a debt held *so it never can* is a
    cage in a ledger's clothes — the whole question the Wall dodges (`act4.deathless_is_the_wall`,
    `act2.cataloguing_defeats_the_wall`). Her own great debt — a broken promise — she keeps uncollected.
  - **Doddering Phinneas** — the quarter's joke, a real philosopher who followed a thought to the floor of the
    abyss (*the universe grants no meaning*) and has drunk eleven years to blur its edge. He stopped one stair
    short of the rescue: if meaning isn't *given* it's *made* — un-foreclosable, because a silent universe
    that grants nothing also revokes nothing. The terror was never the silence; it was facing it *alone*
    (`act4.meaning_lives_in_memory`, `act4.meaning_without_reward`).
  - **Old Sabira** — the fence who raised the companion **Roen** off the street and buried the secret that
    could unmake them both: a desperate, terrible thing Roen did at fourteen — which was *Sabira's* sin
    dressed as the child's. Ties directly to Roen's arc: the Wall destroys not by *exposing* a past but by
    making a single act curdle into a whole name, and the cure is to *name it in daylight and refuse it the
    power of shame* (`sq.roen_secret`, `act5.shame_feeds_the_wall`).
- 🧵 **Theme**: the tavern as the saga's whole argument in miniature — debts that should settle, meaning made
  by hand, shame defeated by daylight. The Low Lantern (#89–94) is **fully voiced**; roster now **94 named
  denizens.**
- 📈 **Content**: dialogue corpus now **178 conversations · 1,149 nodes · 147 skill checks** (up from
  175/1,125/144). All 606 headless CI checks green; the 32-file `.dlg` corpus compiles clean (zero broken
  refs, dup ids, or orphans) and the viewer regenerates byte-for-byte.

---

## 👑 v4.35.0 — *"The Candle and the Tide"* — the Low Lantern fills (Pillar 2 · the wider city)

> The principal saga is voiced end to end; now the Lower City taverns fill with the named faces that give a
> place its lived-in soul — BG2's Copper Coronet, the Gate's way. Three regulars of the Low Lantern, each a
> warm comic surface over a specific private grief.

- 🍺 **The Low Lantern, Wave I** (`lowlantern_denizens.dlg`):
  - **Sweet Anselm** — the bard who wrote "The Candle and the Tide," the most famous love song of the age,
    for a candle-girl named Mira who married a shipwright; now paid nightly to perform the exact measure of
    his own heartbreak at strangers' weddings. The turn: the house and the marriage she chose are dust, and
    the song she rejected outlived them all — he didn't lose, he *won the wrong prize* (`act4.deathless_is_the_wall`
    reprised as consolation).
  - **Deal-Me-In Dax** — a card sharp who cheats *every* hand, to deal the drowning a winner and rob the
    smug; a thumb on the scale of an uncaring luck. Under it: the one honest game that cost him a farm and a
    girl, and a vow never to let chance decide anything that matters again. Can be set on Velith, the Gate's
    unluckiest man — not with a rigged win, but with freedom from luck (`sq.dax_and_velith`).
  - **Old Halloran** — the last survivor of the *Grey Gull*, plotting a course to a phantom island that was
    never real, because while the voyage is unfinished his drowned crew isn't *drowned* — they're still
    underway. A map to nowhere that is really a map to the people he lost; the hard mercy is letting the
    voyage *land* (`act4.deathless_is_the_wall`, the saga's theme at tavern-table scale).
- 🧵 **Theme**: the wider city as the saga in miniature — even the tavern regulars carry the one true thing,
  comedy over grief, each refusing in their own small way to let a lost thing finish (and sometimes learning
  that the deeper love is the letting-go). Roster now **91 named, fully-voiced denizens.**
- 📈 **Content**: dialogue corpus now **175 conversations · 1,125 nodes · 144 skill checks** (up from
  172/1,101/141). All 606 headless CI checks green; the 31-file `.dlg` corpus compiles clean (zero broken
  refs, dup ids, or orphans) and the viewer regenerates byte-for-byte.

---

## 👑 v4.34.0 — *"One Soul, Saving Ourselves"* — the connective tissue, complete (Pillar 2 · the weave)

> The last three recurring threads close the weave — and the loop turns from a closed circle into a chain
> that can talk. A grave-tender doing the companion Garrow's exact work in an age before he was born; a
> serious child reinventing Lula's list without ever having heard of her; and the Returned of prior loops,
> each a step further toward the warden, each leaving a clearer warning for the self behind.

- 🧵 **Connective Tissue Wave II** (`connective_denizens_2.dlg`) — the eternal vocations and the loop's chain:
  - **The Grave-Tender** — a stranger doing Garrow's exact work (washing, digging, naming the unclaimed dead)
    in every era, one link in a ten-thousand-year unbroken line. A message for the companion Garrow: *the job
    is not yours to finish, only to carry well and pass on; you were never alone in the long way*
    (`act5.unbroken_line`, `act5.garrow_not_alone`; her own graves dug for a sister named Maerin).
  - **The Child With the List** — a serious child keeping a list of the forgotten, who invented Lula's idea
    without being told, and learns the remembering *regenerates spontaneously* in every age and can never be
    killed: "the pond is not allowed to close up smooth" (`act5.remembering_regenerates`, `sq.childs_list`).
  - **The Returned Before You** — a prior loop's self, worn and hardening but *not yet* the warden, soft
    enough still to warn the earlier you. The revelation that reframes the whole loop: it's not a closed
    *circle* but a *chain that can talk*, every self passing a clearer warning down — the annotator, the
    Revenant's map, all *one soul trying to save itself*; and the leap that finally breaks it pulls every
    hesitating link through at once, backward and forward (`act5.loop_can_break`, `act5.knows_the_trap`,
    `act5.last_returned_freed`). The trap named precisely: not the crown, but the small cold voice that says
    *"one more loop, to be sure"* — because the leap can never be made from certainty, only faith.
- 🧵 **Theme**: the saga's two deepest consolations, made explicit — that the work of caring for the
  discarded is an unbreakable relay older than any one heart, and that the loop is redeemable as a *we* rather
  than endured as an *I*. **The full named roster #1–88 — every act, every era, every recurring thread — is
  now fully voiced.**
- 📈 **Content**: dialogue corpus now **172 conversations · 1,101 nodes · 141 skill checks** (up from
  169/1,079/138). All 606 headless CI checks green; the 30-file `.dlg` corpus compiles clean (zero broken
  refs, dup ids, or orphans) and the viewer regenerates byte-for-byte.

---

## 👑 v4.33.0 — *"The Same Dog"* — the connective tissue, Wave I (Pillar 2 · the weave)

> The recurring souls who appear in *every* era, ten thousand years and four fallen worlds apart, alarmingly
> the same — the threads that stitch the whole saga into one cloth. A coffin-salesman whose pre-need plans
> are a secret rebellion, a man who complains to the afterlife's empty desk because complaint is the last
> dignity, and a grey dog that has never stopped hearing the song of the kept.

- 🧵 **Connective Tissue Wave I** (`connective_denizens.dlg`) — the "anywhere" NPCs:
  - **The Travelling Coffin-Salesman** — the same tidy man in every era, selling endings to a cosmos that
    walls its dead; a *protest with a sample-case.* He keeps a ledger of empty graves — every soul he sold a
    grave who got the Wall instead, *Halen's hole still open after ten thousand years* — and a grave waiting,
    with your name on the stone (`sq.coffin_ledger`, `act5.graves_waiting`, `act5.grave_for_the_returned`).
  - **The Complaint** — the fussy, persistent man lodging formal grievances at a never-staffed desk, who is
    really a *resistance shaped like a customer complaint*: "accept it" is the mortar of the Wall, and he has
    spent an age refusing to sign off. Under the thousand petty objections is the one too big to write — his
    daughter, walled at eleven for the crime of thinking (`sq.the_complaints`, `act5.ask_them_why`,
    `act5.refusing_consent`).
  - **The Same Dog** — a grey dog with a notched ear, the same one across every era, the one creature that
    never learned the doctrine and so never stopped hearing the Wall's song. The un-teachable mourner that has
    spent ten thousand years howling back at the kept and *waiting to be followed* — it leads to the loudest
    part of the song (`act5.dog_leads_to_wall`, `act5.unlearn_the_doctrine`).
- 🧵 **Theme**: the saga as one woven cloth — the same jobs done in every age (the grave-promiser, the
  objector, the mourner), proof that the impulse to remember, to refuse, to grieve the forgotten regenerates
  spontaneously in every era and can never be fully stamped out.
- 📈 **Content**: dialogue corpus now **169 conversations · 1,079 nodes · 138 skill checks** (up from
  166/1,056/135). All 606 headless CI checks green; the 29-file `.dlg` corpus compiles clean (zero broken
  refs, dup ids, or orphans) and the viewer regenerates byte-for-byte.

---

## 👑 v4.32.0 — *"The Book That Grieves"* — the Court of the Dead, complete (Pillars 1+2)

> The summit's last two voices: the true believer who must be turned, and the book that has held every
> forgotten name for ten thousand years. With them, the saga's principal cast (#1–82) is fully voiced.

- ⚖️ **Court of the Dead Wave II** (`court_denizens_2.dlg`) completes the summit:
  - **Vayle, Doomguide of the Final Watch** — your adversary since the Sword Coast, who has chased you across
    an age to stand with the church's last army between you and the Wall. Not cruel — *faithful*, and she has
    *wept* for every soul she ever consigned. She turns on the truth that the Wall is an *orphaned machine*
    (Jergal disowned it), on the contradiction of a *rest-priest guarding the graveless*, on permission to
    *believe her own swallowed tears*, or on the memory of the *first mercy at the harvest* in Act I — the
    seed that took an age to admit was a seed (`act5.vayle_redeemed`, reactive to your earliest choice).
  - **The Ledger** — ten thousand years of the discarded bound in one book, made into a layered voice of
    millions: the jailer that is *also* the only archive, the instrument of erasure that *secretly kept every
    name it was forced to damn.* The saga's deepest turn: the opposite of imprisonment was never *destruction*
    but *being read* — a roster read aloud with love becomes, name by name, the roll-call of the freed. Burn
    it and the souls go free but *erased*; read it and the ledger-of-the-damned becomes the *book that grieves*
    (`act5.read_the_ledger`, `act5.ledger_becomes_memorial`, `act5.wall_breaking`).
- 🧵 **Theme (resolution)**: the most dangerous guardians of a cruelty are the *faithful* who weep as they
  enforce it — and they need only permission to believe their own grief to become its fiercest undoers; and
  the answer to a Wall of the forgotten is not the flame but the *reading.* **The principal cast — every named
  denizen #1–82 across all six acts — is now fully voiced.**
- 📈 **Content**: dialogue corpus now **166 conversations · 1,056 nodes · 135 skill checks** (up from
  164/1,040/133). All 606 headless CI checks green; the 28-file `.dlg` corpus compiles clean (zero broken
  refs, dup ids, or orphans) and the viewer regenerates byte-for-byte.

---

## 👑 v4.31.0 — *"Read Me By Name"* — Act V opens: the Court of the Dead, the summit (Pillars 1+2)

> The road reaches its end, and every thread it wove becomes a single choice. The god who *already* did what
> you came to do — invented the Wall, then abdicated his own godhood in eons-long penance. Your own future
> self, crowned and trapped, secretly begging you to do the brave thing he couldn't. And the first soul you
> choose to read aloud — where the whole saga collapses to its smallest, unbreakable act.

- ⚖️ **Act V opens at the Court of the Dead** (`court_denizens.dlg`) — the summit, where the seeded flags of
  the entire journey converge:
  - **Jergal, the First Scribe** — the original death-god, who *invented* the filing and the "nothing," then
    abdicated his divinity and made himself the eternal clerk of his own sin, waiting eons for the one who
    could finish what he began. He confirms the keystone — *the Wall has no author who still believes in it* —
    and guards the keyhole of the **golden road**, which leads not to a kinder Wall but to *the fall, the
    ending, the letting-go.* Can be released among the Faithless himself, the oldest prisoner of his own
    cruelty (`act5.jergal_gives_key`, `act5.golden_road_known`, `act5.release_jergal`).
  - **The Last Returned** — *you*, aged by certainty, crowned in cold grave-fire, the most imprisoned soul in
    the Court. He took the crown out of *fear for the souls he came to save*, became the warden, and now
    recruits each next loop into the same choice — while secretly *begging* to be surprised. The crown's true
    temptation laid bare: not power, but *the promise that wearing it, you'll stop being afraid for them.* The
    loop breaks on the Exile's secret — forgiveness set down, not earned (`act5.loop_can_break`,
    `act5.last_returned_freed`, `act5.crown_is_fear`).
  - **The Soul You Judge First** — *Halen, a potter*, half-mortared in the Wall, looked at directly for the
    first time in eons. The whole war reduces to one act: the Wall keeps souls not by force but by making sure
    *no one reads them* until they *agree* they're nothing — and a single name, read aloud and *meant*, is the
    one thing it was never built to survive. Witness, then *release* — read him real, then let him *fall*
    (`act5.read_first_name`, `act5.witness_then_release`, `act5.halen_read`).
- 🧵 **Theme (payoff)**: every seeded thread lands at once — *the Wall has no author* (Jergal), *the crown is
  fear, not the answer* (the Last Returned), *cataloguing/naming defeats the Wall* and *deathlessness is the
  cruelty* (Halen). The summit's argument: you defeat ten thousand years of patient cruelty not with a sword
  or a crown but by *reading one forgotten soul by name and then letting it go.*
- 📈 **Content**: dialogue corpus now **164 conversations · 1,040 nodes · 133 skill checks** (up from
  161/1,017/130). All 606 headless CI checks green; the 27-file `.dlg` corpus compiles clean (zero broken
  refs, dup ids, or orphans) and the viewer regenerates byte-for-byte.

---

## 👑 v4.30.0 — *"In Order, In Order"* — the Spellplague complete; the loop sharpens (Pillar 1/2)

> The blue fire finishes its work on reality — and on memory. A little clerk fights to keep cause-and-effect
> from dissolving into noise, because without it nothing anyone does means anything; and a woman holds a hole
> in her own mind where a murder used to be — a blank that wasn't chaos but *editing*, and the killer she
> half-remembers wore a face she knew and had never seen, out of its own time.

- 🌀 **Spellplague Wave II** (`spellplague_denizens_2.dlg`) completes Act IV's reality-break era:
  - **The Clerk of Causality** — the bureaucrat of meaning itself, shouting "in order, in order" at a
    hurricane as effects precede their causes, terrified that if cause no longer yields effect then no deed,
    no kindness, no *choice* counts. He can be shown that meaning relocates from *time* (lost) into *memory*
    (endures) — a "second causality" enforced by witnesses who carry deeds forward; or that meaning was never
    the same as reward, and a deed done with no guarantee is the purest kind (`act4.meaning_lives_in_memory`,
    `act4.meaning_without_reward`).
  - **The Witness** — who saw a murder "that mattered beyond the two of them," then had the seeing surgically
    erased by the blue fire while everything else was left. Not the plague's *mercy* but a *cover-up* she's
    hosting in her own skull: the killer turned, looked at her, with a face she knew and had never seen — a
    *loop*-murder, deleted to protect the killer's secret. Threads directly into the saga's central mystery
    (`act4.deleted_witness`, `act3.loop_glimpsed`, `sq.reconstruct_the_murder`).
- 🧵 **Theme / plot**: the Spellplague closes by sharpening the loop — the Last Returned, the annotator's
  hand, the witness who saw one version of you kill another. And the saga's spine gets its sharpest statement
  yet: when the rule that made deeds matter *automatically* breaks, meaning doesn't die — it has to be *made
  by hand*, one chosen kindness at a time, and *kept by memory.* The Spellplague (#73–77) is now **fully
  voiced**, completing **Act IV.**
- 📈 **Content**: dialogue corpus now **161 conversations · 1,017 nodes · 130 skill checks** (up from
  159/1,001/128). All 606 headless CI checks green; the 26-file `.dlg` corpus compiles clean (zero broken
  refs, dup ids, or orphans) and the viewer regenerates byte-for-byte.

---

## 👑 v4.29.0 — *"The World That Won't Hold Still"* — the Spellplague opens; the corpus passes 1,000 nodes (Pillar 2)

> Reality stops holding still. A cartographer maps a geography that dissolves as she draws it, keeping a road
> to her daughter open on paper because it's the only place it still exists; two strangers welded into one
> body by the blue fire have built a love so complete each would die for the other; and an old gardener tends
> flowers that cannot die — and grieves them — because deathlessness, he's learned, is just the Wall by
> another name. The first plain statement of the choice waiting at the end of the road.

- 🌀 **The Spellplague opens** (`spellplague_denizens.dlg`) — the reality-breaking era:
  - **The Cartographer** draws the world-as-it-*was* against a world that won't hold still, her plaguechanged
    ink weeping but accurate — a memorial to lost coastlines and the town of *Sevenfields*, swapped intact to
    another world with her estranged daughter inside it, behind a wall of blue fire she'll never cross
    (`act4.sevenfields_kept`, `sq.the_old_map`).
  - **The Two Who Are One** — a cooper and a midwife merged by the fire into a *third* person neither was
    alone; well-adjusted, in love, each having secretly decided to be the one who dies if they're ever
    separated. The cruelest thing you can offer them is the "cure" that would make them whole and singular and
    *alone* (`act4.the_we_is_real`).
  - **The Blue-Fire Gardener** — tends a paradise of deathless flora (fruit that can't fall, blossoms frozen
    at the bud) and has learned the lesson the whole road circles toward: *to keep a thing from ending is not
    to love it but to cage it.* The Wall and the deathless garden are the same crime, and the only mercy for
    the kept is the one the crown will beg you not to give — *an ending* (`act4.deathless_is_the_wall`,
    `act4.gardener_warning`, reinforces `act3.break_the_crown_known`).
- 🧵 **Theme**: the Spellplague as the mirror of the Wall — a world that *severs* and *fuses* and *freezes*,
  teaching by reality-break what the saga has argued all along: that preservation against ending is the
  cruelty, and the fall is the mercy. The gardener is the plainest pre-statement of the endgame choice.
- 📈 **Content**: dialogue corpus now **159 conversations · 1,001 nodes · 128 skill checks** (up from
  156/977/125) — **past a thousand nodes.** All 606 headless CI checks green; the 25-file `.dlg` corpus
  compiles clean (zero broken refs, dup ids, or orphans) and the viewer regenerates byte-for-byte.

---

## 👑 v4.28.0 — *"The Post and the Altar"* — the Time of Troubles, complete (Pillar 2)

> Two souls holding the line on the night the gods went mad — a watchman who files an incident report on a
> god-war because procedure is the last edge the world has left, and the last worshipper of a small dying
> god, who kneels with his back to the death and *will not look*, because a thing is only dead when the last
> believer agrees.

- 🩸 **Time of Troubles Wave II** (`troubles_denizens_2.dlg`) completes Act IV's god-fall era:
  - **Watchman Harn** — a city guardsman taking statements on "two gods conducting a duel above the fish
    market," because if he stops filing then there are no rules and nothing stands between him and the wrong
    sky. The clipboard as rosary; an explicit echo of Tobias (#9) — *the filing is the dignity, the post is
    the thing, not the paper* (`act4.order_as_prayer`, `act4.held_the_post`, `sq.first_incident_report`).
  - **The Last Worshipper** — kneeling at the altar of *Aubrin, the Threshold-Keeper*, a small god dying
    twenty feet behind him, refusing to turn because to *look* is to *confirm* the death and push him into the
    dark. Not denial — the most stubborn love in the saga: the believer who decides never to look. He can be
    moved to *share the watch* (carry the god's name out as a second wall), or to turn at last and walk his
    threshold-god through its final threshold *accompanied* (`act4.faith_as_not_looking`,
    `act4.aubrin_remembered`, `act4.crossed_together`).
- 🧵 **Theme**: how small people hold the world together when the big things go mad — by standing the post and
  writing it down true, and by refusing to *agree* that a loved thing is gone. Both reprise the saga's spine:
  a name kept, a record made, the Wall defeated by what nothing else will keep. The Time of Troubles
  (#67–72) is now **fully voiced**, and **Acts I–IV's named denizens (#1–72) are complete.**
- 📈 **Content**: dialogue corpus now **156 conversations · 977 nodes · 125 skill checks** (up from
  154/961/123). All 606 headless CI checks green; the 24-file `.dlg` corpus compiles clean (zero broken refs,
  dup ids, or orphans) and the viewer regenerates byte-for-byte.

---

## 👑 v4.27.0 — *"The Mask Comes Off"* — Act IV opens: the Time of Troubles, 1358 DR (Pillar 1/2)

> The gods walk Faerûn as mortals, cast down by Ao — and the Pale Companion who has carried your water for
> an entire act reaches up to touch a face that was never a face. The villain reveal, plus the strangest
> congregation of the saga: an amnesiac god at eye level with his worshippers, a smith forging a crown from
> a skull he insists is ivory, and a priestess losing and finding her faith by the hour.

- 🩸 **Act IV opens** (`troubles_denizens.dlg`) — the god-fall, told from inside:
  - **The Pale Companion** unmasks as **Myrkul, Lord of the Dead** — and reveals he rode your pack not to
    judge you but because, in a time of dying gods, even the God of Death went looking for the one soul who
    knows how to come *back.* He can be brought to *yield the crown grieving* rather than fight for it — "I
    would rather be mourned than be needed" (`act4.myrkul_revealed`, `act4.myrkul_will_yield`,
    `act4.myrkul_seeks_return`).
  - **The Lost Avatar** — a god knocked down to eye level and stripped of memory, who discovers the
    not-remembering feels less like amnesia than *relief*; the divinity was a weight he carried, not the
    heart underneath. He can choose to stay gloriously, humbly mortal (`act4.god_stays_mortal`,
    `act4.god_meets_eye_level`).
  - **The Bone-Forger** — an honest craftsman at the peak of his trade, forging the Crown of Horns from a
    material that files like bone and screams below hearing, who has *un-known* what his own hands keep
    telling him because it's the finest work of his life. The Crown's title, sourced (`act4.crown_being_forged`,
    `act4.forger_realizes`).
  - **Sister Aelana, the Doubter** — a priestess at a bleeding altar, alternating faith and collapse like a
    pulse, who learns the doubt was never the failure of her faith but its *exercise* — the only devotion
    built to survive the gods themselves falling into the street (`act4.faith_through_doubt`).
- 🧵 **Theme**: what a god *is* without worship — and the throughline of the Wall arrives at its enforcer-god,
  who turns out to be as weary of the office as the predecessor, the clerk, and the scribe before him. The
  Crown of Horns is named and sourced: a murdered god's skull, forged in pride, not malice.
- 📈 **Content**: dialogue corpus now **154 conversations · 961 nodes · 123 skill checks** (up from
  150/929/119). All 606 headless CI checks green; the 23-file `.dlg` corpus compiles clean (zero broken refs,
  dup ids, or orphans) and the viewer regenerates byte-for-byte.

---

## 👑 v4.26.0 — *"The Song, the Ledger, and the Hand That Stayed Down"* — Crown Wars complete (Pillar 2)

> The founding finishes. The bard who is commanded to make the atrocity *beautiful* — and so writes the
> first dirge in the world; the scribe whose stylus turns a man's spoken cruelty into a permanent fact and
> who insists "I only record"; and, on a hill above it all, the one judge who kept his hand down, exiled and
> erased, who has had ten thousand years to learn what forgiveness actually is.

- 🏛️ **Crown Wars Wave II** (`crownwars_denizens_2.dlg`) completes the origin era:
  - **Singer Lhoris** — commissioned to score the faithless's last walk, and able to make ten thousand years
    of imprisonment sound like a blessing. The artist's complicity: beauty is a gift to the dying *and* an
    anesthetic for the killers, in the same notes. He can weave the victims' *names* into the melody so every
    future dirge is a hidden act of remembrance, or smuggle the truth in its bones down ten thousand years
    (`sq.the_first_dirge`, `act3.truth_smuggled_in_song`).
  - **The Scribe of the First Verdict** — the unremarkable, dutiful woman whose clean hands are the *only*
    thing that makes the cruelty stick: the Wall isn't built of stone, it's built of her ledger, kept
    correctly. The ancestor of the Clerk of the Wall (#49), patient zero of "I only record" — who can leave a
    margin, name the first Faithless in the template, and make the record carry its own refutation forever
    (`act3.wall_is_the_ledger`, `act3.scribe_refuses_inertia`, `sq.scribe_margin`).
  - **The Exile Who Forgave** — the one judge who refused the vote, stripped of name and rank and erased from
    the record, who has sat ten thousand years on a hill above the Wall and learned the secret the whole road
    ahead requires: *forgiveness is never earned, only set down* — a bridge you build alone and then wait on.
    The counterpoint to young Ilfaeril; he can come down off the hill to tell the boy there was another way
    (`act3.forgiveness_is_not_earned`, `act3.refusal_was_possible`, `act3.exile_may_return`).
- 🧵 **Theme**: how an atrocity is *finished* — scored into beauty, recorded into permanence, and written into
  a story that says "there was no other way." Each NPC is the seed of a present-day counterpart (Lhoris→the
  Drowned Choir, the Scribe→the Clerk of the Wall, the Exile→Ilfaeril's unaccepted forgiveness). The Crown
  Wars (#61–66) are now **fully voiced**.
- 📈 **Content**: dialogue corpus now **150 conversations · 929 nodes · 119 skill checks** (up from
  147/905/116). All 606 headless CI checks green; the 22-file `.dlg` corpus compiles clean (zero broken refs,
  dup ids, or orphans) and the viewer regenerates byte-for-byte.

---

## 👑 v4.25.0 — *"The Day the Wall Was Built"* — the Crown Wars origin (Pillar 2 keystone)

> The saga reaches its thematic floor: ~−10,000 DR, the founding chamber where the Wall of the Faithless
> is *invented* — not by a villain, but by a grieving, kind-eyed judge who calls it mercy; where the very
> first soul is sentenced before the word "Faithless" even has a meaning; and where a brilliant young elf
> named Ilfaeril raises his hand to vote yes, on the worst and most innocent day of a ten-thousand-year life.

- 🏛️ **The Crown Wars open** (`crownwars_denizens.dlg`) — the origin of the central evil, shown as a
  committee of good intentions:
  - **Coronal Aedyn** — the high judge who devised the Wall as *preservation* rather than annihilation, and
    genuinely cannot tell whether he's building a mercy or "the most patient cruelty in the history of the
    world." Confront him and the foundation shows: the Wall is a way to *never have to grieve* — filing
    cheaper than weeping — and *"a crown cannot choose mercy, because mercy is sometimes the refusal to rule"*
    (`act3.wall_born_of_mercy`, `act3.crown_cannot_grieve`, `act3.break_the_crown_known`).
  - **The First Faithless** — *Maerys*, a soldier who drowned pulling a bad singer named Calla out of a river,
    standing in the first dock before the word "Faithless" has been given its terrible meaning. She kept every
    faith that mattered — just not to a god — and you can make the Wall *fail at its very first stone* by being
    the one witness who remembers her name (`act3.first_faithless_remembered`, `act3.died_for_a_song`).
  - **Ilfaeril, the Younger** — your ancient, grief-hollowed companion as an *eighteen-year-old prodigy*,
    certain and clear-eyed and proud, an hour from casting his first vote. His airtight logic has one hole —
    he's been *forbidden to ask the defendant* — and you can send him to the dock, or tell him you've met his
    ten-thousand-year-older self still carrying the wound. The exact moment a companion's grief was born,
    happening live (`act3.ilfaeril_asks`, `act3.ilfaeril_hand_stays_down` / `act3.ilfaeril_raises_hand`).
- 🧵 **Theme (keystone)**: the Wall was not built by hatred but by *love exhausted past the strength to
  grieve*, and armored in a sincerity no later century dares question — "the hardest evil to end is the one
  done in love." The first Faithless was never faithless; the Wall was a lie from its first stone; and the
  crown at the road's end is the office, not the answer.
- 📈 **Content**: dialogue corpus now **147 conversations · 905 nodes · 116 skill checks** (up from
  144/881/112). All 606 headless CI checks green; the 21-file `.dlg` corpus compiles clean (zero broken refs,
  dup ids, or orphans) and the viewer regenerates byte-for-byte.

---

## 👑 v4.24.0 — *"The Hand in the Margins"* — Netheril complete + the loop's first thread (Pillar 1/2)

> The Seventh Enclave finishes falling — and the mystery of who *you* are takes its first cold shape. A
> fishwife's daughter decides whether the lady she built is worth living in; a proctor recognizes your
> handwriting in a warning older than your grandfather; a slave wins a nineteen-year legal war one hour
> too late; and Naeve, the last daughter of a murdered house, assembles the indictment of her own father.

- 🏛️ **Netheril Wave II** (`netheril_denizens_2.dlg`) completes Act III's first era:
  - **Lady Sabbath of the Seventh** — a fishwife's daughter who clawed up forty tiers and built a flawless
    aristocrat, now choosing whether to *die elegant* or *survive vulgar*; the truest elegance, it turns out,
    is outliving the audience that wanted you gone (`act3.sabbath_survives`).
  - **Proctor Ynn** — eleven years studying a second hand in the margins of Karsus's Folly, a warning the
    magelords *read and ignored* — and he knows *your* handwriting on sight. The first thread of the loop:
    someone with your exact tired script has been writing *"Stop. You do not need to be a god to be enough"*
    in the margins of catastrophes longer than you've been alive (`act3.returned_annotator`,
    `act3.loop_glimpsed`).
  - **The Bondsman** (Castian) — collared at eleven, named after a dog, who spent nineteen years learning law
    and quietly engineered a clause that *inverted the bond* — and discovers, an hour from the end, that he
    never wanted to *own* his master; he wanted to be *seen* (`act3.bondsman_freed_master`).
  - **Naeve** *(pre-recruit)* — the Seventh Enclave's last daughter, building an indictment out of grief she
    won't let herself feel, proving the apocalypse was a *murder with signatures* — including her father's,
    the man who once named her the stars. A companion who joins not to be saved but to give the indictment
    *legs* (`act3.naeve_will_join`, `act3.naeve_knows_annotator`).
- 🧵 **Theme / plot**: the through-line of the Wall meets the through-line of *you* — the warning unheeded,
  the record that has no room for the part that hurts (that the guilty were *beautiful first*), and the dawning
  shape of the Returned as the one condemned to write *stop* in margins the powerful refuse to read.
  Netheril (#53–60) is now **fully voiced**.
- 📈 **Content**: dialogue corpus now **144 conversations · 881 nodes · 112 skill checks** (up from
  140/848/107). All 606 headless CI checks green; the 20-file `.dlg` corpus compiles clean (zero broken refs,
  dup ids, or orphans) and the viewer regenerates byte-for-byte.

---

## 👑 v4.23.0 — *"The City That Fell Politely"* — Act III opens: Netheril, −339 DR (Pillar 2)

> Karsus reaches for godhood, the Weave begins to die, and the Seventh Enclave starts its long fall out
> of the sky. Four souls meet the apocalypse exactly in character: a steward who serves canapés as a
> *vigil*, a boy who could stop it and won't because no one ever asked his name, an engineer who lies
> beautifully to buy four hundred calm goodbyes, and the one "broken" child the dying Weave cannot touch.

- 🏛️ **Act III opens in Netheril** (`netheril_denizens.dlg`) — the floating-city apocalypse, told from the
  inside:
  - **Steward Vael** keeps the last table of the Seventh Enclave as the floor cants three degrees — not from
    denial but from *love*, the only grammar he was ever given; press him and the house-name falls away to
    *Corin*, the indentured boy who'd have kept *bees* (`act3.witnessed_fall`).
  - **Karsus's Apprentice** stands frozen at the sanctum door, able to end the apocalypse with one word and
    unable to, because the man casting it is *the only person who ever wanted him in the room.* Give him back
    his name — *Eshe* — or reframe obedience as loyalty to the *saner* master, and the door opens
    (`act3.apprentice_opens_door`).
  - **The Mythallar-Keeper** has told four hundred souls everything is *fine* for three hours — mercy and
    cowardice wearing the same smile. A *third path* exists between the pretty lie and the panic: the quiet
    truth, told to the few who can act (`act3.keeper_quiet_truth`); behind her composure is Joss, the
    apprentice she sent to fetch a lens instead of home to his mother (`act3.find_joss`).
  - **The Weave-Deaf Child** — *Sefris* — sits perfectly calm while every arcanist bleeds from the ears,
    because the "defect" that made her worthless in a magocracy is the one thing the dying Weave can't reach.
    The broken girl found the way down: the non-magic cargo-gliders the lords were too proud to take
    (`act3.glider_escape`).
- 🧵 **Theme**: hubris ends an age, and the people who meet the end with grace are never the magelords —
  they're the servant, the foundling, the liar choosing mercy, and the child a proud world threw away.
  The thing called *broken* was built, in secret, for the hour the world breaks.
- 📈 **Content**: dialogue corpus now **140 conversations · 848 nodes · 107 skill checks** (up from
  136/816/101). All 606 headless CI checks green; the 19-file `.dlg` corpus compiles clean (zero broken refs,
  dup ids, or orphans) and the viewer regenerates byte-for-byte.

---

## 👑 v4.22.0 — *"The One Who Came Before, and the Song That Won't Stop"* — Candlekeep & the Fugue, complete (Pillar 2)

> The library's last secrets and the grey's most beautiful sorrow. The Returned who reached the crown
> and lost. The cat who has outlived every reader. A candle hunting a forbidden name, a furious girl who
> refuses to be your reason, a grief made into light, and three drowned voices holding eleven thousand
> names aloft by never falling silent.

- 📚 **Candlekeep Wave III** (`candlekeep_denizens_3.dlg`) finishes the library: **the Sealed-Stacks
  Revenant** — *a previous Returned*, walled into the lore for almost winning the *wrong* way, who reveals
  the crown at the road's end is the **trap, not the prize** (`act2.break_the_crown_known`), and spent forty
  years rewriting his own cautionary warning into a *map*; **the Cat Who Reads** (Margin) — the one unjudged
  creature in Candlekeep, who guards the dangerous book not with claws but by *remembering every reader the
  books forgot* (Speak-with-Animals and Handle-Animal paths); and **Scrivener Pol** — a terrified copyist
  whose sister's debt was paid months ago, who kept copying the sealed truth anyway and became a
  *whistleblower* without admitting it (`sq.pol_pages`, `act2.wall_provenance_pages`).
- ⚖️ **The Fugue Plane, Wave II** (`fugue_denizens_2.dlg`) completes the grey: **Wickless** — a candle-spirit
  lit at Hollowmere's festival "for the person we're not allowed to talk about," who didn't *forget* its
  forbidden name but *hid it from itself*, holding one walled soul a candle's-width into the light
  (`act2.forbidden_name_carried`); **Maerin** — Aldric's daughter, fading and *furious*, who refuses to be
  flattened into anyone's grief and demands to be carried *sharp* — cruel-over-a-carved-fish, unfair-to-walruses,
  recruitable as a companion who comes back to be *furious in person* (`sq.maerin_recruit`,
  `act2.maerin_will_return`); **the Mournlight** — ten thousand forbidden mournings made into light, the
  **golden-path key** that can only be carried by hands holding a grief *they were never allowed to feel*
  (`act2.golden_path_key`); and **the Three Shades of a Drowned Choir** — who discovered a name *sung* is a
  name the Wall *cannot file*, imprisoned eleven thousand names in a song they can never end, and learn the
  way out is not to sing *harder* but *wider* — until the whole world sings the Faithless (`sq.spread_the_song`).
- 🧵 **Theme**: the crown is the office, not the answer — you win by *breaking* it, not wearing it; a name
  *carried*, *sung*, or *grieved* is a name the Wall can't keep (`act2.cataloguing_defeats_the_wall` reinforced
  from three new directions). Candlekeep (#35–44) and the Fugue Plane (#45–52) are now **fully voiced**.
- 📈 **Content**: dialogue corpus now **136 conversations · 816 nodes · 101 skill checks** (up from
  129/759/90). All 606 headless CI checks green; the 18-file `.dlg` corpus compiles clean (zero broken refs,
  dup ids, or orphans) and the viewer regenerates byte-for-byte.

---

## 👑 v4.21.0 — *"The Margins and the Grey"* — Candlekeep Wave II + the Fugue Plane opens (Pillar 2/3)

> The scholars who read *you* like a disputed passage — and, beyond the last page, the grey afterlife
> where the dead stand in line and a clerk has been stamping FAITHLESS for ten thousand years on the
> authority of a god who quit.

- 📚 **Candlekeep Wave II** (`candlekeep_denizens_2.dlg`): **Brother Tym the Indexer**, cross-referencing
  every soul in the Wall against the Catalogue of the Living and going mad *pleasantly* — he'll set you on
  `sq.tym_index` when he notices the one name that appears in both; **Mistress Ekko, Tutor of Dead Languages**,
  who teaches you to read your own half-formed true name (it spells *"the one who returns"*) and weeps at the
  spelling; and **Magister Cole**, who is certain the Returned are a *textual error* and means to correct you —
  until you turn the argument around and convince him the *Wall* is the unauthorized interpolation.
- ⚖️ **The Fugue Plane opens** (`fugue_denizens.dlg`): **the Doorman of the Crystal Spire**, who sorts the
  freshly dead by faith and suffers a small crisis when *you* arrive breathing; **Petitioner 9,000,001**
  (Edwy, who sold pears), who has stood in line so long he has forgotten his own name — the queue, he'll tell
  you, is "the Wall with better manners"; **the Last Honest Psychopomp**, a reaper quietly *hiding* forty-one
  fading Faithless souls instead of delivering them to the masonry; and **the Clerk of the Wall**, Jergal's
  abandoned subordinate, still stamping FAITHLESS out of pure inertia — who lets slip that the Wall has **no
  author** at all, and hands you `sq.wall_ledger`.
- 🧵 **Theme deepened**: the Wall can only take what *nothing else has kept* — so cataloguing, remembering, and
  naming defeat it (`act2.cataloguing_defeats_the_wall`); the Wall is an *agreement*, not a law
  (`act2.wall_is_agreement`), and an orphaned one (`act2.wall_has_no_author`). Reading your own name sets
  `act2.true_name_meaning`; converting Cole sets `act2.wall_is_the_corruption` / `act2.wall_unauthorized`.
- 📈 **Content**: dialogue corpus now **129 conversations · 759 nodes · 90 skill checks** (up from 122/703/82).
  All 606 headless CI checks green; the 16-file `.dlg` corpus compiles clean (zero broken refs, dup ids, or
  orphans) and the viewer regenerates byte-for-byte.

---

## 👑 v4.20.0 — *"The Gate Filled, the Library Opened"* — Wave IV + Candlekeep's scholars (Pillar 3/4)

> The Upper City's idle cruelty, the Docks' practical mercy, a corpse who cheats at dice — and the first
> of Candlekeep's magnificent obsessives.

- 🏛️ **Wave IV finishes the Gilded Gate** (`gate_denizens_4.dlg`): **Duke Aldwin the Absent** (governs by
  committee so no decision is ever *his* — and can be shamed into the first signature in years); **Ysolde de
  Lancie** (a bored patrician shopping for immortality, who can be sold a *use* instead); **Archivist Threnn**
  (keeps two ledgers — the survivable lie for the patriars, the unbearable *true* count for the dead, because
  *someone* has to write the names down right); **Old Marrow** (a corpse who keeps coming up from Grud's
  graveyard to cheat gently at dice, staying square with a grieving friend the harvest can't take from him —
  because he's already gone).
- 📚 **Candlekeep opens** (`candlekeep_denizens.dlg`) — a new region, *Planescape*-hushed: **Gatewarden Imisk**
  (the toll is a book the library lacks — and *you* may be a unique volume); **First Reader Ovid** (blind, has
  read everything, remembers all of it, and reads a single *blank page* for the only silence he has —
  warns you the Crown speaks in your own voice); **Brother Faolan of Acquisitions** (will do anything for the
  *Catalogue of the Unwritten* — because his own unwritten poems are in it); **Henwick the Misfiled** (a man
  catalogued *as a book* sixty years ago, now immortal and unharvestable — the living key: *the Wall can only
  take what nothing else has kept*).
- 🔁 Reactivity into the Act II spine: gates on `prologue.garrow_doubt`, feeds `act2.candlekeep_entered`,
  `crown.is_myrkul`, `act3.lr_loop_revealed`, and the thematic discovery `act2.cataloguing_defeats_the_wall`.
- 📈 **+8 conversations / +65 nodes** — game total **114 → 122 conversations** (638 → **703 nodes**, 73 →
  **82 skill checks**). Roster: **32 NPCs authored** across the Gate and Candlekeep. No id/class collisions;
  suite holds at **606 checks**, all green.

## 👑 v4.19.0 — *"The City, Crowded"* — 12 more denizens (Waves II–III) (Pillar 3/4)

> More souls for the Gilded Gate. Comedy, menace, the companions' faces, and the men who ferry the dead.

- 🎭 **Twelve more fully-voiced NPCs** across two packs (`gate_denizens_2.dlg`, `gate_denizens_3.dlg`):
  **Hessa Dock-Eye** (the one-eyed smuggler running an underground railroad for the Faithless she'd deny to
  the gallows); **Father Brisk** (a Kelemvorite at the exact midpoint of Garrow's road, performing perfect
  empty rites); **Madame Eyes** (a fraud fortune-teller whose cards keep coming up *true* for you — and keep
  repeating, because she's read you in other lives); **Pell the Honest Beggar** (who'll waste your coin on
  drink and says so); **Brindle Quillfeather** (a gnome whose useless "empathy detector" turns out to map the
  harvest); **the Twins Who Aren't** (two strangers who started with a con and grew a brother inside it);
  **Doctor Sallow** (a cowardly plague-doctor afraid not of the fever but of *failing*); **Wrenna Alleywind**
  (Roen's estranged Harper sister); **Quill** (Varra's devil — a courteous broker secretly *rooting* for the
  collateral to beat him); **Justiciar Veld** (the church's true believer, defending a load-bearing wall:
  *if the Wall is wrong, he's a murderer with good penmanship*); **Senna the Glassblower's Widow** (rebuilding
  a memorial the church keeps smashing); **the Ferryman of Chionthar** (a Charon who learns the snare on the
  near bank is the *church's* addition, not the god's toll).
- 🔁 Heavy reactivity: lines gate on `companion.roen.recruited` / `companion.varra.recruited` /
  `act1.fist_ally` / `prologue.garrow_doubt`, and seed companion- and faction-quest hooks (Wrenna↔Roen,
  Quill↔Varra, Veld↔Garrow's quest, plus Hessa's underground, Brindle's harvest-map, the Ferryman's run).
- 📖 `docs/story/NPC_ROSTER.md` updated — **24 Gate denizens now authored** toward the 100+.
- 📈 **+12 conversations / +96 nodes** — game total **102 → 114 conversations** (542 → **638 nodes**, 61 →
  **73 skill checks**). Compiler-validated, no id/class collisions; suite holds at **606 checks**, all green.

## 👑 v4.18.0 — *"A City With a Soul"* — 12 denizens, 2 full side quests, a 100+ NPC bible (Pillar 3/4)

> Turning the dial to 11. BG2's genius was a world *crowded with specific, unforgettable people* — half
> funny, half heartbreaking, all human. This is the first wave of that crowd, and the plan for the rest.

- 🎭 **Twelve fully-voiced Lower City denizens** (`play/dialogue/gate_denizens.dlg`) — the comic, the
  tragic, the absurd, the sinister, many reactive to your Act I deeds: **Jonn "Two-Cups" Tallow** (the
  chandler who supplies a heretic's kettle and doesn't know it); **Grud the Optimist** (cheerful on purpose
  since he buried his whole family); **the Weeping Auctioneer**; **Pib & Wee Pib** (a ratcatcher and his
  rat "consultant"); **Old Threnody** (the blind dirge-bard who hears the Wall sing); **the Smiling Man**
  (a salesman of leashes); **Velith the Unlucky** (the one honest table in a loaded city); **Mother Cass**;
  **Tobias the tax-ghost**; **Lula the Memorist**; **Deacon Mallow**; **Sergeant Bricka**.
- 🪙 **Two full side quests** (`play/dialogue/gate_sidequests.dlg`), each *about* the saga's true question
  (*is a name allowed to matter?*), each a choice with no clean answer, each with a real payoff:
  - **The Unfiled Soul** — help a dead clerk file his own death certificate; the witness-box demands you
    swear a forgotten functionary *mattered* — truly, or kindly, or refuse the Choir's offer to make him
    "matter" by harvest. Branches on Persuade/Insight, the church's stingy ink, and `deed.named_the_forgotten`.
  - **What the Living Owe** — three powers (Choir, church, patriar) come for Lula's list of the forgotten;
    prove that a thing **kept for nothing** is the one thing the Wall can't corrupt (`deed.kept_for_nothing`).
- 📖 **`docs/story/NPC_ROSTER.md`** — the cast bible: **100+ NPCs planned by region** (the Gate, Candlekeep,
  the Fugue, the four eras, the Court of the Dead, recurring "anywhere" faces), with one-line hooks and a
  **Side-Quest Bible** under the design law: *no NPC may be generic; no side quest is done until it finds
  its grief.*
- 📈 **+15 conversations / +128 nodes** — game total **87 → 102 conversations** (414 → **542 nodes**, 44 →
  **61 skill checks**). We crossed **100 conversations**. Compiler-validated; round-trips through
  `extract-dialogue.py`; suite holds at **606 checks**, all green.

## 👑 v4.17.0 — *"The City Remembers"* — Act I deepened with reactive aftermath (Pillar 3/4)

> Chapter 1 stops being a corridor of set-pieces and becomes a *place that reacts* — every NPC reads what
> you did at Hollowmere, who you recruited, and whether you named the Doomguide a monster.

- 🏙️ **Five reactive Act I conversations** (`play/dialogue/act_one_deep.dlg` → `ActOneDeepContent.cs`), each
  gated on the flags Act I sets:
  - **The Count** — return to Aldric after Hollowmere; he reacts to your *third path* / who-you-saved /
    naming-Vayle-a-monster, and the scene can crack his twenty-year self-justification (`aldric.crown_doubt_planted`).
  - **The Doomguide's Question** — Vayle seeks you out, and depending on your firelight choice she turns, doubts,
    or asks to learn a single survivor's name — the discipline-of-mercy lesson she taught Garrow, coming back.
  - **A Name From Hollowmere** — Sable & her son Wick, alive only if you saved them; the small remembering the
    large. Names, not weight.
  - **The Guild's Interest** — a fence who prices you differently if Roen's at your side or you already owe the Guild.
  - **The Choir on the Corner** — a street-preacher recruiting grief; you can nuance, refuse to be a symbol,
    **expose** the harvest's hand in their wells (Persuade 16), or endorse the Choir — each reshaping `faction.choir`.
- 📈 **+5 conversations / +37 nodes**, ~40 condition-gated branches reading `act1.*` / `companion.*` / `faction.*` /
  `aldric.*` flags so the Lower City *answers* your Act I run. Game total **82 → 87 conversations, 377 → 414 nodes**.
  Compiler-validated, round-trips through `extract-dialogue.py`; suite holds at **606 checks**, all green.

## 👑 v4.16.0 — *"The Spine, in Voices"* — chapter dialogue, Prologue → Act V (Pillar 4)

> With the compiler in hand, the words pour: a marquee conversation for every chapter of the saga.

- 📖 **Main-spine dialogue for all six chapters**, authored in the `.dlg` DSL (`play/dialogue/prologue.dlg`,
  `act_one … act_five.dlg`) and compiled to C# `Content`:
  - **Prologue** — the Wall's *Reservation*, waking Returned beside Sister Garrow, the Choir herald who
    points you at Aldric.
  - **Act I** — the Gilded Gate (Flaming Fist), recruiting **Roen** (*The Fist and the Knife*) and **Varra**
    (*A Devil's Arithmetic*), the **Burning of Hollowmere** with Doomguide Vayle, and the first world-skip.
  - **Act II** — Candlekeep's price of entry, the **Sealed Stacks** (*the Crown is Myrkul; you are the
    crack*), the **Audience** with a weary, *bound* Kelemvor, and **the Unmade's Offer**.
  - **Act III** — the **Falling City**, the **Court of Knives** with Ilfaeril, the **Last Returned** lowering
    its hood to show your own face, and Naeve recognizing your hand in her apocalypse's margins.
  - **Act IV** — the **Pale Companion** (Myrkul) dropping its mask as the Crown is forged, and the
    **Deathless Garden** — the paradise-tomb where everyone you've lost is alive, and static, forever.
  - **Act V** — the **Convergence** (Aldric & the Crown), the **Keyhole** (Jergal's golden road), the
    **Name** (the loop has no beginning — *you* are its author), and the **Ledger** (judge the discarded by
    hand, one name at a time).
- 📈 **30 new conversations / 219 nodes**, with **Insight / Arcana / History / Persuade / Intimidate** checks,
  approval shifts, faction rep, and ~150 story-flag writes wiring straight into `GameFlags`. The game's
  dialogue more than **doubles: 52 → 82 conversations, 158 → 377 nodes, 13 → 43 skill checks** — all live in
  the Dialogue Viewer. Every node/branch reference is validated by `dlg-compile.py` and round-trips through
  `extract-dialogue.py`; `dialogue_viewer.test.js` (20) + `dlg.test.js` (14) stay green. Headless suite holds
  at **606 checks**. *(The generated C# compiles on your machine.)*

## 👑 v4.15.0 — *"Words at Scale"* — a dialogue compiler + the first banter flood (Pillar 3/4)

> The bottleneck on BG3-scale writing wasn't ideas — it was that every conversation is verbose hand-C#.
> So: a DSL and a compiler, and the words start to pour.

- ✍️ **`.dlg` → C# dialogue compiler** (`tools/dlg-compile.py`) — author branching dialogue in a clean,
  readable DSL (`node:` / `* choice -> target`, `[INSIGHT 13]` skill checks, `?? fail`, `| if …`, `| fx
  appr.roen +5`), and it generates `Assets/Scripts/Content/<Name>.cs` in the **exact** pattern the runtime
  and `extract-dialogue.py` expect (regular escaped string literals, canonical field names). Writing in the
  DSL is ~5–10× faster than hand-C# and makes the story bible's existing scripts importable. The compiler
  **validates before it emits**: every choice/auto/start reference must resolve, no duplicate ids, no
  unreachable nodes — authoring errors are caught headlessly, before Unity ever sees them.
- 💬 **First content flood — companion campfire banter** (`play/dialogue/campfire_banter.dlg`) — **8 new
  two-hander conversations / 44 nodes**, on-voice for the whole cast (Garrow's grey mercy, Roen's honest
  dishonesty, Varra's infernal debt, Naeve's Netherese grief, Ilfaeril's ten-thousand-year weariness,
  Maerin's "a free gift from no one"), with Insight/Arcana/History/Persuade checks, approval shifts, and
  story-flag writes. The game's conversation total rises **52 → 60** (158 → **202 nodes**, 13 → **18 skill
  checks**), all live in the Dialogue Viewer.
- 🧪 **`dlg.test.js`** runs the real compiler against fixtures and pins the DSL contract (skill/ability
  mapping, `appr.`/`rep.`/`SET`/`CLEAR`/`>=` → the right `FlagClause`/`FlagOp`, fail branches, `END`,
  *regular* strings not verbatim) and that validation rejects dangling refs + duplicate ids — **14 checks**.
  `dialogue_viewer.test.js` still green (20). Headless suite now **606 checks**, all green; the generated
  C# round-trips cleanly through `extract-dialogue.py`. *(The C# compile itself is verified on your machine.)*

## 👑 v4.14.0 — *"Hold the Thought"* — Concentration (Pillar 1: the cost of a buff)

> 5e/BG3's quiet balancing rule: a buff isn't free — keep it up only while you can keep your focus.

- 🧠 **Concentration** in `play/crown_combat.html` — **Mirror Image** is now a **concentration** spell. While
  Naeve sustains it (+4 AC), **any damage she takes forces a Constitution save** — DC **10, or half the
  damage if higher** — and on a failure the spell **drops**. Suddenly her glass-cannon screen is a thing you
  have to *protect*, not just cast: peel the foes off her, or watch the mirrors shatter. (The reaction it
  pairs with — Shield — can still save the hit outright, so the two interact exactly as in BG3.)
- 🧪 **`conc.test.js`** lifts the page's real `concentrationDC` / `concentrationHolds` and proves the DC rule
  across the rounding edges (1→10, 20→10, 21→10, 22→11, 40→20) and the hold/break comparison, plus the
  wiring (Mirror Image flagged `concentration`, damage triggers the save, a failed save strips the effect,
  no roll without damage) — **15 checks**. The smoke run now **casts Mirror Image and tests concentration**
  across 400 games, **0 errors** (heroes win 60.5% — Naeve spends the odd turn shielding herself, still in
  band). Headless suite now **592 checks**, all green.

## 👑 v4.13.0 — *"We Carried Each Other Out"* — the Rescue deed (Pillar 1 × 3 × 4)

> The new downed system, threaded straight through the deed loop: pull a fallen comrade back and win, and
> the whole game remembers it — in voice, in the Chronicle, in the ending.

- 🤝 **Rescue deed** — reviving a downed ally *(v4.12.0)* and going on to win now earns **`deed.combat.rescue`**,
  which flows through the entire deed pipeline built across v4.7–v4.10 with no new plumbing: the **Chronicle**
  records it, a **deed-reactive victory bark** voices it (Garrow: *"We carried each other out. That is the only
  victory that has ever counted."* — and a rescue is now the **most resonant** deed, spoken first), the Save
  Inspector's **Combat Chronicle** panel lists it, and the Endings Explorer gives it a **conduct line** *and* a
  **BG2-style epilogue slide**. *How* you held your party together follows you to the last page.
- 🗣️ **Revive bark** — the moment you Help an ally up, a companion calls it (Garrow: *"On your feet — I have you.
  Stay with me."*, Roen, Varra) — a new `revive` bark beat, present-aware and rate-limited like the rest.
- 🧪 Verified across the existing suites rather than a new file: **`deeds.test.js`** (+ rescue earns/omits the flag,
  stacks to 5, labelled), **`deedbark.test.js`** (+ Garrow speaks it, first priority), **`barks.test.js`** (+ the
  `revive` beat exists and `reviveAlly` fires it), and the cross-page drift test still pins the new label across
  Combat ↔ Save Inspector. Smoke still **400 games, 0 errors** (heroes win 73.0%). Headless suite now **577
  checks**, all green.

## 👑 v4.12.0 — *"Not Dead Yet"* — Downed, death saves & Help (Pillar 1: the marquee mechanic)

> BG3's most-felt rule: a hero at 0 HP doesn't die — they go **down, dying**, and the table holds its breath.

- 🩸 **Downed + death saves** in `play/crown_combat.html` — when a hero drops to 0 HP they no longer die
  outright; they fall **unconscious and dying**, rolling a **death save** each turn (a pure d20: 10+ a
  success, under a failure, a **1** counts double, a **natural 20** snaps them back up at 1 HP). **Three
  successes** stabilises them (out of the fight but safe); **three failures** is death. Downed heroes still
  hold their tile (you can't walk through a body) and are drawn **prone and ghosted** with live **✓/✗ pips**.
- 🤝 **Help (revive)** — a conscious hero standing beside a fallen ally can spend their action on the new
  **🤝 Help** button to drag them **back up at 1 HP**. Real triage: save your friend, or press the attack?
- 💀 **The loss condition is now BG3's** — the party is defeated only when **every** hero is down or dead,
  not the instant one falls. A single hero can hold the line while the others fight to revive the fallen.
- 🧪 **`downed.test.js`** lifts the page's real `deathSaveStep` and proves every transition (success/
  failure/nat-1-double/nat-20-revive, the 3-success stabilise and 3-failure death, tally clamping, a
  missing tally), plus the wiring (heroes go down not dead, downed bodies occupy + render + take their
  save-turn, Help revives to 1 HP) — **21 checks**. The smoke run now **goes down, rolls saves, and
  revives** across 400 games, **0 errors** (triage costs tempo, so heroes win **73.0%** — still in band).
  Headless suite now **572 checks**, all green. (Barks test updated for the refactored death path.)

## 👑 v4.11.0 — *"Hold, or Loose"* — per-reaction stances (Pillar 2: QoL)

> BG3 lets you set each reaction to ask or auto; here you can **arm or hold each reaction independently** —
> keep your Shield up while you stop spending your turn on opportunity attacks.

- ⚔🛡🌀 **Per-reaction stances** in `play/crown_combat.html` — the single ⚡ Reactions switch is now a *master*
  over three independent stance toggles: **⚔ OA**, **🛡 Shield**, **🌀 Counter**. A reaction fires only if the
  master **and** that reaction's stance are on, so you can, say, hold your opportunity attacks (to bait a
  foe past you safely) while keeping Naeve's Shield and Counterspell armed. Stances persist across restarts,
  default to all-on (zero behaviour change out of the box), and dim when the master is off.
- 🧪 **`stance.test.js`** lifts the page's real `reactionAllowed` gate and proves a reaction needs **both**
  master and stance (master-off kills it, stance-off holds it, kinds are independent, unknown/missing inputs
  are safe), plus the wiring (the three gates consult their stance, the toolbar toggles, the re-render) —
  **16 checks**. The Shield/Counter/QoL tests were updated to the new gate. Smoke still **400 games, 0
  errors** (default all-on). Headless suite now **551 checks**, all green.

## 👑 v4.10.0 — *"How You Fought Follows You"* — deeds reach the ending (Pillar 1 × 4)

> The deed loop's final mile: *how* you won a fight now echoes in the **epilogue the game writes for you**.

- ⚰️🕊️ **Deeds reach the ending** in `play/endings_explorer.html` — the Endings Explorer now folds the combat
  Chronicle's `deed.combat.*` flags into the run's flags on arrival (whether or not a save was staged), and
  lets them speak in two places: a **Chronicle conduct line** (*"Conduct at the Wall: merciful — you spared
  even the dead that barred your way"* / *"…ruthless — not one of the Returned you faced was left standing"*)
  and a **BG2-style epilogue slide** keyed to your standout deed — mercy, ruthlessness, a Counterspell that
  unwove the crown's death-song, or a foe cast from the heights. A banner notes the deeds folded in. This is
  the **final mile** of the loop begun in v4.7.0: combat → deeds → Chronicle panel (v4.9.0) → and now all the
  way into the *ending the game writes for you*.
- 🧪 **`deedepi.test.js`** lifts the page's real `deedConductLine` / `deedEpilogueSlide` and proves they
  answer to the deed flags (merciful outranks ruthless; conduct outranks the flavour deeds; the right prose
  per deed; **nothing** when unfought), plus the wiring (deeds read at boot, merged onto staged save flags
  *and* applied with no save, the Chronicle + epilogue hooks, the banner) — **17 checks**. The Endings engine
  port + prose-parity gates still pass. Headless suite now **535 checks**, all green.

## 👑 v4.9.0 — *"The Ledger Made Visible"* — the Combat Chronicle panel (Pillar 1 × 4)

> The deed loop, closed end-to-end: combat writes the deeds, and now the narrative UI *reads them back*.

- 📖 **Combat Chronicle** in `play/save_inspector.html` — a new panel that reads `coh.combat.deeds` (the flags
  the Combat tab writes on victory, v4.7.0) and shows them in plain language: *flawless* / *hard-won*,
  *merciful* / *ruthless*, *a foe cast down from the heights*, *the boss's spell unwoven* — with a count and
  a **clear** button. The Save Inspector already staged flags **into** combat (v3.99.0); this is the return
  leg, so the loop is now whole and **visible**: stage a run → fight → see *how* you fought recorded right
  back in the narrative inspector.
- 🧪 **`deedchron.test.js`** lifts the page's real `deedChronicle` and proves it turns the stored flag map
  into a sorted, labelled list (empty on none, skips cleared flags, labels unknown deeds by key), and —
  importantly — **cross-checks** that every deed the Combat tab can earn (parsed from `crown_combat.html`'s
  `/*<DEEDS>*/` block) has a label here, so the two pages can't drift. Plus the wiring (reads `coh.combat.deeds`,
  renders, clears, boots) — **15 checks**. The existing Save-Inspector round-trip still passes. Headless suite
  now **518 checks**, all green.

## 👑 v4.8.0 — *"They Saw What You Did"* — deed-reactive victory barks (Pillar 1 × 3)

> The deeds don't just go in a ledger — your companions *react* to them, in voice, the moment the fight ends.

- 🗣️ **Deed-reactive victory barks** in `play/crown_combat.html` — the instant a battle is won, the companion
  who's standing speaks to *how* you won it: **Naeve** savours a **Counterspell** unwoven, **Roen** grins at a
  **ledge kill**, **Garrow** blesses a **merciful** (all-spared) victory, **Varra** owns a **ruthless** one,
  and a clean **flawless** or sorrowful **hard-won** finish each draws its own line. The most *distinctive*
  deed speaks first — a Counterspell or a cast-from-the-heights trumps a plain flawless win — and only a
  companion still alive can voice it. Fires right after the Chronicle records the deed, turning v4.7.0's
  ledger entry into a lived reaction.
- 🧪 **`deedbark.test.js`** lifts the page's real `pickDeedBark` and proves the earned flags + who's present
  resolve to exactly one fitting line (right speaker per deed, most-distinctive-first priority, presence-
  gated with fall-through when a speaker is down, nothing on no-deeds), that every prioritised deed has a
  line and every speaker is a real companion, plus the `recordDeeds` wiring — **15 checks**. Smoke still
  **400 games, 0 errors**. Headless suite now **503 checks**, all green.

## 👑 v4.7.0 — *"The Chronicle Remembers"* — Deeds in combat (Pillar 1 × 4: tactics feed the story)

> The superiority twist: BG3's combat is spectacular but *self-contained*. Here, **how** you win writes
> itself into the story.

- 📜 **Deeds in combat** in `play/crown_combat.html` — every victory is now appraised and recorded as
  **story flags** the wider game can read. Win without losing a hero → **flawless**; lose one → **hard-won**.
  Spare every foe (non-lethal) → **merciful**; give no quarter → **ruthless**. Cast a foe off the
  high-ground ledge to their death → **a ledge kill**; unravel the boss's spell with Counterspell → **the
  weave unwoven**. On victory the **Chronicle** narrates the deeds earned and writes them to
  `localStorage` as `coh.combat.deeds` — the **reverse** of the flag bridge the combat already *reads*
  (`coh.combat.flags`), closing the Pillar 1 ↔ 4 loop: the save shapes the barks, and now the fight
  shapes the save.
- 🧪 **`deeds.test.js`** lifts the page's real `deedFlags` mapping and proves a battle summary earns exactly
  the right flags (flawless xor hard-won; merciful/ruthless only on a pure finish; the ledge + counter
  deeds; **nothing** on a loss; up to four stacking on a clean sweep; every flag has a Chronicle label),
  plus the wiring (ledge-kill & countered-boss tracking, the summary counts, the `recordDeeds` call, the
  `coh.combat.deeds` write) — **16 checks**. Smoke still **400 games, 0 errors**. Headless suite now
  **488 checks**, all green. (Tactics help documents deeds.)

## 👑 v4.6.0 — *"The Weave Unravels"* — Counterspell (Pillar 1: reactions)

> Shield's offensive twin: don't survive the boss's spell — *deny* it.

- 🌀 **Counterspell** in `play/crown_combat.html` — **Naeve** gains the reaction's offensive half. When the
  boss **casts a spell** (its **Wail of the Returned**, now flagged `isSpell`), she can spend her reaction
  to **unravel it before it resolves** — the spell **fizzles entirely**: no damage, no Frighten, nothing.
  It's a **once-per-battle** charge (a precious high-level slot), draws from the **same reaction** as Shield
  and opportunity attacks (so it's Counter *or* Shield *or* an OA that round), and is gated by the **⚡
  Reactions** toggle. Together with Shield *(v4.5.0)* this is the working core of BG3's reaction system —
  a defensive *and* an offensive reaction, sharing one honest economy.
- 🧪 **`counter.test.js`** lifts the page's real `canCounter` predicate and proves it triggers on exactly an
  enemy spell with a ready counterer (not a plain attack, not a friendly cast, not once the charge or the
  reaction is spent, not while down, not without the spell known), plus the wiring (the Wail's `isSpell`
  flag, Naeve's `knowsCounter`/`counterReady`, the `useAbility` intercept, the reaction+charge spend, the
  full fizzle) — **15 checks**. The smoke run now **Counterspells across the 400-game auto-play**; still 0
  errors. Headless suite now **472 checks**, all green. (Tactics help + the Reactions toggle now list it.)
- ⚖️ **Balance** — making Counterspell a once-per-battle charge keeps heroes at **81.5%** win (an at-will
  counter that neutered the boss's only AoE spiked it to 89.8%).

## 👑 v4.5.0 — *"Not Today"* — the Shield reaction (Pillar 1: reactions)

> BG3's defensive reaction, and the start of a real reaction UI: the blow that *would* have landed… doesn't.

- 🛡 **Shield** in `play/crown_combat.html` — **Naeve** gains the arcanist's classic reaction. When a foe's
  attack roll would land on her **by 4 or less**, she spends her **reaction** to raise a Shield for **+5
  AC** and turns the blow **fully aside** — no damage, no rider. It never saves against a **crit** or a
  comfortable hit (you can't bluff a clean strike), fires **once per round**, and draws from the **same
  reaction** as opportunity attacks (Shield *or* an OA, not both), all gated by the existing **⚡ Reactions**
  toggle. The first defensive reaction — the bones of BG3's full reaction UI.
- 🧪 **`shield.test.js`** lifts the page's real `shieldNegates` predicate and proves +5 AC turns aside
  exactly the hits it should (lands-by-0..4 yes; by-5, a comfortable hit, a miss, or any crit → no), plus
  the wiring (Naeve's `knowsShield`, the attack-roll/hero/`reacted`/`reactionsOn` gate, the clean early
  return) — **15 checks**. The smoke run now **Shields across the 400-game auto-play**; still 0 errors.
  Headless suite now **457 checks**, all green. (Tactics help + the Reactions toggle now mention Shield.)

## 👑 v4.4.0 — *"The Long Way Down"* — Shove off a ledge (Pillar 1: environmental kill)

> BG3's most-screenshotted moment: don't kill them — let the *floor* do it.

- 🪨 **Shove off a ledge** in `play/crown_combat.html` — **Shove** now reads the terrain. Win the
  Strength contest while the foe is on the **high-ground mound** and you push them *off* it: instead of
  a one-pace nudge they **fall**, taking **2d6 per level of drop** and landing **Prone** (a new
  condition that grants attackers **advantage** next turn). A shove on level ground still just pushes
  them a pace, and shoving a foe **into** the high ground is never a fall — the drop is measured, not
  assumed. This braids three earlier systems — **Height** *(v3.89.0)*, **Shove** *(v3.87.0)*, and the
  surface-into-which-you-shove combo — into one finisher.
- 🧪 **`ledge.test.js`** lifts the page's real height map and the marked `fallDrop` / `fallDamageAvg`
  helpers and proves a high→low shove registers a fall (drop 1), a level shove and a low→high shove do
  not, and the damage scales 2d6/level, plus the wiring (the Prone condition, the drop measurement, the
  2d6×drop hit, the Prone application, the plummet log) — **14 checks**. Smoke still **400 games, 0
  errors**. Headless suite now **442 checks**, all green. (Tactics help now documents the ledge kill
  and the throw.)

## 👑 v4.3.0 — *"A Flask, Well Thrown"* — Throw (Pillar 1: ground-target utility)

> The BG3 zoning tool — lob a flask where you *want* the fight to be, not just where a foe stands.

- 🫙 **Throw** in `play/crown_combat.html` — Roen gains **Alchemist's Fire**, a thrown flask (a new
  `targeting:"throw"` ability kind). Arm it and every tile within **range 5** lights up **orange** —
  click **any passable tile**, empty ground included, and the flask **vaults walls and units** to land
  there: it **bursts on every foe in the blast** (Dex save for half, 1d6 fire) **and seeds a fire
  surface**, igniting any oil/grease it touches. Unlike every prior surface ability — which had to
  target a *foe* — Throw lets you **pre-place a hazard** on empty ground a foe is about to cross.
- 🧪 **`throw.test.js`** lifts the page's real `inThrowRange` predicate and proves a flask reaches any
  tile in range (empty included) but never your own feet, plus the full wiring (the thrown ability,
  `doThrow`'s burst-via-`burstHits` + surface seed + re-check, the tile-target click path, the range
  highlight, label, and hint) — **18 checks**. The smoke run now **throws a flask every game**; still
  **400 games, 0 errors**. Headless suite now **428 checks**, all green.

## 👑 v4.2.0 — *"The Netherese Hand"* — an arcanist joins (Pillar 1 × 3: BG2 wizardry)

> The Infinity-Engine power fantasy: a glass-cannon mage who never misses, drops a fireball
> in a crowd, and steps behind a wall of mirror images. *Baldur's Gate II* in a single hero.

- 🪄 **Naeve, a Netherese arcanist** — a 4th playable hero in `play/crown_combat.html` (INT 18, AC 13,
  22 HP, `spr_naeve` mage sprite). A true glass cannon with three classic spells, each a **new ability
  kind** in the engine:
  - ✶ **Magic Missile** — `autoHit`: a 3d4 **force** bolt that *never rolls to hit*. The forecast shows a
    flat **100% · 7.5 dmg**, the BG2 reliability staple.
  - 🔥 **Fireball** — `enemyburst`: target a tile and it **bursts through every foe in radius 1** (Dex save
    for half, 3d6 fire) **and leaves a fire surface** behind, chaining into the surface combos.
  - 🪞 **Mirror Image** — `selfbuff`: weave it on yourself for **+4 AC for 3 rounds**, the duelist's screen.
- 🧪 **`bg2.test.js`** lifts the page's real `burstHits` and `autoHitDamage` helpers and proves a fireball
  hits exactly the live foes in radius (never allies, never corpses) and that never-miss damage is the
  exact expected value, plus full wiring of Naeve, the three spells, and the resolve/forecast/targeting/
  UI paths — **24 checks**. Smoke still **400 games, 0 errors**. Headless suite now **410 checks**, all green.

## 👑 v4.1.0 — *"Leap of Faith"* — Jump (Pillar 1: verticality)

> The BG3 traversal tool — vault the pillar, clear the gap, get out of reach.

- 🦘 **Jump** in `play/crown_combat.html` — a new movement action (🦘 button / <kbd>J</kbd> hotkey):
  arm it and the leap-range tiles light up **cyan**; click a clear, passable tile within **range 3**
  and your hero **vaults over walls and units** to land there. It spends your move but **provokes no
  opportunity attack** (you're leaping away), and a clean landing is still **undoable**.
- 🧪 **`jump.test.js`** lifts the page's real `inJumpRange` predicate and proves it reaches any tile in
  range (vaulting whatever's between), never your own tile or out of range, plus the wiring (clear/
  passable landing, no OA, button + hotkey + highlight, turn reset) — 10 checks. Smoke still **400
  games, 0 errors**. Headless suite now **386 checks**, all green. (Help panel + hotkey list updated.)
- 🎯 `docs/ROADMAP_BG3.md` — Jump shipped under Pillar 1 verticality.

## 👑 v4.0.0 — *"The Whole Toolbox"* — in-combat Tactics help (milestone)

> A round number, and a fitting capstone to the BG3 push: everything that's been built is now
> **discoverable from inside the fight**.

- ❔ **In-combat Help / Tactics panel** in `play/crown_combat.html` — a **❔ Tactics** button (and the
  <kbd>?</kbd> hotkey, which opens any time) brings up a clean modal that documents the *whole* combat
  toolbox shipped across v3.86–v3.99: **surfaces & their combos** (oil→fire, water→steam, lightning+
  water, poison, ice), **shove**, **height advantage**, **opportunity attacks / Disengage**, the
  **forecast/threat** readouts, the **QoL** toggles (undo · non-lethal · reactions), and the **hotkeys**.
- 🧪 `qol.test.js` extended (now **26 checks**): the panel + button + <kbd>?</kbd> hotkey + open-any-time
  behaviour, and that the help text actually names every system. Headless suite now **376 checks**,
  all green (smoke still 400 games, 0 errors).
- 🏁 **Milestone recap** — across this push the playable combat went from colored tokens to a real
  **isometric, animated, BG3-class tactical fight**, and the four-pillar [roadmap](docs/ROADMAP_BG3.md)
  now shows shipped, *verified* increments on **all four**: mechanics, QoL, characters, and the first
  characters×plot crossover. All explorable, no Unity, every line tested.

## 👑 v3.99.0 — *"Carry It In"* — Save → Combat handoff (the crossover, made visible)

> v3.98 made the companions react to your flags. This lets you *see it*: load a save, watch them speak to that run.

- 🔗 **Save → Combat handoff** — the **Save Inspector** gains an **"⚔️ Open in Combat →"** button that
  stages the run's set flags into `localStorage["coh.combat.flags"]` and opens the Combat tab, which
  reads them on boot and feeds the **story-flag-reactive barks**. So a save with a Varra romance or a
  resolved quest *visibly* changes what your party says mid-fight — the same localStorage bridge the
  Endings Explorer already uses.
- 🧪 `save_inspector.test.js` extended (now **19 checks**): the `toCombat` stager + button, and that the
  Combat page reads the *same* key and routes it into `pickFlagBark`. Headless suite now **373 checks**,
  all green (smoke still 400 games, 0 errors).
- 🎯 `docs/ROADMAP_BG3.md` — the Pillar 3 × 4 crossover is now end-to-end and demonstrable.

## 👑 v3.98.0 — *"She Remembers"* — story-flag-reactive barks (Pillar 3 × 4)

> The reactivity that out-reacts BG3: the *same* combat beat speaks differently depending on
> how your run has gone.

- 🧠 **Story-flag-reactive barks** in `play/crown_combat.html` — a companion's line now *changes with
  your run's `GameFlags`*: a **romance-aware** crit ("That one was for you, love"), a **quest-resolved**
  kill ("No fine print on that one"), a **left-the-faith** spare, an **NG+** wink ("We've done this
  before… skip the speech"). Flag-conditioned lines take priority over the generic ones; the run's
  flags are fed in via `localStorage["coh.combat.flags"]` (so a save's romance/quest state drives the
  voice), defaulting to the generic barks.
- 🧪 `barks.test.js` extended (now **23 checks**): the `pickFlagBark` rule — a set flag with the speaker
  present yields the special line, nothing if the flag's unset or the speaker's absent, plus the
  priority + STORY-load wiring. Smoke still **400 games, 0 errors**. Headless suite now **371 checks**,
  all green.
- 🎯 `docs/ROADMAP_BG3.md` — logged as the first **Pillar 3 × Pillar 4** crossover.

## 👑 v3.97.0 — *"Mercy"* — non-lethal toggle (Pillar 2: QoL)

> BG3's "knock them out, don't kill" — with a spared-vs-slain axis the story can read.

- 🩹 **Non-lethal toggle** in `play/crown_combat.html` — flip it on and a **hero's killing blow on a
  foe knocks them senseless and spares them** instead of slaying (a gentler "downed" beat — no death
  wisp, a green "Spared" pop, and the party reacts: Garrow approves, Varra mock-grumbles). The run
  **tallies how many you spared**, surfaced at victory.
- 🧪 `qol.test.js` extended (now **23 checks**): the pure `outcome()` rule (non-lethal only ever spares
  hero→foe, never your foes' blows or your allies) plus the toggle / tally wiring. Smoke still **400
  games, 0 errors**. Headless suite now **364 checks**, all green.
- 🎯 `docs/ROADMAP_BG3.md` — non-lethal shipped under Pillar 2.

## 👑 v3.96.0 — *"Steam Rising"* — the fire+water combo (Pillar 1 surfaces, complete)

> The last reaction on the floor: dump water on a fire and it *steams*.

- 💨 **Steam** in `play/crown_combat.html` — pour **water onto fire** (or fire onto water) and the tile
  becomes a **steam cloud** instead of just fizzling. Anyone standing in steam gains **cover (+2 AC)**
  (the engine's `armorClassModifier`, so the live forecast already reflects it). Animated wispy tile.
- 🧪 `surfaces.test.js` extended (now **17 checks**): both directions of the fire↔water → steam combo,
  and the steam cover wiring (+2 AC on enter). Smoke still **400 games, 0 errors**. Headless suite now
  **358 checks**, all green.
- 🎯 `docs/ROADMAP_BG3.md` — the full BG3 surface palette is now shipped (fire · grease · water · poison ·
  ice · steam, with ignite / spread / douse / electrify / steam combos).

## 👑 v3.95.0 — *"Bicker"* — inter-companion banter (Pillar 3 deepens)

> They don't just react to the fight — they react to *each other*.

- 🗣️🗣️ **Inter-companion banter** in `play/crown_combat.html` — as a hero's turn opens, the party will
  occasionally trade a **two-line exchange** with each other (Varra needles Garrow; Roen and Varra
  one-up each other; Garrow ribs Roen). **Present-aware** (both speakers must be fielded & alive) and
  **rate-limited** so it stays a flavour-beat, not a wall of text.
- 🧪 `barks.test.js` extended (now **16 checks**): the pair-banter table + picker — both speakers
  present required, no banter if one's absent, distinct speakers, and the turn-start/rate-limit wiring.
  Smoke still **400 games, 0 errors**. Headless suite now **356 checks**, all green.
- 🎯 `docs/ROADMAP_BG3.md` — inter-companion banter shipped.

## 👑 v3.94.0 — *"The Full Palette"* — poison & ice surfaces (Pillar 1 surface breadth complete)

> Every element on the floor now. Pick your hazard.

- ☠️🧊 Two new surface abilities in `play/crown_combat.html` complete the elemental set: **Venom Vial**
  (Roen — lays a **poison cloud** that poisons anyone standing in it) and **Frost** (Garrow — cold
  damage that leaves **ice**, slipping whoever crosses it). With v3.86/v3.88's oil, fire and water,
  all **five** surfaces are now player-creatable and combo-aware.
- 🧪 `surfaces.test.js` extended (now **15 checks**): every elemental surface paints, and the page wires
  all five creating-abilities plus the poison/ice on-enter conditions. Smoke still **400 games, 0
  errors**. Headless suite now **350 checks**, all green.
- 🎯 `docs/ROADMAP_BG3.md` — surface breadth marked shipped (steam/line-of-sight still to come).

## 👑 v3.93.0 — *"They're Watching"* — companion combat barks (Pillar 3: characters begin)

> The reactivity-density win that out-reacts BG3: your party *talks* during the fight.

- 🗣️ **Companion barks** in `play/crown_combat.html` — when a dramatic beat lands (a **crit**, a **kill**,
  an **ally going down**, a foe **igniting**, a **wall-slam**, **victory**), a present companion reacts
  in their own voice — Garrow grave, Roen flip, Varra gleeful. Speech bubble over the speaker + a
  styled line in the chronicle. **Present-aware** (only a living, fielded companion speaks) and
  **rate-limited** so it never spams.
- 🧪 **`barks.test.js`** lifts the page's real bark table + picker and proves only a present companion
  speaks, graceful fallback, determinism for a given roll, and the wiring to every beat (10 checks).
  Smoke still **400 games, 0 errors**. Headless suite now **348 checks**, all green.
- 🎯 `docs/ROADMAP_BG3.md` — first **Pillar 3 (Characters & reactivity)** entry shipped.

## 👑 v3.92.0 — *"Comfort"* — keyboard hotkeys & reaction toggle (Pillar 2: QoL)

> Two more comforts BG3 players never think about until they're gone.

- ⌨️ **Keyboard hotkeys** in `play/crown_combat.html` — `1`-`9` arm the matching ability (now numbered
  on the buttons), `V` shove, `X` disengage, `U` undo, `Enter` end turn, `F` forecast, `R` reactions.
  Pure `hotkey()` map, headless-safe listener.
- ⚡ **Reaction toggle** — a button (BG3's "ask before my opportunity attacks"): flip it off and your
  heroes won't spend their reaction on opportunity attacks. Enemies still do.
- 🧪 `qol.test.js` extended (now **17 checks**): the full hotkey map + the reaction-toggle wiring,
  on top of the Undo predicate. Smoke still **400 games, 0 errors**. Headless suite now **338 checks**,
  all green.
- 🎯 `docs/ROADMAP_BG3.md` — both marked shipped under Pillar 2.

## 👑 v3.91.0 — *"Take It Back"* — Undo Move (Pillar 2: Quality of Life begins)

> The first comfort BG3 players lean on constantly. No more fat-fingered, turn-ruining clicks.

- ↩ **Undo Move** in `play/crown_combat.html` — a new action: take back the active hero's move and
  get the movement **refunded**, as long as the move **triggered nothing irreversible** (no surface
  effect applied, no opportunity attack provoked, and you survived). If a step into fire or out of a
  foe's reach happened, the move is *committed* (BG3-honest) and Undo greys out.
- 🧪 **`qol.test.js`** lifts the page's real `canUndoMove` predicate and proves a clean move is
  undoable while a dirty/lethal one isn't, plus the wiring (snapshot, dirtied-by-surface/OA, restore,
  cleared on act/turn) — 10 checks. Smoke still **400 games, 0 errors**. Headless suite now **331
  checks**, all green.
- 🎯 First **Pillar 2 (Quality of Life)** entry on [the BG3 roadmap](docs/ROADMAP_BG3.md).

## 👑 v3.90.0 — *"No Free Retreat"* — opportunity attacks & Disengage (Pillar 1 continues)

> Positioning gets teeth: you can't just walk away from a foe anymore.

- ⚔️ **Opportunity attacks** in `play/crown_combat.html` — moving **out of an adjacent foe's reach**
  provokes a **free melee strike** from that foe (once per round, for *both* sides — enemies provoke
  when fleeing too). Fully reprojected into the iso board with the lunge animation + combat log.
- 🏃 **Disengage** — a new action: spend it and your movement this turn **draws no opportunity
  attacks** (the tactical-retreat option). Reaction state resets at the start of each unit's turn.
- 🧪 **`opportunity.test.js`** lifts the page's real reach-test and proves OAs fire **only when
  leaving reach** (not when staying, shuffling, approaching, or passing by), plus the wiring
  (Disengage, once-per-round, turn reset) — 11 checks. Smoke still auto-plays **400 games, 0 errors**,
  balance intact. Headless suite now **321 checks**, all green.
- 🎯 `docs/ROADMAP_BG3.md` — opportunity attacks marked shipped.

## 👑 v3.89.0 — *"The High Ground"* — height advantage (Pillar 1 continues)

> "I have the high ground." Now it actually matters.

- ⛰️ **Height advantage** in `play/crown_combat.html` — a contested **raised mound** in the middle of
  the arena. Striking from **higher ground grants advantage**; striking from **below, disadvantage**.
  Folded consistently into the hit math (`resolve`), the XCOM-style **forecast**, and the incoming-
  **threat** readout — so the % you see already reflects the ground you're standing on. High tiles
  are gilded with a ▲ marker, units **stand taller** on them, and the Selected panel flags *▲ high
  ground*.
- 🧪 **`height.test.js`** lifts the page's real elevation helpers and proves high→advantage,
  low→disadvantage, level→neither, plus the wiring into hit math / forecast / view (9 checks). Smoke
  still auto-plays **400 games, 0 errors**, balance intact. Headless suite now **310 checks**, all green.
- 🎯 `docs/ROADMAP_BG3.md` — height advantage marked shipped.

## 👑 v3.88.0 — *"Storm & Tide"* — electrified water (Pillar 1 continues)

> The other combo every BG3 player learns: **water + lightning**. Soak them, then shock them.

- ⚡ **Electrified water** in `play/crown_combat.html` — Garrow's new **Hallowed Water** conjures a
  water surface (and douses fire); Varra's new **Storm Bolt** (lightning) **arcs through any water**
  and zaps *everyone* standing in it for bonus damage + Slowed. A two-hero combo: soak the cluster,
  then shock it.
- 🧪 `surfaces.test.js` extended (now **13 checks**): `isWaterAt` detection + the full lightning/water
  wiring (the `chainWater` ability, `chainLightning`, the `applyOne` trigger). Smoke harness still
  auto-plays **400 games, 0 errors**, balance intact. Headless suite now **301 checks**, all green.
- 🎯 `docs/ROADMAP_BG3.md` — electrified water marked shipped.

## 👑 v3.87.0 — *"Give Ground"* — the Shove maneuver (Pillar 1 continues)

> The combo BG3 players reach for first: shove a foe **into your fire**, or **into a wall**.

- 🫷 **Shove** in `play/crown_combat.html` — a universal action (its own button): a **Strength
  contest** (attacker STR vs the defender's best of STR/DEX) that pushes the foe **one tile
  directly away**. Push them onto a **hazard surface** and they suffer it (shove-into-fire = the
  BG3 environmental kill); push them into a **wall** and they take impact damage. Pairs directly
  with v3.86.0's surfaces.
- 🧪 **`shove.test.js`** lifts the page's real push-direction helper and proves it pushes a foe
  one tile away in **all 8 directions**, plus that the maneuver is wired (contest, shove-into-
  surface combo, wall impact, the UI button) — 9 checks. The smoke harness still auto-plays full
  games with zero errors. Headless suite now **299 checks**, all green.

## 👑 v3.86.0 — *"The Battlefield Reacts"* — environmental surfaces (toward "BG3 but superior")

> The first pillar of the [BG3 roadmap](docs/ROADMAP_BG3.md): BG3's signature combat feature,
> done our way. The ground itself is now a weapon.

- 🔥 **Environmental surfaces** in `play/crown_combat.html` — lay **oil** (Roen's *Oil Flask*),
  then **ignite** it (Garrow's *Firebrand*): fire **spreads** through grease tile-to-tile,
  **burns** anyone standing in it (Burning DoT), and **water douses** it. Surfaces (fire / grease /
  poison / ice / water) apply on enter and at the start of each turn, spread & decay per round,
  and are rendered as animated tiles with their own flickering torch-glow on the iso board.
- 🧪 **`surfaces.test.js`** lifts the page's *real* surface logic and proves the combos:
  grease→fire ignition, fire spread, water dousing, decay, and in-bounds safety (11 checks). The
  smoke harness auto-plays **400 full games** through it with zero errors and balance intact.
  Headless suite now **290 checks**, all green.
- 🎯 **`docs/ROADMAP_BG3.md`** — the honest plan for "similar to BG3 but superior in QoL, plot,
  characters & gameplay mechanics": what we build here vs. what needs Unity/art/budget, shipped
  one verified pillar increment at a time.

## 👑 v3.85.0 — *"In Motion"* — the figures fight, not just stand

> The isometric board was still a diorama. Now the sprites *move*: they turn to face
> their foe, lunge when they strike, recoil when struck, and breathe at rest.

- 🤺 **Attack choreography** in `play/crown_combat.html` (procedural, no new frames):
  - **Lunge** — a striker eases toward its target along the iso vector and back
  - **Recoil** — the struck figure jolts away from the blow
  - **Facing** — each sprite flips horizontally to face whoever it's fighting
  - **Idle breathing** — every living figure has a gentle, phase-offset bob so the board
    is never frozen (a little stronger on the active hero)
- All driven off the existing turn flow + frame clock; the engine, forecasts, conditions,
  VFX and lighting are untouched.
- 🧪 `iso.test.js` now also asserts the choreography is wired (lunge/recoil/facing/breathing);
  the smoke harness still auto-plays full games through the animated `draw()`. Headless suite
  now **279 checks**, all green. Preview + bundle rebuilt.

## 👑 v3.84.0 — *"The Tilt"* — the playable board goes isometric

> The genre's defining look. The Skirmish stops being top-down and becomes a real
> **isometric crypt** — diamond grid, raised stone walls, depth-sorted sprites.

- 🧭 **Isometric renderer** in `play/crown_combat.html` — a 2:1 diamond projection: floor
  tiles are stone diamonds, **walls are extruded 3-D blocks**, and everything (walls, units,
  props) is **depth-sorted back-to-front** so nearer things correctly occlude. The combat
  logic stays on the square grid untouched; only the view + mouse-picking are reprojected.
- 🗿 **Environmental props** dress the crypt — real CC0 feature tiles (a dark altar behind the
  boss, flanking statues, a sealed door) as billboards.
- 🩸 **Persistent decals** — blood/scorch is left on the tile where each combatant falls.
- ✨ The whole juice layer (projectiles, impacts, slashes, crit bursts, torch lighting,
  death wisps, idle-bob) is reprojected into iso. All rules/forecasts/SFX unchanged.
- 🧪 **`iso.test.js`** lifts the page's real projection math and proves click-picking
  round-trips for **all 140 tiles** (centres + interior samples), that the projection is
  injective and in-bounds — objective coverage for the one thing a screenshot can't show.
  `cc0_art.test.js` updated; the smoke harness still auto-plays full games through the iso
  `draw()`. Headless suite now **278 checks**, all green. Preview + bundle rebuilt.

## 👑 v3.83.0 — *"The Quickening"* — the combat comes alive (VFX), real art everywhere

> "PLEASE DO MORE." Done: the fight stops being static. Bolts fly, steel sparks, the
> screen kicks, torches breathe — and the Bestiary now wears the same real sprites.

- ✨ **Combat juice** in `play/crown_combat.html` (pure-canvas, no assets, works in the bundle):
  - **Projectiles** streak attacker → target for ranged/spell hits, with an **impact burst** on
    arrival; **slash arcs** for melee; **crit bursts** with radial sparks.
  - **Screen shake** on hits, harder on crits and kills; **death wisps** rise from the slain
    (fitting, for the Returned).
  - **Dynamic torch lighting** — warm pools follow the heroes, cold necrotic glow the Returned,
    with a living **flicker**; the active hero gives a gentle **idle-bob**.
  - Damage-typed colours (fire/cold/radiant/necrotic/force…); every rule, forecast & SFX
    unchanged.
- ☠ **Bestiary now uses the real monster art** — `tools/make-compendium.py` matches **35/37 foes**
  to the same DCSS sprites the board uses (keyword map, token fallback for the rest).
- 🛡️ Render path stayed headless-safe (the smoke harness auto-plays full games through the new
  `draw()` every run); `cc0_art.test.js` now gates the VFX/lighting/shake wiring. Suite **270 checks**,
  all green. Preview refreshed (`play/combat_preview.png`), hub + bundle rebuilt.

## 👑 v3.82.0 — *"First Light"* — the playable combat now runs on real sprites

> "It looks exactly the same…" — not anymore. The **Combat** tab stops drawing colored
> tokens and renders the real CC0 pixel art, on the actual playable board.

- 🗡️ **`play/crown_combat.html`** now draws **real Dungeon Crawl Stone Soup sprites**: crypt
  floors and mossy brick walls (varied per tile), creature sprites for every combatant (the
  priestess/rogue/warlock heroes, the Returned wights · ghoul · zombie, the eidolon boss),
  ground shadows, side-coloured selection rings, and a torch-lit **crypt vignette** — while
  every rule, forecast, condition and SFX stays exactly as verified.
- 🧰 `tools/make-combat-demo.py` embeds the sprites as base64 data URIs (idempotently, between
  `/*<SPR>*/` markers) so the art renders **standalone and inside the all-in-one bundle**, and
  renders `play/combat_preview.png` — a pixel-faithful raster of the opening board.
- 🛡️ Render path stays headless-safe: the sprite load is guarded for no-DOM, and the canvas
  smoke harness now stubs gradients, so `smoke.js` still auto-plays full games through the real
  draw path.
- 🧪 `cc0_art.test.js` grown to assert the combat page embeds the sprites and is wired to draw
  them (+ the bundle carries it). Headless suite now **269 checks**, all green. Hub combat card
  + the rebuilt single-file bundle updated.

## 👑 v3.81.0 — *"One File"* — the whole game in a single portable HTML

> "Can you give me a functioning HTML of all of this, including dialog trees?" — yes:
> **`play/crown_of_horns.html`**, one self-contained file, no server and no other files.

- 📦 **`play/crown_of_horns.html`** (~1.1 MB) — a tabbed shell that bundles all nine browser
  pages and renders each in an isolated `<iframe srcdoc>` so their scripts/styles never
  collide: **Combat** (the real playable engine), **Dialogue** (every branching tree, Map +
  Walk), **Endings**, **Compendium** (Grimoire/Bestiary/Conditions/Codex/Atlas), **Cast**,
  **Flag Graph**, **Saga Map**, **Save Inspector**, **Analytics**.
- 🔗 In-bundle cross-links are rewired to switch tabs and carry the deep-link hash — the Saga
  Map's "💬 conversation" chips jump straight to that conversation in the Dialogue tab, etc.
  The bundle is itself deep-linkable: `crown_of_horns.html#dialogue:ilfaeril.quest.resolution`.
- 🧰 `tools/make-all-in-one.py` regenerates it from the standalone pages (JSON-embedded, lazy
  srcdoc load).
- 🧪 `all_in_one.test.js` (14 checks: all 9 pages embedded & parse, marquee payloads intact,
  shell + rewiring + deep-links wired) added to CI. Headless suite now **263 checks**, all green.
- 🏠 Hub gains a prominent button to the single-file build.

## 👑 v3.80.0 — *"Real Steel"* — actual game art enters the repo

> The art blockade is broken. Dungeon Crawl Stone Soup's **CC0 (public-domain) tileset**
> lives on GitHub — the one reachable host — and a licensed, curated slice of it is now
> *in the project as real, reusable game assets.*

- 🎨 **114 CC0 tiles** → `Assets/Resources/Art/DCSS/` — crypt/tomb floors &amp; walls, doors,
  altars, statues, **36 monster sprites mapped to our bestiary archetypes** (Returned wights &amp;
  ghouls, sorrow-wraiths, the Hollow Cantor, Kelemvorite templars, Crown-Wars blade-singers,
  Netherese war-constructs, the Herald of the Unmade…), 6 hero-usable humanoids, and 12 real
  weapon/armour/potion item icons.
- ⚖️ **Licensing done right** (`tools/fetch-cc0-tiles.py`): DCSS's LICENSE declares the tiles
  CC0; the curated `crawl/tiles` reuse project publishes an unknown-license exclusion list —
  the fetcher downloads that list and **hard-skips anything on it**, then writes
  `Art/DCSS/LICENSE.md` documenting CC0 + provenance for *every* file (source path table).
- 🖼️ **`play/gameplay_v2.png`** (`tools/render-gameplay-v2.py`) — the Cinderhaunt rebuilt from
  the real tiles: lit crypt, the party, the Choir at a necrotic altar, tactical overlays,
  sprite portraits and real item icons in the HUD. BG1-era fidelity from actual licensed art.
- 🧪 `cc0_art.test.js` (11 checks: PNG magic on every file, group counts, LICENSE documents
  every tile, screenshot rendered) wired into CI. Headless suite now **249 checks**, all green.

## 👑 v3.79.0 — *"The Loop Closed"* — save inspector, saga map & a look at the game

> Close the loop between the explorers, lay the whole campaign out on one page, and — for the
> first time — *show* what the final product looks like.

- 💾 **Save Inspector** (`play/save_inspector.html`) — load a real `SaveData` file (or the bundled
  sample), read &amp; edit every flag / int / quest / hero field / gold, see what it unlocks
  (decoded against the flag graph + codex), then **export JSON that round-trips back into Unity**
  byte-for-byte. A one-click **"Open in Endings Explorer →"** stages the flag state (via
  localStorage) and the Endings page now picks it up on arrival — so a real run drives the real
  endings. `tools/make-save-inspector.py` + `play/sample-save.json`.
- 🗺️ **Saga Map** (`play/saga_map.html`) — the whole campaign on one page: the act spine across the
  eras, the conversations in each act, the flags each sets and gates on, the personal-quest lane,
  and the Court of the Dead fanning out to the six endings. **Every link deep-jumps into the
  matching explorer** — conversations into the Dialogue Viewer (`#id`), flags into the Flag Graph
  (`#key`), endings into the Endings Explorer (the two viewers gained URL-hash routing).
- 🖼️ **A gameplay mock-up** (`play/gameplay_mockup.png`) — a rendered look at an isometric Cinderhaunt
  combat encounter with the full HUD (party roster, iso battlefield with move/threat tiles, attack
  forecast, initiative, ability bar), every value drawn from the real content + verified math.
  `tools/render-gameplay-mockup.py`.
- 🧪 `save_inspector.test.js` (17, incl. a loss-free parse↔export round-trip) + `saga_map.test.js`
  (11, every deep-link resolves to a real conversation/flag). Headless suite now **238 checks**, all green.

## 👑 v3.78.0 — *"The Wiring"* — the whole saga's flag graph, browsable

> The narrative engine runs on one `GameFlags` brain. This release renders that brain: a
> dependency graph of every story flag — what sets it, what gates on it — harvested from
> three independent sources and merged.

- 🕸️ **Flag Dependency Graph** (`play/flags_explorer.html`) — pick any of the **167 story flags**
  to see a bipartite map of what **writes** it (dialogue choices, quest resolutions, deeds, era
  content) and what **reads** it (ending gates, codex unlocks, dialogue conditions), each edge
  colour-coded by source. An **overview** panel surfaces the 27 domains, the most-connected *hub
  flags*, and the orphans — **write-only** (set but ungated: future headroom) and **read-only**
  (gated on but set by runtime systems like combat/losses/rests).
- 🧰 `tools/extract-flags.py` merges three harvests: **C# `GameFlags` calls** (GetBool/GetInt = read,
  Set/AddInt = write) across `Assets/Scripts`, the **dialogue clause data** (`FlagClause`
  conditions/effects), and the **codex unlock flags** — into `play/flags-data.json`
  (**515 read/write edges**). `tools/make-flags-explorer.py` renders the page.
- 🧪 `flags_explorer.test.js` (19 checks, incl. a parse-only JS compile and the marquee
  ending-gate → Endings+codex dependency). Headless suite now **210 checks**, all green.

## 👑 v3.77.0 — *"The Codex"* — the world's conditions and lore, browsable

> Keep expanding the Compendium: the two content categories it didn't cover yet — what the
> conditions actually *do*, and the whole lore journal — now have their own tabs, generated
> straight from the C#.

- 🌀 **Conditions tab** — the **6 authored status effects** (Poisoned, Burning, Frightened, Blessed,
  Slowed, Held) as cards whose mechanics are read *directly from each `StatusEffectDefinition`*: DoT
  dice + type, duration, incapacitation, attacker advantage, attack/AC/speed modifiers — translated
  into the engine's own plain English. Plus the full **12-condition vocabulary**, lit where an effect
  backs it.
- 📓 **Codex tab** — all **69 lore entries** (Premise · The Four Masks · The Company · Bestiary · Lore
  & History) with category filters, each carrying the **GameFlags key** that unlocks it in-game and a
  "known from the start" marker for the ungated ones.
- 🧰 `tools/extract-content.py` extended to lift the `Condition` enum, the `BuildEffects()` catalog, and
  `CodexContent.All` (brace-aware, string-concat aware) into `compendium-data.json`; `make-compendium.py`
  renders both tabs and a generalized category filter.
- 🧪 `compendium.test.js` grown to **32 checks** (effects carry real mechanics; codex spans 5 categories,
  every entry well-formed). Headless suite now **191 checks**, all green.

## 👑 v3.76.0 — *"The Conversation"* — every branch, mapped and playable

> Expand everything, again: the game's branching dialogue, lifted out of the C# and turned into a
> readable graph you can study or *play* — without Unity.

- 💬 **Dialogue Tree Viewer** (`play/dialogue_viewer.html`) — every conversation in the game as an
  auto-laid-out node-link graph: speakers, choice edges, **skill-check edges** (ability + DC, with the
  fail branch drawn dashed), and the story flags each line sets. Click any node for its full line and
  effects. Toggle to **Walk** mode and play the conversation — pick choices, roll the checks, and watch
  the flags the run would set tick over in a live panel.
- 🧰 `tools/extract-dialogue.py` — brace-aware parser that lifts the real `DialogueGraph` builders
  (nodes / choices / `FlagClause` conditions+effects / `checkAbility`+`checkDC`+`failNodeId`) into
  `play/dialogue-data.json`. Reactive witness lines whose text is computed from flags are *marked*,
  not invented. `tools/make-dialogue-viewer.py` renders the page.
- 📊 Coverage: **52 conversations · 158 nodes · 147 choices · 13 skill checks**, straight from the source.
- 🏠 Wired into the hub; `dialogue_viewer.test.js` (20 checks, incl. a parse-only JS compile of the
  page) added to CI. Headless suite now **180 checks**, all green.

## 👑 v3.75.0 — *"The Compendium"* — the whole game, browsable

> Expand everything: a generated, data-driven reference for the entire game, and a hub that ties the
> browser-playable suite together behind one front door.

- 📖 **Compendium** (`play/compendium.html`) — four tabs, generated from the C# content:
  Grimoire (40 abilities, with hit% vs AC mirroring the verified AttackForecast), Armory (every
  item's damage/AC/value), Bestiary (37 foes across six eras, engine-faithful to-hit/AC/XP, threat
  tiers, era filters, embedded tokens), and an Atlas of the seven acts and their gating flags.
- 🧰 `tools/extract-content.py` pulls the data straight from the `Weapon()`/`Spell()`/`Item()`/`Enemy()`
  builders; `tools/make-compendium.py` renders the page.
- 🏠 **Hub** (`play/index.html`) — one landing page linking combat demo, endings explorer, compendium,
  cast gallery, and analytics.
- 🧪 `compendium.test.js` (21 checks) wired into CI; headless suite now **161 checks**, all green.

## 👑 v3.74.0 — *"Know Them"* — clickable character profiles

> The cast gallery stops being a wall of faces and becomes a who's-who you can read: click any
> companion or principal for a full character sheet — who they are, what they fight with, the
> branches of their personal quest, whether you can love them, and who they're bound to.

- 🪪 **12 clickable profiles** (modal: role/class/race, stat chips, 3-paragraph bio, personal-quest
  outcomes, romance, bonds) — Garrow, Roen, Varra, Naeve, Ilfaeril, Maerin, the Returned, Aldric
  Morn, Aelryth, Veld, Wrenna, Mother Cass. Authored from the actual content (build functions +
  EndingResolver + the era scripts). Esc/backdrop to close; mobile layout. Headless-validated.
- 🖼️ **Portrait note:** real art is gated by the environment's network allowlist (Wikimedia/Met/AIC
  all 403; only `raw.githubusercontent.com` reachable) and copyrighted "found online" art is refused
  for a public repo. `tools/fetch-portraits.py` stands ready to cast the party with public-domain
  masterworks the moment an art host is allowlisted, or any CC0/GitHub pack is named.

## 👑 v3.73.0 — *"Painted Faces"* — the cast goes painterly, and a masterwork fetcher waits

> The portrait pass: 37 faces step up from labeled tokens to **layered painterly busts** — era-lit
> atmosphere, key-light and core-shadow on the skin, hoods for the priests, helms for the soldiers,
> circlets for the mages, wisp-crowns for the spirits dissolving at their edges — each in its era's
> palette (DESIGN §5), with rim light, grain, and vignette. Deterministic, regenerable, auto-wired.

- 🖼️ `tools/gen-portraits-v2.py` — the painterly generator (pure PIL, no external art, no licenses).
- 🌐 `tools/fetch-portraits.py` — **ready-to-run**: the environment's network allowlist currently
  blocks Wikimedia Commons (verified), but the moment `commons.wikimedia.org` +
  `upload.wikimedia.org` are allowlisted, one run recasts ~27 named characters with **public-domain
  masterworks** (PD-old painters only), credited in `docs/PORTRAIT_CREDITS.md`. Copyrighted "found
  online" art was deliberately refused.
- 🗂️ Cast gallery regenerated with JPEG thumbnails (3.8 MB → 498 KB standalone).

## 👑 v3.72.0 — *"The Door Out"* — a build path, and the story made explorable

> Two doors open at once: the project can finally **build a player** (it had no scene in Build
> Settings — Build would have failed), and the narrative engine becomes something you can *hold* —
> an interactive explorer where you flip what happened in a playthrough and read the epilogue the
> game would write for you.

- 🚪 **Build path**: `Assets/Scenes/Boot.unity` (minimal empty scene — `GameEntryPoint` spawns
  everything at runtime), registered in Build Settings; `Assets/Editor/BuildScript.cs` (menu +
  headless Windows/Linux/macOS builds); `tools/build.sh` for one-command builds. *Honestly flagged:
  standard formats, but unverified by a real Unity run.*
- 🎭 **`play/endings_explorer.html`**: ~50 story flags + reputation sliders → the 6 endings
  lock/unlock live → full prose, your personalized epilogue, the Chronicle. Engine inlined from the
  byte-parity port at build time (`make-explorer.js`); boots headless in CI (`explorer.test.js`).
- 🖼️ **`play/cast_gallery.html`**: all 68 portraits & battle tokens, base64-embedded standalone.
- 🧪 Gates now **140 checks + 2 DOM smokes**, all green.

## 👑 v3.71.0 — *"The Slice Sings"* — WebAudio SFX in the playable demo

> The Unity build found its voice in v3.70.0; now the browser slice matches it. A tiny WebAudio synth
> speaks the same SFX vocabulary as the Unity placeholders, so the demo *sounds* like the game will —
> no assets, no autoplay before input.

- 🔉 **Demo SFX**: hit (thud+noise), crit (sweep+crack), miss (whoosh), heal (rising chime), condition
  (buzz), death (falling tone) — wired into the same events as the floating combat text, with a
  🔊 on/off toggle. Lazy `AudioContext` on first gesture; graceful no-op where WebAudio is absent
  (the DOM smoke harness still boots clean: 400 games, 0 errors).

## 👑 v3.70.1 — *"Their Own Faces"* — companion battle tokens fixed

> A small, real bug surfaced by `tools/asset-check.sh`: the sprite lookup goes `displayName` →
> first word → faction, so the two **multi-word** companions never found their own art — **Sister
> Garrow** and **Roen Alleywind** fought as generic faction silhouettes while their dialogue
> portraits worked fine.

- 🎭 `tools/alias-companion-tokens.py` copies each one's art to the exact-name token (+ fresh-GUID
  `.meta`). All six companions now show their own face on the battle map. Idempotent — drop real
  art with the same name to supersede.
- 📦 Asset completeness: **89% → 93%** (the two remaining names intentionally resolve via the
  `Player`/`Enemy` faction tokens — "The Returned" is shared by the hero and a basic foe).

## 👑 v3.70.0 — *"Sound and Fury"* — the game finds its voice (placeholder A/V)

> The longest-standing gap finally closed: audio and combat VFX were at 0%, so swings landed in silence
> over static cubes. Now every hit cracks, every heal chimes, every spell flashes, and each mode has an
> ambient bed — all fully synthesized in pure Python, no samples, no licenses. Honest placeholders: drop
> a real clip or frame with the same name to replace any of them.

- 🔊 **14 SFX** (`hit`, `hit_<type>`, `crit`, `miss`, `heal`, `deed`, `levelup`) — stdlib `wave` synthesis.
- 💥 **9 VFX bursts** (`impact`, `impact_<type>`, `heal`, 6 frames each) — PIL, Sprite-imported, auto-played
  by `FxSystem` per damage type.
- 🎵 **7 seamless ambient loops** (`Hub`, `Combat`, `Camp`, `Explore`, `Court`, `Vault`, `Fugue`) — sustained
  pads, click-free (whole-cycle partials), no melody to grate.
- Generators live in `tools/gen-placeholder-{sfx,vfx,music}.py`; committed raw (not LFS) with Unity `.meta`.
  **`tools/asset-check.sh`: 28% → 89%.**

## 👑 v3.69.0 — *"Every Step Optimal"* — A* pathfinding, proven least-cost

> The grid A* that moves every unit — hub click-to-move and tactical combat alike — is now proven not
> just *correct* but *optimal*: cross-checked against a brute-force Dijkstra oracle over 3,000 random
> weighted maps, so it can never quietly return a longer-than-necessary route.

- 🧭 **A* optimality pinned** — `play/pathfind.js` ports `Pathfinding.cs` 1:1 (4-connected, `moveCost`-
  weighted, Manhattan heuristic, step-onto-occupied-goal-but-never-through-bodies) and ships a Dijkstra
  oracle. `pathfind.test.js` (8 checks): straight line, wall detour, unreachable, unwalkable/occupied
  goal, difficult-terrain preference, `start==goal`, **plus a 3,000-map fuzz asserting A* cost == the
  Dijkstra optimum** (and agreement on unreachability). The fuzz caught an edge case on its first run.
- 🧪 Headless verification suite now **139 checks** + the 400-game smoke, all green.

## 👑 v3.68.0 — *"Quests Kept"* — the campaign spine, pinned headless

> Quest progression is the spine of the campaign, and its trickiest behaviour — objectives that complete
> the instant a flag flips, completion vs. *failure* precedence when both land at once — lived only in
> PlayMode, where this repo's CI can't reach. That reactive brain is now pinned in fast headless CI.

- 🧭 **Quest state machine pinned** — `play/quests.js` + `quests.test.js` (15 checks, CI-gated) model
  `QuestManager`/`Quest`: explicit start (once), objectives that fire only when *their* flag flips true
  while Active, `completionFlag` → Completed, `failureFlag` → Failed, **failure beats completion when both
  are true** (the C# checks failure first and `continue`s), Active-only reactions, quest isolation, and
  export → import across managers (the save path).
- 🧪 Headless verification suite now **131 checks** + the 400-game smoke, all green.

## 👑 v3.67.0 — *"The Save Holds"* — pinning the serialization contract

> The least glamorous, most important kind of test: proof that your 20-hour run survives a save/load.
> `SaveSystem` flattens the story brain (GameFlags) into the parallel key/value lists JsonUtility needs;
> a one-element desync there would silently corrupt every save. That contract is now nailed down.

- 💾 **Save round-trip pinned** — `play/save.js` + `save.test.js` (CI-gated): `GameFlags → flattened
  DTO → JSON → back` is lossless across empty state, both bool values, int edges (`0`, `±2³¹`), 250
  keys with **key/value-list lockstep assertions**, dotted/spaced/unicode keys, re-save freshness, and
  double-round-trip stability. Plus 3 on-disk `SaveSystemTests` (re-save overwrites not stacks, a
  200-flag round-trip, int edge values).
- 🧪 Headless verification suite now **116 checks** + the 400-game smoke, all green.

## 👑 v3.66.0 — *"The Odds Against"* — threat forecasts, asset tooling, and the state of the game

> The forecast learns to look both ways: alongside "what are my odds of killing it," the HUD can now
> answer **"what are the odds it kills *me*?"** — exact incoming damage and down-chance from every foe
> in reach, computed by convolving real damage distributions, not guessed. Plus the operational answer
> to "what do I need to ship this?": an asset-completeness checker and an evidence-based report.

- 🛡️ **Threat forecasts** (`ThreatForecast`): mean incoming damage and the true probability the target
  drops this round (miss/crit/save-for-half all modeled). Pinned by `ThreatForecastTests` with a
  40k-seed Monte-Carlo cross-check against the real resolver, mirrored in `play/threat.js` (CI-gated),
  and live in the playable demo ("⚠ incoming ~14 · 38% down").
- 🧰 **`tools/asset-check.sh`**: scans `Resources/` against the exact names the engine loads and reports
  what's present vs missing per category — run it after dropping in any pack.
- 🗺️ **`docs/STATE_OF_THE_GAME.md`**: a full flow audit found the game **complete in code** (23/23
  systems wired, zero stubs, boot → creation → hub → quests → combat → 5 eras → 6 endings); the real
  remaining gaps are a human Unity run, audio+VFX (0%), and a distributable build.

## 👑 v3.65.0 — *"The Odds Made Plain"* — Unity 6.4, the verified slice, and combat forecasts

> The engine moves to **Unity 6000.4.9f1**, the codebase gets its first full audit, and the game
> learns to tell you the truth before you swing: an **XCOM-style forecast** — hit %, crit %, expected
> damage, kill % — on every target, computed from the same math the dice roll. Alongside it, the
> combat and narrative cores now run **outside Unity** in a browser-playable slice, pinned by 100+
> ported tests so nothing can drift silently again.

- 🎯 **Attack forecasts** (`AttackForecast`): analytic hit/crit/damage/lethal preview mirroring
  `AttackResolver` exactly — proven by a Monte-Carlo cross-check (40k seeded rolls per matchup) in
  `AttackForecastTests`. Toggleable via the new `ShowHitChance` setting.
- 🎛️ **QoL settings**: `ShowHitChance`, `ConfirmEndTurn`, `AutoEndTurn`, `AutosaveEnabled` (the
  campaign autosave now respects it), and `ScreenShake` — PlayerPrefs-persisted like the rest.
- 🧭 **Unity 6.4 migration**: every deprecated lookup replaced (`FindAnyObjectByType`, no-arg
  `FindObjectsByType`, `GetInstanceID` dropped), both `CS0252` reference-comparison warnings fixed,
  `ProjectVersion.txt` → 6000.4.9f1 so CI tests on the engine the project ships on.
- 🩺 **The audit**: `CampaignBootstrap` no longer leaks its `OnFlagChanged` handler across
  defeat-restarts (zombie handlers could recruit stale companions into a new run); one-shot
  handlers in `EncounterBuilder`/`PrologueBootstrap`; `BreachDemo` unsubscribes too.
- ⚖️ **Balance canary retuned**: the reference Brute (Str 16, HP 34) brings the oracle into its own
  design bands — **Hero 70% [OK] · Duelist 53% [OK]** (was 95/89 HIGH/HIGH).
- 🕹️ **`play/` — the verified slice**: a browser-playable skirmish (Garrow, Roen, Varra vs the
  Returned) on a seed-faithful port of the combat rules, plus ports of endings, epilogue,
  inventory, and progression. **104 ported checks**, a prose-parity gate, a 500-playthrough fuzz,
  and a 400-game DOM smoke — wired into a new `combat-slice` CI workflow that re-runs whenever
  `Assets/Scripts/**` changes.
- 🧪 **Epilogue coverage**: `EpilogueTests` (17 tests) pins the BG2-style payoff — slide gating,
  quest-outcome priorities, loved-and-lost, the anchor rule, the Chronicle.

## 👑 v3.64.2 — *"The Skull Still Choosing"* — Aldric's fate, named at the forge

> The third cross-era callback, and the tightest loop in the saga: the **Time of Troubles** is the year
> Myrkul dies and his skull is beaten into the **Crown of Horns** — the very relic the gentle heretic
> **Aldric Morn** carries on his shelf in Act I. A new echo at that forge names what *you* saw of Aldric
> over tea, and turns it into a warning about the thing inside the crown.

- ⚒️ **A Keeper of the Bone Crown** (second echo in the Time of Troubles) reads your Act-I read of Aldric
  back to you, most-specific-first:
  - **Sensed the Crown using him** (`aldric.crown_doubt_planted`): "you caught the leash for what it was —
    do not unsee it."
  - **Named him a monster** (`aldric.named_monster`): "hate the relic, not the hand it chose for being kind."
  - **Saw the grieving father** (`aldric.grief_seen`): "love the size of his is the *door* the Crown was built
    to walk through."
  - **Made him say the count** (`aldric.cost_revealed`): "the crown keeps a *different* ledger… the line where
    the price is *him.*"
  - **Merely met him** (`aldric.met`): "the hammer here and the whisper there are the same throat."
  - **Never met him**: pure foreshadow — "a gentle hand will be offered this very crown and told it is a *means.*"
- 🧱 **Reusable plumbing:** `SimpleEra` now supports a **second independent echo slot** (`echoGraph2` /
  `echoLabel2`), so any era can name more than one upstream thread. The Spellplague can pick up a second echo
  later for free.
- 🛡️ **Validated + tested:** the new graph is registered in `ContentValidator` (`era.echo_toot_crown`) and
  `EraEchoesTests` grows to **16 tests** (every branch + priority ordering). **29 suites · ~201 tests.**
  Structural check green (165 C# files).

---

## 🕯️ v3.64.1 — *"The Pulled and the Paid"* — the Breach, named downstream

> A direct extension of v3.64.0's cross-era callbacks: the single heaviest choice in the game — the
> **Breach** at the Wall of the Faithless — now echoes in the late eras, the same way the Crown Wars
> Verdict does. Also closes a reactivity gap: *walking away* from the Wall was previously invisible.

- ⚖️ **The Breach echoes forward.** Both late-era figures (the Time of Troubles **gravedigger** and the
  Spellplague **half-unmade soul**) now layer a third conditional thread on top of the Verdict + Netheril
  ones, keyed to what you did at the Wall:
  - **Pulled Maerin free** (`fugue.pull_maerin`): named as a *trade-death* — "the Wall never gives without
    taking… carry them both, the pulled and the paid."
  - **Walked away** (`fugue.left_maerin`): named as counted restraint — "you did the arithmetic out loud…
    it deserves a marker," and the soul that *would have been the cost* thanks you for it.
- 🩹 **Closed a reactivity gap (ROADMAP Tier 3 "every major choice leaves a trace"):** declining to pull
  Maerin set **no flag** before — the restraint was unwitnessed. It now sets `fugue.left_maerin`, so the
  choice is reactable (and is, immediately, by both echoes).
- 🛡️ **Tests:** `EraEchoesTests` grows to **10 tests** (added pulled/left/neither coverage for both eras).
  **29 suites · ~195 tests.** Structural check green (165 C# files).

---

## ⏳ v3.64.0 — *"The Source Remembers"* — Cross-Era Callbacks (the time-travel earns its keep)

> *A choice in -10,000 DR, spoken aloud in 1385 DR.* The saga's whole conceit is that you walk the
> hinge-points of history — so a verdict at the source should be **named** downstream, with its cost
> made visible. ROADMAP Tier 3 ("no choice forgotten") / DESIGN pillar V. **The last open Tier-3
> reactivity item lands.**

- 🪦 **New `EraEchoes` content** — the *world itself* (not a companion) reacting to an upstream choice:
  - **Time of Troubles (1358 DR):** a **grey gravedigger** working the godless dead while the gods bleed
    speaks the **Crown Wars Verdict** back to you. Argue the damnation *down* (`crownwars.verdict_spared`)
    and the unclaimed get a resting-place "because of that single afternoon — the Wall was only ever a
    wall, never a furnace." Let it *pass* (`crownwars.verdict_passed`) and "the source remembers, even
    when the river forgets."
  - **Spellplague (1385 DR):** a **half-unmade soul** in the blue fire — held together only by being
    *witnessed* — thanks you (spared) or forgives you (passed) for that same verdict, ten thousand years on.
  - Both echoes append a **second thread** if you witnessed **Netheril's fall** (`netheril.cleared/arrived`)
    — the soot of the first apocalypse, the returned shades who "fell once before."
- 🧩 **Reusable wiring:** `SimpleEra.echoGraph` (+`echoLabel`) places the beat as a Talk marker, mirroring
  the existing `witnessGraph` pattern; `CampaignBootstrap` wires both late eras. Built **live from the
  flags**, with a graceful neutral fallback if you witnessed neither upstream era.
- 🛡️ **CI-guarded:** both echo graphs registered in `ContentValidator` (broken-reference gate) and a new
  **`EraEchoesTests`** EditMode suite (**8 tests**) locks each branch — spared / passed / default /
  Netheril-thread — so the no-compiler sandbox still guards the reactivity. **29 suites · ~193 tests.**
- 🟢 Structural check green (**165 C# files**). Roadmap Tier 3 reactivity is now complete.

---

## 🎨 v3.63.0 — *"Status & Ledger"* — Condition Icons + Asset License Ledger (ASSETS)

- ☠️ Added a **12-icon status-effect set** mapped **1:1 to the `Condition` enum** (poisoned, prone,
  stunned, incapacitated, restrained, blinded, frightened, charmed, burning, blessed, hasted, slowed)
  → `Assets/Art/Sprites/Status/game-icons/`, named to the enum for direct wiring. CC BY 3.0,
  attributed in `ATTRIBUTIONS.md` (game-icons set now **57** icons total).
- 📒 Added **`UNITYASSETS.MD`** — an asset **license ledger & buy-later shopping list**: what's already
  in the repo (free, $0) vs. a curated, priced list of **paid** packs to license before launch
  (~$340 art/audio). Documents the legal "test in-editor under your account now, license before
  launch" workflow — **paid/unlicensed assets are NOT committed** (infringement, esp. on a public repo).
- 🟢 No code change; structural check green.

---

## 🎨 v3.62.0 — *"First Real Art"* — Curated CC-BY Icon Set (ASSETS)

- 🎨 Imported a **curated 45-icon set from Game-Icons.net** (CC BY 3.0) — the project's first real
  art drop. Weapons, armor, consumables, spells, loot, and thematic death/grave/holy motifs, for use
  as **ability & item icons**. → `Assets/Art/Sprites/Items/game-icons/` (38) and
  `Assets/Art/Sprites/FX/game-icons/` (7). Original kebab-case names preserved for traceability.
- ⚖️ **License-clean for a public repo:** vendored the CC BY 3.0 text
  (`Assets/Art/Sprites/game-icons-LICENSE.txt`) and added a root **`ATTRIBUTIONS.md`** crediting
  Game-Icons.net and the authors used (Lorc, Delapouite, Sbed, Willdabeast). Committed as text
  (SVG) — no LFS needed. *(Unity: add the Vector Graphics package, or export PNGs.)*
- 📋 Sourced legally within constraints (network is GitHub-only here; pulled from the
  `game-icons/icons` repo, license verified first). See `Assets/FREE_ASSET_SOURCES.md` for the
  CC0-first plan for the rest.
- 🟢 135 scripts / ~19.7k LOC, all clean; 28 test suites; structural check green.

---

## 🧹 v3.61.0 — *"No Echo Twice"* — Codex De-dup + Guard (QA)

- 🐛 Fixed two **duplicate Codex entries** that rendered the same title twice: **"The Echoes"**
  (an early terse version superseded by the richer one — both unlocked at `crownwars.cleared`)
  and **"The Almshouse of the Unclaimed"** (two entries for one place; `AlmshouseScene` sets
  *both* gating flags, so visiting showed it twice). Kept the richer prose in each case and
  pointed the surviving Almshouse entry at the reliable on-entry flag (`lowcity.visited_almshouse`).
- 🛡️ Added a CI guard — `CodexContentTests.Catalog_HasUniqueTitles` — so a duplicate Codex title
  now fails the build. (Found *by* the new EditMode test suite — it's already earning its keep.)
- 📚 Codex: **71 → 69** entries (no content lost — only redundant duplicates removed).
- 🟢 135 scripts / ~19.7k LOC, all clean; 28 test suites (~185 tests), structural check green.

---

## 🎉 v3.60.0 — *"~200 Increments"* — Milestone

- 🎉 Milestone: ~200 self-contained, validated, pushed versions this session (v1.60 → v3.60), all clean.
  Since v3.50 the **gear & onboarding** loops completed (variety pack → vendors → boss drops → inventory/shop
  stats → reputation-reactive fence → starting kit → companions scale on recruit) and **character creation
  got fully informative** (race/class/background summaries, hit dice, a **Randomize** quick-start). Snapshot:
  **135 scripts · ~19.7k LOC · 71 Codex · 33 deeds · 34 banters · 14 items**, zero tech debt, all docs current.
- 🟢 135 scripts / ~19.7k LOC, all clean.

---

## 🎲 v3.59.0 — *"Quick Start"* — Randomize Button at Creation (CODE)

- 🎲 Character creation gains a **"Randomize (quick start)"** button — random race/class/background and a legal
  point-buy spread (the standard array, which costs exactly the 27-point budget) with the class's primary
  stat highest. Dive straight in. `CharacterCreationScreen.RandomizeAll`.
- 🟢 135 scripts / ~19.7k LOC, all clean.

---

## 📋 v3.58.0 — *"Combat Table, Current"* — FEATURES Combat Rows (DOCS)

- 📋 Brought the `FEATURES.md` combat table up to date: Quaff in the action list, flanking **+ cover**, the
  smarter AI (self-preservation, kiting, slot-spending bosses), the high-level kit spells, plus rows for the
  **balance canary** and the Bloodied/camera-follow reads.
- 🟢 135 scripts / ~19.7k LOC, all clean.

---

## 🧙 v3.57.0 — *"Hit Dice Up Front"* — Class Durability at Creation (CODE)

- 🧙 The class summary at creation now shows the **hit die** and average HP-per-level (e.g. *d10 HD (~6
  HP/lvl)*), so you can judge a class's durability alongside its role and kit. `CharacterCreationScreen`.
- 🟢 135 scripts / ~19.7k LOC, all clean.

---

## 🧙 v3.56.0 — *"Know Your Background"* — Background Summary at Creation (CODE)

- 🧙 Character creation now shows a **background summary** too (feature name, skill proficiencies, and flavor
  description) under the picker — so every creation choice (race · class · background) explains itself.
  `CharacterCreationScreen.BackgroundSummary`.
- 🟢 135 scripts / ~19.7k LOC, all clean.

---

## 🧙 v3.55.0 — *"Know Your Class"* — Class Summary at Creation (CODE)

- 🧙 Character creation now shows a one-line **class summary** under the picker — primary stat, martial vs.
  spellcaster, the starting ability, and how the kit *grows* (e.g. *"INT · spellcaster. Starts with Firebolt
  · grows into Ray of Frost, Thunderwave, Fireball"*) — so class choice is informed now that kits deepen by
  level. `CharacterCreationScreen.ClassSummary`.
- 🟢 135 scripts / ~19.7k LOC, all clean.

---

## 🦇 v3.54.0 — *"The Thesis in Miniature"* — The Unbound Maw Bestiary (CONTENT)

- 🦇 **+1 Codex Bestiary entry** (unlocks after the prologue): *The Unbound Maw* — the Cinderhaunt's boss, a
  hunger the harvest fed and forgot to leash, and the saga's thesis met on the first night: a cruelty no one
  built on purpose. *"You will meet its big brother at the end of the world. It is called the Unmade."*
  **71 Codex entries.**
- 🟢 135 scripts / ~19.7k LOC, all clean.

---

## ⚖ v3.53.0 — *"Stats, Not Clutter"* — Refine Companion Scaling (CODE)

- ⚖ Refined `ScaleToHero` to scale only the **level-driven stats** (HP, to-hit, proficiency) and **keep each
  companion's hand-authored ability kit intact** — so Ilfaeril doesn't suddenly sprout a generic javelin on
  recruit, but still joins at the party's level instead of underleveled. (Tightens v3.52.)
- 🟢 135 scripts / ~19.7k LOC, all clean.

---

## ⚖ v3.52.0 — *"No Dead Weight"* — Companions Scale to the Party on Recruit (CODE)

- ⚖ A companion now **scales up to the hero's level when recruited** (never down) — gaining their per-level
  kit abilities and full HP — so a late join (Ilfaeril in the Crown Wars, Maerin at the Wall) isn't a
  level-3/4 liability against a higher-level party. `CampaignBootstrap.ScaleToHero`, applied on both in-game
  recruitment and Continue.
- 🟢 135 scripts / ~19.7k LOC, all clean.

---

## 🎒 v3.51.0 — *"Not Empty-Handed"* — Starting Kit (CODE)

- 🎒 A new hero now begins with a modest **starting kit** — 40 gold, two healing potions, and a set of leather
  armor — so the Returned can engage the economy and survive the Cinderhaunt instead of starting penniless
  and bare. (New game only; Continue is untouched.) `CampaignBootstrap.OnHeroCreated`.
- 🟢 135 scripts / ~19.7k LOC, all clean.

---

## 🎉 v3.50.0 — *"A Fuller Game"* — Checkpoint

- 🎉 Checkpoint. Since v3.40 the **economy & gear loop** got fleshed end-to-end: a 7-piece equipment variety
  pack (weapons, half-plate, shield, helm, ring) using the previously-empty Off-hand/Head/Trinket slots,
  wired into **both vendors** (rep-gated high-end pieces) and as **distinct miniboss drops**, with the
  inventory Off-hand slot + **item stats shown in backpack and shop**, and a **reputation-reactive fence
  greeting**. Plus more lore (the Weave, the Mythallar, the Echoes). All clean, all pushed.
- 🟢 135 scripts / ~19.7k LOC, all clean.

---

## 🪙 v3.49.0 — *"The Fence Warms Up"* — Reputation-Reactive Vendor Greeting (CONTENT)

- 🪙 The Lower City fence's greeting now reacts to your **standing** — the "good *back* shelf" warmth at high
  rep, a cold "don't touch what you can't pay for" at low — instead of one fixed line. The economy reacts to
  reputation in tone as well as stock. `CampaignBootstrap`.
- 🟢 135 scripts / ~19.6k LOC, all clean.

---

## 🪙 v3.48.0 — *"Know What You're Buying"* — Shop Shows Item Stats (CODE)

- 🪙 Vendor buy listings now show each item's **damage / AC** next to the price (parity with the backpack), so
  you can judge gear before spending coin. `ShopScreen`.
- 🟢 135 scripts / ~19.6k LOC, all clean.

---

## 🎒 v3.47.0 — *"Read Before You Wear"* — Item Stats in the Backpack (CODE)

- 🎒 The inventory backpack now shows each item's **stats** inline — weapon damage (e.g. *2d6 Slashing*) or
  armor **+N AC** — so you can compare the new gear at a glance instead of equipping blind. `InventoryScreen.ItemStat`.
- 🟢 135 scripts / ~19.6k LOC, all clean.

---

## 👹 v3.46.0 — *"Boss Loot Worth the Fight"* — Miniboss Gear Drops (CONTENT)

- 👹 The two late minibosses now drop **distinct high-end gear** instead of a chain shirt: the **Avatar of
  Bone** yields **Half Plate** (+5 AC), the **Herald of the Unmade** a **Ring of Protection** (+1 AC, a 240g
  item). The optional fights now pay out a reward you can't easily buy.
- 🟢 135 scripts / ~19.6k LOC, all clean.

---

## 🛡 v3.45.0 — *"Off-Hand Slot"* — Inventory Shows the Shield Slot (CODE)

- 🛡 The inventory (I) equip panel now shows the **Off Hand** slot row, so the new Wooden Shield (and any
  off-hand gear) can be seen and unequipped, not just silently equipped. `InventoryScreen`.
- 🟢 135 scripts / ~19.6k LOC, all clean.

---

## 🪙 v3.44.0 — *"Stocked Shelves"* — New Gear in the Vendors (CONTENT)

- 🪙 Wired the new equipment into both shops: the **fence** now sells a wooden shield, warhammer, iron helm,
  and — at high Lower City standing — **half plate** and a **Ring of Protection** (rep 5/6 rewards); the
  **Docks smuggler** moves a "fell off a ship" rapier and greatsword. Real gear progression you can buy into.
- 🟢 135 scripts / ~19.6k LOC, all clean.

---

## 🎒 v3.43.0 — *"A Fuller Armoury"* — Equipment Variety Pack (CONTENT)

- 🎒 **+7 equippable items** registered into `ItemDatabase` for the loot/vendor pool, using slots that were
  defined but unused: **Warhammer · Rapier · Greatsword** (weapons), **Half Plate** (+5 AC), **Wooden Shield**
  (OffHand +2), **Iron Helm** (Head +1), **Ring of Protection** (Trinket +1) — so gear actually varies and
  the Off-hand/Head/Trinket slots have something to hold. (Vendor wiring next.)
- 🟢 135 scripts / ~19.6k LOC, all clean.

---

## 📖 v3.42.0 — *"Updated the Manual"* — Tactics Codex Refresh (CONTENT)

- 📖 Updated the in-game *"Art of the Duel"* Codex entry to cover the additions it predated: **Quaff (Q)**,
  half-**cover** (+2 AC), the **Bloodied** read, and the reminder that leveled spells cost a slot (L1–L5). The
  player-facing combat manual now matches the combat.
- 🟢 135 scripts / ~19.6k LOC, all clean.

---

## 📚 v3.41.0 — *"The Leash and the Mercy"* — The Weave Lore (CONTENT)

- 📚 **+1 Codex Lore entry** (unlocks after the Spellplague): *The Weave* — Mystra's mediating lattice, torn
  twice in the saga (Karsus, then the Spellplague), and the pattern the Returned can't unsee: every
  catastrophe is a proud hand reaching past the leash to grab power directly — *the Wall is the same reach
  aimed at the dead.* **70 Codex entries.**
- 🟢 135 scripts / ~19.6k LOC, all clean.

---

## 🎉 v3.40.0 — *"Witnessed Everywhere"* — Checkpoint

- 🎉 Checkpoint. Since v3.0 the reactive web deepened: the **two late minibosses now cast the new high-level
  AoE spells** (Flame Strike / Ice Storm, granted real slots → cast twice, then melee), **all four optional
  minibosses gained a companion-witness fireside beat** (Naeve/Garrow/Ilfaeril+Garrow/Ilfaeril+Naeve),
  **both faction reputations** got reactive hub figures + epilogue slides, and the README front door was
  refreshed to match. Snapshot: **135 scripts · ~19.6k LOC · 69 Codex · 32 deeds · 34 banters · 13
  night-talks · 11 keepsakes**, all clean & pushed.
- 🟢 135 scripts / ~19.6k LOC, all clean.

---

## 🦇 v3.39.0 — *"Not Clones — Arguments"* — The Echoes Bestiary (CONTENT)

- 🦇 **+1 Codex Bestiary entry**: *The Echoes* — the Mirror's twisted versions of your own party (the choices
  you didn't make, your companions as they'd be if you'd failed them), and the Last Returned who kneels
  rather than falls, checking whether *this* time you know how it ends. **69 Codex entries.**
- 🟢 135 scripts / ~19.6k LOC, all clean.

---

## 📋 v3.38.0 — *"Front Door II"* — README Narrative Highlights (DOCS)

- 📋 Refreshed the `README.md` narrative-highlights box to reflect the real depth — 6 companions with
  romances & personal quests, 34 reactive banters, the 4 branching moral micro-quests, faction loops, and
  New-Game+ — instead of the early-vertical-slice description.
- 🟢 135 scripts / ~19.6k LOC, all clean.

---

## 📋 v3.37.0 — *"Front Door"* — README Combat Highlights + Badge (DOCS)

- 📋 Refreshed the `README.md` combat-highlights box to reflect the actual tactical depth (the full action
  menu, opportunity attacks / flanking / cover, the smarter AI incl. kiting, the balance canary) and
  corrected the stale script badge to 135 / ~19.6k LOC. The project's front door now matches the game.
- 🟢 135 scripts / ~19.6k LOC, all clean.

---

## 🎖 v3.36.0 — *"Archivist"* — Higher Codex Deed (CONTENT)

- 🎖 New **Deed** — *"Archivist of the Returned"* — for filling fifty Codex entries (the Codex now holds 68),
  a deeper completion incentive above Loremaster's thirty. **32 deeds.**
- 🟢 135 scripts / ~19.6k LOC, all clean.

---

## 📚 v3.35.0 — *"Borrowed Power, Uncounted"* — The Mythallar Lore (CONTENT)

- 📚 **+1 Codex Lore entry** (unlocks after Netheril): *The Mythallar* — the captured magic-node that raised
  Netheril's flying cities and doomed them when Karsus reached for godhood; the saga's recurring lesson that
  borrowed power, uncounted, always comes due — and never to the one who borrowed it. **68 Codex entries.**
- 🟢 135 scripts / ~19.6k LOC, all clean.

---

## 🔥 v3.34.0 — *"This Time I Chose It"* — Naeve After the Colossus (CONTENT)

- 🔥 **+1 `CampGroupBanter`** (gated on `netheril.boss_down`): Naeve on the Mythallar Colossus — her people's
  flawless guardian for a treasure that was a bomb, "no purer portrait of Netheril" — and the relief of
  watching her home fall a second time *with a hand on the wheel.* **All four optional minibosses now have a
  companion-witness fireside beat.** **34 fireside exchanges.**
- 🟢 135 scripts / ~19.6k LOC, all clean.

---

## 🔥 v3.33.0 — *"Forced to Kill the Same Person Twice"* — Ilfaeril After the First Unmade (CONTENT)

- 🔥 **+1 `CampGroupBanter`** (gated on `crownwars.boss_down`): Ilfaeril, who once voted to erase a soul and
  tonight had to raise a blade against the grief he made of it — "there is a word for being forced to kill the
  same person twice; I have never found it." Garrow tells him he did it clean: the mercy he owed it the first
  time. **33 fireside exchanges** — three of the four minibosses now have a companion-witness beat.
- 🟢 135 scripts / ~19.6k LOC, all clean.

---

## 🔥 v3.32.0 — *"For Now Is All Anyone Gets"* — Ilfaeril & Naeve After the Herald (CONTENT)

- 🔥 **+1 `CampGroupBanter`** (gated on `spellplague.herald_down`): the two world-enders reckon with the Herald
  of the Unmade — grief given a sword, "our arithmetic still running, refusing to round down to zero" — and
  Naeve admits she'd trade every theorem she proved for a single reliable *for now,* which the Returned keeps
  handing her. Both late minibosses now have a companion-witness fireside beat. **32 fireside exchanges.**
- 🟢 135 scripts / ~19.6k LOC, all clean.

---

## 🔥 v3.31.0 — *"A Debt Back With Interest"* — Garrow After the Avatar of Bone (CONTENT)

- 🔥 **+1 `CampGroupBanter`** (gated on `toot.avatar_down`): after felling the Avatar of Bone — a scrap of
  Myrkul, the tyrant-god Kelemvor replaced — Garrow doesn't feel horror but *tired*, and admits she's begun
  to wonder if the Returned is here to teach her order a better way to carry the dead. "They'd call it heresy.
  They'd be right." **31 fireside exchanges.**
- 🟢 135 scripts / ~19.6k LOC, all clean.

---

## 👹 v3.30.0 — *"The Boss Casts Too"* — Minibosses Wield the New AoE Spells (CODE)

- 👹 The two late-era optional minibosses now **open with a high-level AoE** before reverting to melee: the
  **Avatar of Bone** calls down **Flame Strike**, the **Herald of the Unmade** a freezing **Ice Storm** — each
  granted real spell slots and the spell at the front of its kit so the cluster-aware AI actually fires it
  (and runs dry after a couple casts). The deeper magic now cuts both ways. `CampaignBootstrap`.
- 🟢 135 scripts / ~19.5k LOC, all clean.

---

## 🧹 v3.29.0 — *"Room for the Spellbook"* — Wider Ability Bar (CODE)

- 🧹 Widened the combat ability bar (900→1180px, min button 120→108) so a high-level caster's full kit (a
  level-9 Cleric now has nine abilities) fits without clipping. Follows the deeper class kits from v3.27–28.
  `CombatHUD`.
- 🟢 135 scripts / ~19.5k LOC, all clean.

---

## ⚔ v3.28.0 — *"Martial Parity"* — High-Level Strikes for Martials (CONTENT)

- ⚔ Gave the martial classes high-level single-target unlocks for parity with the casters' deeper kits:
  **Fighter** Heavy Strike (2d8), **Barbarian** Brutal Strike (2d6 + Frightened), **Rogue** Sneak Attack
  (3d6), **Ranger** Hunter's Volley (2d6 ranged) — no friendly-fire, just power as you level. Every class now
  keeps growing.
- 🟢 135 scripts / ~19.5k LOC, all clean.

---

## 📖 v3.27.0 — *"Deeper Spellbooks"* — High-Level Class Unlocks (CONTENT)

- 📖 Extended the **Wizard** and **Cleric** per-level kits with real high-level spells: **Hold Person** (a new
  *Held*/incapacitate condition), **Spiritual Weapon** (bonus-action force), **Ice Storm** & **Cone of Cold**
  & **Flame Strike** (save-for-half AoE bursts the cluster-aware AI also respects), and **Cure Wounds III**.
  Casters now keep unlocking meaningful options as they level. (Phase B: more abilities/spells.)
- 🟢 135 scripts / ~19.5k LOC, all clean.

---

## 📋 v3.26.0 — *"Documented Factions"* — FEATURES Faction Loop (DOCS)

- 📋 Updated the `FEATURES.md` faction-reputation row to describe the now-complete loop for each tracked
  faction (content → reactive hub figure → Journey standing → deed → epilogue → ending gate).
- 🟢 135 scripts / ~19.5k LOC, all clean.

---

## ⚖ v3.25.0 — *"Where You Stood"* — Faction Epilogue Slides (CONTENT)

- ⚖ The **Doomguides** and the **Faithless Choir** now get epilogue slides keyed to your standing — the church
  that called you a *question* (or never blessed your name), and the grief that made you its prophet. Faction
  reputation now echoes at the very end, not just in the Journey. `EndingResolver.Epilogue`.
- 🟢 135 scripts / ~19.5k LOC, all clean.

---

## 🎖 v3.24.0 — *"Standing With the Powers"* — Faction-Rep Deeds (CONTENT)

- 🎖 **+2 Deeds** for high faction standing: *"Marked as a Question"* (Kelemvor's Doomguides) and *"The Choir
  Sings Your Name"* (the Faithless Choir) — rewarding the faction paths the new hub figures react to. **31 deeds.**
- 🟢 135 scripts / ~19.5k LOC, all clean.

---

## 🜍 v3.23.0 — *"The Choir Sings Your Name"* — Faction-Gated Choir Figure (CONTENT)

- 🜍 A parallel **Faithless Choir** sympathizer now appears in the hub once the Choir has noticed you (faction
  rep ≠ 0), with four standing-tiers — from prophesied-for (they sing your name; remember them at the Court)
  to threatened (the grief you silenced grew teeth; mind your back). Both faction reputations beyond the poor
  now surface as reactive hub figures. (Phase C.)
- 🟢 135 scripts / ~19.5k LOC, all clean.

---

## ⚖ v3.22.0 — *"The Church Has an Opinion"* — Faction-Gated Doomguide (CONTENT)

- ⚖ A **Doomguide of Kelemvor** now appears in the hub once the church has noticed you (faction rep ≠ 0), with
  four standing-tiers of reaction — from honored (Vayle has marked your name as a *question*) to marked (walk
  small near a Doomguide). First content **gated off a faction reputation** beyond the poor. (Phase C.)
- 🟢 135 scripts / ~19.5k LOC, all clean.

---

## 📍 v3.21.0 — *"The Lady's Margins"* — Vault Location Banner (CODE)

- 📍 The **Vault of Tens** riddle room now sets its location banner too (*"the Lady's margins"*), completing
  the banner across literally every explorable mode. `CampaignBootstrap`.
- 🟢 135 scripts / ~19.4k LOC, all clean.

---

## 🎉 v3.20.0 — *"The Reactive Web"* — Checkpoint

- 🎉 Checkpoint: the Lower City's **four branching moral beats** (ferryman / urchin / deathbed / informant)
  are each a complete loop — choice → reputation/conscience → Journey → epilogue — and **each now triggers a
  companion's fireside reaction** (Roen, Garrow, Maerin, Ilfaeril, Varra), several keyed to *which way* you
  chose. Snapshot: **135 scripts · ~19.4k LOC · 67 Codex · 29 deeds · 30 banters · 13 night-talks · 11
  keepsakes**, plus deed/Codex achievement pops, level-up & crit cues, and location banners across every mode.
- 🟢 135 scripts / ~19.4k LOC, all clean.

---

## 🔥 v3.19.0 — *"Some of Them Walk"* — Varra on the Freed Informant (CONTENT)

- 🔥 **+1 `CampGroupBanter`** giving the informant's *free* path its companion echo: Varra — sold and leashed
  at six — on what it means that you opened the door and let a frightened woman *decide*. "It's not the
  collar that breaks you. It's that they never let you choose the collar." Both informant outcomes now have
  a fireside reaction (Ilfaeril for turn, Varra for free). **30 fireside exchanges.**
- 🟢 135 scripts / ~19.4k LOC, all clean.

---

## 📜 v3.18.0 — *"The Verse the Poor Sing Twice"* — Balladeer Folds In the Mercies (CONTENT)

- 📜 The hub balladeer's broadside now adds a verse — written by the Quarter itself, the only true one in the
  ballad — if you pulled old Pell from the river or bought the child's hand back from the Fist: about a dead
  thing that did kindnesses and never stopped to be thanked. The small choices become legend. `BaldursGateHub`.
- 🟢 135 scripts / ~19.4k LOC, all clean.

---

## 📚 v3.17.0 — *"She Is Not Wrong"* — High Doomguide Vayle Lore (CONTENT)

- 📚 **+1 Codex Lore entry** (unlocks at the Crown Wars): *High Doomguide Vayle* — the finale's iron
  conscience, who defends the Wall with the most dangerous argument there is (she is not wrong about the
  alternatives), and the part of you that knows the cost of mercy. Documents the Doomguide's-Peace ending's
  opposition. **67 Codex entries.**
- 🟢 135 scripts / ~19.4k LOC, all clean.

---

## 📚 v3.16.0 — *"Just Paperwork"* — The Ash Pact Lore (CONTENT)

- 📚 **+1 Codex Lore entry** (unlocks on recruiting Varra): *The Ash Pact* — the infernal contract that owns
  her soul, and the saga's recurring horror: how often the worst things (the Wall, the tithe, the Pact) wear
  the same defense — *it's just a contract, just the rules, just paperwork.* **66 Codex entries.**
- 🟢 135 scripts / ~19.4k LOC, all clean.

---

## 🏆 v3.15.0 — *"By Name"* — Deed Pop Names the Deed (CODE)

- 🏆 The deed pop now **names the deed earned** (*"🏆 Deed earned — \"Master Tactician\""*) by tracking which
  predicate just flipped, instead of a bare count — clearer, prouder feedback. `CodexNotifier`.
- 🟢 135 scripts / ~19.4k LOC, all clean.

---

## 🔊 v3.14.0 — *"Level-Up Chime"* — Level-Up Sound Cue (CODE)

- 🔊 Leveling up now plays a *"levelup"* SFX cue alongside the floating burst (art-optional, silent without a
  clip). `EncounterController`.
- 🟢 135 scripts / ~19.4k LOC, all clean.

---

## 🔊 v3.13.0 — *"Ding"* — Deed-Pop Sound Cue (CODE)

- 🔊 Earning a deed now plays a *"deed"* SFX cue alongside the pop (art-optional — silent until a clip lands
  in `Resources/SFX/`). `CodexNotifier`.
- 🟢 135 scripts / ~19.4k LOC, all clean.

---

## 🏆 v3.12.0 — *"Deed Earned!"* — Achievement Pops (CODE)

- 🏆 The progress notifier now also pops *"🏆 Deed earned! (N/total)"* the moment you earn a deed (taking
  precedence over the Codex toast) — a satisfying, classic achievement beat for the meta-layer.
  `CodexNotifier` now watches both Codex unlocks and deeds.
- 🟢 135 scripts / ~19.4k LOC, all clean.

---

## 📖 v3.11.0 — *"New Lore Unlocked"* — Codex Toast (CODE)

- 📖 New `CodexNotifier` persistent overlay shows a brief *"📖 New Codex entry — press K to read"* toast when
  the count of unlocked entries grows during play (polled twice a second), so the saga's revealed lore
  doesn't slip by. Registered alongside the other persistent overlays.
- 🟢 135 scripts / ~19.4k LOC, all clean.

---

## 📋 v3.10.0 — *"Documented Choices"* — Moral Micro-Quests in FEATURES (DOCS)

- 📋 Added a `FEATURES.md` row for the four **branching moral micro-quests** (skiff / urchin / deathbed /
  informant) and their full choice → reputation → Journey → epilogue → companion-reaction loops.
- 🟢 134 scripts / ~19.3k LOC, all clean.

---

## 🎖 v3.9.0 — *"The Quarter's Conscience"* — Moral-Beats Deed (CONTENT)

- 🎖 New **Deed** — *"The Quarter's Conscience"* — for facing all four of the Lower City's hard small choices
  (ferryman, urchin, deathbed, informant), whichever way you chose. **29 deeds.**
- 🟢 134 scripts / ~19.3k LOC, all clean.

---

## 📖 v3.8.0 — *"Handoff Counts"* — CONTINUITY Refresh (DOCS)

- 📖 Updated the `CONTINUITY.md` "Since v1.60" summary with current totals (65 Codex · 28 deeds · 29 banters)
  and the **four branching moral micro-quests** (ferryman / urchin / deathbed / informant), each a full loop
  down to a companion's fireside reaction — so the handoff stays accurate.
- 🟢 134 scripts / ~19.3k LOC, all clean.

---

## 🔥 v3.7.0 — *"Count the Small Ones"* — Ilfaeril on the Turned Informant (CONTENT)

- 🔥 **+1 `CampGroupBanter`** (new `safehouse.informant_turned` flag): if you leaned on the informant until she
  broke, Ilfaeril — who voted a people into the Wall ten thousand years ago — hears the *reasonableness* of it
  and begs Roen to count the small decisions, "because it is always one snitch, one vote… and ten thousand
  years later you cannot find the place where it became a Wall." The informant beat now has its companion
  echo too. **29 fireside exchanges.**
- 🟢 134 scripts / ~19.3k LOC, all clean.

---

## 🔥 v3.6.0 — *"It's a Shape, Not a Reason"* — Maerin on the Saved Ferryman (CONTENT)

- 🔥 **+1 `CampGroupBanter`** completing the moral-beat→companion web: if you went in the river after old Pell,
  Maerin (whom *you* pulled from the Wall) finally understands why anyone would — "he doesn't have a version
  of himself that walks past the drowning. It's not a reason. It's a *shape.*" Garrow: write it with the
  names. **28 fireside exchanges.**
- 🟢 134 scripts / ~19.3k LOC, all clean.

---

## 🔥 v3.5.0 — *"Comfort Is a Tool Too"* — Garrow on the Deathbed Lie (CONTENT)

- 🔥 **+1 `CampGroupBanter`**: if you gave Old Hensley the kind lie, Naeve presses Garrow on it, and the
  Doomguide — who's given ten thousand last rites — answers: the dying don't need *accurate*; comfort isn't
  a lie if you'd die to make it true, and for that old man, she would have. **27 fireside exchanges.**
- 🟢 134 scripts / ~19.3k LOC, all clean.

---

## 🔥 v3.4.0 — *"Pay It Back Up the Line"* — Roen Reacts to the Freed Urchin (CONTENT)

- 🔥 **+1 `CampGroupBanter`** tying the new Market beat to a companion: if you bought the pickpocket child's
  hand back from the Fist, Roen (who *was* that kid, thirty years and a finger ago) resolves to find her,
  slip her coins and a name to ask for — pay it back up the line. "Don't tell anyone. Ruins the brand."
  **26 fireside exchanges.**
- 🟢 134 scripts / ~19.2k LOC, all clean.

---

## 🌀 v3.3.0 — *"She Keeps Re-Reading"* — New-Game+ Epilogue Slide (CONTENT)

- 🌀 On a New-Game+ run, the epilogue now closes with a meta slide: outside the page, the Lady shuts the book
  and reaches for the first chapter again — *each time the ink runs a little kinder.* The loop, acknowledged
  at the very end. `EndingResolver.Epilogue`.
- 🟢 134 scripts / ~19.2k LOC, all clean.

---

## 📍 v3.2.0 — *"Room for the Name"* — Wider Location Banner (CODE)

- 📍 Widened the exploration location banner (340→460px) so the longer era names (e.g. *Netheril — the
  Falling Sky (−339 DR)*) fit without clipping. `ExplorationHUD`.
- 🟢 134 scripts / ~19.2k LOC, all clean.

---

## 🕸️ v3.1.0 — *"The Dignity of Deciding"* — Harper Safehouse Informant Beat (CONTENT)

- 🕸️ A fourth **branching moral beat**, completing one per Lower City room: at the Wandering Niche, a captured
  Fist informant — **turn her** (she spies, and hates you, and a family pays; "how the Wall gets built, one
  reasonable decision at a time") or **cut her loose** (give her the choice no one's given her; maybe she
  turns anyway, on her own terms). Full loop — choice → Journey → epilogue. Every room now holds a real choice.
- 🟢 134 scripts / ~19.2k LOC, all clean.

---

## 🎉 v3.0.0 — *"The Crown of Horns, v3"* — Milestone

- 🎉 **Milestone v3.0** — ~140 self-contained, validated, pushed versions since v1.60, all clean. Snapshot:
  **134 C# scripts · ~19.1k LOC · 65 Codex entries · 28 deeds · 25 fireside banters · 13 night-talks · 11
  keepsakes**. Since v2.0 the world gained **three new branching moral beats** (the sinking skiff, the
  pickpocket child, Old Hensley's deathbed) — each a full loop (choice → reputation/conscience → Journey →
  epilogue) — plus ranged-enemy **kiting**, **cover**, a **balance canary**, camera **auto-focus**, location
  **banners** across every mode, spell-slot & potion HUD readouts, a scrollable Journey, and a pile of
  reactive flavor. Corrected the LOC badges to the true ~19.1k. ROADMAP status refreshed.
- 🟢 134 scripts / ~19.1k LOC, all clean.

---

## 📍 v2.99.0 — *"Beneath the Gate"* — Cinderhaunt Location Banner (CODE)

- 📍 The prologue **Cinderhaunt** dungeon now sets its location banner too (*"beneath the Gate"*), completing
  the location-banner coverage across every explorable mode. `CampaignBootstrap`.
- 🟢 134 scripts / ~20.0k LOC, all clean.

---

## 🦇 v2.98.0 — *"Fellow-Travellers"* — Cinderhaunt Bestiary Entry (CONTENT)

- 🦇 **+1 Codex Bestiary entry** (unlocks after the prologue): *The Cinderhaunt Restless* — the harvest's
  other dead, who rose with you and didn't make it all the way out; you put them down with a tenderness that
  surprises you. The starting area finally has its bestiary note. **65 Codex entries.**
- 🟢 134 scripts / ~20.0k LOC, all clean.

---

## 📜 v2.97.0 — *"Scroll the Saga"* — Journey Screen Scrolls (CODE)

- 📜 The **Journey (J)** screen now scrolls — with the growing Lower City list (now eight beats incl. the
  ferryman, urchin, and deathbed), eras, threads, bonds, field record, standings, and the Lady, it had
  outgrown a fixed panel. `JourneyScreen`.
- 🟢 134 scripts / ~20.0k LOC, all clean.

---

## 🕯️ v2.96.0 — *"The Kindest Lie or the Harder Truth"* — Almshouse Deathbed Beat (CONTENT)

- 🕯️ A third, intimate **branching beat** at the Almshouse — Old Hensley, dying, begs to know if his estranged
  son forgave him (he never came). **Give the kind lie** (he goes out smiling, holding a forgiveness that
  wasn't sent) or **the harder truth** ("you *asked* — that counts"; he dies holding something that's *his*).
  No reputation — just your conscience. Full loop: choice → Journey → epilogue. Crosses **20k LOC**.
- 🟢 134 scripts / ~20.0k LOC, all clean.

---

## 🎖 v2.95.0 — *"Small Mercies"* — Mercy Deed (CONTENT)

- 🎖 New **Deed** — *"Small Mercies"* — for both pulling the ferryman's passenger from the river and buying the
  child's hand back from the Fist. **28 deeds.**
- 🟢 134 scripts / ~19.9k LOC, all clean.

---

## 🍎 v2.94.0 — *"A Small Immortality"* — Urchin Beat Wired to Journey & Epilogue (CONTENT)

- 🍎 The Market pickpocket beat now tracks in the **Journey** and echoes in the **epilogue**: the saved child
  grows crooked-honest and names her first child after you (a small, honest immortality); the abandoned one
  learns the city's early lesson — expect nothing. Both new moral beats are now full loops (choice → rep →
  Journey → ending).
- 🟢 134 scripts / ~19.9k LOC, all clean.

---

## 🍎 v2.93.0 — *"A Finger or the Cells"* — Market Pickpocket Moral Beat (CONTENT)

- 🍎 A second **branching side-interaction**, at the Shrunken Quarter: a Flaming Fist has a child thief by the
  collar — a finger for a first theft, or the Seatower. **Stand surety** (rep **+2**; the child looks back,
  deciding the world might hold mercy) or **let the law take them** (rep **−2**; they don't even cry, which
  is the part that stays). Reactive aftermath; the saved child later shows you a bought apple. (Phase C.)
- 🟢 134 scripts / ~19.9k LOC, all clean.

---

## 🛶 v2.92.0 — *"Old Pell, Twelve More Years"* — Ferryman Epilogue Slide (CONTENT)

- 🛶 The Docks ferryman choice now echoes in the **endgame epilogue**: saved Old Pell lives twelve more years
  (and swears you walked on water); walked-on, another dockhand went in where you wouldn't — a small weight
  you carry anyway. `EndingResolver.Epilogue`.
- 🟢 134 scripts / ~19.8k LOC, all clean.

---

## 📜 v2.91.0 — *"Logged"* — Ferryman Beat in the Journey (CODE)

- 📜 The Docks ferryman moral beat now appears in the **Journey (J)** Lower City list once resolved (*"you
  went in after him"* / *"you walked on"*), so the choice and its standing-shift are tracked like the other
  side beats. `JourneyScreen`.
- 🟢 134 scripts / ~19.8k LOC, all clean.

---

## 🛶 v2.90.0 — *"A Skiff Going Under"* — Docks Ferryman Moral Beat (CONTENT)

- 🛶 A new **branching side-interaction** at the Chionthar Docks: a frantic ferryman whose skiff is sinking
  with old Pell aboard. **Jump in** (Lower City rep **+2**, the dockfolk see who comes for the written-off)
  or **walk on** (rep **−1**; someone else jumps — the quarter keeps its own). Reactive aftermath either
  way. A real choice with a consequence, on the proven multi-node dialogue pattern. (Phase C side quests.)
- 🟢 134 scripts / ~19.8k LOC, all clean.

---

## 🎴 v2.89.0 — *"Rotating Epigraph"* — Main-Menu Flavor (CODE)

- 🎴 The title screen now shows a **random epigraph** from a small pool of the saga's best lines (the soul
  question, "the dead do not come back — you did," the Wall-is-a-choice, love-as-a-stake…) instead of one
  fixed quote. `MainMenu`.
- 🟢 134 scripts / ~19.8k LOC, all clean.

---

## 📚 v2.88.0 — *"Filling Yourself In"* — The Returned's Codex Entry (CONTENT)

- 📚 **+1 Codex Premise entry** (unlocks at the Breach): *The Returned, Becoming* — on the protagonist's arc
  from an accident the universe asked to a choice it must answer; you fill yourself in deed by deed with the
  one thing the Wall can't erase. **64 Codex entries.**
- 🟢 134 scripts / ~19.8k LOC, all clean.

---

## 🏆 v2.87.0 — *"Deeds Done"* — Deeds Tally in the Chronicle (CODE)

- 🏆 The **Chronicle (C)** and finale recap now include your **deeds earned** count (X/Y) alongside battles,
  endings, and standings. `EndingResolver.Chronicle`.
- 🟢 134 scripts / ~19.7k LOC, all clean.

---

## 💤 v2.86.0 — *"Rest Up"* — Low-HP Nudge in Exploration (CODE)

- 💤 The exploration party panel now tints a wounded member's HP red and shows a *"Rest at camp to heal"*
  nudge when anyone's below half — so you don't wander into the next fight bloodied without realizing.
  `ExplorationHUD`.
- 🟢 134 scripts / ~19.7k LOC, all clean.

---

## 🪙 v2.85.0 — *"Say Our Names Slow"* — Token-Aware Almshouse Poor (CONTENT)

- 🪙 If you carry Mother Cass's backwards token, the Almshouse's unclaimed now recognize it on your wrist —
  *"You're carrying us. To the place that judges... say our names slow. Make it listen."* The plea you bear
  to the Court has a face now. `AlmshouseScene.Poor`.
- 🟢 134 scripts / ~19.7k LOC, all clean.

---

## 🏹 v2.84.0 — *"Kiting"* — Ranged Enemies Step Back From Melee (CODE)

- 🏹 A ranged enemy (range ≥ 3) cornered in melee now **steps back one tile to open distance** before firing,
  instead of shooting point-blank — proper kiting that makes archers and casters play like archers and
  casters (and yes, it provokes an opportunity attack, just like it would for you). `EncounterController.FindRetreatCell`.
- 🟢 134 scripts / ~19.7k LOC, all clean. (Phase B: smarter enemy AI — kiting)

---

## 📚 v2.83.0 — *"The Kind God Who Stopped Trying"* — Kelemvor Lore (CONTENT)

- 📚 **+1 Codex Lore entry** (unlocks at the Fugue): *Kelemvor, Lord of the Dead* — the once-mortal god who
  inherited the Wall and couldn't make the machine kind, and the question the saga puts to him: *why keep a
  cruelty you inherited, simply because you inherited it?* **63 Codex entries.**
- 🟢 134 scripts / ~19.7k LOC, all clean.

---

## 🔮 v2.82.0 — *"Slots Left"* — Spell-Slot Readout in the HUD (CODE)

- 🔮 The active-unit panel now shows a compact **spell-slot readout** for casters — filled/empty dots per
  level (e.g. *Slots L1 ●●○ L2 ●○*) — so you can see what leveled spells you can still cast before arming
  one. `CombatHUD.SlotsLine`.
- 🟢 134 scripts / ~19.7k LOC, all clean.

---

## 📍 v2.81.0 — *"You Are Here"* — Location Banner (CODE)

- 📍 Exploration now shows a top-center **location banner** (`ExplorationHUD.Location`) — set by the hub, the
  four Lower City rooms, all four eras, and the Fugue — so with the world's growing breadth you always know
  *where* you are. `ExplorationHUD` + scene/bootstrap wiring.
- 🟢 134 scripts / ~19.7k LOC, all clean.

---

## 🔢 v2.80.0 — *"Foes Left"* — Remaining-Enemy Count in the HUD (CODE)

- 🔢 The combat HUD's initiative header now shows how many **foes remain** alongside the round number, so you
  can read at a glance how close the fight is to won. `CombatHUD`.
- 🟢 134 scripts / ~19.7k LOC, all clean.

---

## 📖 v2.79.0 — *"Handoff Refresh"* — CONTINUITY.md Brought Up to Date (DOCS)

- 📖 Refreshed the `CONTINUITY.md` "Where we are" brief with a concise *Since v1.60* summary (the tactical
  combat layer, four rooms + two vendors, accessibility/meta, content totals, and the explicitly-deferred
  items) so the next session has an accurate handoff.
- 🟢 134 scripts / ~19.7k LOC, all clean.

---

## 📚 v2.78.0 — *"The Crack in Time"* — Time-Travel Lore (CONTENT)

- 📚 **+1 Codex Lore entry** (unlocks at your first era): *The Crack in Time* — you don't time-travel so much
  as fall through your own broken seam into the moments the Wall was made, and *witness* is a verdict all its
  own. **62 Codex entries.**
- 🟢 134 scripts / ~19.6k LOC, all clean.

---

## ⚓ v2.77.0 — *"The River Remembers"* — Era-Aware Dockhand (CONTENT)

- ⚓ Once you've walked three or more eras, the Chionthar dockhand has a new line — the wharves tell a story
  that you've *sailed time*, and the river's been restless since you came, "like it remembers somewhere it
  used to flow." `DocksScene.Dockhand`.
- 🟢 134 scripts / ~19.6k LOC, all clean.

---

## 🎥 v2.76.0 — *"Or Don't"* — Camera-Follow Toggle (CODE)

- 🎥 The new combat camera-follow can be turned **off** in Options for players who prefer full manual control.
  `GameSettings.CameraAutoFocus`, persisted; the HUD's focus hook respects it.
- 🟢 134 scripts / ~19.6k LOC, all clean.

---

## 🎥 v2.75.0 — *"Eyes On"* — Camera Auto-Focuses the Active Combatant (CODE)

- 🎥 The combat camera now **glides to center on whoever's turn it is** — friend or foe — so you never lose
  track of the action off-screen. `IsometricCameraController.FocusOn` (a gentle lerp, instantly cancelled
  the moment you pan manually), hooked to `TurnManager.OnTurnStarted` via the HUD.
- 🟢 134 scripts / ~19.6k LOC, all clean.

---

## 👀 v2.74.0 — *"Both, Love"* — Whole-Company Onlooker Beat (CONTENT)

- 👀 The gawking-onlookers examinable gains a culminating line once you've gathered **all five** companions: a
  child asks whether you and your strange friends are heroes or monsters, and her mother answers — *"Both,
  love. The best ones always are."*
- 🟢 134 scripts / ~19.6k LOC, all clean.

---

## 📋 v2.73.0 — *"Honest Codex II"* — FEATURES.md Narrative/Accessibility Sync (DOCS)

- 📋 Refreshed stale `FEATURES.md` lines: Codex (~61 entries, broadened), night-talks (now 6 companions incl.
  Maerin + second talks + NG+, 13 total), Options (UI-scale/combat-speed/floating-text/legacy reset), and
  marked the accessibility options done.
- 🟢 134 scripts / ~19.5k LOC, all clean.

---

## 🔁 v2.72.0 — *"Twice-Told"* — New-Game+ Deed (CONTENT)

- 🔁 New **Deed** — *"Twice-Told"* — earned by beginning the saga again on a New Game+ (`ng.plus`). **27 deeds.**
- 🟢 134 scripts / ~19.5k LOC, all clean.

---

## 🧹 v2.71.0 — *"Room to Breathe"* — Taller Options Panel (CODE)

- 🧹 Bumped the Options panel height (600→680) so the now-fuller settings list (difficulty, floating-text,
  banter, volume, text, UI-scale, combat-speed, tips/legacy resets) doesn't crowd the Close button.
- 🟢 134 scripts / ~19.5k LOC, all clean.

---

## ♿ v2.70.0 — *"Quieter Numbers"* — Toggle Floating Combat Text (CODE)

- ♿ New accessibility option: **turn off floating combat numbers & words** (damage pops, FLANKED/COVER/
  BLOODIED/LEVEL UP, etc.). `GameSettings.ShowFloatingText`, honoured at the single `FloatingCombatText.Spawn`
  chokepoint and persisted via PlayerPrefs — for players who find the motion/clutter distracting.
- 🟢 134 scripts / ~19.5k LOC, all clean.

---

## ⚖ v2.69.0 — *"Two Canaries"* — Second Balance Matchup (CODE)

- ⚖ The combat-balance canary now runs **two** seeded matchups — a sturdy Hero vs Brute (should usually win)
  and a frail glass-cannon Duelist vs Brute (should be ~coin-flip) — each with its own OK/HIGH/LOW verdict,
  broadening the tripwire's coverage of the resolver math. `CombatBalance.Report`.
- 🟢 134 scripts / ~19.5k LOC, all clean.

---

## 💞 v2.68.0 — *"The Whole Niche Has a Wager"* — Romance-Reactive Harper Handler (CONTENT)

- 💞 The Harper safehouse handler now has a special line if you've sealed the **Roen romance** — the lodge
  has clocked why their slipperiest agent started turning down deep-cover jobs, and they've a wager on you.
  *Be good to him; he's terrible at being happy.*
- 🟢 134 scripts / ~19.5k LOC, all clean.

---

## ✨ v2.67.0 — *"Level Up!"* — Floating Level-Up Burst (CODE)

- ✨ A unit that levels up after a victory now pops a green *"LEVEL UP!"* over its head, so progression is a
  visible beat, not just a log line. `EncounterController`.
- 🟢 134 scripts / ~19.5k LOC, all clean.

---

## 🎭 v2.66.0 — *"The Wall Is a Choice"* — Choir-Reactive Market Slogan (CONTENT)

- 🎭 The Shrunken Quarter gains a **chalked Choir slogan** that reads three different ways depending on how
  you resolved the Faithless Choir: amended to hope (*THE WALL IS A CHOICE. WE GRIEVE BETTER*), scrubbed and
  re-chalked harder (if you silenced them), or claiming you as their own (if you spoke for the Unmade).
- 🟢 134 scripts / ~19.5k LOC, all clean.

---

## 📚 v2.65.0 — *"Her Name Was Elaine"* — Aldric's Daughter Lore (CONTENT)

- 📚 **+1 Codex Lore entry** (unlocks on meeting Aldric): *Aldric's Daughter* — Elaine, who believed in
  nothing and was mortared into the Wall for the crime of being honest; the tragedy that made the First
  Mask, and the horror that he is *not wrong.* **62 Codex entries.**
- 🟢 134 scripts / ~19.5k LOC, all clean.

---

## 🩹 v2.64.0 — *"Down, Not Dead"* — Revive Hint (CODE)

- 🩹 The first time a companion falls in combat, the log now explains the rule once: they're **down, not
  dead** — heal them to bring them back up mid-fight, or win to stabilize them. (Healing a 0-HP ally already
  revives them; this just makes it discoverable.) `AbilityRunner`.
- 🟢 134 scripts / ~19.4k LOC, all clean.

---

## ⚓ v2.63.0 — *"Flotsam from Fallen Ages"* — Spellplague Tide-Line at the Docks (CONTENT)

- ⚓ Once you've walked the Spellplague, the Chionthar Docks gains an examinable **tide-line of impossible
  debris** — skyglass, ancient coin, bone-white horn — flotsam from ages that fell before the Gate was
  founded, because the river forgets which century it's in now. You, of all people, know how that feels.
- 🟢 134 scripts / ~19.4k LOC, all clean.

---

## 🎖 v2.62.0 — *"Pincer Movement"* — Flanking Deed (CODE)

- 🎖 Landing a flanking attack now sets `combat.used_flank`, and a new **Deed** — *"Pincer Movement"* —
  rewards catching a foe between you and an ally. **26 deeds.**
- 🟢 134 scripts / ~19.4k LOC, all clean.

---

## ✦ v2.61.0 — *"You Learn Something"* — Announce New Abilities on Level-Up (CODE)

- ✦ When a level-up unlocks a new per-level ability, the combat log now announces it (*"…learns Ray of
  Frost!"*) instead of granting it silently — so players notice their growing kit. `EncounterController`.
- 🟢 134 scripts / ~19.4k LOC, all clean.

---

## 🛡 v2.60.0 — *"Know Your Kit"* — Abilities on the Party Sheet (CODE)

- 🛡 The **Party (P)** character sheet now lists each member's **known abilities** (their combat kit) under
  the stats — so you can see what Garrow, Naeve, or your hero can actually *do* before a fight, including
  the new per-level unlocks. `RosterScreen`.
- 🟢 134 scripts / ~19.4k LOC, all clean.

---

## 📜 v2.59.0 — *"More Log"* — Combat Log Shows 11 Lines (CODE)

- 📜 The combat log now keeps **11** lines instead of 9 — the deepened combat (opportunity attacks, round
  markers, tactical actions) generates more events worth seeing at a glance. `CombatHUD`.
- 🟢 134 scripts / ~19.3k LOC, all clean.

---

## 📚 v2.58.0 — *"Belong, or Be Erased"* — Faithless/False Lore (CONTENT)

- 📚 **+1 Codex Lore entry** (unlocks at the Fugue): *The Faithless and the False* — the doctrine's cruelest
  distinction (the wicked are tormented; the merely unaffiliated are *erased*), the rotten beam the whole
  saga means to pull out. **60 Codex entries.**
- 🟢 134 scripts / ~19.3k LOC, all clean.

---

## 🎓 v2.57.0 — *"Tips Again"* — Re-show Combat Hint Option (CODE)

- 🎓 Options gains a **"Show the combat tips again"** button (appears once you've dismissed the first-combat
  hint), clearing the PlayerPrefs flag so the tutorial pops next fight. `SettingsScreen`.
- 🟢 134 scripts / ~19.3k LOC, all clean.

---

## 💞 v2.56.0 — *"A Clumsy Love-Verse"* — Balladeer Reacts to Romance (CONTENT)

- 💞 The hub balladeer's broadside now adds a swooning, badly-scanning **love-verse** about the Returned and
  whichever companion you've committed to (a grave-priestess, a Harper thief, a fallen-sky sorceress, or a
  Hell-tabbed warlock) — with a little heart drawn, crossed out, and drawn again. The Gate gossips about
  your heart, not just your sword.
- 🟢 134 scripts / ~19.3k LOC, all clean.

---

## 🎓 v2.55.0 — *"Your Turn"* — First-Combat Tutorial Hint (CODE)

- 🎓 The first time the player enters combat (tracked once per machine via PlayerPrefs), the HUD shows a
  dismissible hint listing how to move/attack and all the action hotkeys — so the deepened combat is
  discoverable without opening Help. `CombatHUD.OnGUI`.
- 🟢 134 scripts / ~19.3k LOC, all clean.

---

## 🦇 v2.54.0 — *"Unfinished Sentences"* — Spellplague-Foe Bestiary (CONTENT)

- 🦇 **+1 Codex Bestiary entry** (unlocks on clearing the Spellplague): *Unmade Aberrations* — the grief of
  the Unmade in half-shapes, lashing at anything still confident enough to be a whole thing. **59 Codex entries.**
- 🟢 134 scripts / ~19.2k LOC, all clean.

---

## 🎖 v2.53.0 — *"Pulled From the Brink"* — Clutch-Win Deed (CODE)

- 🎖 Winning a battle in which a companion was downed now sets `combat.clutch_win`, and a new **Deed** —
  *"Pulled From the Brink"* — rewards the comeback. **25 deeds.**
- 🟢 134 scripts / ~19.2k LOC, all clean.

---

## 🩸 v2.52.0 — *"Bloodied"* — Half-Health Indicator (CODE)

- 🩸 When a unit first drops **below half HP** from a blow, a *"BLOODIED"* word pops over it — a clear,
  classic combat read of "this one's hurting now," beyond the HP bar. `AbilityRunner.ApplyOne`. (Also fixed
  the roadmap's second-vendor line — that shipped in v1.97.)
- 🟢 134 scripts / ~19.2k LOC, all clean.

---

## 🛡 v2.51.0 — *"Take Cover"* — Half-Cover for Ranged Attacks (CODE)

- 🛡 **Cover** — a ranged attack against a target with another creature directly between it and the shooter
  (along an orthogonal or perfectly diagonal line) is made against **+2 AC** (half cover), with a *"COVER"*
  pop on a resulting miss. Deterministic aligned-line check (`AbilityRunner.HasRangedCover`), folded into
  `AttackResolver` via a `targetAcBonus`. Positioning now matters for archers and casters too. Phase B cover ✅.
- 🟢 134 scripts / ~19.2k LOC, all clean.

---

## 🎉 v2.50.0 — *"Ninety Increments"* — Milestone + Roadmap Refresh (DOCS)

- 🎉 Milestone marker: ~90 self-contained, validated versions since v1.60, all on the branch, all clean.
  Snapshot: **134 scripts / ~19.1k LOC · 58 Codex entries · 24 deeds · 25 fireside banters · 13 night-talks
  · 11 keepsakes** — plus the full tactical combat layer, four explorable rooms + two vendors, accessibility
  options, New-Game+ memory, and reactive Lady/Breach beats in every room. ROADMAP status refreshed.
- 🟢 134 scripts / ~19.1k LOC, all clean.

---

## 🎒 v2.49.0 — *"What They Gave You"* — Keepsakes in the Relationships Panel (CODE)

- 🎒 The **Relationships (L)** screen now lists each companion's earned **keepsake** under their entry, so the
  panel shows not just where you stand with them but the small proof they let you matter. `RelationshipsScreen`.
- 🟢 134 scripts / ~19.1k LOC, all clean.

---

## 💾 v2.48.0 — *"Saved."* — Quick-Save/Load Toast (CODE)

- 💾 Quick-save (F5) / quick-load (F9) now flash a brief on-screen confirmation toast (and quick-load reports
  if there's no quick-save yet), so the hotkeys give feedback instead of acting silently. `GameManager`.
- 🟢 134 scripts / ~19.1k LOC, all clean.

---

## 👀 v2.47.0 — *"Unable to Look Away"* — Reactive Onlookers in the Hub (CONTENT)

- 👀 A new hub examinable — *a knot of gawking onlookers* — whose whispers react to **which companions** you
  travel with (the dead-tongue arcanist, the impossibly old elf, the warlock whose shadow won't match, the
  flickering girl, the rogue everyone's been robbed by). The Gate is afraid of you, and cannot look away.
- 🟢 134 scripts / ~19.1k LOC, all clean.

---

## ⚡ v2.46.0 — *"Opportunity!"* — Floating Text on Opportunity Attacks (CODE)

- ⚡ An opportunity attack now pops an *"OPPORTUNITY!"* floating word over the reacting unit, so the reaction
  reads on-screen the instant a foe tries to flee melee. `Reactions.OnMoveCompleted`.
- 🟢 134 scripts / ~19.0k LOC, all clean.

---

## 🔥 v2.45.0 — *"Moments by the Fire"* — Fireside Tally in the Chronicle (CODE)

- 🔥 The **Chronicle (C)** and finale recap now count the **fireside moments** you've shared — night-talks
  and group banters heard — read straight from the camp flags. `EndingResolver.Chronicle`.
- 🟢 134 scripts / ~19.0k LOC, all clean.

---

## 🔥 v2.44.0 — *"What the Poor Remember"* — Quarter-Pledge Banter (CONTENT)

- 🔥 **+1 `CampGroupBanter`** (gated on the Lower City's pledge, `lowcity.allies`): Roen & Ilfaeril on the
  nods from the doorways — the rarest verdict in the Realms, written nowhere and overturned never, and what
  it means for the gutter-born thief who came back for them. **25 fireside exchanges.**
- 🟢 134 scripts / ~19.0k LOC, all clean.

---

## 💾 v2.43.0 — *"Who's Waiting"* — Save Summary on the Title Screen (CODE)

- 💾 The main menu's **Continue** now shows a one-line summary of the autosave (hero name · level · saved
  time) read via `SaveSystem.Peek`, so you know what you're resuming before you click.
- 🟢 134 scripts / ~19.0k LOC, all clean.

---

## 🎭 v2.42.0 — *"A Note in the White Space"* — Reactive Safehouse Cipher-Board (CONTENT)

- 🎭 The Harper safehouse's blank cipher-slip now gains a beat once you've **read the Lady's name**: you
  write, very small, in the white space — *I know you're there. Thank you for staying* — and leave it pinned.
  Somewhere, the margin smiles. Every explorable room now has a Lady-reactive secret.
- 🟢 134 scripts / ~19.0k LOC, all clean.

---

## ⚔ v2.41.0 — *"Field Record"* — Combat Tallies in the Journey (CODE)

- ⚔ The **Journey (J)** screen now shows a compact *Field Record* — battles won and foes laid low — once
  you've fought at all. `JourneyScreen`.
- 🟢 134 scripts / ~18.9k LOC, all clean.

---

## 🎖 v2.40.0 — *"The Hard Way"* — Hard-Mode Finish Deed (CODE)

- 🎖 New **Deed** — *"The Hard Way"* — for reaching the Court of the Dead on **Hard** difficulty. **24 deeds.**
- 🟢 134 scripts / ~18.9k LOC, all clean.

---

## ❔ v2.39.0 — *"Documented Keys"* — Dialogue Hotkeys in Help (DOCS)

- ❔ The in-game **Help (H)** card now documents the new dialogue keyboard shortcuts (1–9 to pick a choice,
  Space to finish/continue).
- 🟢 134 scripts / ~18.9k LOC, all clean.

---

## ⌨ v2.38.0 — *"Keyboard Dialogue"* — Hotkeys for Conversations (CODE)

- ⌨ Dialogue is now keyboard-drivable: **1–9** pick a choice, **Space/Enter** finishes the typewriter line
  and advances "Continue" beats. No conflict with combat hotkeys (dialogue scenes don't run the combat
  input). `DialogueScreen.Update`.
- 🟢 134 scripts / ~18.9k LOC, all clean.

---

## 🪙 v2.37.0 — *"Wheeler-Dealer"* — Trade-Both-Ways Deed (CODE)

- 🪙 Buying and selling now set `shop.bought` / `shop.sold`, and a new **Deed** — *"Wheeler-Dealer"* —
  rewards trading both ways with a Lower City vendor. **23 deeds.**
- 🟢 134 scripts / ~18.8k LOC, all clean.

---

## 🧪 v2.36.0 — *"One Console Run"* — Balance Canary Folded into ValidationDemo (CODE)

- 🧪 `ValidationDemo` now also runs the **combat-balance canary** and shows/logs its win-rate verdict
  alongside the content-validation report — so one Play of the validation scene checks both authored content
  *and* combat-math sanity.
- 🟢 134 scripts / ~18.8k LOC, all clean.

---

## 🧱 v2.35.0 — *"The Same Wall, Read Two Ways"* — Reactive Wall of Names (CONTENT)

- 🧱 The Almshouse **Wall of Names** now gains a layer: if you've read the Lady's name, you grasp that it and
  the Wall of the Faithless are the same wall — one erases, one remembers, the only difference is who holds
  the chalk; if the quarter stands with you, they've started a blank column waiting for the day the Returned
  needs remembering too. All four Lower City rooms now have a Lady/reputation-reactive beat.
- 🟢 134 scripts / ~18.8k LOC, all clean.

---

## 📚 v2.34.0 — *"The Bored God"* — Jergal Lore (+ script count fix)

- 📚 **+1 Codex Lore entry**: *Jergal, the Scribe of the Dead* — the first god of the end, bored into early
  retirement at a clerk's stool, waiting ten thousand years for someone interesting to ask (the golden
  ending's quiet architect). **58 Codex entries.** Also corrected the script-count badge to 134.
- 🟢 134 scripts / ~18.7k LOC, all clean.

---

## ⚖ v2.33.0 — *"Canary"* — Combat-Balance Smoke Test (CODE)

- ⚖ **`CombatBalance`** + **`CombatBalanceDemo`** — a deterministic balance canary that runs 400 seeded melee
  duels through the *real* `AttackResolver` math (hand-built throwaway sheets, no scene/content deps) and
  prints a win-rate verdict. A fast tripwire for changes to the resolver, modifiers, crits, or scaling —
  the validator ethos, extended to combat. (Phase B tooling.)
- 🟢 133 scripts / ~18.7k LOC, all clean.

---

## ⚔ v2.32.0 — *"Tally of Battles"* — Wins in the Chronicle (CODE)

- ⚔ The **Chronicle (C)** and finale recap now include your **battles-won** count, alongside foes laid low
  and endings unlocked. `EndingResolver.Chronicle`.
- 🟢 132 scripts / ~18.6k LOC, all clean.

---

## 🎖 v2.31.0 — *"Veteran"* — Battles-Won Tally + Deed (CODE)

- 🎖 Victories are now tallied (`combat.victories`, incremented on each won fight), and a new **Deed** —
  *"Veteran"* — rewards winning fifteen battles. **22 deeds.**
- 🟢 132 scripts / ~18.6k LOC, all clean.

---

## 🧪 v2.30.0 — *"Greyed When Empty"* — Quaff Button Disables Without Potions (CODE)

- 🧪 The **Quaff** HUD button now greys out when the party has no healing draught left, so you can't burn
  your action on an empty flask. `CombatHUD`.
- 🟢 132 scripts / ~18.6k LOC, all clean.

---

## 🧪 v2.29.0 — *"How Many Left"* — Potion Count in the Combat HUD (CODE)

- 🧪 The active-unit panel now shows **how many healing draughts** remain in the party stash (e.g. *🧪 ×3
  healing*), so you know whether the Quaff (Q) action has anything to drink before you spend the turn on it.
  `CombatHUD.PotionCount`.
- 🟢 132 scripts / ~18.6k LOC, all clean.

---

## ⚓ v2.28.0 — *"A Cord of Your Own"* — Reactive Docks Memorial (CONTENT)

- ⚓ The Chionthar Docks' knotted-rope memorial now reacts if a companion was taken at the Breach: your hands
  tie a cord of your own, low on the piling, for the one the Wall took — who got no grave at all. Not
  nothing; never enough; you tie it anyway. `DocksScene.LostCompanionName`.
- 🟢 132 scripts / ~18.6k LOC, all clean.

---

## 🎭 v2.27.0 — *"Offerings to No One"* — Reactive Market Shrine (CONTENT)

- 🎭 The Shrunken Quarter's shrine-to-no-one now gains a second layer once you've **read the Lady's name**
  (`readers_boon`): the empty niche is exactly the shape of a Lady who only watches from the margins, and
  the poor's offerings-to-nobody become the truest prayers in the Gate — kindness with no expectation of
  being seen. She sees them anyway.
- 🟢 132 scripts / ~18.6k LOC, all clean.

---

## 📚 v2.26.0 — *"The Gate"* — Setting Codex Entry (CONTENT)

- 📚 **+1 Codex Premise entry** (known from the start): *Baldur's Gate* — the mercantile city of coin and the
  Flaming Fist where you died and Returned, and whose poor keep their own ledger of who treated them like
  they counted. Grounds the opening for new players. **57 Codex entries.**
- 🟢 132 scripts / ~18.6k LOC, all clean.

---

## 🔊 v2.25.0 — *"Crack!"* — Critical-Hit Sound Cue (CODE)

- 🔊 A critical hit now plays a distinct **"crit"** SFX cue on top of the impact sound (art-optional, like
  every other audio hook — silent if no clip in `Resources/SFX/`). Crits already pop bigger floating text;
  now they punch in the ears too. `AbilityRunner.ApplyOne`.
- 🟢 132 scripts / ~18.5k LOC, all clean.

---

## 🦇 v2.24.0 — *"Hubris and Moonbows"* — Era-Foe Bestiary (CONTENT)

- 🦇 **+2 Codex Bestiary entries** (unlock on clearing the era): *Netherese Arcanists* (the AoE-slinging
  defenders of the falling enclave — note the cluster-aware burst AI is live here) and *Elven Blade-Singers*
  (the beautiful, lawful warrior-mages of the Crown Wars). **56 Codex entries.**
- 🟢 132 scripts / ~18.5k LOC, all clean.

---

## 🎖 v2.23.0 — *"Loremaster"* — Codex Completion Deed (CONTENT)

- 🎖 New **Deed** — *"Loremaster"* — for filling thirty Codex entries, rewarding the player who witnesses the
  world rather than rushing it (the Codex now has 54 to find). **21 deeds.**
- 🟢 132 scripts / ~18.5k LOC, all clean.

---

## 🏆 v2.22.0 — *"Your Legacy"* — Endings Gallery in the Credits (CODE)

- 🏆 The **Credits** screen now shows a *"Your Legacy"* section once you've finished a saga — every ending
  listed, discovered ones lit (✦) with their title, the rest as *undiscovered*, plus a golden-road note.
  A spoiler-light completion gallery powered by the cross-run `EndingsLog`.
- 🟢 132 scripts / ~18.5k LOC, all clean.

---

## 📚 v2.21.0 — *"Two Fallen Homes"* — Companion Homeworld Lore (CONTENT)

- 📚 **+2 Codex Lore entries** (unlock on recruiting the companion): *The Seventh Enclave* (Naeve's fallen
  Netherese sky-city) and *The Court of the First Damnation* (the elven court where Ilfaeril voted a soul
  into nothing). **54 Codex entries.**
- 🟢 132 scripts / ~18.4k LOC, all clean.

---

## 🕯 v2.20.0 — *"Ongoing, Not Unfinished"* — Grief Banters Completed (CONTENT)

- 🕯 **+2 more grief `CampGroupBanter` exchanges**, completing coverage for all four romanceable Breach
  victims: survivors mourning **Roen** (Garrow & Naeve drink the thief out of the red) and **Naeve** (Varra
  & Ilfaeril, the proof that's *ongoing*, not unfinished). **24 fireside exchanges.**
- 🟢 132 scripts / ~18.4k LOC, all clean.

---

## 🕯 v2.19.0 — *"The Empty Place at the Fire"* — Grief Banters (CONTENT)

- 🕯 **+2 grief `CampGroupBanter` exchanges** that only fire if a particular companion was taken at the
  Breach: survivors mourning **Garrow** (Roen & Ilfaeril keep her list) or **Varra** (Garrow & Roen, the
  receipt no one got to burn). The permanent loss now echoes at the campfire. **22 fireside exchanges.**
- 🟢 132 scripts / ~18.4k LOC, all clean.

---

## 🧹 v2.18.0 — *"Clean Slate (Opt-In)"* — Reset Legacy Record (CODE)

- 🧹 Options now shows your cross-run **legacy line** and a guarded **"Reset legacy record…"** (two-step
  confirm) that wipes the New-Game+/endings-seen memory via `EndingsLog.Clear` — for testers and anyone who
  wants a true fresh start. Only appears once you've finished at least one saga.
- 🟢 132 scripts / ~18.3k LOC, all clean.

---

## 🟡 v2.17.0 — *"Don't Waste It"* — End-Turn Action Nudge (CODE)

- 🟡 The **End Turn** button now tints amber while your active hero still has an **unspent action**, a quiet
  nudge so you don't end a turn with a Defend/Dash/Help/Shove/spell left on the table. `CombatHUD`.
- 🟢 132 scripts / ~18.3k LOC, all clean.

---

## 🔥 v2.16.0 — *"The Arithmetic Declined to Disagree"* — Varra Romance Tease (CONTENT)

- 🔥 **+1 `CampGroupBanter`**: Naeve, on the Varra romance — she models it forty thousand ways, finds it the
  worst risk position she's ever charted, and concludes the model is missing a variable: *want*. Every
  romanceable companion now has a fireside tease. **20 fireside exchanges.**
- 🟢 132 scripts / ~18.3k LOC, all clean.

---

## ✨ v2.15.0 — *"Flanked!"* — Floating Flank Indicator (CODE)

- ✨ A flanked melee hit now pops a *"FLANKED"* floating word over the target, so the positional advantage
  reads on-screen, not just in the log. `AbilityRunner.ApplyOne`.
- 🟢 132 scripts / ~18.2k LOC, all clean.

---

## 💞 v2.14.0 — *"Once Sealed"* — Romance Codex Reflections (CONTENT)

- 💞 **+4 Codex Companion entries** that unlock when a romance is consummated — a short reflection on what
  each sealed bond means (Garrow's other list, Roen's un-run con, Naeve's revised axiom, Varra off the
  books). **52 Codex entries.**
- 🟢 132 scripts / ~18.2k LOC, all clean.

---

## 📚 v2.13.0 — *"Love Is a Stake"* — Two Late Lore Entries (CONTENT)

- 📚 **+2 Codex Lore entries**, earned late: *What Love Is, in This Story* (post-Breach — love as a stake,
  not a reward) and *Why the Dead Stay Gone* (post-Reader's-Boon — the loop, and the Lady's real question).
  **48 Codex entries.**
- 🟢 132 scripts / ~18.2k LOC, all clean.

---

## ⚖ v2.12.0 — *"Standings in the Record"* — Faction Rep in the Chronicle (CODE)

- ⚖ `EndingResolver.Chronicle` now lists your **faction standings** (Kelemvor / Choir / Ash Pact) when
  non-zero, so they appear in both the anytime **Chronicle (C)** and the finale recap — not just the Journey.
- 🟢 132 scripts / ~18.1k LOC, all clean.

---

## 🎖 v2.11.0 — *"Heart of the Company"* — A Camp-Confidant Deed (CONTENT)

- 🎖 New **Deed** — *"Heart of the Company"* — earned by hearing every companion's first campfire night-talk
  (all six, Maerin included). Rewards the player who actually sits at the fire with everyone. **20 deeds.**
- 🟢 132 scripts / ~18.1k LOC, all clean.

---

## 🦇 v2.10.0 — *"Hungry for Inclusion"* — Wall-Wraith Bestiary Entry (CONTENT)

- 🦇 **+1 Codex Bestiary entry** (unlocks at the Fugue): *Wall-Wraiths* — the half-dissolved residue of souls
  the Wall has been eating, drifting the grey marches reaching for any warmth that still has a name. Not
  malicious — hungry for inclusion, which is somehow worse. **46 Codex entries.**
- 🟢 132 scripts / ~18.1k LOC, all clean.

---

## 📜 v2.9.0 — *"The Ballad of the Returned"* — A Reactive Broadside (CONTENT)

- 📜 A new hub examinable — *a balladeer's broadside* (appears once you've walked your first era) — that
  **mythologizes your run**: it counts the ages you've walked and the era-monsters you've felled, and gets
  cheekily meta if you've read the Lady's name. The Gate turning your deeds into penny-ballad legend.
- 🟢 132 scripts / ~18.1k LOC, all clean.

---

## 🧠 v2.8.0 — *"Cornered"* — Wounded Enemies Take Cover (CODE)

- 🧠 An enemy that **can't reach any target** on its turn and is **below 35% HP** now takes a defensive Dodge
  (attacks against it have disadvantage until its next turn) instead of idling — making a fleeing, wounded
  foe harder to finish when you can't close the gap either. Uses the dodge stance directly (does *not* trip
  the player's "Master Tactician" tracking). `EncounterController.RunEnemyTurn`.
- 🟢 132 scripts / ~18.1k LOC, all clean. (Phase B: smarter enemy AI — self-preservation)

---

## 🌀 v2.7.0 — *"A Draft They've Read Before"* — New-Game+ Banter (CONTENT)

- 🌀 **A New-Game+ group banter** (gated on `ng.plus`): Roen & Garrow catch the uncanny déjà vu of a repeat
  run — "like we're a story somebody's re-reading" — and resolve to *do it kinder this time*. Complements
  Naeve's NG+ night-talk; the loop bleeds through to the whole camp. **19 fireside exchanges.**
- 🟢 132 scripts / ~18.1k LOC, all clean.

---

## 🎒 v2.6.0 — *"A Named Pebble"* — Maerin's Keepsake (CONTENT)

- 🎒 **Maerin now leaves a keepsake** — a small named river-pebble, given after her deeper night-talk
  (`camp.nighttalk.maerin2.done`). She has no personal quest, so this is her version of *"I let you matter."*
  Every companion now has at least one keepsake. **11 keepsakes.**
- 🟢 132 scripts / ~18.0k LOC, all clean.

---

## ✨ v2.5.0 — *"Felt Actions"* — Floating Text for Tactical Actions (CODE)

- ✨ The tactical actions now **pop a floating word** over the unit — *Defend, Dash, Helped, Disengage,
  Shoved* — so they read as clearly on-screen as attacks and heals do, not just in the combat log.
  `TurnManager.Floater`.
- 🟢 132 scripts / ~18.0k LOC, all clean.

---

## 🔥 v2.4.0 — *"Three Real Things"* — Maerin's Second Night-Talk (CONTENT)

- 🔥 **Maerin's deeper night-talk** (gated behind her first) — the morning ritual she's built since the Wall:
  naming three real things to prove she's still in a world that has nouns in it. You're usually on the list,
  between the bird and the cold. Maerin now has the full two-talk arc like the rest of the cast. **13 night-talks.**
- 🟢 132 scripts / ~18.0k LOC, all clean.

---

## 🔥 v2.3.0 — *"The Liar and the Judge"* — Roen & Ilfaeril Banter (CONTENT)

- 🔥 **+1 `CampGroupBanter` exchange**: *Roen & Ilfaeril* — the young liar who changes his name to escape the
  man, and the ancient judge who keeps his because the man is the only sentence still being served. Covers
  the last major companion pairing. **18 fireside exchanges.**
- 🟢 132 scripts / ~17.9k LOC, all clean.

---

## ❔ v2.2.0 — *"Complete Card"* — Help Overlay Catch-Up (DOCS)

- ❔ The in-game **Help (H)** card now lists the **L — Relationships** screen (it had been missing) and notes
  the new Options (UI size, combat speed). The card already scrolls, so the expanded combat-action list fits.
- 🟢 132 scripts / ~17.9k LOC, all clean.

---

## 📚 v2.1.0 — *"Where the Dead Wait"* — Two Metaphysics Codex Entries (CONTENT)

- 📚 **+2 Codex Lore entries** (unlock on reaching the Fugue): *The Fugue Plane* (the grey waystation where
  souls wait to be claimed) and *The City of Judgement* (Kelemvor's seat, where just procedure hardened into
  the Wall). Deepens the metaphysics the saga argues with. **45 Codex entries.**
- 🟢 132 scripts / ~17.9k LOC, all clean.

---

## 🎉 v2.0.0 — *"Quaff"* — In-Combat Potions + a Milestone

- 🧪 **Quaff a healing potion in combat** (key **Q**, or the new HUD *"🧪 Quaff"* button) — drinks the first
  healing consumable from the party stash onto the active unit, spending the action. `PlayerCombatInput.TryQuaff`.
- 🎉 **Milestone v2.0.** Since v1.60 the saga gained: a full tactical combat layer (Defend/Dash/Help/
  Disengage/Shove/Quaff + opportunity attacks + flanking + round tracking + AoE-aware, focus-fire enemy AI +
  per-level class kits + party-wipe recovery), four explorable Lower City rooms + two vendors with buy/sell,
  accessibility (UI-scale & combat-speed), cross-run endings memory + New-Game+, and a pile of content
  (banters, night-talks, 43 Codex entries, 19 deeds). All brace/namespace-clean, validated, on the branch.
- 🟢 132 scripts / ~17.8k LOC, all clean.

---

## 🔥 v1.99.0 — *"Keep Choosing"* — Maerin's Night-Talk (CONTENT)

- 🔥 **Maerin finally gets a campfire night-talk** — the girl pulled from the Wall on what counting feels
  like from the inside, and her plea not to be made a *cause* (which you can put down once won) but a
  *friend* (whom you have to keep choosing). Fills the one companion who lacked a night-talk. **12 night-talks.**
- 🟢 132 scripts / ~17.8k LOC, all clean.

---

## 📖 v1.98.0 — *"Who They Became"* — Post-Quest Companion Codex (CONTENT)

- 📖 **+5 Codex Companion entries** that unlock when each companion's personal quest resolves — a short
  reflection on who Garrow, Roen, Varra, Naeve, and Ilfaeril became on the far side of their reckoning,
  spoiler-light and outcome-aware. **43 Codex entries.**
- 🟢 132 scripts / ~17.8k LOC, all clean.

---

## 🛟 v1.97.0 — *"The Smuggler's Cache"* — A Second Vendor (CODE)

- 🛟 **A second merchant.** `ShopScreen` is now reusable (assignable stock + vendor name/tagline/quote), and
  the Chionthar Docks gains a **black-market vendor** — the Smuggler's Cache — selling cheaper smuggled
  goods (incl. a new Smuggled Dagger) with no reputation gate. Buy/sell both work. `ContentValidator` now
  checks both stocks resolve.
- 🟢 132 scripts / ~17.7k LOC, all clean. (Phase C: merchants/economy — second vendor)

---

## 🎖 v1.96.0 — *"Trusted at the Niche"* — Safehouse Deed (CONTENT)

- 🎖 New **Deed** — *"Trusted at the Niche"* — earned by being welcomed into the Harper safehouse
  (`lowcity.visited_safehouse`). **19 deeds.**
- 🟢 132 scripts / ~17.7k LOC, all clean.

---

## 🕸 v1.95.0 — *"The Wandering Niche"* — Harper Safehouse (CONTENT)

- 🕸 **A fourth explorable Lower City room** — `HarperSafehouseScene`, the Wandering Niche, which opens once
  **Roen** is in the company: a Harper handler whose lines react to Roen's personal-quest outcome, a cipher
  board, and a quiet two-chair corner that shifts if you've romanced him (the inn key on the sill). Wired
  via `BaldursGateHub.onEnterSafehouse` + `CampaignBootstrap.EnterSafehouse`, gated on `companion.roen.recruited`.
- 🟢 132 scripts / ~17.7k LOC, all clean. (Phase C: multi-room Lower City — Harper safehouse)

---

## 🪙 v1.94.0 — *"Coin's Coin"* — Sell Loot at the Fence (CODE)

- 🪙 The fence (Sczerla's Sundries) now **buys your loot** as well as selling: a Sell list of your party's
  stacks, paying a fraction of each item's worth that **improves with your Lower City standing** (30%→60%).
  Turns accumulated drops into gold and gives reputation another tangible payoff. `ShopScreen`.
- 🟢 131 scripts / ~17.6k LOC, all clean. (Phase C: merchants/economy)

---

## 📋 v1.93.0 — *"Honest Feature Codex"* — FEATURES.md Combat Catch-Up (DOCS)

- 📋 `FEATURES.md` combat section now lists the deepened combat: tactical actions (Dodge/Dash/Help/
  Disengage/Shove), opportunity attacks, flanking, round tracking, smarter (AoE-aware, focus-fire) enemy
  AI, per-level class kits, and party-wipe recovery. Scripts badge synced to 131 files / ~17.6k LOC.
- 🟢 131 scripts / ~17.6k LOC, all clean.

---

## 🗡 v1.92.0 — *"The Art of the Duel"* — In-Game Combat Reference (CONTENT)

- 🗡 New **Codex** Premise entry (known from the start) documenting every combat action — Defend, Dash, Help,
  Disengage, Shove — plus the opportunity-attack and flanking rules, with hotkeys. An in-fiction tutorial
  reference for the deepened combat, readable any time via the Codex (K). **38 Codex entries.**
- 🟢 131 scripts / ~17.6k LOC, all clean.

---

## 🏷 v1.91.0 — *"Status at a Glance"* — Conditions on Nameplates (CODE)

- 🏷 **World-space nameplates now show status** — a compact, color-coded line under each unit's HP bar lists
  active effects with remaining rounds (green = beneficial, red = harmful) plus the **Dodge**/**Diseng**
  stance. You can now see at a glance who's Burning, Slowed, Blessed, or hunkered down — without opening a
  panel. `UnitNameplates.Conditions`.
- 🟢 131 scripts / ~17.6k LOC, all clean.

---

## 🎖 v1.90.0 — *"Master Tactician"* — Track Combat Actions + New Deed (CODE)

- 🎖 Using each tactical action (Dodge/Dash/Help/Disengage/Shove) now sets a `combat.used_*` flag, and a new
  **Deed** — *"Master Tactician"* — rewards using all five in a run. Ties the deepened combat economy into
  the Chronicle's meta-layer. **18 deeds.**
- 🟢 131 scripts / ~17.5k LOC, all clean.

---

## ⚖ v1.89.0 — *"Standing with the Powers"* — Faction Reputation Surfaced (CODE)

- ⚖ The **Journey (J)** screen now shows a *"Standing with the Powers"* section — your reputation with the
  Doomguides of Kelemvor, the Faithless Choir, and the Ash Pact (reading the existing
  `faction.*.reputation` flags), each with a descriptive tier (known → trusted → honored, or marked →
  distrusted → reviled). Shown only for factions that have noticed you. Phase C groundwork.
- 🟢 131 scripts / ~17.5k LOC, all clean.

---

## 📝 v1.88.0 — *"Doc Sync"* — Controls & Roadmap Catch-Up (DOCS)

- 📝 README controls line now lists the combat actions (G/F/T/X/V); ROADMAP status + DONE snapshot updated
  to reflect the deepened combat (action economy, opportunity attacks, flanking, per-level kits), the three
  Lower City rooms, and the meta additions (UI-scale, combat-speed, New-Game+ memory, party-wipe recovery).
- 🟢 131 scripts / ~17.5k LOC, all clean.

---

## 🪨 v1.87.0 — *"Shove"* — The Shove Action (CODE)

- 🪨 **Shove action** — spend your action (key **V**, or the new HUD *"🪨 Shove"* button), then click an
  adjacent enemy: a **contested Strength check** pushes them one tile directly back if the space behind them
  is open. `TurnManager.TryShove` (uses `GridUnit.PlaceAt`). Battlefield control — break a flank, open a
  lane, or shove a foe off a wounded ally. Pairs with opportunity attacks and flanking.
- 🟢 131 scripts / ~17.5k LOC, all clean. (Phase B: action variety — Shove)

---

## 🔥 v1.86.0 — *"Old Debts at the Fire"* — Two More Camp Banters (CONTENT)

- 🔥 **+2 `CampGroupBanter` exchanges**: *Varra & Ilfaeril* (two souls bound by debts they signed before
  they knew better — one sold to her, one freely incurred) and *Varra & Maerin* (the priced and the
  un-counted, agreeing to stop doing math on their own suffering). **17 fireside exchanges.**
- 🟢 131 scripts / ~17.5k LOC, all clean.

---

## 📚 v1.85.0 — *"Three Doors in the Book"* — Location Codex Entries (CONTENT)

- 📚 **+3 Codex Lore entries** that unlock as you explore the Lower City: the Almshouse of the Unclaimed
  (the anti-Wall, a place built to *remember* the discarded), the Shrunken Quarter (offerings to an empty
  niche), and the Chionthar Docks (the river that buries the poor for free). **37 Codex entries.**
- 🟢 131 scripts / ~17.5k LOC, all clean.

---

## 🎯 v1.84.0 — *"Mind the Cluster"* — AoE-Aware Enemy Targeting (CODE)

- 🎯 **Enemy casters now aim area attacks smartly** — when an enemy's chosen ability is an `AreaBurst`, it
  centers the blast on the reachable party member whose tile catches the **most** of your party (tiebreak
  by proximity), instead of just hitting the nearest/weakest single target. Clumping up against a Fireball-
  toting foe now actually punishes you. `EncounterController.ChooseTarget`.
- 🟢 131 scripts / ~17.4k LOC, all clean. (Phase B: smarter enemy AI — AoE avoidance pressure)

---

## 🏷 v1.83.0 — *"Read Your Kit"* — Richer Ability-Bar Labels (CODE)

- 🏷 **Ability buttons now show their stats** — each combat ability button gets a second line with its
  damage/heal dice + type, range (or *melee*), AoE radius, spell-slot level, and a *bonus* tag for
  bonus-action abilities. With the new per-level class kits, players can finally read what each slot does
  without guessing. `CombatHUD.AbilityLabel`.
- 🟢 131 scripts / ~17.4k LOC, all clean.

---

## 🗺 v1.82.0 — *"Knew Every Door"* — Room-Visited Flags + Explorer Deed (CONTENT)

- 🗺 The three Lower City rooms now set **visited flags** (`lowcity.visited_almshouse/market/docks`), and a
  new **Deed** — *"Knew Every Door"* — rewards walking all three. Ties the explorable breadth into the
  Chronicle's meta-layer. **17 deeds.**
- 🟢 131 scripts / ~17.4k LOC, all clean.

---

## ⚓ v1.81.0 — *"The Waterfront"* — The Chionthar Docks (CONTENT)

- ⚓ **A third explorable Lower City room** — `DocksScene`, the Chionthar Docks: a reputation-reactive
  dockhand (smuggling offer at high standing, cold shoulder at low), a wary smuggler who warms if you
  helped Kallia with the Fist, and a memorial of knotted rope for the drowned poor the church wouldn't
  bury. Wired into `BaldursGateHub` (`onEnterDocks`) + `CampaignBootstrap.EnterDocks`. The district keeps
  growing navigable.
- 🟢 131 scripts / ~17.4k LOC, all clean. (Phase C: multi-room Lower City)

---

## 🩹 v1.80.0 — *"Clean Slate"* — Reset Combat-Transient State Each Encounter (FIX)

- 🩹 **Bugfix:** the new per-combat flags (Dodge / Disengage / Help-advantage / last-reaction-round) lived
  on the `CharacterSheet` and weren't cleared between fights, so e.g. a unit that used its reaction in round
  1 of one battle could be denied its round-1 opportunity attack in the *next* battle. `TurnManager.StartCombat`
  now resets all four for every participant. Each encounter starts genuinely clean.
- 🟢 130 scripts / ~17.3k LOC, all clean.

---

## ✅ v1.79.0 — *"Trust, but Verify"* — Validator Covers Abilities/Classes/Camp Content (CODE)

- ✅ **`ContentValidator` extended** to sanity-check more data-only content: every ability (AreaBurst has a
  radius, heals have heal dice, no negative slot/range), every class's per-level kit resolves with no null
  entries, and camp **banters** / **night-talks** have unique ids/keys, both speakers, and non-empty lines.
  Catches the bug class the sandbox can't (no Unity compiler) — `ValidationDemo` reports PASS/FAIL on Play.
- 🟢 130 scripts / ~17.3k LOC, all clean.

---

## 📖 v1.78.0 — *"A Growing Spellbook"* — Per-Level Class Kits (CONTENT + CODE)

- 📖 **Classes now grow as they level.** `startingAbilities` was already a per-level unlock list but each
  class had only one entry; now every class has a curated kit that unlocks with level:
  - **Fighter:** Longsword → Second Wind (bonus-action self-heal) → Javelin → Greataxe
  - **Wizard:** Fire Bolt → Ray of Frost (slows) → Thunderwave (Con-save burst) → Fireball
  - **Cleric:** Mace → Cure Wounds → Bless → Guiding Bolt → Sacred Flame
  - **Rogue:** Dagger → Shortbow → Shortsword · **Ranger:** Shortbow → Dagger → Longsword · **Barbarian:** Greataxe → Javelin
- ➕ New abilities (Second Wind, Ray of Frost, Sacred Flame, Guiding Bolt, Healing Word, Thunderwave,
  Javelin, Shortsword) and a **Slowed** status. Leveling up now meaningfully changes your options.
- 🟢 130 scripts / ~17.3k LOC, all clean. (Phase B: more abilities/spells)

---

## 🧹 v1.77.0 — *"Tidy Bar"* — Combat HUD Action Button Layout Fix (CODE)

- 🧹 **HUD layout fix** — the four new action buttons (Defend/Dash/Help/Disengage) were overlapping the
  bottom-centered ability bar; they now stack in a clean right-edge column above End Turn. No behavior
  change, just readability.
- 🟢 130 scripts / ~17.2k LOC, all clean.

---

## ↔ v1.76.0 — *"Flanked"* — Flanking Advantage (CODE)

- ↔ **Flanking** (optional 5e rule) — a melee attacker gets **advantage** when an ally stands on the tile
  directly opposite them across the target. Pure positional check (`AbilityRunner.IsFlankingMelee`), folded
  into `AttackResolver` via an `extraAdvantage` input and logged with a *"is flanked!"* note. Rewards
  pincering a foe — and pairs with the new opportunity-attack / positioning layer.
- 🟢 130 scripts / ~17.2k LOC, all clean. (Phase B: cover & flanking)

---

## 🥾 v1.75.0 — *"Disengage"* — The Disengage Action (CODE)

- 🥾 **Disengage action** — spend your action (key **X**, or the new HUD *"🥾 Disengage"* button) so your
  movement this turn provokes **no opportunity attacks**. `TurnManager.TryDisengage()` →
  `CharacterSheet.IsDisengaging` (cleared at your next turn start, honoured by `Reactions`). Makes the new
  opportunity-attack layer something you can play around — the action economy now has a real retreat option.
- 🟢 130 scripts / ~17.2k LOC, all clean. (Phase B: action variety — Dodge/Dash/Help/Disengage complete)

---

## ⚡ v1.74.0 — *"Don't Turn Your Back"* — Opportunity Attacks (CODE)

- ⚡ **Opportunity attacks** — leave an adversary's melee reach (move from an adjacent tile to a non-adjacent
  one) without Disengaging, and that adversary spends its **reaction** (one per round) to make a free melee
  strike. New `Reactions.OnMoveCompleted`, hooked into both player and enemy movement; resolved via the new
  `AbilityRunner.ApplyOne` (which bypasses the action economy). `CharacterSheet.lastReactionRound` /
  `IsDisengaging` track the rules. Positioning suddenly matters.
- 🟢 130 scripts / ~17.2k LOC, all clean. (Phase B: reactions)

---

## 🌀 v1.73.0 — *"Déjà Vu"* — A New-Game+ Camp Moment (CONTENT)

- 🌀 **New-Game+ night-talk** — on a fresh run after finishing the saga (`ng.plus`), Naeve — the woman who
  will one day become the Lady in the Margins — gets an uncanny fireside moment: she feels the loop turn
  under her, *"I have run this proof before."* Ties the New-Game+ system to the story's deepest twist.
- 🟢 129 scripts / ~17.1k LOC, all clean.

---

## 🔢 v1.72.0 — *"Round Count"* — Combat Round Tracking (CODE)

- 🔢 **Round counter** — `TurnManager.RoundNumber` increments each time the initiative order wraps; logged
  at each round boundary and shown in the combat HUD's initiative header. Readability groundwork for
  duration/reaction features (status durations are already round-based).
- 🟢 129 scripts / ~17.1k LOC, all clean.

---

## 🤝 v1.71.0 — *"Help"* — The Help Action (CODE)

- 🤝 **Help action** — spend your action (key **T**, or the new HUD *"🤝 Help"* button), then click an
  adjacent ally: their **next attack roll is made with advantage**. `CharacterSheet.HasHelpAdvantage`
  (consumed on the recipient's next attack-roll ability), `TurnManager.TryHelp()`, help-targeting mode in
  `PlayerCombatInput` (Esc cancels). Completes the Dodge/Dash/Help action trio.
- 🟢 129 scripts / ~17.1k LOC, all clean. (Phase B: action variety)

---

## 🔥 v1.70.0 — *"Two More at the Fire"* — Camp Banters (CONTENT)

- 🔥 **+2 `CampGroupBanter` exchanges**: *Garrow & Maerin* (the gravedigger who once buried a Faithless
  child, and the woman the Wall swallowed) and *Naeve & Roen* (the thousand-year proof and the forger who
  forgot he's the original). **15 fireside exchanges.**
- 🟢 129 scripts / ~17.1k LOC, all clean.

---

## ⏩ v1.69.0 — *"Pacing"* — Combat Speed Setting (CODE)

- ⏩ **Combat speed slider** (0.5×–2.5×) in Options, persisted via PlayerPrefs — scales how quickly enemy
  turns play out (`GameSettings.CombatDelay`, applied to `EncounterController`'s turn pacing). For players
  who want snappier fights or a slower, more readable pace.
- 🟢 129 scripts / ~17.0k LOC, all clean.

---

## 🧠 v1.68.0 — *"Focus Fire"* — Smarter Enemy Targeting (CODE)

- 🧠 **Enemy AI target selection** — instead of always charging the *nearest* hero, enemies now focus-fire
  the **lowest-HP foe they can actually reach and strike this turn** (finishing kills, concentrating
  damage), breaking ties by proximity; they fall back to closing on the nearest only when nothing is in
  reach. `EncounterController.ChooseTarget`. Combat reads sharper and punishes leaving a wounded unit exposed.
- 🟢 129 scripts / ~17.0k LOC, all clean. (Phase B: smarter enemy AI)

---

## 💨 v1.67.0 — *"Dash"* — The Dash Action (CODE)

- 💨 **Dash action** — spend your action (key **F**, or the new HUD *"💨 Dash"* button) for extra movement
  equal to your speed this turn. `TurnManager.TryDash()`, surfaced in the combat HUD + Help (H) overlay.
  Builds on the Dodge action's economy pattern (Phase B: action variety).
- 🟢 129 scripts / ~17.0k LOC, all clean.

---

## ☠ v1.66.0 — *"Stay Gone (or Don't)"* — Downed Clarity + Party-Wipe Recovery (CODE)

- ☠ **Party-wipe → recovery flow** — a total defeat now raises a **`DefeatScreen`** (Load Last Save /
  Return to Title) on its own root object, instead of stranding you on a dead battlefield.
- 🩹 **Downed clarity** — the initiative list distinguishes **(downed)** friendlies from **(slain)** foes,
  and the HUD now shows **Victory** or **Defeat** correctly at the end. Downed companions still stabilize
  after a win — permanent loss stays reserved for the Breach.
- 🟢 129 scripts / ~17.0k LOC, all clean. (ROADMAP near-term item #3 ✅)

---

## 🛡 v1.65.0 — *"Defend"* — The Dodge Action (CODE)

- 🛡 **Dodge / Defend action** — spend your action (key **G**, or the new HUD *"🛡 Defend"* button) and
  attack rolls against you are made at **disadvantage** until your next turn. Implemented as
  `CharacterSheet.IsDodging` (cleared at the dodger's next turn start), `TurnManager.TryDodge()`, factored
  into `AttackResolver`'s advantage math, and surfaced in the combat HUD + Help (H) overlay.
- 🟢 128 scripts / ~16.9k LOC, all clean. (ROADMAP near-term item #2 ✅)

---

## 🔁 v1.64.0 — *"The Loop Remembers"* — New-Game+ Awareness (CODE + CONTENT)

- 🔁 Fresh runs after finishing a saga now set **`ng.plus`** / **`ng.priorRuns`** flags, plus a New-Game+
  Codex Lore entry, *"The Loop Remembers"* — the Lady kept the count between sagas. **34 Codex entries.**
- 🕯️ The in-game **Chronicle (C)** now shows your cross-run **Legacy** line too, not just the title screen.
- 🟢 128 scripts / ~16.9k LOC, all clean.

---

## 🏆 v1.63.0 — *"Legacy"* — Cross-Run Endings Memory + New-Game+ Tally (CODE)

- 🏆 **`EndingsLog`** — a PlayerPrefs record of every ending reached *across all playthroughs* (survives
  saves, deletes, fresh starts). The Court records your choice; the **title screen** now shows
  *"Legacy: N sagas completed · endings discovered M/6 ★"* so completionists can see what's left.
- 🟢 128 scripts / ~16.8k LOC, all clean.

---

## 🔥 v1.62.0 — *"More Voices at the Fire"* — Two New Camp Banters (CONTENT)

- 🔥 **+2 `CampGroupBanter` exchanges** broadening Maerin's web: *Naeve & Maerin* (the theorist of the
  unmaking meets the woman who survived it) and *Roen & Maerin* (the man who always plans an exit, and the
  woman who had none). **13 fireside exchanges.**
- 🟢 127 scripts / ~16.8k LOC, all clean.

---

## ♿ v1.61.0 — *"Bigger Words"* — Accessibility UI Text Size (CODE)

- ♿ **UI text-size slider** (85%–160%) in Options, persisted via PlayerPrefs. New `UiScaler` applies the
  scale to the shared IMGUI skin so **every** OnGUI screen grows/shrinks at once; inline `<size>` headers
  stay proportional. For late-night, low-spoons, glasses-off reading.
- 🟢 127 scripts / ~16.7k LOC, all clean.

---

## 📚 v1.60.0 — *"More Lore in the Book"* — Four Codex History Entries (CODE)

- 📚 **+4 `CodexContent` Lore entries** (flag-gated): The Doom of Kelemvor, "The Wall Was a Decision",
  The Time of Troubles, The Spellplague — the metaphysics the saga argues with. **33 Codex entries.**
- 🟢 126 scripts / ~19.5k LOC, all clean.

---

## 🏙️ v1.59.0 — *"The Shrunken Quarter"* — A Second Hub Area (CODE)

- 🏙️ **`MarketScene`** — a new explorable Lower City room off the hub: a fishwife (Mhaere's sister) and a
  bored Flaming Fist picket whose lines react to your reputation and the Widow/Fist quests, plus a shrine
  "to no one." Wired via `BaldursGateHub.onEnterMarket` + `CampaignBootstrap.EnterMarket`.
- 🟢 126 scripts / ~19.4k LOC, all clean.

---

## ❤ v1.58.0 — *"At a Glance"* — A Relationships Screen (CODE)

- ❤ **`RelationshipsScreen` (press L)** — every companion in your run with an **approval bar**, furthest
  **romance** stage, **rupture** state (mended/uneasy/broken/left/lost), and **personal-quest** beat. Pure
  read of GameFlags; persistent overlay beside Chronicle/Help.
- 🟢 125 scripts / ~19.2k LOC, all clean.

---

## 🗺️ v1.57.0 — *"The Plan of Record"* — Comprehensive Roadmap (DOCS)

> A living development plan you can see at a glance: what's done, what's next, and the long arc to ship.

- 🗺️ Rewrote **`docs/ROADMAP.md`** into a phased, current plan — a condensed ✅ DONE snapshot by pillar,
  a 🎯 near-term queue (next ~5 increments), and 📋 Phases A–G (art/presentation, combat depth, Act II/
  world breadth, Act III/IV main-plot, companion depth, systems/meta, production) with per-task status,
  anti-scope-creep rules, and a contributor map of where everything lives.
- 🟢 124 scripts / ~19.0k LOC, no code change — documentation/planning.

---

## 🏅 v1.56.0 — *"Three More Deeds"* — Achievements Reach the New Systems (CODE)

> The deeds layer grows to recognise the kill-tally and the Lower City's mercies.

- 🏅 **+3 `Deeds`** (16 total): **Hunter of the Returned** (lay low 25 foes, via `slain.total`), **The Whole
  Company** (recruit every companion), **The Gentle Returned** (every Lower City mercy: the widow's hope,
  the Faithless freed, the grave-tithe torn up, the Choir given doubt).
- 🟢 124 scripts / ~19.0k LOC, all clean.

---

## 📜 v1.55.0 — *"The Board Reads You Back"* — A Reactive Proclamation Board (CODE)

> The hub's notice board now answers to how the city sees you — built fresh from the flags each time you
> return to the Lower City.

- 📜 **`BaldursGateHub.ProclamationText`** — beneath the Fist's sealed-stair notice, a reactive line: the
  quarter keeps your name "like a candle" at high standing / the allies pledge, chalks it with "a hard
  word" at negative standing, posts crossed-out "sedition" about a Returned walking dead ages once you've
  cleared Netheril, or just the usual bread-and-lost-dogs otherwise.
- 🟢 124 scripts / ~19.0k LOC, all clean.

---

## ✉️ v1.54.0 — *"The Last Letter"* — A Fifth Act II Side Quest (CODE)

> A dying Flaming Fist veteran, a forty-year-old cowardice, and three kinds of closure.

- ✉️ **"The Last Letter"** (`ActTwoContent`) — Old Davyn asks you to handle a confession to a man he got
  killed: **deliver** it to the victim's daughter (the hard truth; +Garrow), **burn** it (let her keep her
  father a hero; +Ilfaeril), or **read it back to him** so he hears himself be honest once before the end
  (+both). Active + aftermath NPCs, a Journey entry, and three distinct ending slides.
- 🟢 124 scripts / ~18.9k LOC, all clean. Act II now carries **five** side quests.

---

## ⚔️ v1.53.0 — *"Foes Laid Low"* — A Running Bestiary Tally (CODE)

> Every kill now leaves a mark in the ledger.

- ⚔️ **Kill-tracking** — `EncounterController` tallies each defeated enemy by name (`slain.<name>`) plus a
  `slain.total`, at the same combat-end pool that awards XP. The **Chronicle (C)** shows a **⚔️ Foes Laid
  Low (N)** section listing each foe type and how many you've put down.
- 🟢 124 scripts / ~18.7k LOC, all clean.

---

## 🧪 v1.52.0 — *"Trust, but Verify (Harder)"* — The Validator Now Checks Data (CODE)

> The content validator grew up: besides dialogue links, it now sanity-checks the data-only content the
> sandbox can't compile-test — the bug class that would otherwise only surface as a silent null in-game.

- 🧪 **`ContentValidator.CheckData`** — builds `SwordCoastContent` to populate the `ItemDatabase`, then
  verifies **every shop offer's item id resolves**, and that **Codex / Keepsakes / Deeds** entries have
  non-empty ids+titles and no duplicates. Surfaced in the same `ValidationDemo` PASS/FAIL report.
- 🟢 124 scripts / ~18.6k LOC, all clean.

---

## 🗨️ v1.51.0 — *"More Voices, Round Three"* — Banter incl. the Heart of Ilfaeril's Arc (CODE)

> Three more fireside exchanges (now 11), including the one the story bible calls the heart of Ilfaeril's
> bond — and two more romance-reactive ribbings.

- 🗨️ **`CampGroupBanter` +3**: **Ilfaeril & Maerin** (the man who helped build the Wall and the girl it
  swallowed, learning to forgive each other on everyone's behalf — *"It wasn't enough. But it wasn't
  nothing."*), plus **Varra on a Naeve romance** and **Garrow on a Roen romance** (gated on the matching
  `romance.<id>.turn`).
- 🟢 124 scripts / ~18.5k LOC, all clean.

---

## 🪙 v1.50.0 — *"Sczerla's Sundries"* — A Reputation-Gated Lower City Fence (CODE)

> Fifty versions in — a milestone — and the gold you've been hoarding finally has somewhere to go: a
> fence whose shelves *grow* with your standing in the quarter.

- 🪙 **`ShopContent` + `ShopScreen`** — a hub vendor that buys against the shared party gold, with stock
  filtered by `reputation.lowcity`: potions/leather/longsword at any standing, a chain shirt at standing
  ≥ 2, a greataxe at ≥ 4 — rep-locked goods teased with 🔒. A genuine **gold sink** that pays off the Act
  II reputation economy.
- 🔌 Opened from a new hub door (Sczerla's Sundries) via `onEnterShop`; drawn as an overlay on the hub
  (no mode-swap), so you pop in and out without leaving the Lower City.
- 🟢 124 scripts / ~18.4k LOC, all clean.

---

## 🔥 v1.49.0 — *"Deeper Into the Night"* — A Second Night-Talk per Companion (CODE)

> The fire gives up its second confidence. Each companion now has a *deeper* campfire monologue, unlocked
> at higher approval and gated behind the first.

- 🔥 **Five new night-talks** (`CampContent`) at approval ≥ 55, each requiring the first to be heard:
  Garrow's *list she keeps*, Roen's *name underneath*, Varra *sleeping a whole night for the first time in
  twenty years*, Naeve's *unsolved variable*, Ilfaeril's *longest night*.
- 🔧 `NightTalk` gains a unique `key` (own done-flag) + a `requiresFlag` gate, with `Best()` and
  `CampScene` updated to use them — so a companion can have multiple talks without flag collisions, and the
  Act II quest-hooks (keyed to the *first* talk) are untouched.
- 🟢 122 scripts / ~18.2k LOC, all clean. Ten campfire monologues now.

---

## 🎚️ v1.48.0 — *"Sponge or Glass"* — Difficulty Now Scales Enemy HP (CODE)

> Difficulty scaled *damage* (v1.14); now it scales *durability* too, so Story/Hard feel distinct in
> length as well as lethality.

- 🎚️ **`GameSettings.EnemyHpMult`** (Story ×0.8 · Normal ×1.0 · Hard ×1.3) applied at the campaign
  `Enemy()` builder, after HP is rolled — so every campaign foe and miniboss gets frailer or spongier to
  match the chosen mode. Pairs with the existing damage scaling for a real three-tier feel.
- 🟢 122 scripts / ~18.0k LOC, all clean.

---

## 👥 v1.47.0 — *"The Company, in the Book"* — Companion Codex Entries (CODE)

> The Codex covered the villains and the world but not the people beside you. Now it does.

- 👥 **A new "The Company" category** in `CodexContent` with six entries — Garrow, Roen, Varra, Naeve,
  Ilfaeril, Maerin — each a flag-gated portrait of who they are and what moves their approval (so they
  fill in as they join your party). `CodexScreen` labels the new category. **29 Codex entries now.**
- 🟢 122 scripts / ~17.9k LOC, all clean.

---

## 🎬 v1.46.0 — *"The Company You Kept"* — A Credits Screen (CODE)

> A proper capstone roll: the cast, the Four Masks, the ages walked, and the dedication the whole game
> has been circling.

- 🎬 **`CreditsScreen`** — a scrollable credits modal off the main menu (**Credits** button): the Returned,
  the six companions with their one-line epithets, the Four Masks, the eras, the toolset, and the closing
  line — *"You came back. They never come back."*
- 🔌 Wired into `MainMenu` with the same modal pattern as Settings/Saves (suppresses the menu while open).
- 🟢 122 scripts / ~17.8k LOC, all clean.

---

## 🏆 v1.45.0 — *"Deeds"* — A Lightweight Achievements Layer (CODE)

> A quiet meta-layer that marks the shape of a run — and rewards thoroughness — without a single new hook:
> every deed is just a predicate over the flags the game already sets.

- 🏆 **`Deeds`** — 13 milestones, each a title + a `Func<GameFlags,bool>`: *The Returned, Tea with a
  Heretic, Walker of Ages* (all four eras), *Four Foes for Four Ages* (every era miniboss), *The Verdict
  Argued Down, Five Threads Pulled* (all quests), *Beloved, The Bond That Broke, The Quarter's Champion,
  Reader of the Lady, Keeper of Mementos, The Saga Ended,* and *The Golden Road* (earn & choose a golden
  ending).
- 📖 **In the Chronicle** — a **🏆 Deeds (N/total)** section with earned ★ (title + how) and locked ☆
  (title only, to tease). Pure read; nothing to wire.
- 🟢 121 scripts / ~17.7k LOC, all clean.

---

## 🎁 v1.44.0 — *"A Token from the Road"* — Camp Gift Mechanic (CODE)

> A small lever that finally ties the loot economy to the bond: give a companion something from your pack.

- 🎁 **Give a Gift** at camp — `CampScene` gains a section that finds the first **Consumable** in the party
  stash (resolved via `ItemDatabase`) and lets you give it to any fielded companion for **+3 approval**,
  with a flavour line. The item is the cost, so it's a genuine resource→relationship tradeoff.
- 🟢 120 scripts / ~17.6k LOC, all clean. The campfire now has rest, night-talks, cross-talk, romance,
  ruptures, *and* gifts.

---

## ⚔️ v1.43.0 — *"Four Foes for Four Ages"* — Era Minibosses Completed + a Keepsakes Ending (CODE)

> The last two eras get their optional minibosses too — now *every* age has one — and the keepsakes you
> gathered get their own beat at the Court.

- ⚙️ **The Mythallar Colossus** (Netheril) — a war-construct still executing its last order to defend a
  city already dead, fought in the collapsing floor. Clearing it: `netheril.boss_down`, +Naeve, a Codex
  entry, and an ending slide.
- 🌫️ **The First Unmade** (Crown Wars) — the very first soul the court voted to erase, risen from the
  half-place in ten-thousand-year grief; Ilfaeril helps you lay it down where the Wall began.
  `crownwars.boss_down`, +Ilfaeril, Codex, ending slide. **All four eras now offer an optional miniboss.**
- 🔧 Added the optional-fight pattern to the bespoke `NetherilEra` / `CrownWarsEra` builders (the
  `SimpleEra` ones already had it from v1.39), dispatched via `StartNetherilFight` / `StartCrownWarsFight`.
- 🎒 **Keepsakes ending slide** — *"What you carried"*: a dynamic count of the mementos you brought to the
  end of the world ("not armour — the better thing the armour was always protecting").
- 🟢 120 scripts / ~17.5k LOC, all clean.

---

## 💞 v1.42.0 — *"Things They Couldn't Say"* — Romance Keepsakes (CODE)

> A second, more intimate tier of keepsakes — the ones a companion only gives you if you became *theirs.*

- 💞 **Four romance keepsakes** (`Keepsakes`, gated on `romance.<id>.consummated`): **Garrow's other
  list** (your name at the top of the people she refuses to bury), **a key to an inn that doesn't exist
  yet** from Roen ("down payment on an after"), **Naeve's revised axiom** ("Premise: you are meaning.
  Checked."), and **the ash of Varra's receipt** ("the day the price hit zero").
- 🎒 They slot straight into the Chronicle's Keepsakes section beside the quest mementos — the collection
  now spans recruitment to the last night. **10 keepsakes total.**
- 🟢 120 scripts / ~17.3k LOC, all clean.

---

## 🎒 v1.41.0 — *"What You Carry"* — Keepsakes (CODE)

> A companion who let you matter to them gives you something to prove it. Small, useless, priceless —
> the opposite of loot.

- 🎒 **`Keepsakes`** — when a personal quest resolves, that companion presses a memento on you, gated on
  the existing resolved-flag (so it just appears): **Garrow's whetstone** ("for the work, whatever god
  you do or don't keep"), **Roen's spare lockpick**, **Varra's char-edged contract-corner** ("proof I had
  a price once"), **Naeve's first proof** ("the one that did no harm"), **Ilfaeril's renounced signet**,
  and **the unclaimed's backwards Kelemvor token** from Mother Cass.
- 📖 **In the Chronicle** — `ChronicleScreen` (press **C**) gains a **🎒 Keepsakes** section listing each
  memento you carry, with its giver and flavor. Pure data; no item/stat plumbing — story made tangible.
- 🟢 120 scripts / ~17.2k LOC, all clean.

---

## 💀 v1.40.0 — *"The Avatar of Bone"* — A Second Optional Era Miniboss (CODE)

> Proving the v1.39 bonus-fight hook reuses cleanly: a one-pass second miniboss, now in the Time of
> Troubles, with full reactive payoff.

- 💀 **The Avatar of Bone** — at the forge where Myrkul's skull becomes the Crown, a shard of dying
  divinity rises to guard the work: a named boss + two God-Touched Horrors + a Bone-Zealot. Added in one
  pass via `SimpleEra.bonusFightId` (the hook from v1.39) — dispatched in `StartLateFight` →
  `BuildAvatarEncounter`.
- 🏆 **Payoffs** — clearing it sets `toot.avatar_down`, nudges **Garrow** approval (+5 — a Doomguide at
  the death of death's own god: *"Even gods get buried. The most comforting thing I have ever seen."*),
  unlocks a **Codex bestiary entry**, and earns a dedicated ending slide.
- 🟢 119 scripts / ~17.1k LOC, all clean. Two of the four eras now offer an optional miniboss; the hook
  makes the others a one-liner away.

---

## 👹 v1.39.0 — *"The Herald of the Unmade"* — A Named Optional Era Miniboss (CODE)

> The eras had a fight apiece; now one of them has a *choice* of fight. An optional, tougher encounter
> against the villain faction's herald — combat content tied straight to the lore.

- 👹 **The Herald of the Unmade** — a second combat marker in the **Spellplague** (the place the Codex
  says the Unmade comes closest to winning): a tanky named boss + two Unmade Aberrations + a Sorrow, in
  the causality-optional blue fire. Built via the proven `EncounterBuilder` flow + `spellplagueHazard`.
- 🧩 **Reusable hook** — `SimpleEra` gains `bonusFightId` / `bonusFightLabel` / `bonusFightDoneFlag`: an
  optional second exit that any config-driven era can offer, shown until its done-flag is set. Wired in
  `EnterSpellplagueAt`; dispatched in `StartLateFight`.
- 🏆 **Payoffs** — clearing it sets `spellplague.herald_down`, nudges **Naeve & Ilfaeril** approval (+4,
  the two who grasp what it is), unlocks a **Codex bestiary entry**, and earns a dedicated ending slide.
- 🟢 119 scripts / ~17.0k LOC, all clean. (Also synced the stale internal FEATURES badge.)

---

## 🗣️ v1.38.0 — *"More Voices at the Fire"* — Group-Banter, Round Two (CODE)

> Three more fireside exchanges, including the camp's first **romance-reactive** beat — a third companion
> noticing what's growing between you and another.

- 🗣️ **Three new `CampGroupBanter` exchanges** (now 8 total): **Ilfaeril & Naeve** (two who ended worlds —
  guilt across a thousand years vs ten), **Varra & Garrow** (the priced soul and the gravedigger who'd cut
  her name deep anyway), and **Roen, on a certain romance** — gated on `romance.garrow.turn`, so Roen only
  ribs you about Garrow once it's real ("you two are broken in exactly the same shape — it's beautiful").
- 🧩 Reuses the existing pair-presence + `requiresFlag` gating, so the romance beat *just works* off the
  data; no new plumbing.
- 🟢 119 scripts / ~16.9k LOC, all clean. The fire's getting chatty.

---

## 💾 v1.37.0 — *"Many Roads"* — A Save-Slot Manager (CODE)

> One autosave was fine for a prototype; a real CRPG wants parallel playthroughs. Now you can keep
> several, see what's in each at a glance, and pick which one the campaign writes to.

- 💾 **`SaveSlotScreen` + `SaveSlots`** — **Load Game / Saves** off the main menu lists four slots
  (Autosave + Slot 1–3), each previewing **hero · level · current scene · timestamp** via the new
  lightweight `SaveSystem.Peek` (no global state touched). Per slot: **Load**, **New Game** (overwrite,
  labeled), and a **confirm-guarded Delete** (`SaveSystem.Delete`).
- 🎚️ **Active-slot routing** — `SaveSlots.Active` (default `"save"`) now drives the campaign director's
  autosave and Continue, set when you pick a slot; `CampaignBootstrap.SaveSlot` reads it, so everything
  back-compat'd without touching the autosave path.
- 🟢 119 scripts / ~16.8k LOC, all clean. Multiple parallel saves, with readable previews.

---

## 🔥 v1.36.0 — *"The Party Falls to Talking"* — Camp Group-Banter (CODE)

> The BG2 staple the camp was missing: two companions sparking off each other across the fire while you
> listen. Each pairs two wounds against each other — and only fires when you've fielded both.

- 🔥 **`CampGroupBanter`** — five two-speaker exchanges, each gated on *both* companions being in the
  active party, played once (`camp.banter.<id>.done`): **Naeve & Garrow** (knowledge meets faith),
  **Roen & Varra** (two ways out of a bad deal), **Ilfaeril & Garrow** (servants of the same cruel law),
  **Varra & Naeve** (a contract and a catastrophe), **Roen & Garrow** (the liar and the woman who can't).
- 🔥 **At the fire** — `CampScene` gains a **"The Party Falls to Talking"** section surfacing the
  best-eligible exchange (`CampGroupBanter.Best()`); listening warms **both** companions' approval (+2).
- 🟢 117 scripts / ~16.6k LOC, all clean. The camp now has cross-talk, not just monologues — the party
  feels like a party.

---

## 🗝️ v1.35.0 — *"The Gutter That Made Him"* — Roen's Home Beat & a Wider Validator (CODE)

> Completes the cast: every companion now has a reactive "home" beat. Roen — the one with no era — gets
> his in the place that *is* his origin.

- 🗝️ **Roen in the Almshouse** — if he's recruited and fielded, the Outer City orphan stands in the kind
  of room he was pulled out of, among the people he could have been (`AlmshouseScene.RoenWitness`). Built
  live from the flags, keyed to how his quest (his sister Wrenna) resolved — saved / double-agent /
  turned-in / unresolved. Sets `almshouse.roen_witnessed` (+5 approval) and earns its own ending slide.
  **Every companion now has a reactive home beat** (eras for four; the Almshouse for Roen).
- 🧪 **Wider validator** — `ContentValidator` now also walks the static `EraWitness` graphs (Garrow/TooT,
  Varra/Spellplague), extending the broken-reference net to the new era content.
- 🟢 116 scripts / ~16.4k LOC, all clean.

---

## ⏳ v1.34.0 — *"Every Age Remembers"* — Reactive Beats for the Last Two Eras (CODE)

> Completes the set from v1.32–33: now **all four time-travel eras** react to a companion you brought
> who lived through them.

- ⚰️ **Garrow in the Time of Troubles** — watching a god of death beaten into the Crown of Horns, she
  reckons with the thing her whole quest circled: *the Doom was made by people, and people can refuse it.*
  Reacts to her quest (doctrine-won / left-faith / recanted / unresolved).
- 🔥 **Varra in the Spellplague** — her pact-flame going feral as the Weave tears, tempted to vanish into
  the blue fire and outrun her bill — and choosing, every time, to come back out for you. Reacts to her
  quest (patron-bound / debt-taken / freed / unresolved).
- 🧩 **Reusable hook** — added `SimpleEra.witnessNameMatch` + `witnessGraph` (+ a `PartyHas` presence
  check), and a new `EraWitness` content class with both flag-reactive graphs; the director wires them in
  `EnterTimeOfTroublesAt` / `EnterSpellplagueAt`. Each sets `<era>.<id>_witnessed` (+5 approval) and earns
  a dedicated `EndingResolver` slide.
- 🟢 116 scripts / ~16.3k LOC, all clean. **All four eras now see who you brought** — Netheril/Naeve,
  Crown Wars/Ilfaeril, Time of Troubles/Garrow, Spellplague/Varra.

---

## 🌌 v1.33.0 — *"Home, the Last Time"* — Naeve Walks Netheril as It Falls (CODE)

> The mirror of v1.32's Ilfaeril beat, on the same reusable pattern: the last daughter of the Seventh
> Enclave, back in her dead world the warm hour before the sky takes it.

- 🌌 **Naeve in Netheril** — if she's recruited *and* fielded when you walk the falling enclave, a new
  beat appears (`NetherilEra.BuildNaeveWitness`): she stands on "the balcony where I derived my first
  proof," among the faces she'd spend a thousand years failing to grieve. Built **live from the flags**,
  reacting to how her personal quest resolved — *rekindled* ("the proof has a kind conclusion"),
  *released* ("some mistakes you carry"), *preserved* ("a tomb I call a home… ask me again after"), or
  *unresolved* (the shard calling her in the present).
- 💞 Sets `netheril.naeve_witnessed` (+5 approval), with a Chronicle note and a dedicated **`EndingResolver`**
  slide — she got to stand in the proof of her life once more, breathing, witnessed, *home.*
- 🔌 Reuses the `PartyHas` presence pattern from v1.32 — both eras now *see who you brought*.
- 🟢 115 scripts / ~16.1k LOC, all clean. Two of the four eras now react to your party's own history.

---

## 🏛️ v1.32.0 — *"Where It Happened"* — A Companion-Reactive Act III Beat (CODE)

> The first reactive moment inside the time-travel eras — and it's the one the whole premise was begging
> for: a companion confronting his own history in the flesh.

- 🏛️ **Ilfaeril in the Crown Wars** — if he's recruited *and* in your active party when you walk the
  elven high court, a new beat appears (`CrownWarsEra.BuildIlfaerilWitness`): he stands at "the third
  bench from the dais" where he raised the hand that helped damn a people. Built **live from the flags**,
  so it reacts to:
  - whether you've **argued the verdict down here** (`crownwars.verdict_spared`) — gratitude vs. a plea,
  - and **how his personal quest resolved** — *commission* ("her forgiveness was a sword"), *forgiven*
    ("bearable, which is more than I earned"), *penance* ("let me at least be useful"), or *unresolved*
    (foreshadowing the reliquary that waits in the present).
- 💞 Sets `crownwars.ilfaeril_witnessed` (+5 approval), with a **Chronicle note** and a dedicated
  **`EndingResolver`** slide — ten thousand years of nightmare, witnessed at last by someone who stayed.
- 🔌 Adds a `PartyHas` presence check to the era; only shows when he's fielded (never alongside his own
  recruit NPC). Pattern is reusable for Naeve-in-Netheril next.
- 🟢 115 scripts / ~15.9k LOC, all clean. Act III stops being just a fight per era and starts *seeing* you.

---

## 📖 v1.31.0 — *"Recorded in the Margins"* — Act II Enters the Codex (CODE)

> The K-screen "fills in as you witness the saga" — but Act II's whole Lower City was missing from it.
> Now the district you've been shaping writes itself into the lore as you live it.

- 📖 **Five new `CodexContent` entries**, each flag-gated like the rest, so they appear only once you've
  seen them: **The Lower City** (`prologue.cleared`), **The Almshouse of the Unclaimed**
  (`almshouse.visited`), **The Faithless Choir** (`quest.choir.resolved`), **The Hollow Cantor**
  (`quest.choir.cell_cleared`), and **The Grave-Tithe** (`quest.tithe.resolved`).
- 🧵 Each is written to braid Act II into the central question — the grave-tithe as "a small mirror of
  the whole cosmic cruelty", the Almshouse's Wall of Names as "the whole argument of your saga, made by
  people who never heard your name."
- 🟢 115 scripts / ~15.7k LOC, all clean. The Codex now covers **19 entries** across Premise / Masks /
  Bestiary / Lore — and Act II is finally in the book.

---

## 🕯️ v1.30.0 — *"The Almshouse of the Unclaimed"* — A Second Explorable Lower City Room (CODE)

> Act II gets a *place* you can walk into, not just markers on the hub: a refuge for the godless poor that
> mirrors your whole run back at you — and restates the game's question in chalk on a wall.

- 🕯️ **`AlmshouseScene`** — a new explorable room (own grid/camera/exploration UI, mirroring the era
  builders), reached by a hub door once the prologue's cleared. Three reactive beats, all read live from
  the flags each entry:
  - **Mother Cass**, the keeper — warm if you've been good to the quarter (tithe freed/paid, choir given
    doubt, the widow's true comfort, the allies pledge, high standing), wary if you took Vane's cut.
  - **A huddle of the unclaimed** — their mood *is* your reputation made flesh (a grateful father, or a
    child pulled behind a skirt, or a fevered hope you caused by speaking for the Unmade).
  - **The Wall of Names** — the unclaimed dead the quarter refuses to let the Wall erase; one chalked low
    in a child's hand: *"MA, WHO SANG."* The whole argument of the game, written by people who never
    heard your name.
- 🪙 **A token to the Court** — do right by them and Cass presses a backwards Kelemvor token into your hand
  (`almshouse.token`); it earns a dedicated **`EndingResolver`** slide and a Chronicle line — the poor get
  someone *in the room* at the finale who promised they were real.
- 🔌 Wired via a new `BaldursGateHub.onEnterAlmshouse` callback + `CampaignBootstrap.EnterAlmshouse`.
- 🟢 115 scripts / ~15.6k LOC, all clean. Act II is now somewhere you can *stand*, not just pass through.

---

## ⚔️ v1.29.0 — *"The Hollow Cantor"* — An Act II Miniboss (Real Combat, Not Just Talk) (CODE)

> Act II's side content has been all moral conversation so far. This gives it *teeth*: the Faithless
> Choir thread escalates into an optional fight against a named leader, routed through the actual engine.

- ⚔️ **The Hollow Cantor** — once the street preaching is resolved (any way), a militant Choir cell
  gathers in the undercroft. A new hub marker opens an **optional skirmish** (`EnterChoirCell` →
  `BuildChoirCellEncounter`): a tanky named leader, two Unmaking Zealots, two Sorrow-wraiths — built with
  the same `EncounterBuilder` flow the personal quests use, with full nameplates/floating-text/SFX.
- 🛡️ **It matters** — clearing the cell sets `quest.choir.cell_cleared`, **+2 Lower City standing**, and
  **+4 approval** with Garrow and Ilfaeril (you stopped a martyrdom before it started).
- 🧵 **Reactive payoff** — a Journey/Lower City entry, a Chronicle note, and a dedicated `EndingResolver`
  slide ("you went down into the dark and broke it before it could make a martyr of anyone").
- 🔌 Wired via a new `BaldursGateHub.onEnterChoirCell` callback; gated so it only appears after the choir
  quest resolves and disappears once cleared.
- 🟢 114 scripts / ~15.4k LOC, all clean. Act II now has a fist as well as a conscience.

---

## 🧮 v1.28.0 — *"Front to Back"* — Isometric Depth Sorting for Sprites (CODE)

> A small correctness fix that makes the v1.22 sprite seam actually *look* right: when art is present,
> units now layer by depth instead of fighting over who's drawn on top.

- 🧮 **`IsoDepthSorter`** — added to each skinned unit's sprite; sets `SpriteRenderer.sortingOrder =
  -(x+y)*100` from world position every `LateUpdate`, so a unit lower/nearer on the iso diamond draws in
  front, and the order updates correctly as units move.
- 🔌 Wired automatically by `UnitSpriteSkinner` (no extra setup); pure no-op until art exists.
- 📄 `docs/ASSET_INTEGRATION.md`: unit-sprite row + checklist now note the live skinner + depth sort.
- 🟢 114 scripts / ~15.2k LOC, all clean. Drop in sprites and the battlefield reads with real depth.

---

## 🔊 v1.27.0 — *"Give It a Sound"* — The Audio Seam (SFX + Music) (CODE)

> The last sense the game was missing. Same philosophy as the sprite, portrait, and VFX seams: silent
> until you drop a clip in a folder, then it *just plays.*

- 🔊 **`AudioSystem`** — one-shot **SFX** from `Resources/SFX/` and looping **music** from
  `Resources/Music/`, self-bootstrapping a hidden persistent driver, no-op when a clip is missing. Master
  volume already rides `AudioListener.volume` (set from the Options slider), so it's governed for free.
- ⚔️ **Combat sound** — `AbilityRunner` plays **impact SFX by damage type** (`hit_fire`, `hit_ice`, …,
  falling back to `hit`), plus `heal` and `miss` — right beside the floating numbers and hit VFX.
- 🎵 **Music per mode** — the director's single `SwapMode` chokepoint swaps the loop by mode: **Hub,
  Combat, Camp, Fugue, Court, Vault, Explore**. A mode with no track keeps the current music rather than
  cutting to silence.
- 📄 `docs/ASSET_INTEGRATION.md`: SFX & Music rows moved to **LIVE**, with the exact clip names.
- 🟢 114 scripts / ~15.1k LOC, all clean. Drop a folder of audio and the cube-game has a soundtrack.

---

## 🖼️ v1.26.0 — *"A Face to the Voice"* — Dialogue Portraits (CODE)

> The art seam reaches conversations. Every line of the game already flows through one dialogue box —
> now that box can wear a face the instant you give it one.

- 🖼️ **`WorldArt.Portrait`** — loads `Resources/Portraits/<speaker>` with a sensible fallback chain
  (full speaker name → first word → the unit's map sprite), cached, returning null when no art exists.
- 🗣️ **`DialogueScreen`** draws a **portrait panel beside the conversation box**, captioned with the
  speaker, using `DrawTextureWithTexCoords` so both standalone and atlased sprites render correctly. No
  portrait present → the box stands alone, exactly as before (fully graceful).
- 🧩 **One face, many lines** — `Portraits/Doomguide.png` covers "Doomguide Knight", "Doomguide
  Enforcer", … ; `Portraits/Tamsin.png` gives the crier a face; reuse a `Sprites/<name>.png` and it
  works for both the map unit and the dialogue.
- 📄 `docs/ASSET_INTEGRATION.md`: portraits moved from "once a field is added" to **LIVE**.
- 🟢 113 scripts / ~15.0k LOC, all clean. Drop one PNG and a character looks you in the eye.

---

## 🪦 v1.25.0 — *"The Wall Remembers"* — A Reactive Fugue Beat Before the Breach (CODE)

> Act II's mercy (or its absence) follows you past the world's edge. The grey waystation before the
> permanent loss now *answers* for how you treated the Gate's poor and dead — and warns, fairly, what
> the Breach will cost.

- 🪦 **A Faithless soul in the Wall** (`FugueEra`) — a half-dissolved soul that greets you in one of three
  registers depending on your Lower City run: **grateful** (you freed the grave-tithe, paid the poor's
  debts, earned the quarter, gave the widow true comfort, or high standing), **bitter** (took a cut,
  silenced the Choir, or negative standing), or **wary-neutral**.
- ⚖️ **Fair foreshadowing of the Breach** — it spells out the Wall's arithmetic plainly ("the only way a
  soul comes *out* is if another goes *in* … it takes the one nearest your heart"), seeding the romance↔
  Breach tithe before it lands — and even needles a corrupt player about haggling the toll down "to
  twenty percent."
- 🔧 Built **live from the flags each descent** (not at construction), so it always reflects the current
  playthrough; sets `fugue.soul.met`.
- 🟢 113 scripts / ~14.9k LOC, all clean. Act II's choices now reach all the way to the lip of the grave.

---

## ⚰️ v1.24.0 — *"The Tithe Collector"* — Who Pays for the Dead's Rest? (CODE)

> A fourth Act II side quest that points the game's central question — *what is a soul worth?* — straight
> at a marketplace: a man who sells the poor the right to a blessed grave.

- ⚰️ **New side quest — "The Tithe Collector"** (`ActTwoContent`) — Collector Vane charges the Gate's poor
  for consecrated rest; can't pay, and your dead wait Faithless in the pauper's pit. A **trilemma**:
  **[Persuasion DC 15] tear up the ledger** (Garrow ↑↑, Lower City ↑), **pay everyone's debts yourself**
  (generous; Garrow ↑, Lower City ↑), or **take a cut** (corrupt — Garrow ↓↓, Ilfaeril ↓, Lower City ↓),
  with a graceful skill-fail branch and **two outcome-gated aftermath NPCs** (an empty table vs. a busy one).
- 🧵 **Reactive everywhere** — three new crier lines, a Journey/Lower City entry, a **Chronicle** line, and
  two **`EndingResolver`** epilogue slides (the pauper's pit blessed for free vs. piling up like Vane's
  ledger — "the Wall looked a little like his accounts").
- 🟢 113 scripts / ~14.8k LOC, all clean. Act II now carries **four** moral side quests, all braided into
  the crier, the journey, the chronicle, and the ending.

---

## 🧪 v1.23.0 — *"Trust, but Verify"* — A Content-Integrity Validator (CODE)

> The sandbox has no Unity compiler, so the one bug class that can slip through is a mistyped node id in
> a conversation. This closes that gap: a smoke test that proves every dialogue link resolves.

- 🧪 **`ContentValidator`** — walks every reachable authored `DialogueGraph` (all five personal quests via
  their `PersonalQuest` graphs, the three Act II side quests + crier + aftermaths, Aldric's tea, the
  Lady's two riddles) and reports **broken `nextNodeId`/`failNodeId`/`autoNextNodeId`/`startNodeId`,
  duplicate node ids, and empty/no-node graphs** — each with the exact graph + node.
- ▶️ **`ValidationDemo`** — one-click: runs it on Play and prints a **PASS/FAIL** report to the Console
  *and* on-screen (graph count, node count, every issue). Run it after editing dialogue.
- 🟢 113 scripts / ~14.6k LOC, all clean. The authored narrative now has a regression net.

---

## 🎨 v1.22.0 — *"The Art Seam"* — Cubes Become Sprites the Moment Art Arrives (CODE)

> The cleanest possible on-ramp from "systems made visible" to "looks like a game": you don't touch a
> line of logic — you drop a PNG in a folder.

- 🎨 **`UnitSpriteSkinner`** (on the campaign director) scans `GridUnit`s and, when `WorldArt` finds a
  matching sprite in `Resources/Sprites/`, **hides the placeholder cube and parents a camera-facing
  sprite billboard** (a new `CameraBillboard`) — keeping the collider so selection still works. No art
  present → no change (graceful, exactly like the VFX system).
- 🗂️ **Sensible lookup** — full display name → first word → faction, so **one `Sprites/Enemy.png`
  re-skins every foe at once**, while `Sprites/Garrow.png` upgrades her alone. No per-unit wiring.
- 📄 Updated `docs/ASSET_INTEGRATION.md` — §2 goes from "PLANNED" to **"HOOK READY"** with the exact
  drop-in convention.
- 🟢 111 scripts / ~14.4k LOC, all clean. The seam between code and art is now a single folder.

---

## 🔢 v1.21.0 — *"Numbers That Fly"* — Floating Combat Text (CODE)

> The natural partner to the nameplates: now you don't just see *who's* hurt, you see the hit land.

- 🔢 **`FloatingCombatText`** — damage numbers, **+heals** (green), **MISS/RESIST** (grey), oversized
  golden **crits**, and condition names pop off the target, drift upward, and fade, with a cheap shadow
  for legibility. Self-bootstraps a hidden driver on first use; pure IMGUI, world-space, zero art.
- 🎯 **Emitted at the source of truth** — `AbilityRunner` fires the right popup straight off the resolved
  `AttackResult` (after difficulty scaling), so it's correct everywhere combat happens: campaign, eras,
  demos. No per-encounter wiring.
- 🟢 110 scripts / ~14.3k LOC, all clean. With v1.20's nameplates, a cube-fight now reads at a glance.

---

## 🪧 v1.20.0 — *"Faces in the Fray"* — World-Space Nameplates & HP Bars (CODE)

> The biggest *visible* upgrade yet, and a pure win for previewing in Unity: the colored cubes stop being
> anonymous. Now you can read a fight at a glance.

- 🪧 **`UnitNameplates`** — a floating **name + faction-colored HP bar** over every `GridUnit`, projected
  to screen each frame, with the **active combatant highlighted** (gold ▶ + outlined bar) and downed units
  greyed as "(down)". HP bar shades green → amber → red as it drops. Toggle with **N**.
- 🔌 **Wired in where it matters** — added to the persistent campaign director (covers hub, combat, eras,
  every mode) and to `DemoBootstrap` (the flagship combat demo). Drop-in for any scene; it finds units itself.
- ❔ Added to the **Help** card and controls legend.
- 🟢 109 scripts / ~14.2k LOC, all clean. The systems-made-visible game just became *readable* at a glance.

---

## ⚔️ v1.19.0 — *"Knocked Out, Not Dead"* — Combat Consequences & a Breathing Bond (CODE)

> The last open roadmap item — approval that moves *in play* — plus a fix to an unintended cruelty: a
> companion downed in some random skirmish should not be gone forever. Only the Breach gets to do that.

- 🩹 **Stabilize-after-victory** — when the party wins, any **downed (0-HP) companion is pulled back from
  the brink** (revived to ¼ HP), with a line in the log. Random battles no longer risk a permanent
  companion loss; **permanent death stays reserved for the Breach**, exactly as the story intends.
- ❤️ **Combat-time approval nudges** — surviving a fight together moves the meter: every fielded companion
  gains **+1 approval** on victory, and **+3** for anyone you had to pull back from the brink. Lives at the
  single combat-end chokepoint (`EncounterController.AwardExperience`), so it applies to **all** combat —
  campaign, eras, demos — keeping affinity breathing across the whole saga, not just in set-piece dialogue.
- 🟢 108 scripts / ~14.0k LOC, all clean. **The full companion roadmap is now complete.**

---

## 🕯️ v1.18.0 — *"The Faithless Choir"* — The Unmade Comes to the Lower City (CODE)

> Act II stops being only personal errands and starts braiding into the *main* plot: the villain faction
> gets a face on a street corner, and your answer to it follows you to the Court.

- 🕯️ **New side quest — "The Faithless Choir"** (`ActTwoContent`) — Brother Sere preaches the Unmade to
  the Gate's grieving poor, holding *you*, the Returned, up as proof the Wall can fall. A **trilemma**:
  **[Religion DC 15] give him doubt** (you've stood at the Wall; the cure is another atrocity — the
  nuanced best, +Ilfaeril/Garrow), **set the Fist on him** (suppress — Kelemvor ↑, Lower City ↓, the
  grief just goes underground), or **speak for the Unmade** (`choir.sympathizer` — the Choir remembers
  who spoke for it; −Ilfaeril). With a graceful skill-fail branch.
- 🧵 **Reactive everywhere** — Tamsin the crier gains three new conditional lines for your Choir stance;
  the Journey screen's Lower City section and the Chronicle both track it; and `EndingResolver` adds a
  **Faithless-Choir epilogue slide** (doubt → he sits with the dying; suppressed → it surfaced harder;
  spoken-for → they called you theirs).
- 🟢 108 scripts / ~13.9k LOC, all clean. The Lower City now reaches *up* into the central question, not
  just sideways into errands.

---

## 📖 v1.17.0 — *"The Chronicle of the Returned"* — A Run Summary, Any Time (CODE)

> A sprawling, reactive saga is hard to hold in your head. Now you never have to: one keystroke shows
> you the whole shape of your playthrough, and the ending recaps it as a capstone.

- 📖 **`ChronicleScreen` (press C)** — a persistent, press-anytime overlay (beside Options/Help) listing
  the run at a glance: **eras walked; who's still at your side, taken by the Wall, or walked away; quests
  resolved (N/5); hearts given; bonds broken; Lower City standing; the Lady's riddles; endings unlocked;
  difficulty.** Pure read of the flags via the new `EndingResolver.Chronicle()`.
- 🌅 **Recap at the close** — the **ending screen** now has a *"Read the Chronicle of the Returned"*
  expander, so the Court of the Dead leaves you with the whole journey, not just its last image.
- ❔ Added to the **Help** card (`H`) and the controls legend.
- 🟢 108 scripts / ~13.7k LOC, all clean. The saga is now legible in a single keystroke.

---

## 🏙️ v1.16.0 — *"The Quarter Pays Its Debts"* — Lower City Standing Reaches the Ending (CODE)

> The Act II reputation you build now follows you all the way to the Court of the Dead — the small good
> (or its absence) gets the last word it deserves.

- 🤝 **A reputation payoff** — once `reputation.lowcity` ≥ 5, **Tamsin the crier** offers a one-time
  pledge (`lowcity.allies`): the Gate's poor, godless, and grieving will stand with you at the end. Gated
  via a `RequireIntAtLeast` conditional choice, so it only appears when earned and only once.
- 🌅 **It lands in the finale** — `EndingResolver` adds a **Lower City epilogue slide**: a moving "they
  were there, with bread and lanterns and a hundred small refusals to look away" for high standing or the
  pledge — and a quiet, cutting "you spent them like coin" for negative standing.
- 📜 **Legible** — the Journey screen's **Lower City** section now shows the pledge ("the quarter stands
  with you — they'll be there at the end").
- 🟢 107 scripts / ~13.5k LOC, all clean. Act II's small kindnesses now echo in the last accounting.

---

## 🧭 v1.15.0 — *"Never Lost, Never Stuck"* — Help Card & Autosave-on-Rest (CODE)

> Two small comforts that make a sprawling systems-game approachable: you can always find out what a key
> does, and you never lose much progress.

- ❔ **Help overlay** (`HelpOverlay`) — press **H** anytime, in any mode, for a scrollable reference card:
  world controls, battle controls, every hotkey screen (J/P/K/I/O/B), and saving. Lives on the persistent
  director object, so it's always one key away.
- 💾 **Autosave on long rest** — resting at camp now autosaves (`CampScene.onRested` → the director's
  `Autosave`), the natural CRPG checkpoint, with a "progress saved" note at the fire. Decoupled via a
  callback, so demos that use the camp don't touch the save file.
- 🟢 107 scripts / ~13.4k LOC, all clean.

---

## 🎚️ v1.14.0 — *"Your Table, Your Rules"* — Difficulty Modes (CODE)

> Some players are here for the tactics; some are here for the story and want the dice to get out of the
> way. Now both are first-class.

- 🎚️ **Story / Normal / Hard** (`GameSettings.Mode`) — chosen in the Options screen (off the menu or
  in-game with **O**), persisted via `PlayerPrefs`. **Story** softens enemy damage (×0.6) and buffs the
  party (×1.35); **Hard** sharpens enemy damage (×1.4) and nerfs the party a touch (×0.9); **Normal** is
  the intended 5e balance.
- 🎯 **One chokepoint, applies everywhere** — scaling lives in `AbilityRunner` between resolve and apply,
  keyed off the target's faction, so it covers **all** combat: the campaign, every era, and every demo —
  no per-encounter wiring.
- 🟢 106 scripts / ~13.2k LOC, all clean. The table now flexes to the player at it.

---

## ⚓ v1.13.0 — *"What You Stand To Lose"* — Romance ↔ the Breach & the Finale Anchor (CODE)

> Love stops being a side-quest with a happy slide and becomes the highest-stakes choice in the game.
> Whoever you let yourself love is now who the Wall reaches for — and who the whole ending is *about.*

- ⚰️💔 **The Wall takes what you love** — `ChooseBreachVictim` now, once Varra is safe (pact broken),
  is drawn to a **committed romance** (`romance.<id>.turn`) among the Breach candidates before falling
  back to a volunteer or the least-loved. Romancing someone makes the Breach a genuine stake for *them.*
- ⚓ **The finale anchor** — `EndingResolver.AnchorId()` picks the face the cosmos rests on: the deepest
  romance (consummated → committed → the turn), or, lacking one, the companion you let in closest by
  approval. Every ending now closes on an **anchor beat** keyed to whether they're beside you, **lost to
  the Breach**, or **driven away** — three very different last words.
- 💔 **The cruelest epilogue** — a dedicated *"the one you loved, taken by the Wall"* slide (a bespoke
  Varra variant: "the receipt collected before you ever got to burn it"), separate from the generic loss
  slide, so a romanced Breach death lands with its full intended weight.
- 🟢 106 scripts / ~13.1k LOC, all clean. The game's tenderness and its cruelty now share one mechanism.

---

## 🏙️ v1.12.0 — *"The City Watches Back"* — Act II Side Content & a Reactive Lower City (CODE)

> Act II stops being a corridor between eras and starts being a *place.* The first pass at the hub's
> connective tissue: a world that reads your choices back to you, and small moral side quests whose
> stakes are reputation and conscience rather than loot.

- 🏙️ **`ActTwoContent`** — a Lower City content layer the hub places by flag-gate (active quest vs.
  resolved aftermath), the same way it gates Roen/Varra recruitment.
- 📰 **Tamsin, the street crier** — a living "news of you" board: her lines appear only as you *earn*
  them, via **conditional dialogue choices** (`DialogueChoice.conditions`) read live off `GameFlags` —
  the widow you helped, the Faithless man you freed (or didn't), the falling city you walked, the verdict
  you argued down, a romance the quarter is scandalised by, a companion who walked out.
- ⚖️ **Two thematic side quests**, dialogue-resolved (no grind):
  - **"The Widow's Coin"** — Mhaere's son was taken in the harvest, denied rest. A **kind lie**, the
    **hard truth** (Garrow approves), or an **[Insight DC 14]** *true* comfort that reframes remembrance
    as resistance to unmaking (the best — ties straight to the game's central question).
  - **"The Fist and the Faithless"** — a sergeant ordered to hand a godless beggar to the Doomguides.
    **[Persuasion DC 14]** free him (Garrow ↑, Kelemvor ↓), **bribe** the paperwork away, or **uphold the
    law** (Kelemvor ↑, Garrow & Ilfaeril ↓). Each moves `reputation.lowcity`.
- 📜 **Journey legibility** — the **J** screen gains a **Lower City** section (standing + each side
  quest's outcome), so the new content reads as clearly as it plays.
- 🎬 **`ActTwoDemo`** — one-click: walk the hub, resolve a side quest, then re-ask the crier and watch the
  city's tale of you change.
- 🟢 106 scripts / ~13.0k LOC, all clean. The hub now *reacts.* (Next: deeper Act II — multi-room areas,
  Fugue-side beats, and tying side reputation into the finale.)

---

## 💔 v1.11.0 — *"The Bonds That Break"* — Rupture / Betrayal Arcs (CODE)

> The dark mirror of the romances. If love is a stake, this is the bill: drive a companion's approval into
> the ground — keep crossing the one line their whole self is built on — and the bond *frays.* The same
> data-first shape as the romances, pointed the other way.

- 💔 **Rupture engine** (`RuptureContent`) — five companions (Garrow, Roen, Varra, Naeve, Ilfaeril), each
  with a value-line that, crossed too often (approval ≤ −25), triggers a **camp confrontation** in their
  own voice — desecration for Garrow, betrayed trust for Roen, the receipt for Varra, reckless reaching
  for Naeve, vengeance-wearing-mercy for Ilfaeril.
- 🜍 **Three outcomes, and amends has teeth** — **make amends** (only lands if you've earned *standing* —
  `HasStanding`: a night-talk shared or a romance spark; you can't talk back someone you never bothered
  to know → `rupture.<id>.mended`, approval floored back up), **patch it over** (they stay, guarded →
  `rupture.<id>.uneasy`), or **let them walk** (`rupture.<id>.broken` + `companion.<id>.left`, removed
  from the party via `Party.Remove`). A thin amends that lacks standing collapses to patch-or-part.
- 🔥 **Surfaced at the fire** — `CampScene` shows an urgent **"A Bond Frays"** section above everything
  else when `RuptureContent.Pending()` finds a cratered, present, unresolved companion.
- 🌅 **The Court remembers** — `EndingResolver` adds a **walked-away epilogue slide** for any
  `companion.<id>.left` ("you'll never know how it ended for them — that is its own kind of loss").
- 🎬 **`RuptureDemo`** — one-click: a party with every bond cratered; two seeded with standing (amends can
  land), two without (words alone won't do it).
- 🟢 104 scripts / ~12.5k LOC, all clean. Love and its opposite now both have mechanical weight.

---

## ⚙️ v1.10.0 — *"Quality of Life"* — Options & a Legible Quest Log (CODE)

> A polish pass that makes the game *yours* to tune and the saga *legible* at a glance — the connective
> tissue a real player feels even when they never think about it.

- ⚙️ **Options screen** (`GameSettings` + `SettingsScreen`) — three knobs, persisted via `PlayerPrefs`
  and applied live: **ambient-banter toggle, master volume, dialogue text-speed** (or instant). Reachable
  off the main menu **and** in-game at any time with **O** (a persistent overlay above the mode root).
- ⌨️ **Typewriter dialogue** — `DialogueScreen` now reveals lines at the chosen chars/second, with a
  one-click "show the rest" to skip the reveal; honors the Instant-text setting. Wired to the live setting,
  so dragging the slider in Options changes the feel immediately.
- 🔇 **Banter respects the setting** — `AmbientBanter` now checks `GameSettings.BanterEnabled` (the **B**
  quick-mute still works too).
- 📜 **Quest-log beat tracking** — the Journey screen (**J**) now shows each companion quest's **current
  beat** (thread opened → at the scene → fought → *resolved-how*, e.g. "doctrine on trial", "contract
  burned") and a new **Bonds** section tracking each romance's furthest stage (a spark → trust → the turn
  → committed → the last night).
- 🟢 102 scripts / ~12.1k LOC, all clean. The companion content now *reads* as clearly as it plays.

---

## 💞 v1.9.0 — *"The Slow Burns"* — All Four Romance Arcs (CODE)

> The companions stop being people you fight beside and become people you stand to *lose.* Four full
> romances, each an **argument about one of the game's themes** — so falling in love is also taking a
> position on what the game is about. Built data-first, the way the personal quests were.

- 💞 **Romance engine** (`RomanceContent`) — a reusable, data-driven arc table on a shared six-stage
  spine: **Spark → Trust → Turn → Crisis → Choosing → The Last Night.** Each beat is gated by **both** an
  approval threshold **and** progression — the previous stage, and for the **Crisis**, the companion's
  *resolved personal quest* (the romance literally runs through their quest). Mirrors `CampContent`: pure
  data, no scene boilerplate.
- 🎭 **Four arcs, four themes** — **Garrow** (faith: something can be holy in a cruel cosmos), **Roen**
  (belonging: the bravest thing is to *stay*), **Naeve** (guilt: are you allowed to be loved after what
  you did), **Varra** (worth: you are more than your price). Key-beat dialogue drawn from
  `docs/story/16_ROMANCE_SCRIPTS.md`, each with a **deepen-or-stay-platonic** choice (holding is
  non-destructive — the beat waits for you).
- 🔥 **Played at the fire** — `CampScene` gains a **"Grow Closer"** section beside the night-talks,
  surfacing the best-eligible beat (`RomanceContent.Best()`); choosing someone moves their approval.
- 🜍 **Love is a stake, not a collectible** — pursuing two romances past the **Turn** is gently
  disallowed (`CommittedElsewhere`); a single, in-character commitment.
- 🌅 **Epilogue payoffs** — `EndingResolver` adds a bespoke slide per consummated romance, including the
  game's most devastating beats: **Garrow gives you your own last rite in Break the Loop**, **Naeve
  derives the rewritten Law beside you in Jergal's Keyhole**, **Roen at the niche with no joke left**,
  and **Varra burning the receipt** for her soul at last.
- 🎬 **`RomanceDemo`** — one-click tour: all four companions, gates pre-seeded, every arc open to walk.
- 🟢 100 scripts / ~11.8k LOC, all clean. **The companion pillar is whole: recruit → banter → approval →
  camp → personal quest → romance → epilogue.**

---

## 🕊️ v1.8.0 — *"The Vote"* — Ilfaeril's Personal Quest (& the Companion Pillar Complete) (CODE)

> The fifth and final companion arc on the v1.5.0 template — and with it, **all five companions now have
> a playable personal quest, end to end.** Ilfaeril faces the one soul he owes across ten thousand years,
> and the choice you help him make is the one his whole being turns on: *is atonement allowed to end?*

- 🕊️ **Ilfaeril's quest — "The Vote"** (`IlfaerilQuestContent`) — an ancient elven reliquary, its lock
  keyed to **Maerith of the Singing Vale**, one of the souls his Crown-War council voted to *unmake* —
  the verdict that hardened into the **Law of the Faithless.** Its waking draws the **Choir of the
  Unmade**, who revere him as their first witness and offer to end his guilt by proving he never should
  have felt any. Three hand-written conversations (arrival / the Pale Cantor / the resolution).
- 🜍 **A moral trilemma** keyed to his wound (*atonement is not self-erasure; he is allowed to stop
  paying*): **refuse her forgiveness** and guide the lost forever (+10, devotion that is also a kind of
  hiding), **accept it** and let himself be more than his worst vote (+20, the Oath of Redemption
  fulfilled), or an **[Insight DC 16]** gambit that hears her forgiveness for what it is — *not a door,
  a sword held hilt-first* — and turns it into a **commission to tear the Wall open for the rest of her
  people** (+30, forgiveness that puts a man back to work) — with a graceful fail branch. *(First quest
  to gate on Wisdom/Insight rather than Cha/Int.)*
- ⚔️ **The Choir, come for the witness** — a five-enemy encounter (the Pale Cantor, two Choir Heralds,
  two Unmaking Acolytes) routed through the director, returning to the crypt for the resolution on victory.
- 🌅 **Epilogue payoff** — `EndingResolver` adds an **Ilfaeril slide** keyed to his choice (commission /
  forgiven / penance), alongside the other four.
- 🪝 **Hub thread now playable** — Ilfaeril's reliquary hook (`CompanionQuests`) flips to `playable =
  true` with a `followLabel`; also fixed a pronoun slip in its tease (canon: *he* renounced his house).
- 🎬 **`IlfaerilQuestDemo`** — one-click preview of all three resolutions.
- 🟢 98 scripts / ~11.3k LOC, all clean. **The companion-quest pillar is complete: five for five.**

---

## ✨ v1.7.0 — *"After the Sky Fell"* — Naeve's Personal Quest (CODE)

> The fourth companion arc, again almost pure content on the v1.5.0 template: Naeve goes home to a
> fragment of Netheril that never finished falling — and the choice you help her make is the one her
> whole wound turns on: *some mistakes can only be carried, not erased.*

- ✨ **Naeve's quest — "After the Sky Fell"** (`NaeveQuestContent`) — a shard of dead Weave hums her
  home's exact note: a piece of the **Seventh Enclave**, held aloft a thousand years past its death by a
  failing **mythallar**. The echo of the **Steward Vaelin** tells her the truth — *her own* wardstone
  saved this fragment by sentencing it to fall slowly, and keeping it frozen has been draining the last
  live Weave in the world. Three hand-written conversations (arrival / Vaelin's echo / the resolution).
- 🌌 **A moral trilemma** keyed to her wound (*her genius ended a world; she must learn to carry, not
  erase*): **freeze the shard in stasis** and keep the last of Netheril whole (+10, a tomb she calls a
  home), **let it finish falling** and carry the grief instead of the wreckage (+20, acceptance), or an
  **[Arcana DC 16] gambit** that opens the wardstone and gives its live Weave *back to the present* as a
  seed instead of a shroud (+30, "an arithmetic that builds") — with a graceful fail branch that
  collapses to the two plain choices. (A faint *you-are-a-loop* foreshadow hides in the margin note.)
- ⚔️ **The last protocol** — a five-enemy encounter (a Mythallar Ward, two Arcane Sentinels, two
  Weave-wraiths) routed through the director, returning to the core for the resolution on victory.
- 🌅 **Epilogue payoff** — `EndingResolver` adds a **Naeve slide** keyed to her choice (rekindled /
  released / preserved), alongside Roen's, Varra's, and Garrow's.
- 🪝 **Hub thread now playable** — Naeve's shard hook (`CompanionQuests`) flips to `playable = true`
  with a `followLabel`, opening the quest through the existing generic hub wiring.
- 🎬 **`NaeveQuestDemo`** — one-click preview of all three resolutions.
- 🟢 96 scripts / ~11.0k LOC, all clean. **Four playable companion quests; one to go.**

---

## ⚖️ v1.6.0 — *"A God-Shaped Hole"* — Garrow's Personal Quest (CODE)

> The third companion arc, built almost entirely from content on the v1.5.0 template: Sister Garrow is
> summoned to a Kelemvorite tribunal for heresy, and the choice you help her make is the one the whole
> saga is really about — *what is a soul worth that no god ever claimed?*

- ⚖️ **Garrow's quest — "A God-Shaped Hole"** (`GarrowQuestContent`) — a black-edged writ calls Sister
  Garrow to a Doomguide tribunal to answer for failing to lay her fallen mentor and for travelling with
  **the Returned** — a soul no god claimed. **Justiciar Veld**, who taught her catechism, means to make
  her recant or be cast out; the trial is rigged, the verdict pre-written, so it becomes a fight. Three
  hand-written conversations (arrival / the Justiciar's charge / the resolution).
- 🜍 **A moral trilemma** keyed to her wound (*obedience that felt like betrayal*): **recant to keep the
  grey** and do the work from inside (+10, a brittle, sorrowful faith), **walk away from the faith
  entirely** and lay the dead under no one's law (+20, Faithless and unafraid), or a **[Religion DC 16]
  gambit** that puts Kelemvor's *own canon* on trial — proving the Faithless judgment is itself the
  heresy (+30, the seed of the rewritten Law) — with a graceful fail branch that collapses to the two
  plain choices.
- ⚔️ **The Justiciar's tribunal** — a five-enemy encounter (Veld, two templar inquisitors, two grey
  enforcers) routed through the director, returning to the temple for the resolution on victory.
- 🌅 **Epilogue payoff** — `EndingResolver` adds a **Garrow slide** keyed to her answer (reformer /
  apostate / brittle keeper), alongside Roen's and Varra's.
- 🪝 **Hub thread now playable** — Garrow's summons hook (`CompanionQuests`) flips to `playable = true`
  with a `followLabel`, so the existing generic hub wiring opens the quest with no bespoke code.
- 🎬 **`GarrowQuestDemo`** — one-click preview of all three resolutions.
- 🟢 94 scripts / ~10.8k LOC, all clean. **Three playable companion quests; the template keeps delivering.**

---

## ⚖️ v1.5.0 — *"The Bill Comes Due"* — A Reusable Quest Engine & Varra's Arc (CODE)

> The personal-quest pattern becomes a *template*: one data-driven scene now runs every companion's
> arc, and Varra's is the first new quest built on it — an infernal contract, not a sibling rescue.

- 🧩 **`PersonalQuestScene` + `PersonalQuest`** — Roen's bespoke scene is refactored into a **reusable,
  data-driven stage**. The arc shape (arrive → examine → confront → fight → moral call → leave) is now
  code; *what* happens is config. Each remaining companion quest is mostly **content, not boilerplate.**
- ⚖️ **Varra's quest — "The Bill Comes Due"** (`VarraQuestContent`) — her patron calls the contract she
  signed at **six years old**, early. In a deconsecrated chapel, the cambion broker **Quill** who sold
  her waits to collect. A fight (Quill + contract-devils + imps of the fine print) and a **moral
  trilemma**: **carry her debt yourself** (+25, she sleeps for the first time in 20 years), **let her
  burn it freely** (+15, her choice, her clock), or an **[Arcana DC 16] loophole** that binds her
  *patron* instead (+30, the best outcome) — with a graceful fail branch.
- 🌅 **Epilogue payoff** — `EndingResolver` adds a **Varra slide** keyed to your call (patron-bound /
  debt-taken / freed), alongside Roen's Wrenna slide.
- 🪝 **Generic hub threads** — the hub now places a "follow the thread" marker for **any** playable,
  started-but-unresolved companion quest (`CompanionQuests.playable`), wired through a single
  `onEnterPersonalQuest(id)` callback. Roen's old bespoke wiring is gone.
- 🎬 **`VarraQuestDemo`** — one-click preview of all three resolutions.
- 🧹 Removed the now-redundant `RoenQuestScene` (folded into `PersonalQuestScene`).
- 🟢 92 scripts / ~10.6k LOC, all clean. **Two playable companion quests; a template for the rest.**

---

## 🗡️ v1.4.0 — *"The Honest Lie"* — Roen's Personal Quest, End to End (CODE)

> The first companion thread becomes a *playable arc*: a silent Harper safehouse, a sister who lied
> for two years to keep her brother off the Wall, a Doomguide cell, and a moral call that follows you
> all the way to the epilogue. The hook from v1.3.0 now has a quest on the end of it.

- 🗺️ **`RoenQuestScene`** — a bespoke arc on the SimpleEra pattern with its own beat order: **arrive** at
  the safehouse (auto-dialogue), examine the scene, **corner Wrenna** for the reveal, fight, then make
  the call. State-driven by `roen.quest.cleared` (the fight) and `quest.roen.resolved` (the choice).
- 📜 **`RoenQuestContent`** — three hand-written conversations (Arrival, the Wrenna reveal, the
  Resolution) with a real **moral trilemma**: **forgive** her (Roen **+20** approval, Wrenna saved),
  **condemn** her by the rules (**−15**, a Harper intel boon), or a **[Persuasion DC 15] gambit** to
  turn her into a double agent (best outcome: **+25**, both boons) — with a graceful fail branch.
- ⚔️ **The Doomguide cell** — a six-enemy encounter (knights, enforcers, a zealot, an interrogator)
  routed through the director, returning you to the safehouse for the resolution on victory.
- 🌅 **Epilogue payoff** — `EndingResolver` now adds a **Wrenna slide** keyed to your call, so the
  safehouse choice echoes in the Court of the Dead (saved / double-agent / turned-in variants).
- 🪝 **Hub thread** — once you've examined the cipher hook, a *"Follow the cipher"* marker opens the
  quest; it closes once resolved. Wired into the director (`EnterRoenQuest` / `StartRoenFight`).
- 🎬 **`RoenQuestDemo`** — one-click preview (skips the battle) to tour all three resolutions.
- 🟢 89 scripts / ~10.0k LOC, all clean. **The first companion quest is fully playable.**

---

## 🧵 v1.3.0 — *"Threads to Pull"* — Approval in Play & Companion Personal-Quest Hooks (CODE)

> The affinity loop closes end-to-end: kindness now *moves the meter in normal play*, and opening
> someone up at the fire hangs a thread in the world — the start of their personal quest.

- ❤️ **Approval-shifting dialogue** — the Roen and Varra recruit conversations now **grant approval**
  through `companion.<id>.approval` (warmer choices give more), so the meter rises in ordinary play —
  not just demos. The kindness → approval → camp night-talk loop now runs **without scaffolding.**
- 🧵 **Companion personal-quest hooks** (`CompanionQuests`) — once a campfire **night-talk** opens a
  companion up, a pointer to their **Act II personal quest** appears in the hub (Roen's Harper cipher,
  Varra's matured invoice, Garrow's heresy summons, Naeve's shard of dead Weave, Ilfaeril's reliquary).
  Examining it **starts the quest** (`quest.<id>.started`) and reads its hand-written tease.
- 🪝 **`Interactable.onExamined`** — examine markers can now fire a code callback (used to flag quest
  starts); a small, reusable hook for any "inspecting this *does* something" beat.
- 📜 **Journey tracker** now lists a **Companion Threads** section — which personal quests you've opened
  and started — so the new content is legible at a glance (press **J**).
- 🟢 86 scripts / ~9.5k LOC, all clean.

---

## 🔥 v1.2.0 — *"The Fire's Low"* — Camp, Long Rest & Approval-Gated Night-Talks (CODE)

> The quiet half of a CRPG. A place to rest your wounds and refill your magic — and a campfire
> where the companion who trusts you most finally says what they've been carrying.

- 🔥 **`CampScene`** — a campfire mode reached from a **new hub marker**: a **long rest** restores the
  fielded party to **full HP** and **refreshes all spell slots**, and a **"sit with someone"** beat
  offers a night-talk from whichever companion is closest to you.
- 💬 **Approval-gated night-talks** (`CampContent`) — five hand-written monologues (Roen, Varra, Garrow,
  Naeve, Ilfaeril), each unlocked only once that companion's **approval crosses a threshold** *and*
  they're **fielded**. One per companion; listening itself earns a little more approval. The new
  approval meter (v1.1.0) now *pays off.*
- 🎬 **`CampDemo`** — one-click: a battle-worn party at the fire with approval pre-seeded and a wounded
  companion, so you can watch the rest heal them and hear a night-talk immediately.
- 🟢 84 scripts / ~9.3k LOC, all clean.

---

## 📖 v1.1.0 — *"What You've Witnessed"* — The Codex, Companion Sheets & Ambient Banter (CODE)

> The world starts talking back. A compendium that fills in as you witness the saga, full stat
> blocks for your companions (with how much they *like* you), and idle chatter on the road.

- 📖 **`CodexScreen`** (press **`K`**) — a lore & bestiary compendium with a category rail (The Premise,
  The Four Masks, Bestiary, Lore & History) and a reading pane. Entries live in **`CodexContent`** and
  are **gated by `GameFlags`** — you only know what you've witnessed, so the Codex *grows as you play*
  (Aldric unlocks at tea, the Unmade at Netheril, the Last Returned at the Crown Wars, the Lady at the
  Reader's Boon…). Shows **N/total discovered.**
- 🛡️ **Companion sheets** — the Party screen (**`P`**) now expands on click: full **ability block**
  (STR–CHA with modifiers), **AC / HP / initiative / proficiency**, and an **approval meter** showing
  how much each companion likes you (devoted → hostile), read from `companion.<id>.approval`.
- 💬 **`AmbientBanter`** — idle party chatter while you wander, drawn from **`PartyBanter`** and filtered
  to **who's actually in the field** (Roen, Varra, Garrow, Naeve, Ilfaeril each have voice; plus eerie
  anyone-lines about the Wall). Fades in low in the frame; **mute with `B`.**
- 🔖 **Codex hooks** — `EnterVault` now sets `riddle.entered` so the Vault of Tens self-documents.
- 🟢 82 scripts / ~9.1k LOC, all clean.

---

## 🏛️ v1.0.0 — *"The Saga, Made Legible"* — Journey Tracker, Roster Management & the Vertical-Slice Capstone (CODE)

> The slice now reads as a *game with a spine*: a quest log that tracks the whole arc, a party
> bench, a finale that's earned rather than handed to you, and a player-facing how-to-play guide.

- 📜 **`JourneyScreen`** (press **`J`**) — a live **quest tracker** read straight out of `GameFlags`:
  the Cinderhaunt, tea with Aldric, the Breach, all **four eras**, the **Vault of Tens**, and the
  **Reader's Boon** — each ✔/○ — plus **"Endings unlocked: N/6"** and a **★ golden road** indicator
  when one of the two deepest endings is reachable. The saga finally reads as a checklist.
- 🛡️ **`RosterScreen`** (press **`P`**) — **bench/field** any recruited companion up to `maxActive`;
  the **protagonist is locked into the field.** Benched companions keep their levels and gear.
- 🏛️ **Finale gating tightened** — the **Court of the Dead** now opens only after you've **walked at
  least one era** (`netheril.cleared`), so the ending is a *culmination*, not a shortcut. The Journey
  screen tells you exactly why it isn't open yet.
- 📘 **`docs/HOW_TO_PLAY_THE_SLICE.md`** — a 20-minute guided run from the title screen to the Court of
  the Dead: the loop step-by-step, the full control map, and how to read the Journey & Party screens.
- 🟢 78 scripts / ~8.8k LOC, all clean. **Boot → New Game → play the whole arc, now with a quest log.**

---

## 🎛️ v0.9.3 — *"A Front Door"* — Main Menu, Continue, Roaming Tally & Sprite-Ready Units (CODE)

> The wrapper that turns a pile of demos into a *game you boot up.*

- ▶️ **`MainMenu`** — a real front end: **New Game / Continue / Quit.** Drop it in the boot scene; it
  launches `CampaignBootstrap` fresh or resumes the save.
- 💾 **Save & Continue, deepened** — the save now captures the **hero's build** (name, class, race,
  level, ability scores); the campaign **autosaves on every return to the hub**, and **Continue**
  reconstructs the hero *and* re-recruits your roster from the `companion.*.recruited` / `.lost` flags
  (permanent losses stay lost — the Wall keeps what it took, across sessions).
- 🎲 **Roaming Tally** — after you've walked an era, the Lady in the Margins **pops in at the hub**
  (G-Man style) with a one-off roaming riddle; answer it for her amusement. Appears once.
- 🖼️ **Sprite-ready units** — `WorldArt` + a `GridUnit` hook: drop `Resources/Sprites/<Name>.png` and
  that character renders as a sprite billboard instead of a cube — *zero wiring.* No art? Cubes, as
  before. The bridge from prototype to a real CRPG look.
- 🟢 76 scripts / ~8.7k LOC, all clean. **Boot → New Game → play the whole arc → Continue later.**

---

## 🌫️🌀 v0.9.2 — *"Where the Ink Runs"* — The Last Two Eras (CODE)

> All four time-travel eras are now playable — and the Spellplague makes *causality itself* a hazard.

- 🧩 **`SimpleEra`** — a reusable, config-driven era scene (grid + floor + camera + exploration UI +
  leader + a point-of-interest + a battle trigger), so new eras are *config, not boilerplate.*
- 🌫️ **Time of Troubles** (era 3, 1358 DR) — witness the **Crown of Horns forged from Myrkul's skull**;
  the Crown-is-Myrkul reveal lands in-engine (sets `act4.crown_is_myrkul`). Fight the avatar-touched.
- 🌀 **Spellplague** (era 4, 1385 DR) + **`SpellplagueHazard`** — a *causality-optional* battlefield:
  each turn after the second may **swap two combatants' positions** (a reality skip) and lash a random
  one with **blue fire** (Force). Your positioning is never quite safe. `EncounterBuilder` gains a
  `spellplagueHazard` toggle.
- 🔗 Both wired as flag-gated hub rifts (ToT after the Crown Wars; Spellplague after ToT). The director
  is now a **10-mode** machine. Plus **`SpellplagueDemo`** for a one-click taste.
- 🟢 74 scripts / ~8.4k LOC, all clean. **All four eras of the saga are playable.**

---

## 🌅 v0.9.1 — *"The Court of the Dead"* — The Six Endings, In Code

> The capstone that makes every flag *mean* something: a finale that reads your whole playthrough.

- 🧠 **`EndingResolver`** — pure logic that reads GameFlags to decide which endings you've **earned the
  right to choose** (deeper truths gate the golden roads: *Jergal's Keyhole* needs the Lady's riddle +
  a spared Verdict; *Break the Loop* needs you to have grasped the loop), supplies each ending's prose,
  and assembles **BG2-style epilogue slides** keyed to who lived, who the Wall took, the Verdict, the
  Lady's witness, and Aldric's fate.
- 🏛️ **`EndingScreen`** — the **Court of the Dead** finale: the convergence framing, the available
  endings as choices (golden ones starred), and on a pick the full prose + epilogue.
- 🔗 Wired in: a **Court of the Dead** exit in the hub (now an **8-mode** director), plus **`EndingDemo`**
  — a one-click tour that unlocks all six to read prose + epilogues side by side.
- 🟢 70 scripts / ~8.0k LOC, all clean. The full arc — create → play → *end* — now exists in code.

---

## ⚰️ v0.9.0 — *"The Wall Keeps What It Takes"* — The Fugue, Maerin & the Breach (CODE)

> The game's first **permanent loss** — the beat that proves this story will take things and never
> give them back.

- 🩶 **`FugueEra` + `FugueContent`** — descend (from the hub, after the Cinderhaunt) into the grey
  **Fugue Plane** and walk the **Wall of the Faithless**: the faces in the mortar, and the niche with
  **your own name**, a little more formed than last time.
- ⚰️ **The Breach** — pull **Maerin** free of the Wall, and the Wall takes its tithe: a companion is
  **removed from the party forever** (`Party.Remove`; no revival exists, by design). *Who* is decided
  by an **authored-fate** system (doc 15): **Varra** by default (her Act I 'leash' pays off), else a
  companion **volunteers** (Ilfaeril → Garrow → Naeve), else the **lowest-approval** member — so the
  grief is *authored by your earlier care*, not merely inflicted.
- 🧑‍🤝‍🧑 **Two more recruitable companions** — **Varra** (a tiefling warlock, in the hub) and **Maerin**
  (pulled from the Wall) — bringing the playable roster to six, all flag-driven.
- 🎬 **`BreachDemo`** — a one-click harness: walk the Fugue, pull Maerin, watch Varra be taken.
- 🧱 `CampaignBootstrap` is now a **7-mode** director (hub ⇄ dungeon ⇄ Netheril ⇄ Crown Wars ⇄ Fugue ⇄
  Vault ⇄ encounter). 67 scripts / ~7.7k LOC, all clean.

---

## 💰 v0.8.3 — *"The Margin Pays Out"* — Vault Rewards (CODE)

> Solving the Lady's riddles now actually *rewards* you — narrative boons, not a +2 sword.

- 🪙 **Coin of the Tenth Guest** (5 riddles solved) — a usable consumable that mends you with a *nudge
  of fate* (3d8 heal). *"Spend it when fate needs a nudge."*
- 🎁 **Her Favour** (10 solved, high amusement) — a stronger single-use mending (6d8). *"Use it the
  moment you'd otherwise lose someone."*
- 📜 **The Reader's Boon** (solve the secret 11th — her name) — a lore trophy + a flag the finale
  checks. *"Proof that, once, the margin smiled."*
- ⚙️ Rewards are granted **once per milestone** (flag-guarded) by `RiddleVaultUI`, dropped straight into
  the party pouch and usable from the inventory screen.

---

## 🧩 v0.8.2 — *"The Vault Made Real"* — The Riddle Room, In Engine (CODE)

> The Lady in the Margins steps off the page: a playable riddle room built on the existing engine.

- 🎭 **`RiddleContent`** — 15 symbolic **answer-tokens** (as quest items), the **10 object riddles**,
  the **brazier (spoken) riddles**, the locked *Your Name*, the secret 11th (*her* name), and **Tally's**
  hub dialogue.
- 🏛️ **`RiddleVault`** — the **Vault of Tens** as an explorable room: ten pedestals (place a token), a
  row of braziers (speak the word), the violet **Margin Brazier** (the twist riddle), and a way out;
  she hands you the pouch of tokens on entry.
- 🖥️ **`RiddleVaultUI`** — solve riddles by **placing a token from the pouch** or **typing the spoken
  answer**; the **wit-beats-correctness** system rewards clever-wrong tokens with *amusement* instead of
  a solve, and failure costs only her interest — **she never kills you, she only gets bored.**
- 🔗 Wired in: **Tally** appears in the Baldur's Gate hub; hearing her out (`vault.requested`) sends the
  director into the Vault (now a **6-mode** director). Plus **`RiddleVaultDemo`** for a one-click test.
- 🧱 A small modal guard (`RiddleVaultUI.AnyOpen`) so clicking the puzzle panel doesn't also move your
  token. 64 scripts / ~7.1k LOC, all clean.

---

## 🎭 v0.8.1 — *"The Note in the Margin"* — The Riddler & the Vault of Tens

> A surreal, chaotic-neutral fifth-dimensional entity who *monitors the situation* — and a riddle
> room better than Bodhi's, with twenty original riddles.

- 🎭 **`22_THE_LADY_IN_THE_MARGINS.md`** — the recurring riddler in the lineage of **G-Man / the
  Outsider / Gaunter O'Dimm / Mr. Door**: chaotic neutral, beyond gender/comprehension, wants only to
  be *entertained.* Her **HUGE late twist** — she is the **ascended form of Naeve**, riddling you from
  the white space at the end of time (a character you've been interacting with all along,
  pre-ascension) — with a full foreshadowing ledger so it's *fair.*
- 🧩 **The Vault of Tens** — a riddle room that keeps BG2's place-the-object joy and adds four things:
  **wit beats correctness** (clever-wrong answers are rewarded), **no death — only boredom**,
  **token combinations / cross-reference**, and **tiered narrative rewards** (a reroll, a truth, a
  witness — never a +2 sword).
- 📜 **20 original D&D riddles** (+ the secret 21st = her name) — answers as **objects to place** or
  **words to speak**, several woven into the game's own themes (the Mirror→the Last Returned, the
  Skull→the Crown of Horns, *Your Name*→the master key, *Tomorrow*→the deathless Now, and *the Wall of
  the Faithless*, "the one she didn't write"). Includes implementation notes mapping the Vault onto the
  existing engine (pedestal Interactables + token items + flags).

---

## 🪞 v0.8.0 — *"Reflections & the First Sin"* — The Crown Wars, the Echoes, Tiled Floors & Aldric's Tea (CODE)

> A second era (proving the module pattern twice), the reflection-fight against yourself, a real
> isometric floor, and the marquee Act I conversation — all playable.

- 🌫️ **Crown Wars era** (`CrownWarsEra` + `CrownWarsContent`) — an elven high court ~10,000 years
  back, reached via a **second "Deep Skip" rift** after Netheril. Recruit **Ilfaeril**, and the
  signature **moral-hazard scene**: *The Verdict*, where you can **argue a soul-damnation down**
  with a skill check (proving the Wall was a *decision*) — shifting flags + Ilfaeril's approval.
- 🪞 **The Echoes / the Mirror fight** (`MirrorResolver`, `EncounterBuilder.mirrorMode`,
  `MirrorEncounterDemo`) — fight twisted **clones of your own party** (new `CharacterSheet.Clone()`)
  plus **the Last Returned**, who *kneels and yields* once its Echoes fall rather than dying by
  damage. The late-game centrepiece, in miniature.
- 🟫 **Tiled isometric floor** (`TileFloorRenderer`) — every scene now paints a checker-tinted
  diamond floor under the units (walls/voids drawn dark). A real visual upgrade, no art needed.
- ☕ **"Tea with a Heretic"** (`AldricContent`) — Aldric's full branching conversation, in-engine;
  he comes to the Baldur's Gate hub once you've cleared the Cinderhaunt.
- 🧱 `CampaignBootstrap` now drives a **5-mode** director (hub ⇄ dungeon ⇄ Netheril ⇄ Crown Wars ⇄
  encounter) and auto-recruits Naeve & Ilfaeril by flag. 60 scripts / ~6.6k LOC, all clean.

---

## 🌆 v0.7.0 — *"The Falling City"* — Netheril, the First Time-Travel Era (CODE)

> The saga's time-travel becomes *playable.* You step through a rift in Baldur's Gate into a
> Netherese enclave falling out of the sky — and fight as the floor caves in beneath you.

- 🕰️ **`NetherilEra`** — an explorable golden-rubble enclave (Karsus's Folly, −339 DR): arrive
  out of time (auto-played fall sequence), find **Naeve** in the rubble, recruit her, and trigger
  the battle. Flag-tracked like the Cinderhaunt so it survives the round-trip to combat.
- 🕳️ **`FallingHazard`** — the era's signature mechanic: after a short grace period, the floor
  **collapses each turn from the edges inward** — voided tiles block movement and deal falling
  damage; it never collapses below a safe core, so the fight stays winnable but frantic.
  `EncounterBuilder` now has an `environmentalHazard` toggle.
- 🧙 **`NetherilContent`** — arcane abilities (Arcane Bolt, Lightning Lance, **Shatterglass** AoE,
  construct slams), the **Netherese War-Construct** roster, **Naeve** (Wizard companion), and the
  era arrival + recruit dialogue.
- 🌀 **Rift transition** — after clearing the Cinderhaunt, a **Skip rift** appears in the hub;
  `CampaignBootstrap` now drives a 4-mode director (hub ⇄ dungeon ⇄ era ⇄ encounter) and
  auto-recruits Naeve via dialogue flag, into the roster and into the falling-city fight.
- 🌆 **`NetherilDemo`** — one-click jump straight into the collapsing-floor battle.
- 📈 54 scripts / ~5.8k LOC, all brace-balanced and namespace-clean.

---

## 🐉 v0.6.6 — *"Monsters, Money & Act One"* — Bestiary, Pitch & the Full Act I Script

> The combat-as-theme bestiary, the document that makes it fundable, and Act I scene by scene.

- 🐉 **`19_BESTIARY_AND_BOSSES.md`** — enemies & bosses mapped to our combat engine: stat blocks,
  the **Lay vs. Shatter** mercy mechanic, the **Echoes** (mirror fights vs. your own party), and the
  **anti-bosses** (the Unmade and the Last Returned — beaten by *not* fighting). Every enemy is *a
  question with a health bar.*
- 🚀 **`20_PITCH_AND_VERTICAL_SLICE.md`** — the Kickstarter-grade **pitch one-pager** (logline, 5 USPs,
  comparables) + the **one-hour vertical-slice spec** that proves the whole game + honest **scope,
  budget tiers, and risk**.
- 🎬 **`21_ACT_ONE_SCRIPT.md`** — the **full Act I script**, Prologue + 3 chapters, scene by scene
  (purpose · beats · branches · flags), with an Act-I reactivity ledger feeding the later payoffs.

---

## 💞 v0.6.5 — *"Hearts & Endings"* — Romances, Ending Prose & the Art/Music Bible

> The game gets a heart, a curtain call, and a face & a voice.

- 💞 **`16_ROMANCE_SCRIPTS.md`** — full **romance arcs** for Garrow, Roen, Naeve, and Varra — six
  stages each (Spark → Trust → Turn → Crisis → Choosing → The Last Night), with key-beat dialogue,
  approval gating, and how each romance becomes the finale's `anchor`. Each romance is an *argument
  about a theme* (faith / belonging / guilt / worth).
- 🌄 **`17_ENDINGS_PROSE.md`** — all **six endings written as full prose epilogues** (+ the hidden
  "Husk" failure-state): the final scene, the per-companion slides, and the last image, each landing
  the cosmos on a single human face.
- 🎨🎼 **`18_ART_AND_MUSIC_BIBLE.md`** — the **color language** (*warmth = mortality*), per-era and
  per-character visual identity (silhouette-first), UI/diegetic design, and a **leitmotif-driven
  score** built on one unfinished six-note phrase (*The Niche*) that finally resolves on your true
  name in the finale.

---

## 🧬 v0.6.4 — *"Engineering the Gasp"* — Twist Architecture & Two Devastating Scenes

> The craft principles, baked into the structure — plus the two scenes designed to make players
> say *"HOLY SHIT."*

- 🧬 **`13_TWIST_ARCHITECTURE.md`** — the redesign: a **Foreshadowing Ledger** (every clue → its
  payoff, to pass the replay test), the **early permanent loss** design, the **weaponized-kindness**
  placement, **Mirror** visual staging, "**land the cosmos on one face**," the once-only
  **meta-crossing**, and a revised beat sheet sequencing all of it.
- 🪤 **`14_SCENE_WEAPONIZED_KINDNESS.md`** — full scene: the reveal that the player's *compassion*
  (pulling Maerin from the Wall) is the loop's engine — *mercy as the trap.* Branching, shown-not-told.
- 🕳️ **`15_SCENE_THE_BREACH.md`** — full scene: the game's first **irreversible loss**, with an
  **authored-fate system** (who the Wall takes depends on the player's earlier care). Teaches the
  player this story will take things and keep them.

---

## 🎬 v0.6.3 — *"Scenes & Craft"* — Dialogue Scripts, Side Quests & Emotional Design

> The set-piece conversations, the living city, and a craft study of the genre's greatest twists.

- 💬 **`10_DIALOGUE_SCRIPTS.md`** — full branching dialogue trees (node format, with checks/flags/
  approval) for the three marquee scenes: **"Tea with a Heretic"** (Aldric), **"The Offer"** (the
  Unmade at the Wall), and **"The Mirror"** (the Last Returned).
- 🗺️ **`11_SIDEQUESTS_AND_NPCS.md`** — themed Baldur's Gate **side quests** (each a small echo of the
  central question), per-era side content, a roster of **memorable NPCs**, and **reactive world state**.
- 💥 **`12_EMOTIONAL_DESIGN.md`** — a craft study of the **20 most-remembered RPG moments** (Aerith,
  Revan, Planescape, Mordin, the Bloody Baron, Undertale, NieR, Spec Ops…), the anatomy of a
  "HOLY SHIT" twist, and an **honest audit** of how *The Crown of Horns* reaches that tier.

---

## 🖋️ v0.6.2 — *"Flesh and Bone"* — The Story, Fully Fleshed Out

> Everything deepened: full companion arcs, villain origin novellas, the esoteric texts, and
> prose treatments of the key scenes. ~12k more words of canon.

- 🎭 **`06_COMPANIONS_DEEP.md`** — complete dossiers for all seven: full backstories, three-chapter
  **personal quests**, **romance arcs** beat-by-beat, cross-companion **banter**, approval triggers,
  and every fate. (Garrow, Roen, Varra, Naeve, Ilfaeril, Maerin, the Mournlight.)
- 👁️ **`07_VILLAINS_DEEP.md`** — **origin novellas**: Aldric's forty years of graves; Myrkul's long
  patience; **the Unmade told from its own point of view**; the Last Returned's burned timeline;
  Vayle's terrible honesty; Jergal's boredom.
- 📜 **`08_CODEX.md`** — the **esoteric texts in full** (*The Ledger of the Unwalled*, *Karsus's Last
  Theorem*, *The Ilfaeril Confession*, *The Doomsong of Jergal*, *The Niche*), factions, the
  time-travel **timeline**, the **bestiary**, and the **places**.
- ✍️ **`09_PROSE.md`** — novella-grade **prose treatments**: the Death, the Wall, the **Deathless
  Garden**, the **Mirror** (the duel with your future self), and the **Name** (the finale).
- 🔗 Story library index updated.

---

## 📚 v0.6.1 — *"The Deep Water"* — The Complete Story Bible

> A full, novel-grade narrative library — the story fleshed out to BG2 / Planescape depth.

- 📖 New **`docs/story/`** library (6 documents, ~30k words of canon):
  - `00_OVERVIEW` — tone, the central question, themes, and the **tropes we play & subvert**.
  - `01_SYNOPSIS` — the **entire game**, beginning to end, every twist in order.
  - `02_CHARACTERS` — deep backstories for the Returned, the **four villain masks**, Vayle, Jergal,
    and all **seven companions** (wound · secret · want vs. need · arc · personal quest · romance · fates).
  - `03_ACTS` — Prologue + five acts: settings, quests, set-pieces, the major development & twist of each.
  - `04_ENDINGS` — all **six endings** (+ a hidden failure-state) with flag requirements and per-companion
    epilogue slides.
  - `05_LORE_AND_PHILOSOPHY` — the cosmology, **esoteric in-world texts** (with quotes), the philosophical
    debates each faction embodies, and the recurring symbols.
- 🔗 `STORY_BIBLE.md` and `README.md` now point into the deep library.

---

## 🕯️ v0.6.0 — *"The Cinderhaunt"* — A Real Dungeon, Recruitment & the Time-Spanning Saga

> The world loop deepens into an actual dungeon crawl with a recruitable companion — and
> the story explodes into a time-travel epic across Realms history.

### 🏰 The Cinderhaunt Dungeon *(new)*
- 🗺️ `Cinderhaunt`: a **two-chamber dungeon** beneath Baldur's Gate — outer harvest-hall and
  inner sanctum, split by an **iron door** that needs the **Cinderhaunt Key** (found in the
  hub strongbox).
- ⚔️ A **guard fight** and a **boss** (the Unbound Maw), triggered from the world; your whole
  **active party** (hero + Garrow + recruits) deploys into each battle.
- 🧠 Progress is **flag-tracked** — cleared fights, the opened door, and looted chests all
  persist across the explore↔combat round-trips (new `lootFlag` on containers).
- 🧱 `CampaignBootstrap` now drives a 3-mode director: **hub ⇄ dungeon ⇄ encounter**, with a
  forgiving "recover at the door" on a wipe.

### 🧑‍🤝‍🧑 Companion Recruitment *(new)*
- 🪶 **Roen Alleywind**, a Harper rogue, is recruitable in the hub via a branching conversation;
  a dialogue **flag** adds him to the roster and into combat. The director watches
  `companion.<id>.recruited` and recruits automatically — fully data-driven.

### 📖 The Story, Reborn *(major)*
- 🌌 **`STORY_BIBLE.md` rewritten as a time-spanning, existential epic** (BG2 / Planescape
  scale): the **Unmade** (the accreted nothing of every dissolved Faithless soul), **time
  travel** through **Netheril (Karsus's Folly)**, the **Crown Wars**, the **Time of Troubles**,
  and the **Spellplague**; four nested villains ending in **the Last Returned** — your own
  future self; **7 companions across time**; **5 acts**; **6 endings**; and the twist that
  *you are the Unmade's origin.*

---

## 🎒 v0.5.0 — *"Spoils & Spellfire"* — Loot, Equipment & the VFX Pipeline

> Gear up and light up: a full items/loot/equipment layer plus a combat **VFX system**
> that plays the supplied pixel-effect pack on hits and spells.

### 🎒 Items, Loot & Equipment *(new)*
- 🗃️ `ItemDatabase` (id → item registry) + content items (swords, armor, **Potion of
  Healing**, the Cinderhaunt Key).
- 🛡️ `EquipmentSystem` / `Equipment`: equip weapons & armor — gear drives **AC** and the
  basic attack; consumables apply effects on use.
- 🎒 `InventoryScreen` (press **I**): backpack, gold, per-member equipped slots, equip/unequip/use.
- 💰 **Loot drops**: enemies drop gold + items, auto-awarded to the party on victory.
- 📦 **Lootable containers**: a hub strongbox transfers its contents to the party stash.

### 💥 VFX & Asset Pipeline *(new)*
- ✨ `FxSystem` + `FxAnimator`: fire-and-forget sprite-frame effects played on hit/cast,
  **auto-loaded** from `Resources/FX/<effect>/` (no inspector wiring; no-ops if absent).
- 🔥 Abilities gained `hitVfx` / `castVfx`; impacts **auto-pick by damage type**
  (fire/ice/holy/…). Healing plays a `heal` effect.
- 📖 New **`docs/ASSET_INTEGRATION.md`**: exactly where to drop the pixel FX pack (and
  future character sprites, portraits, tilemaps) so they plug straight in.

---

## 🧭 v0.4.0 — *"The Gilded Gate"* — Exploration, the Hub & the World Loop

> The disconnected systems become an actual game: **explore a town, take a quest, walk
> into a dungeon, fight, and return** — the core Baldur's-Gate loop.

### 🧭 Exploration Mode *(new)*
- 🗺️ `ExplorationController`: real-time **click-to-move** the party leader (A* pathing),
  plus **E**/click to interact; movement locks during conversations.
- 🧩 `Interactable`: **NPCs** (talk), **exits/doors** (transition), **points of interest**
  (examine) — snaps to the grid and routes the party beside it.
- 🖥️ `ExplorationHUD`: party vitals, an **interaction prompt**, examine flavor, and a
  **quest journal** whose objectives tick live from GameFlags.

### 🏰 Baldur's Gate Hub *(new)*
- 🏙️ `BaldursGateHub`: a walkable Lower City slice — the **Herald** (quest giver, branching
  convo), a **proclamation board** to examine, and the **Cinderhaunt stairs** to descend.

### 🔁 Mode Director & Reusable Combat *(new)*
- 🎬 `CampaignBootstrap`: the flagship entry — **create → explore → quest → dungeon fight →
  XP/level → return to the hub**, with persistent managers across modes.
- 🧱 `EncounterBuilder`: drop-in turn-based encounter from a roster + victory callback, so
  **any door or trigger** can start a fight. (Combat HUD now tears down cleanly per mode.)

---

## 🏛️ v0.3.0 — *"What the Dead Owe"* — Setting, Creation, Leveling & the Playable Prologue

> The project moves to the **Forgotten Realms / Sword Coast** and gains a full
> create-a-hero → dialogue → battle → level-up loop with a real uGUI HUD.

### 🌍 Setting & Story
- 🗺️ **Story Bible rewritten for the Forgotten Realms** (`docs/STORY_BIBLE.md`): the
  **Wall of the Faithless**, villain **Aldric Morn** & the **Crown of Horns** (Myrkul),
  **Sister Vayle**, **Jergal**, 7 FR companions, factions (Harpers, Flaming Fist, Order of
  the Gauntlet, Zhentarim, church of Kelemvor), 3 acts, and **five branching endings**.

### 🧙 Character Creation *(new)*
- 🧬 `BackgroundDefinition` assets (skills, feature, **flag that unlocks dialogue**).
- 🛠️ `CharacterBuilder`: **27-point point-buy** + **standard array**, racial bonuses,
  HP/AC assembly, background kit & flags, the **Returned** trait.
- 🖥️ `CharacterCreationScreen`: race/class/background pickers + point-buy stepper.

### 📈 Leveling & XP *(new)*
- ⭐ `Progression`: full **5e XP table** (1→20), HP & ability grants on level-up,
  proficiency scaling, `OnLevelUp` event.
- 🏆 **XP awarded on victory** — pooled from defeated foes, split among survivors
  (`EncounterController`), can trigger level-ups mid-run.

### 🖥️ Real uGUI HUD *(new)*
- 🎛️ `CombatHUD`: runtime-built canvas — **party portraits + live HP bars**, initiative,
  active-unit vitals/economy, a **clickable ability bar**, combat log, End Turn. Zero setup.
- 🗨️ `DialogueScreen`: speaker/body/choices with live **skill-check** results; auto-play
  & on-finished hooks.

### 🏰 Act I Content Scaffold *(new)*
- 📦 `SwordCoastContent`: builds FR **races, classes, backgrounds, spells/weapons, status
  effects**, the **opening dialogue** with Aldric's herald, and the **first quest** in code.
- 🎬 `PrologueBootstrap`: the flagship demo — **creation → dialogue → turn-based battle →
  XP/level → quest completion**, fully self-contained.

### 📚 Docs
- ✨ New **`FEATURES.md`** (implemented + planned codex with status icons & badges).
- ✨ **README** redesigned with badges, feature grid, and quick-start tables.

---

## ⚔️ v0.2.0 — Combat Depth Layer

> The systems that make combat *feel* like D&D 5e.

- ✨ **Spell slots / resources** spent by leveled spells, refreshed at combat start.
- ☠️ **Status effects / conditions** (`StatusEffect.cs`, `Condition.cs`): duration, DoT,
  incapacitation, advantage grants, flat AC/attack/speed modifiers.
- ⚖️ **Advantage / disadvantage** in `AttackResolver` (cancel per 5e).
- 🎯 **Saving-throw spells with save-for-half** and **healing abilities**.
- 🧩 **`AbilityRunner`**: one validated entry point — range, slots, action economy, **AoE**,
  condition application, logging. Player input & enemy AI both use it.
- 🛡️ **Equipment hooks** on `CharacterSheet`.
- 🔥 **Demo upgrade**: wizard (Firebolt + **Fireball**), cleric (**Healing Word**), and a
  poison-clawing enemy. HUD shows conditions, slots, and a numbered ability bar.

---

## 🧱 v0.1.0 — Initial Starter

> The foundation.

- 🗺️ Grid + isometric projection, **A\* pathfinding**, isometric camera.
- 🎲 Turn-based combat: initiative + action economy + win/loss.
- 🧙 5e stats: abilities, modifiers, proficiency, AC, HP, d20 attacks vs AC.
- 💬 Data-driven **dialogue / quests / items** via ScriptableObjects.
- 🧠 **GameFlags** story brain; 💾 JSON **save/load**.
- 🕹️ One-click **playable demo** + the full **story bible**.
