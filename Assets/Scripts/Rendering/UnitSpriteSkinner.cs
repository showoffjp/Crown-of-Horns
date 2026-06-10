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
            return WorldArt.Sprite(name)
                ?? WorldArt.Sprite(FirstWord(name))
                ?? WorldArt.Sprite(u.faction.ToString());
        }

        private static void Apply(GridUnit u, Sprite sprite)
        {
            // Hide the placeholder cube's mesh, keep its collider (selection/raycasts still work).
            var mr = u.GetComponent<MeshRenderer>();
            if (mr != null) mr.enabled = false;

            var go = new GameObject("Sprite");
            go.transform.SetParent(u.transform, false);
            go.transform.localPosition = new Vector3(0f, 0.4f, 0f);
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = sprite;
            sr.sortingOrder = 10;
            go.AddComponent<CameraBillboard>();
            go.AddComponent<IsoDepthSorter>().target = sr; // nearer units draw in front, even as they move
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

    /// <summary>Isometric depth sort: a unit lower/closer on the diamond (greater x+y in world space) draws
    /// in front. Updates each frame so moving units re-sort correctly. Pairs with the sprite skinner.</summary>
    public class IsoDepthSorter : MonoBehaviour
    {
        public SpriteRenderer target;

        void LateUpdate()
        {
            if (target == null) return;
            var p = transform.position;
            target.sortingOrder = Mathf.RoundToInt(-(p.x + p.y) * 100f);
        }
    }
}
