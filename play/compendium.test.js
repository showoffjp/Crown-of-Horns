// Structural smoke for the generated Compendium + the index hub: assert every tab,
// the cross-section content, embedded tokens, and the hub links are present and the
// pages are well-formed. (Content accuracy is pinned upstream by the engine tests;
// this guards that the committed pages aren't empty or broken.)
const fs = require("fs");
let pass = 0, fail = 0; const fails = [];
function check(name, cond) { if (cond) pass++; else { fail++; fails.push(name); } }

const c = fs.readFileSync(__dirname + "/compendium.html", "utf8");
check("compendium: Grimoire tab", c.includes('data-t="grim"'));
check("compendium: Armory tab", c.includes('data-t="arm"'));
check("compendium: Bestiary tab", c.includes('data-t="best"'));
check("compendium: Conditions tab", c.includes('data-t="cond"'));
check("compendium: Codex tab", c.includes('data-t="codex"'));
check("compendium: Atlas tab", c.includes('data-t="atlas"'));
check("compendium: >=30 grimoire/table rows", (c.match(/<tr>/g) || []).length >= 30);
check("compendium: >=30 monster cards", (c.match(/class="mon"/g) || []).length >= 30);
check("compendium: >=6 era filter chips", (c.match(/class="chip/g) || []).length >= 6);
check("compendium: >=7 atlas acts", (c.match(/class="act"/g) || []).length >= 7);
check("compendium: >=6 status-effect cards", (c.match(/class="eff"/g) || []).length >= 6);
check("compendium: full condition vocabulary chips", (c.match(/class="cchip/g) || []).length >= 12);
check("compendium: >=50 codex cards", (c.match(/class="cdx"/g) || []).length >= 50);
check("compendium: codex category filter wired", c.includes("data-cf") && c.includes('dataset.cf'));
check("compendium: embedded enemy tokens", (c.match(/data:image\/jpeg/g) || []).length >= 25);
check("compendium: tab + filter JS present", c.includes('classList.add("on")') && c.includes("dataset.f"));
check("compendium: cross-links", c.includes("crown_combat.html") && c.includes("endings_explorer.html") &&
  c.includes("dialogue_viewer.html") && c.includes("cast_gallery.html"));
check("compendium: <script> balanced", (c.match(/<script>/g) || []).length === (c.match(/<\/script>/g) || []).length);

const idx = fs.readFileSync(__dirname + "/index.html", "utf8");
for (const link of ["crown_combat.html","endings_explorer.html","compendium.html","cast_gallery.html","balance_report.html"])
  check("index hub links " + link, idx.includes(link));

const data = JSON.parse(fs.readFileSync(__dirname + "/compendium-data.json", "utf8"));
check("data: abilities extracted", data.abilities.length >= 30);
check("data: items extracted", data.items.length >= 10);
check("data: enemies extracted", data.enemies.length >= 30);
check("data: enemies have real stats", data.enemies.every(e => e.level > 0 && e.xp > 0 && e.str > 0));
check("data: conditions extracted (full enum)", data.conditions.length >= 12);
check("data: status effects extracted", data.effects.length >= 6);
check("data: effects carry real mechanics", data.effects.every(e => e.rounds > 0 && "incapacitates" in e) &&
  data.effects.some(e => e.dotDice) && data.effects.some(e => e.attackRollMod !== 0) &&
  data.effects.some(e => e.incapacitates));
check("data: codex extracted across categories", data.codex.length >= 60 &&
  new Set(data.codex.map(e => e.category)).size >= 5);
check("data: codex entries well-formed", data.codex.every(e => e.id && e.title && e.body) &&
  data.codex.some(e => e.unlockFlag) && data.codex.some(e => !e.unlockFlag));

console.log(`\n  Compendium + hub — structural smoke:`);
console.log(`  ${pass} passed, ${fail} failed`);
if (fail) { fails.forEach(f => console.log("   ✗ " + f)); process.exit(1); }
else console.log("  ✓ Grimoire/Armory/Bestiary/Atlas render, tokens embed, hub links resolve, content extracted.\n");
