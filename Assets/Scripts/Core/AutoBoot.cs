using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

namespace SunderedCrown.Core
{
    /// <summary>
    /// Gives the Boot scene the two things it has never had: a camera and a light.
    /// <para>
    /// Booting the game itself belongs to <see cref="GameEntryPoint"/>, which spawns
    /// the main menu; this only supplies the scene furniture that neither it nor the
    /// mode builders provide early enough.
    /// </para>
    /// <para>
    /// Boot.unity ships with no GameObjects, and this project does not commit most
    /// script .meta files, so script GUIDs differ per machine and a MonoBehaviour
    /// saved into the scene would load as "missing script" on another clone.
    /// Everything is therefore built at runtime.
    /// </para>
    /// <para>
    /// The world is drawn with <c>GameObject.CreatePrimitive</c> cubes carrying
    /// Unity's default <em>lit</em> material. With no light in the scene those
    /// render black, which is why the game read as flat darkness. A single
    /// directional key light plus tinted ambient is all the geometry needs.
    /// </para>
    /// </summary>
    public static class AutoBoot
    {
        // The grey's palette, shared with the web build's backdrops.
        private static readonly Color Void = new Color(0.047f, 0.043f, 0.063f); // #0c0b10
        private static readonly Color Ambient = new Color(0.20f, 0.18f, 0.26f);
        private static readonly Color KeyLight = new Color(1.00f, 0.95f, 0.86f);

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Launch()
        {
            // Only the Boot scene; the demo scenes bring their own entry points.
            if (SceneManager.GetActiveScene().name != "Boot") return;

            EnsureCamera();
            EnsureLighting();
            // Spawning the campaign is NOT this class's job: GameEntryPoint already
            // boots the front-end, and doing it here would race it. Whichever ran
            // first would win, and if this one did, GameEntryPoint would see a live
            // CampaignBootstrap and skip the main menu entirely — the player would
            // drop straight into the game with no title screen.
        }

        /// A camera must exist from the menu onward, or Unity renders nothing at
        /// all ("Display 1 — No cameras rendering") while the IMGUI menu floats
        /// over the void. Mode builders look up Camera.main and reframe this one.
        private static void EnsureCamera()
        {
            // A camera built elsewhere still needs a listener: every hand-built camera
            // in this project was created with AddComponent<Camera>() alone.
            var existing = Camera.main;
            if (existing != null)
            {
                if (existing.GetComponent<AudioListener>() == null &&
                    Object.FindAnyObjectByType<AudioListener>() == null)
                    existing.gameObject.AddComponent<AudioListener>();
                return;
            }

            var go = new GameObject("Main Camera") { tag = "MainCamera" };
            var cam = go.AddComponent<Camera>();
            cam.orthographic = true;              // matches EncounterBuilder's framing
            cam.orthographicSize = 7f;
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = Void;
            cam.transform.position = new Vector3(0f, 0f, -10f);
            go.AddComponent<AudioListener>();     // hand-built cameras need this explicitly
        }

        /// One key light, over a low violet ambient so shadowed faces keep colour.
        /// <para>
        /// The light is raked only slightly, and casts no shadows. The world is a
        /// flat XY plane seen head-on — <c>GridSystem.GridToWorld</c> fakes the
        /// isometry in 2D and leaves Z for sorting — so a steeply angled light lit
        /// the camera-facing quads at a glancing dot product and smeared every
        /// marker's shadow into a long streak clear across the floor. Facing the
        /// plane puts the terrain's own painted brightness on screen unaltered, and
        /// markers get a painted contact shadow instead (Rendering/MarkerArt).
        /// </para>
        private static void EnsureLighting()
        {
            RenderSettings.ambientMode = AmbientMode.Flat;
            RenderSettings.ambientLight = Ambient;

            if (Object.FindAnyObjectByType<Light>() != null) return;

            var go = new GameObject("Key Light");
            var light = go.AddComponent<Light>();
            light.type = LightType.Directional;
            light.color = KeyLight;
            light.intensity = 1.0f;
            light.shadows = LightShadows.None;
            go.transform.rotation = Quaternion.Euler(18f, -14f, 0f);
        }
    }
}
