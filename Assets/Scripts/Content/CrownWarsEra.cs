using System.Collections.Generic;
using UnityEngine;
using SunderedCrown.Characters;
using SunderedCrown.Core;
using SunderedCrown.Dialogue;
using SunderedCrown.Grid;
using SunderedCrown.Items;
using SunderedCrown.Rendering;
using SunderedCrown.UI;
using SunderedCrown.World;

namespace SunderedCrown.Content
{
    /// <summary>
    /// THE CROWN WARS — an elven high court ~10,000 years before the present (Act III, era 2). An
    /// explorable silver-and-shadow hall where you arrive at the first soul-damnation, recruit
    /// Ilfaeril, can argue the verdict **down** (the moral hazard), and break the damnation-rite in
    /// battle against the elven damned. Flag-tracked; mirrors the other era builders.
    /// </summary>
    public class CrownWarsEra : MonoBehaviour
    {
        public SwordCoastContent content;
        public CrownWarsContent crownwars;
        public CharacterSheet leaderSheet;
        public Vector2Int entryCoord = new Vector2Int(3, 7);

        public System.Action<string, Vector2Int> onStartFight;
        public System.Action onLeave;
        public System.Action<Vector2Int> onReenter;

        public void Begin()
        {
            var f = GameFlags.Current;

            var gridGO = new GameObject("CrownWarsGrid");
            gridGO.transform.SetParent(transform);
            var grid = gridGO.AddComponent<GridSystem>();
            grid.width = 18; grid.height = 14; grid.tileWidth = 1f; grid.tileHeight = 0.5f;
            grid.Build();
            gridGO.AddComponent<TileFloorRenderer>(); // tiled elven-hall floor

            var cam = Camera.main;
            if (cam == null) { var g = new GameObject("Main Camera"); g.tag = "MainCamera"; cam = g.AddComponent<Camera>(); }
            cam.orthographic = true; cam.orthographicSize = 7.5f;
            cam.transform.position = grid.GridToWorld(9, 7) + new Vector3(0, 0, -10);
            cam.backgroundColor = new Color(0.10f, 0.14f, 0.14f); // silver-shadow elven court
            if (cam.GetComponent<SunderedCrown.CameraRig.IsometricCameraController>() == null)
                cam.gameObject.AddComponent<SunderedCrown.CameraRig.IsometricCameraController>();

            var dlg = gameObject.AddComponent<DialogueScreen>();
            dlg.playerSpeaker = leaderSheet;
            if (!f.GetBool("crownwars.arrived")) dlg.autoPlay = crownwars.ArrivalDialogue;
            gameObject.AddComponent<ExplorationHUD>();
            gameObject.AddComponent<InventoryScreen>();
            var explore = gameObject.AddComponent<ExplorationController>();
            explore.worldCamera = cam;
            explore.Leader = SpawnLeader(grid, entryCoord);

            MakeExamine(grid, "The Pale Banners", new Vector2Int(6, 10),
                "Banners of a people about to be erased from the memory of the world. In ten thousand years, " +
                "no one will mourn them — because that is precisely what is being voted, here, today.");

            // Ilfaeril, in the court (until recruited).
            if (!f.GetBool("companion.ilfaeril.recruited"))
                MakeNpc(grid, "Ilfaeril (of the High Court)", new Vector2Int(7, 6),
                    new Color(0.7f, 0.75f, 0.8f), crownwars.IlfaerilRecruit);
            // ...but if he's recruited AND with you, he stands in the hall where he cast the vote.
            else if (PartyHas("Ilfaeril"))
                MakeNpc(grid, "Ilfaeril — here, where it happened", new Vector2Int(11, 5),
                    new Color(0.72f, 0.78f, 0.85f), BuildIlfaerilWitness(f));

            // The moral hazard: argue the verdict down.
            MakeNpc(grid, "High Lord Aelryth — the Verdict", new Vector2Int(9, 9),
                new Color(0.8f, 0.8f, 0.55f), crownwars.VerdictDialogue);

            if (!f.GetBool("crownwars.loot1"))
                MakeContainer(grid, "Reliquary of the Damned", new Vector2Int(5, 4), 75,
                    new List<ItemDefinition> { content.Items["healing_potion"] }, "crownwars.loot1");

            // Optional miniboss — the very soul the court damned, risen in grief and fury.
            if (!f.GetBool("crownwars.boss_down"))
                MakeExit(grid, "Face the First Unmade (optional miniboss)", new Vector2Int(14, 4),
                    () => onStartFight?.Invoke("crownwars_boss", new Vector2Int(12, 7)));

            if (!f.GetBool("crownwars.cleared"))
                MakeExit(grid, "Break the Damnation-Rite (battle)", new Vector2Int(14, 7),
                    () => onStartFight?.Invoke("damnation", new Vector2Int(12, 7)));
            else
                MakeExit(grid, "Step Back Through Time (leave the era)", new Vector2Int(14, 7),
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

        /// <summary>Ilfaeril, beside you in the very court that cast the vote — reactive to whether you've
        /// argued the verdict down here, and to how his personal quest ("The Vote") resolved.</summary>
        private DialogueGraph BuildIlfaerilWitness(GameFlags f)
        {
            var g = ScriptableObject.CreateInstance<DialogueGraph>();
            g.conversationId = "crownwars.ilfaeril_witness"; g.startNodeId = "0";

            g.nodes.Add(new DialogueNode
            {
                id = "0", speaker = "Ilfaeril",
                text = "I have stood in this hall ten thousand times, in nightmares. I did not think I would ever stand " +
                       "in it again with *feet.* There — the third bench from the dais. That is where I raised my hand. " +
                       "I can smell the lamp-oil. Gods help me, I can *smell* it.",
                choices = new[]
                {
                    new DialogueChoice { text = "Then change it. Argue the verdict down — you have the chance now.", nextNodeId = "act" },
                    new DialogueChoice { text = "Just breathe. I'm here. You don't face it alone this time.", nextNodeId = "stay" },
                }
            });

            string actLine = f.GetBool("crownwars.verdict_spared")
                ? "...You already did. You stood where I could not and argued one court out of its cruelty, and a valley " +
                  "of souls will rest that did not, in my history. I have wept perhaps three times in ten thousand years. " +
                  "This is the third."
                : "Then let us try. I cannot vote it down myself — the me of this age is over there, certain and young " +
                  "and *wrong* — but you can put the question to the High Lord. Do it. Do what I was too afraid to.";
            g.nodes.Add(Line("act", "Ilfaeril", actLine, "close"));

            g.nodes.Add(Line("stay", "Ilfaeril",
                "...Thank you. The first time, I had a hall full of lords and never felt more alone. This time I have one " +
                "person who knows exactly what I am, standing close enough to touch. It should not change the weight. It " +
                "does.", "close"));

            string closeLine =
                f.GetBool("quest.ilfaeril.commission")
                    ? "Maerith armed me for precisely this room — her forgiveness was never a door, it was a sword. I do " +
                      "not stand here to grieve. I stand here to *work.* Show me the hand that needs lowering."
              : f.GetBool("quest.ilfaeril.forgiven")
                    ? "I was forgiven, in the present — by one of the souls I damned. It does not make this hall easier. " +
                      "It makes it *bearable*, which is more than I earned and exactly what she wanted for me."
              : f.GetBool("quest.ilfaeril.penance")
                    ? "I have not let myself be forgiven; the weight is the last true thing I have of them. So let me at " +
                      "least be *useful* here, where it all began. That, I am still allowed."
              : "There is a reliquary waiting for me in the present, I think — a name I will have to answer for. Not yet. " +
                "First, this hall. One reckoning at a time, or an old man comes apart entirely.";

            g.nodes.Add(new DialogueNode
            {
                id = "close", speaker = "Ilfaeril", text = closeLine,
                onEnter = new[] { new FlagClause { key = "crownwars.ilfaeril_witnessed", op = FlagOp.SetTrue },
                                  new FlagClause { key = "companion.ilfaeril.approval", op = FlagOp.AddInt, amount = 5 } },
            });
            return g;
        }

        private static DialogueNode Line(string id, string speaker, string text, string next)
            => new DialogueNode { id = id, speaker = speaker, text = text,
                choices = new[] { new DialogueChoice { text = "(go on)", nextNodeId = next } } };

        // ---- helpers (mirror the other era builders) ----

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
            var it = MakeMarker(grid, name, coord, new Color(0.7f, 0.6f, 0.7f));
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
