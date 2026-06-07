using UnityEngine;
using SunderedCrown.Dialogue;
using SunderedCrown.Stats;

namespace SunderedCrown.Content
{
    /// <summary>
    /// Content for Ilfaeril's personal quest, "The Vote." An elven reliquary, its lock keyed to a name no
    /// living elf remembers but him, holds the last will of one of the souls his Crown-War council voted to
    /// unmake — the verdict that hardened, over millennia, into the Law of the Faithless. The reliquary's
    /// waking draws the Choir of the Unmade, who revere him as their first witness and would turn his guilt
    /// into a key. Past them, the reliquary opens for him alone, and the soul he damned offers the one thing
    /// he has spent ten thousand years refusing to accept. Three conversations (arrival / the Cantor / the
    /// resolution) and a moral trilemma about whether atonement is allowed to end. Built on the shared
    /// PersonalQuest config, so the scene code is reused wholesale.
    /// </summary>
    public class IlfaerilQuestContent
    {
        public DialogueGraph Arrival { get; private set; }
        public DialogueGraph CantorReveal { get; private set; }
        public DialogueGraph Resolution { get; private set; }
        public PersonalQuest Quest { get; private set; }

        public IlfaerilQuestContent()
        {
            BuildArrival();
            BuildReveal();
            BuildResolution();

            Quest = new PersonalQuest
            {
                id = "ilfaeril",
                arrivedFlag = "ilfaeril.quest.arrived",
                clearedFlag = "ilfaeril.quest.cleared",
                resolvedFlag = "quest.ilfaeril.resolved",
                arrival = Arrival,
                reveal = CantorReveal,
                resolution = Resolution,
                background = new Color(0.10f, 0.07f, 0.13f),
                examineLabel = "The reliquary, keyed to a forgotten name",
                examineText =
                    "An elven reliquary of impossible age, sealed by a name-lock — and the name graven on it belongs " +
                    "to no house any living elf could place. The metal is cold the way only ten thousand years is " +
                    "cold. It has not been opened because, until now, only one person left alive remembered the name " +
                    "that opens it. He is standing very still beside you.",
                revealNpcLabel = "The Pale Cantor of the Choir",
                fightId = "ilfaeril",
                fightLabel = "Deny the Choir the relic (battle)",
                resolutionNpcLabel = "The reliquary opens — for him alone",
                leaveLabel = "Leave the crypt — back to the Gate",
            };
        }

        private void BuildArrival()
        {
            var g = ScriptableObject.CreateInstance<DialogueGraph>();
            g.conversationId = "ilfaeril.quest.arrival"; g.startNodeId = "0";
            g.nodes.Add(new DialogueNode
            {
                id = "0", speaker = "Ilfaeril",
                text = "I knew this would find me eventually. Ten thousand years is a long time for a thing to wait, but " +
                       "patience is the one virtue the dead have in surplus. This reliquary belonged to Maerith of the " +
                       "Singing Vale — one of the people my court voted to *unmake.* Not kill. Unmake. We denied her " +
                       "dead even a grave to rest in. I raised my hand. I have never once raised it back down.",
                choices = new[]
                {
                    new DialogueChoice { text = "You were one vote. The Law took thousands.", nextNodeId = "1" },
                    new DialogueChoice { text = "You don't have to open it. Not today.", nextNodeId = "2" },
                }
            });
            g.nodes.Add(new DialogueNode
            {
                id = "1", speaker = "Ilfaeril",
                text = "Do not do that. Do not make it smaller to spare me. A thousand hands built the Wall, yes — and " +
                       "every one of them told itself it was only one hand. That arithmetic *is* the atrocity. I will " +
                       "not hide inside it. The name on this lock is hers, and I am the last creature alive who can " +
                       "still say it. That is not absolution. It is a debt with my name on the invoice.",
                autoNextNodeId = "3"
            });
            g.nodes.Add(new DialogueNode
            {
                id = "2", speaker = "Ilfaeril",
                text = "There is no 'not today' left in me, friend. I have guided ten thousand strangers gently past the " +
                       "Wall and never once dared face the single soul I owed. If I walk away from this, I am only " +
                       "what I have always been: a man performing mercy on everyone except the people he wronged. " +
                       "No. I open it.",
                autoNextNodeId = "3"
            });
            g.nodes.Add(new DialogueNode
            {
                id = "3", speaker = "Ilfaeril",
                text = "...But we are not alone in this crypt. That cold, underneath the old cold — that is the Choir. " +
                       "Of course they came. To them I am not a sinner; I am a *relic* — the first witness, the proof " +
                       "their unmaking is holy. They will want what's in that reliquary, and they will want me. Draw. " +
                       "They do not get to make my worst day into their scripture.",
                onEnter = new[] { new FlagClause { key = "ilfaeril.quest.arrived", op = FlagOp.SetTrue } }
            });
            Arrival = g;
        }

