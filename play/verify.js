const { Dice, CombatBalance, Sheet, Weapon } = require("./engine.js");

console.log("=== RNG transparency: first 10 d20 rolls from System.Random(0) ===");
Dice.Seed(0);
let rolls = []; for (let i = 0; i < 10; i++) rolls.push(Dice.D20());
console.log("  " + rolls.join(", "));

function report(trials) {
  const p1 = CombatBalance.WinRate(Sheet("Hero", 16, 16, 28), Sheet("Brute", 14, 13, 22),
                                   Weapon("Longsword", "1d8"), Weapon("Club", "1d6"), trials);
  const p2 = CombatBalance.WinRate(Sheet("Duelist", 18, 14, 20), Sheet("Brute", 14, 13, 22),
                                   Weapon("Rapier", "1d10"), Weapon("Club", "1d6"), trials);
  const v1 = (p1 >= 55 && p1 <= 85) ? "OK" : p1 > 85 ? "HIGH" : "LOW";
  const v2 = (p2 >= 35 && p2 <= 70) ? "OK" : p2 > 70 ? "HIGH" : "LOW";
  return { p1, v1, p2, v2 };
}

console.log("\n=== Reproducing CombatBalance.Report() — the engine's own balance oracle ===");
for (const n of [400, 5000]) {
  const r = report(n);
  console.log(`  ⚖ (${String(n).padStart(4)} duels ea.)  Hero vs Brute ${r.p1}% [${r.v1}]  ·  Duelist vs Brute ${r.p2}% [${r.v2}]`);
}
console.log("\n  Design bands:  Hero vs Brute expects 55–85% [OK]   Duelist vs Brute expects 35–70% [OK]");
console.log("  >> 400-duel row is the EXACT line your Unity build's CombatBalance.Report() prints.");
