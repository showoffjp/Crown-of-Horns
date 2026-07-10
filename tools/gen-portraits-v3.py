#!/usr/bin/env python3
"""
Portraits for the WHOLE cast — v3. Where v2 repainted the 37 legacy codex faces,
v3 reads every zone in play/*.json and paints a portrait for every named soul in
the game (257 at time of writing), keyed by the exact in-scene display name the
engine asks for (Resources/Portraits/<speaker>), so every face auto-wires into
DialogueScreen and the cast gallery.

What makes v3 honest to the game's own data:
  - each soul's zone-authored `hue` drives their palette (portraits match their
    in-game token color), and their `sigil` is stamped on a chest medallion;
  - archetype (priest/rogue/mage/warrior/noble/elf/spirit/commoner) is read from
    name+title keywords; spirits unravel, warriors get plate, nobles a circlet;
  - villainous presences (owners, accusers, collectors, advocates of the Nine)
    get the menace treatment — underlit, hard vignette, narrowed glints;
  - non-person souls (the fire, doors, ledgers, bells) are painted as emblem
    icons: their sigil held in a ring of their own light, not a fake face.

Deterministic per name (re-runs are stable). Legacy v2 files are left alone —
this paints alongside them. Honest placeholders still: for *real* paintings,
allowlist commons.wikimedia.org and run tools/fetch-portraits.py.
Re-run: python3 tools/gen-portraits-v3.py
"""
import colorsys, glob, hashlib, json, math, os, random
from PIL import Image, ImageDraw, ImageFilter, ImageFont

ROOT = os.path.join(os.path.dirname(__file__), "..")
OUT = os.path.join(ROOT, "Assets", "Resources", "Portraits")
W, H = 320, 400

FONT_PATHS = ["/usr/share/fonts/truetype/dejavu/DejaVuSans-Bold.ttf",
              "/usr/share/fonts/truetype/dejavu/DejaVuSans.ttf"]

def load_font(size):
    for p in FONT_PATHS:
        if os.path.exists(p):
            return ImageFont.truetype(p, size)
    return ImageFont.load_default()

# ---- roster -----------------------------------------------------------------
# Names the ENGINE asks for that never appear as zone npcs (tools/asset-check.sh)
EXTRAS = {
    "Vayle": {"title": "Doomguide of Kelemvor, with the church's last army",
              "hue": 45, "sigil": "⚖", "zone": "engine"},
}

def roster():
    souls = dict(EXTRAS)
    for p in sorted(glob.glob(os.path.join(ROOT, "play", "*.json"))):
        try:
            z = json.load(open(p))
        except Exception:
            continue
        if not isinstance(z, dict):
            continue
        sc = z.get("scene")
        if not isinstance(sc, dict):
            continue
        for n in sc.get("npcs", []):
            nm = (n.get("name") or "").strip()
            if nm and nm not in souls:
                souls[nm] = {"title": n.get("title", ""), "hue": n.get("hue", 40),
                             "sigil": n.get("sigil", "?"), "zone": sc.get("id", "")}
    return souls

# ---- classification ----------------------------------------------------------
def has(text, *keys):
    return any(k in text for k in keys)

def archetype(name, title):
    # NOTE: in the Fugue *everyone* is "a soul" — that word means nothing here.
    # Professions classify first; only genuine dissolution reads as spirit.
    t = (name + " " + title).lower()
    if has(t, "door", "threshold", "the fire", "ledger", "tally", "bell,", " bell", "kettle",
           "archive itself", "a quiet hour", "the wall itself", "loom", "the road itself",
           "milestone", "crown of horns", "became its walls"): return "icon"
    if has(t, "thief", "sparrow", "broker", "fence", "snitch", "smuggler", "con ", "grifter",
           "cutpurse", "spy", "informer", "pickpocket", "burglar", "the freest soul", "regis"): return "rogue"
    if has(t, "witch", "arcanist", "mage", "wizard", "scribe", "archivist", "scholar",
           "cartographer", "alchemist", "seer", "oracle", "chronicler", "raistlin"): return "mage"
    if has(t, "guard", "fist", "knight", "captain", "sergeant", "warden", "enforcer",
           "duelist", "soldier", "mercenary", "bruiser", "bouncer", "watch", "veteran",
           "iron", "collector of the causeway", "bailiff", "hunter", "the hunt",
           "keeps the ring", "wulfgar", "catti-brie", "bruenor", "drizzt", "barbarian"): return "warrior"
    if has(t, "judge", "arbiter", "keeper", "doomguide", "priest", "mother ", "brother ",
           "sister ", "cantor", "abbot", "vicar", "sexton", "chaplain", "confessor", "herald",
           "canon", "measurer", "accuser", "kelemvor", "jergal", "myrkul", "deathsong",
           "speaks for"): return "priest"
    if has(t, "lord", "lady", "king", "queen", "magistrate", "advocate", "master ",
           "maître", "patriar", "alderman", "chancellor", "factor", "high "): return "noble"
    if has(t, "elf", "elven"): return "elf"
    if has(t, "shade", "wraith", "ghost", "wisp", "echo", "chill", "hollow", "unmade",
           "revenant", "half-dissolved", "gone quiet", "mid-collection", "used to be someone",
           "newly-dead", "newly dead", "haunt", "presence", "memory of", "the wall took",
           "dissolving", "fading", "mid-unmaking"): return "spirit"
    return "commoner"

