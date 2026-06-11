// Pathfinding tests: scenario checks mirroring PathfindingTests.cs, plus the
// load-bearing OPTIMALITY FUZZ — over thousands of random weighted maps, A*'s path
// cost must equal the brute-force Dijkstra optimum (and agree on unreachability).
// That proves the Manhattan heuristic stays admissible and A* never returns a
// merely-valid, sub-optimal path.
const { makeGrid, findPath, optimalCost, pathCost } = require("./pathfind.js");

let pass = 0, fail = 0; const fails = [];
function test(name, fn) { try { fn(); pass++; } catch (e) { fail++; fails.push(`${name}: ${e.message}`); } }
const eq = (a, b, m) => { if (a !== b) throw new Error(`${m || ""} expected ${b}, got ${a}`); };
const T = (c, m) => { if (!c) throw new Error(m || "expected true"); };

function contiguous(path) { for (let i = 1; i < path.length; i++) if (Math.abs(path[i].x - path[i - 1].x) + Math.abs(path[i].y - path[i - 1].y) !== 1) return false; return true; }

test("StraightLine_OnOpenGrid", () => {
  const g = makeGrid(6, 1);
  const path = findPath(g, g.at(0, 0), g.at(5, 0));
  eq(path.length, 5, "5 steps to cross 5 tiles");
  eq(path[path.length - 1], g.at(5, 0), "ends at goal");
  T(!path.includes(g.at(0, 0)), "path excludes start");
  T(contiguous(path)); eq(pathCost(path), 5);
});

test("StartEqualsGoal_IsEmptyPath", () => {
  const g = makeGrid(4, 4);
  const path = findPath(g, g.at(2, 2), g.at(2, 2));
  eq(path.length, 0);
});

test("WallDetour_MatchesOptimal", () => {
  const g = makeGrid(5, 5);
  for (let y = 0; y < 4; y++) g.at(2, y).walkable = false; // wall with a gap at the bottom
  const start = g.at(0, 2), goal = g.at(4, 2);
  const path = findPath(g, start, goal);
  T(path && path.length > 0, "a detour exists");
  T(contiguous(path));
  eq(path[path.length - 1], goal);
  eq(pathCost(path), optimalCost(g, start, goal), "detour is optimal");
});

test("Unreachable_ReturnsNull", () => {
  const g = makeGrid(5, 5);
  for (let y = 0; y < 5; y++) g.at(2, y).walkable = false; // full wall
  eq(findPath(g, g.at(0, 0), g.at(4, 0)), null);
  eq(optimalCost(g, g.at(0, 0), g.at(4, 0)), Infinity, "oracle agrees it's unreachable");
});

test("UnwalkableGoal_ReturnsNull", () => {
  const g = makeGrid(4, 4); g.at(3, 3).walkable = false;
  eq(findPath(g, g.at(0, 0), g.at(3, 3)), null);
});

test("OccupiedGoal_IsReachable_ButNeverPathThroughBodies", () => {
  const g = makeGrid(5, 1);
  g.at(4, 0).occupant = true;                 // goal occupied (attack target)
  let path = findPath(g, g.at(0, 0), g.at(4, 0));
  T(path && path[path.length - 1] === g.at(4, 0), "can step onto an occupied goal");
  // now block the only corridor with a body that is NOT the goal
  const g2 = makeGrid(5, 1); g2.at(2, 0).occupant = true;
  eq(findPath(g2, g2.at(0, 0), g2.at(4, 0)), null, "cannot path through an occupant");
});

test("DifficultTerrain_PrefersCheaperRoute", () => {
  // direct row has a costly tile; a detour row is cheap. A* must take the cheaper total.
  const g = makeGrid(3, 2);
  g.at(1, 0).moveCost = 9; // expensive middle on the top row
  const start = g.at(0, 0), goal = g.at(2, 0);
  const path = findPath(g, start, goal);
  eq(pathCost(path), optimalCost(g, start, goal), "took the optimal-cost route");
  T(!path.includes(g.at(1, 0)), "avoided the 9-cost tile via the cheaper detour");
});

// ---------- the load-bearing optimality fuzz ----------
test("Fuzz_AStarIsAlwaysOptimal_OverRandomWeightedMaps", () => {
  let seed = 0x5EED;
  const rnd = () => (seed = (seed * 1103515245 + 12345) & 0x7fffffff) / 0x7fffffff;
  const N = 3000;
  for (let it = 0; it < N; it++) {
    const w = 2 + Math.floor(rnd() * 7), h = 2 + Math.floor(rnd() * 7);
    const g = makeGrid(w, h);
    for (const c of g.cells) {
      if (rnd() < 0.25) c.walkable = false;
      else { c.moveCost = 1 + Math.floor(rnd() * 3); if (rnd() < 0.12) c.occupant = true; }
    }
    const pick = () => g.cells[Math.floor(rnd() * g.cells.length)];
    const start = pick(), goal = pick();
    start.walkable = true; start.occupant = false; goal.walkable = true; // start clear, goal walkable
    if (start === goal) continue; // trivial empty-path case, covered by StartEqualsGoal scenario
    const path = findPath(g, start, goal);
    const opt = optimalCost(g, start, goal);
    if (path === null) {
      if (opt !== Infinity) throw new Error(`A* said unreachable but optimum is ${opt} (iter ${it})`);
    } else {
      if (!contiguous(path)) throw new Error(`non-contiguous path (iter ${it})`);
      if (path[path.length - 1] !== goal) throw new Error(`path doesn't end at goal (iter ${it})`);
      if (pathCost(path) !== opt) throw new Error(`A* cost ${pathCost(path)} != optimal ${opt} (iter ${it})`);
    }
  }
});

console.log(`\n  Pathfinding — A* correctness + optimality vs a brute-force Dijkstra oracle (3000 random maps):`);
console.log(`  ${pass} passed, ${fail} failed`);
if (fail) { fails.forEach(f => console.log("   ✗ " + f)); process.exit(1); }
else console.log("  ✓ Every A* path is contiguous, valid, and provably least-cost.\n");
