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
            DrawDefaultInspector();
            EditorGUILayout.PropertyField(m_Descriptor);

            if (serializedObject.ApplyModifiedProperties())
            {
                if (self.Descriptor != null)
                {
                    Undo.RecordObject(self.Descriptor, "set Scene Data");
                    self.Descriptor.SceneBuildIndex = self.gameObject.scene.buildIndex;
                    EditorUtility.SetDirty(self.Descriptor);
                }
            }
        }
    }
}