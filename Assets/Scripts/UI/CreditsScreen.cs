using UnityEngine;
using SunderedCrown.Core;

namespace SunderedCrown.UI
{
    /// <summary>
    /// Credits / "the company you kept" — a scrollable roll of the cast, the Four Masks, the ages, and a
    /// closing dedication. Modal: set <see cref="onClose"/> and it draws a Back button. Reachable from the
    /// main menu (and offered after the Court of the Dead). Zero setup.
    /// </summary>
    public class CreditsScreen : MonoBehaviour
    {
        public System.Action onClose;
        private Vector2 _scroll;

        void OnGUI()
        {
            const float w = 560, h = 600;
            float x = Screen.width / 2f - w / 2f, y = Screen.height / 2f - h / 2f;
            GUILayout.BeginArea(new Rect(x, y, w, h), GUI.skin.box);
            GUILayout.Label("<size=24><b>⚔️ THE CROWN OF HORNS</b></size>");
            GUILayout.Label("<color=#c9a0ff><i>A Forgotten Realms CRPG · in the lineage of Baldur's Gate II</i></color>");
            GUILayout.Space(8);
            _scroll = GUILayout.BeginScrollView(_scroll);

            Head("The Returned");
            Line("You. You died in the harvest beneath the Gate, and something pulled you back.");

            Head("The Company You Kept");
            Line("<b>Sister Garrow</b> — the Doomguide who buries everyone and believed in keeping no one.");
            Line("<b>Roen Alleywind</b> — the Harper who let exactly one person past the locks.");
            Line("<b>Varra</b> — the warlock who was sold at six and spent her life refusing the price.");
            Line("<b>Naeve</b> — the arcanist whose elegant proof fell a sky.");
            Line("<b>Ilfaeril</b> — the elf who raised his hand once, ten thousand years ago.");
            Line("<b>Maerin</b> — the girl pulled out of the Wall, who was never your reason.");

            Head("The Four Masks");
            Line("<b>Aldric Morn</b> — the father who would tear down death itself.");
            Line("<b>The Unmade</b> — the logical end of a wall built to forget people.");
            Line("<b>The Last Returned</b> — the you from a future that already lost.");
            Line("<b>The Lady in the Margins</b> — the reader who wanted only to see how it ends.");

            Head("The Ages You Walked");
            Line("Netheril · the Crown Wars · the Time of Troubles · the Spellplague · the Court of the Dead.");

            // Your Legacy — endings discovered across all playthroughs (New-Game+ memory).
            if (EndingsLog.RunsFinished > 0)
            {
                Head($"Your Legacy — {EndingsLog.SeenCount}/{EndingsLog.Total} endings discovered");
                foreach (Ending e in System.Enum.GetValues(typeof(Ending)))
                {
                    bool seen = EndingsLog.IsSeen(e);
                    string mark = seen ? "<color=#ffd866>✦</color>" : "<color=#555>✧</color>";
                    Line(seen ? $"{mark} {EndingResolver.Title(e)}" : $"{mark} <color=#666>— undiscovered —</color>");
                }
                if (EndingsLog.GoldenReached) Line("<color=#ffd866><i>★ You have walked a golden road.</i></color>");
            }

            Head("Built With");
            Line("Unity (C#) · D&D 5e (SRD 5.1) · and an indecent amount of love for the genre.");

            GUILayout.Space(12);
            GUILayout.Label("<color=#d8b86a><i>\"You came back. They never come back.\"</i></color>");
            GUILayout.Label("<size=11><color=#888>The whole game was the discovery of why you always do — and what it costs, just once, to stay gone.</color></size>");

            GUILayout.EndScrollView();
            if (GUILayout.Button("Back", GUILayout.Height(32))) onClose?.Invoke();
            GUILayout.EndArea();
        }

        private static void Head(string t) { GUILayout.Space(8); GUILayout.Label($"<b><color=#d8b86a>{t}</color></b>"); }
        private static void Line(string t) { GUILayout.Label("  " + t); }
    }
}
