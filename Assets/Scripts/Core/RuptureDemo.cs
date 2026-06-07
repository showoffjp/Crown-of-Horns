using UnityEngine;
using SunderedCrown.Characters;
using SunderedCrown.Content;
using SunderedCrown.Stats;

namespace SunderedCrown.Core
{
    /// <summary>
    /// ONE-CLICK RUPTURE DEMO. Drop on an empty GameObject and press Play to sit at the fire with a party
    /// whose bonds have cratered — every companion's approval is in the basement, so each has a pending
    /// "A Bond Frays" confrontation. Two are seeded with *standing* (a shared night-talk), so making amends
    /// can actually land; two are not, to show that words alone can't talk back someone you never bothered
    /// to know. From each you can mend, patch it over (they stay, guarded), or let them walk out for good.
    /// </summary>
    public class RuptureDemo : MonoBehaviour
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

            // Crater every bond so each rupture is pending.
            foreach (var id in new[] { "garrow", "roen", "naeve", "varra" })
                GameFlags.Current.SetInt($"companion.{id}.approval", -30);

            // Give standing with two of them (a shared night-talk) so amends can land; withhold it from the others.
            GameFlags.Current.SetBool("camp.nighttalk.garrow.done", true);
            GameFlags.Current.SetBool("camp.nighttalk.roen.done", true);

            var go = new GameObject("RuptureMode");
            var camp = go.AddComponent<SunderedCrown.UI.CampScene>();
            camp.onLeave = () =>
            {
                Destroy(go);
                string left = "";
                foreach (var id in new[] { "garrow", "roen", "naeve", "varra" })
                    if (GameFlags.Current.GetBool($"companion.{id}.left")) left += id + " ";
                Debug.Log("[RuptureDemo] Camp broken. Walked out: " + (left == "" ? "(no one — you held them)" : left));
            };

            Debug.Log("[RuptureDemo] 'A Bond Frays' is waiting at the fire. Garrow & Roen will hear you out (you've " +
                      "earned standing); Naeve & Varra won't be talked round with words alone. Press P to watch the roster.");
        }

        private CharacterSheet QuickHero(string name, string className, string abilityId)
        {
            var cls = _core.Classes.Find(c => c.className == className);
            var s = new CharacterSheet { displayName = name, classDef = cls, raceDef = _core.Races[0], level = 5, baseArmorClass = 14 };
            s.abilities.Set(Ability.Strength, 14);
            s.abilities.Set(Ability.Dexterity, 14);
            s.abilities.Set(Ability.Constitution, 14);
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
