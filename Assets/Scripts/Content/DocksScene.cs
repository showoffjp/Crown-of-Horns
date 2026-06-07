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
    /// THE CHIONTHAR DOCKS — a third explorable Lower City room: a working waterfront where the river meets
    /// the Gate. A dockhand who reacts to your standing, a smuggler with a wary eye on the Returned, and a
    /// memorial to the drowned (the poor's dead the river keeps for free, which the church never could).
    /// Mirrors MarketScene/AlmshouseScene; set leaderSheet + onLeave, call Begin().
    /// </summary>
    public class DocksScene : MonoBehaviour
    {
        public CharacterSheet leaderSheet;
        public System.Action onLeave;

        public void Begin()
        {
            var f = GameFlags.Current;
            f.SetBool("lowcity.visited_docks", true);
            SunderedCrown.UI.ExplorationHUD.Location = "The Chionthar Docks";
            var gridGO = new GameObject("DocksGrid"); gridGO.transform.SetParent(transform);
            var grid = gridGO.AddComponent<GridSystem>();
            grid.width = 16; grid.height = 12; grid.tileWidth = 1f; grid.tileHeight = 0.5f; grid.Build();
            gridGO.AddComponent<TileFloorRenderer>();

            var cam = Camera.main;
            if (cam == null) { var g = new GameObject("Main Camera"); g.tag = "MainCamera"; cam = g.AddComponent<Camera>(); }
            cam.orthographic = true; cam.orthographicSize = 7f;
            cam.transform.position = grid.GridToWorld(8, 6) + new Vector3(0, 0, -10);
            cam.backgroundColor = new Color(0.09f, 0.12f, 0.14f);
            if (cam.GetComponent<SunderedCrown.CameraRig.IsometricCameraController>() == null)
                cam.gameObject.AddComponent<SunderedCrown.CameraRig.IsometricCameraController>();

            gameObject.AddComponent<DialogueScreen>().playerSpeaker = leaderSheet;
            gameObject.AddComponent<ExplorationHUD>();
            gameObject.AddComponent<JourneyScreen>();
            gameObject.AddComponent<RosterScreen>();
            var explore = gameObject.AddComponent<ExplorationController>();
            explore.worldCamera = cam;
            explore.Leader = SpawnLeader(grid, new Vector2Int(2, 6));

            MakeNpc(grid, "A weary dockhand", new Vector2Int(7, 7), new Color(0.5f, 0.45f, 0.4f), Dockhand(f));
            MakeNpc(grid, f.GetBool("docks.ferryman_resolved") ? "The ferryman, mending nets" : "A ferryman, frantic",
                new Vector2Int(6, 3), new Color(0.45f, 0.5f, 0.45f), Ferryman(f));
            MakeNpc(grid, "A smuggler keeping to the shadows", new Vector2Int(11, 4), new Color(0.4f, 0.45f, 0.5f), Smuggler(f));
            string memorial =
                "A piling hung with hundreds of knotted cords, each a name the river took and would not give back. The " +
                "dockfolk tie one for every drowned pauper the church wouldn't bury — the Chionthar keeps them for free, " +
                "they say, which is more than the Doom ever did. The newest knot is still wet.";
            string lost = LostCompanionName(f);
            if (lost != null)
                memorial += $"\n\n(Your hands move before you decide to. You tie a cord of your own, low on the piling, for " +
                            $"{lost} — who the Wall took, not the river, and who got no grave at all. It is not nothing. " +
                            "It was never going to be enough. You tie it anyway.)";
            MakeExamine(grid, "A memorial of knotted rope", new Vector2Int(5, 9), memorial);
            if (f.GetBool("act4.spellplague_done"))
                MakeExamine(grid, "An impossible tide-line of debris", new Vector2Int(8, 9),
                    "The river has coughed up things that have no business here: a shard of Netherese skyglass still faintly " +
                    "warm, an elven coin ten thousand years out of mint, a splinter of bone-white horn. Flotsam from ages " +
                    "that fell before the Gate was founded, washed up on a Tuesday. Ever since the blue fire walked the " +
                    "world, the river forgets which century it's in. You, of all people, know exactly how that feels.");
            MakeExit(grid, "The smuggler's black-market goods", new Vector2Int(12, 5), OpenSmugglerShop);
            MakeExit(grid, "Back to the Lower City streets", new Vector2Int(2, 11), () => onLeave?.Invoke());
        }

        private void OpenSmugglerShop()
        {
            var shop = gameObject.AddComponent<ShopScreen>();
            shop.stock = ShopContent.SmugglerStock;
            shop.vendorName = "The Smuggler's Cache";
            shop.vendorTagline = "off the books";
            shop.vendorQuote = "\"River brought it in, friend. Don't ask from where. Coin first, conscience later.\"";
            shop.onClose = () => Destroy(shop);
        }

        private static string LostCompanionName(GameFlags f)
        {
            foreach (var (id, name) in new[] {
                ("garrow","Sister Garrow"), ("roen","Roen"), ("varra","Varra"),
                ("naeve","Naeve"), ("ilfaeril","Ilfaeril"), ("maerin","Maerin") })
                if (f.GetBool($"companion.{id}.lost")) return name;
            return null;
        }

        private DialogueGraph Dockhand(GameFlags f)
        {
            int eras = (f.GetBool("netheril.cleared") ? 1 : 0) + (f.GetBool("crownwars.cleared") ? 1 : 0)
                     + (f.GetBool("act4.toot_done") ? 1 : 0) + (f.GetBool("act4.spellplague_done") ? 1 : 0);
            if (eras >= 3)
                return One("Dockhand",
                    "\"There's a story going round the wharves that you've sailed *time* — walked the drowned ages and come " +
                    "back dripping with them. I haul cargo off boats from up and down the coast and I've never heard the like. " +
                    "(He studies you a long moment.) ...The thing is, looking at you? I believe it. The river believes it too. " +
                    "It's been acting strange since you came. Restless. Like it remembers somewhere it used to flow.\"");
            int rep = f.GetInt("reputation.lowcity");
            string line = rep >= 3
                ? "\"You're the Returned that actually gives a damn. Word travels on the water faster than the road. " +
                  "If you ever need a hull that asks no questions, you ask for me. That's not an offer I make twice.\""
                : rep <= -2
                    ? "\"I know your sort. Walk through, take what you need, leave the rest of us to bail. (He spits into " +
                      "the river and turns his back.)\""
                    : "\"Mind the wet boards. We haul Gate's grain and Gate's grief in equal measure down here, and the " +
                      "river doesn't care which it takes. You're the dead one, aren't you. Huh. River's full of those.\"";
            return One("Dockhand", line);
        }

        private DialogueGraph Smuggler(GameFlags f)
        {
            string line = f.GetBool("quest.fist.freed")
                ? "\"You're the one who got Kallia off a pinch with the Fist. That buys you a nod down here, friend — and " +
                  "down here a nod's worth more than a Flaming Fist writ. Don't waste it.\""
                : "\"Keep walking, Returned. Whatever you're selling, the river already bought it cheaper. (Her hand " +
                  "doesn't leave the knife at her belt, but her eyes are doing the arithmetic on you.)\"";
            return One("Smuggler", line);
        }

        private DialogueGraph Ferryman(GameFlags f)
        {
            // Aftermath, once resolved.
            if (f.GetBool("docks.ferryman_resolved"))
                return One("Ferryman", f.GetBool("docks.ferryman_saved")
                    ? "\"You're the one who jumped in without being asked. Old Pell's alive because of it — he tells the " +
                      "story bigger every day. Says you walked on the water. You didn't. But I won't be the one to spoil it.\""
                    : "\"...You. We pulled Pell out cold but breathing, no thanks to anyone with somewhere better to be. " +
                      "He lived. I just thought you'd want to know what your 'no time' nearly cost. Mind the wet boards.\"");

            var g = ScriptableObject.CreateInstance<DialogueGraph>();
            g.conversationId = "docks.ferryman"; g.startNodeId = "0";
            g.nodes.Add(new DialogueNode
            {
                id = "0", speaker = "Ferryman",
                text = "\"Help — please! My skiff's going under by the third piling and old Pell's still aboard, can't swim a " +
                       "stroke! The Fist won't come for a pauper and I can't haul him alone — *please!*\"",
                choices = new[]
                {
                    new DialogueChoice { text = "[Jump in.] Grab him — I've got the other side.", nextNodeId = "saved",
                        effects = new[] {
                            new FlagClause { key = "docks.ferryman_resolved", op = FlagOp.SetTrue },
                            new FlagClause { key = "docks.ferryman_saved", op = FlagOp.SetTrue },
                            new FlagClause { key = "reputation.lowcity", op = FlagOp.AddInt, amount = 2 } } },
                    new DialogueChoice { text = "I've no time for this. (Walk on.)", nextNodeId = "refused",
                        effects = new[] {
                            new FlagClause { key = "docks.ferryman_resolved", op = FlagOp.SetTrue },
                            new FlagClause { key = "reputation.lowcity", op = FlagOp.AddInt, amount = -1 } } },
                }
            });
            g.nodes.Add(new DialogueNode { id = "saved", speaker = "Ferryman",
                text = "You go in cold and graceless and get a fist in Pell's collar; between you the old man comes over the " +
                       "gunwale coughing river. The ferryman can't speak for shaking. The dockfolk saw. They always see who " +
                       "comes for the people the city writes off." });
            g.nodes.Add(new DialogueNode { id = "refused", speaker = "Ferryman",
                text = "\"...Aye. Of course. No one ever has.\" He turns back to the water alone. (Behind you, as you go, a " +
                       "splash and a younger voice swearing — someone else jumped. The Lower City keeps its own.)" });
            return g;
        }

        private static DialogueGraph One(string speaker, string text)
        {
            var g = ScriptableObject.CreateInstance<DialogueGraph>();
            g.conversationId = "docks." + speaker; g.startNodeId = "0";
            g.nodes.Add(new DialogueNode { id = "0", speaker = speaker, text = text });
            return g;
        }

        private GridUnit SpawnLeader(GridSystem grid, Vector2Int coord)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = leaderSheet.displayName; go.transform.SetParent(transform);
            go.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);
            go.GetComponent<Renderer>().material.color = Color.blue;
            var u = go.AddComponent<GridUnit>(); u.faction = Faction.Player; u.startCoord = coord; u.Sheet = leaderSheet;
            return u;
        }
        private Interactable MakeMarker(GridSystem grid, string name, Vector2Int coord, Color color)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = name; go.transform.SetParent(transform);
            go.transform.localScale = new Vector3(0.6f, 0.8f, 0.6f);
            go.GetComponent<Renderer>().material.color = color;
            var it = go.AddComponent<Interactable>(); it.label = name; it.coord = coord; it.Place(grid);
            return it;
        }
        private void MakeNpc(GridSystem grid, string name, Vector2Int coord, Color color, DialogueGraph graph)
        { var it = MakeMarker(grid, name, coord, color); it.kind = InteractionKind.Talk; it.dialogue = graph; }
        private void MakeExamine(GridSystem grid, string name, Vector2Int coord, string text)
        { var it = MakeMarker(grid, name, coord, new Color(0.5f, 0.5f, 0.55f)); it.kind = InteractionKind.Examine; it.examineText = text; }
        private void MakeExit(GridSystem grid, string name, Vector2Int coord, System.Action onExit)
        { var it = MakeMarker(grid, name, coord, new Color(0.5f, 0.5f, 0.6f)); it.kind = InteractionKind.Exit; it.onExit = onExit; }
    }
}
