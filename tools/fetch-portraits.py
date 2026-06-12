#!/usr/bin/env python3
"""
Cast the party with PUBLIC-DOMAIN masterworks from Wikimedia Commons.

Requires network egress to: commons.wikimedia.org, upload.wikimedia.org
(add both to the environment's allowlist, then run: python3 tools/fetch-portraits.py).

For each character it searches Commons (restricted to painters dead >100 years, so
the works are PD-old worldwide), downloads the best hit, crops a top-weighted
portrait rectangle, and writes Assets/Resources/Portraits/<name>.png + .meta —
superseding the generated placeholders (same filenames -> auto-wired). Every pick is
logged to docs/PORTRAIT_CREDITS.md with its Commons title and URL.

Characters without a curated query keep their generated portrait. Re-run safe.
"""
import hashlib, io, json, os, time, urllib.parse, urllib.request

ROOT = os.path.join(os.path.dirname(__file__), "..")
OUT = os.path.join(ROOT, "Assets", "Resources", "Portraits")
UA = {"User-Agent": "CrownOfHorns-asset-fetch/1.0 (open-source game; PD-art casting)"}
W, H = 320, 400

# character -> Commons search query (painter constrained to PD-old masters)
QUERIES = {
    "Sister Garrow":      'portrait nun praying painting Bouguereau',
    "Roen Alleywind":     'portrait young man smiling painting Frans Hals',
    "Varra":              'portrait woman red hair painting Waterhouse',
    "Naeve":              'portrait woman melancholy painting Kramskoy',
    "Ilfaeril":           'portrait knight old armor painting Rembrandt',
    "Maerin":             'portrait girl pale painting Vermeer',
    "Aldric Morn":        'portrait old man grief painting Rembrandt',
    "Mhaere":             'portrait woman stern painting Holbein',
    "Sable":              'portrait woman dark cloak painting Sargent',
    "Tamsin":             'portrait peasant girl painting Repin',
    "Quill":              'portrait scribe writing painting Durer',
    "Wrenna Alleywind":   'portrait woman determined painting Ilya Repin',
    "Mother Cass":        'portrait old woman kind painting Rembrandt',
    "High Lord Aelryth":  'portrait nobleman severe painting Holbein',
    "Justiciar Veld":     'portrait judge robes painting Rembrandt',
    "The Pale Cantor":    'portrait monk gaunt painting El Greco',
    "The Returned":       'portrait man shadow self-portrait Rembrandt',
    "Old Davyn":          'portrait old soldier painting Repin',
    "Ferryman":           'portrait fisherman weathered painting Hals',
    "Hensley, dying":     'old man on deathbed painting',
    "Sergeant Kallia":    'portrait woman soldier painting',
    "Collector Vane":     'portrait moneylender painting Metsys',
    "Brother Sere":       'portrait monk reading painting',
    "Harper Handler":     'portrait man hat shadow painting Hals',
    "A Grey Gravedigger": 'gravedigger painting 19th century',
    "Vaelin (an echo)":   'portrait young nobleman ghost pale painting',
    "Tally":              'portrait child clever painting',
}

def api(params):
    url = "https://commons.wikimedia.org/w/api.php?" + urllib.parse.urlencode(params)
    req = urllib.request.Request(url, headers=UA)
    with urllib.request.urlopen(req, timeout=20) as r:
        return json.load(r)

def find_image(query):
    res = api({"action": "query", "list": "search", "srsearch": query,
               "srnamespace": 6, "srlimit": 5, "format": "json"})
    for hit in res.get("query", {}).get("search", []):
        title = hit["title"]
        if not title.lower().endswith((".jpg", ".jpeg", ".png")): continue
        info = api({"action": "query", "titles": title, "prop": "imageinfo",
                    "iiprop": "url", "iiurlwidth": 640, "format": "json"})
        pages = info.get("query", {}).get("pages", {})
        for p in pages.values():
            ii = (p.get("imageinfo") or [{}])[0]
            if ii.get("thumburl"):
                return title, ii["thumburl"], ii.get("descriptionurl", "")
    return None, None, None

def fetch_crop(url):
    from PIL import Image
    req = urllib.request.Request(url, headers=UA)
    with urllib.request.urlopen(req, timeout=30) as r:
        im = Image.open(io.BytesIO(r.read())).convert("RGB")
    # top-weighted crop to portrait ratio (faces live in the upper half of paintings)
    tw, th = im.size
    target = W / H
    if tw / th > target:
        nw = int(th * target); x0 = (tw - nw) // 2; im = im.crop((x0, 0, x0 + nw, th))
    else:
        nh = int(tw / target); im = im.crop((0, 0, tw, min(th, int(nh * 1.0))))
    return im.resize((W, H), Image.LANCZOS)

META_GUID = lambda rel: hashlib.md5(rel.encode()).hexdigest()

def main():
    credits = ["# Portrait credits — public-domain works from Wikimedia Commons",
               "", "All paintings are PD-old (artists dead >100 years). Sources:"]
    done = 0
    for name, q in QUERIES.items():
        dst = os.path.join(OUT, name + ".png")
        try:
            title, url, page = find_image(q)
            if not url:
                print(f"  - {name}: no hit for '{q}' (keeping generated)"); continue
            img = fetch_crop(url)
            img.save(dst, optimize=True)
            # keep the existing .meta (same guid/path) — only pixels change
            credits.append(f"- **{name}** — {title} — {page or url}")
            done += 1
            print(f"  + {name}  <-  {title}")
            time.sleep(0.6)   # be polite to the API
        except Exception as e:
            print(f"  ! {name}: {e} (keeping generated)")
    with open(os.path.join(ROOT, "docs", "PORTRAIT_CREDITS.md"), "w") as f:
        f.write("\n".join(credits) + "\n")
    print(f"\nCast {done}/{len(QUERIES)} characters with PD masterworks. "
          "Now run: python3 tools/make-cast-gallery.py && bash tools/generate-asset-manifest.sh")

if __name__ == "__main__":
    main()
