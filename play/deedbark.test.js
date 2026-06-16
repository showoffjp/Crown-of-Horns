// Objective checks for deed-reactive victory barks (Pillar 1 × 3 — the party reacts to *how* you
// won). We lift the page's real /*<DEEDBARK>*/ picker and prove the earned deed flags + who's
// standing resolve to one fitting, in-voice line — most-distinctive deed first, gated by presence —
// then assert it's wired into recordDeeds so it fires on victory alongside the Chronicle entry.
const fs = require("fs");
let pass = 0, fail = 0; const fails = [];
function check(name, cond) { if (cond) pass++; else { fail++; fails.push(name); } }

const h = fs.readFileSync(__dirname + "/crown_combat.html", "utf8");
const block = h.match(/\/\*<DEEDBARK>\*\/([\s\S]*?)\/\*<\/DEEDBARK>\*\//);
check("DEEDBARK helper block found in the page", !!block);
const { pickDeedBark, DEED_BARKS, DEED_PRIORITY } = new Function(block[1] + "\nreturn { pickDeedBark, DEED_BARKS, DEED_PRIORITY };")();
const ALL = ["Garrow", "Roen", "Varra", "Naeve"];

check("a merciful win draws Garrow's blessing", (pickDeedBark(["deed.combat.merciful"], ALL) || [])[0] === "Garrow");
check("a ruthless win draws Varra's line", (pickDeedBark(["deed.combat.ruthless"], ALL) || [])[0] === "Varra");
check("a ledge kill draws Roen's line", (pickDeedBark(["deed.combat.ledge_kill"], ALL) || [])[0] === "Roen");
check("a counterspell draws Naeve's line", (pickDeedBark(["deed.combat.counterspell"], ALL) || [])[0] === "Naeve");
check("a rescue draws Garrow's line", (pickDeedBark(["deed.combat.rescue"], ALL) || [])[0] === "Garrow");

// most-distinctive deed wins when several are earned at once
check("a rescue is the most resonant deed and speaks first",
  (pickDeedBark(["deed.combat.flawless", "deed.combat.counterspell", "deed.combat.rescue"], ALL) || [])[1] === DEED_BARKS["deed.combat.rescue"][0].line);
check("a Counterspell trumps a plain flawless win",
  (pickDeedBark(["deed.combat.flawless", "deed.combat.counterspell"], ALL) || [])[0] === "Naeve");
check("a ledge kill trumps a flawless/ruthless combo",
  (pickDeedBark(["deed.combat.flawless", "deed.combat.ruthless", "deed.combat.ledge_kill"], ALL) || [])[0] === "Roen");

// presence-gating: the speaker must be standing
check("no line when the keyed companion has fallen", pickDeedBark(["deed.combat.counterspell"], ["Garrow", "Roen"]) === null);
check("falls through to the next deed when the first speaker is down",
  (pickDeedBark(["deed.combat.counterspell", "deed.combat.merciful"], ["Garrow", "Roen"]) || [])[0] === "Garrow");
check("no deeds -> no bark", pickDeedBark([], ALL) === null);
check("a plain flawless win still earns a word from Roen", (pickDeedBark(["deed.combat.flawless"], ALL) || [])[0] === "Roen");

// integrity: every priority entry has a bark, every speaker is a real companion
check("every prioritised deed has a bark line", DEED_PRIORITY.every(fl => DEED_BARKS[fl] && DEED_BARKS[fl].length));
check("every deed speaker is one of the four companions",
  Object.values(DEED_BARKS).every(arr => arr.every(b => ALL.indexOf(b.who) >= 0)));

// wiring: recordDeeds speaks the deed bark on victory, after the Chronicle entry
check("recordDeeds picks a deed bark from the earned flags", h.includes("const db=pickDeedBark(flags,present);"));
check("the deed bark is logged in-voice with a speech float",
  /log\(`\$\{db\[0\]\}: "\$\{db\[1\]\}"`,"sys"\);if\(sp\)spawnFloat\(sp,"💬"/.test(h));

console.log(`\n  Deed-reactive victory barks — picker + wiring:`);
for (const f of fails) console.log("  ✗ " + f);
console.log(`  ${pass} passed, ${fail} failed`);
process.exit(fail ? 1 : 0);
