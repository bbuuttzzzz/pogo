
using UnityEditor;
using UnityEngine.UIElements;

namespace Pogo
{
    [CustomEditor(typeof(AreaPortal), true)]
    public class AreaPortalEditor : Editor
    {
        private AreaPortal self;

        private SerializedProperty m_shouldSetLevelState;
        private SerializedProperty m_LevelState;

        public override VisualElement CreateInspectorGUI()
        {
            self = target as AreaPortal;
            m_shouldSetLevelState = serializedObject.FindProperty(nameof(self.ShouldSetLevelState));
            m_LevelState = serializedObject.FindProperty(nameof(self.LevelState));
            return base.CreateInspectorGUI();
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EditorGUILayout.PropertyField(m_shouldSetLevelState);
            if (self.ShouldSetLevelState)
            {
                EditorGUILayout.PropertyField (m_LevelState);
            }
            serializedObject.ApplyModifiedProperties();
        }
    }
}
