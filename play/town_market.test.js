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
  "\nreturn {abilityMod,resolveCheck,chanceToPass,newState,applyEffects,isProficient,checkBonus,matchesWhen,pickVariantText,choiceAvailable,isPassiveSkill,passiveScore,passiveBeats,glossaryHits,loreKnown,returnedClarity,rollResult,rollBreakdown,buildBlocked,findPath,nearestFreeAdjacent,inBounds,tileKey};")();

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

// ---- dispositions NPCs REACT to: your reckoning, earned earlier in the visit, changes how later souls greet you ----
const reckon = (key, n) => { const s = E.newState(); s.ints[key] = n; return s; };
const plainSoul = { cls: "Wizard", scores: [12, 12, 12, 12, 12, 12], race: "Gnome", gender: "Female", background: "Sage", law: "Neutral", morality: "Neutral", deity: "Oghma" };
const wren0 = wren.nodes.find(n => n.id === "0");
check("a merciful reputation changes how Wren greets you", wren0.variants.some(v => v.when && v.when.int && v.when.int["disp.merciful"]) &&
  E.pickVariantText(wren0, plainSoul, reckon("disp.merciful", 2)) !== E.pickVariantText(wren0, plainSoul, E.newState()) &&
  /gentle|kind/i.test(E.pickVariantText(wren0, plainSoul, reckon("disp.merciful", 2))));
const tallow0 = tallow.nodes.find(n => n.id === "0");
check("a ruthless reputation makes Tallow welcome you as kindred", /practical|friends|grateful/i.test(E.pickVariantText(tallow0, plainSoul, reckon("disp.ruthless", 2))) &&
  E.pickVariantText(tallow0, plainSoul, reckon("disp.ruthless", 2)) !== E.pickVariantText(tallow0, plainSoul, reckon("disp.merciful", 2)));
const calix0 = CONVS.find(c => c.id === "market.calix").nodes.find(n => n.id === "0");
const joss0 = CONVS.find(c => c.id === "market.joss").nodes.find(n => n.id === "0");
check("Calix and Joss both have disposition-reactive openings", calix0.variants.some(v => (v.when || {}).int) && joss0.variants.some(v => (v.when || {}).int));
check("a haunted soul gets a different welcome than a heretical one (Joss)",
  E.pickVariantText(joss0, plainSoul, reckon("disp.haunted", 2)) !== E.pickVariantText(joss0, plainSoul, reckon("disp.heretical", 2)));
// but identity still wins over disposition where it's iconic: a Faithless player gets the godless greeting
// whether or not they're haunted (the deity variant precedes the disposition one in the list)
const faithlessSoul = { ...plainSoul, deity: "None" };
check("identity greetings still outrank disposition ones",
  E.pickVariantText(joss0, faithlessSoul, reckon("disp.haunted", 2)) === E.pickVariantText(joss0, faithlessSoul, E.newState()) &&
  E.pickVariantText(joss0, faithlessSoul, E.newState()) !== E.pickVariantText(joss0, plainSoul, reckon("disp.haunted", 2)));

// ---- the "30+ authored options, you see a handful" thesis (Mad Joss's mega-node) ----
const joss = CONVS.find(c => c.id === "market.joss");
const mega = joss.nodes.find(n => n.id === "1");
check("the mega-node authors 30+ potential responses", mega.choices.length >= 30);
check("gender is a real reactive axis", MODEL.genders && MODEL.genders.length === 2 &&
  mega.choices.some(ch => ch.when && ch.when.gender === "Female") && mega.choices.some(ch => ch.when && ch.when.gender === "Male"));
const visible = (c, state) => mega.choices.filter(ch => E.choiceAvailable(c, state || E.newState(), ch, MODEL)).length;
// each character sees only a fraction of the whole — and a *different* fraction
const femaleCleric = { cls: "Cleric", scores: [13, 10, 14, 11, 17, 12], race: "Human", gender: "Female", background: "Acolyte", law: "Lawful", morality: "Good", deity: "Kelemvor" };
const maleTiefling = { cls: "Rogue", scores: [10, 16, 12, 13, 11, 16], race: "Tiefling", gender: "Male", background: "Charlatan", law: "Chaotic", morality: "Neutral", deity: "Tymora" };
check("any one character sees far fewer than all the options", visible(femaleCleric) < mega.choices.length / 2 && visible(maleTiefling) < mega.choices.length / 2);
check("every character still has something to say (never a dead node)", visible(femaleCleric) >= 3 && visible(maleTiefling) >= 3);
const setOf = (c) => mega.choices.filter(ch => E.choiceAvailable(c, E.newState(), ch, MODEL)).map(ch => ch.text).join("|");
check("different identities surface visibly different option sets", setOf(femaleCleric) !== setOf(maleTiefling));
check("flipping only gender changes which options appear", (() => {
  const f = { ...femaleCleric }, m = { ...femaleCleric, gender: "Male" };
  return setOf(f) !== setOf(m);
})());
// prior choices widen the set: having met Wren / earned dispositions unlocks more of the 50
const richState = E.newState(); richState.bools["market.met_wren"] = true; richState.bools["market.holding_stone"] = true; richState.ints["disp.heretical"] = 1; richState.ints["disp.merciful"] = 2;
check("what you've already done unlocks still more potential responses", visible(femaleCleric, richState) > visible(femaleCleric));

// ---- BG3 critical hits & fumbles (nat 20 / nat 1) with unique comedic responses ----
check("a natural 20 auto-succeeds against any DC", E.rollResult(20, 30, -5).success === true && E.rollResult(20, 30, -5).crit === true);
check("a natural 1 auto-fails against any DC", E.rollResult(1, 5, 10).success === false && E.rollResult(1, 5, 10).fumble === true);
check("ordinary rolls follow roll+mod vs DC", E.rollResult(14, 15, 3).success === true && E.rollResult(8, 15, 3).success === false &&
  E.rollResult(14, 15, 3).crit === false && E.rollResult(8, 15, 3).fumble === false);
// real checks route to dedicated comedic crit/fumble nodes, and those nodes exist
let critChecks = 0, critNodesOk = true;
for (const conv of CONVS) {
  const ids = new Set(conv.nodes.map(n => n.id));
  for (const n of conv.nodes) for (const ch of (n.choices || [])) {
    if (ch.crit || ch.fumble) { critChecks++;
      if (ch.crit && !ids.has(ch.crit)) critNodesOk = false;
      if (ch.fumble && !ids.has(ch.fumble)) critNodesOk = false; }
  }
}
check("multiple checks have bespoke crit & fumble responses", critChecks >= 6);
check("every crit/fumble target node exists", critNodesOk);
// the crit and fumble outcomes are genuinely different text from the normal pass/fail
const sableHaggle = sable.nodes.find(n => n.id === "1").choices.find(ch => (ch.check || {}).skill === "Persuasion");
const nodeText = (id) => { const n = sable.nodes.find(x => x.id === id); return n ? n.text : ""; };
check("Sable's nat-20 / nat-1 are unique scenes (not the normal branches)", sableHaggle.crit && sableHaggle.fumble &&
  nodeText(sableHaggle.crit) !== nodeText(sableHaggle.next) && nodeText(sableHaggle.fumble) !== nodeText(sableHaggle.fail) &&
  /grandmother|pigeon|stool/i.test(nodeText(sableHaggle.crit)));   // the comedy is actually in there
// a nat 20 still applies the choice's effects (and routes to the crit node), keeping story state honest
check("crit/fumble nodes carry their own effects & still resolve to an ending", CONVS.every(c => {
  const byId = {}; c.nodes.forEach(n => byId[n.id] = n);
  return c.nodes.every(n => (n.choices || []).every(ch =>
    (!ch.crit || byId[ch.crit]) && (!ch.fumble || byId[ch.fumble])));
}));

// ---- BG3-style pre-roll preview: the player sees the whole math before committing ----
const cha = { cls: "Rogue", scores: [10, 16, 12, 13, 11, 16], race: "Tiefling", gender: "Male", background: "Charlatan", law: "Chaotic", morality: "Neutral", deity: "Tymora" };
const bdP = E.rollBreakdown(cha, { skill: "Persuasion", ability: "Charisma", dc: 14 }, MODEL);
check("pre-roll breakdown sums ability mod + proficiency", bdP.abilityMod === E.abilityMod(16) && bdP.prof === true &&
  bdP.profBonus === MODEL.proficiencyBonus && bdP.bonus === E.abilityMod(16) + MODEL.proficiencyBonus);
check("pre-roll tells you the number you need on the die", bdP.need === 14 - bdP.bonus && bdP.needShown === 14 - bdP.bonus);
check("pre-roll chance matches chanceToPass", Math.abs(bdP.chance - E.chanceToPass(14, bdP.bonus)) < 1e-9);
const trivial = E.rollBreakdown(cha, { skill: "Persuasion", ability: "Charisma", dc: 5 }, MODEL);
const brutal = E.rollBreakdown({ ...cha, scores: [8, 8, 8, 8, 8, 6] }, { skill: null, ability: "Charisma", dc: 25 }, MODEL);
check("a trivial check flags 'only a nat 1 can fail you'", trivial.onlyNat1 === true && trivial.onlyNat20 === false);
check("an impossible check flags 'only a nat 20 can save you'", brutal.onlyNat20 === true && brutal.onlyNat1 === false);
check("the pre-roll panel is wired (breakdown + a ROLL button)", h.includes("function rollPreview(") && h.includes("ROLL THE DICE") && h.includes("rollBreakdown(char,chk,MODEL)") && h.includes(".rollprev{"));
check("choosing a check shows the preview before rolling (no instant auto-roll)", h.includes("rollPreview(ch,chk,bonus,resolve)"));
check("a real faceted d20 tumbles, lands & flares on crits", h.includes("const D20_SVG") && h.includes("<polygon points=") &&
  h.includes("@keyframes tumble") && h.includes("@keyframes dland") && h.includes("d20wrap.nat20") && h.includes("d20wrap.nat1"));

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
check("click-to-move wired", h.includes("player.path") && h.includes('addEventListener("click"') && h.includes("function update(") && h.includes("function walkTo("));
// ---- collision + A* pathfinding: you can't walk through people, stalls, or the fountain ----
const SCN = DATA.scene, BL = E.buildBlocked ? E.buildBlocked(SCN) : null;
check("every prop and NPC blocks its tile", BL && Object.keys(BL).length === SCN.props.length + SCN.npcs.length);
check("you cannot path onto an NPC's tile (the walk-through bug, fixed)",
  SCN.npcs.every(n => E.findPath(SCN.playerStart.x, SCN.playerStart.y, n.x, n.y, BL, SCN.w, SCN.h).length === 0));
check("a path never crosses a blocked tile", (() => {
  const p = E.findPath(SCN.playerStart.x, SCN.playerStart.y, 0, 0, BL, SCN.w, SCN.h);
  return p.length > 0 && p.every(s => !BL[E.tileKey(s[0], s[1])]);
})());
check("every NPC is reachable via a free adjacent tile", SCN.npcs.every(n => {
  const adj = E.nearestFreeAdjacent(SCN.playerStart.x, SCN.playerStart.y, n.x, n.y, BL, SCN.w, SCN.h);
  if (!adj) return false;
  return (adj[0] === SCN.playerStart.x && adj[1] === SCN.playerStart.y) ||
    E.findPath(SCN.playerStart.x, SCN.playerStart.y, adj[0], adj[1], BL, SCN.w, SCN.h).length > 0;
}));
check("the path-following loop + path-preview dots are wired", h.includes("player.pathIdx") && h.includes("player.path"));

// ---- second walkable zone: The Reed-Walk, reached by a causeway exit, sharing the visit state ----
const SCENES = DATA.scenes; const REED = SCENES && SCENES.reedwalk;
check("a second zone (the Reed-Walk) ships alongside the market", REED && REED.npcs.length >= 4 && REED.w >= 8 && REED.h >= 6);
check("both zones are keyed by id and the market is the start scene", SCENES && SCENES.market && SCENES.reedwalk && DATA.scene.id === "market");
check("the market has a glowing exit that points at the reed-walk", (SCN.exits || []).some(x => x.to === "reedwalk" && x.dest));
check("the reed-walk has an exit back to the market", (REED.exits || []).some(x => x.to === "market" && x.dest));
check("every exit lands on an in-bounds destination tile", Object.values(SCENES).every(s => (s.exits || []).every(x =>
  x.dest.x >= 0 && x.dest.x < SCENES[x.to].w && x.dest.y >= 0 && x.dest.y < SCENES[x.to].h)));
check("exit tiles are walkable (not blocked) and reachable from each zone's start", Object.values(SCENES).every(s => {
  const bl = E.buildBlocked(s);
  return (s.exits || []).every(x => !bl[E.tileKey(x.x, x.y)] &&
    ((x.x === s.playerStart.x && x.y === s.playerStart.y) ||
      E.findPath(s.playerStart.x, s.playerStart.y, x.x, x.y, bl, s.w, s.h).length > 0));
}));
check("zone-travel is wired (loadScene + a fade + a step-onto-exit trigger)",
  h.includes("function loadScene(") && h.includes("function travelTo(") && h.includes("function exitAt(") && h.includes("traveling"));
// the reed-walk's NPCs are collision-clean too — no walking through the boatman
const RBL = E.buildBlocked(REED);
check("Reed-Walk NPCs/props block their tiles and stay reachable", REED.npcs.every(n => {
  if (RBL[E.tileKey(n.x, n.y)] !== 1) return false;
  const adj = E.nearestFreeAdjacent(REED.playerStart.x, REED.playerStart.y, n.x, n.y, RBL, REED.w, REED.h);
  return adj && ((adj[0] === REED.playerStart.x && adj[1] === REED.playerStart.y) ||
    E.findPath(REED.playerStart.x, REED.playerStart.y, adj[0], adj[1], RBL, REED.w, REED.h).length > 0);
}));

