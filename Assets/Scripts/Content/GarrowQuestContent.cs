using UnityEngine;
using SunderedCrown.Dialogue;
using SunderedCrown.Stats;

namespace SunderedCrown.Content
{
    /// <summary>
    /// Content for Garrow's personal quest, "A God-Shaped Hole." A black-edged writ summons Sister
    /// Garrow to a Kelemvorite tribunal to answer for heresy — for failing to lay her fallen mentor, and
    /// for travelling at the side of the Returned, a soul no god ever claimed (the Faithless made flesh).
    /// The Justiciar means to make her recant or be cast out; when the trial proves rigged, it becomes a
    /// fight. Three conversations (arrival / the Justiciar's charge / the resolution) and a moral trilemma
    /// about what she owes a faith whose law would damn the very people she loves. Built on the shared
    /// PersonalQuest config, so the scene code is reused wholesale.
    /// </summary>
    public class GarrowQuestContent
    {
        public DialogueGraph Arrival { get; private set; }
        public DialogueGraph JusticiarCharge { get; private set; }
        public DialogueGraph Resolution { get; private set; }
        public PersonalQuest Quest { get; private set; }

        public GarrowQuestContent()
        {
            BuildArrival();
            BuildCharge();
            BuildResolution();

            Quest = new PersonalQuest
            {
                id = "garrow",
                arrivedFlag = "garrow.quest.arrived",
                clearedFlag = "garrow.quest.cleared",
                resolvedFlag = "quest.garrow.resolved",
                arrival = Arrival,
                reveal = JusticiarCharge,
                resolution = Resolution,
                background = new Color(0.09f, 0.10f, 0.12f),
                examineLabel = "The Scales of the Doom",
                examineText =
                    "A black iron balance the height of a man, the Doomguide's seal: on one pan a feather, on the " +
                    "other a soul. The temple of Kelemvor judges the dead this way — and, when a Doomguide strays, " +
                    "the living. The feather pan has been weighted. Someone has already decided which way Garrow's " +
                    "verdict falls.",
                revealNpcLabel = "Justiciar Veld — the tribunal",
                fightId = "garrow",
                fightLabel = "Refuse the rigged verdict (battle)",
                resolutionNpcLabel = "The Scales — decide what she answers",
                leaveLabel = "Leave the temple — back to the Gate",
            };
        }

        private void BuildArrival()
        {
            var g = ScriptableObject.CreateInstance<DialogueGraph>();
            g.conversationId = "garrow.quest.arrival"; g.startNodeId = "0";
            g.nodes.Add(new DialogueNode
            {
                id = "0", speaker = "Sister Garrow",
                text = "Grey stone, grey light, grey robes. Home — or what I called it for thirty years. The writ says " +
                       "heresy. Three counts: that I did not lay Aldric when the church bid me forget him. That I have " +
                       "voiced doubt of the Doom. And that I walk beside *you* — a soul no god claimed, which to them " +
                       "is a wound that walks.",
                choices = new[]
                {
                    new DialogueChoice { text = "Doubt isn't heresy. It's the only honest prayer there is.", nextNodeId = "1" },
                    new DialogueChoice { text = "Then we leave. You owe a rigged court nothing.", nextNodeId = "2" },
                }
            });
            g.nodes.Add(new DialogueNode
            {
                id = "1", speaker = "Sister Garrow",
                text = "My father said the same, with a spade in his hand and no god in the conversation. The church " +
                       "disagrees. To the Doom, certainty *is* the sacrament — and I have spent my whole life being " +
                       "correct, and am no longer sure correct and good are the same word.",
                autoNextNodeId = "2"
            });
            g.nodes.Add(new DialogueNode
            {
                id = "2", speaker = "Sister Garrow",
                text = "No. I came here to answer, and I will. Not because I fear the verdict — because if I run, I " +
                       "never learn whether the thing I served for thirty years is as monstrous as I've begun to fear. " +
                       "The Justiciar inside is named Veld. He taught my catechism. Whatever he says in there, stay at " +
                       "my shoulder. I think today I find out what I believe.",
                onEnter = new[] { new FlagClause { key = "garrow.quest.arrived", op = FlagOp.SetTrue } }
            });
            Arrival = g;
        }

