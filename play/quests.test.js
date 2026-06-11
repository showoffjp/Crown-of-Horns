// Quest state-machine tests: mirrors QuestManagerTests.cs, then pins the LIVE
// flag-reactive advancement (objective / completion / failure precedence / isolation)
// that the Unity side only covers in PlayMode — runnable headless in CI.
const { GameFlags, QuestStatus, Quest, Objective, QuestManager } = require("./quests.js");

let pass = 0, fail = 0; const fails = [];
function test(name, fn) { try { fn(); pass++; } catch (e) { fail++; fails.push(`${name}: ${e.message}`); } }
const eq = (a, b, m) => { if (a !== b) throw new Error(`${m || ""} expected ${b}, got ${a}`); };
const T = (c, m) => { if (!c) throw new Error(m || "expected true"); };
const Fa = (c, m) => { if (c) throw new Error(m || "expected false"); };
const mgr = (...qs) => new QuestManager(new GameFlags(), qs);

// ---------- mirror of QuestManagerTests.cs (EditMode contract) ----------
test("StatusOf_Unknown_IsUnstarted", () => eq(mgr().StatusOf("nope"), QuestStatus.Unstarted));
test("StartQuest_ActivatesAndFiresEvent", () => {
  const m = mgr(Quest({ questId: "q1" })); let fired = false; m.onQuestStarted.push(() => fired = true);
  m.StartQuest("q1"); eq(m.StatusOf("q1"), QuestStatus.Active); T(fired);
});
test("StartQuest_UnknownId_IsNoOp", () => {
  const m = mgr(); let fired = false; m.onQuestStarted.push(() => fired = true); m.StartQuest("ghost"); Fa(fired);
});
test("StartQuest_Twice_FiresOnlyOnce", () => {
  const m = mgr(Quest({ questId: "q1" })); let n = 0; m.onQuestStarted.push(() => n++);
  m.StartQuest("q1"); m.StartQuest("q1"); eq(n, 1); eq(m.StatusOf("q1"), QuestStatus.Active);
});
test("ExportState_IsACopy_NotALiveView", () => {
  const m = mgr(Quest({ questId: "q1" })); m.StartQuest("q1");
  const snap = m.ExportState(); eq(snap.q1, QuestStatus.Active);
  snap.q1 = QuestStatus.Completed; eq(m.StatusOf("q1"), QuestStatus.Active, "export must detach");
});
test("ImportState_ReplacesStatus", () => {
  const m = mgr(Quest({ questId: "q1" })); m.ImportState({ q1: QuestStatus.Completed }); eq(m.StatusOf("q1"), QuestStatus.Completed);
});
test("ImportState_Null_ClearsState", () => {
  const m = mgr(Quest({ questId: "q1" })); m.StartQuest("q1"); m.ImportState(null); eq(m.StatusOf("q1"), QuestStatus.Unstarted);
});

