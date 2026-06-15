// Objective checks for the Counterspell reaction (Pillar 1 — BG3's offensive reaction). We lift
// the page's real /*<COUNTER>*/ predicate and prove Naeve can unravel exactly an enemy spell,
// only while she knows it, holds her once-per-battle charge, and still has her reaction — then
// assert it's wired into useAbility (intercepting a foe's spell before it resolves, consuming both
// the round-reaction and the charge) and that the boss's Wail is flagged as a counterable spell.
const fs = require("fs");
let pass = 0, fail = 0; const fails = [];
function check(name, cond) { if (cond) pass++; else { fail++; fails.push(name); } }

const h = fs.readFileSync(__dirname + "/crown_combat.html", "utf8");
const block = h.match(/\/\*<COUNTER>\*\/([\s\S]*?)\/\*<\/COUNTER>\*\//);
check("COUNTER helper block found in the page", !!block);
const { canCounter } = new Function(block[1] + "\nreturn { canCounter };")();

const boss = { side: "foe" };
const spell = { isSpell: true, name: "Wail" };
const naeve = () => ({ alive: true, side: "hero", knowsCounter: true, counterReady: true, reacted: false });

check("counters an enemy spell when Naeve is ready", canCounter(boss, spell, naeve()) === true);
check("does NOT counter a non-spell (a plain attack)", canCounter(boss, { isSpell: false }, naeve()) === false);
check("does NOT counter a friendly cast (only enemy spells)", canCounter({ side: "hero" }, spell, naeve()) === false);
check("cannot counter once the per-battle charge is spent", canCounter(boss, spell, { ...naeve(), counterReady: false }) === false);
check("cannot counter if the reaction is already used this round", canCounter(boss, spell, { ...naeve(), reacted: true }) === false);
check("cannot counter if Naeve is down", canCounter(boss, spell, { ...naeve(), alive: false }) === false);
check("a hero who doesn't know Counterspell can't", canCounter(boss, spell, { ...naeve(), knowsCounter: false }) === false);

// the boss's Wail is a counterable spell, and the reaction is wired into useAbility
check("the boss's Wail of the Returned is flagged isSpell", /name:"Wail of the Returned",isSpell:true/.test(h));
check("Naeve knows Counterspell and holds one battle charge",
  /name:"Naeve"[\s\S]*?knowsCounter:true,counterReady:true/.test(h));
check("useAbility intercepts a foe's spell before it resolves",
  /if\(ab\.isSpell&&att\.side==="foe"&&reactionsOn\)\{/.test(h));
check("a successful counter consumes both the reaction and the battle charge",
  /nv\.reacted=true;nv\.counterReady=false/.test(h));
check("a countered spell fizzles entirely (returns before resolving)",
  /the spell fizzles[\s\S]*?att\.hasActed=true;return true;/.test(h));
check("Counterspell respects the Reactions toggle (reactionsOn)", h.includes('att.side==="foe"&&reactionsOn'));
check("the Reactions toggle now advertises Counterspell", h.includes("Shield & Counterspell"));

console.log(`\n  Counterspell reaction — counter predicate + wiring:`);
for (const f of fails) console.log("  ✗ " + f);
console.log(`  ${pass} passed, ${fail} failed`);
process.exit(fail ? 1 : 0);
