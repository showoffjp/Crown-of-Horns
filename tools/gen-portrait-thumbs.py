#!/usr/bin/env python3
"""
Portrait thumbnails for the web game. The painted portraits
(Assets/Resources/Portraits, tools/gen-portraits-v3.py) are Unity-side PNGs of
~45 KB each — far too heavy to embed in the walkable page. This emits a small
JPEG per soul into play/portraits/<name>.jpg, which town_market.html loads
lazily (only for souls you actually speak to) exactly the way it loads the
painted floors, falling back to the sigil chip when a face is absent.

Deterministic; re-run after repainting portraits:
  python3 tools/gen-portraits-v3.py && python3 tools/gen-portrait-thumbs.py
"""
import glob, os
from PIL import Image

ROOT = os.path.join(os.path.dirname(__file__), "..")
SRC = os.path.join(ROOT, "Assets", "Resources", "Portraits")
OUT = os.path.join(ROOT, "play", "portraits")
W, H = 132, 165          # 4:5, twice the card's display size for crisp hidpi

def main():
    os.makedirs(OUT, exist_ok=True)
    n, total = 0, 0
    for p in sorted(glob.glob(os.path.join(SRC, "*.png"))):
        name = os.path.basename(p)[:-4]
        im = Image.open(p).convert("RGB")
        # cover-crop to the card aspect, favouring the head (upper third)
        sw, sh = im.size
        want = W / H
        if sw / sh > want:
            cw = int(sh * want)
            im = im.crop(((sw - cw) // 2, 0, (sw - cw) // 2 + cw, sh))
        else:
            ch = int(sw / want)
            top = max(0, int(sh * 0.06))
            if top + ch > sh:
                top = sh - ch
            im = im.crop((0, top, sw, top + ch))
        im = im.resize((W, H), Image.LANCZOS)
        out = os.path.join(OUT, name + ".jpg")
        im.save(out, quality=78, optimize=True)
        total += os.path.getsize(out)
        n += 1
    print(f"Wrote {n} portrait thumbnails ({total // 1024} KB) into play/portraits/")

if __name__ == "__main__":
    main()
