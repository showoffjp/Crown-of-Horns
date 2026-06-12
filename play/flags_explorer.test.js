// Structural smoke for the Flag Dependency Graph + its harvested data. Guards that the
// graph is well-formed, the page compiles, and the model carries real read/write edges
// merged from the C#, the dialogue clauses, and the codex unlocks.
const fs = require("fs");
let pass = 0, fail = 0; const fails = [];
function check(name, cond) { if (cond) pass++; else { fail++; fails.push(name); } }

const data = JSON.parse(fs.readFileSync(__dirname + "/flags-data.json", "utf8"));
const S = data.stats, F = data.flags;
check("data: >=120 flags", F.length >= 120);
check("data: stats.flags matches array", S.flags === F.length);
check("data: >=15 domains", data.domains.length >= 15);
check("data: >=300 read/write edges", S.edges >= 300);
check("data: every flag has key+domain", F.every(f => f.key && f.domain));
check("data: edges count is consistent",
  S.edges === F.reduce((a, f) => a + f.writers.length + f.readers.length, 0));
check("data: writers/readers carry a via", F.every(f =>
  f.writers.every(w => w.source && w.via) && f.readers.every(r => r.source && r.via)));
check("data: dialogue + code + codex sources all present", (() => {
  const vias = new Set();
  F.forEach(f => [...f.writers, ...f.readers].forEach(e => vias.add(e.via)));
  return vias.has("dialogue") && vias.has("code") && vias.has("codex");
})());
// the marquee dependency: an ending gate read by the Endings system + codex
const cw = F.find(f => f.key === "crownwars.cleared");
check("data: crownwars.cleared present", !!cw);
check("data: ending-gate flag read by Endings + codex", cw &&
  cw.readers.some(r => r.source === "Endings") && cw.readers.some(r => r.via === "codex"));
// a quest flag written by dialogue
const qr = F.find(f => f.key === "quest.ilfaeril.resolved");
check("data: quest flag written by dialogue, read by endings", qr &&
  qr.writers.some(w => w.via === "dialogue") && qr.readers.some(r => r.source === "Endings"));
check("data: orphan classification consistent", F.every(f =>
  (f.orphanWrite === (f.readers.length === 0)) && (f.orphanRead === (f.writers.length === 0))));

const h = fs.readFileSync(__dirname + "/flags_explorer.html", "utf8");
check("page: <script> balanced", (h.match(/<script>/g) || []).length === (h.match(/<\/script>/g) || []).length);
check("page: embeds the data blob", h.includes("const DATA =") && h.includes('"flags"'));
check("page: bipartite writers/readers render", h.includes("showFlag") && h.includes("writes ▸") && h.includes("◂ reads"));
check("page: overview with domains + orphans", h.includes("showOverview") && h.includes("Write-only") && h.includes("Hub flags"));
check("page: via legend (dialogue/code/codex)", h.includes("via-dialogue") && h.includes("via-code") && h.includes("via-codex"));
check("page: links back to hub", h.includes('href="index.html"'));
const m = h.match(/<script>([\s\S]*)<\/script>/);
try { new Function(m[1] + "\n//# sourceURL=flags"); check("page: script compiles (no syntax errors)", true); }
catch (e) { check("page: script compiles (no syntax errors) — " + e.message, false); }

console.log(`\n  Flag Dependency Graph — structural smoke:`);
console.log(`  ${pass} passed, ${fail} failed`);
if (fail) { fails.forEach(f => console.log("   ✗ " + f)); process.exit(1); }
else console.log(`  ✓ ${F.length} flags / ${S.edges} edges / ${data.domains.length} domains, page compiles, bipartite + overview wired.\n`);
