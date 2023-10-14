using Pogo.Challenges;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Pogo.Inspector
{
    public class DevelopChallengeEditorPopup : PopupWindowContent
    {
        public Action<LevelShareIndexManifest, string> DecodePressedAction;
        public LevelShareIndexManifest Manifest;
        public string DecodeString;

        public DevelopChallengeEditorPopup(Action<LevelShareIndexManifest, string> decodePressedAction)
        {
            DecodePressedAction = decodePressedAction;
            DecodeString = "";
        }

        public override Vector2 GetWindowSize()
        {
            return new Vector2(400, 73);
        }

        public override void OnGUI(Rect rect)
        {
            DrawManifestField();
            DrawDecodeStringField();
            EditorGUILayout.Space();
            DrawDecodeButton();
        }

        private void DrawDecodeButton()
        {
            EditorGUI.BeginDisabledGroup(Manifest == null || DecodeString.Length != ChallengeBuilder.PayloadLength);
            if (GUILayout.Button("Decode"))
            {
                DecodePressedAction?.Invoke(Manifest, DecodeString);
            }
            EditorGUI.EndDisabledGroup();
        }

        private void DrawManifestField()
        {
            Manifest = (LevelShareIndexManifest)EditorGUILayout.ObjectField("Manifest:", Manifest, typeof(LevelShareIndexManifest), false);
        }

        private void DrawDecodeStringField()
        {
            DecodeString = EditorGUILayout.TextField("Code:", DecodeString);
        }
    }
}
