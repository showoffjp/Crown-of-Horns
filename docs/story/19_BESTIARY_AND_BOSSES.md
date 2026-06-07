# 🐉 Bestiary & Boss Design

> The enemies — lore *and* mechanics — designed to map directly onto our 5e-style combat engine
> (`AttackResolver`, `StatusEffect`, `AbilityRunner`, AoE, conditions). Each entry: the fiction, a
> stat block in our format, the *tactical identity* (what it does to the player's plan), and the
> *thematic* reason it exists. Bosses get multi-phase design and the staging notes that make them
> memorable, not just hard.

> **Combat design pillar:** every enemy should pose a *question*, not just a health bar. The Unbound
> ask "will you grant mercy or efficiency?"; the Echoes ask "can you kill what you love?"; the era
> bosses ask the act's thematic question *in mechanics.*

---

## I. The Unbound — the signature foe

Souls torn loose by a harvest and returned *wrong.* Not classic undead — they are people with the
*person* scraped thin, ranging from shambling husks to lucid, grieving wraiths who can *beg.* Every
Unbound was someone the Choir would call "saved."

> **The mercy mechanic (the line that defines the game's combat):** a reduced-to-0 Unbound isn't
> automatically destroyed. The player chooses **Lay** (a clean end — minor XP, +approval with Garrow,
> the soul passes) or **Shatter** (instant, more XP/loot, −approval, and the soul is *unmade* — fed to
> the Unmade). Combat itself asks the central question.

| Unbound | AC | HP | Key stats | Attack / ability | Tactical identity |
|---|---|---|---|---|---|
| **Husk** | 11 | 13 | STR 14, DEX 8 | *Grasp* 1d6 + applies **Slowed** | the wall; soaks, slows your advance |
| **Wailer** | 12 | 9 | CHA 14 | *Keening* (DEX save DC 12, AoE burst, applies **Frightened**) | disrupts formation; punishes clumping |
| **Lucid Wraith** | 14 | 22 | WIS 15 | *Grief-touch* 2d6 necrotic + heals self half | the beggar; can be *talked down* (CHA DC 16 → it lays itself) |
| **Hollow Child** *(rare)* | 10 | 6 | — | none — it only *weeps* | a non-combatant in the fight; killing it is a choice that scars |

> The **Hollow Child** is the bestiary's thesis: an enemy that doesn't fight, can't be "won," and
> exists only to ask whether you'll harm a helpless discarded thing for the XP. (No reward for
> shattering it; a hidden flag if you spare it every time.)

---

## II. The Living Opposition

- **Choir Cultist** (AC 13 / HP 16): true believers, *immune to Frightened* (they've paid the worst
  price already), fight without self-preservation — they'll take opportunity hits to reach an ally to
  *heal* them. Disturbing because they're sympathetic; some *surrender* and beg you to understand.
- **Doomguide Knight** (AC 16 / HP 30): lawful, disciplined, shield-wall tactics; *Lay the Unbound*
  reaction (they'll prioritize "ending" your summoned/Unbound allies). Not evil — they think you're a
  relic to be contained. Can be made to *stand down* with the right reputation.
- **Doomguide Confessor** (AC 14 / HP 22, caster): *Bless*, *Cure Wounds*, and *Sanctuary*; the
  priority kill that teaches focus-fire.
- **Flaming Fist Mercenary** (AC 15 / HP 20): crossbows + a net (*Restrained*); city-fight flavor,
  bribable/avoidable via reputation.

---

## III. Era Foes

- **Netherese War-Construct** (AC 17 / HP 45): arcane golem; **in eras where the Weave is wounded, it
  malfunctions** — a mechanic where each round it rolls on a wild-magic table (sometimes it attacks
  *itself*). Teaches the player that magic is *unreliable* in the broken eras.
- **Crown-War Revenant** (AC 15 / HP 28): the elven damned, ten-thousand-years dissolving; *Vengeful
  Memory* (advantage vs. anyone who's "wronged the dead" — i.e., anyone who's Shattered an Unbound this
  run). The Wall's bookkeeping, made into a debuff.
- **Avatar-Touched Horror** (AC 16 / HP 40): a Time-of-Troubles mortal warped by a dying god; rolls a
  *random damage type* each attack (the divine static). Resistance-juggling puzzle.
- **Spellplague Aberration** (AC 14 / HP 35): causality-optional — its attacks sometimes resolve
  *before* it declares them; periodically *swaps positions* with a random unit. The "fight where the
  rules bend" enemy; deeply unsettling by design.

---

## IV. The Echoes — the mirror fights *(late game)*

The Last Returned's signature horror: **twisted versions of your own party**, fought in reflection.
Each Echo uses a *corrupted* version of that companion's real kit — Garrow's *Lay* becomes *Unmake*;
Roen's inspiration becomes *despair*; Naeve's Fireball burns *cold*.

> **Design rule:** the Echoes must hurt *emotionally*, not just mechanically. An Echo speaks in its
> companion's voice — saying the thing that companion most fears is true about themselves. (Echo-Garrow:
> *"You always knew the rites were empty. Stop pretending. Come dig with me."*) Defeating an Echo is
> staged as a *grief*, not a triumph — and if that companion is **dead** (lost in the Breach), fighting
> their Echo is the cruelest fight in the game: the only time you see them again is to kill them again.

---

## V. THE BOSSES

> Each boss is multi-phase, and each phase *changes the question.* Mechanics first, then the staging
> that makes it land.

### 🜍 The Unbound Maw — *Act I / the Cinderhaunt boss* (the tutorial-boss)
A harvest-site grown into a single vast Unbound mouth in the floor. **Phase 1:** adds (Husks crawl
from the Maw each round — teaches AoE and prioritization). **Phase 2 (50%):** the Maw *inhales* —
pulls all units 2 tiles toward it each round (positioning pressure); standing adjacent applies
**Burning**. **The hook:** at 0 HP it *speaks*, in a chorus of the harvested — the player's first
hint that the "monsters" are *people*, and the first chance to **Lay** vs. **Shatter** at boss scale.

### 👑 Myrkul's Avatar — *Act IV / the forging* (the false-final-boss)
When the Crown-Voice finally drops its mask. **Phase 1:** a classic god-of-death fight — necrotic AoE,
summons, a *Crown of Bone* that must be sundered. **Phase 2 (the twist):** Myrkul reveals he was
*helping* you against the Unmade — and offers an alliance *mid-fight.* The player can **accept** (the
fight ends; Myrkul becomes a temporary, treacherous ally for Act V) or **refuse** (Phase 3: the full
dead-god). A boss that's also a *choice.* Staging: the moment he stops fighting and *reasons* with you
should be more unnerving than any attack.

### 🌌 The Unmade — *not a boss you beat*
The Unmade has no health bar. The "fight" with it is the **Convergence** — a defense/endurance
encounter where you hold the Court of the Dead against everything (Echoes, Choir, Doomguides) *while*
the real resolution happens in dialogue. **You cannot win by damage.** The encounter ends when you make
the *choice* (the ending), not when a bar empties. Mechanically radical, thematically essential: *you
cannot defeat grief; you can only answer it.*

### 🪞 The Last Returned — *the final confrontation* (the anti-boss)
The reflection-duel. **Phase 1:** it fights with *your own build* — same class, same abilities, scaled —
a true mirror match (it even uses items you have). **It cannot be reduced below 1 HP by damage.** **Phase
2:** at 1 HP it *kneels* and the fight becomes the dialogue from `10_DIALOGUE_SCRIPTS.md` §3. The
mechanical genius: the player *tries to win the normal way, and learns they can't*, exactly as the Last
Returned says — *"You can't beat me. I'm you, plus everything you're about to learn."* The win condition
is **lowering your weapon** (a non-combat action that only appears once you've recovered your true name).
Staging: HUD strips at the kneel; silence; the choice. The boss you defeat by *refusing to.*

---

## VI. Boss-Design Principles (the house rules)

1. **Every phase changes the question.** Not "more HP, more damage" — *new mechanic, new dilemma.*
2. **The best bosses are also choices.** The Maw (lay/shatter), Myrkul (ally/refuse), the Last Returned
   (fight/lower the weapon). Damage is one verb; the boss should demand others.
3. **Some bosses can't be beaten with violence — by design.** The Unmade and the Last Returned both
   refuse the health-bar paradigm, teaching the game's deepest lesson *through the controller.*
4. **Stage the turn, not just the stats.** Strip the HUD, cut the music, hold the camera — the
   *audiovisual restraint* (per `13` §IV and `18`) is what turns a hard fight into a remembered one.
5. **Mercy is mechanized.** Lay vs. Shatter runs through the whole bestiary, so the central moral
   question is something the player's *hands* answer, hundreds of times, long before the finale asks it
   with words.

> **Mantra:** *a great RPG enemy is a question with a health bar; a great RPG boss is a question you
> answer with a choice; and the greatest is the one you answer by putting the weapon down.*
