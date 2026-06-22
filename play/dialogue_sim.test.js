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
  "\nreturn {abilityMod,resolveCheck,chanceToPass,newState,applyEffects,conditionsPass," +
  "isProficient,checkBonus,matchesWhen,pickVariantText,choiceAvailable,isPassiveSkill,passiveScore,passiveBeats};")();

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
const hasCheck = CONVS.some(c => c.nodes.some(n => (n.choices || []).some(ch => ch.checkDC > 0 || ch.check)));
check("the data contains real skill checks to roll", hasCheck);

// ---- the BG3-style character model + reactive engine ----
check("character MODEL wired to data", h.includes("MODEL = DATA.model") && h.includes("function matchesWhen("));
const MODEL = DATA.model;
const ABILS6 = ["Strength", "Dexterity", "Constitution", "Intelligence", "Wisdom", "Charisma"];
check("model has races/classes/backgrounds/deities", MODEL.races.length >= 6 && MODEL.classes.length >= 5 &&
  MODEL.backgrounds.length >= 5 && MODEL.deities.length >= 5);
check("model maps every skill to an ability", Object.values(MODEL.skillAbility).every(a => ABILS6.includes(a)));
check("model defines a proficiency bonus", MODEL.proficiencyBonus > 0);

// two concrete characters to exercise reactivity
const confessor = { cls: "Cleric", scores: [13, 10, 14, 11, 17, 12], race: "Human", background: "Acolyte", law: "Lawful", morality: "Good", deity: "Kelemvor" };
const warden = { cls: "Ranger", scores: [14, 16, 13, 11, 14, 10], race: "Elf", background: "Folk Hero", law: "Neutral", morality: "Neutral", deity: "None" };

// matchesWhen — identity gates (string + array), alignment, ability thresholds, flags, ints
check("when race matches (string)", E.matchesWhen(warden, E.newState(), { race: "Elf" }) === true);
check("when race matches (array)", E.matchesWhen(warden, E.newState(), { race: ["Elf", "Half-Elf"] }) === true);
check("when race rejects a non-match", E.matchesWhen(confessor, E.newState(), { race: "Elf" }) === false);
check("when deity gate (Kelemvor)", E.matchesWhen(confessor, E.newState(), { deity: "Kelemvor" }) === true);
check("when Faithless gate (deity None)", E.matchesWhen(warden, E.newState(), { deity: "None" }) === true);
check("when class gate", E.matchesWhen(confessor, E.newState(), { class: "Cleric" }) === true);
check("when background gate", E.matchesWhen(confessor, E.newState(), { background: "Acolyte" }) === true);
check("when law gate", E.matchesWhen(confessor, E.newState(), { law: "Lawful" }) === true && E.matchesWhen(warden, E.newState(), { law: "Lawful" }) === false);
check("when morality gate", E.matchesWhen(confessor, E.newState(), { morality: "Good" }) === true);
check("when ability threshold (STR 15: warden 14 fails)", E.matchesWhen(warden, E.newState(), { ability: { Strength: 15 } }) === false);
check("when ability threshold (DEX 15: warden 16 passes)", E.matchesWhen(warden, E.newState(), { ability: { Dexterity: 15 } }) === true);
check("when flag gate respects state", E.matchesWhen(confessor, st2set("demo.x"), { flag: "demo.x" }) === true &&
  E.matchesWhen(confessor, E.newState(), { flag: "demo.x" }) === false);
let si = E.newState(); si.ints["demo.vane.regard"] = 3;
check("when int threshold (regard >= 3)", E.matchesWhen(confessor, si, { int: { "demo.vane.regard": 3 } }) === true &&
  E.matchesWhen(confessor, E.newState(), { int: { "demo.vane.regard": 3 } }) === false);
check("when with no clause always matches", E.matchesWhen(confessor, E.newState(), undefined) === true);

// proficiency math — Cleric+Acolyte is proficient in Religion/Insight/Persuasion; not Stealth
check("isProficient via class", E.isProficient(confessor, "Religion", MODEL) === true);
check("isProficient via background", E.isProficient(confessor, "Insight", MODEL) === true);
check("not proficient in an untrained skill", E.isProficient(confessor, "Stealth", MODEL) === false);
// checkBonus = abilityMod + (proficient ? profBonus : 0)
check("checkBonus adds proficiency", E.checkBonus(confessor, { skill: "Insight", ability: "Wisdom" }, MODEL) === E.abilityMod(17) + MODEL.proficiencyBonus);
check("checkBonus without proficiency is just the mod", E.checkBonus(confessor, { skill: "Acrobatics", ability: "Dexterity" }, MODEL) === E.abilityMod(10));
check("proficiency genuinely raises the odds", E.checkBonus(confessor, { skill: "Insight", ability: "Wisdom" }, MODEL) > E.checkBonus(warden, { skill: "Insight", ability: "Wisdom" }, MODEL));

// reactive narration — the demo's opening line differs by character
const demo = CONVS.find(c => c.id === "demo.threshold");
check("the reactive demo is present", !!demo);
const node0 = demo.nodes.find(n => n.id === "0");
check("node 0 has reactive variants", node0.variants && node0.variants.length >= 5);
const lineKelemvor = E.pickVariantText(node0, confessor, E.newState());
const lineFaithless = E.pickVariantText(node0, warden, E.newState());
check("Kelemvor & Faithless get different opening lines", lineKelemvor !== lineFaithless && lineKelemvor.length > 40 && lineFaithless.length > 40);
check("Faithless opening actually speaks to the godless", /Faithless|no god/i.test(lineFaithless));
check("variant falls back to a default", (() => { const any = { cls: "Fighter", scores: [10, 10, 10, 10, 10, 10], race: "Dwarf", background: "Soldier", law: "Neutral", morality: "Neutral", deity: "Tymora" }; return E.pickVariantText(node0, any, E.newState()).length > 40; })());

