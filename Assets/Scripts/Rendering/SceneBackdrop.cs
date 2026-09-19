using UnityEngine;
using SunderedCrown.Grid;

namespace SunderedCrown.Rendering
{
    /// <summary>
    /// Puts something behind the room.
    /// <para>
    /// The camera clears to a flat near-black, so an isometric floor sits as a
    /// small lit diamond in the middle of a large dead field — the frame reads as
    /// a 3D viewport with the lights off rather than as a place. The web build
    /// solves this with painted zone backdrops (<c>play/maps/*.jpg</c>), which have
    /// no Unity path; this is the cheap equivalent: one generated sprite, a radial
    /// falloff from an era-tinted glow down to the void, sitting far behind
    /// everything.
    /// </para>
    /// <para>
    /// Its palette comes from the floor family the scene already chose, so the
    /// Crown Wars court sits in cold marble light and Cinderhaunt in ember red
    /// without any scene having to say so twice.
    /// </para>
    /// </summary>
    public class SceneBackdrop : MonoBehaviour
    {
        /// Drawn behind the floor, which starts around -2000 for a large grid.
        private const int SortingOrder = -30000;
        private const int Size = 256;

        /// <summary>Era tints, keyed by the floor family a scene sets on
        /// <see cref="TileFloorRenderer"/>. The fall-through is the grey's own violet.</summary>
        private static Color Glow(string floorFamily) => floorFamily switch
        {
            "marble" => new Color(0.18f, 0.22f, 0.32f),     // the Crown Wars court
            "infernal" => new Color(0.30f, 0.12f, 0.10f),   // Cinderhaunt
            "sandstone" => new Color(0.28f, 0.22f, 0.13f),  // Netheril
            "pebble" => new Color(0.21f, 0.18f, 0.16f),     // Baldur's Gate
            "tomb" => new Color(0.15f, 0.19f, 0.17f),       // crypts
            "grey_dirt" => new Color(0.16f, 0.15f, 0.20f),  // the Fugue
            _ => new Color(0.16f, 0.14f, 0.21f),
        };

        private static readonly Color Void = new Color(0.047f, 0.043f, 0.063f);

        [Tooltip("Multiple of the camera's visible height to cover. Above 1 so the\n        edge never enters frame while the camera pans.")]
        public float cover = 2.6f;

        private Camera _cam;

        /// <summary>Build a backdrop for this scene, parented to the camera.
        /// <para>
        /// Sizing it off the grid instead does not work: every mode reframes the
        /// camera, so a backdrop big enough to cover the widest scene is many times
        /// the view in the narrowest, and all that shows is the flat middle of the
        /// gradient. Riding the camera keeps the falloff at a constant size on
        /// screen, where it reads as a pool of light under the room — and it cannot
        /// end inside the frame, which is the one way a backdrop looks worse than
        /// none at all.
        /// </para></summary>
        public static void Create(Transform parent, GridSystem grid, string floorFamily)
        {
            var cam = Camera.main;

            var go = new GameObject("Backdrop");
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = Build(Glow(floorFamily));
            sr.sortingOrder = SortingOrder;

            if (cam != null)
            {
                go.transform.SetParent(cam.transform, false);
                go.transform.localPosition = new Vector3(0f, 0f, 50f);
                go.AddComponent<SceneBackdrop>()._cam = cam;
                return;
            }

            // No camera yet: fall back to a grid-centred sheet large enough to cover.
            if (grid == null) { Destroy(go); return; }
            go.transform.SetParent(parent, false);
            Vector3 a = grid.GridToWorld(0, 0);
            Vector3 b = grid.GridToWorld(grid.width - 1, grid.height - 1);
            go.transform.position = new Vector3((a.x + b.x) * 0.5f, (a.y + b.y) * 0.5f, 40f);
            float span = Mathf.Max(Mathf.Abs(b.x - a.x), Mathf.Abs(b.y - a.y)) + 24f;
            go.transform.localScale = new Vector3(span * 2.2f, span * 2.2f, 1f);
        }

        void LateUpdate()
        {
            if (_cam == null) _cam = Camera.main;
            if (_cam == null) return;
            // Follows the camera's zoom; the isometric rig changes orthographicSize.
            float h = _cam.orthographic ? _cam.orthographicSize * 2f : 16f;
            float s = h * cover;
            transform.localScale = new Vector3(s, s, 1f);
        }

        private static Sprite Build(Color glow)
        {
            var tex = new Texture2D(Size, Size, TextureFormat.RGBA32, false)
            {
                wrapMode = TextureWrapMode.Clamp,
                hideFlags = HideFlags.DontSave,
            };
            var px = new Color[Size * Size];
            float r = Size * 0.5f;
            for (int y = 0; y < Size; y++)
                for (int x = 0; x < Size; x++)
                {
                    // Squashed to the floor's 2:1 projection, so the pool of light
                    // sits under the room rather than ringing it.
                    float dx = (x - r + 0.5f) / r;
                    float dy = (y - r + 0.5f) / (r * 0.62f);
                    float d = Mathf.Clamp01(Mathf.Sqrt(dx * dx + dy * dy));
                    float t = 1f - d;
                    t = t * t * (3f - 2f * t);            // smoothstep: no visible ring
                    px[y * Size + x] = Color.Lerp(Void, glow, t);
                }
            tex.SetPixels(px);
            tex.Apply();

            var sp = Sprite.Create(tex, new Rect(0, 0, Size, Size), new Vector2(0.5f, 0.5f), Size);
            sp.hideFlags = HideFlags.DontSave;
            return sp;
        }
    }
}
