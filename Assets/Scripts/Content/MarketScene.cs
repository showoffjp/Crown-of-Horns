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
    /// THE SHRUNKEN QUARTER — a second explorable Lower City room: a thin market that's seen better
    /// centuries. Flavor NPCs whose lines react to your reputation, a notice of the Returned, and a way
    /// back. Mirrors the other scene builders; set leaderSheet + onLeave, call Begin().
    /// </summary>
    public class MarketScene : MonoBehaviour
    {
        public CharacterSheet leaderSheet;
        public System.Action onLeave;

        public void Begin()
        {
            var f = GameFlags.Current;
            f.SetBool("lowcity.visited_market", true);
            SunderedCrown.UI.ExplorationHUD.Location = "The Shrunken Quarter";
            var gridGO = new GameObject("MarketGrid"); gridGO.transform.SetParent(transform);
            var grid = gridGO.AddComponent<GridSystem>();
            grid.width = 16; grid.height = 12; grid.tileWidth = 1f; grid.tileHeight = 0.5f; grid.Build();
            gridGO.AddComponent<TileFloorRenderer>();

            var cam = Camera.main;
            if (cam == null) { var g = new GameObject("Main Camera"); g.tag = "MainCamera"; cam = g.AddComponent<Camera>(); }
            cam.orthographic = true; cam.orthographicSize = 7f;
            cam.transform.position = grid.GridToWorld(8, 6) + new Vector3(0, 0, -10);
            cam.backgroundColor = new Color(0.12f, 0.11f, 0.09f);
            if (cam.GetComponent<SunderedCrown.CameraRig.IsometricCameraController>() == null)
                cam.gameObject.AddComponent<SunderedCrown.CameraRig.IsometricCameraController>();

            gameObject.AddComponent<DialogueScreen>().playerSpeaker = leaderSheet;
            gameObject.AddComponent<ExplorationHUD>();
            gameObject.AddComponent<JourneyScreen>();
            gameObject.AddComponent<RosterScreen>();
            var explore = gameObject.AddComponent<ExplorationController>();
            explore.worldCamera = cam;
            explore.Leader = SpawnLeader(grid, new Vector2Int(2, 6));

            MakeNpc(grid, "Mhaere's sister, a fishwife", new Vector2Int(7, 7), new Color(0.5f, 0.55f, 0.5f), Fishwife(f));
            MakeNpc(grid, "A bored Flaming Fist picket", new Vector2Int(11, 5), new Color(0.7f, 0.55f, 0.35f), Picket(f));
            MakeNpc(grid, f.GetBool("market.urchin_resolved") ? "A street child, watching the stalls" : "A Fist gripping a small thief",
                new Vector2Int(9, 3), new Color(0.6f, 0.5f, 0.4f), Urchin(f));
            MakeExamine(grid, "A weathered shrine to no one", new Vector2Int(5, 9),
                "A niche where a god's statue used to stand — pried out and sold in some lean year. The poor leave " +
                "offerings anyway: a coin, a crust, a child's drawing. To *nobody.* Just in case nobody is listening, " +
                "and lonely, and kind." +
                (f.GetBool("readers_boon")
                    ? "\n\n(You know better now. The empty niche is exactly the shape of a Lady who only watches from the " +
                      "margins — and these offerings, to no one, are the truest prayers in the Gate: kindness with no " +
                      "expectation of being seen. She sees them anyway.)"
                    : ""));
            if (f.GetBool("quest.choir.resolved"))
            {
                string slogan = f.GetBool("quest.choir.doubted")
                    ? "A chalked Faithless Choir slogan on the wall — THE WALL FALLS — has been amended, in a different, " +
                      "quieter hand, to read: THE WALL IS A CHOICE. WE GRIEVE BETTER. The preacher you argued into doubt has " +
                      "been busy. It is the most hopeful graffiti you have ever seen."
                    : f.GetBool("quest.choir.suppressed")
                        ? "A chalked Faithless Choir slogan — THE WALL FALLS — has been scrubbed at by the Fist and re-chalked " +
                          "overnight, harder each time. You silenced the Choir once. Silence, it turns out, is a thing that " +
                          "echoes. The wall is greasy with the argument."
                        : "A chalked Faithless Choir slogan glows faintly on the wall — THE WALL FALLS — and beneath it, a " +
                          "newer line in the same fervent hand: AND THE RETURNED WILL PULL THE FIRST STONE. They have decided " +
                          "you are theirs. You are not sure they are wrong.";
                MakeExamine(grid, "A chalked slogan on the wall", new Vector2Int(9, 9), slogan);
            }
            MakeExit(grid, "Back to the Lower City streets", new Vector2Int(2, 11), () => onLeave?.Invoke());
        }

        private DialogueGraph Fishwife(GameFlags f)
        {
            int rep = f.GetInt("reputation.lowcity");
            string line = f.GetBool("quest.widow.resolved")
                ? "\"You're the one who sat with my sister Mhaere over her boy. She sleeps now. Take a fish — no, take " +
                  "it, it's the only thanks the likes of us can make and have it mean anything.\""
                : rep >= 3
                    ? "\"The kind stranger. Word's all over the Quarter. Fish is half-price for you, and that's not " +
                      "charity, that's *respect,* so don't argue.\""
                    : "\"Fish! Fresh as the Chionthar gives, which isn't very, but it's honest. You buying or browsing, " +
                      "Returned? We don't get many of the *living* down here who aren't taking something.\"";
            return One("Fishwife", line);
        }

        private DialogueGraph Picket(GameFlags f)
        {
            string line = f.GetBool("quest.fist.freed")
                ? "\"...You're the one who talked Kallia out of an arrest. Half the company thinks you're trouble. The " +
                  "other half wishes you'd talk *us* out of these edicts too. Off the record.\""
                : f.GetBool("quest.fist.lawful")
                    ? "\"You're the one who let us take that beggar by the book. Tidy. (He won't quite look at you.) " +
                      "...Tidy's a funny word. Means clean. Doesn't mean right. Move along.\""
                    : "\"Flaming Fist business. Keep moving. (He's plainly bored half to death.) ...You're that Returned, " +
                      "aren't you. Do me a favour and don't start anything on my watch. I've nearly made it to pension.\"";
            return One("Fist Picket", line);
        }

        private DialogueGraph Urchin(GameFlags f)
        {
            if (f.GetBool("market.urchin_resolved"))
                return One("A street child", f.GetBool("market.urchin_freed")
                    ? "\"You're the one what got me loose from the Fist. I didn't even thank you proper, I just ran. " +
                      "(She produces a slightly-too-clean apple and offers it, fierce and shy.) ...I didn't nick this one. " +
                      "Bought it. With real coin. First time. Reckoned you'd want to know it took.\""
                    : "\"...You watched them take me. I did a night in the Seatower and a beating I'll feel in the cold for " +
                      "years. (She looks at you with the flat, ancient calm of a child who has stopped expecting better.) " +
                      "It's fine. I learned the lesson they wanted. Don't get caught. Don't expect saving.\"");

            var g = ScriptableObject.CreateInstance<DialogueGraph>();
            g.conversationId = "market.urchin"; g.startNodeId = "0";
            g.nodes.Add(new DialogueNode
            {
                id = "0", speaker = "Flaming Fist",
                text = "\"Caught this one's hand in a fishwife's purse. Edict says a finger for a first theft, or the Seatower " +
                       "cells.\" The child — eight, maybe nine, all elbows and terror — doesn't even struggle. They've done " +
                       "this before. \"Unless someone wants to pay the fishwife's loss and stand surety. You someone, Returned?\"",
                choices = new[]
                {
                    new DialogueChoice { text = "[Stand surety.] I'll cover it. Let the child go.", nextNodeId = "freed",
                        effects = new[] {
                            new FlagClause { key = "market.urchin_resolved", op = FlagOp.SetTrue },
                            new FlagClause { key = "market.urchin_freed", op = FlagOp.SetTrue },
                            new FlagClause { key = "reputation.lowcity", op = FlagOp.AddInt, amount = 2 } } },
                    new DialogueChoice { text = "The law's the law. (Let them take the child.)", nextNodeId = "taken",
                        effects = new[] {
                            new FlagClause { key = "market.urchin_resolved", op = FlagOp.SetTrue },
                            new FlagClause { key = "reputation.lowcity", op = FlagOp.AddInt, amount = -2 } } },
                }
            });
            g.nodes.Add(new DialogueNode { id = "freed", speaker = "Flaming Fist",
                text = "The picket shrugs, takes your coin, and lets go of a fistful of ragged tunic. The child is gone before " +
                       "you can speak — but at the mouth of the alley they stop, and look back at you, and do not run. They " +
                       "are *deciding something.* In the Lower City, that is how every good thing starts: a child, surprised " +
                       "by mercy, deciding the world might hold some after all." });
            g.nodes.Add(new DialogueNode { id = "taken", speaker = "Flaming Fist",
                text = "\"Sensible.\" He marches the child off toward the Seatower. They don't cry. That's the part that stays " +
                       "with you — not tears, just the small flat face of someone learning, on schedule, exactly what the " +
                       "city thinks they're worth. The fishwife won't meet your eye. She has children too." });
            return g;
        }

        private static DialogueGraph One(string speaker, string text)
        {
            var g = ScriptableObject.CreateInstance<DialogueGraph>();
            g.conversationId = "market." + speaker; g.startNodeId = "0";
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
