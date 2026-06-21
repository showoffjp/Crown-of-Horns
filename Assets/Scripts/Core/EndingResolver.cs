using System.Collections.Generic;

namespace SunderedCrown.Core
{
    public enum Ending { Abolition, DoomguidesPeace, JergalsKeyhole, ReturnedThrone, BreakTheLoop, MortalMeasure }

    /// <summary>
    /// The finale brain. Reads the whole playthrough out of GameFlags to decide which endings the
    /// player has *earned the right to choose* (the deeper truths gate the deeper endings), supplies
    /// each ending's prose, and assembles the BG2-style epilogue slides keyed to who lived, who was
    /// lost, the Verdict, the Lady's riddle, and Aldric's fate. Pure logic — no Unity. (Prose
    /// condensed from docs/story/17_ENDINGS_PROSE.md.)
    /// </summary>
    public static class EndingResolver
    {
        /// <summary>Which endings are available, given what the player understood and did.</summary>
        public static List<Ending> Available()
        {
            var f = GameFlags.Current;
            var list = new List<Ending> { Ending.Abolition, Ending.ReturnedThrone, Ending.MortalMeasure };

            if (f.GetInt("faction.kelemvor.reputation") >= 5 || f.GetBool("aldric.named_monster"))
                list.Insert(1, Ending.DoomguidesPeace);

            // The golden road needs understanding (you read the Lady) AND proof the Wall is a *decision*.
            if (f.GetBool("readers_boon") && f.GetBool("crownwars.verdict_spared"))
                list.Add(Ending.JergalsKeyhole);

            // Breaking the loop needs you to have grasped the loop at all.
            if (f.GetBool("readers_boon") || f.GetBool("pc.true_name"))
                list.Add(Ending.BreakTheLoop);

            return list;
        }

        public static bool IsGolden(Ending e) => e == Ending.JergalsKeyhole || e == Ending.BreakTheLoop;

        public static string Title(Ending e) => e switch
        {
            Ending.Abolition       => "🜍 Abolition-by-Atrocity — \"The Mercy That Ate the World\"",
            Ending.DoomguidesPeace => "⚖️ The Doomguide's Peace — \"The Atrocity Kept\"",
            Ending.JergalsKeyhole  => "🗝️ Jergal's Keyhole — \"The Tedious Mercy\"  (golden)",
            Ending.ReturnedThrone  => "👑 The Returned Throne — \"The New Tyrant, Maybe Merciful\"",
            Ending.BreakTheLoop    => "🕯️ Break the Loop — \"Stay Gone\"  (the existential ending)",
            Ending.MortalMeasure   => "🌾 Mortal Measure — \"The Small Good\"",
            _ => "—"
        };

        public static string Choice(Ending e) => e switch
        {
            Ending.Abolition       => "Say YES to the Unmade. Pull the first thread. Save every discarded soul — at any cost.",
            Ending.DoomguidesPeace => "Side with Vayle. Keep the Wall. Defend the oldest cruelty because the alternatives are worse.",
            Ending.JergalsKeyhole  => "Take up Jergal's pen. Rewrite the Law by hand, soul by soul, forever.",
            Ending.ReturnedThrone  => "Take the Crown of Horns yourself. Become the god of the dead.",
            Ending.BreakTheLoop    => "Refuse the Unmade AND refuse to become the Last Returned. Say your true name. Stay gone.",
            Ending.MortalMeasure   => "Refuse all of it. Walk away. Spend your borrowed time on the small human good.",
            _ => "—"
        };

        public static string Prose(Ending e) => e switch
        {
            Ending.Abolition =>
                "You pull the thread, and across every age at once the Wall unravels — every discarded soul freed, " +
                "Maerin among them, whole, forever. And the bill comes due exactly as promised: death itself unbinds, " +
                "the living pay in oceans, and time knots shut into a single deathless Now. You take your seat at the " +
                "centre of the most beautiful tomb in creation. You got everything you wanted. That is the tragedy.",
            Ending.DoomguidesPeace =>
                "You keep the Wall. Aldric falls; the Unmade is suppressed, not answered; death's order endures. And you " +
                "live with having looked at the oldest cruelty in the multiverse, understood it completely, and defended " +
                "it — because the alternatives were worse, and you were not wrong, which is the heaviest thing of all.",
            Ending.JergalsKeyhole =>
                "You neither tear the Wall down nor keep it — you *rewrite* it. Jergal, delighted past ten thousand years " +
                "of boredom, hands you the pen. The Unmade, seen and corrected at last, chooses peace. The Wall comes down " +
                "soul by soul, *judged* instead of dissolved. It will take you the rest of eternity. You take the job. " +
                "Heaven exhales for the first time in ten thousand years.",
            Ending.ReturnedThrone =>
                "You seize the Crown and become the god of the dead — and exactly the thing you watched everyone become. " +
                "A merciful new judge, or the seed of the next catastrophe a future Returned must walk back and stop. Either " +
                "way, the wheel has a new face. Yours.",
            Ending.BreakTheLoop =>
                "You lower your weapon. You say your true name. You do the one thing the loop has never seen: you choose the " +
                "ending. You step into the niche that has worn your name since before the world was young — not to dissolve, " +
                "but to *rest* — and the Unmade, hearing its own first name spoken willingly, with love, finds the one thing " +
                "it never had: an outside. A witness. A place to set the grief down. The machine of sorrow stops. There will " +
                "be no next Returned. You did that — for a stranger you'll never meet, in a world you'll never see.",
            Ending.MortalMeasure =>
                "You walk away from the end of the world and go home. The Wall stands; the Unmade waits; the great wound goes " +
                "unhealed. And you spend your borrowed time on the people within arm's reach — a wedding, a fed child, one " +
                "good season — present and kind and real. The world is not fixed. You mattered anyway.",
            _ => "—"
        };

