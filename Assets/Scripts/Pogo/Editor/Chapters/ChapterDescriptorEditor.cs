using Pogo.Checkpoints;
using System;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using WizardUtils;

namespace Pogo
{
    [CustomEditor(typeof(ChapterDescriptor))]
    public class ChapterDescriptorEditor : Editor
    {
        ChapterDescriptor self;

        public override VisualElement CreateInspectorGUI()
        {
            self = target as ChapterDescriptor;
            return base.CreateInspectorGUI();
        }


        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (self.MainPathCheckpoints.Length > 0
                && self.MainPathCheckpoints[^1] != null
                && GUILayout.Button("+1 Main Checkpoint"))
            {
                var newCheckpoint = DuplicateCheckpoint(self.MainPathCheckpoints[^1]);

                ArrayHelper.InsertAndResize(ref self.MainPathCheckpoints, newCheckpoint);

                self.UpdateMainCheckpoint(self.MainPathCheckpoints.Length - 1);
            }

            if (self.SidePathCheckpoints.Length > 0
                && self.SidePathCheckpoints[^1] != null
                && GUILayout.Button("+1 Side Checkpoint"))
            {
                Undo.SetCurrentGroupName("+1 Side Checkpoint");
                int group = Undo.GetCurrentGroup();

                var newCheckpoint = DuplicateCheckpoint(self.SidePathCheckpoints[^1]);
                Undo.RegisterCreatedObjectUndo(newCheckpoint, "create checkpoint");

                ArrayHelper.InsertAndResize(ref self.SidePathCheckpoints, newCheckpoint);
                self.UpdateMainCheckpoint(self.SidePathCheckpoints.Length - 1);
                Undo.RecordObject(this, "add new checkpoint");
                Undo.RecordObject(newCheckpoint, "setup new checkpoint");

                Undo.CollapseUndoOperations(group);
            }
        }

        public CheckpointDescriptor DuplicateCheckpoint(CheckpointDescriptor original)
        {
            Regex regex = new Regex("(.*?)(\\d+)$", RegexOptions.Compiled);
            Match match = regex.Match(original.name);
            if (!match.Success)
            {
                Debug.LogError($"Checkpoint {original.name} had a badly formatted name", original);
                return null;
            }
            int index = int.Parse(match.Groups[2].Value);

            string newName = $"{match.Groups[1]}{index+1}";

            var path = AssetDatabase.GetAssetPath(original);
            var newPath = $"{Directory.GetParent(path)}{Path.DirectorySeparatorChar}{newName}";

            AssetDatabase.CopyAsset(path, newPath);
            CheckpointDescriptor newAsset = AssetDatabase.LoadAssetAtPath(newPath, typeof(CheckpointDescriptor)) as CheckpointDescriptor;

            return newAsset;
        }
    }
}
