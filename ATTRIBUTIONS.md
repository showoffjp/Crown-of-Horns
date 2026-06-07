# Attributions

This project uses third-party assets under their licenses. Crediting them here satisfies the
attribution terms (required because the repository is public).

---

## Game-Icons.net — fantasy/RPG icons · **CC BY 3.0**

A curated set of **57** vector icons (weapons, armor, consumables, spells, loot, thematic
death/grave/holy motifs, and a full **status-effect** set) used as **ability, item, and
condition icons**.

- **Source:** <https://game-icons.net> · repo <https://github.com/game-icons/icons>
- **License:** Creative Commons Attribution 3.0 (CC BY 3.0) — <https://creativecommons.org/licenses/by/3.0/>
  (full text vendored at `Assets/Art/Sprites/game-icons-LICENSE.txt`)
- **Location in project:**
  - `Assets/Art/Sprites/Items/game-icons/` — gear, loot, consumables, thematic (38)
  - `Assets/Art/Sprites/FX/game-icons/` — spell/projectile effects (7)
  - `Assets/Art/Sprites/Status/game-icons/` — status effects, renamed 1:1 to the `Condition` enum (12)
- **Authors of the icons used (credit required by CC BY):**
  - **Lorc** — <https://lorcblog.blogspot.com> (39 icons)
  - **Delapouite** — <https://delapouite.com> (13 icons)
  - **Sbed** — <https://opengameart.org/content/95-game-icons> (2 icons)
  - **Willdabeast** — <https://wjbstories.blogspot.com> (1 icon)
  - **Skoll** — game-icons.net contributor (1 icon)
  - **Carl Olsen** — <https://twitter.com/unstoppableCarl> (1 icon)
- Item/FX filenames are preserved (kebab-case) for traceability; the Status set is renamed to the
  enum (e.g. `poisoned.svg` ← `lorc/poison-bottle`) — source mapping is in the commit history.

> **Unity import note:** these are **SVG** (vector). To use them as sprites, add Unity's **Vector
> Graphics** package (`com.unity.vectorgraphics`) — or export PNGs from game-icons.net at the size
> you need. They're committed as text (tiny), so no Git LFS is required for them.

> **In-game credit:** also surface this attribution somewhere player-visible (e.g. the Credits
> screen) — CC BY asks for credit "in the manner specified," and a Credits line is the norm for games.

---

## (Template for future CC-BY assets)

```
## <Pack name> — <what> · <license>
- Source: <url>
- License: <name + url>
- Location: <folder>
- Author(s): <name> (<url>)
```

> **CC0 / public-domain assets** (e.g. Kenney, KayKit, Quaternius) need no attribution, but listing
> them here anyway is good manners and good record-keeping. See `Assets/FREE_ASSET_SOURCES.md`.
