using System;
using System.Collections.Generic;
using UnityEngine;

namespace SunderedCrown.Quests
{
    public enum QuestStatus { Unstarted, Active, Completed, Failed }

    [Serializable]
    public class QuestObjective
    {
        public string objectiveId;
        [TextArea(1, 2)] public string description;

        [Tooltip("The GameFlags bool key that, when true, completes this objective.")]
        public string completionFlag;

        [Tooltip("Hidden objectives don't show in the journal until active.")]
        public bool hidden = false;
        public bool optional = false;
    }

    /// <summary>
    /// A quest authored as a ScriptableObject. Objectives complete by watching
    /// GameFlags, so quest logic stays declarative and saves trivially.
    ///
    /// Create via: Assets > Create > Sundered Crown > Quest
    /// </summary>
    [CreateAssetMenu(menuName = "Sundered Crown/Quest", fileName = "NewQuest")]
    public class Quest : ScriptableObject
    {
        public string questId;
        public string title;
        [TextArea(2, 5)] public string summary;

        [Tooltip("Set true automatically when this flag flips (the quest 'turns in').")]
        public string completionFlag;
        public string failureFlag;

        public List<QuestObjective> objectives = new List<QuestObjective>();

        [Header("Rewards")]
        public int experienceReward = 100;
        public int goldReward = 0;
    }
}
