using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Pogo.Cosmetics
{
    [CustomEditor(typeof(VendingMachineManifest))]
    public class VendingMachineManifestEditor : Editor
    {
        private VendingMachineManifest self;

        private SerializedProperty m_Entries;

        public override VisualElement CreateInspectorGUI()
        {
            self = target as VendingMachineManifest;
            m_Entries = serializedObject.FindProperty(nameof(VendingMachineManifest.Entries));
            return base.CreateInspectorGUI();
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.PropertyField(m_Entries);

            var oldArray = self.Entries;
            serializedObject.ApplyModifiedProperties();

            if (!Enumerable.SequenceEqual(oldArray, self.Entries))
            {
                self.Sort();
                EditorUtility.SetDirty(self);
            }
        }
    }

}
