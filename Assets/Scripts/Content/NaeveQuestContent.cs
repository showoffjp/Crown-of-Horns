using UnityEngine;
using SunderedCrown.Dialogue;
using SunderedCrown.Stats;

namespace SunderedCrown.Content
{
    /// <summary>
    /// Content for Naeve's personal quest, "After the Sky Fell." A shard of dead Weave hums with a
    /// frequency she hasn't heard since Netheril fell — a fragment of the Seventh Enclave, held aloft a
    /// thousand years past its death by a failing mythallar, calling its last daughter home. Inside, the
    /// echo of the Steward Vaelin tells her the truth: her own wardstone saved this piece, and keeping it
    /// frozen is draining the last live Weave in the world. The core's old protocol wakes to deny her, and
    /// then she must choose what a person owes the world they ended. Three conversations (arrival / the
    /// echo / the resolution) and a moral trilemma about whether some mistakes are carried, not erased.
    /// Built on the shared PersonalQuest config, so the scene code is reused wholesale.
    /// </summary>
    public class NaeveQuestContent
    {
        public DialogueGraph Arrival { get; private set; }
        public DialogueGraph VaelinEcho { get; private set; }
        public DialogueGraph Resolution { get; private set; }
        public PersonalQuest Quest { get; private set; }

        public NaeveQuestContent()
        {
            BuildArrival();
            BuildEcho();
            BuildResolution();

            Quest = new PersonalQuest
            {
                id = "naeve",
                arrivedFlag = "naeve.quest.arrived",
                clearedFlag = "naeve.quest.cleared",
                resolvedFlag = "quest.naeve.resolved",
                arrival = Arrival,
                reveal = VaelinEcho,
                resolution = Resolution,
                background = new Color(0.07f, 0.09f, 0.16f),
                examineLabel = "The failing mythallar",
                examineText =
                    "A sphere of arcane light the size of a cottage, throbbing slow and sick at the fragment's heart — " +
                    "the engine that held a city in the sky. A thousand years it has burned the last live Weave in the " +
                    "world to keep this one shard from finishing its fall. The light stutters. Somewhere inside it, " +
                    "something is still keeping time.",
                revealNpcLabel = "Vaelin — an echo in the Weave",
                fightId = "naeve",
                fightLabel = "Past the last protocol (battle)",
                resolutionNpcLabel = "The core — decide what becomes of home",
                leaveLabel = "Leave the fragment — back to the present",
            };
        }

        private void BuildArrival()
        {
            var g = ScriptableObject.CreateInstance<DialogueGraph>();
            g.conversationId = "naeve.quest.arrival"; g.startNodeId = "0";
            g.nodes.Add(new DialogueNode
            {
                id = "0", speaker = "Naeve",
                text = "Do you hear it? No — of course you don't. It's the resonance of a live mythallar, and there has " +
                       "not been one of those in this world for a thousand years. This is a piece of the Seventh " +
                       "Enclave. My Enclave. It never finished falling. It has been *up here,* humming my home's " +
                       "exact note, the whole time I thought I'd lost it.",
                choices = new[]
                {
                    new DialogueChoice { text = "Then let's go in. Carefully.", nextNodeId = "1" },
                    new DialogueChoice { text = "How is anything still aloft after the Weave died?", nextNodeId = "2" },
                }
            });
            g.nodes.Add(new DialogueNode
            {
                id = "1", speaker = "Naeve",
                text = "Carefully. Yes. I have not been careful with anything I loved since I was old enough to derive a " +
                       "proof, and look what that bought the sky. So — carefully. Stay close. The wards here knew my " +
                       "hand once. I do not know if a thousand years remembers.",
                autoNextNodeId = "3"
            });
            g.nodes.Add(new DialogueNode
            {
                id = "2", speaker = "Naeve",
                text = "It shouldn't be. That's what frightens me. A mythallar that loses its Weave goes dark and the " +
                       "city comes down — I *watched* it, all of them, at once. For this one to still be burning, " +
                       "someone left it a reserve. A calculation precise enough to ration a thousand years of magic " +
                       "down to a candle-flame. There were perhaps three people alive who could do that math. I was one.",
                autoNextNodeId = "3"
            });
            g.nodes.Add(new DialogueNode
            {
                id = "3", speaker = "Naeve",
                text = "So either a colleague I loved spent their last breath saving a rock — or I did, and forgot, and " +
                       "have spent a millennium grieving a thing that was waiting for me to come back. I have to know " +
                       "which. Whatever's at the core, I face it. Come.",
                onEnter = new[] { new FlagClause { key = "naeve.quest.arrived", op = FlagOp.SetTrue } }
            });
            Arrival = g;
        }

