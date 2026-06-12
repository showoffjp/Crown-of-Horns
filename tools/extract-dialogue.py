#!/usr/bin/env python3
"""
Extract the game's branching conversations out of the C# DialogueGraph builders into
play/dialogue-data.json, so the Dialogue Viewer renders the REAL conversation graphs —
every node, choice, skill check (ability + DC, with the fail branch), and flag effect —
straight from Assets/Scripts/Content/*.cs. Pure brace-aware parsing of the C# object
initializers (no Unity needed). Re-run after content changes:
  python3 tools/extract-dialogue.py

Nodes whose `text` is a computed C# string (reactive witness beats that branch on prior
quest outcomes) can't be a single literal, so they're kept with a `dynamic` marker rather
than invented — the structure (id/speaker/choices) is still captured faithfully.
"""
import json, os, re, glob

ROOT = os.path.join(os.path.dirname(__file__), "..")
SRC = sorted(glob.glob(os.path.join(ROOT, "Assets/Scripts/Content/*.cs")))

# ---- string-aware helpers --------------------------------------------------
def matching_brace(s, i, open_ch="{", close_ch="}"):
    """s[i] == open_ch; return index of the matching close_ch, skipping strings."""
    depth = 0; n = len(s); instr = False
    while i < n:
        c = s[i]
        if instr:
            if c == "\\": i += 2; continue
            if c == '"': instr = False
        else:
            if c == '"': instr = True
            elif c == open_ch: depth += 1
            elif c == close_ch:
                depth -= 1
                if depth == 0: return i
        i += 1
    return -1

def split_top_level(s, sep=","):
    """Split on `sep` only at brace/paren/bracket depth 0, outside strings."""
    out = []; depth = 0; instr = False; cur = []; i = 0; n = len(s)
    while i < n:
        c = s[i]
        if instr:
            cur.append(c)
            if c == "\\" and i + 1 < n: cur.append(s[i+1]); i += 2; continue
            if c == '"': instr = False
        else:
            if c == '"': instr = True; cur.append(c)
            elif c in "{([": depth += 1; cur.append(c)
            elif c in "})]": depth -= 1; cur.append(c)
            elif c == sep and depth == 0:
                out.append("".join(cur)); cur = []
            else: cur.append(c)
        i += 1
    if "".join(cur).strip(): out.append("".join(cur))
    return out

_ESC = {"n": "\n", "t": "\t", '"': '"', "\\": "\\", "r": "\r"}
def parse_string_concat(v):
    """Join a run of C# string literals separated by '+'. Returns (text, is_literal)."""
    v = v.strip()
    if not v.startswith('"'):
        return (None, False)
    out = []; i = 0; n = len(v)
    while i < n:
        if v[i] == '"':
            i += 1
            while i < n and v[i] != '"':
                if v[i] == "\\" and i + 1 < n:
                    out.append(_ESC.get(v[i+1], v[i+1])); i += 2; continue
                out.append(v[i]); i += 1
            i += 1  # closing quote
        elif v[i] in " +\t\r\n":
            i += 1
        else:
            break  # something non-literal (a variable / call) — stop
    return ("".join(out), True)

def fields(body):
    """Parse a C# object-initializer body into {key: raw_value_string}."""
    out = {}
    for member in split_top_level(body, ","):
        if "=" not in member: continue
        # split on the first '=' at depth 0
        depth = 0; instr = False; idx = -1
        for j, c in enumerate(member):
            if instr:
                if c == "\\": continue
                if c == '"': instr = False
            elif c == '"': instr = True
            elif c in "{([": depth += 1
            elif c in "})]": depth -= 1
            elif c == "=" and depth == 0:
                idx = j; break
        if idx < 0: continue
        key = member[:idx].strip(); out[key] = member[idx+1:].strip()
    return out

def inner_braces(v):
    """For a `new[] { ... }` / `new X { ... }` value, return the inside of the braces."""
    a = v.find("{")
    if a < 0: return ""
    b = matching_brace(v, a)
    return v[a+1:b] if b > 0 else ""

def parse_clauses(v):
    out = []
    inner = inner_braces(v)
    for item in split_top_level(inner, ","):
        if "FlagClause" not in item: continue
        f = fields(inner_braces(item))
        key = parse_string_concat(f.get("key", ""))[0]
        op = (f.get("op", "") or "").split(".")[-1].strip()
        amt = f.get("amount")
        c = {"key": key, "op": op}
        if amt is not None:
            try: c["amount"] = int(amt)
            except ValueError: pass
        out.append(c)
    return out

