// Structural smoke for the Dialogue Tree Viewer + its extracted data. Guards that the
// page is well-formed, the script compiles, and the data carries the real branching
// content (nodes, choices, skill checks, flag effects) pulled from the C# DialogueGraphs.
// Content correctness is pinned upstream by DialogueRunnerTests; this guards the export.
const fs = require("fs");
let pass = 0, fail = 0; const fails = [];
function check(name, cond) { if (cond) pass++; else { fail++; fails.push(name); } }

const data = JSON.parse(fs.readFileSync(__dirname + "/dialogue-data.json", "utf8"));
const S = data.stats, C = data.conversations;
check("data: >=40 conversations", C.length >= 40);
check("data: >=140 nodes", S.nodes >= 140);
check("data: >=120 choices", S.choices >= 120);
check("data: >=10 skill checks", S.skillChecks >= 10);
check("data: stats match arrays", S.conversations === C.length &&
  S.nodes === C.reduce((a, c) => a + c.nodes.length, 0));
check("data: every node has id+speaker", C.every(c => c.nodes.every(n => n.id && n.speaker)));
check("data: every conversation has a start node present",
  C.every(c => c.nodes.some(n => n.id === c.start)));
// the marquee branch: Ilfaeril's WIS DC16 insight check with its fail branch + triple effect
const ilf = C.find(c => c.id === "ilfaeril.quest.resolution");
check("data: Ilfaeril resolution present", !!ilf);
const chk = ilf && ilf.nodes.find(n => n.id === "0").choices.find(c => c.checkDC);
check("data: skill check carries ability+DC+fail",
  chk && chk.checkAbility === "Wisdom" && chk.checkDC === 16 && chk.fail === "commission_fail");
check("data: choice effects extracted (flag writes)",
  ilf && ilf.nodes.find(n => n.id === "0").choices.some(c => (c.effects || []).length >= 3));
check("data: onEnter effects extracted somewhere",
  C.some(c => c.nodes.some(n => (n.onEnter || []).length > 0)));
check("data: reactive (dynamic) lines marked, not invented",
  C.some(c => c.nodes.some(n => n.dynamic && n.text === null)));

const h = fs.readFileSync(__dirname + "/dialogue_viewer.html", "utf8");
check("page: <script> balanced", (h.match(/<script>/g) || []).length === (h.match(/<\/script>/g) || []).length);
check("page: has Map + Walk modes", h.includes("setMode('map')") && h.includes("setMode('walk')"));
check("page: embeds the data blob", h.includes("const DATA =") && h.includes('"conversations"'));
check("page: renders an SVG graph", h.includes("renderMap") && h.includes("<svg"));
check("page: skill-check edge styling present", h.includes("edge.check") && h.includes("DC"));
check("page: links back to hub", h.includes('href="index.html"'));
// compile the page's script (parse-only; does not execute) to catch JS syntax errors
const m = h.match(/<script>([\s\S]*)<\/script>/);
check("page: script block found", !!m);
try { new Function(m[1] + "\n//# sourceURL=viewer"); check("page: script compiles (no syntax errors)", true); }
catch (e) { check("page: script compiles (no syntax errors) — " + e.message, false); }

console.log(`\n  Dialogue Tree Viewer — structural smoke:`);
console.log(`  ${pass} passed, ${fail} failed`);
if (fail) { fails.forEach(f => console.log("   ✗ " + f)); process.exit(1); }
else console.log(`  ✓ ${C.length} conversations / ${S.nodes} nodes / ${S.skillChecks} checks export, page compiles, Map+Walk wired.\n`);
