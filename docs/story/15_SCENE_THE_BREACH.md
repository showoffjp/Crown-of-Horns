# 🕳️ Scene — *"The Breach"* (Act II climax · the early permanent loss)

> The game's first irreversible loss — the beat that teaches the player *this story will take things
> and keep them.* It fuses permanence, complicity, agency, and setup→payoff, and it makes *which*
> companion is lost depend on the player's earlier care (the authored-fate system from
> `13_TWIST_ARCHITECTURE.md` §II). Prose treatment, then the branching script, then the fate logic.

> **Context:** you have descended into the Fugue and pulled **Maerin** out of the Wall of the
> Faithless — the act feels like a *victory.* It is not free.

---

## Prose

For one bright, impossible moment, it works. Maerin comes loose from the mortar of the damned with a
sound like a held breath finally let go, and she is *whole* — luminous, gasping, gloriously furious —
and you have your hands full of a saved girl and you think, *we did it, we actually—*

And then the Wall screams.

Not the small constant murmur of the dissolving dead. A *structural* scream, the sound of a thing
that has stood since before the gods deciding it will not give up a soul without taking one back. The
breach you tore to free Maerin yawns wider, and the grey beyond it is not grey anymore — it is hungry,
and it is reaching, and the floor of the afterlife tilts toward the dark.

*"It wants the balance kept,"* someone says — Garrow, or Naeve, whoever knows the rites — *"a soul out
means a soul in. It'll take all of us to close it, or it'll take one of us to* hold *it. From the
inside."*

From the inside. A one-way door. The Wall will have its tithe.

And here is where the story stops protecting you. Because the Wall does not wait for a vote. It reaches
— and it reaches *first* for the one among you it can already feel, the one with a debt to the dark, the
one whose soul was always, in some ledger, already half-claimed.

*(If you never freed Varra from her pact:)* It takes Varra. Of course it takes Varra. She has had a
receipt for her soul since she was six, and the Wall reads receipts. The leash you never helped her
cut goes taut, and the dark yanks, and she is at the breach before any of you can move, her familiar
shrieking, her hands scrabbling at a threshold that has decided. She has time for exactly one line, and
she spends it not on a curse but on a plea, and then the grey closes over her like water over a stone,
and the breach seals, and the Wall is quiet, and there are fewer of you than there were a moment ago,
and there will be fewer of you forever.

*(If you freed her — if you did the work — )* the leash is gone; the Wall cannot find its purchase on
her, and in the awful pause where it gropes for someone to take, a companion *steps forward.* Chooses
it. Mordin-style, eyes open, for their own reasons — and gives you a last line you will carry the rest
of the game.

Either way, you climb out of the Fugue with Maerin alive in your arms and a hole in your party that no
revival, no wish, no god will fill. Either way, the next time the game threatens someone you love, you
will *believe* it.

That is what the Breach is for.

---

## Branching Script

### node `breach.0` — (narration / party)
"Maerin comes free — and the Wall *screams.* The breach tears wide; the grey beyond it reaches for the
living. Garrow's voice cuts through: *'A soul out means a soul in! Close it together, or someone holds
it from the inside — and the inside is a one-way door!'*"
→ auto `breach.1`

### node `breach.1` — (the Wall reaches)
*(The system resolves WHO based on flags — see Fate Logic below. The Wall reaches first for its
likeliest tithe; the player gets one frantic choice that can *change the cost but not erase it.*)*
- → "Take my soul instead — I came out of this Wall, send me back!" → `breach.self` *([the noble lie])*
- → "Everyone grab on — we close it *together!*" → `breach.together`
- → *(reach for the one the Wall is taking)* → `breach.reach`

### node `breach.self` — The Wall / Maerin
*(You offer yourself. The Wall — and Maerin — refuse it, and the refusal is its own gut-punch:)*
"The grey recoils from you — you are the *crack*, the one soul it cannot simply re-file, and it will
not be cheated of a *clean* tithe. Maerin's hand closes on your wrist: *'No. NO. Not you — I did not
claw out of that dark to watch you climb back in for me. Don't you DARE—'* The Wall takes its tithe
elsewhere." → `breach.resolve`

