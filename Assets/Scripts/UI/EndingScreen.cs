using System.Collections.Generic;
using UnityEngine;
using SunderedCrown.Core;

namespace SunderedCrown.UI
{
    /// <summary>
    /// THE COURT OF THE DEAD — the finale (OnGUI). Reads the playthrough via EndingResolver, offers the
    /// endings the player has *earned the right to choose* (deeper truths unlock deeper endings), and on
    /// a pick plays the ending's prose plus the BG2-style epilogue slides. Zero setup; drop it in a mode
    /// and it runs.
    /// </summary>
    public class EndingScreen : MonoBehaviour
    {
        public System.Action onLeave;

        private List<Ending> _available;
        private Ending? _chosen;
        private List<string> _epilogue;
        private bool _showChronicle;
        private Vector2 _scroll;

        void Start() { _available = EndingResolver.Available(); }

        void OnGUI()
        {
            const float w = 820;
            float h = Screen.height - 80;
            float x = Screen.width / 2f - w / 2f, y = 40;
            GUILayout.BeginArea(new Rect(x, y, w, h), GUI.skin.box);
            _scroll = GUILayout.BeginScrollView(_scroll);

            if (_chosen == null) DrawChoices();
            else DrawEnding(_chosen.Value);

            GUILayout.EndScrollView();
            GUILayout.EndArea();
        }

        private void DrawChoices()
        {
            GUILayout.Label("<size=20><b><color=#c9a0ff>THE COURT OF THE DEAD</color></b></size>");
            GUILayout.Label(
                "Every thread arrives at once. Aldric with the Crown. Myrkul, ready to shed him. The Unmade, vast " +
                "beneath the floor of the world. Vayle, and the church's last army. Jergal, watching. And the Last " +
                "Returned, come to close the loop. The wheel of all history pauses on you, and on the question it " +
                "has always been asking:\n\n<i>What is a soul worth, that no god ever claimed?</i>\n");
            GUILayout.Label("<b>Choose your answer:</b>");
            GUILayout.Space(6);

            foreach (var e in _available)
            {
                GUILayout.BeginVertical(GUI.skin.box);
                string gold = EndingResolver.IsGolden(e) ? "  <color=#ffd866>★</color>" : "";
                GUILayout.Label($"<b>{EndingResolver.Title(e)}</b>{gold}");
                GUILayout.Label($"<i>{EndingResolver.Choice(e)}</i>");
                if (GUILayout.Button("Choose this"))
                {
                    _chosen = e;
                    _epilogue = EndingResolver.Epilogue(e);
                    GameFlags.Current.SetBool("game.ended", true);
                    GameFlags.Current.SetInt("game.ending", (int)e);
                    EndingsLog.Record(e); // cross-run legacy memory (surfaced on the title screen)
                    _scroll = Vector2.zero;
                }
                GUILayout.EndVertical();
                GUILayout.Space(4);
            }

            if (_available.Count <= 3)
                GUILayout.Label("\n<color=#8a8>(The deeper endings unlock as you understand more — argue the Verdict down in the Crown Wars, and learn the Lady's name in the Vault of Tens.)</color>");
        }

        private void DrawEnding(Ending e)
        {
            GUILayout.Label($"<size=20><b>{EndingResolver.Title(e)}</b></size>");
            GUILayout.Space(8);
            GUILayout.Label(EndingResolver.Prose(e));
            GUILayout.Space(14);
            GUILayout.Label("<b><color=#c9a0ff>— Epilogue —</color></b>");
            GUILayout.Space(4);
            foreach (var slide in _epilogue)
            {
                GUILayout.Label(slide);
                GUILayout.Space(6);
            }
            GUILayout.Space(14);
            GUILayout.Label("<i>“You came back. They never come back.” — The whole game was the discovery of why you always do, " +
                            "and what it costs, just once, to stay gone.</i>");
            GUILayout.Space(12);

            // The Chronicle — the whole run, recapped at the close.
            if (GUILayout.Button(_showChronicle ? "▾ Hide the Chronicle of the Returned" : "▸ Read the Chronicle of the Returned"))
                _showChronicle = !_showChronicle;
            if (_showChronicle)
            {
                GUILayout.Space(4);
                GUILayout.BeginVertical(GUI.skin.box);
                foreach (var line in EndingResolver.Chronicle())
                {
                    GUILayout.Label("  " + line);
                    GUILayout.Space(3);
                }
                GUILayout.EndVertical();
            }

            GUILayout.Space(12);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Reconsider", GUILayout.Width(160))) { _chosen = null; _scroll = Vector2.zero; }
            if (GUILayout.Button("End", GUILayout.Width(160))) onLeave?.Invoke();
            GUILayout.EndHorizontal();
        }
    }
}
