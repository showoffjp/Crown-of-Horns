#!/usr/bin/env python3
"""
make-town-market.py — build play/town_market.html: a single, self-contained, fully playable
scene. You walk a character around a market square (click-to-move on an isometric board, just
like the combat sim) and approach THREE named NPCs — Mother Sable the charm-seller, Sergeant
Bram the recruiter, and Pip the street-child — each with a deep, BG3-style character-reactive
conversation. The NPCs read your race, class, background, alignment, deity, and stats and answer
differently; skill checks roll d20 + ability modifier + proficiency vs the DC, with an animated die.

Pure resolution logic lives in /*<MKT>*/...*/</MKT>*/ so town_market.test.js can lift and verify it.
Re-run:  python3 tools/make-town-market.py
"""
import json, os

ROOT = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
DEMO = json.load(open(os.path.join(ROOT, "play", "dialogue-demo.json"), encoding="utf-8"))
MKT = json.load(open(os.path.join(ROOT, "play", "town-market.json"), encoding="utf-8"))
REED = json.load(open(os.path.join(ROOT, "play", "reed-walk.json"), encoding="utf-8"))
UNDER = json.load(open(os.path.join(ROOT, "play", "underbridge.json"), encoding="utf-8"))
LASTTORCH = json.load(open(os.path.join(ROOT, "play", "lasttorch.json"), encoding="utf-8"))
LAMPLIT = json.load(open(os.path.join(ROOT, "play", "lamplit.json"), encoding="utf-8"))
COUNTHOUSE = json.load(open(os.path.join(ROOT, "play", "counthouse.json"), encoding="utf-8"))
HEARTH = json.load(open(os.path.join(ROOT, "play", "hearth.json"), encoding="utf-8"))
ALDRIC = json.load(open(os.path.join(ROOT, "play", "aldric.json"), encoding="utf-8"))
WAYSHRINE = json.load(open(os.path.join(ROOT, "play", "wayshrine.json"), encoding="utf-8"))
THRESHOLD = json.load(open(os.path.join(ROOT, "play", "threshold.json"), encoding="utf-8"))
NIGHTMARKET = json.load(open(os.path.join(ROOT, "play", "nightmarket.json"), encoding="utf-8"))
VAULT = json.load(open(os.path.join(ROOT, "play", "vault.json"), encoding="utf-8"))
COURT = json.load(open(os.path.join(ROOT, "play", "court.json"), encoding="utf-8"))
EPILOGUE = json.load(open(os.path.join(ROOT, "play", "epilogue.json"), encoding="utf-8"))
WEEPING = json.load(open(os.path.join(ROOT, "play", "weepinghouse.json"), encoding="utf-8"))
CISTERN = json.load(open(os.path.join(ROOT, "play", "cistern.json"), encoding="utf-8"))
CARTOGRAPHER = json.load(open(os.path.join(ROOT, "play", "cartographer.json"), encoding="utf-8"))
TRIAL = json.load(open(os.path.join(ROOT, "play", "trial.json"), encoding="utf-8"))
SCRIVENER = json.load(open(os.path.join(ROOT, "play", "scrivener.json"), encoding="utf-8"))
ARCHIVE = json.load(open(os.path.join(ROOT, "play", "archive.json"), encoding="utf-8"))
ADVOCATE = json.load(open(os.path.join(ROOT, "play", "advocate.json"), encoding="utf-8"))
ASSIZE = json.load(open(os.path.join(ROOT, "play", "assize.json"), encoding="utf-8"))
SIEGE = json.load(open(os.path.join(ROOT, "play", "siege.json"), encoding="utf-8"))
WAKE = json.load(open(os.path.join(ROOT, "play", "wake.json"), encoding="utf-8"))
INQUEST = json.load(open(os.path.join(ROOT, "play", "inquest.json"), encoding="utf-8"))
THINPLACE = json.load(open(os.path.join(ROOT, "play", "thinplace.json"), encoding="utf-8"))
DISPUTATION = json.load(open(os.path.join(ROOT, "play", "disputation.json"), encoding="utf-8"))
CONFESSION = json.load(open(os.path.join(ROOT, "play", "confession.json"), encoding="utf-8"))
BOAST = json.load(open(os.path.join(ROOT, "play", "boast.json"), encoding="utf-8"))
CANTOR = json.load(open(os.path.join(ROOT, "play", "cantor.json"), encoding="utf-8"))
MEMORY = json.load(open(os.path.join(ROOT, "play", "memory.json"), encoding="utf-8"))
PREDATOR = json.load(open(os.path.join(ROOT, "play", "predator.json"), encoding="utf-8"))
CUSTODY = json.load(open(os.path.join(ROOT, "play", "custody.json"), encoding="utf-8"))
MODEL = DEMO["characterModel"]

BUILDS = [
    {"name": "The Returned", "cls": "Fighter", "scores": [16, 14, 15, 10, 12, 13],
     "race": "Human", "gender": "Male", "background": "Soldier", "law": "Lawful", "morality": "Neutral", "deity": "Kelemvor",
     "blurb": "A plain human soldier of the Judge."},
    {"name": "The Scholar", "cls": "Wizard", "scores": [10, 14, 12, 16, 14, 11],
     "race": "Half-Elf", "gender": "Female", "background": "Sage", "law": "Neutral", "morality": "Good", "deity": "Oghma",
     "blurb": "Half-elf sage — sharp INSIGHT & lore."},
    {"name": "The Confessor", "cls": "Cleric", "scores": [13, 10, 14, 11, 17, 12],
     "race": "Human", "gender": "Female", "background": "Acolyte", "law": "Lawful", "morality": "Good", "deity": "Kelemvor",
     "blurb": "A cleric of Kelemvor — the best WISDOM."},
    {"name": "The Silver Tongue", "cls": "Rogue", "scores": [10, 16, 12, 13, 11, 16],
     "race": "Tiefling", "gender": "Male", "background": "Charlatan", "law": "Chaotic", "morality": "Neutral", "deity": "Tymora",
     "blurb": "A tiefling charlatan — the best CHARISMA."},
    {"name": "The Warden", "cls": "Ranger", "scores": [14, 16, 13, 11, 14, 10],
     "race": "Elf", "gender": "Female", "background": "Folk Hero", "law": "Neutral", "morality": "Neutral", "deity": "None",
     "blurb": "An elf who serves no god — one of the Faithless."},
]

INT_LABELS = {
    "market.sable.regard": "Mother Sable's regard",
    "market.bram.regard": "Sergeant Bram's regard",
    "market.pip.regard": "Pip's trust in you",
    "market.wren.regard": "Wren's trust in you",
    "reed.cassian.regard": "Old Cassian's regard",
    "reed.vharn.regard": "Sister Vharn's regard",
    "reed.wren.regard": "Wren's trust in you",
    "reed.reedwife.regard": "the Reed-Wife's regard",
    "under.pip.regard": "Pip's trust in you",
    "under.wick.regard": "Wick's regard",
    "under.knotwife.regard": "the Knotwife's regard",
    "lt.hale.regard": "Brother Hale's regard",
    "lt.esuele.regard": "Esuele's regard",
    "lt.goodwin.regard": "Goodwin's regard",
    "city.mab.regard": "Mab's regard",
    "city.dace.approval": "Dace's approval",
    "city.pell.regard": "Pell's regard",
    "ch.crake.regard": "Old Crake's regard",
    "ch.tallow.regard": "Goodman Tallow's regard",
    "ch.mereth.regard": "Canon Mereth's regard",
    "hearth.dace.bond": "Dace — your bond",
    "hearth.wren.regard": "Wren's regard",
    "hearth.pip.regard": "Pip's trust in you",
    "hearth.sennet.regard": "Sennet — at the fire",
    "hearth.sparrow.regard": "the Sparrow — at the fire",
    "ald.aldric.regard": "Aldric Morn's regard",
    "ald.wessa.regard": "Wessa's regard",
    "ald.eithne.regard": "Eithne's regard",
    "way.harbinger.regard": "the Harbinger's regard",
    "way.orin.regard": "Justiciar Orin's regard",
    "way.doget.regard": "Goodman Doget's regard",
    "thr.last.regard": "the Last Returned's regard",
    "thr.dace.regard": "Dace — at the edge",
    "thr.orin.regard": "Orin — at the edge",
    "thr.sennet.regard": "Sennet — at the edge",
    "thr.sparrow.regard": "the Sparrow — at the door",
    "epi.sparrow.regard": "the Sparrow — years on",
    "nm.pawn.regard": "the Pawn of Hours' regard",
    "nm.mnemo.regard": "the Lost-and-Found's regard",
    "nm.regular.regard": "the Regular's regard",
    "nm.years_given": "years you've traded away",
    "vault.warden.regard": "the Warden of Tens' regard",
    "vault.pedant.regard": "the Petrified Pedant's regard",
    "vault.tithe.regard": "the Tenth's trust in you",
    "vault.riddle_won": "riddles bested",
    "court.kelemvor.regard": "Kelemvor's regard",
    "court.crown.regard": "the Crown's hold on you",
    "court.esuele.regard": "Esuele's regard",
    "epi.wren.regard": "Wren — years on",
    "epi.pip.regard": "Pip — years on",
    "epi.sennet.regard": "Sennet — years on",
    "epi.annet.regard": "Annet — years on",
    "epi.dace.years": "Dace — years on",
    "epi.orin.years": "Orin — years on",
    "wh.matron.regard": "the Lady of the House's regard",
    "wh.tam.regard": "Tam's trust in you",
    "wh.keeper.regard": "Goodwife Harl's regard",
    "ci.sedge.regard": "Mother Sedge's regard",
    "ci.berin.regard": "Old Berin's regard",
    "ci.gnaw.regard": "the Gnaw — drawn toward you",
    "cg.sennet.approval": "Sennet's approval",
    "cg.tibb.regard": "Tibb's regard",
    "cg.vael.regard": "Goodman Vael's regard",
}

# Lore glossary (shared with the dialogue sim) — common-knowledge hover + tiered 5e passive lore reveals.
GLOSSARY = json.load(open(os.path.join(ROOT, "play", "lore-glossary.json"), encoding="utf-8"))