        private void BuildReveal()
        {
            var g = ScriptableObject.CreateInstance<DialogueGraph>();
            g.conversationId = "ilfaeril.quest.cantor"; g.startNodeId = "0";
            g.nodes.Add(new DialogueNode
            {
                id = "0", speaker = "The Pale Cantor",
                text = "Witness. *Witness.* Do you feel how the air leans toward you? Ten thousand years you have knelt " +
                       "at the edge of the Wall begging to be forgiven, and never once understood: you do not need " +
                       "forgiveness. You need only to be *right.* You voted to unmake the unclaimed. The Unmade simply " +
                       "agrees with you. Give us the relic, and we will give you the one mercy the gods never did — " +
                       "we will tell you that you were correct.",
                choices = new[]
                {
                    new DialogueChoice { text = "He's worth more than the worst thing he ever did.", nextNodeId = "1" },
                    new DialogueChoice { text = "(To Ilfaeril) Don't take the comfortable lie.", nextNodeId = "1" },
                }
            });
            g.nodes.Add(new DialogueNode
            {
                id = "1", speaker = "Ilfaeril",
                text = "You offer to end my guilt by proving I never should have had any. (He sets himself between the " +
                       "Cantor and the reliquary.) Cantor — that is not mercy. That is the most obscene thing anyone " +
                       "has said to me in ten thousand years. My guilt is the only monument those people have. I will " +
                       "not let you melt it down for a key.",
                onEnter = new[] { new FlagClause { key = "ilfaeril.quest.cantor_known", op = FlagOp.SetTrue } },
                choices = new[]
                {
                    new DialogueChoice { text = "(Stand with him. Drive the Choir off.)", nextNodeId = "2" },
                }
            });
            g.nodes.Add(new DialogueNode
            {
                id = "2", speaker = "Ilfaeril",
                text = "Maerith. I am sorry. I have brought a war to your door again. (He lifts his shield, and for the " +
                       "first time in an age it does not feel like a burden.) Let us earn the right to open it.",
            });
            CantorReveal = g;
        }

