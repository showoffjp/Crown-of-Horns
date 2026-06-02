using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using SunderedCrown.Characters;
using SunderedCrown.Grid;

namespace SunderedCrown.Combat
{
    /// <summary>
    /// Turns a scene of GridUnits into a running encounter. Collects participants,
    /// starts the TurnManager, and on each ENEMY turn runs a simple AI: pick an
    /// ability, close to within its range, then use it via AbilityRunner. Player and
    /// ally turns wait for input.
    /// </summary>
    public class EncounterController : MonoBehaviour
    {
        public bool autoStartOnPlay = true;
        private TurnManager _turns;

        void Start()
        {
            _turns = TurnManager.Instance;
            _turns.OnTurnStarted += HandleTurnStarted;
            if (autoStartOnPlay)
                _turns.StartCombat(FindObjectsByType<GridUnit>(FindObjectsSortMode.None));
        }

        void OnDestroy()
        {
            if (_turns != null) _turns.OnTurnStarted -= HandleTurnStarted;
        }

        private void HandleTurnStarted(GridUnit unit)
        {
            if (unit.faction == Faction.Enemy)
                StartCoroutine(RunEnemyTurn(unit));
        }

        private IEnumerator RunEnemyTurn(GridUnit enemy)
        {
            yield return new WaitForSeconds(0.4f);

            // Incapacitated or no targets → pass.
            if (enemy.Sheet.IsIncapacitated) { _turns.NextTurn(); yield break; }
            var target = NearestPlayer(enemy);
            var ability = ChooseAttack(enemy);
            if (target == null || ability == null) { _turns.NextTurn(); yield break; }

            int range = Mathf.Max(1, ability.rangeTiles);

            // Close the distance until within the ability's range (movement budget permitting).
            if (GridSystem.ManhattanDistance(enemy.Cell, target.Cell) > range)
            {
                var path = Pathfinding.FindPath(GridSystem.Instance, enemy.Cell, target.Cell);
                if (path != null && path.Count > 0)
                {
                    // Stop as soon as we're in range; never path onto the target's tile.
                    int stopAt = path.Count;
                    for (int i = 0; i < path.Count; i++)
                        if (GridSystem.ManhattanDistance(path[i], target.Cell) <= range) { stopAt = i + 1; break; }
                    if (stopAt > 0 && path[stopAt - 1] == target.Cell) stopAt--;

                    var trimmed = path.GetRange(0, Mathf.Min(stopAt, Mathf.Min(path.Count, _turns.MovementRemaining)));
                    if (trimmed.Count > 0)
                    {
                        _turns.TrySpendMovement(trimmed.Count);
                        yield return enemy.StartCoroutine(enemy.FollowPath(trimmed));
                    }
                }
            }

            // Attack if now in range.
            if (AbilityRunner.InRange(enemy, target, ability))
                AbilityRunner.TryUse(_turns, enemy, target, ability, _turns.TurnOrder.ToList());

            yield return new WaitForSeconds(0.4f);
            _turns.NextTurn();
        }

        /// <summary>Pick the enemy's best damaging ability it can afford (skip heals/self).</summary>
        private AbilityDefinition ChooseAttack(GridUnit enemy)
        {
            foreach (var ab in enemy.Sheet.knownAbilities)
                if (ab != null && !ab.isHeal && ab.targeting != TargetingMode.Self &&
                    AbilityRunner.CanAfford(_turns, enemy, ab))
                    return ab;
            return enemy.Sheet.DefaultAttack;
        }

        private GridUnit NearestPlayer(GridUnit from)
        {
            GridUnit best = null;
            int bestDist = int.MaxValue;
            foreach (var u in _turns.TurnOrder)
            {
                if (!u.Sheet.IsAlive) continue;
                if (u.faction != Faction.Player && u.faction != Faction.Ally) continue;
                int d = GridSystem.ManhattanDistance(from.Cell, u.Cell);
                if (d < bestDist) { bestDist = d; best = u; }
            }
            return best;
        }
    }
}
