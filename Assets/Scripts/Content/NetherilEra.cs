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
    /// NETHERIL — a falling flying-city enclave (Act III, era 1). An explorable golden ruin where
    /// you arrive out of time, find Naeve in the rubble, and trigger the falling-city battle
    /// (EncounterBuilder with the collapsing-floor hazard). Progress is flag-tracked so it survives
    /// the round-trip to combat, mirroring the Cinderhaunt's structure. The director sets the
    /// callbacks and calls Begin().
    /// </summary>
    public class NetherilEra : MonoBehaviour
    {
        public SwordCoastContent content;
        public NetherilContent netheril;
        public CharacterSheet leaderSheet;
        public Vector2Int entryCoord = new Vector2Int(3, 7);

        public System.Action<string, Vector2Int> onStartFight; // (encounterId, returnCoord)
        public System.Action onLeave;                           // back to the present / hub
        public System.Action<Vector2Int> onReenter;             // rebuild this era at coord

        public void Begin()
        {
            var f = GameFlags.Current;

            // Grid.
            var gridGO = new GameObject("NetherilGrid");
            gridGO.transform.SetParent(transform);
            var grid = gridGO.AddComponent<GridSystem>();
            grid.width = 18; grid.height = 14; grid.tileWidth = 1f; grid.tileHeight = 0.5f;
            grid.Build();
            gridGO.AddComponent<SunderedCrown.Rendering.TileFloorRenderer>();

            // Camera.
            var cam = Camera.main;
            if (cam == null) { var g = new GameObject("Main Camera"); g.tag = "MainCamera"; cam = g.AddComponent<Camera>(); }
            cam.orthographic = true; cam.orthographicSize = 7.5f;
            cam.transform.position = grid.GridToWorld(9, 7) + new Vector3(0, 0, -10);
            cam.backgroundColor = new Color(0.12f, 0.14f, 0.28f); // impossible blue sky
            if (cam.GetComponent<SunderedCrown.CameraRig.IsometricCameraController>() == null)
                cam.gameObject.AddComponent<SunderedCrown.CameraRig.IsometricCameraController>();

            // UI + exploration.
            var dlg = gameObject.AddComponent<DialogueScreen>();
            dlg.playerSpeaker = leaderSheet;
            if (!f.GetBool("netheril.arrived")) dlg.autoPlay = netheril.ArrivalDialogue; // the fall, once
            gameObject.AddComponent<ExplorationHUD>();
            gameObject.AddComponent<InventoryScreen>();
            var explore = gameObject.AddComponent<ExplorationController>();
            explore.worldCamera = cam;
            explore.Leader = SpawnLeader(grid, entryCoord);

            // Point of interest.
            MakeExamine(grid, "The Falling Sky", new Vector2Int(6, 10),
                "Below — where ground should be — there is only blue, and cloud, and the long bright drop. " +
                "The enclave groans. Spires shear away and vanish upward as the city plummets.");

            // Naeve, trapped in the rubble (until recruited).
            if (!f.GetBool("companion.naeve.recruited"))
                MakeNpc(grid, "Naeve (trapped in rubble)", new Vector2Int(7, 6),
                    new Color(0.85f, 0.75f, 0.3f), netheril.NaeveRecruit);
            // ...but if she's recruited AND with you, she walks her own dead world as it falls.
            else if (PartyHas("Naeve"))
                MakeNpc(grid, "Naeve — home, the last time", new Vector2Int(11, 5),
                    new Color(0.9f, 0.82f, 0.45f), BuildNaeveWitness(f));

            // Salvage.
            if (!f.GetBool("netheril.loot1"))
                MakeContainer(grid, "Arcanist's Reliquary", new Vector2Int(5, 4), 60,
                    new List<ItemDefinition> { content.Items["healing_potion"] }, "netheril.loot1");

            // Optional miniboss — a war-construct guarding the falling enclave's core.
            if (!f.GetBool("netheril.boss_down"))
                MakeExit(grid, "Face the Mythallar Colossus (optional miniboss)", new Vector2Int(14, 4),
                    () => onStartFight?.Invoke("netheril_boss", new Vector2Int(12, 7)));

            // The battle — or the way out, once it's cleared.
            if (!f.GetBool("netheril.cleared"))
                MakeExit(grid, "Hold the Line (defend the skybarge)", new Vector2Int(14, 7),
                    () => onStartFight?.Invoke("falling", new Vector2Int(12, 7)));
            else
                MakeExit(grid, "Ride the Wreckage Down (leave the era)", new Vector2Int(14, 7),
                    () => onLeave?.Invoke());
        }

        private static bool PartyHas(string nameMatch)
        {
            var p = Party.Instance;
            if (p == null) return false;
            foreach (var m in p.active)
                if (m?.displayName != null && m.displayName.Contains(nameMatch)) return true;
            return false;
        }

        /// <summary>Naeve, walking the Seventh Enclave the very hour it falls — reactive to how her personal
        /// quest ("After the Sky Fell") resolved, the mirror of Ilfaeril's Crown Wars beat.</summary>
        private DialogueGraph BuildNaeveWitness(GameFlags f)
        {
            var g = ScriptableObject.CreateInstance<DialogueGraph>();
            g.conversationId = "netheril.naeve_witness"; g.startNodeId = "0";

            g.nodes.Add(new DialogueNode
            {
                id = "0", speaker = "Naeve",
                text = "I have done the impossible arithmetic of my own life ten thousand times, and never once did it " +
                       "let me back *here* — the hour it fell, the air still warm with the Weave before Karsus reached " +
                       "too far. That balcony. I derived my first proof on that balcony. It is about to be sky.",
                choices = new[]
                {
                    new DialogueChoice { text = "We can't stop the fall. But you're not watching it alone this time.", nextNodeId = "stay" },
                    new DialogueChoice { text = "Is there anything here you need to see?", nextNodeId = "see" },
                }
            });

            g.nodes.Add(Line("stay", "Naeve",
                "No. We cannot stop it; I have run that equation more than any other, and it is the one answer I am " +
                "certain of. (Her hand finds yours, briefly, which she would never admit to.) But you are right. The " +
                "first time, I fell alone. This time the variable is different. *You* are here. It changes the result " +
                "more than the mathematics says it should.", "close"));

            g.nodes.Add(Line("see", "Naeve",
                "Everything. None of it. The faces — there, in the plaza — are people I will spend a thousand years " +
                "failing to grieve, because I never let it be mine to grieve. To stand among them now, breathing, is " +
                "either the cruelest gift you could give me or the kindest. With you, lately, it is always both.", "close"));

            string closeLine =
                f.GetBool("quest.naeve.rekindled")
                    ? "But I know now what becomes of this — I gave the last of it *back,* in the present. The Enclave " +
                      "ends here, yes. And it also becomes the seed of new magic, by my own hand. I can walk it down " +
                      "without flinching, because for once the proof has a kind conclusion."
              : f.GetBool("quest.naeve.released")
                    ? "And I let it go, in the present — let the last fragment finish its fall. So I can let this go too. " +
                      "Some mistakes you do not erase; you carry them. I am learning to carry this one with my head up."
              : f.GetBool("quest.naeve.preserved")
                    ? "I kept a fragment of it frozen, in the present — a tomb I call a home. Standing here, in the warm " +
                      "living hour of it, I wonder if I was wrong to. ...Ask me again after. I am not ready to know."
              : "There is a shard of this, surviving, in the present — calling me. I can feel it even here. I will have " +
                "to answer it. But not in this hour. In this hour I will simply *be home,* one last impossible time.";

            g.nodes.Add(new DialogueNode
            {
                id = "close", speaker = "Naeve", text = closeLine,
                onEnter = new[] { new FlagClause { key = "netheril.naeve_witnessed", op = FlagOp.SetTrue },
                                  new FlagClause { key = "companion.naeve.approval", op = FlagOp.AddInt, amount = 5 } },
            });
            return g;
        }

        private static DialogueNode Line(string id, string speaker, string text, string next)
            => new DialogueNode { id = id, speaker = speaker, text = text,
                choices = new[] { new DialogueChoice { text = "(go on)", nextNodeId = next } } };

        // ---- spawn helpers (mirrors Cinderhaunt) ----

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

        private void MakeNpc(GridSystem grid, string name, Vector2Int coord, Color color, DialogueGraph graph)
        {
            var it = MakeMarker(grid, name, coord, color);
            it.kind = InteractionKind.Talk; it.dialogue = graph;
        }

        private void MakeExamine(GridSystem grid, string name, Vector2Int coord, string text)
        {
            var it = MakeMarker(grid, name, coord, new Color(0.5f, 0.5f, 0.6f));
            it.kind = InteractionKind.Examine; it.examineText = text;
        }

        private void MakeExit(GridSystem grid, string name, Vector2Int coord, System.Action onExit)
        {
            var it = MakeMarker(grid, name, coord, new Color(0.7f, 0.6f, 0.2f));
            it.kind = InteractionKind.Exit; it.onExit = onExit;
        }

        private void MakeContainer(GridSystem grid, string name, Vector2Int coord, int gold,
            List<ItemDefinition> items, string lootFlag)
        {
            var it = MakeMarker(grid, name, coord, new Color(0.6f, 0.5f, 0.25f));
            it.kind = InteractionKind.Container; it.gold = gold; it.contents = items; it.lootFlag = lootFlag;
        }
    }
}
