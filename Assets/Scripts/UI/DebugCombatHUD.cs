using System.Collections.Generic;
using System.Text;
using UnityEngine;
using SunderedCrown.Combat;
using SunderedCrown.Grid;

namespace SunderedCrown.UI
{
    /// <summary>
    /// Immediate-mode (OnGUI) HUD so combat is fully legible with ZERO canvas setup:
    /// turn order, the active unit's vitals + action economy, active conditions,
    /// spell slots, a numbered ability bar (showing the armed ability), and a rolling
    /// combat log. Replace with a real uGUI / UI Toolkit HUD when you build art.
    /// </summary>
    public class DebugCombatHUD : MonoBehaviour
    {
        private readonly List<string> _log = new List<string>();
        private TurnManager _turns;
        private PlayerCombatInput _input;
        private Vector2 _scroll;

        void Start()
        {
            _turns = TurnManager.Instance;
            _input = FindFirstObjectByType<PlayerCombatInput>();
            if (_turns != null)
                _turns.OnCombatLog += msg =>
                {
                    _log.Add(msg);
                    if (_log.Count > 200) _log.RemoveAt(0);
                    _scroll.y = float.MaxValue;
                };
        }

        void OnGUI()
        {
            if (_turns == null) return;

            // Combat log (bottom-left).
            GUILayout.BeginArea(new Rect(10, Screen.height - 210, 480, 200), GUI.skin.box);
            GUILayout.Label("<b>Combat Log</b>");
            _scroll = GUILayout.BeginScrollView(_scroll);
            foreach (var line in _log) GUILayout.Label(line);
            GUILayout.EndScrollView();
            GUILayout.EndArea();

            if (!_turns.InCombat) return;
            var active = _turns.ActiveUnit;

            // Active unit panel (top-left).
            GUILayout.BeginArea(new Rect(10, 10, 340, 220), GUI.skin.box);
            if (active != null)
            {
                var s = active.Sheet;
                GUILayout.Label($"<b>{s.displayName}</b>  [{active.faction}]");
                GUILayout.Label($"HP {s.currentHitPoints}/{s.maxHitPoints}   AC {s.ArmorClass}   Move {_turns.MovementRemaining}");
                GUILayout.Label($"Action: {(_turns.ActionAvailable ? "●" : "—")}   Bonus: {(_turns.BonusActionAvailable ? "●" : "—")}");

                // Conditions.
                if (s.activeEffects.Count > 0)
                {
                    var sb = new StringBuilder("Conditions: ");
                    foreach (var e in s.activeEffects)
                        if (e.def != null) sb.Append($"{e.def.effectName}({e.remainingRounds}) ");
                    GUILayout.Label(sb.ToString());
                }

                // Spell slots.
                var slotSb = new StringBuilder();
                for (int lvl = 1; lvl < 10; lvl++)
                    if (s.spellSlots.max[lvl] > 0)
                        slotSb.Append($"L{lvl}:{s.spellSlots.current[lvl]}/{s.spellSlots.max[lvl]}  ");
                if (slotSb.Length > 0) GUILayout.Label("Slots: " + slotSb);

                bool playerControlled = active.faction == Faction.Player || active.faction == Faction.Ally;
                if (playerControlled && GUILayout.Button("End Turn (Space)"))
                    _turns.NextTurn();
            }
            GUILayout.EndArea();

            // Ability bar (bottom-center) for player-controlled units.
            if (active != null && (active.faction == Faction.Player || active.faction == Faction.Ally) && _input != null)
            {
                var known = active.Sheet.knownAbilities;
                float barW = Mathf.Max(220, known.Count * 150);
                GUILayout.BeginArea(new Rect(Screen.width / 2f - barW / 2f, Screen.height - 60, barW, 50), GUI.skin.box);
                GUILayout.BeginHorizontal();
                var armed = _input.SelectedAbility(active);
                for (int i = 0; i < known.Count; i++)
                {
                    var ab = known[i];
                    bool isArmed = ab == armed;
                    bool affordable = AbilityRunner.CanAfford(_turns, active, ab);
                    string slot = ab.spellSlotLevel > 0 ? $" (L{ab.spellSlotLevel})" : "";
                    string label = $"{i + 1}. {(isArmed ? "▶" : "")}{ab.abilityName}{slot}{(affordable ? "" : " ✗")}";
                    GUILayout.Label(label);
                }
                GUILayout.EndHorizontal();
                GUILayout.EndArea();
            }

            // Initiative order (top-right).
            GUILayout.BeginArea(new Rect(Screen.width - 200, 10, 190, 260), GUI.skin.box);
            GUILayout.Label("<b>Initiative</b>");
            foreach (var u in _turns.TurnOrder)
            {
                string marker = (u == active) ? "▶ " : "   ";
                string dead = u.Sheet.IsAlive ? "" : " (down)";
                GUILayout.Label($"{marker}{u.Sheet.displayName} [{u.Sheet.currentHitPoints}]{dead}");
            }
            GUILayout.EndArea();
        }
    }
}
