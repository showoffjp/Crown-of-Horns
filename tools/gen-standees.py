#!/usr/bin/env python3
"""
World standees for every soul in the game.

The Unity world skinned its NPC markers with the dialogue portraits, but those
are 320x400 *cards*: opaque, rectangular, with a painted gradient backdrop. Stood
up on a floor tile they read as little framed pictures floating over the ground —
still rectangles, just prettier ones.

This re-runs the very same portrait painter with its backdrop, vignette and grain
suppressed (tools/gen-portraits-v3.py, `standee=True`), so what comes out is the
figure alone on transparency: the same face, the same seeded palette, the same
archetype, cut out. Each is then given a dark keyline so it holds its shape
against a textured floor, cropped to the paint, and written with a bottom-centre
pivot so it stands on its tile instead of hovering over it.

Deterministic, and it does not touch Assets/Resources/Portraits.
Re-run: python3 tools/gen-standees.py
"""
import glob, hashlib, importlib.util, os
from collections import deque
from PIL import Image, ImageDraw, ImageFilter

ROOT = os.path.join(os.path.dirname(__file__), "..")
OUT = os.path.join(ROOT, "Assets", "Resources", "Standees")
PORTRAITS = os.path.join(ROOT, "Assets", "Resources", "Portraits")

CUT_TOL = 6      # per-channel step tolerance while following the backdrop gradient

def _painter():
    """gen-portraits-v3.py has dashes in its name, so import it by path."""
    path = os.path.join(os.path.dirname(__file__), "gen-portraits-v3.py")
    spec = importlib.util.spec_from_file_location("gen_portraits_v3", path)
    mod = importlib.util.module_from_spec(spec)
    spec.loader.exec_module(mod)
    return mod

KEY = (12, 10, 16, 225)

def cutout(im, tol=CUT_TOL):
    """Separate a painted portrait's figure from its backdrop.

    Needed for the portraits the v3 roster does not own: roughly thirty faces
    painted by the earlier generator, among them Naeve, Varra, Sister Garrow, Roen
    and Maerin — i.e. the main companions. Re-running the v3 painter on them would
    work, but it would also repaint them, and their v2 art is the better art.

    The backdrop is a smooth gradient with soft glows and a vignette; the figure is
    flat-shaded with hard edges. So flood from the border, stepping only between
    neighbouring pixels of near-identical colour: the gradient chains together,
    while the figure's edge is a cliff the flood cannot cross. The tolerance has to
    stay low — at 16 the flood walks straight through Roen's near-black robe and
    leaves a floating head.
    """
    im = im.convert("RGB")
    w, h = im.size
    px = im.load()
    bg = bytearray(w * h)
    q = deque()

    def seed(x, y):
        i = y * w + x
        if not bg[i]:
            bg[i] = 1
            q.append((x, y))

    for x in range(w):
        seed(x, 0)                      # the top edge is always backdrop
    for y in range(int(h * 0.55)):      # the sides, down to about the shoulders
        seed(0, y); seed(w - 1, y)

    while q:
        x, y = q.popleft()
        c = px[x, y]
        for nx, ny in ((x + 1, y), (x - 1, y), (x, y + 1), (x, y - 1)):
            if nx < 0 or ny < 0 or nx >= w or ny >= h:
                continue
            i = ny * w + nx
            if bg[i]:
                continue
            d = px[nx, ny]
            if abs(d[0] - c[0]) <= tol and abs(d[1] - c[1]) <= tol and abs(d[2] - c[2]) <= tol:
                bg[i] = 1
                q.append((nx, ny))

    alpha = Image.frombytes("L", (w, h), bytes(255 if not v else 0 for v in bg))
    # Drop the grain specks the backdrop's noise pass strands, then grow back to the
    # original edge so the figure does not come out shrunken.
    alpha = alpha.filter(ImageFilter.MinFilter(3)).filter(ImageFilter.MaxFilter(3))
    out = im.convert("RGBA")
    out.putalpha(alpha)
    return out

