#!/usr/bin/env python3
"""
make-painted-maps.py  —  v2, the BG2/IE way (as far as code can take it).

NOT a flat diamond grid. This builds areas the way an Infinity-Engine backdrop reads:
 - continuous MATERIALS (wooden planks, cut stone, cobble, water) painted as real surfaces,
   not solid-colour tiles;
 - real ARCHITECTURE with thickness — extruded walls (lit face / shade face / top cap),
   the IE "cut-away" interior so you see into rooms;
 - ONE consistent light direction with soft CAST SHADOWS;
 - warm sconce/fire lighting, then a painterly overpaint to unify.

Honest limit: real IE areas are 3-D modelled and hand-overpainted; this is procedural code,
so it approximates the *look*, not the fidelity. Original / CC0 — no AI art, no scraped tiles.

  python3 tools/make-painted-maps.py     ->  play/maps/*.png   (Pillow only)
"""
import os, math, random
from PIL import Image, ImageDraw, ImageFilter, ImageEnhance, ImageFont

ROOT = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
OUT = os.path.join(ROOT, "play", "maps"); os.makedirs(OUT, exist_ok=True)
W, H = 1280, 800
TW, TH = 108, 54          # iso tile (2:1) — large, so an area fills the frame
WALLH = 168               # wall height in px (verticals are NOT foreshortened, IE-style)
LIGHT = (-0.55, -0.83)    # light comes from upper-left
FONTS = "/mnt/skills/examples/canvas-design/canvas-fonts"
DEJA = "/usr/share/fonts/truetype/dejavu"

def _font(sz, it=False):
    for p in ([f"{FONTS}/IBMPlexSerif-Italic.ttf"] if it else [f"{FONTS}/IBMPlexSerif-Regular.ttf"]) + \
             [f"{DEJA}/DejaVuSerif{'-Italic' if it else ''}.ttf"]:
        try: return ImageFont.truetype(p, sz)
        except Exception: pass
    return ImageFont.load_default()

def clamp(v): return max(0, min(255, int(v)))
def mul(c, f): return tuple(clamp(c[i] * f) for i in range(3))
def mix(a, b, t): return tuple(clamp(a[i] + (b[i] - a[i]) * t) for i in range(3))

# ---- iso projection (origin set per-scene) -------------------------------
OX, OY = W // 2, 150
def iso(gx, gy):
    return (OX + (gx - gy) * TW / 2, OY + (gx + gy) * TH / 2)

def quad(d, pts, color):
    d.polygon(pts, fill=color)

# ---- materials: continuous textured surfaces ------------------------------
def plank_floor(base, x0, y0, x1, y1, base_col=(96, 70, 44), seed=1):
    """Wooden plank floor over the grid rect [x0,x1]×[y0,y1]; boards run along +x."""
    d = ImageDraw.Draw(base, "RGBA")
    rnd = random.Random(seed)
    for gy in range(y0, y1):
        # one board strip = a full-width parallelogram, slightly varied tone
        tone = mul(base_col, rnd.uniform(0.82, 1.12))
        p = [iso(x0, gy), iso(x1, gy), iso(x1, gy + 1), iso(x0, gy + 1)]
        quad(d, p, tone + (255,))
        # plank seam (dark) along the far edge, faint light on near edge
        d.line([iso(x0, gy), iso(x1, gy)], fill=(20, 12, 6, 150), width=2)
        d.line([iso(x0, gy + 1), iso(x1, gy + 1)], fill=(150, 120, 80, 36), width=1)
        # board butt-joints + grain ticks
        gx = x0 + rnd.uniform(0, 1.4)
        while gx < x1:
            a, b = iso(gx, gy), iso(gx, gy + 1)
            d.line([a, b], fill=(24, 14, 8, 110), width=1)
            gx += rnd.uniform(1.3, 2.6)
        for _ in range(int((x1 - x0) * 3)):
            ggx = rnd.uniform(x0, x1); ggy = gy + rnd.uniform(0.15, 0.85)
            sx, sy = iso(ggx, ggy)
            d.line([(sx, sy), (sx + rnd.uniform(6, 16), sy)], fill=(40, 26, 14, 50), width=1)

