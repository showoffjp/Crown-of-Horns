using System;
using System.Collections.Generic;
using UnityEngine;
using SunderedCrown.Dialogue;
using SunderedCrown.Stats;

namespace SunderedCrown.Content
{
    /// <summary>
    /// Act II connective tissue for the Lower City hub: reactive NPCs and self-contained side quests that
    /// make the world feel like it's *watching you back*. A street crier reads your reputation, companions,
    /// and the eras you've walked straight off the flags (via conditional dialogue choices); two
    /// dialogue-resolved side quests ("The Widow's Coin", "The Fist and the Faithless") give small,
    /// thematic moral calls that move reputation and companion approval. Pure data — the hub places each
    /// NPC whose <see cref="Npc.available"/> gate passes, the same way it gates Roen/Varra recruitment.
    /// </summary>
    public class ActTwoContent
    {
        public class Npc
        {
            public string label;
            public Vector2Int coord;
            public Color color;
            public Func<bool> available;   // gate (reads GameFlags); null = always
            public DialogueGraph dialogue;
        }

        public List<Npc> Npcs { get; private set; } = new List<Npc>();

        public ActTwoContent()
        {
            bool F(string k) => Core.GameFlags.Current.GetBool(k);

            // --- The street crier: a living "news of you" board, reactive via conditional choices. ---
            Npcs.Add(new Npc
            {
                label = "Tamsin, a street crier", coord = new Vector2Int(9, 3),
                color = new Color(0.8f, 0.7f, 0.55f),
                available = () => F("prologue.cleared"),
                dialogue = Crier(),
            });

            // --- Side quest: The Widow's Coin (active until resolved, then a quiet thank-you). ---
            Npcs.Add(new Npc
            {
                label = "Mhaere, a grieving widow", coord = new Vector2Int(5, 3),
                color = new Color(0.5f, 0.45f, 0.6f),
                available = () => F("prologue.cleared") && !F("quest.widow.resolved"),
                dialogue = Widow(),
            });
            Npcs.Add(new Npc
            {
                label = "Mhaere (at peace, after a fashion)", coord = new Vector2Int(5, 3),
                color = new Color(0.55f, 0.55f, 0.62f),
                available = () => F("quest.widow.resolved"),
                dialogue = WidowAfter(),
            });

            // --- Side quest: The Fist and the Faithless (active until resolved, then an aftermath). ---
            Npcs.Add(new Npc
            {
                label = "Sergeant Kallia of the Flaming Fist", coord = new Vector2Int(12, 9),
                color = new Color(0.75f, 0.55f, 0.3f),
                available = () => F("prologue.cleared") && !F("quest.fist.resolved"),
                dialogue = Fist(),
            });
            Npcs.Add(new Npc
            {
                label = "Sergeant Kallia (off duty)", coord = new Vector2Int(12, 9),
                color = new Color(0.7f, 0.6f, 0.45f),
                available = () => F("quest.fist.resolved"),
                dialogue = FistAfter(),
            });

            // --- Side quest: The Faithless Choir (seeds the Unmade plot in the Lower City). ---
            Npcs.Add(new Npc
            {
                label = "Brother Sere, a preacher of the Unmade", coord = new Vector2Int(8, 11),
                color = new Color(0.5f, 0.48f, 0.62f),
                available = () => F("prologue.cleared") && !F("quest.choir.resolved"),
                dialogue = Choir(),
            });
            Npcs.Add(new Npc
            {
                label = "The corner where the Choir preached", coord = new Vector2Int(8, 11),
                color = new Color(0.4f, 0.4f, 0.5f),
                available = () => F("quest.choir.resolved"),
                dialogue = ChoirAfter(),
            });

            // --- Side quest: The Last Letter (a dying soldier, an old wrong, three kinds of closure). ---
            Npcs.Add(new Npc
            {
                label = "Old Davyn, a dying Fist veteran", coord = new Vector2Int(16, 9),
                color = new Color(0.6f, 0.55f, 0.5f),
                available = () => F("prologue.cleared") && !F("quest.letter.resolved"),
                dialogue = Letter(),
            });
            Npcs.Add(new Npc
            {
                label = "Old Davyn's empty cot", coord = new Vector2Int(16, 9),
                color = new Color(0.5f, 0.48f, 0.45f),
                available = () => F("quest.letter.resolved"),
                dialogue = LetterAfter(),
            });

            // --- Side quest: The Tithe Collector (grief extortion — who pays for the dead's rest?). ---
            Npcs.Add(new Npc
            {
                label = "Collector Vane, who sells the dead their rest", coord = new Vector2Int(15, 5),
                color = new Color(0.55f, 0.5f, 0.35f),
                available = () => F("prologue.cleared") && !F("quest.tithe.resolved"),
                dialogue = Tithe(),
            });
            Npcs.Add(new Npc
            {
                label = "Vane's empty collection table", coord = new Vector2Int(15, 5),
                color = new Color(0.45f, 0.42f, 0.35f),
                available = () => F("quest.tithe.resolved") && !F("quest.tithe.corrupt"),
                dialogue = TitheAfter(false),
            });
            Npcs.Add(new Npc
            {
                label = "Vane's busy collection table", coord = new Vector2Int(15, 5),
                color = new Color(0.5f, 0.45f, 0.3f),
                available = () => F("quest.tithe.corrupt"),
                dialogue = TitheAfter(true),
            });
        }

