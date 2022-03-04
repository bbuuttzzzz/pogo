using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine;

namespace WizardUI.Inspector
{
    [CustomEditor(typeof(MenuController))]
    class MenuControllerEditor : Editor
    {
        MenuController self;

        public override void OnInspectorGUI()
        {
            self = target as MenuController;
            DrawDefaultInspector();

            bool wasOpen = self.IsOpen;
            bool isOpenNow = EditorGUILayout.Toggle("Menu Open", wasOpen);
            if (wasOpen != isOpenNow)
            {
                self.SetOpen(isOpenNow);
                Undo.RecordObject(self, "Toggle Menu");
                PrefabUtility.RecordPrefabInstancePropertyModifications(self);
            }
        }
    }
}