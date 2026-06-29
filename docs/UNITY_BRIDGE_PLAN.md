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

## What does NOT cross yet — the finite Unity-side work list

These are the only things standing between "data converted" and "game runs the session content."
The converter counts each one exactly (see `gap-report.json`); current counts:

| Gap | Count | What it is | Unity-side fix |
|---|---|---|---|
| **variants** | 278 | nodes whose text changes by `when` (flags/disp/deity/etc.) | add `DialogueNode.variants` (a `when`→text list); runner picks first match, else base text. The converter already stores the unconditional default as `text`, so a variant-less C# still *runs* — it just won't react. |
| **crit / fumble** | 84 / 84 | nat-20 / nat-1 narration branches on a skill check | add `critNodeId` / `fumbleNodeId` to `DialogueChoice`; runner routes on a natural 20/1 before DC compare. |
| **non-flag gates** | 75 | `when` gates on race/class/deity/ability/gender/law/morality | add character-state clauses to `FlagClause` (or mirror these into bool flags at chargen). |
| **draw** | 1 | the Wayward Mile random-event router | a `DrawNode` component; or precompute one branch. Single use — low priority. |
| **banter** | 1 | the campfire banter intercept node | the banter system is its own engine (`CampfireBanter.cs` exists); wire the intercept, don't port it as dialogue. |
| **dynamic** | 18 | nodes whose choices are generated at runtime | bespoke per node; smallest count, handle last. |

`droppedSkillName` (286) and `choiceTag` (252) are **informational, not blocking**: C# keeps
ability+DC (the skill *name* is cosmetic), and `[RETURNED]`-style tags are a surfacing hint the UI
can re-derive. They're reported so nothing is invisible.

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
4. **Add the gaps in priority order.** variants (biggest reactivity win) → crit/fumble (the crit
   comedy) → non-flag gates → dynamic → draw. Each is an independent, testable engine extension.
5. **Re-export, re-import.** The converter is the single source of truth; re-run it after any
   prototype change. Nothing is authored twice.

## The honest verdict

The **converter is verified** (tested JS, runs here). The **C# loader is not** (no compiler in this
environment) — it's a small, well-scoped draft you compile once in the Editor. The *logic* of every
conversation is already proven by the 1:1 JS port and its passing tests; the bridge's job is only to
move the authored data into the engine that renders it. That's a known, finite list of work — not a
leap of faith.
