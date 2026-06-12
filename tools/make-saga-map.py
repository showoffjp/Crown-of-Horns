#!/usr/bin/env python3
"""
Generate play/saga_map.html — one campaign flowchart that stitches the whole suite together:
the act/era spine (the Atlas), the conversations that happen in each act (deep-linked into
the Dialogue Viewer), the flags each act's dialogue sets and the flag that gates it
(deep-linked into the Flag Graph), the companion personal-quest lane, and the Court of the
Dead fanning out to the six endings (linked into the Endings Explorer). Built from
play/dialogue-data.json + play/flags-data.json. Re-run:
  python3 tools/extract-dialogue.py && python3 tools/extract-flags.py && python3 tools/make-saga-map.py
"""
import json, os

ROOT = os.path.join(os.path.dirname(__file__), "..")
DLG = json.load(open(os.path.join(ROOT, "play", "dialogue-data.json")))
FLAGS = json.load(open(os.path.join(ROOT, "play", "flags-data.json")))

# main spine: each act buckets conversations by id prefix, and names the flag that gates the NEXT step
SPINE = [
 ("Act I–II — The Gate & the Lower City", "Sword Coast", "prologue.cleared",
  ["prologue","hub","aldric","market","docks","almshouse","safehouse","act2","lowcity"],
  "You wake Returned in Baldur's Gate. Work the Lower City, hear the Herald, take the Cinderhaunt, tea with Aldric — and recruit your first companions."),
 ("Act II — The Fugue & the Breach", "The Fugue", "act2.breach_done",
  ["fugue"],
  "Descend to the Wall of the Faithless. Pull Maerin free — and pay the Breach's price: a companion, forever."),
 ("Act III — Netheril", "Netheril", "netheril.cleared",
  ["netheril"],
  "Fall through your own broken seam into a flying empire's last hour. Naeve, the collapsing enclave, the Mythallar Colossus."),
 ("Act III — The Crown Wars", "Crown Wars", "crownwars.cleared",
  ["crownwars"],
  "The ancient court whose verdict first fed the Wall. Ilfaeril, the First Unmade, and the Verdict you can argue DOWN."),
 ("Act IV — The Time of Troubles", "Time of Troubles", "act4.toot_done",
  ["toot"],
  "The year a god dies and his skull is beaten into the Crown of Horns. The Avatar of Bone."),
 ("Act IV — The Spellplague", "Spellplague", "act4.spellplague_done",
  ["spellplague"],
  "Causality-optional blue fire, the Unmade closing in. The Herald of the Unmade — the last era before the Court."),
]
# side lanes
LANES = [
 ("Personal quests", ["garrow","roen","varra","naeve","ilfaeril"],
  "Each companion's three-conversation arc — arrival, the turn, the resolution — threaded through the eras."),
 ("The Vault & the Lady", ["lady","riddle","vault","tally"],
  "The fourth mask's riddle room, where wit beats correctness."),
]
ENDINGS = [
 ("Abolition", "Tear the Wall down — and trust what comes after.", False),
 ("The Doomguide's Peace", "Keep the oldest atrocity because Vayle made the case and you could not break it.", False),
 ("The Returned Throne", "Take the Crown yourself; rule the dead with the one soul that walked back out.", False),
 ("The Mortal Measure", "A human answer, sized to a human hand — neither god nor nothing.", False),
 ("Jergal's Keyhole", "The bored first god of the dead, asked the right question at last.", True),
 ("Break the Loop", "Step out of the story the Lady is reading — and choose to stay gone.", True),
]

def convs_for(prefixes):
    out = []
    for c in DLG["conversations"]:
        p = c["id"].split(".")[0].replace("(", "").replace(")", "")
        if p in prefixes:
            sets = set()
            for n in c["nodes"]:
                for e in n.get("onEnter", []):
                    if e.get("key"): sets.add(e["key"])
                for ch in n.get("choices", []):
                    for e in ch.get("effects", []):
                        if e.get("key"): sets.add(e["key"])
            out.append((c["id"], c["title"], len(c["nodes"]), sorted(sets)))
    return out

ENDING_FLAGS = sorted({f["key"] for f in FLAGS["flags"]
                       if any(r["source"] == "Endings" for r in f["readers"])})

def esc(s): return (str(s).replace("&", "&amp;").replace("<", "&lt;").replace(">", "&gt;"))

def conv_chip(cid, title, nodes):
    return (f'<a class="cv" href="dialogue_viewer.html#{esc(cid)}" title="{esc(cid)} — open in the Dialogue Viewer">'
            f'💬 {esc(title)} <span class="nn">{nodes}</span></a>')

