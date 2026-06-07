using System.Collections.Generic;
using SunderedCrown.Characters;
using SunderedCrown.Core;

namespace SunderedCrown.Content
{
    /// <summary>
    /// The slow burns. Each romance is an *argument about one of the game's themes* — Garrow/faith,
    /// Roen/belonging, Naeve/guilt, Varra/worth — so falling in love is also taking a position on what
    /// the game is about. Six stages on a shared spine (Spark → Trust → Turn → Crisis → Choosing → The
    /// Last Night), each gated by BOTH an approval threshold AND progression (the previous stage, and for
    /// the Crisis the companion's resolved personal quest). Pure data, mirroring CampContent; CampScene
    /// offers the best-eligible beat at the fire, and the player chooses to deepen it or hold it platonic.
    /// Love here is never a reward — it's a stake: a romanced companion is the most likely finale anchor,
    /// and the epilogue (EndingResolver) keys off romance.&lt;id&gt;.consummated.
    /// </summary>
    public static class RomanceContent
    {
        public enum Stage { Spark, Trust, Turn, Crisis, Choosing, LastNight }

        // The shared spine, in order, with the flag-suffix each stage sets and the approval it asks for.
        private static readonly (Stage stage, string key, int threshold)[] Spine =
        {
            (Stage.Spark,     "spark",       20),
            (Stage.Trust,     "trust",       35),
            (Stage.Turn,      "turn",        50),
            (Stage.Crisis,    "crisis",      55),
            (Stage.Choosing,  "choosing",    65),
            (Stage.LastNight, "consummated", 75),
        };

        public class Beat
        {
            public string id;            // companion id, matches approval/quest flags
            public string nameMatch;     // substring to find them in the active party
            public Stage stage;
            public string setFlag;       // romance.<id>.<key>, set when the player deepens it
            public int threshold;        // approval required
            public string title;
            public string[] lines;       // the scene
            public string pursueLabel;   // the choice that deepens the romance
            public string pursueResult;  // narration after deepening
            public string holdLabel;     // the choice that keeps it platonic / not yet
            public string holdResult;    // narration after holding (non-destructive — beat stays available)
        }

        // ---- per-companion arcs (lines drawn from docs/story/16_ROMANCE_SCRIPTS.md) ----

