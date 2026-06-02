<div align="center">

# ⚔️ THE CROWN OF HORNS ⚔️
## 👑 *A Classic Isometric CRPG for the Forgotten Realms* 👑

*In the lineage of Baldur's Gate · Icewind Dale · Planescape: Torment · built on D&D 5e*

<br/>

![Status](https://img.shields.io/badge/Build-Vertical_Slice_Playable-brightgreen?style=for-the-badge)
![Engine](https://img.shields.io/badge/Unity-6_LTS-black?style=for-the-badge&logo=unity)
![C#](https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=csharp&logoColor=white)
![Rules](https://img.shields.io/badge/D%26D-5e_SRD_5.1-b51e2e?style=for-the-badge&logo=dungeonsanddragons&logoColor=white)

![Combat](https://img.shields.io/badge/⚔️-Turn--Based-blue)
![Iso](https://img.shields.io/badge/🗺️-Isometric_Grid-teal)
![Story](https://img.shields.io/badge/📜-Branching_Narrative-purple)
![Party](https://img.shields.io/badge/🛡️-Party_Based-orange)
![Scripts](https://img.shields.io/badge/💾-38_scripts_·_3.7k_LOC-informational)

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

📖 **Read the full saga →** [`docs/STORY_BIBLE.md`](docs/STORY_BIBLE.md)

---

## ✨ What This Is

A **runnable engine + a complete writers' bible** for building a Baldur's-Gate-class CRPG
in Unity. Not a finished game — your **vertical slice + foundation**. Press Play today and
you get a full loop: **create a hero → talk your way through a branching scene → fight a
tactical 5e battle → earn XP and level up.**

```
🎯 Create the Returned   ➜   💬 Face Aldric's herald   ➜   ⚔️ Survive the Doomguides   ➜   ⭐ Gain XP & level
   (race/class/point-buy)     (skill checks, flags)        (turn-based, spells, AoE)        (5e progression)
```

---

## 🎮 Feature Highlights

<table>
<tr>
<td width="50%" valign="top">

### ⚔️ Tactical 5e Combat
- 🎲 Initiative, action economy (move/action/bonus)
- 🗡️ d20 attacks vs AC · saving throws · **crits**
- ✨ Spell slots, **Fireball-style AoE**, save-for-half
- ☠️ Status effects: Poisoned, Burning (DoT), Prone, Blessed…
- ⚖️ **Advantage / disadvantage** from conditions
- 🤖 Enemy AI: targeting, positioning, melee & ranged

</td>
<td width="50%" valign="top">

### 📜 Reactive Narrative
- 💬 Branching dialogue with **skill checks**
- 🧠 One **GameFlags** brain drives all reactivity
- ❤️ Companion **approval** & faction **reputation**
- 🗒️ Flag-driven **quests** with XP/gold rewards
- 🎭 Background choices unlock **unique dialogue**
- 🌟 Designed for **five branching endings**

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
| 🗡️ Jump into a fight | **`DemoBootstrap`** | Press Play — instant skirmish |
| 🎬 Play the full prologue | **`PrologueBootstrap`** | Create a hero → dialogue → battle → XP |

**🎛️ Controls:** `1`–`9` arm an ability · click a tile to **move** · click a target to **act**
· `Space` **end turn** · mouse-wheel **zoom** · WASD/edges **pan** · `F5`/`F9` **quick save/load**.

📘 Full walkthrough → [`GETTING_STARTED.md`](GETTING_STARTED.md)

---

## 📚 Documentation

| Doc | What's inside |
|---|---|
| 📖 [`docs/STORY_BIBLE.md`](docs/STORY_BIBLE.md) | The Sword Coast saga: villains, **7 companions**, factions, 3 acts, **5 endings** |
| 🚀 [`GETTING_STARTED.md`](GETTING_STARTED.md) | Install Unity → run the demo → author your first content |
| 🧱 [`docs/ARCHITECTURE.md`](docs/ARCHITECTURE.md) | How the systems fit and where to extend |
| ✍️ [`docs/CONTENT_PIPELINE.md`](docs/CONTENT_PIPELINE.md) | Build dialogue/quests/items as assets — no code |
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