def flag_chip(key, cls="fl"):
    return f'<a class="{cls}" href="flags_explorer.html#{esc(key)}" title="open in the Flag Graph">{esc(key)}</a>'

def station(idx, title, era, gate, prefixes, blurb, total):
    convs = convs_for(prefixes)
    sets = sorted({k for _, _, _, ks in convs for k in ks})
    chips = "".join(conv_chip(cid, t, n) for cid, t, n, _ in convs) or '<span class="dim">— no scripted conversations indexed —</span>'
    setline = (f'<div class="sets">sets {len(sets)} flags · ' +
               " ".join(flag_chip(k) for k in sets[:6]) +
               (f' <span class="dim">+{len(sets)-6} more</span>' if len(sets) > 6 else "") + "</div>") if sets else ""
    return f'''<div class="station">
      <div class="dot">{idx}</div>
      <div class="body">
        <div class="sh"><h3>{esc(title)}</h3><span class="era">{esc(era)}</span></div>
        <p>{esc(blurb)}</p>
        <div class="convs">{chips}</div>
        {setline}
        <div class="gate">clears via {flag_chip(gate, "fl gate")} → next era</div>
      </div></div>'''

def lane(title, prefixes, blurb):
    convs = convs_for(prefixes)
    chips = "".join(conv_chip(cid, t, n) for cid, t, n, _ in convs) or '<span class="dim">—</span>'
    return f'''<div class="lane"><h4>{esc(title)} <span class="dim">· {len(convs)} conversations</span></h4>
      <p class="dim">{esc(blurb)}</p><div class="convs">{chips}</div></div>'''

