using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using WizardUtils;
using WizardUtils.Math;

namespace Pogo.Levels
{
    [CustomPropertyDrawer(typeof(LevelState))]
    public class LevelStateDrawer : EasyPropertyDrawer
    {
        private int cachedState = 0;

        protected override float lineCount => 1;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            var propertyLevel = property.FindPropertyRelative(nameof(LevelState.Level));
            var propertyState = property.FindPropertyRelative(nameof(LevelState.State));

            Rect[] rects = RectExtensions.SplitRectHorizontally(position, 0.4f, 0.4f, 0.2f);

            int state = propertyState.intValue;

            EasyLabelledPropertyField(rects[0].Margin(5,0), 0.3f, propertyLevel, new GUIContent("Level"));

            using (new EditorGUI.DisabledScope(state < 0))
            {
                EasyLabelledPropertyField(rects[1].Margin(5,0), 0.3f, propertyState, new GUIContent("State"));
            }
            bool toggle;

            toggle = EasyLabelledToggle(rects[2].Margin(5,0), 0.5f, state < 0, new GUIContent("any?"));
            if (!toggle && state < 0)
            {
                propertyState.intValue = cachedState;
            }
            else if (toggle && state >= 0)
            {
                cachedState = state;
                propertyState.intValue = -1;
            }

            EditorGUI.EndProperty();
        }


        protected bool EasyLabelledToggle(Rect position, float labelWidthFraction, bool value, GUIContent label)
        {
            (var left, var right) = WizardUtils.RectExtensions.CutRectHorizontally(position, labelWidthFraction);
            EditorGUI.LabelField(left, label);
            return EditorGUI.Toggle(right, GUIContent.none, value);
        }
    }
}