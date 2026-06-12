#!/usr/bin/env python3
"""
Generate play/flags_explorer.html — a browsable dependency graph of the game's GameFlags,
from play/flags-data.json (tools/extract-flags.py). Pick any flag to see the bipartite map
of what WRITES it (dialogue choices, quest resolutions, deeds, era content) and what READS
it (ending gates, codex unlocks, dialogue conditions). An overview panel surfaces the
domains, the most-connected hub flags, and the orphans (write-only dead-ends and read-only
engine state). Re-run:
  python3 tools/extract-flags.py && python3 tools/make-flags-explorer.py
"""
import json, os

ROOT = os.path.join(os.path.dirname(__file__), "..")
DATA = json.load(open(os.path.join(ROOT, "play", "flags-data.json")))
S = DATA["stats"]
BLOB = json.dumps(DATA, ensure_ascii=False).replace("</", "<\\/")

HTML = r"""<!DOCTYPE html>
<html lang="en"><head><meta charset="utf-8"><meta name="viewport" content="width=device-width, initial-scale=1">
<title>Crown of Horns — Flag Dependency Graph</title>
<style>
 *{box-sizing:border-box}
 body{margin:0;height:100vh;overflow:hidden;display:flex;flex-direction:column;
  background:radial-gradient(1100px 640px at 80% -10%,#26212f,#0a0910);
  color:#d8d2c2;font:14px/1.55 "Iowan Old Style","Palatino Linotype",Georgia,serif}
 header{padding:12px 20px;border-bottom:1px solid #2a2636;display:flex;align-items:baseline;gap:16px;flex-wrap:wrap}
 header h1{margin:0;color:#e7c873;font-size:20px;letter-spacing:.4px}
 header .s{color:#8d8499;font-size:12px;font-style:italic}
 header a{color:#c9a24b;font-size:12px;text-decoration:none;margin-left:auto}
 header a:hover{text-decoration:underline}
 .wrap{flex:1;display:flex;min-height:0}
 aside{width:300px;border-right:1px solid #2a2636;overflow-y:auto;background:#0d0b13}
 .find{margin:10px;width:calc(100% - 20px);background:#16131e;border:1px solid #2a2636;border-radius:7px;
  color:#d8d2c2;padding:7px 9px;font:13px serif}
 .domfilter{display:flex;flex-wrap:wrap;gap:4px;padding:0 10px 8px}
 .dchip{font-size:10.5px;color:#9a90a8;border:1px solid #2a2636;border-radius:11px;padding:2px 8px;cursor:pointer}
 .dchip.on{border-color:#e7c873;color:#e7c873;background:#1c1828}
 .grp{color:#7b7388;font-size:10px;text-transform:uppercase;letter-spacing:1px;margin:10px 12px 3px}
 .flag{padding:5px 12px;cursor:pointer;font-family:ui-monospace,monospace;font-size:11.5px;color:#bcb3c8;
  border-left:2px solid transparent;display:flex;justify-content:space-between;gap:8px;align-items:center}
 .flag:hover{background:#16131e;color:#e7c873}
 .flag.on{background:#1c1828;color:#e7c873;border-left-color:#e7c873}
 .flag .wr{font-size:9.5px;color:#6e6680;white-space:nowrap}
 .flag .od{color:#b58a5a}.flag .or{color:#79a0c4}
 main{flex:1;display:flex;flex-direction:column;min-width:0}
 .bar{padding:10px 18px;border-bottom:1px solid #2a2636;display:flex;align-items:center;gap:12px;flex-wrap:wrap}
 .bar h2{margin:0;color:#e7c873;font-size:16px;font-family:ui-monospace,monospace}
 .bar .dom{color:#9a90a8;font-size:12px}
 .chips{display:flex;gap:6px;margin-left:auto}
 .chip{font-size:10.5px;color:#9a90a8;border:1px solid #2a2636;border-radius:5px;padding:2px 7px}
 .view{flex:1;overflow:auto;position:relative}
 svg{display:block;margin:0 auto}
 .col{fill:#9a90a8;font:600 11px serif;text-transform:uppercase;letter-spacing:1px}
 .src rect{rx:7}
 .src text{font:11.5px serif}
 .flagnode rect{fill:#211b2c;stroke:#e7c873;stroke-width:2;rx:9}
 .flagnode text{fill:#e7c873;font:600 13px ui-monospace,monospace}
 .edge{fill:none;stroke-width:1.6;opacity:.75}
 .via-dialogue{stroke:#79a0c4}.via-code{stroke:#8f8a9e}.via-codex{stroke:#7fbf76}
 rect.dialogue{fill:#16202c;stroke:#33506e}text.dialogue{fill:#a9c4dc}
 rect.code{fill:#1b1826;stroke:#3a3550}text.code{fill:#c3bace}
 rect.codex{fill:#13241a;stroke:#2c4a32}text.codex{fill:#9fcf96}
 .legend{position:absolute;top:10px;right:14px;font-size:11px;color:#8d8499;background:#0e0c14cc;border:1px solid #2a2636;
  border-radius:8px;padding:8px 11px}
 .legend b{display:inline-block;width:9px;height:9px;border-radius:2px;margin-right:5px}
 /* overview */
 .ov{padding:20px 26px;max-width:1000px;margin:0 auto}
 .ov h3{color:#e7c873;font-size:15px;margin:20px 0 8px;border-bottom:1px solid #221d2c;padding-bottom:5px}
 .statline{display:flex;gap:22px;flex-wrap:wrap;margin-bottom:8px}
 .stat{background:linear-gradient(#16141d,#131119);border:1px solid #2a2636;border-radius:9px;padding:11px 16px;min-width:120px}
 .stat .n{color:#e7c873;font-size:24px;font-weight:600}.stat .l{color:#8a8198;font-size:11px}
 .doms{display:grid;grid-template-columns:repeat(auto-fill,minmax(200px,1fr));gap:7px}
 .domrow{display:flex;align-items:center;gap:8px;cursor:pointer;background:#13111a;border:1px solid #221f2a;border-radius:7px;padding:6px 10px}
 .domrow:hover{border-color:#e7c873}
 .domrow .dn{font-size:12px;color:#cfc7d8;flex:1}.domrow .dc{color:#8a8198;font-size:11px;font-family:ui-monospace,monospace}
 .barfill{height:5px;background:#e7c873;border-radius:3px;margin-top:3px}
 .hublist,.orphlist{display:flex;flex-wrap:wrap;gap:6px}
 .pill{font-family:ui-monospace,monospace;font-size:11px;color:#cbb;background:#15121d;border:1px solid #2a2636;
  border-radius:13px;padding:3px 11px;cursor:pointer}
 .pill:hover{border-color:#e7c873;color:#e7c873}.pill .c{color:#7b7388}
 .note{color:#8a8198;font-size:12px;margin:2px 0 8px}
 .home{color:#c9a24b;cursor:pointer;font-size:12px}
</style></head><body>
<header>
 <h1>👑 Flag Dependency Graph</h1>
 <span class="s">__STATS__</span>
 <a href="index.html">← back to the hub</a>
</header>
<div class="wrap">
 <aside>
  <input class="find" placeholder="search flags…" oninput="filter(this.value)">
  <div class="domfilter" id="domfilter"></div>
  <div id="list"></div>
 </aside>
 <main>
  <div class="bar">
   <span class="home" onclick="showOverview()">⌂ Overview</span>
   <h2 id="title"></h2><span class="dom" id="dom"></span>
   <div class="chips" id="chips"></div>
  </div>
  <div class="view" id="view"></div>
 </main>
</div>
<script>
const DATA = __BLOB__;
const FLAGS = DATA.flags;
const byKey = {}; FLAGS.forEach(f => byKey[f.key] = f);
let cur = null, domFilter = "all";

function esc(s){ return (s==null?"":String(s)).replace(/[&<>]/g,c=>({"&":"&amp;","<":"&lt;",">":"&gt;"}[c])); }
function trunc(s,n){ s=s||""; return s.length>n? s.slice(0,n-1)+"…":s; }

// ---- sidebar ----
function buildDomFilter(){
  const el = document.getElementById("domfilter");
  el.innerHTML = `<span class="dchip${domFilter==="all"?" on":""}" data-d="all">all</span>` +
    DATA.domains.map(d=>`<span class="dchip${domFilter===d?" on":""}" data-d="${esc(d)}">${esc(d)}</span>`).join("");
  el.querySelectorAll(".dchip").forEach(c=>c.onclick=()=>{ domFilter=c.dataset.d; buildDomFilter(); buildList(document.querySelector(".find").value); });
}
function buildList(q){
  q=(q||"").toLowerCase();
  const groups={};
  FLAGS.forEach(f=>{
    if(domFilter!=="all" && f.domain!==domFilter) return;
    if(q && !f.key.toLowerCase().includes(q)) return;
    (groups[f.domain]=groups[f.domain]||[]).push(f);
  });
  const el=document.getElementById("list"); el.innerHTML="";
  Object.keys(groups).sort().forEach(g=>{
    const h=document.createElement("div"); h.className="grp"; h.textContent=g; el.appendChild(h);
    groups[g].forEach(f=>{
      const d=document.createElement("div"); d.className="flag"+(cur===f.key?" on":""); d.dataset.k=f.key;
      const badge = f.orphanWrite? '<span class="od" title="written but never read">write-only</span>'
                  : f.orphanRead?  '<span class="or" title="read but set in code/engine">read-only</span>'
                  : `${f.writers.length}W · ${f.readers.length}R`;
      d.innerHTML=`<span>${esc(f.key)}</span><span class="wr">${badge}</span>`;
      d.onclick=()=>showFlag(f.key); el.appendChild(d);
    });
  });
}
function filter(v){ buildList(v); }

// ---- flag detail: bipartite writers -> flag -> readers ----
function showFlag(key){
  cur=key; const f=byKey[key];
  document.querySelectorAll(".flag").forEach(e=>e.classList.toggle("on",e.dataset.k===key));
  document.getElementById("title").textContent=key;
  document.getElementById("dom").textContent="· "+f.domain;
  document.getElementById("chips").innerHTML=
    `<span class="chip">${f.writers.length} writer${f.writers.length!==1?"s":""}</span>`+
    `<span class="chip">${f.readers.length} reader${f.readers.length!==1?"s":""}</span>`+
    (f.orphanWrite?'<span class="chip" style="color:#b58a5a">write-only</span>':"")+
    (f.orphanRead?'<span class="chip" style="color:#79a0c4">read-only (engine-set)</span>':"");

  const W=f.writers, R=f.readers;
  const rowH=34, padTop=54, boxW=250, gap=150, cx=300+gap+boxW/2;
  const h=Math.max(W.length,R.length,1)*rowH+padTop+30;
  const svgW=boxW*2+gap+300+40;
  const colY=34;
  let s=`<svg width="${svgW}" height="${h}" viewBox="0 0 ${svgW} ${h}">`;
  s+=`<text class="col" x="${30+boxW/2}" y="${colY}" text-anchor="middle">writes ▸ (${W.length})</text>`;
  s+=`<text class="col" x="${cx}" y="${colY}" text-anchor="middle">flag</text>`;
  s+=`<text class="col" x="${svgW-30-boxW/2}" y="${colY}" text-anchor="middle">◂ reads (${R.length})</text>`;
  const fy=padTop+(Math.max(W.length,R.length,1)*rowH)/2-16;
  // edges first
  const ex1=30+boxW, ex2=svgW-30-boxW;
  W.forEach((w,i)=>{ const y=padTop+i*rowH+15;
    s+=`<path class="edge via-${w.via}" d="M${ex1},${y} C${ex1+60},${y} ${cx-boxW/2-60},${fy+16} ${cx-90},${fy+16}"/>`; });
  R.forEach((r,i)=>{ const y=padTop+i*rowH+15;
    s+=`<path class="edge via-${r.via}" d="M${cx+90},${fy+16} C${cx+boxW/2+60},${fy+16} ${ex2-60},${y} ${ex2},${y}"/>`; });
  // writer boxes
  W.forEach((w,i)=>{ const y=padTop+i*rowH; s+=srcBox(30,y,boxW,w,"end"); });
  // flag node
  s+=`<g class="flagnode"><rect x="${cx-90}" y="${fy}" width="180" height="32"/>`+
     `<text x="${cx}" y="${fy+21}" text-anchor="middle">${esc(trunc(key,22))}</text></g>`;
  // reader boxes
  R.forEach((r,i)=>{ const y=padTop+i*rowH; s+=srcBox(svgW-30-boxW,y,boxW,r,"start"); });
  if(!W.length) s+=`<text x="${30+boxW/2}" y="${fy+20}" text-anchor="middle" fill="#6e6680" font-style="italic">set in engine / code state</text>`;
  if(!R.length) s+=`<text x="${svgW-30-boxW/2}" y="${fy+20}" text-anchor="middle" fill="#6e6680" font-style="italic">nothing gates on it yet</text>`;
  s+=`</svg>`;
  document.getElementById("view").innerHTML=s+legend();
}
function srcBox(x,y,w,node,anchor){
  const tx = anchor==="end"? x+w-10 : x+10;
  return `<g class="src"><rect class="${node.via}" x="${x}" y="${y}" width="${w}" height="26" stroke-width="1.3"/>`+
    `<text class="${node.via}" x="${tx}" y="${y+17}" text-anchor="${anchor}">${esc(trunc(node.source,32))}</text></g>`;
}
function legend(){
  return `<div class="legend"><div><b style="background:#79a0c4"></b>dialogue</div>`+
    `<div><b style="background:#8f8a9e"></b>code / system</div>`+
    `<div><b style="background:#7fbf76"></b>codex unlock</div></div>`;
}

// ---- overview ----
function showOverview(){
  cur=null; document.querySelectorAll(".flag").forEach(e=>e.classList.remove("on"));
  document.getElementById("title").textContent=""; document.getElementById("dom").textContent="";
  document.getElementById("chips").innerHTML="";
  const counts={}; FLAGS.forEach(f=>counts[f.domain]=(counts[f.domain]||0)+1);
  const maxc=Math.max(...Object.values(counts));
  const doms=Object.keys(counts).sort((a,b)=>counts[b]-counts[a]);
  const conn=f=>f.writers.length+f.readers.length;
  const hubs=[...FLAGS].sort((a,b)=>conn(b)-conn(a)).slice(0,16);
  const writeOnly=FLAGS.filter(f=>f.orphanWrite);
  const readOnly=FLAGS.filter(f=>f.orphanRead);
  const pill=f=>`<span class="pill" onclick="showFlag('${f.key.replace(/'/g,"\\'")}')">${esc(f.key)} <span class="c">${conn(f)}</span></span>`;
  document.getElementById("view").innerHTML=`<div class="ov">
   <div class="statline">
    <div class="stat"><div class="n">${S.flags}</div><div class="l">flags</div></div>
    <div class="stat"><div class="n">${S.edges}</div><div class="l">read/write edges</div></div>
    <div class="stat"><div class="n">${S.domains}</div><div class="l">domains</div></div>
    <div class="stat"><div class="n">${S.orphanWrites}</div><div class="l">write-only</div></div>
    <div class="stat"><div class="n">${S.orphanReads}</div><div class="l">read-only (engine-set)</div></div>
   </div>
   <h3>Domains</h3>
   <div class="doms">${doms.map(d=>`<div class="domrow" onclick="pickDom('${d.replace(/'/g,"\\'")}')">
     <div style="flex:1"><div class="dn">${esc(d)}</div><div class="barfill" style="width:${Math.round(counts[d]/maxc*100)}%"></div></div>
     <div class="dc">${counts[d]}</div></div>`).join("")}</div>
   <h3>Hub flags — the most-connected wires</h3>
   <div class="note">The flags the most systems touch: tug one and the saga reshapes around it.</div>
   <div class="hublist">${hubs.map(pill).join("")}</div>
   <h3>Write-only flags (${writeOnly.length}) — set, but nothing gates on them yet</h3>
   <div class="note">Either headroom for future content, or telemetry/bookkeeping. Not bugs — just leaves.</div>
   <div class="orphlist">${writeOnly.map(pill).join("")}</div>
   <h3>Read-only flags (${readOnly.length}) — gated on, but set in engine/runtime state</h3>
   <div class="note">These are written by combat/exploration/runtime systems rather than a literal flag call (e.g. companion losses, kills, rests).</div>
   <div class="orphlist">${readOnly.map(pill).join("")}</div>
  </div>`;
}
function pickDom(d){ domFilter=d; buildDomFilter(); buildList(document.querySelector(".find").value);
  document.querySelector("aside").scrollTop=0; }

// ---- boot ----
buildDomFilter(); buildList(""); showOverview();
</script>
</body></html>
"""

stats = (f"{S['flags']} flags · {S['edges']} read/write edges · {S['domains']} domains "
         f"— harvested from the C#, the dialogue graphs, and the codex")
out = HTML.replace("__BLOB__", BLOB).replace("__STATS__", stats)
dst = os.path.join(ROOT, "play", "flags_explorer.html")
open(dst, "w").write(out)
print(f"wrote play/flags_explorer.html ({len(out)//1024} KB) — {S['flags']} flags, {S['edges']} edges")
