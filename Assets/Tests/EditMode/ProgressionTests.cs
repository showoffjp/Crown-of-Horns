using NUnit.Framework;
using SunderedCrown.Characters;

namespace SunderedCrown.Tests.EditMode
{
    /// <summary>
    /// XP and leveling on the 5e table. Awarding XP is a pure, save-safe operation, so we
    /// pin the thresholds, single- and multi-level jumps, the level-20 cap, the no-op guards,
    /// and the OnLevelUp event. Keeps progression honest across saves and content.
    /// </summary>
    public class ProgressionTests
    {
        private CharacterSheet Fresh() =>
            new CharacterSheet { level = 1, experience = 0, classDef = null, maxHitPoints = 10, currentHitPoints = 10 };

        [Test]
        public void XpToReach_MatchesTable()
        {
            Assert.AreEqual(0, Progression.XpToReach(1));
            Assert.AreEqual(300, Progression.XpToReach(2));
            Assert.AreEqual(6500, Progression.XpToReach(5));
            Assert.AreEqual(355000, Progression.XpToReach(20));
        }

        [Test]
        public void XpToReach_OutOfRange_IsMaxValue()
        {
            Assert.AreEqual(int.MaxValue, Progression.XpToReach(0));
            Assert.AreEqual(int.MaxValue, Progression.XpToReach(21));
        }

        [Test]
        public void AwardExperience_CrossingThreshold_LevelsUpOnce()
        {
            var s = Fresh();
            int gained = Progression.AwardExperience(s, 300);
            Assert.AreEqual(1, gained);
            Assert.AreEqual(2, s.level);
            Assert.AreEqual(300, s.experience);
        }

        [Test]
        public void AwardExperience_LargeGrant_JumpsMultipleLevels()
        {
            var s = Fresh();
            int gained = Progression.AwardExperience(s, 6500); // total for level 5
            Assert.AreEqual(5, s.level);
            Assert.AreEqual(4, gained); // 1→5
        }

        [Test]
        public void AwardExperience_NonPositive_IsNoOp()
        {
            var s = Fresh();
            Assert.AreEqual(0, Progression.AwardExperience(s, 0));
            Assert.AreEqual(0, Progression.AwardExperience(s, -50));
            Assert.AreEqual(1, s.level);
            Assert.AreEqual(0, s.experience);
        }

        [Test]
        public void AwardExperience_CapsAtMaxLevel()
        {
            var s = Fresh();
            Progression.AwardExperience(s, 10_000_000);
            Assert.AreEqual(Progression.MaxLevel, s.level);
            Assert.AreEqual(0, Progression.AwardExperience(s, 10_000), "No XP gain past max level.");
        }

        [Test]
        public void XpToNextLevel_ReportsRemaining_AndZeroAtCap()
        {
            var s = Fresh();
            Assert.AreEqual(300, Progression.XpToNextLevel(s));
            Progression.AwardExperience(s, 10_000_000);
            Assert.AreEqual(0, Progression.XpToNextLevel(s));
        }

        [Test]
        public void OnLevelUp_FiresWithNewLevel()
        {
            var s = Fresh();
            int reported = -1;
            System.Action<CharacterSheet, int> handler = (sheet, lvl) => reported = lvl;
            Progression.OnLevelUp += handler;
            try { Progression.AwardExperience(s, 300); }
            finally { Progression.OnLevelUp -= handler; }
            Assert.AreEqual(2, reported);
        }

        [Test]
        public void LevelUp_NullClass_IncrementsLevel_WithoutHpChange()
        {
            var s = Fresh();
            int hpBefore = s.maxHitPoints;
            Progression.LevelUp(s);
            Assert.AreEqual(2, s.level);
            Assert.AreEqual(hpBefore, s.maxHitPoints, "No classDef ⇒ no hit-die HP gain.");
        }
    }
}
