# Crown of Horns — working notes

## Verifying a change (do this before every push)

```bash
cd play && node run-all.js          # THE gate. Exactly what CI runs.
```

`run-all.js` is the single source of truth for the "Verify engine port" check.
Do **not** substitute a loop over `play/*.test.js`: that misses `smoke.js`
(boots each page under a Node DOM shim), `verify.js`, `diagnose.js` and
`autobattle.js`. A change can pass all 42 `.test.js` suites and still break the
build — this happened in v6.114.0, when new top-level
`document.addEventListener` calls killed the smoke boot.

Page scripts must tolerate the smoke shim, whose `document` provides only
`getElementById` and `createElement`. Guard any top-level DOM listener:

```js
if (typeof document !== "undefined" && document.addEventListener) { ... }
```

Other gates worth running when they're in scope:

```bash
bash tools/asset-check.sh                  # engine-expected assets (target 49/49)
bash tools/generate-asset-manifest.sh      # must be a byte-identical no-op before commit
```

## Regenerating derived files

Generated artifacts are committed, so anything touching content or art needs a
rebuild before the gate:

```bash
python3 tools/gen-portraits-v3.py       # Assets/Resources/Portraits (Unity PNGs)
python3 tools/gen-portrait-thumbs.py    # play/portraits (web dialogue faces)
python3 tools/gen-tokens-v2.py          # Assets/Resources/Sprites (battle tokens)
python3 tools/gen-zone-backdrops.py     # play/maps (painted zone floors)
python3 tools/make-town-market.py       # play/town_market.html (the walkable game)
python3 tools/make-cast-gallery.py      # play/cast_gallery.html
python3 tools/make-combat-demo.py       # injects sprites into play/crown_combat.html
python3 tools/make-all-in-one.py        # play/crown_of_horns.html (bundles 11 tabs)
```

Note `play/crown_combat.html` is hand-authored source; `make-combat-demo.py`
only swaps the sprite blob between its `/*<SPR>*/ … /*</SPR>*/` markers.

## Adding a zone

A new `play/<zone>.json` needs all of: an entry in `make-town-market.py`
(the loader line, the `ALL_CONVS` concatenation, and `ALL_SCENES`), an exit
wired from an existing zone onto a walkable tile, `NPC_SENSE` entries for any
sense-reads, and — per the house contract the gate enforces — a `[RETURNED]`
tagged choice on every NPC conversation, terminal nodes with no `choices` key,
and exhaustive `variants` (the last one unconditioned).

## Unity import / Safe Mode

`Assets/Scripts/SunderedCrown.asmdef` governs all scripts under `Assets/Scripts`.
An assembly definition replaces the default auto-references, so **every Unity
assembly a script uses must be listed in its `references`**. `CombatHUD.cs` uses
`UnityEngine.UI` and `UnityEngine.EventSystems` (both live in the
`UnityEngine.UI` assembly from `com.unity.ugui`), so that reference is required —
omitting it yields `CS0246: The type or namespace name 'Image' could not be
found`.

Errors reported against files under `Library/PackageCache/...` are **not** fixable
from this repo: they mean the local package cache is stale or half-extracted.
Recover with Unity closed:

```bash
rm -rf Library/PackageCache Library/ScriptAssemblies    # safe: both are regenerated
```

then reopen the project and let it reimport.

**CI does not compile C#.** The `Unity EditMode tests` job self-skips unless a
`UNITY_LICENSE` secret exists, so it reports green without building anything.
The only static C# guards are:

```bash
bash tools/check-cs-structure.sh     # brace balance + one namespace per file
python3 tools/check-asmdef-refs.py   # asmdef references vs. what scripts `using`
```

Both run in the `Repo hygiene` job. Treat a green CI as saying nothing about
whether the Unity project actually compiles.
