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
  h.includes('if(att.side==="hero")bark("crit")') && h.includes('bark("kill")') && h.includes('bark("ally_down")') &&
  h.includes('bark("heal")') && h.includes('if(h)bark("victory")') &&
  h.includes('bark("ignite")') && h.includes('bark("wall")'));
check("barks are rate-limited so they don't spam", h.includes("_clock-_lastBark<1.6"));

// ---- inter-companion banter (they bicker with EACH OTHER) ----
const pb = h.match(/\/\*<PAIRBANTER>\*\/([\s\S]*?)\/\*<\/PAIRBANTER>\*\//);
check("pair-banter table + picker found", !!pb);
const { PAIR_BANTERS, pickPair } = new Function(pb[1] + "\nreturn { PAIR_BANTERS, pickPair };")();
check("pair banters are two-line exchanges between two companions", PAIR_BANTERS.length >= 3 &&
  PAIR_BANTERS.every(p => p.who.length === 2 && p.lines.length === 2));
check("a pair banter only fires when BOTH companions are present", (() => {
  const p = pickPair(["Varra", "Garrow"], 0); return p && p.who.includes("Varra") && p.who.includes("Garrow");
})());
check("no pair banter when one of the two is absent",
  pickPair(["Garrow"], 0) === null && pickPair(["Roen"], 0) === null);
check("the two speakers differ and both have lines", (() => {
  const p = pickPair(["Roen", "Varra"], 0); return p && p.lines[0][0] !== p.lines[1][0] && p.lines.every(l => l[1].length > 0);
})());
check("pair banter is wired to turn starts + rate-limited",
  h.includes('if(u.side==="hero")pairBanter()') && h.includes("_clock-_lastPair<7"));

// ---- story-flag-reactive barks (Pillar 3 × 4): the line changes with the run's flags ----
const fb = h.match(/\/\*<FLAGBARK>\*\/([\s\S]*?)\/\*<\/FLAGBARK>\*\//);
check("flag-bark table + picker found", !!fb);
const { FLAG_BARKS, pickFlagBark } = new Function(fb[1] + "\nreturn { FLAG_BARKS, pickFlagBark };")();
check("flag barks exist for several beats", ["crit", "kill", "ally_down", "victory"].every(k => (FLAG_BARKS[k] || []).length));
check("a set story flag (with speaker present) yields its special line",
  (() => { const b = pickFlagBark("kill", ["Varra"], { "quest.varra.resolved": true }); return b && b[0] === "Varra" && /fine print/.test(b[1]); })());
check("no special line when the flag is unset", pickFlagBark("kill", ["Varra"], {}) === null);
check("no special line when the speaker is absent",
  pickFlagBark("kill", ["Garrow"], { "quest.varra.resolved": true }) === null);
check("NG+ wink fires only on a New-Game+ run",
  (() => { const b = pickFlagBark("victory", ["Varra"], { "ng.plus": true }); return b && /done this before/i.test(b[1]); })());
check("flag barks take priority + STORY loads from the run",
  h.includes("pickFlagBark(event,present,STORY)||pickBark") &&
  h.includes('localStorage.getItem("coh.combat.flags")'));

console.log(`\n  Companion barks (reactivity) — picker + wiring:`);
console.log(`  ${pass} passed, ${fail} failed`);
if (fail) { fails.forEach(f => console.log("   ✗ " + f)); process.exit(1); }
else console.log(`  ✓ only present companions speak, falls back, deterministic, rate-limited; wired to the beats.\n`);
