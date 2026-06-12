// Structural smoke for the Saga Map — the campaign flowchart that stitches the suite together.
// Guards that the spine renders and that its deep-links into the other explorers resolve to
// real targets (conversations in the dialogue data, flags in the flag data).
const fs = require("fs");
let pass = 0, fail = 0; const fails = [];
function check(name, cond) { if (cond) pass++; else { fail++; fails.push(name); } }

const h = fs.readFileSync(__dirname + "/saga_map.html", "utf8");
const dlg = JSON.parse(fs.readFileSync(__dirname + "/dialogue-data.json", "utf8"));
const flg = JSON.parse(fs.readFileSync(__dirname + "/flags-data.json", "utf8"));
const convIds = new Set(dlg.conversations.map(c => c.id));
const flagKeys = new Set(flg.flags.map(f => f.key));

check("page: 6 act stations", (h.match(/class="station"/g) || []).length === 6);
check("page: Court of the Dead with endings", h.includes("Court of the Dead") && (h.match(/class="ending/g) || []).length === 6);
check("page: two golden endings marked", (h.match(/class="ending golden"/g) || []).length === 2);
check("page: side lanes (quests + vault)", (h.match(/class="lane"/g) || []).length >= 2);
check("page: links back to hub", h.includes('href="index.html"'));

// every conversation deep-link points at a real conversation id
const cvLinks = [...h.matchAll(/dialogue_viewer\.html#([^"]+)/g)].map(m => decodeURIComponent(m[1]));
check("links: >=40 conversation deep-links", cvLinks.length >= 40);
check("links: every conversation link resolves to a real conversation", cvLinks.every(id => convIds.has(id)));

// every flag deep-link points at a real flag key
const flLinks = [...h.matchAll(/flags_explorer\.html#([^"]+)/g)].map(m => decodeURIComponent(m[1]));
check("links: >=30 flag deep-links", flLinks.length >= 30);
check("links: every flag link resolves to a real flag", flLinks.every(k => flagKeys.has(k)));

// the deep-link targets are honored by the generated viewers (hash boot)
const dv = fs.readFileSync(__dirname + "/dialogue_viewer.html", "utf8");
const fe = fs.readFileSync(__dirname + "/flags_explorer.html", "utf8");
check("viewers: dialogue viewer honors #hash deep-link", dv.includes("location.hash") && dv.includes("hashchange"));
check("viewers: flag graph honors #hash deep-link", fe.includes("location.hash") && fe.includes("showFlag(want)"));

console.log(`\n  Saga Map — structural smoke:`);
console.log(`  ${pass} passed, ${fail} failed`);
if (fail) { fails.forEach(f => console.log("   ✗ " + f)); process.exit(1); }
else console.log(`  ✓ spine + court + lanes render; ${cvLinks.length} conversation & ${flLinks.length} flag deep-links all resolve.\n`);