// ---- the river PAYS OFF the market: flags you set in one zone change how souls greet you in the other ----
const cassian = CONVS.find(c => c.id === "reed.cassian");
const vharn = CONVS.find(c => c.id === "reed.vharn");
const reedWren = CONVS.find(c => c.id === "reed.wren");
const reedwife = CONVS.find(c => c.id === "reed.reedwife");
check("the four river souls are present", cassian && vharn && reedWren && reedwife);
// Sister Vharn's welcome flips on what you did to Wren back in the market — the cross-zone hinge
const v0 = vharn.nodes.find(n => n.id === "0");
const betrayedSt = E.newState(); betrayedSt.bools["market.betrayed_wren"] = true;
const protSt = E.newState(); protSt.bools["market.protected_wren"] = true;
check("Vharn greets a Wren-betrayer as a friend and a Wren-protector as a threat", v0.variants &&
  E.pickVariantText(v0, goodGuy, betrayedSt) !== E.pickVariantText(v0, goodGuy, protSt) &&
  /Tallow|friend|gave us|helpful/i.test(E.pickVariantText(v0, goodGuy, betrayedSt)) &&
  /meddler|where is she|stood between/i.test(E.pickVariantText(v0, goodGuy, protSt)));
// river-Wren remembers the market too: gratitude if protected, heartbreak if betrayed
const rw0 = reedWren.nodes.find(n => n.id === "0");
check("river-Wren opens with relief if you saved her, and betrayal if you sold her",
  E.pickVariantText(rw0, goodGuy, protSt) !== E.pickVariantText(rw0, goodGuy, betrayedSt) &&
  /Judas|smiled|gave it to him/i.test(E.pickVariantText(rw0, goodGuy, betrayedSt)));
// the Deception cover at Vharn is gated on having protected Wren (you only lie for someone you saved)
const vDec = vharn.nodes.find(n => n.id === "1").choices.find(ch => (ch.check || {}).skill === "Deception");
check("you can only lie to Vharn to cover a Wren you actually protected", vDec && vDec.when && vDec.when.flag === "market.protected_wren" &&
  E.choiceAvailable(goodGuy, protSt, vDec, MODEL) === true && E.choiceAvailable(goodGuy, E.newState(), vDec, MODEL) === false);
// giving Sable's holding-stone to river-Wren is gated on actually carrying it out of the market
const wStone = reedWren.nodes.find(n => n.id === "1").choices.find(ch => ch.when && ch.when.flag === "market.holding_stone");
const stoneSt = E.newState(); stoneSt.bools["market.holding_stone"] = true;
check("Sable's holding-stone can be handed to Wren only if you took it from the market", wStone &&
  E.choiceAvailable(goodGuy, stoneSt, wStone, MODEL) === true && E.choiceAvailable(goodGuy, E.newState(), wStone, MODEL) === false);
// the river souls keep the deep stack: crit/fumble, dispositions, a Returned line each
check("river checks carry their own nat-20/nat-1 scenes too", [cassian, vharn, reedWren].every(c =>
  c.nodes.some(n => (n.choices || []).some(ch => ch.crit && ch.fumble))));
check("the Reed-Walk grows the new prop vocabulary (boat, reeds, shrine)",
  h.includes('p.type==="boat"') && h.includes('p.type==="reeds"') && h.includes('p.type==="shrine"'));

// ---- third zone: The Underbridge — Pip's dying brother, and a thread that ties back to the river ----
const UNDER = SCENES && SCENES.underbridge;
check("a third zone (the Underbridge) ships too", UNDER && UNDER.npcs.length >= 3 && UNDER.w >= 8 && UNDER.h >= 6);
check("the market has TWO exits now (the river and the stair down)", (SCN.exits || []).length >= 2 &&
  (SCN.exits || []).some(x => x.to === "reedwalk") && (SCN.exits || []).some(x => x.to === "underbridge"));
check("the Underbridge has a stair back up to the market", (UNDER.exits || []).some(x => x.to === "market"));
const UBL = E.buildBlocked(UNDER);
check("Underbridge NPCs/props block their tiles and stay reachable", UNDER.npcs.every(n => {
  if (UBL[E.tileKey(n.x, n.y)] !== 1) return false;
  const adj = E.nearestFreeAdjacent(UNDER.playerStart.x, UNDER.playerStart.y, n.x, n.y, UBL, UNDER.w, UNDER.h);
  return adj && ((adj[0] === UNDER.playerStart.x && adj[1] === UNDER.playerStart.y) ||
    E.findPath(UNDER.playerStart.x, UNDER.playerStart.y, adj[0], adj[1], UBL, UNDER.w, UNDER.h).length > 0);
}));
check("the Underbridge grows the prop vocabulary (pillar, brazier, loom, pallet)",
  h.includes('p.type==="bridgepillar"') && h.includes('p.type==="brazier"') && h.includes('p.type==="loom"') && h.includes('p.type==="pallet"'));

const wick = CONVS.find(c => c.id === "under.wick");
const underPip = CONVS.find(c => c.id === "under.pip");
const knotwife = CONVS.find(c => c.id === "under.knotwife");
check("the three Underbridge souls are present", wick && underPip && knotwife);
// Pip-at-the-pallet pays off how you treated Pip in the market
const up0 = underPip.nodes.find(n => n.id === "0");
const helpedPip = E.newState(); helpedPip.bools["market.helped_pip"] = true;
const scaredPip = E.newState(); scaredPip.bools["market.scared_pip"] = true;
check("Pip greets you warmly if you helped her in the market, hostile if you scared her", up0.variants &&
  E.pickVariantText(up0, goodGuy, helpedPip) !== E.pickVariantText(up0, goodGuy, scaredPip) &&
  /grabbing|threatened|bite/i.test(E.pickVariantText(up0, goodGuy, scaredPip)));
// Wick — the second flame your Returned-sense felt in the market — greets a Returned and a Kelemvorite distinctly
const wk0 = wick.nodes.find(n => n.id === "0");
const kelGuy = { ...goodGuy, deity: "Kelemvor" };
check("Wick reacts to the Judge's servant and to having heard of you", wk0.variants &&
  E.pickVariantText(wk0, kelGuy, helpedPip) !== E.pickVariantText(wk0, goodGuy, E.newState()));
// THE deep cross-zone hinge: the Reed-Wife's "knot has a name" unlocks the Knotwife's buried history
const kn0 = knotwife.nodes.find(n => n.id === "0");
const knewName = E.newState(); knewName.bools["reed.knot_has_a_name"] = true;
check("learning 'the knot has a name' at the river changes how the Knotwife greets you", kn0.variants &&
  E.pickVariantText(knotwife.nodes.find(n => n.id === "0"), goodGuy, knewName) !== E.pickVariantText(kn0, goodGuy, E.newState()) &&
  /Maren|guild-sister|name/i.test(E.pickVariantText(kn0, goodGuy, knewName)));
// and the [HISTORY] reveal of Chancellor Venn is gated on having heard the phrase at the river
const histChoice = knotwife.nodes.find(n => n.id === "1").choices.find(ch => (ch.check || {}).skill === "History");
check("the Chancellor Venn revelation is gated on the river phrase", histChoice && histChoice.when && histChoice.when.flags &&
  histChoice.when.flags.indexOf("reed.knot_has_a_name") >= 0 &&
  E.choiceAvailable(scholar, knewName, histChoice, MODEL) === true && E.choiceAvailable(scholar, E.newState(), histChoice, MODEL) === false);
// the new souls keep the deep stack: crit/fumble comedy + a Returned line each
check("Underbridge checks carry their own nat-20/nat-1 scenes", [underPip, wick, knotwife].every(c =>
  c.nodes.some(n => (n.choices || []).some(ch => ch.crit && ch.fumble))));
check("each Underbridge soul offers a [RETURNED] line", [underPip, wick, knotwife].every(c =>
  c.nodes.some(n => (n.choices || []).some(ch => ch.tag === "returned"))));

// ---- fourth zone: Past the Last Torch — the Wall of the Faithless, and three threads paid off at once ----
const LAST = SCENES && SCENES.lasttorch;
check("a fourth zone (Past the Last Torch) ships too", LAST && LAST.npcs.length >= 3 && LAST.w >= 8 && LAST.h >= 6);
check("the Reed-Walk's causeway now climbs up to the Wall", (REED.exits || []).some(x => x.to === "lasttorch"));
check("the last torch has a way back down the causeway", (LAST.exits || []).some(x => x.to === "reedwalk"));
check("all four zones are reachable as a connected map", (() => {
  // BFS the zone graph from the market via exits
  const seen = new Set(["market"]); const q = ["market"];
  while (q.length) { const z = q.shift(); (SCENES[z].exits || []).forEach(x => { if (!seen.has(x.to)) { seen.add(x.to); q.push(x.to); } }); }
  return seen.has("reedwalk") && seen.has("underbridge") && seen.has("lasttorch");
})());
const LBL = E.buildBlocked(LAST);
check("Last-Torch NPCs/props block their tiles and stay reachable", LAST.npcs.every(n => {
  if (LBL[E.tileKey(n.x, n.y)] !== 1) return false;
  const adj = E.nearestFreeAdjacent(LAST.playerStart.x, LAST.playerStart.y, n.x, n.y, LBL, LAST.w, LAST.h);
  return adj && ((adj[0] === LAST.playerStart.x && adj[1] === LAST.playerStart.y) ||
    E.findPath(LAST.playerStart.x, LAST.playerStart.y, adj[0], adj[1], LBL, LAST.w, LAST.h).length > 0);
}));
check("the Wall zone grows the prop vocabulary (wall, torch, greywort)",
  h.includes('p.type==="wall"') && h.includes('p.type==="torch"') && h.includes('p.type==="greywort"'));

const hale = CONVS.find(c => c.id === "lt.hale");
const esuele = CONVS.find(c => c.id === "lt.esuele");
const goodwin = CONVS.find(c => c.id === "lt.goodwin");
check("the three Last-Torch souls are present", hale && esuele && goodwin);
// PAYOFF 1 — Wick's cure: the greywort errand is gated on having learned of the root under the bridge, and ends by setting the cure flag
const greyChoice = hale.nodes.find(n => n.id === "1").choices.find(ch => ch.when && ch.when.flag === "under.knows_root");
check("the greywort errand is gated on having learned of the root at the Underbridge", greyChoice && greyChoice.when && greyChoice.when.flag === "under.knows_root" &&
  E.choiceAvailable(goodGuy, (() => { const s = E.newState(); s.bools["under.knows_root"] = true; return s; })(), greyChoice, MODEL) === true &&
  E.choiceAvailable(goodGuy, E.newState(), greyChoice, MODEL) === false);
const cureSet = (conv, flag) => conv.nodes.some(n => (n.effects || []).some(e => e.key === flag) ||
  (n.choices || []).some(ch => (ch.effects || []).some(e => e.key === flag)));
check("fetching greywort writes Wick's cure back into the shared state", cureSet(hale, "under.wick_cure"));
// PAYOFF 2 — the Knotwife's lost daughter: Esuele greets you differently once you carry her mother's grief, and you can give it back
const es0 = esuele.nodes.find(n => n.id === "0");
const knewMum = E.newState(); knewMum.bools["under.knotwife_story"] = true;
check("the soul in the Wall reacts to your having met her mother the weaver", es0.variants &&
  E.pickVariantText(es0, goodGuy, knewMum) !== E.pickVariantText(es0, goodGuy, E.newState()) &&
  /wool|mother|grief/i.test(E.pickVariantText(es0, goodGuy, knewMum)));
const tellMum = esuele.nodes.find(n => n.id === "1").choices.find(ch => ch.when && ch.when.flags && ch.when.flags.indexOf("under.knotwife_story") >= 0);
check("you can only tell Esuele her mother never forgot her if you heard the Knotwife's grief", tellMum &&
  E.choiceAvailable(goodGuy, knewMum, tellMum, MODEL) === true && E.choiceAvailable(goodGuy, E.newState(), tellMum, MODEL) === false);
// PAYOFF 3 — Goodwin, the dead man who doesn't know it: the deep stack (crit/fumble + a Returned line) holds for the comic-morose soul too
check("Last-Torch checks carry their own nat-20/nat-1 scenes", [hale, goodwin].every(c =>
  c.nodes.some(n => (n.choices || []).some(ch => ch.crit && ch.fumble))));
check("each Last-Torch soul offers a [RETURNED] line", [hale, esuele, goodwin].every(c =>
  c.nodes.some(n => (n.choices || []).some(ch => ch.tag === "returned"))));
// the Wall itself reacts to a Returned walking up to it, and the greywort can be picked cold-handed
check("a Returned can pick the greywort without braving the cold (a bespoke [RETURNED] gather)",
  hale.nodes.some(n => (n.choices || []).some(ch => ch.tag === "returned" && /greywort_returned/.test(ch.next || ""))));
check("the marquee moment lands: the Wall stirs when a Returned addresses it", (() => {
  const wallStir = esuele.nodes.find(n => n.id === "esuele_returned");
  return wallStir && (wallStir.effects || []).some(e => e.key === "lt.wall_stirs");
})());

// ---- fifth zone: the Lamplit Quarter — two NEW interaction types: recruiting a companion, and a world that gossips ----
const CITY = SCENES && SCENES.lamplit;
check("a fifth zone (the Lamplit Quarter) ships too", CITY && CITY.npcs.length >= 3 && CITY.w >= 8 && CITY.h >= 6);
check("the market opens UP into the living city as well as down to the causeway", (SCN.exits || []).some(x => x.to === "lamplit"));
check("all five zones form one connected map from the market", (() => {
  const seen = new Set(["market"]); const q = ["market"];
  while (q.length) { const z = q.shift(); (SCENES[z].exits || []).forEach(x => { if (SCENES[x.to] && !seen.has(x.to)) { seen.add(x.to); q.push(x.to); } }); }
  return ["reedwalk", "underbridge", "lasttorch", "lamplit"].every(z => seen.has(z));
})());
const CIBL = E.buildBlocked(CITY);
check("Lamplit NPCs/props block their tiles and stay reachable", CITY.npcs.every(n => {
  if (CIBL[E.tileKey(n.x, n.y)] !== 1) return false;
  const adj = E.nearestFreeAdjacent(CITY.playerStart.x, CITY.playerStart.y, n.x, n.y, CIBL, CITY.w, CITY.h);
  return adj && ((adj[0] === CITY.playerStart.x && adj[1] === CITY.playerStart.y) ||
    E.findPath(CITY.playerStart.x, CITY.playerStart.y, adj[0], adj[1], CIBL, CITY.w, CITY.h).length > 0);
}));
check("the city grows the prop vocabulary (lamppost, tavern, table)",
  h.includes('p.type==="lamppost"') && h.includes('p.type==="tavern"') && h.includes('p.type==="table"'));