        private void BuildCharge()
        {
            var g = ScriptableObject.CreateInstance<DialogueGraph>();
            g.conversationId = "garrow.quest.charge"; g.startNodeId = "0";
            g.nodes.Add(new DialogueNode
            {
                id = "0", speaker = "Justiciar Veld",
                text = "Edda Garrow. You were the cleanest catechism I ever taught — a girl who buried the Faithless " +
                       "without weeping, because she understood the Wall was *just.* And here you stand reeking of " +
                       "compassion, with a creature beside you that the Wall itself spat back up. Kneel. Recant the " +
                       "doubt, disavow the Returned, and the Doom is merciful: you keep the grey.",
                choices = new[]
                {
                    new DialogueChoice { text = "You weighted the scales before she walked in.", nextNodeId = "1" },
                    new DialogueChoice { text = "Ask her what she believes — don't tell her.", nextNodeId = "1" },
                }
            });
            g.nodes.Add(new DialogueNode
            {
                id = "1", speaker = "Justiciar Veld",
                text = "Belief is not asked of a Doomguide; it is *issued.* The verdict is recorded already — it was " +
                       "recorded the day she chose your company over her vows. This trial is a courtesy, that she might " +
                       "kneel before she is broken to it. Sister. Last time. Renounce the Faithless thing, or be named " +
                       "one yourself.",
                onEnter = new[] { new FlagClause { key = "garrow.quest.charge_known", op = FlagOp.SetTrue } },
                choices = new[]
                {
                    new DialogueChoice { text = "(Stand at her shoulder. Let her refuse with steel.)", nextNodeId = "2" },
                }
            });
            g.nodes.Add(new DialogueNode
            {
                id = "2", speaker = "Sister Garrow",
                text = "Thirty years I weighed the dead by your feather, Veld, and called the trembling in my hands " +
                       "*reverence.* It was grief. I will not kneel to a verdict that was written before the question. " +
                       "If that makes me Faithless — then draw, and we will see whose god shows up for the fight.",
            });
            JusticiarCharge = g;
        }

