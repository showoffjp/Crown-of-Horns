using System.Collections.Generic;

namespace SunderedCrown.Items
{
    /// <summary>
    /// A simple id → ItemDefinition registry so the Inventory (which stores item ids) and
    /// UI can resolve names, icons, and stats. Content builders register their items here;
    /// in an asset-driven project you'd instead scan an Addressables label or a folder.
    /// </summary>
    public static class ItemDatabase
    {
        private static readonly Dictionary<string, ItemDefinition> _items = new Dictionary<string, ItemDefinition>();

        public static void Register(ItemDefinition item)
        {
            if (item != null && !string.IsNullOrEmpty(item.itemId)) _items[item.itemId] = item;
        }

        public static ItemDefinition Get(string itemId) =>
            (itemId != null && _items.TryGetValue(itemId, out var i)) ? i : null;

        public static IEnumerable<ItemDefinition> All => _items.Values;
        public static void Clear() => _items.Clear();
    }
}
