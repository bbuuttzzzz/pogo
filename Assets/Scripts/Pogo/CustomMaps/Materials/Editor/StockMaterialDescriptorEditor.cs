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
    [CustomEditor(typeof(StockMaterialDescriptor)), CanEditMultipleObjects]
    public class StockMaterialDescriptorEditor : Editor
    {
        private DescriptorManifestAssigner<StockMaterialManifest, StockMaterialDescriptor> dropdown;
        public override VisualElement CreateInspectorGUI()
        {
            dropdown = new DescriptorManifestAssigner<StockMaterialManifest, StockMaterialDescriptor>();
            return base.CreateInspectorGUI();
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            dropdown.DrawRegisterButtons(targets.Cast<StockMaterialDescriptor>().ToArray());

            serializedObject.ApplyModifiedProperties();
        }
    }
}
