using System.Collections.Generic;
using UnityEngine;
using SunderedCrown.Characters;
using SunderedCrown.Dialogue;
using SunderedCrown.Items;
using SunderedCrown.Stats;

namespace SunderedCrown.Content
{
    /// <summary>One riddle: a prompt, an answer (an object token to place, or a word to speak), a set
    /// of "clever-wrong" objects she rewards anyway, and her reactions.</summary>
    public class Riddle
    {
        public string id;
        public string prompt;
        public bool spoken;                 // true = speak the word at the brazier; false = place a token
        public string answerToken;          // expected token id (object riddles)
        public string[] spokenAnswers;      // accepted words (spoken riddles)
        public string[] cleverTokens;       // wrong tokens she finds delightful
        public string onSolve = "Correct. Oh, I do like you.";
        public string onClever = "Wrong — but *delightfully* wrong. Wit's worth more than truth in my vault.";
        public string onDull = "Mm. No. And not even an interesting no. Amuse me, little loop.";
        public bool locked;                 // e.g. Your Name — finale only
    }

    /// <summary>
    /// Content for THE LADY IN THE MARGINS and her Vault of Tens: the symbolic answer-tokens (as quest
    /// items), the riddles (ten object pedestals + the spoken brazier riddles + the secret 'name'), and
    /// her storyteller-guise dialogue. See docs/story/22_THE_LADY_IN_THE_MARGINS.md.
    /// </summary>
    public class RiddleContent
    {
        public readonly Dictionary<string, ItemDefinition> Tokens = new Dictionary<string, ItemDefinition>();
        public readonly Dictionary<string, ItemDefinition> Rewards = new Dictionary<string, ItemDefinition>();
        public readonly List<Riddle> Pedestals = new List<Riddle>();   // 10 object riddles
        public readonly List<Riddle> Spoken = new List<Riddle>();      // brazier (word) riddles
        public Riddle TheName;                                         // the secret 11th
        public DialogueGraph TallyIntro { get; private set; }
        public DialogueGraph RoamingRiddle { get; private set; }       // she pops in between eras

        public RiddleContent()
        {
            BuildTokens();
            BuildRewards();
            BuildRiddles();
            BuildDialogue();
        }

        // ---- the symbolic tokens (quest items the player places) ----
        private void BuildTokens()
        {
            (string id, string name)[] defs =
            {
                ("candle","a Candle"), ("key","a Key"), ("mirror","a Mirror"), ("hourglass","an Hourglass"),
                ("skull","a Skull"), ("map","a Map"), ("heart","a Heart"), ("book","a Book"),
                ("ring","a Ring"), ("dice","a pair of Dice"), ("coin","a Coin"), ("egg","an Egg"),
                ("sword","a Sword"), ("bell","a Bell"), ("lantern","a Lantern"),
            };
            foreach (var (id, name) in defs)
            {
                var it = ScriptableObject.CreateInstance<ItemDefinition>();
                it.itemId = id; it.displayName = name; it.kind = ItemKind.Quest; it.stackable = false;
                Tokens[id] = it;
                ItemDatabase.Register(it);
            }
        }

        // ---- her rewards (chaotic-neutral, thematic — never a +2 sword) ----
        private void BuildRewards()
        {
            // The Coin of the Tenth Guest: a nudge of fate when you'd otherwise fall (a strong heal).
            var nudge = HealAbility("Nudge of Fate", "3d8");
            var coin = RewardItem("coin_tenth", "Coin of the Tenth Guest",
                "She flips it, and for one breath the world owes you a favour. Quaff to be mended by fortune itself.",
                stackable: true);
            coin.useEffect = nudge;
            Rewards["coin_tenth"] = coin;

            // Her Favour: used when you'd lose someone — a far greater mending.
            var favour = HealAbility("Her Favour", "6d8");
            var fav = RewardItem("her_favour", "Her Favour",
                "A single intervention from the margin. Use it the moment before a loss; she does so love a rescue.");
            fav.kind = ItemKind.Consumable;   // usable, even though it doesn't stack
            fav.useEffect = favour;
            Rewards["her_favour"] = fav;

            // The Reader's Boon: proof you read her. A lore trophy (and a flag the finale checks).
            Rewards["readers_boon"] = RewardItem("readers_boon", "The Reader's Boon",
                "A note in the margin, in a hand you almost know: 'You read me. Once, the margin smiled.'");

            foreach (var r in Rewards.Values) ItemDatabase.Register(r);
        }

        private AbilityDefinition HealAbility(string name, string dice)
        {
            var a = ScriptableObject.CreateInstance<AbilityDefinition>();
            a.abilityName = name; a.isHeal = true; a.healDice = dice; a.damageType = DamageType.Radiant;
            a.targeting = TargetingMode.SingleAlly; a.rangeTiles = 6;
            return a;
        }

