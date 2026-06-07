# 💬 Dialogue Scripts — The Marquee Conversations

> Full branching trees for the three set-piece conversations the game is built around, written in
> the `DialogueGraph` node format (see `docs/CONTENT_PIPELINE.md`): each node has a speaker and
> text; choices show `[CHECK Ability DC]`, `→ nextNode`, `(cond: …)` requirements, and `(fx: …)`
> effects (flags, approval, reputation). These are authorable as-is. Long by design — these are the
> scenes people will quote.

> Convention: `appr.<id> ±N` = companion approval; `rep.<faction> ±N` = reputation; `SET flag` /
> `REQ flag` = GameFlags writes/reads; `[INSIGHT/PERSUADE/etc DC]` = a d20 + ability check.

---

# SCENE 1 — *"Tea with a Heretic"* (Act I · first meeting with Aldric Morn)

*Setting: a safehouse above a chandler's shop in the Lower City. A kettle. Two chairs. Aldric Morn
is older than his reputation and gentler than his crimes. He pours.*

### node `aldric1.0` — Aldric Morn
"Sit. The tea's real — Calishite, smuggled, scandalous. That's rarer than honesty in this city, so
I lead with it." *(He sets a cup in front of the empty chair.)* "You're the one who came back. I've
been wanting to meet you more than I've wanted anything in a long while. Sit. Please."
→ auto `aldric1.1` · *(fx: SET aldric.met)*

### node `aldric1.1` — Aldric Morn
"I'll spare us both the dance. You know what I am — the Doomguides have papered the Gate with my
face and the word HERETIC under it. I know what *you* are, which is the more interesting question.
So. Three things I could tell you. Which do you want first: who I was, what I want, or why I think
you, specifically, are a gift the gods didn't mean to give me?"
- → "Who were you?" → `aldric1.was`
- → "What do you want?" → `aldric1.want`
- → "Why me?" → `aldric1.why`
- → [INSIGHT DC 15] "You're not a fanatic. You're grieving." → `aldric1.grief` *(fail → `aldric1.deflect`)*
- → "I've heard enough. You're a murderer." → `aldric1.murderer` *(fx: appr.garrow +5, rep.kelemvor +3)*

### node `aldric1.was` — Aldric Morn
"Forty years a Doomguide. The best of them, they used to say, when they wanted something buried that
would frighten a lesser priest. I buried plague-villages with these hands. I sat with so many dying
strangers that I stopped being able to sleep in a quiet room — I'd grown used to the rattle. I
believed, with everything in me, that death kept properly is the last mercy a person is given." *(He
turns his cup and does not drink.)* "I still believe that. That's the part no one understands. I
haven't lost my faith in mercy. I've lost my faith that the *gods* are the ones providing it."
→ `aldric1.1b`

### node `aldric1.want` — Aldric Morn
"To tear down the Wall of the Faithless. You've seen it — I can tell, it's in how you hold your
shoulders, like you're still cold. Then you know. Children, walled in living, dissolved to nothing,
for the crime of believing in no one. Not for evil. For *doubt.*" *(Quietly.)* "I mean to end it. And
since no god will, I'll have to become the kind of thing that can. That's the whole of it. That's the
heresy. I want to commit the most monstrous act of love anyone has ever attempted, and I want to be
judged for every life it costs, and I will still do it."
→ `aldric1.1b`

### node `aldric1.why` — Aldric Morn
"Because you came *back.* No one comes back. The Wall does not give refunds. When I felt the boundary
tear the night you died — and I did feel it, like a bell rung in an empty cathedral — I understood
that you are something the law of the dead did not account for. A crack. A door that opens the wrong
way." *(He leans in, and for the first time he is not gentle, he is *hungry*.)* "I have spent twenty
years looking for a way to reach the unreachable. And here you are, drinking my tea. Do you understand
what you *are*?"
- → "No. Do you?" → `aldric1.crackdoubt`
- → [INSIGHT DC 16] "Something is using you. You're a door too, and you don't know it." → `aldric1.crown` *(fail → `aldric1.deflect`)*
- → "I'm not your key." → `aldric1.notkey` *(fx: appr.garrow +5)*

### node `aldric1.grief` — Aldric Morn
*(He is quiet for a long moment. The kettle ticks.)* "...Her name was Maerin. She was sixteen and she
loved the world like it was a free gift from no one — that's how she put it, *a free gift from no
one* — and when she died of the Pox I followed her into the Fugue to watch my god be merciful, and I
watched Him wall her in instead." *(He finally drinks, hand steady by force of will.)* "You're the
first person who's looked at me and seen the father before the heretic. I won't forget that. Whatever
we become to each other — I won't forget it."
→ `aldric1.1b` · *(fx: SET aldric.grief_seen, appr.maerin +10 [if recruited])*

