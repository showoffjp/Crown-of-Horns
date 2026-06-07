using System.Collections.Generic;
using SunderedCrown.Core;

namespace SunderedCrown.Content
{
    /// <summary>
    /// The Codex: a lore/bestiary compendium that fills in as you play. Each entry is gated on a
    /// GameFlags key — you only "know" what you've witnessed. Pure data, so the Codex screen (and any
    /// future journal) can render it without touching content. Add entries here; they auto-appear.
    /// </summary>
    public static class CodexContent
    {
        public enum Category { Premise, Masks, Companions, Bestiary, Lore }

        public class Entry
        {
            public string id;
            public Category category;
            public string title;
            public string unlockFlag;   // empty = known from the start
            public string body;
        }

        public static readonly List<Entry> All = new List<Entry>
        {
            // ---- Premise (known immediately) ----
            new Entry {
                id = "wall", category = Category.Premise, title = "The Wall of the Faithless", unlockFlag = "",
                body = "In the City of Judgement, souls who served no god are mortared alive into a vast wall " +
                       "and left to dissolve into nothing. Not punished. Erased. The Realms' quietest cruelty — " +
                       "and the wound at the heart of this saga." },
            new Entry {
                id = "returned", category = Category.Premise, title = "The Returned", unlockFlag = "",
                body = "You died in the harvest beneath Baldur's Gate, and something pulled you back. The dead " +
                       "do not come back. You did. The crack that let you through is a crack in time itself." },
            new Entry {
                id = "tactics", category = Category.Premise, title = "The Art of the Duel (combat actions)", unlockFlag = "",
                body = "Each turn you have a move and one action. Beyond attacking and casting, your action can buy:\n" +
                       "• DEFEND (G) — attacks against you have disadvantage until your next turn.\n" +
                       "• DASH (F) — extra movement equal to your speed this turn.\n" +
                       "• HELP (T) — aid an adjacent ally; their next attack has advantage.\n" +
                       "• DISENGAGE (X) — your movement this turn provokes no opportunity attacks.\n" +
                       "• SHOVE (V) — a Strength contest to push an adjacent enemy back a tile.\n" +
                       "• QUAFF (Q) — drink a healing draught from the party stash (the HUD shows how many remain).\n" +
                       "Three rules reward position: leaving an enemy's reach without Disengaging draws an OPPORTUNITY " +
                       "ATTACK; FLANKING a foe (an ally directly opposite it) gives advantage in melee; and firing a ranged " +
                       "attack through an interposing body grants the target half COVER (+2 AC). A foe dropped below half " +
                       "health flashes BLOODIED. Casters' leveled spells cost a slot (shown as L1–L5 on the button) — spend them wisely." },

            new Entry {
                id = "the_gate", category = Category.Premise, title = "Baldur's Gate", unlockFlag = "",
                body = "A great mercantile city on the Sword Coast, ruled by coin and the Flaming Fist, stacked steep from " +
                       "the harbor's Outer City slums to the Patriar estates above. You died in its lower wards and were " +
                       "Returned there. The Gate does not love its poor, but its poor keep their own quiet ledgers of who " +
                       "treated them like they counted — and yours, it turns out, is one of the names they keep." },

            new Entry {
                id = "returned_becoming", category = Category.Premise, title = "The Returned, Becoming", unlockFlag = "act2.breach_done",
                body = "You started as a question the universe asked by accident — a soul that wouldn't dissolve, thrown " +
                       "back through a crack. Somewhere between the Cinderhaunt and the Breach you stopped being an " +
                       "accident and started being a *choice*: every grave you honored, every companion you let matter, " +
                       "every cruelty you refused to inherit. The dead come back blank. You are filling yourself in, deed " +
                       "by deed, with the one thing the Wall can't erase — a person other people would refuse to forget." },

            // ---- The Four Masks (villains) ----
            new Entry {
                id = "aldric", category = Category.Masks, title = "Aldric Morn — the First Mask", unlockFlag = "aldric.met",
                body = "Once the Sword Coast's finest Doomguide of Kelemvor. He watched his godless daughter " +
                       "mortared into the Wall — and broke. Now he hunts the Crown of Horns to depose the god of " +
                       "the dead and tear the Wall down forever, no matter who among the living he must spend." },
            new Entry {
                id = "crown", category = Category.Masks, title = "The Crown of Horns", unlockFlag = "act4.toot_done",
                body = "Forged in the Time of Troubles from the skull of Myrkul, the dead god of death. To wear it " +
                       "is to inherit his hunger. The Crown does not grant power; it spends the wearer to feed itself." },
            new Entry {
                id = "unmade", category = Category.Masks, title = "The Unmade — the Second Mask", unlockFlag = "netheril.cleared",
                body = "The accreted nothing of every soul the cosmos ever threw away — all the Faithless who " +
                       "dissolved, gathered into a single appetite that wants only to finish the erasing. It is not " +
                       "evil. It is the logical end of a wall built to forget people." },
            new Entry {
                id = "last_returned", category = Category.Masks, title = "The Last Returned — the Third Mask", unlockFlag = "crownwars.cleared",
                body = "A hunter who wears your own face: the you from a future that already lost, walking back " +
                       "through history to stop yourself from making its mistakes. It does not die when struck — " +
                       "it kneels, when its Echoes fall, because it remembers being you." },
            new Entry {
                id = "lady", category = Category.Masks, title = "The Lady in the Margins — the Fourth Mask", unlockFlag = "readers_boon",
                body = "A fifth-dimensional reader who has been with you the whole way — the storyteller at the " +
                       "fire, the voice in the Vault, the one who keeps 'monitoring the situation.' She is what " +
                       "Naeve becomes, ascended out of the story and back into its margins. She does not want to " +
                       "win. She wants to see how it ends — and whether you can surprise her." },

            // ---- The Company (companions; fill in as they join) ----
            new Entry {
                id = "c_garrow", category = Category.Companions, title = "Sister Garrow", unlockFlag = "prologue.cleared",
                body = "A Doomguide of Kelemvor and a gravedigger's daughter, who learned death as a craft before she " +
                       "learned it as a faith. She has spent her life being *correct* and is no longer sure correct and " +
                       "good are the same word. Joins from the start. Gains approval for honoring the dead, mercy, and " +
                       "admitting doubt." },
            new Entry {
                id = "c_roen", category = Category.Companions, title = "Roen Alleywind", unlockFlag = "companion.roen.recruited",
                body = "An orphan of the Outer City the Harpers chose at fourteen — charm, lockpicks, and an exit built " +
                       "into every sentence. His whole life trained him that attachment is a liability; you are the bet " +
                       "he didn't mean to make. Gains approval for loyalty-under-fire and the truth he hates telling." },
            new Entry {
                id = "c_varra", category = Category.Companions, title = "Varra", unlockFlag = "companion.varra.recruited",
                body = "A tiefling warlock sold to a Hell at six years old by a broker with a kind voice and too many " +
                       "teeth. She tests everyone cruelly, certain she's unlovable because she's *priced.* Won not by " +
                       "grand gestures but by staying through the tests. Gains approval for proving her worth isn't her cost." },
            new Entry {
                id = "c_naeve", category = Category.Companions, title = "Naeve of the Seventh Enclave", unlockFlag = "companion.naeve.recruited",
                body = "A Netherese arcanist who helped derive the theorem Karsus weaponized — her clean mathematics, his " +
                       "unclean ambition, a sky that fell. She needs to learn some mistakes are *carried,* not erased. " +
                       "Gains approval for intellectual honesty, restraint, and truth over comfort." },
            new Entry {
                id = "c_ilfaeril", category = Category.Companions, title = "Ilfaeril", unlockFlag = "companion.ilfaeril.recruited",
                body = "A lord of the elven high courts who raised his hand, one terrible afternoon of the Crown Wars, to " +
                       "vote a people's souls *unmade* — and has spent ten thousand years as a half-ghost trying to atone. " +
                       "He must learn atonement is not self-erasure. Gains approval for mercy, oaths kept, and accepting forgiveness." },
            new Entry {
                id = "c_maerin", category = Category.Companions, title = "Maerin", unlockFlag = "companion.maerin.recruited",
                body = "The girl you pulled out of the Wall in the Breach — luminous, half-dissolved, and entirely her own. " +
                       "She is the proof that the unclaimed can be reclaimed, and the living reminder of what it cost. She " +
                       "is not your reason. She insists on that." },

            // ---- Companions: who they became, once their quest resolved ----
            new Entry {
                id = "c_garrow_after", category = Category.Companions, title = "Garrow — after the tribunal", unlockFlag = "quest.garrow.resolved",
                body = "She put Kelemvor's own canon on trial and lived with the verdict. Whether she kept the grey, took " +
                       "the doctrine back, or walked out from under thirty years, the woman who came out the far side no " +
                       "longer mistakes 'correct' for 'good' — and tends the dead by her own conscience, not the church's." },
            new Entry {
                id = "c_roen_after", category = Category.Companions, title = "Roen — after the safehouse", unlockFlag = "quest.roen.resolved",
                body = "The Wrenna affair forced the question every con avoids: which lie do you keep, and who pays for it? " +
                       "However he chose, the man stopped building exits into his sentences. The forger finally let one " +
                       "thing be the original — and it terrifies him more than any heist." },
            new Entry {
                id = "c_varra_after", category = Category.Companions, title = "Varra — after the reckoning", unlockFlag = "quest.varra.resolved",
                body = "Twenty years of a clock in her chest, and she met the collector on her own terms at last — leash " +
                       "turned, debt carried, or contract burned. Whatever the price now reads, she sets it herself. The " +
                       "first transaction of her life with no fine print." },
            new Entry {
                id = "c_naeve_after", category = Category.Companions, title = "Naeve — after the shard", unlockFlag = "quest.naeve.resolved",
                body = "She stopped trying to redo the proof of her own life. Whether she rekindled the last Weave, let the " +
                       "sky finish falling, or froze it mid-fall, the arithmetic of grief finally balanced — not solved, " +
                       "carried. A thousand years late, she let Netheril be past tense." },
            new Entry {
                id = "c_ilfaeril_after", category = Category.Companions, title = "Ilfaeril — after the forgiveness", unlockFlag = "quest.ilfaeril.resolved",
                body = "Maerith's mercy reached him across ten thousand years — as a door, a sword, or a weight he could not " +
                       "set down. However he answered it, the rigid ancient thing in him moved for the first time in an age. " +
                       "Penitent still, perhaps; but no longer entirely alone in it." },

            // ---- Companions: the bonds, once sealed ----
            new Entry {
                id = "love_garrow", category = Category.Companions, title = "Garrow — the one she'll keep", unlockFlag = "romance.garrow.consummated",
                body = "The woman who buried everyone and believed in keeping no one learned, late and against all her " +
                       "training, to keep one person. Your name is on the *other* list now — the one of people she refuses " +
                       "to lay down. She has never once meant a promise about the dead. She means this one." },
            new Entry {
                id = "love_roen", category = Category.Companions, title = "Roen — the con he won't run", unlockFlag = "romance.roen.consummated",
                body = "He builds exits into every sentence — except, lately, the ones he says to you. The forger who " +
                       "buried his own first name decided to let one thing be the original. Staying is the bravest con he's " +
                       "ever run, and the only one he refuses to fold." },
            new Entry {
                id = "love_naeve", category = Category.Companions, title = "Naeve — the revised axiom", unlockFlag = "romance.naeve.consummated",
                body = "She modelled grief as a decay function for a thousand years; you are the variable that broke the " +
                       "model and the first she has no wish to solve. She amended her oldest premise by hand: *you are " +
                       "meaning. Specific. Demonstrable. Checked.* She would unmake a second world before admitting it aloud." },
            new Entry {
                id = "love_varra", category = Category.Companions, title = "Varra — off the books", unlockFlag = "romance.varra.consummated",
                body = "A soul that was a line item since she was six chose you freely — the first transaction of her life " +
                       "with no fine print at all. Whatever she's worth now, she sets it, off the books, un-priceable. She " +
                       "still won't say the soft thing out loud. She just stopped sitting nearest the exit when you're in the room." },

            // ---- Bestiary ----
            new Entry {
                id = "unbound_maw", category = Category.Bestiary, title = "The Unbound Maw", unlockFlag = "prologue.cleared",
                body = "The thing at the bottom of the Cinderhaunt — the first real horror the Returned ever puts down. Not a " +
                       "demon, not undead exactly: a *hunger* that the harvest fed and forgot to leash, grown fat on the " +
                       "souls that didn't make it out with you. It is the saga's whole thesis in miniature, met on the very " +
                       "first night: a cruelty no one built on purpose, that just *accreted* in a place where the dead were " +
                       "treated as fuel. You will meet its big brother at the end of the world. It is called the Unmade." },
            new Entry {
                id = "cinderhaunt_dead", category = Category.Bestiary, title = "The Cinderhaunt Restless", unlockFlag = "prologue.cleared",
                body = "The harvest beneath Baldur's Gate did not take only you. The Cinderhaunt's dead rose with you — " +
                       "shambling things, half-rotted, more confused than cruel, clawing toward the crack you tore on your " +
                       "way back. They are not your enemies, exactly. They are your *fellow-travellers,* the ones who didn't " +
                       "make it all the way out. You put them down with a tenderness that surprises you. There but for a " +
                       "stubborn soul go you." },
            new Entry {
                id = "netherese_arcanist", category = Category.Bestiary, title = "Netherese Arcanists", unlockFlag = "netheril.cleared",
                body = "The enclave's defenders, certain to the last heartbeat of a falling city that their borrowed magic " +
                       "could not also fall. They wield shattering glass-bursts and lances of stolen lightning — AoE death " +
                       "that punishes a clustered party. They are not evil. They are a thousand years of hubris given a wand " +
                       "and ten seconds to live." },
            new Entry {
                id = "the_echoes", category = Category.Bestiary, title = "The Echoes", unlockFlag = "crownwars.cleared",
                body = "Not clones — *arguments.* In the Mirror, the loop throws back twisted versions of your own party: the " +
                       "choices you didn't make, the cruelties you nearly chose, your companions as they'd be if you had " +
                       "failed them. Fighting them is the strangest combat in the saga, because every hit lands on a road " +
                       "not taken. They do not die so much as get *answered* — and the Last Returned that leads them kneels, " +
                       "rather than falling, once its Echoes are spent. It has fought this fight before. It knows how it ends. " +
                       "It is only checking whether, this time, you do too." },
            new Entry {
                id = "elven_bladesinger", category = Category.Bestiary, title = "Elven Blade-Singers", unlockFlag = "crownwars.cleared",
                body = "Warrior-mages of the Crown Wars, beautiful and lethal and utterly sure their cause is just — radiant " +
                       "smites and moonbows in service of a court that is, that very hour, voting a people into nothing. The " +
                       "tragedy of the Crown Wars is not that monsters fought it. It is that *these* did, and called it law." },
            new Entry {
                id = "unmade_aberration", category = Category.Bestiary, title = "Unmade Aberrations", unlockFlag = "act4.spellplague_done",
                body = "In the Spellplague's blue fire, where cause forgets to come before effect, the Unmade's grief takes " +
                       "half-shapes — limbs that end in question marks, faces that haven't decided whose they are. They are " +
                       "not creatures so much as *unfinished sentences,* lashing at anything still confident enough to be a " +
                       "whole thing. Killing them feels less like victory than like finishing a thought for someone who can't." },
            new Entry {
                id = "wall_wraith", category = Category.Bestiary, title = "Wall-Wraiths", unlockFlag = "fugue.arrived",
                body = "Not quite ghosts — the half-dissolved residue of souls the Wall has been eating for centuries, " +
                       "snagged on the edge of forgetting. They drift the Fugue's grey marches, reaching for any warmth " +
                       "that still has a name. They are not malicious. They are *hungry for inclusion* — which, the longer " +
                       "you look, is somehow worse." },
            new Entry {
                id = "avatar_touched", category = Category.Bestiary, title = "The Avatar-Touched", unlockFlag = "act4.toot_done",
                body = "During the Time of Troubles the gods walked Toril as flesh, and what they touched took on a " +
                       "sliver of the divine — and the divine madness with it. Dangerous, and pitiable." },
            new Entry {
                id = "mythallar_colossus", category = Category.Bestiary, title = "The Mythallar Colossus", unlockFlag = "netheril.boss_down",
                body = "The last guardian of a flying city's magic-engine, woven of gold and bound arcana — still " +
                       "executing its final order to defend the core, ten heartbeats from the ground. It did not know the " +
                       "city was already dead. You taught it. It was, in the end, only following a command no one was " +
                       "left to rescind." },
            new Entry {
                id = "first_unmade", category = Category.Bestiary, title = "The First Unmade", unlockFlag = "crownwars.boss_down",
                body = "The soul the elven high court voted, first of all, to erase — risen from the half-place between " +
                       "death and nothing, carrying ten thousand years of a grief that was never permitted a grave. It " +
                       "is not evil. It is the first wound of the Wall, given just enough shape to ask *why.* Ilfaeril " +
                       "could not answer it. He could only help you lay it down." },
            new Entry {
                id = "avatar_bone", category = Category.Bestiary, title = "The Avatar of Bone", unlockFlag = "toot.avatar_down",
                body = "During the Time of Troubles the gods walked Toril as flesh — and could die as flesh. At the forge " +
                       "where Myrkul's skull was beaten into the Crown of Horns, a shard of that dying divinity rose to " +
                       "guard the work: a thing of bone and god-madness, neither fully avatar nor fully dead. You proved " +
                       "it could be the latter." },
            new Entry {
                id = "herald_unmade", category = Category.Bestiary, title = "The Herald of the Unmade", unlockFlag = "spellplague.herald_down",
                body = "Not a creature so much as a *direction* given teeth — the Unmade's appetite, wearing enough shape " +
                       "to fight in the place where causality runs thin. It does not hate you. It simply wants the " +
                       "erasing finished, and you are the one soul that walked back out of the Wall to argue. You met it " +
                       "in the blue fire and held. It will remember that holding." },
            new Entry {
                id = "bluefire", category = Category.Bestiary, title = "Blue Fire (the Spellplague)", unlockFlag = "act4.spellplague_done",
                body = "Where the Weave tore, causality runs like wet ink. Blue fire lashes at random, and two " +
                       "things that should be in different places simply swap. Not a creature — a place that bites." },

            // ---- Lore ----
            new Entry {
                id = "netheril", category = Category.Lore, title = "Karsus's Folly (Netheril)", unlockFlag = "netheril.cleared",
                body = "−339 DR: the archwizard Karsus cast a spell to become a god, and for one heartbeat he " +
                       "succeeded — then magic died, and the flying cities fell out of the sky. You walked one down." },
            new Entry {
                id = "crownwars", category = Category.Lore, title = "The Crown Wars", unlockFlag = "crownwars.cleared",
                body = "The elven civil wars that birthed the drow — and, in this saga, the first time a court " +
                       "voted to damn a soul to nothing. The Wall was not handed down. It was decided, by people." },
            new Entry {
                id = "vault", category = Category.Lore, title = "The Vault of Tens", unlockFlag = "riddle.entered",
                body = "The Lady's riddle room, after the fashion of older, crueler vaults. Place the tokens, speak " +
                       "the answers — but know that with her, wit beats correctness. She would rather be amused " +
                       "than agreed with." },
            new Entry {
                id = "lore_doom", category = Category.Lore, title = "The Doom of Kelemvor", unlockFlag = "aldric.met",
                body = "Kelemvor is the current god of the dead — successor to Myrkul, judge of every soul that comes to " +
                       "the City of Judgement. His Doom is supposed to be fair: none shall pass unjudged. But the Wall " +
                       "is what the Doom does to the unclaimed — discard, not judge — and that gap between the doctrine " +
                       "and the deed is the wound the whole saga presses on." },
            new Entry {
                id = "lore_decision", category = Category.Lore, title = "The Wall Was a Decision", unlockFlag = "crownwars.cleared",
                body = "The deepest, most dangerous idea in the saga: the Wall of the Faithless was not handed down by " +
                       "the gods. It was *voted* — in elven high courts during the Crown Wars, a verdict that hardened " +
                       "over millennia into cosmic law. What people decided, people can un-decide. That is either hope " +
                       "or apocalypse, depending entirely on who is holding the pen." },
            new Entry {
                id = "lore_toot", category = Category.Lore, title = "The Time of Troubles", unlockFlag = "act4.toot_done",
                body = "1358 DR: Ao cast the gods down to walk Toril as mortals, and mortals could kill them. Myrkul " +
                       "died screaming over Waterdeep — and his skull was forged into the Crown of Horns. Gods, it turns " +
                       "out, are no more permanent than the Wall they keep. Everything can be buried. Even the burier." },
            new Entry {
                id = "lore_spellplague", category = Category.Lore, title = "The Spellplague", unlockFlag = "act4.spellplague_done",
                body = "1385 DR: Mystra murdered, the Weave torn, and for a generation magic ran like wet ink — blue " +
                       "fire, floating stone, cause limping after effect. It is where reality is thinnest, and so where " +
                       "the Unmade comes closest to finishing its erasing. The place the rules forgot to hold." },

            new Entry {
                id = "breach", category = Category.Lore, title = "The Breach", unlockFlag = "act2.breach_done",
                body = "It is possible to reach into the Wall and pull one soul back out. It is not possible to do " +
                       "so without a price, and the price is not gold. The Wall takes someone you love in trade, " +
                       "and it chooses by who you loved most. Authored, not arbitrary." },

            // ---- Lore: the Lower City (Act II) ----
            new Entry {
                id = "lowercity", category = Category.Lore, title = "The Lower City", unlockFlag = "prologue.cleared",
                body = "The Gate below the walls, where the Flaming Fist doesn't bother to patrol and the temples " +
                       "send their bills before their blessings. Home to the godless, the priced-out, and the " +
                       "grieving — the exact people the Wall of the Faithless is built to forget. Whatever you are " +
                       "to the gods, down here you are simply whoever the streets decide you are. They keep a ledger." },
            new Entry {
                id = "almshouse", category = Category.Lore, title = "The Almshouse of the Unclaimed", unlockFlag = "lowcity.visited_almshouse",
                body = "A candlelit refuge run by Mother Cass, where the godless poor keep each other warm against a " +
                       "cosmos that would erase them. Its heart is the Wall of Names: a plastered wall crowded with " +
                       "the unclaimed dead, written and rewritten by the living who refuse to let them dissolve. It " +
                       "is the whole argument of your saga, made by people who never heard your name." },
            new Entry {
                id = "choir", category = Category.Bestiary, title = "The Faithless Choir", unlockFlag = "quest.choir.resolved",
                body = "Street-evangelists of the Unmade who preach to the Gate's grieving poor that the Wall can " +
                       "fall — and hold the Returned up as living proof. Grief is their recruiter; despair, their " +
                       "scripture. Suppress them and they harden; speak for them and they remember. The Unmade's " +
                       "first roots in the present, growing where the temples left the soil bare." },
            new Entry {
                id = "hollow_cantor", category = Category.Bestiary, title = "The Hollow Cantor", unlockFlag = "quest.choir.cell_cleared",
                body = "When the Choir's preaching curdled into a militant cell, it found a leader in the undercroft: " +
                       "a hollowed thing that had stopped grieving and started recruiting martyrs. You broke it before " +
                       "it could make one. The Lower City slept easier, and never quite knew why." },
            new Entry {
                id = "gravetithe", category = Category.Lore, title = "The Grave-Tithe", unlockFlag = "quest.tithe.resolved",
                body = "The quiet racket by which the poor are charged for a consecrated grave — pay, or your dead " +
                       "wait Faithless in the pauper's pit, halfway to the Wall already. The Doom never charged for " +
                       "rest; the church just read the part of the canon that paid. A small mirror of the whole " +
                       "cosmic cruelty: a price put on whether the dead are allowed to be remembered." },
            new Entry {
                id = "place_market", category = Category.Lore, title = "The Shrunken Quarter", unlockFlag = "lowcity.visited_market",
                body = "A thin market that has seen better centuries, where a god's statue was once pried from its niche " +
                       "and sold in a lean year — and the poor leave offerings to the empty alcove anyway. Faith in the " +
                       "Gate is not about whether anyone is listening. It is about refusing to stop being kind in case." },
            new Entry {
                id = "place_docks", category = Category.Lore, title = "The Chionthar Docks", unlockFlag = "lowcity.visited_docks",
                body = "The working waterfront where the river meets the Gate. The dockfolk knot a cord for every drowned " +
                       "pauper the church wouldn't bury — the Chionthar keeps them for free, they say, which is more than " +
                       "the Doom ever did. Even the river, it turns out, remembers the dead better than the law does." },
            new Entry {
                id = "vayle", category = Category.Lore, title = "High Doomguide Vayle", unlockFlag = "crownwars.cleared",
                body = "The church's iron conscience — the senior Doomguide who will, at the Court, defend the Wall to the last " +
                       "with the most dangerous argument there is: *she is not wrong about the alternatives.* Tear the Wall " +
                       "down carelessly and death itself unbinds; the Unmade is real and patient and worse than the cruelty " +
                       "that contains it. Vayle is not a villain. She is the part of you that knows the cost of mercy, given a " +
                       "voice and a sword. The hardest ending in the saga is the one where you stand with her, understand her " +
                       "completely, and keep the oldest atrocity in the world because she made the case and you could not break it." },
            new Entry {
                id = "kelemvor", category = Category.Lore, title = "Kelemvor, Lord of the Dead", unlockFlag = "fugue.arrived",
                body = "The current god of death — once a mortal man, Kelemvor Lyonsbane, who took the office after the Time " +
                       "of Troubles. He inherited the Wall of the Faithless; he did not build it. By all accounts he is a " +
                       "*kind* god, as gods of death go — he tried, early, to make judgement gentler, and the cosmos itself " +
                       "(and Jergal's old contracts) pushed back until he hardened into the role. That is the quiet horror " +
                       "Garrow serves and Aldric rages against: not a cruel god, but a kind one who could not make the " +
                       "machine kind, and stopped trying. The question the saga puts to him is the one he stopped asking: " +
                       "*why keep a cruelty you inherited, simply because you inherited it?*" },
            new Entry {
                id = "lore_false_faithless", category = Category.Lore, title = "The Faithless and the False", unlockFlag = "fugue.arrived",
                body = "Two different sins, the doctrine insists. The *False* betrayed their god — broke an oath, turned coat " +
                       "— and are punished in torment. The *Faithless* simply never knelt to anyone, and are not punished at " +
                       "all: they are *erased,* mortared into the Wall to dissolve. The cosmos reserves its cruelest fate not " +
                       "for the wicked but for the unaffiliated. That distinction — that you must belong to *someone* to be " +
                       "allowed to keep existing — is the rotten beam the whole saga leans on, and means to pull out." },
            new Entry {
                id = "fugue_plane", category = Category.Lore, title = "The Fugue Plane", unlockFlag = "fugue.arrived",
                body = "The grey waystation where every mortal soul arrives at death, to wait under the Crystal Spire until " +
                       "its god comes to claim it. The faithful are gathered. The False and the Faithless are not — and the " +
                       "longer they wait unclaimed, the closer the slow walk to the Wall. You have stood here as a *living* " +
                       "thing, which is its own quiet blasphemy, and walked back out, which is a louder one." },
            new Entry {
                id = "city_judgement", category = Category.Lore, title = "The City of Judgement", unlockFlag = "fugue.arrived",
                body = "Kelemvor's seat, where the dead are weighed and sorted — paradise, punishment, or the Wall. It was " +
                       "meant to be just. Justice, run long enough by people who stopped reading their own canon, hardened " +
                       "into procedure; and procedure, asked what to do with a soul no god will sign for, found the cheapest " +
                       "answer and called it law. The whole saga is the argument with that answer." },
            new Entry {
                id = "lore_timecrack", category = Category.Lore, title = "The Crack in Time", unlockFlag = "netheril.cleared",
                body = "The dead do not come back; you did, and the wrongness of that left a fault line. When you Skip into the " +
                       "fallen ages, you are not time-travelling so much as *falling through your own broken seam* into the " +
                       "moments the Wall was made — Netheril, the Crown Wars, the forge, the blue fire. You cannot change what " +
                       "happened. But you were never meant to *witness* it either, and witness, the Lady would say, is a kind " +
                       "of verdict all its own. Every age you walk, you are quietly entering it into a record someone is keeping." },
            new Entry {
                id = "lore_aldric_daughter", category = Category.Lore, title = "Aldric's Daughter", unlockFlag = "aldric.met",
                body = "Her name was Elaine. She believed in nothing — not out of spite, just an honest, stubborn unbelief her " +
                       "father, a Doomguide of Kelemvor, could never argue her out of and loved her too much to try. When " +
                       "she died, the Doom her father had served his whole life sorted her with the Faithless and mortared " +
                       "her into the Wall to dissolve. Aldric watched. Everything he has become since — the hunt for the " +
                       "Crown, the willingness to spend the living — is one man's refusal to accept that the universe was " +
                       "*allowed* to do that to his daughter for the crime of being honest. The horror is that he is not wrong." },
            new Entry {
                id = "lore_jergal", category = Category.Lore, title = "Jergal, the Scribe of the Dead", unlockFlag = "crownwars.cleared",
                body = "Before Myrkul, before Kelemvor, the dead were kept by Jergal — the first god of the end, who grew so " +
                       "*bored* of eternity's bookkeeping that he handed the whole office away and took a clerk's stool " +
                       "instead. He is still there, filing, faintly amused, ten thousand years into a retirement no one " +
                       "noticed. If any power in the multiverse would find the rewriting of the Law a delightful way to pass " +
                       "an age, it is the one who has been waiting, patiently, for someone interesting to ask." },
            new Entry {
                id = "lore_ashpact", category = Category.Lore, title = "The Ash Pact", unlockFlag = "companion.varra.recruited",
                body = "The infernal compact that owns Varra's soul — a Hell's standing offer to the Gate's desperate poor: " +
                       "power now, collection later, the fine print written in a language no six-year-old could read and no " +
                       "court will overturn. It is not, the devils insist, *cruelty.* It is a *contract,* freely entered, " +
                       "terms disclosed. The saga keeps noticing how often the worst things wear that exact defense — the " +
                       "Wall, the tithe, the Pact — all of them just paperwork, all of them just the rules, all of them a " +
                       "choice someone made and called inevitable so they'd never have to make it again." },
            new Entry {
                id = "lore_weave", category = Category.Lore, title = "The Weave", unlockFlag = "act4.spellplague_done",
                body = "Magic in the Realms is not raw — it is *mediated,* drawn through the Weave, the goddess Mystra's living " +
                       "lattice laid over reality so that mortals can touch power without being unmade by it. It is a mercy and " +
                       "a leash both. Twice in the saga you watch it tear: once when Karsus murders its first keeper and " +
                       "Netheril falls, and again when Mystra is murdered a second time and the **Spellplague** boils causality " +
                       "loose. The pattern the Returned cannot stop noticing: every catastrophe in Realms history is some " +
                       "proud hand reaching past the mediator — past the leash, past the law — to grab the power directly. The " +
                       "Wall is only the same reach aimed at the dead.",
            },
            new Entry {
                id = "lore_mythallar", category = Category.Lore, title = "The Mythallar", unlockFlag = "netheril.cleared",
                body = "The engine of Netheril's glory: a great captured node of raw magic that let any dabbler wield an " +
                       "archmage's power, and held the flying cities aloft. It made an empire of wonders in a few " +
                       "generations — and made that empire brittle, dependent, and blind, because when a god-mad mortal " +
                       "named Karsus reached up to *steal divinity itself,* the Weave faltered and every mythallar in the " +
                       "sky died at once. The cities fell in a single afternoon. Naeve watched hers come down. The lesson " +
                       "the Realms refused to learn from it is the one the whole saga keeps re-teaching: borrowed power, " +
                       "uncounted, always comes due — and never to the one who borrowed it." },
            new Entry {
                id = "lore_enclave", category = Category.Lore, title = "The Seventh Enclave", unlockFlag = "companion.naeve.recruited",
                body = "One of the floating cities of Netheril, kept aloft by a mythallar's borrowed magic — gardens and " +
                       "spires and a thousand years of arcane certainty, hung in the sky like an argument no one expected " +
                       "to lose. Naeve was its finest mind. She watched it fall when Karsus reached for godhood and the " +
                       "Weave let go. Every word in her language is now a tombstone for it." },
            new Entry {
                id = "lore_court", category = Category.Lore, title = "The Court of the First Damnation", unlockFlag = "companion.ilfaeril.recruited",
                body = "An elven high court of the Crown Wars, where — in a clean, well-lit room, by orderly vote — the " +
                       "multiverse first decided that a soul could be *unmade* rather than merely judged. They called it " +
                       "mercy: better nothing than torment. Ilfaeril raised his hand from the third bench. He has spent " +
                       "ten thousand years learning the word they should have used instead. *Cowardice.*" },
            new Entry {
                id = "lore_anchor", category = Category.Lore, title = "What Love Is, in This Story", unlockFlag = "act2.breach_done",
                body = "The Breach taught you the saga's cruelest grammar: here, love is never a reward. It is a *stake.* To " +
                       "let someone matter is to hand the cosmos a lever it will, sooner or later, pull. The brave thing is " +
                       "not refusing to love — it is loving anyway, with both eyes open, knowing the bill it runs up. The " +
                       "whole ending comes to rest, finally, on whichever face you were reckless enough to choose." },
            new Entry {
                id = "lore_stay_gone", category = Category.Lore, title = "Why the Dead Stay Gone", unlockFlag = "readers_boon",
                body = "You are the Returned because, somewhere, the loop keeps failing the same way: a soul too stubborn or " +
                       "too loved to dissolve, thrown back through the crack to try again. The dead stay gone because staying " +
                       "gone is a *choice* almost no one is given and fewer still can make. The Lady reads to find out if, " +
                       "this time, you'll be the one who can — and whether she wants you to be." },
            new Entry {
                id = "loop_remembers", category = Category.Lore, title = "The Loop Remembers", unlockFlag = "ng.plus",
                body = "You have walked this saga to its end before — and begun again. The world does not know it; the " +
                       "Lower City mud is the same mud, the harvest the same harvest. But something in the white space " +
                       "between the lines kept the count. The Lady always said she was only watching how it ends. She " +
                       "never said you'd only get to find out once. Whatever you chose last time, the wheel turned, and " +
                       "here you are at the start of it, carrying nothing but the shape of what you learned." },
        };

        /// <summary>Entries the player has unlocked, in declaration order.</summary>
        public static List<Entry> Known()
        {
            var f = GameFlags.Current;
            var outp = new List<Entry>();
            foreach (var e in All)
                if (string.IsNullOrEmpty(e.unlockFlag) || f.GetBool(e.unlockFlag)) outp.Add(e);
            return outp;
        }

        public static int TotalCount => All.Count;
    }
}
