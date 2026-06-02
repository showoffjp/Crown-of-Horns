using System;
using System.Collections.Generic;
using UnityEngine;
using SunderedCrown.Stats;

namespace SunderedCrown.Characters
{
    /// <summary>
    /// The runtime state of one creature (player, companion, or enemy).
    /// This is a plain serializable C# object — NOT a MonoBehaviour — so it can be
    /// saved/loaded, copied, and unit-tested without a Unity scene. A GridUnit
    /// MonoBehaviour wraps it for the on-map representation.
    /// </summary>
    [Serializable]
    public class CharacterSheet
    {
        public string id = Guid.NewGuid().ToString();
        public string displayName = "Unnamed";

        [Header("Build")]
        public ClassDefinition classDef;
        public RaceDefinition raceDef;
        public AbilityScores abilities = new AbilityScores();
        public int level = 1;

        [Header("Vitals")]
        public int maxHitPoints = 1;
        public int currentHitPoints = 1;
        public int baseArmorClass = 10;

        [Header("Loadout")]
        public List<AbilityDefinition> knownAbilities = new List<AbilityDefinition>();

        public bool IsAlive => currentHitPoints > 0;

        // ---- Derived stats (5e math) -------------------------------------

        /// <summary>Proficiency bonus scales with level: +2 at 1-4, +3 at 5-8, etc.</summary>
        public int ProficiencyBonus => 2 + (level - 1) / 4;

        public int Modifier(Ability ability) => abilities.Modifier(ability);

        /// <summary>Movement budget in tiles for one turn.</summary>
        public int SpeedTiles => raceDef != null ? raceDef.baseSpeedTiles : 6;

        /// <summary>AC = base + Dex modifier (armor systems can override base later).</summary>
        public int ArmorClass => baseArmorClass + Modifier(Ability.Dexterity);

        public int InitiativeModifier => Modifier(Ability.Dexterity);

        public Ability SpellcastingAbility =>
            classDef != null ? classDef.spellcastingAbility : Ability.Intelligence;

        // ---- Mutations ---------------------------------------------------

        public void TakeDamage(int amount)
        {
            currentHitPoints = Mathf.Max(0, currentHitPoints - Mathf.Max(0, amount));
        }

        public void Heal(int amount)
        {
            currentHitPoints = Mathf.Min(maxHitPoints, currentHitPoints + Mathf.Max(0, amount));
        }

        /// <summary>
        /// Compute and assign max HP from class hit die + Con modifier across levels.
        /// Call once after setting class/race/abilities/level at creation.
        /// </summary>
        public void RecalculateMaxHitPoints()
        {
            if (classDef == null) { maxHitPoints = Math.Max(1, currentHitPoints); return; }

            int conMod = Modifier(Ability.Constitution);
            // Level 1: full hit die + Con. Subsequent levels: average + Con.
            int hp = classDef.hitDie + conMod;
            for (int lvl = 2; lvl <= level; lvl++)
                hp += classDef.AverageHitDieGain + conMod;

            maxHitPoints = Math.Max(1, hp);
            currentHitPoints = maxHitPoints;
        }

        /// <summary>Roll initiative for combat ordering.</summary>
        public int RollInitiative() => Dice.D20() + InitiativeModifier;
    }
}