def parse_choices(v):
    out = []
    inner = inner_braces(v)
    for item in split_top_level(inner, ","):
        if "DialogueChoice" not in item: continue
        f = fields(inner_braces(item))
        text = parse_string_concat(f.get("text", ""))[0]
        ch = {
            "text": text,
            "next": parse_string_concat(f.get("nextNodeId", ""))[0],
            "fail": parse_string_concat(f.get("failNodeId", ""))[0],
        }
        if f.get("checkDC"):
            try: dc = int(f["checkDC"])
            except ValueError: dc = 0
            if dc > 0:
                ch["checkDC"] = dc
                ch["checkAbility"] = (f.get("checkAbility", "Charisma")).split(".")[-1].strip()
        if f.get("conditions"): ch["conditions"] = parse_clauses(f["conditions"])
        if f.get("effects"): ch["effects"] = parse_clauses(f["effects"])
        out.append(ch)
    return out

# ---- node + conversation extraction ----------------------------------------
def title_of(conv_id):
    parts = re.split(r"[._]", conv_id)
    pretty = " · ".join(p.capitalize() for p in parts if p and p != "*")
    return pretty or conv_id

def extract_file(path):
    txt = open(path).read()
    fname = os.path.basename(path)
    convs = {}            # id -> conversation
    order = []
    current = None
    # walk the file, updating "current conversation" on each conversationId assignment,
    # and attaching every `new DialogueNode { ... }` to it.
    token = re.compile(r'g\.conversationId\s*=\s*("(?:[^"\\]|\\.)*"(?:\s*\+\s*\w+)?)|new\s+DialogueNode\b')
    start_re = re.compile(r'g\.startNodeId\s*=\s*"([^"]*)"')
    for m in token.finditer(txt):
        if m.group(1) is not None:
            cid = parse_string_concat(m.group(1))[0]
            if cid is None: cid = "(dynamic)"
            # dynamic suffix conversations (e.g. "market." + speaker)
            if re.search(r'\+\s*\w+', m.group(1)): cid = cid + "*"
            sm = start_re.search(txt, m.end(), m.end() + 120)
            start = sm.group(1) if sm else "0"
            if cid not in convs:
                convs[cid] = {"id": cid, "file": fname, "title": title_of(cid),
                              "start": start, "nodes": []}
                order.append(cid)
            current = cid
        else:
            a = txt.find("{", m.end())
            if a < 0: continue
            b = matching_brace(txt, a)
            if b < 0: continue
            body = txt[a+1:b]
            f = fields(body)
            nid = parse_string_concat(f.get("id", ""))[0]
            if nid is None: continue
            text, lit = parse_string_concat(f.get("text", ""))
            node = {
                "id": nid,
                "speaker": parse_string_concat(f.get("speaker", '"NPC"'))[0] or "NPC",
                "text": text if lit else None,
                "dynamic": not lit and "text" in f,
                "auto": parse_string_concat(f.get("autoNextNodeId", ""))[0],
                "onEnter": parse_clauses(f["onEnter"]) if f.get("onEnter") else [],
                "choices": parse_choices(f["choices"]) if f.get("choices") else [],
            }
            if current is None:
                current = "(" + fname.replace(".cs", "") + ")"
                if current not in convs:
                    convs[current] = {"id": current, "file": fname, "title": title_of(current),
                                      "start": nid, "nodes": []}
                    order.append(current)
            convs[current]["nodes"].append(node)
    return [convs[c] for c in order if convs[c]["nodes"]]

def main():
    all_convs = []
    for p in SRC:
        all_convs.extend(extract_file(p))
    # tidy: drop graphs with a single bare node and no branching only if also no text
    nodes = sum(len(c["nodes"]) for c in all_convs)
    choices = sum(len(n["choices"]) for c in all_convs for n in c["nodes"])
    checks = sum(1 for c in all_convs for n in c["nodes"] for ch in n["choices"] if ch.get("checkDC"))
    data = {"conversations": all_convs,
            "stats": {"conversations": len(all_convs), "nodes": nodes,
                      "choices": choices, "skillChecks": checks}}
    dst = os.path.join(ROOT, "play", "dialogue-data.json")
    json.dump(data, open(dst, "w"), indent=1, ensure_ascii=False)
    print(f"extracted: {len(all_convs)} conversations, {nodes} nodes, {choices} choices, "
          f"{checks} skill checks -> play/dialogue-data.json")

if __name__ == "__main__":
    main()
