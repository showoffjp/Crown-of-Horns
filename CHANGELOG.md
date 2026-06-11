<div align="center">

# 📝 CHANGELOG — The Crown of Horns

![Keep a Changelog](https://img.shields.io/badge/format-Keep_a_Changelog-orange)
![SemVer](https://img.shields.io/badge/versioning-SemVer-blue)

*All notable changes to the project. Newest first.*

</div>

---

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
