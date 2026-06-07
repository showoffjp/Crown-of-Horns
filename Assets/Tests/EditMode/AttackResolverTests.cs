using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using SunderedCrown.Characters;
using SunderedCrown.Combat;
using SunderedCrown.Stats;

namespace SunderedCrown.Tests.EditMode
{
    /// <summary>
    /// The core of DESIGN pillar IV — "5e you can trust." These pin the rules players play
    /// to: nat-20 always hits and crits, nat-1 always misses, crits double the dice, saves
    /// halve damage, healing is mod-boosted, and identical seeds reproduce identical results.
    /// All built on hand-made sheets — no scene — so they run headless in CI.
    /// </summary>
    public class AttackResolverTests
    {
        private readonly List<Object> _spawned = new List<Object>();

        [TearDown]
        public void Cleanup()
        {
            foreach (var o in _spawned) if (o != null) Object.DestroyImmediate(o);
            _spawned.Clear();
        }

        // --- builders -------------------------------------------------------

        private CharacterSheet MakeSheet(string name, int ac, int str = 10, int hp = 1000)
        {
            var s = new CharacterSheet { displayName = name, classDef = null, level = 1, baseArmorClass = ac };
            s.abilities.Set(Ability.Strength, str);
            s.abilities.Set(Ability.Dexterity, 10);     // +0 → AC == baseArmorClass
            s.abilities.Set(Ability.Intelligence, 10);  // +0 → clean heal math (no classDef ⇒ INT casting)
            s.maxHitPoints = hp;
            s.currentHitPoints = hp;
            return s;
        }

        private AbilityDefinition MakeWeapon(string dice = "1d6")
        {
            var a = ScriptableObject.CreateInstance<AbilityDefinition>();
            a.abilityName = "Test Strike";
            a.isAttackRoll = true;
            a.spellSlotLevel = 0;
            a.isHeal = false;
            a.damageDice = dice;
            a.addAbilityModToDamage = false;   // isolate dice from ability mod
            a.damageType = DamageType.Slashing;
            a.rangeTiles = 1;
            _spawned.Add(a);
            return a;
        }

        // Smallest seed whose *first* d20 shows the wanted face (Resolve's first roll for a
        // no-advantage attack is exactly one Dice.D20()).
        private int SeedForFirstD20(int face)
        {
            for (int s = 0; s < 1_000_000; s++)
            {
                Dice.Seed(s);
                if (Dice.D20() == face) return s;
            }
            Assert.Fail($"No seed produced a first d20 of {face}.");
            return -1;
        }

        // --- tests ----------------------------------------------------------

        [Test]
        public void NaturalTwenty_AlwaysHitsAndCrits_EvenVsAbsurdAC()
        {
            var atk = MakeSheet("Atk", ac: 10);
            var def = MakeSheet("Def", ac: 90);   // unhittable by the math alone
            var wpn = MakeWeapon();

            Dice.Seed(SeedForFirstD20(20));
            var r = AttackResolver.Resolve(atk, def, wpn);

            Assert.AreEqual(20, r.attackRoll);
            Assert.IsTrue(r.critical, "Nat-20 must flag a critical.");
            Assert.IsTrue(r.hit, "Nat-20 must hit regardless of AC.");
        }

        [Test]
        public void NaturalOne_AlwaysMisses_EvenVsTrivialAC()
        {
            var atk = MakeSheet("Atk", ac: 10, str: 20); // big bonus
            var def = MakeSheet("Def", ac: 1);           // trivial AC
            var wpn = MakeWeapon();

            Dice.Seed(SeedForFirstD20(1));
            var r = AttackResolver.Resolve(atk, def, wpn);

            Assert.AreEqual(1, r.attackRoll);
            Assert.IsFalse(r.hit, "Nat-1 must miss regardless of bonuses.");
        }

