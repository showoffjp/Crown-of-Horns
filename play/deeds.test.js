// Objective checks for Deeds in Combat (Pillar 1 × 4 — how you win feeds the story). We lift the
// page's real /*<DEEDS>*/ deedFlags mapping and prove a battle summary earns exactly the right
// story flags (flawless vs hard-won, merciful vs ruthless, the ledge kill, the counterspell, and
// nothing at all on a loss), then assert the wiring: the deeds are tracked (ledge kill, countered
// boss), summarised, and written back to localStorage as coh.combat.deeds — the reverse of the
// flag bridge the combat already reads from.
const fs = require("fs");
let pass = 0, fail = 0; const fails = [];
function check(name, cond) { if (cond) pass++; else { fail++; fails.push(name); } }

const h = fs.readFileSync(__dirname + "/crown_combat.html", "utf8");
const block = h.match(/\/\*<DEEDS>\*\/([\s\S]*?)\/\*<\/DEEDS>\*\//);
check("DEEDS helper block found in the page", !!block);
const { deedFlags, DEED_LABEL } = new Function(block[1] + "\nreturn { deedFlags, DEED_LABEL };")();
const has = (s, f) => deedFlags(s).indexOf(f) >= 0;

const base = { won: true, heroesLost: 0, spared: 0, slain: 6, ledgeKill: false, counteredBoss: false };

// a loss earns nothing
check("a defeat is remembered as no deed at all", deedFlags({ ...base, won: false }).length === 0);

// flawless vs hard-won are mutually exclusive and always exactly one is present
check("no hero lost -> flawless", has(base, "deed.combat.flawless") && !has(base, "deed.combat.pyrrhic"));
check("a hero lost -> hard-won (pyrrhic), not flawless",
  has({ ...base, heroesLost: 1 }, "deed.combat.pyrrhic") && !has({ ...base, heroesLost: 1 }, "deed.combat.flawless"));

// merciful vs ruthless
check("spared all, slew none -> merciful", has({ ...base, slain: 0, spared: 6 }, "deed.combat.merciful"));
check("slew all, spared none -> ruthless", has(base, "deed.combat.ruthless"));
check("a mixed finish is neither merciful nor ruthless",
  !has({ ...base, slain: 3, spared: 3 }, "deed.combat.merciful") && !has({ ...base, slain: 3, spared: 3 }, "deed.combat.ruthless"));

// the special deeds
check("a ledge kill is recorded", has({ ...base, ledgeKill: true }, "deed.combat.ledge_kill"));
check("a countered boss spell is recorded", has({ ...base, counteredBoss: true }, "deed.combat.counterspell"));
check("reviving a fallen comrade is recorded", has({ ...base, rescued: true }, "deed.combat.rescue"));
check("no rescue deed if no one was brought back", !has(base, "deed.combat.rescue"));
check("a clean sweep can stack several deeds at once",
  deedFlags({ won: true, heroesLost: 0, spared: 0, slain: 6, ledgeKill: true, counteredBoss: true, rescued: true }).length === 5);
check("every emitted flag has a human-readable Chronicle label",
  deedFlags({ won: true, heroesLost: 0, spared: 6, slain: 0, ledgeKill: true, counteredBoss: true, rescued: true }).every(f => DEED_LABEL[f]));

// wiring: the deeds are tracked, summarised, and persisted (the reverse of coh.combat.flags)
check("a ledge fall that kills a foe sets the ledge-kill deed",
  h.includes('if(!def.alive&&def.side==="foe")_ledgeKill=true'));
check("a successful counterspell sets the counter deed", /nv\.counterReady=false;_counteredBoss=true/.test(h));
check("battleSummary counts heroes lost, foes spared, and foes slain",
  h.includes("heroesLost:heroes.filter(u=>!u.alive).length") && h.includes('slain:units.filter(u=>u.side==="foe"&&!u.alive&&!u.spared).length'));
check("victory records the deeds", /if\(h\)bark\("victory"\);\s*recordDeeds\(h\)/.test(h));
check("deeds are written back to localStorage as coh.combat.deeds",
  h.includes('localStorage.setItem("coh.combat.deeds"'));

console.log(`\n  Deeds in combat — outcome→flag mapping + wiring:`);
for (const f of fails) console.log("  ✗ " + f);
console.log(`  ${pass} passed, ${fail} failed`);
process.exit(fail ? 1 : 0);
