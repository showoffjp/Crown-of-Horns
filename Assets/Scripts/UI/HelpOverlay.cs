using UnityEngine;

namespace SunderedCrown.UI
{
    /// <summary>
    /// A press-anytime help card (toggle with H) listing the controls and the hotkey screens. Drop it on a
    /// persistent GameObject — like the campaign director — and it's available across every mode. Zero setup.
    /// </summary>
    public class HelpOverlay : MonoBehaviour
    {
        public KeyCode toggleKey = KeyCode.H;
        private bool _open;
        private Vector2 _scroll;

        void Update() { if (Input.GetKeyDown(toggleKey)) _open = !_open; }

        void OnGUI()
        {
            if (!_open) return;

            const float w = 540, h = 460;
            float x = Screen.width / 2f - w / 2f, y = Screen.height / 2f - h / 2f;
            GUILayout.BeginArea(new Rect(x, y, w, h), GUI.skin.box);
            GUILayout.Label($"<size=20><b>❔ HELP & CONTROLS</b></size>   <size=11>(press {toggleKey} to close)</size>");
            GUILayout.Space(8);
            _scroll = GUILayout.BeginScrollView(_scroll);

            Section("In the world");
            Row("Click a tile", "Walk your leader there");
            Row("Click an NPC / marker", "Talk, examine, loot, or take an exit");
            Row("E", "Interact with what you're standing on");
            Row("1 – 9 / Space (in dialogue)", "Pick a choice / finish the line & continue");
            Row("Mouse wheel / WASD / edges", "Zoom & pan the camera");

            Section("In battle");
            Row("1 – 9", "Arm an ability (then click a target)");
            Row("Click a tile", "Move (spends movement)");
            Row("Click an enemy", "Attack with the armed ability");
            Row("G", "Defend (Dodge) — attacks against you have disadvantage until your next turn");
            Row("F", "Dash — spend your action for extra movement equal to your speed");
            Row("T", "Help — then click an adjacent ally; their next attack has advantage");
            Row("X", "Disengage — your move this turn draws no opportunity attacks");
            Row("V", "Shove — then click an adjacent enemy; a Strength check pushes them back a tile");
            Row("Q", "Quaff — drink a healing potion from the party stash (uses your action)");
            Row("Space", "End your turn");

            Section("Screens (toggle keys)");
            Row("J", "Journey — the saga, quest beats, bonds & reputation");
            Row("C", "Chronicle — a running tally of your whole playthrough");
            Row("P", "Party — bench/field, companion sheets & approval");
            Row("L", "Relationships — approval, romance, rupture & quest per companion");
            Row("K", "Codex — lore & bestiary, fills in as you witness it");
            Row("I", "Inventory");
            Row("O", "Options — difficulty, banter, volume, text speed, UI size, combat speed");
            Row("B", "Mute / unmute ambient party banter");
            Row("N", "Toggle floating nameplates & HP bars");
            Row("H", "This help card");

            Section("Saving");
            Row("F5 / F9", "Quick save / quick load");
            Row("Long rest at camp", "Autosaves (a natural checkpoint)");

            GUILayout.Space(8);
            GUILayout.Label("<color=#888><i>The whole game is colored cubes + these panels for now — the systems made " +
                            "visible, ready for art. Everything you do flows through the dialogue boxes and the grid.</i></color>");

            GUILayout.EndScrollView();
            if (GUILayout.Button("Close", GUILayout.Height(30))) _open = false;
            GUILayout.EndArea();
        }

        private static void Section(string title)
        {
            GUILayout.Space(6);
            GUILayout.Label($"<b><color=#d8b86a>{title}</color></b>");
        }

        private static void Row(string key, string desc)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label($"<b>{key}</b>", GUILayout.Width(190));
            GUILayout.Label(desc);
            GUILayout.EndHorizontal();
        }
    }
}
