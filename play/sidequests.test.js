// The Side-Quest Catalog gate. Validates play/sidequests.json — the structured, tracked side quests —
// against the live QuestManager engine, proves every quest actually RUNS (start → objectives advance →
// completes / fails), and cross-checks that every flag the catalog references is set by REAL walkable
// content (so these are live tracked quests, not dead data).
const fs = require("fs");
const path = require("path");
const { GameFlags, QuestStatus, Quest, Objective, QuestManager } = require("./quests.js");

let pass = 0, fail = 0; const fails = [];
function check(name, cond) { if (cond) pass++; else { fail++; fails.push(name); } }

const CAT = JSON.parse(fs.readFileSync(path.join(__dirname, "sidequests.json"), "utf8"));
const QUESTS = CAT.sidequests;

// ---- structure ----
check("the catalog is a substantial set of side quests", Array.isArray(QUESTS) && QUESTS.length >= 16);
check("every quest has a unique questId", new Set(QUESTS.map(q => q.questId)).size === QUESTS.length);
check("every quest carries a sq.* id", QUESTS.every(q => /^sq\./.test(q.questId)));
check("every quest is fully designed (title + premise + theme + region + giver + reward)", QUESTS.every(q =>
  q.title && q.premise && q.theme && q.region && q.giver && q.reward));
check("every quest has a non-empty completionFlag", QUESTS.every(q => typeof q.completionFlag === "string" && q.completionFlag.length));
check("every objective is well-formed (objectiveId + completionFlag + a player-facing desc)", QUESTS.every(q =>
  (q.objectives || []).every(o => o.objectiveId && typeof o.completionFlag === "string" && o.completionFlag.length && o.desc)));
check("optional/hidden objective markers are booleans where present", QUESTS.every(q =>
  (q.objectives || []).every(o => (o.optional === undefined || typeof o.optional === "boolean") &&
    (o.hidden === undefined || typeof o.hidden === "boolean"))));
check("the catalog spans the whole game (Realm-side, Sigil doors, the road, and the meta-ledger)", (() => {
  const regions = QUESTS.map(q => q.region.toLowerCase()).join(" | ");
  return /realm-side/.test(regions) && /sigil/.test(regions) && /wayward mile/.test(regions) && /everywhere/.test(regions);
})());
check("the catalog includes branching, multi-outcome quests (the Calloway secret can ruin or protect)", (() => {
  const knife = QUESTS.find(q => q.questId === "sq.the_knife_you_carry");
  const ids = (knife.objectives || []).map(o => o.objectiveId);
  return ids.includes("ruin") && ids.includes("protect");
})());

// ---- LIVE: every flag the catalog references is actually SET by real walkable content ----
const contentFlags = new Set();
for (const file of fs.readdirSync(__dirname)) {
  if (!file.endsWith(".json")) continue;
  let data; try { data = JSON.parse(fs.readFileSync(path.join(__dirname, file), "utf8")); } catch { continue; }
  const convs = data.conversations || (data.scene ? [] : null);
  if (!convs) continue;
  for (const c of convs) for (const n of (c.nodes || [])) {
    const sinks = [n.effects, n.onEnter].filter(Boolean);
    for (const ch of (n.choices || [])) if (ch.effects) sinks.push(ch.effects);
    for (const arr of sinks) for (const e of arr) if (e && e.key) contentFlags.add(e.key);
  }
}
check("content scan found a large flag namespace (sanity)", contentFlags.size > 200);
const allCatalogFlags = [];
for (const q of QUESTS) {
  allCatalogFlags.push(q.completionFlag);
  if (q.failureFlag) allCatalogFlags.push(q.failureFlag);
  for (const o of (q.objectives || [])) allCatalogFlags.push(o.completionFlag);
}
const deadFlags = [...new Set(allCatalogFlags)].filter(f => !contentFlags.has(f));
check("EVERY catalog flag (objective + completion + failure) is set by real content — no dead quests: " + (deadFlags.length ? deadFlags.join(", ") : "all live"), deadFlags.length === 0);

// ---- ENGINE: every quest actually runs on the QuestManager ----
function runQuest(q) {
  const flags = new GameFlags();
  const quest = Quest({ questId: q.questId, completionFlag: q.completionFlag, failureFlag: q.failureFlag || "",
    objectives: (q.objectives || []).map(o => Objective({ objectiveId: o.objectiveId, completionFlag: o.completionFlag, optional: !!o.optional, hidden: !!o.hidden })) });
  const mgr = new QuestManager(flags, [quest]);
  let objFired = 0, completed = false;
  mgr.onObjectiveCompleted.push(() => objFired++);
  mgr.onQuestCompleted.push(() => completed = true);
  mgr.StartQuest(q.questId);
  if (mgr.StatusOf(q.questId) !== QuestStatus.Active) return { ok: false, why: "did not activate" };
  // fire each objective flag — the manager should report each objective as it lands
  for (const o of (q.objectives || [])) flags.SetBool(o.completionFlag, true);
  // fire the completion flag — quest should complete
  flags.SetBool(q.completionFlag, true);
  if (mgr.StatusOf(q.questId) !== QuestStatus.Completed || !completed) return { ok: false, why: "did not complete" };
  return { ok: true, objFired };
}
const runs = QUESTS.map(runQuest);
check("every quest activates on StartQuest and completes when its completion flag fires", runs.every(r => r.ok));
check("quests with objectives report objective completion as the flags land", (() => {
  return QUESTS.every((q, i) => (q.objectives || []).length === 0 || runs[i].objFired >= 1);
})());

// failure precedence: a quest with a failure flag fails (and failure beats completion), per the C# engine
check("a failure flag fails an active quest, and failure wins over a simultaneous completion (engine contract)", (() => {
  const flags = new GameFlags();
  const q = Quest({ questId: "sq._probe", completionFlag: "c", failureFlag: "f" });
  const m = new QuestManager(flags, [q]);
  m.StartQuest("sq._probe");
  flags.bools["c"] = true; flags.bools["f"] = true;     // both true BEFORE any reactive pass...
  flags.SetBool("c", true);                              // ...then one pass: failure is checked first → Failed wins
  return m.StatusOf("sq._probe") === QuestStatus.Failed;
})());

// save round-trip of the whole catalog's live status
check("the whole catalog round-trips through ExportState/ImportState (save system)", (() => {
  const flags = new GameFlags();
  const quests = QUESTS.map(q => Quest({ questId: q.questId, completionFlag: q.completionFlag }));
  const m = new QuestManager(flags, quests);
  QUESTS.forEach(q => m.StartQuest(q.questId));
  flags.SetBool(QUESTS[0].completionFlag, true);
  const saved = m.ExportState();
  const m2 = new QuestManager(new GameFlags(), quests);
  m2.ImportState(saved);
  return m2.StatusOf(QUESTS[0].questId) === QuestStatus.Completed && m2.StatusOf(QUESTS[1].questId) === m.StatusOf(QUESTS[1].questId);
})());

console.log(`\n  The Side-Quest Catalog — ${QUESTS.length} tracked, branching quests on the live QuestManager:`);
console.log(`  ${pass} passed, ${fail} failed`);
if (fail) { fails.forEach(f => console.log("   ✗ " + f)); process.exit(1); }
else console.log(`  ✓ every quest runs on the engine, and every flag is set by real walkable content (no dead quests).\n`);
