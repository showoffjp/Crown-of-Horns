// Objective check of the isometric projection's click-picking. The combat board renders
// in iso but keeps all game logic on the square grid, so the one interactive-critical
// piece is screenToCell() inverting the grid→screen transform. We pull the REAL iso math
// out of crown_combat.html and prove it round-trips for every cell (and for points spread
// across each diamond), so clicking a tile always selects that tile.
const fs = require("fs");
let pass = 0, fail = 0; const fails = [];
function check(name, cond) { if (cond) pass++; else { fail++; fails.push(name); } }

const h = fs.readFileSync(__dirname + "/crown_combat.html", "utf8");

// lift the actual projection code from the page and run it for real
const block = h.match(/const HW=[\s\S]*?function screenToCell\(sx,sy\)\{[^\n]*\}/);
check("iso math block found in the page", !!block);
const api = new Function(block[0] + "\nreturn { HW, HH, OX, OY, isoX, isoY, cen, screenToCell };")();
const { cen, isoX, isoY, screenToCell, HW, HH } = api;
const COLS = 14, ROWS = 10;

// 1) every tile centre round-trips to its own cell
let centreOk = true;
for (let gx = 0; gx < COLS; gx++) for (let gy = 0; gy < ROWS; gy++) {
  const c = cen(gx, gy), s = screenToCell(c.x, c.y);
  if (s.x !== gx || s.y !== gy) { centreOk = false; fails.push(`centre (${gx},${gy})→(${s.x},${s.y})`); }
}
check("every tile centre round-trips", centreOk);

// 2) points spread across each diamond's interior also resolve to that cell
let interiorOk = true, samples = 0;
for (let gx = 0; gx < COLS; gx++) for (let gy = 0; gy < ROWS; gy++) {
  const c = cen(gx, gy);
  // stay well inside the rhombus: |dx|/HW + |dy|/HH <= ~0.8
  for (const [ox, oy] of [[0, 0], [HW * 0.5, 0], [-HW * 0.5, 0], [0, HH * 0.5], [0, -HH * 0.5], [HW * 0.35, HH * 0.35]]) {
    const s = screenToCell(c.x + ox, c.y + oy); samples++;
    if (s.x !== gx || s.y !== gy) { interiorOk = false; }
  }
}
check(`interior points resolve to their tile (${samples} samples)`, interiorOk);

// 3) distinct cells never share a centre (injective projection)
const seen = new Set(); let injective = true;
for (let gx = 0; gx < COLS; gx++) for (let gy = 0; gy < ROWS; gy++) {
  const c = cen(gx, gy), k = c.x + ":" + c.y;
  if (seen.has(k)) injective = false; seen.add(k);
}
check("projection is injective (no overlapping tiles)", injective);

// 4) the board fits inside the 616×496 canvas
let inBounds = true;
for (let gx = 0; gx <= COLS; gx++) for (let gy = 0; gy <= ROWS; gy++) {
  const x = isoX(gx, gy), y = isoY(gx, gy);
  if (x < 0 || x > 616 || y < -20 || y > 496) inBounds = false;
}
check("the iso board fits the canvas", inBounds);

// 5) the page is actually wired to the iso renderer + picking
check("page uses iso renderer (walls/units/props, depth-sorted)",
  h.includes("function wallBlock") && h.includes("function unitBill") &&
  h.includes("PROPS") && h.includes("ents.sort"));
check("page picks tiles via screenToCell", h.includes("screenToCell(mx,my)"));
check("page keeps persistent decals", h.includes("decals.push"));
check("page animates the figures (lunge / recoil / facing / breathing)",
  h.includes("function faceLunge") && h.includes("function recoil") &&
  h.includes("anim.kind") && h.includes("breathe") && h.includes("face===-1"));

console.log(`\n  Isometric projection — round-trip + wiring:`);
console.log(`  ${pass} passed, ${fail} failed`);
if (fail) { fails.slice(0, 10).forEach(f => console.log("   ✗ " + f)); process.exit(1); }
else console.log(`  ✓ ${COLS * ROWS} tiles round-trip, projection injective & in-bounds, iso renderer wired.\n`);
