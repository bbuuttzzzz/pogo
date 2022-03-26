using System;
using UnityEditor;
using UnityEngine;


public abstract class EasyPropertyDrawer : PropertyDrawer
{
    protected abstract float lineCount { get; }
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUIUtility.singleLineHeight * lineCount + (2 * lineCount - 1);
    }

    protected bool EasyLabelledPropertyField(Rect position, float labelWidthFraction, SerializedProperty property, GUIContent label)
    {
        (var left, var right) = WizardUtils.RectExtensions.CutRectHorizontally(position, labelWidthFraction);
        EditorGUI.LabelField(left, label);
        return EditorGUI.PropertyField(right, property, GUIContent.none);
    }

    protected void EasySplitProperty(Rect position, SplittablePropertyField[] splittableProperties)
    {
        // first we need to figure out how big the expand should be if we have one
        SplittablePropertyField expandingProperty = null;
        float total = 0;
        foreach(SplittablePropertyField field in splittableProperties)
        {
            if (field.ExpandWidth)
            {
                expandingProperty = field;
            }
            total += field.WidthFraction;
        }
        if (expandingProperty == null)
        {
            expandingProperty = splittableProperties[splittableProperties.Length - 1];
        }

        float expandFraction = Mathf.Max(expandingProperty.WidthFraction, (1f - total));
        if (total + expandFraction > 1 + 1E7)
        {
            Debug.LogWarning("Oversizes SplittablePropertyField");
        }

        float[] partFractions = new float[splittableProperties.Length];
        for (int n = 0; n < splittableProperties.Length; n++)
        {
            var field = splittableProperties[n];
            partFractions[n] = field == expandingProperty? expandFraction : field.WidthFraction;
        }

        Rect[] shapes = WizardUtils.RectExtensions.SplitRectHorizontally(position, partFractions);


        for (int n = 0; n < splittableProperties.Length; n++)
        {
            var field = splittableProperties[n];

            if (field.Label != GUIContent.none && field.Label != null)
            {
                bool result = EasyLabelledPropertyField(shapes[n], field.LabelFraction, field.Property, field.Label);
                field.OnPropertyExpanded?.Invoke(result);
            }
            else
            {
                bool result = EditorGUI.PropertyField(shapes[n], field.Property, field.Label);
                field.OnPropertyExpanded?.Invoke(result);
            }
        }
    }

    protected Rect getFirstRow(Rect position)
    {
        return new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
    }

    protected Rect incrementRow(Rect position)
    {
        return new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight + 2, position.width, position.height);
    }

    protected class SplittablePropertyField
    {
        public SerializedProperty Property;
        public GUIContent Label;
        public float LabelFraction = 0.5f;
        public Action<bool> OnPropertyExpanded;
        public float WidthFraction;
        public bool ExpandWidth;
    }
}
