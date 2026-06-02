using System;
using System.Collections.Generic;
using UnityEngine;
using SunderedCrown.Core;

namespace SunderedCrown.Quests
{
    /// <summary>
    /// Tracks quest progress by reacting to GameFlags changes. Designers never
    /// write quest code: they author a Quest asset, point objectives at flag keys,
    /// and the manager advances state automatically.
    /// </summary>
    public class QuestManager : MonoBehaviour
    {
        public static QuestManager Instance { get; private set; }

        [Tooltip("All quests that can appear in this campaign.")]
        public List<Quest> allQuests = new List<Quest>();

        // Runtime status keyed by questId. Persists via the save system.
        private readonly Dictionary<string, QuestStatus> _status = new Dictionary<string, QuestStatus>();

        public event Action<Quest> OnQuestStarted;
        public event Action<Quest> OnQuestCompleted;
        public event Action<Quest, QuestObjective> OnObjectiveCompleted;

        void Awake()
        {
            Instance = this;
        }

        void Start()
        {
            GameFlags.Current.OnFlagChanged += OnFlagChanged;
        }

        void OnDestroy()
        {
            if (GameFlags.Current != null)
                GameFlags.Current.OnFlagChanged -= OnFlagChanged;
        }

        public QuestStatus StatusOf(string questId) =>
            _status.TryGetValue(questId, out var s) ? s : QuestStatus.Unstarted;

        public void StartQuest(string questId)
        {
            var quest = Find(questId);
            if (quest == null || StatusOf(questId) != QuestStatus.Unstarted) return;
            _status[questId] = QuestStatus.Active;
            OnQuestStarted?.Invoke(quest);
        }

        private void OnFlagChanged(string key)
        {
            var flags = GameFlags.Current;
            foreach (var quest in allQuests)
            {
                if (quest == null) continue;
                var status = StatusOf(quest.questId);

                // Auto-start when the first objective's flag is touched? No —
                // starting is explicit. But objective + completion react live.
                if (status == QuestStatus.Active)
                {
                    foreach (var obj in quest.objectives)
                        if (obj.completionFlag == key && flags.GetBool(key))
                            OnObjectiveCompleted?.Invoke(quest, obj);

                    if (!string.IsNullOrEmpty(quest.failureFlag) && flags.GetBool(quest.failureFlag))
                    {
                        _status[quest.questId] = QuestStatus.Failed;
                        continue;
                    }

                    if (!string.IsNullOrEmpty(quest.completionFlag) && flags.GetBool(quest.completionFlag))
                    {
                        _status[quest.questId] = QuestStatus.Completed;
                        OnQuestCompleted?.Invoke(quest);
                    }
                }
            }
        }

        private Quest Find(string questId) => allQuests.Find(q => q != null && q.questId == questId);

        // Save/load hooks.
        public Dictionary<string, QuestStatus> ExportState() => new Dictionary<string, QuestStatus>(_status);
        public void ImportState(Dictionary<string, QuestStatus> state)
        {
            _status.Clear();
            if (state != null) foreach (var kv in state) _status[kv.Key] = kv.Value;
        }
    }
}
