// Objective check of the Undo Move QoL action. We lift the page's real "can this move be
// undone?" predicate (the marked /*<UNDO>*/ block) — a move is undoable only if it triggered
// nothing irreversible (no surface effect, no opportunity attack, and the mover survived) —
// then assert the action is wired (snapshot on a clean move, dirtied by surfaces/OAs, restore,
// and cleared once you act or the turn ends).
const fs = require("fs");
let pass = 0, fail = 0; const fails = [];
function check(name, cond) { if (cond) pass++; else { fail++; fails.push(name); } }

const h = fs.readFileSync(__dirname + "/crown_combat.html", "utf8");
const block = h.match(/\/\*<UNDO>\*\/([\s\S]*?)\/\*<\/UNDO>\*\//);
check("undo predicate block found in the page", !!block);
const { canUndoMove } = new Function(block[1] + "\nreturn { canUndoMove };")();

check("a clean move (survived, nothing triggered) is undoable", canUndoMove(false, true) === true);
check("a move that triggered something is NOT undoable", canUndoMove(true, true) === false);
check("a move that killed the mover is NOT undoable", canUndoMove(false, false) === false);
check("dirty + dead is NOT undoable", canUndoMove(true, false) === false);

// wired into the game
check("the move snapshots an undo point via the predicate",
  h.includes("_undo=canUndoMove(_moveDirty,u.alive)") && h.includes("_moveDirty=false;u.x=x;u.y=y"));
check("surfaces and opportunity attacks mark the move dirty",
  (h.match(/_moveDirty=true/g) || []).length >= 3);
check("Undo Move action restores the prior tile + refunds the move",
  h.includes('id="undo"') && h.includes("u.x=_undo.x;u.y=_undo.y;u.hasMoved=false") &&
  h.includes("reachable=computeReachable(u)"));
check("undo is cleared once you act, disengage, or end the turn",
  (h.match(/_undo=null/g) || []).length >= 4);
check("Undo button is gated to a fresh, un-acted move",
  h.includes('ub.disabled=!_undo||!u||u.side!=="hero"||u.hasActed||!u.hasMoved'));

// ---- keyboard hotkeys ----
const hk = h.match(/\/\*<HOTKEY>\*\/([\s\S]*?)\/\*<\/HOTKEY>\*\//);
check("hotkey map block found", !!hk);
const { hotkey } = new Function(hk[1] + "\nreturn { hotkey };")();
check("number keys arm the matching ability", hotkey("1") === "ability0" && hotkey("5") === "ability4");
check("action hotkeys map correctly", hotkey("v") === "shove" && hotkey("x") === "disengage" &&
  hotkey("u") === "undo" && hotkey("Enter") === "endturn" && hotkey("f") === "forecast" && hotkey("r") === "react");
check("unmapped keys do nothing", hotkey("z") === null && hotkey("0") === null && hotkey("") === null);
check("keydown listener is wired (headless-safe)",
  h.includes('if(document.addEventListener)document.addEventListener("keydown"') && h.includes("hotkey(e.key)"));
check("ability buttons show their hotkey number", h.includes('<span style="color:#c9a24b">${i+1}</span>'));

// ---- reaction toggle (BG3 "ask before my opportunity attacks") ----
check("a Reactions toggle exists and gates hero OAs",
  h.includes('id="react"') && h.includes("reactionsOn=!reactionsOn") &&
  h.includes('e.side==="hero"&&!reactionsOn'));

console.log(`\n  Undo Move (QoL) — predicate + wiring:`);
console.log(`  ${pass} passed, ${fail} failed`);
if (fail) { fails.forEach(f => console.log("   ✗ " + f)); process.exit(1); }
else console.log(`  ✓ a clean move can be taken back; a move that triggered a reaction/hazard cannot. Wired & gated.\n`);
