#!/usr/bin/env python3
"""
Generate play/save_inspector.html — a browser inspector/editor for the game's real save
format (SunderedCrown.Save.SaveSystem.SaveData, the flattened parallel-list JSON that
Unity's JsonUtility writes). Load a save (paste / file / sample), read it human-readably,
edit the flags / ints / quests / hero / gold, see what it unlocks (decoded against the flag
graph + codex), then export valid SaveData JSON that round-trips back into Unity — or hand
the flag state straight to the Endings Explorer.

Embeds play/flags-data.json (domains + what reads each flag, autocomplete) and the codex
unlock flags from play/compendium-data.json. Also writes play/sample-save.json. Re-run:
  python3 tools/extract-flags.py && python3 tools/make-save-inspector.py
"""
import json, os, datetime

ROOT = os.path.join(os.path.dirname(__file__), "..")
FLAGS = json.load(open(os.path.join(ROOT, "play", "flags-data.json")))
CONTENT = json.load(open(os.path.join(ROOT, "play", "compendium-data.json")))
CODEX = [{"title": e["title"], "unlockFlag": e["unlockFlag"]} for e in CONTENT.get("codex", [])]

# ---- a realistic sample save (the "golden-ish" run, so the inspector opens on real data) ----
SAMPLE_BOOLS = {
    "prologue.cleared": True,
    "companion.roen.recruited": True, "companion.varra.recruited": True,
    "companion.naeve.recruited": True, "companion.ilfaeril.recruited": True,
    "companion.maerin.recruited": True,
    "quest.roen.resolved": True, "quest.roen.wrenna_saved": True,
    "quest.varra.resolved": True, "quest.varra.patron_bound": True,
    "quest.garrow.resolved": True, "quest.garrow.doctrine_won": True,
    "quest.naeve.resolved": True, "quest.naeve.rekindled": True,
    "quest.ilfaeril.resolved": True, "quest.ilfaeril.commission": True,
    "romance.naeve.consummated": True,
    "netheril.cleared": True, "crownwars.cleared": True,
    "act4.toot_done": True, "act4.spellplague_done": True,
    "crownwars.verdict_spared": True, "readers_boon": True,
    "lowcity.allies": True, "almshouse.token": True,
}
SAMPLE_INTS = {
    "faction.kelemvor.reputation": 6, "faction.choir.reputation": 1,
    "companion.garrow.approval": 45, "companion.roen.approval": 30,
}
def sample_save():
    return {
        "version": "0.1.0", "sceneName": "TheCourtOfTheDead",
        "savedAtUtc": datetime.datetime(2026, 6, 12, 19, 30, 0).isoformat() + "Z",
        "boolKeys": list(SAMPLE_BOOLS), "boolValues": list(SAMPLE_BOOLS.values()),
        "intKeys": list(SAMPLE_INTS), "intValues": list(SAMPLE_INTS.values()),
        "questIds": ["roen", "varra", "garrow", "naeve", "ilfaeril"],
        "questStatuses": [2, 2, 2, 2, 2],
        "partyGold": 420,
        "heroName": "The Returned", "heroClass": "Fighter", "heroRace": "Human",
        "heroLevel": 9, "heroScores": [16, 14, 15, 10, 12, 13],
    }

SAMPLE = sample_save()
json.dump(SAMPLE, open(os.path.join(ROOT, "play", "sample-save.json"), "w"), indent=1)

EMBED = {
    "sample": SAMPLE,
    "flagInfo": {f["key"]: {"domain": f["domain"],
                            "readers": [r["source"] for r in f["readers"]][:8],
                            "writers": [w["source"] for w in f["writers"]][:8]}
                 for f in FLAGS["flags"]},
    "allFlagKeys": [f["key"] for f in FLAGS["flags"]],
    "codex": CODEX,
}
BLOB = json.dumps(EMBED, ensure_ascii=False).replace("</", "<\\/")

