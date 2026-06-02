using System;
using System.Text.RegularExpressions;

namespace SunderedCrown.Stats
{
    /// <summary>
    /// Central dice utility. All randomness in combat, skill checks, and loot
    /// funnels through here so it can be seeded for deterministic tests / save replays.
    /// </summary>
    public static class Dice
    {
        // Swap this Random out for a seeded instance in tests.
        private static Random _rng = new Random();

        public static void Seed(int seed) => _rng = new Random(seed);

        /// <summary>Roll a single die with the given number of sides (1..sides).</summary>
        public static int Roll(int sides) => _rng.Next(1, sides + 1);

        /// <summary>Roll <paramref name="count"/> dice of <paramref name="sides"/> sides.</summary>
        public static int Roll(int count, int sides)
        {
            int total = 0;
            for (int i = 0; i < count; i++) total += Roll(sides);
            return total;
        }

        public static int D20() => Roll(20);

        /// <summary>Roll a d20 with advantage (take the higher of two).</summary>
        public static int D20Advantage() => Math.Max(Roll(20), Roll(20));

        /// <summary>Roll a d20 with disadvantage (take the lower of two).</summary>
        public static int D20Disadvantage() => Math.Min(Roll(20), Roll(20));

        private static readonly Regex NotationRegex =
            new Regex(@"^\s*(\d+)d(\d+)\s*([+-]\s*\d+)?\s*$", RegexOptions.Compiled);

        /// <summary>
        /// Parse and roll standard dice notation such as "2d6+3", "1d8", "4d4-1".
        /// Returns the summed result.
        /// </summary>
        public static int Roll(string notation)
        {
            var m = NotationRegex.Match(notation);
            if (!m.Success)
                throw new FormatException($"Invalid dice notation: '{notation}'. Expected e.g. '2d6+3'.");

            int count = int.Parse(m.Groups[1].Value);
            int sides = int.Parse(m.Groups[2].Value);
            int modifier = 0;
            if (m.Groups[3].Success)
                modifier = int.Parse(m.Groups[3].Value.Replace(" ", ""));

            return Roll(count, sides) + modifier;
        }
    }
}
