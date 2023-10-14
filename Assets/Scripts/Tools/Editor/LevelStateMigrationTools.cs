using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Pogo.Tools
{
    public static class LevelStateMigrationTools
    {

        private static string[] GetAllScenePaths()
        {
            return EditorBuildSettings.scenes
                .Select(s => s.path)
                .ToArray();
        }
    }
}
