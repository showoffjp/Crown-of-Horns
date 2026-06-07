using System.Linq;
using NUnit.Framework;
using SunderedCrown.Content;
using SunderedCrown.Core;

namespace SunderedCrown.Tests.EditMode
{
    /// <summary>
    /// Keepsakes are story made tangible — flag-gated mementos that appear when a bond
    /// resolves. These pin that they stay hidden until earned, surface on the right flag
    /// (including the intimate romance tier), and that the catalog has unique ids.
    /// </summary>
    public class KeepsakesTests
    {
        private GameFlags _original;

        [SetUp]
        public void SetUp()
        {
            _original = GameFlags.Current;
            GameFlags.Replace(new GameFlags());
        }

        [TearDown]
        public void TearDown() => GameFlags.Replace(_original ?? new GameFlags());

        [Test]
        public void CleanRun_ShowsOnlyAlwaysUnlockedKeepsakes()
        {
            int alwaysOn = Keepsakes.All.Count(k => string.IsNullOrEmpty(k.unlockFlag));
            Assert.AreEqual(alwaysOn, Keepsakes.Earned().Count,
                "Before any bond resolves, only unconditionally-unlocked keepsakes show.");
        }

        [Test]
        public void CompanionKeepsake_AppearsOnQuestResolution()
        {
            GameFlags.Current.SetBool("quest.garrow.resolved", true);
            CollectionAssert.Contains(Keepsakes.Earned().Select(k => k.id).ToList(), "garrow_whetstone");
        }

        [Test]
        public void RomanceKeepsake_RequiresAConsummatedBond()
        {
            Assert.IsFalse(Keepsakes.Earned().Any(k => k.id == "naeve_axiom"), "Hidden before romance.");
            GameFlags.Current.SetBool("romance.naeve.consummated", true);
            Assert.IsTrue(Keepsakes.Earned().Any(k => k.id == "naeve_axiom"), "Appears once the bond is sealed.");
        }

        [Test]
        public void Earned_GrowsMonotonicallyAsFlagsFlip()
        {
            int start = Keepsakes.Earned().Count;
            GameFlags.Current.SetBool("quest.roen.resolved", true);
            GameFlags.Current.SetBool("quest.varra.resolved", true);
            Assert.AreEqual(start + 2, Keepsakes.Earned().Count);
        }

        [Test]
        public void Catalog_HasUniqueIds()
        {
            var ids = Keepsakes.All.Select(k => k.id).ToList();
            CollectionAssert.AllItemsAreUnique(ids);
            Assert.Greater(ids.Count, 0);
        }
    }
}
