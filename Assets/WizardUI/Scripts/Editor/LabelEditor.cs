using UnityEditor;
using UnityEngine;

namespace WizardUI.Inspector
{
    [CustomEditor(typeof(Label))]
    class LabelEditor : Editor
    {
        Label self;

        public override void OnInspectorGUI()
        {
            self = target as Label;
            DrawDefaultInspector();

            
        }
    }
}
