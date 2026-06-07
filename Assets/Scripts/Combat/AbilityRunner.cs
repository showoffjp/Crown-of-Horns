using System.Collections.Generic;
using UnityEngine;
using SunderedCrown.Characters;
using SunderedCrown.FX;
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

            // Cast VFX at the caster (optional).
            if (!string.IsNullOrEmpty(ab.castVfx))
                FxSystem.PlayAt(ab.castVfx, caster.transform.position);

            // Resolve + apply against each.
            foreach (var t in targets)
                ApplyOne(turns, caster, t, ab);

            // The Help action grants advantage on the recipient's NEXT attack roll — consume it now.
            if (ab.isAttackRoll) caster.Sheet.HasHelpAdvantage = false;
            return true;
        }

        /// <summary>Resolve + apply a single ability use against one target, including difficulty scaling,
        /// logging, floating text, SFX and VFX. Does NOT touch the action economy or spell slots — so it is
        /// safe to call for reactions (opportunity attacks) that happen outside the reactor's own turn.</summary>
        public static void ApplyOne(TurnManager turns, GridUnit caster, GridUnit t, AbilityDefinition ab)
        {
            if (t == null || caster == null || ab == null) return;
            bool flanked = IsFlankingMelee(caster, t, ab);
            int cover = HasRangedCover(caster, t, ab) ? 2 : 0;
            var result = AttackResolver.Resolve(caster.Sheet, t.Sheet, ab, flanked, cover);
            if (flanked && result.hit)
            {
                turns.Log($"  ↔ {t.Sheet.displayName} is flanked!");
                FloatingCombatText.Spawn(t.transform.position + Vector3.up * 0.5f, "FLANKED", FloatingCombatText.Condition, 12f);
                var gf = SunderedCrown.Core.GameFlags.Current;
                if (gf != null && (caster.faction == Faction.Player || caster.faction == Faction.Ally))
                    gf.SetBool("combat.used_flank", true);
            }
            if (cover > 0 && !result.hit && ab.isAttackRoll)
                FloatingCombatText.Spawn(t.transform.position + Vector3.up * 0.5f, "COVER", FloatingCombatText.Miss, 11f);
            // Difficulty: scale damage by who's dealing it (enemies → party vs party → enemies).
            if (result.damage > 0 && !result.isHeal)
            {
                float m = t.faction == Faction.Player || t.faction == Faction.Ally
                    ? SunderedCrown.Core.GameSettings.EnemyDamageMult
                    : SunderedCrown.Core.GameSettings.PlayerDamageMult;
                if (m != 1f) result.damage = Mathf.Max(1, Mathf.RoundToInt(result.damage * m));
            }
            int hpBefore = t.Sheet.currentHitPoints;
            AttackResolver.Apply(t.Sheet, result);
            turns.Log(result.log);

            // "Bloodied" — the moment a unit first drops below half health from a blow.
            if (!result.isHeal && result.damage > 0 && t.Sheet.maxHitPoints > 0)
            {
                float half = t.Sheet.maxHitPoints * 0.5f;
                if (hpBefore >= half && t.Sheet.currentHitPoints < half && t.Sheet.IsAlive)
                    FloatingCombatText.Spawn(t.transform.position + Vector3.up * 0.55f, "BLOODIED", FloatingCombatText.Crit, 12f);
            }

            // Floating combat text — the number/word pops off the target (pairs with nameplates).
            Vector3 fp = t.transform.position;
            if (result.isHeal)
            {
                if (result.healing > 0) FloatingCombatText.Spawn(fp, "+" + result.healing, FloatingCombatText.Heal);
            }
            else if (result.damage > 0)
                FloatingCombatText.Spawn(fp, result.critical ? result.damage + "!" : result.damage.ToString(),
                    result.critical ? FloatingCombatText.Crit : FloatingCombatText.Damage, result.critical ? 24f : 17f);
            else if (!result.hit)
                FloatingCombatText.Spawn(fp, ab.isAttackRoll ? "MISS" : "RESIST", FloatingCombatText.Miss, 14f);
            if (result.effectApplied != null)
                FloatingCombatText.Spawn(fp + Vector3.up * 0.3f, result.effectApplied.effectName, FloatingCombatText.Condition, 12f);

            // SFX (art-optional, mirrors the hit VFX): heal / impact-by-type / miss.
            if (result.isHeal) { if (result.healing > 0) AudioSystem.PlaySfx("heal"); }
            else if (result.hit && result.damage > 0) { AudioSystem.PlayHit(ab.damageType); if (result.critical) AudioSystem.PlaySfx("crit"); }
            else if (!result.hit) AudioSystem.PlaySfx("miss");

            // Hit VFX: explicit effect, else auto-pick by damage type.
            if (result.hit && !result.isHeal)
            {
                if (!string.IsNullOrEmpty(ab.hitVfx)) FxSystem.PlayAt(ab.hitVfx, t.transform.position);
                else FxSystem.PlayImpact(ab.damageType, t.transform.position);
            }
            else if (result.isHeal)
            {
                FxSystem.PlayAt(string.IsNullOrEmpty(ab.hitVfx) ? "heal" : ab.hitVfx, t.transform.position);
            }

            if (!t.Sheet.IsAlive)
            {
                turns.Log($"{t.Sheet.displayName} falls!");
                // One-time hint: downed allies aren't dead — a heal brings them back up mid-fight.
                bool friendlyFaller = t.faction == Faction.Player || t.faction == Faction.Ally;
                var gf = SunderedCrown.Core.GameFlags.Current;
                if (friendlyFaller && gf != null && !gf.GetBool("combat.revive_hinted"))
                {
                    gf.SetBool("combat.revive_hinted", true);
                    turns.Log("  (They're down, not dead — heal them to bring them back up, or finish the fight to stabilize them.)");
                }
            }
        }

        private static bool Friendly(Faction f) => f == Faction.Player || f == Faction.Ally;

        /// <summary>Flanking (optional 5e rule): a melee attacker gets advantage when an ally of theirs stands
        /// on the tile directly opposite the attacker across the target. Pure positional check off the grid.</summary>
        private static bool IsFlankingMelee(GridUnit attacker, GridUnit target, AbilityDefinition ab)
        {
            if (ab == null || !ab.isAttackRoll) return false;
            if (Mathf.Max(1, ab.rangeTiles) > 1) return false;            // melee only
            var grid = GridSystem.Instance;
            var ac = attacker.Cell; var tc = target.Cell;
            if (grid == null || ac == null || tc == null) return false;
            if (GridSystem.ManhattanDistance(ac, tc) != 1) return false;  // attacker must be adjacent

            var opposite = grid.GetCell(tc.x + (tc.x - ac.x), tc.y + (tc.y - ac.y));
            return opposite?.occupant is GridUnit mate && mate != attacker && mate.Sheet.IsAlive &&
                   Friendly(mate.faction) == Friendly(attacker.faction);
        }

        /// <summary>Half cover (+2 AC) for a ranged attack when another creature stands on a tile directly
        /// between attacker and target, along an orthogonal or perfectly diagonal line. Simple & deterministic
        /// — no full line-of-sight raycast; just the aligned cases where "a body's in the way" is unambiguous.</summary>
        private static bool HasRangedCover(GridUnit attacker, GridUnit target, AbilityDefinition ab)
        {
            if (ab == null || !ab.isAttackRoll || ab.rangeTiles <= 1) return false; // ranged only
            var grid = GridSystem.Instance;
            var ac = attacker.Cell; var tc = target.Cell;
            if (grid == null || ac == null || tc == null) return false;

            int dx = tc.x - ac.x, dy = tc.y - ac.y;
            bool aligned = dx == 0 || dy == 0 || Mathf.Abs(dx) == Mathf.Abs(dy);
            if (!aligned) return false;
            int steps = Mathf.Max(Mathf.Abs(dx), Mathf.Abs(dy));
            if (steps <= 1) return false; // adjacent: nothing can be between
            int sx = dx == 0 ? 0 : (dx > 0 ? 1 : -1);
            int sy = dy == 0 ? 0 : (dy > 0 ? 1 : -1);
            for (int i = 1; i < steps; i++)
            {
                var mid = grid.GetCell(ac.x + sx * i, ac.y + sy * i);
                if (mid?.occupant is GridUnit blocker && blocker != attacker && blocker != target && blocker.Sheet.IsAlive)
                    return true;
            }
            return false;
        }
    }
}
