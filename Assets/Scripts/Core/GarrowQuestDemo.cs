using UnityEngine;
using SunderedCrown.Characters;
using SunderedCrown.Content;
using SunderedCrown.Stats;

namespace SunderedCrown.Core
{
    /// <summary>
    /// ONE-CLICK PREVIEW of Garrow's personal quest, "A God-Shaped Hole." Drop on an empty GameObject and
    /// press Play: a Kelemvorite heresy tribunal, the Justiciar Veld who taught her catechism, and a moral
    /// trilemma about what she owes a faith whose law would damn the people she loves (recant to keep the
    /// grey / walk away from the faith entirely / a [Religion] gambit that puts Kelemvor's own doctrine on
    /// trial). The battle is skipped for the tour; the choice is real.
    /// </summary>
    public class GarrowQuestDemo : MonoBehaviour
    {
        private SwordCoastContent _core;
        private GarrowQuestContent _quest;
        private CharacterSheet _hero;

        void Start()
        {
            if (GameManager.Instance == null) new GameObject("GameManager").AddComponent<GameManager>();
            if (Party.Instance == null) new GameObject("Party").AddComponent<Party>();

            _core = new SwordCoastContent();
            _quest = new GarrowQuestContent();

            _hero = QuickHero("The Returned", "Wizard", "firebolt");
            Party.Instance.Recruit(_hero);
            Party.Instance.Recruit(QuickHero("Sister Garrow", "Cleric", "mace"));
            GameFlags.Current.SetInt("companion.garrow.approval", 40);

            Enter(new Vector2Int(3, 7));

            Debug.Log("[GarrowQuestDemo] Examine the Scales, hear Justiciar Veld's charge, then 'refuse the rigged " +
                      "verdict' to skip to the moral choice. The [Religion] gambit needs INT — open P to watch " +
                      "Garrow's approval move.");
        }

        private void Enter(Vector2Int entry)
        {
            var root = new GameObject("GarrowQuestMode");
            var scene = root.AddComponent<PersonalQuestScene>();
            scene.leaderSheet = _hero;
            scene.quest = _quest.Quest;
            scene.entryCoord = entry;
            scene.onStartFight = (id, ret) =>
            {
                GameFlags.Current.SetBool(_quest.Quest.clearedFlag, true);
                Destroy(root);
                Enter(ret);
                Debug.Log("[GarrowQuestDemo] (Battle skipped for the preview.) The tribunal is scattered — decide what she answers the Scales.");
            };
            scene.onLeave = () =>
            {
                Destroy(root);
                Debug.Log("[GarrowQuestDemo] Quest resolved. Garrow approval is now " +
                          GameFlags.Current.GetInt("companion.garrow.approval") + ".");
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
