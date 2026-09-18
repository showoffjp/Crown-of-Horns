#!/usr/bin/env python3
"""
Painted floor backdrops for every walkable zone — the first realized slice of
docs/AREA_ART_PIPELINE.md's backdrop hook. One 760x470 image per zone, rendered
in EXACTLY the web renderer's projection (TW=64, TH=32, OX=380, OY=64 — see
tools/make-town-market.py) so the canvas can blit it under the live layer and
every prop/token still sits pixel-perfect on its tile.

Deliberately floor-and-light only: props, tokens, hover, path dots, and exits
stay procedural in JS on top (correct depth-sorting stays trivial; the dynamic
BLOCKED overlay can't be baked anyway). What the painting adds is mood:

  - each zone's floor is tinted by its OWN cast — the circular mean of the
    zone's npc hues (the same authored hues that drive portraits and tokens);
  - warm light pools bloom around every candle, torch, brazier, campfire,
    hearthfire and lamppost the zone authors (112 candles across the grey);
    greyshrines and fountains pool cold;
  - exits breathe a faint causeway blue for the JS pulse to sit in;
  - per-tile value jitter, depth falloff toward the far corner, painted grid
    lines, vignette, grain — the flat checkerboard becomes a floor.

Output: play/maps/<scene-id>.jpg (JPEG — no alpha needed; kept non-LFS via
.gitattributes so it commits from LFS-less environments). The web hook loads
them opportunistically and falls back to flat shading when absent, so the
all-in-one build degrades gracefully. Deterministic. Honest placeholders.
Re-run: python3 tools/gen-zone-backdrops.py
"""
import colorsys, glob, hashlib, json, math, os, random
from PIL import Image, ImageDraw, ImageFilter

ROOT = os.path.join(os.path.dirname(__file__), "..")
OUT = os.path.join(ROOT, "play", "maps")

# the web renderer's projection, supersampled 2x for smooth diamond edges
W, H, SS = 760, 470, 2
TW, TH = 64, 32
OX, OY = W // 2, 64

WARM = {"candle": (36, 34), "torch": (32, 58), "brazier": (28, 62), "campfire": (26, 70),
        "hearthfire": (24, 74), "lamppost": (40, 66), "tavern": (30, 46)}
COLD = {"greyshrine": (210, 44), "fountain": (200, 40), "shrine": (220, 40),
        "deathdoor": (255, 42), "cell": (250, 30)}

def hsv(h, s, v):
    r, g, b = colorsys.hsv_to_rgb((h % 360) / 360.0, s, v)
    return (int(r * 255), int(g * 255), int(b * 255))

def jitter(zone, tx, ty, lo, hi):
    x = int(hashlib.md5(f"{zone}:{tx}:{ty}".encode()).hexdigest()[:6], 16) / 0xFFFFFF
    return lo + (hi - lo) * x

def zone_hue(scene):
    """circular mean of the zone's npc hues — the cast IS the mood"""
    hues = [n.get("hue") for n in scene.get("npcs", []) if n.get("hue") is not None]
    if not hues:
        return 258.0
    sx = sum(math.cos(math.radians(h)) for h in hues)
    sy = sum(math.sin(math.radians(h)) for h in hues)
    if abs(sx) < 1e-6 and abs(sy) < 1e-6:
        return 258.0
    return math.degrees(math.atan2(sy, sx)) % 360

