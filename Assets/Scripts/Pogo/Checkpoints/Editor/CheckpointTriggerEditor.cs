using Codice.Utils;
using Pogo.Checkpoints;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem.iOS;
using UnityEngine.UIElements;

namespace Pogo
{
    [CustomEditor(typeof(CheckpointTrigger)),CanEditMultipleObjects]
    public class CheckpointTriggerEditor : Editor
    {
        CheckpointTrigger self;

        SerializedProperty m_SkipBehavior;
        SerializedProperty m_OnSkip;
        SerializedProperty m_SkipTarget;

        public override VisualElement CreateInspectorGUI()
        {
            self = target as CheckpointTrigger;
            m_SkipBehavior = serializedObject.FindProperty(nameof(self.SkipBehavior));
            m_OnSkip = serializedObject.FindProperty(nameof(self.OnSkip));
            m_SkipTarget = serializedObject.FindProperty(nameof(self.SkipTarget));

            return base.CreateInspectorGUI();
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            EditorGUILayout.Space();

            if (GUILayout.Button("Move Player Here"))
            {
                PogoGameManager gameManager = FindObjectOfType<PogoGameManager>();

                gameManager._CachedCheckpoint = self.Descriptor;
                PogoGameManagerEditor.SetSpawnPointInEditor(gameManager, self.RespawnPoint);
            }

            EditorGUILayout.Space();

            if (self.Descriptor == null)
            {
                EditorGUILayout.HelpBox("Missing Descriptor!", MessageType.Error);

                return;
            }
            
            bool oldCanSkip = self.Descriptor.CanSkip;
            bool canSkip = EditorGUILayout.Toggle(nameof(self.Descriptor.CanSkip), self.Descriptor.CanSkip);
            if (oldCanSkip != canSkip)
            {
                UpdateCanSkip(canSkip);
            }

            if (canSkip)
            {
                DrawSkipProperties();
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawSkipProperties()
        {
            DrawOverrideCheckpointProperty();
            EditorGUILayout.PropertyField(m_SkipBehavior);
            if (self.SkipBehavior != CheckpointTrigger.SkipBehaviors.LevelChange)
            {
                EditorGUILayout.PropertyField(m_OnSkip);
                EditorGUILayout.PropertyField(m_SkipTarget);
                DrawSkipSelectDropdown();
            }
        }

        private void DrawOverrideCheckpointProperty()
        {
            if (self.Descriptor.OverrideSkipToCheckpoint == null
                && self.Descriptor.CheckpointId.CheckpointType == CheckpointTypes.SidePath)
            {
                EditorGUILayout.HelpBox("You NEED an overrideSkipToCheckpoint for SidePaths!", MessageType.Error);
            }

            CheckpointDescriptor result = (CheckpointDescriptor)EditorGUILayout.ObjectField(
                            "Override Next Checkpoint:",
                            self.Descriptor.OverrideSkipToCheckpoint,
                            typeof(CheckpointDescriptor),
                            false);

            if (result == self.Descriptor.OverrideSkipToCheckpoint) return;

            Undo.RecordObject(self.Descriptor, "Update OverrideSkipToCheckpoint");
            self.Descriptor.OverrideSkipToCheckpoint = result;
            EditorUtility.SetDirty(self.Descriptor);
        }

        private void DrawSkipSelectDropdown()
        {
            if (EditorGUILayout.DropdownButton(new GUIContent("Target SkipPoint..."), FocusType.Passive))
            {
                var spawnPoints = GameObject.FindGameObjectsWithTag("SkipPoint");

                var itempaths = new List<(GameObject item, string path)>();
                foreach (var spawnPoint in spawnPoints)
                {
                    string path = AnimationUtility.CalculateTransformPath(spawnPoint.transform, null);
                    path = path.Replace("/", " -> ");

                    itempaths.Add((spawnPoint, path));
                }

                GenericMenu menu = new GenericMenu();
                foreach (var itempath in itempaths.OrderBy(ip => ip.path))
                {
                    if (itempath.item.scene == self.gameObject.scene)
                    {
                        menu.AddItem(new GUIContent(itempath.path), false, () =>
                        {
                            Undo.RecordObject(self, "Select SkipTarget");
                            self.SkipTarget = itempath.item.transform;
                            PrefabUtility.RecordPrefabInstancePropertyModifications(self);
                        });
                    }
                    else
                    {
                        menu.AddItem(new GUIContent($"[CLONE] {itempath.path}"), false, () =>
                        {
                            var clone = Instantiate(itempath.item, self.transform);
                            clone.transform.position = itempath.item.transform.position;
                            clone.transform.rotation = itempath.item.transform.rotation;
                            clone.name = "EXIT SkipPoint Clone";

                            using (new UndoScope("Clone & Select SkipTarget"))
                            {
                                Undo.RegisterCreatedObjectUndo(clone, "CLONE Select SkipTarget");
                                Undo.RecordObject(self, "Select SkipTarget");
                                self.SkipTarget = clone.transform;
                                PrefabUtility.RecordPrefabInstancePropertyModifications(self);
                            }
                        });
                    }
                    
                }
                menu.DropDown(new Rect(Event.current.mousePosition.x, Event.current.mousePosition.y, 0f, 0f));
            }
        }

        private void UpdateCanSkip(bool newValue)
        {
            Undo.RecordObject(self.Descriptor, "Toggle CanSkip");
            self.Descriptor.CanSkip = newValue;
            EditorUtility.SetDirty(self.Descriptor);
        }
    }

}
