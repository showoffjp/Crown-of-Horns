using NUnit.Framework;
using SunderedCrown.Combat;

namespace SunderedCrown.Tests.EditMode
{
    /// <summary>
    /// Exercises the full combat loop end-to-end through the real <see cref="CombatBalance"/>
    /// canary (which drives <see cref="AttackResolver"/> over hundreds of seeded duels). Two
    /// guarantees: the pipeline runs headless without throwing, and it's deterministic — the
    /// balance report is reproducible run to run, so a regression shows as a changed report,
    /// not as flakiness. DESIGN pillar IV.
    /// </summary>
    public class CombatBalanceTests
    {
        [Test]
        public void Report_RunsAndNamesBothMatchups()
        {
            string report = CombatBalance.Report(trials: 100);
            Assert.IsNotEmpty(report);
            StringAssert.Contains("Hero vs Brute", report);
            StringAssert.Contains("Duelist vs Brute", report);
        }

        [Test]
        public void Report_IsDeterministic()
        {
            // Report reseeds the dice per trial, so two runs must produce identical output.
            Assert.AreEqual(CombatBalance.Report(trials: 200), CombatBalance.Report(trials: 200));
        }

        [Test]
        public void FavouredMatchup_WinsMoreThanHalf_ButNotAlways()
        {
            // The armored Hero vs the weaker Brute should be favoured but not a sure thing —
            // a fast guard against the resolver math drifting into a degenerate auto-win/loss.
            string report = CombatBalance.Report(trials: 400);
            int heroPct = ParsePercentAfter(report, "Hero vs Brute ");
            Assert.Greater(heroPct, 50, $"Hero should be favoured; got {heroPct}%.\n{report}");
            Assert.Less(heroPct, 100, $"Hero should not auto-win; got {heroPct}%.\n{report}");
        }

        // Pulls the integer immediately following a label, e.g. "...Hero vs Brute 72% [OK]..." → 72.
        private static int ParsePercentAfter(string s, string label)
        {
            int i = s.IndexOf(label, System.StringComparison.Ordinal);
            Assert.GreaterOrEqual(i, 0, $"Report missing label '{label}': {s}");
            int j = i + label.Length;
            int start = j;
            while (j < s.Length && char.IsDigit(s[j])) j++;
            Assert.Greater(j, start, $"No number after '{label}' in: {s}");
            return int.Parse(s.Substring(start, j - start));
        }
    }
}
