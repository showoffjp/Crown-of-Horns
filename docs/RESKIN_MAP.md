# The Reskin Layer — a licence-independent build from the same source

**Question this answers:** *if the Forgotten Realms licence falls through, how badly does it hurt the
project, and can we make it "incredibly similar" without infringing?*

**Answer:** cosmetically, not structurally. The game's value — the souls, the reactive systems, the
code, the ~258-soul body of writing — is original and yours already. What's Forgotten-Realms-proprietary
is a thin layer of ~50 proper nouns sitting on top, and this layer swaps them for original equivalents
**mechanically, from the same source**, so you can ship either version. Proven green by `tools/reskin.test.js`.

> Not legal advice — get an IP attorney to bless a commercial release. But the practical lay of the land
> below is well-trodden.

## What's actually protected (and what isn't)

| Kind | Covers | Your exposure |
|---|---|---|
| **Copyright** | *specific expression* — named characters (Elminster), named settings/institutions, verbatim text | Only the ~50 names below. Your original souls, beats, and prose are yours. |
| **Trademark** | *brands* — "Dungeons & Dragons", "Forgotten Realms", "Baldur's Gate" | Don't market under them. The game's own name **"Crown of Horns" is itself an FR artifact** — rename it. |
| **Game mechanics** | *not copyrightable at all* — and the **5.1 SRD is CC-BY-4.0** (2023) | Your d20/ability/skill-check engine is already free. Just don't paste SRD *text* verbatim without the CC credit. |

**Kept deliberately:** `tiefling` (it's in the CC SRD 5.1), the generic races (Human/Elf/Dwarf/Gnome/
Halfling/Half-Elf/Half-Orc), and everything original to this project (the Returned, the dead-touched
sense, the Wall as a *concept*, "Sundered Crown"). **Product-Identity monsters** (beholder, mind flayer,
githyanki, …): audited — **none are present in the content**, so nothing to rename there.

## How the swap works (and why it's safe)

`tools/reskin.js` applies `tools/reskin-map.json` as **whole-word, case-sensitive, longest-key-first**
replacements across every `play/*.json` **and** the generator (so `when.deity` gates and the character
BUILDS swap in lockstep — faith-reactivity survives intact). It writes to `tools/reskin-build/` and never
touches your source.

- **Whole-word (ASCII-letter boundaries):** `drow` → `duskling` but `drowned` is untouched; `Amn` → `Kesh`
  but `damn` is untouched.
- **Case-sensitive:** `Bane` (the god) swaps; `bane` (the word) is left. `Mask`/`Chosen`/`Reaper` never
  matched — they only appear here as common words.
- **Longest-first:** `Wall of the Faithless` resolves before `Faithless`; `Baldur's Gate` before `Baldur`.

Last run: **1,976 substitutions across 91 files, zero FR terms surviving, no common word corrupted,
structure intact.**

```
node tools/reskin.js                    # -> tools/reskin-build/ (gitignored)
node tools/reskin.js --check play/x.json  # audit one file for remaining FR terms
node tools/reskin.test.js               # the gate: proves the map is complete & safe
```

To produce a fully playable filed-off build: run the reskin, point the generator at
`tools/reskin-build/play` (or copy it over source on a branch), and regenerate `town_market.html`. Nothing
about the game changes but the nouns. *(Note: the FR-named test gates hardcode names like "Kelemvor" and
validate the canonical source; a reskinned build would want a reskinned gate, or just trust `reskin.test.js`.)*

## The map — original pantheon & world (all tunable)

Every value is a starting suggestion; edit `reskin-map.json` (the single source of truth) to taste.

### The spine: death, the Wall, the faith axis
| Forgotten Realms | Original | Note |
|---|---|---|
| Wall of the Faithless / the Faithless | **Wall of the Unclaimed / the Unclaimed** | The *concept* (souls no god claimed are walled) is a free premise; only the name changes — and "Unclaimed" is arguably sharper |
| Kelemvor (god of the dead, the Judge) | **Sarran** | |
| Doomguide (his clergy) | **Greywarden** | |
| Myrkul (prior death-god) / Lord of Bones | **Vhorruk / King of Bones** | |
| Jergal (first god of the dead, the clerk) | **Ossian** | |
| Crown of Horns (the artifact — and the game's title) | **the Hollow Crown** | ⚠️ rename the game too |

### The pantheon
| FR | Original | | FR | Original |
|---|---|---|---|---|
| Lathander / Morninglord | **Aubrin / Dawnfather** | | Bane | **Vorrus** |
| Mystra | **Aravelle** | | Bhaal | **Kaziss** |
| Oghma | **Halwen** | | Cyric | **Karth** |
| Tymora | **Riala** | | Tyr | **Valdren** |
| Ilmater | **Sethe** | | Chauntea | **Greanna** |
| Mielikki | **Yllowen** | | | |

### Geography & realms
| FR | Original | | FR | Original |
|---|---|---|---|---|
| Sigil / City of Doors | **Threnn / City of Thresholds** | | Waterdeep | **Tidehaven** |
| Candlekeep | **Tallowkeep** | | Neverwinter | **Winterwane** |
| Baldur's Gate | **Ardengate** | | Sword Coast | **Iron Coast** |
| Faerûn | **Aumenar** | | Amn | **Kesh** |
| Cormyr | **Corrath** | | Thay | **Zharim** |
| Underdark | **Underreach** | | Outlands | **Marrowlands** |

### History, planes & figures
| FR | Original | | FR | Original |
|---|---|---|---|---|
| Netheril | **Ophirid** | | Karsus | **Cassar** |
| Crown Wars | **Riven Wars** | | Spellplague | **Weavewrack** |
| Time of Troubles | **the Godsfall** | | Fugue (Plane) | **the Wending** |
| Great Wheel | **Endless Wheel** | | Elminster | **Vandemar** |
| Drizzt | **Vheskar** | | Volo | **Sabir** |
| Khelben | **Halduin** | | drow | **duskling** |
| Harpers | **Lanterns** | | Zhentarim | **Blackchain** |
| Flaming Fist | **Ember Fist** | | | |

## Bottom line

The reskin is **a day's worth of tuning names**, changes *nothing* about what makes the game land, and is
already proven to produce a clean, structurally-valid, faith-reactive build. If the licence comes through,
ship the canonical version; if it doesn't, run one command. Either way the writing, the systems, and the
code are yours.
