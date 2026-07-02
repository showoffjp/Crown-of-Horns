// The bridge gate. Proves json-to-unity.js emits JsonUtility-shaped data whose field names and
// value types match the real C# classes (DialogueGraph / Quest), that the when->FlagClause
// translation is correct, and that every C#-incompatible feature is COUNTED (never silently lost).
"use strict";
const fs = require("fs");
const path = require("path");
const C = require("./json-to-unity.js");

let pass = 0, fail = 0; const fails = [];
function check(name, cond) { if (cond) pass++; else { fail++; fails.push(name); } }

// ---- when -> FlagClause translation ----
const t1 = C.translateWhen({ flags: ["a", "b"], flagsNot: ["c"], flag: "d", int: { "disp.haunted": 14 } });
check("flags -> RequireBoolTrue", t1.conditions.filter(x => x.op === "RequireBoolTrue").map(x => x.key).join() === "a,b,d");
check("flagsNot -> RequireBoolFalse", t1.conditions.some(x => x.op === "RequireBoolFalse" && x.key === "c"));
check("int -> RequireIntAtLeast with amount", t1.conditions.some(x => x.op === "RequireIntAtLeast" && x.key === "disp.haunted" && x.amount === 14));
check("clean when has no untranslated gates", t1.untranslated.length === 0);
const t2 = C.translateWhen({ race: "Elf", deity: "Kelemvor", flags: ["x"] });
check("character-state gates are reported, not faked into conditions", t2.untranslated.length === 2 && t2.conditions.length === 1);
check("empty/missing when is safe", C.translateWhen(null).conditions.length === 0 && C.translateWhen(undefined).untranslated.length === 0);

// ---- choice conversion ----
const gaps0 = mkGaps();
const ch = C.convertChoice({ text: "go", next: "n2", when: { flags: ["seen"] }, effects: [{ key: "did", op: "SetTrue" }], check: { skill: "Persuasion", ability: "Charisma", dc: 14 }, fail: "n3", crit: "win", fumble: "lose", tag: "returned" }, { conv: "c", node: "n1" }, gaps0);
check("choice maps next->nextNodeId, fail->failNodeId", ch.nextNodeId === "n2" && ch.failNodeId === "n3");
check("choice carries effects through unchanged", ch.effects.length === 1 && ch.effects[0].op === "SetTrue");
check("choice when folds into conditions[]", ch.conditions.some(c => c.op === "RequireBoolTrue" && c.key === "seen"));
check("check -> checkAbility (valid enum) + checkDC", ch.checkAbility === "Charisma" && ch.checkDC === 14);
check("crit/fumble/tag/skill-name are recorded as gaps", gaps0.crit.length === 1 && gaps0.fumble.length === 1 && gaps0.choiceTag.length === 1 && gaps0.droppedSkillName.length === 1);
const chBad = C.convertChoice({ text: "x", check: { ability: "Luck", dc: 9 } }, { conv: "c", node: "n" }, gaps0);
check("unknown ability falls back to Charisma and is reported", chBad.checkAbility === "Charisma" && gaps0.unknownAbility.length === 1);
const chPre = C.convertChoice({ text: "y", conditions: [{ key: "k", op: "RequireBoolTrue", amount: 0 }] }, { conv: "c", node: "n" }, gaps0);
check("pre-translated conditions[] pass straight through", chPre.conditions.length === 1 && chPre.conditions[0].key === "k");

// ---- node conversion ----
const gaps1 = mkGaps();
const nd = C.convertNode({ id: "n1", speaker: "X", variants: [{ when: { flags: ["a"] }, text: "A" }, { text: "default" }], onEnter: [{ key: "o", op: "SetTrue" }], effects: [{ key: "e", op: "SetTrue" }], auto: "n2", choices: [] }, "conv", gaps1);
check("variant node takes the unconditional default as its base text", nd.text === "default");
check("variants are counted as a gap", gaps1.variants.length === 1 && gaps1.variants[0].count === 2);
check("onEnter + node effects fold into one onEnter[]", nd.onEnter.length === 2);
check("auto -> autoNextNodeId", nd.autoNextNodeId === "n2");
const gaps2 = mkGaps();
C.convertNode({ id: "b", banter: true, auto: "1" }, "conv", gaps2);
C.convertNode({ id: "d", draw: ["x"], drawMax: 2 }, "conv", gaps2);
check("banter / draw nodes are counted", gaps2.banter.length === 1 && gaps2.draw.length === 1);

