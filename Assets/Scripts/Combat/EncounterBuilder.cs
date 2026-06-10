using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SunderedCrown.Characters;
using SunderedCrown.Grid;

namespace SunderedCrown.Combat
{
    /// <summary>A single participant to place into an encounter.</summary>
    public class Combatant
    {
        public CharacterSheet sheet;
        public Vector2Int coord;
        public Color color = Color.white;
        public Faction faction = Faction.Enemy;
    }

    /// <summary>
    /// Reusable, self-contained combat scene builder. Hand it a list of Combatants and
    /// a victory/defeat callback; it constructs the grid, managers, uGUI HUD, camera,
    /// spawns everyone, and starts the turn-based fight. Everything it creates is
    /// parented under this object so a director can tear the whole encounter down with
    /// one Destroy when combat ends.
    /// </summary>
    public class EncounterBuilder : MonoBehaviour
    {
        public List<Combatant> combatants = new List<Combatant>();
        public System.Action<bool> onEnded;   // true = party victory
        public int gridWidth = 16, gridHeight = 16;
        [Tooltip("Netheril: the floor collapses each turn (the falling-city hazard).")]
        public bool environmentalHazard = false;
        [Tooltip("Spellplague: causality-optional — units swap places & blue fire lashes each turn.")]
        public bool spellplagueHazard = false;
        [Tooltip("The Mirror: the enemy named 'The Last Returned' kneels (yields) once its Echoes fall.")]
        public bool mirrorMode = false;

        private TurnManager _turns;
        private bool _reported;

        public void Begin() => StartCoroutine(Run());

        private IEnumerator Run()
        {
            // Grid.
            var gridGO = new GameObject("GridSystem");
            gridGO.transform.SetParent(transform);
            var grid = gridGO.AddComponent<GridSystem>();
            grid.width = gridWidth; grid.height = gridHeight; grid.tileWidth = 1f; grid.tileHeight = 0.5f;
            gridGO.AddComponent<SunderedCrown.Rendering.TileFloorRenderer>(); // isometric tiled floor

            // Managers + uGUI HUD.
            var cm = new GameObject("CombatManager");
            cm.transform.SetParent(transform);
            _turns = cm.AddComponent<TurnManager>();
            var enc = cm.AddComponent<EncounterController>();
            enc.autoStartOnPlay = false;
            cm.AddComponent<SunderedCrown.UI.CombatHUD>();
            var input = cm.AddComponent<PlayerCombatInput>();

            // Camera (persistent: not parented, reused across modes).
            var cam = Camera.main;
            if (cam == null) { var g = new GameObject("Main Camera"); g.tag = "MainCamera"; cam = g.AddComponent<Camera>(); }
            cam.orthographic = true; cam.orthographicSize = 7f;
            cam.transform.position = grid.GridToWorld(gridWidth / 2, gridHeight / 2) + new Vector3(0, 0, -10);
            if (cam.GetComponent<SunderedCrown.CameraRig.IsometricCameraController>() == null)
                cam.gameObject.AddComponent<SunderedCrown.CameraRig.IsometricCameraController>();
            input.worldCamera = cam;

            _turns.OnCombatEnded += HandleEnded;

            // The falling city: collapsing-floor hazard (subscribes to TurnManager before combat starts).
            if (environmentalHazard) cm.AddComponent<FallingHazard>();
            // The Spellplague: causality-optional reality skips + blue fire.
            if (spellplagueHazard) cm.AddComponent<SpellplagueHazard>();
            // The Mirror: the Last Returned yields rather than dies once the Echoes fall.
            if (mirrorMode) cm.AddComponent<MirrorResolver>();

            // Spawn everyone (so their GridUnit.Start places them this frame)...
            var units = new List<GridUnit>();
            foreach (var c in combatants) units.Add(Spawn(c, grid));

            yield return null; // ...let all Start() run (placement, HUD canvas) before combat begins.

            _turns.StartCombat(units);
        }

        private void HandleEnded()
        {
            if (_reported) return;
            _reported = true;
            _turns.OnCombatEnded -= HandleEnded; // one-shot: never linger past this battle
            bool win = false;
            foreach (var u in _turns.TurnOrder)
                if ((u.faction == Faction.Player || u.faction == Faction.Ally) && u.Sheet.IsAlive) win = true;
            onEnded?.Invoke(win);
        }

        private GridUnit Spawn(Combatant c, GridSystem grid)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = c.sheet.displayName;
            go.transform.SetParent(transform);
            go.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);
            go.GetComponent<Renderer>().material.color = c.color;
            var unit = go.AddComponent<GridUnit>();
            unit.faction = c.faction;
            unit.startCoord = c.coord;
            unit.Sheet = c.sheet;
            return unit;
        }
    }
}
