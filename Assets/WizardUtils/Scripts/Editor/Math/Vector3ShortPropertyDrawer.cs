using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace WizardUtils.Math.Inspector
{
    [CustomPropertyDrawer(typeof(Vector3Short))]
    public class Vector3ShortPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            var propertyX = property.FindPropertyRelative(nameof(Vector3Short.x));
            var propertyY = property.FindPropertyRelative(nameof(Vector3Short.y));
            var propertyZ = property.FindPropertyRelative(nameof(Vector3Short.z));

            (var left, var right) = WizardUtils.RectExtensions.CutRectHorizontally(position, 0.4f);
            EditorGUI.LabelField(left, label);

            float[] fractions = new float[] { 0.333f, 0.333f, 0.333f };
            Rect[] rects = WizardUtils.RectExtensions.SplitRectHorizontally(right, fractions);

            propertyX.intValue = EditorGUI.IntField(rects[0], propertyX.intValue);
            propertyY.intValue = EditorGUI.IntField(rects[1], propertyY.intValue);
            propertyZ.intValue = EditorGUI.IntField(rects[2], propertyZ.intValue);
            //propertyY.intValue = EditorGUI.IntField(right, propertyX.intValue);

            EditorGUI.EndProperty();
        }
    }
}
