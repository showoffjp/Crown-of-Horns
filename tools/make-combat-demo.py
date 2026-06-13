#!/usr/bin/env python3
"""
Embed the CC0 Dungeon Crawl Stone Soup sprites into the playable combat demo
(play/crown_combat.html) as base64 data URIs, so the board renders real pixel art both
standalone and inside the all-in-one bundle. Idempotent: replaces the map between the
/*<SPR>*/ ... /*</SPR>*/ markers. Also renders play/combat_preview.png — a faithful
raster of the exact opening board (same grid, walls, roster, sprites, lighting) so the
look is verifiable without a browser.

  python3 tools/fetch-cc0-tiles.py && python3 tools/make-combat-demo.py
"""
import base64, io, json, os, re
from PIL import Image, ImageDraw

ROOT = os.path.join(os.path.dirname(__file__), "..")
ART = os.path.join(ROOT, "Assets/Resources/Art/DCSS")
HTML = os.path.join(ROOT, "play", "crown_combat.html")

# sprite key -> source tile (these are the names referenced in crown_combat.html)
SPRITES = {
    "floor_tomb0": "floor/tomb0",   "floor_tomb1": "floor/tomb1",
    "floor_tomb2": "floor/tomb2",   "floor_tomb3": "floor/tomb3",
    "wall_brick0": "wall/brick_dark0", "wall_brick1": "wall/brick_dark1",
    "wall_brick2": "wall/brick_dark2", "wall_brick3": "wall/brick_dark3",
    "spr_garrow": "mon/hero_priestess", "spr_roen": "mon/hero_human",
    "spr_varra": "mon/hero_warlock",
    "spr_returned": "mon/returned_wight", "spr_ghoul": "mon/returned_ghoul",
    "spr_zombie": "mon/returned_zombie", "spr_boss": "mon/eidolon",
}

def load(name):
    return Image.open(os.path.join(ART, name + ".png")).convert("RGBA")

def data_uri(name):
    with open(os.path.join(ART, name + ".png"), "rb") as f:
        return "data:image/png;base64," + base64.b64encode(f.read()).decode()

def inject():
    blob = "{" + ",".join(f'"{k}":"{data_uri(v)}"' for k, v in SPRITES.items()) + "}"
    src = open(HTML, encoding="utf-8").read()
    new, n = re.subn(r"/\*<SPR>\*/[\s\S]*?/\*</SPR>\*/", "/*<SPR>*/" + blob + "/*</SPR>*/", src)
    if n != 1:
        raise SystemExit("could not find the /*<SPR>*/ … /*</SPR>*/ marker in crown_combat.html")
    open(HTML, "w", encoding="utf-8").write(new)
    return len(blob)

# ---- faithful preview raster (mirrors the canvas draw) ----------------------
COLS, ROWS, TILE = 14, 10, 44
W, H = COLS * TILE, 496
BLOCKED = {(6,1),(7,1),(6,4),(7,5),(6,7),(7,7),(5,3),(8,2),(8,6),(6,8)}
ROSTER = [  # (x, y, sprite, side)
    (1,2,"spr_garrow","hero"), (1,4,"spr_roen","hero"), (1,6,"spr_varra","hero"),
    (10,1,"spr_returned","foe"), (11,3,"spr_ghoul","foe"),
    (11,6,"spr_returned","foe"), (10,8,"spr_zombie","foe"),
    (12,4,"spr_boss","foe"),
]
RING = {"hero":(127,208,160), "foe":(155,45,45)}

def _hash(x, y): return ((x*73856093) ^ (y*19349663)) & 0xffffffff
def floor_for(x, y): return ["floor_tomb0","floor_tomb1","floor_tomb2","floor_tomb3"][_hash(x,y)%4]
def wall_for(x, y):  return ["wall_brick0","wall_brick1","wall_brick2","wall_brick3"][_hash(x+7,y+3)%4]

def up(im, size):
    return im.resize((size, size), Image.NEAREST)

def preview():
    cache = {k: load(v) for k, v in SPRITES.items()}
    img = Image.new("RGBA", (W, H), (10, 9, 16, 255))
    # floors + walls
    for y in range(ROWS):
        for x in range(COLS):
            px, py = x*TILE, y*TILE
            img.alpha_composite(up(cache[floor_for(x,y)], TILE), (px, py))
            if (x,y) in BLOCKED:
                img.alpha_composite(up(cache[wall_for(x,y)], TILE), (px, py))
    # vignette
    vig = Image.new("L", (W, H), 0)
    vd = ImageDraw.Draw(vig)
    cx0, cy0, R = W*0.46, H*0.42, W*0.62
    for yy in range(0, H, 2):
        for xx in range(0, W, 2):
            d = (((xx-cx0)**2 + (yy-cy0)**2) ** 0.5) / R
            a = 0 if d < 0.5 else min(0.74, (d-0.5)/0.5*0.74)
            if a: vd.rectangle([xx, yy, xx+1, yy+1], fill=int(a*255))
    img = Image.alpha_composite(img, Image.merge("RGBA", (Image.new("L",(W,H),5),
            Image.new("L",(W,H),4), Image.new("L",(W,H),8), vig)))
    d = ImageDraw.Draw(img)
    # a couple of reachable tiles around the active hero (Garrow) to show the move overlay
    for (mx,my) in [(2,2),(2,3),(1,3),(2,1),(1,1),(3,2)]:
        px,py=mx*TILE,my*TILE; d.rectangle([px,py,px+TILE-2,py+TILE-2], outline=(201,162,75,120))
    # units
    for x,y,sp,side in ROSTER:
        cx = x*TILE + TILE//2; baseY = y*TILE + TILE - 2
        d.ellipse([cx-TILE*0.30, baseY-3-TILE*0.12, cx+TILE*0.30, baseY-3+TILE*0.12], fill=(0,0,0,110))
        col = (231,200,115) if (x,y)==(1,2) else RING[side]
        d.ellipse([cx-TILE*0.32, baseY-3-TILE*0.14, cx+TILE*0.32, baseY-3+TILE*0.14], outline=col, width=2)
        sz = TILE+8
        img.alpha_composite(up(cache[sp], sz), (cx-sz//2, baseY-sz+5))
        d = ImageDraw.Draw(img)
        w = TILE-12; bx = cx-w//2; by = y*TILE+TILE-6
        d.rectangle([bx,by,bx+w,by+4], fill=(0,0,0,170))
        d.rectangle([bx,by,bx+w,by+4], fill=(127,208,160) if side=="hero" else (201,85,77))
    img.convert("RGB").save(os.path.join(ROOT, "play", "combat_preview.png"))

def main():
    size = inject()
    preview()
    print(f"injected {len(SPRITES)} sprites (~{size//1024} KB blob) into play/crown_combat.html; "
          f"rendered play/combat_preview.png")

if __name__ == "__main__":
    main()
