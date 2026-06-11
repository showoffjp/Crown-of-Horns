// ============================================================================
// Pathfinding — headless port of Assets/Scripts/Grid/Pathfinding.cs (A* over the
// grid: 4-connected, weighted by per-tile moveCost, Manhattan heuristic, "step onto
// an occupied goal but never path through bodies"). Ships with a brute-force Dijkstra
// oracle so pathfind.test.js can prove A* returns *optimal* paths, not merely *a* path.
// ============================================================================

// Grid model: { w, h, cells:[ {x,y,walkable,occupant,moveCost} ] } indexed y*w+x.
function makeGrid(w, h) {
  const cells = [];
  for (let y = 0; y < h; y++) for (let x = 0; x < w; x++) cells.push({ x, y, walkable: true, occupant: false, moveCost: 1 });
  return { w, h, cells, at: (x, y) => cells[y * w + x] };
}
const manhattan = (a, b) => Math.abs(a.x - b.x) + Math.abs(a.y - b.y);
function neighbours(grid, c) {
  const r = [];
  for (const [dx, dy] of [[1, 0], [-1, 0], [0, 1], [0, -1]]) {
    const nx = c.x + dx, ny = c.y + dy;
    if (nx >= 0 && ny >= 0 && nx < grid.w && ny < grid.h) r.push(grid.cells[ny * grid.w + nx]);
  }
  return r;
}

// A*, ported 1:1. Returns the list of cells from start (exclusive) to goal (inclusive), or null.
function findPath(grid, start, goal, treatOccupiedAsBlocked = true) {
  if (!grid || !start || !goal) return null;
  if (!goal.walkable) return null;

  const open = [], all = new Map(), closed = new Set();
  const startNode = { cell: start, g: 0, f: manhattan(start, goal), parent: null };
  open.push(startNode); all.set(start, startNode);

  while (open.length > 0) {
    let best = 0;
    for (let i = 1; i < open.length; i++) if (open[i].f < open[best].f) best = i;
    const current = open[best]; open.splice(best, 1);

    if (current.cell === goal) return reconstruct(current);
    closed.add(current.cell);

    for (const nb of neighbours(grid, current.cell)) {
      if (closed.has(nb)) continue;
      if (!nb.walkable) continue;
      const blocked = treatOccupiedAsBlocked && nb.occupant && nb !== goal; // step onto goal, never through bodies
      if (blocked) continue;

      const tentativeG = current.g + nb.moveCost;
      let nNode = all.get(nb);
      if (!nNode) { nNode = { cell: nb, g: Infinity, f: 0, parent: null }; all.set(nb, nNode); }
      if (tentativeG < nNode.g) {
        nNode.g = tentativeG;
        nNode.f = tentativeG + manhattan(nb, goal);
        nNode.parent = current;
        if (!open.includes(nNode)) open.push(nNode);
      }
    }
  }
  return null;
}
function reconstruct(node) {
  const path = [];
  while (node.parent) { path.push(node.cell); node = node.parent; }
  path.reverse();
  return path;
}

// Brute-force optimal cost (Dijkstra) — the independent oracle. Cost of a path = sum of
// the moveCost of each cell *entered* (so start is free), matching A*'s g at the goal.
function optimalCost(grid, start, goal, treatOccupiedAsBlocked = true) {
  if (!goal.walkable) return Infinity;
  const dist = new Map([[start, 0]]);
  const pq = [[0, start]];
  while (pq.length) {
    let bi = 0; for (let i = 1; i < pq.length; i++) if (pq[i][0] < pq[bi][0]) bi = i;
    const [d, c] = pq.splice(bi, 1)[0];
    if (c === goal) return d;
    if (d > (dist.get(c) ?? Infinity)) continue;
    for (const nb of neighbours(grid, c)) {
      if (!nb.walkable) continue;
      if (treatOccupiedAsBlocked && nb.occupant && nb !== goal) continue;
      const nd = d + nb.moveCost;
      if (nd < (dist.get(nb) ?? Infinity)) { dist.set(nb, nd); pq.push([nd, nb]); }
    }
  }
  return Infinity;
}

const pathCost = (path) => path.reduce((s, c) => s + c.moveCost, 0);

module.exports = { makeGrid, manhattan, neighbours, findPath, optimalCost, pathCost };
