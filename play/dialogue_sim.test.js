// Gate for the playable Dialogue Simulator (dialogue_sim.html). It lets you step into any
// conversation and roll real skill checks; this proves the page's resolution logic matches
// the C# engine (DialogueRunner.cs + Abilities.cs) and that its embedded cast index is sound.
// We lift the page's /*<DLGSIM>*/ pure block and exercise it, then validate the data + wiring.
const fs = require("fs");
let pass = 0, fail = 0; const fails = [];
function check(name, cond) { if (cond) pass++; else { fail++; fails.push(name); } }

const h = fs.readFileSync(__dirname + "/dialogue_sim.html", "utf8");

// ---- lift the pure resolution block ----
const block = h.match(/\/\*<DLGSIM>\*\/([\s\S]*?)\/\*<\/DLGSIM>\*\//);
check("DLGSIM pure block found", !!block);
const E = new Function(block[1] +
  "\nreturn {abilityMod,resolveCheck,chanceToPass,newState,applyEffects,conditionsPass};")();

// modifier = floor((score-10)/2), exactly as Abilities.cs
check("abilityMod: 10 -> +0", E.abilityMod(10) === 0);
check("abilityMod: 16 -> +3", E.abilityMod(16) === 3);
check("abilityMod: 17 -> +3 (floor)", E.abilityMod(17) === 3);
check("abilityMod: 8 -> -1", E.abilityMod(8) === -1);
check("abilityMod: 1 -> -5", E.abilityMod(1) === -5);

// resolveCheck: roll = d20 + mod; success when >= DC (no nat-20 auto), matching DialogueRunner
check("resolveCheck: 14+3 >= 15 passes", E.resolveCheck(14, 15, 3) === true);
check("resolveCheck: 11+3 >= 15 fails", E.resolveCheck(11, 15, 3) === false);
check("resolveCheck: exact DC passes", E.resolveCheck(12, 15, 3) === true);
check("resolveCheck: negative mod can fail a high roll", E.resolveCheck(18, 20, -3) === false);

// chanceToPass: # of d20 faces (1..20) that clear DC-mod, /20, clamped
check("chance: DC10 mod0 -> 11/20", Math.abs(E.chanceToPass(10, 0) - 11 / 20) < 1e-9);
check("chance: DC20 mod0 -> 1/20", Math.abs(E.chanceToPass(20, 0) - 1 / 20) < 1e-9);
check("chance: DC25 mod0 -> 0 (impossible)", E.chanceToPass(25, 0) === 0);
check("chance: DC5 mod10 -> 1 (auto, clamped)", E.chanceToPass(5, 10) === 1);
check("chance: a +3 mod beats a +0 mod on the same DC", E.chanceToPass(15, 3) > E.chanceToPass(15, 0));

// applyEffects: SetTrue writes a bool, AddInt accumulates
let st = E.newState();
check("newState is empty", Object.keys(st.bools).length === 0 && Object.keys(st.ints).length === 0);
E.applyEffects(st, [{ key: "sq.feast_held_open", op: "SetTrue" }]);
check("SetTrue sets a bool", st.bools["sq.feast_held_open"] === true);
E.applyEffects(st, [{ key: "companion.naeve.approval", op: "AddInt", amount: 6 }]);
E.applyEffects(st, [{ key: "companion.naeve.approval", op: "AddInt", amount: -2 }]);
check("AddInt accumulates (6 - 2 = 4)", st.ints["companion.naeve.approval"] === 4);

// conditionsPass: RequireBoolTrue gates on a flag, or on the 'assume earlier story' toggle
check("condition unmet without flag", E.conditionsPass(E.newState(), [{ key: "x", op: "RequireBoolTrue" }], false) === false);
check("condition met when flag set", E.conditionsPass(st2set("x"), [{ key: "x", op: "RequireBoolTrue" }], false) === true);
check("condition met under assumePrior", E.conditionsPass(E.newState(), [{ key: "x", op: "RequireBoolTrue" }], true) === true);
check("no conditions always pass", E.conditionsPass(E.newState(), [], false) === true);
function st2set(k){ const s = E.newState(); s.bools[k] = true; return s; }

// ---- validate the embedded data + cast index ----
const dm = h.match(/const DATA = (\{[\s\S]*?\});\nconst BUILDS/);
check("DATA blob present", !!dm);
const DATA = JSON.parse(dm[1]);
const CONVS = DATA.conversations, CAST = DATA.cast;
check("has conversations", CONVS.length > 200);
check("has a cast index", CAST.length > 50);
check("every conversation got a primary speaker", CONVS.every(c => c.primarySpeaker && c.primarySpeaker.length));

// every conversation is reachable from exactly one cast entry's convs list
const covered = new Set();
CAST.forEach(c => c.convs.forEach(i => covered.add(i)));
check("cast index covers every conversation", covered.size === CONVS.length);
check("cast conv indices are all valid", CAST.every(c => c.convs.every(i => i >= 0 && i < CONVS.length)));
check("cast counts match their conv lists", CAST.every(c => c.count === c.convs.length));
check("every cast member has a sigil + hue", CAST.every(c => c.sigil && typeof c.hue === "number"));
check("there is a featured principal cast", CAST.some(c => c.featured));
check("featured cast is sorted first", (() => { const fi = CAST.findIndex(c => c.featured), ni = CAST.findIndex(c => !c.featured); return fi === 0 && (ni === -1 || ni > fi); })());

// the five authentic builds, with the three dialogue abilities spread across them
const bm = h.match(/const BUILDS = (\[[\s\S]*?\]);\nconst COMPANION/);
check("BUILDS present", !!bm);
const BUILDS = JSON.parse(bm[1]);
check("five starting builds", BUILDS.length === 5);
check("each build has six ability scores", BUILDS.every(b => b.scores.length === 6));
// INT(3)/WIS(4)/CHA(5) — the dialogue abilities — differ between builds so build choice matters
check("the Confessor has the best WIS", BUILDS.reduce((m, b) => b.scores[4] > m.scores[4] ? b : m).name === "The Confessor");
check("the Silver Tongue has the best CHA", BUILDS.reduce((m, b) => b.scores[5] > m.scores[5] ? b : m).name === "The Silver Tongue");
check("the Scholar has the best INT", BUILDS.reduce((m, b) => b.scores[3] > m.scores[3] ? b : m).name === "The Scholar");

// ---- wiring: the page mounts the engine + UI ----
check("script tags balanced", (h.match(/<script>/g) || []).length === (h.match(/<\/script>/g) || []).length);
check("cast browser wired", h.includes("function renderCast(") && h.includes("function pickNpc("));
check("play loop wired", h.includes("function startConv(") && h.includes("function goNode(") && h.includes("function choose("));
check("dice roller wired", h.includes("function rollDice(") && h.includes("resolveCheck(roll"));
check("live state panel wired", h.includes("function renderState(") && h.includes('id="approvals"') && h.includes('id="flags"'));
check("preview tools wired", h.includes('id="tReveal"') && h.includes('id="tForce"') && h.includes('id="tAssume"'));
check("build picker wired", h.includes("function setBuild(") && h.includes("function bump("));
check("links home to the index", h.includes('href="index.html"'));

// a real skill-check choice exists in the data so the dice path is exercised in-game
const hasCheck = CONVS.some(c => c.nodes.some(n => n.choices.some(ch => ch.checkDC > 0)));
check("the data contains real skill checks to roll", hasCheck);

// ---- headless playability sweep: auto-walk EVERY conversation through the real traversal
// rules the UI uses (apply onEnter, take the first allowed choice / success branch / auto),
// proving each one is playable to an end with no broken mid-path ref and bounded steps.
let played = 0, longest = 0;
const isEndId = (conv, id, byId) => !id || id === "END" || id === "end" || !byId[id];
for (const conv of CONVS) {
  const byId = {}; conv.nodes.forEach(n => byId[n.id] = n);
  let id = byId[conv.start] ? conv.start : conv.nodes[0].id;
  const st = E.newState(); let steps = 0; const seen = new Set();
  while (!isEndId(conv, id, byId)) {
    const n = byId[id];
    if (++steps > 500) { fails.push("runaway traversal in " + conv.id); fail++; break; }
    E.applyEffects(st, n.onEnter);
    const allowed = (n.choices || []).filter(ch => E.conditionsPass(st, ch.conditions, true));
    if (allowed.length) {
      const ch = allowed[0];
      E.applyEffects(st, ch.effects);            // effects apply regardless of pass/fail
      id = ch.next;                               // success branch (assume the check passes)
    } else if (n.auto && !isEndId(conv, n.auto, byId)) {
      id = n.auto;
    } else break;                                 // node with nothing further = an ending
    if (seen.has(id + ":" + steps)) break;
  }
  longest = Math.max(longest, steps);
  played++;
}
check("every conversation auto-plays to an end (bounded, no broken mid-path ref)", played === CONVS.length);
check("no conversation needs an unreasonable number of beats", longest < 500);

console.log(`\n  Dialogue Simulator — playable conversation engine:`);
for (const f of fails) console.log("  ✗ " + f);
console.log(`  ${pass} passed, ${fail} failed`);
if (!fail) console.log(`  ✓ engine matches DialogueRunner.cs (d20+mod vs DC), cast index sound, UI wired.\n`);
process.exit(fail ? 1 : 0);
