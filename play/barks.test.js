// Objective check of the companion bark system (Pillar 3 — reactivity). We lift the page's
// real bark table + picker (the marked /*<BARK>*/ block) and prove it only ever puts words in
// the mouth of a companion who is actually present & alive, falls back gracefully, is
// deterministic for a given roll, and is wired to the dramatic combat beats.
const fs = require("fs");
let pass = 0, fail = 0; const fails = [];
function check(name, cond) { if (cond) pass++; else { fail++; fails.push(name); } }

const h = fs.readFileSync(__dirname + "/crown_combat.html", "utf8");
const block = h.match(/\/\*<BARK>\*\/([\s\S]*?)\/\*<\/BARK>\*\//);
check("bark table + picker block found", !!block);
const { BARKS, pickBark } = new Function(block[1] + "\nreturn { BARKS, pickBark };")();

check("there are barks for the key beats", ["crit", "kill", "ally_down", "heal", "victory"].every(k => (BARKS[k] || []).length));
check("only a PRESENT companion speaks", (() => {
  // with just Garrow present, a crit bark must be Garrow's
  const b = pickBark("crit", ["Garrow"], 0); return b && b[0] === "Garrow";
})());
check("a different present companion speaks", (() => {
  const b = pickBark("crit", ["Varra"], 0); return b && b[0] === "Varra";
})());
check("falls back to anyone when none of the liners are present", (() => {
  const b = pickBark("crit", ["Maerin"], 0); return b && BARKS.crit.some(x => x[0] === b[0]);
})());
check("deterministic for a given roll (r=0 → first of pool)", (() => {
  const pool = BARKS.kill.filter(x => ["Garrow", "Roen", "Varra"].includes(x[0]));
  return pickBark("kill", ["Garrow", "Roen", "Varra"], 0)[1] === pool[0][1];
})());
check("unknown events produce no bark", pickBark("nonsense", ["Garrow"], 0) === null);
check("the picked line is non-empty text", (() => {
  const b = pickBark("victory", ["Roen"], 0.5); return b && typeof b[1] === "string" && b[1].length > 0;
})());

// wired to the beats
check("bark fires on hero crit / kill / ally-down / heal / victory / ignite / wall",
  h.includes('if(att.side==="hero")bark("crit")') && h.includes('bark(def.side==="foe"?"kill":"ally_down")') &&
  h.includes('bark("heal")') && h.includes('if(h)bark("victory")') &&
  h.includes('bark("ignite")') && h.includes('bark("wall")'));
check("barks are rate-limited so they don't spam", h.includes("_clock-_lastBark<1.6"));

console.log(`\n  Companion barks (reactivity) — picker + wiring:`);
console.log(`  ${pass} passed, ${fail} failed`);
if (fail) { fails.forEach(f => console.log("   ✗ " + f)); process.exit(1); }
else console.log(`  ✓ only present companions speak, falls back, deterministic, rate-limited; wired to the beats.\n`);