// ---------- the live reactive logic (PlayMode-only on the Unity side) ----------
test("Objective_CompletesWhenItsFlagFlips", () => {
  const q = Quest({ questId: "q1", objectives: [Objective({ objectiveId: "o1", completionFlag: "met.herald" })] });
  const m = mgr(q); let hit = null; m.onObjectiveCompleted.push((qq, o) => hit = o.objectiveId);
  m.StartQuest("q1");
  m.flags.SetBool("met.herald", true);
  eq(hit, "o1");
});
test("Objective_DoesNotFireOnFalse_OrBeforeStart", () => {
  const q = Quest({ questId: "q1", objectives: [Objective({ objectiveId: "o1", completionFlag: "f" })] });
  const m = mgr(q); let n = 0; m.onObjectiveCompleted.push(() => n++);
  m.flags.SetBool("f", true);   // before StartQuest → ignored (Unstarted)
  eq(n, 0, "no reaction before start");
  m.StartQuest("q1");
  m.flags.SetBool("f", false);  // flag false → no completion
  eq(n, 0, "false flag doesn't complete");
  m.flags.SetBool("f", true);
  eq(n, 1);
});
test("CompletionFlag_MarksCompleted_AndFiresEvent", () => {
  const m = mgr(Quest({ questId: "q1", completionFlag: "done" })); let done = false; m.onQuestCompleted.push(() => done = true);
  m.StartQuest("q1"); m.flags.SetBool("done", true);
  eq(m.StatusOf("q1"), QuestStatus.Completed); T(done);
});
test("FailureFlag_MarksFailed", () => {
  const m = mgr(Quest({ questId: "q1", failureFlag: "blown" }));
  m.StartQuest("q1"); m.flags.SetBool("blown", true);
  eq(m.StatusOf("q1"), QuestStatus.Failed);
});
test("Failure_TakesPrecedenceOverCompletion", () => {
  // both flags true: the C# checks failure first and `continue`s, so failure wins.
  const m = mgr(Quest({ questId: "q1", completionFlag: "done", failureFlag: "blown" }));
  let completed = false; m.onQuestCompleted.push(() => completed = true);
  m.StartQuest("q1");
  m.flags.SetBool("done", true);   // sets done... but not failed yet → would complete
  // reset and do the both-at-once ordering: start fresh
  const m2 = mgr(Quest({ questId: "q1", completionFlag: "done", failureFlag: "blown" }));
  let completed2 = false; m2.onQuestCompleted.push(() => completed2 = true);
  m2.StartQuest("q1");
  m2.flags.bools["done"] = true;   // pre-set done quietly
  m2.flags.SetBool("blown", true); // now a single flag-change sees BOTH true → failure wins
  eq(m2.StatusOf("q1"), QuestStatus.Failed, "failure beats completion when both true");
  Fa(completed2, "completion event must not fire when failed");
});
test("OnlyActiveQuests_React", () => {
  const m = mgr(Quest({ questId: "q1", completionFlag: "done" }));
  m.flags.SetBool("done", true);                 // Unstarted → ignored
  eq(m.StatusOf("q1"), QuestStatus.Unstarted);
  m.StartQuest("q1"); m.flags.SetBool("done", true);
  eq(m.StatusOf("q1"), QuestStatus.Completed);
  let recompleted = 0; m.onQuestCompleted.push(() => recompleted++);
  m.flags.SetBool("done", true);                 // already Completed → no re-fire
  eq(recompleted, 0);
});
test("Quests_AreIsolated", () => {
  const m = mgr(Quest({ questId: "a", completionFlag: "fa" }), Quest({ questId: "b", completionFlag: "fb" }));
  m.StartQuest("a"); m.StartQuest("b");
  m.flags.SetBool("fa", true);
  eq(m.StatusOf("a"), QuestStatus.Completed);
  eq(m.StatusOf("b"), QuestStatus.Active, "b's flag never flipped");
});
test("ExportImport_RoundTrip_AcrossManagers", () => {
  // the save-system path: export from one manager, import into a fresh one.
  const m1 = mgr(Quest({ questId: "a", completionFlag: "fa" }), Quest({ questId: "b" }));
  m1.StartQuest("a"); m1.StartQuest("b"); m1.flags.SetBool("fa", true); // a Completed, b Active
  const saved = m1.ExportState();
  const m2 = mgr(Quest({ questId: "a" }), Quest({ questId: "b" }));
  m2.ImportState(saved);
  eq(m2.StatusOf("a"), QuestStatus.Completed);
  eq(m2.StatusOf("b"), QuestStatus.Active);
});

console.log(`\n  Quest state machine — explicit start, live flag-reactive advancement, save hooks:`);
console.log(`  ${pass} passed, ${fail} failed`);
if (fail) { fails.forEach(f => console.log("   ✗ " + f)); process.exit(1); }
else console.log("  ✓ Objectives, completion, failure-precedence, isolation, and export/import all hold.\n");
