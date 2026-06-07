using UnityEngine;
using SunderedCrown.Dialogue;
using SunderedCrown.Stats;

namespace SunderedCrown.Content
{
    /// <summary>
    /// Content for Roen's personal quest, "The Honest Lie." Three conversations: arriving at the silent
    /// Harper safehouse, cornering his sister Wrenna (the reveal), and the post-fight moral resolution
    /// (forgive / condemn / a Persuasion gambit). The resolution sets quest.roen.resolved plus an
    /// approval swing and a lasting consequence flag. Pure data; the scene wires these up.
    /// </summary>
    public class RoenQuestContent
    {
        public DialogueGraph Arrival { get; private set; }
        public DialogueGraph WrennaReveal { get; private set; }
        public DialogueGraph Resolution { get; private set; }

        /// <summary>The data-driven config the shared PersonalQuestScene runs.</summary>
        public PersonalQuest Quest { get; private set; }

        public RoenQuestContent()
        {
            BuildArrival();
            BuildReveal();
            BuildResolution();

            Quest = new PersonalQuest
            {
                id = "roen",
                arrivedFlag = "roen.quest.arrived",
                clearedFlag = "roen.quest.cleared",
                resolvedFlag = "quest.roen.resolved",
                arrival = Arrival,
                reveal = WrennaReveal,
                resolution = Resolution,
                background = new Color(0.10f, 0.10f, 0.13f),
                examineLabel = "The silent safehouse",
                examineText =
                    "Harper sigils scratched off the doorframe. A teacup, still half full, gone cold. Two years of " +
                    "someone living a careful double life — and the fresh boot-prints of the people who came to end it.",
                revealNpcLabel = "Wrenna, cornered",
                fightId = "roen",
                fightLabel = "Cut down the Doomguide cell (battle)",
                resolutionNpcLabel = "Wrenna, freed — make the call",
                leaveLabel = "Leave the safehouse — back to the Gate",
            };
        }

        private void BuildArrival()
        {
            var g = ScriptableObject.CreateInstance<DialogueGraph>();
            g.conversationId = "roen.quest.arrival"; g.startNodeId = "0";
            g.nodes.Add(new DialogueNode
            {
                id = "0", speaker = "Roen Alleywind",
                text = "This is the safehouse the cipher named. Harper-warded, supposedly. But the wards are cold, " +
                       "the door's off the latch, and there's no countersign chalked on the step.\n\n" +
                       "Someone I care about ran this house. I told you I had a person I'd pull from the Wall. " +
                       "I didn't tell you she's my sister. Wrenna. And I didn't tell you she's not in the Wall yet.",
                choices = new[]
                {
                    new DialogueChoice { text = "Why lie about it?", nextNodeId = "1" },
                    new DialogueChoice { text = "Then let's get her out. Lead on.", nextNodeId = "2" },
                }
            });
            g.nodes.Add(new DialogueNode
            {
                id = "1", speaker = "Roen Alleywind",
                text = "Because if you knew I had something to lose, you'd know exactly where to cut me. Habit. " +
                       "The Harpers train it into you young, and family trains it younger.",
                autoNextNodeId = "2"
            });
            g.nodes.Add(new DialogueNode
            {
                id = "2", speaker = "Roen Alleywind",
                text = "Doomguide sign on the lintel — fresh. Kelemvor's people have been here, and they don't " +
                       "leave witnesses, only verdicts. Knives out. Quietly.",
                onEnter = new[] { new FlagClause { key = "roen.quest.arrived", op = FlagOp.SetTrue } }
            });
            Arrival = g;
        }

        private void BuildReveal()
        {
            var g = ScriptableObject.CreateInstance<DialogueGraph>();
            g.conversationId = "roen.quest.reveal"; g.startNodeId = "0";
            g.nodes.Add(new DialogueNode
            {
                id = "0", speaker = "Wrenna Alleywind",
                text = "Roen. Gods — Roen, you have to go, now, before they— \n\n" +
                       "(She stops. Looks at you. Something in her face folds shut.) \n\n" +
                       "...You brought the Returned here. To my house. Do you have any idea what they'd give to have " +
                       "this one on an altar?",
                choices = new[]
                {
                    new DialogueChoice { text = "\"They.\" The Doomguides. You've been talking to them.", nextNodeId = "1" },
                    new DialogueChoice { text = "Wrenna — what did you do?", nextNodeId = "1" },
                }
            });
            g.nodes.Add(new DialogueNode
            {
                id = "1", speaker = "Wrenna Alleywind",
                text = "They had a writ with your name on it, Roen. The Wall, for 'consorting with the Returned.' " +
                       "I had a choice: hand them Harper routes, names, drops — or watch my brother mortared into " +
                       "nothing while I lived. So I lied. To the Harpers, to you, to everyone. For two years.",
                onEnter = new[] { new FlagClause { key = "roen.quest.truth_known", op = FlagOp.SetTrue } },
                choices = new[]
                {
                    new DialogueChoice { text = "(Say nothing. The Doomguides are coming.)", nextNodeId = "2" },
                }
            });
            g.nodes.Add(new DialogueNode
            {
                id = "2", speaker = "Roen Alleywind",
                text = "...Two years. You sold the Harpers to keep me breathing, and let me think I owed *you* nothing. " +
                       "We'll have that fight later. Right now there are six Doomguides between her and the door. " +
                       "Decide who you are after we live."
            });
            WrennaReveal = g;
        }

