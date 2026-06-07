using UnityEngine;

namespace SunderedCrown.Core
{
    /// <summary>
    /// THE FRONT END. Drop this on an empty GameObject in your boot scene and press Play: a title
    /// screen with New Game / Continue / Quit. "New Game" launches CampaignBootstrap fresh; "Continue"
    /// loads the autosaved campaign (the hero and roster are rebuilt from the save). Zero setup.
    /// </summary>
    public class MainMenu : MonoBehaviour
    {
        private const string SaveSlot = "save";
        private SunderedCrown.UI.SettingsScreen _settings;
        private SunderedCrown.UI.SaveSlotScreen _slots;
        private SunderedCrown.UI.CreditsScreen _credits;

        private static readonly string[] Epigraphs =
        {
            "“What is a soul worth, that no god ever claimed?”",
            "“The dead do not come back. You did.”",
            "“The Wall was not handed down. People made it. What people made, people can refuse.”",
            "“To let someone matter is to hand the cosmos a lever. Love anyway.”",
            "“You came back. They never come back.”",
            "“Even gods get buried. I find that the most comforting thing I have ever seen.”",
        };
        private string _epigraph;

        void OnGUI()
        {
            if (_settings != null || _slots != null || _credits != null) return; // a modal is up — let it draw alone

            const float w = 480, h = 460;
            float x = Screen.width / 2f - w / 2f, y = Screen.height / 2f - h / 2f;
            GUILayout.BeginArea(new Rect(x, y, w, h), GUI.skin.box);

            GUILayout.Space(8);
            GUILayout.Label("<size=28><b>⚔️ THE CROWN OF HORNS</b></size>");
            GUILayout.Label("<i>A Forgotten Realms CRPG · D&D 5e</i>");
            GUILayout.Space(6);
            if (_epigraph == null) _epigraph = Epigraphs[Random.Range(0, Epigraphs.Length)];
            GUILayout.Label($"<color=#c9a0ff><i>{_epigraph}</i></color>");

            string legacy = EndingsLog.MenuLine();
            if (legacy != null)
            {
                GUILayout.Space(8);
                GUILayout.Label($"<size=12>{legacy}</size>");
            }
            GUILayout.Space(24);

            if (GUILayout.Button("New Game", GUILayout.Height(44)))
                StartCampaign(false);

            bool hasSave = SunderedCrown.Save.SaveSystem.Exists(SaveSlot);
            GUI.enabled = hasSave;
            if (GUILayout.Button(hasSave ? "Continue" : "Continue — (no save yet)", GUILayout.Height(44)))
                StartCampaign(true);
            GUI.enabled = true;
            if (hasSave)
            {
                var meta = SunderedCrown.Save.SaveSystem.Peek(SaveSlot);
                if (meta.exists)
                    GUILayout.Label($"<size=11><color=#9ad>   {meta.heroName} · level {meta.heroLevel}</color>" +
                                    (string.IsNullOrEmpty(meta.savedAtUtc) ? "" : $"<color=#777> · saved {meta.savedAtUtc}</color>") + "</size>");
            }

            GUILayout.Space(10);
            if (GUILayout.Button("Load Game / Saves", GUILayout.Height(34)))
            {
                _slots = gameObject.AddComponent<SunderedCrown.UI.SaveSlotScreen>();
                _slots.onLoad = _ => StartCampaign(true);
                _slots.onNew = _ => StartCampaign(false);
                _slots.onClose = () => { Destroy(_slots); _slots = null; };
            }

            GUILayout.Space(10);
            if (GUILayout.Button("Settings", GUILayout.Height(30)))
            {
                _settings = gameObject.AddComponent<SunderedCrown.UI.SettingsScreen>();
                _settings.startOpen = true;
                _settings.onClose = () => { Destroy(_settings); _settings = null; };
            }

            GUILayout.Space(10);
            if (GUILayout.Button("Credits", GUILayout.Height(30)))
            {
                _credits = gameObject.AddComponent<SunderedCrown.UI.CreditsScreen>();
                _credits.onClose = () => { Destroy(_credits); _credits = null; };
            }

            GUILayout.Space(10);
            if (GUILayout.Button("Quit", GUILayout.Height(30)))
            {
                Application.Quit();
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#endif
            }

            GUILayout.FlexibleSpace();
            GUILayout.Label("<size=11><color=#888>You came back. They never come back.</color></size>");
            GUILayout.EndArea();
        }

        private void StartCampaign(bool continueGame)
        {
            var go = new GameObject("CampaignBootstrap");
            var cb = go.AddComponent<CampaignBootstrap>();
            cb.continueGame = continueGame;
            Destroy(gameObject);
        }
    }
}
