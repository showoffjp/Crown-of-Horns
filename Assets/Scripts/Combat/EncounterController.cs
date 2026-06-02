using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using SunderedCrown.Grid;

namespace SunderedCrown.Combat
{
    /// <summary>
    /// Glue that turns a scene full of GridUnits into a running encounter. It
    /// collects participants, kicks off the TurnManager, and on each ENEMY turn
    /// runs the AI then advances. PLAYER turns wait for input (PlayerCombatInput
    /// calls TurnManager.NextTurn when the player ends their turn).
    /// </summary>
    public class EncounterController : MonoBehaviour
    {
        [Tooltip("Auto-start combat with every GridUnit in the scene on Start.")]
        public bool autoStartOnPlay = true;

        private TurnManager _turns;

        void Start()
        {
            _turns = TurnManager.Instance;
            _turns.OnTurnStarted += HandleTurnStarted;

            if (autoStartOnPlay)
            {
                var units = FindObjectsByType<GridUnit>(FindObjectsSortMode.None);
                _turns.StartCombat(units);
            }
        }

        void OnDestroy()
        {
            if (_turns != null) _turns.OnTurnStarted -= HandleTurnStarted;
        }

        private void HandleTurnStarted(GridUnit unit)
        {
            // Enemies act automatically; players/allies are driven by input.
            if (unit.faction == Faction.Enemy)
                StartCoroutine(RunEnemyTurn(unit));
        }

        private IEnumerator RunEnemyTurn(GridUnit enemy)
        {
            yield return new WaitForSeconds(0.4f); // small beat so it's readable

            var target = NearestPlayer(enemy);
            if (target == null) { _turns.NextTurn(); yield break; }

            // Move adjacent to the target if not already.
            if (GridSystem.ManhattanDistance(enemy.Cell, target.Cell) > 1)
            {
                var path = Pathfinding.FindPath(GridSystem.Instance, enemy.Cell, target.Cell);
                if (path != null && path.Count > 1)
                {
                    // Stop one tile short of the target and within movement budget.
                    path.RemoveAt(path.Count - 1);
                    int budget = _turns.MovementRemaining;
                    if (path.Count > budget) path = path.GetRange(0, budget);
                    _turns.TrySpendMovement(path.Count);
                    yield return enemy.StartCoroutine(enemy.FollowPath(path));
                }
            }

            // Attack if adjacent and an action remains.
            if (GridSystem.ManhattanDistance(enemy.Cell, target.Cell) <= 1 && _turns.TrySpendAction())
            {
                var ability = enemy.Sheet.knownAbilities.FirstOrDefault();
                if (ability != null)
                {
                    var result = AttackResolver.Resolve(enemy.Sheet, target.Sheet, ability);
                    AttackResolver.Apply(target.Sheet, result);
                    _turns.Log(result.log);
                    if (!target.Sheet.IsAlive) _turns.Log($"{target.Sheet.displayName} falls!");
                }
                yield return new WaitForSeconds(0.4f);
            }

            _turns.NextTurn();
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
