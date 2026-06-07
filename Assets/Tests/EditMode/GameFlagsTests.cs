using NUnit.Framework;
using SunderedCrown.Core;

namespace SunderedCrown.Tests.EditMode
{
    /// <summary>
    /// Guards DESIGN pillar III — "one brain, total reactivity." Every reactive system
    /// reads GameFlags, so its contract (defaults, accumulation, approval clamping,
    /// change events, null-safe Replace) must hold or the whole world misreports.
    /// </summary>
    public class GameFlagsTests
    {
        [Test]
        public void GetBool_DefaultsFalse()
        {
            var f = new GameFlags();
            Assert.IsFalse(f.GetBool("never.set"));
        }

        [Test]
        public void SetBool_RoundTrips()
        {
            var f = new GameFlags();
            f.SetBool("act1.smith.helped", true);
            Assert.IsTrue(f.GetBool("act1.smith.helped"));
        }

        [Test]
        public void GetInt_DefaultsZero()
        {
            var f = new GameFlags();
            Assert.AreEqual(0, f.GetInt("never.set"));
        }

        [Test]
        public void AddInt_Accumulates()
        {
            var f = new GameFlags();
            f.AddInt("party.gold", 10);
            f.AddInt("party.gold", 5);
            Assert.AreEqual(15, f.GetInt("party.gold"));
        }

        [Test]
        public void AdjustApproval_ClampsToReadableBand()
        {
            var f = new GameFlags();
            f.AdjustApproval("sable", 250);
            Assert.AreEqual(100, f.GetInt("companion.sable.approval"), "Approval caps at +100.");

            f.AdjustApproval("sable", -500);
            Assert.AreEqual(-100, f.GetInt("companion.sable.approval"), "Approval floors at -100.");
        }

        [Test]
        public void OnFlagChanged_FiresWithKey_ForBoolAndInt()
        {
            var f = new GameFlags();
            string lastKey = null;
            f.OnFlagChanged += k => lastKey = k;

            f.SetBool("world.warden_dead", true);
            Assert.AreEqual("world.warden_dead", lastKey);

            f.SetInt("faction.ashpact.reputation", 3);
            Assert.AreEqual("faction.ashpact.reputation", lastKey);
        }

        [Test]
        public void Replace_SwapsCurrent()
        {
            var fresh = new GameFlags();
            fresh.SetBool("scaffold.test", true);
            GameFlags.Replace(fresh);
            Assert.IsTrue(GameFlags.Current.GetBool("scaffold.test"));
        }

        [Test]
        public void Replace_Null_YieldsEmptyButNonNullCurrent()
        {
            GameFlags.Replace(null);
            Assert.IsNotNull(GameFlags.Current, "Replace(null) must never leave Current null.");
            Assert.IsFalse(GameFlags.Current.GetBool("scaffold.test"));
        }
    }
}
