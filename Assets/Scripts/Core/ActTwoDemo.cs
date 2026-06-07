using UnityEngine;
using SunderedCrown.Characters;
using SunderedCrown.Content;
using SunderedCrown.Stats;

namespace SunderedCrown.Core
{
    /// <summary>
    /// ONE-CLICK ACT II DEMO. Drop on an empty GameObject and press Play to walk the Lower City hub with
    /// its Act II connective tissue live: Tamsin the street crier (who reads your reputation, companions,
    /// and eras straight off the flags), and two dialogue-resolved side quests — "The Widow's Coin" and
    /// "The Fist and the Faithless," each a small moral call that moves reputation and companion approval.
    /// The hero carries high WIS + CHA so the [Insight] and [Persuasion] branches can land. Resolve a quest,
    /// then talk to Tamsin again to watch the city's tale of you change.
    /// </summary>
    public class ActTwoDemo : MonoBehaviour
    {
        private SwordCoastContent _core;

        void Start()
        {
            if (GameManager.Instance == null) new GameObject("GameManager").AddComponent<GameManager>();
            if (Party.Instance == null) new GameObject("Party").AddComponent<Party>();

            _core = new SwordCoastContent();
            var hero = QuickHero("The Returned", "Cleric", "mace");
            Party.Instance.Recruit(hero);
            Party.Instance.Recruit(QuickHero("Sister Garrow", "Cleric", "mace"));

            // Act II opens after the Cinderhaunt; pretend that's done so the side content is live.
            GameFlags.Current.SetBool("prologue.cleared", true);
            GameFlags.Current.SetBool("companion.roen.recruited", true);  // hide the recruit NPCs for a tidy demo
            GameFlags.Current.SetBool("companion.varra.recruited", true);

            var go = new GameObject("ActTwoHub");
            var hub = go.AddComponent<BaldursGateHub>();
            hub.leaderSheet = hero;
            hub.content = _core;
            hub.actTwo = new ActTwoContent();
            hub.onEnterDungeon = () => Debug.Log("[ActTwoDemo] (The Cinderhaunt stairs — not part of this demo.)");
            hub.onEnterCamp = () => Debug.Log("[ActTwoDemo] (Camp — see CampDemo/RomanceDemo.)");
            hub.Begin();

            Debug.Log("[ActTwoDemo] Walk to Mhaere (grieving widow) and Sergeant Kallia (Flaming Fist) for the side " +
                      "quests, then talk to Tamsin the crier — and again after resolving one — to see the city react. " +
                      "Press J for the journey, P for the party.");
        }

        private CharacterSheet QuickHero(string name, string className, string abilityId)
        {
            var cls = _core.Classes.Find(c => c.className == className);
            var s = new CharacterSheet { displayName = name, classDef = cls, raceDef = _core.Races[0], level = 5, baseArmorClass = 14 };
            s.abilities.Set(Ability.Wisdom, 16);
            s.abilities.Set(Ability.Charisma, 16);
            s.abilities.Set(Ability.Constitution, 14);
            s.abilities.Set(Ability.Dexterity, 12);
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
