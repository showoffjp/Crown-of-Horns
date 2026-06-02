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
        }

        /// <summary>Instantly occupy a cell (used at spawn and after teleports).</summary>
        public void PlaceAt(GridCell cell)
        {
            if (cell == null) return;
            if (Cell != null && Cell.occupant == this) Cell.occupant = null;
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
                if (Cell != null && Cell.occupant == this) Cell.occupant = null;
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