        private void BuildResolution()
        {
            var g = ScriptableObject.CreateInstance<DialogueGraph>();
            g.conversationId = "ilfaeril.quest.resolution"; g.startNodeId = "0";

            g.nodes.Add(new DialogueNode
            {
                id = "0", speaker = "Ilfaeril",
                text = "The Choir is scattered, and the reliquary is quiet, and it knows my voice. It is opening.\n\n" +
                       "(Inside: no weapon, no relic of power. A message-stone, set ten thousand years ago by a woman " +
                       "who knew that one day her judge might be the only one left to hear it. Maerith's voice fills " +
                       "the crypt — and it is not cursing him. It is *forgiving* him.)\n\nI... I do not know what to " +
                       "do with this. I prepared for her hate. I have no defense against her mercy. Tell me. What do " +
                       "I do?",
                choices = new[]
                {
                    new DialogueChoice
                    {
                        text = "Refuse it. Keep paying — guide the lost, forever. (the penance)", nextNodeId = "penance",
                        effects = new[]
                        {
                            new FlagClause { key = "quest.ilfaeril.resolved", op = FlagOp.SetTrue },
                            new FlagClause { key = "quest.ilfaeril.penance", op = FlagOp.SetTrue },
                            new FlagClause { key = "companion.ilfaeril.approval", op = FlagOp.AddInt, amount = 10 },
                        }
                    },
                    new DialogueChoice
                    {
                        text = "Accept it. You're allowed to be more than your worst vote. (be forgiven)", nextNodeId = "forgiven",
                        effects = new[]
                        {
                            new FlagClause { key = "quest.ilfaeril.resolved", op = FlagOp.SetTrue },
                            new FlagClause { key = "quest.ilfaeril.forgiven", op = FlagOp.SetTrue },
                            new FlagClause { key = "companion.ilfaeril.approval", op = FlagOp.AddInt, amount = 20 },
                        }
                    },
                    new DialogueChoice
                    {
                        text = "[Insight] She isn't closing your debt. She's handing you a charge.",
                        nextNodeId = "commission_win", failNodeId = "commission_fail",
                        checkAbility = Ability.Wisdom, checkDC = 16,
                    },
                }
            });

            g.nodes.Add(new DialogueNode
            {
                id = "penance", speaker = "Ilfaeril",
                text = "No. I cannot take it — not yet, perhaps not ever. To accept her forgiveness is to set down the " +
                       "weight, and the weight is the last true thing I carry of her and her people. So I will keep it. " +
                       "I will guide the lost past the Wall until the stars go out, and call that my answer.\n\n" +
                       "(He bows his head. It is devotion, and it is also a kind of hiding, and he half knows it.) " +
                       "Thank you for standing with me. Even a man who refuses rest is glad not to refuse it alone."
            });

            g.nodes.Add(new DialogueNode
            {
                id = "forgiven", speaker = "Ilfaeril",
                text = "...All right. (The word costs him more than any wound the Choir dealt.) All right, Maerith. I " +
                       "accept it. I accept that I am not only the hand I raised — that ten thousand years of trying " +
                       "to make it right is allowed, at last, to *count.*\n\n(He breathes out, and something ancient " +
                       "and rigid in him eases.) I had forgotten that forgiveness is a thing you do for the *living.* " +
                       "She freed me to stop. One day — not today, but one day — I think I might even rest. You gave " +
                       "me that. I will not waste it."
            });

            g.nodes.Add(new DialogueNode
            {
                id = "commission_win", speaker = "Ilfaeril",
                text = "Wait — listen to her words again. She does not say *be at peace.* She says *do not let it happen " +
                       "to anyone else.* She is not closing the account. She is *hiring* me.\n\nGods. Ten thousand " +
                       "years I treated her forgiveness as a door I had to be worthy to walk through — and it was a " +
                       "*sword* she was holding out, hilt-first, the whole time. I accept it. Not as an ending. As a " +
                       "commission. The Wall took her people; I will spend whatever I have left tearing a hole in it " +
                       "for the rest of them. That is the only forgiveness worth the name: the kind that puts you back " +
                       "to work. You didn't absolve me, friend. You *armed* me.",
                onEnter = new[]
                {
                    new FlagClause { key = "quest.ilfaeril.resolved", op = FlagOp.SetTrue },
                    new FlagClause { key = "quest.ilfaeril.forgiven", op = FlagOp.SetTrue },
                    new FlagClause { key = "quest.ilfaeril.commission", op = FlagOp.SetTrue },
                    new FlagClause { key = "companion.ilfaeril.approval", op = FlagOp.AddInt, amount = 30 },
                }
            });

            g.nodes.Add(new DialogueNode
            {
                id = "commission_fail", speaker = "Ilfaeril",
                text = "Do not reach for a clever reading of her words to make this easy on me. I have hidden inside " +
                       "elegant interpretations for ten thousand years; it is its own kind of cowardice. Her meaning " +
                       "is plain enough. The only question is mine to answer.\n\nSo say it simply: do I take what she " +
                       "offers, or do I keep paying?",
                choices = new[]
                {
                    new DialogueChoice
                    {
                        text = "Keep paying. Guide the lost, forever. (the penance)", nextNodeId = "penance",
                        effects = new[]
                        {
                            new FlagClause { key = "quest.ilfaeril.resolved", op = FlagOp.SetTrue },
                            new FlagClause { key = "quest.ilfaeril.penance", op = FlagOp.SetTrue },
                            new FlagClause { key = "companion.ilfaeril.approval", op = FlagOp.AddInt, amount = 10 },
                        }
                    },
                    new DialogueChoice
                    {
                        text = "Take what she offers. You're allowed to stop. (be forgiven)", nextNodeId = "forgiven",
                        effects = new[]
                        {
                            new FlagClause { key = "quest.ilfaeril.resolved", op = FlagOp.SetTrue },
                            new FlagClause { key = "quest.ilfaeril.forgiven", op = FlagOp.SetTrue },
                            new FlagClause { key = "companion.ilfaeril.approval", op = FlagOp.AddInt, amount = 20 },
                        }
                    },
                }
            });

            Resolution = g;
        }
    }
}