        private static List<Beat> Garrow() => new List<Beat>
        {
            new Beat {
                stage = Stage.Spark, title = "Garrow — the first doubt said aloud",
                lines = new[] {
                    "Garrow cleans her mace in long, even strokes — a prayer with no god at the end of it.",
                    "\"I performed the rite ten thousand times before I let myself wonder if it sends them somewhere " +
                    "worse. You don't say that, in the grey. You just dig.\"",
                    "She finally looks at you. \"I don't know why I'm telling you. You're the one person who's actually " +
                    "*been* there. Tell me I'm wrong to wonder.\"" },
                pursueLabel = "\"You're not wrong. I'll wonder with you.\"",
                pursueResult = "She doesn't smile — Garrow doesn't, much. But she sets the mace down, and for a while you " +
                               "two just sit in the not-knowing together. Something starts there, in the dark.",
                holdLabel = "\"The dead are at peace. Believe that.\"",
                holdResult = "She nods, the way she nods at funerals. \"Of course. Thank you.\" The shutter comes back down.",
            },
            new Beat {
                stage = Stage.Trust, title = "Garrow — a small forbidden mercy",
                lines = new[] {
                    "She finds you before the others wake. \"There's a corpse at the edge of camp. Godless. The rite " +
                    "forbids me to lay it.\" A long pause. \"I'm going to anyway. I want a witness who won't report it.\"",
                    "It is the first thing she has ever trusted you with that could cost her everything." },
                pursueLabel = "Stand with her. Say nothing. Tell no one.",
                pursueResult = "You hold the lantern while she breaks her own church's law to give a stranger dignity. " +
                               "Walking back, her hand finds yours in the dark, just for a step, and lets go — but it happened.",
                holdLabel = "\"Garrow — this could end you. Don't.\"",
                holdResult = "She studies you, then nods. \"Wise. You're right.\" She lays it anyway, alone, and doesn't " +
                             "ask you again for a while.",
            },
            new Beat {
                stage = Stage.Turn, title = "Garrow — the thing she has no prayer for",
                lines = new[] {
                    "On a field of the dead, she says it without armor:",
                    "\"I have buried a thousand people and never once let myself believe I'd see any of them again. It " +
                    "was the only way to keep digging.\"",
                    "\"You walked *out* of the place I send them. You're the first thing in my whole life that's made me " +
                    "want to be wrong about all of it. And I don't have a rite for that. I don't have a *prayer* for " +
                    "that. I just have — you. Standing there. Being impossible.\"" },
                pursueLabel = "\"Then stop burying me before I'm gone. I'm right here.\"",
                pursueResult = "She lets out a breath she has held for forty years. \"...All right,\" she says, like a " +
                               "vow. \"All right.\" *(romance.garrow.turn)*",
                holdLabel = "\"You honor me. But my road ends at the Wall.\"",
                holdResult = "\"I know,\" she says softly. \"I've always known. Forget I — \" She doesn't finish. She " +
                             "doesn't ask again.",
            },
            new Beat {
                stage = Stage.Crisis, title = "Garrow — after the tribunal",
                lines = new[] {
                    "The fire's low. She came through her own heresy trial today, and she came to *you* with it.",
                    "\"Whatever's left of my faith, I chose it standing next to you. I need you to know the choosing " +
                    "wasn't only about the god. It was about who I wanted in the room while I lost him, or kept him.\"",
                    "\"It was you. It is always, lately, you.\"" },
                pursueLabel = "\"Then I'm in the room. For all of it. Always.\"",
                pursueResult = "She rests her forehead against yours — gravely, like a sacrament. Two people who deal in " +
                               "endings, deciding to risk a middle.",
                holdLabel = "\"I'm honored to have stood with you, as a friend.\"",
                holdResult = "\"A friend,\" she repeats, and makes herself mean it kindly. \"That's no small word from me. " +
                             "I'll keep it.\"",
            },
            new Beat {
                stage = Stage.Choosing, title = "Garrow — the other list",
                lines = new[] {
                    "She comes to you holding the worn book she writes in every night.",
                    "\"I keep a list. Everyone I've buried. I add to it nightly; it's the closest thing I have to a prayer.\"",
                    "\"Tonight I wrote your name at the top of a *different* list. The one of people I refuse to bury. I " +
                    "don't know how to keep a list like that. I've never had one. Teach me. Stay alive long enough to " +
                    "teach me.\"" },
                pursueLabel = "\"We'll keep that list together. Both our names on it.\"",
                pursueResult = "She writes a second name beneath yours — her own — the first time in forty years she has " +
                               "dared put herself among the *kept.* *(romance.garrow.choosing)*",
                holdLabel = "Hold her hand, but make no promise.",
                holdResult = "She closes the book. \"No promises. I understand them better than most.\" The list stays.",
            },
            new Beat {
                stage = Stage.LastNight, title = "Garrow — the last night",
                lines = new[] {
                    "Before the Court of the Dead. She has sat with too many people on their last night to lie on yours.",
                    "\"No promises. No someday. I won't pretend tomorrow isn't what it is. Just — here. You. Me. The " +
                    "fire. And me finally, *finally* believing, for one night, that the dead might not be gone forever.\"",
                    "\"You gave me that. Whatever happens at that Court — you already gave me that.\"" },
                pursueLabel = "Stay. Give her the night.",
                pursueResult = "For one night a woman who buries everyone lets herself believe in keeping someone. She " +
                               "does not weep. She just holds on. *(romance.garrow.consummated)*",
                holdLabel = "Sit with her till the fire dies, and no more.",
                holdResult = "\"This is enough,\" she says, and means it. \"This was always enough.\"",
            },
        };

