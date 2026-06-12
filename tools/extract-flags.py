#!/usr/bin/env python3
"""
Build play/flags-data.json — the dependency graph of the game's GameFlags. Every flag is
the wire between something that WRITES it (a dialogue choice, a quest resolution, a deed,
a piece of era content) and something that READS it (an ending gate, a codex unlock, a
dialogue condition, a reactive line). We harvest three sources and merge them:

  1. C# GameFlags calls in Assets/Scripts/**.cs — GetBool/GetInt = read, SetBool/SetInt/AddInt = write.
  2. play/dialogue-data.json — FlagClause conditions (read) and onEnter/choice effects (write).
  3. play/compendium-data.json codex — each entry's unlockFlag (read by the Codex).

Output per flag: who writes it, who reads it, its domain (prefix), and orphan status
(written-but-never-read, or read-but-never-written → set in code / engine state).
Re-run after content changes (run the dialogue + content extractors first):
  python3 tools/extract-dialogue.py && python3 tools/extract-content.py && python3 tools/extract-flags.py
"""
import json, os, re, glob
from collections import defaultdict

ROOT = os.path.join(os.path.dirname(__file__), "..")
CS = glob.glob(os.path.join(ROOT, "Assets/Scripts/**/*.cs"), recursive=True)

# ---- friendly "system" label for a C# file ----------------------------------
def system_of(path):
    b = os.path.basename(path).replace(".cs", "")
    m = re.match(r'(\w+?)QuestContent$', b)
    if m: return f"{m.group(1)} quest"
    table = {
        "EndingResolver": "Endings", "GameFlags": "GameFlags core", "Deeds": "Deeds",
        "CodexContent": "Codex", "EpilogueResolver": "Epilogue", "Epilogue": "Epilogue",
        "CampaignBootstrap": "Campaign flow", "ActTwoContent": "Act II — Lower City",
        "BaldursGateHub": "The Gate (hub)", "NetherilContent": "Netheril",
        "NetherilEra": "Netheril", "CrownWarsContent": "Crown Wars", "CrownWarsEra": "Crown Wars",
        "FugueContent": "The Fugue", "FugueEra": "The Fugue", "EraEchoes": "Era echoes",
        "EraWitness": "Era witnesses", "SwordCoastContent": "Sword Coast", "RiddleContent": "The Vault",
        "AldricContent": "Aldric", "Romance": "Romance", "RomanceArc": "Romance",
    }
    return table.get(b, b)

DOMAIN_LABEL = {
    "quest": "Personal quests", "companion": "Companions", "romance": "Romance",
    "garrow": "Companions", "roen": "Companions", "varra": "Companions",
    "naeve": "Companions", "ilfaeril": "Companions", "maerin": "Companions",
    "act2": "Act II", "act3": "Act III", "act4": "Act IV", "act5": "Finale",
    "netheril": "Netheril", "crownwars": "Crown Wars",
    "toot": "Time of Troubles", "spellplague": "Spellplague", "fugue": "The Fugue",
    "prologue": "Prologue", "aldric": "Aldric", "lowcity": "Lower City",
    "almshouse": "Lower City", "docks": "Lower City", "safehouse": "Lower City",
    "market": "Lower City", "riddle": "The Vault", "vault": "The Vault", "tally": "The Vault",
    "rep": "Reputations", "reputation": "Reputations", "faction": "Reputations",
    "ng": "New Game+", "readers_boon": "The Lady", "hub": "The Gate",
    "camp": "Camp", "combat": "Combat", "dungeon": "Exploration", "slain": "Kills",
    "shop": "Economy", "game": "Game state", "pc": "Player", "tutorial": "Tutorial",
}

def domain(key):
    p = key.split(".")[0]
    return DOMAIN_LABEL.get(p, p.replace("_", " ").title())

# ---- 1. C# GameFlags calls --------------------------------------------------
READ_RE = re.compile(r'\.(GetBool|GetInt)\(\s*"([a-zA-Z0-9_.]+)"')
WRITE_RE = re.compile(r'\.(SetBool|SetInt|AddInt)\(\s*"([a-zA-Z0-9_.]+)"')

def harvest_cs(flags):
    for p in CS:
        if "/Tests/" in p: continue
        txt = open(p).read(); sysname = system_of(p)
        for m in READ_RE.finditer(txt):
            flags[m.group(2)]["readers"].add((sysname, "code"))
        for m in WRITE_RE.finditer(txt):
            flags[m.group(2)]["writers"].add((sysname, "code"))

# ---- 2. dialogue clauses ----------------------------------------------------
def harvest_dialogue(flags):
    path = os.path.join(ROOT, "play", "dialogue-data.json")
    if not os.path.exists(path): return
    data = json.load(open(path))
    for conv in data["conversations"]:
        src = conv["title"]
        for n in conv["nodes"]:
            for e in n.get("onEnter", []):
                if e.get("key"): flags[e["key"]]["writers"].add((src, "dialogue"))
            for ch in n.get("choices", []):
                for e in ch.get("effects", []):
                    if e.get("key"): flags[e["key"]]["writers"].add((src, "dialogue"))
                for c in ch.get("conditions", []):
                    if c.get("key"): flags[c["key"]]["readers"].add((src, "dialogue"))

# ---- 3. codex unlock flags --------------------------------------------------
def harvest_codex(flags):
    path = os.path.join(ROOT, "play", "compendium-data.json")
    if not os.path.exists(path): return
    data = json.load(open(path))
    for e in data.get("codex", []):
        if e.get("unlockFlag"):
            flags[e["unlockFlag"]]["readers"].add((f'Codex: {e["title"]}', "codex"))

def main():
    flags = defaultdict(lambda: {"writers": set(), "readers": set()})
    harvest_cs(flags)
    harvest_dialogue(flags)
    harvest_codex(flags)

    out = []
    for key in sorted(flags):
        w = sorted(flags[key]["writers"]); r = sorted(flags[key]["readers"])
        out.append({
            "key": key, "domain": domain(key),
            "writers": [{"source": s, "via": v} for s, v in w],
            "readers": [{"source": s, "via": v} for s, v in r],
            "orphanWrite": len(r) == 0,   # set but nothing gates on it
            "orphanRead": len(w) == 0,    # gated on but nothing sets it here (engine/code state)
        })
    domains = sorted({f["domain"] for f in out})
    stats = {
        "flags": len(out),
        "edges": sum(len(f["writers"]) + len(f["readers"]) for f in out),
        "domains": len(domains),
        "orphanWrites": sum(1 for f in out if f["orphanWrite"]),
        "orphanReads": sum(1 for f in out if f["orphanRead"]),
    }
    data = {"flags": out, "domains": domains, "stats": stats}
    dst = os.path.join(ROOT, "play", "flags-data.json")
    json.dump(data, open(dst, "w"), indent=1, ensure_ascii=False)
    print(f"extracted: {stats['flags']} flags across {stats['domains']} domains, "
          f"{stats['edges']} read/write edges "
          f"({stats['orphanWrites']} write-only, {stats['orphanReads']} read-only) -> play/flags-data.json")

if __name__ == "__main__":
    main()
