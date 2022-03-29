using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace WizardUtils.Equipment.Inspector
{
    [CustomPropertyDrawer(typeof(EquipmentSlot))]
    public class EquipmentSlotPropertyDrawer : EasyPropertyDrawer
    {
        protected override float lineCount => 1;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            var slotProperty = property.FindPropertyRelative(nameof(EquipmentSlot.Slot));
            var equipmentProperty = property.FindPropertyRelative(nameof(EquipmentSlot.Equipment));
            var prefabProperty = property.FindPropertyRelative(nameof(EquipmentSlot.PrefabInstantiationParent));

            SplittablePropertyField[] properties = new SplittablePropertyField[]
            {
                new SplittablePropertyField()
                {
                    Property = slotProperty,
                    Label = GUIContent.none,
                    WidthFraction = 0.2f
                },
                new SplittablePropertyField()
                {
                    Property = equipmentProperty,
                    Label = GUIContent.none,
                    WidthFraction = 0.4f
                },
                new SplittablePropertyField()
                {
                    Property = prefabProperty,
                    Label = GUIContent.none,
                    WidthFraction = 0.4f,
                    ExpandWidth = true
                },
            };

            EasySplitProperty(position, properties);

            EditorGUI.EndProperty();
        }
    }
}