def menace(name, title):
    t = (name + " " + title).lower()
    return has(t, "owner", "owned him", "owned her", "accuser", "collector", "advocate of the nine",
               "devil", "reeled", "hunger", "maw", "butcher", "closing on its property", "hook",
               "wallmaker", "who speaks for the fear", "predator", "stalks", "debt", "on business",
               "bound clerk", "sifter", "gnaw", "smiling man", "claims the child", "stolen selves",
               "crown of horns", "myrkul")

# ---- palette from zone-authored hue -------------------------------------------
def hsv(h, s, v):
    r, g, b = colorsys.hsv_to_rgb((h % 360) / 360.0, s, v)
    return (int(r * 255), int(g * 255), int(b * 255))

def palette(hue, dark):
    return {"bg": hsv(hue, 0.42, 0.13 if dark else 0.18),
            "glow": hsv(hue, 0.52, 0.62 if dark else 0.78),
            "accent": hsv((hue + 36) % 360, 0.45, 0.82)}

# ---- paint helpers (v2 lineage) -----------------------------------------------
def vgrad(size, top, bottom):
    col = Image.new("RGB", (1, 256))
    for y in range(256):
        t = y / 255
        col.putpixel((0, y), tuple(int(top[i] * (1 - t) + bottom[i] * t) for i in range(3)))
    return col.resize(size)

def rgl(img, cx, cy, r, color, alpha):
    glow = Image.new("L", img.size, 0)
    d = ImageDraw.Draw(glow)
    for i in range(r, 0, -2):
        a = int(alpha * (1 - i / r) ** 2)
        d.ellipse([cx - i, cy - i, cx + i, cy + i], fill=a)
    layer = Image.new("RGB", img.size, color)
    img.paste(layer, (0, 0), glow)

def shade(c, f):
    return tuple(max(0, min(255, int(v * f))) for v in c)

def glyph_ok(font, ch):
    try:
        m = font.getmask(ch)
        return m.getbbox() is not None
    except Exception:
        return False

def draw_sigil(d, cx, cy, sigil, size, ink, halo=None):
    """sigil text centered at (cx,cy); diamond fallback if the font lacks the glyph"""
    font = load_font(size)
    if sigil and all(glyph_ok(font, ch) for ch in sigil):
        bb = d.textbbox((0, 0), sigil, font=font)
        tx, ty = cx - (bb[0] + bb[2]) / 2, cy - (bb[1] + bb[3]) / 2
        if halo:
            for ox, oy in ((-2, 0), (2, 0), (0, -2), (0, 2)):
                d.text((tx + ox, ty + oy), sigil, font=font, fill=halo)
        d.text((tx, ty), sigil, font=font, fill=ink)
    else:
        s = size * 0.42
        pts = [(cx, cy - s), (cx + s, cy), (cx, cy + s), (cx - s, cy)]
        d.polygon(pts, outline=ink, fill=halo)

def medallion(d, cx, cy, sigil, era):
    """chest brooch: shaded metal disc + the soul's sigil"""
    r = 17
    d.ellipse([cx - r, cy - r, cx + r, cy + r], fill=shade(era["glow"], 0.35),
              outline=shade(era["glow"], 1.1), width=2)
    d.ellipse([cx - r + 3, cy - r + 3, cx + r - 5, cy + r - 5], outline=shade(era["glow"], 0.7))
    draw_sigil(d, cx, cy - 1, sigil, 20, shade(era["glow"], 1.5))