        private static List<Beat> Roen() => new List<Beat>
        {
            new Beat {
                stage = Stage.Spark, title = "Roen — the reflex and the tell",
                lines = new[] {
                    "\"I flirt with everyone, it's a professional reflex, don't read into it.\" *(beat)*",
                    "\"...You're reading into it. Stop that. I can see you reading into it. Gods, you're as bad as me.\"" },
                pursueLabel = "\"Maybe I like being as bad as you.\"",
                pursueResult = "For half a second the performance drops and something real blinks through. He covers it " +
                               "with a grin — but you both saw it.",
                holdLabel = "\"Relax, Roen. It's just banter.\"",
                holdResult = "\"Just banter,\" he agrees, far too quickly, and changes the subject like a man fleeing a scene.",
            },
            new Beat {
                stage = Stage.Trust, title = "Roen — the reports he didn't write",
                lines = new[] {
                    "\"I had three reports due on you. I wrote none of them. Do you understand what that — no, you " +
                    "*don't*, the Harpers are the only family I've — \" He stops, genuinely thrown.",
                    "\"Why am I telling you the truth? I don't *do* the truth. You're a menace. This is a menace situation.\"" },
                pursueLabel = "\"Tell me one more true thing. I dare you.\"",
                pursueResult = "He does — something small and unguarded — and looks faintly ill at how good it feels. The " +
                               "con artist, conned by honesty.",
                holdLabel = "\"Your secret's safe. Don't sweat it.\"",
                holdResult = "Relief and disappointment war on his face. \"Right. Yeah. Safe. Great.\" He's quiet a while.",
            },
            new Beat {
                stage = Stage.Turn, title = "Roen — the most naked thing",
                lines = new[] {
                    "Stripped of his deflections after a hard fight:",
                    "\"I've conned everyone I've ever met into thinking I'm fine. My handlers, the Guild, the *mirror.* " +
                    "You're the first person I can't lie to, and it is the most naked I have ever felt with my clothes " +
                    "*on.*\"",
                    "\"I think that's what people mean when they say love, and I hate it, and I have never wanted anything " +
                    "to stop less in my life. Don't you dare stop.\"" },
                pursueLabel = "\"I'm not going anywhere. That's the whole point of me.\"",
                pursueResult = "He laughs, wet-eyed, furious at himself, delighted. \"A menace,\" he whispers. \"An " +
                               "absolute menace.\" *(romance.roen.turn)*",
                holdLabel = "\"You're tired, Roen. Get some sleep.\"",
                holdResult = "The shutter slams back, smooth as a lockpick. \"Course. Long day. Forget I got weird.\" He " +
                             "doesn't get weird again.",
            },
            new Beat {
                stage = Stage.Crisis, title = "Roen — the name he buried",
                lines = new[] {
                    "After his quest, in the dark, he does the most intimate thing he knows:",
                    "\"Roen Alleywind is a name I chose. The one I was born with, the orphan one — I buried it so deep I " +
                    "almost convinced myself I never had one.\" He says it now, quietly, only to you.",
                    "\"You're hunting your true name across all of time. Figured I'd lend you mine to practice on. Don't " +
                    "wear it out.\"" },
                pursueLabel = "Say his birth-name back to him, gently, like it matters.",
                pursueResult = "He flinches, then steadies, like a dislocated thing set right. \"Huh,\" he breathes. " +
                               "\"Still fits. Who knew.\"",
                holdLabel = "\"Roen suits you better. Keep it.\"",
                holdResult = "\"Yeah,\" he says, and tucks the old name back away. \"Yeah, you're probably right.\"",
            },
            new Beat {
                stage = Stage.Choosing, title = "Roen — a future tense",
                lines = new[] {
                    "\"I don't do 'after.' There's the job, the next job, the grave. But lately I keep *catching* " +
                    "myself. Imagining an after. An inn. Two chairs. You, complaining about the wine.\"",
                    "\"It terrifies me, because it means I've got something to lose now, and I spent my whole life making " +
                    "sure I never did. So. Congratulations. You ruined a perfectly good survival strategy. I'm yours.\"" },
                pursueLabel = "\"Two chairs. I'm holding you to it.\"",
                pursueResult = "He grins like a man who just bet everything on one card and finds he doesn't even want to " +
                               "look at the others. *(romance.roen.choosing)*",
                holdLabel = "\"Don't bet on an after with me. Bad odds.\"",
                holdResult = "\"Bad odds are my whole career,\" he says lightly — but he doesn't bring up the inn again.",
            },
            new Beat {
                stage = Stage.LastNight, title = "Roen — pretending, out loud",
                lines = new[] {
                    "He doesn't do solemn. So he plans the after out loud, knowing it might not come:",
                    "\"After this, we're getting that inn. The Wandering Niche — terrible name, you'll hate it. Two " +
                    "chairs. No missions. I'll teach you to cheat at cards; you'll be bad at it; best years of our lives.\"",
                    "\"I *know* what tomorrow is. I'm saying it anyway, with my whole chest. Because if I don't get the " +
                    "after — at least I got to *want* one. Now shut up and let me pretend with you, just for tonight.\"" },
                pursueLabel = "Pretend with him. Build the whole inn, room by room.",
                pursueResult = "You two furnish an imaginary future till the fire gutters — and for one night the after " +
                               "is real because you both chose to live in it. *(romance.roen.consummated)*",
                holdLabel = "\"Get some rest, Roen. Big day.\"",
                holdResult = "\"...Yeah,\" he says, the bit collapsing. \"Big day.\" He keeps the inn to himself.",
            },
        };

