# Changelog

## v0.2.0 — Combat depth layer

Added the systems that make combat feel like D&D 5e:

- **Spell slots / resources** (`SpellSlots` on `CharacterSheet`): per-level slots,
  spent by abilities with `spellSlotLevel > 0`, refreshed at the start of combat.
- **Status effects / conditions** (`Stats/Condition.cs`, `Combat/StatusEffect.cs`):
  data-driven `StatusEffectDefinition` assets with duration, damage-over-time,
  incapacitation, advantage/disadvantage grants, and flat AC/attack/speed modifiers.
  Effects tick (DoT) at the start of a turn and count down at the end.
- **Advantage / disadvantage**: derived from conditions and applied in `AttackResolver`
  (Prone/Restrained grant attackers advantage; Poisoned/Frightened give the bearer
  disadvantage; they cancel out per 5e).
- **Saving-throw spells with save-for-half** and **healing abilities** in
  `AttackResolver`.
- **`AbilityRunner`**: one validated entry point for using any ability — checks range,
  spell slots, and the action economy, gathers single or **area-burst** targets,
  resolves, applies conditions, and logs. Both player input and enemy AI use it.
- **Equipment hooks** on `CharacterSheet` (`equippedWeaponAbility`, `armorClassFromGear`).
- **Demo upgrade**: the one-click skirmish now fields a fighter, a **wizard**
  (Firebolt cantrip + **Fireball** AoE with Dex save, half-on-save, and a Burning DoT),
  and a **cleric** (**Healing Word** bonus action), plus a poison-clawing enemy. The
  HUD shows conditions, spell slots, and a numbered ability bar.

### Demo controls
`1`/`2` arm an ability · click a tile to move · click a valid target to use the armed
ability · `Space` ends the turn. Try Lyra: press `2`, click an enemy cluster.

## v0.1.0 — Initial starter

Grid + isometric projection, A* pathfinding, turn-based combat (initiative + action
economy), 5e stats, data-driven dialogue/quests/items, GameFlags story brain, JSON
save/load, isometric camera, one-click playable demo, and the full story bible.
