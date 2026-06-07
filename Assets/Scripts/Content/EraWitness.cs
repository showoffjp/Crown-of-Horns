using UnityEngine;
using SunderedCrown.Core;
using SunderedCrown.Dialogue;

namespace SunderedCrown.Content
{
    /// <summary>
    /// Companion-reactive "witness" beats for the SimpleEra-based eras (Time of Troubles, Spellplague) —
    /// the same idea as the bespoke Netheril/Crown Wars beats, but for the config-driven eras. Each is
    /// built live from the flags when the era loads, reacts to how that companion's personal quest
    /// resolved, sets an `<era>.<id>_witnessed` flag, and nudges approval. The era only places it when the
    /// companion is actually fielded (SimpleEra checks party presence).
    /// </summary>
    public static class EraWitness
    {
        private static DialogueChoice Go(string next) => new DialogueChoice { text = "(go on)", nextNodeId = next };

        // ---- Garrow, in the Time of Troubles: death's own god, made mortal and beaten into a crown ----
        public static DialogueGraph GarrowTimeOfTroubles()
        {
            var f = GameFlags.Current;
            var g = ScriptableObject.CreateInstance<DialogueGraph>();
            g.conversationId = "toot.garrow_witness"; g.startNodeId = "0";

            g.nodes.Add(new DialogueNode
            {
                id = "0", speaker = "Sister Garrow",
                text = "Forty years a Doomguide and I never once let myself imagine *this.* They are beating a god of " +
                       "death into a crown of horns. Death itself — mortal, screaming, spent like coin. Every certainty " +
                       "I ever buried someone with is being forged into a tyrant's hat, right in front of me.",
                onEnter = new[] { new FlagClause { key = "toot.garrow_witnessed", op = FlagOp.SetTrue },
                                  new FlagClause { key = "companion.garrow.approval", op = FlagOp.AddInt, amount = 5 } },
                choices = new[] { new DialogueChoice { text = "What does it change, for you?", nextNodeId = "close" },
                                  new DialogueChoice { text = "Steady. I'm here.", nextNodeId = "close" } }
            });

            string close =
                f.GetBool("quest.garrow.doctrine_won")
                    ? "It proves my point in the cruelest possible hand. I put the Doom's own canon on trial and won — " +
                      "and here is the proof writ in a god's skull: even death answers to *something.* Even gods get " +
                      "judged. I was right. I have never wanted less to be right."
              : f.GetBool("quest.garrow.left_faith")
                    ? "I left the grey because I could not serve a law that erases the unclaimed. And here is that law's " +
                      "own god, dying like a man in a ditch. I feel no triumph. Only the long cold of how *contingent* " +
                      "all of it is — the Wall, the Doom, the judgement. People made it. People could unmake it."
              : f.GetBool("quest.garrow.recanted")
                    ? "I knelt. I kept the grey. And here is my god's own predecessor, skull and horns, becoming a crown " +
                      "for a monster. I chose to keep serving this office. Watching the forge-light... gods forgive me, " +
                      "I no longer know if I chose right. Stay close. I am not steady."
              : "There is a tribunal waiting for me, in the present — a heresy I'll have to answer for. Watching a " +
                "death-god die, I think I finally know what I'll tell them. Or I am more lost than I have ever been. " +
                "With a god's skull ringing like a struck bell, it is genuinely hard to say.";
            g.nodes.Add(new DialogueNode { id = "close", speaker = "Sister Garrow", text = close });
            return g;
        }

        // ---- Varra, in the Spellplague: the Weave tearing, every rule of magic coming apart ----
        public static DialogueGraph VarraSpellplague()
        {
            var f = GameFlags.Current;
            var g = ScriptableObject.CreateInstance<DialogueGraph>();
            g.conversationId = "spellplague.varra_witness"; g.startNodeId = "0";

            g.nodes.Add(new DialogueNode
            {
                id = "0", speaker = "Varra",
                text = "Oh, I *love* it here. Feel that? The Weave's coming apart at the seams — every rule that ever held " +
                       "magic in place, just... optional, in the blue fire. My pact-flame's going feral in my hands and " +
                       "I have never felt more *free,* which should probably frighten me more than it does.",
                onEnter = new[] { new FlagClause { key = "spellplague.varra_witnessed", op = FlagOp.SetTrue },
                                  new FlagClause { key = "companion.varra.approval", op = FlagOp.AddInt, amount = 5 } },
                choices = new[] { new DialogueChoice { text = "Careful. Free isn't the same as safe.", nextNodeId = "close" },
                                  new DialogueChoice { text = "What are you really thinking?", nextNodeId = "close" } }
            });

            string close =
                f.GetBool("quest.varra.patron_bound")
                    ? "I turned my patron into my *debtor,* remember? And now the whole architecture of contracts is " +
                      "tearing in this fire. If the rules that bind a leash break in here... gods, what does that do to " +
                      "one I already reversed? Exhilarating and terrifying at once. You know — my favourite weather."
              : f.GetBool("quest.varra.debt_taken")
                    ? "You put your name where mine was, on the books. And here's a place where the books mean *nothing* — " +
                      "where a contract's just ink that forgot to stay put. Don't you dare use that to get free of it by " +
                      "getting *unmade.* I'd carry that debt a thousand years before I'd let the blue fire take you instead."
              : f.GetBool("quest.varra.freed")
                    ? "I burned my contract with my own fire, on my own clock. And here's a whole *place* where fire " +
                      "forgets the fine print entirely. Feels like home, honestly — like the universe finally read the " +
                      "thing the way I always did and tore it up."
              : "There's an invoice with my name maturing, back in the present. Standing in here, where cause forgets to " +
                "come before effect, I keep having the same thought: maybe I run into the blue and the bill just never " +
                "finds me. ...Maybe. Don't let me. Probably don't let me.";
            g.nodes.Add(new DialogueNode { id = "close", speaker = "Varra", text = close });
            return g;
        }
    }
}
