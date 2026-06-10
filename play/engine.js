// ============================================================================
// Crown of Horns — faithful headless port of the combat engine.
// 1:1 with Assets/Scripts: Dice.cs, Abilities.cs, CharacterSheet.cs,
// StatusEffect.cs, AttackResolver.cs, CombatBalance.cs.
// RNG = .NET System.Random, reimplemented bit-for-bit (seed-faithful to Unity).
// ============================================================================

class FormatError extends Error {}

// ---- .NET Framework System.Random (Knuth subtractive), exact port ----------
class DotNetRandom {
  constructor(seed) { this.init(seed); }
  init(Seed) {
    const MBIG = 2147483647, MSEED = 161803398;
    this.S = new Array(56).fill(0);
    const sub = (Seed === -2147483648) ? 2147483647 : Math.abs(Seed);
    let mj = MSEED - sub; this.S[55] = mj; let mk = 1;
    for (let i = 1; i < 55; i++) {
      const ii = (21 * i) % 55; this.S[ii] = mk;
      mk = mj - mk; if (mk < 0) mk += MBIG; mj = this.S[ii];
    }
    for (let k = 1; k < 5; k++)
      for (let i = 1; i < 56; i++) {
        this.S[i] -= this.S[1 + (i + 30) % 55];
        if (this.S[i] < 0) this.S[i] += MBIG;
      }
    this.inext = 0; this.inextp = 21;
  }
  internalSample() {
    const MBIG = 2147483647;
    let a = this.inext, b = this.inextp;
    if (++a >= 56) a = 1; if (++b >= 56) b = 1;
    let r = this.S[a] - this.S[b];
    if (r === MBIG) r--; if (r < 0) r += MBIG;
    this.S[a] = r; this.inext = a; this.inextp = b; return r;
  }
  sample() { return this.internalSample() * (1 / 2147483647); }
  next(min, max) { return Math.floor(this.sample() * (max - min)) + min; }
}

// ---- Dice.cs ---------------------------------------------------------------
const NOTATION = /^\s*(\d+)d(\d+)\s*([+-]\s*\d+)?\s*$/;
const Dice = {
  _r: new DotNetRandom(0),
  Seed(s) { this._r = new DotNetRandom(s); },
  // Roll(sides) | Roll(count, sides) | Roll("2d6+3")  — mirrors the C# overloads
  Roll(a, b) {
    if (typeof a === "string") return this.RollNotation(a);
    if (b === undefined) return this._r.next(1, a + 1);
    let t = 0; for (let i = 0; i < a; i++) t += this._r.next(1, b + 1); return t;
  },
  D20() { return this._r.next(1, 21); },
  D20Advantage() { return Math.max(this._r.next(1, 21), this._r.next(1, 21)); },
  D20Disadvantage() { return Math.min(this._r.next(1, 21), this._r.next(1, 21)); },
  RollNotation(n) {
    const m = NOTATION.exec(n);
    if (!m) throw new FormatError(`Invalid dice notation: '${n}'.`);
    const count = +m[1], sides = +m[2];
    const mod = m[3] ? +m[3].replace(/\s/g, "") : 0;
    return this.Roll(count, sides) + mod;
  }
};

// ---- Abilities.cs ----------------------------------------------------------
const ABILITIES = ["strength", "dexterity", "constitution", "intelligence", "wisdom", "charisma"];
class AbilityScores {
  constructor() { for (const a of ABILITIES) this[a] = 10; }
  Get(a) { return this[a]; }
  Set(a, v) { this[a] = v; }
  Modifier(a) { return Math.floor((this[a] - 10) / 2); }   // floor, incl. negatives
}

// ---- data factories (AbilityDefinition / StatusEffectDefinition) -----------
function Ability(o = {}) {
  return Object.assign({
    abilityName: "Strike", spellSlotLevel: 0, targeting: "SingleEnemy",
    rangeTiles: 1, areaRadiusTiles: 0, isAttackRoll: true, saveAbility: "dexterity",
    saveForHalf: false, damageDice: "1d8", damageType: "Slashing",
    addAbilityModToDamage: true, isHeal: false, healDice: "",
    appliedEffect: null, appliedEffectRounds: 0
  }, o);
}
function StatusEffect(o = {}) {
  return Object.assign({
    effectName: "Effect", condition: "None", durationRounds: 2,
    damageOverTimeDice: "", incapacitates: false, attackersHaveAdvantage: false,
    bearerAttacksDisadvantage: false, attackRollModifier: 0,
    armorClassModifier: 0, speedModifier: 0
  }, o);
}

