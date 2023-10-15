using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using System.Linq;

namespace Pogo.Levels
{
    [CustomEditor(typeof(LevelDescriptor))]
    public class LevelDescriptorEditor : Editor
    {
        LevelDescriptor self;
        private ManifestShareCodeGroup[] cachedManifestShareCodeGroups;


        public override VisualElement CreateInspectorGUI()
        {
            self = target as LevelDescriptor;
            RecalculateCachedShareCodeGroups();
            return base.CreateInspectorGUI();
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            DrawRegisterButtons();
        }

        private void RecalculateCachedShareCodeGroups()
        {
            cachedManifestShareCodeGroups = AssetDatabase.FindAssets($"t:{typeof(LevelShareCodeManifest).Name}")
                .Select(id => AssetDatabase.LoadAssetAtPath<LevelShareCodeManifest>(AssetDatabase.GUIDToAssetPath(id)))
                .Select(m => new ManifestShareCodeGroup()
                {
                    manifest = m,
                    group = m.GetCodesForLevel(self)
                })
                .OrderBy(m => m.manifest.EditorDisplayPriority)
                .ToArray();
        }

        private void DrawRegisterButtons()
        {
            GUILayout.Label("LevelStates");
            for (int n = 0; n < self.LevelStatesCount; n++)
            {
                LevelState levelState = new LevelState()
                {
                    Level = self,
                    StateId = n
                };

                int? cachedShareIndex = null;
                bool codeError = false;

                EditorGUILayout.LabelField("StateId", n.ToString());
                using (new EditorGUI.IndentLevelScope())
                {
                    foreach(var manifestGroup in cachedManifestShareCodeGroups)
                    {
                        bool hasCode = manifestGroup.group.TryGetCode(levelState, out ShareCode code);

                        if (hasCode && cachedShareIndex.HasValue
                            && cachedShareIndex != code.ShareIndex)
                        {
                            codeError = true;
                        }
                        else if (hasCode && !cachedShareIndex.HasValue)
                        {
                            cachedShareIndex = code.ShareIndex;
                        }

                        using (new EditorGUILayout.HorizontalScope())
                        {
                            using (new EditorGUI.DisabledScope(true))
                            {
                                EditorGUILayout.ObjectField(manifestGroup.manifest, typeof(LevelShareCodeManifest), false);
                            }
                            bool shouldHaveCode = EditorGUILayout.Toggle(hasCode);
                            EditorGUILayout.LabelField("Share Index", code.ShareIndex.ToString());
                        }
                    }

                    if (codeError)
                    {
                        EditorGUILayout.HelpBox("Inconsistent Share Indexes!!! Please Fix!!!", MessageType.Error);
                    }
                }
            }
        }

        private struct ManifestShareCodeGroup
        {
            public LevelShareCodeManifest manifest;
            public LevelShareCodeGroup group;
        }
    }
}