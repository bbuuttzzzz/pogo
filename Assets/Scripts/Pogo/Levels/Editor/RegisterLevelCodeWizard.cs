using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;

namespace Pogo.Levels
{
    public class RegisterLevelCodeWizard : EditorWindow
    {
        public LevelShareCodeManifest TargetManifest;
        public LevelState LevelState;
        public int ShareIndex;

        public static void Spawn(LevelShareCodeManifest manifest, LevelState levelState)
        {
            RegisterLevelCodeWizard window = EditorWindow.GetWindow<RegisterLevelCodeWizard>();
            window.TargetManifest = manifest;
            window.LevelState = levelState;
            window.ShareIndex = 0;
        }

        private void OnGUI()
        {
            using (new EditorGUI.DisabledScope(true))
            {
                EditorGUILayout.ObjectField("Target Manifest", TargetManifest, typeof(LevelShareCodeManifest), false);
                EditorGUILayout.ObjectField("Target Level", LevelState.Level, typeof(LevelDescriptor), false);
                EditorGUILayout.IntField("Target StateId", LevelState.StateId);
            }
            EditorGUILayout.Space();

            ShareIndex = EditorGUILayout.IntField("ShareIndex", ShareIndex);


            if (ShareIndex <= 0 || ShareIndex > 255)
            {
                EditorGUILayout.HelpBox($"Use a valid ShareIndex (1 <= x <= 255)", MessageType.Error);
                if (GUILayout.Button("Choose Next Empty"))
                {
                    ShareIndex = TargetManifest.GetNextUnusedShareIndex();
                }

            }
            else if (TargetManifest.TryGetLevelState(ShareIndex, out LevelState existingLevelState))
            {
                if (existingLevelState == LevelState)
                {
                    EditorGUILayout.HelpBox("Success!", MessageType.Info);
                }
                else
                {
                    EditorGUILayout.HelpBox($"That ShareIndex is already in use by {existingLevelState}!", MessageType.Error);
                }
            }
            else
            {
                LevelShareCodeGroup codeGroup = TargetManifest.GetCodeGroupForLevel(LevelState.Level);
                bool codeExists = codeGroup.Codes
                    .Any(c => c.LevelState == LevelState);
                if (codeExists)
                {
                    int shareIndex = codeGroup.Codes
                        .Where(c => c.LevelState == LevelState)
                        .First()
                        .ShareIndex;

                    EditorGUILayout.HelpBox($"This LevelCode is already added under ShareIndex {shareIndex}! Confirming will Override!", MessageType.Warning);
                }

                if (GUILayout.Button("Confirm"))
                {
                    Undo.RecordObject(TargetManifest, "Add LevelCode to Manifest");
                    codeGroup.Codes = codeGroup.Codes
                        .Where(c => c.LevelState != LevelState)
                        .Append(new ShareCode()
                        {
                            LevelState = LevelState,
                            ShareIndex = ShareIndex
                        })
                        .ToList();

                    TargetManifest.UpdateWithGroup(codeGroup);

                    EditorUtility.SetDirty(TargetManifest);
                    this.Close();
                }
            }
        }
    }
}
