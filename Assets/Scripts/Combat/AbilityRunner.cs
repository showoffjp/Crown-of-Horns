using System.Collections.Generic;
using UnityEngine;
using SunderedCrown.Characters;
using SunderedCrown.Grid;

namespace SunderedCrown.Combat
{
    /// <summary>
    /// The single entry point for "use an ability." Validates range, spell slots,
    /// and the action economy; gathers targets (single or area burst); resolves and
    /// applies each; logs everything. Both player input and enemy AI route through
    /// here so behavior stays consistent.
    /// </summary>
    public static class AbilityRunner
    {
        /// <summary>Cheap pre-check used by UI to grey out unusable abilities.</summary>
        public static bool CanAfford(TurnManager turns, GridUnit caster, AbilityDefinition ab)
        {
            if (ab == null) return false;
            if (!caster.Sheet.spellSlots.Has(ab.spellSlotLevel)) return false;
            return ab.cost switch
            {
                ActionCost.Action      => turns.ActionAvailable,
                ActionCost.BonusAction => turns.BonusActionAvailable,
                _                      => true
            };
        }

        public static bool InRange(GridUnit caster, GridUnit target, AbilityDefinition ab)
        {
            if (ab.targeting == TargetingMode.Self) return true;
            if (target == null) return false;
            return GridSystem.ManhattanDistance(caster.Cell, target.Cell) <= Mathf.Max(1, ab.rangeTiles);
        }

        /// <summary>
        /// Attempt to use <paramref name="ab"/> from caster against target (and its
        /// area, if any). Returns false without spending anything if it can't be used.
        /// </summary>
        public static bool TryUse(TurnManager turns, GridUnit caster, GridUnit target,
            AbilityDefinition ab, IEnumerable<GridUnit> allUnits)
        {
            if (ab == null || caster == null) return false;
            if (caster.Sheet.IsIncapacitated) { turns.Log($"{caster.Sheet.displayName} is incapacitated."); return false; }

            // Validate target/range before committing resources.
            GridUnit primary = ab.targeting == TargetingMode.Self ? caster : target;
            if (ab.targeting != TargetingMode.Self && primary == null) return false;
            if (!InRange(caster, primary, ab)) { turns.Log($"{ab.abilityName}: target out of range."); return false; }
            if (!CanAfford(turns, caster, ab)) { turns.Log($"{ab.abilityName}: no action/slot available."); return false; }

            // Commit: spend action economy + spell slot.
            switch (ab.cost)
            {
                case ActionCost.Action:      if (!turns.TrySpendAction()) return false; break;
                case ActionCost.BonusAction: if (!turns.TrySpendBonusAction()) return false; break;
            }
            caster.Sheet.spellSlots.Spend(ab.spellSlotLevel);

            // Gather targets.
            var targets = new List<GridUnit>();
            if (ab.targeting == TargetingMode.AreaBurst && primary != null)
            {
                int radius = Mathf.Max(0, ab.areaRadiusTiles);
                foreach (var u in allUnits)
                    if (u != null && u.Sheet.IsAlive &&
                        GridSystem.ManhattanDistance(u.Cell, primary.Cell) <= radius)
                        targets.Add(u);
            }
            else
            {
                targets.Add(primary);
            }

            // Resolve + apply against each.
            foreach (var t in targets)
            {
                if (t == null) continue;
                var result = AttackResolver.Resolve(caster.Sheet, t.Sheet, ab);
                AttackResolver.Apply(t.Sheet, result);
                turns.Log(result.log);
                if (!t.Sheet.IsAlive) turns.Log($"{t.Sheet.displayName} falls!");
            }
            return true;
        }
    }
}
