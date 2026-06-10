using System.Collections.Generic;
using NUnit.Framework;
using SunderedCrown.Core;

namespace SunderedCrown.Tests.EditMode
{
    /// <summary>
    /// The epilogue assembly — the BG2-style payoff in <see cref="EndingResolver.Epilogue"/> and the
    /// running <see cref="EndingResolver.Chronicle"/> — was the largest untested surface in Core.
    /// These pin the gating: who gets a slide (recruited/lost/left), quest-outcome priorities,
    /// loved-and-lost grief, ending-tinted variants, verdict exclusivity, the anchor's
    /// romance-depth-then-approval rule, and the no-empty-slides invariant. Mutates the global
    /// GameFlags, so it snapshots and restores around each test (same pattern as EndingResolverTests).
    /// </summary>
    public class EpilogueTests
    {
        private GameFlags _original;

        [SetUp]
        public void SetUp()
        {
            _original = GameFlags.Current;
            GameFlags.Replace(new GameFlags());
        }

        [TearDown]
        public void TearDown() => GameFlags.Replace(_original ?? new GameFlags());

        private static GameFlags F => GameFlags.Current;

        private static bool Any(List<string> slides, string marker) =>
            slides.Exists(s => s != null && s.Contains(marker));

        // ---- companion slides --------------------------------------------

        [Test]
        public void Baseline_HasGarrow_NoUnrecruited_NoEmptySlides()
        {
            var slides = EndingResolver.Epilogue(Ending.MortalMeasure);
            Assert.IsTrue(Any(slides, "Sister Garrow"), "Garrow is in every run from the start.");
            Assert.IsFalse(Any(slides, "Roen"), "Unrecruited companions get no slide.");
            Assert.IsFalse(Any(slides, "Naeve"), "Unrecruited companions get no slide.");
            Assert.IsTrue(slides.TrueForAll(s => !string.IsNullOrEmpty(s)), "No null/empty slides survive.");
        }

        [Test]
        public void LostCompanion_GetsTheWallSlide()
        {
            F.SetBool("companion.roen.recruited", true);
            F.SetBool("companion.roen.lost", true);
            Assert.IsTrue(Any(EndingResolver.Epilogue(Ending.MortalMeasure), "taken by the Wall as the tithe"));
        }

        [Test]
        public void LeftCompanion_GetsWalkedOutSlide()
        {
            F.SetBool("companion.varra.recruited", true);
            F.SetBool("companion.varra.left", true);
            Assert.IsTrue(Any(EndingResolver.Epilogue(Ending.MortalMeasure), "walked out of the company"));
        }

        [Test]
        public void PresentSlide_IsTintedByEnding()
        {
            F.SetBool("companion.roen.recruited", true);
            Assert.IsTrue(Any(EndingResolver.Epilogue(Ending.BreakTheLoop), "stood with you at the niche"));
            Assert.IsTrue(Any(EndingResolver.Epilogue(Ending.JergalsKeyhole), "kept you company at the Ledger"));
            Assert.IsTrue(Any(EndingResolver.Epilogue(Ending.Abolition), "Deathless Garden"));
            Assert.IsTrue(Any(EndingResolver.Epilogue(Ending.MortalMeasure), "sat at the small table"));
            Assert.IsTrue(Any(EndingResolver.Epilogue(Ending.ReturnedThrone), "walked out of the story changed"));
        }

        // ---- personal-quest slides ----------------------------------------

        [Test]
        public void RoenQuest_DoubleAgent_OutranksOtherOutcomes()
        {
            F.SetBool("companion.roen.recruited", true);
            F.SetBool("quest.roen.resolved", true);
            F.SetBool("quest.roen.double_agent", true);
            F.SetBool("quest.roen.wrenna_saved", true);
            F.SetBool("quest.roen.harper_boon", true);
            var slides = EndingResolver.Epilogue(Ending.MortalMeasure);
            Assert.IsTrue(Any(slides, "a blade in Kelemvor's own house"), "double_agent wins the if/else chain.");
            Assert.IsFalse(Any(slides, "two cold teacups"), "Lower-priority outcomes must not also fire.");
        }

        [Test]
        public void RoenQuest_HarperBoon_AloneFires()
        {
            F.SetBool("companion.roen.recruited", true);
            F.SetBool("quest.roen.resolved", true);
            F.SetBool("quest.roen.harper_boon", true);
            Assert.IsTrue(Any(EndingResolver.Epilogue(Ending.MortalMeasure), "Correct is not the same as forgiven"));
        }

        [Test]
        public void QuestSlides_RequireResolved_AndSurvival()
        {
            F.SetBool("companion.varra.recruited", true);
            F.SetBool("quest.varra.patron_bound", true); // outcome set but quest NOT resolved
            Assert.IsFalse(Any(EndingResolver.Epilogue(Ending.MortalMeasure), "a Hell in her debt"));

            F.SetBool("quest.varra.resolved", true);
            Assert.IsTrue(Any(EndingResolver.Epilogue(Ending.MortalMeasure), "a Hell in her debt"));

            F.SetBool("companion.varra.lost", true); // dead companions get no quest epilogue
            Assert.IsFalse(Any(EndingResolver.Epilogue(Ending.MortalMeasure), "a Hell in her debt"));
        }