        /// <summary>BG2-style epilogue slides — the per-companion / faction / choice 'where they are now'.</summary>
        public static List<string> Epilogue(Ending e)
        {
            var f = GameFlags.Current;
            var slides = new List<string>();

            slides.Add(CompanionSlide("garrow", "Sister Garrow", e, f, present: !f.GetBool("companion.garrow.lost")));
            slides.Add(CompanionSlide("roen", "Roen Alleywind", e, f, present: f.GetBool("companion.roen.recruited") && !f.GetBool("companion.roen.lost")));
            slides.Add(CompanionSlide("varra", "Varra", e, f, present: f.GetBool("companion.varra.recruited") && !f.GetBool("companion.varra.lost")));
            slides.Add(CompanionSlide("naeve", "Naeve", e, f, present: f.GetBool("companion.naeve.recruited") && !f.GetBool("companion.naeve.lost")));
            slides.Add(CompanionSlide("ilfaeril", "Ilfaeril", e, f, present: f.GetBool("companion.ilfaeril.recruited") && !f.GetBool("companion.ilfaeril.lost")));
            slides.Add(CompanionSlide("maerin", "Maerin", e, f, present: f.GetBool("companion.maerin.recruited") && !f.GetBool("companion.maerin.lost")));

            // Roen's quest — "The Honest Lie." The call you made at the safehouse echoes here.
            if (f.GetBool("quest.roen.resolved") && f.GetBool("companion.roen.recruited") && !f.GetBool("companion.roen.lost"))
            {
                if (f.GetBool("quest.roen.double_agent"))
                    slides.Add("🕸️ Wrenna Alleywind — the Harpers never learned she'd turned, because you turned her back: a blade in Kelemvor's own house. She and Roen send word twice a year, in a cipher only the two of them and you can read.");
                else if (f.GetBool("quest.roen.wrenna_saved"))
                    slides.Add("🏠 Wrenna Alleywind — you let Roen choose mercy over the rules, and he never forgot it. The Alleywinds keep a safehouse with two cold teacups on the mantel — one for the sister who lied to save him, one, always, for you.");
                else if (f.GetBool("quest.roen.harper_boon"))
                    slides.Add("⚖️ Wrenna Alleywind — she turned herself in, and the Harpers got their traitor and their routes both. It was the correct call. Roen made it with you and has not quite met your eyes since. Correct is not the same as forgiven.");
            }

            // Varra's quest — "The Bill Comes Due." Who paid the contract follows her to the end.
            if (f.GetBool("quest.varra.resolved") && f.GetBool("companion.varra.recruited") && !f.GetBool("companion.varra.lost"))
            {
                if (f.GetBool("quest.varra.patron_bound"))
                    slides.Add("📜 Varra — you found the patron's true name in the seventh seal and turned her leash into theirs. She walks the Realms a free woman with a Hell in her debt, and she never once stops grinning about it.");
                else if (f.GetBool("quest.varra.debt_taken"))
                    slides.Add("🔥 Varra — you put your name where hers had been since she was six. She sleeps now, for the first time in twenty years, and spends her days hunting the loophole that will get you both off the books. She will not stop until she does.");
                else if (f.GetBool("quest.varra.freed"))
                    slides.Add("🔥 Varra — she burned the contract with her own violet fire: still owed, still doomed, but by her own hand and on her own clock. 'You can't imagine how light that is,' she said — and meant it to the last.");
            }

            // Garrow's quest — "A God-Shaped Hole." How she answered the Scales follows her to the end.
            if (f.GetBool("quest.garrow.resolved") && !f.GetBool("companion.garrow.lost"))
            {
                if (f.GetBool("quest.garrow.doctrine_won"))
                    slides.Add("⚖️ Sister Garrow — she put Kelemvor's own canon on trial and won, and took the Doom back from the cowards who'd stopped reading it. She weighs the unclaimed now — the souls the Wall used to discard — and is, whether the church admits it yet or not, the first High Doomguide of a kinder Law.");
                else if (f.GetBool("quest.garrow.left_faith"))
                    slides.Add("🕯️ Sister Garrow — she set the grey clasp on the empty soul-pan and walked out from under thirty years. She lays the dead for anyone now, plainly, under no law but her own two hands — Faithless, unafraid, and lighter than you ever saw her in the church's keeping.");
                else if (f.GetBool("quest.garrow.recanted"))
                    slides.Add("🪦 Sister Garrow — she knelt and kept the grey, and does the work from inside: at the bedside when the priests won't come. It is a smaller faith now, and a harder one, carried brittle — but carried. She never once forgot that you stayed at her shoulder when the verdict was already written.");
            }

            // Naeve's quest — "After the Sky Fell." What she did with the last of Netheril follows her home.
            if (f.GetBool("quest.naeve.resolved") && f.GetBool("companion.naeve.recruited") && !f.GetBool("companion.naeve.lost"))
            {
                if (f.GetBool("quest.naeve.rekindled"))
                    slides.Add("✨ Naeve — she opened her own hand and let the last live Weave go not down but out, a seed instead of a shroud. Netheril's final magic feeds the present world's recovery, and she teaches, now, an arithmetic that builds: her people ended as the thing that makes new wonders possible.");
                else if (f.GetBool("quest.naeve.released"))
                    slides.Add("🌌 Naeve — she lifted her hand and let the last of Netheril finish its thousand-year fall. She carries the grief instead of the wreckage now — some mistakes you don't erase, you carry — and she stopped, at last, trying to redo the proof of her own life.");
                else if (f.GetBool("quest.naeve.preserved"))
                    slides.Add("❄️ Naeve — she sealed the shard in stasis: the last of Netheril, whole and frozen, a snowflake caught forever mid-fall. She visits the tomb she calls a home and does not pretend, anymore, that clutching it was the same as saving it. Not yet ready to let the sky finish. Perhaps one day.");
            }

            // Ilfaeril's quest — "The Vote." What he did with Maerith's forgiveness follows him to the end.
            if (f.GetBool("quest.ilfaeril.resolved") && f.GetBool("companion.ilfaeril.recruited") && !f.GetBool("companion.ilfaeril.lost"))
            {
                if (f.GetBool("quest.ilfaeril.commission"))
                    slides.Add("⚔️ Ilfaeril — he heard Maerith's forgiveness for what it was: not a door, but a sword held hilt-first. He spends what's left of him tearing a hole in the Wall for the rest of her people — forgiveness that puts a man back to work. Ten thousand years a penitent; now, at last, a paladin again.");
                else if (f.GetBool("quest.ilfaeril.forgiven"))
                    slides.Add("🕊️ Ilfaeril — he did the hardest thing a man who has done the unforgivable can do: he let himself be forgiven. The rigid ancient thing in him eased. He has not rested yet — but he believes, now, that one day he is allowed to, and for him that belief is a whole new world.");
                else if (f.GetBool("quest.ilfaeril.penance"))
                    slides.Add("🌒 Ilfaeril — he could not take what Maerith offered; the weight is the last true thing he has of her. He guides the lost past the Wall still, and will until the stars go out — devotion, and a kind of hiding, and he half knows it. He was glad, at least, not to refuse rest alone.");
            }

            // The cruelest stake: a companion you *loved* taken by the Wall at the Breach. The doc's most
            // devastating playthrough — the bill love ran up, collected before you could pay it down.
            foreach (var rid in new[] { "garrow", "roen", "naeve", "varra" })
            {
                if (!f.GetBool($"companion.{rid}.lost")) continue;
                if (!Loved(f, rid)) continue;
                string n = NameOf(rid);
                slides.Add(rid == "varra"
                    ? "⚰️💔 Varra & you — you loved her, and the Wall took her at the Breach as Maerin's tithe: the receipt collected before you ever got to burn it. She left grinning, because of course she did. You will hear that laugh in every quiet room for the rest of your borrowed life."
                    : $"⚰️💔 {n} & you — you loved them, and the Wall took them at the Breach, and there is no revival in this story and no softening it. The hole where they stood is exactly the shape of everything you were just beginning to let yourself want.");
            }

            // Romances — the slow burns. Love here was never a reward; it was a stake. These land last,
            // because whoever you chose to love is the face the cosmos comes to rest on.
            if (f.GetBool("romance.garrow.consummated") && !f.GetBool("companion.garrow.lost"))
                slides.Add(e == Ending.BreakTheLoop
                    ? "🕯️💞 Garrow & you — she gave you the last rite, your own, weeping: \"I have given ten thousand people their rites and meant none of them like this. Go easy. I'll keep your name. I'll keep it forever. I promise — and I've never once meant a promise about the dead, until you.\""
                    : "💞 Garrow & you — the woman who buried everyone and believed in keeping no one learned, late and against all her training, to keep one person. She still tends the dead. But she comes home now, to a fire and a name she refuses to add to any list but the living one.");
            if (f.GetBool("romance.roen.consummated") && f.GetBool("companion.roen.recruited") && !f.GetBool("companion.roen.lost"))
                slides.Add(e == Ending.BreakTheLoop
                    ? "🕯️💞 Roen & you — at the niche he had no joke left, only your hand in his: \"I had a whole bit ready and I can't do the bit. I found you. In all of it, all the cycles, I found you once. That has to be enough. Go on. I've got you. I've got you.\""
                    : "💞 Roen & you — he got his 'after' after all. There is an inn with a terrible name and two chairs, and a retired Harper who cheats at cards and loses on purpose, and who still can't quite believe the menace stayed.");
            if (f.GetBool("romance.naeve.consummated") && f.GetBool("companion.naeve.recruited") && !f.GetBool("companion.naeve.lost"))
                slides.Add(e == Ending.JergalsKeyhole
                    ? "🗝️💞 Naeve & you — the golden ending's last image: the two of you at the Ledger, deriving the rewritten Law together. \"An infinite proof. One soul at a time. I can think of no better way to spend forever, and no one I would rather spend it not-solving with.\""
                    : "💞 Naeve & you — she revised her oldest axiom and let the universe be full. Two people who each ended a world decided they were allowed to be happy in spite of it, and spent their borrowed time proving, daily, that meaning was specifically, demonstrably, each other.");
            if (f.GetBool("romance.varra.consummated") && f.GetBool("companion.varra.recruited") && !f.GetBool("companion.varra.lost"))
                slides.Add("💞🔥 Varra & you — you burned the receipt together, the literal price of her soul gone to ash on the wind. Whatever she's worth now, she sets it herself, off the books, un-priceable — and she chose you freely, the first transaction of her life with no fine print at all.");

            // Aldric.
            if (f.GetBool("aldric.named_monster"))
                slides.Add("⚔️ Aldric Morn — you named him a monster to his face, and he agreed, and that was worse. He fell at the Court, his arithmetic unfinished, still believing he was late rather than wrong.");
            else if (f.GetBool("aldric.provisional"))
                slides.Add(e == Ending.JergalsKeyhole || e == Ending.BreakTheLoop
                    ? "☕ Aldric Morn — given the Wall's end without his crown, he wept, and then he laughed, and then he went home to tend graves: no longer a god, finally a father who buried his child."
                    : "☕ Aldric Morn — he never got his daughter back the way he wanted. He carries the melt of a wedding ring on a cord, and the count of every life, to the end of his days.");

            // The Mythallar Colossus — the optional Netheril miniboss.
            if (f.GetBool("netheril.boss_down"))
                slides.Add("⚙️ The Mythallar Colossus — ten heartbeats from the ground, you fought a war-construct still guarding a city that was already dead, and put down the last order Netheril ever gave. Naeve watched it fall with a grief no equation could hold. \"It was only following the command,\" she said. \"As were we all.\"");

            // The First Unmade — the optional Crown Wars miniboss.
            if (f.GetBool("crownwars.boss_down"))
                slides.Add("🌫️ The First Unmade — you met the very first soul the court voted to erase, ten thousand years of ungraved grief given just enough shape to ask *why* — and you laid it to rest where the Wall began. Ilfaeril knelt by it a long time. \"The first of mine,\" he said. \"Let it be the first I ever helped set down.\"");

            // The Avatar of Bone — the optional Time of Troubles miniboss.
            if (f.GetBool("toot.avatar_down"))
                slides.Add("💀 The Avatar of Bone — at the forge where a god's skull was beaten into the Crown, a shard of dying divinity rose to guard the work, and you put it down. Garrow stood over it a long while afterward. \"Even gods,\" she said. \"Even gods get buried. I find that the most comforting thing I have ever seen.\"");

            // The Herald of the Unmade — the optional Spellplague miniboss.
            if (f.GetBool("spellplague.herald_down"))
                slides.Add("🜍 The Herald of the Unmade — in the one place the Unmade comes closest to winning, you met its herald in the blue fire and put it down. It was not the war, only a skirmish in it — but the Unmade learned, that day, that the Returned can be met *and held.* That knowledge cost it something it could not name.");

            // Garrow, who watched death's own god made mortal in the Time of Troubles.
            if (f.GetBool("toot.garrow_witnessed") && !f.GetBool("companion.garrow.lost"))
                slides.Add("⚰️ Garrow, at the forge — she watched a god of death beaten into a crown of horns, and learned in the worst possible way the thing her whole quest circled: the Wall, the Doom, the judgement — none of it was handed down. People made it. And what people made, you taught her, people can refuse.");

            // Varra, who walked the Spellplague where every contract's rule came apart.
            if (f.GetBool("spellplague.varra_witnessed") && f.GetBool("companion.varra.recruited") && !f.GetBool("companion.varra.lost"))
                slides.Add("🔥 Varra, in the blue fire — she stood where causality forgets to come before effect, her pact-flame feral and free, and chose, every time, not to vanish into it and escape her bill. She came back out. For you. Off the books, of her own free will — the only currency she ever trusted.");

            // Naeve, returned to the Enclave the hour it fell — if she walked it with you.
            if (f.GetBool("netheril.naeve_witnessed") && f.GetBool("companion.naeve.recruited") && !f.GetBool("companion.naeve.lost"))
                slides.Add("🌌 Naeve, on the balcony — she walked the Seventh Enclave in the warm living hour before it fell, among the faces she'd spent a thousand years failing to grieve, and this time she did not fall alone. Whatever the proof of her life, she got to stand in it once more — breathing, witnessed, home.");

            // Ilfaeril, returned to the court that damned them — if he stood there with you.
            if (f.GetBool("crownwars.ilfaeril_witnessed") && f.GetBool("companion.ilfaeril.recruited") && !f.GetBool("companion.ilfaeril.lost"))
                slides.Add("🕯️ Ilfaeril, at the bench — he went back to the third bench from the dais, where he raised the hand that helped damn a people, and he did not look away, because you stood close enough that he didn't have to look away alone. Ten thousand years of nightmare, witnessed at last by someone who stayed.");

            // The Verdict (Crown Wars).
            if (f.GetBool("crownwars.verdict_spared"))
                slides.Add("🌫️ The Crown-War damned — because you argued one ancient court out of its cruelty, a single valley of souls was spared the Wall ten thousand years early. Ilfaeril wept to see it. It changed one thing. One thing is not nothing.");
            else if (f.GetBool("crownwars.verdict_passed"))
                slides.Add("🌫️ The Crown-War damned — the verdict passed, as it always had. The chamber felt colder than the High Lord expected. History did not record the hour. History only inherited it.");

            // The Lower City — Act II standing. The small good you did (or didn't) echoes at the end.
            int lowcity = f.GetInt("reputation.lowcity");
            if (f.GetBool("lowcity.allies") || lowcity >= 5)
                slides.Add("🏙️ The Lower City — the poor and the godless of the Gate never forgot the Returned who treated them like they mattered. When the end came, they were there — not with armies, with bread and lanterns and a hundred small refusals to look away. It is the only kind of help that was ever really yours, and it was enough.");
            else if (lowcity <= -2)
                slides.Add("🏙️ The Lower City — the Gate's forgotten remember you, too: as one more power that passed through and spent them like coin. No curses. Just the old, patient silence the poor keep for people who were never going to save them anyway.");

            // Roen, back in the kind of room that made him.
            if (f.GetBool("almshouse.roen_witnessed") && f.GetBool("companion.roen.recruited") && !f.GetBool("companion.roen.lost"))
                slides.Add("🗝️ Roen, in the Almshouse — the Outer City orphan stood in the kind of room he was pulled out of, among the people he could have been, and for once didn't reach for a joke or a lockpick. He came out of the gutter; he never forgot it; and in the end he chose, with you, not to become one more power that walked past it.");

            // The Almshouse of the Unclaimed — if Mother Cass sent you to the Court with the poor's token.
            if (f.GetBool("almshouse.token"))
                slides.Add("🪙 Mother Cass's token — you carried the poor's backwards Kelemvor token all the way to the Court, the scales turned inward. When the things that judge convened, the unclaimed of the Gate had, for once, someone in the room who'd promised to say they were real. You said it. It mattered that you said it.");

            // The Last Letter — a dying soldier's forty-year wrong.
            if (f.GetBool("quest.letter.delivered"))
                slides.Add("✉️ Old Davyn's letter — you put the truth in his victim's daughter's hand, and she learned both the worst of one man and the late courage of another. Hard gifts, both. She keeps the letter in a drawer she opens more than she'd admit.");
            else if (f.GetBool("quest.letter.read"))
                slides.Add("✉️ Old Davyn — you read his confession back to him, aloud, once, and a coward of forty years' standing got to hear himself be honest before the end. Whatever became of the page, that hearing was the absolution he'd never asked for.");
            else if (f.GetBool("quest.letter.burned"))
                slides.Add("✉️ Old Davyn's letter — you burned it, and a daughter kept her father a hero, and an old man took the truth down into the dark where it could not cut her. A kindness, or a cowardice, or both. He died believing it was mercy.");

            // The Sinking Skiff — a small mercy on an ordinary afternoon.
            if (f.GetBool("docks.ferryman_saved"))
                slides.Add("🛶 Old Pell, the ferryman's passenger — lived twelve more years and told everyone the Returned walked on water to save him. You didn't. But the story did more good than the truth would have, and on the docks they still toast the dead one who went in cold for a stranger nobody else would.");
            else if (f.GetBool("docks.ferryman_resolved"))
                slides.Add("🛶 Old Pell — lived, no thanks to you; another dockhand went in where you would not. He never knew you'd walked past. But you knew. It is a small weight, as your weights go. You carry it anyway, in the place you keep the afternoons you can't take back.");

            // The bound informant — turned, or given a choice.
            if (f.GetBool("safehouse.informant_freed"))
                slides.Add("🕸️ The Harper snitch you freed — turned, in the end, but on her own terms, and ran the Fist's dockside network into the ground for the lodge that once would have broken her. She tells new recruits there's one Returned who treated her like she had a choice, and that it was the choice, not the mercy, that turned her. The Harpers are still arguing about what that means.");
            else if (f.GetBool("safehouse.informant_resolved"))
                slides.Add("🕸️ The Harper snitch you turned — spied well and hated you the whole while, and a family paid for a name she gave up under the leaning. The work got done. It always does. You learned, watching her face close, exactly how the Wall gets built: one reasonable decision at a time, by people who were not wrong.");

            // Old Hensley's deathbed — the kindest lie or the harder truth.
            if (f.GetBool("almshouse.deathbed_lie"))
                slides.Add("🕯️ Old Hensley — went out smiling, holding a forgiveness his son never sent because you placed it in his hands. You will never know if it was mercy or theft of the truth he was owed. Neither, you suspect, will the universe. Some kindnesses are only between you and a dying stranger, and stay there.");
            else if (f.GetBool("almshouse.deathbed_resolved"))
                slides.Add("🕯️ Old Hensley — died holding the truth instead of a comfort: that his son never came, but that he was still the kind of man who'd want to know. It was a harder thing to die holding. You gave it to him because it was *his.* You hope that was right. You will never be sure, and you have decided to be able to live with that.");

            // The pickpocket child — a finger, or a future.
            if (f.GetBool("market.urchin_freed"))
                slides.Add("🍎 The Quarter's pickpocket — the child you stood surety for grew up crooked-honest and sharp, running messages no one else would carry, and tells anyone who'll listen that a dead thing once bought her hand back from the Fist. She names her own first child after you. It is a small immortality. It is the only honest kind.");
            else if (f.GetBool("market.urchin_resolved"))
                slides.Add("🍎 The Quarter's pickpocket — the child you let the Fist take lost the finger and learned the lesson the city teaches its poor early: expect nothing. They are still out there, somewhere, good at not being caught and at nothing else. You taught them that, on an ordinary afternoon, by doing nothing at all.");

            // The Tithe Collector — grief made a marketplace.
            if (f.GetBool("quest.tithe.freed") || f.GetBool("quest.tithe.paid"))
                slides.Add("⚰️ The pauper's pit — because of you, the Gate's poorest dead were blessed and buried, free, the year the Returned passed through. It is a small mercy against the size of the Wall. But the people who left the flowers did not think it small.");
            else if (f.GetBool("quest.tithe.corrupt"))
                slides.Add("⚰️ The pauper's pit — you took a cut of what the grieving paid to bury their own, and the unblessed dead piled up at the edge of the city like a debt no one would call in. The Wall, when you finally saw it whole, looked a little like Vane's ledger.");

            // The Faithless Choir's militant cell — if you broke it.
            if (f.GetBool("quest.choir.cell_cleared"))
                slides.Add("⚔️ The Hollow Cantor — when the street preaching curdled into a knife-cell in the undercroft, you went down into the dark and broke it before it could make a martyr of anyone. The Lower City slept easier for it, and never quite knew why.");

            // The Faithless Choir — the Unmade's seed in the Lower City.
            if (f.GetBool("quest.choir.doubted"))
                slides.Add("🕯️ The Faithless Choir — the preacher you argued into doubt never did rebuild his flock. He took to sitting with the dying instead of promising them the Wall would fall. He says you taught him grief is something you carry, not something you weaponise.");
            else if (f.GetBool("quest.choir.favored"))
                slides.Add("🜍 The Faithless Choir — you spoke for the Unmade once, in the open, when it cost something, and the Choir never forgot. Whatever you chose at the Court, there were those who'd called you theirs long before the end — and felt it as betrayal or fulfilment, depending.");
            else if (f.GetBool("quest.choir.suppressed"))
                slides.Add("🜍 The Faithless Choir — you scattered them with truncheons, and the grief you silenced went underground and grew teeth. The Choir that surfaced later was harder, and quieter, and it remembered the Returned as just another warden of the Wall.");

            // The Doomguides of Kelemvor — where you stood with the church of the dead.
            if (f.GetInt("faction.kelemvor.reputation") >= 5)
                slides.Add("⚖️ The Doomguides of Kelemvor — the church that keeps the Wall came, in the end, to call you not enemy but *question* — the one soul in an age it could neither damn nor dismiss. Vayle kept a chair empty at the Court for your argument. Whether you took it or tore the whole edifice down, the order will be arguing about the Returned for a thousand years. You made the keepers of the dead *think.* Almost no one ever has.");
            else if (f.GetInt("faction.kelemvor.reputation") <= -2)
                slides.Add("⚖️ The Doomguides of Kelemvor — to the church you remained what you started as: a crack in the order, a dead thing that threatened the Wall and traded in the dead's due. They were glad to see the back of you, and quietly afraid you'd be back. Doomguides have long memories. Yours is not a name they'll bless.");

            // The Faithless Choir — the Unmade's grief, and how it sang you.
            if (f.GetInt("faction.choir.reputation") >= 5)
                slides.Add("🜍 The Faithless Choir — you spoke for the Unmade when it cost you, and the grief of ten thousand years made you its prophet. The Choir sings your name still — in the undercrofts, in the pauper's pits, wherever the discarded gather. You gave the oldest sorrow in the multiverse the one thing it never had: an advocate the gods would let into the room. Whatever you chose at the Court, you were *heard,* and you made them hear.");

            // ── Side quests of the long road — the seeded hooks, paid off at the end. ──
            // "The Graves That Waited."
            if (f.GetBool("sq.field_of_the_rested") || f.GetBool("sq.graves_that_waited_complete"))
                slides.Add("⚰️ The Graves That Waited — you stayed at the coffin-man's field until the last hole was filled: every soul the harvest stole, buried at last, named, a true word over each. They will sing about the crown. But the field of the rested was the thing the crown was always for, and you knew it, and you did the unglamorous part.");
            else if (f.GetBool("sq.graves_to_the_tenders"))
                slides.Add("⚰️ The Graves That Waited — you called the unbroken line of grave-tenders to the coffin-man's field, and a thousand hands filled in an afternoon what one pair could not in an age. The Wall emptied its dead with an army of clerks. You gave them back with an army of the kind, and it was the better-shaped mercy.");
            else if (f.GetBool("sq.every_soul_expected") || f.GetBool("sq.halen_at_rest"))
                slides.Add("⚰️ The Graves That Waited — you carried the coffin-man's ledger back through the Wall, and no soul you freed was released into nowhere: each came out to a grave already dug, a stone already carved, a place that had been waiting ten thousand years. The Wall said you were nothing, unexpected. The open holes said otherwise.");

            // "The Hand in the Margins" — the loop's letter chain.
            if (f.GetBool("sq.margins_for_the_warden"))
                slides.Add("📜 The Hand in the Margins — you carried your own ten-thousand-year warning to the warden who thought the letters had abandoned him, and showed him he was never the failure at the end of the chain, only the draft that made the final warning clear enough to work. Even the tiredest hand got to be one the correspondence was trying to save.");
            else if (f.GetBool("sq.wrote_back_to_the_loop"))
                slides.Add("📜 The Hand in the Margins — for ten thousand years the loop wrote to you, the same tired hand scratching *stop* into the margins of every catastrophe. You were the first who could write back. The crack in your soul went quiet — not silent, but answered — and the correspondence, at last, ran both ways.");
            else if (f.GetBool("sq.margins_warning_heeded"))
                slides.Add("📜 The Hand in the Margins — every loop before you wrote the warning and reached for the crown anyway. You were the one who finally read it and obeyed: not a stronger Returned, a better-warned one, arriving at the Court already carrying the antidote in your own hand. The chain wrote for an age hoping for one reader who'd listen. It got you.");

            // "The Thing in the Dark" — Roen's buried secret.
            if (f.GetBool("sq.roen_and_sabira_reconcile"))
                slides.Add("🗡️ Roen & Old Sabira — you stood in the room while two people who loved and damaged each other finally said the true thing in daylight, and found out love survives it. The fence who raised a child into the dark, and the child who carried her sin as his name, unlocked the room between them at last — and neither had to be alone in it.");
            else if (f.GetBool("sq.roen_tells_the_fire"))
                slides.Add("🗡️ Roen — named the thing in the dark at the fire, once, plain, as a fact and not a confession, and watched the company not leave. The killing at fourteen, half a life carried as a whole identity, shrank to its true size the moment it was shared: a hard thing that happened to a cornered child, and not the name under all his names.");
            else if (f.GetBool("sq.roen_forgives_sabira"))
                slides.Add("🗡️ Roen — set down the grudge he'd carried as long as the guilt, because the Exile's lesson finally landed: forgiveness is a weight you release for your own sake, not a gift the debtor earns. He forgave the woman who saved and damned him in the same hand, and came back lighter than you had ever seen him.");

            // "The Indictment" — Naeve's case against Netheril.
            if (f.GetBool("sq.naeve_grieves_at_last"))
                slides.Add("✨ Naeve — set the indictment down and let herself, for the first time, simply miss her father. She stopped prosecuting the man who showed her the stars and chose his prestige over her life, and started mourning him — beautiful and guilty and gone and hers — which turned out to be a closer kind of keeping than any case.");
            else if (f.GetBool("sq.naeve_keeps_one_page"))
                slides.Add("✨ Naeve — sent the indictment to do its justice and kept one page back: not a vote, not a charge, the one with the stars and the boat. The guilt went to court; the love came home with her. Whatever the record proved about Lord Aubrey the murderer, his daughter always carried the proof of Lord Aubrey the father.");
            else if (f.GetBool("sq.naeve_indictment_at_court"))
                slides.Add("✨ Naeve — read the indictment of Netheril where it finally had to land: not at a dead empire but before the powers that kept doing what its lords did. Her father's signature damned every authority that ever chose its order over a child. She made the grief count for more than a broken heart.");

            // "The Hands That Refused" — the harvest's turncoats.
            if (f.GetBool("sq.harvest_exposed_public"))
                slides.Add("⚔️ The Hands That Refused — a deserter and a singer nailed the harvest's own flowchart where the Gate read over breakfast, and the city looked, for once, at the whole machine at once. You cannot un-ring that bell. Box by box, hand by hand, the ordinary people who fed the Wall began to step out of their places, and the looking would not stop.");
            else if (f.GetBool("sq.harvest_to_the_hands"))
                slides.Add("⚔️ The Hands That Refused — Corwin and Wren took their testimony soldier to soldier, singer to singer, quietly, into the garrisons and Choir-houses, where it could not be spun into a martyrdom. The harvest unmade the way it was made: one cracking hand at a time, in the dark, passed hand to hand. Slower than a rebellion. Impossible to crush.");
            else if (f.GetBool("sq.harvest_to_the_court"))
                slides.Add("⚔️ The Hands That Refused — a Fist deserter and a Choir convert testified before the powers of the dead themselves: that the Wall is fed by ordinary hands, theirs among them, and that theirs opened — which meant every hand that fed it always could. The machine's whole defense was *we had no choice.* They were the living disproof, at the one court that cannot adjourn.");

            // "The Forty-One" — the reaper's hidden souls.
            if (f.GetBool("sq.fortyone_reaper_rests"))
                slides.Add("⚱️ The Last Honest Psychopomp — when the forty-one souls he'd hidden ten thousand years were finally home, the ferryman of endings learned he was owed a shore too, and lay down in the field beside the ones he saved. The coffin-man dug him a good hole, reaper-shaped, among the Faithless he'd refused to let the Wall have.");
            else if (f.GetBool("sq.fortyone_reaper_joins_line"))
                slides.Add("⚱️ The Last Honest Psychopomp — with his forty-one freed, the first reaper to refuse the doctrine went back to the grey to stand between the Wall and the next fading soul, and the next. His cloak never stayed empty. It was never meant to. He had found the unbroken line, and learned he'd been one more hand in it all along.");
            else if (f.GetBool("sq.fortyone_ask_them") || f.GetBool("sq.fortyone_to_the_graves") || f.GetBool("sq.fortyone_gentle_wall"))
                slides.Add("⚱️ The Forty-One — the souls a reaper hid for ten thousand years against the Wall's pull got, at last, an ending instead of an endless keeping: named, chosen, set down. He learned the hardest lesson of his long office — that even mercy curdles into a cage, and that the ones we love are never ours to hold, only ours to carry until they can choose.");

            // "The Forbidden Name" — Wickless's candle.
            if (f.GetBool("sq.forbidden_name_spoken") || f.GetBool("sq.wickless_speaks_alone"))
                slides.Add("🕯️ Wickless — the candle a frightened child lit for the person they weren't allowed to talk about carried a forbidden name through ten thousand years of grey, found the faded soul it was lit for, and said it. Her light caught like a wick taking flame. A whole family's silence broke on one small voice that refused it: proof that a name carried in love is a soul the Wall can never quite finish erasing.");

            // "The First Dirge" — Singer Lhoris's encoded song.
            if (f.GetBool("sq.dirge_becomes_anthem"))
                slides.Add("🎶 The First Dirge — you taught the oldest song in the world to everyone who'd refused to let the forgotten go: the grave-tenders, the list-keepers, the Drowned Choir who learned they'd been singing its second melody all along. Sung wide, by a world that finally knew what it was singing, the names woven into ten thousand years of funerals became the one thing the Wall could never file or forbid.");
            else if (f.GetBool("sq.dirge_sung_at_court"))
                slides.Add("🎶 The First Dirge — you carried Lhoris's masterwork to the Court and sang its second song where it landed hardest: out of the solemn, approved melody the powers had heard at every sorting and believed was a blessing rose the names of every soul they'd walled. You cannot out-argue a dirge that is also a list of your victims. They had no refutation, and the oldest song in the world sang the Faithless home.");
            else if (f.GetBool("sq.dirge_freed_to_change"))
                slides.Add("🎶 The First Dirge — you left the song open, as Lhoris built it to be: a dirge any singer in any century can turn one more word kinder, until the melody that once sealed the Faithless into the Wall becomes the one that overturns it. A Wall is a fact that needs no one and so can never change. The song stays young — always one singer away from kinder — and outlasts every power that tried to fix its meaning.");
            else if (f.GetBool("sq.dirge_names_restored"))
                slides.Add("🎶 The First Dirge — you lifted the names Lhoris had woven into the song's bones and carried them home: out of the intervals that had held them ten thousand years and to the graves, the reading, the field of the rested. The dirge sings its surface now, lighter, emptied at last of the secret it kept until someone came who could do what a melody could not — let the names rest.");
            else if (f.GetBool("sq.dirge_decoded"))
                slides.Add("🎶 The First Dirge — you heard the second song in the oldest dirge in the world and sang it out: ten thousand years of funerals stopped being a blessing on the Wall and became what Lhoris always made them — the longest act of remembrance in history, the names of the Faithless smuggled past every power that wanted them gone, finally read aloud.");

            // The Lady.
            if (f.GetBool("readers_boon"))
                slides.Add("🎭 The Lady in the Margins — you read her, after all. Whatever else the ending took, she remembers it — keeps it, in the white space, in a hand you almost know — so that, this once, your story was *witnessed.* The margin smiled.");

            // New Game+ — the loop, acknowledged at the very end.
            if (f.GetBool("ng.plus"))
                slides.Add("🌀 And somewhere outside the page, the Lady closes the book on this telling and reaches, already, for the first chapter again — because you came this way before, and will come again, and each time the ink runs a little kinder. She does not call it hope. She is too old for the word. But she keeps re-reading, and that is the same thing wearing a quieter coat.");

            // Keepsakes — the small proofs you carried into the Court.
            int kept = SunderedCrown.Content.Keepsakes.Earned().Count;
            if (kept > 0)
                slides.Add($"🎒 What you carried — you walked to the end of the world with your pockets full of small proofs that people had let you matter to them: {kept} keepsakes, from a gravedigger's whetstone to things harder to name. It is not armour. It was never armour. It is the better thing the armour was always protecting.");

            // The anchor — the face the whole cosmos comes to rest on. Whoever you chose to love (or, lacking
            // that, whoever you let in closest) is who the ending is finally *about.*
            string anchor = AnchorId();
            if (anchor != null)
            {
                string n = NameOf(anchor);
                if (f.GetBool($"companion.{anchor}.lost"))
                    slides.Add($"⚓ And in the last accounting, the saga was about {n} — the one you loved and lost. Every choice after the Breach was made in the shape of that absence. The universe ended, or began, around a hole with their name on it.");
                else if (f.GetBool($"companion.{anchor}.left"))
                    slides.Add($"⚓ And in the last accounting, the saga was about {n} — the one you drove away. Wherever they are, they never learned how it ended, or that, when it mattered most, the whole cosmos turned on the space they used to fill.");
                else
                    slides.Add($"⚓ And in the last accounting, the saga was about {n} — the face you chose to love. Whatever the ending was, it was *theirs* as much as yours; the cosmos came to rest, finally, on someone specific, which is the only scale that ever mattered.");
            }

            slides.RemoveAll(string.IsNullOrEmpty);
            return slides;
        }

