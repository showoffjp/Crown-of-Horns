<div align="center">

# ⚔️ THE CROWN OF HORNS ⚔️
## 👑 *A Classic Isometric CRPG for the Forgotten Realms* 👑

*In the lineage of Baldur's Gate · Icewind Dale · Planescape: Torment · built on D&D 5e*

<br/>

![Status](https://img.shields.io/badge/Build-Vertical_Slice_Playable-brightgreen?style=for-the-badge)
![Release](https://img.shields.io/badge/release-v6.62.0-gold?style=for-the-badge)
![Engine](https://img.shields.io/badge/Unity-6_LTS-black?style=for-the-badge&logo=unity)
![C#](https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=csharp&logoColor=white)
![Rules](https://img.shields.io/badge/D%26D-5e_SRD_5.1-b51e2e?style=for-the-badge&logo=dungeonsanddragons&logoColor=white)

![Combat](https://img.shields.io/badge/⚔️-Turn--Based-blue)
![Iso](https://img.shields.io/badge/🗺️-Isometric_Grid-teal)
![Story](https://img.shields.io/badge/📜-Branching_Narrative-purple)
![Party](https://img.shields.io/badge/🛡️-Party_Based-orange)
![Tests](https://img.shields.io/badge/🧪-29_suites_·_~201_tests-success)
![CI](https://img.shields.io/badge/CI-repo--hygiene_+_game--ci-blue?logo=githubactions&logoColor=white)
![Scripts](https://img.shields.io/badge/💾-165_scripts_·_~20k_LOC-informational)

</div>

---

> 🕯️ *"You should be dead. I watched the harvest take you. And yet — here you stand, with
> the Wall's chill still on your soul."*

In the afterlife of the Realms, souls who served no god are mortared alive into the
**Wall of the Faithless** and left to dissolve into nothing. **Aldric Morn** — once the
Sword Coast's finest Doomguide of Kelemvor — watched his godless daughter suffer that
fate, and broke. Now he hunts the **Crown of Horns** to depose the god of the dead and
tear the Wall down forever... no matter how many of the living he must sacrifice to do it.

You died in the harvest beneath Baldur's Gate. Something pulled you back. You are
**Returned** — and you are the one soul who can complete Aldric's apotheosis, or unmake it.

…but Aldric is only the first of **four masks.** The crack in your soul is a crack in
**time**, and the saga walks the hinge-points of Realms history — **Netheril's fall**, the
**Crown Wars**, the **Time of Troubles**, the **Spellplague** — chasing the **Unmade** (the
accreted nothing of every soul the cosmos threw away) and a hunter who wears your own face:
**the Last Returned**, the you from a future that already lost. *What is a soul worth, that
no god ever claimed?*

📖 **Read the full, mind-bending saga →** [`docs/STORY_BIBLE.md`](docs/STORY_BIBLE.md)

---

## ✨ What This Is

A **runnable engine + a complete writers' bible** for building a Baldur's-Gate-class CRPG
in Unity. Not a finished game — your **vertical slice + foundation**. Press Play today and
you get a full loop: **create a hero → talk your way through a branching scene → fight a
tactical 5e battle → earn XP and level up.**

```
🎯 Create ➜ 🧭 Explore the Gate ➜ 💬 Quest & recruit ➜ ⚔️ Cinderhaunt ➜ 🌀 Skip to Netheril ➜ 🌆 Survive the falling city ➜ ⭐ XP & return
 (point-buy)  (click-to-move hub)   (branching dialogue)  (boss + loot)    (time travel!)        (collapsing-floor battle)     (back to town)
```

---

## 🎮 Feature Highlights

<table>
<tr>
<td width="50%" valign="top">

### ⚔️ Tactical 5e Combat
- 🎲 Initiative, action economy, **per-level class kits** (to high-level spells)
- 🗡️ d20 vs AC · saves · **crits** · spell slots · save-for-half AoE
- 🛡️ Full action menu: **Defend · Dash · Help · Disengage · Shove · Quaff**
- ↔️ **Opportunity attacks · flanking · half-cover** — positioning matters
- ☠️ Conditions, DoT, advantage/disadvantage, **Bloodied** reads
- 🤖 Enemy AI: **focus-fire · AoE-cluster bursts · self-preservation · kiting**
- ⚖️ Seeded **balance canary** · camera auto-focus · party-wipe recovery

</td>
<td width="50%" valign="top">

### 📜 Reactive Narrative
- 💬 Branching dialogue with **skill checks**; one **GameFlags** brain drives it all
- 🧑‍🤝‍🧑 **6 companions** — approval → night-talks → personal quests → **4 romances** → epilogues
- 🔥 **34 fireside banters** that react to your choices, losses, and the bosses you fell
- ⚖️ **4 branching moral micro-quests** (choice → rep/conscience → Journey → epilogue → a companion's reaction)
- 🏛️ **Faction reputations** (Kelemvor, the Choir) with reactive hub figures & ending gates
- 🌟 **Six branching endings** + dozens of flag-keyed epilogue slides · **New-Game+** memory

</td>
</tr>
<tr>
<td width="50%" valign="top">

### 🧙 Characters & Progression
- 🧬 Six abilities, classes, ancestries, backgrounds
- 🛠️ **Character creation**: point-buy or standard array
- 📈 **XP & leveling** on the full 5e table
- 🎒 Party roster + shared inventory & gold

</td>
<td width="50%" valign="top">

### 🖥️ UI & Tools
- 🖼️ **Runtime uGUI HUD**: portraits, HP bars, ability bar
- 🗨️ Dialogue & creation screens (zero setup)
- 💾 **JSON save/load** (quick-save F5/F9)
- 🧩 Everything **data-driven** via ScriptableObjects

</td>
</tr>
</table>

📋 **Full feature codex (implemented + planned) →** [`FEATURES.md`](FEATURES.md)

---

## 🚀 Quick Start (≈30 min to playing)

```bash
# 1. New Unity project — 2D (URP) template, Unity 6 LTS or 2022.3 LTS
# 2. Copy this Assets/ into it; copy docs/ + the .md files + .gitignore to the root
# 3. In Unity: new 2D scene → empty GameObject → Add Component →
```

| Want to... | Add this component | Then |
|---|---|---|
| ▶️ **Start here (front end)** | **`MainMenu`** | **New Game / Continue / Quit** — the boot screen; Continue rebuilds your hero & roster from the save |
| 🏰 Play the full loop directly | **`CampaignBootstrap`** | Create → Baldur's Gate → Cinderhaunt → **tea with Aldric** → the four eras → the Court of the Dead |
| 🌆 See the city fall | **`NetherilDemo`** | The **falling-city battle** — the floor collapses each turn |
| 🪞 Fight your reflection | **`MirrorEncounterDemo`** | The **Echoes**: twisted clones of your party + the Last Returned |
| 🌀 Fight where reality breaks | **`SpellplagueDemo`** | **Causality-optional** combat — units swap places & blue fire lashes each turn |
| 🎭 Solve the riddle room | **`RiddleVaultDemo`** | The **Vault of Tens** — place tokens, speak answers; **wit beats correctness** |
| ⚰️ Feel a permanent loss | **`BreachDemo`** | Pull Maerin from the Wall — and lose a companion **forever** |
| 🔥 Rest & hear them out | **`CampDemo`** | The **campfire**: long rest to heal & refresh slots, then **approval-gated night-talks** |
| 💞 Walk a romance arc | **`RomanceDemo`** | The **slow burns**: all four romances, six stages each (Spark→Trust→Turn→Crisis→Choosing→Last Night) |
| 💔 Break a bond | **`RuptureDemo`** | **Betrayal arcs**: cratered approval frays a bond — mend it, patch it, or let them walk out forever |
| 🏙️ Work the Lower City | **`ActTwoDemo`** | **Act II side content**: a reactive street crier + two moral side quests that move your reputation |
| 🗡️ Play Roen's quest | **`RoenQuestDemo`** | **"The Honest Lie"** — the silent safehouse, the reveal, a **moral trilemma** |
| ⚖️ Play Varra's quest | **`VarraQuestDemo`** | **"The Bill Comes Due"** — an infernal contract, the broker Quill, who pays |
| ⚖️ Play Garrow's quest | **`GarrowQuestDemo`** | **"A God-Shaped Hole"** — a heresy tribunal, Justiciar Veld, the doctrine on trial |
| ✨ Play Naeve's quest | **`NaeveQuestDemo`** | **"After the Sky Fell"** — a falling fragment of Netheril, the Steward's echo, what to do with home |
| 🕊️ Play Ilfaeril's quest | **`IlfaerilQuestDemo`** | **"The Vote"** — an ancient reliquary, the Choir's lie, and a forgiveness he can't accept |
| 🌅 Tour the endings | **`EndingDemo`** | The **Court of the Dead** — read all six endings' prose + epilogues |
| 🎬 Play the prologue fight | **`PrologueBootstrap`** | Create a hero → dialogue → battle → XP |
| 🗡️ Jump into a skirmish | **`DemoBootstrap`** | Press Play — instant tactical fight |

**🎛️ Controls:** `1`–`9` arm an ability · click a tile to **move** · click a target to **act**
· `G` **defend** · `F` **dash** · `T` **help** · `X` **disengage** · `V` **shove** · `Q` **quaff potion**
· `Space` **end turn** · `I` **inventory** · `J` **journey/quest log** · `P` **party (bench/field/sheets)**
· `K` **codex/bestiary** · `B` **mute banter** · `O` **options** · `H` **help** · `C` **chronicle** · `N` **nameplates** · `L` **relationships** · `E` **interact** · mouse-wheel **zoom** · WASD/edges
**pan** · `F5`/`F9` **quick save/load**.

📘 Full walkthrough → [`GETTING_STARTED.md`](GETTING_STARTED.md) · 🎮 Guided slice run →
[`docs/HOW_TO_PLAY_THE_SLICE.md`](docs/HOW_TO_PLAY_THE_SLICE.md)

---

## 📚 Documentation

| Doc | What's inside |
|---|---|
| 📖 [`docs/STORY_BIBLE.md`](docs/STORY_BIBLE.md) | Quick-reference overview of the saga: 4 nested villains, 7 companions, 5 acts, 6 endings |
| 📚 [`docs/story/`](docs/story/README.md) | **The full novel-grade bible (~54k words, 24 docs)** — synopsis, deep backstories, act breakdowns, dialogue scripts, romances, all six endings, the twist architecture, the bestiary, the pitch & vertical-slice plan, the art & music bible, the riddler & her Vault of Tens, and a craft study of the genre's greatest twists |
| 📖 [`docs/chapters/`](docs/chapters/README.md) | **Chapter-by-chapter walkthrough** — one detailed file per Prologue + Act (setting, story beat-by-beat, key quests, set-pieces, twists, companion beats, and a *Top 10 things that happen* each) |
| 🎭 [`docs/characters/`](docs/characters/README.md) | **A deep dossier per major character** — protagonist, the four villain masks, Vayle, Jergal, and all seven companions (wound, secret, want vs. need, arc, romance, fates) + a pointer to the 104-strong NPC roster |
| 🚀 [`GETTING_STARTED.md`](GETTING_STARTED.md) | Install Unity → run the demo → author your first content |
| 🧱 [`docs/ARCHITECTURE.md`](docs/ARCHITECTURE.md) | How the systems fit and where to extend |
| ✍️ [`docs/CONTENT_PIPELINE.md`](docs/CONTENT_PIPELINE.md) | Build dialogue/quests/items as assets — no code |
| 🎨 [`docs/ASSET_INTEGRATION.md`](docs/ASSET_INTEGRATION.md) | Drop in art (FX/sprites/tiles) so it auto-plugs into the engine |
| 🗺️ [`docs/ROADMAP.md`](docs/ROADMAP.md) | Milestones from slice → ship, with anti-scope-creep rules |
| 📋 [`FEATURES.md`](FEATURES.md) | The full feature codex with status icons |
| 📝 [`CHANGELOG.md`](CHANGELOG.md) | Version-by-version history |

---

## 🧱 Architecture in One Breath

> 🧩 **Code is the engine. ScriptableObject assets are the game.** Designers build
> classes, spells, items, dialogue, and quests **without touching C#** — the only way a
> small team ships a big CRPG.

```
GameManager ─ Party ─ QuestManager ─ DialogueRunner ─ SaveSystem
         └────────── GameFlags (flags · approval · reputation) ──────────┘
GridSystem ─ TurnManager ─ CharacterSheet ─ AbilityRunner ─ AttackResolver
  (iso+A*)   (initiative)   (5e POCO)        (validate+AoE)   (pure d20)
```

---

## ⚖️ Licensing Note

The **5e mechanics** are open under the **SRD 5.1 (CC-BY-4.0 / OGL)** — fair to ship. The
**Forgotten Realms setting** (Baldur's Gate, the deities, the Crown of Horns) is **Wizards
of the Coast** IP — perfect for a personal/prototype/portfolio build, but a *commercial*
release needs a WotC license or original renames. Everything is data-driven, so reskinning
names later is trivial.

---

<div align="center">

### 🎲 *Roll for initiative.* 🎲

Made with 🖤 for everyone who still hears *"You must gather your party before venturing forth."*

</div>
