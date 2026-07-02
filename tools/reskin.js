#!/usr/bin/env node
// reskin.js — apply the serial-numbers-filed map (tools/reskin-map.json) to produce a build with the
// Forgotten Realms / D&D proper nouns swapped for original equivalents, WITHOUT touching the source.
//
// Why this exists: the project's value (souls, systems, code, writing) is original; only a thin layer of
// proprietary NAMES sits on top. This lets you generate a licence-independent build from the same source,
// prove it works (gates stay green), and keep both versions. If the FR licence falls through, you ship the
// reskinned build; nothing about the game changes but the nouns.
//
// Replacement is WHOLE-WORD (ASCII-letter boundaries via lookaround, so 'drow' never hits 'drowned' and
// 'Amn' never hits 'damn'), CASE-SENSITIVE (so 'Bane' the god is swapped but 'bane' the word is left, and
// 'mask'/'chosen'/'reaper' commonwords are untouched), and applied LONGEST-KEY-FIRST (so 'Wall of the
// Faithless' resolves before 'Faithless', 'Baldur's Gate' before 'Baldur').
//
// Usage:
//   node tools/reskin.js                 # reskin all play/*.json + the generator -> tools/reskin-build/
//   node tools/reskin.js --check FILE     # print remaining FR terms in FILE (audit only, no write)
"use strict";
const fs = require("fs");
const path = require("path");

const ROOT = path.join(__dirname, "..");
const MAP = JSON.parse(fs.readFileSync(path.join(__dirname, "reskin-map.json"), "utf8"));

// Build an ordered list of [regex, replacement], longest key first so multi-word phrases win.
function buildRules(map) {
  const keys = Object.keys(map).sort((a, b) => b.length - a.length);
  return keys.map(k => {
    const esc = k.replace(/[.*+?^${}()|[\]\\]/g, "\\$&");
    // ASCII-letter boundaries: don't match inside a longer word, but DO allow trailing "'s", spaces, punctuation.
    return { key: k, re: new RegExp("(?<![A-Za-z])" + esc + "(?![A-Za-z])", "g"), to: map[k] };
  });
}
const RULES = buildRules(MAP.map);

// Apply the whole map to a string. Longest-first ordering means once a phrase is replaced, its substrings
// won't re-match the freshly written text (the replacements are original words not in the map).
function reskin(text) {
  let out = text;
  for (const r of RULES) out = out.replace(r.re, r.to);
  return out;
}

// The set of source terms that must NOT survive a reskin (every map key). Used by --check and the test.
function remainingTerms(text) {
  const found = {};
  for (const r of RULES) {
    const m = text.match(r.re);
    if (m && m.length) found[r.key] = m.length;
  }
  return found;
}

// Files a full reskin must cover: every walkable-content JSON plus the generator (its NPC_SENSE reads and
// character BUILDS carry deity names that must swap in lockstep with the content's `when.deity` gates).
function sourceFiles() {
  const play = path.join(ROOT, "play");
  const files = fs.readdirSync(play).filter(f => f.endsWith(".json")).map(f => path.join("play", f));
  files.push(path.join("tools", "make-town-market.py"));
  return files;
}

function main() {
  const args = process.argv.slice(2);
  if (args[0] === "--check") {
    const txt = fs.readFileSync(args[1], "utf8");
    const rem = remainingTerms(txt);
    const keys = Object.keys(rem);
    console.log(keys.length ? "FR terms present:\n" + keys.map(k => `  ${String(rem[k]).padStart(4)}  ${k}`).join("\n") : "no mapped FR terms present.");
    return;
  }
  const outDir = args[0] || path.join(__dirname, "reskin-build");
  let files = 0, subs = 0;
  for (const rel of sourceFiles()) {
    const src = fs.readFileSync(path.join(ROOT, rel), "utf8");
    const before = remainingTerms(src);
    const out = reskin(src);
    const dest = path.join(outDir, rel);
    fs.mkdirSync(path.dirname(dest), { recursive: true });
    fs.writeFileSync(dest, out);
    files++;
    subs += Object.values(before).reduce((a, b) => a + b, 0);
  }
  console.log(`  reskin: ${files} source files -> ${outDir}`);
  console.log(`  applied ${subs} whole-word substitutions across ${RULES.length} mapped terms.`);
  console.log(`  to build a filed-off game: point the generator at ${outDir}/play, or copy over source, then regenerate.`);
}

module.exports = { buildRules, reskin, remainingTerms, sourceFiles, MAP, RULES };
if (require.main === module) main();
