using PlasticGui.WorkspaceWindow.BrowseRepository;
using Pogo.Levels;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using WizardUtils.ManifestPattern;

namespace Pogo.Collectibles
{
    [CustomEditor(typeof(CollectibleController)), CanEditMultipleObjects]
    public class CollectibleControllerEditor : Editor
    {
        CollectibleController self;
        public LevelDescriptor level;

        SerializedProperty m_Descriptor;

        private void OnEnable()
        {
            m_Descriptor = serializedObject.FindProperty(nameof(self.Descriptor));
        }

        public override VisualElement CreateInspectorGUI()
        {
            self = target as CollectibleController;
            return base.CreateInspectorGUI();
        }

        public override void OnInspectorGUI()
        {
            if (self.Descriptor == null)
            {
                EditorGUILayout.HelpBox("Missing Descriptor!", MessageType.Error);
            }
            else if (self.Descriptor != null && self.Descriptor.SceneBuildIndex != self.gameObject.scene.buildIndex)
            {
                EditorGUILayout.HelpBox($"Descriptor points to scene {self.Descriptor.SceneBuildIndex}! (expected {self.gameObject.scene.buildIndex})", MessageType.Error);
                
                if (GUILayout.Button("Fix Now"))
                {
                    Undo.RecordObject(self.Descriptor, "set Scene Data");
                    self.Descriptor.SceneBuildIndex = self.gameObject.scene.buildIndex;
                    EditorUtility.SetDirty(self.Descriptor);
                    AssetDatabase.SaveAssetIfDirty(self.Descriptor);
                }
            }

            DrawDefaultInspector();
        }
    }
}