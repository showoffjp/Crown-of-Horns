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

        void Update()
        {
            if (Input.GetKeyDown(quickSaveKey))
                SaveSystem.Save(quickSlot, UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
            if (Input.GetKeyDown(quickLoadKey))
                SaveSystem.Load(quickSlot);
        }
    }
}
