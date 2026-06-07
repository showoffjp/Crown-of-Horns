using UnityEngine;
using SunderedCrown.Content;
using SunderedCrown.Core;

namespace SunderedCrown.UI
{
    /// <summary>
    /// Idle party chatter. While you wander, every so often a companion says something — drawn from
    /// PartyBanter, filtered to who's actually in the field. The line fades in, lingers, fades out, low
    /// in the frame so it never blocks play. Toggle the whole feature with B if you want silence.
    /// </summary>
    public class AmbientBanter : MonoBehaviour
    {
        public float minGap = 22f, maxGap = 40f, holdTime = 6f;
        public KeyCode muteKey = KeyCode.B;

        private System.Random _rng = new System.Random();
        private float _nextAt;
        private string _line;
        private float _shownAt = -999f;
        private bool _muted;

        void Start() => _nextAt = Time.time + Random.Range(minGap, maxGap);

        void Update()
        {
            if (Input.GetKeyDown(muteKey)) { _muted = !_muted; if (_muted) _line = null; }
            if (_muted || !GameSettings.BanterEnabled) { _line = null; return; }
            if (Time.time >= _nextAt)
            {
                var pick = PartyBanter.Pick(_rng);
                if (pick != null) { _line = pick.text; _shownAt = Time.time; }
                _nextAt = Time.time + Random.Range(minGap, maxGap);
            }
        }

        void OnGUI()
        {
            if (string.IsNullOrEmpty(_line)) return;
            float age = Time.time - _shownAt;
            if (age > holdTime) { _line = null; return; }

            // Triangular alpha: fade in over 0.6s, fade out over the last 1.2s.
            float a = Mathf.Min(age / 0.6f, Mathf.Min(1f, (holdTime - age) / 1.2f));
            var prev = GUI.color;
            GUI.color = new Color(1f, 1f, 1f, Mathf.Clamp01(a));

            var style = new GUIStyle(GUI.skin.label)
            { alignment = TextAnchor.MiddleCenter, richText = true, fontSize = 13, wordWrap = true };
            style.normal.textColor = new Color(0.85f, 0.82f, 0.95f);

            float w = Mathf.Min(720, Screen.width - 80);
            var r = new Rect(Screen.width / 2f - w / 2f, Screen.height - 96, w, 60);
            GUI.Label(r, $"<i>{_line}</i>", style);
            GUI.color = prev;
        }
    }
}
