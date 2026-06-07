using System.Collections.Generic;
using UnityEngine;
using SunderedCrown.Core;

namespace SunderedCrown.UI
{
    /// <summary>
    /// "The Chronicle of the Returned" — a press-anytime (C) running tally of the whole playthrough:
    /// eras walked, who's still at your side (and who was lost or left), quests, bonds, Lower City
    /// standing, the Lady's riddles, and the endings you've unlocked. Pure read of `EndingResolver.Chronicle`.
    /// Drop on a persistent GameObject; zero setup.
    /// </summary>
    public class ChronicleScreen : MonoBehaviour
    {
        public KeyCode toggleKey = KeyCode.C;
        private bool _open;
        private Vector2 _scroll;

        void Update() { if (Input.GetKeyDown(toggleKey)) _open = !_open; }

        void OnGUI()
        {
            if (!_open) return;

            const float w = 560, h = 480;
            float x = Screen.width / 2f - w / 2f, y = Screen.height / 2f - h / 2f;
            GUILayout.BeginArea(new Rect(x, y, w, h), GUI.skin.box);
            GUILayout.Label($"<size=20><b>📖 THE CHRONICLE OF THE RETURNED</b></size>   <size=11>(press {toggleKey} to close)</size>");
            GUILayout.Label("<color=#c9a0ff><i>The saga so far, as the Lady keeps it.</i></color>");
            GUILayout.Space(8);
            _scroll = GUILayout.BeginScrollView(_scroll);

            List<string> lines = EndingResolver.Chronicle();
            foreach (var line in lines)
            {
                GUILayout.Label("  " + line);
                GUILayout.Space(4);
            }

            // Keepsakes — the small mementos you carry.
            var keepsakes = SunderedCrown.Content.Keepsakes.Earned();
            if (keepsakes.Count > 0)
            {
                GUILayout.Space(10);
                GUILayout.Label($"<b>🎒 Keepsakes</b>   <size=11><color=#888>({keepsakes.Count})</color></size>");
                foreach (var k in keepsakes)
                {
                    GUILayout.Label($"  <b>{k.title}</b> <size=11><color=#9fd>— from {k.owner}</color></size>");
                    GUILayout.Label($"  <size=11><color=#bbb><i>{k.flavor}</i></color></size>");
                    GUILayout.Space(4);
                }
            }

            // Deeds — lightweight achievements derived from the run.
            GUILayout.Space(10);
            GUILayout.Label($"<b>🏆 Deeds</b>   <size=11><color=#888>({SunderedCrown.Content.Deeds.EarnedCount()}/{SunderedCrown.Content.Deeds.Total})</color></size>");
            var gf = GameFlags.Current;
            foreach (var d in SunderedCrown.Content.Deeds.All_)
            {
                bool got = d.earned(gf);
                string mark = got ? "<color=#ffd866>★</color>" : "<color=#555>☆</color>";
                if (got)
                    GUILayout.Label($"  {mark} <b>{d.title}</b> <size=11><color=#9c9>— {d.desc}</color></size>");
                else
                    GUILayout.Label($"  {mark} <color=#777>{d.title}</color>");
            }

            // Foes laid low — the running bestiary tally.
            int slainTotal = gf.GetInt("slain.total");
            if (slainTotal > 0)
            {
                GUILayout.Space(10);
                GUILayout.Label($"<b>⚔️ Foes Laid Low</b>   <size=11><color=#888>({slainTotal})</color></size>");
                foreach (var kv in gf.ints)
                {
                    if (!kv.Key.StartsWith("slain.") || kv.Key == "slain.total" || kv.Value <= 0) continue;
                    GUILayout.Label($"  <color=#cfe>{kv.Key.Substring(6)}</color> <size=11><color=#888>×{kv.Value}</color></size>");
                }
            }

            // Legacy — the cross-run record the Lady keeps between sagas.
            string legacy = EndingsLog.MenuLine();
            if (legacy != null)
            {
                GUILayout.Space(10);
                GUILayout.Label("<b>🕯️ Legacy</b>   <size=11><color=#888>(across all sagas)</color></size>");
                GUILayout.Label($"  <size=12>{legacy}</size>");
            }

            GUILayout.EndScrollView();
            if (GUILayout.Button("Close", GUILayout.Height(30))) _open = false;
            GUILayout.EndArea();
        }
    }
}
