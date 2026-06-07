using UnityEngine;
using SunderedCrown.Characters;
using SunderedCrown.Content;
using SunderedCrown.Stats;

namespace SunderedCrown.Core
{
    /// <summary>
    /// ONE-CLICK PREVIEW of Naeve's personal quest, "After the Sky Fell." Drop on an empty GameObject and
    /// press Play: a surviving fragment of the Seventh Enclave still falling a thousand years on, the echo
    /// of the Steward Vaelin, and a moral trilemma about what a person owes the world they ended (freeze
    /// the shard as a tomb / let it finish falling and carry the grief / an [Arcana] gambit that gives the
    /// last live Weave back to the present as a seed). The battle is skipped for the tour; the choice is real.
    /// </summary>
    public class NaeveQuestDemo : MonoBehaviour
    {
        private SwordCoastContent _core;
        private NaeveQuestContent _quest;
        private CharacterSheet _hero;

        void Start()
        {
            if (GameManager.Instance == null) new GameObject("GameManager").AddComponent<GameManager>();
            if (Party.Instance == null) new GameObject("Party").AddComponent<Party>();

            _core = new SwordCoastContent();
            _quest = new NaeveQuestContent();

            _hero = QuickHero("The Returned", "Wizard", "firebolt");
            Party.Instance.Recruit(_hero);
            Party.Instance.Recruit(QuickHero("Naeve", "Wizard", "firebolt"));
            GameFlags.Current.SetInt("companion.naeve.approval", 40);

            Enter(new Vector2Int(3, 7));

            Debug.Log("[NaeveQuestDemo] Examine the mythallar, hear Vaelin's echo, then 'past the last protocol' to skip " +
                      "to the moral choice. The [Arcana] gambit needs INT — open P to watch Naeve's approval move.");
        }

        private void Enter(Vector2Int entry)
        {
            var root = new GameObject("NaeveQuestMode");
            var scene = root.AddComponent<PersonalQuestScene>();
            scene.leaderSheet = _hero;
            scene.quest = _quest.Quest;
            scene.entryCoord = entry;
            scene.onStartFight = (id, ret) =>
            {
                GameFlags.Current.SetBool(_quest.Quest.clearedFlag, true);
                Destroy(root);
                Enter(ret);
                Debug.Log("[NaeveQuestDemo] (Battle skipped for the preview.) The wards are still — decide what becomes of the core.");
            };
            scene.onLeave = () =>
            {
                Destroy(root);
                Debug.Log("[NaeveQuestDemo] Quest resolved. Naeve approval is now " +
                          GameFlags.Current.GetInt("companion.naeve.approval") + ".");
            };
            scene.Begin();
        }

        private CharacterSheet QuickHero(string name, string className, string abilityId)
        {
            var cls = _core.Classes.Find(c => c.className == className);
            var s = new CharacterSheet { displayName = name, classDef = cls, raceDef = _core.Races[0], level = 5, baseArmorClass = 14 };
            s.abilities.Set(Ability.Intelligence, 16);
            s.abilities.Set(Ability.Dexterity, 14);
            s.abilities.Set(Ability.Constitution, 14);
            s.abilities.Set(Ability.Wisdom, 14);
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
