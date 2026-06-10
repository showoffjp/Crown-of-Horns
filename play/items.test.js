// InventoryTests + ItemDatabaseTests, ported 1:1 against the JS items core.
const { Item, Inventory, ItemDatabase } = require("./items.js");

let pass = 0, fail = 0; const fails = [];
function test(name, fn) { try { fn(); pass++; } catch (e) { fail++; fails.push(`${name}: ${e.message}`); } }
const eq = (a, b, m) => { if (a !== b) throw new Error(`${m || ""} expected ${b}, got ${a}`); };
const T = (c, m) => { if (!c) throw new Error(m || "expected true"); };
const Fa = (c, m) => { if (c) throw new Error(m || "expected false"); };
const same = (a, b, m) => { if (a !== b) throw new Error(m || "expected same reference"); };
const nul = (a, m) => { if (a !== null) throw new Error(m || "expected null"); };
const empty = (a, m) => { if (a.length !== 0) throw new Error(m || "expected empty"); };

// ===================== InventoryTests ======================================
test("Add_Stackable_MergesIntoOneStack", () => {
  const inv = new Inventory(); const potion = Item("potion", true);
  inv.Add(potion, 2); inv.Add(potion, 3);
  eq(inv.stacks.length, 1); T(inv.Has("potion", 5)); Fa(inv.Has("potion", 6));
});
test("Add_NonStackable_CreatesSeparateStacks", () => {
  const inv = new Inventory(); const sword = Item("sword", false);
  inv.Add(sword); inv.Add(sword); eq(inv.stacks.length, 2);
});
test("Add_NullOrNonPositive_IsNoOp", () => {
  const inv = new Inventory();
  inv.Add(null); inv.Add(Item("x", true), 0); inv.Add(Item("y", true), -5);
  eq(inv.stacks.length, 0);
});
test("Remove_ReducesCount_AndDropsEmptyStack", () => {
  const inv = new Inventory(); inv.Add(Item("potion", true), 3);
  T(inv.Remove("potion", 2)); T(inv.Has("potion", 1));
  T(inv.Remove("potion", 1)); Fa(inv.Has("potion")); eq(inv.stacks.length, 0);
});
test("Remove_MoreThanHeld_Fails", () => {
  const inv = new Inventory(); inv.Add(Item("potion", true), 1);
  Fa(inv.Remove("potion", 2)); T(inv.Has("potion", 1), "failed removal must not mutate");
});
test("Remove_Unknown_Fails", () => { Fa(new Inventory().Remove("ghost")); });
test("AddGold_AccumulatesAndFloorsAtZero", () => {
  const inv = new Inventory();
  inv.AddGold(100); eq(inv.gold, 100);
  inv.AddGold(-30); eq(inv.gold, 70);
  inv.AddGold(-9999); eq(inv.gold, 0, "gold never negative");
});
test("OnChanged_FiresForAddRemoveAndGold", () => {
  const inv = new Inventory(); let fires = 0; inv.onChanged(() => fires++);
  inv.Add(Item("potion", true), 1); inv.Remove("potion", 1); inv.AddGold(5);
  eq(fires, 3);
});

// ===================== ItemDatabaseTests ===================================
test("Register_ThenGet_ReturnsSameItem", () => {
  ItemDatabase.Clear(); const sword = Item("wpn.sword");
  ItemDatabase.Register(sword); same(ItemDatabase.Get("wpn.sword"), sword);
});
test("Get_UnknownId_ReturnsNull", () => { ItemDatabase.Clear(); nul(ItemDatabase.Get("does.not.exist")); });
test("Get_NullId_ReturnsNull", () => { ItemDatabase.Clear(); nul(ItemDatabase.Get(null)); });
test("Register_NullOrEmptyId_IsIgnored", () => {
  ItemDatabase.Clear(); ItemDatabase.Register(null); ItemDatabase.Register(Item(""));
  empty(ItemDatabase.All);
});
test("Register_DuplicateId_OverwritesWithLatest", () => {
  ItemDatabase.Clear(); const first = Item("dup"), second = Item("dup");
  ItemDatabase.Register(first); ItemDatabase.Register(second); same(ItemDatabase.Get("dup"), second);
});
test("Clear_EmptiesRegistry", () => {
  ItemDatabase.Clear(); ItemDatabase.Register(Item("x")); ItemDatabase.Clear();
  nul(ItemDatabase.Get("x")); empty(ItemDatabase.All);
});

console.log(`\n  Items core — Inventory + ItemDatabase (ported from your EditMode tests):`);
console.log(`  ${pass} passed, ${fail} failed`);
if (fail) { fails.forEach(f => console.log("   ✗ " + f)); process.exit(1); }
else console.log("  ✓ Loot, stacking, gold, and the id registry behave exactly as Unity demands.\n");