// ---- CharacterSheet.cs (the runtime sheet) ---------------------------------
class CharacterSheet {
  constructor(o = {}) {
    this.displayName = o.displayName || "Unnamed";
    this.id = o.id || this.displayName;
    this.classDef = o.classDef || null;        // {primaryAbility, spellcastingAbility}
    this.raceDef = o.raceDef || null;           // {baseSpeedTiles}
    this.abilities = new AbilityScores();
    this.level = o.level || 1;
    this.baseArmorClass = o.baseArmorClass != null ? o.baseArmorClass : 10;
    this.armorClassFromGear = o.armorClassFromGear || 0;
    this.maxHitPoints = o.maxHitPoints || 1;
    this.currentHitPoints = o.currentHitPoints != null ? o.currentHitPoints : this.maxHitPoints;
    this.activeEffects = [];
    this.IsDodging = false;
    this.HasHelpAdvantage = false;
  }
  Modifier(a) { return this.abilities.Modifier(a); }
  get ProficiencyBonus() { return 2 + Math.floor((this.level - 1) / 4); }
  get SpellcastingAbility() { return this.classDef ? this.classDef.spellcastingAbility : "intelligence"; }
  get InitiativeModifier() { return this.Modifier("dexterity"); }
  get IsAlive() { return this.currentHitPoints > 0; }
  get SpeedTiles() {
    let s = this.raceDef ? this.raceDef.baseSpeedTiles : 6;
    for (const e of this.activeEffects) s += e.def ? e.def.speedModifier : 0;
    return Math.max(0, s);
  }
  get ArmorClass() {
    let ac = this.baseArmorClass + this.armorClassFromGear + this.Modifier("dexterity");
    for (const e of this.activeEffects) ac += e.def ? e.def.armorClassModifier : 0;
    return ac;
  }
  HasCondition(c) { return this.activeEffects.some(e => e.def && e.def.condition === c); }
  get IsIncapacitated() { return this.activeEffects.some(e => e.def && e.def.incapacitates); }
  get GrantsAdvantageToAttackers() { return this.activeEffects.some(e => e.def && e.def.attackersHaveAdvantage); }
  get AttacksAtDisadvantage() {
    return this.activeEffects.some(e => e.def && (e.def.bearerAttacksDisadvantage || e.def.incapacitates));
  }
  get EffectAttackModifier() { let m = 0; for (const e of this.activeEffects) if (e.def) m += e.def.attackRollModifier; return m; }
  AddEffect(def, rounds, sourceId) {
    if (!def) return;
    for (const e of this.activeEffects) if (e.def === def) { e.remainingRounds = Math.max(e.remainingRounds, rounds); return; }
    this.activeEffects.push({ def, remainingRounds: rounds, sourceId });
  }
  RemoveCondition(c) { this.activeEffects = this.activeEffects.filter(e => !(e.def && e.def.condition === c)); }
  TickStartOfTurn() {
    let total = 0;
    for (const e of this.activeEffects)
      if (e.def && e.def.damageOverTimeDice) { const d = Dice.RollNotation(e.def.damageOverTimeDice); this.TakeDamage(d); total += d; }
    return total;
  }
  TickEndOfTurn() {
    for (let i = this.activeEffects.length - 1; i >= 0; i--) {
      this.activeEffects[i].remainingRounds--;
      if (this.activeEffects[i].remainingRounds <= 0) this.activeEffects.splice(i, 1);
    }
  }
  TakeDamage(a) { this.currentHitPoints = Math.max(0, this.currentHitPoints - Math.max(0, a)); }
  Heal(a) { this.currentHitPoints = Math.min(this.maxHitPoints, this.currentHitPoints + Math.max(0, a)); }
}

