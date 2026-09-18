using UnityEngine;
using SunderedCrown.Grid;

namespace SunderedCrown.Rendering
{
    /// <summary>
    /// Paints an isometric tiled floor for a GridSystem — now with real terrain.
    /// <para>
    /// The 114 CC0 Dungeon Crawl tiles under <c>Resources/Art/DCSS</c> shipped with
    /// the project but nothing ever loaded them, so this drew flat tinted cubes and
    /// the world read as grey rectangles. Walkable cells now take a floor texture
    /// and blocked cells a wall texture, both varied per tile so large rooms do not
    /// visibly repeat.
    /// </para>
    /// <para>
    /// Scenes pick their own palette by setting <see cref="floorFamily"/> and
    /// <see cref="wallFamily"/> before the component starts — "tomb" for the grey,
    /// "marble" for the elven court, "infernal" for Cinderhaunt, and so on. If a
    /// texture is missing the tile falls back to its original tint, so a partial
    /// art set degrades instead of breaking.
    /// </para>
    /// </summary>
    public class TileFloorRenderer : MonoBehaviour
    {
        [Header("Palette (DCSS families under Resources/Art/DCSS)")]
        [Tooltip("grey_dirt · infernal · marble · pebble · sandstone · tomb")]
        public string floorFamily = "tomb";
        [Tooltip("brick_brown · brick_dark · marble_wall · stone_dark · tomb_wall")]
        public string wallFamily = "brick_dark";
        [Tooltip("How many numbered variants to look for in each family.")]
        public int variants = 4;

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
                        // Keep a touch of the checker so the grid still reads at a glance.
                        mat.color = cell.walkable && ((x + y) & 1) == 0
                            ? Color.white : new Color(0.88f, 0.88f, 0.92f);
                    }
                    else
                    {
                        mat.color = !cell.walkable ? blocked : (((x + y) & 1) == 0 ? tileA : tileB);
                    }
                }
        }

        /// Loads "<family>0..N" from Resources/Art/DCSS/<group>, skipping gaps.
        private Texture2D[] LoadFamily(string group, string family)
        {
            if (string.IsNullOrEmpty(family)) return null;
            var found = new System.Collections.Generic.List<Texture2D>();
            for (int i = 0; i < Mathf.Max(1, variants); i++)
            {
                var t = Resources.Load<Texture2D>($"Art/DCSS/{group}/{family}{i}");
                if (t != null) found.Add(t);
            }
            if (found.Count == 0)                       // some families are unnumbered
            {
                var single = Resources.Load<Texture2D>($"Art/DCSS/{group}/{family}");
                if (single != null) found.Add(single);
            }
            return found.Count > 0 ? found.ToArray() : null;
        }

        /// Deterministic per-cell variant: the same tile every run, no visible tiling.
        private static Texture2D Pick(Texture2D[] set, int x, int y)
        {
            if (set == null || set.Length == 0) return null;
            int h = (x * 73856093) ^ (y * 19349663);
            return set[Mathf.Abs(h) % set.Length];
        }
    }
}
