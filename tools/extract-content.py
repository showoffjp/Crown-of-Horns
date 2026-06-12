#!/usr/bin/env python3
"""
Extract the game's combat content out of the C# into play/compendium-data.json, so the
generated Compendium (grimoire / armory / bestiary) shows the REAL numbers, not invented
ones. Pure regex over the content+core scripts; re-run after content changes:
  python3 tools/extract-content.py
"""
import json, os, re, glob

ROOT = os.path.join(os.path.dirname(__file__), "..")
SRC = glob.glob(os.path.join(ROOT, "Assets/Scripts/Content/*.cs")) + \
      glob.glob(os.path.join(ROOT, "Assets/Scripts/Core/*.cs"))

def era_of(path):
    n = os.path.basename(path).lower()
    for key, era in [("netheril","Netheril"),("crownwars","Crown Wars"),("fugue","The Fugue"),
                     ("netheril","Netheril")]:
        if key in n: return era
    if "spellplague" in n: return "Spellplague"
    if "troubles" in n or "toot" in n: return "Time of Troubles"
    if "cinderhaunt" in n or "swordcoast" in n or "baldursgate" in n or "campaign" in n or "acttwo" in n:
        return "Sword Coast"
    return "Sword Coast"

# ---- abilities (Weapon / Spell builders) -----------------------------------
def extract_abilities():
    out = {}
    weapon_re = re.compile(r'Weapon\(\s*"([^"]+)"\s*,\s*"([^"]*)"\s*,\s*DamageType\.(\w+)\s*,\s*(\d+)\s*\)')
    spell_re = re.compile(
        r'Spell\(\s*"([^"]+)"\s*,\s*"([^"]*)"\s*,\s*DamageType\.(\w+)\s*,\s*'
        r'range:\s*(\d+)\s*,\s*slot:\s*(\d+)\s*,\s*attack:\s*(true|false)\s*\)')
    spell_pos = re.compile(
        r'Spell\(\s*"([^"]+)"\s*,\s*"([^"]*)"\s*,\s*DamageType\.(\w+)\s*,\s*(\d+)\s*,\s*(\d+)\s*,\s*(true|false)\s*\)')
    for p in SRC:
        txt = open(p).read()
        for m in weapon_re.finditer(txt):
            name, dice, dtype, rng = m.groups()
            out[name] = {"name": name, "kind": "Weapon", "dice": dice, "type": dtype,
                         "range": int(rng), "slot": 0, "attack": True, "era": era_of(p)}
        for rx in (spell_re, spell_pos):
            for m in rx.finditer(txt):
                name, dice, dtype, rng, slot, atk = m.groups()
                out[name] = {"name": name, "kind": "Cantrip" if int(slot) == 0 else f"Spell (L{slot})",
                             "dice": dice, "type": dtype, "range": int(rng), "slot": int(slot),
                             "attack": atk == "true", "era": era_of(p)}
    return list(out.values())

# ---- items -----------------------------------------------------------------
def extract_items():
    out = {}
    item_re = re.compile(r'Item\(\s*"([^"]+)"\s*,\s*"([^"]+)"\s*,\s*ItemKind\.(\w+)\s*,\s*EquipSlot\.(\w+)([^;]*)\)')
    kw = lambda body, key: (re.search(key + r':\s*"?([\w\d]+)"?', body) or [None, None])[1]
    for p in SRC:
        for m in item_re.finditer(open(p).read()):
            iid, name, kind, slot, rest = m.groups()
            out[iid] = {"id": iid, "name": name, "kind": kind, "slot": slot,
                        "value": kw(rest, "value"), "weaponDamage": (re.search(r'weaponDamage:\s*"([^"]+)"', rest) or [None,None])[1],
                        "wType": (re.search(r'wType:\s*DamageType\.(\w+)', rest) or [None,None])[1],
                        "ac": kw(rest, "ac")}
    return list(out.values())

def enemy_era(name, path):
    n = name.lower()
    table = [
        ("Netheril",        ["mythallar","netherese","arcane sentinel","arcanist","shadow-bound","weave-construct","war-construct"]),
        ("Crown Wars",      ["crown-war","first unmade","echo of the verdict","damned shade","vengeful shade","revenant"]),
        ("Time of Troubles",["avatar of bone","bone-zealot","kelemvorite","doomguide","templar","justiciar","god-touched","inquisitor","interrogator","enforcer","knight"]),
        ("Spellplague",     ["spellplague","herald of the unmade","aberration","weave-wraith","shadow-bound sentinel"]),
        ("The Fugue",       ["sorrow","unbound","maw","the unmade","unmaking","sorrow-wraith","damned"]),
        ("Sword Coast",     ["cinder-hound","contract-devil","imp","hollow cantor","pale cantor","choir","cantor","zealot"]),
    ]
    for era, keys in table:
        if any(k in n for k in keys): return era
    return era_of(path)

# ---- enemies ---------------------------------------------------------------
def extract_enemies():
    out = {}
    enemy_re = re.compile(
        r'Enemy\(\s*"([^"]+)"\s*,\s*\w+\s*,\s*(\d+)\s*,\s*(\d+)\s*,\s*(\d+)\s*,\s*([^,]+?)\s*,\s*(\d+)')
    for p in SRC:
        for m in enemy_re.finditer(open(p).read()):
            name, a, b, level, atk, xp = m.groups()
            atk = re.sub(r'.*\["([^"]+)"\].*', r'\1', atk.strip()).strip()
            key = name
            entry = {"name": name, "str": int(a), "con": int(b), "level": int(level),
                     "weapon": atk, "xp": int(xp), "era": enemy_era(name, p),
                     # CharacterSheet enemy AC = baseArmorClass(13-14) + DEX mod(+1)
                     "acApprox": 14}
            # keep the higher-XP version if a name recurs (the boss tuning)
            if key not in out or entry["xp"] > out[key]["xp"]: out[key] = entry
    return sorted(out.values(), key=lambda e: (e["era"], -e["xp"]))

