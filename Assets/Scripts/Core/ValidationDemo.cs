using UnityEngine;

namespace SunderedCrown.Core
{
    /// <summary>
    /// ONE-CLICK CONTENT SMOKE TEST. Drop on an empty GameObject and press Play: runs `ContentValidator`
    /// over every authored DialogueGraph and prints a PASS/FAIL report to the Console — every broken node
    /// reference, duplicate id, or missing start node, with the exact graph and node. Run it after editing
    /// dialogue to catch typos the compiler can't. Also shows the result on-screen.
    /// </summary>
    public class ValidationDemo : MonoBehaviour
    {
        private string _summary = "Running…";
        private string _balance = "";
        private System.Collections.Generic.List<string> _issues;

        void Start()
        {
            _balance = SunderedCrown.Combat.CombatBalance.Report();
            Debug.Log("[CombatBalance] " + _balance);
            _issues = ContentValidator.Validate(out int graphs, out int nodes);
            if (_issues.Count == 0)
            {
                _summary = $"✅ CONTENT OK — {graphs} dialogue graphs, {nodes} nodes, no broken references.";
                Debug.Log("[ContentValidator] " + _summary);
            }
            else
            {
                _summary = $"❌ {_issues.Count} ISSUE(S) across {graphs} graphs / {nodes} nodes:";
                Debug.LogWarning("[ContentValidator] " + _summary);
                foreach (var issue in _issues) Debug.LogWarning("  • " + issue);
            }
        }

        private Vector2 _scroll;
        void OnGUI()
        {
            const float w = 720, h = 420;
            GUILayout.BeginArea(new Rect(Screen.width / 2f - w / 2f, 40, w, h), GUI.skin.box);
            GUILayout.Label("<size=18><b>🧪 CONTENT VALIDATION</b></size>");
            GUILayout.Label(_summary);
            if (!string.IsNullOrEmpty(_balance)) GUILayout.Label("<size=12><color=#9ad>" + _balance + "</color></size>");
            GUILayout.Space(6);
            if (_issues != null && _issues.Count > 0)
            {
                _scroll = GUILayout.BeginScrollView(_scroll);
                foreach (var issue in _issues) GUILayout.Label("• " + issue);
                GUILayout.EndScrollView();
            }
            else
            {
                GUILayout.Label("<color=#8f8>Every authored conversation's links resolve. Safe to ship.</color>");
            }
            GUILayout.EndArea();
        }
    }
}
