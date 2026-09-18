#!/usr/bin/env python3
"""
Catch the Unity compile error CI cannot: an assembly definition that fails to
reference a package assembly its own scripts `using`.

An .asmdef REPLACES Unity's default auto-references, so every *package*
assembly a script uses must be listed explicitly. Engine modules
(UnityEngine.AI, .Audio, …) arrive via noEngineReferences:false and need no
entry; package assemblies do. Omitting one yields e.g.
  CS0246: The type or namespace name 'Image' could not be found
which is what shipped in SunderedCrown.asmdef until it was fixed.

CI never compiles C# (the Unity job self-skips without a UNITY_LICENSE), so
this static check is the only thing standing between a missing reference and
a Safe Mode prompt on someone's machine.

  python3 tools/check-asmdef-refs.py
"""
import json, os, re, sys

ROOT = os.path.join(os.path.dirname(__file__), "..")

# namespace prefix -> assembly that must appear in an asmdef's "references"
PACKAGE_NS = {
    "UnityEngine.UI":           "UnityEngine.UI",
    "UnityEngine.EventSystems": "UnityEngine.UI",
    "TMPro":                    "Unity.TextMeshPro",
    "UnityEngine.InputSystem":  "Unity.InputSystem",
    "UnityEngine.TestTools":    "UnityEngine.TestRunner",
    "NUnit.Framework":          "nunit.framework.dll",
    "UnityEngine.Timeline":     "Unity.Timeline",
    "Cinemachine":              "Cinemachine",
}

def asmdef_scopes():
    """every .asmdef with the directory it governs, deepest first"""
    out = []
    for dirpath, _dirs, files in os.walk(os.path.join(ROOT, "Assets")):
        for f in files:
            if f.endswith(".asmdef"):
                out.append((dirpath, os.path.join(dirpath, f)))
    return sorted(out, key=lambda p: -len(p[0]))

def owning_asmdef(cs_path, scopes):
    for d, a in scopes:                      # deepest scope wins
        if cs_path.startswith(d + os.sep) or os.path.dirname(cs_path) == d:
            return a
    return None                              # Assembly-CSharp: auto-references all

def main():
    scopes = asmdef_scopes()
    if not scopes:
        print("No .asmdef files — every script is in Assembly-CSharp; nothing to check.")
        return 0

    refs, names = {}, {}
    for _d, a in scopes:
        data = json.load(open(a, encoding="utf-8"))
        names[a] = data.get("name", os.path.basename(a))
        refs[a] = set(data.get("references") or []) | set(data.get("precompiledReferences") or [])

    missing = {}
    for dirpath, _dirs, files in os.walk(os.path.join(ROOT, "Assets")):
        for f in files:
            if not f.endswith(".cs"):
                continue
            p = os.path.join(dirpath, f)
            owner = owning_asmdef(p, scopes)
            if not owner:
                continue
            src = open(p, encoding="utf-8", errors="replace").read()
            for ns in re.findall(r"^\s*using\s+([A-Za-z0-9_.]+)\s*;", src, re.M):
                need = PACKAGE_NS.get(ns)
                if need and need not in refs[owner]:
                    missing.setdefault((owner, need), []).append(
                        os.path.relpath(p, ROOT).replace(os.sep, "/"))

    if missing:
        print("✗ asmdef reference check FAILED\n")
        for (owner, need), users in sorted(missing.items()):
            rel = os.path.relpath(owner, ROOT).replace(os.sep, "/")
            print(f"  {rel}  (assembly '{names[owner]}')")
            print(f"    missing reference: {need}")
            for u in sorted(set(users))[:6]:
                print(f"      used by {u}")
            print()
        print("Add each missing name to that .asmdef's \"references\" array.")
        return 1

    n = sum(1 for _ in scopes)
    print(f"✓ asmdef references complete: {n} assembly definition(s); "
          f"every package namespace a script `using`s is referenced.")
    return 0

if __name__ == "__main__":
    sys.exit(main())
