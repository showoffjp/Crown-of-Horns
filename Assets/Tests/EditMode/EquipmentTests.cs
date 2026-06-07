using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using SunderedCrown.Characters;
using SunderedCrown.Items;
using SunderedCrown.Stats;

namespace SunderedCrown.Tests.EditMode
{
    /// <summary>
    /// Equipping gear drives two derived stats: armor/shield AC, and the main-hand weapon
    /// becoming the character's basic attack (an ability minted from the item so designers
    /// only author items). These pin both hooks plus weapon-swap replacement and unequip.
    /// </summary>
    public class EquipmentTests
    {
        private readonly List<Object> _spawned = new List<Object>();

        private CharacterSheet Sheet()
        {
            var s = new CharacterSheet { displayName = "Wearer", baseArmorClass = 10 };
            s.abilities.Set(Ability.Dexterity, 10); // +0 ⇒ AC == base + gear
            return s;
        }

        private ItemDefinition Armor(string id, int acBonus, EquipSlot slot = EquipSlot.Body)
        {
            var it = ScriptableObject.CreateInstance<ItemDefinition>();
            it.itemId = id; it.displayName = id;
            it.kind = ItemKind.Armor; it.slot = slot; it.armorClassBonus = acBonus;
            _spawned.Add(it);
            return it;
        }

        private ItemDefinition Weapon(string id, string dmg, DamageType type = DamageType.Slashing)
        {
            var it = ScriptableObject.CreateInstance<ItemDefinition>();
            it.itemId = id; it.displayName = id;
            it.kind = ItemKind.Weapon; it.slot = EquipSlot.MainHand;
            it.weaponDamage = dmg; it.weaponDamageType = type;
            _spawned.Add(it);
            return it;
        }

        [TearDown]
        public void Cleanup()
        {
            foreach (var o in _spawned) if (o != null) Object.DestroyImmediate(o);
            _spawned.Clear();
        }

        [Test]
        public void EquipmentSlots_SetAndGetRoundTrip()
        {
            var e = new Equipment();
            var helm = Armor("helm", 1, EquipSlot.Head);
            e.Set(EquipSlot.Head, helm);
            Assert.AreSame(helm, e.Get(EquipSlot.Head));
            Assert.IsNull(e.Get(EquipSlot.Body));
        }

        [Test]
        public void EquipArmor_RaisesArmorClass()
        {
            var s = Sheet();
            int baseAc = s.ArmorClass; // 10
            EquipmentSystem.Equip(s, Armor("plate", 8));
            Assert.AreEqual(baseAc + 8, s.ArmorClass);
        }

        [Test]
        public void EquipWeapon_BecomesBasicAttack()
        {
            var s = Sheet();
            EquipmentSystem.Equip(s, Weapon("longsword", "1d8", DamageType.Slashing));

            Assert.IsNotNull(s.equippedWeaponAbility);
            Assert.AreEqual("1d8", s.equippedWeaponAbility.damageDice);
            Assert.AreEqual(DamageType.Slashing, s.equippedWeaponAbility.damageType);
            Assert.AreSame(s.equippedWeaponAbility, s.DefaultAttack);
            Assert.Contains(s.equippedWeaponAbility, s.knownAbilities);
        }

        [Test]
        public void EquippingSecondWeapon_ReplacesTheFirstAttack()
        {
            var s = Sheet();
            EquipmentSystem.Equip(s, Weapon("dagger", "1d4"));
            var firstAbility = s.equippedWeaponAbility;

            EquipmentSystem.Equip(s, Weapon("greataxe", "1d12"));

            Assert.AreNotSame(firstAbility, s.equippedWeaponAbility);
            Assert.AreEqual("1d12", s.equippedWeaponAbility.damageDice);
            CollectionAssert.DoesNotContain(s.knownAbilities, firstAbility,
                "The previous weapon's attack should be removed from the bar.");
        }

        [Test]
        public void Unequip_RemovesGearAC()
        {
            var s = Sheet();
            EquipmentSystem.Equip(s, Armor("shield", 2, EquipSlot.OffHand));
            Assert.AreEqual(12, s.ArmorClass);

            EquipmentSystem.Unequip(s, EquipSlot.OffHand);
            Assert.AreEqual(10, s.ArmorClass);
            Assert.IsNull(s.equipment.Get(EquipSlot.OffHand));
        }

        [Test]
        public void Equip_SlotlessItem_IsIgnored()
        {
            var s = Sheet();
            var trinket = ScriptableObject.CreateInstance<ItemDefinition>();
            trinket.itemId = "junk"; trinket.slot = EquipSlot.None; trinket.armorClassBonus = 5;
            _spawned.Add(trinket);

            EquipmentSystem.Equip(s, trinket);
            Assert.AreEqual(10, s.ArmorClass, "EquipSlot.None items must not apply.");
        }

        [Test]
        public void Equip_NullSheetOrItem_IsNoOp()
        {
            var s = Sheet();
            Assert.DoesNotThrow(() => EquipmentSystem.Equip(null, Armor("a", 1)));
            Assert.DoesNotThrow(() => EquipmentSystem.Equip(s, null));
            Assert.AreEqual(10, s.ArmorClass);
        }
    }
}