        // ---------- helpers ----------

        private static DialogueNode Line(string id, string speaker, string text, string autoNext = null)
            => new DialogueNode { id = id, speaker = speaker, text = text, autoNextNodeId = autoNext };

        private static FlagClause Req(string key, bool wantTrue = true)
            => new FlagClause { key = key, op = wantTrue ? FlagOp.RequireBoolTrue : FlagOp.RequireBoolFalse };

        private static FlagClause Set(string key) => new FlagClause { key = key, op = FlagOp.SetTrue };
        private static FlagClause Add(string key, int amount) => new FlagClause { key = key, op = FlagOp.AddInt, amount = amount };

        // ---------- the crier (reactive news of you) ----------

        private DialogueGraph Crier()
        {
            var g = ScriptableObject.CreateInstance<DialogueGraph>();
            g.conversationId = "act2.crier"; g.startNodeId = "0";

            g.nodes.Add(new DialogueNode
            {
                id = "0", speaker = "Tamsin",
                text = "The city is a story that tells itself, Returned, and lately you're a chapter of it. Ask — I'll " +
                       "read you back to yourself. (Only the tales the streets have actually earned will be on her lips.)",
                choices = new[]
                {
                    // Each piece of "news" only appears once its flag is true — the board reacts to your run.
                    new DialogueChoice { text = "(A widow blesses your name.)", nextNodeId = "n_widow",
                        conditions = new[] { Req("quest.widow.resolved") } },
                    new DialogueChoice { text = "(You won a Faithless man his freedom.)", nextNodeId = "n_freed",
                        conditions = new[] { Req("quest.fist.freed") } },
                    new DialogueChoice { text = "(You let the Fist take a godless man.)", nextNodeId = "n_lawful",
                        conditions = new[] { Req("quest.fist.lawful") } },
                    new DialogueChoice { text = "(They say you walked the falling city.)", nextNodeId = "n_neth",
                        conditions = new[] { Req("netheril.cleared") } },
                    new DialogueChoice { text = "(They whisper you argued the elf-courts down.)", nextNodeId = "n_crown",
                        conditions = new[] { Req("crownwars.verdict_spared") } },
                    new DialogueChoice { text = "(The grey Doomguide softened — for someone.)", nextNodeId = "n_garrow_love",
                        conditions = new[] { Req("romance.garrow.consummated") } },
                    new DialogueChoice { text = "(Someone walked out on you, they say.)", nextNodeId = "n_left",
                        conditions = new[] { Req("companion.roen.left") } },
                    new DialogueChoice { text = "(You gave the Choir's preacher pause.)", nextNodeId = "n_choir_doubt",
                        conditions = new[] { Req("quest.choir.doubted") } },
                    new DialogueChoice { text = "(You set the Fist on the Choir.)", nextNodeId = "n_choir_supp",
                        conditions = new[] { Req("quest.choir.suppressed") } },
                    new DialogueChoice { text = "(They say you spoke FOR the Unmade.)", nextNodeId = "n_choir_fav",
                        conditions = new[] { Req("quest.choir.favored") } },
                    new DialogueChoice { text = "(You let the poor bury their dead free.)", nextNodeId = "n_tithe_good",
                        conditions = new[] { Req("quest.tithe.freed") } },
                    new DialogueChoice { text = "(You paid the poor's grave-debts yourself.)", nextNodeId = "n_tithe_good",
                        conditions = new[] { Req("quest.tithe.paid") } },
                    new DialogueChoice { text = "(You took a cut of the grave-tithe.)", nextNodeId = "n_tithe_bad",
                        conditions = new[] { Req("quest.tithe.corrupt") } },
                    new DialogueChoice { text = "(The quarter speaks well of you — hear the offer.)", nextNodeId = "n_boon",
                        conditions = new[] {
                            new FlagClause { key = "reputation.lowcity", op = FlagOp.RequireIntAtLeast, amount = 5 },
                            Req("lowcity.allies", false) } },
                    new DialogueChoice { text = "Just give me the ordinary word, then.", nextNodeId = "n_plain" },
                    new DialogueChoice { text = "Enough reading. (Leave.)", nextNodeId = "bye" },
                }
            });

            g.nodes.Add(new DialogueNode
            {
                id = "n_boon", speaker = "Tamsin",
                text = "\"You've spent real kindness down here, and the Lower City pays its debts in the only coin it " +
                       "has: itself. Word's gone round — if it ever comes to the end of things for you, the quarter " +
                       "stands with you. Beggars, dockhands, the godless, the grieving. Not much. But yours.\"",
                onEnter = new[] { Set("lowcity.allies") },
                autoNextNodeId = "0",
            });

            g.nodes.Add(Line("n_widow", "Tamsin",
                "\"A widow in the Shrunken Quarter keeps your name in her prayers — the ones she has left. Says you " +
                "treated her dead like he mattered. Small thing. Small things are most of a city.\"", "0"));
            g.nodes.Add(Line("n_freed", "Tamsin",
                "\"There's an old man sleeping rough by the docks who'd be in a Doomguide cell — or worse — if you " +
                "hadn't stared down the Fist. The godless poor have a short list of friends. You're on it now.\"", "0"));
            g.nodes.Add(Line("n_lawful", "Tamsin",
                "\"They say you let the Fist march a Faithless man to the Wall, all proper and lawful. The well-fed " +
                "nod. The poor remember. Both tell the tale. Only one of them forgets it.\"", "0"));
            g.nodes.Add(Line("n_neth", "Tamsin",
                "\"Maddest one going round: that you walked Netheril as the sky came down and walked back out. I sell " +
                "it as a tall tale. I don't tell them I think it's true.\"", "0"));
            g.nodes.Add(Line("n_crown", "Tamsin",
                "\"A rumour with too much detail to be invented: that you stood in an elven court ten thousand years " +
                "gone and argued a damnation *down.* Where do you even keep a story like that?\"", "0"));
            g.nodes.Add(Line("n_garrow_love", "Tamsin",
                "\"The grey sister — the one who buries everyone and loves no one? They say she softened, for someone. " +
                "The whole quarter's scandalised and delighted. You wouldn't know anything about that.\"", "0"));
            g.nodes.Add(Line("n_left", "Tamsin",
                "\"Word is one of yours walked off into the dark and didn't come back. Companions are like teeth, " +
                "Returned — you don't count them till one's gone. People noticed.\"", "0"));
            g.nodes.Add(Line("n_choir_doubt", "Tamsin",
                "\"That firebrand preacher of the Unmade? They say you didn't club him — you *talked* him quiet. " +
                "Gave him a doubt instead of a martyrdom. Cleverer, and kinder, and rarer than both.\"", "0"));
            g.nodes.Add(Line("n_choir_supp", "Tamsin",
                "\"You called the Fist down on the corner preacher. Tidy. The well-fed approve. But grief doesn't " +
                "scatter when you scatter the man saying it out loud — it just goes quiet, and quiet curdles.\"", "0"));
            g.nodes.Add(Line("n_choir_fav", "Tamsin",
                "\"Whisper is the Returned spoke *for* the Unmade, right out in the open. That'll carry into rooms you " +
                "haven't walked into yet. Some will love you for it. Some will sharpen something.\"", "0"));
            g.nodes.Add(Line("n_tithe_good", "Tamsin",
                "\"You took on Vane the grave-collector and the pauper's pit got blessed for free. The poor can't pay " +
                "you back — so they'll do the only thing they can. They'll remember. Out loud. To everyone.\"", "0"));
            g.nodes.Add(Line("n_tithe_bad", "Tamsin",
                "\"Word is you went into business with Vane — a cut of what the grieving pay to bury their own. The " +
                "well-fed shrug. The poor add your name to a short, bitter list. The city keeps that ledger too.\"", "0"));
            g.nodes.Add(Line("n_plain", "Tamsin",
                "\"Bread's dear, the Fist's nervous, the dead won't stay down, and a heretic's having tea in the Lower " +
                "City like it's nothing. Same as ever. Worse than ever. The city abides.\"", "0"));
            g.nodes.Add(Line("bye", "Tamsin",
                "\"Go on, then. Make me a better chapter. I do love a long story.\""));
            return g;
        }

