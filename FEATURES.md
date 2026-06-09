<div align="center">

# ⚔️ THE CROWN OF HORNS — Feature Codex 📜

### *A classic isometric, party-based CRPG for the Forgotten Realms · built on D&D 5e*

![Status](https://img.shields.io/badge/Build-Vertical_Slice-brightgreen)
![Engine](https://img.shields.io/badge/Engine-Unity_6_LTS-black?logo=unity)
![Lang](https://img.shields.io/badge/C%23-10-239120?logo=csharp)
![Ruleset](https://img.shields.io/badge/Rules-D%26D_5e_(SRD_5.1)-b51e2e?logo=dungeonsanddragons)
![Combat](https://img.shields.io/badge/Combat-Turn--Based-blue)
![Scripts](https://img.shields.io/badge/Scripts-135_files_·_19.4k_LOC-informational)

</div>

---

> **Legend:** ✅ Implemented & playable · 🚧 In progress · 📋 Planned next · 💡 Stretch / future

---

## 🟢 Implemented — Playable Today

### 🗺️ World & Movement
| | Feature | Notes |
|---|---|---|
| ✅ | **Isometric grid** | Classic 2:1 diamond projection, world↔grid conversion, editor gizmos |
| ✅ | **A\* pathfinding** | Respects walkability, difficult-terrain cost, occupied tiles |
| ✅ | **Click-to-move** | Tile pathing that spends a turn's movement budget |
| ✅ | **Isometric camera** | Pan (WASD/edge-scroll), zoom, bounds clamping |
| ✅ | **Smooth unit movement** | Tile-by-tile traversal with depth sorting |

### 🧭 Exploration & World Loop
| | Feature | Notes |
|---|---|---|
| ✅ | **Real-time exploration mode** | Click-to-move the party leader around a hub (A* pathing, no turn budget) |
| ✅ | **Interactables** | NPCs (talk), exits/doors (transition), points of interest (examine) |
| ✅ | **Baldur's Gate hub** | Walkable Lower City slice with the Herald, a proclamation board, and the Cinderhaunt stairs |
| ✅ | **Hub ⇄ dungeon transitions** | A director swaps modes; doors load tactical encounters and return you to town |
| ✅ | **Reusable `EncounterBuilder`** | Any door/trigger spawns a full turn-based fight from a roster + callback |
| ✅ | **Quest journal HUD** | Active quests + objectives tick live from GameFlags while you explore |
| ✅ | **Full campaign loop** | `CampaignBootstrap`: create → explore → quest → fight → XP → return |
| ✅ | **Multi-room dungeon** | The Cinderhaunt: two chambers split by a **locked door** (needs the key), loot, a guard fight + a **boss**, all flag-tracked so progress persists across explore↔combat |
| ✅ | **Party fights together** | Your whole *active* party — hero + Garrow + recruits — deploys into dungeon battles |
| ✅ | **In-world companion recruitment** | Talk to **Roen the Harper** in the hub; a dialogue flag adds him to the roster (and into fights) |
| ✅ | **Persistent containers** | Looted chests stay empty across dungeon rebuilds via `lootFlag` |

### 🕰️ Time-Travel Eras
| | Feature | Notes |
|---|---|---|
| ✅ | **Netheril — the falling city** | Era 1 (Karsus's Folly, −339 DR): an explorable golden enclave plummeting from the sky, reached via the **Skip rift** in the hub after the Cinderhaunt |
| ✅ | **Collapsing-floor hazard** | `FallingHazard`: mid-battle the floor caves in from the edges inward — voided tiles block movement and deal falling damage; never soft-locks |
| ✅ | **Time-displaced companion (Naeve)** | Recruit the Netherese arcanist from the rubble via dialogue; she joins the roster and fights |
| ✅ | **Modular era transitions** | `NetherilEra` + director mode-swap prove the *era-module* structure the whole campaign is built on |
| ✅ | **Optional miniboss — The Mythallar Colossus** | A war-construct guarding the falling enclave's core (in the collapsing floor). Optional; clearing it (`netheril.boss_down`) nudges Naeve approval and earns a Codex + ending slide |
| ✅ | **Crown Wars — the first damnation** | Era 2 (~−10,000 DR): an elven high court; recruit **Ilfaeril**, and **argue a soul-damnation *down*** — a non-combat **moral-hazard** scene proving the Wall was a *decision* |
| ✅ | **The Echoes / the Mirror fight** | Fight twisted **clones of your own party** + **the Last Returned**, who *kneels* once its Echoes fall rather than dying by damage (`MirrorResolver`) |
| ✅ | **Optional miniboss — The First Unmade** | The very soul the court voted to erase, risen in grief. Optional; clearing it (`crownwars.boss_down`) nudges Ilfaeril approval and earns a Codex + ending slide. **All four eras now offer an optional miniboss** |
| ✅ | **Time of Troubles** | Era 3 (1358 DR): witness the **Crown of Horns forged from Myrkul's skull** — the Crown-is-Myrkul reveal, in-engine |
| ✅ | **Optional miniboss — The Avatar of Bone** | A god-touched horror at the forge (`SimpleEra.bonusFightId`): a named boss + God-Touched Horrors + a Bone-Zealot. Optional; clearing it (`toot.avatar_down`) nudges Garrow approval and earns a Codex entry + an ending slide |
| ✅ | **Spellplague — causality-optional combat** | Era 4 (1385 DR): the `SpellplagueHazard` — reality *skips* (units swap places) and **blue fire** lashes a random combatant each turn |
| ✅ | **Optional miniboss — The Herald of the Unmade** | A second, tougher combat marker in the Spellplague (`SimpleEra.bonusFightId`): a named boss + Unmade aberrations + a Sorrow, in the causality-optional blue fire. Optional; clearing it (`spellplague.herald_down`) nudges Naeve/Ilfaeril approval and earns a **Codex bestiary entry** + an ending slide |
| ✅ | **All four eras playable** | Netheril ⇄ Crown Wars ⇄ Time of Troubles ⇄ Spellplague — each a flag-gated rift in the hub, built on the reusable `SimpleEra` |
| ✅ | **One-click era demos** | `NetherilDemo` · `MirrorEncounterDemo` · `SpellplagueDemo` |

### 🎒 Items, Loot & Equipment
| | Feature | Notes |
|---|---|---|
| ✅ | **Item database** | Id → `ItemDefinition` registry feeding inventory & UI |
| ✅ | **Equipment system** | Equip weapons/armor; gear drives AC and the basic attack |
| ✅ | **Inventory & equipment UI** | Press **I**: backpack, gold, per-member slots, equip/use |
| ✅ | **Consumables** | Potions etc. apply an ability effect (e.g. healing) on use |
| ✅ | **Loot drops** | Enemies drop gold + items, auto-awarded to the party on victory |
| ✅ | **Lootable containers** | Strongboxes/chests transfer contents to the party stash |
| ✅ | **Lower City fence (shop)** | `ShopContent` + `ShopScreen`: a hub vendor (Sczerla's Sundries) that buys against party gold — and **widens its stock as your `reputation.lowcity` rises** (rep-locked goods shown 🔒). A gold sink that rewards doing right by the quarter |

### 💥 VFX & Asset Pipeline
| | Feature | Notes |
|---|---|---|
| ✅ | **Combat VFX system** | `FxSystem` plays sprite-frame effects on hits/spells |
| ✅ | **Auto-loaded effects** | Drop frames in `Resources/FX/<effect>/` — no wiring needed |
| ✅ | **Damage-typed impacts** | Auto-picks fire/ice/holy/… effect by an ability's damage type |
| ✅ | **Pixel FX pack ready** | Plugs in the supplied Effect & FX pack (see `docs/ASSET_INTEGRATION.md`) |
| ✅ | **Tiled isometric floor** | `TileFloorRenderer` paints a checker-tinted diamond floor under every scene — no art required |
| ✅ | **Sprite-ready units** | `WorldArt` + `GridUnit` auto-swap: drop `Resources/Sprites/<Name>.png` and that unit shows a sprite billboard instead of a cube — no wiring |
| 📋 | **Isometric tilemaps** | Hand-painted levels via Unity's 2D Tilemap |

### ⚔️ Combat (D&D 5e core)
| | Feature | Notes |
|---|---|---|
| ✅ | **Turn-based initiative** | d20 + DEX, cached roll, clean ordering |
| ✅ | **Action economy** | Move · Action · Bonus Action per turn |
| ✅ | **Attack rolls vs AC** | d20 + ability mod + proficiency, nat-1 miss / nat-20 crit |
| ✅ | **Saving throws** | Spell save DC = 8 + prof + casting mod |
| ✅ | **Save-for-half** | Fireball-style partial damage |
| ✅ | **Advantage / disadvantage** | Derived from conditions, cancel per 5e |
| ✅ | **Critical hits** | Double the damage dice |
| ✅ | **Spell slots** | Per-level pools, spent by leveled spells, refreshed on rest |
| ✅ | **Healing abilities** | Cure Wounds & friends |
| ✅ | **Area-of-effect** | Burst targeting around a tile (Fireball) |
| ✅ | **Status effects / conditions** | Poisoned, Prone, Frightened, Burning (DoT), Blessed… |
| ✅ | **Damage-over-time** | Ticked at start of turn |
| ✅ | **Incapacitation** | Stun/incapacitate skips the turn |
| ✅ | **`AbilityRunner`** | One validated path: range, slots, economy, AoE, logging |
| ✅ | **Tactical actions** | **Dodge** (G), **Dash** (F), **Help** (T), **Disengage** (X), **Shove** (V), **Quaff** (Q) — full action-economy options, hotkeys + HUD buttons |
| ✅ | **Opportunity attacks** | Leave an adversary's melee reach without Disengaging → it spends its reaction to strike (one/round) |
| ✅ | **Flanking & cover** | Melee advantage when an ally stands opposite the target; **half-cover** (+2 AC) for a ranged attack through an interposing body |
| ✅ | **Round tracking** | `TurnManager.RoundNumber`, shown in the HUD; transient stance reset each encounter |
| ✅ | **Enemy AI** | **Focus-fire** · **AoE-cluster** bursts · **self-preservation** Dodge when low · ranged **kiting** out of melee; bosses spend real spell slots |
| ✅ | **Per-level class kits** | All six classes unlock abilities as they level — to high-level spells (Hold Person, Spiritual Weapon, Ice Storm, Cone of Cold, Flame Strike) & martial strikes (Heavy/Brutal/Sneak/Hunter's Volley) |
| ✅ | **Balance canary** | `CombatBalance`: seeded duels through the real resolver report a win-rate verdict (a tripwire for combat-math regressions) |
| ✅ | **Bloodied & camera-follow** | A *BLOODIED* read at half-HP; the camera glides to the active combatant (toggleable) |
| ✅ | **Party-wipe recovery** | Total defeat → a Defeat screen (load last save / return to title), not a soft-lock |
| ✅ | **13 damage types** | Slashing → Psychic → Radiant → Force… |

### 🧙 Characters & Progression
| | Feature | Notes |
|---|---|---|
| ✅ | **Six ability scores** | STR/DEX/CON/INT/WIS/CHA with 5e modifiers |
| ✅ | **Classes (data-driven)** | Hit die, primary ability, spellcasting, granted abilities |
| ✅ | **Ancestries / races** | Speed + ability bonuses (Human, Elf, Dwarf, Tiefling…) |
| ✅ | **Backgrounds** | Skills, feature, **flag that unlocks reactive dialogue** |
| ✅ | **Character creation** | Point-buy (27) **or** standard array, full race/class/bg flow |
| ✅ | **XP & leveling** | Full 5e XP table, HP/proficiency growth, level-up events |
| ✅ | **Party & shared inventory** | Roster, active party (4–6), gold |
| ✅ | **Equipment hooks** | Equipped weapon ability + armor AC |

### 💬 Narrative Systems
| | Feature | Notes |
|---|---|---|
| ✅ | **Branching dialogue** | Graph of nodes/choices authored as assets |
| ✅ | **Dialogue skill checks** | d20 + ability vs DC, success/fail branches |
| ✅ | **Conditions & effects on choices** | Gate/hide choices; set flags; shift approval |
| ✅ | **GameFlags story brain** | One flag store driving *everything* reactive |
| ✅ | **Companion approval** | Per-companion axis, clamped, event-driven |
| ✅ | **Faction reputation** | Numeric rep (Lower City poor, **Kelemvor's Doomguides**, the **Faithless Choir**) — a full loop each: content shifts it → **reactive hub figures** & NPC lines respond by tier → surfaced in the **Journey** standings → **deeds** → **epilogue slides** → gates endings (the Doomguide's Peace needs Kelemvor ≥ 5) |
| ✅ | **Quests** | Objectives auto-complete by watching flags; XP/gold rewards |
| ✅ | **Quest journal events** | Started / objective / completed hooks |
| ✅ | **Marquee scene authored** | **"Tea with a Heretic"** — Aldric's full branching conversation, in-engine (he comes to the hub after the Cinderhaunt) |
| ✅ | **Moral-hazard scenes** | The Crown Wars **Verdict** — argue a soul-damnation down via skill checks; shifts flags & companion approval |
| ✅ | **The Lady in the Margins** | A chaotic-neutral fifth-dimensional **riddler** (G-Man/Gaunter-tier) — meet **Tally** in the hub; her **Vault of Tens** riddle room with token-placing pedestals, spoken-answer braziers, a **wit-beats-correctness** amusement system, the secret eleventh riddle (her identity), and **milestone rewards** (the Coin of the Tenth Guest, Her Favour, the Reader's Boon) |
| ✅ | **Permanent loss — "The Breach"** | Pull **Maerin** from the Wall in the **Fugue Plane** and the Wall takes a companion **forever** (`Party.Remove`) — *who* is chosen by an **authored-fate** system (your earlier care decides: Varra by default, else a volunteer, else lowest-bond) |
| ✅ | **Recruitable companions** | Garrow (start), **Roen** & **Varra** (hub), **Naeve** (Netheril), **Ilfaeril** (Crown Wars), **Maerin** (the Fugue) — all join the roster and fight |
| ✅ | **Six branching endings, in code** | `EndingResolver` reads your whole playthrough (eras walked, companions lost, the Verdict, the Lady's riddle, Aldric's fate) → the **Court of the Dead** finale offers the endings you've *earned* (deeper truths unlock the golden roads), each with prose + **BG2-style epilogue slides** |

### 🖥️ UI & Game Flow
| | Feature | Notes |
|---|---|---|
| ✅ | **Real uGUI combat HUD** | Portraits + live HP bars, initiative, action economy, **clickable ability bar**, log, End Turn — built at runtime, zero setup |
| ✅ | **Dialogue screen** | Speaker, body, choices, live skill-check results |
| ✅ | **Character-creation screen** | Point-buy stepper, race/class/bg pickers |
| ✅ | **Main menu / front end** | `MainMenu`: **New Game / Continue / Load Game / Settings / Credits / Quit** — the shippable boot screen |
| ✅ | **Save-slot manager** | `SaveSlotScreen` + `SaveSlots`: multiple saves (Autosave + 3 slots), each previewing **hero · level · scene · timestamp** (`SaveSystem.Peek`), with **Load / New Game / Delete** (delete is confirm-guarded). The active slot drives the director's autosave & Continue (`SaveSystem.Delete`) |
| ✅ | **Options / settings** | `GameSettings` + `SettingsScreen`: **difficulty (Story / Normal / Hard), ambient-banter toggle, master volume, dialogue text-speed** (or instant), **UI text-size**, **combat speed**, and a **floating-text** accessibility toggle — persisted via `PlayerPrefs`, applied live. Off the menu **and** in-game with **O**. Also surfaces the cross-run legacy with an opt-in reset |
| ✅ | **Difficulty modes** | Story / Normal / Hard scale combat damage at the one chokepoint (`AbilityRunner`), so it applies **everywhere** — campaign, eras, and demos: **Story** softens foes & buffs you, **Hard** sharpens foes & nerfs you a touch. Persists across sessions |
| ✅ | **Help / controls overlay** | `HelpOverlay`: press **H** anytime, in any mode, for a full controls + hotkey-screens reference card (persistent, drop-in) |
| ✅ | **The Chronicle of the Returned** | `ChronicleScreen` (press **C**) + `EndingResolver.Chronicle()`: a running tally of the whole playthrough — eras walked, who's still with you / lost / departed, quests resolved, hearts given, bonds broken, Lower City standing, the Lady's riddles, endings unlocked, difficulty. Also recaps **at the ending screen** |
| ✅ | **Deeds (achievements)** | `Deeds`: 13 milestones derived purely from existing flags (no new hooks) — Walker of Ages, Four Foes for Four Ages, Five Threads Pulled, Beloved, The Golden Road… shown as a **🏆 Deeds (N/total)** section in the Chronicle with earned ★ / locked ☆ |
| ✅ | **Keepsakes** | `Keepsakes`: when a companion's quest resolves they press a memento on you — Garrow's whetstone, Roen's lockpick, Varra's charred contract-corner, Naeve's first proof, Ilfaeril's renounced signet, the unclaimed's backwards token — plus a romance tier (Garrow's other list, an inn key from Roen, Naeve's revised axiom, the ash of Varra's receipt) gated on a consummated romance — all flag-gated, collected in a **🎒 Keepsakes** section of the Chronicle (story made tangible, not stats) |
| ✅ | **Autosave on long rest** | A long rest at camp is a natural checkpoint — the director autosaves there (`CampScene.onRested` → `Autosave`), shown with a "progress saved" note |
| ✅ | **Typewriter dialogue** | `DialogueScreen` now reveals lines at the chosen chars/sec (one click finishes the line); honors the Instant-text setting |
| ✅ | **Save & Continue** | Autosaves on every return to the hub; **Continue** rebuilds your **hero (build/level/stats)** and **roster** (recruits/losses) from the save. Quick-save F5/F9 |
| ✅ | **Journey / quest tracker** | Press **J**: the whole saga as a live ✔/○ checklist read from `GameFlags` — eras, the Breach, the Vault — plus **endings unlocked N/6** and a **★ golden road** flag. Now shows each **companion quest's current beat** (thread opened → at the scene → fought → *resolved-how*) and a **Bonds** section tracking each romance's furthest stage |
| ✅ | **Roster management UI** | Press **P**: **bench/field** any recruited companion up to `maxActive`; the protagonist is locked in. Levels & gear preserved on the bench |
| ✅ | **Companion sheets** | Click a name in the Party screen: full **ability block** (STR–CHA + mods), **AC/HP/init/prof**, and an **approval meter** (devoted → hostile) from `companion.<id>.approval` |
| ✅ | **Codex / bestiary** | Press **K**: a compendium that **fills in as you witness the saga** — the Four Masks, bestiary, lore — gated by `GameFlags`, with category rail + reading pane (`CodexContent`) and an X/Y discovered counter. **~61 entries** across Premise, the Four Masks, the Company (incl. post-quest & romance reflections), Bestiary (era foes & minibosses), and Lore (the metaphysics, the eras, the Lower City rooms, Jergal, Aldric's daughter) |
| ✅ | **Ambient party banter** | Idle road chatter filtered to **who's in the field** (`PartyBanter`/`AmbientBanter`); fades in low in the frame, mute with **B** |
| ✅ | **Camp & long rest** | A campfire mode (`CampScene`): **long rest** restores party HP + all spell slots |
| ✅ | **Approval-gated night-talks** | Campfire monologues (`CampContent`) that unlock once a **fielded** companion's **approval** crosses a threshold — Roen, Varra, Garrow, Naeve, Ilfaeril, **and Maerin** — each with a deeper **second** talk, plus a **New-Game+** déjà-vu beat (13 in all) |
| ✅ | **Party cross-talk (group banter)** | `CampGroupBanter`: BG2-style two-companion exchanges at the fire — *Naeve & Garrow* (knowledge vs faith), *Roen & Varra* (two ways out of a bad deal), *Ilfaeril & Garrow* (servants of the same cruel law), *Varra & Naeve* (a contract and a catastrophe), *Roen & Garrow* (the liar and the woman who can't), *Ilfaeril & Naeve* (two who ended worlds), *Varra & Garrow* (the priced soul & the gravedigger), plus a **romance-reactive** ribbing — surfaced under **"The party falls to talking"** only when *both* speakers are fielded; plays once, warms both bonds |
| ✅ | **Give a gift (camp)** | Spend a **consumable** from the party stash on a fielded companion at the fire to warm the bond (+approval) — `CampScene` resolves giftables via `ItemDatabase`; ties the loot economy to the affinity system |
| ✅ | **Approval shifts in play** | Recruit/dialogue choices grant `companion.<id>.approval` so the meter moves in normal play, feeding the camp loop |
| ✅ | **Companion personal-quest hooks** | After a night-talk, an Act II quest pointer appears in the hub (`CompanionQuests`); examining it starts the thread (`quest.<id>.started`) — surfaced in the Journey tracker |
| ✅ | **Reusable quest engine** | `PersonalQuestScene` + `PersonalQuest` config run any companion arc (arrive → examine → confront → fight → moral call → leave) from data — each new quest is mostly content |
| ✅ | **Roen's personal quest — "The Honest Lie"** | Silent safehouse, the Wrenna reveal, a Doomguide-cell fight, and a **moral trilemma** (forgive/condemn/Persuasion gambit) that swings approval and **echoes in the epilogue** |
| ✅ | **Varra's personal quest — "The Bill Comes Due"** | A deconsecrated chapel, the cambion broker Quill, a devil fight, and a **trilemma** (take her debt / free her / Arcana loophole that binds her patron) — also epilogue-aware |
| ✅ | **Garrow's personal quest — "A God-Shaped Hole"** | A Kelemvorite heresy tribunal, Justiciar Veld who taught her catechism, a templar-inquisitor fight, and a **trilemma** (recant to keep the grey / leave the faith entirely / a [Religion] gambit that puts Kelemvor's own doctrine on trial) — epilogue-aware |
| ✅ | **Naeve's personal quest — "After the Sky Fell"** | A surviving fragment of Netheril still falling a thousand years on, the echo of the Steward Vaelin, a mythallar-ward fight, and a **trilemma** (freeze the shard as a tomb / let it finish falling and carry the grief / an [Arcana] gambit that gives its last live Weave back to the present) — epilogue-aware |
| ✅ | **Ilfaeril's personal quest — "The Vote"** | An elven reliquary keyed to a soul his Crown-War council voted to unmake, the Pale Cantor of the Choir who'd melt his guilt into a key, a Choir fight, and a **trilemma** (refuse forgiveness and keep paying / accept it / an [Insight] gambit that reframes her forgiveness as a commission to free the rest) — epilogue-aware. **Completes all five companion quests.** |
| ✅ | **Romance arcs — the slow burns** | All four romanceable companions (`RomanceContent`), each an **argument about a theme** — Garrow/faith, Roen/belonging, Naeve/guilt, Varra/worth — across a shared six-stage spine: **Spark → Trust → Turn → Crisis → Choosing → The Last Night.** Gated by **both** approval **and** progression (the Crisis *is* their personal quest), played out at the campfire's **"Grow Closer"** with a deepen-or-stay-platonic choice each beat |
| ✅ | **Romance as a stake, not a reward** | Pursuing two past the **Turn** is gently disallowed (`CommittedElsewhere`); whoever you love becomes the finale anchor, and **`romance.<id>.consummated`** writes a bespoke epilogue slide — including the shattering golden-ending beats (Garrow's last rite in **Break the Loop**, Naeve at the Ledger in **Jergal's Keyhole**, Varra burning the receipt) |
| ✅ | **Rupture / betrayal arcs** | The dark mirror (`RuptureContent`): crater a companion's approval and the bond **frays** — at camp they call you on the value you keep crossing. **Make amends** (only lands if you've earned standing — a night-talk shared or a spark kindled), **patch it over** (they stay, guarded), or **let them walk out for good** (`companion.<id>.left`, removed from the party). Epilogue-aware — the Court remembers who you drove away |
| ✅ | **Companion-reactive era beats — all four eras** | Bring the companion *from* an era and they confront their own history in the flesh: **Ilfaeril / Crown Wars** (the bench where he cast the vote), **Naeve / Netheril** (the balcony, the hour before the sky falls), **Garrow / Time of Troubles** (death's own god beaten into a crown), **Varra / Spellplague** (every contract's rule coming apart in the blue fire). Each is built live from the flags, keyed to that companion's quest resolution, grants approval, and earns a bespoke ending slide. Reusable via `SimpleEra.witnessGraph` + `EraWitness` |
| ✅ | **Cross-era callbacks — the time-travel earns its keep** | The *world itself* names a choice you made an age earlier and shows its downstream cost. In the **Time of Troubles**, a **grey gravedigger** working the godless dead speaks the **Crown Wars Verdict** back to you — if you *argued the damnation down* the unclaimed get a resting-place "because of that single afternoon"; if you *let it pass* the Wall "is still carrying." In the **Spellplague**, a **half-unmade soul** in the blue fire thanks (or forgives) you for that same verdict. Both echoes layer **two more conditional threads**: one if you witnessed **Netheril's fall**, and one that names **the Breach** — pulling Maerin from the Wall ("one soul out, one soul in… carry them both, the pulled and the paid") *or* the counted restraint of walking away (now a tracked choice, `fugue.left_maerin`). Built live from the flags, validated in CI, reusable via `SimpleEra.echoGraph` + `EraEchoes`. *A decision in -10,000 DR is spoken aloud in 1385 DR.* |
| ✅ | **A second explorable room — the Almshouse of the Unclaimed** | `AlmshouseScene`: a candlelit refuge for the Gate's godless poor, reachable by a hub door, where **everything reacts to your Act II run** — Mother Cass the keeper, a huddle of the unclaimed, a *Wall of Names*, and (if he's fielded) **Roen back in the gutter that made him** — the Outer City orphan reacting to his own roots, keyed to his sister's fate. Do right by the quarter and Cass sends you to the Court with the poor's **backwards Kelemvor token** (`almshouse.token` → its own ending slide). *Every companion now has a reactive "home" beat.* |
| ✅ | **Act II miniboss — the Hollow Cantor** | After the street-preaching is settled, a militant Faithless-Choir cell forms in the undercroft: an **optional hub skirmish** (`EnterChoirCell`/`BuildChoirCellEncounter`) against a named leader + zealots + sorrow-wraiths, routed through the real combat engine. Clearing it **protects the quarter** (reputation + Garrow/Ilfaeril approval) and earns its own Journey/Chronicle/ending payoff — Act II combat content, not just dialogue |
| ✅ | **Reactive Fugue beat (the Wall remembers)** | At the Wall of the Faithless, a half-dissolved soul speaks differently depending on the mercy you spent in the Lower City (the tithe, the choir, your standing) — and fairly **foreshadows the Breach's arithmetic** ("the only way a soul comes *out* is if another goes *in*… it takes the one nearest your heart"). Built live from the flags each time you descend |
| ✅ | **Romance ↔ the Breach (love as a stake)** | Once Varra is saved, the Wall is **drawn to whoever you love** — a committed romance becomes the Breach's tithe (`ChooseBreachVictim`), with a dedicated *"the one you loved, taken by the Wall"* epilogue. Whoever you let in closest becomes the **finale anchor** (`AnchorId`): every ending closes on a beat keyed to whether they're beside you, lost, or driven away |
| ✅ | **Reputation reaches the ending** | Lower City standing (`reputation.lowcity`) pays off: at ≥ 5 the crier pledges the quarter's allegiance (`lowcity.allies`), and `EndingResolver` gives a **Lower City epilogue slide** — the poor show up for you at the end (high standing) or remember you spent them like coin (negative). Surfaced in the Journey screen |
| ✅ | **Act II side content & a reactive world** | `ActTwoContent`: a Lower City layer where the world **watches you back.** **Tamsin the street crier** reads your reputation, romances, ruptures, and walked eras straight off the flags (via conditional dialogue choices). **Four** dialogue-resolved side quests — **"The Widow's Coin"** (lie / hard truth / [Insight] true comfort), **"The Fist and the Faithless"** ([Persuasion] free him / bribe / uphold the law), **"The Faithless Choir"** (an Unmade street-preacher: [Religion] doubt / suppress / speak for the Unmade — *seeds the main villain faction*), and **"The Tithe Collector"** (grief extortion — [Persuasion] tear up the ledger / pay the poor's debts yourself / take a cut) — each moving **`reputation.lowcity`** / approval, fully epilogue-aware, with outcome-gated aftermath NPCs |
| ✅ | **Branching moral micro-quests** | One per Lower City room: **the sinking skiff** (Docks — go in for old Pell, or walk on), **a finger or the cells** (Market — stand surety for a child thief, or let the Fist take them), **Old Hensley's last question** (Almshouse — a kind lie or the harder truth to a dying man), and **the bound informant** (Safehouse — turn her, or give her the choice no one has). Each a full loop: choice → `reputation.lowcity`/conscience → **Journey** tracking → **epilogue** slide → a **companion's fireside reaction** (Roen, Garrow, Maerin, Ilfaeril) |
| ✅ | **Knocked out, not dead** | Downed (0-HP) companions are **stabilized after a won fight** (back up at ¼ HP) — random battles never permanently cost you a companion; **permanent loss is reserved for the Breach**, by design |
| ✅ | **The bond breathes in battle** | Surviving a fight together nudges fielded companions' approval (`EncounterController`, +1, or **+3** for anyone you pulled back from the brink) — so the meter keeps moving across the saga, not just in set-piece dialogue |
| ✅ | **Floating nameplates & HP bars** | `UnitNameplates`: a name + faction-colored HP bar floats over every unit, with the **active combatant highlighted** — the "colored cubes" become legible at a glance (who's who, who's hurt, whose turn). Zero art, world-space, toggle with **N** |
| ✅ | **Floating combat text** | `FloatingCombatText`: damage/heal numbers, **MISS/RESIST**, big golden **crits**, and condition names pop off the target, drift up, and fade (`AbilityRunner` emits them). Self-bootstrapping, pure IMGUI — a fight finally *reads* without a single sprite |
| ✅ | **Unit art seam (cubes → sprites)** | `UnitSpriteSkinner`: the moment a PNG named after a unit lands in `Resources/Sprites/` (`Garrow.png`, `Doomguide.png`, or a catch-all `Enemy.png`), the cube is hidden and a **camera-facing sprite billboard** takes its place (with `IsoDepthSorter` so nearer units draw in front as they move) — collider kept, zero per-unit wiring, graceful no-op without art. Art-optional, exactly like the VFX system |
| ✅ | **Dialogue portraits** | `WorldArt.Portrait` + `DialogueScreen`: drop `Resources/Portraits/<speaker>.png` (or reuse a unit sprite) and a **portrait panel appears beside the conversation box**, captioned with the speaker — lookup falls full-name → first-word → map sprite, so one `Doomguide.png` covers every Doomguide line. No art → the box stands alone. Atlased or standalone sprites both work |
| ✅ | **Audio seam (SFX + music)** | `AudioSystem`: art-optional sound, exactly like the sprite/VFX seams. Combat plays **impact SFX by damage type** + heal/miss (`hit_fire`, `heal`, …) from `Resources/SFX/`; the director swaps **looping music per mode** (`Hub`/`Combat`/`Camp`/`Fugue`/`Court`/`Vault`/`Explore`) from `Resources/Music/` at the one `SwapMode` chokepoint. Missing clip → silence (music carries over); master volume rides the Options slider |
| ✅ | **Content validator** | `ContentValidator` + `ValidationDemo`: a runtime smoke test that walks **every reachable authored DialogueGraph** (all 5 quests, the Act II side content, Aldric, the riddles, the era-witness beats) and reports any **broken node reference / duplicate / missing start** — the typo class the (compiler-less) sandbox can't catch. PASS/FAIL to the Console *and* on-screen |
| ✅ | **One-click demos** | `DemoBootstrap`, `PrologueBootstrap`, `NetherilDemo`, `MirrorEncounterDemo`, `SpellplagueDemo`, `RiddleVaultDemo`, `BreachDemo`, `CampDemo`, `RomanceDemo`, `RuptureDemo`, `ActTwoDemo`, `RoenQuestDemo`, `VarraQuestDemo`, `GarrowQuestDemo`, `NaeveQuestDemo`, `IlfaerilQuestDemo`, `ValidationDemo`, `EndingDemo` |

---

## 🚧 In Progress / Next Up

| | Feature | Why it matters |
|---|---|---|
| 🚧 | **Full Act I content** | Hub + multi-room Cinderhaunt playable; now expanding NPCs, side quests, more rooms |
| 🚧 | **Companion depth** | Recruitment, banter, approval-gated night-talks, **all five personal quests**, and **all four romance arcs** are in; next: **Act II** connective tissue & betrayals |
| 🚧 | **Time-travel eras** | **Netheril (the falling city) playable**; Crown Wars / Time of Troubles / Spellplague designed & next |
| 📋 | **Roster UI, deepened** | Done: bench/field (press **P**). Next: companion sheets, drag-to-order party formation, approval readouts |
| 📋 | **Tilemap-based maps** | Author iso levels with Unity's Isometric Tilemap + sprite art |
| 📋 | **Sprite characters & animation** | Replace demo cubes with animated portraits/units |
| 📋 | **Inventory & equipment UI** | Drag-drop, slots, tooltips, item comparison |
| 📋 | **Reactions & opportunity attacks** | Leaving reach provokes; readied actions |
| 📋 | **Concentration & buffs** | Concentration checks, dispel, duration UI |
| 📋 | **Short/long rest** | Slot/HP recovery, hit dice, camp scenes |
| 📋 | **Merchants & economy** | Buy/sell, barter, the Guild & Zhentarim fences |

---

## 📋 Planned — The Road to a Baldur's-Gate-Class RPG

<details open>
<summary><b>⚔️ Combat depth</b></summary>

- 📋 Conditions: Grappled, Restrained, Charmed, Paralyzed, Petrified, Stunned (full set)
- 📋 Cover & line-of-sight, flanking, height advantage
- 📋 Multi-target spells, lines/cones, persistent hazards (grease, web, wall of fire)
- 📋 Resistances / immunities / vulnerabilities per damage type
- 📋 Death saves, downed-but-stabilized, revivify
- 💡 Reaction-based utility AI (utility scoring, retreat, focus-fire, kiting)
</details>

<details>
<summary><b>🧙 Character systems</b></summary>

- 📋 Subclasses / archetypes, feats, ability-score improvements
- 📋 Multiclassing
- 📋 Skill list & proficiency (Athletics → Arcana → Persuasion)
- 📋 Spell preparation & spellbook (Wizard) vs known (Sorcerer/Bard)
- 📋 Inspiration, exhaustion, carrying capacity
- 💡 Origin/destiny system tied to the *Returned* trait
</details>

<details>
<summary><b>💬 Narrative & reactivity</b></summary>

- 📋 7 companions with recruit convos, banter, **personal quests**, romances, betrayals
- 📋 Companion approval gates (lock/unlock dialogue, leave the party)
- 📋 Reputation-gated faction questlines (Harpers, Flaming Fist, Order of the Gauntlet, Zhentarim)
- 📋 The **six branching endings** + per-companion/faction epilogue slides
- 📋 Ink / Yarn Spinner import pipeline for writing at scale
- 💡 Reactive world state: villages remember, the dead linger near you
</details>

<details>
<summary><b>🗺️ World & content</b></summary>

- 📋 6 hubs/regions: Baldur's Gate districts, Candlekeep, Elturel, Fields of the Dead, the Fugue Plane, the City of Judgment
- 📋 14 dungeons/encounter maps; the Court of the Dead finale gauntlet
- 📋 Bestiary of ~22 archetypes (the Unbound, Doomguides, devils, Fugue shades)
- 📋 Traps, locks, secret doors, environmental interactables
- 📋 Day/night, fog of war, dynamic 2D lighting (torchlit dungeons)
</details>

<details>
<summary><b>🖥️ UX, audio & production</b></summary>

- 📋 Main menu, settings, save-slot manager, codex/journal/bestiary
- 📋 Tooltips, damage numbers, hit/miss VFX, ability targeting previews
- 📋 Music states (explore/combat/dialogue), SFX, ambience, VO hooks
- ✅ Accessibility: **UI text-size, combat-speed, floating-text toggle** (in Options) · 📋 localization (string tables), colorblind palette, key remap
- 📋 Controller support
- 💡 Steam/itch build pipeline, achievements, cloud saves
</details>

---

## 🧱 Architecture at a Glance

```
GameManager ── Party ── QuestManager ── DialogueRunner ── SaveSystem
     │            │           │              │              │
     └──────── GameFlags (the story brain: flags · approval · reputation) ───────┘
                        │
   GridSystem ── TurnManager ── CharacterSheet ── AbilityRunner ── AttackResolver
   (iso + A*)   (initiative)    (5e stats, POCO)   (validate+AoE)   (pure d20 math)
        │                                                  │
     GridUnit ◄────── EncounterController (AI + XP) ──────►  Progression (XP/level)
        │
   PlayerCombatInput · CombatHUD · DialogueScreen · CharacterCreationScreen
```

> 🧩 **Design pillar:** *Code is the engine, ScriptableObject assets are the game.* A
> writer/designer builds classes, spells, items, dialogue, and quests **without touching
> C#** — which is exactly how a small team ships a big CRPG.

---

<div align="center">

### 🎲 *"You came back. Few do."* 🎲

**Next milestone →** the Baldur's Gate hub & first real dungeon. See `docs/ROADMAP.md`.

</div>
