using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using WizardUtils.SceneManagement;

namespace WizardUtils
{

    [CustomEditor(typeof(GameManager))]
    public class GameManagerEditor : Editor
    {
        protected GameManager self;

        public override void OnInspectorGUI()
        {
            self = target as GameManager;
            DrawDefaultInspector();


            if (EditorGUILayout.DropdownButton(new GUIContent("Load Control Scene..."), FocusType.Passive))
            {
                GenericMenu menu = new GenericMenu();

                var controlScenes = AssetDatabase.FindAssets($"t:{nameof(ControlSceneDescriptor)}")
                    .Select(id => AssetDatabase.LoadAssetAtPath<ControlSceneDescriptor>(AssetDatabase.GUIDToAssetPath(id)));
                foreach (var level in controlScenes)
                {
                    menu.AddItem(new GUIContent(level.name), false, () =>
                    {
                        self.LoadControlSceneInEditor(level);
                        OnControlSceneLoadedInEditor();
                    });
                }
                menu.DropDown(new Rect(Event.current.mousePosition.x, Event.current.mousePosition.y, 0f, 0f));
            }
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(self.CurrentControlScene)));
            EditorGUI.EndDisabledGroup();
        }

        public virtual void OnControlSceneLoadedInEditor()
        {
        }
    }
}