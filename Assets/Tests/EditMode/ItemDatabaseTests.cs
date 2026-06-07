using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using SunderedCrown.Items;

namespace SunderedCrown.Tests.EditMode
{
    /// <summary>
    /// The Inventory stores item *ids* and resolves them through this registry, so its
    /// integrity (round-trip, null-safety, no phantom empty-id entries) keeps loot, shops,
    /// and equipment honest. The DB is a static singleton, so each test clears it.
    /// </summary>
    public class ItemDatabaseTests
    {
        private readonly List<Object> _spawned = new List<Object>();

        private ItemDefinition MakeItem(string id)
        {
            var it = ScriptableObject.CreateInstance<ItemDefinition>();
            it.itemId = id;
            it.displayName = string.IsNullOrEmpty(id) ? "(no id)" : id;
            _spawned.Add(it);
            return it;
        }

        [SetUp]
        public void Reset() => ItemDatabase.Clear();

        [TearDown]
        public void Teardown()
        {
            ItemDatabase.Clear();
            foreach (var o in _spawned) if (o != null) Object.DestroyImmediate(o);
            _spawned.Clear();
        }

        [Test]
        public void Register_ThenGet_ReturnsSameItem()
        {
            var sword = MakeItem("wpn.sword");
            ItemDatabase.Register(sword);
            Assert.AreSame(sword, ItemDatabase.Get("wpn.sword"));
        }

        [Test]
        public void Get_UnknownId_ReturnsNull() => Assert.IsNull(ItemDatabase.Get("does.not.exist"));

        [Test]
        public void Get_NullId_ReturnsNull() => Assert.IsNull(ItemDatabase.Get(null));

        [Test]
        public void Register_NullOrEmptyId_IsIgnored()
        {
            ItemDatabase.Register(null);
            ItemDatabase.Register(MakeItem(""));   // empty id → must not register
            Assert.IsEmpty(new List<ItemDefinition>(ItemDatabase.All));
        }

        [Test]
        public void Register_DuplicateId_OverwritesWithLatest()
        {
            var first = MakeItem("dup");
            var second = MakeItem("dup");
            ItemDatabase.Register(first);
            ItemDatabase.Register(second);
            Assert.AreSame(second, ItemDatabase.Get("dup"));
        }

        [Test]
        public void Clear_EmptiesRegistry()
        {
            ItemDatabase.Register(MakeItem("x"));
            ItemDatabase.Clear();
            Assert.IsNull(ItemDatabase.Get("x"));
            Assert.IsEmpty(new List<ItemDefinition>(ItemDatabase.All));
        }
    }
}
