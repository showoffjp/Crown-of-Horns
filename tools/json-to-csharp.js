#!/usr/bin/env node
// json-to-csharp.js — emit the prototype conversations as C# content classes that match the real
// Unity authoring pattern (e.g. Assets/Scripts/Content/GarrowQuestContent.cs) byte-for-idiom.
//
// The Unity game authors dialogue as hand-written C# that builds DialogueGraph in code:
//     var g = ScriptableObject.CreateInstance<DialogueGraph>();
//     g.conversationId = "..."; g.startNodeId = "0";
//     g.nodes.Add(new DialogueNode { id = "0", speaker = "...", text = "...",
//         onEnter = new[] { new FlagClause { key = "k", op = FlagOp.SetTrue } },
//         choices = new[] { new DialogueChoice { text = "...", nextNodeId = "1" } } });
//
// So the HONEST bridge target isn't JSON + a loader — it's code that looks exactly like what a human
// wrote, drops straight into SunderedCrown.Content, and compiles into the existing DialogueRunner with
// no asset-import step. This emitter produces that, reusing json-to-unity.js's tested, field-mapped
// normalization (when->FlagClause already done there).
//
// FIDELITY NOTE (honest): this emits the "runs, base-text" version. Variant text, crit/fumble routing,
// draw, and non-flag gates are the engine features the C# runner doesn't have yet (see gap-report);
// each variant node ships its unconditional default as text, so the content RUNS — it just won't react
// on those axes until the runner is extended. Nothing is silently dropped; the gaps are counted.
//
// Run:  node tools/json-to-csharp.js [outDir]
// Test: node tools/json-to-csharp.test.js
"use strict";
const fs = require("fs");
const path = require("path");
const U = require("./json-to-unity.js");

const FLAG_OPS = new Set(["SetTrue", "SetFalse", "AddInt", "RequireBoolTrue", "RequireBoolFalse", "RequireIntAtLeast"]);

// "END" is the prototype's end-of-conversation sentinel, not a node. In C# an empty next/auto ends
// the conversation, so sentinels map to "" (omitted).
const isEndSentinel = (id) => /^END$/i.test(id || "");
const endRef = (id) => (isEndSentinel(id) ? "" : (id || ""));

// The set of conversationIds ALREADY authored in the Unity C# build (dialogue-data.json is extracted
// FROM these). The bridge must emit only NEW content — re-emitting an existing id would duplicate a
// conversation and collide. We discover the set by scanning the real content for `conversationId = "..."`.
function csharpConversationIds(root) {
  const ids = new Set();
  root = root || path.join(__dirname, "..", "Assets", "Scripts");
  const re = /conversationId\s*=\s*"([^"]+)"/g;
  (function walk(dir) {
    let entries; try { entries = fs.readdirSync(dir, { withFileTypes: true }); } catch { return; }
    for (const e of entries) {
      const p = path.join(dir, e.name);
      if (e.isDirectory()) walk(p);
      else if (e.name.endsWith(".cs")) {
        const txt = fs.readFileSync(p, "utf8");
        let m; while ((m = re.exec(txt))) ids.add(m[1]);
      }
    }
  })(root);
  return ids;
}

// --- C# string literal escaping (regular, non-verbatim string) ----------------------------------
// Mirrors how the hand-authored content escapes: inner quotes \", backslashes \\, control chars; the
// abundant unicode (…—'') and apostrophes stay literal in the UTF-8 source, exactly as the humans did.
function csharpString(s) {
  s = s == null ? "" : String(s);
  let out = "";
  for (const ch of s) {
    if (ch === "\\") out += "\\\\";
    else if (ch === "\"") out += "\\\"";
    else if (ch === "\n") out += "\\n";
    else if (ch === "\r") out += "\\r";
    else if (ch === "\t") out += "\\t";
    else out += ch;
  }
  return "\"" + out + "\"";
}

// Reverse of csharpString (drops the wrapping quotes) — used by the test to prove round-trip fidelity.
function unescapeCsharpString(lit) {
  let s = lit;
  if (s.startsWith("\"") && s.endsWith("\"")) s = s.slice(1, -1);
  return s.replace(/\\(.)/g, (_, c) => ({ n: "\n", r: "\r", t: "\t", "\"": "\"", "\\": "\\" }[c] ?? c));
}

function emitClause(c) {
  let inner = `key = ${csharpString(c.key)}, op = FlagOp.${c.op}`;
  if (c.op === "AddInt" || c.op === "RequireIntAtLeast") inner += `, amount = ${c.amount | 0}`;
  return `new FlagClause { ${inner} }`;
}
function emitClauseArray(arr) { return `new[] { ${arr.map(emitClause).join(", ")} }`; }

function emitChoice(ch) {
  const parts = [`text = ${csharpString(ch.text)}`];
  const next = endRef(ch.nextNodeId);
  if (next) parts.push(`nextNodeId = ${csharpString(next)}`);
  if (ch.conditions && ch.conditions.length) parts.push(`conditions = ${emitClauseArray(ch.conditions)}`);
  if (ch.effects && ch.effects.length) parts.push(`effects = ${emitClauseArray(ch.effects)}`);
  if (ch.checkDC && ch.checkDC > 0) {
    parts.push(`checkAbility = Ability.${ch.checkAbility}`, `checkDC = ${ch.checkDC}`);
    const fail = endRef(ch.failNodeId);
    if (fail) parts.push(`failNodeId = ${csharpString(fail)}`);
  }
  return `new DialogueChoice { ${parts.join(", ")} }`;
}

