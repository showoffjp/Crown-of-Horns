#!/usr/bin/env python3
"""
make-town-market.py — build play/town_market.html: a single, self-contained, fully playable
scene. You walk a character around a market square (click-to-move on an isometric board, just
like the combat sim) and approach THREE named NPCs — Mother Sable the charm-seller, Sergeant
Bram the recruiter, and Pip the street-child — each with a deep, BG3-style character-reactive
conversation. The NPCs read your race, class, background, alignment, deity, and stats and answer
differently; skill checks roll d20 + ability modifier + proficiency vs the DC, with an animated die.

Pure resolution logic lives in /*<MKT>*/...*/</MKT>*/ so town_market.test.js can lift and verify it.
Re-run:  python3 tools/make-town-market.py
"""
import json, os

ROOT = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
DEMO = json.load(open(os.path.join(ROOT, "play", "dialogue-demo.json"), encoding="utf-8"))
MKT = json.load(open(os.path.join(ROOT, "play", "town-market.json"), encoding="utf-8"))
MODEL = DEMO["characterModel"]

BUILDS = [
    {"name": "The Returned", "cls": "Fighter", "scores": [16, 14, 15, 10, 12, 13],
     "race": "Human", "background": "Soldier", "law": "Lawful", "morality": "Neutral", "deity": "Kelemvor",
     "blurb": "A plain human soldier of the Judge."},
    {"name": "The Scholar", "cls": "Wizard", "scores": [10, 14, 12, 16, 14, 11],
     "race": "Half-Elf", "background": "Sage", "law": "Neutral", "morality": "Good", "deity": "Oghma",
     "blurb": "Half-elf sage — sharp INSIGHT & lore."},
    {"name": "The Confessor", "cls": "Cleric", "scores": [13, 10, 14, 11, 17, 12],
     "race": "Human", "background": "Acolyte", "law": "Lawful", "morality": "Good", "deity": "Kelemvor",
     "blurb": "A cleric of Kelemvor — the best WISDOM."},
    {"name": "The Silver Tongue", "cls": "Rogue", "scores": [10, 16, 12, 13, 11, 16],
     "race": "Tiefling", "background": "Charlatan", "law": "Chaotic", "morality": "Neutral", "deity": "Tymora",
     "blurb": "A tiefling charlatan — the best CHARISMA."},
    {"name": "The Warden", "cls": "Ranger", "scores": [14, 16, 13, 11, 14, 10],
     "race": "Elf", "background": "Folk Hero", "law": "Neutral", "morality": "Neutral", "deity": "None",
     "blurb": "An elf who serves no god — one of the Faithless."},
]

INT_LABELS = {
    "market.sable.regard": "Mother Sable's regard",
    "market.bram.regard": "Sergeant Bram's regard",
    "market.pip.regard": "Pip's trust in you",
}

EMBED = {"scene": MKT["scene"], "conversations": MKT["conversations"], "model": MODEL}
BLOB = json.dumps(EMBED, ensure_ascii=False, separators=(",", ":"))

