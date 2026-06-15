// Objective check of the Jump traversal (Pillar 1 — verticality). We lift the page's real
// jump-range predicate (the marked /*<JUMP>*/ block) and prove it leaps to any tile within
// range (vaulting whatever's between, since it doesn't pathfind) but never to the current
// tile or out of range — then assert the action is wired (no opportunity attack, button + J).
const fs = require("fs");
let pass = 0, fail = 0; const fails = [];
function check(name, cond) { if (cond) pass++; else { fail++; fails.push(name); } }

const h = fs.readFileSync(__dirname + "/crown_combat.html", "utf8");
const block = h.match(/\/\*<JUMP>\*\/([\s\S]*?)\/\*<\/JUMP>\*\//);
check("jump predicate block found", !!block);
const { inJumpRange } = new Function(block[1] + "\nreturn { inJumpRange };")();

const R = 3;
check("can jump to the edge of range (3 straight)", inJumpRange(5, 5, 8, 5, R) === true);
check("can jump diagonally within range", inJumpRange(5, 5, 7, 7, R) === true);
check("can jump over an adjacent obstacle to land 2 out", inJumpRange(5, 5, 5, 7, R) === true);
check("cannot jump beyond range", inJumpRange(5, 5, 9, 5, R) === false && inJumpRange(5, 5, 9, 9, R) === false);
check("cannot 'jump' to your own tile", inJumpRange(5, 5, 5, 5, R) === false);

// wired into the game
check("jump lands only on a clear, passable, in-range tile",
  h.includes("jumpMode&&!u.hasMoved&&!target&&!blocked.has(key(x,y))") &&
  h.includes("inJumpRange(u.x,u.y,x,y,JUMP_RANGE)"));
check("jump does NOT provoke an opportunity attack",
  /jumpMode[\s\S]{0,400}u\.hasMoved=true;jumpMode=false/.test(h) &&
  !/jumpMode[\s\S]{0,400}provokeOA/.test(h));
check("jump is wired to a button + J hotkey + range highlight",
  h.includes('id="jump"') && h.includes('j:"jump"') && h.includes("jumpMode=!jumpMode") &&
  h.includes("inJumpRange(sel.x,sel.y,gx,gy,JUMP_RANGE)"));
check("jump state resets each turn", h.includes("jumpMode=false;") &&
  /armed=u\.side==="hero"\?u\.kit\[0\]:null;jumpMode=false/.test(h));

console.log(`\n  Jump (verticality) — range predicate + wiring:`);
console.log(`  ${pass} passed, ${fail} failed`);
if (fail) { fails.forEach(f => console.log("   ✗ " + f)); process.exit(1); }
else console.log(`  ✓ leaps within range (vaulting obstacles), never onto self/out of range; no OA; wired & gated.\n`);