def keyline(img, width=3):
    """Lay a dilated dark silhouette under the figure. Without it a robe in the
    scene's own palette dissolves straight into the floor it is standing on."""
    grown = img.getchannel("A").filter(ImageFilter.MaxFilter(width * 2 + 1))
    out = Image.new("RGBA", img.size, (0, 0, 0, 0))
    out.paste(Image.new("RGBA", img.size, KEY), (0, 0), grown)
    out.alpha_composite(img)
    return out

def taper(img, start=0.72, end_width=0.44):
    """Narrow the figure toward its feet.

    The portrait painter draws a bust that flares outward and is cut off flat by
    the bottom of the card. Stood on a floor that silhouette reads as a slab. This
    masks the lower part of the standee into a wedge that closes toward the
    ground, so the same paint reads as a robed figure standing up, with a hem."""
    w, h = img.size
    mask = Image.new("L", (w, h), 255)
    md = ImageDraw.Draw(mask)
    y0 = int(h * start)
    cx = w / 2.0
    for y in range(y0, h):
        t = (y - y0) / max(1, h - 1 - y0)
        half = cx * (1.0 - (1.0 - end_width) * (t ** 1.35))
        md.rectangle([0, y, int(cx - half), y], fill=0)
        md.rectangle([int(cx + half), y, w, y], fill=0)
    a = img.getchannel("A").point(lambda v: v)
    out = img.copy()
    out.putalpha(Image.composite(a, Image.new("L", (w, h), 0), mask))
    return out

def crop_centred(img, pad=2):
    bb = img.getchannel("A").getbbox()
    if bb is None:
        return img
    x0, y0, x1, y1 = bb
    y0 = max(0, y0 - pad)
    y1 = min(img.height, y1 + pad)
    cx = img.width / 2.0                     # keep the figure under the pivot
    half = max(cx - (x0 - pad), (x1 + pad) - cx)
    return img.crop((int(cx - half), y0, int(cx + half), y1))

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
  maxTextureSize: 512
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
  spriteMeshType: 0
  alignment: 7
  spritePivot: {{x: 0.5, y: 0}}
  spritePixelsToUnits: 100
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

def write(name, img):
    path = os.path.join(OUT, name + ".png")
    crop_centred(keyline(taper(img))).save(path, optimize=True)
    rel = "Assets/Resources/Standees/" + name + ".png"
    with open(path + ".meta", "w") as f:
        f.write(META.format(guid=hashlib.md5(rel.encode()).hexdigest()))
    return os.path.getsize(path)

def main():
    gp = _painter()
    os.makedirs(OUT, exist_ok=True)
    souls = gp.roster()
    legacy = gp.legacy_names()
    total = painted = cut = 0

    for name, meta in sorted(souls.items()):
        art = os.path.join(PORTRAITS, name + ".png")
        # Mirror the portrait generator: a soul it only fills a gap for keeps the art
        # it already has, so the standee is cut from that painting rather than
        # repainted into a different one. Otherwise a companion would walk around the
        # world in colours their own dialogue portrait never uses.
        if name in legacy and os.path.exists(art):
            total += write(name, cutout(Image.open(art)))
            cut += 1
        else:
            total += write(name, gp.make_portrait(name, meta, standee=True))
            painted += 1

    # Anything else with a portrait on disk gets a standee too.
    for path in sorted(glob.glob(os.path.join(PORTRAITS, "*.png"))):
        name = os.path.splitext(os.path.basename(path))[0]
        if name in souls:
            continue
        total += write(name, cutout(Image.open(path)))
        cut += 1

    print(f"Wrote {painted + cut} world standees ({total // 1024} KB) into "
          f"Assets/Resources/Standees/ — {painted} repainted, {cut} cut from legacy portraits")

if __name__ == "__main__":
    main()
