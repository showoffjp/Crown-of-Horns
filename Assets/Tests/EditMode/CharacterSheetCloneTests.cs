using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using SunderedCrown.Characters;
using SunderedCrown.Combat;
using SunderedCrown.Stats;

namespace SunderedCrown.Tests.EditMode
{
    /// <summary>
    /// CharacterSheet.Clone powers the Echoes — the Crown Wars mirror-fight against twisted
    /// copies of your own party. A clone must share the immutable build but get a fresh id,
    /// full HP, an independent ability list, and NO carried-over status effects. These pin
    /// exactly that so an Echo can never accidentally share state with its original.
    /// </summary>
    public class CharacterSheetCloneTests
    {
        private readonly List<Object> _spawned = new List<Object>();

        [TearDown]
        public void Cleanup()
        {
            foreach (var o in _spawned) if (o != null) Object.DestroyImmediate(o);
            _spawned.Clear();
        }

        private CharacterSheet Original()
        {
            var s = new CharacterSheet
            {
                displayName = "Hero",
                level = 3,
                baseArmorClass = 12,
                armorClassFromGear = 2,
                maxHitPoints = 30,
                currentHitPoints = 10, // damaged — clone should come back full
            };
            s.abilities.Set(Ability.Strength, 16);
            s.abilities.Set(Ability.Dexterity, 14);
            s.spellSlots.max[1] = 3; s.spellSlots.current[1] = 1;

            var ability = ScriptableObject.CreateInstance<AbilityDefinition>();
            ability.abilityName = "Strike";
            _spawned.Add(ability);
            s.knownAbilities.Add(ability);

            var effect = ScriptableObject.CreateInstance<StatusEffectDefinition>();
            effect.effectName = "Poisoned"; effect.condition = Condition.Poisoned;
            _spawned.Add(effect);
            s.AddEffect(effect, 3, "src"); // must NOT carry to the clone
            return s;
        }

        [Test]
        public void Clone_GetsAFreshId()
        {
            var a = Original();
            var c = a.Clone();
            Assert.AreNotEqual(a.id, c.id);
            Assert.IsFalse(string.IsNullOrEmpty(c.id));
        }

        [Test]
        public void Clone_CopiesTheBuild()
        {
            var a = Original();
            var c = a.Clone();
            Assert.AreEqual("Hero", c.displayName);
            Assert.AreEqual(3, c.level);
            Assert.AreEqual(12, c.baseArmorClass);
            Assert.AreEqual(2, c.armorClassFromGear);
            Assert.AreEqual(16, c.abilities.Get(Ability.Strength));
            Assert.AreEqual(14, c.abilities.Get(Ability.Dexterity));
        }

        [Test]
        public void Clone_StartsAtFullHealth()
        {
            var c = Original().Clone();
            Assert.AreEqual(c.maxHitPoints, c.currentHitPoints, "An Echo enters the fight whole.");
            Assert.AreEqual(30, c.currentHitPoints);
        }

        [Test]
        public void Clone_KnownAbilities_IsAnIndependentList()
        {
            var a = Original();
            var c = a.Clone();
            Assert.AreEqual(a.knownAbilities.Count, c.knownAbilities.Count);

            c.knownAbilities.Clear();
            Assert.AreEqual(1, a.knownAbilities.Count, "Mutating the clone's list must not touch the original.");
        }

        [Test]
        public void Clone_DoesNotInheritActiveEffects()
        {
            var a = Original();
            Assert.AreEqual(1, a.activeEffects.Count);
            var c = a.Clone();
            Assert.AreEqual(0, c.activeEffects.Count, "An Echo starts free of the original's conditions.");
        }

        [Test]
        public void Clone_CopiesSpellSlots_AsSeparateArrays()
        {
            var a = Original();
            var c = a.Clone();
            Assert.AreEqual(3, c.spellSlots.max[1]);

            c.spellSlots.max[1] = 9;
            Assert.AreEqual(3, a.spellSlots.max[1], "Spell-slot arrays must be copied, not shared.");
        }
    }
}
