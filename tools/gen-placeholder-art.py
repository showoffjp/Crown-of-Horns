#!/usr/bin/env python3
"""
Generate labeled, LORE-ERA-THEMED placeholder art for the whole cast so the game
renders its characters, companions and foes instead of blank cubes — with zero
inspector wiring.

Why this exists: WorldArt loads Resources/Sprites/<name> (map tokens) and
Resources/Portraits/<speaker> (dialogue busts), returning null -> placeholder cube
when missing. The project runs in 3D default-behavior mode, so a committed *.meta with
textureType: 8 (Sprite) is required for Resources.Load<Sprite> to succeed on first
import. This script emits both the PNG and that .meta.

Each character/foe is classified to one of the five eras (DESIGN.md §5) by keyword and
tinted with that era's signature palette:
  Netheril -> gold & sky-blue   Crown Wars -> cold silver-green
  Time of Troubles -> bone & blood   Spellplague -> blue fire   Fugue -> grey absence
Foes get a red rim (threat read); bosses get a crown. Faction tokens keep gameplay
colors (allegiance reads first). These are honest PLACEHOLDERS — fully locally
generated (no license strings); drop real art with the same names to replace them.
Re-run any time: `python3 tools/gen-placeholder-art.py`.
"""
import colorsys
import hashlib
import os
from PIL import Image, ImageDraw, ImageFont

ROOT = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
SPRITES_DIR = os.path.join(ROOT, "Assets", "Resources", "Sprites")
PORTRAITS_DIR = os.path.join(ROOT, "Assets", "Resources", "Portraits")

# --- eras (DESIGN.md §5) as HSV bands: (hue, sat, val) base + (hue,sat,val) accent ----
ERAS = {
    "netheril":  ((0.12, 0.62, 0.82), (0.56, 0.55, 0.95)),  # gold + sky-blue
    "crownwars": ((0.40, 0.28, 0.68), (0.0,  0.0,  0.85)),  # cold elven-green + silver
    "troubles":  ((0.09, 0.20, 0.90), (0.99, 0.70, 0.65)),  # bone-white + blood
    "spellplague": ((0.53, 0.78, 0.98), (0.50, 0.55, 1.0)), # blue fire
    "fugue":     ((0.62, 0.05, 0.52), (0.0,  0.0,  0.75)),  # grey desaturation
    "sacred":    ((0.10, 0.45, 0.78), (0.08, 0.30, 0.95)),  # default: gold leaf over rot
}

# keyword -> era, first match wins (order matters: specific before generic)
ERA_KEYWORDS = [
    ("netheril", ["netherese", "arcane", "arcanist", "mythallar", "weave", "shadow-bound", "naeve", "high lord", "aelryth"]),
    ("crownwars", ["crown-war", "crown war", "ilfaeril", "elven", "revenant"]),
    ("troubles", ["doomguide", "kelemvor", "myrkul", "bone-zealot", "bone crown", "templar",
                   "justiciar", "veld", "cinder", "god-touched", "damned", "vengeful shade",
                   "garrow", "cass", "kallia", "flaming fist", "herald of the gate"]),
    ("spellplague", ["spellplague", "unmade", "unmaking", "unbound", "sorrow", "aberration",
                      "hollow cantor", "pale cantor", "choir", "avatar of bone", "colossus",
                      "first unmade", "horror"]),
    ("fugue", ["maerin", "ferryman", "wall", "faithless", "unclaimed", "returned",
                "half-unmade", "keeper", "gravedigger", "girl in the", "davyn", "sere",
                "pale cantor"]),
    ("sacred", ["aldric", "morn", "vane", "roen", "wrenna", "tally", "tamsin", "varra",
                 "mhaere", "quill", "sable", "harper", "hensley", "vaelin", "sister"]),
]

BOSSES = {
    "The Avatar of Bone", "The Mythallar Colossus", "The Hollow Cantor", "The Pale Cantor",
    "The First Unmade", "The Herald of the Unmade", "Justiciar Veld", "Quill, the Broker",
    "The Unbound", "The Unbound Maw", "God-Touched Horror", "High Lord Aelryth",
}

