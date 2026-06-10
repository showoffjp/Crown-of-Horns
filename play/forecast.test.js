// Forecast tests: closed-form checks + a Monte-Carlo cross-check that the analytic
// preview equals what the real AttackResolver actually rolls. Runs in CI.
const { Dice, CharacterSheet, Ability, AttackResolver } = require("./engine.js");
const { forecast, average, summary } = require("./forecast.js");

let pass = 0, fail = 0; const fails = [];
function test(name, fn) { try { fn(); pass++; } catch (e) { fail++; fails.push(`${name}: ${e.message}`); } }
const near = (a, b, tol, m) => { if (Math.abs(a - b) > tol) throw new Error(`${m || ""} expected ${b}, got ${a} (tol ${tol})`); };
const T = (c, m) => { if (!c) throw new Error(m || "expected true"); };

function sheet(name, ac, str = 10, hp = 1000) {
  const s = new CharacterSheet({ displayName: name, classDef: null, level: 1, baseArmorClass: ac, maxHitPoints: hp });
  s.abilities.Set("strength", str); s.abilities.Set("dexterity", 10); s.abilities.Set("intelligence", 10);
  return s;
}
const weapon = (dice = "1d8", addMod = false) =>
  Ability({ abilityName: "Forecast Strike", isAttackRoll: true, damageDice: dice, addAbilityModToDamage: addMod, damageType: "Slashing" });

// ---- closed-form ----
test("HitChance_MatchesHandComputedTable", () => {
  const fc = forecast(sheet("A", 10, 10), sheet("D", 13), weapon());
  near(fc.hitChance, 0.5, 1e-6, "toHit +2 vs AC13 -> 10/20");
  near(fc.critChance, 0.05, 1e-6);
  T(!fc.advantage && !fc.disadvantage);
});
test("Advantage_RaisesHitAndCrit", () => {
  const fc = forecast(sheet("A", 10, 10), sheet("D", 13), weapon(), true);
  T(fc.advantage);
  near(fc.hitChance, 1 - 0.5 * 0.5, 1e-6);
  near(fc.critChance, 1 - 0.95 * 0.95, 1e-6);
});
test("CoverBonus_LowersHit", () => {
  const open = forecast(sheet("A", 10, 10), sheet("D", 13), weapon());
  const cover = forecast(sheet("A", 10, 10), sheet("D", 13), weapon(), false, 5);
  T(cover.hitChance < open.hitChance);
});
test("Heal_MeanWithinBounds", () => {
  const heal = Ability({ abilityName: "Mend", isHeal: true, healDice: "2d4", damageDice: "" });
  const fc = forecast(sheet("C", 10, 10), sheet("Ally", 10), heal);
  T(fc.isHeal); near(fc.expectedHealing, 5, 1e-6); near(fc.hitChance, 1, 1e-6);
});
test("Save_FailChanceFollowsDC", () => {
  const spell = Ability({ abilityName: "Blast", isAttackRoll: false, saveAbility: "dexterity", saveForHalf: true, damageDice: "6d6", addAbilityModToDamage: false });
  const fc = forecast(sheet("A", 10, 10), sheet("D", 10), spell);
  T(fc.isSave); near(fc.hitChance, 9 / 20, 1e-6);
});
test("Average_FollowsNotation", () => {
  near(average("1d8"), 4.5, 1e-9); near(average("2d6+3"), 10, 1e-9);
  near(average(""), 0, 1e-9); near(average("garbage"), 0, 1e-9);
});
test("Summary_Renders", () => {
  const s = summary(forecast(sheet("A", 10, 16), sheet("D", 13, 10, 8), weapon("1d8", true)));
  T(/\d+% hit/.test(s) && /dmg/.test(s), `got: ${s}`);
});

// ---- the load-bearing cross-check: analytic == empirical (real resolver) ----
test("Forecast_MatchesResolver_OverManySeeds", () => {
  function cross(atk, def, ab, trials = 40000) {
    const fc = forecast(atk, def, ab);
    let hits = 0, kills = 0, dmg = 0; const hp0 = def.currentHitPoints;
    for (let s = 0; s < trials; s++) {
      Dice.Seed(s);
      const r = AttackResolver.Resolve(atk, def, ab);
      if (r.hit) hits++; dmg += r.damage; if (r.damage >= hp0) kills++;
    }
    near(fc.hitChance, hits / trials, 0.02, "hit rate");
    near(fc.expectedDamage, dmg / trials, 0.4, "mean damage");
    near(fc.lethalChance, kills / trials, 0.03, "kill rate");
  }
  cross(sheet("Hero", 10, 16), sheet("Brute", 13, 14, 8), weapon("1d8", true));
  cross(sheet("Hero", 10, 18), sheet("Tank", 17, 14, 12), weapon("1d12", true));
  cross(sheet("Hero", 10, 14), sheet("Frail", 11, 10, 5), weapon("2d6", true));
  // a saving-throw spell, save-for-half
  const blast = Ability({ abilityName: "Searing", isAttackRoll: false, saveAbility: "dexterity", saveForHalf: true, damageDice: "2d6", addAbilityModToDamage: false });
  cross(sheet("Mage", 10, 10), sheet("Wisp", 12, 10, 7), blast);
});

console.log(`\n  Attack forecast — closed-form + Monte-Carlo agreement with the real resolver:`);
console.log(`  ${pass} passed, ${fail} failed`);
if (fail) { fails.forEach(f => console.log("   ✗ " + f)); process.exit(1); }
else console.log("  ✓ The HUD's hit/crit/damage/kill preview matches the dice it predicts.\n");
