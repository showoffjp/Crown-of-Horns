// One command to prove the whole slice. Runs the gates (must pass) and the
// informational reports. Exits non-zero if any gate fails — CI-ready.
const { execSync } = require("child_process");
const gates = ["tests.js", "abilities.js", "forecast.test.js", "threat.test.js", "endings.test.js", "epilogue.test.js", "items.test.js", "progression.test.js", "save.test.js", "quests.test.js", "smoke.js"];
const info  = ["verify.js", "diagnose.js", "autobattle.js"];

let failed = 0;
function run(f, gate) {
  process.stdout.write(`\n[1m▶ ${f}[0m${gate ? "  (gate)" : ""}\n`);
  try { console.log(execSync(`node ${f}`, { cwd: __dirname }).toString().trim()); }
  catch (e) { console.log((e.stdout || "").toString().trim()); if (gate) failed++; }
}
console.log("Crown of Horns — combat slice verification");
gates.forEach(f => run(f, true));
info.forEach(f => run(f, false));
console.log(`\n${"=".repeat(60)}`);
console.log(failed === 0
  ? "✓ ALL GATES PASSED — the JS engine port matches your Unity combat rules."
  : `✗ ${failed} gate(s) FAILED.`);
process.exit(failed ? 1 : 0);
