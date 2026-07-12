#!/usr/bin/env python3
"""
dlg-compile.py — compile readable .dlg dialogue files into the game's C# DialogueGraph
builders (Assets/Scripts/Content/<Name>Content.cs), so writers can author BG3-scale
branching dialogue in a clean DSL instead of hand-writing verbose C#.

The emitted C# matches the exact style the runtime + tools/extract-dialogue.py expect:
regular (non-verbatim) escaped string literals and the canonical field names.

Run:  python3 tools/dlg-compile.py            # compile every play/dialogue/*.dlg
      python3 tools/dlg-compile.py path.dlg   # compile one file

----------------------------------------------------------------------------------
.dlg FORMAT (one file -> one C# class, one or more conversations)
----------------------------------------------------------------------------------
  # lines starting with # are comments
  class: CampfireBanter                 # optional; else derived from the filename

  conversation: banter.garrow_roen | Garrow & Roen — On Mercy
  start: 0                              # optional; defaults to the first node

  node: 0 | Garrow
    You pray to nothing, thief. I have watched you do it — that little nod before a lock.
    enter: SET banter.garrow_roen.seen
    auto: 1

  node: 1 | Roen
    It's not a prayer. It's a promise to myself that I'll be quick. There's a difference.
    * "Leave it, both of you." -> calm | fx appr.garrow +2, appr.roen +2
    * [INSIGHT 13] "You're both stalling." -> truth ?? deflect
    * "..." -> END

  node: calm | Garrow
    ...Fair. Quick, then. We've a Wall to bring down.

----------------------------------------------------------------------------------
CHOICE TAIL GRAMMAR  (segments separated by ' | ', any order):
  * TEXT -> NEXT                         (NEXT = END or blank ends the conversation)
  * [SKILL DC] TEXT -> OK ?? FAIL        ([SKILL DC] auto-sets the check; ?? = fail branch)
  | fail NODE                            (alternative way to set the fail branch)
  | check SKILL DC                       (alternative way to set the check)
  | if  COND, COND                       (RequireBoolTrue/False, RequireIntAtLeast)
  | fx  FX, FX                           (SetTrue/SetFalse/AddInt)

COND tokens:  flag | !flag | key>=N
FX tokens:    SET flag | CLEAR flag | flag | -flag | appr.<id> +N | rep.<fac> +N | key +N
SKILL -> ability:  INSIGHT/PERCEPTION/MEDICINE/SURVIVAL -> Wisdom;
  PERSUADE/PERSUASION/DECEIVE/DECEPTION/INTIMIDATE/PERFORMANCE -> Charisma;
  ARCANA/HISTORY/INVESTIGATION/NATURE/RELIGION/LORE -> Intelligence;
  ATHLETICS -> Strength;  ACROBATICS/STEALTH/SLEIGHT -> Dexterity;
  ENDURE -> Constitution;  raw STR/DEX/CON/INT/WIS/CHA also accepted.
"""
import os, re, sys, glob

ROOT = os.path.join(os.path.dirname(__file__), "..")
DLG_DIR = os.path.join(ROOT, "play", "dialogue")
OUT_DIR = os.path.join(ROOT, "Assets", "Scripts", "Content")

SKILL_ABILITY = {
    "INSIGHT": "Wisdom", "PERCEPTION": "Wisdom", "MEDICINE": "Wisdom", "SURVIVAL": "Wisdom",
    "ANIMAL": "Wisdom", "WIS": "Wisdom", "WISDOM": "Wisdom",
    "PERSUADE": "Charisma", "PERSUASION": "Charisma", "DECEIVE": "Charisma", "DECEPTION": "Charisma",
    "INTIMIDATE": "Charisma", "INTIMIDATION": "Charisma", "PERFORMANCE": "Charisma", "CHARM": "Charisma",
    "CHA": "Charisma", "CHARISMA": "Charisma",
    "ARCANA": "Intelligence", "HISTORY": "Intelligence", "INVESTIGATION": "Intelligence",
    "NATURE": "Intelligence", "RELIGION": "Intelligence", "LORE": "Intelligence",
    "INT": "Intelligence", "INTELLIGENCE": "Intelligence",
    "ATHLETICS": "Strength", "STR": "Strength", "STRENGTH": "Strength",
    "ACROBATICS": "Dexterity", "STEALTH": "Dexterity", "SLEIGHT": "Dexterity",
    "DEX": "Dexterity", "DEXTERITY": "Dexterity",
    "ENDURE": "Constitution", "CON": "Constitution", "CONSTITUTION": "Constitution",
}

