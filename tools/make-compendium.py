#!/usr/bin/env python3
"""
Build play/compendium.html — the in-world reference: Grimoire (abilities), Armory
(items), Bestiary (foes, with real combat math), and an Atlas of the eras & ending
gates. Reads play/compendium-data.json (from extract-content.py); combat numbers
mirror AttackForecast.cs (hit% = d20 faces that land vs AC; avg damage = dice mean +
mod). Enemy battle tokens are base64-thumbnailed in. Re-run:
  python3 tools/extract-content.py && python3 tools/make-compendium.py
"""
import base64, io, json, math, os, html, re
from PIL import Image

ROOT = os.path.join(os.path.dirname(__file__), "..")
DATA = json.load(open(os.path.join(ROOT, "play", "compendium-data.json")))

DMG_EMOJI = {"Slashing":"⚔️","Piercing":"\U0001F5E1️","Bludgeoning":"\U0001F528","Fire":"\U0001F525",
 "Cold":"❄️","Lightning":"⚡","Radiant":"✨","Necrotic":"\U0001F480","Acid":"\U0001F9EA",
 "Poison":"☠️","Psychic":"\U0001F300","Force":"\U0001F4A5","Thunder":"\U0001F50A"}
KIND_EMOJI = {"Weapon":"\U0001F5E1️","Armor":"\U0001F6E1️","Quest":"\U0001F5DD️","Consumable":"\U0001F9EA","Misc":"\U0001F9F3"}

def dice_avg(notation):
    m = re.match(r'\s*(\d+)d(\d+)\s*([+-]\d+)?', notation or "")
    if not m: return 0.0
    c, s, flat = int(m[1]), int(m[2]), int(m[3] or 0)
    return c * (s + 1) / 2 + flat

def hit_pct(to_hit, ac):
    return sum(1 for f in range(1, 21) if f == 20 or (f != 1 and f + to_hit >= ac)) / 20

def esc(s): return html.escape(str(s))

def thumb(name, folder="Sprites", size=(120, 120)):
    p = os.path.join(ROOT, "Assets/Resources", folder, name + ".png")
    if not os.path.exists(p): return None
    im = Image.open(p).convert("RGB"); im.thumbnail(size)
    b = io.BytesIO(); im.save(b, "JPEG", quality=82)
    return "data:image/jpeg;base64," + base64.b64encode(b.getvalue()).decode()

# ----------------------------------------------------------------- Grimoire
def grimoire():
    rows = []
    for a in sorted(DATA["abilities"], key=lambda x: (x["slot"], x["name"])):
        em = DMG_EMOJI.get(a["type"], "✨")
        avg = dice_avg(a["dice"])
        is_heal = a["name"] in ("Cure Wounds", "Healing Word", "Second Wind")
        # representative caster: +5 to hit (a competent level-3 striker)
        if a["dice"] and not is_heal:
            if a["attack"]:
                eff = f'{int(hit_pct(5,12)*100)}% / {int(hit_pct(5,15)*100)}% / {int(hit_pct(5,18)*100)}%'
                effl = "hit vs AC 12/15/18"
            else:
                eff = "save-for-half / on-fail"; effl = "Dexterity/Wis save"
            dmg = f'~{avg:g} {a["type"].lower()}'
        elif is_heal:
            eff, effl, dmg = "always", "no roll", f'~{avg:g} healing'
        else:
            eff, effl, dmg = "utility", "applies a condition", "—"
        rng = "melee" if a["range"] <= 1 else f'{a["range"]} tiles'
        rows.append(f'''<tr><td class="ico">{em}</td><td><b>{esc(a["name"])}</b><div class="dim">{esc(a["kind"])} · {rng}</div></td>
          <td>{esc(a["dice"] or "—")}</td><td>{dmg}</td><td>{eff}<div class="dim">{effl}</div></td><td class="dim">{esc(a["era"])}</td></tr>''')
    return f'''<table><tr><th></th><th>Ability</th><th>Dice</th><th>Effect</th><th>To hit</th><th>From</th></tr>{"".join(rows)}</table>'''

