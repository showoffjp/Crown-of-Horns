// EndingResolverTests, ported 1:1 and run against the JS narrative engine.
const { GameFlags, Ending, ALL_ENDINGS, EndingResolver } = require("./endings.js");

let pass = 0, fail = 0; const fails = [];
function test(name, fn) { GameFlags.Replace(new GameFlags()); try { fn(); pass++; } catch (e) { fail++; fails.push(`${name}: ${e.message}`); } }
const has = (arr, x) => { if (!arr.includes(x)) throw new Error(`expected ${x} in [${arr}]`); };
const hasnt = (arr, x) => { if (arr.includes(x)) throw new Error(`did not expect ${x} in [${arr}]`); };
const equiv = (a, b) => { const s=x=>[...x].sort().join(","); if (s(a)!==s(b)) throw new Error(`[${a}] != [${b}]`); };
const T = (c, m) => { if (!c) throw new Error(m || "expected true"); };
const Fa = (c, m) => { if (c) throw new Error(m || "expected false"); };

test("Available_Baseline_OffersTheThreeCommonEndings", () => {
  const a = EndingResolver.Available();
  equiv(a, [Ending.Abolition, Ending.ReturnedThrone, Ending.MortalMeasure]);
  hasnt(a, Ending.JergalsKeyhole); hasnt(a, Ending.BreakTheLoop);
});
test("DoomguidesPeace_UnlocksWithKelemvorReputation", () => {
  GameFlags.Current.SetInt("faction.kelemvor.reputation", 5);
  has(EndingResolver.Available(), Ending.DoomguidesPeace);
});
test("DoomguidesPeace_UnlocksByNamingTheMonster", () => {
  GameFlags.Current.SetBool("aldric.named_monster", true);
  has(EndingResolver.Available(), Ending.DoomguidesPeace);
});
test("JergalsKeyhole_NeedsBothUnderstandingAndASparedVerdict", () => {
  GameFlags.Current.SetBool("readers_boon", true);
  hasnt(EndingResolver.Available(), Ending.JergalsKeyhole);
  GameFlags.Current.SetBool("crownwars.verdict_spared", true);
  has(EndingResolver.Available(), Ending.JergalsKeyhole);
  T(EndingResolver.IsGolden(Ending.JergalsKeyhole));
});
test("BreakTheLoop_NeedsEitherUnderstandingOrTrueName", () => {
  GameFlags.Current.SetBool("pc.true_name", true);
  has(EndingResolver.Available(), Ending.BreakTheLoop);
  T(EndingResolver.IsGolden(Ending.BreakTheLoop));
});
test("IsGolden_OnlyForTheTwoDeepestRoads", () => {
  T(EndingResolver.IsGolden(Ending.JergalsKeyhole));
  T(EndingResolver.IsGolden(Ending.BreakTheLoop));
  Fa(EndingResolver.IsGolden(Ending.Abolition));
  Fa(EndingResolver.IsGolden(Ending.DoomguidesPeace));
  Fa(EndingResolver.IsGolden(Ending.ReturnedThrone));
  Fa(EndingResolver.IsGolden(Ending.MortalMeasure));
});
test("EveryEnding_CarriesTitleChoiceAndProse", () => {
  for (const e of ALL_ENDINGS) {
    if (EndingResolver.Title(e) === "—") throw new Error(`${e} has no title`);
    if (EndingResolver.Choice(e) === "—") throw new Error(`${e} has no choice`);
    if (EndingResolver.Prose(e) === "—" || !EndingResolver.Prose(e)) throw new Error(`${e} has no prose`);
  }
});

console.log(`\n  Narrative engine — EndingResolver gating (ported from EndingResolverTests):`);
console.log(`  ${pass} passed, ${fail} failed`);
if (fail) { fails.forEach(f => console.log("   ✗ " + f)); process.exit(1); }
else console.log("  ✓ Choices gate endings exactly as your Unity tests demand.\n");
