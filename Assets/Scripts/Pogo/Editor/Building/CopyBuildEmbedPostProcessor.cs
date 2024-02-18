using System;
using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace Pogo.Building
{
    public static class CopyBuildEmbedPostProcessor
    {
        [PostProcessBuild(1)]
        public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject)
        {
            if (target == BuildTarget.WebGL) return;

            string sourcePath = $"{Application.dataPath}{Path.DirectorySeparatorChar}BuildEmbedRoot";
            string destinationPath = $"{pathToBuiltProject}{Path.DirectorySeparatorChar}pogo_Data";
            CopyFilesRecursively(sourcePath, pathToBuiltProject);
        }

        private static void CopyFilesRecursively(string sourceDir, string destDir)
        {
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
                CopyFilesRecursively(subDir, destSubDir);
            }
        }
    }
}