def main():
    stations = "".join(station(i+1, *s, len(SPINE)) for i, s in enumerate(SPINE))
    lanes = "".join(lane(*l) for l in LANES)
    endcards = "".join(
        f'<a class="ending{" golden" if g else ""}" href="endings_explorer.html" title="explore in the Endings Explorer">'
        f'<div class="et">{"★ " if g else ""}{esc(t)}</div><div class="ed">{esc(d)}</div></a>'
        for t, d, g in ENDINGS)
    deciders = " ".join(flag_chip(k) for k in ENDING_FLAGS)
    n_conv = len(DLG["conversations"])
    page = f'''<!DOCTYPE html><html lang="en"><head><meta charset="utf-8">
<meta name="viewport" content="width=device-width, initial-scale=1"><title>Crown of Horns — Saga Map</title>
<style>
 *{{box-sizing:border-box}}body{{margin:0;background:radial-gradient(1200px 700px at 50% -8%,#272030,#0a0910);
  color:#d8d2c2;font:14px/1.6 "Iowan Old Style","Palatino Linotype",Georgia,serif;padding:0 0 60px}}
 header{{padding:14px 26px;border-bottom:1px solid #2a2636;display:flex;align-items:baseline;gap:16px;flex-wrap:wrap;
  position:sticky;top:0;background:#0b0a10ee;backdrop-filter:blur(6px);z-index:9}}
 header h1{{margin:0;color:#e7c873;font-size:21px}}header .s{{color:#8d8499;font-size:12px;font-style:italic}}
 header a.bk{{color:#c9a24b;font-size:12px;text-decoration:none;margin-left:auto}}header a.bk:hover{{text-decoration:underline}}
 .wrap{{max-width:940px;margin:0 auto;padding:24px}}
 .spine{{position:relative;margin-left:8px;padding-left:34px;border-left:2px solid #322c44}}
 .station{{position:relative;margin:0 0 26px}}
 .dot{{position:absolute;left:-49px;top:0;width:30px;height:30px;border-radius:50%;background:#1a1626;border:2px solid #e7c873;
  color:#e7c873;font-weight:600;display:flex;align-items:center;justify-content:center;font-size:14px}}
 .body{{background:linear-gradient(#16141d,#121017);border:1px solid #2a2636;border-radius:11px;padding:13px 16px}}
 .sh{{display:flex;justify-content:space-between;align-items:baseline;gap:10px}}
 .sh h3{{margin:0;color:#e7c873;font-size:16px}}.era{{color:#8a8198;font-size:11px;white-space:nowrap}}
 .body p{{margin:6px 0 10px;color:#c4bcd0}}
 .convs{{display:flex;flex-wrap:wrap;gap:6px}}
 a.cv{{text-decoration:none;font-size:11.5px;color:#a9c4dc;background:#16202c;border:1px solid #2d4763;border-radius:13px;padding:3px 10px}}
 a.cv:hover{{border-color:#79a0c4;color:#cfe0ee}}.cv .nn{{color:#5f7e9c;font-size:10px}}
 .sets{{margin-top:9px;font-size:11px;color:#8a8198}}
 a.fl{{text-decoration:none;font-family:ui-monospace,monospace;font-size:10.5px;color:#cbb;background:#15121d;border:1px solid #2a2636;border-radius:10px;padding:1px 8px}}
 a.fl:hover{{border-color:#e7c873;color:#e7c873}}a.fl.gate{{color:#9fcf96;border-color:#2c4a32;background:#13241a}}
 .gate{{margin-top:10px;font-size:11.5px;color:#8a8198}}
 .court{{margin:6px 0 24px;text-align:center}}.court .dot2{{display:inline-flex;width:34px;height:34px}}
 .courth{{background:linear-gradient(#211b2c,#17131f);border:1px solid #3a3550;border-radius:11px;padding:14px 18px;margin-top:6px}}
 .courth h3{{margin:0;color:#e7c873;font-size:17px}}.courth p{{color:#a89fb4;margin:6px 0 0}}
 .endgrid{{display:grid;grid-template-columns:repeat(auto-fill,minmax(230px,1fr));gap:12px;margin-top:14px}}
 a.ending{{text-decoration:none;background:linear-gradient(#16141d,#121017);border:1px solid #2a2636;border-radius:10px;padding:12px 14px;display:block}}
 a.ending:hover{{border-color:#e7c873;transform:translateY(-2px)}}a.ending.golden{{border-color:#c9a24b;background:linear-gradient(#221c10,#17130a)}}
 .et{{color:#e7c873;font-size:14px;font-weight:600}}.ed{{color:#a89fb4;font-size:12px;margin-top:4px;line-height:1.45}}
 .deciders{{margin-top:12px;font-size:11px;color:#8a8198}}
 .lanes{{margin-top:10px}}.lane{{background:#120f18;border:1px solid #221f2a;border-left:3px solid #6b5fa0;border-radius:0 9px 9px 0;padding:11px 15px;margin:10px 0}}
 .lane h4{{margin:0 0 4px;color:#cbb6e0;font-size:14px}}.dim{{color:#6e6680;font-size:11px}}
 h2.sec{{color:#e7c873;font-size:14px;text-transform:uppercase;letter-spacing:1.5px;margin:26px 0 12px;border-bottom:1px solid #221d2c;padding-bottom:6px}}
 .legend{{color:#6e6680;font-size:11.5px;margin-top:8px}}
</style></head><body>
<header><h1>👑 Saga Map</h1>
 <span class="s">the whole campaign on one page — every link jumps into the matching explorer</span>
 <a class="bk" href="index.html">← back to the hub</a></header>
<div class="wrap">
 <p class="legend">The act spine of <b>Crown of Horns</b>. Each <a class="cv" href="#" onclick="return false">💬 conversation</a> opens in the Dialogue Viewer; each
  <a class="fl" href="#" onclick="return false">flag</a> opens in the Flag Dependency Graph; the endings open in the Endings Explorer. Built from the same extracted data as those pages.</p>
 <h2 class="sec">The spine — {len(SPINE)} acts across the eras</h2>
 <div class="spine">{stations}</div>
 <div class="court">
   <div class="courth"><h3>⚖️ The Court of the Dead</h3>
   <p>Every era walked, the four masks unmasked — the saga comes to rest here. Which of the six roads opens depends on what you <i>understood</i>, decided by the {len(ENDING_FLAGS)} flags the Endings engine reads.</p>
   <div class="endgrid">{endcards}</div>
   <div class="deciders">Deciding flags (open any in the Flag Graph): {deciders}</div>
   </div>
 </div>
 <h2 class="sec">Threaded through every act</h2>
 <div class="lanes">{lanes}</div>
 <p class="legend">{n_conv} conversations indexed · explore the rest from the <a class="bk" href="index.html" style="margin:0">hub</a>.</p>
</div></body></html>'''
    dst = os.path.join(ROOT, "play", "saga_map.html")
    open(dst, "w").write(page)
    print(f"wrote play/saga_map.html ({len(page)//1024} KB) — {len(SPINE)} spine acts, "
          f"{len(LANES)} lanes, {len(ENDINGS)} endings, {len(ENDING_FLAGS)} deciding flags")

if __name__ == "__main__":
    main()