// ---- AttackResolver.cs (full: attack, save, heal, crit, conditions) --------
const AttackResolver = {
  Resolve(attacker, target, ability, extraAdvantage = false, targetAcBonus = 0) {
    const r = { hit: false, critical: false, isHeal: false, advantage: false, disadvantage: false,
                attackRoll: 0, totalToHit: 0, targetAC: 0, damage: 0, healing: 0,
                damageType: ability.damageType, effectApplied: null, effectRounds: 0, sourceId: attacker.id, log: "" };

    const useCast = ability.spellSlotLevel > 0 || ability.isHeal;
    const modAbility = useCast ? attacker.SpellcastingAbility
                              : (attacker.classDef ? attacker.classDef.primaryAbility : "strength");
    const abilityMod = attacker.Modifier(modAbility);

    if (ability.isHeal) {
      r.isHeal = true; r.hit = true;
      const dice = ability.healDice && ability.healDice.trim() ? ability.healDice : ability.damageDice;
      r.healing = Math.max(0, Dice.RollNotation(dice) + abilityMod);
      return r;
    }

    let effectShouldLand;
    if (ability.isAttackRoll) {
      const advSource = target.GrantsAdvantageToAttackers || attacker.HasHelpAdvantage || extraAdvantage;
      const disadvSource = attacker.AttacksAtDisadvantage || target.IsDodging;
      r.advantage = advSource && !disadvSource;
      r.disadvantage = disadvSource && !advSource;
      r.attackRoll = r.advantage ? Dice.D20Advantage() : r.disadvantage ? Dice.D20Disadvantage() : Dice.D20();
      r.critical = r.attackRoll === 20;
      const autoMiss = r.attackRoll === 1;
      r.totalToHit = r.attackRoll + abilityMod + attacker.ProficiencyBonus + attacker.EffectAttackModifier;
      r.targetAC = target.ArmorClass + targetAcBonus;
      r.hit = !autoMiss && (r.critical || r.totalToHit >= r.targetAC);
      effectShouldLand = r.hit;
    } else {
      const saveDC = 8 + attacker.ProficiencyBonus + abilityMod;
      const save = Dice.D20() + target.Modifier(ability.saveAbility);
      r.targetAC = saveDC; r.totalToHit = save;
      const failedSave = save < saveDC;
      r.hit = failedSave; effectShouldLand = failedSave;
    }

    if (ability.damageDice && ability.damageDice.trim()) {
      const dealDamage = r.hit || (!ability.isAttackRoll && ability.saveForHalf);
      if (dealDamage) {
        let dmg = Dice.RollNotation(ability.damageDice);
        if (r.critical) dmg += Dice.RollNotation(ability.damageDice);
        if (ability.addAbilityModToDamage) dmg += abilityMod;
        if (!r.hit && ability.saveForHalf) dmg = Math.trunc(dmg / 2);   // C# int division
        r.damage = Math.max(0, dmg);
      }
    }

    if (ability.appliedEffect && effectShouldLand) {
      r.effectApplied = ability.appliedEffect;
      r.effectRounds = ability.appliedEffectRounds > 0 ? ability.appliedEffectRounds : ability.appliedEffect.durationRounds;
    }
    return r;
  },
  Apply(target, result) {
    if (result.isHeal) { target.Heal(result.healing); return; }
    if (result.damage > 0) target.TakeDamage(result.damage);
    if (result.effectApplied) target.AddEffect(result.effectApplied, result.effectRounds, result.sourceId);
  }
};

// ---- CombatBalance.cs ------------------------------------------------------
const CombatBalance = {
  DuelAWins(a, b, atkA, atkB, maxTurns = 200) {
    a.currentHitPoints = a.maxHitPoints; b.currentHitPoints = b.maxHitPoints;
    for (let t = 0; t < maxTurns; t++) {
      AttackResolver.Apply(b, AttackResolver.Resolve(a, b, atkA)); if (!b.IsAlive) return true;
      AttackResolver.Apply(a, AttackResolver.Resolve(b, a, atkB)); if (!a.IsAlive) return false;
    }
    return a.currentHitPoints >= b.currentHitPoints;
  },
  WinRate(a, b, atkA, atkB, trials) {
    let wins = 0;
    for (let i = 0; i < trials; i++) { Dice.Seed(i); if (this.DuelAWins(a, b, atkA, atkB)) wins++; }
    return Math.round(100 * wins / Math.max(1, trials));
  }
};

// reference-sheet helpers (mirror CombatBalance.Sheet / Weapon)
function Sheet(name, str, baseAC, hp) {
  const s = new CharacterSheet({ displayName: name, classDef: null, level: 1, baseArmorClass: baseAC, maxHitPoints: hp });
  s.abilities.Set("strength", str); s.abilities.Set("dexterity", 10); s.abilities.Set("constitution", 12);
  return s;
}
function Weapon(name, dice) {
  return Ability({ abilityName: name, damageDice: dice, damageType: "Slashing", rangeTiles: 1, isAttackRoll: true, addAbilityModToDamage: true });
}

module.exports = {
  FormatError, DotNetRandom, Dice, AbilityScores, Ability, StatusEffect,
  CharacterSheet, AttackResolver, CombatBalance, Sheet, Weapon
};