        // ---------- The Widow's Coin ----------

        private DialogueGraph Widow()
        {
            var g = ScriptableObject.CreateInstance<DialogueGraph>();
            g.conversationId = "act2.widow"; g.startNodeId = "0";

            g.nodes.Add(new DialogueNode
            {
                id = "0", speaker = "Mhaere",
                text = "You're the one who came back. Then you've *been* there. My boy — Edrin — he was taken in the " +
                       "harvest under the Cinderhaunt. He didn't worship; he was just a boy who mended nets. The grey " +
                       "priests say the godless don't rest. Tell me they're wrong. Please. I'll give you my last coin " +
                       "to tell me he's at peace.",
                choices = new[]
                {
                    new DialogueChoice
                    {
                        text = "He's at peace. I've seen the other side — he's at peace. (a kindness)", nextNodeId = "lie",
                        effects = new[] { Set("quest.widow.resolved"), Set("quest.widow.lie"),
                                          Add("reputation.lowcity", 1) }
                    },
                    new DialogueChoice
                    {
                        text = "I won't lie to you. He doesn't rest — yet. But I will fight to change that. (truth)", nextNodeId = "truth",
                        effects = new[] { Set("quest.widow.resolved"), Set("quest.widow.truth"),
                                          Add("reputation.lowcity", 1), Add("companion.garrow.approval", 5) }
                    },
                    new DialogueChoice
                    {
                        text = "[Insight] Say his name to me. While it's spoken, the Wall hasn't won.", nextNodeId = "hope_win",
                        failNodeId = "truth", checkAbility = Ability.Wisdom, checkDC = 14,
                    },
                }
            });

            g.nodes.Add(Line("lie", "Mhaere",
                "Oh. Oh, thank the gods — thank *you.* (She weeps, and the weeping is relief, and you watch her believe " +
                "a thing you cannot promise.) Take the coin. I'll sleep tonight. First time since the harvest.\n\n" +
                "(It cost her nothing and you everything, or the reverse. You're honestly not sure which.)"));

            g.nodes.Add(Line("truth", "Mhaere",
                "...That's the cruelest mercy anyone's shown me. (She doesn't take the comfort, because you didn't give " +
                "her a false one.) But you'll *fight* it. You — who walked back out. If anyone could pull my Edrin from " +
                "that Wall — keep the coin. Spend it on whatever lets you keep that promise."));

            g.nodes.Add(new DialogueNode
            {
                id = "hope_win", speaker = "Mhaere",
                text = "Edrin. Edrin of the nets. (You say it with her, and again, until the saying steadies her.) ...As " +
                       "long as someone alive still speaks him, he isn't *unmade* — only kept. That's not a priest's " +
                       "comfort. That's a true one. I can carry a true one. I'll say his name every morning, and you'll " +
                       "say it where it counts. Bargain?",
                onEnter = new[] { Set("quest.widow.resolved"), Set("quest.widow.hope"),
                                  Add("reputation.lowcity", 2), Add("companion.garrow.approval", 8) }
            });
            return g;
        }

