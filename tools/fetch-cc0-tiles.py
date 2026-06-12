#!/usr/bin/env python3
"""
Fetch a curated set of CC0 (public-domain) game tiles from Dungeon Crawl Stone Soup
into Assets/Resources/Art/DCSS/ — real, reusable 32px game art for Crown of Horns:
dungeon floors & walls, doors/altars/statues, ~40 monster sprites mapped to our
bestiary archetypes, hero-usable humanoids, and weapon/armour/potion item icons.

Licensing (documented in the LICENSE.md this writes):
  - crawl/crawl LICENSE: "The majority of Crawl's tiles and artwork are released
    under the CC0 license", with the curated reuse project at github.com/crawl/tiles.
  - crawl/tiles publishes TILES_UNDER_UNKNOWN_LICENSE.md — tiles excluded from reuse.
    We download that list and HARD-SKIP any tile whose filename appears on it.
Only raw.githubusercontent.com is touched (the one allowlisted host).

  python3 tools/fetch-cc0-tiles.py
"""
import os, re, urllib.request

ROOT = os.path.join(os.path.dirname(__file__), "..")
DST = os.path.join(ROOT, "Assets/Resources/Art/DCSS")
RAW = "https://raw.githubusercontent.com/crawl/crawl/master/crawl-ref/source/rltiles"
EXCL_URL = "https://raw.githubusercontent.com/crawl/tiles/master/TILES_UNDER_UNKNOWN_LICENSE.md"

def get(url, binary=False):
    req = urllib.request.Request(url, headers={"User-Agent": "crown-of-horns-cc0-fetch"})
    with urllib.request.urlopen(req, timeout=30) as r:
        data = r.read()
    return data if binary else data.decode("utf-8", "replace")

# ---- the do-not-use list (filenames) ----------------------------------------
def load_exclusions():
    txt = get(EXCL_URL)
    return {m.group(1).strip() for m in re.finditer(r'^-\s*(\S+\.png)', txt, re.M)}

# ---- %sdir-aware manifest parser: tile-name -> repo path --------------------
def parse_manifest(name):
    txt = get(f"{RAW}/{name}")
    out = {}; sdir = ""
    for line in txt.splitlines():
        line = line.split("#")[0].strip()
        if not line: continue
        if line.startswith("%"):
            if line.startswith("%sdir"): sdir = line.split(None, 1)[1].strip()
            continue
        tok = line.split()[0]
        path = tok if "/" in tok else (f"{sdir}/{tok}" if sdir else tok)
        out[tok.split("/")[-1]] = path + ".png"
        # also index by enum name if present (MONS_FOO etc.)
        for enum in line.split()[1:]:
            out[enum] = path + ".png"
    return out