        [Test]
        public void GarrowQuest_AllThreeBranches()
        {
            F.SetBool("quest.garrow.resolved", true); // garrow needs no 'recruited' flag

            F.SetBool("quest.garrow.doctrine_won", true);
            Assert.IsTrue(Any(EndingResolver.Epilogue(Ending.MortalMeasure), "first High Doomguide"));

            GameFlags.Replace(new GameFlags());
            F.SetBool("quest.garrow.resolved", true);
            F.SetBool("quest.garrow.left_faith", true);
            Assert.IsTrue(Any(EndingResolver.Epilogue(Ending.MortalMeasure), "under no law but her own two hands"));

            GameFlags.Replace(new GameFlags());
            F.SetBool("quest.garrow.resolved", true);
            F.SetBool("quest.garrow.recanted", true);
            Assert.IsTrue(Any(EndingResolver.Epilogue(Ending.MortalMeasure), "smaller faith now"));
        }

        // ---- loved and lost -------------------------------------------------

        [Test]
        public void LovedAndLost_AddsGriefSlide_OnlyWithLove()
        {
            F.SetBool("companion.naeve.recruited", true);
            F.SetBool("companion.naeve.lost", true);
            Assert.IsFalse(Any(EndingResolver.Epilogue(Ending.MortalMeasure), "exactly the shape of everything"),
                "Loss without love is grief, but not THIS grief.");

            F.SetBool("romance.naeve.turn", true);
            Assert.IsTrue(Any(EndingResolver.Epilogue(Ending.MortalMeasure), "exactly the shape of everything"));
        }

        [Test]
        public void VarraLovedAndLost_GetsHerOwnSlide()
        {
            F.SetBool("companion.varra.recruited", true);
            F.SetBool("companion.varra.lost", true);
            F.SetBool("romance.varra.turn", true);
            Assert.IsTrue(Any(EndingResolver.Epilogue(Ending.MortalMeasure), "Maerin's tithe"));
        }

        // ---- romance slides --------------------------------------------------

        [Test]
        public void Romance_VariesByEnding()
        {
            F.SetBool("romance.garrow.consummated", true);
            Assert.IsTrue(Any(EndingResolver.Epilogue(Ending.BreakTheLoop), "I'll keep your name"));
            Assert.IsTrue(Any(EndingResolver.Epilogue(Ending.MortalMeasure), "any list but the living one"));
        }

        // ---- the Verdict ------------------------------------------------------

        [Test]
        public void Verdict_SparedOutranksPassed()
        {
            F.SetBool("crownwars.verdict_passed", true);
            Assert.IsTrue(Any(EndingResolver.Epilogue(Ending.MortalMeasure), "History only inherited it"));

            F.SetBool("crownwars.verdict_spared", true); // both set: spared wins the else-if
            var slides = EndingResolver.Epilogue(Ending.MortalMeasure);
            Assert.IsTrue(Any(slides, "ten thousand years early"));
            Assert.IsFalse(Any(slides, "History only inherited it"));
        }

        // ---- the anchor ---------------------------------------------------------

        [Test]
        public void AnchorId_RomanceDepth_BeatsOrderAndApproval()
        {
            F.SetBool("romance.roen.turn", true);          // shallow commitment
            F.SetBool("romance.varra.consummated", true);  // deepest commitment
            Assert.AreEqual("varra", EndingResolver.AnchorId(), "Depth of commitment outranks list order.");
        }

        [Test]
        public void AnchorId_ApprovalFallback_NeedsTwenty()
        {
            F.SetBool("companion.roen.recruited", true);
            F.SetInt("companion.roen.approval", 25);
            F.SetInt("companion.garrow.approval", 10);
            Assert.AreEqual("roen", EndingResolver.AnchorId());

            GameFlags.Replace(new GameFlags());
            F.SetInt("companion.garrow.approval", 19); // below the let-someone-in threshold
            Assert.IsNull(EndingResolver.AnchorId());
        }

        [Test]
        public void AnchorSlide_LostVariant()
        {
            F.SetBool("companion.varra.recruited", true);
            F.SetBool("companion.varra.lost", true);
            F.SetBool("romance.varra.consummated", true);
            Assert.IsTrue(Any(EndingResolver.Epilogue(Ending.MortalMeasure), "a hole with their name on it"));
        }

        // ---- chronicle ----------------------------------------------------------

        [Test]
        public void Chronicle_BaselineLines()
        {
            var lines = EndingResolver.Chronicle();
            Assert.IsTrue(Any(lines, "Eras walked: none yet"));
            Assert.IsTrue(Any(lines, "Companion quests resolved: 0/5"));
            Assert.IsTrue(Any(lines, "Endings unlocked: 3/6"));
            Assert.IsTrue(Any(lines, "Still at your side: Sister Garrow"));
        }

        [Test]
        public void Chronicle_TracksGoldenRoad_AndCompanionBuckets()
        {
            F.SetBool("readers_boon", true); // unlocks BreakTheLoop -> a golden road is open
            F.SetBool("companion.roen.recruited", true);
            F.SetBool("companion.roen.lost", true);
            F.SetBool("companion.naeve.recruited", true);
            F.SetBool("companion.naeve.left", true);
            var lines = EndingResolver.Chronicle();
            Assert.IsTrue(Any(lines, "a golden road is open"));
            Assert.IsTrue(Any(lines, "Taken by the Wall: Roen"));
            Assert.IsTrue(Any(lines, "Walked away: Naeve"));
        }
    }
}
