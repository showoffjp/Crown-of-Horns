using System.Linq;
using NUnit.Framework;
using SunderedCrown.Core;
using SunderedCrown.Content;
using SunderedCrown.Dialogue;

namespace SunderedCrown.Tests.EditMode
{
    /// <summary>
    /// Locks the CROSS-ERA CALLBACKS (ROADMAP Tier 3 / DESIGN pillar V: "no choice forgotten").
    /// The two late-era echoes are built live from <see cref="GameFlags"/>, so this asserts each one
    /// names the correct upstream choice — the Crown Wars Verdict (spared vs passed) and the fall of
    /// Netheril — and falls back gracefully when neither upstream era was witnessed. Pure logic, so the
    /// no-compiler sandbox can still guard the reactivity even though it can't run the scene.
    /// </summary>
    public class EraEchoesTests
    {
        [SetUp]
        public void FreshFlags() => GameFlags.Replace(new GameFlags());

        private static string CloseText(DialogueGraph g)
        {
            Assert.IsNotNull(g, "echo graph was null");
            var node = g.nodes.FirstOrDefault(n => n.id == "close");
            Assert.IsNotNull(node, "echo graph has no 'close' node");
            return node.text;
        }

        private static bool SetsFlag(DialogueGraph g, string startId, string key) =>
            g.nodes.First(n => n.id == startId).onEnter?.Any(c => c.key == key && c.op == FlagOp.SetTrue) == true;

        // ---------- Time of Troubles ----------

        [Test]
        public void TimeOfTroubles_MarksSeen_AndDefaultsGracefully()
        {
            var g = EraEchoes.TimeOfTroubles();
            Assert.IsTrue(SetsFlag(g, "0", "toot.echo_seen"), "echo should set toot.echo_seen on enter");
            StringAssert.Contains("not yet a wall", CloseText(g),
                "with no upstream Verdict, the gravedigger should give the neutral fallback line");
        }

        [Test]
        public void TimeOfTroubles_NamesTheSparedVerdict()
        {
            GameFlags.Current.SetBool("crownwars.verdict_spared", true);
            StringAssert.Contains("never a furnace", CloseText(EraEchoes.TimeOfTroubles()),
                "a spared Verdict should be named back as the reason the unclaimed get a resting-place");
        }

        [Test]
        public void TimeOfTroubles_NamesThePassedVerdict()
        {
            GameFlags.Current.SetBool("crownwars.verdict_passed", true);
            StringAssert.Contains("The source remembers", CloseText(EraEchoes.TimeOfTroubles()),
                "a passed Verdict should be named back as the law still being inherited");
        }

        [Test]
        public void TimeOfTroubles_AppendsNetherilThread_WhenWitnessed()
        {
            var without = CloseText(EraEchoes.TimeOfTroubles());
            StringAssert.DoesNotContain("the soot of it", without, "no Netheril thread without the flag");

            GameFlags.Current.SetBool("netheril.cleared", true);
            StringAssert.Contains("the soot of it", CloseText(EraEchoes.TimeOfTroubles()),
                "witnessing Netheril's fall should add the second callback thread");
        }

        // ---------- Spellplague ----------

        [Test]
        public void Spellplague_MarksSeen_AndDefaultsGracefully()
        {
            var g = EraEchoes.Spellplague();
            Assert.IsTrue(SetsFlag(g, "0", "spellplague.echo_seen"), "echo should set spellplague.echo_seen on enter");
            StringAssert.Contains("I don't know what I was", CloseText(g),
                "with no upstream Verdict, the half-unmade voice should give the neutral fallback line");
        }

        [Test]
        public void Spellplague_NamesTheSparedVerdict()
        {
            GameFlags.Current.SetBool("crownwars.verdict_spared", true);
            StringAssert.Contains("thank you", CloseText(EraEchoes.Spellplague()),
                "a spared Verdict should let the half-unmade soul thank the player downstream");
        }

        [Test]
        public void Spellplague_NamesThePassedVerdict()
        {
            GameFlags.Current.SetBool("crownwars.verdict_passed", true);
            StringAssert.Contains("the vote and the cost", CloseText(EraEchoes.Spellplague()),
                "a passed Verdict should be named back as the vote whose cost is now visible");
        }

        [Test]
        public void Spellplague_AppendsNetherilThread_WhenWitnessed()
        {
            var without = CloseText(EraEchoes.Spellplague());
            StringAssert.DoesNotContain("fell once before", without, "no Netheril thread without the flag");

            GameFlags.Current.SetBool("netheril.arrived", true);
            StringAssert.Contains("fell once before", CloseText(EraEchoes.Spellplague()),
                "witnessing Netheril should surface the returned-shades callback");
        }
    }
}
