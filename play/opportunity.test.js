// Objective check of opportunity attacks. We lift the page's real reach-test helper (the
// marked /*<OA>*/ block) and prove it fires only when a mover actually LEAVES a foe's reach,
// then assert the maneuver is wired (provoke on move, once-per-round, Disengage, turn reset).
const fs = require("fs");
let pass = 0, fail = 0; const fails = [];
function check(name, cond) { if (cond) pass++; else { fail++; fails.push(name); } }

const h = fs.readFileSync(__dirname + "/crown_combat.html", "utf8");
const block = h.match(/\/\*<OA>\*\/([\s\S]*?)\/\*<\/OA>\*\//);
check("OA helper block found in the page", !!block);
const { leavingReachOf } = new Function(block[1] + "\nreturn { leavingReachOf };")();

const foe = { x: 5, y: 5 };
// leaving the foe's reach (adjacent → not adjacent) provokes
check("stepping out of reach provokes", leavingReachOf(5, 6, 5, 8, foe) === true);
check("stepping out of reach (diagonal start) provokes", leavingReachOf(4, 4, 2, 2, foe) === true);
// staying in reach does NOT provoke
check("staying adjacent does not provoke", leavingReachOf(5, 6, 4, 5, foe) === false);
check("shuffling within reach does not provoke", leavingReachOf(4, 5, 6, 5, foe) === false);
// moving while never adjacent does NOT provoke
check("moving far from the foe never provokes", leavingReachOf(1, 1, 9, 9, foe) === false);
// approaching the foe does not provoke (was not adjacent at the start)
check("approaching does not provoke", leavingReachOf(8, 8, 6, 6, foe) === false);

// wired into the game
check("provokeOA respects Disengage + once-per-round",
  h.includes("function provokeOA") && h.includes("if(mover.disengaged)return") && h.includes("!e.reacted") && h.includes("e.reacted=true"));
check("movement provokes (player move + enemy step)",
  (h.match(/provokeOA\(u,fx0,fy0/g) || []).length >= 2);
check("reaction state resets each turn",
  h.includes("u.reacted=false;u.disengaged=false"));
check("Disengage action exists in the UI",
  h.includes('id="disengage"') && h.includes("u.disengaged=true"));

console.log(`\n  Opportunity attacks — reach test + wiring:`);
console.log(`  ${pass} passed, ${fail} failed`);
if (fail) { fails.forEach(f => console.log("   ✗ " + f)); process.exit(1); }
else console.log(`  ✓ fires only when leaving reach; Disengage avoids it, once per round, resets each turn.\n`);
