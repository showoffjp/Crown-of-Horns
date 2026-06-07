using UnityEngine;
using SunderedCrown.Characters;
using SunderedCrown.Content;
using SunderedCrown.Stats;

namespace SunderedCrown.Core
{
    /// <summary>
    /// ONE-CLICK PREVIEW of Ilfaeril's personal quest, "The Vote." Drop on an empty GameObject and press
    /// Play: an ancient elven reliquary keyed to the name of a soul his Crown-War council voted to unmake,
    /// the Pale Cantor of the Choir who would melt his guilt down into a key, and a moral trilemma about
    /// whether atonement is allowed to end (refuse forgiveness and keep paying / accept it / an [Insight]
    /// gambit that reframes her forgiveness as a commission). The battle is skipped for the tour; the
    /// choice is real. The hero carries high WIS so the [Insight] branch can land.
    /// </summary>
    public class IlfaerilQuestDemo : MonoBehaviour
    {
        private SwordCoastContent _core;
        private IlfaerilQuestContent _quest;
        private CharacterSheet _hero;

        void Start()
        {
            if (GameManager.Instance == null) new GameObject("GameManager").AddComponent<GameManager>();
            if (Party.Instance == null) new GameObject("Party").AddComponent<Party>();

            _core = new SwordCoastContent();
            _quest = new IlfaerilQuestContent();

            _hero = QuickHero("The Returned", "Cleric", "mace");
            Party.Instance.Recruit(_hero);
            Party.Instance.Recruit(QuickHero("Ilfaeril", "Fighter", "longsword"));
            GameFlags.Current.SetInt("companion.ilfaeril.approval", 40);

            Enter(new Vector2Int(3, 7));

            Debug.Log("[IlfaerilQuestDemo] Examine the reliquary, hear the Pale Cantor, then 'deny the Choir the relic' " +
                      "to skip to the moral choice. The [Insight] gambit needs WIS — open P to watch Ilfaeril's approval move.");
        }

        private void Enter(Vector2Int entry)
        {
            var root = new GameObject("IlfaerilQuestMode");
            var scene = root.AddComponent<PersonalQuestScene>();
            scene.leaderSheet = _hero;
            scene.quest = _quest.Quest;
            scene.entryCoord = entry;
            scene.onStartFight = (id, ret) =>
            {
                GameFlags.Current.SetBool(_quest.Quest.clearedFlag, true);
                Destroy(root);
                Enter(ret);
                Debug.Log("[IlfaerilQuestDemo] (Battle skipped for the preview.) The Choir is scattered — the reliquary opens.");
            };
            scene.onLeave = () =>
            {
                Destroy(root);
                Debug.Log("[IlfaerilQuestDemo] Quest resolved. Ilfaeril approval is now " +
                          GameFlags.Current.GetInt("companion.ilfaeril.approval") + ".");
            };
            scene.Begin();
        }

        private CharacterSheet QuickHero(string name, string className, string abilityId)
        {
            var cls = _core.Classes.Find(c => c.className == className);
            var s = new CharacterSheet { displayName = name, classDef = cls, raceDef = _core.Races[0], level = 5, baseArmorClass = 14 };
            s.abilities.Set(Ability.Wisdom, 16);
            s.abilities.Set(Ability.Dexterity, 14);
            s.abilities.Set(Ability.Constitution, 14);
            s.abilities.Set(Ability.Intelligence, 12);
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
