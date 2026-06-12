using System.IO;
using UnityEditor;
using UnityEngine;

namespace SunderedCrown.EditorTools
{
    /// <summary>
    /// One-call player builds for the procedural-boot game. The only scene is the empty
    /// <c>Assets/Scenes/Boot.unity</c> — <c>GameEntryPoint</c> spawns the MainMenu and everything
    /// else at runtime, so the build needs nothing more. Run from the menu
    /// (Build/…) or headlessly via <c>tools/build.sh</c>
    /// (<c>Unity -batchmode -executeMethod SunderedCrown.EditorTools.BuildScript.BuildWindows</c>).
    /// Outputs land in <c>Builds/&lt;Target&gt;/</c>.
    /// </summary>
    public static class BuildScript
    {
        private static readonly string[] Scenes = { "Assets/Scenes/Boot.unity" };

        [MenuItem("Build/Windows (x64)")]
        public static void BuildWindows() =>
            Build(BuildTarget.StandaloneWindows64, "Builds/Windows/CrownOfHorns.exe");

        [MenuItem("Build/Linux (x64)")]
        public static void BuildLinux() =>
            Build(BuildTarget.StandaloneLinux64, "Builds/Linux/CrownOfHorns.x86_64");

        [MenuItem("Build/macOS")]
        public static void BuildMacOS() =>
            Build(BuildTarget.StandaloneOSX, "Builds/macOS/CrownOfHorns.app");

        private static void Build(BuildTarget target, string outputPath)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(outputPath));
            var options = new BuildPlayerOptions
            {
                scenes = Scenes,
                locationPathName = outputPath,
                target = target,
                options = BuildOptions.None
            };
            var report = BuildPipeline.BuildPlayer(options);
            var summary = report.summary;
            if (summary.result == UnityEditor.Build.Reporting.BuildResult.Succeeded)
                Debug.Log($"[Build] {target} OK -> {outputPath} ({summary.totalSize / (1024 * 1024)} MB, {summary.totalTime.TotalSeconds:0}s)");
            else
                Debug.LogError($"[Build] {target} FAILED: {summary.result} ({summary.totalErrors} errors)");
        }
    }
}
