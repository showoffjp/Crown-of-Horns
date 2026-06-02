using UnityEngine;
using SunderedCrown.Characters;
using SunderedCrown.Combat;
using SunderedCrown.Grid;
using SunderedCrown.Stats;
using SunderedCrown.UI;

namespace SunderedCrown.Core
{
    /// <summary>
    /// ONE-CLICK PLAYABLE DEMO. Put this on an empty GameObject in a fresh scene and
    /// press Play. It builds a grid, spawns a party (fighter, wizard, cleric) and
    /// enemies as colored cubes, wires camera + HUD, and starts a turn-based fight
    /// that now showcases spell slots, AoE saving-throw spells, healing, and status
    /// conditions — with zero prefabs or data assets.
    ///
    /// Controls: 1..9 arm an ability · click a tile to move · click a valid target to
    /// use the armed ability · Space ends the turn.
    /// </summary>
    public class DemoBootstrap : MonoBehaviour
    {
        void Start()
        {
            // --- Grid ---
            var gridGO = new GameObject("GridSystem");
            var grid = gridGO.AddComponent<GridSystem>();
            grid.width = 16; grid.height = 16;
            grid.tileWidth = 1f; grid.tileHeight = 0.5f;
            grid.Build();

            // --- Managers ---
            var combatGO = new GameObject("CombatManager");
            combatGO.AddComponent<TurnManager>();
            combatGO.AddComponent<EncounterController>();
            combatGO.AddComponent<CombatHUD>();   // real uGUI HUD (swap to DebugCombatHUD for IMGUI)
            var input = combatGO.AddComponent<PlayerCombatInput>();

            // --- Camera ---
            var cam = Camera.main;
            if (cam == null)
            {
                var camGO = new GameObject("Main Camera");
                camGO.tag = "MainCamera";
                cam = camGO.AddComponent<Camera>();
            }
            cam.orthographic = true;
            cam.orthographicSize = 7f;
            cam.transform.position = grid.GridToWorld(8, 8) + new Vector3(0, 0, -10);
            if (cam.GetComponent<SunderedCrown.CameraRig.IsometricCameraController>() == null)
                cam.gameObject.AddComponent<SunderedCrown.CameraRig.IsometricCameraController>();
            input.worldCamera = cam;

            // --- Status effects (built in code for the demo) ---
            var poisoned = ScriptableObject.CreateInstance<StatusEffectDefinition>();
            poisoned.effectName = "Poisoned"; poisoned.condition = Condition.Poisoned;
            poisoned.durationRounds = 3; poisoned.bearerAttacksDisadvantage = true;

            var burning = ScriptableObject.CreateInstance<StatusEffectDefinition>();
            burning.effectName = "Burning"; burning.condition = Condition.Burning;
            burning.durationRounds = 2; burning.damageOverTimeDice = "1d6";
            burning.damageOverTimeType = DamageType.Fire;

            // --- Abilities ---
            var longsword = MakeAbility("Longsword", "1d8", DamageType.Slashing, range: 1);
            var mace      = MakeAbility("Mace", "1d6", DamageType.Bludgeoning, range: 1);

            var firebolt  = MakeAbility("Firebolt", "1d10", DamageType.Fire, range: 12);
            firebolt.description = "Cantrip. Ranged spell attack.";

            var fireball  = MakeAbility("Fireball", "8d6", DamageType.Fire, range: 10);
            fireball.isAttackRoll = false; fireball.saveAbility = Ability.Dexterity;
            fireball.saveForHalf = true; fireball.spellSlotLevel = 3;
            fireball.targeting = TargetingMode.AreaBurst; fireball.areaRadiusTiles = 3;
            fireball.addAbilityModToDamage = false;
            fireball.appliedEffect = burning; fireball.appliedEffectRounds = 2;

            var healingWord = MakeAbility("Healing Word", "1d4", DamageType.Radiant, range: 6);
            healingWord.isHeal = true; healingWord.healDice = "1d4"; healingWord.cost = ActionCost.BonusAction;
            healingWord.spellSlotLevel = 1; healingWord.targeting = TargetingMode.SingleAlly;

            var venomClaw = MakeAbility("Venom Claw", "1d6", DamageType.Piercing, range: 1);
            venomClaw.appliedEffect = poisoned; venomClaw.appliedEffectRounds = 3;

            var claw = MakeAbility("Claw", "1d8", DamageType.Slashing, range: 1);

            // --- Class/race scaffolding so HP math works ---
            var fighter = MakeClass("Fighter", 10, Ability.Strength);
            var wizard  = MakeClass("Wizard", 6, Ability.Intelligence, caster: true, Ability.Intelligence);
            var cleric  = MakeClass("Cleric", 8, Ability.Wisdom, caster: true, Ability.Wisdom);
            var monster = MakeClass("Monster", 8, Ability.Strength);
            var human   = MakeRace("Human", 6);

            // --- Party (blue/cyan) ---
            var sable = Spawn("Sable", Faction.Player, new Vector2Int(2, 6), Color.blue, grid, fighter, human, 16, 14);
            sable.Sheet.knownAbilities.Add(longsword); sable.Sheet.equippedWeaponAbility = longsword;

            var lyra = Spawn("Lyra", Faction.Player, new Vector2Int(2, 8), new Color(0.4f,0.4f,1f), grid, wizard, human, 9, 12);
            lyra.Sheet.knownAbilities.Add(firebolt);   // index 0 = default armed
            lyra.Sheet.knownAbilities.Add(fireball);   // press 2 to arm
            lyra.Sheet.abilities.Set(Ability.Intelligence, 17);
            lyra.Sheet.spellSlots.max[3] = 2;

            var oke = Spawn("Brother Oke", Faction.Ally, new Vector2Int(3, 7), Color.cyan, grid, cleric, human, 12, 14);
            oke.Sheet.knownAbilities.Add(mace);
            oke.Sheet.knownAbilities.Add(healingWord); // press 2 to arm
            oke.Sheet.equippedWeaponAbility = mace;
            oke.Sheet.abilities.Set(Ability.Wisdom, 16);
            oke.Sheet.spellSlots.max[1] = 3;

            // --- Enemies (red) ---
            var fiendA = Spawn("Ashfiend A", Faction.Enemy, new Vector2Int(12, 6), Color.red, grid, monster, human, 14, 13);
            fiendA.Sheet.knownAbilities.Add(venomClaw); fiendA.Sheet.equippedWeaponAbility = venomClaw; fiendA.Sheet.experienceValue = 100;

            var fiendB = Spawn("Ashfiend B", Faction.Enemy, new Vector2Int(13, 9), Color.red, grid, monster, human, 14, 13);
            fiendB.Sheet.knownAbilities.Add(claw); fiendB.Sheet.equippedWeaponAbility = claw; fiendB.Sheet.experienceValue = 100;

            var fiendC = Spawn("Cinder-Hound", Faction.Enemy, new Vector2Int(11, 10), new Color(0.8f,0.2f,0.1f), grid, monster, human, 13, 11);
            fiendC.Sheet.knownAbilities.Add(claw); fiendC.Sheet.equippedWeaponAbility = claw; fiendC.Sheet.experienceValue = 75;

            Debug.Log("[Demo] Skirmish ready. 1/2 = arm ability, click to move/target, Space ends turn. " +
                      "Try Lyra: press 2 to arm Fireball, click an enemy cluster.");
        }

