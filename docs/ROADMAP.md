<div align="center">

# 🗺️ THE CROWN OF HORNS — Development Plan & Roadmap

**Status:** v3.60.0 · 135 C# scripts · ~19.7k LOC · all clean · 71 Codex · 33 deeds · 34 banters · 13 night-talks · 11 keepsakes · 14 items · full economy/gear & onboarding loops
**Legend:** ✅ done · 🚧 in progress · 📋 planned · 💡 stretch

</div>

> A *living* plan. The golden rule still holds: **prove the whole machine on a small scope, then scale.**
> We are well past the "does the machine work" question — the vertical slice is deep and reactive. The
> work ahead is (1) make it *look & sound* like a game, (2) deepen combat, (3) widen the narrative, and
> (4) harden for production. This file is the source of truth for "what's done / what's next."

---

## 0 · Vision (unchanged north star)
A classic isometric, party-based CRPG in the lineage of **Baldur's Gate II**, on **D&D 5e (SRD)**, set in
the Forgotten Realms. You died and were **Returned**; the crack in your soul is a crack in *time*. The saga
walks Realms history (Netheril → Crown Wars → Time of Troubles → Spellplague) chasing four nested villains
behind one question: **what is a soul worth, that no god ever claimed?** *Code is the engine; data is the game.*

---

## 1 · ✅ DONE — the foundation (v1.0 → v1.56)
Condensed; see `CHANGELOG.md` for the blow-by-blow.

**Engine & loop** — ✅ iso grid + A* · ✅ turn-based 5e combat (initiative, action economy, saves,
AoE, conditions, DoT, adv/disadv, crits, spell slots, enemy AI) · ✅ exploration/hub director ·
✅ data-driven dialogue/quests/flags · ✅ save/load **+ multi-slot manager** · ✅ character creation · ✅ XP/leveling.

**Companions (the pillar)** — ✅ recruit → approval → **two night-talks each** → **11 group-banters** →
**5 personal quests** → **4 romance arcs** (6-stage) → **rupture/betrayal arcs** → **keepsakes (10)** →
epilogue payoffs. ✅ Every companion has a reactive "home" beat across the hub & all four eras.

**Act II — a place** — ✅ reactive Lower City hub (Tamsin the crier, reactive proclamation board) ·
✅ **5 side quests** (Widow, Fist, Faithless Choir, Tithe Collector, Last Letter) · ✅ a second explorable
room (the Almshouse) · ✅ a reputation-gated **fence/shop** · ✅ a Choir **miniboss** · ✅ a reactive **Fugue**
beat · ✅ `reputation.lowcity` economy wired into shop, board, ending.

**Act III — the eras** — ✅ all four eras playable with unique gimmicks (falling city, the verdict,
the Mirror/Echoes, the Crown reveal, the Spellplague) · ✅ a **companion-witness beat** per era ·
✅ an **optional miniboss** per era.

**The Breach & endings** — ✅ permanent loss with authored victim selection · ✅ **romance ↔ Breach**
stakes + finale **anchor** · ✅ the Vault of Tens (riddles) · ✅ **6 endings** with dozens of flag-keyed
epilogue slides + a Keepsakes slide.

**UI/UX & meta** — ✅ Journey (J), Party (P), **Relationships (L)**, Codex (K, **37 entries**), Options (O,
difficulty/banter/volume/text-speed/**UI-scale**/**combat-speed**), Help (H), **Chronicle (C)** (run summary +
keepsakes + **deeds (17)** + bestiary tally + **cross-run legacy**), typewriter dialogue, **nameplates +
floating combat text**, autosave-on-rest, **credits**, **New-Game+ memory**, **party-wipe recovery screen**.

**Art/audio seams (drop-in, art-optional)** — ✅ unit sprites + iso depth sort · ✅ dialogue portraits ·
✅ combat VFX · ✅ SFX + per-mode music · ✅ tiled floor. **Drop a folder of PNGs/clips, zero code.**

**Tooling** — ✅ `ContentValidator` (dialogue-link **and** data-integrity checks) + `ValidationDemo` ·
✅ ~18 one-click demos · ✅ difficulty (Story/Normal/Hard) scaling damage **and** HP.

---

## 2 · 🎯 NEAR-TERM QUEUE (the next ~5 increments)
Highest value, lowest risk first. Each is one self-contained version.

1. 📋 **Art drop #1** — once a few sprites/portraits land in `Resources/`, verify the skinner/portrait
   paths in-editor and capture real screenshots. *(Needs your PNGs; see `docs/ASSET_INTEGRATION.md`.)*
