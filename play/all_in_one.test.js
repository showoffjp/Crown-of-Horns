// Structural smoke for the single-file bundle (crown_of_horns.html): every page is
// embedded and non-empty, the JSON blob parses, the tab/iframe shell is wired, and the
// cross-link rewiring + hash deep-linking are present. Guards that the bundle isn't
// stale or truncated relative to the standalone pages.
const fs = require("fs");
let pass = 0, fail = 0; const fails = [];
function check(name, cond) { if (cond) pass++; else { fail++; fails.push(name); } }

const h = fs.readFileSync(__dirname + "/crown_of_horns.html", "utf8");
const TABS = ["combat", "dialogue", "play", "endings", "compendium", "cast", "flags", "saga", "save", "analytics"];

check("bundle: outer shell present", h.includes('id="frame"') && h.includes('class="tabs"'));
check("bundle: a tab button per page", TABS.every(k => h.includes(`data-k="${k}"`)));
check("bundle: lazy srcdoc loader", h.includes("frame.srcdoc = PAGES"));
check("bundle: cross-link rewiring", h.includes("function rewire(") && h.includes("FILE_TO_TAB"));
check("bundle: hash deep-linking + tab switch", h.includes("bootFromHash") && h.includes("applyHash"));

// extract and parse the embedded PAGES blob
const m = h.match(/const PAGES = (\{[\s\S]*?\});\nconst FILE_TO_TAB/);
check("bundle: PAGES blob found", !!m);
let pages = null;
try { pages = JSON.parse(m[1].split("<\\/").join("</")); check("bundle: PAGES blob parses", true); }
catch (e) { check("bundle: PAGES blob parses — " + e.message, false); }

if (pages) {
  check("bundle: all 10 pages embedded", TABS.every(k => k in pages));
  check("bundle: the playable dialogue sim carries its engine", (pages.play || "").includes("DLGSIM") &&
    pages.play.includes("function rollDice("));
  check("bundle: every page is real HTML", Object.values(pages).every(p => p.length > 200 &&
    /<!DOCTYPE|<html|<body/i.test(p)));
  // the marquee pages carry their real, interactive payloads
  check("bundle: dialogue page carries the graphs", (pages.dialogue || "").includes('"conversations"') &&
    pages.dialogue.includes("renderMap"));
  check("bundle: combat page is the real engine demo", (pages.combat || "").length > 10000);
  check("bundle: compendium carries embedded tokens", (pages.compendium || "").includes("data:image/jpeg"));
  check("bundle: flags page carries the graph data", (pages.flags || "").includes('"flags"') &&
    pages.flags.includes("showFlag"));
  // the interactive pages kept their <script> (saga map + analytics are static reports)
  const interactive = ["combat", "dialogue", "play", "endings", "compendium", "cast", "flags", "save"];
  check("bundle: interactive pages kept their <script>", interactive.every(k =>
    (pages[k] || "").includes("<script")));
}

console.log(`\n  All-in-one bundle — structural smoke:`);
console.log(`  ${pass} passed, ${fail} failed`);
if (fail) { fails.forEach(f => console.log("   ✗ " + f)); process.exit(1); }
else console.log(`  ✓ 10 pages bundled & parse, tab/iframe shell + cross-link rewiring + deep-links wired.\n`);