const dace = CONVS.find(c => c.id === "city.dace");
const mab = CONVS.find(c => c.id === "city.mab");
const pell = CONVS.find(c => c.id === "city.pell");
check("the three Lamplit souls are present", dace && mab && pell);
// NEW INTERACTION 1 — recruiting a companion: a [PERSUASION] pitch that, on success, sets a party flag (and accrues approval)
const recruitChoice = dace.nodes.find(n => n.id === "1").choices.find(ch => (ch.check || {}).skill === "Persuasion");
check("Dace can be recruited as a companion (a pitch that sets a party flag)", recruitChoice && recruitChoice.crit && recruitChoice.fumble &&
  dace.nodes.some(n => (n.effects || []).some(e => e.key === "party.dace_recruited")));
check("recruiting tracks an approval score (a new reactive axis)", dace.nodes.some(n =>
  ((n.effects || []).concat(...(n.choices || []).map(ch => ch.effects || []))).some(e => e.key === "city.dace.approval")));
// NEW INTERACTION 2 — a world that remembers: Mab's welcome reads your deeds across ALL the other zones
const m0 = mab.nodes.find(n => n.id === "0");
const wallSt = E.newState(); wallSt.bools["lt.wall_stirs"] = true;
const savedSt = E.newState(); savedSt.bools["reed.wren_lives"] = true;
const choirSt = E.newState(); choirSt.bools["reed.choir_friend"] = true;
check("the tavern-keep gossips about what you did at the Wall, the river, and with the Choir", m0.variants &&
  new Set([E.pickVariantText(m0, goodGuy, wallSt), E.pickVariantText(m0, goodGuy, savedSt),
    E.pickVariantText(m0, goodGuy, choirSt), E.pickVariantText(m0, goodGuy, E.newState())]).size === 4);
check("a Wall-shaker is greeted as legend; a Choir-friend is greeted with cold suspicion",
  /Wall moved|flinched|spooked/i.test(E.pickVariantText(m0, goodGuy, wallSt)) &&
  /Choir|helpful one|keep that.*off your/i.test(E.pickVariantText(m0, goodGuy, choirSt)));
// the Choir's informant likewise flips on whether you served or crossed the Choir at the river
const p0 = pell.nodes.find(n => n.id === "0");
check("the Choir's ledger-clerk greets a Choir-friend as an asset, distinct from a stranger", p0.variants &&
  E.pickVariantText(p0, goodGuy, choirSt) !== E.pickVariantText(p0, goodGuy, E.newState()) &&
  /helpful|Tallow|friend|service/i.test(E.pickVariantText(p0, goodGuy, choirSt)));
// a recruited companion shows up as backup in a later interaction (party flag read in Pell's tree)
const daceBackup = pell.nodes.find(n => n.id === "1").choices.find(ch => ch.when && ch.when.flag === "party.dace_recruited");
const recruited = E.newState(); recruited.bools["party.dace_recruited"] = true;
check("a recruited companion becomes a usable option in later scenes", daceBackup &&
  E.choiceAvailable(goodGuy, recruited, daceBackup, MODEL) === true && E.choiceAvailable(goodGuy, E.newState(), daceBackup, MODEL) === false);
check("Lamplit checks carry their own nat-20/nat-1 scenes, and each soul a [RETURNED] line",
  [dace, pell].every(c => c.nodes.some(n => (n.choices || []).some(ch => ch.crit && ch.fumble))) &&
  [dace, mab, pell].every(c => c.nodes.some(n => (n.choices || []).some(ch => ch.tag === "returned"))));

// ---- sixth zone: the Counting-House — the Act-1 climax, a confrontation that reads the WHOLE web ----
const HOUSE = SCENES && SCENES.counthouse;
check("a sixth zone (the Counting-House) ships too", HOUSE && HOUSE.npcs.length >= 3 && HOUSE.w >= 8 && HOUSE.h >= 6);
check("the Lamplit Quarter leads deeper in to the Choir's seat", (CITY.exits || []).some(x => x.to === "counthouse"));
check("all six zones form one connected map from the market", (() => {
  const seen = new Set(["market"]); const q = ["market"];
  while (q.length) { const z = q.shift(); (SCENES[z].exits || []).forEach(x => { if (SCENES[x.to] && !seen.has(x.to)) { seen.add(x.to); q.push(x.to); } }); }
  return ["reedwalk", "underbridge", "lasttorch", "lamplit", "counthouse"].every(z => seen.has(z));
})());
const HBL = E.buildBlocked(HOUSE);
check("Counting-House NPCs/props block their tiles and stay reachable", HOUSE.npcs.every(n => {
  if (HBL[E.tileKey(n.x, n.y)] !== 1) return false;
  const adj = E.nearestFreeAdjacent(HOUSE.playerStart.x, HOUSE.playerStart.y, n.x, n.y, HBL, HOUSE.w, HOUSE.h);
  return adj && ((adj[0] === HOUSE.playerStart.x && adj[1] === HOUSE.playerStart.y) ||
    E.findPath(HOUSE.playerStart.x, HOUSE.playerStart.y, adj[0], adj[1], HBL, HOUSE.w, HOUSE.h).length > 0);
}));
check("the Counting-House grows the prop vocabulary (the great scale, desks, a cell)",
  h.includes('p.type==="bigscale"') && h.includes('p.type==="desk"') && h.includes('p.type==="cell"'));

const mereth = CONVS.find(c => c.id === "ch.mereth");
const tallow2 = CONVS.find(c => c.id === "ch.tallow");
const crake = CONVS.find(c => c.id === "ch.crake");
check("the three Counting-House souls are present", mereth && tallow2 && crake);
// the boss confrontation reads the whole web — four DISTINCT openings on what you've done
const me0 = mereth.nodes.find(n => n.id === "0");
const stWall = E.newState(); stWall.bools["lt.wall_stirs"] = true;
const stVenn = E.newState(); stVenn.bools["under.venn_revealed"] = true;
const stChoir = E.newState(); stChoir.bools["reed.choir_friend"] = true;
check("Canon Mereth greets a Wall-shaker, a name-bearer, a Choir-friend and a stranger differently", me0.variants &&
  new Set([E.pickVariantText(me0, goodGuy, stWall), E.pickVariantText(me0, goodGuy, stVenn),
    E.pickVariantText(me0, goodGuy, stChoir), E.pickVariantText(me0, goodGuy, E.newState())]).size === 4 &&
  /Venn/i.test(E.pickVariantText(me0, goodGuy, stVenn)));
// the confrontation gates whole arguments on cross-zone achievements (Venn, the Wall, the unpicking-pattern, a sworn sword)
const m1 = mereth.nodes.find(n => n.id === "1");
const vennArg = m1.choices.find(ch => ch.when && ch.when.flags && ch.when.flags.indexOf("under.venn_revealed") >= 0);
const wallArg = m1.choices.find(ch => ch.when && ch.when.flags && ch.when.flags.indexOf("lt.wall_stirs") >= 0);
const swordArg = m1.choices.find(ch => ch.when && ch.when.flags && ch.when.flags.indexOf("party.dace_recruited") >= 0);
check("expose-Venn, the-Wall-flinched, and bring-a-sword are each gated on the deed that earns them", vennArg && wallArg && swordArg &&
  E.choiceAvailable(scholar, stVenn, vennArg, MODEL) === true && E.choiceAvailable(scholar, E.newState(), vennArg, MODEL) === false &&
  E.choiceAvailable(goodGuy, (() => { const s = E.newState(); s.bools["party.dace_recruited"] = true; return s; })(), swordArg, MODEL) === true &&
  E.choiceAvailable(goodGuy, E.newState(), swordArg, MODEL) === false);
// the crux offers multiple resolutions (appeal / unpick / together / chaos), and the unpick path is itself gated on carrying the pattern
const crux = mereth.nodes.find(n => n.id === "mereth_crux");
const unpickPath = crux.choices.find(ch => ch.when && ch.when.flags && ch.when.flags.indexOf("under.unpicking_pattern") >= 0);
check("the climax offers several real resolutions, with the unpick path gated on the weaver's pattern",
  crux.choices.length >= 3 && unpickPath &&
  E.choiceAvailable(goodGuy, (() => { const s = E.newState(); s.bools["under.unpicking_pattern"] = true; return s; })(), unpickPath, MODEL) === true &&
  E.choiceAvailable(goodGuy, E.newState(), unpickPath, MODEL) === false);
check("at least one resolution allies the Canon and begins to crack the Wall", mereth.nodes.some(n =>
  (n.effects || []).some(e => e.key === "ch.mereth_allied")));
// Tallow (the market watcher) and Crake (a marked prisoner) round out the climax with the deep stack
check("Tallow's market betrayal/rescue is remembered at his Choir desk", tallow2.nodes.find(n => n.id === "0").variants.some(v =>
  v.when && v.when.flags && (v.when.flags.indexOf("reed.betrayed_wren") >= 0 || v.when.flags.indexOf("reed.wren_lives") >= 0)));
check("you can free the prisoner Crake (a gated Athletics escape with crit/fumble)", crake.nodes.find(n => n.id === "1").choices.some(ch =>
  (ch.check || {}).skill === "Athletics" && ch.crit && ch.fumble));
check("Counting-House souls keep crit/fumble comedy and a [RETURNED] line each",
  [mereth, tallow2, crake].every(c => c.nodes.some(n => (n.choices || []).some(ch => ch.crit && ch.fumble))) &&
  [mereth, tallow2, crake].every(c => c.nodes.some(n => (n.choices || []).some(ch => ch.tag === "returned"))));

// ---- seventh zone: the Hearth — a denouement, and a NEW engine feature: conditionally-present souls ----
const CAMP = SCENES && SCENES.hearth;
check("a seventh zone (the Hearth) ships too", CAMP && CAMP.npcs.length >= 3 && CAMP.w >= 8 && CAMP.h >= 6);
check("the market opens to a campsite (make camp)", (SCN.exits || []).some(x => x.to === "hearth"));
check("all seven zones form one connected map from the market", (() => {
  const seen = new Set(["market"]); const q = ["market"];
  while (q.length) { const z = q.shift(); (SCENES[z].exits || []).forEach(x => { if (SCENES[x.to] && !seen.has(x.to)) { seen.add(x.to); q.push(x.to); } }); }
  return ["reedwalk", "underbridge", "lasttorch", "lamplit", "counthouse", "hearth"].every(z => seen.has(z));
})());
check("the Hearth grows the prop vocabulary (campfire, log, bedroll, tree)",
  h.includes('p.type==="campfire"') && h.includes('p.type==="log"') && h.includes('p.type==="bedroll"') && h.includes('p.type==="tree"'));

// NEW ENGINE FEATURE — conditionally-present NPCs: souls with a `when` only appear when world-state matches
check("the engine filters NPCs by world-state (npcVisible / activeNpcs)",
  h.includes("function npcVisible(") && h.includes("function activeNpcs(") && h.includes("matchesWhen(char, st, n.when)"));
check("collision + rendering use only the present souls", h.includes("activeNpcs()") &&
  h.includes("function blockedNow(") && h.includes("BLOCKED=blockedNow()"));
const camped = CAMP.npcs.filter(n => n.when);
check("the Hearth's attendees are gated on who you saved (Dace recruited, Wren alive, Pip helped)",
  camped.length >= 3 &&
  CAMP.npcs.some(n => n.when && n.when.flag === "party.dace_recruited") &&
  CAMP.npcs.some(n => n.when && n.when.flag === "reed.wren_lives") &&
  CAMP.npcs.some(n => n.when && n.when.flag === "market.helped_pip"));
check("an always-present anchor (the fire) remains so the camp is never empty", CAMP.npcs.some(n => !n.when));
// verify the gate actually works through the real matcher: Dace present only when recruited
const daceNpc = CAMP.npcs.find(n => n.id === "dace3");
check("a conditional soul is hidden until its flag is set, then appears", daceNpc &&
  E.matchesWhen(goodGuy, (() => { const s = E.newState(); s.bools["party.dace_recruited"] = true; return s; })(), daceNpc.when) === true &&
  E.matchesWhen(goodGuy, E.newState(), daceNpc.when) === false);

const fire = CONVS.find(c => c.id === "hearth.fire");
const hdace = CONVS.find(c => c.id === "hearth.dace");
const hwren = CONVS.find(c => c.id === "hearth.wren");
const hpip = CONVS.find(c => c.id === "hearth.pip");
check("the Hearth's four conversations are present", fire && hdace && hwren && hpip);
// the fire's reflection reads your reckoning — a haunted, merciful and ruthless soul each take stock differently
const f0 = fire.nodes.find(n => n.id === "0");
const reck = (k) => { const s = E.newState(); s.ints[k] = 3; return s; };
check("the fire's denouement reads your dominant disposition", f0.variants &&
  new Set([E.pickVariantText(f0, goodGuy, reck("disp.haunted")), E.pickVariantText(f0, goodGuy, reck("disp.merciful")),
    E.pickVariantText(f0, goodGuy, reck("disp.ruthless")), E.pickVariantText(f0, goodGuy, E.newState())]).size === 4);
// the companion night-talk deepens with approval (an approval-gated layer — the camp/bonding interaction)
const deepTalk = hdace.nodes.find(n => n.id === "1").choices.find(ch => ch.when && ch.when.int && ch.when.int["city.dace.approval"]);
check("Dace's night-talk unlocks a deeper line once her approval is high enough", deepTalk &&
  E.choiceAvailable(confessor, (() => { const s = E.newState(); s.ints["city.dace.approval"] = 3; return s; })(), deepTalk, MODEL) === true &&
  E.choiceAvailable(confessor, E.newState(), deepTalk, MODEL) === false);
check("each Hearth soul still offers a [RETURNED] line", [fire, hdace, hwren, hpip].every(c =>
  c.nodes.some(n => (n.choices || []).some(ch => ch.tag === "returned"))));
// the recruited cartographer joins the fire too — the third companion appears in the denouement when taken on
const hsennet = CONVS.find(c => c.id === "hearth.sennet");
const sennetNpc = CAMP.npcs.find(n => n.conv === "hearth.sennet");
check("Sennet appears at the Hearth only once recruited (party.sennet_recruited)", hsennet && sennetNpc &&
  sennetNpc.when && sennetNpc.when.flag === "party.sennet_recruited" &&
  E.matchesWhen(goodGuy, (() => { const s = E.newState(); s.bools["party.sennet_recruited"] = true; return s; })(), sennetNpc.when) === true &&
  E.matchesWhen(goodGuy, E.newState(), sennetNpc.when) === false);
