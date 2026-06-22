#!/usr/bin/env python3
"""
make-dialogue-sim.py — build play/dialogue_sim.html, an immersive, *playable* dialogue
simulator with BG3-style, character-reactive dialogue.

Two layers of content:
  * the 208 compiled conversations from play/dialogue-data.json (the viewer's data), and
  * hand-authored REACTIVE demos from play/dialogue-demo.json, where the NPC's lines and
    the choices on offer change with your character's race, class, background, alignment,
    deity, and ability scores — and skill checks roll d20 + ability modifier + proficiency
    vs the DC, exactly as DialogueRunner.cs + Abilities.cs do.

The pure resolution logic lives in a /*<DLGSIM>*/.../*</DLGSIM>*/ block so dialogue_sim.test.js
can lift it and prove it matches the engine. Re-run:

    python3 tools/extract-dialogue.py     # refresh the compiled data
    python3 tools/make-dialogue-sim.py    # rebuild this page
"""
import json, os, re, hashlib
from collections import Counter, defaultdict

ROOT = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
DATA = json.load(open(os.path.join(ROOT, "play", "dialogue-data.json"), encoding="utf-8"))
DEMO = json.load(open(os.path.join(ROOT, "play", "dialogue-demo.json"), encoding="utf-8"))
MODEL = DEMO["characterModel"]
GLOSSARY = json.load(open(os.path.join(ROOT, "play", "lore-glossary.json"), encoding="utf-8"))

PLAYER_LABELS = {"The Last Returned", "You", "Player", "The Returned"}
FEATURED = {
    "Justiciar Aldous Vane",
    "Naeve", "Sister Garrow", "Garrow", "Varra", "Ilfaeril", "Maerin", "Roen",
    "Roen Alleywind", "Aldric Morn", "Ysolde de Lancie", "Dot Marigold", "Hessa Dock-Eye",
    "Brindle Quillfeather", "Brother Faolan", "Mistress Ekko", "The Cartographer",
    "The Ferryman", "Archivist Threnn", "Senna the Glassblower", "The Travelling Coffin-Salesman",
    "Wickless", "The Mournlight", "The Complaint", "The First Dirge", "Jergal", "Myrkul",
    "Doomguide Vayle", "The Last Honest Psychopomp", "The Unabridged", "Lula",
    "Wrenna Alleywind", "Mother Cass", "Justiciar Veld", "The Smiling Man", "The Margins",
    "Jonn Tallow", "Tobias Ledgerwhite",
}

def hue_of(name):
    return int(hashlib.md5(name.encode("utf-8")).hexdigest(), 16) % 360

def initials(name):
    words = [w for w in re.sub(r"[^A-Za-z' ]", "", name).split() if w.lower() not in ("the", "a", "an", "of")]
    if not words:
        words = name.split()
    return (words[0][:2].upper() if len(words) == 1 else (words[0][0] + words[-1][0]).upper())

def primary_speaker(conv):
    if conv.get("npc"):
        return conv["npc"]
    c = Counter(n.get("speaker") for n in conv["nodes"] if n.get("speaker"))
    if not c:
        return "—"
    for name, _ in c.most_common():
        if name not in PLAYER_LABELS:
            return name
    return c.most_common(1)[0][0]

# Demo conversations come first so they're easy to find; then the compiled corpus.
conversations = list(DEMO["conversations"]) + list(DATA["conversations"])

cast = defaultdict(list)
demo_speakers = set()
for i, conv in enumerate(conversations):
    ps = primary_speaker(conv)
    conv["primarySpeaker"] = ps
    cast[ps].append(i)
    if conv.get("demo"):
        demo_speakers.add(ps)

cast_list = []
for name, idxs in cast.items():
    cast_list.append({
        "name": name, "convs": idxs, "count": len(idxs),
        "featured": name in FEATURED, "demo": name in demo_speakers,
        "hue": hue_of(name), "sigil": initials(name),
    })
# demo NPCs first, then featured, then by conversation count, then alphabetical.
cast_list.sort(key=lambda c: (not c["demo"], not c["featured"], -c["count"], c["name"].lower()))

EMBED = {"conversations": conversations, "cast": cast_list, "stats": DATA.get("stats", {}), "model": MODEL, "glossary": GLOSSARY}
BLOB = json.dumps(EMBED, ensure_ascii=False, separators=(",", ":"))

# Five authentic, *complete* characters — each a full BG3-style identity so switching presets
# transforms the reactive demo. Order of scores: STR,DEX,CON,INT,WIS,CHA.
BUILDS = [
    {"name": "The Returned", "cls": "Fighter", "scores": [16, 14, 15, 10, 12, 13],
     "race": "Human", "background": "Soldier", "law": "Lawful", "morality": "Neutral", "deity": "Kelemvor",
     "blurb": "A plain human soldier of the Judge — leads with order and the book."},
    {"name": "The Scholar", "cls": "Wizard", "scores": [10, 14, 12, 16, 14, 11],
     "race": "Half-Elf", "background": "Sage", "law": "Neutral", "morality": "Good", "deity": "Oghma",
     "blurb": "Half-elf sage of Oghma — out-reads the doctrine; strong INSIGHT & lore."},
    {"name": "The Confessor", "cls": "Cleric", "scores": [13, 10, 14, 11, 17, 12],
     "race": "Human", "background": "Acolyte", "law": "Lawful", "morality": "Good", "deity": "Kelemvor",
     "blurb": "A cleric of Kelemvor — kin to the gatekeeper; the best WISDOM."},
    {"name": "The Silver Tongue", "cls": "Rogue", "scores": [10, 16, 12, 13, 11, 16],
     "race": "Tiefling", "background": "Charlatan", "law": "Chaotic", "morality": "Neutral", "deity": "Tymora",
     "blurb": "A tiefling charlatan — fiend-blood the church distrusts; the best CHARISMA."},
    {"name": "The Warden", "cls": "Ranger", "scores": [14, 16, 13, 11, 14, 10],
     "race": "Elf", "background": "Folk Hero", "law": "Neutral", "morality": "Neutral", "deity": "None",
     "blurb": "An elf who serves no god — walks toward the Wall as one of the Faithless."},
]

COMPANION_NAMES = {"garrow": "Sister Garrow", "roen": "Roen", "varra": "Varra",
                   "naeve": "Naeve", "ilfaeril": "Ilfaeril", "maerin": "Maerin"}
# Friendly labels for the demo's bespoke state ints, so the live panel reads well.
INT_LABELS = {"demo.vane.regard": "Justiciar Vane's regard"}

