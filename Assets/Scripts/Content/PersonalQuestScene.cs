using UnityEngine;
using SunderedCrown.Characters;
using SunderedCrown.Core;
using SunderedCrown.Dialogue;
using SunderedCrown.Grid;
using SunderedCrown.Rendering;
using SunderedCrown.UI;
using SunderedCrown.World;

namespace SunderedCrown.Content
{
    /// <summary>
    /// The reusable stage for any companion personal quest. Reads a <see cref="PersonalQuest"/> config
    /// and runs the same arc shape every time: arrive (auto-dialogue) → examine the scene → confront the
    /// NPC (reveal) → fight → make the moral call → leave. Beat state comes from the config's flags
    /// (clearedFlag from the director's fight win; resolvedFlag from the resolution choice). Content,
    /// not code, distinguishes Roen's quest from Varra's. Set the fields, then call Begin().
    /// </summary>
    public class PersonalQuestScene : MonoBehaviour
    {
        public CharacterSheet leaderSheet;
        public PersonalQuest quest;
        public Vector2Int entryCoord = new Vector2Int(3, 7);

        public System.Action<string, Vector2Int> onStartFight; // (fightId, returnCoord)
        public System.Action onLeave;

        public void Begin()
        {
            var f = GameFlags.Current;

            var gridGO = new GameObject(quest.id + "QuestGrid");
            gridGO.transform.SetParent(transform);
            var grid = gridGO.AddComponent<GridSystem>();
            grid.width = 18; grid.height = 14; grid.tileWidth = 1f; grid.tileHeight = 0.5f;
            grid.Build();
            gridGO.AddComponent<TileFloorRenderer>();

            var cam = Camera.main;
            if (cam == null) { var g = new GameObject("Main Camera"); g.tag = "MainCamera"; cam = g.AddComponent<Camera>(); }
            cam.orthographic = true; cam.orthographicSize = 7.5f;
            cam.transform.position = grid.GridToWorld(9, 7) + new Vector3(0, 0, -10);
            cam.backgroundColor = quest.background;
            if (cam.GetComponent<SunderedCrown.CameraRig.IsometricCameraController>() == null)
                cam.gameObject.AddComponent<SunderedCrown.CameraRig.IsometricCameraController>();

            var dlg = gameObject.AddComponent<DialogueScreen>();
            dlg.playerSpeaker = leaderSheet;
            if (quest.arrival != null && (string.IsNullOrEmpty(quest.arrivedFlag) || !f.GetBool(quest.arrivedFlag)))
                dlg.autoPlay = quest.arrival;
            gameObject.AddComponent<ExplorationHUD>();
            gameObject.AddComponent<InventoryScreen>();
            gameObject.AddComponent<JourneyScreen>();
            gameObject.AddComponent<RosterScreen>();
            gameObject.AddComponent<CodexScreen>();
            gameObject.AddComponent<AmbientBanter>();
            var explore = gameObject.AddComponent<ExplorationController>();
            explore.worldCamera = cam;
            explore.Leader = SpawnLeader(grid, entryCoord);

            if (!string.IsNullOrEmpty(quest.examineText))
                MakeExamine(grid, quest.examineLabel, new Vector2Int(6, 10), quest.examineText);

            bool cleared = !string.IsNullOrEmpty(quest.clearedFlag) && f.GetBool(quest.clearedFlag);
            bool resolved = !string.IsNullOrEmpty(quest.resolvedFlag) && f.GetBool(quest.resolvedFlag);

            if (!cleared)
            {
                if (quest.reveal != null)
                    MakeNpc(grid, quest.revealNpcLabel, new Vector2Int(10, 8),
                        new Color(0.6f, 0.55f, 0.7f), quest.reveal);
                MakeExit(grid, quest.fightLabel, new Vector2Int(14, 7),
                    new Color(0.55f, 0.2f, 0.2f),
                    () => onStartFight?.Invoke(quest.fightId, new Vector2Int(12, 7)));
            }
            else if (!resolved)
            {
                if (quest.resolution != null)
                    MakeNpc(grid, quest.resolutionNpcLabel, new Vector2Int(10, 8),
                        new Color(0.7f, 0.65f, 0.85f), quest.resolution);
            }
            else
            {
                MakeExit(grid, quest.leaveLabel, new Vector2Int(14, 7),
                    new Color(0.5f, 0.5f, 0.6f), () => onLeave?.Invoke());
            }
        }

        // ---- helpers (mirror SimpleEra) ----
        private GridUnit SpawnLeader(GridSystem grid, Vector2Int coord)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = leaderSheet.displayName; go.transform.SetParent(transform);
            go.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);
            go.GetComponent<Renderer>().material.color = Color.blue;
            var unit = go.AddComponent<GridUnit>();
            unit.faction = Faction.Player; unit.startCoord = coord; unit.Sheet = leaderSheet;
            return unit;
        }

        private void MakeExamine(GridSystem grid, string name, Vector2Int coord, string text)
        {
            var it = MakeMarker(grid, name, coord, new Color(0.5f, 0.5f, 0.6f));
            it.kind = InteractionKind.Examine; it.examineText = text;
        }

        private void MakeNpc(GridSystem grid, string name, Vector2Int coord, Color color, DialogueGraph graph)
        {
            var it = MakeMarker(grid, name, coord, color);
            it.kind = InteractionKind.Talk; it.dialogue = graph;
        }

        private void MakeExit(GridSystem grid, string name, Vector2Int coord, Color color, System.Action onExit)
        {
            var it = MakeMarker(grid, name, coord, color);
            it.kind = InteractionKind.Exit; it.onExit = onExit;
        }

        private Interactable MakeMarker(GridSystem grid, string name, Vector2Int coord, Color color)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = name; go.transform.SetParent(transform);
            go.transform.localScale = new Vector3(0.6f, 0.8f, 0.6f);
            go.GetComponent<Renderer>().material.color = color;
            var it = go.AddComponent<Interactable>();
            it.label = name; it.coord = coord; it.Place(grid);
            return it;
        }
    }
}
