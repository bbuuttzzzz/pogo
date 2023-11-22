using Pogo.Collectibles;
using Pogo.Levels;
using Pogo.Tools;
using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using WizardUtils.ManifestPattern;
using WizardUtils.SerializedObjectHelpers;

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

            if (GUILayout.Button("Clone..."))
            {
                CosmeticDescriptorCloneWizard.Spawn(self);
            }

            dropdown.DrawRegisterButtons(self);

            SerializedObjectUpdater updater = new SerializedObjectUpdater(serializedObject);
            updater.Add(
                get: () => self.Collectible,
                onChangedCallback: OnCollectibleChanged);

            updater.ApplyModifiedProperties();
        }

        private void OnCollectibleChanged(SerializedPropertyChangedArgs<CollectibleDescriptor> args)
        {
            Undo.RecordObject(args.NewValue, "Update Collectible");
            args.NewValue.CollectibleType = CollectibleDescriptor.CollectibleTypes.Cosmetic;
            args.NewValue.CosmeticDescriptor = self;
            EditorUtility.SetDirty(args.NewValue);
            AssetDatabase.SaveAssetIfDirty(args.NewValue);
        }
    }
}