using UnityEngine;
using SunderedCrown.Core;

namespace SunderedCrown.UI
{
    /// <summary>
    /// OPTIONS (OnGUI). Two ways to use it: as a modal off the main menu (set <see cref="onClose"/> and
    /// <see cref="startOpen"/>=true → shows a Back button), or as a persistent in-game overlay (leave
    /// onClose null → toggles with <see cref="toggleKey"/>, default O). Edits GameSettings live and saves
    /// on close. Zero setup; drop it on any GameObject.
    /// </summary>
    public class SettingsScreen : MonoBehaviour
    {
        public System.Action onClose;          // set for modal use (main menu); null for in-game toggle
        public KeyCode toggleKey = KeyCode.O;  // in-game open/close (ignored in modal mode)
        public bool startOpen = false;

        private bool _open;
        private bool _confirmWipe;

        void Start() => _open = startOpen;

        void Update()
        {
            if (onClose == null && Input.GetKeyDown(toggleKey)) { _open = !_open; if (!_open) GameSettings.Save(); }
        }

        void OnGUI()
        {
            bool modal = onClose != null;
            if (!modal && !_open) return;

            const float w = 460, h = 680;
            float x = Screen.width / 2f - w / 2f, y = Screen.height / 2f - h / 2f;
            GUILayout.BeginArea(new Rect(x, y, w, h), GUI.skin.box);

            GUILayout.Label("<size=20><b>⚙️ OPTIONS</b></size>");
            GUILayout.Space(10);

            // --- Difficulty ---
            GUILayout.Label("Difficulty");
            GUILayout.BeginHorizontal();
            foreach (GameSettings.Difficulty d in System.Enum.GetValues(typeof(GameSettings.Difficulty)))
            {
                bool on = GameSettings.Mode == d;
                if (GUILayout.Toggle(on, "  " + d, GUILayout.Width(120)) && !on)
                    GameSettings.Mode = d;
            }
            GUILayout.EndHorizontal();
            GUILayout.Label($"<color=#888><i>{GameSettings.DifficultyBlurb}</i></color>");
            GUILayout.Space(12);

            // --- Floating combat text (accessibility) ---
            GameSettings.ShowFloatingText = GUILayout.Toggle(GameSettings.ShowFloatingText,
                "  Floating combat numbers & words" + (GameSettings.ShowFloatingText ? "" : "  <color=#888>(off)</color>"));
            GUILayout.Space(12);

            // --- Camera auto-focus ---
            GameSettings.CameraAutoFocus = GUILayout.Toggle(GameSettings.CameraAutoFocus,
                "  Camera follows the active combatant" + (GameSettings.CameraAutoFocus ? "" : "  <color=#888>(off)</color>"));
            GUILayout.Space(12);

            // --- Ambient banter ---
            GameSettings.BanterEnabled = GUILayout.Toggle(GameSettings.BanterEnabled,
                "  Ambient party banter" + (GameSettings.BanterEnabled ? "" : "  <color=#888>(silenced)</color>"));
            GUILayout.Space(12);

            // --- Master volume ---
            GUILayout.Label($"Master volume — <b>{Mathf.RoundToInt(GameSettings.MasterVolume * 100)}%</b>");
            GameSettings.MasterVolume = GUILayout.HorizontalSlider(GameSettings.MasterVolume, 0f, 1f);
            GameSettings.Apply(); // live preview as you drag
            GUILayout.Space(12);

            // --- Text speed ---
            GameSettings.InstantText = GUILayout.Toggle(GameSettings.InstantText, "  Instant text");
            GUI.enabled = !GameSettings.InstantText;
            GUILayout.Label(GameSettings.InstantText
                ? "<color=#888>Dialogue appears all at once.</color>"
                : $"Text speed — <b>{Mathf.RoundToInt(GameSettings.TextCharsPerSecond)}</b> chars/sec");
            GameSettings.TextCharsPerSecond = Mathf.Round(GUILayout.HorizontalSlider(GameSettings.TextCharsPerSecond, 15f, 120f));
            GUI.enabled = true;
            GUILayout.Space(12);

            // --- UI text size (accessibility) ---
            GUILayout.Label($"UI text size — <b>{Mathf.RoundToInt(GameSettings.UiScale * 100)}%</b>");
            GameSettings.UiScale = Mathf.Round(GUILayout.HorizontalSlider(GameSettings.UiScale, 0.85f, 1.6f) * 20f) / 20f;
            GUILayout.Label($"<color=#888><i>{GameSettings.UiScaleBlurb}</i></color>");
            GUILayout.Space(12);

            // --- Combat speed ---
            GUILayout.Label($"Combat speed — <b>{GameSettings.CombatSpeed:0.0}×</b>");
            GameSettings.CombatSpeed = Mathf.Round(GUILayout.HorizontalSlider(GameSettings.CombatSpeed, 0.5f, 2.5f) * 2f) / 2f;
            GUILayout.Label("<color=#888><i>How quickly enemy turns play out.</i></color>");
            GUILayout.Space(8);

            // --- Re-show the first-combat tutorial hint ---
            if (PlayerPrefs.GetInt("tutorial.combat_seen", 0) == 1 &&
                GUILayout.Button("Show the combat tips again", GUILayout.Height(24)))
            { PlayerPrefs.DeleteKey("tutorial.combat_seen"); PlayerPrefs.Save(); }

            // --- Legacy record (New-Game+ / endings seen across runs) ---
            if (SunderedCrown.Core.EndingsLog.RunsFinished > 0)
            {
                GUILayout.Space(10);
                GUILayout.Label($"<size=11><color=#9ad>{SunderedCrown.Core.EndingsLog.MenuLine()}</color></size>");
                if (!_confirmWipe)
                {
                    if (GUILayout.Button("Reset legacy record…", GUILayout.Height(24))) _confirmWipe = true;
                }
                else
                {
                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button("<color=#f99>Confirm wipe</color>", GUILayout.Height(24)))
                    { SunderedCrown.Core.EndingsLog.Clear(); _confirmWipe = false; }
                    if (GUILayout.Button("Cancel", GUILayout.Height(24))) _confirmWipe = false;
                    GUILayout.EndHorizontal();
                }
            }

            GUILayout.FlexibleSpace();
            if (GUILayout.Button(modal ? "Back" : "Close", GUILayout.Height(34)))
            {
                GameSettings.Save();
                if (modal) onClose?.Invoke();
                else _open = false;
            }
            GUILayout.Label("<size=10><color=#888>In-game, press O to open options at any time.</color></size>");

            GUILayout.EndArea();
        }
    }
}
