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
    /// ONE-CLICK NETHERIL DEMO. Drop this on an empty GameObject and press Play to jump straight
    /// into the falling-city battle — an arcane war-construct fight on a Netherese enclave where
    /// THE FLOOR COLLAPSES each turn (the FallingHazard). Showcases the era's signature mechanic and
    /// Naeve's spell kit without playing the campaign. For the full arrival → recruit → battle loop,
    /// use CampaignBootstrap and step through the rift after clearing the Cinderhaunt.
    /// </summary>
    public class NetherilDemo : MonoBehaviour
    {
        void Start()
        {
            if (GameManager.Instance == null) new GameObject("GameManager").AddComponent<GameManager>();
            if (Party.Instance == null) new GameObject("Party").AddComponent<Party>();

            var core = new SwordCoastContent();
            var neth = new NetherilContent(core);

            // A small party: a wizard hero, Garrow, and Naeve (era companion).
            Party.Instance.Recruit(QuickWizard("The Returned", core, neth));
            Party.Instance.Recruit(QuickGarrow(core));
            Party.Instance.Recruit(neth.BuildNaeve());

            var root = new GameObject("NetherilFight");
            var enc = root.AddComponent<EncounterBuilder>();
            enc.environmentalHazard = true; // the falling city
            enc.gridWidth = 16; enc.gridHeight = 14;
            enc.combatants = BuildCombatants(core, neth);
            enc.onEnded = win => Debug.Log(win
                ? "[NetherilDemo] You held the line as the city fell. Naeve lives."
                : "[NetherilDemo] The enclave took you with it.");
            enc.Begin();

            Debug.Log("[NetherilDemo] Karsus's Folly. Arm spells (1/2), fight the constructs — and stay off the " +
                      "tiles that fall away. The floor collapses from the edges inward after a few turns.");
        }

        private List<Combatant> BuildCombatants(SwordCoastContent core, NetherilContent neth)
        {
            var list = new List<Combatant>();
            var coords = new[] { new Vector2Int(2, 4), new Vector2Int(2, 6), new Vector2Int(2, 8) };
            var act = Party.Instance.active;
            for (int i = 0; i < act.Count && i < coords.Length; i++)
                list.Add(new Combatant { sheet = act[i], coord = coords[i], color = i == 0 ? Color.blue : Color.cyan, faction = i == 0 ? Faction.Player : Faction.Ally });

            var fighter = core.Classes.Find(c => c.className == "Fighter");
            list.Add(new Combatant { sheet = Enemy("Netherese War-Construct", fighter, 17, 18, 5, neth.Abilities["arcane_slam"], 300), coord = new Vector2Int(12, 5), color = new Color(0.8f, 0.7f, 0.3f), faction = Faction.Enemy });
            list.Add(new Combatant { sheet = Enemy("Shadow-Bound Sentinel", fighter, 14, 13, 3, neth.Abilities["voidlash"], 120), coord = new Vector2Int(13, 8), color = new Color(0.4f, 0.3f, 0.55f), faction = Faction.Enemy });
            return list;
        }

        private CharacterSheet QuickWizard(string name, SwordCoastContent core, NetherilContent neth)
        {
            var wizard = core.Classes.Find(c => c.className == "Wizard");
            var s = new CharacterSheet { displayName = name, classDef = wizard, raceDef = core.Races[0], level = 4, baseArmorClass = 12 };
            s.abilities.Set(Ability.Intelligence, 17);
            s.abilities.Set(Ability.Dexterity, 14);
            s.abilities.Set(Ability.Constitution, 13);
            s.RecalculateMaxHitPoints();
            s.knownAbilities.Add(neth.Abilities["arcane_bolt"]);
            s.knownAbilities.Add(neth.Abilities["shatterglass"]);
            s.equippedWeaponAbility = neth.Abilities["arcane_bolt"];
            s.spellSlots.max[2] = 3;
            return s;
        }

        private CharacterSheet QuickGarrow(SwordCoastContent core)
        {
            var cleric = core.Classes.Find(c => c.className == "Cleric");
            var s = new CharacterSheet { displayName = "Sister Garrow", classDef = cleric, raceDef = core.Races[0], level = 3, baseArmorClass = 15 };
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

        private CharacterSheet Enemy(string name, ClassDefinition cls, int str, int con, int level, AbilityDefinition atk, int xp)
        {
            var s = new CharacterSheet { displayName = name, classDef = cls, raceDef = SwordCoastRace(), level = level, baseArmorClass = 14, experienceValue = xp };
            s.abilities.Set(Ability.Strength, str);
            s.abilities.Set(Ability.Dexterity, 12);
            s.abilities.Set(Ability.Constitution, con);
            s.RecalculateMaxHitPoints();
            s.knownAbilities.Add(atk);
            s.equippedWeaponAbility = atk;
            s.lootGold = Mathf.Max(0, xp / 4);
            return s;
        }

        private RaceDefinition _race;
        private RaceDefinition SwordCoastRace()
        {
            if (_race == null) { _race = ScriptableObject.CreateInstance<RaceDefinition>(); _race.raceName = "Construct"; _race.baseSpeedTiles = 5; }
            return _race;
        }
    }
}
