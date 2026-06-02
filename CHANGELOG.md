<div align="center">

# 📝 CHANGELOG — The Crown of Horns

![Keep a Changelog](https://img.shields.io/badge/format-Keep_a_Changelog-orange)
![SemVer](https://img.shields.io/badge/versioning-SemVer-blue)

*All notable changes to the project. Newest first.*

</div>

---

## 🏛️ v0.3.0 — *"What the Dead Owe"* — Setting, Creation, Leveling & the Playable Prologue

> The project moves to the **Forgotten Realms / Sword Coast** and gains a full
> create-a-hero → dialogue → battle → level-up loop with a real uGUI HUD.

### 🌍 Setting & Story
- 🗺️ **Story Bible rewritten for the Forgotten Realms** (`docs/STORY_BIBLE.md`): the
  **Wall of the Faithless**, villain **Aldric Morn** & the **Crown of Horns** (Myrkul),
  **Sister Vayle**, **Jergal**, 7 FR companions, factions (Harpers, Flaming Fist, Order of
  the Gauntlet, Zhentarim, church of Kelemvor), 3 acts, and **five branching endings**.

### 🧙 Character Creation *(new)*
- 🧬 `BackgroundDefinition` assets (skills, feature, **flag that unlocks dialogue**).
- 🛠️ `CharacterBuilder`: **27-point point-buy** + **standard array**, racial bonuses,
  HP/AC assembly, background kit & flags, the **Returned** trait.
- 🖥️ `CharacterCreationScreen`: race/class/background pickers + point-buy stepper.

### 📈 Leveling & XP *(new)*
- ⭐ `Progression`: full **5e XP table** (1→20), HP & ability grants on level-up,
  proficiency scaling, `OnLevelUp` event.
- 🏆 **XP awarded on victory** — pooled from defeated foes, split among survivors
  (`EncounterController`), can trigger level-ups mid-run.

### 🖥️ Real uGUI HUD *(new)*
- 🎛️ `CombatHUD`: runtime-built canvas — **party portraits + live HP bars**, initiative,
  active-unit vitals/economy, a **clickable ability bar**, combat log, End Turn. Zero setup.
- 🗨️ `DialogueScreen`: speaker/body/choices with live **skill-check** results; auto-play
  & on-finished hooks.

### 🏰 Act I Content Scaffold *(new)*
- 📦 `SwordCoastContent`: builds FR **races, classes, backgrounds, spells/weapons, status
  effects**, the **opening dialogue** with Aldric's herald, and the **first quest** in code.
- 🎬 `PrologueBootstrap`: the flagship demo — **creation → dialogue → turn-based battle →
  XP/level → quest completion**, fully self-contained.

### 📚 Docs
- ✨ New **`FEATURES.md`** (implemented + planned codex with status icons & badges).
- ✨ **README** redesigned with badges, feature grid, and quick-start tables.

---

## ⚔️ v0.2.0 — Combat Depth Layer

> The systems that make combat *feel* like D&D 5e.

- ✨ **Spell slots / resources** spent by leveled spells, refreshed at combat start.
- ☠️ **Status effects / conditions** (`StatusEffect.cs`, `Condition.cs`): duration, DoT,
  incapacitation, advantage grants, flat AC/attack/speed modifiers.
- ⚖️ **Advantage / disadvantage** in `AttackResolver` (cancel per 5e).
- 🎯 **Saving-throw spells with save-for-half** and **healing abilities**.
- 🧩 **`AbilityRunner`**: one validated entry point — range, slots, action economy, **AoE**,
  condition application, logging. Player input & enemy AI both use it.
- 🛡️ **Equipment hooks** on `CharacterSheet`.
- 🔥 **Demo upgrade**: wizard (Firebolt + **Fireball**), cleric (**Healing Word**), and a
  poison-clawing enemy. HUD shows conditions, slots, and a numbered ability bar.

---

## 🧱 v0.1.0 — Initial Starter

> The foundation.

- 🗺️ Grid + isometric projection, **A\* pathfinding**, isometric camera.
- 🎲 Turn-based combat: initiative + action economy + win/loss.
- 🧙 5e stats: abilities, modifiers, proficiency, AC, HP, d20 attacks vs AC.
- 💬 Data-driven **dialogue / quests / items** via ScriptableObjects.
- 🧠 **GameFlags** story brain; 💾 JSON **save/load**.
- 🕹️ One-click **playable demo** + the full **story bible**.