        private void BuildResolution()
        {
            var g = ScriptableObject.CreateInstance<DialogueGraph>();
            g.conversationId = "roen.quest.resolution"; g.startNodeId = "0";

            g.nodes.Add(new DialogueNode
            {
                id = "0", speaker = "Roen Alleywind",
                text = "The cell's down. The safehouse is ours — what's left of it. And Wrenna's standing here a " +
                       "traitor to the Harpers and the only family I've got, in the same body. \n\n" +
                       "I can't make this call clean. Help me make it.",
                choices = new[]
                {
                    new DialogueChoice
                    {
                        text = "She did it to save you. Take her home. (forgive)", nextNodeId = "forgive",
                        effects = new[]
                        {
                            new FlagClause { key = "quest.roen.resolved", op = FlagOp.SetTrue },
                            new FlagClause { key = "quest.roen.wrenna_saved", op = FlagOp.SetTrue },
                            new FlagClause { key = "companion.roen.approval", op = FlagOp.AddInt, amount = 20 },
                        }
                    },
                    new DialogueChoice
                    {
                        text = "She betrayed the Harpers. The rules exist for a reason. (condemn)", nextNodeId = "condemn",
                        effects = new[]
                        {
                            new FlagClause { key = "quest.roen.resolved", op = FlagOp.SetTrue },
                            new FlagClause { key = "quest.roen.harper_boon", op = FlagOp.SetTrue },
                            new FlagClause { key = "companion.roen.approval", op = FlagOp.AddInt, amount = -15 },
                        }
                    },
                    new DialogueChoice
                    {
                        text = "[Persuasion] Neither. We turn her two years of lies on the Doomguides.",
                        nextNodeId = "gambit_win", failNodeId = "gambit_fail",
                        checkAbility = Ability.Charisma, checkDC = 15,
                    },
                }
            });

            g.nodes.Add(new DialogueNode
            {
                id = "forgive", speaker = "Roen Alleywind",
                text = "Yeah. Yeah — come here, you idiot. (He pulls her in.) We'll explain it to the Harpers " +
                       "together, and if they don't like it, I know every alley out of this city.\n\n" +
                       "I won't forget that you let me choose mercy. Whatever's at the end of your road, Returned — " +
                       "I'm on it."
            });

            g.nodes.Add(new DialogueNode
            {
                id = "condemn", speaker = "Roen Alleywind",
                text = "(A long silence.) ...You're right. You're right, and I hate that you're right. She turns " +
                       "herself in. The Harpers get their routes back, their traitor, and a debt to us.\n\n" +
                       "It's the correct call. Don't expect me to thank you for it. But here — everything the cell " +
                       "carried. Use it well."
            });

            g.nodes.Add(new DialogueNode
            {
                id = "gambit_win", speaker = "Roen Alleywind",
                text = "...Gods. That could *work*. Wrenna's still on their books as an asset — they don't know the " +
                       "cell's dead. We feed them what *we* choose, and walk a knife straight into Kelemvor's people.\n\n" +
                       "You didn't just save my sister. You turned her into the best weapon we've got. Remind me " +
                       "never to play cards against you.",
                onEnter = new[]
                {
                    new FlagClause { key = "quest.roen.resolved", op = FlagOp.SetTrue },
                    new FlagClause { key = "quest.roen.wrenna_saved", op = FlagOp.SetTrue },
                    new FlagClause { key = "quest.roen.harper_boon", op = FlagOp.SetTrue },
                    new FlagClause { key = "quest.roen.double_agent", op = FlagOp.SetTrue },
                    new FlagClause { key = "companion.roen.approval", op = FlagOp.AddInt, amount = 25 },
                }
            });

            g.nodes.Add(new DialogueNode
            {
                id = "gambit_fail", speaker = "Wrenna Alleywind",
                text = "It's a pretty idea. It's also how people like me get caught and people like him get a writ. " +
                       "No. You don't get to gamble my brother on your cleverness. \n\n" +
                       "Choose plainly, Returned: do I go home with him, or do I turn myself in?",
                choices = new[]
                {
                    new DialogueChoice
                    {
                        text = "Take her home. (forgive)", nextNodeId = "forgive",
                        effects = new[]
                        {
                            new FlagClause { key = "quest.roen.resolved", op = FlagOp.SetTrue },
                            new FlagClause { key = "quest.roen.wrenna_saved", op = FlagOp.SetTrue },
                            new FlagClause { key = "companion.roen.approval", op = FlagOp.AddInt, amount = 20 },
                        }
                    },
                    new DialogueChoice
                    {
                        text = "She turns herself in. (condemn)", nextNodeId = "condemn",
                        effects = new[]
                        {
                            new FlagClause { key = "quest.roen.resolved", op = FlagOp.SetTrue },
                            new FlagClause { key = "quest.roen.harper_boon", op = FlagOp.SetTrue },
                            new FlagClause { key = "companion.roen.approval", op = FlagOp.AddInt, amount = -15 },
                        }
                    },
                }
            });

            Resolution = g;
        }
    }
}
