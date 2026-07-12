// The reskin gate. Proves the serial-numbers-filed map is COMPLETE (no mapped FR term survives a reskin of
// the whole source), that whole-word boundaries hold (common words like 'drowned', 'damn', 'torment',
// 'mask', 'chosen' are NOT corrupted), that every FR deity the reactive `when.deity` gates rely on is
// covered (so faith-reactivity survives the swap), and that reskinned content is still valid, reference-
// clean JSON. No C#/Unity needed — this is pure text + structure verification.
"use strict";
const fs = require("fs");
const path = require("path");
const R = require("./reskin.js");

let pass = 0, fail = 0; const fails = [];
function check(name, cond) { if (cond) pass++; else { fail++; fails.push(name); } }

// ---- boundary safety: the words the map must never touch ----
const SAFE = ["drowned", "drowning", "drowsy", "damn", "damned", "damnation", "torment", "tormented",
  "mask", "masked", "chosen", "reaper", "shard", "sharp", "share", "harmony", "banish", "urbane", "column", "helmet"];
check("whole-word boundaries protect common words (drowned/damn/torment/mask/chosen/shard...)", (() => {
  for (const w of SAFE) if (R.reskin(w) !== w) return false;
  // and in a sentence
  const s = "The drowned, damned soul writhed in torment behind its mask, chosen and sharp as a shard.";
  return R.reskin(s) === s;
})());

// ---- the swaps actually happen, case-sensitively ----
check("FR proper nouns are swapped (Kelemvor/Sigil/Faithless/Doomguide)", (() => {
  const s = R.reskin("Kelemvor's Doomguide walked the Wall of the Faithless in Sigil.");
  return !/Kelemvor|Doomguide|Faithless|Sigil/.test(s) && /Sarran/.test(s) && /Greywarden/.test(s) && /Unclaimed/.test(s) && /Threnn/.test(s);
})());
check("possessive 's is preserved (Kelemvor's -> Sarran's)", R.reskin("Kelemvor's") === "Sarran's");
check("longest-match-first: 'Wall of the Faithless' becomes 'Wall of the Unclaimed', not 'Wall of the Unclaimed' via two passes", R.reskin("the Wall of the Faithless") === "the Wall of the Unclaimed");
check("'Baldur's Gate' resolves as a whole before 'Baldur'", R.reskin("Baldur's Gate") === "Ardengate");
check("'drow' swaps but 'drowned' does not", R.reskin("the drow drowned") === "the duskling drowned");
check("case-sensitive: 'Bane' (god) swaps, 'bane' (word) is left", R.reskin("Bane was his bane") === "Vorrus was his bane");
check("'tiefling' is intentionally KEPT (CC-licensed SRD)", R.reskin("a tiefling") === "a tiefling");

// ---- COMPLETENESS: reskin the entire source; no mapped FR term may survive ----
let totalRemaining = 0; const survivors = {};
for (const rel of R.sourceFiles()) {
  const src = fs.readFileSync(path.join(__dirname, "..", rel), "utf8");
  const out = R.reskin(src);
  const rem = R.remainingTerms(out);
  for (const k of Object.keys(rem)) { survivors[k] = (survivors[k] || 0) + rem[k]; totalRemaining += rem[k]; }
}
check("a full reskin of all source leaves ZERO mapped FR terms: " + (Object.keys(survivors).join(", ") || "clean"), totalRemaining === 0);

// ---- deity coverage: every FR deity the reactive gates use must be in the map ----
// (a `when.deity`/build value that isn't mapped would leave faith-reactivity half-reskinned)
const KNOWN_FR_DEITIES = ["Kelemvor", "Myrkul", "Jergal", "Lathander", "Mystra", "Oghma", "Tymora", "Bane", "Bhaal", "Cyric", "Tyr", "Ilmater", "Chauntea", "Mielikki"];
check("every FR deity used by faith-reactivity is covered by the map", KNOWN_FR_DEITIES.every(d => d in R.MAP.map));
const deityVals = new Set();
for (const f of fs.readdirSync(path.join(__dirname, "..", "play"))) {
  if (!f.endsWith(".json")) continue;
  let d; try { d = JSON.parse(fs.readFileSync(path.join(__dirname, "..", "play", f), "utf8")); } catch { continue; }
  const scan = (w) => { if (w && w.deity) [].concat(w.deity).forEach(v => deityVals.add(v)); };
  for (const c of (d.conversations || [])) for (const n of (c.nodes || [])) {
    for (const v of (n.variants || [])) scan(v.when);
    for (const ch of (n.choices || [])) scan(ch.when);
  }
}
const uncovered = [...deityVals].filter(v => v !== "None" && !(v in R.MAP.map));
check("every `when.deity` gate value in real content is mapped (or the generic 'None'): " + (uncovered.join(", ") || "all covered"), uncovered.length === 0);

// ---- reskinned JSON stays valid, and introduces NO NEW broken references ----
// (dialogue-data.json has a handful of pre-existing dynamic dangles, resolved by runtime C#; the honest
//  test is that reskinning adds none of its own — node ids and ref targets swap in lockstep or not at all.)
function unresolvedRefs(data) {
  const bad = [];
  for (const c of (data.conversations || [])) {
    const ids = new Set((c.nodes || []).map(n => n.id));
    for (const n of (c.nodes || [])) {
      const refs = [];
      if (n.auto) refs.push(n.auto);
      for (const ch of (n.choices || [])) for (const k of ["next", "fail", "crit", "fumble"]) if (ch[k]) refs.push(ch[k]);
      for (const r of refs) if (r !== "END" && r !== "end" && !ids.has(r)) bad.push(`${c.id}[${n.id}]->${r}`);
    }
  }
  return bad;
}
let jsonOk = true, introduced = 0; const badFiles = []; const newBreaks = [];
for (const f of fs.readdirSync(path.join(__dirname, "..", "play"))) {
  if (!f.endsWith(".json")) continue;
  const src = fs.readFileSync(path.join(__dirname, "..", "play", f), "utf8");
  let orig, resk;
  try { orig = JSON.parse(src); resk = JSON.parse(R.reskin(src)); } catch { jsonOk = false; badFiles.push(f); continue; }
  const before = new Set(unresolvedRefs(orig));
  for (const b of unresolvedRefs(resk)) if (!before.has(b)) { introduced++; newBreaks.push(f + ":" + b); }
}
check("reskinned content is still valid JSON: " + (badFiles.join(", ") || "all parse"), jsonOk);
check("reskinning introduces NO new broken references (ids/targets swap in lockstep): " + (newBreaks.slice(0, 4).join(", ") || "none"), introduced === 0);

console.log(`\n  Reskin layer — ${R.RULES.length} mapped FR terms, whole-word/case-sensitive, longest-first:`);
console.log(`  ${pass} passed, ${fail} failed`);
if (fail) { fails.forEach(f => console.log("   ✗ " + f)); process.exit(1); }
else console.log("  ✓ the game reskins clean: zero FR terms survive, no common word is corrupted, structure holds.\n");
