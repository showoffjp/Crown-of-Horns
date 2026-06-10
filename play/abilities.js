// Verifies the ability kit the playable demo uses — each button's RULES, headless.
const { Dice, Ability, StatusEffect, CharacterSheet, AttackResolver } = require("./engine.js");

const hero = (o)=> Object.assign(new CharacterSheet(o), {});
function mk(name, str, dex, ac, hp, lvl, cast){ const s=new CharacterSheet({displayName:name,baseArmorClass:ac,maxHitPoints:hp,level:lvl,
  classDef:{primaryAbility: dex>str?"dexterity":"strength", spellcastingAbility: cast||"wisdom"}});
  s.abilities.Set("strength",str); s.abilities.Set("dexterity",dex); s.abilities.Set("wisdom",14); return s; }

// effects
const Slowed = StatusEffect({effectName:"Slowed", condition:"Slowed", speedModifier:-3, durationRounds:2});
const Stunned = StatusEffect({effectName:"Stunned", condition:"Stunned", incapacitates:true, attackersHaveAdvantage:true, durationRounds:1});
const Frightened = StatusEffect({effectName:"Frightened", condition:"Frightened", bearerAttacksDisadvantage:true, durationRounds:2});
const Burning = StatusEffect({effectName:"Burning", condition:"Burning", damageOverTimeDice:"1d6", durationRounds:3});

let pass=0, fail=0; const F=[];
const t=(n,f)=>{try{f();pass++}catch(e){fail++;F.push(n+": "+e.message)}};
const ok=(c,m)=>{if(!c)throw new Error(m||"false")};

// 1) Sun's Mending heals an injured ally (2d4 + WIS mod), never past max
t("Sun's Mending heals", ()=>{
  const garrow=mk("Garrow",15,12,16,30,5,"wisdom");
  const ally=mk("Varra",17,12,17,38,5,"strength"); ally.currentHitPoints=10;
  const heal=Ability({abilityName:"Sun's Mending", isHeal:true, healDice:"2d4", rangeTiles:4});
  Dice.Seed(3); const r=AttackResolver.Resolve(garrow,ally,heal); AttackResolver.Apply(ally,r);
  ok(r.isHeal && r.healing>=2, "heals a positive amount"); ok(ally.currentHitPoints>10, "ally HP rose");
});
// 2) Searing Light: Dex save, save-for-half radiant — failed save takes more than a made save
t("Searing Light save-for-half", ()=>{
  const garrow=mk("Garrow",15,12,16,30,5,"wisdom");
  const sturdy=mk("Wraith",13,18,12,200,2); // high dex -> often saves
  const frail=mk("Husk",13,6,12,200,2);      // low dex -> often fails
  const light=Ability({abilityName:"Searing Light", isAttackRoll:false, saveAbility:"dexterity", saveForHalf:true, damageDice:"2d6", damageType:"Radiant", addAbilityModToDamage:false});
  let frailDmg=0, sturdyDmg=0;
  for(let s=0;s<3000;s++){Dice.Seed(s); frailDmg+=AttackResolver.Resolve(garrow,frail,light).damage;}
  for(let s=0;s<3000;s++){Dice.Seed(s); sturdyDmg+=AttackResolver.Resolve(garrow,sturdy,light).damage;}
  ok(frailDmg>sturdyDmg, "the low-Dex target takes more (fails saves more often)");
});
// 3) Hamstring applies Slowed on hit, cutting the victim's movement
t("Hamstring applies Slowed", ()=>{
  const roen=mk("Roen",12,18,15,24,5); const foe=mk("Returned",13,10,8,40,2); // AC 8 so it lands
  foe.raceDef={baseSpeedTiles:4};
  const hamstring=Ability({abilityName:"Hamstring", isAttackRoll:true, damageDice:"1d6", appliedEffect:Slowed, appliedEffectRounds:2, addAbilityModToDamage:true});
  let applied=false; for(let s=0;s<50 && !applied;s++){Dice.Seed(s); const r=AttackResolver.Resolve(roen,foe,hamstring); if(r.hit){AttackResolver.Apply(foe,r); applied=r.effectApplied!=null;}}
  ok(applied, "Slowed landed on a hit"); ok(foe.HasCondition("Slowed")); ok(foe.SpeedTiles===1, "speed 4 -3 = 1");
});
// 4) Crushing Blow can Stun (incapacitate) — a stunned unit attacks at disadvantage & is skipped
t("Crushing Blow stuns", ()=>{
  const varra=mk("Varra",17,12,17,38,5); const foe=mk("Returned",13,10,6,40,2);
  const crush=Ability({abilityName:"Crushing Blow", isAttackRoll:true, damageDice:"1d10", appliedEffect:Stunned, addAbilityModToDamage:true});
  let stunned=false; for(let s=0;s<50 && !stunned;s++){Dice.Seed(s); const r=AttackResolver.Resolve(varra,foe,crush); if(r.hit){AttackResolver.Apply(foe,r); stunned=foe.IsIncapacitated;}}
  ok(stunned, "stun applied"); ok(foe.AttacksAtDisadvantage, "stunned -> attacks at disadvantage"); ok(foe.GrantsAdvantageToAttackers, "and is easier to hit");
});
// 5) Wail of the Returned: AoE Wis save-for-half psychic, applies Frightened to a cluster
t("Boss Wail hits a cluster", ()=>{
  const boss=mk("Last Returned",16,12,15,40,4,"wisdom");
  const cluster=[mk("Garrow",15,12,16,30,5),mk("Roen",12,18,15,24,5),mk("Varra",17,12,17,38,5)];
  const wail=Ability({abilityName:"Wail of the Returned", isAttackRoll:false, saveAbility:"wisdom", saveForHalf:true, damageDice:"2d6", damageType:"Psychic", addAbilityModToDamage:false, appliedEffect:Frightened});
  Dice.Seed(11); let totalDmg=0, frightened=0;
  for(const h of cluster){ const r=AttackResolver.Resolve(boss,h,wail); AttackResolver.Apply(h,r); totalDmg+=r.damage; if(h.HasCondition("Frightened"))frightened++; }
  ok(totalDmg>0, "the cluster took psychic damage"); ok(frightened>=1, "at least one was Frightened");
});
// 6) Burning DoT bites each turn and expires after its duration
t("Burning ticks then expires", ()=>{
  const foe=mk("Husk",13,10,12,100,2); foe.AddEffect(Burning,3,"src");
  Dice.Seed(7); let ticks=0; for(let round=0;round<3;round++){ const d=foe.TickStartOfTurn(); if(d>0)ticks++; foe.TickEndOfTurn(); }
  ok(ticks===3, "bit on all 3 rounds"); ok(!foe.HasCondition("Burning"), "expired after duration");
});

console.log(`\n  Ability-kit verification (the demo's buttons, headless):`);
console.log(`  ${pass} passed, ${fail} failed`);
if(fail){F.forEach(f=>console.log("   ✗ "+f)); process.exit(1);} else console.log("  ✓ Every ability behaves per the real engine.\n");
