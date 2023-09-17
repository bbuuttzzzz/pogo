using Pogo.Challenges;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using WizardUtils.Saving;

namespace Pogo.Inspector
{
    [CustomEditor(typeof(DeveloperChallenge))]
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

        private void onDecodePressed(LevelManifest manifest, string decodeString)
        {
            var challenge = ChallengeBuilder.DecodeChallenge(decodeString, manifest, out ChallengeBuilder.DecodeFailReason failReason);
            if (challenge != null)
            {
                Undo.RecordObject(self, "Decode Challenge");
                challenge.ChallengeType = Challenge.ChallengeTypes.PlayDeveloper;
                challenge.DeveloperChallenge = self;
                self.Challenge = challenge;
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
            if (self.PlayerTimeSaveValue_Legacy == null)
            {
                EditorGUILayout.HelpBox("Missing Save Value!", MessageType.Error);
            }
            if (GUILayout.Button(new GUIContent("Regenerate SaveValueDescriptor")))
            {
                Undo.SetCurrentGroupName("Generate SaveValueDescriptor");
                int undoGroup = Undo.GetCurrentGroup();

                RemoveDanglingBestTimes();

                var parentPath = AssetDatabase.GetAssetPath(self);
                var newSaveValue = ScriptableObject.CreateInstance<SaveValueDescriptor>();
                newSaveValue.name = "sv_" + self.name;
                newSaveValue.Key = "time_" + self.name;
                newSaveValue.DeveloperNote = "Best time in milliseconds";
                newSaveValue.DefaultValue = Challenge.WORST_TIME.ToString();
                AssetDatabase.AddObjectToAsset(newSaveValue, parentPath);
                Undo.RegisterCreatedObjectUndo(newSaveValue, "create");

                self.PlayerTimeSaveValue_Legacy = newSaveValue;
                Undo.RecordObject(self, "set");

                Undo.CollapseUndoOperations(undoGroup);
            }
        }

        private void RemoveDanglingBestTimes()
        {
            var parentPath = UnityEditor.AssetDatabase.GetAssetPath(self);

            var assets = UnityEditor.AssetDatabase.LoadAllAssetsAtPath(parentPath);
            foreach (var asset in assets)
            {
                if (!(asset is SaveValueDescriptor)) continue;

                Undo.DestroyObjectImmediate(asset);
            }
        }
    }
}