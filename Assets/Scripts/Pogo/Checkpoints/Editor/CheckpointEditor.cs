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
    public abstract class CheckpointEditor : Editor
    {
        protected Checkpoint self;

        SerializedProperty m_SkipBehavior;
        SerializedProperty m_OnSkip;
        SerializedProperty m_SkipTarget;

        public override VisualElement CreateInspectorGUI()
        {
            self = target as Checkpoint;
            m_SkipBehavior = serializedObject.FindProperty(nameof(self.SkipBehavior));
            m_OnSkip = serializedObject.FindProperty(nameof(self.OnSkip));
            m_SkipTarget = serializedObject.FindProperty(nameof(self.SkipTarget));

            return base.CreateInspectorGUI();
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (!TryValidate(out var failReason))
            {
                EditorGUILayout.HelpBox(failReason, MessageType.Error);

                return;
            }
            
            EditorGUILayout.Space();

            if (GUILayout.Button("Move Player Here"))
            {
                PogoGameManager gameManager = FindObjectOfType<PogoGameManager>();

                if (target is ExplicitCheckpoint explicitCheckpoint)
                {
                    gameManager._CachedCheckpoint = explicitCheckpoint.Descriptor;
                }
                PogoGameManagerEditor.SetSpawnPointInEditor(gameManager, self.RespawnPoint);
            }

            EditorGUILayout.Space();
            
            bool oldCanSkip = self.CanSkip;
            bool canSkip = EditorGUILayout.Toggle(nameof(self.CanSkip), self.CanSkip);
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

        protected abstract bool TryValidate(out string failReason);

        protected void DrawSkipProperties()
        {
            DrawOverrideCheckpointProperty();
            EditorGUILayout.PropertyField(m_SkipBehavior);
            if (self.SkipBehavior != SkipBehaviors.LevelChange)
            {
                EditorGUILayout.PropertyField(m_OnSkip);
                EditorGUILayout.PropertyField(m_SkipTarget);
                DrawSkipSelectDropdown();
            }
        }

        protected abstract void DrawOverrideCheckpointProperty();
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

        protected virtual void UpdateCanSkip(bool newValue)
        {
            Undo.RecordObject(self, "Toggle CanSkip");
            self.CanSkip = newValue;
            EditorUtility.SetDirty(self);
        }
    }

}
