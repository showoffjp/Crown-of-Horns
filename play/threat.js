// ============================================================================
// Threat — headless twin of Assets/Scripts/Combat/ThreatForecast.cs.
// "If these attackers all reach you, how much damage comes in, and what's the
// chance you drop?" Convolves each attacker's exact damage distribution.
// threat.test.js cross-checks it against the real resolver via Monte Carlo.
// ============================================================================
const NOTATION = /^\s*(\d+)d(\d+)\s*([+-]\s*\d+)?\s*$/;
function parse(n) {
  const m = NOTATION.exec(n || "");
  if (!m) return { count: 0, sides: 0, flat: 0 };
  return { count: +m[1], sides: +m[2], flat: m[3] ? +m[3].replace(/\s/g, "") : 0 };
}
function sumDist(count, sides) {
  let cur = [1];
  if (sides > 0) for (let d = 0; d < count; d++) {
    const next = new Array(cur.length + sides).fill(0);
    for (let s = 0; s < cur.length; s++) if (cur[s] > 0) for (let f = 1; f <= sides; f++) next[s + f] += cur[s] / sides;
    cur = next;
  }
  return cur; // index = sum
}
function convolve(a, b) {
  const r = new Array(a.length + b.length - 1).fill(0);
  for (let i = 0; i < a.length; i++) if (a[i] > 0) for (let j = 0; j < b.length; j++) if (b[j] > 0) r[i + j] += a[i] * b[j];
  return r;
}
function singleHit(toHit, ac) { let h = 0; for (let f = 1; f <= 20; f++) if (f === 20 || (f !== 1 && f + toHit >= ac)) h++; return h / 20; }
function failSave(dc, sm) { let f0 = 0; for (let f = 1; f <= 20; f++) if (f + sm < dc) f0++; return f0 / 20; }

// damage distribution (index = damage) for one attacker/ability vs target
function damageDistribution(att, def, ab) {
  const useCast = ab.spellSlotLevel > 0 || ab.isHeal;
  const modA = useCast ? att.SpellcastingAbility : (att.classDef ? att.classDef.primaryAbility : "strength");
  const abilityMod = att.Modifier(modA);
  const modDmg = (ab.addMod || ab.addAbilityModToDamage) ? abilityMod : 0;
  const { count, sides, flat } = parse(ab.damageDice);
  const acc = new Map();
  const add = (dmg, p) => { if (p <= 0) return; if (dmg < 0) dmg = 0; acc.set(dmg, (acc.get(dmg) || 0) + p); };

  if (ab.isAttackRoll) {
    const advS = def.GrantsAdvantageToAttackers || att.HasHelpAdvantage;
    const disS = att.AttacksAtDisadvantage || def.IsDodging;
    const adv = advS && !disS, dis = disS && !advS;
    const toHit = abilityMod + att.ProficiencyBonus + att.EffectAttackModifier, ac = def.ArmorClass;
    const pS = singleHit(toHit, ac), pCS = 0.05;
    const pHit = adv ? 1 - (1 - pS) ** 2 : dis ? pS ** 2 : pS;
    const pCrit = adv ? 1 - 0.95 ** 2 : dis ? pCS ** 2 : pCS;
    const pHNC = Math.max(0, pHit - pCrit);
    add(0, 1 - pHit);
    const dn = sumDist(count, sides); for (let s = 0; s < dn.length; s++) if (dn[s] > 0) add(s + flat + modDmg, dn[s] * pHNC);
    const dc = sumDist(count * 2, sides); for (let s = 0; s < dc.length; s++) if (dc[s] > 0) add(s + 2 * flat + modDmg, dc[s] * pCrit);
  } else {
    const dc = 8 + att.ProficiencyBonus + abilityMod, sm = def.Modifier(ab.saveAbility), pFail = failSave(dc, sm);
    const dn = sumDist(count, sides);
    for (let s = 0; s < dn.length; s++) if (dn[s] > 0) {
      const full = s + flat + modDmg;
      add(full, dn[s] * pFail);
      if (ab.saveForHalf) add(Math.floor(full / 2), dn[s] * (1 - pFail)); else add(0, dn[s] * (1 - pFail));
    }
  }
  let max = 0; for (const k of acc.keys()) if (k > max) max = k;
  const arr = new Array(max + 1).fill(0); let exp = 0;
  for (const [k, v] of acc) { arr[k] += v; exp += k * v; }
  return { dist: arr, expected: exp };
}

// threats: array of { attacker, ability }
function threat(target, threats) {
  let combined = [1], expected = 0, n = 0;
  for (const { attacker, ability } of (threats || [])) {
    if (!attacker || !ability || ability.isHeal) continue;
    const { dist, expected: e } = damageDistribution(attacker, target, ability);
    expected += e; combined = convolve(combined, dist); n++;
  }
  const hp = Math.max(1, target.currentHitPoints);
  let down = 0; for (let d = hp; d < combined.length; d++) down += combined[d];
  return { expectedIncoming: expected, downChance: Math.min(1, Math.max(0, down)), attackerCount: n };
}

function threatSummary(t) {
  if (t.attackerCount === 0) return "safe";
  let s = `incoming ~${Math.round(t.expectedIncoming * 10) / 10}`;
  if (t.downChance > 0.005) s += ` · ${Math.round(t.downChance * 100)}% down`;
  return s;
}

module.exports = { threat, damageDistribution, threatSummary };
