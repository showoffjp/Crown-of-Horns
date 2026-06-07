using NUnit.Framework;
using SunderedCrown.Core;

namespace SunderedCrown.Tests.EditMode
{
    /// <summary>
    /// Difficulty is meant to scale combat at a single chokepoint, transparently (DESIGN
    /// pillar IV — no hidden rubber-banding). These pin the Story/Normal/Hard multipliers,
    /// the combat-speed clamp, and the descriptive blurbs. Snapshots/restores the global
    /// settings it touches.
    /// </summary>
    public class GameSettingsTests
    {
        private GameSettings.Difficulty _mode;
        private float _combatSpeed, _uiScale;

        [SetUp]
        public void SetUp()
        {
            _mode = GameSettings.Mode;
            _combatSpeed = GameSettings.CombatSpeed;
            _uiScale = GameSettings.UiScale;
        }

        [TearDown]
        public void TearDown()
        {
            GameSettings.Mode = _mode;
            GameSettings.CombatSpeed = _combatSpeed;
            GameSettings.UiScale = _uiScale;
        }

        [Test]
        public void Normal_LeavesCombatUnscaled()
        {
            GameSettings.Mode = GameSettings.Difficulty.Normal;
            Assert.AreEqual(1f, GameSettings.EnemyDamageMult, 1e-4);
            Assert.AreEqual(1f, GameSettings.PlayerDamageMult, 1e-4);
            Assert.AreEqual(1f, GameSettings.EnemyHpMult, 1e-4);
        }

        [Test]
        public void Story_SoftensFoes_AndBuffsPlayer()
        {
            GameSettings.Mode = GameSettings.Difficulty.Story;
            Assert.Less(GameSettings.EnemyDamageMult, 1f, "Foes hit softer on Story.");
            Assert.Greater(GameSettings.PlayerDamageMult, 1f, "Player hits harder on Story.");
            Assert.Less(GameSettings.EnemyHpMult, 1f, "Foes are frailer on Story.");
        }

        [Test]
        public void Hard_SharpensFoes_AndNerfsPlayer()
        {
            GameSettings.Mode = GameSettings.Difficulty.Hard;
            Assert.Greater(GameSettings.EnemyDamageMult, 1f, "Foes hit harder on Hard.");
            Assert.Less(GameSettings.PlayerDamageMult, 1f, "Player hits softer on Hard.");
            Assert.Greater(GameSettings.EnemyHpMult, 1f, "Foes are spongier on Hard.");
        }

        [Test]
        public void CombatDelay_ClampsSpeedToItsRange()
        {
            GameSettings.CombatSpeed = 1f;
            Assert.AreEqual(1f, GameSettings.CombatDelay(1f), 1e-4);

            GameSettings.CombatSpeed = 10f;  // clamps to 2.5
            Assert.AreEqual(1f / 2.5f, GameSettings.CombatDelay(1f), 1e-4);

            GameSettings.CombatSpeed = 0.1f; // clamps to 0.5
            Assert.AreEqual(1f / 0.5f, GameSettings.CombatDelay(1f), 1e-4);
        }

        [Test]
        public void DifficultyBlurb_DiffersPerMode()
        {
            GameSettings.Mode = GameSettings.Difficulty.Story;
            string story = GameSettings.DifficultyBlurb;
            GameSettings.Mode = GameSettings.Difficulty.Hard;
            string hard = GameSettings.DifficultyBlurb;
            GameSettings.Mode = GameSettings.Difficulty.Normal;
            string normal = GameSettings.DifficultyBlurb;

            Assert.AreNotEqual(story, hard);
            Assert.AreNotEqual(story, normal);
            Assert.AreNotEqual(hard, normal);
        }

        [Test]
        public void UiScaleBlurb_ReflectsThresholds()
        {
            GameSettings.UiScale = 0.85f;
            StringAssert.Contains("Compact", GameSettings.UiScaleBlurb);
            GameSettings.UiScale = 1.5f;
            StringAssert.Contains("Extra Large", GameSettings.UiScaleBlurb);
            GameSettings.UiScale = 1f;
            StringAssert.Contains("Standard", GameSettings.UiScaleBlurb);
        }
    }
}
