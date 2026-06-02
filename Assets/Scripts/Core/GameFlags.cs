using System;
using System.Collections.Generic;

namespace SunderedCrown.Core
{
    /// <summary>
    /// The global branching brain of the game. Every meaningful choice — a door
    /// opened, an NPC spared, a faction angered, an item taken — sets a flag here.
    /// Dialogue conditions, quest objectives, companion approval, and ending
    /// selection all read from this single source of truth, and it serializes
    /// cleanly into a save file.
    ///
    /// Convention for flag keys: "act1.smith.helped", "companion.sable.approval",
    /// "faction.ashpact.reputation", "world.warden_dead".
    /// </summary>
    [Serializable]
    public class GameFlags
    {
        // Boolean story switches.
        public Dictionary<string, bool> bools = new Dictionary<string, bool>();
        // Numeric values: reputation, approval, counters, currency.
        public Dictionary<string, int> ints = new Dictionary<string, int>();

        public static GameFlags Current { get; private set; } = new GameFlags();
        public static void Replace(GameFlags flags) => Current = flags ?? new GameFlags();

        public event Action<string> OnFlagChanged;

        // ---- Bools ----
        public bool GetBool(string key) => bools.TryGetValue(key, out var v) && v;
        public void SetBool(string key, bool value)
        {
            bools[key] = value;
            OnFlagChanged?.Invoke(key);
        }

        // ---- Ints ----
        public int GetInt(string key) => ints.TryGetValue(key, out var v) ? v : 0;
        public void SetInt(string key, int value)
        {
            ints[key] = value;
            OnFlagChanged?.Invoke(key);
        }
        public void AddInt(string key, int delta) => SetInt(key, GetInt(key) + delta);

        /// <summary>Convenience for companion approval, clamped to a readable band.</summary>
        public void AdjustApproval(string companionId, int delta)
        {
            string key = $"companion.{companionId}.approval";
            int v = Math.Clamp(GetInt(key) + delta, -100, 100);
            SetInt(key, v);
        }
    }
}
