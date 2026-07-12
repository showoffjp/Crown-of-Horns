# Bridge sample — committed evidence

These two `.cs` files are **committed previews** of what `tools/json-to-csharp.js` emits, so the
bridge output is reviewable in the repo without regenerating it:

- `SeconddeathBridgeContent.cs` — the "Second Death" murder mystery (`sx.sahl` / `sx.corwin` / `sx.liss`).
- `LongoddsBridgeContent.cs` — Calloway "The Honest Devil" (`cal.*`) + the Auditor.

They live **outside `Assets/`** on purpose: Unity only compiles scripts under `Assets/`, so these
previews cannot affect the build. They have **not** been compiled (there is no C# compiler in this
environment) — they are verified structurally by `tools/json-to-csharp.test.js` (string escaping,
brace/bracket/paren balance with string contents masked, every node reference resolves, every enum
value is real).

To regenerate the full set (256 new conversations across 78 zones, deduped against the existing C#
build): `node tools/json-to-csharp.js` → writes to `tools/unity-export/csharp/` (gitignored).

When you're ready to import: drop the generated files into `Assets/Scripts/Content/Bridge/`, call each
`<Class>.Build()` from your content registry, and compile once in the Editor. See
`docs/UNITY_BRIDGE_PLAN.md` for the full path and the engine-feature gaps.
