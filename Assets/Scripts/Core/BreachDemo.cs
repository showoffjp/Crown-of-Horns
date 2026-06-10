using UnityEngine;
using SunderedCrown.Characters;
using SunderedCrown.Content;
using SunderedCrown.Stats;

namespace SunderedCrown.Core
{
    /// <summary>
    /// ONE-CLICK BREACH DEMO. Drop on an empty GameObject and press Play to walk the Fugue Plane with
    /// a small party (hero + Garrow + Varra), reach Maerin in the Wall, and pull her free — which costs
    /// you a companion *permanently* (the Wall's tithe). Open the party screen (I) before and after to
    /// see the hole. Teaches the lesson the whole game leans on: this story takes things and keeps them.
    /// </summary>
    public class BreachDemo : MonoBehaviour
    {
        private SwordCoastContent _core;
        private FugueContent _fugue;
        private CharacterSheet _hero, _varra, _maerin;

        void Start()
        {
            if (GameManager.Instance == null) new GameObject("GameManager").AddComponent<GameManager>();
            if (Party.Instance == null) new GameObject("Party").AddComponent<Party>();

            _core = new SwordCoastContent();
            _fugue = new FugueContent(_core);

            _hero = QuickHero("The Returned", "Fighter", "longsword");
            Party.Instance.Recruit(_hero);
            _varra = QuickHero("Varra", "Rogue", "firebolt");
            Party.Instance.Recruit(_varra);
            Party.Instance.Recruit(QuickHero("Sister Garrow", "Cleric", "mace"));
            _maerin = _fugue.BuildMaerin();

            GameFlags.Current.OnFlagChanged += OnFlag;

            var root = new GameObject("FugueMode");
            var era = root.AddComponent<FugueEra>();
            era.content = _core; era.fugue = _fugue; era.leaderSheet = _hero;
            era.onLeave = () => { Destroy(root); Debug.Log("[BreachDemo] You climb out of the grey, carrying Maerin and a hole that won't fill."); };
            era.Begin();

            Debug.Log("[BreachDemo] Walk to Maerin in the Wall (grey marker). Press I to see your party first. " +
                      "Pulling her free will cost you Varra — forever.");
        }

        void OnDestroy()
        {
            // The demo can be torn down while GameFlags.Current persists - drop the handler.
            GameFlags.Current.OnFlagChanged -= OnFlag;
        }

        private void OnFlag(string key)
        {
            if (GameFlags.Current.GetBool("companion.maerin.recruited") && !Party.Instance.roster.Contains(_maerin))
                Party.Instance.Recruit(_maerin);

            if (key == "fugue.pull_maerin" && GameFlags.Current.GetBool("fugue.pull_maerin") &&
                Party.Instance.roster.Contains(_varra))
            {
                Party.Instance.Remove(_varra);
                GameFlags.Current.SetBool("companion.varra.lost", true);
                Debug.Log("[BreachDemo] The Wall took VARRA as its tithe — \"Of course it's me. I've had a receipt " +
                          "since I was six.\" She is gone. Permanently. Check the party screen.");
            }
        }

        private CharacterSheet QuickHero(string name, string className, string abilityId)
        {
            var cls = _core.Classes.Find(c => c.className == className);
            var s = new CharacterSheet { displayName = name, classDef = cls, raceDef = _core.Races[0], level = 4, baseArmorClass = 14 };
            s.abilities.Set(Ability.Strength, 14);
            s.abilities.Set(Ability.Dexterity, 14);
            s.abilities.Set(Ability.Constitution, 14);
            s.abilities.Set(Ability.Wisdom, 14);
            s.abilities.Set(Ability.Intelligence, 14);
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