def stone_floor(base, x0, y0, x1, y1, base_col=(86, 84, 78), seed=2):
    """Irregular cut-stone / cobble floor — many individual stones with mortar + light."""
    d = ImageDraw.Draw(base, "RGBA")
    rnd = random.Random(seed)
    # base wash
    quad(d, [iso(x0, y0), iso(x1, y0), iso(x1, y1), iso(x0, y1)], mul(base_col, 0.7) + (255,))
    gy = y0
    while gy < y1:
        gx = x0
        row_h = rnd.uniform(0.5, 0.7)
        while gx < x1:
            cw = rnd.uniform(0.55, 0.95)
            cx, cy = gx + cw / 2, gy + row_h / 2
            tone = mul(base_col, rnd.uniform(0.78, 1.18))
            pts = [iso(gx + 0.04, gy + 0.04), iso(gx + cw - 0.04, gy + 0.05),
                   iso(gx + cw - 0.05, gy + row_h - 0.04), iso(gx + 0.05, gy + row_h - 0.05)]
            quad(d, pts, tone + (255,))
            # lit top edge, shadow bottom edge (light from upper-left)
            d.line([pts[0], pts[1]], fill=(220, 215, 200, 50), width=1)
            d.line([pts[2], pts[3]], fill=(10, 10, 12, 90), width=1)
            gx += cw
        gy += row_h