        private DialogueGraph WidowAfter()
        {
            var g = ScriptableObject.CreateInstance<DialogueGraph>();
            g.conversationId = "act2.widow.after"; g.startNodeId = "0";
            g.nodes.Add(new DialogueNode
            {
                id = "0", speaker = "Mhaere",
                text = "There you are. I keep Edrin's name on my lips each dawn now, like you taught me — or I sleep " +
                       "easier for what you said; the days blur. Either way the morning comes, and I meet it. That's " +
                       "your doing. Go safely, Returned.",
                choices = new[] { new DialogueChoice { text = "Be well, Mhaere.", nextNodeId = "bye" } }
            });
            g.nodes.Add(Line("bye", "Mhaere", "She presses the coin into your hand anyway, and folds your fingers over it."));
            return g;
        }

        // ---------- The Fist and the Faithless ----------

        private DialogueGraph Fist()
        {
            var g = ScriptableObject.CreateInstance<DialogueGraph>();
            g.conversationId = "act2.fist"; g.startNodeId = "0";

            g.nodes.Add(new DialogueNode
            {
                id = "0", speaker = "Sergeant Kallia",
                text = "Stay back, Returned — official business. I've an old beggar here the new edict says I hand to the " +
                       "Doomguides: no god, no temple, 'a soul unclaimed and therefore suspect.' He stole bread. That's " +
                       "all he did. But the writ's the writ, and my pension's the pension. ...You're looking at me like " +
                       "you've got an opinion.",
                choices = new[]
                {
                    new DialogueChoice
                    {
                        text = "[Persuasion] He's no heretic. Let him walk. You know this is wrong.",
                        nextNodeId = "freed", failNodeId = "fail", checkAbility = Ability.Charisma, checkDC = 14,
                    },
                    new DialogueChoice
                    {
                        text = "Here's coin for your trouble. Lose the paperwork. (bribe)", nextNodeId = "bribe",
                        effects = new[] { Set("quest.fist.resolved"), Set("quest.fist.bribed"),
                                          Add("reputation.lowcity", 1) }
                    },
                    new DialogueChoice
                    {
                        text = "The law's the law. Hand him over. (lawful)", nextNodeId = "lawful",
                        effects = new[] { Set("quest.fist.resolved"), Set("quest.fist.lawful"),
                                          Add("faction.kelemvor.reputation", 1),
                                          Add("companion.garrow.approval", -3), Add("companion.ilfaeril.approval", -3) }
                    },
                }
            });

            g.nodes.Add(new DialogueNode
            {
                id = "freed", speaker = "Sergeant Kallia",
                text = "...Gods help me, you're right. I knew you were right when I cuffed him. (She strikes the irons. " +
                       "The old man bolts without a word.) There. I'll say he ran. Let the Doomguides chase a beggar if " +
                       "they've nothing better. Thank you, Returned — for making me braver than my pension.",
                onEnter = new[] { Set("quest.fist.resolved"), Set("quest.fist.freed"),
                                  Add("reputation.lowcity", 2), Add("companion.garrow.approval", 5),
                                  Add("faction.kelemvor.reputation", -1) }
            });

            g.nodes.Add(Line("fail", "Sergeant Kallia",
                "Don't. Don't make the speech — I'll start agreeing, and agreeing gets a sergeant cashiered. If you want " +
                "him loose, there's a quieter way: coin for the paperwork, or you tell me to follow the writ. Plainly, " +
                "now. Which.", "0"));

            g.nodes.Add(Line("bribe", "Sergeant Kallia",
                "(The coin vanishes; so does the writ; so, conveniently, does the prisoner.) Never saw him. Never saw " +
                "you. Pleasure doing no business with you, Returned."));

            g.nodes.Add(Line("lawful", "Sergeant Kallia",
                "(She nods, almost grateful to be told.) Aye. The writ's the writ. (The old man is led off grey-faced, " +
                "and does not look back, because there is no one to look back *to.*) ...Clean conscience, that. Mostly. " +
                "Move along."));
            return g;
        }

