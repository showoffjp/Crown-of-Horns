using NUnit.Framework;
using SunderedCrown.Content;
using SunderedCrown.Core;

namespace SunderedCrown.Tests.EditMode
{
    /// <summary>
    /// Deeds are achievements derived *purely* from flags the game already sets — no new
    /// hooks. These pin that a clean run earns nothing, that single/compound/threshold
    /// predicates fire correctly, and that the count tracks the list. Snapshots GameFlags.
    /// </summary>
    public class DeedsTests
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

        private static Deeds.Deed Find(string id) => Deeds.All_.Find(d => d.id == id);

        [Test]
        public void Total_MatchesTheList()
        {
            Assert.AreEqual(Deeds.All_.Count, Deeds.Total);
            Assert.Greater(Deeds.Total, 0);
        }

        [Test]
        public void CleanPlaythrough_EarnsNothing()
        {
            Assert.AreEqual(0, Deeds.EarnedCount());
        }

        [Test]
        public void SingleFlagDeed_FiresWhenItsFlagIsSet()
        {
            GameFlags.Current.SetBool("prologue.cleared", true);
            Assert.IsTrue(Find("returned").earned(GameFlags.Current));
            Assert.AreEqual(1, Deeds.EarnedCount());
        }

        [Test]
        public void CompoundDeed_RequiresEveryFlag()
        {
            var ages = Find("ages");
            GameFlags.Current.SetBool("netheril.cleared", true);
            GameFlags.Current.SetBool("crownwars.cleared", true);
            GameFlags.Current.SetBool("act4.toot_done", true);
            Assert.IsFalse(ages.earned(GameFlags.Current), "Three of four eras ⇒ not yet.");

            GameFlags.Current.SetBool("act4.spellplague_done", true);
            Assert.IsTrue(ages.earned(GameFlags.Current), "All four eras ⇒ earned.");
        }

        [Test]
        public void ThresholdDeed_FiresAtTheBoundary()
        {
            var hunter = Find("hunter"); // slain.total >= 25
            GameFlags.Current.SetInt("slain.total", 24);
            Assert.IsFalse(hunter.earned(GameFlags.Current));
            GameFlags.Current.SetInt("slain.total", 25);
            Assert.IsTrue(hunter.earned(GameFlags.Current));
        }

        [Test]
        public void EarnedCount_RisesAsDeedsAreSatisfied()
        {
            int before = Deeds.EarnedCount();
            GameFlags.Current.SetBool("prologue.cleared", true);
            GameFlags.Current.SetBool("aldric.met", true);
            Assert.AreEqual(before + 2, Deeds.EarnedCount());
        }
    }
}
