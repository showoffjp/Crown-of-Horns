using System.Collections.Generic;
using UnityEngine;
using SunderedCrown.Characters;
using SunderedCrown.Grid;
using SunderedCrown.Rendering;
using SunderedCrown.UI;
using SunderedCrown.World;

namespace SunderedCrown.Content
{
    /// <summary>
    /// THE VAULT OF TENS — the Lady's riddle room (better than Bodhi's). A violet chamber with ten
    /// **pedestals** (place a token to answer an object-riddle), a row of **braziers** (speak the word
    /// to answer a riddle), the locked "Your Name" brazier, and the secret **eleventh** — her own
    /// identity. Wit beats correctness; failure costs only her amusement. Place a leader, walk to a
    /// pedestal, and the RiddleVaultUI opens. On entry she hands you the pouch of tokens.
    /// </summary>
    public class RiddleVault : MonoBehaviour
    {
        public RiddleContent content;
        public CharacterSheet leaderSheet;
        public System.Action onLeave;
        public bool grantTokens = true;

        public void Begin()
        {
            // The Lady's gift: the pouch of tokens (so the vault is solvable).
            if (grantTokens && Party.Instance != null)
                foreach (var kv in content.Tokens)
                    if (!Party.Instance.inventory.Has(kv.Key)) Party.Instance.inventory.Add(kv.Value);

            var gridGO = new GameObject("VaultGrid");
            gridGO.transform.SetParent(transform);
            var grid = gridGO.AddComponent<GridSystem>();
            grid.width = 16; grid.height = 14; grid.tileWidth = 1f; grid.tileHeight = 0.5f;
            grid.Build();
            gridGO.AddComponent<TileFloorRenderer>();

            var cam = Camera.main;
            if (cam == null) { var g = new GameObject("Main Camera"); g.tag = "MainCamera"; cam = g.AddComponent<Camera>(); }
            cam.orthographic = true; cam.orthographicSize = 7.5f;
            cam.transform.position = grid.GridToWorld(8, 7) + new Vector3(0, 0, -10);
            cam.backgroundColor = new Color(0.10f, 0.07f, 0.16f); // the margin's violet dark
            if (cam.GetComponent<SunderedCrown.CameraRig.IsometricCameraController>() == null)
                cam.gameObject.AddComponent<SunderedCrown.CameraRig.IsometricCameraController>();

            gameObject.AddComponent<ExplorationHUD>();
            gameObject.AddComponent<InventoryScreen>();
            var explore = gameObject.AddComponent<ExplorationController>();
            explore.worldCamera = cam;
            var ui = gameObject.AddComponent<RiddleVaultUI>();
            ui.content = content;

            explore.Leader = SpawnLeader(grid, new Vector2Int(2, 7));

            // Ten object pedestals (two rows of five).
            var pedCoords = new[]
            {
                new Vector2Int(4,4), new Vector2Int(6,4), new Vector2Int(8,4), new Vector2Int(10,4), new Vector2Int(12,4),
                new Vector2Int(4,10), new Vector2Int(6,10), new Vector2Int(8,10), new Vector2Int(10,10), new Vector2Int(12,10),
            };
            for (int i = 0; i < content.Pedestals.Count && i < pedCoords.Length; i++)
            {
                var riddle = content.Pedestals[i]; // local copy for the closure
                MakeExit(grid, $"Pedestal — {(Solved(riddle.id) ? "✔ " : "")}place a token", pedCoords[i],
                    new Color(0.4f, 0.3f, 0.55f), () => ui.OpenObject(riddle));
            }

            // The braziers — spoken riddles (middle row).
            var brazierCoords = new[] { new Vector2Int(5,7), new Vector2Int(7,7), new Vector2Int(9,7), new Vector2Int(11,7), new Vector2Int(13,7) };
            for (int i = 0; i < content.Spoken.Count && i < brazierCoords.Length; i++)
            {
                var riddle = content.Spoken[i];
                MakeExit(grid, $"Brazier — {(Solved(riddle.id) ? "✔ " : "")}speak an answer", brazierCoords[i],
                    new Color(0.6f, 0.4f, 0.2f), () => ui.OpenSpoken(riddle));
            }

            // The secret eleventh — her own name.
            MakeExit(grid, "The Margin Brazier (a riddle with no pedestal)", new Vector2Int(8, 7),
                new Color(0.8f, 0.65f, 0.95f), () => ui.OpenSpoken(content.TheName));

            // The way out.
            MakeExit(grid, "Step Back Into the Story (leave the Vault)", new Vector2Int(2, 12),
                new Color(0.5f, 0.5f, 0.6f), () => onLeave?.Invoke());
        }

        private bool Solved(string id) => Core.GameFlags.Current.GetBool($"riddle.{id}.solved");

        // ---- helpers ----
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

        private void MakeExit(GridSystem grid, string name, Vector2Int coord, Color color, System.Action onExit)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = name;
            go.transform.SetParent(transform);
            go.transform.localScale = new Vector3(0.6f, 0.9f, 0.6f);
            go.GetComponent<Renderer>().material.color = color;
            var it = go.AddComponent<Interactable>();
            it.label = name; it.coord = coord; it.kind = InteractionKind.Exit; it.onExit = onExit;
            it.Place(grid);
        }
    }
}