HTML = r"""<!DOCTYPE html>
<html lang="en"><head><meta charset="utf-8"><meta name="viewport" content="width=device-width, initial-scale=1">
<title>Crown of Horns — The Market of the Causeway</title>
<style>
 *{box-sizing:border-box}
 body{margin:0;min-height:100vh;background:radial-gradient(1200px 720px at 50% -10%,#241f30 0%,#0b0a10 70%);
   color:#d8d2c2;font:14px/1.5 "Iowan Old Style","Palatino Linotype",Georgia,serif}
 header{display:flex;align-items:baseline;gap:14px;padding:13px 22px;border-bottom:1px solid #2a2636;flex-wrap:wrap}
 header h1{margin:0;font-size:21px;letter-spacing:.5px;color:#e7c873;font-weight:600}
 header .sub{color:#8a8198;font-size:12px;font-style:italic}
 a.home{margin-left:auto;color:#9a90a8;text-decoration:none;font-size:12.5px;border:1px solid #2a2636;border-radius:7px;padding:6px 11px}
 a.home:hover{border-color:#c9a24b;color:#e7c873}
 .wrap{display:flex;gap:18px;padding:16px 22px;align-items:flex-start;flex-wrap:wrap;justify-content:center}
 .scenecol{display:flex;flex-direction:column;gap:10px}
 canvas{background:#0b0a10;border:1px solid #2a2636;border-radius:10px;box-shadow:0 12px 44px #0009, inset 0 0 70px #0007;cursor:pointer;display:block}
 .hint{color:#9a90a8;font-size:12.5px;font-style:italic;min-height:18px;max-width:760px}
 .hint b{color:#c9a24b;font-style:normal}
 .side{width:330px;display:flex;flex-direction:column;gap:13px}
 .card{background:linear-gradient(#16141d,#131119);border:1px solid #2a2636;border-radius:9px;padding:12px}
 .card h3{font-size:10.5px;letter-spacing:1.3px;text-transform:uppercase;color:#c9a24b;margin:0 0 9px;font-weight:600}
 .builds{display:flex;flex-wrap:wrap;gap:5px;margin-bottom:9px}
 .bld{font:inherit;font-size:11.5px;background:#1a1622;border:1px solid #2a2636;color:#c8c0b4;border-radius:6px;padding:5px 8px;cursor:pointer}
 .bld.on{border-color:#e7c873;color:#e7c873;background:#241d2e}
 .who-grid{display:grid;grid-template-columns:1fr 1fr;gap:6px;margin-bottom:8px}
 .who-grid label{font-size:9.5px;letter-spacing:.5px;color:#7f7790;display:block;margin-bottom:2px}
 .who-grid select{width:100%;background:#14121b;border:1px solid #2a2636;color:#d8d2c2;border-radius:6px;padding:4px 5px;font:inherit;font-size:12px}
 .abil{display:grid;grid-template-columns:repeat(3,1fr);gap:6px}
 .ab{background:#14121b;border:1px solid #232030;border-radius:7px;padding:5px 4px;text-align:center}
 .ab .k{font-size:9px;letter-spacing:.5px;color:#7f7790}.ab .v{font-size:15px;font-variant-numeric:tabular-nums}.ab .m{font-size:10.5px;color:#9fe0b0}
 .ab.dlg{border-color:#4a4368;box-shadow:0 0 0 1px #6a5a9a33}
 .ab .step{display:flex;gap:3px;justify-content:center;margin-top:2px}
 .ab .step button{width:17px;height:15px;line-height:12px;font-size:10px;background:#221d2b;border:1px solid #34304a;color:#c9a24b;border-radius:4px;cursor:pointer;padding:0}
 .prof{font-size:10.5px;color:#7f8aa0;margin-top:7px;line-height:1.4}.prof b{color:#f0c890}
 .toggle{display:flex;align-items:center;gap:7px;font-size:12px;color:#b8b0c4;margin:5px 0;cursor:pointer}.toggle input{accent-color:#c9a24b}
 .stateline{font-size:12px;color:#a89fb4}
 .empty{color:#6e6680;font-style:italic;font-size:12px}
 .appr{display:flex;align-items:center;gap:7px;margin:5px 0;font-size:12px}
 .appr .nm{width:140px;color:#c8c0b4}
 .appr .barwrap{flex:1;height:8px;background:#1c1a26;border-radius:4px;overflow:hidden;position:relative}
 .appr .bar{position:absolute;left:50%;top:0;bottom:0;background:linear-gradient(#4a8a5a,#6fd08a)}
 .appr .bar.neg{background:linear-gradient(#9b2d2d,#d06f6f)}
 .appr .delta{width:28px;text-align:right;font-variant-numeric:tabular-nums}.appr .delta.up{color:#9fe0b0}.appr .delta.down{color:#e0a0a0}
 .flagpill{display:inline-block;font-size:10px;background:#1a1622;border:1px solid #2a2636;border-radius:5px;padding:2px 6px;margin:2px 3px 0 0;color:#b8b0c4}
 .flagpill .d{color:#6e6680}
 .muted{color:#6e6680;font-size:11px}
 .sfxbtn{float:right;background:none;border:1px solid #2a2636;color:#8a8198;border-radius:6px;font-size:11px;padding:3px 7px;cursor:pointer}.sfxbtn:hover{color:#e7c873;border-color:#c9a24b}
 /* dialogue overlay */
 .overlay{position:fixed;inset:0;background:rgba(8,7,12,.72);display:none;align-items:flex-end;justify-content:center;z-index:50;padding:0 0 0}
 .overlay.show{display:flex}
 .dbox{width:min(760px,96vw);max-height:88vh;background:linear-gradient(#15131e,#100e16);border:1px solid #34304a;border-bottom:0;
   border-radius:14px 14px 0 0;box-shadow:0 -10px 50px #000a;display:flex;flex-direction:column;overflow:hidden}
 .dhead{display:flex;align-items:center;gap:12px;padding:13px 18px;border-bottom:1px solid #2a2636;background:#181421}
 .dhead .sig{width:42px;height:42px;border-radius:50%;display:flex;align-items:center;justify-content:center;font-size:15px;font-weight:700;color:#0c0b10}
 .dhead .nm{font-size:17px;color:#e7c873;font-weight:600}.dhead .ti{font-size:12px;color:#9a90a8;font-style:italic}
 .dhead .x{margin-left:auto;background:none;border:1px solid #2a2636;color:#9a90a8;border-radius:7px;padding:5px 10px;cursor:pointer;font:inherit;font-size:12px}
 .dhead .x:hover{border-color:#c9a24b;color:#e7c873}
 .dscript{padding:16px 18px 22px;overflow:auto;display:flex;flex-direction:column;gap:13px}
 .line{opacity:0;animation:fade .3s ease forwards}@keyframes fade{to{opacity:1}}
 .line .body{font-size:15.5px}
 .line.me{align-self:flex-end;max-width:88%}.line.me .body{color:#cfe0f0;background:#15192a;border:1px solid #243049;border-radius:11px 11px 2px 11px;padding:8px 12px;display:inline-block}
 .line.sys .body{font-size:13px;color:#b9a8e0;font-style:italic}
 .stagedir{color:#8a8198;font-style:italic}em{color:#e7c873;font-style:italic}
 .opts{display:flex;flex-direction:column;gap:9px;margin-top:4px}
 .opt{text-align:left;background:linear-gradient(#1b1726,#16121f);border:1px solid #34304a;color:#e8e2d2;border-radius:10px;padding:10px 13px;cursor:pointer;font:inherit;font-size:14.5px;transition:.12s}
 .opt:hover{border-color:#c9a24b;background:linear-gradient(#241d33,#1b1628)}
 .opt.locked{opacity:.45;cursor:not-allowed;border-style:dashed}
 .opt .num{color:#6e6680;font-size:11px;margin-right:6px}
 .opt .tags{display:flex;gap:6px;flex-wrap:wrap;margin-bottom:5px}
 .tg{display:inline-block;font-size:10px;letter-spacing:.5px;border-radius:5px;padding:1px 7px;font-weight:600;border:1px solid}
 .tg.race{color:#9fe0b0;border-color:#2c5a3a;background:#16241b}.tg.class{color:#9ec8e8;border-color:#27405a;background:#13202b}
 .tg.background{color:#7fd0c8;border-color:#245049;background:#13231f}.tg.faith{color:#e0b8f0;border-color:#5a3a6a;background:#221829}
 .tg.alignment{color:#c8c0b4;border-color:#3a3550;background:#1a1622}.tg.stat{color:#f0c890;border-color:#6a4f2a;background:#241c10}
 .tg.check{color:#c9a24b;border-color:#5a4a2a;background:#211c10}
 .ck{margin-top:6px;font-size:11.5px;color:#b9a8e0;display:flex;gap:8px;flex-wrap:wrap;align-items:center}
 .chip{display:inline-block;font-size:10.5px;border:1px solid #3a3550;border-radius:5px;padding:1px 6px;color:#c9a24b}
 .chip.ok{color:#9fe0b0;border-color:#2c4a32}.chip.bad{color:#e0a0a0;border-color:#5a2c2c}.chip.prof{color:#f0c890;border-color:#6a4f2a}.chip.fx{color:#9ec8e8;border-color:#27405a}
 .fxline{margin-top:5px;font-size:11px;color:#7f8aa0}
 .continue{background:linear-gradient(#26212f,#1a1622);color:#e7c873;border:1px solid #c9a24b66;border-radius:8px;padding:8px 15px;cursor:pointer;font:inherit;font-size:13px;align-self:flex-start}
 .continue:hover{border-color:#c9a24b}
 .ending{padding:11px 13px;border:1px dashed #3a3550;border-radius:9px;color:#9a90a8;font-style:italic}
 .leavebtn{background:#15131d;color:#9a90a8;border:1px solid #2a2636;border-radius:8px;padding:8px 14px;cursor:pointer;font:inherit;font-size:13px;margin-top:8px;align-self:flex-start}
 .leavebtn:hover{border-color:#c9a24b;color:#e7c873}
 .dice{display:flex;align-items:center;gap:12px;background:#14121c;border:1px solid #2a2636;border-radius:10px;padding:9px 13px}
 .d20{width:40px;height:40px;border-radius:8px;background:radial-gradient(circle at 40% 35%,#2e2740,#15121d);border:1px solid #4a4368;display:flex;align-items:center;justify-content:center;font-size:17px;font-weight:700;color:#e7c873;font-variant-numeric:tabular-nums}
 .d20.rolling{animation:shake .5s ease}@keyframes shake{0%,100%{transform:translateY(0) rotate(0)}25%{transform:translateY(-3px) rotate(-8deg)}50%{transform:translateY(2px) rotate(7deg)}75%{transform:translateY(-2px) rotate(-5deg)}}
 .dice .calc{font-size:12.5px;color:#b9a8e0}.dice .res{font-weight:700;margin-left:auto}.dice .res.ok{color:#9fe0b0}.dice .res.bad{color:#e0a0a0}
 @media(max-width:980px){.wrap{flex-direction:column;align-items:center}.side{width:min(760px,96vw)}}
</style></head><body>
<header>
 <h1>👑 The Market of the Causeway</h1>
 <span class="sub">walk the square · meet three souls · the way they answer depends on who you are</span>
 <a class="home" href="index.html">← all previews</a>
</header>
<div class="wrap">
 <div class="scenecol">
  <canvas id="board" width="760" height="470"></canvas>
  <div class="hint" id="hint"><b>Click</b> the ground to walk. Approach an NPC and <b>click them</b> (or press <b>E</b>) to talk. Change <b>who you are</b> on the right, then talk again — they notice.</div>
 </div>
 <div class="side">
  <div class="card">
   <h3>Your Character <button class="sfxbtn" id="sfxToggle" onclick="toggleSfx()">🔊 sfx</button></h3>
   <div class="builds" id="builds"></div>
   <div class="muted" id="buildBlurb" style="margin-bottom:8px"></div>
   <div class="who-grid" id="whoGrid"></div>
   <div class="abil" id="abil"></div>
   <div class="prof" id="profLine"></div>
  </div>
  <div class="card">
   <h3>Preview tools</h3>
   <label class="toggle"><input type="checkbox" id="tReveal"> Reveal what each choice does</label>
   <label class="toggle"><input type="checkbox" id="tForce"> Let me decide check results</label>
   <label class="toggle"><input type="checkbox" id="tLocked"> Show choices I don't qualify for</label>
  </div>
  <div class="card">
   <h3>How the market sees you</h3>
   <div id="approvals"><div class="empty">You haven't spoken to anyone yet.</div></div>
  </div>
  <div class="card">
   <h3>What your visit has written</h3>
   <div id="flags"><div class="empty">Nothing yet — your choices leave a mark here.</div></div>
  </div>
 </div>
</div>
<div class="overlay" id="overlay"><div class="dbox">
  <div class="dhead"><div class="sig" id="dsig"></div><div><div class="nm" id="dname"></div><div class="ti" id="dtitle"></div></div>
    <button class="x" onclick="closeDialogue()">✕ step back</button></div>
  <div class="dscript" id="dscript"></div>
</div></div>
<script>
const DATA = __BLOB__;
const BUILDS = __BUILDS__;
const INT_LABELS = __INTLABELS__;
const SCENE = DATA.scene, CONVS = DATA.conversations, MODEL = DATA.model;
const ABILS = ["Strength","Dexterity","Constitution","Intelligence","Wisdom","Charisma"];
const ABBR  = {Strength:"STR",Dexterity:"DEX",Constitution:"CON",Intelligence:"INT",Wisdom:"WIS",Charisma:"CHA"};

/*<MKT>*/
// Pure resolution — mirrors Assets/Scripts/Dialogue/DialogueRunner.cs + Abilities.cs.
function abilityMod(score){ return Math.floor((score - 10) / 2); }
function resolveCheck(roll, dc, mod){ return (roll + mod) >= dc; }
function chanceToPass(dc, mod){ return Math.max(0, Math.min(20, 21 - (dc - mod))) / 20; }
function newState(){ return { bools:{}, ints:{} }; }
function applyEffects(state, effects){ (effects||[]).forEach(e=>{
  if(e.op==="SetTrue") state.bools[e.key]=true; else if(e.op==="AddInt") state.ints[e.key]=(state.ints[e.key]||0)+(e.amount||0); }); return state; }
function isProficient(char, skill, model){ if(!skill) return false;
  const cp=(model.classProficiencies[char.cls]||[]), bp=(model.backgroundProficiencies[char.background]||[]);
  return cp.indexOf(skill)>=0 || bp.indexOf(skill)>=0; }
function checkBonus(char, check, model){ const i=["Strength","Dexterity","Constitution","Intelligence","Wisdom","Charisma"].indexOf(check.ability);
  let mod=abilityMod(char.scores[i]); if(check.skill && isProficient(char, check.skill, model)) mod+=(model.proficiencyBonus||0); return mod; }
function matchesWhen(char, state, when){ if(!when) return true;
  const i=(v,val)=>Array.isArray(v)?v.indexOf(val)>=0:v===val;
  if(when.race!==undefined && !i(when.race,char.race)) return false;
  if(when.class!==undefined && !i(when.class,char.cls)) return false;
  if(when.background!==undefined && !i(when.background,char.background)) return false;
  if(when.deity!==undefined && !i(when.deity,char.deity)) return false;
  if(when.law!==undefined && !i(when.law,char.law)) return false;
  if(when.morality!==undefined && !i(when.morality,char.morality)) return false;
  const idx=n=>["Strength","Dexterity","Constitution","Intelligence","Wisdom","Charisma"].indexOf(n);
  if(when.ability){ for(const k in when.ability){ if(char.scores[idx(k)]<when.ability[k]) return false; } }
  if(when.flag!==undefined && state.bools[when.flag]!==true) return false;
  if(when.flags){ for(const k of when.flags){ if(state.bools[k]!==true) return false; } }
  if(when.int){ for(const k in when.int){ if((state.ints[k]||0)<when.int[k]) return false; } }
  return true; }
function pickVariantText(node, char, state){ if(node.variants&&node.variants.length){
  for(const v of node.variants){ if(matchesWhen(char,state,v.when)) return v.text; } return ""; } return node.text||""; }
function choiceAvailable(char, state, ch){ return matchesWhen(char, state, ch.when); }
/*</MKT>*/

// ---- player character ----
let char = JSON.parse(JSON.stringify(BUILDS[0])), buildIdx = 0;
function modFor(a){ return abilityMod(char.scores[ABILS.indexOf(a)]); }
function renderBuilds(){
  document.getElementById("builds").innerHTML = BUILDS.map((b,i)=>`<button class="bld${i===buildIdx?' on':''}" onclick="setBuild(${i})">${esc(b.name)}</button>`).join("")
    + `<button class="bld${buildIdx===-1?' on':''}" style="cursor:default;opacity:${buildIdx===-1?1:.5}">Custom</button>`;
  document.getElementById("buildBlurb").textContent = char.blurb || "A character of your own making.";
  renderWho(); renderAbil(); renderProf();
}
function setBuild(i){ buildIdx=i; char=JSON.parse(JSON.stringify(BUILDS[i])); renderBuilds(); }
function field2key(f){ return {Race:"race",Class:"cls",Background:"background",Deity:"deity"}[f]; }
function setField(f,v){ char[f]=v; char.name="Custom"; char.blurb=""; buildIdx=-1; renderBuilds(); }
function setAlign(v){ const[l,m]=v.split(" "); char.law=l; char.morality=m; char.name="Custom"; char.blurb=""; buildIdx=-1; renderBuilds(); }
function bump(i,d){ char.scores[i]=Math.max(1,Math.min(20,char.scores[i]+d)); char.name="Custom"; char.blurb=""; buildIdx=-1; renderBuilds(); }
function renderWho(){
  const sel=(f,opts,val)=>`<div><label>${f.toUpperCase()}</label><select onchange="setField('${field2key(f)}',this.value)">`+opts.map(o=>`<option${o===val?' selected':''}>${esc(o)}</option>`).join("")+`</select></div>`;
  const align=`<div><label>ALIGNMENT</label><select onchange="setAlign(this.value)">`+MODEL.laws.flatMap(l=>MODEL.moralities.map(m=>{const v=l+" "+m;return `<option${(char.law+" "+char.morality)===v?' selected':''}>${esc(v)}</option>`;})).join("")+`</select></div>`;
  document.getElementById("whoGrid").innerHTML=sel("Race",MODEL.races,char.race)+sel("Class",MODEL.classes,char.cls)+sel("Background",MODEL.backgrounds,char.background)+sel("Deity",MODEL.deities,char.deity)+align;
}
function renderAbil(){ document.getElementById("abil").innerHTML=ABILS.map((a,i)=>{const m=abilityMod(char.scores[i]),dlg=(a==="Intelligence"||a==="Wisdom"||a==="Charisma");
  return `<div class="ab${dlg?' dlg':''}"><div class="k">${ABBR[a]}</div><div class="v">${char.scores[i]}</div><div class="m">${m>=0?'+':''}${m}</div><div class="step"><button onclick="bump(${i},-1)">▼</button><button onclick="bump(${i},1)">▲</button></div></div>`;}).join(""); }
function renderProf(){ const cp=(MODEL.classProficiencies[char.cls]||[]),bp=(MODEL.backgroundProficiencies[char.background]||[]),all=[...new Set([...cp,...bp])].sort();
  document.getElementById("profLine").innerHTML=`Proficient (+${MODEL.proficiencyBonus}) in: `+(all.length?all.map(s=>`<b>${esc(s)}</b>`).join(", "):"—")+`. INT · WIS · CHA drive the checks.`; }

// ---- persistent visit state ----
let st = newState();
function renderState(){
  const apps=[]; Object.keys(st.ints).forEach(k=>apps.push({name:INT_LABELS[k]||k, v:st.ints[k]}));
  const ap=document.getElementById("approvals");
  ap.innerHTML = !apps.length ? `<div class="empty">You haven't spoken to anyone yet.</div>` :
    apps.sort((a,b)=>Math.abs(b.v)-Math.abs(a.v)).map(a=>{const w=Math.min(50,Math.abs(a.v)*6),neg=a.v<0;
      return `<div class="appr"><div class="nm">${esc(a.name)}</div><div class="barwrap"><div class="bar${neg?' neg':''}" style="width:${w}%;${neg?'right:50%;left:auto;':''}"></div></div><div class="delta ${a.v>=0?'up':'down'}">${a.v>=0?'+':''}${a.v}</div></div>`;}).join("");
  const keys=Object.keys(st.bools), fl=document.getElementById("flags");
  fl.innerHTML = !keys.length ? `<div class="empty">Nothing yet — your choices leave a mark here.</div>` :
    keys.sort().map(k=>{const dot=k.indexOf("."),dom=dot<0?'':k.slice(0,dot)+'·';return `<span class="flagpill"><span class="d">${esc(dom)}</span>${esc(dot<0?k:k.slice(dot+1))}</span>`;}).join("");
}

// ====================== the market scene (canvas) ======================
const cv=document.getElementById("board"), ctx=cv.getContext("2d");
SCENE.props.forEach(p=>{ p.tx=p.x; p.ty=p.y; });   // scene authored in x/y; the engine draws in tx/ty
SCENE.npcs.forEach(n=>{ n.tx=n.x; n.ty=n.y; });
const TW=64, TH=32, OX=cv.width/2, OY=64;
function iso(tx,ty){ return { x: OX+(tx-ty)*TW/2, y: OY+(tx+ty)*TH/2 }; }
function unIso(sx,sy){ const a=(sx-OX)/(TW/2), b=(sy-OY)/(TH/2); return { tx:(a+b)/2, ty:(b-a)/2 }; }
const player={ tx:SCENE.playerStart.x, ty:SCENE.playerStart.y, target:null, facing:1 };
let hoverTile=null, hoverNpc=null, nearNpc=null, autoTalk=null, lastT=0;

function npcAt(id){ return SCENE.npcs.find(n=>n.id===id); }
function dist(ax,ay,bx,by){ return Math.hypot(ax-bx,ay-by); }

cv.addEventListener("mousemove",e=>{ const r=cv.getBoundingClientRect(), sx=e.clientX-r.left, sy=e.clientY-r.top;
  const t=unIso(sx,sy); hoverTile={tx:Math.round(t.tx),ty:Math.round(t.ty)};
  hoverNpc=null; for(const n of SCENE.npcs){ const p=iso(n.tx,n.ty); if(Math.hypot(sx-p.x,sy-(p.y-26))<26){ hoverNpc=n; break; } }
  cv.style.cursor = hoverNpc ? "pointer" : "default";
});
cv.addEventListener("click",e=>{ if(document.getElementById("overlay").classList.contains("show")) return;
  const r=cv.getBoundingClientRect(), sx=e.clientX-r.left, sy=e.clientY-r.top;
  if(hoverNpc){ const d=dist(player.tx,player.ty,hoverNpc.tx,hoverNpc.ty);
    if(d<1.8){ talk(hoverNpc); } else { walkToward(hoverNpc); autoTalk=hoverNpc; } sfx('step'); return; }
  const t=unIso(sx,sy); const tx=Math.max(0,Math.min(SCENE.w-1,Math.round(t.tx))), ty=Math.max(0,Math.min(SCENE.h-1,Math.round(t.ty)));
  player.target={tx,ty}; autoTalk=null; sfx('step');
});
function walkToward(n){ // walk to a tile one step toward the player from the NPC
  const dx=Math.sign(player.tx-n.tx), dy=Math.sign(player.ty-n.ty);
  player.target={tx:n.tx+(dx||0), ty:n.ty+(dy||0)};
}
window.addEventListener("keydown",e=>{
  if(document.getElementById("overlay").classList.contains("show")) return;
  if((e.key==="e"||e.key==="E") && nearNpc){ talk(nearNpc); }
});

function update(dt){
  if(player.target){ const dx=player.target.tx-player.tx, dy=player.target.ty-player.ty, d=Math.hypot(dx,dy);
    if(d<0.06){ player.tx=player.target.tx; player.ty=player.target.ty; player.target=null; }
    else { const sp=4.2*dt; player.tx+=dx/d*Math.min(sp,d); player.ty+=dy/d*Math.min(sp,d); if(Math.abs(dx)>0.01) player.facing=dx>=0?1:-1; } }
  // nearest NPC for the talk prompt + auto-talk on arrival
  nearNpc=null; let best=1.8;
  for(const n of SCENE.npcs){ const d=dist(player.tx,player.ty,n.tx,n.ty); if(d<best){ best=d; nearNpc=n; } }
  if(autoTalk && dist(player.tx,player.ty,autoTalk.tx,autoTalk.ty)<1.8 && !player.target){ const n=autoTalk; autoTalk=null; talk(n); }
}

// ---- drawing ----
function tokenColor(h,l){ return `hsl(${h} 46% ${l}%)`; }
function drawDiamond(cx,cy,fill,stroke){ ctx.beginPath(); ctx.moveTo(cx,cy-TH/2); ctx.lineTo(cx+TW/2,cy); ctx.lineTo(cx,cy+TH/2); ctx.lineTo(cx-TW/2,cy); ctx.closePath(); ctx.fillStyle=fill; ctx.fill(); if(stroke){ ctx.strokeStyle=stroke; ctx.lineWidth=1; ctx.stroke(); } }
function drawShadow(cx,by){ ctx.beginPath(); ctx.ellipse(cx,by,15,7,0,0,7); ctx.fillStyle="rgba(0,0,0,.34)"; ctx.fill(); }
function drawBox(cx,by,w,h,top,side,dark){ // simple iso box: by = base center y
  const hw=w/2; ctx.beginPath(); // left face
  ctx.moveTo(cx-hw,by); ctx.lineTo(cx-hw,by-h); ctx.lineTo(cx,by-h+TH/4); ctx.lineTo(cx,by+TH/4); ctx.closePath(); ctx.fillStyle=dark; ctx.fill();
  ctx.beginPath(); ctx.moveTo(cx+hw,by); ctx.lineTo(cx+hw,by-h); ctx.lineTo(cx,by-h+TH/4); ctx.lineTo(cx,by+TH/4); ctx.closePath(); ctx.fillStyle=side; ctx.fill();
  ctx.beginPath(); ctx.moveTo(cx,by-h-TH/4); ctx.lineTo(cx+hw,by-h); ctx.lineTo(cx,by-h+TH/4); ctx.lineTo(cx-hw,by-h); ctx.closePath(); ctx.fillStyle=top; ctx.fill();
}
function drawProp(p){ const s=iso(p.tx,p.ty);
  if(p.type==="fountain"){ drawDiamond(s.x,s.y,"#26222f","#1a1722"); ctx.beginPath(); ctx.ellipse(s.x,s.y,26,13,0,0,7); ctx.fillStyle="#1d2733"; ctx.fill(); ctx.beginPath(); ctx.ellipse(s.x,s.y-3,18,9,0,0,7); ctx.fillStyle="#2e4a63"; ctx.fill(); ctx.beginPath(); ctx.ellipse(s.x,s.y-4,9,4.5,0,0,7); ctx.fillStyle="#4a7a9a"; ctx.fill(); ctx.fillStyle="#6fa8c8"; ctx.fillRect(s.x-1.5,s.y-22,3,18); }
  else if(p.type==="stall"){ drawShadow(s.x,s.y+8); drawBox(s.x,s.y+6,40,20,"#3a3026","#2c241c","#211a14"); // awning
    ctx.beginPath(); ctx.moveTo(s.x-26,s.y-22); ctx.lineTo(s.x+26,s.y-22); ctx.lineTo(s.x+20,s.y-34); ctx.lineTo(s.x-20,s.y-34); ctx.closePath(); ctx.fillStyle=`hsl(${p.hue} 44% 46%)`; ctx.fill();
    ctx.fillStyle=`hsl(${p.hue} 44% 38%)`; for(let i=-2;i<=2;i++){ ctx.fillRect(s.x+i*10-2,s.y-34,4,12); } ctx.fillStyle="#1a1410"; ctx.fillRect(s.x-22,s.y-22,44,3); }
  else if(p.type==="crate"){ drawShadow(s.x,s.y+6); drawBox(s.x,s.y+5,26,16,"#6a4f2e","#553f24","#3e2d18"); }
  else if(p.type==="barrel"){ drawShadow(s.x,s.y+6); ctx.fillStyle="#4a3722"; ctx.beginPath(); ctx.ellipse(s.x,s.y-12,11,5,0,0,7); ctx.fill(); ctx.fillStyle="#5a4329"; ctx.fillRect(s.x-11,s.y-12,22,16); ctx.fillStyle="#3e2d18"; ctx.beginPath(); ctx.ellipse(s.x,s.y+4,11,5,0,0,7); ctx.fill(); ctx.fillStyle="#6a4f2e"; ctx.beginPath(); ctx.ellipse(s.x,s.y-12,11,5,0,0,7); ctx.fill(); }
  else if(p.type==="banner"){ ctx.strokeStyle="#3a2f22"; ctx.lineWidth=3; ctx.beginPath(); ctx.moveTo(s.x,s.y); ctx.lineTo(s.x,s.y-46); ctx.stroke(); ctx.fillStyle=`hsl(${p.hue} 44% 44%)`; ctx.beginPath(); ctx.moveTo(s.x,s.y-46); ctx.lineTo(s.x+22,s.y-42); ctx.lineTo(s.x+18,s.y-24); ctx.lineTo(s.x,s.y-28); ctx.closePath(); ctx.fill(); }
}
function drawToken(tx,ty,hue,label,opts){ opts=opts||{}; const s=iso(tx,ty); drawShadow(s.x,s.y+2);
  const glow=opts.glow; if(glow){ ctx.beginPath(); ctx.ellipse(s.x,s.y,20,10,0,0,7); ctx.fillStyle="rgba(231,200,115,.18)"; ctx.fill(); ctx.strokeStyle="rgba(231,200,115,.55)"; ctx.lineWidth=2; ctx.stroke(); }
  // cloak/body
  ctx.fillStyle=tokenColor(hue,opts.player?54:44); ctx.beginPath(); ctx.moveTo(s.x-9,s.y-2); ctx.quadraticCurveTo(s.x,s.y-30,s.x+9,s.y-2); ctx.closePath(); ctx.fill();
  ctx.fillStyle=tokenColor(hue,opts.player?44:36); ctx.fillRect(s.x-9,s.y-8,18,7);
  // head
  ctx.fillStyle="#d8b48c"; ctx.beginPath(); ctx.arc(s.x,s.y-30,6.5,0,7); ctx.fill();
  ctx.fillStyle=tokenColor(hue,30); ctx.beginPath(); ctx.arc(s.x,s.y-33,7,Math.PI,0); ctx.fill(); // hood/hair
  if(label){ ctx.font="600 12px Iowan Old Style, Georgia, serif"; const w=ctx.measureText(label).width;
    ctx.fillStyle="rgba(12,11,16,.78)"; ctx.fillRect(s.x-w/2-6,s.y-58,w+12,18); ctx.fillStyle=opts.player?"#e7c873":"#e8e2d2"; ctx.textAlign="center"; ctx.fillText(label,s.x,s.y-45); }
  if(opts.prompt){ ctx.font="600 12px Iowan Old Style, Georgia, serif"; ctx.fillStyle="#e7c873"; ctx.textAlign="center"; ctx.fillText("▶ talk (E)",s.x,s.y-63); }
}
function render(){
  ctx.clearRect(0,0,cv.width,cv.height);
  // floor
  for(let ty=0;ty<SCENE.h;ty++) for(let tx=0;tx<SCENE.w;tx++){ const s=iso(tx,ty);
    const shade=((tx+ty)%2)?"#181620":"#1c1a26";
    drawDiamond(s.x,s.y, hoverTile&&hoverTile.tx===tx&&hoverTile.ty===ty?"#2a2740":shade, "#13111a"); }
  if(player.target){ const s=iso(player.target.tx,player.target.ty); drawDiamond(s.x,s.y,"rgba(231,200,115,.10)","rgba(231,200,115,.5)"); }
  // depth-sorted entities (props + npcs + player)
  const ents=[];
  SCENE.props.forEach(p=>ents.push({d:p.tx+p.ty + (p.type==="banner"?-0.5:0), draw:()=>drawProp(p)}));
  SCENE.npcs.forEach(n=>ents.push({d:n.tx+n.ty, draw:()=>drawToken(n.tx,n.ty,n.hue,n.name,{glow:nearNpc===n||hoverNpc===n, prompt:nearNpc===n})}));
  ents.push({d:player.tx+player.ty, draw:()=>drawToken(player.tx,player.ty,46,null,{player:true})});
  ents.sort((a,b)=>a.d-b.d).forEach(e=>e.draw());
}
function loop(t){ const dt=Math.min(0.05,(t-lastT)/1000||0); lastT=t; update(dt); render(); requestAnimationFrame(loop); }

// ====================== the dialogue overlay ======================
let curConv=null, curNpc=null, pendingOpts=null;
function talk(npc){
  curNpc=npc; curConv=CONVS.find(c=>c.id===npc.conv); if(!curConv) return;
  player.target=null; autoTalk=null;
  document.getElementById("dsig").textContent=npc.sigil; document.getElementById("dsig").style.background=`hsl(${npc.hue} 52% 62%)`;
  document.getElementById("dname").textContent=npc.name; document.getElementById("dtitle").textContent=npc.title;
  document.getElementById("dscript").innerHTML=""; document.getElementById("overlay").classList.add("show");
  sfx('talk');
  const start=curConv.nodes.find(n=>n.id===curConv.start)?curConv.start:curConv.nodes[0].id;
  goNode(start);
}
function closeDialogue(){ document.getElementById("overlay").classList.remove("show"); curConv=null; pendingOpts=null; renderState(); }
function nodeById(id){ return curConv.nodes.find(n=>n.id===id); }
function isEnd(id){ return !id||id==="END"||id==="end"||!nodeById(id); }
function goNode(id){
  pendingOpts=null; const n=nodeById(id), script=document.getElementById("dscript");
  if(!n){ endScene(); return; }
  applyEffects(st, n.onEnter); renderState();
  addLine("", curNpc.name, pickVariantText(n,char,st)||"〔(no line for this character)〕");
  const opts=document.createElement("div"); opts.className="opts"; script.appendChild(opts);
  const all=n.choices||[];
  if(all.length){ pendingOpts={node:n,el:opts}; paintChoices(); }
  else if(n.auto && !isEnd(n.auto)){ const b=document.createElement("button"); b.className="continue"; b.textContent="Continue ▸"; b.onclick=()=>{ sfx('page'); opts.remove(); goNode(n.auto); }; opts.appendChild(b); }
  else endScene();
  script.scrollTop=script.scrollHeight;
}
function normCheck(ch){ if(ch.check) return ch.check; if(ch.checkDC) return {skill:null,ability:ch.checkAbility,dc:ch.checkDC}; return null; }
function tagFor(ch){ const w=ch.when; if(!w) return null;
  if(w.race!==undefined) return {cls:"race",label:[].concat(w.race).join("/")};
  if(w.class!==undefined) return {cls:"class",label:[].concat(w.class).join("/")};
  if(w.background!==undefined) return {cls:"background",label:[].concat(w.background).join("/")};
  if(w.deity!==undefined) return {cls:"faith",label:[].concat(w.deity).map(d=>d==="None"?"Faithless":d).join("/")};
  if(w.law!==undefined) return {cls:"alignment",label:[].concat(w.law).join("/")};
  if(w.morality!==undefined) return {cls:"alignment",label:[].concat(w.morality).join("/")};
  if(w.ability){ const k=Object.keys(w.ability)[0]; return {cls:"stat",label:`${ABBR[k]} ${w.ability[k]}`}; } return null; }
function paintChoices(){ if(!pendingOpts) return; const {node:n,el:opts}=pendingOpts; opts.innerHTML="";
  const reveal=document.getElementById("tReveal").checked, force=document.getElementById("tForce").checked, showLocked=document.getElementById("tLocked").checked;
  let num=0;
  n.choices.forEach(ch=>{ const ok=choiceAvailable(char,st,ch); if(!ok&&!showLocked) return; const idx=++num;
    const b=document.createElement("button"); b.className="opt"+(ok?"":" locked"); const chk=normCheck(ch), tag=tagFor(ch);
    let tags=""; if(tag) tags+=`<span class="tg ${tag.cls}">${esc(tag.label)}</span>`; if(chk) tags+=`<span class="tg check">🎲 ${esc(chk.skill||ABBR[chk.ability]||chk.ability)}</span>`;
    let inner=(tags?`<div class="tags">${tags}</div>`:"")+`<span class="num">${idx}.</span>${esc(stripBracket(ch.text)||"(continue)")}`;
    if(chk){ const bonus=checkBonus(char,chk,MODEL), pct=Math.round(chanceToPass(chk.dc,bonus)*100), prof=chk.skill&&isProficient(char,chk.skill,MODEL);
      inner+=`<div class="ck"><span class="chip">${chk.skill?esc(chk.skill)+' · ':''}${ABBR[chk.ability]||esc(chk.ability)} DC ${chk.dc}</span><span class="chip ${pct>=50?'ok':'bad'}">${pct}% (d20 ${bonus>=0?'+':''}${bonus})</span>${prof?`<span class="chip prof">proficient +${MODEL.proficiencyBonus}</span>`:''}${ch.fail?`<span class="muted">miss ▸ another path</span>`:''}</div>`; }
    if(!ok){ const t=tagFor(ch); inner+=`<div class="fxline">🔒 needs ${t?(t.cls==="faith"?"faith":t.cls)+": "+t.label:"a different character"}</div>`; }
    if(reveal){ const fx=describeEffects(ch.effects); if(fx) inner+=`<div class="fxline">▸ ${fx}</div>`; }
    b.innerHTML=inner; if(ok) b.onclick=()=>choose(n,ch,force); opts.appendChild(b);
  });
  if(!num){ const d=document.createElement("div"); d.className="muted"; d.textContent="(no choice here fits this character — toggle “show choices I don't qualify for”, or change who you are)"; opts.appendChild(d); }
}
function choose(n,ch,force){ sfx('page'); pendingOpts.el.remove(); pendingOpts=null;
  addLine("me","You", stripBracket(ch.text)||"(continue)");
  const chk=normCheck(ch); let next=ch.next;
  const proceed=ok=>{ if(chk&&!ok&&ch.fail) next=ch.fail; applyEffects(st,ch.effects); renderState(); goNode(next); };
  if(chk){ const bonus=checkBonus(char,chk,MODEL); if(force) return offerForced(chk,proceed); rollDice(chk,bonus,proceed); }
  else { applyEffects(st,ch.effects); renderState(); goNode(next); }
}
function offerForced(chk,proceed){ const box=document.createElement("div"); box.className="opts";
  box.innerHTML=`<div class="muted" style="margin-bottom:4px">Preview either branch of this ${chk.skill||ABBR[chk.ability]} DC ${chk.dc} check:</div>`;
  const mk=(l,c,ok)=>{const b=document.createElement("button");b.className="opt";b.innerHTML=`<span class="chip ${c}">${l}</span>`;b.onclick=()=>{box.remove();addLine("sys","",`(forced ${ok?'SUCCESS':'FAILURE'})`);proceed(ok);};return b;};
  box.appendChild(mk("✓ success branch","ok",true)); box.appendChild(mk("✗ failure branch","bad",false));
  document.getElementById("dscript").appendChild(box); document.getElementById("dscript").scrollTop=1e9; }
function rollDice(chk,bonus,proceed){ const wrap=document.createElement("div"); wrap.className="dice";
  wrap.innerHTML=`<div class="d20 rolling" id="d20m">?</div><div class="calc">rolling ${chk.skill||ABBR[chk.ability]}…</div>`;
  const sc=document.getElementById("dscript"); sc.appendChild(wrap); sc.scrollTop=1e9; sfx('dice');
  let ticks=0; const die=wrap.querySelector("#d20m"), calc=wrap.querySelector(".calc");
  const spin=setInterval(()=>{ die.textContent=1+Math.floor(Math.random()*20); if(++ticks>=12){ clearInterval(spin); land(); } },42);
  function land(){ const roll=1+Math.floor(Math.random()*20), total=roll+bonus, ok=resolveCheck(roll,chk.dc,bonus);
    die.classList.remove("rolling"); die.textContent=roll; calc.innerHTML=`${roll} ${bonus>=0?'+':'−'} ${Math.abs(bonus)} <b>= ${total}</b> vs DC ${chk.dc}`;
    const r=document.createElement("div"); r.className="res "+(ok?"ok":"bad"); r.textContent=ok?"SUCCESS":"FAIL"; wrap.appendChild(r); sfx(ok?'good':'bad'); setTimeout(()=>proceed(ok),560); } }
function endScene(){ const sc=document.getElementById("dscript"); const e=document.createElement("div"); e.className="ending"; e.textContent="▪ The conversation comes to rest."; sc.appendChild(e);
  const b=document.createElement("button"); b.className="leavebtn"; b.textContent="↩ back to the market"; b.onclick=closeDialogue; sc.appendChild(b); sc.scrollTop=1e9; }

function addLine(cls,who,body){ const w=document.getElementById("dscript"); const d=document.createElement("div"); d.className="line "+cls;
  let sig=""; if(who&&cls==="" ){ sig=`<div style="font-size:11px;letter-spacing:1px;text-transform:uppercase;color:#c9a24b;margin-bottom:3px">${esc(who)}</div>`; }
  if(cls==="me") sig=`<div style="font-size:11px;letter-spacing:1px;text-transform:uppercase;color:#9fc0e0;margin-bottom:3px;text-align:right">You</div>`;
  d.innerHTML=sig+`<div class="body">${cls==="sys"?esc(body):prose(body)}</div>`; w.appendChild(d); }
function esc(s){ return (s==null?"":String(s)).replace(/[&<>]/g,c=>({"&":"&amp;","<":"&lt;",">":"&gt;"}[c])); }
function prose(s){ let h=esc(s); h=h.replace(/\*\(([\s\S]*?)\)\*/g,'<span class="stagedir">($1)</span>'); h=h.replace(/\*([^*]+)\*/g,'<em>$1</em>'); return h; }
function stripBracket(t){ return (t||"").replace(/^\s*\[[^\]]*\]\s*/,""); }
function describeEffects(effects){ return (effects||[]).map(e=>{ if(e.op==="SetTrue") return `sets <span class="chip fx">${esc(e.key)}</span>`;
  if(e.op==="AddInt") return `${esc(INT_LABELS[e.key]||e.key)} ${e.amount>=0?'+':''}${e.amount}`; return esc(e.key); }).join(" · "); }

// ---- SFX ----
let sfxOn=true, actx=null;
function toggleSfx(){ sfxOn=!sfxOn; document.getElementById("sfxToggle").textContent=(sfxOn?'🔊':'🔇')+' sfx'; }
function sfx(kind){ if(!sfxOn) return; try{ actx=actx||new (window.AudioContext||window.webkitAudioContext)(); }catch(e){ return; }
  const t=actx.currentTime,o=actx.createOscillator(),g=actx.createGain(); o.connect(g); g.connect(actx.destination);
  const env=(a,d,v)=>{g.gain.setValueAtTime(0.0001,t);g.gain.exponentialRampToValueAtTime(v,t+a);g.gain.exponentialRampToValueAtTime(0.0001,t+a+d);};
  if(kind==='step'){ o.type='sine'; o.frequency.setValueAtTime(150,t); env(0.004,0.04,0.025); o.start(t);o.stop(t+0.06); }
  else if(kind==='talk'){ o.type='triangle'; o.frequency.setValueAtTime(360,t); o.frequency.exponentialRampToValueAtTime(540,t+0.07); env(0.008,0.08,0.05); o.start(t);o.stop(t+0.11); }
  else if(kind==='page'){ o.type='triangle'; o.frequency.setValueAtTime(420,t); o.frequency.exponentialRampToValueAtTime(640,t+0.06); env(0.008,0.07,0.045); o.start(t);o.stop(t+0.1); }
  else if(kind==='dice'){ o.type='square'; o.frequency.setValueAtTime(180,t); o.frequency.linearRampToValueAtTime(120,t+0.12); env(0.005,0.12,0.04); o.start(t);o.stop(t+0.14); }
  else if(kind==='good'){ o.type='sine'; o.frequency.setValueAtTime(523,t); o.frequency.setValueAtTime(784,t+0.09); env(0.01,0.22,0.06); o.start(t);o.stop(t+0.26); }
  else if(kind==='bad'){ o.type='sawtooth'; o.frequency.setValueAtTime(196,t); o.frequency.exponentialRampToValueAtTime(98,t+0.2); env(0.01,0.22,0.05); o.start(t);o.stop(t+0.26); }
}

// ---- boot ----
renderBuilds(); renderState(); requestAnimationFrame(loop);
</script>
</body></html>"""

out = (HTML.replace("__BLOB__", BLOB)
       .replace("__BUILDS__", json.dumps(BUILDS, ensure_ascii=False))
       .replace("__INTLABELS__", json.dumps(INT_LABELS, ensure_ascii=False)))
dst = os.path.join(ROOT, "play", "town_market.html")
open(dst, "w", encoding="utf-8").write(out)
print(f"wrote play/town_market.html ({len(out)//1024} KB) — {len(MKT['scene']['npcs'])} NPCs, "
      f"{sum(len(c['nodes']) for c in MKT['conversations'])} dialogue beats, walkable {MKT['scene']['w']}×{MKT['scene']['h']} square")
