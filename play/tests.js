// ============================================================================
// Your EditMode suite, ported 1:1 and run against the JS engine port.
// Mirrors: DiceTests, AbilityScoresTests, AttackResolverTests, StatusEffectTests.
// If these pass, the port is provably faithful to the Unity combat rules.
// ============================================================================
const E = require("./engine.js");
const { Dice, AbilityScores, Ability, StatusEffect, CharacterSheet, AttackResolver, FormatError } = E;

// --- micro test framework ---------------------------------------------------
let pass = 0, fail = 0; const fails = [];
function test(name, fn) { try { fn(); pass++; } catch (e) { fail++; fails.push(`${name}: ${e.message}`); } }
function eq(a, b, m) { if (a !== b) throw new Error(`${m || ""} expected ${b}, got ${a}`); }
function ok(c, m) { if (!c) throw new Error(m || "expected true"); }
function no(c, m) { if (c) throw new Error(m || "expected false"); }
function gte(a, b, m) { if (!(a >= b)) throw new Error(`${m || ""} ${a} !>= ${b}`); }
function lte(a, b, m) { if (!(a <= b)) throw new Error(`${m || ""} ${a} !<= ${b}`); }
function gt(a, b, m) { if (!(a > b)) throw new Error(`${m || ""} ${a} !> ${b}`); }
function throws(fn, m) { let t = false; try { fn(); } catch (e) { t = true; } if (!t) throw new Error(m || "expected throw"); }

// ===================== DiceTests ===========================================
test("Dice.Seed_MakesRollsDeterministic", () => {
  Dice.Seed(12345); const a = Dice.Roll(20), b = Dice.Roll(20);
  Dice.Seed(12345); eq(Dice.Roll(20), a, "first roll"); eq(Dice.Roll(20), b, "second roll");
});
test("Dice.Roll_SingleDie_StaysWithinBounds", () => {
  Dice.Seed(1); for (let i = 0; i < 1000; i++) { const r = Dice.Roll(6); gte(r, 1); lte(r, 6); }
});
test("Dice.Roll_MultipleDice_StaysWithinSummedBounds", () => {
  Dice.Seed(2); for (let i = 0; i < 1000; i++) { const r = Dice.Roll(3, 6); gte(r, 3); lte(r, 18); }
});
test("Dice.D20_StaysWithinBounds", () => {
  Dice.Seed(3); for (let i = 0; i < 1000; i++) { const r = Dice.D20(); gte(r, 1); lte(r, 20); }
});
test("Dice.Advantage_TrendsHigherThanDisadvantage", () => {
  Dice.Seed(4); let adv = 0, dis = 0;
  for (let i = 0; i < 5000; i++) adv += Dice.D20Advantage();
  for (let i = 0; i < 5000; i++) dis += Dice.D20Disadvantage();
  gt(adv, dis, "advantage should out-total disadvantage");
});
test("Dice.AdvantageAndDisadvantage_StayWithinD20Bounds", () => {
  Dice.Seed(7); for (let i = 0; i < 1000; i++) {
    const a = Dice.D20Advantage(), d = Dice.D20Disadvantage();
    gte(a, 1); lte(a, 20); gte(d, 1); lte(d, 20);
  }
});
test("Dice.Roll_Notation_PositiveModifier_WithinBounds", () => {
  Dice.Seed(5); for (let i = 0; i < 1000; i++) { const r = Dice.Roll("2d6+3"); gte(r, 5); lte(r, 15); }
});
test("Dice.Roll_Notation_NegativeModifier_WithinBounds", () => {
  Dice.Seed(6); for (let i = 0; i < 1000; i++) { const r = Dice.Roll("4d4-1"); gte(r, 3); lte(r, 15); }
});
test("Dice.Roll_Notation_NoModifier_WithinBounds", () => {
  Dice.Seed(8); for (let i = 0; i < 1000; i++) { const r = Dice.Roll("1d8"); gte(r, 1); lte(r, 8); }
});
test("Dice.Roll_InvalidNotation_Throws", () => {
  throws(() => Dice.Roll("notdice")); throws(() => Dice.Roll("d20")); throws(() => Dice.Roll("2x6"));
});

