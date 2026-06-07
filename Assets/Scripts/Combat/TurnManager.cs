using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using SunderedCrown.Grid;
using SunderedCrown.Stats;
using SunderedCrown.Core;
using SunderedCrown.FX;

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
        public int RoundNumber { get; private set; }

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
                // Treat the start of combat like a short rest: refresh spell slots.
                r.unit.Sheet.spellSlots.RestoreAll();
                // Clear any combat-transient state lingering from a previous encounter, so round 1 is clean.
                r.unit.Sheet.IsDodging = false;
                r.unit.Sheet.IsDisengaging = false;
                r.unit.Sheet.HasHelpAdvantage = false;
                r.unit.Sheet.lastReactionRound = 0;
                Log($"  {r.unit.Sheet.displayName} rolls initiative {r.init}.");
            }

            InCombat = true;
            _turnIndex = -1;
            RoundNumber = 1;
            Log("⚔ Combat begins!  — Round 1 —");
            OnCombatStarted?.Invoke();
            NextTurn();
        }

        public void NextTurn()
        {
            if (!InCombat) return;

            if (ActiveUnit != null)
            {
                // End-of-turn: count down status effect durations.
                ActiveUnit.Sheet.TickEndOfTurn();
                OnTurnEnded?.Invoke(ActiveUnit);
            }

            if (CheckEndConditions()) { EndCombat(); return; }

            // Advance to the next living combatant. If the index wraps back to/before where we were, a new
            // round has begun.
            int prev = _turnIndex;
            for (int guard = 0; guard < _order.Count; guard++)
            {
                _turnIndex = (_turnIndex + 1) % _order.Count;
                ActiveUnit = _order[_turnIndex];
                if (ActiveUnit.Sheet.IsAlive) break;
            }
            if (_turnIndex <= prev) { RoundNumber++; Log($"  — Round {RoundNumber} —"); }

            BeginTurnFor(ActiveUnit);
        }

        private void BeginTurnFor(GridUnit unit)
        {
            // A Dodge or Disengage taken last turn lasts only until the start of this unit's next turn.
            unit.Sheet.IsDodging = false;
            unit.Sheet.IsDisengaging = false;

            // Start-of-turn: apply damage-over-time (Burning, Poisoned-as-DoT, etc.).
            int dot = unit.Sheet.TickStartOfTurn();
            if (dot > 0)
            {
                Log($"  {unit.Sheet.displayName} takes {dot} damage from ongoing effects.");
                if (!unit.Sheet.IsAlive) { Log($"{unit.Sheet.displayName} succumbs!"); NextTurn(); return; }
            }

            MovementRemaining = unit.Sheet.SpeedTiles;
            ActionAvailable = !unit.Sheet.IsIncapacitated;
            BonusActionAvailable = !unit.Sheet.IsIncapacitated;

            if (unit.Sheet.IsIncapacitated)
            {
                MovementRemaining = 0;
                Log($"▶ {unit.Sheet.displayName} is incapacitated and loses the turn.");
            }
            else
            {
                Log($"▶ {unit.Sheet.displayName}'s turn ({unit.faction}).");
            }
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

        /// <summary>The Dodge action: spend your action to defend. Until your next turn, attack rolls against
        /// you are made at disadvantage. You may still move afterward. Returns false if no action remains.</summary>
        public bool TryDodge()
        {
            if (ActiveUnit == null || !TrySpendAction()) return false;
            ActiveUnit.Sheet.IsDodging = true; MarkUsed("dodge"); Floater(ActiveUnit, "Defend");
            Log($"  🛡 {ActiveUnit.Sheet.displayName} takes the Dodge — attacks against them have disadvantage until their next turn.");
            return true;
        }

        /// <summary>The Shove action: spend your action to push an adjacent enemy one tile directly away, if
        /// that tile is open. A contested Strength check decides success. Returns false (no spend) if the
        /// shove is impossible (not adjacent, blocked destination, no action).</summary>
        public bool TryShove(GridUnit shover, GridUnit target)
        {
            if (ActiveUnit == null || shover == null || target == null || shover == target) return false;
            if (!target.Sheet.IsAlive || shover.Cell == null || target.Cell == null) return false;
            if (GridSystem.ManhattanDistance(shover.Cell, target.Cell) != 1) return false;

            int dx = target.Cell.x - shover.Cell.x, dy = target.Cell.y - shover.Cell.y;
            var grid = GridSystem.Instance;
            var dest = grid != null ? grid.GetCell(target.Cell.x + dx, target.Cell.y + dy) : null;
            if (dest == null || !dest.IsFree) return false; // nowhere to push them → not a legal shove

            if (!TrySpendAction()) return false;
            MarkUsed("shove");
            int atk = Dice.D20() + shover.Sheet.Modifier(Ability.Strength);
            int def = Dice.D20() + target.Sheet.Modifier(Ability.Strength);
            if (atk >= def)
            {
                target.PlaceAt(dest); Floater(target, "Shoved");
                Log($"  🪨 {shover.Sheet.displayName} shoves {target.Sheet.displayName} back a step! ({atk} vs {def})");
            }
            else
            {
                Log($"  🪨 {shover.Sheet.displayName}'s shove fails — {target.Sheet.displayName} holds their ground. ({atk} vs {def})");
            }
            return true;
        }

        /// <summary>The Disengage action: spend your action so that moving this turn provokes no opportunity
        /// attacks. Returns false if no action remains.</summary>
        public bool TryDisengage()
        {
            if (ActiveUnit == null || !TrySpendAction()) return false;
            ActiveUnit.Sheet.IsDisengaging = true; MarkUsed("disengage"); Floater(ActiveUnit, "Disengage");
            Log($"  🥾 {ActiveUnit.Sheet.displayName} Disengages — moving this turn draws no opportunity attacks.");
            return true;
        }

        /// <summary>The Help action: spend your action to aid an ally; their next attack roll has advantage.
        /// Returns false if no action remains or the ally is invalid.</summary>
        public bool TryHelp(GridUnit ally)
        {
            if (ActiveUnit == null || ally == null || ally == ActiveUnit) return false;
            if (!ally.Sheet.IsAlive) return false;
            if (!TrySpendAction()) return false;
            ally.Sheet.HasHelpAdvantage = true; MarkUsed("help"); Floater(ally, "Helped");
            Log($"  🤝 {ActiveUnit.Sheet.displayName} Helps {ally.Sheet.displayName} — their next attack has advantage.");
            return true;
        }

        /// <summary>The Dash action: spend your action to gain extra movement equal to your speed this turn.
        /// Returns false if no action remains.</summary>
        public bool TryDash()
        {
            if (ActiveUnit == null || !TrySpendAction()) return false;
            int gained = ActiveUnit.Sheet.SpeedTiles;
            MovementRemaining += gained; MarkUsed("dash"); Floater(ActiveUnit, "Dash");
            Log($"  💨 {ActiveUnit.Sheet.displayName} Dashes — +{gained} movement this turn ({MovementRemaining} remaining).");
            return true;
        }

        public void Log(string message) => OnCombatLog?.Invoke(message);

        /// <summary>Record (for the meta-layer / deeds) that the player used a tactical action this run.</summary>
        private static void MarkUsed(string key)
        {
            var gf = GameFlags.Current;
            if (gf != null) gf.SetBool("combat.used_" + key, true);
        }

        /// <summary>Pop a small floating word over a unit — feedback for the tactical (non-damage) actions.</summary>
        private static void Floater(GridUnit u, string text)
        {
            if (u != null) FloatingCombatText.Spawn(u.transform.position + Vector3.up * 0.4f, text, FloatingCombatText.Condition, 13f);
        }

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
