using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SunderedCrown.Characters;

namespace SunderedCrown.Grid
{
    public enum Faction { Player, Ally, Enemy, Neutral }

    /// <summary>
    /// The on-map representation of a creature. Bridges the logical CharacterSheet
    /// with a position on the GridSystem and a visual transform. Handles smooth
    /// movement along a path. Attach to each character prefab.
    /// </summary>
    [RequireComponent(typeof(Transform))]
    public class GridUnit : MonoBehaviour
    {
        [Header("Identity")]
        public Faction faction = Faction.Enemy;
        public Vector2Int startCoord;

        [Header("Movement")]
        public float moveSpeed = 4f; // world units / second

        // The sheet is assigned at spawn time by the encounter/party loader.
        public CharacterSheet Sheet { get; set; } = new CharacterSheet();

        public GridCell Cell { get; private set; }
        public bool IsMoving { get; private set; }

        private GridSystem _grid;

        void Start()
        {
            _grid = GridSystem.Instance;
            PlaceAt(_grid.GetCell(startCoord));
            TryApplySprite();
        }

        /// <summary>
        /// If an art pack provides a sprite named after this unit (Resources/Sprites/&lt;name&gt;),
        /// show it as a billboard and hide the placeholder cube. No art → keep the cube. This is how
        /// the game goes from prototype cubes to a real CRPG look just by dropping in art.
        /// </summary>
        private void TryApplySprite()
        {
            string id = (Sheet != null && !string.IsNullOrEmpty(Sheet.displayName)) ? Sheet.displayName : name;
            var sprite = SunderedCrown.Rendering.WorldArt.Sprite(id);
            if (sprite == null) return;

            var mesh = GetComponent<MeshRenderer>();
            if (mesh != null) mesh.enabled = false; // hide the cube
            var col = GetComponent<Collider>();
            if (col != null) col.enabled = false;

            var go = new GameObject("Sprite");
            go.transform.SetParent(transform, false);
            // The unit cube is scaled to 0.6; counter it so the sprite reads at ~1 tile, lifted a touch.
            go.transform.localScale = Vector3.one * (1f / 0.6f);
            go.transform.localPosition = new Vector3(0, 0.25f, -0.2f);
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = sprite;
            sr.sortingOrder = 10;
        }

        /// <summary>Instantly occupy a cell (used at spawn and after teleports).</summary>
        public void PlaceAt(GridCell cell)
        {
            if (cell == null) return;
            if (Cell != null && (object)Cell.occupant == this) Cell.occupant = null;
            Cell = cell;
            cell.occupant = this;
            transform.position = SortedWorld(_grid.GridToWorld(cell.x, cell.y));
        }

        /// <summary>
        /// Walk the unit along a precomputed path, one tile at a time.
        /// Returns a coroutine you can yield on.
        /// </summary>
        public IEnumerator FollowPath(List<GridCell> path)
        {
            if (path == null || path.Count == 0) yield break;
            IsMoving = true;

            foreach (var step in path)
            {
                if (Cell != null && (object)Cell.occupant == this) Cell.occupant = null;
                Vector3 target = SortedWorld(_grid.GridToWorld(step.x, step.y));
                while ((transform.position - target).sqrMagnitude > 0.0001f)
                {
                    transform.position = Vector3.MoveTowards(
                        transform.position, target, moveSpeed * Time.deltaTime);
                    yield return null;
                }
                Cell = step;
                step.occupant = this;
            }

            IsMoving = false;
        }

        /// <summary>
        /// In a 2D isometric scene, draw order = depth. Lower on screen (and "further"
        /// in grid terms) should render in front. We bake that into Z here; a
        /// SpriteRenderer with sortingOrder = -(x+y) works too.
        /// </summary>
        private Vector3 SortedWorld(Vector3 world)
        {
            world.z = (Cell != null) ? (Cell.x + Cell.y) * 0.01f : world.z;
            return world;
        }
    }
}
