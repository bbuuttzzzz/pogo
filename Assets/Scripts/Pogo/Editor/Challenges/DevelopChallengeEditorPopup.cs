using Pogo.Challenges;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Pogo.Inspector
{
    public class DevelopChallengeEditorPopup : PopupWindowContent
    {
        public Action<LevelManifest, string> DecodePressedAction;
        public LevelManifest Manifest;
        public string DecodeString;

        public DevelopChallengeEditorPopup(Action<LevelManifest, string> decodePressedAction)
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
            Manifest = (LevelManifest)EditorGUILayout.ObjectField("Manifest:", Manifest, typeof(LevelManifest), false);
        }

        private void DrawDecodeStringField()
        {
            DecodeString = EditorGUILayout.TextField("Code:", DecodeString);
        }
    }
}
