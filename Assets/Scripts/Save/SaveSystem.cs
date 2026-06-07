using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using SunderedCrown.Core;
using SunderedCrown.Quests;
using SunderedCrown.Stats;

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

            // The hero's build (so Continue can reconstruct the protagonist). The roster is
            // rebuilt from the companion.<id>.recruited / .lost flags above.
            public string heroName;
            public string heroClass;
            public string heroRace;
            public int heroLevel = 1;
            public List<int> heroScores = new List<int>(); // STR, DEX, CON, INT, WIS, CHA
        }

        /// <summary>The most recently loaded data — read by the bootstrapper to reconstruct the hero.</summary>
        public static SaveData Last { get; private set; }

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
            {
                data.partyGold = Characters.Party.Instance.inventory.gold;

                // Capture the hero (the first active member = the Player leader).
                if (Characters.Party.Instance.active.Count > 0)
                {
                    var h = Characters.Party.Instance.active[0];
                    data.heroName = h.displayName;
                    data.heroClass = h.classDef != null ? h.classDef.className : "";
                    data.heroRace = h.raceDef != null ? h.raceDef.raceName : "";
                    data.heroLevel = h.level;
                    foreach (Ability a in Enum.GetValues(typeof(Ability))) data.heroScores.Add(h.abilities.Get(a));
                }
            }

            File.WriteAllText(PathFor(slot), JsonUtility.ToJson(data, true));
            Debug.Log($"[Save] Wrote slot '{slot}' to {PathFor(slot)}");
        }

        public static bool Load(string slot)
        {
            string path = PathFor(slot);
            if (!File.Exists(path)) { Debug.LogWarning($"[Save] No save at {path}"); return false; }

            var data = JsonUtility.FromJson<SaveData>(File.ReadAllText(path));
            Last = data;

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

        public static void Delete(string slot)
        {
            var p = PathFor(slot);
            if (File.Exists(p)) { File.Delete(p); Debug.Log($"[Save] Deleted slot '{slot}'."); }
        }

        /// <summary>Lightweight slot summary for a save-slot menu — no global state touched.</summary>
        public struct SlotMeta { public bool exists; public string heroName; public int heroLevel; public string sceneName; public string savedAtUtc; }

        public static SlotMeta Peek(string slot)
        {
            var p = PathFor(slot);
            if (!File.Exists(p)) return new SlotMeta { exists = false };
            try
            {
                var d = JsonUtility.FromJson<SaveData>(File.ReadAllText(p));
                return new SlotMeta { exists = true, heroName = d.heroName, heroLevel = d.heroLevel,
                                      sceneName = d.sceneName, savedAtUtc = d.savedAtUtc };
            }
            catch { return new SlotMeta { exists = false }; }
        }
    }
}
