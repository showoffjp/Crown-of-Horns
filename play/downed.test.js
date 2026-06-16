// Objective checks for Downed + death saves + Help/revive (Pillar 1 — BG3's signature). A hero at 0 HP
// goes down instead of dying and rolls a death save each turn; an adjacent ally can spend their action
// to revive them. We lift the page's real /*<DOWNED>*/ deathSaveStep and prove every transition
// (success/failure/crit-revive/nat-1/stabilise/die), then assert the wiring: heroes go down (not die),
// downed bodies still occupy + render + take their death-save turn, and the Help action revives.
const fs = require("fs");
let pass = 0, fail = 0; const fails = [];
function check(name, cond) { if (cond) pass++; else { fail++; fails.push(name); } }

const h = fs.readFileSync(__dirname + "/crown_combat.html", "utf8");
const block = h.match(/\/\*<DOWNED>\*\/([\s\S]*?)\/\*<\/DOWNED>\*\//);
check("DOWNED helper block found in the page", !!block);
const { deathSaveStep } = new Function(block[1] + "\nreturn { deathSaveStep };")();
const z = { s: 0, f: 0 };

// per-roll transitions
check("10+ is a success", deathSaveStep(z, 10).s === 1 && deathSaveStep(z, 17).s === 1);
check("2..9 is a failure", deathSaveStep(z, 9).f === 1 && deathSaveStep(z, 2).f === 1);
check("a natural 1 is two failures", deathSaveStep(z, 1).f === 2);
check("a natural 20 revives (and clears the tally)", (() => { const r = deathSaveStep({ s: 1, f: 2 }, 20); return r.revived && r.s === 0 && r.f === 0; })());
check("a success does not (yet) flag dead/stable/revived", (() => { const r = deathSaveStep(z, 12); return !r.dead && !r.stable && !r.revived; })());

// the thresholds
check("the third success stabilises", deathSaveStep({ s: 2, f: 0 }, 15).stable === true);
check("the third failure is death", deathSaveStep({ s: 0, f: 2 }, 4).dead === true);
check("a nat-1 from one failure reaches three -> death", deathSaveStep({ s: 0, f: 1 }, 1).dead === true);
check("stabilising is not dying", deathSaveStep({ s: 2, f: 0 }, 15).dead === false);
check("the tally is clamped at the threshold", (() => { const r = deathSaveStep({ s: 0, f: 2 }, 5); return r.f === 3; })());
check("missing tally is treated as zero", deathSaveStep(null, 13).s === 1);

// wiring — heroes go DOWN, not dead; the turn loop rolls saves; Help revives
check("a hero at 0 HP goes down instead of dying", h.includes('if(def.side==="hero"){ goDown(def); }'));
check("goDown sets the dying state and zeroes HP", /function goDown\(u\)\{[\s\S]*?u\.downed=true;[\s\S]*?u\.hp=0/.test(h));
check("a downed hero's turn is an automatic death save", h.includes("if(u.downed){") && h.includes("deathSaveStep(u.ds,Dice.D20())"));
check("the turn loop stops on a dying hero (not just the conscious)",
  h.includes("order[turnIdx].downed&&!order[turnIdx].stable&&!order[turnIdx].dead"));
check("downed bodies still occupy their tile", h.includes("(u.alive||u.downed)&&u.x===x&&u.y===y"));
check("downed heroes are still drawn", h.includes("units.filter(u=>u.alive||u.downed).forEach(u=>ents.push"));
check("reviveAlly brings a fallen ally up at 1 HP", /function reviveAlly\(rescuer,ally\)\{[\s\S]*?ally\.downed=false;[\s\S]*?ally\.hp=1/.test(h));
check("there is a Help action wired to revive an adjacent downed ally",
  h.includes('id="revive"') && /helpMode&&!u\.hasActed&&target&&target\.side==="hero"&&target\.downed[\s\S]*?reviveAlly\(u,target\)/.test(h));
check("the party loses only when no hero is conscious (downed don't count as alive)",
  h.includes('get alive(){return this.hp>0;}'));

console.log(`\n  Downed + death saves + Help — transitions + wiring:`);
for (const f of fails) console.log("  ✗ " + f);
console.log(`  ${pass} passed, ${fail} failed`);
process.exit(fail ? 1 : 0);
