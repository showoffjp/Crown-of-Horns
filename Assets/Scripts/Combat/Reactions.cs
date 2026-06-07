using UnityEngine;
using SunderedCrown.Characters;
using SunderedCrown.Grid;
using SunderedCrown.FX;

namespace SunderedCrown.Combat
{
    /// <summary>
    /// Combat reactions — currently the **opportunity attack**. When a unit moves out of melee reach of an
    /// adversary without Disengaging, that adversary may spend its reaction (one per round) to make a free
    /// melee strike. Kept deliberately simple: provocation is checked once, on the *completion* of a move,
    /// by comparing who threatened the start tile versus the end tile — no mid-path interruption. The strike
    /// is resolved via <see cref="AbilityRunner.ApplyOne"/>, which bypasses the action economy (a reaction is
    /// not the reactor's action). Player and enemy movement both route through <see cref="OnMoveCompleted"/>.
    /// </summary>
    public static class Reactions
    {
        private static bool Friendly(Faction f) => f == Faction.Player || f == Faction.Ally;

        /// <summary>Resolve opportunity attacks provoked by <paramref name="mover"/> finishing a move from
        /// <paramref name="startCell"/> to its current cell. No-op if the mover Disengaged.</summary>
        public static void OnMoveCompleted(TurnManager turns, GridUnit mover, GridCell startCell)
        {
            if (turns == null || mover == null || startCell == null || mover.Cell == null) return;
            if (!turns.InCombat) return;
            if (mover.Sheet.IsDisengaging) return;          // Disengage: this move provokes nothing
            if (!mover.Sheet.IsAlive) return;

            GridCell endCell = mover.Cell;

            // Snapshot the turn order: an opportunity attack can drop the mover, so iterate a copy.
            var order = turns.TurnOrder;
            for (int i = 0; i < order.Count; i++)
            {
                var reactor = order[i];
                if (reactor == null || reactor == mover || !reactor.Sheet.IsAlive) continue;
                if (Friendly(reactor.faction) == Friendly(mover.faction)) continue;   // only adversaries react
                if (reactor.Sheet.IsIncapacitated) continue;
                if (reactor.Sheet.lastReactionRound == turns.RoundNumber) continue;   // reaction already spent
                if (reactor.Cell == null) continue;

                bool threatenedStart = GridSystem.ManhattanDistance(reactor.Cell, startCell) <= 1;
                bool threatensEnd    = GridSystem.ManhattanDistance(reactor.Cell, endCell) <= 1;
                if (!threatenedStart || threatensEnd) continue;                       // didn't leave its reach

                var attack = reactor.Sheet.DefaultAttack;
                if (attack == null) continue;

                reactor.Sheet.lastReactionRound = turns.RoundNumber;
                turns.Log($"  ⚡ {reactor.Sheet.displayName} makes an opportunity attack on {mover.Sheet.displayName}!");
                FloatingCombatText.Spawn(reactor.transform.position + Vector3.up * 0.5f, "OPPORTUNITY!", FloatingCombatText.Condition, 11f);
                AbilityRunner.ApplyOne(turns, reactor, mover, attack);

                if (!mover.Sheet.IsAlive) break; // the runaway fell to the blow — stop
            }
        }
    }
}
