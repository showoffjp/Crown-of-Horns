using UnityEngine;
using SunderedCrown.Dialogue;
using SunderedCrown.Stats;

namespace SunderedCrown.Content
{
    /// <summary>
    /// Content for Varra's personal quest, "The Bill Comes Due." Her patron is calling the contract she
    /// signed at six years old — early. The trail leads to a deconsecrated chapel where the broker who
    /// signed for her, a cambion called Quill, waits to collect. Three conversations (arrival, the
    /// confrontation with Quill, the resolution) and a moral trilemma about who pays. Built on the
    /// shared PersonalQuest config, so the scene code is reused wholesale.
    /// </summary>
    public class VarraQuestContent
    {
        public DialogueGraph Arrival { get; private set; }
        public DialogueGraph QuillReveal { get; private set; }
        public DialogueGraph Resolution { get; private set; }
        public PersonalQuest Quest { get; private set; }

        public VarraQuestContent()
        {
            BuildArrival();
            BuildReveal();
            BuildResolution();

            Quest = new PersonalQuest
            {
                id = "varra",
                arrivedFlag = "varra.quest.arrived",
                clearedFlag = "varra.quest.cleared",
                resolvedFlag = "quest.varra.resolved",
                arrival = Arrival,
                reveal = QuillReveal,
                resolution = Resolution,
                background = new Color(0.14f, 0.08f, 0.10f),
                examineLabel = "The deconsecrated chapel",
                examineText =
                    "A chapel to a god nobody remembers, its altar re-carved with a newer, hungrier sigil. The air " +
                    "tastes of sulfur and old incense. On the lectern, a contract in a child's careful letters — and " +
                    "a countersignature in a hand that writes in something darker than ink.",
                revealNpcLabel = "Quill, the broker (a cambion)",
                fightId = "varra",
                fightLabel = "Break the collection (battle)",
                resolutionNpcLabel = "The contract — decide how it ends",
                leaveLabel = "Leave the chapel — back to the Gate",
            };
        }

        private void BuildArrival()
        {
            var g = ScriptableObject.CreateInstance<DialogueGraph>();
            g.conversationId = "varra.quest.arrival"; g.startNodeId = "0";
            g.nodes.Add(new DialogueNode
            {
                id = "0", speaker = "Varra",
                text = "So. The invoice didn't lie. This is where I signed — or where the thing that signed *for* me " +
                       "did. I was six. I remember the candles and a man with a kind voice and too many teeth, and " +
                       "then I remember being able to throw fire and never being able to sleep.",
                choices = new[]
                {
                    new DialogueChoice { text = "You were a child. A child can't sign anything.", nextNodeId = "1" },
                    new DialogueChoice { text = "Then let's tear the contract up. Together.", nextNodeId = "2" },
                }
            });
            g.nodes.Add(new DialogueNode
            {
                id = "1", speaker = "Varra",
                text = "Tell that to the Hells' clerks. Down there, intent is a technicality and signatures are " +
                       "scripture. A child can't sign — but a *broker* can sign on a child's behalf, for a cut. " +
                       "That's who's waiting in there. The one who sold me. I've wanted to meet him my whole life.",
                autoNextNodeId = "2"
            });
            g.nodes.Add(new DialogueNode
            {
                id = "2", speaker = "Varra",
                text = "His name's Quill. Don't let the smile fool you — everything in that chapel is a clause, " +
                       "including the floor. Whatever he offers you, the price is always in a footnote. Ready?",
                onEnter = new[] { new FlagClause { key = "varra.quest.arrived", op = FlagOp.SetTrue } }
            });
            Arrival = g;
        }

        private void BuildReveal()
        {
            var g = ScriptableObject.CreateInstance<DialogueGraph>();
            g.conversationId = "varra.quest.reveal"; g.startNodeId = "0";
            g.nodes.Add(new DialogueNode
            {
                id = "0", speaker = "Quill",
                text = "Varra! Look at you, all grown and overdue. And you've brought the Returned — oh, this is " +
                       "*delicious.* A soul no god will claim, standing next to a soul a Hell already owns. Do you " +
                       "know what the two of you are worth in the right ledger?",
                choices = new[]
                {
                    new DialogueChoice { text = "She was six. You sold a child.", nextNodeId = "1" },
                    new DialogueChoice { text = "What do you actually want, Quill?", nextNodeId = "1" },
                }
            });
            g.nodes.Add(new DialogueNode
            {
                id = "1", speaker = "Quill",
                text = "I want what I'm owed, and a promotion. The patron wants Varra's soul matured and collected. " +
                       "*I* want to renegotiate upward — and you, Returned, are the collateral that lets me. Sign her " +
                       "term over to me, take a boon, walk away clean. She was always going to pay. Why should you?",
                onEnter = new[] { new FlagClause { key = "varra.quest.terms_known", op = FlagOp.SetTrue } },
                choices = new[]
                {
                    new DialogueChoice { text = "(Refuse. Draw steel.)", nextNodeId = "2" },
                }
            });
            g.nodes.Add(new DialogueNode
            {
                id = "2", speaker = "Varra",
                text = "He always did talk like a contract reads. Here's my counter-offer, Quill: every fireball " +
                       "you ever gave me, all at once. Let's see which of us the footnotes favor.",
            });
            QuillReveal = g;
        }

