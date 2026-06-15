// Objective check of the environmental-surfaces combos. We lift the REAL surface logic
// out of crown_combat.html (the marked /*<SURF>*/ block) and run it headless to prove the
// BG3-style interactions: grease ignites into fire, fire spreads through adjacent grease,
// water douses fire, surfaces decay, and nothing spills out of bounds.
const fs = require("fs");
let pass = 0, fail = 0; const fails = [];
function check(name, cond) { if (cond) pass++; else { fail++; fails.push(name); } }

const h = fs.readFileSync(__dirname + "/crown_combat.html", "utf8");
const block = h.match(/\/\*<SURF>\*\/([\s\S]*?)\/\*<\/SURF>\*\//);
check("surface logic block found in the page", !!block);

const prelude =
  "const COLS=14,ROWS=10;const key=(x,y)=>x+','+y;" +
  "const inB=(x,y)=>x>=0&&y>=0&&x<COLS&&y<ROWS;const blocked=new Set();let surfaces=new Map();";
const api = new Function(prelude + block[1] +
  "\nreturn { surfaces, paintSurface, igniteAround, createSurfaceArea, surfaceTick, isWaterAt };")();
const { surfaces, createSurfaceArea, paintSurface, surfaceTick, isWaterAt } = api;
const typesOf = () => [...surfaces.values()].map(s => s.type);
const count = t => typesOf().filter(x => x === t).length;

// 1) lay a 3×3 grease slick
surfaces.clear();
createSurfaceArea(5, 5, 1, "grease");
check("oil flask lays a 3×3 grease slick (9 tiles)", surfaces.size === 9 && count("grease") === 9);

// 2) igniting the centre catches the whole slick (grease→fire on ignite)
createSurfaceArea(5, 5, 0, "fire");
check("igniting grease spreads fire across the slick", count("fire") === 9 && count("grease") === 0);

// 3) water on fire makes STEAM (the flame is doused into a cover cloud) — BG3 combo
paintSurface(5, 5, "water");
check("water on fire becomes steam", (surfaces.get("5,5") || {}).type === "steam" && count("fire") === 8);
// fire onto water also makes steam (clean tiles)
surfaces.clear();
paintSurface(3, 8, "water"); paintSurface(3, 8, "fire");
check("fire on water also becomes steam", (surfaces.get("3,8") || {}).type === "steam");

// 4) over a round, fire spreads into adjacent grease, then decays
surfaces.clear();
paintSurface(5, 5, "fire");      // fire next to fresh grease
paintSurface(6, 5, "grease");
surfaceTick();
check("fire spreads into adjacent grease each round", (surfaces.get("6,5") || {}).type === "fire");
check("a tick decrements surface lifetimes", (surfaces.get("5,5") || {}).rounds === 2);

// 5) surfaces expire when their lifetime runs out
surfaces.clear();
createSurfaceArea(5, 5, 0, "poison");   // poison lasts 4 rounds
for (let i = 0; i < 4; i++) surfaceTick();
check("surfaces decay away after their lifetime", surfaces.size === 0);

// 6) surfaces never spill out of bounds
surfaces.clear();
createSurfaceArea(0, 0, 1, "grease");   // a -1 ring would be off-grid
check("surfaces stay in bounds", surfaces.size === 4 && !surfaces.has("-1,0") && !surfaces.has("0,-1"));

// 7) water detection (drives the lightning chain)
surfaces.clear();
paintSurface(5, 5, "water"); createSurfaceArea(7, 7, 0, "grease");
check("isWaterAt detects water tiles only", isWaterAt(5, 5) === true && isWaterAt(7, 7) === false && isWaterAt(9, 9) === false);

// 7b) all five elemental surfaces paint correctly
surfaces.clear();
["fire", "grease", "water", "poison", "ice"].forEach((t, i) => createSurfaceArea(i * 2, 0, 0, t));
check("every elemental surface type can be laid",
  ["fire", "grease", "water", "poison", "ice"].every((t, i) => (surfaces.get(`${i * 2},0`) || {}).type === t));

// 8) the page wires surfaces + the lightning/water combo into play
check("page has surface-creating abilities for all five (oil/fire/water/poison/ice)",
  h.includes('createsSurface:"grease"') && h.includes('createsSurface:"fire"') && h.includes('createsSurface:"water"') &&
  h.includes('createsSurface:"poison"') && h.includes('createsSurface:"ice"'));
check("poison + ice apply their conditions on enter",
  h.includes('s.type==="poison"){u.add(PoisonCloud') && h.includes('s.type==="ice"){u.add(Slowed'));
check("steam (fire+water) grants cover on enter (+2 AC)",
  h.includes('armorClassModifier:2') && h.includes('s.type==="steam"){u.add(Obscured') &&
  h.includes('type:"steam"'));
check("page applies surfaces on enter + each round",
  h.includes("function enterSurface") && h.includes("surfaceTick()") && h.includes("createSurfaceArea"));
check("page renders surfaces", h.includes("function surfFill") && h.includes("for(const[k,s]of surfaces)"));
check("lightning chains through water (Storm Bolt combo)",
  h.includes("chainWater:true") && h.includes("function chainLightning") &&
  h.includes("isWaterAt(u.x,u.y)") && h.includes("if(ab.chainWater)chainLightning(def)"));

console.log(`\n  Environmental surfaces — combos + wiring:`);
console.log(`  ${pass} passed, ${fail} failed`);
if (fail) { fails.forEach(f => console.log("   ✗ " + f)); process.exit(1); }
else console.log(`  ✓ grease ignites, fire spreads, water douses, surfaces decay & stay in bounds; wired into the demo.\n`);
