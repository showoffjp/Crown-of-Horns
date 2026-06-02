using System.Collections.Generic;
using UnityEngine;
using SunderedCrown.Characters;
using SunderedCrown.Grid;

namespace SunderedCrown.Combat
{
    /// <summary>
    /// Mouse + hotkey control for the active player unit's turn:
    ///   • Number keys 1..9 → select which known ability is "armed".
    ///   • Left-click an empty tile → path there (spends movement).
    ///   • Left-click a valid target → use the armed ability (attack / heal / spell).
    ///   • Space → end turn.
    /// The armed ability's targeting mode decides what counts as a valid click
    /// (enemy for damage, ally/self for heals, area-burst centers on the click).
    /// </summary>
    public class PlayerCombatInput : MonoBehaviour
    {
        public Camera worldCamera;
        public int SelectedAbilityIndex { get; private set; } = 0;

        private TurnManager _turns;
        private GridSystem _grid;

        void Start()
        {
            _turns = TurnManager.Instance;
            _grid = GridSystem.Instance;
            if (worldCamera == null) worldCamera = Camera.main;
        }

        /// <summary>Arm an ability by index (called by the HUD ability bar buttons).</summary>
        public void Arm(int index) => SelectedAbilityIndex = Mathf.Max(0, index);

        public AbilityDefinition SelectedAbility(GridUnit active)
        {
            if (active == null) return null;
            var known = active.Sheet.knownAbilities;
            if (known.Count == 0) return active.Sheet.DefaultAttack;
            return known[Mathf.Clamp(SelectedAbilityIndex, 0, known.Count - 1)];
        }

        void Update()
        {
            if (_turns == null || !_turns.InCombat) return;

            var active = _turns.ActiveUnit;
            if (active == null || active.IsMoving) return;
            if (active.faction != Faction.Player && active.faction != Faction.Ally) return;

            // Hotkeys 1..9 select abilities.
            for (int i = 0; i < 9; i++)
                if (Input.GetKeyDown(KeyCode.Alpha1 + i) && i < active.Sheet.knownAbilities.Count)
                    SelectedAbilityIndex = i;

            if (Input.GetKeyDown(KeyCode.Space)) { _turns.NextTurn(); return; }

            if (Input.GetMouseButtonDown(0)) HandleClick(active);
        }

        private void HandleClick(GridUnit active)
        {
            Vector3 world = worldCamera.ScreenToWorldPoint(Input.mousePosition);
            world.z = 0;
            var cell = _grid.GetCell(_grid.WorldToGrid(world));
            if (cell == null) return;

            var ability = SelectedAbility(active);

            // Clicked a unit → try to use the armed ability on it.
            if (cell.occupant is GridUnit clicked)
            {
                if (ability != null && IsValidTarget(active, clicked, ability))
                    AbilityRunner.TryUse(_turns, active, clicked, ability, AllUnits());
                return;
            }

            // Self-targeted ability cast on empty ground? Allow self-cast.
            if (ability != null && ability.targeting == TargetingMode.Self)
            {
                AbilityRunner.TryUse(_turns, active, active, ability, AllUnits());
                return;
            }

            // Empty walkable cell → move there within the movement budget.
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

        private bool IsValidTarget(GridUnit caster, GridUnit clicked, AbilityDefinition ab)
        {
            bool clickedIsFriendly = clicked.faction == Faction.Player || clicked.faction == Faction.Ally;
            return ab.targeting switch
            {
                TargetingMode.SingleEnemy => clicked.faction == Faction.Enemy,
                TargetingMode.SingleAlly  => clickedIsFriendly,
                TargetingMode.Self        => clicked == caster,
                TargetingMode.AnyTile     => true,
                TargetingMode.AreaBurst   => true, // burst can be centered on anyone
                _ => false
            };
        }

        private List<GridUnit> AllUnits()
        {
            var list = new List<GridUnit>();
            foreach (var u in _turns.TurnOrder) list.Add(u);
            return list;
        }
    }
}
