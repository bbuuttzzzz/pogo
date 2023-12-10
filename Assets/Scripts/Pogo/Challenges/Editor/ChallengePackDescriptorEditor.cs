using Pogo.Cosmetics;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using WizardUtils.ManifestPattern;

namespace Pogo
{
    [CustomEditor(typeof(ChallengePackDescriptor)), CanEditMultipleObjects]
    public class ChallengePackDescriptorEditor : Editor
    {
        private DescriptorManifestAssigner<ChallengePackManifest, ChallengePackDescriptor> dropdown;
        public override VisualElement CreateInspectorGUI()
        {
            dropdown = new DescriptorManifestAssigner<ChallengePackManifest, ChallengePackDescriptor>();
            return base.CreateInspectorGUI();
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            dropdown.DrawRegisterButtons(targets.Cast<ChallengePackDescriptor>().ToArray());

            serializedObject.ApplyModifiedProperties();
        }
    }
}
