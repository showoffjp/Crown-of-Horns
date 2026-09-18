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
