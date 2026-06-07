using UnityEngine;
using SunderedCrown.Core;

namespace SunderedCrown.UI
{
    /// <summary>
    /// THE PARTY HAS FALLEN. Shown when a battle ends with no one left standing. Offers a graceful recovery:
    /// reload the last save (if one exists) or return to the title — instead of leaving the player stranded
    /// on a dead battlefield. Lives on its own root GameObject (so it survives tearing down the campaign it
    /// is replacing). Spawned by EncounterController on a total-party defeat. Zero setup.
    /// </summary>
    public class DefeatScreen : MonoBehaviour
    {
        private const string SaveSlot = "save";
        private bool _busy;

        void OnGUI()
        {
            if (_busy) return;

            const float w = 460, h = 280;
            float x = Screen.width / 2f - w / 2f, y = Screen.height / 2f - h / 2f;
            GUILayout.BeginArea(new Rect(x, y, w, h), GUI.skin.box);

            GUILayout.Space(10);
            GUILayout.Label("<size=24><b><color=#c95>✘ THE PARTY HAS FALLEN</color></b></size>");
            GUILayout.Space(6);
            GUILayout.Label("<i>The dark closes over you. But the Returned have never been good at staying gone — " +
                            "and neither, it seems, are you.</i>");
            GUILayout.Space(18);

            bool hasSave = SunderedCrown.Save.SaveSystem.Exists(SaveSlot);
            GUI.enabled = hasSave;
            if (GUILayout.Button(hasSave ? "↺  Load Last Save" : "↺  Load Last Save — (none found)", GUILayout.Height(46)))
                ReloadFromSave();
            GUI.enabled = true;

            GUILayout.Space(10);
            if (GUILayout.Button("⌂  Return to Title", GUILayout.Height(40)))
                ReturnToTitle();

            GUILayout.FlexibleSpace();
            GUILayout.Label("<size=11><color=#888>Permanent loss in this saga is reserved for the Breach — a wipe is never the end.</color></size>");
            GUILayout.EndArea();
        }

        private void ReloadFromSave()
        {
            _busy = true;
            var old = FindFirstObjectByType<CampaignBootstrap>();
            var go = new GameObject("CampaignBootstrap");
            var cb = go.AddComponent<CampaignBootstrap>();
            cb.continueGame = true; // EnsureCore rebuilds the singletons; ContinueGame restores the save
            if (old != null) Destroy(old.gameObject);
            Destroy(gameObject);
        }

        private void ReturnToTitle()
        {
            _busy = true;
            var old = FindFirstObjectByType<CampaignBootstrap>();
            var go = new GameObject("MainMenu");
            go.AddComponent<MainMenu>();
            if (old != null) Destroy(old.gameObject);
            Destroy(gameObject);
        }
    }
}
