using UnityEngine;
using SunderedCrown.Characters;
using SunderedCrown.Content;
using SunderedCrown.Stats;

namespace SunderedCrown.Core
{
    /// <summary>
    /// ONE-CLICK PREVIEW of Roen's personal quest, "The Honest Lie." Drop on an empty GameObject and
    /// press Play: arrive at the silent safehouse, corner Wrenna for the reveal, then make the moral
    /// call (forgive / condemn / a Persuasion gambit) and watch Roen's approval move. For a fast tour
    /// the demo skips the Doomguide battle (the full campaign runs it); the choice and its flags are real.
    /// </summary>
    public class RoenQuestDemo : MonoBehaviour
    {
        private SwordCoastContent _core;
        private RoenQuestContent _quest;
        private CharacterSheet _hero;

        void Start()
        {
            if (GameManager.Instance == null) new GameObject("GameManager").AddComponent<GameManager>();
            if (Party.Instance == null) new GameObject("Party").AddComponent<Party>();

            _core = new SwordCoastContent();
            _quest = new RoenQuestContent();

            _hero = QuickHero("The Returned", "Fighter", "longsword");
            Party.Instance.Recruit(_hero);
            Party.Instance.Recruit(QuickHero("Roen Alleywind", "Rogue", "dagger"));
            GameFlags.Current.SetInt("companion.roen.approval", 40);

            Enter(new Vector2Int(3, 7));

            Debug.Log("[RoenQuestDemo] Examine the safehouse, talk to Wrenna, then 'cut down the cell' to skip to " +
                      "the moral choice. Open P to watch Roen's approval shift with your call.");
        }

        private void Enter(Vector2Int entry)
        {
            var root = new GameObject("RoenQuestMode");
            var scene = root.AddComponent<PersonalQuestScene>();
            scene.leaderSheet = _hero;
            scene.quest = _quest.Quest;
            scene.entryCoord = entry;
            // Fast path: skip the battle so the preview reaches the choice quickly.
            scene.onStartFight = (id, ret) =>
            {
                GameFlags.Current.SetBool("roen.quest.cleared", true);
                Destroy(root);
                Enter(ret);
                Debug.Log("[RoenQuestDemo] (Battle skipped for the preview.) The cell is down — make the call.");
            };
            scene.onLeave = () =>
            {
                Destroy(root);
                Debug.Log("[RoenQuestDemo] Quest resolved. Roen approval is now " +
                          GameFlags.Current.GetInt("companion.roen.approval") + ". Check the ending epilogue (EndingDemo).");
            };
            scene.Begin();
        }

        private CharacterSheet QuickHero(string name, string className, string abilityId)
        {
            var cls = _core.Classes.Find(c => c.className == className);
            var s = new CharacterSheet { displayName = name, classDef = cls, raceDef = _core.Races[0], level = 4, baseArmorClass = 15 };
            s.abilities.Set(Ability.Strength, 15);
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
