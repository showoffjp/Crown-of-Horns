using UnityEngine;
using SunderedCrown.Grid;

namespace SunderedCrown.Rendering
{
    /// <summary>
    /// Paints an isometric floor for a GridSystem.
    /// <para>
    /// The grid projects to a 2:1 diamond lattice (<see cref="GridSystem.GridToWorld"/>),
    /// but this used to drop an axis-aligned square quad on each diamond centre.
    /// Neighbouring squares then overlapped by half a tile in both directions, so
    /// the floor came out as a dense mat of overlapping rectangles with a
    /// staircased edge — it read as a brick wall laid flat, not as a floor.
    /// </para>
    /// <para>
    /// It now draws diamond sprites from <c>Resources/Art/DCSS/iso</c>
    /// (tools/gen-iso-tiles.py), which tessellate the lattice exactly: each is
    /// 128x64 at 128 pixels per unit, i.e. precisely one tile, so no scaling is
    /// involved and no seams can open up. Blocked cells are drawn as raised blocks
    /// — a stack of darkened copies for the side, a lit cap on top — so walls read
    /// as walls rather than as differently-coloured floor.
    /// </para>
    /// <para>
    /// Scenes pick their palette by setting <see cref="floorFamily"/> and
    /// <see cref="wallFamily"/> before the component starts. If the iso sprites are
    /// missing the old textured-cube path still runs, and if the textures are
    /// missing too the tiles fall back to flat tints.
    /// </para>
    /// </summary>
    public class TileFloorRenderer : MonoBehaviour
    {
        [Header("Palette (DCSS families under Resources/Art/DCSS)")]
        [Tooltip("grey_dirt · infernal · marble · pebble · sandstone · tomb")]
        public string floorFamily = "tomb";
        [Tooltip("brick_brown · brick_dark · marble_wall · stone_dark · tomb_wall")]
        public string wallFamily = "brick_dark";
        [Tooltip("Highest variant index to look for. Families are sparse — grey_dirt and\n        brick_dark have 8, marble starts at 1 — so scan wide and keep what exists.")]
        public int variants = 8;

        [Header("Walls")]
        [Tooltip("How far a blocked cell is raised above the floor, in world units.")]
        public float wallRise = 0.34f;
        [Tooltip("Copies stacked to fill the side of a raised block.")]
        public int wallLayers = 5;

        [Header("Fallback tints (used when a texture is missing)")]
        public Color tileA = new Color(0.16f, 0.16f, 0.20f);
        public Color tileB = new Color(0.13f, 0.13f, 0.17f);
        public Color blocked = new Color(0.05f, 0.05f, 0.07f);

        [Tooltip("Draw order depth; larger = further behind the units.")]
        public float depth = 0.5f;

        void Start()
        {
            var grid = GridSystem.Instance;
            if (grid == null) return;

            // Something behind the room, and something moving in front of it.
            SceneBackdrop.Create(transform, grid, floorFamily);
            AmbientMotes.Create(transform, grid, floorFamily);

            var isoFloors = LoadSprites("floor", floorFamily);
            var isoWalls = LoadSprites("wall", wallFamily);
            if (isoFloors != null)
            {
                BuildIso(grid, isoFloors, isoWalls);
                return;
            }
            BuildQuads(grid);
        }

        // ---- the isometric path -------------------------------------------------

        private void BuildIso(GridSystem grid, Sprite[] floors, Sprite[] walls)
        {
            for (int x = 0; x < grid.width; x++)
                for (int y = 0; y < grid.height; y++)
                {
                    var cell = grid.GetCell(x, y);
                    if (cell == null) continue;

                    Vector3 w = grid.GridToWorld(x, y);
                    // Sorting runs off the tile's own row, so a wall one row nearer
                    // the camera occludes what stands behind it.
                    int row = Mathf.RoundToInt(-w.y * 100f);

                    if (cell.walkable || walls == null)
                    {
                        var set = cell.walkable ? floors : (walls ?? floors);
                        Tile(set, x, y, new Vector3(w.x, w.y, depth), row - 20, Color.white);
                        continue;
                    }

                    // Side of the block: darkened copies stacked from the floor up,
                    // so the rise reads as stone rather than as a floating cap.
                    int layers = Mathf.Max(1, wallLayers);
                    for (int i = 0; i < layers; i++)
                    {
                        float t = i / (float)layers;
                        Tile(walls, x, y, new Vector3(w.x, w.y + wallRise * t, depth),
                             row - 12 + i, new Color(0.34f + t * 0.16f, 0.34f + t * 0.16f, 0.38f + t * 0.16f));
                    }
                    Tile(walls, x, y, new Vector3(w.x, w.y + wallRise, depth), row - 6, Color.white);
                }
        }

