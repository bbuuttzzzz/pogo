using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using WizardUtils.Saving;

namespace Pogo
{
    [CustomPropertyDrawer(typeof(RespawnPointId))]
    public class RespawnPointIdDrawer : EasyPropertyDrawer
    {
        protected override float lineCount => 1;

        private SplittablePropertyField[] properties;

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            properties = new SplittablePropertyField[]
            {
                new SplittablePropertyField()
                {
                    Property = property.FindPropertyRelative(nameof(RespawnPointId.Index)),
                    Label = GUIContent.none,
                    WidthFraction = 0.7f
                },
                new SplittablePropertyField()
                {
                    Property = property.FindPropertyRelative(nameof(RespawnPointId.IsSpecial)),
                    Label = new GUIContent("[?]", "Is this outside the regular sequence of checkpoints?"),
                    LabelFraction = 0.2f,
                    ExpandWidth = true
                }
            };

            return base.CreatePropertyGUI(property);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            EasySplitProperty(position, properties);
            EditorGUI.EndProperty();
        }
    }
}
