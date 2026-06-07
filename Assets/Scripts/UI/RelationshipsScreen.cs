using UnityEngine;
using SunderedCrown.Core;
using SunderedCrown.Content;

namespace SunderedCrown.UI
{
    /// <summary>
    /// The Relationships panel (press L): every companion at a glance — approval bar, furthest romance
    /// stage, rupture state, and their personal-quest beat. Pure read of GameFlags; persistent overlay.
    /// </summary>
    public class RelationshipsScreen : MonoBehaviour
    {
        public KeyCode toggleKey = KeyCode.L;
        private bool _open;
        private Vector2 _scroll;

        private static readonly (string id, string name)[] Cast =
        {
            ("garrow","Sister Garrow"), ("roen","Roen"), ("varra","Varra"),
            ("naeve","Naeve"), ("ilfaeril","Ilfaeril"), ("maerin","Maerin"),
        };
        private static readonly (string key, string label)[] RomanceLadder =
        {
            ("consummated","the last night"), ("choosing","committed"), ("crisis","through the crisis"),
            ("turn","the turn"), ("trust","trust"), ("spark","a spark"),
        };

        void Update() { if (Input.GetKeyDown(toggleKey)) _open = !_open; }

        void OnGUI()
        {
            if (!_open) return;
            var f = GameFlags.Current;

            const float w = 520, h = 540;
            float x = Screen.width / 2f - w / 2f, y = Screen.height / 2f - h / 2f;
            GUILayout.BeginArea(new Rect(x, y, w, h), GUI.skin.box);
            GUILayout.Label($"<size=20><b>❤ RELATIONSHIPS</b></size>   <size=11>(press {toggleKey} to close)</size>");
            GUILayout.Space(6);
            _scroll = GUILayout.BeginScrollView(_scroll);

            int shown = 0;
            foreach (var (id, name) in Cast)
            {
                bool inRun = id == "garrow" || f.GetBool($"companion.{id}.recruited");
                if (!inRun) continue;
                shown++;

                bool lost = f.GetBool($"companion.{id}.lost");
                bool left = f.GetBool($"companion.{id}.left");
                int appr = f.GetInt($"companion.{id}.approval");

                GUILayout.BeginVertical(GUI.skin.box);
                string state = lost ? " <color=#a66>(lost to the Wall)</color>" : left ? " <color=#a66>(walked away)</color>" : "";
                GUILayout.Label($"<b>{name}</b>{state}");

                // approval bar (-100..100)
                float pct = Mathf.Clamp01((appr + 100) / 200f);
                GUILayout.Label($"Approval: <b>{appr}</b>   {Bar(pct)}");

                string rom = RomanceStage(f, id);
                if (rom != null) GUILayout.Label($"  <color=#f9c>♥ Romance: {rom}</color>");
                if (f.GetBool($"rupture.{id}.broken")) GUILayout.Label("  <color=#e88>Bond: broken</color>");
                else if (f.GetBool($"rupture.{id}.uneasy")) GUILayout.Label("  <color=#dca>Bond: uneasy</color>");
                else if (f.GetBool($"rupture.{id}.mended")) GUILayout.Label("  <color=#9c9>Bond: mended</color>");

                if (f.GetBool($"quest.{id}.resolved")) GUILayout.Label("  <color=#9fd>Personal quest: resolved</color>");
                else if (f.GetBool($"quest.{id}.started")) GUILayout.Label("  <color=#cc9>Personal quest: in progress</color>");

                // Keepsakes this companion has pressed on you.
                string capId = char.ToUpper(id[0]) + id.Substring(1);
                foreach (var k in Keepsakes.Earned())
                    if (!string.IsNullOrEmpty(k.owner) && k.owner.Contains(capId))
                        GUILayout.Label($"  <color=#dcb>🎒 {k.title}</color>");

                GUILayout.EndVertical();
                GUILayout.Space(3);
            }
            if (shown == 0) GUILayout.Label("<color=#888><i>No companions yet. The road provides.</i></color>");

            GUILayout.EndScrollView();
            if (GUILayout.Button("Close", GUILayout.Height(30))) _open = false;
            GUILayout.EndArea();
        }

        private static string Bar(float pct)
        {
            int filled = Mathf.RoundToInt(pct * 20);
            string c = pct > 0.66f ? "#6ed96e" : pct > 0.4f ? "#e6cf4d" : "#e0703a";
            return $"<color={c}>{new string('█', filled)}</color><color=#444>{new string('█', 20 - filled)}</color>";
        }

        private static string RomanceStage(GameFlags f, string id)
        {
            foreach (var (key, label) in RomanceLadder)
                if (f.GetBool($"romance.{id}.{key}")) return label;
            return null;
        }
    }
}
