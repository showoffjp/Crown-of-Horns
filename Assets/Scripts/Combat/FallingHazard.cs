using System.Collections.Generic;
using UnityEngine;
using SunderedCrown.Grid;
using SunderedCrown.Stats;

namespace SunderedCrown.Combat
{
    /// <summary>
    /// THE FALLING CITY. An environmental hazard for the Netheril era: as a battle rages on
    /// a Netherese enclave plummeting out of the sky, the floor itself gives way. After a short
    /// grace period, tiles collapse each turn (preferring the edges — the city crumbles from its
    /// rim inward), becoming impassable voids; a unit caught on a collapsing tile takes falling
    /// damage. It never collapses below a safe core, so the fight stays winnable, just frantic.
    ///
    /// Add to the CombatManager (EncounterBuilder does this when environmentalHazard = true).
    /// </summary>
    public class FallingHazard : MonoBehaviour
    {
        [Tooltip("Turns of calm before the floor starts to go.")]
        public int graceTurns = 3;
        [Tooltip("Tiles that collapse on each turn after the grace period.")]
        public int collapsesPerTurn = 1;
        public string fallingDamage = "2d6";
        [Tooltip("Never collapse below this many walkable tiles, so the fight can't soft-lock.")]
        public int minWalkableFloor = 14;

        private TurnManager _turns;
        private GridSystem _grid;
        private int _turnCount;

        void Start()
        {
            _turns = TurnManager.Instance;
            _grid = GridSystem.Instance;
            if (_turns != null) _turns.OnTurnStarted += OnTurnStarted;
        }

        void OnDestroy()
        {
            if (_turns != null) _turns.OnTurnStarted -= OnTurnStarted;
        }

        private void OnTurnStarted(GridUnit unit)
        {
            _turnCount++;
            if (_turnCount < graceTurns) return;
            if (_turnCount == graceTurns)
            {
                _turns.Log("⚠ The enclave is falling — the floor won't hold much longer!");
                return;
            }
            for (int i = 0; i < collapsesPerTurn; i++) CollapseOne();
        }

        private void CollapseOne()
        {
            var walkable = GatherWalkable();
            if (walkable.Count <= minWalkableFloor) return;

            var cell = PickEdgeBiased(walkable);
            if (cell == null) return;

            cell.walkable = false;
            cell.moveCost = 99;

            // Visual: a dark void where the floor was.
            var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = "Void";
            go.transform.SetParent(transform);
            Vector3 w = _grid.GridToWorld(cell.x, cell.y);
            go.transform.position = new Vector3(w.x, w.y, 0.2f);
            go.transform.localScale = new Vector3(_grid.tileWidth * 0.9f, _grid.tileHeight * 0.9f, 0.05f);
            go.GetComponent<Renderer>().material.color = new Color(0.02f, 0.02f, 0.07f);
            var col = go.GetComponent<Collider>();
            if (col != null) Destroy(col);

            // Damage anyone standing on the tile as it gives way.
            if (cell.occupant is GridUnit occ && occ.Sheet.IsAlive)
            {
                int dmg = Dice.Roll(fallingDamage);
                occ.Sheet.TakeDamage(dmg);
                _turns.Log($"The floor gives way beneath {occ.Sheet.displayName} — {dmg} falling damage!");
                SunderedCrown.FX.FxSystem.PlayImpact(DamageType.Force, w);
                if (!occ.Sheet.IsAlive) _turns.Log($"{occ.Sheet.displayName} is lost to the falling sky!");
            }
        }

        private List<GridCell> GatherWalkable()
        {
            var list = new List<GridCell>();
            for (int x = 0; x < _grid.width; x++)
                for (int y = 0; y < _grid.height; y++)
                {
                    var c = _grid.GetCell(x, y);
                    if (c != null && c.walkable) list.Add(c);
                }
            return list;
        }

        /// <summary>Weighted-random pick favouring tiles closest to the map edge.</summary>
        private GridCell PickEdgeBiased(List<GridCell> cells)
        {
            float total = 0f;
            foreach (var c in cells) total += Weight(c);
            if (total <= 0f) return null;
            float r = Random.value * total;
            foreach (var c in cells)
            {
                r -= Weight(c);
                if (r <= 0f) return c;
            }
            return cells[cells.Count - 1];
        }

        private float Weight(GridCell c)
        {
            int borderDist = Mathf.Min(Mathf.Min(c.x, _grid.width - 1 - c.x),
                                       Mathf.Min(c.y, _grid.height - 1 - c.y));
            return 1f / (borderDist + 1f); // edges (dist 0) weigh most
        }
    }
}
