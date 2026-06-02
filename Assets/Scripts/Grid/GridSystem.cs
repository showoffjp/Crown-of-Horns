using System.Collections.Generic;
using UnityEngine;

namespace SunderedCrown.Grid
{
    /// <summary>
    /// Owns the logical tile grid and converts between grid coordinates and
    /// isometric world positions. Attach to an empty GameObject in your scene
    /// ("GridSystem") and size it in the inspector.
    ///
    /// ISOMETRIC NOTE: We use a "2:1 diamond" projection — the classic CRPG look.
    /// A tile that is `tileWidth` wide is `tileHeight` (= tileWidth/2) tall on screen.
    /// Grid (x,y) maps to world so that +x goes screen-right-down and +y goes
    /// screen-left-down, producing diamonds. Swap GridToWorld for a different look.
    /// </summary>
    public class GridSystem : MonoBehaviour
    {
        public static GridSystem Instance { get; private set; }

        [Header("Dimensions")]
        public int width = 20;
        public int height = 20;

        [Header("Isometric Projection (world units)")]
        [Tooltip("Full width of one tile diamond in world units.")]
        public float tileWidth = 1f;
        [Tooltip("Full height of one tile diamond. For true 2:1 iso this is tileWidth/2.")]
        public float tileHeight = 0.5f;

        private GridCell[,] _cells;

        void Awake()
        {
            Instance = this;
            Build();
        }

        public void Build()
        {
            _cells = new GridCell[width, height];
            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                    _cells[x, y] = new GridCell(x, y);
        }

        public bool InBounds(int x, int y) => x >= 0 && y >= 0 && x < width && y < height;

        public GridCell GetCell(int x, int y) => InBounds(x, y) ? _cells[x, y] : null;
        public GridCell GetCell(Vector2Int c) => GetCell(c.x, c.y);

        /// <summary>Convert grid coordinates to an isometric world position.</summary>
        public Vector3 GridToWorld(int x, int y)
        {
            // Diamond projection. Z is used as screen-depth via sorting; Y is screen height.
            float worldX = (x - y) * (tileWidth * 0.5f);
            float worldY = (x + y) * (tileHeight * 0.5f);
            return transform.position + new Vector3(worldX, worldY, 0f);
        }

        public Vector3 GridToWorld(Vector2Int c) => GridToWorld(c.x, c.y);

        /// <summary>Inverse projection: world position back to the nearest grid cell.</summary>
        public Vector2Int WorldToGrid(Vector3 world)
        {
            Vector3 local = world - transform.position;
            float a = local.x / (tileWidth * 0.5f);
            float b = local.y / (tileHeight * 0.5f);
            int x = Mathf.RoundToInt((a + b) * 0.5f);
            int y = Mathf.RoundToInt((b - a) * 0.5f);
            return new Vector2Int(x, y);
        }

        /// <summary>4-directional neighbours (no diagonal movement by default).</summary>
        public IEnumerable<GridCell> Neighbours(GridCell cell)
        {
            if (cell == null) yield break;
            var dirs = new (int dx, int dy)[] { (1, 0), (-1, 0), (0, 1), (0, -1) };
            foreach (var (dx, dy) in dirs)
            {
                var n = GetCell(cell.x + dx, cell.y + dy);
                if (n != null) yield return n;
            }
        }

        public static int ManhattanDistance(GridCell a, GridCell b) =>
            Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);

        // Draw the grid in the editor so you can see the playfield without art.
        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.cyan;
            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                {
                    Vector3 c = GridToWorld(x, y);
                    Vector3 top = GridToWorld(x, y) + new Vector3(0, tileHeight * 0.5f, 0);
                    Vector3 right = GridToWorld(x, y) + new Vector3(tileWidth * 0.5f, 0, 0);
                    Vector3 bottom = GridToWorld(x, y) + new Vector3(0, -tileHeight * 0.5f, 0);
                    Vector3 left = GridToWorld(x, y) + new Vector3(-tileWidth * 0.5f, 0, 0);
                    Gizmos.DrawLine(top, right);
                    Gizmos.DrawLine(right, bottom);
                    Gizmos.DrawLine(bottom, left);
                    Gizmos.DrawLine(left, top);
                }
        }
    }
}
