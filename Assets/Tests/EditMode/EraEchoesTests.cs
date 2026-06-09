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

        [Test]
        public void TimeOfTroubles_NamesTheBreach_PulledVsLeft()
        {
            StringAssert.DoesNotContain("trade-death", CloseText(EraEchoes.TimeOfTroubles()),
                "no Breach thread before the Wall choice");

            GameFlags.Current.SetBool("fugue.pull_maerin", true);
            StringAssert.Contains("balanced the books", CloseText(EraEchoes.TimeOfTroubles()),
                "pulling a soul from the Wall should be named back as a trade-death");

            FreshFlags();
            GameFlags.Current.SetBool("fugue.left_maerin", true);
            StringAssert.Contains("did the arithmetic", CloseText(EraEchoes.TimeOfTroubles()),
                "declining the Wall should be named back as counted restraint");
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

        [Test]
        public void Spellplague_NamesTheBreach_PulledVsLeft()
        {
            StringAssert.DoesNotContain("one soul in", CloseText(EraEchoes.Spellplague()),
                "no Breach thread before the Wall choice");

            GameFlags.Current.SetBool("fugue.pull_maerin", true);
            StringAssert.Contains("The pulled and the paid", CloseText(EraEchoes.Spellplague()),
                "pulling a soul from the Wall should echo in the blue fire as one-out-one-in");

            FreshFlags();
            GameFlags.Current.SetBool("fugue.left_maerin", true);
            StringAssert.Contains("thank a soul for restraint", CloseText(EraEchoes.Spellplague()),
                "declining the Wall should be thanked by the soul that would have been the cost");
        }

        // ---------- The Crown Bearer (Aldric, named at the forge) ----------

        [Test]
        public void CrownBearer_MarksSeen_AndForeshadowsWhenAldricUnmet()
        {
            var g = EraEchoes.CrownBearer();
            Assert.IsTrue(SetsFlag(g, "0", "toot.crown_echo_seen"), "echo should set toot.crown_echo_seen on enter");
            StringAssert.Contains("a gentle hand will be offered", CloseText(g),
                "with Aldric unmet, the keeper foreshadows the bearer instead of naming the meeting");
        }

        [Test]
        public void CrownBearer_NamesTheCrownDoubt_WhenPlanted()
        {
            GameFlags.Current.SetBool("aldric.crown_doubt_planted", true);
            StringAssert.Contains("do not unsee it", CloseText(EraEchoes.CrownBearer()),
                "sensing the Crown using Aldric should be paid off at the forge as the confirmed leash");
        }

        [Test]
        public void CrownBearer_NamesTheMonsterVerdict()
        {
            GameFlags.Current.SetBool("aldric.named_monster", true);
            StringAssert.Contains("Hate the relic", CloseText(EraEchoes.CrownBearer()),
                "having damned Aldric, the keeper should redirect that certainty at the relic");
        }

        [Test]
        public void CrownBearer_NamesTheGriefSeen()
        {
            GameFlags.Current.SetBool("aldric.grief_seen", true);
            StringAssert.Contains("the *door* the Crown was built to walk through", CloseText(EraEchoes.CrownBearer()),
                "having seen the father, the keeper names the grief as the Crown's chosen door");
        }

        [Test]
        public void CrownBearer_NamesTheCostRevealed()
        {
            GameFlags.Current.SetBool("aldric.cost_revealed", true);
            StringAssert.Contains("a *different* ledger", CloseText(EraEchoes.CrownBearer()),
                "having made Aldric say the count, the keeper names the Crown's separate ledger");
        }

        [Test]
        public void CrownBearer_FallsBackToMet_AndPrioritisesDoubt()
        {
            GameFlags.Current.SetBool("aldric.met", true);
            StringAssert.Contains("both ends of it", CloseText(EraEchoes.CrownBearer()),
                "a bare meeting should still be named (the same throat, both ends)");

            // The crown-doubt reading outranks a bare meeting when both are set.
            GameFlags.Current.SetBool("aldric.crown_doubt_planted", true);
            StringAssert.Contains("do not unsee it", CloseText(EraEchoes.CrownBearer()),
                "crown_doubt_planted should take priority over the generic met fallback");
        }
    }
}
