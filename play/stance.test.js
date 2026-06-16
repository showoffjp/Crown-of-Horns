// Objective checks for per-reaction Stances (Pillar 2 QoL — BG3-style granular reaction control). We
// lift the page's real /*<STANCE>*/ gate and prove a reaction fires only when the master toggle AND
// the reaction's own stance are on, then assert the three independent stances are wired into the OA,
// Shield, and Counterspell gates plus the toolbar toggles.
const fs = require("fs");
let pass = 0, fail = 0; const fails = [];
function check(name, cond) { if (cond) pass++; else { fail++; fails.push(name); } }

const h = fs.readFileSync(__dirname + "/crown_combat.html", "utf8");
const block = h.match(/\/\*<STANCE>\*\/([\s\S]*?)\/\*<\/STANCE>\*\//);
check("STANCE helper block found in the page", !!block);
const { reactionAllowed } = new Function(block[1] + "\nreturn { reactionAllowed };")();
const ON = { oa: true, shield: true, counter: true };

// master AND stance must both be on
check("master on + stance on -> reaction allowed", reactionAllowed(true, ON, "shield") === true);
check("master OFF kills the reaction even with stance on", reactionAllowed(false, ON, "shield") === false);
check("master on but stance OFF -> held", reactionAllowed(true, { ...ON, shield: false }, "shield") === false);
check("each kind is independent (holding OA leaves Shield armed)",
  reactionAllowed(true, { oa: false, shield: true, counter: true }, "oa") === false &&
  reactionAllowed(true, { oa: false, shield: true, counter: true }, "shield") === true);
check("an unknown stance kind is not allowed", reactionAllowed(true, ON, "mystery") === false);
check("a missing stances object is safe", reactionAllowed(true, null, "oa") === false);

// default stances are all on (no behaviour change out of the box)
check("stances default to all-on", /let stances=\{oa:true,shield:true,counter:true\}/.test(h));

// the three gates each consult the matching stance through the master
check("hero opportunity attacks gate on the oa stance",
  h.includes('!(e.side==="hero"&&!reactionAllowed(reactionsOn,stances,"oa"))'));
check("Shield gates on the shield stance",
  h.includes('!def.reacted&&reactionAllowed(reactionsOn,stances,"shield")&&r.hit'));
check("Counterspell gates on the counter stance",
  h.includes('att.side==="foe"&&reactionAllowed(reactionsOn,stances,"counter")'));

// the toolbar exposes the three independent stance toggles
check("there are OA / Shield / Counter stance buttons",
  h.includes('id="stOA"') && h.includes('id="stShield"') && h.includes('id="stCounter"'));
check("each stance button flips exactly its own stance", h.includes('toggleStance("oa")') &&
  h.includes('toggleStance("shield")') && h.includes('toggleStance("counter")'));
check("toggleStance flips the stance and reports it", /function toggleStance\(kind\)\{stances\[kind\]=!stances\[kind\]/.test(h));
check("the stance buttons re-render their on/off labels", h.includes("function renderStances()") && h.includes('"⚔ OA: "+(stances.oa?"on":"off")'));
check("the master toggle still works and notes stances apply",
  h.includes("reactionsOn=!reactionsOn") && h.includes("per-reaction stances still apply"));

console.log(`\n  Reaction stances — master×stance gate + wiring:`);
for (const f of fails) console.log("  ✗ " + f);
console.log(`  ${pass} passed, ${fail} failed`);
process.exit(fail ? 1 : 0);