# ---- the curated want-list ---------------------------------------------------
# group -> [(local_name, tile key in manifest, manifest)]
WANT = {
 "floor": [(f"tomb{i}", f"tomb{i}") for i in range(4)] +
          [(f"grey_dirt{i}", f"grey_dirt{i}") for i in range(8)] +
          [(f"sandstone{i}", f"sandstone_floor{i}") for i in range(5)] +
          [(f"infernal{i}", f"metal_infernal{i}") for i in range(4)] +
          [(f"marble{i}", f"marble_floor{i}") for i in range(1, 5)] +
          [(f"pebble{i}", f"pebble_brown{i}") for i in range(4)],
 "wall": [(f"brick_dark{i}", f"brick_dark_1_{i}") for i in range(8)] +
         [(f"brick_brown{i}", f"brick_brown{i}") for i in range(4)] +
         [(f"stone_dark{i}", f"stone_dark{i}") for i in range(4)] +
         [(f"marble_wall{i}", f"marble_wall{i}") for i in range(1, 5)] +
         [(f"tomb_wall{i}", f"tomb{i}") for i in range(4)],
 "feat": [("door_closed", "DNGN_CLOSED_DOOR_CRYPT"), ("door_open", "DNGN_OPEN_DOOR_CRYPT"),
          ("statue_dwarf", "DNGN_GRANITE_STATUE"), ("statue_archer", "statue_archer"),
          ("statue_metal", "DNGN_METAL_STATUE"), ("altar_dark", "DNGN_ALTAR_YREDELEMNUL"),
          ("altar_stone", "DNGN_ALTAR_ZIN")],
 # bestiary archetypes — local name says what WE use it for
 "mon": [
  ("returned_wight", "MONS_WIGHT"), ("returned_ghoul", "MONS_GHOUL"),
  ("returned_zombie", "MONS_JIANGSHI"), ("restless_dead", "MONS_NECROPHAGE"),
  ("sorrow_wraith", "MONS_SHADOW_WRAITH"), ("wall_wraith", "MONS_WRAITH"),
  ("cold_wraith", "MONS_FREEZING_WRAITH"), ("hollow_cantor", "MONS_SILENT_SPECTRE"),
  ("pale_cantor", "MONS_MUMMY_PRIEST"), ("choir_shade", "MONS_SHADOW_DEMON"),
  ("flying_skull", "MONS_LAUGHING_SKULL"), ("bone_avatar", "MONS_ANCIENT_LICH"),
  ("bone_dragon", "MONS_BONE_DRAGON"), ("eidolon", "MONS_EIDOLON"),
  ("templar", "MONS_HELL_KNIGHT"), ("doomguide", "MONS_NECROMANCER"),
  ("inquisitor", "MONS_VAULT_SENTINEL"), ("zealot", "MONS_DEATH_KNIGHT"),
  ("bladesinger", "MONS_DEEP_ELF_BLADEMASTER"), ("elf_knight", "MONS_DEEP_ELF_KNIGHT"),
  ("elf_archer", "MONS_DEEP_ELF_ARCHER"), ("elf_sorcerer", "MONS_DEEP_ELF_SORCERER"),
  ("elf_deathmage", "MONS_DEEP_ELF_DEATH_MAGE"), ("elf_annihilator", "MONS_DEEP_ELF_ANNIHILATOR"),
  ("war_construct", "MONS_IRON_GOLEM"), ("weave_construct", "MONS_CRYSTAL_GUARDIAN"),
  ("clay_sentinel", "MONS_USHABTI"), ("war_gargoyle", "MONS_WAR_GARGOYLE"),
  ("arcane_sentinel", "MONS_ORB_GUARDIAN"), ("imp", "MONS_CRIMSON_IMP"),
  ("contract_devil", "MONS_RED_DEVIL"), ("cinder_hound", "MONS_HELL_HOUND"),
  ("herald_unmade", "MONS_TENTACLED_MONSTROSITY"), ("unmade_aberration", "MONS_ABOMINATION_LARGE"),
  ("unmade_small", "MONS_ABOMINATION_SMALL"), ("mythallar_colossus", "MONS_IRON_DRAGON"),
  # hero-usable humanoids
  ("hero_knight", "MONS_VAULT_GUARD"), ("hero_warden", "MONS_VAULT_WARDEN"),
  ("hero_priestess", "MONS_BURIAL_ACOLYTE"), ("hero_human", "MONS_HUMAN"),
  ("hero_mage", "MONS_ARCANIST"), ("hero_warlock", "MONS_OCCULTIST"),
 ],
 "item": [
  ("longsword", "WPN_LONG_SWORD"), ("mace", "WPN_MACE"), ("dagger", "WPN_DAGGER"),
  ("shortbow", "WPN_SHORTBOW"), ("quarterstaff", "WPN_QUARTERSTAFF"),
  ("chain_mail", "ARM_CHAIN_MAIL"), ("robe", "ARM_ROBE"), ("shield", "ARM_KITE_SHIELD"),
  ("potion", "UNSEEN_POTION"), ("scroll", "SCROLL"),
  ("amulet", "AMU_NORMAL_OFFSET"), ("book", "BOOK_MANUAL"),
 ],
}
MANIFEST_FOR = {"floor": "dc-floor.txt", "wall": "dc-wall.txt", "feat": "dc-feat.txt",
                "mon": "dc-mon.txt", "item": "dc-item.txt"}

def main():
    print("loading exclusion list (tiles under unknown license)…")
    excl = load_exclusions()
    print(f"  {len(excl)} excluded filenames")
    manifests = {g: parse_manifest(m) for g, m in MANIFEST_FOR.items()}
    got, skip_excl, miss = 0, 0, 0
    fetched = []
    for group, wants in WANT.items():
        man = manifests[group]
        os.makedirs(os.path.join(DST, group), exist_ok=True)
        for local, key in wants:
            path = man.get(key)
            if not path:
                # fall back: try lowercase tile-name form of the enum
                alt = key.replace("MONS_", "").replace("ITEM_", "").lower()
                path = man.get(alt)
            if not path:
                print(f"  ?? not in manifest: {group}/{key}"); miss += 1; continue
            if os.path.basename(path) in excl:
                print(f"  !! excluded (unknown license): {path}"); skip_excl += 1; continue
            try:
                data = get(f"{RAW}/{path}", binary=True)
            except Exception as e:
                print(f"  ?? fetch failed {path}: {e}"); miss += 1; continue
            if not data.startswith(b"\x89PNG"):
                print(f"  ?? not a PNG: {path}"); miss += 1; continue
            out = os.path.join(DST, group, local + ".png")
            open(out, "wb").write(data)
            fetched.append((group, local, path)); got += 1
    # provenance + license note
    with open(os.path.join(DST, "LICENSE.md"), "w") as f:
        f.write(
"""# CC0 art — provenance

These tiles come from **Dungeon Crawl Stone Soup** (github.com/crawl/crawl,
`crawl-ref/source/rltiles/`). Per the project's LICENSE: *"The majority of
Crawl's tiles and artwork are released under the CC0 license"*
(https://creativecommons.org/publicdomain/zero/1.0/), with the curated reuse
project at github.com/crawl/tiles.

Every file here was checked against that project's
`TILES_UNDER_UNKNOWN_LICENSE.md` exclusion list at fetch time; anything on the
list is never downloaded. Re-fetch / re-verify with `tools/fetch-cc0-tiles.py`.

Thanks to the DCSS dev team and tile artists (github.com/crawl/tiles/blob/master/ARTISTS.md).

| local file | source tile |
|---|---|
""")
        for g, l, p in fetched:
            f.write(f"| {g}/{l}.png | rltiles/{p} |\n")
    print(f"\nfetched {got} CC0 tiles -> Assets/Resources/Art/DCSS/ "
          f"({skip_excl} excluded by license list, {miss} not found)")

if __name__ == "__main__":
    main()
