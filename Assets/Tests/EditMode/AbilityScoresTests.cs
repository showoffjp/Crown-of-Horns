using NUnit.Framework;
using SunderedCrown.Stats;

namespace SunderedCrown.Tests.EditMode
{
    /// <summary>
    /// The 5e ability-modifier curve underpins every roll in the game, so it gets pinned
    /// exactly: floor((score − 10) / 2), including the negative side where C# integer
    /// division would round toward zero instead of flooring.
    /// </summary>
    public class AbilityScoresTests
    {
        [Test]
        public void Default_AllTens_GiveZeroModifiers()
        {
            var a = new AbilityScores();
            foreach (Ability ab in System.Enum.GetValues(typeof(Ability)))
            {
                Assert.AreEqual(10, a.Get(ab));
                Assert.AreEqual(0, a.Modifier(ab));
            }
        }

        [TestCase(1, -5)]
        [TestCase(7, -2)]
        [TestCase(8, -1)]
        [TestCase(9, -1)]
        [TestCase(10, 0)]
        [TestCase(11, 0)]
        [TestCase(12, 1)]
        [TestCase(14, 2)]
        [TestCase(15, 2)]
        [TestCase(20, 5)]
        [TestCase(30, 10)]
        public void Modifier_FollowsFifthEditionCurve(int score, int expectedMod)
        {
            var a = new AbilityScores();
            a.Set(Ability.Strength, score);
            Assert.AreEqual(expectedMod, a.Modifier(Ability.Strength),
                $"score {score} should give modifier {expectedMod}.");
        }

        [Test]
        public void SetAndGet_AreIndependentPerAbility()
        {
            var a = new AbilityScores();
            a.Set(Ability.Strength, 16);
            a.Set(Ability.Dexterity, 8);
            a.Set(Ability.Constitution, 14);
            a.Set(Ability.Intelligence, 12);
            a.Set(Ability.Wisdom, 13);
            a.Set(Ability.Charisma, 18);

            Assert.AreEqual(16, a.Get(Ability.Strength));
            Assert.AreEqual(8, a.Get(Ability.Dexterity));
            Assert.AreEqual(14, a.Get(Ability.Constitution));
            Assert.AreEqual(12, a.Get(Ability.Intelligence));
            Assert.AreEqual(13, a.Get(Ability.Wisdom));
            Assert.AreEqual(18, a.Get(Ability.Charisma));

            // …and their modifiers.
            Assert.AreEqual(3, a.Modifier(Ability.Strength));
            Assert.AreEqual(-1, a.Modifier(Ability.Dexterity));
            Assert.AreEqual(4, a.Modifier(Ability.Charisma));
        }
    }
}
