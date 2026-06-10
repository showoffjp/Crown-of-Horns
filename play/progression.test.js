// ProgressionTests, ported 1:1 against the JS leveling port.
const { Progression, MaxLevel, INT_MAX } = require("./progression.js");
const Fresh = () => ({ level: 1, experience: 0, classDef: null, maxHitPoints: 10, currentHitPoints: 10 });

let pass = 0, fail = 0; const fails = [];
function test(name, fn) { try { fn(); pass++; } catch (e) { fail++; fails.push(`${name}: ${e.message}`); } }
const eq = (a, b, m) => { if (a !== b) throw new Error(`${m || ""} expected ${b}, got ${a}`); };

test("XpToReach_MatchesTable", () => {
  eq(Progression.XpToReach(1), 0); eq(Progression.XpToReach(2), 300);
  eq(Progression.XpToReach(5), 6500); eq(Progression.XpToReach(20), 355000);
});
test("XpToReach_OutOfRange_IsMaxValue", () => { eq(Progression.XpToReach(0), INT_MAX); eq(Progression.XpToReach(21), INT_MAX); });
test("AwardExperience_CrossingThreshold_LevelsUpOnce", () => {
  const s = Fresh(); eq(Progression.AwardExperience(s, 300), 1); eq(s.level, 2); eq(s.experience, 300);
});
test("AwardExperience_LargeGrant_JumpsMultipleLevels", () => {
  const s = Fresh(); const g = Progression.AwardExperience(s, 6500); eq(s.level, 5); eq(g, 4);
});
test("AwardExperience_NonPositive_IsNoOp", () => {
  const s = Fresh(); eq(Progression.AwardExperience(s, 0), 0); eq(Progression.AwardExperience(s, -50), 0);
  eq(s.level, 1); eq(s.experience, 0);
});
test("AwardExperience_CapsAtMaxLevel", () => {
  const s = Fresh(); Progression.AwardExperience(s, 10000000);
  eq(s.level, Progression.MaxLevel); eq(Progression.AwardExperience(s, 10000), 0, "no XP past max");
});
test("XpToNextLevel_ReportsRemaining_AndZeroAtCap", () => {
  const s = Fresh(); eq(Progression.XpToNextLevel(s), 300);
  Progression.AwardExperience(s, 10000000); eq(Progression.XpToNextLevel(s), 0);
});
test("OnLevelUp_FiresWithNewLevel", () => {
  const s = Fresh(); let reported = -1; const h = (sheet, lvl) => reported = lvl;
  Progression.onLevelUp(h); try { Progression.AwardExperience(s, 300); } finally { Progression.offLevelUp(h); }
  eq(reported, 2);
});
test("LevelUp_NullClass_IncrementsLevel_WithoutHpChange", () => {
  const s = Fresh(); const hp = s.maxHitPoints; Progression.LevelUp(s); eq(s.level, 2); eq(s.maxHitPoints, hp, "no classDef => no HP gain");
});

console.log(`\n  Leveling — Progression (ported from ProgressionTests):`);
console.log(`  ${pass} passed, ${fail} failed`);
if (fail) { fails.forEach(f => console.log("   ✗ " + f)); process.exit(1); }
else console.log("  ✓ XP table, level-ups, the cap, and the event all match Unity.\n");
