using UnityEngine;
using SunderedCrown.Content;

namespace SunderedCrown.UI
{
    /// <summary>
    /// The Codex (toggle with K). A lore/bestiary compendium that fills in as you witness the saga —
    /// the four masks, the bestiary, the eras. Entries are gated in CodexContent by GameFlags, so this
    /// screen just renders whatever you've earned the right to know. Left rail = categories; click an
    /// entry to read it.
    /// </summary>
    public class CodexScreen : MonoBehaviour
    {
        public KeyCode toggleKey = KeyCode.K;
        private bool _open;
        private int _selected = -1;
        private Vector2 _listScroll, _bodyScroll;

        void Update() { if (Input.GetKeyDown(toggleKey)) _open = !_open; }

        void OnGUI()
        {
            if (!_open) return;
            var known = CodexContent.Known();

            const float w = 720, h = 520;
            float x = Screen.width / 2f - w / 2f, y = Screen.height / 2f - h / 2f;
            GUILayout.BeginArea(new Rect(x, y, w, h), GUI.skin.box);

            GUILayout.Label($"<size=18><b>📖 THE CODEX</b></size>   <color=#aaa>{known.Count}/{CodexContent.TotalCount} discovered</color>   <size=11>(press {toggleKey} to close)</size>");
            GUILayout.Space(6);

            GUILayout.BeginHorizontal();

            // Left rail: the entry list, grouped by category.
            _listScroll = GUILayout.BeginScrollView(_listScroll, GUILayout.Width(250));
            CodexContent.Category? lastCat = null;
            for (int i = 0; i < known.Count; i++)
            {
                var e = known[i];
                if (lastCat != e.category)
                {
                    if (lastCat != null) GUILayout.Space(6);
                    GUILayout.Label($"<b><color=#c9a0ff>{CategoryLabel(e.category)}</color></b>");
                    lastCat = e.category;
                }
                bool sel = i == _selected;
                if (GUILayout.Button((sel ? "▸ " : "   ") + e.title, LeftAligned(sel)))
                    _selected = i;
            }
            GUILayout.EndScrollView();

            // Right pane: the selected entry.
            GUILayout.BeginVertical(GUI.skin.box);
            if (_selected >= 0 && _selected < known.Count)
            {
                var e = known[_selected];
                GUILayout.Label($"<size=15><b>{e.title}</b></size>");
                GUILayout.Label($"<color=#888><i>{CategoryLabel(e.category)}</i></color>");
                GUILayout.Space(8);
                _bodyScroll = GUILayout.BeginScrollView(_bodyScroll);
                GUILayout.Label(e.body);
                GUILayout.EndScrollView();
            }
            else
            {
                GUILayout.FlexibleSpace();
                GUILayout.Label("<color=#888><i>Select an entry. The Codex grows as you witness the saga —\nwalk the eras, meet the masks, read her name.</i></color>");
                GUILayout.FlexibleSpace();
            }
            GUILayout.EndVertical();

            GUILayout.EndHorizontal();
            GUILayout.EndArea();
        }

        private static GUIStyle _left, _leftSel;
        private GUIStyle LeftAligned(bool sel)
        {
            if (_left == null)
            {
                _left = new GUIStyle(GUI.skin.button) { alignment = TextAnchor.MiddleLeft, richText = true };
                _leftSel = new GUIStyle(_left);
                _leftSel.normal.textColor = new Color(1f, 0.85f, 0.4f);
            }
            return sel ? _leftSel : _left;
        }

        private static string CategoryLabel(CodexContent.Category c) => c switch
        {
            CodexContent.Category.Premise  => "The Premise",
            CodexContent.Category.Masks    => "The Four Masks",
            CodexContent.Category.Companions => "The Company",
            CodexContent.Category.Bestiary => "Bestiary",
            CodexContent.Category.Lore     => "Lore & History",
            _ => c.ToString()
        };
    }
}
