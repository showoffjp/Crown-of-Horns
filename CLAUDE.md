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
python3 tools/gen-standees.py           # Assets/Resources/Standees (world cut-outs)
python3 tools/gen-props.py              # Assets/Resources/Props (chests, doors, …)
python3 tools/gen-lit-tiles.py          # Art/DCSS/lit  (brightness-corrected tiles)
python3 tools/gen-iso-tiles.py          # Art/DCSS/iso  (diamond floor sprites)
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

Having no lock file is fine — Unity regenerates one on open, so
`/Packages/packages-lock.json` is now **gitignored**. Tracking it caused a second
failure mode beyond the corrupt-lock one: Unity rewrites the file on every open,
so `git pull` aborts with "Your local changes would be overwritten by merge" and
the user silently keeps running old code. Recover with
`git restore Packages/packages-lock.json` then pull.

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
**empty** (zero GameObjects). Two runtime hooks furnish it, and they must not
overlap:

* **`Core/GameEntryPoint.cs`** — the original, and the one that *boots the game*:
  it spawns the `MainMenu` front-end unless a menu or campaign already exists.
* **`Core/AutoBoot.cs`** — supplies only the scene furniture nothing else creates
  early enough: the camera (without one the menu draws over an unrendered scene,
  "Display 1 — No cameras rendering"), an `AudioListener`, and the project's only
  `Light`. It must **never** spawn `CampaignBootstrap`: both hooks are
  `AfterSceneLoad` with undefined order, and if AutoBoot won the race
  GameEntryPoint would see a live campaign and skip the title screen.

It is done at runtime rather than saved into the scene because **script .meta
files are largely uncommitted** (38 of 231), so script GUIDs are generated per
machine — a MonoBehaviour reference stored in the scene would load as "missing
script" on any other clone. `CampaignBootstrap` has no serialized fields, so
nothing needs Inspector wiring.

If you want scene-stored references to survive across machines, commit the
`.cs.meta` files Unity generates on import (they are already un-ignored by
`.gitignore`). Do that from **one** machine and let the others pull, so the
GUIDs agree.

## Two builds, very different fidelity

This repo contains **two games sharing one body of content**:

* **The web build** — `play/crown_of_horns.html` (and `play/town_market.html`).
  This is the polished one: painted zone backdrops, weather, pyreflies, portrait
  cards in dialogue, world map with fog of war, WebAudio ambience. Open it in a
  browser; it needs no build step.
* **The Unity project** — a **greybox skeleton**. It shares the zone/dialogue/
  quest data and uses the generated portraits and battle tokens, but renders the
  world with untextured primitives. There is no floor art, no backdrop, and no
  scene lighting.

Screenshots of "the game looking good" are the web build. Do not expect the
Unity Game view to resemble them until the renderer is actually wired to art.

Known Unity-side gaps, roughly in order of payoff:

1. Floor/props render as tinted primitives — no tile art (`Assets/Resources/Art/DCSS`
   holds 114 CC0 tiles that nothing in Unity currently loads).
2. No scene lighting; the Boot scene is empty and cameras are built in code.
3. `play/maps/*.jpg` (the painted zone floors) are web-only and have no Unity path.

### Unity art wiring

Two central hooks give the Unity world its look; prefer extending them over
adding art code to individual content files.

* **`Rendering/MarkerArt.cs`** — called from `Interactable.Start()`, so it reaches
  every NPC, door and object in every scene. It picks art by what the marker *is*:
  a talkable marker gets its `WorldArt.Standee`, a container gets a chest (open
  once emptied), anything else is matched against `PropArt`'s keyword table using
  the marker's own player-facing label. It hides the placeholder cube's mesh while
  keeping its collider, normalises the art to a world height (and clamps its
  width), and adds a painted contact shadow. No art for that name means the tinted
  cube simply stays.

  It runs from `Start`, not `Place`: the `MakeMarker` factories call `Place`
  *before* setting `kind`, `lootFlag` or the dialogue, so art chosen there cannot
  tell a chest from a door.

* **`Rendering/PropArt.cs`** — the label → prop keyword table. Rules are checked in
  order and the first hit wins, so narrow rules come first ("The Stilled Maw" has
  to reach `brazier_cold` before the battle rule claims every "maw"). Extending the
  game with a new kind of object means adding a painter to `tools/gen-props.py` and
  a rule here — not touching any content file.

* **`Rendering/WorldArt.Standee`** — `Resources/Standees/<name>`, the portrait's
  figure cut out on transparency with a bottom-centre pivot. World markers and
  `UnitSpriteSkinner` both use it. Do **not** put a dialogue portrait in the world:
  those are 320x400 opaque cards with a painted backdrop, and stood on a floor tile
  they read as framed pictures hovering in mid-air. Battle tokens
  (`Resources/Sprites`) are worse — circular UI chips with the unit's name printed
  across the bottom.
