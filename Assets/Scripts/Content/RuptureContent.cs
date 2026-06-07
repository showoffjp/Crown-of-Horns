using System.Collections.Generic;
using SunderedCrown.Characters;
using SunderedCrown.Core;

namespace SunderedCrown.Content
{
    /// <summary>
    /// The dark mirror of the romances. When a companion's approval craters — you've crossed the line
    /// their whole self is built on, again and again — the bond *frays*, and at the next camp they call
    /// you on it. You can make amends (but only if you've actually invested in them — a night-talk shared
    /// or a spark kindled; you can't talk back someone you never bothered to know), patch it over and keep
    /// them aboard but guarded, or let them walk out of the story for good. Pure data, mirroring
    /// CampContent/RomanceContent; CampScene surfaces a pending rupture, and EndingResolver remembers who
    /// you drove away.
    /// </summary>
    public static class RuptureContent
    {
        public class Rupture
        {
            public string id;
            public string nameMatch;
            public int approvalAtMost;   // frays once approval falls to/below this
            public string title;
            public string[] lines;       // the grievance, in their voice

            public string amendsLabel;   // try to make it right
            public string amendsWin;     // narration if you've earned the standing to be heard
            public string amendsThin;    // narration if you haven't — words aren't enough without history
            public string patchLabel;    // own it, keep them aboard but cool
            public string patchResult;
            public string partLabel;     // let them go
            public string partResult;
        }

