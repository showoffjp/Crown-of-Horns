using UnityEngine;
using UnityEngine.SceneManagement;

namespace SunderedCrown.Core
{
    /// <summary>
    /// Starts the campaign when the Boot scene plays.
    /// <para>
    /// Boot.unity ships with no GameObjects, and this project does not commit
    /// script .meta files (38 of 231), so script GUIDs are generated per machine.
    /// A MonoBehaviour reference saved into the scene would therefore resolve as
    /// "missing script" on any other clone. Spawning the entry point at runtime
    /// sidesteps GUIDs entirely and works on a fresh checkout.
    /// </para>
    /// <para>
    /// CampaignBootstrap has no serialized fields — its Start() builds the whole
    /// game via AddComponent — so nothing here needs Inspector wiring.
    /// </para>
    /// </summary>
    public static class AutoBoot
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Launch()
        {
            // Only the Boot scene; the 29 demo scenes bring their own entry points.
            if (SceneManager.GetActiveScene().name != "Boot") return;
            if (Object.FindAnyObjectByType<CampaignBootstrap>() != null) return;

            var go = new GameObject("CampaignBootstrap");
            go.AddComponent<CampaignBootstrap>();
        }
    }
}
