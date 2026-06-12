#!/usr/bin/env python3
"""Build play/cast_gallery.html — every face, base64-thumbnailed into one standalone
page, with CLICKABLE full character profiles for the playable cast & principals.
Re-run after regenerating art: python3 tools/make-cast-gallery.py"""
import base64, glob, io, json, os, html
from PIL import Image

ROOT = os.path.join(os.path.dirname(__file__), "..")
COMPANIONS = ["Sister Garrow","Roen Alleywind","Varra","Naeve","Ilfaeril","Maerin"]
PRINCIPALS = ["The Returned","Aldric Morn","Mhaere","Sable","Tamsin","Quill","Wrenna Alleywind","Mother Cass",
              "High Lord Aelryth","Justiciar Veld","The Pale Cantor"]

# ------------------------------------------------------------------ profiles
# Authored from the game's content (class/race/stats from the build functions;
# arcs/quests/bonds from EndingResolver, the era content, and the codex).
PROFILES = {
 "The Returned": {"role":"The protagonist — the Player Character","tag":"A soul too stubborn to stay erased.",
  "stats":[("Class","Yours to choose at creation"),("Race","Yours"),("Origin","The Wall of the Faithless")],
  "bio":["You died Faithless — owing no god, claimed by none — and were filed into the Wall of the Faithless to dissolve. You refused. You are the Returned: pulled back into a body and a world that has no place for you, carrying the one thing the Wall is built to erase — a *self*.",
   "The dead of the Realms are kept by an old machinery: judgement, the Doom, and the Wall that swallows the unclaimed. You are the crack in it. Every power in the story — Kelemvor's Doomguides who keep the Wall, the Faithless Choir who would tear it down, the gentle heretic Aldric Morn who carries the Crown of Horns — wants to know what you are.",
   "Across five ages you will learn the truth the loop keeps failing on, and at the Court of the Dead you will choose: free every discarded soul at any cost, keep the cruel order, rewrite the Law by hand, take the Crown yourself — or say your true name and stay gone, so the next Returned never has to."],
  "quest":"The whole game. Six endings, two of them golden.","romance":"You are the one others may love.",
  "bonds":"Everyone orbits you."},

 "Sister Garrow": {"role":"Companion · Cleric (Warpriest of Kelemvor)","tag":"She buried everyone, and believed in keeping no one.",
  "stats":[("Class","Cleric"),("Faith","Kelemvor, god of the dead"),("Weapon","Warpriest's Maul"),("Joins","From the very start")],
  "bio":["A grey-robed Doomguide who has laid more dead to rest than she can count, Garrow does the work the church forgets to: she sits with the dying, she names the unclaimed, she keeps the Doom by hand. She is dry, unsentimental, and quietly furious at a clergy that stopped reading its own canon.",
   "She is the one companion who can credibly defend the Wall — order against the abyss — which is exactly why the game gives her to you first. Watch her wrestle with what you are: a Faithless thing that should be filed away, walking around with a soul.",
   "Her quest, A God-Shaped Hole, puts Kelemvor's own doctrine on trial. She can win it and become the first High Doomguide of a kinder Law, set down the grey clasp and lay the dead under no law but her own two hands, or kneel and keep the grey — a smaller, harder faith, carried brittle."],
  "quest":"A God-Shaped Hole — doctrine won / faith left / recanted.","romance":"Yes. The woman who kept no one learns to keep one.",
  "bonds":"Witnessed a god beaten into the Crown at the Time of Troubles forge."},

 "Roen Alleywind": {"role":"Companion · Rogue (Harper agent)","tag":"Came out of the gutter; never forgot it.",
  "stats":[("Class","Rogue"),("Affiliation","The Harpers"),("Weapon","Twin Rapiers"),("Joins","Baldur's Gate hub")],
  "bio":["An Outer City orphan turned charming, deniable Harper blade — all jokes and lockpicks over a bottomless loyalty he'd rather you didn't notice. Roen reads a room in a heartbeat and trusts almost no one, which makes the people he does let in the whole story of his life.",
   "His quest, The Honest Lie, is his sister Wrenna — a Harper who turned, and whom Roen must choose whether to save, turn back, or hand over. Mercy, the rules, or something cleverer.",
   "Romance him and he finally gets his 'after': an inn with a terrible name, two chairs, and a retired Harper who cheats at cards and loses on purpose."],
  "quest":"The Honest Lie — double agent / saved by mercy / given to the Harpers.","romance":"Yes — the menace who can't believe you stayed.",
  "bonds":"Sister: Wrenna Alleywind. Stood in the Almshouse he could have died in."},

 "Varra": {"role":"Companion · Rogue/Warlock (Tiefling)","tag":"Her soul has been on the books since she was six.",
  "stats":[("Class","Rogue / pact-warlock"),("Race","Tiefling"),("Patron","A Hell that owns her"),("Weapon","Pact-fire & dagger")],
  "bio":["Violet-flamed and grinning, Varra signed away her soul before she understood the words, and has spent twenty years being magnificent in spite of it. She trades in leverage and never once trusts a thing she can't read the fine print on — except, eventually, you.",
   "Her quest, The Bill Comes Due, is the contract itself: find the patron's true name and turn the leash around, take her debt onto your own name, or help her burn the contract with her own fire — still doomed, but on her own clock.",
   "She is the cruelest stake in the game: if you love her, the Wall can take her at the Breach as the tithe for Maerin — the receipt collected before you ever get to burn it."],
  "quest":"The Bill Comes Due — patron bound / debt taken / contract burned.","romance":"Yes — the first transaction of her life with no fine print.",
  "bonds":"Walked the Spellplague's blue fire and chose, every time, to come back out — for you."},

 "Naeve": {"role":"Companion · Wizard (Arcanist of Netheril)","tag":"Still trying to redo the proof of her own life.",
  "stats":[("Class","Wizard / Arcanist"),("Era","Netheril, the Seventh Enclave"),("Lost","An entire civilization"),("Joins","The Netheril rift")],
  "bio":["The last living mind of a flying empire that fell ten thousand years ago, Naeve survived the Sky's fall and has spent every year since failing to grieve it — re-deriving, again and again, the equation by which her people might not have died. Precise, haunted, and a thousand years lonely.",
   "Her quest, After the Sky Fell, is what she does with the very last live Weave of Netheril: let it out as a seed for the present world, let it finish its thousand-year fall, or seal it in stasis — a snowflake caught forever mid-fall.",
   "She is the heart of the golden Jergal's Keyhole ending: the two of you at the Ledger, deriving the rewritten Law of the dead together, one soul at a time, forever."],
  "quest":"After the Sky Fell — rekindled / released / preserved.","romance":"Yes — two people who each ended a world, allowed to be happy anyway.",
  "bonds":"Witnessed the Seventh Enclave in its last living hour — and did not fall alone."},

 "Ilfaeril": {"role":"Companion · Fighter (penitent paladin, High Elf)","tag":"Ten thousand years a penitent; once, a paladin.",
  "stats":[("Class","Fighter"),("Race","High Elf"),("Level","4 — older than your bloodline"),("Joins","The Crown Wars rift")],
  "bio":["At the ancient elven court of the Crown Wars, Ilfaeril raised his hand and helped vote a whole people into damnation. He has spent the ten thousand years since guiding the lost past the Wall in penance, a rigid, sorrowing thing that has not let itself rest once.",
   "His quest, The Vote, is whether he can hear the forgiveness offered by Maerith — a survivor of the people he damned — for what it is: not a door, but a sword held hilt-first. He can take up that sword and tear a hole in the Wall for the rest of her people, let himself be forgiven, or keep the weight as the last true thing he has of her.",
   "He is the game's credible voice that order is sometimes built by people who were not wrong — and the proof that even that can be set down."],
  "quest":"The Vote — commission / forgiven / penance kept.","romance":"No — his heart is ten thousand years elsewhere.",
  "bonds":"Returned to the third bench from the dais where he raised his hand — and did not look away alone."},

 "Maerin": {"role":"Companion · Ranger","tag":"The one in the Wall. Freeing her costs you everything.",
  "stats":[("Class","Ranger"),("Found","Inside the Wall of the Faithless"),("Level","3"),("The price","A companion, forever")],
  "bio":["Maerin waits in the grey of the Fugue, half-dissolved into the Wall of the Faithless — a soul being slowly unmade, held together only by being witnessed. To pull her free is the most human thing you can do, and the most expensive: the Wall takes a tithe, and at the Breach it will collect a companion you cannot get back.",
   "She is the moral engine of the whole bargain: love runs up a bill the story collects before you can pay it down. Whether you reach for her — and whom the Wall takes in exchange — is the choice the rest of the game is shaped around.",
   "There is no revival in this story, and no softening it. The hole where the tithe stood is exactly the shape of everything you were just beginning to let yourself want."],
  "quest":"The Breach — and its permanent, unrevivable cost.","romance":"No — she is the price, not the prize.",
  "bonds":"Her freedom is paid for in someone else's name."},

 # ---- principals ----
 "Aldric Morn": {"role":"The gentle heretic","tag":"A grieving father who carries a god's skull on his shelf.",
  "stats":[("Carries","The Crown of Horns"),("Was","A father"),("Wants","His daughter back")],
  "bio":["Over tea, Aldric Morn is the kindest man you will meet in the Gate — soft-spoken, generous, ruinously sad. He is also carrying the Crown of Horns, the relic beaten from a dead god's skull, and he believes it will give him back the daughter death took.",
   "He is not a villain so much as the door the Crown was built to walk through: love the size of his is exactly the lever. You can name him a monster to his face (and watch him agree, which is worse), see only the grieving father, or make him say the count of what his arithmetic costs.",
   "How you read Aldric in Act I is named back to you an age later, at the forge where the Crown was made."],
  "quest":"Drives the central question of the game.","romance":"No.","bonds":"The Crown wears him as much as he wears it."},

 "High Lord Aelryth": {"role":"The Verdict (Crown Wars)","tag":"He damned a valley of souls because it was correct.",
  "stats":[("Era","The Crown Wars"),("Office","High Lord of the court")],
  "bio":["Aelryth presides over the ancient court whose verdict first fed the Wall — the original sin the whole machinery of the dead is built on. He is not cruel; he is reasonable, which is how the Wall always gets built: one defensible decision at a time.",
   "The game's central moral-hazard scene is his: you can argue the damnation down, sparing a single valley of souls ten thousand years early. It changes one thing. One thing is not nothing."],
  "quest":"The Verdict — argued down, or passed.","romance":"No.","bonds":"His afternoon echoes into every later age."},

 "Justiciar Veld": {"role":"Doomguide of Kelemvor","tag":"The church that keeps the Wall.",
  "stats":[("Order","The Doomguides of Kelemvor"),("Keeps","The Doom, and the Wall")],
  "bio":["A hard, honest keeper of the dead's order, Veld speaks for the institution that maintains the Wall of the Faithless — and that cannot decide whether you are an enemy to be put down or a question it can neither damn nor dismiss.",
   "Where your reputation with the Doomguides lands decides whether the church meets you at the Court with a drawn blade or an empty chair kept for your argument."],
  "quest":"Faction: Kelemvor's Doomguides.","romance":"No.","bonds":"The order will argue about the Returned for a thousand years."},

 "Wrenna Alleywind": {"role":"Roen's sister · Harper","tag":"The reason Roen's jokes have a floor under them.",
  "stats":[("Affiliation","The Harpers — turned"),("Kin","Roen's sister")],
  "bio":["Wrenna turned on the Harpers, and Roen's whole quest is what you do about it. Save her, turn her back into a blade in Kelemvor's own house, or hand her in — each leaves a different set of cold teacups on a different mantel.",
   "She is the proof that Roen, for all the deflection, has exactly one thing he cannot be flippant about."],
  "quest":"Central to Roen's 'The Honest Lie'.","romance":"No.","bonds":"Brother: Roen Alleywind."},

 "Mother Cass": {"role":"The Almshouse of the Unclaimed","tag":"Keeps a backwards Kelemvor token for the poor's dead.",
  "stats":[("Runs","The Almshouse"),("Speaks for","The unclaimed")],
  "bio":["In the Lower City, Mother Cass tends the dead nobody will claim — the exact souls the Wall is built to swallow. She can send you to the Court of the Dead carrying the poor's token, scales turned inward, so that for once the unclaimed have someone in the room who promised to say they were real.",
   "Carry her token to the end and it matters that you said it."],
  "quest":"The Almshouse token.","romance":"No.","bonds":"Speaks for everyone the story would otherwise forget."},
}

