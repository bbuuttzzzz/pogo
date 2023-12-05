using Pogo.Collectibles;
using Pogo.Levels;
using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using WizardUtils.ManifestPattern;

namespace Pogo.Cosmetics
{
    [CustomEditor(typeof(CosmeticSlotManifest), true), CanEditMultipleObjects]
    public class CosmeticSlotManifestEditor : Editor
    {
        private CosmeticSlotManifest self;
        private DescriptorManifestAssigner<CosmeticManifest, CosmeticSlotManifest> dropdown;


        public override VisualElement CreateInspectorGUI()
        {
            self = target as CosmeticSlotManifest;
            dropdown = new DescriptorManifestAssigner<CosmeticManifest, CosmeticSlotManifest>();
            return base.CreateInspectorGUI();
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            dropdown.DrawRegisterButtons(targets.Cast<CosmeticSlotManifest>().ToArray());

            serializedObject.ApplyModifiedProperties();
        }
    }
}