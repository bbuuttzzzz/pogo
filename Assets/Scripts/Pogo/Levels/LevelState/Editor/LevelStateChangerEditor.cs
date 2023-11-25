using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Pogo.Levels
{
    [CustomEditor(typeof(LevelStateChanger))]
    public class LevelStateChangerEditor : Editor
    {
        private LevelStateChanger self;

        public override VisualElement CreateInspectorGUI()
        {
            self = target as LevelStateChanger;
            return base.CreateInspectorGUI();
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            using (new EditorGUI.DisabledScope(!Application.isPlaying))
            {
                for(int n = 0; n < self.Level.LevelStatesCount; n++)
                {
                    if (GUILayout.Button($"Go To State {n}"))
                    {
                        self.GoToState(n);
                    }
                }
            }

        }
    }
}