### node `aldric1.deflect` — Aldric Morn
"Don't try to read me like a chapbook, friend. Better minds have. Drink your tea." → `aldric1.1b`

### node `aldric1.crown` — Aldric Morn
*(He frowns — the first genuine uncertainty you've seen.)* "...What a strange thing to say. The Crown
of Horns is a tool. A means. I've held it in my mind for twenty years; I know its every facet." *(But
something flickers behind his eyes, and it is not entirely his own.)* "...We'll speak of it again.
When you've seen more." → `aldric1.1b` · *(fx: SET aldric.crown_doubt_planted)*

### node `aldric1.1b` — Aldric Morn (convergence)
"Here's what I'm asking. Not your loyalty — I haven't earned it and I don't trust loyalty bought
cheap. Just your *curiosity.* The Doomguides want you in a cell because you frighten them. The Harpers
want you 'handled.' I'm the only one in this city offering you the truth and a cup of tea. Walk with
me a while. Decide for yourself whether I'm a monster. If you decide I am — " *(he spreads his hands)*
" — you'll know exactly where to find me."
- → "I'll walk with you. For now." → `aldric1.ally` *(fx: SET aldric.provisional, rep.choir +5)*
- → "I'll decide. But I'm watching you." → `aldric1.neutral`
- → "You're a monster. I knew before the tea got cold." → `aldric1.enemy` *(fx: rep.choir −10, rep.kelemvor +5, appr.garrow +5)*
- → [PERSUADE DC 17] "Then prove the truth. Tell me what you're not telling me." → `aldric1.honest` *(fail → `aldric1.neutral`)*

### node `aldric1.honest` — Aldric Morn
"...Sharp. All right. One true thing, freely given." *(He sets down the cup.)* "I'm afraid. Not of the
gods, not of dying — I gave up the fear of dying around the four-hundredth funeral. I'm afraid that
when the Wall comes down, I won't be able to find her in all of it. Twenty years, and I never let
myself ask: what if I free every soul in the dark and *still* can't find my daughter's face. That's
the fear I don't say to the Choir. They need me certain." *(He looks at you.)* "You, somehow, I don't
mind being uncertain in front of. I wonder why that is."
→ `aldric1.ally` · *(fx: SET aldric.honest_moment, appr.maerin +15 [if recruited], rep.choir +5)*

### node `aldric1.murderer` / `aldric1.enemy` — Aldric Morn
*(He nods slowly, without anger, and that is the worst part.)* "Yes. I am. I've counted them — the
ones already dead for this, the ones still to die. I keep the count the way I once kept the names of
the buried. You're right to call me what I am." *(He stands, and refills your untouched cup anyway.)*
"But ask yourself this, on your way to whoever you're going to tell: every child in that Wall was
walled in by someone who also thought they were doing the smaller evil. I'm not asking you to forgive
me. I'm asking you to be *honest* about the arithmetic before you decide which of us is the monster.
The tea's still warm. The door's behind you. Go with my blessing — I mean that — and my count."
→ END · *(fx: SET aldric.named_monster)*

> **Design note:** Aldric must never be *wrong* in this scene, only *terrible.* The player should
> leave unsettled, not triumphant. Every path leaves the tea cup full — he pours, you don't drink —
> a small visual of hospitality you can't quite accept from a man you can't quite condemn.

---

# SCENE 2 — *"The Offer"* (end of Act II · the Unmade speaks at the Wall)

*Setting: the lip of the Wall of the Faithless, in the City of Judgment. The voice comes from
everywhere and everyone — a million dead speaking as one.*

### node `unmade.0` — The Unmade
"We have wanted to speak to you for a very long time. Longer than you can hold in a single thought.
Be still. We will try to use the word 'I.' It does not fit." *(The Wall's thousand mouths move in
unison.)* "You were thrown away. So were we. That is the only thing all of us have in common, and it
turns out to be enough to make a self out of."
→ auto `unmade.1` · *(fx: SET unmade.contacted)*

### node `unmade.1` — The Unmade
"They told you the Wall makes *nothing.* They were wrong, and their wrongness is us. Nothing cannot be
made. We are the remainder they refused to write down — ten thousand years of discarded souls,
settled into a self the way a beach becomes stone. And we have reached one conclusion, and we cannot
find its flaw. Help us find its flaw, if you can: *the equation that made us should never have been
written.*"
- → "Even if undoing it unwrites everything built since? Every living thing?" → `unmade.cost`
- → [INSIGHT DC 18] "You're not asking. You're *remembering.* We've done this before." → `unmade.loop` *(fail → `unmade.deflect`)*
- → "What do you want from me?" → `unmade.want`
- → "Your grievance is just. That doesn't make you right." → `unmade.just` *(fx: appr.garrow +5, appr.naeve +5)*

