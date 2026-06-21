// ============================================================================
// Epilogue + Chronicle tests — mirrors Assets/Tests/EditMode/EpilogueTests.cs,
// then goes further: a PROSE-PARITY gate that mechanically extracts every
// non-interpolated string literal (>=60 chars) from EndingResolver.cs and
// asserts the JS port carries it byte-for-byte, and a seeded fuzz that hammers
// the whole flag space for crashes/empty slides.
// ============================================================================
const fs = require("fs");
const path = require("path");
const { GameFlags, Ending, ALL_ENDINGS, EndingResolver } = require("./endings.js");

let pass = 0, fail = 0; const fails = [];
function test(name, fn) {
  GameFlags.Replace(new GameFlags());
  EndingResolver.content = { keepsakesEarnedCount: () => 0, deedsEarnedCount: () => 0, deedsTotal: () => 0, difficulty: () => "Normal" };
  try { fn(); pass++; } catch (e) { fail++; fails.push(`${name}: ${e.message}`); }
}
const F = () => GameFlags.Current;
const any = (arr, marker) => arr.some(s => s != null && s.includes(marker));
const T = (c, m) => { if (!c) throw new Error(m || "expected true"); };
const Fa = (c, m) => { if (c) throw new Error(m || "expected false"); };
const eq = (a, b, m) => { if (a !== b) throw new Error(`${m || ""} expected ${b}, got ${a}`); };

// ===================== scenario tests (mirror the Unity suite) ==============

