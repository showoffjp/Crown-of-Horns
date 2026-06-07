using UnityEngine;
using SunderedCrown.Content;

namespace SunderedCrown.UI
{
    /// <summary>
    /// A quiet "📖 New Codex entry" toast that fires when the number of unlocked entries grows during play —
    /// so the lore the game reveals as you witness the saga doesn't slip by unread. Polls
    /// <see cref="CodexContent.Known"/> a couple of times a second (cheap at this scale) and shows a brief
    /// top-center banner. Drop one on a persistent object (the campaign director adds it). Zero setup.
    /// </summary>
    public class CodexNotifier : MonoBehaviour
    {
        private int _lastCodex = -1;
        private readonly System.Collections.Generic.HashSet<string> _earned = new System.Collections.Generic.HashSet<string>();
        private bool _baselined;
        private float _nextPoll;
        private float _toastUntil;
        private string _toast;

        void Update()
        {
            if (Time.unscaledTime < _nextPoll) return;
            _nextPoll = Time.unscaledTime + 0.5f;

            var gf = SunderedCrown.Core.GameFlags.Current;
            if (gf == null) return;

            // Find a freshly-earned deed (by id) so we can name it.
            string newDeed = null;
            foreach (var d in Deeds.All_)
            {
                if (d == null || d.earned == null) continue;
                bool now = d.earned(gf);
                if (now && _earned.Add(d.id) && _baselined) newDeed = newDeed ?? d.title;
            }

            int codex = CodexContent.Known().Count;
            if (_lastCodex < 0) { _lastCodex = codex; _baselined = true; return; } // baseline on first poll

            if (newDeed != null)   // deeds take precedence — the rarer, prouder pop
            {
                Show($"<color=#ffd866>🏆 Deed earned — “{newDeed}”</color>");
                SunderedCrown.FX.AudioSystem.PlaySfx("deed"); // art-optional: silent without a clip
            }
            else if (codex > _lastCodex)
                Show(codex - _lastCodex > 1
                    ? $"<color=#c9a0ff>📖 {codex - _lastCodex} new Codex entries — press K to read</color>"
                    : "<color=#c9a0ff>📖 New Codex entry — press K to read</color>");

            _lastCodex = codex;
        }

        private void Show(string msg) { _toast = msg; _toastUntil = Time.unscaledTime + 3f; }

        void OnGUI()
        {
            if (Time.unscaledTime > _toastUntil || string.IsNullOrEmpty(_toast)) return;
            var style = new GUIStyle(GUI.skin.box) { fontSize = 13, alignment = TextAnchor.MiddleCenter, richText = true };
            GUI.Label(new Rect(Screen.width / 2f - 180, 44, 360, 26), _toast, style);
        }
    }
}
