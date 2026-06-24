#!/usr/bin/env python3
"""
make-painted-maps.py — pre-rendered, hand-painted-style AREA MAPS in the lineage of the
Infinity Engine (Baldur's Gate, Icewind Dale, Planescape: Torment): moody, atmospheric,
2.5-D painted backdrops for the walkable zones of Crown of Horns.

Everything here is ORIGINAL, procedural, CC0 — no AI art, no scraped tiles. The painterly
look is built the IE way in spirit: layered light, soft shadow, mottled texture, fog, bloom,
and a heavy vignette, then a final "painted" pass (blur + unsharp + colour grade).

  python3 tools/make-painted-maps.py        # renders play/maps/*.png

Pillow only (no numpy). Gradients are rendered tiny and scaled up; texture is effect_noise;
glows are concentric ellipses, blurred.
"""
import os, math, random
from PIL import Image, ImageDraw, ImageFilter, ImageChops, ImageEnhance, ImageFont

ROOT = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
OUT = os.path.join(ROOT, "play", "maps")
os.makedirs(OUT, exist_ok=True)
W, H = 1280, 800
FONTS = "/mnt/skills/examples/canvas-design/canvas-fonts"
DEJA = "/usr/share/fonts/truetype/dejavu"

def _font(sz, italic=False):
    for p in ([f"{FONTS}/IBMPlexSerif-Italic.ttf"] if italic else [f"{FONTS}/IBMPlexSerif-Regular.ttf"]) + \
             [f"{DEJA}/DejaVuSerif-Italic.ttf" if italic else f"{DEJA}/DejaVuSerif.ttf"]:
        try: return ImageFont.truetype(p, sz)
        except Exception: pass
    return ImageFont.load_default()

# ---------------------------------------------------------------- helpers
def vgrad(w, h, top, bottom):
    """Vertical gradient, rendered 1×h then scaled — smooth and cheap."""
    strip = Image.new("RGB", (1, h))
    for y in range(h):
        t = y / max(1, h - 1)
        strip.putpixel((0, y), tuple(int(top[i] + (bottom[i] - top[i]) * t) for i in range(3)))
    return strip.resize((w, h))

def radial(w, h, cx, cy, r, inner, outer):
    """Soft radial fill (RGBA), drawn as concentric ellipses then blurred."""
    img = Image.new("RGBA", (w, h), (0, 0, 0, 0))
    d = ImageDraw.Draw(img)
    steps = 26
    for i in range(steps, 0, -1):
        t = i / steps
        rr = r * t
        col = tuple(int(outer[j] + (inner[j] - outer[j]) * (1 - t)) for j in range(4))
        d.ellipse([cx - rr, cy - rr * 0.62, cx + rr, cy + rr * 0.62], fill=col)
    return img.filter(ImageFilter.GaussianBlur(r * 0.05 + 2))

def glow(w, h, cx, cy, r, color, strength=210):
    img = Image.new("RGBA", (w, h), (0, 0, 0, 0))
    d = ImageDraw.Draw(img)
    steps = 22
    for i in range(steps, 0, -1):
        t = i / steps
        a = int(strength * (1 - t) ** 1.8)
        rr = r * t
        d.ellipse([cx - rr, cy - rr, cx + rr, cy + rr], fill=(color[0], color[1], color[2], a))
    return img.filter(ImageFilter.GaussianBlur(r * 0.08 + 3))

