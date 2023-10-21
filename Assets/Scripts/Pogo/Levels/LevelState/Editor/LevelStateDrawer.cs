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
            var propertyState = property.FindPropertyRelative(nameof(LevelState.StateId));

            Rect[] rects = RectExtensions.SplitRectHorizontally(position, 0.4f, 0.25f, 0.2f, 0.15f);

            int state = propertyState.intValue;

            EditorGUI.LabelField(rects[0], label);

            EditorGUI.PropertyField(rects[1], propertyLevel,GUIContent.none);

            using (new EditorGUI.DisabledScope(state < 0))
            {
                EasyLabelledPropertyField(rects[2].Margin(5,0), 0.6f, propertyState, new GUIContent(nameof(LevelState.StateId)));
            }
            bool toggle;

            toggle = EasyLabelledToggle(rects[3].Margin(0,0,0,5), 0.5f, state < 0, new GUIContent("any?"));
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