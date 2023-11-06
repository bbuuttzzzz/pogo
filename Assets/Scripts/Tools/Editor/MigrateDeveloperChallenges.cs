using Pogo.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Tools.Editor
{
    public static class Asdf
    {
        [MenuItem("Pogo/Migrate DeveloperChallenges")]
        public static void MigrateChallenges()
        {
            using (new UndoScope("migrate challenges"))
            {
                int count = 0;
                foreach (var devChallenge in new AssetEnumerator<DeveloperChallenge>())
                {
                    count++;
                    Debug.Log($"Migrating devChallenge {devChallenge.name}");
                    Undo.RecordObject(devChallenge, "migrate challenge");
                    devChallenge.Challenge.SharePositionCm = devChallenge.Challenge.StartPointCm;
                    EditorUtility.SetDirty(devChallenge);
                }

                Debug.Log($"Finished migrating {count} developerChallenges :D");
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }
    }

}