# ---- the two painters ----------------------------------------------------------
def paint_icon(name, sigil, era, rnd):
    """non-person souls: the sigil held in a ring of its own light"""
    img = vgrad((W, H), shade(era["bg"], 1.5), shade(era["bg"], 0.5))
    cx, cy = W // 2, int(H * 0.44)
    for _ in range(5):
        rgl(img, rnd.randint(40, W - 40), rnd.randint(40, H - 60), rnd.randint(50, 130),
            era["glow"], rnd.randint(12, 26))
    rgl(img, cx, cy, 150, era["glow"], 60)
    d = ImageDraw.Draw(img, "RGBA")
    # ring
    for rr, wd, f in ((96, 6, 1.2), (112, 2, 0.7)):
        d.ellipse([cx - rr, cy - rr, cx + rr, cy + rr], outline=shade(era["glow"], f), width=wd)
    # orbiting motes
    for i in range(10):
        a = rnd.uniform(0, 2 * math.pi); rr = rnd.uniform(100, 132); s = rnd.randint(2, 4)
        x, y = cx + math.cos(a) * rr, cy + math.sin(a) * rr * 0.92
        d.ellipse([x - s, y - s, x + s, y + s], fill=(*era["accent"], rnd.randint(90, 180)))
    draw_sigil(d, cx, cy, sigil, 118, shade(era["glow"], 1.45), halo=(*shade(era["bg"], 2.2), 255))
    # plinth of light below
    d.polygon([(cx - 70, H - 40), (cx + 70, H - 40), (cx + 40, H - 24), (cx - 40, H - 24)],
              fill=(*shade(era["glow"], 0.35), 120))
    return img