// ---- quest conversion matches C# Quest field names ----
const q = C.convertQuest({ questId: "sq.x", title: "T", premise: "P", completionFlag: "cf", failureFlag: "", objectives: [{ objectiveId: "o", desc: "d", completionFlag: "ocf", optional: true }] });
const QF = ["questId", "title", "summary", "completionFlag", "failureFlag", "objectives", "experienceReward", "goldReward"];
check("quest has exactly the C# Quest fields", QF.every(k => k in q) && Object.keys(q).length === QF.length);
check("premise -> summary, default rewards applied", q.summary === "P" && q.experienceReward === 100 && q.goldReward === 0);
const OF = ["objectiveId", "description", "completionFlag", "hidden", "optional"];
check("objective desc->description and has exactly C# QuestObjective fields", q.objectives[0].description === "d" && OF.every(k => k in q.objectives[0]) && Object.keys(q.objectives[0]).length === OF.length);

// ---- full live build over the real prototype ----
const built = C.build();
check("build produces graphs from real content (76+ conversations)", built.graphs.length >= 70);
check("build produces the 20 tracked quests", built.quests.length >= 20);
check("every emitted graph has the C# DialogueGraph fields and a start node that exists", built.graphs.every(g =>
  "conversationId" in g && "startNodeId" in g && Array.isArray(g.nodes) &&
  (g.nodes.length === 0 || g.nodes.some(n => n.id === g.startNodeId))));
check("every emitted node has exactly the C# DialogueNode fields", (() => {
  const NF = ["id", "speaker", "text", "onEnter", "choices", "autoNextNodeId"];
  return built.graphs.every(g => g.nodes.every(n => NF.every(k => k in n) && Object.keys(n).length === NF.length));
})());
check("every emitted choice has exactly the C# DialogueChoice fields", (() => {
  const CF = ["text", "nextNodeId", "conditions", "effects", "checkAbility", "checkDC", "failNodeId"];
  return built.graphs.every(g => g.nodes.every(n => (n.choices || []).every(c => CF.every(k => k in c) && Object.keys(c).length === CF.length)));
})());
check("every condition/effect clause uses a real FlagOp and a non-empty key", (() => {
  const OPS = new Set(["SetTrue", "SetFalse", "AddInt", "RequireBoolTrue", "RequireBoolFalse", "RequireIntAtLeast"]);
  return built.graphs.every(g => g.nodes.every(n =>
    [...n.onEnter, ...n.choices.flatMap(c => [...c.conditions, ...c.effects])].every(cl => cl.key && OPS.has(cl.op))));
})());
check("every checkAbility is a real Stats.Ability enum name", built.graphs.every(g => g.nodes.every(n => n.choices.every(c => C.ABILITIES.has(c.checkAbility)))));
check("the gap report counts the known C#-incompatible features (variants are real and many)", built.report.summary.gapsByFeature.variants >= 200);
check("the report itemises every gap (counts equal listed locations)", Object.keys(built.report.gaps).every(k => built.report.gaps[k].length === built.report.summary.gapsByFeature[k]));

function mkGaps() { return { variants: [], banter: [], draw: [], dynamic: [], crit: [], fumble: [], nonFlagGates: [], unknownAbility: [], droppedSkillName: [], choiceTag: [] }; }

const g = built.report.summary.gapsByFeature;
console.log(`\n  Unity bridge converter — ${built.graphs.length} graphs / ${built.quests.length} quests, every field matched to the real C# model:`);
console.log(`  ${pass} passed, ${fail} failed`);
console.log(`  honest gap list (Unity-side work): variants=${g.variants} crit=${g.crit} fumble=${g.fumble} draw=${g.draw} banter=${g.banter} non-flag-gates=${g.nonFlagGates}`);
if (fail) { fails.forEach(f => console.log("   ✗ " + f)); process.exit(1); }
else console.log("  ✓ the bridge is real and tested; nothing crosses silently.\n");
