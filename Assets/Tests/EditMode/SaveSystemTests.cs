using NUnit.Framework;
using SunderedCrown.Core;
using SunderedCrown.Save;

namespace SunderedCrown.Tests.EditMode
{
    /// <summary>
    /// Persistence is what makes DESIGN pillar II ("the game remembers") real. This is a
    /// genuine round-trip: write GameFlags — the story brain — to a JSON save on disk, wipe
    /// state, load it back, and assert every flag returns. Uses a throwaway slot under the
    /// real persistentDataPath and cleans up after itself.
    /// </summary>
    public class SaveSystemTests
    {
        private const string Slot = "unit_test_slot";
        private GameFlags _original;

        [SetUp]
        public void SetUp()
        {
            _original = GameFlags.Current;
            SaveSystem.Delete(Slot); // start clean
        }

        [TearDown]
        public void TearDown()
        {
            SaveSystem.Delete(Slot);
            GameFlags.Replace(_original ?? new GameFlags());
        }

        [Test]
        public void SaveThenLoad_RestoresEveryFlag()
        {
            var flags = new GameFlags();
            flags.SetBool("act1.smith.helped", true);
            flags.SetInt("party.gold", 123);
            flags.SetInt("companion.sable.approval", -7);
            GameFlags.Replace(flags);

            SaveSystem.Save(Slot, "TestScene");

            // Wipe to prove the values come back from disk, not memory.
            GameFlags.Replace(new GameFlags());
            Assert.IsFalse(GameFlags.Current.GetBool("act1.smith.helped"));

            Assert.IsTrue(SaveSystem.Load(Slot));
            Assert.IsTrue(GameFlags.Current.GetBool("act1.smith.helped"));
            Assert.AreEqual(123, GameFlags.Current.GetInt("party.gold"));
            Assert.AreEqual(-7, GameFlags.Current.GetInt("companion.sable.approval"));
            Assert.AreEqual("TestScene", SaveSystem.Last.sceneName);
        }

        [Test]
        public void Exists_TracksSaveAndDelete()
        {
            GameFlags.Replace(new GameFlags());
            Assert.IsFalse(SaveSystem.Exists(Slot));
            SaveSystem.Save(Slot, "S");
            Assert.IsTrue(SaveSystem.Exists(Slot));
            SaveSystem.Delete(Slot);
            Assert.IsFalse(SaveSystem.Exists(Slot));
        }

        [Test]
        public void Load_MissingSlot_ReturnsFalse()
        {
            Assert.IsFalse(SaveSystem.Load("definitely_missing_slot_xyz"));
        }

        [Test]
        public void Peek_ReportsSceneMetadata_WithoutTouchingGlobalState()
        {
            GameFlags.Replace(new GameFlags());
            SaveSystem.Save(Slot, "PeekScene");

            var meta = SaveSystem.Peek(Slot);
            Assert.IsTrue(meta.exists);
            Assert.AreEqual("PeekScene", meta.sceneName);

            var missing = SaveSystem.Peek("definitely_missing_slot_xyz");
            Assert.IsFalse(missing.exists);
        }

        [Test]
        public void Resave_OverwritesWithLatestState()
        {
            var flags = new GameFlags();
            flags.SetInt("party.gold", 100);
            flags.SetBool("door.open", true);
            GameFlags.Replace(flags);
            SaveSystem.Save(Slot, "S1");

            // Change state and save the SAME slot again.
            GameFlags.Current.SetInt("party.gold", 250);
            GameFlags.Current.SetBool("door.open", false);
            SaveSystem.Save(Slot, "S2");

            GameFlags.Replace(new GameFlags());
            Assert.IsTrue(SaveSystem.Load(Slot));
            Assert.AreEqual(250, GameFlags.Current.GetInt("party.gold"), "re-save must overwrite, not stack");
            Assert.IsFalse(GameFlags.Current.GetBool("door.open"));
            Assert.AreEqual("S2", SaveSystem.Last.sceneName);
        }

        [Test]
        public void ManyFlags_AllRoundTrip()
        {
            var flags = new GameFlags();
            for (int i = 0; i < 200; i++) { flags.SetBool("b." + i, i % 2 == 0); flags.SetInt("i." + i, i * 7 - 100); }
            GameFlags.Replace(flags);
            SaveSystem.Save(Slot, "Many");

            GameFlags.Replace(new GameFlags());
            Assert.IsTrue(SaveSystem.Load(Slot));
            for (int i = 0; i < 200; i++)
            {
                Assert.AreEqual(i % 2 == 0, GameFlags.Current.GetBool("b." + i), $"bool b.{i}");
                Assert.AreEqual(i * 7 - 100, GameFlags.Current.GetInt("i." + i), $"int i.{i}");
            }
        }

        [Test]
        public void IntEdgeValues_RoundTrip()
        {
            var flags = new GameFlags();
            flags.SetInt("zero", 0);
            flags.SetInt("max", int.MaxValue);
            flags.SetInt("min", int.MinValue);
            flags.SetInt("neg", -7);
            GameFlags.Replace(flags);
            SaveSystem.Save(Slot, "Edge");

            GameFlags.Replace(new GameFlags());
            Assert.IsTrue(SaveSystem.Load(Slot));
            Assert.AreEqual(0, GameFlags.Current.GetInt("zero"));
            Assert.AreEqual(int.MaxValue, GameFlags.Current.GetInt("max"));
            Assert.AreEqual(int.MinValue, GameFlags.Current.GetInt("min"));
            Assert.AreEqual(-7, GameFlags.Current.GetInt("neg"));
        }
    }
}