        private ItemDefinition RewardItem(string id, string name, string desc, bool stackable = false)
        {
            var it = ScriptableObject.CreateInstance<ItemDefinition>();
            it.itemId = id; it.displayName = name; it.description = desc;
            it.kind = stackable ? ItemKind.Consumable : ItemKind.Quest;
            it.slot = EquipSlot.None; it.stackable = stackable; it.valueGold = 0;
            return it;
        }

        private Riddle Obj(string id, string prompt, string answer, string solve, params string[] clever)
            => new Riddle { id = id, prompt = prompt, answerToken = answer, cleverTokens = clever, onSolve = solve };

        private Riddle Say(string id, string prompt, string solve, params string[] answers)
            => new Riddle { id = id, prompt = prompt, spoken = true, spokenAnswers = answers, onSolve = solve };

        private void BuildRiddles()
        {
            // The Vault of Tens — ten object pedestals.
            Pedestals.Add(Obj("candle",
                "I am tallest before my work's begun, and stumped and weeping at its end. I die a little with every hour I shine — the brighter I burn, the less I spend.",
                "candle", "A warm one to start. I like the ones who light the room before they read it.", "lantern"));
            Pedestals.Add(Obj("key",
                "I have teeth that have never known hunger, a throat that has never known sound. I am the question every locked door asks — and the only answer ever found.",
                "key", "Mm. You'd be amazed how many try to answer a door with their shoulder."));
            Pedestals.Add(Obj("mirror",
                "I keep a prisoner who is always you: he apes each breath, each brow, each sigh; he stands where you stand, lies where you lie — and dies the very instant you pass by.",
                "mirror", "Keep that one near. You're going to meet him. Properly. Soon."));
            Pedestals.Add(Obj("hourglass",
                "I am the one liar the gods allow: I count your falling sand true and fair — then one slow turn of a patient hand makes all my honest arithmetic air.",
                "hourglass", "Time's the only thing I lie about, and I do it by telling the truth and flipping it over. Watch the hand, not the sand."));
            Pedestals.Add(Obj("skull",
                "A crown once sat where the worm now sits; I held every thought a king possessed. I have given my whole face to the dark — and I grin, because I am finally at rest.",
                "skull", "Careful with that one. Somebody important is still *using* his."));
            Pedestals.Add(Obj("map",
                "I hold oceans that have never wet a finger, and peaks no boot has ever pressed. Fold me — and I put the whole wide world to sleep in a traveller's chest.",
                "map", "A good woman drew that. She believed in latitude and weather and nothing else. She's in the Wall now, of course."));
            Pedestals.Add(Obj("heart",
                "I am the drummer no eye has seen, keeping time in a cage of bone. March me, race me, break me — still I beat, until the one day I am left alone.",
                "heart", "The only clock that speeds up when it's frightened. Marvelous design."));
            Pedestals.Add(Obj("book",
                "I am stuffed with voices and cannot speak; I have lived a thousand lives and none. Pry me open, and a dead man's tongue will tell you things he told no one.",
                "book", "My native country, the inside of a book. I came out for the quiet."));
            Pedestals.Add(Obj("ring",
                "No beginning and no end have I, and I bind two restless souls as one. Melt me to a shapeless bead of gold — the vow I sealed is still not undone.",
                "ring", "He still wears the melt of it on a cord. He never once believed she was really gone. He was right, you know."));
            Pedestals.Add(Obj("dice",
                "We are fate's littlest councillors: cubes of old bone with eyes that can't see. Yet kings hold their breath, and even your *gods* lean close to read what we decree.",
                "dice", "'Even your gods.' I do love a riddle that knows who's *really* reading it. Roll well, little loop. Someone always is.", "coin"));

            // The brazier — spoken (word) riddles.
            Spoken.Add(Say("silence",
                "I am the one thing in all the worlds whose very naming is my end. Say me aloud, and I am already gone. What am I?",
                "And you broke me to prove you had me. Everyone does. I never tire of it.", "silence", "quiet"));
            Spoken.Add(Say("echo",
                "I'll not speak till I am spoken to, and then I always have the last word. Mock me from the throat of a mountain — your own voice mocks you back, unheard.",
                "You'll fight a roomful of those later. Wearing the faces you love. Remember they're only *you*, thrown back wrong.", "echo", "an echo"));
            Spoken.Add(Say("tomorrow",
                "I am promised to every living thing and, in the end, delivered to none. I am always one short step ahead — step, and already I have moved on.",
                "There's a man — you, in a worse coat — trying to abolish me. Don't let him. A story with no tomorrow is just a very nice page no one's allowed to turn.",
                "tomorrow", "the future"));
            Spoken.Add(Say("wall",
                "I am built of all the ones no one came for, and mortared with the love that went unclaimed. I unmake without anger, I judge without a tongue — and the longer you look, the more of you I'll one day name.",
                "...I didn't write that one. The *dungeon* wrote that one. Sometimes the margin isn't the only thing watching. Sometimes the story riddles back.",
                "wall", "the wall", "wall of the faithless", "the wall of the faithless"));

            var name = Say("yourname",
                "I am yours — and yet from the hour they gave me, I am the thing of yours you least will say; others wear me to summon, to bless, to damn you — and one day they will carve me where you lay.",
                "...No. Don't. Not *that* one. Not yet. The day you remember it, the game ends — for both of us. Keep it. I'll wait.",
                "my name", "your name", "name", "a name");
            name.locked = true;
            Spoken.Add(name);

            // The secret eleventh — her own identity (the twist).
            TheName = Say("hername",
                "I have asked you twenty, and you answered most with your hands. This last you answer with your whole life: I am no god and no fiend; I have worn a face you already trust and walked the whole road at your side, for no reason but the joy of it. I am the note in the margin, in a hand you almost know. Who am I?",
                "...There she is. There *you* are. You *read* me, after all this. That's the best gift anyone's given me since I left the book. Ask your boon, reader. You've earned the real one.",
                "naeve", "tally", "the lady in the margins", "lady in the margins", "the margin", "you", "myself");
        }

