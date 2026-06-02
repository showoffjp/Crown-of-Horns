using System.Linq;
using UnityEngine;
using SunderedCrown.Grid;

namespace SunderedCrown.Combat
{
    /// <summary>
    /// Mouse control for the active player unit's turn:
    ///   • Left-click an empty tile  → path there, spending movement budget.
    ///   • Left-click an adjacent enemy → attack with the first known ability.
    ///   • Press Space / End Turn button → pass the turn.
    /// Deliberately minimal; replace with your UI's action bar later.
    /// </summary>
    public class PlayerCombatInput : MonoBehaviour
    {
        public Camera worldCamera;
        private TurnManager _turns;
        private GridSystem _grid;

        void Start()
        {
            _turns = TurnManager.Instance;
            _grid = GridSystem.Instance;
            if (worldCamera == null) worldCamera = Camera.main;
        }

        void Update()
        {
            if (_turns == null || !_turns.InCombat) return;

            var active = _turns.ActiveUnit;
            if (active == null || active.IsMoving) return;
            if (active.faction != Faction.Player && active.faction != Faction.Ally) return;

            if (Input.GetKeyDown(KeyCode.Space))
            {
                _turns.NextTurn();
                return;
            }

            if (Input.GetMouseButtonDown(0))
                HandleClick(active);
        }

        private void HandleClick(GridUnit active)
        {
            Vector3 world = worldCamera.ScreenToWorldPoint(Input.mousePosition);
            world.z = 0;
            Vector2Int coord = _grid.WorldToGrid(world);
            var cell = _grid.GetCell(coord);
            if (cell == null) return;

            // Clicked an occupied cell → maybe an attack.
            if (cell.occupant is GridUnit target && target != active)
            {
                if (target.faction == Faction.Enemy &&
                    GridSystem.ManhattanDistance(active.Cell, target.Cell) <= 1 &&
                    _turns.TrySpendAction())
                {
                    var ability = active.Sheet.knownAbilities.FirstOrDefault();
                    if (ability != null)
                    {
                        var result = AttackResolver.Resolve(active.Sheet, target.Sheet, ability);
                        AttackResolver.Apply(target.Sheet, result);
                        _turns.Log(result.log);
                        if (!target.Sheet.IsAlive) _turns.Log($"{target.Sheet.displayName} falls!");
                    }
                }
                return;
            }

            // Clicked an empty walkable cell → move there within budget.
            if (cell.IsFree)
            {
                var path = Pathfinding.FindPath(_grid, active.Cell, cell);
                if (path == null || path.Count == 0) return;
                if (path.Count > _turns.MovementRemaining)
                    path = path.GetRange(0, _turns.MovementRemaining);
                if (path.Count == 0) return;
                _turns.TrySpendMovement(path.Count);
                StartCoroutine(active.FollowPath(path));
            }
        }
    }
}
