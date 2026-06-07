using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using SunderedCrown.Characters;
using SunderedCrown.Combat;
using SunderedCrown.Stats;

namespace SunderedCrown.Tests.EditMode
{
    /// <summary>
    /// Conditions are authored as data (StatusEffectDefinition) and resolved by the bearer's
    /// CharacterSheet. These pin that contract: applying sets the condition, DoT bites at the
    /// start of turn, durations count down and expire, re-applying refreshes rather than
    /// stacks, and the mechanical flags drive the right queries. DESIGN pillars III & IV.
    /// </summary>
    public class StatusEffectTests
    {
        private readonly List<Object> _spawned = new List<Object>();

        private StatusEffectDefinition Make(
            Condition condition = Condition.Poisoned,
            string dot = "",
            bool incapacitates = false,
            bool attackersHaveAdvantage = false,
            bool bearerAttacksDisadvantage = false,
            int attackRollModifier = 0,
            int armorClassModifier = 0,
            int durationRounds = 2)
        {
            var d = ScriptableObject.CreateInstance<StatusEffectDefinition>();
            d.effectName = condition.ToString();
            d.condition = condition;
            d.damageOverTimeDice = dot;
            d.incapacitates = incapacitates;
            d.attackersHaveAdvantage = attackersHaveAdvantage;
            d.bearerAttacksDisadvantage = bearerAttacksDisadvantage;
            d.attackRollModifier = attackRollModifier;
            d.armorClassModifier = armorClassModifier;
            d.durationRounds = durationRounds;
            _spawned.Add(d);
            return d;
        }

        private CharacterSheet Sheet(int hp = 100)
        {
            var s = new CharacterSheet { displayName = "Bearer", maxHitPoints = hp, currentHitPoints = hp };
            s.abilities.Set(Ability.Dexterity, 10);
            return s;
        }

        [TearDown]
        public void Cleanup()
        {
            foreach (var o in _spawned) if (o != null) Object.DestroyImmediate(o);
            _spawned.Clear();
        }

        [Test]
        public void AddEffect_SetsCondition()
        {
            var s = Sheet();
            s.AddEffect(Make(Condition.Poisoned), 2, "src");
            Assert.IsTrue(s.HasCondition(Condition.Poisoned));
            Assert.IsFalse(s.HasCondition(Condition.Stunned));
        }

        [Test]
        public void AddEffect_Null_IsNoOp()
        {
            var s = Sheet();
            s.AddEffect(null, 2, "src");
            Assert.AreEqual(0, s.activeEffects.Count);
        }

        [Test]
        public void DamageOverTime_BitesAtStartOfTurn()
        {
            var s = Sheet(100);
            s.AddEffect(Make(Condition.Burning, dot: "1d6"), 3, "src");

            Dice.Seed(99);
            int dealt = s.TickStartOfTurn();
            Assert.GreaterOrEqual(dealt, 1);
            Assert.LessOrEqual(dealt, 6);
            Assert.AreEqual(100 - dealt, s.currentHitPoints);
        }

        [Test]
        public void Duration_CountsDownThenExpires()
        {
            var s = Sheet();
            s.AddEffect(Make(Condition.Prone, durationRounds: 2), 2, "src");

            s.TickEndOfTurn();
            Assert.IsTrue(s.HasCondition(Condition.Prone), "Still active after one round.");
            s.TickEndOfTurn();
            Assert.IsFalse(s.HasCondition(Condition.Prone), "Expired after two rounds.");
            Assert.AreEqual(0, s.activeEffects.Count);
        }

        [Test]
        public void ReApplyingSameDef_RefreshesInsteadOfStacking()
        {
            var s = Sheet();
            var poison = Make(Condition.Poisoned);
            s.AddEffect(poison, 2, "src");
            s.TickEndOfTurn();                 // remaining → 1
            s.AddEffect(poison, 2, "src");     // refresh back up to 2, not a second instance

            Assert.AreEqual(1, s.activeEffects.Count, "Same definition must not stack.");
            Assert.AreEqual(2, s.activeEffects[0].remainingRounds);
        }

        [Test]
        public void Incapacitation_DrivesQueries()
        {
            var s = Sheet();
            s.AddEffect(Make(Condition.Stunned, incapacitates: true), 2, "src");
            Assert.IsTrue(s.IsIncapacitated);
            Assert.IsTrue(s.AttacksAtDisadvantage, "Incapacitated bearers attack at disadvantage.");
        }

        [Test]
        public void AttackersHaveAdvantage_Flag_IsExposed()
        {
            var s = Sheet();
            s.AddEffect(Make(Condition.Prone, attackersHaveAdvantage: true), 2, "src");
            Assert.IsTrue(s.GrantsAdvantageToAttackers);
        }

        [Test]
        public void AttackRollModifiers_SumAcrossEffects()
        {
            var s = Sheet();
            s.AddEffect(Make(Condition.Blessed, attackRollModifier: 2), 2, "a");
            s.AddEffect(Make(Condition.Hasted, attackRollModifier: 1), 2, "b");
            Assert.AreEqual(3, s.EffectAttackModifier);
        }

        [Test]
        public void ArmorClassModifier_AltersArmorClass()
        {
            var s = Sheet();
            int baseAc = s.ArmorClass;
            s.AddEffect(Make(Condition.Blessed, armorClassModifier: 2), 2, "src");
            Assert.AreEqual(baseAc + 2, s.ArmorClass);
        }

        [Test]
        public void RemoveCondition_DropsMatchingEffect()
        {
            var s = Sheet();
            s.AddEffect(Make(Condition.Poisoned), 5, "src");
            s.RemoveCondition(Condition.Poisoned);
            Assert.IsFalse(s.HasCondition(Condition.Poisoned));
        }
    }
}