        private DialogueGraph FistAfter()
        {
            var g = ScriptableObject.CreateInstance<DialogueGraph>();
            g.conversationId = "act2.fist.after"; g.startNodeId = "0";
            g.nodes.Add(new DialogueNode
            {
                id = "0", speaker = "Sergeant Kallia",
                text = "Off duty, for once. Whatever you made me do at that corner — I've thought about it since. Don't " +
                       "get many chances to find out what kind of Fist you are. You showed me mine. Buy you a drink " +
                       "sometime, Returned. We'll not talk about edicts.",
                choices = new[] { new DialogueChoice { text = "Some other night, Sergeant.", nextNodeId = "bye" } }
            });
            g.nodes.Add(Line("bye", "Sergeant Kallia", "She salutes you with two fingers and ambles off toward the taverns."));
            return g;
        }

        // ---------- The Faithless Choir (seeds the Unmade plot) ----------

        private DialogueGraph Choir()
        {
            var g = ScriptableObject.CreateInstance<DialogueGraph>();
            g.conversationId = "act2.choir"; g.startNodeId = "0";

            g.nodes.Add(new DialogueNode
            {
                id = "0", speaker = "Brother Sere",
                text = "...and the gods kept a *Wall*, friends — a Wall to grind the souls of everyone who never knelt! The " +
                       "godless, the forgotten, the poor — *us!* But the Wall can come DOWN. (He sees you, and his eyes " +
                       "light like lamps.) And here — *here* — is the proof! A soul that walked back OUT of death! Tell " +
                       "them, Returned. Tell them the Unmade will set us free.",
                choices = new[]
                {
                    new DialogueChoice
                    {
                        text = "[Religion] You don't know what tearing the Wall costs. I've seen it. (complicate)",
                        nextNodeId = "doubt_win", failNodeId = "doubt_fail", checkAbility = Ability.Intelligence, checkDC = 15,
                    },
                    new DialogueChoice
                    {
                        text = "This is a danger to everyone here. I'll bring the Fist. (suppress)", nextNodeId = "suppress",
                        effects = new[] { Set("quest.choir.resolved"), Set("quest.choir.suppressed"),
                                          Add("faction.kelemvor.reputation", 1), Add("reputation.lowcity", -2),
                                          Add("companion.ilfaeril.approval", 2) }
                    },
                    new DialogueChoice
                    {
                        text = "Maybe he's right. The Wall is an atrocity. (sympathize)", nextNodeId = "favored",
                        effects = new[] { Set("quest.choir.resolved"), Set("quest.choir.favored"), Set("choir.sympathizer"),
                                          Add("companion.ilfaeril.approval", -4) }
                    },
                }
            });

            g.nodes.Add(new DialogueNode
            {
                id = "doubt_win", speaker = "Brother Sere",
                text = "I— what? You... you've *stood* at it. (For the first time the certainty cracks.) You say the Wall " +
                       "is a horror — and that pulling its thread drowns the living to free the dead. That the cure is " +
                       "another atrocity wearing mercy's face.\n\n(The crowd murmurs and thins. He sits, suddenly young.) " +
                       "Then what are we supposed to *do* with all this grief? ...I'll think. Gods help me, I'll think. " +
                       "Thank you for not just silencing me.",
                onEnter = new[] { Set("quest.choir.resolved"), Set("quest.choir.doubted"),
                                  Add("reputation.lowcity", 1), Add("companion.ilfaeril.approval", 5),
                                  Add("companion.garrow.approval", 3) }
            });

            g.nodes.Add(Line("doubt_fail", "Brother Sere",
                "You quote the canon at me? (He grins, unshaken, and the crowd leans back in.) The Returned defends the " +
                "Wall! Even *you* have been frightened into loving your cage! No — I won't be lawyered out of the truth. " +
                "Say it plain: do you silence me with the Fist, or do you stand with the Unmade?", "0"));

            g.nodes.Add(Line("suppress", "Brother Sere",
                "(The Fist scatters his flock with truncheons; he's dragged off mid-sentence.) You'll regret this! The " +
                "Wall makes Faithless of us ALL in the end — even its wardens! (The poor who'd gathered watch you, now, " +
                "the way they watch the Fist.) ...The corner is quiet again. Quiet isn't the same as right."));

            g.nodes.Add(Line("favored", "Brother Sere",
                "(He seizes your hand, radiant.) You see! The Returned stands with us! (Word of this will run ahead of " +
                "you into stranger, hungrier rooms than this one — the Choir keeps its friends in mind.) Whatever comes, " +
                "Returned, the Unmade remembers who spoke for it when speaking cost something."));
            return g;
        }

