#!/usr/bin/env python3
"""
Every marker and unit in the Unity world must resolve to art.

The whole point of the standee/prop work is that nothing renders as a coloured
cube any more. That is easy to regress and invisible in CI, because CI does not
compile or run Unity: add an NPC whose name has no portrait, or rename a marker
so it stops matching a PropArt rule, and it silently goes back to being a box.
This mirrors the C# resolution order in Python and fails when anything falls
through it.

Mirrored from:
  Rendering/MarkerArt.Resolve   — container -> chest, talk -> standee, else prop,
                                  then standee, then a neutral standee
  Rendering/UnitSpriteSkinner.Resolve — standee(name) -> standee(class)
                                  -> standee(faction) -> battle token
  Rendering/WorldArt.Standee    — exact name, then first word

Run: python3 tools/check-world-art.py
"""
import glob, os, re, sys

ROOT = os.path.join(os.path.dirname(__file__), "..")
SCRIPTS = os.path.join(ROOT, "Assets", "Scripts")
RES = os.path.join(ROOT, "Assets", "Resources")

def names_in(folder):
    d = os.path.join(RES, folder)
    if not os.path.isdir(d):
        return set()
    return {f[:-4] for f in os.listdir(d) if f.endswith(".png")}

def prop_rules():
    """Parse the keyword table out of PropArt.cs, in declaration order."""
    src = open(os.path.join(SCRIPTS, "Rendering", "PropArt.cs"), encoding="utf-8").read()
    rules = []
    for m in re.finditer(r'\(new\[\] \{ ((?:[^{}])*) \}, "([a-z_]+)", ([0-9.]+)f\)', src, re.S):
        keys = [k.strip().strip('"') for k in m.group(1).replace("\n", " ").split(",") if k.strip()]
        rules.append((keys, m.group(2)))
    return rules

def cs_sources():
    return sorted(glob.glob(os.path.join(SCRIPTS, "**", "*.cs"), recursive=True))

def main():
    props, standees, tokens = names_in("Props"), names_in("Standees"), names_in("Sprites")
    rules = prop_rules()
    if not rules:
        print("✗ could not parse any rules out of PropArt.cs", file=sys.stderr)
        return 1

    def prop_for(label):
        low = label.lower()
        for keys, name in rules:
            if any(k in low for k in keys):
                return name if name in props else None
        return None

    def standee_for(label):
        if not label:
            return None
        if label in standees:
            return label
        first = label.split(" ")[0]
        return first if first in standees else None

    markers, units = {}, set()
    marker_pat = re.compile(r'Make(Npc|Examine|Exit|Container|Marker)\(grid,\s*"([^"]+)"')
    unit_pat = re.compile(r'displayName\s*=\s*"([^"]+)"')
    for path in cs_sources():
        text = open(path, encoding="utf-8", errors="replace").read()
        for kind, label in marker_pat.findall(text):
            markers.setdefault(label, set()).add(kind)
        units.update(unit_pat.findall(text))

    bad, generic = [], []
    for label, kinds in sorted(markers.items()):
        kind = "Npc" if "Npc" in kinds else sorted(kinds)[0]
        if kind == "Container":
            ok = ("chest" in props) or bool(prop_for(label))
        elif kind == "Npc":
            # A talkable marker ends at the neutral standee, so it can only fail if
            # that fallback itself is missing. Landing there is not a cube, but it is
            # a named character wearing a stranger's face — worth saying out loud.
            own = bool(standee_for(label)) or bool(prop_for(label))
            if not own:
                generic.append(label)
            ok = own or "Neutral" in standees
        else:
            ok = bool(prop_for(label)) or bool(standee_for(label))
        if not ok:
            bad.append(("marker/" + kind, label))

    # Units fall back through class and faction; those archetypes must all exist.
    for archetype in ("Fighter", "Barbarian", "Cleric", "Ranger", "Rogue", "Wizard",
                      "Player", "Ally", "Enemy", "Neutral"):
        if archetype not in standees:
            bad.append(("archetype", archetype))
    for name in sorted(units):
        if not (standee_for(name) or name in tokens or "Neutral" in standees):
            bad.append(("unit", name))

    for label in generic:
        print(f"  ! {label!r} has no art of its own — falling back to the generic "
              f"Neutral standee", file=sys.stderr)

    if bad:
        print(f"✗ {len(bad)} thing(s) in the Unity world would render as a coloured cube:",
              file=sys.stderr)
        for kind, label in bad:
            print(f"    {kind:16} {label}", file=sys.stderr)
        print("\n  Add a portrait (tools/gen-portraits-v3.py picks up dialogue speakers and\n"
              "  Unity MakeNpc labels automatically), or a PropArt rule plus a painter in\n"
              "  tools/gen-props.py, then re-run tools/gen-standees.py.", file=sys.stderr)
        return 1

    print(f"✓ world art complete: {len(markers)} markers and {len(units)} unit names all "
          f"resolve ({len(props)} props, {len(standees)} standees)")
    return 0

if __name__ == "__main__":
    sys.exit(main())
