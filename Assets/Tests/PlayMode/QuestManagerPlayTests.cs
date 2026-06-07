using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using SunderedCrown.Core;
using SunderedCrown.Quests;

namespace SunderedCrown.Tests.PlayMode
{
    /// <summary>
    /// The flag-reactive half of QuestManager — the part wired up in Start() (a play-mode
    /// message), so EditMode can't reach it. Designers point objectives/quests at GameFlags
    /// keys and the manager advances state automatically when those flags flip. These pin
    /// that live reaction: objectives complete, quests complete, and quests fail, all from a
    /// flag change — and only for quests that are actually Active.
    ///
    /// Key detail: the manager subscribes to GameFlags.Current in Start(), so the suite sets
    /// up a fresh flag store *before* the manager spawns and then mutates that same instance
    /// (never Replace) so the subscription stays live.
    /// </summary>
    public class QuestManagerPlayTests
    {
        private readonly List<Object> _spawned = new List<Object>();
        private GameFlags _original;

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            _original = GameFlags.Current;
            GameFlags.Replace(new GameFlags()); // fresh store the manager will subscribe to
            yield break;
        }

        [TearDown]
        public void TearDown()
        {
            foreach (var o in _spawned) if (o != null) Object.DestroyImmediate(o); // OnDestroy unsubscribes
            _spawned.Clear();
            GameFlags.Replace(_original ?? new GameFlags());
        }

        private Quest MakeQuest(string id, string completionFlag = null, string failureFlag = null,
            string objectiveFlag = null)
        {
            var q = ScriptableObject.CreateInstance<Quest>();
            q.questId = id;
            q.title = id;
            q.completionFlag = completionFlag;
            q.failureFlag = failureFlag;
            if (objectiveFlag != null)
                q.objectives = new List<QuestObjective>
                {
                    new QuestObjective { objectiveId = "o1", description = "do the thing", completionFlag = objectiveFlag }
                };
            _spawned.Add(q);
            return q;
        }

        // Spawns the manager and runs a frame so Start() subscribes to GameFlags.Current.
        private IEnumerator Manager(Quest quest, System.Action<QuestManager> ready)
        {
            var go = new GameObject("QuestManager");
            _spawned.Add(go);
            var m = go.AddComponent<QuestManager>();
            m.allQuests = new List<Quest> { quest };
            yield return null; // Start() runs here
            ready(m);
        }

        [UnityTest]
        public IEnumerator CompletionFlag_FlippingTrue_CompletesAnActiveQuest()
        {
            var q = MakeQuest("q1", completionFlag: "q1.done");
            QuestManager mgr = null;
            yield return Manager(q, m => mgr = m);

            bool completed = false;
            mgr.OnQuestCompleted += _ => completed = true;
            mgr.StartQuest("q1");

            GameFlags.Current.SetBool("q1.done", true); // fires OnFlagChanged → manager reacts

            Assert.AreEqual(QuestStatus.Completed, mgr.StatusOf("q1"));
            Assert.IsTrue(completed, "OnQuestCompleted should fire.");
        }

        [UnityTest]
        public IEnumerator CompletionFlag_OnAnUnstartedQuest_DoesNothing()
        {
            var q = MakeQuest("q1", completionFlag: "q1.done");
            QuestManager mgr = null;
            yield return Manager(q, m => mgr = m);

            // Never started — flipping the flag must not auto-complete it.
            GameFlags.Current.SetBool("q1.done", true);

            Assert.AreEqual(QuestStatus.Unstarted, mgr.StatusOf("q1"));
        }

        [UnityTest]
        public IEnumerator ObjectiveFlag_FiresObjectiveCompleted()
        {
            var q = MakeQuest("q1", objectiveFlag: "q1.obj1");
            QuestManager mgr = null;
            yield return Manager(q, m => mgr = m);

            QuestObjective reported = null;
            mgr.OnObjectiveCompleted += (_, obj) => reported = obj;
            mgr.StartQuest("q1");

            GameFlags.Current.SetBool("q1.obj1", true);

            Assert.IsNotNull(reported, "OnObjectiveCompleted should fire for the matching objective.");
            Assert.AreEqual("o1", reported.objectiveId);
        }

        [UnityTest]
        public IEnumerator FailureFlag_FlippingTrue_FailsAnActiveQuest()
        {
            var q = MakeQuest("q1", failureFlag: "q1.failed");
            QuestManager mgr = null;
            yield return Manager(q, m => mgr = m);

            mgr.StartQuest("q1");
            GameFlags.Current.SetBool("q1.failed", true);

            Assert.AreEqual(QuestStatus.Failed, mgr.StatusOf("q1"));
        }
    }
}