        private static List<Beat> Naeve() => new List<Beat>
        {
            new Beat {
                stage = Stage.Spark, title = "Naeve — a logical impossibility",
                lines = new[] {
                    "\"You are a logical impossibility, you realize. A soul that predates itself. I should find you " +
                    "horrifying.\"",
                    "She frowns, displeased with herself. \"Instead I find you — *interesting.* Do not let it go to your " +
                    "head. I once found a paradox interesting and it ended a civilization.\"" },
                pursueLabel = "\"Interesting is the best thing you could call anyone, isn't it.\"",
                pursueResult = "Her eyes sharpen with something that is not, quite, only intellect. \"...Yes,\" she admits, " +
                               "as if conceding a theorem. \"It is.\"",
                holdLabel = "\"I'm not a paradox to be solved, Naeve.\"",
                holdResult = "\"No,\" she says, retreating into cool courtesy. \"My apologies. I forget myself.\"",
            },
            new Beat {
                stage = Stage.Trust, title = "Naeve — the worst thing she ever did",
                lines = new[] {
                    "She shows you the proof — Karsus's theorem, the mathematics that fell a sky.",
                    "\"Everyone I have told either recoiled or absolved me. I want neither. I want someone to look at the " +
                    "worst thing I ever did, understand it completely, and *neither* run nor forgive.\"",
                    "\"Just — stay in the room with it. With me. Can you do that? Most cannot.\"" },
                pursueLabel = "Read it all the way through. Stay. Say nothing absolving.",
                pursueResult = "You stay. You do not flinch and you do not forgive. When you finish, she is looking at you " +
                               "like a constant she didn't know was in the equation.",
                holdLabel = "\"It wasn't your fault, Naeve. Karsus did that.\"",
                holdResult = "\"There it is,\" she says, almost gently. \"Absolution. I told you I didn't want it.\" She " +
                             "rolls the proof closed.",
            },
            new Beat {
                stage = Stage.Turn, title = "Naeve — you gave me back my guilt",
                lines = new[] {
                    "After she proves you're a loop, the argument turns personal:",
                    "\"Determinism is a *comfort* — if every step is forced, then my step, the one that ended the world, " +
                    "was not truly mine. And then *you* did the one thing my mathematics cannot model. You *chose.*\"",
                    "\"If you can choose, then so did I. Which means it *was* mine. You gave me back my guilt — the most " +
                    "appalling gift anyone has ever given me, and I find I love you for it, which is also impossible, " +
                    "and I am *done* arguing with the impossible where you're concerned.\"" },
                pursueLabel = "\"Then stop arguing and let yourself have this.\"",
                pursueResult = "A thousand years of certainty quietly revises one of its axioms. \"...Very well,\" she " +
                               "says, and it is the most romantic sentence she owns. *(romance.naeve.turn)*",
                holdLabel = "\"Don't love me for the guilt. That's not a foundation.\"",
                holdResult = "\"A fair objection,\" she says, stung and hiding it. \"Strike it from the record, then.\"",
            },
            new Beat {
                stage = Stage.Crisis, title = "Naeve — a person over a proof",
                lines = new[] {
                    "She burned her life's masterwork rather than risk it becoming the Unmade's tool — for you.",
                    "\"I valued a person over a proof tonight. I have never done that. I do not have a framework for it.\"",
                    "She looks at her empty hands. \"I find I do not want one. I want the person. Specifically. You.\"" },
                pursueLabel = "\"You don't need a framework for this one. Just me.\"",
                pursueResult = "She laces her fingers through yours and studies the join like a result she'll accept " +
                               "without proof — for the first time in her life.",
                holdLabel = "\"You burned it for the world, not for me.\"",
                holdResult = "\"Perhaps,\" she allows, withdrawing her hands. \"Let us call it the world, then. It is " +
                             "tidier.\"",
            },
            new Beat {
                stage = Stage.Choosing, title = "Naeve — premise, revised",
                lines = new[] {
                    "\"I built my entire understanding of existence on the premise that meaning is an illusion the " +
                    "living tell themselves. I would like to formally amend it.\"",
                    "\"Premise, revised: *you* are meaning. Not in the abstract. Specifically. Demonstrably. I have " +
                    "checked the work. It holds. I am, it turns out, in love, and I have the proof.\"" },
                pursueLabel = "\"I accept your revised premise. All of it.\"",
                pursueResult = "\"Good,\" she says, with the satisfaction of a thousand-year problem closed. \"Then we are " +
                               "agreed, and the universe is no longer empty.\" *(romance.naeve.choosing)*",
                holdLabel = "\"Careful. You hate being wrong about the universe.\"",
                holdResult = "\"I do,\" she says, retracting the amendment with visible effort. \"Noted. Withdrawn.\"",
            },
            new Beat {
                stage = Stage.LastNight, title = "Naeve — an open variable",
                lines = new[] {
                    "\"I cannot promise you tomorrow; I have run the equations and tomorrow is genuinely indeterminate — " +
                    "which, for me, is the most romantic thing I can offer you. An *open* variable.\"",
                    "She takes your hand, studies it like a proof. \"I have hated open variables my entire existence. " +
                    "Tonight I love them. Tonight I love everything I cannot solve, because all of it is *you.*\"",
                    "\"Stay. Let us be unsolved together, for one night, before the world makes us answer.\"" },
                pursueLabel = "Stay. Be gloriously unsolved with her.",
                pursueResult = "Two world-enders spend one night refusing to compute the future, and find the not-knowing " +
                               "is the closest either has come to peace. *(romance.naeve.consummated)*",
                holdLabel = "\"We should rest. Tomorrow needs us sharp.\"",
                holdResult = "\"Spoken like a solved problem,\" she says, wry and a little sad. \"Rest, then.\"",
            },
        };