check("Sennet's fireside talk reads the recruit (a devoted variant) and carries a [RETURNED] line + a Returned-sense",
  hsennet.nodes.find(n => n.id === "0").variants.some(v => v.when && v.when.flag === "cg.sennet_devoted") &&
  hsennet.nodes.some(n => (n.choices || []).some(ch => ch.tag === "returned")) && hsennet.returned);

// ---- eighth zone: the Doomguide's Table — the bridge into the main saga, the keystone confrontation ----
const TABLE = SCENES && SCENES.aldric;
check("an eighth zone (the Doomguide's Table) ships too", TABLE && TABLE.npcs.length >= 3 && TABLE.w >= 8 && TABLE.h >= 6);
check("the Lamplit Quarter carries a summons to Aldric's table", (CITY.exits || []).some(x => x.to === "aldric"));
check("all eight zones form one connected map from the market", (() => {
  const seen = new Set(["market"]); const q = ["market"];
  while (q.length) { const z = q.shift(); (SCENES[z].exits || []).forEach(x => { if (SCENES[x.to] && !seen.has(x.to)) { seen.add(x.to); q.push(x.to); } }); }
  return ["reedwalk", "underbridge", "lasttorch", "lamplit", "counthouse", "hearth", "aldric"].every(z => seen.has(z));
})());
const TBL = E.buildBlocked(TABLE);
check("Aldric's-Table NPCs/props block their tiles and stay reachable", TABLE.npcs.every(n => {
  if (TBL[E.tileKey(n.x, n.y)] !== 1) return false;
  const adj = E.nearestFreeAdjacent(TABLE.playerStart.x, TABLE.playerStart.y, n.x, n.y, TBL, TABLE.w, TABLE.h);
  return adj && ((adj[0] === TABLE.playerStart.x && adj[1] === TABLE.playerStart.y) ||
    E.findPath(TABLE.playerStart.x, TABLE.playerStart.y, adj[0], adj[1], TBL, TABLE.w, TABLE.h).length > 0);
}));
check("the Doomguide's Table grows the prop vocabulary (tea table, chair, hearthfire, bookshelf)",
  h.includes('p.type==="teatable"') && h.includes('p.type==="chair"') && h.includes('p.type==="hearthfire"') && h.includes('p.type==="bookshelf"'));

const aldric = CONVS.find(c => c.id === "ald.aldric");
const wessa = CONVS.find(c => c.id === "ald.wessa");
const eithne = CONVS.find(c => c.id === "ald.eithne");
check("the three souls at the table are present (Aldric, his herald, his unseen daughter)", aldric && wessa && eithne);
// the keystone confrontation reads the Act-1 web — distinct openings for an ally-of-the-Canon, a Choir-friend, a Kelemvorite, and a stranger
const a0 = aldric.nodes.find(n => n.id === "0");
const stCanon = E.newState(); stCanon.bools["ch.mereth_allied"] = true;
const stChoirF = E.newState(); stChoirF.bools["reed.choir_friend"] = true;
const kelGuy2 = { ...goodGuy, deity: "Kelemvor" };
check("Aldric greets the Canon's ally, a Choir-friend, a Kelemvorite and a stranger differently", a0.variants &&
  new Set([E.pickVariantText(a0, goodGuy, stCanon), E.pickVariantText(a0, goodGuy, stChoirF),
    E.pickVariantText(a0, kelGuy2, E.newState()), E.pickVariantText(a0, goodGuy, E.newState())]).size === 4);
// showing him the gentle road is gated on having actually cracked the Canon
const a1 = aldric.nodes.find(n => n.id === "1");
const gentleOpt = a1.choices.find(ch => ch.when && ch.when.flags && ch.when.flags.indexOf("ch.mereth_allied") >= 0);
check("you can only show Aldric the gentle road if you cracked the Canon", gentleOpt &&
  E.choiceAvailable(goodGuy, stCanon, gentleOpt, MODEL) === true && E.choiceAvailable(goodGuy, E.newState(), gentleOpt, MODEL) === false);
// the crux offers real divergent endings, and the reunion path is gated on his having come to suspect the thread
const acrux = aldric.nodes.find(n => n.id === "ald_crux");
const reunionOpt = acrux.choices.find(ch => ch.when && ch.when.flags && ch.when.flags.indexOf("ald.suspects_thread") >= 0);
check("the keystone crux offers divergent endings, with the daughter-reunion gated on suspecting the thread",
  acrux.choices.length >= 3 && reunionOpt &&
  E.choiceAvailable(goodGuy, (() => { const s = E.newState(); s.bools["ald.suspects_thread"] = true; return s; })(), reunionOpt, MODEL) === true &&
  E.choiceAvailable(goodGuy, E.newState(), reunionOpt, MODEL) === false);
check("at least one resolution allies Aldric and makes him put down the matches", aldric.nodes.some(n =>
  (n.effects || []).some(e => e.key === "ald.matches_down")));
// the daughter only the Returned can see — her whole conversation, and a [RETURNED] line, exist
check("the unseen daughter has the lowest sense-DC of any soul (she wants to be seen)", eithne.returned && eithne.returned.dc <= 7);
check("Eithne's plea (that her father stop) can be carried to Aldric — the threads connect",
  eithne.nodes.some(n => (n.effects || []).some(e => e.key === "ald.eithne_plea")) &&
  aldric.nodes.some(n => (n.effects || []).some(e => e.key === "ald.eithne_heard")));
check("the table's souls keep crit/fumble where rolled, and a [RETURNED] line each",
  [aldric, wessa, eithne].every(c => c.nodes.some(n => (n.choices || []).some(ch => ch.tag === "returned"))));
// the Crown of Horns — the saga's central object — is named and explained here, bridging to the main quest
check("the Crown of Horns is revealed at the table (the bridge into the main campaign)", aldric.nodes.some(n =>
  (n.effects || []).some(e => e.key === "ald.crown_revealed")) && /Crown of Horns/.test(JSON.stringify(aldric)));

// ---- ninth zone: the Grey Wayshrine — the Judge's own voice, the deep cosmology, and a god's uncertainty ----
const SHRINE = SCENES && SCENES.wayshrine;
check("a ninth zone (the Grey Wayshrine) ships too", SHRINE && SHRINE.npcs.length >= 3 && SHRINE.w >= 8 && SHRINE.h >= 6);
check("the road past camp leads on to the wayshrine", (CAMP.exits || []).some(x => x.to === "wayshrine"));
check("all nine zones form one connected map from the market", (() => {
  const seen = new Set(["market"]); const q = ["market"];
  while (q.length) { const z = q.shift(); (SCENES[z].exits || []).forEach(x => { if (SCENES[x.to] && !seen.has(x.to)) { seen.add(x.to); q.push(x.to); } }); }
  return ["reedwalk", "underbridge", "lasttorch", "lamplit", "counthouse", "hearth", "aldric", "wayshrine"].every(z => seen.has(z));
})());
const SHBL = E.buildBlocked(SHRINE);
check("Wayshrine NPCs/props block their tiles and stay reachable", SHRINE.npcs.every(n => {
  if (SHBL[E.tileKey(n.x, n.y)] !== 1) return false;
  const adj = E.nearestFreeAdjacent(SHRINE.playerStart.x, SHRINE.playerStart.y, n.x, n.y, SHBL, SHRINE.w, SHRINE.h);
  return adj && ((adj[0] === SHRINE.playerStart.x && adj[1] === SHRINE.playerStart.y) ||
    E.findPath(SHRINE.playerStart.x, SHRINE.playerStart.y, adj[0], adj[1], SHBL, SHRINE.w, SHRINE.h).length > 0);
}));
check("the Wayshrine grows the prop vocabulary (the grey shrine, milestones, cairns)",
  h.includes('p.type==="greyshrine"') && h.includes('p.type==="milestone"') && h.includes('p.type==="cairn"'));

const harb = CONVS.find(c => c.id === "way.harbinger");
const orin = CONVS.find(c => c.id === "way.orin");
const doget = CONVS.find(c => c.id === "way.doget");
check("the three souls at the shrine are present (the Judge's aspect, his sword, a fair death)", harb && orin && doget);
// the Harbinger's opening reads which campaign-road you took (take the Crown / file the appeal / godless / stranger)
const h0 = harb.nodes.find(n => n.id === "0");
const stCrown = E.newState(); stCrown.bools["ald.path_crown"] = true;
const stAppeal = E.newState(); stAppeal.bools["ald.path_appeal"] = true;
const faithlessSoul2 = { ...goodGuy, deity: "None" };
const goddedStranger = { ...goodGuy, deity: "Oghma" };
check("the Judge's aspect greets a Crown-taker, an appeal-filer, the godless and a stranger differently", h0.variants &&
  new Set([E.pickVariantText(h0, goddedStranger, stCrown), E.pickVariantText(h0, goddedStranger, stAppeal),
    E.pickVariantText(h0, faithlessSoul2, E.newState()), E.pickVariantText(h0, goddedStranger, E.newState())]).size === 4);
// the deep cosmology — the Concord and the Unmade — is revealed here (the why under the whole saga)
check("the Concord and the Unmade (the seawall, the Hunger) are revealed at the shrine", harb.nodes.some(n =>
  (n.effects || []).some(e => e.key === "way.concord_revealed") && (n.effects || []).some(e => e.key === "way.unmade_revealed")) ||
  (harb.nodes.some(n => (n.effects || []).some(e => e.key === "way.concord_revealed")) &&
   harb.nodes.some(n => (n.effects || []).some(e => e.key === "way.unmade_revealed"))));
// the crux poses the question under the saga (what about the Hunger?) with divergent answers, one gated on the Judge's own question
const hcrux = harb.nodes.find(n => n.id === "harb_crux");
const witnessAns = hcrux.choices.find(ch => ch.when && ch.when.flags && ch.when.flags.indexOf("way.the_question_asked") >= 0);
check("the shrine's crux offers divergent answers, one gated on having been asked the Judge's question",
  hcrux.choices.length >= 3 && witnessAns &&
  E.choiceAvailable(goodGuy, (() => { const s = E.newState(); s.bools["way.the_question_asked"] = true; return s; })(), witnessAns, MODEL) === true &&
  E.choiceAvailable(goodGuy, E.newState(), witnessAns, MODEL) === false);
check("a wise answer allies the Judge's aspect; a reckless one earns a warning", harb.nodes.some(n =>
  (n.effects || []).some(e => e.key === "way.harbinger_allied")) && harb.nodes.some(n =>
  (n.effects || []).some(e => e.key === "way.harbinger_warned")));
// the Justiciar can be turned from arrest to alliance — the order's own sword, questioning
check("the Doomguard sword can be talked from arrest toward joining you", orin.nodes.some(n =>
  (n.effects || []).some(e => e.key === "party.orin_may_join")));
check("the Wayshrine souls keep crit/fumble where rolled and a [RETURNED] line each",
  orin.nodes.some(n => (n.choices || []).some(ch => ch.crit && ch.fumble)) &&
  [harb, orin, doget].every(c => c.nodes.some(n => (n.choices || []).some(ch => ch.tag === "returned"))));

// ---- tenth zone: the Threshold of the Crown's Road — the mirror, the loop, the capstone ----
const THRESH = SCENES && SCENES.threshold;
check("a tenth zone (the Threshold) ships too", THRESH && THRESH.npcs.length >= 3 && THRESH.w >= 8 && THRESH.h >= 6);
check("the road ends at the Threshold (the wayshrine leads on to it)", (SHRINE.exits || []).some(x => x.to === "threshold"));
check("all ten zones form one connected map from the market", (() => {
  const seen = new Set(["market"]); const q = ["market"];
  while (q.length) { const z = q.shift(); (SCENES[z].exits || []).forEach(x => { if (SCENES[x.to] && !seen.has(x.to)) { seen.add(x.to); q.push(x.to); } }); }
  return ["reedwalk", "underbridge", "lasttorch", "lamplit", "counthouse", "hearth", "aldric", "wayshrine", "threshold"].every(z => seen.has(z));
})());
check("the Threshold grows the prop vocabulary (the death-door)", h.includes('p.type==="deathdoor"'));

const last = CONVS.find(c => c.id === "thr.last");
check("the Last Returned (your future self) is present at the door", last);
// the conditional companions follow you to the edge (the conditional-NPC system, used for party)
const camped2 = THRESH.npcs.filter(n => n.when);
check("companions who joined you appear at the edge (Dace if recruited, Orin if she joined)", camped2.length >= 2 &&
  THRESH.npcs.some(n => n.when && n.when.flag === "party.dace_recruited") &&
  THRESH.npcs.some(n => n.when && n.when.flag === "party.orin_may_join"));
// the recruited cartographer follows you to death's door — and cannot cross (the saga's most pointed companion beat)
const thrSennet = CONVS.find(c => c.id === "thr.sennet");
const thrSennetNpc = THRESH.npcs.find(n => n.conv === "thr.sennet");
check("Sennet follows to the Threshold only if recruited, and cannot cross (gated on party.sennet_recruited)", thrSennet && thrSennetNpc &&
  thrSennetNpc.when && thrSennetNpc.when.flag === "party.sennet_recruited" &&
  E.matchesWhen(goodGuy, (() => { const s = E.newState(); s.bools["party.sennet_recruited"] = true; return s; })(), thrSennetNpc.when) === true &&
  E.matchesWhen(goodGuy, E.newState(), thrSennetNpc.when) === false);
check("at the edge you can hold Sennet back from the door, or make them your eyes past it (a [RETURNED] beat)", thrSennet &&
  thrSennet.nodes.some(n => (n.effects || []).some(e => e.key === "thr.sennet_holds")) &&
  thrSennet.nodes.some(n => (n.choices || []).some(ch => ch.tag === "returned" && /eyes/.test(ch.next || ""))) && thrSennet.returned);
// the Sparrow is the one companion who CAN cross — a dead soul, the exception the death-door makes
const thrSparrow = CONVS.find(c => c.id === "thr.sparrow");
const thrSparrowNpc = THRESH.npcs.find(n => n.conv === "thr.sparrow");
check("the Sparrow follows to the Threshold only if recruited — and, being dead, can actually cross", thrSparrow && thrSparrowNpc &&
  thrSparrowNpc.when && thrSparrowNpc.when.flag === "party.sparrow_recruited" &&
  E.matchesWhen(goodGuy, (() => { const s = E.newState(); s.bools["party.sparrow_recruited"] = true; return s; })(), thrSparrowNpc.when) === true &&
  E.matchesWhen(goodGuy, E.newState(), thrSparrowNpc.when) === false);
