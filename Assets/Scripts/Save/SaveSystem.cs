using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using SunderedCrown.Core;
using SunderedCrown.Quests;

namespace SunderedCrown.Save
{
    /// <summary>
    /// JSON save/load. We serialize the GameFlags (the story brain), quest status,
    /// party gold, and lightweight character snapshots. ScriptableObjects (classes,
    /// items, dialogue) are referenced by id and re-linked on load, so saves stay
    /// tiny and survive content edits.
    ///
    /// Unity's JsonUtility can't serialize Dictionaries directly, so we flatten
    /// them into parallel lists in the DTO below.
    /// </summary>
    public static class SaveSystem
    {
        [Serializable]
        public class SaveData
        {
            public string version = "0.1.0";
            public string sceneName;
            public string savedAtUtc;

            // Flattened GameFlags.
            public List<string> boolKeys = new List<string>();
            public List<bool> boolValues = new List<bool>();
            public List<string> intKeys = new List<string>();
            public List<int> intValues = new List<int>();

            // Flattened quest status.
            public List<string> questIds = new List<string>();
            public List<int> questStatuses = new List<int>();

            public int partyGold;
        }

        private static string SaveDir => Path.Combine(Application.persistentDataPath, "saves");
        private static string PathFor(string slot) => Path.Combine(SaveDir, $"{slot}.json");

        public static void Save(string slot, string sceneName)
        {
            Directory.CreateDirectory(SaveDir);
            var data = new SaveData
            {
                sceneName = sceneName,
                savedAtUtc = DateTime.UtcNow.ToString("o")
            };

            var flags = GameFlags.Current;
            foreach (var kv in flags.bools) { data.boolKeys.Add(kv.Key); data.boolValues.Add(kv.Value); }
            foreach (var kv in flags.ints)  { data.intKeys.Add(kv.Key);  data.intValues.Add(kv.Value); }

            if (QuestManager.Instance != null)
                foreach (var kv in QuestManager.Instance.ExportState())
                {
                    data.questIds.Add(kv.Key);
                    data.questStatuses.Add((int)kv.Value);
                }

            if (Characters.Party.Instance != null)
                data.partyGold = Characters.Party.Instance.inventory.gold;

            File.WriteAllText(PathFor(slot), JsonUtility.ToJson(data, true));
            Debug.Log($"[Save] Wrote slot '{slot}' to {PathFor(slot)}");
        }

        public static bool Load(string slot)
        {
            string path = PathFor(slot);
            if (!File.Exists(path)) { Debug.LogWarning($"[Save] No save at {path}"); return false; }

            var data = JsonUtility.FromJson<SaveData>(File.ReadAllText(path));

            var flags = new GameFlags();
            for (int i = 0; i < data.boolKeys.Count; i++) flags.bools[data.boolKeys[i]] = data.boolValues[i];
            for (int i = 0; i < data.intKeys.Count; i++)  flags.ints[data.intKeys[i]]   = data.intValues[i];
            GameFlags.Replace(flags);

            if (QuestManager.Instance != null)
            {
                var status = new Dictionary<string, QuestStatus>();
                for (int i = 0; i < data.questIds.Count; i++)
                    status[data.questIds[i]] = (QuestStatus)data.questStatuses[i];
                QuestManager.Instance.ImportState(status);
            }

            if (Characters.Party.Instance != null)
                Characters.Party.Instance.inventory.gold = data.partyGold;

            Debug.Log($"[Save] Loaded slot '{slot}' (scene {data.sceneName}).");
            return true;
        }

        public static bool Exists(string slot) => File.Exists(PathFor(slot));
    }
}
