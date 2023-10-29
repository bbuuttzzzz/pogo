using Pogo.Checkpoints;
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
        [MenuItem("Pogo/Migration Tools/Checkpoints")]
        public static void MigrateCheckpoints()
        {

            var pathPairs = AssetDatabase.FindAssets($"t:{nameof(CheckpointDescriptor)}")
                .Select(id =>
                {
                    var path = AssetDatabase.GUIDToAssetPath(id);
                    return new
                    {
                        Path = path,
                        Asset = AssetDatabase.LoadAssetAtPath<CheckpointDescriptor>(path)
                    };
                });

            foreach (var pathPair in pathPairs)
            {
                if (pathPair.Asset.LevelState == pathPair.Asset.MainLevelState)
                {
                    Debug.Log($"Skipping already migrated checkpoint @ {pathPair.Path}");
                }
                else
                {
                    Debug.Log($"Migrating checkpoint @ {pathPair.Path}");
                    pathPair.Asset.MainLevelState = pathPair.Asset.LevelState;

                    EditorUtility.SetDirty(pathPair.Asset);
                }
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private static string[] GetAllScenePaths()
        {
            return EditorBuildSettings.scenes
                .Select(s => s.path)
                .ToArray();
        }
    }
}