        /// <summary>"The Chronicle of the Returned" — a spoiler-free running tally of the whole playthrough,
        /// read straight from the flags. Shown on the ending screen and from the anytime Chronicle (C).</summary>
        public static List<string> Chronicle()
        {
            var f = GameFlags.Current;
            var lines = new List<string>();

            // Eras walked.
            var eras = new List<string>();
            if (f.GetBool("netheril.cleared")) eras.Add("Netheril");
            if (f.GetBool("crownwars.cleared")) eras.Add("the Crown Wars");
            if (f.GetBool("act4.toot_done")) eras.Add("the Time of Troubles");
            if (f.GetBool("act4.spellplague_done")) eras.Add("the Spellplague");
            lines.Add(eras.Count > 0
                ? "🗺️ Eras walked: " + string.Join(", ", eras)
                : "🗺️ Eras walked: none yet — the rifts still wait in the hub.");

            // Companions — present, lost, departed.
            var with = new List<string>(); var gone = new List<string>(); var left = new List<string>();
            foreach (var id in new[] { "garrow", "roen", "varra", "naeve", "ilfaeril", "maerin" })
            {
                bool inRun = id == "garrow" || f.GetBool($"companion.{id}.recruited");
                if (!inRun) continue;
                if (f.GetBool($"companion.{id}.lost")) gone.Add(NameOf(id));
                else if (f.GetBool($"companion.{id}.left")) left.Add(NameOf(id));
                else with.Add(NameOf(id));
            }
            if (with.Count > 0) lines.Add("🛡️ Still at your side: " + string.Join(", ", with));
            if (gone.Count > 0) lines.Add("⚰️ Taken by the Wall: " + string.Join(", ", gone));
            if (left.Count > 0) lines.Add("🚪 Walked away: " + string.Join(", ", left));

            // Personal quests resolved.
            int quests = 0;
            foreach (var id in new[] { "roen", "varra", "garrow", "naeve", "ilfaeril" })
                if (f.GetBool($"quest.{id}.resolved")) quests++;
            lines.Add($"📜 Companion quests resolved: {quests}/5");

            // Side quests of the long road brought home (one per quest, any resolution counts).
            var sideQuestFlags = new[] {
                new[] { "sq.field_of_the_rested", "sq.graves_to_the_tenders", "sq.every_soul_expected", "sq.halen_at_rest" },
                new[] { "sq.margins_for_the_warden", "sq.wrote_back_to_the_loop", "sq.margins_warning_heeded", "sq.margins_back_to_ynn" },
                new[] { "sq.roen_and_sabira_reconcile", "sq.roen_tells_the_fire", "sq.roen_forgives_sabira", "sq.roen_darkthing_rests" },
                new[] { "sq.naeve_grieves_at_last", "sq.naeve_keeps_one_page", "sq.naeve_indictment_at_court", "sq.naeve_read_together" },
                new[] { "sq.harvest_exposed_public", "sq.harvest_to_the_hands", "sq.harvest_to_the_court" },
                new[] { "sq.fortyone_reaper_rests", "sq.fortyone_reaper_joins_line", "sq.fortyone_ask_them", "sq.fortyone_to_the_graves", "sq.fortyone_gentle_wall", "sq.fortyone_victory" },
                new[] { "sq.forbidden_name_spoken", "sq.wickless_speaks_alone" },
                new[] { "sq.dirge_becomes_anthem", "sq.dirge_sung_at_court", "sq.dirge_freed_to_change", "sq.dirge_names_restored", "sq.dirge_decoded" },
            };
            int sideDone = 0;
            foreach (var group in sideQuestFlags) { foreach (var k in group) if (f.GetBool(k)) { sideDone++; break; } }
            if (sideDone > 0) lines.Add($"🪙 Side quests of the long road: {sideDone}/8 brought home");

            // Bonds.
            var loves = new List<string>();
            foreach (var id in Romanceable) if (Loved(f, id)) loves.Add(NameOf(id) + (f.GetBool($"romance.{id}.consummated") ? " ♥" : ""));
            if (loves.Count > 0) lines.Add("💞 Hearts given: " + string.Join(", ", loves));
            int broken = 0;
            foreach (var id in new[] { "garrow", "roen", "varra", "naeve", "ilfaeril" }) if (f.GetBool($"rupture.{id}.broken")) broken++;
            if (broken > 0) lines.Add($"💔 Bonds broken past mending: {broken}");

            // Fireside moments shared (night-talks + group banters heard).
            int fireside = 0;
            foreach (var kv in f.bools)
                if (kv.Value && kv.Key.EndsWith(".done") &&
                    (kv.Key.StartsWith("camp.banter.") || kv.Key.StartsWith("camp.nighttalk.")))
                    fireside++;
            if (fireside > 0) lines.Add($"🔥 Fireside moments shared: {fireside}");

            // The Lower City (Act II).
            int lowcity = f.GetInt("reputation.lowcity");
            if (lowcity != 0 || f.GetBool("quest.widow.resolved") || f.GetBool("quest.fist.resolved"))
                lines.Add($"🏙️ Lower City standing: {lowcity}" + (f.GetBool("lowcity.allies") ? " — the quarter stands with you" : ""));
            if (f.GetBool("quest.choir.resolved"))
                lines.Add("🕯️ The Faithless Choir: " + (f.GetBool("quest.choir.doubted") ? "given doubt" : f.GetBool("quest.choir.favored") ? "spoken for" : "silenced")
                    + (f.GetBool("quest.choir.cell_cleared") ? " · undercroft cell broken" : ""));
            if (f.GetBool("quest.tithe.resolved"))
                lines.Add("⚰️ The grave-tithe: " + (f.GetBool("quest.tithe.corrupt") ? "took a cut" : "the poor's dead rest free"));
            if (f.GetBool("almshouse.token"))
                lines.Add("🪙 The Almshouse: you carry the unclaimed's token to the Court");

            // The Lady & the Verdict.
            int riddles = f.GetInt("riddle.solvedCount");
            if (riddles > 0 || f.GetBool("readers_boon"))
                lines.Add($"🎭 The Lady: {riddles}/11 riddles solved" + (f.GetBool("readers_boon") ? ", her name read" : ""));
            if (f.GetBool("crownwars.verdict_spared")) lines.Add("⚖️ The Crown-War verdict: argued DOWN — a valley of souls spared.");
            else if (f.GetBool("crownwars.verdict_passed")) lines.Add("⚖️ The Crown-War verdict: passed, as it always had.");

            // Standing with the Realms' powers (non-zero only).
            foreach (var (flag, name) in new[] {
                ("faction.kelemvor.reputation", "Kelemvor's Doomguides"),
                ("faction.choir.reputation", "the Faithless Choir"),
                ("faction.ashpact.reputation", "the Ash Pact") })
            {
                int r = f.GetInt(flag);
                if (r != 0) lines.Add($"⚖️ Standing — {name}: {r:+0;-0}");
            }

            // Battles won this run.
            int wins = f.GetInt("combat.victories");
            if (wins > 0) lines.Add($"⚔️ Battles won: {wins}");

            // Deeds earned.
            lines.Add($"🏆 Deeds earned: {SunderedCrown.Content.Deeds.EarnedCount()}/{SunderedCrown.Content.Deeds.Total}");

            // Endings unlocked + difficulty.
            lines.Add($"👑 Endings unlocked: {Available().Count}/6" + (Available().Exists(IsGolden) ? "  ★ a golden road is open" : ""));
            lines.Add($"🎚️ Difficulty: {GameSettings.Mode}");

            return lines;
        }

