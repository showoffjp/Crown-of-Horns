// Generates endings_explorer.html: a standalone, interactive preview of the game's
// narrative reactivity. The engine is INLINED from endings.js at build time (module
// plumbing stripped), so the explorer always carries the same byte-parity prose the
// test suite pins. Re-run after any endings.js change: node make-explorer.js
const fs = require("fs");
let engine = fs.readFileSync("endings.js", "utf8")
  .replace(/module\.exports[\s\S]*$/, "");  // strip the export block

const FLAGS = [
  ["Companions", [
    ["companion.roen.recruited","Roen recruited"],["companion.varra.recruited","Varra recruited"],
    ["companion.naeve.recruited","Naeve recruited"],["companion.ilfaeril.recruited","Ilfaeril recruited"],
    ["companion.maerin.recruited","Maerin pulled from the Wall"],
    ["companion.garrow.lost","Garrow LOST at the Breach"],["companion.roen.lost","Roen LOST"],
    ["companion.varra.lost","Varra LOST"],["companion.naeve.lost","Naeve LOST"],
    ["companion.roen.left","Roen walked away"],["companion.varra.left","Varra walked away"]]],
  ["Hearts", [
    ["romance.garrow.consummated","Garrow — love consummated"],["romance.roen.consummated","Roen — love consummated"],
    ["romance.naeve.consummated","Naeve — love consummated"],["romance.varra.consummated","Varra — love consummated"],
    ["romance.garrow.turn","Garrow — the Turn"],["romance.varra.turn","Varra — the Turn"]]],
  ["Personal quests", [
    ["quest.roen.resolved","Roen's quest resolved"],["quest.roen.double_agent","· Wrenna turned double agent"],
    ["quest.roen.wrenna_saved","· Wrenna saved by mercy"],["quest.roen.harper_boon","· handed to the Harpers"],
    ["quest.varra.resolved","Varra's quest resolved"],["quest.varra.patron_bound","· patron bound by its name"],
    ["quest.varra.debt_taken","· you took her debt"],["quest.varra.freed","· she burned the contract"],
    ["quest.garrow.resolved","Garrow's quest resolved"],["quest.garrow.doctrine_won","· doctrine put on trial"],
    ["quest.garrow.left_faith","· she left the faith"],["quest.garrow.recanted","· she recanted, kept the grey"],
    ["quest.naeve.resolved","Naeve's quest resolved"],["quest.naeve.rekindled","· the last Weave, given away"],
    ["quest.naeve.released","· let Netheril fall"],["quest.naeve.preserved","· sealed it in stasis"],
    ["quest.ilfaeril.resolved","Ilfaeril's quest resolved"],["quest.ilfaeril.commission","· forgiveness as a sword"],
    ["quest.ilfaeril.forgiven","· he let himself be forgiven"],["quest.ilfaeril.penance","· he kept the weight"]]],
  ["The deep truths", [
    ["readers_boon","You read the Lady (readers' boon)"],["pc.true_name","You learned your true name"],
    ["crownwars.verdict_spared","The Verdict — argued DOWN"],["crownwars.verdict_passed","The Verdict — passed"],
    ["aldric.named_monster","You named Aldric a monster"],["aldric.provisional","Aldric stood down, provisional"]]],
  ["Deeds & world", [
    ["netheril.cleared","Netheril cleared"],["crownwars.cleared","Crown Wars cleared"],
    ["act4.toot_done","Time of Troubles done"],["act4.spellplague_done","Spellplague done"],
    ["netheril.boss_down","Mythallar Colossus slain"],["crownwars.boss_down","First Unmade laid to rest"],
    ["toot.avatar_down","Avatar of Bone broken"],["spellplague.herald_down","Herald of the Unmade held"],
    ["lowcity.allies","The Lower City stands with you"],["almshouse.token","Mother Cass's token carried"],
    ["ng.plus","New Game+ (the loop, seen)"]]],
];
const INTS = [["faction.kelemvor.reputation","Doomguide reputation",-5,9],
              ["faction.choir.reputation","Faithless Choir reputation",-5,9],
              ["companion.garrow.approval","Garrow approval",-30,60],
              ["companion.roen.approval","Roen approval",-30,60]];