2. ✅ **Combat: the Dodge/Defend action** *(v1.65.0)* — spend your action to Defend (key **G** or the HUD
   button); attacks against you are made at disadvantage until your next turn. `CharacterSheet.IsDodging`,
   cleared at the dodger's next turn start; `TurnManager.TryDodge()`; factored into `AttackResolver`.
3. ✅ **Death's-door / downed clarity** *(v1.66.0)* — initiative now reads *(downed)* for friendlies vs
   *(slain)* for foes, the HUD shows **Victory/Defeat** correctly, and a total-party wipe raises a
   **`DefeatScreen`** (load last save / return to title) instead of stranding the player. Downed
   companions still stabilize after victory (permanent loss stays reserved for the Breach).
4. ✅ **A second hub area** *(v1.59.0)* — the Shrunken Quarter market (`MarketScene`), a second explorable
   Lower City room with reputation-reactive NPCs and a way back.
5. ✅ **Relationships screen** *(v1.58.0)* — a dedicated panel (key **L**) showing each companion's approval
   bar, romance stage, rupture state, and quest beat at a glance.

---

## 3 · 📋 PHASE A — "Make it a game to *look at*"
The seams exist; this is asset + presentation work.
- 📋 Drop the **pixel-FX pack** into `Resources/FX/*` (fire/ice/holy/impact/heal).
- 📋 **Unit sprites** for the cast + enemy archetypes (`Resources/Sprites/<Name>.png`); idle/walk/attack
  clips (extend `UnitSpriteSkinner` → a small frame-animator).
- 📋 **Portraits** for every speaker (`Resources/Portraits/*`).
- 📋 **Hand-built isometric Tilemaps** for the hub & key scenes (replace code-built grids; `📋` in
  `ASSET_INTEGRATION.md` §3) — keep `GridSystem.walkable` driven from a collision layer.
- 📋 **uGUI HUD skin pass** — replace IMGUI panels with a styled Canvas (portraits, action bar, turn
  order, combat log) once art exists. Keep the IMGUI fallbacks.
- 📋 **Music & SFX** drop (`Resources/Music/*`, `Resources/SFX/*`) — hooks already fire.
- 💡 Lighting/atmosphere (URP 2D lights), screen-shake on crits, hit-pause.

## 4 · 📋 PHASE B — Combat depth
- 🚧 **Reactions** — ✅ opportunity attacks *(v1.74)* (leave an adversary's melee reach without Disengaging
  → it spends its reaction to strike, one per round); 📋 readied actions, a reaction-prompt UI.
- 🚧 **Action variety** — ✅ Dodge *(v1.65)* · ✅ Dash *(v1.67)* · ✅ Help *(v1.71)* · ✅ Disengage *(v1.75)*; ✅ Shove *(v1.87)* · 📋 Grapple; ✅ **flanking** *(v1.76)* · ✅ **cover** *(v2.51)*.
- 📋 **Death saves** proper (currently knocked-out-not-dead; add the 3-strike rule option on Hard).
- 📋 **Smarter enemy AI** — ✅ focus-fire · ✅ AoE-aware bursts · ✅ self-preservation Dodge · ✅ ranged **kiting** *(v2.84)*; 📋 use
  of consumables, per-archetype tactics.
- 🚧 **Encounter budgets & a balance harness** — ✅ a seeded duel **balance canary** *(v2.33)*
  (`CombatBalance` + `CombatBalanceDemo`, win-rate verdict through the real resolver); 📋 full scripted
  encounters & per-difficulty assertions.
- 🚧 **More abilities/spells** — ✅ per-level class kits *(v1.78)*: each class now unlocks new abilities as
  it levels (Second Wind, Ray of Frost, Sacred Flame, Guiding Bolt, Healing Word, Thunderwave, Javelin…);
  ✅ deeper kits with high-level spells *(v3.27)* (Hold Person, Spiritual
  Weapon, Ice Storm, Cone of Cold, Flame Strike, Cure Wounds III); 📋 reaction spells; concentration.
- 💡 **Feats / multiclass**, level-up choice UI, subclasses.

## 5 · 📋 PHASE C — Act II & world breadth
- 🚧 **Multi-room Lower City** — ✅ Almshouse · ✅ Shrunken Quarter market *(v1.59)* · ✅ Chionthar Docks
  *(v1.81)* · ✅ Harper safehouse *(v1.95, Roen-gated)*.