        private void BuildDialogue()
        {
            var g = ScriptableObject.CreateInstance<DialogueGraph>();
            g.conversationId = "lady.tally";
            g.startNodeId = "0";
            g.nodes.Add(new DialogueNode
            {
                id = "0", speaker = "Tally (a one-eyed storyteller)",
                text = "—and the hero died in the first chapter and came back *wrong.* Oh! Speak of the margin. " +
                       "Sit. I've a pouch for you and a question. The pouch first; you'll want it for the question. " +
                       "I keep a little vault between the pages — ten pedestals, a brazier, and the best company " +
                       "you'll ever lose a riddle to. Care to play?",
                choices = new[]
                {
                    new DialogueChoice
                    {
                        text = "Show me your Vault of Tens.", nextNodeId = "yes",
                        effects = new[] { new FlagClause { key = "vault.requested", op = FlagOp.SetTrue } }
                    },
                    new DialogueChoice { text = "Who ARE you?", nextNodeId = "who" },
                    new DialogueChoice { text = "Maybe later.", nextNodeId = "later" },
                }
            });
            g.nodes.Add(new DialogueNode { id = "who", speaker = "Tally",
                text = "A storyteller. A gambler. A note someone left in the margin of a very long book and forgot to erase. " +
                       "Which of those is true? *All of them, on a good day.* Now — the Vault?",
                choices = new[]
                {
                    new DialogueChoice
                    {
                        text = "Lead on.", nextNodeId = "yes",
                        effects = new[] { new FlagClause { key = "vault.requested", op = FlagOp.SetTrue } }
                    },
                    new DialogueChoice { text = "Not yet.", nextNodeId = "later" },
                }
            });
            g.nodes.Add(new DialogueNode { id = "yes", speaker = "Tally",
                text = "Wonderful. Mind the step between the pages — it's a long way down to the footnotes." });
            g.nodes.Add(new DialogueNode { id = "later", speaker = "Tally",
                text = "Mm. I'll be here. I'm always here. That's rather the whole tragedy of me." });
            TallyIntro = g;

            // A roaming, one-off riddle she offers when she pops in between eras (G-Man style).
            var r = ScriptableObject.CreateInstance<DialogueGraph>();
            r.conversationId = "lady.roaming1";
            r.startNodeId = "0";
            r.nodes.Add(new DialogueNode
            {
                id = "0", speaker = "Tally (between the pages)",
                text = "Fancy meeting you here, time-walker. A quick one — no vault required: I'll not speak till " +
                       "I'm spoken to, and then I always have the last word. Mock me from a mountain, and you're " +
                       "mocked in your own voice, unheard. What am I?",
                choices = new[]
                {
                    new DialogueChoice
                    {
                        text = "An echo.", nextNodeId = "right",
                        effects = new[]
                        {
                            new FlagClause { key = "riddle.amusement", op = FlagOp.AddInt, amount = 3 },
                            new FlagClause { key = "tally.roaming1.done", op = FlagOp.SetTrue },
                        }
                    },
                    new DialogueChoice { text = "A shadow.", nextNodeId = "wrong",
                        effects = new[] { new FlagClause { key = "tally.roaming1.done", op = FlagOp.SetTrue } } },
                    new DialogueChoice { text = "Silence.", nextNodeId = "wrong",
                        effects = new[] { new FlagClause { key = "tally.roaming1.done", op = FlagOp.SetTrue } } },
                }
            });
            r.nodes.Add(new DialogueNode { id = "right", speaker = "Tally",
                text = "Mm. You'll fight a roomful of those, later — wearing the faces you love. Keep the answer. " +
                       "Keep the warning. I'll be three pages ahead, as ever." });
            r.nodes.Add(new DialogueNode { id = "wrong", speaker = "Tally",
                text = "Ha — no. But you *tried*, and trying amuses me almost as much as winning. Almost. Off you go." });
            RoamingRiddle = r;
        }
    }
}
