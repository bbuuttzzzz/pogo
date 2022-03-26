using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace WizardUtils.Saving.Inspector
{
    [CustomPropertyDrawer(typeof(SaveValue))]
    public class SaveValuePropertyDrawer : EasyPropertyDrawer
    {
        protected override float lineCount => 1;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            var descriptorProperty = property.FindPropertyRelative(nameof(SaveValue.Descriptor));
            var developerNote = (descriptorProperty.objectReferenceValue as SaveValueDescriptor)?.DeveloperNote;

            SplittablePropertyField[] properties = new SplittablePropertyField[]
            {
                new SplittablePropertyField()
                {
                    Property = descriptorProperty,
                    Label = GUIContent.none,
                    WidthFraction = 0.7f
                },
                new SplittablePropertyField()
                {
                    Property = property.FindPropertyRelative(nameof(SaveValue.StringValue)),
                    Label = new GUIContent("[?]", developerNote??"idk"),
                    LabelFraction = 0.2f,
                    WidthFraction = 0f,
                    ExpandWidth = true
                },
            };

            EasySplitProperty(position, properties);

            EditorGUI.EndProperty();
        }
    }
}
