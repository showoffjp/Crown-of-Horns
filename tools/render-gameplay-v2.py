#!/usr/bin/env python3
"""
Render gameplay_v2 — a Crown of Horns combat screenshot composed from REAL game art:
the CC0 Dungeon Crawl Stone Soup tiles fetched by tools/fetch-cc0-tiles.py, laid out as
a crypt encounter (the Cinderhaunt) with torch lighting, fog, tactical overlays, and a
classic-CRPG HUD using the real item icons. Classic BG1-style presentation: the scene
fills the frame, UI on the edges.
  python3 tools/fetch-cc0-tiles.py && python3 tools/render-gameplay-v2.py
"""
import os
from PIL import Image, ImageDraw, ImageFont, ImageFilter, ImageChops, ImageEnhance

ROOT = os.path.join(os.path.dirname(__file__), "..")
ART = os.path.join(ROOT, "Assets/Resources/Art/DCSS")
FONTS = "/mnt/skills/examples/canvas-design/canvas-fonts"
DEJA = "/usr/share/fonts/truetype/dejavu"
serif  = lambda s: ImageFont.truetype(f"{FONTS}/IBMPlexSerif-Regular.ttf", s)
serifb = lambda s: ImageFont.truetype(f"{FONTS}/IBMPlexSerif-Bold.ttf", s)
serifi = lambda s: ImageFont.truetype(f"{FONTS}/IBMPlexSerif-Italic.ttf", s)
mono   = lambda s: ImageFont.truetype(f"{DEJA}/DejaVuSansMono.ttf", s)
monob  = lambda s: ImageFont.truetype(f"{DEJA}/DejaVuSansMono-Bold.ttf", s)

T = 32           # native tile px
SC = 2           # integer upscale
TS = T * SC      # on-screen tile size (64)
COLS, ROWS = 25, 13
W, H = COLS * TS, 1000   # 1600 x 1000

def tile(group, name):
    return Image.open(os.path.join(ART, group, name + ".png")).convert("RGBA")

import random
rng = random.Random(7)

# ---------------- the map ----------------
#  # wall   . floor   D closed door   d open door   A dark altar   S statue   M metal statue
MAP = [
 "#########################",
 "#########################",
 "##.....##########.....###",
 "##.....#####....#......##",
 "##..S......d....A...S..##",
 "##.....##.......#......##",
 "###....##.....#.#####.###",
 "###.#####.......#####.###",
 "##......#..............##",
 "##......#......##......##",
 "##..M...........#..M...##",
 "##.....................##",
 "#########################",
]

scene = Image.new("RGBA", (COLS * T, ROWS * T), (6, 5, 9, 255))

floors = [tile("floor", f"tomb{i}") for i in range(4)] + [tile("floor", f"grey_dirt{i}") for i in range(3)]
walls  = [tile("wall", f"brick_dark{i}") for i in range(8)]
feat = {"D": tile("feat", "door_closed"), "d": tile("feat", "door_open"),
        "A": tile("feat", "altar_dark"), "S": tile("feat", "statue_dwarf"),
        "M": tile("feat", "statue_metal")}

for r, row in enumerate(MAP):
    for c, ch in enumerate(row):
        if ch == "#":
            scene.paste(walls[rng.randrange(len(walls))], (c * T, r * T))
        else:
            f = floors[rng.randrange(4) if rng.random() < .8 else rng.randrange(len(floors))]
            scene.paste(f, (c * T, r * T))
            if ch in feat:
                g = feat[ch]
                scene.alpha_composite(g, (c * T, r * T + T - g.height))

# ---------------- tactical overlays under units ----------------
ov = Image.new("RGBA", scene.size, (0, 0, 0, 0))
od = ImageDraw.Draw(ov)
MOVE = [(5,9),(6,9),(7,9),(4,10),(5,10),(6,10),(7,10),(8,10),(5,11),(6,11),(7,11),(6,8),(7,8)]
for c, r in MOVE:
    od.rectangle([c*T+1, r*T+1, c*T+T-2, r*T+T-2], fill=(64, 110, 190, 54), outline=(120, 170, 235, 110))
