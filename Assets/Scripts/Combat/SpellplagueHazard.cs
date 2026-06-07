using System.Collections.Generic;
using UnityEngine;
using SunderedCrown.Grid;
using SunderedCrown.Stats;

namespace SunderedCrown.Combat
{
    /// <summary>
    /// THE SPELLPLAGUE — a *causality-optional* battlefield. In the wound where the Weave shattered,
    /// reality runs like wet ink: each turn it may **swap two combatants' positions** (a "reality
    /// skip"), or lash a random combatant with **blue fire** (Force damage). Deeply disorienting by
    /// design — your careful positioning is never quite safe. Never lethal-on-its-own; never soft-locks.
    ///
    /// Added by EncounterBuilder when spellplagueHazard = true.
    /// </summary>
    public class SpellplagueHazard : MonoBehaviour
    {
        public int graceTurns = 2;
        [Tooltip("Chance per turn of a reality-skip (position swap).")]
        [Range(0f, 1f)] public float skipChance = 0.5f;
        [Tooltip("Chance per turn of a blue-fire lash.")]
        [Range(0f, 1f)] public float burnChance = 0.5f;
        public string blueFireDamage = "1d6";

        private TurnManager _turns;
        private int _turnCount;

        void Start()
        {
            _turns = TurnManager.Instance;
            if (_turns != null) _turns.OnTurnStarted += OnTurnStarted;
        }

        void OnDestroy() { if (_turns != null) _turns.OnTurnStarted -= OnTurnStarted; }

        private void OnTurnStarted(GridUnit unit)
        {
            _turnCount++;
            if (_turnCount < graceTurns) return;
            if (_turnCount == graceTurns) { _turns.Log("⚡ The Weave runs like wet ink — cause no longer waits for effect."); return; }

            if (Random.value < skipChance) RealitySkip();
            if (Random.value < burnChance) BlueFireLash();
        }

        private List<GridUnit> Living()
        {
            var list = new List<GridUnit>();
            foreach (var u in _turns.TurnOrder) if (u != null && u.Sheet.IsAlive) list.Add(u);
            return list;
        }

        private void RealitySkip()
        {
            var living = Living();
            if (living.Count < 2) return;
            var a = living[Random.Range(0, living.Count)];
            GridUnit b;
            do { b = living[Random.Range(0, living.Count)]; } while (b == a);

            var ca = a.Cell; var cb = b.Cell;
            if (ca == null || cb == null) return;
            ca.occupant = null; cb.occupant = null;
            a.PlaceAt(cb); b.PlaceAt(ca);
            _turns.Log($"Reality skips — {a.Sheet.displayName} and {b.Sheet.displayName} change places.");
        }

        private void BlueFireLash()
        {
            var living = Living();
            if (living.Count == 0) return;
            var t = living[Random.Range(0, living.Count)];
            int dmg = Dice.Roll(blueFireDamage);
            t.Sheet.TakeDamage(dmg);
            _turns.Log($"Blue fire licks across {t.Sheet.displayName} — {dmg} force damage.");
            SunderedCrown.FX.FxSystem.PlayImpact(DamageType.Force, t.transform.position);
            if (!t.Sheet.IsAlive) _turns.Log($"{t.Sheet.displayName} unravels into the blue.");
        }
    }
}
