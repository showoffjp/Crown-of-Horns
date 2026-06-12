#!/usr/bin/env bash
# One-command player build. Usage: tools/build.sh [windows|linux|macos]
# Finds the Unity editor (env UNITY_PATH first, then common Hub install dirs),
# then runs the headless BuildScript. Output: Builds/<Target>/.
set -euo pipefail
cd "$(dirname "$0")/.."
TARGET="${1:-windows}"
case "$TARGET" in
  windows) METHOD=BuildWindows ;;
  linux)   METHOD=BuildLinux ;;
  macos)   METHOD=BuildMacOS ;;
  *) echo "usage: tools/build.sh [windows|linux|macos]"; exit 2 ;;
esac

VERSION="$(grep -oP 'm_EditorVersion: \K.*' ProjectSettings/ProjectVersion.txt)"
find_unity() {
  [ -n "${UNITY_PATH:-}" ] && { echo "$UNITY_PATH"; return; }
  for p in \
    "$HOME/Unity/Hub/Editor/$VERSION/Editor/Unity" \
    "/opt/unity/editors/$VERSION/Editor/Unity" \
    "/Applications/Unity/Hub/Editor/$VERSION/Unity.app/Contents/MacOS/Unity" \
    "/c/Program Files/Unity/Hub/Editor/$VERSION/Editor/Unity.exe" \
    "C:/Program Files/Unity/Hub/Editor/$VERSION/Editor/Unity.exe"; do
    [ -x "$p" ] && { echo "$p"; return; }
  done
  echo ""
}
UNITY="$(find_unity)"
if [ -z "$UNITY" ]; then
  echo "Unity $VERSION not found. Set UNITY_PATH=/path/to/Unity and re-run." >&2
  exit 1
fi
echo "Building $TARGET with $UNITY ..."
"$UNITY" -batchmode -quit -projectPath "$PWD" \
  -executeMethod "SunderedCrown.EditorTools.BuildScript.$METHOD" -logFile - \
  || { echo "Build FAILED — see log above."; exit 1; }
echo "Done. See Builds/."
