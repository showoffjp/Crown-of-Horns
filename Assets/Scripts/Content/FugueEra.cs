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
    /// THE FUGUE PLANE — the grey waystation of the dead and the Wall of the Faithless (Act II). You
    /// walk the Wall, find the niche with your own name, and may pull Maerin free — which triggers the
    /// Breach (the director's permanent loss). Mirrors the era builders; grey, silent, dread.
    /// </summary>
    public class FugueEra : MonoBehaviour
    {
        public SwordCoastContent content;
        public FugueContent fugue;
        public CharacterSheet leaderSheet;
        public Vector2Int entryCoord = new Vector2Int(3, 7);

        public System.Action onLeave;

        public void Begin()
        {
            var f = GameFlags.Current;

            var gridGO = new GameObject("FugueGrid");
            gridGO.transform.SetParent(transform);
            var grid = gridGO.AddComponent<GridSystem>();
            grid.width = 18; grid.height = 14; grid.tileWidth = 1f; grid.tileHeight = 0.5f;
            grid.Build();
            gridGO.AddComponent<TileFloorRenderer>();

            var cam = Camera.main;
            if (cam == null) { var g = new GameObject("Main Camera"); g.tag = "MainCamera"; cam = g.AddComponent<Camera>(); }
            cam.orthographic = true; cam.orthographicSize = 7.5f;
            cam.transform.position = grid.GridToWorld(9, 7) + new Vector3(0, 0, -10);
            cam.backgroundColor = new Color(0.13f, 0.13f, 0.14f); // the grey that is the absence of colour
            if (cam.GetComponent<SunderedCrown.CameraRig.IsometricCameraController>() == null)
                cam.gameObject.AddComponent<SunderedCrown.CameraRig.IsometricCameraController>();

            var dlg = gameObject.AddComponent<DialogueScreen>();
            dlg.playerSpeaker = leaderSheet;
            if (!f.GetBool("fugue.arrived")) dlg.autoPlay = fugue.ArrivalDialogue;
            gameObject.AddComponent<ExplorationHUD>();
            gameObject.AddComponent<InventoryScreen>();
            var explore = gameObject.AddComponent<ExplorationController>();
            explore.worldCamera = cam;
            explore.Leader = SpawnLeader(grid, entryCoord);

            // The Wall, and your niche in it.
            MakeExamine(grid, "The Wall of the Faithless", new Vector2Int(13, 4),
                "A living mortar of mouths, dissolving slow as stone. And near the end of the curve, one niche " +
                "stands empty — the only empty one — your name half-carved above it, a little more formed than " +
                "the last time you stood here. It is waiting. It has always been waiting.");

            // Maerin — in the Wall (until pulled free).
            if (!f.GetBool("companion.maerin.recruited"))
                MakeNpc(grid, "A girl in the Wall (Maerin)", new Vector2Int(11, 7),
                    new Color(0.7f, 0.7f, 0.8f), fugue.MaerinDialogue);
            else
                MakeExamine(grid, "Maerin (freed)", new Vector2Int(11, 7),
                    "She walks at your side now — luminous, half-dissolved, fading, and entirely her own. " +
                    "She does not look at the empty space in your party. Neither, after a while, can you.");

            // A Faithless soul in the Wall — reactive to how you treated the Gate's poor and dead (Act II),
            // and a fair, aching foreshadow of the Breach's arithmetic. Only before you pull Maerin free.
            if (!f.GetBool("companion.maerin.recruited"))
                MakeNpc(grid, "A Faithless soul, half-dissolved", new Vector2Int(8, 5),
                    new Color(0.55f, 0.55f, 0.62f), BuildFaithlessSoul(f));

            MakeExit(grid, "Climb Back Out of the Fugue", new Vector2Int(2, 12),
                () => onLeave?.Invoke());
        }

        /// <summary>The soul speaks differently depending on the mercy (or its absence) you spent in the
        /// Lower City — and warns, fairly, what pulling a soul out of the Wall costs.</summary>
        private DialogueGraph BuildFaithlessSoul(GameFlags f)
        {
            int rep = f.GetInt("reputation.lowcity");
            bool kind = f.GetBool("quest.tithe.freed") || f.GetBool("quest.tithe.paid") ||
                        f.GetBool("lowcity.allies") || f.GetBool("quest.widow.hope") || rep >= 3;
            bool cruel = f.GetBool("quest.tithe.corrupt") || f.GetBool("quest.choir.suppressed") || rep <= -2;

            var g = ScriptableObject.CreateInstance<DialogueGraph>();
            g.conversationId = "fugue.soul"; g.startNodeId = "0";

            string open =
                kind ? "...You. The poor up there speak your name like a candle in a cellar. The one who let the unblessed " +
                       "be buried free, who gave the grieving the truth instead of a toll. Word reaches even here — even " +
                       "*into* the Wall. You cannot imagine what a single kindness is worth, to the unclaimed."
              : cruel ? "...I know you. Word came down even to us: the Returned who took a cut of our graves, who set the law " +
                        "on the desperate. You walk out of death again and again — and we dissolve. Curious, isn't it, " +
                        "whose soul gets claimed, and whose does not."
              : "...A warm thing, here, where nothing is warm. You're the one who keeps walking back out. We don't. Did " +
                "you ever wonder why this Wall holds a niche with *your* name cut above it, and not one with ours?";

            g.nodes.Add(new DialogueNode
            {
                id = "0", speaker = "A Faithless soul", text = open,
                onEnter = new[] { new FlagClause { key = "fugue.soul.met", op = FlagOp.SetTrue } },
                choices = new[]
                {
                    new DialogueChoice { text = "There's an empty niche with my name. Why?", nextNodeId = "niche" },
                    new DialogueChoice { text = "Can anyone ever leave this Wall?", nextNodeId = "leave" },
                }
            });

            g.nodes.Add(new DialogueNode
            {
                id = "niche", speaker = "A Faithless soul",
                text = "Because it has worn your name since before the world was young, and it is patient. The only way a " +
                       "soul comes *out* of this Wall is if another goes *in.* That is the oldest arithmetic there is. If " +
                       "you ever pull someone free of the grey — and I think you will; it's written all over you — know " +
                       "what the Wall asks back.",
                choices = new[]
                {
                    new DialogueChoice { text = "Who would it take?", nextNodeId = "who" },
                }
            });

            g.nodes.Add(Line2("leave", "A Faithless soul",
                "Leave? No. We *thin.* We become the mortar. But you — you could pull one of us loose, if you were fool " +
                "enough, or loving enough, to pay the toll. And it is a toll. Ask your heart which of your companions it " +
                "would miss most, and you'll know the Wall's price before it names it.", "who"));

            g.nodes.Add(new DialogueNode
            {
                id = "who", speaker = "A Faithless soul",
                text = kind
                    ? "Whoever the Wall can reach — and never you; you've made certain of that. It takes the one nearest " +
                      "your heart. So if mercy is a habit with you, spend it now, on the living, while they're still warm " +
                      "to spend it on. Some of us learned that lesson a breath too late. Go on. Be loud up there. Sorry " +
                      "never blessed a grave — but you already knew that, didn't you."
                    : "Whoever the Wall can reach — and never *you,* of course. It takes the one nearest your heart. " +
                      "Strange currency, for a soul that haggles. When the bill comes due in the grey, I wonder if you'll " +
                      "argue it down to twenty percent. (It almost smiles, and then there is less of it than there was.)"
            });
            return g;
        }

        // ---- helpers ----
        private static DialogueNode Line2(string id, string speaker, string text, string next)
            => new DialogueNode { id = id, speaker = speaker, text = text,
                choices = new[] { new DialogueChoice { text = "...", nextNodeId = next } } };

        private GridUnit SpawnLeader(GridSystem grid, Vector2Int coord)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = leaderSheet.displayName;
            go.transform.SetParent(transform);
            go.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);
            go.GetComponent<Renderer>().material.color = Color.blue;
            var unit = go.AddComponent<GridUnit>();
            unit.faction = Faction.Player; unit.startCoord = coord; unit.Sheet = leaderSheet;
            return unit;
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

        private void MakeNpc(GridSystem grid, string name, Vector2Int coord, Color color, DialogueGraph graph)
        {
            var it = MakeMarker(grid, name, coord, color);
            it.kind = InteractionKind.Talk; it.dialogue = graph;
        }

        private void MakeExamine(GridSystem grid, string name, Vector2Int coord, string text)
        {
            var it = MakeMarker(grid, name, coord, new Color(0.45f, 0.45f, 0.5f));
            it.kind = InteractionKind.Examine; it.examineText = text;
        }

        private void MakeExit(GridSystem grid, string name, Vector2Int coord, System.Action onExit)
        {
            var it = MakeMarker(grid, name, coord, new Color(0.5f, 0.5f, 0.6f));
            it.kind = InteractionKind.Exit; it.onExit = onExit;
        }
    }
}