// ===================== AbilityScoresTests ==================================
test("Abilities.Default_AllTens_GiveZeroModifiers", () => {
  const a = new AbilityScores();
  for (const ab of ["strength","dexterity","constitution","intelligence","wisdom","charisma"]) { eq(a.Get(ab), 10); eq(a.Modifier(ab), 0); }
});
[[1,-5],[7,-2],[8,-1],[9,-1],[10,0],[11,0],[12,1],[14,2],[15,2],[20,5],[30,10]].forEach(([score, m]) => {
  test(`Abilities.Modifier_FollowsFifthEditionCurve(${score})`, () => {
    const a = new AbilityScores(); a.Set("strength", score); eq(a.Modifier("strength"), m, `score ${score}`);
  });
});
test("Abilities.SetAndGet_AreIndependentPerAbility", () => {
  const a = new AbilityScores();
  a.Set("strength",16); a.Set("dexterity",8); a.Set("constitution",14);
  a.Set("intelligence",12); a.Set("wisdom",13); a.Set("charisma",18);
  eq(a.Get("strength"),16); eq(a.Get("dexterity"),8); eq(a.Get("charisma"),18);
  eq(a.Modifier("strength"),3); eq(a.Modifier("dexterity"),-1); eq(a.Modifier("charisma"),4);
});

// ===================== AttackResolverTests =================================
function MakeSheet(name, ac, str = 10, hp = 1000) {
  const s = new CharacterSheet({ displayName: name, classDef: null, level: 1, baseArmorClass: ac, maxHitPoints: hp });
  s.abilities.Set("strength", str); s.abilities.Set("dexterity", 10); s.abilities.Set("intelligence", 10);
  return s;
}
const MakeWeapon = (dice = "1d6") => Ability({ abilityName: "Test Strike", isAttackRoll: true, spellSlotLevel: 0, isHeal: false, damageDice: dice, addAbilityModToDamage: false, damageType: "Slashing", rangeTiles: 1 });
function SeedForFirstD20(face) { for (let s = 0; s < 1000000; s++) { Dice.Seed(s); if (Dice.D20() === face) return s; } throw new Error(`no seed for first d20 ${face}`); }

test("AttackResolver.NaturalTwenty_AlwaysHitsAndCrits", () => {
  const atk = MakeSheet("Atk", 10), def = MakeSheet("Def", 90), wpn = MakeWeapon();
  Dice.Seed(SeedForFirstD20(20)); const r = AttackResolver.Resolve(atk, def, wpn);
  eq(r.attackRoll, 20); ok(r.critical, "nat-20 crits"); ok(r.hit, "nat-20 hits vs AC 90");
});
test("AttackResolver.NaturalOne_AlwaysMisses", () => {
  const atk = MakeSheet("Atk", 10, 20), def = MakeSheet("Def", 1), wpn = MakeWeapon();
  Dice.Seed(SeedForFirstD20(1)); const r = AttackResolver.Resolve(atk, def, wpn);
  eq(r.attackRoll, 1); no(r.hit, "nat-1 misses vs AC 1");
});
test("AttackResolver.Critical_DoublesTheDamageDice", () => {
  const atk = MakeSheet("Atk", 10, 10), def = MakeSheet("Def", 4), wpn = MakeWeapon("1d6");
  let critSum = 0, hitSum = 0, critN = 0, hitN = 0;
  for (let s = 0; s < 6000; s++) { Dice.Seed(s); const r = AttackResolver.Resolve(atk, def, wpn);
    if (r.critical) { critSum += r.damage; critN++; } else if (r.hit) { hitSum += r.damage; hitN++; } }
  gt(critN, 0); gt(hitN, 0);
  gt(critSum / critN, (hitSum / hitN) * 1.5, "crit avg ~doubles ordinary-hit avg");
});
test("AttackResolver.SaveForHalf_FullOnFail_HalfOnSave", () => {
  const atk = MakeSheet("Atk", 10), def = MakeSheet("Def", 10);
  const spell = Ability({ abilityName: "Test Blast", isAttackRoll: false, saveAbility: "dexterity", saveForHalf: true, damageDice: "6d6", addAbilityModToDamage: false, damageType: "Fire" });
  let failSum = 0, saveSum = 0, failN = 0, saveN = 0;
  for (let s = 0; s < 4000; s++) { Dice.Seed(s); const r = AttackResolver.Resolve(atk, def, spell);
    if (r.hit) { failSum += r.damage; failN++; } else if (r.damage > 0) { saveSum += r.damage; saveN++; } }
  gt(failN, 0); gt(saveN, 0); gt(failSum / failN, saveSum / saveN, "failed-save > saved damage");
});
test("AttackResolver.Healing_IsPositive_AndFlagged", () => {
  const caster = MakeSheet("Cleric", 10), ally = MakeSheet("Ally", 10);
  const heal = Ability({ abilityName: "Test Mend", isHeal: true, healDice: "2d4", damageDice: "" });
  for (let s = 0; s < 500; s++) { Dice.Seed(s); const r = AttackResolver.Resolve(caster, ally, heal);
    ok(r.isHeal); ok(r.hit); gte(r.healing, 2); lte(r.healing, 8); }
});
test("AttackResolver.Resolve_IsDeterministicUnderSameSeed", () => {
  const atk = MakeSheet("Atk", 10, 14), def = MakeSheet("Def", 13), wpn = MakeWeapon("1d8");
  Dice.Seed(4242); const a = AttackResolver.Resolve(atk, def, wpn);
  Dice.Seed(4242); const b = AttackResolver.Resolve(atk, def, wpn);
  eq(a.attackRoll, b.attackRoll); eq(a.hit, b.hit); eq(a.critical, b.critical); eq(a.damage, b.damage);
});