function emitNode(n, indent) {
  const parts = [`id = ${csharpString(n.id)}`, `speaker = ${csharpString(n.speaker)}`, `text = ${csharpString(n.text)}`];
  if (n.onEnter && n.onEnter.length) parts.push(`onEnter = ${emitClauseArray(n.onEnter)}`);
  const auto = endRef(n.autoNextNodeId);
  if (auto) parts.push(`autoNextNodeId = ${csharpString(auto)}`);
  let body = parts.join(", ");
  if (n.choices && n.choices.length) {
    const ci = indent + "    ";
    const choices = n.choices.map(c => ci + emitChoice(c)).join(",\n");
    body += `,\n${indent}    choices = new[]\n${indent}    {\n${choices}\n${indent}    }`;
  }
  return `${indent}g.nodes.Add(new DialogueNode { ${body} });`;
}

function emitGraph(g, indent) {
  const lines = [];
  lines.push(`${indent}g = ScriptableObject.CreateInstance<DialogueGraph>();`);
  lines.push(`${indent}g.conversationId = ${csharpString(g.conversationId)}; g.startNodeId = ${csharpString(g.startNodeId)};`);
  for (const n of g.nodes) lines.push(emitNode(n, indent));
  lines.push(`${indent}graphs.Add(g);`);
  return lines.join("\n");
}

function className(file) {
  let base = file.replace(/\.json$/, "").replace(/[^A-Za-z0-9]+/g, " ").trim().split(/\s+/)
    .map(w => w.charAt(0).toUpperCase() + w.slice(1)).join("");
  if (!base || !/^[A-Za-z]/.test(base)) base = "Zone" + base; // C# identifiers can't start with a digit
  return base + "BridgeContent";
}

function emitFile(file, graphs) {
  const cls = className(file);
  const ind = "            ";
  const blocks = graphs.map(g => emitGraph(g, ind)).join("\n\n");
  return `// <auto-generated> Emitted by tools/json-to-csharp.js from play/${file}. Regenerate; do not hand-edit.
// The honest prototype->Unity bridge: this builds the same DialogueGraph objects the hand-authored
// content does (see GarrowQuestContent.cs). Base-text fidelity — variant/crit/fumble reactivity needs
// the runner extensions noted in docs/UNITY_BRIDGE_PLAN.md.
using System.Collections.Generic;
using UnityEngine;
using SunderedCrown.Dialogue;
using SunderedCrown.Stats;

namespace SunderedCrown.Content.Bridge
{
    /// <summary>Bridged conversations from play/${file} (${graphs.length} graph${graphs.length === 1 ? "" : "s"}).</summary>
    public static class ${cls}
    {
        public static List<DialogueGraph> Build()
        {
            var graphs = new List<DialogueGraph>();
            DialogueGraph g;

${blocks}

            return graphs;
        }
    }
}
`;
}

// Group the converter's flat graph list back by source file so each emitted class mirrors one zone.
// `skip` is the set of conversationIds already in the C# build (dedup); those are omitted. Returns
// { byFile: Map<file, graphs[]>, skipped: number } so callers can report the dedup honestly.
function graphsByFile(skip) {
  skip = skip || csharpConversationIds();
  const byFile = new Map();
  let skipped = 0;
  const dir = path.join(__dirname, "..", "play");
  for (const file of fs.readdirSync(dir)) {
    if (!file.endsWith(".json")) continue;
    let data; try { data = JSON.parse(fs.readFileSync(path.join(dir, file), "utf8")); } catch { continue; }
    if (!data || !Array.isArray(data.conversations)) continue;
    const gaps = { variants: [], banter: [], draw: [], dynamic: [], crit: [], fumble: [], nonFlagGates: [], unknownAbility: [], droppedSkillName: [], choiceTag: [] };
    const graphs = [];
    for (const c of data.conversations) {
      if (!c || !c.id) continue;
      if (skip.has(c.id)) { skipped++; continue; }
      graphs.push(U.convertConversation(c, gaps));
    }
    if (graphs.length) byFile.set(file, graphs);
  }
  return { byFile, skipped };
}

function main() {
  const outDir = process.argv[2] || path.join(__dirname, "unity-export", "csharp");
  fs.mkdirSync(outDir, { recursive: true });
  const { byFile, skipped } = graphsByFile();
  let files = 0, graphs = 0;
  for (const [file, gs] of byFile) {
    fs.writeFileSync(path.join(outDir, className(file) + ".cs"), emitFile(file, gs));
    files++; graphs += gs.length;
  }
  console.log(`  json-to-csharp: emitted ${files} C# content classes (${graphs} NEW DialogueGraphs) to ${outDir}`);
  console.log(`  skipped ${skipped} conversations already authored in the C# build (dedup — no collisions).`);
  console.log(`  drop into Assets/Scripts/Content/Bridge/ and call <Class>.Build(); compile once in the Editor.`);
}

module.exports = { csharpString, unescapeCsharpString, emitClause, emitChoice, emitNode, emitGraph, emitFile, className, graphsByFile, csharpConversationIds, isEndSentinel, endRef, FLAG_OPS };
if (require.main === module) main();
