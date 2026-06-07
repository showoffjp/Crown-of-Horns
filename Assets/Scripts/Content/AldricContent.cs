using UnityEngine;
using SunderedCrown.Dialogue;
using SunderedCrown.Stats;

namespace SunderedCrown.Content
{
    /// <summary>
    /// The Act I marquee conversation — "Tea with a Heretic." Aldric Morn, the gentle monster who is
    /// *right* about the Wall. Authored from docs/story/10 §1. Built in code so it can be dropped onto
    /// an NPC; in a full project this would be a DialogueGraph asset.
    /// </summary>
    public class AldricContent
    {
        public DialogueGraph TeaDialogue { get; private set; }

        public AldricContent()
        {
            var g = ScriptableObject.CreateInstance<DialogueGraph>();
            g.conversationId = "aldric.tea";
            g.startNodeId = "0";

            g.nodes.Add(new DialogueNode
            {
                id = "0", speaker = "Aldric Morn",
                text = "Sit. The tea's real — Calishite, smuggled, scandalous. That's rarer than honesty in this " +
                       "city, so I lead with it. You're the one who came back. I've wanted to meet you more than " +
                       "I've wanted anything in a long while.",
                autoNextNodeId = "1",
                onEnter = new[] { new FlagClause { key = "aldric.met", op = FlagOp.SetTrue } }
            });

            g.nodes.Add(new DialogueNode
            {
                id = "1", speaker = "Aldric Morn",
                text = "You know what I am — the Doomguides have papered the Gate with my face and the word " +
                       "HERETIC beneath it. So let me tell you the truth they won't: I mean to tear down the " +
                       "Wall of the Faithless. You've seen it. Then you know why.",
                choices = new[]
                {
                    new DialogueChoice
                    {
                        text = "[INSIGHT DC 15] You're not a fanatic. You're grieving.",
                        nextNodeId = "grief", checkAbility = Ability.Wisdom, checkDC = 15, failNodeId = "deflect"
                    },
                    new DialogueChoice
                    {
                        text = "[INSIGHT DC 16] Something is using you. You're a door too, and you don't know it.",
                        nextNodeId = "crown", checkAbility = Ability.Wisdom, checkDC = 16, failNodeId = "deflect"
                    },
                    new DialogueChoice { text = "How many die for your crusade?", nextNodeId = "cost" },
                    new DialogueChoice
                    {
                        text = "You're a murderer. I knew before the tea got cold.",
                        nextNodeId = "enemy",
                        effects = new[]
                        {
                            new FlagClause { key = "companion.garrow.approval", op = FlagOp.AddInt, amount = 5 },
                            new FlagClause { key = "faction.kelemvor.reputation", op = FlagOp.AddInt, amount = 5 },
                            new FlagClause { key = "aldric.named_monster", op = FlagOp.SetTrue },
                        }
                    },
                }
            });

            g.nodes.Add(new DialogueNode
            {
                id = "grief", speaker = "Aldric Morn",
                text = "...Her name was Maerin. Sixteen. She loved the world like it was a free gift from no one. " +
                       "When the Pox took her I followed her into the Fugue to watch my god be merciful — and " +
                       "watched Him wall her in instead. You're the first to see the father before the heretic. " +
                       "I won't forget that.",
                onEnter = new[] { new FlagClause { key = "aldric.grief_seen", op = FlagOp.SetTrue } },
                autoNextNodeId = "offer"
            });
            g.nodes.Add(new DialogueNode
            {
                id = "crown", speaker = "Aldric Morn",
                text = "...What a strange thing to say. The Crown of Horns is a tool. A means. I've held it in my " +
                       "mind for twenty years.\n(But something flickers behind his eyes, and it is not entirely " +
                       "his own.)\n...We'll speak of it again. When you've seen more.",
                onEnter = new[] { new FlagClause { key = "aldric.crown_doubt_planted", op = FlagOp.SetTrue } },
                autoNextNodeId = "offer"
            });
            g.nodes.Add(new DialogueNode
            {
                id = "deflect", speaker = "Aldric Morn",
                text = "Don't try to read me like a chapbook, friend. Better minds have. Drink your tea.",
                autoNextNodeId = "offer"
            });
            g.nodes.Add(new DialogueNode
            {
                id = "cost", speaker = "Aldric Morn",
                text = "Thousands. I have counted every one and I will answer for them. Against an *eternity* of " +
                       "the Faithless dissolving into nothing, it is the smallest price ever asked for the largest " +
                       "mercy. You think me a monster. I think me *late.*",
                onEnter = new[] { new FlagClause { key = "aldric.cost_revealed", op = FlagOp.SetTrue } },
                autoNextNodeId = "offer"
            });

            g.nodes.Add(new DialogueNode
            {
                id = "offer", speaker = "Aldric Morn",
                text = "I'm not asking for your loyalty — I don't trust loyalty bought cheap. Just your curiosity. " +
                       "Walk with me a while. Decide for yourself whether I'm a monster. If you decide I am — " +
                       "you'll know exactly where to find me.",
                choices = new[]
                {
                    new DialogueChoice
                    {
                        text = "I'll walk with you. For now.", nextNodeId = "ally",
                        effects = new[]
                        {
                            new FlagClause { key = "aldric.provisional", op = FlagOp.SetTrue },
                            new FlagClause { key = "faction.choir.reputation", op = FlagOp.AddInt, amount = 5 },
                        }
                    },
                    new DialogueChoice { text = "I'll decide. But I'm watching you.", nextNodeId = "neutral" },
                }
            });

            g.nodes.Add(new DialogueNode { id = "ally", speaker = "Aldric Morn",
                text = "Good. The tea's getting cold, and the road's long. We'll make a heretic of you yet — or you'll make an honest man of me. Either would be a mercy." });
            g.nodes.Add(new DialogueNode { id = "neutral", speaker = "Aldric Morn",
                text = "Watching is wiser than most manage. Watch closely. I've nothing to hide — only a great deal to answer for." });
            g.nodes.Add(new DialogueNode { id = "enemy", speaker = "Aldric Morn",
                text = "Yes. I am. I keep the count the way I once kept the names of the buried. But ask yourself, on your way to whoever you'll tell: every child in that Wall was put there by someone who also thought they were doing the smaller evil. Go with my blessing. I mean that. And my count." });

            TeaDialogue = g;
        }
    }
}
