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
    /// THE WANDERING NICHE — a hidden Harper safehouse above a Lower City taproom, open once Roen is in the
    /// company. A wary Harper handler whose lines react to Roen's personal quest, a cipher-board for the
    /// agents who pass through, and a quiet corner that is, unmistakably, Roen's. Mirrors the other scene
    /// builders; set leaderSheet + onLeave, call Begin().
    /// </summary>
    public class HarperSafehouseScene : MonoBehaviour
    {
        public CharacterSheet leaderSheet;
        public System.Action onLeave;

        public void Begin()
        {
            var f = GameFlags.Current;
            f.SetBool("lowcity.visited_safehouse", true);
            SunderedCrown.UI.ExplorationHUD.Location = "The Wandering Niche";
            var gridGO = new GameObject("SafehouseGrid"); gridGO.transform.SetParent(transform);
            var grid = gridGO.AddComponent<GridSystem>();
            grid.width = 14; grid.height = 11; grid.tileWidth = 1f; grid.tileHeight = 0.5f; grid.Build();
            gridGO.AddComponent<TileFloorRenderer>();

            var cam = Camera.main;
            if (cam == null) { var g = new GameObject("Main Camera"); g.tag = "MainCamera"; cam = g.AddComponent<Camera>(); }
            cam.orthographic = true; cam.orthographicSize = 6.5f;
            cam.transform.position = grid.GridToWorld(7, 5) + new Vector3(0, 0, -10);
            cam.backgroundColor = new Color(0.10f, 0.10f, 0.13f);
            if (cam.GetComponent<SunderedCrown.CameraRig.IsometricCameraController>() == null)
                cam.gameObject.AddComponent<SunderedCrown.CameraRig.IsometricCameraController>();

            gameObject.AddComponent<DialogueScreen>().playerSpeaker = leaderSheet;
            gameObject.AddComponent<ExplorationHUD>();
            gameObject.AddComponent<JourneyScreen>();
            gameObject.AddComponent<RosterScreen>();
            var explore = gameObject.AddComponent<ExplorationController>();
            explore.worldCamera = cam;
            explore.Leader = SpawnLeader(grid, new Vector2Int(2, 5));

            MakeNpc(grid, "A wary Harper handler", new Vector2Int(7, 6), new Color(0.45f, 0.5f, 0.6f), Handler(f));
            MakeNpc(grid, f.GetBool("safehouse.informant_resolved") ? "An empty chair, the ropes cut" : "A bound informant, watched by Harpers",
                new Vector2Int(11, 7), new Color(0.5f, 0.45f, 0.5f), Informant(f));
            MakeExamine(grid, "A cipher-board of pinned notes", new Vector2Int(5, 8),
                "A corkboard of pinned slips in a dozen ciphers — drop-points, watchwords, the small secret weather of a " +
                "spy network. One pin holds a blank slip. The Harpers leave one always, they say: a place for the message " +
                "you can't yet bear to write down." +
                (f.GetBool("readers_boon")
                    ? "\n\n(You look at the blank slip a long moment. You know, now, who reads the margins — who keeps the " +
                      "unwritten things. You take the pencil. You do not write a watchword. You write, very small, in the " +
                      "white space: *I know you're there. Thank you for staying.* You leave it pinned. Somewhere, the margin smiles.)"
                    : ""));
            MakeExamine(grid, "A quiet corner with two chairs", new Vector2Int(10, 4),
                f.GetBool("romance.roen.consummated")
                    ? "Two chairs by the window, angled together. A brass key sits on the sill — to an inn that doesn't " +
                      "exist yet. Roen keeps the corner exactly so. You know precisely who the second chair is for."
                    : "A worn corner with two mismatched chairs and a deck of cards mid-game against no one. It is, " +
                      "unmistakably, Roen's — the spot a man picks when he's deciding whether to let himself stay.");
            MakeExit(grid, "Back down to the taproom", new Vector2Int(2, 10), () => onLeave?.Invoke());
        }

        private DialogueGraph Handler(GameFlags f)
        {
            if (f.GetBool("romance.roen.consummated"))
                return One("Harper Handler",
                    "\"...Oh. *Oh.* You're the one. The reason Roen Alleywind, the most slippery agent this lodge ever " +
                    "trained, started turning down deep-cover jobs because they'd take him 'away.' Twenty years he never " +
                    "once said that word. The whole Niche has a wager on you, you know. Be good to him. He's terrible at " +
                    "being happy and we'd all like to see him get the hang of it.\"");
            string line = f.GetBool("quest.roen.double_agent")
                ? "\"You're the one who turned Wrenna into our blade in Kelemvor's house. Bold. Half of us think it'll get " +
                  "Roen killed; the other half are quietly impressed. Welcome to the Niche. Touch nothing, read everything.\""
                : f.GetBool("quest.roen.wrenna_saved")
                    ? "\"Roen chose his sister over the rules, with you at his shoulder. The Harpers don't forgive that " +
                      "easily — but they don't forget who walked him through it, either. You've a chair here. Mind it's not a trap.\""
                    : f.GetBool("quest.roen.harper_boon")
                        ? "\"Roen turned his own sister in. Clean. Correct. The lodge is grateful and Roen hasn't smiled in a " +
                          "tenday. Funny, how often those two go together in this work. You did that with him. Sit, if you can bear to.\""
                        : "\"Friend of Alleywind's, then. The Niche keeps no locks for the people Roen vouches for — and a very " +
                          "long memory for the ones he doesn't. You're the first kind. So far.\"";
            return One("Harper Handler", line);
        }

        private DialogueGraph Informant(GameFlags f)
        {
            if (f.GetBool("safehouse.informant_resolved"))
                return One("Harper Handler", f.GetBool("safehouse.informant_freed")
                    ? "\"You let the Fist's snitch walk. Half the lodge called it soft. Then she came back — *turned* — and " +
                      "handed us the Fist's whole dockside ledger, because you were the first person on either side who " +
                      "treated her like she had a choice. Mercy's a long con, it turns out. You'd have made a fine Harper.\""
                    : "\"The snitch you gave us gave up three drops and a courier before the week was out. Clean work. " +
                      "(The handler doesn't smile.) She also gave up a name she shouldn't have known, and a family paid for " +
                      "it. That's the trade. We don't pretend it isn't. You shouldn't either.\"");

            var g = ScriptableObject.CreateInstance<DialogueGraph>();
            g.conversationId = "safehouse.informant"; g.startNodeId = "0";
            g.nodes.Add(new DialogueNode
            {
                id = "0", speaker = "Harper Handler",
                text = "A woman sits bound to a chair, jaw set, eyes flicking. \"Caught her selling our drop-points to the " +
                       "Flaming Fist. Roen vouches for your judgement, so — your call. We can *turn* her, lean hard till she " +
                       "spies for us instead. Or you can argue for cutting her loose. I'll warn you: loose, she might run " +
                       "straight back to the Fist. People mostly do.\"",
                choices = new[]
                {
                    new DialogueChoice { text = "Turn her. A frightened asset is still an asset.", nextNodeId = "turned",
                        effects = new[] {
                            new FlagClause { key = "safehouse.informant_resolved", op = FlagOp.SetTrue },
                            new FlagClause { key = "safehouse.informant_turned", op = FlagOp.SetTrue } } },
                    new DialogueChoice { text = "Cut her loose. Give her the choice no one's given her.", nextNodeId = "freed",
                        effects = new[] {
                            new FlagClause { key = "safehouse.informant_resolved", op = FlagOp.SetTrue },
                            new FlagClause { key = "safehouse.informant_freed", op = FlagOp.SetTrue } } },
                }
            });
            g.nodes.Add(new DialogueNode { id = "turned", speaker = "Harper Handler",
                text = "It works, the way leaning on the frightened works. She'll spy for you now, and hate you, and be useful, " +
                       "and the work will get done. The handler nods, satisfied. You watch the woman's face close like a door " +
                       "and understand, a little better than you wanted to, how the Wall got built one reasonable decision at a time." });
            g.nodes.Add(new DialogueNode { id = "freed", speaker = "Harper Handler",
                text = "The handler grimaces but cuts the ropes — Roen's name carries weight. The woman rubs her wrists, " +
                       "stares at you like you've handed her a live coal, and walks out into the dark unowned by anyone for " +
                       "the first time in years. Maybe she runs back to the Fist. Maybe she doesn't. You gave her the one " +
                       "thing the Wall never gives: the dignity of being allowed to decide." });
            return g;
        }

        private static DialogueGraph One(string speaker, string text)
        {
            var g = ScriptableObject.CreateInstance<DialogueGraph>();
            g.conversationId = "safehouse." + speaker; g.startNodeId = "0";
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