# ---- conditions (the Condition enum vocabulary) ----------------------------
def extract_conditions():
    path = os.path.join(ROOT, "Assets/Scripts/Stats/Condition.cs")
    txt = open(path).read()
    body = txt[txt.index("enum Condition"):]
    body = body[body.index("{")+1: body.index("}")]
    out = []
    for line in body.splitlines():
        note = ""
        if "//" in line:
            line, note = line.split("//", 1); note = note.strip()
        name = line.strip().rstrip(",").strip()
        if not re.fullmatch(r'\w+', name) or name == "None": continue
        out.append({"name": name, "note": note})
    return out

# ---- status effects (the authored StatusEffectDefinition catalog) ----------
def _str_concat(v):
    parts = re.findall(r'"((?:[^"\\]|\\.)*)"', v)
    return "".join(parts).encode().decode("unicode_escape") if parts else None

def extract_effects():
    path = os.path.join(ROOT, "Assets/Scripts/Content/SwordCoastContent.cs")
    txt = open(path).read()
    i = txt.index("void BuildEffects")
    body = txt[txt.index("{", i):]
    depth = 0; end = 0
    for j, ch in enumerate(body):
        if ch == "{": depth += 1
        elif ch == "}":
            depth -= 1
            if depth == 0: end = j; break
    body = body[1:end]

    call_re = re.compile(
        r'Effect\(\s*"([^"]+)"\s*,\s*Condition\.(\w+)\s*,\s*(\d+)([^;]*)\)')
    out = []; var_of = {}  # variable name -> effect dict
    for m in call_re.finditer(body):
        name, cond, rounds, rest = m.groups()
        eff = {"name": name, "condition": cond, "rounds": int(rounds),
               "beneficial": "beneficial: true" in rest,
               "bearerDisadvantage": "bearerDisadv: true" in rest,
               "attackRollMod": int((re.search(r'atkMod:\s*(-?\d+)', rest) or [0, 0])[1]),
               "armorClassMod": 0, "speedMod": 0,
               "incapacitates": False, "attackersAdvantage": False,
               "dotDice": None, "dotType": None}
        # what variable / key was it bound to? look back on the same statement
        pre = body[:m.start()].rstrip()
        vm = re.search(r'(?:var\s+(\w+)|Effects\[\s*"(\w+)"\s*\])\s*=\s*$', pre)
        if vm and vm.group(1): var_of[vm.group(1)] = eff
        out.append(eff)
    # fold in the post-construction field assignments (burning DoT, slow speed, held flags)
    for am in re.finditer(r'(\w+)\.(\w+)\s*=\s*([^;]+);', body):
        var, field, val = am.groups()
        if var not in var_of: continue
        eff = var_of[var]; val = val.strip()
        if field == "damageOverTimeDice": eff["dotDice"] = _str_concat(val)
        elif field == "damageOverTimeType": eff["dotType"] = val.split(".")[-1]
        elif field == "speedModifier": eff["speedMod"] = int(val)
        elif field == "armorClassModifier": eff["armorClassMod"] = int(val)
        elif field == "incapacitates": eff["incapacitates"] = val == "true"
        elif field == "attackersHaveAdvantage": eff["attackersAdvantage"] = val == "true"
    return out

# ---- codex (the lore/bestiary journal) -------------------------------------
def extract_codex():
    path = os.path.join(ROOT, "Assets/Scripts/Content/CodexContent.cs")
    txt = open(path).read()
    out = []
    for m in re.finditer(r'new Entry\s*\{', txt):
        a = m.end() - 1
        depth = 0; end = 0
        for j in range(a, len(txt)):
            c = txt[j]
            if c == "{": depth += 1
            elif c == "}":
                depth -= 1
                if depth == 0: end = j; break
        blk = txt[a+1:end]
        def field(key):
            fm = re.search(key + r'\s*=\s*((?:"(?:[^"\\]|\\.)*"\s*\+?\s*)+)', blk)
            return _str_concat(fm.group(1)) if fm else None
        cat = re.search(r'category\s*=\s*Category\.(\w+)', blk)
        out.append({"id": field("id"), "category": cat.group(1) if cat else "Lore",
                    "title": field("title"), "unlockFlag": field("unlockFlag") or "",
                    "body": field("body") or ""})
    return out

def main():
    data = {"abilities": extract_abilities(), "items": extract_items(), "enemies": extract_enemies(),
            "conditions": extract_conditions(), "effects": extract_effects(), "codex": extract_codex()}
    dst = os.path.join(ROOT, "play", "compendium-data.json")
    json.dump(data, open(dst, "w"), indent=1)
    print(f"extracted: {len(data['abilities'])} abilities, {len(data['items'])} items, "
          f"{len(data['enemies'])} enemies, {len(data['conditions'])} conditions, "
          f"{len(data['effects'])} effects, {len(data['codex'])} codex entries -> play/compendium-data.json")

if __name__ == "__main__":
    main()