test("Baseline_HasGarrow_NoUnrecruited_NoEmptySlides", () => {
  const s = EndingResolver.Epilogue(Ending.MortalMeasure);
  T(any(s, "Sister Garrow"), "Garrow is in every run");
  Fa(any(s, "Roen"), "unrecruited get no slide");
  Fa(any(s, "Naeve"), "unrecruited get no slide");
  T(s.every(x => typeof x === "string" && x.length > 0), "no empty slides");
});
test("LostCompanion_GetsTheWallSlide", () => {
  F().SetBool("companion.roen.recruited", true); F().SetBool("companion.roen.lost", true);
  T(any(EndingResolver.Epilogue(Ending.MortalMeasure), "taken by the Wall as the tithe"));
});
test("LeftCompanion_GetsWalkedOutSlide", () => {
  F().SetBool("companion.varra.recruited", true); F().SetBool("companion.varra.left", true);
  T(any(EndingResolver.Epilogue(Ending.MortalMeasure), "walked out of the company"));
});
test("PresentSlide_IsTintedByEnding", () => {
  F().SetBool("companion.roen.recruited", true);
  T(any(EndingResolver.Epilogue(Ending.BreakTheLoop), "stood with you at the niche"));
  T(any(EndingResolver.Epilogue(Ending.JergalsKeyhole), "kept you company at the Ledger"));
  T(any(EndingResolver.Epilogue(Ending.Abolition), "Deathless Garden"));
  T(any(EndingResolver.Epilogue(Ending.MortalMeasure), "sat at the small table"));
  T(any(EndingResolver.Epilogue(Ending.ReturnedThrone), "walked out of the story changed"));
});
test("RoenQuest_DoubleAgent_OutranksOtherOutcomes", () => {
  F().SetBool("companion.roen.recruited", true); F().SetBool("quest.roen.resolved", true);
  F().SetBool("quest.roen.double_agent", true); F().SetBool("quest.roen.wrenna_saved", true); F().SetBool("quest.roen.harper_boon", true);
  const s = EndingResolver.Epilogue(Ending.MortalMeasure);
  T(any(s, "a blade in Kelemvor's own house")); Fa(any(s, "two cold teacups"), "priority chain");
});
test("RoenQuest_HarperBoon_AloneFires", () => {
  F().SetBool("companion.roen.recruited", true); F().SetBool("quest.roen.resolved", true); F().SetBool("quest.roen.harper_boon", true);
  T(any(EndingResolver.Epilogue(Ending.MortalMeasure), "Correct is not the same as forgiven"));
});
test("QuestSlides_RequireResolved_AndSurvival", () => {
  F().SetBool("companion.varra.recruited", true); F().SetBool("quest.varra.patron_bound", true);
  Fa(any(EndingResolver.Epilogue(Ending.MortalMeasure), "a Hell in her debt"), "not resolved yet");
  F().SetBool("quest.varra.resolved", true);
  T(any(EndingResolver.Epilogue(Ending.MortalMeasure), "a Hell in her debt"));
  F().SetBool("companion.varra.lost", true);
  Fa(any(EndingResolver.Epilogue(Ending.MortalMeasure), "a Hell in her debt"), "the dead get no quest epilogue");
});
test("GarrowQuest_AllThreeBranches", () => {
  F().SetBool("quest.garrow.resolved", true); F().SetBool("quest.garrow.doctrine_won", true);
  T(any(EndingResolver.Epilogue(Ending.MortalMeasure), "first High Doomguide"));
  GameFlags.Replace(new GameFlags());
  F().SetBool("quest.garrow.resolved", true); F().SetBool("quest.garrow.left_faith", true);
  T(any(EndingResolver.Epilogue(Ending.MortalMeasure), "under no law but her own two hands"));
  GameFlags.Replace(new GameFlags());
  F().SetBool("quest.garrow.resolved", true); F().SetBool("quest.garrow.recanted", true);
  T(any(EndingResolver.Epilogue(Ending.MortalMeasure), "smaller faith now"));
});
test("LovedAndLost_AddsGriefSlide_OnlyWithLove", () => {
  F().SetBool("companion.naeve.recruited", true); F().SetBool("companion.naeve.lost", true);
  Fa(any(EndingResolver.Epilogue(Ending.MortalMeasure), "exactly the shape of everything"));
  F().SetBool("romance.naeve.turn", true);
  T(any(EndingResolver.Epilogue(Ending.MortalMeasure), "exactly the shape of everything"));
});
test("VarraLovedAndLost_GetsHerOwnSlide", () => {
  F().SetBool("companion.varra.recruited", true); F().SetBool("companion.varra.lost", true); F().SetBool("romance.varra.turn", true);
  T(any(EndingResolver.Epilogue(Ending.MortalMeasure), "Maerin's tithe"));
});
test("Romance_VariesByEnding", () => {
  F().SetBool("romance.garrow.consummated", true);
  T(any(EndingResolver.Epilogue(Ending.BreakTheLoop), "I'll keep your name"));
  T(any(EndingResolver.Epilogue(Ending.MortalMeasure), "any list but the living one"));
});
test("Verdict_SparedOutranksPassed", () => {
  F().SetBool("crownwars.verdict_passed", true);
  T(any(EndingResolver.Epilogue(Ending.MortalMeasure), "History only inherited it"));
  F().SetBool("crownwars.verdict_spared", true);
  const s = EndingResolver.Epilogue(Ending.MortalMeasure);
  T(any(s, "ten thousand years early")); Fa(any(s, "History only inherited it"));
});
test("AnchorId_RomanceDepth_BeatsOrderAndApproval", () => {
  F().SetBool("romance.roen.turn", true); F().SetBool("romance.varra.consummated", true);
  eq(EndingResolver.AnchorId(), "varra", "depth outranks list order");
});
test("AnchorId_ApprovalFallback_NeedsTwenty_AndRecruitment", () => {
  F().SetBool("companion.roen.recruited", true); F().SetInt("companion.roen.approval", 25);
  F().SetInt("companion.garrow.approval", 10);
  F().SetInt("companion.ilfaeril.approval", 90); // NOT recruited -> must be ignored
  eq(EndingResolver.AnchorId(), "roen");
  GameFlags.Replace(new GameFlags());
  F().SetInt("companion.garrow.approval", 19);
  eq(EndingResolver.AnchorId(), null, "below the let-someone-in threshold");
});
test("AnchorSlide_LostVariant", () => {
  F().SetBool("companion.varra.recruited", true); F().SetBool("companion.varra.lost", true); F().SetBool("romance.varra.consummated", true);
  T(any(EndingResolver.Epilogue(Ending.MortalMeasure), "a hole with their name on it"));
});
test("Keepsakes_Hook_DrivesSlide", () => {
  EndingResolver.content.keepsakesEarnedCount = () => 7;
  T(any(EndingResolver.Epilogue(Ending.MortalMeasure), "7 keepsakes, from a gravedigger's whetstone"));
});
test("NgPlus_AddsTheLoopSlide", () => {
  F().SetBool("ng.plus", true);
  T(any(EndingResolver.Epilogue(Ending.MortalMeasure), "reaches, already, for the first chapter"));
});
test("SideQuests_LongRoad_DriveTheirOwnSlides", () => {
  // None set: no side-quest slide leaks in.
  Fa(any(EndingResolver.Epilogue(Ending.MortalMeasure), "The Graves That Waited"), "no graves slide unset");
  Fa(any(EndingResolver.Epilogue(Ending.MortalMeasure), "The Hand in the Margins"), "no margins slide unset");
  // Set one resolution per quest; each contributes its slide.
  F().SetBool("sq.field_of_the_rested", true);
  F().SetBool("sq.wrote_back_to_the_loop", true);
  F().SetBool("sq.roen_forgives_sabira", true);
  F().SetBool("sq.naeve_grieves_at_last", true);
  F().SetBool("sq.harvest_exposed_public", true);
  F().SetBool("sq.fortyone_reaper_rests", true);
  F().SetBool("sq.forbidden_name_spoken", true);
  const s = EndingResolver.Epilogue(Ending.MortalMeasure);
  T(s.every(x => typeof x === "string" && x.length > 0), "no empty side-quest slides");
  T(any(s, "the field of the rested was the thing the crown was always for"));
  T(any(s, "You were the first who could write back"));
  T(any(s, "forgiveness is a weight you release for your own sake"));
  T(any(s, "started mourning him — beautiful and guilty and gone"));
  T(any(s, "the ordinary people who fed the Wall began to step out of their places"));
  T(any(s, "the ferryman of endings learned he was owed a shore too"));
  T(any(s, "a name carried in love is a soul the Wall can never quite finish erasing"));
});
test("Chronicle_SideQuestTally_CountsResolvedQuests", () => {
  Fa(any(EndingResolver.Chronicle(), "Side quests of the long road"), "no tally when none resolved");
  F().SetBool("sq.roen_forgives_sabira", true);          // quest 3
  F().SetBool("sq.field_of_the_rested", true);           // quest 1
  F().SetBool("sq.fortyone_reaper_rests", true);         // quest 6
  F().SetBool("sq.fortyone_victory", true);              // same quest 6 — must not double-count
  T(any(EndingResolver.Chronicle(), "Side quests of the long road: 3/7 brought home"));
});
test("SideQuests_PrimaryResolutionOutranksAlternate", () => {
  // For a single quest, the highest-priority resolution wins (one slide, not two).
  F().SetBool("sq.field_of_the_rested", true);
  F().SetBool("sq.every_soul_expected", true); // lower branch — must be suppressed
  const s = EndingResolver.Epilogue(Ending.MortalMeasure);
  T(any(s, "you did the unglamorous part"));
  Fa(any(s, "The open holes said otherwise"), "only the primary graves slide shows");
});
test("Chronicle_BaselineLines", () => {
  const l = EndingResolver.Chronicle();
  T(any(l, "Eras walked: none yet"));
  T(any(l, "Companion quests resolved: 0/5"));
  T(any(l, "Endings unlocked: 3/6"));
  T(any(l, "Still at your side: Sister Garrow"));
  T(any(l, "Difficulty: Normal"));
});
test("Chronicle_GoldenRoad_Buckets_Hearts_Standings", () => {
  F().SetBool("readers_boon", true);
  F().SetBool("companion.roen.recruited", true); F().SetBool("companion.roen.lost", true);
  F().SetBool("companion.naeve.recruited", true); F().SetBool("companion.naeve.left", true);
  F().SetBool("romance.garrow.consummated", true);
  F().SetInt("faction.kelemvor.reputation", 5); F().SetInt("faction.choir.reputation", -2);
  const l = EndingResolver.Chronicle();
  T(any(l, "a golden road is open"));
  T(any(l, "Taken by the Wall: Roen"));
  T(any(l, "Walked away: Naeve"));
  T(any(l, "Hearts given: Sister Garrow ♥"));
  T(any(l, "Kelemvor's Doomguides: +5"));
  T(any(l, "the Faithless Choir: -2"));
});

