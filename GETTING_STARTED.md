# Getting Started — The Sundered Crown (Unity)

This walks you from "nothing installed" to "a turn-based isometric skirmish running on
my screen" in about 30 minutes, then points you at how to build real content.

---

## 0. What you need

| Thing | Recommendation | Why |
|---|---|---|
| **Unity Hub** | latest | Manages engine versions/projects |
| **Unity Editor** | **6000.x (Unity 6 LTS)** or 2022.3 LTS | LTS = stable; either runs this code |
| **IDE** | VS Code + C# Dev Kit, or Rider | Code editing + IntelliSense |
| **Source control** | Git + Git LFS | Unity projects need LFS for binary assets |
| Optional | GIMP/Krita, Aseprite, Blender | Art when you're ready |

> This is a **2D/2.5D isometric** starter. You can ship it fully 2D (orthographic
> camera + sprites) or 2.5D (3D models, tilted ortho camera). The code supports both.

---

## 1. Create the Unity project

1. Open **Unity Hub → Projects → New Project**.
2. Choose the **2D (URP)** template (or **2D Core** if you want minimal). URP gives you
   nice 2D lighting later for that moody CRPG torchlit look.
3. Name it `SunderedCrown`. Create.

## 2. Import this starter

1. Copy the **`Assets/`** folder from this package into your new project's `Assets/`
   (merge — don't delete Unity's generated files).
2. Copy the **`docs/`** folder and the `.gitignore` to the project root.
3. Back in Unity, let it compile. Watch the Console — it should compile clean. (All
   scripts live under `Assets/Scripts/` in tidy namespaces; no external packages needed.)

## 3. Run the one-click demo (proves everything works)

1. **File → New Scene → Basic 2D**. Save it as `Assets/Scenes/Demo.unity`.
2. Create an empty GameObject (**GameObject → Create Empty**), name it `Bootstrap`.
3. **Add Component → Demo Bootstrap** (the `DemoBootstrap` script).
4. Press **Play**.

You should see colored cubes on a diamond grid, an on-screen combat log, an initiative
list, and an active-unit panel. Controls:
- **Left-click a tile** → your active unit paths there (spends movement).
- **Left-click an adjacent red enemy** → attack it.
- **Space** or the **End Turn** button → pass the turn.
- Enemies take their turns automatically (move + attack).
- **Mouse wheel** zooms; **WASD / arrows / screen edges** pan the camera.

That's the full loop — grid, isometric projection, A* pathing, initiative, the 5e
action economy, attack rolls vs AC, damage, win/loss — running with zero art and zero
data assets. From here you replace pieces with real content.

## 4. Make your first *real* content (no code)

The whole game is data-driven through **ScriptableObjects**. Try this:

1. In the Project window: **Create → Sundered Crown → Class Definition**. Name it
   `Fighter`. Set Hit Die 10, Primary Ability Strength.
2. **Create → Sundered Crown → Race Definition** → `Human`, speed 6.
3. **Create → Sundered Crown → Ability** → `Longsword`, damage `1d8`, range 1.
4. **Create → Sundered Crown → Dialogue Graph** → author Corin's intro from the Story
   Bible (see `docs/CONTENT_PIPELINE.md` for exactly how nodes/choices/flags map).
5. **Create → Sundered Crown → Quest** and **→ Item** similarly.

None of that requires touching C#. That separation — code is the engine, assets are the
game — is what lets one or two people build a big CRPG.

## 5. Put it under version control

```bash
cd SunderedCrown
git init
git lfs install
git add .gitattributes .gitignore Assets docs ProjectSettings Packages
git commit -m "Sundered Crown: engine starter + story bible"
```
Create a new empty repo on GitHub and push. (The `.gitignore` here already excludes
`Library/`, `Temp/`, `Build/`, etc.) Add a `.gitattributes` for LFS covering
`*.png *.psd *.fbx *.wav *.mp3 *.anim` and friends.

---

## 6. Where to go next

- **`docs/ARCHITECTURE.md`** — how the systems fit together and where to extend.
- **`docs/CONTENT_PIPELINE.md`** — author dialogue/quests/items as assets.
- **`docs/ROADMAP.md`** — a milestone plan from vertical slice to full game.
- **`docs/STORY_BIBLE.md`** — the world, villains, companions, and acts to build.

### Recommended learning (free, high quality)
- **Unity Learn** — *Junior Programmer* and *Creative Core* pathways.
- **Brackeys** (YouTube, archive) — timeless Unity fundamentals.
- **Tarodev** & **Code Monkey** (YouTube) — grid/A*/turn-based and clean C# patterns.
- **Sebastian Lague** — *A\* Pathfinding* and procedural series (superb).
- **Game Programming Patterns** (free book, gameprogrammingpatterns.com) — the patterns
  this starter leans on (State, Observer/events, Service Locator, Component).
- **Unity 2D Isometric** docs + the **2D Tilemap** package for hand-built iso maps.
- For writing branching narrative at scale, look at **Ink** (inkle) or **Yarn Spinner**
  — you can swap our DialogueGraph for either if you want a dedicated scripting language.
```
