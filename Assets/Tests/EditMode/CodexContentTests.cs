using System.Linq;
using NUnit.Framework;
using SunderedCrown.Content;
using SunderedCrown.Core;

namespace SunderedCrown.Tests.EditMode
{
    /// <summary>
    /// The Codex fills in as you witness the world — "you only know what you've seen."
    /// These pin that the premise entries are known from the start, flag-gated entries
    /// surface on their flag, and the catalog is internally consistent (unique ids,
    /// TotalCount tracks the list).
    /// </summary>
    public class CodexContentTests
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

        [Test]
        public void CleanRun_KnowsExactlyTheUnconditionalEntries()
        {
            int alwaysKnown = CodexContent.All.Count(e => string.IsNullOrEmpty(e.unlockFlag));
            Assert.AreEqual(alwaysKnown, CodexContent.Known().Count);
            Assert.Greater(alwaysKnown, 0, "Some premise lore should be known from the start.");
            CollectionAssert.Contains(CodexContent.Known().Select(e => e.id).ToList(), "wall");
        }

        [Test]
        public void GatedEntry_UnlocksOnItsFlag()
        {
            Assert.IsFalse(CodexContent.Known().Any(e => e.id == "aldric"), "Hidden until met.");
            GameFlags.Current.SetBool("aldric.met", true);
            Assert.IsTrue(CodexContent.Known().Any(e => e.id == "aldric"), "Known once Aldric is met.");
        }

        [Test]
        public void TotalCount_TracksTheList()
        {
            Assert.AreEqual(CodexContent.All.Count, CodexContent.TotalCount);
            Assert.GreaterOrEqual(CodexContent.TotalCount, CodexContent.Known().Count,
                "You can never know more than exists.");
        }

        [Test]
        public void Catalog_HasUniqueIds()
        {
            var ids = CodexContent.All.Select(e => e.id).ToList();
            CollectionAssert.AllItemsAreUnique(ids);
        }

        [Test]
        public void Catalog_HasUniqueTitles()
        {
            // Two entries with the same title render as a confusing duplicate in the Codex
            // screen (it happened: two "The Echoes", two "The Almshouse of the Unclaimed").
            var dupes = CodexContent.All
                .GroupBy(e => e.title)
                .Where(g => g.Count() > 1)
                .Select(g => $"{g.Key} ×{g.Count()}")
                .ToList();
            CollectionAssert.IsEmpty(dupes, "Duplicate Codex titles: " + string.Join(", ", dupes));
        }
    }
}
