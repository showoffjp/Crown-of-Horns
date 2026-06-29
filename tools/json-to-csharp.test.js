// The C#-emitter gate. There is no C# compiler in this environment, so this verifies the emitted
// code as hard as possible WITHOUT one: string-literal escaping round-trips, the emitted source is
// brace/bracket-balanced once string contents are masked out (which also proves no literal is left
// unterminated), every node reference in the NEW content resolves to a real node (the bug class that
// would dead-end a conversation at runtime — not even a compile error), every enum value is real, and
// the dedup against the existing C# build is correct (no conversation is emitted twice).
"use strict";
const fs = require("fs");
const path = require("path");
const E = require("./json-to-csharp.js");
const U = require("./json-to-unity.js");

let pass = 0, fail = 0; const fails = [];
function check(name, cond) { if (cond) pass++; else { fail++; fails.push(name); } }

// ---- C# string-literal escaping round-trips on the nasty cases ----
for (const s of [`he said "stop"`, `a\\b backslash`, "line\nbreak", "tab\tend", "Kelemvor's doom — '…'", "*emphasis* and ‘curly'", ""]) {
  const lit = E.csharpString(s);
  check("round-trip: " + JSON.stringify(s).slice(0, 30), lit.startsWith("\"") && lit.endsWith("\"") && E.unescapeCsharpString(lit) === s);
}
check("inner quotes are backslash-escaped, not raw", E.csharpString(`say "hi"`) === `"say \\"hi\\""`);
check("backslash is doubled", E.csharpString("a\\b") === `"a\\\\b"`);

// ---- END sentinel maps to 'end conversation' (empty), not a dangling ref ----
check("END (any case) is recognised as the end sentinel", E.isEndSentinel("END") && E.isEndSentinel("end") && !E.isEndSentinel("ending"));
check("endRef blanks the sentinel, passes real ids through", E.endRef("END") === "" && E.endRef("n3") === "n3");
check("a choice pointing at END omits nextNodeId (so C# ends the conversation)", !E.emitChoice({ text: "bye", nextNodeId: "END" }).includes("nextNodeId"));

// ---- clause / choice / node emission shape ----
check("FlagClause emits key+op, and amount only for int ops", (() => {
  const a = E.emitClause({ key: "k", op: "SetTrue" });
  const b = E.emitClause({ key: "d", op: "AddInt", amount: 5 });
  const c = E.emitClause({ key: "i", op: "RequireIntAtLeast", amount: 14 });
  return a === `new FlagClause { key = "k", op = FlagOp.SetTrue }` && b.includes("amount = 5") && c.includes("amount = 14") && !a.includes("amount");
})());
check("a check choice emits Ability enum + DC + failNodeId", (() => {
  const s = E.emitChoice({ text: "x", nextNodeId: "win", checkAbility: "Wisdom", checkDC: 16, failNodeId: "lose" });
  return s.includes("checkAbility = Ability.Wisdom") && s.includes("checkDC = 16") && s.includes(`failNodeId = "lose"`);
})());
check("crit/fumble route nodes emit only inside a real check", (() => {
  const withCheck = E.emitChoice({ text: "x", nextNodeId: "ok", checkAbility: "Charisma", checkDC: 14, critNodeId: "great", fumbleNodeId: "oops" });
  const noCheck = E.emitChoice({ text: "y", nextNodeId: "ok", critNodeId: "great" });
  return withCheck.includes(`critNodeId = "great"`) && withCheck.includes(`fumbleNodeId = "oops"`) && !noCheck.includes("critNodeId");
})());
check("a variant emits when-conditions + text; the default variant omits when", (() => {
  const cond = E.emitVariant({ when: [{ key: "k", op: "RequireBoolTrue" }], text: "reactive" });
  const def = E.emitVariant({ text: "default" });
  return cond.includes("when = new[]") && cond.includes(`text = "reactive"`) && !def.includes("when") && def === `new DialogueVariant { text = "default" }`;
})());
check("a node with variants emits the variants array", E.emitNode({ id: "0", speaker: "X", text: "base", variants: [{ when: [{ key: "k", op: "RequireBoolTrue" }], text: "alt" }], choices: [] }, "").includes("variants = new[] { new DialogueVariant"));
check("a plain choice omits check/conditions/effects entirely", E.emitChoice({ text: "y", nextNodeId: "1" }) === `new DialogueChoice { text = "y", nextNodeId = "1" }`);
check("a node with no choices/auto/onEnter emits just id+speaker+text", E.emitNode({ id: "0", speaker: "X", text: "hi", choices: [] }, "").trim() === `g.nodes.Add(new DialogueNode { id = "0", speaker = "X", text = "hi" });`);
check("className sanitises a filename to a C# identifier", E.className("second_death.json") === "SecondDeathBridgeContent" && /^[A-Za-z]/.test(E.className("123.json")));

