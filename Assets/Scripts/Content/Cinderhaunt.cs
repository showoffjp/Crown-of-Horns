using System.Collections.Generic;
using UnityEngine;
using SunderedCrown.Characters;
using SunderedCrown.Core;
using SunderedCrown.Dialogue;
using SunderedCrown.Grid;
using SunderedCrown.Items;
using SunderedCrown.UI;
using SunderedCrown.World;

namespace SunderedCrown.Content
{
    /// <summary>
    /// The Cinderhaunt — a two-room dungeon beneath Baldur's Gate, explorable in real time.
    /// An outer harvest-chamber (a guard fight + loot) is split from the inner sanctum (the
    /// boss) by an iron door that needs the Cinderhaunt Key. Progress — cleared fights, opened
    /// door, looted chests — is tracked in GameFlags, so the dungeon survives the round-trips
    /// to the turn-based combat scene and back.
    ///
    /// The director sets the callbacks and calls Begin().
    /// </summary>
    public class Cinderhaunt : MonoBehaviour
    {
        public SwordCoastContent content;
        public CharacterSheet leaderSheet;
        public Vector2Int entryCoord = new Vector2Int(2, 7);

        public System.Action<string, Vector2Int> onStartFight; // (encounterId, returnCoord)
        public System.Action onLeaveDungeon;                    // back to hub
        public System.Action<Vector2Int> onReenter;             // rebuild this dungeon at coord

        private const int W = 22, H = 14;
        private const int DoorX = 11, DoorY = 7;

        public void Begin()
        {
            var f = GameFlags.Current;
            bool doorOpen = f.GetBool("dungeon.cinder.door");
            bool hasKey = doorOpen || (Party.Instance != null && Party.Instance.inventory.Has("cinderhaunt_key"));

            // Grid + walls.
            var gridGO = new GameObject("DungeonGrid");
            gridGO.transform.SetParent(transform);
            var grid = gridGO.AddComponent<GridSystem>();
            grid.width = W; grid.height = H; grid.tileWidth = 1f; grid.tileHeight = 0.5f;
            grid.Build();
            BuildWalls(grid, doorOpen);
            gridGO.AddComponent<SunderedCrown.Rendering.TileFloorRenderer>();

            // Camera.
            var cam = Camera.main;
            if (cam == null) { var g = new GameObject("Main Camera"); g.tag = "MainCamera"; cam = g.AddComponent<Camera>(); }
            cam.orthographic = true; cam.orthographicSize = 8f;
            cam.transform.position = grid.GridToWorld(W / 2, H / 2) + new Vector3(0, 0, -10);
            if (cam.GetComponent<SunderedCrown.CameraRig.IsometricCameraController>() == null)
                cam.gameObject.AddComponent<SunderedCrown.CameraRig.IsometricCameraController>();

            // UI + exploration.
            var dlg = gameObject.AddComponent<DialogueScreen>();
            dlg.playerSpeaker = leaderSheet;
            gameObject.AddComponent<ExplorationHUD>();
            gameObject.AddComponent<InventoryScreen>();
            var explore = gameObject.AddComponent<ExplorationController>();
            explore.worldCamera = cam;
            explore.Leader = SpawnLeader(grid, entryCoord);

            // Stairs back up to Baldur's Gate.
            MakeExit(grid, "Stairs Up", new Vector2Int(2, 2), () => onLeaveDungeon?.Invoke());

            // Outer chamber loot.
            if (!f.GetBool("dungeon.cinder.chest1"))
                MakeContainer(grid, "Ash-Caked Chest", new Vector2Int(4, 4), 15,
                    new List<ItemDefinition> { content.Items["healing_potion"] }, "dungeon.cinder.chest1");

            // Outer chamber guards (or their aftermath).
            if (!f.GetBool("dungeon.cinder.guards"))
                MakeExit(grid, "Harvest Brazier", new Vector2Int(8, 5),
                    () => onStartFight?.Invoke("guards", new Vector2Int(7, 5)));
            else
                MakeExamine(grid, "Cold Brazier", new Vector2Int(8, 5),
                    "The brazier is cold ash now. The Doomguides lie where they fell.");

            // The iron door at the wall gap.
            if (doorOpen)
            {
                // passage left open by BuildWalls; a flavor marker beside it
                MakeExamine(grid, "Forced Iron Door", new Vector2Int(DoorX, 4),
                    "The Doomguide door, torn from its sigils. Cold air breathes from the sanctum beyond.");
            }
            else if (hasKey)
            {
                MakeExit(grid, "Iron Door (use Cinderhaunt Key)", new Vector2Int(DoorX, DoorY), () =>
                {
                    GameFlags.Current.SetBool("dungeon.cinder.door", true);
                    onReenter?.Invoke(new Vector2Int(DoorX + 1, DoorY)); // step through
                });
            }
            else
            {
                MakeExamine(grid, "Iron Door (locked)", new Vector2Int(DoorX, DoorY),
                    "Sealed with Doomguide death-sigils. You need the Cinderhaunt Key — the Herald said the " +
                    "Flaming Fist kept one in their strongbox above.");
            }

            // Inner sanctum (only reachable once the door is open).
            if (!f.GetBool("dungeon.cinder.chest2"))
                MakeContainer(grid, "Reliquary", new Vector2Int(18, 3), 40,
                    new List<ItemDefinition> { content.Items["chain_shirt"] }, "dungeon.cinder.chest2");

            if (!f.GetBool("dungeon.cinder.boss"))
                MakeExit(grid, "The Unbound Maw", new Vector2Int(18, 7),
                    () => onStartFight?.Invoke("boss", new Vector2Int(15, 7)));
            else
                MakeExamine(grid, "The Stilled Maw", new Vector2Int(18, 7),
                    "Where the harvest fed. Silent now. The road to Aldric lies open.");
        }

