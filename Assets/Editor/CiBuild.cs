using System;
using System.IO;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEngine.Video;

namespace Metroidvania.Editor
{
    public static class CiBuild
    {
        public static void BuildWindows64()
        {
            try
            {
                var scenes = EditorBuildSettings.scenes.Where(scene => scene.enabled)
                    .Select(scene => scene.path).ToArray();
                if (scenes.Length == 0)
                    throw new InvalidOperationException("Build Settings has no enabled scenes.");

                var arguments = Environment.GetCommandLineArgs();
                var pathIndex = Array.IndexOf(arguments, "-customBuildPath");
                if (pathIndex < 0 || pathIndex + 1 >= arguments.Length ||
                    string.IsNullOrWhiteSpace(arguments[pathIndex + 1]) ||
                    arguments[pathIndex + 1].StartsWith("-"))
                    throw new InvalidOperationException("CI requires -customBuildPath followed by an .exe output path.");

                var buildPath = arguments[pathIndex + 1];
                if (!string.Equals(Path.GetExtension(buildPath), ".exe", StringComparison.OrdinalIgnoreCase))
                    throw new InvalidOperationException("The Windows build output must be an .exe path.");

                if (AssetDatabase.LoadAssetAtPath<TMP_Settings>(
                        "Assets/TextMesh Pro/Resources/TMP Settings.asset") == null)
                    throw new InvalidOperationException(
                        "Import TMP Essential Resources with -importPackage before invoking the CI build.");

                // A successful player build can still omit videos that failed to import.
                foreach (var videoPath in AssetDatabase.GetAllAssetPaths().Where(path =>
                             path.StartsWith("Assets/") && path.EndsWith(".mp4", StringComparison.OrdinalIgnoreCase)))
                {
                    if (AssetDatabase.LoadAssetAtPath<VideoClip>(videoPath) == null)
                        throw new InvalidOperationException($"Video failed to import: {videoPath}. Check Git LFS checkout and the Unity import log.");
                }

                // Windows Mono support is included in the Windows Editor installation.
                PlayerSettings.SetScriptingBackend(NamedBuildTarget.Standalone, ScriptingImplementation.Mono2x);

                var report = BuildPipeline.BuildPlayer(new BuildPlayerOptions
                {
                    scenes = scenes,
                    locationPathName = buildPath,
                    target = BuildTarget.StandaloneWindows64,
                    options = BuildOptions.None
                });
                if (report.summary.result != BuildResult.Succeeded)
                    throw new InvalidOperationException(
                        $"Windows build failed: {report.summary.result}, {report.summary.totalErrors} errors.");

                Debug.Log($"CI Windows 64-bit build succeeded: {report.summary.totalSize} bytes.");
                EditorApplication.Exit(0);
            }
            catch (Exception error)
            {
                Debug.LogException(error);
                EditorApplication.Exit(1);
            }
        }
    }
}
