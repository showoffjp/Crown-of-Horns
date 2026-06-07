using NUnit.Framework;
using SunderedCrown.Core;

namespace SunderedCrown.Tests.EditMode
{
    /// <summary>
    /// The finale brain — DESIGN pillar II's payoff. Endings must be *earned*: the deeper
    /// truths gate the deeper roads. These pin the gating from a synthetic playthrough's
    /// flags and assert every ending carries its prose. Mutates the global GameFlags, so it
    /// snapshots and restores it around each test.
    /// </summary>
    public class EndingResolverTests
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
        public void Available_Baseline_OffersTheThreeCommonEndings()
        {
            var available = EndingResolver.Available();
            CollectionAssert.AreEquivalent(
                new[] { Ending.Abolition, Ending.ReturnedThrone, Ending.MortalMeasure },
                available);
            // No golden roads without understanding.
            CollectionAssert.DoesNotContain(available, Ending.JergalsKeyhole);
            CollectionAssert.DoesNotContain(available, Ending.BreakTheLoop);
        }

        [Test]
        public void DoomguidesPeace_UnlocksWithKelemvorReputation()
        {
            GameFlags.Current.SetInt("faction.kelemvor.reputation", 5);
            CollectionAssert.Contains(EndingResolver.Available(), Ending.DoomguidesPeace);
        }

        [Test]
        public void DoomguidesPeace_UnlocksByNamingTheMonster()
        {
            GameFlags.Current.SetBool("aldric.named_monster", true);
            CollectionAssert.Contains(EndingResolver.Available(), Ending.DoomguidesPeace);
        }

        [Test]
        public void JergalsKeyhole_NeedsBothUnderstandingAndASparedVerdict()
        {
            GameFlags.Current.SetBool("readers_boon", true);
            CollectionAssert.DoesNotContain(EndingResolver.Available(), Ending.JergalsKeyhole);

            GameFlags.Current.SetBool("crownwars.verdict_spared", true);
            CollectionAssert.Contains(EndingResolver.Available(), Ending.JergalsKeyhole);
            Assert.IsTrue(EndingResolver.IsGolden(Ending.JergalsKeyhole));
        }

        [Test]
        public void BreakTheLoop_NeedsEitherUnderstandingOrTrueName()
        {
            GameFlags.Current.SetBool("pc.true_name", true);
            CollectionAssert.Contains(EndingResolver.Available(), Ending.BreakTheLoop);
            Assert.IsTrue(EndingResolver.IsGolden(Ending.BreakTheLoop));
        }

        [Test]
        public void IsGolden_OnlyForTheTwoDeepestRoads()
        {
            Assert.IsTrue(EndingResolver.IsGolden(Ending.JergalsKeyhole));
            Assert.IsTrue(EndingResolver.IsGolden(Ending.BreakTheLoop));
            Assert.IsFalse(EndingResolver.IsGolden(Ending.Abolition));
            Assert.IsFalse(EndingResolver.IsGolden(Ending.DoomguidesPeace));
            Assert.IsFalse(EndingResolver.IsGolden(Ending.ReturnedThrone));
            Assert.IsFalse(EndingResolver.IsGolden(Ending.MortalMeasure));
        }

        [Test]
        public void EveryEnding_CarriesTitleChoiceAndProse()
        {
            foreach (Ending e in System.Enum.GetValues(typeof(Ending)))
            {
                Assert.AreNotEqual("—", EndingResolver.Title(e), $"{e} has no title.");
                Assert.AreNotEqual("—", EndingResolver.Choice(e), $"{e} has no choice text.");
                Assert.AreNotEqual("—", EndingResolver.Prose(e), $"{e} has no prose.");
                Assert.IsNotEmpty(EndingResolver.Prose(e));
            }
        }
    }
}
