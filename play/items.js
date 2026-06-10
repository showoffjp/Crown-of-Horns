// ============================================================================
// Items core — headless port of Items/Inventory.cs + Items/ItemDatabase.cs.
// Keeps loot/shops/equipment honest without Unity.
// ============================================================================

// ItemDefinition is a ScriptableObject in Unity; here a plain record.
const Item = (itemId, stackable = false) => ({ itemId, displayName: itemId || "(no id)", stackable });

// ---- Inventory.cs ----------------------------------------------------------
class Inventory {
  constructor() { this.gold = 0; this.stacks = []; this._listeners = []; }
  onChanged(cb) { this._listeners.push(cb); }      // subscribe (C# event OnChanged)
  _fire() { for (const l of this._listeners) l(); }
  Add(item, count = 1) {
    if (!item || count <= 0) return;
    if (item.stackable) {
      const ex = this.stacks.find(s => s.itemId === item.itemId);
      if (ex) { ex.count += count; this._fire(); return; }
    }
    this.stacks.push({ itemId: item.itemId, count });
    this._fire();
  }
  Remove(itemId, count = 1) {
    const ex = this.stacks.find(s => s.itemId === itemId);
    if (!ex || ex.count < count) return false;
    ex.count -= count;
    if (ex.count <= 0) this.stacks = this.stacks.filter(s => s !== ex);
    this._fire();
    return true;
  }
  Has(itemId, count = 1) {
    const ex = this.stacks.find(s => s.itemId === itemId);
    return !!ex && ex.count >= count;
  }
  AddGold(amount) { this.gold = Math.max(0, this.gold + amount); this._fire(); }
}

// ---- ItemDatabase.cs (static id -> item registry) --------------------------
const ItemDatabase = (() => {
  const m = new Map();
  return {
    Register(item) { if (item && item.itemId) m.set(item.itemId, item); },
    Get(itemId) { return (itemId != null && m.has(itemId)) ? m.get(itemId) : null; },
    get All() { return [...m.values()]; },
    Clear() { m.clear(); }
  };
})();

module.exports = { Item, Inventory, ItemDatabase };
