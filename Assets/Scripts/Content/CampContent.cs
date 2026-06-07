using System.Collections.Generic;
using SunderedCrown.Characters;
using SunderedCrown.Core;

namespace SunderedCrown.Content
{
    /// <summary>
    /// Campfire night-talks. Each fielded companion has a quiet monologue they only offer once their
    /// approval has crossed a threshold — the classic BG2 "the fire's low, and they finally talk"
    /// beat. Pure data; CampScene picks the highest-approval eligible companion who hasn't spoken yet.
    /// </summary>
    public static class CampContent
    {
        public class NightTalk
        {
            public string id;          // companion id (lowercase first name), matches approval flag
            public string nameMatch;   // substring to find them in the active party
            public int threshold;      // approval required to unlock
            public string title;
            public string[] lines;
            public string key;         // unique done-flag suffix (defaults to id, for the first talk)
            public string requiresFlag;// optional: must be true to offer (e.g. the first talk's done-flag)

            public string DoneKey => string.IsNullOrEmpty(key) ? id : key;
        }

        public static readonly List<NightTalk> Talks = new List<NightTalk>
        {
            new NightTalk {
                id = "roen", nameMatch = "Roen", threshold = 25, title = "Roen — \"The Honest Lie\"",
                lines = new[] {
                    "Roen pokes the fire, not looking at you.",
                    "\"Funny thing about being a Harper. They teach you a hundred lies and one truth, and they " +
                    "never tell you which is which until it's too late to give either back.\"",
                    "\"I told you I was sent to watch you. That was true. I didn't tell you I asked for the job. " +
                    "Because the soul that walked back out of death? I wanted to see if coming back was possible. " +
                    "I've got someone I'd walk into the Wall to pull out, if it were.\"",
                    "He finally meets your eyes. \"So. Is it? Possible?\"",
                    "You don't have an answer yet. But he nods like the honesty was enough." }
            },
            new NightTalk {
                id = "varra", nameMatch = "Varra", threshold = 25, title = "Varra — \"The Bill Comes Due\"",
                lines = new[] {
                    "Varra is counting something on her fingers. She stops when she notices you watching.",
                    "\"Days,\" she says. \"I count the days. Since I was six there's been a number, and when it hits " +
                    "zero my patron collects. I used to think the number was a curse. Now I think it's the only " +
                    "honest clock I've ever owned.\"",
                    "\"Everyone's running a tab, Returned. Gods, kings, the man who sells bad fish at the docks. " +
                    "Mine just sends the invoice in person.\" She laughs, and it's almost real.",
                    "\"Don't look at me like that. I'm not asking you to pay it. I'm asking you to be loud enough, " +
                    "when it comes, that the whole Hells hear me leave swinging.\"" }
            },
            new NightTalk {
                id = "garrow", nameMatch = "Garrow", threshold = 20, title = "Garrow — \"A God-Shaped Hole\"",
                lines = new[] {
                    "Garrow sharpens his blade in long, even strokes — a prayer with no god at the end of it.",
                    "\"I served Kelemvor forty years. Buried the faithful, judged the wicked, slept sound.\"",
                    "\"Then I saw the Wall. Saw what my god does to the ones who simply… didn't kneel. And the " +
                    "sleeping stopped.\" The whetstone pauses. \"You don't kneel, and you came back anyway. " +
                    "Maybe that's the answer. Maybe the dead don't need a god. Maybe they just need someone to " +
                    "refuse to let them be forgotten.\"",
                    "\"That's a heresy worth a sword arm. You've got mine.\"" }
            },
            new NightTalk {
                id = "naeve", nameMatch = "Naeve", threshold = 25, title = "Naeve — \"After the Sky Fell\"",
                lines = new[] {
                    "Naeve watches the smoke rise and not fall, as if surprised it still obeys.",
                    "\"In Netheril we thought we'd solved death. We thought we'd solved everything — that's how you " +
                    "know a civilization is about to end. Then Karsus reached for godhood and the sky remembered " +
                    "it could let go.\"",
                    "\"I have outlived my entire world. Every word in my language is a tombstone. And here you are, " +
                    "carrying a crack in time exactly the shape of mine.\" She almost smiles.",
                    "\"Do not reach for godhood, Returned. I have seen where the reaching ends. Reach for the " +
                    "person beside you instead. It is a smaller magic. It is the only one that ever held.\"" }
            },
            new NightTalk {
                id = "ilfaeril", nameMatch = "Ilfaeril", threshold = 30, title = "Ilfaeril — \"The Vote\"",
                lines = new[] {
                    "Ilfaeril sits apart from the fire, as the very old often do.",
                    "\"Ten thousand years ago I raised my hand in a high court and voted that a soul deserved to be " +
                    "unmade. We called it justice. We called it mercy, even — better nothing than torment.\"",
                    "\"I have spent every year since learning the word we should have used. *Cowardice.* It is " +
                    "easier to erase a question than to answer it.\"",
                    "\"You are walking backward through all our worst votes, Returned. When you reach the one that " +
                    "matters — and you will — raise your hand the way I should have. Raise it for them.\"" }
            },

            new NightTalk {
                id = "maerin", nameMatch = "Maerin", threshold = 20, title = "Maerin — \"What counting feels like\"",
                lines = new[] {
                    "Maerin sits very close to the fire, the way someone does who spent a long time somewhere cold.",
                    "\"In the Wall you stop being a person and become a... a subtraction. They don't hurt you. That's the " +
                    "trick of it. They just stop including you. Slowly. Until the sentence you're in has no room for you.\"",
                    "\"Out here it's the opposite and it's almost worse, because I'm not used to it. You set a bowl for me. " +
                    "You wait when I'm slow. Someone says my name and *means* the place it points to.\"",
                    "She looks at you, fierce and frightened at once. \"Don't make me your cause, Returned. Make me your " +
                    "friend. A cause you can put down when it's won. A friend you have to keep choosing. I need the kind " +
                    "you have to keep choosing.\"" }
            },

            // ---- Second, deeper night-talks (higher approval, gated behind the first) ----
            new NightTalk {
                id = "garrow", nameMatch = "Garrow", threshold = 55, key = "garrow2",
                requiresFlag = "camp.nighttalk.garrow.done", title = "Garrow — the list she keeps",
                lines = new[] {
                    "Garrow has a small book out, writing by firelight in a careful, even hand.",
                    "\"Every name I ever buried. I add to it nightly. It's the closest thing to a prayer I have left — " +
                    "and the only one I've never once doubted.\"",
                    "She doesn't look up. \"Lately I've caught myself wanting a second list. People I refuse to put on " +
                    "the first. I've never had one of those. It's a terrifying kind of book to start.\"" }
            },
            new NightTalk {
                id = "roen", nameMatch = "Roen", threshold = 55, key = "roen2",
                requiresFlag = "camp.nighttalk.roen.done", title = "Roen — the name underneath",
                lines = new[] {
                    "Roen's quiet for once, turning a worn Harper pin over in his fingers.",
                    "\"Roen Alleywind's a name I picked. The one before it — the gutter one — I buried so deep I half " +
                    "believe the lie that I never had one.\"",
                    "\"You're out here hunting your *true* name across all of time. Must be nice, caring enough to look. " +
                    "...That came out more honest than I meant. Forget I said it. You won't, will you. Menace.\"" }
            },
            new NightTalk {
                id = "varra", nameMatch = "Varra", threshold = 55, key = "varra2",
                requiresFlag = "camp.nighttalk.varra.done", title = "Varra — the first warm thing",
                lines = new[] {
                    "Varra is staring into the fire like she's daring it to bill her.",
                    "\"Twenty years I've slept maybe two hours a night — can't, with a clock in your chest counting down " +
                    "to collection. But the last few nights, by this fire, with you lot snoring...\"",
                    "\"...I slept. Whole nights. Don't make it weird. I'm just noting, for the record, that it turns out " +
                    "I *can.* Apparently I just needed somewhere it was safe to.\"" }
            },
            new NightTalk {
                id = "naeve", nameMatch = "Naeve", threshold = 55, key = "naeve2",
                requiresFlag = "camp.nighttalk.naeve.done", title = "Naeve — the unsolved variable",
                lines = new[] {
                    "Naeve watches the sparks rise, frowning as if they've violated a theorem.",
                    "\"I have modelled grief as a decay function for a thousand years. Clean. Predictable. It approaches " +
                    "zero and never arrives.\"",
                    "\"Then I met a soul that predates itself, and the model broke. You are the first variable I cannot " +
                    "solve and do not *want* to. I find I have been afraid to tell you that. So. Now you know. Filed.\"" }
            },
            new NightTalk {
                id = "ilfaeril", nameMatch = "Ilfaeril", threshold = 55, key = "ilfaeril2",
                requiresFlag = "camp.nighttalk.ilfaeril.done", title = "Ilfaeril — the longest night",
                lines = new[] {
                    "Ilfaeril sits the way the very old sit — as if the dark is an acquaintance.",
                    "\"Ten thousand years I have refused myself rest, because rest felt like a thing I had not earned " +
                    "and the dead had not been granted.\"",
                    "\"Sitting here, among people who know exactly what I did and let me pass the bread anyway — I find " +
                    "the refusal harder to keep. You are doing something to my arithmetic, Returned. I have not decided " +
                    "yet whether to forgive you for it.\"" }
            },

            new NightTalk {
                id = "maerin", nameMatch = "Maerin", threshold = 50, key = "maerin2",
                requiresFlag = "camp.nighttalk.maerin.done", title = "Maerin — the first morning",
                lines = new[] {
                    "Maerin is awake before the others, watching the grey light come up over the camp.",
                    "\"I've started doing a thing. Every morning I make myself name three real ones. A bird. The cold. " +
                    "Your stupid snoring. Proof I'm still in a world that has *things in it.*\"",
                    "\"In the Wall there was nothing to name. That's how it takes you — not pain, just... no nouns left. " +
                    "So I hoard them now. Greedy for ordinary mornings.\"",
                    "She glances at you, almost shy. \"You're on the list most days. Just so you know. Right between the " +
                    "bird and the cold — the three things I'd refuse to forget, if it ever came for me again.\"" }
            },

            // ---- New Game+ only: the woman who becomes the Lady feels the loop turn under her. ----
            new NightTalk {
                id = "naeve", nameMatch = "Naeve", threshold = 25, key = "naeve_ngplus",
                requiresFlag = "ng.plus", title = "Naeve — \"I have run this proof before\"",
                lines = new[] {
                    "Naeve is very still, staring at her own hands as though they belong to someone she used to be.",
                    "\"There is a sensation I have no clean word for. Not memory — I would trust memory. This is the " +
                    "*shape* of a memory with the content scraped out. A theorem I am certain I have proved, and cannot " +
                    "find the proof of.\"",
                    "\"I keep looking at you and arriving, with full rigor and no evidence, at a single conclusion: that " +
                    "we have sat at this exact fire before. That I said something kind. That it... ended. And began.\"",
                    "She manages a thin, frightened smile. \"Tell me I am being unscientific, Returned. Please. I find " +
                    "I would very much like to be wrong, and I do not think that I am.\"" }
            },
        };

        /// <summary>The best night-talk available right now: highest approval among fielded, eligible,
        /// not-yet-heard companions. Returns null if no one is ready to open up.</summary>
        public static NightTalk Best()
        {
            var party = Party.Instance;
            var f = GameFlags.Current;
            if (party == null) return null;

            NightTalk best = null;
            int bestApproval = int.MinValue;
            foreach (var t in Talks)
            {
                if (f.GetBool($"camp.nighttalk.{t.DoneKey}.done")) continue;
                if (!string.IsNullOrEmpty(t.requiresFlag) && !f.GetBool(t.requiresFlag)) continue;
                bool present = false;
                foreach (var m in party.active)
                    if (m.displayName != null && m.displayName.Contains(t.nameMatch)) { present = true; break; }
                if (!present) continue;

                int approval = f.GetInt($"companion.{t.id}.approval");
                if (approval < t.threshold) continue;
                if (approval > bestApproval) { bestApproval = approval; best = t; }
            }
            return best;
        }
    }
}