FACTIONS = {  # gameplay allegiance colors (these read before era)
    "Player": (0.58, 0.55, 0.85), "Ally": (0.33, 0.50, 0.80),
    "Enemy": (0.00, 0.60, 0.80), "Neutral": (0.0, 0.0, 0.60),
}

COMPANIONS = ["Sable", "Naeve", "Garrow", "Maerin", "Mhaere",
              "Ilfaeril", "Quill", "Tamsin", "Varra"]

ENEMIES = [
    "A Sorrow of the Unmade", "Arcane Sentinel", "Arcanist Revenant",
    "Bone-Zealot of Myrkul", "Choir Herald", "Cinder-Hound", "Contract-Devil",
    "Crown-War Revenant", "Damned Shade", "Doomguide Enforcer",
    "Doomguide Interrogator", "Doomguide Knight", "Echo of the Verdict",
    "God-Touched Horror", "Imp of the Fine Print", "Justiciar Veld",
    "Kelemvorite Zealot", "Mythallar Ward", "Netherese War-Construct",
    "Quill, the Broker", "Shadow-Bound Sentinel", "Sorrow-wraith",
    "Spellplague Aberration", "Templar Inquisitor", "The Avatar of Bone",
    "The First Unmade", "The Herald of the Unmade", "The Hollow Cantor",
    "The Mythallar Colossus", "The Pale Cantor", "The Unbound", "The Unbound Maw",
    "Unmade Aberration", "Unmaking Acolyte", "Unmaking Zealot", "Vengeful Shade",
    "Weave-wraith",
]

SPEAKERS = [
    "A Faithless soul", "A Grey Gravedigger", "A Half-Unmade Voice",
    "A Keeper of the Bone Crown", "A girl in the Wall", "Aldric Morn",
    "Brother Sere", "Collector Vane", "Ferryman", "Flaming Fist", "Garrow",
    "Harper Handler", "Hensley, dying", "Herald of the Gate", "High Lord Aelryth",
    "Ilfaeril", "Justiciar Veld", "Maerin", "Mhaere", "Mother Cass", "NPC",
    "Naeve", "Old Davyn", "Quill", "Roen Alleywind", "Roen", "Sable",
    "Sergeant Kallia", "Sister Garrow", "Tally", "Tamsin", "The Pale Cantor",
    "The Returned", "The unclaimed", "Vaelin (an echo)", "Varra", "Wrenna Alleywind",
]

META_TEMPLATE = """fileFormatVersion: 2
guid: {guid}
TextureImporter:
  internalIDToNameTable: []
  externalObjects: {{}}
  serializedVersion: 13
  mipmaps:
    mipMapMode: 0
    enableMipMap: 0
    sRGBTexture: 1
    linearTexture: 0
    fadeOut: 0
    borderMipMap: 0
    mipMapsPreserveCoverage: 0
    alphaTestReferenceValue: 0.5
    mipMapFadeDistanceStart: 1
    mipMapFadeDistanceEnd: 3
  bumpmap:
    convertToNormalMap: 0
    externalNormalMap: 0
    heightScale: 0.25
    normalMapFilter: 0
  isReadable: 0
  streamingMipmaps: 0
  streamingMipmapsPriority: 0
  vTOnly: 0
  ignoreMasterTextureLimit: 0
  grayScaleToAlpha: 0
  generateCubemap: 6
  cubemapConvolution: 0
  seamlessCubemap: 0
  textureFormat: 1
  maxTextureSize: 2048
  textureSettings:
    serializedVersion: 2
    filterMode: 1
    aniso: 1
    mipBias: 0
    wrapU: 1
    wrapV: 1
    wrapW: 1
  nPOTScale: 0
  lightmap: 0
  compressionQuality: 50
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
  singleChannelComponent: 0
  flipbookRows: 1
  flipbookColumns: 1
  maxTextureSizeSet: 0
  compressionQualitySet: 0
  textureFormatSet: 0
  ignorePngGamma: 0
  applyGammaDecoding: 0
  swizzle: 50462976
  cookieLightType: 0
  platformSettings:
  - serializedVersion: 3
    buildTarget: DefaultTexturePlatform
    maxTextureSize: 2048
    resizeAlgorithm: 0
    textureFormat: -1
    textureCompression: 1
    compressionQuality: 50
    crunchedCompression: 0
    allowsAlphaSplitting: 0
    overridden: 0
    ignorePlatformSupport: 0
    androidETC2FallbackOverride: 0
    forceMaximumCompressionQuality_BC6H_BC7: 0
  spriteSheet:
    serializedVersion: 2
    sprites: []
    outline: []
    physicsShape: []
    bones: []
    spriteID: 5e97eb03825dee720800000000000000
    internalID: 0
    vertices: []
    indices:
    edges: []
    weights: []
    secondaryTextures: []
    nameFileIdTable: {{}}
  spritePackingTag:
  pSDRemoveMatte: 0
  userData:
  assetBundleName:
  assetBundleVariant:
"""


