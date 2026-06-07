using UnityEngine;
using SunderedCrown.Characters;
using SunderedCrown.Content;
using SunderedCrown.Stats;

namespace SunderedCrown.Core
{
    /// <summary>
    /// ONE-CLICK CAMP DEMO. Drop on an empty GameObject and press Play to sit at the campfire with a
    /// small, battle-worn party. Their approval is pre-seeded high enough to unlock the night-talks, and
    /// one companion is "wounded" so you can watch the long rest heal them. Teaches the quiet half of a
    /// CRPG: rest, and let the people you travel with finally say what they've been carrying.
    /// </summary>
    public class CampDemo : MonoBehaviour
    {
        private SwordCoastContent _core;

        void Start()
        {
            if (GameManager.Instance == null) new GameObject("GameManager").AddComponent<GameManager>();
            if (Party.Instance == null) new GameObject("Party").AddComponent<Party>();

            _core = new SwordCoastContent();

            Party.Instance.Recruit(QuickHero("The Returned", "Fighter", "longsword"));
            var roen = QuickHero("Roen Alleywind", "Rogue", "dagger");
            Party.Instance.Recruit(roen);
            var varra = QuickHero("Varra", "Wizard", "firebolt");
            Party.Instance.Recruit(varra);

            // Pre-seed approval so the fire loosens tongues, and wound someone to show the rest heal.
            GameFlags.Current.SetInt("companion.roen.approval", 45);
            GameFlags.Current.SetInt("companion.varra.approval", 30);
            roen.TakeDamage(roen.maxHitPoints / 2);
            varra.spellSlots?.Spend(1);

            var go = new GameObject("CampMode");
            var camp = go.AddComponent<SunderedCrown.UI.CampScene>();
            camp.onLeave = () => { Destroy(go); Debug.Log("[CampDemo] Camp broken. The road again."); };

            Debug.Log("[CampDemo] At the fire: take a long rest (heals Roen), then sit with Roen or Varra " +
                      "for a night-talk. Open the party screen with P to see HP/approval.");
        }

        private CharacterSheet QuickHero(string name, string className, string abilityId)
        {
            var cls = _core.Classes.Find(c => c.className == className);
            var s = new CharacterSheet { displayName = name, classDef = cls, raceDef = _core.Races[0], level = 4, baseArmorClass = 14 };
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
