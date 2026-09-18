#!/usr/bin/env python3
"""
Turn the square Crawl tiles into isometric floor diamonds.

The grid projects to a 2:1 diamond lattice (GridSystem.GridToWorld), but the
floor was drawn with axis-aligned square quads sitting on those diamond centres.
Neighbouring squares overlap by half a tile in both axes, so the floor rendered
as a dense mat of overlapping rectangles — it read as a brick *wall* laid flat,
with a staircased edge, instead of a tiled floor.

A rhombus of width tileWidth and height tileHeight tessellates that lattice
exactly. This warps each lit tile into one: for an output pixel inside the
diamond, the inverse of the rotate-45°-and-squash transform gives the point to
sample in the square source. Pixels outside the diamond get alpha 0, and the
renderer draws them with a cutout shader, so there is no transparency sorting to
get wrong.

Each diamond also gets its edges darkened slightly, so a large floor still reads
as individual tiles rather than one smear of noise.

Writes Assets/Resources/Art/DCSS/iso/<group>/<same name>.png. Run after
tools/gen-lit-tiles.py:

  python3 tools/gen-lit-tiles.py && python3 tools/gen-iso-tiles.py
"""
import os
from PIL import Image

ROOT = os.path.join(os.path.dirname(__file__), "..")
SRC = os.path.join(ROOT, "Assets", "Resources", "Art", "DCSS")
LIT = os.path.join(SRC, "lit")
OUT = os.path.join(SRC, "iso")

W, H = 128, 64           # 2:1, matching tileWidth 1.0 / tileHeight 0.5
SS = 2                   # supersample factor for a clean diamond edge
EDGE = 0.13              # fraction of the half-diagonal that darkens toward the rim
EDGE_MIN = 0.62          # brightness at the very rim

def diamond(src):
    """Warp a square tile into a 2:1 rhombus with transparent corners."""
    src = src.convert("RGB")
    sw, sh = src.size
    spx = src.load()
    ow, oh = W * SS, H * SS
    out = Image.new("RGBA", (ow, oh), (0, 0, 0, 0))
    opx = out.load()
    for v in range(oh):
        ny = (v + 0.5) / (oh / 2.0) - 1.0          # -1 .. 1 down the diamond
        for u in range(ow):
            nx = (u + 0.5) / (ow / 2.0) - 1.0      # -1 .. 1 across the diamond
            m = abs(nx) + abs(ny)
            if m > 1.0:
                continue                            # outside the rhombus
            # inverse of rotate-45 + 2:1 squash, back into the unit square
            sx = (nx + ny + 1.0) * 0.5
            sy = (ny - nx + 1.0) * 0.5
            r, g, b = spx[min(sw - 1, int(sx * sw)), min(sh - 1, int(sy * sh))]
            if m > 1.0 - EDGE:                      # darken toward the rim
                t = (m - (1.0 - EDGE)) / EDGE
                f = 1.0 - (1.0 - EDGE_MIN) * t
                r, g, b = int(r * f), int(g * f), int(b * f)
            opx[u, v] = (r, g, b, 255)
    return out.resize((W, H), Image.LANCZOS)

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
  maxTextureSize: 256
  textureSettings:
    serializedVersion: 2
    filterMode: 1
    aniso: 1
    mipBias: 0
    wrapU: 1
    wrapV: 1
  nPOTScale: 0
  spriteMode: 1
  spriteExtrude: 0
  spriteMeshType: 0
  alignment: 0
  spritePivot: {{x: 0.5, y: 0.5}}
  spritePixelsToUnits: 128
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
    import hashlib
    total = 0
    for group in ("floor", "wall"):
        srcdir = os.path.join(LIT, group)
        if not os.path.isdir(srcdir):
            srcdir = os.path.join(SRC, group)
        if not os.path.isdir(srcdir):
            continue
        outdir = os.path.join(OUT, group)
        os.makedirs(outdir, exist_ok=True)
        for f in sorted(os.listdir(srcdir)):
            if not f.endswith(".png"):
                continue
            path = os.path.join(outdir, f)
            diamond(Image.open(os.path.join(srcdir, f))).save(path, optimize=True)
            rel = "Assets/Resources/Art/DCSS/iso/%s/%s" % (group, f)
            with open(path + ".meta", "w") as m:
                m.write(META.format(guid=hashlib.md5(rel.encode()).hexdigest()))
            total += 1
    print("Wrote %d isometric tiles into Assets/Resources/Art/DCSS/iso/" % total)

if __name__ == "__main__":
    main()