        private void BuildResolution()
        {
            var g = ScriptableObject.CreateInstance<DialogueGraph>();
            g.conversationId = "varra.quest.resolution"; g.startNodeId = "0";

            g.nodes.Add(new DialogueNode
            {
                id = "0", speaker = "Varra",
                text = "Quill's down, and the contract's still on the lectern — glowing, waiting for a hand. It can't " +
                       "just be torn; the Hells don't recognize destruction, only transfer. Someone has to be named " +
                       "as holder, or named as paid.\n\nI've read it a hundred times in my nightmares. Three ways " +
                       "this ends. Help me pick.",
                choices = new[]
                {
                    new DialogueChoice
                    {
                        text = "Name me the holder. I'll carry your debt. (take the burden)", nextNodeId = "burden",
                        effects = new[]
                        {
                            new FlagClause { key = "quest.varra.resolved", op = FlagOp.SetTrue },
                            new FlagClause { key = "quest.varra.debt_taken", op = FlagOp.SetTrue },
                            new FlagClause { key = "companion.varra.approval", op = FlagOp.AddInt, amount = 25 },
                        }
                    },
                    new DialogueChoice
                    {
                        text = "Burn it with your own fire. Owe it yourself, freely. (her choice)", nextNodeId = "freedom",
                        effects = new[]
                        {
                            new FlagClause { key = "quest.varra.resolved", op = FlagOp.SetTrue },
                            new FlagClause { key = "quest.varra.freed", op = FlagOp.SetTrue },
                            new FlagClause { key = "companion.varra.approval", op = FlagOp.AddInt, amount = 15 },
                        }
                    },
                    new DialogueChoice
                    {
                        text = "[Arcana] There's a third name in the fine print — the patron's own.",
                        nextNodeId = "loophole_win", failNodeId = "loophole_fail",
                        checkAbility = Ability.Intelligence, checkDC = 16,
                    },
                }
            });

            g.nodes.Add(new DialogueNode
            {
                id = "burden", speaker = "Varra",
                text = "...You'd put your name where mine is. After everyone who ever bought me sent a bill.\n\n" +
                       "(For once she isn't joking.) I won't let it ride forever — we'll find the loophole, you and " +
                       "I, before any clerk comes knocking. But tonight? Tonight I sleep. First time in twenty years. " +
                       "Thank you, Returned."
            });

            g.nodes.Add(new DialogueNode
            {
                id = "freedom", speaker = "Varra",
                text = "No. You don't get to be one more person who pays for me. This one's mine.\n\n" +
                       "(She presses her palm to the page and it goes up in violet fire — her fire, freely spent.) " +
                       "Still owed. Still doomed. But by my own hand now, on my own clock. You can't imagine how " +
                       "light that is. I'm with you to the end — and I choose it."
            });

            g.nodes.Add(new DialogueNode
            {
                id = "loophole_win", speaker = "Varra",
                text = "There — *there*, under the seventh seal. The patron countersigned in their true name to bind " +
                       "a child. Which means the contract binds *them* to the same court. Quill didn't sell me a debt. " +
                       "He sold me a leash with two collars.\n\nWe don't pay it. We *enforce* it — against them. " +
                       "Gods, Returned. You just made my patron my debtor.",
                onEnter = new[]
                {
                    new FlagClause { key = "quest.varra.resolved", op = FlagOp.SetTrue },
                    new FlagClause { key = "quest.varra.freed", op = FlagOp.SetTrue },
                    new FlagClause { key = "quest.varra.patron_bound", op = FlagOp.SetTrue },
                    new FlagClause { key = "companion.varra.approval", op = FlagOp.AddInt, amount = 30 },
                }
            });

            g.nodes.Add(new DialogueNode
            {
                id = "loophole_fail", speaker = "Varra",
                text = "Don't. I see you chasing the clever exit, and the clever exit is how clever people end up as " +
                       "footnotes. There's no trick here. There's just who pays.\n\nSo say it plainly: do you carry " +
                       "it for me, or do I burn it myself?",
                choices = new[]
                {
                    new DialogueChoice
                    {
                        text = "I'll carry it. (take the burden)", nextNodeId = "burden",
                        effects = new[]
                        {
                            new FlagClause { key = "quest.varra.resolved", op = FlagOp.SetTrue },
                            new FlagClause { key = "quest.varra.debt_taken", op = FlagOp.SetTrue },
                            new FlagClause { key = "companion.varra.approval", op = FlagOp.AddInt, amount = 25 },
                        }
                    },
                    new DialogueChoice
                    {
                        text = "Burn it yourself, freely. (her choice)", nextNodeId = "freedom",
                        effects = new[]
                        {
                            new FlagClause { key = "quest.varra.resolved", op = FlagOp.SetTrue },
                            new FlagClause { key = "quest.varra.freed", op = FlagOp.SetTrue },
                            new FlagClause { key = "companion.varra.approval", op = FlagOp.AddInt, amount = 15 },
                        }
                    },
                }
            });

            Resolution = g;
        }
    }
}
