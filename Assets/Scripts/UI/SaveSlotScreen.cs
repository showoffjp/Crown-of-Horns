using UnityEngine;
using SunderedCrown.Core;
using SunderedCrown.Save;

namespace SunderedCrown.UI
{
    /// <summary>
    /// The save-slot manager (OnGUI modal). Lists every slot with its hero/level/scene/time, and lets you
    /// **Load** an existing save, start a **New Game** into any slot, or **Delete** one. Picking an action
    /// sets <see cref="SaveSlots.Active"/> and fires the matching callback (the main menu launches the
    /// campaign). Drop it on a GameObject, set the callbacks + onClose, and it draws itself.
    /// </summary>
    public class SaveSlotScreen : MonoBehaviour
    {
        public System.Action<string> onLoad;  // load existing slot
        public System.Action<string> onNew;   // new game into slot
        public System.Action onClose;

        private string _confirmDelete;

        void OnGUI()
        {
            const float w = 520, h = 460;
            float x = Screen.width / 2f - w / 2f, y = Screen.height / 2f - h / 2f;
            GUILayout.BeginArea(new Rect(x, y, w, h), GUI.skin.box);
            GUILayout.Label("<size=20><b>💾 SAVES</b></size>");
            GUILayout.Space(8);

            foreach (var slot in SaveSlots.All)
            {
                var m = SaveSystem.Peek(slot);
                GUILayout.BeginVertical(GUI.skin.box);

                GUILayout.Label($"<b>{SaveSlots.Label(slot)}</b>");
                if (m.exists)
                    GUILayout.Label($"<color=#cfc>{(string.IsNullOrEmpty(m.heroName) ? "The Returned" : m.heroName)} — " +
                                    $"level {m.heroLevel} · {Pretty(m.sceneName)}</color>   <size=10><color=#888>{When(m.savedAtUtc)}</color></size>");
                else
                    GUILayout.Label("<color=#888><i>— empty —</i></color>");

                GUILayout.BeginHorizontal();
                GUI.enabled = m.exists;
                if (GUILayout.Button("Load", GUILayout.Height(26))) { SaveSlots.Active = slot; onLoad?.Invoke(slot); }
                GUI.enabled = true;
                if (GUILayout.Button(m.exists ? "Overwrite — New Game" : "New Game", GUILayout.Height(26)))
                { SaveSlots.Active = slot; onNew?.Invoke(slot); }
                GUI.enabled = m.exists;
                if (_confirmDelete == slot)
                {
                    if (GUILayout.Button("<color=#f88>Confirm delete</color>", GUILayout.Height(26)))
                    { SaveSystem.Delete(slot); _confirmDelete = null; }
                }
                else if (GUILayout.Button("Delete", GUILayout.Height(26))) _confirmDelete = slot;
                GUI.enabled = true;
                GUILayout.EndHorizontal();

                GUILayout.EndVertical();
                GUILayout.Space(4);
            }

            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Back", GUILayout.Height(32))) onClose?.Invoke();
            GUILayout.EndArea();
        }

        private static string Pretty(string scene) => string.IsNullOrEmpty(scene) ? "in the Realms" : scene;

        private static string When(string iso)
        {
            if (string.IsNullOrEmpty(iso)) return "";
            return System.DateTime.TryParse(iso, null, System.Globalization.DateTimeStyles.RoundtripKind, out var dt)
                ? dt.ToLocalTime().ToString("yyyy-MM-dd HH:mm") : "";
        }
    }
}
