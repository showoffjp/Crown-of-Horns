using System;
using UnityEngine;
using SunderedCrown.Stats;

namespace SunderedCrown.Characters
{
    /// <summary>
    /// Experience and level progression on the 5e XP table. Awarding XP and leveling
    /// up are pure operations on a CharacterSheet, so they're testable and save-safe.
    /// Level-up grants average hit points (+CON), and the higher level automatically
    /// improves proficiency bonus (computed in CharacterSheet) and any class abilities
    /// gated by level (ClassDefinition.startingAbilities indexed by level).
    /// </summary>
    public static class Progression
    {
        public const int MaxLevel = 20;

        // Total XP required to BE a given level (index = level; index 0 unused).
        private static readonly int[] XpForLevel =
        {
            0,
            0, 300, 900, 2700, 6500, 14000, 23000, 34000, 48000, 64000,
            85000, 100000, 120000, 140000, 165000, 195000, 225000, 265000, 305000, 355000
        };

        public static int XpToReach(int level) =>
            (level >= 1 && level <= MaxLevel) ? XpForLevel[level] : int.MaxValue;

        public static int XpToNextLevel(CharacterSheet s) =>
            s.level >= MaxLevel ? 0 : Mathf.Max(0, XpForLevel[s.level + 1] - s.experience);

        /// <summary>Raised after a sheet gains a level. (sheet, newLevel)</summary>
        public static event Action<CharacterSheet, int> OnLevelUp;

        /// <summary>Award XP and apply any level-ups it triggers. Returns levels gained.</summary>
        public static int AwardExperience(CharacterSheet sheet, int amount)
        {
            if (sheet == null || amount <= 0 || sheet.level >= MaxLevel) return 0;
            sheet.experience += amount;

            int gained = 0;
            while (sheet.level < MaxLevel && sheet.experience >= XpForLevel[sheet.level + 1])
            {
                LevelUp(sheet);
                gained++;
            }
            return gained;
        }

        /// <summary>Advance one level: HP, granted abilities. Proficiency scales by level automatically.</summary>
        public static void LevelUp(CharacterSheet sheet)
        {
            sheet.level++;

            if (sheet.classDef != null)
            {
                int conMod = sheet.Modifier(Ability.Constitution);
                int hpGain = Mathf.Max(1, sheet.classDef.AverageHitDieGain + conMod);
                sheet.maxHitPoints += hpGain;
                sheet.currentHitPoints += hpGain; // heal by the gained amount

                // Grant any ability the class unlocks at this level (1-indexed array).
                var grants = sheet.classDef.startingAbilities;
                int idx = sheet.level - 1;
                if (grants != null && idx >= 0 && idx < grants.Length && grants[idx] != null &&
                    !sheet.knownAbilities.Contains(grants[idx]))
                    sheet.knownAbilities.Add(grants[idx]);
            }

            OnLevelUp?.Invoke(sheet, sheet.level);
        }
    }
}
