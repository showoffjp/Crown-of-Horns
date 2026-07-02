// ============================================================================
// Reactive party banter — headless model of Content/CampfireBanter.cs +
// PartyBanter.cs + CampGroupBanter.cs. Companions react to each other, to what
// you've done, and to how cold/merciful you've become. A banter is eligible when
// every companion it needs is present, its flags/flagsNot/int gates pass, and
// (if `once`) it hasn't fired. This is the connective tissue that turns a set of
// great scenes into a party you travel with.
// ============================================================================

// companion id -> the flag that means "in your party". garrow is the starting
// companion (assumed present); unknown ids are treated as absent.
const PRESENT_FLAG = {
  garrow: null,
  roen: "companion.roen.recruited",
  varra: "companion.varra.recruited",
  naeve: "companion.naeve.recruited",
  ilfaeril: "companion.ilfaeril.recruited",
  maerin: "companion.maerin.recruited",
  dot: "lf.dot_joined",
  mournlight: "companion.mournlight.recruited",
};

function companionPresent(state, id) {
  if (!(id in PRESENT_FLAG)) return false;
  const f = PRESENT_FLAG[id];
  if (f === null) return id === "garrow";      // always-present starter
  return state.bools[f] === true;
}

// every requirement must hold: all companions present, all flags set, all
// flagsNot unset, all int thresholds met (>=).
function requiresMet(state, req) {
  req = req || {};
  for (const c of (req.companions || [])) if (!companionPresent(state, c)) return false;
  for (const f of (req.flags || [])) if (state.bools[f] !== true) return false;
  for (const f of (req.flagsNot || [])) if (state.bools[f] === true) return false;
  for (const k in (req.int || {})) if ((state.ints[k] || 0) < req.int[k]) return false;
  return true;
}

function banterEligible(state, b) {
  if (b.once !== false && state.bools["banter.seen." + b.id] === true) return false;
  return requiresMet(state, b.requires);
}

function eligibleBanters(catalog, state) { return catalog.filter(b => banterEligible(state, b)); }

// pick a random eligible banter (rnd in [0,1)); null if none. Mark it seen via
// markSeen so a `once` banter never repeats.
function pickBanter(catalog, state, rnd) {
  const pool = eligibleBanters(catalog, state);
  if (!pool.length) return null;
  return pool[Math.floor(rnd() * pool.length)];
}
function markSeen(state, b) { state.bools["banter.seen." + b.id] = true; }

module.exports = { PRESENT_FLAG, companionPresent, requiresMet, banterEligible, eligibleBanters, pickBanter, markSeen };