# ----------------------------------------------------------------- Armory
def armory():
    rows = []
    for it in sorted(DATA["items"], key=lambda x: (x["kind"], x["name"])):
        em = KIND_EMOJI.get(it["kind"], "\U0001F9F3")
        if it.get("weaponDamage"):
            stat = f'{it["weaponDamage"]} {DMG_EMOJI.get(it.get("wType") or "", "")} {(it.get("wType") or "").lower()}'
        elif it.get("ac"):
            stat = f'+{it["ac"]} AC'
        else:
            stat = "—"
        val = (it.get("value") and f'{it["value"]} gp') or "—"
        rows.append(f'''<tr><td class="ico">{em}</td><td><b>{esc(it["name"])}</b></td><td>{esc(it["kind"])}</td>
          <td>{esc(it["slot"]).replace("None","—")}</td><td>{stat}</td><td class="dim">{val}</td></tr>''')
    return f'''<table><tr><th></th><th>Item</th><th>Kind</th><th>Slot</th><th>Stat</th><th>Value</th></tr>{"".join(rows)}</table>'''

# ----------------------------------------------------------------- Bestiary
def threat_tier(xp):
    return ("☠ Boss", "#e0a0a0") if xp >= 350 else \
           ("▲▲▲ Elite", "#e7c873") if xp >= 200 else \
           ("▲▲ Standard", "#b8b0c4") if xp >= 100 else ("▲ Minor", "#8a8198")
def bestiary():
    cards = []
    for e in sorted(DATA["enemies"], key=lambda x: (x["era"], -x["xp"])):
        prof = 2 + (e["level"] - 1) // 4
        strmod = (e["str"] - 10) // 2
        to_hit = strmod + prof
        tier, col = threat_tier(e["xp"])
        img = thumb(e["name"]) or thumb("Enemy")
        imgtag = f'<img src="{img}">' if img else '<div class="noimg">☠</div>'
        cards.append(f'''<div class="mon" data-era="{esc(e["era"])}">
          {imgtag}
          <div class="mn">{esc(e["name"])}</div>
          <div class="tier" style="color:{col}">{tier}</div>
          <div class="ms"><span>Lv {e["level"]}</span><span>~AC {e["acApprox"]}</span><span>+{to_hit} hit</span><span>{e["xp"]} XP</span></div>
          <div class="dim">{esc(e["era"])} · {esc(e["weapon"]).replace("_"," ")}</div></div>''')
    eras = sorted({e["era"] for e in DATA["enemies"]})
    chips = '<button class="chip on" data-f="all">All</button>' + "".join(
        f'<button class="chip" data-f="{esc(x)}">{esc(x)}</button>' for x in eras)
    return f'<div class="filters">{chips}</div><div class="mongrid">{"".join(cards)}</div>'

# ----------------------------------------------------------------- Conditions
COND_ICON = {"Poisoned":"☠️","Prone":"\U0001F938","Stunned":"\U0001F4AB","Incapacitated":"\U0001F6AB",
 "Restrained":"\U0001F578️","Blinded":"\U0001F441️","Frightened":"\U0001F628","Charmed":"\U0001F49E",
 "Burning":"\U0001F525","Blessed":"✨","Hasted":"\U0001F3C3","Slowed":"\U0001F40C"}
def effect_mechanics(e):
    """Translate the StatusEffectDefinition fields into the game's own plain English (from its tooltips)."""
    out = []
    if e["dotDice"]: out.append((f'{e["dotDice"]} {(e["dotType"] or "").lower()} damage at the start of each of its turns', "dot"))
    if e["incapacitates"]: out.append(("Cannot take actions or move", "bad"))
    if e["attackersAdvantage"]: out.append(("Attacks against it have advantage", "bad"))
    if e["bearerDisadvantage"]: out.append(("Its own attacks have disadvantage", "bad"))
    if e["attackRollMod"]: out.append((f'{e["attackRollMod"]:+d} to its attack rolls', "good" if e["attackRollMod"] > 0 else "bad"))
    if e["armorClassMod"]: out.append((f'{e["armorClassMod"]:+d} Armor Class', "good" if e["armorClassMod"] > 0 else "bad"))
    if e["speedMod"]: out.append((f'{e["speedMod"]:+d} movement tiles', "good" if e["speedMod"] > 0 else "bad"))
    if not out: out.append(("Flavour / vocabulary only — no mechanical change", "dim"))
    return out
