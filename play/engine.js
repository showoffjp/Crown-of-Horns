// ============================================================================
// Crown of Horns — faithful headless port of the real combat rules.
// Ported 1:1 from Assets/Scripts: Dice.cs, Abilities.cs, CharacterSheet.cs,
// AttackResolver.cs, CombatBalance.cs.  RNG is .NET System.Random reimplemented
// bit-for-bit so seeded results match the Unity build seed-for-seed.
// ============================================================================

// ---- .NET Framework System.Random (Knuth subtractive), exact port ----------
class DotNetRandom {
  constructor(seed) { this.init(seed); }
  init(Seed) {
    const MBIG = 2147483647, MSEED = 161803398;
    this.SeedArray = new Array(56).fill(0);
    let subtraction = (Seed === -2147483648) ? 2147483647 : Math.abs(Seed);
    let mj = MSEED - subtraction;
    this.SeedArray[55] = mj;
    let mk = 1;
    for (let i = 1; i < 55; i++) {
      let ii = (21 * i) % 55;
      this.SeedArray[ii] = mk;
      mk = mj - mk;
      if (mk < 0) mk += MBIG;
      mj = this.SeedArray[ii];
    }
    for (let k = 1; k < 5; k++) {
      for (let i = 1; i < 56; i++) {
        this.SeedArray[i] -= this.SeedArray[1 + (i + 30) % 55];
        if (this.SeedArray[i] < 0) this.SeedArray[i] += MBIG;
      }
    }
    this.inext = 0; this.inextp = 21;
  }
  internalSample() {
    const MBIG = 2147483647;
    let locINext = this.inext, locINextp = this.inextp;
    if (++locINext >= 56) locINext = 1;
    if (++locINextp >= 56) locINextp = 1;
    let retVal = this.SeedArray[locINext] - this.SeedArray[locINextp];
    if (retVal === MBIG) retVal--;
    if (retVal < 0) retVal += MBIG;
    this.SeedArray[locINext] = retVal;
    this.inext = locINext; this.inextp = locINextp;
    return retVal;
  }
  sample() { return this.internalSample() * (1.0 / 2147483647); }
  // Next(minValue, maxValue) for small ranges (all dice qualify)
  next(minValue, maxValue) {
    const range = maxValue - minValue;
    return Math.floor(this.sample() * range) + minValue;
  }
}

// ---- Dice.cs ---------------------------------------------------------------
const Dice = {
  _rng: new DotNetRandom(0),
  Seed(s) { this._rng = new DotNetRandom(s); },
  Roll(sides) { return this._rng.next(1, sides + 1); },
  RollN(count, sides) { let t = 0; for (let i = 0; i < count; i++) t += this.Roll(sides); return t; },
  D20() { return this.Roll(20); },
  D20Adv() { return Math.max(this.Roll(20), this.Roll(20)); },
  D20Dis() { return Math.min(this.Roll(20), this.Roll(20)); },
  RollNotation(notation) {
    const m = /^\s*(\d+)d(\d+)\s*([+-]\s*\d+)?\s*$/.exec(notation);
    if (!m) throw new Error("Invalid dice notation: " + notation);
    const count = +m[1], sides = +m[2];
    let mod = m[3] ? +m[3].replace(/\s/g, "") : 0;
    return this.RollN(count, sides) + mod;
  }
};

// ---- Abilities.cs : modifier = floor((score-10)/2) -------------------------
const mod = (score) => Math.floor((score - 10) / 2);

