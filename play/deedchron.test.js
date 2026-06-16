// Objective checks for the Combat Chronicle panel (Pillar 1 × 4 — the deed loop made visible). The
// Save Inspector stages flags INTO combat; this reads back the deeds combat WRITES (coh.combat.deeds),
// closing the loop in the narrative UI. We lift the page's real /*<DEEDCHRON>*/ deedChronicle mapping
// and prove it turns the stored flag map into a sorted, labelled list, then assert the panel is wired:
// it reads coh.combat.deeds, renders, and can be cleared.
const fs = require("fs");
let pass = 0, fail = 0; const fails = [];
function check(name, cond) { if (cond) pass++; else { fail++; fails.push(name); } }

const h = fs.readFileSync(__dirname + "/save_inspector.html", "utf8");
const block = h.match(/\/\*<DEEDCHRON>\*\/([\s\S]*?)\/\*<\/DEEDCHRON>\*\//);
check("DEEDCHRON helper block found in the Save Inspector", !!block);
const { deedChronicle, DEED_LABEL } = new Function(block[1] + "\nreturn { deedChronicle, DEED_LABEL };")();

check("no deeds map -> empty list", deedChronicle(null).length === 0 && deedChronicle({}).length === 0);

const sample = { "deed.combat.flawless": true, "deed.combat.counterspell": true };
const list = deedChronicle(sample);
check("each set flag becomes one entry", list.length === 2);
check("entries are sorted by flag key", list[0].flag <= list[1].flag);
check("every entry carries a human-readable label", list.every(d => d.label && d.label !== d.flag));
check("a flawless deed reads in plain language", deedChronicle({ "deed.combat.flawless": true })[0].label === "flawless (no hero fell)");
check("a counterspell deed reads in plain language", deedChronicle({ "deed.combat.counterspell": true })[0].label === "the boss's spell unwoven");

check("a false/cleared flag is not shown", deedChronicle({ "deed.combat.ruthless": false, "deed.combat.merciful": true }).length === 1);
check("an unknown deed flag still lists (labelled as its key)",
  deedChronicle({ "deed.combat.future": true })[0].label === "deed.combat.future");

// the Inspector's label table matches the six deeds the Combat tab can write
const combat = fs.readFileSync(__dirname + "/crown_combat.html", "utf8");
const cblock = combat.match(/\/\*<DEEDS>\*\/([\s\S]*?)\/\*<\/DEEDS>\*\//);
const combatDeeds = (cblock[1].match(/deed\.combat\.\w+/g) || []).filter((v, i, a) => a.indexOf(v) === i);
check("every deed the Combat tab can earn has a label in the Inspector",
  combatDeeds.every(f => DEED_LABEL[f]));

// wiring: the panel reads coh.combat.deeds, renders, and can be cleared
check("renderDeeds reads coh.combat.deeds from localStorage",
  /renderDeeds\(\)\{[\s\S]*?localStorage\.getItem\("coh\.combat\.deeds"\)/.test(h));
check("renderDeeds paints the deeds panel", h.includes('document.getElementById("deeds").innerHTML='));
check("there's a deeds panel + hint in the markup", h.includes('id="deeds"') && h.includes('id="deedsHint"'));
check("clearDeeds removes the stored Chronicle", /clearDeeds\(\)\{[\s\S]*?localStorage\.removeItem\("coh\.combat\.deeds"\)/.test(h));
check("the panel is rendered on boot", /loadModel\(JSON\.parse[\s\S]*?\);\s*renderDeeds\(\);/.test(h));

console.log(`\n  Combat Chronicle panel — deed read-back + wiring:`);
for (const f of fails) console.log("  ✗ " + f);
console.log(`  ${pass} passed, ${fail} failed`);
process.exit(fail ? 1 : 0);
