// Pins tools/dlg-compile.py — the .dlg → C# dialogue compiler. Exercises the parser, the
// skill/flag mapping, the fail-branch + END handling, and that validation rejects dangling
// node references. Runs the real Python module against inline fixtures (no files written),
// so the generated C# can't silently drift from the DSL contract.
const { execSync } = require("child_process");
let pass = 0, fail = 0; const fails = [];
function check(name, cond) { if (cond) pass++; else { fail++; fails.push(name); } }

const driver = `
import importlib.util, json, os
spec = importlib.util.spec_from_file_location("dlgc", os.path.join("..","tools","dlg-compile.py"))
m = importlib.util.module_from_spec(spec); spec.loader.exec_module(m)
good = '''conversation: t.x | Test
node: 0 | Garrow
  A line of dialogue.
  * [INSIGHT 13] "probe" -> b ?? c | fx appr.roen +5, SET t.flag | if t.gate, !t.block, t.k>=2
  * "leave" -> END
node: b | Garrow
  win branch
node: c | Garrow
  fail branch'''
p = m.parse_dlg(good, "t.dlg")
errs = m.validate(p, "t.dlg")
cs = m.emit_cs(p)
bad = '''conversation: t.y | T2
node: 0 | A
  hi
  * "go" -> nowhere'''
berrs = m.validate(m.parse_dlg(bad, "b.dlg"), "b.dlg")
dup = '''conversation: t.z | T3
node: 0 | A
  one
  * "x" -> 0
node: 0 | A
  two'''
derrs = m.validate(m.parse_dlg(dup, "d.dlg"), "d.dlg")
print(json.dumps({
  "valid_ok": errs == [],
  "ability": "checkAbility = Ability.Wisdom" in cs,
  "dc": "checkDC = 13" in cs,
  "fail_branch": 'failNodeId = "c"' in cs,
  "fx_appr": '"companion.roen.approval"' in cs and "op = FlagOp.AddInt, amount = 5" in cs,
  "fx_set": '"t.flag", op = FlagOp.SetTrue' in cs,
  "cond_true": '"t.gate", op = FlagOp.RequireBoolTrue' in cs,
  "cond_false": '"t.block", op = FlagOp.RequireBoolFalse' in cs,
  "cond_int": '"t.k", op = FlagOp.RequireIntAtLeast, amount = 2' in cs,
  "end_empty": 'nextNodeId = ""' in cs,
  "keeps_bracket": '[INSIGHT 13]' in cs,
  "conv_id": 'g.conversationId = "t.x"' in cs,
  "regular_string": '@"' not in cs,            # must NOT use verbatim strings (extractor can't read them)
  "bad_caught": any("nowhere" in e for e in berrs),
  "dup_caught": any("duplicate" in e for e in derrs),
}))
`;

let out;
try { out = execSync(`python3 -c '${driver.replace(/'/g, "'\\''")}'`, { cwd: __dirname }).toString(); }
catch (e) { console.log("dlg-compile driver failed:\n" + (e.stdout || "") + (e.stderr || "")); process.exit(1); }
const r = JSON.parse(out);

check("a well-formed .dlg validates clean", r.valid_ok);
check("[INSIGHT 13] maps to a Wisdom check", r.ability && r.keeps_bracket);
check("the DC is carried through", r.dc);
check("'?? c' sets the fail branch", r.fail_branch);
check("'appr.roen +5' becomes an AddInt on companion.roen.approval", r.fx_appr);
check("'SET t.flag' becomes a SetTrue effect", r.fx_set);
check("'if t.gate' becomes RequireBoolTrue", r.cond_true);
check("'if !t.block' becomes RequireBoolFalse", r.cond_false);
check("'if t.k>=2' becomes RequireIntAtLeast 2", r.cond_int);
check("'-> END' emits an empty nextNodeId", r.end_empty);
check("the conversationId is emitted", r.conv_id);
check("emits regular (escapable) C# strings, not verbatim @\"\"", r.regular_string);
check("validation rejects a dangling node reference", r.bad_caught);
check("validation rejects a duplicate node id", r.dup_caught);

console.log(`\n  .dlg → C# dialogue compiler — DSL contract + validation:`);
for (const f of fails) console.log("  ✗ " + f);
console.log(`  ${pass} passed, ${fail} failed`);
process.exit(fail ? 1 : 0);
