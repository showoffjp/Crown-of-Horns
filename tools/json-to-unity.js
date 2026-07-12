#!/usr/bin/env node
// json-to-unity.js — the honest bridge from the play/ prototype to the real Unity game.
//
// The play/ JSON is the proven, tested source of truth (1,500+ passing assertions). The Unity
// build reads C# ScriptableObjects: SunderedCrown.Dialogue.DialogueGraph and
// SunderedCrown.Quests.Quest. This converter transforms the prototype conversations + side-quests
// into JsonUtility-shaped JSON whose field names match those C# classes EXACTLY, so a tiny C#
// loader (EditorJsonUtility.FromJsonOverwrite on a fresh asset) can mint the assets in one pass.
//
// It is deliberately HONEST about what does NOT survive the crossing. The C# DialogueGraph has no
// concept of variants, draw-routing, banter nodes, skill-check crit/fumble branches, or the
// character-state gates (race/class/deity/ability/...). Every one of those is COUNTED and itemised
// in a gap report rather than silently dropped, so the Unity-side work is a known, finite list —
// not a surprise at compile time.
//
// Output (default ./tools/unity-export/):
//   dialogue-graphs.json  — { graphs: [ {conversationId, startNodeId, nodes:[...]}, ... ] }
//   quests.json           — { quests: [ {questId, title, summary, completionFlag, ...}, ... ] }
//   gap-report.json       — every feature the C# model can't yet hold, with counts + locations
//
// Run:  node tools/json-to-unity.js [outDir]
// Test: node tools/json-to-unity.test.js
"use strict";
const fs = require("fs");
const path = require("path");

const PLAY_DIR = path.join(__dirname, "..", "play");

// Stats.Ability enum (Assets/Scripts/Stats/Abilities.cs). JsonUtility serialises enums by name.
const ABILITIES = new Set(["Strength", "Dexterity", "Constitution", "Intelligence", "Wisdom", "Charisma"]);
// Character-state gates the C# FlagClause model has no clause for (it speaks only bool/int flags).
const NONFLAG_GATES = ["ability", "background", "class", "deity", "gender", "law", "morality", "race"];

// --- when{} (high-level visibility gate) -> FlagClause[] (the C# condition vocabulary) ----------
// Returns { conditions:[...], untranslated:[{kind,detail}] }. flags/flagsNot/flag/int translate
// cleanly; the character-state gates can't and are reported, not faked.
function translateWhen(when) {
  const conditions = [];
  const untranslated = [];
  if (!when || typeof when !== "object") return { conditions, untranslated };
  for (const f of when.flags || []) conditions.push({ key: f, op: "RequireBoolTrue", amount: 0 });
  for (const f of when.flagsNot || []) conditions.push({ key: f, op: "RequireBoolFalse", amount: 0 });
  if (when.flag) conditions.push({ key: when.flag, op: "RequireBoolTrue", amount: 0 });
  if (when.int && typeof when.int === "object") {
    for (const k of Object.keys(when.int)) conditions.push({ key: k, op: "RequireIntAtLeast", amount: when.int[k] });
  }
  for (const g of NONFLAG_GATES) {
    if (g in when) untranslated.push({ kind: g, detail: JSON.stringify(when[g]) });
  }
  return { conditions, untranslated };
}

// Pick the text a C# single-text node will carry from a variant list: the first variant with no
// `when` is the unconditional default the prototype always falls through to.
function baseTextFromVariants(variants) {
  if (!Array.isArray(variants)) return "";
  const def = variants.find(v => !v.when) || variants[variants.length - 1];
  return (def && def.text) || "";
}

function convertChoice(ch, loc, gaps) {
  const out = { text: ch.text || "...", nextNodeId: ch.next || "", conditions: [], effects: [], checkAbility: "Charisma", checkDC: 0, failNodeId: ch.fail || "" };

  // conditions: pre-translated FlagClause[] pass straight through; `when` is translated.
  if (Array.isArray(ch.conditions)) out.conditions.push(...ch.conditions);
  const tw = translateWhen(ch.when);
  out.conditions.push(...tw.conditions);
  for (const u of tw.untranslated) gaps.nonFlagGates.push({ at: loc, ...u });

  if (Array.isArray(ch.effects)) out.effects = ch.effects;

  if (ch.check) {
    const ab = ch.check.ability;
    out.checkAbility = ABILITIES.has(ab) ? ab : "Charisma";
    out.checkDC = ch.check.dc | 0;
    if (!ABILITIES.has(ab)) gaps.unknownAbility.push({ at: loc, ability: ab });
    if (ch.check.skill) gaps.droppedSkillName.push({ at: loc, skill: ch.check.skill }); // C# keeps ability+DC only
  }
  // crit/fumble: the prototype branches the narration on nat-20/nat-1; C# has no such branch yet.
  if (ch.crit) gaps.crit.push({ at: loc, node: loc.node, target: ch.crit });
  if (ch.fumble) gaps.fumble.push({ at: loc, node: loc.node, target: ch.fumble });
  if (ch.tag) gaps.choiceTag.push({ at: loc, tag: ch.tag }); // e.g. [RETURNED] surfacing tag
  return out;
}

