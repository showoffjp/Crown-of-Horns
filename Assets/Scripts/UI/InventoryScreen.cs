using UnityEngine;
using SunderedCrown.Characters;
using SunderedCrown.Combat;
using SunderedCrown.Items;

namespace SunderedCrown.UI
{
    /// <summary>
    /// Inventory + equipment screen, toggled with the I key. Shows the shared party
    /// stash and the selected member's equipped slots; lets you equip/unequip gear and
    /// quaff consumables. OnGUI for zero setup — swap for a uGUI panel with item icons
    /// later (ItemDefinition already carries a Sprite icon).
    /// </summary>
    public class InventoryScreen : MonoBehaviour
    {
        public KeyCode toggleKey = KeyCode.I;
        private bool _open;
        private int _member;
        private Vector2 _scroll;

        void Update()
        {
            if (Input.GetKeyDown(toggleKey)) _open = !_open;
        }

        void OnGUI()
        {
            if (!_open || Party.Instance == null) return;
            var roster = Party.Instance.roster;
            if (roster.Count == 0) return;
            _member = Mathf.Clamp(_member, 0, roster.Count - 1);
            var m = roster[_member];

            const float w = 560, h = 520;
            float x = Screen.width / 2f - w / 2f, y = Screen.height / 2f - h / 2f;
            GUILayout.BeginArea(new Rect(x, y, w, h), GUI.skin.box);
            GUILayout.Label($"<b>INVENTORY</b>   ·   Gold: {Party.Instance.inventory.gold}      (press {toggleKey} to close)");

            // Member selector.
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("◀", GUILayout.Width(28))) _member = (_member - 1 + roster.Count) % roster.Count;
            GUILayout.Label($"<b>{m.displayName}</b>  Lv {m.level}  HP {m.currentHitPoints}/{m.maxHitPoints}  AC {m.ArmorClass}", GUILayout.Width(380));
            if (GUILayout.Button("▶", GUILayout.Width(28))) _member = (_member + 1) % roster.Count;
            GUILayout.EndHorizontal();

            // Equipped slots.
            GUILayout.Label("<b>Equipped</b>");
            DrawSlot(m, EquipSlot.MainHand, "Main Hand");
            DrawSlot(m, EquipSlot.OffHand, "Off Hand");
            DrawSlot(m, EquipSlot.Body, "Body");
            DrawSlot(m, EquipSlot.Head, "Head");
            DrawSlot(m, EquipSlot.Trinket, "Trinket");

            // Backpack.
            GUILayout.Label("<b>Backpack</b>");
            _scroll = GUILayout.BeginScrollView(_scroll, GUILayout.Height(220));
            var stacks = Party.Instance.inventory.stacks;
            if (stacks.Count == 0) GUILayout.Label("   (empty)");
            for (int i = stacks.Count - 1; i >= 0; i--)
            {
                var stack = stacks[i];
                var item = ItemDatabase.Get(stack.itemId);
                if (item == null) continue;
                GUILayout.BeginHorizontal();
                GUILayout.Label($"{item.displayName}  x{stack.count}   {ItemStat(item)}", GUILayout.Width(330));

                if (item.slot != EquipSlot.None && GUILayout.Button("Equip", GUILayout.Width(70)))
                    Equip(m, item);
                else if (item.kind == ItemKind.Consumable && item.useEffect != null && GUILayout.Button("Use", GUILayout.Width(70)))
                    Use(m, item);
                GUILayout.EndHorizontal();
            }
            GUILayout.EndScrollView();

            GUILayout.EndArea();
        }

        /// <summary>A compact stat tag for the backpack list — damage for weapons, AC for armor, else worth.</summary>
        private static string ItemStat(ItemDefinition item)
        {
            if (item.kind == ItemKind.Weapon)
                return $"<color=#d9b08c>{item.weaponDamage} {item.weaponDamageType}</color>";
            if (item.kind == ItemKind.Armor)
                return $"<color=#9cd1e8>+{item.armorClassBonus} AC</color>";
            return $"<color=#888><i>{item.kind}</i></color>";
        }

        private void DrawSlot(CharacterSheet m, EquipSlot slot, string label)
        {
            GUILayout.BeginHorizontal();
            var item = m.equipment.Get(slot);
            GUILayout.Label($"   {label}: {(item != null ? item.displayName : "—")}", GUILayout.Width(380));
            if (item != null && GUILayout.Button("Unequip", GUILayout.Width(90)))
            {
                EquipmentSystem.Unequip(m, slot);
                Party.Instance.inventory.Add(item);
            }
            GUILayout.EndHorizontal();
        }

        private void Equip(CharacterSheet m, ItemDefinition item)
        {
            var previous = m.equipment.Get(item.slot);
            EquipmentSystem.Equip(m, item);
            Party.Instance.inventory.Remove(item.itemId, 1);
            if (previous != null) Party.Instance.inventory.Add(previous);
        }

        private void Use(CharacterSheet m, ItemDefinition item)
        {
            var result = AttackResolver.Resolve(m, m, item.useEffect);
            AttackResolver.Apply(m, result);
            Party.Instance.inventory.Remove(item.itemId, 1);
        }
    }
}