### node `breach.together` — (the attempt)
"You all seize the breach and *pull.* For a heartbeat it might hold — and then the Wall takes the one
it had already chosen, tearing them from the chain, and the rest of you are flung back as it seals.
Together was always going to be one short." → `breach.resolve`

### node `breach.reach` — (the attempt)
"You lunge for them — fingers close on cloth, on a hand, on nothing. The grey is faster. It was always
faster. It has had ten thousand years of practice and you have had a heartbeat." → `breach.resolve`

### node `breach.resolve` — (THE LOSS — text varies by who)

**If Varra (leashed, not freed):**
> Varra is at the breach before any of you can move — the pact-leash gone taut, the dark collecting on
> a debt six years old. Spite shrieks and cannot reach her. She has time for one line:
> *"Of course it's me. It was always going to be me — I've had a receipt since I was six and the Wall
> reads receipts."* *(and then, not a curse — a plea:)* *"Just— don't let it have been for nothing.
> That's the one thing I can't—"*
> The grey closes over her like water over a stone.
> *(fx: SET companion.varra.lost, remove Varra permanently, SET breach.cost=varra)*

**If a volunteer (player freed Varra; highest-bond or arc-appropriate companion steps forward):**
> *(Ilfaeril, if present and high-bond:)* "Ten thousand years I have guided souls *past* this Wall.
> Let me, for once, hold the door open from the *inside.* It is the only verdict I ever wished to cast.
> Take the girl. Tell her an old man finally did one thing right."
> *(He steps into the grey before you can stop him, and holds the breach with his hands and his oath,
> and is gone, and the Wall is quiet.)*
> *(fx: SET companion.<id>.lost, remove that companion permanently, SET breach.cost=volunteer,
> appr.all +10 [grief solidarity])*

**If neglect (no bonds raised; lowest-approval companion taken impersonally):**
> The Wall takes the one of you it barely had to reach for — the one you never quite let in, who never
> quite let you. There are no last words. There was never enough between you for last words. That, too,
> is a kind of grief, and it is the one you chose.
> *(fx: SET companion.<id>.lost, remove permanently, SET breach.cost=neglect)*

### node `breach.after` — Maerin
*(climbing out of the Fugue, Maerin — alive — looks at the empty space where a companion was)*
"...I didn't ask for this. I want you to know that. I didn't ask to be worth—" *(she can't finish)*
"Whoever they were. They paid for me. I'm going to *be worth it.* I don't know how yet. But I am."
→ END · *(fx: SET maerin.recruited, SET act2.breach_done)*

---

## Fate Logic (who the Wall takes)

Resolved at `breach.1`, in priority order:
1. **Varra present AND her pact not severed** (`!companion.varra.pact_broken`) → **Varra is taken.**
   (Her Act I "leash" secret pays off. The most thematically perfect default.)
2. **Else, a volunteer steps forward** — the first match among present companions:
   Ilfaeril (`oath of redemption — wants the door`) → the Mournlight (`offers gladly`) → Garrow
   (`belongs to the dead`). The volunteer is lost; party gains grief-solidarity approval.
3. **Else (no eligible volunteer / all bonds neglected)** → the **lowest-approval** present companion
   is taken impersonally.

> **Permanence is absolute:** the lost companion is removed for the rest of the game — their slot
> empty, their romance (if any) ended, their banter gone, their personal quest failed in the journal.
> No revival exists in this game *by design.* (Aerith's law.) The loss is *authored* by the player's
> prior care — saving Varra in time, or raising a companion's bond high enough that they choose the
> sacrifice with dignity rather than being snatched without words — which makes it grief the player
> *owns*, not grief inflicted on them.

> **The teaching:** after the Breach, every later threat in the game is *believed.* The finale's
> stakes are no longer theoretical, because the player has already lost someone to this story and
> watched it refuse to give them back.
