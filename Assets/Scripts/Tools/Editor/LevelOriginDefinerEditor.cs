using Pogo.Checkpoints;
using Pogo.Levels;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace Pogo.Tools
{
    [CustomEditor(typeof(LevelOriginDefiner)), CanEditMultipleObjects]
    public class LevelOriginDefinerEditor : Editor
    {
        LevelOriginDefiner self;
        public LevelDescriptor level;

        public override VisualElement CreateInspectorGUI()
        {
            self = target as LevelOriginDefiner;
            return base.CreateInspectorGUI();
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            if (self.Level == null)
            {
                EditorGUILayout.HelpBox("Please define a Level", MessageType.Error);
                return;
            }

            if (GUILayout.Button("Save Changes"))
            {
                Undo.RecordObject(self.Level, "Apply LevelOrigin Changes");
                self.Level.ShareOrigin = self.transform.position;
                EditorUtility.SetDirty(self.Level);
            }
        }
    }
}