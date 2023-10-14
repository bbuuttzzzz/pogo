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
        [MenuItem("Pogo/Migration Tools/Challenges")]
        public static void MigrateChallenges()
        {

            var pathPairs = AssetDatabase.FindAssets($"t:{nameof(DeveloperChallenge)}")
                .Select(id =>
                {
                    var path = AssetDatabase.GUIDToAssetPath(id);
                    return new
                    {
                        Path = path,
                        Asset = AssetDatabase.LoadAssetAtPath<DeveloperChallenge>(path)
                    };
                });

            foreach (var pathPair in pathPairs)
            {
                if (pathPair.Asset.Challenge.LevelState.Level == pathPair.Asset.Challenge.Level)
                {
                    Debug.Log($"Skipping already migrated developerChallenge @ {pathPair.Path}");
                }
                else
                {
                    Debug.Log($"Migrating developerChallenge @ {pathPair.Path}");
                    pathPair.Asset.Challenge.LevelState = new Levels.LevelState()
                    {
                        Level = pathPair.Asset.Challenge.Level,
                        StateId = 0
                    };

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