const html = `<!DOCTYPE html>
<html lang="en"><head><meta charset="utf-8"><meta name="viewport" content="width=device-width, initial-scale=1">
<title>Crown of Horns — Endings & Epilogue Explorer</title>
<style>
 *{box-sizing:border-box} body{margin:0;background:radial-gradient(1100px 600px at 50% -10%, #241f30, #0c0b10);
 color:#d8d2c2;font:14px/1.55 "Iowan Old Style","Palatino Linotype",Georgia,serif}
 header{padding:16px 24px;border-bottom:1px solid #2a2636}
 h1{margin:0;font-size:22px;color:#e7c873} .sub{color:#8a8198;font-size:12px;font-style:italic}
 .wrap{display:flex;gap:18px;padding:18px 24px;align-items:flex-start;flex-wrap:wrap}
 .panel{background:linear-gradient(#16141d,#131119);border:1px solid #2a2636;border-radius:8px;padding:14px 16px}
 .flags{width:360px;max-height:78vh;overflow:auto}
 .flags h3{margin:12px 0 4px;font-size:11px;letter-spacing:1.5px;text-transform:uppercase;color:#c9a24b}
 label{display:flex;gap:8px;align-items:center;font-size:12.5px;padding:2px 0;cursor:pointer;color:#b8b0c4}
 label:hover{color:#e7c873} input[type=checkbox]{accent-color:#c9a24b}
 .ints label{justify-content:space-between} input[type=range]{width:130px;accent-color:#c9a24b}
 .main{flex:1;min-width:420px;display:flex;flex-direction:column;gap:14px}
 .ending{border:1px solid #2a2636;border-radius:8px;padding:10px 14px;margin:6px 0;cursor:pointer;background:#14121b}
 .ending:hover{border-color:#c9a24b} .ending.locked{opacity:.35;cursor:default;filter:grayscale(.6)}
 .ending.sel{border-color:#e7c873;box-shadow:0 0 0 1px #c9a24b55}
 .ending .t{color:#e7c873;font-weight:600} .ending .c{color:#8a8198;font-size:12px}
 .golden{outline:1px dashed #e7c87366}
 .prose{font-size:15px;line-height:1.7;color:#e8e2d2;font-style:italic}
 .slide{margin:9px 0;padding:8px 12px;border-left:3px solid #c9a24b55;background:#14121b;border-radius:0 6px 6px 0;font-size:13px}
 .chron .slide{border-color:#4a6b8a55}
 .count{color:#8a8198;font-size:12px}
 button{font:inherit;font-size:12.5px;background:#221d2b;color:#e7c873;border:1px solid #c9a24b66;border-radius:6px;padding:6px 12px;cursor:pointer;margin-right:8px}
 .foot{padding:8px 24px 22px;color:#6e6680;font-size:11.5px} code{color:#c9a24b}
</style></head><body>
<header><h1>Crown of Horns — Endings &amp; Epilogue Explorer</h1>
<span class="sub">The narrative engine, live: flip what happened in your playthrough; watch which endings you've <i>earned</i>, and read the epilogue the game would write for you. Same code, byte-identical prose, as the Unity build (pinned by 28 tests + a prose-parity gate).</span></header>
<div class="wrap">
 <div class="panel flags" id="flags"></div>
 <div class="main">
  <div class="panel"><h3 style="margin:0 0 6px;font-size:11px;letter-spacing:1.5px;text-transform:uppercase;color:#c9a24b">Endings — <span id="count" class="count"></span></h3>
   <div id="endings"></div>
   <button id="preset1">Preset: golden road, everyone alive &amp; loved</button>
   <button id="preset2">Preset: the devastating run</button>
   <button id="reset">Reset</button></div>
  <div class="panel" id="proseBox" style="display:none"><div class="prose" id="prose"></div></div>
  <div class="panel" id="epi" style="display:none"><h3 style="margin:0 0 4px;font-size:11px;letter-spacing:1.5px;text-transform:uppercase;color:#c9a24b">Where they are now</h3><div id="slides"></div></div>
  <div class="panel chron"><h3 style="margin:0 0 4px;font-size:11px;letter-spacing:1.5px;text-transform:uppercase;color:#c9a24b">The Chronicle</h3><div id="chron"></div></div>
 </div>
</div>
<div class="foot">Generated from <code>play/endings.js</code> — the 1:1 port of <code>Core/EndingResolver.cs</code> + <code>GameFlags.cs</code>. Regenerate with <code>node play/make-explorer.js</code>.</div>
<script>
${engine}
// ---------------- explorer UI ----------------
const FLAGS=${JSON.stringify(FLAGS)};
const INTS=${JSON.stringify(INTS)};
let selected=null;
const $=id=>document.getElementById(id);
function buildFlags(){
  const root=$("flags");root.innerHTML="";
  for(const[group,items]of FLAGS){
    const h=document.createElement("h3");h.textContent=group;root.appendChild(h);
    for(const[key,label]of items){
      const l=document.createElement("label");
      const cb=document.createElement("input");cb.type="checkbox";cb.dataset.key=key;
      cb.checked=GameFlags.Current.GetBool(key);
      cb.onchange=()=>{GameFlags.Current.SetBool(key,cb.checked);render();};
      l.appendChild(cb);l.appendChild(document.createTextNode(label));root.appendChild(l);
    }}
  const h=document.createElement("h3");h.textContent="Standing & bonds";root.appendChild(h);
  const ints=document.createElement("div");ints.className="ints";root.appendChild(ints);
  for(const[key,label,min,max]of INTS){
    const l=document.createElement("label");
    const span=document.createElement("span");
    const r=document.createElement("input");r.type="range";r.min=min;r.max=max;r.dataset.key=key;
    r.value=GameFlags.Current.GetInt(key);
    const upd=()=>{span.textContent=label+": "+r.value;};upd();
    r.oninput=()=>{GameFlags.Current.SetInt(key,+r.value);upd();render();};
    l.appendChild(span);l.appendChild(r);ints.appendChild(l);
  }}
function render(){
  const avail=EndingResolver.Available();
  $("count").textContent=avail.length+"/6 earned";
  const box=$("endings");box.innerHTML="";
  for(const e of ALL_ENDINGS){
    const ok=avail.includes(e);
    const d=document.createElement("div");
    d.className="ending"+(ok?"":" locked")+(selected===e?" sel":"")+(EndingResolver.IsGolden(e)?" golden":"");
    d.innerHTML='<div class="t">'+EndingResolver.Title(e)+'</div><div class="c">'+
      (ok?EndingResolver.Choice(e):"— not yet earned: the deeper truths gate the deeper roads —")+'</div>';
    if(ok)d.onclick=()=>{selected=e;render();};
    box.appendChild(d);
  }
  if(selected&&avail.includes(selected)){
    $("proseBox").style.display="block";$("epi").style.display="block";
    $("prose").textContent=EndingResolver.Prose(selected);
    const s=$("slides");s.innerHTML="";
    for(const slide of EndingResolver.Epilogue(selected)){const d=document.createElement("div");d.className="slide";d.textContent=slide;s.appendChild(d);}
  } else { $("proseBox").style.display="none";$("epi").style.display="none"; }
  const c=$("chron");c.innerHTML="";
  for(const line of EndingResolver.Chronicle()){const d=document.createElement("div");d.className="slide";d.textContent=line;c.appendChild(d);}
}
function setAll(pairs){GameFlags.Replace(new GameFlags());for(const[k,v]of pairs)typeof v==="number"?GameFlags.Current.SetInt(k,v):GameFlags.Current.SetBool(k,v);selected=null;buildFlags();render();}
$("preset1").onclick=()=>setAll([["companion.roen.recruited",true],["companion.varra.recruited",true],["companion.naeve.recruited",true],["companion.ilfaeril.recruited",true],["companion.maerin.recruited",true],
 ["romance.naeve.consummated",true],["quest.roen.resolved",true],["quest.roen.wrenna_saved",true],["quest.varra.resolved",true],["quest.varra.patron_bound",true],
 ["quest.garrow.resolved",true],["quest.garrow.doctrine_won",true],["quest.naeve.resolved",true],["quest.naeve.rekindled",true],["quest.ilfaeril.resolved",true],["quest.ilfaeril.commission",true],
 ["readers_boon",true],["crownwars.verdict_spared",true],["aldric.provisional",true],["netheril.cleared",true],["crownwars.cleared",true],["act4.toot_done",true],["act4.spellplague_done",true],
 ["lowcity.allies",true],["almshouse.token",true],["faction.kelemvor.reputation",6],["companion.garrow.approval",45]]);
$("preset2").onclick=()=>setAll([["companion.roen.recruited",true],["companion.varra.recruited",true],["companion.naeve.recruited",true],
 ["romance.varra.consummated",true],["companion.varra.lost",true],["companion.roen.left",true],
 ["quest.garrow.resolved",true],["quest.garrow.recanted",true],["crownwars.verdict_passed",true],["aldric.named_monster",true],
 ["netheril.cleared",true],["pc.true_name",true],["faction.choir.reputation",6]]);
$("reset").onclick=()=>setAll([]);
buildFlags();render();
</script></body></html>`;
fs.writeFileSync("endings_explorer.html", html);
console.log("wrote endings_explorer.html (" + (html.length/1024).toFixed(0) + " KB)");
