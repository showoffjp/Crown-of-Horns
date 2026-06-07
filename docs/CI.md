# Continuous Integration

CI lives in `.github/workflows/ci.yml` and implements **Tier 0** of `ROADMAP.md` — the
safety net that protects the ~19.4k LOC already working.

## What runs

### 1. `repo-hygiene` — always on, no setup
Runs on every push and PR, no secrets required:

- **Asset manifest freshness** — regenerates `Assets/asset-manifest.csv` via
  `tools/generate-asset-manifest.sh` and fails if it differs from the committed copy, so
  the catalog never silently drifts from the real asset library.
- **Git LFS hygiene** — asserts every core binary extension (`png`, `psd`, `fbx`, `wav`,
  `ogg`, …) is routed through Git LFS in `.gitattributes`, so a stray large binary can't
  sneak into git history and blow the repo up.
- **C# structural check** (`tools/check-cs-structure.sh`) — the project's long-standing
  no-compiler smoke test: every `.cs` under `Assets/` must be brace-balanced with exactly
  one `namespace`. Catches truncated files, bad merges, and copy-paste errors on every push
  without needing Unity. (Parens are deliberately not counted — they appear unbalanced
  inside string literals like `"(adv)"`.)

### 2. `unity-tests` — opt-in, needs a license
Compiles the project and runs **both** the EditMode suites under `Assets/Tests/EditMode/`
and the PlayMode suites under `Assets/Tests/PlayMode/` (`testMode: all`) via
[game-ci](https://game.ci/). It **skips cleanly** until you add a Unity license, so CI is
never red just because the license isn't wired yet.

The repo now carries the project metadata that makes this possible:
- `Packages/manifest.json` — declares `com.unity.ugui` (the only non-module dependency the
  code uses) + `com.unity.test-framework`, plus the standard Unity 6 module set.
- `ProjectSettings/ProjectVersion.txt` — pins the editor version game-ci pulls.
  **⚠️ Set this to your exact Unity version** (currently `6000.0.32f1`); game-ci downloads
  the matching editor image from it.
- `Assets/Scripts/SunderedCrown.asmdef` — puts the game code in one named assembly so the
  test assembly can reference it.

> Unity regenerates the rest of `ProjectSettings/` and all `.meta` files on first open, so
> they're intentionally not committed yet. Open the project once in your editor and commit
> the generated `.meta` + settings to make builds fully reproducible.

## Enabling Unity tests

1. **Get a license file.** Follow game-ci's activation guide:
   <https://game.ci/docs/github/activation>. For a free Personal license you request a
   `.alf`, upload it to Unity, and download a `.ulf`.
2. **Add the secret.** In the GitHub repo: *Settings → Secrets and variables → Actions →
   New repository secret*, named `UNITY_LICENSE`, with the **contents of the `.ulf`** file.
3. Push. The `unity-tests` job now activates and runs any EditMode tests under
   `Assets/` (Unity Test Framework, NUnit).

## The tests so far

Living in `Assets/Tests/EditMode/` (Unity Test Framework, NUnit), covering the pure,
high-value logic first:

- **`DiceTests`** — bounds, seeded determinism (so saves/replays reproduce), advantage vs
  disadvantage, and the `NdM±K` notation parser. *DESIGN pillar IV — trustworthy 5e.*
- **`GameFlagsTests`** — defaults, accumulation, approval clamping, change events,
  null-safe `Replace`. *Pillar III — the one brain every system reads.*
- **`ContentValidatorTests`** — promotes the runtime `ContentValidator` to a CI gate: a
  broken `nextNodeId` in any authored dialogue graph now fails the build. *Pillars I & III.*
- **`AttackResolverTests`** — the 5e contract: nat-20 always hits & crits, nat-1 always
  misses, crits double the dice, save-for-half halves, healing is positive, and identical
  seeds reproduce identical results. *Pillar IV.*
- **`CombatBalanceTests`** — runs the full duel loop end-to-end; asserts the pipeline is
  exception-free, deterministic, and that a favoured matchup wins >50% but <100%. *Pillar IV.*
- **`ItemDatabaseTests`** — registry round-trip, null/empty-id safety, overwrite, clear.
  Keeps loot/shops/equipment resolving honestly. *Pillar III.*
- **`ProgressionTests`** — 5e XP thresholds, single/multi-level jumps, the level-20 cap,
  no-op guards, the OnLevelUp event. *Save-safe progression.*
- **`StatusEffectTests`** — apply sets condition, DoT bites at start of turn, durations
  expire, re-apply refreshes (no stack), flags drive queries & AC. *Pillars III & IV.*
- **`PathfindingTests`** — A* shortest paths, wall detours, unreachability, occupant
  routing, the attack-the-occupied-goal exception, difficult-terrain cost. *Movement spine.*
- **`CharacterBuilderTests`** — point-buy accounting, standard array, illegal-build
  rejection, level-1 sheet assembly. *Creation can't mint illegal heroes.*
- **`SaveSystemTests`** — a real JSON round-trip of GameFlags through disk, plus
  Exists/Delete/Peek. *Pillar II — the game remembers.*
- **`QuestManagerTests`** — explicit start, status tracking, export-is-a-copy, import
  replace/clear (the save hooks the journal & persistence depend on). *Pillar III.*
- **`InventoryTests`** — stackable merge vs distinct stacks, removal accounting, gold
  floored at zero, change events. *Loot economy integrity.*
- **`EquipmentTests`** — armor raises AC, main-hand weapon becomes the basic attack,
  weapon-swap replacement, unequip, slotless/null safety. *Gear → derived stats.*
- **`PartyTests`** — recruit into roster + capped field party, permanent removal (the
  Breach), bench/field toggling, wipe detection. *Roster integrity.*

### PlayMode candidates (need scene/MonoBehaviour lifecycle)
These wire up in `Start()` / instantiate `GridUnit`s / spawn FX, so they belong in a
PlayMode suite (`Assets/Tests/PlayMode/`) rather than EditMode:
- `Combat/TurnManager.cs` — initiative ordering, round advance, incapacitation skip, win/loss.
- `Quests/QuestManager.cs` reactive path — objectives auto-completing as flags flip.
- `Grid/GridUnit.cs` movement — placement, occupancy, depth sorting.

### More EditMode coverage
- **`AbilityScoresTests`** — the 5e modifier curve pinned exactly (incl. the negative-side
  floor C# would otherwise round toward zero), Get/Set independence. *Underpins every roll.*
- **`EndingResolverTests`** — earned-ending gating from synthetic flags (Doomguide via
  Kelemvor rep or naming the monster; the two golden roads' prerequisites), IsGolden, and
  that every ending carries title/choice/prose. *Pillar II — the payoff.*

### Even more EditMode coverage
- **`GameSettingsTests`** — Story/Normal/Hard combat multipliers, the combat-speed clamp,
  and distinct difficulty/UI-scale blurbs. *Pillar IV — transparent scaling.*
- **`DeedsTests`** — clean run earns nothing; single/compound/threshold predicates fire
  correctly; count tracks the list. *Achievements purely from existing flags.*

`Combat/Reactions.cs` (opportunity attacks) joins the PlayMode list — it instantiates
GridUnits and spawns FX, so it isn't EditMode-reachable.

### Content-integrity coverage
- **`KeepsakesTests`** — hidden until a bond resolves, surface on the right flag (incl. the
  romance tier), monotonic growth, unique ids. *Story made tangible.*
- **`CodexContentTests`** — premise entries known from the start, gated entries unlock on
  their flag, TotalCount tracks the list, unique ids. *"You know what you've witnessed."*
- **`DialogueRunnerTests`** — onEnter effects fire, choices gate on flag conditions,
  picking applies effects & navigates, skill checks branch on success/failure (seeded),
  auto-advance chains lines, conversations end cleanly. *Pillar III's mouthpiece.*

### PlayMode suite (live MonoBehaviour lifecycle)
- **`TurnManagerPlayTests`** — initiative ordering (living only), instant win/loss on a
  one-faction field, round advance on wrap, the incapacitation skip, and one-action-per-turn
  economy. Units place themselves on a live `GridSystem` in `Start()`. *Combat's conductor.*
- **`GridUnitPlayTests`** — spawn placement, PlaceAt occupancy hand-off, FollowPath walking
  tile-by-tile with occupancy following and prior cells clearing, null/empty no-op. *Movement.*

### Next targets
- PlayMode `Reactions` (opportunity attack) — needs an FX/audio-safe harness, since
  `AbilityRunner.ApplyOne` spawns FloatingCombatText / FxSystem / AudioSystem. Defer until a
  test seam mutes those (e.g. a headless flag), then assert the reaction strike + reaction-
  per-round limit.
- EditMode `EraWitness` static graphs — assert authored choices/checks beyond ref integrity.
- A `[SetUp]` that mutes FX/audio globally for combat PlayMode tests.

---

## Coverage so far

**24 EditMode + 4 PlayMode suites (~185 tests)** spanning every major system: dice &
ability math, the GameFlags brain, combat resolution & balance, the Echoes clone, status
effects, character creation & progression, items/inventory/equipment, pathfinding, party
roster, quests, persistence, dialogue, settings/difficulty, and the meta-layer (deeds,
keepsakes, codex, endings + epilogue/Chronicle). The MonoBehaviour systems are all covered
in PlayMode now — `TurnManager` (initiative/economy), `GridUnit` (placement/movement),
`Reactions` (opportunity attacks), and `QuestManager` (the live flag-reactive objective/
completion/failure path). No major system is left untested.

> **Note on this dev environment:** the cloud container that authors this project has
> **no Unity and no C# compiler**, so tests are written here but *compiled and run by CI*
> (or locally in your Unity editor). That's exactly why the game-ci job exists.
