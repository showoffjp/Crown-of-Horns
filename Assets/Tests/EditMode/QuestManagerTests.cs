using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using SunderedCrown.Quests;

namespace SunderedCrown.Tests.EditMode
{
    /// <summary>
    /// The quest state machine: explicit start, status tracking, and the save hooks
    /// (export/import). The live flag-reactive objective completion is wired in Start()
    /// (a play-mode message), so it's covered by PlayMode tests; here we pin the pure,
    /// EditMode-reachable contract that persistence and the journal depend on.
    /// </summary>
    public class QuestManagerTests
    {
        private readonly List<Object> _spawned = new List<Object>();

        private QuestManager Mgr(params Quest[] quests)
        {
            var go = new GameObject("QuestManager");
            _spawned.Add(go);
            var m = go.AddComponent<QuestManager>();
            m.allQuests = new List<Quest>(quests);
            return m;
        }

        private Quest MakeQuest(string id)
        {
            var q = ScriptableObject.CreateInstance<Quest>();
            q.questId = id;
            q.title = id;
            _spawned.Add(q);
            return q;
        }

        [TearDown]
        public void Cleanup()
        {
            foreach (var o in _spawned) if (o != null) Object.DestroyImmediate(o);
            _spawned.Clear();
        }

        [Test]
        public void StatusOf_Unknown_IsUnstarted()
        {
            Assert.AreEqual(QuestStatus.Unstarted, Mgr().StatusOf("nope"));
        }

        [Test]
        public void StartQuest_ActivatesAndFiresEvent()
        {
            var q = MakeQuest("q1");
            var m = Mgr(q);
            bool fired = false;
            m.OnQuestStarted += _ => fired = true;

            m.StartQuest("q1");

            Assert.AreEqual(QuestStatus.Active, m.StatusOf("q1"));
            Assert.IsTrue(fired);
        }

        [Test]
        public void StartQuest_UnknownId_IsNoOp()
        {
            var m = Mgr();
            bool fired = false;
            m.OnQuestStarted += _ => fired = true;
            m.StartQuest("ghost");
            Assert.IsFalse(fired);
        }

        [Test]
        public void StartQuest_Twice_FiresOnlyOnce()
        {
            var q = MakeQuest("q1");
            var m = Mgr(q);
            int count = 0;
            m.OnQuestStarted += _ => count++;

            m.StartQuest("q1");
            m.StartQuest("q1");

            Assert.AreEqual(1, count);
            Assert.AreEqual(QuestStatus.Active, m.StatusOf("q1"));
        }

        [Test]
        public void ExportState_IsACopy_NotALiveView()
        {
            var q = MakeQuest("q1");
            var m = Mgr(q);
            m.StartQuest("q1");

            var snapshot = m.ExportState();
            Assert.AreEqual(QuestStatus.Active, snapshot["q1"]);

            snapshot["q1"] = QuestStatus.Completed; // mutate the copy
            Assert.AreEqual(QuestStatus.Active, m.StatusOf("q1"), "Export must be a detached copy.");
        }

        [Test]
        public void ImportState_ReplacesStatus()
        {
            var q = MakeQuest("q1");
            var m = Mgr(q);
            m.ImportState(new Dictionary<string, QuestStatus> { { "q1", QuestStatus.Completed } });
            Assert.AreEqual(QuestStatus.Completed, m.StatusOf("q1"));
        }

        [Test]
        public void ImportState_Null_ClearsState()
        {
            var q = MakeQuest("q1");
            var m = Mgr(q);
            m.StartQuest("q1");
            m.ImportState(null);
            Assert.AreEqual(QuestStatus.Unstarted, m.StatusOf("q1"));
        }
    }
}
