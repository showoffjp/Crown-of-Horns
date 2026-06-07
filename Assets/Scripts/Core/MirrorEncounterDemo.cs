using System.Collections.Generic;
using UnityEngine;
using SunderedCrown.Characters;
using SunderedCrown.Combat;
using SunderedCrown.Content;
using SunderedCrown.Grid;
using SunderedCrown.Stats;

namespace SunderedCrown.Core
{
    /// <summary>
    /// ONE-CLICK MIRROR DEMO. Drop on an empty GameObject and press Play to fight **the Echoes** —
    /// twisted copies of your own party — and **the Last Returned**, your own aged reflection. The
    /// Echoes use your party's kits against you; the Last Returned cannot be killed by damage (it's
    /// you), but it *kneels* once the Echoes fall (see MirrorResolver). The emotional centrepiece of
    /// the late game, in miniature.
    /// </summary>
    public class MirrorEncounterDemo : MonoBehaviour
    {
        void Start()
        {
            if (GameManager.Instance == null) new GameObject("GameManager").AddComponent<GameManager>();
            if (Party.Instance == null) new GameObject("Party").AddComponent<Party>();

            var core = new SwordCoastContent();

            // A small party to mirror.
            var hero = QuickHero(core);
            Party.Instance.Recruit(hero);
            Party.Instance.Recruit(QuickGarrow(core));

            var root = new GameObject("MirrorFight");
            var enc = root.AddComponent<EncounterBuilder>();
            enc.mirrorMode = true; // the Last Returned yields once its Echoes fall
            enc.gridWidth = 16; enc.gridHeight = 12;
            enc.combatants = BuildCombatants();
            enc.onEnded = win => Debug.Log(win
                ? "[MirrorDemo] You did not defeat yourself. You refused to. That was the only way."
                : "[MirrorDemo] You lost to your own reflection. As it always feared you would.");
            enc.Begin();

            Debug.Log("[MirrorDemo] Fight the Echoes (dark copies of your party). The Last Returned (centre) " +
                      "cannot be killed by damage — defeat the Echoes and it will kneel.");
        }

        private List<Combatant> BuildCombatants()
        {
            var list = new List<Combatant>();
            var act = Party.Instance.active;

            // Your party (left).
            var pcoords = new[] { new Vector2Int(2, 4), new Vector2Int(2, 6), new Vector2Int(2, 8) };
            for (int i = 0; i < act.Count && i < pcoords.Length; i++)
                list.Add(new Combatant { sheet = act[i], coord = pcoords[i], color = i == 0 ? Color.blue : Color.cyan, faction = i == 0 ? Faction.Player : Faction.Ally });

            // The Echoes — clones of your party, twisted (right).
            var dark = new Color(0.35f, 0.1f, 0.12f);
            var ecoords = new[] { new Vector2Int(12, 4), new Vector2Int(12, 8) };
            for (int i = 0; i < act.Count && i < ecoords.Length; i++)
            {
                var echo = act[i].Clone();
                echo.displayName = $"Echo of {act[i].displayName}";
                echo.experienceValue = 150;
                list.Add(new Combatant { sheet = echo, coord = ecoords[i], color = dark, faction = Faction.Enemy });
            }

            // The Last Returned — your own aged reflection (centre-right). Cannot be killed by damage.
            var lr = act[0].Clone();
            lr.displayName = "The Last Returned";
            lr.maxHitPoints += 60; lr.currentHitPoints = lr.maxHitPoints;
            lr.experienceValue = 0;
            list.Add(new Combatant { sheet = lr, coord = new Vector2Int(13, 6), color = new Color(0.55f, 0.55f, 0.6f), faction = Faction.Enemy });

            return list;
        }

        private CharacterSheet QuickHero(SwordCoastContent core)
        {
            var fighter = core.Classes.Find(c => c.className == "Fighter");
            var s = new CharacterSheet { displayName = "The Returned", classDef = fighter, raceDef = core.Races[0], level = 5, baseArmorClass = 15 };
            s.abilities.Set(Ability.Strength, 16);
            s.abilities.Set(Ability.Dexterity, 14);
            s.abilities.Set(Ability.Constitution, 15);
            s.RecalculateMaxHitPoints();
            s.knownAbilities.Add(core.Abilities["longsword"]);
            s.equippedWeaponAbility = core.Abilities["longsword"];
            return s;
        }

        private CharacterSheet QuickGarrow(SwordCoastContent core)
        {
            var cleric = core.Classes.Find(c => c.className == "Cleric");
            var s = new CharacterSheet { displayName = "Sister Garrow", classDef = cleric, raceDef = core.Races[0], level = 4, baseArmorClass = 16 };
            s.abilities.Set(Ability.Wisdom, 16);
            s.abilities.Set(Ability.Constitution, 14);
            s.abilities.Set(Ability.Strength, 12);
            s.RecalculateMaxHitPoints();
            s.knownAbilities.Add(core.Abilities["mace"]);
            s.knownAbilities.Add(core.Abilities["cure_wounds"]);
            s.equippedWeaponAbility = core.Abilities["mace"];
            s.spellSlots.max[1] = 3;
            return s;
        }
    }
}
