// Boots endings_explorer.html under a stubbed DOM: flips every flag, drags every
// slider, applies both presets, selects every earnable ending — no throw, prose
// and slides render non-empty.
const fs=require("fs"),vm=require("vm");
const html=fs.readFileSync("endings_explorer.html","utf8");
const script=html.match(/<script>([\s\S]*)<\/script>/)[1];
function el(){const e={innerHTML:"",textContent:"",className:"",style:{},dataset:{},children:[],
  appendChild(c){this.children.push(c);return c;},onchange:null,onclick:null,oninput:null,checked:false,value:0,type:""};
  return e;}
const els={};const document={getElementById:id=>els[id]||(els[id]=el()),createElement:()=>el(),
  createTextNode:t=>({text:t})};
// patch appendChild on getElementById'd roots too (same el factory covers it)
const sandbox={document,console,Math,JSON,Object,Array};sandbox.window=sandbox;sandbox.globalThis=sandbox;
vm.createContext(sandbox);
try{vm.runInContext(script,sandbox,{filename:"explorer"});}catch(e){console.log("BOOT ERROR:",e.message);process.exit(1);}
let errs=0;
const walk=(node,fn)=>{fn(node);(node.children||[]).forEach(c=>walk(c,fn));};
try{
  // flip every checkbox and drag every range via their handlers
  walk(els["flags"],n=>{ if(n.onchange){n.checked=true;n.onchange();} if(n.oninput){n.value=n.max||5;n.oninput();} });
  // presets + reset
  els["preset1"].onclick(); els["preset2"].onclick(); els["preset1"].onclick();
  // select every earnable ending (preset1 = golden road: all 6)
  const clickable=[]; walk(els["endings"],n=>{if(n.onclick)clickable.push(n);});
  if(clickable.length<5) throw new Error("expected most endings earnable on preset1, got "+clickable.length);
  for(const c of clickable){ c.onclick();
    if(!els["prose"].textContent||els["prose"].textContent.length<100) throw new Error("prose empty after select");
    if(!(els["slides"].children||[]).length) throw new Error("no epilogue slides after select");
  }
  if(!(els["chron"].children||[]).length) throw new Error("chronicle empty");
}catch(e){console.log("RUNTIME ERROR:",e.message);errs++;}
console.log(errs===0?"  ✓ Explorer boots; flags/presets/sliders/ending-selection all render (prose + slides + chronicle non-empty).":"  ✗ failures above");
process.exit(errs?1:0);
