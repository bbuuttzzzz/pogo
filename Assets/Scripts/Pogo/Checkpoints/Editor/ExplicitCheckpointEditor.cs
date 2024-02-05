using Codice.Client.Commands;
using Pogo.Checkpoints;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Pogo
{

    [CustomEditor(typeof(ExplicitCheckpoint)), CanEditMultipleObjects]
    public class ExplicitCheckpointEditor : CheckpointEditor
    {
        new protected ExplicitCheckpoint self => base.self as ExplicitCheckpoint;

        SerializedProperty m_SkipBehavior;
        SerializedProperty m_OnSkip;
        SerializedProperty m_SkipTarget;

        public override VisualElement CreateInspectorGUI()
        {
            var element = base.CreateInspectorGUI();
            m_SkipBehavior = serializedObject.FindProperty(nameof(self.SkipBehavior));
            m_OnSkip = serializedObject.FindProperty(nameof(self.OnSkip));
            m_SkipTarget = serializedObject.FindProperty(nameof(self.SkipTarget));
            return element;
        }

        protected override bool TryValidate(out string failReason)
        {
            if (self.Descriptor == null)
            {
                failReason = "Missing Descriptor!";
                return false;
            }

            failReason = default;
            return true;
        }

        protected override void DrawSkipProperties()
        {
            DrawOverrideCheckpointProperty();
            EditorGUILayout.PropertyField(m_SkipBehavior);
            if (self.SkipBehavior != ExplicitCheckpoint.SkipBehaviors.LevelChange)
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


        protected override void UpdateCanSkip(bool newValue)
        {
            Undo.RecordObject(self.Descriptor, "Toggle CanSkip");
            self.Descriptor.CanSkip = newValue;
            EditorUtility.SetDirty(self.Descriptor);
        }
    }
}