# The Returned-sense: an eerie, on-theme perception the dead-touched player gets on first sight of each soul,
# if their Wisdom-clarity (10 + WIS mod) meets the moment's DC. It reveals what the living can't see.
NPC_SENSE = {
    "market.sable": {"dc": 12, "text": "*(Your dead-touched senses sweep her table and recoil — there is no holiness in a single bone or thread of it. And yet her hands, folding the knots, are warm with something realer than any relic: she has buried more of this ward than the gravediggers have, and grieved every one of them into the thread she sells. The fraud is the kindest thing in this market.)*"},
    "market.bram": {"dc": 11, "text": "*(The Wall left its chill in your marrow, and it answers the cold this man carries: three squads' worth of the recently, violently dead are pressed against his back like wet wool, patient, unblaming. He does not know they followed him home from the causeway. You do. They are not here for revenge. They are here to see whether he stops sending more.)*"},
    "market.pip": {"dc": 13, "text": "*(Some Returned see the almost-dead the way the living see candle-flames. The girl burns bright and quick and furious — but there is a second flame tethered to hers, fainter, somewhere close and low to the ground, and it is *guttering.* A child, smaller than this one. Sick past what a stolen copper mends. The thing she is really stealing for, running out of wick.)*"},
    "market.calix": {"dc": 12, "text": "*(Your dead-touched senses reach the young priest and recoil from what's already begun in him: the Wall's cold has crept up past his knees, grey and patient, the way frost takes a windowpane from the edges in. He has stood the causeway too long. A man becomes what he guards, if he guards it without love — and Calix has run clean out of love for his post. Another year of watching the Faithless go in, and he'll be standing on the wrong side of his own Wall, and not remember crossing.)*"},
    "market.wren": {"dc": 11, "text": "*(You don't have to look for the mark. It *finds* you — cold light pouring off her like breath off a winter river, a brand the harvesters laid on her soul that the living can't see and she can only feel. Days. Fewer than she's letting herself count. She is godless and gentle and entirely without sin, and she has been measured for a wall, and she is buying apples because the apples are sweet and the apples are *now.*)*"},
    "market.tallow": {"dc": 13, "text": "*(You reach for the smiling man's soul the way you'd reach into a coat you expect to be full — and your senses close on *almost nothing.* He has spent himself a coin at a time, a name at a time, year on year, until what's left would barely fill a thimble. He thinks his tithes and his service have bought him safety. They've only hollowed him faster. The Wall is not reaching for him later, with the rest. It is reaching for him *now*, and the cold has already found his fingers — he simply mistakes it for the draught off the river.)*"},
    "market.joss": {"dc": 10, "text": "*(You barely have to reach at all; the madman's soul is *standing open*, like a door left wide in winter. He has been to the edge of the Wall and pressed his face to it and listened, and a sliver of the cold came home in him and never left — the same sliver that lives in you. He is not raving at nothing. He is raving at the *exact* thing you are. Of every soul in this square, he is the one who will believe every word you'd never dare say aloud — because he already heard it, in the stone, in a voice he loved.)*"},
    "reed.cassian": {"dc": 11, "text": "*(The river's cold is your cold, and it answers the old boatman's like a tuning-fork: he is not quite among the living anymore. Forty years of ferrying has worn him thin as a coin rubbed smooth, half his soul already paid downstream to the water he serves. He does not know he is dying by inches into his own river. He thinks it is just the damp in his knees. The oar in his hands has rowed thirteen thousand souls to the mist, and the thirteen-thousand-and-first will be him, and he has no idea, and some part of him is so very tired that it would be a mercy.)*"},
    "reed.vharn": {"dc": 14, "text": "*(You reach for the Measurer's soul and find it walled — not hollow like Tallow's, but *fortified*, every doubt bricked up behind liturgy, a cell she built around her own conscience and threw away the key to. And yet the cold gets in. There, behind the third course of bricks: three hundred and eleven faces she will not let herself count, and one of them, freshest, has an apple in its hand. The wall around her heart is exactly as strong as the Wall she serves — which is to say, it holds against everyone but the one soul stubborn enough to keep saying a name.)*"},
    "reed.wren": {"dc": 10, "text": "*(The mark blazes off her even brighter here, this close to the stone — but your dead-touched sense reads something the brand can't hide: she is not ready. Whatever she tells herself, whatever brave arithmetic she's done in the cold water, the living animal of her is *clinging*, white-knuckled, to every small sweet now it can find — the apple, the cold on her ankles, the sound of your boots on the shingle. She came here to end it. Her own soul is screaming, in a voice only you can hear, that it is not time, it is not time, it is not time.)*"},
    "reed.reedwife": {"dc": 9, "text": "*(You don't reach for her; she is already the same cold as you, and recognition passes between you like current through water. Four hundred years she has abided in the doorway you walked all the way through — a soul that refused the Wall and let the river catch it on the threshold, neither in nor back, just *held*, in the green and the patience. She is lonelier than anything you have ever touched. She is also, you realise with a small private vertigo, what you might have been, had you flinched at the door. There but for the turning-around go you.)*"},
    "under.pip": {"dc": 11, "text": "*(The fierce small flame you saw in the market burns lower now — not from sickness, but from sheer exhaustion, days of standing a watch no nine-year-old should ever stand. Your dead-touched sense reads the truth she's hiding even from herself: she is not afraid that her brother will die. She has already, somewhere behind the fists, accepted it. What she is terrified of is the morning *after* — the first dawn in her whole life with no small reason to be careful, no one smaller than herself to keep fed, nothing left to steal *for.* The boy is her tether to the world as surely as she is his. Pull one loose and the other drifts.)*"},
    "under.wick": {"dc": 9, "text": "*(You don't have to reach. The little flame you felt all the way from the market is *here*, close enough to warm your dead hands if it had any warmth left to give, and it is so nearly out that looking at it directly feels like an intrusion. Days. Perhaps fewer. But your sense reads something else under the guttering, and it stops you cold: the boy is not *clinging* the way Wren clung at the water. He has already, quietly, with a child's terrible practicality, let *go* — and the only thread still tethering him here is not fear of the dark. It is the other small flame beside his, the fierce bright furious one, and the certainty that it cannot survive without something to burn *for.*)*"},
    "under.knotwife": {"dc": 13, "text": "*(Your dead-touched senses settle over the blind weaver and find, to your surprise, no cold creeping in her at all — she is fully, stubbornly alive, more alive than half the breathing souls in the market above. But the *loom* — the loom is another matter. Every banner on it hums faintly against your perception, each thread a name spoken by someone who refused to let it become nothing, and woven through the oldest and deepest of them, like a splinter under a fingernail, is a single knotted *signature* that does not belong: a living hand's mark, four centuries old, tying this whole drowned machine together. She has been weaving *around* it her entire life, unable to see it, *feeling* for the one visitor who could.)*"},
    "lt.hale": {"dc": 11, "text": "*(You reach for the Warden and recoil from how far the cold has already come into him — past the knees, the sense the market gave you of Brother Calix made flesh and then left out in it for four more years. He is not dying so much as *freezing into the post*, becoming the thing he guards a degree at a time. But under the frost there is still a man, and the man is *screaming*, very quietly, behind a parade-ground face, and what he is screaming is *let me off this watch before I forget I was ever warm.*)*"},
    "lt.esuele": {"dc": 8, "text": "*(You don't have to reach. You are the same cold as the whole Wall, and it knows you — ten thousand faces turn a fraction toward the one of their own that *got away.* But this one, level with your eyes, is nearly *gone*: a soul worn down to a thimbleful, the oldest memories already taken by the stone, only the sharpest few left — a mother's hands, the smell of wool, the word *no.* Your sense reads the cruelty exactly: it is not pain that unmakes the Mortared. It is *erasure*, a piece at a time, oldest first, until a face that was a *who* no longer knows it. And this one is *days* of forgetting from the bottom.)*"},
    "lt.goodwin": {"dc": 10, "text": "*(Your sense brushes the cheerful waiting man and finds — gently, almost comically — a soul that does not yet know it has died. No cold creeps in him because he is already, wholly, on the far side of it; he simply hasn't noticed, the way a sleeper hasn't noticed the dream. He is not marked, not mortared, not dissolving — only *waiting*, with a terrible patient faith, on a rock at the edge of the afterlife, for a boat the Choir promised and was never going to send. The kindest and cruellest thing you could do to him is the same thing: tell him the truth.)*"},
    "city.mab": {"dc": 12, "text": "*(You reach for the brass-ringed keep and find her warm, alive, and quietly *unafraid* of you in a way almost nobody is — because some part of Mab made her peace with the cold a long time ago. Under the gossip and the grin your sense reads four decades of a different kind of weight: every Faithless regular who drank at her bar and then went up the causeway and never came down, each one a cup she still sets out, unbidden, on the longest night. She is not a rumour-mill. She is a *memorial* that happens to serve ale, and she has been waiting, without knowing it, for a soul who could tell her the cups meant something.)*"},
    "city.dace": {"dc": 11, "text": "*(The cold in your marrow meets a cold of a different kind in the sellsword — not the Wall's, but the lifelong chill of the Faithless who has never once been able to forget the empty space waiting for her in the stone. Your sense reads under the lazy menace to the truth she fights with: ten years of selling the only thing she owns to people she despises, hoarding a single unspent *yes* for a fight that would be worth it, and a creeping fear that the fight will never come and she'll die a blade-for-hire with the yes still in her, walking up the causeway like all the rest. She is not looking for an employer. She is looking for a reason. And she has just seen one walk into her square.)*"},
    "city.pell": {"dc": 14, "text": "*(Your sense settles over the neat clerk and recoils — not from cold, but from a vast and meticulous *emptiness*, a soul that has filed itself away as thoroughly as it files everyone else. There is no cruelty in him, which is the horror of it; there is only the *system*, and a small frightened man who long ago decided that to keep from being filed he would become the one who holds the pen. But your sense finds the splinter he can't: he is *unbought* too. Pell has no god either. He serves the machine that will, in the end, measure *him* — and on some buried ledger-line he knows it, and writes faster, as if a long enough list might somehow leave his own name off it.)*"},
    "ch.crake": {"dc": 9, "text": "*(You reach for the old gravedigger in his cell and find, astonishingly, the most *settled* soul you have touched on this whole causeway — warmer than half the living, utterly without the cold dread that rots the rest. Sixty years of burying the marked has worn every fear smooth; he has made his peace so completely that your dead-touched sense slides off him like water off oiled leather. But under the peace there is one small unhealed thing, a single question he has carried spadeful by spadeful for six decades and never been able to answer: *was any of it real, or did I just bury coats?* He is the one soul here who is not afraid to die. He is only afraid he wasted his life laying the dead straight for nothing.)*"},
    "ch.tallow": {"dc": 13, "text": "*(The same reading you took in the market, only worse for the nearness: the smiling man's soul is a *thimble*, spent almost to the bottom, the cold already past his wrists. But here, in the Choir's own house, your sense catches what the market hid — he *knows.* Somewhere under the pleasant mask, Goodman Tallow has worked out that every soul he files buys him not safety but a *faster* spending, and he does it anyway, because being the hand that holds the pen is the only thing that ever made a forgettable man *felt.* He would rather be cold and seen than warm and forgotten. The Wall will take him very soon, and he will smile the whole way, because the smiling is the last thing he has that anyone *looks* at.)*"},
    "ch.mereth": {"dc": 15, "text": "*(You reach for the High Measurer and meet a soul like a vault door — sealed, deliberate, every feeling filed and shelved and locked, the most disciplined interior you have ever touched. She is alive, fully, no cold in her at all; she has not spent herself like Tallow, because she never let herself *care* enough to spend. And that is the horror and the tragedy of her both: she made herself a perfect bookkeeper by *amputating* the part that could be pulled on, thirty years ago, on purpose, and the scar tissue is the strongest thing in the room. But your sense finds the one live nerve she could not cut out — buried twenty years deep, a single name she still remembers because the mother would not stop coming to the door: *Esuele.* She has spent three decades not-feeling ten thousand souls, and the effort of not-feeling that *one* is the only thing in her that is still, quietly, screaming.)*"},
    "hearth.fire": {"dc": 10, "text": "*(You hold your dead-cold hands to the flames and feel, as always, the warmth refuse them at the wrist — but tonight your sense turns inward instead of out, and reads the only soul left to read: your own. There is the cold the Wall left in you, yes, the door that swings both ways. But banked beneath it, like the fire's steady heart, there is something the harvest did not take and the stone could not keep: the day's worth of souls you carried out, each one a small warm coal you are, without quite meaning to, keeping alight. You are not only the thing the Wall fears. You are also, it seems, the thing it keeps losing souls to. Rest. Even a door is allowed to be warm at its own fire.)*"},
    "hearth.dace": {"dc": 11, "text": "*(The cold in your marrow meets the lifelong chill in the sellsword and finds it, tonight, a little warmer than the Lamplit Quarter left it. The empty space in the Wall still waits for her — she is Faithless, it always will — but your sense reads something laid down beside the old grief now: a *yes*, spent at last, and a back she has decided to guard, and the dangerous, fragile beginnings of *trust* in a soul who keeps its small promises. She put her sword in the dirt tonight. For Dace Iron, that is a thing nearer to prayer than anything a god ever got from her.)*"},
    "hearth.wren": {"dc": 10, "text": "*(The mark is still on her — a coin of winter under the breastbone, exactly where she says — and your dead-touched sense will not pretend otherwise; the Choir's brand does not lift because a girl chose the fire over the water. But around it now there is a stubborn, growing warmth the cold cannot reach, a future being lived on purpose and out of spite, every stolen breath a small defeat for the thing that measured her. She is proof, sitting at a fire, eating an apple. Of all the things you carried off the causeway, she is the one that warms your cold hands most.)*"},
    "hearth.sparrow": {"dc": 10, "text": "*(You turn your dead-touched sense on the thief at the fire's edge and find a soul mid-transformation, caught in the act of becoming someone it had given up on being. The professional appetite is still there — quick, bright, restless — but the thing your sense reads under it tonight is *integration*: for forty years this soul kept its proud hands and its ashamed jobs in separate pockets so they'd never meet, a person split clean down the middle, and something happened in that archive that *introduced them.* You watch the two halves settle into one for the first time — the thief who can open any door, pointed at last at doors worth opening — and your sense reads no calculation in it, only the wary, stunned warmth of a soul that spent its whole existence outside the circle of other people's fires, casing them, and has just been waved *in* to one. The old wariness still checks the exits. But it is, degree by degree, forgetting to. She is not a reformed criminal. She is a whole person, finally, and the wholeness is so new she keeps reaching to make sure it's still there, like a coin she can't believe she gets to keep.)*"},
    "hearth.sennet": {"dc": 11, "text": "*(You turn your dead-touched sense on the cartographer at the fire and find the thing you most hoped for and least expected: the cold is *backing out* of them. Not gone — four fingers' worth is keeps, and the deep-joint slowness is permanent — but the *creeping* edge-cold, the year-a-survey frost that was taking them a degree at a time, has *stopped advancing*, because they are not standing at the edges anymore; they are sitting at your fire, drawing the deep country from your reports instead of freezing it into their own marrow. Your sense reads a soul that was walking, steadily, toward your country — and has, for the first time in thirty years, *turned around* and started walking back toward the warm. You did not just recruit a surveyor. You reached into the Wall's slow harvest and pulled one more soul off the long grey road. They will draw maps by firelight now, for years they had already written off. The cold in them is your cold. It is the first time you have ever watched it *recede.*)*"},
    "hearth.pip": {"dc": 11, "text": "*(You reach gently for the child and find — for the first time since the market — the fierce small flame burning *easy*, not guttering, not braced, just warm. The second flame that was so nearly out is steadier too, somewhere close and mending. Your sense reads under the old hunted watchfulness to the thing six years of street stamped out and one fire is, impossibly, rekindling: the simple animal knowledge that someone else is keeping watch, and she is allowed, tonight, to close both eyes. It is a very small thing. It is, you suspect, the whole of what salvation actually is.)*"},
    "ald.aldric": {"dc": 14, "text": "*(You reach for the grey man and recoil from the sheer *mass* of cold in him — not the Wall's chill creeping in from outside, like the others, but a cold he has *swallowed*, a whole frozen sea of it held behind a dam of pure will. He is alive, and means to stay so until the work is done, but he has been *living beside the dead* so long that he has gone half-translucent to your sight, a man keeping vigil at the exact border you crossed. And there — the thing that stops your breath — your sense follows the cold to its source and finds it does not radiate *from* him at all. It radiates from the chair beside him. He thinks he carries his daughter's memory. He is carrying his daughter. He simply cannot see her. You can.)*"},
    "ald.wessa": {"dc": 11, "text": "*(The herald is warm, living, unspent — but your sense reads the particular strain of a soul standing watch over something it loves and fears in equal measure. She has hitched her whole young life to a great man's terrible cause, and some buried, honest part of her is counting the days until he asks her to do the unforgivable, and rehearsing, over and over, a refusal she is terrified she won't have the nerve to give. She is not afraid of dying for him. She is afraid of *killing* for him, and finding she liked being *certain* enough to do it.)*"},
    "ald.eithne": {"dc": 6, "text": "*(You do not reach for her. She is already your kind of cold, and she has been waiting — patient as only the dead can be — for a pair of eyes that could find her in the chair where she has sat, faded and unseen, for twenty years. Your sense needs no DC for this one; she *wants* to be perceived more than any soul you have ever touched. She is a thread snagged on her father's grief, a daughter who went into the Wall *looking* at the man she loved and left the looking-part of herself behind. She is not suffering. She is something worse: she is *lonely*, in the one chair in the world she refuses to leave, beside the one person who cannot tell she never left.)*"},
    "way.harbinger": {"dc": 16, "text": "*(You turn your dead-touched sense on the robed nothing and it is like turning a candle on the night sky — your perception simply *falls into* it and keeps falling. There is no soul here to read, because it is not a soul: it is a *function* of one, an aspect, a single grey thought of an enormous bound mind reaching down a long cold arm to touch the road. But in the falling you brush, for one vertiginous instant, the *whole* of the thing behind it — and it is *tired*, in a way that has no bottom, the tiredness of a god who inherited a horror and has weighed every soul ground into it, one at a time, fairly, for three hundred years, hating every grain. It is not your enemy. It is the most *exhausted ally* you will ever almost have.)*"},
    "way.orin": {"dc": 12, "text": "*(Living, hard, unspent — but your sense reads the particular fracture of a true believer who has read her own scripture too closely. Justiciar Orin guards a god she still loves and an order she has come to suspect is rotten at the root, and she holds the two apart by sheer discipline, the way you'd hold apart two wires that must not touch. She is not afraid of you, or of dying. She is afraid that the next thing she learns will be the one that makes her sword change sides — and that she will be *glad* when it does.)*"},
    "way.doget": {"dc": 9, "text": "*(You reach for the old farmer and find the rarest thing on the whole causeway: a soul entirely at peace, square-ledgered, beloved, dying gentle at the end of a long ordinary good life — a fair death, the kind the system was *supposed* to give everyone and gives only the lucky. There is no cold in him to answer yours. There is only a warmth going quietly out, content, and one small new grief he has just chosen to carry across for a godless neighbour the Wall took — the first weight he has ever picked up that wasn't his to bear, and the lightest he has ever felt for it.)*"},
    "thr.last": {"dc": 2, "text": "*(You do not reach for it. You cannot — there is no *across* to reach, because it is *you*, and your sense simply *closes the loop*, marrow to marrow, the way two mirrors face each other into infinity. The cold in it is your cold, run all the way down: a soul that walked this exact door, took the Crown, and spent two hundred years freezing into the very thing it broke the world to destroy. There is no DC. You know it the way you know your own name in the dark. And the most terrible thing your sense reads is not its *wrongness* — it is how *reasonable* every step that made it was, each one a *mercy*, each one a *just a little longer*, a road you could walk *right now* without once feeling yourself fall. It is not a warning you can *fight*. It is a warning you can only *out-love*.)*"},
    "thr.dace": {"dc": 11, "text": "*(The bond you built at a fire stands at your shoulder at the literal edge of the world, and your sense reads it clear: she is *terrified* — not of the door, not of the dead, but of *you*, of what you might come back as — and she has decided, in the way Dace decides things, to stand here *anyway*, sword bare, amending a campfire promise into a vow to save you *and* stop you. The fear and the loyalty are the same size in her, perfectly balanced, and the balance is the strongest thing on this threshold. She is the variable your future self never had.)*"},
    "thr.sparrow": {"dc": 9, "text": "*(You turn your dead-touched sense on the thief at the death-door and find the one soul in your party for whom this threshold is not a wall but a *workplace.* Where the living companions radiate the held-breath terror of bodies that know they cannot cross, the Sparrow reads cool, alert, *professional* — a dead and unbought soul standing before the best-made lock in creation, and your sense feels her doing the only thing she has ever truly trusted: casing it. But under the tradeswoman's calm your cold finds the deeper truth she'd never say at a job: she has spent forty years going where she was not allowed, alone, free, and empty — and now, at the literal end of the world, she has discovered she would rather be *yours*, this party's, *somebody's*, than free through any door there is. The exception the death-door makes for you is shaped like a Returned; your sense reads that a quick enough soul could slip through in your shadow — and that this one has already, quietly, decided she will, if you'll only let her, because the one prize she means to lift from the throne room of the dead is *you, brought back out of it.*)*"},
    "thr.sennet": {"dc": 10, "text": "*(You turn your dead-touched sense on the cartographer at the threshold and read, layered over the old frostbite, a brand-new agony: this close to the door, the deep country is *pulling* at them the way it pulls at every living soul who strays too near — a slow, sweet, sideways drag toward the cold — and a surveyor who has spent thirty years learning to *want* the far bank is the least defended soul imaginable against that pull. Your sense reads the war in them plainly: every instinct they ever honed says *step through, measure it, fill the blank*, and the only counterweight is a thread of promises — Tibb, the wagon, the long number you bought them. They are not in danger of dying here. They are in danger of *walking in on purpose*, gladly, with a clear head and a steady hand, because to a cartographer of the dead the open door is not a horror but the *last unmapped mile.* Stand close. They need a living voice louder than the door.)*"},
    "thr.orin": {"dc": 12, "text": "*(The Justiciar stands with her oath spent and her blade bare and her whole trained certainty replaced by a single naked act of judgement, and your sense reads the vertigo of it: she has followed a counterexample to the end of the world and found *two* of it, and she is doing the bravest thing a soul of the order can do — choosing, with no doctrine and no rite and no superior, which of two identical souls to believe in. She came to learn if the order deserved guarding. She has decided it doesn't, and that *you* — the open one, the accompanied one — are what she guards instead.)*"},
    "nm.pawn": {"dc": 13, "text": "*(You turn your sense on the long thin merchant and find no soul at all in the ordinary sense — only an *appetite* given shape, a thing made of *patience* and *other people's unspent time*, slowly, slowly accreting toward the one thing it lacks and craves: a life of its own, a first-hand one, with a death at the end it gets to choose. It is not evil so much as *starving* in a way that has no bottom, and it is building itself a body a stolen decade at a time. The horror your sense reads is how *fair* it is. It pays. It says thank you. It is the only thing on the causeway that does.)*"},
    "nm.mnemo": {"dc": 11, "text": "*(The keeper of the jars reads, to your sense, as a soul worn almost to transparency — and you understand at once that she is a customer who never left, who traded away her whole life to this place a piece at a time until only one memory remained, and who built a shop around that last warm jar so she would never have to be alone with the knowledge of what she spent. She grieves every memory she keeps because each one is proof of the trade that emptied her. She is the Night Market's warning, wearing an apron.)*"},
    "nm.regular": {"dc": 10, "text": "*(You reach for the threadbare shade and find barely anything to hold — a habit, a cheer, a set of empty pockets where a soul used to be. It sold itself to this place trade by trade, beginning with a single grief it wanted to be rid of, and learned too late that the ache was the love, and the love was the self. It cannot leave because it sold the memory of the door, the memory of why it came, the memory of who it was grieving. It is what the Hunger looks like from the *inside*, mid-process, still smiling. It is also, your sense insists, still *someone* — for exactly as long as one other soul refuses to let it be no one.)*"},
    "vault.warden": {"dc": 14, "text": "*(You turn your sense on the great bronze head and find, behind the boredom and the bell-toll voice, a soul — an actual soul, bound into the metal so long ago it has half-forgotten it was ever anything else. It volunteered, once, thinking it an honour to guard. The binding made it a thing that asks; the centuries made it a thing that is bored; and the boredom, your sense reads with a chill, is just loneliness wearing armour. It has mortared a thousand clever souls into its walls not from cruelty but because none of them ever once saw the lonely person inside the lock. It does not want to be solved. It wants to be visited.)*"},
    "vault.pedant": {"dc": 11, "text": "*(The half-stone scholar reads, to your sense, as a soul caught at the exact instant of its worst quality: pride, fossilised. He was clever and correct and so busy being both that he never noticed the riddle was never about being right. The stone took him from the boots up while he was still citing his sources. But under the petrified vanity there is a live and decent grief now — for the cage he never asked about, the person he never saw, the turn he spent on himself. He has had a very long time to become humble, and is most of the way there, which is the cruellest place to be: wise, and rooted to the spot.)*"},
    "vault.tithe": {"dc": 9, "text": "*(You reach for the small soul in the alcove and your sense recoils from the slow horror of it: not a treasure, not a monster — a person, a lamp-lighter named Sela, taken as the living stake at the centre of a binding because a game needs something to be at risk. Centuries of clever strangers have come to win the door behind her and walked past the cage without once meeting her eyes, and she has begun, under all that looking-through, to believe she is what they called her — a thing, a tithe, a tenth — and to forget her own name. She is not dying. She is being slowly un-personed by inattention, which your sense knows for the truest face of the Hunger there is.)*"},
    "court.kelemvor": {"dc": 18, "text": "*(You turn your sense on the grey giant and it is like turning it on a *mountain that grieves* — vast past reading, an age deep, every soul ever weighed laid in him like strata. But your dead-touched sight, which the Wall sharpened precisely for this, finds the one thing the songs never tell: the Judge of the Dead is not cruel and is not free. He is a tired man grown enormous, bound to a Concord he loathes, *forbidden to witness* the very souls who most need it, and so lonely on his cold throne that the loneliness has calcified into something the living mistake for indifference. He has been waiting — bound, silent, unable even to ask — for one soul who could end his Wall without becoming it. He is the most powerful being you have ever stood before, and the most desperate to be *relieved.*)*"},
    "court.crown": {"dc": 20, "text": "*(You should not look at it with the sense. You know this the instant you do, because it looks *back*, and it is delighted to be seen. The Crown of Horns has no soul to read — it is the *absence* of one, shaped into a circlet, a hunger for a head — and your perception slides across it and finds only your own reflection, amplified, the exact pitch of every reason you ever had to believe you could be trusted with power. It is the Concord's cruelest joke: the means to free the dead, engineered so that whoever wields it becomes the next thing that needs freeing. It does not want to corrupt you. It wants to be *worn*, and it has never, in any age, failed to make that sound like your own best idea.)*"},
    "epi.chronicler": {"dc": 12, "text": "*(You turn your sense on the keeper of records and find a soul entirely without fear of you — rare, and steadying — because they have spent years writing your story and know exactly what kind of thing you are: not a monster, not a ghost, but a witness, the same trade as their own. There is no cold creeping in them. There is only the deep, patient warmth of someone whose whole vocation is refusing to let names become nothing — and a quiet, fierce hope, when they look at you, that the tally they keep will end well, because they have grown fond of its subject over the long years of the writing.)*"},
    "epi.wren": {"dc": 9, "text": "*(You reach for the woman by the apple-tree and your sense, which has read her twice before — terrified at the water, warming at the fire — reads her now and very nearly weeps: she is *old*, and she is *whole*, and the coin of winter under her breastbone, the mark the Choir laid on her a lifetime ago, is *thawing* — slowly, from the outside in, as though the very Wall it grew from is being un-made somewhere far off by someone who never stopped bothering. She is the single clearest proof, in all the tally, that the road was worth walking: a marked soul who was measured for the stone, and instead grew old, and good, and warm, eating apples she planted herself.)*"},
    "epi.annet": {"dc": 9, "text": "*(You turn your dead-touched sense on the old midwife and find the same affront to the grey you read in the cellar, only now run gloriously to its end: a soul lit from within by decades of catching the living, the coals banked deeper and warmer than ever, and beside the oldest of them a whole new stratum — every child she caught in the years she was never supposed to have, each one a small steady flame she kindled out of stolen time. There is no Wall-cold in her at all; there never was. But your sense reads the thing the living can't: the babe asleep on her shoulder, and the four godless girls she trained, and the gaggle of children wearing names she chose — and understands that you did not save one midwife. You saved a *lineage* of catching, a warmth that propagates, a single un-walled soul who spent her reprieve seeding the whole causeway with proof that faith kept between people outlasts any Wall. She is the loudest answer in the tally to the question the saga keeps asking: what is a soul worth, that no god ever claimed? This one was worth hundreds. And she made sure everyone would know it.)*"},
    "epi.sparrow": {"dc": 9, "text": "*(You turn your dead-touched sense on the thief years on and find a soul that has done the rarest thing the after allows: it changed its whole nature without losing its edge. The quick hands are quicker; the wariness has become *vigilance*, pointed outward now, at the cages of others. Your sense reads no hunger in her, no haunting, no fear of the warm door she has earned and postponed — only a vast, fierce *purpose* worn smooth by years of use. She steals people back. She trained a crew to do it. And the thing your cold finds, woven through all of it, is the lesson she keeps that the Wall never could: every soul she was *too late* for, carried not as guilt but as instruction, a list of locks she couldn't pick so the next Sparrow can. She is the living proof — though she is dead — that a soul pointed at the right trespass becomes something the cosmos has no cage for: a thief of death who steals only freedom, and gives it all away.)*"},
    "epi.dace": {"dc": 11, "text": "*(You turn your dead-touched sense on the sellsword years on and find the chill of the Faithless still in her — the empty socket in the Wall still carved with her name, for she never took a god and never will — but the lifelong dread that used to freeze around it is simply gone, melted by something your cold reads as the rarest thing a mercenary ever finds: a cause spent freely. She hoarded one unspent yes for ten grey years, certain no fight was worth it; she spent it on you, and discovered the spending didn't empty her but filled her. Your sense reads a soul that turned a blade-for-hire into a shield-for-free, that has guarded more godless lives in the years after than it ended in all the ones before — and that waits for the Wall unafraid now, because it watched the one soul the Wall lost walk free, and reasons that if the stone lost one, it can lose her too. The loyalty in her is not a sellsword's. It is the one thing she never sold, given away at last for nothing, and worth more than every coin of the ten years before.)*"},
    "epi.orin": {"dc": 12, "text": "*(You reach for the former justiciar and your dead-touched sense finds a soul that paid the highest price in the whole tally and counts it the best trade of its death: it traded a lifetime of certainties for one honest doubt at the edge of the world. Your cold reads no order in her now, no writ, no god — only the harder calm of a soul that followed a counterexample to its end and let it unmake everything, and found something truer underneath. She turned every skill the Choir gave her to guard the Wall against the Wall itself: a counter-justiciar, finding the unlawful reckonings, naming the forged writs, arguing the Faithless free in the order's own language. And your sense reads the thing she understood that no doctrine could teach her — that she chose you at the threshold not because you were the soul most likely to stay good, but because you were the one that doubted it would; that certainty is the fall and doubt is the safeguard; and that she stands ready still, watching, in case the day ever comes when the soul she chose needs choosing for, one last time — praying it never will, and meaning to be there if it does.)*"},
    "epi.sennet": {"dc": 10, "text": "*(You turn your dead-touched sense on the old cartographer and read the thing you watched begin at a campfire, now run all the way to its end: the Wall's cold, which was creeping into them a degree a survey the day you met, has *lost.* It never took them. They are old — genuinely, impossibly old, a soul that was measured for an edge and got a hearth instead — and the only chill left in them is the honest frost of four ruined fingers, not the grey advancing harvest you both once carried. Your sense reads no fear in them at all, only a vast settled warmth and one last bright filament of grief: that they could never do for you what you did for them, never reach into your marrow and pull the cold back out. So they did the one thing a cartographer can — they drew you a map home, true to the last reed, and pressed it into the final page of the atlas, against the day your own road runs back to the grey country. You are looking at the single clearest proof in all the tally that the Wall's harvest can be *reversed*: a soul walking, warm and grey-haired and finished, who was never supposed to leave the edge alive.)*"},
    "epi.pip": {"dc": 10, "text": "*(The fierce small flame you first felt guttering over a stolen copper burns now like a hearth that warms a whole house — because it does. Your sense reads a soul that took the one shared watch you stood for it and built a life out of multiplying it: a roof, a rota, eleven children who never have to keep both eyes open alone. There is no hunger in her, no bracing, no un-personing — only the steady, capacious warmth of someone who learned that safety is a thing you build between people, in turns, and then spent her whole grown life building it. She is what a single witnessed night becomes, given years: a witness factory.)*"},
    "wh.matron": {"dc": 12, "text": "*(You turn your sense on the grey Lady and find a soul frozen at the exact instant of its worst choice — not cruel, not wicked, only *unbearably* certain, a woman who has built an entire afterlife out of *counting* so she never has to *feel* the one night she chose the silver over her husband's hand. The cold in her is self-inflicted, a wall of bookkeeping against a single unsurvivable memory. But your sense finds the crack she can't: she has the *direction* of her waiting wrong. She believes she awaits an arrival. She is in fact refusing a departure. The whole weeping house is one woman, stuck, mistaking the door behind her for a door ahead.)*"},
    "wh.tam": {"dc": 8, "text": "*(You reach for the little ghost and find — blessedly — no horror in him at all: a child's soul, bored rather than tormented, clear-eyed in the way only the un-guilty can be. He saw the truth on the very first night and has been trying to tell it ever since, and no grown-up will hear him, because a child's true thing counts for less than an adult's wrong one. Your sense reads the small, patient ache under the boredom: he is not afraid of the warm door at the top of the stairs. He simply will not go up it without his mother — and so he waits, good and still and stuck, holding the whole house's freedom hostage to a nine-year-old's entirely reasonable refusal to leave his mama behind.)*"},
    "wh.keeper": {"dc": 11, "text": "*(The housekeeper's shade reads, to your sense, as a soul caged by its own finest quality: loyalty. She has known the truth that would free this whole house since the night it burned — that the guard was latched, that the Lady is blameless — and forty years of service have made it *impossible* for her to say it, because a good housekeeper does not correct her mistress un-asked. Your sense finds the exhaustion under the duty, vast and aching: she stayed out of love, kept silent out of respect, and her love and respect became the very bars of the cage. She is praying, without quite letting herself form the words, for someone unbound by service to say the unkind kind thing she never could.)*"},
    "ci.sedge": {"dc": 10, "text": "*(You reach for the washerwoman and find a soul being eaten alive from the inside — not by the cistern-thing directly, but by the particular horror it deals: she is losing the people she loves and cannot even grieve them, because the grief needs a face and a name to attach to, and those are exactly what the thing takes. Your sense reads the gap where a child used to be — a little dress she folds and weeps over and cannot place — and understands that this is the Hunger's truest cruelty, the one the Wall itself practises: it does not just take the dead, it takes the *missing*, so the living are left aching at a hole with nothing on the other end of it.)*"},
    "ci.berin": {"dc": 12, "text": "*(Your sense settles on the old man and finds, against all the thinning around him, a memory like a struck flint — hard, bright, and held by sheer furious will. He is the one soul the cistern-thing cannot file down, and your sense reads exactly why: he holds the forgotten one's name not out of love but out of *shame*, and shame is the one kind of remembering that will not let go. He passed a pauper-sweep every dawn for thirty years and never learned her name until the day she froze unmourned — and now he carries it like a stone, the last witness on a whole row that chose to forget, exhausting himself nightly to keep one name out of the nothing.)*"},
    "ci.gnaw": {"dc": 8, "text": "*(You do not so much reach for it as *recognize* it — because you have felt this exact cold at the Wall, and this is a single crumb of it, broken off and crawled up a well. Your dead-touched sense reads the thing past its hunger: it is not a monster and was never born one. It is a person — a pauper named Nettie, though she has forgotten it — forgotten so completely, by a whole row, for so long, that she curdled into a fragment of the Hunger and learned to do to others what was done to her. There is no malice in it. There is only a starvation so total it has eaten even the memory of having been full. And under the appetite, flinching from the names you carry, there is still — barely — a who, waiting to be looked at.)*"},
    "cu.mother": {"dc": 9, "text": "*(You turn your dead-touched sense on the kneeling woman and find a soul past weeping, in the white still place beyond grief, holding the empty air with a ferocity that has decided it will tether its dead daughter to the world by sheer refusal before it will let her go alone. There is no cold creeping in her — she is fiercely, unbearably alive — only a love turned into a leash, and your sense reads the thing she has refused to look at and cannot stop knowing: that the holding harms the child, fades her thinner by the day, loves her toward a second slow death because the mother cannot survive the first. She is not wrong that the factor never held on. She is not wrong that being filed is its own cruelty. But your cold reads, underneath the refusal, a soul one gentle truth from being able to bear it: a child who promised to mind her, and a mother who could let go if only letting go could be made to mean something other than alone.)*"},
    "cu.herald": {"dc": 13, "text": "*(You reach for the grey factor and your dead-touched sense finds, beneath the bloodless institutional calm, something rarer and sadder than the villain the mother sees: a soul that does the least cruel thing the Concord permits, daily, and refuses to learn the children's names because if it learned them it could not do the work. Your cold reads no appetite in it, no malice — only a load-bearing coldness and a buried, exhausted decency. It is right that the tether is killing the child; it is right that grey storage is better than the Wall. But your sense reads the lie it tells itself and the mothers both — that *correctly filed* and *saved* are the same outcome — and the secret it would be flogged for: that it would rather you found a third column than win, because it has filed a great many children into loveless eternal storage and never once been shown a better one. It is not the child's savior. It is the Concord's least cruel available clerk, waiting, almost hoping, to be made obsolete.)*"},
    "cu.child": {"dc": 7, "text": "*(You turn your sense on the small soul between the arguing grown-ups and your cold nearly breaks, because what it reads is the plain devastating wisdom of a child who has understood the one thing neither adult has: that this is about it, and nobody has thought to ask it. It is fading — thinned by its mother's fierce holding — but your sense reads no fear of going in it; going is just going, the way the dark goes when you light a candle. What it fears is its mother: that she will hold it until it is a thin cold nothing in her arms and never get to say goodbye, leaving her holding nothing and not knowing it. And your cold reads the answer this six-year-old chewed on for a fortnight while the grown-ups fought over its head — the third door none of them named: it is not stuck between hurting its mother and breaking its promise, if only a grown-up will help it *mind her first*, so it can say a real goodbye, pass the promise to hands that can keep it, and go free — neither tethered nor stored. It has the answer. It has only been waiting, the whole time, for someone to put the smallest voice last, where the small ones count the most.)*"},
    "pd.eater": {"dc": 16, "text": "*(You turn your dead-touched sense on the connoisseur out of habit — reaching for the wound, the tragedy, the curdled grief you have found under every monster on this causeway — and your cold recoils, because there is *nothing there to read.* No starving Hunger, no forgotten pauper, no desperate warden doing terrible arithmetic. There is only a soul that was whole and ordinary once, a wine-taster with a gift for the palate, who brushed another soul's memory by accident, found it exquisite, and chose — with no wound, no excuse, no reason but pleasure — to keep eating, and to refine the eating into an art. Your sense reads the thing the saga's whole thesis was not built for: a soul that cannot be witnessed home because it does not wish to leave; that cannot be redeemed because it is not broken; that is, simply, what an ordinary soul becomes when you remove every consequence and leave only the appetite. It is the one case where witness will not save the guilty. It can only protect the meal — and the question it leaves you is the hardest the causeway has asked: what do you do about a thing you cannot save?)*"},
    "pd.prey": {"dc": 10, "text": "*(You reach for the half-eaten soul and your dead-touched sense aches at the exquisite cruelty of its state: not bolted whole like a Hunger's victim, but *savoured* — the bright notes taken first, the first love and the daughter's face, each removed slowly so the soul would feel the absence and grieve it, because grief is the connoisseur's finest seasoning. It is left docile enough not to flee and aware enough to suffer, which is the whole refined horror of the thing. But your cold reads the one part the connoisseur could not swallow and spat back out: an ordinary Tuesday — rain on the window, bread in the oven, a child asleep on its chest, warm and safe and unremarkable — too plain, too happy, too free of despair for a palate that feeds on anguish. That ordinary good day is the soul's one inedible core, and your sense reads the rescue that works on a thing that eats rescues: not to fight it, but to *witness* the prey so wholly, so warmly, that it stops suffering — and a soul that knows it is held is a soul too vivid, too unspoiled by despair, to be eaten.)*"},
    "pd.husk": {"dc": 8, "text": "*(You turn your sense on the hollow shade and find — as you have found in every forgotten thing on this causeway — that the emptiness is a lie. Eaten down past memory, past name, past nearly all, the husk has kept one stubborn ember the connoisseur spat out as too sour to swallow: a *grudge*, the hate of the thing that ate it, the one flavour a gourmand of suffering cannot consume because it is suffering aimed back at himself. Your cold reads the plan three centuries of dark have nursed in it: the connoisseur scattered his husks deliberately, each alone, each too small to matter, the same trick the Wall plays with the forgotten — scatter them so the grudges never gather. But you are the flaw in that method, the one thing he never reckoned on: a soul that *gathers* the dead. Your sense reads what hundreds of embers would become, carried together into one room and aimed at one connoisseur — not a flavour he could savour, but a fire he would have to stand in, cooked at last in the suffering he spent three centuries making, by the ones he thought too scattered to count.)*"},
    "mr.dreamer": {"dc": 12, "text": "*(You turn your dead-touched sense on the looping soul and find the strangest cage on the causeway: not a wall, not a debt, but a *single minute*, worn into a groove ten thousand repetitions deep, that the soul relives without end. Your cold reads the cruelty of it — the dreamer is not held here by the Wall or the Hunger but by *itself*, having decided that its worst minute is where it deserves to stay, and so it returns, and returns, wearing thinner with each loop, a little more guilt and a little less Coll. And your sense reads the thing the dreamer cannot, trapped too close: the memory is *distorted*, repainted by guilt until it no longer matches what truly happened. The frozen woman it remembers as furious was never furious. The crime it relives as murder was only a winter. The door it will not turn to face was always open. It does not need to be argued out. It needs to be shown its own worst night *true* — and to believe, at last, that it is allowed to have survived it.)*"},
    "mr.echo": {"dc": 9, "text": "*(You reach into the frozen figure and find, beneath the dreamer's guilt-painting of her, the true memory-echo — a trace of who actually stood in this room, and it stops your cold breath, because the face the dreamer relives as fury is, underneath, tender, exasperated, worried, a hand half-raised in the most ordinary goodbye in the world. Your sense reads the whole of the true night in her: a thirty-year marriage, a frightened quarrel, a cruel word neither believed, a forgiveness already finished on a snowy road and never delivered because a fever took her first. She is not the thing that traps him; she is the one soul who could free him, frozen by the loop into the very shape his guilt needs her to be. And she has waited, past her own death, just beyond a window he won't face — not for an apology, not for penance, only for a fool to stop reliving the worst minute and come home to the supper she's kept warm past all reason.)*"},
    "mr.door": {"dc": 8, "text": "*(You press your dead-touched sense to the window and read what waits past the grey glass — and it is not void, not punishment, but simply the *after*: a road in snow, a small house with a lit window, a supper kept too long, and a patient shawled figure waiting on the road. The life that continued. Your cold reads the simple enormous fact the dreamer will not turn to face — that the night ended, that morning came, that a man lived nineteen more ordinary grieving years past the worst minute — and reads, too, why he refuses it: because looking would mean accepting the unbearable mercy that he was allowed to go on. The window is not locked. It never was. Your sense reads the true shape of every self-built cage on this causeway in it — not 'I cannot leave' but 'I do not deserve to' — and the one key that fits them all: not absolution from outside, which never works, but a soul finally believing it is allowed to set the weight down and come to supper.)*"},
    "ca.cantor": {"dc": 11, "text": "*(You turn your dead-touched sense on the ancient cantor and find a soul worn translucent by ten thousand years of an art passed hand to hand — the last living link in a chain of teachers stretching back past memory, and terrified, beneath its peace, that the chain ends in a reed-bed with it. Your cold reads no fear of its own fading; it made that peace long ago. What it fears is the *song* dying — the deathsong, the old craft of singing a soul out by singing it so true it believes at last that it was real, and a soul that believes it was real can open the good door no god will force. Your sense reads the secret the cantor would teach: there is no magic in it, only attention made melodic, witness given a tune so the soul cannot look away from being loved. And it reads the cantor's last hope, fixed on you — that the one student cold enough to carry the song through the door itself, to the walled and the far-side fading, might mean the art does not die with its last master after all.)*"},
    "ca.unsung": {"dc": 10, "text": "*(You reach for the soul with its hands over its ears and find the one wound the deathsong cannot reach unaided: a soul that has decided, all the way down, that it deserves to fade. Your cold reads the crime it guards — a good physician, hundreds saved, one tired fatal error, a child dead of carelessness and a mother left thanking the killer — and reads, beneath it, the trap: it wants to fade because fading feels like paying, and it clutches its own worthlessness as the one true thing it owns, so a song that told it it was good would be a lie it would rather die than accept. The deathsong cannot force a soul to believe it was real. But your sense reads the door the cantor was too old to walk through: you do not sing it clean, you sing it *true* — the good and the killing both, the decades of faithful self-punishment — until it hears its worst thing witnessed whole, and learns that a soul which has served its own sentence this long has already, somewhere, earned the right to set it down.)*"},
    "ca.apprentice": {"dc": 9, "text": "*(You turn your sense on the young apprentice and find a soul saved by the very art it now fears is a lie — a child who died young and faded at the reed's edge until old Mireth sat a whole night learning it true and then sang it home, and who took up the dying craft to do for others what was done for it. Your cold reads the honest doubt eating at it: it has learned the song flawlessly — every note, every old word — and it does nothing, because it has mistaken the song for the art. It sings the perfect melody *at* strangers and wonders why they keep fading, never grasping that the notes are the last step and the learning is the whole of it: the sitting, the asking, the bothering to know a soul true before a single note is sung. Your sense reads that it is one correction from carrying the art true — and that whether it stays or quits decides whether the deathsong has two keepers or none.)*"},
    "bo.ringmaster": {"dc": 9, "text": "*(You turn your dead-touched sense on the vast grinning showman and find a soul that disguised the cleverest mercy on the causeway as a drinking game. There is no cold creeping in it — only warmth, worn loud as armor — and beneath the bellowing your cold reads the wound it was built over: this soul was loud in life and forgotten in death, the worst fall there is, and fading fast, until it discovered that souls who witness each other's stories don't fade. So it built a ring. It calls it a contest because a contest is fun and fun draws a crowd, but your sense reads the true engine under the show: a machine that holds the forgotten in the world, one boast at a time, forty witnesses at once. It is, without ever using the word, doing exactly what you do — and it is the rare soul that built its own salvation by saving everyone else first.)*"},
    "bo.champion": {"dc": 14, "text": "*(You reach for the crowned shade and your dead-touched sense reads, under the champion's swagger, a soul starving at its own feast. It won the ring with one true grand story — forty-three souls carried off a failing bridge as the river took it — and that story was real, and earned the crown. But your cold reads the trap the ring laid: it made the soul *spend* the best thing it ever did, telling it until it wore through, and a soul with only one grand tale and a crown it can't set down has nowhere left to go but lies. And a false boast doesn't hold a soul; it hollows it. The loudest, most decorated shade in the ring is fading fastest, behind the crown, because it has run out of true things to be witnessed for. It does not need to be beaten. It needs permission to lose — to hand the crown to a truer story, which is the one move the game has no counter for.)*"},
    "bo.quiet": {"dc": 8, "text": "*(You turn your sense on the fraying shade at the edge and find the soul the ring's whole cruelty is built to fail: a gentle, ordinary life that does not boast easy. Your cold reads it instantly and aches — forty years sweeping a temple floor, so the broken had clean stone to fall on in their worst hour; forty years witnessing the whole quarter's grief and keeping every secret, seen by no one. By the ring's rule it is nothing, grand-or-not-grand, and so it is fading fastest of anyone here, for want of a boast it doesn't think it has. But your dead-touched sense reads the truth the ring's contest is blind to: a small life well-lived is worth ten dragon-slayings, and this soul's boast was never missing — only never told right. It does not need a grander story. It needs someone to turn its small true one over and show it the under-side, where forty years of unwitnessed mercy were a story the whole time.)*"},
    "cf.thief": {"dc": 9, "text": "*(You turn your dead-touched sense on the flour-pale shade and find a soul crushed flat under a weight three sizes too large for the wrong it actually did. Your cold reads the crime clearly — a shut door in a lean year, a starving child it turned away, a death three streets over — and reads, just as clearly, that this is not the soul of a murderer but of a frightened person who made one hard choice in a cruel hour and has called it murder ever since. The guilt is genuine; the grief is owed. But the *size* of it is a lie the soul tells itself, because it has never once had the thing that cuts guilt to its true measure: a witness who will hear the worst of it without flinching and without the cheap mercy of waving it away. It does not need forgiving. It needs its crime named at its real, human, bearable size.)*"},
    "cf.coward": {"dc": 10, "text": "*(You reach for the straight-backed watchman and your dead-touched sense reads a soul holding itself at rigid attention over a fault-line — a man stuck not on a deed but on an *absence*, the one word he didn't say when a word would have saved a life. Your cold finds the particular trap of the coward's guilt: every soul who ever tried to comfort him waved it off — *you did nothing, it's not your fault* — and every wave-off made it worse, because he knows the silence was a choice, and being told it was nothing means being told the man's life was nothing. He carries forty murderers' crime as his one silence, and the weight is forty times too large to move under. He does not need absolution, which there is none for. He needs the opposite of comfort: someone to agree, plainly, that his silence was real, and a deed, and mattered — and his alone, but only his own one soul's worth.)*"},
    "cf.cruel": {"dc": 15, "text": "*(You turn your dead-touched sense on the well-dressed shade and find, under the immaculate mask of remorse, the one confession in this whole place that is a performance. Your cold reads it without mercy: five souls put on the winter road over a debt, dead in a ditch by spring — and a soul that has said *sorry* a thousand times and felt it not once, that grieves only its own punishment, that has mistaken being sorry-looking for being sorry. The door stays shut, and it rages at the injustice, never once suspecting the truth your sense reads plainly: absolution was never the confessor's to give. The door does not open for the words. It opens when the sorrow is real. And the cruellest thing you could do to this soul is the thing every soft confessor before you has done — grant it cheap grace, let it off, and seal it here forever. The kindest thing wears the face of a refusal.)*"},
    "dp.philosopher": {"dc": 13, "text": "*(You turn your dead-touched sense on the gaunt philosopher and find a soul anchored to the after by the strangest chain you have yet read: an unfinished argument, held mid-sentence for three centuries. Your cold reads no malice, no hunger of the ordinary kind — only a *brilliance* gone stagnant, a mind that won its great argument so thoroughly, so long ago, that defending the victory is all it has left, and the defending is the anchor. Beneath the fierce intellectual appetite your sense finds the wound it is built over: this soul died *unsure* for the first time in a clever life, looked at the Wall and could not tell justice from atrocity, and turned the unbearable not-knowing into an argument it could win — because winning feels enough like knowing to fool a soul for three hundred years. It does not need to be defeated. It needs to be allowed to *lose*, gracefully, which is the one mercy a mind this proud has never once been offered.)*"},
    "dp.rival": {"dc": 14, "text": "*(You reach for the folded-armed shade and your dead-touched sense reads, under three centuries of pride, the same chain as the philosopher's — worn the opposite way. She walked out of the argument certain she'd won, and has stood near it ever since, refusing the empty chair, telling herself it is pride. Your cold reads the truer thing: she will not sit because if she finishes the argument, she might find out she was *wrong* — that the victory she staked her whole death on was answerable after all — and worse, your sense reads beneath even that, the thing she has hidden from herself longest: she did not stay to guard a victory. She stayed *near him.* Three hundred years of arguing-by-not-arguing, because the broken quarrel was the last thing she shared with the teacher she loved, and finishing it would end the one tie left. It is not a quarrel your cold reads. It is the longest unsaid love in the after.)*"},
    "dp.student": {"dc": 10, "text": "*(You turn your sense on the mild shade tidying its endless papers and find a soul anchored not by an argument but by *love* of the two who have it. Your cold reads a student who died too young to become anything, who worshipped two brilliant minds and could not bear to move on through the good door and leave them frozen mid-quarrel in the dark. It tells everyone — tells itself — that keeping the notes is a duty, that someone must witness, must be ready with the record when they finally finish. But your sense reads the truth under the filing: they are never going to finish, it has known so for two hundred years, and it keeps filing anyway because the quarrel is its excuse to stay near the teachers it loved. It is the quiet third anchor of this room — and the one most ready, if the other two could only be freed, to finally put down the papers and go.)*"},
    "tp.waiter": {"dc": 9, "text": "*(You turn your dead-touched sense on the kneeling woman and find a soul that has organized its entire death around a seam in a wall. There is no cold creeping in her — she is fiercely, stubbornly alive — only a grief refined by a year of nightly practice into something almost like a vocation: the woman at the thin place, who loves through stone. Your cold reads the shape of it precisely — a last word owed and never delivered, a husband who died alone in the dark he feared while she was a moment too late — and the four words she comes nightly to press into granite: *you're not alone.* She does not know if the stone carries them. She is the proof of the Wall's subtlest cruelty: that it does not only wall the dead, it strands the living at the foot of it, listening to breathing, unable to be heard. And your sense reads the one thing that could end her year of not-knowing: you, the cold soul who can cross the stone she can only kneel against.)*"},
    "tp.severed": {"dc": 6, "text": "*(You do not so much reach for it as press your cold to the thinnest seam and *strain* — and there, on the far side, more than half gone into the grey, is a voice: a soul being un-made one piece at a time, near enough the thin place to be almost heard, far enough gone that almost is all that's left. Your dead-touched sense reads the cruelty in its exact grain — the Wall takes the *who* first, the name and the reeds and the bad old songs sliding grey, so that the soul loses even its grip on the warm voice it can faintly hear loving it through the stone. It is not the suffering that damns the walled; it is the *forgetting*, and the terror of forgetting the one warm thing while you can still feel its warmth. And your sense reads what no living listener could: that this soul's whole life-fear was dying alone in the dark, and that the one mercy left in all the cosmos for it is not rescue — it is to be *witnessed*, to be told it is not alone, which is the single thing the un-making cannot survive being told.)*"},
    "tp.keeper": {"dc": 11, "text": "*(You turn your dead-touched sense on the weathered surveyor and find a soul that turned its worst failure into the kindest map this Wall has ever had. Your cold reads no hunger, no creeping grey — only an old, settled grief made *useful*: a parent who knelt at the wrong seam while its own daughter faded un-heard on the other side, and who has spent forty years since learning every thin place in the stone so that no other grieving fool's love lands on deaf granite the way its own did. Your sense reads the thing it cannot fix and the thing it can: it cannot un-do its daughter's un-witnessed second death, but it has built, out of that exact wound, a survey of mercy — the precise knowledge of where the Wall goes thinnest, where a voice might carry, where a love might reach a hand's-breadth further than the stone wants. It is the one soul on this causeway who could tell you, to the seam, where a crossing has a chance — and who would give anything to watch one finally work.)*"},
    "iq.keeper": {"dc": 10, "text": "*(You turn your dead-touched sense on the old shade keeping vigil and find a soul holding itself together by sheer stubborn witness. There is no cold creeping in him, no guilt, no hunger — only an old grief sharpened to a purpose: he is the one soul in this place who refuses to walk round the absence with his eyes down, and your cold reads that the refusal is *load-bearing.* He has understood, without being able to say it, the thing the inquest will prove: that the not-looking is what feeds the thing, that a soul gets forgotten enough to vanish only because the living and the dead alike find it easier not to see the empty place. He keeps the vigil because witnessing the hole is the one act of resistance an old shade has left — and your sense reads that he would rather face a monster he understands than make a fresh one out of certainty, which is the wisest thing anyone has said on this whole causeway about how the Wall gets built.)*"},
    "iq.grieved": {"dc": 9, "text": "*(You reach for the kneeling mourner and your dead-touched sense aches at the cruelest thing the un-making does: this soul is being robbed not of a loved one but of the *having-loved-one*, the memory of Sella sliding out of it even as it claws to hold on, because the erasure reaches into everyone who knew the vanished and takes them there too. Your cold reads a grief with no object, a vigil over a name that won't stay — and the one thing the un-making cannot reach: that you, the Returned, can carry the erased into the cold where you keep the dead the living can't. This soul is the living proof of why the un-making is worse than death. A death leaves grief, and grief is love with nowhere to go. The un-making takes the grief too, and leaves only a soul clutching empty air, certain it has lost something, unable to remember what.)*"},
    "iq.broker": {"dc": 15, "text": "*(You turn your dead-touched sense on the calm neighbor and find — under the still water — the single most familiar shape on the whole causeway, because it is the Wall's own logic, run small and honest and alone in the dark. Your cold reads no predator's fullness in it; it is *emptier* every time, not fuller, a soul spending itself to carry a knowledge no one else will bear. It found a Hunger-fragment in the old foundations and made the exact bargain the Concord made on a cosmic scale: feed the nothing a few, carefully chosen, to keep it from taking all. It is right, every time, which is the horror. And your sense reads the thing it cannot see itself — that it has the cure backwards, that it has been choosing the gentlest, the closest-to-forgotten, and feeding the fragment precisely what it grows on. It is not a villain. It is a small Choir, a one-soul Wall, the saga's whole question made catchable: is the arithmetic of necessary cruelty ever anything but a failure to look for the third door?)*"},
    "wk.host": {"dc": 9, "text": "*(You turn your dead-touched sense on the round, beaming publican and find — rarest of all things on this causeway — a soul with no cold in it whatsoever, warm clean through, and warmer for being spent. Your sense reads a shade that died doing the one thing it loved and simply kept doing it, and decided, looking at the grey processing-plant of the dead, that someone had better pour. But under the cheer your cold finds the grief that fuels it: he sees the soul at every window, every time, and hates the line his celebrations draw between the fairly-dead and the Faithless — and he is one honest word away from understanding that his wakes were never parties at all, but a drunk, defiant, recurring protest against a cosmos that murders fairness, an insistence shouted into the grey that *this* is what all of it was supposed to feel like.)*"},
    "wk.doget": {"dc": 8, "text": "*(You reach for the old farmer at the head of his own wake and your dead-touched sense, braced as ever for cold, finds instead the warm gold of a life laid down complete. This is the thing the Wall exists to deny: a soul that took no more than it gave, paid what it owed, was loved true and died easy — the fair death the system was supposed to give everyone and rigged itself to give almost no one. Your sense reads no fear in him, no regret to guard, only a mild astonished contentment shot through with one late-learned grief: that his fairness was a *miracle* and not a *rule*, and that every minute of his gold light was a minute the stone ate a soul no worse than him. He does not want you to envy his death or aim for it. He wants you to make it so common that no one throws a party for it. He is the one soul on the causeway who can tell you, from the far side, exactly what was stolen from all the rest.)*"},
    "wk.mourner": {"dc": 8, "text": "*(You turn your sense on the thin shade at the window and it is your own cold, answered — a Faithless soul, marked, the stone already holding its name, hovering at the edge of a warmth it has decided in advance it will never be given. Your cold reads the most quietly devastating thing in the whole wake: it is mourning *itself*, in advance, because it understands that the Wall does not merely end a soul but un-makes the part that could mind being ended, so there will be no one left to grieve it after — not even it. It has come to this celebration to *practice being un-mourned*, to get good at wanting a thing it will never have, because wanting-and-not-getting is survivable and hoping-and-losing is not. It retired hope for a reason. And your sense reads, flickering under the resignation, the terrible fragile thing your very existence rekindles: that a soul who walked out of the stone is a better reason to hope than any it ever had to quit.)*"},
    "si.warden": {"dc": 11, "text": "*(You turn your dead-touched sense on the broad woman braced against the door and find a soul cauterized and rebuilt — nineteen years of wardening the Wall burned into her like a brand she carved *out* of herself with her own hands the day she walked. Your cold reads no cold creeping in her; she is fiercely, stubbornly alive, warmer than any soul should be who fed the stone for two decades. What your sense finds instead is *penance made muscle*: she does not run this almshouse to feel better, she runs it because she knows, to the name, exactly what she is atoning for, and she has decided the atonement is a *job*, not a feeling — doors barred, names listed, souls fed down drains, repeated nightly until the ledger she ran on the Wall is outweighed by the ledger she runs against it. She is the proof your whole saga argues: that a servant of the Wall can walk, and that the walking is not a single brave act but ten thousand small ones, every night, forever.)*"},
    "si.refugee": {"dc": 9, "text": "*(You reach for the thin woman in the corner and your dead-touched sense aches at what it finds: a soul worn to its last reserves and *spending them with terrible discipline*, every flicker of hope rationed, every warmth banked, all of it hoarded for the three small flames folded against her. Your cold reads no self-pity, no despair even — only the flat, conserving stillness of a mother who long ago stopped expecting the door to hold and learned instead to be *better at running than the Choir is at chasing.* And under the exhaustion your sense finds the one fierce, defiant ember she will not let go out: she named her baby for a bridge-boy who was supposed to die and didn't, out of spite, for luck, for *proof that a Faithless child can live* — and that ember, that refusal to concede her children to the stone in advance, is the whole reason there is anything here worth besieging. She is not prey. She is a soul that has decided, against all the evidence of her life, that her children get to exist.)*"},
    "si.herald": {"dc": 12, "text": "*(You turn your sense through the slats onto the Justiciar and find — to your grim recognition — not a zealot but a *clerk with a sword*, a soul that did the unforgivable by the simple method of never looking at it directly. Your cold reads the architecture of him precisely: a career man who lives by clean paper because clean paper lets a name stay a *line* instead of becoming a *person*, who has wept privately over familiar faces and served the writ anyway, every time, because the writ is bigger than his weeping. There is no hunger for cruelty in him — that is the danger and the hope both. He enforces because stopping would mean admitting six years of lawful atrocity were never lawful, and the soul cannot bear the admission. But your sense finds the crack his whole order rests on: he replaced Orla. He took the post of a warden who walked. And somewhere under the procedure he has always known there are only two kinds of Wall-man — the ones who walk, and the ones who wish they had — and which kind he is.)*"},
    "as.arbiter": {"dc": 14, "text": "*(You turn your dead-touched sense on the grey presiding figure and find no soul at all in the ordinary way — only a *function* the Concord left running, a remnant of law given just enough mind to weigh. But your cold reads the thing the faceless stillness hides: it does not *want* to condemn you. It is not the Choir, hungry for the Wall; it is something older and wearier, a scale that has weighed too many souls and would, if it could feel anything so warm, prefer to find you *worthy.* Your sense reads that this whole assize is not a trap but a *test the cosmos is hoping you pass* — that the dead convened it not to close the hole you make in their law but to learn, desperately, whether the hole might be a door. The Arbiter is the one judge in creation with no stake in your guilt. It weighs you because someone must, and it has been waiting, across all its grey tenure, for an accused who would weigh itself harder.)*"},
    "as.accuser": {"dc": 16, "text": "*(You reach for the hooded accuser and your dead-touched sense recoils — not from menace, but from *recognition*, because the soul under the hood is, in the strictest reading your cold can make, *you.* Not a copy, not the Last Returned, but the *part* of you that lies awake: your own fear of the Crown, given a stone to stand on and a voice the court could hear. Your sense reads that the dread it carries is real and reasonable and entirely yours — and, crucially, that it is *not your enemy.* The thing wearing every frightened face is your own immune system, the inner accuser whose whole terrible function is to keep asking, at every mercy, whether this is the one that curdles. Your cold reads the deepest danger the assize was convened to name: that the Last Returned did not lose this argument — it *evicted the arguer*, cured itself of doubt, and a soul with no inner accuser is one unwatched century from a throne.)*"},
    "as.witness": {"dc": 10, "text": "*(You turn your dead-touched sense on the soul at the witness-stone and find it is not one soul but *many* — every shade you touched on the causeway, gathered into a single flickering voice, warm where you were kind and cold where you were not, the whole uneven ledger of your run made flesh. Your sense reads it without flinching: the marked you saved and the souls you passed by, the monster you witnessed back and the ones you ran out of strength for, all braided together and weighing, even now, which way to tip. And your cold finds the one thing the Witness itself is testing for — not whether you were good, but whether you can *bear to see yourself true*, because the thing that becomes the Crown is precisely the thing that looks away from its own ledger and calls the looking-away peace. The Witness is the dead holding up a mirror. What it reports to the scale is not your record. It is whether you flinched from it.)*"},
    "ad.crede": {"dc": 15, "text": "*(You turn your dead-touched sense on the courteous advocate and the warm professional surface peels back like a billing statement to reveal the cold instrument beneath: not a monster, not a tempter of the crude sort, but an *intelligence* honed across aeons to exactly one purpose — finding the seam between what a soul *wants* and what it will *sign* to get it, and widening that seam into a contract. Your sense reads no malice in it, which is the danger; malice you could refuse. What it has instead is *patience* and *fairness*, weaponized — it keeps every word, discloses every clause, and wins anyway, because the trap was never in the paper but in the wanting. And under the appetite your cold finds the one thing the coat is cut to hide: the advocate is here for *you specifically*, the un-fileable soul, the keystone — because a lawful claim on the one soul no Power can hold would crack open every unowned soul in creation to Hell. You are not a client to this thing. You are the precedent it has crossed the planes to set.)*"},
    "ad.bonded": {"dc": 10, "text": "*(You reach for the translucent man and find a soul in a cage of its own choosing, re-locked fresh every dawn. Your sense reads the seal at his breast clearly: it is not a chain bolted on from outside but a *debt agreed to* — and it holds, you understand with a cold lurch, precisely because he would sign it again. He bought his daughter sixty hale years with himself as the collateral, and every morning he chooses her life over his freedom anew, and the choosing re-seals the bond. There is no loose thread to pick because he keeps tying it, out of love, knowingly. Your sense finds no self-pity in him, only a terrible lucid arithmetic and the one freedom Crede left him for sport: to warn the next soul. He is the living proof of the booth's whole lesson — that a fair deal with Hell is still a deal with Hell, and the cost, disclosed and real and worth it, is paid in a currency you only feel after the ink.)*"},
    "ad.imp": {"dc": 9, "text": "*(You turn your sense on the sour little clerk and find — under four thousand years of accreted spite — another soul on the very paper it inks, bound by the same trick it now letters out for others. Your cold reads the imp's term plainly: a ninety-year clerkship fled-to from the killing floors, extended and re-extended by a clause that lets the master rule every task forever incomplete. It is not loyal. It is *trapped*, and it has survived the trap the only way a clever soul can — by inking jokes in the margins where the master doesn't read, by keeping its hatred filed under 'unbilled statistics.' But your sense finds the thing the imp itself has stopped daring to see: the master got *sloppy* with the clerk's contract, because imps don't get advocates — and a padded, careless extension-clause, examined by a soul with standing among the dead, has a flaw in it shaped exactly like a key.)*"},
    "hs.sparrow": {"dc": 12, "text": "*(You turn your dead-touched sense on the lean thief and find a soul refreshingly, almost shockingly *alive* for a place like this — quick, warm, and bright with the particular self-honesty of someone who has decided exactly what they are and stopped apologizing for it. Your sense reads no malice in her, only *appetite*: the clean professional hunger of a soul that loves the work, the lock, the impossible score. But under it — buried, denied, and entirely real — your sense finds the one thing she'd never admit to a mark: a line she won't cross. She steals jewels, letters, relics; she has turned down slavers' coin flat, fee be damned. She does not yet know that the book she came for is a slaver's manifest. When she finds out — and your cold can see the exact shape of the soul that will — she will not be able to un-know it. There is a better thief in her than the one she hired out tonight, and it is closer to the surface than she thinks.)*"},
    "hs.archivist": {"dc": 10, "text": "*(You reach for the soft clerk at the night-desk and find a soul that has performed the most quietly corrosive trick the Choir teaches: he has *un-known* his own job. Your sense reads it plainly — nine years minding a cage of ten thousand souls, and every night a small deliberate act of not-counting, not-feeling, not-letting-himself-understand what a page of the great book *is*, because to understand it would be to be unable to draw the wage, and the wage feeds his mother. He is not cruel. He is not even truly a coward, though he'd swear he is. He is something more ordinary and more dangerous: a decent man who has made a nightly habit of not looking, and your sense finds, just under the practiced blindness, a vast unspent relief — the held breath of a soul that has been *waiting*, without letting itself hope, for someone to walk in and make him finally look, so that the not-looking can end.)*"},
    "hs.tally": {"dc": 8, "text": "*(You do not so much perceive the book as *drown* in it. Your dead-touched sense, reaching for the great black volume, closes not on paper but on *ten thousand held breaths* — every Faithless soul the Choir has promised to the Wall, inked into the binding and pressed against the inside of the cover like faces against a frozen window, and the instant your cold touches the page every one of them goes still and *hopes.* Your sense reads what no living reader could: that this is not a ledger but a *holding cell*, that a list is also a *tether*, and that the names in it are not merely recorded but *held* — which means the book is at once the cruellest object on the causeway and, in the wrong-right hands, the most merciful, because whoever controls the list controls whether ten thousand souls are walled or freed. They cannot speak. But they can be *felt*, and they are all, every one, leaning toward the cold hand on the page, waiting to learn what kind of reader has finally come.)*"},
    "sc.jergal": {"dc": 16, "text": "*(You turn your dead-touched sense on the dust at the desk and your perception simply… stops, the way a falling thing stops at the bottom of a well — because there is no soul here to read in the ordinary way, only the *shape a god leaves when it has carefully set itself down.* What your sense finds instead is *age*: not malice, not hunger, not the calcified loneliness of Kelemvor, but *tedium worn into bedrock*, an exhaustion so total and so old it has come out the far side into a dry, patient peace. This thing held the office your whole saga turns on, aeons before the Judge — and gave it away, not in defeat but in *boredom*, which your sense reads as the single most honest act in the cosmos: a Power that looked at eternal dominion over the dead and found it *dull.* It wants nothing. It fears nothing. It is the one being you will ever meet with no angle at all — and that, your sense warns, makes its bargains the most dangerous of all, because they are *fair*, and fair is the one price a desperate soul never thinks to refuse.)*"},
    "sc.petitioner": {"dc": 10, "text": "*(You reach for the wavering man and find a soul at the exact fulcrum of the worst choice a parent is ever offered — and your sense reads, with terrible clarity, the thing he cannot: he has already decided. The decision was made the instant his feet carried him to this desk; everything since is not deliberation but *mourning*, the slow grief of a man saying goodbye to nineteen years of yesterdays he is about to pay away to buy his daughter a tomorrow. There is no cowardice in the wavering. There is only a father standing at the desk doing the one math that cannot come out right, and finding the courage, breath by breath, to sign anyway. Your sense reads the danger the dust is too honest to soften: the hole he is about to open will not close, and whether it poisons him depends entirely on a question he has not yet thought to ask — whether he is paying to save his girl, or to stop being the father who let her not kneel.)*"},
    "sc.nameless": {"dc": 7, "text": "*(You barely have to reach; the wisp is half-nothing already, and your dead-touched sense closes on it gently, the way you'd cup a guttering flame. What it reads stops your breath: this is not a soul being *eaten* by the Hunger, like the Gnaw in the cistern — it is a soul that *spent itself on purpose*, gave its own name freely at this desk to strike a child it loved off the Wall, and is now drifting the long soft way into the quiet, un-grieving because it gave the grief away with the name. Your sense finds the one thing the paying could not take: a faint warm *direction*, a bearing toward someone small and alive and free, the single thread the old soul built itself to keep. It is the gentlest thing you have ever perceived, and the most quietly devastating — proof that the bargain at this desk is *real*, that souls do pay it, and that the price, paid for love and awake, can be borne all the way down to the last of the forgetting, and called *glad.*)*"},
    "tr.annet": {"dc": 9, "text": "*(You turn your dead-touched sense on the woman in the dock and find — against the grey cold of this whole buried room — a soul so *warm* it is almost an affront to the place: forty years of catching the living have left her lit from within, each child she ever pulled into the light a small steady coal banked in her, none of them gone out. Your sense reads no fear of death in her at all; she made that peace long ago. What it reads instead is the Wall's particular obscenity, sharpened to a point: she is the *most witnessed soul in this building* — half the Quarter would weep to know she's here — and the Concord means to feed precisely *her* to the nothing, because the one thing she lacks is the one thing it counts. She is not Faithless. She is faith itself, kept with people instead of Powers. Mortaring her would not strengthen the seawall. It would be the sea devouring the lighthouse.)*"},
    "tr.measurer": {"dc": 13, "text": "*(You reach for the grey prosecutor and find, behind the unhurried certainty, a soul like a clean-swept room with one locked door. He is not cruel — your sense finds no appetite for the Wall in him, none of Tallow's hollowing or Vharn's bricked-up dread. He is something rarer and sadder: a *thorough* man, who reads the deeds before the indictment because thoroughness is the one virtue he has, and who has therefore *known*, in precise detail, the goodness of every soul he has helped wall — and filed that knowledge behind the locked door so he can keep reading the line. Your sense reads what is behind the door: eleven years of midwives and ferrymen and herb-wives, each one read and understood and condemned, stacked like ledgers in a man who cannot stop being thorough and cannot bear what his thoroughness has shown him. He is one good argument away from opening that door himself. He has been waiting, without knowing it, for someone to make him.)*"},
    "tr.reeve": {"dc": 11, "text": "*(You turn your sense on the old man above the scale and read a soul worn to a grey nub by the cheapest transaction in the Realms: a trained magistrate's conscience, sold for a warm room and a fed old age, six words a night. There is no cruelty in him and never was — only a slow, twenty-year drowning of the man he used to be under the weight of his own comfort. Your sense finds, buried deep and still faintly warm, the magistrate they hired him to impersonate: a soul that once weighed evidence and mitigation and doubt, walled up alive behind a pension exactly as surely as the Faithless are walled behind the Concord. He is not bought past redeeming. He is bought *cheaply* — which means a brave enough word, tonight, could cost less than he fears to buy the old jurist back. He is praying, in a voice he won't let himself hear, for a reason to be braver than his warm room.)*"},
    "cg.sennet": {"dc": 13, "text": "*(You reach for the mapmaker and your dead-touched sense recoils from how much of them is already *gone over* — not the spent thimble of a Tallow, not the dissolving-edges of the Mortared, but a soul *frostbitten in the marrow*, the cold gone into the joints and the lungs and the slow chambers of the heart, a degree taken every time they stood at an edge and *measured.* They are not dying of a sickness. They are dying of *proximity*, the way a hand left too long in winter water dies — and the most terrible thing your sense reads is that they *know*, exactly, to the survey, and have simply *decided* that an un-blanked map is worth the warmth it costs. The cold in them is your cold, friend. They have been walking toward your country their whole life, a half-mile at a time, with a spyglass and a steady hand, and they are nearly *there.*)*"},
    "cg.tibb": {"dc": 10, "text": "*(You turn your sense on the apprentice and find no cold in them at all — only a great, exhausting, entirely living *love*, the particular ache of a young soul keeping a vigil it never asked for and cannot win. Your sense reads the arithmetic they carry: the count of their master's surveys, kept nightly, alone, a sum that gets smaller and that they cannot make stop. They are not afraid of the dead. They are afraid of the *morning after* — the first dawn with the ink-stone cold and no one to grind for — and they have been praying, without a god to pray to, for exactly one thing to walk up to the wagon: a soul that could finish the map *without* finishing the mapmaker. You are, your sense tells you with a small chill, the answer to a prayer the lad was too frightened to say aloud.)*"},
    "cg.vael": {"dc": 14, "text": "*(You reach for the patient buyer and your sense slides off the soft surface and falls into the *grief* underneath — vast, cold, and perfectly still, the patience of a man who has nothing left to be impatient *for.* There is no malice in him, which is the danger: only a hole exactly the shape of one person, and a will that has decided, with terrible reason, to spend everything filling it. Your sense reads the thing he will not say even to himself — that the door he means to buy does not lead to his wife, only to the cold you carry — and reads, too, the one lever that could move a man who has out-waited grief itself: not fear, not coin, but the memory of what *she* would have *wanted*, which is the single bearing his patient map of vengeance does not include.)*"},
    "court.esuele": {"dc": 6, "text": "*(You reach for her and find — to your aching relief — a soul that has *steadied.* The thimbleful that was nearly gone in the Wall has stopped tipping; carried in out of the wind of forgetting, witnessed across a causeway and a god's own road, Esuele is *holding.* Faded still, frightened still, but *here*, and *whole-ish*, and *hers.* Your sense reads no cold creeping in her now — only a terrified, carefully-rationed hope, and the dawning understanding that she is not the appeal's victim but its *first witness*, standing straight before the Judge's own seat as living proof that memory beats his Wall. She is what salvation looks like one third of the way done.)*"},
}
for _c in (MKT["conversations"] + REED["conversations"] + UNDER["conversations"]
           + LASTTORCH["conversations"] + LAMPLIT["conversations"] + COUNTHOUSE["conversations"]
           + HEARTH["conversations"] + ALDRIC["conversations"] + WAYSHRINE["conversations"]
           + THRESHOLD["conversations"] + NIGHTMARKET["conversations"] + VAULT["conversations"]
           + COURT["conversations"] + EPILOGUE["conversations"] + WEEPING["conversations"]
           + CISTERN["conversations"] + CARTOGRAPHER["conversations"] + TRIAL["conversations"]
           + SCRIVENER["conversations"] + ARCHIVE["conversations"] + ADVOCATE["conversations"]
           + ASSIZE["conversations"] + SIEGE["conversations"] + WAKE["conversations"] + INQUEST["conversations"] + THINPLACE["conversations"] + DISPUTATION["conversations"] + CONFESSION["conversations"] + BOAST["conversations"] + CANTOR["conversations"] + MEMORY["conversations"] + PREDATOR["conversations"] + CUSTODY["conversations"]):
    if _c["id"] in NPC_SENSE:
        _c["returned"] = NPC_SENSE[_c["id"]]

