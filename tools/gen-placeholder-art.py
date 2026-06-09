#!/usr/bin/env python3
"""
Generate labeled, era-tinted PLACEHOLDER art for the cast so the game renders its
characters instead of blank cubes, with zero inspector wiring.

Why this exists: WorldArt loads Resources/Sprites/<name> (map tokens) and
Resources/Portraits/<speaker> (dialogue busts), returning null -> placeholder cube
when missing. The project is in 3D default-behavior mode, so a committed *.meta with
textureType: 8 (Sprite) is required for Resources.Load<Sprite> to succeed on first
import. This script emits both the PNG and that .meta.

These are honest PLACEHOLDERS (deterministic color + initial + name banner), not final
art — drop real sprites with the same names to replace them. 100% locally generated,
so no license strings attached. Re-run any time: `python3 tools/gen-placeholder-art.py`.
"""
import colorsys
import hashlib
import os
from PIL import Image, ImageDraw, ImageFont

ROOT = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
SPRITES_DIR = os.path.join(ROOT, "Assets", "Resources", "Sprites")
PORTRAITS_DIR = os.path.join(ROOT, "Assets", "Resources", "Portraits")

# --- the cast, harvested from the dialogue/content layer -------------------------
FACTIONS = ["Player", "Ally", "Enemy", "Neutral"]
FACTION_HUE = {"Player": 0.58, "Ally": 0.33, "Enemy": 0.00, "Neutral": 0.0}
FACTION_SAT = {"Player": 0.55, "Ally": 0.50, "Enemy": 0.60, "Neutral": 0.0}

COMPANIONS = ["Sable", "Naeve", "Garrow", "Maerin", "Mhaere",
              "Ilfaeril", "Quill", "Tamsin", "Varra"]

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
        return ImageFont.load_default(size=size)  # Pillow >= 10
    except TypeError:
        return ImageFont.load_default()


def hue_for(name):
    h = int(hashlib.md5(name.encode()).hexdigest(), 16)
    return (h % 360) / 360.0


def guid_for(rel_path):
    return hashlib.md5(rel_path.encode()).hexdigest()  # 32 hex, stable & unique


def rgb(hue, sat, val):
    r, g, b = colorsys.hsv_to_rgb(hue, sat, val)
    return (int(r * 255), int(g * 255), int(b * 255))


def initials(name):
    words = [w for w in name.replace(",", " ").split() if w[:1].isalpha()]
    skip = {"a", "the", "of", "in", "an"}
    words = [w for w in words if w.lower() not in skip] or words
    if len(words) >= 2:
        return (words[0][0] + words[1][0]).upper()
    return name[:2].capitalize()


def display_name(name):
    n = name.split("(")[0].split(",")[0].strip()
    return n if n else name


def text_centered(d, cx, y, s, fnt, fill):
    bb = d.textbbox((0, 0), s, font=fnt)
    d.text((cx - (bb[2] - bb[0]) / 2, y), s, font=fnt, fill=fill)
    return bb[3] - bb[1]


def save(img, directory, name):
    os.makedirs(directory, exist_ok=True)
    png = os.path.join(directory, name + ".png")
    img.save(png)
    rel = os.path.relpath(png, ROOT).replace(os.sep, "/")
    with open(png + ".meta", "w") as f:
        f.write(META_TEMPLATE.format(guid=guid_for(rel)))


def make_token(name, hue, sat=0.5):
    """Map unit token: a coin with the figure's initials, name on a banner."""
    S = 192
    img = Image.new("RGBA", (S, S), (0, 0, 0, 0))
    d = ImageDraw.Draw(img)
    cx, cy, r = S // 2, S // 2 - 8, 74
    base = rgb(hue, sat, 0.85 if sat else 0.0, ) if sat else (150, 150, 155)
    dark = rgb(hue, min(sat + 0.1, 1), 0.45) if sat else (70, 70, 75)
    d.ellipse([cx - r, cy - r, cx + r, cy + r], fill=dark)
    d.ellipse([cx - r + 7, cy - r + 7, cx + r - 7, cy + r - 7], fill=base)
    # simple figure: head + shoulders
    d.ellipse([cx - 18, cy - 40, cx + 18, cy - 4], fill=dark)
    d.pieslice([cx - 40, cy - 6, cx + 40, cy + 70], 180, 360, fill=dark)
    text_centered(d, cx, cy + 8, initials(name), font(34), (255, 255, 255, 235))
    # name banner
    label = display_name(name)
    fnt = font(22)
    bb = d.textbbox((0, 0), label, font=fnt)
    bw = bb[2] - bb[0]
    d.rounded_rectangle([cx - bw / 2 - 10, S - 30, cx + bw / 2 + 10, S - 2],
                        radius=6, fill=(20, 20, 24, 210))
    text_centered(d, cx, S - 28, label, fnt, (240, 240, 245))
    return img


def make_portrait(name):
    """Dialogue bust: tinted gradient, silhouette, name plate."""
    W, H = 256, 320
    hue = hue_for(name)
    img = Image.new("RGBA", (W, H), (0, 0, 0, 255))
    d = ImageDraw.Draw(img)
    top = rgb(hue, 0.45, 0.55)
    bot = rgb(hue, 0.55, 0.22)
    for y in range(H):  # vertical gradient
        t = y / H
        d.line([(0, y), (W, y)],
               fill=(int(top[0] * (1 - t) + bot[0] * t),
                     int(top[1] * (1 - t) + bot[1] * t),
                     int(top[2] * (1 - t) + bot[2] * t)))
    # bust silhouette
    sil = rgb(hue, 0.30, 0.14)
    cx = W // 2
    d.ellipse([cx - 48, 70, cx + 48, 166], fill=sil)          # head
    d.pieslice([cx - 95, 150, cx + 95, 330], 180, 360, fill=sil)  # shoulders
    d.ellipse([cx - 92, 250, cx + 92, 430], fill=sil)         # body fill
    # initials on the chest
    text_centered(d, cx, 196, initials(name), font(56), (255, 255, 255, 60))
    # frame
    d.rectangle([2, 2, W - 3, H - 3], outline=rgb(hue, 0.25, 0.85), width=3)
    # name plate
    label = display_name(name)
    fnt = font(24)
    if d.textbbox((0, 0), label, font=fnt)[2] > W - 24:
        fnt = font(18)
    d.rectangle([0, H - 44, W, H], fill=(14, 14, 18, 235))
    d.line([(0, H - 44), (W, H - 44)], fill=rgb(hue, 0.3, 0.8), width=2)
    text_centered(d, cx, H - 36, label, fnt, (238, 238, 242))
    return img


def main():
    n = 0
    for fac in FACTIONS:
        save(make_token(fac, FACTION_HUE[fac], FACTION_SAT[fac]), SPRITES_DIR, fac)
        n += 1
    for c in COMPANIONS:
        save(make_token(c, hue_for(c), 0.5), SPRITES_DIR, c)
        n += 1
    for s in SPEAKERS:
        save(make_portrait(s), PORTRAITS_DIR, s)
        n += 1
    print(f"Generated {n} placeholder assets (+ .meta) into Assets/Resources/")


if __name__ == "__main__":
    main()