class DlgError(Exception):
    pass

def cs_escape(s):
    """A C# regular (non-verbatim) string body: escape \\ and ", newlines -> \\n."""
    return s.replace("\\", "\\\\").replace('"', '\\"').replace("\n", "\\n").replace("\r", "")

def parse_ability(skill):
    key = skill.strip().upper()
    if key not in SKILL_ABILITY:
        raise DlgError(f"unknown skill/ability '{skill}'")
    return SKILL_ABILITY[key]

def parse_clause(tok, as_condition):
    """One COND or FX token -> a dict {key, op, amount?}."""
    t = tok.strip()
    if not t:
        return None
    if as_condition:
        m = re.match(r"^([A-Za-z0-9_.]+)\s*>=\s*(-?\d+)$", t)
        if m:
            return {"key": m.group(1), "op": "RequireIntAtLeast", "amount": int(m.group(2))}
        if t.startswith("!"):
            return {"key": t[1:].strip(), "op": "RequireBoolFalse"}
        return {"key": t, "op": "RequireBoolTrue"}
    # ---- effects ----
    m = re.match(r"^(?:SET)\s+([A-Za-z0-9_.]+)$", t, re.I)
    if m:
        return {"key": m.group(1), "op": "SetTrue"}
    m = re.match(r"^(?:CLEAR|UNSET)\s+([A-Za-z0-9_.]+)$", t, re.I)
    if m:
        return {"key": m.group(1), "op": "SetFalse"}
    # appr.<id> +N  /  rep.<fac> +N  /  key +N  (AddInt)
    m = re.match(r"^appr\.([A-Za-z0-9_]+)\s*([+-]\s*\d+)$", t)
    if m:
        return {"key": f"companion.{m.group(1)}.approval", "op": "AddInt",
                "amount": int(m.group(2).replace(" ", ""))}
    m = re.match(r"^rep\.([A-Za-z0-9_]+)\s*([+-]\s*\d+)$", t)
    if m:
        return {"key": f"faction.{m.group(1)}.reputation", "op": "AddInt",
                "amount": int(m.group(2).replace(" ", ""))}
    m = re.match(r"^([A-Za-z0-9_.]+)\s*([+-]\s*\d+)$", t)
    if m:
        return {"key": m.group(1), "op": "AddInt", "amount": int(m.group(2).replace(" ", ""))}
    if t.startswith("-"):
        return {"key": t[1:].strip(), "op": "SetFalse"}
    return {"key": t, "op": "SetTrue"}

def parse_clause_list(s, as_condition):
    out = []
    for tok in s.split(","):
        c = parse_clause(tok, as_condition)
        if c:
            out.append(c)
    return out

