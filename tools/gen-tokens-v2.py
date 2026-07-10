#!/usr/bin/env python3
"""
Battle tokens v2 — every unit the engine can put on the grid, painted with the
same identity system as the v3 portraits, sourced from the game's own data:

  - the web combat roster's authored col+glyph (play/crown_combat.html) for the
    six units that have one — Garrow's gold ✛, the Returned's blood ☠, the Last
    Returned's violet ♛;
  - the Combatant.color tints authored inline in the encounter builders
    (Assets/Scripts/Core/*.cs) for 25 enemies;
  - Ilfaeril's zone-authored hue/sigil (play/naming.json);
  - v1's era palettes (gen-placeholder-art.py, imported — not duplicated) for
    everything else, with keyword-derived sigils.

Closes the roster gaps v1 left: "The Returned" and "The Last Returned" (the
protagonist and the final mirror — the two names tools/asset-check.sh has been
yellow about), Avatar-Touched Horror, Doomguide Acolyte, Ashfiend A/B, Lyra,
Brother Oke, exact-name companion tokens ("Sister Garrow", "Roen Alleywind" —
superseding the alias copies), and an "Echo" token that catches every dynamic
"Echo of <name>" mirror-clone via UnitSpriteSkinner's first-word fallback.

Same contract as v1: 192x192 RGBA PNG into Assets/Resources/Sprites/<Name>.png
with a committed .meta (deterministic guid, textureType 8), hostile red threat
rim, boss crown, name plate. Deterministic per name. Honest placeholders.
Re-run: python3 tools/gen-tokens-v2.py
"""
import colorsys, importlib.util, os, re

ROOT = os.path.join(os.path.dirname(__file__), "..")
OUT = os.path.join(ROOT, "Assets", "Resources", "Sprites")

_spec = importlib.util.spec_from_file_location(
    "genart", os.path.join(os.path.dirname(__file__), "gen-placeholder-art.py"))
v1 = importlib.util.module_from_spec(_spec)
_spec.loader.exec_module(v1)
from PIL import Image, ImageDraw  # noqa: E402  (after v1 import; same PIL)

# ---- identity sources (priority order) --------------------------------------
def hexrgb(h):
    h = h.lstrip("#")
    return tuple(int(h[i:i + 2], 16) for i in (0, 2, 4))

# 1) web combat roster: authored col + glyph (play/crown_combat.html roster())
WEB = {
    "Sister Garrow":     (hexrgb("#e7c873"), "✛"),   # ✛
    "Roen Alleywind":    (hexrgb("#7fd0a0"), "⚔"),   # ⚔
    "Varra":             (hexrgb("#6fa8dc"), "⛏"),   # ⛏
    "Naeve":             (hexrgb("#c79bf0"), "✶"),   # ✶
    "The Returned":      (hexrgb("#9b2d2d"), "☠"),   # ☠
    "The Last Returned": (hexrgb("#b06fd0"), "♛"),   # ♛
}

# 2) zone-authored identity (play/naming.json). S/V follow gen-portraits-v3's
#    glow formula so the one zone-authored token matches its portrait palette.
def hsv255(h, s, v):
    r, g, b = colorsys.hsv_to_rgb((h % 360) / 360.0, s, v)
    return (int(r * 255), int(g * 255), int(b * 255))

ZONE = {"Ilfaeril": (hsv255(265, 0.52, 0.62), "†")}  # †

# 3) tints authored at Enemy() call sites through VARIABLES, invisible to the
#    regex below — read from the source by hand (cite the exact lines)
AUTHORED = {
    # CampaignBootstrap.cs:934-939 — Time of Troubles branch, new Color(0.7f,0.6f,0.3f)
    "Avatar-Touched Horror": (178, 153, 76),
}

# 4) Combatant.color tints authored in the encounter builders (Core/*.cs)
def campaign_tints():
    tints = dict(AUTHORED)
    import glob
    for p in sorted(glob.glob(os.path.join(ROOT, "Assets", "Scripts", "Core", "*.cs"))):
        src = open(p, encoding="utf-8").read()
        for m in re.finditer(
                r'Enemy\(\s*"([^"]+)"[^;]*?new Color\(([\d.f]+),\s*([\d.f]+),\s*([\d.f]+)', src):
            rgb = tuple(round(float(x.rstrip("f")) * 255) for x in m.groups()[1:4])
            tints.setdefault(m.group(1), rgb)
    return tints

