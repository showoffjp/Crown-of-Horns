using UnityEngine;
using SunderedCrown.Grid;

namespace SunderedCrown.Rendering
{
    /// <summary>
    /// Paints a simple isometric tiled floor for a GridSystem, so scenes read as a tiled
    /// playfield instead of empty space — a real visual upgrade with no art required. Drop it
    /// on (or AddComponent it to) any scene that has a GridSystem; it builds flat, checker-tinted
    /// diamonds under the units. Non-walkable cells are drawn darker (walls/voids). When an art
    /// pack exists, swap the colored quads for a floor sprite (see docs/ASSET_INTEGRATION.md).
    /// </summary>
    public class TileFloorRenderer : MonoBehaviour
    {
        public Color tileA = new Color(0.16f, 0.16f, 0.20f);
        public Color tileB = new Color(0.13f, 0.13f, 0.17f);
        public Color blocked = new Color(0.05f, 0.05f, 0.07f);
        [Tooltip("Draw order depth; larger = further behind the units.")]
        public float depth = 0.5f;

        void Start()
        {
            var grid = GridSystem.Instance;
            if (grid == null) return;

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

                    Color c = !cell.walkable ? blocked : (((x + y) & 1) == 0 ? tileA : tileB);
                    tile.GetComponent<Renderer>().material.color = c;
                }
        }
    }
}
