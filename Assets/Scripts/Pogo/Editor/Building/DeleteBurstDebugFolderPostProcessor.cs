using System;
using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.Callbacks;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

namespace Pogo.Building
{
    public class DeleteBurstDebugFolderPostProcessor : IPostprocessBuildWithReport
    {
        public int callbackOrder => 1;

        public void OnPostprocessBuild(BuildReport report)
        {
            string folderPrefix = "pogo";
            string buildPath = report.summary.outputPath;

            if (report.summary.platform == BuildTarget.WebGL)
            {
                // we don't care about this on webgl
                return;
            }
            else if (report.summary.platform == BuildTarget.StandaloneLinux64)
            {
                // linux build points to the executable instead of the root folder :^)
                buildPath = Path.GetDirectoryName(buildPath);
                folderPrefix = "pogo.x86";
            }

            string path = $"{buildPath}{Path.DirectorySeparatorChar}{folderPrefix}_BurstDebugInformation_DoNotShip";
            Debug.Log($"Trying to delete {path}");
            Directory.Delete(path, true);
        }
    }
}
