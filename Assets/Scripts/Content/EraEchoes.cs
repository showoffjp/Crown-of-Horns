using UnityEngine;
using SunderedCrown.Core;
using SunderedCrown.Dialogue;

namespace SunderedCrown.Content
{
    /// <summary>
    /// CROSS-ERA CALLBACKS — the time-travel earning its keep (ROADMAP Tier 3 / DESIGN pillar V:
    /// "no choice forgotten"). Where <see cref="EraWitness"/> reacts to a *companion's* arc, these are the
    /// *world itself* naming a choice you made an age earlier and seeing its consequence ripple downstream.
    ///
    /// Each is a static builder (like EraWitness) read live from <see cref="GameFlags"/> when the late era
    /// loads, placed by <see cref="SimpleEra"/> as a Talk marker. They name, by name, the signature upstream
    /// decisions — the Crown Wars *Verdict* (<c>crownwars.verdict_spared</c> / <c>crownwars.verdict_passed</c>)
    /// and the fall of Netheril (<c>netheril.cleared</c>/<c>netheril.arrived</c>) — so a choice in -10,000 DR
    /// is spoken aloud in 1358/1385 DR. Each sets an <c>&lt;era&gt;.echo_seen</c> flag (no approval; this is
    /// the world, not a friend). If you witnessed neither upstream era, the line falls back to a quiet,
    /// non-accusing default so the beat always reads.
    /// </summary>
    public static class EraEchoes
    {
        private static DialogueChoice Go(string next) => new DialogueChoice { text = "(let her speak)", nextNodeId = next };

        // ---- The Time of Troubles (1358 DR): a grey gravedigger working the godless dead while the gods bleed.
        //      Calls back to the Crown Wars Verdict — the afternoon the Wall was first voted into being. ----
        public static DialogueGraph TimeOfTroubles()
        {
            var f = GameFlags.Current;
            var g = ScriptableObject.CreateInstance<DialogueGraph>();
            g.conversationId = "toot.echo"; g.startNodeId = "0";

            g.nodes.Add(new DialogueNode
            {
                id = "0", speaker = "A Grey Gravedigger",
                text = "Mind the rows, traveller — fresh ones, all of them. When the gods walk as men and die as men, " +
                       "the dead come faster than the prayers. I bury the claimed on the hill. The *unclaimed* — the " +
                       "godless, the faithless — I bury here, in the low ground, where the law says their souls go to " +
                       "the Wall and nowhere. You have the look of someone who has argued with that law before.",
                onEnter = new[] { new FlagClause { key = "toot.echo_seen", op = FlagOp.SetTrue } },
                choices = new[] { Go("close"), new DialogueChoice { text = "Once. Ten thousand years ago.", nextNodeId = "close" } }
            });

            // Branch on the upstream Verdict — the cruelest/kindest callback the saga can make.
            string close =
                f.GetBool("crownwars.verdict_spared")
                    ? "Then you are the reason I have a low ground to dig at all. There is an old elven word the " +
                      "grey-robes still teach — that once, in a silver court before the world was counted, a motion " +
                      "to *unmake* a beaten people's dead was withdrawn. Withdrawn. They say a stranger argued it down. " +
                      "Because of that single afternoon, the Wall was only ever a wall — never a furnace. These poor " +
                      "godless souls get a resting-place tonight. That is your doing, whether you remember it or not."
              : f.GetBool("crownwars.verdict_passed")
                    ? "Then you watched the law I work under get its first vote, and you let it pass. I do not blame " +
                      "you — a court of immortals is a hard room to shout in. But understand what I inherit: the motion " +
                      "to unmake the faithless dead *carried,* that silver afternoon, and ten thousand years later it is " +
                      "still carrying. I bury these unclaimed knowing the Wall will take them anyway. Root and memory. " +
                      "You stood at the source of that, Returned. The source remembers, even when the river forgets."
              : "Then you have walked further than I can dream of, and I will not pry. Only this: somewhere upstream of " +
                "all these graves there was a *first* one — a first court that decided the unclaimed deserve nothing. " +
                "If your road ever takes you that far back... a gravedigger in a dying age would take it kindly if you " +
                "argued the matter, while it was still only a matter and not yet a wall.";

            // A second, quieter thread for those who saw the first apocalypse fall.
            if (f.GetBool("netheril.cleared") || f.GetBool("netheril.arrived"))
                close += "\n\n...You flinch at the wrong sky. I have seen that flinch on one other — an old man who " +
                         "swore he watched a city of magic fall out of the blue and break. You have the same eyes. " +
                         "Whatever you saw burn back then, friend, you are still carrying the soot of it.";

            g.nodes.Add(new DialogueNode { id = "close", speaker = "A Grey Gravedigger", text = close });
            return g;
        }

