// ============================================================================
// Narrative engine — headless port of Core/GameFlags.cs + the gating/prose surface
// of Core/EndingResolver.cs. Proves "choices -> earned endings" runs without Unity.
// (Epilogue slides are omitted — large and untested; the gating is the brain.)
// ============================================================================

// ---- GameFlags.cs ----------------------------------------------------------
class GameFlags {
  constructor() { this.bools = {}; this.ints = {}; }
  GetBool(k) { return this.bools[k] === true; }
  SetBool(k, v) { this.bools[k] = v; }
  GetInt(k) { return this.ints[k] || 0; }
  SetInt(k, v) { this.ints[k] = v; }
  AddInt(k, d) { this.SetInt(k, this.GetInt(k) + d); }
  AdjustApproval(id, d) {
    const k = `companion.${id}.approval`;
    this.SetInt(k, Math.max(-100, Math.min(100, this.GetInt(k) + d)));
  }
}
GameFlags._current = new GameFlags();
Object.defineProperty(GameFlags, "Current", { get() { return GameFlags._current; } });
GameFlags.Replace = (f) => { GameFlags._current = f || new GameFlags(); };

// ---- EndingResolver.cs (gating + prose) ------------------------------------
const Ending = {
  Abolition: "Abolition", DoomguidesPeace: "DoomguidesPeace", JergalsKeyhole: "JergalsKeyhole",
  ReturnedThrone: "ReturnedThrone", BreakTheLoop: "BreakTheLoop", MortalMeasure: "MortalMeasure"
};
const ALL_ENDINGS = Object.values(Ending);

const EndingResolver = {
  Available() {
    const f = GameFlags.Current;
    const list = [Ending.Abolition, Ending.ReturnedThrone, Ending.MortalMeasure];
    if (f.GetInt("faction.kelemvor.reputation") >= 5 || f.GetBool("aldric.named_monster"))
      list.splice(1, 0, Ending.DoomguidesPeace);
    if (f.GetBool("readers_boon") && f.GetBool("crownwars.verdict_spared"))
      list.push(Ending.JergalsKeyhole);
    if (f.GetBool("readers_boon") || f.GetBool("pc.true_name"))
      list.push(Ending.BreakTheLoop);
    return list;
  },
  IsGolden(e) { return e === Ending.JergalsKeyhole || e === Ending.BreakTheLoop; },
  Title(e) {
    return ({
      [Ending.Abolition]:       "🜍 Abolition-by-Atrocity — \"The Mercy That Ate the World\"",
      [Ending.DoomguidesPeace]: "⚖️ The Doomguide's Peace — \"The Atrocity Kept\"",
      [Ending.JergalsKeyhole]:  "🗝️ Jergal's Keyhole — \"The Tedious Mercy\"  (golden)",
      [Ending.ReturnedThrone]:  "👑 The Returned Throne — \"The New Tyrant, Maybe Merciful\"",
      [Ending.BreakTheLoop]:    "🕯️ Break the Loop — \"Stay Gone\"  (the existential ending)",
      [Ending.MortalMeasure]:   "🌾 Mortal Measure — \"The Small Good\"",
    })[e] || "—";
  },
  Choice(e) {
    return ({
      [Ending.Abolition]:       "Say YES to the Unmade. Pull the first thread. Save every discarded soul — at any cost.",
      [Ending.DoomguidesPeace]: "Side with Vayle. Keep the Wall. Defend the oldest cruelty because the alternatives are worse.",
      [Ending.JergalsKeyhole]:  "Take up Jergal's pen. Rewrite the Law by hand, soul by soul, forever.",
      [Ending.ReturnedThrone]:  "Take the Crown of Horns yourself. Become the god of the dead.",
      [Ending.BreakTheLoop]:    "Refuse the Unmade AND refuse to become the Last Returned. Say your true name. Stay gone.",
      [Ending.MortalMeasure]:   "Refuse all of it. Walk away. Spend your borrowed time on the small human good.",
    })[e] || "—";
  },
  Prose(e) {
    return ({
      [Ending.Abolition]:
        "You pull the thread, and across every age at once the Wall unravels — every discarded soul freed, " +
        "Maerin among them, whole, forever. And the bill comes due exactly as promised: death itself unbinds, " +
        "the living pay in oceans, and time knots shut into a single deathless Now. You take your seat at the " +
        "centre of the most beautiful tomb in creation. You got everything you wanted. That is the tragedy.",
      [Ending.DoomguidesPeace]:
        "You keep the Wall. Aldric falls; the Unmade is suppressed, not answered; death's order endures. And you " +
        "live with having looked at the oldest cruelty in the multiverse, understood it completely, and defended " +
        "it — because the alternatives were worse, and you were not wrong, which is the heaviest thing of all.",
      [Ending.JergalsKeyhole]:
        "You neither tear the Wall down nor keep it — you *rewrite* it. Jergal, delighted past ten thousand years " +
        "of boredom, hands you the pen. The Unmade, seen and corrected at last, chooses peace. The Wall comes down " +
        "soul by soul, *judged* instead of dissolved. It will take you the rest of eternity. You take the job. " +
        "Heaven exhales for the first time in ten thousand years.",
      [Ending.ReturnedThrone]:
        "You seize the Crown and become the god of the dead — and exactly the thing you watched everyone become. " +
        "A merciful new judge, or the seed of the next catastrophe a future Returned must walk back and stop. Either " +
        "way, the wheel has a new face. Yours.",
      [Ending.BreakTheLoop]:
        "You lower your weapon. You say your true name. You do the one thing the loop has never seen: you choose the " +
        "ending. You step into the niche that has worn your name since before the world was young — not to dissolve, " +
        "but to *rest* — and the Unmade, hearing its own first name spoken willingly, with love, finds the one thing " +
        "it never had: an outside. A witness. A place to set the grief down. The machine of sorrow stops. There will " +
        "be no next Returned. You did that — for a stranger you'll never meet, in a world you'll never see.",
      [Ending.MortalMeasure]:
        "You walk away from the end of the world and go home. The Wall stands; the Unmade waits; the great wound goes " +
        "unhealed. And you spend your borrowed time on the people within arm's reach — a wedding, a fed child, one " +
        "good season — present and kind and real. The world is not fixed. You mattered anyway.",
    })[e] || "—";
  }
};

module.exports = { GameFlags, Ending, ALL_ENDINGS, EndingResolver };