- 🚧 **Faction reputations beyond the poor** — ✅ Kelemvor / Choir / Ash Pact standings now surface in the
  Journey (J) *(v1.89)* · ✅ a Kelemvor Doomguide in the hub now **reacts to your church standing** *(v3.22)*;
  📋 Flaming Fist & Harpers, deeper gating.
- 🚧 **More side quests** — ✅ the Docks ferryman moral micro-beat *(v2.90, a real choice + rep stakes)*;
  📋 more, incl. a couple with real combat.
- 📋 **Expand the Fugue** into a small explorable act-beat (it's currently one scene).
- 🚧 **Merchants/economy** — ✅ sell loot at the fence (rep-scaled rate) *(v1.94)* · ✅ a second vendor
  (the Docks smuggler) *(v1.97)*; 📋 prices that shift with supply.
- 💡 Stealth / traps / locks / world skill-checks (not just dialogue checks).

## 6 · 📋 PHASE D — Act III/IV main-plot scenes
- 📋 **The Four Masks** as authored confrontations (Aldric, the Unmade, the Last Returned, the Lady) —
  set-piece dialogues + fights, beyond the current reveals.
- 📋 **The Mirror** deepened (Echoes that mimic *your* current build/abilities).
- 📋 **The Court of the Dead** branching expanded — choices within the finale, not just the six picks.
- 📋 **Twist architecture** wired end-to-end (`docs/story/13`): foreshadow → reveal → recontextualize.
- 💡 A "Ninth"/secret companion or ending (story bible hints).

## 7 · 📋 PHASE E — Companion depth (finish the cast)
- 📋 **Approval-gated dialogue in the world** (not just camp) — companions chime into era/hub scenes.
- 📋 **Romance "Last Night" + golden-ending scenes** rendered as full set-pieces.
- 📋 **Deeper rupture arcs** — a chance to win them back mid-quarrel; a lost companion as a *future enemy*.
- 📋 **Maerin** content beat (she joins at the Breach; give her a quest/voice).
- 💡 Cross-companion relationships (two companions romancing each other if you don't).

## 8 · 📋 PHASE F — Systems & meta
- 📋 **Relationships/Journal screens** (see near-term #5).
- 📋 **New Game+** — carry difficulty/deeds/cosmetic title; remember past endings.
- 📋 **Crafting** (optional) + consumable variety.
- 📋 **Codex auto-unlock on first sighting** (bestiary entries when first fought, not just on boss-kill).
- 📋 **Settings**: keybind remap, text size, colorblind-safe combat states, combat speed slider.

## 9 · 📋 PHASE G — Production / ship
- 📋 **Localization** — externalize all strings (do before the string count explodes!).
- 📋 **Accessibility** — font scale, colorblind palettes, remappable input, combat-speed, reduce-motion.
- 📋 **Performance** — object pooling (VFX/floating text/nameplates), A* heap, chunked/tilemapped scenes.
- 📋 **Save-migration safety** (version the `SaveData`, migrate on load).
- 📋 **Build pipeline** — itch/Steam page, CI build, demo build.
- 📋 **QA pass** + a real balance pass per difficulty.

---

## 🔒 Anti-scope-creep rules (still law)
1. **No new system without shipping the last one** — each version is validated + pushed to `main`.
2. **Reactivity > breadth** — a small world that *remembers* beats a big one that doesn't. (This is our edge.)
3. **Art is drop-in** — never block logic on assets; every art feature degrades gracefully to cubes/IMGUI.
4. **Validate before commit** — braces + namespaces + `ContentValidator`; the sandbox has no compiler.
5. **Cut toward the Prologue+Act I+the eras slice**; the full Acts wait their turn.

## 🧭 Where things live (for contributors / future sessions)
- Read `CONTINUITY.md` first (the handoff brief). `FEATURES.md` = what exists; `CHANGELOG.md` = how it got here.
- **Engine:** `Assets/Scripts/{Grid,Combat,Characters,Dialogue,Quests,Save,Items,Rendering,FX,UI,World}`.
- **Content (the game):** `Assets/Scripts/Content/*` — quests, romances, ruptures, banter, eras, codex,
  keepsakes, deeds, shop. **New content is data, not boilerplate.**
- **Director:** `Assets/Scripts/Core/CampaignBootstrap.cs` swaps modes; flags live in `Core/GameFlags.cs`.
- **Story bible:** `docs/story/` (24 docs). **Assets:** `docs/ASSET_INTEGRATION.md`.
