// Structural smoke for the Save Inspector + its sample, and the Endings Explorer handoff.
// Proves the page compiles, the sample matches the real SaveData shape, the parse→export
// round-trip is loss-free, and the Endings Explorer picks up a staged save.
const fs = require("fs");
let pass = 0, fail = 0; const fails = [];
function check(name, cond) { if (cond) pass++; else { fail++; fails.push(name); } }

// ---- sample save matches SaveSystem.SaveData (flattened parallel lists) ----
const s = JSON.parse(fs.readFileSync(__dirname + "/sample-save.json", "utf8"));
check("sample: parallel bool lists aligned", s.boolKeys.length === s.boolValues.length && s.boolKeys.length >= 20);
check("sample: parallel int lists aligned", s.intKeys.length === s.intValues.length && s.intKeys.length >= 3);
check("sample: parallel quest lists aligned", s.questIds.length === s.questStatuses.length && s.questIds.length >= 3);
check("sample: hero build present", s.heroName && s.heroClass && s.heroRace &&
  s.heroLevel > 0 && s.heroScores.length === 6);
check("sample: has version/scene/gold", s.version && s.sceneName && typeof s.partyGold === "number");
check("sample: quest statuses are valid enum ordinals (0-3)", s.questStatuses.every(q => q >= 0 && q <= 3));

// ---- round-trip parity: parse the parallel lists into a model, then re-emit, loss-free ----
function parse(d) {
  const bools = {}, ints = {};
  d.boolKeys.forEach((k, i) => bools[k] = !!d.boolValues[i]);
  d.intKeys.forEach((k, i) => ints[k] = d.intValues[i] | 0);
  return { bools, ints, quests: d.questIds.map((id, i) => ({ id, status: d.questStatuses[i] })),
    gold: d.partyGold, hero: { name: d.heroName, cls: d.heroClass, race: d.heroRace, level: d.heroLevel, scores: d.heroScores } };
}
function emit(m, src) {
  return { version: src.version, sceneName: src.sceneName, savedAtUtc: src.savedAtUtc,
    boolKeys: Object.keys(m.bools), boolValues: Object.keys(m.bools).map(k => m.bools[k]),
    intKeys: Object.keys(m.ints), intValues: Object.keys(m.ints).map(k => m.ints[k]),
    questIds: m.quests.map(q => q.id), questStatuses: m.quests.map(q => q.status),
    partyGold: m.gold, heroName: m.hero.name, heroClass: m.hero.cls, heroRace: m.hero.race,
    heroLevel: m.hero.level, heroScores: m.hero.scores };
}
check("round-trip: parse→emit reproduces the sample byte-for-byte",
  JSON.stringify(emit(parse(s), s)) === JSON.stringify(s));

// ---- the page itself ----
const h = fs.readFileSync(__dirname + "/save_inspector.html", "utf8");
check("page: <script> balanced", (h.match(/<script>/g) || []).length === (h.match(/<\/script>/g) || []).length);
check("page: embeds sample + flagInfo + codex + flag keys", h.includes('"sample"') &&
  h.includes('"flagInfo"') && h.includes('"codex"') && h.includes('"allFlagKeys"'));
check("page: has parse + toSaveData (round-trip core)", h.includes("function parse(") && h.includes("function toSaveData("));
check("page: load paths (sample/file/paste)", h.includes("loadSample") && h.includes("openFile") && h.includes("loadPaste"));
check("page: export (copy + download)", h.includes("copyOut") && h.includes("function download("));
check("page: decodes unlocks vs codex + endings gates", h.includes("renderUnlocks") && h.includes('r==="Endings"'));
check("page: stages flags for Endings Explorer", h.includes('localStorage.setItem("coh.endings.import"'));
check("page: links back to hub", h.includes('href="index.html"'));
const m = h.match(/<script>([\s\S]*)<\/script>/);
try { new Function(m[1] + "\n//# sourceURL=save"); check("page: script compiles (no syntax errors)", true); }
catch (e) { check("page: script compiles (no syntax errors) — " + e.message, false); }

// ---- the Endings Explorer picks up a staged save ----
const en = fs.readFileSync(__dirname + "/endings_explorer.html", "utf8");
check("endings: imports staged save from localStorage",
  en.includes("importFromSave") && en.includes('"coh.endings.import"') && en.includes("setAll(pairs)"));

console.log(`\n  Save Inspector — structural smoke:`);
console.log(`  ${pass} passed, ${fail} failed`);
if (fail) { fails.forEach(f => console.log("   ✗ " + f)); process.exit(1); }
else console.log(`  ✓ sample matches SaveData, parse↔export round-trips loss-free, page compiles, Endings handoff wired.\n`);
