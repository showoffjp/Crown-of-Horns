using UnityEngine;
using SunderedCrown.Characters;
using SunderedCrown.Combat;
using SunderedCrown.Grid;
using SunderedCrown.Stats;
using SunderedCrown.UI;

namespace SunderedCrown.Core
{
    /// <summary>
    /// ONE-CLICK PLAYABLE DEMO. Put this single component on an empty GameObject
    /// in a fresh scene and press Play — it builds a grid, spawns a small party and
    /// some enemies as colored cubes, wires the camera + HUD, and starts a
    /// turn-based skirmish. No prefabs, no ScriptableObject assets required.
    ///
    /// It exists so you can SEE the architecture working on day one. Once you have
    /// real art and data assets, delete this and spawn from your encounter data.
    /// </summary>
    public class DemoBootstrap : MonoBehaviour
    {
        void Start()
        {
            // --- Grid ---
            var gridGO = new GameObject("GridSystem");
            var grid = gridGO.AddComponent<GridSystem>();
            grid.width = 14; grid.height = 14;
            grid.tileWidth = 1f; grid.tileHeight = 0.5f;
            grid.Build();

            // --- Managers ---
            var combatGO = new GameObject("CombatManager");
            combatGO.AddComponent<TurnManager>();
            combatGO.AddComponent<EncounterController>();
            combatGO.AddComponent<DebugCombatHUD>();
            var input = combatGO.AddComponent<PlayerCombatInput>();

            // --- Camera (orthographic, centered on the field) ---
            var cam = Camera.main;
            if (cam == null)
            {
                var camGO = new GameObject("Main Camera");
                camGO.tag = "MainCamera";
                cam = camGO.AddComponent<Camera>();
            }
            cam.orthographic = true;
            cam.orthographicSize = 6f;
            cam.transform.position = grid.GridToWorld(7, 7) + new Vector3(0, 0, -10);
            if (cam.GetComponent<SunderedCrown.CameraRig.IsometricCameraController>() == null)
                cam.gameObject.AddComponent<SunderedCrown.CameraRig.IsometricCameraController>();
            input.worldCamera = cam;

            // --- A shared "longsword" ability built in code for the demo ---
            var sword = ScriptableObject.CreateInstance<AbilityDefinition>();
            sword.abilityName = "Longsword"; sword.damageDice = "1d8";
            sword.damageType = DamageType.Slashing; sword.rangeTiles = 1;

            var bow = ScriptableObject.CreateInstance<AbilityDefinition>();
            bow.abilityName = "Shortbow"; bow.damageDice = "1d6";
            bow.damageType = DamageType.Piercing; bow.rangeTiles = 6;

            // Minimal class/race so HP math works.
            var fighter = ScriptableObject.CreateInstance<ClassDefinition>();
            fighter.className = "Fighter"; fighter.hitDie = 10; fighter.primaryAbility = Ability.Strength;
            var human = ScriptableObject.CreateInstance<RaceDefinition>();
            human.raceName = "Human"; human.baseSpeedTiles = 6;

            // --- Spawn party (blue) ---
            SpawnUnit("Sable",  Faction.Player, new Vector2Int(2, 3), Color.blue, grid, fighter, human, sword, 16, 15);
            SpawnUnit("Garrick", Faction.Ally,  new Vector2Int(2, 5), Color.cyan, grid, fighter, human, bow, 14, 12);

            // --- Spawn enemies (red) ---
            SpawnUnit("Ashfiend A", Faction.Enemy, new Vector2Int(10, 4), Color.red, grid, fighter, human, sword, 13, 11);
            SpawnUnit("Ashfiend B", Faction.Enemy, new Vector2Int(11, 6), Color.red, grid, fighter, human, sword, 13, 11);

            Debug.Log("[Demo] Skirmish ready. Click a tile to move, click an adjacent enemy to attack, Space to end turn.");
        }

        private void SpawnUnit(string name, Faction faction, Vector2Int coord, Color color,
            GridSystem grid, ClassDefinition cls, RaceDefinition race, AbilityDefinition atk, int str, int dexAc)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = name;
            go.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);
            var rend = go.GetComponent<Renderer>();
            rend.material.color = color;

            var unit = go.AddComponent<GridUnit>();
            unit.faction = faction;
            unit.startCoord = coord;

            var sheet = new CharacterSheet
            {
                displayName = name,
                classDef = cls,
                raceDef = race,
                level = 3,
                baseArmorClass = 14
            };
            sheet.abilities.Set(Ability.Strength, str);
            sheet.abilities.Set(Ability.Dexterity, 14);
            sheet.abilities.Set(Ability.Constitution, dexAc);
            sheet.knownAbilities.Add(atk);
            sheet.RecalculateMaxHitPoints();

            unit.Sheet = sheet;
        }
    }
}
