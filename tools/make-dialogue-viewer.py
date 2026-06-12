#!/usr/bin/env python3
"""
Generate play/dialogue_viewer.html — an interactive browser view of every branching
conversation in the game, built straight from play/dialogue-data.json (produced by
tools/extract-dialogue.py from the real C# DialogueGraph builders).

Two modes per conversation:
  • Map  — an auto-laid-out SVG node-link graph of the whole tree: speakers, choice
           edges, skill-check edges (gold, ability + DC, with the fail branch dashed),
           flag effects. Click a node for its full line.
  • Walk — play the conversation: pick choices, roll the skill checks, watch the flags
           the game would set tick over in a live state panel.

Re-run after extract:
  python3 tools/extract-dialogue.py && python3 tools/make-dialogue-viewer.py
"""
import json, os

ROOT = os.path.join(os.path.dirname(__file__), "..")
DATA = json.load(open(os.path.join(ROOT, "play", "dialogue-data.json")))
S = DATA["stats"]
BLOB = json.dumps(DATA, ensure_ascii=False).replace("</", "<\\/")

HTML = """<!DOCTYPE html>
<html lang="en"><head><meta charset="utf-8"><meta name="viewport" content="width=device-width, initial-scale=1">
<title>Crown of Horns — Dialogue Tree Viewer</title>
<style>
 *{box-sizing:border-box}
 body{margin:0;height:100vh;overflow:hidden;display:flex;flex-direction:column;
  background:radial-gradient(1100px 640px at 18% -10%,#27212f,#0a0910);
  color:#d8d2c2;font:14px/1.55 "Iowan Old Style","Palatino Linotype",Georgia,serif}
 header{padding:12px 20px;border-bottom:1px solid #2a2636;display:flex;align-items:baseline;gap:16px;flex-wrap:wrap}
 header h1{margin:0;color:#e7c873;font-size:20px;letter-spacing:.4px}
 header .s{color:#8d8499;font-size:12px;font-style:italic}
 header a{color:#c9a24b;font-size:12px;text-decoration:none;margin-left:auto}
 header a:hover{text-decoration:underline}
 .wrap{flex:1;display:flex;min-height:0}
 /* sidebar */
 aside{width:268px;border-right:1px solid #2a2636;overflow-y:auto;padding:8px 0;background:#0d0b13}
 .find{margin:6px 10px 10px;width:calc(100% - 20px);background:#16131e;border:1px solid #2a2636;border-radius:7px;
  color:#d8d2c2;padding:7px 9px;font:13px serif}
 .grp{color:#7b7388;font-size:10.5px;text-transform:uppercase;letter-spacing:1px;margin:12px 14px 4px}
 .conv{padding:6px 14px;cursor:pointer;font-size:13px;color:#b9b1c4;border-left:2px solid transparent;line-height:1.3}
 .conv:hover{background:#16131e;color:#e7c873}
 .conv.on{background:#1c1828;color:#e7c873;border-left-color:#e7c873}
 .conv .meta{display:block;color:#6e6680;font-size:10.5px}
 /* main */
 main{flex:1;display:flex;flex-direction:column;min-width:0}
 .bar{padding:10px 18px;border-bottom:1px solid #2a2636;display:flex;align-items:center;gap:12px;flex-wrap:wrap}
 .bar h2{margin:0;color:#e7c873;font-size:17px}
 .bar .id{color:#7b7388;font-size:11px;font-family:ui-monospace,monospace}
 .chips{display:flex;gap:6px;margin-left:auto}
 .chip{font-size:10.5px;color:#9a90a8;border:1px solid #2a2636;border-radius:5px;padding:2px 7px}
 .toggle{display:flex;border:1px solid #2a2636;border-radius:7px;overflow:hidden}
 .toggle button{background:#13111a;color:#9a90a8;border:0;padding:5px 14px;cursor:pointer;font:12px serif}
 .toggle button.on{background:#e7c873;color:#1a1622;font-weight:600}
 .view{flex:1;overflow:auto;position:relative}
 /* map */
 svg{display:block}
 .nodebox{cursor:pointer}
 .nodebox rect{fill:#17141f;stroke:#39334a;stroke-width:1.5;rx:9}
 .nodebox.start rect{stroke:#e7c873;stroke-width:2.2}
 .nodebox.sel rect{stroke:#c9a24b;fill:#211b2c}
 .nodebox .sp{fill:#e7c873;font:600 12px serif}
 .nodebox .tx{fill:#b4abc0;font:11.5px serif}
 .nodebox .eff{fill:#6f9e6a;font:10px ui-monospace,monospace}
 .edge{fill:none;stroke:#4b4460;stroke-width:1.6}
 .edge.check{stroke:#d8a73c;stroke-width:2}
 .edge.fail{stroke:#9d5252;stroke-dasharray:5 4}
 .edge.auto{stroke:#3c3850;stroke-dasharray:2 5}
 .elabel{font:10.5px serif;fill:#9a90a8}
 .elabel.check{fill:#e7c873}
 .term{fill:#6e6680;font:italic 10.5px serif}
 /* detail panel */
 .detail{position:absolute;right:0;top:0;width:340px;height:100%;background:#0e0c14ee;border-left:1px solid #2a2636;
  padding:16px 18px;overflow-y:auto;backdrop-filter:blur(3px);display:none}
 .detail.show{display:block}
 .detail h3{margin:0 0 2px;color:#e7c873;font-size:15px}
 .detail .who{color:#9a90a8;font-size:11px;font-style:italic;margin-bottom:10px}
 .detail .body{color:#cfc7d8;white-space:pre-wrap;font-size:13px;border-left:2px solid #2a2636;padding-left:11px;margin-bottom:12px}
 .detail .dyn{color:#b58a5a;font-style:italic}
 .detail .lbl{color:#7b7388;font-size:10px;text-transform:uppercase;letter-spacing:1px;margin:12px 0 5px}
 .ch{border:1px solid #241f30;border-radius:7px;padding:8px 10px;margin-bottom:7px;background:#13111a}
 .ch .t{color:#d8d2c2;font-size:12.5px}
 .ch .arr{color:#6e6680;font-family:ui-monospace,monospace;font-size:10.5px;margin-top:3px}
 .tag{display:inline-block;font-size:10px;border-radius:4px;padding:1px 6px;margin:3px 4px 0 0}
 .tag.chk{background:#3a2f14;color:#e7c873;border:1px solid #5a4a1e}
 .tag.eff{background:#16281a;color:#7fbf76;border:1px solid #2c4a2e}
 .tag.cond{background:#1a2230;color:#79a0c4;border:1px solid #2c3e54}
 .x{position:absolute;top:10px;right:12px;color:#9a90a8;cursor:pointer;font-size:18px;background:none;border:0}
 /* walk */
 .walk{max-width:680px;margin:0 auto;padding:26px 22px 80px}
 .line{margin-bottom:18px}
 .line .who{color:#e7c873;font-weight:600;font-size:13px;margin-bottom:3px}
 .line .body{color:#cfc7d8;white-space:pre-wrap}
 .line.me .who{color:#79a0c4}
 .line.me .body{color:#a9c0d8;font-style:italic}
 .line.sys{color:#8d8499;font-style:italic;text-align:center;border-top:1px solid #221d2c;padding-top:14px}
 .opts{margin-top:6px}
 .opt{display:block;width:100%;text-align:left;background:#15121d;border:1px solid #2a2636;color:#d8d2c2;
  border-radius:8px;padding:10px 13px;margin-bottom:8px;cursor:pointer;font:13px serif;transition:.12s}
 .opt:hover{border-color:#e7c873;background:#1c1828}
 .opt .ck{color:#e7c873;font-size:11px;margin-top:4px}
 .flags{position:fixed;right:14px;bottom:14px;width:230px;background:#0e0c14ee;border:1px solid #2a2636;border-radius:9px;
  padding:10px 12px;font-size:11.5px;max-height:40vh;overflow-y:auto;display:none}
 .flags.show{display:block}
 .flags h4{margin:0 0 6px;color:#e7c873;font-size:11px;text-transform:uppercase;letter-spacing:1px}
 .flags .f{font-family:ui-monospace,monospace;color:#8fae8a;font-size:10.5px;line-height:1.5}
 .restart{background:#241f30;border:1px solid #3a3550;color:#e7c873;border-radius:7px;padding:7px 14px;cursor:pointer;font:13px serif;margin-top:8px}
</style></head><body>
<header>
 <h1>👑 Dialogue Tree Viewer</h1>
 <span class="s">__STATS__</span>
 <a href="index.html">← back to the hub</a>
</header>
<div class="wrap">
 <aside><input class="find" placeholder="search conversations…" oninput="filter(this.value)"><div id="list"></div></aside>
 <main>
  <div class="bar">
   <h2 id="title">—</h2><span class="id" id="cid"></span>
   <div class="toggle"><button id="mMap" class="on" onclick="setMode('map')">Map</button><button id="mWalk" onclick="setMode('walk')">Walk</button></div>
   <div class="chips" id="chips"></div>
  </div>
  <div class="view" id="view"></div>
 </main>
</div>
<div class="flags" id="flags"><h4>Flags this run sets</h4><div id="flagbody" class="f"></div></div>
<script>
const DATA = __BLOB__;
const CONVS = DATA.conversations;
let cur = null, mode = "map", selNode = null;

// ---- sidebar ----
function groupOf(id){ const i=id.indexOf("."); return (i<0?id:id.slice(0,i)).replace(/[()*]/g,""); }
function buildList(q){
  q=(q||"").toLowerCase();
  const groups={};
  CONVS.forEach((c,idx)=>{ if(q && !(c.title+" "+c.id).toLowerCase().includes(q)) return;
    (groups[groupOf(c.id)]=groups[groupOf(c.id)]||[]).push([idx,c]); });
  const el=document.getElementById("list"); el.innerHTML="";
  Object.keys(groups).sort().forEach(g=>{
    const h=document.createElement("div"); h.className="grp"; h.textContent=g; el.appendChild(h);
    groups[g].forEach(([idx,c])=>{
      const d=document.createElement("div"); d.className="conv"+(cur===idx?" on":""); d.dataset.i=idx;
      const nn=c.nodes.length, ch=c.nodes.reduce((a,n)=>a+n.choices.length,0);
      d.innerHTML=esc(c.title)+'<span class="meta">'+nn+' nodes · '+ch+' choices · '+c.file+'</span>';
      d.onclick=()=>load(idx); el.appendChild(d);
    });
  });
}
function filter(v){ buildList(v); }

// ---- helpers ----
function esc(s){ return (s==null?"":String(s)).replace(/[&<>]/g,c=>({"&":"&amp;","<":"&lt;",">":"&gt;"}[c])); }
function trunc(s,n){ s=s||""; return s.length>n? s.slice(0,n-1)+"…":s; }
function clauseTag(c){
  const op=c.op||"";
  if(op==="SetTrue") return c.key+" → true";
  if(op==="SetFalse") return c.key+" → false";
  if(op==="AddInt") return c.key+" "+(c.amount>=0?"+":"")+c.amount;
  if(op==="RequireBoolTrue") return "needs "+c.key;
  if(op==="RequireBoolFalse") return "needs !"+c.key;
  if(op==="RequireIntAtLeast") return c.key+" ≥ "+c.amount;
  return c.key+" "+op;
}

// ---- load a conversation ----
function load(idx){
  cur=idx; selNode=null;
  const c=CONVS[idx];
  document.getElementById("title").textContent=c.title;
  document.getElementById("cid").textContent=c.id+"  ·  "+c.file;
  const nn=c.nodes.length, ch=c.nodes.reduce((a,n)=>a+n.choices.length,0),
        ck=c.nodes.reduce((a,n)=>a+n.choices.filter(x=>x.checkDC).length,0);
  document.getElementById("chips").innerHTML=
    '<span class="chip">'+nn+' nodes</span><span class="chip">'+ch+' choices</span>'+
    (ck?'<span class="chip">🎲 '+ck+' skill check'+(ck>1?'s':'')+'</span>':'');
  buildList(document.querySelector(".find").value);
  render();
}
function setMode(m){ mode=m; document.getElementById("mMap").classList.toggle("on",m==="map");
  document.getElementById("mWalk").classList.toggle("on",m==="walk"); render(); }
function render(){ if(cur==null) return; document.getElementById("flags").classList.toggle("show",mode==="walk");
  mode==="map"? renderMap(): renderWalk(); }

// ---- MAP: layered DAG ----
function renderMap(){
  const c=CONVS[cur], nodes=c.nodes, byId={}; nodes.forEach(n=>byId[n.id]=n);
  // assign levels by BFS from start
  const level={}; const start=byId[c.start]?c.start:nodes[0].id;
  const q=[[start,0]]; level[start]=0;
  while(q.length){ const [id,l]=q.shift(); const n=byId[id]; if(!n) continue;
    const outs=[]; n.choices.forEach(ch=>{ if(ch.next)outs.push(ch.next); if(ch.fail)outs.push(ch.fail);});
    if(n.auto)outs.push(n.auto);
    outs.forEach(t=>{ if(byId[t] && level[t]===undefined){ level[t]=l+1; q.push([t,l+1]); } });
  }
  let maxL=0; nodes.forEach(n=>{ if(level[n.id]===undefined) level[n.id]=0; maxL=Math.max(maxL,level[n.id]); });
  // unreached nodes → place after max
  nodes.forEach(n=>{ if(level[n.id]===undefined) level[n.id]=maxL+1; });
  const byLvl={}; nodes.forEach(n=>{ (byLvl[level[n.id]]=byLvl[level[n.id]]||[]).push(n); });
  const W=300, H=92, GX=70, GY=66; const pos={};
  let maxCols=0; Object.values(byLvl).forEach(a=>maxCols=Math.max(maxCols,a.length));
  Object.keys(byLvl).forEach(l=>{ byLvl[l].forEach((n,i)=>{
    pos[n.id]={x:30+i*(W+GX), y:30+(+l)*(H+GY)}; }); });
  const svgW=Math.max(maxCols*(W+GX)+40, 600), svgH=(maxL+1)*(H+GY)+40;
  let edges="", boxes="";
  nodes.forEach(n=>{
    const p=pos[n.id]; if(!p) return;
    const drawEdge=(targetId,cls,label)=>{
      const t=pos[targetId];
      if(!t){ // terminal / cross-graph
        edges+=`<path class="edge ${cls}" d="M${p.x+W/2},${p.y+H} C${p.x+W/2},${p.y+H+26} ${p.x+W/2},${p.y+H+26} ${p.x+W/2},${p.y+H+34}"/>`+
               `<text class="term" x="${p.x+W/2}" y="${p.y+H+48}" text-anchor="middle">${esc(label?trunc(label,30):"")} ▸ ${esc(targetId||"end")}</text>`;
        return;
      }
      const x1=p.x+W/2,y1=p.y+H,x2=t.x+W/2,y2=t.y, my=(y1+y2)/2;
      edges+=`<path class="edge ${cls}" d="M${x1},${y1} C${x1},${my} ${x2},${my} ${x2},${y2}"/>`;
      if(label) edges+=`<text class="elabel ${cls==='check'?'check':''}" x="${(x1+x2)/2}" y="${my-3}" text-anchor="middle">${esc(trunc(label,30))}</text>`;
    };
    n.choices.forEach(ch=>{
      if(ch.checkDC){ drawEdge(ch.next,"check","🎲 "+ch.checkAbility.slice(0,3).toUpperCase()+" DC"+ch.checkDC+" — "+(ch.text||""));
        if(ch.fail) drawEdge(ch.fail,"fail","✗ fail"); }
      else drawEdge(ch.next,"",ch.text);
    });
    if(n.auto && !n.choices.length) drawEdge(n.auto,"auto","");
    if(!n.choices.length && !n.auto)
      edges+=`<text class="term" x="${p.x+W/2}" y="${p.y+H+20}" text-anchor="middle">▪ conversation ends</text>`;
  });
  nodes.forEach(n=>{ const p=pos[n.id]; if(!p) return;
    const isStart=n.id===(byId[c.start]?c.start:nodes[0].id);
    const sel=selNode===n.id;
    const eff=n.onEnter.map(clauseTag);
    boxes+=`<g class="nodebox ${isStart?'start':''} ${sel?'sel':''}" onclick="pick('${n.id}')">`+
      `<rect x="${p.x}" y="${p.y}" width="${W}" height="${H}" rx="9"/>`+
      `<text class="sp" x="${p.x+12}" y="${p.y+20}">${esc(trunc(n.speaker,34))}${isStart?'  ▸start':''}</text>`+
      wrapText(n.dynamic?"〔reactive line — varies〕":(n.text||""), p.x+12, p.y+38, 46, 3, n.dynamic?"tx dyn":"tx")+
      (eff.length?`<text class="eff" x="${p.x+12}" y="${p.y+H-8}">⚑ ${esc(trunc(eff.join(", "),44))}</text>`:"")+
      `</g>`;
  });
  document.getElementById("view").innerHTML=
    `<svg width="${svgW}" height="${svgH+30}" viewBox="0 0 ${svgW} ${svgH+30}">${edges}${boxes}</svg>`+
    `<div class="detail" id="detail"></div>`;
  if(selNode) showDetail(selNode);
}
function wrapText(s,x,y,per,maxLines,cls){
  s=s||""; const words=s.split(/\s+/); let line="",lines=[];
  for(const w of words){ if((line+" "+w).length>per){ lines.push(line); line=w; if(lines.length>=maxLines)break; } else line=line?line+" "+w:w; }
  if(lines.length<maxLines && line) lines.push(line);
  if(lines.length>=maxLines) lines[maxLines-1]=trunc(lines[maxLines-1],per)+(s.length>lines.join(" ").length?"…":"");
  return lines.map((l,i)=>`<text class="${cls||'tx'}" x="${x}" y="${y+i*14}">${esc(l)}</text>`).join("");
}
function pick(id){ selNode=id; renderMap(); }
function showDetail(id){
  const c=CONVS[cur], n=c.nodes.find(x=>x.id===id); if(!n) return;
  const d=document.getElementById("detail"); d.classList.add("show");
  let h=`<button class="x" onclick="selNode=null;renderMap()">×</button>`;
  h+=`<h3>${esc(n.id)}</h3><div class="who">${esc(n.speaker)}</div>`;
  h+=`<div class="body${n.dynamic?' dyn':''}">${n.dynamic?'〔This line is written live from the story flags — it reads differently depending on how the speaker\\'s quest and the world have gone.〕':esc(n.text||"")}</div>`;
  if(n.onEnter.length){ h+=`<div class="lbl">on enter</div>`; n.onEnter.forEach(e=>h+=`<span class="tag eff">⚑ ${esc(clauseTag(e))}</span>`); }
  if(n.choices.length){ h+=`<div class="lbl">choices</div>`;
    n.choices.forEach(ch=>{ h+=`<div class="ch"><div class="t">${esc(ch.text||"(continue)")}</div>`;
      let arr=ch.next?("▸ "+ch.next):"▸ ends";
      h+=`<div class="arr">${esc(arr)}</div>`;
      if(ch.checkDC) h+=`<span class="tag chk">🎲 ${esc(ch.checkAbility)} DC ${ch.checkDC}${ch.fail?" · fail ▸ "+esc(ch.fail):""}</span>`;
      (ch.conditions||[]).forEach(x=>h+=`<span class="tag cond">? ${esc(clauseTag(x))}</span>`);
      (ch.effects||[]).forEach(x=>h+=`<span class="tag eff">⚑ ${esc(clauseTag(x))}</span>`);
      h+=`</div>`; });
  } else if(n.auto){ h+=`<div class="lbl">auto-advance</div><div class="arr">▸ ${esc(n.auto)}</div>`; }
  else { h+=`<div class="lbl">▪ conversation ends here</div>`; }
  d.innerHTML=h;
}

// ---- WALK: play it ----
let walkFlags={};
function renderWalk(){
  walkFlags={}; const v=document.getElementById("view");
  v.innerHTML=`<div class="walk" id="walk"></div>`;
  document.getElementById("flagbody").innerHTML="<i style='color:#6e6680'>none yet</i>";
  const c=CONVS[cur]; goNode(c.start && c.nodes.find(n=>n.id===c.start)? c.start : c.nodes[0].id, true);
}
function applyClauses(list){ (list||[]).forEach(e=>{
  if(e.op==="SetTrue") walkFlags[e.key]=true;
  else if(e.op==="SetFalse") walkFlags[e.key]=false;
  else if(e.op==="AddInt") walkFlags[e.key]=(walkFlags[e.key]||0)+ (e.amount||0);
}); paintFlags(); }
function paintFlags(){ const keys=Object.keys(walkFlags);
  document.getElementById("flagbody").innerHTML= keys.length? keys.sort().map(k=>
    `<div class="f">${esc(k)} = ${esc(String(walkFlags[k]))}</div>`).join(""):"<i style='color:#6e6680'>none yet</i>"; }
function condOk(list){ return (list||[]).every(c=>{
  if(c.op==="RequireBoolTrue") return walkFlags[c.key]===true;
  if(c.op==="RequireBoolFalse") return !walkFlags[c.key];
  if(c.op==="RequireIntAtLeast") return (walkFlags[c.key]||0)>=c.amount; return true; }); }
function goNode(id, first){
  const c=CONVS[cur], n=c.nodes.find(x=>x.id===id), w=document.getElementById("walk");
  if(!n){ addLine("sys","",(id?"▸ continues elsewhere ("+id+")":"")+" — conversation ends."); endWalk(); return; }
  applyClauses(n.onEnter);
  addLine("", n.speaker, n.dynamic?"〔This line is written live from the story flags — here shown as a placeholder.〕":(n.text||""));
  const opts=document.createElement("div"); opts.className="opts"; w.appendChild(opts);
  const choices=n.choices.filter(ch=>condOk(ch.conditions));
  if(choices.length){
    choices.forEach(ch=>{
      const b=document.createElement("button"); b.className="opt";
      b.innerHTML=esc(ch.text||"(continue)")+(ch.checkDC?`<div class="ck">🎲 ${esc(ch.checkAbility)} check · DC ${ch.checkDC}</div>`:"");
      b.onclick=()=>{ opts.remove(); addLine("me","You", ch.text||"(continue)");
        applyClauses(ch.effects);
        if(ch.checkDC){ const roll=1+Math.floor(Math.random()*20), ok=roll+0>=ch.checkDC; // unmodified d20 vs DC (mods unknown headless)
          addLine("sys","", `🎲 ${ch.checkAbility} check — rolled ${roll} vs DC ${ch.checkDC} → ${ok?"SUCCESS":"FAIL"}`);
          goNode(ok? ch.next : (ch.fail||ch.next)); }
        else goNode(ch.next);
      }; opts.appendChild(b);
    });
  } else if(n.auto){
    const b=document.createElement("button"); b.className="opt"; b.textContent="(continue)";
    b.onclick=()=>{ opts.remove(); goNode(n.auto); }; opts.appendChild(b);
  } else { addLine("sys","","▪ The conversation ends."); endWalk(); }
  w.scrollIntoView(false); window.scrollTo(0,document.body.scrollHeight);
  document.getElementById("view").scrollTop=document.getElementById("view").scrollHeight;
}
function addLine(cls,who,body){ const w=document.getElementById("walk"); const d=document.createElement("div");
  d.className="line "+cls; d.innerHTML=(who?`<div class="who">${esc(who)}</div>`:"")+`<div class="body">${esc(body)}</div>`;
  w.appendChild(d); }
function endWalk(){ const w=document.getElementById("walk"); const b=document.createElement("button");
  b.className="restart"; b.textContent="↻ Play again"; b.onclick=renderWalk; w.appendChild(b);
  document.getElementById("view").scrollTop=document.getElementById("view").scrollHeight; }

// ---- boot ----
buildList(""); load(0);
</script>
</body></html>
"""

stats = (f"{S['conversations']} conversations · {S['nodes']} nodes · {S['choices']} choices · "
         f"{S['skillChecks']} skill checks — extracted from the C# DialogueGraphs")
out = HTML.replace("__BLOB__", BLOB).replace("__STATS__", stats)
dst = os.path.join(ROOT, "play", "dialogue_viewer.html")
open(dst, "w").write(out)
print(f"wrote play/dialogue_viewer.html ({len(out)//1024} KB) — "
      f"{S['conversations']} conversations, {S['nodes']} nodes")