# 5) keyword-derived sigils for enemies with no authored glyph
SIGIL_KEYS = [
    ("⚖", ["doomguide", "justiciar", "templar", "kelemvorite"]),      # ⚖
    ("✶", ["arcanist", "arcane", "weave", "mythallar", "netherese"]), # ✶
    ("∅", ["unmade", "unmaking", "unbound"]),                         # ∅
    ("♪", ["cantor", "choir"]),                                       # ♪
    ("☽", ["shade", "wraith", "sorrow", "echo", "revenant"]),         # ☽
    ("☠", ["myrkul", "bone", "avatar", "horror"]),                    # ☠
    ("✒", ["broker", "quill"]),                                       # ✒
    ("§", ["devil", "imp", "fine print", "contract"]),                # §
    ("✸", ["cinder", "ashfiend", "fire"]),                            # ✸
    ("⊗", ["construct", "sentinel", "ward", "colossus"]),             # ⊗
]
def sigil_for(name):
    n = name.lower()
    for g, keys in SIGIL_KEYS:
        if any(k in n for k in keys):
            return g
    return "◆"                                                        # ◆

# ---- glyph coverage: DejaVu renders missing glyphs as a .notdef box whose
# bbox is nonzero, so bbox checks can't catch tofu — compare rendered bytes
# against the guaranteed-missing U+E000 instead. Authored glyphs outside
# DejaVu (Varra's ⛏, U+26CF) substitute to a covered cousin.
SUBSTITUTES = {"⛏": "✸", "⏳": "∞", "⟁": "△"}
_COV_FONT = None
_NOTDEF = None
def _render_glyph(ch):
    img = Image.new("L", (72, 72), 0)
    ImageDraw.Draw(img).text((4, 4), ch, font=_COV_FONT, fill=255)
    return img.tobytes()

def covered(ch):
    global _COV_FONT, _NOTDEF
    if _COV_FONT is None:
        _COV_FONT = v1.font(40)
        try:
            _NOTDEF = _render_glyph("")
        except Exception:
            _NOTDEF = b""
    try:
        r = _render_glyph(ch)
    except Exception:
        return False
    return any(r) and r != _NOTDEF

def safe_glyph(g, name):
    for cand in (g, SUBSTITUTES.get(g), sigil_for(name), "◆"):
        if cand and covered(cand):
            return cand
    return None

# ---- roster ------------------------------------------------------------------
# Every static name the spawners use (CampaignBootstrap / Prologue / Demo /
# Mirror / Netheril / Spellplague bootstraps), plus v1's short names (firstWord
# fallback + gallery continuity), plus "Echo" for dynamic mirror-clones.
NEW_UNITS = ["The Returned", "The Last Returned", "Avatar-Touched Horror",
             "Doomguide Acolyte", "Ashfiend A", "Ashfiend B", "Lyra",
             "Brother Oke", "Echo", "Sister Garrow", "Roen Alleywind"]
# NOTE: "Echo" is NOT a hero — Echo.png exists to catch the dynamic
# "Echo of <name>" mirror-clones, and every one of those spawns Faction.Enemy
# (MirrorEncounterDemo.cs:64), so the fallback token keeps the threat rim.
HEROES = {"The Returned", "Sister Garrow", "Roen Alleywind", "Varra", "Naeve",
          "Ilfaeril", "Maerin", "Sable", "Lyra", "Brother Oke",
          *v1.COMPANIONS}
# v1 crowned two trash adds: God-Touched Horror (180-HP adds in the Avatar
# fight, CampaignBootstrap.cs:957) and The Unbound (50-HP adds; the dungeon's
# real boss is The Unbound Maw). Crowns belong to bosses only.
BOSSES = (set(v1.BOSSES) | {"The Last Returned"}) - {"God-Touched Horror", "The Unbound"}

def all_units():
    names = list(dict.fromkeys(v1.ENEMIES + v1.COMPANIONS + NEW_UNITS))
    return names

# ---- painter -----------------------------------------------------------------
def shade(c, f):
    return tuple(max(0, min(255, int(v * f))) for v in c)