        private static List<Beat> Varra() => new List<Beat>
        {
            new Beat {
                stage = Stage.Spark, title = "Varra — every flirt is a dare",
                lines = new[] {
                    "She looks you over like an appraiser. \"Careful, Returned. People who get close to me tend to read " +
                    "the fine print too late.\"",
                    "Every word is a dare: *prove me right. Leave.*" },
                pursueLabel = "\"I read fine print for fun. Try me.\"",
                pursueResult = "She blinks, recalibrating. The dare didn't land — and she has no script for someone who " +
                               "stays to read.",
                holdLabel = "\"Noted. I'll keep my distance.\"",
                holdResult = "\"Smart,\" she says, with a grin that doesn't reach her eyes. \"Everyone does, eventually.\"",
            },
            new Beat {
                stage = Stage.Trust, title = "Varra — the test she expects you to fail",
                lines = new[] {
                    "She picks a fight over nothing — sharp, cruel, surgical — and watches your face for the exact " +
                    "moment you decide she isn't worth it.",
                    "It's a thing she's done to everyone. The moment always comes. She's never once been wrong." },
                pursueLabel = "Don't take the bait. Stay. Stay anyway.",
                pursueResult = "The moment doesn't come. She runs out of cruelty before you run out of staying, and the " +
                               "silence afterward is the most honest thing between you yet.",
                holdLabel = "Give her the space she's demanding.",
                holdResult = "\"That's what I thought,\" she says, vindicated and hollow. She got what she wanted. She " +
                             "hates it.",
            },
            new Beat {
                stage = Stage.Turn, title = "Varra — worth selling",
                lines = new[] {
                    "Quietly, for once without the acid:",
                    "\"You know the worst thing about being sold? Some part of you agrees you were worth selling. I've " +
                    "spent a year refusing to agree, and I have no idea what to do with that except — apparently — " +
                    "this.\"",
                    "She won't look at you. \"Don't make me regret refusing.\"" },
                pursueLabel = "\"You were never the price they put on you. Never.\"",
                pursueResult = "Something in her unclenches that has been a fist since she was six. \"...Okay,\" she " +
                               "whispers. \"Okay. Don't be lying.\" *(romance.varra.turn)*",
                holdLabel = "\"Everyone's worth is complicated, Varra.\"",
                holdResult = "\"Right. Complicated.\" The fist closes again. \"Forget I said it. I always do.\"",
            },
            new Beat {
                stage = Stage.Crisis, title = "Varra — the receipt, not yet burned",
                lines = new[] {
                    "After her quest — the contract dealt with, one way or another — she sits closer than she ever has.",
                    "\"For the first time the math on me isn't a death sentence. I don't know who I am without the clock " +
                    "ticking. Terrifying. Might need someone to figure it out next to.\"",
                    "\"Strictly for the company. Obviously.\"" },
                pursueLabel = "\"I'll be the someone. Strictly for the company.\"",
                pursueResult = "She laughs — a real one, no edge to it — and lets her head rest against your shoulder like " +
                               "it's the most natural debt she's ever carried.",
                holdLabel = "\"You'll figure it out. You always do.\"",
                holdResult = "\"Yeah,\" she says, sitting back up. \"Alone's a skill. I'm great at it.\"",
            },
            new Beat {
                stage = Stage.Choosing, title = "Varra — un-priced",
                lines = new[] {
                    "\"Here's a thing no one's ever gotten from me, because no one's ever stayed long enough to earn it.\"",
                    "\"I'm choosing you. Not because you saved me, not because I owe you — I'm *done* owing. I'm choosing " +
                    "you because I want to, freely, off the books. That's the only currency I've got left that means " +
                    "anything.\"" },
                pursueLabel = "\"I'm choosing you back. Off the books. Forever.\"",
                pursueResult = "It's the first transaction of her life with no fine print — and she signs it grinning, " +
                               "for once unafraid of what she's agreed to. *(romance.varra.choosing)*",
                holdLabel = "\"You don't owe me anything. We're square.\"",
                holdResult = "\"Square,\" she echoes, mistaking the kindness for a door closing. \"Sure. Square's good.\"",
            },
            new Beat {
                stage = Stage.LastNight, title = "Varra — burn the receipt",
                lines = new[] {
                    "Before the Court, she produces a charred scrap she's carried for twenty years — the literal receipt " +
                    "for her soul.",
                    "\"Whatever tomorrow does to us, I'm not walking into it still *priced.* So we burn it. Tonight. " +
                    "Together. And whatever I'm worth after that — it's mine to say, for once.\"" },
                pursueLabel = "Burn the receipt with her. Watch her become un-priceable.",
                pursueResult = "The contract goes up in violet fire, ash on the wind, and Varra watches her own price " +
                               "disappear with a fierce, wet, unguarded joy. \"Worth everything,\" she says. \"Turns " +
                               "out.\" *(romance.varra.consummated)*",
                holdLabel = "\"Keep it as a reminder of how far you've come.\"",
                holdResult = "\"...Maybe.\" She tucks the scrap away again. \"Maybe a reminder's better than a fire.\"",
            },
        };

