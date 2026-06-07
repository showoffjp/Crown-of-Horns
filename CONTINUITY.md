# 🧭 CONTINUITY & HANDOFF — *The Crown of Horns*

*A living brief so any new Claude Code session picks up exactly where we left off — same vision,
same conventions, same momentum. Read this first.*

---

## 🎯 What this project is

A **classic isometric, party-based CRPG** in the lineage of Baldur's Gate II / Icewind Dale /
Planescape: Torment — built in **Unity (C#)** on **D&D 5e (SRD)** rules, **turn-based** combat,
set in the **Forgotten Realms / Sword Coast**. Working title: **The Crown of Horns**.

The pitch: *You died in the harvest beneath Baldur's Gate and were pulled back — **Returned**. The
crack in your soul is a crack in **time**, and the saga walks the hinge-points of Realms history
(Netheril's fall, the Crown Wars, the Time of Troubles, the Spellplague) chasing four nested villains
behind a single question: **what is a soul worth, that no god ever claimed?***

**North star:** an EPIC, existential, emotionally devastating story that rivals BG2 — paired with a
real, runnable engine. The user loves **both** the code AND the lore expansion. Lean into both.

---

## 📍 Where we are (as of this handoff)

- **Version:** v3.60.0 · **135 C# scripts** (~19.7k LOC) · all brace-balanced & namespace-clean.
- **Engine:** tactical 5e combat (initiative, action economy, d20 vs AC, saves, AoE, conditions,
  advantage/disadvantage, spell slots, enemy AI), isometric grid + A*, exploration/hub mode,
  data-driven dialogue, quests, inventory, save/load, character creation, leveling.
- **Campaign:** a full director (`CampaignBootstrap`) swaps "modes" — hub → Cinderhaunt dungeon →
  tea with Aldric → four time-travel eras → the Breach (permanent loss) → the Vault of Tens (riddles)
  → the Court of the Dead (six endings).
- **Companion pillar (the recent focus):** a complete BG2-style affinity loop —
  **be kind in dialogue → approval rises (visible in the Party screen `P`) → field them → camp
  night-talk (`CampScene`) → personal-quest hook appears in the hub → playable quest with a real
  moral choice → epilogue payoff in the ending.**
- **All five companion personal quests are playable** — Roen's *"The Honest Lie,"* Varra's *"The Bill
  Comes Due,"* Garrow's *"A God-Shaped Hole,"* Naeve's *"After the Sky Fell,"* and Ilfaeril's *"The
  Vote"* — all built on the reusable, data-driven `PersonalQuestScene` + `PersonalQuest` config. **The
  companion-quest pillar is complete.**
- **All four romance arcs are playable** (`RomanceContent`) — Garrow/faith, Roen/belonging, Naeve/guilt,
  Varra/worth — on a shared six-stage spine (Spark → Trust → Turn → Crisis → Choosing → Last Night),
  approval+progression-gated, played at the campfire's **"Grow Closer,"** epilogue-aware (including the
  golden-ending beats). **The full companion loop runs: recruit → banter → approval → camp → personal
  quest → romance → epilogue.**
- **UI:** Journey/quest tracker (`J`, now with per-quest beats + a romance Bonds section), Party/roster +
  companion sheets (`P`), Codex/bestiary (`K`), ambient party banter (`B` to mute), **Options** (`O`), **Help** (`H`), **Chronicle** (`C`), typewriter dialogue, difficulty modes, autosave-on-rest, main menu with New Game / Continue.
- **Act II is becoming a *place*** — `ActTwoContent` adds a reactive Lower City layer to the hub: Tamsin
  the crier (reads your run off the flags via conditional choices) and **four** side quests (Widow, Fist, Faithless Choir — which seeds the Unmade villain faction — and the Tithe Collector),
  all moving `reputation.lowcity`/approval and epilogue-aware. Plus a reactive Fugue beat at the Wall (v1.25.0). Still open: multi-room areas, deeper Act III.
- **18 one-click demos** for every system (see `FEATURES.md`).
- **Since v1.60 (latest big batch — see `CHANGELOG.md`):**
  - **Tactical combat layer:** Defend (G) / Dash (F) / Help (T) / Disengage (X) / Shove (V) / Quaff (Q) +
    **opportunity attacks**, **flanking**, **half-cover**, round tracking, AoE-aware + focus-fire enemy AI,
    wounded-enemy Dodge, **per-level class kits**, party-wipe **DefeatScreen**, mid-combat revive, a seeded
    **balance canary** (`CombatBalance`), camera auto-focus, BLOODIED/LEVEL-UP/FLANKED/COVER/OPPORTUNITY pops.
  - **World breadth:** four explorable Lower City rooms (Almshouse, Shrunken Quarter market, Chionthar Docks,
    Roen-gated Harper safehouse), **two vendors** (fence + smuggler) with **buy & sell**, faction standings in
    the Journey, and Lady/Breach/era-reactive examinables in every room + the hub (balladeer, onlookers).
  - **Accessibility/meta:** UI-scale, combat-speed, floating-text & camera-follow toggles; cross-run
    **endings memory** + **New-Game+** (Naeve/Roen déjà-vu beats); first-combat tutorial; dialogue hotkeys.
  - **Content:** **65 Codex entries**, **28 deeds**, **29 fireside banters** (incl. grief beats + reactions to the
    moral beats), **13 night-talks**, **11 keepsakes**, **4 branching moral micro-quests** (one per Lower City
    room — ferryman / urchin / deathbed / informant, each a full loop: choice → conscience → Journey → epilogue
    → a companion's fireside reaction), post-quest & romance reflections.
  - **Deferred (need a Unity editor / not worth risking the clean build headless):** spell **concentration**,
    proper **death saves**, full scripted-encounter harness, art drop, uGUI HUD skin. See `docs/ROADMAP.md`.

---

## 🧪 Testing & project infrastructure (NEW — the repo is now buildable & tested)

The long-standing "no Unity compiler in the sandbox" gap is closed at the repo level: this is
now a **real, buildable, testable Unity project**, even though the authoring sandbox still can't
compile it (CI / a local editor does).

- **Unity project scaffold** (was missing, though `.gitignore` always expected it): `Packages/manifest.json`
  (uGUI + Test Framework + Unity 6 modules), `ProjectSettings/ProjectVersion.txt` (**set this to your exact
  editor version** — currently `6000.0.32f1`), and `Assets/Scripts/SunderedCrown.asmdef` (one named game assembly).
- **24 automated test suites · 161 tests** under `Assets/Tests/` — **22 EditMode** (pure logic: dice/ability
  math, GameFlags, AttackResolver incl. crit/save-for-half/determinism, CombatBalance, status effects,
  creation/progression, items/inventory/equipment, A* pathfinding, party, quests, a real on-disk save/load
  round-trip, the DialogueRunner engine, difficulty, and the meta-layer — deeds/keepsakes/codex/**endings**)
  and **2 PlayMode** (`TurnManager` initiative/rounds/economy, `GridUnit` placement/movement). `ContentValidator`
  is promoted to a CI gate. **When you add new *pure* logic, add a suite** — mirror an existing one.
- **CI** (`.github/workflows/ci.yml`, see `docs/CI.md`): a `repo-hygiene` job (no secrets, **runs today**) =
  asset-manifest drift + Git-LFS rules + the `tools/check-cs-structure.sh` brace/namespace check; and a
  `unity-tests` job (game-ci, EditMode+PlayMode) that **self-skips until a `UNITY_LICENSE` secret is added**.
- **Asset pipeline:** structured `Assets/Art|Audio|...` tree + `Assets/ASSETS.md` (naming + Git-LFS workflow,
  to ingest a big art library past the 25/30 MB upload limits) + `tools/generate-asset-manifest.sh`.
- **North-star docs:** `DESIGN.md` (vision + five pillars) and `ROADMAP.md` (prioritized backlog).

## 🧱 Architecture in one breath

> **Code is the engine. ScriptableObject assets / content classes are the game.**

- **`GameFlags`** is the single story brain: `bool` + `int` dictionaries (flags, approval, reputation).
  Everything reactive reads/writes flags. Approval is `companion.<id>.approval` (−100..100).
- **Director pattern:** `CampaignBootstrap` swaps "modes" under a disposable `_modeRoot` GameObject
  (hub / dungeon / eras / vault / camp / quest / encounter / finale).
- **Reusable builders:** `EncounterBuilder` (combat scenes), `SimpleEra` (config-driven era
  exploration), `PersonalQuestScene` (companion quests), `Interactable`/`ExplorationController` (hubs).
- **UIs are OnGUI/IMGUI** MonoBehaviours added at runtime — zero scene setup, drop-in components.
- **Content lives in plain classes** (e.g. `RoenQuestContent`, `CampContent`, `CodexContent`) that
  build `DialogueGraph`s and config objects — so new content is *data, not boilerplate*.

---

## 🤝 Standing working agreements (KEEP DOING THESE)

1. **Update the MD files on EVERY functional change** — `README.md`, `FEATURES.md`, `CHANGELOG.md`,
   and the story docs / `STORY_BIBLE` when lore changes. This is a hard standing instruction from the
   user. Bump the script-count badges, add a CHANGELOG entry with a codename + rationale, add FEATURES
   rows with ✅ status icons. Make them look amazing (icons, tables, emoji).
2. **Validate before committing:** the sandbox has no Unity compiler, so check **brace balance**
   and **namespace/`using` correctness** on every changed file. This is now scripted —
   run **`bash tools/check-cs-structure.sh`** (also enforced in CI). When you add pure logic,
   add an EditMode test suite under `Assets/Tests/EditMode/` mirroring an existing one.
3. **Commit style:** clear, descriptive messages with a version codename (e.g.
   *"v1.6.0: Garrow's personal quest 'A God-Shaped Hole'"*). Commit signing is off in the sandbox.
4. **Every turn should deliver real value** and end with a short, concrete set of "what's next"
   options — the user often replies "continue / do all of it / you pick," and trusts the agent to
   choose well. Lean ambitious.
5. **Voice:** the user adores a "real-ass DM." Be epic, warm, literate; write companion dialogue and
   lore with genuine craft and emotional stakes. Foreshadow twists fairly.

---

## 🗺️ Roadmap — what's queued next (in priority order)

1. ✅ **Garrow's personal quest — "A God-Shaped Hole"** (heresy trial at Kelemvor's temple). **Done in
   v1.6.0** — `GarrowQuestContent`, the Justiciar's tribunal fight, the recant/leave/[Religion] trilemma,
   an `EndingResolver` epilogue slide, and a `GarrowQuestDemo`.
2. ✅ **Naeve's personal quest — "After the Sky Fell"** (a surviving Netherese fragment calls her home).
   **Done in v1.7.0** — `NaeveQuestContent`, the last-protocol fight, the preserve/release/[Arcana]
   trilemma, an `EndingResolver` epilogue slide, and a `NaeveQuestDemo`.
3. ✅ **Ilfaeril's quest — "The Vote"** (the reliquary of the soul he helped damn). **Done in v1.8.0** —
   `IlfaerilQuestContent`, the Choir-of-the-Unmade fight, the penance/forgiven/[Insight] trilemma, an
   `EndingResolver` epilogue slide, and an `IlfaerilQuestDemo`. **All five companion quests now playable.**
4. ✅ **Romance arcs** — **Done in v1.9.0.** `RomanceContent` (six-stage spine), `CampScene` "Grow Closer"
   integration, `CommittedElsewhere` mutual-exclusivity, `EndingResolver` romance slides, and a `RomanceDemo`.
5. **Act II proper** — flesh out the hub-to-eras connective tissue, side quests, and NPCs. The biggest
   remaining narrative frontier.
6. ✅ **Companion betrayals / rupture arcs** — **Done in v1.11.0** (`RuptureContent`, `CampScene` "A Bond
   Frays", `HasStanding` amends gate, `companion.<id>.left` + `EndingResolver` walked-away slide, `RuptureDemo`).
7. 🚧 **Act II proper** — **started in v1.12.0** (`ActTwoContent`: reactive crier + two side quests);
   **v1.16.0** tied `reputation.lowcity` into the ending (the `lowcity.allies` pledge + epilogue slides).
   Still open: multi-room Lower City areas, Fugue-side story beats, more side quests.
8. ✅ **Romance ↔ Breach stakes** — **Done in v1.13.0**: `ChooseBreachVictim` is drawn to a committed
   romance; `EndingResolver.AnchorId()` + anchor/lost-love epilogue beats. The cruelest playthrough lands.
9. ✅ **Settings / options screen** + **quest-log beat tracking** — **Done in v1.10.0** (`GameSettings`,
   `SettingsScreen`, typewriter `DialogueScreen`, Journey beats + Bonds).
10. ✅ **Combat-time approval nudges** — **Done in v1.19.0** (`EncounterController`: +1 on victory, +3 if
   pulled from the brink; also stabilizes downed companions so only the Breach is permanent). Plus extras
   shipped along the way: **difficulty modes** (v1.14.0), **Help (H)** & **autosave-on-rest** (v1.15.0),
   the **Chronicle (C)** run-summary (v1.17.0).

**Roadmap status: every queued item is complete.** Open frontiers from here are deeper Act II (multi-room
areas, Fugue-side beats, more side quests), Act III/IV content, and an art/sprite hook to replace the cubes.

The two big content engines (`PersonalQuestScene`, `RomanceContent`) are both done and reusable. The
natural next frontier is **Act II** proper and the **betrayal/rupture** arcs (story scripts in `docs/story/`).

---

## 🧩 How to add a companion personal quest (the pattern)

1. Write `Assets/Scripts/Content/<Name>QuestContent.cs` — three `DialogueGraph`s (arrival / reveal /
   resolution) + a `PersonalQuest` config (flags: `<id>.quest.arrived/cleared`, `quest.<id>.resolved`).
   Give the resolution a **moral trilemma**, including one **skill-check** branch with a graceful fail.
2. In `CampaignBootstrap`: add the content field, construct it, add it to `QuestById(id)`, and add a
   `Build<Name>Encounter()` (dispatched in `StartPersonalFight`).
3. In `CompanionQuests.cs`: set `playable = true` and a `followLabel` on that companion's hook.
4. Add an epilogue slide in `EndingResolver.Epilogue()` keyed to the resolution flags.
5. Add a one-click `<Name>QuestDemo` for testing.
6. Update README / FEATURES / CHANGELOG. Validate braces. Commit.

> *Worked example: Garrow's "A God-Shaped Hole" (v1.6.0) follows every step above — copy its shape.*

---

## 🔑 Repo & access notes

- This project is its **own standalone repo** — it was never part of the `Vine` (Next.js) repo.
- It now lives at **github.com/showoffjp/Isometric-CRPG** (pushed to `main`).
- Work directly in this repo from a Claude Code session scoped to it (full read/write). No more zips.
- The earlier "Access denied" friction was **session scoping** (a session is bound to one repo at
  launch), not a permissions problem — solved by starting a session against this repo.

---

*Roll for initiative. 🎲 — handoff written so the story never loses the thread.*