check("at the door you can take her through (she scouts the way out) or have her hold it — a real fork", thrSparrow &&
  thrSparrow.nodes.some(n => (n.effects || []).some(e => e.key === "thr.sparrow_comes")) &&
  thrSparrow.nodes.some(n => (n.effects || []).some(e => e.key === "thr.sparrow_holds")) &&
  thrSparrow.nodes.some(n => (n.choices || []).some(ch => ch.tag === "returned")) && thrSparrow.returned);
// the mirror's opening reads which road you're on — a witness-answerer, a reckless-answerer, a Crown-taker, an accompanied soul, and a lone stranger
const l0 = last.nodes.find(n => n.id === "0");
const stWit = E.newState(); stWit.bools["way.named_witness"] = true;
const stRk = E.newState(); stRk.bools["way.answer_reckless"] = true;
const stCr = E.newState(); stCr.bools["ald.path_crown"] = true;
const stAnchor = E.newState(); stAnchor.bools["thr.has_anchor"] = true;
check("your future self greets you differently by the road you've walked (witness/reckless/Crown/anchored/alone)", l0.variants &&
  new Set([E.pickVariantText(l0, goodGuy, stWit), E.pickVariantText(l0, goodGuy, stRk),
    E.pickVariantText(l0, goodGuy, stCr), E.pickVariantText(l0, goodGuy, stAnchor),
    E.pickVariantText(l0, goodGuy, E.newState())]).size === 5);
// the loop — the masks are the same soul, looping — is revealed here, the saga's deepest twist
check("the loop (the masks are the same Returned, looping) is revealed at the door", last.nodes.some(n =>
  (n.effects || []).some(e => e.key === "thr.loop_revealed")));
// the crux offers ways to break the loop, two gated on what you carry (an anchor / knowing the loop)
const lcrux = last.nodes.find(n => n.id === "last_crux");
const nameOpt = lcrux.choices.find(ch => ch.when && ch.when.flags && ch.when.flags.indexOf("thr.has_anchor") >= 0);
const handOpt = lcrux.choices.find(ch => ch.when && ch.when.flags && ch.when.flags.indexOf("thr.loop_revealed") >= 0);
check("the capstone crux offers loop-breaking answers, gated on your anchor and on knowing the loop", lcrux.choices.length >= 3 && nameOpt && handOpt &&
  E.choiceAvailable(goodGuy, stAnchor, nameOpt, MODEL) === true && E.choiceAvailable(goodGuy, E.newState(), nameOpt, MODEL) === false &&
  E.choiceAvailable(goodGuy, (() => { const s = E.newState(); s.bools["thr.loop_revealed"] = true; return s; })(), handOpt, MODEL) === true);
check("breaking the loop lays your future self to rest", last.nodes.some(n =>
  (n.effects || []).some(e => e.key === "thr.loop_broken")) && last.nodes.some(n =>
  (n.effects || []).some(e => e.key === "thr.last_at_rest")));
check("your future self has the lowest sense-DC of all (it is literally you)", last.returned && last.returned.dc <= 3);
check("the Threshold companions and the mirror each carry a [RETURNED] line", THRESH.npcs.every(n => {
  const c = CONVS.find(cc => cc.id === n.conv);
  return c && c.nodes.some(nd => (nd.choices || []).some(ch => ch.tag === "returned"));
}));

// ---- eleventh zone: the Night Market — breadth + a new interaction type: barter/trade ----
const NIGHT = SCENES && SCENES.nightmarket;
check("an eleventh zone (the Night Market) ships too", NIGHT && NIGHT.npcs.length >= 3 && NIGHT.w >= 8 && NIGHT.h >= 6);
check("a hidden stair from the market reaches the Night Market", (SCN.exits || []).some(x => x.to === "nightmarket"));
check("all eleven zones form one connected map from the market", (() => {
  const seen = new Set(["market"]); const q = ["market"];
  while (q.length) { const z = q.shift(); (SCENES[z].exits || []).forEach(x => { if (SCENES[x.to] && !seen.has(x.to)) { seen.add(x.to); q.push(x.to); } }); }
  return ["reedwalk", "underbridge", "lasttorch", "lamplit", "counthouse", "hearth", "aldric", "wayshrine", "threshold", "nightmarket"].every(z => seen.has(z));
})());
const NBL = E.buildBlocked(NIGHT);
check("Night-Market NPCs/props block their tiles and stay reachable", NIGHT.npcs.every(n => {
  if (NBL[E.tileKey(n.x, n.y)] !== 1) return false;
  const adj = E.nearestFreeAdjacent(NIGHT.playerStart.x, NIGHT.playerStart.y, n.x, n.y, NBL, NIGHT.w, NIGHT.h);
  return adj && ((adj[0] === NIGHT.playerStart.x && adj[1] === NIGHT.playerStart.y) ||
    E.findPath(NIGHT.playerStart.x, NIGHT.playerStart.y, adj[0], adj[1], NBL, NIGHT.w, NIGHT.h).length > 0);
}));

const pawn = CONVS.find(c => c.id === "nm.pawn");
const mnemo = CONVS.find(c => c.id === "nm.mnemo");
const regular = CONVS.find(c => c.id === "nm.regular");
check("the three Night-Market vendors are present", pawn && mnemo && regular);
// NEW INTERACTION — barter/trade: an accumulating resource (years given) spent and gated, all on the existing int engine
const giveYear = pawn.nodes.find(n => n.id === "1").choices.find(ch =>
  (ch.effects || []).some(e => e.key === "nm.years_given" && e.op === "AddInt" && e.amount > 0));
check("you can trade a resource (give a year) that accumulates in the visit state", giveYear);
const deepGoods = pawn.nodes.find(n => n.id === "1").choices.find(ch => ch.when && ch.when.int && ch.when.int["nm.years_given"]);
const spent3 = E.newState(); spent3.ints["nm.years_given"] = 3;
check("the back-of-stall merchandise unlocks only once you've spent enough (a real shop gate)", deepGoods &&
  E.choiceAvailable(goodGuy, spent3, deepGoods, MODEL) === true && E.choiceAvailable(goodGuy, E.newState(), deepGoods, MODEL) === false);
check("the traded resource is surfaced in the ledger (an INT_LABEL)", h.includes("years you've traded away") && h.includes("nm.years_given"));
// the zone keeps the deep stack and its own idiosyncratic theme (the market is a trap that eats souls a trade at a time)
check("the Night Market warns it is a trap that spends souls (the Regular, the keeper)", regular.nodes.some(n =>
  (n.effects || []).some(e => e.key === "nm.market_warning")) || mnemo.nodes.some(n =>
  (n.effects || []).some(e => e.key === "nm.market_warning")));
check("a witness can carry a spent soul out by remembering it (ties to the saga's core)", regular.nodes.some(n =>
  (n.effects || []).some(e => e.key === "nm.witnessed_regular")));
check("each Night-Market vendor offers a [RETURNED] line", [pawn, mnemo, regular].every(c =>
  c.nodes.some(n => (n.choices || []).some(ch => ch.tag === "returned"))));

// ---- twelfth zone: the Vault of Tens — a puzzle interaction where wit beats correctness ----
const VAULT = SCENES && SCENES.vault;
check("a twelfth zone (the Vault of Tens) ships too", VAULT && VAULT.npcs.length >= 3);
check("a hidden way from the Underbridge reaches the Vault", (SCENES.underbridge.exits || []).some(x => x.to === "vault"));
const vWarden = CONVS.find(c => c.id === "vault.warden");
check("the Vault's riddle-game rewards wit over correctness (the clever-wrong answer wins; the correct one petrifies)", vWarden && (() => {
  const r = vWarden.nodes.find(n => n.id === "1");
  const correct = r.choices.find(ch => /\bTime\b/.test(ch.text) && ch.next === "warden_correct");
  const witty = r.choices.find(ch => ch.next === "warden_witty");
  const stoneNode = vWarden.nodes.find(n => n.id === "warden_correct");
  return correct && witty && stoneNode && (stoneNode.effects || []).some(e => e.key === "vault.stoning");
})());
check("naming the caged soul (Sela) is gated on having learned her name, and breaks the binding", (() => {
  const r = vWarden.nodes.find(n => n.id === "1");
  const selaOpt = r.choices.find(ch => ch.when && ch.when.flags && ch.when.flags.indexOf("vault.knows_sela") >= 0);
  const knows = E.newState(); knows.bools["vault.knows_sela"] = true;
  return selaOpt && E.choiceAvailable(goodGuy, knows, selaOpt, MODEL) === true &&
    E.choiceAvailable(goodGuy, E.newState(), selaOpt, MODEL) === false;
})());
check("the real solve is freeing the prisoner, not taking the hoard", vWarden.nodes.some(n =>
  (n.effects || []).some(e => e.key === "vault.freed_tithe")));

// ---- thirteenth zone: the Court of the Dead — the finale, through the death-door ----
const COURT = SCENES && SCENES.court;
check("a thirteenth zone (the Court of the Dead) ships too", COURT && COURT.npcs.length >= 3);
check("the death-door at the Threshold opens into the Court", (SCENES.threshold.exits || []).some(x => x.to === "court"));
const kelC = CONVS.find(c => c.id === "court.kelemvor");
const crownC = CONVS.find(c => c.id === "court.crown");
check("the finale's three presences are here (the Judge, the Crown, the first appellant)", kelC && crownC && CONVS.find(c => c.id === "court.esuele"));
// the Judge's welcome reads which road you walked here on (loop-broken / appeal / Crown-seeker / stranger)
const k0 = kelC.nodes.find(n => n.id === "0");
const stLoop = E.newState(); stLoop.bools["thr.loop_broken"] = true;
const stApp = E.newState(); stApp.bools["ald.appeal_pledged"] = true;
const stKr = E.newState(); stKr.bools["ald.path_crown"] = true; stKr.bools["way.named_keeper"] = true;
check("Kelemvor greets the loop-breaker, the appeal-bringer, the Crown-seeker and a stranger differently", k0.variants &&
  new Set([E.pickVariantText(k0, goodGuy, stLoop), E.pickVariantText(k0, goodGuy, stApp),
    E.pickVariantText(k0, goodGuy, stKr), E.pickVariantText(k0, goodGuy, E.newState())]).size === 4);
// the Crown of Horns is a temptation that uses your own voice and can be beaten by self-knowledge
check("the Crown tempts in your own voice and can be refused by naming your own weakness", crownC.nodes.some(n =>
  (n.choices || []).some(ch => ch.tag === "returned" && ch.next === "crown_refused")) &&
  crownC.nodes.some(n => (n.effects || []).some(e => e.key === "court.crown_beaten")));
// the finale offers four divergent endings, the appeal one gated on having pledged it
const kcrux = kelC.nodes.find(n => n.id === "kel_crux");
const appealEnd = kcrux.choices.find(ch => ch.when && ch.when.flags && ch.when.flags.indexOf("ald.appeal_pledged") >= 0);
check("the Court's crux offers several endings, the appeal one gated on the road that earned it",
  kcrux.choices.length >= 4 && appealEnd &&
  E.choiceAvailable(goodGuy, stApp, appealEnd, MODEL) === true && E.choiceAvailable(goodGuy, E.newState(), appealEnd, MODEL) === false);
const endingFlags = new Set();
kelC.nodes.forEach(n => (n.choices || []).concat(n).forEach(x => (x.effects || []).forEach(e => { if (/^court\.ending_/.test(e.key)) endingFlags.add(e.key); })));
check("the finale resolves to several distinct endings (witness-wall / appeal / relieve / crown)", endingFlags.size >= 4);
check("taking the Crown is the tragic ending (you become the tyrant); the others resolve the saga", kelC.nodes.some(n =>
  (n.effects || []).some(e => e.key === "court.became_tyrant")) && kelC.nodes.some(n =>
  (n.effects || []).some(e => e.key === "court.saga_resolved")));
check("each Court soul carries a [RETURNED] line", [kelC, crownC, CONVS.find(c => c.id === "court.esuele")].every(c =>
  c.nodes.some(n => (n.choices || []).some(ch => ch.tag === "returned"))));
check("the Court grows the prop vocabulary (the throne)", h.includes('p.type==="throne"'));

// ---- fourteenth zone: Some Years After — the epilogue, paying off the whole saga ----
const EPI = SCENES && SCENES.epilogue;
check("a fourteenth zone (the epilogue) ships too", EPI && EPI.npcs.length >= 3);
check("the Court opens onto the epilogue (the years after)", (COURT.exits || []).some(x => x.to === "epilogue"));
const chron = CONVS.find(c => c.id === "epi.chronicler");
check("the Chronicler frames the after by which ending you chose", (() => {
  const c0 = chron.nodes.find(n => n.id === "0");
  const stWW = E.newState(); stWW.bools["court.ending_witness_wall"] = true;
  const stAp = E.newState(); stAp.bools["court.ending_appeal"] = true;
  const stTy = E.newState(); stTy.bools["court.became_tyrant"] = true;
  return c0.variants && new Set([E.pickVariantText(c0, goodGuy, stWW), E.pickVariantText(c0, goodGuy, stAp),
    E.pickVariantText(c0, goodGuy, stTy), E.pickVariantText(c0, goodGuy, E.newState())]).size === 4;
})());
check("the surviving souls appear in the epilogue only if you saved them (conditional codas)", EPI.npcs.filter(n => n.when).length >= 2 &&
  EPI.npcs.some(n => n.when && n.when.flag === "reed.wren_lives") &&
  EPI.npcs.some(n => n.when && n.when.flag === "market.helped_pip"));
const epiWren = CONVS.find(c => c.id === "epi.wren");
check("years-later Wren's coda exists and pays off her thread (and the thawing mark)", epiWren &&
  epiWren.nodes.some(n => (n.effects || []).some(e => e.key === "epi.wren_thawing")));
// the recruited cartographer's whole arc lands in the epilogue: old, alive, the atlas finished
const epiSennet = CONVS.find(c => c.id === "epi.sennet");
const epiSennetNpc = EPI.npcs.find(n => n.conv === "epi.sennet");
check("Sennet's years-after coda appears only if recruited, paying off the third-companion arc", epiSennet && epiSennetNpc &&
  epiSennetNpc.when && epiSennetNpc.when.flag === "party.sennet_recruited" &&
  E.matchesWhen(goodGuy, (() => { const s = E.newState(); s.bools["party.sennet_recruited"] = true; return s; })(), epiSennetNpc.when) === true &&
  E.matchesWhen(goodGuy, E.newState(), epiSennetNpc.when) === false);
