// Objective checks for the "Baldur's Gate 2" arcanist release. We lift the page's real
// burst-targeting and auto-hit helpers (the marked /*<BG2>*/ block) and prove a Fireball
// hits every foe in its blast radius and that Magic Missile's never-miss damage is exact,
// then assert Naeve and the three Infinity-Engine spells (Magic Missile / Fireball /
// Mirror Image) are wired into the roster, targeting, resolve, and the UI. The d20/save
// randomness is standard and exercised by the 400-game smoke run.
const fs = require("fs");
let pass = 0, fail = 0; const fails = [];
function check(name, cond) { if (cond) pass++; else { fail++; fails.push(name); } }

const h = fs.readFileSync(__dirname + "/crown_combat.html", "utf8");
const block = h.match(/\/\*<BG2>\*\/([\s\S]*?)\/\*<\/BG2>\*\//);
check("BG2 helper block found in the page", !!block);
const { burstHits, autoHitDamage } = new Function(block[1] + "\nreturn { burstHits, autoHitDamage };")();

// ---- Fireball burst — hits every foe within Chebyshev radius of the centre tile ----
const arr = [
  { name: "centre", side: "foe", alive: true, x: 8, y: 4 },   // on the target tile
  { name: "adjacent", side: "foe", alive: true, x: 9, y: 5 }, // cheb 1 — in
  { name: "diagonal", side: "foe", alive: true, x: 7, y: 3 }, // cheb 1 — in
  { name: "far", side: "foe", alive: true, x: 11, y: 4 },     // cheb 3 — out
  { name: "dead", side: "foe", alive: false, x: 8, y: 4 },    // corpse — skipped
  { name: "ally", side: "hero", alive: true, x: 8, y: 4 },    // friendly — never hit
];
const hits = burstHits(arr, "hero", 8, 4, 1).map(t => t.name).sort();
check("Fireball blast hits exactly the live foes inside radius 1", JSON.stringify(hits) === JSON.stringify(["adjacent", "centre", "diagonal"]));
check("Fireball never hits the caster's own side", !burstHits(arr, "hero", 8, 4, 1).some(t => t.side === "hero"));
check("Fireball never hits a downed foe", !burstHits(arr, "hero", 8, 4, 1).some(t => t.alive === false));
check("a larger radius catches the far foe too", burstHits(arr, "hero", 8, 4, 3).some(t => t.name === "far"));
check("radius defaults to 1 when unset", burstHits(arr, "hero", 8, 4).length === 3);

// ---- Magic Missile — never-miss damage is the exact expected value ----
check("autoHit 3d4 averages exactly 7.5 (no ability mod)", autoHitDamage({ damageDice: "3d4", addMod: false }, 5) === 7.5);
check("autoHit adds the ability mod when addMod is set", autoHitDamage({ damageDice: "2d6+1", addMod: true }, 4) === 2 * 3.5 + 1 + 4);
check("autoHit of nothing is zero", autoHitDamage({ damageDice: "" }, 9) === 0);

// ---- resolve / forecast paths for the new ability kinds ----
check("resolve() has an autoHit branch that always hits", /if\(ab\.autoHit\)\{r\.hit=true;r\.dmg=/.test(h));
check("resolve() has a selfbuff branch that applies an effect", /if\(ab\.selfbuff\)\{r\.hit=true;r\.effect=ab\.effect/.test(h));
check("forecast() shows 100% for an autoHit spell", h.includes("if(ab.autoHit)") && h.includes("fc.hit=1;fc.dmg=autoHitDamage(ab,m)"));

// ---- Naeve the arcanist + her spellbook are wired in ----
check("Naeve is in the roster (INT 18 arcanist)", /name:"Naeve"[\s\S]*?int:18[\s\S]*?cast:"intelligence"/.test(h));
check("Naeve carries the spr_naeve sprite", h.includes('sprite:"spr_naeve"'));
check("Magic Missile is a never-miss force bolt", /name:"Magic Missile",autoHit:true,damageDice:"3d4",dmgType:"force"/.test(h));
check("Fireball is a dex-save burst that seeds a fire surface", /name:"Fireball"[\s\S]*?targeting:"enemyburst"[\s\S]*?createsSurface:"fire"/.test(h));
check("Mirror Image is a self-buff", /name:"Mirror Image",selfbuff:true/.test(h));
check("Mirror Image grants +4 AC for 3 rounds", /MirrorImage=\{[\s\S]*?armorClassModifier:4,durationRounds:3/.test(h));

// ---- targeting + dispatch wiring ----
check("legalTargets treats selfbuff as a self-target", h.includes('ab.targeting==="selfburst"||ab.targeting==="selfbuff"'));
check("legalTargets routes enemyburst through the enemy filter", h.includes("enemy & enemyburst pick a target foe"));
check("useAbility dispatches selfbuff", /if\(ab\.selfbuff\)\{ log/.test(h));
check("useAbility dispatches enemyburst through burstHits", h.includes('ab.targeting==="enemyburst"') && h.includes("burstHits(units,att.side,target.x,target.y,ab.radius)"));
check("the click handler arms a selfbuff onto the caster", h.includes('armed.targeting==="selfburst"||armed.targeting==="selfbuff"'));
check("the action bar labels auto / burst / self-buff abilities", h.includes('ab.selfbuff?"self buff"') && h.includes('ab.autoHit?"auto "'));

console.log(`\n  BG2 arcanist — burst targeting + never-miss + wiring:`);
for (const f of fails) console.log("  ✗ " + f);
console.log(`  ${pass} passed, ${fail} failed`);
process.exit(fail ? 1 : 0);
