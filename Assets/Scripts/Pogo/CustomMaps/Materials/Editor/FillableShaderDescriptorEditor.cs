using Pogo.Cosmetics;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using WizardUtils.ManifestPattern;

namespace Pogo.CustomMaps.Materials
{
    [CustomEditor(typeof(FillableShaderDescriptor)), CanEditMultipleObjects]
    public class FillableShaderDescriptorEditor : Editor
    {
        private DescriptorManifestAssigner<FillableShaderManifest, FillableShaderDescriptor> dropdown;
        public override VisualElement CreateInspectorGUI()
        {
            dropdown = new DescriptorManifestAssigner<FillableShaderManifest, FillableShaderDescriptor>();
            return base.CreateInspectorGUI();
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            dropdown.DrawRegisterButtons(targets.Cast<FillableShaderDescriptor>().ToArray());

            serializedObject.ApplyModifiedProperties();
        }
    }
}
