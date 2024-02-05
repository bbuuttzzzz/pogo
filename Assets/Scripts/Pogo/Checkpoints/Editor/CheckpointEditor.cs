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

        public override VisualElement CreateInspectorGUI()
        {
            self = target as Checkpoint;

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

        protected abstract void DrawSkipProperties();



        protected virtual void UpdateCanSkip(bool newValue)
        {
            Undo.RecordObject(self, "Toggle CanSkip");
            self.CanSkip = newValue;
            EditorUtility.SetDirty(self);
        }
    }

}
