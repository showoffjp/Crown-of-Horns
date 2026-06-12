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

def main():
    data = {"abilities": extract_abilities(), "items": extract_items(), "enemies": extract_enemies()}
    dst = os.path.join(ROOT, "play", "compendium-data.json")
    json.dump(data, open(dst, "w"), indent=1)
    print(f"extracted: {len(data['abilities'])} abilities, {len(data['items'])} items, "
          f"{len(data['enemies'])} enemies -> play/compendium-data.json")

if __name__ == "__main__":
    main()