def parse_choice(line, ctx):
    """'* TEXT -> NEXT | seg | seg' -> a choice dict."""
    body = line[1:].strip()  # drop leading '*'
    if " -> " not in body:
        raise DlgError(f"{ctx}: choice has no ' -> <node>': {line!r}")
    head, _, rest = body.partition(" -> ")
    # split the target+tail on ' | '
    segs = [s.strip() for s in rest.split(" | ")]
    target = segs[0].strip()
    tail = segs[1:]

    ch = {"text": head.strip()}

    # inline [SKILL DC] prefix in the text → a skill check; keep the bracket in the text.
    m = re.match(r"^\[\s*([A-Za-z]+)\s+(?:DC\s+)?(\d+)\s*\]", ch["text"])
    if m:
        ch["checkAbility"] = parse_ability(m.group(1))
        ch["checkDC"] = int(m.group(2))

    # target may carry a ' ?? failNode'
    if " ?? " in target:
        target, _, fail = target.partition(" ?? ")
        ch["failNodeId"] = "" if fail.strip().upper() == "END" else fail.strip()
        target = target.strip()
    ch["nextNodeId"] = "" if target.upper() == "END" else target

    for seg in tail:
        if not seg:
            continue
        kw, _, arg = seg.partition(" ")
        kw = kw.lower(); arg = arg.strip()
        if kw == "fail":
            ch["failNodeId"] = "" if arg.upper() == "END" else arg
        elif kw == "check":
            parts = arg.split()
            ch["checkAbility"] = parse_ability(parts[0])
            ch["checkDC"] = int(parts[1])
        elif kw == "if":
            ch["conditions"] = parse_clause_list(arg, True)
        elif kw == "fx":
            ch["effects"] = parse_clause_list(arg, False)
        else:
            raise DlgError(f"{ctx}: unknown choice segment '{seg}'")
    return ch

def parse_dlg(text, fname):
    """Parse a .dlg file into {class_name, conversations:[{id,title,start,nodes:[...]}]}."""
    class_name = None
    conversations = []
    conv = None
    node = None
    text_lines = None  # accumulating a node's text

    def flush_text():
        nonlocal node, text_lines
        if node is not None and text_lines is not None:
            node["text"] = "\n".join(text_lines).strip()
            text_lines = None

    for raw in text.splitlines():
        line = raw.rstrip("\n")
        stripped = line.strip()
        if not stripped or stripped.startswith("#"):
            continue
        ctx = f"{fname}"

        m = re.match(r"^class:\s*(\S+)\s*$", stripped)
        if m:
            class_name = m.group(1); continue

        m = re.match(r"^conversation:\s*(.+)$", stripped)
        if m:
            flush_text(); node = None
            spec = [p.strip() for p in m.group(1).split("|")]
            cid = spec[0]
            title = spec[1] if len(spec) > 1 and spec[1] else None
            conv = {"id": cid, "title": title, "start": None, "nodes": []}
            conversations.append(conv); continue

        m = re.match(r"^start:\s*(\S+)\s*$", stripped)
        if m:
            if conv is None:
                raise DlgError(f"{ctx}: 'start:' before any conversation")
            conv["start"] = m.group(1); continue

        m = re.match(r"^node:\s*(.+)$", stripped)
        if m:
            flush_text()
            if conv is None:
                raise DlgError(f"{ctx}: 'node:' before any conversation")
            spec = [p.strip() for p in m.group(1).split("|")]
            nid = spec[0]
            speaker = spec[1] if len(spec) > 1 and spec[1] else "NPC"
            node = {"id": nid, "speaker": speaker, "text": "", "auto": "",
                    "onEnter": [], "choices": []}
            conv["nodes"].append(node)
            text_lines = []
            if conv["start"] is None:
                conv["start"] = nid
            continue

        if node is None:
            raise DlgError(f"{ctx}: content outside any node: {stripped!r}")

        m = re.match(r"^enter:\s*(.+)$", stripped)
        if m:
            flush_text(); node["onEnter"] = parse_clause_list(m.group(1), False); continue
        m = re.match(r"^auto:\s*(\S+)\s*$", stripped)
        if m:
            flush_text(); node["auto"] = "" if m.group(1).upper() == "END" else m.group(1); continue
        # a choice line is '* ' (star + space); prose stage directions like '*(...)*' are text.
        if stripped.startswith("* "):
            flush_text(); node["choices"].append(parse_choice(stripped, ctx)); continue

        # otherwise: a line of node text (continuation)
        if text_lines is None:
            text_lines = []
        text_lines.append(stripped)

    flush_text()
    if class_name is None:
        base = os.path.splitext(os.path.basename(fname))[0]
        class_name = "".join(p.capitalize() for p in re.split(r"[^A-Za-z0-9]+", base) if p)
    return {"class_name": class_name, "conversations": conversations}