check("the coda reads the threshold choice (eyes-past-the-door variant) and pays off the atlas + the cold receding", epiSennet &&
  epiSennet.nodes.find(n => n.id === "0").variants.some(v => v.when && v.when.flag === "thr.sennet_eyes") &&
  epiSennet.nodes.some(n => (n.effects || []).some(e => e.key === "epi.sennet_atlas_lives")) &&
  epiSennet.nodes.some(n => (n.effects || []).some(e => e.key === "epi.sennet_map_home")));
// the trial pays off years-later too: the spared midwife appears, and her coda reads how the reckoning broke
const epiAnnet = CONVS.find(c => c.id === "epi.annet");
const epiAnnetNpc = EPI.npcs.find(n => n.conv === "epi.annet");
check("Annet's years-after coda appears only if she was spared at the trial", epiAnnet && epiAnnetNpc &&
  epiAnnetNpc.when && epiAnnetNpc.when.flag === "tr.annet_spared" &&
  E.matchesWhen(goodGuy, (() => { const s = E.newState(); s.bools["tr.annet_spared"] = true; return s; })(), epiAnnetNpc.when) === true &&
  E.matchesWhen(goodGuy, E.newState(), epiAnnetNpc.when) === false);
check("the midwife's coda reads HOW the reckoning broke (the reeve's recantation, or the voided Concord)", epiAnnet &&
  epiAnnet.nodes.find(n => n.id === "0").variants.some(v => v.when && (v.when.flags || []).indexOf("tr.reeve_recants") >= 0) &&
  epiAnnet.nodes.find(n => n.id === "0").variants.some(v => v.when && (v.when.flags || []).indexOf("tr.concord_void") >= 0) &&
  epiAnnet.returned);
// the Sparrow's years-after coda — a dead companion who turned "steal people back" into a vocation
const epiSparrow = CONVS.find(c => c.id === "epi.sparrow");
const epiSparrowNpc = EPI.npcs.find(n => n.conv === "epi.sparrow");
check("the Sparrow's years-after coda appears only if recruited, paying off the fourth-companion arc", epiSparrow && epiSparrowNpc &&
  epiSparrowNpc.when && epiSparrowNpc.when.flag === "party.sparrow_recruited" &&
  E.matchesWhen(goodGuy, (() => { const s = E.newState(); s.bools["party.sparrow_recruited"] = true; return s; })(), epiSparrowNpc.when) === true &&
  E.matchesWhen(goodGuy, E.newState(), epiSparrowNpc.when) === false);
check("her coda reads whether she scouted the throne-room exit, and pays off the Sparrow's Wing", epiSparrow &&
  epiSparrow.nodes.find(n => n.id === "0").variants.some(v => v.when && (v.when.flags || []).indexOf("thr.sparrow_scouts") >= 0) &&
  epiSparrow.nodes.some(n => (n.effects || []).some(e => e.key === "epi.sparrow_wing")) && epiSparrow.returned);
check("each epilogue soul carries a [RETURNED] line", EPI.npcs.every(n => {
  const c = CONVS.find(cc => cc.id === n.conv);
  return c && c.nodes.some(nd => (nd.choices || []).some(ch => ch.tag === "returned"));
}));

// ---- fifteenth zone: the Weeping House — a self-contained investigation/mystery side quest ----
const WEEP = SCENES && SCENES.weeping;
check("a fifteenth zone (the Weeping House) ships as side content", WEEP && WEEP.npcs.length >= 3);
check("the Weeping House is reachable from the Lamplit Quarter (a side door)", (SCENES.lamplit.exits || []).some(x => x.to === "weeping"));
const matron = CONVS.find(c => c.id === "wh.matron");
const tamC = CONVS.find(c => c.id === "wh.tam");
const keeperC = CONVS.find(c => c.id === "wh.keeper");
check("the haunting's three souls are present (the Lady, the child, the housekeeper)", matron && tamC && keeperC);
// the investigation gates the resolution: you can only confront the Lady with the truth once you've gathered the clues
const whM1 = matron.nodes.find(n => n.id === "1");
const papaConfront = whM1.choices.find(ch => ch.when && ch.when.flag === "wh.clue_papa_left");
const guardConfront = whM1.choices.find(ch => ch.when && ch.when.flag === "wh.knows_truth");
const cluePapa = E.newState(); cluePapa.bools["wh.clue_papa_left"] = true;
const clueTruth = E.newState(); clueTruth.bools["wh.knows_truth"] = true;
check("confronting the Lady with the truth is gated on having investigated for it", papaConfront && guardConfront &&
  E.choiceAvailable(goodGuy, cluePapa, papaConfront, MODEL) === true && E.choiceAvailable(goodGuy, E.newState(), papaConfront, MODEL) === false &&
  E.choiceAvailable(goodGuy, clueTruth, guardConfront, MODEL) === true && E.choiceAvailable(goodGuy, E.newState(), guardConfront, MODEL) === false);
// the child supplies a clue the adults can't/won't, and the housekeeper supplies the withheld truth (clue sources)
check("the child and the housekeeper each surface clues that unlock the confrontation",
  tamC.nodes.some(n => (n.effects || []).some(e => e.key === "wh.clue_papa_left")) &&
  keeperC.nodes.some(n => (n.effects || []).some(e => e.key === "wh.knows_truth")));
// an Investigation check reads the house itself (the latched hearth-guard) — the physical evidence
const investChoice = keeperC.nodes.find(n => n.id === "1").choices.find(ch => (ch.check || {}).skill === "Investigation");
check("an Investigation check reads the house's physical evidence (the latched guard)", investChoice &&
  keeperC.nodes.some(n => (n.effects || []).some(e => e.key === "wh.clue_guard_latched")));
// the quest resolves: the family is freed, and a crit-tier persuasion exists for the breakthrough
check("solving the haunting frees the family (and a nat-20 breakthrough exists)", matron.nodes.some(n =>
  (n.effects || []).some(e => e.key === "wh.freed_family")) && papaConfront.crit && papaConfront.fumble);
check("each Weeping-House soul carries a [RETURNED] line", [matron, tamC, keeperC].every(c =>
  c.nodes.some(n => (n.choices || []).some(ch => ch.tag === "returned"))));

// ---- sixteenth zone: the Old Cistern — a monster-with-a-reason side quest (redeem or destroy) ----
const CIST = SCENES && SCENES.cistern;
check("a sixteenth zone (the Old Cistern) ships as side content", CIST && CIST.npcs.length >= 3);
check("the Cistern is reachable from the Underbridge (another optional branch)", (SCENES.underbridge.exits || []).some(x => x.to === "cistern"));
const gnaw = CONVS.find(c => c.id === "ci.gnaw");
const berinC = CONVS.find(c => c.id === "ci.berin");
check("the haunting's three souls are present (the frightened local, the rememberer, the monster)", gnaw &&
  berinC && CONVS.find(c => c.id === "ci.sedge"));
check("the monster is revealed as the Hunger (a forgotten soul) — the saga's heart, in a side quest", gnaw.returned &&
  /Hunger|Nettie|forgotten/i.test(JSON.stringify(gnaw.nodes.find(n => n.id === "0"))));
const witnessOpt = gnaw.nodes.find(n => n.id === "1").choices.find(ch => ch.when && ch.when.flags && ch.when.flags.indexOf("ci.knows_nettie") >= 0);
const knowsName = E.newState(); knowsName.bools["ci.knows_nettie"] = true;
check("you can only witness the monster back into a person once you've learned its name (Nettie)", witnessOpt &&
  E.choiceAvailable(goodGuy, knowsName, witnessOpt, MODEL) === true && E.choiceAvailable(goodGuy, E.newState(), witnessOpt, MODEL) === false);
check("the rememberer surfaces the name; the quest can resolve by redemption or by a sorrowful end",
  berinC.nodes.some(n => (n.effects || []).some(e => e.key === "ci.knows_nettie")) &&
  gnaw.nodes.some(n => (n.effects || []).some(e => e.key === "ci.gnaw_witnessed")) &&
  gnaw.nodes.some(n => (n.effects || []).some(e => e.key === "ci.gnaw_ended")));
check("the monster offers a force fork (intimidate) with nat-20/nat-1, distinct from the witness path",
  gnaw.nodes.find(n => n.id === "1").choices.some(ch => (ch.check || {}).skill === "Intimidation" && ch.crit && ch.fumble));
check("each Cistern soul carries a [RETURNED] line", [gnaw, berinC, CONVS.find(c => c.id === "ci.sedge")].every(c =>
  c.nodes.some(n => (n.choices || []).some(ch => ch.tag === "returned"))));

// ---- seventeenth zone: the Mapmaker's Wagon — a recruitable third companion + a moral fork ----
const CART = SCENES && SCENES.cartographer;
check("a seventeenth zone (the Mapmaker's Wagon) ships as a companion vignette", CART && CART.npcs.length >= 3);
check("the Wagon is reachable off the Hearth (a lantern off the road)", (SCENES.hearth.exits || []).some(x => x.to === "cartographer"));
const sennetC = CONVS.find(c => c.id === "cg.sennet");
const tibbC = CONVS.find(c => c.id === "cg.tibb");
const vaelC = CONVS.find(c => c.id === "cg.vael");
check("the wagon's three souls are present (the dying mapmaker, the worried apprentice, the patient buyer)", sennetC && tibbC && vaelC);
const sennetMenu = sennetC.nodes.find(n => n.id === "1");
const recruitOpt = sennetMenu.choices.find(ch => ch.tag === "returned");
check("the Returned can offer to walk the deep country for Sennet — a [RETURNED]-tagged recruit path", !!recruitOpt);
const recruitNode = sennetC.nodes.find(n => n.id === recruitOpt.next);
check("recruiting Sennet sets party.sennet_recruited and saves their life (cg.sennet_lives)", recruitNode &&
  recruitNode.effects.some(e => e.key === "party.sennet_recruited") && recruitNode.effects.some(e => e.key === "cg.sennet_lives"));
check("a Persuasion hire offers a second recruit path with nat-20/nat-1 forks", sennetMenu.choices.some(ch =>
  (ch.check || {}).skill === "Persuasion" && ch.crit && ch.fumble));
const sennetCost = sennetMenu.choices.find(ch => (ch.check || {}).skill === "Insight" && ch.when && (ch.when.flags || []).indexOf("cg.sennet_dying") >= 0);
const dyingKnown = E.newState(); dyingKnown.bools["cg.sennet_dying"] = true;
check("the cutting Insight at Sennet is flag-gated — it only opens once you've learned they're dying", sennetCost &&
  E.matchesWhen(goodGuy, dyingKnown, sennetCost.when) === true && E.matchesWhen(goodGuy, E.newState(), sennetCost.when) === false);
check("Tibb is the one who surfaces the clue (cg.sennet_dying), the apprentice's burden", tibbC.nodes.some(n =>
  (n.effects || []).some(e => e.key === "cg.sennet_dying")));
const vaelMenu = vaelC.nodes.find(n => n.id === "1");
check("Goodman Vael can be routed (Intimidation nat-20 redeems the grieving widower) or dug in — a real fork", (() => {
  const intim = vaelMenu.choices.find(ch => (ch.check || {}).skill === "Intimidation");
  const routed = intim && vaelC.nodes.find(n => n.id === intim.crit);
  return intim && routed && routed.effects.some(e => e.key === "cg.vael_redeemed");
})());
check("Vael's secret (a widower buying a door to his dead wife) is reachable by Insight DC 15", vaelMenu.choices.some(ch =>
  (ch.check || {}).skill === "Insight" && (ch.check || {}).dc >= 15) &&
  vaelC.nodes.some(n => (n.effects || []).some(e => e.key === "cg.vael_grief")));
check("each Wagon soul carries a [RETURNED] line", [sennetC, tibbC, vaelC].every(c =>
  c.nodes.some(n => (n.choices || []).some(ch => ch.tag === "returned")) && c.returned));

// ---- eighteenth zone: the Reckoning Court — a courtroom side quest (defend a soul from the Wall) ----
const TRI = SCENES && SCENES.trial;
check("an eighteenth zone (the Reckoning Court) ships as side content", TRI && TRI.npcs.length >= 3);
check("the Court is reached down a stair from the Counting-House", (SCENES.counthouse.exits || []).some(x => x.to === "trial"));
const annet = CONVS.find(c => c.id === "tr.annet");
const sevard = CONVS.find(c => c.id === "tr.measurer");
const reeve = CONVS.find(c => c.id === "tr.reeve");
check("the trial's three roles are present (the accused, the prosecutor, the reeve who pronounces)", annet && sevard && reeve);
const sevMenu = sevard.nodes.find(n => n.id === "1");
check("the prosecution can be argued down by Persuasion or Insight, each with nat-20/nat-1", sevMenu &&
  sevMenu.choices.some(ch => (ch.check || {}).skill === "Persuasion" && ch.crit && ch.fumble) &&
  sevMenu.choices.some(ch => (ch.check || {}).skill === "Insight"));
// evidence you gathered upstairs in the Counting-House climax unlocks two decisive arguments
const trVennArg = sevMenu.choices.find(ch => ch.when && (ch.when.flags || []).indexOf("ch.venn_confirmed") >= 0);
const trMerethArg = sevMenu.choices.find(ch => ch.when && (ch.when.flags || []).indexOf("ch.mereth_cracked") >= 0);
const trVennState = E.newState(); trVennState.bools["ch.venn_confirmed"] = true;
check("confirming the forged Concord (Venn) unlocks a decisive argument that voids the case", trVennArg &&
  E.choiceAvailable(goodGuy, trVennState, trVennArg, MODEL) === true && E.choiceAvailable(goodGuy, E.newState(), trVennArg, MODEL) === false &&
  sevard.nodes.find(n => n.id === trVennArg.next).effects.some(e => e.key === "tr.annet_spared"));
check("cracking the High Measurer upstairs unlocks a second decisive argument (a stay)", trMerethArg &&
  sevard.nodes.find(n => n.id === trMerethArg.next).effects.some(e => e.key === "tr.annet_spared"));
check("only the Returned can testify to what the Wall actually does to a soul (a [RETURNED] argument)",
  sevMenu.choices.some(ch => ch.tag === "returned") &&
  sevard.nodes.some(n => (n.effects || []).some(e => e.key === "tr.testimony_given")));
