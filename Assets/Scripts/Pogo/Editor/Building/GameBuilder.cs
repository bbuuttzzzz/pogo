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
            public string Extension;

            public PogoBuildTarget(string buildName, BuildTarget target, string extension)
            {
                BuildName = buildName;
                Target = target;
                Extension = extension;
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
            }

            public void WriteLine(string message)
            {
                Debug.Log(message);
                var writer = File.AppendText(logPath);
                writer.WriteLine(message);
                writer.Dispose();
            }
        }

        public static void BuildAll()
        {
            var parser = new CommandLineArgsParser();
            if (!parser.TryGetArg("-pathRoot", out string pathRoot))
            {
                Debug.LogError("Missing REQUIRED argument -pathRoot <value>");
                return;
            }
            if (!parser.TryGetArg("-shortLogs", out string myLogPath))
            {
                myLogPath = $"{pathRoot}{Path.DirectorySeparatorChar}Logs.txt";
            }

            FileLogger logger = new FileLogger(myLogPath);
            BuildShared(pathRoot, logger);
        }

        [MenuItem("Pogo/Build All")]
        public static void BuildAllFromEditor()
        {
            string pathRoot = $"{Path.GetDirectoryName(Application.dataPath)}{Path.DirectorySeparatorChar}Build";
            FileLogger logger = new FileLogger($"{pathRoot}{Path.DirectorySeparatorChar}Logs.txt");

            BuildShared(pathRoot, logger);
        }

        private static void BuildShared(string pathRoot, FileLogger logger)
        {
            logger.WriteLine("Starting Build");

            PogoBuildTarget[] pogoTargets = new PogoBuildTarget[]
            {
            new PogoBuildTarget("win32", BuildTarget.StandaloneWindows, ".exe"),
            new PogoBuildTarget("win64", BuildTarget.StandaloneWindows64, ".exe"),
            new PogoBuildTarget("linux64", BuildTarget.StandaloneLinux64, "x86.64"),
            };

            List<PogoBuildResult> Results = new List<PogoBuildResult>();

            foreach (var pogoTarget in pogoTargets)
            {
                BuildReport report = Build(
                    $"{pathRoot}{Path.DirectorySeparatorChar}{pogoTarget.BuildName}{Path.DirectorySeparatorChar}pogo{Path.DirectorySeparatorChar}pogo{pogoTarget.Extension}",
                    pogoTarget.Target,
                    logger.WriteLine);
                Results.Add(new PogoBuildResult(pogoTarget, report));
            }

            foreach (var result in Results)
            {
                logger.WriteLine("Final Results: ");

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
                .Select(s => s.path)
                .ToArray();
        }
    }
}