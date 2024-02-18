using System;
using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace Pogo.Building
{
    public static class DeleteBurstDebugFolderPostProcessor
    {
        [PostProcessBuild(1)]
        public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject)
        {
            if (target == BuildTarget.WebGL) return;
            Directory.Delete($"{pathToBuiltProject}{Path.DirectorySeparatorChar}pogo_BurstDebugInformation_DoNotShip", true);
        }
    }
}
