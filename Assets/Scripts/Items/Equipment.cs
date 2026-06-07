using System;
using UnityEngine;
using SunderedCrown.Characters;
using SunderedCrown.Stats;

namespace SunderedCrown.Items
{
    /// <summary>What a character has equipped, by slot. Runtime references.</summary>
    [Serializable]
    public class Equipment
    {
        public ItemDefinition mainHand;
        public ItemDefinition offHand;
        public ItemDefinition body;
        public ItemDefinition head;
        public ItemDefinition trinket;

        public ItemDefinition Get(EquipSlot slot) => slot switch
        {
            EquipSlot.MainHand => mainHand,
            EquipSlot.OffHand  => offHand,
            EquipSlot.Body     => body,
            EquipSlot.Head     => head,
            EquipSlot.Trinket  => trinket,
            _ => null
        };

        public void Set(EquipSlot slot, ItemDefinition item)
        {
            switch (slot)
            {
                case EquipSlot.MainHand: mainHand = item; break;
                case EquipSlot.OffHand:  offHand = item; break;
                case EquipSlot.Body:     body = item; break;
                case EquipSlot.Head:     head = item; break;
                case EquipSlot.Trinket:  trinket = item; break;
            }
        }
    }

    /// <summary>
    /// Applies equipment to a CharacterSheet: armor/shield contribute AC, and the
    /// main-hand weapon supplies the character's basic attack ability (created on the
    /// fly from the item's damage so designers only author items, not duplicate abilities).
    /// </summary>
    public static class EquipmentSystem
    {
        public static void Equip(CharacterSheet sheet, ItemDefinition item)
        {
            if (sheet == null || item == null || item.slot == EquipSlot.None) return;
            sheet.equipment.Set(item.slot, item);
            Recompute(sheet);
        }

        public static void Unequip(CharacterSheet sheet, EquipSlot slot)
        {
            if (sheet == null) return;
            sheet.equipment.Set(slot, null);
            Recompute(sheet);
        }

        private static void Recompute(CharacterSheet sheet)
        {
            // Sum armor/shield AC.
            int ac = 0;
            foreach (EquipSlot s in Enum.GetValues(typeof(EquipSlot)))
            {
                var it = sheet.equipment.Get(s);
                if (it != null) ac += it.armorClassBonus;
            }
            sheet.armorClassFromGear = ac;

            // Main-hand weapon becomes the basic attack.
            var weapon = sheet.equipment.mainHand;
            if (weapon != null && weapon.kind == ItemKind.Weapon)
            {
                var ability = WeaponAbility(weapon);
                // Replace the previously equipped weapon ability at the front of the bar.
                if (sheet.equippedWeaponAbility != null)
                    sheet.knownAbilities.Remove(sheet.equippedWeaponAbility);
                sheet.equippedWeaponAbility = ability;
                if (!sheet.knownAbilities.Contains(ability))
                    sheet.knownAbilities.Insert(0, ability);
            }
        }

        private static AbilityDefinition WeaponAbility(ItemDefinition weapon)
        {
            var a = ScriptableObject.CreateInstance<AbilityDefinition>();
            a.abilityName = weapon.displayName;
            a.damageDice = string.IsNullOrWhiteSpace(weapon.weaponDamage) ? "1d4" : weapon.weaponDamage;
            a.damageType = weapon.weaponDamageType;
            a.rangeTiles = 1;
            a.isAttackRoll = true;
            return a;
        }
    }
}
