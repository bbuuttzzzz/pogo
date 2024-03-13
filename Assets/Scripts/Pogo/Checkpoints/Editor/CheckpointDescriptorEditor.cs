using Pogo.Checkpoints;
using Pogo.Levels;
using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using WizardUtils.ManifestPattern;

namespace Pogo.Collectibles
{
    [CustomEditor(typeof(CheckpointDescriptor)), CanEditMultipleObjects]
    public class CheckpointDescriptorEditor : Editor
    {
        CheckpointDescriptor self;
        public LevelDescriptor level;

        public override VisualElement CreateInspectorGUI()
        {
            self = target as CheckpointDescriptor;
            return base.CreateInspectorGUI();
        }

        public override void OnInspectorGUI()
        {
            bool corrupt = false;
            try
            {
                if (self.Chapter.GetCheckpointDescriptor(self.CheckpointId) != self)
                {
                    corrupt = true;
                }
            }
            catch(Exception e)
            {
                corrupt = true;
            }

            if (corrupt)
            {
                EditorGUILayout.HelpBox("Corrupted CheckpointId/Chapter!", MessageType.Error);
            }


            DrawDefaultInspector();

            if (!corrupt)
            {
                if (GUILayout.Button("Find In World"))
                {
                    var manager = UnityEngine.Object.FindFirstObjectByType<PogoLevelManager>();
                    if (manager.CurrentLevel != self.MainLevelState.Level)
                    {
                        if (!UnityEditor.SceneManagement.EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                        {
                            return;
                        }
                        manager.LoadLevelInEditor(self.MainLevelState.Level);
                    }

                    var worldObject = FindWorldObject();
                    Selection.activeObject = worldObject.transform;
                    EditorApplication.ExecuteMenuItem("Edit/Frame Selected");
                }
            }
        }

        private ExplicitCheckpoint FindWorldObject()
        {
            var checkpointTriggers = FindObjectsOfType<ExplicitCheckpoint>();
            foreach (var checkpointTrigger in checkpointTriggers)
            {
                if (checkpointTrigger.Descriptor == self)
                {
                    return checkpointTrigger;
                }
            }

            throw new NullReferenceException($"Couldn't find checkpoint {self.CheckpointId} in currently loaded scenes :(");
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