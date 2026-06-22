#!/usr/bin/env python3
"""
Bundle the entire browser suite into ONE portable, self-contained file:
play/crown_of_horns.html. Each page (the playable combat, the dialogue-tree viewer, the
endings explorer, the compendium, the cast, the flag graph, the saga map, the save
inspector, the analytics) is embedded verbatim and rendered in an isolated <iframe srcdoc>,
so their scripts/styles never collide. A tab bar switches between them; cross-page links
(e.g. the saga map → a specific conversation) are rewired to switch tabs and carry the
deep-link hash. Open the single file — everything works, no server, no other files.

Re-run after regenerating any page:
  python3 tools/make-all-in-one.py
"""
import json, os

ROOT = os.path.join(os.path.dirname(__file__), "..")
PLAY = os.path.join(ROOT, "play")

# tab key -> (source file, icon, label, blurb)
TABS = [
 ("combat",   "crown_combat.html",     "▶️", "Combat",      "Play a real tactical fight"),
 ("dialogue", "dialogue_viewer.html",  "💬", "Dialogue",    "Every branching conversation"),
 ("play",     "dialogue_sim.html",     "🎬", "Play Dialogue","Step into a conversation & choose"),
 ("endings",  "endings_explorer.html", "🎭", "Endings",     "Flip flags, watch the six endings"),
 ("compendium","compendium.html",      "📖", "Compendium",  "Grimoire · Bestiary · Codex"),
 ("cast",     "cast_gallery.html",     "🖼️", "Cast",        "Every face & profile"),
 ("flags",    "flags_explorer.html",   "🕸️", "Flag Graph",  "The saga's wiring"),
 ("saga",     "saga_map.html",         "🗺️", "Saga Map",    "The whole campaign, one page"),
 ("save",     "save_inspector.html",   "💾", "Save",        "Inspect & edit a save"),
 ("analytics","balance_report.html",   "⚖️", "Analytics",   "Monte-Carlo balance"),
]
# map source filename -> tab key, so in-bundle links switch tabs instead of breaking
FILE_TO_TAB = {src: key for key, src, *_ in TABS}

def main():
    pages = {}
    for key, src, *_ in TABS:
        p = os.path.join(PLAY, src)
        if os.path.exists(p):
            pages[key] = open(p, encoding="utf-8").read()
        else:
            pages[key] = f"<!doctype html><body style='font-family:serif;color:#999;background:#111;padding:40px'>missing: {src}</body>"
    BLOB = json.dumps(pages, ensure_ascii=False).replace("</", "<\\/")
    F2T = json.dumps(FILE_TO_TAB)
    tabbtns = "".join(
        f'<button class="tab" data-k="{k}" onclick="show(\'{k}\')"><span class="i">{ic}</span>'
        f'<span class="l">{lbl}</span></button>' for k, src, ic, lbl, bl in TABS)

    html = """<!DOCTYPE html><html lang="en"><head><meta charset="utf-8">
<meta name="viewport" content="width=device-width, initial-scale=1">
<title>Crown of Horns — Playable Compendium (all-in-one)</title>
<style>
 *{box-sizing:border-box}html,body{margin:0;height:100%}
 body{display:flex;flex-direction:column;background:#0a0910;color:#d8d2c2;
  font:14px/1.5 "Iowan Old Style","Palatino Linotype",Georgia,serif;overflow:hidden}
 header{display:flex;align-items:center;gap:14px;padding:9px 16px;border-bottom:1px solid #2a2636;
  background:linear-gradient(#15121d,#0d0b12)}
 header h1{margin:0;color:#e7c873;font-size:17px;letter-spacing:.4px;white-space:nowrap}
 header .s{color:#8d8499;font-size:11.5px;font-style:italic;white-space:nowrap;overflow:hidden;text-overflow:ellipsis}
 .tabs{display:flex;gap:3px;overflow-x:auto;padding:6px 10px;border-bottom:1px solid #2a2636;background:#0c0b11}
 .tab{flex:0 0 auto;display:flex;flex-direction:column;align-items:center;gap:1px;background:#16131e;color:#a89fb4;
  border:1px solid #241f30;border-radius:9px;padding:6px 13px;cursor:pointer;font:inherit;min-width:74px}
 .tab .i{font-size:17px;line-height:1}.tab .l{font-size:11.5px}
 .tab:hover{border-color:#5a4f74;color:#e7c873}
 .tab.on{background:linear-gradient(#2c2638,#201a2c);border-color:#e7c873;color:#e7c873}
 .stage{flex:1;position:relative;background:#0a0910}
 iframe{position:absolute;inset:0;width:100%;height:100%;border:0;background:#0a0910}
 .load{position:absolute;inset:0;display:flex;align-items:center;justify-content:center;color:#6e6680;font-style:italic}
</style></head><body>
<header>
 <h1>👑 Crown of Horns</h1>
 <span class="s">the whole game in one file — combat, dialogue trees, endings, lore &amp; more · no Unity, no server</span>
</header>
<div class="tabs">__TABS__</div>
<div class="stage">
 <div class="load" id="load">loading…</div>
 <iframe id="frame" title="Crown of Horns"></iframe>
</div>
<script>
const PAGES = __BLOB__;
const FILE_TO_TAB = __F2T__;
const frame = document.getElementById("frame");
const loadEl = document.getElementById("load");
let curHash = "";

function show(key, hash){
  document.querySelectorAll(".tab").forEach(t=>t.classList.toggle("on", t.dataset.k===key));
  curHash = hash || "";
  loadEl.style.display = "flex";
  frame.onload = ()=>{ loadEl.style.display="none"; rewire(); if(curHash) applyHash(curHash); };
  frame.srcdoc = PAGES[key] || "<body style='color:#999;background:#111'>missing</body>";
}
function applyHash(h){
  try{ frame.contentWindow.location.hash = h; }catch(e){}
}
// rewrite in-bundle links (X.html / X.html#id) to switch tabs + carry the hash
function rewire(){
  let doc; try{ doc = frame.contentDocument; }catch(e){ return; }
  if(!doc) return;
  doc.querySelectorAll('a[href]').forEach(a=>{
    const href = a.getAttribute("href") || "";
    const m = href.match(/^([a-z0-9_]+\.html)(#.*)?$/i);
    if(m && FILE_TO_TAB[m[1]]){
      a.addEventListener("click", ev=>{ ev.preventDefault();
        show(FILE_TO_TAB[m[1]], (m[2]||"").replace(/^#/,"")); }, {capture:true});
    } else if(href==="index.html"){
      a.addEventListener("click", ev=>{ ev.preventDefault(); show("combat"); }, {capture:true});
    }
  });
}
// deep-link the whole bundle via its own hash, e.g. ...crown_of_horns.html#dialogue:ilfaeril.quest.resolution
function bootFromHash(){
  const raw = decodeURIComponent(location.hash.replace(/^#/,""));
  const [tab, ...rest] = raw.split(":");
  if(tab && PAGES[tab]) show(tab, rest.join(":"));
  else show("combat");
}
window.addEventListener("hashchange", bootFromHash);
bootFromHash();
</script></body></html>"""
    out = (html.replace("__TABS__", tabbtns).replace("__BLOB__", BLOB).replace("__F2T__", F2T))
    dst = os.path.join(PLAY, "crown_of_horns.html")
    open(dst, "w", encoding="utf-8").write(out)
    print(f"wrote play/crown_of_horns.html ({len(out)//1024} KB) — {len(TABS)} tabs bundled: "
          + ", ".join(k for k,*_ in TABS))

if __name__ == "__main__":
    main()
