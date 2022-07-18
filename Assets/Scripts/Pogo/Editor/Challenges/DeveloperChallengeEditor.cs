using Pogo.Challenges;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Pogo.Inspector
{
    [CustomEditor(typeof(DeveloperChallenge))]
    public class DeveloperChallengeEditor : Editor
    {
        DeveloperChallenge self;
        DevelopChallengeEditorPopup popup;

        public override VisualElement CreateInspectorGUI()
        {
            popup = new DevelopChallengeEditorPopup(onDecodePressed);
            return base.CreateInspectorGUI();
        }

        private void onDecodePressed(LevelManifest manifest, string decodeString)
        {
            self = target as DeveloperChallenge;
            var challenge = ChallengeBuilder.DecodeChallenge(decodeString, manifest, out ChallengeBuilder.DecodeFailReason failReason);
            if (challenge != null)
            {
                Undo.RecordObject(self, "Decode Challenge");
                self.Challenge = challenge;
            }
            else
            {
                Debug.LogError($"Decode Failed: {failReason}");
            }
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (EditorGUILayout.DropdownButton(new GUIContent("Load from Code..."), FocusType.Passive))
            {
                var rect = new Rect(Event.current.mousePosition.x, Event.current.mousePosition.y, 0f, 0f);
                popup.DecodeString = "";
                UnityEditor.PopupWindow.Show(rect, popup);
            }
        }
    }
}