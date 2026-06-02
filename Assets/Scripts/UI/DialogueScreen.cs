using System.Collections.Generic;
using UnityEngine;
using SunderedCrown.Characters;
using SunderedCrown.Dialogue;

namespace SunderedCrown.UI
{
    /// <summary>
    /// A zero-setup OnGUI renderer for the DialogueRunner: shows the current speaker
    /// and line, lists available choices as buttons (with skill-check DCs), and reports
    /// check results. Replace with a styled uGUI panel later — the DialogueRunner is
    /// UI-agnostic. Set PlayerSpeaker so skill checks use the hero's real modifiers.
    /// </summary>
    public class DialogueScreen : MonoBehaviour
    {
        public CharacterSheet playerSpeaker;

        [Tooltip("If set, this conversation auto-starts once the screen is ready.")]
        public DialogueGraph autoPlay;

        /// <summary>Invoked when the active conversation ends (e.g. to start combat).</summary>
        public System.Action onFinished;

        private DialogueRunner _runner;
        private DialogueNode _node;
        private List<DialogueChoice> _choices = new List<DialogueChoice>();
        private string _flash; // last skill-check result line
        private float _flashUntil;

        void Start()
        {
            _runner = DialogueRunner.Instance;
            if (_runner == null)
            {
                var go = new GameObject("DialogueRunner");
                _runner = go.AddComponent<DialogueRunner>();
            }
            _runner.ResolvePlayerSpeaker = () => playerSpeaker;
            _runner.OnNodeShown += (node, choices) => { _node = node; _choices = choices; };
            _runner.OnConversationEnded += () => { _node = null; _choices = null; onFinished?.Invoke(); };
            _runner.OnSkillCheck += (ability, dc, roll, ok) =>
            {
                _flash = $"{ability} check: rolled {roll} vs DC {dc} — {(ok ? "SUCCESS" : "FAILURE")}";
                _flashUntil = Time.time + 3f;
            };

            // Subscribed first, now safe to auto-start the conversation.
            if (autoPlay != null) _runner.Begin(autoPlay);
        }

        void OnGUI()
        {
            if (_runner == null || !_runner.IsActive || _node == null) return;

            const float w = 760, h = 280;
            float x = Screen.width / 2f - w / 2f;
            float y = Screen.height - h - 30;
            GUILayout.BeginArea(new Rect(x, y, w, h), GUI.skin.box);

            GUILayout.Label($"<b><color=#d8b86a>{_node.speaker}</color></b>");
            GUILayout.Label(_node.text);
            GUILayout.Space(8);

            if (Time.time < _flashUntil)
                GUILayout.Label($"<color=#9fd>{_flash}</color>");

            if (_choices != null && _choices.Count > 0)
            {
                for (int i = 0; i < _choices.Count; i++)
                {
                    var c = _choices[i];
                    string dc = c.checkDC > 0 ? $"  [{c.checkAbility} DC {c.checkDC}]" : "";
                    if (GUILayout.Button($"{i + 1}. {c.text}{dc}"))
                        _runner.Choose(c);
                }
            }
            else
            {
                if (GUILayout.Button("Continue ▸")) _runner.Continue();
            }

            GUILayout.EndArea();
        }
    }
}
