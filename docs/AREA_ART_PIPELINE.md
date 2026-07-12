# Area Art Pipeline — painted backdrops, the Infinity-Engine way

> **TL;DR.** Area backgrounds are *painted/rendered images*, not built in the game engine. The
> engine composites walkable characters, props, and lighting **on top of** a static painted
> backdrop, gated by an invisible **search map** (walkability) and lit by an optional **light
> map**. This is exactly how BG/IWD/PST worked, and it's how Crown of Horns should work too.
>
> **Generate the paintings with a real image tool (e.g. Grok Imagine).** This repo's
> `tools/make-painted-maps.py` only produces *procedural placeholders* — fine for greyboxing,
> not a substitute for painted art. This doc is the spec so the painted art drops straight in.

---

## 1. The camera (prompt for this, paint to this)

The Infinity Engine uses a fixed **dimetric** projection, not true isometric:

| Property | Value |
|---|---|
| View | bird's-eye, looking **down and to the side** |
| Vertical tilt | ~**51° from vertical** (≈ a 0.61–0.75 vertical squash of the ground plane) |
| Rotation | ground grid at **45°** in plan; the camera does **not** rotate |
| Verticals | walls/towers rise **straight up**, *not* foreshortened |
| Light | one consistent key light, classically from the **upper-left**, warm; cool fill |

**Prompt cue for an image tool:** *"isometric/dimetric bird's-eye RPG area background in the Infinity Engine / Baldur's Gate 2 / Icewind Dale style, ~3/4 top-down angle, hand-painted, pre-rendered, moody torch-lit, no characters, no UI, fills the whole frame."*

Keep one painting = one **area** (interiors can be a building **cut-away** — near walls removed
so you see into the rooms; exteriors fill edge-to-edge).

---

## 2. Deliverables per area

Paint/produce these layers (all the same pixel dimensions, PNG):

1. **`<area>.png`** — the backdrop. The hero art. Fills the frame, no characters/UI.
2. **`<area>_search.png`** — the **search map** (walkability + terrain). Indexed/flat colour, one
   colour per terrain class. Minimum: **walkable** vs **blocked**. Optional classes below.
3. **`<area>_height.png`** *(optional)* — render order / elevation, so a token behind a pillar is
   occluded. Greyscale; brighter = nearer the camera.
4. **`<area>_light.png`** *(optional)* — a baked light map (torch pools, god-rays) multiplied
   over moving tokens so they pick up the scene's lighting. Day/night variants if wanted.

### Search-map colour key (suggested, matches the engine's needs)

| Colour (RGB) | Class | Meaning |
|---|---|---|
| `#000000` | impassable | wall, water, void — tokens can't enter |
| `#ffffff` | normal | open ground |
| `#3a6ea5` | shallow / slow | wade, half-speed |
| `#7a5230` | wood | footstep SFX = wood |
| `#8a8a8a` | stone | footstep SFX = stone |
| `#c8a24b` | trigger | an exit / transition tile (pair with a target in scene JSON) |

The engine already models walkability the same way in JS: `buildBlocked` + 8-direction A*
(`findPath`) in `tools/make-town-market.py`. A painted search map is the **authoring surface**
for that same blocked-set — paint black where `BLOCKED` should be true.

---

## 3. Resolution & framing

| Use | Pixels |
|---|---|
| Single-screen area (this demo's zones) | **1280×800** (or 1920×1080 for a 16:9 master) |
| Scrolling area (a town, a dungeon level) | paint **wide** — e.g. 2400×1600+, the camera pans |

Paint a touch of bleed past every edge; the camera never shows a hard border.

---

## 4. How it composites at runtime (Unity & the web demo)

```
draw  <area>.png                     (static backdrop, z = back)
for each entity sorted by (gx+gy):   (painter's algorithm, IE depth sort)
    if height_map[token] < backdrop  -> token is occluded by scenery, skip/clip
    draw token sprite
    multiply token by light_map at its position   (picks up torch pools)
draw  overhead VFX / weather / fog    (z = front)
input: a click resolves against search_map -> A* path over walkable cells
```

- **Web demo (today):** `town_market.html` draws each zone procedurally. To use a painted
  backdrop, a scene gains `"backdrop": "maps/<area>.png"` and the canvas blits it first, then
  draws the existing iso tokens over it (their grid is unchanged). The `search` map can replace
  the per-prop `BLOCKED` set 1:1.
- **Unity:** the backdrop is a single full-screen `SpriteRenderer` (sorting layer *Background*);
  the search map feeds the existing `TileGraph`/pathfinder; tokens sort by world-y; the light
  map is a multiply overlay. No new engine concepts — IE did all of this in 1998.

---

## 5. Division of labour

| Who | What |
|---|---|
| **Image tool (Grok Imagine / a painter)** | the `<area>.png` paintings — the part that needs real art |
| **This codebase / the systems** | the reactive content, the walkable engine, the search-map collision, the dialogue/quest data, the byte-parity rules — and a `make-painted-maps.py` *placeholder* generator for greyboxing only |

Paint an area, drop the four PNGs in `play/maps/` (or `Assets/Resources/Art/Areas/` for Unity)
named to match a scene id, and the engine has everything it needs to make it walkable.

---

*The procedural placeholder (`tools/make-painted-maps.py`) is honest about what it is: code
drawing shapes. It greyboxes the framing, materials, and light so a painter (or Grok) knows the
target — it is not the final art, and was never meant to be.*
