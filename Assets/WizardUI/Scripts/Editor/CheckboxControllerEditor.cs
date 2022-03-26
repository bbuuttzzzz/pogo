using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace WizardUtils
{
    [CustomEditor(typeof(CheckboxController))]
    public class CheckboxControllerEditor : Editor
    {
        CheckboxController self;

        public override void OnInspectorGUI()
        {
            self = target as CheckboxController;
            DrawDefaultInspector();

            bool wasToggled = self.Toggled;
            bool isToggledNow = EditorGUILayout.Toggle("Toggled", wasToggled);
            if (wasToggled != isToggledNow)
            {
                self.Toggle();
                Undo.RecordObject(self, "Toggle");
                PrefabUtility.RecordPrefabInstancePropertyModifications(self);
            }
        }
    }
}