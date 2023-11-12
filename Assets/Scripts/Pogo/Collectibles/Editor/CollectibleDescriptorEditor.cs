using Pogo.Cosmetics;
using Pogo.Levels;
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

        private SerializedProperty m_CosmeticDescriptor;
        private SerializedProperty m_NotificationPrefab;
        private SerializedProperty m_SpawnGenericNotification;
        private SerializedProperty m_GenericNotificationTitle;
        private SerializedProperty m_GenericNotificationTitle_HalfUnlocked;
        private SerializedProperty m_GenericNotificationBody;
        private SerializedProperty m_GenericNotificationBody_HalfUnlocked;
        private SerializedProperty m_GenericNotification3DIcon;

        public override VisualElement CreateInspectorGUI()
        {
            self = target as CollectibleDescriptor;
            dropdown = new CollectibleDescriptorManifestAssigner<CollectibleManifest, CollectibleDescriptor>();
            dropdown.OnNoSetManifests.AddListener(dropdown_noSetManifests);

            m_CosmeticDescriptor = serializedObject.FindProperty(nameof(self.CosmeticDescriptor));
            m_NotificationPrefab = serializedObject.FindProperty(nameof(self.NotificationPrefab));
            m_SpawnGenericNotification = serializedObject.FindProperty(nameof(self.SpawnGenericNotification));
            m_GenericNotificationTitle = serializedObject.FindProperty(nameof(self.GenericNotificationTitle));
            m_GenericNotificationTitle_HalfUnlocked = serializedObject.FindProperty(nameof(self.GenericNotificationTitle_HalfUnlocked));
            m_GenericNotificationBody = serializedObject.FindProperty(nameof(self.GenericNotificationBody));
            m_GenericNotificationBody_HalfUnlocked = serializedObject.FindProperty(nameof(self.GenericNotificationBody_HalfUnlocked));
            m_GenericNotification3DIcon = serializedObject.FindProperty(nameof(self.GenericNotification3DIcon));

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
            if (self.ExistsInWorld && self.SceneBuildIndex == -1)
            {
                EditorGUILayout.HelpBox("Lost Connection with Prefab. Please reset descriptor.", MessageType.Error);
            }

            DrawDefaultInspector();

            if (self.CollectibleType == CollectibleDescriptor.CollectibleTypes.Cosmetic)
            {
                EditorGUILayout.PropertyField(m_CosmeticDescriptor);
            }
            else
            {
                EditorGUILayout.PropertyField(m_SpawnGenericNotification);
                if (self.SpawnGenericNotification)
                {
                    EditorGUILayout.PropertyField(m_GenericNotificationTitle);
                    EditorGUILayout.PropertyField(m_GenericNotificationTitle_HalfUnlocked);
                    EditorGUILayout.PropertyField(m_GenericNotificationBody);
                    EditorGUILayout.PropertyField(m_GenericNotificationBody_HalfUnlocked);
                    EditorGUILayout.PropertyField(m_GenericNotification3DIcon);
                }
                else
                {
                    EditorGUILayout.PropertyField(m_NotificationPrefab);
                }
            }

            dropdown.DrawRegisterButtons(self);
            if (self.ExistsInWorld && self.SceneBuildIndex != -1)
            {
                if (GUILayout.Button("Find In World"))
                {
                    if (!SceneIsOpen(self.SceneBuildIndex))
                    {
                        OpenScene(self.SceneBuildIndex);
                    }

                    var controller = FindController();
                    Selection.activeObject = controller.transform;
                    EditorApplication.ExecuteMenuItem("Edit/Frame Selected");
                }
            }

            serializedObject.ApplyModifiedProperties();
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