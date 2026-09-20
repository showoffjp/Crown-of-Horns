using UnityEngine;
using SunderedCrown.Grid;

namespace SunderedCrown.Rendering
{
    /// <summary>
    /// Pyreflies: slow-drifting soul-motes over the floor.
    /// <para>
    /// The web build has had these since the FFX atmosphere pass and they do more
    /// for a room than any amount of tile detail — a still frame becomes a place
    /// with air in it. Unity had nothing moving on screen at all between turns.
    /// This is a direct port of <c>drawPyreflies</c> in tools/make-town-market.py,
    /// with its motion kept: a slow orbital phase, a lateral drift off the cosine
    /// of that phase, a vertical wander that nets upward, and a twinkle on a
    /// separate, slower period so the field never pulses in unison.
    /// </para>
    /// <para>
    /// Built from plain SpriteRenderers rather than a ParticleSystem: the count is
    /// tiny, it keeps the asmdef free of another module reference, and each mote
    /// can re-sort itself against the characters every frame the way
    /// <see cref="IsoDepthSorter"/> does — which is what lets them drift in front
    /// of someone and then behind them.
    /// </para>
    /// </summary>
    public class AmbientMotes : MonoBehaviour
    {
        [Tooltip("Motes in the field. Scaled off the room's size when created.")]
        public int count = 14;
        [Tooltip("World-units-per-second drift, before each mote's own variation.")]
        public float speed = 0.30f;
        [Tooltip("Mote diameter in world units; one floor tile is 1.0 wide. These read\n        in motion rather than in a still frame, so they are deliberately small —\n        raise them here if the field is too quiet on screen.")]
        public float sizeMin = 0.13f;
        public float sizeMax = 0.30f;
        [Range(0f, 1f)] public float opacity = 0.95f;
        public Color tint = new Color(0.62f, 0.55f, 0.85f);

        private struct Mote
        {
            public Transform T;
            public SpriteRenderer R;
            public float Phase, Speed, Size, Twinkle;
        }

        private Mote[] _motes;
        /// Candidate spawn points, one per grid cell. Motes are seeded and respawned
        /// from these rather than from a bounding rectangle: the room is a diamond,
        /// so a rectangle around it is mostly void, and motes seeded there drift
        /// outside the floor and read as dust on the lens rather than as the room's
        /// own air.
        private Vector2[] _spawns;
        private float _ceiling;
        private static Sprite _dot;

        /// <summary>Mote colour per era, keyed by the floor family the scene chose —
        /// the same hook the backdrop uses, so a scene declares its era once.</summary>
        private static Color Tint(string floorFamily) => floorFamily switch
        {
            "marble" => new Color(0.62f, 0.74f, 0.95f),     // the Crown Wars court
            "infernal" => new Color(0.98f, 0.62f, 0.38f),   // Cinderhaunt
            "sandstone" => new Color(0.95f, 0.82f, 0.52f),  // Netheril
            "pebble" => new Color(0.85f, 0.80f, 0.66f),     // Baldur's Gate
            "tomb" => new Color(0.60f, 0.86f, 0.72f),       // crypts
            "grey_dirt" => new Color(0.68f, 0.62f, 0.90f),  // the Fugue
            _ => new Color(0.62f, 0.55f, 0.85f),
        };

        public static void Create(Transform parent, GridSystem grid, string floorFamily)
        {
            if (grid == null) return;

            var go = new GameObject("Ambient Motes");
            go.transform.SetParent(parent, false);
            var m = go.AddComponent<AmbientMotes>();
            m.tint = Tint(floorFamily);

            var spawns = new System.Collections.Generic.List<Vector2>(grid.width * grid.height);
            float top = float.MinValue;
            for (int x = 0; x < grid.width; x++)
                for (int y = 0; y < grid.height; y++)
                {
                    Vector3 w = grid.GridToWorld(x, y);
                    spawns.Add(new Vector2(w.x, w.y));
                    if (w.y > top) top = w.y;
                }
            m._spawns = spawns.ToArray();
            m._ceiling = top + 1.5f;      // a little headroom above the far edge

            // Scaled off the room rather than off a head-count of the living: the
            // marker factories and this renderer both run in the same frame with no
            // ordering guarantee, so counting Interactables here would sometimes
            // find none and give a busy room an empty sky.
            m.count = Mathf.Clamp(6 + Mathf.RoundToInt((grid.width + grid.height) * 0.4f), 8, 24);
        }