        private static readonly string[] Romanceable = { "garrow", "roen", "naeve", "varra" };

        /// <summary>Did the player commit to this companion romantically (past the Turn)?</summary>
        private static bool Loved(GameFlags f, string id) =>
            f.GetBool($"romance.{id}.turn") || f.GetBool($"romance.{id}.choosing") || f.GetBool($"romance.{id}.consummated");

        private static string NameOf(string id) => id switch
        {
            "garrow" => "Sister Garrow", "roen" => "Roen", "naeve" => "Naeve", "varra" => "Varra",
            "ilfaeril" => "Ilfaeril", "maerin" => "Maerin", _ => id,
        };

        /// <summary>The finale anchor — the face the cosmos rests on. The one you loved (deepest commitment
        /// wins), or, lacking a romance, the companion you let in closest by approval.</summary>
        public static string AnchorId()
        {
            var f = GameFlags.Current;
            // 1) A romance, by depth of commitment.
            foreach (var key in new[] { "consummated", "choosing", "turn" })
                foreach (var id in Romanceable)
                    if (f.GetBool($"romance.{id}.{key}")) return id;

            // 2) Else the highest-approval companion who was actually part of the run.
            string best = null; int bestA = int.MinValue;
            foreach (var id in new[] { "garrow", "roen", "varra", "naeve", "ilfaeril", "maerin" })
            {
                bool inRun = id == "garrow" || f.GetBool($"companion.{id}.recruited");
                if (!inRun) continue;
                int a = f.GetInt($"companion.{id}.approval");
                if (a > bestA) { bestA = a; best = id; }
            }
            return bestA >= 20 ? best : null; // only if you let *someone* in
        }