        // ---- layout ----

        private void BuildWalls(GridSystem grid, bool doorOpen)
        {
            for (int x = 0; x < W; x++)
                for (int y = 0; y < H; y++)
                {
                    bool border = x == 0 || y == 0 || x == W - 1 || y == H - 1;
                    bool dividing = x == DoorX;
                    var cell = grid.GetCell(x, y);
                    if (cell != null && (border || dividing)) cell.walkable = false;
                }
            // Open the doorway when unlocked.
            if (doorOpen) { var d = grid.GetCell(DoorX, DoorY); if (d != null) d.walkable = true; }
        }

        // ---- spawn helpers ----

        private GridUnit SpawnLeader(GridSystem grid, Vector2Int coord)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = leaderSheet.displayName;
            go.transform.SetParent(transform);
            go.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);
            go.GetComponent<Renderer>().material.color = Color.blue;
            var unit = go.AddComponent<GridUnit>();
            unit.faction = Faction.Player;
            unit.startCoord = coord;
            unit.Sheet = leaderSheet;
            return unit;
        }

        private Interactable MakeMarker(GridSystem grid, string name, Vector2Int coord, Color color)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = name;
            go.transform.SetParent(transform);
            go.transform.localScale = new Vector3(0.6f, 0.8f, 0.6f);
            go.GetComponent<Renderer>().material.color = color;
            var it = go.AddComponent<Interactable>();
            it.label = name; it.coord = coord;
            it.Place(grid);
            return it;
        }

        private void MakeExit(GridSystem grid, string name, Vector2Int coord, System.Action onExit)
        {
            var it = MakeMarker(grid, name, coord, new Color(0.3f, 0.1f, 0.4f));
            it.kind = InteractionKind.Exit; it.onExit = onExit;
        }

        private void MakeExamine(GridSystem grid, string name, Vector2Int coord, string text)
        {
            var it = MakeMarker(grid, name, coord, new Color(0.5f, 0.5f, 0.55f));
            it.kind = InteractionKind.Examine; it.examineText = text;
        }

        private void MakeContainer(GridSystem grid, string name, Vector2Int coord, int gold,
            List<ItemDefinition> items, string lootFlag)
        {
            var it = MakeMarker(grid, name, coord, new Color(0.6f, 0.45f, 0.2f));
            it.kind = InteractionKind.Container; it.gold = gold; it.contents = items; it.lootFlag = lootFlag;
        }
    }
}
