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
import base64, io, json, math, os, re
from PIL import Image, ImageDraw, ImageChops

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
    "prop_altar": "feat/altar_dark", "prop_statue": "feat/statue_dwarf",
    "prop_statue2": "feat/statue_archer", "prop_door": "feat/door_closed",
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

# ---- faithful preview raster (mirrors the ISOMETRIC canvas draw) ------------
COLS, ROWS, TILE = 14, 10, 44
W, H = 616, 496
HW, HH, WALL_H, OX, OY = 24, 12, 18, 262, 52
BLOCKED = {(6,1),(7,1),(6,4),(7,5),(6,7),(7,7),(5,3),(8,2),(8,6),(6,8)}
ROSTER = [  # (x, y, sprite, side)
    (1,2,"spr_garrow","hero"), (1,4,"spr_roen","hero"), (1,6,"spr_varra","hero"),
    (10,1,"spr_returned","foe"), (11,3,"spr_ghoul","foe"),
    (11,6,"spr_returned","foe"), (10,8,"spr_zombie","foe"),
    (12,4,"spr_boss","foe"),
]
PROPS = [(13,4,"prop_altar"), (12,1,"prop_statue"), (12,7,"prop_statue2"), (0,5,"prop_door")]
DECALS = [(9,4)]
SURFS = {(10,1):"fire",(11,1):"fire",(10,2):"fire",(11,2):"fire",   # an ignited oil slick…
         (11,3):"grease",(12,3):"grease",(11,6):"grease",(12,6):"grease"}  # …spreading toward more oil
RING = {"hero":(127,208,160), "foe":(155,45,45)}

def isoX(gx, gy): return OX + (gx-gy)*HW
def isoY(gx, gy): return OY + (gx+gy)*HH
def cen(gx, gy):  return (OX + (gx-gy)*HW, OY + (gx+gy+1)*HH)
def dia(gx, gy, up=0):
    return [(isoX(gx,gy), isoY(gx,gy)-up), (isoX(gx+1,gy), isoY(gx+1,gy)-up),
            (isoX(gx+1,gy+1), isoY(gx+1,gy+1)-up), (isoX(gx,gy+1), isoY(gx,gy+1)-up)]