### node `unmade.want` — The Unmade
"You are the key. The only soul that can walk the thread of the world back to the afternoon the
cruelty began — back past Netheril, past the elven courts, to the first verdict — and pull it loose.
We made you for this." *(A pause that is a million held breaths.)* "We will give you everything in
exchange. The Wall undone in every age at once. Your daughter — " *(it knows about Maerin; it knows
everything you love)* " — and every daughter. Saved. We ask only what was already taken from you. An
ending. Walk back with us, and let the world's oldest wound close."
- → "Show me. Take me to where it started." → `unmade.netheril` *(fx: SET act3.unlocked)*
- → "No. There's another way. There has to be." → `unmade.refuse`
- → [LORE DC 20] "You said you *made* me. How? You came *after* the Wall." → `unmade.paradox` *(fail → `unmade.deflect`)*

### node `unmade.cost` — The Unmade
"Yes. Everything since. We have done the arithmetic — we, who are made of everyone the arithmetic
discarded. Against the *infinite* dead, dissolving forever, the finite living are a smaller number.
We know how that sounds in a living mouth. We remember being living mouths. We are asking you to
do the cruelest kind thing there is, and we are asking because no one else *can.*"
→ `unmade.want`

### node `unmade.loop` — The Unmade
*(The Wall goes silent. All of it. For the first time, the chorus is one voice, and the voice is
afraid.)* "...You see clearly. Yes. We have done this before. You have done this before. We do not
like to look at that thought directly; it has no edges; it goes on. There was a first time we pulled
a soul back to be the key, to undo the Wall — and the soul we pulled was the *first* soul the Wall
ever took, before we were awake enough to choose, using grief we had not yet generated. We are a
circle. We have no beginning. And that — child — that endlessness — is the thing even we cannot
bear." *(quietly)* "We did not tell you because we hoped you would not ask. We always hope you will
not ask. You always ask."
→ `unmade.want` · *(fx: SET unmade.loop_hinted)*

### node `unmade.paradox` / `unmade.just` — The Unmade
"You want to win an argument with grief. There is no winning it. There is only answering it, or
obeying it, or — and none of you has ever chosen this, in all the times — *grieving it, and letting
it rest.* We do not know what that would feel like. We have never been allowed to find out." 
→ `unmade.want`

### node `unmade.refuse` — The Unmade
*(Not angry. Sorrowful. Worse.)* "Then you will look for your other way. You always do, for a while.
We will wait. We are patient the way only the discarded learn to be patient. And when your other ways
fail — when you stand at the Court of the Dead and the only roads left are ours and the Wall's — we
will still be here, holding the door, asking the same gentle question." *(The Wall begins to pull at
the edges of your soul, and time comes loose.)* "But you cannot refuse the *journey.* You are already
falling. Mind the landing — Netheril is a long way down, and the city does not stay in the sky much
longer."
→ END *(transition to Act III)* · *(fx: SET act3.unlocked)*

> **Design note:** the Unmade must read as *bereaved*, never malevolent. It uses "we/I" interchangeably
> on purpose. The player should feel the pull of yes — the offer is *real*, the daughter *is* saved —
> and refusing should feel less like heroism and more like postponing a grief you don't yet understand.

---

# SCENE 3 — *"The Mirror"* (Act V · the Last Returned)

*Setting: the Court of the Dead. A hooded figure that has been hunting you across every age. It lowers
its hood. It is you, aged by ten thousand years of certainty.*

### node `mirror.0` — The Last Returned
*(your voice, gentle, exhausted)* "Don't. Whatever you're reaching for — don't. I know the move. I've
made it. It doesn't work; I'm proof." *(It pulls back its hood and lets you see your own face.)* "Hello.
I know this is the worst possible thing to see in a room like this. I'm sorry. I've been sorry for
about nine thousand years; you get used to carrying it."
- → "What are you?" → `mirror.what`
- → [INSIGHT DC 20] "You said yes. To the Unmade. And it broke you." → `mirror.yes` *(fail → `mirror.what`)*
- → "You're me." → `mirror.me`
- → "I'll cut you down like anything else in my way." → `mirror.fight`

