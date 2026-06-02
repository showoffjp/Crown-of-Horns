using System.Collections.Generic;
using UnityEngine;
using SunderedCrown.Core;
using SunderedCrown.Stats;

namespace SunderedCrown.Characters
{
    /// <summary>
    /// Assembles a level-1 CharacterSheet from creation choices using 5e rules, and
    /// provides the two standard ability-score generation methods (point-buy and the
    /// standard array). Pure logic — drive it from any UI (the OnGUI
    /// CharacterCreationScreen, or a future uGUI screen).
    /// </summary>
    public static class CharacterBuilder
    {
        public const int PointBuyBudget = 27;
        public const int PointBuyMin = 8;
        public const int PointBuyMax = 15;

        public static readonly int[] StandardArray = { 15, 14, 13, 12, 10, 8 };

        // 5e point-buy cost per score (8..15).
        private static readonly Dictionary<int, int> PointCost = new Dictionary<int, int>
        {
            {8,0},{9,1},{10,2},{11,3},{12,4},{13,5},{14,7},{15,9}
        };

        /// <summary>Total points spent for a set of base scores. Returns -1 if any score is out of the legal 8..15 range.</summary>
        public static int PointsSpent(AbilityScores baseScores)
        {
            int total = 0;
            foreach (Ability a in System.Enum.GetValues(typeof(Ability)))
            {
                int v = baseScores.Get(a);
                if (!PointCost.TryGetValue(v, out int cost)) return -1;
                total += cost;
            }
            return total;
        }

        public static bool IsLegalPointBuy(AbilityScores baseScores)
        {
            int spent = PointsSpent(baseScores);
            return spent >= 0 && spent <= PointBuyBudget;
        }

        /// <summary>
        /// Build a complete, playable level-1 sheet. <paramref name="baseScores"/> are
        /// the pre-racial scores; racial bonuses from the race are added here.
        /// </summary>
        public static CharacterSheet Build(
            string name,
            RaceDefinition race,
            ClassDefinition cls,
            BackgroundDefinition background,
            AbilityScores baseScores,
            bool writeFlags = true)
        {
            var sheet = new CharacterSheet
            {
                displayName = string.IsNullOrWhiteSpace(name) ? "Adventurer" : name,
                raceDef = race,
                classDef = cls,
                level = 1
            };

            // Copy base scores, then apply racial bonuses.
            foreach (Ability a in System.Enum.GetValues(typeof(Ability)))
            {
                int v = baseScores.Get(a) + (race != null ? race.BonusFor(a) : 0);
                sheet.abilities.Set(a, v);
            }

            // Starting AC: unarmored = 10 (+Dex computed in CharacterSheet.ArmorClass).
            sheet.baseArmorClass = 10;

            // Hit points from class hit die + CON.
            sheet.RecalculateMaxHitPoints();

            // Grant level-1 class abilities.
            if (cls != null && cls.startingAbilities != null && cls.startingAbilities.Length > 0 &&
                cls.startingAbilities[0] != null)
                sheet.knownAbilities.Add(cls.startingAbilities[0]);

            // Background reactivity + kit.
            if (background != null)
            {
                if (writeFlags && !string.IsNullOrEmpty(background.grantsFlag))
                    GameFlags.Current.SetBool(background.grantsFlag, true);
                if (Party.Instance != null)
                {
                    Party.Instance.inventory.AddGold(background.startingGold);
                    if (background.startingItem != null)
                        Party.Instance.inventory.Add(background.startingItem);
                }
            }

            // The protagonist is always "Returned" in this saga (see Story Bible).
            if (writeFlags) GameFlags.Current.SetBool("pc.returned", true);

            return sheet;
        }
    }
}
