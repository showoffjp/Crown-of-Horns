#!/usr/bin/env python3
"""
Brighten the CC0 Dungeon Crawl tiles for use as 3D terrain.

The source tiles average 15–32 of 255 — Crawl draws them tiny, on black, with
its own contrast. Dropped onto lit geometry in Unity they read as pure black,
which is exactly how the Unity floor looked once it was textured.

This writes brightened copies to Resources/Art/DCSS/lit/<group>/, preserving
filenames so the renderer's family/variant lookup is unchanged. Each tile is
scaled toward a target mean and gently gamma-lifted so dark detail survives
instead of crushing. Deterministic; re-run any time:

  python3 tools/gen-lit-tiles.py
"""
import colorsys, math, os
from PIL import Image

ROOT = os.path.join(os.path.dirname(__file__), "..")
SRC = os.path.join(ROOT, "Assets", "Resources", "Art", "DCSS")
OUT = os.path.join(SRC, "lit")

TARGET = {"floor": 70, "wall": 78}    # mean brightness to aim for — dim enough to stay "the grey"
GAMMA = 0.85                          # <1 lifts midtones without clipping
MAX_GAIN = 3.4                        # beyond this a tile stops being terrain and starts glowing
SHOULDER = 1.7                        # exponential roll-off: bright pixels compress, never clip
SAT_CAP = {"floor": 0.34, "wall": 0.28}   # hard ceiling on chroma after the lift
SAT_FALLOFF = 0.80                    # how strongly brightening desaturates (1.0 = fully)

def mean_of(im):
    px = list(im.convert("RGB").getdata())
    return sum(sum(p) for p in px) / (3 * len(px)) or 1.0

def _tone_lut(gain):
    """Gamma lift, then a gain rolled off exponentially instead of clipped."""
    norm = 1.0 - math.exp(-SHOULDER)
    lut = []
    for i in range(256):
        v = ((i / 255.0) ** GAMMA) * gain
        lut.append((1.0 - math.exp(-SHOULDER * min(1.0, v))) / norm)
    return lut

def lift(im, target, sat_cap):
    """Brighten a Crawl tile into something that can be terrain.

    The tone curve is applied to *value only*, in HSV. A straight RGB multiply
    raises every channel by the same factor, which preserves the ratio between
    them and therefore preserves saturation — so a dark, saturated brick became a
    bright, equally saturated brick, i.e. fluorescent orange. Real light does the
    opposite: brightening a surface washes its chroma out. So saturation is scaled
    down in proportion to how far the pixel was lifted, then capped outright, and
    the tiles come back as stone instead of candy.
    """
    im = im.convert("RGB")
    gain = min(MAX_GAIN, target / (mean_of(im.point([min(255, round(((i / 255.0) ** GAMMA) * 255))
                                                     for i in range(256)] * 3))))
    lut = _tone_lut(gain)
    cache = {}
    out = Image.new("RGB", im.size)
    src = list(im.getdata())
    dst = []
    for rgb in src:
        hit = cache.get(rgb)
        if hit is None:
            h, sat, v = colorsys.rgb_to_hsv(rgb[0] / 255.0, rgb[1] / 255.0, rgb[2] / 255.0)
            nv = lut[int(v * 255)]
            if nv > 1e-6 and v > 1e-6:
                sat *= (v / nv) ** SAT_FALLOFF
            sat = min(sat, sat_cap)
            r, g, b = colorsys.hsv_to_rgb(h, sat, nv)
            hit = (int(r * 255 + 0.5), int(g * 255 + 0.5), int(b * 255 + 0.5))
            cache[rgb] = hit
        dst.append(hit)
    out.putdata(dst)
    return out

def main():
    total = 0
    for group, target in TARGET.items():
        srcdir, outdir = os.path.join(SRC, group), os.path.join(OUT, group)
        if not os.path.isdir(srcdir):
            continue
        os.makedirs(outdir, exist_ok=True)
        for f in sorted(os.listdir(srcdir)):
            if not f.endswith(".png"):
                continue
            lift(Image.open(os.path.join(srcdir, f)), target, SAT_CAP.get(group, 0.4)).save(
                os.path.join(outdir, f), optimize=True)
            total += 1
    print(f"Wrote {total} lit tiles into Assets/Resources/Art/DCSS/lit/")

if __name__ == "__main__":
    main()
