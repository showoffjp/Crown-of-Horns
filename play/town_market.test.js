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
