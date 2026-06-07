using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using SunderedCrown.Characters;
using SunderedCrown.Grid;

namespace SunderedCrown.Tests.PlayMode
{
    /// <summary>
    /// PlayMode coverage for GridUnit — the bridge between the logical CharacterSheet and a
    /// position on the live GridSystem. Pins spawn placement, teleport (PlaceAt) occupancy
    /// hand-off, walking a path tile-by-tile (occupancy follows, prior cells clear), and the
    /// null/empty-path no-op. Movement runs over real frames.
    /// </summary>
    public class GridUnitPlayTests
    {
        private readonly List<Object> _spawned = new List<Object>();
        private GridSystem _grid;

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            var go = new GameObject("Grid");
            _spawned.Add(go);
            _grid = go.AddComponent<GridSystem>();
            _grid.width = 8;
            _grid.height = 8;
            _grid.Build();
            yield return null;
        }

        [TearDown]
        public void TearDown()
        {
            foreach (var o in _spawned) if (o != null) Object.DestroyImmediate(o);
            _spawned.Clear();
        }

        private GridUnit Unit(int x, int y)
        {
            var go = new GameObject("Unit");
            _spawned.Add(go);
            var u = go.AddComponent<GridUnit>();
            u.faction = Faction.Player;
            u.startCoord = new Vector2Int(x, y);
            u.moveSpeed = 50f; // fast, so the walk resolves in a few frames
            u.Sheet = new CharacterSheet { displayName = "Walker", maxHitPoints = 10, currentHitPoints = 10 };
            return u;
        }

        private IEnumerator WaitWhileMoving(GridUnit u)
        {
            for (int i = 0; i < 600 && u.IsMoving; i++) yield return null;
        }

        [UnityTest]
        public IEnumerator Start_PlacesUnitAtStartCoord_AndOccupies()
        {
            var u = Unit(2, 3);
            yield return null; // GridUnit.Start

            Assert.AreSame(_grid.GetCell(2, 3), u.Cell);
            Assert.AreSame(u, _grid.GetCell(2, 3).occupant);
        }

        [UnityTest]
        public IEnumerator PlaceAt_MovesOccupancy_AndClearsOldCell()
        {
            var u = Unit(0, 0);
            yield return null;

            u.PlaceAt(_grid.GetCell(5, 5));

            Assert.AreSame(_grid.GetCell(5, 5), u.Cell);
            Assert.AreSame(u, _grid.GetCell(5, 5).occupant);
            Assert.IsNull(_grid.GetCell(0, 0).occupant, "The old cell is vacated.");
        }

        [UnityTest]
        public IEnumerator FollowPath_WalksToEnd_AndOccupancyFollows()
        {
            var u = Unit(0, 0);
            yield return null;

            var path = new List<GridCell> { _grid.GetCell(1, 0), _grid.GetCell(2, 0) };
            u.StartCoroutine(u.FollowPath(path));
            yield return WaitWhileMoving(u);

            Assert.IsFalse(u.IsMoving, "Walk completes.");
            Assert.AreSame(_grid.GetCell(2, 0), u.Cell, "Ends on the last path cell.");
            Assert.AreSame(u, _grid.GetCell(2, 0).occupant, "Destination is occupied.");
            Assert.IsNull(_grid.GetCell(0, 0).occupant, "Start cell vacated.");
            Assert.IsNull(_grid.GetCell(1, 0).occupant, "Transit cell vacated as it passed through.");
        }

        [UnityTest]
        public IEnumerator FollowPath_NullOrEmpty_IsANoOp()
        {
            var u = Unit(4, 4);
            yield return null;

            u.StartCoroutine(u.FollowPath(null));
            yield return null;
            Assert.IsFalse(u.IsMoving);
            Assert.AreSame(_grid.GetCell(4, 4), u.Cell, "Stays put on a null path.");

            u.StartCoroutine(u.FollowPath(new List<GridCell>()));
            yield return null;
            Assert.IsFalse(u.IsMoving);
            Assert.AreSame(_grid.GetCell(4, 4), u.Cell, "Stays put on an empty path.");
        }
    }
}
