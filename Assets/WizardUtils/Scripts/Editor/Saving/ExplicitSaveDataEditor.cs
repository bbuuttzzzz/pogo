using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace WizardUtils.Saving.Inspector
{
    [CustomEditor(typeof(ExplicitSaveData))]
    class ExplicitSaveDataEditor : Editor
    {
        ExplicitSaveData self;

        public override void OnInspectorGUI()
        {
            self = target as ExplicitSaveData;
            DrawDefaultInspector();

            if (EditorGUILayout.DropdownButton(new GUIContent("Add Descriptor..."), FocusType.Passive))
            {
                GenericMenu menu = new GenericMenu();

                var descriptors = self.SaveManifest.SaveValueDescriptors;

                foreach (var descriptor in descriptors)
                {
                    bool alreadyAdded = false;
                    foreach (SaveValue value in self.Data)
                    {
                        if (value.Descriptor == descriptor)
                        {
                            alreadyAdded = true;
                            break;
                        }
                    }
                    if (!alreadyAdded)
                    {
                        menu.AddItem(new GUIContent(descriptor.name, descriptor.DeveloperNote), false, () =>
                        {
                            Array.Resize(ref self.Data, self.Data.Length + 1);
                            self.Data[self.Data.Length - 1] = new SaveValue(descriptor);
                        });
                    }
                }
                menu.DropDown(new Rect(Event.current.mousePosition.x, Event.current.mousePosition.y, 0f, 0f));
            }
        }
    }
}