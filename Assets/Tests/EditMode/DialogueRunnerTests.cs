using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using SunderedCrown.Core;
using SunderedCrown.Dialogue;
using SunderedCrown.Stats;

namespace SunderedCrown.Tests.EditMode
{
    /// <summary>
    /// The branching-dialogue engine — DESIGN pillar III's mouthpiece. Drives a runner over
    /// hand-authored graphs to pin: onEnter effects fire, choices gate on flag conditions,
    /// picking a choice applies its effects and navigates, skill checks branch on success/
    /// failure, auto-advance chains lines, and conversations end cleanly.
    /// </summary>
    public class DialogueRunnerTests
    {
        private readonly List<Object> _spawned = new List<Object>();
        private GameFlags _original;

        [SetUp]
        public void SetUp()
        {
            _original = GameFlags.Current;
            GameFlags.Replace(new GameFlags());
        }

        [TearDown]
        public void TearDown()
        {
            foreach (var o in _spawned) if (o != null) Object.DestroyImmediate(o);
            _spawned.Clear();
            GameFlags.Replace(_original ?? new GameFlags());
        }

        // --- authoring helpers ---------------------------------------------

        private DialogueRunner Runner()
        {
            var go = new GameObject("DialogueRunner");
            _spawned.Add(go);
            return go.AddComponent<DialogueRunner>();
        }

        private DialogueGraph Graph(string start, params DialogueNode[] nodes)
        {
            var g = ScriptableObject.CreateInstance<DialogueGraph>();
            g.startNodeId = start;
            g.nodes = new List<DialogueNode>(nodes);
            _spawned.Add(g);
            return g;
        }

        private static DialogueNode Node(string id, DialogueChoice[] choices = null,
            string autoNext = null, FlagClause[] onEnter = null)
            => new DialogueNode { id = id, speaker = "NPC", text = id, choices = choices, autoNextNodeId = autoNext, onEnter = onEnter };

        private static DialogueChoice Choice(string text, string next, FlagClause[] conditions = null,
            FlagClause[] effects = null, int dc = 0, string fail = null, Ability ability = Ability.Charisma)
            => new DialogueChoice { text = text, nextNodeId = next, conditions = conditions, effects = effects,
                                    checkDC = dc, failNodeId = fail, checkAbility = ability };

        private static FlagClause Clause(string key, FlagOp op, int amount = 0)
            => new FlagClause { key = key, op = op, amount = amount };

        private static int SeedForFirstD20(int face)
        {
            for (int s = 0; s < 1_000_000; s++) { Dice.Seed(s); if (Dice.D20() == face) return s; }
            Assert.Fail($"no seed gave first d20 {face}"); return -1;
        }

        // --- tests ----------------------------------------------------------

        [Test]
        public void Begin_ShowsStartNode_AndAppliesOnEnterEffects()
        {
            var g = Graph("start", Node("start", onEnter: new[] { Clause("entered", FlagOp.SetTrue) }));
            var r = Runner();
            DialogueNode shown = null;
            r.OnNodeShown += (n, _) => shown = n;

            r.Begin(g);

            Assert.IsTrue(r.IsActive);
            Assert.AreEqual("start", shown.id);
            Assert.IsTrue(GameFlags.Current.GetBool("entered"), "onEnter effects must fire when the node shows.");
        }

        [Test]
        public void Choices_AreGatedByConditions()
        {
            var locked = Choice("secret", "end", conditions: new[] { Clause("key", FlagOp.RequireBoolTrue) });
            var open = Choice("hello", "end");
            var g = Graph("hub", Node("hub", new[] { open, locked }), Node("end"));
            var r = Runner();
            List<DialogueChoice> available = null;
            r.OnNodeShown += (_, choices) => available = choices;

            r.Begin(g);
            Assert.AreEqual(1, available.Count, "Locked choice hidden while its flag is false.");

            GameFlags.Current.SetBool("key", true);
            r.Begin(g); // re-enter with the flag set
            Assert.AreEqual(2, available.Count, "Both choices visible once the condition passes.");
        }

        [Test]
        public void Choose_AppliesEffects_AndNavigates()
        {
            var pick = Choice("take gold", "end",
                effects: new[] { Clause("party.gold", FlagOp.AddInt, 5), Clause("talked", FlagOp.SetTrue) });
            var g = Graph("hub", Node("hub", new[] { pick }), Node("end"));
            var r = Runner();
            DialogueNode shown = null;
            r.OnNodeShown += (n, _) => shown = n;

            r.Begin(g);
            r.Choose(pick);

            Assert.AreEqual(5, GameFlags.Current.GetInt("party.gold"));
            Assert.IsTrue(GameFlags.Current.GetBool("talked"));
            Assert.AreEqual("end", shown.id);
        }

        [Test]
        public void SkillCheck_Success_TakesTheMainBranch()
        {
            var check = Choice("persuade", "win", dc: 10, fail: "lose");
            var g = Graph("ask", Node("ask", new[] { check }), Node("win"), Node("lose"));
            var r = Runner();
            DialogueNode shown = null; bool? success = null;
            r.OnNodeShown += (n, _) => shown = n;
            r.OnSkillCheck += (_, _, _, ok) => success = ok;

            r.Begin(g);
            Dice.Seed(SeedForFirstD20(20)); // a 20 beats DC 10
            r.Choose(check);

            Assert.IsTrue(success.Value);
            Assert.AreEqual("win", shown.id);
        }

        [Test]
        public void SkillCheck_Failure_TakesTheFailBranch()
        {
            var check = Choice("persuade", "win", dc: 10, fail: "lose");
            var g = Graph("ask", Node("ask", new[] { check }), Node("win"), Node("lose"));
            var r = Runner();
            DialogueNode shown = null; bool? success = null;
            r.OnNodeShown += (n, _) => shown = n;
            r.OnSkillCheck += (_, _, _, ok) => success = ok;

            r.Begin(g);
            Dice.Seed(SeedForFirstD20(1)); // a 1 fails DC 10
            r.Choose(check);

            Assert.IsFalse(success.Value);
            Assert.AreEqual("lose", shown.id);
        }

        [Test]
        public void AutoNext_ChainsLinesWithoutChoices()
        {
            var g = Graph("a", Node("a", autoNext: "b"), Node("b"));
            var r = Runner();
            DialogueNode shown = null;
            r.OnNodeShown += (n, _) => shown = n;

            r.Begin(g);
            Assert.AreEqual("b", shown.id, "A choiceless node auto-advances to its autoNext.");
        }

        [Test]
        public void Conversation_EndsOnEmptyNextOrContinue()
        {
            var farewell = Choice("bye", ""); // empty next → ends
            var g = Graph("hub", Node("hub", new[] { farewell }));
            var r = Runner();
            bool ended = false;
            r.OnConversationEnded += () => ended = true;

            r.Begin(g);
            r.Choose(farewell);

            Assert.IsTrue(ended);
            Assert.IsFalse(r.IsActive);
        }

        [Test]
        public void Begin_Null_IsIgnored()
        {
            var r = Runner();
            r.Begin(null);
            Assert.IsFalse(r.IsActive);
        }
    }
}
