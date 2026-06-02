using UnityEngine;
using SunderedCrown.Stats;

namespace SunderedCrown.Items
{
    public enum ItemKind { Weapon, Armor, Shield, Consumable, Quest, Misc }
    public enum EquipSlot { None, MainHand, OffHand, Body, Head, Trinket }

    /// <summary>
    /// Data-only item. Weapons/armor define their combat contribution; consumables
    /// reference an AbilityDefinition for their effect.
    ///
    /// Create via: Assets > Create > Sundered Crown > Item
    /// </summary>
    [CreateAssetMenu(menuName = "Sundered Crown/Item", fileName = "NewItem")]
    public class ItemDefinition : ScriptableObject
    {
        [Header("Identity")]
        public string itemId;
        public string displayName;
        [TextArea(2, 4)] public string description;
        public Sprite icon;
        public ItemKind kind = ItemKind.Misc;
        public int valueGold = 0;
        public bool stackable = false;

        [Header("Equipment")]
        public EquipSlot slot = EquipSlot.None;
        [Tooltip("Weapon damage dice, e.g. '1d8'.")]
        public string weaponDamage = "1d8";
        public DamageType weaponDamageType = DamageType.Slashing;
        [Tooltip("Flat AC granted by armor/shield.")]
        public int armorClassBonus = 0;

        [Header("Consumable")]
        public Characters.AbilityDefinition useEffect;
    }
}