HTML = r"""<!DOCTYPE html>
<html lang="en"><head><meta charset="utf-8"><meta name="viewport" content="width=device-width, initial-scale=1">
<title>Crown of Horns — Dialogue, Played</title>
<style>
 *{box-sizing:border-box}
 body{margin:0;min-height:100vh;background:radial-gradient(1200px 760px at 50% -10%,#241f30 0%,#0b0a10 70%);
   color:#d8d2c2;font:15px/1.6 "Iowan Old Style","Palatino Linotype",Georgia,serif}
 header{display:flex;align-items:baseline;gap:14px;padding:14px 22px;border-bottom:1px solid #2a2636;flex-wrap:wrap}
 header h1{margin:0;font-size:22px;letter-spacing:.5px;color:#e7c873;font-weight:600}
 header .sub{color:#8a8198;font-size:12px;font-style:italic}
 a.home{margin-left:auto;color:#9a90a8;text-decoration:none;font-size:12.5px;border:1px solid #2a2636;border-radius:7px;padding:6px 11px}
 a.home:hover{border-color:#c9a24b;color:#e7c873}
 .layout{display:grid;grid-template-columns:262px minmax(0,1fr) 326px;gap:0;align-items:stretch;min-height:calc(100vh - 56px)}
 .rail{border-right:1px solid #2a2636;padding:14px 12px;overflow:auto;max-height:calc(100vh - 56px)}
 .rail.right{border-right:0;border-left:1px solid #2a2636}
 .rail h2{font-size:11px;letter-spacing:1.5px;text-transform:uppercase;color:#c9a24b;margin:2px 0 10px;font-weight:600}
 .search{width:100%;background:#15131d;border:1px solid #2a2636;color:#d8d2c2;border-radius:7px;padding:8px 10px;font:inherit;font-size:13px;margin-bottom:10px}
 .grp{font-size:10px;letter-spacing:1px;text-transform:uppercase;color:#6e6680;margin:12px 0 5px}
 .grp.demo{color:#e7c873}
 .npc{display:flex;align-items:center;gap:9px;padding:6px 7px;border-radius:8px;cursor:pointer;border:1px solid transparent}
 .npc:hover{background:#17151f;border-color:#2a2636}
 .npc.on{background:#1d1a28;border-color:#c9a24b66}
 .npc.demo{border-color:#3a3550;background:#1a1726}
 .sig{width:30px;height:30px;border-radius:50%;flex:0 0 auto;display:flex;align-items:center;justify-content:center;
   font-size:11.5px;font-weight:700;color:#0c0b10;font-family:"Iowan Old Style",Georgia,serif}
 .npc .nm{flex:1;font-size:13.5px;line-height:1.25}
 .npc .ct{font-size:10.5px;color:#7f7790}
 .star{color:#e7c873;font-size:10px}
 .stage{padding:22px 26px 90px;max-width:760px;margin:0 auto;width:100%}
 .pick{color:#9a90a8;font-style:italic;margin:6px 0 16px}
 .cta{display:block;width:100%;text-align:left;background:linear-gradient(#2a2238,#1d1830);border:1px solid #c9a24b66;
   border-radius:11px;padding:14px 16px;cursor:pointer;font:inherit;margin:14px 0 6px;color:#e8e2d2}
 .cta:hover{border-color:#e7c873;transform:translateY(-1px)}
 .cta b{color:#e7c873;font-size:16px}.cta .m{color:#9a90a8;font-size:12.5px;margin-top:3px}
 .convlist{display:flex;flex-direction:column;gap:8px;margin-top:6px}
 .convbtn{text-align:left;background:linear-gradient(#16141d,#121019);border:1px solid #2a2636;color:#d8d2c2;border-radius:9px;padding:11px 13px;cursor:pointer;font:inherit}
 .convbtn:hover{border-color:#c9a24b;transform:translateY(-1px)}
 .convbtn.demo{border-color:#c9a24b55;background:linear-gradient(#221c30,#181323)}
 .convbtn .t{color:#e7c873;font-size:15px}
 .convbtn .m{color:#7f7790;font-size:11.5px;margin-top:2px}
 .nowplaying{display:flex;align-items:center;gap:12px;margin-bottom:14px;padding-bottom:12px;border-bottom:1px solid #2a2636}
 .nowplaying .big{width:46px;height:46px;border-radius:50%;font-size:16px}
 .nowplaying .tt{font-size:13px;color:#9a90a8}.nowplaying .tt b{color:#e7c873;font-size:17px;font-weight:600}
 .youare{font-size:11px;color:#7f8aa0;margin-top:3px}
 .script{display:flex;flex-direction:column;gap:14px;margin-bottom:18px}
 .line{opacity:0;animation:fade .35s ease forwards}
 @keyframes fade{to{opacity:1}}
 .line .who{font-size:11px;letter-spacing:1px;text-transform:uppercase;color:#c9a24b;margin-bottom:3px;display:flex;align-items:center;gap:7px}
 .line .who .s2{width:18px;height:18px;border-radius:50%;font-size:8px;font-weight:700;color:#0c0b10;display:inline-flex;align-items:center;justify-content:center}
 .line .body{font-size:15.5px}
 .line.me{align-self:flex-end;max-width:88%;text-align:right}
 .line.me .who{color:#9fc0e0;justify-content:flex-end}
 .line.me .body{color:#cfe0f0;background:#15192a;border:1px solid #243049;border-radius:11px 11px 2px 11px;padding:8px 12px;display:inline-block;text-align:left}
 .line.sys .body{font-size:13px;color:#b9a8e0;font-style:italic}
 .stagedir{color:#8a8198;font-style:italic}
 em{color:#e7c873;font-style:italic}
 .gloss{border-bottom:1px dotted #6a5a9a;cursor:help}.gloss:hover{color:#cbb8f0;border-bottom-color:#cbb8f0}
 #tip{position:fixed;z-index:99;max-width:300px;background:linear-gradient(#1a1626,#13101c);border:1px solid #4a4368;border-radius:9px;padding:10px 12px;font-size:12.5px;line-height:1.5;color:#cfc6dc;box-shadow:0 10px 34px #000b;pointer-events:none;display:none}
 #tip .tn{color:#e0b8f0;font-weight:600;font-size:13px;margin-bottom:3px}
 #tip .tl{margin-top:7px;padding-top:7px;border-top:1px solid #2e2940;color:#c8d8b8;font-style:italic}#tip .tl b{color:#9fe0b0;font-style:normal}
 .lore{margin:2px 0 4px;padding:8px 11px;border-left:3px solid #5a4a8a;background:#171327;border-radius:0 8px 8px 0;font-size:13px;color:#c6bcda}
 .lore .lk{color:#b08fe0;font-weight:600;letter-spacing:.3px;font-size:11px}
 .lore .deep{margin-top:7px;padding-top:7px;border-top:1px solid #3a2f55;color:#e0cba0}
 .sense{margin:2px 0 6px;padding:9px 12px;border-left:3px solid #3a6a8a;background:#0f1922;border-radius:0 8px 8px 0;font-size:13.5px;color:#a8c8d8;font-style:italic}
 .sense .lk{color:#7fc8e0;font-weight:600;font-style:normal;letter-spacing:.3px;font-size:11px;display:block;margin-bottom:3px}
 .reckon{display:flex;align-items:center;gap:7px;margin:4px 0;font-size:12px}.reckon .nm{width:78px}.reckon .pips{flex:1;letter-spacing:1px}.reckon .rk{font-variant-numeric:tabular-nums;color:#8a8198}
 .tg.disp{color:#d8b0e8;border-color:#5a3a6a;background:#1f1726}.tg.returned{color:#cbb8f0;border-color:#5a4a8a;background:#1a1626}
 .opts{display:flex;flex-direction:column;gap:9px;margin-top:6px}
 .opt{text-align:left;background:linear-gradient(#1b1726,#16121f);border:1px solid #34304a;color:#e8e2d2;border-radius:10px;padding:11px 14px;cursor:pointer;font:inherit;font-size:14.5px;transition:.12s;position:relative}
 .opt:hover{border-color:#c9a24b;background:linear-gradient(#241d33,#1b1628)}
 .opt .num{color:#6e6680;font-size:11px;margin-right:7px}
 .opt.locked{opacity:.45;cursor:not-allowed;border-style:dashed}
 .opt .tags{display:flex;gap:6px;flex-wrap:wrap;margin-bottom:5px}
 .tg{display:inline-block;font-size:10px;letter-spacing:.5px;border-radius:5px;padding:1px 7px;font-weight:600;border:1px solid}
 .tg.race{color:#9fe0b0;border-color:#2c5a3a;background:#16241b}
 .tg.class{color:#9ec8e8;border-color:#27405a;background:#13202b}
 .tg.background{color:#7fd0c8;border-color:#245049;background:#13231f}
 .tg.faith{color:#e0b8f0;border-color:#5a3a6a;background:#221829}
 .tg.alignment{color:#c8c0b4;border-color:#3a3550;background:#1a1622}
 .tg.stat{color:#f0c890;border-color:#6a4f2a;background:#241c10}
 .tg.check{color:#c9a24b;border-color:#5a4a2a;background:#211c10}
 .opt .ck{margin-top:6px;font-size:11.5px;color:#b9a8e0;display:flex;gap:8px;flex-wrap:wrap;align-items:center}
 .chip{display:inline-block;font-size:10.5px;border:1px solid #3a3550;border-radius:5px;padding:1px 6px;color:#c9a24b}
 .chip.ok{color:#9fe0b0;border-color:#2c4a32}.chip.bad{color:#e0a0a0;border-color:#5a2c2c}
 .chip.fx{color:#9ec8e8;border-color:#27405a}.chip.prof{color:#f0c890;border-color:#6a4f2a}
 .opt .fxline{margin-top:5px;font-size:11px;color:#7f8aa0}
 .continue{background:linear-gradient(#26212f,#1a1622);color:#e7c873;border:1px solid #c9a24b66;border-radius:8px;padding:9px 16px;cursor:pointer;font:inherit;font-size:13.5px;align-self:flex-start}
 .continue:hover{border-color:#c9a24b}
 .restart{background:#15131d;color:#9a90a8;border:1px solid #2a2636;border-radius:8px;padding:8px 14px;cursor:pointer;font:inherit;font-size:13px;margin-top:8px}
 .restart:hover{border-color:#c9a24b;color:#e7c873}
 .ending{margin-top:14px;padding:12px 14px;border:1px dashed #3a3550;border-radius:9px;color:#9a90a8;font-style:italic}
 .dice{display:flex;align-items:center;gap:12px;background:#14121c;border:1px solid #2a2636;border-radius:10px;padding:10px 14px;margin:2px 0}
 .d20{width:42px;height:42px;flex:0 0 auto;border-radius:8px;background:radial-gradient(circle at 40% 35%,#2e2740,#15121d);border:1px solid #4a4368;display:flex;align-items:center;justify-content:center;font-size:18px;font-weight:700;color:#e7c873;font-variant-numeric:tabular-nums}
 .d20.rolling{animation:shake .5s ease}
 @keyframes shake{0%,100%{transform:translateY(0) rotate(0)}25%{transform:translateY(-3px) rotate(-8deg)}50%{transform:translateY(2px) rotate(7deg)}75%{transform:translateY(-2px) rotate(-5deg)}}
 .dice .calc{font-size:12.5px;color:#b9a8e0}
 .dice .res{font-weight:700;margin-left:auto}.dice .res.ok{color:#9fe0b0}.dice .res.bad{color:#e0a0a0}
 .card{background:linear-gradient(#16141d,#131119);border:1px solid #2a2636;border-radius:9px;padding:12px;margin-bottom:13px}
 .card h3{font-size:10.5px;letter-spacing:1.3px;text-transform:uppercase;color:#c9a24b;margin:0 0 9px;font-weight:600}
 .builds{display:flex;flex-wrap:wrap;gap:5px;margin-bottom:9px}
 .bld{font:inherit;font-size:11.5px;background:#1a1622;border:1px solid #2a2636;color:#c8c0b4;border-radius:6px;padding:5px 8px;cursor:pointer}
 .bld.on{border-color:#e7c873;color:#e7c873;background:#241d2e}
 .who-grid{display:grid;grid-template-columns:1fr 1fr;gap:6px;margin-bottom:8px}
 .who-grid label{font-size:9.5px;letter-spacing:.5px;color:#7f7790;display:block;margin-bottom:2px}
 .who-grid select{width:100%;background:#14121b;border:1px solid #2a2636;color:#d8d2c2;border-radius:6px;padding:4px 5px;font:inherit;font-size:12px}
 .abil{display:grid;grid-template-columns:repeat(3,1fr);gap:6px}
 .ab{background:#14121b;border:1px solid #232030;border-radius:7px;padding:6px 4px;text-align:center}
 .ab .k{font-size:9.5px;letter-spacing:.5px;color:#7f7790}
 .ab .v{font-size:16px;font-variant-numeric:tabular-nums}.ab .m{font-size:11px;color:#9fe0b0}
 .ab.dlg{border-color:#4a4368;box-shadow:0 0 0 1px #6a5a9a33}
 .ab .step{display:flex;gap:3px;justify-content:center;margin-top:3px}
 .ab .step button{width:18px;height:16px;line-height:13px;font-size:11px;background:#221d2b;border:1px solid #34304a;color:#c9a24b;border-radius:4px;cursor:pointer;padding:0}
 .prof{font-size:10.5px;color:#7f8aa0;margin-top:7px;line-height:1.4}
 .prof b{color:#f0c890;font-weight:600}
 .toggle{display:flex;align-items:center;gap:7px;font-size:12px;color:#b8b0c4;margin:6px 0;cursor:pointer}
 .toggle input{accent-color:#c9a24b}
 .stateline{font-size:12px;color:#a89fb4;padding:2px 0;border-bottom:1px solid #1c1a26}
 .stateline .k{color:#8a8198;font-size:10.5px}
 .empty{color:#6e6680;font-style:italic;font-size:12px}
 .appr{display:flex;align-items:center;gap:7px;margin:5px 0;font-size:12px}
 .appr .nm{width:120px;color:#c8c0b4}
 .appr .barwrap{flex:1;height:8px;background:#1c1a26;border-radius:4px;overflow:hidden;position:relative}
 .appr .bar{position:absolute;left:50%;top:0;bottom:0;background:linear-gradient(#4a8a5a,#6fd08a)}
 .appr .bar.neg{background:linear-gradient(#9b2d2d,#d06f6f)}
 .appr .delta{width:30px;text-align:right;font-variant-numeric:tabular-nums}
 .appr .delta.up{color:#9fe0b0}.appr .delta.down{color:#e0a0a0}
 .flagpill{display:inline-block;font-size:10.5px;background:#1a1622;border:1px solid #2a2636;border-radius:5px;padding:2px 6px;margin:2px 3px 0 0;color:#b8b0c4}
 .flagpill .d{color:#6e6680}
 .muted{color:#6e6680;font-size:11px}
 .sfxbtn{float:right;background:none;border:1px solid #2a2636;color:#8a8198;border-radius:6px;font-size:11px;padding:3px 7px;cursor:pointer}
 .sfxbtn:hover{color:#e7c873;border-color:#c9a24b}
 @media(max-width:980px){.layout{grid-template-columns:1fr}.rail,.rail.right{border:0;border-bottom:1px solid #2a2636;max-height:none}}
</style></head><body>
<header>
 <h1>👑 Dialogue, Played</h1>
 <span class="sub">step into any conversation — the NPC reads your race, faith, alignment, and stats, and answers differently</span>
 <a class="home" href="index.html">← all previews</a>
</header>
<div class="layout">
 <div class="rail" id="castRail">
  <h2>The Cast</h2>
  <input class="search" id="search" placeholder="search NPCs…" oninput="renderCast(this.value)">
  <div id="castList"></div>
 </div>
 <div class="stage" id="stage"></div>
 <div class="rail right">
  <div class="card">
   <h3>Your Character <button class="sfxbtn" id="sfxToggle" onclick="toggleSfx()">🔊 sfx</button></h3>
   <div class="builds" id="builds"></div>
   <div class="muted" id="buildBlurb" style="margin-bottom:8px"></div>
   <div class="who-grid" id="whoGrid"></div>
   <div class="abil" id="abil"></div>
   <div class="muted" style="margin-top:7px">INT · WIS · CHA drive every check. Tap ▲▼ or switch race / faith / alignment to see the NPC react.</div>
   <div class="prof" id="profLine"></div>
  </div>
  <div class="card">
   <h3>Preview tools</h3>
   <label class="toggle"><input type="checkbox" id="tReveal" onchange="rerenderChoices()"> Reveal what each choice does</label>
   <label class="toggle"><input type="checkbox" id="tForce" onchange="rerenderChoices()"> Let me decide check results</label>
   <label class="toggle"><input type="checkbox" id="tAssume" onchange="rerenderChoices()"> Assume earlier story flags</label>
   <label class="toggle"><input type="checkbox" id="tLocked" onchange="rerenderChoices()"> Show choices I don't qualify for</label>
  </div>
  <div class="card">
   <h3>How they regard you</h3>
   <div id="approvals"><div class="empty">No one's measure of you has moved yet.</div></div>
  </div>
  <div class="card">
   <h3>Your reckoning <span style="float:right;font-weight:400;text-transform:none;letter-spacing:0;color:#6e6680">who you're becoming</span></h3>
   <div id="reckon"><div class="empty">Your choices haven't tilted you yet.</div></div>
  </div>
  <div class="card">
   <h3>Flags this run</h3>
   <div id="flags"><div class="empty">Nothing set yet — your choices write here.</div></div>
  </div>
 </div>
</div>
<div id="tip"></div>
<script>
const DATA = __BLOB__;
const BUILDS = __BUILDS__;
const COMPANION_NAMES = __COMPANIONS__;
const INT_LABELS = __INTLABELS__;
const CONVS = DATA.conversations, CAST = DATA.cast, MODEL = DATA.model, GLOSS = DATA.glossary;
const ABILS = ["Strength","Dexterity","Constitution","Intelligence","Wisdom","Charisma"];
const ABBR  = {Strength:"STR",Dexterity:"DEX",Constitution:"CON",Intelligence:"INT",Wisdom:"WIS",Charisma:"CHA"};
// Pillars-of-Eternity dispositions — choices accrue a reckoning that persists across the conversation.
const DISP = { "disp.merciful":{name:"Merciful",hue:140}, "disp.ruthless":{name:"Ruthless",hue:0},
  "disp.honest":{name:"Honest",hue:200}, "disp.cunning":{name:"Cunning",hue:280}, "disp.devout":{name:"Devout",hue:48},
  "disp.heretical":{name:"Heretical",hue:320}, "disp.stoic":{name:"Stoic",hue:210}, "disp.haunted":{name:"Haunted",hue:265} };
function roman(n){ return ["","I","II","III","IV","V","VI","VII","VIII","IX","X"][Math.min(10,Math.abs(n))]||(""+n); }
const GLOSS_INDEX = (function(){ const a=[]; GLOSS.forEach((e,i)=>{ [e.term].concat(e.aka||[]).forEach(t=>a.push({t:t,i:i})); }); a.sort((x,y)=>y.t.length-x.t.length); return a; })();
function escRe(s){ return s.replace(/[.*+?^${}()|[\]\\]/g,"\\$&"); }
const GLOSS_RE = GLOSS_INDEX.length ? new RegExp("\\b("+GLOSS_INDEX.map(o=>escRe(o.t)).join("|")+")\\b","i") : null;

/*<DLGSIM>*/
// Pure, testable resolution — mirrors Assets/Scripts/Dialogue/DialogueRunner.cs + Abilities.cs.
function abilityMod(score){ return Math.floor((score - 10) / 2); }              // floor((score-10)/2)
function resolveCheck(roll, dc, mod){ return (roll + mod) >= dc; }              // roll = d20+mod; success = roll>=DC
function chanceToPass(dc, mod){ return Math.max(0, Math.min(20, 21 - (dc - mod))) / 20; }
function newState(){ return { bools:{}, ints:{} }; }
function applyEffects(state, effects){
  (effects||[]).forEach(e=>{
    if(e.op==="SetTrue") state.bools[e.key] = true;
    else if(e.op==="AddInt") state.ints[e.key] = (state.ints[e.key]||0) + (e.amount||0);
  });
  return state;
}
// legacy flag conditions (the compiled corpus): RequireBoolTrue clauses on a choice
function conditionsPass(state, conditions, assumePrior){
  return (conditions||[]).every(c=>{
    if(c.op==="RequireBoolTrue") return state.bools[c.key]===true || !!assumePrior;
    return true;
  });
}
// is the character proficient in a skill (class proficiencies ∪ background proficiencies)?
function isProficient(char, skill, model){
  if(!skill) return false;
  const cp=(model.classProficiencies[char.cls]||[]);
  const bp=(model.backgroundProficiencies[char.background]||[]);
  return cp.indexOf(skill)>=0 || bp.indexOf(skill)>=0;
}
// total modifier for a check = ability modifier (+ proficiency bonus if proficient in the skill)
function checkBonus(char, check, model){
  const i=ABILS_INDEX(check.ability);
  let mod=abilityMod(char.scores[i]);
  if(check.skill && isProficient(char, check.skill, model)) mod += (model.proficiencyBonus||0);
  return mod;
}
function ABILS_INDEX(name){ return ["Strength","Dexterity","Constitution","Intelligence","Wisdom","Charisma"].indexOf(name); }
// BG3-style identity/state gate. ALL present conditions must hold (arrays = membership / OR).
function matchesWhen(char, state, when){
  if(!when) return true;
  const inOr=(v,val)=> Array.isArray(v) ? v.indexOf(val)>=0 : v===val;
  if(when.race!==undefined && !inOr(when.race, char.race)) return false;
  if(when.class!==undefined && !inOr(when.class, char.cls)) return false;
  if(when.background!==undefined && !inOr(when.background, char.background)) return false;
  if(when.deity!==undefined && !inOr(when.deity, char.deity)) return false;
  if(when.law!==undefined && !inOr(when.law, char.law)) return false;
  if(when.morality!==undefined && !inOr(when.morality, char.morality)) return false;
  if(when.ability){ for(const k in when.ability){ if(char.scores[ABILS_INDEX(k)] < when.ability[k]) return false; } }
  if(when.flag!==undefined && state.bools[when.flag]!==true) return false;
  if(when.flags){ for(const k of when.flags){ if(state.bools[k]!==true) return false; } }
  if(when.int){ for(const k in when.int){ if((state.ints[k]||0) < when.int[k]) return false; } }
  return true;
}
// the reactive line for a node: first variant whose 'when' matches, else node.text
function pickVariantText(node, char, state){
  if(node.variants && node.variants.length){
    for(const v of node.variants){ if(matchesWhen(char, state, v.when)) return v.text; }
    return "";
  }
  return node.text || "";
}
// BG3/5e: knowledge & awareness skills are PASSIVE — they auto-succeed when 10+mod(+prof) >= DC, and the
// option simply doesn't appear otherwise. The social "attempt" skills are ACTIVE — you roll for those.
// (Compiled-corpus checks carry only an ability and no skill name, so they stay active rolls.)
var PASSIVE_SKILLS=["Perception","Insight","Investigation","Arcana","History","Religion","Nature","Medicine","Survival","Animal Handling"];
function isPassiveSkill(s){ return !!s && PASSIVE_SKILLS.indexOf(s)>=0; }
function checkOf(ch){ return ch.check || (ch.checkDC ? {skill:(ch.checkSkill||null), ability:ch.checkAbility, dc:ch.checkDC} : null); }
function passiveScore(char, check, model){ return 10 + checkBonus(char, check, model); }   // 10 + ability mod (+ proficiency)
function passiveBeats(char, check, model){ return passiveScore(char, check, model) >= check.dc; }
// can this choice be taken by this character/state? (identity gate + legacy flag conditions + passive checks)
function choiceAvailable(char, state, ch, assumePrior, model){
  if(!matchesWhen(char, state, ch.when)) return false;                                   // identity/stat gate: ruled out if unmet
  if(!conditionsPass(state, ch.conditions, assumePrior)) return false;
  const chk=checkOf(ch);
  if(chk && isPassiveSkill(chk.skill) && !passiveBeats(char, chk, model)) return false;  // passive skill: only shown if you'd already pass
  return true;
}
// LORE (5e passive knowledge) + the Returned-sense (innate, Wisdom-scaled soul perception).
function glossaryHits(text, glossary){ const t=(text||"").toLowerCase(), hits=[];
  for(var i=0;i<glossary.length;i++){ var names=[glossary[i].term].concat(glossary[i].aka||[]);
    for(var j=0;j<names.length;j++){ if(t.indexOf(names[j].toLowerCase())>=0){ hits.push(i); break; } } }
  return hits; }
function loreKnown(char, entry, model){ if(!entry||!entry.skill) return false;
  return passiveBeats(char, {skill:entry.skill, ability:model.skillAbility[entry.skill], dc:entry.dc||10}, model); }
function secretKnown(char, entry, model){ if(!entry||!entry.secret) return false;
  return passiveBeats(char, {skill:entry.secretSkill, ability:model.skillAbility[entry.secretSkill], dc:entry.secretDc||18}, model); }
function returnedClarity(char){ return 10 + abilityMod(char.scores[4]); }   // a soul back from the Wall; clarity = 10 + WIS mod
/*</DLGSIM>*/

// ---- player character ----
let char = JSON.parse(JSON.stringify(BUILDS[0])), buildIdx = 0;
function modFor(abilityName){ return abilityMod(char.scores[ABILS.indexOf(abilityName)]); }

function renderBuilds(){
  document.getElementById("builds").innerHTML = BUILDS.map((b,i)=>
    `<button class="bld${i===buildIdx?' on':''}" onclick="setBuild(${i})">${esc(b.name)}</button>`).join("")
    + `<button class="bld${buildIdx===-1?' on':''}" onclick="void 0" style="cursor:default;opacity:${buildIdx===-1?1:.5}">Custom</button>`;
  document.getElementById("buildBlurb").textContent = char.blurb || "A character of your own making.";
  renderWho(); renderAbil(); renderProf();
}
function setBuild(i){ buildIdx=i; char=JSON.parse(JSON.stringify(BUILDS[i])); renderBuilds(); rerenderChoices(); refreshYouAre(); }
function setField(field, val){ char[field]=val; char.name="Custom"; char.blurb=""; buildIdx=-1; renderBuilds(); rerenderChoices(); refreshYouAre(); }
function bump(i,d){ char.scores[i]=Math.max(1,Math.min(20,char.scores[i]+d)); char.name="Custom"; char.blurb=""; buildIdx=-1; renderBuilds(); rerenderChoices(); }
function renderWho(){
  const sel=(field,opts,val)=>`<div><label>${field.toUpperCase()}</label><select onchange="setField('${field2key(field)}',this.value)">`+
    opts.map(o=>`<option${o===val?' selected':''}>${esc(o)}</option>`).join("")+`</select></div>`;
  const align=`<div><label>ALIGNMENT</label><select onchange="setAlign(this.value)">`+
    MODEL.laws.flatMap(l=>MODEL.moralities.map(m=>{ const v=l+" "+m; const cur=(char.law+" "+char.morality)===v;
      return `<option${cur?' selected':''}>${esc(v)}</option>`; })).join("")+`</select></div>`;
  document.getElementById("whoGrid").innerHTML =
    sel("Race", MODEL.races, char.race) + sel("Class", MODEL.classes, char.cls) +
    sel("Background", MODEL.backgrounds, char.background) + sel("Deity", MODEL.deities, char.deity) + align;
}
function field2key(f){ return {Race:"race",Class:"cls",Background:"background",Deity:"deity"}[f]; }
function setAlign(v){ const [l,m]=v.split(" "); char.law=l; char.morality=m; char.name="Custom"; char.blurb=""; buildIdx=-1; renderBuilds(); rerenderChoices(); refreshYouAre(); }
function renderAbil(){
  document.getElementById("abil").innerHTML = ABILS.map((a,i)=>{
    const m=abilityMod(char.scores[i]); const isDlg=(a==="Intelligence"||a==="Wisdom"||a==="Charisma");
    return `<div class="ab${isDlg?' dlg':''}"><div class="k">${ABBR[a]}</div><div class="v">${char.scores[i]}</div>
      <div class="m">${m>=0?'+':''}${m}</div><div class="step"><button onclick="bump(${i},-1)">▼</button><button onclick="bump(${i},1)">▲</button></div></div>`;
  }).join("");
}
function renderProf(){
  const cp=(MODEL.classProficiencies[char.cls]||[]), bp=(MODEL.backgroundProficiencies[char.background]||[]);
  const all=[...new Set([...cp,...bp])].sort();
  document.getElementById("profLine").innerHTML = `Proficient (+${MODEL.proficiencyBonus}) in: ` +
    (all.length? all.map(s=>`<b>${esc(s)}</b>`).join(", ") : "—") + `. Proficiency is added to matching skill checks.`;
}

// ---- cast browser ----
let castSel=-1;
function renderCast(q){
  q=(q||"").toLowerCase();
  const demo=[], featured=[], rest=[];
  CAST.forEach((c,i)=>{ if(q && !c.name.toLowerCase().includes(q)) return; (c.demo?demo:(c.featured?featured:rest)).push(i); });
  const row=i=>{ const c=CAST[i];
    return `<div class="npc${i===castSel?' on':''}${c.demo?' demo':''}" onclick="pickNpc(${i})">
      <div class="sig" style="background:hsl(${c.hue} 52% 62%)">${esc(c.sigil)}</div>
      <div class="nm">${esc(c.name)} ${c.demo?'<span class="star">✦</span>':(c.featured?'<span class="star">★</span>':'')}<div class="ct">${c.demo?'reactive demo · ':''}${c.count} conversation${c.count>1?'s':''}</div></div></div>`; };
  let h="";
  if(demo.length){ h+=`<div class="grp demo">✦ Character-reactive demo</div>`+demo.map(row).join(""); }
  if(featured.length){ h+=`<div class="grp">Principal cast</div>`+featured.map(row).join(""); }
  if(rest.length){ h+=`<div class="grp">Everyone else</div>`+rest.map(row).join(""); }
  if(!h) h=`<div class="empty">No speaker matches “${esc(q)}”.</div>`;
  document.getElementById("castList").innerHTML=h;
}
function pickNpc(i){ castSel=i; renderCast(document.getElementById("search").value); showConvList(i); }
function showConvList(i){
  const c=CAST[i];
  const items=c.convs.map(ci=>{ const cv=CONVS[ci];
    const nodes=cv.nodes.length, choices=cv.nodes.reduce((a,n)=>a+(n.choices?n.choices.length:0),0),
          checks=cv.nodes.reduce((a,n)=>a+(n.choices?n.choices.filter(x=>x.checkDC||x.check).length:0),0);
    return `<button class="convbtn${cv.demo?' demo':''}" onclick="startConv(${ci})">
      <div class="t">${cv.demo?'✦ ':''}${esc(cv.title)}</div>
      <div class="m">${cv.blurb?esc(cv.blurb):(nodes+' beats · '+choices+' choices'+(checks?` · 🎲 ${checks} check${checks>1?'s':''}`:''))}</div></button>`;
  }).join("");
  document.getElementById("stage").innerHTML =
    `<div class="nowplaying"><div class="sig big" style="background:hsl(${c.hue} 52% 62%)">${esc(c.sigil)}</div>
       <div class="tt">Conversations with<br><b>${esc(c.name)}</b></div></div>
     <div class="pick">Choose one to step into. ${c.demo?'This one changes line-by-line with the character on the right — switch race, faith, or alignment and replay to see it answer differently.':'Your build and the preview tools on the right apply live.'}</div>
     <div class="convlist">${items}</div>`;
}

// ---- play engine ----
let st=newState(), curConv=null, history=[], pendingOpts=null, loreSeen={};
function startConv(ci){
  curConv=CONVS[ci]; st=newState(); history=[]; loreSeen={};
  const start=curConv.nodes.find(n=>n.id===curConv.start)?curConv.start:curConv.nodes[0].id;
  const hue=(CAST[castSel]?CAST[castSel].hue:hue0(curConv.primarySpeaker)), sig=(CAST[castSel]?CAST[castSel].sigil:'…');
  document.getElementById("stage").innerHTML =
    `<div class="nowplaying"><div class="sig big" style="background:hsl(${hue} 52% 62%)">${esc(sig)}</div>
       <div class="tt"><b>${curConv.demo?'✦ ':''}${esc(curConv.title)}</b><br>with ${esc(curConv.primarySpeaker)}
       <div class="youare" id="youare"></div></div></div>
     <div class="script" id="script"></div>`;
  refreshYouAre(); renderState();
  // The Returned-sense — on first sight, if the soul's clarity is keen enough, perceive the unseen.
  if(curConv.returned && returnedClarity(char) >= curConv.returned.dc){ addSense(curConv.returned.text); }
  goNode(start, true);
}
function refreshYouAre(){ const el=document.getElementById("youare"); if(el) el.textContent =
  `you: ${char.name==="Custom"?"":char.name+" — "}${char.race} ${char.cls} · ${char.background} · ${char.law} ${char.morality} · ${char.deity==="None"?"Faithless":char.deity}`; }
function hue0(n){ let h=0; for(const ch of (n||"")) h=(h*31+ch.charCodeAt(0))%360; return h; }
function nodeById(id){ return curConv.nodes.find(n=>n.id===id); }
function isEnd(id){ return !id || id==="END" || id==="end" || !nodeById(id); }

function goNode(id, first){
  pendingOpts=null;
  const n=nodeById(id), script=document.getElementById("script");
  if(!n){ endScene(id); return; }
  applyEffects(st, n.onEnter); applyEffects(st, n.effects); renderState();   // node entry + outcome effects
  const body=pickVariantText(n, char, st);
  addLine("", n.speaker, body || "〔(this line had no variant for your character)〕", n.speaker);
  // LORE — 5e passive knowledge: surface what your character would recall about topics in this line.
  glossaryHits(body, GLOSS).forEach(i=>{ const e=GLOSS[i]; if(e.skill && !loreSeen[i] && loreKnown(char, e, MODEL)){ loreSeen[i]=true; addLore(e); } });
  const opts=document.createElement("div"); opts.className="opts"; script.appendChild(opts);
  const all=n.choices||[];
  if(all.length){ pendingOpts={node:n, el:opts}; paintChoices(); }
  else if(n.auto && !isEnd(n.auto)){ const b=document.createElement("button"); b.className="continue"; b.textContent="Continue ▸";
    b.onclick=()=>{ sfx('page'); opts.remove(); goNode(n.auto); }; opts.appendChild(b); }
  else endScene(n.auto);
  scrollEnd();
}
function normCheck(ch){ // unify legacy (checkDC/checkAbility) and rich (check{}) shapes
  if(ch.check) return ch.check;
  if(ch.checkDC) return {skill:null, ability:ch.checkAbility, dc:ch.checkDC};
  return null;
}
function tagFor(ch){ // BG3-style identity tag chip derived from the choice's tag/when
  if(ch.tag) return {cls:ch.tag, label:(ch.tagLabel||ch.tag.toUpperCase())};
  const w=ch.when; if(!w) return null;
  if(w.race!==undefined) return {cls:"race", label:[].concat(w.race).join("/")};
  if(w.class!==undefined) return {cls:"class", label:[].concat(w.class).join("/")};
  if(w.background!==undefined) return {cls:"background", label:[].concat(w.background).join("/")};
  if(w.deity!==undefined) return {cls:"faith", label:[].concat(w.deity).map(d=>d==="None"?"Faithless":d).join("/")};
  if(w.law!==undefined) return {cls:"alignment", label:[].concat(w.law).join("/")};
  if(w.morality!==undefined) return {cls:"alignment", label:[].concat(w.morality).join("/")};
  if(w.ability){ const k=Object.keys(w.ability)[0]; return {cls:"stat", label:`${ABBR[k]} ${w.ability[k]}`}; }
  if(w.int){ const k=Object.keys(w.int)[0]; if(k.indexOf("disp.")===0){ const d=DISP[k]||{name:k}; return {cls:"disp", label:`${d.name} ${roman(w.int[k])}`}; }
    return {cls:"alignment", label:`${(INT_LABELS[k]||k)} ≥ ${w.int[k]}`}; }
  return null;
}
function paintChoices(){
  if(!pendingOpts) return;
  const {node:n, el:opts}=pendingOpts; opts.innerHTML="";
  const reveal=document.getElementById("tReveal").checked, assume=document.getElementById("tAssume").checked,
        force=document.getElementById("tForce").checked, showLocked=document.getElementById("tLocked").checked;
  let shown=0, n9=0;
  n.choices.forEach((ch)=>{
    const ok=choiceAvailable(char, st, ch, assume, MODEL);
    if(!ok && !showLocked) return;
    shown++; const idx=++n9;
    const b=document.createElement("button"); b.className="opt"+(ok?"":" locked");
    const chk=normCheck(ch); const tag=tagFor(ch); const passive=chk&&isPassiveSkill(chk.skill);
    let tags="";
    if(tag) tags+=`<span class="tg ${tag.cls}">${esc(tag.label)}</span>`;
    if(chk) tags+=`<span class="tg check">${passive?'👁':'🎲'} ${esc(chk.skill||ABBR[chk.ability]||chk.ability)}${passive?' · passive':''}</span>`;
    let inner = (tags?`<div class="tags">${tags}</div>`:"") + `<span class="num">${idx}.</span>${esc(stripBracket(ch.text)||"(continue)")}`;
    if(chk){
      const bonus=checkBonus(char, chk, MODEL), prof=chk.skill&&isProficient(char,chk.skill,MODEL);
      if(passive){ const sc=10+bonus;
        inner+=`<div class="ck"><span class="chip">${esc(chk.skill)} (passive)</span><span class="chip ${sc>=chk.dc?'ok':'bad'}">${sc} vs DC ${chk.dc}${sc>=chk.dc?' ✓ auto':' ✗'}</span>`+
               (prof?`<span class="chip prof">proficient +${MODEL.proficiencyBonus}</span>`:'')+`</div>`;
      } else { const pct=Math.round(chanceToPass(chk.dc,bonus)*100);
        inner+=`<div class="ck"><span class="chip">${esc(chk.skill||ABBR[chk.ability])} DC ${chk.dc}</span>`+
               `<span class="chip ${pct>=50?'ok':'bad'}">${pct}% (d20 ${bonus>=0?'+':''}${bonus})</span>`+
               (prof?`<span class="chip prof">proficient +${MODEL.proficiencyBonus}</span>`:'')+
               (ch.fail?`<span class="muted">miss ▸ a different path</span>`:'')+`</div>`;
      }
    }
    if(!ok){ inner+=`<div class="fxline">🔒 ${lockReason(ch)}</div>`; }
    if(reveal){ const fx=describeEffects(ch.effects); if(fx) inner+=`<div class="fxline">▸ ${fx}</div>`; }
    b.innerHTML=inner;
    if(ok) b.onclick=()=>choose(n, ch, force); else b.onclick=()=>{};
    opts.appendChild(b);
  });
  if(!shown){ const d=document.createElement("div"); d.className="muted"; d.textContent="(no choice here fits your character — try “Show choices I don't qualify for”, or a different build.)"; opts.appendChild(d); }
}
function lockReason(ch){
  const chk=normCheck(ch);
  if(chk && isPassiveSkill(chk.skill) && !passiveBeats(char,chk,MODEL)){ const sc=10+checkBonus(char,chk,MODEL); return `passive ${esc(chk.skill)} ${sc} &lt; DC ${chk.dc} — you don't catch it`; }
  const t=tagFor(ch); if(t) return `needs ${t.cls==="faith"?"faith":t.cls}: ${t.label}`;
  const need=(ch.conditions||[]).filter(c=>c.op==="RequireBoolTrue").map(c=>c.key);
  if(need.length) return `needs earlier: ${need.map(esc).join(", ")}`;
  if(ch.when&&ch.when.int){ const k=Object.keys(ch.when.int)[0]; return `needs ${INT_LABELS[k]||k} ≥ ${ch.when.int[k]}`; }
  return "unavailable to this character";
}
function rerenderChoices(){ renderAbil(); renderProf(); paintChoices(); }

function choose(n, ch, force){
  sfx('page'); pendingOpts.el.remove(); pendingOpts=null;
  addLine("me","You", stripBracket(ch.text)||"(continue)");
  history.push(JSON.parse(JSON.stringify(st)));
  const chk=normCheck(ch); let next=ch.next;
  const proceed=(success)=>{ if(chk && !success && ch.fail) next=ch.fail; applyEffects(st, ch.effects); renderState(); goNode(next); };
  if(chk && isPassiveSkill(chk.skill)){ // passive: only appeared because you already pass — auto-success, no roll
    addLine("sys","",`(${chk.skill} — passive ${10+checkBonus(char,chk,MODEL)} vs DC ${chk.dc}: success)`);
    applyEffects(st, ch.effects); renderState(); goNode(next); return; }
  if(chk){ const bonus=checkBonus(char, chk, MODEL); if(force){ offerForced(chk, proceed); return; } rollDice(chk, bonus, proceed); }
  else { applyEffects(st, ch.effects); renderState(); goNode(next); }
}
function offerForced(chk, proceed){
  const box=document.createElement("div"); box.className="opts";
  box.innerHTML=`<div class="muted" style="margin-bottom:4px">Preview either branch of this ${chk.skill||ABBR[chk.ability]} DC ${chk.dc} check:</div>`;
  const mk=(label,cls,ok)=>{ const b=document.createElement("button"); b.className="opt"; b.innerHTML=`<span class="chip ${cls}">${label}</span>`;
    b.onclick=()=>{ box.remove(); addLine("sys","",`(forced ${ok?'SUCCESS':'FAILURE'})`); proceed(ok); }; return b; };
  box.appendChild(mk("✓ Take the success branch","ok",true)); box.appendChild(mk("✗ Take the failure branch","bad",false));
  document.getElementById("script").appendChild(box); scrollEnd();
}
function rollDice(chk, bonus, proceed){
  const wrap=document.createElement("div"); wrap.className="dice";
  wrap.innerHTML=`<div class="d20 rolling" id="d20">?</div><div class="calc">rolling ${chk.skill||ABBR[chk.ability]}…</div>`;
  document.getElementById("script").appendChild(wrap); scrollEnd(); sfx('dice');
  let ticks=0; const die=wrap.querySelector("#d20"), calc=wrap.querySelector(".calc");
  const spin=setInterval(()=>{ die.textContent=1+Math.floor(Math.random()*20); if(++ticks>=12){ clearInterval(spin); land(); } }, 42);
  function land(){
    const roll=1+Math.floor(Math.random()*20), total=roll+bonus, ok=resolveCheck(roll, chk.dc, bonus);
    die.classList.remove("rolling"); die.textContent=roll;
    calc.innerHTML=`${roll} ${bonus>=0?'+':'−'} ${Math.abs(bonus)} <b>= ${total}</b> vs DC ${chk.dc}`;
    const r=document.createElement("div"); r.className="res "+(ok?"ok":"bad"); r.textContent=ok?"SUCCESS":"FAIL"; wrap.appendChild(r);
    sfx(ok?'good':'bad'); setTimeout(()=>proceed(ok), 560);
  }
}
function endScene(id){
  const script=document.getElementById("script");
  const e=document.createElement("div"); e.className="ending";
  e.textContent = isEnd(id) ? "▪ The conversation comes to rest here." : "▸ The thread continues elsewhere in the saga.";
  script.appendChild(e);
  const wrap=document.createElement("div");
  const again=document.createElement("button"); again.className="restart"; again.textContent="↻ Play again";
  again.onclick=()=>startConv(CONVS.indexOf(curConv)); wrap.appendChild(again);
  script.appendChild(wrap); scrollEnd();
}

// ---- transcript + state ----
function addLine(cls, who, body, sigilName){
  const w=document.getElementById("script"); const d=document.createElement("div"); d.className="line "+cls;
  let sig="";
  if(who && cls!=="me" && cls!=="sys"){ const c=CAST.find(x=>x.name===who), hue=c?c.hue:hue0(who), si=c?c.sigil:(who[0]||"?").toUpperCase();
    sig=`<span class="s2" style="background:hsl(${hue} 52% 62%)">${esc(si)}</span>`; }
  d.innerHTML=(who?`<div class="who">${sig}${esc(who)}</div>`:"")+`<div class="body">${cls==="sys"?esc(body):prose(body)}</div>`;
  w.appendChild(d); if(cls!=="sys") linkifyEl(d.querySelector(".body"));
}
function addSense(text){ const w=document.getElementById("script"); const d=document.createElement("div"); d.className="sense";
  d.innerHTML=`<span class="lk">✦ The Returned senses —</span><div>${prose(text)}</div>`; w.appendChild(d); linkifyEl(d); }
function addLore(e){ const w=document.getElementById("script"); const d=document.createElement("div"); d.className="lore";
  let h=`<span class="lk">👁 ${esc(e.skill)} (passive) — you recall:</span> ${prose(e.lore)}`;
  if(e.secret && secretKnown(char,e,MODEL)) h+=`<div class="deep"><span class="lk" style="color:#e7c873">…and, deeper (${esc(e.secretSkill)} ${e.secretDc}):</span> ${prose(e.secret)}</div>`;
  d.innerHTML=h; w.appendChild(d); linkifyEl(d); }
function linkifyEl(el){ if(!el||!GLOSS_RE) return;
  const walker=document.createTreeWalker(el, NodeFilter.SHOW_TEXT, null); const nodes=[]; while(walker.nextNode()) nodes.push(walker.currentNode);
  const used={};
  nodes.forEach(node=>{ if(node.parentNode && node.parentNode.classList && node.parentNode.classList.contains("gloss")) return;
    let txt=node.nodeValue, m, idx=0, frag=null; const re=new RegExp(GLOSS_RE.source,"gi");
    while((m=re.exec(txt))){ const hit=GLOSS_INDEX.find(o=>o.t.toLowerCase()===m[0].toLowerCase()); if(!hit||used[hit.i]) continue; used[hit.i]=true;
      frag=frag||document.createDocumentFragment(); frag.appendChild(document.createTextNode(txt.slice(idx,m.index)));
      const span=document.createElement("span"); span.className="gloss"; span.dataset.k=hit.i; span.textContent=m[0]; frag.appendChild(span); idx=m.index+m[0].length; }
    if(frag){ frag.appendChild(document.createTextNode(txt.slice(idx))); node.parentNode.replaceChild(frag,node); }
  });
}
const tip=document.getElementById("tip");
document.addEventListener("mouseover",e=>{ const g=e.target.closest&&e.target.closest(".gloss"); if(!g) return;
  const e0=GLOSS[+g.dataset.k]; if(!e0) return; const known=loreKnown(char,e0,MODEL), deep=secretKnown(char,e0,MODEL);
  tip.innerHTML=`<div class="tn">${esc(e0.term)}</div>${esc(e0.gloss)}`+
    (e0.skill?(known?`<div class="tl"><b>${esc(e0.skill)} — you recall:</b> ${esc(e0.lore)}</div>`:`<div class="tl" style="color:#6e6680">(a deeper ${esc(e0.skill)} insight escapes you)</div>`):"")+
    (deep?`<div class="tl" style="border-color:#5a4a2a"><b style="color:#e7c873">${esc(e0.secretSkill)} — and deeper:</b> ${esc(e0.secret)}</div>`:"");
  tip.style.display="block"; moveTip(e); });
document.addEventListener("mousemove",e=>{ if(tip.style.display==="block" && e.target.closest&&e.target.closest(".gloss")) moveTip(e); });
document.addEventListener("mouseout",e=>{ if(e.target.closest&&e.target.closest(".gloss")) tip.style.display="none"; });
function moveTip(e){ const pad=14,w=tip.offsetWidth||280,h=tip.offsetHeight||80; let x=e.clientX+pad,y=e.clientY+pad;
  if(x+w>innerWidth-8) x=e.clientX-w-pad; if(y+h>innerHeight-8) y=e.clientY-h-pad; tip.style.left=Math.max(8,x)+"px"; tip.style.top=Math.max(8,y)+"px"; }
function scrollEnd(){ window.scrollTo(0, document.body.scrollHeight); }
function renderState(){
  const apps=[];
  Object.keys(st.ints).forEach(k=>{ if(k.indexOf("disp.")===0) return;
    const m=k.match(/^companion\.(\w+)\.approval$/);
    if(m) apps.push({name: COMPANION_NAMES[m[1]]||m[1], v:st.ints[k]});
    else apps.push({name: INT_LABELS[k]||k, v:st.ints[k]});
  });
  const ap=document.getElementById("approvals");
  ap.innerHTML = !apps.length ? `<div class="empty">No one's measure of you has moved yet.</div>` :
    apps.sort((a,b)=>Math.abs(b.v)-Math.abs(a.v)).map(a=>{ const w=Math.min(50,Math.abs(a.v)*6), neg=a.v<0;
      return `<div class="appr"><div class="nm">${esc(a.name)}</div>
        <div class="barwrap"><div class="bar${neg?' neg':''}" style="width:${w}%;${neg?'right:50%;left:auto;':''}"></div></div>
        <div class="delta ${a.v>=0?'up':'down'}">${a.v>=0?'+':''}${a.v}</div></div>`; }).join("");
  const disp=Object.keys(st.ints).filter(k=>k.indexOf("disp.")===0&&st.ints[k]>0).map(k=>({name:(DISP[k]||{}).name||k,hue:(DISP[k]||{}).hue||0,v:st.ints[k]}));
  const rk=document.getElementById("reckon");
  if(rk) rk.innerHTML = !disp.length ? `<div class="empty">Your choices haven't tilted you yet.</div>` :
    disp.sort((a,b)=>b.v-a.v).map(d=>`<div class="reckon"><div class="nm" style="color:hsl(${d.hue} 50% 70%)">${esc(d.name)}</div><div class="pips" style="color:hsl(${d.hue} 50% 64%)">${"◆".repeat(Math.min(6,d.v))}</div><div class="rk">${roman(d.v)}</div></div>`).join("");
  const keys=Object.keys(st.bools), fl=document.getElementById("flags");
  fl.innerHTML = !keys.length ? `<div class="empty">Nothing set yet — your choices write here.</div>` :
    keys.sort().map(k=>{ const dot=k.indexOf("."); const dom=dot<0?'':k.slice(0,dot)+'·';
      return `<span class="flagpill"><span class="d">${esc(dom)}</span>${esc(dot<0?k:k.slice(dot+1))}</span>`; }).join("")
      + `<div class="muted" style="margin-top:6px">${keys.length} flag${keys.length>1?'s':''} written — exactly what the real save would record.</div>`;
}

// ---- text helpers ----
function esc(s){ return (s==null?"":String(s)).replace(/[&<>]/g,c=>({"&":"&amp;","<":"&lt;",">":"&gt;"}[c])); }
function prose(s){ let h=esc(s);
  h=h.replace(/\*\(([\s\S]*?)\)\*/g,'<span class="stagedir">($1)</span>');
  h=h.replace(/\*([^*]+)\*/g,'<em>$1</em>'); return h; }
function stripBracket(t){ return (t||"").replace(/^\s*\[[^\]]*\]\s*/,""); }
function describeEffects(effects){
  return (effects||[]).map(e=>{
    if(e.op==="SetTrue") return `sets <span class="chip fx">${esc(e.key)}</span>`;
    if(e.op==="AddInt"){ const m=e.key.match(/^companion\.(\w+)\.approval$/); const who=m?(COMPANION_NAMES[m[1]]||m[1]):(INT_LABELS[e.key]||e.key);
      return `${esc(who)} ${e.amount>=0?'+':''}${e.amount}`; }
    return esc(e.key);
  }).join(" · ");
}

// ---- SFX ----
let sfxOn=true, actx=null;
function toggleSfx(){ sfxOn=!sfxOn; document.getElementById("sfxToggle").textContent=(sfxOn?'🔊':'🔇')+' sfx'; if(sfxOn) sfx('page'); }
function sfx(kind){ if(!sfxOn) return; try{ actx=actx||new (window.AudioContext||window.webkitAudioContext)(); }catch(e){ return; }
  const t=actx.currentTime, o=actx.createOscillator(), g=actx.createGain(); o.connect(g); g.connect(actx.destination);
  const env=(a,d,v)=>{ g.gain.setValueAtTime(0.0001,t); g.gain.exponentialRampToValueAtTime(v,t+a); g.gain.exponentialRampToValueAtTime(0.0001,t+a+d); };
  if(kind==='page'){ o.type='triangle'; o.frequency.setValueAtTime(420,t); o.frequency.exponentialRampToValueAtTime(640,t+0.06); env(0.008,0.07,0.05); o.start(t);o.stop(t+0.1); }
  else if(kind==='dice'){ o.type='square'; o.frequency.setValueAtTime(180,t); o.frequency.linearRampToValueAtTime(120,t+0.12); env(0.005,0.12,0.04); o.start(t);o.stop(t+0.14); }
  else if(kind==='good'){ o.type='sine'; o.frequency.setValueAtTime(523,t); o.frequency.setValueAtTime(784,t+0.09); env(0.01,0.22,0.07); o.start(t);o.stop(t+0.26); }
  else if(kind==='bad'){ o.type='sawtooth'; o.frequency.setValueAtTime(196,t); o.frequency.exponentialRampToValueAtTime(98,t+0.2); env(0.01,0.22,0.06); o.start(t);o.stop(t+0.26); }
}
window.addEventListener("keydown",e=>{ if(!pendingOpts) return; const n=parseInt(e.key,10);
  if(n>=1&&n<=9){ const btns=pendingOpts.el.querySelectorAll(".opt:not(.locked)"); if(btns[n-1]) btns[n-1].click(); } });

// ---- boot ----
renderBuilds(); renderCast("");
(function(){
  const want=decodeURIComponent(location.hash.replace(/^#/,""));
  if(want){ const ci=CONVS.findIndex(c=>c.id===want); if(ci>=0){ const ni=CAST.findIndex(c=>c.convs.includes(ci)); if(ni>=0){castSel=ni;renderCast("");} startConv(ci); return; } }
  const demoIdx=CONVS.findIndex(c=>c.demo);
  document.getElementById("stage").innerHTML=
    `<div class="pick" style="font-size:16px;margin-top:24px">Pick a character on the right — race, class, background, alignment, deity, and stats — then step into a conversation. The NPC answers <b>you</b>, specifically.</div>`+
    (demoIdx>=0?`<button class="cta" onclick="(function(){const ni=CAST.findIndex(c=>c.convs.includes(${demoIdx}));if(ni>=0){castSel=ni;renderCast('');}startConv(${demoIdx});})()">
       <b>✦ Try the reactive demo: “${esc(CONVS[demoIdx].title)}”</b>
       <div class="m">A gatekeeper of the dead reads your blood, your god, and your bearing — and decides whether you pass. Switch your character and replay to watch every line change. Try it as the Confessor (a kinsman), the Warden (Faithless), and the Silver Tongue (a tiefling) to feel the range.</div></button>`:'')+
    `<div class="muted" style="margin-top:10px">${DATA.stats.conversations||CONVS.length} conversations · ${DATA.stats.skillChecks||''} skill checks — every one playable here on the real engine.</div>`;
})();
window.addEventListener("hashchange",()=>{ const want=decodeURIComponent(location.hash.replace(/^#/,"")); const ci=CONVS.findIndex(c=>c.id===want); if(ci>=0) startConv(ci); });
</script>
</body></html>"""

out = (HTML
       .replace("__BLOB__", BLOB)
       .replace("__BUILDS__", json.dumps(BUILDS, ensure_ascii=False))
       .replace("__COMPANIONS__", json.dumps(COMPANION_NAMES, ensure_ascii=False))
       .replace("__INTLABELS__", json.dumps(INT_LABELS, ensure_ascii=False)))

dst = os.path.join(ROOT, "play", "dialogue_sim.html")
open(dst, "w", encoding="utf-8").write(out)
demo_n = sum(1 for c in conversations if c.get("demo"))
print(f"wrote play/dialogue_sim.html ({len(out)//1024} KB) — {len(conversations)} conversations "
      f"({demo_n} reactive demo), {len(cast_list)} speakers, {len(BUILDS)} builds, "
      f"{len(MODEL['races'])} races × {len(MODEL['deities'])} deities × {len(MODEL['classes'])} classes")