        private void Tile(Sprite[] set, int x, int y, Vector3 pos, int order, Color tint)
        {
            var go = new GameObject($"Tile_{x}_{y}");
            go.transform.SetParent(transform);
            go.transform.position = pos;
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = Pick(set, x, y);
            sr.color = tint;
            sr.sortingOrder = order;
        }

        // ---- the original textured-cube path, kept as a fallback -----------------

        private void BuildQuads(GridSystem grid)
        {
            var floors = LoadFamily("floor", floorFamily);
            var walls = LoadFamily("wall", wallFamily);

            for (int x = 0; x < grid.width; x++)
                for (int y = 0; y < grid.height; y++)
                {
                    var cell = grid.GetCell(x, y);
                    if (cell == null) continue;

                    var tile = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    tile.name = $"Tile_{x}_{y}";
                    tile.transform.SetParent(transform);
                    var col = tile.GetComponent<Collider>();
                    if (col != null) Destroy(col);

                    Vector3 w = grid.GridToWorld(x, y);
                    tile.transform.position = new Vector3(w.x, w.y, depth);
                    tile.transform.localScale = new Vector3(grid.tileWidth * 0.94f, grid.tileHeight * 0.94f, 0.02f);

                    var mat = tile.GetComponent<Renderer>().material;
                    var set = cell.walkable ? floors : walls;
                    var tex = Pick(set, x, y);

                    if (tex != null)
                    {
                        mat.mainTexture = tex;
                        mat.color = cell.walkable && ((x + y) & 1) == 0
                            ? Color.white : new Color(0.88f, 0.88f, 0.92f);
                    }
                    else
                    {
                        mat.color = !cell.walkable ? blocked : (((x + y) & 1) == 0 ? tileA : tileB);
                    }
                }
        }

        // ---- loading ---------------------------------------------------------------

        /// Loads the diamond sprites "<family>0..N" from Resources/Art/DCSS/iso/<group>.
        private Sprite[] LoadSprites(string group, string family)
        {
            if (string.IsNullOrEmpty(family)) return null;
            var found = new System.Collections.Generic.List<Sprite>();
            for (int i = 0; i <= Mathf.Max(1, variants); i++)
            {
                var s = Resources.Load<Sprite>($"Art/DCSS/iso/{group}/{family}{i}");
                if (s != null) found.Add(s);            // families are sparse; skip the gaps
            }
            if (found.Count == 0)
            {
                var single = Resources.Load<Sprite>($"Art/DCSS/iso/{group}/{family}");
                if (single != null) found.Add(single);
            }
            return found.Count > 0 ? found.ToArray() : null;
        }

        /// Loads "<family>0..N" from Resources/Art/DCSS/<group>, skipping gaps.
        private Texture2D[] LoadFamily(string group, string family)
        {
            if (string.IsNullOrEmpty(family)) return null;
            var found = new System.Collections.Generic.List<Texture2D>();
            // "lit/" holds brightness-corrected copies (tools/gen-lit-tiles.py). The raw
            // Crawl tiles average ~20 of 255 and render as solid black on lit geometry.
            for (int i = 0; i <= Mathf.Max(1, variants); i++)
            {
                var t = Resources.Load<Texture2D>($"Art/DCSS/lit/{group}/{family}{i}")
                     ?? Resources.Load<Texture2D>($"Art/DCSS/{group}/{family}{i}");
                if (t != null) found.Add(t);
            }
            if (found.Count == 0)
            {
                var single = Resources.Load<Texture2D>($"Art/DCSS/lit/{group}/{family}")
                          ?? Resources.Load<Texture2D>($"Art/DCSS/{group}/{family}");
                if (single != null) found.Add(single);
            }
            return found.Count > 0 ? found.ToArray() : null;
        }

        /// Deterministic per-cell variant: the same tile every run, no visible tiling.
        private static T Pick<T>(T[] set, int x, int y) where T : Object
        {
            if (set == null || set.Length == 0) return null;
            int h = (x * 73856093) ^ (y * 19349663);
            return set[Mathf.Abs(h) % set.Length];
        }
    }
}
