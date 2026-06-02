using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using SunderedCrown.Grid;

namespace SunderedCrown.Combat
{
    /// <summary>
    /// Drives turn-based combat: rolls initiative, orders combatants, tracks the
    /// per-turn action economy (1 move budget, 1 action, 1 bonus action), and
    /// raises events the UI/AI listen to. Attach one to a "CombatManager" object.
    /// </summary>
    public class TurnManager : MonoBehaviour
    {
        public static TurnManager Instance { get; private set; }

        public bool InCombat { get; private set; }
        public GridUnit ActiveUnit { get; private set; }

        // Action economy for the active unit's current turn.
        public int MovementRemaining { get; private set; }
        public bool ActionAvailable { get; private set; }
        public bool BonusActionAvailable { get; private set; }

        private readonly List<GridUnit> _order = new List<GridUnit>();
        private int _turnIndex = -1;

        // Events — wire UI, audio, camera, and the AI controller to these.
        public event Action OnCombatStarted;
        public event Action OnCombatEnded;
        public event Action<GridUnit> OnTurnStarted;
        public event Action<GridUnit> OnTurnEnded;
        public event Action<string> OnCombatLog;

        void Awake() => Instance = this;

        public void StartCombat(IEnumerable<GridUnit> participants)
        {
            _order.Clear();

            // Roll initiative ONCE per unit and cache it, then sort by the cached
            // value. (Rolling inside the comparator would re-roll on every comparison
            // and produce an inconsistent — even invalid — ordering.)
            var rolled = new List<(GridUnit unit, int init)>();
            foreach (var u in participants.Where(u => u != null && u.Sheet.IsAlive))
                rolled.Add((u, u.Sheet.RollInitiative()));

            // Higher initiative first; ties broken by Dexterity modifier.
            rolled.Sort((a, b) =>
            {
                if (b.init != a.init) return b.init.CompareTo(a.init);
                return b.unit.Sheet.InitiativeModifier.CompareTo(a.unit.Sheet.InitiativeModifier);
            });

            foreach (var r in rolled)
            {
                _order.Add(r.unit);
                Log($"  {r.unit.Sheet.displayName} rolls initiative {r.init}.");
            }

            InCombat = true;
            _turnIndex = -1;
            Log("⚔ Combat begins!");
            OnCombatStarted?.Invoke();
            NextTurn();
        }

        public void NextTurn()
        {
            if (!InCombat) return;

            if (ActiveUnit != null) OnTurnEnded?.Invoke(ActiveUnit);

            if (CheckEndConditions()) { EndCombat(); return; }

            // Advance to the next living combatant.
            for (int guard = 0; guard < _order.Count; guard++)
            {
                _turnIndex = (_turnIndex + 1) % _order.Count;
                ActiveUnit = _order[_turnIndex];
                if (ActiveUnit.Sheet.IsAlive) break;
            }

            BeginTurnFor(ActiveUnit);
        }

        private void BeginTurnFor(GridUnit unit)
        {
            MovementRemaining = unit.Sheet.SpeedTiles;
            ActionAvailable = true;
            BonusActionAvailable = true;
            Log($"▶ {unit.Sheet.displayName}'s turn ({unit.faction}).");
            OnTurnStarted?.Invoke(unit);
        }

        // ---- Action economy spend helpers --------------------------------

        public bool TrySpendMovement(int tiles)
        {
            if (tiles <= 0 || tiles > MovementRemaining) return false;
            MovementRemaining -= tiles;
            return true;
        }

        public bool TrySpendAction()
        {
            if (!ActionAvailable) return false;
            ActionAvailable = false;
            return true;
        }

        public bool TrySpendBonusAction()
        {
            if (!BonusActionAvailable) return false;
            BonusActionAvailable = false;
            return true;
        }

        public void Log(string message) => OnCombatLog?.Invoke(message);

        // ---- Win/loss ----------------------------------------------------

        private bool CheckEndConditions()
        {
            bool anyPlayers = _order.Any(u => u.Sheet.IsAlive &&
                (u.faction == Faction.Player || u.faction == Faction.Ally));
            bool anyEnemies = _order.Any(u => u.Sheet.IsAlive && u.faction == Faction.Enemy);
            return !anyPlayers || !anyEnemies;
        }

        private void EndCombat()
        {
            InCombat = false;
            bool victory = _order.Any(u => u.Sheet.IsAlive &&
                (u.faction == Faction.Player || u.faction == Faction.Ally));
            Log(victory ? "✔ Victory!" : "✘ The party has fallen...");
            ActiveUnit = null;
            OnCombatEnded?.Invoke();
        }

        public IReadOnlyList<GridUnit> TurnOrder => _order;
    }
}
