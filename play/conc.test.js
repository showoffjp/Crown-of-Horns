// Objective checks for Concentration (Pillar 1 — 5e/BG3). A concentration spell (Naeve's Mirror Image)
// holds only while its caster keeps focus: take damage and you roll a Constitution save or it drops.
// We lift the page's real /*<CONC>*/ helpers and prove the DC rule (10, or half the damage, whichever
// is higher) and the hold/break comparison, then assert the wiring: Mirror Image is a concentration
// effect, damage triggers the save, and a failed save strips the effect.
const fs = require("fs");
let pass = 0, fail = 0; const fails = [];
function check(name, cond) { if (cond) pass++; else { fail++; fails.push(name); } }

const h = fs.readFileSync(__dirname + "/crown_combat.html", "utf8");
const block = h.match(/\/\*<CONC>\*\/([\s\S]*?)\/\*<\/CONC>\*\//);
check("CONC helper block found in the page", !!block);
const { concentrationDC, concentrationHolds } = new Function(block[1] + "\nreturn { concentrationDC, concentrationHolds };")();

// the DC is max(10, floor(dmg/2))
check("small hits set the floor DC of 10", concentrationDC(1) === 10 && concentrationDC(19) === 10);
check("exactly 20 damage is still DC 10 (half = 10)", concentrationDC(20) === 10);
check("21 damage rounds down to DC 10", concentrationDC(21) === 10);
check("22 damage is DC 11", concentrationDC(22) === 11);
check("a big hit scales the DC by half its damage", concentrationDC(40) === 20 && concentrationDC(7) === 10);

// the hold/break comparison
check("a save total meeting the DC holds", concentrationHolds(15, 15) === true);
check("a save total above the DC holds", concentrationHolds(22, 11) === true);
check("a save total below the DC breaks", concentrationHolds(9, 10) === false);

// wiring
check("Mirror Image is flagged as a concentration effect",
  /const MirrorImage=\{[\s\S]*?concentration:true/.test(h));
check("taking damage triggers a concentration check", h.includes("if(r.dmg>0)checkConcentration(def,r.dmg)"));
check("checkConcentration finds the concentration effect and rolls a CON save",
  h.includes("def.activeEffects.find(x=>x.def.concentration)") && h.includes('Dice.D20()+def.Mod("con")'));
check("a failed save strips the concentration effect",
  /concentration breaks[\s\S]*?def\.activeEffects=def\.activeEffects\.filter\(x=>x!==e\)/.test(h) ||
  /def\.activeEffects=def\.activeEffects\.filter\(x=>x!==e\)[\s\S]*?concentration breaks/.test(h));
check("the DC scales with the damage taken (concentrationDC(dmg))",
  h.includes("const dc=concentrationDC(dmg)"));
check("no concentration roll without damage or while down",
  /function checkConcentration\(def,dmg\)\{\s*if\(dmg<=0\|\|!def\.alive\)return;/.test(h));

console.log(`\n  Concentration — DC rule + save/break wiring:`);
for (const f of fails) console.log("  ✗ " + f);
console.log(`  ${pass} passed, ${fail} failed`);
process.exit(fail ? 1 : 0);