* **`Rendering/TileFloorRenderer.cs`** — draws the floor from the **diamond**
  sprites in `Resources/Art/DCSS/iso/<group>/<family>N`, with a deterministic
  per-cell variant so rooms do not visibly repeat. Blocked cells become raised
  blocks: darkened copies stacked for the side, a lit cap on top. Scenes set
  `floorFamily` / `wallFamily` right after `AddComponent` to pick their era:
  marble for the Crown Wars court, grey_dirt for the Fugue, infernal for
  Cinderhaunt, tomb for crypts. Missing iso sprites fall back to the old textured
  cubes, and missing textures to flat tints.

Available families — floor: `grey_dirt · infernal · marble · pebble · sandstone ·
tomb`; wall: `brick_brown · brick_dark · marble_wall · stone_dark · tomb_wall`.

Families are **sparse**: `marble` and `marble_wall` start at index 1, `grey_dirt`
and `brick_dark` run to 7, the rest have 0–3. The loader scans 0..8 and keeps
whatever exists, so gaps are fine.

Tiles pass through two generators before Unity sees them, and both must be re-run
(in this order) if the source tiles change:

1. `tools/gen-lit-tiles.py` → `Art/DCSS/lit/`. The raw Crawl tiles average **~20
   of 255** — Crawl draws them small, on black, with its own contrast — so they
   render as solid black. The lift runs on **value only, in HSV**: an RGB multiply
   preserves the ratio between channels and therefore preserves saturation, so a
   dark saturated brick came back as a bright *equally saturated* brick, i.e.
   fluorescent orange. Saturation is scaled down in proportion to how far a pixel
   was lifted and then capped, and the gain is rolled off exponentially rather
   than clipped.
2. `tools/gen-iso-tiles.py` → `Art/DCSS/iso/`. Warps each lit tile into a 128x64
   rhombus with transparent corners, imported at 128 px/unit so one sprite is
   exactly one `tileWidth` x `tileHeight` tile.

Why the diamonds matter: `GridToWorld` projects to a 2:1 diamond lattice, so
axis-aligned square tiles dropped on those centres overlap their neighbours by
half a tile in both axes. That is what made the floor read as a brick *wall* laid
flat with a staircased edge.

### The IMGUI theme

All ~25 screens draw with `OnGUI` against Unity's built-in skin — the flat grey
editor chrome — so the HUD, menu and dialogue read as a debug overlay. Rather
than restyle twenty-five files, `UI/UiTheme.cs` repaints **the shared skin
object**: `GUI.skin` is global, so mutating its styles carries to every component
that draws afterwards. That is the same mechanism `UiScaler` uses for text size,
and the reason a new screen needs no opt-in.

`AutoBoot` puts a `UiThemeApplier` on a DontDestroyOnLoad object, because the
theme has to exist before the title screen and `UiScaler` is only added by
`CampaignBootstrap`. `UiTheme` touches colour, background and padding only —
font sizes stay with the accessibility scale.

**`UI/CombatHUD.cs` is the only uGUI screen in the game** (which is why the
asmdef needs `UnityEngine.UI` at all). It cannot inherit the skin, so it dresses
itself from the same source: `UiTheme.PanelSprite` / `ButtonSprite` are the same
generated art 9-sliced for `Image`, and `UiTheme.Health(frac)` is the one HP
ramp. Style combat through those, not with fresh literals, or combat drifts into
being the one screen that still looks like grey debug boxes.

### Everything is 2D

`GridToWorld` fakes the isometry in the **XY plane** and leaves Z for sorting, so
despite the cubes there is no 3D scene here. Consequences worth knowing:

* `CameraBillboard` is a no-op — the camera looks straight down −Z.
* A steeply angled `Light` lights the camera-facing quads at a glancing angle and
  smears every marker's shadow into a long streak across the floor. `AutoBoot`
  therefore rakes its key light only slightly and casts **no** shadows; markers
  get a painted contact blob from `MarkerArt` instead.
* Depth is world **Y alone** (`IsoDepthSorter`). The old `-(x + y)` let a marker's
  horizontal position change its depth, so a near-right sprite could draw behind a
  far-left one. Art lifted off the ground passes `basis` so it still sorts by the
  tile it stands on.

### Containers must carry a lootFlag

A scene rebuilds all its markers from scratch every time the party walks back in,
so a container whose emptied state lives only in `Interactable.looted` refills
itself on every return trip. `BaldursGateHub`'s Strongbox did exactly that, handing
out its gold, potion and the Cinderhaunt key again and again. Every `MakeContainer`
now takes a `lootFlag`, and `Interactable.Start` restores `looted` from it.
