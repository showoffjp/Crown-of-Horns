// Objective checks for the Shield reaction (Pillar 1 — BG3's defensive reaction). We lift the
// page's real /*<SHIELD>*/ predicate and prove +5 AC negates exactly the hits it should — a
// blow that *just* lands, never a crit, never a comfortable hit, never a miss — then assert
// Shield is wired into applyOne as Naeve's once-per-round arcane reaction, sharing the reaction
// economy with opportunity attacks and gated by the Reactions toggle. The d20 is exercised by
// the smoke run (Naeve Shields across the 400-game auto-play).
const fs = require("fs");
let pass = 0, fail = 0; const fails = [];
function check(name, cond) { if (cond) pass++; else { fail++; fails.push(name); } }

const h = fs.readFileSync(__dirname + "/crown_combat.html", "utf8");
const block = h.match(/\/\*<SHIELD>\*\/([\s\S]*?)\/\*<\/SHIELD>\*\//);
check("SHIELD helper block found in the page", !!block);
const { shieldNegates } = new Function(block[1] + "\nreturn { shieldNegates };")();

// +5 AC turns aside a hit that landed by 0..4 over AC; nothing else
check("negates a hit that lands exactly on AC", shieldNegates(15, 15, false) === true);
check("negates a hit that beats AC by 4 (the +5 edge)", shieldNegates(19, 15, false) === true);
check("does NOT negate a hit that beats AC by 5 (now too strong)", shieldNegates(20, 15, false) === false);
check("does NOT negate a comfortable hit", shieldNegates(25, 15, false) === false);
check("does NOT 'negate' an attack that already missed", shieldNegates(14, 15, false) === false);
check("never negates a critical hit, even a near one", shieldNegates(15, 15, true) === false);

// the reaction is wired into applyOne as Naeve's once-per-round, toggle-gated arcane reaction
check("Naeve knows Shield (knowsShield flag)", /name:"Naeve"[\s\S]*?knowsShield:true/.test(h));
check("Shield only triggers against an attack roll on a hero who knows it",
  h.includes('ab.isAttackRoll&&def.knowsShield&&def.side==="hero"'));
check("Shield consumes the shared once-per-round reaction (def.reacted)",
  /shieldNegates\(r\.total,r\.ac,r\.crit\)\)\{\s*def\.reacted=true/.test(h));
check("Shield respects the Reactions toggle", /!def\.reacted&&reactionsOn&&r\.hit&&shieldNegates/.test(h));
check("Shield only fires when the attack actually landed (r.hit)", h.includes("&&r.hit&&shieldNegates(r.total,r.ac,r.crit)"));
check("a Shielded blow is fully turned aside (early return, no damage)",
  /spawnFloat\(def,"🛡 Shield!"[\s\S]*?sfx\("cond"\);return;\}/.test(h));
check("the log narrates the +5 AC that turned the blow aside", h.includes("+5 AC turns ${att.name}'s blow aside"));
check("the Reactions toggle now advertises Shield too", h.includes("opportunity attacks & Shield"));

console.log(`\n  Shield reaction — negation predicate + wiring:`);
for (const f of fails) console.log("  ✗ " + f);
console.log(`  ${pass} passed, ${fail} failed`);
process.exit(fail ? 1 : 0);
