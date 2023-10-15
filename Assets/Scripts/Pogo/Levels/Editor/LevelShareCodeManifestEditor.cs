using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

namespace Pogo.Levels
{
    [CustomEditor(typeof(LevelShareCodeManifest))]
    public class LevelShareCodeManifestEditor : Editor
    {
        LevelShareCodeManifest self;
        LevelShareCodeGroup[] collation;

        public override VisualElement CreateInspectorGUI()
        {
            self = target as LevelShareCodeManifest;
            if (self.ShareCodes == null)
            {
                self.ShareCodes = new ShareCode[] { };
                EditorUtility.SetDirty(self);
            }

            collation = self.CollateCodesByLevel();
            return base.CreateInspectorGUI();
        }

        public override void OnInspectorGUI()
        {
            foreach(var group in collation)
            {
                using (new EditorGUI.DisabledScope(true))
                {
                    EditorGUILayout.ObjectField(group.Level, typeof(LevelDescriptor), false);

                    using (new EditorGUI.IndentLevelScope())
                    {
                        foreach (var code in group.Codes)
                        {
                            using (new EditorGUILayout.HorizontalScope())
                            {
                                EditorGUILayout.IntField(code.ShareIndex);
                                EditorGUILayout.IntField(code.LevelState.StateId);
                            }
                        }
                    }
                }
            }

            base.OnInspectorGUI();
        }

    }
}