using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using WizardUtils.ManifestPattern;

namespace Pogo.Collectibles
{
    [CustomEditor(typeof(CollectibleDescriptor)), CanEditMultipleObjects]
    public class CollectibleDescriptorEditor : Editor
    {
        CollectibleDescriptor self;
        public LevelDescriptor level;

        CollectibleDescriptorManifestAssigner<CollectibleManifest, CollectibleDescriptor> dropdown;

        public override VisualElement CreateInspectorGUI()
        {
            self = target as CollectibleDescriptor;
            dropdown = new CollectibleDescriptorManifestAssigner<CollectibleManifest, CollectibleDescriptor>();
            dropdown.OnNoSetManifests.AddListener(dropdown_noSetManifests);
            return base.CreateInspectorGUI();
        }

        private void dropdown_noSetManifests()
        {
            if (self.SceneBuildIndex == -1) { return; }

            self.SceneBuildIndex = -1;
            EditorUtility.SetDirty(self);
        }

        public override void OnInspectorGUI()
        {
            if (self.SceneBuildIndex == -1)
            {
                EditorGUILayout.HelpBox("Lost Connection with Prefab. Please reset descriptor.", MessageType.Error);
            }

            DrawDefaultInspector();

            dropdown.DrawRegisterButtons(self);
            if (self.SceneBuildIndex != -1)
            {
                if (GUILayout.Button("Find In World"))
                {
                    if (!SceneIsOpen(self.SceneBuildIndex))
                    {
                        OpenScene(self.SceneBuildIndex);
                    }

                    var controller = FindController();
                    Selection.activeObject = controller.transform;
                }
            }
        }

        private CollectibleController FindController()
        {
            var collectibles = FindObjectsOfType<CollectibleController>();
            foreach (var collectible in collectibles)
            {
                if (collectible.Descriptor == self)
                {
                    return collectible;
                }
            }

            throw new NullReferenceException("Couldn't find the collectible in currently loaded scenes :(");
        }

        private static void OpenScene(int buildIndex)
        {
            var scenePath = SceneUtility.GetScenePathByBuildIndex(buildIndex);
            UnityEditor.SceneManagement.EditorSceneManager.OpenScene(scenePath, UnityEditor.SceneManagement.OpenSceneMode.Additive);
        }

        private static bool SceneIsOpen(int buildIndex)
        {
            for (int n = 0; n < SceneManager.sceneCount; n++)
            {
                if (SceneManager.GetSceneAt(n).buildIndex == buildIndex)
                {
                    return true;
                }
            }
            return false;
        }
    }
}