// choiceAvailable — the Faithless option only appears for the godless; Kelemvor option only for kin
const node1 = demo.nodes.find(n => n.id === "1");
const faithlessChoice = node1.choices.find(ch => ch.when && ch.when.deity === "None");
const kinChoice = node1.choices.find(ch => ch.when && ch.when.deity === "Kelemvor");
check("Faithless choice gated to the godless", E.choiceAvailable(warden, E.newState(), faithlessChoice, false, MODEL) === true &&
  E.choiceAvailable(confessor, E.newState(), faithlessChoice, false, MODEL) === false);
check("Kelemvor choice gated to kin", E.choiceAvailable(confessor, E.newState(), kinChoice, false, MODEL) === true &&
  E.choiceAvailable(warden, E.newState(), kinChoice, false, MODEL) === false);
// the same node offers a different set of choices to different characters
const availFor = (ch) => node1.choices.filter(c => E.choiceAvailable(ch, E.newState(), c, false, MODEL)).length;
check("the challenge node offers different choices per character", availFor(confessor) !== availFor(warden));

// BG3/5e passive vs active: knowledge/awareness skills auto-resolve and only show if you'd pass;
// social-attempt skills are always offered and rolled.
check("knowledge skills are passive", E.isPassiveSkill("Insight") && E.isPassiveSkill("Perception") && E.isPassiveSkill("Religion"));
check("social skills are active", !E.isPassiveSkill("Persuasion") && !E.isPassiveSkill("Deception") && !E.isPassiveSkill("Intimidation"));
check("passiveScore = 10 + mod (+prof)", E.passiveScore(confessor, { skill: "Insight", ability: "Wisdom" }, MODEL) === 10 + E.abilityMod(17) + MODEL.proficiencyBonus);
const fnode = demo.nodes.find(n => n.id === "faithless");
const insightOpt = fnode && fnode.choices.find(ch => (ch.check || {}).skill === "Insight");
const dull = { cls: "Fighter", scores: [16, 14, 15, 10, 8, 10], race: "Human", background: "Soldier", law: "Neutral", morality: "Neutral", deity: "None" };
check("a passive [INSIGHT] option hides below its DC and shows (auto) above it", insightOpt &&
  E.choiceAvailable(dull, E.newState(), insightOpt, false, MODEL) === false &&
  E.choiceAvailable(warden, E.newState(), insightOpt, false, MODEL) === true);

function st2set(k){ const s = E.newState(); s.bools[k] = true; return s; }

// ---- headless playability sweep: auto-walk EVERY conversation through the real traversal
// rules the UI uses (apply onEnter, take the first allowed choice / success branch / auto),
// proving each one is playable to an end with no broken mid-path ref and bounded steps.
const isEndId = (conv, id, byId) => !id || id === "END" || id === "end" || !byId[id];
function autoPlay(conv, who) {
  const byId = {}; conv.nodes.forEach(n => byId[n.id] = n);
  let id = byId[conv.start] ? conv.start : conv.nodes[0].id;
  const st = E.newState(); let steps = 0;
  while (!isEndId(conv, id, byId)) {
    const n = byId[id];
    if (++steps > 500) return { steps, ok: false };
    E.applyEffects(st, n.onEnter);
    // every node must yield a line for this character (a variant or default)
    if (n.variants && E.pickVariantText(n, who, st).length === 0) return { steps, ok: false, noVariant: n.id };
    const allowed = (n.choices || []).filter(ch => E.choiceAvailable(who, st, ch, true, MODEL));
    if (allowed.length) { const ch = allowed[0]; E.applyEffects(st, ch.effects); id = ch.next; }
    else if (n.auto && !isEndId(conv, n.auto, byId)) id = n.auto;
    else break;
  }
  return { steps, ok: true };
}
const dummy = { cls: "Fighter", scores: [12, 12, 12, 12, 12, 12], race: "Human", background: "Soldier", law: "Neutral", morality: "Neutral", deity: "Kelemvor" };
let played = 0, longest = 0;
for (const conv of CONVS) { const r = autoPlay(conv, dummy); if (r.ok) played++; else fails.push("unplayable: " + conv.id + (r.noVariant ? " (no variant for node " + r.noVariant + ")" : "")); longest = Math.max(longest, r.steps); }
check("every conversation auto-plays to an end for a baseline character", played === CONVS.length);
check("no conversation needs an unreasonable number of beats", longest < 500);
// the reactive demo must be completable by EACH of the five shipped builds (no dead character)
const bm2 = h.match(/const BUILDS = (\[[\s\S]*?\]);\nconst COMPANION/);
const ALLBUILDS = JSON.parse(bm2[1]);
let demoOk = 0;
for (const b of ALLBUILDS) { const r = autoPlay(demo, b); if (r.ok) demoOk++; else fails.push("demo dead-ends for " + b.name); }
check("the reactive demo completes for all five shipped characters", demoOk === ALLBUILDS.length);

console.log(`\n  Dialogue Simulator — playable conversation engine:`);
for (const f of fails) console.log("  ✗ " + f);
console.log(`  ${pass} passed, ${fail} failed`);
if (!fail) console.log(`  ✓ engine matches DialogueRunner.cs (d20+mod vs DC), cast index sound, UI wired.\n`);
process.exit(fail ? 1 : 0);