        [Test]
        public void Critical_DoublesTheDamageDice()
        {
            // AC 4 with a +2 proficiency means any non-1 roll lands, so we bucket cleanly
            // into crit hits (2d6) vs ordinary hits (1d6) and compare averages.
            var atk = MakeSheet("Atk", ac: 10, str: 10);
            var def = MakeSheet("Def", ac: 4);
            var wpn = MakeWeapon("1d6");

            double critSum = 0, hitSum = 0;
            int critN = 0, hitN = 0;
            for (int s = 0; s < 6000; s++)
            {
                Dice.Seed(s);
                var r = AttackResolver.Resolve(atk, def, wpn);
                if (r.critical) { critSum += r.damage; critN++; }
                else if (r.hit) { hitSum += r.damage; hitN++; }
            }

            Assert.Greater(critN, 0, "Expected some critical hits across 6000 seeds.");
            Assert.Greater(hitN, 0, "Expected some ordinary hits across 6000 seeds.");
            double critAvg = critSum / critN, hitAvg = hitSum / hitN;
            Assert.Greater(critAvg, hitAvg * 1.5,
                $"Crit avg {critAvg:F2} should ~double ordinary-hit avg {hitAvg:F2} (dice doubled).");
        }

        [Test]
        public void SaveForHalf_DealsHalfOnSuccess_FullOnFailure()
        {
            var atk = MakeSheet("Atk", ac: 10);
            var def = MakeSheet("Def", ac: 10);
            var spell = ScriptableObject.CreateInstance<AbilityDefinition>();
            spell.abilityName = "Test Blast";
            spell.isAttackRoll = false;       // saving-throw ability
            spell.saveAbility = Ability.Dexterity;
            spell.saveForHalf = true;
            spell.damageDice = "6d6";
            spell.addAbilityModToDamage = false;
            spell.damageType = DamageType.Fire;
            _spawned.Add(spell);

            double failSum = 0, saveSum = 0;
            int failN = 0, saveN = 0;
            for (int s = 0; s < 4000; s++)
            {
                Dice.Seed(s);
                var r = AttackResolver.Resolve(atk, def, spell);
                if (r.hit) { failSum += r.damage; failN++; }       // hit == failed save → full
                else if (r.damage > 0) { saveSum += r.damage; saveN++; } // saved → half
            }

            Assert.Greater(failN, 0, "Expected some failed saves (full damage).");
            Assert.Greater(saveN, 0, "Expected some successful saves still taking half.");
            Assert.Greater(failSum / failN, saveSum / saveN,
                "Failed-save damage should exceed saved (halved) damage on average.");
        }

        [Test]
        public void Healing_IsPositive_AndFlaggedAsHeal()
        {
            var caster = MakeSheet("Cleric", ac: 10);
            var ally = MakeSheet("Ally", ac: 10);
            var heal = ScriptableObject.CreateInstance<AbilityDefinition>();
            heal.abilityName = "Test Mend";
            heal.isHeal = true;
            heal.healDice = "2d4";          // 2..8, +0 INT mod
            _spawned.Add(heal);

            for (int s = 0; s < 500; s++)
            {
                Dice.Seed(s);
                var r = AttackResolver.Resolve(caster, ally, heal);
                Assert.IsTrue(r.isHeal);
                Assert.IsTrue(r.hit);
                Assert.GreaterOrEqual(r.healing, 2);
                Assert.LessOrEqual(r.healing, 8);
            }
        }

        [Test]
        public void Resolve_IsDeterministicUnderSameSeed()
        {
            var atk = MakeSheet("Atk", ac: 10, str: 14);
            var def = MakeSheet("Def", ac: 13);
            var wpn = MakeWeapon("1d8");

            Dice.Seed(4242);
            var a = AttackResolver.Resolve(atk, def, wpn);
            Dice.Seed(4242);
            var b = AttackResolver.Resolve(atk, def, wpn);

            Assert.AreEqual(a.attackRoll, b.attackRoll);
            Assert.AreEqual(a.hit, b.hit);
            Assert.AreEqual(a.critical, b.critical);
            Assert.AreEqual(a.damage, b.damage);
        }
    }
}