def water(base, x0, y0, x1, y1, seed=3):
    d = ImageDraw.Draw(base, "RGBA")
    rnd = random.Random(seed)
    for gy in range(int(y0 * 2), int(y1 * 2)):
        gyy = gy / 2
        sh = 14 + int(12 * math.sin(gyy * 1.3 + seed))
        col = (18 + sh // 2, 40 + sh, 60 + sh)
        p = [iso(x0, gyy), iso(x1, gyy), iso(x1, gyy + 0.5), iso(x0, gyy + 0.5)]
        quad(d, p, col + (255,))
    # specular flecks
    for _ in range(220):
        gx, gy = rnd.uniform(x0, x1), rnd.uniform(y0, y1)
        sx, sy = iso(gx, gy)
        d.line([(sx, sy), (sx + rnd.uniform(5, 14), sy)], fill=(150, 180, 200, rnd.randint(20, 60)), width=1)

# ---- cast shadows (soft, on their own layer, light upper-left) ------------
def shadow_layer():
    return Image.new("RGBA", (W, H), (0, 0, 0, 0))

def cast(shadowimg, pts, dx=26, dy=14, strength=120):
    d = ImageDraw.Draw(shadowimg, "RGBA")
    d.polygon([(p[0] + dx, p[1] + dy) for p in pts], fill=(0, 0, 0, strength))

# ---- architecture: extruded stone walls with thickness & block texture ----
def wall_run(base, shadowimg, a, b, h=WALLH, col=(96, 96, 104), thick=0.18, lit=True, blocks=True):
    """A stone wall from grid point a to grid point b, extruded up by h px."""
    d = ImageDraw.Draw(base, "RGBA")
    ax, ay = iso(*a); bx, by = iso(*b)
    # face normal lighting: how aligned the run is with the light
    dirx, diry = (b[0] - a[0]), (b[1] - a[1])
    L = math.hypot(dirx, diry) or 1
    nx, ny = -diry / L, dirx / L
    facing = nx * LIGHT[0] + ny * LIGHT[1]
    base_f = 1.06 if facing > 0 else 0.74
    if not lit: base_f *= 0.9
    face = mul(col, base_f)
    # cast shadow on ground first (under everything) — footprint pushed by light
    cast(shadowimg, [(ax, ay), (bx, by), (bx, by + 6), (ax, ay + 6)], dx=20, dy=12, strength=70)
    # the wall face (a→b, up by h)
    quad(d, [(ax, ay), (bx, by), (bx, by - h), (ax, ay - h)], face + (255,))
    # top cap (a little depth)
    cap = mul(col, 1.18 if facing > 0 else 0.9)
    quad(d, [(ax, ay - h), (bx, by - h), (bx + 4, by - h - 4), (ax + 4, ay - h - 4)], cap + (255,))
    # stone block courses
    if blocks:
        rnd = random.Random(int(ax + by) % 9999)
        courses = max(3, h // 18)
        for c in range(courses + 1):
            t = c / courses
            yA = (ay - h * (1 - t)); yB = (by - h * (1 - t))
            d.line([(ax, yA), (bx, yB)], fill=(0, 0, 0, 60), width=1)
        # vertical joints, brick-staggered
        n = int(L * 3)
        for i in range(1, n):
            for c in range(courses):
                off = (0.5 if c % 2 else 0.0)
                tt = (i + off) / n
                if tt >= 1: continue
                px = ax + (bx - ax) * tt; pyb = ay + (by - ay) * tt
                y1 = pyb - h * (c / courses); y2 = pyb - h * ((c + 1) / courses)
                d.line([(px, y1), (px, y2)], fill=(0, 0, 0, 45), width=1)
        # subtle weathering blotches
        for _ in range(int(L * 6)):
            tt = rnd.random(); hh = rnd.random()
            px = ax + (bx - ax) * tt; py = (ay + (by - ay) * tt) - h * hh
            r = rnd.uniform(3, 9)
            d.ellipse([px - r, py - r * 0.6, px + r, py + r * 0.6],
                      fill=(mul(col, 0.85) if rnd.random() < .5 else mul(col, 1.1)) + (40,))

def pitched_roof(base, shadowimg, cx, cy, gw, gd, h=70, col=(120, 70, 52)):
    """A simple pitched roof block for an exterior building."""
    d = ImageDraw.Draw(base, "RGBA")
    bl = iso(cx - gw, cy + gd); br = iso(cx + gw, cy + gd)
    tl = iso(cx - gw, cy - gd); tr = iso(cx + gw, cy - gd)
    ridgeL = (tl[0], tl[1] - h - 24); ridgeR = (tr[0], tr[1] - h - 24)
    # two roof faces
    quad(d, [(bl[0], bl[1] - WALLH), (br[0], br[1] - WALLH), ridgeR, ridgeL], mul(col, 1.05) + (255,))
    quad(d, [(tl[0], tl[1] - WALLH), (tr[0], tr[1] - WALLH), ridgeR, ridgeL], mul(col, 0.72) + (255,))
    # eaves line
    d.line([(bl[0], bl[1] - WALLH), (br[0], br[1] - WALLH)], fill=(20, 12, 8, 160), width=2)

# ---- furniture / props ----------------------------------------------------
def box(base, shadowimg, gx, gy, gw, gd, h, top, lit, dark, shadow=True):
    d = ImageDraw.Draw(base, "RGBA")
    bl = iso(gx - gw, gy + gd); br = iso(gx + gw, gy + gd)
    fl = iso(gx - gw, gy - gd); fr = iso(gx + gw, gy - gd)
    if shadow:
        cast(shadowimg, [bl, br, fr, fl], dx=16, dy=9, strength=70)
    # left & right faces
    quad(d, [fl, (fl[0], fl[1] - h), (bl[0], bl[1] - h), bl], mul(dark, 1) + (255,))
    quad(d, [br, (br[0], br[1] - h), (fr[0], fr[1] - h), fr], mul(lit, 1) + (255,))
    # top
    quad(d, [(fl[0], fl[1] - h), (fr[0], fr[1] - h), (br[0], br[1] - h), (bl[0], bl[1] - h)], top + (255,))
    return (fl, fr, br, bl)

def rug(base, gx, gy, gw, gd, col=(120, 40, 44)):
    d = ImageDraw.Draw(base, "RGBA")
    pts = [iso(gx - gw, gy), iso(gx, gy - gd), iso(gx + gw, gy), iso(gx, gy + gd)]
    quad(d, pts, col + (235,))
    inner = [iso(gx - gw + .5, gy), iso(gx, gy - gd + .25), iso(gx + gw - .5, gy), iso(gx, gy + gd - .25)]
    d.polygon(inner, outline=(220, 190, 120, 180), width=2)
    d.polygon(pts, outline=(30, 12, 14, 200), width=2)

def bed(base, shadowimg, gx, gy):
    box(base, shadowimg, gx, gy, 0.5, 0.85, 16, (70, 54, 36), (74, 58, 38), (50, 38, 24))
    d = ImageDraw.Draw(base, "RGBA")
    # mattress + blanket fold + pillow
    top = [iso(gx - .42, gy - .78), iso(gx + .42, gy - .78), iso(gx + .42, gy + .78), iso(gx - .42, gy + .78)]
    top = [(p[0], p[1] - 18) for p in top]
    quad(d, top, (120, 120, 134, 255))
    bl = [(iso(gx - .42, gy + .1)[0], iso(gx - .42, gy + .1)[1] - 18),
          (iso(gx + .42, gy + .1)[0], iso(gx + .42, gy + .1)[1] - 18),
          (iso(gx + .42, gy + .78)[0], iso(gx + .42, gy + .78)[1] - 18),
          (iso(gx - .42, gy + .78)[0], iso(gx - .42, gy + .78)[1] - 18)]
    quad(d, bl, (74, 92, 110, 255))
    pil = [(iso(gx - .34, gy - .74)[0], iso(gx - .34, gy - .74)[1] - 20),
           (iso(gx + .34, gy - .74)[0], iso(gx + .34, gy - .74)[1] - 20),
           (iso(gx + .34, gy - .46)[0], iso(gx + .34, gy - .46)[1] - 20),
           (iso(gx - .34, gy - .46)[0], iso(gx - .34, gy - .46)[1] - 20)]
    quad(d, pil, (208, 204, 196, 255))

def table(base, shadowimg, gx, gy, gw=0.55, gd=0.42):
    box(base, shadowimg, gx, gy, gw, gd, 30, (110, 80, 48), (96, 68, 40), (66, 46, 26))

def barrel(base, shadowimg, gx, gy):
    d = ImageDraw.Draw(base, "RGBA")
    x, y = iso(gx, gy)
    cast(shadowimg, [(x - 12, y), (x + 12, y), (x + 12, y + 6), (x - 12, y + 6)], dx=12, dy=7, strength=60)
    d.ellipse([x - 12, y - 6, x + 12, y + 6], fill=(70, 50, 30, 255))
    d.rectangle([x - 12, y - 30, x + 12, y], fill=(96, 70, 42, 255))
    d.ellipse([x - 12, y - 36, x + 12, y - 24], fill=(120, 90, 56, 255))
    for yy in (y - 28, y - 12):
        d.line([(x - 12, yy), (x + 12, yy)], fill=(50, 34, 18, 220), width=2)

def fireplace(base, shadowimg, glowlayer, gx, gy):
    d = ImageDraw.Draw(base, "RGBA")
    box(base, shadowimg, gx, gy, 0.7, 0.3, 92, (78, 78, 86), (84, 84, 92), (54, 54, 62))
    x, y = iso(gx, gy)
    d.rectangle([x - 22, y - 78, x + 22, y - 30], fill=(14, 10, 10, 255))  # hearth opening
    gl = ImageDraw.Draw(glowlayer, "RGBA")
    for i in range(8, 0, -1):
        a = int(150 * (1 - i / 8) ** 1.6); r = 30 * i / 8
        gl.ellipse([x - r, y - 54 - r * .6, x + r, y - 54 + r * .6], fill=(255, 150, 60, a))
    d.polygon([(x - 12, y - 32), (x, y - 64), (x + 12, y - 32)], fill=(255, 170, 70, 235))
    d.polygon([(x - 6, y - 34), (x, y - 54), (x + 6, y - 34)], fill=(255, 226, 150, 240))

def sconce(base, glowlayer, x, y, r=70, color=(255, 180, 80)):
    gl = ImageDraw.Draw(glowlayer, "RGBA")
    for i in range(10, 0, -1):
        a = int(150 * (1 - i / 10) ** 1.7); rr = r * i / 10
        gl.ellipse([x - rr, y - rr, x + rr, y + rr], fill=(color[0], color[1], color[2], a))
    ImageDraw.Draw(base, "RGBA").ellipse([x - 3, y - 4, x + 3, y + 4], fill=(255, 232, 160, 255))

# ---- atmosphere & finishing ----------------------------------------------
def ambient_occlusion(base, x0, y0, x1, y1, depth=70):
    """Darken floor near the back walls a touch — fake AO for grounding."""
    ao = Image.new("RGBA", (W, H), (0, 0, 0, 0)); d = ImageDraw.Draw(ao)
    for t in range(depth):
        a = int(46 * (1 - t / depth))
        gy = y0 + t / depth
        d.line([iso(x0, gy), iso(x1, gy)], fill=(0, 0, 0, a), width=3)
    base.alpha_composite(ao.filter(ImageFilter.GaussianBlur(6)))

def grain(base, amt=10):
    n = Image.effect_noise((W, H), 30)
    g = Image.new("RGBA", (W, H), (0, 0, 0, 0))
    g.putalpha(n.point(lambda v: int(abs(v - 128) / 128 * amt)))
    sol = Image.new("RGBA", (W, H), (255, 255, 255, 255)); sol.putalpha(g.getchannel("A"))
    base.alpha_composite(sol)

def radial(w, h, cx, cy, r, inner, outer):
    img = Image.new("RGBA", (w, h), (0, 0, 0, 0)); d = ImageDraw.Draw(img)
    for i in range(24, 0, -1):
        t = i / 24; rr = r * t
        col = tuple(int(outer[j] + (inner[j] - outer[j]) * (1 - t)) for j in range(4))
        d.ellipse([cx - rr, cy - rr * .62, cx + rr, cy + rr * .62], fill=col)
    return img.filter(ImageFilter.GaussianBlur(r * .05 + 2))

def vignette(strength=150):
    m = Image.new("L", (W, H), 0)
    ImageDraw.Draw(m).ellipse([int(W * .07), int(H * .04), int(W * .93), int(H * 1.0)], fill=255)
    m = m.filter(ImageFilter.GaussianBlur(150))
    inv = m.point(lambda v: int((255 - v) / 255 * strength))
    out = Image.new("RGBA", (W, H), (6, 5, 9, 255)); out.putalpha(inv); return out

def painted(img):
    img = img.convert("RGB")
    img = img.filter(ImageFilter.GaussianBlur(0.6))
    img = img.filter(ImageFilter.UnsharpMask(3, 110, 2))
    img = img.filter(ImageFilter.SMOOTH_MORE)
    img = img.filter(ImageFilter.UnsharpMask(2, 70, 1))
    img = ImageEnhance.Color(img).enhance(1.10)
    img = ImageEnhance.Contrast(img).enhance(1.05)
    return img

def title(img, t, s):
    d = ImageDraw.Draw(img, "RGBA")
    d.rectangle([0, 0, W, H], outline=(18, 14, 10, 255), width=10)
    d.rectangle([6, 6, W - 6, H - 6], outline=(120, 96, 50, 80), width=2)
    f1, f2 = _font(28), _font(15, True)
    w = max(d.textlength(t, font=f1), d.textlength(s, font=f2))
    d.rectangle([26, H - 80, 26 + w + 34, H - 24], fill=(8, 7, 11, 170))
    d.text((42, H - 74), t, font=f1, fill=(231, 200, 115, 255))
    d.text((43, H - 40), s, font=f2, fill=(176, 168, 150, 255))

def save(img, name):
    painted(img).save(os.path.join(OUT, name + ".png"))
    print(f"  painted play/maps/{name}.png")

# =========================================================================
# SCENE 1 — a BG2-style interior: a house in the Lamplit Quarter (cut-away)
# =========================================================================
def house():
    global OX, OY
    x0, y0, x1, y1 = 0, 0, 11, 9
    # centre the footprint: middle grid point maps to frame centre-ish
    midx, midy = (x0 + x1) / 2, (y0 + y1) / 2
    OX = W // 2 - (midx - midy) * TW / 2
    OY = 300 - (midx + midy) * TH / 2 + WALLH * 0.45
    img = Image.new("RGBA", (W, H), (10, 9, 13, 255))
    sh = shadow_layer(); glow = Image.new("RGBA", (W, H), (0, 0, 0, 0))
    # floors: parlour planks (left), stone entry (front), bedrooms planks (right)
    plank_floor(img, x0, y0, 6, 5, (100, 74, 47), seed=4)
    stone_floor(img, x0, 5, 6, y1, (94, 90, 82), seed=7)
    plank_floor(img, 6, y0, x1, y1, (92, 68, 43), seed=9)
    ambient_occlusion(img, x0, y0, x1, y1, depth=110)
    # back walls (top-left & top-right) — IE cut-away: near walls omitted, we see in
    wall_run(img, sh, (x0, y0), (x1, y0))      # back-right wall (long)
    wall_run(img, sh, (x0, y0), (x0, y1))      # back-left wall (long)
    # interior partitions making rooms (lower, so we read over them)
    wall_run(img, sh, (6, y0), (6, 5.0), h=96, col=(108, 104, 100))   # vertical divider
    wall_run(img, sh, (6, 3.0), (x1, 3.0), h=96, col=(108, 104, 100)) # bedrooms split
    wall_run(img, sh, (6, 5.0), (x1, 5.0), h=96, col=(108, 104, 100)) # corridor wall
    img.alpha_composite(sh.filter(ImageFilter.GaussianBlur(6)))       # shadows under props
    # --- parlour (left) ---
    rug(img, 2.8, 2.6, 1.9, 1.25, (124, 44, 46))
    fireplace(img, sh, glow, 0.55, 1.4)
    table(img, sh, 2.8, 2.6, .62, .46)
    for cx in (1.7, 3.9):
        box(img, sh, cx, 2.6, .2, .2, 26, (62, 46, 30), (66, 50, 32), (44, 32, 20))   # chairs
    box(img, sh, 0.7, 4.0, .42, .5, 132, (70, 52, 34), (74, 56, 36), (48, 36, 22))    # tall dresser
    box(img, sh, 4.6, 0.7, .45, .35, 150, (66, 50, 32), (70, 54, 34), (44, 34, 22))   # bookshelf
    # --- bedrooms (right, two of them) ---
    bed(img, sh, 8.0, 1.4); box(img, sh, 9.6, 1.3, .35, .3, 30, (84, 64, 40), (88, 68, 42), (56, 42, 26))  # bed + nightstand
    bed(img, sh, 8.0, 4.2); box(img, sh, 9.6, 4.2, .35, .3, 30, (84, 64, 40), (88, 68, 42), (56, 42, 26))
    table(img, sh, 8.4, 7.0, .6, .45)
    box(img, sh, 9.8, 7.2, .2, .2, 26, (62, 46, 30), (66, 50, 32), (44, 32, 20))
    # --- entry / storage (front-left, stone) ---
    barrel(img, sh, 1.0, 6.6); barrel(img, sh, 1.7, 6.9); barrel(img, sh, 1.2, 7.6)
    box(img, sh, 3.4, 7.2, .5, .5, 40, (96, 70, 42), (100, 74, 44), (66, 48, 28))     # crate stack
    box(img, sh, 4.2, 7.6, .4, .4, 30, (96, 70, 42), (100, 74, 44), (66, 48, 28))
    # warm light: fire + wall sconces along the back walls
    for (gx, gy) in [(2.4, 0.2), (5.4, 0.2), (8.6, 0.2), (0.2, 2.4), (0.2, 5.6)]:
        x, y = iso(gx, gy); sconce(img, glow, x, y - 96)
    img.alpha_composite(glow.filter(ImageFilter.GaussianBlur(2)))
    # cool moonlight from the open near side, warm interior pool
    amb = Image.new("RGBA", (W, H), (0, 0, 0, 0))
    ImageDraw.Draw(amb).rectangle([0, 0, W, H], fill=(38, 44, 76, 24))
    img.alpha_composite(amb)
    img.alpha_composite(radial(W, H, *iso(3, 2.6), 360, (255, 180, 90, 30), (0, 0, 0, 0)))
    grain(img, 9)
    img.alpha_composite(vignette(150))
    title(img, "A House in the Lamplit Quarter", "interior · Infinity-Engine cut-away — plank floors, cut-stone walls, hearthlight")
    save(img, "house")

if __name__ == "__main__":
    print("Painting IE-style area maps (v2 — materials, architecture, light)…")
    house()
    print("done — see play/maps/")
