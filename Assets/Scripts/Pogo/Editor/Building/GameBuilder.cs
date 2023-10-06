using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace Pogo.Building
{
    public static class GameBuilder
    {
        public struct PogoBuildTarget
        {
            public string BuildName;
            public BuildTarget Target;
            public string FileName;

            public PogoBuildTarget(string buildName, BuildTarget target, string extension)
            {
                BuildName = buildName;
                Target = target;
                FileName = extension;
            }
        }

        public struct PogoBuildResult
        {
            public PogoBuildTarget Target;
            public BuildResult Result;

            public PogoBuildResult(PogoBuildTarget target, BuildReport report)
            {
                Target = target;
                Result = report.summary.result;
            }
        }

        public class FileLogger
        {
            string logPath;

            public FileLogger(string logPath)
            {
                this.logPath = logPath;
                Directory.CreateDirectory(Path.GetDirectoryName(logPath));
            }

            public void WriteLine(string message)
            {
                Debug.Log(message);
                var writer = File.AppendText(logPath);
                writer.WriteLine(message);
                writer.Dispose();
            }
        }


#if DISABLESTEAMWORKS
        [MenuItem("Pogo/Build (Itch)")]
        public static void BuildItch()
        {
            string pathRoot = $"{Path.GetDirectoryName(Application.dataPath)}{Path.DirectorySeparatorChar}Build{Path.DirectorySeparatorChar}Itch";
            FileLogger logger = new FileLogger($"{pathRoot}{Path.DirectorySeparatorChar}Logs.txt");

            EditorApplication.delayCall += () =>
            {
                Debug.Log("Starting Build...");
                var targets = new PogoBuildTarget[]
                {
                    new PogoBuildTarget("win64", BuildTarget.StandaloneWindows64, "pogo.exe"),
                    new PogoBuildTarget("linux", BuildTarget.StandaloneLinux64, "pogo.x86.64"),
                };

                BuildTargets(pathRoot, logger, targets);
            };

            Debug.Log("Switching Build Target...");
            EditorUserBuildSettings.SwitchActiveBuildTargetAsync(BuildTargetGroup.Standalone, BuildTarget.StandaloneWindows);
        }

        [MenuItem("Pogo/Build Web (Itch)")]
        public static void BuildItchWeb()
        {
            string pathRoot = $"{Path.GetDirectoryName(Application.dataPath)}{Path.DirectorySeparatorChar}Build{Path.DirectorySeparatorChar}Itch";
            FileLogger logger = new FileLogger($"{pathRoot}{Path.DirectorySeparatorChar}Logs.txt");

            Debug.Log("Switching Build Target...");

            EditorApplication.delayCall += () =>
            {
                Debug.Log("Starting Build...");

                var targets = new PogoBuildTarget[]
                {
                    new PogoBuildTarget("web", BuildTarget.WebGL, "index.html")
                };

                BuildTargets(pathRoot, logger, targets);
            };
            EditorUserBuildSettings.SwitchActiveBuildTargetAsync(BuildTargetGroup.WebGL, BuildTarget.WebGL);
        }

#else
        [MenuItem("Pogo/Build (Steam)")]
        public static void BuildSteam()
        {
            string pathRoot = $"{Path.GetDirectoryName(Application.dataPath)}{Path.DirectorySeparatorChar}Build{Path.DirectorySeparatorChar}Steam";
            FileLogger logger = new FileLogger($"{pathRoot}{Path.DirectorySeparatorChar}Logs.txt");
            
            EditorApplication.delayCall += () =>
            {
                Debug.Log("Starting Build...");

                var targets = new PogoBuildTarget[]
                {
                    new PogoBuildTarget("win32", BuildTarget.StandaloneWindows, "pogo.exe"),
                    new PogoBuildTarget("win64", BuildTarget.StandaloneWindows64, "pogo.exe"),
                    new PogoBuildTarget("linux", BuildTarget.StandaloneLinux64, "pogo.x86.64")
                };

                BuildTargets(pathRoot, logger, targets);
            };

            Debug.Log("Switching Build Target...");
            EditorUserBuildSettings.SwitchActiveBuildTargetAsync(BuildTargetGroup.Standalone, BuildTarget.StandaloneWindows);
        }
#endif

        private static void BuildTargets(string pathRoot, FileLogger logger, PogoBuildTarget[] targets)
        {
            logger.WriteLine("Starting Build");

            List<PogoBuildResult> Results = new List<PogoBuildResult>();

            foreach (var pogoTarget in targets)
            {

                BuildReport report = Build(
                    $"{pathRoot}{Path.DirectorySeparatorChar}{pogoTarget.BuildName}{Path.DirectorySeparatorChar}pogo{Path.DirectorySeparatorChar}{pogoTarget.FileName}",
                    pogoTarget.Target,
                    logger.WriteLine);
                Results.Add(new PogoBuildResult(pogoTarget, report));
            }

            logger.WriteLine("Final Results: ");
            foreach (var result in Results)
            {
                logger.WriteLine($"\t{result.Target.BuildName}: {result.Result}");
            }
        }

        public static BuildReport Build(string path, BuildTarget target, Action<string> logAction)
        {
            logAction?.Invoke($"[{target}] Building to {path}");
            var options = new BuildPlayerOptions
            {
                scenes = GetScenes(),
                target = target,
                locationPathName = path,
            };

            var result = BuildPipeline.BuildPlayer(options);

            string resultText = result.summary.result == UnityEditor.Build.Reporting.BuildResult.Succeeded
                ? "SUCCESS"
                : "FAILURE";

            logAction?.Invoke($"[{target}] Build {resultText}");
            return result;
        }

        private static string[] GetScenes()
        {
            return EditorBuildSettings.scenes
                .Where(s => s.enabled)
                .Select(s => s.path)
                .ToArray();
        }
    }
}