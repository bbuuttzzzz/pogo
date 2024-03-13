using Pogo.Cosmetics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using WizardUtils.ManifestPattern;

namespace Pogo.CustomMaps.Materials
{
    [CustomEditor(typeof(SkyMaterialDescriptor)), CanEditMultipleObjects]
    public class SkyMaterialDescriptorEditor : Editor
    {
        private DescriptorManifestAssigner<SkyMaterialManifest, SkyMaterialDescriptor> dropdown;
        public override VisualElement CreateInspectorGUI()
        {
            foreach (var _target in targets)
            {
                var descriptor = _target as SkyMaterialDescriptor;
                if (descriptor.FillableShader == null) continue;

                if (descriptor.PropertyValues == null)
                {
                    descriptor.PropertyValues = new string[descriptor.FillableShader.Properties.Length];
                }
                else if (descriptor.PropertyValues.Length != descriptor.FillableShader.Properties.Length)
                {
                    Array.Resize(ref descriptor.PropertyValues, descriptor.FillableShader.Properties.Length);
                }
            }

            dropdown = new DescriptorManifestAssigner<SkyMaterialManifest, SkyMaterialDescriptor>();
            return base.CreateInspectorGUI();
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            DrawProperties();

            dropdown.DrawRegisterButtons(targets.Cast<SkyMaterialDescriptor>().ToArray());

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawProperties()
        {
            GUILayout.Label("Properties");
            FillableShaderDescriptor firstShader = (targets[0] as SkyMaterialDescriptor).FillableShader;

            if (firstShader == null)
            {
                EditorGUILayout.HelpBox("Missing FillableShader", MessageType.Warning);
                return;
            }

            if (targets.Any(t => (t as SkyMaterialDescriptor).FillableShader != firstShader))
            {
                EditorGUILayout.HelpBox("Multi-Editing is not supported for SkyMaterials for different shaders", MessageType.Info);
                return;
            }

            for (int n = 0; n < firstShader.Properties.Length; n++)
            {
                (bool mixed, string value) = GetProperty(firstShader, n);

                FillableShaderProperty property = firstShader.Properties[n];

                EditorGUI.showMixedValue = mixed;
                string newValue = EditorGUILayout.TextField($"{property.ParameterName} ({property.ParameterType}):", value);
                EditorGUI.showMixedValue = false;

                if (newValue != value)
                {
                    SetProperty(n, newValue);
                }
            }
        }

        private (bool mixed, string value) GetProperty(FillableShaderDescriptor shader, int index)
        {
            var firstTarget = targets[0] as SkyMaterialDescriptor;
            FillableShaderProperty property = shader.Properties[index];
            if (targets.Cast<SkyMaterialDescriptor>().Any(t =>  t.PropertyValues[index] != firstTarget.PropertyValues[index]))
            {
                return (true, "");
            }
            else
            {
                return (false, firstTarget.PropertyValues[index]);
            }
        }

        private void SetProperty(int index, string value)
        {
            using (new UndoScope("Set Material Property"))
            {
                foreach (SkyMaterialDescriptor target in targets.Cast<SkyMaterialDescriptor>())
                {
                    Undo.RecordObject(target, "Set Material Property");
                    target.PropertyValues[index] = value;
                    EditorUtility.SetDirty(target);
                }
            }
        }
    }
}
