using UnityEngine;
using SunderedCrown.Save;

namespace SunderedCrown.Core
{
    /// <summary>
    /// Top-level persistent singleton. Holds global services and the quick
    /// save/load hotkeys. Put this on a "GameManager" object in your boot scene;
    /// it survives scene loads.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("Quick Save/Load")]
        public string quickSlot = "quick";
        public KeyCode quickSaveKey = KeyCode.F5;
        public KeyCode quickLoadKey = KeyCode.F9;

        void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private string _toast = "";
        private float _toastUntil;

        void Update()
        {
            if (Input.GetKeyDown(quickSaveKey))
            {
                SaveSystem.Save(quickSlot, UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
                Toast("💾 Quick-saved.");
            }
            if (Input.GetKeyDown(quickLoadKey))
            {
                if (SaveSystem.Exists(quickSlot)) { SaveSystem.Load(quickSlot); Toast("↺ Quick-loaded."); }
                else Toast("↺ No quick-save yet.");
            }
        }

        private void Toast(string msg) { _toast = msg; _toastUntil = Time.unscaledTime + 1.6f; }

        void OnGUI()
        {
            if (Time.unscaledTime > _toastUntil || string.IsNullOrEmpty(_toast)) return;
            var style = new GUIStyle(GUI.skin.box) { fontSize = 14, alignment = TextAnchor.MiddleCenter };
            GUI.Label(new Rect(Screen.width / 2f - 90, 12, 180, 28), _toast, style);
        }
    }
}
