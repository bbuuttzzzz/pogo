using System;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.Callbacks;
using UnityEngine;

namespace Pogo.Building
{
    public class CopyBuildEmbedPostProcessor : IPostprocessBuildWithReport
    {
        public int callbackOrder => 1;
        public void OnPostprocessBuild(BuildReport report)
        {
            string buildPath = Path.GetDirectoryName(report.summary.outputPath);

            if (report.summary.platform == BuildTarget.WebGL)
            {
                // we don't care about this on webgl
                return;
            }

            string sourcePath = $"{Application.dataPath}{Path.DirectorySeparatorChar}BuildEmbedRoot";
            string destinationPath = buildPath;
            CopyFilesRecursivelyIgnoringMetaFiles(sourcePath, destinationPath);
        }

        private static void CopyFilesRecursivelyIgnoringMetaFiles(string sourceDir, string destDir)
        {
            Regex regex = new Regex(".*\\.meta", RegexOptions.Compiled);

            // Check if the source directory exists
            if (!Directory.Exists(sourceDir))
            {
                throw new IOException("Source directory does not exist.");
            }

            // Check if the destination directory exists, if not, create it
            if (!Directory.Exists(destDir))
            {
                Directory.CreateDirectory(destDir);
            }

            // Copy files
            string[] files = Directory.GetFiles(sourceDir);
            foreach (string file in files)
            {
                // ignore meta files
                if (regex.Match(file).Success)
                {
                    continue;
                }

                string fileName = Path.GetFileName(file);
                string destFile = Path.Combine(destDir, fileName);
                File.Copy(file, destFile, true);
            }

            // Recursively copy files in subdirectories
            string[] subDirectories = Directory.GetDirectories(sourceDir);
            foreach (string subDir in subDirectories)
            {
                string subDirName = Path.GetFileName(subDir);
                string destSubDir = Path.Combine(destDir, subDirName);
                CopyFilesRecursivelyIgnoringMetaFiles(subDir, destSubDir);
            }
        }
    }
}
