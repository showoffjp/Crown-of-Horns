#!/usr/bin/env python3
"""
Build a single contact-sheet PNG of the whole generated placeholder cast — map
tokens grouped by lore-era, then the dialogue portraits — so the team can see the
entire roster at a glance and judge what to replace with real art. Reproducible:
`python3 tools/make-contact-sheet.py`. Output: docs/art-preview/cast-contact-sheet.png
"""
import importlib.util
import os
from PIL import Image, ImageDraw, ImageFont

ROOT = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
SPRITES = os.path.join(ROOT, "Assets", "Resources", "Sprites")
PORTRAITS = os.path.join(ROOT, "Assets", "Resources", "Portraits")
OUT = os.path.join(ROOT, "docs", "art-preview", "cast-contact-sheet.png")

spec = importlib.util.spec_from_file_location("g", os.path.join(ROOT, "tools", "gen-placeholder-art.py"))
g = importlib.util.module_from_spec(spec)
spec.loader.exec_module(g)

ERA_TITLES = {
    "netheril": "NETHERIL  ·  gold & sky-blue",
    "crownwars": "CROWN WARS  ·  cold silver-green",
    "troubles": "TIME OF TROUBLES  ·  bone & blood",
    "spellplague": "SPELLPLAGUE  ·  blue fire",
    "fugue": "THE FUGUE / THE WALL  ·  grey absence",
    "sacred": "SWORD COAST  ·  sacred dread",
}
ERA_ORDER = ["netheril", "crownwars", "troubles", "spellplague", "fugue", "sacred"]


def font(sz, bold=True):
    for p in ("/usr/share/fonts/truetype/dejavu/DejaVuSans-Bold.ttf",
              "/usr/share/fonts/truetype/dejavu/DejaVuSans.ttf"):
        if os.path.exists(p):
            return ImageFont.truetype(p, sz)
    return ImageFont.load_default(size=sz)


def load(d, name):
    p = os.path.join(d, name + ".png")
    return Image.open(p).convert("RGBA") if os.path.exists(p) else None


def grid(images, cols, cell, pad, header_h):
    rows = (len(images) + cols - 1) // cols
    return cols * cell + pad * 2, rows * cell + header_h + pad, rows


def main():
    factions = ["Player", "Ally", "Enemy", "Neutral"]
    units = sorted(set(g.COMPANIONS + g.ENEMIES))
    by_era = {e: [] for e in ERA_ORDER}
    for u in units:
        by_era[g.classify(u)].append(u)

    cell, pad, hh, cols = 116, 28, 46, 8
    W = cols * cell + pad * 2

    # measure height
    sections = [("FACTIONS  ·  allegiance colors", factions)]
    for e in ERA_ORDER:
        if by_era[e]:
            sections.append((ERA_TITLES[e], by_era[e]))
    portraits = [p[:-4] for p in sorted(os.listdir(PORTRAITS)) if p.endswith(".png")]

    y = pad
    layout = []
    for title, names in sections:
        rows = (len(names) + cols - 1) // cols
        layout.append((title, names, y, "token"))
        y += hh + rows * cell + 18
    pcols = 7
    prows = (len(portraits) + pcols - 1) // pcols
    pcell = int(W - pad * 2) // pcols
    layout.append(("DIALOGUE PORTRAITS", portraits, y, "portrait"))
    y += hh + prows * int(pcell * 1.25) + pad
    H = y

    sheet = Image.new("RGBA", (W, H), (22, 22, 26, 255))
    d = ImageDraw.Draw(sheet)
    d.text((pad, 6), "THE CROWN OF HORNS — generated placeholder cast",
           font=font(22), fill=(232, 226, 210))

    for title, names, sy, kind in layout:
        d.rectangle([pad, sy + 6, W - pad, sy + 8], fill=(90, 80, 60))
        d.text((pad, sy + 12), title, font=font(17), fill=(210, 200, 175))
        if kind == "token":
            for i, nm in enumerate(names):
                im = load(SPRITES, nm)
                if im is None:
                    continue
                im = im.resize((cell - 12, cell - 12))
                cx = pad + (i % cols) * cell
                cy = sy + hh + (i // cols) * cell
                sheet.alpha_composite(im, (cx, cy))
        else:
            for i, nm in enumerate(names):
                im = load(PORTRAITS, nm)
                if im is None:
                    continue
                w = pcell - 10
                h = int(w * 1.25)
                im = im.resize((w, h))
                cx = pad + (i % pcols) * pcell
                cy = sy + hh + (i // pcols) * int(pcell * 1.25)
                sheet.alpha_composite(im, (cx, cy))

    os.makedirs(os.path.dirname(OUT), exist_ok=True)
    sheet.convert("RGB").save(OUT, "PNG")
    print(f"Wrote {OUT}  ({W}x{H}, {len(units)+len(factions)} tokens, {len(portraits)} portraits)")


if __name__ == "__main__":
    main()