def preview():
    cache = {k: load(v) for k, v in SPRITES.items()}
    img = Image.new("RGBA", (W, H), (11, 10, 16, 255))
    d = ImageDraw.Draw(img, "RGBA")
    # PASS 1 — floor diamonds, decals, the active hero's move overlay
    for gy in range(ROWS):
        for gx in range(COLS):
            if (gx,gy) in BLOCKED: continue
            d.polygon(dia(gx,gy), fill=(30,27,39) if (gx+gy)%2 else (24,21,32), outline=(0,0,0))
    for (gx,gy) in DECALS:
        cx,cy = cen(gx,gy); d.ellipse([cx-15,cy-7,cx+15,cy+7], fill=(40,30,55,150))
    for (gx,gy) in [(2,2),(2,1),(1,3),(2,3),(3,2),(1,1)]:
        d.polygon(dia(gx,gy), outline=(201,162,75,170))
    # environmental surfaces — grease slicks and the fire eating through them
    for (gx,gy),ty in SURFS.items():
        d.polygon(dia(gx,gy), fill=(255,150,45,120) if ty=="fire" else (22,16,28,150))
        if ty=="fire":
            cx,cy = cen(gx,gy)
            for i in range(3):
                fxp = cx+math.sin(i*2.1+gx)*7; fyp = cy-3-((i*9+gx*5)%14)
                d.ellipse([fxp-2,fyp-2,fxp+2,fyp+2], fill=(255,210,74) if i%2 else (255,122,42))
    # torch lighting (screen-blended) — warm on heroes, cold on the Returned
    base = img.convert("RGB"); glow = Image.new("RGB",(W,H),(0,0,0))
    for x,y,sp,side in ROSTER:
        cx,cy = cen(x,y); color = (150,86,40) if side=="hero" else (58,70,150)
        lay = Image.new("RGB",(W,H),(0,0,0)); ld = ImageDraw.Draw(lay); rad=int(TILE*1.5)
        for r in range(rad,0,-2):
            f=1-r/rad; ld.ellipse([cx-r,cy-r,cx+r,cy+r], fill=(int(color[0]*f),int(color[1]*f),int(color[2]*f)))
        glow = ImageChops.screen(glow, lay)
    for (gx,gy),ty in SURFS.items():
        if ty != "fire": continue
        cx,cy = cen(gx,gy); lay = Image.new("RGB",(W,H),(0,0,0)); ld = ImageDraw.Draw(lay); rad=int(TILE*1.1)
        for r in range(rad,0,-2):
            f=1-r/rad; ld.ellipse([cx-r,cy-r,cx+r,cy+r], fill=(int(255*f*0.5),int(140*f*0.5),int(50*f*0.5)))
        glow = ImageChops.screen(glow, lay)
    img = ImageChops.screen(base, glow).convert("RGBA"); d = ImageDraw.Draw(img, "RGBA")
    # PASS 2 — walls, props, units, depth-sorted back-to-front
    ents = [(gx+gy,0,(gx,gy)) for (gx,gy) in BLOCKED]
    ents += [(x+y,1,(x,y,s)) for (x,y,s) in PROPS]
    ents += [(x+y,2,(x,y,s,side)) for (x,y,s,side) in ROSTER]
    ents.sort(key=lambda e:(e[0], e[1]))
    for _,t,e in ents:
        if t == 0:
            gx,gy = e
            A,B,C,D = [(isoX(a,b),isoY(a,b)) for a,b in [(gx,gy),(gx+1,gy),(gx+1,gy+1),(gx,gy+1)]]
            U = lambda p:(p[0],p[1]-WALL_H)
            d.polygon([B,C,U(C),U(B)], fill=(36,31,46))
            d.polygon([D,C,U(C),U(D)], fill=(21,18,29))
            d.polygon(dia(gx,gy,WALL_H), fill=(51,47,64) if (gx+gy)%2 else (44,40,56), outline=(12,10,18))
        else:
            x,y = e[0],e[1]; sp = e[2]; cx,cy = cen(x,y); sz = 40 if t==1 else 46
            d.ellipse([cx-sz*0.33,cy-sz*0.16,cx+sz*0.33,cy+sz*0.16], fill=(0,0,0,110))
            if t==2:
                side = e[3]; col = (231,200,115) if (x,y)==(1,2) else RING[side]
                d.ellipse([cx-sz*0.36,cy-sz*0.18,cx+sz*0.36,cy+sz*0.18], outline=col, width=2)
            img.alpha_composite(cache[sp].resize((sz,sz), Image.NEAREST), (int(cx-sz/2), int(cy-sz+6)))
            d = ImageDraw.Draw(img, "RGBA")
            if t==2:
                w=30; bx=cx-w/2; by=cy-sz-2
                d.rectangle([bx,by,bx+w,by+4], fill=(0,0,0,170))
                d.rectangle([bx,by,bx+w,by+4], fill=(127,208,160) if e[3]=="hero" else (201,85,77))
    # mid-fight VFX: Varra's force bolt, a crit burst, a slash (in iso screen space)
    ax,ay = cen(1,6); bx,by = cen(11,6); ay-=20; by-=20
    d.line([ax+18,ay,bx-10,by], fill=(240,138,210,235), width=3)
    d.ellipse([bx-14,by-4,bx-6,by+4], fill=(255,255,255,255))
    for rr,al in [(20,120),(11,185)]:
        d.ellipse([bx-rr,by-rr,bx+rr,by+rr], outline=(240,138,210,al), width=2)
    gx2,gy2 = cen(11,3); gy2-=18
    for rr,al in [(27,150),(16,205)]:
        d.ellipse([gx2-rr,gy2-rr,gx2+rr,gy2+rr], outline=(255,233,168,al), width=3)
    for s in range(7):
        a=s/7*6.283; sx,sy = gx2+math.cos(a)*33, gy2+math.sin(a)*33
        d.ellipse([sx-2,sy-2,sx+2,sy+2], fill=(255,255,255,235))
    zx,zy = cen(10,8); zy-=18
    d.arc([zx-16,zy-16,zx+16,zy+16], -62, 78, fill=(223,230,239,235), width=3)
    img.convert("RGB").save(os.path.join(ROOT, "play", "combat_preview.png"))

def main():
    size = inject()
    preview()
    print(f"injected {len(SPRITES)} sprites (~{size//1024} KB blob) into play/crown_combat.html; "
          f"rendered play/combat_preview.png")

if __name__ == "__main__":
    main()
