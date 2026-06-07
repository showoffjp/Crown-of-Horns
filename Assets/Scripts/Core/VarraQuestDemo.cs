using UnityEngine;
using SunderedCrown.Characters;
using SunderedCrown.Content;
using SunderedCrown.Stats;

namespace SunderedCrown.Core
{
    /// <summary>
    /// ONE-CLICK PREVIEW of Varra's personal quest, "The Bill Comes Due." Drop on an empty GameObject
    /// and press Play: a deconsecrated chapel, the cambion broker Quill who sold her at six, and a moral
    /// trilemma about who pays the contract (carry it for her / let her burn it freely / an Arcana
    /// loophole that binds her patron instead). The battle is skipped for the tour; the choice is real.
    /// </summary>
    public class VarraQuestDemo : MonoBehaviour
    {
        private SwordCoastContent _core;
        private VarraQuestContent _quest;
        private CharacterSheet _hero;

        void Start()
        {
            if (GameManager.Instance == null) new GameObject("GameManager").AddComponent<GameManager>();
            if (Party.Instance == null) new GameObject("Party").AddComponent<Party>();

            _core = new SwordCoastContent();
            _quest = new VarraQuestContent();

            _hero = QuickHero("The Returned", "Wizard", "firebolt");
            Party.Instance.Recruit(_hero);
            Party.Instance.Recruit(QuickHero("Varra", "Wizard", "firebolt"));
            GameFlags.Current.SetInt("companion.varra.approval", 40);

            Enter(new Vector2Int(3, 7));

            Debug.Log("[VarraQuestDemo] Examine the chapel, confront Quill, then 'break the collection' to skip to " +
                      "the moral choice. The Arcana loophole needs INT — open P to watch Varra's approval move.");
        }

        private void Enter(Vector2Int entry)
        {
            var root = new GameObject("VarraQuestMode");
            var scene = root.AddComponent<PersonalQuestScene>();
            scene.leaderSheet = _hero;
            scene.quest = _quest.Quest;
            scene.entryCoord = entry;
            scene.onStartFight = (id, ret) =>
            {
                GameFlags.Current.SetBool(_quest.Quest.clearedFlag, true);
                Destroy(root);
                Enter(ret);
                Debug.Log("[VarraQuestDemo] (Battle skipped for the preview.) Quill is down — decide how the contract ends.");
            };
            scene.onLeave = () =>
            {
                Destroy(root);
                Debug.Log("[VarraQuestDemo] Quest resolved. Varra approval is now " +
                          GameFlags.Current.GetInt("companion.varra.approval") + ".");
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
