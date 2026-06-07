using System.Collections.Generic;
using SunderedCrown.Characters;
using SunderedCrown.Core;

namespace SunderedCrown.Content
{
    /// <summary>
    /// Party cross-talk at the fire — the BG2 staple where two companions argue, needle, or quietly
    /// understand each other while you listen. Each exchange needs both speakers *fielded*, optionally a
    /// flag, and plays once (camp.banter.&lt;id&gt;.done). Pure data; CampScene surfaces the best-eligible
    /// one under "The party falls to talking." Each theme pairs two companions' wounds against each other.
    /// </summary>
    public static class CampGroupBanter
    {
        public class Exchange
        {
            public string id;
            public string aMatch, bMatch;   // both must be in the active party
            public string requiresFlag;     // optional bool gate ("" = none)
            public string title;
            public string[] lines;
        }

        public static readonly List<Exchange> All = new List<Exchange>
        {
            new Exchange {
                id = "naeve_garrow", aMatch = "Naeve", bMatch = "Garrow",
                title = "Naeve & Garrow — knowledge meets faith",
                lines = new[] {
                    "<b>Naeve:</b> \"You bury them with words you suspect are false. I find that fascinating. Why keep the rite?\"",
                    "<b>Garrow:</b> \"For sitting with the dying when the people who love them can't bear to. The words " +
                    "aren't for the dead, Arcanist. They never were.\"",
                    "<b>Naeve:</b> (a long pause) \"...That is a better proof of a god than any I derived in a thousand " +
                    "years. I withdraw the question.\"",
                    "<b>Garrow:</b> \"Keep it. Questions are the only prayer I still trust.\"" }
            },
            new Exchange {
                id = "roen_varra", aMatch = "Roen", bMatch = "Varra",
                title = "Roen & Varra — two ways out of a bad deal",
                lines = new[] {
                    "<b>Varra:</b> \"You always sit nearest the exit. I respect it. Professional courtesy.\"",
                    "<b>Roen:</b> \"Says the woman with an escape clause stitched into her soul.\"",
                    "<b>Varra:</b> \"Mine came with the body. Yours you *chose.* That's sadder, somehow.\"",
                    "<b>Roen:</b> (grinning) \"Don't psychoanalyze the rogue, it's bad for morale.\" (He does not move " +
                    "away from the exit. Neither does she.)" }
            },
            new Exchange {
                id = "ilfaeril_garrow", aMatch = "Ilfaeril", bMatch = "Garrow",
                title = "Ilfaeril & Garrow — two servants of the same cruel law",
                lines = new[] {
                    "<b>Garrow:</b> \"You helped *write* the Wall. I spent my life feeding it. We are not so different, elf.\"",
                    "<b>Ilfaeril:</b> \"No. You buried them with grief in your hands. I voted, and went to dinner. Do not " +
                    "flatter my sin by comparing it to your service.\"",
                    "<b>Garrow:</b> \"...Ten thousand years, and you still keep the accounts that carefully.\"",
                    "<b>Ilfaeril:</b> \"It is the only penance left that no one can take from me. Let an old man keep his ledger.\"" }
            },
            new Exchange {
                id = "varra_naeve", aMatch = "Varra", bMatch = "Naeve",
                title = "Varra & Naeve — a contract and a catastrophe",
                lines = new[] {
                    "<b>Varra:</b> \"So you ended a world with a math error. I got sold for one signature. Whose is worse?\"",
                    "<b>Naeve:</b> \"Mine. Yours was done *to* you. Mine I did, cleanly, with elegant proofs and an " +
                    "untroubled conscience. The conscience came later. It has been catching up ever since.\"",
                    "<b>Varra:</b> \"Huh. Here I thought I was the one carrying the bigger tab.\" (She passes the flask.) " +
                    "\"To bad arithmetic.\"",
                    "<b>Naeve:</b> (taking it) \"To carrying it anyway.\"" }
            },
            new Exchange {
                id = "roen_garrow", aMatch = "Roen", bMatch = "Garrow",
                title = "Roen & Garrow — the liar and the woman who can't",
                lines = new[] {
                    "<b>Roen:</b> \"You never lie. Not once. It's unnerving. How do you *survive* it?\"",
                    "<b>Garrow:</b> \"I dig graves for liars and honest folk alike. The hole's the same size. Takes the " +
                    "drama out of it.\"",
                    "<b>Roen:</b> \"...That's the most comforting thing anyone's said to me in years, and I hate that it is.\"",
                    "<b>Garrow:</b> (almost a smile) \"Grief makes poor company of us all, thief. Sit. The fire doesn't care what you are.\"" }
            },
            new Exchange {
                id = "ilfaeril_naeve", aMatch = "Ilfaeril", bMatch = "Naeve",
                title = "Ilfaeril & Naeve — two who ended worlds",
                lines = new[] {
                    "<b>Naeve:</b> \"Ten thousand years of guilt. I am scarcely a thousand into mine and already it is " +
                    "unbearable. Tell me it gets easier.\"",
                    "<b>Ilfaeril:</b> \"It does not get easier. It gets *familiar*, which the young mistake for easier. " +
                    "The weight is the same. You simply learn the shape of your own back under it.\"",
                    "<b>Naeve:</b> \"...That is the least comforting true thing anyone has ever said to me.\"",
                    "<b>Ilfaeril:</b> \"I did not offer comfort. I offered company. At our scale of sin, child, company is " +
                    "the only mercy left — and it is not nothing.\"" }
            },
            new Exchange {
                id = "varra_garrow", aMatch = "Varra", bMatch = "Garrow",
                title = "Varra & Garrow — the priced soul and the gravedigger",
                lines = new[] {
                    "<b>Garrow:</b> \"Your soul's promised to a Hell. By the Doom's law, that makes you mine to weigh, one day.\"",
                    "<b>Varra:</b> \"Romantic. Most people buy me a drink first.\"",
                    "<b>Garrow:</b> \"I don't weigh the way the church does. I'd dig you a grave with your name cut deep — " +
                    "godless, pact-bound, and all — and dare anyone alive to call it heresy.\"",
                    "<b>Varra:</b> (quiet, for once) \"...Careful, Doomguide. Say a thing like that to a girl who's only " +
                    "ever been a line item, and she might start to believe you.\"" }
            },
            new Exchange {
                id = "roen_teases_garrow", aMatch = "Roen", bMatch = "Garrow",
                requiresFlag = "romance.garrow.turn",
                title = "Roen, on a certain romance",
                lines = new[] {
                    "<b>Roen:</b> \"So. You and the boss. The grimmest woman on the Sword Coast, and our Returned looks at " +
                    "you like a sunrise. I have questions. I have *so many* questions.\"",
                    "<b>Garrow:</b> \"Ask one and I'll bury you with full rites.\"",
                    "<b>Roen:</b> \"See — *that* — that's the romance, isn't it? You threaten a man's funeral and they find " +
                    "it charming. Gods. You two are broken in exactly the same shape. It's genuinely beautiful. Don't " +
                    "tell anyone I said that.\"",
                    "<b>Garrow:</b> (to you, dry as a crypt) \"I will give him to the Wall myself.\" (But she is, almost, smiling.)" }
            },
            new Exchange {
                id = "varra_teases_naeve", aMatch = "Varra", bMatch = "Naeve",
                requiresFlag = "romance.naeve.turn",
                title = "Varra, on a certain romance",
                lines = new[] {
                    "<b>Varra:</b> \"So you're courting the woman who ended a civilization with a *math error.* Bold. " +
                    "I court disaster too, but I usually buy it a drink first.\"",
                    "<b>Naeve:</b> \"The Returned did not court me. The Returned presented a logical impossibility I have " +
                    "been unable to refute and have, after due process, accepted into the proof.\"",
                    "<b>Varra:</b> (delighted) \"Oh, she's got it *bad.* That's the most lovesick sentence I've ever heard " +
                    "and it had the word 'proof' in it. Returned, you absolute menace, well done.\"" }
            },
            new Exchange {
                id = "garrow_teases_roen", aMatch = "Garrow", bMatch = "Roen",
                requiresFlag = "romance.roen.turn",
                title = "Garrow, on a certain romance",
                lines = new[] {
                    "<b>Garrow:</b> \"You. Thief. You've stopped building exits into your sentences when you talk to them.\"",
                    "<b>Roen:</b> \"...Have I? Huh. Don't tell anyone. That's a professional liability, that is.\"",
                    "<b>Garrow:</b> \"I bury secrets for a living. Yours is safe.\" (a rare, dry warmth) \"For what it's " +
                    "worth, thief — staying is the bravest con you've ever run. Don't fold now.\"" }
            },
            new Exchange {
                id = "naeve_maerin", aMatch = "Naeve", bMatch = "Maerin",
                title = "Naeve & Maerin — the theorist and the proof she never wanted",
                lines = new[] {
                    "<b>Naeve:</b> \"You were inside it. The unmaking. I have models of that place — elegant, recursive, " +
                    "almost beautiful. Tell me how wrong they are. I find I need to know.\"",
                    "<b>Maerin:</b> \"There's no math in there. That's the part your models can't hold. It isn't a place " +
                    "that *runs* on anything. It just... forgets you, very slowly, with great patience.\"",
                    "<b>Naeve:</b> (very quietly) \"...Then my work was a comfort I told myself. A railing painted on a cliff.\"",
                    "<b>Maerin:</b> \"Maybe. But you painted it where people could see it and not be so afraid on the way " +
                    "down. Don't throw that out because it wasn't a wall. Not everything has to be a wall.\"" }
            },
            new Exchange {
                id = "roen_maerin", aMatch = "Roen", bMatch = "Maerin",
                title = "Roen & Maerin — the man who plans exits and the woman who had none",
                lines = new[] {
                    "<b>Roen:</b> \"I case every room for the way out. Always have. You... you went somewhere there wasn't one.\"",
                    "<b>Maerin:</b> \"There wasn't. That's the thing nobody tells you about the worst rooms. You stop " +
                    "looking for the door. You just learn the walls.\"",
                    "<b>Roen:</b> (he is quiet a moment, the patter gone) \"...Then I'll be the door. If it ever comes for " +
                    "you again. You don't have to learn the walls twice. Not while I'm standing.\"",
                    "<b>Maerin:</b> \"Careful, thief. Say a thing like that and mean it, and I'll hold you to it.\" (He nods. He means it.)" }
            },
            new Exchange {
                id = "ngplus_roen_garrow", aMatch = "Roen", bMatch = "Garrow",
                requiresFlag = "ng.plus",
                title = "Roen & Garrow — a draft they've read before",
                lines = new[] {
                    "<b>Roen:</b> \"Tell me you feel it too. This whole camp. I keep knowing what you're about to say a beat " +
                    "before you say it. I cased a hundred rooms; I've never walked into one I'd already *robbed.*\"",
                    "<b>Garrow:</b> \"I buried a man twice once. Exhumation, reburial — different graves, same hands, same " +
                    "words. It felt like this. Like grief with the serial numbers filed off.\"",
                    "<b>Roen:</b> \"...That's grim even for you. But yeah. That. Like we're a story somebody's *re-reading.*\"",
                    "<b>Garrow:</b> \"Then we give them a good one. If we've done this before, thief, let's do it kinder this " +
                    "time. That much, at least, I think we're allowed to change.\"" }
            },
            new Exchange {
                id = "roen_ilfaeril", aMatch = "Roen", bMatch = "Ilfaeril",
                title = "Roen & Ilfaeril — the young liar and the old judge",
                lines = new[] {
                    "<b>Roen:</b> \"Ten thousand years, and you never once just... *left?* Changed your name, picked a new town, " +
                    "let the old you be somebody else's problem? It's not even hard. I've done it six times.\"",
                    "<b>Ilfaeril:</b> \"That is the difference between us, thief. You change the name to escape the man. I keep " +
                    "the name because the man is the only sentence still being served for what he did. Run, and the dead I " +
                    "voted to unmake go unattended. Someone must stay and remember.\"",
                    "<b>Roen:</b> \"...Gods. And here I thought *I* was committed to a bit.\" (quieter) \"For what it's " +
                    "worth — I think the staying's braver than any con I ever pulled. Don't tell anyone I said the s-word.\"",
                    "<b>Ilfaeril:</b> (the ghost of a smile) \"Your secret, like all the rest you carry, is safe with the man " +
                    "who forgets nothing.\"" }
            },
            new Exchange {
                id = "varra_ilfaeril", aMatch = "Varra", bMatch = "Ilfaeril",
                title = "Varra & Ilfaeril — two old debts",
                lines = new[] {
                    "<b>Varra:</b> \"You and me, old man, we're the same trade. Both of us bound by a thing we signed before " +
                    "we knew better. Yours just has a nicer name. *Penance.*\"",
                    "<b>Ilfaeril:</b> \"There is a difference. Your debt was sold *to* you, by a power that lied. Mine I " +
                    "incurred freely, with a clear head, in a high clean room. Do not mistake my chains for yours. Yours, " +
                    "at least, you may be forgiven for.\"",
                    "<b>Varra:</b> \"...Huh. That's the first time anyone's told me my Hell-pact was the *good* kind of " +
                    "doomed.\" (She almost laughs.) \"You're terrible at comfort, you know that?\"",
                    "<b>Ilfaeril:</b> \"I had ten thousand years to practice and never once a reason. You are, I think, the " +
                    "first. Do not let it go to your head.\"" }
            },
            new Exchange {
                id = "varra_maerin", aMatch = "Varra", bMatch = "Maerin",
                title = "Varra & Maerin — the spent and the swallowed",
                lines = new[] {
                    "<b>Varra:</b> \"They put a price on me at six. They put *nothing* on you — just took you off the books " +
                    "entirely. I used to think mine was worse. I don't, anymore.\"",
                    "<b>Maerin:</b> \"Don't rank them. I did that, in there — ranked my suffering against everyone's, looking " +
                    "for a floor to stand on. There isn't one. There's just whether someone's still counting you. You are. " +
                    "That's the whole thing.\"",
                    "<b>Varra:</b> \"...Gods. Fine. New rule: we both stop doing math on it.\" (She offers the flask.) " +
                    "\"To being bad at arithmetic.\"",
                    "<b>Maerin:</b> (taking it) \"To still being on someone's list.\"" }
            },
            new Exchange {
                id = "garrow_maerin", aMatch = "Garrow", bMatch = "Maerin",
                title = "Garrow & Maerin — the gravedigger and the ungraved",
                lines = new[] {
                    "<b>Maerin:</b> \"You bury people. All your life. Did you ever bury one of... mine? One of the Faithless? " +
                    "Before you knew the Wall took them anyway?\"",
                    "<b>Garrow:</b> (a long silence) \"...Yes. A child, godless, whose mother begged me. I did the full rite. " +
                    "I knew it was air. I did it slow and I meant every word, because she was watching, and because the boy " +
                    "deserved one person to act as if he counted.\"",
                    "<b>Maerin:</b> \"That's the thing the Wall can't take, then. Not the soul. The *acting as if.* I felt it, " +
                    "in there — the ones who'd been grieved properly took longer to forget themselves.\"",
                    "<b>Garrow:</b> \"...Then I'll grieve you properly, here, while you can hear it. Sit. Let me practice on " +
                    "the living for once.\"" }
            },
            new Exchange {
                id = "naeve_roen", aMatch = "Naeve", bMatch = "Roen",
                title = "Naeve & Roen — the proof and the con",
                lines = new[] {
                    "<b>Roen:</b> \"You unsettle me, Arcanist. I read people for a living and I can't read you at all. It's " +
                    "like trying to pickpocket a statue.\"",
                    "<b>Naeve:</b> \"I spent a thousand years as the only mind in an empty proof. You learn to give nothing " +
                    "away when there is no one to give it *to.* You, by contrast, are a forgery so practiced you have " +
                    "forgotten you are also the original.\"",
                    "<b>Roen:</b> (genuinely startled) \"...Okay. That one landed. Remind me never to play cards with you.\"",
                    "<b>Naeve:</b> \"I would win, and you would enjoy losing, which is the only reason I would consent to play. " +
                    "Deal the cards, thief.\"" }
            },
            new Exchange {
                id = "naeve_colossus", aMatch = "Naeve", bMatch = "Roen",
                requiresFlag = "netheril.boss_down",
                title = "Naeve, after the Mythallar Colossus",
                lines = new[] {
                    "<b>Roen:</b> \"That war-construct in the falling city. The Colossus. You went quiet in a way I haven't seen " +
                    "off you. And you fight loud, Arcanist. That was a different quiet.\"",
                    "<b>Naeve:</b> \"My people built that. To guard a mythallar that was, that very hour, killing them. We made " +
                    "a perfect guardian for a treasure that was a bomb. There is no purer portrait of Netheril than that — " +
                    "flawless engineering in service of a catastrophe we were too proud to see.\"",
                    "<b>Roen:</b> \"You smashed it pretty thoroughly, for a portrait of home.\"",
                    "<b>Naeve:</b> \"It was already a tomb, thief. I only closed the lid. (a long pause) Do you know what I felt, " +
                    "watching it fall a second time? Relief. That this time, *I chose it.* The first time the sky just took " +
                    "everything. This time I had a hand on the wheel, even if the wheel only steered toward the same grief.\"" }
            },
            new Exchange {
                id = "ilfaeril_first_unmade", aMatch = "Ilfaeril", bMatch = "Garrow",
                requiresFlag = "crownwars.boss_down",
                title = "Ilfaeril, after the First Unmade",
                lines = new[] {
                    "<b>Garrow:</b> (gently, for her) \"That one in the Crown Wars. The First Unmade. You knew it. Knew its " +
                    "*name.* I saw your hand on your blade before it ever rose.\"",
                    "<b>Ilfaeril:</b> \"I voted to erase that soul. Ten thousand years ago, from the third bench, I raised my " +
                    "hand and called it mercy. And tonight I had to raise a blade against the grief I made of it. There is a " +
                    "word for being forced to kill the same person twice, gravedigger. I have never found it. I have looked.\"",
                    "<b>Garrow:</b> \"There isn't one. Some weights don't get a word. They just get *carried.* (a beat) You " +
                    "didn't flinch, at the end. You did it clean. That's not nothing — that's the mercy you owed it the first time.\"",
                    "<b>Ilfaeril:</b> \"...Is that what that was. I could not tell, through the shaking. Thank you. I will " +
                    "try to believe you. It is the only funeral rite I have left that I have not yet performed for myself.\"" }
            },
            new Exchange {
                id = "herald_witness", aMatch = "Ilfaeril", bMatch = "Naeve",
                requiresFlag = "spellplague.herald_down",
                title = "Ilfaeril & Naeve, after the Herald of the Unmade",
                lines = new[] {
                    "<b>Ilfaeril:</b> \"The Herald. It was grief given a shape and a sword. Ten thousand years of the erased, " +
                    "compressed into one screaming want. I have spent those same ten thousand years carrying my piece of that " +
                    "guilt quietly. To see it walk, and rage, and have to *cut it down*...\"",
                    "<b>Naeve:</b> \"It is the thing we both made, in our different ways. You with a vote. Me with a spell that " +
                    "fell a sky. The Unmade is just our arithmetic, still running, refusing to round down to zero.\"",
                    "<b>Ilfaeril:</b> \"And the Returned put it down so that the rest might rest. Not killed it — there is no " +
                    "killing grief. *Quieted* it. For now.\"",
                    "<b>Naeve:</b> \"For now is all anyone gets, elf. I ended a world believing I could buy *forever* with one " +
                    "perfect equation. Now I would trade every theorem I ever proved for a single reliable *for now.* The " +
                    "Returned keeps handing me them. I have stopped being too proud to take them.\"" }
            },
            new Exchange {
                id = "garrow_avatar", aMatch = "Garrow", bMatch = "Naeve",
                requiresFlag = "toot.avatar_down",
                title = "Garrow, after the Avatar of Bone",
                lines = new[] {
                    "<b>Naeve:</b> \"That thing at the forge. The Avatar of Bone. It wore the shape of Myrkul — the god your " +
                    "Kelemvor *replaced.* You put it down without a word. I'd have expected... I don't know. Reverence. Horror.\"",
                    "<b>Garrow:</b> \"Myrkul was a tyrant who hoarded the dead like coin. Kelemvor took the office to make it " +
                    "*kinder* and the office made him hard instead. Watching a scrap of the old god rise at that forge, I " +
                    "didn't feel horror. I felt — tired. Like watching a debt you thought was paid come back with interest.\"",
                    "<b>Naeve:</b> \"You serve the institution that did that. That hardened a kind man into a jailer.\"",
                    "<b>Garrow:</b> \"I serve the *dead,* Arcanist. The institution is just the cart I carry them in. And lately " +
                    "—\" (she looks at you) \"—I've been wondering if the Returned isn't here to teach us all a better way to " +
                    "carry them. Don't tell my order I said that. They'd call it heresy. They'd be right.\"" }
            },
            new Exchange {
                id = "varra_informant", aMatch = "Varra", bMatch = "Roen",
                requiresFlag = "safehouse.informant_freed",
                title = "Varra, on a frightened woman set loose",
                lines = new[] {
                    "<b>Varra:</b> \"That informant. You cut her loose instead of turning her. (She's not quite looking at " +
                    "you.) You know what they offered me, the first time, at six? A way out. Power. A leash so fine I called " +
                    "it a gift for fifteen years. Nobody ever once just... opened the door and let me decide.\"",
                    "<b>Roen:</b> \"She might run straight back to the Fist, Varra. Probably will.\"",
                    "<b>Varra:</b> \"Maybe. But it'll be *hers* to run. (A long breath.) That's the whole thing nobody " +
                    "understands about a leash, thief. It's not the collar that breaks you. It's that they never even let you " +
                    "*choose* the collar. You gave her the choice. I don't care what she does with it. I care that she has it.\"",
                    "<b>Roen:</b> (gently) \"...You'd have run back, would you? Six-year-old you, given the door?\"",
                    "<b>Varra:</b> \"Gods, no. I'd have walked out so fast. That's why it matters. Some of them walk.\"" }
            },
            new Exchange {
                id = "ilfaeril_informant", aMatch = "Ilfaeril", bMatch = "Roen",
                requiresFlag = "safehouse.informant_turned",
                title = "Ilfaeril, on a frightened woman turned",
                lines = new[] {
                    "<b>Ilfaeril:</b> \"You leaned on that informant until she broke and called it sound practice. I want you " +
                    "to hear how reasonable it was. Every word. Because *that* — that exact reasonableness — is the sound the " +
                    "Crown Wars court made, the day we voted a people into the Wall. We were not cruel. We were *practical.*\"",
                    "<b>Roen:</b> \"It's one snitch, old man, not a genocide.\"",
                    "<b>Ilfaeril:</b> \"It is always one snitch. One vote. One reasonable decision, and then the next, and " +
                    "ten thousand years later you cannot find the place where it became a Wall. I am not condemning you. I am " +
                    "the last man alive entitled to. I am only... asking you to *count* the small ones. I did not, and look at me.\"",
                    "<b>Roen:</b> (after a long beat) \"...Yeah. Alright. I'll count.\"" }
            },
            new Exchange {
                id = "maerin_ferryman", aMatch = "Maerin", bMatch = "Garrow",
                requiresFlag = "docks.ferryman_saved",
                title = "Maerin, on a stranger pulled from the water",
                lines = new[] {
                    "<b>Maerin:</b> \"You went in after that old man. Into the river. For a stranger who had nothing to give " +
                    "you. (She's looking at you like she's solving something.) That's — that's the *exact* thing. That's what " +
                    "you did for me, in the Breach. I've been trying to understand why anyone would. I think I just watched the answer.\"",
                    "<b>Garrow:</b> \"And the answer is?\"",
                    "<b>Maerin:</b> \"There isn't one. That's the answer. He doesn't do it *because.* He just doesn't have a " +
                    "version of himself that walks past the drowning. Neither did whoever pulled me. It's not a reason. It's a *shape.*\"",
                    "<b>Garrow:</b> (quietly) \"...That's the most accurate thing anyone's said about the Returned. Write it on the wall. " +
                    "It belongs with the names.\"" }
            },
            new Exchange {
                id = "garrow_deathbed", aMatch = "Garrow", bMatch = "Naeve",
                requiresFlag = "almshouse.deathbed_lie",
                title = "Garrow, on a kind lie to the dying",
                lines = new[] {
                    "<b>Naeve:</b> \"You told that dying man his son forgave him. It was false. I watched you do it without a " +
                    "flicker. I have been trying to reconcile it with the woman who will not lie about the weather.\"",
                    "<b>Garrow:</b> \"I've given ten thousand people their last rite. The words are mostly false too — I told " +
                    "you that. The dying don't need *accurate.* They need a hand and a story to hold while the dark comes up.\"",
                    "<b>Naeve:</b> \"So truth is a luxury of the living.\"",
                    "<b>Garrow:</b> \"Truth is a tool *for* the living — to fix things, to do better. You can't fix a thing " +
                    "with one breath left. So you give them the other thing. Comfort. It's not a lie if you'd die to make it " +
                    "true. And for that old man, in that moment? I would have.\"" }
            },
            new Exchange {
                id = "roen_urchin", aMatch = "Roen", bMatch = "Garrow",
                requiresFlag = "market.urchin_freed",
                title = "Roen, on a child bought back from the Fist",
                lines = new[] {
                    "<b>Roen:</b> (unusually quiet) \"That kid today. The pickpocket. You stood surety, got her loose. You " +
                    "know that was me, yeah? Thirty years ago, give or take a finger. Nobody stood surety for *me.* I just " +
                    "got faster.\"",
                    "<b>Garrow:</b> \"And here you are, with all your fingers, vouched for by a dead woman with a heart of " +
                    "grave-dirt. The wheel turns. Sometimes it turns *kind.*\"",
                    "<b>Roen:</b> \"...I'm going to find that kid again. Slip her a few coins, a name to ask for if the Fist " +
                    "leans on her. Pay it back up the line. Don't tell anyone. Ruins the brand.\"",
                    "<b>Garrow:</b> (dry) \"Your secret's in a grave only I dig. It's safe.\"" }
            },
            new Exchange {
                id = "quarter_pledge", aMatch = "Roen", bMatch = "Ilfaeril",
                requiresFlag = "lowcity.allies",
                title = "Roen & Ilfaeril — what the poor remember",
                lines = new[] {
                    "<b>Roen:</b> \"You see them at the doors when we pass? The Lower City. They don't cheer — too smart for " +
                    "that. They just... nod. Like they've decided something about us and aren't going to argue it.\"",
                    "<b>Ilfaeril:</b> \"They have decided you treated them as though they counted. It is a rarer verdict than " +
                    "any high court ever rendered, and it does not get overturned. The poor keep the truest records in the " +
                    "Realms — written nowhere, forgotten never.\"",
                    "<b>Roen:</b> \"I came up out of that gutter, old man. I spent twenty years making sure nobody could place " +
                    "the accent. And now they're nodding at me like I'm one of theirs after all.\"",
                    "<b>Ilfaeril:</b> \"You are. You never stopped being. The only thing that changed is that you came back " +
                    "for them. Do not undervalue it. I have ten thousand years of evidence that almost no one does.\"" }
            },

            // ---- Grief at the fire — only if a particular companion was taken at the Breach. ----
            new Exchange {
                id = "grief_garrow", aMatch = "Roen", bMatch = "Ilfaeril",
                requiresFlag = "companion.garrow.lost",
                title = "Roen & Ilfaeril — the empty place at the fire",
                lines = new[] {
                    "<b>Roen:</b> \"She'd have hated this. Us, maudlin, over her. She'd have said the hole's the same size " +
                    "whoever you put in it and gone back to sharpening that blade.\"",
                    "<b>Ilfaeril:</b> \"She kept a list of every soul she buried. I find I cannot stop thinking that no one " +
                    "kept one for her. That the gravedigger went ungraved.\"",
                    "<b>Roen:</b> (after a long silence) \"...Then we keep it. The list. Her name at the top, in a hand that " +
                    "doesn't shake. She taught us how. Least we can do is the one thing she was best at.\"",
                    "<b>Ilfaeril:</b> \"...Yes. Pass me the book, thief. I forget nothing. I will not start with her.\"" }
            },
            new Exchange {
                id = "grief_varra", aMatch = "Garrow", bMatch = "Roen",
                requiresFlag = "companion.varra.lost",
                title = "Garrow & Roen — the receipt no one got to burn",
                lines = new[] {
                    "<b>Roen:</b> \"She spent her whole life as a line item. And the Wall took her as a *tithe.* Collected " +
                    "the bill before we ever got to tear it up. Tell me that's not the cruelest joke you ever heard.\"",
                    "<b>Garrow:</b> \"I have buried the bought and the freed alike. The hole is the same size. I used to find " +
                    "that comforting.\" (a beat) \"I do not, tonight.\"",
                    "<b>Roen:</b> \"She left grinning. Course she did. I'll hear that laugh in every quiet room I'm ever in.\"",
                    "<b>Garrow:</b> \"Then she is not gone from those rooms. The dead stay as long as someone keeps the door " +
                    "open for them. Keep yours open, thief. That's the whole of the rite. The rest is just words.\"" }
            },
            new Exchange {
                id = "grief_roen", aMatch = "Garrow", bMatch = "Naeve",
                requiresFlag = "companion.roen.lost",
                title = "Garrow & Naeve — the door he left open",
                lines = new[] {
                    "<b>Naeve:</b> \"He cased every room for the exit. And in the end he walked through the one door with " +
                    "no way back, and did not even reach for a joke. I have replayed it four thousand times. It does not improve.\"",
                    "<b>Garrow:</b> \"He stopped building exits into his sentences, near the end. With us. With the Returned. " +
                    "A man like that, choosing to stay — that's not a small thing, Arcanist. That's the whole of him.\"",
                    "<b>Naeve:</b> \"...He once told me I court disaster but always buy it a drink first. I never bought him one.\"",
                    "<b>Garrow:</b> \"So buy it now. Pour two. He'll have left a tab open somewhere — he always did. " +
                    "We drink the thief out of the red. It's the only funeral he'd have laughed at.\"" }
            },
            new Exchange {
                id = "grief_naeve", aMatch = "Varra", bMatch = "Ilfaeril",
                requiresFlag = "companion.naeve.lost",
                title = "Varra & Ilfaeril — the unfinished proof",
                lines = new[] {
                    "<b>Varra:</b> \"She ended one world by accident and got unmade trying to be worth the second one. " +
                    "There's a symmetry to it she'd have appreciated and I absolutely hate.\"",
                    "<b>Ilfaeril:</b> \"She carried ten thousand years of grief as cleanly as I carry mine, and she was barely " +
                    "a thousand into it. I had thought, selfishly, that we would be old together. However that word applies to us.\"",
                    "<b>Varra:</b> \"She left a proof half-finished. I found it. I can't read a word of it. Doesn't matter. " +
                    "I'm keeping the slate. Some things you hold onto *because* you can't solve them. She taught me that, near the end.\"",
                    "<b>Ilfaeril:</b> \"...Then it is not unfinished. It is *ongoing.* She would insist on the distinction. " +
                    "Let us, for her, insist on it too.\"" }
            },
            new Exchange {
                id = "naeve_teases_varra", aMatch = "Naeve", bMatch = "Varra",
                requiresFlag = "romance.varra.turn",
                title = "Naeve, on a certain romance",
                lines = new[] {
                    "<b>Naeve:</b> \"You have begun courting a woman whose soul is collateral on an infernal contract. From a " +
                    "risk-management standpoint this is the single worst position I have ever modelled, and I ended a civilization.\"",
                    "<b>Varra:</b> \"Romantic. You should write the cards.\"",
                    "<b>Naeve:</b> \"I am not finished. I ran it forty thousand ways. In every branch where the Returned " +
                    "withdraws, the expected suffering is *lower* — and in every branch the Returned chooses you anyway. I " +
                    "have concluded the model is missing a variable. I believe the variable is *want*. I find it fascinating.\"",
                    "<b>Varra:</b> (genuinely thrown) \"...Did you just math your way into telling me I'm worth the risk?\"",
                    "<b>Naeve:</b> \"I would never be so imprecise. I said the *Returned* thinks so. The arithmetic merely declined to disagree.\"" }
            },
            new Exchange {
                id = "ilfaeril_maerin", aMatch = "Ilfaeril", bMatch = "Maerin",
                title = "Ilfaeril & Maerin — the builder and the swallowed",
                lines = new[] {
                    "<b>Ilfaeril:</b> \"Child. I helped build the thing that took you. I will not insult you by asking " +
                    "forgiveness. I only wanted you to know that I know it.\"",
                    "<b>Maerin:</b> (she considers him a long moment, this ancient, ruined man) \"...You came back to the " +
                    "edge of it for ten thousand years. Guiding people past. I felt it, in there. A hand at the dark.\"",
                    "<b>Maerin:</b> \"That wasn't nothing. It wasn't enough. But it wasn't nothing.\"",
                    "<b>Ilfaeril:</b> (an old man, weeping without sound) \"...No. I suppose it wasn't.\"" }
            },
        };

        /// <summary>The best banter to play now: the first whose pair is both fielded, gate passes, and
        /// hasn't fired yet. Returns null if nothing's eligible.</summary>
        public static Exchange Best()
        {
            var party = Party.Instance;
            var f = GameFlags.Current;
            if (party == null) return null;

            foreach (var e in All)
            {
                if (f.GetBool($"camp.banter.{e.id}.done")) continue;
                if (!string.IsNullOrEmpty(e.requiresFlag) && !f.GetBool(e.requiresFlag)) continue;
                if (Present(party, e.aMatch) && Present(party, e.bMatch)) return e;
            }
            return null;
        }

        private static bool Present(Party party, string nameMatch)
        {
            foreach (var m in party.active)
                if (m?.displayName != null && m.displayName.Contains(nameMatch)) return true;
            return false;
        }
    }
}