        // ---- tiny code-only asset factories (replace with real assets later) ----

        private AbilityDefinition MakeAbility(string name, string dice, DamageType type, int range)
        {
            var a = ScriptableObject.CreateInstance<AbilityDefinition>();
            a.abilityName = name; a.damageDice = dice; a.damageType = type; a.rangeTiles = range;
            return a;
        }

        private ClassDefinition MakeClass(string name, int hitDie, Ability primary, bool caster = false, Ability castStat = Ability.Intelligence)
        {
            var c = ScriptableObject.CreateInstance<ClassDefinition>();
            c.className = name; c.hitDie = hitDie; c.primaryAbility = primary;
            c.isSpellcaster = caster; c.spellcastingAbility = castStat;
            return c;
        }

        private RaceDefinition MakeRace(string name, int speed)
        {
            var r = ScriptableObject.CreateInstance<RaceDefinition>();
            r.raceName = name; r.baseSpeedTiles = speed;
            return r;
        }

        private GridUnit Spawn(string name, Faction faction, Vector2Int coord, Color color,
            GridSystem grid, ClassDefinition cls, RaceDefinition race, int str, int con)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = name;
            go.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);
            go.GetComponent<Renderer>().material.color = color;

            var unit = go.AddComponent<GridUnit>();
            unit.faction = faction;
            unit.startCoord = coord;

            var sheet = new CharacterSheet
            {
                displayName = name, classDef = cls, raceDef = race, level = 3, baseArmorClass = 13
            };
            sheet.abilities.Set(Ability.Strength, str);
            sheet.abilities.Set(Ability.Dexterity, 13);
            sheet.abilities.Set(Ability.Constitution, con);
            sheet.RecalculateMaxHitPoints();
            unit.Sheet = sheet;
            return unit;
        }
    }
}
