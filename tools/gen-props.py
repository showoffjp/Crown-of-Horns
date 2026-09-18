#!/usr/bin/env python3
"""
Painted world props for the Unity build — v1.

Every non-person marker in the game used to render as a tinted cube: chests,
doors, stairways, braziers, notice boards, rifts. This paints a small library of
flat-shaded standees for them, in the same vocabulary as the portraits (warm rim
light from the upper left, a cool body, a dark keyline) so the world reads as one
piece of art.

Output: Assets/Resources/Props/<kind>.png, RGBA, cropped to the artwork, with a
bottom-centre pivot so a standee stands on its tile instead of floating. The
C# side (Rendering/PropArt.cs) maps a marker label to one of these names.

Deterministic. Re-run: python3 tools/gen-props.py
"""
import hashlib, math, os, random
from PIL import Image, ImageDraw, ImageFilter

ROOT = os.path.join(os.path.dirname(__file__), "..")
OUT = os.path.join(ROOT, "Assets", "Resources", "Props")
S = 256                      # square working canvas
GROUND = int(S * 0.94)       # where a prop's feet sit

# ---- shared palette ----------------------------------------------------------
WOOD      = (104, 72, 44)
WOOD_LIT  = (146, 104, 64)
WOOD_DARK = (58, 39, 24)
IRON      = (92, 96, 108)
IRON_LIT  = (140, 146, 160)
IRON_DARK = (44, 46, 56)
GOLD      = (214, 170, 78)
GOLD_DARK = (138, 104, 40)
STONE     = (108, 106, 112)
STONE_LIT = (152, 150, 156)
STONE_DARK= (56, 55, 62)
FLAME     = (248, 186, 84)
FLAME_HOT = (255, 236, 170)
EMBER     = (196, 78, 36)
CLOTH     = (92, 60, 76)
CLOTH_LIT = (134, 90, 110)
PAPER     = (206, 196, 172)
PAPER_DK  = (150, 140, 118)
VOID      = (26, 22, 34)
KEY       = (18, 15, 22, 235)   # keyline

def shade(c, f):
    return tuple(max(0, min(255, int(v * f))) for v in c[:3])

def canvas():
    return Image.new("RGBA", (S, S), (0, 0, 0, 0))

def box(d, x0, y0, x1, y1, base, lit=None, dark=None, key=True):
    """A slab with a lit top-left bevel and a shadowed bottom-right one."""
    lit = lit or shade(base, 1.35)
    dark = dark or shade(base, 0.6)
    d.rectangle([x0, y0, x1, y1], fill=base, outline=KEY if key else None, width=3)
    b = max(3, int((x1 - x0) * 0.09))
    d.polygon([(x0, y0), (x1, y0), (x1 - b, y0 + b), (x0 + b, y0 + b)], fill=lit)
    d.polygon([(x0, y0), (x0 + b, y0 + b), (x0 + b, y1 - b), (x0, y1)], fill=lit)
    d.polygon([(x1, y0), (x1, y1), (x1 - b, y1 - b), (x1 - b, y0 + b)], fill=dark)
    d.polygon([(x0, y1), (x1, y1), (x1 - b, y1 - b), (x0 + b, y1 - b)], fill=dark)

def flame(d, cx, by, w, h, hot=FLAME_HOT, mid=FLAME, low=EMBER, rnd=None):
    rnd = rnd or random.Random(7)
    for scale, col in ((1.0, low), (0.72, mid), (0.40, hot)):
        pts = [(cx - w * scale, by)]
        steps = 9
        for i in range(steps + 1):
            t = i / steps
            a = math.pi * t
            wobble = math.sin(t * math.pi * 3 + scale * 5) * w * 0.14 * scale
            x = cx - w * scale * math.cos(a) + wobble
            y = by - h * scale * math.sin(a) ** 0.7
            pts.append((x, y))
        pts.append((cx + w * scale, by))
        d.polygon(pts, fill=col)

def glow(img, cx, cy, r, color, alpha):
    g = Image.new("L", img.size, 0)
    gd = ImageDraw.Draw(g)
    for i in range(r, 0, -2):
        gd.ellipse([cx - i, cy - i, cx + i, cy + i], fill=int(alpha * (1 - i / r) ** 2))
    img.alpha_composite(Image.merge("RGBA", (*[Image.new("L", img.size, c) for c in color], g)))

# ---- the props ---------------------------------------------------------------

def p_chest(open_lid=False):
    img = canvas(); d = ImageDraw.Draw(img, "RGBA")
    cx = S // 2
    w, h = 96, 62
    top = GROUND - h
    if open_lid:
        # lid swung back, interior in shadow, nothing left inside
        d.polygon([(cx - w, top + 4), (cx + w, top + 4), (cx + w - 14, top - 46), (cx - w + 14, top - 46)],
                  fill=shade(WOOD, 0.75), outline=KEY, width=3)
        d.polygon([(cx - w + 20, top - 2), (cx + w - 20, top - 2), (cx + w - 28, top - 38), (cx - w + 28, top - 38)],
                  fill=WOOD_DARK)
    box(d, cx - w, top, cx + w, GROUND, WOOD, WOOD_LIT, WOOD_DARK)
    if open_lid:
        d.rectangle([cx - w + 12, top, cx + w - 12, top + 22], fill=(22, 18, 16), outline=KEY, width=2)
    else:
        # domed lid
        d.pieslice([cx - w, top - 44, cx + w, top + 30], 180, 360, fill=WOOD, outline=KEY, width=3)
        d.pieslice([cx - w + 10, top - 34, cx + w - 10, top + 18], 190, 300, fill=WOOD_LIT)
    # iron banding
    for bx in (cx - 54, cx + 54):
        d.rectangle([bx - 9, top - (34 if not open_lid else 0), bx + 9, GROUND - 4],
                    fill=IRON, outline=IRON_DARK, width=2)
        d.rectangle([bx - 9, top - (34 if not open_lid else 0), bx - 4, GROUND - 4], fill=IRON_LIT)
    # lock plate
    d.rectangle([cx - 16, top - 6, cx + 16, top + 26], fill=GOLD, outline=KEY, width=2)
    d.rectangle([cx - 16, top - 6, cx - 10, top + 26], fill=shade(GOLD, 1.3))
    d.ellipse([cx - 6, top + 6, cx + 6, top + 18], fill=GOLD_DARK if not open_lid else (20, 18, 16))
    return img