        /// <summary>The full arc table, built once. Each beat carries its id/nameMatch/threshold/setFlag.</summary>
        public static readonly List<Beat> Beats = BuildAll();

        private static readonly (string id, string name)[] Romanceable =
        {
            ("garrow", "Garrow"), ("roen", "Roen"), ("naeve", "Naeve"), ("varra", "Varra"),
        };

        private static List<Beat> BuildAll()
        {
            var all = new List<Beat>();
            void Add(string id, string nameMatch, List<Beat> arc)
            {
                for (int i = 0; i < arc.Count && i < Spine.Length; i++)
                {
                    var b = arc[i];
                    b.id = id; b.nameMatch = nameMatch;
                    b.stage = Spine[i].stage;
                    b.setFlag = $"romance.{id}.{Spine[i].key}";
                    b.threshold = Spine[i].threshold;
                    all.Add(b);
                }
            }
            Add("garrow", "Garrow", Garrow());
            Add("roen", "Roen", Roen());
            Add("naeve", "Naeve", Naeve());
            Add("varra", "Varra", Varra());
            return all;
        }

        /// <summary>Has the player given their heart to someone *other* than id (past the Turn)? Romance is
        /// freely explored through Trust; committing past the Turn to two people is gently disallowed.</summary>
        public static bool CommittedElsewhere(string id)
        {
            var f = GameFlags.Current;
            foreach (var r in Romanceable)
                if (r.id != id && f.GetBool($"romance.{r.id}.turn")) return true;
            return false;
        }

