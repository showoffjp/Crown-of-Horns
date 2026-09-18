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
bash tools/check-cs-structure.sh      # brace balance + one namespace per file
python3 tools/check-asmdef-refs.py    # asmdef references vs. what scripts `using`
python3 tools/check-package-lock.py   # lock file vs. manifest dependencies
```

Both run in the `Repo hygiene` job. Treat a green CI as saying nothing about
whether the Unity project actually compiles.

### packages-lock.json

Unity resolves packages **from the lock file** when one exists, so a lock that
omits a manifest dependency half-wires that package: its source extracts into
`Library/PackageCache` but its assembly references are never established, and it
fails to compile its own files. That is what produced

    Library/PackageCache/com.unity.ugui@.../UI/Core/Dropdown.cs:
    error CS0246: The type or namespace name 'Image' could not be found

The committed lock held 37 builtin modules and **no registry packages**, while
the manifest required `com.unity.ugui` and `com.unity.test-framework`. Deleting
`Library/` never helped, because the lock lives in `Packages/` and was tracked.

Having no lock file is fine — Unity regenerates one on open. If you commit a
regenerated lock, `tools/check-package-lock.py` will verify it stays consistent.

### Packages/ must contain only manifest.json

Unity treats **any folder inside `Packages/`** as an *embedded* package that
overrides the registry version `manifest.json` asks for. Symlinked package
folders there (`Packages/com.unity.ugui -> ...`) make Unity load uGUI from a
path it does not manage, so the `UnityEngine.UI` assembly never exists and
every script using it fails with

    CS0234: The type or namespace name 'UI' does not exist in the namespace
    'UnityEngine'

alongside editor warnings that "assets located in immutable packages were
unexpectedly altered". `tools/check-package-lock.py` fails on any stray entry.

To recover, with Unity closed, delete the **links** (never their targets), then
delete `Library/` and reopen:

```powershell
# Windows PowerShell, from the project root
Get-ChildItem Packages -Force | Where-Object { $_.LinkType } | ForEach-Object { $_.Delete() }
Remove-Item -Recurse -Force Library
```

## Running the game in Unity

`Assets/Scenes/Boot.unity` is the only scene in Build Settings and it ships
**empty** (zero GameObjects). Pressing Play worked only because
`Assets/Scripts/Core/AutoBoot.cs` spawns `CampaignBootstrap` via
`[RuntimeInitializeOnLoadMethod]` when the active scene is named `Boot`.

It is done at runtime rather than saved into the scene because **script .meta
files are largely uncommitted** (38 of 231), so script GUIDs are generated per
machine — a MonoBehaviour reference stored in the scene would load as "missing
script" on any other clone. `CampaignBootstrap` has no serialized fields, so
nothing needs Inspector wiring.

If you want scene-stored references to survive across machines, commit the
`.cs.meta` files Unity generates on import (they are already un-ignored by
`.gitignore`). Do that from **one** machine and let the others pull, so the
GUIDs agree.