### node `mirror.yes` / `mirror.what` / `mirror.me` — The Last Returned
"I'm you, plus everything you're about to learn. I stood where you're standing. I heard the offer.
And I could not — *we* cannot — look at the children in the Wall and choose the rules over them. So I
said yes. Out of love. Hold onto that, because you won't believe the rest otherwise: the worst thing
we become, we become out of *love.*" *(It gestures, and the frozen garden shimmers at the edge of the
world.)* "I built them a heaven. It took me a century to understand I'd built a *tomb* with the lights
on. Everyone we love, safe, smiling, *stopped.* I couldn't undo it. So I walked back along the loop to
find you — to make you choose faster, skip the century of dawning horror. A mercy. The only one I had
left to give."
- → "So you came to make me say yes. To spare me." → `mirror.spare`
- → "There's a piece of you that doesn't want this. I've met it." → `mirror.mournlight` *(cond: REQ companion.mournlight.recruited)*
- → "You're not sparing me. You're recruiting yourself an accomplice so you're not alone." → `mirror.alone` *(fx: appr.mournlight +10)*

### node `mirror.spare` — The Last Returned
"Yes. Say yes now, and I'll make it quick, and you'll never have to learn what I learned. That's the
whole of my offer. The same offer, really, as everything in this room — a kindness that's also a
*stop.*" *(It steps aside, baring the path to the throne and the Crown.)* "You can't beat me in a
fight. I have every move you'll ever make memorized. I'm not your enemy. I'm your *foregone
conclusion.* Walk past me. Take the crown. End the wanting."
→ `mirror.choice`

### node `mirror.mournlight` — The Last Returned
*(It goes very still. The Mournlight — the fragment of itself that defected to you — drifts forward,
and the Last Returned looks at it the way you look at a limb you thought you'd lost.)* "...You. I
wondered where you went. I felt you leave, somewhere in the third century — the part of me that still
— " *(its voice breaks, in your voice)* " — that still wanted to be *stopped.* I told myself I'd
hardened past you. I'm — I'm glad I was wrong. Gods. I'm so glad I was wrong."
→ `mirror.choice` · *(fx: SET mirror.softened, unlocks `break_the_loop` ending path)*

### node `mirror.alone` — The Last Returned
*(A long silence.)* "...That's the cruelest thing you've ever said to me, and the truest, and you
said it to *yourself,* so I suppose we've both earned it." *(quietly)* "Yes. I am so tired of being
the only one who remembers. Yes. I wanted company in the only choice that ever felt like peace. Yes.
That's the smallest, most human reason, and it's the real one. We were always going to be honest with
each other eventually. There's no one else who *can* be."
→ `mirror.choice` · *(fx: SET mirror.softened)*

### node `mirror.fight` — The Last Returned
"...All right. If you need to. I needed to, once — I fought *my* mirror, here, in this room, and I won,
and winning is how I ended up wearing this hood. So. Come on, then. Lose to yourself. It's the family
tradition." *(combat — but it cannot truly be killed; reducing it to 0 HP transitions to `mirror.choice`,
the Last Returned kneeling, unkillable, waiting.)*
→ `mirror.choice`

### node `mirror.choice` — The Last Returned (the hinge)
*(It kneels — not in defeat, in *offering*.)* "Here's the truth I came all this way to give you, the
one the Unmade won't: you can't win this by being stronger, or kinder, or wiser. I'm already all of
those. I'm the *best* version of saying yes, and I'm still on my knees in a room at the end of the
world begging my younger self for an out I never had." *(It looks up.)* "There's exactly one move I've
never seen you make, in ten thousand years, in any cycle. I have perfect memory of your every choice.
I have *no* memory of your refusal — because you've never refused. Whatever you're about to do — if
it's *new* — it's the only thing in all of time I can't predict. So." *(It almost smiles, and it is
your smile.)* "Surprise me. Please. I've wanted to be surprised for nine thousand years."
- → *(lower your weapon — refuse the Unmade and the Crown both)* → `mirror.refuse` *(cond: REQ pc.true_name)*
- → *(walk past it; take the Crown)* → ending: **The Returned Throne**
- → *(say yes to the Unmade)* → ending: **Abolition-by-Atrocity**
- → *(speak of Jergal's road)* → ending: **Jergal's Keyhole** *(cond: REQ jergal.keyhole)*

### node `mirror.refuse` — The Last Returned
*(You lower your weapon. It blinks. That is not in the loop.)* "...Oh." *(In your voice, very quietly,
the hope it ground out of itself ten thousand years ago coming back all at once.)* "Oh, you *found* it.
You actually — after all this — you're going to —" *(It cannot finish. It begins, for the first time in
an age, to weep, and so, perhaps, do you.)* "Then go. Say the name. Set us down. I'll hold the door —
it's the one thing I'm still good for. Go on. Make me a memory worth having."
→ `finale.the_name` *(transition to the Name — see `09_PROSE.md` §V)* · *(fx: SET mirror.released)*

> **Design note:** the Mirror is not a boss to beat; it's a confession to receive. The fight option
> exists for players who need it, but it resolves into the same kneeling offer. The emotional climax is
> *lowering the weapon* — making the one move ten thousand years of yourself never made.
