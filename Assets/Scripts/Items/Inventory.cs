using System;
using System.Collections.Generic;

namespace SunderedCrown.Items
{
    /// <summary>Shared party inventory + gold. Plain class, serializes by item id.</summary>
    [Serializable]
    public class Inventory
    {
        [Serializable]
        public class Stack
        {
            public string itemId;
            public int count = 1;
        }

        public int gold = 0;
        public List<Stack> stacks = new List<Stack>();

        public event Action OnChanged;

        public void Add(ItemDefinition item, int count = 1)
        {
            if (item == null || count <= 0) return;
            if (item.stackable)
            {
                var existing = stacks.Find(s => s.itemId == item.itemId);
                if (existing != null) { existing.count += count; OnChanged?.Invoke(); return; }
            }
            stacks.Add(new Stack { itemId = item.itemId, count = count });
            OnChanged?.Invoke();
        }

        public bool Remove(string itemId, int count = 1)
        {
            var existing = stacks.Find(s => s.itemId == itemId);
            if (existing == null || existing.count < count) return false;
            existing.count -= count;
            if (existing.count <= 0) stacks.Remove(existing);
            OnChanged?.Invoke();
            return true;
        }

        public bool Has(string itemId, int count = 1)
        {
            var existing = stacks.Find(s => s.itemId == itemId);
            return existing != null && existing.count >= count;
        }

        public void AddGold(int amount) { gold = Math.Max(0, gold + amount); OnChanged?.Invoke(); }
    }
}
