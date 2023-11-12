using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Pogo.Cosmetics
{
    [CustomPropertyDrawer(typeof(VendingMachineEntry))]
    public class VendingMachineEntryDrawer : EasyPropertyDrawer
    {
        protected override float lineCount => 1;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            var thresholdProperty = property.FindPropertyRelative(nameof(VendingMachineEntry.Cost));
            var cosmeticProperty = property.FindPropertyRelative(nameof(VendingMachineEntry.Cosmetic));

            SplittablePropertyField[] properties = new SplittablePropertyField[2]
            {
                new SplittablePropertyField()
                {
                    Property = thresholdProperty,
                    Label = GUIContent.none,
                    WidthFraction = 0.3f
                },
                new SplittablePropertyField()
                {
                    Property = cosmeticProperty,
                    Label = GUIContent.none,
                    ExpandWidth = true,
                }
            };

            EasySplitProperty(position, properties);

            EditorGUI.EndProperty();
        }
    }
}