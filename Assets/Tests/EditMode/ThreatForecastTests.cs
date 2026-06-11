using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using SunderedCrown.Characters;
using SunderedCrown.Combat;
using SunderedCrown.Stats;

namespace SunderedCrown.Tests.EditMode
{
    /// <summary>
    /// The defensive preview ("if these foes all reach you, how likely are you to drop this round?")
    /// must match the dice. These pin the monotonic behaviour (more attackers → more danger), the
    /// can't-one-shot-a-titan guard, that heals aren't threats — and, the load-bearing one, a
    /// Monte-Carlo cross-check: roll the real <see cref="AttackResolver"/> for every attacker
    /// focus-firing the target and assert the observed incoming-damage and down-rate converge to the
    /// analytic <see cref="ThreatForecast"/>. Headless, hand-made sheets.
    /// </summary>
    public class ThreatForecastTests
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
        private AbilityDefinition Claw(string dice = "1d6")
        {
            var a = ScriptableObject.CreateInstance<AbilityDefinition>();
            a.abilityName = "Claw"; a.isAttackRoll = true; a.damageDice = dice; a.addAbilityModToDamage = true;
            a.damageType = DamageType.Slashing; _spawned.Add(a); return a;
        }
        private (CharacterSheet, AbilityDefinition) Pair(CharacterSheet a, AbilityDefinition b) => (a, b);

        [Test]
        public void NoAttackers_IsSafe()
        {
            var t = ThreatForecast.Against(Sheet("Hero", 16, 10, 20), new List<(CharacterSheet, AbilityDefinition)>());
            Assert.AreEqual(0, t.attackerCount);
            Assert.AreEqual(0f, t.expectedIncoming, 1e-4);
            Assert.AreEqual(0f, t.downChance, 1e-4);
            Assert.AreEqual("safe", t.Summary());
        }

        [Test]
        public void MoreAttackers_RaiseIncomingAndDownChance()
        {
            var target = Sheet("Hero", 14, 10, 14);
            var one = ThreatForecast.Against(target, new List<(CharacterSheet, AbilityDefinition)> { Pair(Sheet("A", 10, 14), Claw()) });
            var three = ThreatForecast.Against(target, new List<(CharacterSheet, AbilityDefinition)> {
                Pair(Sheet("A", 10, 14), Claw()), Pair(Sheet("B", 10, 14), Claw()), Pair(Sheet("C", 10, 14), Claw()) });
            Assert.Greater(three.expectedIncoming, one.expectedIncoming);
            Assert.Greater(three.downChance, one.downChance);
            Assert.AreEqual(3, three.attackerCount);
        }

        [Test]
        public void HugeHpTarget_CannotBeDropped()
        {
            var t = ThreatForecast.Against(Sheet("Titan", 12, 10, 100000),
                new List<(CharacterSheet, AbilityDefinition)> { Pair(Sheet("A", 10, 18), Claw("1d12")) });
            Assert.AreEqual(0f, t.downChance, 1e-4);
        }

        [Test]
        public void Heals_AreNotThreats()
        {
            var heal = ScriptableObject.CreateInstance<AbilityDefinition>();
            heal.abilityName = "Mend"; heal.isHeal = true; heal.healDice = "2d8"; _spawned.Add(heal);
            var t = ThreatForecast.Against(Sheet("Hero", 14, 10, 20),
                new List<(CharacterSheet, AbilityDefinition)> { Pair(Sheet("Cleric", 10, 10), heal) });
            Assert.AreEqual(0, t.attackerCount);
            Assert.AreEqual(0f, t.expectedIncoming, 1e-4);
        }

        [Test]
        public void Threat_MatchesResolver_OverManySeeds()
        {
            void Cross(System.Func<CharacterSheet> targetMaker, List<(CharacterSheet a, AbilityDefinition b)> threats, int trials = 40000)
            {
                var t = ThreatForecast.Against(targetMaker(),
                    threats.ConvertAll(p => (p.a, p.b)));
                int hp0 = targetMaker().currentHitPoints;
                int downs = 0; double dmg = 0;
                for (int s = 0; s < trials; s++)
                {
                    Dice.Seed(s);
                    int sum = 0;
                    var tgt = targetMaker(); // fresh full-HP target → independent rolls vs initial HP
                    foreach (var (a, b) in threats) sum += AttackResolver.Resolve(a, tgt, b).damage;
                    dmg += sum;
                    if (sum >= hp0) downs++;
                }
                Assert.AreEqual(t.expectedIncoming, (float)(dmg / trials), 0.5f, "mean incoming");
                Assert.AreEqual(t.downChance, (float)downs / trials, 0.03f, "down chance");
            }

            Cross(() => Sheet("Hero", 14, 10, 12), new List<(CharacterSheet, AbilityDefinition)> {
                Pair(Sheet("A", 10, 14), Claw("1d6")), Pair(Sheet("B", 10, 16), Claw("1d8")) });
            Cross(() => Sheet("Squishy", 12, 10, 8), new List<(CharacterSheet, AbilityDefinition)> {
                Pair(Sheet("A", 10, 14), Claw("1d6")), Pair(Sheet("A2", 10, 14), Claw("1d6")), Pair(Sheet("B", 10, 16), Claw("1d10")) });
        }
    }
}