        public static readonly List<Rupture> Ruptures = new List<Rupture>
        {
            new Rupture {
                id = "garrow", nameMatch = "Garrow", approvalAtMost = -25,
                title = "Garrow — \"I gave my life to this\"",
                lines = new[] {
                    "Garrow blocks your path to the fire, and does not move.",
                    "\"I have spent forty years giving the dead their dignity. I have watched you spend it like coin — " +
                    "desecrate, mock, leave them wrong in the ground.\"",
                    "\"I told myself you'd been *there,* that you understood. I was wrong, and I am too old to keep being " +
                    "wrong about people. So. Convince me you're not what you keep showing me you are.\"" },
                amendsLabel = "\"You're right. I lost the thread. Let me earn it back.\"",
                amendsWin = "She studies you a long moment — and remembers the nights you *did* sit with her in the dark. " +
                            "\"...All right. One more chance. Don't make me bury this too.\" The grey settles back at your side.",
                amendsThin = "\"Pretty words,\" she says, \"from someone who never once sat with me long enough to mean " +
                             "them.\" She isn't moved. The choice is plainer than you'd like: keep her cold, or let her go.",
                patchLabel = "\"I won't pretend I'll be perfect. But I'll do better.\"",
                patchResult = "\"Better,\" she repeats, without warmth. \"We'll see.\" She stays — but the easy trust is " +
                              "gone, and you both feel the chill where it used to be.",
                partLabel = "\"Then maybe you shouldn't be here.\"",
                partResult = "She nods once, like a verdict she expected. \"No. Maybe not.\" She gathers her grey and walks " +
                             "into the dark to find dead who still deserve her. She does not look back.",
            },
            new Rupture {
                id = "roen", nameMatch = "Roen", approvalAtMost = -25,
                title = "Roen — \"I let you past the locks\"",
                lines = new[] {
                    "Roen's grin is gone — and you didn't know how much you relied on it until now.",
                    "\"Here's the thing nobody gets about me: I let exactly *one* person past the locks. That was a " +
                    "mistake I decided to make on purpose. You.\"",
                    "\"And you've spent that trust like it cost you nothing. So I'm standing here doing the math on whether " +
                    "I'm an idiot. Talk fast — and for once, don't perform.\"" },
                amendsLabel = "Don't perform. Just tell him the truth.",
                amendsWin = "He hears the thing he can't con — sincerity — and hates how much it lands. \"...Damn you,\" he " +
                            "mutters, the grin flickering back. \"Fine. Locks stay open. Don't make me regret the math.\"",
                amendsThin = "\"See, that'd land,\" he says quietly, \"if you'd ever once let me see the real you. You " +
                             "didn't.\" The performance is gone, and so is the warmth. Keep him cold, or cut him loose.",
                patchLabel = "\"I can't undo it. I can stop doing it.\"",
                patchResult = "\"Stopping's a start,\" he allows, flat. He stays — but the locks are back, and the next time " +
                              "he flirts there's an exit built into every word again.",
                partLabel = "\"Maybe the math says you should walk.\"",
                partResult = "\"Yeah,\" he says, already turning, every door shutting at once. \"Yeah, it does. Survival " +
                             "strategy, restored. Should never have changed it.\" And the Outer City takes its son back.",
            },
            new Rupture {
                id = "varra", nameMatch = "Varra", approvalAtMost = -25,
                title = "Varra — \"Knew it\"",
                lines = new[] {
                    "Varra's smile is the cruel one, the armored one, and it's aimed at herself as much as you.",
                    "\"Knew it. Always do. Everyone who gets close to me is just reading the fine print, working out what " +
                    "I'm worth and what I'll cost. You took longer than most. Almost had me.\"",
                    "\"So here's your out, free of charge. Prove me right and leave, like the receipt always said you " +
                    "would. Go on.\"" },
                amendsLabel = "Don't leave. Don't argue. Just refuse to go.",
                amendsWin = "You stay through the acid, the way someone once should have. The smile cracks. \"...You're " +
                            "supposed to be gone,\" she whispers, off-script and undone. \"Okay. Okay. Don't be lying.\"",
                amendsThin = "\"Staying *now*?\" she laughs, brittle. \"You've spent this whole road proving the receipt " +
                             "right. One quiet night won't un-sign it.\" She won't be talked round. Cold, or gone.",
                patchLabel = "\"I'm not leaving. But I won't pretend I haven't hurt you.\"",
                patchResult = "\"Honesty. Novel.\" She stays — but the armor's bolted back on, and every kindness from you " +
                              "now gets weighed for the catch she's certain is coming.",
                partLabel = "Say nothing. Let her be right.",
                partResult = "\"There it is.\" Something behind her eyes goes out, and she collects herself like a debt " +
                             "called in. \"Worth exactly what they said, after all.\" She's gone before the fire dies.",
            },
            new Rupture {
                id = "naeve", nameMatch = "Naeve", approvalAtMost = -25,
                title = "Naeve — \"I have seen where the reaching ends\"",
                lines = new[] {
                    "Naeve sets down her work with a precision that means she is furious.",
                    "\"I have watched you reach — recklessly, repeatedly — for power whose cost I understand better than " +
                    "any soul alive. I held a piece of the proof that fell a sky. I know the shape of this hunger.\"",
                    "\"I will not stand at the elbow of another Karsus and hand him the math. Convince me you are not " +
                    "becoming one, or I withdraw my mind from your service entirely.\"" },
                amendsLabel = "Show her you've weighed the cost — and chosen restraint.",
                amendsWin = "She tests your reasoning like a proof, and finds, reluctantly, that it holds. \"...Acceptable,\" " +
                            "she says, which from Naeve is nearly tenderness. \"Do not make me re-derive it. Stay your hand.\"",
                amendsThin = "\"You offer me a conclusion with no working shown,\" she says coldly. \"I have seen what " +
                             "unproven confidence costs. I am not reassured.\" Keep her, distant — or let her go.",
                patchLabel = "\"I hear you. I'll be more careful with what I reach for.\"",
                patchResult = "\"Careful is a start. It is not a proof.\" She remains — but she no longer volunteers her " +
                              "knowledge freely, and watches your choices the way one watches a variable trending wrong.",
                partLabel = "\"I'll reach for what I have to. With or without you.\"",
                partResult = "\"Then without,\" she says, and there is grief under the ice. \"I ended one world by helping a " +
                             "man who would not be told no. I will not help a second.\" She gathers her proofs and goes.",
            },
            new Rupture {
                id = "ilfaeril", nameMatch = "Ilfaeril", approvalAtMost = -25,
                title = "Ilfaeril — \"I will not author more cruelty\"",
                lines = new[] {
                    "Ilfaeril regards you with the terrible patience of someone who has seen this exact thing before.",
                    "\"Ten thousand years ago I raised my hand for a cruelty I called justice, and I have spent every year " +
                    "since learning what that costs. I know vengeance when I see it wearing mercy's face.\"",
                    "\"I see it on you, lately. And I swore on the unmade dead that I would never again *stand beside* it " +
                    "in silence. So speak — and mean it — or I keep my oath and leave.\"" },
                amendsLabel = "Lay down the vengeance. Mean it where he can see.",
                amendsWin = "He searches you for the lie his ten thousand years has taught him to expect, and does not find " +
                            "it. \"...Then I was wrong to doubt,\" he says, humbled. \"Forgive an old man his vigilance.\"",
                amendsThin = "\"I have heard that exact promise in a high court,\" he says, \"from lords who meant it and " +
                             "voted yes regardless. Words are not enough; they never were.\" Keep him, grieving — or release him.",
                patchLabel = "\"I'm not proud of who I've been. I'll try to be better.\"",
                patchResult = "\"Trying is not nothing,\" he concedes gravely. He stays — but he keeps a watchful distance " +
                              "now, an old sentinel who has learned to mistrust even the people he loves.",
                partLabel = "\"Then keep your oath, old man. Go.\"",
                partResult = "\"I will.\" There is no anger in him, only a vast and practiced sorrow. \"I have left worse " +
                             "rooms for less. Be better than I was. It is the only forgiveness that matters.\" He walks into the dusk.",
            },
        };

        private static bool Resolved(GameFlags f, string id) =>
            f.GetBool($"rupture.{id}.mended") || f.GetBool($"rupture.{id}.uneasy") || f.GetBool($"rupture.{id}.broken");

        /// <summary>Has the player earned the standing to be heard — a shared night-talk or a kindled
        /// spark? You cannot talk back someone you never bothered to know.</summary>
        public static bool HasStanding(GameFlags f, string id) =>
            f.GetBool($"camp.nighttalk.{id}.done") || f.GetBool($"romance.{id}.spark");

        /// <summary>The fraying bond that needs facing right now, if any: a present companion whose
        /// approval has cratered and whose rupture hasn't been resolved yet.</summary>
        public static Rupture Pending()
        {
            var party = Party.Instance;
            var f = GameFlags.Current;
            if (party == null) return null;

            foreach (var r in Ruptures)
            {
                if (Resolved(f, r.id)) continue;
                if (f.GetBool($"companion.{r.id}.left") || f.GetBool($"companion.{r.id}.lost")) continue;

                bool present = false;
                foreach (var m in party.active)
                    if (m?.displayName != null && m.displayName.Contains(r.nameMatch)) { present = true; break; }
                if (!present) continue;

                if (f.GetInt($"companion.{r.id}.approval") <= r.approvalAtMost) return r;
            }
            return null;
        }
    }
}