def font(size):
    for path in (
        "/usr/share/fonts/truetype/dejavu/DejaVuSans-Bold.ttf",
        "/usr/share/fonts/truetype/dejavu/DejaVuSans.ttf",
    ):
        if os.path.exists(path):
            return ImageFont.truetype(path, size)
    try:
        return ImageFont.load_default(size=size)
    except TypeError:
        return ImageFont.load_default()


def name_hash(name):
    return int(hashlib.md5(name.encode()).hexdigest(), 16)


def guid_for(rel_path):
    return hashlib.md5(rel_path.encode()).hexdigest()


def classify(name):
    low = name.lower()
    for era, words in ERA_KEYWORDS:
        if any(w in low for w in words):
            return era
    return "sacred"


def rgb(hsv):
    r, g, b = colorsys.hsv_to_rgb(*hsv)
    return (int(r * 255), int(g * 255), int(b * 255))


def era_colors(name, era):
    """Base/dark/accent tinted to the era, jittered slightly per-name so each
    character is distinct while the era still reads."""
    (bh, bs, bv), accent = ERAS[era]
    j = ((name_hash(name) % 1000) / 1000.0 - 0.5) * 0.06  # +-0.03 hue jitter
    bh = (bh + j) % 1.0
    base = rgb((bh, bs, bv))
    dark = rgb((bh, min(bs + 0.12, 1), max(bv - 0.38, 0.12)))
    return base, dark, rgb(accent)


def initials(name):
    words = [w for w in name.replace(",", " ").split() if w[:1].isalpha()]
    skip = {"a", "the", "of", "in", "an", "to"}
    keep = [w for w in words if w.lower() not in skip] or words
    if len(keep) >= 2:
        return (keep[0][0] + keep[1][0]).upper()
    return keep[0][:2].capitalize() if keep else name[:2]


def display_name(name):
    n = name.split("(")[0].split(",")[0].strip()
    return n if n else name


def text_centered(d, cx, y, s, fnt, fill):
    bb = d.textbbox((0, 0), s, font=fnt)
    d.text((cx - (bb[2] - bb[0]) / 2, y), s, font=fnt, fill=fill)


def crown(d, cx, top, w, color):
    h = w * 0.5
    pts = [(cx - w, top), (cx - w * 0.5, top - h * 0.6), (cx, top),
           (cx + w * 0.5, top - h * 0.6), (cx + w, top),
           (cx + w * 0.7, top + h * 0.35), (cx - w * 0.7, top + h * 0.35)]
    d.polygon(pts, fill=color)


def save(img, directory, name):
    os.makedirs(directory, exist_ok=True)
    png = os.path.join(directory, name + ".png")
    img.save(png)
    rel = os.path.relpath(png, ROOT).replace(os.sep, "/")
    with open(png + ".meta", "w") as f:
        f.write(META_TEMPLATE.format(guid=guid_for(rel)))


