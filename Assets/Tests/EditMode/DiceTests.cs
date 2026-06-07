using System;
using NUnit.Framework;
using SunderedCrown.Stats;

namespace SunderedCrown.Tests.EditMode
{
    /// <summary>
    /// Guards DESIGN pillar IV — "5e you can trust." If the dice lie, the tactical
    /// layer can't carry the narrative weight. These pin the bounds, the determinism
    /// (so saves/replays reproduce), and the notation parser.
    /// </summary>
    public class DiceTests
    {
        [Test]
        public void Seed_MakesRollsDeterministic()
        {
            Dice.Seed(12345);
            int a = Dice.Roll(20);
            int b = Dice.Roll(20);

            Dice.Seed(12345);
            Assert.AreEqual(a, Dice.Roll(20), "Same seed must reproduce the first roll.");
            Assert.AreEqual(b, Dice.Roll(20), "Same seed must reproduce the second roll.");
        }

        [Test]
        public void Roll_SingleDie_StaysWithinBounds()
        {
            Dice.Seed(1);
            for (int i = 0; i < 1000; i++)
            {
                int r = Dice.Roll(6);
                Assert.GreaterOrEqual(r, 1);
                Assert.LessOrEqual(r, 6);
            }
        }

        [Test]
        public void Roll_MultipleDice_StaysWithinSummedBounds()
        {
            Dice.Seed(2);
            for (int i = 0; i < 1000; i++)
            {
                int r = Dice.Roll(3, 6); // 3..18
                Assert.GreaterOrEqual(r, 3);
                Assert.LessOrEqual(r, 18);
            }
        }

        [Test]
        public void D20_StaysWithinBounds()
        {
            Dice.Seed(3);
            for (int i = 0; i < 1000; i++)
            {
                int r = Dice.D20();
                Assert.GreaterOrEqual(r, 1);
                Assert.LessOrEqual(r, 20);
            }
        }

        [Test]
        public void Advantage_TrendsHigherThanDisadvantage()
        {
            // Statistical sanity: over many trials, advantage must out-total disadvantage.
            Dice.Seed(4);
            long adv = 0, dis = 0;
            for (int i = 0; i < 5000; i++) adv += Dice.D20Advantage();
            for (int i = 0; i < 5000; i++) dis += Dice.D20Disadvantage();
            Assert.Greater(adv, dis, "Advantage should sum higher than disadvantage across many rolls.");
        }

        [Test]
        public void AdvantageAndDisadvantage_StayWithinD20Bounds()
        {
            Dice.Seed(7);
            for (int i = 0; i < 1000; i++)
            {
                int a = Dice.D20Advantage();
                int d = Dice.D20Disadvantage();
                Assert.GreaterOrEqual(a, 1); Assert.LessOrEqual(a, 20);
                Assert.GreaterOrEqual(d, 1); Assert.LessOrEqual(d, 20);
            }
        }

        [Test]
        public void Roll_Notation_PositiveModifier_WithinBounds()
        {
            Dice.Seed(5);
            for (int i = 0; i < 1000; i++)
            {
                int r = Dice.Roll("2d6+3"); // 5..15
                Assert.GreaterOrEqual(r, 5);
                Assert.LessOrEqual(r, 15);
            }
        }

        [Test]
        public void Roll_Notation_NegativeModifier_WithinBounds()
        {
            Dice.Seed(6);
            for (int i = 0; i < 1000; i++)
            {
                int r = Dice.Roll("4d4-1"); // 3..15
                Assert.GreaterOrEqual(r, 3);
                Assert.LessOrEqual(r, 15);
            }
        }

        [Test]
        public void Roll_Notation_NoModifier_WithinBounds()
        {
            Dice.Seed(8);
            for (int i = 0; i < 1000; i++)
            {
                int r = Dice.Roll("1d8"); // 1..8
                Assert.GreaterOrEqual(r, 1);
                Assert.LessOrEqual(r, 8);
            }
        }

        [Test]
        public void Roll_InvalidNotation_Throws()
        {
            Assert.Throws<FormatException>(() => Dice.Roll("notdice"));
            Assert.Throws<FormatException>(() => Dice.Roll("d20"));   // missing leading count
            Assert.Throws<FormatException>(() => Dice.Roll("2x6"));   // wrong separator
        }
    }
}