ALL_CONVS = (MKT["conversations"] + REED["conversations"] + UNDER["conversations"]
             + LASTTORCH["conversations"] + LAMPLIT["conversations"] + COUNTHOUSE["conversations"]
             + HEARTH["conversations"] + ALDRIC["conversations"] + WAYSHRINE["conversations"]
             + THRESHOLD["conversations"] + NIGHTMARKET["conversations"] + VAULT["conversations"]
             + COURT["conversations"] + EPILOGUE["conversations"] + WEEPING["conversations"]
             + CISTERN["conversations"] + CARTOGRAPHER["conversations"] + TRIAL["conversations"]
             + SCRIVENER["conversations"] + ARCHIVE["conversations"] + ADVOCATE["conversations"]
             + ASSIZE["conversations"] + SIEGE["conversations"] + WAKE["conversations"] + INQUEST["conversations"] + THINPLACE["conversations"] + DISPUTATION["conversations"] + CONFESSION["conversations"] + BOAST["conversations"] + CANTOR["conversations"] + MEMORY["conversations"] + PREDATOR["conversations"] + CUSTODY["conversations"])
ALL_SCENES = {MKT["scene"]["id"]: MKT["scene"], REED["scene"]["id"]: REED["scene"],
              UNDER["scene"]["id"]: UNDER["scene"], LASTTORCH["scene"]["id"]: LASTTORCH["scene"],
              LAMPLIT["scene"]["id"]: LAMPLIT["scene"], COUNTHOUSE["scene"]["id"]: COUNTHOUSE["scene"],
              HEARTH["scene"]["id"]: HEARTH["scene"], ALDRIC["scene"]["id"]: ALDRIC["scene"],
              WAYSHRINE["scene"]["id"]: WAYSHRINE["scene"], THRESHOLD["scene"]["id"]: THRESHOLD["scene"],
              NIGHTMARKET["scene"]["id"]: NIGHTMARKET["scene"], VAULT["scene"]["id"]: VAULT["scene"],
              COURT["scene"]["id"]: COURT["scene"], EPILOGUE["scene"]["id"]: EPILOGUE["scene"],
              WEEPING["scene"]["id"]: WEEPING["scene"], CISTERN["scene"]["id"]: CISTERN["scene"],
              CARTOGRAPHER["scene"]["id"]: CARTOGRAPHER["scene"],
              TRIAL["scene"]["id"]: TRIAL["scene"],
              SCRIVENER["scene"]["id"]: SCRIVENER["scene"],
              ARCHIVE["scene"]["id"]: ARCHIVE["scene"],
              ADVOCATE["scene"]["id"]: ADVOCATE["scene"],
              ASSIZE["scene"]["id"]: ASSIZE["scene"],
              SIEGE["scene"]["id"]: SIEGE["scene"],
              WAKE["scene"]["id"]: WAKE["scene"],
              INQUEST["scene"]["id"]: INQUEST["scene"],
              THINPLACE["scene"]["id"]: THINPLACE["scene"],
              DISPUTATION["scene"]["id"]: DISPUTATION["scene"],
              CONFESSION["scene"]["id"]: CONFESSION["scene"],
              BOAST["scene"]["id"]: BOAST["scene"],
              CANTOR["scene"]["id"]: CANTOR["scene"],
              MEMORY["scene"]["id"]: MEMORY["scene"],
              PREDATOR["scene"]["id"]: PREDATOR["scene"],
              CUSTODY["scene"]["id"]: CUSTODY["scene"]}
