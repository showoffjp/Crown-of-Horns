# 🎲 The Wayward Mile — *what the road throws*

> A **random-event system**: a deep pool of wildly idiosyncratic one-off encounters, surfaced rarely and
> at random, so **no two playthroughs see the same handful.** Each event is *very* unique, *very*
> memorable, and happens to a soul **exactly once** — and a single passage only ever spends a few of
> them. Reached from the causeway via *"a path that wasn't there a moment ago…"* — the **Wayward Mile**
> (`play/caprice.json`), a milestone at a bend you're fairly sure was straight a breath ago.

---

## The mechanic — `draw` (built & tested)
A new, reusable engine primitive: a node can carry a `draw` list instead of a fixed `next`. On entry the
renderer **routes at random** to one of the events — *but* it:
- **skips events already seen** (each draw entry has a `once` flag the event's entry node sets), so an
  event **never repeats** in a run;
- **honors a per-run cap** (`drawMax` on a `drawCount` key — currently **4**), so a single passage only
  ever spends a *handful* of the pool no matter how many times you return;
- **falls through to `drawElse`** (a "the road has gone still and ordinary… it will be different, next
  life" beat) when capped or drained.

Because `pickDraw` is a pure function (`pickDraw(node, state, rnd)`), it's **unit-tested deterministically**
in `dialogue_sim.test.js` (rnd→0 picks the first unseen; seen events are skipped; the cap and the drained
pool both fall to `drawElse`). The draw node also carries an `auto` equal to its `drawElse`, so any engine
path that doesn't understand `draw` (e.g. the structural `autoPlay` validator) still **completes
gracefully**. Runtime randomness uses `Math.random` in the browser build — never in the tested paths.

> **Why this scales to "extremely unique each playthrough":** the pool is meant to grow to **dozens**.
> The per-run cap stays small (~4). So a run draws a few, never repeats them, and across the large pool,
> *which* few you get — and in what order, with what crit/fumble swings — is different every life.

## The first batch — six one-offs, no two alike in tone
| Event | Shape | The hook |
|---|---|---|
| 🪙 **The Coin That Calls Heads** | a **gamble** (Perception check, crit/fumble) | A dead gambler whose coin *is his own worn-thin soul.* Win → a true thing about the road (*you've walked it before*). **Crit:** the coin lands on its **edge** — the one outcome the loop has no rule for — and he gives it to you. **Fumble:** he drops himself down a crack in the world. |
| 🐕 **The Same Dog** | **wordless, moving** (canon) | The dog that hears the Wall's song drops a bone scratched with a name — *Elsa.* Keep it, bury it, or **follow** it — and learn it has been carrying names to the dissolving, every day, for nothing, because the dead are only souls waiting by a door no one opens. |
| ⚖️ **The Last Word** | **devastating** (canon: Corval) | Corval the Weeping Auctioneer auctions a stranger's **final word** — paid not in coin but in *carrying their name.* Bid, and hear it: *"…oh, how silly. I've left the kettle on."* — Hettie Mallow, 81, and now *someone knows.* |
| 🫂 **The Recursive Beggar** | a **puzzle of generosity** (Insight) | A soul made entirely of giving, who *splits in two every time you give him something.* The cure is the one alms that doesn't divide him: **refusal** — *"you already gave; it counted; you're allowed to keep yourself."* |
| 🌑 **The Apocalypse Salesman** | **comic → dark** (Myrkul tie) | A chipper door-to-door salesman hawking *"one apocalypse, gently used, one careful owner (a god)."* See the truth: it's Myrkul's discarded end-of-world, and the salesman a soul *bound to sell the thing that consumed him* — even the apocalypse is just one more discarded soul. **Buy** it and you free him, and the world's ending is yours, to never use. |
| 🃏 **The Mid-Joke Comedian** | **comic / sad** (Performance, crit/fumble) | A jester frozen three centuries one beat before the punchline he died before delivering. **Land it** (crit: the funniest sound in the afterlife; fumble: you bomb, *confidently*, in front of a dead pro) — or **leave him the setup**, because for a comic the laugh is a little death and the *almost* is the joke alive forever. |

Every event sets reactive flags + dispositions (mercy, haunt, cunning, heretical) and leaves a keepsake
or a mark — *the edge-coin, the name Elsa, Hettie's last word, the apocalypse in a case* — so the
caprices aren't dead-ends; they ripple.

## Rollout
This is the **system + the first six.** The pool is built to grow — each new event is just another node
subtree plus one line in the `draw` list and a `once` flag; the cap and the router need no further
change. Future batches can also seed Caprice exits from **other** hubs (the device already fits the
world's *"a stair that wasn't there a moment ago"* idiom), and the surfacing can later move into
`loadScene` for true mid-travel ambushes.

> Design intent: *the strange visits a soul only so many times in one passage.* Make each visit
> unforgettable, make them rare, make them never the same twice — and the world feels **alive and
> infinite**, which is the whole point of a road that is never where you left it.
