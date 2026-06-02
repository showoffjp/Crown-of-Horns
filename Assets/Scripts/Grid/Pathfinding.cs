using System.Collections.Generic;
using UnityEngine;

namespace SunderedCrown.Grid
{
    /// <summary>
    /// A* pathfinding over the GridSystem. Returns the list of cells from start
    /// (exclusive) to goal (inclusive), or null if unreachable. Respects walkability,
    /// per-tile move cost (difficult terrain), and occupied cells.
    /// </summary>
    public static class Pathfinding
    {
        private class Node
        {
            public GridCell cell;
            public int g;      // cost from start
            public int f;      // g + heuristic
            public Node parent;
        }

        public static List<GridCell> FindPath(GridSystem grid, GridCell start, GridCell goal,
            bool treatOccupiedAsBlocked = true)
        {
            if (grid == null || start == null || goal == null) return null;
            if (!goal.walkable) return null;

            var open = new List<Node>();
            var allNodes = new Dictionary<GridCell, Node>();
            var closed = new HashSet<GridCell>();

            var startNode = new Node { cell = start, g = 0, f = GridSystem.ManhattanDistance(start, goal) };
            open.Add(startNode);
            allNodes[start] = startNode;

            while (open.Count > 0)
            {
                // Pick lowest f. (A binary heap would be faster for big maps.)
                int best = 0;
                for (int i = 1; i < open.Count; i++)
                    if (open[i].f < open[best].f) best = i;
                Node current = open[best];
                open.RemoveAt(best);

                if (current.cell == goal)
                    return Reconstruct(current);

                closed.Add(current.cell);

                foreach (var neighbour in grid.Neighbours(current.cell))
                {
                    if (closed.Contains(neighbour)) continue;
                    if (!neighbour.walkable) continue;
                    // Allow stepping onto the goal even if blocked by intent (e.g. attacking),
                    // but never path THROUGH occupied tiles.
                    bool blocked = treatOccupiedAsBlocked && neighbour.occupant != null && neighbour != goal;
                    if (blocked) continue;

                    int tentativeG = current.g + neighbour.moveCost;

                    if (!allNodes.TryGetValue(neighbour, out var nNode))
                    {
                        nNode = new Node { cell = neighbour };
                        allNodes[neighbour] = nNode;
                        nNode.g = int.MaxValue;
                    }

                    if (tentativeG < nNode.g)
                    {
                        nNode.g = tentativeG;
                        nNode.f = tentativeG + GridSystem.ManhattanDistance(neighbour, goal);
                        nNode.parent = current;
                        if (!open.Contains(nNode)) open.Add(nNode);
                    }
                }
            }

            return null; // no path
        }

        private static List<GridCell> Reconstruct(Node node)
        {
            var path = new List<GridCell>();
            while (node.parent != null)
            {
                path.Add(node.cell);
                node = node.parent;
            }
            path.Reverse();
            return path;
        }
    }
}
