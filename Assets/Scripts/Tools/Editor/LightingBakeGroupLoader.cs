using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Pogo.Tools
{
    public static class LightingBakeGroupLoader
    {
        [MenuItem("Pogo/Baking/Load Lighting Bake Group...")]
        public static void OpenLightingBakeGroupWizard()
        {
            LightingBakeGroupSelectWizard.Spawn();
        }

        public static void OpenLightingBakeGroup(LightingBakeGroup group)
        {
            EditorSceneManager.OpenScene(GetScenePath(group.MainScene), OpenSceneMode.Single);
            EditorSceneManager.OpenScene("Assets/Project/misc/GameScene.unity", OpenSceneMode.Additive);

            foreach (var otherScene in group.OtherScenes)
            {
                EditorSceneManager.OpenScene(GetScenePath(otherScene), OpenSceneMode.Additive);
            }
        }

        public static string GetScenePath(SceneAsset sceneAsset)
        {
            return AssetDatabase.GetAssetPath(sceneAsset);
        }
    }
}
