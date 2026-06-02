using System.Collections.Generic;
using UnityEngine;
using SunderedCrown.Combat;
using SunderedCrown.Grid;

namespace SunderedCrown.UI
{
    /// <summary>
    /// Immediate-mode (OnGUI) heads-up display so combat is legible with ZERO
    /// canvas setup. Shows the turn order, the active unit's action economy, a
    /// rolling combat log, and an End Turn button. Replace with a proper uGUI /
    /// UI Toolkit HUD when you build art — but this is enough to play and debug.
    /// </summary>
    public class DebugCombatHUD : MonoBehaviour
    {
        private readonly List<string> _log = new List<string>();
        private TurnManager _turns;
        private Vector2 _scroll;

        void Start()
        {
            _turns = TurnManager.Instance;
            if (_turns != null)
            {
                _turns.OnCombatLog += msg =>
                {
                    _log.Add(msg);
                    if (_log.Count > 200) _log.RemoveAt(0);
                    _scroll.y = float.MaxValue;
                };
            }
        }

        void OnGUI()
        {
            if (_turns == null) return;

            // ---- Combat log (bottom-left) ----
            GUILayout.BeginArea(new Rect(10, Screen.height - 210, 460, 200), GUI.skin.box);
            GUILayout.Label("<b>Combat Log</b>");
            _scroll = GUILayout.BeginScrollView(_scroll);
            foreach (var line in _log) GUILayout.Label(line);
            GUILayout.EndScrollView();
            GUILayout.EndArea();

            if (!_turns.InCombat) return;

            // ---- Active unit panel (top-left) ----
            var active = _turns.ActiveUnit;
            GUILayout.BeginArea(new Rect(10, 10, 320, 150), GUI.skin.box);
            if (active != null)
            {
                var s = active.Sheet;
                GUILayout.Label($"<b>{s.displayName}</b>  [{active.faction}]");
                GUILayout.Label($"HP {s.currentHitPoints}/{s.maxHitPoints}   AC {s.ArmorClass}");
                GUILayout.Label($"Move left: {_turns.MovementRemaining} tiles");
                GUILayout.Label($"Action: {(_turns.ActionAvailable ? "●" : "—")}   " +
                                $"Bonus: {(_turns.BonusActionAvailable ? "●" : "—")}");

                bool playerControlled = active.faction == Faction.Player || active.faction == Faction.Ally;
                if (playerControlled && GUILayout.Button("End Turn (Space)"))
                    _turns.NextTurn();
            }
            GUILayout.EndArea();

            // ---- Initiative order (top-right) ----
            GUILayout.BeginArea(new Rect(Screen.width - 200, 10, 190, 240), GUI.skin.box);
            GUILayout.Label("<b>Initiative</b>");
            foreach (var u in _turns.TurnOrder)
            {
                string marker = (u == active) ? "▶ " : "   ";
                string dead = u.Sheet.IsAlive ? "" : " (down)";
                GUILayout.Label($"{marker}{u.Sheet.displayName}{dead}");
            }
            GUILayout.EndArea();
        }
    }
}
