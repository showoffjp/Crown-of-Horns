#!/usr/bin/env python3
"""
make-dialogue-sim.py — build play/dialogue_sim.html, an immersive, *playable* dialogue
simulator. Where dialogue_viewer.html shows the trees as graphs, this lets you step INTO
a conversation as your own "Returned": pick a character, make choices, roll real skill
checks (d20 + your ability modifier vs the DC, exactly as DialogueRunner.cs does), and
watch the story flags, companion approvals, and faction reputation move in real time.

Reuses play/dialogue-data.json (the same blob the viewer and saga map read). The pure
resolution logic lives in a /*<DLGSIM>*/...*/</DLGSIM>*/ block so dialogue_sim.test.js can
lift it and prove it matches the C# engine. Re-run:

    python3 tools/extract-dialogue.py     # refresh the data
    python3 tools/make-dialogue-sim.py    # rebuild this page
"""
import json, os, re, hashlib

ROOT = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
DATA = json.load(open(os.path.join(ROOT, "play", "dialogue-data.json"), encoding="utf-8"))

# Player/narration speakers we don't want to headline a conversation's "cast" entry.
PLAYER_LABELS = {"The Last Returned", "You", "Player", "The Returned"}

# The soul-bearing principals — companions + the major quest-givers — featured first.
FEATURED = {
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
    h = int(hashlib.md5(name.encode("utf-8")).hexdigest(), 16)
    return h % 360

def initials(name):
    # A compact sigil: up to two "strong" initials, ignoring leading articles.
    words = [w for w in re.sub(r"[^A-Za-z' ]", "", name).split() if w.lower() not in ("the", "a", "an", "of")]
    if not words:
        words = name.split()
    if len(words) == 1:
        return words[0][:2].upper()
    return (words[0][0] + words[-1][0]).upper()

def primary_speaker(conv):
    from collections import Counter
    c = Counter(n.get("speaker") for n in conv["nodes"] if n.get("speaker"))
    if not c:
        return "—"
    # Prefer the most common non-player speaker; fall back to whatever's there.
    for name, _ in c.most_common():
        if name not in PLAYER_LABELS:
            return name
    return c.most_common(1)[0][0]

# Annotate conversations with their headline speaker and build the cast index.
from collections import defaultdict
cast = defaultdict(list)
for i, conv in enumerate(DATA["conversations"]):
    ps = primary_speaker(conv)
    conv["primarySpeaker"] = ps
    cast[ps].append(i)

cast_list = []
for name, idxs in cast.items():
    cast_list.append({
        "name": name,
        "convs": idxs,
        "count": len(idxs),
        "featured": name in FEATURED,
        "hue": hue_of(name),
        "sigil": initials(name),
    })
# Featured first, then by number of conversations, then alphabetical.
cast_list.sort(key=lambda c: (not c["featured"], -c["count"], c["name"].lower()))

EMBED = {"conversations": DATA["conversations"], "cast": cast_list, "stats": DATA.get("stats", {})}
BLOB = json.dumps(EMBED, ensure_ascii=False, separators=(",", ":"))

# Authentic starting builds — the five real classes (CharacterBuilder.cs), with ability
# arrays chosen so the three dialogue abilities (INT/WIS/CHA) genuinely differ between them.
# Order: STR, DEX, CON, INT, WIS, CHA.
BUILDS = [
    {"name": "The Returned",  "cls": "Fighter", "scores": [16, 14, 15, 10, 12, 13], "blurb": "The default pilgrim — steady, plain-spoken."},
    {"name": "The Scholar",   "cls": "Wizard",  "scores": [10, 14, 12, 16, 14, 11], "blurb": "Reads the world; strong INSIGHT & lore."},
    {"name": "The Confessor", "cls": "Cleric",  "scores": [13, 10, 14, 11, 17, 12], "blurb": "Sees through people; the best WISDOM."},
    {"name": "The Silver Tongue", "cls": "Rogue", "scores": [10, 16, 12, 13, 11, 16], "blurb": "Charms and cons; the best CHARISMA."},
    {"name": "The Warden",    "cls": "Ranger",  "scores": [14, 16, 13, 11, 14, 10], "blurb": "Quiet and watchful; even-handed."},
]

# Friendly names for companion approval & faction reputation int-flags.
COMPANION_NAMES = {
    "garrow": "Sister Garrow", "roen": "Roen", "varra": "Varra", "naeve": "Naeve",
    "ilfaeril": "Ilfaeril", "maerin": "Maerin",
}

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
 .layout{display:grid;grid-template-columns:262px minmax(0,1fr) 312px;gap:0;align-items:stretch;min-height:calc(100vh - 56px)}
 .rail{border-right:1px solid #2a2636;padding:14px 12px;overflow:auto;max-height:calc(100vh - 56px)}
 .rail.right{border-right:0;border-left:1px solid #2a2636}
 .rail h2{font-size:11px;letter-spacing:1.5px;text-transform:uppercase;color:#c9a24b;margin:2px 0 10px;font-weight:600}
 .search{width:100%;background:#15131d;border:1px solid #2a2636;color:#d8d2c2;border-radius:7px;padding:8px 10px;font:inherit;font-size:13px;margin-bottom:10px}
 .grp{font-size:10px;letter-spacing:1px;text-transform:uppercase;color:#6e6680;margin:12px 0 5px}
 .npc{display:flex;align-items:center;gap:9px;padding:6px 7px;border-radius:8px;cursor:pointer;border:1px solid transparent}
 .npc:hover{background:#17151f;border-color:#2a2636}
 .npc.on{background:#1d1a28;border-color:#c9a24b66}
 .sig{width:30px;height:30px;border-radius:50%;flex:0 0 auto;display:flex;align-items:center;justify-content:center;
   font-size:11.5px;font-weight:700;color:#0c0b10;font-family:"Iowan Old Style",Georgia,serif}
 .npc .nm{flex:1;font-size:13.5px;line-height:1.25}
 .npc .ct{font-size:10.5px;color:#7f7790}
 .star{color:#e7c873;font-size:10px}
 /* stage */
 .stage{padding:22px 26px 90px;max-width:760px;margin:0 auto;width:100%}
 .pick{color:#9a90a8;font-style:italic;margin:6px 0 16px}
 .convlist{display:flex;flex-direction:column;gap:8px;margin-top:6px}
 .convbtn{text-align:left;background:linear-gradient(#16141d,#121019);border:1px solid #2a2636;color:#d8d2c2;
   border-radius:9px;padding:11px 13px;cursor:pointer;font:inherit}
 .convbtn:hover{border-color:#c9a24b;transform:translateY(-1px)}
 .convbtn .t{color:#e7c873;font-size:15px}
 .convbtn .m{color:#7f7790;font-size:11.5px;margin-top:2px}
 .nowplaying{display:flex;align-items:center;gap:12px;margin-bottom:14px;padding-bottom:12px;border-bottom:1px solid #2a2636}
 .nowplaying .big{width:46px;height:46px;border-radius:50%;font-size:16px}
 .nowplaying .tt{font-size:13px;color:#9a90a8}.nowplaying .tt b{color:#e7c873;font-size:17px;font-weight:600}
 .script{display:flex;flex-direction:column;gap:14px;margin-bottom:18px}
 .line{opacity:0;animation:fade .35s ease forwards}
 @keyframes fade{to{opacity:1}}
 .line .who{font-size:11px;letter-spacing:1px;text-transform:uppercase;color:#c9a24b;margin-bottom:3px;display:flex;align-items:center;gap:7px}
 .line .who .s2{width:18px;height:18px;border-radius:50%;font-size:8px;font-weight:700;color:#0c0b10;display:inline-flex;align-items:center;justify-content:center}
 .line .body{font-size:15.5px}
 .line.me{align-self:flex-end;max-width:86%;text-align:right}
 .line.me .who{color:#9fc0e0;justify-content:flex-end}
 .line.me .body{color:#cfe0f0;background:#15192a;border:1px solid #243049;border-radius:11px 11px 2px 11px;padding:8px 12px;display:inline-block;text-align:left}
 .line.sys .body{font-size:13px;color:#b9a8e0;font-style:italic}
 .stagedir{color:#8a8198;font-style:italic}
 em{color:#e7c873;font-style:italic}
 .opts{display:flex;flex-direction:column;gap:9px;margin-top:6px}
 .opt{text-align:left;background:linear-gradient(#1b1726,#16121f);border:1px solid #34304a;color:#e8e2d2;
   border-radius:10px;padding:11px 14px;cursor:pointer;font:inherit;font-size:14.5px;transition:.12s;position:relative}
 .opt:hover{border-color:#c9a24b;background:linear-gradient(#241d33,#1b1628)}
 .opt .num{color:#6e6680;font-size:11px;margin-right:7px}
 .opt.locked{opacity:.5;cursor:not-allowed;border-style:dashed}
 .opt .ck{margin-top:6px;font-size:11.5px;color:#b9a8e0;display:flex;gap:8px;flex-wrap:wrap;align-items:center}
 .chip{display:inline-block;font-size:10.5px;border:1px solid #3a3550;border-radius:5px;padding:1px 6px;color:#c9a24b}
 .chip.ok{color:#9fe0b0;border-color:#2c4a32}.chip.bad{color:#e0a0a0;border-color:#5a2c2c}
 .chip.fx{color:#9ec8e8;border-color:#27405a}
 .opt .fxline{margin-top:5px;font-size:11px;color:#7f8aa0}
 .continue{background:linear-gradient(#26212f,#1a1622);color:#e7c873;border:1px solid #c9a24b66;border-radius:8px;padding:9px 16px;cursor:pointer;font:inherit;font-size:13.5px;align-self:flex-start}
 .continue:hover{border-color:#c9a24b}
 .restart{background:#15131d;color:#9a90a8;border:1px solid #2a2636;border-radius:8px;padding:8px 14px;cursor:pointer;font:inherit;font-size:13px;margin-top:8px}
 .restart:hover{border-color:#c9a24b;color:#e7c873}
 .ending{margin-top:14px;padding:12px 14px;border:1px dashed #3a3550;border-radius:9px;color:#9a90a8;font-style:italic}
 /* dice */
 .dice{display:flex;align-items:center;gap:12px;background:#14121c;border:1px solid #2a2636;border-radius:10px;padding:10px 14px;margin:2px 0}
 .d20{width:42px;height:42px;flex:0 0 auto;border-radius:8px;background:radial-gradient(circle at 40% 35%,#2e2740,#15121d);
   border:1px solid #4a4368;display:flex;align-items:center;justify-content:center;font-size:18px;font-weight:700;color:#e7c873;font-variant-numeric:tabular-nums}
 .d20.rolling{animation:shake .5s ease}
 @keyframes shake{0%,100%{transform:translateY(0) rotate(0)}25%{transform:translateY(-3px) rotate(-8deg)}50%{transform:translateY(2px) rotate(7deg)}75%{transform:translateY(-2px) rotate(-5deg)}}
 .dice .calc{font-size:12.5px;color:#b9a8e0}
 .dice .res{font-weight:700;margin-left:auto}
 .dice .res.ok{color:#9fe0b0}.dice .res.bad{color:#e0a0a0}
 /* right rail: sheet + state */
 .card{background:linear-gradient(#16141d,#131119);border:1px solid #2a2636;border-radius:9px;padding:12px;margin-bottom:13px}
 .card h3{font-size:10.5px;letter-spacing:1.3px;text-transform:uppercase;color:#c9a24b;margin:0 0 9px;font-weight:600}
 .builds{display:flex;flex-wrap:wrap;gap:5px;margin-bottom:9px}
 .bld{font:inherit;font-size:11.5px;background:#1a1622;border:1px solid #2a2636;color:#c8c0b4;border-radius:6px;padding:5px 8px;cursor:pointer}
 .bld.on{border-color:#e7c873;color:#e7c873;background:#241d2e}
 .abil{display:grid;grid-template-columns:repeat(3,1fr);gap:6px}
 .ab{background:#14121b;border:1px solid #232030;border-radius:7px;padding:6px 4px;text-align:center}
 .ab .k{font-size:9.5px;letter-spacing:.5px;color:#7f7790}
 .ab .v{font-size:16px;font-variant-numeric:tabular-nums}
 .ab .m{font-size:11px;color:#9fe0b0}
 .ab.dlg{border-color:#4a4368;box-shadow:0 0 0 1px #6a5a9a33}
 .ab .step{display:flex;gap:3px;justify-content:center;margin-top:3px}
 .ab .step button{width:18px;height:16px;line-height:13px;font-size:11px;background:#221d2b;border:1px solid #34304a;color:#c9a24b;border-radius:4px;cursor:pointer;padding:0}
 .toggle{display:flex;align-items:center;gap:7px;font-size:12px;color:#b8b0c4;margin:6px 0;cursor:pointer}
 .toggle input{accent-color:#c9a24b}
 .stateline{font-size:12px;color:#a89fb4;padding:2px 0;border-bottom:1px solid #1c1a26}
 .stateline .k{color:#8a8198;font-size:10.5px}
 .empty{color:#6e6680;font-style:italic;font-size:12px}
 .appr{display:flex;align-items:center;gap:7px;margin:5px 0;font-size:12px}
 .appr .nm{width:84px;color:#c8c0b4}
 .appr .barwrap{flex:1;height:8px;background:#1c1a26;border-radius:4px;overflow:hidden;position:relative}
 .appr .bar{position:absolute;left:50%;top:0;bottom:0;background:linear-gradient(#4a8a5a,#6fd08a)}
 .appr .bar.neg{background:linear-gradient(#9b2d2d,#d06f6f);transform-origin:right}
 .appr .delta{width:34px;text-align:right;font-variant-numeric:tabular-nums}
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
 <span class="sub">step into any conversation — make the choices, roll the checks, watch the story remember you</span>
 <a class="home" href="index.html">← all previews</a>
</header>
<div class="layout">
 <div class="rail" id="castRail">
  <h2>The Cast</h2>
  <input class="search" id="search" placeholder="search 187 speakers…" oninput="renderCast(this.value)">
  <div id="castList"></div>
 </div>
 <div class="stage" id="stage"></div>
 <div class="rail right">
  <div class="card">
   <h3>Your Returned <button class="sfxbtn" id="sfxToggle" onclick="toggleSfx()">🔊 sfx</button></h3>
   <div class="builds" id="builds"></div>
   <div class="muted" id="buildBlurb"></div>
   <div class="abil" id="abil" style="margin-top:9px"></div>
   <div class="muted" style="margin-top:7px">INT · WIS · CHA drive every dialogue check. Tap ▲▼ to preview any build.</div>
  </div>
  <div class="card">
   <h3>Preview tools</h3>
   <label class="toggle"><input type="checkbox" id="tReveal" onchange="rerenderChoices()"> Reveal what each choice does</label>
   <label class="toggle"><input type="checkbox" id="tForce" onchange="rerenderChoices()"> Let me decide check results</label>
   <label class="toggle"><input type="checkbox" id="tAssume" onchange="rerenderChoices()"> Assume earlier story flags</label>
  </div>
  <div class="card">
   <h3>Companion approval</h3>
   <div id="approvals"><div class="empty">No bonds moved yet.</div></div>
  </div>
  <div class="card">
   <h3>Flags this run</h3>
   <div id="flags"><div class="empty">Nothing set yet — your choices write here.</div></div>
  </div>
 </div>
</div>
<script>
const DATA = __BLOB__;
const BUILDS = __BUILDS__;
const COMPANION_NAMES = __COMPANIONS__;
const CONVS = DATA.conversations, CAST = DATA.cast;
const ABILS = ["Strength","Dexterity","Constitution","Intelligence","Wisdom","Charisma"];
const ABBR  = {Strength:"STR",Dexterity:"DEX",Constitution:"CON",Intelligence:"INT",Wisdom:"WIS",Charisma:"CHA"};

/*<DLGSIM>*/
// Pure, testable resolution — mirrors Assets/Scripts/Dialogue/DialogueRunner.cs + Abilities.cs.
function abilityMod(score){ return Math.floor((score - 10) / 2); }              // floor((score-10)/2)
function resolveCheck(roll, dc, mod){ return (roll + mod) >= dc; }              // roll = d20+mod; success = roll>=DC
function chanceToPass(dc, mod){                                                 // P(d20+mod >= DC), no nat-20 auto
  const faces = 21 - (dc - mod);                                               // # of d20 faces that clear it
  return Math.max(0, Math.min(20, faces)) / 20;
}
function newState(){ return { bools:{}, ints:{} }; }
function applyEffects(state, effects){
  (effects||[]).forEach(e=>{
    if(e.op==="SetTrue") state.bools[e.key] = true;
    else if(e.op==="AddInt") state.ints[e.key] = (state.ints[e.key]||0) + (e.amount||0);
  });
  return state;
}
function conditionsPass(state, conditions, assumePrior){
  return (conditions||[]).every(c=>{
    if(c.op==="RequireBoolTrue") return state.bools[c.key]===true || !!assumePrior;
    return true;
  });
}
/*</DLGSIM>*/

// ---- player build / ability scores ----
let build = JSON.parse(JSON.stringify(BUILDS[0]));
let buildIdx = 0;
function modFor(abilityName){ const i = ABILS.indexOf(abilityName); return abilityMod(build.scores[i]); }

function renderBuilds(){
  document.getElementById("builds").innerHTML = BUILDS.map((b,i)=>
    `<button class="bld${i===buildIdx?' on':''}" onclick="setBuild(${i})">${esc(b.name)}</button>`).join("");
  document.getElementById("buildBlurb").textContent = build.blurb || "";
  renderAbil();
}
function setBuild(i){ buildIdx=i; build=JSON.parse(JSON.stringify(BUILDS[i])); renderBuilds(); rerenderChoices(); }
function bump(i,d){ build.scores[i]=Math.max(1,Math.min(20,build.scores[i]+d)); build.name="Custom"; buildIdx=-1; renderBuilds(); rerenderChoices(); }
function renderAbil(){
  document.getElementById("abil").innerHTML = ABILS.map((a,i)=>{
    const m=abilityMod(build.scores[i]); const isDlg=(a==="Intelligence"||a==="Wisdom"||a==="Charisma");
    return `<div class="ab${isDlg?' dlg':''}"><div class="k">${ABBR[a]}</div><div class="v">${build.scores[i]}</div>
      <div class="m">${m>=0?'+':''}${m}</div>
      <div class="step"><button onclick="bump(${i},-1)">▼</button><button onclick="bump(${i},1)">▲</button></div></div>`;
  }).join("");
}

// ---- cast browser ----
function renderCast(q){
  q=(q||"").toLowerCase();
  const featured=[], rest=[];
  CAST.forEach((c,i)=>{ if(q && !c.name.toLowerCase().includes(q)) return; (c.featured?featured:rest).push(i); });
  let h="";
  const row=i=>{ const c=CAST[i];
    return `<div class="npc${i===castSel?' on':''}" onclick="pickNpc(${i})">
      <div class="sig" style="background:hsl(${c.hue} 52% 62%)">${esc(c.sigil)}</div>
      <div class="nm">${esc(c.name)} ${c.featured?'<span class="star">★</span>':''}<div class="ct">${c.count} conversation${c.count>1?'s':''}</div></div></div>`; };
  if(featured.length){ h+=`<div class="grp">Principal cast</div>`+featured.map(row).join(""); }
  if(rest.length){ h+=`<div class="grp">Everyone else</div>`+rest.map(row).join(""); }
  if(!h) h=`<div class="empty">No speaker matches “${esc(q)}”.</div>`;
  document.getElementById("castList").innerHTML=h;
}
let castSel=-1;
function pickNpc(i){ castSel=i; renderCast(document.getElementById("search").value); showConvList(i); }

function showConvList(i){
  const c=CAST[i];
  const items=c.convs.map(ci=>{ const cv=CONVS[ci];
    const nodes=cv.nodes.length, choices=cv.nodes.reduce((a,n)=>a+n.choices.length,0),
          checks=cv.nodes.reduce((a,n)=>a+n.choices.filter(x=>x.checkDC).length,0);
    return `<button class="convbtn" onclick="startConv(${ci})">
      <div class="t">${esc(cv.title)}</div>
      <div class="m">${nodes} beats · ${choices} choices${checks?` · 🎲 ${checks} skill check${checks>1?'s':''}`:''}</div></button>`;
  }).join("");
  document.getElementById("stage").innerHTML =
    `<div class="nowplaying"><div class="sig big" style="background:hsl(${c.hue} 52% 62%)">${esc(c.sigil)}</div>
       <div class="tt">Conversations with<br><b>${esc(c.name)}</b></div></div>
     <div class="pick">Choose a conversation to step into. Your build and the preview tools on the right apply live.</div>
     <div class="convlist">${items}</div>`;
}

// ---- the play engine ----
let st=newState(), curConv=null, history=[];
function startConv(ci){
  curConv=CONVS[ci]; st=newState(); history=[];
  const c=CAST[castSel] || {hue:hue0(curConv.primarySpeaker), sigil:"…", name:curConv.primarySpeaker};
  const start=curConv.nodes.find(n=>n.id===curConv.start)?curConv.start:curConv.nodes[0].id;
  document.getElementById("stage").innerHTML =
    `<div class="nowplaying"><div class="sig big" style="background:hsl(${(CAST[castSel]?CAST[castSel].hue:hue0(curConv.primarySpeaker))} 52% 62%)">${esc(CAST[castSel]?CAST[castSel].sigil:'…')}</div>
       <div class="tt"><b>${esc(curConv.title)}</b><br>with ${esc(curConv.primarySpeaker)}</div></div>
     <div class="script" id="script"></div>`;
  renderState();
  goNode(start, true);
}
function hue0(n){ let h=0; for(const ch of (n||"")) h=(h*31+ch.charCodeAt(0))%360; return h; }

function nodeById(id){ return curConv.nodes.find(n=>n.id===id); }
function isEnd(id){ return !id || id==="END" || id==="end" || !nodeById(id); }

let pendingOpts=null; // remember the live choice container for re-render on toggle/build change
function goNode(id, first){
  pendingOpts=null;
  const n=nodeById(id);
  const script=document.getElementById("script");
  if(!n){ endScene(id); return; }
  applyEffects(st, n.onEnter); renderState();
  addLine("", n.speaker, n.dynamic? "〔This line is written live from your story flags — shown here as a placeholder.〕" : (n.text||""), n.speaker);
  const opts=document.createElement("div"); opts.className="opts"; script.appendChild(opts);
  const all=n.choices||[];
  if(all.length){
    pendingOpts={node:n, el:opts};
    paintChoices();
  } else if(n.auto && !isEnd(n.auto)){
    const b=document.createElement("button"); b.className="continue"; b.textContent="Continue ▸";
    b.onclick=()=>{ sfx('page'); opts.remove(); goNode(n.auto); };
    opts.appendChild(b);
  } else {
    endScene(n.auto);
  }
  scrollEnd();
}
function paintChoices(){
  if(!pendingOpts) return;
  const {node:n, el:opts}=pendingOpts; opts.innerHTML="";
  const reveal=document.getElementById("tReveal").checked;
  const assume=document.getElementById("tAssume").checked;
  const force=document.getElementById("tForce").checked;
  n.choices.forEach((ch,idx)=>{
    const ok=conditionsPass(st, ch.conditions, assume);
    const b=document.createElement("button"); b.className="opt"+(ok?"":" locked");
    let inner=`<span class="num">${idx+1}</span>${esc(stripBracket(ch.text)||"(continue)")}`;
    if(ch.checkDC){
      const mod=modFor(ch.checkAbility), pct=Math.round(chanceToPass(ch.checkDC,mod)*100);
      const sk=bracketLabel(ch.text);
      inner+=`<div class="ck"><span class="chip">🎲 ${sk?esc(sk)+' · ':''}${ABBR[ch.checkAbility]||esc(ch.checkAbility)} DC ${ch.checkDC}</span>`+
             `<span class="chip ${pct>=50?'ok':'bad'}">${pct}% with ${esc(build.name)} (${mod>=0?'+':''}${mod})</span>`+
             (ch.fail?`<span class="muted">miss ▸ a different path</span>`:'')+`</div>`;
    }
    if(!ok){ const need=(ch.conditions||[]).filter(c=>c.op==="RequireBoolTrue").map(c=>c.key); inner+=`<div class="fxline">🔒 needs earlier: ${need.map(esc).join(", ")}</div>`; }
    if(reveal){ const fx=describeEffects(ch.effects); if(fx) inner+=`<div class="fxline">▸ ${fx}</div>`; }
    b.innerHTML=inner;
    if(ok) b.onclick=()=>choose(n, ch, idx, force);
    opts.appendChild(b);
  });
}
function rerenderChoices(){ renderBuilds && renderAbil(); paintChoices(); }

function choose(n, ch, idx, force){
  sfx('page');
  // lock the panel: replace with the chosen line
  pendingOpts.el.remove(); pendingOpts=null;
  addLine("me","You", stripBracket(ch.text)||"(continue)");
  history.push(JSON.parse(JSON.stringify(st)));
  let next=ch.next;
  const proceed=(success)=>{
    if(ch.checkDC && !success && ch.fail) next=ch.fail;
    applyEffects(st, ch.effects); renderState();
    goNode(next);
  };
  if(ch.checkDC){
    const mod=modFor(ch.checkAbility);
    if(force){ offerForcedResult(ch, mod, proceed); return; }
    rollDice(ch, mod, proceed);
  } else {
    applyEffects(st, ch.effects); renderState(); goNode(next);
  }
}
function offerForcedResult(ch, mod, proceed){
  const script=document.getElementById("script");
  const box=document.createElement("div"); box.className="opts";
  box.innerHTML=`<div class="muted" style="margin-bottom:4px">Preview either branch of this ${ABBR[ch.checkAbility]||ch.checkAbility} DC ${ch.checkDC} check:</div>`;
  const mk=(label,cls,ok)=>{ const b=document.createElement("button"); b.className="opt"; b.innerHTML=`<span class="chip ${cls}">${label}</span>`;
    b.onclick=()=>{ box.remove(); addLine("sys","",`(forced ${ok?'SUCCESS':'FAILURE'})`); proceed(ok); }; return b; };
  box.appendChild(mk("✓ Take the success branch","ok",true));
  box.appendChild(mk("✗ Take the failure branch","bad",false));
  script.appendChild(box); scrollEnd();
}
function rollDice(ch, mod, proceed){
  const script=document.getElementById("script");
  const wrap=document.createElement("div"); wrap.className="dice";
  wrap.innerHTML=`<div class="d20 rolling" id="d20">?</div><div class="calc">rolling ${ABBR[ch.checkAbility]||ch.checkAbility}…</div>`;
  script.appendChild(wrap); scrollEnd(); sfx('dice');
  let ticks=0; const die=wrap.querySelector("#d20"); const calc=wrap.querySelector(".calc");
  const spin=setInterval(()=>{ die.textContent=1+Math.floor(Math.random()*20); if(++ticks>=12){ clearInterval(spin); land(); } }, 42);
  function land(){
    const roll=1+Math.floor(Math.random()*20); const total=roll+mod; const ok=resolveCheck(roll, ch.checkDC, mod);
    die.classList.remove("rolling"); die.textContent=roll;
    calc.innerHTML=`${roll} ${mod>=0?'+':'−'} ${Math.abs(mod)} <b>= ${total}</b> vs DC ${ch.checkDC}`;
    const r=document.createElement("div"); r.className="res "+(ok?"ok":"bad"); r.textContent=ok?"SUCCESS":"FAIL"; wrap.appendChild(r);
    sfx(ok?'good':'bad');
    setTimeout(()=>proceed(ok), 520);
  }
}

function endScene(id){
  const script=document.getElementById("script");
  const e=document.createElement("div"); e.className="ending";
  e.textContent = isEnd(id) ? "▪ The conversation comes to rest here." : "▸ The thread continues elsewhere in the saga.";
  script.appendChild(e);
  const wrap=document.createElement("div");
  const again=document.createElement("button"); again.className="restart"; again.textContent="↻ Play this conversation again";
  again.onclick=()=>startConv(CONVS.indexOf(curConv));
  wrap.appendChild(again);
  if(history.length){ const back=document.createElement("button"); back.className="restart"; back.style.marginLeft="8px";
    back.textContent="↶ Back to the last choice"; back.onclick=rewind; wrap.appendChild(back); }
  script.appendChild(wrap); scrollEnd();
}
function rewind(){
  // Rebuild from scratch up to the previous decision is complex; simplest faithful preview:
  // restore the flag/int state to before the last choice and replay from the conversation start visually.
  if(!history.length) return;
  st = history.pop();
  // re-render state; keep the transcript but let the player continue exploring from a fresh play.
  renderState();
  addLineRaw('<div class="muted">↶ Reverted the story state to before your last choice. Replay to explore the other branch.</div>');
  scrollEnd();
}

// ---- transcript helpers ----
function addLine(cls, who, body, sigilName){
  const w=document.getElementById("script"); const d=document.createElement("div"); d.className="line "+cls;
  let sig="";
  if(who && cls!=="me" && cls!=="sys"){ const c=CAST.find(x=>x.name===who); const hue=c?c.hue:hue0(who); const si=c?c.sigil:(who[0]||"?").toUpperCase();
    sig=`<span class="s2" style="background:hsl(${hue} 52% 62%)">${esc(si)}</span>`; }
  d.innerHTML=(who?`<div class="who">${sig}${esc(who)}</div>`:"")+`<div class="body">${cls==="sys"?esc(body):prose(body)}</div>`;
  w.appendChild(d);
}
function addLineRaw(html){ const w=document.getElementById("script"); const d=document.createElement("div"); d.className="line"; d.innerHTML=html; w.appendChild(d); }
function scrollEnd(){ window.scrollTo(0, document.body.scrollHeight); }

// ---- state panel ----
function renderState(){
  // approvals (companion.X.approval) + faction reputation deltas
  const apps=[]; const others=[];
  Object.keys(st.ints).forEach(k=>{
    const m=k.match(/^companion\.(\w+)\.approval$/);
    if(m){ apps.push({name: COMPANION_NAMES[m[1]]||m[1], v:st.ints[k]}); }
    else others.push({k, v:st.ints[k]});
  });
  const ap=document.getElementById("approvals");
  if(!apps.length && !others.length){ ap.innerHTML=`<div class="empty">No bonds moved yet.</div>`; }
  else {
    ap.innerHTML = apps.sort((a,b)=>Math.abs(b.v)-Math.abs(a.v)).map(a=>{
      const w=Math.min(50, Math.abs(a.v)); const neg=a.v<0;
      return `<div class="appr"><div class="nm">${esc(a.name)}</div>
        <div class="barwrap"><div class="bar${neg?' neg':''}" style="width:${w}%;${neg?'right:50%;left:auto;':''}"></div></div>
        <div class="delta ${a.v>=0?'up':'down'}">${a.v>=0?'+':''}${a.v}</div></div>`;
    }).join("") + others.map(o=>`<div class="stateline"><span class="k">${esc(o.k)}:</span> ${o.v>=0?'+':''}${o.v}</div>`).join("");
  }
  // flags (bool) grouped by domain prefix
  const keys=Object.keys(st.bools);
  const fl=document.getElementById("flags");
  if(!keys.length){ fl.innerHTML=`<div class="empty">Nothing set yet — your choices write here.</div>`; }
  else {
    fl.innerHTML = keys.sort().map(k=>{ const dot=k.indexOf("."); const dom=dot<0?'':k.slice(0,dot)+'·';
      return `<span class="flagpill"><span class="d">${esc(dom)}</span>${esc(dot<0?k:k.slice(dot+1))}</span>`; }).join("")
      + `<div class="muted" style="margin-top:6px">${keys.length} flag${keys.length>1?'s':''} written — these are exactly what the real save would record.</div>`;
  }
}

// ---- text helpers ----
function esc(s){ return (s==null?"":String(s)).replace(/[&<>]/g,c=>({"&":"&amp;","<":"&lt;",">":"&gt;"}[c])); }
function prose(s){ // stage directions *( ... )* dim; *emphasis* gold
  let h=esc(s);
  h=h.replace(/\*\(([\s\S]*?)\)\*/g,'<span class="stagedir">($1)</span>');
  h=h.replace(/\*([^*]+)\*/g,'<em>$1</em>');
  return h;
}
function stripBracket(t){ return (t||"").replace(/^\s*\[[^\]]*\]\s*/,""); }
function bracketLabel(t){ const m=(t||"").match(/^\s*\[([^\]\s]+)/); return m?m[1]:""; }
function describeEffects(effects){
  return (effects||[]).map(e=>{
    if(e.op==="SetTrue") return `sets <span class="chip fx">${esc(e.key)}</span>`;
    if(e.op==="AddInt"){ const m=e.key.match(/^companion\.(\w+)\.approval$/);
      const who=m?(COMPANION_NAMES[m[1]]||m[1]):e.key; return `${esc(who)} ${e.amount>=0?'+':''}${e.amount}`; }
    return esc(e.key);
  }).join(" · ");
}

// ---- tiny synthesized SFX (WebAudio), in the spirit of the combat sim ----
let sfxOn=true, actx=null;
function toggleSfx(){ sfxOn=!sfxOn; document.getElementById("sfxToggle").textContent=(sfxOn?'🔊':'🔇')+' sfx'; if(sfxOn) sfx('page'); }
function sfx(kind){
  if(!sfxOn) return; try{ actx=actx||new (window.AudioContext||window.webkitAudioContext)(); }catch(e){ return; }
  const t=actx.currentTime; const o=actx.createOscillator(), g=actx.createGain(); o.connect(g); g.connect(actx.destination);
  const env=(a,d,v)=>{ g.gain.setValueAtTime(0.0001,t); g.gain.exponentialRampToValueAtTime(v,t+a); g.gain.exponentialRampToValueAtTime(0.0001,t+a+d); };
  if(kind==='page'){ o.type='triangle'; o.frequency.setValueAtTime(420,t); o.frequency.exponentialRampToValueAtTime(640,t+0.06); env(0.008,0.07,0.05); o.start(t);o.stop(t+0.1); }
  else if(kind==='dice'){ o.type='square'; o.frequency.setValueAtTime(180,t); o.frequency.linearRampToValueAtTime(120,t+0.12); env(0.005,0.12,0.04); o.start(t);o.stop(t+0.14); }
  else if(kind==='good'){ o.type='sine'; o.frequency.setValueAtTime(523,t); o.frequency.setValueAtTime(784,t+0.09); env(0.01,0.22,0.07); o.start(t);o.stop(t+0.26); }
  else if(kind==='bad'){ o.type='sawtooth'; o.frequency.setValueAtTime(196,t); o.frequency.exponentialRampToValueAtTime(98,t+0.2); env(0.01,0.22,0.06); o.start(t);o.stop(t+0.26); }
}

// ---- keyboard: number keys pick choices ----
window.addEventListener("keydown",e=>{
  if(!pendingOpts) return;
  const n=parseInt(e.key,10);
  if(n>=1 && n<=9){ const btns=pendingOpts.el.querySelectorAll(".opt:not(.locked)"); if(btns[n-1]){ btns[n-1].click(); } }
});

// ---- boot ----
renderBuilds();
renderCast("");
(function(){
  const want=decodeURIComponent(location.hash.replace(/^#/,""));
  if(want){ const ci=CONVS.findIndex(c=>c.id===want); if(ci>=0){ const ni=CAST.findIndex(c=>c.convs.includes(ci)); if(ni>=0){castSel=ni;renderCast("");} startConv(ci); return; } }
  // landing: a gentle prompt in the stage
  document.getElementById("stage").innerHTML=
    `<div class="pick" style="font-size:16px;margin-top:30px">Pick a character from <b>The Cast</b> on the left, then a conversation, and step in.</div>
     <div class="muted" style="margin-top:8px">${DATA.stats.conversations||CONVS.length} conversations · ${DATA.stats.nodes||''} beats · ${DATA.stats.skillChecks||''} skill checks — every one playable here on the real engine.</div>`;
})();
window.addEventListener("hashchange",()=>{ const want=decodeURIComponent(location.hash.replace(/^#/,"")); const ci=CONVS.findIndex(c=>c.id===want); if(ci>=0) startConv(ci); });
</script>
</body></html>"""

out = (HTML
       .replace("__BLOB__", BLOB)
       .replace("__BUILDS__", json.dumps(BUILDS, ensure_ascii=False))
       .replace("__COMPANIONS__", json.dumps(COMPANION_NAMES, ensure_ascii=False))
       .replace("search 187 speakers", f"search {len(cast_list)} speakers"))

dst = os.path.join(ROOT, "play", "dialogue_sim.html")
open(dst, "w", encoding="utf-8").write(out)
print(f"wrote play/dialogue_sim.html ({len(out)//1024} KB) — {len(DATA['conversations'])} conversations, "
      f"{len(cast_list)} speakers, {len(BUILDS)} builds")
