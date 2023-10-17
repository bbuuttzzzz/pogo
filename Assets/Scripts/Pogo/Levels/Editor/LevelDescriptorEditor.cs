using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using System.Linq;
using System;

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
            return base.CreateInspectorGUI();
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            RecalculateCachedShareCodeGroups();
            DrawRegisterButtons();
        }

        private void RecalculateCachedShareCodeGroups()
        {
            cachedManifestShareCodeGroups = AssetDatabase.FindAssets($"t:{typeof(LevelShareCodeManifest).Name}")
                .Select(id => AssetDatabase.LoadAssetAtPath<LevelShareCodeManifest>(AssetDatabase.GUIDToAssetPath(id)))
                .Select(m => new ManifestShareCodeGroup()
                {
                    manifest = m,
                    group = m.GetCodeGroupForLevel(self)
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
                bool conflictingCodesError = false;

                EditorGUILayout.LabelField("StateId", n.ToString());
                using (new EditorGUI.IndentLevelScope())
                {
                    foreach(var manifestGroup in cachedManifestShareCodeGroups)
                    {
                        bool hasCode = manifestGroup.group.TryGetCode(levelState, out ShareCode existingCode);

                        if (hasCode && cachedShareIndex.HasValue
                            && cachedShareIndex != existingCode.ShareIndex)
                        {
                            conflictingCodesError = true;
                        }
                        else if (hasCode && !cachedShareIndex.HasValue)
                        {
                            cachedShareIndex = existingCode.ShareIndex;
                        }

                        using (new EditorGUILayout.HorizontalScope())
                        {
                            using (new EditorGUI.DisabledScope(true))
                            {
                                EditorGUILayout.ObjectField(manifestGroup.manifest, typeof(LevelShareCodeManifest), false);
                            }

                            bool shouldHaveCode = hasCode;
                            using (new EditorGUI.DisabledScope(conflictingCodesError))
                            {
                                shouldHaveCode = EditorGUILayout.Toggle(hasCode);
                            }
                            if (hasCode)
                            {
                                EditorGUILayout.LabelField("Share Index", existingCode.ShareIndex.ToString());
                            }
                            else
                            {
                                EditorGUILayout.LabelField("", "");
                            }

                            if (shouldHaveCode && !hasCode
                                && !conflictingCodesError)
                            {
                                if (cachedShareIndex.HasValue)
                                {
                                    AddCodeToManifest(manifestGroup, levelState, cachedShareIndex.Value);
                                }
                                else
                                {
                                    SpawnAddCodeToManifestWizard(manifestGroup, levelState); ;
                                }
                            }
                            else if (!shouldHaveCode && hasCode
                                && !conflictingCodesError)
                            {
                                RemoveCodeFromManifest(manifestGroup, existingCode);
                            }
                        }
                    }

                    if (conflictingCodesError)
                    {
                        EditorGUILayout.HelpBox("Inconsistent Share Indexes!!! Please Fix!!!", MessageType.Error);
                    }
                }
            }

            if (GUILayout.Button("+ Add State"))
            {
                Undo.RecordObject(self, "Add State");
                self.LevelStatesCount++;
                EditorUtility.SetDirty(self);
            }
        }

        private void AddCodeToManifest(ManifestShareCodeGroup manifestGroup, LevelState levelState, int shareIndex)
        {
            Undo.RecordObject(manifestGroup.manifest, "Remove LevelCode from Manifest");

            manifestGroup.group.Codes.Add(new ShareCode()
            {
                LevelState = levelState,
                ShareIndex = shareIndex
            });

            manifestGroup.manifest.UpdateWithGroup(manifestGroup.group);
            EditorUtility.SetDirty(manifestGroup.manifest);
        }

        private void SpawnAddCodeToManifestWizard(ManifestShareCodeGroup manifestGroup, LevelState levelState)
        {
            RegisterLevelCodeWizard.Spawn(manifestGroup.manifest, levelState);
        }

        private void RemoveCodeFromManifest(ManifestShareCodeGroup manifestGroup, ShareCode code)
        {
            Undo.RecordObject(manifestGroup.manifest, "Remove LevelCode from Manifest");

            manifestGroup.group.Codes.Remove(code);
            manifestGroup.manifest.UpdateWithGroup(manifestGroup.group);
            EditorUtility.SetDirty(manifestGroup.manifest);
        }

        private struct ManifestShareCodeGroup
        {
            public LevelShareCodeManifest manifest;
            public LevelShareCodeGroup group;
        }
    }
}