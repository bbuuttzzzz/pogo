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
    [CustomEditor(typeof(CosmeticDescriptor), true), CanEditMultipleObjects]
    public class CosmeticDescriptorEditor : Editor
    {
        private CosmeticDescriptor self;
        private SerializedProperty m_Collectible;
        private DescriptorManifestAssigner<CosmeticSlotManifest, CosmeticDescriptor> dropdown;


        public override VisualElement CreateInspectorGUI()
        {
            self = target as CosmeticDescriptor;
            m_Collectible = serializedObject.FindProperty(nameof(self.Collectible));
            dropdown = new DescriptorManifestAssigner<CosmeticSlotManifest, CosmeticDescriptor>();
            return base.CreateInspectorGUI();
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            if (self.UnlockType == CosmeticDescriptor.UnlockTypes.Collectible)
            {
                EditorGUILayout.PropertyField(m_Collectible);
            }
            dropdown.DrawRegisterButtons(self);

            serializedObject.ApplyModifiedProperties();
        }
    }
}