// ---- dedup against the real C# build ----
const csIds = E.csharpConversationIds();
check("the C# build's conversationIds are discoverable (200+)", csIds.size >= 200);
const { byFile, skipped } = E.graphsByFile(csIds);
const emitted = [...byFile.values()].flat();
check("dedup actually skips the already-built conversations", skipped >= 150);
check("no emitted graph collides with an existing C# conversationId", emitted.every(g => !csIds.has(g.conversationId)));
check("the new content is a substantial body (200+ graphs)", emitted.length >= 200);
check("the session's marquee content is in the emit set", (() => {
  const ids = new Set(emitted.map(g => g.conversationId));
  return ids.has("sx.sahl") && [...ids].some(i => i.startsWith("cal.")) && [...ids].some(i => i.startsWith("lf."));
})());

// ---- reference integrity across ALL emitted (NEW) content (incl. crit/fumble routing) ----
const badRefs = [];
let critCount = 0, fumbleCount = 0, variantCount = 0;
for (const g of emitted) {
  const ids = new Set(g.nodes.map(n => n.id));
  if (g.startNodeId && !ids.has(g.startNodeId)) badRefs.push(`${g.conversationId} start->${g.startNodeId}`);
  for (const n of g.nodes) {
    variantCount += (n.variants || []).length;
    const refs = [];
    if (E.endRef(n.autoNextNodeId)) refs.push(E.endRef(n.autoNextNodeId));
    for (const c of n.choices) {
      if (E.endRef(c.nextNodeId)) refs.push(E.endRef(c.nextNodeId));
      if (c.checkDC > 0) {
        if (E.endRef(c.failNodeId)) refs.push(E.endRef(c.failNodeId));
        if (E.endRef(c.critNodeId)) { refs.push(E.endRef(c.critNodeId)); critCount++; }
        if (E.endRef(c.fumbleNodeId)) { refs.push(E.endRef(c.fumbleNodeId)); fumbleCount++; }
      }
    }
    for (const r of refs) if (!ids.has(r)) badRefs.push(`${g.conversationId}[${n.id}]->${r}`);
  }
}
check("every node reference in the new content resolves — incl. crit/fumble routing (no dead-ends): " + (badRefs.slice(0, 5).join(", ") || "all resolve"), badRefs.length === 0);
check("the new engine features are actually emitted (crit + fumble + variants, in quantity)", critCount >= 80 && fumbleCount >= 80 && variantCount >= 200);

// ---- emit a real file and verify it is brace/bracket-balanced with strings masked ----
const sampleFile = [...byFile.keys()].find(f => f.includes("seconddeath")) || [...byFile.keys()][0];
const src = E.emitFile(sampleFile, byFile.get(sampleFile));
check("emitted file carries the auto-generated header and the right usings", src.includes("<auto-generated>") && src.includes("using SunderedCrown.Dialogue;") && src.includes("using SunderedCrown.Stats;"));
check("emitted file declares a static Build() returning the graph list", /public static List<DialogueGraph> Build\(\)/.test(src) && src.includes("return graphs;"));

// Mask C# string literals to "" so brace counting can't be fooled by braces inside dialogue text;
// a parse error here also means a literal was left unterminated.
function maskStrings(code) {
  let out = "", i = 0;
  while (i < code.length) {
    const ch = code[i];
    if (ch === "\"") {
      i++;
      while (i < code.length) {
        if (code[i] === "\\") { i += 2; continue; }
        if (code[i] === "\"") { i++; break; }
        if (code[i] === "\n") return null; // unterminated literal on this line
        i++;
      }
      out += '""';
    } else { out += ch; i++; }
  }
  return out;
}
const masked = maskStrings(src);
check("no string literal is left unterminated", masked !== null);
function balanced(code, open, close) {
  let d = 0;
  for (const ch of code) { if (ch === open) d++; else if (ch === close) { d--; if (d < 0) return false; } }
  return d === 0;
}
check("braces balance with string contents masked", masked && balanced(masked, "{", "}"));
check("brackets balance with string contents masked", masked && balanced(masked, "[", "]"));
check("parens balance with string contents masked", masked && balanced(masked, "(", ")"));

// ---- enum validity over everything emitted ----
const ABIL = U.ABILITIES;
let badEnum = 0;
for (const g of emitted) for (const n of g.nodes) {
  for (const cl of (n.onEnter || [])) if (!E.FLAG_OPS.has(cl.op)) badEnum++;
  for (const v of (n.variants || [])) for (const cl of (v.when || [])) if (!E.FLAG_OPS.has(cl.op)) badEnum++;
  for (const c of n.choices) {
    for (const cl of (c.conditions || []).concat(c.effects || [])) if (!E.FLAG_OPS.has(cl.op)) badEnum++;
    if (c.checkDC > 0 && !ABIL.has(c.checkAbility)) badEnum++;
  }
}
check("every emitted FlagOp and Ability is a real enum value (incl. variant when-clauses)", badEnum === 0);

console.log(`\n  C# content emitter — ${emitted.length} NEW DialogueGraphs from ${byFile.size} zones, ${skipped} dedup-skipped:`);
console.log(`  ${pass} passed, ${fail} failed`);
if (fail) { fails.forEach(f => console.log("   ✗ " + f)); process.exit(1); }
else console.log("  ✓ emitted C# round-trips, balances, dedups, and every branch resolves — verified without a compiler.\n");
