#!/usr/bin/env bash
#
# check-cs-structure.sh
# A no-compiler smoke test for the C# sources: every .cs under Assets/ must have
# balanced { } and exactly one `namespace` declaration. This is the same cheap
# brace-balance methodology the project has always used to catch truncated files,
# bad merges, and copy-paste errors without a Unity compiler — now runnable in CI
# (see .github/workflows/ci.yml).
#
# Note: we intentionally do NOT count ( ) — parentheses appear unbalanced inside
# string literals and comments ("(adv)", "(half)") across the codebase, so they're
# a false-positive magnet. Braces in strings are rare enough to be a reliable signal.
#
# Usage:  bash tools/check-cs-structure.sh
# Exit:   0 = all good, 1 = a structural problem was found.
#
set -euo pipefail

ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
fail=0
count=0

while IFS= read -r -d '' f; do
  count=$((count + 1))
  open=$(tr -cd '{' < "$f" | wc -c)
  close=$(tr -cd '}' < "$f" | wc -c)
  ns=$(grep -c '^namespace ' "$f" || true)

  rel="${f#"$ROOT"/}"
  if [ "$open" != "$close" ]; then
    echo "::error file=$rel::brace mismatch — { $open vs } $close"
    fail=1
  fi
  if [ "$ns" -ne 1 ]; then
    echo "::error file=$rel::expected exactly one namespace, found $ns"
    fail=1
  fi
done < <(find "$ROOT/Assets" -name '*.cs' -print0 | sort -z)

if [ "$fail" -eq 0 ]; then
  echo "Structural check passed: $count C# file(s), all brace-balanced with one namespace."
fi
exit $fail