function convertNode(n, convId, gaps) {
  const loc = { conv: convId, node: n.id };
  const out = { id: n.id, speaker: n.speaker || "NPC", text: "", onEnter: [], choices: [], autoNextNodeId: n.auto || "" };

  // text: direct, else the unconditional variant default.
  if (typeof n.text === "string") out.text = n.text;
  else if (n.variants) { out.text = baseTextFromVariants(n.variants); gaps.variants.push({ at: loc, count: n.variants.length }); }

  // entry effects: onEnter and node-level effects are both apply-on-show in the prototype; C# folds
  // them into a single onEnter[].
  if (Array.isArray(n.onEnter)) out.onEnter.push(...n.onEnter);
  if (Array.isArray(n.effects)) out.onEnter.push(...n.effects);

  for (const ch of n.choices || []) out.choices.push(convertChoice(ch, { conv: convId, node: n.id }, gaps));

  // engine primitives with no C# home yet — counted, and the node still ships its base text.
  if (n.banter) gaps.banter.push({ at: loc });
  if ("draw" in n) gaps.draw.push({ at: loc });
  if (n.dynamic) gaps.dynamic.push({ at: loc });
  return out;
}

function convertConversation(conv, gaps) {
  return {
    conversationId: conv.id,
    startNodeId: conv.start || (conv.nodes && conv.nodes[0] && conv.nodes[0].id) || "",
    nodes: (conv.nodes || []).map(n => convertNode(n, conv.id, gaps)),
  };
}

function convertQuest(q) {
  return {
    questId: q.questId,
    title: q.title || "",
    summary: q.premise || q.summary || "",
    completionFlag: q.completionFlag || "",
    failureFlag: q.failureFlag || "",
    objectives: (q.objectives || []).map(o => ({
      objectiveId: o.objectiveId || "",
      description: o.desc || o.description || "",
      completionFlag: o.completionFlag || "",
      hidden: !!o.hidden,
      optional: !!o.optional,
    })),
    experienceReward: q.experienceReward != null ? q.experienceReward : 100,
    goldReward: q.goldReward != null ? q.goldReward : 0,
  };
}

function build() {
  const gaps = { variants: [], banter: [], draw: [], dynamic: [], crit: [], fumble: [], nonFlagGates: [], unknownAbility: [], droppedSkillName: [], choiceTag: [] };
  const graphs = [];
  let convCount = 0, nodeCount = 0, fileCount = 0;

  for (const file of fs.readdirSync(PLAY_DIR)) {
    if (!file.endsWith(".json")) continue;
    let data;
    try { data = JSON.parse(fs.readFileSync(path.join(PLAY_DIR, file), "utf8")); } catch { continue; }
    if (!data || typeof data !== "object" || !Array.isArray(data.conversations)) continue;
    fileCount++;
    for (const conv of data.conversations) {
      if (!conv || !conv.id) continue;
      graphs.push(convertConversation(conv, gaps));
      convCount++;
      nodeCount += (conv.nodes || []).length;
    }
  }

  let quests = [];
  try {
    const sq = JSON.parse(fs.readFileSync(path.join(PLAY_DIR, "sidequests.json"), "utf8"));
    quests = (sq.sidequests || []).map(convertQuest);
  } catch { /* no quests */ }

  const report = {
    summary: {
      files: fileCount, conversations: convCount, nodes: nodeCount, quests: quests.length,
      gapsByFeature: Object.fromEntries(Object.keys(gaps).map(k => [k, gaps[k].length])),
    },
    note: "Each gap is a Unity-side feature to add or a clause that can't cross unchanged. Counts are the exact, finite work list — nothing is silently dropped.",
    gaps,
  };
  return { graphs, quests, report };
}

function main() {
  const outDir = process.argv[2] || path.join(__dirname, "unity-export");
  fs.mkdirSync(outDir, { recursive: true });
  const { graphs, quests, report } = build();
  fs.writeFileSync(path.join(outDir, "dialogue-graphs.json"), JSON.stringify({ graphs }, null, 1));
  fs.writeFileSync(path.join(outDir, "quests.json"), JSON.stringify({ quests }, null, 1));
  fs.writeFileSync(path.join(outDir, "gap-report.json"), JSON.stringify(report, null, 2));
  const s = report.summary;
  console.log(`  json-to-unity: ${s.conversations} conversations / ${s.nodes} nodes / ${s.quests} quests from ${s.files} files`);
  console.log(`  wrote ${outDir}/{dialogue-graphs,quests,gap-report}.json`);
  console.log(`  gaps (Unity-side work list): ` + Object.entries(s.gapsByFeature).filter(([, v]) => v).map(([k, v]) => `${k}=${v}`).join("  "));
}

module.exports = { translateWhen, convertChoice, convertNode, convertConversation, convertQuest, build, ABILITIES, NONFLAG_GATES };
if (require.main === module) main();
