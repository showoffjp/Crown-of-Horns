using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using SunderedCrown.Items;

namespace SunderedCrown.Tests.EditMode
{
    /// <summary>
    /// The shared party inventory: stackable merge vs distinct stacks, removal accounting,
    /// gold floored at zero, and change notifications the UI rides on. Plain serializable
    /// class, so it tests headless.
    /// </summary>
    public class InventoryTests
    {
        private readonly List<Object> _spawned = new List<Object>();

        private ItemDefinition Item(string id, bool stackable)
        {
            var it = ScriptableObject.CreateInstance<ItemDefinition>();
            it.itemId = id;
            it.displayName = id;
            it.stackable = stackable;
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
        public void Add_Stackable_MergesIntoOneStack()
        {
            var inv = new Inventory();
            var potion = Item("potion", stackable: true);
            inv.Add(potion, 2);
            inv.Add(potion, 3);

            Assert.AreEqual(1, inv.stacks.Count);
            Assert.IsTrue(inv.Has("potion", 5));
            Assert.IsFalse(inv.Has("potion", 6));
        }

        [Test]
        public void Add_NonStackable_CreatesSeparateStacks()
        {
            var inv = new Inventory();
            var sword = Item("sword", stackable: false);
            inv.Add(sword);
            inv.Add(sword);
            Assert.AreEqual(2, inv.stacks.Count);
        }

        [Test]
        public void Add_NullOrNonPositive_IsNoOp()
        {
            var inv = new Inventory();
            inv.Add(null);
            inv.Add(Item("x", true), 0);
            inv.Add(Item("y", true), -5);
            Assert.AreEqual(0, inv.stacks.Count);
        }

        [Test]
        public void Remove_ReducesCount_AndDropsEmptyStack()
        {
            var inv = new Inventory();
            inv.Add(Item("potion", true), 3);

            Assert.IsTrue(inv.Remove("potion", 2));
            Assert.IsTrue(inv.Has("potion", 1));

            Assert.IsTrue(inv.Remove("potion", 1));
            Assert.IsFalse(inv.Has("potion"));
            Assert.AreEqual(0, inv.stacks.Count);
        }

        [Test]
        public void Remove_MoreThanHeld_Fails()
        {
            var inv = new Inventory();
            inv.Add(Item("potion", true), 1);
            Assert.IsFalse(inv.Remove("potion", 2));
            Assert.IsTrue(inv.Has("potion", 1), "Failed removal must not mutate the stack.");
        }

        [Test]
        public void Remove_Unknown_Fails()
        {
            Assert.IsFalse(new Inventory().Remove("ghost"));
        }

        [Test]
        public void AddGold_AccumulatesAndFloorsAtZero()
        {
            var inv = new Inventory();
            inv.AddGold(100);
            Assert.AreEqual(100, inv.gold);
            inv.AddGold(-30);
            Assert.AreEqual(70, inv.gold);
            inv.AddGold(-9999);
            Assert.AreEqual(0, inv.gold, "Gold never goes negative.");
        }

        [Test]
        public void OnChanged_FiresForAddRemoveAndGold()
        {
            var inv = new Inventory();
            int fires = 0;
            inv.OnChanged += () => fires++;

            inv.Add(Item("potion", true), 1); // 1
            inv.Remove("potion", 1);          // 2
            inv.AddGold(5);                    // 3

            Assert.AreEqual(3, fires);
        }
    }
}