        private void BuildEcho()
        {
            var g = ScriptableObject.CreateInstance<DialogueGraph>();
            g.conversationId = "naeve.quest.echo"; g.startNodeId = "0";
            g.nodes.Add(new DialogueNode
            {
                id = "0", speaker = "Vaelin (an echo)",
                text = "Arcanist Naeve. You took your time. I am — was — Vaelin, Steward of the Seventh. What's left of " +
                       "me the mythallar keeps, the way it keeps everything: imperfectly, and forever. You want to know " +
                       "who saved this shard. Child. *You* did. You set the rationing wardstone in the last hour, while " +
                       "the rest of us screamed at Karsus's sky.",
                choices = new[]
                {
                    new DialogueChoice { text = "I don't remember doing that.", nextNodeId = "1" },
                    new DialogueChoice { text = "Then why does it feel like a tomb, not a rescue?", nextNodeId = "1" },
                }
            });
            g.nodes.Add(new DialogueNode
            {
                id = "1", speaker = "Vaelin (an echo)",
                text = "Because it is both. You saved a fragment by sentencing it to fall *slowly* — for a thousand " +
                       "years, instead of an afternoon. Everyone in it is still here. Still falling. Still mine to " +
                       "steward. And the candle is nearly out: the wardstone has been drinking the world's last live " +
                       "Weave to keep us, and there is almost none left. You came back at the very end of the math.",
                onEnter = new[] { new FlagClause { key = "naeve.quest.truth_known", op = FlagOp.SetTrue } },
                autoNextNodeId = "2"
            });
            g.nodes.Add(new DialogueNode
            {
                id = "2", speaker = "Vaelin (an echo)",
                text = "There is one mercy left to me: the core's old protocol. *Admit no one who would unmake the work.* " +
                       "I cannot tell anymore whether you have come to save us or to finish us — and the wards will not " +
                       "let either of us decide it with words. They are waking. I am sorry, Arcanist. Get to the core, " +
                       "and *then* choose. I find I would rather lose to you than to time.",
            });
            VaelinEcho = g;
        }