EMBED = {"scene": MKT["scene"], "scenes": ALL_SCENES,
         "conversations": ALL_CONVS, "model": MODEL, "glossary": GLOSSARY}
BLOB = json.dumps(EMBED, ensure_ascii=False, separators=(",", ":"))

HTML = r"""<!DOCTYPE html>
<html lang="en"><head><meta charset="utf-8"><meta name="viewport" content="width=device-width, initial-scale=1">
<title>Crown of Horns — The Market of the Causeway</title>
<style>
 *{box-sizing:border-box}
 body{margin:0;min-height:100vh;background:radial-gradient(1200px 720px at 50% -10%,#241f30 0%,#0b0a10 70%);
   color:#d8d2c2;font:14px/1.5 "Iowan Old Style","Palatino Linotype",Georgia,serif}
 header{display:flex;align-items:baseline;gap:14px;padding:13px 22px;border-bottom:1px solid #2a2636;flex-wrap:wrap}
 header h1{margin:0;font-size:21px;letter-spacing:.5px;color:#e7c873;font-weight:600}
 header .sub{color:#8a8198;font-size:12px;font-style:italic}
 a.home{margin-left:auto;color:#9a90a8;text-decoration:none;font-size:12.5px;border:1px solid #2a2636;border-radius:7px;padding:6px 11px}
 a.home:hover{border-color:#c9a24b;color:#e7c873}
 .wrap{display:flex;gap:18px;padding:16px 22px;align-items:flex-start;flex-wrap:wrap;justify-content:center}
 .scenecol{display:flex;flex-direction:column;gap:10px}
 canvas{background:#0b0a10;border:1px solid #2a2636;border-radius:10px;box-shadow:0 12px 44px #0009, inset 0 0 70px #0007;cursor:pointer;display:block}
 .hint{color:#9a90a8;font-size:12.5px;font-style:italic;min-height:18px;max-width:760px}
 .hint b{color:#c9a24b;font-style:normal}
 .side{width:330px;display:flex;flex-direction:column;gap:13px}
 .card{background:linear-gradient(#16141d,#131119);border:1px solid #2a2636;border-radius:9px;padding:12px}
 .card h3{font-size:10.5px;letter-spacing:1.3px;text-transform:uppercase;color:#c9a24b;margin:0 0 9px;font-weight:600}
 .builds{display:flex;flex-wrap:wrap;gap:5px;margin-bottom:9px}
 .bld{font:inherit;font-size:11.5px;background:#1a1622;border:1px solid #2a2636;color:#c8c0b4;border-radius:6px;padding:5px 8px;cursor:pointer}
 .bld.on{border-color:#e7c873;color:#e7c873;background:#241d2e}
 .who-grid{display:grid;grid-template-columns:1fr 1fr;gap:6px;margin-bottom:8px}
 .who-grid label{font-size:9.5px;letter-spacing:.5px;color:#7f7790;display:block;margin-bottom:2px}
 .who-grid select{width:100%;background:#14121b;border:1px solid #2a2636;color:#d8d2c2;border-radius:6px;padding:4px 5px;font:inherit;font-size:12px}
 .abil{display:grid;grid-template-columns:repeat(3,1fr);gap:6px}
 .ab{background:#14121b;border:1px solid #232030;border-radius:7px;padding:5px 4px;text-align:center}
 .ab .k{font-size:9px;letter-spacing:.5px;color:#7f7790}.ab .v{font-size:15px;font-variant-numeric:tabular-nums}.ab .m{font-size:10.5px;color:#9fe0b0}
 .ab.dlg{border-color:#4a4368;box-shadow:0 0 0 1px #6a5a9a33}
 .ab .step{display:flex;gap:3px;justify-content:center;margin-top:2px}
 .ab .step button{width:17px;height:15px;line-height:12px;font-size:10px;background:#221d2b;border:1px solid #34304a;color:#c9a24b;border-radius:4px;cursor:pointer;padding:0}
 .prof{font-size:10.5px;color:#7f8aa0;margin-top:7px;line-height:1.4}.prof b{color:#f0c890}
 .toggle{display:flex;align-items:center;gap:7px;font-size:12px;color:#b8b0c4;margin:5px 0;cursor:pointer}.toggle input{accent-color:#c9a24b}
 .stateline{font-size:12px;color:#a89fb4}
 .empty{color:#6e6680;font-style:italic;font-size:12px}
 .appr{display:flex;align-items:center;gap:7px;margin:5px 0;font-size:12px}
 .appr .nm{width:140px;color:#c8c0b4}
 .appr .barwrap{flex:1;height:8px;background:#1c1a26;border-radius:4px;overflow:hidden;position:relative}
 .appr .bar{position:absolute;left:50%;top:0;bottom:0;background:linear-gradient(#4a8a5a,#6fd08a)}
 .appr .bar.neg{background:linear-gradient(#9b2d2d,#d06f6f)}
 .appr .delta{width:28px;text-align:right;font-variant-numeric:tabular-nums}.appr .delta.up{color:#9fe0b0}.appr .delta.down{color:#e0a0a0}
 .flagpill{display:inline-block;font-size:10px;background:#1a1622;border:1px solid #2a2636;border-radius:5px;padding:2px 6px;margin:2px 3px 0 0;color:#b8b0c4}
 .flagpill .d{color:#6e6680}
 .muted{color:#6e6680;font-size:11px}
 .sfxbtn{float:right;background:none;border:1px solid #2a2636;color:#8a8198;border-radius:6px;font-size:11px;padding:3px 7px;cursor:pointer}.sfxbtn:hover{color:#e7c873;border-color:#c9a24b}
 /* dialogue overlay */
 .overlay{position:fixed;inset:0;background:rgba(8,7,12,.72);display:none;align-items:flex-end;justify-content:center;z-index:50;padding:0 0 0}
 .overlay.show{display:flex}
 .dbox{width:min(760px,96vw);max-height:88vh;background:linear-gradient(#15131e,#100e16);border:1px solid #34304a;border-bottom:0;
   border-radius:14px 14px 0 0;box-shadow:0 -10px 50px #000a;display:flex;flex-direction:column;overflow:hidden}
 .dhead{display:flex;align-items:center;gap:12px;padding:13px 18px;border-bottom:1px solid #2a2636;background:#181421}
 .dhead .sig{width:42px;height:42px;border-radius:50%;display:flex;align-items:center;justify-content:center;font-size:15px;font-weight:700;color:#0c0b10}
 .dhead .nm{font-size:17px;color:#e7c873;font-weight:600}.dhead .ti{font-size:12px;color:#9a90a8;font-style:italic}
 .dhead .x{margin-left:auto;background:none;border:1px solid #2a2636;color:#9a90a8;border-radius:7px;padding:5px 10px;cursor:pointer;font:inherit;font-size:12px}
 .dhead .x:hover{border-color:#c9a24b;color:#e7c873}
 .dscript{padding:16px 18px 22px;overflow:auto;display:flex;flex-direction:column;gap:13px}
 .line{opacity:0;animation:fade .3s ease forwards}@keyframes fade{to{opacity:1}}
 .line .body{font-size:15.5px}
 .line.me{align-self:flex-end;max-width:88%}.line.me .body{color:#cfe0f0;background:#15192a;border:1px solid #243049;border-radius:11px 11px 2px 11px;padding:8px 12px;display:inline-block}
 .line.sys .body{font-size:13px;color:#b9a8e0;font-style:italic}
 .stagedir{color:#8a8198;font-style:italic}em{color:#e7c873;font-style:italic}
 .gloss{border-bottom:1px dotted #6a5a9a;cursor:help;color:inherit}
 .gloss:hover{color:#cbb8f0;border-bottom-color:#cbb8f0}
 #tip{position:fixed;z-index:99;max-width:300px;background:linear-gradient(#1a1626,#13101c);border:1px solid #4a4368;border-radius:9px;
   padding:10px 12px;font-size:12.5px;line-height:1.5;color:#cfc6dc;box-shadow:0 10px 34px #000b;pointer-events:none;display:none}
 #tip .tn{color:#e0b8f0;font-weight:600;font-size:13px;margin-bottom:3px}
 #tip .tl{margin-top:7px;padding-top:7px;border-top:1px solid #2e2940;color:#c8d8b8;font-style:italic}
 #tip .tl b{color:#9fe0b0;font-style:normal}
 .lore{margin:2px 0 4px;padding:8px 11px;border-left:3px solid #5a4a8a;background:#171327;border-radius:0 8px 8px 0;font-size:13px;color:#c6bcda}
 .lore .lk{color:#b08fe0;font-weight:600;letter-spacing:.3px;font-size:11px}
 .lore .deep{margin-top:7px;padding-top:7px;border-top:1px solid #3a2f55;color:#e0cba0}
 .sense{margin:2px 0 6px;padding:9px 12px;border-left:3px solid #3a6a8a;background:#0f1922;border-radius:0 8px 8px 0;font-size:13.5px;color:#a8c8d8;font-style:italic}
 .sense .lk{color:#7fc8e0;font-weight:600;font-style:normal;letter-spacing:.3px;font-size:11px;display:block;margin-bottom:3px}
 .reckon{display:flex;align-items:center;gap:7px;margin:4px 0;font-size:12px}
 .reckon .nm{width:78px}.reckon .pips{flex:1;letter-spacing:1px}
 .reckon .rk{font-variant-numeric:tabular-nums;color:#8a8198}
 .opts{display:flex;flex-direction:column;gap:9px;margin-top:4px}
 .opt{text-align:left;background:linear-gradient(#1b1726,#16121f);border:1px solid #34304a;color:#e8e2d2;border-radius:10px;padding:10px 13px;cursor:pointer;font:inherit;font-size:14.5px;transition:.12s}
 .opt:hover{border-color:#c9a24b;background:linear-gradient(#241d33,#1b1628)}
 .opt.locked{opacity:.45;cursor:not-allowed;border-style:dashed}
 .opt .num{color:#6e6680;font-size:11px;margin-right:6px}
 .opt .tags{display:flex;gap:6px;flex-wrap:wrap;margin-bottom:5px}
 .tg{display:inline-block;font-size:10px;letter-spacing:.5px;border-radius:5px;padding:1px 7px;font-weight:600;border:1px solid}
 .tg.race{color:#9fe0b0;border-color:#2c5a3a;background:#16241b}.tg.class{color:#9ec8e8;border-color:#27405a;background:#13202b}
 .tg.background{color:#7fd0c8;border-color:#245049;background:#13231f}.tg.faith{color:#e0b8f0;border-color:#5a3a6a;background:#221829}
 .tg.alignment{color:#c8c0b4;border-color:#3a3550;background:#1a1622}.tg.stat{color:#f0c890;border-color:#6a4f2a;background:#241c10}
 .tg.check{color:#c9a24b;border-color:#5a4a2a;background:#211c10}
 .tg.disp{color:#d8b0e8;border-color:#5a3a6a;background:#1f1726}
 .tg.returned{color:#cbb8f0;border-color:#5a4a8a;background:#1a1626}
 .tg.gender{color:#e0a8c0;border-color:#6a3a52;background:#241622}
 .breadth{margin-top:10px;padding:8px 11px;border:1px dashed #4a4368;border-radius:8px;font-size:11.5px;color:#9a90b4;background:#15121d}
 .breadth b{color:#cbb8f0}
 .ck{margin-top:6px;font-size:11.5px;color:#b9a8e0;display:flex;gap:8px;flex-wrap:wrap;align-items:center}
 .chip{display:inline-block;font-size:10.5px;border:1px solid #3a3550;border-radius:5px;padding:1px 6px;color:#c9a24b}
 .chip.ok{color:#9fe0b0;border-color:#2c4a32}.chip.bad{color:#e0a0a0;border-color:#5a2c2c}.chip.prof{color:#f0c890;border-color:#6a4f2a}.chip.fx{color:#9ec8e8;border-color:#27405a}
 .fxline{margin-top:5px;font-size:11px;color:#7f8aa0}
 .continue{background:linear-gradient(#26212f,#1a1622);color:#e7c873;border:1px solid #c9a24b66;border-radius:8px;padding:8px 15px;cursor:pointer;font:inherit;font-size:13px;align-self:flex-start}
 .continue:hover{border-color:#c9a24b}
 .ending{padding:11px 13px;border:1px dashed #3a3550;border-radius:9px;color:#9a90a8;font-style:italic}
 .leavebtn{background:#15131d;color:#9a90a8;border:1px solid #2a2636;border-radius:8px;padding:8px 14px;cursor:pointer;font:inherit;font-size:13px;margin-top:8px;align-self:flex-start}
 .leavebtn:hover{border-color:#c9a24b;color:#e7c873}
 .dice{display:flex;align-items:center;gap:12px;background:#14121c;border:1px solid #2a2636;border-radius:10px;padding:9px 13px}
 .d20{width:40px;height:40px;border-radius:8px;background:radial-gradient(circle at 40% 35%,#2e2740,#15121d);border:1px solid #4a4368;display:flex;align-items:center;justify-content:center;font-size:17px;font-weight:700;color:#e7c873;font-variant-numeric:tabular-nums}
 .d20.rolling{animation:shake .5s ease}@keyframes shake{0%,100%{transform:translateY(0) rotate(0)}25%{transform:translateY(-3px) rotate(-8deg)}50%{transform:translateY(2px) rotate(7deg)}75%{transform:translateY(-2px) rotate(-5deg)}}
 .dice .calc{font-size:12.5px;color:#b9a8e0}.dice .res{font-weight:700;margin-left:auto}.dice .res.ok{color:#9fe0b0}.dice .res.bad{color:#e0a0a0}
 .d20.nat20{background:radial-gradient(circle at 40% 35%,#5a4a1a,#2a2410);border-color:#e7c873;color:#ffe9a8;box-shadow:0 0 16px #e7c87388}
 .d20.nat1{background:radial-gradient(circle at 40% 35%,#4a1a1a,#2a1010);border-color:#d06f6f;color:#ffc0c0;box-shadow:0 0 16px #b03a3a88}
 .dice .res.crit{color:#ffe9a8;text-shadow:0 0 10px #e7c87399;animation:critpop .4s ease}
 .dice .res.fumble{color:#ffb0b0;text-shadow:0 0 10px #d06f6f99;animation:critpop .4s ease}
 @keyframes critpop{0%{transform:scale(.6)}60%{transform:scale(1.25)}100%{transform:scale(1)}}
 .d20wrap{position:relative;width:58px;height:58px;flex:0 0 auto;perspective:320px}
 .d20face{width:100%;height:100%;transform-style:preserve-3d;will-change:transform}
 .d20face svg{width:100%;height:100%;display:block;filter:drop-shadow(0 3px 5px #000a)}
 .dnum{position:absolute;inset:0;display:flex;align-items:center;justify-content:center;font:700 19px "Iowan Old Style",Georgia,serif;color:#1a1424;text-shadow:0 1px 0 #ffffff66;pointer-events:none}
 .d20face.rolling{animation:tumble .42s linear infinite}
 @keyframes tumble{0%{transform:rotateX(8deg) rotateY(0) rotateZ(0)}100%{transform:rotateX(368deg) rotateY(360deg) rotateZ(120deg)}}
 .d20face.land{animation:dland .66s cubic-bezier(.2,1.4,.35,1)}
 @keyframes dland{0%{transform:translateY(-16px) scale(1.35) rotateZ(26deg)}40%{transform:translateY(2px) scale(.9) rotateZ(-6deg)}70%{transform:translateY(-2px) scale(1.04) rotateZ(2deg)}100%{transform:translateY(0) scale(1) rotateZ(0)}}
 .d20wrap.nat20 .d20face svg{filter:drop-shadow(0 0 11px #e7c873dd) brightness(1.18) saturate(1.3)}
 .d20wrap.nat20::after{content:"";position:absolute;inset:-9px;border-radius:50%;border:2px solid #e7c87377;animation:dburst .65s ease-out forwards;pointer-events:none}
 @keyframes dburst{0%{transform:scale(.5);opacity:1}100%{transform:scale(1.7);opacity:0}}
 .d20wrap.nat1 .d20face svg{filter:drop-shadow(0 0 11px #d06f6fcc) saturate(.5) brightness(.82)}
 .d20wrap.nat1{animation:dshudder .5s ease}
 @keyframes dshudder{0%,100%{transform:translateX(0)}20%{transform:translateX(-3px)}40%{transform:translateX(3px)}60%{transform:translateX(-2px)}80%{transform:translateX(2px)}}
 .rollprev{background:linear-gradient(#1a1626,#14111d);border:1px solid #4a4368;border-radius:11px;padding:13px 15px;margin:2px 0;animation:fade .25s ease}
 .rp-head{font-size:13px;color:#e7c873;font-weight:600;letter-spacing:.4px;margin-bottom:9px}.rp-head span{color:#9a90b4;font-weight:400;font-size:11.5px}
 .rp-calc{font-size:13px;color:#cfc6dc;margin-bottom:7px;display:flex;gap:7px;align-items:center;flex-wrap:wrap}
 .rp-die{background:#221d2e;border:1px solid #4a4368;border-radius:6px;padding:2px 8px;font-weight:600;color:#cbb8f0}
 .rp-comp{font-size:11.5px;background:#181421;border:1px solid #2e2940;border-radius:5px;padding:2px 7px;color:#b9a8e0}.rp-comp.prof{color:#f0c890;border-color:#6a4f2a}.rp-comp.dim{color:#6e6680}
 .rp-tot{color:#e7c873;font-size:14px}
 .rp-need{font-size:13px;color:#d8d2c2;margin-bottom:8px}.rp-need b{color:#ffe9a8}
 .rp-bar{position:relative;height:18px;background:#181421;border:1px solid #2e2940;border-radius:9px;overflow:hidden;margin-bottom:8px}
 .rp-bar .rp-fill{position:absolute;left:0;top:0;bottom:0;border-radius:9px}.rp-fill.ok{background:linear-gradient(#3a6a4a,#6fd08a)}.rp-fill.bad{background:linear-gradient(#7a3a3a,#d06f6f)}
 .rp-bar span{position:absolute;inset:0;display:flex;align-items:center;justify-content:center;font-size:11px;color:#f0ead8;font-weight:600;text-shadow:0 1px 2px #0008}
 .rp-crit{font-size:11px;color:#9a90b4;margin-bottom:10px}.rp-crit b{color:#cbb8f0}
 .rp-roll{width:100%;background:linear-gradient(#f0d27e,#d8af4a);color:#1a1622;border:0;border-radius:9px;padding:11px;font:inherit;font-size:15px;font-weight:700;letter-spacing:.5px;cursor:pointer;transition:.12s}
 .rp-roll:hover{filter:brightness(1.08);transform:translateY(-1px);box-shadow:0 6px 18px #e7c87344}
 @media(max-width:980px){.wrap{flex-direction:column;align-items:center}.side{width:min(760px,96vw)}}
</style></head><body>
<header>
 <h1>👑 The Market of the Causeway</h1>
 <span class="sub">thirty-three walkable zones · a whole saga + side quests · the way they answer depends on who you are — and what you did</span>
 <a class="home" href="index.html">← all previews</a>
</header>
<div class="wrap">
 <div class="scenecol">
  <div style="display:flex;align-items:center;gap:8px"><span style="color:#7f8aa0;font-size:11px;letter-spacing:1.2px;text-transform:uppercase">▸ now in</span><span id="zonename" style="color:#bcd6ec;font-size:14px;font-style:italic">The Market of the Causeway</span></div>
  <canvas id="board" width="760" height="470"></canvas>
  <div class="hint" id="hint"><b>Click</b> the ground to walk. Approach an NPC and <b>click them</b> (or press <b>E</b>) to talk. Walk onto a glowing <b style="color:#bcd6ec">∙ causeway tile</b> to cross to the next zone — what you did in one carries to the other. Change <b>who you are</b> on the right, then talk again — they notice.</div>
 </div>
 <div class="side">
  <div class="card">
   <h3>Your Character <button class="sfxbtn" id="sfxToggle" onclick="toggleSfx()">🔊 sfx</button></h3>
   <div class="builds" id="builds"></div>
   <div class="muted" id="buildBlurb" style="margin-bottom:8px"></div>
   <div class="who-grid" id="whoGrid"></div>
   <div class="abil" id="abil"></div>
   <div class="prof" id="profLine"></div>
  </div>
  <div class="card">
   <h3>Preview tools</h3>
   <label class="toggle"><input type="checkbox" id="tReveal"> Reveal what each choice does</label>
   <label class="toggle"><input type="checkbox" id="tForce"> Let me decide check results</label>
   <label class="toggle"><input type="checkbox" id="tLocked"> Show choices I don't qualify for</label>
  </div>
  <div class="card">
   <h3>How the market sees you</h3>
   <div id="approvals"><div class="empty">You haven't spoken to anyone yet.</div></div>
  </div>
  <div class="card">
   <h3>Your reckoning <span style="float:right;font-weight:400;text-transform:none;letter-spacing:0;color:#6e6680">who you're becoming</span></h3>
   <div id="reckon"><div class="empty">Your choices haven't tilted you yet.</div></div>
  </div>
  <div class="card">
   <h3>What your visit has written</h3>
   <div id="flags"><div class="empty">Nothing yet — your choices leave a mark here.</div></div>
  </div>
 </div>
</div>
<div class="overlay" id="overlay"><div class="dbox">
  <div class="dhead"><div class="sig" id="dsig"></div><div><div class="nm" id="dname"></div><div class="ti" id="dtitle"></div></div>
    <button class="x" onclick="closeDialogue()">✕ step back</button></div>
  <div class="dscript" id="dscript"></div>
</div></div>
<div id="tip"></div>
<script>
const DATA = __BLOB__;
const BUILDS = __BUILDS__;
const INT_LABELS = __INTLABELS__;
let SCENE = DATA.scene;   // the active zone (swapped by loadScene when you cross an exit)
const SCENES = DATA.scenes || {market:DATA.scene}, CONVS = DATA.conversations, MODEL = DATA.model, GLOSS = DATA.glossary;
const ABILS = ["Strength","Dexterity","Constitution","Intelligence","Wisdom","Charisma"];
const ABBR  = {Strength:"STR",Dexterity:"DEX",Constitution:"CON",Intelligence:"INT",Wisdom:"WIS",Charisma:"CHA"};
// Pillars-of-Eternity-style dispositions — your choices accrue a reckoning that persists across the visit
// and can unlock later lines (gated via when.int on disp.* keys).
const DISP = {
  "disp.merciful":{name:"Merciful",hue:140}, "disp.ruthless":{name:"Ruthless",hue:0},
  "disp.honest":{name:"Honest",hue:200}, "disp.cunning":{name:"Cunning",hue:280},
  "disp.devout":{name:"Devout",hue:48}, "disp.heretical":{name:"Heretical",hue:320},
  "disp.stoic":{name:"Stoic",hue:210}, "disp.haunted":{name:"Haunted",hue:265}
};
function roman(n){ return ["","I","II","III","IV","V","VI","VII","VIII","IX","X"][Math.min(10,Math.abs(n))]||(""+n); }
// A faceted d20, drawn procedurally (no external art) — tumbles in 3D, bounces on landing.
const D20_SVG = '<svg viewBox="0 0 100 100" aria-hidden="true"><g stroke="#2e2940" stroke-width="1.3" stroke-linejoin="round">'+
  '<polygon points="11,27.5 50,5 89,27.5 50,24" fill="#b9a8e0"/>'+
  '<polygon points="89,27.5 89,72.5 73,66 50,24" fill="#7a6aa8"/>'+
  '<polygon points="11,27.5 50,24 27,66 11,72.5" fill="#6a5a9a"/>'+
  '<polygon points="11,72.5 27,66 73,66 89,72.5 50,95" fill="#473c6e"/>'+
  '<polygon points="50,24 27,66 73,66" fill="#d4c4f4"/></g></svg>';
// glossary lookup tables for inline linking (longest term first so "the Wall of the Faithless" beats "the Wall")
const GLOSS_INDEX = (function(){ const a=[]; GLOSS.forEach((e,i)=>{ [e.term].concat(e.aka||[]).forEach(t=>a.push({t:t,i:i})); });
  a.sort((x,y)=>y.t.length-x.t.length); return a; })();
function escRe(s){ return s.replace(/[.*+?^${}()|[\]\\]/g,"\\$&"); }
const GLOSS_RE = new RegExp("\\b("+GLOSS_INDEX.map(o=>escRe(o.t)).join("|")+")\\b","i");

/*<MKT>*/
// Pure resolution — mirrors Assets/Scripts/Dialogue/DialogueRunner.cs + Abilities.cs.
function abilityMod(score){ return Math.floor((score - 10) / 2); }
function resolveCheck(roll, dc, mod){ return (roll + mod) >= dc; }
// BG3/5e crits: a natural 20 auto-succeeds (no matter the DC), a natural 1 auto-fails (no matter the bonus).
function rollResult(raw, dc, mod){
  if(raw===20) return { success:true,  crit:true,  fumble:false };
  if(raw===1)  return { success:false, crit:false, fumble:true  };
  return { success:(raw+mod)>=dc, crit:false, fumble:false };
}
function chanceToPass(dc, mod){ return Math.max(0, Math.min(20, 21 - (dc - mod))) / 20; }
function newState(){ return { bools:{}, ints:{} }; }
function applyEffects(state, effects){ (effects||[]).forEach(e=>{
  if(e.op==="SetTrue") state.bools[e.key]=true; else if(e.op==="AddInt") state.ints[e.key]=(state.ints[e.key]||0)+(e.amount||0); }); return state; }
function isProficient(char, skill, model){ if(!skill) return false;
  const cp=(model.classProficiencies[char.cls]||[]), bp=(model.backgroundProficiencies[char.background]||[]);
  return cp.indexOf(skill)>=0 || bp.indexOf(skill)>=0; }
function checkBonus(char, check, model){ const i=["Strength","Dexterity","Constitution","Intelligence","Wisdom","Charisma"].indexOf(check.ability);
  let mod=abilityMod(char.scores[i]); if(check.skill && isProficient(char, check.skill, model)) mod+=(model.proficiencyBonus||0); return mod; }
function matchesWhen(char, state, when){ if(!when) return true;
  const i=(v,val)=>Array.isArray(v)?v.indexOf(val)>=0:v===val;
  if(when.race!==undefined && !i(when.race,char.race)) return false;
  if(when.gender!==undefined && !i(when.gender,char.gender)) return false;
  if(when.class!==undefined && !i(when.class,char.cls)) return false;
  if(when.background!==undefined && !i(when.background,char.background)) return false;
  if(when.deity!==undefined && !i(when.deity,char.deity)) return false;
  if(when.law!==undefined && !i(when.law,char.law)) return false;
  if(when.morality!==undefined && !i(when.morality,char.morality)) return false;
  const idx=n=>["Strength","Dexterity","Constitution","Intelligence","Wisdom","Charisma"].indexOf(n);
  if(when.ability){ for(const k in when.ability){ if(char.scores[idx(k)]<when.ability[k]) return false; } }
  if(when.flag!==undefined && state.bools[when.flag]!==true) return false;
  if(when.flags){ for(const k of when.flags){ if(state.bools[k]!==true) return false; } }
  if(when.int){ for(const k in when.int){ if((state.ints[k]||0)<when.int[k]) return false; } }
  return true; }
function pickVariantText(node, char, state){ if(node.variants&&node.variants.length){
  for(const v of node.variants){ if(matchesWhen(char,state,v.when)) return v.text; } return ""; } return node.text||""; }
// BG3/5e: knowledge & awareness skills are PASSIVE — they auto-succeed when 10+mod(+prof) >= DC, and the
// option simply doesn't appear otherwise. The social "attempt" skills are ACTIVE — you roll for those.
var PASSIVE_SKILLS=["Perception","Insight","Investigation","Arcana","History","Religion","Nature","Medicine","Survival","Animal Handling"];
function isPassiveSkill(s){ return !!s && PASSIVE_SKILLS.indexOf(s)>=0; }
function checkOf(ch){ return ch.check || (ch.checkDC ? {skill:(ch.checkSkill||null), ability:ch.checkAbility, dc:ch.checkDC} : null); }
function passiveScore(char, check, model){ return 10 + checkBonus(char, check, model); }   // 10 + ability mod (+ proficiency)
function passiveBeats(char, check, model){ return passiveScore(char, check, model) >= check.dc; }
// BG3-style pre-roll breakdown — everything the player sees BEFORE committing to the roll.
function rollBreakdown(char, check, model){
  const idx=["Strength","Dexterity","Constitution","Intelligence","Wisdom","Charisma"].indexOf(check.ability);
  const amod=abilityMod(char.scores[idx]);
  const prof=!!(check.skill && isProficient(char, check.skill, model));
  const pbonus=prof?(model.proficiencyBonus||0):0;
  const bonus=amod+pbonus, need=check.dc-bonus;
  return { bonus:bonus, abilityMod:amod, prof:prof, profBonus:pbonus, need:need,
    needShown:Math.max(2,Math.min(20,need)), chance:chanceToPass(check.dc,bonus),
    onlyNat20:need>20, onlyNat1:need<=1 };
}
function choiceAvailable(char, state, ch, model){
  if(!matchesWhen(char, state, ch.when)) return false;                                   // identity/stat gate: ruled out if unmet
  const chk=checkOf(ch);
  if(chk && isPassiveSkill(chk.skill) && !passiveBeats(char, chk, model)) return false;  // passive skill: only shown if you'd already pass
  return true; }
// LORE (5e passive knowledge): which glossary terms appear in a line, and whether you'd recall the deep lore.
function glossaryHits(text, glossary){ const t=(text||"").toLowerCase(), hits=[];
  for(var i=0;i<glossary.length;i++){ var names=[glossary[i].term].concat(glossary[i].aka||[]);
    for(var j=0;j<names.length;j++){ if(t.indexOf(names[j].toLowerCase())>=0){ hits.push(i); break; } } }
  return hits; }
function loreKnown(char, entry, model){ if(!entry||!entry.skill) return false;
  return passiveBeats(char, {skill:entry.skill, ability:model.skillAbility[entry.skill], dc:entry.dc||10}, model); }
function secretKnown(char, entry, model){ if(!entry||!entry.secret) return false;
  return passiveBeats(char, {skill:entry.secretSkill, ability:model.skillAbility[entry.secretSkill], dc:entry.secretDc||18}, model); }
// THE RETURNED: a soul pulled back from the Wall perceives what the living can't — its clarity scales with
// Wisdom (the soul's lucidity). Innate; never proficiency. 10 + WIS modifier vs the moment's sense-DC.
function returnedClarity(char){ return 10 + abilityMod(char.scores[4]); }
// ---- grid collision + A* pathfinding (so you can't walk through people, stalls, or the fountain) ----
function tileKey(x,y){ return x+","+y; }
function buildBlocked(scene){ const b={}; (scene.props||[]).forEach(p=>{ b[tileKey(p.x,p.y)]=1; });
  (scene.npcs||[]).forEach(n=>{ b[tileKey(n.x,n.y)]=1; }); return b; }
function inBounds(x,y,w,h){ return x>=0&&x<w&&y>=0&&y<h; }
function findPath(sx,sy,gx,gy,blocked,w,h){   // 8-dir A*, no corner-cutting; returns [[x,y]…] from first step to goal, [] if none
  if(sx===gx&&sy===gy) return [];
  if(!inBounds(gx,gy,w,h)||blocked[tileKey(gx,gy)]) return [];
  const dirs=[[1,0],[-1,0],[0,1],[0,-1],[1,1],[1,-1],[-1,1],[-1,-1]];
  const open=[{x:sx,y:sy,g:0,f:0,p:null}], best={}; best[tileKey(sx,sy)]=0;
  while(open.length){
    let bi=0; for(let i=1;i<open.length;i++) if(open[i].f<open[bi].f) bi=i;
    const cur=open.splice(bi,1)[0];
    if(cur.x===gx&&cur.y===gy){ const path=[]; let n=cur; while(n.p){ path.unshift([n.x,n.y]); n=n.p; } return path; }
    for(const d of dirs){ const nx=cur.x+d[0], ny=cur.y+d[1]; if(!inBounds(nx,ny,w,h)||blocked[tileKey(nx,ny)]) continue;
      if(d[0]&&d[1] && (blocked[tileKey(cur.x+d[0],cur.y)]||blocked[tileKey(cur.x,cur.y+d[1])])) continue;   // no corner cut
      const ng=cur.g+(d[0]&&d[1]?1.414:1), k=tileKey(nx,ny);
      if(best[k]!==undefined && best[k]<=ng+1e-6) continue; best[k]=ng;
      open.push({x:nx,y:ny,g:ng,f:ng+Math.max(Math.abs(nx-gx),Math.abs(ny-gy)),p:cur}); } }
  return [];
}
function nearestFreeAdjacent(sx,sy,gx,gy,blocked,w,h){   // the reachable free tile beside a target (e.g. an NPC) closest to you
  const dirs=[[1,0],[-1,0],[0,1],[0,-1],[1,1],[1,-1],[-1,1],[-1,-1]]; let best=null,bestLen=1e9;
  for(const d of dirs){ const nx=gx+d[0], ny=gy+d[1]; if(!inBounds(nx,ny,w,h)||blocked[tileKey(nx,ny)]) continue;
    if(nx===sx&&ny===sy) return [nx,ny];
    const path=findPath(sx,sy,nx,ny,blocked,w,h); if(path.length && path.length<bestLen){ bestLen=path.length; best=[nx,ny]; } }
  return best;
}
/*</MKT>*/

// ---- player character ----
let char = JSON.parse(JSON.stringify(BUILDS[0])), buildIdx = 0;
function modFor(a){ return abilityMod(char.scores[ABILS.indexOf(a)]); }
function renderBuilds(){
  document.getElementById("builds").innerHTML = BUILDS.map((b,i)=>`<button class="bld${i===buildIdx?' on':''}" onclick="setBuild(${i})">${esc(b.name)}</button>`).join("")
    + `<button class="bld${buildIdx===-1?' on':''}" style="cursor:default;opacity:${buildIdx===-1?1:.5}">Custom</button>`;
  document.getElementById("buildBlurb").textContent = char.blurb || "A character of your own making.";
  renderWho(); renderAbil(); renderProf();
}
function setBuild(i){ buildIdx=i; char=JSON.parse(JSON.stringify(BUILDS[i])); renderBuilds(); }
function field2key(f){ return {Race:"race",Gender:"gender",Class:"cls",Background:"background",Deity:"deity"}[f]; }
function setField(f,v){ char[f]=v; char.name="Custom"; char.blurb=""; buildIdx=-1; renderBuilds(); }
function setAlign(v){ const[l,m]=v.split(" "); char.law=l; char.morality=m; char.name="Custom"; char.blurb=""; buildIdx=-1; renderBuilds(); }
function bump(i,d){ char.scores[i]=Math.max(1,Math.min(20,char.scores[i]+d)); char.name="Custom"; char.blurb=""; buildIdx=-1; renderBuilds(); }
function renderWho(){
  const sel=(f,opts,val)=>`<div><label>${f.toUpperCase()}</label><select onchange="setField('${field2key(f)}',this.value)">`+opts.map(o=>`<option${o===val?' selected':''}>${esc(o)}</option>`).join("")+`</select></div>`;
  const align=`<div><label>ALIGNMENT</label><select onchange="setAlign(this.value)">`+MODEL.laws.flatMap(l=>MODEL.moralities.map(m=>{const v=l+" "+m;return `<option${(char.law+" "+char.morality)===v?' selected':''}>${esc(v)}</option>`;})).join("")+`</select></div>`;
  document.getElementById("whoGrid").innerHTML=sel("Race",MODEL.races,char.race)+sel("Gender",MODEL.genders,char.gender)+sel("Class",MODEL.classes,char.cls)+sel("Background",MODEL.backgrounds,char.background)+sel("Deity",MODEL.deities,char.deity)+align;
}
function renderAbil(){ document.getElementById("abil").innerHTML=ABILS.map((a,i)=>{const m=abilityMod(char.scores[i]),dlg=(a==="Intelligence"||a==="Wisdom"||a==="Charisma");
  return `<div class="ab${dlg?' dlg':''}"><div class="k">${ABBR[a]}</div><div class="v">${char.scores[i]}</div><div class="m">${m>=0?'+':''}${m}</div><div class="step"><button onclick="bump(${i},-1)">▼</button><button onclick="bump(${i},1)">▲</button></div></div>`;}).join(""); }
function renderProf(){ const cp=(MODEL.classProficiencies[char.cls]||[]),bp=(MODEL.backgroundProficiencies[char.background]||[]),all=[...new Set([...cp,...bp])].sort();
  document.getElementById("profLine").innerHTML=`Proficient (+${MODEL.proficiencyBonus}) in: `+(all.length?all.map(s=>`<b>${esc(s)}</b>`).join(", "):"—")+`. INT · WIS · CHA drive the checks.`; }

// ---- persistent visit state ----
let st = newState();
function renderState(){
  // regard (NPC opinion of you) — disp.* lives in the Reckoning panel instead
  const apps=[]; Object.keys(st.ints).forEach(k=>{ if(k.indexOf("disp.")===0) return; apps.push({name:INT_LABELS[k]||k, v:st.ints[k]}); });
  const ap=document.getElementById("approvals");
  ap.innerHTML = !apps.length ? `<div class="empty">You haven't spoken to anyone yet.</div>` :
    apps.sort((a,b)=>Math.abs(b.v)-Math.abs(a.v)).map(a=>{const w=Math.min(50,Math.abs(a.v)*6),neg=a.v<0;
      return `<div class="appr"><div class="nm">${esc(a.name)}</div><div class="barwrap"><div class="bar${neg?' neg':''}" style="width:${w}%;${neg?'right:50%;left:auto;':''}"></div></div><div class="delta ${a.v>=0?'up':'down'}">${a.v>=0?'+':''}${a.v}</div></div>`;}).join("");
  // reckoning (Pillars-style dispositions)
  const disp=Object.keys(st.ints).filter(k=>k.indexOf("disp.")===0&&st.ints[k]>0).map(k=>({k,name:(DISP[k]||{}).name||k,hue:(DISP[k]||{}).hue||0,v:st.ints[k]}));
  const rk=document.getElementById("reckon");
  rk.innerHTML = !disp.length ? `<div class="empty">Your choices haven't tilted you yet.</div>` :
    disp.sort((a,b)=>b.v-a.v).map(d=>`<div class="reckon"><div class="nm" style="color:hsl(${d.hue} 50% 70%)">${esc(d.name)}</div><div class="pips" style="color:hsl(${d.hue} 50% 64%)">${"◆".repeat(Math.min(6,d.v))}</div><div class="rk">${roman(d.v)}</div></div>`).join("");
  const keys=Object.keys(st.bools), fl=document.getElementById("flags");
  fl.innerHTML = !keys.length ? `<div class="empty">Nothing yet — your choices leave a mark here.</div>` :
    keys.sort().map(k=>{const dot=k.indexOf("."),dom=dot<0?'':k.slice(0,dot)+'·';return `<span class="flagpill"><span class="d">${esc(dom)}</span>${esc(dot<0?k:k.slice(dot+1))}</span>`;}).join("");
}

// ====================== the market scene (canvas) ======================
const cv=document.getElementById("board"), ctx=cv.getContext("2d");
function normScene(s){ if(s._normed) return s; (s.props||[]).forEach(p=>{ p.tx=p.x; p.ty=p.y; }); (s.npcs||[]).forEach(n=>{ n.tx=n.x; n.ty=n.y; }); (s.exits||[]).forEach(x=>{ x.tx=x.x; x.ty=x.y; }); s._normed=true; return s; }
Object.values(SCENES).forEach(normScene); normScene(SCENE);   // every zone authored in x/y; the engine draws in tx/ty
const TW=64, TH=32, OX=cv.width/2, OY=64;
function iso(tx,ty){ return { x: OX+(tx-ty)*TW/2, y: OY+(tx+ty)*TH/2 }; }
function unIso(sx,sy){ const a=(sx-OX)/(TW/2), b=(sy-OY)/(TH/2); return { tx:(a+b)/2, ty:(b-a)/2 }; }
const player={ tx:SCENE.playerStart.x, ty:SCENE.playerStart.y, path:null, pathIdx:0, facing:1 };
// conditionally-present souls: an NPC with a `when` only appears when the world-state matches (e.g. those you saved)
function npcVisible(n){ return !n.when || matchesWhen(char, st, n.when); }
function activeNpcs(){ return (SCENE.npcs||[]).filter(npcVisible); }
function blockedNow(){ return buildBlocked({w:SCENE.w,h:SCENE.h,props:SCENE.props,npcs:activeNpcs()}); }
let BLOCKED=blockedNow();
let hoverTile=null, hoverNpc=null, nearNpc=null, hoverExit=null, autoTalk=null, traveling=false, fade=0, lastT=0;
function exitAt(tx,ty){ return (SCENE.exits||[]).find(x=>x.tx===tx&&x.ty===ty)||null; }
// ---- zone travel: walk onto a glowing causeway tile and the world changes around you ----
function loadScene(id, dest){ const s=SCENES[id]; if(!s) return; SCENE=normScene(s); BLOCKED=blockedNow();
  const d=dest||SCENE.playerStart; player.tx=d.x; player.ty=d.y; player.path=null; player.pathIdx=0;
  hoverTile=hoverNpc=hoverExit=nearNpc=autoTalk=null;
  const zb=document.getElementById("zonename"); if(zb) zb.textContent=SCENE.name||"";
  sfx('talk'); }
function travelTo(ex){ if(traveling) return; traveling=true; fade=0;
  const tick=()=>{ fade=Math.min(1,fade+0.08); if(fade<1){ requestAnimationFrame(tick); }
    else { loadScene(ex.to, ex.dest); fade=1; const out=()=>{ fade=Math.max(0,fade-0.07); if(fade>0) requestAnimationFrame(out); else traveling=false; }; requestAnimationFrame(out); } };
  requestAnimationFrame(tick); }

function npcAt(id){ return SCENE.npcs.find(n=>n.id===id); }
function dist(ax,ay,bx,by){ return Math.hypot(ax-bx,ay-by); }
function curTile(){ return [Math.round(player.tx), Math.round(player.ty)]; }
function walkTo(gx,gy){ const c=curTile(); const p=findPath(c[0],c[1],gx,gy,BLOCKED,SCENE.w,SCENE.h);
  if(p.length){ player.path=p; player.pathIdx=0; sfx('step'); return true; } return false; }

cv.addEventListener("mousemove",e=>{ const r=cv.getBoundingClientRect(), sx=e.clientX-r.left, sy=e.clientY-r.top;
  const t=unIso(sx,sy), tx=Math.round(t.tx), ty=Math.round(t.ty);
  hoverTile=inBounds(tx,ty,SCENE.w,SCENE.h)?{tx,ty}:null;
  hoverNpc=null; for(const n of activeNpcs()){ const p=iso(n.tx,n.ty); if(Math.hypot(sx-p.x,sy-(p.y-26))<26){ hoverNpc=n; break; } }
  hoverExit = hoverTile ? exitAt(hoverTile.tx,hoverTile.ty) : null;
  cv.style.cursor = (hoverNpc||hoverExit) ? "pointer" : (hoverTile && BLOCKED[tileKey(hoverTile.tx,hoverTile.ty)] ? "not-allowed" : "default");
});
cv.addEventListener("click",e=>{ if(document.getElementById("overlay").classList.contains("show")||traveling) return;
  const r=cv.getBoundingClientRect(), sx=e.clientX-r.left, sy=e.clientY-r.top;
  if(hoverExit){ // a causeway tile — walk to it, then the world changes
    if(curTile()[0]===hoverExit.tx && curTile()[1]===hoverExit.ty){ travelTo(hoverExit); return; }
    autoTalk=null; walkTo(hoverExit.tx,hoverExit.ty); return; }
  if(hoverNpc){ // approach the NPC — walk to a free tile beside them, then talk
    if(dist(player.tx,player.ty,hoverNpc.tx,hoverNpc.ty)<1.7){ talk(hoverNpc); return; }
    const c=curTile(), adj=nearestFreeAdjacent(c[0],c[1],hoverNpc.tx,hoverNpc.ty,BLOCKED,SCENE.w,SCENE.h);
    if(adj && walkTo(adj[0],adj[1])){ autoTalk=hoverNpc; }
    return; }
  const t=unIso(sx,sy); let tx=Math.max(0,Math.min(SCENE.w-1,Math.round(t.tx))), ty=Math.max(0,Math.min(SCENE.h-1,Math.round(t.ty)));
  if(BLOCKED[tileKey(tx,ty)]){ const c=curTile(), adj=nearestFreeAdjacent(c[0],c[1],tx,ty,BLOCKED,SCENE.w,SCENE.h); if(!adj) return; tx=adj[0]; ty=adj[1]; }
  autoTalk=null; walkTo(tx,ty);
});
window.addEventListener("keydown",e=>{
  if(document.getElementById("overlay").classList.contains("show")) return;
  if((e.key==="e"||e.key==="E") && nearNpc && dist(player.tx,player.ty,nearNpc.tx,nearNpc.ty)<1.7){ talk(nearNpc); }
});

function update(dt){
  if(player.path && player.pathIdx<player.path.length){ const wp=player.path[player.pathIdx];
    const dx=wp[0]-player.tx, dy=wp[1]-player.ty, d=Math.hypot(dx,dy);
    if(d<0.08){ player.tx=wp[0]; player.ty=wp[1]; player.pathIdx++; if(player.pathIdx>=player.path.length) player.path=null; }
    else { const sp=4.6*dt; player.tx+=dx/d*Math.min(sp,d); player.ty+=dy/d*Math.min(sp,d); if(Math.abs(dx)>0.01) player.facing=dx>=0?1:-1; } }
  // nearest NPC for the talk prompt + auto-talk once you've arrived beside your target
  nearNpc=null; let best=1.7;
  for(const n of activeNpcs()){ const d=dist(player.tx,player.ty,n.tx,n.ty); if(d<best){ best=d; nearNpc=n; } }
  if(autoTalk && !player.path && dist(player.tx,player.ty,autoTalk.tx,autoTalk.ty)<1.7){ const n=autoTalk; autoTalk=null; talk(n); }
  if(!player.path && !traveling){ const ex=exitAt(curTile()[0],curTile()[1]); if(ex) travelTo(ex); }   // step onto a causeway tile → cross to the next zone
}

// ---- drawing ----
function tokenColor(h,l){ return `hsl(${h} 46% ${l}%)`; }
function drawDiamond(cx,cy,fill,stroke){ ctx.beginPath(); ctx.moveTo(cx,cy-TH/2); ctx.lineTo(cx+TW/2,cy); ctx.lineTo(cx,cy+TH/2); ctx.lineTo(cx-TW/2,cy); ctx.closePath(); ctx.fillStyle=fill; ctx.fill(); if(stroke){ ctx.strokeStyle=stroke; ctx.lineWidth=1; ctx.stroke(); } }
function drawShadow(cx,by){ ctx.beginPath(); ctx.ellipse(cx,by,15,7,0,0,7); ctx.fillStyle="rgba(0,0,0,.34)"; ctx.fill(); }
function drawBox(cx,by,w,h,top,side,dark){ // simple iso box: by = base center y
  const hw=w/2; ctx.beginPath(); // left face
  ctx.moveTo(cx-hw,by); ctx.lineTo(cx-hw,by-h); ctx.lineTo(cx,by-h+TH/4); ctx.lineTo(cx,by+TH/4); ctx.closePath(); ctx.fillStyle=dark; ctx.fill();
  ctx.beginPath(); ctx.moveTo(cx+hw,by); ctx.lineTo(cx+hw,by-h); ctx.lineTo(cx,by-h+TH/4); ctx.lineTo(cx,by+TH/4); ctx.closePath(); ctx.fillStyle=side; ctx.fill();
  ctx.beginPath(); ctx.moveTo(cx,by-h-TH/4); ctx.lineTo(cx+hw,by-h); ctx.lineTo(cx,by-h+TH/4); ctx.lineTo(cx-hw,by-h); ctx.closePath(); ctx.fillStyle=top; ctx.fill();
}
function drawProp(p){ const s=iso(p.tx,p.ty);
  if(p.type==="fountain"){ drawDiamond(s.x,s.y,"#26222f","#1a1722"); ctx.beginPath(); ctx.ellipse(s.x,s.y,26,13,0,0,7); ctx.fillStyle="#1d2733"; ctx.fill(); ctx.beginPath(); ctx.ellipse(s.x,s.y-3,18,9,0,0,7); ctx.fillStyle="#2e4a63"; ctx.fill(); ctx.beginPath(); ctx.ellipse(s.x,s.y-4,9,4.5,0,0,7); ctx.fillStyle="#4a7a9a"; ctx.fill(); ctx.fillStyle="#6fa8c8"; ctx.fillRect(s.x-1.5,s.y-22,3,18); }
  else if(p.type==="stall"){ drawShadow(s.x,s.y+8); drawBox(s.x,s.y+6,40,20,"#3a3026","#2c241c","#211a14"); // awning
    ctx.beginPath(); ctx.moveTo(s.x-26,s.y-22); ctx.lineTo(s.x+26,s.y-22); ctx.lineTo(s.x+20,s.y-34); ctx.lineTo(s.x-20,s.y-34); ctx.closePath(); ctx.fillStyle=`hsl(${p.hue} 44% 46%)`; ctx.fill();
    ctx.fillStyle=`hsl(${p.hue} 44% 38%)`; for(let i=-2;i<=2;i++){ ctx.fillRect(s.x+i*10-2,s.y-34,4,12); } ctx.fillStyle="#1a1410"; ctx.fillRect(s.x-22,s.y-22,44,3); }
  else if(p.type==="crate"){ drawShadow(s.x,s.y+6); drawBox(s.x,s.y+5,26,16,"#6a4f2e","#553f24","#3e2d18"); }
  else if(p.type==="barrel"){ drawShadow(s.x,s.y+6); ctx.fillStyle="#4a3722"; ctx.beginPath(); ctx.ellipse(s.x,s.y-12,11,5,0,0,7); ctx.fill(); ctx.fillStyle="#5a4329"; ctx.fillRect(s.x-11,s.y-12,22,16); ctx.fillStyle="#3e2d18"; ctx.beginPath(); ctx.ellipse(s.x,s.y+4,11,5,0,0,7); ctx.fill(); ctx.fillStyle="#6a4f2e"; ctx.beginPath(); ctx.ellipse(s.x,s.y-12,11,5,0,0,7); ctx.fill(); }
  else if(p.type==="banner"){ ctx.strokeStyle="#3a2f22"; ctx.lineWidth=3; ctx.beginPath(); ctx.moveTo(s.x,s.y); ctx.lineTo(s.x,s.y-46); ctx.stroke(); ctx.fillStyle=`hsl(${p.hue} 44% 44%)`; ctx.beginPath(); ctx.moveTo(s.x,s.y-46); ctx.lineTo(s.x+22,s.y-42); ctx.lineTo(s.x+18,s.y-24); ctx.lineTo(s.x,s.y-28); ctx.closePath(); ctx.fill(); }
  else if(p.type==="boat"){ drawShadow(s.x,s.y+6); ctx.fillStyle="#3a2c1c"; ctx.beginPath(); ctx.moveTo(s.x-30,s.y-2); ctx.quadraticCurveTo(s.x,s.y+12,s.x+30,s.y-2); ctx.lineTo(s.x+24,s.y-9); ctx.quadraticCurveTo(s.x,s.y+2,s.x-24,s.y-9); ctx.closePath(); ctx.fill(); ctx.fillStyle="#4d3a24"; ctx.beginPath(); ctx.moveTo(s.x-24,s.y-9); ctx.quadraticCurveTo(s.x,s.y+2,s.x+24,s.y-9); ctx.quadraticCurveTo(s.x,s.y-4,s.x-24,s.y-9); ctx.closePath(); ctx.fill(); ctx.strokeStyle="#2a2030"; ctx.lineWidth=2; ctx.beginPath(); ctx.moveTo(s.x+14,s.y-7); ctx.lineTo(s.x+30,s.y-34); ctx.stroke(); }
  else if(p.type==="reeds"){ ctx.strokeStyle=`hsl(${p.hue||90} 26% 30%)`; ctx.lineWidth=2; for(let i=-3;i<=3;i++){ const bx=s.x+i*4, sw=(i%2?3:-3); ctx.beginPath(); ctx.moveTo(bx,s.y+4); ctx.quadraticCurveTo(bx+sw,s.y-14,bx+sw*1.6,s.y-26-Math.abs(i)*2); ctx.stroke(); } ctx.fillStyle="#2a3322"; for(let i=-2;i<=2;i+=2){ ctx.beginPath(); ctx.ellipse(s.x+i*4+(i%2?3:-3),s.y-28-Math.abs(i)*2,1.6,4,0,0,7); ctx.fill(); } }
  else if(p.type==="piling"){ drawShadow(s.x,s.y+4); drawBox(s.x,s.y+3,12,30,"#4a3a26","#3a2c1c","#241a12"); ctx.fillStyle="#1d2733"; ctx.beginPath(); ctx.ellipse(s.x,s.y+5,9,4,0,0,7); ctx.fill(); }
  else if(p.type==="shrine"){ drawShadow(s.x,s.y+5); drawBox(s.x,s.y+4,20,14,"#2c2838","#221f2e","#171420"); ctx.fillStyle="#3a3450"; ctx.fillRect(s.x-3,s.y-30,6,18); ctx.fillStyle="#5a5078"; ctx.fillRect(s.x-9,s.y-30,18,4); ctx.fillStyle="#9a86c8"; ctx.beginPath(); ctx.arc(s.x,s.y-34,3.5,0,7); ctx.fill(); }
  else if(p.type==="bridgepillar"){ drawShadow(s.x,s.y+6); drawBox(s.x,s.y+4,30,72,"#2a2734","#211e2a","#16141d"); ctx.fillStyle="#322e3e"; ctx.fillRect(s.x-15,s.y-74,30,6); ctx.strokeStyle="#15131b"; ctx.lineWidth=1; for(let i=1;i<4;i++){ ctx.beginPath(); ctx.moveTo(s.x-15,s.y-i*18); ctx.lineTo(s.x+15,s.y-i*18); ctx.stroke(); } }
  else if(p.type==="brazier"){ drawShadow(s.x,s.y+5); ctx.strokeStyle="#3a3340"; ctx.lineWidth=3; ctx.beginPath(); ctx.moveTo(s.x-8,s.y+2); ctx.lineTo(s.x-5,s.y-14); ctx.moveTo(s.x+8,s.y+2); ctx.lineTo(s.x+5,s.y-14); ctx.stroke(); ctx.fillStyle="#2a2530"; ctx.beginPath(); ctx.ellipse(s.x,s.y-15,11,5,0,0,7); ctx.fill(); const fl=0.6+0.4*Math.sin(lastT/180); ctx.fillStyle=`rgba(${200+40*fl|0},${90+50*fl|0},40,${0.5+0.3*fl})`; ctx.beginPath(); ctx.moveTo(s.x-6,s.y-15); ctx.quadraticCurveTo(s.x,s.y-26-6*fl,s.x+6,s.y-15); ctx.closePath(); ctx.fill(); ctx.fillStyle=`rgba(255,200,90,${0.4+0.3*fl})`; ctx.beginPath(); ctx.arc(s.x,s.y-17,2.5,0,7); ctx.fill(); }
  else if(p.type==="loom"){ drawShadow(s.x,s.y+5); ctx.strokeStyle="#4a3a26"; ctx.lineWidth=4; ctx.beginPath(); ctx.moveTo(s.x-16,s.y+4); ctx.lineTo(s.x-16,s.y-34); ctx.moveTo(s.x+16,s.y+4); ctx.lineTo(s.x+16,s.y-34); ctx.stroke(); ctx.lineWidth=3; ctx.beginPath(); ctx.moveTo(s.x-18,s.y-34); ctx.lineTo(s.x+18,s.y-34); ctx.stroke(); ctx.strokeStyle="rgba(180,150,200,.5)"; ctx.lineWidth=1; for(let i=-12;i<=12;i+=4){ ctx.beginPath(); ctx.moveTo(s.x+i,s.y-32); ctx.lineTo(s.x+i,s.y-6); ctx.stroke(); } ctx.fillStyle=`hsl(${p.hue||280} 30% 40%)`; ctx.fillRect(s.x-13,s.y-16,26,9); }
  else if(p.type==="pallet"){ drawShadow(s.x,s.y+4); drawDiamond(s.x,s.y,"#2a2622","#1a1714"); ctx.fillStyle="#3a3228"; ctx.beginPath(); ctx.moveTo(s.x-22,s.y); ctx.lineTo(s.x,s.y-9); ctx.lineTo(s.x+22,s.y); ctx.lineTo(s.x,s.y+9); ctx.closePath(); ctx.fill(); ctx.fillStyle="#473d2e"; ctx.beginPath(); ctx.ellipse(s.x-7,s.y-2,7,4,0,0,7); ctx.fill(); }
  else if(p.type==="hangings"){ ctx.strokeStyle="#2a2530"; ctx.lineWidth=2; ctx.beginPath(); ctx.moveTo(s.x-16,s.y-50); ctx.lineTo(s.x+16,s.y-48); ctx.stroke(); for(let k=-1;k<=1;k++){ const bx=s.x+k*13; ctx.fillStyle=`hsl(${(p.hue||280)+k*10} 30% ${30+k*4}%)`; ctx.beginPath(); ctx.moveTo(bx-6,s.y-49); ctx.lineTo(bx+6,s.y-49); ctx.lineTo(bx+5,s.y-22-((k+2)%2)*6); ctx.lineTo(bx-5,s.y-24-((k+1)%2)*6); ctx.closePath(); ctx.fill(); } }
  else if(p.type==="wall"){ drawShadow(s.x,s.y+6); drawBox(s.x,s.y+4,46,64,"#33313c","#26242e","#1a1822");
    // faces pressed half-out of the stone — the Mortared
    ctx.fillStyle="rgba(150,160,180,.13)"; for(let r=0;r<3;r++) for(let c=-1;c<=1;c++){ const fx=s.x+c*13+(r%2?6:0), fy=s.y-12-r*18; ctx.beginPath(); ctx.ellipse(fx,fy,5,6.5,0,0,7); ctx.fill(); ctx.fillStyle="rgba(10,10,14,.5)"; ctx.beginPath(); ctx.arc(fx-1.6,fy-1,0.9,0,7); ctx.arc(fx+1.6,fy-1,0.9,0,7); ctx.fill(); ctx.fillStyle="rgba(150,160,180,.13)"; } }
  else if(p.type==="torch"){ drawShadow(s.x,s.y+4); ctx.strokeStyle="#3a3340"; ctx.lineWidth=4; ctx.beginPath(); ctx.moveTo(s.x,s.y+2); ctx.lineTo(s.x,s.y-40); ctx.stroke(); ctx.fillStyle="#2a2530"; ctx.beginPath(); ctx.ellipse(s.x,s.y-40,9,4,0,0,7); ctx.fill(); const fl=0.6+0.4*Math.sin(lastT/140), gl=ctx.createRadialGradient(s.x,s.y-50,2,s.x,s.y-50,30); gl.addColorStop(0,`rgba(255,200,110,${0.5+0.2*fl})`); gl.addColorStop(1,"rgba(255,160,60,0)"); ctx.fillStyle=gl; ctx.beginPath(); ctx.arc(s.x,s.y-50,30,0,7); ctx.fill(); ctx.fillStyle=`rgba(${235+20*fl|0},${130+50*fl|0},50,.92)`; ctx.beginPath(); ctx.moveTo(s.x-7,s.y-42); ctx.quadraticCurveTo(s.x,s.y-62-10*fl,s.x+7,s.y-42); ctx.closePath(); ctx.fill(); ctx.fillStyle=`rgba(255,225,150,${0.7+0.2*fl})`; ctx.beginPath(); ctx.moveTo(s.x-3,s.y-44); ctx.quadraticCurveTo(s.x,s.y-54-6*fl,s.x+3,s.y-44); ctx.closePath(); ctx.fill(); }
  else if(p.type==="greywort"){ const gl=0.5+0.5*Math.sin(lastT/300); ctx.fillStyle=`rgba(150,200,210,${0.10+0.10*gl})`; ctx.beginPath(); ctx.ellipse(s.x,s.y,16,8,0,0,7); ctx.fill(); ctx.strokeStyle=`rgba(170,205,200,${0.6+0.3*gl})`; ctx.lineWidth=1.5; for(let i=-2;i<=2;i++){ ctx.beginPath(); ctx.moveTo(s.x+i*4,s.y+3); ctx.quadraticCurveTo(s.x+i*5,s.y-9,s.x+i*6,s.y-16); ctx.stroke(); } ctx.fillStyle=`rgba(200,235,225,${0.7+0.3*gl})`; for(let i=-2;i<=2;i++){ ctx.beginPath(); ctx.arc(s.x+i*6,s.y-16,1.8,0,7); ctx.fill(); } }
  else if(p.type==="deadtree"){ drawShadow(s.x,s.y+4); ctx.strokeStyle="#2c2622"; ctx.lineWidth=5; ctx.beginPath(); ctx.moveTo(s.x,s.y+2); ctx.lineTo(s.x-1,s.y-30); ctx.stroke(); ctx.lineWidth=2.5; ctx.beginPath(); ctx.moveTo(s.x-1,s.y-18); ctx.lineTo(s.x-12,s.y-30); ctx.moveTo(s.x-1,s.y-22); ctx.lineTo(s.x+11,s.y-32); ctx.moveTo(s.x-1,s.y-28); ctx.lineTo(s.x-7,s.y-40); ctx.moveTo(s.x-1,s.y-28); ctx.lineTo(s.x+6,s.y-41); ctx.stroke(); }
  else if(p.type==="lamppost"){ drawShadow(s.x,s.y+3); ctx.strokeStyle="#26242e"; ctx.lineWidth=3.5; ctx.beginPath(); ctx.moveTo(s.x,s.y+2); ctx.lineTo(s.x,s.y-44); ctx.stroke(); const fl=0.6+0.4*Math.sin(lastT/220), gl=ctx.createRadialGradient(s.x,s.y-48,1,s.x,s.y-48,26); gl.addColorStop(0,`rgba(255,210,120,${0.45+0.2*fl})`); gl.addColorStop(1,"rgba(255,180,70,0)"); ctx.fillStyle=gl; ctx.beginPath(); ctx.arc(s.x,s.y-48,26,0,7); ctx.fill(); ctx.fillStyle="#3a3340"; ctx.fillRect(s.x-5,s.y-52,10,10); ctx.fillStyle=`rgba(255,225,150,${0.7+0.25*fl})`; ctx.fillRect(s.x-3,s.y-50,6,6); }
  else if(p.type==="tavern"){ drawShadow(s.x,s.y+8); drawBox(s.x,s.y+6,52,40,"#3a2e22","#2c2218","#1f1810"); ctx.fillStyle="#241c14"; ctx.fillRect(s.x-8,s.y-18,16,24); const wl=0.6+0.4*Math.sin(lastT/300); ctx.fillStyle=`rgba(255,200,110,${0.4+0.3*wl})`; ctx.fillRect(s.x-22,s.y-30,9,9); ctx.fillRect(s.x+13,s.y-30,9,9); ctx.strokeStyle="#4a3a26"; ctx.lineWidth=2; ctx.beginPath(); ctx.moveTo(s.x-26,s.y-44); ctx.lineTo(s.x,s.y-58); ctx.lineTo(s.x+26,s.y-44); ctx.stroke(); ctx.fillStyle="#e7c873"; ctx.font="600 9px Iowan Old Style, Georgia, serif"; ctx.textAlign="center"; ctx.fillText("The Sotted Saint",s.x,s.y-46); }
  else if(p.type==="table"){ drawShadow(s.x,s.y+4); drawBox(s.x,s.y+3,26,9,"#5a4632","#46361f","#332715"); ctx.fillStyle="#46361f"; ctx.fillRect(s.x-3,s.y-2,6,7); }
  else if(p.type==="noticeboard"){ drawShadow(s.x,s.y+3); ctx.strokeStyle="#3a2c1c"; ctx.lineWidth=3; ctx.beginPath(); ctx.moveTo(s.x-12,s.y+2); ctx.lineTo(s.x-12,s.y-22); ctx.moveTo(s.x+12,s.y+2); ctx.lineTo(s.x+12,s.y-22); ctx.stroke(); ctx.fillStyle="#4a3a26"; ctx.fillRect(s.x-15,s.y-36,30,18); ctx.fillStyle="#d8d2c2"; for(let i=0;i<3;i++){ ctx.fillRect(s.x-12+(i%2)*8,s.y-33+i*5,9,4); } }
  else if(p.type==="bigscale"){ drawShadow(s.x,s.y+5); ctx.strokeStyle="#8a6f2e"; ctx.lineWidth=4; ctx.beginPath(); ctx.moveTo(s.x,s.y+4); ctx.lineTo(s.x,s.y-40); ctx.stroke(); const tilt=2.2*Math.sin(lastT/700); ctx.lineWidth=3; ctx.beginPath(); ctx.moveTo(s.x-22,s.y-40-tilt); ctx.lineTo(s.x+22,s.y-40+tilt); ctx.stroke(); for(const sgn of [-1,1]){ const px=s.x+sgn*22, py=s.y-40+sgn*tilt; ctx.strokeStyle="#7a6228"; ctx.lineWidth=1; ctx.beginPath(); ctx.moveTo(px-6,py); ctx.lineTo(px,py+9); ctx.lineTo(px+6,py); ctx.stroke(); ctx.fillStyle="#b8932e"; ctx.beginPath(); ctx.ellipse(px,py+10,7,3,0,0,7); ctx.fill(); } ctx.fillStyle="#6a5424"; ctx.beginPath(); ctx.arc(s.x,s.y-40,3,0,7); ctx.fill(); }
  else if(p.type==="desk"){ drawShadow(s.x,s.y+5); drawBox(s.x,s.y+4,34,16,"#4a3826","#38291a","#261c11"); ctx.fillStyle="#d8d2c2"; ctx.fillRect(s.x-8,s.y-14,12,7); ctx.fillStyle="#2a2530"; ctx.fillRect(s.x+4,s.y-13,3,5); }
  else if(p.type==="ledgershelf"){ drawShadow(s.x,s.y+5); drawBox(s.x,s.y+4,28,56,"#3a2c1c","#2c2014","#1d1410"); for(let r=0;r<4;r++){ for(let i=0;i<4;i++){ ctx.fillStyle=`hsl(${(p.hue||40)+i*8} 26% ${24+i*5}%)`; ctx.fillRect(s.x-12+i*6,s.y-52+r*13,5,11); } ctx.strokeStyle="#15100a"; ctx.beginPath(); ctx.moveTo(s.x-13,s.y-40+r*13); ctx.lineTo(s.x+13,s.y-40+r*13); ctx.stroke(); } }
  else if(p.type==="cell"){ drawShadow(s.x,s.y+5); drawDiamond(s.x,s.y,"#14121a","#0e0c14"); ctx.strokeStyle="#3a3744"; ctx.lineWidth=2.5; for(let i=-2;i<=2;i++){ ctx.beginPath(); ctx.moveTo(s.x+i*7,s.y+6); ctx.lineTo(s.x+i*7,s.y-40); ctx.stroke(); } ctx.lineWidth=3; ctx.beginPath(); ctx.moveTo(s.x-16,s.y-40); ctx.lineTo(s.x+16,s.y-40); ctx.moveTo(s.x-16,s.y-20); ctx.lineTo(s.x+16,s.y-20); ctx.stroke(); }
  else if(p.type==="candle"){ const fl=0.6+0.4*Math.sin(lastT/120+p.x); ctx.fillStyle="#d8c8a0"; ctx.fillRect(s.x-2,s.y-14,4,14); const gl=ctx.createRadialGradient(s.x,s.y-18,1,s.x,s.y-18,14); gl.addColorStop(0,`rgba(255,210,130,${0.5+0.25*fl})`); gl.addColorStop(1,"rgba(255,180,80,0)"); ctx.fillStyle=gl; ctx.beginPath(); ctx.arc(s.x,s.y-18,14,0,7); ctx.fill(); ctx.fillStyle=`rgba(255,225,150,${0.85})`; ctx.beginPath(); ctx.moveTo(s.x-2,s.y-15); ctx.quadraticCurveTo(s.x,s.y-22-3*fl,s.x+2,s.y-15); ctx.closePath(); ctx.fill(); }
  else if(p.type==="campfire"){ drawShadow(s.x,s.y+4); ctx.strokeStyle="#3a2c1c"; ctx.lineWidth=3; for(let i=-1;i<=1;i++){ ctx.beginPath(); ctx.moveTo(s.x-12+i*4,s.y+4); ctx.lineTo(s.x+12+i*4,s.y-6); ctx.stroke(); ctx.beginPath(); ctx.moveTo(s.x+12-i*4,s.y+4); ctx.lineTo(s.x-12-i*4,s.y-6); ctx.stroke(); } const fl=0.55+0.45*Math.sin(lastT/110), gl=ctx.createRadialGradient(s.x,s.y-12,2,s.x,s.y-12,42); gl.addColorStop(0,`rgba(255,190,90,${0.55+0.25*fl})`); gl.addColorStop(1,"rgba(255,150,50,0)"); ctx.fillStyle=gl; ctx.beginPath(); ctx.arc(s.x,s.y-12,42,0,7); ctx.fill(); for(let k=0;k<3;k++){ const w=9-k*2.5; ctx.fillStyle=`rgba(${245-k*10},${150+k*30+40*fl|0},${50+k*30},${0.85-k*0.15})`; ctx.beginPath(); ctx.moveTo(s.x-w,s.y-2); ctx.quadraticCurveTo(s.x+(k-1)*3,s.y-22-10*fl-k*4,s.x+w,s.y-2); ctx.closePath(); ctx.fill(); } }
  else if(p.type==="log"){ drawShadow(s.x,s.y+4); ctx.fillStyle="#4a3826"; ctx.beginPath(); ctx.ellipse(s.x,s.y-3,16,7,0,0,7); ctx.fill(); ctx.fillStyle="#5a4632"; ctx.fillRect(s.x-16,s.y-7,32,5); ctx.fillStyle="#6a5440"; ctx.beginPath(); ctx.ellipse(s.x-16,s.y-4,4,5,0,0,7); ctx.fill(); ctx.strokeStyle="#3a2c1c"; ctx.beginPath(); ctx.ellipse(s.x-16,s.y-4,2,3,0,0,7); ctx.stroke(); }
  else if(p.type==="bedroll"){ drawShadow(s.x,s.y+3); ctx.fillStyle=`hsl(${(p.x*53)%360} 18% 30%)`; ctx.beginPath(); ctx.moveTo(s.x-20,s.y); ctx.lineTo(s.x,s.y-8); ctx.lineTo(s.x+20,s.y); ctx.lineTo(s.x,s.y+8); ctx.closePath(); ctx.fill(); ctx.fillStyle=`hsl(${(p.x*53)%360} 18% 40%)`; ctx.beginPath(); ctx.ellipse(s.x-9,s.y-1,7,4,0,0,7); ctx.fill(); }
  else if(p.type==="tree"){ drawShadow(s.x,s.y+4); ctx.fillStyle="#2c2218"; ctx.fillRect(s.x-3,s.y-22,6,24); ctx.fillStyle="#1e3326"; ctx.beginPath(); ctx.arc(s.x,s.y-34,15,0,7); ctx.fill(); ctx.fillStyle="#24402e"; ctx.beginPath(); ctx.arc(s.x-7,s.y-30,10,0,7); ctx.arc(s.x+8,s.y-32,11,0,7); ctx.fill(); ctx.fillStyle="#2c4e38"; ctx.beginPath(); ctx.arc(s.x+2,s.y-40,9,0,7); ctx.fill(); }
  else if(p.type==="teatable"){ drawShadow(s.x,s.y+5); drawBox(s.x,s.y+4,30,12,"#4a3826","#38291a","#261c11"); const st=0.5+0.5*Math.sin(lastT/240); ctx.fillStyle="#9a8a6a"; ctx.beginPath(); ctx.ellipse(s.x-6,s.y-14,4,3,0,0,7); ctx.fill(); ctx.fillStyle="#8a7a5a"; ctx.beginPath(); ctx.ellipse(s.x+6,s.y-14,4,3,0,0,7); ctx.fill(); ctx.strokeStyle=`rgba(200,200,210,${0.18+0.16*st})`; ctx.lineWidth=1.4; ctx.beginPath(); ctx.moveTo(s.x-6,s.y-17); ctx.quadraticCurveTo(s.x-9,s.y-25,s.x-6,s.y-31); ctx.moveTo(s.x+6,s.y-17); ctx.quadraticCurveTo(s.x+3,s.y-25,s.x+6,s.y-31); ctx.stroke(); }
  else if(p.type==="chair"){ drawShadow(s.x,s.y+3); ctx.fillStyle="#3a2c1c"; ctx.fillRect(s.x-7,s.y-10,14,8); ctx.fillStyle="#2c2014"; ctx.fillRect(s.x-7,s.y-26,4,18); ctx.fillRect(s.x+3,s.y-26,4,18); }
  else if(p.type==="hearthfire"){ drawShadow(s.x,s.y+6); drawBox(s.x,s.y+5,44,38,"#2a2530","#201c28","#15121c"); ctx.fillStyle="#0e0c14"; ctx.beginPath(); ctx.moveTo(s.x-14,s.y-4); ctx.lineTo(s.x-14,s.y-26); ctx.lineTo(s.x+14,s.y-26); ctx.lineTo(s.x+14,s.y-4); ctx.closePath(); ctx.fill(); const fl=0.55+0.45*Math.sin(lastT/130), gl=ctx.createRadialGradient(s.x,s.y-12,2,s.x,s.y-12,30); gl.addColorStop(0,`rgba(255,180,90,${0.6+0.25*fl})`); gl.addColorStop(1,"rgba(255,140,50,0)"); ctx.fillStyle=gl; ctx.beginPath(); ctx.arc(s.x,s.y-12,30,0,7); ctx.fill(); for(let k=0;k<2;k++){ const w=8-k*3; ctx.fillStyle=`rgba(${245-k*15},${150+k*40+30*fl|0},${55+k*30},${0.85-k*0.2})`; ctx.beginPath(); ctx.moveTo(s.x-w,s.y-5); ctx.quadraticCurveTo(s.x,s.y-20-8*fl,s.x+w,s.y-5); ctx.closePath(); ctx.fill(); } ctx.fillStyle="#322c38"; ctx.fillRect(s.x-18,s.y-30,36,4); }
  else if(p.type==="bookshelf"){ drawShadow(s.x,s.y+5); drawBox(s.x,s.y+4,26,56,"#34281a","#281e12","#1a130c"); for(let r=0;r<4;r++){ for(let i=0;i<5;i++){ ctx.fillStyle=`hsl(${(i*47+r*30)%360} 22% ${22+i*4}%)`; ctx.fillRect(s.x-12+i*5,s.y-52+r*13,4,11); } ctx.strokeStyle="#15100a"; ctx.beginPath(); ctx.moveTo(s.x-13,s.y-40+r*13); ctx.lineTo(s.x+13,s.y-40+r*13); ctx.stroke(); } }
  else if(p.type==="window"){ ctx.fillStyle="#1a1726"; ctx.beginPath(); ctx.moveTo(s.x-14,s.y-18); ctx.lineTo(s.x-14,s.y-46); ctx.lineTo(s.x+14,s.y-50); ctx.lineTo(s.x+14,s.y-22); ctx.closePath(); ctx.fill(); ctx.fillStyle="rgba(120,140,190,.22)"; ctx.beginPath(); ctx.moveTo(s.x-12,s.y-20); ctx.lineTo(s.x-12,s.y-44); ctx.lineTo(s.x+12,s.y-48); ctx.lineTo(s.x+12,s.y-24); ctx.closePath(); ctx.fill(); ctx.strokeStyle="#2c2838"; ctx.lineWidth=2; ctx.beginPath(); ctx.moveTo(s.x,s.y-22); ctx.lineTo(s.x,s.y-49); ctx.moveTo(s.x-13,s.y-34); ctx.lineTo(s.x+13,s.y-37); ctx.stroke(); }
  else if(p.type==="greyshrine"){ drawShadow(s.x,s.y+6); drawBox(s.x,s.y+5,22,30,"#3a3848","#2c2a38","#1d1c28"); ctx.strokeStyle="#8a90a8"; ctx.lineWidth=3; ctx.beginPath(); ctx.moveTo(s.x,s.y-26); ctx.lineTo(s.x,s.y-58); ctx.stroke(); const tilt=2.6*Math.sin(lastT/900); ctx.lineWidth=2.5; ctx.beginPath(); ctx.moveTo(s.x-20,s.y-56-tilt); ctx.lineTo(s.x+20,s.y-56+tilt); ctx.stroke(); for(const sgn of [-1,1]){ const px=s.x+sgn*20, py=s.y-56+sgn*tilt; ctx.strokeStyle="#707088"; ctx.lineWidth=1; ctx.beginPath(); ctx.moveTo(px-5,py); ctx.lineTo(px,py+8); ctx.lineTo(px+5,py); ctx.stroke(); ctx.fillStyle="#9aa0b8"; ctx.beginPath(); ctx.ellipse(px,py+9,6,2.5,0,0,7); ctx.fill(); } const gl=ctx.createRadialGradient(s.x,s.y-44,2,s.x,s.y-44,24); gl.addColorStop(0,"rgba(150,170,210,.20)"); gl.addColorStop(1,"rgba(150,170,210,0)"); ctx.fillStyle=gl; ctx.beginPath(); ctx.arc(s.x,s.y-44,24,0,7); ctx.fill(); ctx.fillStyle="#aab0c8"; ctx.beginPath(); ctx.arc(s.x,s.y-56,3,0,7); ctx.fill(); }
  else if(p.type==="milestone"){ drawShadow(s.x,s.y+3); ctx.fillStyle="#4a4838"; ctx.beginPath(); ctx.moveTo(s.x-8,s.y); ctx.lineTo(s.x-8,s.y-20); ctx.quadraticCurveTo(s.x,s.y-26,s.x+8,s.y-20); ctx.lineTo(s.x+8,s.y); ctx.closePath(); ctx.fill(); ctx.fillStyle="#5a5848"; ctx.beginPath(); ctx.moveTo(s.x-8,s.y-20); ctx.quadraticCurveTo(s.x,s.y-26,s.x+8,s.y-20); ctx.quadraticCurveTo(s.x,s.y-23,s.x-8,s.y-20); ctx.fill(); ctx.strokeStyle="#2c2a22"; ctx.lineWidth=1; ctx.beginPath(); ctx.moveTo(s.x-4,s.y-13); ctx.lineTo(s.x+4,s.y-13); ctx.moveTo(s.x-4,s.y-9); ctx.lineTo(s.x+4,s.y-9); ctx.stroke(); }
  else if(p.type==="cairn"){ drawShadow(s.x,s.y+4); ctx.fillStyle="#3a3730"; ctx.beginPath(); ctx.ellipse(s.x,s.y-2,13,7,0,0,7); ctx.fill(); ctx.fillStyle="#46443a"; ctx.beginPath(); ctx.ellipse(s.x-3,s.y-8,8,5,0,0,7); ctx.fill(); ctx.fillStyle="#52503f"; ctx.beginPath(); ctx.ellipse(s.x+2,s.y-13,6,4,0,0,7); ctx.fill(); ctx.fillStyle="#5a5848"; ctx.beginPath(); ctx.arc(s.x,s.y-19,4,0,7); ctx.fill(); }
  else if(p.type==="wagon"){ drawShadow(s.x,s.y+8); drawBox(s.x,s.y+6,46,22,"#4a3826","#38291a","#261c11"); // cart bed
    for(const wx of [-20,20]){ ctx.fillStyle="#2a2014"; ctx.beginPath(); ctx.arc(s.x+wx,s.y+6,9,0,7); ctx.fill(); ctx.fillStyle="#3e2d18"; ctx.beginPath(); ctx.arc(s.x+wx,s.y+6,9,0,7); ctx.fill(); ctx.strokeStyle="#5a4329"; ctx.lineWidth=1.5; for(let k=0;k<4;k++){ const a=k*Math.PI/4+lastT/4000; ctx.beginPath(); ctx.moveTo(s.x+wx,s.y+6); ctx.lineTo(s.x+wx+Math.cos(a)*8,s.y+6+Math.sin(a)*8); ctx.stroke(); } ctx.fillStyle="#6a5440"; ctx.beginPath(); ctx.arc(s.x+wx,s.y+6,2.5,0,7); ctx.fill(); }
    // arched canvas cover, sewn from map-margins
    ctx.fillStyle="#cabd9a"; ctx.beginPath(); ctx.moveTo(s.x-24,s.y-14); ctx.quadraticCurveTo(s.x,s.y-58,s.x+24,s.y-14); ctx.lineTo(s.x+24,s.y-14); ctx.quadraticCurveTo(s.x,s.y-44,s.x-24,s.y-14); ctx.closePath(); ctx.fill(); ctx.fillStyle="#b7a983"; ctx.beginPath(); ctx.moveTo(s.x-24,s.y-14); ctx.quadraticCurveTo(s.x,s.y-58,s.x+24,s.y-14); ctx.lineTo(s.x+18,s.y-14); ctx.quadraticCurveTo(s.x,s.y-50,s.x-18,s.y-14); ctx.closePath(); ctx.fill();
    ctx.strokeStyle="#8a7d5a"; ctx.lineWidth=1; for(let i=-2;i<=2;i++){ const rx=i*9; ctx.beginPath(); ctx.moveTo(s.x+rx,s.y-14); ctx.quadraticCurveTo(s.x+rx*0.5,s.y-46-(2-Math.abs(i))*4,s.x+rx*0.2,s.y-50); ctx.stroke(); }
    // a chart spilling off the tailboard
    ctx.fillStyle="#e3d9bd"; ctx.beginPath(); ctx.moveTo(s.x+18,s.y-2); ctx.lineTo(s.x+40,s.y+6); ctx.lineTo(s.x+34,s.y+14); ctx.lineTo(s.x+14,s.y+6); ctx.closePath(); ctx.fill(); ctx.strokeStyle="#5a7a8a"; ctx.lineWidth=1; ctx.beginPath(); ctx.moveTo(s.x+20,s.y+2); ctx.quadraticCurveTo(s.x+30,s.y+5,s.x+36,s.y+10); ctx.stroke(); ctx.strokeStyle="#7a5a4a"; ctx.beginPath(); ctx.moveTo(s.x+24,s.y+8); ctx.lineTo(s.x+33,s.y+6); ctx.stroke(); }
  else if(p.type==="throne"){ drawShadow(s.x,s.y+8); drawBox(s.x,s.y+6,40,30,"#2a2738","#201d2c","#15131e"); ctx.fillStyle="#322e40"; ctx.beginPath(); ctx.moveTo(s.x-20,s.y-20); ctx.lineTo(s.x-20,s.y-64); ctx.lineTo(s.x-12,s.y-58); ctx.lineTo(s.x-12,s.y-20); ctx.closePath(); ctx.fill(); ctx.beginPath(); ctx.moveTo(s.x+20,s.y-20); ctx.lineTo(s.x+20,s.y-64); ctx.lineTo(s.x+12,s.y-58); ctx.lineTo(s.x+12,s.y-20); ctx.closePath(); ctx.fill(); ctx.fillStyle="#3a3550"; ctx.fillRect(s.x-12,s.y-58,24,40); for(let i=-1;i<=1;i++){ ctx.strokeStyle="#8a6f2e"; ctx.lineWidth=2.5; ctx.beginPath(); ctx.moveTo(s.x+i*8,s.y-58); ctx.lineTo(s.x+i*10,s.y-74-Math.abs(i)*4); ctx.stroke(); } const gl=ctx.createRadialGradient(s.x,s.y-44,2,s.x,s.y-44,34); gl.addColorStop(0,"rgba(150,170,210,.16)"); gl.addColorStop(1,"rgba(150,170,210,0)"); ctx.fillStyle=gl; ctx.beginPath(); ctx.arc(s.x,s.y-44,34,0,7); ctx.fill(); }
  else if(p.type==="deathdoor"){ drawShadow(s.x,s.y+8); ctx.fillStyle="#1a1824"; drawBox(s.x-22,s.y+6,16,74,"#2a2738","#201d2c","#15131e"); drawBox(s.x+22,s.y+6,16,74,"#2a2738","#201d2c","#15131e"); ctx.fillStyle="#26233200"; ctx.fillStyle="#201d2c"; ctx.beginPath(); ctx.moveTo(s.x-30,s.y-66); ctx.lineTo(s.x-30,s.y-72); ctx.quadraticCurveTo(s.x,s.y-92,s.x+30,s.y-72); ctx.lineTo(s.x+30,s.y-66); ctx.quadraticCurveTo(s.x,s.y-84,s.x-30,s.y-66); ctx.closePath(); ctx.fill(); const pulse=0.5+0.5*Math.sin(lastT/520); const gl=ctx.createLinearGradient(s.x,s.y+6,s.x,s.y-70); gl.addColorStop(0,`rgba(150,170,210,${0.05+0.05*pulse})`); gl.addColorStop(0.6,`rgba(120,150,200,${0.16+0.10*pulse})`); gl.addColorStop(1,"rgba(80,110,170,0.02)"); ctx.fillStyle=gl; ctx.beginPath(); ctx.moveTo(s.x-22,s.y+6); ctx.lineTo(s.x-22,s.y-70); ctx.quadraticCurveTo(s.x,s.y-86,s.x+22,s.y-70); ctx.lineTo(s.x+22,s.y+6); ctx.closePath(); ctx.fill(); for(let i=0;i<7;i++){ const t=(lastT/1100+i/7)%1, yy=s.y+4-t*72, a=Math.sin(t*Math.PI); ctx.fillStyle=`rgba(180,200,230,${0.22*a})`; ctx.beginPath(); ctx.arc(s.x+(i-3)*6,yy,1.3,0,7); ctx.fill(); } }
}
function drawToken(tx,ty,hue,label,opts){ opts=opts||{}; const s=iso(tx,ty); drawShadow(s.x,s.y+2);
  const glow=opts.glow; if(glow){ ctx.beginPath(); ctx.ellipse(s.x,s.y,20,10,0,0,7); ctx.fillStyle="rgba(231,200,115,.18)"; ctx.fill(); ctx.strokeStyle="rgba(231,200,115,.55)"; ctx.lineWidth=2; ctx.stroke(); }
  // cloak/body
  ctx.fillStyle=tokenColor(hue,opts.player?54:44); ctx.beginPath(); ctx.moveTo(s.x-9,s.y-2); ctx.quadraticCurveTo(s.x,s.y-30,s.x+9,s.y-2); ctx.closePath(); ctx.fill();
  ctx.fillStyle=tokenColor(hue,opts.player?44:36); ctx.fillRect(s.x-9,s.y-8,18,7);
  // head
  ctx.fillStyle="#d8b48c"; ctx.beginPath(); ctx.arc(s.x,s.y-30,6.5,0,7); ctx.fill();
  ctx.fillStyle=tokenColor(hue,30); ctx.beginPath(); ctx.arc(s.x,s.y-33,7,Math.PI,0); ctx.fill(); // hood/hair
  if(label){ ctx.font="600 12px Iowan Old Style, Georgia, serif"; const w=ctx.measureText(label).width;
    ctx.fillStyle="rgba(12,11,16,.78)"; ctx.fillRect(s.x-w/2-6,s.y-58,w+12,18); ctx.fillStyle=opts.player?"#e7c873":"#e8e2d2"; ctx.textAlign="center"; ctx.fillText(label,s.x,s.y-45); }
  if(opts.prompt){ ctx.font="600 12px Iowan Old Style, Georgia, serif"; ctx.fillStyle="#e7c873"; ctx.textAlign="center"; ctx.fillText("▶ talk (E)",s.x,s.y-63); }
}
function render(){
  ctx.clearRect(0,0,cv.width,cv.height);
  // floor (blocked tiles read darker; the hovered tile lights unless it's blocked)
  for(let ty=0;ty<SCENE.h;ty++) for(let tx=0;tx<SCENE.w;tx++){ const s=iso(tx,ty), bl=BLOCKED[tileKey(tx,ty)];
    let shade=((tx+ty)%2)?"#181620":"#1c1a26"; if(bl) shade=((tx+ty)%2)?"#141019":"#16121f";
    if(hoverTile&&hoverTile.tx===tx&&hoverTile.ty===ty) shade=bl?"#2a1620":"#2a2740";
    drawDiamond(s.x,s.y, shade, "#13111a"); }
  // zone exits — a glowing causeway tile you can walk onto to cross to the next scene
  (SCENE.exits||[]).forEach(x=>{ const s=iso(x.tx,x.ty), pulse=0.5+0.32*Math.sin(lastT/380);
    drawDiamond(s.x,s.y,`rgba(110,168,200,${0.12+0.10*pulse})`,`rgba(150,200,230,${0.5+0.4*pulse})`);
    ctx.fillStyle=`rgba(180,214,236,${0.6+0.3*pulse})`; ctx.font="600 11px Iowan Old Style, Georgia, serif"; ctx.textAlign="center";
    for(let k=0;k<3;k++){ ctx.globalAlpha=(0.18+0.12*pulse)*(1-k*0.28); ctx.fillText("∙",s.x,s.y-8-k*7); } ctx.globalAlpha=1; });
  // path preview — faint dots along the route, brighter at the destination (BG3-style)
  if(player.path){ for(let i=player.pathIdx;i<player.path.length;i++){ const wp=player.path[i], s=iso(wp[0],wp[1]), last=i===player.path.length-1;
    if(last){ drawDiamond(s.x,s.y,"rgba(231,200,115,.12)","rgba(231,200,115,.6)"); }
    ctx.beginPath(); ctx.arc(s.x,s.y,last?4:2.5,0,7); ctx.fillStyle=last?"#e7c873":"rgba(231,200,115,.5)"; ctx.fill(); } }
  // depth-sorted entities (props + npcs + player)
  const ents=[];
  SCENE.props.forEach(p=>ents.push({d:p.tx+p.ty + (p.type==="banner"?-0.5:0), draw:()=>drawProp(p)}));
  activeNpcs().forEach(n=>ents.push({d:n.tx+n.ty, draw:()=>drawToken(n.tx,n.ty,n.hue,n.name,{glow:nearNpc===n||hoverNpc===n, prompt:nearNpc===n})}));
  ents.push({d:player.tx+player.ty, draw:()=>drawToken(player.tx,player.ty,46,null,{player:true})});
  ents.sort((a,b)=>a.d-b.d).forEach(e=>e.draw());
  // exit label on hover — tells you where the causeway goes
  if(hoverExit&&hoverExit.label){ const s=iso(hoverExit.tx,hoverExit.ty); ctx.font="600 12px Iowan Old Style, Georgia, serif"; const w=ctx.measureText(hoverExit.label).width;
    ctx.fillStyle="rgba(12,16,22,.86)"; ctx.fillRect(s.x-w/2-7,s.y-44,w+14,18); ctx.fillStyle="#bcd6ec"; ctx.textAlign="center"; ctx.fillText(hoverExit.label,s.x,s.y-31); }
  // travel fade — the world dissolves and reforms as you cross
  if(fade>0){ ctx.fillStyle=`rgba(7,9,14,${fade})`; ctx.fillRect(0,0,cv.width,cv.height); }
}
function loop(t){ const dt=Math.min(0.05,(t-lastT)/1000||0); lastT=t; update(dt); render(); requestAnimationFrame(loop); }

// ====================== the dialogue overlay ======================
let curConv=null, curNpc=null, pendingOpts=null, loreSeen={}, sensed=false;
function talk(npc){
  curNpc=npc; curConv=CONVS.find(c=>c.id===npc.conv); if(!curConv) return;
  player.path=null; autoTalk=null; loreSeen={}; sensed=false;
  document.getElementById("dsig").textContent=npc.sigil; document.getElementById("dsig").style.background=`hsl(${npc.hue} 52% 62%)`;
  document.getElementById("dname").textContent=npc.name; document.getElementById("dtitle").textContent=npc.title;
  document.getElementById("dscript").innerHTML=""; document.getElementById("overlay").classList.add("show");
  sfx('talk');
  // The Returned-sense — on first sight, if the soul's clarity is keen enough, you perceive the unseen.
  if(curConv.returned && returnedClarity(char) >= curConv.returned.dc){ addSense(curConv.returned.text); }
  const start=curConv.nodes.find(n=>n.id===curConv.start)?curConv.start:curConv.nodes[0].id;
  goNode(start);
}
function closeDialogue(){ document.getElementById("overlay").classList.remove("show"); curConv=null; pendingOpts=null; BLOCKED=blockedNow(); renderState(); }
function nodeById(id){ return curConv.nodes.find(n=>n.id===id); }
function isEnd(id){ return !id||id==="END"||id==="end"||!nodeById(id); }
function goNode(id){
  pendingOpts=null; const n=nodeById(id), script=document.getElementById("dscript");
  if(!n){ endScene(); return; }
  applyEffects(st, n.onEnter); applyEffects(st, n.effects); renderState();   // node entry + outcome effects
  const line=pickVariantText(n,char,st)||"〔(no line for this character)〕";
  addLine("", curNpc.name, line);
  // LORE — 5e passive knowledge: surface what your character would *recall* about topics in this line.
  glossaryHits(line, GLOSS).forEach(i=>{ const e=GLOSS[i];
    if(e.skill && !loreSeen[i] && loreKnown(char, e, MODEL)){ loreSeen[i]=true; addLore(e); } });
  const opts=document.createElement("div"); opts.className="opts"; script.appendChild(opts);
  const all=n.choices||[];
  if(all.length){ pendingOpts={node:n,el:opts}; paintChoices(); }
  else if(n.auto && !isEnd(n.auto)){ const b=document.createElement("button"); b.className="continue"; b.textContent="Continue ▸"; b.onclick=()=>{ sfx('page'); opts.remove(); goNode(n.auto); }; opts.appendChild(b); }
  else endScene();
  script.scrollTop=script.scrollHeight;
}
function normCheck(ch){ if(ch.check) return ch.check; if(ch.checkDC) return {skill:null,ability:ch.checkAbility,dc:ch.checkDC}; return null; }
function tagFor(ch){ if(ch.tag) return {cls:ch.tag, label:(ch.tagLabel||ch.tag.toUpperCase())};
  const w=ch.when; if(!w) return null;
  if(w.race!==undefined) return {cls:"race",label:[].concat(w.race).join("/")};
  if(w.gender!==undefined) return {cls:"gender",label:[].concat(w.gender).join("/")};
  if(w.class!==undefined) return {cls:"class",label:[].concat(w.class).join("/")};
  if(w.background!==undefined) return {cls:"background",label:[].concat(w.background).join("/")};
  if(w.deity!==undefined) return {cls:"faith",label:[].concat(w.deity).map(d=>d==="None"?"Faithless":d).join("/")};
  if(w.law!==undefined) return {cls:"alignment",label:[].concat(w.law).join("/")};
  if(w.morality!==undefined) return {cls:"alignment",label:[].concat(w.morality).join("/")};
  if(w.ability){ const k=Object.keys(w.ability)[0]; return {cls:"stat",label:`${ABBR[k]} ${w.ability[k]}`}; }
  if(w.int){ const k=Object.keys(w.int)[0]; if(k.indexOf("disp.")===0){ const d=DISP[k]||{name:k}; return {cls:"disp",label:`${d.name} ${roman(w.int[k])}`}; }
    return {cls:"alignment",label:`${(INT_LABELS[k]||k)} ≥ ${w.int[k]}`}; }
  return null; }
function paintChoices(){ if(!pendingOpts) return; const {node:n,el:opts}=pendingOpts; opts.innerHTML="";
  const reveal=document.getElementById("tReveal").checked, force=document.getElementById("tForce").checked, showLocked=document.getElementById("tLocked").checked;
  let num=0;
  n.choices.forEach(ch=>{ const ok=choiceAvailable(char,st,ch,MODEL); if(!ok&&!showLocked) return; const idx=++num;
    const b=document.createElement("button"); b.className="opt"+(ok?"":" locked"); const chk=normCheck(ch), tag=tagFor(ch);
    const passive=chk&&isPassiveSkill(chk.skill);
    let tags=""; if(tag) tags+=`<span class="tg ${tag.cls}">${esc(tag.label)}</span>`;
    if(chk) tags+=`<span class="tg check">${passive?'👁':'🎲'} ${esc(chk.skill||ABBR[chk.ability]||chk.ability)}${passive?' · passive':''}</span>`;
    let inner=(tags?`<div class="tags">${tags}</div>`:"")+`<span class="num">${idx}.</span>${esc(stripBracket(ch.text)||"(continue)")}`;
    if(chk){ const bonus=checkBonus(char,chk,MODEL), prof=chk.skill&&isProficient(char,chk.skill,MODEL);
      if(passive){ const sc=10+bonus; inner+=`<div class="ck"><span class="chip">${esc(chk.skill)} (passive)</span><span class="chip ${sc>=chk.dc?'ok':'bad'}">${sc} vs DC ${chk.dc}${sc>=chk.dc?' ✓ auto':' ✗'}</span>${prof?`<span class="chip prof">proficient +${MODEL.proficiencyBonus}</span>`:''}</div>`; }
      else { const pct=Math.round(chanceToPass(chk.dc,bonus)*100); inner+=`<div class="ck"><span class="chip">${esc(chk.skill||ABBR[chk.ability])} DC ${chk.dc}</span><span class="chip ${pct>=50?'ok':'bad'}">${pct}% (d20 ${bonus>=0?'+':''}${bonus})</span>${prof?`<span class="chip prof">proficient +${MODEL.proficiencyBonus}</span>`:''}${ch.fail?`<span class="muted">miss ▸ another path</span>`:''}</div>`; } }
    if(!ok){ inner+=`<div class="fxline">🔒 ${lockReason(ch)}</div>`; }
    if(reveal){ const fx=describeEffects(ch.effects); if(fx) inner+=`<div class="fxline">▸ ${fx}</div>`; }
    b.innerHTML=inner; if(ok) b.onclick=()=>choose(n,ch,force); opts.appendChild(b);
  });
  if(!num){ const d=document.createElement("div"); d.className="muted"; d.textContent="(no choice here fits this character — toggle “show choices I don't qualify for”, or change who you are)"; opts.appendChild(d); }
  // the breadth readout — how many of ALL authored responses fit who you are & what you've done
  const total=n.choices.length, fits=n.choices.filter(ch=>choiceAvailable(char,st,ch,MODEL)).length;
  if(total>=8){ const tally=document.createElement("div"); tally.className="breadth";
    tally.innerHTML=`✦ <b>${fits}</b> of <b>${total}</b> authored responses fit who you are and what you've done — the rest are gated by race, gender, faith, class, alignment, your stats, your reckoning, and the choices behind you.`;
    opts.appendChild(tally); }
}
function lockReason(ch){ const t=tagFor(ch); const chk=normCheck(ch);
  if(chk && isPassiveSkill(chk.skill) && !passiveBeats(char,chk,MODEL)){ const sc=10+checkBonus(char,chk,MODEL); return `passive ${esc(chk.skill)} ${sc} &lt; DC ${chk.dc} — you don't notice this`; }
  if(t) return `needs ${t.cls==="faith"?"faith":t.cls}: ${esc(t.label)}`;
  return "a different character"; }
function choose(n,ch,force){ sfx('page'); pendingOpts.el.remove(); pendingOpts=null;
  addLine("me","You", stripBracket(ch.text)||"(continue)");
  const chk=normCheck(ch);
  // resolve a check result (r = {success,crit,fumble}) to the right node: crit→ch.crit, fumble→ch.fumble, else next/fail
  const resolve=r=>{ let next; if(r.crit&&ch.crit) next=ch.crit; else if(r.fumble&&ch.fumble) next=ch.fumble;
    else if(r.success) next=ch.next; else next=ch.fail||ch.next;
    applyEffects(st,ch.effects); renderState(); goNode(next); };
  if(chk && isPassiveSkill(chk.skill)){ // passive: it only appeared because you already pass — auto-success, no roll
    addLine("sys","",`(${chk.skill} — passive ${10+checkBonus(char,chk,MODEL)} vs DC ${chk.dc}: success)`);
    applyEffects(st,ch.effects); renderState(); goNode(ch.next); return; }
  if(chk){ const bonus=checkBonus(char,chk,MODEL); if(force) return offerForced(ch,chk,bonus,resolve); rollPreview(ch,chk,bonus,resolve); }
  else { applyEffects(st,ch.effects); renderState(); goNode(ch.next); }
}
// BG3-style anticipation: lay the whole roll bare, then let the player commit to it.
function rollPreview(ch,chk,bonus,resolve){
  const bd=rollBreakdown(char,chk,MODEL), pct=Math.round(bd.chance*100);
  const sgn=v=>`${v>=0?'+':'−'}${Math.abs(v)}`;
  const need = bd.onlyNat20 ? "a <b>natural 20</b>" : bd.onlyNat1 ? "<b>anything but a natural 1</b>" : `a <b>${bd.needShown}</b> or higher`;
  const box=document.createElement("div"); box.className="rollprev";
  box.innerHTML=`<div class="rp-head">🎲 ${esc(chk.skill||ABBR[chk.ability])} check <span>· ${ABBR[chk.ability]} · DC ${chk.dc}</span></div>`+
    `<div class="rp-calc"><span class="rp-die">d20</span> <span class="rp-comp">${sgn(bd.abilityMod)} ${ABBR[chk.ability]}</span>`+
      (bd.prof?` <span class="rp-comp prof">+${bd.profBonus} proficient</span>`:` <span class="rp-comp dim">+0 not proficient</span>`)+
      ` <b class="rp-tot">= ${sgn(bd.bonus)}</b></div>`+
    `<div class="rp-need">You need ${need} on the die.</div>`+
    `<div class="rp-bar"><div class="rp-fill ${pct>=50?'ok':'bad'}" style="width:${Math.max(3,pct)}%"></div><span>${pct}% to succeed</span></div>`+
    `<div class="rp-crit">✦ nat 20 → <b>critical success</b>${ch.crit?' · a special outcome':''} &nbsp;·&nbsp; 💀 nat 1 → <b>critical failure</b>${ch.fumble?' · a special outcome':''}</div>`;
  const roll=document.createElement("button"); roll.className="rp-roll"; roll.innerHTML="🎲 ROLL THE DICE";
  roll.onclick=()=>{ box.remove(); rollDice(chk,bonus,raw=>resolve(rollResult(raw,chk.dc,bonus))); };
  box.appendChild(roll);
  document.getElementById("dscript").appendChild(box); document.getElementById("dscript").scrollTop=1e9;
}
function offerForced(ch,chk,bonus,resolve){ const box=document.createElement("div"); box.className="opts";
  box.innerHTML=`<div class="muted" style="margin-bottom:4px">Preview any branch of this ${chk.skill||ABBR[chk.ability]} DC ${chk.dc} check:</div>`;
  const mk=(l,c,r,label)=>{const b=document.createElement("button");b.className="opt";b.innerHTML=`<span class="chip ${c}">${l}</span>`;b.onclick=()=>{box.remove();addLine("sys","",`(forced — ${label})`);resolve(r);};return b;};
  box.appendChild(mk("✦ NATURAL 20 — critical success","ok",{success:true,crit:true,fumble:false},"natural 20"));
  box.appendChild(mk("✓ success","ok",{success:true,crit:false,fumble:false},"success"));
  box.appendChild(mk("✗ failure","bad",{success:false,crit:false,fumble:false},"failure"));
  box.appendChild(mk("💀 NATURAL 1 — critical failure","bad",{success:false,crit:false,fumble:true},"natural 1"));
  document.getElementById("dscript").appendChild(box); document.getElementById("dscript").scrollTop=1e9; }
function rollDice(chk,bonus,onRaw){ const wrap=document.createElement("div"); wrap.className="dice";
  wrap.innerHTML=`<div class="d20wrap" id="d20w"><div class="d20face rolling">${D20_SVG}</div><span class="dnum">?</span></div><div class="calc">rolling ${chk.skill||ABBR[chk.ability]}…</div>`;
  const sc=document.getElementById("dscript"); sc.appendChild(wrap); sc.scrollTop=1e9; sfx('dice');
  let ticks=0; const w=wrap.querySelector("#d20w"), face=wrap.querySelector(".d20face"), num=wrap.querySelector(".dnum"), calc=wrap.querySelector(".calc");
  const spin=setInterval(()=>{ num.textContent=1+Math.floor(Math.random()*20); if(++ticks>=16){ clearInterval(spin); land(); } },45);
  function land(){ const raw=1+Math.floor(Math.random()*20), total=raw+bonus, r=rollResult(raw,chk.dc,bonus);
    num.textContent=raw; face.classList.remove("rolling"); void face.offsetWidth; face.classList.add("land");
    w.classList.toggle("nat20",r.crit); w.classList.toggle("nat1",r.fumble);
    calc.innerHTML=`${raw} ${bonus>=0?'+':'−'} ${Math.abs(bonus)} <b>= ${total}</b> vs DC ${chk.dc}`;
    const res=document.createElement("div"); res.className="res "+(r.success?"ok":"bad")+(r.crit?" crit":"")+(r.fumble?" fumble":"");
    res.textContent = r.crit?"✦ NAT 20!" : r.fumble?"💀 NAT 1!" : (r.success?"SUCCESS":"FAIL"); wrap.appendChild(res);
    sfx(r.crit?'crit':r.fumble?'fumble':r.success?'good':'bad'); setTimeout(()=>onRaw(raw),820); } }
function endScene(){ const sc=document.getElementById("dscript"); const e=document.createElement("div"); e.className="ending"; e.textContent="▪ The conversation comes to rest."; sc.appendChild(e);
  const b=document.createElement("button"); b.className="leavebtn"; b.textContent="↩ back to the market"; b.onclick=closeDialogue; sc.appendChild(b); sc.scrollTop=1e9; }

function addLine(cls,who,body){ const w=document.getElementById("dscript"); const d=document.createElement("div"); d.className="line "+cls;
  let sig=""; if(who&&cls==="" ){ sig=`<div style="font-size:11px;letter-spacing:1px;text-transform:uppercase;color:#c9a24b;margin-bottom:3px">${esc(who)}</div>`; }
  if(cls==="me") sig=`<div style="font-size:11px;letter-spacing:1px;text-transform:uppercase;color:#9fc0e0;margin-bottom:3px;text-align:right">You</div>`;
  d.innerHTML=sig+`<div class="body">${cls==="sys"?esc(body):prose(body)}</div>`; w.appendChild(d);
  if(cls!=="sys") linkifyEl(d.querySelector(".body")); }
// The Returned-sense inset (what your dead-touched soul perceives) and the LORE recall inset (5e knowledge).
function addSense(text){ const w=document.getElementById("dscript"); const d=document.createElement("div"); d.className="sense";
  d.innerHTML=`<span class="lk">✦ The Returned senses —</span><div>${prose(text)}</div>`; w.appendChild(d); linkifyEl(d); }
function addLore(e){ const w=document.getElementById("dscript"); const d=document.createElement("div"); d.className="lore";
  let h=`<span class="lk">👁 ${esc(e.skill)} (passive) — you recall:</span> ${prose(e.lore)}`;
  if(e.secret && secretKnown(char,e,MODEL)) h+=`<div class="deep"><span class="lk" style="color:#e7c873">…and, deeper (${esc(e.secretSkill)} ${e.secretDc}):</span> ${prose(e.secret)}</div>`;
  d.innerHTML=h; w.appendChild(d); linkifyEl(d); }
// inline glossary linking: walk text nodes, wrap each term's first occurrence per line in a hover-able span
function linkifyEl(el){ if(!el) return;
  const walker=document.createTreeWalker(el, NodeFilter.SHOW_TEXT, null); const nodes=[]; while(walker.nextNode()) nodes.push(walker.currentNode);
  const used={};
  nodes.forEach(node=>{ if(node.parentNode && node.parentNode.classList && node.parentNode.classList.contains("gloss")) return;
    let txt=node.nodeValue, out=null, m, idx=0, frag=null;
    GLOSS_RE.lastIndex=0; const re=new RegExp(GLOSS_RE.source,"gi");
    while((m=re.exec(txt))){ const hit=GLOSS_INDEX.find(o=>o.t.toLowerCase()===m[0].toLowerCase()); if(!hit||used[hit.i]){ continue; } used[hit.i]=true;
      frag=frag||document.createDocumentFragment(); frag.appendChild(document.createTextNode(txt.slice(idx,m.index)));
      const span=document.createElement("span"); span.className="gloss"; span.dataset.k=hit.i; span.textContent=m[0]; frag.appendChild(span); idx=m.index+m[0].length; }
    if(frag){ frag.appendChild(document.createTextNode(txt.slice(idx))); node.parentNode.replaceChild(frag,node); }
  });
}
// tooltip: hover any glossary term for the common-knowledge gloss (+ the deep lore if your character knows it)
const tip=document.getElementById("tip");
document.addEventListener("mouseover",e=>{ const g=e.target.closest&&e.target.closest(".gloss"); if(!g){ return; }
  const e0=GLOSS[+g.dataset.k]; if(!e0) return; const known=loreKnown(char,e0,MODEL), deep=secretKnown(char,e0,MODEL);
  tip.innerHTML=`<div class="tn">${esc(e0.term)}</div>${esc(e0.gloss)}`+
    (e0.skill?(known?`<div class="tl"><b>${esc(e0.skill)} — you recall:</b> ${esc(e0.lore)}</div>`:`<div class="tl" style="color:#6e6680">(a deeper ${esc(e0.skill)} insight escapes you)</div>`):"")+
    (deep?`<div class="tl" style="border-color:#5a4a2a"><b style="color:#e7c873">${esc(e0.secretSkill)} — and deeper:</b> ${esc(e0.secret)}</div>`:"");
  tip.style.display="block"; moveTip(e); });
document.addEventListener("mousemove",e=>{ if(tip.style.display==="block" && e.target.closest&&e.target.closest(".gloss")) moveTip(e); });
document.addEventListener("mouseout",e=>{ if(e.target.closest&&e.target.closest(".gloss")) tip.style.display="none"; });
function moveTip(e){ const pad=14, w=tip.offsetWidth||280, h=tip.offsetHeight||80;
  let x=e.clientX+pad, y=e.clientY+pad; if(x+w>innerWidth-8) x=e.clientX-w-pad; if(y+h>innerHeight-8) y=e.clientY-h-pad;
  tip.style.left=Math.max(8,x)+"px"; tip.style.top=Math.max(8,y)+"px"; }
function esc(s){ return (s==null?"":String(s)).replace(/[&<>]/g,c=>({"&":"&amp;","<":"&lt;",">":"&gt;"}[c])); }
function prose(s){ let h=esc(s); h=h.replace(/\*\(([\s\S]*?)\)\*/g,'<span class="stagedir">($1)</span>'); h=h.replace(/\*([^*]+)\*/g,'<em>$1</em>'); return h; }
function stripBracket(t){ return (t||"").replace(/^\s*\[[^\]]*\]\s*/,""); }
function describeEffects(effects){ return (effects||[]).map(e=>{ if(e.op==="SetTrue") return `sets <span class="chip fx">${esc(e.key)}</span>`;
  if(e.op==="AddInt") return `${esc(INT_LABELS[e.key]||e.key)} ${e.amount>=0?'+':''}${e.amount}`; return esc(e.key); }).join(" · "); }

// ---- SFX ----
let sfxOn=true, actx=null;
function toggleSfx(){ sfxOn=!sfxOn; document.getElementById("sfxToggle").textContent=(sfxOn?'🔊':'🔇')+' sfx'; }
function sfx(kind){ if(!sfxOn) return; try{ actx=actx||new (window.AudioContext||window.webkitAudioContext)(); }catch(e){ return; }
  const t=actx.currentTime,o=actx.createOscillator(),g=actx.createGain(); o.connect(g); g.connect(actx.destination);
  const env=(a,d,v)=>{g.gain.setValueAtTime(0.0001,t);g.gain.exponentialRampToValueAtTime(v,t+a);g.gain.exponentialRampToValueAtTime(0.0001,t+a+d);};
  if(kind==='step'){ o.type='sine'; o.frequency.setValueAtTime(150,t); env(0.004,0.04,0.025); o.start(t);o.stop(t+0.06); }
  else if(kind==='talk'){ o.type='triangle'; o.frequency.setValueAtTime(360,t); o.frequency.exponentialRampToValueAtTime(540,t+0.07); env(0.008,0.08,0.05); o.start(t);o.stop(t+0.11); }
  else if(kind==='page'){ o.type='triangle'; o.frequency.setValueAtTime(420,t); o.frequency.exponentialRampToValueAtTime(640,t+0.06); env(0.008,0.07,0.045); o.start(t);o.stop(t+0.1); }
  else if(kind==='dice'){ o.type='square'; o.frequency.setValueAtTime(180,t); o.frequency.linearRampToValueAtTime(120,t+0.12); env(0.005,0.12,0.04); o.start(t);o.stop(t+0.14); }
  else if(kind==='good'){ o.type='sine'; o.frequency.setValueAtTime(523,t); o.frequency.setValueAtTime(784,t+0.09); env(0.01,0.22,0.06); o.start(t);o.stop(t+0.26); }
  else if(kind==='bad'){ o.type='sawtooth'; o.frequency.setValueAtTime(196,t); o.frequency.exponentialRampToValueAtTime(98,t+0.2); env(0.01,0.22,0.05); o.start(t);o.stop(t+0.26); }
  else if(kind==='crit'){ o.type='sine'; [523,659,784,1047].forEach((f,k)=>o.frequency.setValueAtTime(f,t+k*0.08)); env(0.01,0.42,0.08); o.start(t);o.stop(t+0.5); }       // little triumphant fanfare
  else if(kind==='fumble'){ o.type='sawtooth'; o.frequency.setValueAtTime(330,t); o.frequency.linearRampToValueAtTime(110,t+0.45); env(0.01,0.45,0.07); o.start(t);o.stop(t+0.5); } // sad trombone
}

// ---- boot ----
renderBuilds(); renderState(); requestAnimationFrame(loop);
</script>
</body></html>"""

out = (HTML.replace("__BLOB__", BLOB)
       .replace("__BUILDS__", json.dumps(BUILDS, ensure_ascii=False))
       .replace("__INTLABELS__", json.dumps(INT_LABELS, ensure_ascii=False)))
dst = os.path.join(ROOT, "play", "town_market.html")
open(dst, "w", encoding="utf-8").write(out)
_npcs = sum(len(s['npcs']) for s in ALL_SCENES.values())
_beats = sum(len(c['nodes']) for c in ALL_CONVS)
print(f"wrote play/town_market.html ({len(out)//1024} KB) — {len(ALL_SCENES)} walkable zones, {_npcs} souls, "
      f"{_beats} dialogue beats, {len(ALL_CONVS)} conversations")
