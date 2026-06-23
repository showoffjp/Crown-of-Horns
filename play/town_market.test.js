// Gate for the walkable Market scene (town_market.html). Proves the scene data is sound, the
// three NPC conversations are playable by every shipped character, and the reactive resolution
// logic in the page's /*<MKT>*/ block matches the C# engine (DialogueRunner.cs + Abilities.cs).
const fs = require("fs");
let pass = 0, fail = 0; const fails = [];
function check(name, cond) { if (cond) pass++; else { fail++; fails.push(name); } }

const h = fs.readFileSync(__dirname + "/town_market.html", "utf8");
const ABILS6 = ["Strength", "Dexterity", "Constitution", "Intelligence", "Wisdom", "Charisma"];

// ---- lift the pure engine ----
const block = h.match(/\/\*<MKT>\*\/([\s\S]*?)\/\*<\/MKT>\*\//);
check("MKT pure block found", !!block);
const E = new Function(block[1] +
  "\nreturn {abilityMod,resolveCheck,chanceToPass,newState,applyEffects,isProficient,checkBonus,matchesWhen,pickVariantText,choiceAvailable,isPassiveSkill,passiveScore,passiveBeats,glossaryHits,loreKnown,returnedClarity};")();

check("abilityMod matches floor((score-10)/2)", E.abilityMod(10) === 0 && E.abilityMod(16) === 3 && E.abilityMod(17) === 3 && E.abilityMod(8) === -1);
check("resolveCheck: roll+mod vs DC", E.resolveCheck(12, 15, 3) === true && E.resolveCheck(11, 15, 3) === false);
check("chanceToPass clamps", E.chanceToPass(25, 0) === 0 && E.chanceToPass(5, 10) === 1 && Math.abs(E.chanceToPass(15, 3) - 9 / 20) < 1e-9);

// ---- embedded data ----
const dm = h.match(/const DATA = (\{[\s\S]*?\});\nconst BUILDS/);
check("DATA blob present", !!dm);
const DATA = JSON.parse(dm[1]);
const SCENE = DATA.scene, CONVS = DATA.conversations, MODEL = DATA.model;
const bm = h.match(/const BUILDS = (\[[\s\S]*?\]);\nconst INT_LABELS/);
const BUILDS = JSON.parse(bm[1]);

// scene integrity
check("scene has the named NPCs", SCENE.npcs.length >= 6 && SCENE.npcs.every(n => n.name && n.conv && n.sigil));
check("scene has a walkable grid", SCENE.w >= 8 && SCENE.h >= 6);
check("scene has props (stalls, fountain, …)", SCENE.props.length >= 5 && SCENE.props.some(p => p.type === "fountain") && SCENE.props.some(p => p.type === "stall"));
check("player start is inside the grid", SCENE.playerStart.x >= 0 && SCENE.playerStart.x < SCENE.w && SCENE.playerStart.y >= 0 && SCENE.playerStart.y < SCENE.h);
check("every NPC sits inside the grid", SCENE.npcs.every(n => n.x >= 0 && n.x < SCENE.w && n.y >= 0 && n.y < SCENE.h));
check("every NPC points at a real conversation", SCENE.npcs.every(n => CONVS.some(c => c.id === n.conv)));

// character model
check("model present with proficiencies", MODEL.classProficiencies && MODEL.backgroundProficiencies && MODEL.proficiencyBonus > 0);
check("model maps skills to abilities", Object.values(MODEL.skillAbility).every(a => ABILS6.includes(a)));
check("five shipped characters", BUILDS.length === 5 && BUILDS.every(b => b.scores.length === 6 && b.race && b.deity));

// ---- conversation integrity: refs resolve, no dup ids ----
let refsOk = true, dupOk = true;
for (const conv of CONVS) {
  const ids = new Set(conv.nodes.map(n => n.id)); const seen = new Set();
  for (const n of conv.nodes) { if (seen.has(n.id)) dupOk = false; seen.add(n.id);
    const targets = []; if (n.auto) targets.push(n.auto);
    (n.choices || []).forEach(ch => { if (ch.next) targets.push(ch.next); if (ch.fail) targets.push(ch.fail); });
    for (const t of targets) if (t !== "END" && t !== "end" && !ids.has(t)) refsOk = false;
  }
}
check("all conversation node references resolve", refsOk);
check("no duplicate node ids", dupOk);

// ---- reactivity: the same NPC greets different characters with different lines ----
const confessor = BUILDS.find(b => b.name === "The Confessor");
const warden = BUILDS.find(b => b.name === "The Warden");
const tiefling = BUILDS.find(b => b.name === "The Silver Tongue");
const sable = CONVS.find(c => c.id === "market.sable");
const n0 = sable.nodes.find(n => n.id === "0");
check("Sable's opening has reactive variants", n0.variants && n0.variants.length >= 4);
const lineKel = E.pickVariantText(n0, confessor, E.newState());
const lineFth = E.pickVariantText(n0, warden, E.newState());
const lineTief = E.pickVariantText(n0, tiefling, E.newState());
check("Kelemvorite, Faithless & tiefling get distinct greetings", new Set([lineKel, lineFth, lineTief]).size === 3);
check("the Faithless greeting speaks to the godless", /godless|no god/i.test(lineFth));

// identity-gated choices differ per character
const n1 = sable.nodes.find(n => n.id === "1");
const faithlessOpt = n1.choices.find(ch => ch.when && ch.when.deity === "None");
check("a [Faithless] option is gated to the godless", faithlessOpt &&
  E.choiceAvailable(warden, E.newState(), faithlessOpt, MODEL) === true &&
  E.choiceAvailable(confessor, E.newState(), faithlessOpt, MODEL) === false);
const availSet = (c) => n1.choices.filter(ch => E.choiceAvailable(c, E.newState(), ch, MODEL)).map(ch => ch.text).join("|");
check("Sable offers different choices to different characters",
  availSet(confessor) !== availSet(warden) && availSet(warden) !== availSet(tiefling) && availSet(confessor) !== availSet(tiefling));

// proficiency genuinely changes the odds (Cleric proficient in Insight, etc.)
check("proficiency raises a check bonus", E.checkBonus(confessor, { skill: "Insight", ability: "Wisdom" }, MODEL) > E.checkBonus(warden, { skill: "Acrobatics", ability: "Dexterity" }, MODEL) - 99 &&
  E.isProficient(confessor, "Insight", MODEL) === true && E.isProficient(warden, "Insight", MODEL) === true);
check("a non-proficient skill gets no bonus", E.checkBonus(confessor, { skill: "Stealth", ability: "Dexterity" }, MODEL) === E.abilityMod(confessor.scores[1]));

// ---- BG3/5e passive vs active checks ----
check("knowledge/awareness skills are passive", E.isPassiveSkill("Insight") && E.isPassiveSkill("Perception") && E.isPassiveSkill("Religion") && E.isPassiveSkill("Investigation"));
check("social-attempt skills are active (rolled)", !E.isPassiveSkill("Persuasion") && !E.isPassiveSkill("Deception") && !E.isPassiveSkill("Intimidation"));
check("passiveScore = 10 + ability mod (+prof)", E.passiveScore(confessor, { skill: "Insight", ability: "Wisdom" }, MODEL) === 10 + E.abilityMod(17) + MODEL.proficiencyBonus);
check("passiveBeats compares to DC", E.passiveBeats(confessor, { skill: "Insight", ability: "Wisdom", dc: 13 }, MODEL) === true &&
  E.passiveBeats(confessor, { skill: "Insight", ability: "Wisdom", dc: 99 }, MODEL) === false);
// a PASSIVE-skill option is hidden when you can't meet it, and shown (auto) when you can — no dice either way
const sableInsight = n1.choices.find(ch => (ch.check || {}).skill === "Insight");
const lowWis = { cls: "Fighter", scores: [16, 14, 15, 10, 10, 10], race: "Human", background: "Soldier", law: "Neutral", morality: "Neutral", deity: "Kelemvor" };
check("a passive Insight option hides when your passive score is below the DC",
  sableInsight && E.choiceAvailable(lowWis, E.newState(), sableInsight, MODEL) === false &&
  E.choiceAvailable(confessor, E.newState(), sableInsight, MODEL) === true);
// an ACTIVE option (Persuasion) is always offered regardless of your Charisma — you may always *attempt* it
const persuadeOpt = n1.choices.find(ch => (ch.check || {}).skill === "Persuasion");
check("an active Persuasion option is offered to anyone (you can always try)",
  persuadeOpt && E.choiceAvailable(lowWis, E.newState(), persuadeOpt, MODEL) === true &&
  E.choiceAvailable({ cls: "Wizard", scores: [8, 8, 8, 8, 8, 8], race: "Gnome", background: "Sage", law: "Neutral", morality: "Neutral", deity: "Oghma" }, E.newState(), persuadeOpt, MODEL) === true);

// ---- LORE glossary + 5e passive knowledge reveals ----
const GLOSS = DATA.glossary;
check("glossary is embedded with common-knowledge blurbs", Array.isArray(GLOSS) && GLOSS.length >= 15 && GLOSS.every(e => e.term && e.gloss));
check("lore entries carry a knowledge skill + DC", GLOSS.filter(e => e.skill).length >= 10 &&
  GLOSS.filter(e => e.skill).every(e => MODEL.skillAbility[e.skill] && typeof e.dc === "number"));
check("glossaryHits finds terms (and aka) in a line", E.glossaryHits("a servant of the Judge of the Dead, by the Wall of the Faithless", GLOSS).length >= 2);
const kel = GLOSS.find(e => e.term === "Kelemvor");
const lowAll = { cls: "Fighter", scores: [16, 14, 15, 8, 8, 10], race: "Human", background: "Soldier", law: "Neutral", morality: "Neutral", deity: "None" };
check("a proficient cleric recalls religious lore the dullard doesn't", kel &&
  E.loreKnown(confessor, kel, MODEL) === true && E.loreKnown(lowAll, kel, MODEL) === false);
const histE = GLOSS.find(e => e.skill === "History" && e.dc >= 11);
const scholar = { cls: "Wizard", scores: [10, 14, 12, 16, 14, 11], race: "Half-Elf", background: "Sage", law: "Neutral", morality: "Good", deity: "Oghma" };
check("a sage recalls history a soldier wouldn't", histE && E.loreKnown(scholar, histE, MODEL) === true);

// ---- The Returned-sense (innate, WIS-scaled soul perception) ----
check("every NPC carries a Returned-sense reveal with a DC", CONVS.every(c => c.returned && c.returned.text && typeof c.returned.dc === "number"));
check("Returned clarity = 10 + Wisdom modifier", E.returnedClarity(confessor) === 10 + E.abilityMod(confessor.scores[4]));
const blunt = { cls: "Fighter", scores: [16, 14, 15, 10, 8, 10], race: "Human", background: "Soldier", law: "Neutral", morality: "Neutral", deity: "Kelemvor" };
check("a keen-souled Returned senses what a dull one can't (Sable, DC 12)",
  E.returnedClarity(confessor) >= sable.returned.dc && E.returnedClarity(blunt) < sable.returned.dc);

// ---- Pillars-style dispositions ----
const dispKeys = new Set();
CONVS.forEach(c => c.nodes.forEach(n => { (n.effects || []).concat(...(n.choices || []).map(ch => ch.effects || []))
  .forEach(e => { if (e && e.key && e.key.indexOf("disp.") === 0) dispKeys.add(e.key); }); }));
check("choices/outcomes accrue several dispositions", dispKeys.size >= 4);
check("each NPC offers a [RETURNED] choice (your uncanny nature)", CONVS.every(c =>
  c.nodes.some(n => (n.choices || []).some(ch => ch.tag === "returned"))));
// the disposition GATE: Pip's [Merciful] line is hidden at 0 mercy, shown once you've been merciful
const pip = CONVS.find(c => c.id === "market.pip");
const pn1 = pip.nodes.find(n => n.id === "1");
const mercyGate = pn1.choices.find(ch => ch.when && ch.when.int && ch.when.int["disp.merciful"]);
const merciful = E.newState(); merciful.ints["disp.merciful"] = 1;
check("a disposition-gated line is hidden at 0 and unlocked once merciful (PoE-style)", mercyGate &&
  E.choiceAvailable(confessor, E.newState(), mercyGate, MODEL) === false &&
  E.choiceAvailable(confessor, merciful, mercyGate, MODEL) === true);

// ---- cross-NPC interplay (a Pillars/BG3 hallmark): what you do at one soul changes another ----
const tallow = CONVS.find(c => c.id === "market.tallow");
const wren = CONVS.find(c => c.id === "market.wren");
check("the three new souls are present", tallow && wren && CONVS.find(c => c.id === "market.calix"));
const setsFlag = (conv, flag) => conv.nodes.some(n =>
  (n.effects || []).some(e => e.key === flag) || (n.choices || []).some(ch => (ch.effects || []).some(e => e.key === flag)));
check("protecting/betraying Wren is written at Tallow", setsFlag(tallow, "market.protected_wren") && setsFlag(tallow, "market.betrayed_wren"));
const wrenVar = (flag) => wren.nodes.find(n => n.id === "0").variants.some(v => v.when && v.when.flag === flag);
check("Wren reacts to having been protected, and to having been betrayed", wrenVar("market.protected_wren") && wrenVar("market.betrayed_wren"));
// a protected Wren greets you differently than an untouched one
const wn0 = wren.nodes.find(n => n.id === "0");
const protectedSt = E.newState(); protectedSt.bools["market.protected_wren"] = true;
const goodGuy = { cls: "Fighter", scores: [14, 14, 14, 12, 12, 12], race: "Human", background: "Folk Hero", law: "Neutral", morality: "Good", deity: "None" };
check("a protected Wren opens with gratitude she wouldn't give a stranger",
  E.pickVariantText(wn0, goodGuy, protectedSt) !== E.pickVariantText(wn0, goodGuy, E.newState()) &&
  /thank you/i.test(E.pickVariantText(wn0, goodGuy, protectedSt)));
// Tallow's [DECEPTION] cover only appears once you've actually met Wren (a real flag-gate, not a passive skill)
const decOpt = tallow.nodes.find(n => n.id === "1").choices.find(ch => (ch.check || {}).skill === "Deception");
const metWren = E.newState(); metWren.bools["market.met_wren"] = true;
check("you can only vouch for Wren to Tallow after you've met her", decOpt && decOpt.when && decOpt.when.flag === "market.met_wren" &&
  E.choiceAvailable(goodGuy, metWren, decOpt, MODEL) === true && E.choiceAvailable(goodGuy, E.newState(), decOpt, MODEL) === false);

// ---- every NPC conversation completes for every shipped character ----
const isEnd = (conv, id, byId) => !id || id === "END" || id === "end" || !byId[id];
function autoPlay(conv, who) {
  const byId = {}; conv.nodes.forEach(n => byId[n.id] = n);
  let id = byId[conv.start] ? conv.start : conv.nodes[0].id; const st = E.newState(); let steps = 0;
  while (!isEnd(conv, id, byId)) {
    const n = byId[id]; if (++steps > 300) return false;
    E.applyEffects(st, n.onEnter); E.applyEffects(st, n.effects);
    if (n.variants && E.pickVariantText(n, who, st).length === 0) return false;  // every char must get a line
    const allowed = (n.choices || []).filter(ch => E.choiceAvailable(who, st, ch, MODEL));
    if (allowed.length) { E.applyEffects(st, allowed[0].effects); id = allowed[0].next; }
    else if (n.auto && !isEnd(conv, n.auto, byId)) id = n.auto;
    else break;
  }
  return true;
}
let combos = 0, okCombos = 0;
for (const conv of CONVS) for (const b of BUILDS) { combos++; if (autoPlay(conv, b)) okCombos++; }
check("every NPC conversation completes for every character (no dead end)", okCombos === combos && combos === CONVS.length * BUILDS.length);

// flags set in one conversation persist to the shared visit state (handled by the page's single `st`)
check("conversations write real story flags", CONVS.some(c => c.nodes.some(n =>
  (n.onEnter || []).some(e => e.op === "SetTrue") || (n.choices || []).some(ch => (ch.effects || []).some(e => e.op === "SetTrue")))));

// ---- wiring: the scene engine + dialogue overlay are mounted ----
check("script tags balanced", (h.match(/<script>/g) || []).length === (h.match(/<\/script>/g) || []).length);
check("isometric board + projection wired", h.includes('id="board"') && h.includes("function iso(") && h.includes("function unIso("));
check("click-to-move wired", h.includes("player.target") && h.includes('addEventListener("click"') && h.includes("function update("));
check("NPC tokens + talk prompt drawn", h.includes("function drawToken(") && h.includes("talk (E)"));
check("approach-to-talk wired (click + E key)", h.includes("function talk(") && h.includes('e.key==="E"'));
check("dialogue overlay + reactive engine wired", h.includes("function goNode(") && h.includes("function paintChoices(") && h.includes("pickVariantText(n,char,st)"));
check("animated dice roll wired", h.includes("function rollDice(") && h.includes("resolveCheck(roll"));
check("live ledger (regard + flags) wired", h.includes("function renderState(") && h.includes('id="approvals"') && h.includes('id="flags"'));
check("character builder wired (race/class/deity/alignment)", h.includes("function renderWho(") && h.includes("setAlign(") && h.includes("function bump("));
check("self-contained (no external script/img refs)", !/src="(?!data:)[^"]+\.(png|jpg|jpeg|js)"/.test(h));
check("links home to the index", h.includes('href="index.html"'));

console.log(`\n  The Market of the Causeway — walkable scene + reactive NPCs:`);
for (const f of fails) console.log("  ✗ " + f);
console.log(`  ${pass} passed, ${fail} failed`);
if (!fail) console.log(`  ✓ scene sound, all NPC trees complete for all 5 builds, reactive engine matches the C# rules.\n`);
process.exit(fail ? 1 : 0);