        private DialogueGraph ChoirAfter()
        {
            var g = ScriptableObject.CreateInstance<DialogueGraph>();
            g.conversationId = "act2.choir.after"; g.startNodeId = "0";
            g.nodes.Add(new DialogueNode
            {
                id = "0", speaker = "(the corner)",
                text = "Where the preacher stood there's only chalk now — a circle, a broken Wall scratched through it, " +
                       "and a single word the rain hasn't taken: REMEMBER. Whatever you chose here, the Lower City's " +
                       "grief didn't go anywhere. It only learned your name.",
                choices = new[] { new DialogueChoice { text = "Move on.", nextNodeId = "bye" } }
            });
            g.nodes.Add(Line("bye", "(the corner)", "The chalk crunches under your boot as you go."));
            return g;
        }

        // ---------- The Tithe Collector (who pays for the dead's rest?) ----------

        private DialogueGraph Tithe()
        {
            var g = ScriptableObject.CreateInstance<DialogueGraph>();
            g.conversationId = "act2.tithe"; g.startNodeId = "0";

            g.nodes.Add(new DialogueNode
            {
                id = "0", speaker = "Collector Vane",
                text = "Rest isn't free, friend. A proper consecrated grave, the rites, a name cut in stone so the soul " +
                       "isn't *Faithless* in the eyes of the Doom — that costs. (He taps a fat ledger of the poor's " +
                       "debts.) These folk can't pay, so their dead wait in the pauper's pit, unblessed, halfway to the " +
                       "Wall already. Sad. But the church's candles don't light themselves.",
                choices = new[]
                {
                    new DialogueChoice
                    {
                        text = "[Persuasion] The dead don't pay tolls. Tear up the ledger. (free them)",
                        nextNodeId = "freed", failNodeId = "fail", checkAbility = Ability.Charisma, checkDC = 15,
                    },
                    new DialogueChoice
                    {
                        text = "Then I'll clear their debts myself. All of them. (pay)", nextNodeId = "paid",
                        effects = new[] { Set("quest.tithe.resolved"), Set("quest.tithe.paid"),
                                          Add("reputation.lowcity", 2), Add("companion.garrow.approval", 3) }
                    },
                    new DialogueChoice
                    {
                        text = "What's your cut, for a partner who looks the other way? (take a cut)", nextNodeId = "corrupt",
                        effects = new[] { Set("quest.tithe.resolved"), Set("quest.tithe.corrupt"),
                                          Add("reputation.lowcity", -3), Add("companion.garrow.approval", -6),
                                          Add("companion.ilfaeril.approval", -3) }
                    },
                }
            });

            g.nodes.Add(new DialogueNode
            {
                id = "freed", speaker = "Collector Vane",
                text = "You— you can't just— (But you can, and the way you say it leaves no daylight in it. He swallows.) " +
                       "...The Doom never actually *charged* for rest. I read the canon same as anyone — I just read the " +
                       "part that paid me. (He closes the ledger.) Fine. The pit gets blessed. No charge. Tell them it " +
                       "was you. They won't believe it was me.",
                onEnter = new[] { Set("quest.tithe.resolved"), Set("quest.tithe.freed"),
                                  Add("reputation.lowcity", 2), Add("companion.garrow.approval", 6) }
            });

            g.nodes.Add(Line("fail", "Collector Vane",
                "Spare me the sermon — I've heard better from real Doomguides, and they take their cut too. You want " +
                "these dead blessed, there's two honest ways and we both know them: open your own purse, or open a " +
                "*partnership* with mine. Which is it?", "0"));

            g.nodes.Add(Line("paid", "Collector Vane",
                "(He counts it twice, astonished.) Huh. Nobody's ever just... paid the *others'* tab before. The pit " +
                "gets its rites tonight, every name. You're a strange one, Returned. Strange in the way the temples " +
                "used to mean *holy,* before they meant *expensive.*"));

            g.nodes.Add(Line("corrupt", "Collector Vane",
                "(He grins, relieved to meet a kindred ledger.) Twenty percent, and I'll say you're a *consultant.* " +
                "Pleasure. (The pit stays unblessed, and the poor learn the Returned has a price like everyone else. " +
                "Somewhere, a grey sister you travel with hears of it, and says nothing, which is worse than if she did.)"));
            return g;
        }