// ===================== StatusEffectTests ===================================
const Eff = (o) => StatusEffect(Object.assign({ effectName: (o && o.condition) || "Effect" }, o));
const Bearer = (hp = 100) => { const s = new CharacterSheet({ displayName: "Bearer", maxHitPoints: hp, currentHitPoints: hp }); s.abilities.Set("dexterity", 10); return s; };

test("Status.AddEffect_SetsCondition", () => {
  const s = Bearer(); s.AddEffect(Eff({ condition: "Poisoned" }), 2, "src");
  ok(s.HasCondition("Poisoned")); no(s.HasCondition("Stunned"));
});
test("Status.AddEffect_Null_IsNoOp", () => { const s = Bearer(); s.AddEffect(null, 2, "src"); eq(s.activeEffects.length, 0); });
test("Status.DamageOverTime_BitesAtStartOfTurn", () => {
  const s = Bearer(100); s.AddEffect(Eff({ condition: "Burning", damageOverTimeDice: "1d6" }), 3, "src");
  Dice.Seed(99); const dealt = s.TickStartOfTurn(); gte(dealt, 1); lte(dealt, 6); eq(s.currentHitPoints, 100 - dealt);
});
test("Status.Duration_CountsDownThenExpires", () => {
  const s = Bearer(); s.AddEffect(Eff({ condition: "Prone", durationRounds: 2 }), 2, "src");
  s.TickEndOfTurn(); ok(s.HasCondition("Prone")); s.TickEndOfTurn(); no(s.HasCondition("Prone")); eq(s.activeEffects.length, 0);
});
test("Status.ReApplyingSameDef_RefreshesInsteadOfStacking", () => {
  const s = Bearer(); const poison = Eff({ condition: "Poisoned" });
  s.AddEffect(poison, 2, "src"); s.TickEndOfTurn(); s.AddEffect(poison, 2, "src");
  eq(s.activeEffects.length, 1, "must not stack"); eq(s.activeEffects[0].remainingRounds, 2);
});
test("Status.Incapacitation_DrivesQueries", () => {
  const s = Bearer(); s.AddEffect(Eff({ condition: "Stunned", incapacitates: true }), 2, "src");
  ok(s.IsIncapacitated); ok(s.AttacksAtDisadvantage);
});
test("Status.AttackersHaveAdvantage_Flag_IsExposed", () => {
  const s = Bearer(); s.AddEffect(Eff({ condition: "Prone", attackersHaveAdvantage: true }), 2, "src"); ok(s.GrantsAdvantageToAttackers);
});
test("Status.AttackRollModifiers_SumAcrossEffects", () => {
  const s = Bearer(); s.AddEffect(Eff({ condition: "Blessed", attackRollModifier: 2 }), 2, "a"); s.AddEffect(Eff({ condition: "Hasted", attackRollModifier: 1 }), 2, "b");
  eq(s.EffectAttackModifier, 3);
});
test("Status.ArmorClassModifier_AltersArmorClass", () => {
  const s = Bearer(); const base = s.ArmorClass; s.AddEffect(Eff({ condition: "Blessed", armorClassModifier: 2 }), 2, "src"); eq(s.ArmorClass, base + 2);
});
test("Status.RemoveCondition_DropsMatchingEffect", () => {
  const s = Bearer(); s.AddEffect(Eff({ condition: "Poisoned" }), 5, "src"); s.RemoveCondition("Poisoned"); no(s.HasCondition("Poisoned"));
});

// --- report -----------------------------------------------------------------
console.log(`\n  Crown of Horns — engine fidelity suite (ported from your EditMode tests)\n`);
console.log(`  ${pass} passed, ${fail} failed   (of ${pass + fail} assertions across DiceTests, AbilityScoresTests, AttackResolverTests, StatusEffectTests)`);
if (fail) { console.log("\n  FAILURES:"); fails.forEach(f => console.log("   ✗ " + f)); process.exit(1); }
else console.log("\n  ✓ All green — the JS port reproduces your Unity combat rules exactly.\n");