def iso(tx, ty):
    return (OX + (tx - ty) * TW // 2) * SS, (OY + (tx + ty) * TH // 2) * SS

def diamond(d, cx, cy, fill, outline=None):
    pts = [(cx, cy - TH * SS // 2), (cx + TW * SS // 2, cy),
           (cx, cy + TH * SS // 2), (cx - TW * SS // 2, cy)]
    d.polygon(pts, fill=fill, outline=outline)

def glow(img, cx, cy, r, color, alpha):
    """squashed radial pool of light on the floor plane (2:1 iso ellipse)"""
    mask = Image.new("L", img.size, 0)
    d = ImageDraw.Draw(mask)
    for i in range(r, 0, -3):
        a = int(alpha * (1 - i / r) ** 2)
        d.ellipse([cx - i, cy - i * 0.55, cx + i, cy + i * 0.55], fill=a)
    layer = Image.new("RGB", img.size, color)
    img.paste(layer, (0, 0), mask)

def paint(scene):
    zid = scene.get("id", "zone")
    zw, zh = scene.get("w", 10), scene.get("h", 10)
    hue = zone_hue(scene)
    base_v = 0.165

    img = Image.new("RGB", (W * SS, H * SS), hsv(hue, 0.30, 0.055))
    d = ImageDraw.Draw(img)
    # horizon breath behind the grid's top corner
    glow(img, OX * SS, int(OY * 0.9) * SS, 340 * SS, hsv(hue, 0.38, 0.34), 78)

    # floor: parity checker kept (the game reads by it), per-tile jitter,
    # value falling off toward the far (bottom) corner like the key light says
    depth_span = max(1, zw + zh - 2)
    for ty in range(zh):
        for tx in range(zw):
            cx, cy = iso(tx, ty)
            parity = 1.0 if (tx + ty) % 2 else 0.82
            depth = 1.0 - 0.20 * ((tx + ty) / depth_span)
            v = base_v * parity * depth * jitter(zid, tx, ty, 0.88, 1.12)
            s = 0.22 * jitter(zid, ty, tx, 0.85, 1.15)
            diamond(d, cx, cy, hsv(hue, s, v), hsv(hue, 0.30, v * 0.55))

    # authored light sources pool onto the floor (warm ones get a hot core)
    for p in scene.get("props", []):
        t = p.get("type", "")
        pool = WARM.get(t) or COLD.get(t)
        if not pool:
            continue
        ph, pr = pool
        cx, cy = iso(p.get("x", 0), p.get("y", 0))
        warm = t in WARM
        glow(img, cx, cy, int(pr * 1.25) * SS, hsv(ph, 0.55 if warm else 0.30, 0.58), 112 if warm else 84)
        if warm:
            glow(img, cx, cy, int(pr * 0.45) * SS, hsv(ph - 8, 0.42, 0.85), 120)
    # exits breathe causeway blue under the JS pulse
    for x in scene.get("exits", []):
        cx, cy = iso(x.get("x", 0), x.get("y", 0))
        glow(img, cx, cy, 46 * SS, (110, 168, 200), 70)

    # water zones (reeds/pilings/boats): the void beyond the grid becomes still
    # water — horizontal sheen bands + a vertical mirror-smear under each light
    types = {p.get("type") for p in scene.get("props", [])}
    if types & {"reeds", "piling", "boat"}:
        wd = ImageDraw.Draw(img, "RGBA")
        rw = random.Random(int(hashlib.md5((zid + ":water").encode()).hexdigest(), 16))
        for _ in range(90):
            yy = rw.randint(int(H * 0.10) * SS, (H - 8) * SS)
            xx = rw.randint(0, W * SS); ln = rw.randint(20, 90) * SS
            wd.line([(xx, yy), (xx + ln, yy)], fill=(150, 190, 210, rw.randint(4, 12)), width=SS)
        for p in scene.get("props", []):
            if p.get("type") not in WARM: continue
            ph, _pr = WARM[p["type"]]
            cx, cy = iso(p.get("x", 0), p.get("y", 0))
            col = hsv(ph, 0.5, 0.5)
            for k in range(10):
                a = max(4, 26 - k * 2)
                wd.line([(cx - SS, cy + (8 + k * 7) * SS), (cx + SS, cy + (8 + k * 7) * SS)],
                        fill=(*col, a), width=3 * SS)

    # painterly finish. Grain is seeded per zone id — PIL's effect_noise pulls
    # from an unseeded process-global rand(), which makes output depend on
    # paint order and libc; md5-seeded randbytes is stable everywhere.
    img = img.resize((W, H), Image.LANCZOS)
    img = Image.blend(img, img.filter(ImageFilter.GaussianBlur(1.4)), 0.30)
    rnd = random.Random(int(hashlib.md5(zid.encode()).hexdigest(), 16))
    noise = Image.frombytes("L", (W, H), rnd.randbytes(W * H)).point(lambda v: 255 - (255 - v) // 16)
    img = Image.composite(img, Image.new("RGB", (W, H), (0, 0, 0)), noise)
    vmask = Image.new("L", (W, H), 0)
    ImageDraw.Draw(vmask).ellipse([-W * 0.30, -H * 0.35, W * 1.30, H * 1.35], fill=255)
    vmask = vmask.filter(ImageFilter.GaussianBlur(70))
    img = Image.composite(img, Image.new("RGB", (W, H), hsv(hue, 0.30, 0.045)), vmask)
    return img

def main():
    os.makedirs(OUT, exist_ok=True)
    n, total = 0, 0
    for p in sorted(glob.glob(os.path.join(ROOT, "play", "*.json"))):
        try:
            z = json.load(open(p))
        except Exception:
            continue
        if not isinstance(z, dict) or not isinstance(z.get("scene"), dict):
            continue
        scene = z["scene"]
        img = paint(scene)
        out = os.path.join(OUT, scene.get("id", "zone") + ".jpg")
        img.save(out, quality=80, optimize=True)
        total += os.path.getsize(out)
        n += 1
    print(f"Painted {n} zone floor backdrops ({total // 1024} KB) into play/maps/")

if __name__ == "__main__":
    main()