# ---- C# emission -----------------------------------------------------------
def emit_clause(c):
    s = f'new FlagClause {{ key = "{cs_escape(c["key"])}", op = FlagOp.{c["op"]}'
    if "amount" in c:
        s += f", amount = {c['amount']}"
    return s + " }"

def emit_clause_array(clauses):
    return "new[] { " + ", ".join(emit_clause(c) for c in clauses) + " }"

def emit_choice(ch, ind):
    parts = [f'text = "{cs_escape(ch["text"])}"']
    parts.append(f'nextNodeId = "{cs_escape(ch.get("nextNodeId",""))}"')
    if "checkDC" in ch:
        parts.append(f'checkAbility = Ability.{ch["checkAbility"]}')
        parts.append(f'checkDC = {ch["checkDC"]}')
    if ch.get("failNodeId") is not None and ch.get("failNodeId") != "" :
        parts.append(f'failNodeId = "{cs_escape(ch["failNodeId"])}"')
    elif "failNodeId" in ch:  # explicit blank fail (END) — emit it so intent is clear
        parts.append('failNodeId = ""')
    if ch.get("conditions"):
        parts.append("conditions = " + emit_clause_array(ch["conditions"]))
    if ch.get("effects"):
        parts.append("effects = " + emit_clause_array(ch["effects"]))
    return ind + "new DialogueChoice { " + ", ".join(parts) + " }"

def emit_node(n, ind):
    lines = [ind + "g.nodes.Add(new DialogueNode"]
    lines.append(ind + "{")
    b = ind + "    "
    lines.append(b + f'id = "{cs_escape(n["id"])}", speaker = "{cs_escape(n["speaker"])}",')
    lines.append(b + f'text = "{cs_escape(n["text"])}",')
    if n.get("auto"):
        lines.append(b + f'autoNextNodeId = "{cs_escape(n["auto"])}",')
    if n.get("onEnter"):
        lines.append(b + "onEnter = " + emit_clause_array(n["onEnter"]) + ",")
    if n.get("choices"):
        lines.append(b + "choices = new[]")
        lines.append(b + "{")
        for ch in n["choices"]:
            lines.append(emit_choice(ch, b + "    ") + ",")
        lines.append(b + "}")
    # trim a trailing comma on the last property for tidiness (optional in C#, but clean)
    # (C# allows trailing commas in initializers, so we leave them.)
    lines.append(ind + "});")
    return "\n".join(lines)

def prop_name(conv_id):
    parts = [p for p in re.split(r"[^A-Za-z0-9]+", conv_id) if p]
    name = "".join(p.capitalize() for p in parts)
    if not name or not name[0].isalpha():
        name = "Conv" + name
    return name

