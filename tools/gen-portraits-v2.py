#!/usr/bin/env python3
"""
Painterly procedural portraits — v2 of the cast art. Replaces the labeled-token
portraits with layered, era-lit character busts: gradient atmosphere, shaded
skin/hood/helm/hair by archetype, rim light, era accents (gold circlet / bone /
blue fire / grey ash), brushy noise and vignette. Pure PIL, deterministic per name
(re-runs are stable), same filenames as before so every portrait auto-wires.

Honest placeholders still — for *real* paintings, allowlist commons.wikimedia.org
and run tools/fetch-portraits.py. Re-run: python3 tools/gen-portraits-v2.py
"""
import hashlib, math, os, random
from PIL import Image, ImageDraw, ImageFilter

ROOT = os.path.join(os.path.dirname(__file__), "..")
OUT = os.path.join(ROOT, "Assets", "Resources", "Portraits")
W, H = 320, 400

# ---- era palettes (DESIGN §5) ----------------------------------------------
ERAS = {
    "netheril":   {"bg": (24, 30, 48),  "glow": (212, 175, 84),  "accent": (120, 180, 230)},
    "crownwars":  {"bg": (18, 32, 28),  "glow": (150, 190, 170), "accent": (200, 220, 210)},
    "troubles":   {"bg": (30, 22, 20),  "glow": (216, 200, 170), "accent": (150, 40, 40)},
    "spellplague":{"bg": (14, 22, 40),  "glow": (90, 170, 240),  "accent": (60, 220, 255)},
    "fugue":      {"bg": (26, 26, 30),  "glow": (140, 140, 150), "accent": (95, 95, 105)},
    "gate":       {"bg": (28, 24, 34),  "glow": (201, 162, 75),  "accent": (155, 45, 45)},
}
def classify_era(name):
    n = name.lower()
    for era, keys in [
        ("netheril", ["naeve", "netheril", "mythallar", "arcanist", "sentinel", "weave", "construct"]),
        ("crownwars", ["ilfaeril", "crown-war", "verdict", "aelryth", "damned", "vaelin", "echo"]),
        ("troubles", ["bone", "myrkul", "avatar", "keeper", "kelemvorite", "zealot of myrkul"]),
        ("spellplague", ["spellplague", "half-unmade", "aberration", "herald of the unmade", "weave-wraith"]),
        ("fugue", ["maerin", "fugue", "wall", "unclaimed", "faithless soul", "girl in the wall", "gravedigger", "sorrow", "unmade", "unbound", "shade", "wraith", "cantor", "hollow"]),
    ]:
        if any(k in n for k in keys): return era
    return "gate"

def archetype(name):
    n = name.lower()
    if any(k in n for k in ["garrow", "sere", "cass", "doomguide", "justiciar", "templar", "keeper", "vayle"]): return "priest"
    if any(k in n for k in ["roen", "quill", "sable", "tally", "harper", "snitch", "wrenna", "tamsin"]): return "rogue"
    if any(k in n for k in ["varra", "naeve", "mhaere", "arcanist", "cantor", "herald of the gate"]): return "mage"
    if any(k in n for k in ["kallia", "fist", "knight", "enforcer", "aldric", "davyn", "ferryman", "hensley", "aelryth", "vaelin", "veld"]): return "warrior"
    if any(k in n for k in ["wraith", "shade", "unmade", "sorrow", "husk", "revenant", "girl in the wall", "faithless soul", "unclaimed", "half-unmade", "returned", "maw", "horror", "aberration", "avatar"]): return "spirit"
    if any(k in n for k in ["ilfaeril", "maerin"]): return "elf"
    return "commoner"

# ---- paint helpers ----------------------------------------------------------
def vgrad(size, top, bottom):
    """vertical gradient via 1px column resize (cheap, smooth)"""
    col = Image.new("RGB", (1, 256))
    for y in range(256):
        t = y / 255
        col.putpixel((0, y), tuple(int(top[i] * (1 - t) + bottom[i] * t) for i in range(3)))
    return col.resize(size)

def rgl(img, cx, cy, r, color, alpha):
    """radial glow layer composited onto img"""
    glow = Image.new("L", img.size, 0)
    d = ImageDraw.Draw(glow)
    for i in range(r, 0, -2):
        a = int(alpha * (1 - i / r) ** 2)
        d.ellipse([cx - i, cy - i, cx + i, cy + i], fill=a)
    layer = Image.new("RGB", img.size, color)
    img.paste(layer, (0, 0), glow)

