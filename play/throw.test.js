// Objective checks for the Throw maneuver (Pillar 1, BG3 verticality/utility). We lift the
// page's real range predicate (the marked /*<THROW>*/ block) and prove a flask reaches any
// tile in range — empty ground included, but never your own feet — then assert Throw is
// wired: the ground-target ability, doThrow (burst + surface seed), the click path, the
// range highlight, and the action-bar label. The save/dice randomness is standard and
// exercised by the 400-game smoke run (which now throws every game).
const fs = require("fs");
let pass = 0, fail = 0; const fails = [];
function check(name, cond) { if (cond) pass++; else { fail++; fails.push(name); } }

const h = fs.readFileSync(__dirname + "/crown_combat.html", "utf8");
const block = h.match(/\/\*<THROW>\*\/([\s\S]*?)\/\*<\/THROW>\*\//);
check("THROW helper block found in the page", !!block);
const { inThrowRange } = new Function(block[1] + "\nreturn { inThrowRange };")();

// ---- range predicate: Chebyshev <= range, but not the thrower's own tile ----
check("reaches an empty tile within range", inThrowRange(5, 5, 8, 5, 5) === true);
check("reaches a diagonal tile within range", inThrowRange(5, 5, 8, 8, 5) === true);
check("can land exactly at the edge of range", inThrowRange(5, 5, 10, 5, 5) === true);
check("cannot throw beyond range", inThrowRange(5, 5, 11, 5, 5) === false && inThrowRange(5, 5, 11, 11, 5) === false);
check("cannot throw at your own feet", inThrowRange(5, 5, 5, 5, 5) === false);
check("range is Chebyshev (a knight-ish tile still counts as 3)", inThrowRange(5, 5, 8, 7, 3) === true && inThrowRange(5, 5, 8, 7, 2) === false);

// ---- the maneuver is wired into the game + UI ----
check("Roen carries a thrown Alchemist's Fire (range 5, seeds fire)",
  /name:"Alchemist's Fire",thrown:true,targeting:"throw",range:5[\s\S]*?createsSurface:"fire"/.test(h));
check("doThrow exists and lands on a tile", h.includes("function doThrow(att,tx,ty,ab)"));
check("doThrow bursts on every foe in the blast via burstHits",
  h.includes("burstHits(units,att.side,tx,ty,ab.surfaceRadius||1)"));
check("doThrow seeds its surface at the landing tile",
  h.includes("createSurfaceArea(tx,ty,ab.surfaceRadius||1,ab.createsSurface)"));
check("doThrow re-checks surfaces so anyone standing in the new fire is caught",
  /doThrow[\s\S]*?units\.filter\(u=>u\.alive\)\.forEach\(enterSurface\)/.test(h));
check("legalTargets treats throw as tile-targeted (no unit target)",
  h.includes('if(ab.targeting==="throw")return[];'));
check("the click path lands a throw on any passable tile in range",
  h.includes('armed.targeting==="throw"') && h.includes("inThrowRange(u.x,u.y,x,y,armed.range)") && h.includes("doThrow(u,x,y,armed)"));
check("the throw clears the armed flask after it lands", /doThrow\(u,x,y,armed\)\)\{armed=null/.test(h));
check("the board highlights the throw-range tiles", /armed\.targeting==="throw"[\s\S]*?inThrowRange\(sel\.x,sel\.y,gx,gy,armed\.range\)/.test(h));
check("the action bar labels a throw ability", h.includes('ab.targeting==="throw"?"throw "+ab.createsSurface'));
check("the hint tells you to click a ground tile", h.includes('armed.targeting==="throw"?"glowing ground tile to lob it"'));

console.log(`\n  Throw maneuver — ground-target range + wiring:`);
for (const f of fails) console.log("  ✗ " + f);
console.log(`  ${pass} passed, ${fail} failed`);
process.exit(fail ? 1 : 0);
