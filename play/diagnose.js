const { Dice, AttackResolver, CharacterSheet, Sheet, Weapon } = require("./engine.js");

// 1) Is my .NET RNG uniform on a d20?
Dice.Seed(12345);
const N = 1_000_000, hist = new Array(21).fill(0);
for (let i = 0; i < N; i++) hist[Dice.D20()]++;
const lo = Math.min(...hist.slice(1)), hi = Math.max(...hist.slice(1));
console.log(`d20 uniformity over ${N.toLocaleString()} rolls: each face expected ${(N/20).toLocaleString()}`);
console.log(`  min face count ${lo}, max ${hi}  (spread ${(100*(hi-lo)/(N/20)).toFixed(2)}% — want <~1%)`);

// 2) Hand-math says Hero(+5 to hit) vs Brute(AC13) hits on d20>=8 => 65%; Brute(+4) vs Hero(AC16) => d20>=12 => 45%.
function empirical(att, def, atk, label, expectHitPct) {
  Dice.Seed(999);
  let hits = 0, dmgSum = 0, T = 500000;
  for (let i = 0; i < T; i++) {
    const r = AttackResolver.Resolve(att, def, atk);
    if (r.hit) { hits++; dmgSum += r.damage; }
  }
  console.log(`  ${label}: hit ${(100*hits/T).toFixed(1)}% (expect ~${expectHitPct}%), avg dmg/hit ${(dmgSum/hits).toFixed(2)}`);
}
console.log("\nPer-swing fidelity vs hand-computed 5e math:");
empirical(Sheet("Hero",16,16,28), Sheet("Brute",14,13,22), Weapon("Longsword","1d8"), "Hero->Brute", 65);
empirical(Sheet("Brute",14,13,22), Sheet("Hero",16,16,28), Weapon("Club","1d6"), "Brute->Hero", 45);

// 3) So who SHOULD win? expected dmg/turn * to-kill turns
const heroDPT = 0.65*(4.5+3), bruteDPT = 0.45*(3.5+2);
console.log(`\nExpected dmg/turn: Hero ${heroDPT.toFixed(2)} (kills 22hp in ${(22/heroDPT).toFixed(1)} turns) · Brute ${bruteDPT.toFixed(2)} (kills 28hp in ${(28/bruteDPT).toFixed(1)} turns)`);
console.log("If Hero kills in ~half the turns, a ~90%+ win rate is CORRECT math — the band is the thing that's off.");
