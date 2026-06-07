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
    /// ONE-CLICK SPELLPLAGUE DEMO. Drop on an empty GameObject and press Play to fight in the wound
    /// where the Weave shattered — a **causality-optional** battle where reality skips (units swap
    /// places) and blue fire lashes out each turn. Your positioning is never quite safe. Showcases the
    /// `SpellplagueHazard`.
    /// </summary>
    public class SpellplagueDemo : MonoBehaviour
    {
        private SwordCoastContent _core;
        private LateEraContent _late;

        void Start()
        {
            if (GameManager.Instance == null) new GameObject("GameManager").AddComponent<GameManager>();
            if (Party.Instance == null) new GameObject("Party").AddComponent<Party>();

            _core = new SwordCoastContent();
            _late = new LateEraContent();

            Party.Instance.Recruit(Quick("The Returned", "Fighter", "longsword"));
            Party.Instance.Recruit(Quick("Naeve", "Wizard", "fireball"));
            Party.Instance.Recruit(Quick("Sister Garrow", "Cleric", "mace"));

            var root = new GameObject("SpellplagueFight");
            var enc = root.AddComponent<EncounterBuilder>();
            enc.spellplagueHazard = true;
            enc.gridWidth = 16; enc.gridHeight = 14;
            enc.combatants = BuildCombatants();
            enc.onEnded = win => Debug.Log(win
                ? "[SpellplagueDemo] You held 'now' together long enough to win."
                : "[SpellplagueDemo] The blue fire unwrote you.");
            enc.Begin();

            Debug.Log("[SpellplagueDemo] Causality is optional here — units swap places and blue fire lashes each " +
                      "turn after the second. Adapt fast.");
        }

        private List<Combatant> BuildCombatants()
        {
            var list = new List<Combatant>();
            var coords = new[] { new Vector2Int(2, 4), new Vector2Int(2, 6), new Vector2Int(2, 8) };
            var act = Party.Instance.active;
            for (int i = 0; i < act.Count && i < coords.Length; i++)
                list.Add(new Combatant { sheet = act[i], coord = coords[i], color = i == 0 ? Color.blue : Color.cyan, faction = i == 0 ? Faction.Player : Faction.Ally });

            var fighter = _core.Classes.Find(c => c.className == "Fighter");
            list.Add(new Combatant { sheet = Enemy("Spellplague Aberration", fighter, 16, 16, 5, _late.BlueFire, 280), coord = new Vector2Int(12, 5), color = new Color(0.3f, 0.5f, 0.9f), faction = Faction.Enemy });
            list.Add(new Combatant { sheet = Enemy("Spellplague Aberration", fighter, 14, 13, 4, _late.BlueFire, 160), coord = new Vector2Int(13, 8), color = new Color(0.3f, 0.5f, 0.9f), faction = Faction.Enemy });
            return list;
        }

        private CharacterSheet Quick(string name, string className, string abilityId)
        {
            var cls = _core.Classes.Find(c => c.className == className);
            var s = new CharacterSheet { displayName = name, classDef = cls, raceDef = _core.Races[0], level = 5, baseArmorClass = 14 };
            s.abilities.Set(Ability.Strength, 15); s.abilities.Set(Ability.Dexterity, 14);
            s.abilities.Set(Ability.Constitution, 14); s.abilities.Set(Ability.Intelligence, 16); s.abilities.Set(Ability.Wisdom, 15);
            s.RecalculateMaxHitPoints();
            if (_core.Abilities.ContainsKey(abilityId)) { s.knownAbilities.Add(_core.Abilities[abilityId]); s.equippedWeaponAbility = _core.Abilities[abilityId]; }
            if (className == "Wizard") s.spellSlots.max[3] = 2;
            return s;
        }

        private CharacterSheet Enemy(string name, ClassDefinition cls, int str, int con, int level, AbilityDefinition atk, int xp)
        {
            var s = new CharacterSheet { displayName = name, classDef = cls, raceDef = _core.Races[0], level = level, baseArmorClass = 14, experienceValue = xp };
            s.abilities.Set(Ability.Strength, str); s.abilities.Set(Ability.Dexterity, 13); s.abilities.Set(Ability.Constitution, con);
            s.RecalculateMaxHitPoints();
            s.knownAbilities.Add(atk); s.equippedWeaponAbility = atk;
            s.lootGold = Mathf.Max(0, xp / 4);
            return s;
        }
    }
}
