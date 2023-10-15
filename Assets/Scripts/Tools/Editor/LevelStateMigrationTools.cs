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
        [MenuItem("Pogo/Migration Tools/AreaPortals")]
        public static void MigrateAreaPortals()
        {
            string[] scenePaths = GetAllScenePaths();

            MultiSceneComponentEnumerator<AreaPortal> areaPortals = new MultiSceneComponentEnumerator<AreaPortal>(scenePaths);
            foreach (AreaPortal areaPortal in areaPortals)
            {
                string path = AnimationUtility.CalculateTransformPath(areaPortal.transform, null);
                if (areaPortal.LevelState.Level == areaPortal.Level)
                {
                    Debug.Log($"Skipping already migrated AreaPortal @ {areaPortals.CurrentScenePath} {path}...", areaPortal);
                }
                else
                {
                    Debug.Log($"Updating AreaPortal @ {areaPortals.CurrentScenePath} {path}...", areaPortal);
                    areaPortal.Level = areaPortal.LevelState.Level;
                    EditorUtility.SetDirty(areaPortal);
                }
            }
        }

        private static string[] GetAllScenePaths()
        {
            return EditorBuildSettings.scenes
                .Select(s => s.path)
                .ToArray();
        }
    }
}
