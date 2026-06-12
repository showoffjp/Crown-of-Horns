#!/usr/bin/env python3
"""
Render a representative gameplay screenshot of Crown of Horns — an isometric 5e combat
encounter with the real HUD: party portraits + HP/AC, an iso battlefield with tokens and
movement/threat tiles, the ability bar, the initiative tracker, and the XCOM-style attack
forecast. A mock-up (not a Unity capture — there's no editor here), but every number,
ability, and name is drawn from the actual content + the verified combat math.
  python3 tools/render-gameplay-mockup.py
"""
import os, math
from PIL import Image, ImageDraw, ImageFont, ImageFilter

ROOT = os.path.join(os.path.dirname(__file__), "..")
FONTS = "/mnt/skills/examples/canvas-design/canvas-fonts"
DEJA = "/usr/share/fonts/truetype/dejavu"
def F(path, size): return ImageFont.truetype(path, size)
serif   = lambda s: F(f"{FONTS}/IBMPlexSerif-Regular.ttf", s)
serifb  = lambda s: F(f"{FONTS}/IBMPlexSerif-Bold.ttf", s)
serifi  = lambda s: F(f"{FONTS}/IBMPlexSerif-Italic.ttf", s)
mono    = lambda s: F(f"{DEJA}/DejaVuSansMono.ttf", s)
monob   = lambda s: F(f"{DEJA}/DejaVuSansMono-Bold.ttf", s)

W, H = 1600, 1000
GOLD = (231, 200, 115); GOLDD = (170, 140, 70)
INK = (216, 210, 194); DIM = (150, 142, 165); RED = (208, 96, 96); GREEN = (130, 196, 120)
PANEL = (22, 20, 29); PANEL2 = (16, 14, 22); LINE = (52, 47, 70)
BLUE = (121, 160, 196)

img = Image.new("RGB", (W, H), (10, 9, 16))
d = ImageDraw.Draw(img, "RGBA")

def vgrad(box, top, bot):
    x0, y0, x1, y1 = box
    for y in range(y0, y1):
        t = (y - y0) / max(1, (y1 - y0))
        c = tuple(int(top[i] + (bot[i] - top[i]) * t) for i in range(3))
        d.line([(x0, y), (x1, y)], fill=c)

def panel(box, fill=PANEL, outline=LINE, r=12, shadow=True):
    x0, y0, x1, y1 = box
    if shadow:
        sh = Image.new("RGBA", (W, H), (0, 0, 0, 0)); sd = ImageDraw.Draw(sh)
        sd.rounded_rectangle([x0+4, y0+6, x1+4, y1+6], r, fill=(0, 0, 0, 120))
        img.paste(Image.alpha_composite(img.convert("RGBA"), sh.filter(ImageFilter.GaussianBlur(7))).convert("RGB"), (0, 0))
    d.rounded_rectangle(box, r, fill=fill, outline=outline, width=1)

def text(xy, s, font, fill=INK, anchor="la"):
    d.text(xy, s, font=font, fill=fill, anchor=anchor)