        private void BuildResolution()
        {
            var g = ScriptableObject.CreateInstance<DialogueGraph>();
            g.conversationId = "garrow.quest.resolution"; g.startNodeId = "0";

            g.nodes.Add(new DialogueNode
            {
                id = "0", speaker = "Sister Garrow",
                text = "Veld's guard is down and the tribunal is scattered, but the temple still stands, and so does the " +
                       "record. They'll want my answer entered before I leave — recanted, or refused. The grey isn't a " +
                       "coat I can just drop; it's thirty years.\n\nSo say it with me. What do I answer the Scales?",
                choices = new[]
                {
                    new DialogueChoice
                    {
                        text = "Recant. Keep the grey — do the work from inside. (save her standing)", nextNodeId = "recant",
                        effects = new[]
                        {
                            new FlagClause { key = "quest.garrow.resolved", op = FlagOp.SetTrue },
                            new FlagClause { key = "quest.garrow.recanted", op = FlagOp.SetTrue },
                            new FlagClause { key = "companion.garrow.approval", op = FlagOp.AddInt, amount = 10 },
                        }
                    },
                    new DialogueChoice
                    {
                        text = "Walk away from the faith entirely. Lay the dead under no one's law. (her freedom)", nextNodeId = "apostate",
                        effects = new[]
                        {
                            new FlagClause { key = "quest.garrow.resolved", op = FlagOp.SetTrue },
                            new FlagClause { key = "quest.garrow.left_faith", op = FlagOp.SetTrue },
                            new FlagClause { key = "companion.garrow.approval", op = FlagOp.AddInt, amount = 20 },
                        }
                    },
                    new DialogueChoice
                    {
                        text = "[Religion] Put Kelemvor's own doctrine on trial — the Faithless judgment is the heresy.",
                        nextNodeId = "doctrine_win", failNodeId = "doctrine_fail",
                        checkAbility = Ability.Intelligence, checkDC = 16,
                    },
                }
            });

            g.nodes.Add(new DialogueNode
            {
                id = "recant", speaker = "Sister Garrow",
                text = "...I recant. The words taste like the order to forget Aldric did. (She kneels, and signs, and " +
                       "the feather pan settles smug.)\n\nThey keep their Doomguide. I keep the only thing that ever " +
                       "mattered — the right to be at the bedside when the priests won't come. It is a smaller faith " +
                       "now, and a harder one, and I will carry it brittle for the rest of my days. But I carry it. " +
                       "You stayed. I'll not forget that you stayed."
            });

            g.nodes.Add(new DialogueNode
            {
                id = "apostate", speaker = "Sister Garrow",
                text = "Then enter it: *Faithless, and unafraid.* (She unpins the grey clasp and sets it on the empty " +
                       "soul-pan, and walks out from under thirty years.)\n\nGods keep ledgers. So will I — my own. I'll " +
                       "lay the dead the way my father did, plainly, for anyone, under no law but the one in my own two " +
                       "hands. I should be terrified. Instead I feel like a held breath finally let go. Thank you — for " +
                       "being the proof that a soul no god claims can still be *good.*"
            });

            g.nodes.Add(new DialogueNode
            {
                id = "doctrine_win", speaker = "Sister Garrow",
                text = "*Read your own canon, Veld.* The Doom is not 'damn the unclaimed' — it is **'none shall pass " +
                       "unjudged.'** The Wall doesn't weigh the Faithless. It *discards* them, unweighed, ungrieved. " +
                       "That isn't the Doom's law. That's the church refusing to do its work — and calling its laziness " +
                       "scripture.\n\nThe heresy in this room was never my doubt. It's a faith that stopped reading. I " +
                       "don't leave the Doom and I don't kneel to its cowards. I take it *back.* From here — souls get " +
                       "weighed. Even the ones no god wanted. *Especially* those.",
                onEnter = new[]
                {
                    new FlagClause { key = "quest.garrow.resolved", op = FlagOp.SetTrue },
                    new FlagClause { key = "quest.garrow.doctrine_won", op = FlagOp.SetTrue },
                    new FlagClause { key = "companion.garrow.approval", op = FlagOp.AddInt, amount = 30 },
                }
            });

            g.nodes.Add(new DialogueNode
            {
                id = "doctrine_fail", speaker = "Sister Garrow",
                text = "Don't — don't hand me a clever reading of scripture to hide behind. I've hidden in correct " +
                       "answers my whole life; that's the sin that brought me here. There's no doctrine that makes this " +
                       "painless. There's only what I choose.\n\nSo plainly: do I kneel and keep the grey, or do I set " +
                       "it down and walk?",
                choices = new[]
                {
                    new DialogueChoice
                    {
                        text = "Keep the grey. Do the work from inside. (save her standing)", nextNodeId = "recant",
                        effects = new[]
                        {
                            new FlagClause { key = "quest.garrow.resolved", op = FlagOp.SetTrue },
                            new FlagClause { key = "quest.garrow.recanted", op = FlagOp.SetTrue },
                            new FlagClause { key = "companion.garrow.approval", op = FlagOp.AddInt, amount = 10 },
                        }
                    },
                    new DialogueChoice
                    {
                        text = "Set it down. Walk free. (her freedom)", nextNodeId = "apostate",
                        effects = new[]
                        {
                            new FlagClause { key = "quest.garrow.resolved", op = FlagOp.SetTrue },
                            new FlagClause { key = "quest.garrow.left_faith", op = FlagOp.SetTrue },
                            new FlagClause { key = "companion.garrow.approval", op = FlagOp.AddInt, amount = 20 },
                        }
                    },
                }
            });

            Resolution = g;
        }
    }
}