def p_barrel():
    img = canvas(); d = ImageDraw.Draw(img, "RGBA")
    cx = S // 2; w, h = 64, 130
    top = GROUND - h
    d.polygon([(cx - w, top + 14), (cx - w - 10, GROUND - 50), (cx - w, GROUND - 6),
               (cx + w, GROUND - 6), (cx + w + 10, GROUND - 50), (cx + w, top + 14)],
              fill=WOOD, outline=KEY, width=3)
    d.polygon([(cx - w, top + 14), (cx - w - 10, GROUND - 50), (cx - w, GROUND - 6), (cx - w + 22, GROUND - 10),
               (cx - w + 12, GROUND - 52), (cx - w + 24, top + 12)], fill=WOOD_LIT)
    d.ellipse([cx - w, top - 10, cx + w, top + 34], fill=shade(WOOD, 1.15), outline=KEY, width=3)
    for y in (top + 34, GROUND - 46):
        d.rectangle([cx - w - 9, y, cx + w + 9, y + 13], fill=IRON, outline=IRON_DARK, width=2)
        d.rectangle([cx - w - 9, y, cx + w + 9, y + 4], fill=IRON_LIT)
    return img

def p_door(open_door=False):
    img = canvas(); d = ImageDraw.Draw(img, "RGBA")
    cx = S // 2; w = 78; top = GROUND - 200
    # stone frame
    d.rectangle([cx - w - 26, top - 16, cx + w + 26, GROUND], fill=STONE, outline=KEY, width=3)
    d.rectangle([cx - w - 26, top - 16, cx - w - 12, GROUND], fill=STONE_LIT)
    d.rectangle([cx + w + 12, top - 16, cx + w + 26, GROUND], fill=STONE_DARK)
    d.pieslice([cx - w - 26, top - 76, cx + w + 26, top + 44], 180, 360, fill=STONE, outline=KEY, width=3)
    d.pieslice([cx - w - 12, top - 62, cx + w + 12, top + 30], 180, 360, fill=shade(STONE, 0.78))
    if open_door:
        d.rectangle([cx - w, top, cx + w, GROUND], fill=(16, 13, 20), outline=KEY, width=3)
        d.pieslice([cx - w, top - 60, cx + w, top + 40], 180, 360, fill=(16, 13, 20))
        glow(img, cx, top + 80, 90, (60, 48, 90), 90)
        d = ImageDraw.Draw(img, "RGBA")
        d.rectangle([cx - w, top, cx - w + 22, GROUND], fill=shade(WOOD, 0.55), outline=KEY, width=2)
    else:
        d.rectangle([cx - w, top, cx + w, GROUND], fill=WOOD, outline=KEY, width=3)
        d.pieslice([cx - w, top - 60, cx + w, top + 40], 180, 360, fill=WOOD, outline=KEY, width=3)
        for i in range(-2, 3):
            x = cx + i * 31
            d.line([(x, top - 30), (x, GROUND - 4)], fill=WOOD_DARK, width=3)
        d.rectangle([cx - w + 4, top + 44, cx + w - 4, top + 60], fill=IRON, outline=IRON_DARK, width=2)
        d.rectangle([cx - w + 4, top + 140, cx + w - 4, top + 156], fill=IRON, outline=IRON_DARK, width=2)
        d.ellipse([cx + 34, top + 96, cx + 58, top + 120], fill=GOLD, outline=KEY, width=2)
    return img

def p_stairs(down=False):
    img = canvas(); d = ImageDraw.Draw(img, "RGBA")
    cx = S // 2
    steps = 5
    if down:
        # a dark mouth in the floor, treads receding
        d.polygon([(cx - 108, GROUND), (cx + 108, GROUND), (cx + 74, GROUND - 104), (cx - 74, GROUND - 104)],
                  fill=(14, 12, 18), outline=KEY, width=3)
        for i in range(steps):
            t = i / steps
            y = GROUND - 12 - i * 19
            hw = 104 - i * 15
            d.polygon([(cx - hw, y), (cx + hw, y), (cx + hw - 10, y - 13), (cx - hw + 10, y - 13)],
                      fill=shade(STONE, 0.85 - t * 0.5), outline=KEY, width=2)
            d.line([(cx - hw + 10, y - 13), (cx + hw - 10, y - 13)], fill=shade(STONE_LIT, 0.9 - t * 0.5), width=3)
    else:
        for i in range(steps):
            t = i / steps
            y = GROUND - i * 26
            hw = 108 - i * 13
            d.polygon([(cx - hw, y), (cx + hw, y), (cx + hw - 8, y - 26), (cx - hw + 8, y - 26)],
                      fill=shade(STONE, 0.72 + t * 0.30), outline=KEY, width=2)
            d.line([(cx - hw + 8, y - 26), (cx + hw - 8, y - 26)], fill=STONE_LIT, width=4)
        glow(img, cx, GROUND - 150, 74, (120, 108, 80), 80)
    return img

