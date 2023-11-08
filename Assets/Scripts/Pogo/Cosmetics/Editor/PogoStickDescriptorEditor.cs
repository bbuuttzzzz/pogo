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
        public LevelDescriptor level;

        DescriptorManifestAssigner<PogoStickManifest, PogoStickDescriptor> dropdown;

        public override VisualElement CreateInspectorGUI()
        {
            self = target as PogoStickDescriptor;
            dropdown = new DescriptorManifestAssigner<PogoStickManifest, PogoStickDescriptor>();
            return base.CreateInspectorGUI();
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            dropdown.DrawRegisterButtons(self);
        }
    }
}