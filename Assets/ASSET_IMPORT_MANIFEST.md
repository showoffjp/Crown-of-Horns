# Asset Import Manifest

The plan for getting a large asset library into the project **without** fighting the
25 MB (GitHub web) / 30 MB (chat) upload limits. Fill in the tables as you go.

> **The golden rule:** only *your own* assets belong in git (via Git LFS). Anything that
> can be **re-imported** — Unity Asset Store packages, Package Manager packages — should be
> *listed here and re-imported*, **not uploaded**. That alone usually removes 80–90% of the bulk.

See `ASSETS.md` for the folder layout, naming convention, and the exact Git-LFS push workflow.

---

## Decision tree for every asset

```
Is it from the Unity Asset Store / Package Manager / a marketplace?
├─ YES → DON'T upload it. Add a row to "Re-import" below; re-add it in your editor.
└─ NO  → Is it a huge raw master (.psd/.blend/.wav source)?
         ├─ YES → Assets/_Source/ via LFS, OR keep it out of the repo (Drive/Dropbox).
         └─ NO  → It's a game-ready custom asset → commit via Git LFS (see ASSETS.md).
```

---

## 1. Re-import (do NOT upload these)

Packages you own that get pulled back in via the Unity editor (Window → Package Manager →
My Assets, or Package Manager → Unity Registry). List them so any fresh clone is reproducible.

| Asset / package | Source | Publisher | Version | Used for | License notes |
|---|---|---|---|---|---|
| _e.g. Synty POLYGON Fantasy_ | Asset Store | Synty Studios | 1.x | Town props, characters | per-seat |
|  |  |  |  |  |  |
|  |  |  |  |  |  |

> **Tip:** Asset Store purchases live in your Unity account forever — a teammate (or a CI
> machine) re-imports them; they never travel through git. Keep this list current and a clone
> is always rebuildable.

---

## 2. Commit via Git LFS (your own / custom assets)

The game-ready assets that *do* belong in the repo. Group by destination folder (see `ASSETS.md`).

| Asset(s) | Destination | Type | Approx size | LFS? | Status |
|---|---|---|---|---|---|
| _e.g. hero 8-dir sprite set_ | `Assets/Art/Sprites/Characters/` | png | 40 MB | yes | ☐ to push |
|  |  |  |  |  |  |
|  |  |  |  |  |  |

After dropping files in, run `bash tools/generate-asset-manifest.sh` and commit the updated
`Assets/asset-manifest.csv` so the catalog stays in sync (CI checks this).

---

## 3. Keep OUT of the repo (raw masters / huge sources)

Editable originals the engine never loads. Park them in cloud storage; link here.

| Asset(s) | Where it lives | Why it's out | Link |
|---|---|---|---|
| _e.g. character .blend masters_ | Google Drive | 3 GB, never loaded at runtime | _link_ |
|  |  |  |  |

---

## The push workflow (no terminal required)

1. **GitHub Desktop** → clone `Isometric-CRPG`, switch to branch `claude/determined-maxwell-XP0IV`.
2. Drop your **section-2** assets into the right `Assets/...` folders.
3. GitHub Desktop detects Git LFS automatically (the repo's `.gitattributes` is already set up).
4. Commit → Push. Files up to **2 GB each** sail through — no web/chat limit involved.

Terminal equivalent and the LFS-quota details (1 GB free; $5/50 GB data packs; self-host option)
are in `ASSETS.md`.

---

## LFS quota tracker

GitHub free tier: **1 GB storage + 1 GB/month bandwidth.** Keep a rough running total so a
data-pack purchase never surprises you.

| Date | Pushed | Cumulative LFS size | Notes |
|---|---|---|---|
|  |  |  |  |