        private DialogueGraph TitheAfter(bool corrupt)
        {
            var g = ScriptableObject.CreateInstance<DialogueGraph>();
            g.conversationId = corrupt ? "act2.tithe.after.bad" : "act2.tithe.after"; g.startNodeId = "0";
            g.nodes.Add(new DialogueNode
            {
                id = "0", speaker = "(Vane's table)",
                text = corrupt
                    ? "Vane's table is busier than ever, and the pauper's pit is fuller. Business, as they say, is good — " +
                      "and a cut of it is yours. The flowers out past the wall are for the dead nobody could afford to bless."
                    : "Vane's collection table sits empty, the ledger gone. Out past the wall, the pauper's pit has fresh " +
                      "flowers on it — the kind the poor leave when, for once, someone let the dead rest for free.",
                choices = new[] { new DialogueChoice { text = "Move on.", nextNodeId = "bye" } }
            });
            g.nodes.Add(Line("bye", "(Vane's table)", "You leave the ledger's corner behind."));
            return g;
        }

        // ---------- The Last Letter (a dying soldier, an old wrong) ----------

        private DialogueGraph Letter()
        {
            var g = ScriptableObject.CreateInstance<DialogueGraph>();
            g.conversationId = "act2.letter"; g.startNodeId = "0";
            g.nodes.Add(new DialogueNode
            {
                id = "0", speaker = "Old Davyn",
                text = "Returned. You'll do. I'm dying — don't argue, I can hear the rattle same as you. There's a letter " +
                       "under my cot. To a man I got killed, forty years back, by being a coward at the wrong gate. I " +
                       "wrote the truth of it. Never had the spine to send it. His daughter's still in the Gate.",
                choices = new[]
                {
                    new DialogueChoice
                    {
                        text = "I'll put it in her hand. She deserves the truth. (deliver)", nextNodeId = "deliver",
                        effects = new[] { Set("quest.letter.resolved"), Set("quest.letter.delivered"),
                                          Add("reputation.lowcity", 1), Add("companion.garrow.approval", 4) }
                    },
                    new DialogueChoice
                    {
                        text = "Some truths only wound. I'll burn it — let her keep her father whole. (burn)", nextNodeId = "burn",
                        effects = new[] { Set("quest.letter.resolved"), Set("quest.letter.burned"),
                                          Add("companion.ilfaeril.approval", 3) }
                    },
                    new DialogueChoice
                    {
                        text = "Let me read it back to you. You should hear it said aloud, once. (read)", nextNodeId = "read",
                        effects = new[] { Set("quest.letter.resolved"), Set("quest.letter.read"),
                                          Add("companion.garrow.approval", 5), Add("companion.ilfaeril.approval", 3) }
                    },
                }
            });
            g.nodes.Add(Line("deliver", "Old Davyn",
                "...Aye. Aye, she does. (He sags, lighter by forty years.) Tell her her father was braver than the man " +
                "who outlived him. Tell her I said so. Go on — before I lose the nerve I never had."));
            g.nodes.Add(Line("burn", "Old Davyn",
                "(He watches the page curl in your hand till it's ash, and something in his face un-clenches.) Maybe " +
                "that's the kinder lie. Maybe I'm a coward to the last. ...But she keeps her father a hero, and I take " +
                "the truth down with me where it can't cut her. Thank you."));
            g.nodes.Add(Line("read", "Old Davyn",
                "(You read it low, the whole wretched honest thing, and the old man weeps without shame — the first " +
                "time, he says, in forty years.) That's it. That's the sound of it out of me at last. Do with the page " +
                "what your conscience says. I've already had the only part that was mine to need."));
            return g;
        }

        private DialogueGraph LetterAfter()
        {
            var g = ScriptableObject.CreateInstance<DialogueGraph>();
            g.conversationId = "act2.letter.after"; g.startNodeId = "0";
            g.nodes.Add(new DialogueNode
            {
                id = "0", speaker = "(Davyn's cot)",
                text = "The cot is empty, the blanket folded. Old Davyn went quiet in the night — easier, the other " +
                       "veterans say, than a man with his ledger had any right to. Whatever you did with his letter, he " +
                       "went out of the world a little lighter for your visit.",
                choices = new[] { new DialogueChoice { text = "Rest, soldier.", nextNodeId = "bye" } }
            });
            g.nodes.Add(Line("bye", "(Davyn's cot)", "Someone has left a Fist medallion on the pillow."));
            return g;
        }
    }
}