def emit_cs(parsed):
    cls = parsed["class_name"]
    convs = parsed["conversations"]
    props = [(prop_name(c["id"]) + "Dialogue", c) for c in convs]
    L = []
    L.append("// AUTO-GENERATED by tools/dlg-compile.py from a .dlg source — do not edit by hand.")
    L.append("// Re-generate with: python3 tools/dlg-compile.py")
    L.append("using UnityEngine;")
    L.append("using SunderedCrown.Dialogue;")
    L.append("using SunderedCrown.Stats;")
    L.append("")
    L.append("namespace SunderedCrown.Content")
    L.append("{")
    L.append(f"    public class {cls}")
    L.append("    {")
    for pn, _ in props:
        L.append(f"        public DialogueGraph {pn} {{ get; private set; }}")
    L.append("")
    L.append(f"        public {cls}()")
    L.append("        {")
    for i, (pn, c) in enumerate(props):
        L.append(f"            Build{pn}();")
    L.append("        }")
    for pn, c in props:
        L.append("")
        L.append(f"        private void Build{pn}()")
        L.append("        {")
        L.append("            var g = ScriptableObject.CreateInstance<DialogueGraph>();")
        L.append(f'            g.conversationId = "{cs_escape(c["id"])}";')
        L.append(f'            g.startNodeId = "{cs_escape(c["start"] or (c["nodes"][0]["id"] if c["nodes"] else "0"))}";')
        L.append("")
        for n in c["nodes"]:
            L.append(emit_node(n, "            "))
            L.append("")
        L.append(f"            {pn} = g;")
        L.append("        }")
    L.append("    }")
    L.append("}")
    return "\n".join(L) + "\n"

# ---- validation (headless; catches authoring errors before Unity ever sees it) ----
def validate(parsed, fname):
    errs = []
    for c in parsed["conversations"]:
        ids = [n["id"] for n in c["nodes"]]
        dup = {x for x in ids if ids.count(x) > 1}
        if dup:
            errs.append(f"{c['id']}: duplicate node id(s) {sorted(dup)}")
        idset = set(ids)
        if c["start"] not in idset:
            errs.append(f"{c['id']}: start node '{c['start']}' not found")
        def check_ref(ref, where):
            if ref and ref not in idset:
                errs.append(f"{c['id']}: {where} -> '{ref}' has no such node")
        for n in c["nodes"]:
            check_ref(n.get("auto"), f"node {n['id']} auto")
            for ch in n["choices"]:
                check_ref(ch.get("nextNodeId"), f"node {n['id']} choice")
                check_ref(ch.get("failNodeId"), f"node {n['id']} choice fail")
        # reachability from start
        seen, stack = set(), [c["start"]]
        while stack:
            cur = stack.pop()
            if cur in seen or cur not in idset:
                continue
            seen.add(cur)
            nd = next(n for n in c["nodes"] if n["id"] == cur)
            if nd.get("auto"):
                stack.append(nd["auto"])
            for ch in nd["choices"]:
                if ch.get("nextNodeId"):
                    stack.append(ch["nextNodeId"])
                if ch.get("failNodeId"):
                    stack.append(ch["failNodeId"])
        orphans = idset - seen
        if orphans:
            errs.append(f"{c['id']}: unreachable node(s) {sorted(orphans)}")
    return errs

def compile_file(path):
    parsed = parse_dlg(open(path, encoding="utf-8").read(), os.path.basename(path))
    errs = validate(parsed, path)
    if errs:
        raise DlgError("validation failed:\n  - " + "\n  - ".join(errs))
    cs = emit_cs(parsed)
    out = os.path.join(OUT_DIR, parsed["class_name"] + ".cs")
    open(out, "w", encoding="utf-8").write(cs)
    nodes = sum(len(c["nodes"]) for c in parsed["conversations"])
    return out, len(parsed["conversations"]), nodes

def main():
    args = sys.argv[1:]
    files = args if args else sorted(glob.glob(os.path.join(DLG_DIR, "*.dlg")))
    if not files:
        print("no .dlg files found (looked in play/dialogue/)."); return
    total_c = total_n = 0
    for f in files:
        try:
            out, nc, nn = compile_file(f)
            total_c += nc; total_n += nn
            print(f"compiled {os.path.basename(f)} -> {os.path.relpath(out, ROOT)} "
                  f"({nc} conversation(s), {nn} nodes)")
        except DlgError as e:
            print(f"ERROR in {os.path.basename(f)}: {e}"); sys.exit(1)
    print(f"done: {total_c} conversations, {total_n} nodes across {len(files)} file(s).")

if __name__ == "__main__":
    main()