        private void BuildResolution()
        {
            var g = ScriptableObject.CreateInstance<DialogueGraph>();
            g.conversationId = "naeve.quest.resolution"; g.startNodeId = "0";

            g.nodes.Add(new DialogueNode
            {
                id = "0", speaker = "Naeve",
                text = "The wards are still. The core is open, and Vaelin's right — the wardstone is nearly dry. Whatever " +
                       "I do, I do it now, in the last live Weave my own younger hand set aside.\n\nThree ways this " +
                       "ends. I have run them a thousand times in my sleep and never once been brave enough to pick. " +
                       "Help me. Please.",
                choices = new[]
                {
                    new DialogueChoice
                    {
                        text = "Lock it in stasis. Keep the last of Netheril, whole, forever. (preserve)", nextNodeId = "preserve",
                        effects = new[]
                        {
                            new FlagClause { key = "quest.naeve.resolved", op = FlagOp.SetTrue },
                            new FlagClause { key = "quest.naeve.preserved", op = FlagOp.SetTrue },
                            new FlagClause { key = "companion.naeve.approval", op = FlagOp.AddInt, amount = 10 },
                        }
                    },
                    new DialogueChoice
                    {
                        text = "Let it finish falling. Lay your world to rest — and carry it. (release)", nextNodeId = "release",
                        effects = new[]
                        {
                            new FlagClause { key = "quest.naeve.resolved", op = FlagOp.SetTrue },
                            new FlagClause { key = "quest.naeve.released", op = FlagOp.SetTrue },
                            new FlagClause { key = "companion.naeve.approval", op = FlagOp.AddInt, amount = 20 },
                        }
                    },
                    new DialogueChoice
                    {
                        text = "[Arcana] The wardstone can give its Weave back — not embalm the dead, but feed the living.",
                        nextNodeId = "rekindle_win", failNodeId = "rekindle_fail",
                        checkAbility = Ability.Intelligence, checkDC = 16,
                    },
                }
            });

            g.nodes.Add(new DialogueNode
            {
                id = "preserve", speaker = "Naeve",
                text = "...I can't. I can't be the hand that ends it twice. (She seals the wardstone, and the shard stops " +
                       "falling — a snowflake caught mid-air, forever.)\n\nThey're safe now. Frozen, but safe; mine to " +
                       "keep. Don't say it. I know what it is — a tomb I'll visit and call a home, a mistake I refused " +
                       "to carry because clutching it was easier. I am not ready to let the sky finish. Maybe I never " +
                       "will be. But you stood here while I wasn't brave. That is its own kind of saving."
            });

            g.nodes.Add(new DialogueNode
            {
                id = "release", speaker = "Naeve",
                text = "No. A thing held out of its ending isn't saved — it's only made to fall forever, and I have " +
                       "asked enough people to fall forever. (She lifts her hand from the wardstone, and gently, the " +
                       "last of Netheril lets go.)\n\nIt's done. It fell. It was always going to fall, and I have spent " +
                       "a thousand years pretending my grief was a calculation I could redo. It isn't. Some mistakes " +
                       "you don't erase. You *carry* them. I think — I think I can carry this now. Because I'm not " +
                       "carrying it alone."
            });

            g.nodes.Add(new DialogueNode
            {
                id = "rekindle_win", speaker = "Naeve",
                text = "Wait — *wait.* The wardstone isn't a cage, it's a held breath. The Weave inside it isn't spent, " +
                       "it's *saved* — the last live magic in the world, and I have been treating it as a shroud. I can " +
                       "open my own hand. Let it go not down, but *out* — back into the dead Weave of the present, a " +
                       "seed instead of a tomb.\n\nThere's a notation under my old proof in a hand I almost know — it " +
                       "says the same thing, as if someone left me the answer to find. Netheril doesn't get to live " +
                       "again. But it gets to *give.* My people end as the thing that makes new magic possible. That's " +
                       "not erasing the mistake. That's the mistake, finally, building something.",
                onEnter = new[]
                {
                    new FlagClause { key = "quest.naeve.resolved", op = FlagOp.SetTrue },
                    new FlagClause { key = "quest.naeve.released", op = FlagOp.SetTrue },
                    new FlagClause { key = "quest.naeve.rekindled", op = FlagOp.SetTrue },
                    new FlagClause { key = "companion.naeve.approval", op = FlagOp.AddInt, amount = 30 },
                }
            });

            g.nodes.Add(new DialogueNode
            {
                id = "rekindle_fail", speaker = "Naeve",
                text = "Don't — don't hand me an elegant third door. Elegance is what I trusted last time, and it dropped " +
                       "a civilization out of the sky. There's no clever proof that makes this not hurt. There's only " +
                       "what I'm willing to carry.\n\nSo, plainly: do I freeze them and keep the tomb, or do I let the " +
                       "sky finish falling?",
                choices = new[]
                {
                    new DialogueChoice
                    {
                        text = "Keep them. Freeze the shard. (preserve)", nextNodeId = "preserve",
                        effects = new[]
                        {
                            new FlagClause { key = "quest.naeve.resolved", op = FlagOp.SetTrue },
                            new FlagClause { key = "quest.naeve.preserved", op = FlagOp.SetTrue },
                            new FlagClause { key = "companion.naeve.approval", op = FlagOp.AddInt, amount = 10 },
                        }
                    },
                    new DialogueChoice
                    {
                        text = "Let it fall. Lay it to rest. (release)", nextNodeId = "release",
                        effects = new[]
                        {
                            new FlagClause { key = "quest.naeve.resolved", op = FlagOp.SetTrue },
                            new FlagClause { key = "quest.naeve.released", op = FlagOp.SetTrue },
                            new FlagClause { key = "companion.naeve.approval", op = FlagOp.AddInt, amount = 20 },
                        }
                    },
                }
            });

            Resolution = g;
        }
    }
}
