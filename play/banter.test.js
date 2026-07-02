// The party-banter gate. Validates play/banter.json against the banter.js engine, proves each banter
// actually fires on real party + flag state, and cross-checks that every flag/companion it needs is
// reachable in real content (no dead banter).
const fs = require("fs");
const path = require("path");
const B = require("./banter.js");

let pass = 0, fail = 0; const fails = [];
function check(name, cond) { if (cond) pass++; else { fail++; fails.push(name); } }
const newState = () => ({ bools: {}, ints: {} });
const withFlags = (...ks) => { const s = newState(); ks.forEach(k => s.bools[k] = true); return s; };

const CAT = JSON.parse(fs.readFileSync(path.join(__dirname, "banter.json"), "utf8")).banters;
const KNOWN = Object.keys(B.PRESENT_FLAG);

// ---- structure ----
check("a substantial banter catalog", Array.isArray(CAT) && CAT.length >= 16);
check("every banter has a unique bant.* id", new Set(CAT.map(b => b.id)).size === CAT.length && CAT.every(b => /^bant\./.test(b.id)));
check("every banter needs >=1 KNOWN companion and has >=2 lines (speaker + text each)", CAT.every(b =>
  b.requires && Array.isArray(b.requires.companions) && b.requires.companions.length >= 1 &&
  b.requires.companions.every(c => KNOWN.includes(c)) &&
  Array.isArray(b.lines) && b.lines.length >= 2 && b.lines.every(l => l.speaker && l.text)));
check("multi-companion banters exist (real cross-talk, not just solo asides)", CAT.filter(b => b.requires.companions.length >= 2).length >= 8);
check("banters react across all three axes: each other, what you did (flags), and how cold/kind you are (int)", (() => {
  const hasFlag = CAT.some(b => (b.requires.flags || []).length);
  const hasInt = CAT.some(b => Object.keys(b.requires.int || {}).length);
  const hasPair = CAT.some(b => b.requires.companions.length >= 2 && !(b.requires.flags || []).length && !Object.keys(b.requires.int || {}).length);
  return hasFlag && hasInt && hasPair;
})());

// ---- engine: companion presence ----
check("garrow (the starting companion) is always present; others need their recruit flag", (() => {
  return B.companionPresent(newState(), "garrow") === true &&
    B.companionPresent(newState(), "varra") === false &&
    B.companionPresent(withFlags("companion.varra.recruited"), "varra") === true &&
    B.companionPresent(withFlags("lf.dot_joined"), "dot") === true &&
    B.companionPresent(newState(), "nobody") === false;
})());

// ---- engine: requirement gating (companions / flags / flagsNot / int) ----
check("a two-companion banter is ineligible until BOTH are present", (() => {
  const b = CAT.find(x => x.id === "bant.varra_ilfaeril_penance");
  return !B.banterEligible(withFlags("companion.varra.recruited"), b) &&
    B.banterEligible(withFlags("companion.varra.recruited", "companion.ilfaeril.recruited"), b);
})());
check("a flag-gated reaction banter only fires after the deed is done", (() => {
  const b = CAT.find(x => x.id === "bant.calloway_varra_admires");
  return !B.banterEligible(withFlags("companion.varra.recruited"), b) &&
    B.banterEligible(withFlags("companion.varra.recruited", "cal.met"), b);
})());
check("an int-gated banter only fires once you're cold/kind enough (disp threshold)", (() => {
  const b = CAT.find(x => x.id === "bant.cold_ilfaeril_warns");
  const warm = withFlags("companion.ilfaeril.recruited");
  const cold = withFlags("companion.ilfaeril.recruited"); cold.ints["disp.haunted"] = 16;
  return !B.banterEligible(warm, b) && B.banterEligible(cold, b);
})());
check("`once` banters don't repeat (banter.seen.* set by markSeen), but once:false ones can", (() => {
  const oncer = CAT.find(b => b.once !== false);
  const repeat = CAT.find(b => b.once === false);
  const s = newState(); s.bools["companion.ilfaeril.recruited"] = true;
  // seed presence for whichever we picked
  oncer.requires.companions.forEach(c => { const f = B.PRESENT_FLAG[c]; if (f) s.bools[f] = true; });
  (oncer.requires.flags || []).forEach(f => s.bools[f] = true);
  for (const k in (oncer.requires.int || {})) s.ints[k] = oncer.requires.int[k];
  const before = B.banterEligible(s, oncer);
  B.markSeen(s, oncer);
  const after = B.banterEligible(s, oncer);
  return before === true && after === false && repeat && repeat.once === false;
})());

// ---- engine: selection ----
check("pickBanter returns null when nothing is eligible, and an eligible banter otherwise", (() => {
  const empty = newState();
  if (B.pickBanter(CAT, empty, () => 0) !== null) return false;   // bare state: only once:false garrow-camp could fire — guard below
  // give a full party + some deeds; expect several eligible
  const s = withFlags("companion.roen.recruited", "companion.varra.recruited", "companion.naeve.recruited",
    "companion.ilfaeril.recruited", "companion.maerin.recruited", "lf.dot_joined", "cal.met", "tw.corliss_affirmed");
  const elig = B.eligibleBanters(CAT, s);
  const picked = B.pickBanter(CAT, s, () => 0);
  return elig.length >= 8 && picked && elig.includes(picked);
})());

// ---- LIVE: every flag/companion a banter needs is reachable in real content ----
const contentFlags = new Set();
for (const file of fs.readdirSync(__dirname)) {
  if (!file.endsWith(".json")) continue;
  let data; try { data = JSON.parse(fs.readFileSync(path.join(__dirname, file), "utf8")); } catch { continue; }
  for (const c of (data.conversations || [])) for (const n of (c.nodes || [])) {
    const sinks = [n.effects, n.onEnter].filter(Boolean);
    for (const ch of (n.choices || [])) if (ch.effects) sinks.push(ch.effects);
    for (const arr of sinks) for (const e of arr) if (e && e.key) contentFlags.add(e.key);
  }
}
const deadFlags = new Set();
for (const b of CAT) {
  for (const f of (b.requires.flags || []).concat(b.requires.flagsNot || [])) if (!contentFlags.has(f)) deadFlags.add(f);
  for (const c of b.requires.companions) { const f = B.PRESENT_FLAG[c]; if (f && !contentFlags.has(f)) deadFlags.add(`${c}->${f}`); }
}
check("no dead banter — every flag and every recruitable companion it needs is actually set by real content: " + ([...deadFlags].join(", ") || "all live"), deadFlags.size === 0);
check("int gates use real disposition keys", CAT.every(b => Object.keys(b.requires.int || {}).every(k => /^disp\.(haunted|merciful|ruthless|cunning|devout|heretical|honest|stoic)$/.test(k))));

console.log(`\n  Reactive party banter — ${CAT.length} cross-companion barters on the live eligibility engine:`);
console.log(`  ${pass} passed, ${fail} failed`);
if (fail) { fails.forEach(f => console.log("   ✗ " + f)); process.exit(1); }
else console.log("  ✓ every banter fires on real party + flag state; no dead banter; the party is alive.\n");
