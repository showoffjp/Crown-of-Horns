using NUnit.Framework;
using SunderedCrown.Combat;

namespace SunderedCrown.Tests.EditMode
{
    /// <summary>
    /// Pins the 5e/BG3 death-save rule (DESIGN pillar I). The C# port of the browser slice's
    /// deathSaveStep, which is itself pinned by play/downed.test.js — these assert the two stay
    /// in lockstep. Pure math, no scene, runs headless in CI.
    /// </summary>
    public class DeathSavesTests
    {
        // ---- per-roll transitions -----------------------------------------

        [Test]
        public void TenOrMore_IsASuccess()
        {
            Assert.AreEqual(1, DeathSaves.Step(0, 0, 10).successes);
            Assert.AreEqual(1, DeathSaves.Step(0, 0, 17).successes);
        }

        [Test]
        public void TwoToNine_IsAFailure()
        {
            Assert.AreEqual(1, DeathSaves.Step(0, 0, 9).failures);
            Assert.AreEqual(1, DeathSaves.Step(0, 0, 2).failures);
        }

        [Test]
        public void NaturalOne_IsTwoFailures()
        {
            Assert.AreEqual(2, DeathSaves.Step(0, 0, 1).failures);
        }

        [Test]
        public void NaturalTwenty_Revives_AndClearsTheTally()
        {
            var r = DeathSaves.Step(1, 2, 20);
            Assert.IsTrue(r.revived);
            Assert.AreEqual(0, r.successes);
            Assert.AreEqual(0, r.failures);
        }

        [Test]
        public void AMidSuccess_FlagsNothingTerminal()
        {
            var r = DeathSaves.Step(0, 0, 12);
            Assert.IsFalse(r.dead);
            Assert.IsFalse(r.stable);
            Assert.IsFalse(r.revived);
        }

        // ---- the thresholds -----------------------------------------------

        [Test]
        public void ThirdSuccess_Stabilises()
        {
            Assert.IsTrue(DeathSaves.Step(2, 0, 15).stable);
            Assert.IsFalse(DeathSaves.Step(2, 0, 15).dead);
        }

        [Test]
        public void ThirdFailure_IsDeath()
        {
            Assert.IsTrue(DeathSaves.Step(0, 2, 4).dead);
        }

        [Test]
        public void NatOneFromOneFailure_ReachesThree_AndDies()
        {
            Assert.IsTrue(DeathSaves.Step(0, 1, 1).dead);
        }

        [Test]
        public void TheTally_IsClampedAtTheThreshold()
        {
            Assert.AreEqual(3, DeathSaves.Step(0, 2, 5).failures);
            Assert.AreEqual(3, DeathSaves.Step(2, 0, 15).successes);
        }
    }
}