def make_token(name, base, dark, accent, hostile=False, boss=False):
    S = 192
    img = Image.new("RGBA", (S, S), (0, 0, 0, 0))
    d = ImageDraw.Draw(img)
    cx, cy, r = S // 2, S // 2 - 6, 72
    if hostile:
        d.ellipse([cx - r - 7, cy - r - 7, cx + r + 7, cy + r + 7], fill=(150, 22, 26))
    d.ellipse([cx - r, cy - r, cx + r, cy + r], fill=dark)
    d.ellipse([cx - r + 7, cy - r + 7, cx + r - 7, cy + r - 7], fill=base)
    d.ellipse([cx - 17, cy - 38, cx + 17, cy - 4], fill=dark)          # head
    d.pieslice([cx - 38, cy - 6, cx + 38, cy + 66], 180, 360, fill=dark)  # shoulders
    text_centered(d, cx, cy + 6, initials(name), font(33), (255, 255, 255, 235))
    if boss:
        crown(d, cx, cy - r - 2, 22, accent)
    label = display_name(name)
    fnt = font(20)
    bb = d.textbbox((0, 0), label, font=fnt)
    if bb[2] - bb[0] > S - 14:
        fnt = font(15)
        bb = d.textbbox((0, 0), label, font=fnt)
    bw = bb[2] - bb[0]
    d.rounded_rectangle([cx - bw / 2 - 9, S - 28, cx + bw / 2 + 9, S - 2],
                        radius=6, fill=(18, 18, 22, 215))
    text_centered(d, cx, S - 27, label, fnt, (240, 240, 245))
    return img


def make_portrait(name, base, dark, accent):
    W, H = 256, 320
    img = Image.new("RGBA", (W, H), dark)
    d = ImageDraw.Draw(img)
    top = base
    bot = tuple(int(c * 0.4) for c in base)
    for y in range(H):
        t = y / H
        d.line([(0, y), (W, y)], fill=(int(top[0] * (1 - t) + bot[0] * t),
                                       int(top[1] * (1 - t) + bot[1] * t),
                                       int(top[2] * (1 - t) + bot[2] * t)))
    cx = W // 2
    sil = tuple(int(c * 0.45) for c in dark)
    d.ellipse([cx - 48, 70, cx + 48, 166], fill=sil)
    d.pieslice([cx - 95, 150, cx + 95, 330], 180, 360, fill=sil)
    d.ellipse([cx - 92, 250, cx + 92, 430], fill=sil)
    text_centered(d, cx, 192, initials(name), font(54), (255, 255, 255, 55))
    d.rectangle([2, 2, W - 3, H - 3], outline=accent, width=3)
    label = display_name(name)
    fnt = font(24)
    if d.textbbox((0, 0), label, font=fnt)[2] > W - 24:
        fnt = font(18)
    d.rectangle([0, H - 44, W, H], fill=(14, 14, 18, 235))
    d.line([(0, H - 44), (W, H - 44)], fill=accent, width=2)
    text_centered(d, cx, H - 36, label, fnt, (238, 238, 242))
    return img


def main():
    n = 0
    for fac, hsv in FACTIONS.items():
        base = rgb(hsv)
        dark = rgb((hsv[0], min(hsv[1] + 0.1, 1), max(hsv[2] - 0.4, 0.1)))
        save(make_token(fac, base, dark, (235, 235, 240), hostile=(fac == "Enemy")),
             SPRITES_DIR, fac)
        n += 1
    for c in COMPANIONS:
        b, dk, ac = era_colors(c, classify(c))
        save(make_token(c, b, dk, ac), SPRITES_DIR, c)
        n += 1
    for e in ENEMIES:
        b, dk, ac = era_colors(e, classify(e))
        save(make_token(e, b, dk, ac, hostile=True, boss=(e in BOSSES)), SPRITES_DIR, e)
        n += 1
    for s in SPEAKERS:
        b, dk, ac = era_colors(s, classify(s))
        save(make_portrait(s, b, dk, ac), PORTRAITS_DIR, s)
        n += 1
    print(f"Generated {n} era-themed placeholder assets (+ .meta) into Assets/Resources/")


if __name__ == "__main__":
    main()
