using System.Collections.Generic;
using UnityEngine;
using SunderedCrown.Items;

namespace SunderedCrown.Characters
{
    /// <summary>
    /// The player's roster: the protagonist plus recruited companions. Holds the
    /// shared inventory and which members are in the active (max 4-6) field party.
    /// Lives on a persistent "GameManager" object across scene loads.
    /// </summary>
    public class Party : MonoBehaviour
    {
        public static Party Instance { get; private set; }

        [Tooltip("Maximum members fielded at once (classic CRPG: 4-6).")]
        public int maxActive = 4;

        public readonly List<CharacterSheet> roster = new List<CharacterSheet>();
        public readonly List<CharacterSheet> active = new List<CharacterSheet>();
        public Inventory inventory = new Inventory();

        void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public void Recruit(CharacterSheet member)
        {
            if (member == null || roster.Contains(member)) return;
            roster.Add(member);
            if (active.Count < maxActive) active.Add(member);
        }

        public bool SetActive(CharacterSheet member, bool makeActive)
        {
            if (makeActive)
            {
                if (active.Count >= maxActive || !roster.Contains(member)) return false;
                if (!active.Contains(member)) active.Add(member);
                return true;
            }
            return active.Remove(member);
        }

        public bool IsWiped()
        {
            foreach (var m in active) if (m.IsAlive) return false;
            return true;
        }
    }
}
