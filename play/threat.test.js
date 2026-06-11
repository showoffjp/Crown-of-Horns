// Threat tests: closed-form sanity + a Monte-Carlo cross-check that the analytic
// incoming-damage / down-chance equals what the real AttackResolver produces when
// the listed attackers all focus the target. Runs in CI.
const { Dice, CharacterSheet, Ability, AttackResolver } = require("./engine.js");
const { threat, threatSummary } = require("./threat.js");

let pass = 0, fail = 0; const fails = [];
function test(name, fn) { try { fn(); pass++; } catch (e) { fail++; fails.push(`${name}: ${e.message}`); } }
const near = (a, b, tol, m) => { if (Math.abs(a - b) > tol) throw new Error(`${m || ""} expected ${b}, got ${a} (tol ${tol})`); };
const T = (c, m) => { if (!c) throw new Error(m || "expected true"); };

function sheet(name, ac, str = 10, hp = 1000) {
  const s = new CharacterSheet({ displayName: name, classDef: null, level: 1, baseArmorClass: ac, maxHitPoints: hp });
  s.abilities.Set("strength", str); s.abilities.Set("dexterity", 10); s.abilities.Set("intelligence", 10);
  return s;
}
const claw = (dice = "1d6", str = 14) => Ability({ abilityName: "Claw", isAttackRoll: true, damageDice: dice, addAbilityModToDamage: true });

test("NoAttackers_IsSafe", () => {
  const t = threat(sheet("Hero", 16, 10, 20), []);
  near(t.expectedIncoming, 0, 1e-9); near(t.downChance, 0, 1e-9); T(t.attackerCount === 0);
  T(threatSummary(t) === "safe");
});
test("MoreAttackers_RaiseIncomingAndDownChance", () => {
  const target = sheet("Hero", 14, 10, 14);
  const one = threat(target, [{ attacker: sheet("A", 10, 14), ability: claw() }]);
  const three = threat(target, [0, 1, 2].map(() => ({ attacker: sheet("A", 10, 14), ability: claw() })));
  T(three.expectedIncoming > one.expectedIncoming, "more attackers => more incoming");
  T(three.downChance > one.downChance, "more attackers => higher down chance");
  T(three.attackerCount === 3);
});
test("HugeHpTarget_CannotBeDropped", () => {
  const t = threat(sheet("Tank", 12, 10, 100000), [{ attacker: sheet("A", 10, 18), ability: claw("1d12") }]);
  near(t.downChance, 0, 1e-9, "can't one-shot 100k HP");
});
test("HealersInThreatList_AreIgnored", () => {
  const heal = Ability({ abilityName: "Mend", isHeal: true, healDice: "2d8", damageDice: "" });
  const t = threat(sheet("Hero", 14, 10, 20), [{ attacker: sheet("Cleric", 10, 10), ability: heal }]);
  T(t.attackerCount === 0 && t.expectedIncoming === 0, "a heal is not a threat");
});

// the load-bearing cross-check: analytic == empirical, attackers focus-firing
test("Threat_MatchesResolver_OverManySeeds", () => {
  function cross(targetMaker, threats, trials = 40000) {
    const t = threat(targetMaker(), threats);
    let downs = 0, dmgTot = 0;
    const hp0 = targetMaker().currentHitPoints;
    for (let s = 0; s < trials; s++) {
      Dice.Seed(s);
      let sum = 0;
      const tgt = targetMaker();
      for (const { attacker, ability } of threats) sum += AttackResolver.Resolve(attacker, tgt, ability).damage; // no Apply: independent rolls vs full HP
      dmgTot += sum;
      if (sum >= hp0) downs++;
    }
    near(t.expectedIncoming, dmgTot / trials, 0.5, "mean incoming");
    near(t.downChance, downs / trials, 0.03, "down chance");
  }
  const A = () => sheet("A", 10, 14), B = () => sheet("B", 10, 16);
  cross(() => sheet("Hero", 14, 10, 12), [{ attacker: A(), ability: claw("1d6") }, { attacker: B(), ability: claw("1d8") }]);
  cross(() => sheet("Squishy", 12, 10, 8), [{ attacker: A(), ability: claw("1d6") }, { attacker: A(), ability: claw("1d6") }, { attacker: B(), ability: claw("1d10") }]);
  // include a save-for-half AoE attacker
  const blast = Ability({ abilityName: "Wail", isAttackRoll: false, saveAbility: "wisdom", saveForHalf: true, damageDice: "2d6", addAbilityModToDamage: false });
  cross(() => sheet("Hero", 14, 10, 15), [{ attacker: B(), ability: claw("1d8") }, { attacker: sheet("Boss", 10, 16), ability: blast }]);
});

console.log(`\n  Threat forecast — incoming damage + down-chance vs the real resolver:`);
console.log(`  ${pass} passed, ${fail} failed`);
if (fail) { fails.forEach(f => console.log("   ✗ " + f)); process.exit(1); }
else console.log("  ✓ The 'how likely am I to die here' readout matches the dice.\n");
