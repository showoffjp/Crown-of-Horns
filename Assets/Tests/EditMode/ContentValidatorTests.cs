using System.Collections.Generic;
using NUnit.Framework;
using SunderedCrown.Core;

namespace SunderedCrown.Tests.EditMode
{
    /// <summary>
    /// Promotes the existing runtime <see cref="ContentValidator"/> into a CI gate.
    /// It walks every reachable authored DialogueGraph (the five personal quests, Act II
    /// content, Aldric's tea, the Lady's riddles, the era-witness graphs) and flags broken
    /// node references — the bug class the no-compiler sandbox can't catch. Now a broken
    /// nextNodeId fails the build instead of shipping. DESIGN pillars I & III.
    /// </summary>
    public class ContentValidatorTests
    {
        [Test]
        public void AllAuthoredDialogueGraphs_HaveNoBrokenReferences()
        {
            List<string> issues = ContentValidator.Validate(out int graphCount, out int nodeCount);

            Assert.Greater(graphCount, 0,
                "Validator reached zero dialogue graphs — content wiring may have changed.");
            Assert.Greater(nodeCount, 0,
                "Validator counted zero dialogue nodes — content wiring may have changed.");
            Assert.IsEmpty(issues,
                "ContentValidator found broken dialogue references:\n - " + string.Join("\n - ", issues));
        }
    }
}
