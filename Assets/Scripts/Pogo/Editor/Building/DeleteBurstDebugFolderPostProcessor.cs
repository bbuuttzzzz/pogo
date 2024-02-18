using System;
using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.Callbacks;
using UnityEngine;

namespace Pogo.Building
{
    public class DeleteBurstDebugFolderPostProcessor : IPostprocessBuildWithReport
    {
        public int callbackOrder => 1;

        public void OnPostprocessBuild(BuildReport report)
        {
            string folderPrefix = "pogo";
            string buildPath = Path.GetDirectoryName(report.summary.outputPath);

            if (report.summary.platform == BuildTarget.WebGL)
            {
                // we don't care about this on webgl
                return;
            }
            else if (report.summary.platform == BuildTarget.StandaloneLinux64)
            {
                folderPrefix = "pogo.x86";
            }

            string path = $"{buildPath}{Path.DirectorySeparatorChar}{folderPrefix}_BurstDebugInformation_DoNotShip";
            Debug.Log($"Trying to delete {path}");
            Directory.Delete(path, true);
        }
    }
}