check("the accused can ultimately be spared — multiple paths set tr.annet_spared",
  [sevard, reeve].some(c => c.nodes.filter(n => (n.effects || []).some(e => e.key === "tr.annet_spared")).length >= 2));
check("the reeve's acquittal is gated on the prosecution collapsing first (a real two-step)", (() => {
  const acquit = reeve.nodes.find(n => n.id === "1").choices.find(ch => ch.when && (ch.when.flags || []).indexOf("tr.annet_spared") >= 0);
  const spared = E.newState(); spared.bools["tr.annet_spared"] = true;
  return acquit && E.choiceAvailable(goodGuy, spared, acquit, MODEL) === true && E.choiceAvailable(goodGuy, E.newState(), acquit, MODEL) === false;
})());
check("each Reckoning-Court soul carries a [RETURNED] line + a Returned-sense", [annet, sevard, reeve].every(c =>
  c.nodes.some(n => (n.choices || []).some(ch => ch.tag === "returned")) && c.returned));

// ---- nineteenth zone: the Bone Scrivener — a god's-bargain structure (Jergal, the First Lord of the Dead) ----
const SCR = SCENES && SCENES.scrivener;
check("a nineteenth zone (the Bone Scrivener) ships as a god's-bargain set-piece", SCR && SCR.npcs.length >= 3);
check("the Scrivener keeps a desk off the Grey Wayshrine (a dusty side-path)", (SCENES.wayshrine.exits || []).some(x => x.to === "scrivener"));
const jergal = CONVS.find(c => c.id === "sc.jergal");
const petitioner = CONVS.find(c => c.id === "sc.petitioner");
const wisp = CONVS.find(c => c.id === "sc.nameless");
check("the bargain's three souls are present (the broker, the one deciding, the one who paid)", jergal && petitioner && wisp);
check("Jergal is the First Lord of the Dead — Kelemvor's predecessor, who resigned the office to tedium", jergal &&
  /First Lord|Jergal|before your .?Kelemvor|gave the whole|resign/i.test(JSON.stringify(jergal.nodes)));
const bargainMenu = jergal.nodes.find(n => n.id === "2");
check("the bargain offers three coins of self — a year, a memory, or a name — each striking a soul off the Wall", bargainMenu &&
  bargainMenu.choices.some(ch => /year/i.test(ch.text)) && bargainMenu.choices.some(ch => /memory/i.test(ch.text)) &&
  bargainMenu.choices.some(ch => ch.tag === "returned" && /name/i.test(ch.text)) &&
  ["jergal_pay_year", "jergal_pay_memory", "jergal_pay_name"].every(id => jergal.nodes.find(n => n.id === id && (n.effects || []).some(e => e.key === "sc.faithless_saved"))));
// the clever path: pay nothing by outwitting the ledger — but only once you've learned the book owes you
const outwit = bargainMenu.choices.find(ch => (ch.check || {}).skill === "Persuasion");
const owedState = E.newState(); owedState.bools["sc.knows_owed"] = true;
check("an outwit path (assign the debt the cosmos already owes you) unlocks only after learning the ledger sums to zero", outwit &&
  outwit.crit && outwit.fumble &&
  E.choiceAvailable(goodGuy, owedState, outwit, MODEL) === true && E.choiceAvailable(goodGuy, E.newState(), outwit, MODEL) === false);
check("the nat-20 outwit escalates to emptying the Wall itself (the structure's biggest swing)", jergal.nodes.find(n => n.id === "jergal_outwit_crit") &&
  jergal.nodes.find(n => n.id === "jergal_outwit_crit").effects.some(e => e.key === "sc.jergal_audits_wall"));
check("refusing the bargain is its own honest path — and teaches the way to empty the book at the throne", jergal.nodes.find(n => n.id === "jergal_refuse") &&
  jergal.nodes.find(n => n.id === "jergal_refuse").effects.some(e => e.key === "sc.knows_empty_the_book"));
check("each Scrivener soul carries a [RETURNED] line + a Returned-sense", [jergal, petitioner, wisp].every(c =>
  c.nodes.some(n => (n.choices || []).some(ch => ch.tag === "returned")) && c.returned));

// ---- twentieth zone: the Deep Archive — a heist structure (steal the master ledger of the Faithless) ----
const ARC = SCENES && SCENES.archive;
check("a twentieth zone (the Deep Archive) ships as a heist set-piece", ARC && ARC.npcs.length >= 3);
check("the Archive is reached by a darker stair from the Counting-House", (SCENES.counthouse.exits || []).some(x => x.to === "archive"));
const sparrow = CONVS.find(c => c.id === "hs.sparrow");
const codex = CONVS.find(c => c.id === "hs.archivist");
const tally = CONVS.find(c => c.id === "hs.tally");
check("the heist's three pieces are present (the crew/thief, the obstacle/watch, the score/ledger)", sparrow && codex && tally);
check("the inside thief (the Sparrow) offers a partner-or-rival fork, and can be turned off the slaver's job", sparrow &&
  sparrow.nodes.some(n => (n.effects || []).some(e => e.key === "hs.partnered")) &&
  sparrow.nodes.some(n => (n.effects || []).some(e => e.key === "hs.sparrow_allied")));
const codexMenu = codex.nodes.find(n => n.id === "1");
check("the night-watch is a multi-approach obstacle — bluff, slip past, turn, or cow (each with skill + fork)", codexMenu &&
  codexMenu.choices.some(ch => (ch.check || {}).skill === "Deception" && ch.crit && ch.fumble) &&
  codexMenu.choices.some(ch => (ch.check || {}).skill === "Stealth") &&
  codexMenu.choices.some(ch => (ch.check || {}).skill === "Intimidation") &&
  codexMenu.choices.some(ch => ch.tag === "returned"));
check("the Deception bluff lands harder once you've cracked the Canon upstairs (the gossip is real)", (() => {
  const v = codex.nodes.find(n => n.id === "0").variants;
  return v.some(x => x.when && (x.when.flags || []).indexOf("ch.mereth_cracked") >= 0);
})());
const tallyMenu = tally.nodes.find(n => n.id === "1");
const bypassed = E.newState(); bypassed.bools["hs.archivist_bypassed"] = true;
check("the score is a real choice — burn it, deliver it to Jergal, or cut out the forged founding page",
  tally.nodes.find(n => n.id === "tally_burn") && tally.nodes.find(n => n.id === "tally_burn").effects.some(e => e.key === "hs.list_destroyed") &&
  tally.nodes.find(n => n.id === "tally_jergal") && tally.nodes.find(n => n.id === "tally_jergal").effects.some(e => e.key === "hs.jergal_gets_book") &&
  tally.nodes.find(n => n.id === "tally_venn") && tally.nodes.find(n => n.id === "tally_venn").effects.some(e => e.key === "hs.law_unmade"));
check("burning frees all but scatters the frail; delivering to Jergal saves them whole (a real trade-off)",
  tally.nodes.find(n => n.id === "tally_burn").effects.some(e => e.key === "hs.frail_scattered") &&
  tally.nodes.find(n => n.id === "tally_jergal").effects.some(e => e.key === "hs.tally_safe"));
const jergalScore = tallyMenu.choices.find(ch => ch.next === "tally_jergal");
check("the safe-delivery option only opens if you've actually met Jergal at the crossroads", jergalScore && jergalScore.when &&
  (jergalScore.when.flags || []).indexOf("sc.met_jergal") >= 0);
const vennScore = tallyMenu.choices.find(ch => ch.next === "tally_venn");
check("the elegant page-cut option only opens if you confirmed the forged Concord", vennScore && vennScore.when &&
  (vennScore.when.flags || []).indexOf("ch.venn_confirmed") >= 0);
check("a [RETURNED] score lets you consult the ten thousand souls themselves before choosing", tallyMenu.choices.some(ch =>
  ch.tag === "returned" && tally.nodes.find(n => n.id === ch.next).effects.some(e => e.key === "hs.souls_consented")));
check("each Deep-Archive soul carries a [RETURNED] line + a Returned-sense", [sparrow, codex, tally].every(c =>
  c.nodes.some(n => (n.choices || []).some(ch => ch.tag === "returned")) && c.returned));
// the Sparrow is a FOURTH recruitable companion — turn her conscience and she joins
check("the Sparrow can be recruited as a fourth companion (party.sparrow_recruited) once she doubts the job", (() => {
  const recruitChoice = sparrow.nodes.find(n => n.id === "1").choices.find(ch => ch.next === "sparrow_recruit");
  const doubts = E.newState(); doubts.bools["hs.sparrow_doubts"] = true;
  const joins = sparrow.nodes.find(n => n.id === "sparrow_recruit");
  return recruitChoice && joins && joins.effects.some(e => e.key === "party.sparrow_recruited") &&
    E.choiceAvailable(goodGuy, doubts, recruitChoice, MODEL) === true && E.choiceAvailable(goodGuy, E.newState(), recruitChoice, MODEL) === false;
})());
check("the Returned can turn her straight off the slaver's job and recruit her in one stroke",
  sparrow.nodes.find(n => n.id === "sparrow_returned").effects.some(e => e.key === "party.sparrow_recruited"));
const hSparrow = CONVS.find(c => c.id === "hearth.sparrow");
const hSparrowNpc = CAMP.npcs.find(n => n.conv === "hearth.sparrow");
check("the recruited Sparrow joins the Hearth fire, gated on party.sparrow_recruited", hSparrow && hSparrowNpc &&
  hSparrowNpc.when && hSparrowNpc.when.flag === "party.sparrow_recruited" &&
  E.matchesWhen(goodGuy, (() => { const s = E.newState(); s.bools["party.sparrow_recruited"] = true; return s; })(), hSparrowNpc.when) === true &&
  E.matchesWhen(goodGuy, E.newState(), hSparrowNpc.when) === false &&
  hSparrow.nodes.some(n => (n.choices || []).some(ch => ch.tag === "returned")) && hSparrow.returned);

// ---- twenty-first zone: the Advocate's Booth — a negotiation-with-a-devil structure (read the fine print) ----
const ADV = SCENES && SCENES.advocate;
check("a twenty-first zone (the Advocate's Booth) ships as a devil-negotiation set-piece", ADV && ADV.npcs.length >= 3);
check("the Advocate keeps a booth at the dark edge of the Night Market", (SCENES.nightmarket.exits || []).some(x => x.to === "advocate"));
const crede = CONVS.find(c => c.id === "ad.crede");
const bonded = CONVS.find(c => c.id === "ad.bonded");
const imp = CONVS.find(c => c.id === "ad.imp");
check("the negotiation's three souls are present (the devil-advocate, the bonded soul, the bound clerk)", crede && bonded && imp);
const credeMenu = crede.nodes.find(n => n.id === "1");
check("the contract is read, not just signed — Investigation finds the trap clause, Insight finds the real motive", credeMenu &&
  credeMenu.choices.some(ch => (ch.check || {}).skill === "Investigation") &&
  credeMenu.choices.some(ch => (ch.check || {}).skill === "Insight") &&
  crede.nodes.find(n => n.id === "crede_fineprint").effects.some(e => e.key === "ad.found_clause_nine") &&
  crede.nodes.find(n => n.id === "crede_motive").effects.some(e => e.key === "ad.crede_unmasked"));
check("the [RETURNED] reading voids the devil's whole estate (unowned means unownable, by Hell too)", credeMenu.choices.some(ch =>
  ch.tag === "returned" && crede.nodes.find(n => n.id === ch.next).effects.some(e => e.key === "ad.estate_voided")));
check("the bonded soul is a warning the player can witness to peace, not just pity", bonded &&
  bonded.nodes.some(n => (n.choices || []).some(ch => ch.tag === "returned")) &&
  bonded.nodes.some(n => (n.effects || []).some(e => e.key === "ad.bonded_at_peace")) && bonded.returned);
check("the bound clerk Sevenpence can be FREED by a Returned declaring his padded task complete", imp &&
  imp.nodes.some(n => (n.choices || []).some(ch => ch.tag === "returned")) &&
  imp.nodes.find(n => n.id === "imp_freed") && imp.nodes.find(n => n.id === "imp_freed").effects.some(e => e.key === "ad.freed_sevenpence") &&
  imp.nodes.some(n => (n.effects || []).some(e => e.key === "ad.knows_crede_fears_signing")) && imp.returned);

// ---- twenty-second zone: the Assize of the Returned — a trial where YOU are the accused (read your run) ----
const ASZ = SCENES && SCENES.assize;
check("a twenty-second zone (the Assize) ships — the trial-where-you're-accused inversion", ASZ && ASZ.npcs.length >= 3);
check("the Assize is convened off the Grey Wayshrine", (SCENES.wayshrine.exits || []).some(x => x.to === "assize"));
const arbiter = CONVS.find(c => c.id === "as.arbiter");
const accuser = CONVS.find(c => c.id === "as.accuser");
const aswitness = CONVS.find(c => c.id === "as.witness");
check("the assize's three roles are present (the arbiter, the accuser, the witness)", arbiter && accuser && aswitness);
const arbMenu = arbiter.nodes.find(n => n.id === "1");
check("you defend yourself, with disposition-gated pleas (merciful/honest) plus universal options", arbMenu &&
  arbMenu.choices.some(ch => ch.when && ch.when.int && ch.when.int["disp.merciful"]) &&
  arbMenu.choices.some(ch => ch.when && ch.when.int && ch.when.int["disp.honest"]) &&
  arbMenu.choices.some(ch => ch.next === "arb_witness") && arbMenu.choices.some(ch => ch.next === "arb_accept"));
check("refusing the court's authority IS the winning defense — a soul that judges itself can't be judged", (() => {
  const refuse = arbMenu.choices.find(ch => ch.tag === "returned" && ch.next === "arb_refuse");
  const node = arbiter.nodes.find(n => n.id === "arb_refuse");
  return refuse && node && node.effects.some(e => e.key === "as.verdict_self_judged") && node.effects.some(e => e.key === "as.court_dissolved");
})());
check("the merciful/honest pleas escalate to the forward question (what stops you becoming the Crown)", (() => {
  const mercy = arbiter.nodes.find(n => n.id === "arb_mercy");
  const node2 = arbiter.nodes.find(n => n.id === "2");
  return mercy && mercy.auto === "2" && node2 && node2.choices.some(ch => ch.next === "arb_answer_unknowing");
})());
check("the accuser is revealed (Insight) as your own fear given a hood — keep the doubt, don't banish it", accuser &&
  accuser.nodes.find(n => n.id === "acc_mirror") &&
  accuser.nodes.find(n => n.id === "acc_mirror").effects.some(e => e.key === "as.accuser_is_self"));
