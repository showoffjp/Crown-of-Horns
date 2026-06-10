using UnityEngine;
using UnityEngine.SceneManagement;

namespace SunderedCrown.Core
{
    /// <summary>
    /// AUTO-BOOT — the missing link that makes "press Play" actually start the game.
    ///
    /// The project boots procedurally (there is no authored .unity scene, by design — everything is
    /// built in code). Without an entry hook, pressing Play on an empty scene does nothing. This fires
    /// automatically when you enter Play mode and spawns the <see cref="MainMenu"/> front-end if nothing
    /// else has — so the title screen comes up from any scene, including the default empty one.
    ///
    /// Guards keep it from interfering: it never double-boots over an existing menu/campaign, never runs
    /// inside the Test Runner's bootstrap scene, and can be switched off via <see cref="AutoBootEnabled"/>.
    /// </summary>
    public static class GameEntryPoint
    {
        /// <summary>Set false (e.g. from a custom boot scene or a test) to suppress the auto-boot.</summary>
        public static bool AutoBootEnabled = true;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Boot()
        {
            if (!AutoBootEnabled) return;

            // Never boot inside the Unity Test Runner's own scene (PlayMode tests build their own world).
            var sceneName = SceneManager.GetActiveScene().name;
            if (!string.IsNullOrEmpty(sceneName) && sceneName.StartsWith("InitTestScene")) return;

            // Don't boot on top of an existing front-end or a running campaign (a hand-authored boot
            // scene, or a test that spins these up itself, already owns the session).
            if (Object.FindAnyObjectByType<MainMenu>() != null) return;
            if (Object.FindAnyObjectByType<CampaignBootstrap>() != null) return;
            if (Object.FindAnyObjectByType<PrologueBootstrap>() != null) return;

            var go = new GameObject("MainMenu (auto-boot)");
            go.AddComponent<MainMenu>();
        }
    }
}
