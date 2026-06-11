#!/usr/bin/env python3
"""
Generate honest PLACEHOLDER combat VFX — short sprite-frame bursts, fully procedural
(PIL only, no external art). FxSystem plays Resources/FX/<effect>/ frame folders
(LoadAll<Sprite>, natural-sorted) and no-ops when missing, so hits/heals have no flash
until art is dropped in. This emits the exact effect folders the engine auto-picks by
damage type (FxSystem.PlayImpact), each a small expanding tinted burst.

Each frame is a Sprite-imported PNG with a committed *.meta (textureType: 8) — required
for Resources.Load<Sprite> in this 3D-default project, same as the cast art.
Re-run any time: `python3 tools/gen-placeholder-vfx.py`.
"""
import os, math, random, hashlib
from PIL import Image, ImageDraw

OUT = os.path.join(os.path.dirname(__file__), "..", "Assets", "Resources", "FX")
SIZE = 64
FRAMES = 6

# effect -> (core color, ring color)
EFFECTS = {
    "impact":           ((255, 255, 255), (200, 200, 210)),
    "impact_fire":      ((255, 240, 180), (240, 90, 30)),
    "impact_ice":       ((230, 255, 255), (90, 200, 240)),
    "impact_lightning": ((255, 255, 210), (240, 220, 60)),
    "impact_holy":      ((255, 250, 210), (230, 190, 80)),
    "impact_dark":      ((180, 120, 210), (90, 40, 120)),
    "impact_acid":      ((220, 255, 170), (120, 210, 40)),
    "impact_poison":    ((200, 240, 150), (110, 170, 60)),
}

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
  maxTextureSize: 2048
  textureSettings:
    serializedVersion: 2
    filterMode: 0
    aniso: 1
    mipBias: 0
    wrapU: 1
    wrapV: 1
  nPOTScale: 0
  spriteMode: 1
  spriteExtrude: 1
  spriteMeshType: 1
  alignment: 0
  spritePivot: {{x: 0.5, y: 0.5}}
  spritePixelsToUnits: 64
  spriteBorder: {{x: 0, y: 0, z: 0, w: 0}}
  spriteGenerateFallbackPhysicsShape: 1
  alphaUsage: 1
  alphaIsTransparency: 1
  spriteTessellationDetail: -1
  textureType: 8
  textureShape: 1
  userData:\x20
  assetBundleName:\x20
  assetBundleVariant:\x20
"""

def guid_for(rel): return hashlib.md5(rel.encode()).hexdigest()

def burst_frame(core, ring, f, seed):
    img = Image.new("RGBA", (SIZE, SIZE), (0, 0, 0, 0))
    d = ImageDraw.Draw(img)
    cx = cy = SIZE // 2
    prog = f / (FRAMES - 1)               # 0..1 expansion
    alpha = int(255 * (1 - prog) ** 1.4)  # fade out
    r = int(6 + prog * 24)
    cr = int(11 * (1 - prog))             # shrinking bright core
    if cr > 0:
        d.ellipse([cx - cr, cy - cr, cx + cr, cy + cr], fill=(*core, alpha))
    if r > 0:
        d.ellipse([cx - r, cy - r, cx + r, cy + r], outline=(*ring, alpha), width=max(1, int(4 * (1 - prog))))
    random.seed(seed + f * 17)
    for _ in range(9):
        ang = random.uniform(0, 2 * math.pi); pr = r * random.uniform(0.55, 1.05)
        px, py = cx + math.cos(ang) * pr, cy + math.sin(ang) * pr
        ps = random.randint(1, 3)
        d.ellipse([px - ps, py - ps, px + ps, py + ps], fill=(*ring, alpha))
    return img

def heal_frame(f, seed):
    img = Image.new("RGBA", (SIZE, SIZE), (0, 0, 0, 0))
    d = ImageDraw.Draw(img)
    prog = f / (FRAMES - 1)
    random.seed(seed)
    for k in range(10):
        basex = random.uniform(12, SIZE - 12)
        rise = prog * 34 + random.uniform(0, 6)
        py = SIZE - 8 - rise
        a = int(230 * (1 - prog))
        ps = random.choice([1, 2, 2, 3])
        col = (160, 240, 170) if k % 2 else (220, 255, 220)
        d.ellipse([basex - ps, py - ps, basex + ps, py + ps], fill=(*col, a))
    # a soft central glow that fades
    gr = int(8 + prog * 6); ga = int(120 * (1 - prog))
    d.ellipse([SIZE//2 - gr, SIZE//2 - gr, SIZE//2 + gr, SIZE//2 + gr], fill=(180, 255, 190, ga))
    return img

def emit(effect, frame_maker, seed):
    folder = os.path.join(OUT, effect)
    os.makedirs(folder, exist_ok=True)
    for f in range(FRAMES):
        img = frame_maker(f, seed)
        png = os.path.join(folder, f"{f}.png")
        img.save(png)
        rel = f"Assets/Resources/FX/{effect}/{f}.png"
        with open(png + ".meta", "w") as fh:
            fh.write(META.format(guid=guid_for(rel)))

def main():
    n = 0
    for effect, (core, ring) in EFFECTS.items():
        seed = int(hashlib.md5(effect.encode()).hexdigest(), 16) & 0xffffff
        emit(effect, lambda f, s, c=core, r=ring: burst_frame(c, r, f, s), seed)
        n += 1
    emit("heal", heal_frame, 0x4EA1)
    n += 1
    print(f"Generated {n} placeholder VFX effects ({FRAMES} frames each, + .meta) into Assets/Resources/FX/")

if __name__ == "__main__":
    main()
