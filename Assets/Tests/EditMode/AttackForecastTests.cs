using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using SunderedCrown.Characters;
using SunderedCrown.Combat;
using SunderedCrown.Stats;

namespace SunderedCrown.Tests.EditMode
{
    /// <summary>
    /// The combat HUD's hit-chance preview must not lie. These pin the closed-form 5e probabilities
    /// (to-hit vs AC, advantage/disadvantage, crit odds, save DCs) and — the load-bearing one — run the
    /// real <see cref="AttackResolver"/> over thousands of seeds and assert the forecast matches the
    /// observed dice: hit rate, mean damage, and kill rate all converge. If the resolver math ever drifts
    /// from the forecast, this goes red. All on hand-made sheets, headless.
    /// </summary>
    public class AttackForecastTests
    {
        private readonly List<Object> _spawned = new List<Object>();

        [TearDown]
        public void Cleanup()
        {
            foreach (var o in _spawned) if (o != null) Object.DestroyImmediate(o);
            _spawned.Clear();
        }

        private CharacterSheet Sheet(string name, int ac, int str = 10, int hp = 1000)
        {
            var s = new CharacterSheet { displayName = name, classDef = null, level = 1, baseArmorClass = ac };
            s.abilities.Set(Ability.Strength, str);
            s.abilities.Set(Ability.Dexterity, 10);
            s.abilities.Set(Ability.Intelligence, 10);
            s.maxHitPoints = hp; s.currentHitPoints = hp;
            return s;
        }
        private AbilityDefinition Weapon(string dice = "1d8", bool addMod = false)
        {
            var a = ScriptableObject.CreateInstance<AbilityDefinition>();
            a.abilityName = "Forecast Strike"; a.isAttackRoll = true; a.spellSlotLevel = 0; a.isHeal = false;
            a.damageDice = dice; a.addAbilityModToDamage = addMod; a.damageType = DamageType.Slashing; a.rangeTiles = 1;
            _spawned.Add(a); return a;
        }

        [Test]
        public void HitChance_MatchesHandComputedTable()
        {
            // toHit = strMod(0) + prof(2) = +2. vs AC 13 -> need face >= 11 -> faces 11..20 hit = 10/20.
            var fc = AttackForecast.Of(Sheet("A", 10, 10), Sheet("D", 13), Weapon());
            Assert.AreEqual(0.50f, fc.hitChance, 1e-4);
            Assert.AreEqual(0.05f, fc.critChance, 1e-4, "nat-20 only, on one die");
            Assert.IsFalse(fc.advantage); Assert.IsFalse(fc.disadvantage);
        }

        [Test]
        public void Advantage_RaisesHit_AndCrit()
        {
            var baseFc = AttackForecast.Of(Sheet("A", 10, 10), Sheet("D", 13), Weapon());
            var advFc = AttackForecast.Of(Sheet("A", 10, 10), Sheet("D", 13), Weapon(), extraAdvantage: true);
            Assert.IsTrue(advFc.advantage);
            Assert.AreEqual(1f - (1f - 0.5f) * (1f - 0.5f), advFc.hitChance, 1e-4, "advantage: 1-(1-p)^2");
            Assert.AreEqual(1f - 0.95f * 0.95f, advFc.critChance, 1e-4);
            Assert.Greater(advFc.hitChance, baseFc.hitChance);
        }

        [Test]
        public void CoverBonus_LowersHitChance()
        {
            var open = AttackForecast.Of(Sheet("A", 10, 10), Sheet("D", 13), Weapon());
            var cover = AttackForecast.Of(Sheet("A", 10, 10), Sheet("D", 13), Weapon(), targetAcBonus: 5);
            Assert.Less(cover.hitChance, open.hitChance);
        }

        [Test]
        public void Heal_ForecastsMeanWithinBounds()
        {
            var heal = ScriptableObject.CreateInstance<AbilityDefinition>();
            heal.abilityName = "Mend"; heal.isHeal = true; heal.healDice = "2d4"; _spawned.Add(heal);
            var fc = AttackForecast.Of(Sheet("C", 10, 10), Sheet("Ally", 10), heal);
            Assert.IsTrue(fc.isHeal);
            Assert.AreEqual(5f, fc.expectedHealing, 1e-4, "2d4 mean = 5, +0 INT mod");
            Assert.AreEqual(1f, fc.hitChance);
        }

        [Test]
        public void Save_FailChance_FollowsDC()
        {
            // attacker INT 10 -> saveDC = 8 + prof(2) + 0 = 10. target DEX 10 -> saveMod 0.
            // fail iff f + 0 < 10 -> faces 1..9 = 9/20.
            var spell = ScriptableObject.CreateInstance<AbilityDefinition>();
            spell.abilityName = "Blast"; spell.isAttackRoll = false; spell.saveAbility = Ability.Dexterity;
            spell.saveForHalf = true; spell.damageDice = "6d6"; spell.addAbilityModToDamage = false; _spawned.Add(spell);
            var fc = AttackForecast.Of(Sheet("A", 10, 10), Sheet("D", 10), spell);
            Assert.IsTrue(fc.isSave);
            Assert.AreEqual(9f / 20f, fc.hitChance, 1e-4);
        }

        [Test]
        public void Average_FollowsDiceNotation()
        {
            Assert.AreEqual(4.5f, AttackForecast.Average("1d8"), 1e-4);
            Assert.AreEqual(10.5f, AttackForecast.Average("2d6+3.5".Replace("3.5", "3")), 1e-4); // 7 + 3
            Assert.AreEqual(0f, AttackForecast.Average(""), 1e-4);
            Assert.AreEqual(0f, AttackForecast.Average("garbage"), 1e-4);
        }

        // The load-bearing test: forecast must equal what the dice actually do.
        [Test]
        public void Forecast_MatchesAttackResolver_OverManySeeds()
        {
            void CrossCheck(CharacterSheet atk, CharacterSheet def, AbilityDefinition ab, int trials = 40000)
            {
                var fc = AttackForecast.Of(atk, def, ab);
                int hits = 0, kills = 0; double dmg = 0;
                int hp0 = def.currentHitPoints;
                for (int s = 0; s < trials; s++)
                {
                    Dice.Seed(s);
                    var r = AttackResolver.Resolve(atk, def, ab);
                    if (r.hit) hits++;
                    dmg += r.damage;
                    if (r.damage >= hp0) kills++;
                }
                Assert.AreEqual(fc.hitChance, (float)hits / trials, 0.02f, "hit rate");
                Assert.AreEqual(fc.expectedDamage, (float)(dmg / trials), 0.4f, "mean damage");
                Assert.AreEqual(fc.lethalChance, (float)kills / trials, 0.03f, "kill rate");
            }

            CrossCheck(Sheet("Hero", 10, 16), Sheet("Brute", 13, 14, hp: 8), Weapon("1d8", addMod: true));
            CrossCheck(Sheet("Hero", 10, 18), Sheet("Tank", 17, 14, hp: 12), Weapon("1d12", addMod: true));
            CrossCheck(Sheet("Hero", 10, 14), Sheet("Frail", 11, 10, hp: 5), Weapon("2d6", addMod: true));
        }
    }
}
