using UnityEngine;
using SunderedCrown.Characters;
using SunderedCrown.Content;
using SunderedCrown.Stats;

namespace SunderedCrown.Core
{
    /// <summary>
    /// ONE-CLICK ROMANCE DEMO. Drop on an empty GameObject and press Play to sit at the fire with the four
    /// romanceable companions, their affinity pre-seeded high and their personal quests pre-resolved, so
    /// every romance's full six-stage arc is open to walk: Spark → Trust → Turn → Crisis → Choosing → The
    /// Last Night. Each beat lets you deepen it (sets romance.&lt;id&gt;.&lt;stage&gt;) or keep it platonic.
    /// Commit past the Turn with one and the others' Turn beats politely bow out — romance is a stake, not
    /// a collectible. Open P to watch approval move as you choose someone.
    /// </summary>
    public class RomanceDemo : MonoBehaviour
    {
        private SwordCoastContent _core;

        void Start()
        {
            if (GameManager.Instance == null) new GameObject("GameManager").AddComponent<GameManager>();
            if (Party.Instance == null) new GameObject("Party").AddComponent<Party>();

            _core = new SwordCoastContent();

            Party.Instance.Recruit(QuickHero("The Returned", "Fighter", "longsword"));
            Party.Instance.Recruit(QuickHero("Sister Garrow", "Cleric", "mace"));
            Party.Instance.Recruit(QuickHero("Roen Alleywind", "Rogue", "dagger"));
            Party.Instance.Recruit(QuickHero("Naeve", "Wizard", "firebolt"));
            Party.Instance.Recruit(QuickHero("Varra", "Wizard", "firebolt"));

            // Seed the gates: high approval, night-talks heard, personal quests resolved (the Crisis beat).
            foreach (var id in new[] { "garrow", "roen", "naeve", "varra" })
            {
                GameFlags.Current.SetInt($"companion.{id}.approval", 90);
                GameFlags.Current.SetBool($"camp.nighttalk.{id}.done", true);
                GameFlags.Current.SetBool($"quest.{id}.resolved", true);
            }

            var go = new GameObject("RomanceMode");
            var camp = go.AddComponent<SunderedCrown.UI.CampScene>();
            camp.onLeave = () => { Destroy(go); Debug.Log("[RomanceDemo] Camp broken. The road again."); };

            Debug.Log("[RomanceDemo] At the fire, open 'Grow Closer' to walk a romance arc. Deepen one past its " +
                      "Turn and the others step back. Press P to watch approval move.");
        }

        private CharacterSheet QuickHero(string name, string className, string abilityId)
        {
            var cls = _core.Classes.Find(c => c.className == className);
            var s = new CharacterSheet { displayName = name, classDef = cls, raceDef = _core.Races[0], level = 5, baseArmorClass = 14 };
            s.abilities.Set(Ability.Strength, 14);
            s.abilities.Set(Ability.Dexterity, 14);
            s.abilities.Set(Ability.Constitution, 14);
            s.abilities.Set(Ability.Wisdom, 14);
            s.abilities.Set(Ability.Charisma, 14);
            s.RecalculateMaxHitPoints();
            if (_core.Abilities.ContainsKey(abilityId))
            {
                s.knownAbilities.Add(_core.Abilities[abilityId]);
                s.equippedWeaponAbility = _core.Abilities[abilityId];
            }
            return s;
        }
    }
}
