// ============================================================================
// Forecast — headless twin of Assets/Scripts/Combat/AttackForecast.cs.
// Analytic 5e outcome preview (hit %, crit %, expected damage, lethal %) over the
// same modifiers AttackResolver rolls. forecast.test.js cross-checks it against the
// real resolver via Monte Carlo, so this can't silently drift from the dice.
// ============================================================================
const NOTATION = /^\s*(\d+)d(\d+)\s*([+-]\s*\d+)?\s*$/;
function parse(notation) {
  const m = NOTATION.exec(notation || "");
  if (!m) return { count: 0, sides: 0, flat: 0 };
  return { count: +m[1], sides: +m[2], flat: m[3] ? +m[3].replace(/\s/g, "") : 0 };
}
function average(notation) {
  const { count, sides, flat } = parse(notation);
  return (count === 0 || sides === 0) ? flat : count * (sides + 1) / 2 + flat;
}
// distribution of the sum of `count` d`sides`, as an array indexed by sum.
function sumDist(count, sides) {
  let cur = [1]; // sum 0, prob 1
  if (sides <= 0) return cur;
  for (let d = 0; d < count; d++) {
    const next = new Array(cur.length + sides).fill(0);
    for (let s = 0; s < cur.length; s++)
      if (cur[s] > 0) for (let face = 1; face <= sides; face++) next[s + face] += cur[s] / sides;
    cur = next;
  }
  return cur;
}
function pAtLeast(dist, shift, threshold) {
  let p = 0;
  for (let s = 0; s < dist.length; s++) if (dist[s] > 0 && s + shift >= threshold) p += dist[s];
  return p;
}
function singleRollHitChance(toHit, ac) {
  let hits = 0;
  for (let f = 1; f <= 20; f++) if (f === 20 || (f !== 1 && f + toHit >= ac)) hits++;
  return hits / 20;
}
function failSaveChance(dc, saveMod) {
  let fails = 0;
  for (let f = 1; f <= 20; f++) if (f + saveMod < dc) fails++;
  return fails / 20;
}

function forecast(attacker, target, ability, extraAdvantage = false, targetAcBonus = 0) {
  const fc = { isHeal: !!ability.isHeal, isSave: !ability.isAttackRoll && !ability.isHeal,
               advantage: false, disadvantage: false, hitChance: 0, critChance: 0,
               expectedDamage: 0, expectedHealing: 0, lethalChance: 0 };

  const useCast = ability.spellSlotLevel > 0 || ability.isHeal;
  const modAbility = useCast ? attacker.SpellcastingAbility
                            : (attacker.classDef ? attacker.classDef.primaryAbility : "strength");
  const abilityMod = attacker.Modifier(modAbility);

  if (ability.isHeal) {
    const dice = ability.healDice && ability.healDice.trim() ? ability.healDice : ability.damageDice;
    fc.expectedHealing = Math.max(0, average(dice) + abilityMod);
    fc.hitChance = 1;
    return fc;
  }

  const { count, sides, flat } = parse(ability.damageDice);
  const diceAvg = (count === 0 || sides === 0) ? 0 : count * (sides + 1) / 2;
  const modDmg = ability.addMod || ability.addAbilityModToDamage ? abilityMod : 0;
  const hp = Math.max(1, target.currentHitPoints);

  if (ability.isAttackRoll) {
    const advSource = target.GrantsAdvantageToAttackers || attacker.HasHelpAdvantage || extraAdvantage;
    const disadvSource = attacker.AttacksAtDisadvantage || target.IsDodging;
    fc.advantage = advSource && !disadvSource;
    fc.disadvantage = disadvSource && !advSource;

    const toHit = abilityMod + attacker.ProficiencyBonus + attacker.EffectAttackModifier;
    const ac = target.ArmorClass + targetAcBonus;
    const pSingle = singleRollHitChance(toHit, ac);
    const pCritSingle = 0.05;

    if (fc.advantage)        { fc.hitChance = 1 - (1 - pSingle) ** 2; fc.critChance = 1 - (1 - pCritSingle) ** 2; }
    else if (fc.disadvantage){ fc.hitChance = pSingle ** 2;          fc.critChance = pCritSingle ** 2; }
    else                     { fc.hitChance = pSingle;               fc.critChance = pCritSingle; }

    const pHitNotCrit = Math.max(0, fc.hitChance - fc.critChance);
    const normalAvg = diceAvg + flat + modDmg;
    const critAvg = 2 * (diceAvg + flat) + modDmg;
    fc.expectedDamage = Math.max(0, pHitNotCrit * normalAvg + fc.critChance * critAvg);

    const pNormalKill = pAtLeast(sumDist(count, sides), flat + modDmg, hp);
    const pCritKill = pAtLeast(sumDist(count * 2, sides), 2 * flat + modDmg, hp);
    fc.lethalChance = pHitNotCrit * pNormalKill + fc.critChance * pCritKill;
  } else {
    const saveDC = 8 + attacker.ProficiencyBonus + abilityMod;
    const saveMod = target.Modifier(ability.saveAbility);
    const pFail = failSaveChance(saveDC, saveMod);
    fc.hitChance = pFail;

    const full = diceAvg + flat + modDmg;
    fc.expectedDamage = Math.max(0, ability.saveForHalf
      ? pFail * full + (1 - pFail) * Math.floor(full / 2)
      : pFail * full);

    const dist = sumDist(count, sides);
    const shift = flat + modDmg;
    let lethal = pFail * pAtLeast(dist, shift, hp);
    if (ability.saveForHalf) lethal += (1 - pFail) * pAtLeast(dist, shift, 2 * hp);
    fc.lethalChance = lethal;
  }
  return fc;
}

function summary(fc) {
  if (fc.isHeal) return `heal ~${(Math.round(fc.expectedHealing * 10) / 10)}`;
  const pct = p => `${Math.round(p * 100)}%`;
  let s = `${pct(fc.hitChance)} ${fc.isSave ? "fail" : "hit"} · ~${Math.round(fc.expectedDamage * 10) / 10} dmg`;
  if (fc.critChance > 0.0001) s += ` · ${pct(fc.critChance)} crit`;
  if (fc.lethalChance > 0.005) s += ` · ${pct(fc.lethalChance)} kill`;
  return s;
}

module.exports = { forecast, average, summary, parse, sumDist, pAtLeast };
