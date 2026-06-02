using System;

namespace SunderedCrown.Grid
{
    /// <summary>One logical tile on the battle/exploration map.</summary>
    [Serializable]
    public class GridCell
    {
        public readonly int x;
        public readonly int y;

        /// <summary>Can creatures stand here? (walls, water, chasms = false)</summary>
        public bool walkable = true;

        /// <summary>Extra movement cost (difficult terrain = 2). Base cost is 1.</summary>
        public int moveCost = 1;

        /// <summary>The unit currently occupying this cell, if any. Null when empty.</summary>
        public object occupant; // typed as object to avoid a hard dependency cycle; cast to GridUnit at call sites.

        public bool IsFree => walkable && occupant == null;

        public GridCell(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public override string ToString() => $"({x},{y})";
    }
}
