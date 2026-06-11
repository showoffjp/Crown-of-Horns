#!/usr/bin/env python3
"""
Give multi-word companions a battle-map token under their EXACT runtime displayName.

UnitSpriteSkinner resolves a unit's sprite as Resources/Sprites/<displayName> ->
<firstWord> -> <faction>. "Sister Garrow" and "Roen Alleywind" therefore never hit
their own art (Garrow.png / their portrait) and fall back to a generic faction token.
This copies the best available source art to the exact-name file (+ a fresh-GUID .meta
cloned from the source's importer settings), so they show their own token. Idempotent;
re-run after adding real art with the matching name to supersede these aliases.
"""
import hashlib, os, re, shutil, sys

ROOT = os.path.join(os.path.dirname(__file__), "..")
ALIASES = {
    # exact runtime displayName -> source art to clone (token preferred; portrait as fallback)
    "Sister Garrow":  "Assets/Resources/Sprites/Garrow.png",
    "Roen Alleywind": "Assets/Resources/Portraits/Roen Alleywind.png",
}

def main():
    n = 0
    for name, src_rel in ALIASES.items():
        src = os.path.join(ROOT, src_rel)
        dst_rel = f"Assets/Resources/Sprites/{name}.png"
        dst = os.path.join(ROOT, dst_rel)
        if not os.path.exists(src):
            print(f"  skip {name}: source missing ({src_rel})"); continue
        shutil.copyfile(src, dst)
        meta = open(src + ".meta").read()
        guid = hashlib.md5(dst_rel.encode()).hexdigest()
        meta = re.sub(r"^guid: .*$", f"guid: {guid}", meta, count=1, flags=re.M)
        open(dst + ".meta", "w").write(meta)
        n += 1
        print(f"  + {dst_rel}  (from {os.path.basename(src_rel)})")
    print(f"Aliased {n} companion battle tokens.")

if __name__ == "__main__":
    main()
