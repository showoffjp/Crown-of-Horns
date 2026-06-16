// Objective checks for deeds reaching the ending (Pillar 1 × 4 — the deed loop's final mile). The
// combat Chronicle writes deed.combat.* flags; the Save Inspector reads them; now the Endings
// Explorer folds them into the flags and lets them speak in the Chronicle summary and the epilogue.
// We lift the page's real /*<DEEDEPI>*/ helpers and prove the conduct line + epilogue slide answer to
// the deed flags (merciful/ruthless priority, the flavour deeds, nothing when unfought), then assert
// the wiring: deeds are folded in at boot and both surfaces consume the helpers.
const fs = require("fs");
let pass = 0, fail = 0; const fails = [];
function check(name, cond) { if (cond) pass++; else { fail++; fails.push(name); } }

const h = fs.readFileSync(__dirname + "/endings_explorer.html", "utf8");
const block = h.match(/\/\*<DEEDEPI>\*\/([\s\S]*?)\/\*<\/DEEDEPI>\*\//);
check("DEEDEPI helper block found in the Endings Explorer", !!block);
const { deedConductLine, deedEpilogueSlide } = new Function(block[1] + "\nreturn { deedConductLine, deedEpilogueSlide };")();
const flags = (...on) => ({ GetBool: k => on.indexOf(k) >= 0 });

// the Chronicle conduct line
check("no combat deeds -> no conduct line", deedConductLine(flags()) === null);
check("a merciful run reads as merciful conduct", /merciful/.test(deedConductLine(flags("deed.combat.merciful"))));
check("a ruthless run reads as ruthless conduct", /ruthless/.test(deedConductLine(flags("deed.combat.ruthless"))));
check("merciful outranks ruthless if both somehow set", /merciful/.test(deedConductLine(flags("deed.combat.merciful", "deed.combat.ruthless"))));

// the epilogue slide
check("no deeds -> no epilogue slide", deedEpilogueSlide(flags()) === null);
check("merciful earns the mercy epilogue", /spared even the dead/.test(deedEpilogueSlide(flags("deed.combat.merciful"))));
check("ruthless earns the warden epilogue", /new warden/.test(deedEpilogueSlide(flags("deed.combat.ruthless"))));
check("a counterspell earns Naeve's epilogue when that's the standout deed",
  /unwove the crown's own death-song/.test(deedEpilogueSlide(flags("deed.combat.counterspell"))));
check("a ledge kill earns Roen's cairn epilogue",
  /cairn no one admits to building/.test(deedEpilogueSlide(flags("deed.combat.ledge_kill"))));
check("conduct (merciful/ruthless) outranks the flavour deeds in the epilogue",
  /spared even the dead/.test(deedEpilogueSlide(flags("deed.combat.merciful", "deed.combat.counterspell", "deed.combat.ledge_kill"))));

// wiring: deeds folded in at boot, consumed by Chronicle + Epilogue
check("boot reads coh.combat.deeds into deedPairs",
  /localStorage\.getItem\("coh\.combat\.deeds"\)[\s\S]*?deedPairs\.push/.test(h));
check("deeds are merged onto staged save flags", h.includes("setAll(pairs.concat(deedPairs))"));
check("deeds still apply when there is no staged save", /if\(deedPairs\.length\) setAll\(deedPairs\)/.test(h));
check("the Chronicle summary appends the conduct line", h.includes("const conduct = deedConductLine(f);") && h.includes("if (conduct) lines.push(conduct);"));
check("the epilogue appends the deed slide before filtering", /slides\.push\(deedEpilogueSlide\(f\)\);\s*\n\s*return slides\.filter/.test(h));
check("a banner announces the folded-in deeds", h.includes("Folded in ") && h.includes("how you fought"));

console.log(`\n  Deeds reaching the ending — conduct/epilogue + wiring:`);
for (const f of fails) console.log("  ✗ " + f);
console.log(`  ${pass} passed, ${fail} failed`);
process.exit(fail ? 1 : 0);
