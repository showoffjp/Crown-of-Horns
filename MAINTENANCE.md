# 🔧 Project Maintenance Conventions

## 📌 Standing rule: keep the docs live

**On every change/prompt that adds or alters functionality, refresh the Markdown docs in
the same pass** so they always reflect reality:

- 📋 **`FEATURES.md`** — move items between ✅ / 🚧 / 📋 / 💡; add new capabilities.
- 📝 **`CHANGELOG.md`** — add or extend the current version entry (newest first).
- 📖 **`README.md`** — update highlights, badges, counts, and quick-start if flows change.
- 🗺️ **`docs/STORY_BIBLE.md`** — extend when story/characters/lore are added.
- 🧱 **`docs/ARCHITECTURE.md`**, ✍️ **`docs/CONTENT_PIPELINE.md`**, 🚀 **`GETTING_STARTED.md`**,
  🗺️ **`docs/ROADMAP.md`** — update when systems, authoring steps, or milestones change.

Style: rich and scannable — badges, emoji icons, tables, and short blurbs.

> 💡 To *enforce* this automatically across sessions, add a Claude Code **UserPromptSubmit
> hook** that injects this reminder on every prompt (a hook can remind; the doc edits are
> still authored by the assistant). Ask to set it up.

## 🔢 Versioning
Semantic-ish: bump **minor** for a new system/feature set, **patch** for fixes/polish.
