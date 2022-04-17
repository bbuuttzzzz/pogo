using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Pogo.Inspector
{
    [CustomPropertyDrawer(typeof(EquipmentDifficulty))]
    public class EquipmentDifficultyPropertyDrawer : EasyPropertyDrawer
    {
        protected override float lineCount => 1;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            var equipmentProperty = property.FindPropertyRelative(nameof(EquipmentDifficulty.Equipment));
            var difficultyProperty = property.FindPropertyRelative(nameof(EquipmentDifficulty.Difficulty));

            SplittablePropertyField[] properties = new SplittablePropertyField[]
            {
                new SplittablePropertyField()
                {
                    Property = equipmentProperty,
                    Label = new GUIContent("Equipment"),
                    LabelFraction = 0.3f,
                    WidthFraction = 0.5f
                },
                new SplittablePropertyField()
                {
                    Property = difficultyProperty,
                    Label = new GUIContent("Difficulty"),
                    LabelFraction = 0.4f,
                    WidthFraction = 0f,
                    ExpandWidth = true
                },
            };

            EasySplitProperty(position, properties);

            EditorGUI.EndProperty();
        }
    }
}