# planned path active hero -> wight
for c, r in [(7,9),(8,9),(9,9),(10,8)]:
    od.ellipse([c*T+T//2-3, r*T+T//2-3, c*T+T//2+3, r*T+T//2+3], fill=(231, 200, 115, 170))
scene.alpha_composite(ov)

# ---------------- units ----------------
def put(group, name, c, r, ring=None, flip=False):
    g = tile(group, name)
    if flip: g = g.transpose(Image.FLIP_LEFT_RIGHT)
    x, y = c * T, r * T + T - g.height
    if ring:
        rd = ImageDraw.Draw(scene)
        rd.ellipse([c*T+3, r*T+T-12, c*T+T-3, r*T+T+2], outline=ring, width=2)
    scene.alpha_composite(g, (x, y))
    return (c * T + T // 2, y)

# enemies (the Returned + the Choir) near the altar
e1 = put("mon", "returned_wight", 10, 4, ring=(190, 70, 70, 230))
e2 = put("mon", "returned_wight", 12, 5, ring=(190, 70, 70, 230))
e3 = put("mon", "returned_ghoul", 9, 5, ring=(190, 70, 70, 230))
e4 = put("mon", "sorrow_wraith", 13, 4, ring=(190, 70, 70, 230))
e5 = put("mon", "hollow_cantor", 11, 3, ring=(150, 90, 190, 230))
# party
p1 = put("mon", "hero_knight", 6, 9, ring=(231, 200, 115, 255))          # the Returned (active)
p2 = put("mon", "hero_priestess", 4, 10)                                  # Sister Garrow
p3 = put("mon", "hero_warlock", 5, 11, flip=True)                         # Varra
p4 = put("mon", "hero_human", 7, 11)                                      # Roen

# ---------------- lighting ----------------
base = scene.convert("RGB")
# ambient: cool, dark crypt (but still readable)
amb = ImageEnhance.Color(ImageEnhance.Brightness(base).enhance(0.62)).enhance(0.82)
amb = ImageChops.multiply(amb, Image.new("RGB", base.size, (205, 208, 240)))

def radial(size, color, strength):
    g = Image.new("L", (size, size), 0)
    gd = ImageDraw.Draw(g)
    for i in range(size // 2, 0, -1):
        a = int(strength * (1 - i / (size / 2)) ** 1.6)
        gd.ellipse([size//2-i, size//2-i, size//2+i, size//2+i], fill=a)
    lay = Image.new("RGB", (size, size), color)
    lay.putalpha(g)
    return lay

light = Image.new("RGB", base.size, (0, 0, 0))
def add_light(cx, cy, size, color, strength=255):
    lay = radial(size, color, strength)
    tmp = Image.new("RGB", base.size, (0, 0, 0))
    tmp.paste(lay.convert("RGB"), (cx - size // 2, cy - size // 2), lay.split()[3])
    global light
    light = ImageChops.screen(light, tmp)

# warm torchlight around the party & the door; cold light at the altar/wraiths
add_light(*[v for v in p1], 560, (255, 178, 105))
add_light(p2[0], p2[1], 440, (255, 160, 92), 240)
add_light(p4[0], p4[1], 400, (240, 150, 88), 220)
add_light(11 * T + 16, 4 * T, 460, (135, 160, 255), 250)   # altar: cold necrotic glow
add_light(e4[0], e4[1], 260, (120, 140, 230), 190)
add_light(12 * T, 4 * T + 10, 220, (150, 110, 230), 170)
add_light(11 * T + 16, 4 * T + 8, 90, (210, 225, 255), 255) # altar hot core

lit = ImageChops.screen(amb, ImageChops.multiply(base, light))
lit = ImageEnhance.Brightness(lit).enhance(1.08)
# light fog drifting
fog = Image.new("L", base.size, 0)
fd = ImageDraw.Draw(fog)
for i in range(70):
    x, y = rng.randrange(base.width), rng.randrange(base.height)
    rr = rng.randrange(30, 90)
    fd.ellipse([x-rr, y-rr//2, x+rr, y+rr//2], fill=rng.randrange(8, 26))
fog = fog.filter(ImageFilter.GaussianBlur(18))
lit = Image.composite(Image.new("RGB", base.size, (140, 140, 170)), lit, fog.point(lambda v: v // 2))

scene_big = lit.resize((COLS * TS, ROWS * TS), Image.NEAREST)

# ---------------- compose frame + HUD ----------------
img = Image.new("RGB", (W, H), (8, 7, 12))
img.paste(scene_big, (0, 0))
d = ImageDraw.Draw(img, "RGBA")

GOLD = (231, 200, 115); INK = (216, 210, 194); DIM = (155, 147, 168)
RED = (224, 120, 120); GREEN = (140, 205, 130); PANEL = (16, 14, 22); LINE = (58, 52, 78)

# floating combat text (over the scene, full res)
def float_text(pt, s, col, size=22):
    x, y = pt[0] * SC, pt[1] * SC
    d.text((x+2, y-46+2), s, font=serifb(size), fill=(0, 0, 0, 200), anchor="mm")
    d.text((x, y-46), s, font=serifb(size), fill=col, anchor="mm")
float_text(e3, "9", (255, 120, 90))
float_text(e5, "CRIT 14!", GOLD, 24)
float_text(p2, "+6", GREEN, 20)

# name plates over hovered enemy
hx, hy = e1[0]*SC, e1[1]*SC
d.rounded_rectangle([hx-86, hy-86, hx+86, hy-50], 7, fill=(10, 9, 15, 215), outline=(150, 70, 70))
d.text((hx, hy-74), "Returned Wight", font=serifb(14), fill=INK, anchor="mm")
d.text((hx, hy-59), "AC 14 · 9/14 hp · Bloodied", font=mono(10), fill=RED, anchor="mm")

# ---- top bar ----
d.rectangle([0, 0, W, 46], fill=(10, 9, 15, 232))
d.line([0, 46, W, 46], fill=LINE)
d.text((20, 23), "👑 THE CINDERHAUNT", font=serifb(19), fill=GOLD, anchor="lm")
d.text((250, 24), "· crypts beneath Baldur's Gate — Act I", font=serifi(13), fill=DIM, anchor="lm")
d.text((W//2 + 140, 23), "Round 3 — The Returned's turn", font=serifb(15), fill=INK, anchor="mm")
d.text((W-22, 23), "⛶   ⚙   ⏸", font=serif(15), fill=DIM, anchor="rm")

# ---- combat log (translucent, upper left under the title bar) ----
d.rounded_rectangle([14, 60, 562, 202], 9, fill=(8, 7, 12, 200), outline=LINE)
log = [("Varra", "Eldritch Blast — natural 20!  CRIT for 14 force.", (200, 160, 230)),
       ("Sister Garrow", "Healing Word — the Returned regains 6 hp.", (150, 205, 140)),
       ("The Returned", "moves: 4 tiles — the wight is in reach.", (130, 170, 215)),
       ("Hollow Cantor", "begins the verse of unmaking…  (save next round)", (224, 120, 120))]
ly = 74
for who, line, col in log:
    d.text((30, ly), who, font=serifb(13), fill=col)
    d.text((30 + d.textlength(who, font=serifb(13)) + 8, ly), line, font=serif(13), fill=INK)
    ly += 23
d.text((30, ly+4), "▸ Mace vs Returned Wight:  72% hit · 7.4 expected · 31% lethal", font=monob(12), fill=GOLD)

# ---- bottom HUD ----
HUDY = H - 162
d.rectangle([0, HUDY, W, H], fill=(11, 10, 16, 244))
d.line([0, HUDY, W, HUDY], fill=(90, 78, 40))

# party portraits (real sprites, upscaled, framed)
party = [("The Returned", "hero_knight", 28, 34, True),
         ("Sister Garrow", "hero_priestess", 21, 27, False),
         ("Varra", "hero_warlock", 16, 22, False),
         ("Roen Alleywind", "hero_human", 19, 24, False)]
px = 18
for name, sp, hp, hpmax, act in party:
    fy = HUDY + 14
    d.rounded_rectangle([px, fy, px+96, fy+132], 8, fill=(24, 21, 32),
                        outline=(GOLD if act else LINE), width=2 if act else 1)
    g = tile("mon", sp)
    g = g.resize((g.width*3, g.height*3), Image.NEAREST)
    img.paste(g, (px + 48 - g.width//2, fy + 58 - g.height//2 - 6), g)
    d = ImageDraw.Draw(img, "RGBA")
    d.text((px+48, fy+106), name.split()[-1] if not act else "You", font=serifb(13),
           fill=(GOLD if act else INK), anchor="mm")
    frac = hp/hpmax
    d.rounded_rectangle([px+8, fy+118, px+88, fy+126], 4, fill=(14, 12, 20), outline=LINE)
    d.rounded_rectangle([px+9, fy+119, px+9+int(78*frac), fy+125], 4,
                        fill=(GREEN if frac > .5 else GOLD if frac > .25 else RED))
    px += 108

# ability bar with REAL item icons
abil = [("1", "longsword", "Longsword", "1d8"), ("2", "mace", "Mace", "1d6"),
        ("3", "shortbow", "Shortbow", "1d6"), ("4", "scroll", "Sacred Flame", "1d8"),
        ("5", "book", "Cure Wounds", "L1"), ("6", "amulet", "Bless", "L1"),
        ("Q", "potion", "Quaff", "×3"), ("G", "shield", "Defend", "")]
bx = px + 26
for key, ic, nm, tag in abil:
    fy = HUDY + 22
    d.rounded_rectangle([bx, fy, bx+86, fy+86], 9, fill=(26, 23, 36), outline=(110, 95, 52))
    g = tile("item", ic); g = g.resize((T*2, T*2), Image.NEAREST)
    img.paste(g, (bx + 43 - g.width//2, fy + 36 - g.height//2), g)
    d = ImageDraw.Draw(img, "RGBA")
    d.rounded_rectangle([bx+4, fy+4, bx+20, fy+20], 4, fill=(46, 40, 60), outline=GOLD)
    d.text((bx+12, fy+12), key, font=monob(11), fill=GOLD, anchor="mm")
    if tag: d.text((bx+80, fy+10), tag, font=mono(10), fill=DIM, anchor="rm")
    d.text((bx+43, fy+76), nm, font=serif(11), fill=INK, anchor="mm")
    bx += 96
    d.text((bx-96+43, fy+98), "", font=serif(9))

# end turn
d.rounded_rectangle([W-188, HUDY+30, W-22, HUDY+98], 12, fill=(48, 38, 15), outline=GOLD, width=2)
d.text((W-105, HUDY+54), "END TURN", font=serifb(18), fill=GOLD, anchor="mm")
d.text((W-105, HUDY+78), "↵ Enter", font=mono(11), fill=DIM, anchor="mm")
d.text((W-105, HUDY+118), "initiative: Garrow 19 · Wight 17 · You 15", font=mono(10), fill=DIM, anchor="mm")

# footer note
d.text((18, H-10), "in-engine target render · CC0 art (DCSS tileset) · combat numbers from the verified engine",
       font=serifi(11), fill=(120, 112, 134), anchor="lb")

out = os.path.join(ROOT, "play", "gameplay_v2.png")
img.save(out, "PNG")
print(f"wrote {out} ({os.path.getsize(out)//1024} KB, {W}x{H})")
