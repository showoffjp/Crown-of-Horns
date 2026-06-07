using System.Collections.Generic;
using UnityEngine;
using SunderedCrown.Characters;
using SunderedCrown.Core;
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

        // Typewriter reveal (speed from GameSettings).
        private DialogueNode _revealNode;  // node the reveal clock is tracking
        private float _revealAt;           // when the current node started revealing
        private bool _snap;                // player asked to show the whole line now

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

        void Update()
        {
            if (_runner == null || !_runner.IsActive || _node == null) return;
            string text = _node.text ?? "";
            bool complete = RevealedLength(text) >= text.Length;

            if (!complete)
            {
                if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return)) _snap = true;
                return;
            }
            if (_choices != null && _choices.Count > 0)
            {
                for (int i = 0; i < _choices.Count && i < 9; i++)
                    if (Input.GetKeyDown(KeyCode.Alpha1 + i)) { _runner.Choose(_choices[i]); return; }
            }
            else if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
            {
                _runner.Continue();
            }
        }

        void OnGUI()
        {
            if (_runner == null || !_runner.IsActive || _node == null) return;

            const float w = 760, h = 280;
            float x = Screen.width / 2f - w / 2f;
            float y = Screen.height - h - 30;

            // Speaker portrait, to the left of the box — shown only if art for the speaker exists
            // (Resources/Portraits/<speaker> or Sprites/<speaker>); otherwise the box stands alone.
            var portrait = SunderedCrown.Rendering.WorldArt.Portrait(_node.speaker);
            if (portrait != null)
            {
                var panel = new Rect(x - 138, y, 128, 150);
                GUI.Box(panel, GUIContent.none);
                var img = new Rect(panel.x + 8, panel.y + 8, 112, 112);
                var tex = portrait.texture;
                var tr = portrait.textureRect;
                GUI.DrawTextureWithTexCoords(img, tex,
                    new Rect(tr.x / tex.width, tr.y / tex.height, tr.width / tex.width, tr.height / tex.height));
                var cap = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, richText = true, fontSize = 12 };
                GUI.Label(new Rect(panel.x, panel.y + 122, panel.width, 22), $"<color=#d8b86a>{_node.speaker}</color>", cap);
            }

            GUILayout.BeginArea(new Rect(x, y, w, h), GUI.skin.box);

            // Reset the reveal clock whenever a new node is shown.
            if (_node != _revealNode) { _revealNode = _node; _revealAt = Time.time; _snap = false; }

            string full = _node.text ?? "";
            int shown = RevealedLength(full);
            bool complete = shown >= full.Length;

            GUILayout.Label($"<b><color=#d8b86a>{_node.speaker}</color></b>");
            GUILayout.Label(complete ? full : full.Substring(0, shown));
            GUILayout.Space(8);

            if (Time.time < _flashUntil)
                GUILayout.Label($"<color=#9fd>{_flash}</color>");

            if (!complete)
            {
                // Still typing: one click finishes the line (then the real choices appear).
                if (GUILayout.Button("▸  (show the rest)")) _snap = true;
            }
            else if (_choices != null && _choices.Count > 0)
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

        /// <summary>How many characters of the current line are revealed, per the text-speed setting.</summary>
        private int RevealedLength(string text)
        {
            if (_snap || GameSettings.InstantText || GameSettings.TextCharsPerSecond <= 0f) return text.Length;
            int chars = Mathf.FloorToInt((Time.time - _revealAt) * GameSettings.TextCharsPerSecond);
            return Mathf.Clamp(chars, 0, text.Length);
        }
    }
}
