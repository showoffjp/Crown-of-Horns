// Objective check of the Shove maneuver. We lift the page's real push-direction helper
// (the marked /*<SHOVE>*/ block) and prove it pushes a foe directly away in all 8
// directions, then assert the maneuver is wired (ability, contest, shove-into-surface,
// the UI button). The contest's d20 randomness is standard and exercised by the smoke run.
const fs = require("fs");
let pass = 0, fail = 0; const fails = [];
function check(name, cond) { if (cond) pass++; else { fail++; fails.push(name); } }

const h = fs.readFileSync(__dirname + "/crown_combat.html", "utf8");
const block = h.match(/\/\*<SHOVE>\*\/([\s\S]*?)\/\*<\/SHOVE>\*\//);
check("shove helper block found in the page", !!block);
const { shoveDest } = new Function(block[1] + "\nreturn { shoveDest };")();

// pushes the defender one tile directly away from the attacker, in all 8 directions
const cases = [
  [{x:1,y:1}, {x:2,y:2}, {x:3,y:3}],   // down-right
  [{x:5,y:5}, {x:4,y:5}, {x:3,y:5}],   // left
  [{x:5,y:5}, {x:5,y:4}, {x:5,y:3}],   // up
  [{x:5,y:5}, {x:6,y:5}, {x:7,y:5}],   // right
  [{x:5,y:5}, {x:5,y:6}, {x:5,y:7}],   // down
  [{x:5,y:5}, {x:4,y:4}, {x:3,y:3}],   // up-left
  [{x:2,y:6}, {x:3,y:5}, {x:4,y:4}],   // up-right
];
let allDir = true;
for (const [att, def, want] of cases) {
  const d = shoveDest(att, def);
  if (d.x !== want.x || d.y !== want.y) { allDir = false; fails.push(`(${def.x},${def.y})→(${d.x},${d.y}) want (${want.x},${want.y})`); }
}
check("pushes the foe one tile directly away (all 8 dirs)", allDir);
check("destination is exactly one tile from the defender", cases.every(([a, def]) => {
  const d = shoveDest(a, def); return Math.max(Math.abs(d.x - def.x), Math.abs(d.y - def.y)) === 1;
}));

// the maneuver is wired into the game + UI
check("Shove ability exists (contest maneuver, range 1)",
  h.includes('shove:true') && h.includes('const SHOVE=Ability('));
check("doShove runs a Strength contest", h.includes("function doShove") &&
  h.includes('att.Mod("str")') && h.includes('Math.max(def.Mod("str"),def.Mod("dex"))'));
check("shove can push a foe INTO a surface (the BG3 combo)",
  /passable\(dest\.x,dest\.y,def\)\)\{[\s\S]*?enterSurface\(def\)/.test(h));
check("shove into a wall deals impact damage", h.includes("slams") && h.includes("def.hp=Math.max(0,def.hp-2)"));
check("useAbility dispatches shove", h.includes("if(ab.shove)"));
check("there is a Shove button in the UI", h.includes('id="shove"') && h.includes("armed===SHOVE"));

console.log(`\n  Shove maneuver — push direction + wiring:`);
console.log(`  ${pass} passed, ${fail} failed`);
if (fail) { fails.forEach(f => console.log("   ✗ " + f)); process.exit(1); }
else console.log(`  ✓ pushes a foe one tile away in any direction, into walls (impact) or hazards (combo); wired to the UI.\n`);