def conditions():
    cards = []
    for e in DATA["effects"]:
        ico = COND_ICON.get(e["name"], COND_ICON.get(e["condition"], "•"))
        kind = "beneficial" if e["beneficial"] else "affliction"
        kcol = "#7fbf76" if e["beneficial"] else "#e0a0a0"
        mech = "".join(f'<li class="{c}">{esc(t)}</li>' for t, c in effect_mechanics(e))
        cond = "" if e["condition"] in ("None", e["name"]) else f' · maps to <i>{esc(e["condition"])}</i>'
        cards.append(f'''<div class="eff">
          <div class="effh"><span class="ei">{ico}</span><b>{esc(e["name"])}</b>
            <span class="ek" style="color:{kcol}">{kind}</span></div>
          <div class="dim">lasts {e["rounds"]} round{"s" if e["rounds"]!=1 else ""}{cond}</div>
          <ul class="mech">{mech}</ul></div>''')
    # the full Condition vocabulary, noting which have an authored effect
    authored = {e["condition"] for e in DATA["effects"]} | {e["name"] for e in DATA["effects"]}
    chips = "".join(
        f'<span class="cchip{" live" if c["name"] in authored else ""}">{COND_ICON.get(c["name"],"•")} {esc(c["name"])}'
        f'{(" — " + esc(c["note"])) if c["note"] else ""}</span>'
        for c in DATA["conditions"])
    return (f'<div class="effgrid">{"".join(cards)}</div>'
            f'<h3 class="glh">The condition vocabulary</h3>'
            f'<p class="dim">The 5e-style conditions the engine understands. Lit chips have an authored effect above; '
            f'the rest are reserved for content. Mechanics are read straight from each effect\'s fields.</p>'
            f'<div class="cvocab">{chips}</div>')

# ----------------------------------------------------------------- Codex
CAT_ORDER = ["Premise", "Masks", "Companions", "Bestiary", "Lore"]
CAT_LABEL = {"Premise":"\U0001F56F️ Premise","Masks":"\U0001F3AD The Four Masks","Companions":"\U0001F6E1️ The Company",
 "Bestiary":"☠ Bestiary","Lore":"\U0001F4DC Lore & History"}
def codex():
    cats = sorted({c["category"] for c in DATA["codex"]}, key=lambda x: CAT_ORDER.index(x) if x in CAT_ORDER else 99)
    chips = '<button class="chip on" data-cf="all">All</button>' + "".join(
        f'<button class="chip" data-cf="{esc(c)}">{esc(CAT_LABEL.get(c,c))}</button>' for c in cats)
    cards = []
    for e in DATA["codex"]:
        flag = (f'<span class="unlock">unlocks: <code>{esc(e["unlockFlag"])}</code></span>'
                if e["unlockFlag"] else '<span class="unlock known">known from the start</span>')
        cards.append(f'''<div class="cdx" data-cat="{esc(e["category"])}">
          <div class="cdxh"><b>{esc(e["title"])}</b><span class="catpip">{esc(CAT_LABEL.get(e["category"],e["category"]))}</span></div>
          <p>{esc(e["body"])}</p>{flag}</div>''')
    return f'<div class="filters">{chips}</div><div class="cdxgrid">{"".join(cards)}</div>'

# ----------------------------------------------------------------- Atlas
ATLAS = [
 ("Act I — The Gate", "Sword Coast", "You wake Returned in Baldur's Gate. Hear the Herald, take the Cinderhaunt, and clear the prologue — the Wall's first whisper. Recruit Sister Garrow, Roen, Varra.", "prologue.cleared"),
 ("Act II — The Fugue", "The Fugue", "Descend to the Wall of the Faithless. Maerin waits, half-unmade. Pulling her free costs a companion, forever — the Breach, the bill love runs up.", "(the permanent loss)"),
 ("Act III — Netheril", "Netheril", "The first rift: a flying empire's last hour. Naeve, the falling city, the Mythallar Colossus. Clears → the Crown Wars open.", "netheril.cleared"),
 ("Act III — Crown Wars", "Crown Wars", "The ancient court whose verdict first fed the Wall. Ilfaeril, the First Unmade, and the Verdict you can argue DOWN.", "crownwars.cleared / verdict_spared"),
 ("Act IV — Time of Troubles", "Time of Troubles", "The year a god dies and his skull is beaten into the Crown of Horns — the relic Aldric carries. The Avatar of Bone.", "act4.toot_done"),
 ("Act IV — Spellplague", "Spellplague", "Causality-optional blue fire, the Unmade closing in. The Herald of the Unmade. The last era before the Court.", "act4.spellplague_done"),
 ("Finale — The Court of the Dead", "The Court", "Six endings, gated by what you understood: Abolition, Doomguide's Peace, the Returned Throne, Mortal Measure — and two GOLDEN roads, Jergal's Keyhole and Break the Loop.", "netheril.cleared → the Court"),
]
def atlas():
    return "".join(
        f'''<div class="act"><div class="acth"><h3>{esc(t)}</h3><span class="era-tag">{esc(era)}</span></div>
        <p>{esc(desc)}</p><div class="gate">unlocks via <code>{esc(flag)}</code></div></div>'''
        for t, era, desc, flag in ATLAS)

