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
    /// THE ALMSHOUSE OF THE UNCLAIMED — a second explorable Lower City room: a refuge where the Gate's
    /// godless poor shelter, run by Mother Cass. Everything here *reacts* to your Act II run (the tithe,
    /// the Choir, the widow, your standing) — read live from the flags each time you enter. A wall of the
    /// unclaimed dead's names quietly restates the game's central question. Mirrors the other scene
    /// builders (own grid/camera/exploration UI, parented for one-Destroy teardown). Set fields, call Begin().
    /// </summary>
    public class AlmshouseScene : MonoBehaviour
    {
        public CharacterSheet leaderSheet;
        public System.Action onLeave;

        public void Begin()
        {
            var f = GameFlags.Current;
            f.SetBool("lowcity.visited_almshouse", true);
            SunderedCrown.UI.ExplorationHUD.Location = "The Almshouse of the Unclaimed";

            var gridGO = new GameObject("AlmshouseGrid");
            gridGO.transform.SetParent(transform);
            var grid = gridGO.AddComponent<GridSystem>();
            grid.width = 16; grid.height = 12; grid.tileWidth = 1f; grid.tileHeight = 0.5f;
            grid.Build();
            gridGO.AddComponent<TileFloorRenderer>();

            var cam = Camera.main;
            if (cam == null) { var g = new GameObject("Main Camera"); g.tag = "MainCamera"; cam = g.AddComponent<Camera>(); }
            cam.orthographic = true; cam.orthographicSize = 7f;
            cam.transform.position = grid.GridToWorld(8, 6) + new Vector3(0, 0, -10);
            cam.backgroundColor = new Color(0.10f, 0.09f, 0.08f); // warm dark — candlelight, not the grey of the Wall
            if (cam.GetComponent<SunderedCrown.CameraRig.IsometricCameraController>() == null)
                cam.gameObject.AddComponent<SunderedCrown.CameraRig.IsometricCameraController>();

            var dlg = gameObject.AddComponent<DialogueScreen>();
            dlg.playerSpeaker = leaderSheet;
            gameObject.AddComponent<ExplorationHUD>();
            gameObject.AddComponent<JourneyScreen>();
            gameObject.AddComponent<RosterScreen>();
            var explore = gameObject.AddComponent<ExplorationController>();
            explore.worldCamera = cam;
            explore.Leader = SpawnLeader(grid, new Vector2Int(2, 6));

            // Mother Cass — the keeper, reactive to how you've treated the quarter.
            MakeNpc(grid, "Mother Cass, the almshouse keeper", new Vector2Int(8, 7),
                new Color(0.8f, 0.7f, 0.45f), Keeper(f));

            // The huddle of the godless poor — their mood is your reputation, made flesh.
            MakeNpc(grid, "A huddle of the unclaimed", new Vector2Int(11, 4),
                new Color(0.55f, 0.5f, 0.45f), Poor(f));

            // Roen, if he's with you — the Outer City orphan, back in the kind of room that made him.
            if (f.GetBool("companion.roen.recruited") && PartyHas("Roen"))
                MakeNpc(grid, "Roen — back in the gutter that made him", new Vector2Int(3, 4),
                    new Color(0.45f, 0.7f, 0.55f), RoenWitness(f));

            // A dying man on a cot — an intimate mercy-or-truth beat (no reputation, just your conscience).
            MakeNpc(grid, f.GetBool("almshouse.deathbed_resolved") ? "A freshly-tended cot, its blanket folded" : "A dying man on a cot",
                new Vector2Int(12, 8), new Color(0.6f, 0.55f, 0.5f), Deathbed(f));

            // The Wall of Names — the quarter's own answer to the game's question.
            string wall =
                "Not stone — a plastered wall, every inch crowded with names in a hundred hands: the unclaimed dead the " +
                "quarter refuses to let the Wall of the Faithless erase. Some are fresh. One chalked low, in a child's " +
                "letters, reads simply: MA, WHO SANG. No god kept them. The living do. It is the whole argument of your " +
                "life, written by people who never heard your name.";
            if (f.GetBool("readers_boon"))
                wall += "\n\n(You understand, now, that the Wall of the Faithless and this wall are the same wall, read " +
                        "two ways. One erases; one remembers. The only difference is who holds the chalk — and whether " +
                        "they bother. You have spent the whole saga learning to bother.)";
            else if (f.GetBool("lowcity.allies") || f.GetInt("reputation.lowcity") >= 3)
                wall += "\n\n(Near the bottom, in a hand you don't recognize, someone has begun a new column — and left " +
                        "the first space blank, waiting. For the day the Returned needs remembering too, they say. They " +
                        "mean it kindly. It is the kindest threat you have ever been paid.)";
            MakeExamine(grid, "The Wall of Names", new Vector2Int(5, 9), wall);

            MakeExit(grid, "Back to the Lower City streets", new Vector2Int(2, 11), () => onLeave?.Invoke());
        }

        // ---- reactive conversations ----

        private static DialogueGraph One(string speaker, string text)
        {
            var g = ScriptableObject.CreateInstance<DialogueGraph>();
            g.conversationId = "almshouse." + speaker; g.startNodeId = "0";
            g.nodes.Add(new DialogueNode { id = "0", speaker = speaker, text = text });
            return g;
        }

        private DialogueGraph Deathbed(GameFlags f)
        {
            if (f.GetBool("almshouse.deathbed_resolved"))
                return One("Mother Cass", f.GetBool("almshouse.deathbed_lie")
                    ? "\"Old Hensley went in the night — easy, smiling. Whatever you told him about his boy, he carried it " +
                      "out like a lamp. I don't ask whether it was true. I've buried too many to think true is the only " +
                      "thing worth giving a dying man.\""
                    : "\"Hensley passed hard, after — wouldn't settle. You gave him the truth and the truth had teeth. " +
                      "(She doesn't say it unkindly.) ...You weren't wrong to. I only ever wonder, with the dying, whether " +
                      "right is the same as kind. I've never once been sure.\"");

            var g = ScriptableObject.CreateInstance<DialogueGraph>();
            g.conversationId = "almshouse.deathbed"; g.startNodeId = "0";
            g.nodes.Add(new DialogueNode
            {
                id = "0", speaker = "Hensley, dying",
                text = "An old man, hours from the end, grips your hand with a strength that surprises you. \"You've an honest " +
                       "face. Tell me true — did my boy ever ask after me? Tomas. I drove him off, forty years gone, and I'd " +
                       "give the rest of my breath to know he... that he didn't hate me at the end.\" (Mother Cass, behind you, " +
                       "gives the smallest shake of her head. Tomas never came. Tomas never wrote. There is no record of mercy here.)",
                choices = new[]
                {
                    new DialogueChoice { text = "\"He asked after you. He forgave you. He told me so.\" (A lie, to ease him out.)",
                        nextNodeId = "lie",
                        effects = new[] {
                            new FlagClause { key = "almshouse.deathbed_resolved", op = FlagOp.SetTrue },
                            new FlagClause { key = "almshouse.deathbed_lie", op = FlagOp.SetTrue } } },
                    new DialogueChoice { text = "\"I won't lie to a dying man. He never came. But you *asked* — that counts.\"",
                        nextNodeId = "truth",
                        effects = new[] { new FlagClause { key = "almshouse.deathbed_resolved", op = FlagOp.SetTrue } } },
                }
            });
            g.nodes.Add(new DialogueNode { id = "lie", speaker = "Hensley, dying",
                text = "Something in his face comes untied — forty years of held breath, let go. \"He did. Of course he did. " +
                       "He was always the better of us.\" He is asleep before you finish, smiling. You will never know if it " +
                       "was the cruelest kindness or the kindest cruelty. Mother Cass squeezes your shoulder. She has made " +
                       "this choice a thousand times. She does not tell you which way." });
            g.nodes.Add(new DialogueNode { id = "truth", speaker = "Hensley, dying",
                text = "He weeps, which you feared. But then — quieter — \"...I *asked,* though. After all this. That's not " +
                       "nothing, is it. To still be the kind of man who'd want to know.\" He holds that, instead of a comfort " +
                       "that wasn't his. It is a harder thing to die holding. He dies holding it anyway, and it is *his.*" });
            return g;
        }

        private DialogueGraph Keeper(GameFlags f)
        {
            int rep = f.GetInt("reputation.lowcity");
            bool warm = f.GetBool("lowcity.allies") || f.GetBool("quest.tithe.freed") || f.GetBool("quest.tithe.paid") ||
                        f.GetBool("quest.choir.doubted") || f.GetBool("quest.widow.hope") || rep >= 3;
            bool cold = f.GetBool("quest.tithe.corrupt") || rep <= -2;

            var g = ScriptableObject.CreateInstance<DialogueGraph>();
            g.conversationId = "almshouse.keeper"; g.startNodeId = "0";

            string open =
                warm ? "Returned. We don't get many of the living down here who aren't taking something. You gave — the " +
                       "grave-tithe broken, or the poor's dead blessed, or a kind word where the Doom gives none. Word " +
                       "runs ahead of you in here like warmth down a cold corridor. Sit. You're welcome at this fire."
              : cold ? "...You. We heard. The one who found a *price* in the dead, same as the temples, same as Vane. " +
                       "You can come in — we turn no one from the Almshouse, that's the whole point of it — but don't " +
                       "expect the children to smile at you. They've a long memory for who sold them."
              : "A stranger, and a living one. We're the Almshouse of the Unclaimed — the godless, the priced-out, the " +
                "ones the Doom would call Faithless and the temples would call *arrears.* We keep each other. It's not " +
                "much. It's everything we have.";

            g.nodes.Add(new DialogueNode
            {
                id = "0", speaker = "Mother Cass", text = open,
                onEnter = warm
                    ? new[] { new FlagClause { key = "almshouse.visited", op = FlagOp.SetTrue },
                              new FlagClause { key = "almshouse.blessing", op = FlagOp.SetTrue } }
                    : new[] { new FlagClause { key = "almshouse.visited", op = FlagOp.SetTrue } },
                choices = new[]
                {
                    new DialogueChoice { text = "Who do you keep here?", nextNodeId = "who" },
                    new DialogueChoice { text = "What do you need?", nextNodeId = "need" },
                }
            });

            g.nodes.Add(Line("who", "Mother Cass",
                "Whoever the Wall would take and the temples won't bury. Dockers, beggars, a fallen priest or two, a " +
                "tiefling girl who throws fire and won't say her name. We don't ask what god you knelt to. We ask if " +
                "you're cold. That's the only catechism that ever fed anyone.", "0"));

            g.nodes.Add(new DialogueNode
            {
                id = "need", speaker = "Mother Cass",
                text = warm
                    ? "From you? You've already done it — you made the quarter believe a powerful thing could be *kind.* " +
                      "That belief is bread, down here. Take this." + " (She presses a worn Kelemvor token into your " +
                      "hand — turned backwards, the scales facing in.) \"For when you reach the place that judges. So it " +
                      "knows the poor sent someone who spoke for us.\""
                    : "Nothing you'd give cheaply, and I'll not beg it. Just — when you reach whatever judges at the end " +
                      "of all this, and the likes of us aren't in the room, remember we were real. That's the ask. " +
                      "That's the only ask the unclaimed ever have.",
                onEnter = warm
                    ? new[] { new FlagClause { key = "almshouse.token", op = FlagOp.SetTrue } }
                    : null,
            });
            return g;
        }

        private DialogueGraph Poor(GameFlags f)
        {
            var g = ScriptableObject.CreateInstance<DialogueGraph>();
            g.conversationId = "almshouse.poor"; g.startNodeId = "0";

            string line =
                f.GetBool("almshouse.token")
                    ? "(An old woman presses her thumb to your wrist, where Mother Cass's backwards token rides.) \"You're " +
                      "carrying us. To the place that judges. No one's ever carried us anywhere but the pit.\" (Her grip " +
                      "tightens, fierce.) \"When you stand before whatever weighs souls — say our names slow. Make it *listen.*\""
              : f.GetBool("quest.tithe.freed") || f.GetBool("quest.tithe.paid")
                    ? "(A man with grave-dirt under his nails nods at you.) \"My boy's got a stone with his name on it now, " +
                      "because of you. A *name.* You know what that's worth, to people the world keeps a pit for?\""
              : f.GetBool("quest.tithe.corrupt")
                    ? "(They go quiet as you approach, the way the poor go quiet around the Fist.) \"...It's him. The one " +
                      "with Vane.\" A child is pulled behind a skirt. No one meets your eye."
              : f.GetBool("quest.choir.favored")
                    ? "(A woman with fevered eyes grips your sleeve.) \"You *spoke* for the Unmade. You'll tear the Wall " +
                      "down, won't you? Promise me. Promise me my sister comes back.\" (It is not a comfortable hope to " +
                      "have caused.)"
              : "(A circle of the godless poor, sharing one thin candle.) \"Sit if you like. We've not much, but the cold's " +
                "easier shared. That's the only magic anyone ever proved to me — and it's never once failed.\"";

            g.nodes.Add(new DialogueNode { id = "0", speaker = "The unclaimed", text = line });
            return g;
        }

        private static bool PartyHas(string nameMatch)
        {
            var p = Party.Instance;
            if (p == null) return false;
            foreach (var m in p.active)
                if (m?.displayName != null && m.displayName.Contains(nameMatch)) return true;
            return false;
        }

        /// <summary>Roen, the Outer City orphan, standing in the kind of room he came out of — reactive to
        /// how his personal quest (his sister Wrenna) resolved. Completes the cast's "home" beats.</summary>
        private DialogueGraph RoenWitness(GameFlags f)
        {
            var g = ScriptableObject.CreateInstance<DialogueGraph>();
            g.conversationId = "almshouse.roen_witness"; g.startNodeId = "0";

            g.nodes.Add(new DialogueNode
            {
                id = "0", speaker = "Roen",
                text = "Funny. I've cracked mansions, a palace, a god's own counting-house. But this — a room full of " +
                       "people the world forgot — this is the only kind of place I've ever walked into and felt my " +
                       "shoulders come *down.* I grew up in a room like this. Might've died in one, if the Harpers " +
                       "hadn't picked me out of it.",
                onEnter = new[] { new FlagClause { key = "almshouse.roen_witnessed", op = FlagOp.SetTrue },
                                  new FlagClause { key = "companion.roen.approval", op = FlagOp.AddInt, amount = 5 } },
                choices = new[] { new DialogueChoice { text = "You're not that kid anymore.", nextNodeId = "close" },
                                  new DialogueChoice { text = "These are still your people.", nextNodeId = "close" } }
            });

            string close =
                f.GetBool("quest.roen.wrenna_saved")
                    ? "Wrenna would've ended up in a room like this, if her lie hadn't held. We both came out of one. I " +
                      "let her keep her mercy — let her be more than the rules. Standing here, with these faces? I'm " +
                      "glad I did. Gods help me, I'm actually glad."
              : f.GetBool("quest.roen.double_agent")
                    ? "I turned my own sister into a blade in Kelemvor's house. Clever. Cold. And I learned cold in a " +
                      "room exactly like this one, from people exactly this tired. Don't know if I'm proud of the lesson " +
                      "or sick to my teeth of it. Both, probably. It's always both with me."
              : f.GetBool("quest.roen.harper_boon")
                    ? "I handed Wrenna to the Harpers. For the cause. (He won't look at the children.) Tell that to a " +
                      "room full of people the cause never once came for. ...Let's not stay long. I can't be the person " +
                      "I was in here and the person who did that. Not in the same room."
              : "There's a cipher in my pocket I keep not decoding — a safehouse, a name from before all this. Standing " +
                "in the room that made me, I think I'm finally ready to pull that thread. Or I'm terrified to. With me, " +
                "as you've learned, it is reliably both.";
            g.nodes.Add(new DialogueNode { id = "close", speaker = "Roen", text = close });
            return g;
        }

        private static DialogueNode Line(string id, string speaker, string text, string next)
            => new DialogueNode { id = id, speaker = speaker, text = text,
                choices = new[] { new DialogueChoice { text = "(go on)", nextNodeId = next } } };

        // ---- helpers (mirror the other scene builders) ----
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
            var it = MakeMarker(grid, name, coord, new Color(0.5f, 0.5f, 0.55f));
            it.kind = InteractionKind.Examine; it.examineText = text;
        }

        private void MakeExit(GridSystem grid, string name, Vector2Int coord, System.Action onExit)
        {
            var it = MakeMarker(grid, name, coord, new Color(0.5f, 0.5f, 0.6f));
            it.kind = InteractionKind.Exit; it.onExit = onExit;
        }
    }
}
