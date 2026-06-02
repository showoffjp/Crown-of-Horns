using UnityEngine;
using SunderedCrown.Stats;

namespace SunderedCrown.Characters
{
    /// <summary>
    /// Data-only definition of an ancestry/race (Human, Elf, Dwarf, the setting-specific
    /// Ashborn, etc.). Authored as a ScriptableObject.
    ///
    /// Create via: Assets > Create > Sundered Crown > Race Definition
    /// </summary>
    [CreateAssetMenu(menuName = "Sundered Crown/Race Definition", fileName = "NewRace")]
    public class RaceDefinition : ScriptableObject
    {
        [Header("Identity")]
        public string raceName = "Human";
        [TextArea(2, 5)] public string description;

        [Header("Mechanics")]
        [Tooltip("Tiles per turn of movement (5e: 30ft = 6 tiles).")]
        public int baseSpeedTiles = 6;

        [Tooltip("Flat bonuses applied to ability scores at character creation.")]
        public AbilityBonus[] abilityBonuses;

        [System.Serializable]
        public struct AbilityBonus
        {
            public Ability ability;
            public int bonus;
        }

        public int BonusFor(Ability ability)
        {
            int total = 0;
            if (abilityBonuses != null)
                foreach (var b in abilityBonuses)
                    if (b.ability == ability) total += b.bonus;
            return total;
        }
    }
}