# ----------------------------------------------------------------- assemble
def main():
    page = """<!DOCTYPE html><html lang="en"><head><meta charset="utf-8">
<meta name="viewport" content="width=device-width, initial-scale=1"><title>Crown of Horns - Compendium</title>
<style>*{box-sizing:border-box}body{margin:0;background:radial-gradient(1100px 600px at 50% -10%,#241f30,#0c0b10);
color:#d8d2c2;font:14px/1.5 Georgia,serif;padding:0 0 40px}
header{padding:16px 26px;border-bottom:1px solid #2a2636;position:sticky;top:0;background:#0c0b10ee;backdrop-filter:blur(6px);z-index:5}
h1{color:#e7c873;font-size:23px;margin:0}.sub{color:#8a8198;font-size:12px;font-style:italic}
.tabs{display:flex;gap:6px;margin-top:12px;flex-wrap:wrap}
.tab{font:inherit;font-size:13px;background:#181620;color:#c9a24b;border:1px solid #2a2636;border-radius:7px;padding:7px 14px;cursor:pointer}
.tab.on{background:linear-gradient(#2e2838,#211b2c);border-color:#e7c873;color:#e7c873}
.links a{color:#8a8198;font-size:12px;margin-left:14px;text-decoration:none}.links a:hover{color:#e7c873}
.wrap{padding:18px 26px}.panel{display:none}.panel.on{display:block}
table{border-collapse:collapse;width:100%}td,th{padding:7px 10px;text-align:left;border-bottom:1px solid #1c1a24;vertical-align:top}
th{color:#8a8198;font-weight:400;font-size:11px;letter-spacing:1px;text-transform:uppercase}
tr:hover td{background:#14121b}.ico{font-size:18px}.dim{color:#6e6680;font-size:11px}
b{color:#e7c873;font-weight:600}
.filters{margin-bottom:14px;display:flex;gap:6px;flex-wrap:wrap}
.chip{font:inherit;font-size:12px;background:#181620;color:#b8b0c4;border:1px solid #2a2636;border-radius:14px;padding:4px 12px;cursor:pointer}
.chip.on{border-color:#e7c873;color:#e7c873}
.mongrid{display:grid;grid-template-columns:repeat(auto-fill,minmax(150px,1fr));gap:12px}
.mon{background:linear-gradient(#16141d,#131119);border:1px solid #2a2636;border-radius:9px;padding:11px;text-align:center}
.mon img{width:84px;height:84px;object-fit:cover;border-radius:7px;background:#0f0d15}.noimg{font-size:40px}
.mn{color:#e7c873;font-size:13px;margin-top:7px;font-weight:600;min-height:32px}.tier{font-size:11px;margin:2px 0 6px}
.ms{display:flex;flex-wrap:wrap;gap:4px 8px;justify-content:center;font-size:11px;color:#b8b0c4}
.act{background:linear-gradient(#16141d,#131119);border:1px solid #2a2636;border-left:3px solid #c9a24b;border-radius:0 9px 9px 0;padding:12px 16px;margin:10px 0}
.acth{display:flex;justify-content:space-between;align-items:baseline}.act h3{color:#e7c873;margin:0;font-size:17px}
.era-tag{color:#8a8198;font-size:11px}.act p{line-height:1.6;margin:8px 0}.gate{font-size:11px;color:#8a8198}code{color:#c9a24b}
.count{color:#6e6680;font-size:12px;margin-bottom:10px}
.effgrid{display:grid;grid-template-columns:repeat(auto-fill,minmax(260px,1fr));gap:12px}
.eff{background:linear-gradient(#16141d,#131119);border:1px solid #2a2636;border-radius:9px;padding:12px 14px}
.effh{display:flex;align-items:center;gap:8px}.ei{font-size:20px}.ek{margin-left:auto;font-size:11px;text-transform:uppercase;letter-spacing:1px}
.mech{margin:8px 0 0;padding-left:18px}.mech li{margin:3px 0;font-size:12.5px}
.mech li.bad{color:#e0a0a0}.mech li.good{color:#7fbf76}.mech li.dot{color:#f0b46a}.mech li.dim{color:#6e6680;list-style:none;margin-left:-14px}
.glh{color:#e7c873;font-size:16px;margin:22px 0 2px}
.cvocab{display:flex;flex-wrap:wrap;gap:7px;margin-top:8px}
.cchip{font-size:12px;background:#141119;color:#6e6680;border:1px solid #221f2a;border-radius:13px;padding:4px 11px}
.cchip.live{color:#d8d2c2;border-color:#3a3550;background:#1b1826}
.cdxgrid{display:grid;grid-template-columns:repeat(auto-fill,minmax(300px,1fr));gap:12px}
.cdx{background:linear-gradient(#16141d,#131119);border:1px solid #2a2636;border-radius:9px;padding:13px 15px}
.cdxh{display:flex;justify-content:space-between;align-items:baseline;gap:8px}
.catpip{color:#7b7388;font-size:10.5px;white-space:nowrap}
.cdx p{line-height:1.6;margin:8px 0;color:#cfc7d8;font-size:13px}
.unlock{font-size:10.5px;color:#7b7388}.unlock.known{color:#6f9e6a}
</style></head><body>
<header><h1>Crown of Horns - Compendium</h1>
<span class="sub">The in-world reference, generated from the game's actual content. Combat numbers mirror the verified AttackForecast.</span>
<div class="tabs">
 <button class="tab on" data-t="grim">\U0001F4D6 Grimoire</button>
 <button class="tab" data-t="arm">⚔️ Armory</button>
 <button class="tab" data-t="best">☠ Bestiary</button>
 <button class="tab" data-t="cond">\U0001F300 Conditions</button>
 <button class="tab" data-t="codex">\U0001F4D3 Codex</button>
 <button class="tab" data-t="atlas">\U0001F5FA️ Atlas</button>
 <span class="links">also: <a href="crown_combat.html">▶ Combat demo</a><a href="endings_explorer.html">\U0001F3AD Endings explorer</a><a href="dialogue_viewer.html">\U0001F4AC Dialogue</a><a href="cast_gallery.html">\U0001F5BC️ Cast gallery</a></span>
</div></header>
<div class="wrap">
 <div class="panel on" id="grim"><div class="count">""" + str(len(DATA["abilities"])) + """ abilities — weapons, cantrips, spells & maneuvers. To-hit shown for a +5 striker.</div>""" + grimoire() + """</div>
 <div class="panel" id="arm"><div class="count">""" + str(len(DATA["items"])) + """ items in the world's loot tables.</div>""" + armory() + """</div>
 <div class="panel" id="best"><div class="count">""" + str(len(DATA["enemies"])) + """ foes across six eras. ~AC and to-hit computed as the engine does (base 13-14 + Dex, Str mod + proficiency).</div>""" + bestiary() + """</div>
 <div class="panel" id="cond"><div class="count">""" + str(len(DATA["effects"])) + """ authored status effects and the """ + str(len(DATA["conditions"])) + """-condition vocabulary. Mechanics read straight from each StatusEffectDefinition.</div>""" + conditions() + """</div>
 <div class="panel" id="codex"><div class="count">""" + str(len(DATA["codex"])) + """ Codex entries — the lore journal that fills in as you witness the saga. Each carries the flag that unlocks it in-game.</div>""" + codex() + """</div>
 <div class="panel" id="atlas"><div class="count">The shape of a playthrough — acts, eras, and the flags that gate them.</div>""" + atlas() + """</div>
</div>
<script>
document.querySelectorAll(".tab").forEach(function(b){b.onclick=function(){
 document.querySelectorAll(".tab").forEach(t=>t.classList.remove("on"));
 document.querySelectorAll(".panel").forEach(p=>p.classList.remove("on"));
 b.classList.add("on");document.getElementById(b.dataset.t).classList.add("on");};});
document.querySelectorAll(".chip[data-f]").forEach(function(c){c.onclick=function(){
 document.querySelectorAll(".chip[data-f]").forEach(x=>x.classList.remove("on"));c.classList.add("on");
 var f=c.dataset.f;document.querySelectorAll(".mon").forEach(function(m){
   m.style.display=(f==="all"||m.dataset.era===f)?"":"none";});};});
document.querySelectorAll(".chip[data-cf]").forEach(function(c){c.onclick=function(){
 document.querySelectorAll(".chip[data-cf]").forEach(x=>x.classList.remove("on"));c.classList.add("on");
 var f=c.dataset.cf;document.querySelectorAll(".cdx").forEach(function(m){
   m.style.display=(f==="all"||m.dataset.cat===f)?"":"none";});};});
</script></body></html>"""
    dst = os.path.join(ROOT, "play", "compendium.html")
    open(dst, "w").write(page)
    print(f"wrote play/compendium.html ({len(page)//1024} KB) - "
          f"{len(DATA['abilities'])} abilities, {len(DATA['items'])} items, {len(DATA['enemies'])} foes")

if __name__ == "__main__":
    main()
