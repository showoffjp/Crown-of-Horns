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

            // A third thread — the Breach. The gravedigger knows a trade-death when he sees one carried.
            if (f.GetBool("fugue.pull_maerin"))
                close += "\n\nAnd you have the other look, too — the one a soul wears after the Wall has taken " +
                         "someone *for* them. I have buried trade-deaths. The Wall never gives without taking; you " +
                         "reached in and it balanced the books on someone you loved. I'll not call that wrong. I'll " +
                         "only say — dig deep for that one when you get home. Some graves you owe more than dirt.";
            else if (f.GetBool("fugue.left_maerin"))
                close += "\n\nAnd you have the rarer look — a soul who stood at the Wall, was offered a life, and " +
                         "*did the arithmetic out loud.* Most can't. They see one face and stop counting the cost. " +
                         "You counted, and you walked. I've no grave to dig for restraint, friend — but I would dig " +
                         "one for yours if the ground allowed it. It deserves a marker.";

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

            // The Breach callback — one soul out, one soul in; the Wall's only honest law, felt even here.
            if (f.GetBool("fugue.pull_maerin"))
                close += "\n\nYou've done this before, haven't you — reached into the grey and *taken one back.* I felt " +
                         "the ripple of it, even in here. One soul out, one soul in; the Wall's only honest law. Whoever " +
                         "you traded for is somewhere in this fire too, scratched out so another could stay written. " +
                         "Carry them both. The pulled and the paid. That is the whole weight of being able to reach.";
            else if (f.GetBool("fugue.left_maerin"))
                close += "\n\nYou've stood where I'm standing, too — close enough to the Wall to pull one free, and you " +
                         "*didn't,* because you saw the second name the price would write. I'd have begged you to save me. " +
                         "I'd also have been the cost. The undone don't get to thank a soul for restraint... but I will " +
                         "anyway. Someone should, before the fire takes the word.";

            g.nodes.Add(new DialogueNode { id = "close", speaker = "A Half-Unmade Voice", text = close });
            return g;
        }

        // ---- The Time of Troubles, second echo: the forge where Myrkul's skull becomes the Crown of Horns.
        //      Calls back to ALDRIC MORN — the gentle bearer you met over tea — and what you saw in him. The
        //      Crown the keeper is hammering here is, downriver, the relic on Aldric's shelf. ----
        public static DialogueGraph CrownBearer()
        {
            var f = GameFlags.Current;
            var g = ScriptableObject.CreateInstance<DialogueGraph>();
            g.conversationId = "toot.crown_echo"; g.startNodeId = "0";

            g.nodes.Add(new DialogueNode
            {
                id = "0", speaker = "A Keeper of the Bone Crown",
                text = "Careful where you step, pilgrim — that is not a forge-fire's heat you feel. It is *attention.* " +
                       "They are beating a god's skull thin and curved, and the god is still *in* it — still awake, " +
                       "still choosing. Most who come here feel only the cold. You feel the *looking.* You've felt it " +
                       "before, haven't you. Somewhere far downriver of this hammer.",
                onEnter = new[] { new FlagClause { key = "toot.crown_echo_seen", op = FlagOp.SetTrue } },
                choices = new[]
                {
                    Go("close"),
                    new DialogueChoice { text = "I've felt it. On a gentle man's shelf.", nextNodeId = "close" }
                }
            });

            // Branch on what you learned of Aldric over tea (most-specific reading first).
            string close =
                f.GetBool("aldric.crown_doubt_planted")
                    ? "Then you already know the secret this whole forge is built to keep: it is not a tool. You sat " +
                      "across a tea-table from a kind man who called it a *means* — and something behind his eyes " +
                      "disagreed, and you *caught* it. Trust that more than his kindness. The Crown does not want a " +
                      "villain; villains are loud. It wants exactly what he is — a grieving, gentle, *certain* hand " +
                      "that will carry it to the Wall believing the whole way that it holds the leash. You saw the " +
                      "leash for what it was. Whatever else you do, downriver: do not unsee it."
              : f.GetBool("aldric.named_monster")
                    ? "Then you have already passed a verdict on the hand that will carry this — named the man a " +
                      "*monster* across a cooling cup of tea. Perhaps you were right about his deeds. But the thing " +
                      "they are hammering here is not *in* him; it is in the crown, and the crown is patient enough " +
                      "to let you spend all your certainty on the grieving fool while it picks the lock behind his " +
                      "eyes. Aim better, traveller. Hate the relic — not the hand it chose precisely for being kind."
              : f.GetBool("aldric.grief_seen")
                    ? "Then you have already met the hand it is shaping itself to fit — a father who followed his " +
                      "girl into the grey and watched a god wall her in. You saw the grief beneath the heresy; good. " +
                      "But understand: what I am forging here saw it too. Love the size of his is not a weakness the " +
                      "Crown exploits — it is the *door* the Crown was built to walk through. Watch that gentle man, " +
                      "downriver. Watch what he carries more."
              : f.GetBool("aldric.cost_revealed")
                    ? "Then you have already done the one thing his crusade fears — made him say the number aloud. He " +
                      "counts his dead; grant him that, it is more than most butchers manage. But the thing in this " +
                      "crown keeps a *different* ledger, and his arithmetic is not its arithmetic. He believes he is " +
                      "paying thousands for a mercy. He has not yet been shown the line where the price is *him.* You " +
                      "have stood at this forge and seen the skull still choosing. Carry that downriver."
              : f.GetBool("aldric.met")
                    ? "Then somewhere downriver you have already shared a roof with the bearer this crown is being " +
                      "shaped to fit — a soft-spoken man who calls a *god* a tool, and means it. Remember the weight " +
                      "of his voice when next you feel the looking. The hammer here and the whisper there are the " +
                      "same throat. You are the one soul who has stood at both ends of it."
              : "Then carry only this back down the road you came: somewhere ahead, a gentle hand will be offered " +
                "this very crown and told it is a *means* — a tool, a key, the smallest evil for the largest mercy. " +
                "Whoever tells them that will believe it, and the crown will let them. When you meet that hand, " +
                "remember you once stood at the forge and heard the skull still *choosing.*";

            g.nodes.Add(new DialogueNode { id = "close", speaker = "A Keeper of the Bone Crown", text = close });
            return g;
        }
    }
}
