# THE CROWN OF HORNS — Content & Build Roadmap

A prioritized, pick-up-cold backlog so momentum survives across sessions. Every item
names **why it matters** (which `DESIGN.md` pillar it serves) so we never build content
that dilutes the throughline. Work top-down; check items off as they land.

> **How to use this:** Pull the top unchecked item in the highest active tier, do it,
> commit, check it off, push. If an item is ambiguous, it has an **open question** —
> resolve that first. New ideas go in the right tier, not at the top.

---

## Tier 0 — Foundation (keep the lights on)
*These protect the 19.4k LOC that already works.*

- [ ] **Smoke-test harness** — a headless pass that boots `CampaignBootstrap` + each
      `*Demo` and asserts no exceptions. Pillar IV (trust). *Protects every future change.*
- [ ] **`ContentValidator` in CI** — run the existing validator on every push so broken
      flag references / dialogue graphs fail fast. Pillar III (one brain stays coherent).
- [ ] **Asset manifest in CI** — auto-run `tools/generate-asset-manifest.sh` and commit
      the diff, so the catalog never drifts from reality.

## Tier 1 — Make the Wall *felt* (the bid, §4.2)
*The single highest-leverage emotional work. Theme made tangible.*

- [ ] **The Fugue / Wall art pass** — grey desaturation, faces in the stone. The Breach
      scene must *look* like erasure. Pillar I. **Open Q:** static backdrop or animated?
- [ ] **The Breach has a sound** — one audio cue the player flinches at later when a
      companion is gone. Pillar II (permanence). *Sparse, not bombastic (see DESIGN §5).*
- [ ] **Verdict scene weight** — the Crown Wars "argue a damnation down" needs visual +
      audio gravity to match its writing. Pillar V (witness then decide).
- [ ] **Per-era palette enforcement** — apply the DESIGN §5 palettes to each era's tint
      so eras read apart at a glance and each indicts itself. Pillar I.

## Tier 2 — World density (turn the screw, §6.1)
*More authored content per era — each piece must make the core question heavier.*

- [ ] **Audit existing scenes vs DESIGN §1** — go scene by scene; flag any that don't
      illuminate the Wall; cut or re-point them. Pillar I. *Quality gate before adding more.*
- [ ] **One new reactive hub figure per reputation tier** — the world should visibly
      change as Kelemvor's Doomguides / the Faithless Choir rep shifts. Pillar III.
- [ ] **A companion who *defends* the Wall** — the design needs a credible voice for
      damnation-as-order, or the question is rigged. Pillar I, V. **Open Q:** existing
      companion (Garrow?) or new recruit?
- [ ] **A second moral-hazard scene per era** — replicate the Verdict's "argue it down"
      pattern so each era has a decision, not just a fight. Pillar V.

## Tier 3 — Tighten reactivity (no choice forgotten, §4.3)
- [ ] **Keepsake / deed coverage sweep** — every major choice should leave a tangible
      trace (keepsake, deed, reactive line, or ending slide). Find the gaps. Pillar II.
- [ ] **Ending-slide audit** — confirm each of the six endings reads back *every* major
      thread (`EndingResolver`). Pillar II. *The finale is the payoff for permanence.*
- [ ] **Cross-era callbacks** — a choice in Netheril should be *named* later in the
      Spellplague. Time-travel earns its keep when the past talks to the future. Pillar V.

## Tier 4 — Systems polish (only what serves theme)
- [ ] **Tactical depth that reads** — surface flanking/OA/cover cues in the HUD so players
      can *play to* the 5e math they can trust. Pillar IV.
- [ ] **Hand-painted isometric tilemaps** — the `📋` item in FEATURES; replaces the
      placeholder checker floor once tilesets land via the asset pipeline. Pillar IV.
- [ ] **Accessibility deepening** — the settings already lead here; keep the Wall's horror
      *legible* to everyone (text size, colorblind-safe era palettes, audio cues).

## Someday / stretch (capture, don't build yet)
- [ ] Voice-over for the Court of the Dead finale only (highest emotional ROI per line).
- [ ] A New Game+ that remembers your last verdict in the world's framing.
- [ ] A "Codex of the Damned" — every named soul you witnessed, and what you did.

---

## Decision log
*When an **Open Q** above is resolved, record it here so it stays resolved.*

| Date | Question | Decision | Rationale |
|------|----------|----------|-----------|
| _—_  | _(none yet)_ | | |

---

> Pull the top item. Do it. Check it off. Push. Repeat — *that's* "keep going forever."