def p_brazier(lit=True):
    img = canvas(); d = ImageDraw.Draw(img, "RGBA")
    cx = S // 2
    rim = GROUND - 128            # the lip of the bowl
    # three splayed legs
    for dx in (-52, 0, 52):
        d.line([(cx + dx, GROUND - 4), (cx + dx * 0.34, rim + 20)], fill=IRON_DARK, width=13)
        d.line([(cx + dx - 2, GROUND - 4), (cx + dx * 0.34 - 2, rim + 20)], fill=IRON, width=6)
    d.ellipse([cx - 58, GROUND - 18, cx + 58, GROUND + 2], fill=IRON_DARK)
    # a shallow bowl: sides, then the coal bed, then flame rising ABOVE the rim
    d.polygon([(cx - 74, rim), (cx + 74, rim), (cx + 46, rim + 46), (cx - 46, rim + 46)],
              fill=IRON, outline=KEY, width=3)
    d.polygon([(cx - 74, rim), (cx - 46, rim + 46), (cx - 22, rim + 44), (cx - 46, rim + 2)], fill=IRON_LIT)
    d.ellipse([cx - 74, rim - 17, cx + 74, rim + 17], fill=IRON_DARK, outline=KEY, width=3)
    d.ellipse([cx - 62, rim - 12, cx + 62, rim + 12],
              fill=(58, 40, 34) if not lit else (128, 44, 24))
    if lit:
        glow(img, cx, rim - 70, 128, (192, 96, 32), 150)
        d = ImageDraw.Draw(img, "RGBA")
        # coals sitting in the bed
        for dx, r in ((-38, 9), (-12, 11), (16, 9), (40, 8)):
            d.ellipse([cx + dx - r, rim - r // 2, cx + dx + r, rim + r // 2], fill=(214, 92, 38))
        flame(d, cx - 22, rim - 2, 26, 62, rnd=random.Random(5))
        flame(d, cx + 24, rim - 2, 22, 54, rnd=random.Random(9))
        flame(d, cx, rim + 2, 44, 128, rnd=random.Random(3))
        for dx, dy, r in ((-34, -212, 5), (26, -238, 4), (-8, -262, 3), (42, -192, 4)):
            d.ellipse([cx + dx - r, GROUND + dy - r, cx + dx + r, GROUND + dy + r], fill=(*FLAME, 200))
    else:
        for dx, r in ((-34, 10), (-6, 12), (22, 9), (44, 8)):
            d.ellipse([cx + dx - r, rim - r // 2, cx + dx + r, rim + r // 2], fill=(62, 56, 56))
            d.ellipse([cx + dx - r + 2, rim - r // 2 + 1, cx + dx, rim], fill=(84, 78, 78))
        # a thread of cold smoke
        for i in range(5):
            y = rim - 26 - i * 22
            d.arc([cx - 20 - i * 3, y - 14, cx + 20 + i * 3, y + 14],
                  200 + i * 30, 340 + i * 30, fill=(96, 92, 96, 150 - i * 22), width=4)
    return img
def p_campfire():
    img = canvas(); d = ImageDraw.Draw(img, "RGBA")
    cx = S // 2
    glow(img, cx, GROUND - 70, 130, (196, 108, 40), 150)
    d = ImageDraw.Draw(img, "RGBA")
    # ring of stones first — the fire sits inside it
    for i in range(9):
        a = math.pi * (0.05 + i / 9.4)
        x = cx - math.cos(a) * 96; y = GROUND - 8 + math.sin(a) * 10
        d.ellipse([x - 16, y - 13, x + 16, y + 13], fill=STONE, outline=KEY, width=2)
        d.ellipse([x - 13, y - 11, x + 3, y], fill=STONE_LIT)
    flame(d, cx, GROUND - 26, 46, 112, rnd=random.Random(11))
    # logs leaning through the flame, drawn on top so the wood stays legible
    for a, ln, off in ((-0.62, 116, -6), (0.58, 110, 8), (-0.18, 92, 2)):
        x0, y0 = cx - math.cos(a) * ln * 0.55 + off, GROUND - 12
        x1, y1 = cx + math.cos(a) * ln * 0.42 + off, GROUND - 12 - ln * 0.62
        d.line([(x0, y0), (x1, y1)], fill=(24, 18, 16), width=19)
        d.line([(x0, y0), (x1, y1)], fill=WOOD_DARK, width=13)
        d.line([(x0 - 2, y0), (x1 - 2, y1)], fill=WOOD, width=5)
        d.ellipse([x1 - 8, y1 - 8, x1 + 8, y1 + 8], fill=(216, 96, 40))
    flame(d, cx - 6, GROUND - 40, 22, 58, rnd=random.Random(4))
    for dx, dy, r in ((-30, -170, 4), (22, -192, 3), (0, -214, 3)):
        d.ellipse([cx + dx - r, GROUND + dy - r, cx + dx + r, GROUND + dy + r], fill=(*FLAME, 200))
    return img
def p_board():
    img = canvas(); d = ImageDraw.Draw(img, "RGBA")
    cx = S // 2
    d.rectangle([cx - 14, GROUND - 120, cx + 14, GROUND], fill=WOOD_DARK, outline=KEY, width=3)
    box(d, cx - 100, GROUND - 216, cx + 100, GROUND - 104, WOOD, WOOD_LIT, WOOD_DARK)
    d.rectangle([cx - 86, GROUND - 202, cx + 86, GROUND - 118], fill=(48, 38, 32))
    rnd = random.Random(5)
    for i in range(5):
        pw, ph = rnd.randint(30, 50), rnd.randint(28, 44)
        px = cx - 80 + (i % 3) * 56 + rnd.randint(-4, 4)
        py = GROUND - 198 + (i // 3) * 46 + rnd.randint(-3, 3)
        d.rectangle([px, py, px + pw, py + ph], fill=PAPER, outline=PAPER_DK, width=2)
        for ln in range(3):
            d.line([(px + 5, py + 8 + ln * 9), (px + pw - 5 - ln * 5, py + 8 + ln * 9)], fill=PAPER_DK, width=2)
        d.ellipse([px + pw / 2 - 4, py + 2, px + pw / 2 + 4, py + 10], fill=EMBER)
    return img

def p_shrine():
    img = canvas(); d = ImageDraw.Draw(img, "RGBA")
    cx = S // 2
    glow(img, cx, GROUND - 150, 96, (96, 116, 150), 90)
    d = ImageDraw.Draw(img, "RGBA")
    box(d, cx - 92, GROUND - 54, cx + 92, GROUND, STONE, STONE_LIT, STONE_DARK)
    box(d, cx - 68, GROUND - 92, cx + 68, GROUND - 48, STONE, STONE_LIT, STONE_DARK)
    d.polygon([(cx - 52, GROUND - 88), (cx + 52, GROUND - 88), (cx + 38, GROUND - 210), (cx - 38, GROUND - 210)],
              fill=STONE, outline=KEY, width=3)
    d.polygon([(cx - 52, GROUND - 88), (cx - 38, GROUND - 210), (cx - 16, GROUND - 208), (cx - 28, GROUND - 90)],
              fill=STONE_LIT)
    d.pieslice([cx - 38, GROUND - 244, cx + 38, GROUND - 178], 180, 360, fill=shade(STONE, 1.1), outline=KEY, width=3)
    # a weathered, unreadable name-band
    d.rectangle([cx - 30, GROUND - 176, cx + 30, GROUND - 160], fill=STONE_DARK)
    for i in range(4):
        d.line([(cx - 24 + i * 14, GROUND - 172), (cx - 18 + i * 14, GROUND - 164)], fill=shade(STONE_LIT, 0.8), width=2)
    # guttered candles at its foot
    for dx in (-58, -34, 40, 62):
        d.rectangle([cx + dx - 6, GROUND - 74, cx + dx + 6, GROUND - 52], fill=(206, 198, 176), outline=KEY, width=2)
        d.ellipse([cx + dx - 4, GROUND - 82, cx + dx + 4, GROUND - 72], fill=FLAME)
    return img

def p_banner():
    img = canvas(); d = ImageDraw.Draw(img, "RGBA")
    cx = S // 2
    d.line([(cx, GROUND), (cx, GROUND - 232)], fill=WOOD_DARK, width=13)
    d.line([(cx - 3, GROUND), (cx - 3, GROUND - 232)], fill=WOOD, width=5)
    d.line([(cx - 84, GROUND - 224), (cx + 84, GROUND - 224)], fill=IRON, width=9)
    pts = [(cx - 78, GROUND - 218), (cx + 78, GROUND - 218), (cx + 78, GROUND - 66),
           (cx + 40, GROUND - 92), (cx, GROUND - 60), (cx - 40, GROUND - 92), (cx - 78, GROUND - 66)]
    d.polygon(pts, fill=CLOTH, outline=KEY, width=3)
    d.polygon([(cx - 78, GROUND - 218), (cx - 24, GROUND - 218), (cx - 24, GROUND - 74),
               (cx - 40, GROUND - 92), (cx - 78, GROUND - 66)], fill=CLOTH_LIT)
    d.ellipse([cx - 40, GROUND - 180, cx + 40, GROUND - 100], outline=GOLD, width=6)
    d.line([(cx, GROUND - 176), (cx, GROUND - 104)], fill=GOLD, width=6)
    d.line([(cx - 28, GROUND - 140), (cx + 28, GROUND - 140)], fill=GOLD, width=6)
    return img

def p_rift():
    img = canvas(); d = ImageDraw.Draw(img, "RGBA")
    cx, cy = S // 2, GROUND - 120
    glow(img, cx, cy, 124, (92, 72, 190), 170)
    d = ImageDraw.Draw(img, "RGBA")
    rnd = random.Random(17)
    pts = []
    for i in range(22):
        t = i / 21
        a = math.pi * 2 * t
        r = 74 + math.sin(t * math.pi * 5) * 16 + rnd.randint(-8, 8)
        pts.append((cx + math.cos(a) * r * 0.62, cy + math.sin(a) * r * 1.3))
    d.polygon(pts, fill=(14, 10, 26), outline=(168, 148, 255), width=4)
    inner = [(cx + (x - cx) * 0.6, cy + (y - cy) * 0.6) for x, y in pts]
    d.polygon(inner, fill=(46, 28, 96))
    core = [(cx + (x - cx) * 0.26, cy + (y - cy) * 0.26) for x, y in pts]
    d.polygon(core, fill=(196, 182, 255))
    for i in range(18):
        a = rnd.uniform(0, math.pi * 2); r = rnd.uniform(90, 138)
        x, y = cx + math.cos(a) * r * 0.7, cy + math.sin(a) * r
        s = rnd.randint(2, 5)
        d.ellipse([x - s, y - s, x + s, y + s], fill=(*(178, 160, 255), rnd.randint(120, 230)))
    return img

def p_wall():
    """The Wall of the Faithless: mortared souls, seen up close."""
    img = canvas(); d = ImageDraw.Draw(img, "RGBA")
    cx = S // 2
    d.rectangle([cx - 116, GROUND - 236, cx + 116, GROUND], fill=(52, 46, 54), outline=KEY, width=3)
    rnd = random.Random(23)
    for row in range(7):
        y = GROUND - 228 + row * 32
        off = 0 if row % 2 == 0 else 26
        for col in range(-2, 3):
            x = cx + col * 52 + off
            if x < cx - 108 or x > cx + 108: continue
            g = rnd.randint(-10, 12)
            d.rectangle([x - 23, y, x + 23, y + 26], fill=(74 + g, 62 + g, 74 + g), outline=(34, 30, 36), width=2)
            d.line([(x - 21, y + 2), (x + 21, y + 2)], fill=(102 + g, 90 + g, 104 + g), width=3)
            if rnd.random() < 0.45:   # a face pressed out of the mortar
                d.ellipse([x - 11, y + 5, x + 11, y + 22], fill=(96, 86, 100))
                d.ellipse([x - 7, y + 10, x - 3, y + 15], fill=(28, 24, 30))
                d.ellipse([x + 3, y + 10, x + 7, y + 15], fill=(28, 24, 30))
                d.arc([x - 6, y + 13, x + 6, y + 22], 20, 160, fill=(28, 24, 30), width=2)
    glow(img, cx, GROUND - 120, 130, (70, 78, 104), 70)
    return img

def p_crowd():
    """A knot of onlookers — silhouettes, no faces: they are scenery, not souls."""
    img = canvas(); d = ImageDraw.Draw(img, "RGBA")
    cx = S // 2
    # back row first, dimmer and shorter, so the group reads as a crowd with depth
    for dx, depth in ((-78, 0.72), (-30, 0.78), (34, 0.74), (80, 0.70),
                      (-52, 0.96), (0, 1.02), (54, 0.94)):
        h = int(150 * depth); w = int(30 * depth)
        top = GROUND - h
        col = shade((54, 48, 64), 0.72 + depth * 0.42)
        neck = top + int(w * 1.5)
        d.polygon([(cx + dx - w, GROUND), (cx + dx - w + 3, neck + 8), (cx + dx - w * 0.45, neck),
                   (cx + dx + w * 0.45, neck), (cx + dx + w - 3, neck + 8), (cx + dx + w, GROUND)],
                  fill=col, outline=KEY, width=3)
        # a clear gap of keyline between shoulders and head, so heads stay heads
        d.ellipse([cx + dx - w * 0.62, top, cx + dx + w * 0.62, top + w * 1.24],
                  fill=shade(col, 1.22), outline=KEY, width=3)
        d.ellipse([cx + dx - w * 0.5, top + 4, cx + dx - w * 0.06, top + w * 0.72],
                  fill=shade(col, 1.5))
    return img
def p_market():
    img = canvas(); d = ImageDraw.Draw(img, "RGBA")
    cx = S // 2
    d.line([(cx - 96, GROUND), (cx - 96, GROUND - 150)], fill=WOOD_DARK, width=11)
    d.line([(cx + 96, GROUND), (cx + 96, GROUND - 150)], fill=WOOD_DARK, width=11)
    box(d, cx - 104, GROUND - 78, cx + 104, GROUND - 46, WOOD, WOOD_LIT, WOOD_DARK)
    # striped awning
    for i in range(7):
        x0 = cx - 112 + i * 32
        col = (168, 72, 62) if i % 2 == 0 else (212, 198, 170)
        d.polygon([(x0, GROUND - 156), (x0 + 32, GROUND - 156), (x0 + 38, GROUND - 112), (x0 + 6, GROUND - 112)],
                  fill=col, outline=KEY, width=2)
    d.line([(cx - 112, GROUND - 156), (cx + 112, GROUND - 156)], fill=WOOD_DARK, width=7)
    # wares
    for dx, col in ((-66, (188, 132, 62)), (-26, (142, 58, 58)), (18, (94, 128, 78)), (62, GOLD)):
        d.ellipse([cx + dx - 16, GROUND - 100, cx + dx + 16, GROUND - 72], fill=col, outline=KEY, width=2)
        d.ellipse([cx + dx - 12, GROUND - 96, cx + dx - 2, GROUND - 86], fill=shade(col, 1.4))
    return img

def p_debris():
    img = canvas(); d = ImageDraw.Draw(img, "RGBA")
    rnd = random.Random(31); cx = S // 2
    for i in range(14):
        x = cx + rnd.randint(-104, 104)
        y = GROUND - rnd.randint(0, 46)
        w = rnd.randint(16, 46); h = rnd.randint(10, 26)
        a = rnd.uniform(-0.5, 0.5)
        col = rnd.choice([WOOD, WOOD_DARK, STONE, shade(STONE, 0.7), (70, 78, 74)])
        dxs = [(-w, -h), (w, -h), (w, h), (-w, h)]
        pts = [(x + px * math.cos(a) - py * math.sin(a), y + px * math.sin(a) + py * math.cos(a)) for px, py in dxs]
        d.polygon(pts, fill=col, outline=KEY, width=2)
        d.line([pts[0], pts[1]], fill=shade(col, 1.45), width=3)
    # a tide line of salt
    d.arc([cx - 120, GROUND - 76, cx + 120, GROUND - 4], 200, 340, fill=(146, 152, 148), width=5)
    return img

def p_battle():
    """Crossed blades — 'there is a fight through here'."""
    img = canvas(); d = ImageDraw.Draw(img, "RGBA")
    cx, cy = S // 2, GROUND - 110
    glow(img, cx, cy, 104, (160, 52, 40), 120)
    d = ImageDraw.Draw(img, "RGBA")
    for sgn in (1, -1):
        a = math.radians(38 * sgn)
        dx, dy = math.sin(a), -math.cos(a)
        tipx, tipy = cx + dx * 112, cy + dy * 112
        hx, hy = cx - dx * 92, cy - dy * 92
        d.line([(hx, hy), (tipx, tipy)], fill=IRON_DARK, width=20)
        d.line([(hx, hy), (tipx, tipy)], fill=IRON_LIT if sgn > 0 else IRON, width=11)
        # crossguard + grip
        gx, gy = cx - dx * 62, cy - dy * 62
        d.line([(gx - dy * 30, gy + dx * 30), (gx + dy * 30, gy - dx * 30)], fill=GOLD, width=12)
        d.line([(hx, hy), (gx, gy)], fill=WOOD_DARK, width=16)
        d.ellipse([hx - 13, hy - 13, hx + 13, hy + 13], fill=GOLD, outline=KEY, width=2)
    return img

def p_chairs():
    img = canvas(); d = ImageDraw.Draw(img, "RGBA")
    cx = S // 2
    for dx, flip in ((-56, 1), (56, -1)):
        x = cx + dx
        d.rectangle([x - 34, GROUND - 60, x + 34, GROUND - 44], fill=WOOD, outline=KEY, width=3)
        d.rectangle([x - 34, GROUND - 60, x + 34, GROUND - 54], fill=WOOD_LIT)
        for lx in (x - 28, x + 28):
            d.rectangle([lx - 5, GROUND - 46, lx + 5, GROUND], fill=WOOD_DARK)
        bx = x + flip * 30
        d.rectangle([bx - 6, GROUND - 136, bx + 6, GROUND - 56], fill=WOOD, outline=KEY, width=3)
        for sy in (GROUND - 130, GROUND - 106):
            xa, xb = sorted((bx - flip * 6, bx - flip * 34))
            d.rectangle([xa, sy, xb, sy + 14], fill=WOOD_DARK)
    # a small table between them, one candle
    d.rectangle([cx - 22, GROUND - 74, cx + 22, GROUND - 62], fill=WOOD_LIT, outline=KEY, width=2)
    d.rectangle([cx - 5, GROUND - 62, cx + 5, GROUND], fill=WOOD_DARK)
    d.rectangle([cx - 5, GROUND - 96, cx + 5, GROUND - 72], fill=(210, 202, 180), outline=KEY, width=2)
    d.ellipse([cx - 5, GROUND - 106, cx + 5, GROUND - 94], fill=FLAME)
    glow(img, cx, GROUND - 100, 46, (190, 140, 60), 110)
    return img

def p_corpse():
    """A body under a shroud — the shape of a person, not a plank."""
    img = canvas(); d = ImageDraw.Draw(img, "RGBA")
    cx = S // 2
    base = GROUND - 8
    # the shroud follows a body: head swell, shoulder ridge, a dip, then the feet
    pts = [(cx - 108, base), (cx - 104, base - 30), (cx - 86, base - 52), (cx - 62, base - 56),
           (cx - 40, base - 44), (cx - 4, base - 40), (cx + 34, base - 48), (cx + 62, base - 40),
           (cx + 86, base - 50), (cx + 100, base - 34), (cx + 104, base)]
    d.polygon(pts, fill=CLOTH, outline=KEY, width=3)
    d.polygon([(cx - 108, base), (cx - 104, base - 30), (cx - 86, base - 52), (cx - 62, base - 56),
               (cx - 44, base - 46), (cx - 48, base)], fill=CLOTH_LIT)
    # folds
    for dx in (-26, 8, 46):
        d.line([(cx + dx, base - 42), (cx + dx + 8, base - 2)], fill=shade(CLOTH, 0.62), width=4)
    # one bare foot escaped the cloth, and a hand
    d.ellipse([cx + 88, base - 26, cx + 122, base - 2], fill=(186, 162, 140), outline=KEY, width=3)
    d.ellipse([cx - 128, base - 20, cx - 100, base - 2], fill=(186, 162, 140), outline=KEY, width=3)
    # a spill of hair from the head end
    for i in range(5):
        d.arc([cx - 118 - i * 4, base - 74 + i * 3, cx - 58 + i * 3, base - 34 + i * 3],
              180, 300, fill=(46, 38, 34), width=5)
    return img
def p_statue():
    """A forgotten god, robed, faceless, one arm broken off at the elbow."""
    img = canvas(); d = ImageDraw.Draw(img, "RGBA")
    cx = S // 2
    box(d, cx - 78, GROUND - 40, cx + 78, GROUND, STONE_DARK, shade(STONE, 0.88), (32, 30, 36))
    box(d, cx - 60, GROUND - 66, cx + 60, GROUND - 36, STONE, STONE_LIT, STONE_DARK)
    hem = GROUND - 66
    shoulder = hem - 128
    crown = shoulder - 66
    # robe: wide at the hem, narrowing to the shoulders
    d.polygon([(cx - 56, hem), (cx + 56, hem), (cx + 34, shoulder + 16), (cx + 26, shoulder),
               (cx - 26, shoulder), (cx - 34, shoulder + 16)], fill=STONE, outline=KEY, width=3)
    d.polygon([(cx - 56, hem), (cx - 34, shoulder + 16), (cx - 26, shoulder), (cx - 8, shoulder + 4),
               (cx - 22, hem)], fill=STONE_LIT)
    for dx in (-14, 12, 32):
        d.line([(cx + dx, shoulder + 26), (cx + dx * 1.5, hem - 6)], fill=STONE_DARK, width=4)
    # cowl over a blank face
    d.pieslice([cx - 40, crown - 16, cx + 40, crown + 76], 180, 360, fill=shade(STONE, 1.06),
               outline=KEY, width=3)
    d.polygon([(cx - 40, crown + 30), (cx + 40, crown + 30), (cx + 30, shoulder + 6), (cx - 30, shoulder + 6)],
              fill=shade(STONE, 1.02), outline=KEY, width=3)
    d.pieslice([cx - 27, crown - 2, cx + 27, crown + 62], 180, 360, fill=(48, 44, 52))
    d.rectangle([cx - 27, crown + 28, cx + 27, crown + 52], fill=(48, 44, 52))
    for dx in (-11, 11):   # two pale, pupil-less eyes in the dark of the hood
        d.ellipse([cx + dx - 6, crown + 26, cx + dx + 6, crown + 40], fill=(178, 176, 182))
    # left arm intact and holding a broken scale; right arm sheared at the elbow
    d.line([(cx - 28, shoulder + 14), (cx - 62, shoulder + 74)], fill=STONE, width=19)
    d.line([(cx - 30, shoulder + 14), (cx - 64, shoulder + 74)], fill=STONE_LIT, width=7)
    d.line([(cx + 28, shoulder + 14), (cx + 52, shoulder + 52)], fill=STONE, width=19)
    d.polygon([(cx + 44, shoulder + 40), (cx + 62, shoulder + 46), (cx + 56, shoulder + 64),
               (cx + 40, shoulder + 56)], fill=(38, 36, 42))   # the raw break
    # cracks
    d.line([(cx + 18, hem - 10), (cx + 6, shoulder + 50), (cx + 20, shoulder + 24)],
           fill=(44, 42, 48), width=3)
    return img
def p_ledger():
    """A book on a lectern — ledgers, cipher-boards, broadsides, the Wall's accounts."""
    img = canvas(); d = ImageDraw.Draw(img, "RGBA")
    cx = S // 2
    d.polygon([(cx - 54, GROUND), (cx + 54, GROUND), (cx + 26, GROUND - 24), (cx - 26, GROUND - 24)],
              fill=WOOD_DARK, outline=KEY, width=3)
    d.rectangle([cx - 11, GROUND - 130, cx + 11, GROUND - 20], fill=WOOD, outline=KEY, width=3)
    d.rectangle([cx - 11, GROUND - 130, cx - 4, GROUND - 20], fill=WOOD_LIT)
    d.polygon([(cx - 96, GROUND - 130), (cx + 96, GROUND - 130), (cx + 76, GROUND - 176), (cx - 76, GROUND - 176)],
              fill=WOOD, outline=KEY, width=3)
    glow(img, cx, GROUND - 170, 82, (172, 156, 96), 110)
    d = ImageDraw.Draw(img, "RGBA")
    for sgn in (-1, 1):
        d.polygon([(cx, GROUND - 146), (cx + sgn * 82, GROUND - 158), (cx + sgn * 78, GROUND - 200),
                   (cx, GROUND - 186)], fill=PAPER if sgn < 0 else shade(PAPER, 0.92), outline=KEY, width=2)
        for i in range(5):
            y = GROUND - 182 + i * 8
            d.line([(cx + sgn * 12, y), (cx + sgn * 66, y + sgn * 2)], fill=PAPER_DK, width=2)
    d.line([(cx, GROUND - 186), (cx, GROUND - 146)], fill=shade(PAPER, 0.7), width=4)
    return img

def p_bones():
    """A memorial of knotted rope: one knot tied for each name nobody will say."""
    img = canvas(); d = ImageDraw.Draw(img, "RGBA")
    cx = S // 2; rnd = random.Random(37)
    ROPE = (172, 150, 112); ROPE_LIT = (208, 188, 150); ROPE_DK = (112, 96, 70)
    # two leaning stakes with a line strung between them
    for sx, lean in ((-96, 8), (96, -8)):
        d.line([(cx + sx, GROUND), (cx + sx + lean, GROUND - 176)], fill=(22, 18, 16), width=17)
        d.line([(cx + sx, GROUND), (cx + sx + lean, GROUND - 176)], fill=WOOD_DARK, width=11)
        d.line([(cx + sx - 3, GROUND), (cx + sx + lean - 3, GROUND - 176)], fill=WOOD, width=4)
    # the strung line sags
    span = [(cx - 88 + i * 17.6, GROUND - 170 + math.sin(i / 10 * math.pi) * 16) for i in range(11)]
    d.line(span, fill=ROPE_DK, width=8)
    d.line([(x, y - 2) for x, y in span], fill=ROPE, width=4)
    # knotted cords hanging off it, each with a knot or two and a small token
    for i, (x, y) in enumerate(span[1:-1]):
        ln = 44 + rnd.randint(0, 62)
        sway = rnd.uniform(-9, 9)
        d.line([(x, y), (x + sway, y + ln)], fill=ROPE_DK, width=6)
        d.line([(x - 1, y), (x + sway - 1, y + ln)], fill=ROPE, width=3)
        for k in range(1 + (i % 2)):
            ky = y + 16 + k * 26
            if ky > y + ln - 8: break
            kx = x + sway * (ky - y) / ln
            d.ellipse([kx - 8, ky - 7, kx + 8, ky + 7], fill=ROPE, outline=KEY, width=2)
            d.ellipse([kx - 6, ky - 5, kx + 1, ky + 1], fill=ROPE_LIT)
        tx, ty = x + sway, y + ln
        if i % 3 == 0:
            d.ellipse([tx - 9, ty - 4, tx + 9, ty + 14], fill=(198, 190, 168), outline=KEY, width=2)
        elif i % 3 == 1:
            d.polygon([(tx, ty - 4), (tx + 10, ty + 8), (tx, ty + 18), (tx - 10, ty + 8)],
                      fill=GOLD, outline=KEY, width=2)
        else:
            d.rectangle([tx - 8, ty - 3, tx + 8, ty + 13], fill=PAPER, outline=KEY, width=2)
            d.line([(tx - 4, ty + 2), (tx + 4, ty + 2)], fill=PAPER_DK, width=2)
            d.line([(tx - 4, ty + 7), (tx + 2, ty + 7)], fill=PAPER_DK, width=2)
    # a few spent candles at the foot of the stakes
    for dx in (-104, -80, 84, 104):
        d.rectangle([cx + dx - 5, GROUND - 26, cx + dx + 5, GROUND - 4], fill=(204, 196, 176),
                    outline=KEY, width=2)
    return img

def p_figure():
    """One hooded onlooker — the 'a watchful X' / 'a hooded figure' markers."""
    img = canvas(); d = ImageDraw.Draw(img, "RGBA")
    cx = S // 2
    h = 214; w = 46
    top = GROUND - h
    body = (62, 56, 74)
    neck = top + 66
    d.polygon([(cx - w - 14, GROUND), (cx - w, neck + 16), (cx - w * 0.5, neck),
               (cx + w * 0.5, neck), (cx + w, neck + 16), (cx + w + 14, GROUND)],
              fill=body, outline=KEY, width=3)
    d.polygon([(cx - w - 14, GROUND), (cx - w, neck + 16), (cx - w * 0.5, neck),
               (cx - 6, neck + 10), (cx - 16, GROUND)], fill=shade(body, 1.35))
    for dx in (-10, 16, 34):     # robe folds
        d.line([(cx + dx, neck + 26), (cx + dx * 1.7, GROUND - 6)], fill=shade(body, 0.62), width=4)
    # hood: a cowl with a dark hollow and two points of light
    d.pieslice([cx - 40, top, cx + 40, top + 96], 180, 360, fill=shade(body, 1.12), outline=KEY, width=3)
    d.polygon([(cx - 40, top + 46), (cx + 40, top + 46), (cx + 30, neck + 8), (cx - 30, neck + 8)],
              fill=shade(body, 1.06), outline=KEY, width=3)
    d.pieslice([cx - 27, top + 14, cx + 27, top + 82], 180, 360, fill=(26, 22, 32))
    d.rectangle([cx - 27, top + 46, cx + 27, top + 70], fill=(26, 22, 32))
    for dx in (-10, 10):
        d.ellipse([cx + dx - 5, top + 44, cx + dx + 5, top + 56], fill=(206, 196, 168))
    # hands folded at the waist
    d.ellipse([cx - 18, neck + 58, cx + 18, neck + 84], fill=(168, 142, 118), outline=KEY, width=3)
    return img

def p_signpost():
    """A leaning signpost with two boards — 'the Docks (waterfront)', 'the Niche'."""
    img = canvas(); d = ImageDraw.Draw(img, "RGBA")
    cx = S // 2
    d.line([(cx + 8, GROUND), (cx - 6, GROUND - 216)], fill=(22, 18, 16), width=21)
    d.line([(cx + 8, GROUND), (cx - 6, GROUND - 216)], fill=WOOD_DARK, width=15)
    d.line([(cx + 4, GROUND), (cx - 10, GROUND - 216)], fill=WOOD, width=6)
    for y, sgn, wdt in ((GROUND - 186, 1, 108), (GROUND - 128, -1, 92)):
        x0 = cx - 2
        x1 = x0 + sgn * wdt
        xa, xb = min(x0, x1), max(x0, x1)
        d.polygon([(xa, y), (xb, y + (0 if sgn > 0 else 0)), (xb, y + 40), (xa, y + 40)],
                  fill=WOOD, outline=KEY, width=3)
        tip = (xb if sgn > 0 else xa)
        d.polygon([(tip - sgn * 26, y), (tip + sgn * 6, y + 20), (tip - sgn * 26, y + 40)],
                  fill=WOOD if sgn > 0 else WOOD, outline=KEY, width=3)
        d.rectangle([xa + 4, y + 3, xb - 4, y + 12], fill=WOOD_LIT)
        for i in range(3):       # unreadable painted lettering
            ly = y + 14 + (i % 2) * 11
            lx0 = xa + 12 + i * 26
            d.line([(lx0, ly), (lx0 + 18, ly)], fill=PAPER_DK, width=4)
    # a lantern hung off the post
    d.line([(cx - 2, GROUND - 200), (cx + 34, GROUND - 194)], fill=IRON_DARK, width=5)
    d.rectangle([cx + 24, GROUND - 192, cx + 52, GROUND - 156], fill=(58, 54, 62), outline=KEY, width=3)
    d.rectangle([cx + 30, GROUND - 186, cx + 46, GROUND - 162], fill=FLAME)
    glow(img, cx + 38, GROUND - 174, 62, (200, 150, 62), 130)
    return img

PROPS = {
    "chest":          (p_chest,   dict(open_lid=False)),
    "figure":         (p_figure,  {}),
    "signpost":       (p_signpost, {}),
    "chest_open":     (p_chest,   dict(open_lid=True)),
    "barrel":         (p_barrel,  {}),
    "door":           (p_door,    dict(open_door=False)),
    "door_open":      (p_door,    dict(open_door=True)),
    "stairs_up":      (p_stairs,  dict(down=False)),
    "stairs_down":    (p_stairs,  dict(down=True)),
    "brazier":        (p_brazier, dict(lit=True)),
    "brazier_cold":   (p_brazier, dict(lit=False)),
    "campfire":       (p_campfire, {}),
    "board":          (p_board,   {}),
    "shrine":         (p_shrine,  {}),
    "banner":         (p_banner,  {}),
    "rift":           (p_rift,    {}),
    "wall":           (p_wall,    {}),
    "crowd":          (p_crowd,   {}),
    "market":         (p_market,  {}),
    "debris":         (p_debris,  {}),
    "battle":         (p_battle,  {}),
    "chairs":         (p_chairs,  {}),
    "corpse":         (p_corpse,  {}),
    "statue":         (p_statue,  {}),
    "ledger":         (p_ledger,  {}),
    "bones":          (p_bones,   {}),
}

# ---- finishing ----------------------------------------------------------------

def keyline(img, width=3, color=(12, 10, 16, 210)):
    """Dilate the alpha and lay a dark silhouette underneath, so the prop reads
    against any floor texture instead of dissolving into it."""
    a = img.getchannel("A")
    grown = a.filter(ImageFilter.MaxFilter(width * 2 + 1))
    out = Image.new("RGBA", img.size, (0, 0, 0, 0))
    out.paste(Image.new("RGBA", img.size, color), (0, 0), grown)
    out.alpha_composite(img)
    return out

def crop_to_art(img, pad=2):
    bb = img.getchannel("A").getbbox()
    if bb is None:
        return img
    x0, y0, x1, y1 = bb
    x0, y0 = max(0, x0 - pad), max(0, y0 - pad)
    x1, y1 = min(img.width, x1 + pad), min(img.height, y1 + pad)
    # keep the art horizontally centred so the bottom-centre pivot lands under it
    cx = img.width / 2.0
    half = max(cx - x0, x1 - cx)
    return img.crop((int(cx - half), y0, int(cx + half), y1))

META = """fileFormatVersion: 2
guid: {guid}
TextureImporter:
  internalIDToNameTable: []
  externalObjects: {{}}
  serializedVersion: 13
  mipmaps:
    mipMapMode: 0
    enableMipMap: 0
    sRGBTexture: 1
  isReadable: 0
  textureFormat: 1
  maxTextureSize: 512
  textureSettings:
    serializedVersion: 2
    filterMode: 1
    aniso: 1
    mipBias: 0
    wrapU: 1
    wrapV: 1
  nPOTScale: 0
  spriteMode: 1
  spriteExtrude: 1
  spriteMeshType: 0
  alignment: 7
  spritePivot: {{x: 0.5, y: 0}}
  spritePixelsToUnits: 100
  spriteBorder: {{x: 0, y: 0, z: 0, w: 0}}
  spriteGenerateFallbackPhysicsShape: 0
  alphaUsage: 1
  alphaIsTransparency: 1
  spriteTessellationDetail: -1
  textureType: 8
  textureShape: 1
  userData:\x20
  assetBundleName:\x20
  assetBundleVariant:\x20
"""

def main():
    os.makedirs(OUT, exist_ok=True)
    total = 0
    for name, (fn, kw) in sorted(PROPS.items()):
        img = crop_to_art(keyline(fn(**kw)))
        path = os.path.join(OUT, name + ".png")
        img.save(path, optimize=True)
        rel = "Assets/Resources/Props/" + name + ".png"
        with open(path + ".meta", "w") as f:
            f.write(META.format(guid=hashlib.md5(rel.encode()).hexdigest()))
        total += os.path.getsize(path)
    print(f"Painted {len(PROPS)} world props ({total // 1024} KB) into Assets/Resources/Props/")

if __name__ == "__main__":
    main()
