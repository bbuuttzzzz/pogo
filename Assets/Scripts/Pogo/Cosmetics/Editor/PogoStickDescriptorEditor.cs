using Pogo.Collectibles;
using Pogo.Levels;
using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using WizardUtils.ManifestPattern;

namespace Pogo.Cosmetics
{
    [CustomEditor(typeof(PogoStickDescriptor)), CanEditMultipleObjects]
    public class CollectibleDescriptorEditor : Editor
    {
        PogoStickDescriptor self;
        private SerializedProperty m_Collectible;

        DescriptorManifestAssigner<PogoStickManifest, PogoStickDescriptor> dropdown;


        public override VisualElement CreateInspectorGUI()
        {
            self = target as PogoStickDescriptor;
            dropdown = new DescriptorManifestAssigner<PogoStickManifest, PogoStickDescriptor>();
            m_Collectible = serializedObject.FindProperty(nameof(self.Collectible));
            return base.CreateInspectorGUI();
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            
            if (self.UnlockType == PogoStickDescriptor.UnlockTypes.Collectible)
            {
                EditorGUILayout.PropertyField(m_Collectible);
            }

            dropdown.DrawRegisterButtons(self);
            serializedObject.ApplyModifiedProperties();
        }
    }
}