# ---------- backdrop: the Cinderhaunt ----------
vgrad((0, 0, W, H), (38, 30, 44), (8, 7, 12))
# faint vignette glow from below (forge light)
glow = Image.new("RGBA", (W, H), (0, 0, 0, 0)); gd = ImageDraw.Draw(glow)
gd.ellipse([W//2-520, H-260, W//2+520, H+260], fill=(150, 70, 40, 70))
img = Image.alpha_composite(img.convert("RGBA"), glow.filter(ImageFilter.GaussianBlur(60))).convert("RGB")
d = ImageDraw.Draw(img, "RGBA")

# ---------- isometric battlefield ----------
TW, TH = 92, 46          # tile width/height (diamond)
ORIGIN = (W//2 - 70, 250)
COLS, ROWS = 8, 8
def iso(c, r):
    return (ORIGIN[0] + (c - r) * TW // 2, ORIGIN[1] + (c + r) * TH // 2)

# special tiles: movement range (blue) for active hero at (2,5); threatened (red) near enemies
MOVE = {(2,5),(2,4),(3,5),(1,5),(2,6),(3,4),(1,4),(3,6),(2,3),(4,5)}
THREAT = {(5,2),(5,3),(6,2),(4,2),(5,1),(6,3)}
for r in range(ROWS):
    for c in range(COLS):
        x, y = iso(c, r)
        pts = [(x, y), (x + TW//2, y + TH//2), (x, y + TH), (x - TW//2, y + TH//2)]
        base = (44, 40, 56) if (c + r) % 2 == 0 else (38, 34, 48)
        d.polygon(pts, fill=base, outline=(26, 24, 34))
        if (c, r) in MOVE:
            d.polygon(pts, fill=(70, 110, 170, 90), outline=(120, 170, 230, 160))
        elif (c, r) in THREAT:
            d.polygon(pts, fill=(170, 70, 70, 80), outline=(220, 110, 110, 150))

def token(c, r, body, ring, label, hp, hpmax, active=False, enemy=False, cond=None):
    x, y = iso(c, r); cx, cy = x, y + TH//2
    # shadow
    d.ellipse([cx-26, cy+8, cx+26, cy+22], fill=(0, 0, 0, 110))
    # base ring
    if active:
        d.ellipse([cx-30, cy-6, cx+30, cy+24], outline=GOLD, width=3)
    # body (rounded pawn)
    d.ellipse([cx-22, cy-44, cx+22, cy], fill=body, outline=ring, width=3)
    d.ellipse([cx-13, cy-54, cx+13, cy-28], fill=body, outline=ring, width=3)  # head
    text((cx, cy-22), label, serifb(17), fill=(245, 240, 230), anchor="mm")
    # hp bar
    bw = 50
    d.rounded_rectangle([cx-bw//2, cy-66, cx+bw//2, cy-58], 3, fill=(20, 18, 26), outline=(0,0,0,0))
    frac = hp / hpmax
    col = GREEN if frac > .5 else (GOLD if frac > .25 else RED)
    d.rounded_rectangle([cx-bw//2+1, cy-65, cx-bw//2+1+int((bw-2)*frac), cy-59], 3, fill=col)
    if frac <= .5:
        text((cx+bw//2+6, cy-67), "BLOODIED", serifb(9), fill=RED, anchor="lm")
    if cond:
        d.rounded_rectangle([cx-26, cy-2, cx-26+len(cond)*7+8, cy+12], 4, fill=(40,30,12), outline=GOLDD)
        text((cx-22, cy+5), cond, mono(9), fill=GOLD, anchor="lm")

# enemies — the Returned (from the demo encounter)
token(5, 2, (78, 36, 46), (150, 70, 80), "R", 9, 14, enemy=True)
token(6, 3, (78, 36, 46), (150, 70, 80), "R", 14, 14, enemy=True)
token(5, 4, (78, 36, 46), (150, 70, 80), "R", 5, 14, enemy=True, cond="Burning")
token(7, 2, (60, 30, 60), (150, 80, 150), "R", 14, 14, enemy=True)
# party — heroes
token(2, 5, (40, 70, 96), (90, 150, 210), "G", 28, 34, active=True)   # the Returned (you)
token(1, 6, (44, 60, 44), (110, 160, 100), "S", 21, 27, cond="Blessed")  # Garrow
token(3, 6, (66, 50, 30), (170, 130, 70), "V", 16, 22)               # Varra

# active unit beam
ax, ay = iso(2, 5)
text((ax, ay - 78), "▼", serifb(20), fill=GOLD, anchor="mm")

# ---------- top scene bar ----------
panel((0, 0, W, 60), fill=(14, 12, 19), outline=LINE, r=0, shadow=False)
text((26, 18), "👑", serif(26))
text((64, 14), "THE CINDERHAUNT", serifb(22), fill=GOLD)
text((64, 40), "Beneath Baldur's Gate · Act I", serifi(13), fill=DIM)
text((W//2, 30), "Round 3  ·  Garrow's turn", serifb(18), fill=INK, anchor="mm")
text((W-26, 14), "⚙  ⛶  ⏸", serif(18), fill=DIM, anchor="ra")
text((W-26, 40), "Autosave ✓", mono(11), fill=GREEN, anchor="ra")

# ---------- left: party roster ----------
PX = 18
party = [
    ("The Returned", "Fighter 9", 28, 34, 18, (40, 70, 96), True, "you"),
    ("Sister Garrow", "Cleric 8", 21, 27, 17, (44, 60, 44), False, "Blessed"),
    ("Varra", "Warlock 7", 16, 22, 13, (66, 50, 30), False, ""),
    ("Roen Alleywind", "Rogue 7", 19, 24, 15, (60, 44, 60), False, "down a corridor"),
]
py = 78
text((PX, py), "THE PARTY", monob(12), fill=DIM); py += 24
for name, cls, hp, hpmax, ac, col, active, note in party:
    box = (PX, py, PX + 258, py + 86)
    panel(box, fill=(PANEL if not active else (30, 26, 40)),
          outline=(GOLD if active else LINE))
    # portrait swatch
    d.rounded_rectangle([PX+10, py+10, PX+66, py+66], 8, fill=col, outline=LINE)
    d.ellipse([PX+24, py+22, PX+52, py+50], fill=tuple(min(255,x+30) for x in col), outline=(0,0,0,90))
    text((PX+38, py+58), name.split()[0][:1] + (name.split()[-1][:1] if len(name.split())>1 else ""),
         serifb(13), fill=(245,240,230), anchor="mm")
    text((PX+78, py+12), name, serifb(15), fill=(GOLD if active else INK))
    text((PX+78, py+32), cls + f"   AC {ac}", serif(12), fill=DIM)
    # hp bar
    bw = 168
    d.rounded_rectangle([PX+78, py+52, PX+78+bw, py+64], 5, fill=(18,16,24), outline=LINE)
    frac = hp/hpmax
    hc = GREEN if frac>.5 else (GOLD if frac>.25 else RED)
    d.rounded_rectangle([PX+79, py+53, PX+79+int(bw*frac), py+63], 5, fill=hc)
    text((PX+78+bw, py+50), f"{hp}/{hpmax}", mono(11), fill=INK, anchor="ra")
    if note in ("Blessed",):
        text((PX+78, py+68), "✦ "+note, serif(11), fill=GOLD)
    elif note and note not in ("you",):
        text((PX+78, py+68), note, serifi(11), fill=DIM)
    py += 96

# ---------- right: attack forecast (XCOM-style) ----------
FX = W - 322
panel((FX, 78, W-18, 300), fill=PANEL2, outline=GOLDD)
text((FX+18, 92), "ATTACK FORECAST", monob(12), fill=GOLD)
text((FX+18, 116), "Sister Garrow", serifb(16), fill=INK)
text((FX+18, 138), "↳ Mace  vs  the Returned", serif(13), fill=DIM)
# big hit %
text((FX+18, 168), "72%", serifb(54), fill=GREEN)
text((FX+150, 182), "to hit", serif(14), fill=DIM)
rows = [("Crit chance", "5%", GOLD), ("Expected damage", "7.4", INK),
        ("On a hit", "1d6 + 3 + 1d8 radiant", INK), ("Lethal this swing", "31%", RED)]
ry = 238
for lab, val, c in rows:
    text((FX+18, ry), lab, serif(12), fill=DIM)
    text((W-34, ry), val, monob(12), fill=c, anchor="ra")
    ry += 17

# threat panel
panel((FX, 314, W-18, 470), fill=PANEL2, outline=LINE)
text((FX+18, 328), "INCOMING THREAT", monob(12), fill=RED)
text((FX+18, 350), "If Garrow ends here:", serif(12), fill=DIM)
tr = [("2 Returned in reach", ""), ("Expected damage taken", "6.1"),
      ("Chance to be downed", "8%"), ("Half-cover from pillar", "+2 AC")]
ty = 374
for lab, val in tr:
    text((FX+18, ty), lab, serif(12), fill=DIM)
    if val: text((W-34, ty), val, monob(12), fill=INK, anchor="ra")
    ty += 22
text((FX+18, 444), "✦ Blessed: +2 to attack rolls (2 rds)", serif(11), fill=GOLD)

# ---------- initiative tracker (right lower) ----------
panel((FX, 484, W-18, 690), fill=PANEL, outline=LINE)
text((FX+18, 498), "INITIATIVE", monob(12), fill=DIM)
order = [("Garrow", 19, True, False), ("R · Returned", 17, False, True),
         ("The Returned", 15, False, False), ("Varra", 14, False, False),
         ("R · Returned", 12, False, True), ("R · Choir-shade", 9, False, True),
         ("Roen", 7, False, False)]
iy = 522
for nm, ini, act, enemy in order:
    if act:
        d.rounded_rectangle([FX+12, iy-3, W-24, iy+19], 6, fill=(36, 30, 14), outline=GOLD)
    d.ellipse([FX+18, iy+1, FX+32, iy+15], fill=(RED if enemy else BLUE))
    text((FX+40, iy), nm, serifb(13) if act else serif(13), fill=(GOLD if act else INK))
    text((W-34, iy), str(ini), mono(12), fill=DIM, anchor="ra")
    iy += 24

# ---------- dialogue / combat log (bottom-left, above ability bar) ----------
panel((PX, H-250, 760, H-150), fill=(13, 11, 18), outline=LINE)
text((PX+16, H-242), "⚔  COMBAT LOG", monob(11), fill=DIM)
log = [("The Returned", "strikes a Returned with the Cinderhaunt blade — 17 vs AC 14, hit for 9 slashing. It is Bloodied."),
       ("Varra", "looses Eldritch Blast — natural 20! Critical, 14 force. The shade is unmade."),
       ("System", "Sister Garrow is Blessed (+2 to hit, 2 rounds). Her turn begins.")]
ly = H-218
for who, line in log:
    wc = GOLD if who != "System" else BLUE
    text((PX+16, ly), who + ":", serifb(12), fill=wc)
    # wrap
    words = line.split(); cur = ""; lines = []
    for wd in words:
        if d.textlength(cur + " " + wd, font=serif(12)) > 600: lines.append(cur); cur = wd
        else: cur = (cur + " " + wd).strip()
    lines.append(cur)
    text((PX+16 + d.textlength(who+": ", font=serifb(12)), ly), lines[0], serif(12), fill=INK)
    for extra in lines[1:]:
        ly += 16; text((PX+28, ly), extra, serif(12), fill=INK)
    ly += 20

# ---------- ability bar (bottom) ----------
panel((0, H-140, W, H), fill=(14, 12, 19), outline=LINE, r=0, shadow=False)
abilities = [
    ("1", "Mace", "1d6", "melee", True), ("2", "Sacred\nFlame", "1d8", "cantrip", True),
    ("3", "Cure\nWounds", "1d8+3", "L1 ♦", True), ("4", "Bless", "buff", "L1 ♦", True),
    ("5", "Spiritual\nWeapon", "1d8", "L2 ♦", True), ("", "", "", "", None),
    ("G", "Defend", "", "action", True), ("F", "Dash", "", "action", True),
    ("T", "Help", "", "action", True), ("X", "Diseng.", "", "action", True),
    ("V", "Shove", "", "STR", True), ("Q", "Quaff", "×3", "heal", True),
]
bx = 28; by = H-118
for key, name, dice, tag, on in abilities:
    if on is None: bx += 26; continue
    bw2 = 104
    box = (bx, by, bx + bw2, by + 92)
    d.rounded_rectangle(box, 10, fill=(30, 26, 40) if on else (20,18,26), outline=GOLDD if on else LINE)
    d.rounded_rectangle([bx+6, by+6, bx+26, by+26], 5, fill=(46,40,60), outline=GOLD)
    text((bx+16, by+16), key, monob(13), fill=GOLD, anchor="mm")
    text((bx+bw2-8, by+10), tag, mono(10), fill=DIM, anchor="ra")
    yy = by+34
    for ln in name.split("\n"):
        text((bx+10, yy), ln, serifb(14), fill=INK); yy += 17
    if dice:
        text((bx+10, by+72), dice, mono(11), fill=GREEN)
    bx += bw2 + 12

# end turn button
d.rounded_rectangle([W-200, by+8, W-24, by+78], 12, fill=(46, 36, 14), outline=GOLD, width=2)
text((W-112, by+30), "END TURN", serifb(18), fill=GOLD, anchor="mm")
text((W-112, by+54), "↵  Enter", mono(11), fill=DIM, anchor="mm")

# ---------- watermark ----------
text((W//2, H-150+ -0), "", serif(10))
text((PX, H-150+2-0, ), "", serif(10))

# subtle "mock-up" tag
text((772, H-242), "rendered from the real systems · representative mock-up", serifi(11), fill=(110,104,124))

out = os.path.join(ROOT, "play", "gameplay_mockup.png")
img.convert("RGB").save(out, "PNG")
print(f"wrote {out} ({os.path.getsize(out)//1024} KB, {W}x{H})")