        private static string CompanionSlide(string id, string name, Ending e, GameFlags f, bool present)
        {
            if (f.GetBool($"companion.{id}.lost"))
                return $"⚰️ {name} — taken by the Wall as the tithe for Maerin, and never returned. There is no revival in this story. The hole where they stood never filled.";
            if (f.GetBool($"companion.{id}.left"))
                return $"🚪 {name} — walked out of the company the night the bond finally frayed past mending, and did not come to the Court. Somewhere out there they are still living the life your choices drove them to. You'll never know how it ended for them. That is its own kind of loss.";
            if (!present) return null; // never recruited / not in this run

            // Present-at-the-end flavour, tinted by the ending.
            return e switch
            {
                Ending.BreakTheLoop => $"🕯️ {name} — stood with you at the niche, and spoke your name so it would never again be forgotten.",
                Ending.JergalsKeyhole => $"🗝️ {name} — kept you company at the Ledger, on the first of an eternity of nights spent judging the dead by hand.",
                Ending.Abolition => $"🜍 {name} — smiles, in the Deathless Garden, forever. The same smile. The same instant. Loved into a padded eternity, never allowed to finish the sentence.",
                Ending.MortalMeasure => $"🌾 {name} — sat at the small table on an ordinary evening, alive, while outside the window the long dark went patient and unremarked.",
                _ => $"🛡️ {name} — walked out of the story changed, and carried what it cost for the rest of their days."
            };
        }
    }
}
