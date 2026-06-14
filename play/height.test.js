// Objective check of height advantage. We lift the page's real elevation helpers (the
// marked /*<HEIGHT>*/ block) and prove that striking from higher ground yields advantage,
// from below yields disadvantage, and level ground neither — then assert the modifier is
// wired into the hit math, the forecast, and the render.
const fs = require("fs");
let pass = 0, fail = 0; const fails = [];
function check(name, cond) { if (cond) pass++; else { fail++; fails.push(name); } }

const h = fs.readFileSync(__dirname + "/crown_combat.html", "utf8");
const block = h.match(/\/\*<HEIGHT>\*\/([\s\S]*?)\/\*<\/HEIGHT>\*\//);
check("height helper block found in the page", !!block);
const prelude = 'const key=(x,y)=>x+","+y;const HIGH=new Set(["8,4","9,4","8,5","9,5"]);';
const { elevOf, heightVs } = new Function(prelude + block[1] + "\nreturn { elevOf, heightVs };")();

check("high-ground tiles read as elevated", elevOf(8, 4) === 1 && elevOf(9, 5) === 1);
check("ordinary tiles read as level", elevOf(0, 0) === 0 && elevOf(7, 4) === 0);

const hi = { x: 8, y: 4 }, lo = { x: 1, y: 1 }, hi2 = { x: 9, y: 5 }, lo2 = { x: 2, y: 2 };
check("striking from high ground → advantage (>0)", heightVs(hi, lo) > 0);
check("striking from below → disadvantage (<0)", heightVs(lo, hi) < 0);
check("level ground → no height modifier (0)", heightVs(lo, lo2) === 0 && heightVs(hi, hi2) === 0);

// wired into the engine, the forecast, and the view
check("resolve folds height into adv/disadvantage",
  h.includes("const hv=heightVs(att,def)") && h.includes("hv>0") && h.includes("hv<0"));
check("forecast + threat account for height", (h.match(/heightVs\(att,def\)/g) || []).length >= 3);
check("high ground is rendered + reported", h.includes("function floorTile") &&
  h.includes("elevOf(gx,gy)") && h.includes("high ground") && h.includes("elevOf(u.x,u.y)*ELEV_H"));

console.log(`\n  Height advantage — adv/disadvantage + wiring:`);
console.log(`  ${pass} passed, ${fail} failed`);
if (fail) { fails.forEach(f => console.log("   ✗ " + f)); process.exit(1); }
else console.log(`  ✓ high ground grants advantage, low ground disadvantage; wired into hit math, forecast & view.\n`);
