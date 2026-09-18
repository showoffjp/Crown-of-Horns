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
import os
from PIL import Image

ROOT = os.path.join(os.path.dirname(__file__), "..")
SRC = os.path.join(ROOT, "Assets", "Resources", "Art", "DCSS")
OUT = os.path.join(SRC, "lit")

TARGET = {"floor": 92, "wall": 112}   # mean brightness to aim for
GAMMA = 0.85                          # <1 lifts midtones without clipping
MAX_GAIN = 6.0

def mean_of(im):
    px = list(im.convert("RGB").getdata())
    return sum(sum(p) for p in px) / (3 * len(px)) or 1.0

def lift(im, target):
    """Gamma-lift first, then scale to the target mean — applying the gain before
    the gamma compounds the two and badly overshoots."""
    im = im.convert("RGB")
    gamma_lut = [min(255, round(((i / 255.0) ** GAMMA) * 255)) for i in range(256)]
    lifted = im.point(gamma_lut * 3)
    gain = min(MAX_GAIN, target / mean_of(lifted))
    gain_lut = [min(255, round(i * gain)) for i in range(256)]
    return lifted.point(gain_lut * 3)

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
            lift(Image.open(os.path.join(srcdir, f)), target).save(
                os.path.join(outdir, f), optimize=True)
            total += 1
    print(f"Wrote {total} lit tiles into Assets/Resources/Art/DCSS/lit/")

if __name__ == "__main__":
    main()
