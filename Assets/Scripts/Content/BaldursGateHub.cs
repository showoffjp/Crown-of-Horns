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
    /// Builds a small explorable slice of Baldur's Gate (the Lower City near a sealed
    /// stair into the Cinderhaunt). Spawns the leader, an NPC herald who gives the first
    /// task, a point of interest to examine, and a dungeon exit that hands control back
    /// to the director to start the fight. All objects are parented to this component so
    /// the director can tear the hub down in one Destroy.
    ///
    /// Set the public fields, then call Begin().
    /// </summary>
    public class BaldursGateHub : MonoBehaviour
    {
        public CharacterSheet leaderSheet;
        public System.Action onEnterDungeon;
        public System.Action onEnterNetheril;
        public System.Action onEnterCrownWars;
        public System.Action onEnterTimeOfTroubles;
        public System.Action onEnterSpellplague;
        public System.Action onEnterFugue;
        public System.Action onEnterFinale;
        public System.Action onEnterCamp;
        public System.Action<string> onEnterPersonalQuest;
        public System.Action onEnterChoirCell;
        public System.Action onEnterAlmshouse;
        public System.Action onEnterShop;
        public System.Action onEnterMarket;
        public System.Action onEnterDocks;
        public System.Action onEnterSafehouse;
        public DialogueGraph aldricDialogue;
        public DialogueGraph tallyDialogue;
        public DialogueGraph tallyRoamingDialogue;
        public ActTwoContent actTwo;
        public SwordCoastContent content;

        public void Begin()
        {
            // Grid.
            var gridGO = new GameObject("HubGrid");
            gridGO.transform.SetParent(transform);
            var grid = gridGO.AddComponent<GridSystem>();
            grid.width = 20; grid.height = 14; grid.tileWidth = 1f; grid.tileHeight = 0.5f;
            grid.Build();
            gridGO.AddComponent<SunderedCrown.Rendering.TileFloorRenderer>();

            // Camera (persistent, reused across modes).
            var cam = Camera.main;
            if (cam == null) { var g = new GameObject("Main Camera"); g.tag = "MainCamera"; cam = g.AddComponent<Camera>(); }
            cam.orthographic = true; cam.orthographicSize = 7f;
            cam.transform.position = grid.GridToWorld(10, 7) + new Vector3(0, 0, -10);
            if (cam.GetComponent<SunderedCrown.CameraRig.IsometricCameraController>() == null)
                cam.gameObject.AddComponent<SunderedCrown.CameraRig.IsometricCameraController>();

            // Dialogue + exploration UI.
            var dlg = gameObject.AddComponent<DialogueScreen>();
            dlg.playerSpeaker = leaderSheet;
            gameObject.AddComponent<ExplorationHUD>();
            gameObject.AddComponent<InventoryScreen>(); // press I
            gameObject.AddComponent<JourneyScreen>();   // press J — the quest tracker
            gameObject.AddComponent<RosterScreen>();    // press P — bench/field companions
            gameObject.AddComponent<CodexScreen>();     // press K — the lore/bestiary compendium
            gameObject.AddComponent<AmbientBanter>();   // idle party chatter (mute with B)
            var explore = gameObject.AddComponent<ExplorationController>();
            explore.worldCamera = cam;

            // Leader token.
            explore.Leader = SpawnLeader(grid, new Vector2Int(3, 7));

            // NPC: the Herald (quest giver).
            MakeNpc(grid, "Herald of the Gate", new Vector2Int(8, 8), new Color(0.85f, 0.75f, 0.3f), HeraldDialogue());

            // NPC: Roen Alleywind, a Harper you can recruit (until he's joined).
            if (!GameFlags.Current.GetBool("companion.roen.recruited"))
                MakeNpc(grid, "Roen Alleywind", new Vector2Int(12, 5), new Color(0.3f, 0.6f, 0.4f), RoenDialogue());

            // NPC: Varra, a tiefling warlock you can recruit (until she's joined).
            if (!GameFlags.Current.GetBool("companion.varra.recruited") && !GameFlags.Current.GetBool("companion.varra.lost"))
                MakeNpc(grid, "Varra (a tiefling warlock)", new Vector2Int(13, 3), new Color(0.7f, 0.25f, 0.3f), VarraDialogue());

            // Once you've cleared the Cinderhaunt, you can descend into the Fugue & the Wall (Act II).
            if (onEnterFugue != null && GameFlags.Current.GetBool("prologue.cleared"))
            {
                var descent = MakeMarker(grid, "Descend into the Fugue (the Wall of the Faithless)", new Vector2Int(4, 11),
                    new Color(0.45f, 0.45f, 0.5f));
                descent.kind = InteractionKind.Exit;
                descent.onExit = onEnterFugue;
            }

            // The Court of the Dead — the finale. Opens only once you've walked at least one era of
            // history, so it reads as a culmination rather than a shortcut. The deeper, golden endings
            // unlock further as you understand more (the Reader's Boon, the verdict, your true name).
            if (onEnterFinale != null && GameFlags.Current.GetBool("netheril.cleared"))
            {
                var court = MakeMarker(grid, "The Court of the Dead (end the saga)", new Vector2Int(8, 11),
                    new Color(0.55f, 0.4f, 0.65f));
                court.kind = InteractionKind.Exit;
                court.onExit = onEnterFinale;
            }

            // A campfire on the edge of the Lower City — rest, and let the party open up.
            if (onEnterCamp != null)
            {
                var camp = MakeMarker(grid, "Campfire (rest & talk)", new Vector2Int(3, 11),
                    new Color(0.85f, 0.5f, 0.25f));
                camp.kind = InteractionKind.Exit;
                camp.onExit = onEnterCamp;
            }

            // Companion personal quests — each playable thread opens once you've examined its cipher hook,
            // and closes once you've made the call. Placed along the west edge so they read as a "to-do".
            if (onEnterPersonalQuest != null)
            {
                int row = 5;
                foreach (var hook in CompanionQuests.Hooks)
                {
                    if (!hook.playable) continue;
                    if (!GameFlags.Current.GetBool(hook.startedFlag) || GameFlags.Current.GetBool(hook.resolvedFlag)) continue;
                    var thread = MakeMarker(grid, hook.followLabel, new Vector2Int(2, row),
                        new Color(0.4f, 0.6f, 0.45f));
                    thread.kind = InteractionKind.Exit;
                    string qid = hook.id; // capture
                    thread.onExit = () => onEnterPersonalQuest(qid);
                    row += 3;
                }
            }

            // Point of interest: the proclamation board — reactive to how the city now sees you.
            MakeExamine(grid, "Proclamation Board", new Vector2Int(6, 5), ProclamationText());

            // A balladeer's broadside — the city mythologizing the Returned as your legend grows.
            if (GameFlags.Current.GetBool("netheril.cleared"))
                MakeExamine(grid, "A balladeer's broadside", new Vector2Int(8, 5), BalladeerText());

            // Gawking onlookers — the Gate's nervous reaction to your strange company.
            MakeExamine(grid, "A knot of gawking onlookers", new Vector2Int(10, 6), OnlookersText());

            // A Doomguide of Kelemvor — appears once the church has an opinion of you (faction rep ≠ 0).
            if (GameFlags.Current.GetInt("faction.kelemvor.reputation") != 0)
                MakeExamine(grid, "A watchful Doomguide of Kelemvor", new Vector2Int(12, 6), DoomguideText());

            // A hooded Choir-sympathizer — appears once the Faithless Choir has an opinion of you.
            if (GameFlags.Current.GetInt("faction.choir.reputation") != 0)
                MakeExamine(grid, "A hooded figure marked with the Choir's ash", new Vector2Int(13, 8), ChoirText());

            // Lootable strongbox (gold + a potion + the dungeon key).
            if (content != null)
                MakeContainer(grid, "Strongbox", new Vector2Int(5, 9), 20,
                    new List<ItemDefinition> { content.Items["healing_potion"], content.Items["cinderhaunt_key"] });

            // Exit: stairs into the Cinderhaunt dungeon.
            MakeExit(grid, "Cinderhaunt Stairs", new Vector2Int(16, 7), onEnterDungeon);

            // After the Cinderhaunt, the crack in your soul opens onto another age: a shimmering
            // rift to Netheril, the falling city (Act III, era 1).
            if (onEnterNetheril != null && GameFlags.Current.GetBool("prologue.cleared"))
            {
                var rift = MakeMarker(grid, "The Skip — a shimmering rift", new Vector2Int(16, 11),
                    new Color(0.45f, 0.35f, 0.8f));
                rift.kind = InteractionKind.Exit;
                rift.onExit = onEnterNetheril;
            }

            // A second, deeper rift opens once you've walked Netheril — to the Crown Wars (era 2).
            if (onEnterCrownWars != null && GameFlags.Current.GetBool("netheril.cleared"))
            {
                var rift2 = MakeMarker(grid, "The Deep Skip — a darker rift", new Vector2Int(16, 3),
                    new Color(0.3f, 0.5f, 0.45f));
                rift2.kind = InteractionKind.Exit;
                rift2.onExit = onEnterCrownWars;
            }

            // After the Crown Wars: a rift to the Time of Troubles (era 3 — the forging of the Crown).
            if (onEnterTimeOfTroubles != null && GameFlags.Current.GetBool("crownwars.cleared"))
            {
                var rift3 = MakeMarker(grid, "The God-Wound Rift (Time of Troubles)", new Vector2Int(14, 3),
                    new Color(0.6f, 0.4f, 0.25f));
                rift3.kind = InteractionKind.Exit;
                rift3.onExit = onEnterTimeOfTroubles;
            }

            // After the Time of Troubles: a rift into the Spellplague (era 4 — where the ink runs).
            if (onEnterSpellplague != null && GameFlags.Current.GetBool("act4.toot_done"))
            {
                var rift4 = MakeMarker(grid, "The Blue-Fire Tear (Spellplague)", new Vector2Int(18, 3),
                    new Color(0.3f, 0.5f, 0.9f));
                rift4.kind = InteractionKind.Exit;
                rift4.onExit = onEnterSpellplague;
            }

            // Aldric Morn comes to the Gate once you've cleared the Cinderhaunt — "Tea with a Heretic."
            if (aldricDialogue != null && GameFlags.Current.GetBool("prologue.cleared"))
                MakeNpc(grid, "Aldric Morn (the tea is on)", new Vector2Int(10, 4),
                    new Color(0.82f, 0.72f, 0.5f), aldricDialogue);

            // Tally, a one-eyed storyteller, is always at the fire — the Lady in the Margins' guise.
            // Hear her out to enter the Vault of Tens.
            if (tallyDialogue != null)
                MakeNpc(grid, "Tally (a one-eyed storyteller)", new Vector2Int(6, 3),
                    new Color(0.7f, 0.55f, 0.85f), tallyDialogue);

            // After you've walked an era, she pops in with a one-off roaming riddle (G-Man style).
            if (tallyRoamingDialogue != null && GameFlags.Current.GetBool("netheril.cleared") &&
                !GameFlags.Current.GetBool("tally.roaming1.done"))
                MakeNpc(grid, "Tally (between the pages)", new Vector2Int(8, 5),
                    new Color(0.75f, 0.6f, 0.9f), tallyRoamingDialogue);

            // Companion personal-quest hooks: once a campfire night-talk has opened someone up, a pointer
            // to their Act II quest appears in the hub. Examining it starts (flags) the quest.
            foreach (var hook in CompanionQuests.Hooks)
            {
                if (!GameFlags.Current.GetBool($"camp.nighttalk.{hook.id}.done")) continue;
                var marker = MakeMarker(grid, hook.label, hook.coord, new Color(0.85f, 0.78f, 0.45f));
                marker.kind = InteractionKind.Examine;
                marker.examineText = hook.tease;
                marker.onExamined = () => GameFlags.Current.SetBool(hook.startedFlag, true);
            }

            // Act II connective tissue: the street crier and the Lower City side quests, each placed only
            // when its own flag-gate passes (active quest vs. resolved aftermath).
            if (actTwo != null)
                foreach (var npc in actTwo.Npcs)
                    if (npc.available == null || npc.available())
                        MakeNpc(grid, npc.label, npc.coord, npc.color, npc.dialogue);

            // A door to the Shrunken Quarter market — a second explorable Lower City room.
            if (onEnterMarket != null && GameFlags.Current.GetBool("prologue.cleared"))
            {
                var mk = MakeMarker(grid, "The Shrunken Quarter (market)", new Vector2Int(2, 5), new Color(0.6f, 0.55f, 0.4f));
                mk.kind = InteractionKind.Exit; mk.onExit = onEnterMarket;
            }

            // A path to the Chionthar Docks — a third explorable Lower City room.
            if (onEnterDocks != null && GameFlags.Current.GetBool("prologue.cleared"))
            {
                var dk = MakeMarker(grid, "The Chionthar Docks (waterfront)", new Vector2Int(2, 3), new Color(0.4f, 0.5f, 0.55f));
                dk.kind = InteractionKind.Exit; dk.onExit = onEnterDocks;
            }

            // The Harper safehouse — opens once Roen is in the company.
            if (onEnterSafehouse != null && GameFlags.Current.GetBool("companion.roen.recruited"))
            {
                var sh = MakeMarker(grid, "The Wandering Niche (Harper safehouse)", new Vector2Int(4, 3), new Color(0.45f, 0.5f, 0.6f));
                sh.kind = InteractionKind.Exit; sh.onExit = onEnterSafehouse;
            }

            // A Lower City fence — buy supplies; better stock as your standing rises.
            if (onEnterShop != null && GameFlags.Current.GetBool("prologue.cleared"))
            {
                var fence = MakeMarker(grid, "Sczerla's Sundries (a fence)", new Vector2Int(4, 5),
                    new Color(0.7f, 0.6f, 0.25f));
                fence.kind = InteractionKind.Exit;
                fence.onExit = onEnterShop;
            }

            // A door to the Almshouse of the Unclaimed — a second explorable Lower City room (Act II).
            if (onEnterAlmshouse != null && GameFlags.Current.GetBool("prologue.cleared"))
            {
                var door = MakeMarker(grid, "The Almshouse of the Unclaimed (enter)", new Vector2Int(6, 13),
                    new Color(0.7f, 0.6f, 0.4f));
                door.kind = InteractionKind.Exit;
                door.onExit = onEnterAlmshouse;
            }

            // Once the street preaching is settled, a militant Choir cell forms in the undercroft — an
            // optional miniboss skirmish. Clearing it protects the quarter (reputation + approval).
            if (onEnterChoirCell != null && GameFlags.Current.GetBool("quest.choir.resolved") &&
                !GameFlags.Current.GetBool("quest.choir.cell_cleared"))
            {
                var cell = MakeMarker(grid, "The Choir's undercroft — a cell gathers (battle)", new Vector2Int(8, 13),
                    new Color(0.45f, 0.42f, 0.55f));
                cell.kind = InteractionKind.Exit;
                cell.onExit = onEnterChoirCell;
            }
        }

        private void MakeContainer(GridSystem grid, string name, Vector2Int coord, int gold, List<ItemDefinition> items)
        {
            var it = MakeMarker(grid, name, coord, new Color(0.6f, 0.45f, 0.2f));
            it.kind = InteractionKind.Container; it.gold = gold; it.contents = items;
        }

        // ---- spawn helpers ----

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
            var it = MakeMarker(grid, name, coord, new Color(0.5f, 0.5f, 0.55f));
            it.kind = InteractionKind.Examine; it.examineText = text;
        }

        private void MakeExit(GridSystem grid, string name, Vector2Int coord, System.Action onExit)
        {
            var it = MakeMarker(grid, name, coord, new Color(0.3f, 0.1f, 0.4f));
            it.kind = InteractionKind.Exit; it.onExit = onExit;
        }

        private string BalladeerText()
        {
            var f = GameFlags.Current;
            int eras = 0;
            if (f.GetBool("netheril.cleared")) eras++;
            if (f.GetBool("crownwars.cleared")) eras++;
            if (f.GetBool("act4.toot_done")) eras++;
            if (f.GetBool("act4.spellplague_done")) eras++;
            int bosses = 0;
            foreach (var b in new[] { "netheril.boss_down", "crownwars.boss_down", "toot.avatar_down", "spellplague.herald_down" })
                if (f.GetBool(b)) bosses++;

            string s = "A cheap broadside, ink still tacky, hawking the latest ballad of *The Returned* — penny-dreadful " +
                       "stuff, half-invented, wholly delighted with itself.\n\n";
            s += eras >= 4
                ? "\"...who walked all four ages of the world and came back from each! Netheril's fall, the elven damnation, " +
                  "the god-skull forge, the blue fire — and lived to drink in the Gate!\" "
                : $"\"...who has walked {eras} age{(eras == 1 ? "" : "s")} that died before your grandsire's grandsire, and " +
                  "returned to tell it!\" ";
            if (bosses >= 4)
                s += "A second verse swears you felled a horror in every age — a colossus, a grief, an avatar, a herald. ";
            else if (bosses > 0)
                s += $"A scrawled verse credits you with {bosses} monstrous foe{(bosses == 1 ? "" : "s")} laid low in ages past. ";
            string beloved = f.GetBool("romance.garrow.turn") ? "a grim grave-priestess" :
                             f.GetBool("romance.roen.turn") ? "a silver-tongued Harper thief" :
                             f.GetBool("romance.naeve.turn") ? "a sorceress out of a fallen sky" :
                             f.GetBool("romance.varra.turn") ? "a warlock with a Hell on her tab" : null;
            if (beloved != null)
                s += $"There is even a love-verse — clumsy, swooning — about the Returned and {beloved}. It scans badly and " +
                     "means every word. Someone has drawn a small heart beside it, then crossed it out, then drawn it again. ";
            if (f.GetBool("docks.ferryman_saved") || f.GetBool("market.urchin_freed"))
                s += "There's a verse the well-off skip and the poor make him sing twice — about a dead thing that pulled a " +
                     "drowning man from the river / and bought a child's hand back from the Fist / and never once stopped to " +
                     "be thanked. The Quarter wrote that one themselves. It's the only verse in here that's true. ";
            if (f.GetBool("readers_boon"))
                s += "The last line is odd — it claims a Lady in the margins is *also* writing this, and finds you amusing. " +
                     "The balladeer has, of course, no idea how right he is.";
            else
                s += "The balladeer waves a hat for coppers and gets the colour of your eyes wrong. Legends never do check.";
            return s;
        }

        private string ChoirText()
        {
            int rep = GameFlags.Current.GetInt("faction.choir.reputation");
            if (rep >= 5)
                return "The hooded figure presses ash-marked fingertips to your wrist in a benediction you didn't ask for. " +
                       "\"The Choir sings your name now, Returned. You spoke for the Unmade when it cost you. When you reach " +
                       "the place where the Wall is decided, *remember us* — remember that the grief has a voice, and the " +
                       "voice has been waiting ten thousand years for someone the gods would actually let into the room.\" " +
                       "(It is not entirely comfortable, being someone's prophecy.)";
            if (rep >= 2)
                return "The hooded figure watches you with the wary hope of the grief-struck. \"You didn't silence us. " +
                       "Most do. The Choir remembers the few who let us speak.\" Ash-smudged, patient, it lets you pass.";
            if (rep <= -2)
                return "The hooded figure's eyes are flat and cold above the ash. \"You broke our voice with truncheons and " +
                       "called it peace. The grief you silenced didn't die, Returned. It went underground and grew teeth. " +
                       "Mind your back in the dark places. The Choir sings other songs now.\"";
            return "A figure in a grey hood, a smudge of ash across the brow — the mark of the Faithless Choir. It studies " +
                   "you without hostility and without trust, weighing whether the dead thing that walks is a threat to its " +
                   "grief, or the answer to it. It has not decided either.";
        }

        private string DoomguideText()
        {
            int rep = GameFlags.Current.GetInt("faction.kelemvor.reputation");
            if (rep >= 5)
                return "A grey-clad Doomguide watches you the way you watch a riddle you've nearly solved. \"The church " +
                       "does not know what to make of you, Returned. You honor the dead more faithfully than half my order " +
                       "— and you would tear down the Wall my god has kept since before the Gate had walls. (A pause, almost " +
                       "respect.) Vayle has marked your name. Not as an enemy. As a *question.* Come to the Court when it's " +
                       "time. She would hear your argument before she answers it with steel.\"";
            if (rep >= 2)
                return "The Doomguide inclines her head a careful degree. \"You've done right by the dead more than once. " +
                       "Kelemvor's people noticed. We notice everything that touches the dead.\" (Her hand rests easy. " +
                       "You are, for now, on the gentler page of a ledger that has only two.)";
            if (rep <= -2)
                return "The Doomguide does not incline her head. \"We know what you've done with the dead's due, Returned. " +
                       "The grave is not a market and the Wall is not yours to threaten. (Her grey is the grey of a closed " +
                       "door.) Walk small near a Doomguide. We have very long memories and very sharp scales.\"";
            return "A Doomguide of Kelemvor stands at watch, grey and patient as a headstone. Her eyes track you — the dead " +
                   "thing that walks — with a professional, unhurried interest. \"Curious,\" is all she says. The church is " +
                   "deciding what you are. It has not yet decided.";
        }

        private string OnlookersText()
        {
            var f = GameFlags.Current;
            var whispers = new System.Collections.Generic.List<string>();
            if (f.GetBool("companion.naeve.recruited")) whispers.Add("the pale woman who speaks a dead tongue and watches the smoke rise wrong");
            if (f.GetBool("companion.ilfaeril.recruited")) whispers.Add("the elf so old the Gate's whole history is a single afternoon to him");
            if (f.GetBool("companion.varra.recruited")) whispers.Add("the warlock whose shadow doesn't quite match her");
            if (f.GetBool("companion.maerin.recruited")) whispers.Add("the girl who flickers at the edges, like a candle deciding whether to stay lit");
            if (f.GetBool("companion.roen.recruited")) whispers.Add("that Alleywind rogue, who everyone swears they've been robbed by");

            string s = "A handful of Gate folk have stopped to stare — and to whisper. You are, after all, a dead thing " +
                       "walking, and the company you keep does not help.";
            if (whispers.Count == 0)
                return s + " (For now they only whisper about *you.* Give it time; the Realms are wide, and your road runs strange.)";
            s += " The talk is all of your companions:\n\n";
            foreach (var w in whispers) s += $"• \"...{w}...\"\n";
            s += "\nThey are afraid of you. They are also, you notice, unable to look away. The Gate has never seen the " +
                 "like — and it will be telling the tale long after you are gone. However that goes.";
            if (whispers.Count >= 5)
                s += "\n\n(A child tugs her mother's sleeve and asks, in the carrying voice of the very young, whether you " +
                     "and your strange friends are heroes or monsters. The mother, to her credit, says: 'Both, love. The " +
                     "best ones always are.')";
            return s;
        }

        private string ProclamationText()
        {
            var f = GameFlags.Current;
            string s = "By order of the Flaming Fist: the lower stair is SEALED. The dead beneath do not rest. " +
                       "Doomguides of Kelemvor hunt a heretic — and a soul that walked back out of death.\n\n";
            int rep = f.GetInt("reputation.lowcity");
            if (f.GetBool("lowcity.allies") || rep >= 3)
                s += "(Scrawled beneath, in a dozen different hands: a single name. Yours. Kept here like a candle by " +
                     "the people the Fist never bothers to count.)";
            else if (rep <= -2)
                s += "(Someone has chalked your name here too, with a hard word beside it. The poor keep their own " +
                     "proclamations, and they do not seal them.)";
            else if (f.GetBool("netheril.cleared"))
                s += "(A newer notice, half-torn: mad tales of a Returned walking ages that fell ten thousand years " +
                     "gone. The Fist crossed it out as sedition. The crossing-out only made people read it.)";
            else
                s += "(The rest is the usual: bread dear, a lost dog, a warning about the Outer City after dark.)";
            return s;
        }

        // ---- the hub conversation ----

        private DialogueGraph HeraldDialogue()
        {
            var g = ScriptableObject.CreateInstance<DialogueGraph>();
            g.conversationId = "hub.herald";
            g.startNodeId = "0";

            g.nodes.Add(new DialogueNode
            {
                id = "0", speaker = "Herald of the Gate",
                text = "Returned. I felt you cross back — like a bell rung in an empty cathedral. " +
                       "Aldric Morn would speak with you. But the Doomguides have sealed the Cinderhaunt " +
                       "below, where the harvest still festers. Clear it, and the road to him opens.",
                choices = new[]
                {
                    new DialogueChoice { text = "Why should I trust Aldric Morn?", nextNodeId = "1" },
                    new DialogueChoice { text = "I'll clear the Cinderhaunt.", nextNodeId = "accept" },
                }
            });

            g.nodes.Add(new DialogueNode
            {
                id = "1", speaker = "Herald of the Gate",
                text = "Trust nothing. But you've seen the Wall of the Faithless. You know the question he " +
                       "asks is real. Decide below — in blood and shadow, where such things are decided.",
                autoNextNodeId = "accept"
            });

            g.nodes.Add(new DialogueNode
            {
                id = "accept", speaker = "Herald of the Gate",
                text = "Descend when you're ready; the stairs are marked. Try not to die a second time.",
                onEnter = new[] { new FlagClause { key = "prologue.herald_met", op = FlagOp.SetTrue } }
            });

            return g;
        }

        // ---- Roen's recruitment conversation ----

        private DialogueGraph RoenDialogue()
        {
            var g = ScriptableObject.CreateInstance<DialogueGraph>();
            g.conversationId = "hub.roen";
            g.startNodeId = "0";

            g.nodes.Add(new DialogueNode
            {
                id = "0", speaker = "Roen Alleywind",
                text = "So you're the one who walked back out of death. Relax — I'm a friend. The kind that " +
                       "knows every alley, lock, and lie in this city. The Harpers sent me to keep an eye on " +
                       "you. I'd rather keep the rest of me on you too. Mind some company down those stairs?",
                choices = new[]
                {
                    new DialogueChoice
                    {
                        text = "A Harper spy. Why would I trust you?", nextNodeId = "trust"
                    },
                    new DialogueChoice
                    {
                        text = "Welcome aboard. We could use the blades — and the lockpicks.", nextNodeId = "join",
                        effects = new[]
                        {
                            new FlagClause { key = "companion.roen.recruited", op = FlagOp.SetTrue },
                            new FlagClause { key = "companion.roen.approval", op = FlagOp.AddInt, amount = 10 },
                        }
                    },
                }
            });

            g.nodes.Add(new DialogueNode
            {
                id = "trust", speaker = "Roen Alleywind",
                text = "You don't. That's the smart play. But the Doomguides want you in a cell and Aldric wants " +
                       "you on an altar — and I want neither. That puts me a cut above your other options. Coming?",
                choices = new[]
                {
                    new DialogueChoice
                    {
                        text = "Fair enough. Join me.", nextNodeId = "join",
                        effects = new[]
                        {
                            new FlagClause { key = "companion.roen.recruited", op = FlagOp.SetTrue },
                            new FlagClause { key = "companion.roen.approval", op = FlagOp.AddInt, amount = 15 },
                        }
                    },
                    new DialogueChoice { text = "Not yet.", nextNodeId = "later" },
                }
            });

            g.nodes.Add(new DialogueNode
            {
                id = "join", speaker = "Roen Alleywind",
                text = "Then I'm yours. Try not to get us both killed — I've grown fond of breathing."
            });

            g.nodes.Add(new DialogueNode
            {
                id = "later", speaker = "Roen Alleywind",
                text = "Suit yourself. I'll be here, looking trustworthy. Come find me when you change your mind."
            });

            return g;
        }

        // ---- Varra's recruitment conversation ----

        private DialogueGraph VarraDialogue()
        {
            var g = ScriptableObject.CreateInstance<DialogueGraph>();
            g.conversationId = "hub.varra";
            g.startNodeId = "0";

            g.nodes.Add(new DialogueNode
            {
                id = "0", speaker = "Varra",
                text = "Returned, are you? A soul with a hole in it. Charming. I'm Varra — I've a hole in mine too, " +
                       "shaped exactly like a contract I signed when I was six. Misery loves company, and company " +
                       "with a sword is even better. Take me along before my patron remembers I exist?",
                choices = new[]
                {
                    new DialogueChoice
                    {
                        text = "Welcome aboard. We've both got debts to outrun.", nextNodeId = "join",
                        effects = new[]
                        {
                            new FlagClause { key = "companion.varra.recruited", op = FlagOp.SetTrue },
                            new FlagClause { key = "companion.varra.approval", op = FlagOp.AddInt, amount = 15 },
                        }
                    },
                    new DialogueChoice { text = "A pact-bound warlock sounds like a liability.", nextNodeId = "liability" },
                }
            });
            g.nodes.Add(new DialogueNode
            {
                id = "liability", speaker = "Varra",
                text = "Oh, absolutely. I'm a leash with a person on the end of it, and one day someone's going to " +
                       "*pull.* But until then I'm the funniest liability you'll ever meet, and I throw fire. Your call.",
                choices = new[]
                {
                    new DialogueChoice
                    {
                        text = "Fair. Come on, then.", nextNodeId = "join",
                        effects = new[]
                        {
                            new FlagClause { key = "companion.varra.recruited", op = FlagOp.SetTrue },
                            new FlagClause { key = "companion.varra.approval", op = FlagOp.AddInt, amount = 8 },
                        }
                    },
                    new DialogueChoice { text = "Not yet.", nextNodeId = "later" },
                }
            });
            g.nodes.Add(new DialogueNode { id = "join", speaker = "Varra",
                text = "Excellent. Don't get attached — everyone who buys me eventually gets a bill. But the interim's a riot." });
            g.nodes.Add(new DialogueNode { id = "later", speaker = "Varra",
                text = "Mm. Hovering near the exit. Smart. I'd do the same, if I had one." });
            return g;
        }
    }
}
