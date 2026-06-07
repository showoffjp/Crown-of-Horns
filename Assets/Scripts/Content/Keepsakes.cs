using System.Collections.Generic;
using SunderedCrown.Core;

namespace SunderedCrown.Content
{
    /// <summary>
    /// Keepsakes — small mementos a companion presses on you when their personal quest resolves. Not gear,
    /// not stats: proof that someone let you matter to them. Each is gated on an existing flag (so it just
    /// appears when earned), and the Chronicle (C) shows the ones you carry. Pure data.
    /// </summary>
    public static class Keepsakes
    {
        public class Item
        {
            public string id;
            public string owner;       // companion display-ish name
            public string title;
            public string unlockFlag;  // appears once this flag is true
            public string flavor;
        }

        public static readonly List<Item> All = new List<Item>
        {
            new Item {
                id = "garrow_whetstone", owner = "Garrow", title = "Garrow's whetstone", unlockFlag = "quest.garrow.resolved",
                flavor = "A gravedigger's whetstone, worn concave by forty years of honest edges. She pressed it on you " +
                         "after the tribunal: \"For the work. Whatever god you do or don't keep.\"" },
            new Item {
                id = "roen_pick", owner = "Roen", title = "Roen's spare lockpick", unlockFlag = "quest.roen.resolved",
                flavor = "A pick filed down from a Harper cipher-pin. \"Every door's just a lie that hasn't been told " +
                         "the truth yet. You're better at that than me now. Don't let it go to your head.\"" },
            new Item {
                id = "varra_contract", owner = "Varra", title = "A char-edged scrap of contract", unlockFlag = "quest.varra.resolved",
                flavor = "The burnt corner of the contract for her soul. \"Proof I had a price once. Keep it — so one of " +
                         "us always remembers I'm off the books now.\"" },
            new Item {
                id = "naeve_proof", owner = "Naeve", title = "Naeve's first proof", unlockFlag = "quest.naeve.resolved",
                flavor = "A sliver of slate scratched with a child's equation — the first proof she ever derived, on a " +
                         "balcony that is now sky. \"This one built nothing. I want you to have the one that did no harm.\"" },
            new Item {
                id = "ilfaeril_signet", owner = "Ilfaeril", title = "Ilfaeril's renounced signet", unlockFlag = "quest.ilfaeril.resolved",
                flavor = "The house-signet he renounced after the vote, its crest filed smooth. \"A name I unmade, to " +
                         "remember the ones I helped unmake. Carry it lighter than I ever could.\"" },
            new Item {
                id = "maerin_pebble", owner = "Maerin", title = "A small named pebble", unlockFlag = "camp.nighttalk.maerin2.done",
                flavor = "A grey river-pebble, utterly ordinary, that Maerin pressed into your hand one grey morning. \"I named " +
                         "it. That's all it takes — a name, and meaning it. I named it after the first thing I'd refuse to forget. " +
                         "Don't ask which. Just... keep it on the list.\"" },
            new Item {
                id = "lowcity_token", owner = "Mother Cass", title = "The unclaimed's backwards token", unlockFlag = "almshouse.token",
                flavor = "A Kelemvor token turned backwards, the scales facing in. \"For when you reach the place that " +
                         "judges — so it knows the poor sent someone who'd speak for us.\"" },

            // ---- Romance keepsakes (the most intimate; only if the bond was sealed) ----
            new Item {
                id = "garrow_list", owner = "Garrow ♥", title = "Garrow's other list", unlockFlag = "romance.garrow.consummated",
                flavor = "The worn book where she writes the dead each night — opened, once, to the *other* list: the one " +
                         "of people she refuses to bury. Your name is at the top, in a hand that does not shake. \"Don't " +
                         "make a liar of it.\"" },
            new Item {
                id = "roen_innkey", owner = "Roen ♥", title = "A key to an inn that doesn't exist yet", unlockFlag = "romance.roen.consummated",
                flavor = "A brass key to The Wandering Niche — an inn he hasn't bought, with two chairs he hasn't placed. " +
                         "\"Down payment on an after. Hold onto it. Make me come back for it.\"" },
            new Item {
                id = "naeve_axiom", owner = "Naeve ♥", title = "Naeve's revised axiom", unlockFlag = "romance.naeve.consummated",
                flavor = "A single line of impossible mathematics, amended in her own hand: \"Premise: you are meaning. " +
                         "Specific. Demonstrable. Checked.\" She would unmake a second world before admitting she gave it to you." },
            new Item {
                id = "varra_ash", owner = "Varra ♥", title = "The ash of Varra's receipt", unlockFlag = "romance.varra.consummated",
                flavor = "A twist of paper holding the ash of the contract you burned together. \"What I'm worth now, I " +
                         "set. This is just to remember the day the price hit zero.\"" },
        };

        public static List<Item> Earned()
        {
            var f = GameFlags.Current;
            var outp = new List<Item>();
            foreach (var k in All)
                if (string.IsNullOrEmpty(k.unlockFlag) || f.GetBool(k.unlockFlag)) outp.Add(k);
            return outp;
        }
    }
}
