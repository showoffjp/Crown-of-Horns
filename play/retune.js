// Design-space search: find Brute stats that put BOTH reference duels in-band.
// Uses the seed-faithful port, so the chosen numbers are EXACTLY what Unity prints.
const { CombatBalance, Sheet, Weapon } = require("./engine.js");
const rate=(a,b,wa,wb)=>CombatBalance.WinRate(a,b,wa,wb,400);

const hero   = ()=>Sheet("Hero",16,16,28),  longsword=Weapon("Longsword","1d8");
const duel   = ()=>Sheet("Duelist",18,14,20), rapier =Weapon("Rapier","1d10");

// Bands: p1 in [55,85] target ~70 ; p2 in [35,70] target ~52. Brute stays a brute:
// low AC, big HP pool, simple weapon.
let best=null;
for (const str of [14,15,16])
 for (const ac of [13,14])
  for (const hp of [22,26,30,34,38,42,46])
   for (const club of ["1d6","1d8","2d4"]) {
     const b1=Sheet("Brute",str,ac,hp), w=Weapon("Club",club);
     const p1=rate(hero(),b1,longsword,w);
     if (p1<58||p1>82) continue;                       // keep margin inside [55,85]
     const p2=rate(duel(),Sheet("Brute",str,ac,hp),rapier,w);
     if (p2<38||p2>67) continue;                        // margin inside [35,70]
     const score=Math.abs(p1-70)+Math.abs(p2-52);
     if (!best||score<best.score) best={str,ac,hp,club,p1,p2,score};
   }
console.log(best ? `BEST: Brute Str${best.str} AC${best.ac} HP${best.hp} Club ${best.club}  ->  Hero ${best.p1}% [OK]  Duelist ${best.p2}% [OK]`
                 : "no candidate found — widen search");