def make_token_v2(name, base, hostile, boss, glyph):
    S = 192
    img = Image.new("RGBA", (S, S), (0, 0, 0, 0))
    d = ImageDraw.Draw(img)
    cx, cy, r = S // 2, S // 2 - 6, 72
    dark, lite = shade(base, 0.42), shade(base, 1.35)

    if hostile:  # v1 contract: red threat rim
        d.ellipse([cx - r - 7, cy - r - 7, cx + r + 7, cy + r + 7], fill=(150, 22, 26))

    # bevelled ring: dark disc, then a light arc (key light upper-left)
    d.ellipse([cx - r, cy - r, cx + r, cy + r], fill=dark)
    d.arc([cx - r + 2, cy - r + 2, cx + r - 2, cy + r - 2], 120, 330, fill=lite, width=4)

    # radial-shaded face of the disc: stacked ellipses dark->base->lit toward
    # the upper-left key light (cheap radial gradient, no numpy)
    steps = 12
    for i in range(steps):
        t = i / (steps - 1)
        rr = int((r - 7) * (1 - 0.55 * t))
        col = tuple(int(dark[j] * (1 - t) + base[j] * t) for j in range(3))
        d.ellipse([cx - rr - int(6 * t) - 0, cy - rr - int(8 * t),
                   cx + rr - int(6 * t), cy + rr - int(8 * t)], fill=col)

    # bust silhouette (dark against the lit disc) + rim light on its lit edge
    d.ellipse([cx - 17, cy - 38, cx + 17, cy - 4], fill=dark)
    d.pieslice([cx - 38, cy - 6, cx + 38, cy + 66], 180, 360, fill=dark)
    d.arc([cx - 17, cy - 38, cx + 17, cy - 4], 150, 300, fill=lite, width=3)

    # big sigil watermark behind the initials, low alpha — identity at a glance
    # (try/except: a bitmap fallback font raises UnicodeEncodeError on non-latin
    # glyphs before any bbox guard can run — degrade to no watermark, not a crash)
    if glyph:
        try:
            overlay = Image.new("RGBA", (S, S), (0, 0, 0, 0))
            od = ImageDraw.Draw(overlay)
            gf = v1.font(88)
            bb = od.textbbox((0, 0), glyph, font=gf)
            if bb[2] - bb[0] > 0:
                od.text((cx - (bb[0] + bb[2]) / 2, cy - (bb[1] + bb[3]) / 2 - 2),
                        glyph, font=gf, fill=(*lite, 64))
                img = Image.alpha_composite(img, overlay)
                d = ImageDraw.Draw(img)
        except UnicodeEncodeError:
            pass

    v1.text_centered(d, cx, cy - 12, v1.initials(name), v1.font(33), (255, 255, 255, 240))

    # sigil badge at the disc's foot (portrait-medallion language)
    if glyph:
        br = 15
        bx, by = cx, cy + r - 18
        d.ellipse([bx - br, by - br, bx + br, by + br], fill=shade(dark, 0.8),
                  outline=lite, width=2)
        try:
            gf = v1.font(18)
            bb = d.textbbox((0, 0), glyph, font=gf)
            if bb[2] - bb[0] > 0:
                d.text((bx - (bb[0] + bb[2]) / 2, by - (bb[1] + bb[3]) / 2 - 1),
                       glyph, font=gf, fill=lite)
        except UnicodeEncodeError:
            pass

    if boss:
        v1.crown(d, cx, cy - r - 2, 22, lite)

    # name plate (v1 contract; shrink until it fits — v1 clipped at 15pt)
    label = v1.display_name(name)
    for size in (20, 17, 15, 13, 11):
        fnt = v1.font(size)
        bb = d.textbbox((0, 0), label, font=fnt)
        if bb[2] - bb[0] <= S - 14:
            break
    bw = bb[2] - bb[0]
    d.rounded_rectangle([cx - bw / 2 - 9, S - 28, cx + bw / 2 + 9, S - 2],
                        radius=6, fill=(18, 18, 22, 215))
    v1.text_centered(d, cx, S - 27, label, fnt, (240, 240, 245))
    return img

def base_color(name, tints):
    if name in WEB: return WEB[name][0]
    if name in ZONE: return ZONE[name][0]
    if name in tints: return tints[name]
    era = v1.classify(name)
    b, _d, _a = v1.era_colors(name, era)
    return b

def glyph_of(name):
    if name in WEB: return safe_glyph(WEB[name][1], name)
    if name in ZONE: return safe_glyph(ZONE[name][1], name)
    return safe_glyph(sigil_for(name), name)

def main():
    tints = campaign_tints()
    units = all_units()
    for name in units:
        hostile = name not in HEROES and name not in v1.FACTIONS
        img = make_token_v2(name, base_color(name, tints), hostile,
                            name in BOSSES, glyph_of(name))
        v1.save(img, OUT, name)
    # faction tokens keep gameplay colors; Enemy keeps the threat rim
    for f, hsv in v1.FACTIONS.items():
        img = make_token_v2(f, v1.rgb(hsv), f == "Enemy", False, None)
        v1.save(img, OUT, f)
    src = {"web": len(WEB), "zone": len(ZONE), "campaign tints": len(tints)}
    print(f"Painted {len(units) + len(v1.FACTIONS)} battle tokens (v2) into Assets/Resources/Sprites/")
    print("identity sources — " + ", ".join(f"{k}:{v}" for k, v in src.items())
          + f"; era fallback for the rest. Bosses crowned: {len(BOSSES & set(units))}.")

if __name__ == "__main__":
    main()
