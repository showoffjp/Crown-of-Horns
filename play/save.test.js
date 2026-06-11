// Save-contract tests: GameFlags -> flatten -> JSON -> parse -> unflatten must be
// lossless, the parallel key/value lists must stay in lockstep, and a re-save must
// reflect current state (no stale carry-over). Mirrors SaveSystem.cs's data model.
const { GameFlags } = require("./endings.js");
const { flatten, toJson, fromJson, apply } = require("./save.js");

let pass = 0, fail = 0; const fails = [];
function test(name, fn) { try { fn(); pass++; } catch (e) { fail++; fails.push(`${name}: ${e.message}`); } }
const eq = (a, b, m) => { if (a !== b) throw new Error(`${m || ""} expected ${JSON.stringify(b)}, got ${JSON.stringify(a)}`); };
const T = (c, m) => { if (!c) throw new Error(m || "expected true"); };

const roundtrip = (f, opts) => apply(fromJson(toJson(flatten(f, opts))), GameFlags);

test("EmptyFlags_RoundTripToEmpty", () => {
  const out = roundtrip(new GameFlags());
  eq(Object.keys(out.bools).length, 0); eq(Object.keys(out.ints).length, 0);
});

test("Bools_BothValuesSurvive", () => {
  const f = new GameFlags();
  f.SetBool("act1.smith.helped", true); f.SetBool("act1.smith.angered", false);
  const out = roundtrip(f);
  eq(out.GetBool("act1.smith.helped"), true);
  eq(out.GetBool("act1.smith.angered"), false);
  eq(out.GetBool("never.set"), false, "unset reads false");
});

test("Ints_EdgeValuesSurvive", () => {
  const f = new GameFlags();
  f.SetInt("party.gold", 123); f.SetInt("companion.sable.approval", -7);
  f.SetInt("zero", 0); f.SetInt("big", 2147483647); f.SetInt("min", -2147483648);
  const out = roundtrip(f);
  for (const [k, v] of [["party.gold", 123], ["companion.sable.approval", -7], ["zero", 0], ["big", 2147483647], ["min", -2147483648]])
    eq(out.GetInt(k), v, k);
});

test("ManyKeys_AllSurvive_AndStayInLockstep", () => {
  const f = new GameFlags();
  for (let i = 0; i < 250; i++) { f.SetBool("b." + i, i % 2 === 0); f.SetInt("i." + i, i * 7 - 100); }
  const dto = flatten(f);
  eq(dto.boolKeys.length, dto.boolValues.length, "bool key/value lists must match length");
  eq(dto.intKeys.length, dto.intValues.length, "int key/value lists must match length");
  eq(dto.boolKeys.length, 250); eq(dto.intKeys.length, 250);
  const out = apply(fromJson(toJson(dto)), GameFlags);
  for (let i = 0; i < 250; i++) { eq(out.GetBool("b." + i), i % 2 === 0, "b." + i); eq(out.GetInt("i." + i), i * 7 - 100, "i." + i); }
});

test("KeysWithDotsAndSpecials_Survive", () => {
  const f = new GameFlags();
  const keys = ["companion.garrow.approval", "quest.roen.double_agent", "a.b.c.d.e", "weird key with spaces", "emoji_🜍_key"];
  keys.forEach((k, i) => f.SetInt(k, i + 1));
  const out = roundtrip(f);
  keys.forEach((k, i) => eq(out.GetInt(k), i + 1, k));
});

test("Resave_ReflectsCurrentState_NoStaleCarryOver", () => {
  const f = new GameFlags();
  f.SetInt("party.gold", 100); f.SetBool("door.open", true);
  let out = roundtrip(f);
  eq(out.GetInt("party.gold"), 100);
  // mutate and re-save from the SAME flags object
  f.SetInt("party.gold", 250); f.SetBool("door.open", false);
  out = roundtrip(f);
  eq(out.GetInt("party.gold"), 250, "re-save sees the new value");
  eq(out.GetBool("door.open"), false, "re-save sees the cleared bool");
});

test("DoubleRoundTrip_IsStable", () => {
  const f = new GameFlags();
  f.SetBool("x", true); f.SetInt("y", 42);
  const once = roundtrip(f);
  const twice = roundtrip(once);
  eq(JSON.stringify(flatten(once).boolKeys), JSON.stringify(flatten(twice).boolKeys));
  eq(twice.GetBool("x"), true); eq(twice.GetInt("y"), 42);
});

console.log(`\n  Save round-trip — GameFlags ⇄ flattened DTO ⇄ JSON (the SaveSystem serialization contract):`);
console.log(`  ${pass} passed, ${fail} failed`);
if (fail) { fails.forEach(f => console.log("   ✗ " + f)); process.exit(1); }
else console.log("  ✓ Every flag survives the save/load hop; the parallel key/value lists never desync.\n");