def thumb_b64(p, size=(176,220)):
    im = Image.open(p).convert("RGB"); im.thumbnail(size)
    buf = io.BytesIO(); im.save(buf, "JPEG", quality=84); return base64.b64encode(buf.getvalue()).decode()

def card(name, img_path, sub=""):
    has = name in PROFILES
    cls = "card" + (" has-profile" if has else "")
    attr = f' data-id="{html.escape(name)}"' if has else ""
    badge = '<div class="more">&#9656; profile</div>' if has else ""
    return (f'<div class="{cls}"{attr}><img src="data:image/jpeg;base64,{thumb_b64(img_path)}" alt="{html.escape(name)}">'
            f'<div class="nm">{html.escape(name)}</div>'
            + (f'<div class="sub">{html.escape(sub)}</div>' if sub else "") + badge + "</div>")

def main():
    P = lambda n: os.path.join(ROOT, "Assets/Resources/Portraits", n + ".png")
    sections = []; total = 0

    comp = [card(n, P(n), "companion") for n in COMPANIONS if os.path.exists(P(n))]
    sections.append(("The Company", comp))
    npcs = [card(n, P(n)) for n in PRINCIPALS if os.path.exists(P(n))]
    shown = set(COMPANIONS) | set(PRINCIPALS)
    for p in sorted(glob.glob(os.path.join(ROOT, "Assets/Resources/Portraits/*.png"))):
        n = os.path.basename(p)[:-4]
        if n not in shown and n not in ("NPC", "Roen", "Garrow"): npcs.append(card(n, p))
    sections.append(("Faces of the Gate & the Eras", npcs))
    foes = []; skip = shown | {"Player","Ally","Neutral","Enemy","Garrow","Quill, the Broker"}
    for p in sorted(glob.glob(os.path.join(ROOT, "Assets/Resources/Sprites/*.png"))):
        n = os.path.basename(p)[:-4]
        if n not in skip: foes.append(card(n, p, "battle token"))
    sections.append(("The Rogues' Gallery (battle tokens)", foes))

    body = ""
    for title, cards in sections:
        total += len(cards)
        body += f'<h2>{title} <span class="ct">({len(cards)})</span></h2><div class="grid">' + "".join(cards) + "</div>"

    PROF_JSON = json.dumps({k: {**v, "stats": v.get("stats", [])} for k, v in PROFILES.items()})
    out = """<!DOCTYPE html><html lang="en"><head><meta charset="utf-8">
<meta name="viewport" content="width=device-width, initial-scale=1"><title>Crown of Horns - Cast Gallery</title>
<style>*{box-sizing:border-box}body{margin:0;background:radial-gradient(1100px 600px at 50% -10%,#241f30,#0c0b10);
color:#d8d2c2;font:14px/1.5 Georgia,serif;padding:18px 26px}
h1{color:#e7c873;font-size:24px;margin:6px 0 2px}.subt{color:#8a8198;font-size:12px;font-style:italic;max-width:760px}
h2{color:#c9a24b;font-size:14px;letter-spacing:1.5px;text-transform:uppercase;margin:26px 0 10px;border-bottom:1px solid #2a2636;padding-bottom:6px}.ct{color:#6e6680;font-weight:400}
.grid{display:grid;grid-template-columns:repeat(auto-fill,minmax(132px,1fr));gap:12px}
.card{position:relative;background:linear-gradient(#16141d,#131119);border:1px solid #2a2636;border-radius:8px;padding:10px;text-align:center;transition:.12s}
.card.has-profile{cursor:pointer}.card.has-profile:hover{border-color:#e7c873;transform:translateY(-2px);box-shadow:0 8px 22px #0007}
.card img{width:100%;border-radius:5px;background:#0f0d15}
.nm{margin-top:7px;font-size:12.5px;color:#e7c873}.sub{font-size:10.5px;color:#8a8198}
.more{margin-top:4px;font-size:10px;color:#c9a24b;opacity:.85}
.modal{position:fixed;inset:0;background:#0a0910ee;display:none;align-items:center;justify-content:center;padding:24px;z-index:20}
.sheet{display:flex;gap:20px;max-width:880px;max-height:90vh;overflow:auto;background:linear-gradient(#181620,#100e16);
border:1px solid #3a3550;border-radius:12px;padding:22px;box-shadow:0 20px 80px #000}
.sheet img{width:240px;border-radius:8px;align-self:flex-start;border:1px solid #2a2636}
.info{flex:1;min-width:300px}.info h3{margin:0;color:#e7c873;font-size:24px}
.role{color:#c9a24b;font-size:13px;letter-spacing:.5px;margin:2px 0}.tag{color:#b9a8e0;font-style:italic;margin:0 0 12px}
.stats{display:flex;flex-wrap:wrap;gap:6px;margin:10px 0 14px}
.stat{background:#14121b;border:1px solid #2a2636;border-radius:5px;padding:4px 9px;font-size:12px}
.stat b{color:#8a8198;font-weight:400}.stat span{color:#e7c873}
.info p{line-height:1.65;color:#d8d2c2;margin:8px 0}
.field{margin-top:12px;font-size:13px}.field .k{color:#c9a24b;letter-spacing:1px;text-transform:uppercase;font-size:11px}
.x{position:fixed;top:18px;right:24px;font-size:30px;color:#8a8198;cursor:pointer}.x:hover{color:#e7c873}
.hint{color:#c9a24b}.foot{margin-top:26px;color:#6e6680;font-size:11.5px}
@media(max-width:640px){.sheet{flex-direction:column}.sheet img{width:160px;align-self:center}}</style></head><body>
<h1>Crown of Horns - Cast Gallery</h1>
<div class="subt">Every face in the game - """ + str(total) + """ era-tinted portraits &amp; battle tokens. <b class="hint">Click any companion or principal (&#9656; profile) for a full character sheet.</b> Portraits are honest placeholders - the exact files the engine loads by name.</div>
""" + body + """
<div class="modal" id="modal"><div class="x" id="x">&times;</div><div class="sheet" id="sheet"></div></div>
<div class="foot">Profiles authored from the game's content (class/stats from the build functions; arcs from EndingResolver &amp; the era scripts). Generated by tools/make-cast-gallery.py.</div>
<script>
const PROFILES=""" + PROF_JSON + """;
const IMG={};
document.querySelectorAll(".card.has-profile").forEach(function(c){
  IMG[c.dataset.id]=c.querySelector("img").src;
  c.onclick=function(){openP(c.dataset.id);};
});
function esc(s){return String(s).replace(/&/g,"&amp;").replace(/</g,"&lt;");}
function openP(id){
  var p=PROFILES[id];if(!p)return;
  var stats=(p.stats||[]).map(function(s){return '<div class="stat"><b>'+esc(s[0])+':</b> <span>'+esc(s[1])+'</span></div>';}).join("");
  var bio=(p.bio||[]).map(function(t){return '<p>'+esc(t)+'</p>';}).join("");
  var field=function(k,v){return v?'<div class="field"><div class="k">'+k+'</div>'+esc(v)+'</div>':"";};
  document.getElementById("sheet").innerHTML=
   '<img src="'+(IMG[id]||"")+'" alt="'+esc(id)+'">'+
   '<div class="info"><h3>'+esc(id)+'</h3><div class="role">'+esc(p.role||"")+'</div><div class="tag">'+esc(p.tag||"")+'</div>'+
   '<div class="stats">'+stats+'</div>'+bio+
   field("Personal quest",p.quest)+field("Romance",p.romance)+field("Bonds",p.bonds)+'</div>';
  document.getElementById("modal").style.display="flex";
}
function close_(){document.getElementById("modal").style.display="none";}
document.getElementById("x").onclick=close_;
document.getElementById("modal").onclick=function(e){if(e.target.id==="modal")close_();};
document.addEventListener("keydown",function(e){if(e.key==="Escape")close_();});
</script></body></html>"""
    open(os.path.join(ROOT, "play", "cast_gallery.html"), "w").write(out)
    print(f"wrote play/cast_gallery.html ({len(out)//1024} KB, {total} cards, {len(PROFILES)} clickable profiles)")

if __name__ == "__main__":
    main()
