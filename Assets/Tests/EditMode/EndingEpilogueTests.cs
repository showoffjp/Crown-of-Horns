using System.Collections.Generic;
using NUnit.Framework;
using SunderedCrown.Core;

namespace SunderedCrown.Tests.EditMode
{
    /// <summary>
    /// The BG2-style epilogue slides and the Chronicle run-summary — DESIGN pillar II made
    /// readable: the finale reflects who lived, who the Wall took, what you resolved. These
    /// pin that slides react to the loss flags and the Chronicle tallies the playthrough.
    /// Mutates the global GameFlags, so it snapshots/restores around each test.
    /// </summary>
    public class EndingEpilogueTests
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

        private static string SlideMentioning(List<string> slides, string name)
            => slides.Find(s => s != null && s.Contains(name));

        // ---- Epilogue ------------------------------------------------------

        [Test]
        public void Epilogue_AlwaysSpeaksToTheStartingCompanion()
        {
            var slides = EndingResolver.Epilogue(Ending.MortalMeasure);
            Assert.IsNotNull(SlideMentioning(slides, "Garrow"),
                "Garrow (the starting companion) always gets an epilogue slide.");
        }

        [Test]
        public void Epilogue_LostCompanion_ReadsAsTakenByTheWall()
        {
            GameFlags.Current.SetBool("companion.garrow.lost", true);
            var slide = SlideMentioning(EndingResolver.Epilogue(Ending.MortalMeasure), "Garrow");
            StringAssert.Contains("taken by the Wall", slide);
        }

        [Test]
        public void Epilogue_RecruitedCompanion_GetsASlide()
        {
            Assert.IsNull(SlideMentioning(EndingResolver.Epilogue(Ending.MortalMeasure), "Roen"),
                "Un-recruited companions get no slide.");
            GameFlags.Current.SetBool("companion.roen.recruited", true);
            Assert.IsNotNull(SlideMentioning(EndingResolver.Epilogue(Ending.MortalMeasure), "Roen"));
        }

        [Test]
        public void Epilogue_PresentSlide_IsTintedByTheEnding()
        {
            string mortal = SlideMentioning(EndingResolver.Epilogue(Ending.MortalMeasure), "Garrow");
            string loop = SlideMentioning(EndingResolver.Epilogue(Ending.BreakTheLoop), "Garrow");
            Assert.AreNotEqual(mortal, loop, "A present companion's send-off changes with the ending chosen.");
        }

        // ---- Chronicle -----------------------------------------------------

        [Test]
        public void Chronicle_CleanRun_ReportsNoErasAndNoQuests()
        {
            var lines = EndingResolver.Chronicle();
            Assert.IsNotEmpty(lines);
            Assert.IsTrue(lines.Exists(l => l.Contains("Eras walked") && l.Contains("none yet")));
            Assert.IsTrue(lines.Exists(l => l.Contains("Companion quests resolved: 0/5")));
        }

        [Test]
        public void Chronicle_ListsErasWalked()
        {
            GameFlags.Current.SetBool("netheril.cleared", true);
            GameFlags.Current.SetBool("crownwars.cleared", true);
            var eraLine = EndingResolver.Chronicle().Find(l => l.Contains("Eras walked"));
            StringAssert.Contains("Netheril", eraLine);
            StringAssert.Contains("Crown Wars", eraLine);
        }

        [Test]
        public void Chronicle_CountsResolvedCompanionQuests()
        {
            GameFlags.Current.SetBool("quest.roen.resolved", true);
            GameFlags.Current.SetBool("quest.garrow.resolved", true);
            Assert.IsTrue(EndingResolver.Chronicle().Exists(l => l.Contains("Companion quests resolved: 2/5")));
        }

        [Test]
        public void Chronicle_RecordsACompanionTakenByTheWall()
        {
            GameFlags.Current.SetBool("companion.garrow.lost", true);
            var line = EndingResolver.Chronicle().Find(l => l.Contains("Taken by the Wall"));
            Assert.IsNotNull(line);
            StringAssert.Contains("Garrow", line);
        }
    }
}