check("the witness is the sum of the dead you touched, and its variant reads merciful vs ruthless runs", aswitness &&
  aswitness.nodes.find(n => n.id === "0").variants.some(v => v.when && v.when.int && v.when.int["disp.merciful"]) &&
  aswitness.nodes.find(n => n.id === "0").variants.some(v => v.when && v.when.int && v.when.int["disp.ruthless"]));
check("each Assize soul carries a [RETURNED] line + a Returned-sense", [arbiter, accuser, aswitness].every(c =>
  c.nodes.some(n => (n.choices || []).some(ch => ch.tag === "returned")) && c.returned));

// ---- twenty-third zone: the Siege of the Almshouse — a siege held by witness, not the sword ----
const SIE = SCENES && SCENES.siege;
check("a twenty-third zone (the Siege) ships as a defend-the-refuge set-piece", SIE && SIE.npcs.length >= 3);
check("the Siege is reached off the Lamplit Quarter", (SCENES.lamplit.exits || []).some(x => x.to === "siege"));
const orla = CONVS.find(c => c.id === "si.warden");
const vane = CONVS.find(c => c.id === "si.herald");
const sheltered = CONVS.find(c => c.id === "si.refugee");
check("the siege's three roles are present (the defender, the besieger's herald, the sheltered)", orla && vane && sheltered);
const orlaMenu = orla.nodes.find(n => n.id === "1");
check("the defense is multi-approach — rally the room (Persuasion) or find an evacuation (Investigation)", orlaMenu &&
  orlaMenu.choices.some(ch => (ch.check || {}).skill === "Persuasion" && ch.crit && ch.fumble) &&
  orlaMenu.choices.some(ch => (ch.check || {}).skill === "Investigation"));
const vaneMenu = vane.nodes.find(n => n.id === "1");
check("the siege is won by WITNESS, not the sword — every resolution lifts it without a battle", vaneMenu &&
  ["vane_persuade", "vane_stand"].every(id => vane.nodes.find(n => n.id === id).effects.some(e => e.key === "si.siege_lifted")));
check("the [RETURNED] stand-in-the-breach breaks the enforcers' nerve (the Wall's servants face the one who walked out)",
  vaneMenu.choices.some(ch => ch.tag === "returned" && ch.next === "vane_stand") &&
  vane.nodes.find(n => n.id === "vane_stand").effects.some(e => e.key === "si.broke_their_nerve"));
check("the forged-Concord evidence works here too — an unlawful writ a career Justiciar can't risk serving", (() => {
  const v = vaneMenu.choices.find(ch => ch.when && (ch.when.flags || []).indexOf("ch.venn_confirmed") >= 0);
  const known = E.newState(); known.bools["ch.venn_confirmed"] = true;
  return v && E.choiceAvailable(goodGuy, known, v, MODEL) === true && E.choiceAvailable(goodGuy, E.newState(), v, MODEL) === false;
})());
check("a stall-for-evacuation option opens only once Orla's drain is found (siege as a clock)", (() => {
  const v = vaneMenu.choices.find(ch => ch.next === "vane_stall");
  return v && v.when && (v.when.flags || []).indexOf("si.drain_open") >= 0;
})());
check("the sheltered mother reads how you fought, and the nat-20 rally un-names the slur (a congregation, not prey)",
  orla.nodes.find(n => n.id === "orla_rally_crit").effects.some(e => e.key === "si.congregation") &&
  sheltered.nodes.find(n => n.id === "0").variants.some(v => v.when && (v.when.flags || []).indexOf("si.siege_lifted") >= 0));
check("each Siege soul carries a [RETURNED] line + a Returned-sense", [orla, vane, sheltered].every(c =>
  c.nodes.some(n => (n.choices || []).some(ch => ch.tag === "returned")) && c.returned));

// ---- twenty-fourth zone: the Wake — tonal relief, the one place the dead celebrate ----
const WK = SCENES && SCENES.wake;
check("a twenty-fourth zone (the Wake) ships — tonal contrast, a celebration among the grief", WK && WK.npcs.length >= 3);
check("the Wake is found upriver off the Reed-Walk", (SCENES.reedwalk.exits || []).some(x => x.to === "wake"));
const wkTib = CONVS.find(c => c.id === "wk.host");
const wkDoget = CONVS.find(c => c.id === "wk.doget");
const wkUninvited = CONVS.find(c => c.id === "wk.mourner");
check("the wake's three souls are present (the host, the guest of honor, the wkUninvited)", wkTib && wkDoget && wkUninvited);
check("the wake's warmth carries the saga's ache — a fair death is a miracle, not the rule it should be", wkDoget &&
  wkDoget.nodes.some(n => (n.effects || []).some(e => e.key === "wk.aim_for_the_rule")));
check("the Faithless soul at the window can be invited IN — the small mercy that fixes the wake's one crack", wkUninvited &&
  wkUninvited.nodes.find(n => n.id === "mourner_invite") &&
  wkUninvited.nodes.find(n => n.id === "mourner_invite").effects.some(e => e.key === "wk.window_opened_wide"));
check("you can carry the wkUninvited's song even if you can't carry the soul (a wake that lives in another's mouth)", wkUninvited &&
  wkUninvited.nodes.find(n => n.id === "mourner_song").effects.some(e => e.key === "wk.carries_the_song"));
check("each Wake soul carries a [RETURNED] line + a Returned-sense", [wkTib, wkDoget, wkUninvited].every(c =>
  c.nodes.some(n => (n.choices || []).some(ch => ch.tag === "returned")) && c.returned));

// ---- twenty-fifth zone: the Inquest of the Un-Made — a whodunit with clue-gated accusation ----
const IQ = SCENES && SCENES.inquest;
check("a twenty-fifth zone (the Inquest) ships — a deduction/accusation structure", IQ && IQ.npcs.length >= 3);
check("the Inquest is reached off the Underbridge", (SCENES.underbridge.exits || []).some(x => x.to === "inquest"));
const iqKeeper = CONVS.find(c => c.id === "iq.keeper");
const iqGrieved = CONVS.find(c => c.id === "iq.grieved");
const iqBroker = CONVS.find(c => c.id === "iq.broker");
check("the inquest's three roles are present (the keeper, the bereaved, the suspect)", iqKeeper && iqGrieved && iqBroker);
check("the accusation is CLUE-GATED — you can only name the culprit once you've gathered evidence from both others", (() => {
  const accuse = iqKeeper.nodes.find(n => n.id === "1").choices.find(ch => ch.next === "keeper_accuse");
  const both = E.newState(); both.bools["iq.clue_grieved"] = true; both.bools["iq.clue_broker"] = true;
  return accuse && accuse.when && (accuse.when.flags || []).indexOf("iq.clue_grieved") >= 0 &&
    E.choiceAvailable(goodGuy, both, accuse, MODEL) === true && E.choiceAvailable(goodGuy, E.newState(), accuse, MODEL) === false;
})());
check("the [RETURNED] can read the absence itself for a clue (nothing is ever truly nothing)", iqKeeper.nodes.find(n => n.id === "1").choices.some(ch =>
  ch.tag === "returned" && iqKeeper.nodes.find(n => n.id === ch.next).effects.some(e => e.key === "iq.clue_broker")));
check("the twist: the culprit is a trolley-problem warden feeding a Hunger-fragment — the Wall in miniature", iqBroker &&
  iqBroker.nodes.some(n => (n.effects || []).some(e => e.key === "iq.wall_in_miniature")));
check("the cistern's lesson applies — a [RETURNED] who learned the bargain can hand the warden the third door", (() => {
  const cure = iqBroker.nodes.find(n => n.id === "1").choices.find(ch => ch.tag === "returned" && ch.next === "broker_cure");
  const known = E.newState(); known.bools["iq.knows_the_bargain"] = true;
  return cure && E.choiceAvailable(goodGuy, known, cure, MODEL) === true && E.choiceAvailable(goodGuy, E.newState(), cure, MODEL) === false &&
    iqBroker.nodes.find(n => n.id === "broker_cure").effects.some(e => e.key === "iq.broker_redeemed");
})());
check("the dead-touched can carry the erased Sella out of reach of the un-making (witness beats erasure)", iqGrieved.nodes.some(n =>
  (n.effects || []).some(e => e.key === "iq.keeps_sella")));
check("each Inquest soul carries a [RETURNED] line + a Returned-sense", [iqKeeper, iqGrieved, iqBroker].every(c =>
  c.nodes.some(n => (n.choices || []).some(ch => ch.tag === "returned")) && c.returned));

// ---- twenty-sixth zone: the Thin Place — a reunion across the Wall (the logistics of love) ----
const TP = SCENES && SCENES.thinplace;
check("a twenty-sixth zone (the Thin Place) ships — a reunion-across-the-Wall structure", TP && TP.npcs.length >= 3);
check("the Thin Place is reached off the last torch, at the Wall's foot", (SCENES.lasttorch.exits || []).some(x => x.to === "thinplace"));
const tpWaiter = CONVS.find(c => c.id === "tp.waiter");
const tpSevered = CONVS.find(c => c.id === "tp.severed");
const tpKeeper = CONVS.find(c => c.id === "tp.keeper");
check("the reunion's three roles are present (the one who waits, the one in the stone, the one who maps the thin places)", tpWaiter && tpSevered && tpKeeper);
check("only the Returned can be the courier across the Wall the widow can only kneel against", tpWaiter.nodes.find(n => n.id === "1").choices.some(ch =>
  ch.tag === "returned" && tpWaiter.nodes.find(n => n.id === ch.next).effects.some(e => e.key === "tp.accepted_courier")));
check("carrying the message both ways gives Aldous back his name and his answer to Pell", tpSevered &&
  tpSevered.nodes.some(n => (n.effects || []).some(e => e.key === "tp.aldous_remembers")) &&
  tpSevered.nodes.some(n => (n.effects || []).some(e => e.key === "tp.aldous_message_for_pell")));
check("the message/name beats at Aldous are gated on having learned them from Pell first (real courier logic)", (() => {
  const named = tpSevered.nodes.find(n => n.id === "1").choices.find(ch => ch.next === "severed_named");
  const knows = E.newState(); knows.bools["tp.knows_aldous"] = true;
  return named && named.when && E.choiceAvailable(goodGuy, knows, named, MODEL) === true && E.choiceAvailable(goodGuy, E.newState(), named, MODEL) === false;
})());
check("the Reed-Knower lost a daughter to the wrong seam, and made a map of mercy from it (and will witness a crossing)", tpKeeper &&
  tpKeeper.nodes.some(n => (n.effects || []).some(e => e.key === "tp.keeper_lost_daughter")) &&
  tpKeeper.nodes.some(n => (n.effects || []).some(e => e.key === "tp.keeper_witnesses")));
check("the reunion can land — the two sing the bad old river-song through the stone (witness beats the Wall)", tpSevered.nodes.some(n =>
  (n.effects || []).some(e => e.key === "tp.reunion_made")));
check("each Thin-Place soul carries a [RETURNED] line + a Returned-sense", [tpWaiter, tpSevered, tpKeeper].every(c =>
  c.nodes.some(n => (n.choices || []).some(ch => ch.tag === "returned")) && c.returned));

// ---- twenty-seventh zone: the Unfinished Argument — a disputation (free a soul by finishing its argument) ----
const DP = SCENES && SCENES.disputation;
check("a twenty-seventh zone (the Unfinished Argument) ships — a disputation/debate structure", DP && DP.npcs.length >= 3);
check("the disputation is reached off the Market", (SCENES.market.exits || []).some(x => x.to === "disputation"));
const phil = CONVS.find(c => c.id === "dp.philosopher");
const dpRival = CONVS.find(c => c.id === "dp.rival");
const dpStudent = CONVS.find(c => c.id === "dp.student");
check("the disputation's three roles are present (the philosopher, the rival, the keeper-student)", phil && dpRival && dpStudent);
const philMenu = phil.nodes.find(n => n.id === "1");
check("the debate is a real argument — Insight diagnoses (with a fail branch), Persuasion argues with crit/fumble", philMenu &&
  philMenu.choices.some(ch => (ch.check || {}).skill === "Insight" && ch.fail) &&
  philMenu.choices.some(ch => (ch.check || {}).skill === "Persuasion" && ch.crit && ch.fumble));
check("you free the philosopher not by defeating but by letting him lose gracefully (the nat-20 frees him)", phil.nodes.find(n => n.id === "phil_argued_crit") &&
  phil.nodes.find(n => n.id === "phil_argued_crit").effects.some(e => e.key === "dp.phil_freed"));
check("the argument escalates to the saga's thesis — the third door, necessity as failure of imagination", (() => {
  const node2 = phil.nodes.find(n => n.id === "2");
  return node2 && node2.choices.some(ch => ch.next === "phil_thirddoor") &&
    phil.nodes.find(n => n.id === "phil_thirddoor").effects.some(e => e.key === "dp.third_door_doctrine");
})());
check("the [RETURNED] can finish it with testimony — a witness beats a philosopher who argued death from a chair",
  philMenu.choices.some(ch => ch.tag === "returned" && ch.next === "phil_witness"));
check("the rival's true anchor (not pride, but unsaid love) and the student's (love of both) are readable", dpRival &&
  dpRival.nodes.some(n => (n.effects || []).some(e => e.key === "dp.rival_grief_named")) &&
  dpStudent.nodes.some(n => (n.effects || []).some(e => e.key === "dp.student_anchor_named")));
check("each Disputation soul carries a [RETURNED] line + a Returned-sense", [phil, dpRival, dpStudent].every(c =>
  c.nodes.some(n => (n.choices || []).some(ch => ch.tag === "returned")) && c.returned));

// ---- grand totals across the whole walkable Act ----
check("the playable Act spans a dozen connected zones and 40+ souls", Object.keys(SCENES).length >= 12 &&
  Object.values(SCENES).reduce((a, s) => a + s.npcs.length, 0) >= 40 && (() => {
    const seen = new Set(["market"]); const q = ["market"];
    while (q.length) { const z = q.shift(); (SCENES[z].exits || []).forEach(x => { if (SCENES[x.to] && !seen.has(x.to)) { seen.add(x.to); q.push(x.to); } }); }
    return seen.size === Object.keys(SCENES).length;
  })());
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