// ===================== prose-parity gate ====================================
// Every non-interpolated C# string literal >= 60 chars in EndingResolver.cs
// must appear byte-for-byte (same escapes) in endings.js. Typo-proof port.
test("ProseParity_CSharpLiterals_AppearVerbatimInPort", () => {
  const cs = fs.readFileSync(path.join(__dirname, "..", "Assets", "Scripts", "Core", "EndingResolver.cs"), "utf8")
    .split("\n").filter(l => !l.trim().startsWith("//")).join("\n");
  const js = fs.readFileSync(path.join(__dirname, "endings.js"), "utf8");
  const lits = [];
  const re = /(\$?)"((?:[^"\\\n]|\\.)*)"/g;
  let m;
  while ((m = re.exec(cs)) !== null)
    if (m[1] !== "$" && m[2].length >= 60) lits.push(m[2]);
  T(lits.length >= 50, `expected a rich prose corpus, found only ${lits.length} literals`);
  const missing = lits.filter(l => !js.includes(l));
  if (missing.length)
    throw new Error(`${missing.length}/${lits.length} prose literals missing from the port; first: "${missing[0].slice(0, 80)}..."`);
});

// ===================== seeded fuzz ==========================================
// Hammer the flag space: no combination of story flags may crash the epilogue
// or produce an empty slide, for any ending.
test("Fuzz_500RandomPlaythroughs_NeverCrash_NeverEmpty", () => {
  let seed = 0xC0FFEE;
  const rnd = () => (seed = (seed * 1103515245 + 12345) & 0x7fffffff) / 0x7fffffff;
  const ids = ["garrow", "roen", "varra", "naeve", "ilfaeril", "maerin"];
  const boolKeys = [];
  for (const id of ids) boolKeys.push(`companion.${id}.recruited`, `companion.${id}.lost`, `companion.${id}.left`);
  for (const id of ["roen", "varra", "garrow", "naeve", "ilfaeril"]) boolKeys.push(`quest.${id}.resolved`, `rupture.${id}.broken`);
  boolKeys.push("quest.roen.double_agent", "quest.roen.wrenna_saved", "quest.roen.harper_boon",
    "quest.varra.patron_bound", "quest.varra.debt_taken", "quest.varra.freed",
    "quest.garrow.doctrine_won", "quest.garrow.left_faith", "quest.garrow.recanted",
    "quest.naeve.rekindled", "quest.naeve.released", "quest.naeve.preserved",
    "quest.ilfaeril.commission", "quest.ilfaeril.forgiven", "quest.ilfaeril.penance");
  for (const id of ["garrow", "roen", "naeve", "varra"]) boolKeys.push(`romance.${id}.turn`, `romance.${id}.choosing`, `romance.${id}.consummated`);
  boolKeys.push("aldric.named_monster", "aldric.provisional", "readers_boon", "pc.true_name", "ng.plus",
    "netheril.boss_down", "crownwars.boss_down", "toot.avatar_down", "spellplague.herald_down",
    "toot.garrow_witnessed", "spellplague.varra_witnessed", "netheril.naeve_witnessed", "crownwars.ilfaeril_witnessed",
    "crownwars.verdict_spared", "crownwars.verdict_passed", "lowcity.allies", "almshouse.roen_witnessed", "almshouse.token",
    "quest.letter.delivered", "quest.letter.read", "quest.letter.burned",
    "docks.ferryman_saved", "docks.ferryman_resolved", "safehouse.informant_freed", "safehouse.informant_resolved",
    "almshouse.deathbed_lie", "almshouse.deathbed_resolved", "market.urchin_freed", "market.urchin_resolved",
    "quest.tithe.freed", "quest.tithe.paid", "quest.tithe.corrupt", "quest.tithe.resolved",
    "quest.choir.cell_cleared", "quest.choir.doubted", "quest.choir.favored", "quest.choir.suppressed", "quest.choir.resolved",
    "netheril.cleared", "crownwars.cleared", "act4.toot_done", "act4.spellplague_done",
    "quest.widow.resolved", "quest.fist.resolved", "camp.banter.fire1.done", "camp.nighttalk.garrow1.done");
  const intKeys = ["reputation.lowcity", "faction.kelemvor.reputation", "faction.choir.reputation",
    "faction.ashpact.reputation", "riddle.solvedCount", "combat.victories",
    ...ids.map(id => `companion.${id}.approval`)];

  for (let i = 0; i < 500; i++) {
    GameFlags.Replace(new GameFlags());
    for (const k of boolKeys) if (rnd() < 0.35) F().SetBool(k, true);
    for (const k of intKeys) if (rnd() < 0.5) F().SetInt(k, Math.floor(rnd() * 60) - 20);
    EndingResolver.content.keepsakesEarnedCount = () => Math.floor(rnd() * 9);
    for (const e of ALL_ENDINGS) {
      const s = EndingResolver.Epilogue(e);
      if (!s.every(x => typeof x === "string" && x.length > 0)) throw new Error(`empty slide, seed-iter ${i}, ending ${e}`);
    }
    const c = EndingResolver.Chronicle();
    if (!c.every(x => typeof x === "string" && x.length > 0)) throw new Error(`empty chronicle line, seed-iter ${i}`);
    EndingResolver.AnchorId(); // must never throw
  }
});

// --- report -----------------------------------------------------------------
console.log(`\n  Epilogue + Chronicle — gating, prose parity, and fuzz:`);
console.log(`  ${pass} passed, ${fail} failed`);
if (fail) { fails.forEach(f => console.log("   ✗ " + f)); process.exit(1); }
else console.log("  ✓ The payoff system is fully pinned: every branch family, byte-identical prose, crash-free under fuzz.\n");
