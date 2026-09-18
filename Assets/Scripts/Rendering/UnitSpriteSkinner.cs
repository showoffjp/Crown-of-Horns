using System.Collections.Generic;
using UnityEngine;
using SunderedCrown.Grid;

namespace SunderedCrown.Rendering
{
    /// <summary>
    /// The art seam for units. Periodically scans for `GridUnit`s and, if a matching sprite exists in
    /// <c>Resources/Sprites/</c> (via <see cref="WorldArt"/>), hides the placeholder cube and parents a
    /// camera-facing sprite billboard in its place — keeping the cube's collider so clicks still land.
    /// If no art is present it no-ops (the cubes stay), so the game looks identical until the moment you
    /// drop in a sprite named after a unit (e.g. <c>Sprites/Garrow.png</c>, <c>Sprites/Enemy.png</c>).
    /// Drop one on a persistent object (the campaign director adds it); zero per-unit wiring. See
    /// docs/ASSET_INTEGRATION.md.
    /// </summary>
    public class UnitSpriteSkinner : MonoBehaviour
    {
        public float scanInterval = 0.5f;
        private float _next;
        private readonly HashSet<GridUnit> _done = new HashSet<GridUnit>();

        void Update()
        {
            if (Time.time < _next) return;
            _next = Time.time + scanInterval;

            foreach (var u in FindObjectsByType<GridUnit>())
            {
                if (u == null) continue;
                if (_done.Contains(u)) continue;

                var sprite = Resolve(u);
                _done.Add(u);               // resolved either way — don't rescan this unit
                if (sprite != null) Apply(u, sprite);
            }
        }

        private static Sprite Resolve(GridUnit u)
        {
            string name = u.Sheet != null ? u.Sheet.displayName : null;
            string cls = u.Sheet != null && u.Sheet.classDef != null ? u.Sheet.classDef.className : null;

            // A standee first. Resources/Sprites holds *battle tokens*: circular UI
            // chips with the unit's initials and its name printed across the bottom.
            // Stood in the world those read as poker counters with captions, which is
            // what the party looked like. They stay as the last-resort fallback so a
            // unit with no painted soul still gets something.
            //
            // Class and faction come before the tokens because the player character
            // can never have a portrait of their own: their name is whatever was typed
            // at character creation. Without this the one unit the player looks at
            // most was the last cube left on screen.
            return WorldArt.Standee(name)
                ?? WorldArt.Standee(cls)
                ?? WorldArt.Standee(u.faction.ToString())
                ?? WorldArt.Sprite(name)
                ?? WorldArt.Sprite(FirstWord(name))
                ?? WorldArt.Sprite(u.faction.ToString());
        }

        /// World height, in units, for a unit — matched to MarkerArt so the party
        /// and the NPCs they walk past are the same size as each other.
        private const float UnitHeight = 1.02f;
        private const float MaxWidth = 1.15f;
        private const float FootDrop = -0.12f;

        private static void Apply(GridUnit u, Sprite sprite)
        {
            // Hide the placeholder cube's mesh, keep its collider (selection/raycasts still work).
            var mr = u.GetComponent<MeshRenderer>();
            if (mr != null) mr.enabled = false;

            var go = new GameObject("Sprite");
            go.transform.SetParent(u.transform, false);
            go.transform.localPosition = new Vector3(0f, FootDrop, -0.02f);
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = sprite;
            sr.sortingOrder = 10;
            sr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            sr.receiveShadows = false;

            // Standees are tall and tokens are square; normalise both, and never let
            // anything grow wider than a tile and a bit.
            float h = sprite.bounds.size.y, w = sprite.bounds.size.x;
            if (h > 0.0001f && w > 0.0001f)
            {
                float k = Mathf.Min(UnitHeight / h, MaxWidth / w);
                go.transform.localScale = new Vector3(k, k, k);
            }

            go.AddComponent<CameraBillboard>();
            var depth = go.AddComponent<IsoDepthSorter>(); // nearer units draw in front, even as they move
            depth.target = sr;
            depth.basis = u.transform;
        }

        private static string FirstWord(string s)
        {
            if (string.IsNullOrEmpty(s)) return null;
            int sp = s.IndexOf(' ');
            return sp > 0 ? s.Substring(0, sp) : s;
        }
    }

    /// <summary>Keeps a sprite facing the camera each frame (so 2D art reads in the 2.5D scene).</summary>
    public class CameraBillboard : MonoBehaviour
    {
        void LateUpdate()
        {
            var cam = Camera.main;
            if (cam != null) transform.rotation = cam.transform.rotation;
        }
    }

    /// <summary>Isometric depth sort: a thing lower on screen is nearer the camera and
    /// draws in front. Updates each frame so moving units re-sort correctly.
    /// <para>
    /// Depth is world Y alone. GridToWorld maps the grid to a diamond where Y is
    /// screen height and X is screen left/right, so the old <c>-(x + y)</c> let a
    /// marker's horizontal position change its depth: two things standing on the
    /// same screen row sorted by which was further left, and a near-right sprite
    /// could draw behind a far-left one.
    /// </para>
    /// <para><c>offset</c> separates parts of one marker — a ground shadow rides one
    /// step behind the art it belongs to.</para></summary>
    public class IsoDepthSorter : MonoBehaviour
    {
        public SpriteRenderer target;
        public int offset;
        [Tooltip("Whose position sets the depth. Defaults to this object; set it to the\n        marker root so art lifted off the ground still sorts by the tile it stands on.")]
        public Transform basis;

        void LateUpdate()
        {
            if (target == null) return;
            var t = basis != null ? basis : transform;
            target.sortingOrder = Mathf.RoundToInt(-t.position.y * 100f) + offset;
        }
    }
}
