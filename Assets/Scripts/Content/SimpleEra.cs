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
    /// A reusable, config-driven era scene (for eras without a unique exploration gimmick): grid +
    /// tiled floor + camera + exploration UI + leader + one point-of-interest + a battle trigger that
    /// becomes the exit once cleared. The Time of Troubles and the Spellplague are built on this.
    /// Set the fields, then call Begin().
    /// </summary>
    public class SimpleEra : MonoBehaviour
    {
        public CharacterSheet leaderSheet;
        public Vector2Int entryCoord = new Vector2Int(3, 7);

        public string sceneTitle = "Era";
        public Color background = new Color(0.12f, 0.12f, 0.14f);
        public DialogueGraph arrivalDialogue;
        public string arrivedFlag;
        public string clearedFlag;
        public string examineLabel = "A Strange Sight";
        [TextArea(2, 5)] public string examineText = "";
        public string fightId = "fight";
        public string fightLabel = "Join the battle";

        // Optional companion-reactive "witness" beat — placed only if that companion is fielded.
        public string witnessNameMatch;
        public System.Func<DialogueGraph> witnessGraph;

        // Optional cross-era "echo" beat — the world naming an upstream choice (EraEchoes). Always placed
        // (it's the world, not a companion), as long as the builder is wired and returns a graph.
        public string echoLabel = "An Echo of an Older Choice";
        public System.Func<DialogueGraph> echoGraph;

        // A second, independent echo slot — an era may name more than one upstream thread (e.g. the Time of
        // Troubles speaks both the Crown Wars Verdict and the fate of the Crown's gentle bearer). Same rules.
        public string echoLabel2 = "Another Echo of an Older Choice";
        public System.Func<DialogueGraph> echoGraph2;

        // Optional extra "miniboss" fight — a second combat exit, available until its done-flag is set.
        public string bonusFightId;
        public string bonusFightLabel = "An optional fight";
        public string bonusFightDoneFlag;

        public System.Action<string, Vector2Int> onStartFight;
        public System.Action onLeave;

        public void Begin()
        {
            var f = GameFlags.Current;

            var gridGO = new GameObject(sceneTitle + "Grid");
            gridGO.transform.SetParent(transform);
            var grid = gridGO.AddComponent<GridSystem>();
            grid.width = 18; grid.height = 14; grid.tileWidth = 1f; grid.tileHeight = 0.5f;
            grid.Build();
            gridGO.AddComponent<TileFloorRenderer>();

            var cam = Camera.main;
            if (cam == null) { var g = new GameObject("Main Camera"); g.tag = "MainCamera"; cam = g.AddComponent<Camera>(); }
            cam.orthographic = true; cam.orthographicSize = 7.5f;
            cam.transform.position = grid.GridToWorld(9, 7) + new Vector3(0, 0, -10);
            cam.backgroundColor = background;
            if (cam.GetComponent<SunderedCrown.CameraRig.IsometricCameraController>() == null)
                cam.gameObject.AddComponent<SunderedCrown.CameraRig.IsometricCameraController>();

            var dlg = gameObject.AddComponent<DialogueScreen>();
            dlg.playerSpeaker = leaderSheet;
            if (arrivalDialogue != null && !string.IsNullOrEmpty(arrivedFlag) && !f.GetBool(arrivedFlag))
                dlg.autoPlay = arrivalDialogue;
            gameObject.AddComponent<ExplorationHUD>();
            gameObject.AddComponent<InventoryScreen>();
            gameObject.AddComponent<JourneyScreen>();   // J — consistent across eras
            gameObject.AddComponent<RosterScreen>();    // P
            gameObject.AddComponent<CodexScreen>();     // K — fills in as you walk history
            gameObject.AddComponent<AmbientBanter>();   // idle chatter on the road (mute B)
            var explore = gameObject.AddComponent<ExplorationController>();
            explore.worldCamera = cam;
            explore.Leader = SpawnLeader(grid, entryCoord);

            if (!string.IsNullOrEmpty(examineText))
                MakeExamine(grid, examineLabel, new Vector2Int(6, 10), examineText);

            // A fielded companion confronting this era's history (Garrow/TooT, Varra/Spellplague, …).
            if (!string.IsNullOrEmpty(witnessNameMatch) && witnessGraph != null && PartyHas(witnessNameMatch))
            {
                var it = MakeMarker(grid, witnessNameMatch + " — here, now", new Vector2Int(11, 5),
                    new Color(0.7f, 0.7f, 0.78f));
                it.kind = InteractionKind.Talk; it.dialogue = witnessGraph();
            }

            // A cross-era echo — the world speaking your upstream choice back to you (Pillar V).
            if (echoGraph != null)
            {
                var eg = echoGraph();
                if (eg != null)
                {
                    var it = MakeMarker(grid, echoLabel, new Vector2Int(8, 3), new Color(0.55f, 0.5f, 0.45f));
                    it.kind = InteractionKind.Talk; it.dialogue = eg;
                }
            }

            // A second cross-era echo (independent of the first) — same contract, different thread.
            if (echoGraph2 != null)
            {
                var eg2 = echoGraph2();
                if (eg2 != null)
                {
                    var it = MakeMarker(grid, echoLabel2, new Vector2Int(4, 4), new Color(0.78f, 0.74f, 0.66f));
                    it.kind = InteractionKind.Talk; it.dialogue = eg2;
                }
            }

            // Optional miniboss — a second combat marker, available until you've put it down.
            if (!string.IsNullOrEmpty(bonusFightId) &&
                (string.IsNullOrEmpty(bonusFightDoneFlag) || !f.GetBool(bonusFightDoneFlag)))
                MakeExit(grid, bonusFightLabel, new Vector2Int(14, 4), new Color(0.42f, 0.4f, 0.62f),
                    () => onStartFight?.Invoke(bonusFightId, new Vector2Int(12, 7)));

            bool cleared = !string.IsNullOrEmpty(clearedFlag) && f.GetBool(clearedFlag);
            if (!cleared)
                MakeExit(grid, fightLabel, new Vector2Int(14, 7), new Color(0.6f, 0.4f, 0.5f),
                    () => onStartFight?.Invoke(fightId, new Vector2Int(12, 7)));
            else
                MakeExit(grid, "Step Back Through Time (leave the era)", new Vector2Int(14, 7),
                    new Color(0.5f, 0.5f, 0.6f), () => onLeave?.Invoke());
        }

        // ---- helpers ----
        private static bool PartyHas(string nameMatch)
        {
            var p = Party.Instance;
            if (p == null) return false;
            foreach (var m in p.active)
                if (m?.displayName != null && m.displayName.Contains(nameMatch)) return true;
            return false;
        }

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
