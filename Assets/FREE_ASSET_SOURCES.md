# Free Asset Sourcing Guide (CC0-first)

How to fill the game with **legally safe** free fantasy / isometric assets — written for a repo
that is (or is about to be) **public**, so licensing is treated as load-bearing, not an afterthought.

> **Why this is a guide, not a bulk import:** the authoring container's network is **GitHub-only**
> (kenney.nl, opengameart.org, the Unity Asset Store all return 403 here), and — more importantly —
> **"import everything" is a license trap.** Asset Store "free" packs may not be redistributed in a
> repo (their EULA), and many "free" assets are CC-BY (attribution) or non-commercial. Pull these in
> your **Unity editor / browser** (where it's legal for your account) and commit only what the
> license allows. See `ASSET_IMPORT_MANIFEST.md` for the per-asset triage workflow.

---

## The license rule of thumb

| License | Safe to commit to a public repo? | Obligation |
|---|---|---|
| **CC0 / Public Domain** | ✅ Yes | None (credit is kind, not required) |
| **CC-BY** | ✅ Yes | **Must** credit the author (see `ATTRIBUTIONS.md` below) |
| **CC-BY-SA / GPL (copyleft)** | ⚠️ Caution | Credit **and** share-alike; can force your derivatives' license |
| **CC-BY-NC (non-commercial)** | ❌ No (if you may ever sell) | Non-commercial only |
| **Unity Asset Store "free"** | ❌ **Never commit** | Per-account license; import in-editor only |

**Default to CC0.** It's the only category with zero strings.

---

## Recommended sources, mapped to the project

### 🟢 CC0 — commit freely (best first stop)

- **Kenney** — <https://kenney.nl> · **CC0**. The gold standard. Has **isometric** tile packs,
  **fantasy** UI, RTS/RPG sprites, particle/FX packs, and audio. → `Assets/Art/Sprites/Tiles`,
  `Assets/Art/Sprites/UI`, `Assets/Art/Sprites/FX`, `Assets/Audio/SFX`. *(Your uploaded
  `kenney_fantasy-ui-borders.zip` is already from here — extract it into `Assets/Art/Sprites/UI/`.)*
- **Kay Lousberg / KayKit** — <https://kaylousberg.com> · **CC0**. Stylized 3D **dungeon**,
  **adventurers**, **skeletons**, **enemies** — practically a CRPG starter kit. → `Assets/Models`.
- **Quaternius** — <https://quaternius.com> · **CC0**. Modular 3D dungeons, fantasy RPG props,
  creatures, weapons. → `Assets/Models`, `Assets/Art/Textures`.
- **OpenGameArt (CC0 filter)** — <https://opengameart.org> · filter **License = CC0**. Deep well of
  isometric tilesets and sprite sheets. *Check each item's license — the site is mixed.*

### 🟡 CC-BY — commit WITH attribution (record it in `ATTRIBUTIONS.md`)

- **Game-Icons.net** — GitHub: `game-icons/icons` · **CC-BY 3.0**. **4000+** fantasy/RPG icons —
  perfect for **ability and item icons** (`Assets/Art/Sprites/Items`, `UI`). *Reachable from here
  via GitHub if you want me to pull a curated subset — see "Want me to fetch?" below.*
- **Kevin MacLeod / Incompetech** — <https://incompetech.com> · **CC-BY**. Royalty-free orchestral/
  ambient tracks for the era themes (Netheril gold, Fugue grey…). → `Assets/Audio/Music`.

### 🟠 Copyleft / careful

- **LPC (Liberated Pixel Cup)** — OpenGameArt · **CC-BY-SA 3.0 + GPL 3.0**. Huge, consistent
  top-down/iso character sprite set with a generator. Powerful, but **share-alike** — keep it
  quarantined and documented if you use it, or prefer CC0 alternatives.

### 🔵 Audio one-shots

- **Sonniss GDC Game Audio Bundle** — <https://sonniss.com/gameaudiogdc> · **royalty-free for
  commercial use**. Gigabytes of pro SFX. → `Assets/Audio/SFX`, `Ambience`.
- **Freesound.org** — per-sound license (filter **CC0**). → `Assets/Audio/SFX`.

### ⛔ Do NOT commit
- **Unity Asset Store** packs (even free) — import in-editor via *Window → Package Manager → My
  Assets*; list them in `ASSET_IMPORT_MANIFEST.md` §1 instead. Committing them is an EULA violation,
  doubly so on a public repo.

---

## Mapping to the era palettes (`DESIGN.md` §5)

Pick CC0 sets that can be **tinted** to each era rather than hunting era-specific art:

- **Netheril** — gold/sky-blue gilded tiles + floating-architecture props (Kenney/Quaternius).
- **Crown Wars** — silver/elven-green; clean fantasy tilesets + blade-singer-ish models (KayKit).
- **Time of Troubles** — bone/blood; crypt & temple props (KayKit dungeon/skeletons).
- **Spellplague** — the signature **blue fire**; Kenney particle FX recolored.
- **The Fugue / the Wall** — grey desaturation; reuse crypt tiles at low saturation (theme = absence).

---

## `ATTRIBUTIONS.md` (create this when you add any CC-BY asset)

```
# Attributions
This project uses the following assets under their licenses:

- Game-Icons.net icons — CC-BY 3.0 — authors listed at https://game-icons.net — used for ability/item icons.
- "Track Name" by Kevin MacLeod (incompetech.com) — CC-BY 4.0.
- <asset> by <author> (<url>) — <license>.
```
A public repo with CC-BY assets and no attribution file is a license violation. This file fixes that.

---

## Want me to fetch a curated set?

I can only reach **GitHub** from here, so I can pull from **GitHub-hosted, license-verified** sources
(e.g., the `game-icons/icons` CC-BY repo) — I'll read the `LICENSE`, pull a *curated subset* (not the
whole thing — quota!), drop it in the right folder, and write the `ATTRIBUTIONS.md` entry. Everything
else (Kenney, KayKit, Quaternius, Asset Store) you grab in your editor/browser, where it's legal and
unthrottled, and bring in via the LFS workflow in `ASSETS.md`.

**Just say the word** (e.g. *"pull ~40 fantasy ability icons from game-icons"*) and I'll do that one
safely, with attribution. I won't bulk-dump "everything" — that's how repos get bloated and into
license trouble.
