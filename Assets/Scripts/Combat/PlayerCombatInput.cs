using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SunderedCrown.Characters;
using SunderedCrown.Grid;
using SunderedCrown.Items;

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
        public bool HelpPending { get; private set; }
        public bool ShovePending { get; private set; }

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

        /// <summary>Enter Help-targeting mode: the next click on an adjacent ally aids them (HUD button + key T).</summary>
        public void BeginHelp()
        {
            if (_turns != null && _turns.ActionAvailable) { ShovePending = false; HelpPending = true; _turns.Log("🤝 Help: click an adjacent ally."); }
        }

        /// <summary>Enter Shove-targeting mode: the next click on an adjacent enemy shoves them (HUD button + key V).</summary>
        public void BeginShove()
        {
            if (_turns != null && _turns.ActionAvailable) { HelpPending = false; ShovePending = true; _turns.Log("🪨 Shove: click an adjacent enemy."); }
        }

        /// <summary>Quaff the first healing consumable in the party stash on the active unit, spending the
        /// action. HUD button + key Q. Returns false if there's nothing to drink or no action left.</summary>
        public bool TryQuaff()
        {
            var active = _turns != null ? _turns.ActiveUnit : null;
            if (active == null || !_turns.InCombat) return false;
            if (active.faction != Faction.Player && active.faction != Faction.Ally) return false;
            if (!_turns.ActionAvailable) { _turns.Log("No action left to quaff a potion."); return false; }

            var inv = Party.Instance != null ? Party.Instance.inventory : null;
            if (inv == null) return false;
            foreach (var st in inv.stacks)
            {
                var def = ItemDatabase.Get(st.itemId);
                if (def != null && def.kind == ItemKind.Consumable && def.useEffect != null && def.useEffect.isHeal)
                {
                    _turns.TrySpendAction();
                    _turns.Log($"🧪 {active.Sheet.displayName} quaffs {def.displayName}.");
                    AbilityRunner.ApplyOne(_turns, active, active, def.useEffect);
                    inv.Remove(st.itemId, 1);
                    return true;
                }
            }
            _turns.Log("No healing draught in the pack.");
            return false;
        }

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

            // Dodge / Defend (G for Guard — D is the camera pan): spend the action to make attacks
            // against you disadvantaged until your next turn.
            if (Input.GetKeyDown(KeyCode.G)) { _turns.TryDodge(); return; }

            // Dash (F): spend the action for extra movement equal to your speed this turn.
            if (Input.GetKeyDown(KeyCode.F)) { _turns.TryDash(); return; }

            // Disengage (X): spend the action so this turn's movement draws no opportunity attacks.
            if (Input.GetKeyDown(KeyCode.X)) { _turns.TryDisengage(); return; }

            // Help (T): arm help-targeting; the next click on an adjacent ally aids them.
            if (Input.GetKeyDown(KeyCode.T)) { BeginHelp(); return; }

            // Shove (V): arm shove-targeting; the next click on an adjacent enemy pushes them back.
            if (Input.GetKeyDown(KeyCode.V)) { BeginShove(); return; }

            // Quaff (Q): drink a healing potion from the party stash, spending the action.
            if (Input.GetKeyDown(KeyCode.Q)) { TryQuaff(); return; }

            if ((HelpPending || ShovePending) && Input.GetKeyDown(KeyCode.Escape)) { HelpPending = ShovePending = false; return; }

            if (Input.GetMouseButtonDown(0)) HandleClick(active);
        }

        private void HandleClick(GridUnit active)
        {
            Vector3 world = worldCamera.ScreenToWorldPoint(Input.mousePosition);
            world.z = 0;
            var cell = _grid.GetCell(_grid.WorldToGrid(world));
            if (cell == null) return;

            // Help-targeting mode: the click must land on an adjacent living ally (not yourself).
            if (HelpPending)
            {
                HelpPending = false;
                if (cell.occupant is GridUnit ally && ally != active && ally.Sheet.IsAlive &&
                    (ally.faction == Faction.Player || ally.faction == Faction.Ally) &&
                    GridSystem.ManhattanDistance(active.Cell, ally.Cell) <= 1)
                    _turns.TryHelp(ally);
                else
                    _turns.Log("🤝 Help needs an adjacent ally.");
                return;
            }

            // Shove-targeting mode: the click must land on an adjacent enemy with room behind them.
            if (ShovePending)
            {
                ShovePending = false;
                if (cell.occupant is GridUnit foe && foe.faction == Faction.Enemy && foe.Sheet.IsAlive &&
                    GridSystem.ManhattanDistance(active.Cell, foe.Cell) <= 1)
                {
                    if (!_turns.TryShove(active, foe)) _turns.Log("🪨 No room to shove them there.");
                }
                else
                    _turns.Log("🪨 Shove needs an adjacent enemy.");
                return;
            }

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
                var startCell = active.Cell;
                StartCoroutine(MoveThenReact(active, path, startCell));
            }
        }

        /// <summary>Walk the path, then resolve any opportunity attacks the move provoked.</summary>
        private IEnumerator MoveThenReact(GridUnit unit, List<GridCell> path, GridCell startCell)
        {
            yield return StartCoroutine(unit.FollowPath(path));
            Reactions.OnMoveCompleted(_turns, unit, startCell);
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