def paint_bust(name, sigil, era, arch, dark, rnd):
    spirit = arch == "spirit"
    img = vgrad((W, H), shade(era["bg"], 1.4), shade(era["bg"], 0.55))
    for _ in range(4):
        rgl(img, rnd.randint(0, W), rnd.randint(0, H // 2), rnd.randint(60, 150),
            era["glow"], rnd.randint(14, 30))
    d = ImageDraw.Draw(img, "RGBA")

    cx, cy = W // 2, int(H * 0.42)
    hw, hh = 52 + rnd.randint(-4, 6), 66 + rnd.randint(-4, 6)
    lit = 1 if rnd.random() < 0.5 else -1

    if spirit:
        base = shade(era["accent"], 0.8); dk = shade(era["accent"], 0.35); hi = shade(era["accent"], 1.3)
    else:
        tone = rnd.choice([(224, 188, 154), (196, 156, 120), (162, 122, 92), (120, 88, 66), (210, 180, 160)])
        base, dk, hi = tone, shade(tone, 0.55), shade(tone, 1.25)
    if dark:  # menace: everything a step underlit
        base, hi = shade(base, 0.88), shade(hi, 0.9)

    sh_top = cy + hh - 8
    bust = [(cx - 118, H), (cx - 96, sh_top + 36), (cx - 48, sh_top + 6), (cx, sh_top - 2),
            (cx + 48, sh_top + 6), (cx + 96, sh_top + 36), (cx + 118, H)]
    garb = {"priest": (64, 60, 72), "rogue": (44, 48, 52), "mage": (52, 42, 70),
            "warrior": (70, 70, 78), "spirit": shade(era["accent"], 0.25),
            "elf": (58, 66, 60), "noble": (74, 52, 60), "commoner": (74, 64, 52)}[arch]
    # the soul's zone-authored hue reaches the garb — clothing IS identity here
    tint = era["glow"]
    garb = tuple(int(g * 0.55 + t * 0.32) for g, t in zip(garb, tint))
    d.polygon(bust, fill=shade(garb, 1.0))
    d.polygon([(cx - lit * 8, sh_top - 2), (cx - lit * 118, H), (cx - lit * 40, H)], fill=(*shade(garb, 0.6), 200))
    d.line([(cx + lit * 30, sh_top + 8), (cx + lit * 86, H)], fill=(*shade(era["glow"], 0.9), 110), width=4)
    if arch == "warrior":
        d.ellipse([cx + lit * 34 - 30, sh_top + 4, cx + lit * 34 + 42, sh_top + 60],
                  fill=shade(garb, 1.35), outline=shade(garb, 0.5))
    if arch == "noble":  # fur collar
        for i in range(16):
            a = math.pi + i * math.pi / 15
            x = cx + math.cos(a) * 92; y = sh_top + 26 + abs(math.sin(a)) * 10
            d.ellipse([x - 11, y - 11, x + 11, y + 11], fill=shade(garb, 1.5))

    d.rectangle([cx - 16, cy + hh - 26, cx + 16, sh_top + 8], fill=dk)
    d.ellipse([cx - hw, cy - hh, cx + hw, cy + hh], fill=base)
    d.ellipse([cx - hw + (0 if lit > 0 else 18), cy - hh + 6, cx + hw - (18 if lit > 0 else 0), cy + hh - 4], fill=dk)
    d.ellipse([cx - hw + (14 if lit > 0 else 26), cy - hh + 10, cx + hw - (26 if lit > 0 else 14), cy + hh - 10], fill=base)
    d.ellipse([cx - hw + (20 if lit > 0 else 44), cy - hh + 16, cx + hw - (44 if lit > 0 else 20), cy + hh - 22], fill=hi)

    ey = cy - 6
    d.ellipse([cx - 26, ey - 6, cx - 10, ey + 4], fill=shade(dk, 0.8))
    d.ellipse([cx + 10, ey - 6, cx + 26, ey + 4], fill=shade(dk, 0.8))
    glint = era["accent"] if spirit else (235, 235, 225)
    gy = 1 if dark else 0  # menace: narrowed glints
    d.ellipse([cx - 21, ey - 3 + gy, cx - 15, ey + 3 - gy], fill=glint)
    d.ellipse([cx + 15, ey - 3 + gy, cx + 21, ey + 3 - gy], fill=glint)
    d.line([(cx - 10, cy + 28), (cx + 10, cy + 28)], fill=shade(dk, 0.8), width=3)

    if arch == "priest":
        d.arc([cx - hw - 14, cy - hh - 18, cx + hw + 14, cy + hh + 30], 180, 360, fill=shade(garb, 1.2), width=26)
        d.pieslice([cx - hw - 10, cy - hh - 14, cx + hw + 10, cy + 4], 180, 360, fill=shade(garb, 1.1))
    elif arch == "mage":
        d.pieslice([cx - hw - 6, cy - hh - 8, cx + hw + 6, cy + 20], 180, 360, fill=shade(garb, 0.8))
        d.arc([cx - hw + 6, cy - hh + 2, cx + hw - 6, cy + 10], 200, 340, fill=era["glow"], width=5)
    elif arch == "warrior":  # helm: dome + brow band + nasal bar (off the eyes)
        d.pieslice([cx - hw - 8, cy - hh - 10, cx + hw + 8, cy + 6], 180, 360, fill=shade((90, 92, 100), 1.0))
        d.line([(cx - hw - 6, cy - 26), (cx + hw + 6, cy - 26)], fill=(60, 62, 70), width=7)
        d.line([(cx, cy - 26), (cx, cy + 6)], fill=(70, 72, 80), width=5)
    elif arch == "rogue":
        d.pieslice([cx - hw - 4, cy - hh - 6, cx + hw + 4, cy + 12], 180, 360, fill=(40, 36, 34))
        d.line([(cx + lit * 26, cy - 30), (cx + lit * 34, cy + 6)], fill=shade(dk, 0.7), width=3)
    elif arch == "noble":
        d.pieslice([cx - hw - 4, cy - hh - 8, cx + hw + 4, cy + 8], 180, 360, fill=(64, 50, 40))
        d.arc([cx - hw + 4, cy - hh, cx + hw - 4, cy + 8], 195, 345, fill=era["glow"], width=6)
        for i in range(3):
            x = cx - 22 + i * 22
            d.polygon([(x - 5, cy - hh + 10), (x, cy - hh - 2), (x + 5, cy - hh + 10)], fill=era["glow"])
    elif arch == "elf":
        d.pieslice([cx - hw - 4, cy - hh - 10, cx + hw + 4, cy + 8], 180, 360, fill=(208, 198, 170))
        d.polygon([(cx - hw - 2, cy - 4), (cx - hw - 16, cy - 16), (cx - hw + 4, cy + 8)], fill=base)
        d.polygon([(cx + hw + 2, cy - 4), (cx + hw + 16, cy - 16), (cx + hw - 4, cy + 8)], fill=base)
    elif spirit:
        for _ in range(7):
            a = rnd.uniform(math.pi, 2 * math.pi); r0 = rnd.uniform(hw * 0.9, hw * 1.5)
            d.ellipse([cx + math.cos(a) * r0 - 4, cy + math.sin(a) * r0 * 0.9 - 4,
                       cx + math.cos(a) * r0 + 4, cy + math.sin(a) * r0 * 0.9 + 4], fill=(*era["accent"], 150))
    else:  # plain hair — seeded color so the crowd is a crowd, not clones
        hair = rnd.choice([(70, 56, 44), (38, 34, 32), (120, 60, 36), (150, 140, 130),
                           (196, 168, 110), (90, 78, 60), (52, 48, 56)])
        d.pieslice([cx - hw - 2, cy - hh - 4, cx + hw + 2, cy + 10], 180, 360, fill=hair)

    rim = shade(era["glow"], 1.25) if not dark else shade(hsv(0, 0.55, 0.55), 1.0)
    d.arc([cx - hw - 2, cy - hh - 2, cx + hw + 2, cy + hh + 2],
          (300 if lit > 0 else 150), (30 if lit > 0 else 240), fill=rim, width=5)

    if spirit:
        for _ in range(40):
            x = rnd.randint(cx - 110, cx + 110); y = rnd.randint(int(H * 0.78), H - 6)
            s = rnd.randint(1, 4)
            d.ellipse([x - s, y - s, x + s, y + s], fill=(*shade(era["bg"], 1.6), rnd.randint(90, 200)))

    medallion(d, cx + lit * 44, sh_top + 42, sigil, era)
    return img

def finish(img, era, dark):
    """painterly grain + vignette + soft bloom (menace = harder vignette)"""
    noise = Image.effect_noise((W, H), 18).convert("L")
    img = Image.composite(img, Image.new("RGB", (W, H), (0, 0, 0)), noise.point(lambda v: 255 - (255 - v) // 6))
    vmask = Image.new("L", (W, H), 0); vd = ImageDraw.Draw(vmask)
    inset = 0.30 if not dark else 0.22
    vd.ellipse([-W * 0.35, -H * inset, W * 1.35, H * 1.25], fill=255)
    vmask = vmask.filter(ImageFilter.GaussianBlur(60 if not dark else 44))
    img = Image.composite(img, Image.new("RGB", (W, H), shade(era["bg"], 0.35)), vmask)
    return Image.blend(img, img.filter(ImageFilter.GaussianBlur(2)), 0.22)

def make_portrait(name, meta):
    seed = int(hashlib.md5(name.encode()).hexdigest(), 16)
    rnd = random.Random(seed)
    arch = archetype(name, meta["title"])
    dark = menace(name, meta["title"])
    era = palette(meta.get("hue", 40), dark)
    if arch == "icon":
        img = paint_icon(name, meta["sigil"], era, rnd)
    else:
        img = paint_bust(name, meta["sigil"], era, arch, dark, rnd)
    return finish(img, era, dark)

# ---- .meta (same convention as v2) ---------------------------------------------
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
    os.makedirs(OUT, exist_ok=True)
    souls = roster()
    counts = {}
    for name, meta in sorted(souls.items()):
        img = make_portrait(name, meta)
        # quantize: keeps the whole 257-face fleet a fraction of truecolor weight
        img = img.convert("P", palette=Image.ADAPTIVE, colors=128)
        path = os.path.join(OUT, name + ".png")
        img.save(path, optimize=True)
        rel = "Assets/Resources/Portraits/" + name + ".png"
        with open(path + ".meta", "w") as f:
            f.write(META.format(guid=hashlib.md5(rel.encode()).hexdigest()))
        counts[archetype(name, meta["title"])] = counts.get(archetype(name, meta["title"]), 0) + 1
    total_kb = sum(os.path.getsize(os.path.join(OUT, n + ".png")) for n in souls) // 1024
    by = ", ".join(f"{k}:{v}" for k, v in sorted(counts.items()))
    print(f"Painted {len(souls)} zone-soul portraits ({total_kb} KB total) into Assets/Resources/Portraits/")
    print(f"archetypes — {by}")

if __name__ == "__main__":
    main()