def shade(c, f):
    return tuple(max(0, min(255, int(v * f))) for v in c)

def make_portrait(name):
    seed = int(hashlib.md5(name.encode()).hexdigest(), 16)
    rnd = random.Random(seed)
    era = ERAS[classify_era(name)]
    arch = archetype(name)
    spirit = arch == "spirit"

    img = vgrad((W, H), shade(era["bg"], 1.4), shade(era["bg"], 0.55))
    # atmosphere: drifting glow blobs behind the figure
    for _ in range(4):
        rgl(img, rnd.randint(0, W), rnd.randint(0, H // 2), rnd.randint(60, 150),
            era["glow"], rnd.randint(14, 30))
    d = ImageDraw.Draw(img, "RGBA")

    cx, cy = W // 2, int(H * 0.42)              # head center
    hw, hh = 52 + rnd.randint(-4, 6), 66 + rnd.randint(-4, 6)
    lit = 1 if rnd.random() < 0.5 else -1        # which side the key light is on

    # skin / substance tones
    if spirit:
        base = shade(era["accent"], 0.8); dark = shade(era["accent"], 0.35); hi = shade(era["accent"], 1.3)
    else:
        tone = rnd.choice([(224, 188, 154), (196, 156, 120), (162, 122, 92), (120, 88, 66), (210, 180, 160)])
        base, dark, hi = tone, shade(tone, 0.55), shade(tone, 1.25)

    # shoulders / bust
    sh_top = cy + hh - 8
    bust = [(cx - 118, H), (cx - 96, sh_top + 36), (cx - 48, sh_top + 6), (cx, sh_top - 2),
            (cx + 48, sh_top + 6), (cx + 96, sh_top + 36), (cx + 118, H)]
    garb = {"priest": (64, 60, 72), "rogue": (44, 48, 52), "mage": (52, 42, 70),
            "warrior": (70, 70, 78), "spirit": shade(era["accent"], 0.25),
            "elf": (58, 66, 60), "commoner": (74, 64, 52)}[arch]
    d.polygon(bust, fill=shade(garb, 1.0))
    # garb shading: dark wedge on shadow side, light seam on lit side
    d.polygon([(cx - lit * 8, sh_top - 2), (cx - lit * 118, H), (cx - lit * 40, H)], fill=(*shade(garb, 0.6), 200))
    d.line([(cx + lit * 30, sh_top + 8), (cx + lit * 86, H)], fill=(*shade(era["glow"], 0.9), 110), width=4)
    if arch == "warrior":  # pauldron plate
        d.ellipse([cx + lit * 34 - 30, sh_top + 4, cx + lit * 34 + 42, sh_top + 60], fill=shade(garb, 1.35), outline=shade(garb, 0.5))

    # neck + head
    d.rectangle([cx - 16, cy + hh - 26, cx + 16, sh_top + 8], fill=dark)
    d.ellipse([cx - hw, cy - hh, cx + hw, cy + hh], fill=base)
    # core shadow on the off side + lit plane
    d.ellipse([cx - hw + (0 if lit > 0 else 18), cy - hh + 6, cx + hw - (18 if lit > 0 else 0), cy + hh - 4], fill=dark)
    d.ellipse([cx - hw + (14 if lit > 0 else 26), cy - hh + 10, cx + hw - (26 if lit > 0 else 14), cy + hh - 10], fill=base)
    d.ellipse([cx - hw + (20 if lit > 0 else 44), cy - hh + 16, cx + hw - (44 if lit > 0 else 20), cy + hh - 22], fill=hi)

    # features: brow shadow, eye glints, mouth hint
    ey = cy - 6
    d.ellipse([cx - 26, ey - 6, cx - 10, ey + 4], fill=shade(dark, 0.8))
    d.ellipse([cx + 10, ey - 6, cx + 26, ey + 4], fill=shade(dark, 0.8))
    glint = era["accent"] if spirit else (235, 235, 225)
    d.ellipse([cx - 21, ey - 3, cx - 15, ey + 3], fill=glint)
    d.ellipse([cx + 15, ey - 3, cx + 21, ey + 3], fill=glint)
    d.line([(cx - 10, cy + 28), (cx + 10, cy + 28)], fill=shade(dark, 0.8), width=3)

    # headwear / hair by archetype
    if arch == "priest":      # deep hood
        d.arc([cx - hw - 14, cy - hh - 18, cx + hw + 14, cy + hh + 30], 180, 360, fill=shade(garb, 1.2), width=26)
        d.pieslice([cx - hw - 10, cy - hh - 14, cx + hw + 10, cy + 4], 180, 360, fill=shade(garb, 1.1))
    elif arch == "mage":      # circlet + long hair
        d.pieslice([cx - hw - 6, cy - hh - 8, cx + hw + 6, cy + 20], 180, 360, fill=shade(garb, 0.8))
        d.arc([cx - hw + 6, cy - hh + 2, cx + hw - 6, cy + 10], 200, 340, fill=era["glow"], width=5)
    elif arch == "warrior":   # helm rim
        d.pieslice([cx - hw - 8, cy - hh - 10, cx + hw + 8, cy + 6], 180, 360, fill=shade((90, 92, 100), 1.0))
        d.line([(cx - hw - 6, cy - 8), (cx + hw + 6, cy - 8)], fill=(60, 62, 70), width=6)
    elif arch == "rogue":     # asymmetric hair + scar
        d.pieslice([cx - hw - 4, cy - hh - 6, cx + hw + 4, cy + 12], 180, 360, fill=(40, 36, 34))
        d.line([(cx + lit * 26, cy - 30), (cx + lit * 34, cy + 6)], fill=shade(dark, 0.7), width=3)
    elif arch == "elf":       # swept hair + ear hints
        d.pieslice([cx - hw - 4, cy - hh - 10, cx + hw + 4, cy + 8], 180, 360, fill=(208, 198, 170))
        d.polygon([(cx - hw - 2, cy - 4), (cx - hw - 16, cy - 16), (cx - hw + 4, cy + 8)], fill=base)
        d.polygon([(cx + hw + 2, cy - 4), (cx + hw + 16, cy - 16), (cx + hw - 4, cy + 8)], fill=base)
    elif spirit:              # unravelling crown of wisps
        for _ in range(7):
            a = rnd.uniform(math.pi, 2 * math.pi); r0 = rnd.uniform(hw * 0.9, hw * 1.5)
            d.ellipse([cx + math.cos(a) * r0 - 4, cy + math.sin(a) * r0 * 0.9 - 4,
                       cx + math.cos(a) * r0 + 4, cy + math.sin(a) * r0 * 0.9 + 4], fill=(*era["accent"], 150))
    else:                     # plain hair
        d.pieslice([cx - hw - 2, cy - hh - 4, cx + hw + 2, cy + 10], 180, 360, fill=(70, 56, 44))

    # rim light along the lit edge
    d.arc([cx - hw - 2, cy - hh - 2, cx + hw + 2, cy + hh + 2],
          (300 if lit > 0 else 150), (30 if lit > 0 else 240), fill=shade(era["glow"], 1.25), width=5)

    # spirits dissolve at the base
    if spirit:
        for _ in range(40):
            x = rnd.randint(cx - 110, cx + 110); y = rnd.randint(int(H * 0.78), H - 6)
            s = rnd.randint(1, 4)
            d.ellipse([x - s, y - s, x + s, y + s], fill=(*shade(era["bg"], 1.6), rnd.randint(90, 200)))

    # painterly grain + vignette + soft bloom
    noise = Image.effect_noise((W, H), 18).convert("L")
    img = Image.composite(img, Image.new("RGB", (W, H), (0, 0, 0)), noise.point(lambda v: 255 - (255 - v) // 6))
    vmask = Image.new("L", (W, H), 0); vd = ImageDraw.Draw(vmask)
    vd.ellipse([-W * 0.35, -H * 0.30, W * 1.35, H * 1.25], fill=255)
    vmask = vmask.filter(ImageFilter.GaussianBlur(60))
    img = Image.composite(img, Image.new("RGB", (W, H), shade(era["bg"], 0.35)), vmask)
    img = Image.blend(img, img.filter(ImageFilter.GaussianBlur(2)), 0.22)
    return img

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
    filterMode: 1
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
  spritePixelsToUnits: 100
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

def main():
    import glob
    names = sorted(os.path.basename(p)[:-4] for p in glob.glob(os.path.join(OUT, "*.png")))
    for n in names:
        img = make_portrait(n)
        path = os.path.join(OUT, n + ".png")
        img.save(path, optimize=True)
        rel = "Assets/Resources/Portraits/" + n + ".png"
        with open(path + ".meta", "w") as f:
            f.write(META.format(guid=hashlib.md5(rel.encode()).hexdigest()))
    print(f"Repainted {len(names)} portraits (painterly v2) into Assets/Resources/Portraits/")

if __name__ == "__main__":
    main()
