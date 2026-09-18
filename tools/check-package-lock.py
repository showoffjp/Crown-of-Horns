#!/usr/bin/env python3
"""
Guard against a corrupt Packages/packages-lock.json.

Unity resolves packages from the lock file when one is present. A lock that
omits a dependency the manifest requires resolves into a half-wired state: the
package's source lands in Library/PackageCache but its assembly references are
never established, so the package fails to compile *its own* files — e.g.

  Library/PackageCache/com.unity.ugui@.../Runtime/UGUI/UI/Core/Dropdown.cs:
  error CS0246: The type or namespace name 'Image' could not be found

That is what shipped here: the committed lock held 37 builtin modules and not
one registry package, while the manifest required com.unity.ugui and
com.unity.test-framework. Deleting Library/ never fixed it, because the lock
lives in Packages/ and is tracked in git.

No lock file is fine — Unity regenerates one. An inconsistent lock is not.

  python3 tools/check-package-lock.py
"""
import json, os, sys

ROOT = os.path.join(os.path.dirname(__file__), "..")
LOCK = os.path.join(ROOT, "Packages", "packages-lock.json")
MAN = os.path.join(ROOT, "Packages", "manifest.json")

def stray_package_entries():
    """Packages/ should hold manifest.json (+ a generated lock). Anything else —
    a folder or, worse, a symlink — is an EMBEDDED package that overrides the
    registry copy. A symlinked com.unity.ugui makes Unity load uGUI from a path
    it does not control, and UnityEngine.UI then fails to exist as an assembly:
      CS0234: The type or namespace name 'UI' does not exist in 'UnityEngine'
    """
    d = os.path.join(ROOT, "Packages")
    allowed = {"manifest.json", "packages-lock.json"}
    out = []
    for e in sorted(os.listdir(d)) if os.path.isdir(d) else []:
        if e in allowed:
            continue
        p = os.path.join(d, e)
        out.append((e, "symlink" if os.path.islink(p) else
                       "directory" if os.path.isdir(p) else "file"))
    return out


def main():
    stray = stray_package_entries()
    if stray:
        print("✗ Packages/ contains entries that override registry packages\n")
        for name, kind in stray:
            print(f"    {name}   ({kind})")
        print("\n  Unity treats any folder in Packages/ as an EMBEDDED package,")
        print("  taking precedence over the version manifest.json asks for.")
        print("  Remove them (delete the LINK, never the target) and reopen Unity.")
        return 1

    if not os.path.exists(LOCK):
        print("✓ no packages-lock.json — Unity will resolve from manifest.json and regenerate it.")
        return 0
    try:
        lock = json.load(open(LOCK, encoding="utf-8")).get("dependencies", {})
        man = json.load(open(MAN, encoding="utf-8")).get("dependencies", {})
    except (OSError, ValueError) as e:
        print(f"✗ could not read package files: {e}")
        return 1

    missing = [k for k in man if k not in lock]
    if missing:
        print("✗ packages-lock.json is inconsistent with manifest.json\n")
        print("  required by manifest but absent from the lock:")
        for k in sorted(missing):
            print(f"    {k}   (manifest wants {man[k]})")
        print("\n  Unity resolves from the lock, so these packages will be wired")
        print("  incorrectly and may fail to compile their own source.")
        print("  Fix: delete Packages/packages-lock.json and let Unity regenerate it.")
        return 1

    reg = sum(1 for v in lock.values() if v.get("source") != "builtin")
    print(f"✓ packages-lock.json consistent: {len(lock)} locked "
          f"({reg} registry, {len(lock)-reg} builtin); every manifest dependency present.")
    return 0

if __name__ == "__main__":
    sys.exit(main())
