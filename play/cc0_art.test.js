// Integrity smoke for the fetched CC0 art (Assets/Resources/Art/DCSS): every file is a
// real PNG, the curated groups are populated, and the provenance LICENSE.md is present
// and documents every file. Guards against a partial fetch or an undocumented binary
// slipping into the repo.
const fs = require("fs");
const path = require("path");
let pass = 0, fail = 0; const fails = [];
function check(name, cond) { if (cond) pass++; else { fail++; fails.push(name); } }

const ART = path.join(__dirname, "..", "Assets", "Resources", "Art", "DCSS");
check("art dir exists", fs.existsSync(ART));

const groups = { floor: 20, wall: 18, feat: 5, mon: 35, item: 10 };
let all = [];
for (const [g, min] of Object.entries(groups)) {
  const dir = path.join(ART, g);
  const files = fs.existsSync(dir) ? fs.readdirSync(dir).filter(f => f.endsWith(".png")) : [];
  check(`${g}: >=${min} tiles (${files.length})`, files.length >= min);
  all = all.concat(files.map(f => ({ g, f, p: path.join(dir, f) })));
}
check("every file is a real PNG (magic bytes)", all.every(({ p }) => {
  const b = fs.readFileSync(p);
  return b.length > 8 && b[0] === 0x89 && b[1] === 0x50 && b[2] === 0x4e && b[3] === 0x47;
}));

const licPath = path.join(ART, "LICENSE.md");
check("LICENSE.md present", fs.existsSync(licPath));
const lic = fs.existsSync(licPath) ? fs.readFileSync(licPath, "utf8") : "";
check("LICENSE.md names CC0 + source + exclusion check",
  lic.includes("CC0") && lic.includes("crawl") && lic.includes("TILES_UNDER_UNKNOWN_LICENSE"));
check("LICENSE.md documents every fetched file",
  all.every(({ g, f }) => lic.includes(`${g}/${f}`)));

// the rendered screenshot exists and is a PNG
const shot = path.join(__dirname, "gameplay_v2.png");
check("gameplay_v2.png rendered", fs.existsSync(shot) && fs.readFileSync(shot)[0] === 0x89);

console.log(`\n  CC0 art — integrity smoke:`);
console.log(`  ${pass} passed, ${fail} failed`);
if (fail) { fails.forEach(f => console.log("   ✗ " + f)); process.exit(1); }
else console.log(`  ✓ ${all.length} CC0 tiles verified, provenance documented, screenshot rendered.\n`);
