# The Unity Bridge — Honest Field-by-Field Plan

This is the concrete, de-risked path from the **proven prototype** (`play/*.json`, 1,500+ passing
assertions) to the **real Unity game** (`Assets/`, C# ScriptableObjects). It is deliberately honest
about what crosses cleanly and what does not. Nothing here is a promise; the converter is tested and
runnable today, and every feature that *can't* cross yet is counted, not hand-waved.

## What exists right now (verified)

- **`tools/json-to-unity.js`** — converts every prototype conversation + the side-quest catalog into
  JsonUtility-shaped JSON whose field names match the real C# classes exactly.
- **`tools/json-to-unity.test.js`** — 30 assertions proving the field mapping, the `when`→`FlagClause`
  translation, and that every C#-incompatible feature is counted. Wired into `play/run-all.js`.
- Run `node tools/json-to-unity.js` → writes `tools/unity-export/{dialogue-graphs,quests,gap-report}.json`
  (gitignored; regenerate any time in under a second).

Last build: **457 conversations / 3,726 nodes / 20 quests** from 78 files.

## The native bridge target: C# content classes (recommended path)

The Unity game does **not** author dialogue as JSON assets — it hand-writes C# that builds
`DialogueGraph` in code (see `Assets/Scripts/Content/GarrowQuestContent.cs`). So the cleanest, lowest-risk
bridge emits **C# that looks exactly like what a human wrote**, drops into `SunderedCrown.Content`, and
compiles into the existing `DialogueRunner` with no asset-import step.

- **`tools/json-to-csharp.js`** — emits one C# content class per prototype zone, reusing the tested
  normalization from `json-to-unity.js`. Each class exposes `public static List<DialogueGraph> Build()`.
- **`tools/json-to-csharp.test.js`** — 30 assertions; since there's no C# compiler here, it verifies the
  output as hard as possible without one (string-escape round-trip, brace/bracket/paren balance with
  string contents masked, enum validity, and **every node reference resolves**).
- **`tools/bridge-sample/`** — two committed preview files (`SeconddeathBridgeContent.cs`,
  `LongoddsBridgeContent.cs`) so the output is reviewable in the repo. They live outside `Assets/`, so
  Unity never compiles them.

### The dedup finding (important)

`dialogue-data.json` is **extracted from the existing C# content** — those conversations are already in
the build. The emitter scans `Assets/Scripts/**/*.cs` for `conversationId = "..."` and **skips anything
already authored**, so re-emitting can't collide. Current split:

- **201 conversations already in the C# build** → skipped.
- **256 genuinely new conversations** (this session's work: Calloway, Sevrin, the Second Death mystery,
  Dot, the signature systems, the Wayward Mile, the new banter) → emitted.
- The new content is **100% reference-clean**: every `next`/`auto`/`fail` resolves to a real node (or the
  `END` sentinel → empty = end conversation). The only broken branches in the corpus (37) live entirely
  in the already-built main spine, where they're handled by runtime/dynamic C# code — not the bridge's
  concern.

> Two emit targets exist and both are tested: **JSON + JsonUtility loader** (`json-to-unity.js`) and
> **native C# content classes** (`json-to-csharp.js`). The C# path is recommended — it matches the
> codebase's own authoring idiom and needs no importer.

## The field mapping (prototype JSON → C# class)

### Conversation → `SunderedCrown.Dialogue.DialogueGraph`

| Prototype | C# field | Notes |
|---|---|---|
| `conv.id` | `conversationId` | 1:1 |
| `conv.start` | `startNodeId` | falls back to first node id |
| `conv.nodes[]` | `nodes` (`List<DialogueNode>`) | converted per below |

### Node → `DialogueNode`

| Prototype | C# field | Notes |
|---|---|---|
| `id` | `id` | 1:1 |
| `speaker` | `speaker` | defaults to `"NPC"` |
| `text` | `text` | 1:1 |
| `onEnter[]` **+** node-level `effects[]` | `onEnter` (`FlagClause[]`) | both are apply-on-show in the prototype; folded into one array |
| `auto` | `autoNextNodeId` | blank = end conversation |
| `choices[]` | `choices` (`DialogueChoice[]`) | converted per below |

### Choice → `DialogueChoice`

| Prototype | C# field | Notes |
|---|---|---|
| `text` | `text` | 1:1 |
| `next` | `nextNodeId` | 1:1 |
| `fail` | `failNodeId` | skill-check failure target |
| `effects[]` | `effects` (`FlagClause[]`) | **op names already match `FlagOp`** — pass-through |
| `conditions[]` | `conditions` (`FlagClause[]`) | already FlagClause-shaped — pass-through |
| `when{}` | `conditions` (`FlagClause[]`) | **translated** (see below), appended to conditions |
| `check.ability` | `checkAbility` (`Stats.Ability`) | enum-by-name; unknown → `Charisma` + reported |
| `check.dc` | `checkDC` | 1:1 |

### `when{}` → `FlagClause[]` translation (the load-bearing piece)

| `when` key | becomes | `FlagOp` |
|---|---|---|
| `flags: [a,b]` | one clause each | `RequireBoolTrue` |
| `flagsNot: [c]` | one clause each | `RequireBoolFalse` |
| `flag: "d"` | one clause | `RequireBoolTrue` |
| `int: {k: n}` | one clause, `amount = n` | `RequireIntAtLeast` |

The C# `FlagOp` enum (`SetTrue, SetFalse, AddInt, RequireBoolTrue, RequireBoolFalse, RequireIntAtLeast`)
already contains every op the prototype's effects and translatable conditions use. That's why the
crossing is clean: **no new C# enum values are required for the data that crosses.**

### Quest → `SunderedCrown.Quests.Quest`

| Prototype (`sidequests.json`) | C# field |
|---|---|
| `questId` | `questId` |
| `title` | `title` |
| `premise` | `summary` |
| `completionFlag` / `failureFlag` | same |
| `objectives[].{objectiveId, desc, completionFlag, hidden, optional}` | `QuestObjective.{objectiveId, description, completionFlag, hidden, optional}` |
| (defaults) | `experienceReward = 100`, `goldReward = 0` |

## Engine features — now IMPLEMENTED in the runner

**Every reactivity feature the prototype uses is now done in C#** — additive and backward-compatible, so
existing conversations are untouched and behave identically. All are emitted by `json-to-csharp.js` and
verified by its gate (every routing target resolves — 236 variants, 84 crit, 84 fumble, plus the draw
router, banter, and dynamic hooks):

| Feature | Status | How |
|---|---|---|
| **variants** | ✅ implemented | `DialogueNode.variants` (`DialogueVariant { FlagClause[] when; string text }`). `DialogueRunner.ResolveText` returns the first variant whose conditions pass, else `text`, exposed as `CurrentText` (the UI prefers it). The emitter translates each variant's flag/int `when`; the unconditional default stays as `text`. |
| **crit / fumble** | ✅ implemented | `DialogueChoice.critNodeId` / `fumbleNodeId`. `DialogueRunner.Choose` captures the raw d20: a natural 20 routes to the crit branch and a natural 1 to the fumble branch (regardless of DC); both are no-ops when unset, so old checks are unchanged. |
| **non-flag gates** | ✅ implemented | race/class/deity/gender/law/morality/background/ability gates map onto GameFlags via the `pc.*` convention (`PlayerProfileFlags.Apply`, called once at chargen), so the runner needs **no special evaluation** — they're ordinary bool/int clauses. A scalar gate → one `RequireBoolTrue pc.<cat>.<value>`; an ability gate → `RequireIntAtLeast pc.score.<ability>`; an array (OR) gate → OR-expanded into one alternative per value (the player has exactly one race/etc., so at most one matches). 116 bool + 27 int clauses emitted across the new content. |
| **draw (random routing)** | ✅ implemented | `DialogueNode.draw` (`DrawOption[] { to, onceFlag, needFlag }`) + `drawCountKey`/`drawMax`/`drawElse`. `DialogueRunner.HandleDraw` picks a random eligible option (skipping seen `onceFlag`s and unmet `needFlag`s, honouring `drawMax`), sets its once-flag, bumps the counter, and routes — falling back to `drawElse`/auto when exhausted. Drives the Wayward Mile caprice router. |
| **banter** | ✅ implemented | `DialogueNode.isBanter` + a `BanterHook` on the runner. A banter node hands off to the party-banter system (`CampfireBanter`) and auto-advances; no subscriber = it simply auto-advances. The banter engine stays separate — the node is just the intercept. |
| **dynamic** | ✅ implemented | `DialogueNode.isDynamic` + a `ResolveDynamicChoices` hook. Game code supplies runtime choices (e.g. the crier who recaps your deeds); returning null falls back to the node's static choices/auto, so it always runs. |

## What does NOT cross — informational only (nothing blocking)

`droppedSkillName` (286) and `choiceTag` (252) are the last two entries, and they are **not blocking**:
C# keeps ability+DC (the skill *name* is cosmetic), and `[RETURNED]`-style tags are a surfacing hint the
UI can re-derive. Every reactivity feature the prototype uses now has a C# home.

> **One chargen call wires up the faith/identity gates:** at character creation, call
> `PlayerProfileFlags.Apply(GameFlags.Current, playerSheet, new PlayerProfile { gender, deity, law, morality, background })`.
> Race/class/ability scores are read off the sheet automatically. `deity = "None"` marks the Faithless —
> the game's central reactive axis. Until that call exists, these gates simply never match and the default
> text/choices show; nothing breaks. The banter and dynamic hooks are likewise optional — unwired, those
> nodes gracefully auto-advance.

`droppedSkillName` (286) and `choiceTag` (252) are **informational, not blocking**: C# keeps
ability+DC (the skill *name* is cosmetic), and `[RETURNED]`-style tags are a surfacing hint the UI
can re-derive. They're reported so nothing is invisible.

> **Honest caveat:** the runner changes (`DialogueGraph.cs`, `DialogueRunner.cs`, `DialogueScreen.cs`)
> are additive and mirror the proven prototype semantics (which the JS port tests cover), but there is
> **no C# compiler in this environment** — they are unverified until compiled once in the Editor. The
> changes are deliberately small and backward-compatible to keep that first compile boring.

## Staged import checklist

1. **Loader (small, C#).** Write `Assets/Scripts/Dialogue/Editor/DialogueImporter.cs`: read
   `dialogue-graphs.json`, and for each graph `ScriptableObject.CreateInstance<DialogueGraph>()` +
   `EditorJsonUtility.FromJsonOverwrite(json, asset)` + `AssetDatabase.CreateAsset(...)`. Same for
   quests. **Unverified until compiled** — there is no C# compiler in this environment, so treat the
   importer as a clearly-labeled draft until you run it in the Editor once.
2. **Compile.** Open the project in Unity 6 (6000.4.9f1). Fix the handful of compile errors the
   session content surfaces (paste them back and they get fixed fast). The data model itself is
   structurally clean (static-audited).
3. **Import without the gaps.** Run the importer. Every conversation loads and *plays* using base
   text + translatable conditions/effects. This is a real, walkable build of the whole session's
   content — just not yet reactive on the 6 gap features.
4. **Reactivity.** variants + crit/fumble are already in the runner (above). The remaining gaps —
   non-flag gates → dynamic → draw → banter — are independent, each small, and addressed last.
5. **Re-export, re-import.** The converter is the single source of truth; re-run it after any
   prototype change. Nothing is authored twice.

## The honest verdict

The **converter is verified** (tested JS, runs here). The **C# loader is not** (no compiler in this
environment) — it's a small, well-scoped draft you compile once in the Editor. The *logic* of every
conversation is already proven by the 1:1 JS port and its passing tests; the bridge's job is only to
move the authored data into the engine that renders it. That's a known, finite list of work — not a
leap of faith.
