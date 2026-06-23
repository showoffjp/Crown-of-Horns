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
REED = json.load(open(os.path.join(ROOT, "play", "reed-walk.json"), encoding="utf-8"))
MODEL = DEMO["characterModel"]

BUILDS = [
    {"name": "The Returned", "cls": "Fighter", "scores": [16, 14, 15, 10, 12, 13],
     "race": "Human", "gender": "Male", "background": "Soldier", "law": "Lawful", "morality": "Neutral", "deity": "Kelemvor",
     "blurb": "A plain human soldier of the Judge."},
    {"name": "The Scholar", "cls": "Wizard", "scores": [10, 14, 12, 16, 14, 11],
     "race": "Half-Elf", "gender": "Female", "background": "Sage", "law": "Neutral", "morality": "Good", "deity": "Oghma",
     "blurb": "Half-elf sage — sharp INSIGHT & lore."},
    {"name": "The Confessor", "cls": "Cleric", "scores": [13, 10, 14, 11, 17, 12],
     "race": "Human", "gender": "Female", "background": "Acolyte", "law": "Lawful", "morality": "Good", "deity": "Kelemvor",
     "blurb": "A cleric of Kelemvor — the best WISDOM."},
    {"name": "The Silver Tongue", "cls": "Rogue", "scores": [10, 16, 12, 13, 11, 16],
     "race": "Tiefling", "gender": "Male", "background": "Charlatan", "law": "Chaotic", "morality": "Neutral", "deity": "Tymora",
     "blurb": "A tiefling charlatan — the best CHARISMA."},
    {"name": "The Warden", "cls": "Ranger", "scores": [14, 16, 13, 11, 14, 10],
     "race": "Elf", "gender": "Female", "background": "Folk Hero", "law": "Neutral", "morality": "Neutral", "deity": "None",
     "blurb": "An elf who serves no god — one of the Faithless."},
]

INT_LABELS = {
    "market.sable.regard": "Mother Sable's regard",
    "market.bram.regard": "Sergeant Bram's regard",
    "market.pip.regard": "Pip's trust in you",
    "market.wren.regard": "Wren's trust in you",
    "reed.cassian.regard": "Old Cassian's regard",
    "reed.vharn.regard": "Sister Vharn's regard",
    "reed.wren.regard": "Wren's trust in you",
    "reed.reedwife.regard": "the Reed-Wife's regard",
}

# Lore glossary (shared with the dialogue sim) — common-knowledge hover + tiered 5e passive lore reveals.
GLOSSARY = json.load(open(os.path.join(ROOT, "play", "lore-glossary.json"), encoding="utf-8"))

# The Returned-sense: an eerie, on-theme perception the dead-touched player gets on first sight of each soul,
# if their Wisdom-clarity (10 + WIS mod) meets the moment's DC. It reveals what the living can't see.
NPC_SENSE = {
    "market.sable": {"dc": 12, "text": "*(Your dead-touched senses sweep her table and recoil — there is no holiness in a single bone or thread of it. And yet her hands, folding the knots, are warm with something realer than any relic: she has buried more of this ward than the gravediggers have, and grieved every one of them into the thread she sells. The fraud is the kindest thing in this market.)*"},
    "market.bram": {"dc": 11, "text": "*(The Wall left its chill in your marrow, and it answers the cold this man carries: three squads' worth of the recently, violently dead are pressed against his back like wet wool, patient, unblaming. He does not know they followed him home from the causeway. You do. They are not here for revenge. They are here to see whether he stops sending more.)*"},
    "market.pip": {"dc": 13, "text": "*(Some Returned see the almost-dead the way the living see candle-flames. The girl burns bright and quick and furious — but there is a second flame tethered to hers, fainter, somewhere close and low to the ground, and it is *guttering.* A child, smaller than this one. Sick past what a stolen copper mends. The thing she is really stealing for, running out of wick.)*"},
    "market.calix": {"dc": 12, "text": "*(Your dead-touched senses reach the young priest and recoil from what's already begun in him: the Wall's cold has crept up past his knees, grey and patient, the way frost takes a windowpane from the edges in. He has stood the causeway too long. A man becomes what he guards, if he guards it without love — and Calix has run clean out of love for his post. Another year of watching the Faithless go in, and he'll be standing on the wrong side of his own Wall, and not remember crossing.)*"},
    "market.wren": {"dc": 11, "text": "*(You don't have to look for the mark. It *finds* you — cold light pouring off her like breath off a winter river, a brand the harvesters laid on her soul that the living can't see and she can only feel. Days. Fewer than she's letting herself count. She is godless and gentle and entirely without sin, and she has been measured for a wall, and she is buying apples because the apples are sweet and the apples are *now.*)*"},
    "market.tallow": {"dc": 13, "text": "*(You reach for the smiling man's soul the way you'd reach into a coat you expect to be full — and your senses close on *almost nothing.* He has spent himself a coin at a time, a name at a time, year on year, until what's left would barely fill a thimble. He thinks his tithes and his service have bought him safety. They've only hollowed him faster. The Wall is not reaching for him later, with the rest. It is reaching for him *now*, and the cold has already found his fingers — he simply mistakes it for the draught off the river.)*"},
    "market.joss": {"dc": 10, "text": "*(You barely have to reach at all; the madman's soul is *standing open*, like a door left wide in winter. He has been to the edge of the Wall and pressed his face to it and listened, and a sliver of the cold came home in him and never left — the same sliver that lives in you. He is not raving at nothing. He is raving at the *exact* thing you are. Of every soul in this square, he is the one who will believe every word you'd never dare say aloud — because he already heard it, in the stone, in a voice he loved.)*"},
    "reed.cassian": {"dc": 11, "text": "*(The river's cold is your cold, and it answers the old boatman's like a tuning-fork: he is not quite among the living anymore. Forty years of ferrying has worn him thin as a coin rubbed smooth, half his soul already paid downstream to the water he serves. He does not know he is dying by inches into his own river. He thinks it is just the damp in his knees. The oar in his hands has rowed thirteen thousand souls to the mist, and the thirteen-thousand-and-first will be him, and he has no idea, and some part of him is so very tired that it would be a mercy.)*"},
    "reed.vharn": {"dc": 14, "text": "*(You reach for the Measurer's soul and find it walled — not hollow like Tallow's, but *fortified*, every doubt bricked up behind liturgy, a cell she built around her own conscience and threw away the key to. And yet the cold gets in. There, behind the third course of bricks: three hundred and eleven faces she will not let herself count, and one of them, freshest, has an apple in its hand. The wall around her heart is exactly as strong as the Wall she serves — which is to say, it holds against everyone but the one soul stubborn enough to keep saying a name.)*"},
    "reed.wren": {"dc": 10, "text": "*(The mark blazes off her even brighter here, this close to the stone — but your dead-touched sense reads something the brand can't hide: she is not ready. Whatever she tells herself, whatever brave arithmetic she's done in the cold water, the living animal of her is *clinging*, white-knuckled, to every small sweet now it can find — the apple, the cold on her ankles, the sound of your boots on the shingle. She came here to end it. Her own soul is screaming, in a voice only you can hear, that it is not time, it is not time, it is not time.)*"},
    "reed.reedwife": {"dc": 9, "text": "*(You don't reach for her; she is already the same cold as you, and recognition passes between you like current through water. Four hundred years she has abided in the doorway you walked all the way through — a soul that refused the Wall and let the river catch it on the threshold, neither in nor back, just *held*, in the green and the patience. She is lonelier than anything you have ever touched. She is also, you realise with a small private vertigo, what you might have been, had you flinched at the door. There but for the turning-around go you.)*"},
}
for _c in MKT["conversations"] + REED["conversations"]:
    if _c["id"] in NPC_SENSE:
        _c["returned"] = NPC_SENSE[_c["id"]]

