// Objective checks for "shove off a ledge" (Pillar 1 — BG3's signature environmental kill).
// We lift the page's real height map + the marked /*<LEDGE>*/ fall helpers and prove a shove
// from high ground to low registers a fall (and a flat shove on level ground does not), then
// assert doShove deals scaling fall damage and knocks the foe Prone. The d20 contest and 2d6
// roll are standard and exercised by the smoke run.
const fs = require("fs");
let pass = 0, fail = 0; const fails = [];
function check(name, cond) { if (cond) pass++; else { fail++; fails.push(name); } }

const h = fs.readFileSync(__dirname + "/crown_combat.html", "utf8");
const high = h.match(/const HIGH=new Set\((\[[\s\S]*?\])\);/);
const heightBlk = h.match(/\/\*<HEIGHT>\*\/([\s\S]*?)\/\*<\/HEIGHT>\*\//);
const ledgeBlk = h.match(/\/\*<LEDGE>\*\/([\s\S]*?)\/\*<\/LEDGE>\*\//);
check("LEDGE helper block found in the page", !!ledgeBlk);
const src = `const HIGH=new Set(${high[1]});const key=(x,y)=>x+","+y;` + heightBlk[1] + ledgeBlk[1] +
  "\nreturn { fallDrop, fallDamageAvg, elevOf };";
const { fallDrop, fallDamageAvg, elevOf } = new Function(src)();

// the height map: (8,4)(9,4)(8,5)(9,5) is the raised mound
check("the mound tiles read as high ground", elevOf(8, 4) === 1 && elevOf(9, 5) === 1);
check("the surrounding floor reads as low", elevOf(7, 4) === 0 && elevOf(10, 5) === 0);

// fallDrop — elevation lost when shoved from (fx,fy) to (tx,ty)
check("shoving from high ground down to low is a fall (drop 1)", fallDrop(8, 4, 7, 4) === 1);
check("a shove that stays on level low ground is no fall", fallDrop(7, 4, 6, 4) === 0);
check("a shove that stays on the mound is no fall", fallDrop(8, 4, 9, 4) === 0);
check("being shoved UP onto the ledge is never a negative fall", fallDrop(7, 4, 8, 4) === 0);
check("fall damage scales with the drop (2d6 avg per level)", fallDamageAvg(1) === 7 && fallDamageAvg(0) === 0 && fallDamageAvg(2) === 14);

// the maneuver is wired into doShove + the Prone condition exists
check("Prone is a real condition that grants attackers advantage",
  /const Prone=\{[\s\S]*?attackersHaveAdvantage:true/.test(h));
check("doShove measures the drop before moving the foe",
  h.includes("const drop=fallDrop(def.x,def.y,dest.x,dest.y)"));
check("a fall deals 2d6 per level of drop",
  /if\(drop>0\)\{const fd=Dice\.Notation\("2d6"\)\*drop/.test(h));
check("a fall knocks the foe Prone", /if\(drop>0\)[\s\S]*?def\.add\(Prone,Prone\.durationRounds\)/.test(h));
check("a fall reads as a plummet in the log (not just a shove)", h.includes("off the ledge — they plummet for"));
check("a level shove still just pushes them a pace", h.includes("shoves ${def.name} back a pace"));

console.log(`\n  Shove off a ledge — fall detection + wiring:`);
for (const f of fails) console.log("  ✗ " + f);
console.log(`  ${pass} passed, ${fail} failed`);
process.exit(fail ? 1 : 0);