// ---- CharacterSheet.cs (the derived stats the duel touches) ----------------
class CharacterSheet {
  constructor({ name, str = 10, dex = 10, con = 10, baseAC = 10, hp = 1, level = 1 }) {
    this.displayName = name;
    this.str = str; this.dex = dex; this.con = con;
    this.baseArmorClass = baseAC; this.armorClassFromGear = 0;
    this.level = level;
    this.maxHitPoints = hp; this.currentHitPoints = hp;
    this.id = name;
  }
  get ProficiencyBonus() { return 2 + Math.floor((this.level - 1) / 4); }
  modifier(which) { return mod(this[which]); }
  get ArmorClass() { return this.baseArmorClass + this.armorClassFromGear + mod(this.dex); }
  get InitiativeModifier() { return mod(this.dex); }
  get IsAlive() { return this.currentHitPoints > 0; }
  RollInitiative() { return Dice.D20() + this.InitiativeModifier; }
  TakeDamage(a) { this.currentHitPoints = Math.max(0, this.currentHitPoints - Math.max(0, a)); }
  Heal(a) { this.currentHitPoints = Math.min(this.maxHitPoints, this.currentHitPoints + Math.max(0, a)); }
  // no-condition stubs (match the duel's path)
  get GrantsAdvantageToAttackers() { return false; }
  get HasHelpAdvantage() { return false; }
  get AttacksAtDisadvantage() { return false; }
  get IsDodging() { return false; }
  get EffectAttackModifier() { return 0; }
}

// ---- AttackResolver.cs (weapon/attack-roll + crit + mod-to-damage) ---------
const AttackResolver = {
  Resolve(attacker, target, ability, extraAdvantage = false, targetAcBonus = 0) {
    const r = { hit: false, critical: false, damage: 0, log: "" };
    // classDef null in reference sheets -> Strength
    const abilityMod = attacker.modifier("str");

    const advSource = attacker.GrantsAdvantageToAttackers || attacker.HasHelpAdvantage || extraAdvantage;
    const disadvSource = attacker.AttacksAtDisadvantage || target.IsDodging;
    r.advantage = advSource && !disadvSource;
    r.disadvantage = disadvSource && !advSource;
    r.attackRoll = r.advantage ? Dice.D20Adv() : r.disadvantage ? Dice.D20Dis() : Dice.D20();

    r.critical = r.attackRoll === 20;
    const autoMiss = r.attackRoll === 1;
    r.totalToHit = r.attackRoll + abilityMod + attacker.ProficiencyBonus + attacker.EffectAttackModifier;
    r.targetAC = target.ArmorClass + targetAcBonus;
    r.hit = !autoMiss && (r.critical || r.totalToHit >= r.targetAC);

    if (r.hit) {
      let dmg = Dice.RollNotation(ability.damageDice);
      if (r.critical) dmg += Dice.RollNotation(ability.damageDice);
      if (ability.addAbilityModToDamage) dmg += abilityMod;
      r.damage = Math.max(0, dmg);
    }
    return r;
  },
  Apply(target, result) {
    if (result.damage > 0) target.TakeDamage(result.damage);
  }
};

// ---- CombatBalance.cs ------------------------------------------------------
const CombatBalance = {
  DuelAWins(a, b, atkA, atkB, maxTurns = 200) {
    a.currentHitPoints = a.maxHitPoints;
    b.currentHitPoints = b.maxHitPoints;
    for (let t = 0; t < maxTurns; t++) {
      AttackResolver.Apply(b, AttackResolver.Resolve(a, b, atkA));
      if (!b.IsAlive) return true;
      AttackResolver.Apply(a, AttackResolver.Resolve(b, a, atkB));
      if (!a.IsAlive) return false;
    }
    return a.currentHitPoints >= b.currentHitPoints;
  },
  WinRate(a, b, atkA, atkB, trials) {
    let wins = 0;
    for (let i = 0; i < trials; i++) { Dice.Seed(i); if (this.DuelAWins(a, b, atkA, atkB)) wins++; }
    return Math.round(100 * wins / Math.max(1, trials));
  }
};

const Sheet = (name, str, baseAC, hp) =>
  new CharacterSheet({ name, str, dex: 10, con: 12, baseAC, hp, level: 1 });
const Weapon = (name, dice) => ({ abilityName: name, damageDice: dice, addAbilityModToDamage: true, isAttackRoll: true });

module.exports = { DotNetRandom, Dice, mod, CharacterSheet, AttackResolver, CombatBalance, Sheet, Weapon };
