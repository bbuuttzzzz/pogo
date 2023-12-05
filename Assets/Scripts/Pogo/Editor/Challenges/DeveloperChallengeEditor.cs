using Pogo.Challenges;
using Pogo.Levels;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using WizardUtils.Saving;

namespace Pogo.Inspector
{
    [CustomEditor(typeof(DeveloperChallenge)), CanEditMultipleObjects]
    public class DeveloperChallengeEditor : Editor
    {
        DeveloperChallenge self;
        DevelopChallengeEditorPopup popup;

        public override VisualElement CreateInspectorGUI()
        {
            self = target as DeveloperChallenge;
            popup = new DevelopChallengeEditorPopup(onDecodePressed);
            return base.CreateInspectorGUI();
        }

        private void onDecodePressed(LevelShareCodeManifest manifest, string decodeString)
        {
            var challenge = ChallengeBuilder.DecodeChallenge(decodeString, manifest, out ChallengeBuilder.DecodeFailReason failReason);
            if (challenge != null)
            {
                Undo.RecordObject(self, "Decode Challenge");
                challenge.ChallengeType = Challenge.ChallengeTypes.PlayDeveloper;
                challenge.DeveloperChallenge = self;
                self.Challenge = challenge;
                EditorUtility.SetDirty(self);
            }
            else
            {
                Debug.LogError($"Decode Failed: {failReason}");
            }
        }

        public override void OnInspectorGUI()
        {
            if (EditorGUILayout.DropdownButton(new GUIContent("Load from Code..."), FocusType.Passive))
            {
                var rect = new Rect(Event.current.mousePosition.x, Event.current.mousePosition.y, 0f, 0f);
                popup.DecodeString = "";
                UnityEditor.PopupWindow.Show(rect, popup);
            }

            base.OnInspectorGUI();
        }
    }
}