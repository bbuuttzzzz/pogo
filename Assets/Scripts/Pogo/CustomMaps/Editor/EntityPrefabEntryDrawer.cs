using Pogo.CustomMaps;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using WizardUtils;

[CustomPropertyDrawer(typeof(EntityPrefabEntry))]
public class EntityPrefabEntryDrawer : EasyPropertyDrawer
{
    protected override float lineCount => 1;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);


        var nameProperty = property.FindPropertyRelative(nameof(EntityPrefabEntry.ClassName));
        var prefabProperty = property.FindPropertyRelative(nameof(EntityPrefabEntry.Prefab));

        SplittablePropertyField[] properties = new SplittablePropertyField[]
        {
            new SplittablePropertyField()
            {
                Property = nameProperty,
                Label = GUIContent.none,
                WidthFraction = 0.4f
            },
            new SplittablePropertyField()
            {
                Property = prefabProperty,
                Label = GUIContent.none,
                ExpandWidth = true,
            },
        };

        EasySplitProperty(position, properties);

        EditorGUI.EndProperty();
    }
}