        // ---- The Spellplague (1385 DR): a soul half-erased in the blue fire, where cause forgets to precede
        //      effect and the Unmade comes closest to winning. Calls back to Netheril AND the Verdict. ----
        public static DialogueGraph Spellplague()
        {
            var f = GameFlags.Current;
            var g = ScriptableObject.CreateInstance<DialogueGraph>();
            g.conversationId = "spellplague.echo"; g.startNodeId = "0";

            g.nodes.Add(new DialogueNode
            {
                id = "0", speaker = "A Half-Unmade Voice",
                text = "You can *see* me? Most can't, in the blue. I'm coming apart — not dying, undoing, like a word " +
                       "scratched out mid-sentence. The fire doesn't burn here so much as it *forgets* you. And yet you " +
                       "look at me and I stay. Stay a moment longer. Being looked at is the only thing keeping me a *me.*",
                onEnter = new[] { new FlagClause { key = "spellplague.echo_seen", op = FlagOp.SetTrue } },
                choices = new[] { Go("close"), new DialogueChoice { text = "I see you. I'm not looking away.", nextNodeId = "close" } }
            });

            string close =
                f.GetBool("crownwars.verdict_spared")
                    ? "I know you. Not your face — your *act.* I am one of the unclaimed, and there is a reason I am " +
                      "half-erased in this fire and not wholly gone: an old verdict that should have unmade my kind " +
                      "root and branch was *withdrawn,* once, in a court ten thousand years deep. That mercy is the only " +
                      "thread the blue fire hasn't found yet. You spun that thread, Returned. Hold the look. Let me finish " +
                      "the sentence I was scratched out of: *thank you.*"
              : f.GetBool("crownwars.verdict_passed")
                    ? "I know what unmade me. Not this fire — the fire only finishes it. The verdict that took my kind " +
                      "passed in a silver court an age ago, root and memory, and you were *there,* weren't you. I can " +
                      "smell that afternoon on you. I am not angry. The undone do not get to keep anger. I only want one " +
                      "soul to have watched it both times — the vote and the cost — and not look away. You are that soul. " +
                      "That is something. In the blue, witness is nearly everything."
              : "I don't know what I was. The fire took the name first. But you — you are *anchored,* somehow, against a " +
                "current that unmakes gods. Whatever holds you together, hold it harder. The Unmade is closest to winning " +
                "right here, in this wound, and the only thing it has never learned to erase is a thing that refuses to " +
                "stop bearing witness. Be that thing. For both of us.";

            // Netheril callback — the Spellplague is literally when the surviving Netherese returned.
            if (f.GetBool("netheril.cleared") || f.GetBool("netheril.arrived"))
                close += "\n\n...There are shades in this fire who say they fell once before — a city, a blue sky, a " +
                         "silence where every spell went out at once. They speak of it like it was yesterday. To them, " +
                         "with cause unstitched in here, perhaps it *is.* You were there for that too. Of course you were. " +
                         "You are the one soul this whole burning age keeps handing its grief to.";

            g.nodes.Add(new DialogueNode { id = "close", speaker = "A Half-Unmade Voice", text = close });
            return g;
        }
    }
}
