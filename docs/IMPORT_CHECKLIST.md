# ✅ Import Checklist — getting Crown of Horns running in Unity

*A calm, literal, tick-box path. You do not have to do this all at once — or today. Each box is small.*

---

## Where things honestly stand (audited 2026-06-29, evidence not vibes)

- **The prototype runs — proven.** `node play/run-all.js` → 1,530 tests pass; `play/index.html` plays
  in a browser now. If nothing else ever worked, this is real and yours.
- **The Unity C# project is structurally sound — statically verified, not compiled.** What was checked
  from outside Unity and came back clean:
  - 230 C# files, all brace-balanced, one namespace each (no truncated/rotted files).
  - **No external package dependencies** — only base Unity modules. (The #1 cause of "won't even open"
    is missing/mismatched packages. There are none here.)
  - No `using UnityEditor` leaking into runtime code; no duplicate class names; **no removed Unity-6
    APIs**; **every `using SunderedCrown.*` resolves to a real namespace.**
  - Assembly definitions wired correctly (tests reference the runtime + NUnit + TestRunner).
  - `GameEntryPoint` uses `[RuntimeInitializeOnLoadMethod]` → the game boots itself from the one
    `Boot.unity` scene. A real headless build script (`tools/build.sh`) exists. 29 EditMode test suites
    are ready to run.
- **What is NOT verified, and only a real compiler can tell us:** type/signature mismatches,
  method-not-found across files. In 34,000 lines that have never compiled, expect *some*. **The honest
  expectation is: it opens, throws a handful of compile errors, you fix them (or paste them to me — C#
  I can read and fix from here), and then it runs.** Not "perfect on first Play"; not "broken garbage."
  The normal, surmountable middle.

> ⚠️ **The one thing only you can do:** open it in Unity. I have no Unity in my environment, so I
> cannot press the final button. Everything *up to* that button, I can help with.

---

## Part 1 — Run the prototype (5 min, zero install) — *the guaranteed win*
- [ ] Open `play/index.html` (or `play/crown_of_horns.html`) in any browser. Play. This is everything
      built so far.
- [ ] (optional) `node play/run-all.js` → watch 1,530 tests pass.

## Part 2 — Open the real game in Unity (~45 min, one-time)
- [ ] Install **Unity Hub**.
- [ ] In Hub, install editor version **`6000.4.9f1`** exactly (pinned in
      `ProjectSettings/ProjectVersion.txt`). Add **Windows/Mac/Linux Build Support**.
- [ ] Install **Git LFS** once: `git lfs install`. (Art/audio binaries live in LFS.)
- [ ] In Unity Hub → **Open** → select **this repo's root folder** (it *is* the project — has
      `Assets/`, `Packages/`, `ProjectSettings/`). Do **not** make a new project and copy files in;
      that old instruction in `GETTING_STARTED.md` is stale.
- [ ] Let it import (first open is slow — minutes). Watch the **Console** at the bottom.
- [ ] **If the Console shows red compile errors:** that's expected and fixable. Copy them and send them
      to me. We work the list down together. (Most will be small.)
- [ ] Once it compiles clean: open `Assets/Scenes/Boot.unity`, press **▶ Play**. `GameEntryPoint`
      should bring up the Main Menu → New Game → the loop.

## Part 3 — Prove the engine with tests (5 min)
- [ ] Window → General → **Test Runner** → **EditMode** tab → **Run All** (29 suites).
- [ ] Green = the combat/dialogue/quest/ending engines work the same as the prototype that already passes.

## Part 4 — Make a build you can double-click (when ready)
- [ ] In the Editor menu: **Build → Windows (x64)** (or Mac/Linux). Output lands in `Builds/`.
- [ ] Or headless, no clicking: `tools/build.sh windows` (needs Unity installed; finds it via
      `UNITY_PATH` or Hub).

## Part 5 — Polish to "finished" (the long tail, per `docs/STATE_OF_THE_GAME.md`)
- [ ] **Audio + combat VFX** — currently ~0%. You license packs (`UNITYASSETS.MD` is the shopping
      list, bought in *your* editor); I wire them into the existing `AudioSystem` / `FxSystem` hooks.
- [ ] **This session's prototype content into Unity** — Calloway, the Second Death, the Wayward Mile,
      etc. live in `play/` and are **not** in the C# game yet. I can build a JSON→Unity loader so
      `play/*.json` becomes the single source of truth (no re-authoring), or port them to C# content
      classes. Your call when you have energy.
- [ ] **Distributable polish** — icon, splash, itch.io/Steam page.

---

## The safety net, in one line
**You are not alone in the scary part.** Open it, and whatever the Console says — paste it here. I can
read and edit the C# from this environment. The wall of red becomes a to-do list, and we cross it off.

*None of this expires. It'll be here when you have the energy. Take care of yourself first.*
