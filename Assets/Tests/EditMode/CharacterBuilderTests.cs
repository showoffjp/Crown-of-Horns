using NUnit.Framework;
using SunderedCrown.Characters;
using SunderedCrown.Stats;

namespace SunderedCrown.Tests.EditMode
{
    /// <summary>
    /// 5e character creation: point-buy accounting, the standard array, and assembling a
    /// playable level-1 sheet. Pure logic driven with writeFlags:false so it touches no
    /// global state. Guards that creation can't mint illegal builds.
    /// </summary>
    public class CharacterBuilderTests
    {
        private static AbilityScores Scores(int str, int dex, int con, int intel, int wis, int cha)
        {
            var a = new AbilityScores();
            a.Set(Ability.Strength, str);
            a.Set(Ability.Dexterity, dex);
            a.Set(Ability.Constitution, con);
            a.Set(Ability.Intelligence, intel);
            a.Set(Ability.Wisdom, wis);
            a.Set(Ability.Charisma, cha);
            return a;
        }

        [Test]
        public void PointBuyConstants_MatchFifthEdition()
        {
            Assert.AreEqual(27, CharacterBuilder.PointBuyBudget);
            Assert.AreEqual(8, CharacterBuilder.PointBuyMin);
            Assert.AreEqual(15, CharacterBuilder.PointBuyMax);
            CollectionAssert.AreEqual(new[] { 15, 14, 13, 12, 10, 8 }, CharacterBuilder.StandardArray);
        }

        [Test]
        public void StandardArray_CostsExactlyTheBudget()
        {
            var a = Scores(15, 14, 13, 12, 10, 8);
            Assert.AreEqual(27, CharacterBuilder.PointsSpent(a));
            Assert.IsTrue(CharacterBuilder.IsLegalPointBuy(a));
        }

        [Test]
        public void AllEights_CostNothing_AndAreLegal()
        {
            var a = Scores(8, 8, 8, 8, 8, 8);
            Assert.AreEqual(0, CharacterBuilder.PointsSpent(a));
            Assert.IsTrue(CharacterBuilder.IsLegalPointBuy(a));
        }

        [Test]
        public void AllFifteens_ExceedBudget_AndAreIllegal()
        {
            var a = Scores(15, 15, 15, 15, 15, 15);
            Assert.AreEqual(54, CharacterBuilder.PointsSpent(a)); // 6 × 9
            Assert.IsFalse(CharacterBuilder.IsLegalPointBuy(a));
        }

        [Test]
        public void OutOfRangeScore_IsRejected()
        {
            Assert.AreEqual(-1, CharacterBuilder.PointsSpent(Scores(16, 8, 8, 8, 8, 8)), "16 is above point-buy max.");
            Assert.AreEqual(-1, CharacterBuilder.PointsSpent(Scores(7, 8, 8, 8, 8, 8)), "7 is below point-buy min.");
            Assert.IsFalse(CharacterBuilder.IsLegalPointBuy(Scores(16, 8, 8, 8, 8, 8)));
        }

        [Test]
        public void Build_AppliesNameFallback_LevelOne_AndUnarmoredBase()
        {
            var sheet = CharacterBuilder.Build("", null, null, null, Scores(10, 10, 10, 10, 10, 10), writeFlags: false);
            Assert.AreEqual("Adventurer", sheet.displayName);
            Assert.AreEqual(1, sheet.level);
            Assert.AreEqual(10, sheet.baseArmorClass);
            Assert.GreaterOrEqual(sheet.maxHitPoints, 1);
        }

        [Test]
        public void Build_CopiesBaseScores_WhenNoRaceBonus()
        {
            var sheet = CharacterBuilder.Build("Hero", null, null, null, Scores(13, 14, 15, 12, 10, 8), writeFlags: false);
            Assert.AreEqual(13, sheet.abilities.Get(Ability.Strength));
            Assert.AreEqual(14, sheet.abilities.Get(Ability.Dexterity));
            Assert.AreEqual(15, sheet.abilities.Get(Ability.Constitution));
            Assert.AreEqual(12, sheet.abilities.Get(Ability.Intelligence));
            Assert.AreEqual(10, sheet.abilities.Get(Ability.Wisdom));
            Assert.AreEqual(8, sheet.abilities.Get(Ability.Charisma));
        }
    }
}
