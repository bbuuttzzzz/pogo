using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine.UIElements;

namespace Pogo.Levels
{
    [CustomEditor(typeof(LevelStateSubListener), true), CanEditMultipleObjects]
    public class LevelStateSubListenerEditor : Editor
    {
        protected LevelStateSubListener self;

        public override VisualElement CreateInspectorGUI()
        {
            self = target as LevelStateSubListener;
            return base.CreateInspectorGUI();
        }

        public override void OnInspectorGUI()
        {
            if (self.Parent == null)
            {
                var parent = self.GetComponentInParent<LevelStateListener>();
                if (parent != null)
                {
                    self.Parent = parent;
                    EditorUtility.SetDirty(self);
                }
                EditorGUILayout.HelpBox("Missing Parent!", MessageType.Error);
            }
            else
            {
                using (new EditorGUI.DisabledScope(true))
                {
                    EditorGUILayout.ObjectField("Parent Target Level", self.Parent.TargetLevel,typeof(LevelDescriptor) , false);
                }
            }

            DrawDefaultInspector();
        }
    }
}
