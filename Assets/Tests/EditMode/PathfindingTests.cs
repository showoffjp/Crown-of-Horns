using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using SunderedCrown.Grid;

namespace SunderedCrown.Tests.EditMode
{
    /// <summary>
    /// A* over the GridSystem — the spine of both tactical movement and click-to-move
    /// exploration. These pin shortest-path correctness, wall detours, unreachability,
    /// the "step onto an occupied goal to attack but never path through bodies" rule,
    /// and difficult-terrain cost. Builds throwaway grids, so it runs headless.
    /// </summary>
    public class PathfindingTests
    {
        private readonly List<Object> _spawned = new List<Object>();

        private GridSystem MakeGrid(int w, int h)
        {
            var go = new GameObject("TestGrid");
            _spawned.Add(go);
            var g = go.AddComponent<GridSystem>();
            g.width = w;
            g.height = h;
            g.Build();
            return g;
        }

        [TearDown]
        public void Cleanup()
        {
            foreach (var o in _spawned) if (o != null) Object.DestroyImmediate(o);
            _spawned.Clear();
        }

        private static void AssertContiguous(List<GridCell> path)
        {
            for (int i = 1; i < path.Count; i++)
                Assert.AreEqual(1, GridSystem.ManhattanDistance(path[i - 1], path[i]),
                    "Each step must move to a 4-adjacent cell.");
        }

        [Test]
        public void StraightLine_IsShortestAndExcludesStart()
        {
            var g = MakeGrid(6, 6);
            var path = Pathfinding.FindPath(g, g.GetCell(0, 0), g.GetCell(3, 0));

            Assert.IsNotNull(path);
            Assert.AreEqual(3, path.Count, "Manhattan distance 3 ⇒ 3 steps (start excluded).");
            Assert.AreEqual(g.GetCell(3, 0), path[path.Count - 1], "Goal is included, last.");
            CollectionAssert.DoesNotContain(path, g.GetCell(0, 0), "Start is excluded.");
            AssertContiguous(path);
        }

        [Test]
        public void DiagonalTarget_PathLengthEqualsManhattan()
        {
            var g = MakeGrid(8, 8);
            var start = g.GetCell(0, 0);
            var goal = g.GetCell(2, 3);
            var path = Pathfinding.FindPath(g, start, goal);

            Assert.IsNotNull(path);
            Assert.AreEqual(GridSystem.ManhattanDistance(start, goal), path.Count);
            AssertContiguous(path);
        }

        [Test]
        public void Wall_ForcesDetour_AroundTheGap()
        {
            var g = MakeGrid(5, 5);
            // Wall the whole x=2 column except a gap at the top (2,4).
            for (int y = 0; y < 4; y++) g.GetCell(2, y).walkable = false;

            var path = Pathfinding.FindPath(g, g.GetCell(0, 0), g.GetCell(4, 0));
            Assert.IsNotNull(path, "A gap exists, so a (longer) path should be found.");
            Assert.Greater(path.Count, 4, "The detour must be longer than the blocked straight line.");
            foreach (var c in path) Assert.IsTrue(c.walkable, "Path never crosses an unwalkable cell.");
            Assert.AreEqual(g.GetCell(4, 0), path[path.Count - 1]);
        }

        [Test]
        public void FullyWalledOff_ReturnsNull()
        {
            var g = MakeGrid(5, 5);
            for (int y = 0; y < 5; y++) g.GetCell(2, y).walkable = false; // split the map
            Assert.IsNull(Pathfinding.FindPath(g, g.GetCell(0, 0), g.GetCell(4, 0)));
        }

        [Test]
        public void UnwalkableGoal_ReturnsNull()
        {
            var g = MakeGrid(4, 4);
            var goal = g.GetCell(3, 3);
            goal.walkable = false;
            Assert.IsNull(Pathfinding.FindPath(g, g.GetCell(0, 0), goal));
        }

        [Test]
        public void OccupiedTile_IsRoutedAround_WhenTreatedAsBlocked()
        {
            var g = MakeGrid(3, 3);
            g.GetCell(1, 0).occupant = new object(); // body on the direct route

            var path = Pathfinding.FindPath(g, g.GetCell(0, 0), g.GetCell(2, 0), treatOccupiedAsBlocked: true);
            Assert.IsNotNull(path);
            CollectionAssert.DoesNotContain(path, g.GetCell(1, 0), "Must not path through an occupied cell.");
            Assert.AreEqual(g.GetCell(2, 0), path[path.Count - 1]);
        }

        [Test]
        public void OccupiedTile_IsTraversed_WhenNotTreatedAsBlocked()
        {
            var g = MakeGrid(3, 3);
            g.GetCell(1, 0).occupant = new object();

            var path = Pathfinding.FindPath(g, g.GetCell(0, 0), g.GetCell(2, 0), treatOccupiedAsBlocked: false);
            Assert.IsNotNull(path);
            Assert.AreEqual(2, path.Count, "Free to pass straight through when bodies are ignored.");
        }

        [Test]
        public void OccupiedGoal_IsStillReachable_ForAttacks()
        {
            var g = MakeGrid(3, 3);
            var goal = g.GetCell(2, 0);
            goal.occupant = new object(); // the enemy you're walking up to hit

            var path = Pathfinding.FindPath(g, g.GetCell(0, 0), goal, treatOccupiedAsBlocked: true);
            Assert.IsNotNull(path, "You can step onto an occupied goal (to attack it).");
            Assert.AreEqual(goal, path[path.Count - 1]);
        }

        [Test]
        public void DifficultTerrain_IsAvoidedWhenADetourIsCheaper()
        {
            var g = MakeGrid(3, 3);
            g.GetCell(1, 0).moveCost = 10; // expensive tile on the straight line

            var path = Pathfinding.FindPath(g, g.GetCell(0, 0), g.GetCell(2, 0));
            Assert.IsNotNull(path);
            CollectionAssert.DoesNotContain(path, g.GetCell(1, 0),
                "A* should prefer the cheaper detour over the costly tile.");
        }

        [Test]
        public void NullArguments_ReturnNull()
        {
            var g = MakeGrid(3, 3);
            Assert.IsNull(Pathfinding.FindPath(null, g.GetCell(0, 0), g.GetCell(1, 0)));
            Assert.IsNull(Pathfinding.FindPath(g, null, g.GetCell(1, 0)));
            Assert.IsNull(Pathfinding.FindPath(g, g.GetCell(0, 0), null));
        }
    }
}