def noise_tex(w, h, sigma=44, tint=(255, 255, 255), alpha=26, blur=1.4, scale=2):
    """Painterly mottle: low-res gaussian noise, tinted, scaled up for soft blobs."""
    n = Image.effect_noise((max(2, w // scale), max(2, h // scale)), sigma).resize((w, h))
    n = n.filter(ImageFilter.GaussianBlur(blur))
    out = Image.new("RGBA", (w, h), (0, 0, 0, 0))
    out.putalpha(n.point(lambda v: int(abs(v - 128) / 128 * alpha)))
    solid = Image.new("RGBA", (w, h), (tint[0], tint[1], tint[2], 255))
    solid.putalpha(out.getchannel("A"))
    return solid

def vignette(w, h, strength=160):
    # bright centre ellipse on black, blurred, inverted → dark soft edges
    m = Image.new("L", (w, h), 0)
    md = ImageDraw.Draw(m)
    md.ellipse([int(w * 0.10), int(h * 0.06), int(w * 0.90), int(h * 0.98)], fill=255)
    m = m.filter(ImageFilter.GaussianBlur(140))
    inv = m.point(lambda v: int((255 - v) / 255 * strength))
    out = Image.new("RGBA", (w, h), (6, 5, 10, 255))
    out.putalpha(inv)
    return out

def iso(cx, cy, tx, ty, tw=78, th=39):
    return (cx + (tx - ty) * tw / 2, cy + (tx + ty) * th / 2)

def diamond(d, x, y, tw, th, fill):
    d.polygon([(x, y - th / 2), (x + tw / 2, y), (x, y + th / 2), (x - tw / 2, y)], fill=fill)

def iso_floor(base, cx, cy, cols, rows, pal, tw=78, th=39, jitter=10, seed=7, cracks=True):
    """A painted isometric ground plane — mottled flagstones, depth-shaded, overflowing frame."""
    lay = Image.new("RGBA", base.size, (0, 0, 0, 0))
    d = ImageDraw.Draw(lay)
    rnd = random.Random(seed)
    for r in range(rows):
        for c in range(cols):
            x, y = iso(cx, cy, c, r, tw, th)
            if y < -th or y > base.size[1] + th or x < -tw or x > base.size[0] + tw:
                continue
            depth = (r + c) / (cols + rows)
            shade = rnd.randint(-jitter, jitter)
            col = tuple(max(0, min(255, int(pal[i] * (0.62 + 0.5 * (1 - depth))) + shade)) for i in range(3))
            diamond(d, x, y, tw + 1, th + 1, col + (255,))
            # grout: dark on the down-left edge, faint light on the down-right (painted bevel)
            d.line([(x - tw / 2, y), (x, y + th / 2)], fill=(0, 0, 0, 40))
            d.line([(x, y + th / 2), (x + tw / 2, y)], fill=(255, 250, 235, 16))
            if cracks and rnd.random() < 0.16:  # a few painted cracks/flag-splits
                d.line([(x, y), (x + rnd.randint(-tw // 3, tw // 3), y + rnd.randint(-th // 3, th // 3))],
                       fill=(0, 0, 0, 34), width=1)
    base.alpha_composite(lay)

def back_arch(base, top, band, pal_light, pal_dark, count=9, jitter=40, seed=2, kind="house"):
    """Fill the upper band edge-to-edge with painted architecture so there is no void."""
    d = ImageDraw.Draw(base, "RGBA")
    rnd = random.Random(seed)
    step = W // count
    for i in range(-1, count + 1):
        x = i * step + step // 2 + rnd.randint(-20, 20)
        bw = step + rnd.randint(10, 40)
        bh = band + rnd.randint(-jitter, jitter)
        ybase = top + band
        lit = tuple(max(0, min(255, pal_light[j] + rnd.randint(-12, 12))) for j in range(3))
        drk = tuple(max(0, min(255, pal_dark[j] + rnd.randint(-12, 12))) for j in range(3))
        # facade
        d.rectangle([x - bw // 2, ybase - bh, x + bw // 2, ybase], fill=drk + (255,))
        d.rectangle([x - bw // 2, ybase - bh, x - bw // 2 + bw, ybase - bh + 6], fill=lit + (255,))
        if kind == "house":
            # roofline
            d.polygon([(x - bw // 2 - 6, ybase - bh), (x + bw // 2 + 6, ybase - bh),
                       (x + 6, ybase - bh - 26), (x - 6, ybase - bh - 26)], fill=tuple(int(c * 0.8) for c in drk) + (255,))
            # a warm window or two
            for wy in range(ybase - bh + 18, ybase - 14, 30):
                if rnd.random() < 0.55:
                    wx = x + rnd.randint(-bw // 3, bw // 3)
                    d.rectangle([wx - 5, wy - 7, wx + 5, wy + 7], fill=(255, 196, 96, 230))
                    base.alpha_composite(glow(W, H, wx, wy, 22, (255, 180, 80), 70))

def iso_block(d, x, ybase, w, hgt, top, light, dark, th=29):
    hw = w / 2
    d.polygon([(x - hw, ybase), (x - hw, ybase - hgt), (x, ybase - hgt + th / 4), (x, ybase + th / 4)], fill=dark)
    d.polygon([(x + hw, ybase), (x + hw, ybase - hgt), (x, ybase - hgt + th / 4), (x, ybase + th / 4)], fill=light)
    d.polygon([(x, ybase - hgt - th / 4), (x + hw, ybase - hgt), (x, ybase - hgt + th / 4), (x - hw, ybase - hgt)], fill=top)

def painted_pass(img):
    """The final 'hand-painted' grade: soften, then bite the edges back, warm the light."""
    img = img.convert("RGB")
    img = img.filter(ImageFilter.GaussianBlur(0.7))
    img = img.filter(ImageFilter.UnsharpMask(radius=3, percent=90, threshold=2))
    img = ImageEnhance.Color(img).enhance(1.12)
    img = ImageEnhance.Contrast(img).enhance(1.06)
    # subtle painterly clumping
    img = img.filter(ImageFilter.MedianFilter(3))
    img = img.filter(ImageFilter.UnsharpMask(radius=2, percent=60, threshold=1))
    return img

def fog(base, y0, y1, color=(150, 160, 185), amax=60, bands=5):
    lay = Image.new("RGBA", base.size, (0, 0, 0, 0))
    for b in range(bands):
        t = b / max(1, bands - 1)
        yy = int(y0 + (y1 - y0) * t)
        n = noise_tex(base.size[0], 80, sigma=60, tint=color, alpha=int(amax * (1 - t * 0.5)), blur=8, scale=3)
        lay.alpha_composite(n, (0, yy))
    base.alpha_composite(lay)

def frame_title(img, title, sub):
    d = ImageDraw.Draw(img, "RGBA")
    d.rectangle([0, 0, W, H], outline=(20, 16, 12, 255), width=10)
    d.rectangle([6, 6, W - 6, H - 6], outline=(120, 96, 50, 90), width=2)
    f1, f2 = _font(30), _font(16, italic=True)
    tw = d.textlength(title, font=f1)
    d.rectangle([28, H - 86, 28 + max(tw, d.textlength(sub, font=f2)) + 34, H - 26], fill=(8, 7, 11, 165))
    d.text((44, H - 80), title, font=f1, fill=(231, 200, 115, 255))
    d.text((45, H - 44), sub, font=f2, fill=(176, 168, 150, 255))

def save(img, name):
    img = painted_pass(img)
    p = os.path.join(OUT, name + ".png")
    img.save(p)
    print(f"  painted {p}  ({img.size[0]}×{img.size[1]})")

# ---------------------------------------------------------------- MAPS

def market():
    img = vgrad(W, H, (40, 33, 48), (14, 11, 18)).convert("RGBA")
    # night sky glow above the rooftops
    img.alpha_composite(radial(W, H, W * 0.5, 40, W * 0.8, (66, 54, 78, 150), (0, 0, 0, 0)))
    # architecture closes off the top — painted townhouses edge to edge
    back_arch(img, 30, 150, (66, 54, 44), (40, 32, 26), count=8, jitter=46, seed=11, kind="house")
    img.alpha_composite(noise_tex(W, H, sigma=46, tint=(90, 78, 60), alpha=16))
    # the square itself fills the rest of the frame, overflowing the sides and bottom
    iso_floor(img, W // 2, 300, 22, 18, (60, 52, 44), tw=78, th=39, jitter=14, seed=5)
    img.alpha_composite(noise_tex(W, H, sigma=52, tint=(126, 104, 80), alpha=20))
    d = ImageDraw.Draw(img, "RGBA")
    cx, cy = W // 2, 300
    # fountain pool (center)
    fx, fy = iso(cx, cy, 5, 4)
    d.ellipse([fx - 52, fy - 26, fx + 52, fy + 26], fill=(30, 40, 52, 255))
    d.ellipse([fx - 38, fy - 19, fx + 38, fy + 19], fill=(46, 74, 96, 255))
    d.ellipse([fx - 20, fy - 11, fx + 20, fy + 11], fill=(86, 132, 158, 220))
    img.alpha_composite(glow(W, H, fx, fy - 6, 70, (120, 170, 200), 60))
    # stalls (painted awning boxes)
    for (tx, ty, hue) in [(2, 2, (150, 70, 150)), (8, 2, (170, 120, 60)), (2, 7, (70, 110, 170)), (8, 7, (150, 70, 90))]:
        bx, by = iso(cx, cy, tx, ty)
        iso_block(d, bx, by + 6, 46, 22, (58, 46, 36), (62, 50, 38), (40, 32, 24))
        d.polygon([(bx - 30, by - 24), (bx + 30, by - 24), (bx + 22, by - 38), (bx - 22, by - 38)], fill=hue + (255,))
        d.polygon([(bx - 30, by - 24), (bx + 30, by - 24), (bx + 30, by - 21), (bx - 30, by - 21)], fill=(26, 20, 16, 255))
    # crates / barrels
    for (tx, ty) in [(4, 7), (6, 7), (1, 5), (9, 5), (1, 2)]:
        bx, by = iso(cx, cy, tx, ty)
        iso_block(d, bx, by + 4, 26, 16, (106, 79, 46), (90, 64, 36), (62, 45, 24))
    # banners + torches along the rim
    for (tx, ty, hue) in [(0, 1, (150, 70, 150)), (10, 1, (170, 120, 60)), (0, 8, (70, 110, 170))]:
        bx, by = iso(cx, cy, tx, ty)
        d.line([(bx, by), (bx, by - 50)], fill=(40, 32, 26, 255), width=4)
        d.polygon([(bx, by - 50), (bx + 22, by - 46), (bx + 18, by - 26), (bx, by - 30)], fill=hue + (255,))
    for (tx, ty) in [(3, 3), (7, 6)]:
        bx, by = iso(cx, cy, tx, ty)
        img.alpha_composite(glow(W, H, bx, by - 44, 64, (255, 180, 90), 150))
        d.ellipse([bx - 4, by - 52, bx + 4, by - 44], fill=(255, 224, 150, 255))
    # a couple of distant rooftops for depth (top of frame)
    for rx in range(-1, 12, 2):
        bx, by = iso(cx, cy - 70, rx, 0)
        iso_block(d, bx, by + 10, 70, 60, (40, 34, 44), (34, 28, 38), (24, 20, 28))
    fog(img, 60, 220, (90, 80, 110), amax=42, bands=4)
    img.alpha_composite(glow(W, H, W * 0.5, 70, W * 0.6, (90, 80, 130), 70))
    img.alpha_composite(vignette(W, H, 150))
    frame_title(img, "The Market of the Causeway", "torchlit square at dusk · where the road to the Wall begins")
    save(img, "market")

def wall():
    img = vgrad(W, H, (40, 44, 56), (8, 10, 16)).convert("RGBA")
    d = ImageDraw.Draw(img, "RGBA")
    # the Wall of the Faithless — vast grey stone filling the whole upper frame, faces in it
    wall_top = 0
    wall_h = 250
    d.rectangle([0, wall_top, W, wall_h], fill=(44, 44, 54, 255))
    # block courses
    for j in range(0, wall_h, 42):
        d.line([(0, j), (W, j)], fill=(28, 28, 36, 140), width=2)
        for i in range((j // 42 % 2) * 32, W, 64):
            d.line([(i, j), (i, j + 42)], fill=(28, 28, 36, 120), width=2)
    img.alpha_composite(noise_tex(W, H, sigma=40, tint=(110, 115, 135), alpha=20))
    rnd = random.Random(3)
    for _ in range(420):  # ten thousand faces, suggested
        fx, fy = rnd.randint(8, W - 8), rnd.randint(18, wall_h - 18)
        s = rnd.uniform(0.7, 1.5)
        d.ellipse([fx - 5 * s, fy - 7 * s, fx + 5 * s, fy + 7 * s], fill=(150, 155, 172, 34))
        d.ellipse([fx - 2 * s, fy - 2, fx - 1 * s, fy - 1], fill=(8, 8, 12, 110))
        d.ellipse([fx + 1 * s, fy - 2, fx + 2 * s, fy - 1], fill=(8, 8, 12, 110))
        d.arc([fx - 3 * s, fy + 1, fx + 3 * s, fy + 5], 200, 340, fill=(8, 8, 12, 70))
    img.alpha_composite(glow(W, H, W * 0.5, wall_h - 30, W * 0.9, (110, 130, 170), 44))
    # the salt-marsh floor in front, overflowing the frame
    iso_floor(img, W // 2, 290, 22, 16, (42, 46, 50), jitter=10, seed=4)
    img.alpha_composite(noise_tex(W, H, sigma=46, tint=(86, 96, 116), alpha=18))
    cx, cy = W // 2, 290
    # the last torch (lone warm light against all that grey)
    tx, ty = iso(cx, cy, 5, 2)
    img.alpha_composite(glow(W, H, tx, ty - 50, 120, (255, 170, 80), 200))
    d.line([(tx, ty + 2), (tx, ty - 44)], fill=(40, 36, 30, 255), width=5)
    d.polygon([(tx - 8, ty - 44), (tx, ty - 70), (tx + 8, ty - 44)], fill=(255, 180, 80, 255))
    d.polygon([(tx - 4, ty - 46), (tx, ty - 60), (tx + 4, ty - 46)], fill=(255, 232, 160, 255))
    # greywort glowing at the wall's foot
    gx, gy = iso(cx, cy, 2, 1)
    img.alpha_composite(glow(W, H, gx, gy, 36, (150, 200, 200), 90))
    # dead marsh tree + pilings
    for (px, py) in [(1, 5), (9, 5), (9, 7)]:
        bx, by = iso(cx, cy, px, py)
        iso_block(d, bx, by + 4, 14, 30, (60, 52, 36), (48, 40, 26), (30, 24, 16))
    dx, dy = iso(cx, cy, 8, 3)
    d.line([(dx, dy + 2), (dx - 2, dy - 34)], fill=(38, 32, 28, 255), width=5)
    for ang in (-0.5, 0.3, -0.2, 0.6):
        d.line([(dx, dy - 24), (dx + math.cos(ang) * 18, dy - 24 - abs(math.sin(ang)) * 22)], fill=(38, 32, 28, 255), width=2)
    # marsh reeds
    for (px, py) in [(1, 4), (10, 4), (3, 6)]:
        bx, by = iso(cx, cy, px, py)
        for k in range(-3, 4):
            d.line([(bx + k * 3, by + 3), (bx + k * 3 + (k % 2) * 3, by - 22 - abs(k) * 2)], fill=(40, 60, 44, 220), width=2)
    fog(img, wall_top + 110, 360, (140, 150, 175), amax=70, bands=6)
    img.alpha_composite(vignette(W, H, 165))
    frame_title(img, "Past the Last Torch", "the Wall of the Faithless · ten thousand faces in the cold grey stone")
    save(img, "wall")

def court():
    img = vgrad(W, H, (24, 26, 44), (5, 5, 11)).convert("RGBA")
    img.alpha_composite(radial(W, H, W * 0.5, 80, W * 0.85, (44, 54, 96, 150), (0, 0, 0, 0)))
    # a far back wall of the cathedral-of-the-dead, faces in the gloom, filling the top
    d = ImageDraw.Draw(img, "RGBA")
    d.rectangle([0, 0, W, 150], fill=(20, 22, 36, 255))
    rnd = random.Random(9)
    for _ in range(120):
        fx, fy = rnd.randint(8, W - 8), rnd.randint(14, 140)
        d.ellipse([fx - 4, fy - 6, fx + 4, fy + 6], fill=(90, 100, 150, 26))
    img.alpha_composite(noise_tex(W, H, sigma=40, tint=(70, 80, 130), alpha=14))
    iso_floor(img, W // 2, 320, 22, 16, (30, 32, 48), jitter=8, seed=6)
    img.alpha_composite(noise_tex(W, H, sigma=42, tint=(78, 88, 138), alpha=16))
    cx, cy = W // 2, 320
    # great pillars receding, filling the depth to the frame edges
    for (px, py) in [(-1, 6), (10, 6), (0, 3), (9, 3), (1, 0), (8, 0), (3, -1), (6, -1)]:
        bx, by = iso(cx, cy, px, py)
        iso_block(d, bx, by + 6, 40, 230, (40, 42, 58), (32, 34, 48), (20, 22, 34))
    # the throne (antlered, on a dais, cold blue god-light behind)
    txx, tyy = iso(cx, cy, 5, 1)
    img.alpha_composite(glow(W, H, txx, tyy - 40, 150, (110, 140, 210), 120))
    iso_block(d, txx, tyy + 6, 44, 30, (44, 44, 62), (36, 36, 52), (24, 24, 38))
    d.polygon([(txx - 13, tyy - 18), (txx - 13, tyy - 62), (txx + 13, tyy - 62), (txx + 13, tyy - 18)], fill=(58, 58, 84, 255))
    for k in (-1, 0, 1):  # antler finials
        d.line([(txx + k * 8, tyy - 62), (txx + k * 10, tyy - 80 - abs(k) * 6)], fill=(150, 130, 70, 255), width=3)
    # the Crown of Horns, turning at the throne's foot
    crx, cry = iso(cx, cy, 5, 3)
    img.alpha_composite(glow(W, H, crx, cry - 6, 40, (210, 170, 90), 120))
    d.arc([crx - 16, cry - 12, crx + 16, cry + 6], 180, 360, fill=(40, 36, 30, 255), width=4)
    for k in (-2, -1, 0, 1, 2):
        d.line([(crx + k * 6, cry - 8), (crx + k * 7, cry - 22 - abs(k) * 2)], fill=(24, 20, 18, 255), width=2)
    # two grey balance-shrines flanking
    for (px, py) in [(2, 2), (8, 2)]:
        bx, by = iso(cx, cy, px, py)
        iso_block(d, bx, by + 5, 18, 26, (44, 46, 60), (36, 38, 50), (24, 26, 36))
        d.line([(bx, by - 22), (bx, by - 52)], fill=(130, 135, 160, 255), width=2)
        d.line([(bx - 16, by - 50), (bx + 16, by - 48)], fill=(130, 135, 160, 255), width=2)
    # cold candle pairs
    for (px, py) in [(4, 3), (6, 3)]:
        bx, by = iso(cx, cy, px, py)
        img.alpha_composite(glow(W, H, bx, by - 14, 26, (255, 210, 130), 90))
    # god-rays from above
    rays = Image.new("RGBA", (W, H), (0, 0, 0, 0))
    rd = ImageDraw.Draw(rays)
    for k in range(-3, 4):
        rd.polygon([(W / 2, -40), (W / 2 + k * 60 - 30, H), (W / 2 + k * 60 + 30, H)], fill=(120, 150, 210, 16))
    img.alpha_composite(rays.filter(ImageFilter.GaussianBlur(18)))
    fog(img, 120, 320, (90, 110, 160), amax=46, bands=5)
    img.alpha_composite(vignette(W, H, 185))
    frame_title(img, "The Court of the Dead", "Kelemvor's hall · the Crown turns at the foot of the throne")
    save(img, "court")

def reedwalk():
    img = vgrad(W, H, (38, 44, 50), (10, 14, 20)).convert("RGBA")
    img.alpha_composite(radial(W, H, W * 0.62, 60, W * 0.8, (62, 72, 74, 130), (0, 0, 0, 0)))
    # far shore + the mist-veiled causeway running off to the Wall, filling the top band
    d = ImageDraw.Draw(img, "RGBA")
    d.rectangle([0, 70, W, 150], fill=(30, 36, 40, 220))
    back_arch(img, 30, 96, (44, 48, 50), (28, 32, 34), count=7, jitter=30, seed=21, kind="cliff")
    iso_floor(img, W // 2, 300, 22, 9, (52, 52, 46), jitter=11, seed=8)
    cx, cy = W // 2, 300
    # the river — painted water plane filling the lower frame, overflowing
    water = Image.new("RGBA", (W, H), (0, 0, 0, 0))
    wd = ImageDraw.Draw(water)
    for r in range(9, 22):
        for c in range(-2, 22):
            x, y = iso(cx, cy, c, r)
            if y < -40 or y > H + 40:
                continue
            shimmer = 18 + int(14 * math.sin(c * 0.7 + r))
            diamond(wd, x, y, 80, 40, (26 + shimmer // 2, 50 + shimmer, 70 + shimmer, 255))
    water = water.filter(ImageFilter.GaussianBlur(1.4))
    img.alpha_composite(water)
    img.alpha_composite(noise_tex(W, H, sigma=40, tint=(120, 140, 150), alpha=18))
    # the ferry, moored
    bx, by = iso(cx, cy, 8, 8)
    d.polygon([(bx - 34, by - 2), (bx + 34, by - 2), (bx + 24, by - 12), (bx - 24, by - 12)], fill=(58, 44, 28, 255))
    d.polygon([(bx - 24, by - 12), (bx + 24, by - 12), (bx, by - 6), (bx, by - 6)], fill=(74, 56, 34, 255))
    d.line([(bx + 14, by - 9), (bx + 34, by - 40)], fill=(40, 32, 24, 255), width=3)
    # a wayshrine + a great causeway pier
    sx, sy = iso(cx, cy, 5, 3)
    img.alpha_composite(glow(W, H, sx, sy - 30, 40, (150, 130, 200), 60))
    iso_block(d, sx, sy + 5, 20, 30, (44, 40, 56), (36, 32, 48), (24, 22, 34))
    # reeds along the bank
    for (px, py) in [(1, 4), (10, 4), (3, 6), (10, 6), (2, 8)]:
        rx, ry = iso(cx, cy, px, py)
        for k in range(-3, 4):
            d.line([(rx + k * 3, ry + 4), (rx + k * 3 + (k % 2) * 3, ry - 26 - abs(k) * 2)], fill=(46, 66, 48, 230), width=2)
    fog(img, 200, 420, (150, 165, 175), amax=66, bands=6)
    img.alpha_composite(vignette(W, H, 160))
    frame_title(img, "The Reed-Walk", "the cold river below the market · where the causeway meets the water")
    save(img, "reedwalk")

if __name__ == "__main__":
    print("Painting Infinity-Engine-style area maps (original / CC0)…")
    market(); wall(); court(); reedwalk()
    print("done — see play/maps/")
