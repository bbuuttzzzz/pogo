using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using System.Linq;

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

            return base.CreateInspectorGUI();
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            collation = self.CollateCodesByLevel();

            int invalidCount = self.ShareCodes.Where(x => x.LevelState.Level == null).Count();
            if (invalidCount > 0)
            {
                EditorGUILayout.HelpBox($"{invalidCount} LevelStates missing a LevelDescriptor!", MessageType.Error);
                if (GUILayout.Button("Fix Now"))
                {
                    Undo.RecordObject(self, "Fix Share Codes Missing LevelDescriptors");
                    self.ShareCodes = self.ShareCodes
                        .Where(x => x.LevelState.Level != null)
                        .ToArray();
                    EditorUtility.SetDirty(self);
                }
            }

            foreach (var group in collation)
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
                                EditorGUILayout.IntField("Share Index", code.ShareIndex);
                                EditorGUILayout.IntField("State Id", code.LevelState.StateId);
                            }
                        }
                    }
                }
            }
        }

    }
}