ALL_CONVS = MKT["conversations"] + REED["conversations"]
EMBED = {"scene": MKT["scene"], "scenes": {MKT["scene"]["id"]: MKT["scene"], REED["scene"]["id"]: REED["scene"]},
         "conversations": ALL_CONVS, "model": MODEL, "glossary": GLOSSARY}
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
 .gloss{border-bottom:1px dotted #6a5a9a;cursor:help;color:inherit}
 .gloss:hover{color:#cbb8f0;border-bottom-color:#cbb8f0}
 #tip{position:fixed;z-index:99;max-width:300px;background:linear-gradient(#1a1626,#13101c);border:1px solid #4a4368;border-radius:9px;
   padding:10px 12px;font-size:12.5px;line-height:1.5;color:#cfc6dc;box-shadow:0 10px 34px #000b;pointer-events:none;display:none}
 #tip .tn{color:#e0b8f0;font-weight:600;font-size:13px;margin-bottom:3px}
 #tip .tl{margin-top:7px;padding-top:7px;border-top:1px solid #2e2940;color:#c8d8b8;font-style:italic}
 #tip .tl b{color:#9fe0b0;font-style:normal}
 .lore{margin:2px 0 4px;padding:8px 11px;border-left:3px solid #5a4a8a;background:#171327;border-radius:0 8px 8px 0;font-size:13px;color:#c6bcda}
 .lore .lk{color:#b08fe0;font-weight:600;letter-spacing:.3px;font-size:11px}
 .lore .deep{margin-top:7px;padding-top:7px;border-top:1px solid #3a2f55;color:#e0cba0}
 .sense{margin:2px 0 6px;padding:9px 12px;border-left:3px solid #3a6a8a;background:#0f1922;border-radius:0 8px 8px 0;font-size:13.5px;color:#a8c8d8;font-style:italic}
 .sense .lk{color:#7fc8e0;font-weight:600;font-style:normal;letter-spacing:.3px;font-size:11px;display:block;margin-bottom:3px}
 .reckon{display:flex;align-items:center;gap:7px;margin:4px 0;font-size:12px}
 .reckon .nm{width:78px}.reckon .pips{flex:1;letter-spacing:1px}
 .reckon .rk{font-variant-numeric:tabular-nums;color:#8a8198}
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
 .tg.disp{color:#d8b0e8;border-color:#5a3a6a;background:#1f1726}
 .tg.returned{color:#cbb8f0;border-color:#5a4a8a;background:#1a1626}
 .tg.gender{color:#e0a8c0;border-color:#6a3a52;background:#241622}
 .breadth{margin-top:10px;padding:8px 11px;border:1px dashed #4a4368;border-radius:8px;font-size:11.5px;color:#9a90b4;background:#15121d}
 .breadth b{color:#cbb8f0}
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
 .d20.nat20{background:radial-gradient(circle at 40% 35%,#5a4a1a,#2a2410);border-color:#e7c873;color:#ffe9a8;box-shadow:0 0 16px #e7c87388}
 .d20.nat1{background:radial-gradient(circle at 40% 35%,#4a1a1a,#2a1010);border-color:#d06f6f;color:#ffc0c0;box-shadow:0 0 16px #b03a3a88}
 .dice .res.crit{color:#ffe9a8;text-shadow:0 0 10px #e7c87399;animation:critpop .4s ease}
 .dice .res.fumble{color:#ffb0b0;text-shadow:0 0 10px #d06f6f99;animation:critpop .4s ease}
 @keyframes critpop{0%{transform:scale(.6)}60%{transform:scale(1.25)}100%{transform:scale(1)}}
 .d20wrap{position:relative;width:58px;height:58px;flex:0 0 auto;perspective:320px}
 .d20face{width:100%;height:100%;transform-style:preserve-3d;will-change:transform}
 .d20face svg{width:100%;height:100%;display:block;filter:drop-shadow(0 3px 5px #000a)}
 .dnum{position:absolute;inset:0;display:flex;align-items:center;justify-content:center;font:700 19px "Iowan Old Style",Georgia,serif;color:#1a1424;text-shadow:0 1px 0 #ffffff66;pointer-events:none}
 .d20face.rolling{animation:tumble .42s linear infinite}
 @keyframes tumble{0%{transform:rotateX(8deg) rotateY(0) rotateZ(0)}100%{transform:rotateX(368deg) rotateY(360deg) rotateZ(120deg)}}
 .d20face.land{animation:dland .66s cubic-bezier(.2,1.4,.35,1)}
 @keyframes dland{0%{transform:translateY(-16px) scale(1.35) rotateZ(26deg)}40%{transform:translateY(2px) scale(.9) rotateZ(-6deg)}70%{transform:translateY(-2px) scale(1.04) rotateZ(2deg)}100%{transform:translateY(0) scale(1) rotateZ(0)}}
 .d20wrap.nat20 .d20face svg{filter:drop-shadow(0 0 11px #e7c873dd) brightness(1.18) saturate(1.3)}
 .d20wrap.nat20::after{content:"";position:absolute;inset:-9px;border-radius:50%;border:2px solid #e7c87377;animation:dburst .65s ease-out forwards;pointer-events:none}
 @keyframes dburst{0%{transform:scale(.5);opacity:1}100%{transform:scale(1.7);opacity:0}}
 .d20wrap.nat1 .d20face svg{filter:drop-shadow(0 0 11px #d06f6fcc) saturate(.5) brightness(.82)}
 .d20wrap.nat1{animation:dshudder .5s ease}
 @keyframes dshudder{0%,100%{transform:translateX(0)}20%{transform:translateX(-3px)}40%{transform:translateX(3px)}60%{transform:translateX(-2px)}80%{transform:translateX(2px)}}
 .rollprev{background:linear-gradient(#1a1626,#14111d);border:1px solid #4a4368;border-radius:11px;padding:13px 15px;margin:2px 0;animation:fade .25s ease}
 .rp-head{font-size:13px;color:#e7c873;font-weight:600;letter-spacing:.4px;margin-bottom:9px}.rp-head span{color:#9a90b4;font-weight:400;font-size:11.5px}
 .rp-calc{font-size:13px;color:#cfc6dc;margin-bottom:7px;display:flex;gap:7px;align-items:center;flex-wrap:wrap}
 .rp-die{background:#221d2e;border:1px solid #4a4368;border-radius:6px;padding:2px 8px;font-weight:600;color:#cbb8f0}
 .rp-comp{font-size:11.5px;background:#181421;border:1px solid #2e2940;border-radius:5px;padding:2px 7px;color:#b9a8e0}.rp-comp.prof{color:#f0c890;border-color:#6a4f2a}.rp-comp.dim{color:#6e6680}
 .rp-tot{color:#e7c873;font-size:14px}
 .rp-need{font-size:13px;color:#d8d2c2;margin-bottom:8px}.rp-need b{color:#ffe9a8}
 .rp-bar{position:relative;height:18px;background:#181421;border:1px solid #2e2940;border-radius:9px;overflow:hidden;margin-bottom:8px}
 .rp-bar .rp-fill{position:absolute;left:0;top:0;bottom:0;border-radius:9px}.rp-fill.ok{background:linear-gradient(#3a6a4a,#6fd08a)}.rp-fill.bad{background:linear-gradient(#7a3a3a,#d06f6f)}
 .rp-bar span{position:absolute;inset:0;display:flex;align-items:center;justify-content:center;font-size:11px;color:#f0ead8;font-weight:600;text-shadow:0 1px 2px #0008}
 .rp-crit{font-size:11px;color:#9a90b4;margin-bottom:10px}.rp-crit b{color:#cbb8f0}
 .rp-roll{width:100%;background:linear-gradient(#f0d27e,#d8af4a);color:#1a1622;border:0;border-radius:9px;padding:11px;font:inherit;font-size:15px;font-weight:700;letter-spacing:.5px;cursor:pointer;transition:.12s}
 .rp-roll:hover{filter:brightness(1.08);transform:translateY(-1px);box-shadow:0 6px 18px #e7c87344}
 @media(max-width:980px){.wrap{flex-direction:column;align-items:center}.side{width:min(760px,96vw)}}
</style></head><body>
<header>
 <h1>👑 The Market of the Causeway</h1>
 <span class="sub">two walkable zones · eleven souls · the way they answer depends on who you are — and what you did</span>
 <a class="home" href="index.html">← all previews</a>
</header>
<div class="wrap">
 <div class="scenecol">
  <div style="display:flex;align-items:center;gap:8px"><span style="color:#7f8aa0;font-size:11px;letter-spacing:1.2px;text-transform:uppercase">▸ now in</span><span id="zonename" style="color:#bcd6ec;font-size:14px;font-style:italic">The Market of the Causeway</span></div>
  <canvas id="board" width="760" height="470"></canvas>
  <div class="hint" id="hint"><b>Click</b> the ground to walk. Approach an NPC and <b>click them</b> (or press <b>E</b>) to talk. Walk onto a glowing <b style="color:#bcd6ec">∙ causeway tile</b> to cross to the next zone — what you did in one carries to the other. Change <b>who you are</b> on the right, then talk again — they notice.</div>
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
   <h3>Your reckoning <span style="float:right;font-weight:400;text-transform:none;letter-spacing:0;color:#6e6680">who you're becoming</span></h3>
   <div id="reckon"><div class="empty">Your choices haven't tilted you yet.</div></div>
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
<div id="tip"></div>
<script>
const DATA = __BLOB__;
const BUILDS = __BUILDS__;
const INT_LABELS = __INTLABELS__;
let SCENE = DATA.scene;   // the active zone (swapped by loadScene when you cross an exit)
const SCENES = DATA.scenes || {market:DATA.scene}, CONVS = DATA.conversations, MODEL = DATA.model, GLOSS = DATA.glossary;
const ABILS = ["Strength","Dexterity","Constitution","Intelligence","Wisdom","Charisma"];
const ABBR  = {Strength:"STR",Dexterity:"DEX",Constitution:"CON",Intelligence:"INT",Wisdom:"WIS",Charisma:"CHA"};
// Pillars-of-Eternity-style dispositions — your choices accrue a reckoning that persists across the visit
// and can unlock later lines (gated via when.int on disp.* keys).
const DISP = {
  "disp.merciful":{name:"Merciful",hue:140}, "disp.ruthless":{name:"Ruthless",hue:0},
  "disp.honest":{name:"Honest",hue:200}, "disp.cunning":{name:"Cunning",hue:280},
  "disp.devout":{name:"Devout",hue:48}, "disp.heretical":{name:"Heretical",hue:320},
  "disp.stoic":{name:"Stoic",hue:210}, "disp.haunted":{name:"Haunted",hue:265}
};
function roman(n){ return ["","I","II","III","IV","V","VI","VII","VIII","IX","X"][Math.min(10,Math.abs(n))]||(""+n); }
// A faceted d20, drawn procedurally (no external art) — tumbles in 3D, bounces on landing.
const D20_SVG = '<svg viewBox="0 0 100 100" aria-hidden="true"><g stroke="#2e2940" stroke-width="1.3" stroke-linejoin="round">'+
  '<polygon points="11,27.5 50,5 89,27.5 50,24" fill="#b9a8e0"/>'+
  '<polygon points="89,27.5 89,72.5 73,66 50,24" fill="#7a6aa8"/>'+
  '<polygon points="11,27.5 50,24 27,66 11,72.5" fill="#6a5a9a"/>'+
  '<polygon points="11,72.5 27,66 73,66 89,72.5 50,95" fill="#473c6e"/>'+
  '<polygon points="50,24 27,66 73,66" fill="#d4c4f4"/></g></svg>';
// glossary lookup tables for inline linking (longest term first so "the Wall of the Faithless" beats "the Wall")
const GLOSS_INDEX = (function(){ const a=[]; GLOSS.forEach((e,i)=>{ [e.term].concat(e.aka||[]).forEach(t=>a.push({t:t,i:i})); });
  a.sort((x,y)=>y.t.length-x.t.length); return a; })();
function escRe(s){ return s.replace(/[.*+?^${}()|[\]\\]/g,"\\$&"); }
const GLOSS_RE = new RegExp("\\b("+GLOSS_INDEX.map(o=>escRe(o.t)).join("|")+")\\b","i");

/*<MKT>*/
// Pure resolution — mirrors Assets/Scripts/Dialogue/DialogueRunner.cs + Abilities.cs.
function abilityMod(score){ return Math.floor((score - 10) / 2); }
function resolveCheck(roll, dc, mod){ return (roll + mod) >= dc; }
// BG3/5e crits: a natural 20 auto-succeeds (no matter the DC), a natural 1 auto-fails (no matter the bonus).
function rollResult(raw, dc, mod){
  if(raw===20) return { success:true,  crit:true,  fumble:false };
  if(raw===1)  return { success:false, crit:false, fumble:true  };
  return { success:(raw+mod)>=dc, crit:false, fumble:false };
}
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
  if(when.gender!==undefined && !i(when.gender,char.gender)) return false;
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
// BG3/5e: knowledge & awareness skills are PASSIVE — they auto-succeed when 10+mod(+prof) >= DC, and the
// option simply doesn't appear otherwise. The social "attempt" skills are ACTIVE — you roll for those.
var PASSIVE_SKILLS=["Perception","Insight","Investigation","Arcana","History","Religion","Nature","Medicine","Survival","Animal Handling"];
function isPassiveSkill(s){ return !!s && PASSIVE_SKILLS.indexOf(s)>=0; }
function checkOf(ch){ return ch.check || (ch.checkDC ? {skill:(ch.checkSkill||null), ability:ch.checkAbility, dc:ch.checkDC} : null); }
function passiveScore(char, check, model){ return 10 + checkBonus(char, check, model); }   // 10 + ability mod (+ proficiency)
function passiveBeats(char, check, model){ return passiveScore(char, check, model) >= check.dc; }
// BG3-style pre-roll breakdown — everything the player sees BEFORE committing to the roll.
function rollBreakdown(char, check, model){
  const idx=["Strength","Dexterity","Constitution","Intelligence","Wisdom","Charisma"].indexOf(check.ability);
  const amod=abilityMod(char.scores[idx]);
  const prof=!!(check.skill && isProficient(char, check.skill, model));
  const pbonus=prof?(model.proficiencyBonus||0):0;
  const bonus=amod+pbonus, need=check.dc-bonus;
  return { bonus:bonus, abilityMod:amod, prof:prof, profBonus:pbonus, need:need,
    needShown:Math.max(2,Math.min(20,need)), chance:chanceToPass(check.dc,bonus),
    onlyNat20:need>20, onlyNat1:need<=1 };
}
function choiceAvailable(char, state, ch, model){
  if(!matchesWhen(char, state, ch.when)) return false;                                   // identity/stat gate: ruled out if unmet
  const chk=checkOf(ch);
  if(chk && isPassiveSkill(chk.skill) && !passiveBeats(char, chk, model)) return false;  // passive skill: only shown if you'd already pass
  return true; }
// LORE (5e passive knowledge): which glossary terms appear in a line, and whether you'd recall the deep lore.
function glossaryHits(text, glossary){ const t=(text||"").toLowerCase(), hits=[];
  for(var i=0;i<glossary.length;i++){ var names=[glossary[i].term].concat(glossary[i].aka||[]);
    for(var j=0;j<names.length;j++){ if(t.indexOf(names[j].toLowerCase())>=0){ hits.push(i); break; } } }
  return hits; }
function loreKnown(char, entry, model){ if(!entry||!entry.skill) return false;
  return passiveBeats(char, {skill:entry.skill, ability:model.skillAbility[entry.skill], dc:entry.dc||10}, model); }
function secretKnown(char, entry, model){ if(!entry||!entry.secret) return false;
  return passiveBeats(char, {skill:entry.secretSkill, ability:model.skillAbility[entry.secretSkill], dc:entry.secretDc||18}, model); }
// THE RETURNED: a soul pulled back from the Wall perceives what the living can't — its clarity scales with
// Wisdom (the soul's lucidity). Innate; never proficiency. 10 + WIS modifier vs the moment's sense-DC.
function returnedClarity(char){ return 10 + abilityMod(char.scores[4]); }
// ---- grid collision + A* pathfinding (so you can't walk through people, stalls, or the fountain) ----
function tileKey(x,y){ return x+","+y; }
function buildBlocked(scene){ const b={}; (scene.props||[]).forEach(p=>{ b[tileKey(p.x,p.y)]=1; });
  (scene.npcs||[]).forEach(n=>{ b[tileKey(n.x,n.y)]=1; }); return b; }
function inBounds(x,y,w,h){ return x>=0&&x<w&&y>=0&&y<h; }
function findPath(sx,sy,gx,gy,blocked,w,h){   // 8-dir A*, no corner-cutting; returns [[x,y]…] from first step to goal, [] if none
  if(sx===gx&&sy===gy) return [];
  if(!inBounds(gx,gy,w,h)||blocked[tileKey(gx,gy)]) return [];
  const dirs=[[1,0],[-1,0],[0,1],[0,-1],[1,1],[1,-1],[-1,1],[-1,-1]];
  const open=[{x:sx,y:sy,g:0,f:0,p:null}], best={}; best[tileKey(sx,sy)]=0;
  while(open.length){
    let bi=0; for(let i=1;i<open.length;i++) if(open[i].f<open[bi].f) bi=i;
    const cur=open.splice(bi,1)[0];
    if(cur.x===gx&&cur.y===gy){ const path=[]; let n=cur; while(n.p){ path.unshift([n.x,n.y]); n=n.p; } return path; }
    for(const d of dirs){ const nx=cur.x+d[0], ny=cur.y+d[1]; if(!inBounds(nx,ny,w,h)||blocked[tileKey(nx,ny)]) continue;
      if(d[0]&&d[1] && (blocked[tileKey(cur.x+d[0],cur.y)]||blocked[tileKey(cur.x,cur.y+d[1])])) continue;   // no corner cut
      const ng=cur.g+(d[0]&&d[1]?1.414:1), k=tileKey(nx,ny);
      if(best[k]!==undefined && best[k]<=ng+1e-6) continue; best[k]=ng;
      open.push({x:nx,y:ny,g:ng,f:ng+Math.max(Math.abs(nx-gx),Math.abs(ny-gy)),p:cur}); } }
  return [];
}
function nearestFreeAdjacent(sx,sy,gx,gy,blocked,w,h){   // the reachable free tile beside a target (e.g. an NPC) closest to you
  const dirs=[[1,0],[-1,0],[0,1],[0,-1],[1,1],[1,-1],[-1,1],[-1,-1]]; let best=null,bestLen=1e9;
  for(const d of dirs){ const nx=gx+d[0], ny=gy+d[1]; if(!inBounds(nx,ny,w,h)||blocked[tileKey(nx,ny)]) continue;
    if(nx===sx&&ny===sy) return [nx,ny];
    const path=findPath(sx,sy,nx,ny,blocked,w,h); if(path.length && path.length<bestLen){ bestLen=path.length; best=[nx,ny]; } }
  return best;
}
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
function field2key(f){ return {Race:"race",Gender:"gender",Class:"cls",Background:"background",Deity:"deity"}[f]; }
function setField(f,v){ char[f]=v; char.name="Custom"; char.blurb=""; buildIdx=-1; renderBuilds(); }
function setAlign(v){ const[l,m]=v.split(" "); char.law=l; char.morality=m; char.name="Custom"; char.blurb=""; buildIdx=-1; renderBuilds(); }
function bump(i,d){ char.scores[i]=Math.max(1,Math.min(20,char.scores[i]+d)); char.name="Custom"; char.blurb=""; buildIdx=-1; renderBuilds(); }
function renderWho(){
  const sel=(f,opts,val)=>`<div><label>${f.toUpperCase()}</label><select onchange="setField('${field2key(f)}',this.value)">`+opts.map(o=>`<option${o===val?' selected':''}>${esc(o)}</option>`).join("")+`</select></div>`;
  const align=`<div><label>ALIGNMENT</label><select onchange="setAlign(this.value)">`+MODEL.laws.flatMap(l=>MODEL.moralities.map(m=>{const v=l+" "+m;return `<option${(char.law+" "+char.morality)===v?' selected':''}>${esc(v)}</option>`;})).join("")+`</select></div>`;
  document.getElementById("whoGrid").innerHTML=sel("Race",MODEL.races,char.race)+sel("Gender",MODEL.genders,char.gender)+sel("Class",MODEL.classes,char.cls)+sel("Background",MODEL.backgrounds,char.background)+sel("Deity",MODEL.deities,char.deity)+align;
}
function renderAbil(){ document.getElementById("abil").innerHTML=ABILS.map((a,i)=>{const m=abilityMod(char.scores[i]),dlg=(a==="Intelligence"||a==="Wisdom"||a==="Charisma");
  return `<div class="ab${dlg?' dlg':''}"><div class="k">${ABBR[a]}</div><div class="v">${char.scores[i]}</div><div class="m">${m>=0?'+':''}${m}</div><div class="step"><button onclick="bump(${i},-1)">▼</button><button onclick="bump(${i},1)">▲</button></div></div>`;}).join(""); }
function renderProf(){ const cp=(MODEL.classProficiencies[char.cls]||[]),bp=(MODEL.backgroundProficiencies[char.background]||[]),all=[...new Set([...cp,...bp])].sort();
  document.getElementById("profLine").innerHTML=`Proficient (+${MODEL.proficiencyBonus}) in: `+(all.length?all.map(s=>`<b>${esc(s)}</b>`).join(", "):"—")+`. INT · WIS · CHA drive the checks.`; }

// ---- persistent visit state ----
let st = newState();
function renderState(){
  // regard (NPC opinion of you) — disp.* lives in the Reckoning panel instead
  const apps=[]; Object.keys(st.ints).forEach(k=>{ if(k.indexOf("disp.")===0) return; apps.push({name:INT_LABELS[k]||k, v:st.ints[k]}); });
  const ap=document.getElementById("approvals");
  ap.innerHTML = !apps.length ? `<div class="empty">You haven't spoken to anyone yet.</div>` :
    apps.sort((a,b)=>Math.abs(b.v)-Math.abs(a.v)).map(a=>{const w=Math.min(50,Math.abs(a.v)*6),neg=a.v<0;
      return `<div class="appr"><div class="nm">${esc(a.name)}</div><div class="barwrap"><div class="bar${neg?' neg':''}" style="width:${w}%;${neg?'right:50%;left:auto;':''}"></div></div><div class="delta ${a.v>=0?'up':'down'}">${a.v>=0?'+':''}${a.v}</div></div>`;}).join("");
  // reckoning (Pillars-style dispositions)
  const disp=Object.keys(st.ints).filter(k=>k.indexOf("disp.")===0&&st.ints[k]>0).map(k=>({k,name:(DISP[k]||{}).name||k,hue:(DISP[k]||{}).hue||0,v:st.ints[k]}));
  const rk=document.getElementById("reckon");
  rk.innerHTML = !disp.length ? `<div class="empty">Your choices haven't tilted you yet.</div>` :
    disp.sort((a,b)=>b.v-a.v).map(d=>`<div class="reckon"><div class="nm" style="color:hsl(${d.hue} 50% 70%)">${esc(d.name)}</div><div class="pips" style="color:hsl(${d.hue} 50% 64%)">${"◆".repeat(Math.min(6,d.v))}</div><div class="rk">${roman(d.v)}</div></div>`).join("");
  const keys=Object.keys(st.bools), fl=document.getElementById("flags");
  fl.innerHTML = !keys.length ? `<div class="empty">Nothing yet — your choices leave a mark here.</div>` :
    keys.sort().map(k=>{const dot=k.indexOf("."),dom=dot<0?'':k.slice(0,dot)+'·';return `<span class="flagpill"><span class="d">${esc(dom)}</span>${esc(dot<0?k:k.slice(dot+1))}</span>`;}).join("");
}

// ====================== the market scene (canvas) ======================
const cv=document.getElementById("board"), ctx=cv.getContext("2d");
function normScene(s){ if(s._normed) return s; (s.props||[]).forEach(p=>{ p.tx=p.x; p.ty=p.y; }); (s.npcs||[]).forEach(n=>{ n.tx=n.x; n.ty=n.y; }); (s.exits||[]).forEach(x=>{ x.tx=x.x; x.ty=x.y; }); s._normed=true; return s; }
Object.values(SCENES).forEach(normScene); normScene(SCENE);   // every zone authored in x/y; the engine draws in tx/ty
const TW=64, TH=32, OX=cv.width/2, OY=64;
function iso(tx,ty){ return { x: OX+(tx-ty)*TW/2, y: OY+(tx+ty)*TH/2 }; }
function unIso(sx,sy){ const a=(sx-OX)/(TW/2), b=(sy-OY)/(TH/2); return { tx:(a+b)/2, ty:(b-a)/2 }; }
const player={ tx:SCENE.playerStart.x, ty:SCENE.playerStart.y, path:null, pathIdx:0, facing:1 };
let BLOCKED=buildBlocked(SCENE);
let hoverTile=null, hoverNpc=null, nearNpc=null, hoverExit=null, autoTalk=null, traveling=false, fade=0, lastT=0;
function exitAt(tx,ty){ return (SCENE.exits||[]).find(x=>x.tx===tx&&x.ty===ty)||null; }
// ---- zone travel: walk onto a glowing causeway tile and the world changes around you ----
function loadScene(id, dest){ const s=SCENES[id]; if(!s) return; SCENE=normScene(s); BLOCKED=buildBlocked(SCENE);
  const d=dest||SCENE.playerStart; player.tx=d.x; player.ty=d.y; player.path=null; player.pathIdx=0;
  hoverTile=hoverNpc=hoverExit=nearNpc=autoTalk=null;
  const zb=document.getElementById("zonename"); if(zb) zb.textContent=SCENE.name||"";
  sfx('talk'); }
function travelTo(ex){ if(traveling) return; traveling=true; fade=0;
  const tick=()=>{ fade=Math.min(1,fade+0.08); if(fade<1){ requestAnimationFrame(tick); }
    else { loadScene(ex.to, ex.dest); fade=1; const out=()=>{ fade=Math.max(0,fade-0.07); if(fade>0) requestAnimationFrame(out); else traveling=false; }; requestAnimationFrame(out); } };
  requestAnimationFrame(tick); }

function npcAt(id){ return SCENE.npcs.find(n=>n.id===id); }
function dist(ax,ay,bx,by){ return Math.hypot(ax-bx,ay-by); }
function curTile(){ return [Math.round(player.tx), Math.round(player.ty)]; }
function walkTo(gx,gy){ const c=curTile(); const p=findPath(c[0],c[1],gx,gy,BLOCKED,SCENE.w,SCENE.h);
  if(p.length){ player.path=p; player.pathIdx=0; sfx('step'); return true; } return false; }

cv.addEventListener("mousemove",e=>{ const r=cv.getBoundingClientRect(), sx=e.clientX-r.left, sy=e.clientY-r.top;
  const t=unIso(sx,sy), tx=Math.round(t.tx), ty=Math.round(t.ty);
  hoverTile=inBounds(tx,ty,SCENE.w,SCENE.h)?{tx,ty}:null;
  hoverNpc=null; for(const n of SCENE.npcs){ const p=iso(n.tx,n.ty); if(Math.hypot(sx-p.x,sy-(p.y-26))<26){ hoverNpc=n; break; } }
  hoverExit = hoverTile ? exitAt(hoverTile.tx,hoverTile.ty) : null;
  cv.style.cursor = (hoverNpc||hoverExit) ? "pointer" : (hoverTile && BLOCKED[tileKey(hoverTile.tx,hoverTile.ty)] ? "not-allowed" : "default");
});
cv.addEventListener("click",e=>{ if(document.getElementById("overlay").classList.contains("show")||traveling) return;
  const r=cv.getBoundingClientRect(), sx=e.clientX-r.left, sy=e.clientY-r.top;
  if(hoverExit){ // a causeway tile — walk to it, then the world changes
    if(curTile()[0]===hoverExit.tx && curTile()[1]===hoverExit.ty){ travelTo(hoverExit); return; }
    autoTalk=null; walkTo(hoverExit.tx,hoverExit.ty); return; }
  if(hoverNpc){ // approach the NPC — walk to a free tile beside them, then talk
    if(dist(player.tx,player.ty,hoverNpc.tx,hoverNpc.ty)<1.7){ talk(hoverNpc); return; }
    const c=curTile(), adj=nearestFreeAdjacent(c[0],c[1],hoverNpc.tx,hoverNpc.ty,BLOCKED,SCENE.w,SCENE.h);
    if(adj && walkTo(adj[0],adj[1])){ autoTalk=hoverNpc; }
    return; }
  const t=unIso(sx,sy); let tx=Math.max(0,Math.min(SCENE.w-1,Math.round(t.tx))), ty=Math.max(0,Math.min(SCENE.h-1,Math.round(t.ty)));
  if(BLOCKED[tileKey(tx,ty)]){ const c=curTile(), adj=nearestFreeAdjacent(c[0],c[1],tx,ty,BLOCKED,SCENE.w,SCENE.h); if(!adj) return; tx=adj[0]; ty=adj[1]; }
  autoTalk=null; walkTo(tx,ty);
});
window.addEventListener("keydown",e=>{
  if(document.getElementById("overlay").classList.contains("show")) return;
  if((e.key==="e"||e.key==="E") && nearNpc && dist(player.tx,player.ty,nearNpc.tx,nearNpc.ty)<1.7){ talk(nearNpc); }
});

function update(dt){
  if(player.path && player.pathIdx<player.path.length){ const wp=player.path[player.pathIdx];
    const dx=wp[0]-player.tx, dy=wp[1]-player.ty, d=Math.hypot(dx,dy);
    if(d<0.08){ player.tx=wp[0]; player.ty=wp[1]; player.pathIdx++; if(player.pathIdx>=player.path.length) player.path=null; }
    else { const sp=4.6*dt; player.tx+=dx/d*Math.min(sp,d); player.ty+=dy/d*Math.min(sp,d); if(Math.abs(dx)>0.01) player.facing=dx>=0?1:-1; } }
  // nearest NPC for the talk prompt + auto-talk once you've arrived beside your target
  nearNpc=null; let best=1.7;
  for(const n of SCENE.npcs){ const d=dist(player.tx,player.ty,n.tx,n.ty); if(d<best){ best=d; nearNpc=n; } }
  if(autoTalk && !player.path && dist(player.tx,player.ty,autoTalk.tx,autoTalk.ty)<1.7){ const n=autoTalk; autoTalk=null; talk(n); }
  if(!player.path && !traveling){ const ex=exitAt(curTile()[0],curTile()[1]); if(ex) travelTo(ex); }   // step onto a causeway tile → cross to the next zone
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
  else if(p.type==="boat"){ drawShadow(s.x,s.y+6); ctx.fillStyle="#3a2c1c"; ctx.beginPath(); ctx.moveTo(s.x-30,s.y-2); ctx.quadraticCurveTo(s.x,s.y+12,s.x+30,s.y-2); ctx.lineTo(s.x+24,s.y-9); ctx.quadraticCurveTo(s.x,s.y+2,s.x-24,s.y-9); ctx.closePath(); ctx.fill(); ctx.fillStyle="#4d3a24"; ctx.beginPath(); ctx.moveTo(s.x-24,s.y-9); ctx.quadraticCurveTo(s.x,s.y+2,s.x+24,s.y-9); ctx.quadraticCurveTo(s.x,s.y-4,s.x-24,s.y-9); ctx.closePath(); ctx.fill(); ctx.strokeStyle="#2a2030"; ctx.lineWidth=2; ctx.beginPath(); ctx.moveTo(s.x+14,s.y-7); ctx.lineTo(s.x+30,s.y-34); ctx.stroke(); }
  else if(p.type==="reeds"){ ctx.strokeStyle=`hsl(${p.hue||90} 26% 30%)`; ctx.lineWidth=2; for(let i=-3;i<=3;i++){ const bx=s.x+i*4, sw=(i%2?3:-3); ctx.beginPath(); ctx.moveTo(bx,s.y+4); ctx.quadraticCurveTo(bx+sw,s.y-14,bx+sw*1.6,s.y-26-Math.abs(i)*2); ctx.stroke(); } ctx.fillStyle="#2a3322"; for(let i=-2;i<=2;i+=2){ ctx.beginPath(); ctx.ellipse(s.x+i*4+(i%2?3:-3),s.y-28-Math.abs(i)*2,1.6,4,0,0,7); ctx.fill(); } }
  else if(p.type==="piling"){ drawShadow(s.x,s.y+4); drawBox(s.x,s.y+3,12,30,"#4a3a26","#3a2c1c","#241a12"); ctx.fillStyle="#1d2733"; ctx.beginPath(); ctx.ellipse(s.x,s.y+5,9,4,0,0,7); ctx.fill(); }
  else if(p.type==="shrine"){ drawShadow(s.x,s.y+5); drawBox(s.x,s.y+4,20,14,"#2c2838","#221f2e","#171420"); ctx.fillStyle="#3a3450"; ctx.fillRect(s.x-3,s.y-30,6,18); ctx.fillStyle="#5a5078"; ctx.fillRect(s.x-9,s.y-30,18,4); ctx.fillStyle="#9a86c8"; ctx.beginPath(); ctx.arc(s.x,s.y-34,3.5,0,7); ctx.fill(); }
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
  // floor (blocked tiles read darker; the hovered tile lights unless it's blocked)
  for(let ty=0;ty<SCENE.h;ty++) for(let tx=0;tx<SCENE.w;tx++){ const s=iso(tx,ty), bl=BLOCKED[tileKey(tx,ty)];
    let shade=((tx+ty)%2)?"#181620":"#1c1a26"; if(bl) shade=((tx+ty)%2)?"#141019":"#16121f";
    if(hoverTile&&hoverTile.tx===tx&&hoverTile.ty===ty) shade=bl?"#2a1620":"#2a2740";
    drawDiamond(s.x,s.y, shade, "#13111a"); }
  // zone exits — a glowing causeway tile you can walk onto to cross to the next scene
  (SCENE.exits||[]).forEach(x=>{ const s=iso(x.tx,x.ty), pulse=0.5+0.32*Math.sin(lastT/380);
    drawDiamond(s.x,s.y,`rgba(110,168,200,${0.12+0.10*pulse})`,`rgba(150,200,230,${0.5+0.4*pulse})`);
    ctx.fillStyle=`rgba(180,214,236,${0.6+0.3*pulse})`; ctx.font="600 11px Iowan Old Style, Georgia, serif"; ctx.textAlign="center";
    for(let k=0;k<3;k++){ ctx.globalAlpha=(0.18+0.12*pulse)*(1-k*0.28); ctx.fillText("∙",s.x,s.y-8-k*7); } ctx.globalAlpha=1; });
  // path preview — faint dots along the route, brighter at the destination (BG3-style)
  if(player.path){ for(let i=player.pathIdx;i<player.path.length;i++){ const wp=player.path[i], s=iso(wp[0],wp[1]), last=i===player.path.length-1;
    if(last){ drawDiamond(s.x,s.y,"rgba(231,200,115,.12)","rgba(231,200,115,.6)"); }
    ctx.beginPath(); ctx.arc(s.x,s.y,last?4:2.5,0,7); ctx.fillStyle=last?"#e7c873":"rgba(231,200,115,.5)"; ctx.fill(); } }
  // depth-sorted entities (props + npcs + player)
  const ents=[];
  SCENE.props.forEach(p=>ents.push({d:p.tx+p.ty + (p.type==="banner"?-0.5:0), draw:()=>drawProp(p)}));
  SCENE.npcs.forEach(n=>ents.push({d:n.tx+n.ty, draw:()=>drawToken(n.tx,n.ty,n.hue,n.name,{glow:nearNpc===n||hoverNpc===n, prompt:nearNpc===n})}));
  ents.push({d:player.tx+player.ty, draw:()=>drawToken(player.tx,player.ty,46,null,{player:true})});
  ents.sort((a,b)=>a.d-b.d).forEach(e=>e.draw());
  // exit label on hover — tells you where the causeway goes
  if(hoverExit&&hoverExit.label){ const s=iso(hoverExit.tx,hoverExit.ty); ctx.font="600 12px Iowan Old Style, Georgia, serif"; const w=ctx.measureText(hoverExit.label).width;
    ctx.fillStyle="rgba(12,16,22,.86)"; ctx.fillRect(s.x-w/2-7,s.y-44,w+14,18); ctx.fillStyle="#bcd6ec"; ctx.textAlign="center"; ctx.fillText(hoverExit.label,s.x,s.y-31); }
  // travel fade — the world dissolves and reforms as you cross
  if(fade>0){ ctx.fillStyle=`rgba(7,9,14,${fade})`; ctx.fillRect(0,0,cv.width,cv.height); }
}
function loop(t){ const dt=Math.min(0.05,(t-lastT)/1000||0); lastT=t; update(dt); render(); requestAnimationFrame(loop); }

// ====================== the dialogue overlay ======================
let curConv=null, curNpc=null, pendingOpts=null, loreSeen={}, sensed=false;
function talk(npc){
  curNpc=npc; curConv=CONVS.find(c=>c.id===npc.conv); if(!curConv) return;
  player.path=null; autoTalk=null; loreSeen={}; sensed=false;
  document.getElementById("dsig").textContent=npc.sigil; document.getElementById("dsig").style.background=`hsl(${npc.hue} 52% 62%)`;
  document.getElementById("dname").textContent=npc.name; document.getElementById("dtitle").textContent=npc.title;
  document.getElementById("dscript").innerHTML=""; document.getElementById("overlay").classList.add("show");
  sfx('talk');
  // The Returned-sense — on first sight, if the soul's clarity is keen enough, you perceive the unseen.
  if(curConv.returned && returnedClarity(char) >= curConv.returned.dc){ addSense(curConv.returned.text); }
  const start=curConv.nodes.find(n=>n.id===curConv.start)?curConv.start:curConv.nodes[0].id;
  goNode(start);
}
function closeDialogue(){ document.getElementById("overlay").classList.remove("show"); curConv=null; pendingOpts=null; renderState(); }
function nodeById(id){ return curConv.nodes.find(n=>n.id===id); }
function isEnd(id){ return !id||id==="END"||id==="end"||!nodeById(id); }
function goNode(id){
  pendingOpts=null; const n=nodeById(id), script=document.getElementById("dscript");
  if(!n){ endScene(); return; }
  applyEffects(st, n.onEnter); applyEffects(st, n.effects); renderState();   // node entry + outcome effects
  const line=pickVariantText(n,char,st)||"〔(no line for this character)〕";
  addLine("", curNpc.name, line);
  // LORE — 5e passive knowledge: surface what your character would *recall* about topics in this line.
  glossaryHits(line, GLOSS).forEach(i=>{ const e=GLOSS[i];
    if(e.skill && !loreSeen[i] && loreKnown(char, e, MODEL)){ loreSeen[i]=true; addLore(e); } });
  const opts=document.createElement("div"); opts.className="opts"; script.appendChild(opts);
  const all=n.choices||[];
  if(all.length){ pendingOpts={node:n,el:opts}; paintChoices(); }
  else if(n.auto && !isEnd(n.auto)){ const b=document.createElement("button"); b.className="continue"; b.textContent="Continue ▸"; b.onclick=()=>{ sfx('page'); opts.remove(); goNode(n.auto); }; opts.appendChild(b); }
  else endScene();
  script.scrollTop=script.scrollHeight;
}
function normCheck(ch){ if(ch.check) return ch.check; if(ch.checkDC) return {skill:null,ability:ch.checkAbility,dc:ch.checkDC}; return null; }
function tagFor(ch){ if(ch.tag) return {cls:ch.tag, label:(ch.tagLabel||ch.tag.toUpperCase())};
  const w=ch.when; if(!w) return null;
  if(w.race!==undefined) return {cls:"race",label:[].concat(w.race).join("/")};
  if(w.gender!==undefined) return {cls:"gender",label:[].concat(w.gender).join("/")};
  if(w.class!==undefined) return {cls:"class",label:[].concat(w.class).join("/")};
  if(w.background!==undefined) return {cls:"background",label:[].concat(w.background).join("/")};
  if(w.deity!==undefined) return {cls:"faith",label:[].concat(w.deity).map(d=>d==="None"?"Faithless":d).join("/")};
  if(w.law!==undefined) return {cls:"alignment",label:[].concat(w.law).join("/")};
  if(w.morality!==undefined) return {cls:"alignment",label:[].concat(w.morality).join("/")};
  if(w.ability){ const k=Object.keys(w.ability)[0]; return {cls:"stat",label:`${ABBR[k]} ${w.ability[k]}`}; }
  if(w.int){ const k=Object.keys(w.int)[0]; if(k.indexOf("disp.")===0){ const d=DISP[k]||{name:k}; return {cls:"disp",label:`${d.name} ${roman(w.int[k])}`}; }
    return {cls:"alignment",label:`${(INT_LABELS[k]||k)} ≥ ${w.int[k]}`}; }
  return null; }
function paintChoices(){ if(!pendingOpts) return; const {node:n,el:opts}=pendingOpts; opts.innerHTML="";
  const reveal=document.getElementById("tReveal").checked, force=document.getElementById("tForce").checked, showLocked=document.getElementById("tLocked").checked;
  let num=0;
  n.choices.forEach(ch=>{ const ok=choiceAvailable(char,st,ch,MODEL); if(!ok&&!showLocked) return; const idx=++num;
    const b=document.createElement("button"); b.className="opt"+(ok?"":" locked"); const chk=normCheck(ch), tag=tagFor(ch);
    const passive=chk&&isPassiveSkill(chk.skill);
    let tags=""; if(tag) tags+=`<span class="tg ${tag.cls}">${esc(tag.label)}</span>`;
    if(chk) tags+=`<span class="tg check">${passive?'👁':'🎲'} ${esc(chk.skill||ABBR[chk.ability]||chk.ability)}${passive?' · passive':''}</span>`;
    let inner=(tags?`<div class="tags">${tags}</div>`:"")+`<span class="num">${idx}.</span>${esc(stripBracket(ch.text)||"(continue)")}`;
    if(chk){ const bonus=checkBonus(char,chk,MODEL), prof=chk.skill&&isProficient(char,chk.skill,MODEL);
      if(passive){ const sc=10+bonus; inner+=`<div class="ck"><span class="chip">${esc(chk.skill)} (passive)</span><span class="chip ${sc>=chk.dc?'ok':'bad'}">${sc} vs DC ${chk.dc}${sc>=chk.dc?' ✓ auto':' ✗'}</span>${prof?`<span class="chip prof">proficient +${MODEL.proficiencyBonus}</span>`:''}</div>`; }
      else { const pct=Math.round(chanceToPass(chk.dc,bonus)*100); inner+=`<div class="ck"><span class="chip">${esc(chk.skill||ABBR[chk.ability])} DC ${chk.dc}</span><span class="chip ${pct>=50?'ok':'bad'}">${pct}% (d20 ${bonus>=0?'+':''}${bonus})</span>${prof?`<span class="chip prof">proficient +${MODEL.proficiencyBonus}</span>`:''}${ch.fail?`<span class="muted">miss ▸ another path</span>`:''}</div>`; } }
    if(!ok){ inner+=`<div class="fxline">🔒 ${lockReason(ch)}</div>`; }
    if(reveal){ const fx=describeEffects(ch.effects); if(fx) inner+=`<div class="fxline">▸ ${fx}</div>`; }
    b.innerHTML=inner; if(ok) b.onclick=()=>choose(n,ch,force); opts.appendChild(b);
  });
  if(!num){ const d=document.createElement("div"); d.className="muted"; d.textContent="(no choice here fits this character — toggle “show choices I don't qualify for”, or change who you are)"; opts.appendChild(d); }
  // the breadth readout — how many of ALL authored responses fit who you are & what you've done
  const total=n.choices.length, fits=n.choices.filter(ch=>choiceAvailable(char,st,ch,MODEL)).length;
  if(total>=8){ const tally=document.createElement("div"); tally.className="breadth";
    tally.innerHTML=`✦ <b>${fits}</b> of <b>${total}</b> authored responses fit who you are and what you've done — the rest are gated by race, gender, faith, class, alignment, your stats, your reckoning, and the choices behind you.`;
    opts.appendChild(tally); }
}
function lockReason(ch){ const t=tagFor(ch); const chk=normCheck(ch);
  if(chk && isPassiveSkill(chk.skill) && !passiveBeats(char,chk,MODEL)){ const sc=10+checkBonus(char,chk,MODEL); return `passive ${esc(chk.skill)} ${sc} &lt; DC ${chk.dc} — you don't notice this`; }
  if(t) return `needs ${t.cls==="faith"?"faith":t.cls}: ${esc(t.label)}`;
  return "a different character"; }
function choose(n,ch,force){ sfx('page'); pendingOpts.el.remove(); pendingOpts=null;
  addLine("me","You", stripBracket(ch.text)||"(continue)");
  const chk=normCheck(ch);
  // resolve a check result (r = {success,crit,fumble}) to the right node: crit→ch.crit, fumble→ch.fumble, else next/fail
  const resolve=r=>{ let next; if(r.crit&&ch.crit) next=ch.crit; else if(r.fumble&&ch.fumble) next=ch.fumble;
    else if(r.success) next=ch.next; else next=ch.fail||ch.next;
    applyEffects(st,ch.effects); renderState(); goNode(next); };
  if(chk && isPassiveSkill(chk.skill)){ // passive: it only appeared because you already pass — auto-success, no roll
    addLine("sys","",`(${chk.skill} — passive ${10+checkBonus(char,chk,MODEL)} vs DC ${chk.dc}: success)`);
    applyEffects(st,ch.effects); renderState(); goNode(ch.next); return; }
  if(chk){ const bonus=checkBonus(char,chk,MODEL); if(force) return offerForced(ch,chk,bonus,resolve); rollPreview(ch,chk,bonus,resolve); }
  else { applyEffects(st,ch.effects); renderState(); goNode(ch.next); }
}
// BG3-style anticipation: lay the whole roll bare, then let the player commit to it.
function rollPreview(ch,chk,bonus,resolve){
  const bd=rollBreakdown(char,chk,MODEL), pct=Math.round(bd.chance*100);
  const sgn=v=>`${v>=0?'+':'−'}${Math.abs(v)}`;
  const need = bd.onlyNat20 ? "a <b>natural 20</b>" : bd.onlyNat1 ? "<b>anything but a natural 1</b>" : `a <b>${bd.needShown}</b> or higher`;
  const box=document.createElement("div"); box.className="rollprev";
  box.innerHTML=`<div class="rp-head">🎲 ${esc(chk.skill||ABBR[chk.ability])} check <span>· ${ABBR[chk.ability]} · DC ${chk.dc}</span></div>`+
    `<div class="rp-calc"><span class="rp-die">d20</span> <span class="rp-comp">${sgn(bd.abilityMod)} ${ABBR[chk.ability]}</span>`+
      (bd.prof?` <span class="rp-comp prof">+${bd.profBonus} proficient</span>`:` <span class="rp-comp dim">+0 not proficient</span>`)+
      ` <b class="rp-tot">= ${sgn(bd.bonus)}</b></div>`+
    `<div class="rp-need">You need ${need} on the die.</div>`+
    `<div class="rp-bar"><div class="rp-fill ${pct>=50?'ok':'bad'}" style="width:${Math.max(3,pct)}%"></div><span>${pct}% to succeed</span></div>`+
    `<div class="rp-crit">✦ nat 20 → <b>critical success</b>${ch.crit?' · a special outcome':''} &nbsp;·&nbsp; 💀 nat 1 → <b>critical failure</b>${ch.fumble?' · a special outcome':''}</div>`;
  const roll=document.createElement("button"); roll.className="rp-roll"; roll.innerHTML="🎲 ROLL THE DICE";
  roll.onclick=()=>{ box.remove(); rollDice(chk,bonus,raw=>resolve(rollResult(raw,chk.dc,bonus))); };
  box.appendChild(roll);
  document.getElementById("dscript").appendChild(box); document.getElementById("dscript").scrollTop=1e9;
}
function offerForced(ch,chk,bonus,resolve){ const box=document.createElement("div"); box.className="opts";
  box.innerHTML=`<div class="muted" style="margin-bottom:4px">Preview any branch of this ${chk.skill||ABBR[chk.ability]} DC ${chk.dc} check:</div>`;
  const mk=(l,c,r,label)=>{const b=document.createElement("button");b.className="opt";b.innerHTML=`<span class="chip ${c}">${l}</span>`;b.onclick=()=>{box.remove();addLine("sys","",`(forced — ${label})`);resolve(r);};return b;};
  box.appendChild(mk("✦ NATURAL 20 — critical success","ok",{success:true,crit:true,fumble:false},"natural 20"));
  box.appendChild(mk("✓ success","ok",{success:true,crit:false,fumble:false},"success"));
  box.appendChild(mk("✗ failure","bad",{success:false,crit:false,fumble:false},"failure"));
  box.appendChild(mk("💀 NATURAL 1 — critical failure","bad",{success:false,crit:false,fumble:true},"natural 1"));
  document.getElementById("dscript").appendChild(box); document.getElementById("dscript").scrollTop=1e9; }
function rollDice(chk,bonus,onRaw){ const wrap=document.createElement("div"); wrap.className="dice";
  wrap.innerHTML=`<div class="d20wrap" id="d20w"><div class="d20face rolling">${D20_SVG}</div><span class="dnum">?</span></div><div class="calc">rolling ${chk.skill||ABBR[chk.ability]}…</div>`;
  const sc=document.getElementById("dscript"); sc.appendChild(wrap); sc.scrollTop=1e9; sfx('dice');
  let ticks=0; const w=wrap.querySelector("#d20w"), face=wrap.querySelector(".d20face"), num=wrap.querySelector(".dnum"), calc=wrap.querySelector(".calc");
  const spin=setInterval(()=>{ num.textContent=1+Math.floor(Math.random()*20); if(++ticks>=16){ clearInterval(spin); land(); } },45);
  function land(){ const raw=1+Math.floor(Math.random()*20), total=raw+bonus, r=rollResult(raw,chk.dc,bonus);
    num.textContent=raw; face.classList.remove("rolling"); void face.offsetWidth; face.classList.add("land");
    w.classList.toggle("nat20",r.crit); w.classList.toggle("nat1",r.fumble);
    calc.innerHTML=`${raw} ${bonus>=0?'+':'−'} ${Math.abs(bonus)} <b>= ${total}</b> vs DC ${chk.dc}`;
    const res=document.createElement("div"); res.className="res "+(r.success?"ok":"bad")+(r.crit?" crit":"")+(r.fumble?" fumble":"");
    res.textContent = r.crit?"✦ NAT 20!" : r.fumble?"💀 NAT 1!" : (r.success?"SUCCESS":"FAIL"); wrap.appendChild(res);
    sfx(r.crit?'crit':r.fumble?'fumble':r.success?'good':'bad'); setTimeout(()=>onRaw(raw),820); } }
function endScene(){ const sc=document.getElementById("dscript"); const e=document.createElement("div"); e.className="ending"; e.textContent="▪ The conversation comes to rest."; sc.appendChild(e);
  const b=document.createElement("button"); b.className="leavebtn"; b.textContent="↩ back to the market"; b.onclick=closeDialogue; sc.appendChild(b); sc.scrollTop=1e9; }

function addLine(cls,who,body){ const w=document.getElementById("dscript"); const d=document.createElement("div"); d.className="line "+cls;
  let sig=""; if(who&&cls==="" ){ sig=`<div style="font-size:11px;letter-spacing:1px;text-transform:uppercase;color:#c9a24b;margin-bottom:3px">${esc(who)}</div>`; }
  if(cls==="me") sig=`<div style="font-size:11px;letter-spacing:1px;text-transform:uppercase;color:#9fc0e0;margin-bottom:3px;text-align:right">You</div>`;
  d.innerHTML=sig+`<div class="body">${cls==="sys"?esc(body):prose(body)}</div>`; w.appendChild(d);
  if(cls!=="sys") linkifyEl(d.querySelector(".body")); }
// The Returned-sense inset (what your dead-touched soul perceives) and the LORE recall inset (5e knowledge).
function addSense(text){ const w=document.getElementById("dscript"); const d=document.createElement("div"); d.className="sense";
  d.innerHTML=`<span class="lk">✦ The Returned senses —</span><div>${prose(text)}</div>`; w.appendChild(d); linkifyEl(d); }
function addLore(e){ const w=document.getElementById("dscript"); const d=document.createElement("div"); d.className="lore";
  let h=`<span class="lk">👁 ${esc(e.skill)} (passive) — you recall:</span> ${prose(e.lore)}`;
  if(e.secret && secretKnown(char,e,MODEL)) h+=`<div class="deep"><span class="lk" style="color:#e7c873">…and, deeper (${esc(e.secretSkill)} ${e.secretDc}):</span> ${prose(e.secret)}</div>`;
  d.innerHTML=h; w.appendChild(d); linkifyEl(d); }
// inline glossary linking: walk text nodes, wrap each term's first occurrence per line in a hover-able span
function linkifyEl(el){ if(!el) return;
  const walker=document.createTreeWalker(el, NodeFilter.SHOW_TEXT, null); const nodes=[]; while(walker.nextNode()) nodes.push(walker.currentNode);
  const used={};
  nodes.forEach(node=>{ if(node.parentNode && node.parentNode.classList && node.parentNode.classList.contains("gloss")) return;
    let txt=node.nodeValue, out=null, m, idx=0, frag=null;
    GLOSS_RE.lastIndex=0; const re=new RegExp(GLOSS_RE.source,"gi");
    while((m=re.exec(txt))){ const hit=GLOSS_INDEX.find(o=>o.t.toLowerCase()===m[0].toLowerCase()); if(!hit||used[hit.i]){ continue; } used[hit.i]=true;
      frag=frag||document.createDocumentFragment(); frag.appendChild(document.createTextNode(txt.slice(idx,m.index)));
      const span=document.createElement("span"); span.className="gloss"; span.dataset.k=hit.i; span.textContent=m[0]; frag.appendChild(span); idx=m.index+m[0].length; }
    if(frag){ frag.appendChild(document.createTextNode(txt.slice(idx))); node.parentNode.replaceChild(frag,node); }
  });
}
// tooltip: hover any glossary term for the common-knowledge gloss (+ the deep lore if your character knows it)
const tip=document.getElementById("tip");
document.addEventListener("mouseover",e=>{ const g=e.target.closest&&e.target.closest(".gloss"); if(!g){ return; }
  const e0=GLOSS[+g.dataset.k]; if(!e0) return; const known=loreKnown(char,e0,MODEL), deep=secretKnown(char,e0,MODEL);
  tip.innerHTML=`<div class="tn">${esc(e0.term)}</div>${esc(e0.gloss)}`+
    (e0.skill?(known?`<div class="tl"><b>${esc(e0.skill)} — you recall:</b> ${esc(e0.lore)}</div>`:`<div class="tl" style="color:#6e6680">(a deeper ${esc(e0.skill)} insight escapes you)</div>`):"")+
    (deep?`<div class="tl" style="border-color:#5a4a2a"><b style="color:#e7c873">${esc(e0.secretSkill)} — and deeper:</b> ${esc(e0.secret)}</div>`:"");
  tip.style.display="block"; moveTip(e); });
document.addEventListener("mousemove",e=>{ if(tip.style.display==="block" && e.target.closest&&e.target.closest(".gloss")) moveTip(e); });
document.addEventListener("mouseout",e=>{ if(e.target.closest&&e.target.closest(".gloss")) tip.style.display="none"; });
function moveTip(e){ const pad=14, w=tip.offsetWidth||280, h=tip.offsetHeight||80;
  let x=e.clientX+pad, y=e.clientY+pad; if(x+w>innerWidth-8) x=e.clientX-w-pad; if(y+h>innerHeight-8) y=e.clientY-h-pad;
  tip.style.left=Math.max(8,x)+"px"; tip.style.top=Math.max(8,y)+"px"; }
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
  else if(kind==='crit'){ o.type='sine'; [523,659,784,1047].forEach((f,k)=>o.frequency.setValueAtTime(f,t+k*0.08)); env(0.01,0.42,0.08); o.start(t);o.stop(t+0.5); }       // little triumphant fanfare
  else if(kind==='fumble'){ o.type='sawtooth'; o.frequency.setValueAtTime(330,t); o.frequency.linearRampToValueAtTime(110,t+0.45); env(0.01,0.45,0.07); o.start(t);o.stop(t+0.5); } // sad trombone
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
_npcs = len(MKT['scene']['npcs']) + len(REED['scene']['npcs'])
_beats = sum(len(c['nodes']) for c in ALL_CONVS)
print(f"wrote play/town_market.html ({len(out)//1024} KB) — 2 walkable zones, {_npcs} souls, "
      f"{_beats} dialogue beats, {len(ALL_CONVS)} conversations")