HTML = r"""<!DOCTYPE html>
<html lang="en"><head><meta charset="utf-8"><meta name="viewport" content="width=device-width, initial-scale=1">
<title>Crown of Horns — Save Inspector</title>
<style>
 *{box-sizing:border-box}
 body{margin:0;min-height:100vh;background:radial-gradient(1100px 640px at 20% -10%,#26212f,#0a0910);
  color:#d8d2c2;font:14px/1.55 "Iowan Old Style","Palatino Linotype",Georgia,serif;padding:0 0 60px}
 header{padding:14px 24px;border-bottom:1px solid #2a2636;display:flex;align-items:baseline;gap:16px;flex-wrap:wrap;
  position:sticky;top:0;background:#0b0a10ee;backdrop-filter:blur(6px);z-index:9}
 header h1{margin:0;color:#e7c873;font-size:20px}header .s{color:#8d8499;font-size:12px;font-style:italic}
 header a{color:#c9a24b;font-size:12px;text-decoration:none;margin-left:auto}header a:hover{text-decoration:underline}
 .wrap{max-width:1080px;margin:0 auto;padding:18px 24px}
 .toolbar{display:flex;gap:8px;flex-wrap:wrap;align-items:center;margin-bottom:16px}
 button{font:13px serif;background:#181620;color:#c9a24b;border:1px solid #2a2636;border-radius:7px;padding:7px 13px;cursor:pointer}
 button:hover{border-color:#e7c873;color:#e7c873}button.primary{background:linear-gradient(#2e2838,#211b2c);border-color:#e7c873;color:#e7c873}
 .file{position:relative;overflow:hidden;display:inline-block}.file input{position:absolute;inset:0;opacity:0;cursor:pointer}
 .cols{display:grid;grid-template-columns:1fr 1fr;gap:16px}
 @media(max-width:820px){.cols{grid-template-columns:1fr}}
 .card{background:linear-gradient(#16141d,#121017);border:1px solid #2a2636;border-radius:11px;padding:14px 16px;margin-bottom:16px}
 .card h2{margin:0 0 10px;color:#e7c873;font-size:15px;display:flex;justify-content:space-between;align-items:baseline}
 .card h2 .c{color:#8a8198;font-size:11px;font-weight:400}
 .hero{display:grid;grid-template-columns:auto 1fr;gap:6px 12px;align-items:center}
 .hero label{color:#8a8198;font-size:12px}.hero input{background:#0f0d15;border:1px solid #2a2636;color:#d8d2c2;border-radius:5px;padding:4px 7px;font:13px serif;width:100%}
 .scores{display:flex;gap:6px;flex-wrap:wrap;margin-top:8px}
 .score{background:#0f0d15;border:1px solid #2a2636;border-radius:6px;padding:4px 8px;text-align:center;min-width:48px}
 .score .a{color:#8a8198;font-size:10px}.score input{width:34px;background:none;border:0;color:#e7c873;font:600 15px serif;text-align:center}
 .row{display:flex;align-items:center;gap:8px;padding:4px 0;border-bottom:1px solid #1a1822}
 .row:last-child{border-bottom:0}
 .row .k{font-family:ui-monospace,monospace;font-size:11.5px;color:#cbc3d6;flex:1;word-break:break-all}
 .row .meta{font-size:10px;color:#6e6680;white-space:nowrap}
 .row .meta b{color:#8aa0bf;font-weight:400}
 .row.unknown .k{color:#d89a6a}
 .row input[type=number]{width:64px;background:#0f0d15;border:1px solid #2a2636;color:#e7c873;border-radius:5px;padding:3px 6px;font:13px ui-monospace}
 .row .del{color:#7b5;opacity:.5;cursor:pointer;border:0;background:none;color:#a86;font-size:14px;padding:0 4px}
 .row .del:hover{opacity:1}
 .switch{position:relative;width:34px;height:18px;flex:0 0 auto}
 .switch input{opacity:0;width:100%;height:100%;cursor:pointer;margin:0}
 .switch .tr{position:absolute;inset:0;background:#2a2636;border-radius:10px;transition:.12s;pointer-events:none}
 .switch .tr:after{content:"";position:absolute;width:14px;height:14px;border-radius:50%;background:#6e6680;top:2px;left:2px;transition:.12s}
 .switch input:checked+.tr{background:#3a5a38}.switch input:checked+.tr:after{background:#9fdf96;left:18px}
 .add{display:flex;gap:6px;margin-top:10px}.add input{flex:1;background:#0f0d15;border:1px solid #2a2636;color:#d8d2c2;border-radius:6px;padding:6px 9px;font:12px ui-monospace}
 .scroll{max-height:340px;overflow-y:auto;margin:-2px -4px;padding:0 4px}
 .unlocks{display:flex;gap:18px;flex-wrap:wrap}
 .ub{background:#0f0d15;border:1px solid #2a2636;border-radius:9px;padding:10px 16px}
 .ub .n{color:#e7c873;font-size:22px;font-weight:600}.ub .l{color:#8a8198;font-size:11px}
 .gatelist{margin-top:8px;display:flex;flex-wrap:wrap;gap:6px}
 .gpill{font-family:ui-monospace,monospace;font-size:10.5px;color:#9fcf96;background:#13241a;border:1px solid #2c4a32;border-radius:11px;padding:2px 9px}
 textarea{width:100%;min-height:130px;background:#0d0b13;border:1px solid #2a2636;color:#cbc3d6;border-radius:8px;
  padding:10px;font:11.5px/1.5 ui-monospace,monospace;resize:vertical}
 .hint{color:#6e6680;font-size:11.5px;margin:6px 0}
 .banner{background:#13241a;border:1px solid #2c4a32;color:#9fcf96;border-radius:7px;padding:8px 12px;margin-bottom:12px;font-size:13px;display:none}
 datalist{display:none}
</style></head><body>
<header>
 <h1>👑 Save Inspector</h1>
 <span class="s">read, edit &amp; round-trip the real SaveData format — and hand it to the Endings Explorer</span>
 <a href="index.html">← back to the hub</a>
</header>
<div class="wrap">
 <div class="banner" id="banner"></div>
 <div class="toolbar">
  <button class="primary" onclick="loadSample()">↺ Load sample save</button>
  <span class="file"><button>📂 Open .json…</button><input type="file" accept=".json,application/json" onchange="openFile(event)"></span>
  <button onclick="togglePaste()">📋 Paste JSON</button>
  <span style="flex:1"></span>
  <button onclick="toEndings()">🎭 Open in Endings Explorer →</button>
  <button onclick="copyOut()">⧉ Copy SaveData</button>
  <button onclick="download()">⤓ Download .json</button>
 </div>
 <div id="pasteWrap" style="display:none;margin-bottom:16px">
  <textarea id="paste" placeholder="paste a SaveData JSON here…"></textarea>
  <div class="hint">Then <button onclick="loadPaste()">load it ▸</button></div>
 </div>

 <div class="card">
  <h2>What this save unlocks <span class="c">decoded against the flag graph &amp; codex</span></h2>
  <div class="unlocks" id="unlocks"></div>
  <div class="gatelist" id="gates"></div>
 </div>

 <div class="cols">
  <div>
   <div class="card">
    <h2>Hero</h2>
    <div class="hero" id="hero"></div>
    <div class="scores" id="scores"></div>
   </div>
   <div class="card">
    <h2>Party <span class="c" id="goldc"></span></h2>
    <div class="row"><span class="k">gold</span><input type="number" id="gold" oninput="model.gold=+this.value;render()"></div>
   </div>
   <div class="card">
    <h2>Quests <span class="c" id="questc"></span></h2>
    <div id="quests"></div>
   </div>
   <div class="card">
    <h2>Integers <span class="c" id="intc"></span></h2>
    <div class="scroll" id="ints"></div>
    <div class="add"><input id="addIntKey" list="flagList" placeholder="flag key…"><input id="addIntVal" type="number" value="0" style="flex:0 0 80px"><button onclick="addInt()">+ int</button></div>
   </div>
  </div>
  <div>
   <div class="card">
    <h2>Flags (bool) <span class="c" id="boolc"></span></h2>
    <div class="scroll" id="bools"></div>
    <div class="add"><input id="addBoolKey" list="flagList" placeholder="add a flag key…"><button onclick="addBool()">+ flag = true</button></div>
   </div>
   <div class="card">
    <h2>Raw SaveData (export) <span class="c">JsonUtility shape</span></h2>
    <textarea id="out" readonly></textarea>
   </div>
  </div>
 </div>
 <datalist id="flagList"></datalist>
</div>
<script>
const EMBED = __BLOB__;
const QUEST_STATUS = ["Unstarted","Active","Completed","Failed"];
let model = null;

function esc(s){ return (s==null?"":String(s)).replace(/[&<>]/g,c=>({"&":"&amp;","<":"&lt;",">":"&gt;"}[c])); }

// ---- parse the parallel-list SaveData into a working model ----
function parse(data){
  const bools={}, ints={};
  (data.boolKeys||[]).forEach((k,i)=>bools[k]=!!(data.boolValues||[])[i]);
  (data.intKeys||[]).forEach((k,i)=>ints[k]=(data.intValues||[])[i]|0);
  const quests=(data.questIds||[]).map((id,i)=>({id, status:(data.questStatuses||[])[i]|0}));
  return { meta:{version:data.version||"0.1.0", sceneName:data.sceneName||"", savedAtUtc:data.savedAtUtc||""},
    bools, ints, quests, gold:data.partyGold|0,
    hero:{ name:data.heroName||"", cls:data.heroClass||"", race:data.heroRace||"",
      level:data.heroLevel||1, scores:(data.heroScores||[10,10,10,10,10,10]).slice() } };
}
// ---- model back to the exact SaveData DTO ----
function toSaveData(m){
  return {
    version:m.meta.version, sceneName:m.meta.sceneName, savedAtUtc:m.meta.savedAtUtc,
    boolKeys:Object.keys(m.bools), boolValues:Object.keys(m.bools).map(k=>m.bools[k]),
    intKeys:Object.keys(m.ints), intValues:Object.keys(m.ints).map(k=>m.ints[k]),
    questIds:m.quests.map(q=>q.id), questStatuses:m.quests.map(q=>q.status),
    partyGold:m.gold,
    heroName:m.hero.name, heroClass:m.hero.cls, heroRace:m.hero.race,
    heroLevel:m.hero.level, heroScores:m.hero.scores
  };
}

const ABIL=["STR","DEX","CON","INT","WIS","CHA"];
function render(){
  // hero
  const h=model.hero;
  document.getElementById("hero").innerHTML=
    [["name","name"],["cls","class"],["race","race"],["level","level"]].map(([k,l])=>
     `<label>${l}</label><input value="${esc(h[k])}" oninput="model.hero.${k}=this.${k==='level'?'valueAsNumber||1':'value'};syncOut()">`).join("");
  document.getElementById("scores").innerHTML=ABIL.map((a,i)=>
    `<div class="score"><div class="a">${a}</div><input type="number" value="${h.scores[i]|0}" oninput="model.hero.scores[${i}]=this.valueAsNumber|0;syncOut()"></div>`).join("");
  document.getElementById("gold").value=model.gold;
  document.getElementById("goldc").textContent=model.gold+" gp";
  // quests
  document.getElementById("questc").textContent=model.quests.length+" tracked";
  document.getElementById("quests").innerHTML=model.quests.map((q,i)=>
    `<div class="row"><span class="k">${esc(q.id)}</span>
     <select onchange="model.quests[${i}].status=+this.value;syncOut()" style="background:#0f0d15;border:1px solid #2a2636;color:#d8d2c2;border-radius:5px;padding:3px;font:12px serif">
     ${QUEST_STATUS.map((s,si)=>`<option value="${si}" ${si===q.status?"selected":""}>${s}</option>`).join("")}</select>
     <button class="del" onclick="model.quests.splice(${i},1);render()">✕</button></div>`).join("") || '<div class="hint">none</div>';
  // ints
  const ik=Object.keys(model.ints);
  document.getElementById("intc").textContent=ik.length;
  document.getElementById("ints").innerHTML=ik.map(k=>{
    const info=EMBED.flagInfo[k];
    return `<div class="row${info?"":" unknown"}"><span class="k">${esc(k)}</span>
     ${info?`<span class="meta">${esc(info.domain)}</span>`:'<span class="meta">unknown</span>'}
     <input type="number" value="${model.ints[k]}" oninput="model.ints['${esc(k)}']=this.valueAsNumber|0;syncOut()">
     <button class="del" onclick="delete model.ints['${esc(k)}'];render()">✕</button></div>`;}).join("") || '<div class="hint">none</div>';
  // bools
  const bk=Object.keys(model.bools);
  document.getElementById("boolc").textContent=bk.filter(k=>model.bools[k]).length+" set / "+bk.length;
  document.getElementById("bools").innerHTML=bk.map(k=>{
    const info=EMBED.flagInfo[k];
    const gates=info&&info.readers.length?`<span class="meta">gates: <b>${esc(info.readers.slice(0,2).join(", "))}</b>${info.readers.length>2?" +"+(info.readers.length-2):""}</span>`
      : info?`<span class="meta">${esc(info.domain)}</span>`:'<span class="meta">unknown</span>';
    return `<div class="row${info?"":" unknown"}">
     <label class="switch"><input type="checkbox" ${model.bools[k]?"checked":""} onchange="model.bools['${esc(k)}']=this.checked;render()"><span class="tr"></span></label>
     <span class="k">${esc(k)}</span>${gates}
     <button class="del" onclick="delete model.bools['${esc(k)}'];render()">✕</button></div>`;}).join("") || '<div class="hint">none</div>';
  renderUnlocks(); syncOut();
}
function renderUnlocks(){
  const codexOn=EMBED.codex.filter(e=>!e.unlockFlag || model.bools[e.unlockFlag]===true).length;
  const gates=Object.keys(model.bools).filter(k=>model.bools[k] && EMBED.flagInfo[k] &&
    EMBED.flagInfo[k].readers.some(r=>r==="Endings"));
  document.getElementById("unlocks").innerHTML=
    `<div class="ub"><div class="n">${codexOn}/${EMBED.codex.length}</div><div class="l">Codex entries unlocked</div></div>`+
    `<div class="ub"><div class="n">${gates.length}</div><div class="l">flags the Endings engine reads</div></div>`+
    `<div class="ub"><div class="n">${Object.keys(model.bools).filter(k=>model.bools[k]).length}</div><div class="l">flags set true</div></div>`;
  document.getElementById("gates").innerHTML=gates.sort().map(g=>`<span class="gpill">${esc(g)}</span>`).join("");
}
function syncOut(){ document.getElementById("out").value=JSON.stringify(toSaveData(model), null, 1); }

// ---- load paths ----
function loadModel(data){ try{ model=parse(data); render(); banner(""); }
  catch(e){ banner("Could not parse that save: "+e.message, true); } }
function loadSample(){ loadModel(JSON.parse(JSON.stringify(EMBED.sample))); banner("Loaded the bundled sample save (a near-golden run).",); }
function openFile(ev){ const f=ev.target.files[0]; if(!f) return; const r=new FileReader();
  r.onload=()=>{ try{ loadModel(JSON.parse(r.result)); banner("Loaded "+f.name); }catch(e){ banner("Not valid JSON: "+e.message,true); } }; r.readAsText(f); }
function togglePaste(){ const w=document.getElementById("pasteWrap"); w.style.display=w.style.display==="none"?"block":"none"; }
function loadPaste(){ try{ loadModel(JSON.parse(document.getElementById("paste").value)); togglePaste(); }
  catch(e){ banner("Not valid JSON: "+e.message,true); } }
function addBool(){ const i=document.getElementById("addBoolKey"); const k=i.value.trim(); if(!k) return;
  model.bools[k]=true; i.value=""; render(); }
function addInt(){ const i=document.getElementById("addIntKey"), v=document.getElementById("addIntVal");
  const k=i.value.trim(); if(!k) return; model.ints[k]=v.valueAsNumber|0; i.value=""; render(); }

// ---- export / bridge ----
function copyOut(){ navigator.clipboard.writeText(document.getElementById("out").value)
  .then(()=>banner("SaveData JSON copied to clipboard.")); }
function download(){ const blob=new Blob([document.getElementById("out").value],{type:"application/json"});
  const a=document.createElement("a"); a.href=URL.createObjectURL(blob);
  a.download=(model.hero.name||"save").replace(/\s+/g,"_")+".json"; a.click(); banner("Downloaded."); }
function toEndings(){
  try{ localStorage.setItem("coh.endings.import", JSON.stringify({bools:model.bools, ints:model.ints}));
    banner("Flag state staged — opening the Endings Explorer…");
    setTimeout(()=>location.href="endings_explorer.html", 350);
  }catch(e){ banner("Couldn't stage flags (storage blocked): "+e.message, true); } }
function banner(msg,bad){ const b=document.getElementById("banner");
  if(!msg){ b.style.display="none"; return; }
  b.style.display="block"; b.textContent=msg;
  b.style.color=bad?"#e0a0a0":"#9fcf96"; b.style.borderColor=bad?"#5a2c2c":"#2c4a32"; b.style.background=bad?"#241313":"#13241a"; }

// ---- boot ----
document.getElementById("flagList").innerHTML=EMBED.allFlagKeys.map(k=>`<option value="${esc(k)}">`).join("");
loadModel(JSON.parse(JSON.stringify(EMBED.sample)));
</script>
</body></html>
"""

out = HTML.replace("__BLOB__", BLOB)
dst = os.path.join(ROOT, "play", "save_inspector.html")
open(dst, "w").write(out)
print(f"wrote play/save_inspector.html ({len(out)//1024} KB) + play/sample-save.json "
      f"({len(SAMPLE['boolKeys'])} bools, {len(SAMPLE['intKeys'])} ints, {len(SAMPLE['questIds'])} quests)")
