using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

namespace Pogo.Levels
{
    [CustomEditor(typeof(LevelDescriptor))]
    public class LevelDescriptorEditor : Editor
    {
        LevelDescriptor self;

        public override VisualElement CreateInspectorGUI()
        {
            self = target as LevelDescriptor;
            return base.CreateInspectorGUI();
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
        }
    }
}