        void Start()
        {
            if (_spawns == null || _spawns.Length == 0) { enabled = false; return; }

            _motes = new Mote[Mathf.Max(1, count)];
            var rnd = new System.Random(_spawns.Length * 73856093 ^ count * 19349663);
            for (int i = 0; i < _motes.Length; i++)
            {
                var go = new GameObject("Mote");
                go.transform.SetParent(transform, false);

                var sr = go.AddComponent<SpriteRenderer>();
                sr.sprite = Dot();
                sr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                sr.receiveShadows = false;

                float size = Mathf.Lerp(sizeMin, Mathf.Max(sizeMin, sizeMax), (float)rnd.NextDouble());
                Vector2 seed = _spawns[rnd.Next(_spawns.Length)];
                go.transform.position = new Vector3(seed.x, seed.y, -0.4f);
                go.transform.localScale = new Vector3(size, size, 1f);

                _motes[i] = new Mote
                {
                    T = go.transform,
                    R = sr,
                    Phase = (float)rnd.NextDouble() * Mathf.PI * 2f,
                    Speed = 0.4f + (float)rnd.NextDouble() * 0.8f,
                    Size = size,
                    Twinkle = (float)rnd.NextDouble() * Mathf.PI * 2f,
                };
            }
        }

        void Update()
        {
            if (_motes == null) return;
            float dt = Time.deltaTime;
            float t = Time.time;

            for (int i = 0; i < _motes.Length; i++)
            {
                var m = _motes[i];
                if (m.T == null) continue;

                m.Phase += (0.24f + m.Speed * 0.24f) * dt;
                float v = speed * m.Speed;
                var p = m.T.position;
                p.x += Mathf.Cos(m.Phase) * v * dt;
                // Wanders, but nets upward — a mote that only rose would read as
                // smoke rather than as something adrift.
                p.y += (Mathf.Sin(m.Phase * 0.7f) * 0.6f + 0.55f) * v * dt;

                // Risen off the far edge: come back somewhere over the floor. There is
                // no lateral wrap — the drift is a cosine, so it swings back on its
                // own, and wrapping x would fling motes out over the void.
                if (p.y > _ceiling)
                {
                    Vector2 s2 = _spawns[Random.Range(0, _spawns.Length)];
                    p.x = s2.x;
                    p.y = s2.y;
                }
                m.T.position = p;

                // Twinkle on its own period, so the field never pulses in unison.
                float tw = 0.35f + 0.30f * Mathf.Sin(t * 1.4f + m.Twinkle * 3f);
                var c = tint;
                c.a = Mathf.Clamp01(tw) * opacity;
                m.R.color = c;

                // Same depth rule as every other sprite, recomputed because these
                // move: a mote drifts in front of someone, then behind them.
                m.R.sortingOrder = Mathf.RoundToInt(-p.y * 100f) + 1;

                _motes[i] = m;
            }
        }

        /// <summary>A soft dot: a bright core inside a wide, faint halo, matching the
        /// two-circle draw the web build uses.</summary>
        private static Sprite Dot()
        {
            if (_dot != null) return _dot;

            const int N = 64;
            var tex = new Texture2D(N, N, TextureFormat.RGBA32, false)
            {
                wrapMode = TextureWrapMode.Clamp,
                hideFlags = HideFlags.DontSave,
            };
            var px = new Color32[N * N];
            float r = N * 0.5f;
            for (int y = 0; y < N; y++)
                for (int x = 0; x < N; x++)
                {
                    float d = Mathf.Sqrt((x - r + 0.5f) * (x - r + 0.5f) + (y - r + 0.5f) * (y - r + 0.5f)) / r;
                    float halo = Mathf.Clamp01(1f - d);
                    halo = halo * halo * halo * 0.30f;                 // wide, faint
                    float core = Mathf.Clamp01(1f - d * 3.2f);
                    core = core * core;                                 // small, bright
                    float a = Mathf.Clamp01(halo + core);
                    px[y * N + x] = new Color32(255, 255, 255, (byte)(a * 255f));
                }
            tex.SetPixels32(px);
            tex.Apply();

            _dot = Sprite.Create(tex, new Rect(0, 0, N, N), new Vector2(0.5f, 0.5f), N);
            _dot.hideFlags = HideFlags.DontSave;
            return _dot;
        }
    }
}
