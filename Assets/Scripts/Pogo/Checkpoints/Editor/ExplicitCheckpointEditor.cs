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

        protected override void DrawOverrideCheckpointProperty()
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

        protected override void UpdateCanSkip(bool newValue)
        {
            Undo.RecordObject(self.Descriptor, "Toggle CanSkip");
            self.Descriptor.CanSkip = newValue;
            EditorUtility.SetDirty(self.Descriptor);
        }
    }
}