        /// <summary>The best romance beat available right now: the next un-taken stage for a present,
        /// sufficiently-close companion whose progression gates are met. Highest approval wins ties.
        /// Returns null if no one's heart is ready for its next beat.</summary>
        public static Beat Best()
        {
            var party = Party.Instance;
            var f = GameFlags.Current;
            if (party == null) return null;

            Beat best = null;
            int bestApproval = int.MinValue;

            foreach (var r in Romanceable)
            {
                // present in the field?
                bool present = false;
                foreach (var m in party.active)
                    if (m?.displayName != null && m.displayName.Contains(r.name)) { present = true; break; }
                if (!present) continue;

                int approval = f.GetInt($"companion.{r.id}.approval");

                // find this companion's next un-taken beat in spine order
                foreach (var spine in Spine)
                {
                    string flag = $"romance.{r.id}.{spine.key}";
                    if (f.GetBool(flag)) continue;            // already taken — advance
                    if (!GateMet(r.id, spine, f, approval)) break; // gate not met — stop this companion here

                    // candidate is this beat
                    if (approval > bestApproval)
                    {
                        bestApproval = approval;
                        best = Beats.Find(b => b.setFlag == flag);
                    }
                    break;
                }
            }
            return best;
        }

        private static bool GateMet(string id, (Stage stage, string key, int threshold) spine, GameFlags f, int approval)
        {
            if (approval < spine.threshold) return false;

            switch (spine.stage)
            {
                case Stage.Spark:
                    // they've opened up at the fire first
                    return f.GetBool($"camp.nighttalk.{id}.done");
                case Stage.Trust:
                    return f.GetBool($"romance.{id}.spark");
                case Stage.Turn:
                    // committing past the Turn to two people at once is disallowed (lightly)
                    return f.GetBool($"romance.{id}.trust") && !CommittedElsewhere(id);
                case Stage.Crisis:
                    // the Crisis IS their personal quest — it must be resolved first
                    return f.GetBool($"romance.{id}.turn") && f.GetBool($"quest.{id}.resolved");
                case Stage.Choosing:
                    return f.GetBool($"romance.{id}.crisis");
                case Stage.LastNight:
                    return f.GetBool($"romance.{id}.choosing");
            }
            return false;
        }
    }
}
