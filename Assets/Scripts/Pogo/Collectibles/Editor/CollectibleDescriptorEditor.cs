using System;
using UnityEditor;
using UnityEngine;
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

        public override VisualElement CreateInspectorGUI()
        {
            self = target as CollectibleDescriptor;
            dropdown = new CollectibleDescriptorManifestAssigner<CollectibleManifest, CollectibleDescriptor>();
            dropdown.OnNoSetManifests.AddListener(dropdown_noSetManifests);
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
            if (self.SceneBuildIndex == -1)
            {
                EditorGUILayout.HelpBox("Lost Connection with Prefab. Please reset descriptor.", MessageType.Error);
            }

            DrawDefaultInspector();

            dropdown.DrawRegisterButtons(self);
            if (self.SceneBuildIndex != -1)
            {
                if (GUILayout.Button("Find Quarter"))
                {

                }
            }
        }
    }
}